using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterTable", menuName = "Scriptable Objects/EncounterTable")]
public class EncounterTable : ScriptableObject
{
    public List<EnemyData> enemyPool;
    public SpawnCountWeight spawnWeights;
}
