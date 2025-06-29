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
    public Image InsertImage1;
    public Image InsertImage2;
    public Button[] choiceButtons;
    [Tooltip("用于场景切换的黑屏遮罩")]
    public Image blackScreenImage;

    [Header("效果参数")]
    public float typeSpeed = 0.05f;
    public float typeTime = 0.1f; // 打字音效的平均间隔
    public float fadeDuration = 0.3f;
    public float scrollUnrollDuration = 1.0f;

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
        if (InsertImage1 != null) InsertImage1.gameObject.SetActive(false);
        //if (InsertImage2 != null) InsertImage2.gameObject.SetActive(false);

        CanvasGroup dialogueCG = dialoguePanel.GetComponent<CanvasGroup>() ?? dialoguePanel.AddComponent<CanvasGroup>();
        dialogueCG.alpha = 0;
        dialogueCG.DOFade(1, fadeDuration);

        ProcessCharacterAction(character1Image, currentLine.Cha1Action, currentLine.CoordinateX1, currentLine.Cha1ImageSource);
        ProcessCharacterAction(character2Image, currentLine.Cha2Action, currentLine.CoordinateX2, currentLine.Cha2ImageSource);

        speakerNameText.text = currentLine.ContentSpeaker;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLineCoroutine(currentLine.Content));

        ProcessInsertImage(InsertImage1, currentLine.InsertImage1Path);
    }

    void ProcessInsertImage(Image image, string imagePath)
    {
        if (image == null) return;
        if (string.IsNullOrEmpty(imagePath))
        {
            if (image.gameObject.activeSelf)
            {
                image.DOFade(0, fadeDuration).OnComplete(() => image.gameObject.SetActive(false));
            }
            return;
        }

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

        string nextContentString = currentLine.NextContent;
        int nextKey;

        if (nextContentString.Contains("|"))
        {
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
        if(InsertImage2!=null)
            InsertImage2.gameObject.SetActive(true);

        yield return new WaitForSeconds(scrollUnrollDuration);
    }
    #endregion
}