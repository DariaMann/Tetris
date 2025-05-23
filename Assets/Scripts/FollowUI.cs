using UnityEngine;

[ExecuteAlways]
public class FollowUI : MonoBehaviour
{
    [SerializeField] private RectTransform target;

    void LateUpdate()
    {
        if (target == null)
            return;

        transform.position = target.position;
    }
}