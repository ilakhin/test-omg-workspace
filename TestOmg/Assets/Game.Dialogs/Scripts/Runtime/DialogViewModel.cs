using Game.Popups;
using JetBrains.Annotations;
using R3;

namespace Game.Dialogs
{
    public sealed class DialogViewModel : PopupViewModel
    {
        private readonly ReactiveProperty<string> _messageKey;

        private DialogModel _model;
        private int _messageKeyIndex;

        [UsedImplicitly]
        public DialogViewModel()
        {
            _messageKey = new ReactiveProperty<string>();
        }

        public ReadOnlyReactiveProperty<string> MessageKey => _messageKey;

        protected override void OnDispose()
        {
            base.OnDispose();

            _messageKey.Dispose();
        }

        public void SetModel(DialogModel model)
        {
            _model = model;

            if (model.MessageKeys.Count > 0)
            {
                _messageKey.Value = model.MessageKeys[0];
            }
        }

        public bool TryChangeMessageKey()
        {
            var messageKeys = _model.MessageKeys;

            if (_messageKeyIndex >= messageKeys.Count - 1)
            {
                return false;
            }

            _messageKeyIndex++;
            _messageKey.Value = messageKeys[_messageKeyIndex];

            return true;
        }
    }
}
