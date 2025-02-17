using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using R3;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Game.Assets
{
    public sealed class AssetManager
    {
        [UsedImplicitly]
        public AssetManager()
        {
        }

        public async UniTask<TObject> LoadAssetAsync<TObject>(string assetGuid, ICollection<IDisposable> disposables, CancellationToken cancellationToken)
            where TObject : Object
        {
            var handle = Addressables.LoadAssetAsync<TObject>(assetGuid);
            var asset = await handle.ToUniTask(cancellationToken: cancellationToken, cancelImmediately: true, autoReleaseWhenCanceled: true);
            var disposable = Disposable.Create(handle, static handle => Addressables.Release(handle));

            disposables.Add(disposable);

            return asset;
        }
    }
}
