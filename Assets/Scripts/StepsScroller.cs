using System;
using System.Collections;
using System.Collections.Generic;
using Jackpot.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Jackpot.Spin
{
    public class StepsScroller : MonoBehaviour
    {
        private bool _wasSpin = false;
        [SerializeField] private WinScreen winScreen;
        [SerializeField] private Transform parent;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Scrollbar scrollbar;
        
        [SerializeField] private AnimationCurve scrollPass;
        [SerializeField] private AnimationCurve scrollPassRollback;

        [Header("Sound")] 
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip spinClip;
        
        private List<float> _tabsPos = new List<float>();
        private List<GameObject> _spawnedItems = new List<GameObject>();
        private int activeBtn;
        private float _distance;

        public void Init()
        {
            winScreen.Init();
            _wasSpin = false;
            activeBtn = -1;

            _tabsPos = new List<float>(new float[parent.childCount]);


            if (_tabsPos.Count != 1)
                _distance = 1f / (_tabsPos.Count - 1f);
            else
                _distance = 0f;
            for (int i = 0; i < _tabsPos.Count; i++)
            {
                _tabsPos[i] = _distance * i;
            }

            //scrollbar.value = _tabsPos[2];
            StartCoroutine(ScrollbarLateUpdate(_tabsPos[2]));
        }

        private IEnumerator ScrollbarLateUpdate(float pos)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            scrollbar.value = pos;
        }

        public void SpawnItem(GameObject item)
        {
            GameObject go = Instantiate(item, parent);
            _spawnedItems.Add(go);
        }

        public void StartSpin()
        {
            if (_wasSpin) return;
            
            audioSource.PlayOneShot(spinClip, .8f);
            _wasSpin = true;
            StartCoroutine(Scroll());
        }

        private IEnumerator Scroll()
        {
            float time = .2f;
            float initValue = scrollbar.value;
            float startRollBackValue = initValue - _distance / 5;
            activeBtn = Random.Range((int) (parent.childCount * .75f), (int) (parent.childCount - 1));
            float finishPosition = _tabsPos[activeBtn];
            while (time>=0)
            {
                time -= Time.deltaTime;
                scrollbar.value = Mathf.Lerp(scrollbar.value, startRollBackValue, 0.1f);
                yield return new WaitForEndOfFrame();
            }
            initValue = scrollbar.value;
            time = 5;
            var curCurve = Random.Range(0f, 1f) > .5f ? scrollPassRollback : scrollPass;
            while (time>=0)
            {
                time -= Time.deltaTime;
                scrollbar.value = Mathf.LerpUnclamped(initValue, finishPosition, curCurve.Evaluate((5-time)/5));
                yield return new WaitForEndOfFrame();
            }

            scrollbar.value = finishPosition;
            yield return new WaitForEndOfFrame();
            //scrollRect.enabled = false;
            SlotSelected();
        }

        private void SlotSelected()
        {
            var slot = _spawnedItems[activeBtn].GetComponent<Slot>();
            if (slot is SimpleSlot simpleSlot)
            {
                simpleSlot.ShowAnimation();
            }
            else if(slot is CrystalSlot crystalSlot)
            {
                crystalSlot.ShowAnimation();
            }
            else
            {
                slot.ShowAnimation();
            }

            winScreen.ShowJP(slot.value,slot.color);
        }


        public void ClearAll()
        {
            while (parent.childCount > 0)
            {
                DestroyImmediate(parent.GetChild(0).gameObject);
            }
            _spawnedItems.Clear();
        }
    }
}
