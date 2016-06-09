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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.WindowsUI.Base;
#if SILVERLIGHT
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class SplitPanel : BaseItemsPanel {
		#region static
		public static readonly DependencyProperty PaddingProperty;
		public static readonly DependencyProperty ItemSizeModeProperty;
		public static readonly DependencyProperty IndexProperty;
		public static readonly DependencyProperty SizeModeProperty;
		public static readonly DependencyProperty StretchRatioProperty;
		public static readonly DependencyProperty AllowClippingProperty;
		static SplitPanel() {
			var dProp = new DependencyPropertyRegistrator<SplitPanel>();
			dProp.Register("Padding", ref PaddingProperty, new Thickness(0),
				(dObj, e) => ((SplitPanel)dObj).OnPaddingChanged());
			dProp.Register("ItemSizeMode", ref ItemSizeModeProperty, ItemSizeMode.Default,
				(dObj, e) => ((SplitPanel)dObj).OnItemSizeModeChanged());
			dProp.RegisterAttached("Index", ref IndexProperty, -1,
				OnIndexChanged);
			dProp.RegisterAttached("StretchRatio", ref StretchRatioProperty, double.NaN,
				OnStretchRatioChanged);
			dProp.RegisterAttached("SizeMode", ref SizeModeProperty, ItemSizeMode.Default,
				OnSizeModeChanged);
			dProp.Register("AllowClipping", ref AllowClippingProperty, false,
				OnAllowClippingChanged);
		}
		static void OnIndexChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			InvalidateItemsPanelMeasure(dObj);
		}
		static void OnStretchRatioChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			InvalidateItemsPanelMeasure(dObj);
		}
		static void OnSizeModeChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			InvalidateItemsPanelMeasure(dObj);
		}
		static void OnAllowClippingChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			InvalidateItemsPanelMeasure(dObj);
		}
		public static int GetIndex(DependencyObject dObj) {
			return (int)dObj.GetValue(IndexProperty);
		}
		public static void SetIndex(DependencyObject dObj, int index) {
			dObj.SetValue(IndexProperty, index);
		}
		public static double GetStretchRatio(DependencyObject dObj) {
			return (double)dObj.GetValue(StretchRatioProperty);
		}
		public static void SetStretchRatio(DependencyObject dObj, double size) {
			dObj.SetValue(StretchRatioProperty, size);
		}
		public static ItemSizeMode GetSizeMode(DependencyObject dObj) {
			return (ItemSizeMode)dObj.GetValue(SizeModeProperty);
		}
		public static void SetSizeMode(DependencyObject dObj, ItemSizeMode strategy) {
			dObj.SetValue(SizeModeProperty, strategy);
		}
		#endregion static
		#region Properties
		public Thickness Padding {
			get { return (Thickness)GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}
		public ItemSizeMode ItemSizeMode {
			get { return (ItemSizeMode)GetValue(ItemSizeModeProperty); }
			set { SetValue(ItemSizeModeProperty, value); }
		}
		public bool IsStretch {
			get { return ItemSizeMode != ItemSizeMode.AutoSize; }
		}
		public bool AllowClipping {
			get { return (bool)GetValue(AllowClippingProperty); }
			set { SetValue(AllowClippingProperty, value); }
		}
		#endregion
		#region Property Changed
		protected virtual void OnPaddingChanged() {
			OnObjectChanged();
		}
		protected virtual void OnItemSizeModeChanged() {
			strategyCore = null;
			OnObjectChanged();
		}
		protected virtual void OnItemSizeModeProviderChanged() {
			OnObjectChanged();
		}
		#endregion Property Changed
		#region Strategy
		ISplitStrategy strategyCore;
		protected ISplitStrategy Strategy {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(strategyCore == null) {
					switch(ItemSizeMode) {
						case ItemSizeMode.Default:
						case ItemSizeMode.Stretch:
							strategyCore = CreateProportionalSplitStrategy();
							break;
						case ItemSizeMode.AutoSize:
							strategyCore = CreateStackedSplitStrategy();
							break;
						default:
							throw new NotSupportedException();
					}
				}
				return strategyCore;
			}
		}
		protected virtual ISplitStrategy CreateProportionalSplitStrategy() {
			return new ProportionalSplitStrategy();
		}
		protected virtual ISplitStrategy CreateStackedSplitStrategy() {
			return new StackedSplitStrategy();
		}
		#endregion Strategy
		protected virtual ILayoutCell[] GetLayoutCells(Size availableSize, UIElementCollection elements, bool isMeasure) {
#if SILVERLIGHT
			IDictionary<int, ICollection<ILayoutCell>> cells = new Dictionary<int, ICollection<ILayoutCell>>();
#else
			IDictionary<int, ICollection<ILayoutCell>> cells = new SortedDictionary<int, ICollection<ILayoutCell>>();
#endif
			bool horz = IsHorizontal;
			Size availableAutoSize = new Size(
				horz ? double.PositiveInfinity : availableSize.Width,
				horz ? availableSize.Height : double.PositiveInfinity);
			bool isInfiniteMeasure = double.IsInfinity(horz ? availableSize.Width : availableSize.Height);
			int cellIndex = 0;
			foreach(UIElement child in Children) {
				if(child.Visibility == Visibility.Collapsed) {
					if(isMeasure)
						child.Measure(availableSize);
					continue;
				}
				int index = Math.Max(-1, GetIndex(child));
				if(index == -1)
					index = cellIndex++;
				ICollection<ILayoutCell> cellElements;
				if(!cells.TryGetValue(index, out cellElements)) {
					cellElements = new List<ILayoutCell>();
					cells.Add(index, cellElements);
				}
				if(isMeasure) {
					if(isInfiniteMeasure || IsAutoSize(child))
						child.Measure(availableAutoSize);
					else child.Measure(availableSize);
				}
				cellElements.Add(CreateLayoutCell(child));
			}
			int i = 0;
			ILayoutCell[] result = new ILayoutCell[cells.Count];
			foreach(var pair in cells)
				result[i++] = MergeLayoutCells(pair.Value);
			return result;
		}
		protected virtual ILayoutCell CreateLayoutCell(UIElement child) {
			bool isAutoSize = IsAutoSize(child);
			double ratio = !isAutoSize ? GetStretchRatio(child) : double.NaN;
			return new LayoutCell(child.DesiredSize, GetLayoutAlignment(child), ratio, isAutoSize);
		}
		protected bool IsAutoSize(UIElement child) {
			object mode = child.ReadLocalValue(SizeModeProperty);
			ItemSizeMode realSizeMode = (mode == DependencyProperty.UnsetValue || object.Equals(mode, ItemSizeMode.Default)) ?
				ItemSizeMode : (ItemSizeMode)mode;
			return (realSizeMode == ItemSizeMode.AutoSize);
		}
		protected LayoutAlignment GetLayoutAlignment(UIElement child) {
			bool horz = IsHorizontal;
			DependencyProperty alignmentProperty = horz ?
				FrameworkElement.HorizontalAlignmentProperty : FrameworkElement.VerticalAlignmentProperty;
			object nearAlignment = horz ? (object)HorizontalAlignment.Left : (object)VerticalAlignment.Top;
			object farAlignment = horz ? (object)HorizontalAlignment.Right : (object)VerticalAlignment.Bottom;
			object alignment = child.ReadLocalValue(alignmentProperty);
			if(object.Equals(nearAlignment, alignment))
				return LayoutAlignment.Near;
			if(object.Equals(farAlignment, alignment))
				return LayoutAlignment.Far;
			return LayoutAlignment.Default;
		}
		protected ILayoutCell MergeLayoutCells(IEnumerable<ILayoutCell> cells) {
			Size desiredSize = new Size(0, 0);
			double ratio = double.NaN;
			bool isAutoSize = false;
			LayoutAlignment? desiredAlignment = null;
			foreach(ILayoutCell cell in cells) {
				desiredSize = MathHelper.Union(desiredSize, cell.DesiredSize);
				if(!desiredAlignment.HasValue) {
					if(cell.DesiredAlignment != LayoutAlignment.Default)
						desiredAlignment = cell.DesiredAlignment;
				}
				else {
					if(cell.DesiredAlignment != desiredAlignment.Value)
						desiredAlignment = LayoutAlignment.Default;
				}
				isAutoSize |= cell.IsAutoSize;
				if(!isAutoSize && !double.IsNaN(cell.StretchRatio))
					ratio = Math.Max(cell.StretchRatio, MathHelper.GetDimension(ratio));
			}
			if(isAutoSize)
				ratio = double.NaN;
			return new LayoutCell(desiredSize, desiredAlignment.GetValueOrDefault(LayoutAlignment.Default), ratio, isAutoSize);
		}
		protected virtual void StretchLayoutCells(Size availableSize, ILayoutCell[] cells, Rect[] rects) {
			bool horz = IsHorizontal;
			int cellIndex = 0;
			bool isInfiniteMeasure = double.IsInfinity(horz ? availableSize.Width : availableSize.Height);
			foreach(UIElement child in Children) {
				if(child.Visibility == Visibility.Collapsed) continue;
				int index = NextLayoutCellIndex(ref cellIndex, child, cells.Length);
				if(isInfiniteMeasure || cells[index].IsAutoSize) continue;
				Size availableCellSize = new Size(
					horz ? rects[index].Width : availableSize.Width,
					horz ? availableSize.Height : rects[index].Height);
				child.Measure(availableCellSize);
			}
		}
		protected int NextLayoutCellIndex(ref int cellIndex, UIElement child, int maxIndex) {
			int index = Math.Max(-1, GetIndex(child));
			if(index == -1)
				index = cellIndex++;
			else index = Math.Min(maxIndex, index);
			return index;
		}
		Rect[] layoutCellsCore;
		protected Rect[] LayoutCells {
			get { return layoutCellsCore; }
		}
		protected virtual Rect[] SplitCore(Rect content, ILayoutCell[] layoutCells, double spacing, bool horz) {
			return Strategy.Split(content, layoutCells, spacing, horz, AllowClipping);
		}
		protected override Size OnArrange(Size finalSize) {
			bool horz = IsHorizontal;
			Rect content = MathHelper.DeflateSize(finalSize, Padding);
			Size arrangeSize = new Size(content.Width, content.Height);
			ILayoutCell[] layoutCells = GetLayoutCells(arrangeSize, Children, false);
			double spacing = MathHelper.GetDimension(ItemSpacing);
			Rect[] rects = SplitCore(content, layoutCells, spacing, horz);
			layoutCellsCore = rects;
			int cellIndex = 0;
			foreach(UIElement child in Children) {
				if(child.Visibility == Visibility.Collapsed) continue;
				int index = NextLayoutCellIndex(ref cellIndex, child, rects.Length);
				child.Arrange(rects[index]);
			}
			return finalSize;
		}
		protected override Size OnMeasure(Size availableSize) {
			bool horz = IsHorizontal;
			Rect content = MathHelper.DeflateSize(availableSize, Padding);
			Size measureSize = new Size(content.Width, content.Height);
			ILayoutCell[] layoutCells = GetLayoutCells(measureSize, Children, true);
			double spacing = MathHelper.GetDimension(ItemSpacing);
			Rect[] rects = SplitCore(content, layoutCells, spacing, horz);
			StretchLayoutCells(measureSize, layoutCells, rects);
			double w = 0, h = 0;
			for(int i = 0; i < rects.Length; i++) {
				Size actualCellSize = layoutCells[i].DesiredSize;
				if(horz) {
					double actualSpacing = (i == 0 || actualCellSize.Width <= 0) ? 0.0 : spacing;
					w += (actualCellSize.Width + actualSpacing);
					h = Math.Max(h, actualCellSize.Height);
				}
				else {
					double actualSpacing = (i == 0 || actualCellSize.Height <= 0) ? 0.0 : spacing;
					w = Math.Max(w, actualCellSize.Width);
					h += (actualCellSize.Height + actualSpacing);
				}
			}
			w = Math.Min(content.Width, w);
			h = Math.Min(content.Height, h);
			return MathHelper.Inflate(new Size(w, h), Padding);
		}
	}
	static class LayoutAlignmentHelper {
		internal static void CorrectCellRectsByAlignment(Rect[] rects, ILayoutCell[] layoutCells, Rect bounds, double spacing, bool horizontal) {
			double[] spacings = new double[System.Math.Max(0, layoutCells.Length - 1)];
			for(int i = 0; i < spacings.Length; i++)
				spacings[i] = spacing;
			CorrectCellRectsByAlignment(rects, layoutCells, bounds, spacings, horizontal);
		}
		internal static void CorrectCellRectsByAlignment(Rect[] rects, ILayoutCell[] layoutCells, Rect bounds, double[] spacings, bool horizontal) {
			double far = horizontal ? bounds.Right : bounds.Bottom;
			for(int i = layoutCells.Length - 1; i >= 0; i--) {
				if(layoutCells[i].DesiredAlignment != LayoutAlignment.Far) break;
				double l = horizontal ? rects[i].Width : rects[i].Height;
				if(horizontal)
					rects[i] = new Rect(far - l, rects[i].Top, rects[i].Width, rects[i].Height);
				else
					rects[i] = new Rect(rects[i].Left, far - l, rects[i].Width, rects[i].Height);
				if(i - 1 >= 0)
					far -= (l + spacings[i - 1]);
			}
		}
		internal static void CorrectAllCellRectsByAlignment(Rect[] rects, ILayoutCell[] layoutCells, Rect bounds, double spacing, bool horizontal) {
			double[] spacings = new double[System.Math.Max(0, layoutCells.Length - 1)];
			for(int i = 0; i < spacings.Length; i++)
				spacings[i] = spacing;
			CorrectAllCellRectsByAlignment(rects, layoutCells, bounds, spacings, horizontal);
		}
		internal static void CorrectAllCellRectsByAlignment(Rect[] rects, ILayoutCell[] layoutCells, Rect bounds, double[] spacings, bool horizontal) {
			double far = horizontal ? bounds.Right : bounds.Bottom;
			double near = 0;
			for(int i = layoutCells.Length - 1; i >= 0; i--) {
				if(layoutCells[i].DesiredAlignment == LayoutAlignment.Default) {
					continue;
				}
				double l = horizontal ? rects[i].Width : rects[i].Height;
				if(layoutCells[i].DesiredAlignment == LayoutAlignment.Far) {
					double x = horizontal ? far - l : rects[i].Left;
					double y = horizontal ? rects[i].Top : far - l;
					rects[i] = new Rect(x, y, rects[i].Width, rects[i].Height);
					if(i - 1 >= 0)
						far -= (l + spacings[i - 1]);
				}
			}
			for(int i = 0; i < layoutCells.Length; i++) {
				if(layoutCells[i].DesiredAlignment != LayoutAlignment.Near) {
					continue;
				}
				double l = horizontal ? rects[i].Width : rects[i].Height;
				double x = horizontal ? near : rects[i].Left;
				double y = horizontal ? rects[i].Top : near;
				rects[i] = new Rect(x, y, rects[i].Width, rects[i].Height);
				if(i + 1 < layoutCells.Length)
					near += (l + spacings[i]);
			}
			for(int i = 0; i < layoutCells.Length; i++) {
				if(layoutCells[i].DesiredAlignment != LayoutAlignment.Default) {
					continue;
				}
				double l = horizontal ? rects[i].Width : rects[i].Height;
				double x = horizontal ? near : rects[i].Left;
				double y = horizontal ? rects[i].Top : near;
				rects[i] = new Rect(x, y, rects[i].Width, rects[i].Height);
				if(i + 1 < layoutCells.Length)
					near += (l + spacings[i]);
			}
		}
	}
}
