using THMSV.RPGBuilder.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace THMSV.RPGBuilder.UIElements
{
    public class GetItemSlot : MonoBehaviour
    {
        public RPGItem thisitem;
        public Image icon;


        public void GetItem()
        {
            DevUIManager.Instance.GetItem(thisitem);
        }
    }
}