using System;

namespace Client.Entity.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}