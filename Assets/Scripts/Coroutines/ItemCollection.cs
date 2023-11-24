using System.Collections;

namespace Fineallday.Coroutines
{
    public class ItemCollection : IEnumerator
    {
        private  object[] collection;
        private int index = -1;
        public ItemCollection(object[] inventoryItems)
        {
            
            collection = inventoryItems;
        }

        public object Current => collection[index];
        
        public bool MoveNext()
        {
            index++;
            return index < collection.Length;
        }

        public void Reset()
        {
            index = -1;
        }
    }
}
