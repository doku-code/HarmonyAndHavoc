using System.Collections;
using UnityEngine;

namespace charles
{
    public class LerpFireball : MonoBehaviour
    {
        [SerializeField] private float lerpSpeed = 1.0f;
        [SerializeField] private float targetYOffset = 10.0f;

        private Vector3 startPosition;
        private Vector3 targetPosition;
        private bool isLerping = false;

        void Start()
        {
            startPosition = transform.position;
            targetPosition = transform.position + Vector3.up * targetYOffset;
            StartLerp();
        }

        void Update()
        {
            if (isLerping)
            {
                float t = Mathf.PingPong(Time.time * lerpSpeed, 1.0f);
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            }
        }

        public void StartLerp()
        {
            StartCoroutine(LerpLoop());
        }

        IEnumerator LerpLoop()
        {
            while (true)
            {
                isLerping = true;
                float durationLength = Vector3.Distance(startPosition, targetPosition);
                float startTime = Time.time;

                while (isLerping)
                {
                    float distanceCovered = (Time.time - startTime) * lerpSpeed;
                    float fractionOfDuration = distanceCovered / durationLength;
                    transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfDuration);

                    if (fractionOfDuration >= 1.0f)
                    {
                        isLerping = false;
                        yield return new WaitForSeconds(0.1f);
                    }
                    yield return null;
                }
            }
        }
    }
}
