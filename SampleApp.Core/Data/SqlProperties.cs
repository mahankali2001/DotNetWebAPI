
namespace SampleApp.Core.Data
{
    public static class SqlProperties
    {
        public const string SP_PARAMETER_RETURN_VALUE_NAME = "@ReturnValue";

        public enum SqlOperation
        {
            Insert,
            Select,
            Update,
            Delete,
            StoredProcedure,
            SqlText
        }

        public enum SqlParameterDirection
        {
            Input,
            Output,
            InputOuput,
            ReturnValue
        }

    }
}
