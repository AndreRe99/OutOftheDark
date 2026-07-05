using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds one row per level from LevelIds.AllLevels at runtime: locked levels show a lock label,
/// unlocked levels show completion/stars/best time/best ammo used and are clickable to load.
/// Runtime-built (rather than baked per level in the editor) so it stays correct if levels are
/// added or removed later without touching this scene.
/// </summary>
public class LevelSelectController : MonoBehaviour
{
    public RectTransform content;
    public Font uiFont;

    void Start()
    {
        BuildRows();
    }

    private void BuildRows()
    {
        const float rowHeight = 90f;
        float y = 0f;

        foreach (string levelId in LevelIds.AllLevels)
        {
            BuildRow(levelId, y);
            y -= rowHeight;
        }

        if (content != null)
        {
            content.sizeDelta = new Vector2(content.sizeDelta.x, LevelIds.AllLevels.Length * rowHeight + 20f);
        }
    }

    private void BuildRow(string levelId, float y)
    {
        bool unlocked = ProgressManager.IsUnlocked(levelId);
        bool completed = ProgressManager.IsCompleted(levelId);

        var rowGo = new GameObject("Row_" + levelId);
        rowGo.transform.SetParent(content, false);
        var rowRect = rowGo.AddComponent<RectTransform>();
        rowRect.anchorMin = new Vector2(0f, 1f);
        rowRect.anchorMax = new Vector2(1f, 1f);
        rowRect.pivot = new Vector2(0.5f, 1f);
        rowRect.anchoredPosition = new Vector2(0f, y);
        rowRect.sizeDelta = new Vector2(0f, 80f);

        var bg = rowGo.AddComponent<Image>();
        bg.color = unlocked ? new Color(1f, 1f, 1f, 0.08f) : new Color(0f, 0f, 0f, 0.3f);

        var button = rowGo.AddComponent<Button>();
        button.interactable = unlocked;

        string label = !unlocked ? $"{levelId}  (gesperrt)" : completed ? $"{levelId}  - abgeschlossen" : levelId;
        CreateLabel(rowGo.transform, "MainLabel", label, 22, new Vector2(0f, 0.5f), new Vector2(20f, 0f));

        if (unlocked && completed)
        {
            int stars = ProgressManager.GetStars(levelId);
            float bestTime = ProgressManager.GetBestTime(levelId);
            int bestUsed = ProgressManager.GetBestUsedBalls(levelId);
            string starsText = new string('*', stars) + new string('-', 3 - stars);
            int minutes = Mathf.FloorToInt(Mathf.Max(0f, bestTime) / 60f);
            int seconds = Mathf.FloorToInt(Mathf.Max(0f, bestTime) % 60f);
            string detail = $"{starsText}   Zeit {minutes:00}:{seconds:00}   Baelle {bestUsed}";
            CreateLabel(rowGo.transform, "DetailLabel", detail, 16, new Vector2(1f, 0.5f), new Vector2(-20f, 0f));
        }
        else if (!unlocked)
        {
            CreateLabel(rowGo.transform, "LockLabel", "gesperrt", 16, new Vector2(1f, 0.5f), new Vector2(-20f, 0f));
        }

        if (unlocked)
        {
            string targetLevel = levelId;
            button.onClick.AddListener(() => SceneLoader.LoadScene(targetLevel));
        }
    }

    private Text CreateLabel(Transform parent, string name, string text, int fontSize, Vector2 anchor, Vector2 offset)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = offset;
        rect.sizeDelta = new Vector2(420f, 60f);

        var textComponent = go.AddComponent<Text>();
        textComponent.font = uiFont != null ? uiFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = fontSize;
        textComponent.color = Color.white;
        textComponent.alignment = anchor.x < 0.5f ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
        textComponent.text = text;
        return textComponent;
    }

    public void OnBackButtonPressed() => SceneLoader.LoadScene(LevelIds.MainMenu);
}
