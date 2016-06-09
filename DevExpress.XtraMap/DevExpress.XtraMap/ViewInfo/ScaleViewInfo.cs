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
using System.Linq;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Drawing {
	public enum ScaleViewType {
		None,
		Meters,
		Miles,
		All
	}
	public class ScaleViewInfo : TextElementViewInfoBase {
		static int LineTopHeight = (int)(Math.Ceiling(ScaleLineHeight / 2.0));
		static int LineBottomHeight = (int)(Math.Floor(ScaleLineHeight / 2.0));
		static double[] kilometersScaleRanges = new double[] { 10000.0, 5000.0, 2500.0, 1000.0, 500.0, 250.0, 200.0, 100.0, 50.0, 25.0, 20.0, 10.0, 5.0, 2.5, 1.0, 0.5, 0.25, 0.2, 0.1, 0.05, 0.025, 0.010, 0.005, 0.002, 0.001, 0.0005, 0.0002, 0.0001, 0.00005, 0.00001, 0.000005, 0.000001 };
		static double[] milesScaleRanges = new double[] { 10000.0, 5000.0, 2500.0, 1000.0, 500.0, 250.0, 200.0, 100.0, 50.0, 25.0, 20.0, 10.0, 5.0, 2.5, 1.0, 0.5, 0.25, 0.2, 0.1, 0.05, 0.025, 0.010, 0.005 };
		static double[] footsScaleRanges = new double[] { 5000.0, 2500.0, 1000.0, 500.0, 250.0, 200.0, 100.0, 50.0, 25.0, 20.0, 10.0, 5.0, 2.0, 1.0 };
		static double[] inchesScaleRanges = new double[] { 6.0, 3.0, 2.0, 1.0 };
		const int ScaleLineHeight = 3;
		const int TextLinePadding = 3;
		public const int DefaultMaxScaleLineWidth = 130;
		Rectangle metersBounds;
		Rectangle milesBounds;
		TextElementStyle scaleInfoStyle;
		double kilometersScale;
		int maxScaleLineWidth = DefaultMaxScaleLineWidth;
		internal Rectangle MetersBounds { get { return metersBounds; } }
		internal Rectangle MilesBounds { get { return milesBounds; } }
		internal Rectangle LineRect { get; set; }
		internal Rectangle MilesRightLineRect { get; set; }
		internal Rectangle MetersRightLineRect { get; set; }
		internal Rectangle MilesLeftLineRect { get; set; }
		internal Rectangle MetersLeftLineRect { get; set; }
		internal string MetersText { get; set; }
		internal string MilesText { get; set; }
		internal Rectangle MetersTextRect { get; set; }
		internal Rectangle MilesTextRect { get; set; }
		int TextHeight { get { return scaleInfoStyle.Font.Height; } }
		internal int MaxScaleLineWidth { get { return maxScaleLineWidth; } }
		internal int KilometersScaleLineWidth { get; set; }
		internal int MilesScaleLineWidth { get; set; }
		public override int DesiredWidth { get { return MaxScaleLineWidth; } }
		public override int RightMargin { get { return NavigationPanelViewInfo.ItemPadding; } }
		public double KilometersScale {
			get { return kilometersScale; }
			set {
				if (kilometersScale == value)
					return;
				kilometersScale = value;
				CalculateScaleLineWidth(kilometersScale);
			}
		}
		public ScaleViewType ViewType { get; set; }
		public ScaleViewInfo(InnerMap map, TextElementStyle scaleInfoStyle)
			: base(map) {
				this.scaleInfoStyle = scaleInfoStyle;
		}
		int GetLineWidth() {
			switch (ViewType) {
				case ScaleViewType.Meters:
					return metersBounds.Width;
				case ScaleViewType.Miles:
					return milesBounds.Width;
				default:
					return Math.Max(metersBounds.Width, milesBounds.Width);
			}
		}
		void CalculateLine(ref Rectangle availableMeterBounds, ref Rectangle availableMilesBounds) {
			int lineWidth = GetLineWidth();
			LineRect = new Rectangle(0, 0, lineWidth, ScaleLineHeight);
			if (ViewType == ScaleViewType.All) {
				LineRect = RectUtils.AlignRectangle(LineRect, ClientBounds, ContentAlignment.MiddleLeft);
				availableMeterBounds = RectUtils.CutFromBottom(metersBounds, LineTopHeight);
				availableMilesBounds = RectUtils.CutFromTop(milesBounds, LineBottomHeight);
			}
			else {
				LineRect = RectUtils.AlignRectangle(LineRect, availableMeterBounds, ContentAlignment.BottomLeft);
				availableMeterBounds = RectUtils.CutFromBottom(availableMeterBounds, ScaleLineHeight);
				availableMilesBounds = RectUtils.CutFromBottom(availableMilesBounds, ScaleLineHeight);
			}
			int dLW = (int)(ScaleLineHeight * 0.75);
			Rectangle sideLineRect = new Rectangle(0, 0, dLW, ScaleLineHeight * 2);
			MetersLeftLineRect = RectUtils.AlignRectangle(sideLineRect, availableMeterBounds, ContentAlignment.BottomLeft);
			MetersRightLineRect = RectUtils.AlignRectangle(sideLineRect, availableMeterBounds, ContentAlignment.BottomRight);
			MilesLeftLineRect = RectUtils.AlignRectangle(sideLineRect, availableMilesBounds, ViewType == ScaleViewType.All ? ContentAlignment.TopLeft : ContentAlignment.BottomLeft);
			MilesRightLineRect = RectUtils.AlignRectangle(sideLineRect, availableMilesBounds, ViewType == ScaleViewType.All ? ContentAlignment.TopRight : ContentAlignment.BottomRight);
		}
		void CalculateTextRects(Graphics gr, Rectangle availableMeterBounds, Rectangle availableMilesBounds) {
			int metersTextWidth = MapUtils.CalcStringPixelSize(gr, MetersText, scaleInfoStyle.Font, MetersBounds.Width).Width;
			int milesTextWidth = MapUtils.CalcStringPixelSize(gr, MilesText, scaleInfoStyle.Font, MilesBounds.Width).Width;
			availableMeterBounds = RectUtils.CutFromBottom(availableMeterBounds, TextLinePadding);
			MetersTextRect = new Rectangle(0, 0, metersTextWidth, TextHeight);
			MetersTextRect = RectUtils.AlignRectangle(MetersTextRect, availableMeterBounds, ContentAlignment.BottomCenter);
			if (ViewType == ScaleViewType.All)
				availableMilesBounds = RectUtils.CutFromTop(availableMilesBounds, TextLinePadding - 1);
			else
				availableMilesBounds = RectUtils.CutFromBottom(availableMilesBounds, TextLinePadding);
			MilesTextRect = new Rectangle(0, 0, milesTextWidth, TextHeight);
			MilesTextRect = RectUtils.AlignRectangle(MilesTextRect, availableMilesBounds, ContentAlignment.TopCenter);
		}
		void CalculateMetersMilesBounds(out Rectangle availableMeterBounds, out Rectangle availableMilesBounds) {
			int halfBoundsHeight = ClientBounds.Height / 2;
			milesBounds = new Rectangle(0, 0, MilesScaleLineWidth, halfBoundsHeight);
			metersBounds = new Rectangle(0, 0, KilometersScaleLineWidth, halfBoundsHeight);
			if (ViewType == ScaleViewType.All) {
				availableMeterBounds = metersBounds = RectUtils.AlignRectangle(metersBounds, ClientBounds, ContentAlignment.TopLeft);
				availableMilesBounds = milesBounds = RectUtils.AlignRectangle(milesBounds, ClientBounds, ContentAlignment.BottomLeft);
			}
			else {
				int wholeRectHeight = TextHeight + TextLinePadding + ScaleLineHeight;
				metersBounds = RectUtils.AlignRectangle(metersBounds, ClientBounds, ContentAlignment.MiddleLeft);
				milesBounds = RectUtils.AlignRectangle(milesBounds, ClientBounds, ContentAlignment.MiddleLeft);
				availableMeterBounds = new Rectangle(0, 0, KilometersScaleLineWidth, wholeRectHeight);
				availableMilesBounds = new Rectangle(0, 0, MilesScaleLineWidth, wholeRectHeight);
				availableMeterBounds = RectUtils.AlignRectangle(availableMeterBounds, metersBounds, ContentAlignment.MiddleLeft);
				availableMilesBounds = RectUtils.AlignRectangle(availableMilesBounds, milesBounds, ContentAlignment.MiddleLeft);
			}
		}
		string CalculateMetersText(double scaleRange) {
			if(scaleRange >= 1)
				return scaleRange.ToString() + " km";
			return scaleRange >= 0.001 ? (scaleRange * 1000.0).ToString() + " m" : (scaleRange * 1000000.0).ToString() + " mm";
		}
		void CalculateScaleLineWidth(double kilometersScale) {
			CalculateMetersScaleLineWidth(kilometersScale);
			CalculateMilesScaleLineWidth(kilometersScale * 0.621371192);
		}
		void CalculateMetersScaleLineWidth(double kilometersScale) {
			foreach (double scaleRange in kilometersScaleRanges)
				if (scaleRange <= kilometersScale) {
					UpdateKilometersScale((int)(scaleRange / kilometersScale * MaxScaleLineWidth), CalculateMetersText(scaleRange));
					return;
				}
			UpdateKilometersScale(MaxScaleLineWidth, "0 mm");
		}
		void UpdateKilometersScale(int scaleWidth, string scaleText) {
			KilometersScaleLineWidth = scaleWidth;
			MetersText = scaleText;
		}
		void CalculateMilesScaleLineWidth(double milesScale) {
			if (milesScale >= 1) {
				foreach (double scaleRange in milesScaleRanges)
					if (scaleRange <= milesScale) {
						UpdateMilesScale((int)(scaleRange / milesScale * MaxScaleLineWidth), scaleRange.ToString() + " mi");
						return;
					}
			} else if (milesScale >= 1.0 / 5280.0) {
				double footsScale = milesScale * 5280.0;
				foreach (double scaleRange in footsScaleRanges)
					if (scaleRange <= footsScale) {
						UpdateMilesScale((int)(scaleRange / footsScale * MaxScaleLineWidth), scaleRange.ToString() + " ft");
						return;
					}
			} else {
				double inchScale = milesScale * 5280.0 * 12.0;
				foreach (double scaleRange in inchesScaleRanges)
					if (scaleRange <= inchScale) {
						UpdateMilesScale((int)(scaleRange / inchScale * MaxScaleLineWidth), scaleRange.ToString() + " in");
						return;
					}
			}
			UpdateMilesScale(MaxScaleLineWidth, "0 in");
		}
		void UpdateMilesScale(int scaleWidth, string scaleText) {
			MilesScaleLineWidth = scaleWidth;
			MilesText = scaleText;
		}
		protected override void CalcClientBounds(Graphics gr, TextElementStyle textStyle, Rectangle availableBounds) {
			Rectangle clientBounds = new Rectangle(availableBounds.X, availableBounds.Y, MaxScaleLineWidth, availableBounds.Height);
			ClientBounds = RectUtils.AlignRectangle(clientBounds, availableBounds, ContentAlignment.MiddleRight);
			Rectangle availableMeterBounds;
			Rectangle availableMilesBounds;
			CalculateMetersMilesBounds(out availableMeterBounds, out availableMilesBounds);
			CalculateLine(ref availableMeterBounds, ref availableMilesBounds);
			CalculateTextRects(gr, availableMeterBounds, availableMilesBounds);
		}
	}
}
