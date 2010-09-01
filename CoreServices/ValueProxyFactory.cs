using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataEngine.CoreServices
{
    public abstract class ValueProxyFactory
    {
        public abstract ValueProxy Create(Object value);

        public abstract int GetValueCode();

        public abstract Type GetValueType();

        public virtual Type GetResultType()
        {
            return GetValueType();
        }

        public abstract bool IsNumeric { get; }        

        public abstract int Compare(ValueProxyFactory other);        
    }
}
