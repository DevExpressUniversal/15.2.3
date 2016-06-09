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
using DevExpress.DashboardCommon.Data;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Win.Gauges.Linear;
namespace DevExpress.DashboardCommon.Viewer {
	public class LinearGaugeGenerator : GaugeGenerator {
		bool IsVertical { get { return ViewType == GaugeViewType.LinearVertical; } }
		public override int GaugeMinWidth { get { return GetGaugeModelSize(false).Width; } }
		public LinearGaugeGenerator(GaugeViewType viewType, GaugeTheme theme, DeltaColorsGetter deltaColorsGetter)
			: base(viewType, theme, deltaColorsGetter) {
		}
		public override IGauge CreateGauge() {
			IGauge gauge = base.CreateGauge();
			LinearGauge linearGauge = gauge as LinearGauge;
			if(linearGauge != null && linearGauge.Scales.Count > 0 && linearGauge.Levels.Count > 0) {
				KeepOneListItem(linearGauge.Scales);
				LinearScaleComponent scale = linearGauge.Scales[0];
				if(ViewType == GaugeViewType.LinearVertical)
					linearGauge.AutoSize = Utils.DefaultBoolean.False;
				int height = ViewType == GaugeViewType.LinearHorizontal ? GetGaugeModelSize(false).Height : GetGaugeModelSize(false).Width;
				scale.StartPoint = new PointF2D(height / 2 - 3, scale.StartPoint.Y);
				scale.EndPoint = new PointF2D(scale.StartPoint.X, scale.EndPoint.Y);
				linearGauge.AutoSize = Utils.DefaultBoolean.False;
			}
			return gauge;
		}
		protected override void ApplyGaugeSettingsCore(IGauge gauge, GaugeModel model) {
			GaugeRangeModel range = model.Range;
			LinearGauge linearGauge = gauge as LinearGauge;
			if(linearGauge != null && !IsGaugeUpdated(linearGauge, range, model.Value, model.Target, model.SeriesLabel, model.ValueLabel, model.DeltaIndicatorType, model.DeltaIsGood)) {
				if(Theme == GaugeTheme.FlatDark && DashboardGaugeForeColor != Color.Empty) {
					linearGauge.Scales[0].Shader = new FlatBackgroundShader(Color.FromArgb(77, 77, 77), DashboardGaugeForeColor);
				}
				ApplyRangeSettings<LinearScaleComponent>(linearGauge.Scales[0], range);
				SetValue(linearGauge, CoerceValue(range, model.Value));
				if(model.Target == null) {
					if(linearGauge.Markers.Count != 0)
						linearGauge.Markers.Clear();
				}
				else {
					if(linearGauge.Markers.Count == 0) {
						linearGauge.AddMarker();
						Color fore = ((SolidBrushObject)linearGauge.Scales[0].AppearanceTickmarkText.TextBrush).Color;
						linearGauge.Markers[0].Shader = new StyleShader() { StyleColor1 = fore, StyleColor2 = fore };
					}
					linearGauge.Markers[0].Value = CoerceValue(range, (float)model.Target);
				}
			}
		}
		protected override float GetValueLimit(float range) {
			return range / 60;
		}
		public override Size GetGaugeModelSize(bool includeLabelSize) {
			int width = IsVertical ? 130 : 250;
			int height = IsVertical ? 250 : 130;
			if(includeLabelSize) {
				int padding = (int)(DashboardGaugeControlViewer.DefaultBorderProportion * width);
				width += padding;
				height += padding + (IsVertical ? (SeriesLabelHeight + ValueLabelHeight) : Math.Max(SeriesLabelHeight, ValueLabelHeight));
				if(!IsVertical && (Theme == GaugeTheme.FlatDark || Theme == GaugeTheme.FlatLight))
					height -= 65;
			}
			return new Size(width, height);
		}
		protected override IDiscreteScale GetScale(IGauge gauge) {
			return ((LinearGauge)gauge).Scales[0];
		}
		bool IsGaugeUpdated(LinearGauge linearGauge, GaugeRangeModel range, float value, float? target, string[] topCaption, string[] bottomCaption, IndicatorType deltaIndicatorType, bool deltaIsGood) {
			int count = target == null ? 0 : 1;
			return GetValue(linearGauge) == value && linearGauge.Markers.Count == count && (count == 0 || linearGauge.Markers[0].Value == target) &&
				linearGauge.Scales[0].MinValue == range.MinRangeValue && linearGauge.Scales[0].MaxValue == range.MaxRangeValue &&
				IsLabelsUpdated(linearGauge, GetLabelCaption(topCaption, true), GetLabelCaption(bottomCaption, false, deltaIndicatorType, deltaIsGood));
		}
		float? GetValue(LinearGauge gauge) {
			return gauge.Levels.Count > 0 ? gauge.Levels[0].Value : gauge.RangeBars[0].Value;
		}
		void SetValue(LinearGauge gauge, float value) {
			if(gauge.Levels.Count > 0)
				gauge.Levels[0].Value = value;
			else
				gauge.RangeBars[0].Value = value;
		}
		public override int GetTopPadding() {
			return IsVertical ? base.GetTopPadding() : 0;
		}
		protected override StringAlignment GetLabelHorizontalTextAlign(bool isTop) {
			return IsVertical || isTop ? StringAlignment.Near : StringAlignment.Far;
		}
		protected override XtraGauges.Core.Base.PointF2D GetLabelPosition(bool isTop, Size modelSize, float height) {
			XtraGauges.Core.Base.PointF2D point;
			if(IsVertical) {
				point = base.GetLabelPosition(isTop, modelSize, height);
				if(Theme == GaugeTheme.FlatLight || Theme == GaugeTheme.FlatDark)
					if(isTop)
						point.Y = point.Y + 15;
					else
						point.Y = point.Y - 15;				
				return point;
			}
			else {
				point = new XtraGauges.Core.Base.PointF2D(isTop ? (modelSize.Width / 4) : (modelSize.Width * 3 / 4), 
																						  base.GetLabelPosition(false, modelSize, height).Y);
				if(Theme == GaugeTheme.FlatLight || Theme == GaugeTheme.FlatDark)
				if(isTop)
						point.X += 10.5f;
				else
						point.X -= 10.5f;
				return point;
			}
		}
		protected override SizeF GetLabelSize(bool isTop) {
			SizeF size = base.GetLabelSize(isTop);
			size = IsVertical ? size : new SizeF(size.Width / 2, size.Height);
			if(!IsVertical && (Theme == GaugeTheme.FlatDark || Theme == GaugeTheme.FlatLight))
				if(isTop)
					size.Width = size.Width - 10.5f;
				else
					size.Width = size.Width - 6f;
			return size;
		}
		protected override int GetGaugeHeightDiff() {
			if(Theme != GaugeTheme.FlatDark && Theme != GaugeTheme.FlatLight || IsVertical)
				return base.GetGaugeHeightDiff();
			else
				return 55;
		}
	}
}
