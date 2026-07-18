using UnityEngine;
using UnityEngine.EventSystems;
using FarmFarmer.Data;

namespace FarmFarmer.UI
{
    // Routes through UGUI's EventSystem/Graphic raycasting rather than raw Input polling -- this
    // project's Active Input Handling is New Input System only (legacy Input.* silently no-ops),
    // and this approach works identically for touch (Android) and mouse (Editor) either way.
    public class EnemyTapInput : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private CombatController combatController;

        public void OnPointerClick(PointerEventData eventData)
        {
            combatController.OnEnemyTapped();
        }
    }
}
