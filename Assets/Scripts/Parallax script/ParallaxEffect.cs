using UnityEngine;

namespace charles
{
    public class ParallaxEffect : MonoBehaviour
    {
        private float startingPosX;
        private float startingPosY;
        private float lengthOfSprite;
        [SerializeField] private float AmountOfParallaxX;
        [SerializeField] private float AmountOfParallaxY;
        [SerializeField] private bool FollowY;
        [SerializeField] private float YOffset;
        [SerializeField] private Camera MainCamera;

        private void Start()
        {
            startingPosX = transform.position.x;
            startingPosY = transform.position.y;
            lengthOfSprite = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        private void Update()
        {
            Vector3 position = MainCamera.transform.position;
            float tempX = position.x * (1 - AmountOfParallaxX);
            float distanceX = position.x * AmountOfParallaxX;

            Vector3 newPositionX = new Vector3(startingPosX + distanceX, transform.position.y, transform.position.z);

            transform.position = newPositionX;

            if (tempX > startingPosX + (lengthOfSprite / 2))
            {
                startingPosX += lengthOfSprite;
            }
            else if (tempX < startingPosX - (lengthOfSprite / 2))
            {
                startingPosX -= lengthOfSprite;
            }

            if (FollowY)
            {
                float tempY = position.y * (1 - AmountOfParallaxY) + YOffset;
                float distanceY = position.y * AmountOfParallaxY;

                Vector3 newPositionY = new Vector3(transform.position.x, startingPosY + distanceY, transform.position.z);

                transform.position = newPositionY;
            }
        }
    }
}
