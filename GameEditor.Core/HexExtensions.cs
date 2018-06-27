using System;
using System.Globalization;

namespace GameEditor.Core
{

    internal static class HexExtensions
    {
        public static int ToInt( this string hexString )
        {
            string trimed = hexString.Trim();

            // strip the leading 0x
            if ( trimed.StartsWith( "0x", StringComparison.OrdinalIgnoreCase ) )
            {
                trimed = trimed.Substring( 2 );
            }

            return int.Parse( trimed, NumberStyles.HexNumber );
        }

        public static string ToHex( this int value )
        {
            return $"0x{value:X}";
        }
    }

}