using System;

namespace Test.Helpers
{
    public class TestRef<T> where T : class
    {
        public Func<T> GetRef { get; set; }
    }
}