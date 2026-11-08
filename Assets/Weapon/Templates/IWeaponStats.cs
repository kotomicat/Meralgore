using UnityEngine;

// 1. для любого оружия
public interface IWeaponStats
{
    float Damage { get; }
    float Cooldown { get; } // задержка перед следующим действием
    float Range { get; }    // дистанция поражения (пуля / замах молота)
}

// 2. для любого стреляющего оружия
public interface IAmmoStats
{
    int MaxAmmo { get; }
    float Spread { get; }   // разброс
}

// 3. для дробовиков
public interface IShotgunStats
{
    int PelletsCount { get; } // количество дробинок в одном выстреле
}

// 4. для заряжаемого оружия
public interface IChargeableStats
{
    float MaxChargeTime { get; }     // максимальное время на которое можно удерживать заряд
    float DamageScale { get; } // максимальный множитель урона (тот самый damageMultiplier)
    float MinChargeThreshold { get; }  // минимальный заряд, ниже которого урон = minDamage
}
