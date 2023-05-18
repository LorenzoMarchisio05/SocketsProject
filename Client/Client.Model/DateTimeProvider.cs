using System;
using Client.Model.Interfaces;

namespace Client.Model
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now { get; } = DateTime.Now;
    }
}