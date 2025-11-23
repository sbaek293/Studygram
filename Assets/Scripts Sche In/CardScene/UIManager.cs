    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        public CanvasGroup overlayFade;
        public GameObject panelMainMenu, panelCardCreator, panelSetSelector, panelStudyMode;
        public GameObject buyPopup;      // Assign in Inspector
        public TMP_Text buyTitleText;    // e.g. "Buy Set: Animals"
        public TMP_Text buyPriceText;    // e.g. "Price: 50 Coins"
        public Button buyButton;
        public Button cancelBuyButton;

        private string pendingBuySetId = "";
        private string pendingBuySetName = "";

        public void ShowBuyPopup(string setName, string setId)
        {
            pendingBuySetId = setId;
            pendingBuySetName = setName;

            buyTitleText.text = "Buy Set: " + setName;

            // Load price from Firebase (async)
            OnlineCardManager.Instance.GetSetPrice(setId, (price) =>
            {
                buyPriceText.text = "Price: " + price + " Coins";
            });

            buyPopup.SetActive(true);

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() =>
            {
                OnlineCardManager.Instance.BuySet(pendingBuySetId);
                buyPopup.SetActive(false);
            });

            cancelBuyButton.onClick.RemoveAllListeners();
            cancelBuyButton.onClick.AddListener(() =>
            {
                buyPopup.SetActive(false);
            });
        }

    public void HideBuyPopup()
    {
        buyPopup.SetActive(false);
    }
    private IEnumerator Start()
        {
            while (string.IsNullOrEmpty(AppContext.UserId))
                yield return null;

            Instance = this;
            ShowMainMenu();
            DataManager.LoadData();
            OnlineCardManager.Instance.DownloadAllUserSets();
        }
        public void ShowPanel(GameObject activePanel)
        {
            StartCoroutine(SwitchPanel(activePanel));
        }

        IEnumerator SwitchPanel(GameObject target)
        {
            yield return Fade(1);
            panelMainMenu.SetActive(false);
            panelCardCreator.SetActive(false);
            panelSetSelector.SetActive(false);
            panelStudyMode.SetActive(false);
            target.SetActive(true);
            yield return Fade(0);
        }

        IEnumerator Fade(float toAlpha)
        {
            float start = overlayFade.alpha;
            float t = 0;
            while (t < 0.25f)
            {
                overlayFade.alpha = Mathf.Lerp(start, toAlpha, t / 0.25f);
                t += Time.deltaTime;
                yield return null;
            }
            overlayFade.alpha = toAlpha;
        }

        // simple public helpers
        public void ShowMainMenu() => ShowPanel(panelMainMenu);
        public void ShowCardCreator() => ShowPanel(panelCardCreator);
        public void ShowSetSelector() => ShowPanel(panelSetSelector);
        public void ShowStudyMode() => ShowPanel(panelStudyMode);
    }
