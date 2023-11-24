using System.Collections;
using System.Collections.Generic;
using Fineallday.DelegateInterface;
using UnityEngine;

namespace Fineallday
{
    public class Grounded : MonoBehaviour, ICharacterState
    {
        private ICharacterState _characterStateImplementation;

        private Collider col;

        private Rigidbody rb;
        private Vector3 colCenterBottomPoint;
        private float mindistanceToGround = 0.1f;
        
        [SerializeField] private LayerMask canJumpFrom;
        
        void Awake()
        {
            canJumpFrom = (1 << 3);
            col = GetComponent<Collider>();
            rb = GetComponent<Rigidbody>();
          
        }
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(colCenterBottomPoint, mindistanceToGround);
        }


        public void UpdateStateHandler(PlayerController pc)
        {
           
        }

        public bool PingStateHandler(PlayerController pc)
        {
            colCenterBottomPoint = new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z);
            return Physics.CheckSphere(colCenterBottomPoint, mindistanceToGround, canJumpFrom);
        }
    }
}
