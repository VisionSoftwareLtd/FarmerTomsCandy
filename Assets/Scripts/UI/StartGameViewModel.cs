using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blocks.Sessions.Common
{
    class StartGameViewModel : IDisposable, IDataSourceViewHashProvider, INotifyBindablePropertyChanged
    {
        SessionObserver m_SessionObserver;
        ISession m_Session;

        [CreateProperty]
        public bool CanStartGame
        {
            get => m_CanStartGame;
            private set
            {
                if (m_CanStartGame == value)
                {
                    return;
                }

                m_CanStartGame = value;
                Notify();
            }
        }

        bool m_CanStartGame;

        public StartGameViewModel(string sessionType)
        {
            m_SessionObserver = new SessionObserver(sessionType);
            m_SessionObserver.SessionAdded += OnSessionAdded;
            if (m_SessionObserver.Session != null)
            {
                OnSessionAdded(m_SessionObserver.Session);
            }
        }

        void OnSessionAdded(ISession newSession)
        {
            m_Session = newSession;
            m_Session.RemovedFromSession += OnSessionRemoved;
            m_Session.Deleted += OnSessionRemoved;
            CanStartGame = true;
        }

        void OnSessionRemoved()
        {
            CanStartGame = false;
            CleanupSession();
        }

        public void StartGame()
        {
            GameManager.Instance.HideSessionDocument();
        }

        void CleanupSession()
        {
            m_Session.RemovedFromSession -= OnSessionRemoved;
            m_Session.Deleted -= OnSessionRemoved;
            m_Session = null;
        }

        public void Dispose()
        {
            if (m_SessionObserver != null)
            {
                m_SessionObserver.Dispose();
                m_SessionObserver = null;
            }

            if (m_Session != null)
            {
                CleanupSession();
            }
        }

        /// <summary>
        /// This method is used by UIToolkit to determine if any data bound to the UI has changed.
        /// Instead of hashing the data, an m_CanStartGame boolean is toggled when changes occur.
        /// </summary>
        public long GetViewHashCode() => m_CanStartGame ? 1 : 0;

        /// <summary>
        /// Suggested implementation of INotifyBindablePropertyChanged from UIToolkit.
        /// </summary>
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        void Notify([CallerMemberName] string property = null)
        {
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        }
    }
}
