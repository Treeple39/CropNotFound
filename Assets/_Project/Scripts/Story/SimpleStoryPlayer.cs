// 文件名: SimpleStoryPlayer.cs (终极方案)
// 作用: 支持双人对话和独立动画的极简剧情播放器。

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleStory;

[RequireComponent(typeof(AudioSource))]
public class SimpleStoryPlayer : MonoBehaviour
{
    [Header("UI 引用")]
    public Image backgroundImage;
    public Image character1Image; // 角色1
    public Image character2Image; // 角色2
    public Text speakerNameText;
    public Text contentText;
    public Image dialogueBoxImage;
    public GameObject choicePanel;
    public Button[] choiceButtons;

    [Header("配置")]
    public TextAsset storyJsonFile;
    public GameObject playerControllerObject;

    [Header("效果参数")]
    public float typeSpeed = 0.05f;
    public float fadeDuration = 0.5f;
    public AudioClip typeSound;
    public AudioClip nextLineSound;

    private AudioSource audioSource, bgmSource;
    private List<StoryLine> storyLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private Dictionary<Image, Vector2> originalPositions = new Dictionary<Image, Vector2>();
    private Dictionary<Image, Coroutine> actionCoroutines = new Dictionary<Image, Coroutine>();

    void Awake()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        audioSource = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
        bgmSource = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;

        if (playerControllerObject != null) playerControllerObject.SetActive(false);

        if (character1Image != null) originalPositions[character1Image] = character1Image.rectTransform.anchoredPosition;
        if (character2Image != null) originalPositions[character2Image] = character2Image.rectTransform.anchoredPosition;

        if (dialogueBoxImage != null)
        {
            dialogueBoxImage.gameObject.SetActive(true); // <--- 【新增】在这里添加这一行
        }

        ClearUI();
    }

    void Start()
    {
        storyLines = JsonHelper.LoadStory(storyJsonFile);
        ShowLine(currentLineIndex);
    }

    void Update()
    {
        if (choicePanel.activeSelf) return;
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping) CompleteLine();
            else GoToNextLine();
        }
    }

    void ShowLine(int index)
    {
        if (index >= storyLines.Count) { EndStory(); return; }
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        currentLineIndex = index;
        StoryLine line = storyLines[index];

        UpdateBackgroundImage(line.BackgroundImage);
        UpdateBackgroundMusic(line.BackgroundMusic);
        UpdateDialogueSound(line.DialogueSound);

        ProcessCharacterAndActions(line);

        speakerNameText.text = line.Speaker;
        typingCoroutine = StartCoroutine(TypeLineCoroutine(line.Content));
        choicePanel.SetActive(false);
    }

    private void ProcessCharacterAndActions(StoryLine line)
    {
        if (!string.IsNullOrEmpty(line.Character))
        {
            string[] parts = line.Character.Split(':');
            if (parts.Length == 2)
            {
                UpdateImage(GetCharacterImage(parts[0]), parts[1], true);
            }
        }

        if (!string.IsNullOrEmpty(line.Action))
        {
            string[] commands = line.Action.Split(',');
            foreach (var command in commands)
            {
                string[] parts = command.Trim().Split(':');
                if (parts.Length == 2)
                {
                    ExecuteActionOnCharacter(GetCharacterImage(parts[0]), parts[1]);
                }
            }
        }
    }

    private Image GetCharacterImage(string target)
    {
        if (target.ToLower() == "char1") return character1Image;
        if (target.ToLower() == "char2") return character2Image;
        Debug.LogWarning("未知的角色目标: " + target);
        return null;
    }

    private void ExecuteActionOnCharacter(Image targetImage, string actionCommand)
    {
        if (targetImage == null) return;

        if (actionCoroutines.ContainsKey(targetImage) && actionCoroutines[targetImage] != null)
        {
            StopCoroutine(actionCoroutines[targetImage]);
        }

        string[] parts = actionCommand.Split('=');
        string command = parts[0].ToLower();

        switch (command)
        {
            case "appear":
                StartCoroutine(FadeImageCoroutine(targetImage, 1f));
                break;
            case "hide":
                StartCoroutine(FadeImageCoroutine(targetImage, 0f));
                break;
            case "shake":
                actionCoroutines[targetImage] = StartCoroutine(ShakeCharacter(targetImage));
                break;
            case "move":
                if (parts.Length > 1 && float.TryParse(parts[1], out float xPos))
                {
                    actionCoroutines[targetImage] = StartCoroutine(MoveCharacter(targetImage, xPos));
                }
                break;
        }
    }

    private IEnumerator ShakeCharacter(Image image)
    {
        if (image.color.a < 0.1f) yield break;
        float duration = 0.5f;
        float magnitude = 10f;
        Vector2 startPos = originalPositions[image];
        float elapsed = 0f;
        while (elapsed < duration)
        {
            image.rectTransform.anchoredPosition = startPos + Random.insideUnitCircle * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }
        image.rectTransform.anchoredPosition = startPos;
    }

    private IEnumerator MoveCharacter(Image image, float targetX)
    {
        if (image.color.a < 0.1f) yield break;
        Vector2 startPos = image.rectTransform.anchoredPosition;
        Vector2 endPos = new Vector2(targetX, startPos.y);
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            image.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        image.rectTransform.anchoredPosition = endPos;
        originalPositions[image] = endPos;
    }

    private IEnumerator TypeLineCoroutine(string text)
    {
        isTyping = true;
        contentText.text = "";
        foreach (char c in text)
        {
            contentText.text += c;
            if (typeSound != null) audioSource.PlayOneShot(typeSound, 0.5f);
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        ShowChoicesIfNeeded();
    }

    private void CompleteLine()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isTyping = false;
        contentText.text = storyLines[currentLineIndex].Content;
        ShowChoicesIfNeeded();
    }

    private void ShowChoicesIfNeeded()
    {
        StoryLine line = storyLines[currentLineIndex];
        if (line.Choices != null && line.Choices.Length > 0)
        {
            choicePanel.SetActive(true);
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < line.Choices.Length)
                {
                    choiceButtons[i].gameObject.SetActive(true);
                    choiceButtons[i].GetComponentInChildren<Text>().text = line.Choices[i];
                    int id = line.ChoiceNextIDs[i];
                    choiceButtons[i].onClick.RemoveAllListeners();
                    choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(id));
                }
                else
                {
                    choiceButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void GoToNextLine()
    {
        if (nextLineSound != null) audioSource.PlayOneShot(nextLineSound);
        int next = storyLines[currentLineIndex].NextID;
        ShowLine(next != -1 ? next : currentLineIndex + 1);
    }

    void OnChoiceSelected(int nextID)
    {
        choicePanel.SetActive(false);
        ShowLine(nextID);
    }

    void EndStory()
    {
        if (playerControllerObject != null) playerControllerObject.SetActive(true);
        gameObject.SetActive(false);
    }

    void UpdateBackgroundImage(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        Sprite sprite = Resources.Load<Sprite>(path);
        if (backgroundImage != null && backgroundImage.sprite != sprite)
        {
            StartCoroutine(FadeImageCoroutine(backgroundImage, 1f, () => backgroundImage.sprite = sprite));
        }
    }

    void UpdateImage(Image image, string path, bool useFade)
    {
        if (image == null) return;
        Sprite sprite = string.IsNullOrEmpty(path) ? null : Resources.Load<Sprite>(path);
        if (sprite != image.sprite)
        {
            StartCoroutine(FadeImageCoroutine(image, sprite == null ? 0f : 1f, () => image.sprite = sprite));
        }
    }

    void UpdateBackgroundMusic(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip != null && bgmSource.clip != clip)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }

    void UpdateDialogueSound(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip != null) audioSource.PlayOneShot(clip);
    }

    IEnumerator FadeImageCoroutine(Image image, float targetAlpha, System.Action onHalfway = null)
    {
        float startAlpha = image.color.a;
        if (Mathf.Approximately(startAlpha, targetAlpha)) yield break;
        if (targetAlpha > startAlpha && onHalfway != null) onHalfway();
        float time = 0;
        while (time < fadeDuration)
        {
            Color color = image.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            image.color = color;
            time += Time.deltaTime;
            yield return null;
        }
        Color finalColor = image.color;
        finalColor.a = targetAlpha;
        image.color = finalColor;
        if (targetAlpha < startAlpha && onHalfway != null) onHalfway();
    }

    // ... 其他方法 (FadeImageCoroutine 等) ...

    void ClearUI()
    {
        // 隐藏文本
        speakerNameText.text = "";
        contentText.text = "";

        // 使用FadeImageCoroutine平滑地隐藏所有图片
        // 注意：我们不希望在Awake时有渐变，所以直接设置颜色
        // 但如果之后在其他地方调用，渐变会更柔和

        // 隐藏背景图 (设为完全透明)
        if (backgroundImage != null)
        {
            Color bgColor = backgroundImage.color;
            bgColor.a = 0;
            backgroundImage.color = bgColor;
            backgroundImage.sprite = null; // 清除sprite以防万一
        }

        // 隐藏角色1
        if (character1Image != null)
        {
            Color c1Color = character1Image.color;
            c1Color.a = 0;
            character1Image.color = c1Color;
        }

        // 隐藏角色2
        if (character2Image != null)
        {
            Color c2Color = character2Image.color;
            c2Color.a = 0;
            character2Image.color = c2Color;
        }

        // 隐藏选项面板
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
    }
}