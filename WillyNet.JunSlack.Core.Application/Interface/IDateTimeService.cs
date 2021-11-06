using System;
using System.Collections.Generic;
using System.Text;

namespace WillyNet.JunSlack.Core.Application.Interfaces
{
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
    }
}
