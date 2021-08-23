using DataAccess.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Hubs
{
    public class PostHub : Hub
    {
        protected IHubContext<PostHub> _context;

        public PostHub(IHubContext<PostHub> context)
        {
            this._context = context;
        }
        public async Task SendMessage(string FullName,string message )
        {
            await _context.Clients.All.SendAsync("ReceiveMessage", FullName, message);

        }

    }
}
