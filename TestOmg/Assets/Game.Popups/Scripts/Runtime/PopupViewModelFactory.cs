using System;
using JetBrains.Annotations;
using VContainer;

namespace Game.Popups
{
    public sealed class PopupViewModelFactory
    {
        private readonly IObjectResolver _objectResolver;

        [UsedImplicitly]
        public PopupViewModelFactory(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public PopupViewModel Create(Type viewModelType)
        {
            var viewModel = (PopupViewModel)_objectResolver.Resolve(viewModelType);

            return viewModel;
        }
    }
}
