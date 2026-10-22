
using UnityEngine;

public class RevolverLogic : BaseWeapon
{
    [Header("Revolver")]
    [SerializeField] private Animator animator;

    [Header("Animation triggers")]
    [SerializeField] private string fireTrigger = "Fire";
    [SerializeField] private string reloadTrigger = "Reload";

    protected override void OnAttackExecuted()
    {
        base.OnAttackExecuted();

        if (animator != null)
            animator.SetTrigger(fireTrigger);
    }

    protected override void OnReloadStarted()
    {
        base.OnReloadStarted();

        if (animator != null)
            animator.SetTrigger(reloadTrigger);
    }

    public override void StartAltAttack()
    {
        if (IsReloading)
            return;

        // Здесь позднее будет альтернативная атака монетой.
    }
}
