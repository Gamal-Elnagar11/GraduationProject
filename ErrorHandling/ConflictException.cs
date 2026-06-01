namespace E_Commerce_API.ErrorHandling
{
 
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message)
        {
        }
    }
}
