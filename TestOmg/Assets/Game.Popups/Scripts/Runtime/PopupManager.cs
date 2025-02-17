using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableCollections;
using R3;
using VContainer.Unity;

namespace Game.Popups
{
    public sealed class PopupManager : IStartable, IDisposable
    {
        private readonly PopupViewModelFactory _viewModelFactory;

        private readonly ObservableList<PopupViewModel> _viewModels;
        private readonly Subject<PopupViewModel> _requestCloseSubject;

        [UsedImplicitly]
        public PopupManager(PopupViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;

            _viewModels = new ObservableList<PopupViewModel>();
            _requestCloseSubject = new Subject<PopupViewModel>();
        }

        public IReadOnlyObservableList<PopupViewModel> ViewModels => _viewModels;

        public PopupViewModel CreatePopup(Type viewModelType, string popupGuid, out IDisposable disposable)
        {
            var viewModel = _viewModelFactory.Create(viewModelType);

            viewModel.PopupGuid = popupGuid;
            viewModel.SetRequestCloseSubject(_requestCloseSubject);

            disposable = Disposable.Create((PopupManager: this, ViewModel: viewModel), static tuple =>
            {
                var (popupManager, viewModel) = tuple;

                popupManager.DestroyPopup(viewModel);
            });

            _viewModels.Add(viewModel);

            return viewModel;
        }

        public PopupViewModel CreatePopup(Type viewModelType, string popupGuid, ICollection<IDisposable> disposables)
        {
            var viewModel = CreatePopup(viewModelType, popupGuid, out var disposable);

            disposables.Add(disposable);

            return viewModel;
        }

        public void DestroyPopup(PopupViewModel viewModel)
        {
            _viewModels.Remove(viewModel);
        }

        void IStartable.Start()
        {
            _requestCloseSubject.Subscribe(this, static (viewModel, popupManager) => popupManager.DestroyPopup(viewModel));
        }

        void IDisposable.Dispose()
        {
            _viewModels.Clear();
            _requestCloseSubject.Dispose();
        }
    }
}
