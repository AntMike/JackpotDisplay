using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Jackpot.Spin
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] protected Transform title;
        [SerializeField] protected Material blinkMaterial;
        public SlotValue value;
        
        public virtual void ShowAnimation()
        {
            EnlargeTitle();
            StartCoroutine(BlinkTitle());
        }

        protected IEnumerator BlinkTitle()
        {
            title.GetComponent<Image>().material = blinkMaterial;

            yield return new WaitForSeconds(7);
            title.GetComponent<Image>().material = null;
        }

        protected void EnlargeTitle()
        {
            title.DOScale(1.2f, .5f).SetLoops(14, LoopType.Yoyo);
        }
    }


    public enum SlotValue
    {
        x12=0,
        x25=1,
        x50=2,
        x100=3,
        x1000=4,
        Bonus=5,
        FreeSpeens=6, 
        JPSilver=7, 
        JPGold=8, 
        JPSuper=9
    }
}
