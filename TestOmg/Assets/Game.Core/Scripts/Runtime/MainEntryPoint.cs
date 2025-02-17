using System;
using Cysharp.Threading.Tasks;
using Game.Dialogs;
using Game.Popups;
using JetBrains.Annotations;
using R3;
using VContainer.Unity;

namespace Game.Core
{
    public sealed class MainEntryPoint : IStartable, IDisposable
    {
        private readonly PopupManager _popupManager;

        private CompositeDisposable _compositeDisposable;

        [UsedImplicitly]
        public MainEntryPoint(PopupManager popupManager)
        {
            _popupManager = popupManager;
        }

        private async UniTaskVoid StartAsync()
        {
            await UniTask.WaitForSeconds(1f);

            // TODO: Для примера
            var model = new DialogModel(new[]
            {
                "message_a",
                "message_b",
                "message_c",
                "message_d",
                "message_e",
                "message_f"
            });
            var viewModel = (DialogViewModel)_popupManager.CreatePopup(typeof(DialogViewModel), "ad3c713af81df448ca6469629be5054a", _compositeDisposable);

            viewModel.SetModel(model);
        }

        void IStartable.Start()
        {
            _compositeDisposable = new CompositeDisposable();

            StartAsync().Forget();
        }

        void IDisposable.Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}
