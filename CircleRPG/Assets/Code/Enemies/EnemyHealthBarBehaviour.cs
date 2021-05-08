using System;
using Code.Domain.Interfaces;
using Code.Enemies.Types;
using DG.Tweening;
using FredericRP.ObjectPooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Enemies
{
    public class EnemyHealthBarBehaviour : MonoBehaviour
    {
        [SerializeField]             private Image _healthBar;
        [SerializeField]             private Image _whiteShrinkBar;

        [SerializeField] private MyObjectPoolText _pool;
        [SerializeField] private string           _textPoolName = "Text";
        
        [SerializeField] private float     _healthBarFade = 1.0f;
        [SerializeField] private float     _whiteShrinkBarFade = 0.5f;
        
        private EnemyBaseBehaviour _enemy;
        
        private void OnEnable()
        {
            _enemy = GetComponent<EnemyBaseBehaviour>();
            _enemy.OnDamaged += OnDamaged;
        }

        private void OnDisable()
        {
            _enemy.OnDamaged -= OnDamaged;
        }
        
        private void OnDamaged(int amount, Color heroIndex)
        {
            TweenBarFillAmount();
            RequestTextAnim(amount, heroIndex);
        }

        private void RequestTextAnim(int amount, Color heroIndex)
        {
            var go = _pool.GetFromPool(_textPoolName);
            go.SetText($"-{amount.ToString()}", heroIndex);
        }

        private void TweenBarFillAmount()
        {
            float percent = (float) _enemy.GetCurrentHealth() / _enemy.GetMaxHealth();

            _healthBar.DOFillAmount(percent, _healthBarFade).OnComplete(() =>
            {
                _whiteShrinkBar.DOFillAmount(percent, _whiteShrinkBarFade);
            });
        }
    }
}