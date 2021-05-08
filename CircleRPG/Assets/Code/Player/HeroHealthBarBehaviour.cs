using Code.Player.Heroes;
using DG.Tweening;
using FredericRP.ObjectPooling;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Enemies
{
    public class HeroHealthBarBehaviour : MonoBehaviour
    {
        [SerializeField] private Image _healthBar;
        [SerializeField] private Image _whiteShrinkBar;

        [SerializeField] private MyObjectPoolText _pool;
        [SerializeField] private string           _textPoolName = "Text";
        
        [SerializeField] private float _healthBarFade      = 1.0f;
        [SerializeField] private float _whiteShrinkBarFade = 0.5f;

        [SerializeField] private Color _colorDamage;
        
        private HeroBaseBehaviour _hero;

        private void OnEnable()
        {
            _hero = GetComponent<HeroBaseBehaviour>();
            _hero.OnDamaged += OnDamaged;
        }

        private void OnDisable()
        {
            _hero.OnDamaged -= OnDamaged;
        }
        
        private void OnDamaged(int amount)
        {
            TweenBarFillAmount();
            RequestTextAnim(amount);
        }

        private void RequestTextAnim(int amount)
        {
            var go = _pool.GetFromPool(_textPoolName);
            go.SetText($"-{amount.ToString()}", _colorDamage);
        }

        private void TweenBarFillAmount()
        {
            float percent = (float) _hero.GetCurrentHealth() / _hero.GetMaxHealth();

            _healthBar.DOFillAmount(percent, _healthBarFade).OnComplete(() =>
            {
                _whiteShrinkBar.DOFillAmount(percent, _whiteShrinkBarFade);
            });
        }
    }
}