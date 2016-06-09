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
using System.Text;
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.ComponentModel;
using System.Windows.Input;
using DevExpress.Xpf.Bars.Themes;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Bars.Automation;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	public class SubMenuClientPanelBase : Panel {
		public SubMenuClientPanelBase() {
		}
		SubMenuBarControl subMenuControl = null;
		GlyphSidePanel panel = null;
		PopupMenuBase menuBase = null;
		protected internal SubMenuBarControl SubMenuControl {
			get {
				if(subMenuControl == null)
					subMenuControl = LayoutHelper.FindParentObject<SubMenuBarControl>(this);
				return subMenuControl;
			}
		}		
		protected internal GlyphSidePanel Panel {
			get {
				if(SubMenuControl==null) return null;
				if(panel == null)
					panel = SubMenuControl.GlyphSidePanel;
				return panel;
			}
		}
		protected internal PopupMenuBase MenuBase {
			get {
				if(menuBase == null)
					menuBase = GetPopupMenuBase(Panel, SubMenuControl);
				return menuBase;
			}
		}
		internal int DesiredColumnsCount { get; set; }
		internal double DesiredColumnWidth { get; set; }
		internal double DesiredColumnHeight { get; set; }
		internal double ColumnSplitterWidth { get; set; }
		internal Size SplitterDesiredSize { get; set; }
		internal double ResultWidth { get; set; }
		protected override Size MeasureOverride(Size availableSize) {
			double maxColumnWidth = 0d;
			if(MenuBase == null) return new Size();
			int maxRowsCount = MenuBase.MaxRowCount;
			double maxHeight = MenuBase.MaxColumnHeight;
			for(int calcBestWidthAttempt=0; calcBestWidthAttempt<3; calcBestWidthAttempt++) {				
				for (int i = 0; i < Children.Count; i++) {
					BarItemLinkInfo info = Children[i] as BarItemLinkInfo;
					if (info == null) continue;
					SubMenuClientPanelHelper.SetSkipArrange(info, false);
					var mh = (info.LinkControl as BarItemLinkMenuHeaderControl).If(x => x.ActualItemsOrientation == HeaderOrientation.Horizontal);
					info.Measure(SizeHelper.Infinite);
					if (calcBestWidthAttempt >= 1) {
						if (mh == null)
							continue;
						info.Measure(new Size(maxColumnWidth == 0d ? double.PositiveInfinity : maxColumnWidth, double.PositiveInfinity));
					}
					maxColumnWidth = Math.Max(info.DesiredSize.Width, maxColumnWidth);
				}
			}
			maxColumnWidth = Math.Ceiling(maxColumnWidth);
			int currentRow = 0;
			double currentColumnHeight = 0;
			DesiredColumnHeight = 0;
			DesiredColumnsCount = 1;
			BarItemLinkControlBase previousLinkControl = null;
			for(int i = 0; i < Children.Count; i++) {
				BarItemLinkInfo info = Children[i] as BarItemLinkInfo;
				if(info == null) continue;
				if(info.LinkControl is BarItemLinkMenuHeaderControl && ((BarItemLinkMenuHeaderControl)info.LinkControl).ActualItemsOrientation == HeaderOrientation.Horizontal)
					info.Measure(new Size(maxColumnWidth, double.PositiveInfinity));
				if(!(info.LinkControl is BarItemLinkSeparatorControl)) 
					currentRow++;
				currentColumnHeight += info.DesiredSize.Height;
				var currentItemAsMenuHeader = info.LinkControl as BarItemLinkMenuHeaderControl;
				var previousItemAsMenuHeader = previousLinkControl as BarItemLinkMenuHeaderControl;
				var previousItemAsSeparator = previousLinkControl as BarItemLinkSeparatorControl;
				var currentItemAsSeparator = info.LinkControl as BarItemLinkSeparatorControl;
				if(currentRow > maxRowsCount || currentColumnHeight >= maxHeight) {
					if(previousItemAsMenuHeader != null) { 
						previousItemAsMenuHeader.RowPosition = currentRow == 1 ? VerticalAlignment.Stretch : VerticalAlignment.Bottom;
					}
					if(previousItemAsSeparator != null) { 
						SubMenuClientPanelHelper.SetSkipArrange(previousItemAsSeparator.LinkInfo, true);
					}
					if(currentItemAsSeparator != null) { 
						SubMenuClientPanelHelper.SetSkipArrange(currentItemAsSeparator.LinkInfo, true);
					}
					currentRow = 1;
					DesiredColumnHeight = Math.Max(DesiredColumnHeight, currentColumnHeight - info.DesiredSize.Height);
					currentColumnHeight = info.DesiredSize.Height;
					DesiredColumnsCount++;
				}
				if(currentItemAsMenuHeader != null) {
					if(previousItemAsMenuHeader != null) {
						previousItemAsMenuHeader.PrecedesHeader = true;
					}
					currentItemAsMenuHeader.PrecedesHeader = false;
					currentItemAsMenuHeader.RowPosition = currentRow == 1 ? VerticalAlignment.Top : VerticalAlignment.Center;
					currentItemAsMenuHeader.ColumnPosition = DesiredColumnsCount == 1 ? HorizontalAlignment.Left : System.Windows.HorizontalAlignment.Center;
					if(previousItemAsSeparator!=null) 
						SubMenuClientPanelHelper.SetSkipArrange(previousItemAsSeparator.LinkInfo, true);
				}
				if(previousItemAsMenuHeader != null && currentItemAsSeparator != null &&
					(previousItemAsMenuHeader.ActualItemsOrientation != HeaderOrientation.Vertical || previousItemAsMenuHeader.IsEmpty))
					SubMenuClientPanelHelper.SetSkipArrange(currentItemAsSeparator, true);
				SubMenuClientPanelHelper.SetColumnIndex(info, DesiredColumnsCount);
				maxColumnWidth = Math.Max(info.DesiredSize.Width, maxColumnWidth);
				previousLinkControl = info.LinkControl;
			}
			maxColumnWidth = Math.Ceiling(maxColumnWidth);
			for(int i = Children.Count - 1; i >= 0; i--) {
				if(SubMenuClientPanelHelper.GetColumnIndex(Children[i]) != DesiredColumnsCount) break;
				var menuHeader = TryGetLinkControlAtIndex<BarItemLinkMenuHeaderControl>(i);
				if(menuHeader == null) continue;
				menuHeader.ColumnPosition = DesiredColumnsCount == 1 ? HorizontalAlignment.Stretch : HorizontalAlignment.Right;
				if(i == Children.Count - 1) menuHeader.RowPosition = System.Windows.VerticalAlignment.Bottom;
			}
			PopupMenuColumnSplitter splitter = new PopupMenuColumnSplitter();
			SplitterDesiredSize = panel == null ? new Size(0, 0) : panel.MeasureSplitter(maxHeight, splitter);
			DesiredColumnHeight = Math.Max(DesiredColumnHeight, currentColumnHeight);
			DesiredColumnWidth = maxColumnWidth;
			ResultWidth = DesiredColumnWidth * DesiredColumnsCount + (DesiredColumnsCount - 1) * SplitterDesiredSize.Width;
			Size result = new Size(Math.Ceiling(ResultWidth), DesiredColumnHeight);
			if (result != DesiredSize && SubMenuControl != null && SubMenuControl.ScrollViewer != null) {
				Bars.Helpers.BarLayoutHelper.InvalidateMeasureTree(VisualTreeHelper.GetParent(this) as UIElement, SubMenuControl.ScrollViewer);
			}
			return result;
		}
		T TryGetLinkControlAtIndex<T>(int index) where T : BarItemLinkControlBase {
			if(index < 0 || Children.Count <= index) return null;
			BarItemLinkInfo linkInfo = Children[index] as BarItemLinkInfo;
			return linkInfo.LinkControl as T;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(MenuBase == null) return new Size();
			double arrangeWidth = Math.Max(ResultWidth, finalSize.Width);
			double arrangeHeight = Math.Max(finalSize.Height, DesiredSize.Height);
			double columnWidth = (arrangeWidth - (DesiredColumnsCount - 1) * SplitterDesiredSize.Width) / (double)DesiredColumnsCount;
			List<UIElement> column = new List<UIElement>();
			int currentColumnIndex = 1;
			DesiredColumnHeight = finalSize.Height;
			List<RectEx> glyphSidePanelInfo = new List<RectEx>();
			for(int i = 0; i < Children.Count; i++) {
				UIElement child = Children[i];
				int newColumnIndex = SubMenuClientPanelHelper.GetColumnIndex(child);
				if(newColumnIndex != currentColumnIndex) {
					glyphSidePanelInfo.AddRange(ArrangeColumn(columnWidth, column, currentColumnIndex, false));
					column.Clear();
					currentColumnIndex++;
				}
				if(!SubMenuClientPanelHelper.GetSkipArrange(child))
					column.Add(child);
				else
					child.Arrange(new Rect(new Point(double.MinValue, double.MinValue), child.DesiredSize));
			}
			glyphSidePanelInfo.AddRange(ArrangeColumn(columnWidth, column, currentColumnIndex, true));
			if (Panel != null)
				Panel.Update(arrangeWidth, arrangeHeight, DesiredColumnsCount, glyphSidePanelInfo, DesiredColumnWidth);
			return new Size(arrangeWidth, arrangeHeight);
		}
		List<RectEx> ArrangeColumn(double columnWidth, List<UIElement> column, int currentColumnIndex, bool isLastRow) {
			List<RectEx> glyphSidePanelInfo = new List<RectEx>();
			double columnCoeff = 1d;
			if(MenuBase.StretchRows && !isLastRow) {
				double desiredColumnHeight = 0d;
				for(int cIndex = 0; cIndex < column.Count; cIndex++) {
					desiredColumnHeight += column[cIndex].DesiredSize.Height;
				}
				columnCoeff = DesiredColumnHeight / Math.Max(desiredColumnHeight, 1d);
			}
			double leftOffset = (currentColumnIndex - 1) * (columnWidth + SplitterDesiredSize.Width);
			double topOffset = 0d;
			double lastBarItemLinkInfoBottomY = 0d;
			for(int cIndex = 0; cIndex < column.Count; cIndex++) {
				var menuHeaderControl = ((BarItemLinkInfo)column[cIndex]).LinkControl as BarItemLinkMenuHeaderControl;
				double rowDesiredSize = column[cIndex].DesiredSize.Height * columnCoeff;
				column[cIndex].Arrange(new Rect(leftOffset, topOffset, columnWidth, rowDesiredSize));
				topOffset += rowDesiredSize;
				if(menuHeaderControl != null) {
					if(!LayoutDoubleHelper.AreClose(0, topOffset - rowDesiredSize - lastBarItemLinkInfoBottomY))
						glyphSidePanelInfo.Add(new RectEx(
							leftOffset,
							lastBarItemLinkInfoBottomY,
							SubMenuControl.GlyphSidePanelWidth,
							topOffset - rowDesiredSize - lastBarItemLinkInfoBottomY,
							currentColumnIndex == 1 && glyphSidePanelInfo.Count == 0 ? VerticalAlignment.Top : System.Windows.VerticalAlignment.Center
							));
					lastBarItemLinkInfoBottomY = topOffset;
				}
			}
			if(!LayoutDoubleHelper.AreClose(0, DesiredColumnHeight - lastBarItemLinkInfoBottomY))
				glyphSidePanelInfo.Add(new RectEx(
					leftOffset,
					lastBarItemLinkInfoBottomY,
					SubMenuControl.GlyphSidePanelWidth,
					DesiredColumnHeight - lastBarItemLinkInfoBottomY,
					currentColumnIndex == 1 ? (glyphSidePanelInfo.Count == 0 ? VerticalAlignment.Stretch : VerticalAlignment.Bottom) : System.Windows.VerticalAlignment.Center));
			return glyphSidePanelInfo;
		}		
		private PopupMenuBase GetPopupMenuBase(GlyphSidePanel panel = null, SubMenuBarControl subMenu = null) {
			if(subMenu == null)
				subMenu = SubMenuControl;
			PopupMenuBase popup = null;
			if(panel != null && panel.Popup != null)
				popup = panel.Popup;
			else
				if(subMenu != null && subMenu.ItemLinks != null) {
					ILinksHolder holder = subMenu.LinksHolder;
					if(holder is PopupMenuBase)
						popup = holder as PopupMenuBase;
				}
			if(popup == null) {
				BarPopupBorderControl borderControl = LayoutHelper.FindParentObject<BarPopupBorderControl>(this);
				if(borderControl == null)
					return null;
				popup = borderControl.Parent as PopupMenuBase;
				if(popup == null)
					return null;
			}
			return popup;
		}
	}
	public class SubMenuClientPanelHelper {
		public static int GetColumnIndex(DependencyObject obj) {
			return (int)obj.GetValue(ColumnIndexProperty);
		}
		public static void SetColumnIndex(DependencyObject obj, int value) {
			obj.SetValue(ColumnIndexProperty, value);
		}		
		public static int GetRowIndex(DependencyObject obj) {
			return (int)obj.GetValue(RowIndexProperty);
		}
		public static void SetRowIndex(DependencyObject obj, int value) {
			obj.SetValue(RowIndexProperty, value);
		}
		public static bool GetSkipArrange(DependencyObject obj) {
			return (bool)obj.GetValue(SkipArrangeProperty);
		}
		public static void SetSkipArrange(DependencyObject obj, bool value) {
			obj.SetValue(SkipArrangeProperty, value);
		}
		public static readonly DependencyProperty SkipArrangeProperty =
			DependencyPropertyManager.RegisterAttached("SkipArrange", typeof(bool), typeof(SubMenuClientPanelHelper), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty RowIndexProperty =
			DependencyPropertyManager.RegisterAttached("RowIndex", typeof(int), typeof(SubMenuClientPanelHelper), new FrameworkPropertyMetadata(0));
		public static readonly DependencyProperty ColumnIndexProperty =
			DependencyPropertyManager.RegisterAttached("ColumnIndex", typeof(int), typeof(SubMenuClientPanelHelper), new FrameworkPropertyMetadata(0));
	}
	public class SubMenuClientPanel : SubMenuClientPanelBase {
		public LinksControl Owner { get { return LayoutHelper.FindParentObject<LinksControl>(this); } }
		public SubMenuClientPanel() { }
		protected override Size MeasureOverride(Size constraint) {
			if(Owner != null) Owner.CalculateMaxGlyphSize();
			Size res = base.MeasureOverride(constraint);
			if(!double.IsPositiveInfinity(constraint.Width))
				res.Width = constraint.Width;
			return res;
		}
	}
	struct RectEx {
		double x;
		double y;
		double width;
		double height;				
		public RectEx(double x, double y, double width, double height, VerticalAlignment position){
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			verticalPosition = position;
		}
		VerticalAlignment verticalPosition;
		public VerticalAlignment VerticalPosition { get { return verticalPosition; } }
		public static implicit operator Rect(RectEx rX) {
			return new Rect(rX.x, rX.y, Math.Max(0,rX.width), Math.Max(0, rX.height));
		}
	}
}
