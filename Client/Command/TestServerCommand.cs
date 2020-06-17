﻿using Client.Model;
using Client.UI.SceneTitle;
using Library.Helper;
using Library.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Client.FormMain;

namespace Client.Command
{
    public class TestServerCommand
    {
        public async Task send(ServerInfo o, Dictionary<string, int?> map, Dispatcher dispatcher, UIStartGameDialog uiStartGameDialog)
        {
            try
            {
                var gc = NetworkHelper.getGameClient();

                await gc.send(o.ip, o.port, new TestServerData()
                {
                    serverCode = o.code,
                    timeStamp = DateTime.Now
                }.toCommandString(nameof(TestServerCommand)), (ogc, data) =>
                {
                    var (name, s) = data.fromCommandString();

                    var c = s.fromJson<TestServerData>();

                    map[c.serverCode] = (int)(DateTime.Now - c.timeStamp).TotalMilliseconds;

                    gc.disconnect();

                    dispatcher.invoke(() => uiStartGameDialog.refresh(map));
                });
            }
            catch
            {
                map[o.code] = null;

                dispatcher.invoke(() => uiStartGameDialog.refresh(map));
            }
        }
    }
}
