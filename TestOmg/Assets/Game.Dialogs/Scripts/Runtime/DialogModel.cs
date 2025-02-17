using System.Collections.Generic;
using System.Linq;

namespace Game.Dialogs
{
    public sealed class DialogModel
    {
        public readonly IReadOnlyList<string> MessageKeys;

        public DialogModel(IEnumerable<string> messageKeys)
        {
            MessageKeys = messageKeys.ToArray();
        }
    }
}
