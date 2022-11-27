namespace Api.Domain.Interfaces
{
    public interface IHasUser<T>
    {
        public T UserId { get; set; }

    }
}
