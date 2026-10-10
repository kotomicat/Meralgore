using Unity.VisualScripting;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject currentWeapon;
    public Weapon weapon;
    


    void Start()
    {
//активация первого оружия по индексу
        ChangeWeapon(0);
    }

    public void ChangeWeapon(int weaponNum)
    {
        if (weaponNum < transform.childCount)
        {
            int i = 0;
            foreach (Transform weapon in transform)
            {
                // активность оружия зависит от того, совпадает ли его индекс с выбранным номером оружия
                weapon.gameObject.SetActive(i == weaponNum);
                i++;
            }

            
            currentWeapon = transform.GetChild(weaponNum).gameObject;
            weapon = currentWeapon.GetComponent<Weapon>();
        }
    }

}
