﻿using System;
using System.Threading;

namespace MyFinance.Methods
{
    public static class ExtensionMethods
    {
        public static void RunAction(this SynchronizationContext context, Action actionToPerform)
        {
            context.Post((object ob) => actionToPerform(), null);
        }
    }
}
