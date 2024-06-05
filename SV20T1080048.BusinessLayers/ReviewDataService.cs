using SV20T1080048.BusinessLayers;
using SV20T1080048.DataLayers;
using SV20T1080048.DataLayers.SQLServer;
using SV20T1080048.DomainModels;


namespace SV20T1080048.BusinessLayers
{
    public static class ReviewDataService
    {
        private static readonly IReviewDAL reviewDB;

        static ReviewDataService()
        {
            string connectionString = "server=DESKTOP-HVNL2AM\\SQLEXPRESS;user id=sa;password=123123;database=OanhVuSpaDB;TrustServerCertificate=true";
            reviewDB = new DataLayers.SQLServer.ReviewDAL(connectionString);
        }

        public static void AddReview(Review review)
        {
            reviewDB.AddReview(review);
        }

        public static List<Review> ListReview(int productId)
        {
            return reviewDB.ListReview(productId).ToList();
        }

    }
}
