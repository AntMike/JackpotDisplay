using System.Collections;
using System.Collections.Generic;
using Jackpot.Spin;
using UnityEngine;
using UnityEngine.UI;

namespace Jackpot.UI
{
    public class WinScreen : MonoBehaviour
    {
        
        [SerializeField] private GameObject winPanel;
        [SerializeField] private Image jpImage;
        [SerializeField] private List<Sprite> jpSprites;
        [SerializeField] private SetValue jpSize;
        [SerializeField] private FireworksController fireworksController;

        [Header("Test Fields")] 
        [SerializeField] private GameObject respin;

        public void Init()
        {
            winPanel.SetActive(false);
            respin.SetActive(false);
            fireworksController.Stop();
        }
        
        public void ShowJP(SlotValue jp, Color color)
        {
            respin.SetActive(true);

            fireworksController.SetFireworkColor(color);
            fireworksController.Play();
            
            int sp = jp switch
            {
                SlotValue.JPSilver => 0,
                SlotValue.JPGold => 1,
                SlotValue.JPSuper => 2,
                _ => 3
            };
            if(sp == 3) return;
            jpImage.sprite = jpSprites[sp];
            winPanel.SetActive(true);
            jpSize.duration = 4;
            jpSize.SetValueToText(254958);

        }


    }
}
