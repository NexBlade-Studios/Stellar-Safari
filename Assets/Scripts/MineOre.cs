using UnityEngine;

public class MineOre : MonoBehaviour
{
    // Private variables
    private PlayerInteractUI playerInteractUI;
    private PlayerController playerController;
    private SwipeLogger swipeLogger;
    private Drill drill;
    public bool showingPrompt;
    [SerializeField] private float fadelength;

    void Start()
    {
        // References
        playerInteractUI = GameObject.Find("PlayerInteractUI").GetComponent<PlayerInteractUI>();
        playerController = GameObject.Find("Astronaut").GetComponent<PlayerController>();
        swipeLogger = GameObject.Find("SwipeLogger").GetComponent<SwipeLogger>();
        drill = GameObject.Find("DrillHolder").GetComponent<Drill>();
    }

    void Update()
    {
        // Mines if E is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            Mine();
            playerInteractUI.Hide();
        }
    }

    // Checks if the player is facing an ore to display a prompt to mine
    public void OreInteract()
    {
        RaycastHit hit;
        if (!playerController.CanMove(playerController.oreCheckDir, out hit)) // Checks that the player can't move forward (something is obstructing it)
        {
            if (hit.collider.CompareTag("Ore")) // Checks that thing is an ore
            {
                if (!showingPrompt) // Checks if prompt is already shown
                {
                    string message;
                    if (Application.isMobilePlatform)
                    {
                        message = "<sprite anim=\"0,1,1\"> to mine"; // Tap to mine with animation
                    }
                    else
                    {
                        message = "Press <sprite name=\"E\"> to mine ";
                    }
                    playerInteractUI.Show(message);
                    showingPrompt = true;
                }
                return;
            }
        }
        else if (showingPrompt)
        {
            playerInteractUI.Hide();
            showingPrompt = false;
        }
    }

    private void Mine()
    {
        RaycastHit hit;

        if (playerController.CanMove(playerController.oreCheckDir, out hit)) // If can move then return as then there's no ore
            return;
        if (!hit.collider.CompareTag("Ore")) // If not an ore then return
            return;

        Ore ore = hit.collider.GetComponent<Ore>(); // Gets Ore component of the hit Ore

        if (ore == null) // If no ore then return
            return;
        playerController.anim.CrossFade("Mining", fadelength);
        SetDrillTransform(drill.transform.GetChild(0).gameObject);

        if (!playerController.isMoving)
        {
            drill.Mine(ore); // Calls drill to mine the ore
        }
    }

    private void SetDrillTransform(GameObject drill)
    {
        drill.SetActive(false);
        drill.transform.localPosition = new Vector3(0, -0.475f, 0.8f);
        drill.transform.localRotation = Quaternion.Euler(-75, 0, 0);
        drill.transform.localScale = Vector3.one;
        drill.SetActive(true);
    }

    public void ResetDrillTransform()
    {
        GameObject obj = drill.transform.GetChild(0).gameObject;
        obj.SetActive(false);
        obj.transform.localPosition = new Vector3(0, -0.35f, -0.6f);
        obj.transform.localRotation = Quaternion.Euler(0, -180, -45);
        obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        obj.SetActive(true);
        playerController.anim.CrossFade("Idle", 1f);
    }
}
