using UnityEngine;

public class WeaponsHandler : MonoBehaviour
{

    public int SelectedWeapon = 0;
    private PlayerInput inputActions;
    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
        inputActions = new PlayerInput();
        inputActions.Combat.Enable();
        inputActions.Combat.SwitchWeapons.performed += ctx => SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        

    }


    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == SelectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }
}
