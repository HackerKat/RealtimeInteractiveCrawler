using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLib
{
    public class MessageQueue
    {
        private Queue<Packet> messages;

        public MessageQueue()
        {
            messages = new Queue<Packet>();
        }

        public void Push(Packet p)
        {
            lock (messages) 
            {
                messages.Enqueue(p);
            }
        }

        public Packet Pop()
        {
            lock (messages)
            {
                if(messages.Count > 0)
                {
                    return messages.Dequeue();
                }
                return null;
            }
        }

        public int Count()
        {
            lock (messages)
            {
                return messages.Count;
            }
        }
    }
}
