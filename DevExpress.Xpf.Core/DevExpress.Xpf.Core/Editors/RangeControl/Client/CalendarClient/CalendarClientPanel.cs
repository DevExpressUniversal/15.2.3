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
using System.Windows.Navigation;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors.RangeControl.Internal {
	public class CalendarClientPanel : Panel {
		const double MaxGroupItemWidth = 300d;
		const int LeftTextPadding = 15;
		internal CalendarClient Owner { get; set; }
		IRangeControlClient Instance { get { return Owner as IRangeControlClient; } }
		List<CalendarItem> Items { get; set; }
		List<CalendarGroupItem> GroupItems { get; set; }
		Rect RenderBounds { get; set; }
		bool IsSelectionMarkerVisible { get; set; }
		internal LayoutInfo LayoutInfo { get; private set; }
		int freeItemIndex = 0;
		private CalendarItem GetItem() {
			if (Items == null)
				Items = new List<CalendarItem>();
			if (freeItemIndex >= Items.Count) {
				CalendarItem item = new CalendarItem();
				if (Owner.ItemStyle != null) item.Style = Owner.ItemStyle;
				Items.Add(item);
				Children.Add(item);
			}
			return Items[freeItemIndex++];
		}
		int freeGroupItemIndex = 0;
		private CalendarGroupItem GetGroupItem() {
			if (GroupItems == null)
				GroupItems = new List<CalendarGroupItem>();
			if (freeGroupItemIndex >= GroupItems.Count) {
				CalendarGroupItem groupItem = new CalendarGroupItem();
				if (Owner.GroupItemStyle != null) groupItem.Style = Owner.GroupItemStyle;
				GroupItems.Add(groupItem);
				Children.Add(groupItem);
			}
			return GroupItems[freeGroupItemIndex++];
		}
		private void HideItems() {
			if (Items == null) return;
			for (int i = freeItemIndex; i < Items.Count; i++)
				Items[i].Arrange(new Rect(-100, 0.0, 1.0, 1.0));
		}
		private void HideIGrouptems() {
			if (GroupItems == null) return;
			for (int i = freeGroupItemIndex; i < GroupItems.Count; i++)
				GroupItems[i].Arrange(new Rect(-100, 0.0, 1.0, 1.0));
		}
		private void ClearIndexes() {
			freeItemIndex = 0;
			freeGroupItemIndex = 0;
		}
		private void ClearGroupItems() {
			GroupItems = null;
			Children.Clear();
		}
		private void ClearItems() {
			Items = null;
			Children.Clear();
		}
		protected virtual void FinishRender() {
			HideItems();
			HideIGrouptems();
			ClearIndexes();
		}
		internal virtual void Render() {
			if (ActualWidth < 1) return;
			RenderContent();
		}
		void RenderContent() {
			double groupingHeight = Owner.GetGroupingHeight();
			double clientHeight = RenderBounds.Size.Height - groupingHeight;
			if (Owner.AllowGrouping)
				RenderGroupIntervals(CreateGroupMapping(groupingHeight));
			RenderItemIntervals(CreateClientMapping(clientHeight));
		}
		private Rect CalcTextRect(Rect rect) {
			if (rect.Width < LeftTextPadding)
				return Rect.Empty;
			RectHelper.Deflate(ref rect, new Thickness(LeftTextPadding, 0, 0, 0));
			return rect;
		}
		private IMapping CreateClientMapping(double clientHeight) {
			return new PointMapping(new Point(0, 0), new Rect(0, 0, ActualWidth, clientHeight));
		}
		private IMapping CreateGroupMapping(double groupHeight) {
			return new PointMapping(new Point(0, RenderBounds.Size.Height - groupHeight), new Rect(0, 0, ActualWidth, ActualHeight));
		}
		private void RenderItemIntervals(IMapping pointMapping) {
			RenderItemIntervalsInternal(pointMapping);
		}
		protected void RenderGroupIntervals(IMapping pointMapping) {
			RenderRects(pointMapping, Owner.GroupIntervalFactory, CalcTextRect, true);
		}
		protected void RenderItemIntervalsInternal(IMapping pointMapping) {
			RenderRects(pointMapping, Owner.ItemIntervalFactory, GetItemTextRect, false);
		}
		private Rect GetItemTextRect(Rect rect) {
			RectHelper.Inflate(ref rect, 0d, -8d);
			return rect;
		}
		private void RenderRects(IMapping mapping, IntervalFactory intervalFactory, Func<Rect, Rect> calcTextRect, bool isGroup) {
			double comparableRenderStart = LayoutInfo.ComparableRenderVisibleStart;
			object realRegionStart = intervalFactory.Snap(Instance.GetRealValue(comparableRenderStart));
			object snappedStart = intervalFactory.Snap(realRegionStart);
			double comparableSnappedStart = Instance.GetComparableValue(snappedStart);
			double comparableRenderEnd = LayoutInfo.ComparableRenderVisibleEnd;
			object realRegionEnd = intervalFactory.Snap(Instance.GetRealValue(comparableRenderEnd));
			object snappedEnd = intervalFactory.Snap(realRegionEnd);
			snappedEnd = intervalFactory.GetNextValue(snappedEnd);
			double comparableSnappedEnd = Instance.GetComparableValue(snappedEnd);
			double currentComparable = comparableSnappedStart;
			object current = snappedStart;
			double textWidth = 0;
			while (currentComparable < comparableSnappedEnd) {
				object next = intervalFactory.GetNextValue(current);
				double nextComparable = Instance.GetComparableValue(next);
				double leftNormalValue = LayoutInfo.GetNormalValue(currentComparable);
				double rightNormalValue = LayoutInfo.GetNormalValue(nextComparable);
				Point leftTop = mapping.GetSnappedPoint(leftNormalValue, 0);
				Point middleBottom = mapping.GetSnappedPoint(rightNormalValue, 1);
				Rect rect = new Rect(leftTop, middleBottom);
				Rect arrangeRect = rect;
				Rect textRect = calcTextRect(rect);
				string text;
				if (textWidth == 0) textWidth = textRect.Size.Width - 1;
				intervalFactory.FormatText(current, out text, Owner.FontSize, textWidth);
				CalendarItemBase item;
				double step = nextComparable - currentComparable;
				if (!isGroup) {
					item = GetItem();
					(item as CalendarItem).IsSelected = IsInSelectionRange(currentComparable, nextComparable);
					PatchArrangeRect(ref arrangeRect, currentComparable == comparableSnappedStart, nextComparable >= comparableSnappedEnd - step);
					double selectionStart = Instance.GetComparableValue(Instance.SelectionStart);
					double selectionEnd = Instance.GetComparableValue(Instance.SelectionEnd);
					double selectionRange = selectionEnd - selectionStart;
					bool isSelectionInRange = currentComparable <= selectionStart && nextComparable >= selectionEnd;
					if (selectionRange != 0 && isSelectionInRange && selectionRange < step) {
						IsSelectionMarkerVisible = true;
						selectionMarker.Content = DataContext;
						selectionMarker.ContentTemplate = Owner.ZoomOutSelectionMarkerTemplate;
						selectionMarker.Measure(arrangeRect.Size);
						if (!double.IsInfinity(arrangeRect.Left) && !double.IsInfinity(arrangeRect.Width))
							selectionMarker.Arrange(arrangeRect);
						else
							selectionMarker.Arrange(new Rect(0, 0, 0, 0));
					}
					else if (!IsSelectionMarkerVisible) selectionMarker.Arrange(new Rect(0, 0, 0, 0));
				}
				else {
					item = GetGroupItem();
					double renderLeft = ComparableToRender(LayoutInfo.ComparableVisibleStart);
					double textLeft = arrangeRect.Left;
					double offset = renderLeft > textLeft ? (renderLeft - textLeft) : 0;
					((CalendarGroupItem)item).SetTextOffset(offset);
					if (arrangeRect.Width > MaxGroupItemWidth)
						text = intervalFactory.GetLongestText(current);
					PatchArrangeRect(ref arrangeRect, currentComparable == comparableSnappedStart, nextComparable >= comparableSnappedEnd - step);
				}
				item.Text = text;
				item.Measure(arrangeRect.Size);
				if (!double.IsInfinity(arrangeRect.Left) && !double.IsInfinity(arrangeRect.Width))
					item.Arrange(arrangeRect);
				else
					item.Arrange(new Rect(0, 0, 0, 0));
				current = next;
				currentComparable = nextComparable;
			}
		}
		private void PatchArrangeRect(ref Rect arrangeRect, bool isFirstItem, bool isLastItem) {
			if (isFirstItem && LayoutInfo.ComparableVisibleStart == LayoutInfo.ComparableStart)
				arrangeRect = new Rect(arrangeRect.Left - 1, arrangeRect.Top, arrangeRect.Width + 1, arrangeRect.Height);
			else if (isLastItem && LayoutInfo.ComparableVisibleEnd == LayoutInfo.ComparableEnd)
				arrangeRect = new Rect(arrangeRect.Left, arrangeRect.Top, arrangeRect.Width + 1, arrangeRect.Height);
		}
		private double ComparableToRender(double comparable) {
			return ToNormalized(comparable) * ActualWidth;
		}
		private double ToNormalized(double value) {
			double comparableStart = LayoutInfo.ComparableStart;
			double comparableEnd = LayoutInfo.ComparableEnd;
			return (value - comparableStart) / (comparableEnd - comparableStart);
		}
		private bool IsInSelectionRange(double currentComparable, double nextComparable) {
			return ContainsPoint(LayoutInfo.ComparableSelectionStart, LayoutInfo.ComparableRenderVisibleEnd, currentComparable) && ContainsPoint(LayoutInfo.ComparableSelectionStart, LayoutInfo.ComparableSelectionEnd, nextComparable);
		}
		private bool ContainsPoint(double start, double end, double current) {
			return current >= start && current <= end;
		}
		internal void Clear() {
			ClearItems();
			ClearGroupItems();
			ClearIndexes();
		}
		internal void Invalidate(LayoutInfo info, Rect bounds) {
			LayoutInfo = info;
			RenderBounds = bounds;
			InvalidateArrange();
		}
		ContentPresenter selectionMarker;
		protected override Size ArrangeOverride(Size finalSize) {
			if (double.IsInfinity(finalSize.Height) || double.IsInfinity(finalSize.Width)) return base.ArrangeOverride(finalSize);
			if (selectionMarker == null) CreateSelectionMarker();
			BeginRender();
			Render();
			FinishRender();
			return finalSize;
		}
		private void CreateSelectionMarker() {
			selectionMarker = new ContentPresenter();
			selectionMarker.SetValue(Panel.ZIndexProperty, 1);
			selectionMarker.ContentTemplate = Owner.ZoomOutSelectionMarkerTemplate;
			Children.Add(selectionMarker);
		}
		private void BeginRender() {
			IsSelectionMarkerVisible = false;
		}
		internal void SetLayoutInfo(LayoutInfo info, Rect bounds) {
			LayoutInfo = info;
			RenderBounds = bounds;
		}
	}
}
