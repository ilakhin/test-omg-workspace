using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Assets;
using JetBrains.Annotations;
using ObservableCollections;
using R3;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Game.Popups
{
    public sealed class PopupCanvas : IStartable, IDisposable
    {
        private readonly AssetManager _assetManager;
        private readonly PopupManager _popupManager;
        private readonly Transform _parentTransform;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CompositeDisposable _compositeDisposable;
        private readonly Dictionary<PopupViewModel, PopupView> _views;

        [UsedImplicitly]
        public PopupCanvas(AssetManager assetManager, PopupManager popupManager, Transform parentTransform)
        {
            _assetManager = assetManager;
            _popupManager = popupManager;
            _parentTransform = parentTransform;

            _cancellationTokenSource = new CancellationTokenSource();
            _compositeDisposable = new CompositeDisposable();
            _views = new Dictionary<PopupViewModel, PopupView>(32);
        }

        private void CreateView(PopupViewModel viewModel)
        {
            CreateViewAsync(viewModel, _cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid CreateViewAsync(PopupViewModel viewModel, CancellationToken cancellationToken)
        {
            await UniTask.Yield(cancellationToken);

            var viewPrefab = await _assetManager.LoadAssetAsync<GameObject>(viewModel.PopupGuid, _compositeDisposable, cancellationToken);
            var viewGameObject = Object.Instantiate(viewPrefab, _parentTransform);
            var view = viewGameObject.GetComponent<PopupView>();

            view.CreateView(viewModel);

            _views.Add(viewModel, view);
        }

        private void DestroyView(PopupViewModel viewModel)
        {
            if (!_views.Remove(viewModel, out var view))
            {
                return;
            }

            view.DestroyView(viewModel);
        }

        void IStartable.Start()
        {
            foreach (var viewModel in _popupManager.ViewModels)
            {
                CreateView(viewModel);
            }

            _popupManager.ViewModels
                .ObserveAdd(_cancellationTokenSource.Token)
                .Subscribe(this, static (addEvent, projector) => projector.CreateView(addEvent.Value))
                .AddTo(_compositeDisposable);

            _popupManager.ViewModels
                .ObserveRemove(_cancellationTokenSource.Token)
                .Subscribe(this, static (addEvent, projector) => projector.DestroyView(addEvent.Value))
                .AddTo(_compositeDisposable);
        }

        void IDisposable.Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _compositeDisposable.Dispose();

            foreach (var (viewModel, view) in _views)
            {
                view.DestroyView(viewModel);
            }

            _views.Clear();
        }
    }
}
