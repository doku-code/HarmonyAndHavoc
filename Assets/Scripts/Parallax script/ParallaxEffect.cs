using UnityEngine;

namespace charles
{
    public class ParallaxEffect : MonoBehaviour
    {
        private float startingPosX;
        private float lengthOfSprite;
        [SerializeField] private float AmountOfParallax;
        [SerializeField] private Camera MainCamera;
        [SerializeField] private bool activateYFollow;

        [SerializeField] private float yOffsetDelay = 0.0f;

        private void Start()
        {
            startingPosX = transform.position.x;
            lengthOfSprite = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        private void Update()
        {
            Vector3 cameraPosition = MainCamera.transform.position;
            float tempX = cameraPosition.x * (1 - AmountOfParallax);
            float distanceX = cameraPosition.x * AmountOfParallax;

            Vector3 newPosition = new Vector3(startingPosX + distanceX, transform.position.y, transform.position.z);

            if (activateYFollow)
            {
                float targetY = cameraPosition.y;
                float currentY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime / yOffsetDelay);
                newPosition.y = currentY;
            }

            transform.position = newPosition;

            if (tempX > startingPosX + (lengthOfSprite / 2))
            {
                startingPosX += lengthOfSprite;
            }
            else if (tempX < startingPosX - (lengthOfSprite / 2))
            {
                startingPosX -= lengthOfSprite;
            }
        }
    }
}
