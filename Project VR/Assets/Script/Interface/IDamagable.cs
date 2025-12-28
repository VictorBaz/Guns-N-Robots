namespace Script.Interface
{
    public interface IDamagable
    {
        void TakeDamage();

        bool CanTakeDamage { get; set; }
    }
}