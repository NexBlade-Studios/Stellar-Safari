using UnityEngine;

public class MineOre : MonoBehaviour
{
    private PlayerInteractUI playerInteractUI;
    private PlayerController playerController;
    private SwipeLogger swipeLogger;
    private bool showingPrompt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInteractUI = GameObject.Find("PlayerInteractUI").GetComponent<PlayerInteractUI>();
        playerController = GameObject.Find("Astronaut").GetComponent<PlayerController>();
        swipeLogger = GameObject.Find("SwipeLogger").GetComponent<SwipeLogger>();
    }

    // Update is called once per frame
    void Update()
    {
        OreInteract();
    }

    public void OreInteract()
    {
        RaycastHit hit;
        if (!playerController.CanMove(playerController.oreCheckDir, out hit))
        {
            if (hit.collider.CompareTag("Ore"))
            {
                if (!showingPrompt)
                {
                    string message;
                    if (Application.isMobilePlatform)
                    {
                        message = "Tap <sprite name=\"tap\"> to Mine ";
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

    }
}
