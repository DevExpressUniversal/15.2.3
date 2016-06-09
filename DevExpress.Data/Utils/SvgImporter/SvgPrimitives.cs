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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
namespace DevExpress.Data.Svg {
	public struct SvgRect  {
		readonly static SvgRect empty = new SvgRect(double.NaN, double.NaN, double.NaN, double.NaN);
		double x1;
		double y1;
		double x2;
		double y2;
		public double X1 { get { return x1; } }
		public double X2 { get { return x2; } }
		public double Y1 { get { return y1; } }
		public double Y2 { get { return y2; } }
		public double Width { get { return X2 - X1; } }
		public double Height { get { return Y2 - Y1; } }
		public bool IsEmpty { get { return double.IsNaN(x1) && double.IsNaN(y1) && double.IsNaN(x2) && double.IsNaN(y2); } }
		public SvgRect(double x1, double y1, double x2, double y2) {
			this.x1 = Math.Min(x1, x2);
			this.y1 = Math.Min(y1, y2);
			this.x2 = Math.Max(x1, x2);
			this.y2 = Math.Max(y1, y2);
		}
		public static SvgRect Empty { get { return empty; } }
		public static SvgRect Union(SvgRect bounds, double x, double y) {
			SvgRect additionalBounds = new SvgRect(x, y, x, y);
			if (bounds.IsEmpty)
				return additionalBounds;
			return Union(bounds, additionalBounds);
		}
		public static SvgRect Union(SvgRect rect1, SvgRect rect2) {
			if(rect1.IsEmpty)
				return rect2;
			if(rect2.IsEmpty)
				return rect1;
			double x1 = Math.Min(rect1.X1, rect2.X1);
			double y1 = Math.Min(rect1.Y1, rect2.Y1);
			double x2 = Math.Max(rect1.X2, rect2.X2);
			double y2 = Math.Max(rect1.Y2, rect2.Y2);
			return new SvgRect(x1, y1, x2, y2);
		}
	}
	public struct SvgSize {
		double width;
		double height;
		public bool IsEmpty { get { return width == 0.0 && height == 0.0; } }
		public double Width { get { return width; } set { width = value; } }
		public double Height { get { return height; } set { height = value; } }
		public SvgSize(double width, double height) {
			this.width = width;
			this.height = height;
		}
	}
	public struct SvgPoint {
		public static readonly SvgPoint Empty = new SvgPoint();
		public static bool operator ==(SvgPoint Point1, SvgPoint Point2) {
			return Point1.X == Point2.X && Point1.Y == Point2.Y;
		}
		public static bool operator !=(SvgPoint Point1, SvgPoint Point2) {
			return !(Point1 == Point2);
		}
		public static SvgPoint operator +(SvgPoint Point1, SvgPoint Point2) {
			return new SvgPoint(Point1.X + Point2.X, Point1.Y + Point2.Y);
		}
		public static SvgPoint operator -(SvgPoint Point1, SvgPoint Point2) {
			return new SvgPoint(Point1.X - Point2.X, Point1.Y - Point2.Y);
		}
		public static bool Equals(SvgPoint Point1, SvgPoint Point2) {
			return Point1.X.Equals(Point2.X) && Point1.Y.Equals(Point2.Y);
		}
		double x;
		double y;
		public bool IsEmpty { get { return x == 0.0 && y == 0.0; } }
		public double X { get { return x; } set { x = value; } }
		public double Y { get { return y; } set { y = value; } }
		public SvgPoint(double x, double y) {
			this.x = x;
			this.y = y;
		}
		static string GetSeparatorByProvider(IFormatProvider provider) {
			NumberFormatInfo formatInfo = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
			return String.Format("{0} ", (formatInfo == null || formatInfo.NumberDecimalSeparator != ",") ? "," : ";");
		}
		public override bool Equals(object obj) {
			if (obj != null && obj is SvgPoint)
				return SvgPoint.Equals(this, (SvgPoint)obj);
			return false;
		}
		public override int GetHashCode() {
			return Y.GetHashCode() ^ X.GetHashCode();
		}
		public override string ToString() {
			return this.ToString(SvgNumberParser.Culture);
		}
		public string ToString(IFormatProvider provider) {
			return this.ToString(SvgNumberParser.Culture, GetSeparatorByProvider(provider));
		}
		public string ToString(IFormatProvider provider, string pointsSeparator) {
			return String.Format("{0}{1}{2}", this.ToStringX(provider), pointsSeparator, this.ToStringY(provider));
		}
		public string ToStringX() { return this.ToStringX(SvgNumberParser.Culture); }
		public string ToStringY() { return this.ToStringY(SvgNumberParser.Culture); }
		public string ToStringX(IFormatProvider provider) { return X.ToString(provider); }
		public string ToStringY(IFormatProvider provider) { return Y.ToString(provider); }
		public void ParseX(string point) { X = SvgNumberParser.ParseDouble(point); }
		public void ParseY(string point) { Y = SvgNumberParser.ParseDouble(point); }
		public void ParseX(string point, IFormatProvider provider) { X = SvgNumberParser.ParseDouble(point, provider); }
		public void ParseY(string point, IFormatProvider provider) { Y = SvgNumberParser.ParseDouble(point, provider); }
		public static SvgPoint Parse(string pointString, IFormatProvider provider) {
			return Parse(pointString, provider, GetSeparatorByProvider(provider));
		}
		public static SvgPoint Parse(string pointString, IFormatProvider provider, string pointsSeparator) {
			return Parse(pointString, provider, new string[] { pointsSeparator });
		}
		public static SvgPoint Parse(string pointString, IFormatProvider provider, string[] separatorsList) {
			string[] moieties = pointString.Split(separatorsList, StringSplitOptions.RemoveEmptyEntries);
			if (moieties.Length == 2) return Parse(moieties[0].Trim(), moieties[1].Trim(), provider);
			return new SvgPoint();
		}
		public static SvgPoint Parse(string value1, string value2) {
			return Parse(value1, value2, SvgNumberParser.Culture);
		}
		public static SvgPoint Parse(string value1, string value2, IFormatProvider provider) {
			if(String.IsNullOrEmpty(value1) || String.IsNullOrEmpty(value2)) return new SvgPoint();
			return new SvgPoint(SvgNumberParser.ParseDouble(value1, provider), SvgNumberParser.ParseDouble(value2, provider));
		}
	}
	public static class SvgNumberParser {
		static readonly NumberStyles NumberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign;
		public static readonly IFormatProvider Culture = CultureInfo.InvariantCulture;
		public static double ParseDouble(string value) { return ParseDouble(value, Culture); }
		public static double ParseDouble(string value, IFormatProvider provider) { return Double.Parse(value, NumberStyles, provider); }
	}
	public class SvgPointCollection : List<SvgPoint> {
#if DOTNET
		static readonly RegexOptions RegOptions = RegexOptions.None;
#else
		static readonly RegexOptions RegOptions = RegexOptions.Compiled;
#endif
		public IList<SvgPoint> Points { get { return this; } }
		public SvgPointCollection(int capacity)
			: base(capacity) {
		}
		public SvgPointCollection()
			: this(new List<SvgPoint>()) {
		}
		public SvgPointCollection(IList<SvgPoint> svgPoints) {
			base.AddRange(svgPoints);
		}
		internal static string NormalizeSourceString(string sourceString) {
			string normalizedString = Regex.Replace(sourceString, @"\s*,\s*|\s+", " ", RegOptions);
			return normalizedString.Trim();
		}
		public static SvgPointCollection Parse(string pointsString) {
			string normolizedPointsString = NormalizeSourceString(pointsString);
			string[] pointsList = normolizedPointsString.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			return Parse(pointsList);
		}
		public static SvgPointCollection Parse(string[] pointsList) {
			int count = pointsList.Length;
			IList<SvgPoint> parsePoints = new List<SvgPoint>(count);
			for (int i = 0; i < count; i += 2) {
				SvgPoint svgPoint = SvgPoint.Parse(pointsList[i], pointsList[i + 1]);
				parsePoints.Add(svgPoint);
			}
			return new SvgPointCollection(parsePoints);
		}
		public SvgRect GetBoundaryPoints() {
			SvgRect bounds = SvgRect.Empty;
			foreach(SvgPoint point in this)
				bounds = SvgRect.Union(bounds, point.X, point.Y);
			return bounds;
		}
	}
	public enum SvgCommandAction { Start, Motion, Stop }
	public abstract class SvgCommandBase {
		SvgPointCollection points = new SvgPointCollection();
		public SvgPointCollection Points { get { return points; } protected internal set { points = value; } }
		public virtual bool IsRelative { get { return false; } }
		public virtual int ParametersCount { get { return 2; } }
		public virtual int InitialPointsCount { get { return 1; } }
		public virtual SvgCommandAction CommandAction { get { return SvgCommandAction.Motion; } }
		public SvgPoint GeneralPoint {
			get {
				int count = points.Count;
				return points.Count > 0 ? points[count - 1] : SvgPoint.Empty;
			}
		}
		protected virtual SvgPointCollection ParsePoints(string[] commandsElementsList, int index, SvgPoint prevPoint) {
			SvgPointCollection points = new SvgPointCollection();
			for(int i = 0; i < InitialPointsCount; i++) {
				SvgPoint point = SvgPoint.Parse(commandsElementsList[index + i * 2], commandsElementsList[index + i * 2 + 1]);
				points.Add(point);
			}
			return points;
		}
		protected void CaclculateAbsolutePoints(SvgPoint prevPoint) {
			if(IsRelative)
				for(int i = 0; i < points.Count; i++)
					this.points[i] += prevPoint;
		}
		public void FillCommand(string[] commandsElementsList, int index, SvgPoint prevPoint) {
			this.points = ParsePoints(commandsElementsList, index, prevPoint);
			CaclculateAbsolutePoints(prevPoint);
		}
	}
	[FormatElement("M")]
	public class SvgCommandMove : SvgCommandBase {
		public override SvgCommandAction CommandAction { get { return SvgCommandAction.Start; } }
	}
	[FormatElement("m")]
	public class SvgCommandMoveRelative : SvgCommandMove {
		public override bool IsRelative { get { return true; } }
	}
	[FormatElement("L")]
	public class SvgCommandLine : SvgCommandBase {
	}
	[FormatElement("l")]
	public class SvgCommandLineRelative : SvgCommandLine {
		public override bool IsRelative { get { return true; } }
	}
	[FormatElement("T")]
	public class SvgCommandShortQuadraticBezier : SvgCommandBase {
	}
	[FormatElement("t")]
	public class SvgCommandShortQuadraticBezierRelative : SvgCommandShortQuadraticBezier {
		public override bool IsRelative { get { return true; } }
	}
	[FormatElement("Q")]
	public class SvgCommandQuadraticBezier : SvgCommandBase {
		public override int ParametersCount { get { return 4; } }
		public override int InitialPointsCount { get { return 2; } }
	}
	[FormatElement("q")]
	public class SvgCommandQuadraticBezierRelative : SvgCommandQuadraticBezier {
		public override bool IsRelative { get { return true; } }
	}
	[FormatElement("S")]
	public class SvgCommandShortCubicBezier : SvgCommandBase {
		public override int ParametersCount { get { return 4; } }
		public override int InitialPointsCount { get { return 2; } }
	}
	[FormatElement("s")]
	public class SvgCommandShortCubicBezierRelative : SvgCommandShortCubicBezier {
		public override bool IsRelative { get { return true; } }
	}
	[FormatElement("C")]
	public class SvgCommandCubicBezier : SvgCommandBase {
		public override int ParametersCount { get { return 6; } }
		public override int InitialPointsCount { get { return 3; } }
	}
	[FormatElement("c")]
	public class SvgCommandCubicBezierRelative : SvgCommandCubicBezier {
		public override bool IsRelative { get { return true; } }
	}
	[FormatElement("Z")]
	public class SvgCommandClose : SvgCommandBase {
		public override int ParametersCount { get { return 0; } }
		public override int InitialPointsCount { get { return 0; } }
		public override SvgCommandAction CommandAction { get { return SvgCommandAction.Stop; } }
	}
	[FormatElement("z")]
	public class SvgCommandCloseRelative : SvgCommandClose {
		public override bool IsRelative { get { return true; } }
	}
	[FormatElement("V")]
	public class SvgCommandVertical : SvgCommandBase {
		public override int ParametersCount { get { return 1; } }
		protected SvgPointCollection Parse(string[] commandsElementsList, int i, SvgPoint point) {
			point.ParseY(commandsElementsList[i]);
			return new SvgPointCollection() { point };
		}
		protected override SvgPointCollection ParsePoints(string[] commandsElementsList, int i, SvgPoint prevPoint) {
			return Parse(commandsElementsList, i, prevPoint);
		}
	}
	[FormatElement("v")]
	public class SvgCommandVerticalRelative : SvgCommandVertical {
		public override bool IsRelative { get { return true; } }
		protected override SvgPointCollection ParsePoints(string[] commandsElementsList, int i, SvgPoint prevPoint) {
			return Parse(commandsElementsList, i, SvgPoint.Empty);
		}
	}
	[FormatElement("H")]
	public class SvgCommandHorizontal : SvgCommandBase {
		public override int ParametersCount { get { return 1; } }
		protected SvgPointCollection Parse(string[] commandsElementsList, int i, SvgPoint point) {
			point.ParseX(commandsElementsList[i]);
			return new SvgPointCollection() { point };
		}
		protected override SvgPointCollection ParsePoints(string[] commandsElementsList, int i, SvgPoint prevPoint) {
			return Parse(commandsElementsList, i, prevPoint);
		}
	}
	[FormatElement("h")]
	public class SvgCommandHorizontalRelative : SvgCommandHorizontal {
		public override bool IsRelative { get { return true; } }
		protected override SvgPointCollection ParsePoints(string[] commandsElementsList, int i, SvgPoint prevPoint) {
			return Parse(commandsElementsList, i, SvgPoint.Empty);
		}
	}
	[FormatElement("A")]
	public class SvgCommandArc : SvgCommandBase {
		SvgPoint radius;
		double rotation;
		bool isLargeArc;
		bool isSwap;
		public SvgPoint Radius { get { return radius; } protected internal set { radius = value; } }
		public double Rotation { get { return rotation; } protected internal set { rotation = value; } }
		public bool IsLargeArc { get { return isLargeArc; } protected internal set { isLargeArc = value; } }
		public bool IsSwap { get { return isSwap; } protected internal set { isSwap = value; } }
		public override int ParametersCount { get { return 7; } }
		public override int InitialPointsCount { get { return 1; } }
		protected override SvgPointCollection ParsePoints(string[] commandsElementsList, int index, SvgPoint prevPoint) {
			radius = SvgPoint.Parse(commandsElementsList[index], commandsElementsList[index + 1]);
			rotation = SvgNumberParser.ParseDouble(commandsElementsList[index + 2]);
			isLargeArc = (int)SvgNumberParser.ParseDouble(commandsElementsList[index + 3]) != 0;
			isSwap = (int)SvgNumberParser.ParseDouble(commandsElementsList[index + 4]) != 0;
			return new SvgPointCollection() { SvgPoint.Parse(commandsElementsList[index + 5], commandsElementsList[index + 6]) };
		}
	}
	[FormatElement("a")]
	public class SvgCommandArcRelative : SvgCommandArc {
		public override bool IsRelative { get { return true; } }
	}
	public class SvgCommandCollection {
		readonly IList<SvgCommandBase> commands;
		static readonly RegexOptions regOptions ;
		static readonly FormatElementFactory<SvgCommandBase> commandsFactory;
		public IList<SvgCommandBase> Commands { get { return commands; } }
		static SvgCommandCollection() {
#if DOTNET
			regOptions = RegexOptions.None;
#else
			regOptions = RegexOptions.Compiled;
#endif
			commandsFactory = new FormatElementFactory<SvgCommandBase>();
		}
		public SvgCommandCollection()
			: this(new List<SvgCommandBase>()) {
		}
		public SvgCommandCollection(IList<SvgCommandBase> svgCommands) {
			commands = svgCommands;
		}
		static char GetByPrevCommandSymbol(char lastCommandSymbol) {
			switch (lastCommandSymbol) {
				case 'M':
				case 'm': return 'l';
				default: return lastCommandSymbol;
			}
		}
		internal static string NormalizeSourceString(string sourceString) {
			string normolizedString = sourceString;
			normolizedString = Regex.Replace(normolizedString, @"\s*,\s*|\s+", " ", regOptions);
			normolizedString = Regex.Replace(normolizedString, @"(\d)-", m => String.Format("{0} -", m.Groups[1].Value), regOptions); 
			normolizedString = Regex.Replace(normolizedString, @"([a-zA-Z])([a-zA-Z])", m => String.Format("{0} {1}", m.Groups[1].Value, m.Groups[2].Value), regOptions);
			normolizedString = Regex.Replace(normolizedString, @"([a-df-zA-DF-Z]+?)(\d|\.|-)", m => String.Format("{0} {1}", m.Groups[1].Value, m.Groups[2].Value), regOptions);
			normolizedString = Regex.Replace(normolizedString, @"(\d|\.)([a-df-zA-DF-Z]+?)", m => String.Format("{0} {1}", m.Groups[1].Value, m.Groups[2].Value), regOptions);
			return normolizedString.Trim();
		}
		public static SvgCommandCollection Parse(string commandsString) {
			string normolizedCommandsString = NormalizeSourceString(commandsString);
			string[] commandsElements = normolizedCommandsString.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			return Parse(commandsElements);
		}	
		public static SvgCommandCollection Parse(string[] commandsElementsList) {
			int count = commandsElementsList.Length;
			IList<SvgCommandBase> parseCommands = new List<SvgCommandBase>(count);
			SvgPoint lastPoint = SvgPoint.Empty;
			char commandSymbol = ' ';
			int i = 0;
			while (i < count) {
				bool isCurrentSymbolCommand = commandsElementsList[i].Length == 1 && Char.IsLetter(commandsElementsList[i][0]);
				if (isCurrentSymbolCommand) {
					commandSymbol = commandsElementsList[i][0];
					i++;
				} else
					commandSymbol = GetByPrevCommandSymbol(commandSymbol);
				SvgCommandBase command = commandsFactory.CreateInstance(commandSymbol.ToString());
				if (command != null) {
					command.FillCommand(commandsElementsList, i, lastPoint);
					parseCommands.Add(command);
					lastPoint = command.GeneralPoint;
					i += command.ParametersCount;
				} else if (!isCurrentSymbolCommand)
					i++;
			}
			return new SvgCommandCollection(parseCommands);
		}
		public SvgRect GetBoundaryPoints() {
			SvgRect bounds = SvgRect.Empty;
			for (int i = 0; i < commands.Count; i++)
				bounds = SvgRect.Union(bounds, commands[i].Points.GetBoundaryPoints());
			return bounds;
		}
	}
}
