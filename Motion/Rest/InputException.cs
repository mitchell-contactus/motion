using System;
namespace Motion.Rest
{
    public class InputException : Exception
    {
        public string Reason { get; }

        public InputException(string missingValue)
        {
            this.Reason = "Missing input value: " + missingValue;
        }
    }
}
