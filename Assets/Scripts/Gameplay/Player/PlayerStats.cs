using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DeadPixel
{
    public class PlayerStats : MonoBehaviour
    {
        public int startHp;
        public int maxHp;

        public int startMemory;
        public int maxMemory;

        public int startShield;
        public int maxShield;

        public Stat HP;
        public Stat Memory;
        public Stat Shield;

        void Awake()
        {
            HP = new Stat(startHp, maxHp);
            Memory = new Stat(startMemory, maxMemory);
            Shield = new Stat(startShield, maxShield);
        }

    }

    public class Stat
    {
        public int start;
        public int value;
        public int max;
        //public int limit;

        public Stat(int start, int max)
        {
            this.start = start;
            this.max = max;
            value = start;
        }

        public void Change(int addValue) => value = Mathf.Clamp(value + addValue, 0, max);

        public void IncreaseMax(int addValue) => max += addValue;
        public void IncreaseMax() => IncreaseMax(1);
    }

}
