﻿namespace Kala.Common
{
    /// <summary>
    /// A class that holds all the constants for the app
    /// </summary>
    public sealed class Constants
    {
        /// <summary>
        /// Holds the constants used in server calls
        /// </summary>
        public sealed class Api
        {
            /// <summary>
            /// The call to fetch the sitemaps
            /// </summary>
            public const string Sitemaps = "rest/sitemaps";

            /// <summary>
            /// The call to set updates
            /// </summary>
            public const string Items = "rest/items/";

            /// <summary>
            /// The call to get updates
            /// </summary>
            public const string Events = "rest/events/?type=ItemStateEvent";
        }
    }
}
