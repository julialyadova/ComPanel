using System;
using System.Threading;

namespace ComPanel.DataFlow
{
    class DataFlowController
    {
        public Action<byte[], int> OnEventuallyDataRecieved;
        public Action<byte[], int> OnDataRecieved { get; }


        private const int waitingMS = 50;
        private const int pauseMaxTimeMS = 500;

        private Action<byte[], int> _send;
        private Action<byte[], int> _onDataRecieved;

        private bool _sending;
        private bool _pause;
        private int _responseCount;
        private readonly object _lockObject;

        private byte[] _timerData;
        private int _timerTimeOutMS = 500;
        private Thread timerThread;

        public DataFlowController(Action<byte[], int> send)
        {
            if (send == null)
                throw new ArgumentNullException("send", "send method must be not null. Unable to send Data.");

            _responseCount = 0;
            _lockObject = new object();
            _sending = true;
            _pause = false;

            _send = send;
            OnDataRecieved += (data, length) =>
                {
                    if (_pause)
                    {
                        _onDataRecieved?.Invoke(data, length);
                        _pause = false;
                        _onDataRecieved = null;
                    }
                    else
                    {
                        OnEventuallyDataRecieved?.Invoke(data, length);
                    }                    
                };
        }

        public void StartTimerSending(byte[] data, int timeOutMS, Action<byte[], int> onDataRecieved)
        {
            _timerData = data;
            _timerTimeOutMS = timeOutMS;
            _onDataRecieved = onDataRecieved;

            if (timerThread == null)
            {
                _sending = true;

                var checkPauseLocksThread = new Thread(MonitorPauseLock);
                checkPauseLocksThread.IsBackground = true;
                checkPauseLocksThread.Start();

                timerThread = new Thread(SendOnTimerThread);
                timerThread.IsBackground = true;
                timerThread.Start();
            }                
        }

        public void StopTimerSending()
        {
            _sending = false;
            timerThread = null;
        }

        public void Send(byte[] data, int length, Action<byte[], int> onDataRecieved = null)
        {
            while (!TryPause())
                Thread.Sleep(waitingMS);

            _send(data, length);
            if (onDataRecieved != null)
            {
                var rememberedAction = _onDataRecieved;
                _onDataRecieved = (data, length) =>
                {
                    onDataRecieved(data, length);
                    _onDataRecieved = rememberedAction;
                };
            }
        }

        private void SendOnTimerThread()
        {
            while (_sending)
            {
                if (!TryPause())
                {
                    Thread.Sleep(waitingMS);
                    continue;
                }

                if (_timerData != null)
                {
                    _send(_timerData, _timerData.Length);
                }
                Thread.Sleep(_timerTimeOutMS);
            }
        }

        private void MonitorPauseLock()
        {
            var lastResponseIndex = _responseCount;
            while (_sending)
            {
                Thread.Sleep(pauseMaxTimeMS);
                lock (_lockObject)
                {
                    if (_pause)
                    {
                        if (_responseCount == lastResponseIndex)
                            _pause = false;
                        else
                            lastResponseIndex = _responseCount;
                    }
                }
            }
        }

        private bool TryPause()
        {
            lock (_lockObject)
            {
                if (_pause)
                {
                    return false;
                }
                if (_responseCount == int.MaxValue)
                    _responseCount = 0;
                else
                    _responseCount++;
                _pause = true;
                return true;
            }
        }
    }
}

