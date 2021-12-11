namespace R.Systems.Auth.SharedKernel.Validation
{
    public class OperationResult<T>
    {
        public bool Result { get; set; }

        public T? Data { get; set; }
    }
}
