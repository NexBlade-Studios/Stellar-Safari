using TMPro;
using UnityEngine;

public class PlayerInteractUI : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private TMP_SpriteAsset mobileSprites;

    private void Start()
    {
        if (Application.isMobilePlatform)
        {
            container.GetComponentInChildren<TextMeshProUGUI>().spriteAsset = mobileSprites;
        }
    }

    public void Show(string message)
    {
        container.GetComponentInChildren<TextMeshProUGUI>().text = message;
        container.SetActive(true);
    }

    public void Hide()
    {
        container.SetActive(false);
    }
}
