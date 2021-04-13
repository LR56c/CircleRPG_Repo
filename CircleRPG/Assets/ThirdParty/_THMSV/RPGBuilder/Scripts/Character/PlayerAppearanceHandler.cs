using THMSV.RPGBuilder.Managers;
using UnityEngine;

namespace THMSV.RPGBuilder.Character
{
    public class PlayerAppearanceHandler : MonoBehaviour
    {
        public GameObject[] armorPieces;
        public Transform OneHandWeapon1CombatSlot, OneHandWeapon1RestSlot, OneHandWeapon2CombatSlot, OneHandWeapon2RestSlot, TwoHandWeaponCombatSlot, TwoHandWeaponRestSlot;
        public GameObject weapon1GO, weapon2GO;
        public GameObject chestBody, LegsBody, HandsBody, FeetBody;
        public Transform HeadTransformSocket;
        public Transform ProjectilePoint;
        private RPGItem weapon1RPGItem, weapon2RPGItem;
        public void ShowArmor(string armorPieceName)
        {
            foreach (var t in armorPieces)
                if (t.name == armorPieceName) t.SetActive(true);
        }

        public void HideArmor(string armorPieceName)
        {
            foreach (var t in armorPieces)
                if (t.name == armorPieceName) t.SetActive(false);
        }

        public void HideWeapon(int weaponID)
        {
            switch (weaponID)
            {
                case 1:
                {
                    if (weapon1GO != null)
                    {
                        Destroy(weapon1GO);
                        weapon1RPGItem = null;
                    }

                    break;
                }
                case 2:
                {
                    if (weapon2GO != null)
                    {
                        Destroy(weapon2GO);
                        weapon2RPGItem = null;
                    }

                    break;
                }
            }
        }

        public void UpdateWeaponStates(bool inCombat)
        {
            if (weapon1GO != null)
            {
                if (inCombat)
                {
                    weapon1GO.transform.SetParent(weapon1RPGItem.slotType == "TWO HAND"
                        ? TwoHandWeaponCombatSlot
                        : OneHandWeapon1CombatSlot);
                }
                else
                {
                    if (weapon1RPGItem.weaponType == "SHIELD")
                    {
                        weapon1GO.transform.SetParent(TwoHandWeaponRestSlot);
                    }
                    else
                    {
                        weapon1GO.transform.SetParent(weapon1RPGItem.slotType == "TWO HAND"
                            ? TwoHandWeaponRestSlot
                            : OneHandWeapon1RestSlot);
                    }
                }
                SetWeaponPosition(weapon1GO, weapon1RPGItem, inCombat);
            }
            if (weapon2GO != null)
            {
                if (weapon2RPGItem.weaponType == "SHIELD")
                {
                    weapon2GO.transform.SetParent(inCombat ? OneHandWeapon2CombatSlot : TwoHandWeaponRestSlot);
                }
                else
                {
                    weapon2GO.transform.SetParent(inCombat ? OneHandWeapon2CombatSlot : OneHandWeapon2RestSlot);
                }

                SetWeaponPosition(weapon2GO, weapon2RPGItem, inCombat);
                
            }
        }

        void SetWeaponPosition(GameObject go, RPGItem weaponItem, bool inCombat)
        {
            Vector3[] weaponPositionData = getWeaponPositionData(weaponItem, inCombat);
            go.transform.localPosition = weaponPositionData[0];
            go.transform.localRotation = Quaternion.Euler(weaponPositionData[1]);
            go.transform.localScale = weaponPositionData[2];
        }

        Vector3[] getWeaponPositionData(RPGItem weaponItem, bool inCombat)
        {
            Vector3[] weaponPositionData = new Vector3[3];
            weaponPositionData[2] = Vector3.one;

            foreach (var t in weaponItem.weaponPositionDatas)
            {
                if (t.raceID != CharacterData.Instance.raceID) continue;
                foreach (var t1 in t.genderPositionDatas)
                {
                    if (t1.gender != CharacterData.Instance.gender) continue;
                    if (inCombat)
                    {
                        weaponPositionData[0] = t1.CombatPositionInSlot;
                        weaponPositionData[1] = t1.CombatRotationInSlot;
                        weaponPositionData[2] = t1.CombatScaleInSlot;
                    }
                    else
                    {
                        weaponPositionData[0] = t1.RestPositionInSlot;
                        weaponPositionData[1] = t1.RestRotationInSlot;
                        weaponPositionData[2] = t1.RestScaleInSlot;
                    }
                }
            }

            return weaponPositionData;
        }

        private void ParentWeaponToSlot(GameObject weaponGO, int weaponID, string slotType, RPGItem weaponItem)
        {
            if (CombatManager.playerCombatInfo == null)
            {
                if (weaponID == 1)
                {
                    if (weaponItem.weaponType == "SHIELD")
                    {
                        weaponGO.transform.SetParent(TwoHandWeaponRestSlot);
                    }
                    else
                    {
                        weaponGO.transform.SetParent(slotType == "TWO HAND"
                            ? TwoHandWeaponRestSlot
                            : OneHandWeapon1RestSlot);
                    }

                    SetWeaponPosition(weaponGO, weaponItem, false);
                }
                else
                {
                    weaponGO.transform.SetParent(weaponItem.weaponType == "SHIELD"
                        ? TwoHandWeaponRestSlot
                        : OneHandWeapon2RestSlot);

                    SetWeaponPosition(weaponGO, weaponItem, false);
                }
            }
            else
            {
                if (CombatManager.playerCombatInfo.inCombat)
                {
                    if (weaponID == 1)
                    {
                        if (weaponItem.weaponType == "SHIELD")
                        {
                            weaponGO.transform.SetParent(TwoHandWeaponRestSlot);
                        }
                        else
                        {
                            weaponGO.transform.SetParent(slotType == "TWO HAND"
                                ? TwoHandWeaponCombatSlot
                                : OneHandWeapon1CombatSlot);
                        }

                        SetWeaponPosition(weaponGO, weaponItem, true);
                    }
                    else
                    {
                        weaponGO.transform.SetParent(OneHandWeapon2CombatSlot);

                        SetWeaponPosition(weaponGO, weaponItem, true);
                    }
                }
                else
                {
                    if (weaponID == 1)
                    {
                        if (weaponItem.weaponType == "SHIELD")
                        {
                            weaponGO.transform.SetParent(TwoHandWeaponRestSlot);
                        }
                        else
                        {
                            weaponGO.transform.SetParent(slotType == "TWO HAND"
                                ? TwoHandWeaponRestSlot
                                : OneHandWeapon1RestSlot);
                        }

                        SetWeaponPosition(weaponGO, weaponItem, false);
                    }
                    else
                    {
                        weaponGO.transform.SetParent(weaponItem.weaponType == "SHIELD"
                            ? TwoHandWeaponRestSlot
                            : OneHandWeapon2RestSlot);

                        SetWeaponPosition(weaponGO, weaponItem, false);
                    }
                }
            }
        }

        public void ShowWeapon(RPGItem weaponItem, int weaponID)
        {
            switch (weaponID)
            {
                case 1:
                {
                    var newWeaponGO = Instantiate(weaponItem.weaponModel, transform.position, Quaternion.identity);
                    ParentWeaponToSlot(newWeaponGO, weaponID, weaponItem.slotType, weaponItem);
                    if (weapon1GO != null) Destroy(weapon1GO);

                    weapon1GO = newWeaponGO;
                    weapon1RPGItem = weaponItem;
                    break;
                }
                case 2:
                {
                    var newWeaponGO = Instantiate(weaponItem.weaponModel, transform.position, Quaternion.identity);
                    ParentWeaponToSlot(newWeaponGO, weaponID, weaponItem.slotType, weaponItem);
                    if (weapon2GO != null) Destroy(weapon2GO);

                    weapon2GO = newWeaponGO;
                    weapon2RPGItem = weaponItem;
                    break;
                }
            }
        }

        public void HideWeapon(RPGItem weaponItem, int weaponID)
        {
            switch (weaponID)
            {
                case 1:
                {
                    if (weapon1GO != null)
                    {
                        Destroy(weapon1GO);
                        weapon1RPGItem = null;
                    }
                    break;
                }
                case 2:
                {
                    if (weapon2GO != null)
                    {
                        Destroy(weapon2GO);
                        weapon2RPGItem = null;
                    }
                    break;
                }
            }
        }

    }
}
