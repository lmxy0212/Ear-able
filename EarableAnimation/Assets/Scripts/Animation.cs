using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    [Header("3D Models")]
    public GameObject m_initialObj;
    public GameObject m_parentObj;
    public GameObject m_topObj;
    public GameObject m_bodyObj;
    public GameObject m_bottomObj;

    [Header("Frame0: Rotation Settings")]
    public float m_rotationTime1;
    public float m_rotationAngle;

    [Header("Frame1: Rotation Settings")]
    public float m_rotationTime2;
    public GameObject m_step2Target;

    [Header("Frame2: Exploded View Settings")]
    public GameObject m_topPosExploded;
    public GameObject m_bodyPosExploded;
    public GameObject m_bottomPosExploded;
    public float m_step2lerpDuration;

    [Header("Frame3: Move Left Settings")]
    public GameObject m_parentSlideLeft;
    public float m_step3moveDuration;

    [Header("Frame4: Exploded View Expand Settings")]
    public GameObject m_topPosExplodedClose;
    public GameObject m_bodyPosExplodedOut; 
    public float m_step4explodeDuration;
    public float m_step4lerpleftDuration;

    [Header("Frame5: Body at Center Settings")]
    public GameObject m_bodyCenterPos;
    public GameObject m_topOutPos;
    public GameObject m_bottomOutPos;
    public float m_step5lerpDuration;


    [Header("Counter")]
    public int m_stepCounter;

    void start(){
        m_parentObj.SetActive(false);
        m_parentSlideLeft.SetActive(false);
        m_initialObj.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch(m_stepCounter) 
            {
            case 0:
                RotateObj(m_initialObj, m_rotationTime1, m_rotationAngle);
                break;
            case 1:
                Step2Lerp(m_initialObj, m_step2Target.transform, m_rotationTime2);
                break;
            case 2:
                ExplodeObj(m_initialObj, m_parentObj, m_bottomPosExploded.transform.position, m_bodyPosExploded.transform.position, m_topPosExploded.transform.position, m_step2lerpDuration);
                break;
            case 3:
                StartLerpingPosition(m_parentObj.transform, m_parentSlideLeft.transform.position, m_step3moveDuration);
                break;
            case 4:
                ExpodedViewSlideOut(m_bodyObj, m_bodyPosExplodedOut.transform.position, m_step4explodeDuration);
                break;
            case 5:
                StartCoroutine(LerpToTransform(m_bodyObj, m_bodyCenterPos.transform, m_step5lerpDuration));
                StartLerpingPosition(m_topObj.transform, m_topOutPos.transform.position, m_step5lerpDuration-0.05f);
                StartLerpingPosition(m_bottomObj.transform, m_bottomOutPos.transform.position, m_step5lerpDuration-0.05f);
                break;
            default:
                Debug.Log("Counter reaches the last frame...");
                break;
            }
            m_stepCounter = m_stepCounter + 1;
        }
    }

    
    public void ExpodedViewSlideOut(GameObject body, Vector3 bodySlideOutPos, float lerpDuration)
    {
        StartLerpingPosition(body.transform, bodySlideOutPos, lerpDuration);
    }

     public void ExplodeObj(GameObject oldObj, GameObject newObj, Vector3 position0, Vector3 position1, Vector3 position2, float lerpDuration)
    {
        // Hide oldObj and show newObj
        oldObj.SetActive(false);
        newObj.SetActive(true);

        // Check if newObj has at least three children
        if (newObj.transform.childCount < 3)
        {
            Debug.LogError("newObj does not have enough children.");
            return;
        }

        // Start lerping positions for the first three children
        StartLerpingPosition(newObj.transform.GetChild(0), position0, lerpDuration);
        StartLerpingPosition(newObj.transform.GetChild(1), position1, lerpDuration);
        StartLerpingPosition(newObj.transform.GetChild(2), position2, lerpDuration);
    }

    private void StartLerpingPosition(Transform child, Vector3 targetPosition, float duration)
    {
        StartCoroutine(LerpPosition(child, targetPosition, duration));
    }

    IEnumerator LerpPosition(Transform objectToLerp, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = objectToLerp.position;
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            objectToLerp.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position matches the target position exactly
        objectToLerp.position = targetPosition;
    }

    public void RotateObj(GameObject obj, float duration, float angle)
    {
        StartCoroutine(Rotate(obj, duration, angle));
    }

    IEnumerator Rotate(GameObject obj, float duration, float angle)
    {
        float startRotation = obj.transform.eulerAngles.y;
        float endRotation = startRotation + angle; // Rotate 360 degrees
        float t = 0.0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % angle;
            obj.transform.eulerAngles = new Vector3(obj.transform.eulerAngles.x, yRotation, obj.transform.eulerAngles.z);
            yield return null;
        }

        // Ensure the rotation is exactly 360 degrees at the end
        obj.transform.eulerAngles = new Vector3(obj.transform.eulerAngles.x, endRotation % angle, obj.transform.eulerAngles.z);
    }

    public void Step2Lerp(GameObject original, Transform target, float duration)
    {
        if (original == null || target == null)
        {
            Debug.LogError("Original object or target transform is not assigned.");
            return;
        }

        StartCoroutine(LerpToTransform(original, target, duration));
    }

    IEnumerator LerpToTransform(GameObject original, Transform target, float duration)
    {
        Vector3 startPosition = original.transform.position;
        Quaternion startRotation = original.transform.rotation;
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Lerp position
            original.transform.position = Vector3.Lerp(startPosition, target.position, t);

            // Lerp rotation
            original.transform.rotation = Quaternion.Lerp(startRotation, target.rotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position and rotation match the target exactly
        original.transform.position = target.position;
        original.transform.rotation = target.rotation;
    }
}
