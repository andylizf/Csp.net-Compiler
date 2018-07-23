using System;
using System.Dynamic;

namespace UnionType
{
    public class Union<T, U>
    {
        private (String str, T value) a;
        private (String str, U value) b;
        public Object v
        {
            get
            {
                if (a.value != null)
                    return a.value;
                if (b.value != null)
                    return b.value;
                return null;
            }
        }

        public T va
        {
            set
            {
                va = value;
                vb = default(U);
            }
        }

        public U vb
        {
            set
            {
                vb = value;
                va = default(T);
            }
        }

        public Union() { }
        public Union((String str, T value) a, (String str, U value) b)
        {
            this.a = a;
            this.b = b;
        }
        public Union(T va)
        {
            this.va = va;
        }
    }
}
