using System.ComponentModel.DataAnnotations;
using Game.Assets;
using Game.Dialogs;
using Game.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Core
{
    [DisallowMultipleComponent]
    public sealed class MainLifetimeScope : LifetimeScope
    {
        [Required]
        [SerializeField]
        private Transform _popupParentTransform;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterEntryPoint<MainEntryPoint>();

            builder.Register<AssetManager>(Lifetime.Singleton);
            builder.Register<PopupManager>(Lifetime.Singleton)
                .AsSelf()
                .AsImplementedInterfaces();
            builder.Register<PopupCanvas>(Lifetime.Singleton)
                .AsSelf()
                .AsImplementedInterfaces()
                .WithParameter("parentTransform", _popupParentTransform);
            builder.Register<PopupViewModelFactory>(Lifetime.Singleton);

            builder.Register<DialogViewModel>(Lifetime.Transient);
        }
    }
}
