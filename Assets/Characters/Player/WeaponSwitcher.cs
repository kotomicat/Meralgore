using Unity.VisualScripting;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    private Animator anim; 
    private int changeHash;

    public GameObject currentWeapon;
    public Weapon weapon;
    


    void Start()
    {

        anim = GetComponent<Animator>();
        changeHash = Animator.StringToHash("Change");
        ChangeWeapon(0);
    }

    public void ChangeWeapon(int weaponNum)
    {
        if (weaponNum < transform.childCount)
        {
            anim.SetTrigger(changeHash);
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
