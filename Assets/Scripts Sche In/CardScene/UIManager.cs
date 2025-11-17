using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public CanvasGroup overlayFade;
    public GameObject panelMainMenu, panelCardCreator, panelSetSelector, panelStudyMode;

    void Awake()
    {
        Instance = this;
        ShowMainMenu();
        DataManager.LoadData();
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
