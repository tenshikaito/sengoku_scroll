﻿using Library.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class GameClient
    {
        public TcpClient tcpClient;
        public NetworkStream networkStream;
        public Action<GameClient> clientDisconnected;
        public Action<GameClient, string> dataReceived;

        private List<byte> receiveDataBytes;
        private List<byte> sendDataBytes;
        private byte[] buffer;

        public string ip => tcpClient.getIp();

        public GameClient(TcpClient tc, int dataBufferSize = 10240, int socketBufferSize = 4096)
        {
            tcpClient = tc;

            receiveDataBytes = new List<byte>(dataBufferSize);
            sendDataBytes = new List<byte>(dataBufferSize);
            buffer = new byte[socketBufferSize];
        }

        public void start()
        {
            networkStream = tcpClient.GetStream();

            _ = receive();
        }

        public async Task connect(string ip, int port)
        {
            await tcpClient.ConnectAsync(ip, port);
        }

        private async Task receive()
        {
            try
            {
                var length = await networkStream.ReadAsync(buffer, 0, buffer.Length);

                if (length > 0)
                {
                    receiveDataBytes.AddRange(buffer.Take(length));

                    processData();

                    _ = receive();
                }
                else
                {
                    clientDisconnected?.Invoke(this);

                    disconnect();
                }
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private void processData()
        {
            var r = NetworkHelper.splitDataStream(receiveDataBytes);

            if (r.hasResult)
            {
                dataReceived?.Invoke(this, r.data);

                processData();
            }
        }

        public async Task write(string data)
        {
            sendDataBytes.Clear();

            var ns = networkStream;

            var r = NetworkHelper.combineDataStream(sendDataBytes, data);

            await ns.WriteAsync(r, 0, r.Length);

            await ns.FlushAsync();
        }

        public void disconnect()
        {
            networkStream.Close();
            tcpClient.Close();
        }

        public static implicit operator TcpClient(GameClient gc) => gc.tcpClient;

        public static implicit operator NetworkStream(GameClient gc) => gc.networkStream;
    }
}
