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

using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraMap.Drawing {
	public abstract class CoordinateViewInfo : TextElementViewInfoBase {
		static Rectangle CalculateTextRect(Graphics gr, TextElementStyle textStyle, string text, Rectangle availableBounds) {
			if(string.IsNullOrEmpty(text) || availableBounds.Width == 0 || availableBounds.Height == 0)
				return Rectangle.Empty;
			Size textSize = MapUtils.CalcStringPixelSize(gr, text, textStyle.Font, availableBounds.Width);
			Rectangle textRect = new Rectangle(Point.Empty, textSize);
			return RectUtils.AlignRectangle(textRect, availableBounds, ContentAlignment.MiddleRight);
		}
		CoordinatePatternFormatterBase formatter;
		double coordinate;
		string pattern;
		string text;
		public string Text { get { return text; } }
		public double Coordinate {
			get { return coordinate; }
			set {
				if(coordinate == value)
					return;
				coordinate = value;
				UpdateText();
			}
		}
		public override int RightMargin { get { return NavigationPanelViewInfo.ItemPadding; } }
		protected internal string Pattern {
			get { return pattern; }
			set {
				if (string.Equals(pattern, value))
					return;
				pattern = value;
				formatter = CreateCoordinatePatternParser();
			}
		}
		protected CoordinatePatternFormatterBase Formatter { get { return formatter; } }
		protected CoordinateViewInfo(InnerMap map, string pattern)
			: base(map) {
			Pattern = pattern;
			UpdateText();
		}
		void UpdateText() {
			this.text = Formatter.Format(Coordinate);
		}
		protected abstract CoordinatePatternFormatterBase CreateCoordinatePatternParser();
		protected override void CalcClientBounds(Graphics gr, TextElementStyle textStyle, Rectangle availableBounds) {
			ClientBounds = CalculateTextRect(gr, textStyle, Text, availableBounds);
		}
	}
	public class LatitudeViewInfo : CoordinateViewInfo {
		public LatitudeViewInfo(InnerMap map, string pattern)
			: base(map, pattern) {
		}
		protected override CoordinatePatternFormatterBase CreateCoordinatePatternParser() {
			return new GeoCoordinatePatternFormatter(Pattern, CardinalDirection.NorthSouth);
		}
	}
	public class LongitudeViewInfo : CoordinateViewInfo {
		public LongitudeViewInfo(InnerMap map, string pattern)
			: base(map, pattern) {
		}
		protected override CoordinatePatternFormatterBase CreateCoordinatePatternParser() {
			return new GeoCoordinatePatternFormatter(Pattern, CardinalDirection.WestEast);
		}
	}
	public class CartesianCoordinateViewInfo : CoordinateViewInfo {
		CartesianMapCoordinateSystem CoordSystem { get { return (CartesianMapCoordinateSystem)Map.CoordinateSystem; } }
		public CartesianCoordinateViewInfo(InnerMap map, string pattern)
			: base(map, pattern) {
		}
		protected override CoordinatePatternFormatterBase CreateCoordinatePatternParser() {
			return new CartesianCoordinatePatternFormatter(Pattern, CoordSystem.MeasureUnit);
		}
	}
}
