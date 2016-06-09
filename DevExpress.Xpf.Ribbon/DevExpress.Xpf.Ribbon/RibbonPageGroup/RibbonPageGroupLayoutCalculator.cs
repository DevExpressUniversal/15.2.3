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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
namespace DevExpress.Xpf.Ribbon {
	public abstract class RibbonPageGroupLayoutCalculator {
		public static RibbonPageGroupLayoutCalculator CreateLayoutCalculator(RibbonPageGroupItemsPanel owner) {
			if(owner.RibbonStyle != RibbonStyle.Office2007 && owner.RibbonStyle != RibbonStyle.Office2010)
				return new OfficeTabletRibbonPageGroupLayoutCalculator(owner);
			else
				return new DefaultRibbonPageGroupLayoutCalculator(owner);
		}
		readonly RibbonPageGroupItemsPanel owner;
		public RibbonPageGroupItemsPanel Owner { get { return owner; } }
		readonly RibbonStyle ribbonStyle;
		public RibbonStyle RibbonStyle { get { return ribbonStyle; } }
		protected RibbonControl Ribbon { get { return Owner.Ribbon; } }
		protected RibbonPageGroupControl PageGroupControl { get { return Owner.PageGroupControl; } }
		protected RibbonPageGroupLayoutCalculator(RibbonPageGroupItemsPanel owner) {
			if(owner == null)
				throw new ArgumentNullException("owner");
			this.owner = owner;
			this.ribbonStyle = owner.RibbonStyle;
		}
		public abstract Size MeasureOverride(Size availableSize);
		public abstract Size ArrangeOverride(Size finalSize);
		protected virtual double CalcBestHeight() {
			if(PageGroupControl == null)
				return 0d;
			return PageGroupControl.GetBestHeightForPageGroupPanel();
		}
		protected BarItemLinkControlBase GetLinkControl(int index) {
			return GetLinkControl(GetChild(index));
		}
		protected BarItemLinkControlBase GetLinkControl(UIElement child) {
			return ((BarItemLinkInfo)child).LinkControl;
		}
		protected UIElement GetChild(int index) {
			return Owner.Children[index];
		}
		protected int GetChildrenCount() {
			return Owner.Children.Count;
		}
	}
	public class DefaultRibbonPageGroupLayoutCalculator : RibbonPageGroupLayoutCalculator {
		public DefaultRibbonPageGroupLayoutCalculator(RibbonPageGroupItemsPanel owner) :
			base(owner) { }
		public override Size MeasureOverride(Size availableSize) {
			int childrenCount = GetChildrenCount();
			Size constraint = new Size(availableSize.Width, CalcBestHeight());
			for(int i = 0; i < childrenCount; i++) {
				UIElement child = GetChild(i);
				child.Measure(constraint);
			}
			if(PageGroupControl.IsCollapsed)
				return Size.Empty;
			return UpdateLayout(constraint);
		}
		public override Size ArrangeOverride(Size finalSize) {
			var columns = Owner.Children.OfType<BarItemLinkInfo>().Where(elem => elem.Item != null && elem.Item.IsVisible).GroupBy(elem => RibbonPageGroupItemsPanel.GetColumn(elem));
			Rect arrangeRect = new Rect();
			foreach(var column in columns) {
				arrangeRect.Y = 0;
				var rows = column.GroupBy(elem => RibbonPageGroupItemsPanel.GetRow(elem));
				double rowStart = arrangeRect.X;
				double rowsMaxWidth = rows.Max(row => row.Sum(elem => elem.DesiredSize.Width));
				bool onlyButtonGroups = column.All(elem => elem.LinkControl is BarButtonGroupLinkControl);
				double horizIndent = onlyButtonGroups ? Ribbon.ButtonGroupsIndent : Ribbon.ColumnIndent;
				foreach(var row in rows) {
					if(row.All(elem => StretchItemHeight(elem.LinkControl)))
						arrangeRect.Height = finalSize.Height;
					else if(onlyButtonGroups && rows.Any())
						arrangeRect.Height = (finalSize.Height - Ribbon.RowIndent * (rows.Count() - 1)) / rows.Count();
					else
						arrangeRect.Height = Math.Max(row.Max(elem => elem.DesiredSize.Height), (finalSize.Height - Ribbon.RowIndent * 2) / 3);
					foreach(var elem in row) {
						arrangeRect.Width = elem.LinkControl is BarEditItemLinkControl ? rowsMaxWidth : elem.DesiredSize.Width;
						elem.Arrange(arrangeRect);
						arrangeRect.X += arrangeRect.Width + horizIndent;
					}
					arrangeRect.X = rowStart;
					arrangeRect.Y += Ribbon.RowIndent + arrangeRect.Height;
				}
				arrangeRect.X += rowsMaxWidth + Ribbon.ColumnIndent;
			}
			return finalSize;
		}
		protected virtual Size UpdateLayout(Size availableSize) {
			int lastColumnElementIndex = 0;
			int firstColumnElementIndex = 0;
			double columnIndent = Ribbon == null ? 0 : Ribbon.ColumnIndent;
			Size totalSize = new Size();
			int column = 0;
			int childrenCount = GetChildrenCount();
			while(firstColumnElementIndex < childrenCount) {
				lastColumnElementIndex = FindLastColumnElementIndex(firstColumnElementIndex, availableSize.Height);
				Size columnSize = LayoutColumn(firstColumnElementIndex, lastColumnElementIndex, availableSize.Height, totalSize.Width, column);
				totalSize.Width += columnSize.Width;
				totalSize.Height = Math.Max(totalSize.Height, columnSize.Height);
				firstColumnElementIndex = lastColumnElementIndex + 1;
				column++;
			}
			totalSize.Width += Math.Max(0, column - 1) * columnIndent;
			totalSize.Height = Math.Max(totalSize.Height, CalcBestHeight());
			return totalSize;
		}
		protected virtual Size GetMaxSize(int startElementIndex, int endElementIndex) {
			Size size = new Size(0, 0);
			UIElement child = null;
			for(int i = startElementIndex; i <= endElementIndex; i++) {
				child = GetChild(i);
				size.Width = Math.Max(size.Width, child.DesiredSize.Width);
				size.Height = Math.Max(size.Height, child.DesiredSize.Height);
			}
			return size;
		}
		protected virtual int FindLastColumnElementIndex(int startElementIndex, double availableHeight) {
			double height = 0;
			var children = Owner.Children.Cast<BarItemLinkInfo>();
			int childrenCount = children.Count();
			int i = startElementIndex;
			for(; i < childrenCount; i++) {
				BarItemLinkControlBase ctrl = GetLinkControl(i);
				if(ctrl == null)
					continue;
				if(StretchItemHeight(ctrl))
					return Math.Max(startElementIndex, i - 1);
				if(ctrl is BarButtonGroupLinkControl) {
					if(i != startElementIndex)
						return i - 1;
					for(int j = i + 1; j < childrenCount; j++) {
						ctrl = GetLinkControl(j);
						if(!(ctrl is BarButtonGroupLinkControl))
							return j - 1;
					}
					return childrenCount - 1;
				}
				height += ctrl.DesiredSize.Height;
				if(height > availableHeight) {
					i--;
					break;
				} else if (ctrl.LinkInfo.Link.ActualIsVisible)
					height += Ribbon.RowIndent;
				if(children.Skip(startElementIndex).Take(i - startElementIndex + 1).Count(linkInfo => linkInfo.Link.ActualIsVisible) == 3)
					break;
			}
			return Math.Min(i, childrenCount - 1);
		}
		protected virtual bool StretchItemHeight(BarItemLinkControlBase ctrl) {
			var barItemLinkControl = ctrl as BarItemLinkControl;
			if(barItemLinkControl == null)
				return false;
			return barItemLinkControl.RibbonItemInfo.IsLargeButton || (ctrl is BarItemLinkSeparatorControl || ctrl is RibbonGalleryBarItemLinkControl);
		}
		protected virtual Size LayoutButtonGroupRow(int firstIndex, int lastIndex, double x, double y, int rowIndex, int columnIndex) {
			Size rowSize = GetMaxSize(firstIndex, lastIndex);
			Rect location = new Rect(x, Math.Round(y, 0), 0, 0);
			for(int i = firstIndex; i <= lastIndex; i++) {
				UIElement child = GetChild(i);
				RibbonPageGroupItemsPanel.SetRow(child, rowIndex);
				RibbonPageGroupItemsPanel.SetColumn(child, columnIndex);
				location.Y = y + (rowSize.Height - child.DesiredSize.Height) / 2;
				location.Width = child.DesiredSize.Width;
				location.Height = child.DesiredSize.Height;
				location.X += location.Width;
				if(i != lastIndex)
					location.X += Ribbon.ButtonGroupsIndent;
				RibbonPageGroupItemsPanel.SetIsEndOfRow(GetLinkControl(child), i == lastIndex);
			}
			return new Size(location.X - x, rowSize.Height);
		}
		protected virtual Size LayoutButtonGroups(int firstIndex, int lastIndex, double availableHeight, double x, int column) {
			double width = 0;
			Rect location = new Rect(x, 0, 0, 0);
			double rowY = 0;
			Size rowSize = new Size();
			UIElement child = GetChild(firstIndex);
			int rowCount = ButtonGroupSetLayoutCalculator.GetRowCount(GetLinkControl(child));
			int actualRowCount = ButtonGroupSetLayoutCalculator.CalcGroupsLayout(Owner, firstIndex, lastIndex, rowCount);
			double totalHeight = 0d;
			if(actualRowCount == 1) {
				RibbonPageGroupItemsPanel.SetRow(child, 0);
				RibbonPageGroupItemsPanel.SetColumn(child, column);
				width = child.DesiredSize.Width;
				totalHeight = child.DesiredSize.Height;
			} else if(actualRowCount == 2) {
				double rowsHeight = GetMaxSize(ButtonGroupSetLayoutCalculator.GetFirstElementIndexOfRow(firstIndex, 0), ButtonGroupSetLayoutCalculator.GetLastElementIndexOfRow(firstIndex, 0)).Height;
				rowsHeight += GetMaxSize(ButtonGroupSetLayoutCalculator.GetFirstElementIndexOfRow(firstIndex, 1), ButtonGroupSetLayoutCalculator.GetLastElementIndexOfRow(firstIndex, 1)).Height;
				double rowIndent = (availableHeight - rowsHeight) / actualRowCount;
				rowY = rowIndent;
				rowSize = LayoutButtonGroupRow(ButtonGroupSetLayoutCalculator.GetFirstElementIndexOfRow(firstIndex, 0), ButtonGroupSetLayoutCalculator.GetLastElementIndexOfRow(firstIndex, 0), x, rowY, 0, column);
				width = Math.Max(width, rowSize.Width);
				rowY += rowSize.Height + rowIndent;
				rowSize = LayoutButtonGroupRow(ButtonGroupSetLayoutCalculator.GetFirstElementIndexOfRow(firstIndex, 1), ButtonGroupSetLayoutCalculator.GetLastElementIndexOfRow(firstIndex, 1), x, rowY, 2, column);
				width = Math.Max(width, rowSize.Width);
				totalHeight = rowSize.Height + rowY;
			} else {
				double rowsHeight = GetMaxSize(ButtonGroupSetLayoutCalculator.GetFirstElementIndexOfRow(firstIndex, 0), ButtonGroupSetLayoutCalculator.GetLastElementIndexOfRow(firstIndex, 0)).Height;
				rowsHeight += GetMaxSize(ButtonGroupSetLayoutCalculator.GetFirstElementIndexOfRow(firstIndex, 1), ButtonGroupSetLayoutCalculator.GetLastElementIndexOfRow(firstIndex, 1)).Height;
				rowsHeight += GetMaxSize(ButtonGroupSetLayoutCalculator.GetFirstElementIndexOfRow(firstIndex, 2), ButtonGroupSetLayoutCalculator.GetLastElementIndexOfRow(firstIndex, 2)).Height;
				double rowIndent = (availableHeight - rowsHeight) / actualRowCount;
				rowY = rowIndent;
				rowSize = LayoutButtonGroupRow(ButtonGroupSetLayoutCalculator.GetFirstElementIndexOfRow(firstIndex, 0), ButtonGroupSetLayoutCalculator.GetLastElementIndexOfRow(firstIndex, 0), x, rowY, 0, column);
				width = Math.Max(width, rowSize.Width);
				rowY += rowSize.Height + rowIndent;
				rowSize = LayoutButtonGroupRow(ButtonGroupSetLayoutCalculator.GetFirstElementIndexOfRow(firstIndex, 1), ButtonGroupSetLayoutCalculator.GetLastElementIndexOfRow(firstIndex, 1), x, rowY, 1, column);
				width = Math.Max(width, rowSize.Width);
				rowY += rowSize.Height + rowIndent;
				rowSize = LayoutButtonGroupRow(ButtonGroupSetLayoutCalculator.GetFirstElementIndexOfRow(firstIndex, 2), ButtonGroupSetLayoutCalculator.GetLastElementIndexOfRow(firstIndex, 2), x, rowY, 2, column);
				width = Math.Max(width, rowSize.Width);
				totalHeight = rowSize.Height + rowY;
			}
			return new Size(width, totalHeight);
		}
		int GetEditorCount(int firstIndex, int lastIndex) {
			var elems = Owner.Children.OfType<BarItemLinkInfo>().Skip(firstIndex).Take(lastIndex - firstIndex + 1);
			return elems.Count(elem => elem.LinkControl is BarEditItemLinkControl);
		}
		protected virtual Size LayoutColumn(int firstIndex, int lastIndex, double availableHeight, double leftOffset, int column) {
			double width = 0;
			if(firstIndex == lastIndex && StretchItemHeight(GetLinkControl(firstIndex))) {
				UIElement child = GetChild(firstIndex);
				RibbonPageGroupItemsPanel.SetRow(child, 0);
				RibbonPageGroupItemsPanel.SetColumn(child, column);
				width = child.DesiredSize.Width;
				return new Size(width, availableHeight);
			}
			Rect location = new Rect(leftOffset, 0, 0, 0);
			if(GetLinkControl(firstIndex) is BarButtonGroupLinkControl) {
				return LayoutButtonGroups(firstIndex, lastIndex, availableHeight, leftOffset, column);
			}
			double rowWidth = 0;
			if(GetEditorCount(firstIndex, lastIndex) > 0)
				rowWidth = GetMaxSize(firstIndex, lastIndex).Width;
			for(int i = firstIndex; i <= lastIndex; i++) {
				BarItemLinkInfo child = GetChild(i) as BarItemLinkInfo;
				if(!child.Link.ActualIsVisible)
					continue;
				location.Width = child.LinkControl is BarEditItemLinkControl ? rowWidth : child.DesiredSize.Width;
				location.Height = child.DesiredSize.Height;
				RibbonPageGroupItemsPanel.SetRow(child, i - firstIndex);
				RibbonPageGroupItemsPanel.SetColumn(child, column); 
				width = Math.Max(width, location.Width);
				location.Y += location.Height + Ribbon.RowIndent;
			}
			return new Size(width, availableHeight);
		}
	}
	public class OfficeTabletRibbonPageGroupLayoutCalculator : RibbonPageGroupLayoutCalculator {
		public OfficeTabletRibbonPageGroupLayoutCalculator(RibbonPageGroupItemsPanel owner) :
			base(owner) { }
		public override Size MeasureOverride(Size availableSize) {
			Size totalSize = new Size();
			Size measureSize = new Size(double.PositiveInfinity, availableSize.Height);
			MeasureType measureType = RibbonPageGroupsItemsPanel.GetMeasureType(Owner);
			BarItemLinkInfo collapseItem = GetCollapseItem();
			foreach(UIElement child in Owner.Children) {
				child.ClearValue(RibbonControlLayoutHelper.IsItemCollapsedProperty);
				var lc = GetLinkControl(child) as BarItemLinkControl;
				if(lc != null) {
					lc.CurrentRibbonStyle = (measureType == MeasureType.Default || child == collapseItem) ? RibbonItemStyles.SmallWithText : RibbonItemStyles.SmallWithoutText;
					RibbonPageGroupItemsPanel.SetIsEndOfRow(lc, false);
				}
				child.Measure(measureSize);
				if(child == collapseItem)
					continue;
				totalSize.Height = Math.Max(child.DesiredSize.Height, totalSize.Height);
				totalSize.Width += child.DesiredSize.Width;
			}
			if(measureType == MeasureType.Collapsed)
				return MeasureCollapsed(totalSize, availableSize, collapseItem);
			if(measureType == MeasureType.AllowCollapse || measureType == MeasureType.Default)
				totalSize = AdaptToAvailableSize(availableSize, totalSize, collapseItem);
			if(collapseItem != null) {
				if(measureType == MeasureType.AllowHide && availableSize.Width < totalSize.Width && totalSize.Width > collapseItem.DesiredSize.Width) {
					totalSize.Width += collapseItem.DesiredSize.Width;
					totalSize = HideItems(availableSize, totalSize, collapseItem);
				} else {
					RibbonControlLayoutHelper.SetIsItemCollapsed(collapseItem, true);
				}
			}
			return totalSize;
		}
		public override Size ArrangeOverride(Size finalSize) {
			Rect arrangeRect = new Rect(new Size(0d, finalSize.Height));
			var children = GetChildrenWithLastCollapseItem();
			foreach(FrameworkElement child in children) {
				arrangeRect.Width = child.DesiredSize.Width;
				child.Arrange(arrangeRect);
				arrangeRect.X += child.ActualWidth;
			}
			return finalSize;
		}
		IEnumerable<UIElement> GetChildrenWithLastCollapseItem() {
			var collapsedItem = GetCollapseItem();
			var children = Owner.Children.Cast<UIElement>().ToList();
			if(collapsedItem != null) {
				children.Remove(collapsedItem);
				children.Add(collapsedItem);
			}
			return children;
		}
		protected virtual Size AdaptToAvailableSize(Size availableSize, Size currentSize, BarItemLinkInfo collapseItem) {
			double currentWidth = 0d;
			for(int i = Owner.Children.Count - 1; i >= 0 && currentSize.Width > availableSize.Width; i--) {
				UIElement child = Owner.Children[i];
				var lc = GetLinkControl(child) as BarItemLinkControl;
				if(child == collapseItem || lc == null || lc.CurrentRibbonStyle == RibbonItemStyles.SmallWithoutText)
					continue;
				currentWidth = child.DesiredSize.Width;
				lc.CurrentRibbonStyle = RibbonItemStyles.SmallWithoutText;
				child.Measure(availableSize);
				currentSize.Width -= currentWidth - child.DesiredSize.Width;
			}
			Owner.Children.OfType<BarItemLinkInfo>().Where(info => info.DesiredSize.Width > 0 && info != collapseItem).LastOrDefault().Do(info => {
				currentWidth = info.DesiredSize.Width;
				RibbonPageGroupItemsPanel.SetIsEndOfRow(info.LinkControl, true);
				info.Measure(info.DesiredSize);
				currentSize.Width -= currentWidth - info.DesiredSize.Width;
			});
			return currentSize;
		}
		protected Size HideItems(Size availableSize, Size currentSize, BarItemLinkInfo collapseInfo) {
			for(int i = Owner.Children.Count - 1; i >= 0 && currentSize.Width > availableSize.Width; i--) {
				UIElement child = Owner.Children[i];
				if(child == collapseInfo)
					continue;
				currentSize.Width -= child.DesiredSize.Width;
				RibbonControlLayoutHelper.SetIsItemCollapsed(child, true);
			}
			return currentSize;
		}
		protected virtual Size MeasureCollapsed(Size currentSize, Size availableSize, BarItemLinkInfo collapseItem) {
			if(collapseItem == null)
				return currentSize;
			foreach(UIElement child in Owner.Children) {
				if(child == collapseItem)
					continue;
				RibbonControlLayoutHelper.SetIsItemCollapsed(child, true);
			}
			return collapseItem.DesiredSize;
		}
		BarItemLinkInfo GetCollapseItem() {
			if(PageGroupControl == null || PageGroupControl.IsOrigin || PageGroupControl.CollapseButtonInfo.Link.LinkControl == null)
				return null;
			return Owner.Children.OfType<BarItemLinkInfo>().FirstOrDefault(linkInfo => linkInfo.Link == PageGroupControl.CollapseButtonInfo.Link);
		}
	}
}
