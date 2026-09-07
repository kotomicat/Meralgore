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
    
    [Header("Animation")]
    [SerializeField] private float backRotationAngle = 15f;
    [SerializeField] private float forwardRotationAngle = 15f;
    [HideInInspector] public Quaternion startRotation;
    public float angleStep; // ИСПОЛЬЗУЕТСЯ В ТЕХ МЕСТАХ АНИМАЦИИ ГДЕ ROTATETOWARDS
    public float step; // ИСПОЛЬЗУЕТСЯ В ТЕХ МЕСТАХ АНИМАЦИИ ГДЕ LERP
    private Quaternion backRotationDelta;
    private Quaternion forwardRotationDelta;

    [Header("Other")]
    public bool lockAttack;
    private Coroutine beatCoroutine; 
    private Coroutine cancelledBeatCoroutine;

    void Start()
    {   
        lockAttack = false;
        rBarImage = rangeBar.GetComponent<Image>();
        
        backRotationDelta = Quaternion.Euler(0, 0, -backRotationAngle);
        forwardRotationDelta = Quaternion.Euler(0, 0, forwardRotationAngle);
        
        startRotation = transform.localRotation;
    }

    void Update()
    {
        Attack();
    }

    public override void Attack() // здесь происходят все действия, когда зажата кнопка
    // в данном случае - небольшой замах назад, определение мощности удара
    {
        if (lockAttack) return; 
        if  (Mathf.Abs(targetDamage - damage) > 0.001f && attack) // пока урон не достиг максимума, либо пока зажата лкм
        {
            targetDamage = Mathf.MoveTowards(targetDamage, damage, Time.deltaTime * scaleSpeed); // набирается урон
            rBarImage.fillAmount = targetDamage / damage; // заполняется колесико

            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, startRotation * backRotationDelta, Time.deltaTime * 5f); 
        }
    }

    public override void StartAttack()
    {
        if (lockAttack) // в случае, если сейчас происходит атака
        {
            if (beatCoroutine != null)
            {
                StopCoroutine(beatCoroutine); // жестко останавливаем процесс удара

                // сбрасываем rangebar и текущий урон 
                rBarImage.fillAmount = 0;
                targetDamage = minDamage;
                rangeBar.SetActive(false);

                cancelledBeatCoroutine = StartCoroutine(CancelledBeat()); // запускам процесс отмены удара
                return; // не даем этой функции доработать, так как все последующие в ней события уже лежат на корутине отмены
            }

            transform.localRotation = startRotation;
            lockAttack = false;
        }

        attack = true;
        rangeBar.SetActive(true);
    }

    public override void EndAttack()
    {   
        lockAttack = true;
        attack = false;
        
        rBarImage.fillAmount = 0;
        rangeBar.SetActive(false);

        beatCoroutine = StartCoroutine(Beat()); // на отжатие лкм запускается корутина удара
    }
    
    private IEnumerator Beat() // ПРОЦЕСС УДАРА
    {
        Quaternion _strikeRotation = startRotation * forwardRotationDelta; // точка, до которой происходит замах

        // 1. первоначальный взмах - от текущей позиции до положения удара
        while (Quaternion.Angle(transform.localRotation, _strikeRotation) > 0.1f) // пока истинный поворот не приблизился к конечной точке
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, startRotation * forwardRotationDelta, Time.deltaTime * step);
            yield return null;
        }
        
        // доводка поворота до нужной точки
        transform.localRotation = _strikeRotation;
        yield return new WaitForSeconds(0.05f);
        // 2. возвращение обратно

        while (Quaternion.Angle(transform.localRotation, startRotation) > 0.1f) // пока истинный поворот не приблизился к конечной точке
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, startRotation, Time.deltaTime * angleStep);
            yield return null;
        }

        targetDamage = minDamage;
        lockAttack = false;    
        
        yield break;
    }

    private IEnumerator CancelledBeat() // ПЛАВНО ВОЗВРАЩАЕТ МОЛОТ В ИСХОДНОЕ ПОЛОЖЕНИЕ ПОСЛЕ ОТМЕНЫ АТАКИ
    {
        lockAttack = true; // в момент возвраащения нельзя атаковать, лочим

        while (Quaternion.Angle(transform.localRotation, startRotation) > 0.1f) // пока истинный поворот не приблизился к конечной точке
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, startRotation, Time.deltaTime *  angleStep * 4);
            yield return null;
        }
        
        transform.localRotation = startRotation; // доводка положения

        // стандартные процедуры для начала новой атаки после доводки
        attack = true;
        rangeBar.SetActive(true); 

        targetDamage = minDamage; // сборс урона
        lockAttack = false;
    }
    
    public void Hit(Collider other) // эту функцию активирует кончик молота при его соприкоснновении с объектами
    {
        if (lockAttack)
        {
            Health _enemyHealth = other.gameObject.GetComponent<Health>();
            Debug.Log(_enemyHealth);

            if (_enemyHealth != null)
                _enemyHealth.Damage(targetDamage);
        }
    }
}