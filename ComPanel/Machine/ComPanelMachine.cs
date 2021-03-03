using System;
using System.Collections.Generic;


namespace ComPanel.Machine
{
	/// <summary>
	/// Summary description for Class1
	/// </summary>
	public class ComPanelMachine
	{
		const int waitTime = 100;

		Action CurrentState;
		Queue<Message> messageQueue;
		int time;

		public ComPanelMachine()
		{
			CurrentState = Passive;
		}

		public ComPanelMachine SendMessage(Message message)
        {
			messageQueue.Enqueue(message);
			CurrentState();
			return this;
        }

		public ComPanelMachine GetResponse(byte[] message)
		{
			
			return this;
		}


		void Passive()
        {
        }

		void Sending()
		{
			if (messageQueue.Count != 0)
            {
				Send(messageQueue.Dequeue());
				CurrentState = WaitForResponse;
            }
		}

		void WaitForResponse()
		{
			if (time < waitTime)
            {
				Send(messageQueue.Dequeue());
            }
		}

		void Send(Message message)
        {

        }
	}

	public class Message
    {
		public byte[] Data;
		public int Offset;
		public int Count;
		
		int timer;

		public void AddTime(int seconds)
        {
			timer += seconds;
        }
    }
}

