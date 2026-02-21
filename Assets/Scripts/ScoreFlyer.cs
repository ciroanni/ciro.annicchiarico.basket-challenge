using TMPro;
using UnityEngine;

public class ScoreFlyer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private float duration = 0.9f;
    [SerializeField] private Vector3 moveOffset = new Vector3(0f, 1.2f, 0.2f);
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0.6f, 1f, 1f);
    [SerializeField] private bool faceCamera = true;

    private float elapsed;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Color baseColor;

    private void Awake()
    {
        if (label == null)
        {
            label = GetComponentInChildren<TextMeshProUGUI>();
        }

        startPosition = transform.position;
        endPosition = startPosition + moveOffset;

        if (label != null)
        {
            baseColor = label.color;
        }
    }

    public void Setup(string text, Color color)
    {
        if (label != null)
        {
            label.text = text;
            label.color = color;
            baseColor = color;
        }
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        float moveT = moveCurve.Evaluate(t);
        transform.position = Vector3.LerpUnclamped(startPosition, endPosition, moveT);

        float scaleT = scaleCurve.Evaluate(t);
        transform.localScale = Vector3.one * scaleT;

        if (label != null)
        {
            Color color = baseColor;
            color.a = Mathf.Lerp(1f, 0f, t);
            label.color = color;
        }

        if (faceCamera && Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
