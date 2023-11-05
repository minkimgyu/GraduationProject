using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void OnSightEnter();

    void OnInteract();

    void OnSightExit();
}

public class InteractionComponent : MonoBehaviour
{
    [SerializeField] private Transform _camTransform;
    [SerializeField] private float _range = 10;

    IInteractable _interactableTarget;
    public IInteractable InteractableTarget { get { return _interactableTarget; } }

    public bool CanInteract()
    {
        RaycastHit hit;
        Physics.Raycast(_camTransform.position, _camTransform.forward, out hit, _range);
        if (hit.collider == null) return false;

        //Debug.DrawRay(_camTransform.position, _camTransform.forward * _range, Color.green, 10); // 디버그 레이를 카메라가 바라보고 있는 방향으로 발사한다.

        IInteractable target = hit.collider.GetComponent<IInteractable>();
        if (target == null) return false;

        _interactableTarget = target;

        return true;
    }
}
