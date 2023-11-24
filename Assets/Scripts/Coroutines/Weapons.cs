namespace Fineallday.Coroutines
{
    public class Weapons : Item, ICauseDamage
    {
        public Weapons(string name, int weight,int damage)
        {
            Name = name;
            Weight = weight;
            Damage = damage;
        }

        public override string Name { get; }
        public override int Weight { get; }
        public int Damage { get; }
    }     
}