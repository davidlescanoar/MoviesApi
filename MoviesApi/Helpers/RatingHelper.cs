namespace MoviesApi.Helpers
{
    public static class RatingHelper
    {
        public static double RoundToNearest(double value, double step)
        {
            return Math.Round(value / step, MidpointRounding.AwayFromZero) * step;
        }
    }
}
