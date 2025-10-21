using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNWSalt
{
    public interface IObjectPool
    {
        public void ReturnToPool();
    }
}