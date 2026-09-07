using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    CinemachineCamera cam;
    private enum CameraState { Idle, PulseAway, PulseBack } // состояния камеры
    private CameraState currentState = CameraState.Idle; // состояние камеры при запуске - покой

    [SerializeField] float offsetFOV = 10f;
    [Tooltip("Величина, на которую изменится FOV при пульсации")]

    [SerializeField] float animAwayDuration = 0.075f;
    [Tooltip("Время, за которое FOV увеличится при пульсации")]
    [SerializeField] float animBackDuration = 0.075f;
    [Tooltip("Время, за которое FOV вернется в исходное состояние при пульсации")]

    float animTimer = 0f;
    float animDuration = 0f;

    float originalFOV;
    float targetFOV;
    float currentFOV;

    private void Start()
    {
        cam = GetComponent<CinemachineCamera>();

        originalFOV = cam.Lens.FieldOfView;
        targetFOV = cam.Lens.FieldOfView + offsetFOV;
    }

    // НА ПРИМЕРЕ КАМЕРЫ Я НАУЧИЛАСЬ РАБОТАТЬ С ENUM И ПЕРЕКЛЮЧЕНИЕМ СОСТОЯНИЙ

    private void Update()
    {
        if (currentState == CameraState.Idle) return; // в покое с камерой ничего делать не нужно, код далее не выполнится

        animTimer += Time.deltaTime; // таймер, накапливающий прошедшее время

        float progress = Mathf.Clamp01(animTimer / animDuration); // доля прошедшего времени от максимального
        float smoothProgress = Mathf.SmoothStep(0, 1, progress); // эта функция смягчает уровень прогресса
        //когда lerp использует вместо шага, множенного на delta time, готовый прогесс - он двигает значение линейно
        //smoothstep позволяет lerp делать плакную анимацию, несмотря на фиксированный параметр прогресса

        switch (currentState) // машина, отвечающая за логику в каждом состоянии
        {
            case CameraState.PulseAway: // если камера пульсирует вперед
                cam.Lens.FieldOfView = Mathf.Lerp(currentFOV, targetFOV, smoothProgress);

                if (progress >= 1f)  // при конечном итоге прогресса
                {
                    cam.Lens.FieldOfView = targetFOV; // выравнивание fov во избещание смещений
                    SetState(CameraState.PulseBack, animBackDuration); // одно состояние переходит в другое - возвращение назад
                    return;
                }
                break;

            case CameraState.PulseBack: // если камера возвращается назад
                cam.Lens.FieldOfView = Mathf.Lerp(currentFOV, originalFOV, smoothProgress);

                if (progress >= 1f) // при конечном итоге прогресса
                {
                    cam.Lens.FieldOfView = originalFOV; // выравнивание
                    SetState(CameraState.Idle, 0f); // возвращение в покой
                    return;
                }
                break;
        }
    }

    private void SetState(CameraState newState, float duration) // быстрое переключение состояние со сборсом временных переменных
    {
        currentState = newState; 
        animTimer = 0f;
        animDuration = duration; // записанное время в функцию стновится следующим

        currentFOV = cam.Lens.FieldOfView; 
    }

    public void Pulse()
    {
        currentFOV = cam.Lens.FieldOfView;
        SetState(CameraState.PulseAway, animAwayDuration);
    }
}
