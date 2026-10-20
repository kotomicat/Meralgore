using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Audio.ControlContext;

public class Hammer : Weapon
{
    [Header("UI")]
    [SerializeField] GameObject rangeBar;
    Image rBarImage;

    [Header("Damage Scaling")]
    [SerializeField] float scaleSpeed = 2;
    [SerializeField] float minDamage = 1f;
    public float targetDamage = 0.1f;

    [Header("Other")]
    private bool charging = false;
    private Coroutine attackProcessCoroutine;

    void Start()
    {
        attack = false;
        rBarImage = rangeBar.GetComponent<Image>();
        targetDamage = minDamage;
    }

    private void OnEnable()
    {
        attack = false;
        // защита от ошибки в случае неициалированного Animator
        if (animator != null)
        {
            // сброс старых триггеров
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Charging");

            // анимация экипировки оружия
            animator.SetTrigger("Equip");
        }
    }

    void Update()
    {
        if (!attack && charging)
        {
            ChargeUpdate();
        }
    }

    private void ChargeUpdate()
    {
        if (Mathf.Abs(targetDamage - damage) > 0.001f)
        {
            targetDamage = Mathf.MoveTowards(targetDamage, damage, Time.deltaTime * scaleSpeed);
            rBarImage.fillAmount = targetDamage / damage;
        }
    }

    public override void StartAttack()
    {
        charging = true;
        if (attack) return;
        
        StartCharging();
    }

    public override void EndAttack()
    {
        charging = false;
        StopCharging();

        // если в текущий момент мы не атакуем, то запускаем процесс атаки
        if (!attack)
        {
            attack = true; // сразу блокируем возможность повторной атаки во избежание наложения анимаций

            if (attackProcessCoroutine != null) StopCoroutine(attackProcessCoroutine);
            attackProcessCoroutine = StartCoroutine(AttackProcess());
        }
    }

    private void StartCharging() // запуск всех процессов для начала замаха (!!использовать с связке с извменением charging на true!!)
    {
        animator.ResetTrigger("Attack"); // сбрасываем триггер удара на всякий случай
        
        targetDamage = minDamage; // сброс потенциального урона при начале нового замаха

        animator.SetTrigger("Charging"); // триггер для анимации замаха
        rangeBar.SetActive(true);  // включаем шкалу заряда
    }
    private void StopCharging()
    {
        rBarImage.fillAmount = 0;
        rangeBar.SetActive(false);
    }

    private IEnumerator AttackProcess()
    {
        animator.SetTrigger("Attack");

        yield return null;

        // счетчик превышения времени ожидания анимации
        float timeout = 0f;

        // данный цикл работает пока не включится анимация, и для защиты от вечного зависания накручивает timeout
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && timeout < 1.0f) 
        {
            timeout += Time.deltaTime;
            yield return null;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); //берем информацию о текущем состоянии на слое 0
        float animationLength = stateInfo.length; // берем длину анимации в секундах
        yield return new WaitForSeconds(animationLength - 0.001f); // делаем паузу на время анимации, чтобы дать ей завершиться

        // после завершения анимации сбрасываем флаг действующей атаки
        attack = false;

        if (charging) // если во время атаки игрок продолжает удерживать кнопку атаки, то сразу запускаем процесс заряда для следующей атаки
        {
            StartCharging();
        }
    }

    public void Hit(Collider other) // эта функция активирует кончик молота при его соприкоснновении с объектами
    {
        if (attack)
        {
            Health _enemyHealth = other.gameObject.GetComponent<Health>();
            Debug.Log(_enemyHealth);

            if (_enemyHealth != null)
                _enemyHealth.Damage(targetDamage);
        }
    }
}