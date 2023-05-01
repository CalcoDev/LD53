using System;
using Calco.LD53.GameObjects;
using Calco.LD53.Managers;
using Calco.LD53.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace Calco.LD53.UI
{
    public class PlayerUIManager : MonoBehaviour
    {
        [SerializeField] private RawImage _weaponPreviewSlot;
        [SerializeField] private RectTransform[] _milkCartonMasks;

        private void Start()
        {
            Player.Instance.OnAbilityChanged.AddListener(ChangeWeaponPreview);
            MilkManager.Instance.Milk.OnHealthChanged.AddListener(ChangeMilkCartonMask);
        }

        private void ChangeMilkCartonMask(float prev, float curr, float max)
        {
            float percentage = curr / max;

            int maskIndex = Mathf.FloorToInt(percentage * _milkCartonMasks.Length);

            maskIndex = Mathf.Clamp(maskIndex, 0, _milkCartonMasks.Length - 1);
            
            var maxSize = new Vector2(9f, 16f);
            var minSize = new Vector2(9f, 0f);
            
            for (int i = 0; i < maskIndex; i++)
                _milkCartonMasks[i].sizeDelta = maxSize;

            for (int i = maskIndex + 1; i < _milkCartonMasks.Length; i++)
                _milkCartonMasks[i].sizeDelta = minSize;
            
            // Calculate how much one mask means in global percentage
            var amount = 1f / _milkCartonMasks.Length;
            var localPercentage = (percentage - (amount * maskIndex)) / amount;

            const float divisions = 8f;
            localPercentage = Mathf.Ceil(localPercentage * divisions) / divisions;
            
            var size = Vector2.Lerp(minSize, maxSize, localPercentage);
            _milkCartonMasks[maskIndex].sizeDelta = size;
        }

        // TODO(calco): SHOW COOLDOWN
        private void ChangeWeaponPreview(PlayerAbility ability)
        {
            Debug.Log("BBBBBBBBBB");
            
            _weaponPreviewSlot.color = Color.white;
            _weaponPreviewSlot.texture = ability.Icon;
        }
    }
}