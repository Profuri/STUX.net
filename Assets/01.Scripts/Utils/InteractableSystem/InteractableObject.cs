using UnityEngine;

namespace InteractableSystem
{
    public abstract class InteractableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private EInteractType _interactType;
        public EInteractType InteractType => _interactType;

        [SerializeField] private EInteractableAttribute _attribute;
        public EInteractableAttribute Attribute => _attribute;

        public abstract void OnInteraction(PlayerController player, bool interactValue);
    }
}