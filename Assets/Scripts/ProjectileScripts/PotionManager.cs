using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionManager : Projectile
{
    [HideInInspector] public float areaOfEffect = 0;
    [HideInInspector] public float slownessEffectMultiplier;
    [HideInInspector] public float slownessEffectDuration; 
    List<EnemyManager> affectedEnemies = new List<EnemyManager>(); 

    private void OnTriggerEnter(Collider other)
    {
        affectedEnemies.Clear();
        Dictionary<EnemyManager, GameObject> activeEnemies = ObjectPooler.Instance.GetAllActiveEnemies();
        if (!activeEnemies.ContainsValue(other.gameObject)) return; 
        foreach (var item in activeEnemies)
        {
            if(Vector3.Distance(item.Value.transform.position, transform.position) < areaOfEffect)
            {
                affectedEnemies.Add(item.Key);
            }
        }
        foreach (var item in affectedEnemies)
        {
            item.TakeDamage(damagePotential);
            item.slowedTime = slownessEffectDuration;
            item.slowedTimer = 0;
            if (item.isSlowed) continue;
            item.isSlowed = true;
            item.speed *= slownessEffectMultiplier;
        }
        
        gameObject.SetActive(false);
    }
}
