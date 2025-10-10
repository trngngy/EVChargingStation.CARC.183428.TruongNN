﻿using System.Text.RegularExpressions;

namespace EVChargingStation.CARC.Infrastructure.TruongNN.Utils;

public static class StringExtensions
{
    private static readonly Regex _stripJsonWhitespaceRegex =
        new("(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", RegexOptions.Compiled);

    public static string StripJsonWhitespace(this string json)
    {
        return _stripJsonWhitespaceRegex.Replace(json, "$1");
    }
}