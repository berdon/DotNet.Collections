using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleUsage
{
    public class MyService
    {
        public string Message { get; set; } = "Initial message";

        public void Foo()
        {
            Console.WriteLine(Message);
        }
    }
}
