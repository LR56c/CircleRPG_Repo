using System;
using UnityConstants;
using UnityEngine;

namespace Code.LevelEssentials
{
    public class DoorZone : MonoBehaviour
    {
        [SerializeField]             private LevelZone _levelZone;
        [SerializeField] private bool         bCanOpen = false;

        private void OnCollisionEnter(Collision other)
        {
            if(!other.gameObject.CompareTag(Tags.Player) && !bCanOpen) return;
            _levelZone.ZoneComplete();
        }

        public void Open()
        {
            bCanOpen = true;
            //open anim
        }
    }
}