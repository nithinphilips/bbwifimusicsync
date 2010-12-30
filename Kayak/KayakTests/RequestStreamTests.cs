﻿using System.Collections.Generic;
using System.Text;
using NUnit.Core;
using NUnit.Framework;
using Kayak;
using System.IO;
using System;
using Moq;
using System.Linq;

namespace KayakTests
{
    [TestFixture]
    public class RequestStreamTests
    {
        [TestFixture]
        public class ReadAsyncPacketTests
        {
            string first = "this is before";
            string last = " and this is after.";
            byte[] buffer;

            ArraySegment<byte> firstSeg;

            Mock<ISocket> mockSocket;
            Mock<IObserver<ArraySegment<byte>>> mockObserver;

            RequestStream stream;

            [SetUp]
            public void SetUp()
            {
                buffer = Encoding.ASCII.GetBytes(first + last);
                firstSeg = new ArraySegment<byte>(buffer, 0, first.Length);
                mockObserver = new Mock<IObserver<ArraySegment<byte>>>();
                mockSocket = new Mock<ISocket>();
            }

            [Test]
            public void First()
            {
                stream = new RequestStream(null, firstSeg, firstSeg.Count);

                mockObserver.Setup(o => o.OnNext(It.Is<ArraySegment<byte>>(v => v.Equals(firstSeg)))).Verifiable();
                mockObserver.Setup(o => o.OnError(It.IsAny<Exception>())).Throws(new Exception("Should not have got error."));
                mockObserver.Setup(o => o.OnCompleted()).Verifiable();

                stream.ReadAsync().Subscribe(mockObserver.Object);

                mockObserver.Verify();
            }

            [Test]
            public void FirstAndOne()
            {
                var restSeg = new ArraySegment<byte>(buffer, first.Length, last.Length);

                stream = new RequestStream(mockSocket.Object, firstSeg, buffer.Length);

                mockObserver.Setup(o => o.OnNext(It.Is<ArraySegment<byte>>(v => v.Equals(firstSeg)))).Verifiable();
                mockObserver.Setup(o => o.OnError(It.IsAny<Exception>())).Throws(new Exception("Should not have got error."));
                mockObserver.Setup(o => o.OnCompleted()).Verifiable();

                stream.ReadAsync().Subscribe(mockObserver.Object);

                mockObserver.Verify();

                int bufferSize = 1024;

                mockSocket
                    .Setup(s => s.Read(
                        It.Is<byte[]>(b => b.Length == bufferSize),
                        It.Is<int>(o => o == 0),
                        It.Is<int>(c => c == bufferSize)))
                    .Returns<byte[], int, int>((byte[] b, int o, int c) =>
                        Observable.Create<int>(ob =>
                        {
                            ob.OnNext(restSeg.Count);
                            ob.OnCompleted();
                            return () => { };
                        }))
                    .Verifiable();

                mockObserver.Setup(o => o.OnNext(It.Is<ArraySegment<byte>>(v => v.Count == restSeg.Count))).Verifiable();
                mockObserver.Setup(o => o.OnError(It.IsAny<Exception>())).Throws(new Exception("Should not have got error."));
                mockObserver.Setup(o => o.OnCompleted()).Verifiable();

                stream.ReadAsync().Subscribe(mockObserver.Object);

                mockSocket.Verify();
                mockObserver.Verify();
            }
        }

        //RequestStream requestStream;

        ////byte[] source;
        //SynchronousMemoryStream first, rest;
        //string firstString, restString;

        //int copyBufferSize = 1;
        //int firstBufferSize = 1;

        //byte[] copyBuffer;
        //MemoryStream destination;
        //int expectedLength;

        //public void SetUp()
        //{
        //    copyBuffer = new byte[copyBufferSize];
        //    destination = new MemoryStream();

        //    firstString = "This is, say, some headers or whatever.";
        //    restString = "This is, say, some body or whatever.";
        //    expectedLength = restString.Length;

        //    var sourceStream = new SynchronousMemoryStream(Encoding.UTF8.GetBytes(firstString + restString));

        //    // so we read a big hunk that goes into the body.

        //    var buffer = new byte[firstString.Length + firstBufferSize];

        //    sourceStream.Read(buffer, 0, buffer.Length);

        //    var firstBuffer = new byte[firstBufferSize];
        //    Buffer.BlockCopy(buffer, firstString.Length, firstBuffer, 0, firstBuffer.Length);

        //    //requestStream = new RequestStream(sourceStream, firstBuffer, restString.Length);
        //}

        //public string GetReadString()
        //{
        //    destination.Position = 0;
        //    return new StreamReader(destination, Encoding.UTF8).ReadToEnd();
        //}

        //public void AsyncRead()
        //{
        //    // since the underlying stream is synchronous, the read will be complete immediately.
        //    SetUp();
        //    BeginRead();

        //    destination.Position = 0;
        //    var readString = new StreamReader(destination, Encoding.UTF8).ReadToEnd();

        //    Assert.AreEqual(restString, GetReadString(), "Strings differ.");
        //}

        //void BeginRead()
        //{
        //    //Console.WriteLine("BeginRead");
        //    requestStream.BeginRead(copyBuffer, 0, copyBuffer.Length, ReadCallback, null);
        //}

        //void ReadCallback(IAsyncResult iasr)
        //{
        //    var bytesRead = requestStream.EndRead(iasr);
        //    //Console.WriteLine("Read " + bytesRead + " bytes.");
        //    destination.Write(copyBuffer, 0, bytesRead);
        //    //Console.WriteLine("destination.length = " + destination.Length);
        //    //Console.WriteLine("expectedLength = " + expectedLength);
        //    //Console.WriteLine();

        //    if (destination.Length < expectedLength && bytesRead != 0)
        //        BeginRead();
        //}

        //public void SyncRead()
        //{
        //    SetUp();

        //    int bytesRead = 0;
        //    do
        //    {
        //        bytesRead = requestStream.Read(copyBuffer, 0, copyBuffer.Length);
        //        //Console.WriteLine("Read " + bytesRead + " bytes.");
        //        destination.Write(copyBuffer, 0, bytesRead);
        //    }
        //    while (destination.Length < expectedLength && bytesRead != 0);

        //    Assert.AreEqual(restString, GetReadString(), "Strings differ.");
        //}

        //[Test]
        //public void AsyncRead1()
        //{
        //    copyBufferSize = 1;
        //    firstBufferSize = 1;
        //    AsyncRead();
        //}

        //[Test]
        //public void AsyncRead2()
        //{
        //    copyBufferSize = 2;
        //    firstBufferSize = 2;
        //    AsyncRead();
        //}

        //[Test]
        //public void AsyncRead3()
        //{
        //    copyBufferSize = 4;
        //    firstBufferSize = 10;
        //    AsyncRead();
        //}

        //[Test]
        //public void AsyncRead4()
        //{
        //    copyBufferSize = 20;
        //    firstBufferSize = 16;
        //    AsyncRead();
        //}

        //[Test]
        //public void SyncRead1()
        //{
        //    copyBufferSize = 1;
        //    firstBufferSize = 1;
        //    SyncRead();
        //}

        //[Test]
        //public void SyncRead2()
        //{
        //    copyBufferSize = 2;
        //    firstBufferSize = 2;
        //    SyncRead();
        //}

        //[Test]
        //public void SyncRead3()
        //{
        //    copyBufferSize = 4;
        //    firstBufferSize = 2;
        //    SyncRead();
        //}

        //[Test]
        //public void SyncRead4()
        //{
        //    copyBufferSize = 20;
        //    firstBufferSize = 16;
        //    SyncRead();
        //}
    }
}
