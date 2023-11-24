using Fineallday.Coroutines;
using UnityEngine;

namespace Fineallday
{
    public class Shields : Item, IGiveProtection
    {
        public Shields(string name, int weight,int protection)
        {
            Name = name;
            Weight = weight;
            Protection = protection;
        }

        public override string Name { get; }    
        public override int Weight { get; }
        public int Protection { get; }
    }
}
