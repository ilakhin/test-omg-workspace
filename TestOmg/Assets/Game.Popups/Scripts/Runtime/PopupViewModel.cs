using System;
using JetBrains.Annotations;
using R3;

namespace Game.Popups
{
    public class PopupViewModel : IDisposable
    {
        private Subject<PopupViewModel> _requestCloseSubject;

        [UsedImplicitly]
        public PopupViewModel()
        {
        }

        public string PopupGuid
        {
            get;
            set;
        }

        public void SetRequestCloseSubject(Subject<PopupViewModel> requestCloseSubject)
        {
            _requestCloseSubject = requestCloseSubject;
        }

        public void RequestClose()
        {
            _requestCloseSubject?.OnNext(this);
        }

        protected virtual void OnDispose()
        {
        }

        void IDisposable.Dispose()
        {
            OnDispose();
        }
    }
}
