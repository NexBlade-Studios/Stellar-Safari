using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Ore : MonoBehaviour
{
    // Public variables
    public OreData oreData;

    // Private variables
    [SerializeField] private float currentDurability;
    [SerializeField] private AudioClip oreBreak;
    private MineOre interactManager;
    private PlayerController playerController;


    void Start()
    {
        currentDurability = oreData.maxDurability; // Sets to default durability
        interactManager = GameObject.Find("InteractManager").GetComponent<MineOre>();
        playerController = GameObject.Find("Astronaut").GetComponent<PlayerController>();
    }

    public void CurrentDurability(float value)
    {
        currentDurability = value;
    }

    public void Mine(float damage)
    {
        currentDurability -= damage; // Damages ore if mining

        if (currentDurability <= 0)
        {
            Break(); // Breaks ore if durability is less than or equal to zero
        }
    }

    private void Break()
    {
        GiveDrops();

        Drill drill = GameObject.Find("DrillHolder").GetComponent<Drill>();
        drill.UseDurability(); // Reduces drill durability

        interactManager.ResetDrillTransform();
        Destroy(gameObject); // Destroys ore
        SFXManager.instance.PlaySFXClip(oreBreak, transform, 0.5f);
    }

    private void GiveDrops()
    {
        if (gameObject.name.Contains("Copper"))
        {
            if (gameObject.name.Contains("Large"))
            {
                playerController.copperCount += 4;
            }
            else { playerController.copperCount++; }
        }
        if (gameObject.name.Contains("Iron"))
        {
            if (gameObject.name.Contains("Large"))
            {
                playerController.ironCount += 3;
            }
            else { playerController.ironCount++; }
        }
    }
}
