using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using CustomStorySystem; // 确保你的数据结构命名空间被正确引用
using DG.Tweening;      // 确保你已导入DOTween插件

public class StoryManager : MonoBehaviour
{
    [Header("UI 引用")]
    public TextAsset storyJsonFile;
    public Image character1Image;
    public Image character2Image;
    public Text speakerNameText;
    public Text contentText;
    public GameObject dialoguePanel;
    public GameObject choicePanel;
    public Image InsertImage1; // 只保留第一张插图的引用
    public Button[] choiceButtons;
    [Tooltip("用于场景切换的黑屏遮罩")]
    public Image blackScreenImage;

    [Header("效果参数")]
    public float typeSpeed = 0.05f;
    public float fadeDuration = 0.3f;
    public float scrollUnrollDuration = 1.0f;

    // --- 内部变量 ---
    private Dictionary<int, StoryLine> storyData;
    private StoryLine currentLine;
    private int currentLineKey = 1;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private Dictionary<Image, Vector2> originalPositions = new Dictionary<Image, Vector2>();
    private Dictionary<Image, Vector2> originalImageSizes = new Dictionary<Image, Vector2>();

    void Start()
    {
        // 确保黑屏在开始时是透明且禁用的
        if (blackScreenImage != null)
        {
            blackScreenImage.color = new Color(0, 0, 0, 0);
            blackScreenImage.gameObject.SetActive(false);
        }

        // 加载剧情数据
        storyData = JsonHelper.LoadStory(storyJsonFile);
        if (storyData == null)
        {
            Debug.LogError("剧情数据加载失败，请检查JSON文件！");
            this.enabled = false;
            return;
        }

        // 保存角色初始位置
        if (character1Image != null) originalPositions[character1Image] = character1Image.rectTransform.anchoredPosition;
        if (character2Image != null) originalPositions[character2Image] = character2Image.rectTransform.anchoredPosition;

        // 保存插图原始大小
        if (InsertImage1 != null)
        {
            originalImageSizes[InsertImage1] = InsertImage1.rectTransform.sizeDelta;
            InsertImage1.gameObject.SetActive(false);
        }

        // 剧情开始时，立即隐藏所有元素
        HideAllImmediately();

        // 显示第一行剧情
        ShowLine(currentLineKey);
    }

    void Update()
    {
        // 如果正在显示选项，则不响应任何操作
        if (choicePanel.activeSelf) return;

        // 监听点击或空格键
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // 如果正在打字，则立即完成打字
                CompleteLine();
            }
            else
            {
                // 如果打字已结束，则进入下一句
                if (typingCoroutine == null)
                {
                    GoToNextLine();
                }
            }
        }
    }

    void ShowLine(int key)
    {
        // 检查剧情是否结束
        if (!storyData.ContainsKey(key) || key == 0)
        {
            EndStory();
            return;
        }

        currentLineKey = key;
        currentLine = storyData[key];

        // 如果是选项节点，则显示选项并中断
        if (currentLine.ContentSpeaker == "CHOICE_NODE")
        {
            ShowChoices();
            return;
        }

        // 准备UI
        dialoguePanel.SetActive(true);
        choicePanel.SetActive(false);
        if (InsertImage1 != null) InsertImage1.gameObject.SetActive(false);

        // 对话框淡入效果
        CanvasGroup dialogueCG = dialoguePanel.GetComponent<CanvasGroup>() ?? dialoguePanel.AddComponent<CanvasGroup>();
        dialogueCG.alpha = 0;
        dialogueCG.DOFade(1, fadeDuration);

        // 处理角色动作
        ProcessCharacterAction(character1Image, currentLine.Cha1Action, currentLine.CoordinateX1, currentLine.Cha1ImageSource);
        ProcessCharacterAction(character2Image, currentLine.Cha2Action, currentLine.CoordinateX2, currentLine.Cha2ImageSource);

        // 显示文本
        speakerNameText.text = currentLine.ContentSpeaker;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLineCoroutine(currentLine.Content));

        // 处理单张插入图片
        ProcessInsertImage(InsertImage1, currentLine.InsertImage1Path);
    }

    /// <summary>
    /// 处理单张插入图片的显示/隐藏和卷轴效果
    /// </summary>
    void ProcessInsertImage(Image image, string imagePath)
    {
        if (image == null) return;

        if (!string.IsNullOrEmpty(imagePath))
        {
            Sprite newSprite = Resources.Load<Sprite>(imagePath);
            if (newSprite != null)
            {
                image.sprite = newSprite;
                image.gameObject.SetActive(true);
                image.rectTransform.sizeDelta = new Vector2(0, originalImageSizes[image].y);
                image.rectTransform.DOSizeDelta(originalImageSizes[image], scrollUnrollDuration).SetEase(Ease.OutQuad);
                image.color = new Color(1, 1, 1, 0);
                image.DOFade(1, scrollUnrollDuration);
            }
            else
            {
                Debug.LogWarning($"无法加载图片资源: {imagePath}");
                image.gameObject.SetActive(false);
            }
        }
        else
        {
            // 如果路径为空，确保图片是隐藏的（带淡出效果）
            if (image.gameObject.activeSelf)
            {
                image.DOFade(0, fadeDuration).OnComplete(() => image.gameObject.SetActive(false));
            }
        }
    }

    void ProcessCharacterAction(Image characterImage, string action, float xPos, string imageSource)
    {
        if (characterImage == null) return;

        if (!string.IsNullOrEmpty(imageSource))
        {
            characterImage.sprite = Resources.Load<Sprite>("Characters/" + imageSource);
        }
        xPos *= 1.5f; // 应用位置乘数
        switch (action)
        {
            case "AppearAt":
                characterImage.rectTransform.anchoredPosition = new Vector2(xPos, characterImage.rectTransform.anchoredPosition.y);
                StartCoroutine(FadeImage(characterImage, 1f));
                break;
            case "Disappear":
                StartCoroutine(FadeImage(characterImage, 0f));
                break;
            case "MoveTo":
                StartCoroutine(MoveCharacter(characterImage, xPos));
                break;
            case "ShakeAt":
                characterImage.rectTransform.anchoredPosition = new Vector2(xPos, characterImage.rectTransform.anchoredPosition.y);
                if (characterImage.color.a < 0.1f) StartCoroutine(FadeImage(characterImage, 1f));
                StartCoroutine(ShakeCharacter(characterImage));
                break;
            case "Shake":
                StartCoroutine(ShakeCharacter(characterImage));
                break;
            case "Continue":
            default:
                break;
        }
    }

    void GoToNextLine()
    {
        if (string.IsNullOrEmpty(currentLine.NextContent))
        {
            EndStory();
            return;
        }
        int nextKey = int.Parse(currentLine.NextContent);
        ShowLine(nextKey);
    }

    void ShowChoices()
    {
        dialoguePanel.SetActive(false);
        if (character1Image != null) StartCoroutine(FadeImage(character1Image, 0f));
        if (character2Image != null) StartCoroutine(FadeImage(character2Image, 0f));

        choicePanel.SetActive(true);
        string[] nextKeysStr = currentLine.NextContent.Split(',');
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choiceButtons.Length && i < nextKeysStr.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                int choiceKey = int.Parse(nextKeysStr[i].Trim());
                choiceButtons[i].GetComponentInChildren<Text>().text = storyData.ContainsKey(choiceKey) ? storyData[choiceKey].Content : "无效选项";
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choiceKey));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnChoiceSelected(int selectedKey)
    {
        choicePanel.SetActive(false);
        ShowLine(selectedKey);
    }

    IEnumerator TypeLineCoroutine(string text)
    {
        isTyping = true;
        contentText.text = "";
        foreach (char c in text)
        {
            contentText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        typingCoroutine = null;
    }

    void CompleteLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        isTyping = false;
        contentText.text = currentLine.Content;
    }

    void HideAllImmediately()
    {
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        if (character1Image != null) { Color c = character1Image.color; c.a = 0; character1Image.color = c; character1Image.gameObject.SetActive(false); }
        if (character2Image != null) { Color c = character2Image.color; c.a = 0; character2Image.color = c; character2Image.gameObject.SetActive(false); }
    }

    void EndStory()
    {
        Debug.Log("剧情结束，开始转场...");
        StartCoroutine(EndStoryCoroutine());
    }

    IEnumerator EndStoryCoroutine()
    {
        dialoguePanel.GetComponent<CanvasGroup>()?.DOFade(0, fadeDuration);
        if (character1Image != null && character1Image.gameObject.activeSelf) StartCoroutine(FadeImage(character1Image, 0f));
        if (character2Image != null && character2Image.gameObject.activeSelf) StartCoroutine(FadeImage(character2Image, 0f));
        yield return new WaitForSeconds(fadeDuration);

        if (blackScreenImage != null)
        {
            blackScreenImage.gameObject.SetActive(true);
            yield return blackScreenImage.DOFade(1, 0.5f).WaitForCompletion();
        }

        if (GameManager.Instance != null) GameManager.Instance.StartLevel();
        else Debug.LogError("找不到GameManager实例！无法加载关卡。");
    }

    #region 动画协程
    IEnumerator FadeImage(Image image, float targetAlpha)
    {
        if (image == null) yield break;
        if (targetAlpha > 0f && !image.gameObject.activeSelf) image.gameObject.SetActive(true);

        yield return image.DOFade(targetAlpha, fadeDuration).WaitForCompletion();

        if (targetAlpha <= 0f) image.gameObject.SetActive(false);
    }

    IEnumerator MoveCharacter(Image image, float targetX)
    {
        if (image == null || image.color.a < 0.1f) yield break;
        yield return image.rectTransform.DOAnchorPosX(targetX, fadeDuration).SetEase(Ease.OutQuad).WaitForCompletion();
    }

    IEnumerator ShakeCharacter(Image image)
    {
        if (image == null || image.color.a < 0.1f) yield break;
        yield return image.transform.DOShakePosition(0.5f, new Vector3(10, 10, 0), 20).WaitForCompletion();
    }
    #endregion
}