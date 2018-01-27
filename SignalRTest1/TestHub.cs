using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.AspNet.SignalR;
using StackExchange.Redis;

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
            //throw new ArgumentException("参数异常");
            return await Task.FromResult(p);
        }

        public void Login(string userName)
        {
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379"))
            {
                IDatabase db = redis.GetDatabase();//默认是访问 db0 数据库，可以通过方法参数指定数字访问不同的数据库

                db.StringSet("UserNameState" + userName, true);
                db.StringSet("ConnectionId_Username" + this.Context.ConnectionId, userName);
                db.SetAdd("UserNameSet", userName);
            }

            StateChange();

        }

        public void StateChange()
        {
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379"))
            {
                IDatabase db = redis.GetDatabase();
                var usernames = db.SetMembers("UserNameSet");
                var userNameStates = db.StringGet(usernames.Select(un => (RedisKey)("UserNameState" + un)).ToArray());
                Dictionary<string, bool> userNameStateDic = new Dictionary<string, bool>();
                for (int i = 0; i < usernames.Length; i++)
                {
                    userNameStateDic.Add(usernames[i], (bool)userNameStates[i]);
                }

                Clients.All.onUserStateChange(userNameStateDic);
            }
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
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379"))
            {
                var db = redis.GetDatabase();
                var userNmae = db.StringGet("ConnectionId_Username" + Context.ConnectionId);
                if (!userNmae.IsNullOrEmpty)
                {
                    db.StringSet("UserNameState" + userNmae, false);
                }

            }
            StateChange();
            return base.OnDisconnected(stopCalled);
        }
    }
}