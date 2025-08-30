using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebuffUIManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject debuffIconPrefab;

    [Header("Layout Containers")]
    [SerializeField] private Transform stackingDebuffParent;
    [SerializeField] private Transform temporaryDebuffParent;

    private Dictionary<string, GameObject> activeStackingIcons = new Dictionary<string, GameObject>();

    public void AddOrUpdateStackingDebuff(string debuffName, Color color, int stackCount)
    {
        TextMeshProUGUI textComponent;
        if (activeStackingIcons.ContainsKey(debuffName))
        {
            GameObject icon = activeStackingIcons[debuffName];
            textComponent = icon.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = $"{debuffName} x{stackCount}";
        }
        else
        {
            GameObject newIcon = Instantiate(debuffIconPrefab, stackingDebuffParent);
            textComponent = newIcon.GetComponentInChildren<TextMeshProUGUI>();

            textComponent.color = color;
            textComponent.text = $"{debuffName} x{stackCount}";

            newIcon.GetComponent<FadeOutUI>().enabled = false;
            activeStackingIcons.Add(debuffName, newIcon);
        }
    }

    public void ShowTemporaryDebuff(string debuffName, Color color)
    {
        GameObject newIcon = Instantiate(debuffIconPrefab, temporaryDebuffParent);
        TextMeshProUGUI textComponent = newIcon.GetComponentInChildren<TextMeshProUGUI>();

        // --- THIS IS THE FIX ---
        // Set the color of the TextMeshProUGUI component here as well.
        textComponent.color = color;
        textComponent.text = debuffName;
    }

    public void ClearAllStackingDebuffs()
    {
        foreach (var icon in activeStackingIcons.Values)
        {
            if (icon != null)
            {
                icon.GetComponent<FadeOutUI>().enabled = true;
            }
        }
        activeStackingIcons.Clear();
    }
}