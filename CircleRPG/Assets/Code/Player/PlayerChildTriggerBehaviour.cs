using UnityEngine;

namespace Code.Player
{
    public class PlayerChildTriggerBehaviour : MonoBehaviour
    {
        [SerializeField] private PlayerGroupBehaviour _playerGroup;

        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag(UnityConstants.Tags.Enemy)) return;
            _playerGroup.AddCollider(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if(!other.CompareTag(UnityConstants.Tags.Enemy)) return;
            _playerGroup.RemoveCollider(other);
        }
    }
}