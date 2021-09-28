using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using LoggerUtils;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using QuestionsWebApplication.Hubs;

namespace QuestionsWebApplication
{
    public class DataUpdateNotifier
    {
        private readonly static Lazy<DataUpdateNotifier> _instance = new Lazy<DataUpdateNotifier>(() => new DataUpdateNotifier(GlobalHost.ConnectionManager.GetHubContext<DataUpdatedHub>().Clients));
        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public static DataUpdateNotifier Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private DataUpdateNotifier(IHubConnectionContext<dynamic> pClients)
        {
            try
            {
                Clients = pClients;
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }

        /// <summary>
        /// Helper function that notifies all the hubs that are registered to this function to recieve a notification whenever this function gets called.
        /// </summary>
        public void NotifyDataChanged()
        {
            try
            {
                Clients.All.broadcastUpdateData();
            }
            catch (Exception tException)
            {
                Logger.WriteExceptionMessage(tException);
            }
        }
    }
}