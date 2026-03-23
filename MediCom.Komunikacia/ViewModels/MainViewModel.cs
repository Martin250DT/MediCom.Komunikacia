using MediCom.Komunikacia.Models;
using MediCom.Komunikacia.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MediCom.Komunikacia.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly OpenAiChatService _chatService = new OpenAiChatService();
        private bool _isBusy;
        private string _messageText;
        private string _statusText;

        public MainViewModel()
        {
            Messages = new ObservableCollection<ChatMessage>();
            SendMessageCommand = new AsyncRelayCommand(SendMessageAsync, CanSendMessage);

            StatusText = "Pripravené na chat.";
            Messages.Add(new ChatMessage
            {
                Sender = "ChatGPT",
                Text = "Ahoj 👋 Napíš správu a ja odpoviem ako v jednoduchej WhatsApp-style aplikácii.",
                Timestamp = DateTime.Now,
                IsUser = false
            });
        }

        public ObservableCollection<ChatMessage> Messages { get; }

        public ICommand SendMessageCommand { get; }

        public string MessageText
        {
            get => _messageText;
            set
            {
                if (_messageText == value)
                {
                    return;
                }

                _messageText = value;
                OnPropertyChanged(nameof(MessageText));
                (SendMessageCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string StatusText
        {
            get => _statusText;
            set
            {
                if (_statusText == value)
                {
                    return;
                }

                _statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool CanSendMessage()
        {
            return !_isBusy && !string.IsNullOrWhiteSpace(MessageText);
        }

        private async Task SendMessageAsync()
        {
            var text = MessageText?.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            _isBusy = true;
            (SendMessageCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            StatusText = "ChatGPT práve píše...";

            Messages.Add(new ChatMessage
            {
                Sender = "Ja",
                Text = text,
                Timestamp = DateTime.Now,
                IsUser = true
            });

            MessageText = string.Empty;

            try
            {
                var reply = await _chatService.SendMessageAsync(Messages);
                Messages.Add(new ChatMessage
                {
                    Sender = "ChatGPT",
                    Text = reply,
                    Timestamp = DateTime.Now,
                    IsUser = false
                });

                StatusText = "Odpoveď prijatá.";
            }
            catch (Exception ex)
            {
                Messages.Add(new ChatMessage
                {
                    Sender = "Systém",
                    Text = "Nastala chyba: " + ex.Message,
                    Timestamp = DateTime.Now,
                    IsUser = false
                });

                StatusText = "Chyba pri odoslaní.";
            }
            finally
            {
                _isBusy = false;
                (SendMessageCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private sealed class AsyncRelayCommand : ICommand
        {
            private readonly Func<Task> _execute;
            private readonly Func<bool> _canExecute;

            public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute();
            }

            public async void Execute(object parameter)
            {
                await _execute();
            }

            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
