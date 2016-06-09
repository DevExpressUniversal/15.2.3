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
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.Web.Design {
	public static class LayoutCellDrawHelper {
		public const int DefaultEditorHeight = 24;
		public const int CellPadding = 18;
		public const int BorderSize = 1;
		public const int LayoutCellContentMargin = BorderSize + CellPadding;
		public const int DefaultCellHeight = DefaultEditorHeight + LayoutCellContentMargin * 2;
		public static Color BorderColor = Color.FromArgb(255, 255, 255);
		public static Color SelectedBorderColor = Color.FromArgb(150, 150, 150);
		public static Color BackColor = Color.FromArgb(238, 238, 238);
		public static Color SelectedBackColor = Color.FromArgb(246, 246, 246);
		public static Color BorderEditorColor = Color.FromArgb(203, 203, 203);
		public static Rectangle GetInternalRectangle(Rectangle rect) {
			return new Rectangle(rect.X + LayoutCellContentMargin, rect.Y + LayoutCellContentMargin,
				rect.Width - LayoutCellContentMargin * 2, rect.Height - LayoutCellContentMargin * 2);
		}
		public static void Draw(LayoutGroupMap groupMap, Rectangle rectangle, Graphics graphics, bool isSelected) {
			DrawItemBase(rectangle, graphics, isSelected);
		}
		static void DrawItemBase(Rectangle rect, Graphics graphics, bool isSelected) {
			var cellRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
			var backColor = isSelected ? SelectedBackColor : BackColor;
			using(var brush = new SolidBrush(backColor)) {
				var inflateSize = -BorderSize;
				cellRect.Inflate(inflateSize, inflateSize);
				graphics.FillRectangle(brush, cellRect);
			}
			using(var pen = GetBorderPen(isSelected))
				graphics.DrawRectangle(pen, cellRect);
		}
		static Pen GetBorderPen(bool isSelected) {
			if(isSelected)
				return new Pen(SelectedBorderColor, BorderSize) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
			return new Pen(BorderColor, BorderSize);
		}
	}
	public static class LayoutItemDrawHelper {
		public const int FontSize = 10;
		public const string FontName = "Tahoma";
		const int CaptionDistance = 6;
		const int MarkHorzShiftRelativelyOfCaption = -3;
		const int VerticalCaptionShift = 3;
		const int BorderWidth = 1;
		const int CellHeightForVerticalCaptionLocation = 60;
		static Rectangle GetInternalRectangle(LayoutItem layoutItem, Rectangle rect) {
			int itemRectVerticalShift = (rect.Height - GetLayoutItemCellHeight(layoutItem)) / 2;
			return LayoutCellDrawHelper.GetInternalRectangle(new Rectangle(rect.X, rect.Y + itemRectVerticalShift, rect.Width, GetLayoutItemCellHeight(layoutItem)));
		}
		static Rectangle GetEditorRectangle(ASPxFormLayout formLayout, LayoutItem layoutItem, Rectangle itemRect, Graphics graphics) {
			Rectangle result = new Rectangle(itemRect.X, itemRect.Y, itemRect.Width, LayoutCellDrawHelper.DefaultEditorHeight);
			if(IsCaptionItemRequired(layoutItem)) {
				if(IsCaptionLocatedHorizontally(layoutItem)) {
					int horizontalShift = GetCaptionWidth(formLayout.GetItemCaptionWithMark(layoutItem), graphics) + CaptionDistance;
					if(layoutItem.CaptionSettings.GetLocation() == LayoutItemCaptionLocation.Left)
						result.X = result.X + horizontalShift;
					result.Width = result.Width - horizontalShift;
				}
				if(layoutItem.CaptionSettings.GetLocation() == LayoutItemCaptionLocation.Top)
					result.Y = result.Y + FontSize + CaptionDistance;
			}
			return result;
		}
		public static int GetCaptionWidth(string caption, Graphics graphics) {
			return (int)Math.Ceiling(graphics.MeasureString(caption, GetCaptionFont()).Width);
		}
		public static void Draw(ASPxFormLayout formLayout, LayoutItem layoutItem, Rectangle rect, Graphics graphics) {
			var itemRect = GetInternalRectangle(layoutItem, rect);
			if(IsCaptionItemRequired(layoutItem))
				DrawCaption(formLayout, layoutItem, GetCaptionPoint(formLayout, layoutItem, itemRect, graphics), graphics);
			DrawEditor(layoutItem, GetEditorRectangle(formLayout, layoutItem, itemRect, graphics), graphics);
		}
		static void DrawEditor(LayoutItem layoutItem, Rectangle editorRect, Graphics graphics) {
			using(var pen = new Pen(LayoutCellDrawHelper.BorderEditorColor, BorderWidth)) {
				graphics.DrawRectangle(pen, editorRect);
				editorRect.Inflate(-BorderWidth, -BorderWidth);
				graphics.FillRectangle(Brushes.White, editorRect);
			}
			var nestedControl = layoutItem.GetNestedControl();
			var contentAlias = GetContentAlias(layoutItem);
			if(!string.IsNullOrEmpty(contentAlias)) {
				using(var brush = GetControlNameBrush())
					graphics.DrawString(contentAlias, GetCaptionFont(), brush, GetControlNamePoint(contentAlias, editorRect, graphics));
			}
		}
		static string GetContentAlias(LayoutItem layoutItem) {
			if(layoutItem is CardViewCommandLayoutItem)
				return "Command Buttons";
			if(layoutItem is EditModeCommandLayoutItem)
				return "Edit Command Buttons";
			string controlName = "";
			System.Web.UI.Control firstLayoutItemControl = layoutItem.GetNestedControl();
			if(firstLayoutItemControl != null) {
				controlName = firstLayoutItemControl.GetType().Name;
				if(layoutItem.ContainsSeveralNotLiteralControls())
					controlName += FormLayoutItemsOwner.ThreeDots;
			}
			return controlName;
		}
		static PointF GetControlNamePoint(string controlName, Rectangle editorRect, Graphics graphics) {
			return new PointF(editorRect.X + (editorRect.Width - GetCaptionWidth(controlName, graphics)) / 2, editorRect.Y + (editorRect.Height - FontSize) / 2 - VerticalCaptionShift);
		}
		static PointF GetCaptionPoint(ASPxFormLayout formLayout, LayoutItem layoutItem, Rectangle rect, Graphics graphics) {
			PointF result = new PointF(rect.X, rect.Y);
			if(IsCaptionLocatedHorizontally(layoutItem)) {
				result.Y = result.Y + GetCaptionVerticalShiftRelativelyOfEditor(layoutItem.CaptionSettings.GetVerticalAlign());
				if(layoutItem.CaptionSettings.GetLocation() == LayoutItemCaptionLocation.Right)
					result.X = result.X + rect.Width - GetCaptionWidth(formLayout.GetItemCaptionWithMark(layoutItem), graphics);
			}
			if(IsCaptionLocatedVertically(layoutItem)) {
				result.X = result.X + GetCaptionHorizontalShiftRelativelyOfEditor(formLayout, layoutItem, rect.Width, graphics);
				if(layoutItem.CaptionSettings.GetLocation() == LayoutItemCaptionLocation.Bottom)
					result.Y = result.Y + LayoutCellDrawHelper.DefaultEditorHeight + CaptionDistance;
			}
			result.Y = result.Y - VerticalCaptionShift;
			return result;
		}
		static int GetCaptionHorizontalShiftRelativelyOfEditor(ASPxFormLayout formLayout, LayoutItem layoutItem, int editorWidth, Graphics graphics) {
			switch(layoutItem.CaptionSettings.GetHorizontalAlign()) {
				case FormLayoutHorizontalAlign.Left:
					return 0;
				case FormLayoutHorizontalAlign.Center:
					return (editorWidth - GetCaptionWidth(formLayout.GetItemCaptionWithMark(layoutItem), graphics)) / 2;
				case FormLayoutHorizontalAlign.Right:
					return editorWidth - GetCaptionWidth(formLayout.GetItemCaptionWithMark(layoutItem), graphics);
				default:
					return 0;
			}
		}
		static int GetCaptionVerticalShiftRelativelyOfEditor(FormLayoutVerticalAlign captionVerticalAlign) {
			switch(captionVerticalAlign) {
				case FormLayoutVerticalAlign.Top:
					return 0;
				case FormLayoutVerticalAlign.Middle:
					return (LayoutCellDrawHelper.DefaultEditorHeight - FontSize) / 2;
				case FormLayoutVerticalAlign.Bottom:
					return LayoutCellDrawHelper.DefaultEditorHeight - FontSize;
				default:
					return 0;
			}
		}
		static bool IsCaptionLocatedHorizontally(LayoutItem layoutItem) {
			return IsCaptionItemRequired(layoutItem) && (layoutItem.CaptionSettings.GetLocation() == LayoutItemCaptionLocation.Left || layoutItem.CaptionSettings.GetLocation() == LayoutItemCaptionLocation.Right);
		}
		static bool IsCaptionLocatedVertically(LayoutItem layoutItem) {
			return IsCaptionItemRequired(layoutItem) && (layoutItem.CaptionSettings.GetLocation() == LayoutItemCaptionLocation.Top || layoutItem.CaptionSettings.GetLocation() == LayoutItemCaptionLocation.Bottom);
		}
		static bool IsCaptionItemRequired(LayoutItem layoutItem) { 
			return layoutItem.FormLayout != null ? layoutItem.IsCaptionCellRequired() : !string.IsNullOrEmpty(layoutItem.GetItemCaption());
		}
		public static int GetLayoutItemCellHeight(LayoutItem layoutItem) {
			if(IsCaptionLocatedVertically(layoutItem))
				return CellHeightForVerticalCaptionLocation;
			return LayoutCellDrawHelper.DefaultCellHeight;
		}
		public static void DrawCaption(ASPxFormLayout formLayout, LayoutItem layoutItem, PointF captionPoint, Graphics graphics) {
			Font font = GetCaptionFont();
			using(SolidBrush captionBrush = GetCaptionBrush())
				graphics.DrawString(layoutItem.GetItemCaption(), font, captionBrush, captionPoint);
			string mark = formLayout.GetItemCaptionWithMark(layoutItem).Remove(0, layoutItem.GetItemCaption().Length);
			if(!string.IsNullOrEmpty(mark)) {
				PointF markPoint = new PointF(captionPoint.X + GetCaptionWidth(layoutItem.GetItemCaption(), graphics)
					+ (!string.IsNullOrEmpty(layoutItem.GetItemCaption()) ? MarkHorzShiftRelativelyOfCaption : 0), captionPoint.Y);
				using(SolidBrush markBrush = GetMarkBrush(formLayout, layoutItem))
					graphics.DrawString(mark, font, markBrush, markPoint);
			}
		}
		public static Font GetCaptionFont() {
			return new Font(FontName, FontSize, FontStyle.Regular);
		}
		public static SolidBrush GetCaptionBrush() {
			return new SolidBrush(Color.FromArgb(47, 47, 47));
		}
		public static SolidBrush GetControlNameBrush() {
			return new SolidBrush(Color.Gray);
		}
		public static SolidBrush GetMarkBrush(ASPxFormLayout formLayout, LayoutItem layoutItem) {
			return formLayout.ShowItemRequiredMark(layoutItem) ? new SolidBrush(Color.Green) : new SolidBrush(Color.Gray);
		}
	}
	public static class EmptyLayoutItemDrawHelper {
		static Brush GetBrush(bool isSelected) {
			return new SolidBrush(isSelected ? LayoutCellDrawHelper.SelectedBackColor : Color.LightGray);
		}
		public static void Draw(Graphics graphics, Rectangle rect, bool isSelected) {
			using(Brush emptyLayoutItemBrush = GetBrush(isSelected))
				graphics.FillRectangle(emptyLayoutItemBrush, rect);
		}
	}
	public static class LayoutGroupDrawHelper {
		const int CaptionRectHeight = 10;
		const int CaptionRectLeftPadding = 8;
		const int CaptionLeftPadding = 3;
		const int CaptionRightPadding = 8;
		static Color BorderColor = Color.FromArgb(203, 203, 203);
		public static SolidBrush GetCaptionBrush() {
			return new SolidBrush(Color.Gray);
		}
		public static SelectedBag Draw(LayoutGroup layoutGroup, Rectangle internalRect, Rectangle parentCellRect, Graphics graphics, Point selectionPoint, LayoutItemBase currentSelectedItem) {
			var decoration = layoutGroup.GetGroupBoxDecoration();
			if(decoration != GroupBoxDecoration.None) {
				using(var groupBorderPen = new Pen(BorderColor)) {
					if(decoration == GroupBoxDecoration.Box) {
						graphics.DrawRectangle(groupBorderPen, internalRect);
						if(layoutGroup.Items.IsEmpty) {
							internalRect.Inflate(-1, -1);
							graphics.FillRectangle(Brushes.White, internalRect);
						}
					} else if(decoration == GroupBoxDecoration.HeadingLine) {
						graphics.DrawLine(groupBorderPen, internalRect.X, internalRect.Y, internalRect.Right, internalRect.Y);
					}
				}
				if(layoutGroup.GetShowCaption())
					DrawCaption(layoutGroup, internalRect, graphics);
			}
			return !selectionPoint.IsEmpty && parentCellRect.Contains(selectionPoint) || layoutGroup == currentSelectedItem ? new SelectedBag(layoutGroup, parentCellRect) : null;
		}
		static void DrawCaption(LayoutGroup layoutGroup, Rectangle rect, Graphics graphics) {
			var captionPoint = PointF.Empty;
			var captionFont = GetCaptionFont();
			var captionSize = graphics.MeasureString(layoutGroup.GetItemCaption(), captionFont);
			var decoration = layoutGroup.GetGroupBoxDecoration();
			if(decoration == GroupBoxDecoration.Box) {
				var delta = Convert.ToInt32((LayoutCellDrawHelper.CellPadding + CaptionRectHeight) / 2);
				var captionBackgroundRectangle = new Rectangle(rect.X - delta, rect.Y - delta, (int)captionSize.Width + CaptionLeftPadding + CaptionRightPadding, CaptionRectHeight);
				using(Brush captionBackgroundBrush = new SolidBrush(SystemColors.Control))
					graphics.FillRectangle(captionBackgroundBrush, captionBackgroundRectangle);
				captionPoint = new PointF(captionBackgroundRectangle.X + CaptionLeftPadding, captionBackgroundRectangle.Y + (captionBackgroundRectangle.Height - captionSize.Height) / 2);
			}
			if(decoration == GroupBoxDecoration.HeadingLine)
				captionPoint = new PointF(rect.X + CaptionLeftPadding, rect.Y - captionSize.Height);
			using(var captionBrush = GetCaptionBrush())
				graphics.DrawString(layoutGroup.GetItemCaption(), captionFont, captionBrush, captionPoint);
		}
		static Font GetCaptionFont() {
			return new Font(LayoutItemDrawHelper.FontName, LayoutItemDrawHelper.FontSize, FontStyle.Regular | FontStyle.Bold);
		}
	}
	public static class TabbedLayoutGroupDrawHelper {
		public const int TabHeaderHeight = 32;
		const int TabCaptionHorizontalPadding = 4;
		const int TabCaptionTopPadding = 2;
		const int SpacingBetweenTabs = 0;
		const int FirstTabLeftPadding = 0;
		static Font TabFont = new Font(LayoutItemDrawHelper.FontName, LayoutItemDrawHelper.FontSize, FontStyle.Regular);
		static Font ActiveTabFont = new Font(LayoutItemDrawHelper.FontName, LayoutItemDrawHelper.FontSize - 1, FontStyle.Bold);
		public static SelectedBag Draw(TabbedLayoutGroupMap tabbedGroupMap, Rectangle parentCellRect, Graphics graphics, Point selectionPoint, LayoutItemBase currentSelectedItem) {
			var tabbedGroup = tabbedGroupMap.TabbedGroup;
			var visibleItemsCount = tabbedGroup.Items.GetVisibleItemCount();
			var result = DrawGroupSelection(tabbedGroup, parentCellRect, graphics, selectionPoint, currentSelectedItem, visibleItemsCount);
			if(visibleItemsCount == 0)
				return result;
			var internalRect = LayoutCellDrawHelper.GetInternalRectangle(parentCellRect);
			tabbedGroup.ActiveTabIndex = DrawTabStrip(tabbedGroupMap, internalRect, graphics, selectionPoint, currentSelectedItem);
			var activeTabRect = tabbedGroupMap.TabHeaderRectangles[tabbedGroup.ActiveTabIndex];
			using(var tabbedGroupPen = GetTabbedGroupPen()) {
				graphics.DrawLine(tabbedGroupPen, new Point(internalRect.X, internalRect.Y + TabHeaderHeight), new Point(activeTabRect.X, internalRect.Y + TabHeaderHeight));
				graphics.DrawLine(tabbedGroupPen, new Point(activeTabRect.Right, internalRect.Y + TabHeaderHeight), new Point(internalRect.Right, internalRect.Y + TabHeaderHeight));
				graphics.DrawLine(tabbedGroupPen, new Point(internalRect.Right, internalRect.Y + TabHeaderHeight), new Point(internalRect.Right, internalRect.Bottom));
				graphics.DrawLine(tabbedGroupPen, new Point(internalRect.Left, internalRect.Bottom), new Point(internalRect.Right, internalRect.Bottom));
				graphics.DrawLine(tabbedGroupPen, new Point(internalRect.Left, internalRect.Y + TabHeaderHeight), new Point(internalRect.Left, internalRect.Bottom));
			}
			var activeTabItem = tabbedGroup.Items.GetVisibleItemOrGroup(tabbedGroup.ActiveTabIndex);
			bool isTabSelected = !selectionPoint.IsEmpty && activeTabRect.Contains(selectionPoint) || activeTabItem == currentSelectedItem;
			if(isTabSelected)
				result = new SelectedBag(activeTabItem, new Rectangle(internalRect.X, internalRect.Y + TabHeaderHeight, internalRect.Width, internalRect.Height - TabHeaderHeight));
			return result;
		}
		static SelectedBag DrawGroupSelection(TabbedLayoutGroup tabbedGroup, Rectangle parentCellRect, Graphics graphics, Point selectionPoint, LayoutItemBase currentSelectedItem, int visibleItemsCount) {
			var isSelected = !selectionPoint.IsEmpty && parentCellRect.Contains(selectionPoint) || tabbedGroup == currentSelectedItem;
			if(isSelected) {
				if(visibleItemsCount == 0) {
					using(var brush = new SolidBrush(LayoutCellDrawHelper.SelectedBackColor))
						graphics.FillRectangle(brush, parentCellRect);
				}
				return new SelectedBag(tabbedGroup, parentCellRect);
			}
			return null;
		}
		public static int GetActiveTabIndex(TabbedLayoutGroupMap tabbedGroupMap, Rectangle rectangle, Graphics graphics, Point selectionPoint, LayoutItemBase currentSelectedItem, bool isTabStripShrinked) {
			var tabbedGroup = tabbedGroupMap.TabbedGroup;
			var result = tabbedGroup.ActiveTabIndex;
			tabbedGroupMap.TabHeaderRectangles.Clear();
			bool isTabStripClicked = IsTabStripClicked(tabbedGroup, rectangle, selectionPoint, graphics, isTabStripShrinked);
			for(int i = 0; i < tabbedGroup.Items.GetVisibleItemCount(); i++) {
				var childItem = tabbedGroup.Items.GetVisibleItemOrGroup(i);
				var tabHeaderRectangle = GetTabHeaderRectangle(tabbedGroup, rectangle, i, graphics, isTabStripShrinked);
				var tabClicked = tabbedGroup.ShowGroupDecoration && !selectionPoint.IsEmpty && tabHeaderRectangle.Contains(selectionPoint);
				var tabIsActive = i == result;
				var tabSelected = tabClicked || !isTabStripClicked && tabIsActive || childItem == currentSelectedItem || currentSelectedItem != null && IsFirstItemContainsSecondItem(childItem, currentSelectedItem);
				if(tabSelected)
					result = i;
				if(tabbedGroup.ShowGroupDecoration) {
					tabbedGroupMap.TabHeaderRectangles.Add(tabHeaderRectangle);
					tabbedGroupMap.ItemRectangles.Add(tabHeaderRectangle);
				}
			}
			var lastTabIndex = tabbedGroupMap.TabHeaderRectangles.Count - 1;
			return lastTabIndex >= result ? result : lastTabIndex;
		}
		static int DrawTabStrip(TabbedLayoutGroupMap tabbedGroupMap, Rectangle rectangle, Graphics graphics, Point selectionPoint, LayoutItemBase currentSelectedItem) {
			bool isTabShrinked = false;
			int activeTabIndex = GetActiveTabIndex(tabbedGroupMap, rectangle, graphics, selectionPoint, currentSelectedItem, isTabShrinked);
			var lastIndex = tabbedGroupMap.TabHeaderRectangles.Count - 1;
			if(tabbedGroupMap.TabHeaderRectangles[lastIndex].Right > rectangle.Right) {
				isTabShrinked = true;
				activeTabIndex = GetActiveTabIndex(tabbedGroupMap, rectangle, graphics, selectionPoint, currentSelectedItem, isTabShrinked);
			}
			var tabbedGroup = tabbedGroupMap.TabbedGroup;
			for(int i = 0; i < tabbedGroupMap.TabHeaderRectangles.Count; i++)
				DrawTab(tabbedGroupMap.TabHeaderRectangles[i], tabbedGroup.Items.GetVisibleItemOrGroup(i).Caption, graphics, i == activeTabIndex, isTabShrinked);
			return activeTabIndex;
		}
		static void DrawTab(Rectangle tabRect, string itemCaption, Graphics graphics, bool isActiveTab, bool isTabShrinked) {
			using(var tabBrush = GetTabBrush(isActiveTab))
				graphics.FillRectangle(tabBrush, tabRect);
			using(var tabbedGroupPen = GetTabbedGroupPen()) {
				if(!isActiveTab)
					graphics.DrawRectangle(tabbedGroupPen, tabRect);
				else {
					graphics.DrawLine(tabbedGroupPen, tabRect.Location, new Point(tabRect.Right, tabRect.Top));
					graphics.DrawLine(tabbedGroupPen, tabRect.Location, new Point(tabRect.Left, tabRect.Bottom));
					graphics.DrawLine(tabbedGroupPen, new Point(tabRect.Right, tabRect.Top), new Point(tabRect.Right, tabRect.Bottom));
				}
			}
			using(var tabCaptionBrush = LayoutItemDrawHelper.GetCaptionBrush()) {
				var caption = GetTabCaption(tabRect, itemCaption, graphics, isTabShrinked);
				var font = GetCaptionFont(isActiveTab);
				var captionSize = graphics.MeasureString(caption, font);
				graphics.DrawString(caption, font, tabCaptionBrush, BringToCenter(tabRect.Width, (int)captionSize.Width, tabRect.X),
					BringToCenter(tabRect.Height, (int)captionSize.Height, tabRect.Y));
			}
		}
		static Font GetCaptionFont(bool isActiveTab) { return isActiveTab ? ActiveTabFont : TabFont; }
		static int BringToCenter(int containerSize, int elementSize, int offset = 0) {
			return offset + containerSize / 2 - elementSize / 2;
		}
		static SolidBrush GetTabBrush(bool isActiveTab) {
			return isActiveTab ? new SolidBrush(LayoutCellDrawHelper.SelectedBackColor) : new SolidBrush(Color.White);
		}
		static Pen GetTabbedGroupPen() {
			return new Pen(LayoutCellDrawHelper.BorderColor);
		}
		static string GetTabCaption(Rectangle tabRect, string itemCaption, Graphics graphics, bool isTabShrinked) {
			if(!isTabShrinked || string.IsNullOrEmpty(itemCaption) || IsTextFitToTabRect(itemCaption, tabRect, graphics))
				return itemCaption;
			string captionPart = "";
			string tabCaptionPostfix = "...";
			for(int i = 0; i < itemCaption.Length; i++) {
				captionPart += itemCaption[i];
				if(!IsTextFitToTabRect(captionPart + tabCaptionPostfix, tabRect, graphics))
					break;
			}
			return captionPart.Remove(captionPart.Length - 1) + tabCaptionPostfix;
		}
		static bool IsTextFitToTabRect(string text, Rectangle tabRect, Graphics graphics) {
			return LayoutItemDrawHelper.GetCaptionWidth(text, graphics) + TabCaptionHorizontalPadding * 2 < tabRect.Width;
		}
		static bool IsFirstItemContainsSecondItem(LayoutItemBase firstItem, LayoutItemBase secondItem) {
			return secondItem.Path.StartsWith(firstItem.Path);
		}
		static bool IsTabStripClicked(TabbedLayoutGroup tabbedGroup, Rectangle rect, Point point, Graphics graphics, bool isTabStripShrinked) {
			if(!tabbedGroup.ShowGroupDecoration)
				return false;
			int tabsWidth = isTabStripShrinked ? rect.Width : GetTabHeaderRectangle(tabbedGroup, rect, tabbedGroup.Items.GetVisibleItemCount() - 1, graphics, false).Right - rect.X;
			Rectangle tabsRect = new Rectangle(rect.X, rect.Y, tabsWidth, TabHeaderHeight);
			return !point.IsEmpty && tabsRect.Contains(point);
		}
		static Rectangle GetTabHeaderRectangle(TabbedLayoutGroup tabbedGroup, Rectangle tabbedGroupRectangle, int tabIndex, Graphics graphics, bool isTabStripShrinked) {
			var x = tabbedGroupRectangle.X;
			if(tabIndex > 0)
				x = GetTabHeaderRectangle(tabbedGroup, tabbedGroupRectangle, tabIndex - 1, graphics, isTabStripShrinked).Right + SpacingBetweenTabs;
			if(tabIndex == 0)
				x += FirstTabLeftPadding;
			return new Rectangle(x, tabbedGroupRectangle.Y, GetTabWidth(tabbedGroup, tabbedGroupRectangle, graphics, tabIndex, isTabStripShrinked), TabHeaderHeight);
		}
		static int GetTabWidth(TabbedLayoutGroup tabbedGroup, Rectangle tabbedGroupRectangle, Graphics graphics, int tabIndex, bool isTabStripShrinked) {
			return isTabStripShrinked ? GetShrinkedTabWidth(tabbedGroup, tabbedGroupRectangle)
				: LayoutItemDrawHelper.GetCaptionWidth(tabbedGroup.Items.GetVisibleItemOrGroup(tabIndex).Caption, graphics) + TabCaptionHorizontalPadding * 2;
		}
		static int GetShrinkedTabWidth(TabbedLayoutGroup tabbedGroup, Rectangle tabbedGroupRectangle) {
			return (tabbedGroupRectangle.Width - FirstTabLeftPadding - SpacingBetweenTabs * (tabbedGroup.Items.GetVisibleItemCount() - 1))
				/ tabbedGroup.Items.GetVisibleItemCount();
		}
	}
	public class SelectedBag {
		LayoutItemBase item;
		Rectangle rectCellToSelect;
		public static InsertDirection MovingItemInsertDirection;
		public LayoutItemBase Item {
			get { return item; }
		}
		public Rectangle RectCellToSelect {
			get { return rectCellToSelect; }
		}
		public SelectedBag(LayoutItemBase item, Rectangle rectCellToSelect) {
			this.item = item;
			this.rectCellToSelect = rectCellToSelect;
		}
	}
	public class TabbedLayoutGroupMap : LayoutGroupMap {
		List<Rectangle> tabHeaderRectangles;
		public TabbedLayoutGroupMap(ASPxFormLayout formLayout, TabbedLayoutGroup tabbedGroup)
			: base(formLayout) {
			TabbedGroup = tabbedGroup;
		}
		public TabbedLayoutGroup TabbedGroup { get; set; }
		public override int ContainerTopMargin { get { return LayoutCellDrawHelper.LayoutCellContentMargin + TabbedLayoutGroupDrawHelper.TabHeaderHeight; } }
		public List<Rectangle> TabHeaderRectangles {
			get {
				if(tabHeaderRectangles == null)
					tabHeaderRectangles = new List<Rectangle>();
				return tabHeaderRectangles;
			}
		}
		public override int ProcessLayoutElement(LayoutItemBase layoutItemBase) {
			int maxTabHeight = 0;
			var visibleItemCount = TabbedGroup.Items.GetVisibleItemCount();
			map = new MapPoint[1, visibleItemCount];
			for(int i = 0; i < visibleItemCount; ++i) {
				var tab = TabbedGroup.Items.GetVisibleItemOrGroup(i);
				var tabMap = CreateLayoutGroupMap(FormLayout, tab);
				int tabHeight = tabMap.ProcessLayoutElement(tab);
				if(maxTabHeight < tabHeight)
					maxTabHeight = tabHeight;
				map[0, i] = new MapPoint(FormLayout, tab, tabMap, tabHeight, 0, 0);
			}
			return maxTabHeight > 0 ? maxTabHeight + TabbedLayoutGroupDrawHelper.TabHeaderHeight + LayoutCellDrawHelper.LayoutCellContentMargin * 2 : LayoutCellDrawHelper.DefaultCellHeight;
		}
		protected override MapPoint GetMapPointFromLocationRecursive(LayoutGroupMap layoutGroupMap, int x, int y, ref Rectangle targetRect) {
			var innerRect = Rectangle.Empty;
			var result = GetClickedHeaderTabMapItem(x, y, ref innerRect);
			if(result != null) {
				targetRect = innerRect;
				SelectedBag.MovingItemInsertDirection = x > targetRect.Right / 2 ? InsertDirection.Right : InsertDirection.Left;
				return result;
			}
			result = FindInnerMapItem(x, y, ref innerRect);
			if(result != null) {
				targetRect = innerRect;
				return result;
			}
			return null;
		}
		MapPoint FindInnerMapItem(int x, int y, ref Rectangle targetRect) { 
			var result = GetLocatedSelectedMapItem(x, y, ref targetRect);
			if(result != null) {
				var innerRect = Rectangle.Empty;
				var innerMapItem = result.Map.GetMapPointFromLocation(x, y, ref innerRect);
				if(innerMapItem != null) {
					result = innerMapItem;
					targetRect = innerRect;
				}
			}
			return result;
		}
		public MapPoint GetClickedHeaderTabMapItem(int x, int y, ref Rectangle tabRect) {
			var rect = Rectangle.Empty;
			for(var i = 0; i < TabHeaderRectangles.Count; ++i) {
				var tRect = TabHeaderRectangles[i];
				rect.Location = tRect.Location;
				rect.Size = tRect.Size;
				rect.Height += LayoutCellDrawHelper.CellPadding;
				if(rect.Contains(x, y) && map.GetLength(1) > i) {
					rect.Height -= LayoutCellDrawHelper.CellPadding;
					tabRect = rect;
					return map[0, i];
				}
			}
			tabRect = Rectangle.Empty;
			return null;
		}
		MapPoint GetLocatedSelectedMapItem(int x, int y, ref Rectangle targetRect) { 
			var activeTabIndex = TabbedGroup.ActiveTabIndex;
			if(map.GetLength(1) > activeTabIndex) {
				var selectedMap = map[0, activeTabIndex];
				if(selectedMap.LocationRect.Contains(x, y)) {
					targetRect = selectedMap.LocationRect;
					return selectedMap;
				}
			}
			return null;
		}
		public override SelectedBag DrawLayoutElementCore(MapPoint mapPoint, Graphics graphics, Color backColor, Point selectionPoint, LayoutItemBase currentSelectedItem, Rectangle adjustedItemRect) {
			SelectedBag tabbedGroupSelectedBag = null;
			SelectedBag result = null;
			var visibleItemCount = TabbedGroup.Items.GetVisibleItemCount();
			if(TabbedGroup.ShowGroupDecoration) {
				tabbedGroupSelectedBag = TabbedLayoutGroupDrawHelper.Draw(this, adjustedItemRect, graphics, selectionPoint, currentSelectedItem);
			} else {
				if(TabbedGroup == currentSelectedItem || !selectionPoint.IsEmpty && adjustedItemRect.Contains(selectionPoint))
					tabbedGroupSelectedBag = new SelectedBag(TabbedGroup, adjustedItemRect);
				if(visibleItemCount > 0) {
					TabbedGroup.ActiveTabIndex = TabbedLayoutGroupDrawHelper.GetActiveTabIndex(this, adjustedItemRect, graphics, selectionPoint, currentSelectedItem, false);
					if(TabbedGroup.Items.GetVisibleItem(TabbedGroup.ActiveTabIndex) == currentSelectedItem)
						tabbedGroupSelectedBag = new SelectedBag(currentSelectedItem, adjustedItemRect);
				}
			}
			if(visibleItemCount > 0) {
				var tabbedGroupContentRect = GetTabContentRectangle(TabbedGroup, adjustedItemRect);
				var mapItem = map[0, TabbedGroup.ActiveTabIndex];
				mapItem.Rect = new Rectangle(Point.Empty, tabbedGroupContentRect.Size);
				mapItem.GroupHeight = tabbedGroupContentRect.Height;
				result = mapItem.Map.DrawLayoutElement(mapItem, graphics, backColor, tabbedGroupContentRect, selectionPoint, currentSelectedItem);
			}
			return result != null ? result : tabbedGroupSelectedBag;
		}
		protected Rectangle GetTabContentRectangle(TabbedLayoutGroup tabbedGroup, Rectangle parentCellRect) {
			if(!tabbedGroup.ShowGroupDecoration)
				return parentCellRect;
			var internalRect = LayoutCellDrawHelper.GetInternalRectangle(parentCellRect);
			return new Rectangle(internalRect.X, internalRect.Y + TabbedLayoutGroupDrawHelper.TabHeaderHeight, internalRect.Width,
				internalRect.Height - TabbedLayoutGroupDrawHelper.TabHeaderHeight);
		}
	}
	public class LayoutGroupMap {
		const int HorizontalMovingAreaLength = 48;
		const int AdditionalTopMarginForTile = 10;
		protected internal MapPoint[,] map;
		protected internal int[] maxRowHeights;
		List<Rectangle> itemRectangles;
		Busy busy;
		public LayoutGroupMap(ASPxFormLayout formLayout) {
			FormLayout = formLayout;
		}
		public ASPxFormLayout FormLayout { get; private set; }
		bool ContainsHierarchy { get { return map != null && map.GetLength(1) > 0; } }
		Color BackColor { get { return Color.FromArgb(238, 238, 238); } }
		Busy BusyItem {
			get {
				if(busy == null)
					busy = new Busy(FormLayout, new EmptyLayoutItem(), LayoutCellDrawHelper.DefaultCellHeight);
				return busy;
			}
		}
		public virtual int ContainerTopMargin { get { return LayoutCellDrawHelper.LayoutCellContentMargin; } }
		public virtual int ContainerLeftMargin { get { return LayoutCellDrawHelper.LayoutCellContentMargin; } }
		public Point Location { get; set; }
		public List<Rectangle> ItemRectangles {
			get {
				if(itemRectangles == null)
					itemRectangles = new List<Rectangle>();
				return itemRectangles;
			}
		}
		public class MapPoint {
			LayoutItemBase item;
			LayoutGroupMap map;
			int groupHeight;
			Rectangle rect;
			Rectangle locationRect;
			int startJ;
			int startI;
			public MapPoint(ASPxFormLayout formLayout, LayoutItemBase item, int groupHeight)
				: this(formLayout, item, null, groupHeight, 0, 0) {
			}
			public MapPoint(ASPxFormLayout formLayout, LayoutItemBase item, LayoutGroupMap map, int groupHeight, int startI, int startJ) {
				FormLayout = formLayout;
				this.item = item;
				this.map = map;
				this.groupHeight = groupHeight;
				this.startI = startI;
				this.startJ = startJ;
				this.locationRect = Rectangle.Empty;
			}
			public ASPxFormLayout FormLayout { get; private set; }
			public LayoutItemBase Item { get { return this.item; } }
			public int GroupHeight { get { return groupHeight; } set { groupHeight = value; } }
			public LayoutGroupMap Map { get { return map; } }
			public Rectangle Rect { get { return rect; } set { rect = value; } }
			public int StartJ { get { return startJ; } }
			public int StartI { get { return startI; } }
			public Rectangle LocationRect {
				get {
					if(locationRect == Rectangle.Empty && Map != null)
						locationRect = new Rectangle(Map.Location, Rect.Size);
					return locationRect;
				}
			}
		}
		class Busy : MapPoint {
			public Busy(ASPxFormLayout formLayout, LayoutItemBase item, int groupHeight)
				: base(formLayout, item, null, groupHeight, -1, -1) {
			}
			public Busy(ASPxFormLayout formLayout, LayoutItemBase item, LayoutGroupMap map, int groupHeight, int startI, int startJ)
				: base(formLayout, item, map, groupHeight, startI, startJ) {
			}
		}
		int GetLayoutGroupVerticalMargins(LayoutGroup layoutGroup) {
			int result = LayoutCellDrawHelper.LayoutCellContentMargin * 2;
			if(layoutGroup.GetGroupBoxDecoration() == GroupBoxDecoration.HeadingLine)
				result += AdditionalTopMarginForTile;
			return result;
		}
		Rectangle GetLayoutGroupInternalRectangle(LayoutGroup layoutGroup, Rectangle layoutGroupRect) {
			var result = LayoutCellDrawHelper.GetInternalRectangle(layoutGroupRect);
			if(layoutGroup.GetGroupBoxDecoration() == GroupBoxDecoration.HeadingLine) {
				result.Y = result.Y + AdditionalTopMarginForTile;
				result.Height = result.Height - AdditionalTopMarginForTile;
			}
			return result;
		}
		public virtual int ProcessLayoutElement(LayoutItemBase layoutItemBase) {
			if(layoutItemBase is LayoutGroup)
				return ProcessLayoutGroup((LayoutGroup)layoutItemBase);
			if(layoutItemBase is LayoutItem)
				return LayoutItemDrawHelper.GetLayoutItemCellHeight((LayoutItem)layoutItemBase);
			return LayoutCellDrawHelper.DefaultCellHeight;
		}
		int ProcessLayoutGroup(LayoutGroup layoutGroup) {
			int colIndex = 0, rowIndex = 0;
			int maxRowCount = GetMaxRowCount(layoutGroup);
			map = new MapPoint[layoutGroup.ColCount, maxRowCount];
			maxRowHeights = new int[maxRowCount];
			var visibleItemCount = layoutGroup.Items.GetVisibleItemCount();
			for(int k = 0; k < visibleItemCount; k++) {
				var item = layoutGroup.Items.GetVisibleItemOrGroup(k);
				while(true) {
					GetNextEmptyCell(map, layoutGroup.ColCount, maxRowCount, ref colIndex, ref rowIndex);
					if(HasEnoughSpace(layoutGroup.ColCount, colIndex, rowIndex, item))
						break;
					map[colIndex, rowIndex] = BusyItem;
				}
				var startI = colIndex;
				var startJ = rowIndex;
				var currentBusy = item.GetRowSpan() > 1 ? new Busy(FormLayout, item, null, -1, startI, startJ) : BusyItem;
				for(int i = colIndex; i <= colIndex + item.GetColSpan() - 1; i++) {
					for(int j = rowIndex; j <= rowIndex + item.GetRowSpan() - 1; j++) {
						map[i, j] = currentBusy;
					}
				}
				var layoutItemMap = CreateLayoutGroupMap(FormLayout, item);
				int groupHeight = layoutItemMap.ProcessLayoutElement(item);
				var itemMapPoint = new MapPoint(FormLayout, item, layoutItemMap, groupHeight, startI, startJ);
				map[colIndex, rowIndex] = itemMapPoint;
			}
			int groupRowsHeight = CalcRowHeights(layoutGroup, maxRowCount);
			int groupContentHeight = groupRowsHeight > 0 ? groupRowsHeight : LayoutCellDrawHelper.DefaultCellHeight;
			return groupContentHeight + GetLayoutGroupVerticalMargins(layoutGroup);
		}
		public static LayoutGroupMap CreateLayoutGroupMap(ASPxFormLayout formLayout, LayoutItemBase layoutItem) {
			if(layoutItem is TabbedLayoutGroup)
				return new TabbedLayoutGroupMap(formLayout, (TabbedLayoutGroup)layoutItem);
			return new LayoutGroupMap(formLayout);
		}
		public LayoutItemBase GetItemFromLocation(int x, int y) {
			var targetRect = new Rectangle();
			var layoutMap = GetMapPointFromLocation(x, y, ref targetRect);
			return layoutMap != null ? layoutMap.Item : null;
		}
		public MovingItem GetMovingItemFromLocation(FormLayoutItemsOwner owner, int x, int y) {
			var targetRect = new Rectangle();
			SelectedBag.MovingItemInsertDirection = InsertDirection.None;
			var layoutMap = GetMapPointFromLocation(x, y, ref targetRect);
			var direction = SelectedBag.MovingItemInsertDirection;
			if(direction == InsertDirection.None)
				direction = DetermineMovingDirection(owner, layoutMap, ref targetRect, new Point(x, y));
			return new MovingItem(layoutMap, direction, targetRect);
		}
		public MapPoint GetMapPointFromLocation(int x, int y, ref Rectangle targetRect) {
			return GetMapPointFromLocationCore(map, x, y, ref targetRect);
		}
		MapPoint GetMapPointFromLocationCore(MapPoint[,] groupMap, int x, int y, ref Rectangle targetRect) {
			if(groupMap == null)
				return null;
			var lenRank0 = groupMap.GetLength(0);
			var lenRank1 = groupMap.GetLength(1);
			for(var i = 0; i < lenRank0; ++i) {
				for(var j = 0; j < lenRank1; ++j) {
					var mapItem = groupMap[i, j];
					var itemLayoutGroup = GetLayoutGroupMap(mapItem);
					if(itemLayoutGroup == null || !mapItem.LocationRect.Contains(x, y))
						continue;
					targetRect = mapItem.LocationRect;
					if(!targetRect.IsEmpty) {
						var innerRect = new Rectangle(targetRect.Location, targetRect.Size);
						var innerMapItem = itemLayoutGroup.GetMapPointFromLocationRecursive(itemLayoutGroup, x, y, ref innerRect);
						if(innerMapItem != null) {
							targetRect = innerRect;
							return innerMapItem;
						}
						return mapItem;
					}
				}
			}
			return null;
		}
		protected virtual MapPoint GetMapPointFromLocationRecursive(LayoutGroupMap layoutGroupMap, int x, int y, ref Rectangle targetRect) {
			return GetMapPointFromLocationCore(layoutGroupMap.map, x, y, ref targetRect);
		}
		InsertDirection DetermineMovingDirection(FormLayoutItemsOwner owner, MapPoint mapItem, ref Rectangle clientItemRect, Point movingPoint) {
			if(mapItem == null)
				return InsertDirection.Inside;
			var layoutItem = mapItem.Item;
			var result = DetermineMovingDirectionInside(owner, layoutItem, ref clientItemRect, movingPoint);
			if(result != InsertDirection.None)
				return result;
			result = DetermineMovingDirectionInnerContent(owner, layoutItem, clientItemRect, movingPoint);
			if(result != InsertDirection.None)
				return result;
			var parentMultiColumnMapItem = GetParentMulticolumnGroupMapPoint(owner, map, layoutItem);
			if(parentMultiColumnMapItem != null)
				return DetermineMovingDirectionForColSpanGroupItems(owner, layoutItem, clientItemRect, movingPoint);
			result = movingPoint.Y < (clientItemRect.Y + (clientItemRect.Height >> 1)) ? InsertDirection.Before : InsertDirection.After;
			if(!owner.CanMoveItem(owner.FocusedItem, layoutItem, result))
				return InsertDirection.None;
			result = !owner.MovingBetweenNeighbours(layoutItem, result) ? result : InsertDirection.None;
			if(parentMultiColumnMapItem != null) {
				var groupRect = new Rectangle(parentMultiColumnMapItem.Map.Location.X, clientItemRect.Y, parentMultiColumnMapItem.Rect.Width, parentMultiColumnMapItem.Rect.Height);
				groupRect.Inflate(-LayoutCellDrawHelper.CellPadding, 0);
				clientItemRect = groupRect;
			}
			return result;
		}
		InsertDirection DetermineMovingDirectionInside(FormLayoutItemsOwner owner, LayoutItemBase layoutItem, ref Rectangle clientItemRect, Point movingPoint) { 
			if(owner.CanMoveInside(layoutItem)) {
				var insideRect = new Rectangle(clientItemRect.Location, clientItemRect.Size);
				var inflateValue = -LayoutCellDrawHelper.CellPadding;
				insideRect.Inflate(inflateValue, inflateValue);
				if(insideRect.Contains(movingPoint)) {
					clientItemRect = insideRect;
					return InsertDirection.Inside;
				}
			}
			return InsertDirection.None;
		}
		InsertDirection DetermineMovingDirectionInnerContent(FormLayoutItemsOwner owner, LayoutItemBase layoutItem, Rectangle clientItemRect, Point movingPoint) {
			var parentMapItem = GetParentMulticolumnGroupMapPoint(owner, map, layoutItem);
			return parentMapItem != null ? DetermineMovingDirectionForColSpanGroupItems(owner, layoutItem, clientItemRect, movingPoint) : InsertDirection.None;
		}
		MapPoint GetParentMulticolumnGroupMapPoint(FormLayoutItemsOwner owner, MapPoint[,] map, LayoutItemBase layoutItem) {
			var parentMapItem = FindMapItemRecursive(map, layoutItem.ParentGroup);
			if(parentMapItem == null)
				return null;
			var parentGroup = parentMapItem.Item as LayoutGroup;
			if(parentGroup == null || owner.RootLayoutGroup == parentGroup || layoutItem.ColSpan >= parentGroup.ColCount)
				return null;
			return parentMapItem;
		}
		internal MapPoint FindMapItem(LayoutItemBase layoutItem) {
			return FindMapItemRecursive(map, layoutItem);
		}
		MapPoint FindMapItemRecursive(MapPoint[,] map, LayoutItemBase layoutItem) {
			if(layoutItem == null || map == null)
				return null;
			var lenRank0 = map.GetLength(0);
			var lenRank1 = map.GetLength(1);
			for(var i = 0; i < lenRank0; ++i) {
				for(var j = 0; j < lenRank1; ++j) {
					var mapItem = map[i, j];
					if(mapItem == null)
						continue;
					if(mapItem.Item.Path == layoutItem.Path)
						return mapItem;
					if(ValidateLayoutGroupMap(mapItem.Map)) {
						mapItem = FindMapItemRecursive(mapItem.Map.map, layoutItem);
						if(mapItem != null)
							return mapItem;
					}
				}
			}
			return null;
		}
		bool ValidateLayoutGroupMap(LayoutGroupMap layoutGroupMap) {
			return layoutGroupMap != null && layoutGroupMap.map != null;
		}
		InsertDirection DetermineMovingDirectionForColSpanGroupItems(FormLayoutItemsOwner owner, LayoutItemBase layoutItem, Rectangle clientItemRect, Point movingPoint) {
			var result = InsertDirection.None;
			var movingAreaLength = (int)clientItemRect.Width / 3;
			if(movingAreaLength > HorizontalMovingAreaLength)
				movingAreaLength = HorizontalMovingAreaLength;
			if(movingPoint.X <= clientItemRect.Left + HorizontalMovingAreaLength)
				result = !owner.MovingBetweenNeighbours(layoutItem, InsertDirection.Before) ? InsertDirection.Left : InsertDirection.None;
			else if(movingPoint.X >= clientItemRect.Right - HorizontalMovingAreaLength)
				result = !owner.MovingBetweenNeighbours(layoutItem, InsertDirection.After) ? InsertDirection.Right : InsertDirection.None;
			return result;
		}
		LayoutGroupMap GetLayoutGroupMap(MapPoint mapPoint) {
			return mapPoint != null ? mapPoint.Map : null;
		}
		public LayoutItemBase Draw(Graphics graphics, Color backColor, Rectangle canvasRect, LayoutItemBase selectedItem) {
			return Draw(graphics, BackColor, canvasRect, Point.Empty, selectedItem);
		}
		public LayoutItemBase Draw(Graphics graphics, Color backColor, Rectangle canvasRect, Point clickPoint, LayoutItemBase selectedItem) {
			var selectedBag = DrawRecursive(graphics, BackColor, canvasRect, clickPoint, selectedItem);
			return selectedBag == null ? null : selectedBag.Item;
		}
		public SelectedBag DrawRecursive(Graphics graphics, Color backColor, Rectangle canvasRect, Point clickPoint, LayoutItemBase selectedItem) {
			return DrawLayoutElements(map, canvasRect, graphics, BackColor, clickPoint, selectedItem);
		}
		SelectedBag DrawLayoutElements(MapPoint[,] map, Rectangle canvasRect, Graphics graphics, Color backColor, Point clickPoint, LayoutItemBase currentSelectedItem) {
			if(map == null)
				return null;
			int columnCount = map.GetLength(0);
			int columnWidth = canvasRect.Width / columnCount;
			SelectedBag selectedBag = null;
			int currentY = 0;
			for(int j = 0; j < map.GetLength(1); j++) {
				for(int i = 0; i < columnCount; i++) {
					var currentMap = map[i, j];
					if(currentMap != null && !(currentMap is Busy)) {
						var rect = new Rectangle();
						rect.X = i * columnWidth;
						rect.Y = currentY;
						rect.Width = currentMap.Item.GetColSpan() * columnWidth;
						var currentMapRowSpan = currentMap.Item.GetRowSpan() - 1;
						for(int k = j; k <= j + currentMapRowSpan; k++)
							rect.Height += maxRowHeights[k];
						currentMap.Rect = rect;
						var currentSelectedBag = currentMap.Map.DrawLayoutElement(currentMap, graphics, backColor, canvasRect, clickPoint, currentSelectedItem);
						if(selectedBag == null)
							selectedBag = currentSelectedBag;
					}
				}
				currentY += maxRowHeights[j];
			}
			return selectedBag;
		}
		public SelectedBag DrawLayoutElement(MapPoint mapPoint, Graphics graphics, Color backColor, Rectangle canvasRect, Point selectionPoint, LayoutItemBase currentSelectedItem) {
			var adjustedItemRect = AdjustItemRectToCanvas(mapPoint.Rect, canvasRect);
			ItemRectangles.Clear();
			Location = adjustedItemRect.Location;
			LayoutCellDrawHelper.Draw(this, adjustedItemRect, graphics, false);
			ItemRectangles.Add(adjustedItemRect);
			var result = DrawLayoutElementCore(mapPoint, graphics, BackColor, selectionPoint, currentSelectedItem, adjustedItemRect);
			return result;
		}
		public virtual SelectedBag DrawLayoutElementCore(MapPoint mapPoint, Graphics graphics, Color backColor, Point selectionPoint, LayoutItemBase currentSelectedItem, Rectangle adjustedItemRect) {
			var item = mapPoint.Item;
			if(item is LayoutItem || item is EmptyLayoutItem)
				return DrawLayoutItem(graphics, selectionPoint, mapPoint, BackColor, currentSelectedItem, adjustedItemRect);
			if(item is LayoutGroup)
				return DrawLayoutGroup(graphics, mapPoint, adjustedItemRect, BackColor, selectionPoint, currentSelectedItem);
			return null;
		}
		SelectedBag DrawLayoutItem(Graphics graphics, Point selectionPoint, MapPoint mapPoint, Color BackColor, LayoutItemBase currentSelectedItem, Rectangle itemCanvasRect) {
			var result = ProcessSelectedItem(graphics, selectionPoint, mapPoint, currentSelectedItem, itemCanvasRect);
			var item = mapPoint.Item;
			if(item is LayoutItem)
				LayoutItemDrawHelper.Draw(FormLayout, item as LayoutItem, itemCanvasRect, graphics);
			else
				EmptyLayoutItemDrawHelper.Draw(graphics, itemCanvasRect, result != null);
			return result;
		}
		protected SelectedBag DrawLayoutGroup(Graphics graphics, MapPoint mapPoint, Rectangle parentCellRect, Color backColor, Point selectionPoint, LayoutItemBase currentSelectedItem) {
			var layoutGroup = (LayoutGroup)mapPoint.Item;
			var groupMap = mapPoint.Map;
			var internalRect = GetLayoutGroupInternalRectangle(layoutGroup, parentCellRect);
			if(!groupMap.ContainsHierarchy || !internalRect.Contains(selectionPoint))
				ProcessSelectedItem(graphics, selectionPoint, mapPoint, currentSelectedItem, parentCellRect);
			var childSelectedBag = groupMap.DrawRecursive(graphics, BackColor, internalRect, selectionPoint, currentSelectedItem);
			internalRect.Inflate(2, 2);
			var groupSelectedBag = LayoutGroupDrawHelper.Draw(layoutGroup, internalRect, parentCellRect, graphics, selectionPoint, currentSelectedItem);
			return childSelectedBag != null ? childSelectedBag : groupSelectedBag;
		}
		static SelectedBag ProcessSelectedItem(Graphics graphics, Point selectionPoint, MapPoint mapPoint, LayoutItemBase currentSelectedItem, Rectangle itemCanvasRect) {
			var item = mapPoint.Item;
			var isClicked = !selectionPoint.IsEmpty && itemCanvasRect.Contains(selectionPoint);
			if(isClicked || item == currentSelectedItem) {
				var result = new SelectedBag(item, itemCanvasRect);
				LayoutCellDrawHelper.Draw(mapPoint.Map, result.RectCellToSelect, graphics, true);
				return result;
			}
			return null;
		}
		protected Rectangle AdjustItemRectToCanvas(Rectangle rect, Rectangle canvasRect) {
			return new Rectangle(rect.X + canvasRect.X, rect.Y + canvasRect.Y, rect.Width, rect.Height);
		}
		protected bool HasEnoughSpace(int mapWidth, int colIndex, int rowIndex, LayoutItemBase item) {
			int firstBusyIndex = colIndex;
			while(firstBusyIndex < mapWidth && !(map[firstBusyIndex, rowIndex] is Busy))
				firstBusyIndex++;
			return colIndex + item.GetColSpan() - 1 < firstBusyIndex;
		}
		protected int CalcRowHeights(LayoutGroup layoutGroup, int maxRowCount) {
			CalcRowHeightsWithoutRowSpans(layoutGroup, maxRowCount);
			return CalcRowHeightsWithRowSpans(layoutGroup, maxRowCount);
		}
		protected void CalcRowHeightsWithoutRowSpans(LayoutGroup layoutGroup, int maxRowCount) {
			for(int j = 0; j < maxRowCount; j++) {
				int maxRowHeight = 0;
				for(int i = 0; i < layoutGroup.ColCount; i++) {
					if(map[i, j] != null && maxRowHeight < map[i, j].GroupHeight && map[i, j].Item.GetRowSpan() == 1)
						maxRowHeight = map[i, j].GroupHeight;
				}
				maxRowHeights[j] = maxRowHeight;
			}
		}
		protected int CalcRowHeightsWithRowSpans(LayoutGroup layoutGroup, int maxRowCount) {
			int mapHeight = 0;
			for(int j = 0; j < maxRowCount; j++) {
				int maxRowHeight = maxRowHeights[j];
				for(int i = 0; i < layoutGroup.ColCount; i++) {
					if(map[i, j] != null && map[i, j].Item.GetRowSpan() > 1) {
						maxRowHeight = CalcRowHeightWithRowSpans(layoutGroup, maxRowCount, mapHeight, i, j);
						break;
					}
				}
				maxRowHeights[j] = maxRowHeight;
				mapHeight += maxRowHeight;
			}
			return mapHeight;
		}
		int CalcRowHeightWithRowSpans(LayoutGroup layoutGroup, int maxRowCount, int mapHeight, int currentI, int currentJ) {
			int maxRowHeight = maxRowHeights[currentJ];
			for(int i = currentI; i < layoutGroup.ColCount; i++) {
				if(map[i, currentJ] != null && map[i, currentJ].Item.GetRowSpan() > 1) {
					MapPoint point = map[i, currentJ];
					MapPoint originPoint = map[point.StartI, point.StartJ];
					int itemMinHeight = originPoint.GroupHeight;
					int minItemRowHeight = (itemMinHeight - GetItemPreviousHeight(point.StartJ, currentJ)) / (point.Item.GetRowSpan() - (currentJ - point.StartJ));
					if(maxRowHeight < minItemRowHeight)
						maxRowHeight = minItemRowHeight;
				}
			}
			return maxRowHeight;
		}
		int GetItemPreviousHeight(int startJ, int currentJ) {
			int itemPreviousHeight = 0;
			for(int j = currentJ - 1; j >= startJ; j--) {
				itemPreviousHeight += maxRowHeights[j];
			}
			return itemPreviousHeight;
		}
		protected void GetNextEmptyCell(object[,] rootMap, int mapWidth, int mapHeight, ref int colIndex, ref int rowIndex) {
			bool firstRow = true;
			for(int j = rowIndex; j < mapHeight; j++) {
				for(int i = colIndex; i < mapWidth; i++) {
					if(rootMap[i, j] == null) {
						colIndex = i;
						rowIndex = j;
						return;
					}
				}
				if(firstRow) {
					firstRow = false;
					colIndex = 0;
				}
			}
		}
		protected int GetMaxRowCount(LayoutGroup layoutGroup) {
			int maxRowCount = 0;
			for(int i = 0; i < layoutGroup.Items.GetVisibleItemCount(); i++) {
				maxRowCount += layoutGroup.Items.GetVisibleItemOrGroup(i).GetRowSpan();
			}
			return maxRowCount;
		}
	}
	public class MovingItem {
		public MovingItem(DevExpress.Web.Design.LayoutGroupMap.MapPoint mapPoint, InsertDirection direction, Rectangle targetRect) {
			LayoutItem = mapPoint != null ? mapPoint.Item : null;
			Direction = direction;
			TargetRect = targetRect;
		}
		public LayoutItemBase LayoutItem { get; set; }
		public InsertDirection Direction { get; set; }
		public Rectangle TargetRect { get; set; }
		public string Path { get { return LayoutItem != null ? LayoutItem.Path : string.Empty; } }
	}
}
