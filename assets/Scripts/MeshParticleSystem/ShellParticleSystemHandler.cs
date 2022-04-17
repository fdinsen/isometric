using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

[RequireComponent(typeof(MeshParticleSystem))]
public class ShellParticleSystemHandler : MonoBehaviour
{
    public static ShellParticleSystemHandler Instance { get; private set; }

    [SerializeField] private float _minShellSpeed = 2f;
    [SerializeField] private float _maxShellSpeed = 1f;
    [SerializeField] private float _shellSlowdownFactor = 3.5f;
    [SerializeField] private Vector3 _shellSize = Vector3.one;

    private MeshParticleSystem meshParticleSystem;
    private List<Single> singleList;

    private void Awake()
    {
        Instance = this;
        singleList = new List<Single>();
        meshParticleSystem = GetComponent<MeshParticleSystem>();
    }

    private void Start()
    {
        IWeapon.OnPlayerShoot += IWeapon_OnPlayerShoot;
    }

    private void Update()
    {
        for(int i = 0; i < singleList.Count; i++)
        {
            Single single = singleList[i];
            single.Update();
            if (single.IsMovemementComplete())
            {
                singleList.RemoveAt(i);
                i--;
            }
        }
    }
    public void IWeapon_OnPlayerShoot(IWeapon.OnPlayerShootEventArgs e)
    {
        Vector3 quadPosition = e.gunEndpointPosition;
        Vector3 quadSize = new Vector3(1f, 1f);

        Vector3 shootDir = (e.gunPosition - e.gunEndpointPosition).normalized;
        //quadPosition += (shootDir * -1f) * 8f;

        Vector3 shellMoveDir = UtilsClass.ApplyRotationToVector(shootDir, Random.Range(-95f, -85f));
        SpawnShell(quadPosition, shellMoveDir);
    }

    public void SpawnShell(Vector3 position, Vector3 direction)
    {
        singleList.Add(new Single(position, direction, _shellSize, _minShellSpeed, _maxShellSpeed, _shellSlowdownFactor, meshParticleSystem));
    }

    private class Single
    {
        private MeshParticleSystem meshParticleSystem;
        private Vector3 position;
        private Vector3 direction;
        private int quadIndex;
        private Vector3 quadSize;
        private float rotation;
        private float moveSpeed;
        private float slowDownFactor;


        public Single(Vector3 position, Vector3 direction, Vector3 quadSize, float minMoveSpeed, float maxMoveSpeed, float slowDownFactor, MeshParticleSystem meshParticleSystem)
        {
            this.position = position;
            this.direction = direction;
            this.quadSize = quadSize;
            this.slowDownFactor = slowDownFactor;
            this.meshParticleSystem = meshParticleSystem;

            rotation = Random.Range(0,360);
            moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);

            quadIndex = meshParticleSystem.AddQuad(position,rotation, quadSize, true, 0);
        }

        public void Update()
        {
            position += moveSpeed * Time.deltaTime * direction;
            rotation += 360f * (moveSpeed / 10f) * Time.deltaTime;

            meshParticleSystem.UpdateQuad(quadIndex, position, rotation, quadSize, true, 0);

            moveSpeed -= moveSpeed * slowDownFactor * Time.deltaTime;
        }

        public bool IsMovemementComplete()
        {
            return moveSpeed < .1f;
        }
    }
}
