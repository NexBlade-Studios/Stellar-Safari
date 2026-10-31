using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class Drill : MonoBehaviour
{
    [Header("Drill Attributes")]
    [SerializeField] protected float maxHeat = 100f;
    [SerializeField] protected float maxEnergy = 100f;
    [SerializeField] protected float heatPerSecond = 20f;
    [SerializeField] protected float energyPerUse = 10f;
    [SerializeField] protected int maxDurability = 100;
    [SerializeField] protected float miningPower = 5f;

    [Header("Current Drill Values")]
    [SerializeField] protected float currentHeat;
    [SerializeField] protected float currentEnergy;
    [SerializeField] protected int currentDurability;

    [Header("Booleans")]
    [SerializeField] protected bool isMining;
    [SerializeField] protected bool isOverheated;
    [SerializeField] private bool deductingOreDurability;

    // Public variables
    public Slider heatBar;
    public Slider energyBar;
    public Image heatBarFill;

    // Private variables
    private Ore oreReference;
    private PlayerController playerController;
    private PlayerInteractUI playerInteractUI;
    private MineOre interactManager;
    [SerializeField] private AudioClip drillSound;
    [SerializeField] private AudioClip overheated;
    private AudioSource drillingAudio;

    // Virtual can be overrided in a class that derives the main class
    protected virtual void Start()
    {
        // Set default values
        currentEnergy = maxEnergy;
        currentDurability = maxDurability;
        heatBar.value = maxHeat;
        energyBar.value = maxEnergy;

        // References
        playerInteractUI = GameObject.Find("PlayerInteractUI").GetComponent<PlayerInteractUI>();
        playerController = GameObject.Find("Astronaut").GetComponent<PlayerController>();
        interactManager = GameObject.Find("InteractManager").GetComponent<MineOre>();
    }

    protected virtual void Update()
    {
        RaycastHit hit;

        if (playerController.CanMove(playerController.oreCheckDir, out hit)) // If can move then return as then there's no ore
            isMining = false;

        if (isMining)
        {
            IncreaseHeat(); // Mining causes heat to increase
            

            if (drillingAudio == null)
            {
                drillingAudio = SFXManager.instance.PlayLoopingSFX(drillSound, transform, 0.5f);
            }
        }
        else
        {
            CoolDown(); // If not mining then on cooldown or idle

            if (drillingAudio != null)
            {
                SFXManager.instance.StopLoopingSFX(drillingAudio);
                drillingAudio = null;
            }
        }

        while (!isOverheated && !deductingOreDurability && isMining)
        {
            StartCoroutine(MineOre(oreReference)); // Can only mine if not overheated and not deducting ore durability
        }
    }

    private void IncreaseHeat()
    {
        currentHeat += heatPerSecond * Time.deltaTime; // Heat increased per second
        heatBar.value = currentHeat / maxHeat; // Heat bar shows percentage heat currently

        if (currentHeat >= maxHeat)
        {
            currentHeat = maxHeat;
            SFXManager.instance.PlaySFXClip(overheated, transform, 0.5f);
            Overheat(); // Reached max heat so overheated
        }
    }

    private void CoolDown()
    {
        currentHeat = Mathf.Max(0, currentHeat - heatPerSecond * Time.deltaTime); // Current heat is decreased but never goes below 0
        heatBarFill.color = new Color32(145, 95, 70, 255); // Change colour to show on cooldown
        heatBar.value = currentHeat / maxHeat; // Heat bar shows percentage heat currently

        if (currentHeat <= 0)
        {
            isOverheated = false; // No longer overheated
            currentHeat = 0;
            heatBarFill.color = new Color32(240, 81, 0, 255); // Colour returns to bright orange
            interactManager.showingPrompt = false;
        }
    }

    // Can no longer mine when overheated
    private void Overheat()
    {
        isMining = false;
        isOverheated = true;
        // This is probably where the particles would go
    }

    public virtual void Mine(Ore ore)
    {
        if (isMining || isOverheated)
            return;
        if (currentEnergy < energyPerUse)
            return;
        if (currentDurability <= 0)
            return;

        currentEnergy -= energyPerUse; // Energy is reduced per use of the drill
        energyBar.value = currentEnergy / maxEnergy; // Energy bar shows percentage energy currently

        isMining = true; // Allows the while loop in the update function to run
        deductingOreDurability = false; // Not currently deducting anything

        oreReference = ore; // Takes reference from when Mine is called from MineOre
    }

    public void UseDurability()
    {
        currentDurability--; // Subtracts when the entire ore has been destroyed as opposed to each use
        // Durability colours can probably go here
    }

    // Deducts durability from ore every second when mining
    IEnumerator MineOre(Ore ore)
    {
        deductingOreDurability = true;
        yield return new WaitForSeconds(1);

        ore.Mine(miningPower);
        deductingOreDurability = false;
    }
}
