using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progress.tests
{
    public class Test1
    {
        public T CreateInstance<T>() where T : new()
        {
            return new T();
        }
    }
}
