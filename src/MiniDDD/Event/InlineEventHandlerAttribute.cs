using System;

namespace MiniDDD
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class InlineEventHandlerAttribute : Attribute
    {
    }
}
