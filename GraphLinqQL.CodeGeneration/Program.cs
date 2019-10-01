using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine(string.Join(",", args));
            // TODO - use args to build options and do code generation
            await Task.Yield();
        }
    }
}
