namespace Thalia.Services
{
    public interface ICookiesService<T>
    {
        void Save(string key, T entity);
        T Load(string key);
    }
}
