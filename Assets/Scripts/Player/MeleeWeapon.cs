using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Combat Settings")]
    public int damageAmount = 25;
    public float attackRate = 0.5f;
    private bool isAttacking = false;

    [Header("References")]
    public Animator weaponAnimator;
    public GameObject hitbox;

    public void Awake()
    {
        hitbox.SetActive(false);
    }

    // 2. This function is called automatically by the "Player Input" 
    // component if "Behavior" is set to "Send Messages"
    public void OnAttack(InputValue value)
    {
        // The Input System calls this method automatically 
        // whenever the button defined as 'Attack' is pressed.
        if (value.isPressed && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        weaponAnimator.SetTrigger("Swing");
        yield return new WaitForSeconds(0.2f);

        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        hitbox.SetActive(false);

        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }

    public void ProcessHit(Collider other)
    {
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount);
            Debug.Log("Dealt " + damageAmount + " damage to enemy");
            hitbox.SetActive(false);
        }
    }
}