using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class JarAbility : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject jarPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float throwForce = 10f;

    [Header("Jars")]
    public int numJars;

    private bool isThrowing = false;

    void Awake()
    {
        numJars = 0;
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed && !isThrowing && numJars > 0)
        {
            StartCoroutine(ThrowRoutine());
        }
    }

    public void GrantJar(int jarAmount)
    {
        numJars += jarAmount;
    }

    private IEnumerator ThrowRoutine()
    {
        isThrowing = true;

        // Throwing Logic
        GameObject jar = Instantiate(jarPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = jar.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * throwForce, ForceMode.Impulse);

        // PLAY THROW SOUND
        SoundManager.PlaySound(SoundType.Weapon_Jar_Fire);


        numJars--;

        // Cooldown
        yield return new WaitForSeconds(1f);
        isThrowing = false;
    }
    private void OnDisable()
    {
        isThrowing = false;
    }

    private void OnEnable()
    {
        isThrowing = false;
    }
}