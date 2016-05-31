﻿using Moq;
using Server.Core;

namespace Server.Test
{
    public class MockHttpService : IHttpServiceProcessor
    {
        private readonly Mock<IHttpServiceProcessor> _mock;

        public MockHttpService()
        {
            _mock = new Mock<IHttpServiceProcessor>();
            _mock.Setup(m => m.CanProcessRequest(It.IsAny<string>(), It.IsAny<ServerProperties>())).Returns(true);
        }

        public bool CanProcessRequest(string request, ServerProperties serverProperties)
        {
            return _mock.Object.CanProcessRequest(request, serverProperties);
        }

        public IHttpResponse ProcessRequest(string request, IHttpResponse httpResponse, ServerProperties serverProperties)
        {
            return _mock.Object.ProcessRequest(request, httpResponse, serverProperties);
        }
    }
}