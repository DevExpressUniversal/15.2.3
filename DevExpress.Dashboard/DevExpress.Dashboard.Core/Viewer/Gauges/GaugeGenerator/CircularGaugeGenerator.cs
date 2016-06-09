#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Win.Gauges.Circular;
using System.Linq;
namespace DevExpress.DashboardCommon.Viewer {
	public abstract class CircularGaugeGenerator : GaugeGenerator {
		public override int GaugeMinWidth { get { return 180; } }
		protected CircularGaugeGenerator(GaugeViewType viewType, GaugeTheme theme, DeltaColorsGetter deltaColorsGetter)
			: base(viewType, theme, deltaColorsGetter) {
		}
		protected override float GetDefaultFontSize(IDiscreteScale scale) {
			return 10;
		}
		public override IGauge CreateGauge() {
			IGauge gauge = base.CreateGauge();
			CircularGauge circularGauge = gauge as CircularGauge;
			if(circularGauge != null && circularGauge.Scales.Count > 0 && circularGauge.Needles.Count > 0) {
				KeepOneListItem(circularGauge.Scales);
				if(ViewType == GaugeViewType.CircularHalf || ViewType == GaugeViewType.CircularThreeFourth)
					circularGauge.Scales[0].Center = new XtraGauges.Core.Base.PointF2D(125, 125);
				circularGauge.AutoSize = Utils.DefaultBoolean.False;
			}
			return gauge;
		}
		protected override void ApplyGaugeSettingsCore(IGauge gauge, GaugeModel model) {
			GaugeRangeModel range = model.Range;
			CircularGauge circularGauge = gauge as CircularGauge;
			if(circularGauge != null && !IsGaugeUpdated(circularGauge, range, model.Value, model.Target, GetLabelCaption(model.SeriesLabel, true), GetLabelCaption(model.ValueLabel, false, model.DeltaIndicatorType, model.DeltaIsGood))) {
				if(Theme == GaugeTheme.CleanWhite && (ViewType == GaugeViewType.CircularFull || ViewType == GaugeViewType.CircularThreeFourth))
					circularGauge.BackgroundLayers[0].Shader = new FlatBackgroundShader(Color.FromArgb(245, 80, 0), Color.FromArgb(86, 86, 86));
				if(Theme == GaugeTheme.FlatLight && DashboardGaugeBackColor != Color.Empty)
					circularGauge.BackgroundLayers[0].Shader = new FlatBackgroundShader(Color.FromArgb(255, 255, 255), DashboardGaugeBackColor);
				ApplyRangeSettings<ArcScaleComponent>(circularGauge.Scales[0], range);
				circularGauge.Needles[0].Value = CoerceValue(range, model.Value);
				if(model.Target == null) {
					if(circularGauge.Markers.Count != 0)
						circularGauge.Markers.Clear();
				} else {
					if(circularGauge.Markers.Count == 0) {
						circularGauge.AddMarker();
						Color fore = ((SolidBrushObject)circularGauge.Scales[0].AppearanceTickmarkText.TextBrush).Color;
						circularGauge.Markers[0].Shader = new StyleShader() { StyleColor1 = fore, StyleColor2 = fore };
					}
					circularGauge.Markers[0].Value = CoerceValue(range, (float)model.Target);
				}
			}
			if(DashboardGaugeForeColor != Color.Empty && Theme == GaugeTheme.FlatDark)
				circularGauge.Needles[0].Shader = new FlatBackgroundShader(Color.FromArgb(110, 110, 110), DashboardGaugeForeColor);
		}
		public override Size GetGaugeModelSize(bool includeLabelSize) {
			int width = 250;
			int height = 250;
			int padding = (int)(DashboardGaugeControlViewer.DefaultBorderProportion * width);
			if(includeLabelSize) {
				height += SeriesLabelHeight + ValueLabelHeight + padding;
				width += padding;
			}
			return new Size(width, height);
		}
		protected override IDiscreteScale GetScale(IGauge gauge) {
			return ((CircularGauge)gauge).Scales[0];
		}
		protected override int GetLargeScaleTextOffset() {
			return 5;
		}
		protected override int GetSmallScaleTextOffset() {
			return 3;
		}
		bool IsGaugeUpdated(CircularGauge circularGauge, GaugeRangeModel range, float value, float? target, string topCaption, string bottomCaption) {
			int count = target == null ? 0 : 1;
			return circularGauge.Needles[0].Value == value && circularGauge.Markers.Count == count && (count != 1 || circularGauge.Markers[0].Value == target) &&
				circularGauge.Scales[0].MinValue == range.MinRangeValue && circularGauge.Scales[0].MaxValue == range.MaxRangeValue &&
				IsLabelsUpdated(circularGauge, topCaption, bottomCaption);
		}
	}
	public abstract class CircularBottomOnlyCaptionGaugeGenerator : CircularGaugeGenerator {
		protected CircularBottomOnlyCaptionGaugeGenerator(GaugeViewType viewType, GaugeTheme theme, DeltaColorsGetter deltaColorsGetter)
			: base(viewType, theme, deltaColorsGetter) {
		}
		public override Size GetGaugeModelSize(bool includeLabelSize) {
			Size fullSize = base.GetGaugeModelSize(includeLabelSize);
			return new Size(fullSize.Width, includeLabelSize ? fullSize.Height - ValueLabelHeight : fullSize.Height);
		}
		public override int GetTopPadding() {
			return 0;
		}
	}
	public abstract class CircularOneBlockCaptionGaugeGenerator : CircularBottomOnlyCaptionGaugeGenerator {
		protected CircularOneBlockCaptionGaugeGenerator(GaugeViewType viewType, GaugeTheme theme, DeltaColorsGetter deltaColorsGetter)
			: base(viewType, theme, deltaColorsGetter) {
		}
		protected override SizeF GetLabelSize(bool isTop) {
			SizeF size = base.GetLabelSize(isTop);
			size = new SizeF(size.Width / 2, size.Height);
			if(Theme == GaugeTheme.FlatLight)
				return new PointF2D(size.Width - (isTop ? 17 : 19), size.Height);
			if(Theme == GaugeTheme.FlatDark)
				return new PointF2D(size.Width - (isTop ? 17 : 19), size.Height);
			return size;
		}
		protected override XtraGauges.Core.Base.PointF2D GetLabelPosition(bool isTop, Size modelSize, float height) {
			PointF2D position = base.GetLabelPosition(false, modelSize, height);
			PointF2D point = new XtraGauges.Core.Base.PointF2D(isTop ? modelSize.Width / 4 : modelSize.Width * 3 / 4, position.Y);
			if(Theme == GaugeTheme.FlatLight)
				return new PointF2D(point.X + (isTop ? 14 : -7), point.Y);
			if(Theme == GaugeTheme.FlatDark)
				return new PointF2D(point.X + (isTop ? 14 : -7), point.Y);
			return point;
		}
		protected override StringAlignment GetLabelHorizontalTextAlign(bool isTop) {
			return isTop ? StringAlignment.Near : StringAlignment.Far;
		}
		public override Size GetGaugeModelSize(bool includeLabelSize) {
			Size size = base.GetGaugeModelSize(includeLabelSize);
			return includeLabelSize ? new Size(size.Width, size.Height - GetGaugeHeightDiff() - SeriesLabelHeight + Math.Max(SeriesLabelHeight, ValueLabelHeight)) : size;
		}
	}
	public class CircularQuarterGaugeGenerator : CircularGaugeGenerator {
		bool IsRight { get { return ViewType == GaugeViewType.CircularQuarterRight; } }
		public CircularQuarterGaugeGenerator(GaugeViewType viewType, GaugeTheme theme, DeltaColorsGetter deltaColorsGetter)
			: base(viewType, theme, deltaColorsGetter) {
		}
		protected override StringAlignment GetLabelHorizontalTextAlign(bool isTop) {
			return IsRight ? StringAlignment.Near : StringAlignment.Far;
		}
		protected override float GetValueLimit(float range) {
			return range / 15;
		}
		protected override int GetLargeScaleTextOffset() {
			return 0;
		}
		protected override int GetSmallScaleTextOffset() {
			return 0;
		}
		protected override int GetGaugeHeightDiff() {
			if(Theme == GaugeTheme.FlatDark || Theme == GaugeTheme.FlatLight)
				return -10;
			return base.GetGaugeHeightDiff();
		}
		protected override SizeF GetLabelSize(bool isTop) {
			SizeF size = base.GetLabelSize(isTop);
			if(Theme == GaugeTheme.FlatDark || Theme == GaugeTheme.FlatLight)
				return new SizeF(Math.Max(0, size.Width - 4), size.Height);
			return size;
		}
		protected override PointF2D GetLabelPosition(bool isTop, Size modelSize, float height) {
			PointF2D point = base.GetLabelPosition(isTop, modelSize, height);
			bool isFlatTheme = Theme == GaugeTheme.FlatDark || Theme == GaugeTheme.FlatLight;
			if(isFlatTheme)
				point.Y += 4;
			if(IsRight && isFlatTheme)
				point.X += 4;
			return point;
		}
	}
	public class CircularFullGaugeGenerator : CircularBottomOnlyCaptionGaugeGenerator {
		public CircularFullGaugeGenerator(GaugeViewType viewType, GaugeTheme theme, DeltaColorsGetter deltaColorsGetter)
			: base(viewType, theme, deltaColorsGetter) {
		}
		protected override XtraGauges.Core.Base.PointF2D GetLabelPosition(bool isTop, Size modelSize, float height) {
			XtraGauges.Core.Base.PointF2D point;
			if(isTop) {
				point = base.GetLabelPosition(!isTop, modelSize, height);
				if(Theme == GaugeTheme.FlatDark || Theme == GaugeTheme.FlatLight)
					point.Y -= 15;
				return point;
			}
			else {
				point = new XtraGauges.Core.Base.PointF2D(modelSize.Width / 2, 137 * modelSize.Height / 200);
				if(Theme == GaugeTheme.FlatDark || Theme == GaugeTheme.FlatLight)
					point.Y -= 5;
				return point;
			}
		}
		protected override float GetValueLimit(float range) {
			return range / 60;
		}
	}
	public class CircularThreeFourGaugeGenerator : CircularOneBlockCaptionGaugeGenerator {
		public CircularThreeFourGaugeGenerator(GaugeViewType viewType, GaugeTheme theme, DeltaColorsGetter deltaColorsGetter)
			: base(viewType, theme, deltaColorsGetter) {
		}
		protected override int GetGaugeHeightDiff() {
			switch(Theme) {
				case GaugeTheme.CleanWhite:
					return 55;
				case GaugeTheme.PureDark:
					return 51;
				case GaugeTheme.White:
					return 48;
				case GaugeTheme.DeepFire:
				case GaugeTheme.SilverBlur:
					return 46;
				case GaugeTheme.AfricaSunset:
				case GaugeTheme.Mechanical:
					return 45;
				case GaugeTheme.DarkNight:
					return 42;
				case GaugeTheme.Military:
					return 39;
				case GaugeTheme.GothicMat:
					return 38;
				case GaugeTheme.IceColdZone:
					return 34;
				case GaugeTheme.ShiningDark:
					return 32;
				case GaugeTheme.SportCar:
					return 27;
				case GaugeTheme.Disco:
					return 0;
				case GaugeTheme.FlatLight:
				case GaugeTheme.FlatDark:
					return 33;
				default:
					return 38;
			}
		}
		protected override float GetValueLimit(float range) {
			return range / 45;
		}
	}
	public class CircularHalfGaugeGenerator : CircularOneBlockCaptionGaugeGenerator {
		public CircularHalfGaugeGenerator(GaugeViewType viewType, GaugeTheme theme, DeltaColorsGetter deltaColorsGetter)
			: base(viewType, theme, deltaColorsGetter) {
		}
		protected override int GetGaugeHeightDiff() {
			switch(Theme) {
				case GaugeTheme.CleanWhite:
				case GaugeTheme.White:
				case GaugeTheme.AfricaSunset:
				case GaugeTheme.PureDark:
				case GaugeTheme.Default:
				case GaugeTheme.Military:
					return 97;
				case GaugeTheme.FlatLight:
				case GaugeTheme.FlatDark:
					return 121;
				case GaugeTheme.DeepFire:
					return 96;
				case GaugeTheme.Retro:
					return 92;
				case GaugeTheme.Disco:
					return 88;
				case GaugeTheme.SilverBlur:
					return 76;
				case GaugeTheme.DarkNight:
					return 75;
				case GaugeTheme.IceColdZone:
					return 74;
				case GaugeTheme.SportCar:
					return 70;
				case GaugeTheme.GothicMat:
					return 69;
				case GaugeTheme.Mechanical:
					return 60;
				case GaugeTheme.ShiningDark:
					return 58;
				default:
					return 80;
			}
		}
		protected override float GetValueLimit(float range) {
			return range / 30;
		}
	}
}
