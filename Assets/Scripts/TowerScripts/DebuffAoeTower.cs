using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffAoeTower : RangeAttackTower
{
    [SerializeField] float splashRange;

    [SerializeField] float slownessEffectMultiplier;
    [SerializeField] float slownessEffectDuration;

    public override void Attack()
    {
        attacking = true;
        PotionManager shotPotion = (PotionManager)ShootProjectile(1);
        shotPotion.areaOfEffect = splashRange;
        shotPotion.slownessEffectDuration = slownessEffectDuration;
        shotPotion.slownessEffectMultiplier = slownessEffectMultiplier;
    }

    public override void Upgrade(int upgradeType, float vlaue)
    {
        base.Upgrade(upgradeType, vlaue);
        if (upgradeType == (int)Upgradable.UpgradeType.effectDuration) slownessEffectDuration = vlaue;
        if (upgradeType == (int)Upgradable.UpgradeType.effectRange) splashRange = vlaue;
    }
}
