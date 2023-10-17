using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace charles
{
    public class ParallaxEffect : MonoBehaviour
    {
        private float startingPos;
        private float lengthOfSprite;
        [SerializeField] private float AmountOfParallax;
        [SerializeField] private Camera MainCamera;

        private void Start()
        {
            startingPos = transform.position.x;
            lengthOfSprite = GetComponent<SpriteRenderer>().bounds.size.x;
        }
        private void Update()
        {
            Vector3 Position = MainCamera.transform.position;
            float Temp = Position.x * (1 - AmountOfParallax);
            float Distance = Position.x * AmountOfParallax;

            Vector3 NewPosition = new Vector3(startingPos + Distance, transform.position.y, transform.position.z);

            transform.position = NewPosition;

            if (Temp > startingPos + (lengthOfSprite / 2))
            {
                startingPos += lengthOfSprite;
            }
            else if (Temp < startingPos - (lengthOfSprite / 2))
            {
                startingPos -= lengthOfSprite;
            }
        }
    }
}
