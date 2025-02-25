using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLoadingApp.Domain.Enums
{
    public enum LoadStatusEnum
    {
        Created = 1,
        Available = 2,
        Assigned = 3,
        PickedUp = 4,
        Delivered = 5,
        Cancelled = 6,
        Pending = 7
    }
}
