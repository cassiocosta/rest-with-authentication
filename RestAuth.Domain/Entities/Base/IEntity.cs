namespace RestAuth.Domain.Entities
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }

        DateTime CreationDate { get; set; }

        DateTime? LastChangeDate { get; set; }

        DateTime? DeletionDate { get; set; }
    }
}