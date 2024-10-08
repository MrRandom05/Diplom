using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Entities
{
    public abstract class Docs
    {
        public abstract Document CastToDocument();

        public abstract T CastFromDocument<T>();
    }
}
