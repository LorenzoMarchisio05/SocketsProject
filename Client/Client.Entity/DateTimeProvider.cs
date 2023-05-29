using System;
using Client.Entity.Interfaces;

namespace Client.Entity
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now { get; } = DateTime.Now;
    }
}