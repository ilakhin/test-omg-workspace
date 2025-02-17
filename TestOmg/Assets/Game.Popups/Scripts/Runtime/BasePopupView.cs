using LitMotion.Animation;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Popups
{
    public abstract class BasePopupView<TViewModel> : PopupView<TViewModel>
        where TViewModel : PopupViewModel
    {
        [SerializeField]
        private LitMotionAnimation _showAnimation;

        [SerializeField]
        private float _showDuration;

        [SerializeField]
        private LitMotionAnimation _hideAnimation;

        [SerializeField]
        private float _hideDuration;

        [SerializeField]
        private Button _closeButton;

        protected override void OnCreateView(TViewModel viewModel)
        {
            _closeButton
                .OnClickAsObservable()
                .Subscribe(this, static (_, view) => view.OnCloseButtonClick())
                .RegisterTo(destroyCancellationToken);
        }

        protected sealed override float OnShow()
        {
            _showAnimation.Play();

            return _showDuration;
        }

        protected sealed override float OnHide()
        {
            _hideAnimation.Play();

            return _hideDuration;
        }

        protected virtual void OnCloseButtonClick()
        {
            RequestClose();
        }
    }
}
