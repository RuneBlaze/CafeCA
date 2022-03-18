namespace Cafeo.Templates
{
    public interface ITemplate<out T>
    {
        public T Create();
    }
}