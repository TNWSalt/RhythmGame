using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] bool autoReturn;
        public string objectPoolName;
        public float timeReturn;

        [SerializeField] protected float timeLeft;

        public virtual void OnEnable()
        {
            timeLeft = timeReturn;
        }

        public virtual void Update()
        {
            if (autoReturn) 
            {
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0)
                {
                    ReturnToPool();
                }
            }            
        }

        public virtual void ReturnToPool()
        {            
            ObjectPoolManager.GetInstance().ReturnToPool(objectPoolName, gameObject);
        }
    }


