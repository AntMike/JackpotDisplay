using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Jackpot.Spin
{
    public class StepsScroller : MonoBehaviour
    {
        public bool calculateScroll = false;
        private bool _isTouching = false;
        private bool _wasSpin = false;
        public bool isTouching
        {
            get => _isTouching && !_wasSpin;
            set
            {
                _isTouching = value;
                //SetPosition(parent.childCount - Random.Range(1, 3));
            }
        }


        public Transform parent;
        public ScrollRect scrollRect;
        public AnimationCurve scrollPass;
        public AnimationCurve scrollPassRollback;
        private List<float> _tabsPos = new List<float>();
        public int activeBtn;
        public Scrollbar scrollbar;
        private float _distance;
        private float _scrollPos;
        private float _scrollTargetPos;

        private Coroutine _inertiaCor;
        private bool _inertiaDoing = false;
        private float _inertiaMaxDur = 5;
        private float _inertiaDur = 0;


        private void Start()
        {
            Init();
        }

        public void Init()
        {
            _inertiaDoing = false;
            _inertiaDur = 0;

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

            scrollbar.value =  _tabsPos[1];
        }

        public void StartSpin()
        {
            if (!_wasSpin)
            {
                _wasSpin = true;
                StartCoroutine(Scroll());
            }
        }

        private IEnumerator Scroll()
        {
            float time = .2f;
            float initValue = scrollbar.value;
            float startRollBackValue = initValue - _distance / 5;
            float finishPosition = _tabsPos[parent.childCount - Random.Range(2, 5)];
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
            scrollRect.enabled = false;
            
            yield break;
        }

        private void SetPosition(int num)
        {
            _scrollTargetPos = _scrollPos = _tabsPos[num];
            _inertiaDur = _inertiaMaxDur - .2f;
        }


        private void ClearAllBtns()
        {
            while (parent.childCount > 0)
            {
                DestroyImmediate(parent.GetChild(0).gameObject);
            }
        }


        /*private void Update()
        {
            if (!calculateScroll) return;
            if (_inertiaDoing)
            {
                _inertiaDur += Time.deltaTime;

                if (_inertiaDur > _inertiaMaxDur)
                {
                    _inertiaDoing = false;
                    _inertiaDur = 0;
                }
            }

            Scroll();
        }

        private void Scroll()
        {
            if (isTouching || _inertiaDoing)
            {
                _scrollPos = scrollbar.value;
                if (isTouching)
                {
                    _inertiaDoing = true;
                }

                for (int i = 0; i < posDuration.Count; i++)
                {
                    if (_scrollPos < posDuration[i] + (_distance / 2) &&
                        _scrollPos > posDuration[i] - (_distance / 2))
                    {
                        int k = i;
                        if (activeBtn != k)
                        {
                            activeBtn = k;
                            _scrollTargetPos = posDuration[i];
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < posDuration.Count; i++)
                {
                    if ((_scrollTargetPos < posDuration[i] + (_distance / 2) &&
                         _scrollTargetPos > posDuration[i] - (_distance / 2)) ||
                        (_scrollTargetPos < 0 && i == 0) || (_scrollTargetPos > 1 && i == posDuration.Count - 1))
                    {
                        int k = i;
                        if (k != activeBtn)
                        {
                            activeBtn = k;
                            _scrollTargetPos = posDuration[i];
                        }

                        scrollbar.value = Mathf.Lerp(scrollbar.value, posDuration[i], 0.1f);
                    }
                }
            }
        }*/
    }
}
