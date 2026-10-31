using UnityEngine;

[CreateAssetMenu(fileName = "NewOre", menuName = "Ores/OreData")]
public class OreData : ScriptableObject
{
    public string oreName;

    public float maxDurability = 100f;

    public int minDropAmount = 1;
    public int maxDropAmount = 3;
}
