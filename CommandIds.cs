using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CensionExtension
{
    internal class CommandIds
    {
        public static readonly Guid CommandSetGuid = new Guid("65c90eaa-d973-4188-9445-a42a5b060a0c");

        public const int CensionCommandId = 0x0100;
        public const int CensionContextAwarenessCommandId = 0x0101;
    }
}
