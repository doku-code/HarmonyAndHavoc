using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fineallday.DelegateInterface
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicMovement : MonoBehaviour
    {
        private Rigidbody rb;

        protected ForceMode _forceMode;
        [SerializeField] protected byte groundSpeed; //0 à 255

        protected void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        protected void Move(Vector3 dir)
        {
            rb.AddForce(dir * groundSpeed, _forceMode);
        }
    }
}
