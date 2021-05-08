using System;
using Code.Player;
using Code.Player.Heroes;
using Code.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

namespace Code.UI
{
    public enum EAbilityType : int
    {
        Archer = 0,
        Hammer = 1,
        Shield = 2,
    }
    
    public class UIHeroAbility : MonoBehaviour
    {
        [SerializeField] private Button _archerAbilityButton;
        [SerializeField] private Button _hammerAbilityButton;
        [SerializeField] private Button _shieldAbilityButton;

        [SerializeField] private Button[] _type;
        
        [Header("External")]
        [SerializeField] private HeroAbilityBase _archerAbility;
        [SerializeField] private HeroAbilityBase _hammerAbility;
        [SerializeField] private HeroAbilityBase _shieldAbility;

        //Se coloca fijo, pero si serian mas, se enlazaria button con diccionario
        private void OnValidate()
        {
            _type = new[] {_archerAbilityButton, 
                            _hammerAbilityButton, 
                            _shieldAbilityButton};
        }
        
        private void OnEnable()
        {
            _archerAbilityButton.onClick.AddListener(ArcherAbilityResponse);
            _shieldAbilityButton.onClick.AddListener(ShieldAbilityResponse);
            _hammerAbilityButton.onClick.AddListener(HammerAbilityResponse);
        }
        
        private void OnDisable()
        {
            _archerAbilityButton.onClick.RemoveListener(ArcherAbilityResponse);
            _shieldAbilityButton.onClick.RemoveListener(ShieldAbilityResponse);
            _hammerAbilityButton.onClick.RemoveListener(HammerAbilityResponse);
        }

        private void HammerAbilityResponse()
        {
            _hammerAbility.Ability();
            _type[(int)EAbilityType.Hammer].gameObject.SetActive(false);
        }

        private void ShieldAbilityResponse()
        {
            _shieldAbility.Ability();
            _type[(int)EAbilityType.Shield].gameObject.SetActive(false);
        }

        private void ArcherAbilityResponse()
        {
            _archerAbility.Ability();
            _type[(int)EAbilityType.Archer].gameObject.SetActive(false);

        }
        
        public void ActiveHeroAbility(int eType)
        {
            _type[eType].gameObject.SetActive(true);
        }
    }
}