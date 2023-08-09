using System.Data;

namespace ElectroPrognizer.Services.Models
{
    public class ForecastTable : DataTable
    {
        // An instance of a DataTable with some default columns.  Expressions help quickly calculate E
        public ForecastTable()
        {
            Columns.Add("Instance", typeof(int));    //The position in which this value occurred in the time-series
            Columns.Add("Value", typeof(double));     //The value which actually occurred
            Columns.Add("Forecast", typeof(double));  //The forecasted value for this instance
            Columns.Add("Holdout", typeof(bool));   //Identifies a holdout actual value row, for testing after err is calculated

            //E(t) = D(t) - F(t)
            Columns.Add("Error", typeof(double), "Forecast - Value");
            //Absolute Error = |E(t)|
            Columns.Add("AbsoluteError", typeof(double), "IIF(Error >= 0, Error, Error * -1)");
            //Percent Error = E(t) / D(t)
            Columns.Add("PercentError", typeof(double), "IIF(Value <> 0, Error / Value, Null)");
            //Absolute Percent Error = |E(t)| / D(t)
            Columns.Add("AbsolutePercentError", typeof(double), "IIF(Value <> 0, AbsoluteError / Value, Null)");
        }
    }
}
