using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class StanceDetail
    {
        public StanceDetail(int? forId, int? againstId, int? neutralId)
	    {
            if (forId.HasValue)
            {
                Stance = Stance.For;
                Id = forId.Value;
            }
            else
                if (againstId.HasValue)
                {
                    Stance = Stance.Against;
                    Id = againstId.Value;
                }
                else
                    if (neutralId.HasValue)
                    {
                        Stance = Stance.Neutral;
                        Id = neutralId.Value;
                    }
	    }            

        public Stance Stance { get; set; }
        public int Id { get; set; }
    }
}
