using System;
using System.Threading;

namespace ComPanel.DataFlow
{
    class DataFlowController
    {
        public Action<byte[], int> OnUnexpectedDataRecieved;


        private const int waitingMS = 50;
        private const int pauseMaxTimeMS = 500;
        private const int waitingLimit = 8;

        private Action<byte[], int> _send;
        private Action<byte[], int> _OnExpectedDataRecieved;

        private bool _sending;        
        private int _waitingCount;

        private bool _expectingData;
        private int _responseCount;
        private readonly object _lockObject;
        private Thread pauseLockMonitor;

        public DataFlowController(Action<byte[], int> send)
        {
            if (send == null)
                throw new ArgumentNullException("send", "send method must be not null. Unable to send Data.");

            _send = send;

            _sending = true;
            _waitingCount = 0;

            _expectingData = false;
            _responseCount = 0;
            _lockObject = new object();

            pauseLockMonitor = new Thread(MonitorPauseLock);
            pauseLockMonitor.IsBackground = true;
            pauseLockMonitor.Start();
        }

        public void Start()
        {
            _sending = true;
        }

        public void Pause()
        {
            _sending = false;
            _OnExpectedDataRecieved = null;
            _expectingData = false;
        }

        public void Send(byte[] data, int length, Action<byte[], int> onDataRecieved = null)
        {
            //sending is available and not ot many requests at one time
            if (!_sending || _waitingCount == waitingLimit)
                return;

            //waiting for another messages to send and recieve answers
            _waitingCount++;
            while (!TryPause())
            {
                if (_sending)
                    Thread.Sleep(waitingMS);
                else
                    return;
            }

            //no sense to wait if not expecting data
            if (onDataRecieved == null)
                _expectingData = false;
            else
                _OnExpectedDataRecieved = onDataRecieved;
            _send(data, length);
            //stops waiting
            _waitingCount--;
        }

        public void RecieveData(byte[] data, int length)
        {
            if (_expectingData)
            {
                _OnExpectedDataRecieved?.Invoke(data, length);
                _expectingData = false;
            }
            else
            {
                OnUnexpectedDataRecieved?.Invoke(data, length);
            }
        }

        private bool TryPause()
        {
            lock (_lockObject)
            {
                if (_expectingData)
                    return false;

                if (_responseCount == int.MaxValue)
                    _responseCount = 0;
                else
                    _responseCount++;

                _expectingData = true;
                return true;
            }
        }

        private void MonitorPauseLock()
        {
            var lastResponseIndex = _responseCount;
            while (true)
            {
                Thread.Sleep(pauseMaxTimeMS);
                lock (_lockObject)
                {
                    if (_expectingData)
                    {
                        if (_responseCount == lastResponseIndex)
                            _expectingData = false;
                        else
                            lastResponseIndex = _responseCount;
                    }
                }
            }
        }
    }
}

