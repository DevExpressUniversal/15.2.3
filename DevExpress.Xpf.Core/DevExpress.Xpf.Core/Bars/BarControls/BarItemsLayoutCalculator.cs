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
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Bars {
	public abstract class BarItemsLayoutCalculator {
		public static BarItemsLayoutCalculator CreatePanel(Panel panel, BarControl barControl) {
			return barControl.Bar.GetIsMultiLine() ? new MultilineBarItemsLayoutCalculator(panel, barControl) : (BarItemsLayoutCalculator)new SingleLineBarItemsLayoutCalculator(panel, barControl);
		}
		public BarControl BarControl { get; private set; }
		public Bar Bar { get { return BarControl != null ? BarControl.Bar : null; } }
		protected Panel Panel { get; private set; }
		protected IEnumerable<BarItemLinkInfo> BarItems { get; set; }
		protected IEnumerable<BarItemLinkInfo> BarItemsLeft { get; set; }
		protected IEnumerable<BarItemLinkInfo> BarItemsRight { get; set; }
		protected Size lastContstaint = Size.Empty;
		public BarItemsLayoutCalculator(Panel panel, BarControl barControl) {
			Panel = panel;
			BarControl = barControl;
		}
		public virtual Size MeassureBar(Size constraint) {
			if (Bar == null)
				return new Size(0, 0);
			BarItems = GetBarItemsList();
			BarItemsLeft = GetBarItemsLeft(BarItems);
			BarItemsRight = GetBarItemsRight(BarItems);
			SetItemsVisible();
			HideBarEditItemsInVerticalOrientation();
			foreach (var item in BarItems) {
				item.Measure(SizeHelper.Infinite);
			}
			Size result = MeassureCore(constraint);
			lastContstaint = constraint;
			return result;
		}
		public virtual Size ArrangeBar(Size arrangeBounds) {
			if (Bar == null)
				return arrangeBounds;
			UpdateItemsRowHeight();
			return ArrangeCore(arrangeBounds);
		}
		public virtual double CalcMinWidth() {
			double leftMinWidth = 0.0;
			double rightMinWidth = 0.0;
			if (BarItemsLeft == null || BarItemsRight == null) {
				BarItems = GetBarItemsList();
				BarItemsLeft = GetBarItemsLeft(BarItems);
				BarItemsRight = GetBarItemsRight(BarItems);
			}
			leftMinWidth = GetOccupiedMinArea(BarItemsLeft) + Panel.Margin.Left;
			rightMinWidth = GetOccupiedMinArea(BarItemsRight) + Panel.Margin.Right;
			return leftMinWidth + rightMinWidth;
		}
		public virtual double CalcMaxWidth() {
			if(BarItems == null)
				BarItems = GetBarItemsList();
			return GetLineWidth(BarItems) + Panel.Margin.Left + Panel.Margin.Right;
		}
		protected abstract void ArrangeRightBarItems(Size arrangeBounds);
		protected abstract void ArrangeLeftBarItems(Size arrangeBounds);
		protected abstract Size MeassureLeftBarItems(Size constraint);
		protected abstract Size MeassureRightBarItems(Size constraint);
		protected virtual Size ArrangeCore(Size arrangeBounds) {
			if(BarItemsLeft == null || BarItemsRight == null)
				return arrangeBounds;
			ArrangeLeftBarItems(arrangeBounds);
			ArrangeRightBarItems(arrangeBounds);
			return arrangeBounds;
		}
		protected virtual Size MeassureCore(Size constraint) {
			Size leftItemsSize = MeassureLeftBarItems(constraint);
			double availableWidth = Math.Max(constraint.Width - leftItemsSize.Width, 0d);
			Size rightItemsSize = MeassureRightBarItems(new Size(availableWidth, constraint.Height));
			return GetDesiredSize(leftItemsSize, rightItemsSize);
		}
		protected virtual double CalcRowHeight(int rowIndex) {
			double res = 0;
			for (int i = 0; i < BarControl.Items.Count; i++) {
				BarItemLinkInfo info = (BarItemLinkInfo)BarControl.Items[i];
				if (info.IsHidden())
					continue;
				if (info.LinkControl != null && GetRowIndex(info) == rowIndex)
					res = Math.Max(res, info.DesiredSize.Height);
			}
			return res;
		}
		protected virtual IEnumerable<BarItemLinkInfo> GetBarItemsList() {
			return Panel.Children.OfType<BarItemLinkInfo>();
		}
		protected virtual IEnumerable<BarItemLinkInfo> GetBarItemsLeft(IEnumerable<BarItemLinkInfo> barItems) {
			return barItems.Except(GetBarItemsRight(barItems));
		}
		protected virtual IEnumerable<BarItemLinkInfo> GetBarItemsRight(IEnumerable<BarItemLinkInfo> barItems) {
			return barItems.Where(item => GetAlignment(item) == BarItemAlignment.Far);
		}
		protected virtual double GetRowLineHeight(IEnumerable<BarItemLinkInfo> line) {
			double res = 0;
			double resultValue = 0d;
			for (int i = 0; i < line.Count(); i++) {
				var linkInfo = (BarItemLinkInfo)line.ElementAt(i);
				if (linkInfo.IsHidden())
					continue;
				if (CheckGetLineHeight(line, linkInfo, ref resultValue))
					res = Math.Max(res, resultValue);
				else
					res = Math.Max(res, GetRowHeight(linkInfo));
			}
			return res;
		}
		protected virtual double GetLineHeight(IEnumerable<BarItemLinkInfo> line) {
			double res = 0;
			double resultValue = 0d;
			for (int i = 0; i < line.Count(); i++) {
				var linkInfo = line.ElementAt(i);
				if (linkInfo.IsHidden())
					continue;
				if (CheckGetLineHeight(line, linkInfo, ref resultValue))
					res = Math.Max(res, resultValue);
				else
					res = Math.Max(res, (linkInfo.DesiredSize.Height)); 
			}
			return res;
		}
		protected virtual double GetLineWidth(IEnumerable<BarItemLinkInfo> line) {
			var visibleItems = line.Where(elem => !elem.IsHidden());
			return visibleItems.Count() > 0 ? visibleItems.Sum(elem => elem.DesiredSize.Width + Bar.BarItemHorzIndent) : 0d;
		}
		protected virtual void UpdateItemsRowHeight() {
			int rowIndex = 0;
			var coll = BarItems.Select(x => GetRowIndex(x));
			BarControl.RowCount = coll.Count() == 0 ? 0 : coll.Max() + 1;
			double rowHeight = 0.0;
			while (rowIndex < BarControl.RowCount) {
				rowHeight = CalcRowHeight(rowIndex);
				for (int i = 0; i < BarControl.Items.Count; i++) {
					BarItemLinkInfo info = (BarItemLinkInfo)BarControl.Items[i];
					if (info.LinkControl == null || info.IsHidden() || GetRowIndex(info) != rowIndex)
						continue;
					SetRowHeight(info, rowHeight);
				}
				rowIndex++;
			}
		}
		protected virtual double GetOccupiedMinArea(IEnumerable<BarItemLinkInfo> barItems) {
			var visibleItems = barItems.Where(item => !item.IsHidden());
			return visibleItems.Count() > 0 ? visibleItems.Max(item => item.DesiredSize.Width) : 0d;
		}
		protected bool CheckGetLineHeight(IEnumerable<BarItemLinkInfo> line, BarItemLinkInfo linkInfo, ref double resultValue) {
			var separatorControl = linkInfo.LinkControl as BarItemLinkSeparatorControl;
			if(separatorControl != null) {
				if(line.Count() != 1) {
					if(separatorControl.LayoutOrientation != Orientation.Horizontal) {
						separatorControl.LayoutOrientation = Orientation.Horizontal;
						linkInfo.InvalidateMeasure();
						separatorControl.InvalidateMeasure();
						linkInfo.Measure(SizeHelper.Infinite);
					}
				} else {
					if(separatorControl.LayoutOrientation == Orientation.Vertical) {
						resultValue = separatorControl.DesiredSize.Height;
						return true;
					}
				}
			}
			return false;
		}
		protected int GetRowIndex(BarItemLinkInfo info) {
			if (info == null || info.LinkControl == null)
				return 0;
			return info.LinkControl.RowIndex;
		}
		protected virtual void SetItemsVisible() {
			for (int i = 0; i < BarControl.Items.Count; i++) {
				BarItemLinkInfo linkInfo = BarControl.Items[i] as BarItemLinkInfo;
				linkInfo.Show();
			}
		}
		protected void SetRowIndex(BarItemLinkInfo info, int rowIndex) {
			if (info.LinkControl != null) {
				info.LinkControl.RowIndex = rowIndex;
			}
		}
		protected void SetRowHeight(BarItemLinkInfo info, double rowHeight) {
			if (info.LinkControl != null) {
				info.LinkControl.RowHeight = rowHeight;
			}
		}
		protected double GetRowHeight(BarItemLinkInfo info) {
			if (info == null || info.LinkControl == null) {
				return 0;
			}
			return info.LinkControl.RowHeight;
		}
		protected BarItemAlignment GetAlignment(BarItemLinkInfo info) {
			BarItemAlignment res = BarItemAlignment.Default;
			BarItemLinkBase link = info.Link ?? info.LinkBase;
			if (link != null)
				res = link.ActualAlignment;
			if (Bar != null && res == BarItemAlignment.Default)
				res = Bar.BarItemsAlignment;
			return res;
		}
		protected Size GetDesiredSize(Size left, Size right) {
			return new Size(left.Width + right.Width, Math.Max(left.Height, right.Height));
		}
		protected void HideBarEditItemsInVerticalOrientation() {
			if (BarControl.ContainerOrientation == Orientation.Vertical) {
				foreach (var linkInfo in BarItems) {
					if (linkInfo.LinkControl is BarEditItemLinkControl && ((BarEditItemLinkControl)linkInfo.LinkControl).ShowInVerticalBar != DevExpress.Utils.DefaultBoolean.True)
						linkInfo.Hide();
				}
			}
		}
	}
	public class SingleLineBarItemsLayoutCalculator : BarItemsLayoutCalculator {
		public SingleLineBarItemsLayoutCalculator(Panel panel, BarControl barControl)
			: base(panel, barControl) { }
		protected override Size MeassureCore(Size constraint) {
			for (int i = 0; i < BarItems.Count(); i++)
				SetRowIndex(BarItems.ElementAt(i), 0);
			return base.MeassureCore(constraint);
		}
		protected override Size MeassureRightBarItems(Size constraint) {
			return MeassureBarItems(constraint, BarItemsRight);
		}
		protected override Size MeassureLeftBarItems(Size constraint) {
			return MeassureBarItems(constraint, BarItemsLeft);
		}
		protected override Size ArrangeCore(Size arrangeBounds) {
			double height = Math.Max(GetRowLineHeight(BarItems), arrangeBounds.Height);
			foreach(BarItemLinkInfo linkInfo in BarItems) {
				SetRowHeight(linkInfo, height);
			}
			return base.ArrangeCore(arrangeBounds);
		}
		protected override void ArrangeRightBarItems(Size arrangeBounds) {
			double offset = arrangeBounds.Width;
			int fillItemsCount = GetFillItemsCount();
			double aveWidth = GetAvailableWidthForFillItems(arrangeBounds, fillItemsCount);
			for(int i = BarItemsRight.Count() - 1; i >= 0; i--) {
				var linkInfo = BarItemsRight.ElementAt(i);
				if(linkInfo.IsHidden())
					continue;
				UIElement elem = linkInfo.LinkControl as UIElement;
				double width = GetElementWidth(elem, aveWidth, fillItemsCount);
				double height = GetRowHeight(linkInfo);
				BarItemLinkSeparatorControl separator = elem as BarItemLinkSeparatorControl;
				if(separator != null && separator.LayoutOrientation == Orientation.Vertical) {
					height = width;
					width = separator.RowHeight;
				}
				linkInfo.Arrange(new Rect(new Point(offset - width, 0), new Size(width, height)));
				offset -= width + Bar.BarItemHorzIndent;
			}
		}
		protected override void ArrangeLeftBarItems(Size arrangeBounds) {
			double offset = 0d;
			int fillItemsCount = GetFillItemsCount();
			double aveWidth = GetAvailableWidthForFillItems(arrangeBounds, fillItemsCount);
			for(int i = 0; i < BarItemsLeft.Count(); i++) {
				var linkInfo = BarItemsLeft.ElementAt(i);
				if(linkInfo.IsHidden())
					continue;
				UIElement elem = linkInfo.LinkControl as UIElement;
				double width = GetElementWidth(elem, aveWidth, fillItemsCount);
				double height = GetRowHeight(linkInfo);
				BarItemLinkSeparatorControl separator = elem as BarItemLinkSeparatorControl;
				if(separator != null && separator.LayoutOrientation == Orientation.Vertical) {
					height = width;
					width = separator.RowHeight;
				}
				linkInfo.Arrange(new Rect(new Point(offset, 0), new Size(width, height)));
				offset += width + Bar.BarItemHorzIndent;
			}
		}
		protected virtual bool AllowFillItem(UIElement elem) {
			ISupportAutoSize auto = elem as ISupportAutoSize;
			return auto != null && auto.GetAutoSize() == BarItemAutoSizeMode.Fill && Bar.IsUseWholeRow;
		}
		protected virtual double GetAvailableWidthForFillItems(Size arrangeBounds, int fillItemsCount) {
			if(fillItemsCount == 0)
				return 0;
			double res = arrangeBounds.Width;
			for(int i = 0; i < BarItems.Count(); i++) {
				var linkInfo = BarItems.ElementAt(i);
				if(!AllowFillItem(linkInfo.LinkControl))
					res -= linkInfo.DesiredSize.Width - Bar.BarItemHorzIndent;
			}
			return res;
		}
		protected virtual int GetFillItemsCount() {
			int res = 0;
			for(int i = 0; i < BarItems.Count(); i++)
				if(AllowFillItem(BarItems.ElementAt(i).LinkControl))
					res++;
			return res;
		}
		protected virtual double GetElementWidth(UIElement elem, double aveWidth, int fillItemsCount) {
			if(elem == null)
				return 0;
			double width = elem.DesiredSize.Width;
			if(AllowFillItem(elem)) {
				width = Math.Max(((ISupportAutoSize)elem).GetMinWidth(), aveWidth / fillItemsCount);
				if(elem.Visibility == Visibility.Collapsed || elem is BarItemLinkInfo && ((BarItemLinkInfo)elem).IsHidden())
					width = 0;
			}
			return width;
		}
		Size MeassureBarItems(Size constraint, IEnumerable<BarItemLinkInfo> itemsCollection) {
			var items = itemsCollection.ToList();
			if (!double.IsPositiveInfinity(constraint.Width)) {
				while (GetLineWidth(items) > constraint.Width && items.Count() > 0) {
					var item = items.Last();
					SetRowIndex(item, -1);
					item.Hide();
					items.RemoveAt(items.Count - 1);
				}
			}
			return new Size(GetLineWidth(itemsCollection), GetLineHeight(itemsCollection));
		}
	}
	public class MultilineBarItemsLayoutCalculator : BarItemsLayoutCalculator {
		protected IEnumerable<IEnumerable<BarItemLinkInfo>> RightLines { get; set; }
		protected IEnumerable<IEnumerable<BarItemLinkInfo>> LeftLines { get; set; }
		public MultilineBarItemsLayoutCalculator(Panel panel, BarControl barControl)
			: base(panel, barControl) { }
		protected override Size MeassureCore(Size constraint) {
			Size rightItemsSize = MeassureRightBarItems(constraint);
			Size leftItemsSize = MeassureLeftBarItems(constraint);
			Size desiredSize = GetDesiredSize(leftItemsSize, rightItemsSize);
			if (desiredSize.Width > constraint.Width) {
				double minRightWidth = GetRightMinWidth();
				double minLeftWidth = GetLeftMinWidth();
				var measureSize = new Size(Math.Max(constraint.Width - minRightWidth, minRightWidth), constraint.Height);
				leftItemsSize = MeassureLeftBarItems(measureSize);
				measureSize.Width = Math.Max(Math.Max(constraint.Width - leftItemsSize.Width, minLeftWidth), minRightWidth);
				rightItemsSize = MeassureRightBarItems(measureSize);
				desiredSize = GetDesiredSize(leftItemsSize, rightItemsSize);
			}
			return desiredSize;
		}
		protected override Size MeassureLeftBarItems(Size constraint) {
			LeftLines = GetLinesForLeftItems(constraint.Width);
			foreach(var line in LeftLines) {
				CheckSingleLineSeparatorOrientation(line);
			}
			return CalcLinesSize(LeftLines);
		}
		protected override Size MeassureRightBarItems(Size constraint) {
			RightLines = GetLinesForRightItems(constraint.Width);
			foreach(var line in RightLines) {
				CheckSingleLineSeparatorOrientation(line);
			}
			return CalcLinesSize(RightLines);
		}
		protected override Size ArrangeCore(Size arrangeBounds) {
			if (LeftLines == null || RightLines == null)
				return arrangeBounds;
			return base.ArrangeCore(arrangeBounds);
		}
		protected override void ArrangeLeftBarItems(Size arrangeBounds) {
			ArrangeItemsCore(arrangeBounds, LeftLines, ArrangeLeftBarItemLine);
		}
		protected override void ArrangeRightBarItems(Size arrangeBounds) {
			ArrangeItemsCore(arrangeBounds, RightLines, ArrangeRightBarItemLine);
		}
		void ArrangeItemsCore(Size arrangeBounds, IEnumerable<IEnumerable<BarItemLinkInfo>> lines, Action<IEnumerable<BarItemLinkInfo>, double, Size> arrangeAction) {
			double vOffset = 0;
			if (lines.Count() == 1) {
				foreach (BarItemLinkInfo item in lines.ElementAt(0)) {
					SetRowHeight(item, arrangeBounds.Height);
				}
			}
			for (int i = 0; i < lines.Count(); i++) {
				var line = lines.ElementAt(i);
				arrangeAction(line, vOffset, arrangeBounds);
				vOffset += GetLineHeight(line) + Bar.BarItemVertIndent;
			}
		}
		protected override void UpdateItemsRowHeight() {
			LeftLines.ForEach(line => SetLineRowHeight(line, line.Max(info => info.DesiredSize.Height)));
			RightLines.ForEach(line => SetLineRowHeight(line, line.Max(info => info.DesiredSize.Height)));
		}
		void SetLineRowHeight(IEnumerable<BarItemLinkInfo> line, double rowHeight) {
			line.ForEach(info => SetRowHeight(info, rowHeight));
		}
		protected virtual double GetLeftMinWidth() {
			return LeftLines.Count() > 0 ? LeftLines.Max(line => GetOccupiedMinArea(line)) : 0d;
		}
		protected virtual double GetRightMinWidth() {
			return RightLines.Count() > 0 ? RightLines.Max(line => GetOccupiedMinArea(line)) : 0d;
		}
		protected virtual void ArrangeRightBarItemLine(IEnumerable<BarItemLinkInfo> line, double vOffset, Size arrangeBounds) {
			ArrangeBarItemLine(line, vOffset, arrangeBounds, arrangeBounds.Width - GetLineWidth(line));
		}
		protected virtual void ArrangeLeftBarItemLine(IEnumerable<BarItemLinkInfo> line, double vOffset, Size arrangeBounds) {
			ArrangeBarItemLine(line, vOffset, arrangeBounds, 0d);
		}
		protected virtual Size CalcLinesSize(IEnumerable<IEnumerable<BarItemLinkInfo>> lines) {
			int linesCount = lines.Count();
			if(linesCount == 0)
				return new Size();
			double maxLineWidth = lines.Max(line => GetLineWidth(line));
			double maxLineHeight = lines.Max(line => GetLineHeight(line));
			return new Size(maxLineWidth, maxLineHeight * linesCount);
		}
		protected virtual void CheckSingleLineSeparatorOrientation(IEnumerable<BarItemLinkInfo> list) {
			if (list.Count() == 1) {
				var linkInfo = list.ElementAt(0);
				BarItemLinkSeparatorControl separatorControl = linkInfo.LinkControl as BarItemLinkSeparatorControl;
				if (separatorControl != null) {
					separatorControl.LayoutOrientation = Orientation.Vertical;
					separatorControl.InvalidateMeasure();
					linkInfo.InvalidateMeasure();
					linkInfo.Measure(SizeHelper.Infinite);
				}
			}
		}
		protected virtual IEnumerable<IEnumerable<BarItemLinkInfo>> GetLinesForRightItems(double availWidth) {
			return SplitToRightLines(availWidth, BarItemsRight);
		}
		protected virtual IEnumerable<IEnumerable<BarItemLinkInfo>> GetLinesForLeftItems(double availWidth) {
			return SplitToLeftLines(availWidth, BarItemsLeft);
		}
		Size ArrangeBarItemLine(IEnumerable<BarItemLinkInfo> line, double vOffset, Size arrangeBounds, double zero = 0d) {
			double offset = 0;
			double width;
			for(int i = 0; i < line.Count(); i++) {
				BarItemLinkInfo linkInfo = line.ElementAt(i);
				if(linkInfo.IsHidden())
					continue;
				width = linkInfo.DesiredSize.Width;
				double height = GetRowHeight(linkInfo);
				if(line.Count() == 1 && linkInfo.LinkControl is BarItemLinkSeparatorControl) {
					BarItemLinkSeparatorControl separator = (BarItemLinkSeparatorControl)linkInfo.LinkControl;
					if(separator.LayoutOrientation == Orientation.Vertical) {
						height = width;
						width = separator.RowHeight;
					}
				}
				linkInfo.Arrange(new Rect(new Point(offset + zero, vOffset), new Size(width, height)));
				offset += width + Bar.BarItemHorzIndent;
			}
			return arrangeBounds;
		}
		IEnumerable<IEnumerable<BarItemLinkInfo>> SplitToLeftLines(double availWidth, IEnumerable<BarItemLinkInfo> line) {
			if (GetOccupiedMinArea(line) > availWidth) {
				List<IEnumerable<BarItemLinkInfo>> res = new List<IEnumerable<BarItemLinkInfo>>();
				res.Add(line);
				return res;
			}
			List<List<BarItemLinkInfo>> result = new List<List<BarItemLinkInfo>>();
			int idx = 0;
			while (idx < line.Count()) {
				result.Add(new List<BarItemLinkInfo>());
				double lineWidth = 0d;
				while (lineWidth <= availWidth) {
					if (idx >= line.Count()) break;
					var elem = line.ElementAt(idx);
					if ((lineWidth + elem.DesiredSize.Width) > availWidth) break;
					result[result.Count - 1].Add(elem);
					lineWidth += elem.DesiredSize.Width;
					idx++;
				}
			}
			return result;
		}
		IEnumerable<IEnumerable<BarItemLinkInfo>> SplitToRightLines(double availWidth, IEnumerable<BarItemLinkInfo> line) {
			if (GetOccupiedMinArea(line) > availWidth) {
				List<IEnumerable<BarItemLinkInfo>> res = new List<IEnumerable<BarItemLinkInfo>>();
				res.Add(line);
				return res;
			}
			List<List<BarItemLinkInfo>> result = new List<List<BarItemLinkInfo>>();
			int idx = line.Count() - 1;
			while (idx >= 0) {
				result.Add(new List<BarItemLinkInfo>());
				double lineWidth = 0d;
				while (lineWidth <= availWidth) {
					if (idx < 0) break;
					var elem = line.ElementAt(idx);
					if ((lineWidth + elem.DesiredSize.Width) > availWidth) break;
					result[result.Count - 1].Add(elem);
					lineWidth += elem.DesiredSize.Width;
					idx--;
				}
			}
			return result;
		}
	}
}
