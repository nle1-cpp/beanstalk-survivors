using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] public GameObject meleeWeaponObject;
    [SerializeField] private GameObject rocketLauncherObject;
    [SerializeField] private GameObject jarObject;

    void Start()
    {
        SwitchToMelee();
    }

    public void SwitchToMelee()
    {
        rocketLauncherObject.SetActive(false);
        jarObject.SetActive(false);
        meleeWeaponObject.SetActive(true);
    }

    public void SwitchToRocket()
    {
        meleeWeaponObject.SetActive(false);
        jarObject.SetActive(false);
        rocketLauncherObject.SetActive(true);

    }

    public void SwitchToJar()
    {
        rocketLauncherObject.SetActive(false);
        meleeWeaponObject.SetActive(false);
        jarObject.SetActive(true);
    }

    // Controls
    public void OnPrevious(InputValue value) // 1
    {
        SwitchToMelee();
    }
    public void OnNext(InputValue value) // 2
    {
        SwitchToRocket();
    }

    public void OnJar(InputValue value) // 3
    {
        SwitchToJar();
    }
}