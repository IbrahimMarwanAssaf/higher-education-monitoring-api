using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace UNIOOP.App.Exceptions
{
    public sealed class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {

        }
    }
}