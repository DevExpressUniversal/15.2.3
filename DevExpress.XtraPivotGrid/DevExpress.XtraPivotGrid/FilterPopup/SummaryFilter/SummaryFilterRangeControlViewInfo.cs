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

using DevExpress.XtraEditors;
using DevExpress.XtraPivotGrid.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
namespace DevExpress.XtraPivotGrid.FilterPopup.SummaryFilter {
	class RangeControlClientViewInfo : RangeControlViewInfo {
		private const int MaxCountStringRightMargin = 3;
		List<Rectangle> unfilteredBars;
		List<Rectangle> filteredBars;
		List<RangeControlRulerValueViewInfo> rulerValues;
		const int maxUnilteredCountTop = 20;
		int maxFilteredCountTop;
		StringViewInfo maxUnfilteredCountStringViewInfo;
		StringViewInfo maxFilteredCountStringViewInfo;
		public RangeControlClientViewInfo(SummaryFilterRangeControl control) : base(control) { }
		public new SummaryFilterRangeControl RangeControl { get { return (SummaryFilterRangeControl)base.RangeControl; } }
		public Font Font { get { return RangeControl.Font; } }
		public FormatInfo GetValueFormat(object value) { return RangeControl.GetValueFormat(value); }
		public IList<Rectangle> UnfilteredBars { get { return unfilteredBars; } }
		public IList<Rectangle> FilteredBars { get { return filteredBars; } }
		public int RulerLineTop { get { return RangeClientBounds.Bottom; } }
		public int MaxUnfilteredCountTop { get { return maxUnilteredCountTop; } }
		public int MaxFilteredCountTop { get { return maxFilteredCountTop; } }
		public Point[] MaxUnfilteredCountLinePoints { get { return GetMaxCountLinePoints(MaxUnfilteredCountTop); } }
		public Point[] MaxFilteredCountLinePoints { get { return MaxUnfilteredCountTop == MaxFilteredCountTop ? new Point[0] : GetMaxCountLinePoints(MaxFilteredCountTop); } }
		public StringViewInfo MaxUnfilteredCountStringViewInfo { get { return maxUnfilteredCountStringViewInfo; } }
		public StringViewInfo MaxFilteredCountStringViewInfo { get { return maxFilteredCountStringViewInfo; } }
		public bool IsSingleMaxValueCountLine { get { return MaxUnfilteredCountTop == MaxFilteredCountTop; } }
		public IEnumerable<RangeControlRulerValueViewInfo> VisibleRulerValues { get { return rulerValues.Where(vi => vi.IsVisible); } }
		Point[] GetMaxCountLinePoints(int top) {
			return new Point[] { new Point(RangeClientBounds.Left, top), new Point(RangeClientBounds.Right, top) };
		}
		public void CalculateHistogram(IList<int> unfilteredDistribution, IList<int> filteredDistribution) {
			Guard.ArgumentNotNull(unfilteredDistribution, "unfilteredDistribution");
			Guard.ArgumentNotNull(filteredDistribution, "filteredDistribution");
			int unfilteredDistributionMax = unfilteredDistribution.Max();
			int filteredDistributionMax = filteredDistribution.Max();
			this.unfilteredBars = CalculateHistogramBars(unfilteredDistribution, unfilteredDistributionMax);
			this.filteredBars = CalculateHistogramBars(filteredDistribution, unfilteredDistributionMax);
			this.maxFilteredCountTop = RangeClientBounds.Height - (FilteredBars.Count == 0 ? 0 : FilteredBars.Max(bar => bar.Height));
			CalculateInfoLineStringViewInfo(filteredDistributionMax, unfilteredDistributionMax);
		}
		void CalculateInfoLineStringViewInfo(int filteredDistributionMax, int unfilteredDistributionMax) {
			this.maxUnfilteredCountStringViewInfo = CalculateStringViewInfo(string.Format("{0} = {1}", PivotGridLocalizer.GetString(PivotGridStringId.SummaryFilterMaxValueCount), unfilteredDistributionMax), MaxUnfilteredCountTop, false);
			if(!IsSingleMaxValueCountLine) {
				bool drawUnderTheLine = MaxFilteredCountTop - MaxUnfilteredCountTop < maxUnfilteredCountStringViewInfo.Size.Height;
				this.maxFilteredCountStringViewInfo = CalculateStringViewInfo(string.Format("{0} = {1}", PivotGridLocalizer.GetString(PivotGridStringId.SummaryFilterMaxVisibleCount), filteredDistributionMax), MaxFilteredCountTop, drawUnderTheLine);
			}
		}
		StringViewInfo CalculateStringViewInfo(string text, int top, bool drawUnderTheLine) {
			StringViewInfo stringViewInfo = new StringViewInfo(Font, text);
			stringViewInfo.Location = new Point(RangeClientBounds.Right - stringViewInfo.Size.Width - MaxCountStringRightMargin, top - (drawUnderTheLine ? 0 : stringViewInfo.Size.Height));
			return stringViewInfo;
		}
		List<Rectangle> CalculateHistogramBars(IList<int> distribution, int distributionMax) {
			List<Rectangle> result = new List<Rectangle>();
			int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
			bool isDiscrete = RangeControl.IsDiscrete;
			for(int i = 0; i < distribution.Count; i++) {
				y1 = RangeClientBounds.Top + MaxUnfilteredCountTop + (int)((RangeClientBounds.Height - MaxUnfilteredCountTop) * (double)(distributionMax - distribution[i]) / distributionMax);
				y2 = RangeClientBounds.Bottom;
				if(isDiscrete && i == distribution.Count - 1) {
					x1 = CalcX(1);
				} else
					x1 = i > 0 ? x2 : CalcX(0);
				x2 = CalcX((double)(i + 1) / distribution.Count);
				if(distribution[i] == 0) continue;
				result.Add(GetBar(x1, y1, x2, y2, isDiscrete));
			}
			return result;
		}
		Rectangle GetBar(int x1, int y1, int x2, int y2, bool isDiscrete) {
			const int LineWidth = 4;
			if(isDiscrete)
				return new Rectangle(x1 - 1, y1, LineWidth, y2 - y1);
			else
				return new Rectangle(x1 + 1, y1, x2 - x1 - 1, y2 - y1);
		}
		int CalcX(double normalizedValue) {
			return ((IRangeControl)RangeControl).CalcX(normalizedValue);
		}
		public void CalculateRuler(object currentRangeMinimum, object currentRangeMaximum, object viewPortMinimum, object viewPortMaximum) {
			int rangeBoundsNear = IsRightToLeft ? RangeBounds.Right : RangeBounds.Left;
			int rangeClientBoundsNear = IsRightToLeft ? RangeClientBounds.Right: RangeClientBounds.Left;
			int rangeBoundsFar =IsRightToLeft ? RangeBounds.Left:  RangeBounds.Right;
			int rangeClientBoundsFar = IsRightToLeft ? RangeClientBounds.Left: RangeClientBounds.Right;
			RangeControlRulerValueViewInfo rangeNear = new RangeControlRulerValueViewInfo(currentRangeMinimum, GetValueFormat, Font, rangeBoundsNear, true);
			RangeControlRulerValueViewInfo rangeFar = new RangeControlRulerValueViewInfo(currentRangeMaximum, GetValueFormat, Font, rangeBoundsFar, true);
			RangeControlRulerValueViewInfo rangeMinimum = new RangeControlRulerValueViewInfo(viewPortMinimum, GetValueFormat, Font, rangeClientBoundsNear, false);
			RangeControlRulerValueViewInfo rangeMaximum = new RangeControlRulerValueViewInfo(viewPortMaximum, GetValueFormat, Font, rangeClientBoundsFar, false);
			RangeControlRulerValueViewInfo rulerLabel = new RangeControlRulerValueViewInfo(PivotGridLocalizer.GetString(PivotGridStringId.SummaryFilterLabelValues),
				 GetValueFormat, Font, RangeClientBounds.Left + RangeClientBounds.Width / 2, false);
			RangeControlRulerValuesArranger arranger = new RangeControlRulerValuesArranger(RulerBounds);
			arranger.Add(rangeNear, rangeFar, rangeMinimum, rangeMaximum, rulerLabel);
			arranger.Arrange();
			this.rulerValues = arranger.ToList();
		}
	}
	class StringViewInfo {
		Font font;
		string value;
		Size size;
		public StringViewInfo(Font font, string value) {
			this.font = font;
			this.value = value;
			UpdateSize();
		}
		public string Value {
			get { return value; }
			set {
				if(this.value == value) return;
				this.value = value;
				UpdateSize();
			}
		}
		public Font Font {
			get { return font; }
			set {
				if(font == value) return;
				font = value;
				UpdateSize();
			}
		}
		public Point Location { get; set; }
		public Size Size { get { return size; } }
		public Rectangle GetBounds() {
			return new Rectangle(Location, Size);
		}
		void UpdateSize() {
			this.size = TextRenderer.MeasureText(Value, Font);
		}
	}
	class RangeControlRulerValueViewInfo {
		public delegate FormatInfo GetFormatDelegate(object value);
		public RangeControlRulerValueViewInfo(object value, GetFormatDelegate getFormat, Font font, int preferredPosition, bool isRequired) {
			Value = value as string ?? PivotDisplayValueFormatter.Format(getFormat(value), value);
			Bounds = new Rectangle(Point.Empty, TextRenderer.MeasureText(Value, font));
			PreferredPosition = preferredPosition;
			IsRequired = isRequired;
			IsVisible = true;
		}
		public string Value { get; private set; }
		public int PreferredPosition { get; private set; }
		public bool IsRequired { get; private set; }
		public Rectangle Bounds { get; set; }
		public bool IsVisible { get; set; }
	}
	class RangeControlRulerValuesArranger : IEnumerable<RangeControlRulerValueViewInfo> {
		readonly List<RangeControlRulerValueViewInfo> viewInfos = new List<RangeControlRulerValueViewInfo>();
		readonly Rectangle rulerBounds;
		public RangeControlRulerValuesArranger(Rectangle rulerBounds) {
			this.rulerBounds = rulerBounds;
		}
		protected Rectangle RulerBounds { get { return rulerBounds; } }
		public void Add(params RangeControlRulerValueViewInfo[] values) {
			viewInfos.AddRange(values);
		}
		public void Arrange() {
			SetPreferredPosition();
			ResolveBoundsExceeding();
			ResolveRequiredValuesOverlapping();
			HideOverlappedNotRequired();
		}
		void SetPreferredPosition() {
			foreach(RangeControlRulerValueViewInfo viewInfo in viewInfos) {
				Rectangle bounds = viewInfo.Bounds;
				int left = viewInfo.PreferredPosition - bounds.Width / 2;
				int top = RulerBounds.Top + (RulerBounds.Height - bounds.Height) / 2;
				bounds.Location = new Point(left, top);
				viewInfo.Bounds = bounds;
			}
		}
		void ResolveBoundsExceeding() {
			foreach(RangeControlRulerValueViewInfo viewInfo in viewInfos) {
				Rectangle bounds = viewInfo.Bounds;
				if(bounds.Left < RulerBounds.Left)
					bounds.Offset(RulerBounds.Left - bounds.Left, 0);
				if(bounds.Right > RulerBounds.Right)
					bounds.Offset(RulerBounds.Right - bounds.Right, 0);
				viewInfo.Bounds = bounds;
			}
		}
		void ResolveRequiredValuesOverlapping() {
			List<RangeControlRulerValueViewInfo> requiredViewInfo = viewInfos.FindAll(vi => vi.IsRequired);
			RangeControlRulerValueViewInfo leftElementViewInfo = null;
			RangeControlRulerValueViewInfo rightElementViewInfo = null;
			if(requiredViewInfo[0].Bounds.Left <= requiredViewInfo[1].Bounds.Left) {
				leftElementViewInfo = requiredViewInfo[0];
				rightElementViewInfo = requiredViewInfo[1];
			} else {
				leftElementViewInfo = requiredViewInfo[1];
				rightElementViewInfo = requiredViewInfo[0];
			}
			Rectangle leftElementbounds = leftElementViewInfo.Bounds;
			Rectangle rightElementBounds = rightElementViewInfo.Bounds;
			if(!leftElementbounds.IntersectsWith(rightElementBounds)) return;
			if(leftElementbounds.Right > rightElementBounds.Left)
				ResolveRequiredValuesOverlappingCore(ref leftElementbounds, ref rightElementBounds);
			else
				ResolveRequiredValuesOverlappingCore(ref rightElementBounds, ref leftElementbounds);
			leftElementViewInfo.Bounds = leftElementbounds;
			rightElementViewInfo.Bounds = rightElementBounds;
		}
		void ResolveRequiredValuesOverlappingCore(ref Rectangle bounds1, ref Rectangle bounds2) {
			int offsetX = (bounds1.Right - bounds2.Left) / 2;
			bounds1.Offset(-offsetX, 0);
			bounds2.Offset(offsetX, 0);
			if(bounds1.Left < RulerBounds.Left) {
				offsetX = RulerBounds.Left - bounds1.Left;
				bounds1.Offset(offsetX, 0);
				bounds2.Offset(offsetX, 0);
			}
			if(bounds2.Right > RulerBounds.Right) {
				offsetX = RulerBounds.Right - bounds2.Right;
				bounds1.Offset(offsetX, 0);
				bounds2.Offset(offsetX, 0);
			}
		}
		void HideOverlappedNotRequired() {
			foreach(var notRequired in viewInfos.Where(vi => !vi.IsRequired))
				notRequired.IsVisible = viewInfos.Where(vi => vi.IsRequired).All(vi => !notRequired.Bounds.IntersectsWith(vi.Bounds));
		}
		public IEnumerator<RangeControlRulerValueViewInfo> GetEnumerator() {
			return viewInfos.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return viewInfos.GetEnumerator();
		}
	}
}
