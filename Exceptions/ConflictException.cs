using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace UNIOOP.App.Exceptions
{
    public sealed class ConflictException : Exception
    {
        public ConflictException(string message) : base(message)
        {
        }
    }
}
