#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
namespace DevExpress.Map.Native {
	public static class SqlDataRegex {
		public const string Empty = "EMPTY";
#if DOTNET
		static readonly RegexOptions Options = RegexOptions.Singleline;
#else
		static readonly RegexOptions Options = RegexOptions.Compiled | RegexOptions.Singleline;
#endif
		static readonly Regex GeometryContentRegex = new Regex(@"(?<geom>(POLYGON|POINT|MULTILINESTRING))\s*\((\s)*?(?<ring>\(.*\))\s*\)$", Options);
		static readonly Regex CoordListRegex = new Regex(@"\((?<val>.*?)\)", Options);
		static readonly Regex XYRegex = new Regex(@"(?<x>(-?\d+\.?\d*))\s*(?<y>(-?\d+\.?\d*))");
		static readonly Regex XYZMRegex = new Regex(@"(?<x>(-?\d+\.?\d*))\s*(?<y>(-?\d+\.?\d*))\s*(?<z>(-?\d*\.?\d*))\s*(?<m>(-?\d*\.?\d*))");
		static readonly Regex CoordRegex = new Regex(@"(?<val>(\-?\d+\.?\d*))");
		static readonly Regex ElementContentRegex = new Regex(@"\((\s)*?\((?<val>.*?)\)(\s)*?\)", Options);
		static readonly Regex GeometriesRegex = new Regex(@"\((?<val>.*)\)$", Options);
		static readonly Regex GeometriesListRegex = new Regex(@",(\s)*?[A-Z]", Options);
		static bool IsValidString(string s) {
			return !string.IsNullOrEmpty(s);
		}
		static string ReplaceForTrim(Match m) {
			string x = m.ToString();
			if(!string.IsNullOrEmpty(x))
				return x.Replace(",", "*");
			return x;
		}
		public static bool IsEmpty(string s) {
			return IsValidString(s) ? s.Trim().ToUpper() == Empty : false;
		}
		public static string GetGeometryContent(string input) {
			if (string.IsNullOrEmpty(input))
				return string.Empty;
			Match match = GeometryContentRegex.Match(input);
			if (match.Success)
				return match.Groups["ring"].Value;
			return string.Empty;
		}
		public static string GetGeometriesContent(string input) {
			if(IsEmpty(input))
				return string.Empty;
			Match match = GeometriesRegex.Match(input);
			if(match.Success)
				return match.Groups["val"].Value;
			return string.Empty;
		}
		public static List<string> GetGeometries(string input) {
			List<string> result = new List<string>();
			if(IsEmpty(input))
				return result;
			string[] elements = new string[0];
			string prepareStr = GeometriesListRegex.Replace(input, new MatchEvaluator(ReplaceForTrim));
			elements = Regex.Split(prepareStr, @"\*");
			foreach(string element in elements)
				result.Add(element.TrimStart());
			return result;   
		}
		public static List<string> GetElementContent(string input) {
			List<string> result = new List<string>();
			if(string.IsNullOrEmpty(input))
				return result;
			foreach(Match m in ElementContentRegex.Matches(input))
				if(m.Success)
					result.Add("(" + m.Groups["val"].Value + ")");
			return result;
		}
		public static List<string> GetCoordinateList(string input) {
			List<string> result = new List<string>();
			foreach (Match m in CoordListRegex.Matches(input))
				if (m.Success)
					result.Add("(" + m.Groups["val"].Value + ")");
			return result;
		}
		public static string[] SplitCoordinateList(string input) {
			return Regex.Split(input, ",");
		}
		public static double ParseCoordinate(string input) {
			double result  = double.NaN;
			if (string.IsNullOrEmpty(input))
				return result;
			Match match = CoordRegex.Match(input);
			if (match.Success) {
				return ToDouble(match.Groups["val"].Value);
			}
			return result;
		}
		[SuppressMessage("Microsoft.Design", "CA1021: Avoid out parameters")]
		public static void ParseCoordinates(string input, out double x, out double y) {
			x = double.NaN; y = double.NaN;
			if (string.IsNullOrEmpty(input))
				return;
			Match match = XYRegex.Match(input);
			if (match.Success) {
				x = ToDouble(match.Groups["x"].Value);
				y = ToDouble(match.Groups["y"].Value);
			}
		}
		[SuppressMessage("Microsoft.Design", "CA1021: Avoid out parameters")]
		public static void ParseCoordinatesM(string input, out double x, out double y, out double z, out double m) {
			x = double.NaN; y = double.NaN;
			z = double.NaN; m = double.NaN;
			if (string.IsNullOrEmpty(input))
				return;
			Match match = XYZMRegex.Match(input);
			if (match.Success) {
				x = ToDouble(match.Groups["x"].Value);
				y = ToDouble(match.Groups["y"].Value);
				Group zG = match.Groups["z"];
				Group zM = match.Groups["m"];
				if (!string.IsNullOrEmpty(zG.Value))
					z = ToDouble(zG.Value);
				if (!string.IsNullOrEmpty(zM.Value))
					m = ToDouble(zM.Value);
			}
		}
		static double ToDouble(string value) {
			return Convert.ToDouble(value.Trim(), CultureInfo.InvariantCulture);
		}
	}
}
