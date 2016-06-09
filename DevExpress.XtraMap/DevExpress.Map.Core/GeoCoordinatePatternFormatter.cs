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

using DevExpress.Map.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace DevExpress.Map.Native {
	public abstract class CoordinatePatternFormatterBase {
		string currentUserPattern;
		string formatString;
		Regex regex;
		double coordinate;
		List<object> elementsToReplace = new List<object>();
		protected abstract string CoordinatePattern { get; }
		protected abstract string DefaultPattern { get; }
		protected string CurrentUserPattern { get { return currentUserPattern; } }
		protected string FormatString { get { return formatString; } }
		protected Regex Regex { get { return regex; } }
		protected double Coordinate { get { return coordinate; } }
		protected List<object> ElementsToReplace { get { return elementsToReplace; } }
		protected CoordinatePatternFormatterBase(string userPattern) {
			this.currentUserPattern = userPattern;
			this.regex = new Regex(CoordinatePattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
			this.formatString = ParsePattern();
		}
		string ParsePattern() {
			if (string.IsNullOrEmpty(CurrentUserPattern) || !string.Equals(Regex.Match(CurrentUserPattern).Value, CurrentUserPattern)) {
				this.elementsToReplace = GetDefaultElements();
				return DefaultPattern;
			}
			MatchEvaluator ev = new MatchEvaluator(OnPatternRecognized);
			return Regex.Replace(CurrentUserPattern, ev);
		}
		string ApplyPattern() {
			object[] parsedObjects = GetParsedObjects();
			return string.Format(CultureInfo.InvariantCulture, FormatString, parsedObjects);
		}
		protected abstract object[] GetParsedObjects();
		protected abstract List<object> GetDefaultElements();
		protected abstract string OnPatternRecognized(Match match);
		protected string DecodeSimpleString(string pattern, string elementSymbol, string elementPatternFormat, string formatReplacement, object element) {
			string formatString = pattern;
			string elementPattern = string.Format(elementPatternFormat, elementSymbol);
			if (formatString.Contains(elementPattern)) {
				formatString = formatString.Replace(elementPattern, string.Format(formatReplacement, ElementsToReplace.Count));
				ElementsToReplace.Add(element);
			}
			return formatString;
		}
		public string Format(double coordinate) {
			this.coordinate = coordinate;
			return ApplyPattern();
		}
	}
	public class CartesianCoordinatePatternFormatter : CoordinatePatternFormatterBase {
		const string CartesianCoordinatePattern = @"(\{(MU|F|F:\d)\}|([^\{\}]))*";
		MeasureUnitCore measureUnit;
		protected override string CoordinatePattern { get { return CartesianCoordinatePattern; } }
		protected override string DefaultPattern { get { return "{0:F1}{1}"; } }
		public CartesianCoordinatePatternFormatter(string userPattern, MeasureUnitCore measureUnit)
			: base(userPattern) {
				this.measureUnit = measureUnit;
		}
		protected override List<object> GetDefaultElements() {
			return new List<object>() { CartesianCoordinateElement.PrecisionValue, CartesianCoordinateElement.MeasureUnit };
		}
		protected override string OnPatternRecognized(Match match) {
			if (match == null || !match.Success)
				return null;
			string result = match.Value;
			result = DecodeSimpleString(result, "F", "{{{0}}}", "{{{0}}}", CartesianCoordinateElement.Value);
			result = DecodeSimpleString(result, "MU", "{{{0}}}", "{{{0}}}", CartesianCoordinateElement.MeasureUnit);
			result = DecodeSimpleString(result, "F", "{{{0}:", "{{{0}:F", CartesianCoordinateElement.PrecisionValue);
			return result;
		}
		protected override object[] GetParsedObjects() {
			List<object> parsedObjects = new List<object>();
			foreach (CartesianCoordinateElement element in ElementsToReplace) {
				switch (element) {
					case CartesianCoordinateElement.Value:
						parsedObjects.Add(Math.Round(Coordinate, 0));
						break;
					case CartesianCoordinateElement.MeasureUnit:
						parsedObjects.Add(measureUnit.Abbreviation);
						break;
					case CartesianCoordinateElement.PrecisionValue:
						parsedObjects.Add(Coordinate);
						break;
				}
			}
			return parsedObjects.ToArray();
		}
	}
	public class GeoCoordinatePatternFormatter : CoordinatePatternFormatterBase {
		public const string DegreeChar = "°";
		public const string MuniteChar = "'";
		public const string SecondChar = "''";
		public const string CardinalPointPattern = "CP";
		public const string DegreePattern = "D";
		public const string MinutePattern = "M";
		public const string SecondPattern = "S";
		const string GeoCoordinatePattern = @"(\{(CP|D|(D|M|S):\d|M|S)\}|([^\{\}]))*";
		CardinalDirection direction;
		protected override string DefaultPattern { get { return "{0:F1}" + DegreeChar + "{1}"; } }
		protected override string CoordinatePattern { get { return GeoCoordinatePattern; } }
		public GeoCoordinatePatternFormatter(string userPattern, CardinalDirection direction) : base(userPattern) {
			this.direction = direction;
		}
		string CalculateDirectionSign(double coordinate) {
			bool isNegative = coordinate <= 0;
			if (direction == CardinalDirection.NorthSouth)
				return isNegative ? MapLocalizer.GetString(MapStringId.LatitudeNegativeChar) : MapLocalizer.GetString(MapStringId.LatitudePositiveChar);
			return isNegative ? MapLocalizer.GetString(MapStringId.LongitudeNegativeChar) : MapLocalizer.GetString(MapStringId.LongitudePositiveChar);
		}
		string DecodePrecisionString(string pattern, string elementSymbol, object element) {
			string formatString = pattern;
			string elementPattern = string.Format("{{{0}:", elementSymbol);
			if (formatString.Contains(elementPattern)) {
				int elementStartIndex;
				do {
					elementStartIndex = formatString.IndexOf(string.Format("{0}", elementPattern));
					if (elementStartIndex != -1) {
						int elementPrecision = GetElementPrecision(formatString, elementStartIndex);
						string precisionMask = new string('0', elementPrecision);
						formatString = formatString.Replace(string.Format("{0}{1}", elementPattern, elementPrecision), string.Format("{{{0}:00.{1}", ElementsToReplace.Count, precisionMask));
					}
				} while (elementStartIndex != -1);
				ElementsToReplace.Add(element);
			}
			return formatString;
		}
		int GetElementPrecision(string str, int elementStartIndex) {
			int precisionStartIndex = elementStartIndex + 3;
			string result = "";
			int i = 0;
			char ch;
			do {
				ch = str[precisionStartIndex + i];
				if (ch != '}') {
					result = result + ch;
					i++;
				}
			} while (ch != '}');
			return Convert.ToInt32(result);
		}
		protected override List<object> GetDefaultElements() {
			return new List<object>() { GeoCoordinateElement.PrecisionDegree, GeoCoordinateElement.CardinalPoint };
		}
		protected override string OnPatternRecognized(Match match) {
			if(match == null || !match.Success)
				return null;
			string result = match.Value;
			result = DecodeSimpleString(result, DegreePattern, "{{{0}}}", "{{{0}}}", GeoCoordinateElement.Degree);
			result = DecodeSimpleString(result, MinutePattern, "{{{0}}}", "{{{0}:00}}", GeoCoordinateElement.Minute);
			result = DecodeSimpleString(result, SecondPattern, "{{{0}}}", "{{{0}:00}}", GeoCoordinateElement.Second);
			result = DecodeSimpleString(result, CardinalPointPattern, "{{{0}}}", "{{{0}}}", GeoCoordinateElement.CardinalPoint);
			result = DecodeSimpleString(result, DegreePattern, "{{{0}:", "{{{0}:F", GeoCoordinateElement.PrecisionDegree);
			result = DecodePrecisionString(result, MinutePattern, GeoCoordinateElement.PrecisionMinute);
			result = DecodePrecisionString(result, SecondPattern, GeoCoordinateElement.PrecisionSecond);
			return result;
		}
		protected override object[] GetParsedObjects() {
			List<object> parsedObjects = new List<object>();
			foreach(GeoCoordinateElement element in ElementsToReplace) {
				switch(element) {
					case GeoCoordinateElement.Degree:
						parsedObjects.Add(Math.Round(MathUtils.GetDegrees(Math.Abs(Coordinate), 0)));
						break;
					case GeoCoordinateElement.Minute:
						parsedObjects.Add(Math.Round(MathUtils.GetMinutes(Math.Abs(Coordinate), 0)));
						break;
					case GeoCoordinateElement.Second:
						parsedObjects.Add(Math.Round(MathUtils.GetSeconds(Math.Abs(Coordinate), 0)));
						break;
					case GeoCoordinateElement.CardinalPoint:
						parsedObjects.Add(CalculateDirectionSign(Coordinate));
						break;
					case GeoCoordinateElement.PrecisionDegree:
						parsedObjects.Add(Math.Abs(Coordinate));
						break;
					case GeoCoordinateElement.PrecisionMinute:
						parsedObjects.Add(MathUtils.GetMinutes(Math.Abs(Coordinate)));
						break;
					case GeoCoordinateElement.PrecisionSecond:
						parsedObjects.Add(MathUtils.GetSeconds(Math.Abs(Coordinate)));
						break;
				}
			}
			return parsedObjects.ToArray();
		}
	}
}
