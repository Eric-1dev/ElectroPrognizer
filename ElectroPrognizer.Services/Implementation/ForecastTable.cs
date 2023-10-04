using System.Data;
using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Implementation;

public class TimeSeries
{
    public const int DEFAULT_IGNORE = 3;
    public const int DEFAULT_HOLDOUT = 6;

    //Bayes: use prior actual value as forecast
    public static ForecastTable Naive(decimal[] values, int Extension, int Holdout)
    {
        var dt = new ForecastTable();

        for (int i = 0; i < (values.Length + Extension); i++)
        {
            //Insert a row for each value in set
            DataRow row = dt.NewRow();
            dt.Rows.Add(row);

            row.BeginEdit();
            //assign its sequence number
            row["Instance"] = i;

            //if i is in the holdout range of values
            row["Holdout"] = (i > (values.Length - 1 - Holdout)) && (i < values.Length);

            if (i < values.Length)
            { //processing values which actually occurred
              //Assign the actual value to the DataRow
                row["Value"] = values[i];
                if (i == 0)
                {
                    //first row, value gets itself
                    row["Forecast"] = values[i];
                }
                else
                {
                    //Put the prior row's value into the current row's forecasted value
                    row["Forecast"] = values[i - 1];
                }
            }
            else
            {//Extension rows
                row["Forecast"] = values[values.Length - 1];
            }
            row.EndEdit();
        }
        dt.AcceptChanges();
        return dt;
    }

    //
    //Simple Moving Average
    //
    //            ( Dt + D(t-1) + D(t-2) + ... + D(t-n+1) )
    //  F(t+1) =  -----------------------------------------
    //                              n
    public static ForecastTable SimpleMovingAverage(double[] values, int Extension, int Periods, int Holdout)
    {
        var dt = new ForecastTable();

        for (var i = 0; i < values.Length + Extension; i++)
        {
            //Insert a row for each value in set
            var row = dt.NewRow();

            dt.Rows.Add(row);

            row.BeginEdit();
            //assign its sequence number
            row["Instance"] = i;

            if (i < values.Length)
            {//processing values which actually occurred
                row["Value"] = values[i];
            }

            //Indicate if this is a holdout row
            row["Holdout"] = (i > (values.Length - Holdout)) && (i < values.Length);

            if (i == 0)
            {//Initialize first row with its own value
                row["Forecast"] = values[i];
            }
            else if (i <= values.Length - Holdout)
            {//processing values which actually occurred, but not in holdout set
                var avg = 0.0;
                var rows = dt.Select("Instance>=" + (i - Periods).ToString() + " AND Instance < " + i.ToString(), "Instance");

                foreach (var priorRow in rows)
                {
                    avg += (double)priorRow["Value"];
                }
                avg /= rows.Length;

                row["Forecast"] = avg;
            }
            else
            {//must be in the holdout set or the extension
                var avg = 0.0;

                //get the Periods-prior rows and calculate an average actual value
                var rows = dt.Select("Instance>=" + (i - Periods).ToString() + " AND Instance < " + i.ToString(), "Instance");

                foreach (var priorRow in rows)
                {
                    if ((int)priorRow["Instance"] < values.Length)
                    {//in the test or holdout set
                        avg += (double)priorRow["Value"];
                    }
                    else
                    {//extension, use forecast since we don't have an actual value
                        avg += (double)priorRow["Forecast"];
                    }
                }
                avg /= rows.Length;

                //set the forecasted value
                row["Forecast"] = avg;
            }
            row.EndEdit();
        }

        dt.AcceptChanges();
        return dt;
    }

    //
    //Weighted Moving Average
    //            
    //  F(t+1) =  (Weight1 * D(t)) + (Weight2 * D(t-1)) + (Weight3 * D(t-2)) + ... + (WeightN * D(t-n+1))
    //          
    public static ForecastTable WeightedMovingAverage(double[] values, int Extension, params double[] PeriodWeight)
    {
        //PeriodWeight[].Length is used to determine the number of periods over which to average
        //PeriodWeight[x] is used to apply a weight to the prior period's value

        //Make sure PeriodWeight values add up to 100%
        var test = 0.0;

        foreach (var weight in PeriodWeight)
        {
            test += weight;
        }

        if (test != 1)
            throw new Exception("Period weights must add up to 1.0");

        var dt = new ForecastTable();

        for (var i = 0; i < values.Length + Extension; i++)
        {
            //Insert a row for each value in set
            var row = dt.NewRow();

            dt.Rows.Add(row);

            row.BeginEdit();
            //assign its sequence number
            row["Instance"] = i;

            if (i < values.Length)
            {//we're in the test set
                row["Value"] = values[i];
            }

            if (i == 0)
            {//initialize forecast with first row's value
                row["Forecast"] = values[i];
            }
            else if ((i < values.Length) && (i < PeriodWeight.Length))
            {//processing one of the first rows, before we've advanced enough to properly weight past rows
                var avg = 0.0;

                //Get the datarows representing the values within the WMA length
                var rows = dt.Select("Instance>=" + (i - PeriodWeight.Length).ToString() + " AND Instance < " + i.ToString(), "Instance");

                for (int j = 0; j < rows.Length; j++)
                {//apply an initial, uniform weight (1 / rows.Length) to the initial rows
                    avg += (double)rows[j]["Value"] * (1 / rows.Length);
                }
                row["Forecast"] = avg;
            }
            else if ((i < values.Length) && (i >= PeriodWeight.Length))
            {//Out of initial rows and processing the test set
                var avg = 0.0;

                //Get the rows within the weight range just prior to the current row
                var rows = dt.Select("Instance>=" + (i - PeriodWeight.Length).ToString() + " AND Instance < " + i.ToString(), "Instance");

                for (int j = 0; j <= rows.Length - 1; j++)
                {//Apply the appropriate period's weight to the value
                    avg += (double)rows[j]["Value"] * PeriodWeight[j];
                }
                //Assign the forecasted value to the current row
                row["Forecast"] = avg;
            }
            else
            {//into the extension
                var avg = 0.0;

                var rows = dt.Select("Instance>=" + (i - PeriodWeight.Length).ToString() + " AND Instance < " + i.ToString(), "Instance");

                for (int j = 0; j < rows.Length; j++)
                {//with no actual values to weight, use the previous rows' forecast instead
                    avg += (double)rows[j]["Forecast"] * PeriodWeight[j];
                }
                row["Forecast"] = avg;
            }
            row.EndEdit();
        }

        dt.AcceptChanges();
        return dt;
    }

    //
    //Exponential Smoothing
    //
    //  F(t+1) =    ( Alpha * D(t) ) + (1 - Alpha) * F(t)
    //
    public static ForecastTable ExponentialSmoothing(double[] values, int Extension, double Alpha)
    {
        var dt = new ForecastTable();

        for (var i = 0; i < (values.Length + Extension); i++)
        {
            //Insert a row for each value in set
            var row = dt.NewRow();

            dt.Rows.Add(row);

            row.BeginEdit();
            //assign its sequence number
            row["Instance"] = i;
            if (i < values.Length)
            {//test set
                row["Value"] = values[i];
                if (i == 0)
                {//initialize first value
                    row["Forecast"] = values[i];
                }
                else
                {//main area of forecasting
                    var priorRow = dt.Select("Instance=" + (i - 1).ToString())[0];
                    var PriorForecast = (double)priorRow["Forecast"];
                    var PriorValue = (double)priorRow["Value"];

                    row["Forecast"] = PriorForecast + (Alpha * (PriorValue - PriorForecast));
                }
            }
            else
            {//extension has to use prior forecast instead of prior value
                var priorRow = dt.Select("Instance=" + (i - 1).ToString())[0];
                var PriorForecast = (double)priorRow["Forecast"];
                var PriorValue = (double)priorRow["Forecast"];

                row["Forecast"] = PriorForecast + (Alpha * (PriorValue - PriorForecast));
            }
            row.EndEdit();
        }

        dt.AcceptChanges();

        return dt;
    }

    //
    // Adaptive Rate Smoothing
    //
    public static ForecastTable AdaptiveRateSmoothing(double[] values, int Extension, double MinGamma, double MaxGamma)
    {
        var dt = new ForecastTable();

        for (var i = 0; i < (values.Length + Extension); i++)
        {
            //Insert a row for each value in set
            var row = dt.NewRow();

            dt.Rows.Add(row);

            row.BeginEdit();
            //assign its sequence number
            row["Instance"] = i;
            if (i < values.Length)
            {
                row["Value"] = values[i];

                if (i == 0)
                {//initialize first row
                    row["Forecast"] = values[i];
                }
                else
                {//calculate gamma and forecast value
                    var priorRow = dt.Select("Instance=" + (i - 1).ToString())[0];
                    var PriorForecast = (double)priorRow["Forecast"];
                    var PriorValue = (double)priorRow["Value"];

                    var Gamma = Math.Abs(TrackingSignal(dt, false, 3));
                    if (Gamma < MinGamma)
                        Gamma = MinGamma;
                    if (Gamma > MaxGamma)
                        Gamma = MaxGamma;

                    row["Forecast"] = PriorForecast + (Gamma * (PriorValue - PriorForecast));
                }
            }
            else
            {//extension set, can't use actual values anymore
                var priorRow = dt.Select("Instance=" + (i - 1).ToString())[0];
                var PriorForecast = (double)priorRow["Forecast"];
                var PriorValue = (double)priorRow["Forecast"];

                var Gamma = Math.Abs(TrackingSignal(dt, false, 3));
                if (Gamma < MinGamma)
                    Gamma = MinGamma;
                if (Gamma > MaxGamma)
                    Gamma = MaxGamma;

                row["Forecast"] = PriorForecast + (Gamma * (PriorValue - PriorForecast));
            }
            row.EndEdit();
        }

        dt.AcceptChanges();

        return dt;

    }

    //MeanSignedError = Sum(E(t)) / n
    public static double MeanSignedError(ForecastTable dt, bool Holdout, int IgnoreInitial)
    {
        var Filter = "Error Is Not Null AND Instance > " + IgnoreInitial.ToString();

        if (Holdout)
            Filter += " AND Holdout=True";

        if (dt.Select(Filter).Length == 0)
            return 0;

        return (double)dt.Compute("Avg(Error)", Filter);
    }

    //MeanAbsoluteError = Sum(|E(t)|) / n
    public static double MeanAbsoluteError(ForecastTable dt, bool Holdout, int IgnoreInitial)
    {
        var Filter = "AbsoluteError Is Not Null AND Instance > " + IgnoreInitial.ToString();
        if (Holdout)
            Filter += " AND Holdout=True";

        if (dt.Select(Filter).Length == 0)
            return 0;

        return (double)dt.Compute("Avg(AbsoluteError)", Filter);
    }

    //MeanPercentError = Sum( PercentError ) / n
    public static double MeanPercentError(ForecastTable dt, bool Holdout, int IgnoreInitial)
    {
        var Filter = "PercentError Is Not Null AND Instance > " + IgnoreInitial.ToString();

        if (Holdout)
            Filter += " AND Holdout=True";

        if (dt.Select(Filter).Length == 0)
            return 0;

        return (double)dt.Compute("Avg(PercentError)", Filter);
    }

    //MeanAbsolutePercentError = Sum( |PercentError| ) / n
    public static double MeanAbsolutePercentError(ForecastTable dt, bool Holdout, int IgnoreInitial)
    {
        var Filter = "AbsolutePercentError Is Not Null AND Instance > " + IgnoreInitial.ToString();

        if (Holdout)
            Filter += " AND Holdout=True";

        if (dt.Select(Filter).Length == 0)
            return 1;

        return (double)dt.Compute("AVG(AbsolutePercentError)", Filter);
    }

    //Tracking signal = MeanSignedError / MeanAbsoluteError
    public static double TrackingSignal(ForecastTable dt, bool Holdout, int IgnoreInitial)
    {
        var MAE = MeanAbsoluteError(dt, Holdout, IgnoreInitial);

        if (MAE == 0)
            return 0;

        return MeanSignedError(dt, Holdout, IgnoreInitial) / MAE;
    }

    //MSE = Sum( E(t)^2 ) / n
    public static double MeanSquaredError(ForecastTable dt, bool Holdout, int IgnoreInitial, int DegreesOfFreedom)
    {
        var SquareError = 0.0;
        var Filter = "Error Is Not Null AND Instance > " + IgnoreInitial.ToString();

        if (Holdout)
            Filter += " AND Holdout=True";

        var rows = dt.Select(Filter);
        if (rows.Length == 0)
            return 0;

        foreach (var row in rows)
        {
            SquareError = (double)Math.Pow(double.Parse(row["Error"].ToString()), (double)2.0);
        }

        return SquareError / (dt.Rows.Count - DegreesOfFreedom);
    }

    //CumulativeSignedError = Sum( E(t) )
    public static double CumulativeSignedError(ForecastTable dt, bool Holdout, int IgnoreInitial)
    {
        var Filter = "Error Is Not Null AND Instance > " + IgnoreInitial.ToString();

        if (Holdout)
            Filter += " AND Holdout=True";

        if (dt.Select(Filter).Length == 0)
            return 0;

        return (double)dt.Compute("SUM(Error)", Filter);
    }

    //CumulativeAbsoluteError = Sum( |E(t)| )
    public static double CumulativeAbsoluteError(ForecastTable dt, bool Holdout, int IgnoreInitial)
    {
        var Filter = "AbsoluteError Is Not Null AND Instance > " + IgnoreInitial.ToString();

        if (Holdout)
            Filter += " AND Holdout=True";

        if (dt.Select(Filter).Length == 0)
            return 0;

        return (double)dt.Compute("SUM(AbsoluteError)", Filter);
    }
}
