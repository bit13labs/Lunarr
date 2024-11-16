﻿using System;

namespace Lunarr.Host
{
    public class TerminateApplicationException : ApplicationException
    {
        public TerminateApplicationException(string reason)
            : base("Application is being terminated. Reason : " + reason)
        {
        }
    }
}
