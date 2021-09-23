using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace QuestionsWebApplication.Hubs
{
    [HubName("questionTicker")]
    public class DataUpdatedHub : Hub
    {
        private readonly DataUpdateNotifier DataUpdateNotifierInstance;

        public DataUpdatedHub() : this(DataUpdateNotifier.Instance) { }

        public DataUpdatedHub(DataUpdateNotifier pDataUpdateNotifierInstance)
        {
            DataUpdateNotifierInstance = pDataUpdateNotifierInstance;
        }
    }
}