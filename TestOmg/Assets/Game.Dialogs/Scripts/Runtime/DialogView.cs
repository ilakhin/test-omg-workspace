using Game.Popups;
using LitMotion;
using LitMotion.Extensions;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Game.Dialogs
{
    public sealed class DialogView : BasePopupView<DialogViewModel>
    {
        [SerializeField]
        private LocalizeStringEvent _localizeStringEvent;

        [SerializeField]
        private TMP_Text _text;

        private DialogViewModel _viewModel;
        private CompositeDisposable _textDisposable;

        protected override void OnCreateView(DialogViewModel viewModel)
        {
            base.OnCreateView(viewModel);

            _viewModel = viewModel;
            _textDisposable = new CompositeDisposable();

            _localizeStringEvent.OnUpdateString
                .AsObservable(destroyCancellationToken)
                .Subscribe((Disposable: _textDisposable, Text: _text), static (value, stateTuple) =>
                {
                    // TODO: Анимация на скорую руку.
                    var (disposable, text) = stateTuple;

                    text.text = value;
                    text.ForceMeshUpdate();
                    
                    disposable.Clear();

                    for (var i = 0; i < text.textInfo.characterCount; i++)
                    {
                        var handle = LMotion.Create(Color.white, Color.red, 0.1f)
                            .WithDelay(i * 0.02f)
                            .WithEase(Ease.OutQuad)
                            .BindToTMPCharColor(text, i);
                        var handleDisposable = Disposable.Create(handle, static handle =>
                        {
                            if (handle.IsActive())
                            {
                                handle.Cancel();
                            }
                        });

                        disposable.Add(handleDisposable);
                    }
                })
                .RegisterTo(destroyCancellationToken);

            _viewModel.MessageKey
                .Subscribe(_localizeStringEvent, static (messageKey, localizeStringEvent) =>
                {
                    localizeStringEvent.StringReference.TableEntryReference = messageKey;
                })
                .RegisterTo(destroyCancellationToken);
        }

        protected override void OnDestroyView(DialogViewModel viewModel)
        {
            base.OnDestroyView(viewModel);

            _viewModel = null;
            _textDisposable.Dispose();
            _textDisposable = null;
        }

        protected override void OnCloseButtonClick()
        {
            var changeMessageKeyResult = _viewModel.TryChangeMessageKey();

            if (changeMessageKeyResult)
            {
                return;
            }

            base.OnCloseButtonClick();
        }
    }
}
