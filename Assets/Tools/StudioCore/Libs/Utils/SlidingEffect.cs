using UnityEngine;
using System.Collections;

public class SlidingEffect : MonoBehaviour
{
    #region PUBLIC_VARS

    [Header("Animation-Curve")]
    public AnimationCurve slideCurve;
    public Vector3 finalPosition;
    public float startDelay;
    public float exitDelay;
    [SerializeField]
    private float time = 1;

    #endregion

    #region PRIVATE_VARS

    private Vector3 initialPosition;
    private WaitForSeconds startingDelay;
    private WaitForSeconds endingDelay;

    #endregion

    #region UNITY_CALLBACKS

    private void Awake()
    {
        startingDelay = new WaitForSeconds(startDelay);
        endingDelay = new WaitForSeconds(exitDelay);

    }

    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            initialPosition = rectTransform.anchoredPosition;
        }
        else
        {
            initialPosition = transform.localPosition;
        }
    }
    #endregion

    #region PUBLIC_FUNCTIONS

    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    #region CO-ROUTINESprivate
    public static void StartSlide(GameObject obj, float time = 0)
    {
        var script = obj.GetComponent<SlidingEffect>();
        if (time >= 0)
        {
            script.time = time;
        }
        script.StartCoroutine(script.EntryEffect());
    }

    public IEnumerator EntryEffect()
    {
        float i = 0;
        float rate = 1 / time;

        yield return startingDelay;
        RectTransform rectTransform = GetComponent<RectTransform>();

        while (i < 1)
        {
            i += rate * Time.deltaTime;
            if (rectTransform == null)
            {
                transform.localPosition = Vector3.Lerp(initialPosition, finalPosition, slideCurve.Evaluate(i));
            }
            else
            {
                rectTransform.anchoredPosition = Vector3.Lerp(initialPosition, finalPosition, slideCurve.Evaluate(i));
            }
            yield return 0;
        }
    }

    public IEnumerator ExitEffect()
    {
        float i = 0;
        float rate = 1 / time;

        yield return endingDelay;

        while (i < 1)
        {
            i += rate * Time.deltaTime;
            transform.localPosition = Vector3.Lerp(finalPosition, initialPosition, slideCurve.Evaluate(i));
            yield return 0;
        }
    }

    public static async void MoveTo(GameObject obj, Vector3 pos, float time = 0)
    {
        float i = 0;
        float rate = 1 / time;

        while (i < 1)
        {
            i += rate * Time.deltaTime;
            if (obj!=null){
                obj.transform.localPosition = Vector3.Lerp(obj.transform.localPosition, pos, i);
                await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(Time.deltaTime));
            }
            else{
                break;
            }
        }
    }
    #endregion

    #region EVENT_HANDLERS

    #endregion

    #region UI_CALLBACKS

    #endregion
}