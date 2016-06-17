﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    public class DefaultZSocket : IZSocket
    {
        private const int BufferSize = 1024;
        private readonly Socket _tcpSocket;

        public DefaultZSocket(IPEndPoint localEndPoint)
        {
            _tcpSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            _tcpSocket.Bind(localEndPoint);
            _tcpSocket.Listen(int.MaxValue);
        }

        public DefaultZSocket(Socket tcpSocket)
        {
            _tcpSocket = tcpSocket;
        }

        public IZSocket Accept()
        {
            return new DefaultZSocket(_tcpSocket.Accept());
        }

        public void Close()
        {
            _tcpSocket.Close();
        }

        public bool Connected()
        {
            return _tcpSocket.Connected;
        }

        public int Send(string message)
        {
            return _tcpSocket.Send(Encoding.ASCII.GetBytes(message));
        }

        public void SendFile(string message)
        {
            using (var fileStream = File.Open(message,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                var buffer = new byte[BufferSize];
                int bytesRead;
                while ((bytesRead = fileStream.Read(buffer, 0, BufferSize)) > 0)
                {
                    _tcpSocket.Send(buffer, bytesRead, SocketFlags.None);
                }
            }
        }

        public string Receive()
        {
            var readData = new byte[BufferSize];
            var lengthRead = _tcpSocket.Receive(readData);
            return (Encoding.Default.GetString(readData).Substring(0, lengthRead));
        }
    }
}