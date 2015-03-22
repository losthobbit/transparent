using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mapping
{
    /// <remarks>
    /// Keeping things simple.  http://www.uglybugger.org/software/post/friends_dont_let_friends_use_automapper
    /// </remarks>
    public interface IMapToExisting<TSource, TTarget>
    {
        void Map(TSource source, TTarget target);
    }
}
