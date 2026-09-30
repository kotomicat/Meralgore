using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrace : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float initialWidth = 0.2f;
    [SerializeField] private float visibleDuration = 0.1f;
    [SerializeField] private float fadeDuration = 0.002f;

    public void Show(Vector3[] points)
    {
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);

        lineRenderer.startWidth = initialWidth;
        lineRenderer.endWidth = initialWidth;
        lineRenderer.enabled = true;

        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(visibleDuration);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float fadeStep = Mathf.Clamp01(elapsed / fadeDuration);
            float width = Mathf.Lerp(initialWidth, 0f, fadeStep);

            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            yield return null;
        }

        lineRenderer.enabled = false;
        Destroy(gameObject);
    }

}
