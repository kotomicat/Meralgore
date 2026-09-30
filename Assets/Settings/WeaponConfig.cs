using UnityEngine;

public class WeaponConfig : ScriptableObject
{
    [Header("Coin settings")]
    [SerializeField] private float coinHitRadius = 0.5f;
    [SerializeField] private float coinCastRange = 30f;
    [SerializeField] private float shootTraceWidth = 0.2f;
    [SerializeField] private float shootTraceThinningStep = 0.001f;


    public float CoinHitRadius => coinHitRadius;
    public float CoinCastRange => coinCastRange;
    public float SootTraceWidth => shootTraceWidth;
}


