using UnityEngine;
using Unity.VisualScripting;

[UnitTitle("Bounce Cube")]
[UnitCategory("Custom Nodes")]
public class CubeAnimation : Unit
{
    [DoNotSerialize]
    public ControlInput trigger;

    [DoNotSerialize]
    public ControlOutput animationComplete;

    [DoNotSerialize]
    public ValueInput cubeObject;

    [DoNotSerialize]
    public ValueInput jumpHeight;

    [DoNotSerialize]
    public ValueInput compressionAmount;

    [DoNotSerialize]
    public ValueInput duration;

    private bool isAnimating = false; // Flag per gestire animazioni multiple

    protected override void Definition()
    {
        trigger = ControlInput("trigger", Execute);
        animationComplete = ControlOutput("animationComplete");

        cubeObject = ValueInput<GameObject>("cubeObject");
        jumpHeight = ValueInput<float>("jumpHeight", 2f);
        compressionAmount = ValueInput<float>("compressionAmount", 0.5f);
        duration = ValueInput<float>("duration", 0.5f);

        Requirement(cubeObject, trigger);
        Succession(trigger, animationComplete);
    }

    private ControlOutput Execute(Flow flow)
    {
        // Se l'animazione è già in corso, ignora il trigger
        if (isAnimating)
        {
            return null;
        }

        isAnimating = true; // Imposta il flag a true

        GameObject cube = flow.GetValue<GameObject>(cubeObject);
        float height = flow.GetValue<float>(jumpHeight);
        float compression = flow.GetValue<float>(compressionAmount);
        float animDuration = flow.GetValue<float>(duration);

        if (cube != null)
        {
            MonoBehaviour monoBehaviour = cube.GetComponent<MonoBehaviour>();
            if (monoBehaviour != null)
            {
                monoBehaviour.StartCoroutine(AnimateBounce(cube, height, compression, animDuration));
            }
        }

        return animationComplete;
    }

    private System.Collections.IEnumerator AnimateBounce(GameObject cube, float jumpHeight, float compressionAmount, float duration)
    {
        Transform transform = cube.transform;
        Vector3 originalScale = transform.localScale;
        Vector3 originalPosition = transform.position;

        float halfDuration = duration / 2f;
        
        float timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / halfDuration;

            transform.localScale = new Vector3(
                originalScale.x,
                Mathf.Lerp(originalScale.y, originalScale.y * compressionAmount, progress),
                originalScale.z
            );

            transform.position = new Vector3(
                originalPosition.x,
                Mathf.Lerp(originalPosition.y, originalPosition.y + jumpHeight, progress),
                originalPosition.z
            );

            yield return null;
        }

        timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / halfDuration;

            transform.localScale = new Vector3(
                originalScale.x,
                Mathf.Lerp(originalScale.y * compressionAmount, originalScale.y, progress),
                originalScale.z
            );

            transform.position = new Vector3(
                originalPosition.x,
                Mathf.Lerp(originalPosition.y + jumpHeight, originalPosition.y, progress),
                originalPosition.z
            );

            yield return null;
        }

        transform.localScale = originalScale;
        transform.position = originalPosition;

        isAnimating = false;
    }
}
