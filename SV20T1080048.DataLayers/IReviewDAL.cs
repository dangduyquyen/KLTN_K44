using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080048.DataLayers
{
    using SV20T1080048.DomainModels;
    using System.Collections.Generic;

    public interface IReviewDAL
    {
        int AddReview(Review data);

        IList<Review> ListReview(int productid);


    }

}
