namespace WUnderground.Data
{
    public enum DataFeatureChoices
    {
        /// <summary>
        /// Returns the short name description, expiration time and a long text description of a severe alert — if one has been issued for the searched upon location.
        /// </summary>
        /// <remarks></remarks>
        Alerts,
        /// <summary>
        /// Historical average temperature for today
        /// </summary>
        /// <remarks></remarks>
        Almanac,
        /// <summary>
        /// Returns the moon phase, sunrise and sunset times.
        /// </summary>
        /// <remarks></remarks>
        Astronomy,
        /// <summary>
        /// Returns the current temperature, weather condition, humidity, wind, 'feels like' temperature, barometric pressure, and visibility.
        /// </summary>
        /// <remarks></remarks>
        Conditions,
        /// <summary>
        /// Returns the current position, forecast, and track of all current hurricanes.
        /// </summary>
        /// <remarks></remarks>
        CurrentHurricane,
        /// <summary>
        /// Returns a summary of the weather for the next 3 days. This includes high and low temperatures, a string text forecast and the conditions.
        /// </summary>
        /// <remarks></remarks>
        Forecast,
        /// <summary>
        /// Returns a summary of the weather for the next 10 days. This includes high and low temperatures, a string text forecast and the conditions.
        /// </summary>
        /// <remarks></remarks>
        Forecast10Day,
        /// <summary>
        /// Returns the the city name, zip code / postal code, latitude-longitude coordinates and nearby personal weather stations.
        /// </summary>
        /// <remarks></remarks>
        GeoLookup,
        /// <summary>
        /// history_YYYYMMDD returns a summary of the observed weather for the specified date.
        /// </summary>
        /// <remarks></remarks>
        History,
        /// <summary>
        /// Returns an hourly forecast for the next 36 hours immediately following the API request.
        /// </summary>
        /// <remarks></remarks>
        Hourly,
        /// <summary>
        /// Returns an hourly forecast for the next 10 days
        /// </summary>
        /// <remarks></remarks>
        Hourly10Day,
        /// <summary>
        /// planner_MMDDMMDD returns a weather summary based on historical information between the specified dates (30 days max).
        /// </summary>
        /// <remarks></remarks>
        Planner,
        /// <summary>
        /// Raw Tidal information for graphs
        /// </summary>
        /// <remarks></remarks>
        RawTide,
        /// <summary>
        /// Tidal information
        /// </summary>
        /// <remarks></remarks>
        Tide,
        /// <summary>
        /// Returns locations of nearby Personal Weather Stations and URL's for images from their web cams.
        /// </summary>
        /// <remarks></remarks>
        Webcams,
        /// <summary>
        /// Returns a summary of the observed weather history for yesterday.
        /// </summary>
        /// <remarks></remarks>
        Yesterday
    }

}
