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

    [Header("Frame5: Exploded components Setting")]
    public GameObject m_componentParent;
    public float durationOut;
    public GameObject m_lipo;
    public GameObject m_microcontroller;
    public GameObject m_wireless;
    public GameObject m_PET1;
    public GameObject m_PET2;

    public GameObject m_lipoEnd;
    public GameObject m_microcontrollerEnd;
    public GameObject m_wirelessEnd;
    public GameObject m_PET1End;
    public GameObject m_PET2End;
    

    [Header("Frame5: Body at Center Settings")]
    public GameObject m_bodyCenterPos;
    public GameObject m_topOutPos;
    public GameObject m_bottomOutPos;
    public float m_step5lerpDuration;

    public Vector3 targetScale = new Vector3(1f, 1f, 1f);


    [Header("Counter")]
    public int m_stepCounter;

    void start(){
        m_parentObj.SetActive(false);
        m_parentSlideLeft.SetActive(false);
        m_initialObj.SetActive(true);
        Physics.gravity = new Vector3(0, -50f, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch(m_stepCounter) 
            {
                case 0:
                    m_initialObj.GetComponent<Collider>().enabled = false;
                    m_initialObj.GetComponent<Rigidbody>().isKinematic = true;
                    RotateObj(m_initialObj, m_rotationTime1, m_rotationAngle, targetScale);
                    break;
                case 1:
                    m_initialObj.GetComponent<Collider>().enabled = false;
                    m_initialObj.GetComponent<Rigidbody>().isKinematic = true;
                    Step2Lerp(m_initialObj, m_step2Target.transform, m_rotationTime2);
                    // Vector3 targetScale = new Vector3(1f, 1f, 1f);
                    ScaleObj(m_initialObj, m_rotationTime1, targetScale);
                    break;
                case 2:
                    ExplodeObj(m_initialObj, m_parentObj, m_bottomPosExploded.transform.position, m_bodyPosExploded.transform.position, m_topPosExploded.transform.position, m_step2lerpDuration);
                    break;
                // case 3:
                //     // StartLerpingPosition(m_parentObj.transform, m_parentSlideLeft.transform.position, m_step3moveDuration);
                //     break;
                case 3:
                    ExpodedViewSlideOut(m_bodyObj, m_bodyPosExplodedOut.transform, m_step4explodeDuration);
                    break;
                // case 4:
                    // m_componentParent.SetActive(true);
                    // StartLerpingPosition(m_lipo.transform, m_lipoEnd.transform.position, durationOut);
                    // StartLerpingPosition(m_microcontroller.transform, m_microcontrollerEnd.transform.position, durationOut);
                    // StartLerpingPosition(m_wireless.transform, m_wirelessEnd.transform.position, durationOut);
                    // StartCoroutine(LerpToTransform(m_PET1, m_PET1End.transform, durationOut));
                    // StartCoroutine(LerpToTransform(m_PET2, m_PET2End.transform, durationOut));
                    // break;
                case 4:
                    m_componentParent.SetActive(false);
                    StartCoroutine(LerpToTransform(m_bodyObj, m_bodyCenterPos.transform, m_step5lerpDuration));
                    StartLerpingPosition(m_topObj.transform, m_topOutPos.transform.position, m_step5lerpDuration-0.05f);
                    StartLerpingPosition(m_bottomObj.transform, m_bottomOutPos.transform.position, m_step5lerpDuration-0.05f);
                    break;
                case 5:
                    RotateObj(m_bodyObj, 7f, 360f, targetScale);
                    break;
                default:
                    Debug.Log("Counter reaches the last frame...");
                    break;
            }
            m_stepCounter = m_stepCounter + 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            m_stepCounter = m_stepCounter - 1;
        }
    }

    
    public void ExpodedViewSlideOut(GameObject body, Transform bodySlideOutPos, float lerpDuration)
    {
        // StartLerpingPosition(body.transform, bodySlideOutPos, lerpDuration);
        StartCoroutine(LerpToTransform(body, bodySlideOutPos, lerpDuration));
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

     public void ScaleObj(GameObject obj, float duration, Vector3 targetScale)
    {
        StartCoroutine(Scale(obj, duration, targetScale));
    }

    IEnumerator Scale(GameObject obj, float duration, Vector3 targetScale)
    {
        Vector3 startScale = obj.transform.localScale;
        float t = 0.0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            obj.transform.localScale = Vector3.Lerp(startScale, targetScale, t / duration);
            yield return null;
        }

        // Ensure the scale is set to the final value
        obj.transform.localScale = targetScale;
    }

    public void RotateObj(GameObject obj, float duration, float angle, Vector3 targetScale)
    {
        StartCoroutine(RotateAndScale(obj, duration, angle, targetScale));
    }

    IEnumerator RotateAndScale(GameObject obj, float duration, float angle, Vector3 targetScale)
    {
        float startRotation = obj.transform.eulerAngles.y;
        float endRotation = startRotation + angle;
        Vector3 startScale = obj.transform.localScale;
        float t = 0.0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % angle;
            obj.transform.eulerAngles = new Vector3(obj.transform.eulerAngles.x, yRotation, obj.transform.eulerAngles.z);

            // Lerp scale
            obj.transform.localScale = Vector3.Lerp(startScale, targetScale, t / duration);

            yield return null;
        }

        // Ensure the rotation and scale are set to the final values
        obj.transform.eulerAngles = new Vector3(obj.transform.eulerAngles.x, endRotation % angle, obj.transform.eulerAngles.z);
        obj.transform.localScale = targetScale;
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
