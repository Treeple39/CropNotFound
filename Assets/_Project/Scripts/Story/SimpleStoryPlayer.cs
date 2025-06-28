// 文件名: StoryManager.cs (优化版)
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using CustomStorySystem;
using DG.Tweening; // 引用我们自己的命名空间

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
    public Image InsertImage1;
    public Image InsertImage2;
    public Button[] choiceButtons;

    [Tooltip("用于场景切换的黑屏遮罩")]
    public Image blackScreenImage;

    [Header("效果参数")]
    public float typeSpeed = 0.05f;
    public float fadeDuration = 0.3f;


    private Dictionary<int, StoryLine> storyData;
    private StoryLine currentLine;
    private int currentLineKey = 1; // 从Key为1的行开始
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    public float scrollUnrollDuration = 1.0f;

    // 存储角色初始位置 (这个可以保留，以备将来需要复位功能)
    private Dictionary<Image, Vector2> originalPositions = new Dictionary<Image, Vector2>();
    private Dictionary<Image, Vector2> originalImageSizes = new Dictionary<Image, Vector2>();

    private bool waitingForClick = false;
    private Image currentInsertImage = null;
    private string nextImagePath = "";
    private bool isImageShowing = false;
    private bool waitingForSecondImage = false;
    private bool isFirstImageShowing = false;

    void Start()
    {
        storyData = JsonHelper.LoadStory(storyJsonFile);
        if (storyData == null)
        {
            Debug.LogError("剧情数据加载失败，请检查JSON文件！");
            this.enabled = false;
            return;
        }
        if (blackScreenImage != null)
        {
            blackScreenImage.color = new Color(0, 0, 0, 0);
            blackScreenImage.gameObject.SetActive(false);
        }

        // 保存初始位置
        if (character1Image != null) originalPositions[character1Image] = character1Image.rectTransform.anchoredPosition;
        if (character2Image != null) originalPositions[character2Image] = character2Image.rectTransform.anchoredPosition;

        if (InsertImage1 != null)
        {
            originalImageSizes[InsertImage1] = InsertImage1.rectTransform.sizeDelta;
            InsertImage1.gameObject.SetActive(false);
        }
        if (InsertImage2 != null)
        {
            originalImageSizes[InsertImage2] = InsertImage2.rectTransform.sizeDelta;
            InsertImage2.gameObject.SetActive(false);
        }

        // 【优化1】: 剧情开始时，立即隐藏所有元素，而不是等待淡出
        HideAllImmediately();

        ShowLine(currentLineKey);
    }

    void Update()
    {
        if (choicePanel.activeSelf) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                CompleteLine();
            }
            else if (waitingForSecondImage)
            {
                // 点击后显示第二张图片
                waitingForSecondImage = false;
                if (!string.IsNullOrEmpty(currentLine.InsertImage2Path))
                {
                    ProcessInsertImage(InsertImage2, currentLine.InsertImage2Path, false);
                }
                else
                {
                    // 没有第二张图片了，继续剧情
                    GoToNextLine();
                }
            }
            else
            {
                // 确保没有正在执行的协程时才进入下一句
                if (typingCoroutine == null && !isFirstImageShowing)
                {
                    GoToNextLine();
                }
            }
        }
    }

    void ShowLine(int key)
    {
        if (!storyData.ContainsKey(key) || key == 0)
        {
            EndStory();
            return;
        }

        currentLineKey = key;
        currentLine = storyData[key];

        // 如果是选项节点，则特殊处理
        if (currentLine.ContentSpeaker == "CHOICE_NODE")
        {
            ShowChoices();
            return;
        }

        // 确保对话框显示，选项框隐藏
        dialoguePanel.SetActive(true);
        choicePanel.SetActive(false);
        InsertImage1.gameObject.SetActive(false);
        InsertImage2.gameObject.SetActive(false);

        // 添加对话框淡入效果
        if (!dialoguePanel.GetComponent<CanvasGroup>())
        {
            dialoguePanel.AddComponent<CanvasGroup>();
        }
        CanvasGroup dialogueCG = dialoguePanel.GetComponent<CanvasGroup>();
        dialogueCG.alpha = 0;
        dialogueCG.DOFade(1, fadeDuration);

        ProcessCharacterAction(character1Image, currentLine.Cha1Action, currentLine.CoordinateX1, currentLine.Cha1ImageSource);
        ProcessCharacterAction(character2Image, currentLine.Cha2Action, currentLine.CoordinateX2, currentLine.Cha2ImageSource);

        speakerNameText.text = currentLine.ContentSpeaker;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLineCoroutine(currentLine.Content));

        // 处理插入图片 - 先显示第一张(如果有)
        if (!string.IsNullOrEmpty(currentLine.InsertImage1Path))
        {
            isFirstImageShowing = true;
            ProcessInsertImage(InsertImage1, currentLine.InsertImage1Path, true);
        }
        else if (!string.IsNullOrEmpty(currentLine.InsertImage2Path))
        {
            // 如果没有第一张但有第二张，直接显示第二张
            ProcessInsertImage(InsertImage2, currentLine.InsertImage2Path, false);
        }
    }
    /// <summary>
    /// 处理插入图片的显示/隐藏和卷轴效果
    /// </summary>
    /// <summary>
    /// 处理插入图片的显示/隐藏和卷轴效果
    /// </summary>
    /// <param name="image">要处理的图片组件</param>
    /// <param name="imagePath">图片资源路径</param>
    /// <param name="isFirstImage">是否是第一张图片(需要等待点击显示第二张)</param>
    void ProcessInsertImage(Image image, string imagePath, bool isFirstImage)
    {
        if (image == null) return;

        // 如果提供了图片路径
        if (!string.IsNullOrEmpty(imagePath))
        {
            // 加载图片资源
            Sprite newSprite = Resources.Load<Sprite>(imagePath);
            if (newSprite != null)
            {
                image.sprite = newSprite;

                // 激活图片并设置初始大小为0
                image.gameObject.SetActive(true);
                image.rectTransform.sizeDelta = new Vector2(0, originalImageSizes[image].y);

                // 使用DOTween实现卷轴展开效果
                image.rectTransform.DOSizeDelta(originalImageSizes[image], scrollUnrollDuration)
                    .SetEase(Ease.OutQuad);

                // 同时添加淡入效果
                image.color = new Color(1, 1, 1, 0);
                image.DOFade(1, scrollUnrollDuration)
                    .OnComplete(() =>
                    {
                        if (isFirstImage && !string.IsNullOrEmpty(currentLine.InsertImage2Path))
                        {
                            // 如果是第一张图片且有第二张图片，等待点击
                            waitingForSecondImage = true;
                        }
                        else
                        {
                            // 否则重置状态
                            isFirstImageShowing = false;
                        }
                    });
            }
            else
            {
                Debug.LogWarning($"无法加载图片资源: {imagePath}");
                image.gameObject.SetActive(false);
                isFirstImageShowing = false;
            }
        }
        else
        {
            // 如果没有图片路径，隐藏图片（带淡出效果）
            if (image.gameObject.activeSelf)
            {
                image.DOFade(0, fadeDuration).OnComplete(() =>
                {
                    image.gameObject.SetActive(false);
                    isFirstImageShowing = false;
                });
            }
        }
    }

    void ProcessCharacterAction(Image characterImage, string action, float xPos, string imageSource)
    {
        if (characterImage == null) return;

        // 只有在提供了新的图片资源时才更新Sprite
        if (!string.IsNullOrEmpty(imageSource))
        {
            characterImage.sprite = Resources.Load<Sprite>("Characters/" + imageSource);
        }
        xPos *= 1.5f;
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
                // 确保角色是可见的才执行震动
                if (characterImage.color.a < 0.1f)
                {
                    StartCoroutine(FadeImage(characterImage, 1f));
                }
                StartCoroutine(ShakeCharacter(characterImage));
                break;
            case "Shake":
                StartCoroutine(ShakeCharacter(characterImage));
                break;
            case "Continue":
            default: // 如果action为空或未定义，则什么都不做，保持现状
                break;
        }
    }

    void GoToNextLine()
    {
        // 修正：NextContent可能为0，表示结束
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
        // 【优化2】: 进入选项时，隐藏对话框和所有角色
        dialoguePanel.SetActive(false);
        StartCoroutine(FadeImage(character1Image, 0f));
        StartCoroutine(FadeImage(character2Image, 0f));


        choicePanel.SetActive(true);

        string[] nextKeysStr = currentLine.NextContent.Split(',');

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < nextKeysStr.Length)
            {
                int choiceKey = int.Parse(nextKeysStr[i].Trim());
                if (storyData.ContainsKey(choiceKey))
                {
                    choiceButtons[i].gameObject.SetActive(true);
                    choiceButtons[i].GetComponentInChildren<Text>().text = storyData[choiceKey].Content;

                    choiceButtons[i].onClick.RemoveAllListeners();
                    choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choiceKey));
                }
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnChoiceSelected(int selectedKey)
    {
        // 做出选择后，隐藏选项面板，准备显示下一句对话
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

    /// <summary>
    /// 【新增方法】立即隐藏所有UI元素，用于剧情初始化。
    /// </summary>
    void HideAllImmediately()
    {
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);

        if (character1Image != null)
        {
            Color c = character1Image.color; c.a = 0; character1Image.color = c;
            character1Image.gameObject.SetActive(false);
        }
        if (character2Image != null)
        {
            Color c = character2Image.color; c.a = 0; character2Image.color = c;
            character2Image.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 使用淡出效果结束剧情
    /// </summary>
    void EndStory()
    {
        Debug.Log("剧情结束");
        StartCoroutine(EndStoryCoroutine());
    }

    /// <summary>
    /// ★★★★★【核心修改】★★★★★
    /// 结束剧情的协程，现在加入了黑屏淡入逻辑
    /// </summary>
    IEnumerator EndStoryCoroutine()
    {
        // 1. 淡出所有当前UI元素 (对话框、角色等)
        dialoguePanel.GetComponent<CanvasGroup>()?.DOFade(0, fadeDuration); // 使用安全调用
        if (character1Image != null && character1Image.gameObject.activeSelf) StartCoroutine(FadeImage(character1Image, 0f));
        if (character2Image != null && character2Image.gameObject.activeSelf) StartCoroutine(FadeImage(character2Image, 0f));

        // 等待UI元素淡出完成
        yield return new WaitForSeconds(fadeDuration);

        // 2. 黑屏淡入
        if (blackScreenImage != null)
        {
            blackScreenImage.gameObject.SetActive(true);
            float timer = 0f;
            // 我们可以用一个新的淡入时长，或者复用 fadeDuration
            float blackScreenFadeInTime = 0.5f;
            while (timer < blackScreenFadeInTime)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Clamp01(timer / blackScreenFadeInTime);
                blackScreenImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            // 确保完全不透明
            blackScreenImage.color = Color.black;
        }
        else
        {
            Debug.LogWarning("未指定黑屏图片，将直接切换场景。");
        }

        // 3. 调用GameManager加载下一个场景
        // 因为屏幕已经是黑色的，所以加载过程是看不见的
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartLevel();
        }
        else
        {
            Debug.LogError("找不到GameManager实例！无法加载关卡。");
        }

        // 此时脚本所在的GameObject可以不用禁用了，因为场景马上要被卸载
    }

    #region 动画协程
    IEnumerator FadeImage(Image image, float targetAlpha)
    {
        if (image == null) yield break;

        // 如果要显示图片但它被禁用了，先激活它
        if (targetAlpha > 0f && !image.gameObject.activeSelf)
        {
            image.gameObject.SetActive(true);
        }

        float startAlpha = image.color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            Color color = image.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            image.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        // 确保最终alpha正确
        Color finalColor = image.color;
        finalColor.a = targetAlpha;
        image.color = finalColor;

        // 如果完全透明了，就禁用GameObject以节省性能
        if (targetAlpha <= 0f)
        {
            image.gameObject.SetActive(false);
        }
    }

    IEnumerator MoveCharacter(Image image, float targetX)
    {
        if (image == null || image.color.a < 0.1f) yield break;

        Vector2 startPos = image.rectTransform.anchoredPosition;
        Vector2 endPos = new Vector2(targetX, startPos.y);
        float time = 0;

        while (time < fadeDuration)
        {
            image.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        image.rectTransform.anchoredPosition = endPos;
    }

    IEnumerator ShakeCharacter(Image image)
    {
        if (image == null || image.color.a < 0.1f) yield break;

        float duration = 0.5f;
        float magnitude = 10f;
        Vector2 startPos = image.rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            image.rectTransform.anchoredPosition = startPos + new Vector2(x, y);
            elapsed += Time.deltaTime;
            yield return null;
        }
        image.rectTransform.anchoredPosition = startPos;
    }
    #endregion
}