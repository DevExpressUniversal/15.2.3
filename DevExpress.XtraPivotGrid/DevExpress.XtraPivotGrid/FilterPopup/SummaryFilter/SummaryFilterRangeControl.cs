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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraPivotGrid.FilterPopup.SummaryFilter {
	[ToolboxItem(false)]
	class SummaryFilterRangeControl : RangeControl {
		readonly ISummaryFilterController filter;
		Font boldFont;
		Type dataType;
		bool areEventsLocked;
		bool isDiscrete;
		SummaryFilterRangeControl() { }
		public SummaryFilterRangeControl(ISummaryFilterController filter) {
			this.filter = filter;
			this.dataType = typeof(decimal);
			this.isDiscrete = false;
			Client = new SummaryFilterRangeControlClient((RangeControlClientViewInfo)RangeViewInfo) { RangeControl = this };
		}
		public override Font Font {
			get { return boldFont ?? (boldFont = new Font(base.Font, FontStyle.Bold)); }
			set {
				if(base.Font != value)
				base.Font = value;
				this.boldFont = null;
			}
		}
		public DevExpress.Utils.FormatInfo GetValueFormat(object value) { return Filter.GetFormatInfo(value); }
		public ISummaryFilterController Filter { get { return filter; } }
		public new SummaryFilterRangeControlClient Client { get { return (SummaryFilterRangeControlClient)base.Client; } set { base.Client = value; } }
		public object Minimum { get { return Client.Minimum; } set { Client.Minimum = value; } }
		public object Maximum { get { return Client.Maximum; } set { Client.Maximum = value; } }
		public Color UnfilteredColor { get { return Client.UnfilteredColor; } }
		public Color FilteredColor { get { return Client.FilteredColor; } }
		public Type DataType { get { return dataType; } }
		public bool IsDiscrete { get {return isDiscrete; } }
		void LockEvents() { this.areEventsLocked = true; }
		void UnlockEvents() { this.areEventsLocked = false; }
		bool AreEventsLocked { get { return areEventsLocked; } }
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new RangeControlClientViewInfo(this);
		}
		public void UpdateRange() {
			PivotSummaryInterval interval = Filter.GetSummaryInterval(false);
			Enabled = !interval.IsEmpty;
			if(interval.IsEmpty)
				return;
			Minimum = interval.StartValue;
			Maximum = interval.EndValue;
			LockEvents();
			try {
				SelectedRange.Maximum = Filter.EndValue ?? Maximum;
				SelectedRange.Minimum = Filter.StartValue ?? Minimum;
				if(Convert.ToDecimal(SelectedRange.Maximum) > Convert.ToDecimal(Maximum))
					SelectedRange.Maximum = Maximum;
				if(Convert.ToDecimal(SelectedRange.Minimum) < Convert.ToDecimal(Minimum))
					SelectedRange.Minimum = Minimum;
			} finally {
				UnlockEvents();
			}
			dataType = interval.DataType;
			isDiscrete = interval.IsDiscrete;
			PivotSummaryInterval visibleInterval = Filter.GetSummaryInterval(true);
			Client.UnfilteredDistribution = interval.Distribution;
			Client.FilteredDistribution = visibleInterval.Distribution ?? new int[interval.Distribution.Count];
		}
		protected override void RaiseRangeChanged() {
			if(AreEventsLocked) return;
			base.RaiseRangeChanged();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(boldFont != null) {
					boldFont.Dispose();
					boldFont = null;
				}
			}
			base.Dispose(disposing);
		}
	}
	class SummaryFilterRangeControlClient : NumericRangeControlClient {
		readonly RangeControlClientViewInfo viewInfo;
		Color? unfilteredColor;
		Color? filteredColor;
		IList<int> unfilteredDistribution;
		IList<int> filteredDistribution;
		public SummaryFilterRangeControlClient(RangeControlClientViewInfo viewInfo) {
			this.viewInfo = viewInfo;
		}
		protected RangeControlClientViewInfo ViewInfo { get { return viewInfo; } }
		public IList<int> UnfilteredDistribution {
			get { return unfilteredDistribution; }
			set { unfilteredDistribution = value; }
		}
		public IList<int> FilteredDistribution {
			get { return filteredDistribution; }
			set { filteredDistribution = value; }
		}
		public Color UnfilteredColor { get { return (Color)(unfilteredColor ?? (unfilteredColor = GetUnfilteredColor())); } }
		public Color FilteredColor { get { return (Color)(filteredColor ?? (filteredColor = GetFilteredColor())); } }
		protected Color GetUnfilteredColor() {
			Color color = FilteredColor;
			const double opacity = 0.35;
			return Color.FromArgb((int)(color.A * opacity), color.R, color.G, color.B);
		}
		protected Color GetFilteredColor() {
			return CommonSkins.GetSkin(RangeControl.LookAndFeel).Colors.GetColor("Question");
		}
		protected override bool IsValidTypeCore(Type type) {
			return base.IsValidTypeCore(type) || typeof(IPivotCustomSummaryValue).IsAssignableFrom(type);
		}
		protected override object ChangeType(double value) {
			IPivotCustomSummaryValue customSummaryValue = Minimum as IPivotCustomSummaryValue;
			if(customSummaryValue == null) {
				return base.ChangeType(value);
			} else {
				return customSummaryValue.FromDouble(value);
			}
		}
		#region Drawing
		protected override void DrawContentCore(RangeControlPaintEventArgs e) {
			base.DrawContentCore(e);
			if(!RangeControl.Enabled) return;
			viewInfo.CalculateHistogram(UnfilteredDistribution, FilteredDistribution);
			using(Brush brush = new SolidBrush(UnfilteredColor))
				DrawDistributionHistogram(viewInfo.UnfilteredBars, brush, e);
			using(Brush brush = new SolidBrush(FilteredColor))
				DrawDistributionHistogram(viewInfo.FilteredBars, brush, e);
		}
		protected override bool DrawRulerCore(RangeControlPaintEventArgs e) {
			DrawRulerUpperLine(e);
			if(!RangeControl.Enabled) return true;
			ViewInfo.CalculateRuler(GetValueCore(e.ActualRangeMinimum), GetValueCore(e.ActualRangeMaximum), GetValueCore(e.RangeControl.VisibleRangeStartPosition), GetValueCore(e.RangeControl.VisibleRangeStartPosition + e.RangeControl.VisibleRangeWidth));
			Point[] maxUnfilteredCountLinePoints = ViewInfo.MaxUnfilteredCountLinePoints;
			Point[] maxFilteredCountLinePoints = ViewInfo.MaxFilteredCountLinePoints;
			if(ViewInfo.IsSingleMaxValueCountLine)
				DrawDistributionHistogramInfoLine(e.Cache, e.LabelColor, maxUnfilteredCountLinePoints, false);
			else {
				DrawDistributionHistogramInfoLine(e.Cache, e.LabelColor, maxUnfilteredCountLinePoints, false);
				DrawDistributionHistogramInfoLine(e.Cache, FilteredColor, maxFilteredCountLinePoints, true);
			}
			DrawRuler(e);
			return true;
		}
		void DrawDistributionHistogramInfoLine(IGraphicsCache cache, Color color, Point[] points, bool filtered) {
			using(Brush infoLineBrush = new SolidBrush(color))
			using(Pen infoLinePen = GetInfoLinePen(color)) {
				cache.Graphics.DrawLine(infoLinePen, points[0], points[1]);
				StringViewInfo stringViewInfo = filtered ? ViewInfo.MaxFilteredCountStringViewInfo : ViewInfo.MaxUnfilteredCountStringViewInfo;
				if(stringViewInfo == null) return;
				cache.DrawString(stringViewInfo.Value, stringViewInfo.Font, infoLineBrush, stringViewInfo.GetBounds(), StringFormat.GenericDefault);
			}
		}
		Pen GetInfoLinePen(Color color) {
			return new Pen(color, 1.0f) { DashPattern = new float[] { 3.0f, 3.0f } };
		}
		private void DrawRulerUpperLine(RangeControlPaintEventArgs e) {
			using(Pen rulerLinePen = new Pen(e.BorderColor))
				e.Graphics.DrawLine(rulerLinePen, ViewInfo.Bounds.Left, ViewInfo.RulerLineTop, ViewInfo.Bounds.Right, ViewInfo.RulerLineTop);
		}
		void DrawRuler(RangeControlPaintEventArgs e) {
			using(Brush labelBrush = new SolidBrush(e.LabelColor))
			using(Pen rulerLinePen = new Pen(e.BorderColor))
			using(Brush filteredBrush = new SolidBrush(FilteredColor))
			using(Font boldFont = new Font(RangeControl.Font, FontStyle.Bold)) {
				foreach(RangeControlRulerValueViewInfo rulerValue in ViewInfo.VisibleRulerValues)
					e.Cache.DrawString(rulerValue.Value, boldFont, rulerValue.IsRequired ? filteredBrush : labelBrush, rulerValue.Bounds, StringFormat.GenericDefault); 
			}
		}
		static void DrawDistributionHistogram(IList<Rectangle> bars, Brush histogramBrush, RangeControlPaintEventArgs e) {
			if(bars.Count == 0) return;
			int start = Math.Max(0, (int)(e.RangeControl.VisibleRangeStartPosition * bars.Count) - 2);
			int end = Math.Min(bars.Count, start + ((int)(e.RangeControl.VisibleRangeWidth * bars.Count)) + 2);
			for(int i = start; i < end; i++)
				e.Graphics.FillRectangle(histogramBrush, bars[i]);
		}
		#endregion
	}
}
