using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalRTest1
{
    public class Person
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class TestHub : Hub
    {

        public void SendMessage(string name, string msg)
        {

            Clients.All.onMessage(name + "跟大家说" + msg);
        }
        public async Task<Person> Hello()
        {
            Person p = new Person
            {
                Id = 333,
                Name = "Tom",
                Age = 23
            };
            throw new ArgumentException("参数异常");
            return await Task.FromResult(p);
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
    }
}