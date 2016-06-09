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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Gauges {
	public class SymbolsPanel : Panel {
		const double defaultWidth = 300.0;
		const double defaultHeight = 300.0;
		DigitalGaugeControl Gauge { get { return DataContext as DigitalGaugeControl; } }
		TextHorizontalAlignment TextHorizontalAlignment { get { return Gauge != null ? Gauge.TextHorizontalAlignment : TextHorizontalAlignment.Center; } }
		TextVerticalAlignment TextVerticalAlignment { get { return Gauge != null ? Gauge.TextVerticalAlignment : TextVerticalAlignment.Center; } }
		protected override Size MeasureOverride(Size availableSize) {
			double constraintWidth = double.IsInfinity(availableSize.Width) ? defaultWidth : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? defaultHeight : availableSize.Height;
			Size constraint = new Size(constraintWidth, constraintHeight);
			double width = 0, height = 0;
			foreach (UIElement child in Children) {
				child.Measure(constraint);
				width = Math.Max(width, child.DesiredSize.Width);
				height = Math.Max(height, child.DesiredSize.Height);
			}
			return new Size(width, height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (UIElement child in Children) {
				double x, y;
				switch (TextHorizontalAlignment) {
					case TextHorizontalAlignment.Left:
					x = 0.0;
					break;
					case TextHorizontalAlignment.Center:
					x = 0.5 * (finalSize.Width - child.DesiredSize.Width);
					break;
					case TextHorizontalAlignment.Right:
					x = finalSize.Width - child.DesiredSize.Width;
					break;
					default:
					goto case TextHorizontalAlignment.Center;
				}
				switch (TextVerticalAlignment) {
					case TextVerticalAlignment.Top:
					y = 0.0;
					break;
					case TextVerticalAlignment.Center:
					y = 0.5 * (finalSize.Height - child.DesiredSize.Height);
					break;
					case TextVerticalAlignment.Bottom:
					y = finalSize.Height - child.DesiredSize.Height;
					break;
					default:
					goto case TextVerticalAlignment.Center;
				}
				child.Arrange(new Rect(Math.Max(0, x), Math.Max(0, y), child.DesiredSize.Width, child.DesiredSize.Height));
			}
			return finalSize;
		}
	}
	public class SymbolsLayoutControl : Control {
		const double defaultWidth = 300.0;
		const double defaultHeight = 300.0;
		DigitalGaugeControl Gauge { get { return DataContext as DigitalGaugeControl; } }
		SymbolViewBase View { get { return Gauge != null ? Gauge.ActualSymbolView : null; } }
		int SymbolCount { get { return Gauge != null ? Gauge.ActualSymbolCount : 1; } }
		double ActualHeightToWidthRatio {
			get {
				if (View.Height.Type == SymbolLengthType.Proportional) {
					if (View.Width.Type == SymbolLengthType.Proportional)
						return View.Height.ProportionalLength / View.Width.ProportionalLength;
					else
						return View.Height.ProportionalLength;
				}
				else
					if (View.Width.Type == SymbolLengthType.Proportional)
						return 1 / View.Width.ProportionalLength;
				return Gauge.SymbolViewInternal.DefaultHeightToWidthRatio;
			}
		}
		bool IsAutoSize {
			get {
				return (View.Height.Type == SymbolLengthType.Auto && View.Width.Type == SymbolLengthType.Auto) ||
						View.Height.Type == SymbolLengthType.Proportional && View.Width.Type == SymbolLengthType.Proportional;
			}
		}
		bool IsStretchByHorizontal(Size availableSymbolSize) { return View.Width.Type == SymbolLengthType.Stretch || (IsAutoSize && IsBasedOnWidth(availableSymbolSize)) || (View.Width.Type == SymbolLengthType.Auto && IsBasedOnWidth(availableSymbolSize)); }
		bool IsStretchByVertical(Size availableSymbolSize) { return View.Height.Type == SymbolLengthType.Stretch || (IsAutoSize && !IsBasedOnWidth(availableSymbolSize)) || (View.Height.Type == SymbolLengthType.Auto && !IsBasedOnWidth(availableSymbolSize)); }
		bool IsBasedOnWidth(Size availableSymbolSize) {
			if (IsAutoSize) {
				double height = availableSymbolSize.Width / SymbolCount * ActualHeightToWidthRatio;
				return height < availableSymbolSize.Height;
			}
			if (View.Width.Type == SymbolLengthType.Auto)
				return View.Height.Type == SymbolLengthType.Proportional;
			return View.Width.Type != SymbolLengthType.Proportional;
		}
		Size CalcSizeByHorizontal(Size availableSymbolSize) {
			double width = IsStretchByHorizontal(availableSymbolSize) ? availableSymbolSize.Width / SymbolCount : View.Width.FixedLength;
			double height;
			switch (View.Height.Type) {
				case SymbolLengthType.Stretch:
				height = availableSymbolSize.Height;
				break;
				case SymbolLengthType.Fixed:
				height = View.Height.FixedLength;
				break;
				case SymbolLengthType.Auto:
				case SymbolLengthType.Proportional:
				default:
				height = width * ActualHeightToWidthRatio;
				break;
			}
			return new Size(width, height);
		}
		Size CalcSizeByVertical(Size availableSymbolSize) {
			double height = IsStretchByVertical(availableSymbolSize) ? availableSymbolSize.Height : View.Height.FixedLength;
			double width;
			switch (View.Width.Type) {
				case SymbolLengthType.Stretch:
				width = availableSymbolSize.Width / SymbolCount;
				break;
				case SymbolLengthType.Fixed:
				width = View.Width.FixedLength;
				break;
				case SymbolLengthType.Auto:
				case SymbolLengthType.Proportional:
				default:
				width = height / ActualHeightToWidthRatio;
				break;
			}
			return new Size(width, height);
		}
		Size CalcSymbolSize(Size availableSymbolSize) {
			return IsBasedOnWidth(availableSymbolSize) ? CalcSizeByHorizontal(availableSymbolSize) : CalcSizeByVertical(availableSymbolSize);
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (Gauge == null)
				return base.MeasureOverride(availableSize);
			double constraintWidth = double.IsInfinity(availableSize.Width) ? defaultWidth : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? defaultHeight : availableSize.Height;
			Size symbolSize = CalcSymbolSize(new Size(constraintWidth, constraintHeight));
			Gauge.SymbolsLayout = new SymbolsLayout(this, symbolSize);
			return new Size(Math.Min(symbolSize.Width * SymbolCount, constraintWidth), Math.Min(symbolSize.Height, constraintHeight));
		}
	}
}
namespace DevExpress.Xpf.Gauges.Native {
	public class SymbolsLayout {
		readonly SymbolsLayoutControl layoutControl;
		readonly Size symbolSize;
		public Size SymbolSize { get { return symbolSize; } }
		public SymbolsLayoutControl LayoutControl { get { return layoutControl; } }
		public SymbolsLayout(SymbolsLayoutControl layoutControl, Size symbolSize) {
			this.layoutControl = layoutControl;
			this.symbolSize = symbolSize;
		}
		public Point GetSymbolLocation(UIElement baseLayoutElement, SymbolInfo symbolInfo) {
			Rect bounds = LayoutHelper.GetRelativeElementRect(layoutControl, baseLayoutElement);
			return new Point(bounds.Left + SymbolSize.Width * symbolInfo.SymbolIndex, bounds.Top);
		}
		public Rect GetClipBounds(UIElement baseLayoutElement) { return LayoutHelper.GetRelativeElementRect(layoutControl, baseLayoutElement); }
		public void Invalidate() {
			layoutControl.InvalidateMeasure();
			UIElement element = LayoutHelper.GetParent(layoutControl) as UIElement;
			if (element != null)
				element.InvalidateMeasure();
		}
	}
}
