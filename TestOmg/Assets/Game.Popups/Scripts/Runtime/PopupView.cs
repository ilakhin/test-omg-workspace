using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Popups
{
    [DisallowMultipleComponent]
    public class PopupView : MonoBehaviour
    {
        private PopupViewModel _viewModel;

        public void CreateView(PopupViewModel viewModel)
        {
            _viewModel = viewModel;

            OnCreateView(viewModel);
            OnShow();
        }

        protected virtual void OnCreateView(PopupViewModel viewModel)
        {
        }

        public void DestroyView(PopupViewModel viewModel)
        {
            var hideDuration = OnHide();

            if (hideDuration > 0f)
            {
                DestroyViewAsync(hideDuration, viewModel, destroyCancellationToken).Forget();

                return;
            }

            OnDestroyView(viewModel);

            _viewModel = null;
        }

        private async UniTaskVoid DestroyViewAsync(float delay, PopupViewModel viewModel, CancellationToken cancellationToken)
        {
            await UniTask.WaitForSeconds(delay, cancellationToken: cancellationToken);

            OnDestroyView(viewModel);

            _viewModel = null;
        }

        protected virtual void OnDestroyView(PopupViewModel viewModel)
        {
            if (this == null)
            {
                return;
            }

            Destroy(gameObject);
        }

        protected virtual float OnShow()
        {
            return 0f;
        }

        protected virtual float OnHide()
        {
            return 0f;
        }

        protected void RequestClose()
        {
            _viewModel.RequestClose();
        }
    }

    public abstract class PopupView<TViewModel> : PopupView
        where TViewModel : PopupViewModel
    {
        protected sealed override void OnCreateView(PopupViewModel viewModel)
        {
            base.OnCreateView(viewModel);

            OnCreateView((TViewModel)viewModel);
        }

        protected virtual void OnCreateView(TViewModel viewModel)
        {
        }

        protected sealed override void OnDestroyView(PopupViewModel viewModel)
        {
            OnDestroyView((TViewModel)viewModel);

            base.OnDestroyView(viewModel);
        }

        protected virtual void OnDestroyView(TViewModel viewModel)
        {
        }
    }
}
