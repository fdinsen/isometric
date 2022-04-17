using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

[RequireComponent(typeof(MeshParticleSystem))]
public class BloodParticleSystemHandler : MonoBehaviour
{
    public static BloodParticleSystemHandler Instance { get; private set; }

    [SerializeField] private float _minBloodSpeed = 2f;
    [SerializeField] private float _maxBloodSpeed = 1f;
    [SerializeField] private int _minAmountOfBloods = 1;
    [SerializeField] private int _maxAmountOfBloods = 5;
    [SerializeField] private float _bloodSlowdownFactor = 3.5f;
    [SerializeField] private Vector3 _bloodSize = Vector3.one;

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
        EnemyHealth.OnAnEnemyDamaged += EnemyHealth_OnAnEnemyDamaged;
    }

    private void Update()
    {
        for (int i = 0; i < singleList.Count; i++)
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
    public void EnemyHealth_OnAnEnemyDamaged(EnemyHealth.EnemyHealthEventArgs e)
    {
        Vector3 quadPosition = e.pos;

        SpawnBlood(quadPosition, e.hitdir);
    }

    public void SpawnBlood(Vector3 position, Vector3 direction)
    {
        int bloodParticleCount = Random.Range(_minAmountOfBloods, _maxAmountOfBloods);
        for(int i = 0; i < bloodParticleCount; i++)
        {
            singleList.Add(
                new Single(position,
                           UtilsClass.ApplyRotationToVector(direction, Random.Range(-15f, 15f)), 
                           _bloodSize, 
                           _minBloodSpeed, 
                           _maxBloodSpeed, 
                           _bloodSlowdownFactor, 
                           meshParticleSystem));
        }
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
        private int uvIndex;


        public Single(Vector3 position, Vector3 direction, Vector3 quadSize, float minMoveSpeed, float maxMoveSpeed, float slowDownFactor, MeshParticleSystem meshParticleSystem)
        {
            this.position = position;
            this.direction = direction;
            this.quadSize = quadSize;
            this.slowDownFactor = slowDownFactor;
            this.meshParticleSystem = meshParticleSystem;

            rotation = Random.Range(0, 360);
            moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
            uvIndex = Random.Range(0, meshParticleSystem.GetFrameCount());

            quadIndex = meshParticleSystem.AddQuad(position, rotation, quadSize, true, uvIndex);
        }

        public void Update()
        {
            position += moveSpeed * Time.deltaTime * direction;
            rotation += 360f * (moveSpeed / 10f) * Time.deltaTime;

            meshParticleSystem.UpdateQuad(quadIndex, position, rotation, quadSize, true, uvIndex);

            moveSpeed -= moveSpeed * slowDownFactor * Time.deltaTime;
        }

        public bool IsMovemementComplete()
        {
            return moveSpeed < .1f;
        }

        public void DestroySelf()
        {
            meshParticleSystem.DestroyQuad(quadIndex);
        }
    }
}