using System.Collections;

namespace Fineallday.Coroutines
{
    public interface IItem {
        public string Name {get; }
        public int Damage {get; }
        public int Protection {get; }
    }
}
