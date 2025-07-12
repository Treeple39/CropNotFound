using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using CustomStorySystem; // 确保你的数据结构命名空间被正确引用
using DG.Tweening;      // 确保你已导入DOTween插件
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.VisualScripting; // 引入Linq以使用 .Where() 和 .ToList()

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
    public Button[] choiceButtons;
    [Tooltip("用于场景切换的黑屏遮罩")]
    public Image blackScreenImage;

    [Header("效果参数")]
    public float typeSpeed = 0.05f;
    public float typeTime = 0.1f; // 打字音效的平均间隔
    public float fadeDuration = 0.3f;
    public float chMoveDuration = 0.3f;
    public float scrollUnrollDuration = 1.0f;
    public float currentScore;

    [Header("打字机音效")]
    public List<AudioClip> typingClips;
    [Range(0.1f, 1f)]
    public Vector2 typingVolumeRange = new Vector2(0.5f, 0.7f);
    [Tooltip("打字音高抖动范围")]
    public Vector2 typingPitchRange = new Vector2(0.9f, 1.1f);

    // --- 内部变量 ---
    private Dictionary<int, StoryLine> storyData;
    private StoryLine currentLine;
    private int currentLineKey = 1;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private Dictionary<Image, Vector2> originalPositions = new Dictionary<Image, Vector2>();
    private Dictionary<Image, Vector2> originalImageSizes = new Dictionary<Image, Vector2>();
    // 定义卡池等级
    private enum Rarity { B, A, S, SSS }

    public bool End = false;
    public GameObject creditsPanel;
    // 定义结局Key与卡池等级的对应关系 (硬编码)
    // Key = 剧情Key, Value = 稀有度
    private readonly Dictionary<int, Rarity> endingRarityMap = new Dictionary<int, Rarity>
    {
        // B级结局
        { 7, Rarity.B },  // 天使
        { 10, Rarity.B }, // 龙
        { 13, Rarity.B }, // 恶魔

        // A级结局
        { 16, Rarity.A }, // 精灵
        { 19, Rarity.A }, // 人类
        { 22, Rarity.A }, // 狼人

        // S级结局
        { 25, Rarity.S }, // 二郎神
        { 28, Rarity.S }, // 神眷者
        { 31, Rarity.S }, // 吸血鬼

        // SSS级结局
        { 34, Rarity.SSS } // 新神
    };

    void Start()
    {
        if (blackScreenImage != null)
        {
            blackScreenImage.color = new Color(0, 0, 0, 0);
            blackScreenImage.gameObject.SetActive(false);
        }

        storyData = JsonHelper.LoadStory(storyJsonFile);
        if (storyData == null)
        {
            Debug.LogError("剧情数据加载失败，请检查JSON文件！");
            this.enabled = false;
            return;
        }
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }

        InitializeImage(character1Image);
        InitializeImage(character2Image);
        InitializeImage(InsertImage1);

        HideAllImmediately();
        ShowLine(currentLineKey);
    }

    private void InitializeImage(Image image)
    {
        if (image == null) return;
        originalPositions[image] = image.rectTransform.anchoredPosition;
        originalImageSizes[image] = image.rectTransform.sizeDelta;
        image.gameObject.SetActive(false);
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
            else if (typingCoroutine == null)
            {
                GoToNextLine();
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

        if (currentLine.ContentSpeaker == "CHOICE_NODE")
        {
            ShowChoices();
            return;
        }

        dialoguePanel.SetActive(true);
        choicePanel.SetActive(false);

        if (InsertImage1 != null && (currentLine == null || !currentLine.InsertImage1Persistent))
        {
            InsertImage1.gameObject.SetActive(false);
        }

        CanvasGroup dialogueCG = dialoguePanel.GetComponent<CanvasGroup>() ?? dialoguePanel.AddComponent<CanvasGroup>();
        //dialogueCG.alpha = 0;
        //dialogueCG.DOFade(1, fadeDuration);

        ProcessCharacterAction(character1Image, currentLine.Cha1Action, currentLine.CoordinateX1, currentLine.Cha1ImageSource);
        ProcessCharacterAction(character2Image, currentLine.Cha2Action, currentLine.CoordinateX2, currentLine.Cha2ImageSource);

        speakerNameText.text = currentLine.ContentSpeaker;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLineCoroutine(currentLine.Content));

        if (!string.IsNullOrEmpty(currentLine.InsertImage1Path))
            ProcessInsertImage(InsertImage1, currentLine.InsertImage1Path);
    }

    void ProcessInsertImage(Image image, string imagePath)
    {
        if (image == null || image.gameObject == null) return;

        if (string.IsNullOrEmpty(imagePath))
        {
            Debug.Log(imagePath + "Do not exit");
            if (image.gameObject.activeSelf)
            {
                Config.ImageUFX.UFX_Fade(image, scrollUnrollDuration, () => image.gameObject.SetActive(false));
            }
            else
            {
                image.gameObject.SetActive(false);
            }

            return;
        }

        imagePath = $"{imagePath}";
        Sprite newSprite = Resources.Load<Sprite>(imagePath);

        if (newSprite != null)
        {
            image.sprite = newSprite;
            image.gameObject.SetActive(true);

            if (originalImageSizes.TryGetValue(image, out Vector2 originalSize))
            {
                image.rectTransform.sizeDelta = new Vector2(0, originalSize.y);
                Config.ImageUFX.UFX_Stretch(image, originalSize, scrollUnrollDuration);
            }
            else
            {
                //Debug.LogWarning($"未找到 {image} 的原始尺寸，无法调整大小");
            }
        }
        else
        {
            Debug.LogWarning($"无法加载图片资源: {imagePath}");
            image.gameObject.SetActive(false);
        }
    }
    void ProcessCharacterAction(Image characterImage, string action, float xPos, string imageSource)
    {
        if (characterImage == null) return;

        xPos *= 0.8f;

        switch (action)
        {
            case "AppearAt":
                if (!string.IsNullOrEmpty(imageSource)) characterImage.sprite = Resources.Load<Sprite>("Characters/" + imageSource);
                characterImage.rectTransform.anchoredPosition = new Vector2(xPos, originalPositions[characterImage].y);
                StartCoroutine(FadeImage(characterImage, 1f));
                break;

            case "FadeAt":
                if (string.IsNullOrEmpty(imageSource)) break;
                characterImage.rectTransform.anchoredPosition = new Vector2(xPos, originalPositions[characterImage].y);
                StartCoroutine(FadeCharacter(characterImage, "Characters/" + imageSource));
                break;

            case "Disappear":
                StartCoroutine(FadeImage(characterImage, 0f));
                break;
            case "MoveTo":
                if (!string.IsNullOrEmpty(imageSource)) characterImage.sprite = Resources.Load<Sprite>("Characters/" + imageSource);
                StartCoroutine(MoveCharacter(characterImage, xPos));
                break;
            case "ShakeAt":
                if (!string.IsNullOrEmpty(imageSource)) characterImage.sprite = Resources.Load<Sprite>("Characters/" + imageSource);
                characterImage.rectTransform.anchoredPosition = new Vector2(xPos, originalPositions[characterImage].y);
                if (characterImage.color.a < 0.1f) StartCoroutine(FadeImage(characterImage, 1f));
                StartCoroutine(ShakeCharacter(characterImage));
                break;
            case "Shake":
                if (!string.IsNullOrEmpty(imageSource)) characterImage.sprite = Resources.Load<Sprite>("Characters/" + imageSource);
                StartCoroutine(ShakeCharacter(characterImage));
                break;
            case "Continue":
                if (!string.IsNullOrEmpty(imageSource)) characterImage.sprite = Resources.Load<Sprite>("Characters/" + imageSource);
                break;
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

        string nextContentString = currentLine.ContinueTag;
        if (nextContentString != "?")
        {
            nextContentString = currentLine.NextContent;
        }
        int nextKey;

        // ★★★★★【核心修改 #2：执行抽卡逻辑】★★★★★
        // 我们约定，当NextContent是"?"时，执行抽卡逻辑
        Debug.Log(nextContentString);
        if (nextContentString == "?")
        {
            Debug.Log("检测到抽卡节点 '?'，开始根据分数选择结局...");
            nextKey = DetermineEndingByScore();


            if (nextKey == 0) // 如果抽卡失败，则结束剧情
            {
                EndStory();
                return;
            }
        }
        else
        {
            Debug.Log("原来的");
            if (nextContentString.Contains("|"))
            {
                // (保留旧的随机分支逻辑，以备不时之需)
                string[] possibleNextKeys = nextContentString.Split('|');
                string randomKeyString = possibleNextKeys[Random.Range(0, possibleNextKeys.Length)];
                if (!int.TryParse(randomKeyString.Trim(), out nextKey))
                {
                    Debug.LogError($"随机分支解析失败！'{randomKeyString}' 不是一个有效的数字。");
                    EndStory();
                    return;
                }
            }
            else
            {
                if (!int.TryParse(nextContentString.Trim(), out nextKey))
                {
                    Debug.LogError($"NextContent 解析失败！'{nextContentString}' 不是一个有效的数字。");
                    EndStory();
                    return;
                }
            }
        }
        // ★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★

        ShowLine(nextKey);
    }

    /// <summary>
    /// 【新增方法】根据当前分数决定结局
    /// </summary>
    private int DetermineEndingByScore()
    {
        // 1. 获取当前分数
        currentScore = Score.score;

        Debug.Log($"当前分数为: {currentScore}，开始匹配卡池...");

        // 2. 根据分数确定可抽的卡池等级
        List<Rarity> availablePool = new List<Rarity>();
        if (currentScore <= 500)
        {
            availablePool.Add(Rarity.B);
        }
        else if (currentScore <= 1200)
        {
            availablePool.Add(Rarity.A);
            availablePool.Add(Rarity.B);
        }
        else if (currentScore <= 2000)
        {
            availablePool.Add(Rarity.S);
            availablePool.Add(Rarity.A);
        }
        else // 2000+
        {
            availablePool.Add(Rarity.SSS);
            availablePool.Add(Rarity.S);
        }

        Debug.Log("可抽卡池: " + string.Join(", ", availablePool));

        // 3. 从对应关系表中，筛选出所有在可抽卡池内的结局
        List<int> possibleEndings = endingRarityMap
            .Where(pair => availablePool.Contains(pair.Value)) // 筛选出所有稀有度在卡池中的键值对
            .Select(pair => pair.Key) // 只取出它们的Key（也就是剧情ID）
            .ToList(); // 转换成一个列表

        // 4. 从可能的结局中随机抽取一个
        if (possibleEndings.Count > 0)
        {
            int randomIndex = Random.Range(0, possibleEndings.Count);
            int chosenEndingKey = possibleEndings[randomIndex];
            Debug.Log($"从 {possibleEndings.Count} 个可能结局中，抽中了 Key: {chosenEndingKey}");

            if (ArchiveManager.Instance.IsUnlocked(chosenEndingKey) == false)
            {
                var item = ArchiveManager.Instance.GetItem(StoryKeyToArchiveId(chosenEndingKey));

                if (item != null)
                {
                    ItemUIData messageData = new ItemUIData
                    {
                        messageImage = Resources.Load<Sprite>("Characters/" + item.imagePath),
                        message = $"解锁了新的宝子！好耶！",
                        messageID = -1,
                    };

                    EventHandler.CallMessageShow(messageData);
                }
                else
                {
                    Debug.Log("item is null");
                }
            }
            else
            {
                Debug.Log("不是新角色");
            }

            ArchiveManager.Instance.UnlockByStoryKey(chosenEndingKey);

            // 根据抽到的结局稀有度，触发科技点增加事件
            if (endingRarityMap.TryGetValue(chosenEndingKey, out Rarity rarity))
            {
                float techPoints = 0f;
                switch (rarity)
                {
                    case Rarity.B:
                        techPoints = 10f;
                        break;
                    case Rarity.A:
                        techPoints = 12f;
                        break;
                    case Rarity.S:
                        techPoints = 15f;
                        break;
                    case Rarity.SSS:
                        techPoints = 20f;
                        break;
                }

                Debug.Log($"抽到稀有度为 {rarity} 的结局，触发加科技点数：{techPoints}");
                EventHandler.CallTechPointChange(techPoints);
                UIManager.Instance.UILevelUpPanel.OpenTab();
            }


            return chosenEndingKey;
        }
        else
        {
            Debug.LogError("根据当前分数和卡池规则，找不到任何可选的结局！");
            return 0; // 返回0表示失败
        }
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
            if (i < nextKeysStr.Length)
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

    private IEnumerator TypeLineCoroutine(string line)
    {
        isTyping = true;
        contentText.text = "";
        float nextSfxTime = Random.Range(0f, typeTime);
        float sfxTimer = 0f;

        foreach (char c in line)
        {
            contentText.text += c;
            float wait = typeSpeed;
            float elapsed = 0f;
            while (elapsed < wait)
            {
                float dt = Time.deltaTime;
                elapsed += dt;
                sfxTimer += dt;
                if (sfxTimer >= nextSfxTime && typingClips != null && typingClips.Count > 0)
                {
                    var clip = typingClips[Random.Range(0, typingClips.Count)];
                    float vol = Random.Range(typingVolumeRange.x, typingVolumeRange.y);
                    float pit = Random.Range(typingPitchRange.x, typingPitchRange.y);
                    if (AudioManager.S != null) AudioManager.S.PlayFX(clip, vol, pit);
                    sfxTimer = 0f;
                    nextSfxTime = Random.Range(0f, typeTime);
                }
                yield return null;
            }
        }
        isTyping = false;
        typingCoroutine = null;
    }

    void CompleteLine()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = null;
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

        if (GameManager.Instance != null)
        {
            if (End)
            {
                GameManager.Instance.GoToThanks();
            }
            else
            {
                GameManager.Instance.StartLevel();
            }
        }
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
        yield return image.rectTransform.DOAnchorPosX(targetX, chMoveDuration).SetEase(Ease.OutQuad).WaitForCompletion();
    }

    IEnumerator ShakeCharacter(Image image)
    {
        if (image == null || image.color.a < 0.1f) yield break;
        yield return image.transform.DOShakePosition(0.5f, new Vector3(10, 10, 0), 20).WaitForCompletion();
    }


    private IEnumerator FadeCharacter(Image image, string spritePath)
    {
        if (image == null) yield break;

        Sprite newSprite = Resources.Load<Sprite>(spritePath);
        if (newSprite == null)
        {
            Debug.LogWarning($"[FadeCharacter] 无法加载贴图：{spritePath}");
            yield break;
        }

        image.sprite = newSprite;
        image.gameObject.SetActive(true);
        var rt = image.rectTransform;

        if (!originalImageSizes.ContainsKey(image))
        {
            Debug.LogError($"没有在Start中为 {image.name} 记录原始尺寸！");
            yield break;
        }

        // ★★★★★【核心修改：动态计算尺寸以保持比例】★★★★★

        // 1. 开启“保持长宽比”模式
        image.preserveAspect = true;

        // 2. 以预设的框体高度为基准
        float referenceHeight = originalImageSizes[image].y;

        // 3. 根据新加载图片的原始宽高比，计算出在保持高度不变的情况下，应有的宽度
        float aspectRatio = newSprite.rect.width / newSprite.rect.height;
        Vector2 targetSize = new Vector2(referenceHeight * aspectRatio, referenceHeight);

        // 4. 从“宽度=0”开始展开
        rt.sizeDelta = new Vector2(0, targetSize.y);

        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);

        // 5. 同时开始淡入和尺寸动画，动画的目标是新计算出的 targetSize
        image.DOFade(1f, scrollUnrollDuration);
        rt.DOSizeDelta(targetSize, scrollUnrollDuration).SetEase(Ease.OutQuad);

        // ★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★

        yield return new WaitForSeconds(scrollUnrollDuration);
    }
    #endregion
    private int StoryKeyToArchiveId(int storyKey) =>
   (storyKey - 7) / 3 + 1; // 7→1, 10→2, 13→3...
}