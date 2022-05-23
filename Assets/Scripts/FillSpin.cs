using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Jackpot.Spin
{
    public class FillSpin : MonoBehaviour
    {
        [SerializeField] private StepsScroller scroller;
        [Header("Prefabs")] 
        [SerializeField] private List<GameObject> prefabs;


        private void Start()
        {
            Init();
        }

        public void Init()
        {
            scroller.ClearAll();

            for (int i = 0; i < 50; i++)
            {
                scroller.SpawnItem(SelectPrefab());
            }
            scroller.Init();
        }

        private GameObject SelectPrefab()
        {
            return prefabs[Random.Range(0, prefabs.Count - 1)];
        }


    }
}
