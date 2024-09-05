namespace TodoApi.Data
{
    public interface IRepositoryInitializer
    {
        /// <summary>
        /// Ensures that the Repository is Created
        /// </summary>
        /// <returns></returns>
        public Task EnsureCreatedAsync();
    }
}
