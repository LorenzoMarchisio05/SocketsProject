using System;

namespace Client.Model.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}