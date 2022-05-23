using DG.Tweening;
using UnityEngine;

namespace Jackpot.Spin
{
    public class CrystalSlot : Slot
    {
        [SerializeField] private Transform crystal;
        public override void ShowAnimation()
        {
            base.ShowAnimation();
            RotateCrystal();
        }

        private void RotateCrystal()
        {
            crystal.DOLocalRotate(new Vector3(0, 0, -360), 1, RotateMode.LocalAxisAdd).SetEase(Ease.Linear)
                .SetLoops(7, LoopType.Restart); 
        }
    }
}