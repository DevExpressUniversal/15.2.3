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
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Skins;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using DevExpress.Utils.Text;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraBars.Ribbon.Internal;
using System.Drawing.Imaging;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Utils;
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class RibbonItemCalculatorDelegateHelper {
		RibbonItemViewInfoCalculator calc;
		public RibbonItemCalculatorDelegateHelper(RibbonItemViewInfoCalculator calc) {
			this.calc = calc;
			dGetSplitButtonElementInfo = new DGetElementInfo(calc.GetSplitButtonElementInfo);
			dGetSplitButtonElementInfo2 = new DGetElementInfo(calc.GetSplitButtonElementInfo2);
			dGetLargeSplitButtonElementInfo = new DGetElementInfo(calc.GetLargeSplitButtonElementInfo);
			dGetLargeSplitButtonElementInfo2 = new DGetElementInfo(calc.GetLargeSplitButtonElementInfo2);
			dGetLargeButtonElementInfo = new DGetElementInfo(calc.GetLargeButtonElementInfo);
			dGetEditElementInfo = new DGetElementInfo(calc.GetEditElementInfo);
			dGetStaticElementInfo = new DGetElementInfo(calc.GetStaticElementInfo);
			dGetButtonElementInfo = new DGetElementInfo(calc.GetButtonElementInfo);
			dGetVertSeparatorElementInfo = new DGetElementInfo(calc.GetVertSeparatorElementInfo);
			dGetButtonGroupElementInfo = new DGetElementInfo(calc.GetButtonGroupElementInfo);
			dCalcLargeButtonViewInfo = new DCalcViewInfo(calc.CalcLargeButtonViewInfo);
			dCalcVertSeparatorViewInfo = new DCalcViewInfo(calc.CalcVertSeparatorViewInfo);
			dCalcLargeSplitButtonViewInfo = new DCalcViewInfo(calc.CalcLargeSplitButtonViewInfo);
			dCalcSplitButtonViewInfo = new DCalcViewInfo(calc.CalcSplitButtonViewInfo);
			dCalcSplitButtonNoTextViewInfo = new DCalcViewInfo(calc.CalcSplitButtonNoTextViewInfo);
			dCalcStaticItemViewInfo = new DCalcViewInfo(calc.CalcStaticItemViewInfo);
			dCalcButtonViewInfo = new DCalcViewInfo(calc.CalcButtonViewInfo);
			dCalcButtonNoTextViewInfo = new DCalcViewInfo(calc.CalcButtonNoTextViewInfo);
			dCalcDropDownButtonViewInfo = new DCalcViewInfo(calc.CalcDropDownButtonViewInfo);
			dCalcDropDownButtonNoTextViewInfo = new DCalcViewInfo(calc.CalcDropDownButtonNoTextViewInfo);
			dCalcButtonGroupViewInfo = new DCalcViewInfo(calc.CalcButtonGroupViewInfo);
			dCalcEditItemViewInfo = new DCalcViewInfo(calc.CalcEditItemViewInfo);
			dCalcToggleSwitchItemViewInfo = new DCalcViewInfo(calc.CalcToggleSwitchItemViewInfo);
			dCalcVertSeparatorSize = new DCalcSize(calc.CalcVertSeparatorSize);
			dCalcLargeDropDownButtonSize = new DCalcSize(calc.CalcLargeDropDownButtonSize);
			dCalcLargeButtonSize = new DCalcSize(calc.CalcLargeButtonSize);
			dCalcLargeSplitButtonSize = new DCalcSize(calc.CalcLargeSplitButtonSize);
			dCalcButtonGroupSize = new DCalcSize(calc.CalcButtonGroupSize);
			dCalcStaticItemSize = new DCalcSize(calc.CalcStaticItemSize);
			dCalcSplitButtonSize = new DCalcSize(calc.CalcSplitButtonSize);
			dCalcSplitButtonNoTextSize = new DCalcSize(calc.CalcSplitButtonNoTextSize);
			dCalcButtonNoTextSize = new DCalcSize(calc.CalcButtonNoTextSize);
			dCalcButtonSize = new DCalcSize(calc.CalcButtonSize);
			dCalcDropDownButtonNoTextSize = new DCalcSize(calc.CalcDropDownButtonNoTextSize);
			dCalcDropDownButtonSize = new DCalcSize(calc.CalcDropDownButtonSize);
			dCalcEditItemSize = new DCalcSize(calc.CalcEditItemSize);
			dCalcToggleSwitchSize = new DCalcSize(calc.CalcToggleSwitchItemSize);
			dDrawEditItem = new DDrawItem(calc.DrawEditItem);
			dDrawEditItemText = new DDrawItem(calc.DrawEditItemText);
			dDrawButton = new DDrawItem(calc.DrawButton);
			dDrawPageGroupButton = new DDrawItem(calc.DrawPageGroupButton);
			dDrawPageGroupButtonText = new DDrawItem(calc.DrawPageGroupButtonText);
			dDrawButtonText = new DDrawItem(calc.DrawButtonText);
			dDrawDropDownButtonNoText = new DDrawItem(calc.DrawDropDownButtonNoText);
			dDrawDropDownButton = new DDrawItem(calc.DrawDropDownButton);
			dDrawDropDownButtonText = new DDrawItem(calc.DrawDropDownButtonText);
			dDrawLargeDropDownButton = new DDrawItem(calc.DrawLargeDropDownButton);
			dDrawLargeDropDownButtonText = new DDrawItem(calc.DrawLargeDropDownButtonText);
			dDrawLargeButton = new DDrawItem(calc.DrawLargeButton);
			dDrawLargeButtonText = new DDrawItem(calc.DrawLargeButtonText);
			dDrawLargeGroupDropDownButton = new DDrawItem(calc.DrawLargeGroupDropDownButton);
			dDrawLargeGroupDropDownButtonText = new DDrawItem(calc.DrawLargeGroupDropDownButtonText);
			dDrawLargeSplitButton = new DDrawItem(calc.DrawLargeSplitButton);
			dDrawLargeSplitButtonText = new DDrawItem(calc.DrawLargeSplitButtonText);
			dDrawSplitButton = new DDrawItem(calc.DrawSplitButton);
			dDrawSplitButtonText = new DDrawItem(calc.DrawSplitButtonText);
			dDrawVertSeparator = new DDrawItem(calc.DrawVertSeparator);
			dDrawButtonGroup = new DDrawItem(calc.DrawButtonGroup);
			dDrawToggleSwitchItem = new DDrawItem(calc.DrawToggleSwitchItem);
		}
		DGetElementInfo dGetSplitButtonElementInfo,
						dGetSplitButtonElementInfo2,
						dGetLargeSplitButtonElementInfo,
						dGetLargeSplitButtonElementInfo2,
						dGetLargeButtonElementInfo,
						dGetEditElementInfo,
						dGetStaticElementInfo,
						dGetButtonElementInfo,
						dGetVertSeparatorElementInfo,
						dGetButtonGroupElementInfo;
		DCalcViewInfo dCalcLargeButtonViewInfo,
						dCalcVertSeparatorViewInfo,
						dCalcLargeSplitButtonViewInfo,
						dCalcSplitButtonViewInfo,
						dCalcSplitButtonNoTextViewInfo,
						dCalcStaticItemViewInfo,
						dCalcButtonViewInfo,
						dCalcButtonNoTextViewInfo,
						dCalcDropDownButtonViewInfo,
						dCalcDropDownButtonNoTextViewInfo,
						dCalcButtonGroupViewInfo,
						dCalcEditItemViewInfo,
						dCalcToggleSwitchItemViewInfo;
		DCalcSize dCalcVertSeparatorSize,
						dCalcLargeDropDownButtonSize,
						dCalcLargeButtonSize,
						dCalcLargeSplitButtonSize,
						dCalcButtonGroupSize,
						dCalcStaticItemSize,
						dCalcSplitButtonSize,
						dCalcSplitButtonNoTextSize,
						dCalcButtonNoTextSize,
						dCalcButtonSize,
						dCalcDropDownButtonNoTextSize,
						dCalcDropDownButtonSize,
						dCalcEditItemSize,
						dCalcToggleSwitchSize;
		DDrawItem dDrawEditItem,
						dDrawEditItemText,
						dDrawButton,
						dDrawPageGroupButton,
						dDrawButtonText,
						dDrawPageGroupButtonText,
						dDrawDropDownButtonNoText,
						dDrawDropDownButton,
						dDrawDropDownButtonText,
						dDrawLargeDropDownButton,
						dDrawLargeDropDownButtonText,
						dDrawLargeButton,
						dDrawLargeButtonText,
						dDrawLargeGroupDropDownButton,
						dDrawLargeGroupDropDownButtonText,
						dDrawLargeSplitButton,
						dDrawLargeSplitButtonText,
						dDrawSplitButton,
						dDrawSplitButtonText,
						dDrawVertSeparator,
						dDrawButtonGroup,
						dDrawToggleSwitchItem;
		public DGetElementInfo GetSplitButtonElementInfo { get { return dGetSplitButtonElementInfo; } }
		public DGetElementInfo GetSplitButtonElementInfo2 { get { return dGetSplitButtonElementInfo2; } }
		public DGetElementInfo GetLargeSplitButtonElementInfo { get { return dGetLargeSplitButtonElementInfo; } }
		public DGetElementInfo GetLargeSplitButtonElementInfo2 { get { return dGetLargeSplitButtonElementInfo2; } }
		public DGetElementInfo GetLargeButtonElementInfo { get { return dGetLargeButtonElementInfo; } }
		public DGetElementInfo GetEditElementInfo { get { return dGetEditElementInfo; } }
		public DGetElementInfo GetStaticElementInfo { get { return dGetStaticElementInfo; } }
		public DGetElementInfo GetButtonElementInfo { get { return dGetButtonElementInfo; } }
		public DGetElementInfo GetVertSeparatorElementInfo { get { return dGetVertSeparatorElementInfo; } }
		public DGetElementInfo GetButtonGroupElementInfo { get { return dGetButtonGroupElementInfo; } }
		public DCalcViewInfo CalcLargeButtonViewInfo { get { return dCalcLargeButtonViewInfo; } }
		public DCalcViewInfo CalcVertSeparatorViewInfo { get { return dCalcVertSeparatorViewInfo; } }
		public DCalcViewInfo CalcLargeSplitButtonViewInfo { get { return dCalcLargeSplitButtonViewInfo; } }
		public DCalcViewInfo CalcSplitButtonViewInfo { get { return dCalcSplitButtonViewInfo; } }
		public DCalcViewInfo CalcSplitButtonNoTextViewInfo { get { return dCalcSplitButtonNoTextViewInfo; } }
		public DCalcViewInfo CalcStaticItemViewInfo { get { return dCalcStaticItemViewInfo; } }
		public DCalcViewInfo CalcButtonViewInfo { get { return dCalcButtonViewInfo; } }
		public DCalcViewInfo CalcButtonNoTextViewInfo { get { return dCalcButtonNoTextViewInfo; } }
		public DCalcViewInfo CalcDropDownButtonViewInfo { get { return dCalcDropDownButtonViewInfo; } }
		public DCalcViewInfo CalcDropDownButtonNoTextViewInfo { get { return dCalcDropDownButtonNoTextViewInfo; } }
		public DCalcViewInfo CalcButtonGroupViewInfo { get { return dCalcButtonGroupViewInfo; } }
		public DCalcViewInfo CalcEditItemViewInfo { get { return dCalcEditItemViewInfo; } }
		public DCalcViewInfo CalcToggleSwitchItemViewInfo { get { return dCalcToggleSwitchItemViewInfo; } }
		public DCalcSize CalcVertSeparatorSize { get { return dCalcVertSeparatorSize; } }
		public DCalcSize CalcLargeDropDownButtonSize { get { return dCalcLargeDropDownButtonSize; } }
		public DCalcSize CalcLargeButtonSize { get { return dCalcLargeButtonSize; } }
		public DCalcSize CalcLargeSplitButtonSize { get { return dCalcLargeSplitButtonSize; } }
		public DCalcSize CalcButtonGroupSize { get { return dCalcButtonGroupSize; } }
		public DCalcSize CalcStaticItemSize { get { return dCalcStaticItemSize; } }
		public DCalcSize CalcSplitButtonSize { get { return dCalcSplitButtonSize; } }
		public DCalcSize CalcSplitButtonNoTextSize { get { return dCalcSplitButtonNoTextSize; } }
		public DCalcSize CalcButtonNoTextSize { get { return dCalcButtonNoTextSize; } }
		public DCalcSize CalcButtonSize { get { return dCalcButtonSize; } }
		public DCalcSize CalcDropDownButtonNoTextSize { get { return dCalcDropDownButtonNoTextSize; } }
		public DCalcSize CalcDropDownButtonSize { get { return dCalcDropDownButtonSize; } }
		public DCalcSize CalcEditItemSize { get { return dCalcEditItemSize; } }
		public DCalcSize CalcToggleSwitchItemSize { get { return dCalcToggleSwitchSize; } }
		public DDrawItem DrawEditItem { get { return dDrawEditItem; } }
		public DDrawItem DrawEditItemText { get { return dDrawEditItemText; } }
		public DDrawItem DrawButton { get { return dDrawButton; } }
		public DDrawItem DrawPageGroupButtonText { get { return dDrawPageGroupButtonText; } }
		public DDrawItem DrawPageGroupButton { get { return dDrawPageGroupButton; } }
		public DDrawItem DrawButtonText { get { return dDrawButtonText; } }
		public DDrawItem DrawDropDownButtonNoText { get { return dDrawDropDownButtonNoText; } }
		public DDrawItem DrawDropDownButton { get { return dDrawDropDownButton; } }
		public DDrawItem DrawDropDownButtonText { get { return dDrawDropDownButtonText; } }
		public DDrawItem DrawLargeDropDownButton { get { return dDrawLargeDropDownButton; } }
		public DDrawItem DrawLargeDropDownButtonText { get { return dDrawLargeDropDownButtonText; } }
		public DDrawItem DrawLargeButton { get { return dDrawLargeButton; } }
		public DDrawItem DrawLargeButtonText { get { return dDrawLargeButtonText; } }
		public DDrawItem DrawLargeGroupDropDownButton { get { return dDrawLargeGroupDropDownButton; } }
		public DDrawItem DrawLargeGroupDropDownButtonText { get { return dDrawLargeGroupDropDownButtonText; } }
		public DDrawItem DrawLargeSplitButton { get { return dDrawLargeSplitButton; } }
		public DDrawItem DrawLargeSplitButtonText { get { return dDrawLargeSplitButtonText; } }
		public DDrawItem DrawSplitButton { get { return dDrawSplitButton; } }
		public DDrawItem DrawSplitButtonText { get { return dDrawSplitButtonText; } }
		public DDrawItem DrawVertSeparator { get { return dDrawVertSeparator; } }
		public DDrawItem DrawButtonGroup { get { return dDrawButtonGroup; } }
		public DDrawItem DrawToggleSwitchItem { get { return dDrawToggleSwitchItem; } }
	}
	public class RibbonMacStyleItemViewInfoCalculator : RibbonItemViewInfoCalculator {
		public RibbonMacStyleItemViewInfoCalculator(BaseRibbonViewInfo viewInfo) : base(viewInfo) { 
		}
		public override int CalcLargeSplitButtonHeight(Graphics graphics) {
			int glyphHeight = ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, GetSplitButtonElementInfo(null), new Rectangle(Point.Empty, ViewInfo.LargeImageSize)).Height;
			return glyphHeight + ViewInfo.ButtonTextHeight;
		}
		public override void DrawLargeSplitButtonText(GraphicsCache cache, RibbonItemViewInfo item) {
			DrawLargeCaption(cache, item, false);
		}
		protected override Size CalcLargeButtonSizeCore(Graphics graphics, RibbonItemViewInfo item, bool drawArrow) {
			if(item is RibbonGroupItemViewInfo)
				return base.CalcLargeButtonSizeCore(graphics, item, drawArrow);
			RibbonButtonItemViewInfo buttonInfo = (RibbonButtonItemViewInfo)item;
			graphics = CheckGraphics(graphics);
			int width = GetButtonWidthByLevel(item);
			SizeF caption = item.Appearance.CalcTextSize(graphics, GetStringFormat(item.Appearance), item.Text, width);
			SkinElementInfo itemInfo = item.GetItemInfo();
			item.CaptionBounds = new Rectangle(Point.Empty, new Size((int)caption.Width, (int)caption.Height));
			buttonInfo.PushBounds = ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, itemInfo, new Rectangle(Point.Empty,ViewInfo.LargeImageSize));
			item.GlyphBounds = new Rectangle(Point.Empty, ViewInfo.LargeImageSize);
			int arrowWidth = drawArrow? CalcArrowWidth(graphics): 0;
			return new Size(Math.Max(item.CaptionBounds.Width + itemInfo.Element.ContentMargins.Left + itemInfo.Element.ContentMargins.Right, buttonInfo.PushBounds.Width + arrowWidth), buttonInfo.PushBounds.Height + ViewInfo.ButtonTextHeight);		
		}
		public override void CalcLargeButtonViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			if(item is RibbonGroupItemViewInfo) {
				base.CalcLargeButtonViewInfo(graphics, item);
				return;
			}
			RibbonButtonItemViewInfo buttonItem = (RibbonButtonItemViewInfo)item;
			graphics = GInfo.AddGraphics(graphics);
			try {
				int arrowWidth = ShouldDrawArrow(item) ? CalcArrowWidth(graphics): 0;
				SkinElementInfo itemInfo = item.GetItemInfo();
				item.CaptionBounds = new Rectangle(new Point(item.Bounds.X + (item.Bounds.Width - item.CaptionBounds.Width) / 2, item.Bounds.Bottom - ViewInfo.ButtonTextHeight), item.CaptionBounds.Size);
				buttonItem.PushBounds = new Rectangle(new Point(item.Bounds.X + (item.Bounds.Width - buttonItem.PushBounds.Width - arrowWidth) / 2, item.Bounds.Y), buttonItem.PushBounds.Size);
				SkinElementInfo info = item.GetItemInfo();
				info.Bounds = buttonItem.PushBounds;
				item.GlyphBounds = ObjectPainter.GetObjectClientRectangle(graphics, SkinElementPainter.Default, info);
				if(ViewInfo.IsRightToLeft) {
					item.CaptionBounds = BarUtilites.ConvertBoundsToRTL(item.CaptionBounds, item.Bounds);
					item.GlyphBounds = BarUtilites.ConvertBoundsToRTL(item.GlyphBounds, item.Bounds);
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected override Rectangle LayoutArrow(Rectangle bounds, RibbonItemViewInfo item, Rectangle textBounds) {
			if(!item.Item.IsLargeButton)
				return base.LayoutArrow(bounds, item, textBounds);
			Size arrowSize = CalcArrowSize(item.ViewInfo.GInfo.Graphics);
			Rectangle rect = new Rectangle(new Point((ViewInfo.IsRightToLeft ? item.Bounds.X : item.GlyphBounds.Right) + ArrowIndent, item.GlyphBounds.Y + (item.GlyphBounds.Height - arrowSize.Height) / 2), arrowSize);
			return rect;
		}
		public override void DrawLargeSplitButton(GraphicsCache cache, RibbonItemViewInfo item) {
			RibbonSplitButtonItemViewInfo splitItem = (RibbonSplitButtonItemViewInfo)item;
			CheckItemViewInfo(cache, item);
			DrawItemBackground(cache, item, item.Bounds, item.GetItemInfo(), item.Appearance);
			DrawItemBackground(cache, item, splitItem.DropButtonBounds, splitItem.GetDropDownInfo(), splitItem.DropDownAppearance);
			DrawLargeGlyph(cache, item);
			DrawText(cache, item);
			DrawArrow(cache, splitItem.DropButtonBounds, ObjectState.Normal);
			DrawSelection(cache, item);
			DrawReduceOperationSelection(cache, item);
		}
		protected internal override void DrawArrow(GraphicsCache cache, Rectangle bounds, ObjectState state) {
			if(bounds == Rectangle.Empty) return;
			SkinElementInfo rightPart = GetSplitButtonElementInfo2(null);
			rightPart.Bounds = bounds;
			Rectangle rect = ObjectPainter.GetObjectClientRectangle(cache.Graphics, SkinElementPainter.Default, rightPart);
			SkinElementInfo info = GetArrowElementInfo();
			info.Bounds = bounds;
			info.ImageIndex = -1;
			info.State = state;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		public override Size CalcLargeSplitButtonSize(Graphics graphics, RibbonItemViewInfo item) {
			graphics = CheckGraphics(graphics);
			int width = GetButtonWidthByLevel(item);
			SizeF caption = item.Appearance.CalcTextSize(graphics, item.Text, width);
			item.CaptionBounds = new Rectangle(Point.Empty, new Size((int)caption.Width, (int)caption.Height));
			SkinElementInfo leftPart = GetSplitButtonElementInfo(item);
			SkinElementInfo arrowInfo = new SkinElementInfo(RibbonSkins.GetSkin(item.ViewInfo.Provider)[RibbonSkins.SkinButtonArrow], Rectangle.Empty);
			SkinElementInfo rightPart = GetSplitButtonElementInfo2(item);
			item.GlyphBounds = new Rectangle(Point.Empty, ViewInfo.LargeImageSize);
			Rectangle arrowBounds = ObjectPainter.CalcObjectMinBounds(graphics, SkinElementPainter.Default, arrowInfo);
			Rectangle rightBounds = ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, rightPart, arrowBounds);
			Rectangle glyphBounds = ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, leftPart, item.GlyphBounds);
			return new Size(Math.Max((int)caption.Width, rightBounds.Width + glyphBounds.Width), ViewInfo.ButtonTextHeight + glyphBounds.Height);
		}
		public override void CalcLargeSplitButtonViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			RibbonSplitButtonItemViewInfo splitItem = (RibbonSplitButtonItemViewInfo)item;
			graphics = GInfo.AddGraphics(graphics);
			try {
				SkinElementInfo leftPart = GetSplitButtonElementInfo(item);
				SkinElementInfo rightPart = GetSplitButtonElementInfo2(item);
				SkinElementInfo arrowInfo = new SkinElementInfo(RibbonSkins.GetSkin(item.ViewInfo.Provider)[RibbonSkins.SkinButtonArrow], Rectangle.Empty);
				item.CaptionBounds = new Rectangle(item.Bounds.X + (item.Bounds.Width - item.CaptionBounds.Width) / 2, item.Bounds.Bottom - ViewInfo.ButtonTextHeight, item.CaptionBounds.Width, ViewInfo.ButtonTextHeight);
				Rectangle arrowBounds = ObjectPainter.CalcObjectMinBounds(graphics, SkinElementPainter.Default, arrowInfo);
				Rectangle rightBounds = ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, rightPart, arrowBounds);
				Rectangle leftBounds = ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, leftPart, item.GlyphBounds);
				Rectangle glyphAndArrow = new Rectangle(item.Bounds.X + (item.Bounds.Width - rightBounds.Width - leftBounds.Width) / 2, item.Bounds.Y, rightBounds.Width + leftBounds.Width, item.Bounds.Height);
				leftBounds.Y = rightBounds.Y = item.Bounds.Y;
				leftBounds.X = glyphAndArrow.X;
				leftBounds.Height = item.Bounds.Height - item.CaptionBounds.Height;
				rightBounds.X = leftBounds.Right;
				rightBounds.Height = leftBounds.Height;
				splitItem.PushButtonBounds = leftBounds;
				splitItem.DropButtonBounds = rightBounds;
				leftPart.Bounds = leftBounds;
				item.GlyphBounds = ObjectPainter.GetObjectClientRectangle(graphics, SkinElementPainter.Default, leftPart);
				if(ViewInfo.IsRightToLeft) {
					item.CaptionBounds = BarUtilites.ConvertBoundsToRTL(item.CaptionBounds, item.Bounds);
					item.GlyphBounds = BarUtilites.ConvertBoundsToRTL(item.GlyphBounds, item.Bounds);
					splitItem.DropButtonBounds = BarUtilites.ConvertBoundsToRTL(splitItem.DropButtonBounds, item.Bounds);
					splitItem.PushBounds = BarUtilites.ConvertBoundsToRTL(splitItem.PushBounds, item.Bounds);
					splitItem.PushButtonBounds = BarUtilites.ConvertBoundsToRTL(splitItem.PushButtonBounds, item.Bounds);
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
	}
	public class RibbonItemViewInfoCalculator {
		BaseRibbonViewInfo viewInfo;
		RibbonItemCalculatorDelegateHelper helper;
		public RibbonItemViewInfoCalculator(BaseRibbonViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			this.helper = new RibbonItemCalculatorDelegateHelper(this);
		}
		public RibbonItemCalculatorDelegateHelper Helper { get { return helper; } }
		public static void DrawItem(GraphicsCache cache, RibbonItemViewInfo itemInfo) {
			XtraAnimator.Current.DrawAnimationHelper(cache, itemInfo.OwnerControl as ISupportXtraAnimation, itemInfo,
				new RibbonItemPainter(), new RibbonDrawInfo(itemInfo),
				new DrawTextInvoker(DrawItemText), itemInfo);
		}
		public static void DrawItemTextOnGlass(GraphicsCache cache, object info) {
			RibbonItemViewInfo itemInfo = (RibbonItemViewInfo)info;
			AppearanceObject app = itemInfo.GetPaintAppearance();
			using(StringFormat format = (StringFormat)app.GetStringFormat().Clone()) { 
				format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
				ObjectPainter.DrawTextOnGlass(cache.Graphics, app, itemInfo.Text, itemInfo.CaptionBounds, format);
			}
		}
		public static bool ShouldDrawTextOnGlass(RibbonItemViewInfo itemInfo) {
			RibbonViewInfo vi = itemInfo.ViewInfo as RibbonViewInfo;
			if(vi == null || vi.Form == null || !vi.Form.IsGlassForm)
				return false;
			BarItemLink link = itemInfo.Item as BarItemLink;
			BarLinksHolder holder = link.Holder is BarButtonGroup? ((itemInfo.Owner as RibbonButtonGroupItemViewInfo).Item as BarItemLink).Holder: link.Holder;
			if(vi.Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007 && holder is RibbonPageHeaderItemLinkCollection)
				return true;
			return holder is RibbonQuickToolbarItemLinkCollection && vi.Ribbon.ToolbarLocation != RibbonQuickAccessToolbarLocation.Below;
		}
		protected internal bool IsInCustomDrawEvent { get; set; }
		public static void DrawItemText(GraphicsCache cache, object info) {
			RibbonItemViewInfo itemInfo = (RibbonItemViewInfo)info;
			BarItemLink link = (BarItemLink)itemInfo.Item;
			if(!itemInfo.ItemCalculator.IsInCustomDrawEvent) {
				BarItemCustomDrawEventArgs e = new BarItemCustomDrawEventArgs(cache, itemInfo);
				e.DrawOnlyText = true;
				link.Manager.RaiseCustomDrawItem(e);
				if(e.Handled)
					return;
			}
			if(itemInfo.Item.IsAllowHtmlText) {
				if(itemInfo.CaptionBounds.Location == Point.Empty) return;
				StringPainter.Default.DrawString(cache, itemInfo.HtmlStringInfo);
				if(itemInfo.ItemCalculator.ShouldDrawArrowHtmlText(itemInfo))
					itemInfo.ItemCalculator.DrawArrow(cache, CalcArrowBoundsHtmlText(cache, itemInfo, itemInfo.ItemCalculator), itemInfo.CalcState());
			}
			else {
				DDrawItem drawText = itemInfo.GetInfo().DrawText;
				if(drawText != null) drawText(cache, itemInfo);
			}
		}
		static Rectangle CalcArrowBoundsHtmlText(GraphicsCache cache, RibbonItemViewInfo itemInfo, RibbonItemViewInfoCalculator calc) {
			Rectangle arrowRect = new Rectangle();
			arrowRect.Size = calc.CalcArrowSize(cache.Graphics);
			if(itemInfo.IsLargeButton) {
				arrowRect.X = itemInfo.Bounds.X + itemInfo.Bounds.Width / 2 - arrowRect.Width / 2;
				arrowRect.Y = itemInfo.HtmlStringInfo.Bounds.Y + itemInfo.HtmlStringInfo.Bounds.Height;
			}
			else {
				arrowRect.X = itemInfo.HtmlStringInfo.Bounds.X + itemInfo.HtmlStringInfo.Bounds.Width + ((itemInfo.Bounds.X + itemInfo.Bounds.Width - (itemInfo.HtmlStringInfo.Bounds.X + itemInfo.HtmlStringInfo.Bounds.Width)) / 2 - arrowRect.Width / 2);
				arrowRect.Y = itemInfo.Bounds.Y + itemInfo.Bounds.Height / 2 - arrowRect.Height / 2;
			}
			return arrowRect;
		}
		public BaseRibbonViewInfo ViewInfo { get { return viewInfo; } }
		protected GraphicsCache CheckCache(GraphicsCache cache) {
			if(cache == null) return ViewInfo.GInfo.Cache;
			return cache;
		}
		protected Graphics CheckGraphics(Graphics graphics) {
			if(graphics == null) return ViewInfo.GInfo.Graphics;
			return graphics;
		}
		protected virtual bool ShouldDrawArrowHtmlText(RibbonItemViewInfo item) {
			if(ShouldDrawArrow(item)) return true;
			if(item.Item is BarButtonItemLink)
				return (((BarButtonItemLink)item.Item).Item.IsDropDownButtonStyle && item.IsLargeButton);
			return false;
		}
		protected virtual bool ShouldDrawArrow(RibbonItemViewInfo item) {
			RibbonDropDownItemViewInfo dropDownItem = item as RibbonDropDownItemViewInfo;
			if(dropDownItem != null)
				return dropDownItem.AllowDrawDropDownArrow;
			return false;
		}
		protected internal virtual int CalcArrowWidth(Graphics graphics) {
			return ArrowIndent + CalcArrowSize(graphics).Width;
		}
		protected internal virtual Size CalcArrowSize(Graphics graphics) {
			SkinElementInfo arrowInfo = new SkinElementInfo(RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinButtonArrow], Rectangle.Empty);
			return ObjectPainter.CalcObjectMinBounds(graphics, SkinElementPainter.Default, arrowInfo).Size;
		}
		protected GraphicsInfo GInfo { get { return ViewInfo.GInfo; } }
		protected virtual int CaptionVGlyphIndent { get { return 4; } }
		protected internal virtual int CaptionHGlyphIndent { get { return 5; } }
		protected virtual int CaptionHEditIndent { get { return 2; } }
		protected virtual int ArrowIndent { get { return 2; } }
		protected virtual int LineIndent { get { return 0; } }
		protected SkinElement GetElement(string name) {
			if(ViewInfo == null) return null;
			return RibbonSkins.GetSkin(ViewInfo.Provider)[name]; 
		}
		public virtual Size CalcLineTextSize(Graphics graphics, RibbonItemViewInfo item, string text) {
			if(item.Item.IsAllowHtmlText) return StringPainter.Default.Calculate(graphics, item.Appearance, text.Replace("&", ""), 0).Bounds.Size;
			return CalcLineTextSize(graphics, item.Appearance, text);
		}
		public virtual Size CalcLineTextSize(Graphics graphics, AppearanceObject obj, string text) {
			return obj.CalcTextSize(graphics, GetStringFormat(obj), text, 0).ToSize();
		}
		public virtual SkinElementInfo GetArrowElementInfo() { return new SkinElementInfo(GetElement(RibbonSkins.SkinButtonArrow)); }
		public virtual SkinElementInfo GetSplitButtonElementInfo(RibbonItemViewInfo item) {
			if(item != null && item.OwnerButtonGroup != null) return new SkinElementInfo(GetElement(RibbonSkins.SkinButtonGroupSplitButton)); 
			return new SkinElementInfo(GetElement(RibbonSkins.SkinSplitButton)); 
		}
		public virtual SkinElementInfo GetSplitButtonElementInfo2(RibbonItemViewInfo item) {
			if(item != null && item.OwnerButtonGroup != null) return new SkinElementInfo(GetElement(RibbonSkins.SkinButtonGroupSplitButton2));
			return new SkinElementInfo(GetElement(RibbonSkins.SkinSplitButton2)); 
		}
		public virtual SkinElementInfo GetLargeSplitButtonElementInfo(RibbonItemViewInfo item) { return new SkinElementInfo(GetElement(RibbonSkins.SkinLargeSplitButton)); }
		public virtual SkinElementInfo GetLargeSplitButtonElementInfo2(RibbonItemViewInfo item) { return new SkinElementInfo(GetElement(RibbonSkins.SkinLargeSplitButton2)); }
		public virtual SkinElementInfo GetLargeButtonElementInfo(RibbonItemViewInfo item) { return new SkinElementInfo(GetElement(RibbonSkins.SkinLargeButton)); }
		public virtual SkinElementInfo GetEditElementInfo(RibbonItemViewInfo item) {
			return new SkinElementInfo(GetElement(RibbonSkins.SkinButton));
		}
		public virtual SkinElementInfo GetStaticElementInfo(RibbonItemViewInfo item) {
			return GetEditElementInfo(item);
		}
		public virtual SkinElementInfo GetButtonElementInfo(RibbonItemViewInfo item) {
			if(item != null && item is AutoHiddenPagesMenuItemViewInfo) return new SkinElementInfo(GetElement(RibbonSkins.SkinTabHeaderPage));
			if(item != null && item.OwnerButtonGroup != null) return new SkinElementInfo(GetElement(RibbonSkins.SkinButtonGroupButton));
			return new SkinElementInfo(GetElement(RibbonSkins.SkinButton)); 
		}
		public virtual SkinElementInfo GetVertSeparatorElementInfo(RibbonItemViewInfo item) { return new SkinElementInfo(GetElement(RibbonSkins.SkinSeparator)); }
		public virtual SkinElementInfo GetButtonGroupElementInfo(RibbonItemViewInfo item) { return new SkinElementInfo(GetElement(RibbonSkins.SkinButtonGroup)); }
		public virtual SkinElementInfo GetButtonGroupSeparatorElementInfo() { return new SkinElementInfo(GetElement(RibbonSkins.SkinButtonGroupSeparator)); }
		public virtual void CalcLargeButtonViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			graphics = GInfo.AddGraphics(graphics);
			try {
				Rectangle glyph, bounds = item.Bounds;
				SkinElementInfo info = GetLargeButtonElementInfo(item);
				info.Bounds = bounds;
				Rectangle client = ObjectPainter.GetObjectClientRectangle(graphics, SkinElementPainter.Default, info);
				glyph = client;
				glyph.Size = ViewInfo.LargeImageSize;
				glyph.X += (client.Width - glyph.Width) / 2;
				item.GlyphBounds = glyph;
				IRibbonGroupInfo groupInfo = item.Owner as IRibbonGroupInfo;
				Rectangle caption = client;
				caption.Height = GetLargeButtonTextHeight(groupInfo != null && groupInfo.IsSingleLineLargeButton);
				caption.Y = client.Bottom - caption.Height;
				item.CaptionBounds = caption;
				item.HtmlStringInfo = CalcHtmlStringInfo(graphics, item);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		public virtual int GetLargeButtonTextHeight() {
			return GetLargeButtonTextHeight(false);
		}
		protected virtual int GetLargeButtonTextHeight(bool singleLine) {
			int lc = singleLine? 1: ViewInfo.LargeButtonTextLinesCount;
			return ViewInfo.ButtonTextHeight * lc + LineIndent * (lc - 1);
		}
		public virtual void CalcVertSeparatorViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			RibbonSeparatorItemViewInfo separator = (RibbonSeparatorItemViewInfo)item;
			Rectangle bounds = new Rectangle(item.Bounds.X, separator.OwnerContentBounds.Top, item.Bounds.Width, separator.OwnerContentBounds.Height);
			separator.Bounds = bounds;
		}
		public virtual void CalcLargeSplitButtonViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			RibbonSplitButtonItemViewInfo splitItem = (RibbonSplitButtonItemViewInfo)item;
			graphics = GInfo.AddGraphics(graphics);
			try {
				Rectangle glyph, bounds = item.Bounds, dropBounds;
				SkinElementInfo drop = GetLargeSplitButtonElementInfo2(item);
				dropBounds = drop.Bounds = bounds;
				Rectangle client = ObjectPainter.GetObjectClientRectangle(graphics, SkinElementPainter.Default, drop);
				Rectangle caption = client;
				caption.Height = GetLargeButtonTextHeight();
				int deltaHeight = client.Height - caption.Height;
				caption.Y = client.Bottom - caption.Height;
				item.CaptionBounds = caption;
				dropBounds.Height -= deltaHeight;
				dropBounds.Y += deltaHeight;
				SkinElementInfo info = GetLargeSplitButtonElementInfo(item);
				bounds.Height = dropBounds.Top - bounds.Top;
				info.Bounds = bounds;
				client = ObjectPainter.GetObjectClientRectangle(graphics, SkinElementPainter.Default, info);
				glyph = client;
				glyph.Size = ViewInfo.LargeImageSize;
				glyph.X += (client.Width - glyph.Width) / 2;
				item.GlyphBounds = glyph;
				splitItem.DropButtonBounds = dropBounds;
				splitItem.PushButtonBounds = bounds;
				item.HtmlStringInfo = CalcHtmlStringInfo(graphics, item);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual void CalcSplitButtonViewInfoCore(Graphics graphics, RibbonItemViewInfo item, bool withText) {
			RibbonSplitButtonItemViewInfo splitItem = (RibbonSplitButtonItemViewInfo)item;
			graphics = GInfo.AddGraphics(graphics);
			try {
				Rectangle glyph, bounds = item.Bounds, dropBounds;
				SkinElementInfo arrowInfo = new SkinElementInfo(RibbonSkins.GetSkin(item.ViewInfo.Provider)[RibbonSkins.SkinButtonArrow], Rectangle.Empty);
				Rectangle arrowRect = ObjectPainter.CalcObjectMinBounds(graphics, SkinElementPainter.Default, arrowInfo);
				SkinElementInfo drop = GetSplitButtonElementInfo2(item);
				dropBounds = bounds;
				int width = ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, drop, arrowRect).Width;
				dropBounds.Width = width;
				dropBounds.X = bounds.Right - width;
				bounds.Width -= width;
				SkinElementInfo info = GetSplitButtonElementInfo(item);
				info.Bounds = bounds;
				Rectangle client = ObjectPainter.GetObjectClientRectangle(graphics, SkinElementPainter.Default, info);
				glyph = client;
				glyph.Size = item.ImageSize;
				glyph.Y += (client.Height - glyph.Height) / 2;
				if(!withText) glyph.X += (client.Width - glyph.Width) / 2;
				if(!item.IsImageExists) glyph = Rectangle.Empty;
				item.GlyphBounds = glyph;
				Rectangle caption = Rectangle.Empty;
				if(withText) {
					caption = client;
					if(glyph.Width != 0) caption.X = glyph.Right + CaptionHGlyphIndent;
					caption.Width = client.Right - caption.X;
				}
				item.CaptionBounds = caption;
				splitItem.DropButtonBounds = dropBounds;
				splitItem.PushButtonBounds = bounds;
				item.HtmlStringInfo = CalcHtmlStringInfo(graphics, item);
				if(ViewInfo.IsRightToLeft) {
					item.CaptionBounds = BarUtilites.ConvertBoundsToRTL(item.CaptionBounds, item.Bounds);
					item.GlyphBounds = BarUtilites.ConvertBoundsToRTL(item.GlyphBounds, item.Bounds);
					splitItem.DropButtonBounds = BarUtilites.ConvertBoundsToRTL(dropBounds, item.Bounds);
					splitItem.PushBounds = BarUtilites.ConvertBoundsToRTL(splitItem.PushBounds, item.Bounds);
					splitItem.PushButtonBounds = BarUtilites.ConvertBoundsToRTL(splitItem.PushButtonBounds, item.Bounds);
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		public virtual void CalcSplitButtonViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			CalcSplitButtonViewInfoCore(graphics, item, true);
		}
		public virtual void CalcSplitButtonNoTextViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			CalcSplitButtonViewInfoCore(graphics, item, false);
		}
		protected virtual void CalcButtonViewInfoCore(Graphics graphics, RibbonItemViewInfo item, bool withText, bool withArrow) {
			RibbonCheckItemViewInfo checkItem = item as RibbonCheckItemViewInfo;
			graphics = GInfo.AddGraphics(graphics);
			try {
				SkinElementInfo info = GetButtonElementInfo(item);
				info.Bounds = item.Bounds;
				Rectangle client = ObjectPainter.GetObjectClientRectangle(graphics, SkinElementPainter.Default, info);
				if(checkItem != null && checkItem.GetCheckBoxVisibility() != CheckBoxVisibility.None)
					checkItem.CheckBounds = CalcCheckBoxBounds(client, item);
				item.GlyphBounds = CalcGlyphBounds(client, item, withText, withArrow);
				item.CaptionBounds = CalcCaptionBounds(graphics, client, item, withText, withArrow);
				if(ViewInfo.IsRightToLeft) {
					if(checkItem != null) checkItem.CheckBounds = BarUtilites.ConvertBoundsToRTL(checkItem.CheckBounds, item.Bounds);
					item.GlyphBounds = BarUtilites.ConvertBoundsToRTL(item.GlyphBounds, item.Bounds);
					item.CaptionBounds = BarUtilites.ConvertBoundsToRTL(item.CaptionBounds, item.Bounds);
				}
				item.HtmlStringInfo = CalcHtmlStringInfo(graphics, item);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual Rectangle CalcCheckBoxBounds(Rectangle client, RibbonItemViewInfo item) {
			RibbonCheckItemViewInfo checkItem = item as RibbonCheckItemViewInfo;
			Rectangle checkBox = Rectangle.Empty;
			checkBox.Size = checkItem.GetCheckSize();
			checkBox.Y = client.Y + (client.Height - checkBox.Size.Height) / 2;
			if(checkItem.GetCheckBoxVisibility() == CheckBoxVisibility.BeforeText)
				checkBox.X = client.X;
			else
				checkBox.X = client.Right - checkBox.Width;
			return checkBox;
		}
		protected virtual Rectangle CalcGlyphBounds(Rectangle client, RibbonItemViewInfo item, bool withText, bool withArrow) {
			if(!item.IsImageExists) return Rectangle.Empty;
			RibbonCheckItemViewInfo checkItem = item as RibbonCheckItemViewInfo;
			Rectangle glyph = client;
			glyph.Size = item.ImageSize;
			glyph.Y += (client.Height - glyph.Height) / 2;
			if(checkItem != null && checkItem.GetCheckBoxVisibility() == CheckBoxVisibility.BeforeText)
				glyph.X += checkItem.GetCheckSize().Width + CaptionHGlyphIndent;
			if(!withText && !withArrow && checkItem == null)
				glyph.X += (client.Width - glyph.Width) / 2;
			return glyph;
		}
		protected virtual Rectangle CalcCaptionBounds(Graphics g, Rectangle client, RibbonItemViewInfo item, bool withText, bool withArrow) {
			RibbonCheckItemViewInfo checkItem = item as RibbonCheckItemViewInfo;
			Rectangle caption = Rectangle.Empty;
			if(withText || (withArrow && checkItem == null)) {
				caption = client;
				if(item.IsImageExists) caption.X += item.GlyphBounds.Width + CaptionHGlyphIndent;
				if(checkItem != null && checkItem.GetCheckBoxVisibility() == CheckBoxVisibility.BeforeText)
					caption.X += checkItem.GetCheckSize().Width + CaptionHGlyphIndent;
				caption.Width = client.Right - caption.X;
				if(checkItem != null && checkItem.GetCheckBoxVisibility() == CheckBoxVisibility.AfterText)
					caption.Width -= checkItem.CheckBounds.Width + CaptionHGlyphIndent;
			}
			return caption;
		}
		public virtual void CalcStaticItemViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			BarStaticItemLink link = item.Item as BarStaticItemLink;
			if(link != null)
				item.Bounds = new Rectangle(item.Bounds.X + link.Item.LeftIndent, item.Bounds.Y, item.Bounds.Width - link.Item.LeftIndent - link.Item.RightIndent, item.Bounds.Height);
			CalcButtonViewInfoCore(graphics, item, true, false);
		}
		public virtual void CalcButtonViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			CalcButtonViewInfoCore(graphics, item, true, false);
		}
		public virtual void CalcButtonNoTextViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			CalcButtonViewInfoCore(graphics, item, false, false);
		}
		public virtual void CalcDropDownButtonViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			RibbonDropDownItemViewInfo dropDownItem = (RibbonDropDownItemViewInfo)item;
			CalcButtonViewInfoCore(graphics, item, true, dropDownItem.AllowDrawDropDownArrow);
			dropDownItem.ArrowBounds = LayoutArrow(item.Bounds, item, item.CaptionBounds);
		}
		public virtual void CalcDropDownButtonNoTextViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			RibbonDropDownItemViewInfo dropDownItem =(RibbonDropDownItemViewInfo)item;
			CalcButtonViewInfoCore(graphics, item, false, dropDownItem.AllowDrawDropDownArrow);
			dropDownItem.ArrowBounds = LayoutArrow(item.Bounds, item, item.CaptionBounds);
		}
		public virtual void CalcButtonGroupViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			RibbonButtonGroupItemViewInfo group = (RibbonButtonGroupItemViewInfo)item;
			Rectangle bounds, itemBounds;
			SkinElementInfo info = GetButtonGroupElementInfo(item);
			info.Bounds = group.Bounds;
			bounds = ObjectPainter.GetObjectClientRectangle(graphics, SkinElementPainter.Default, info);
			bounds.Y = group.Bounds.Y;
			bounds.Height = group.Bounds.Height;
			itemBounds = bounds;
			for(int n = 0; n < group.Items.Count; n++) {
				RibbonItemViewInfo gi = group.Items[n];
				itemBounds.Width = gi.GetInfo().CalcSize(graphics, gi).Width;
				Rectangle r = itemBounds;
				gi.Bounds = r;
				itemBounds.X += itemBounds.Width + CalcButtonGroupSeparatorWidth(graphics);
				if(ViewInfo.IsRightToLeft)
					gi.Bounds = BarUtilites.ConvertBoundsToRTL(gi.Bounds, bounds);
			}
		}
		protected virtual int GetEditHeight(Graphics graphics, RibbonEditItemViewInfo editItem) {
			int res = GetEditItemSize(editItem).Height;
			RibbonViewInfo vi = editItem.ViewInfo as RibbonViewInfo;
			res = Math.Max(res, editItem.EditViewInfo.CalcMinHeight(graphics));
			if(vi != null) {
				if(vi.Ribbon.PageHeaderItemLinks.Contains(editItem.Item as BarItemLink)) res = Math.Min(res, vi.EditorHeight);
				else res = Math.Min(vi.GroupContentHeight, res);
			}
			if((editItem.Item as BarEditItemLink).Item.EditHeight == -1)
				res = Math.Min(res, ViewInfo.EditorHeight);
			return res;
		}
		public virtual void DrawToggleSwitchItem(GraphicsCache cache, RibbonItemViewInfo item) {
			CheckItemViewInfo(cache, item);
			RibbonToggleSwitchItemViewInfo toggleItem = (RibbonToggleSwitchItemViewInfo)item;
			DrawButtonText(cache, item);
			DrawGlyph(cache, item);
			DrawToggleSwitch(cache, item);
			DrawSelection(cache, item);
		}
		protected virtual void DrawToggleSwitch(GraphicsCache cache, RibbonItemViewInfo vi) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetToggleBackgroundElementInfo(vi));
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetToggleElementInfo(vi));
		}
		protected virtual SkinElementInfo GetToggleBackgroundElementInfo(RibbonItemViewInfo vi) {
			RibbonToggleSwitchItemViewInfo toggleItem = (RibbonToggleSwitchItemViewInfo)vi;
			SkinElementInfo info = new SkinElementInfo(EditorsSkins.GetSkin(ViewInfo.Provider)[EditorsSkins.SkinToggleSwitch], toggleItem.ToggleBounds);
			info.ImageIndex = ((BarToggleSwitchItemLink)vi.Item).Item.Checked && vi.CalcState() != ObjectState.Disabled ? 1 : 0;
			return info;
		}
		protected virtual SkinElementInfo GetToggleElementInfo(RibbonItemViewInfo vi) {
			RibbonToggleSwitchItemViewInfo toggleItem = (RibbonToggleSwitchItemViewInfo)vi;
			SkinElementInfo info = new SkinElementInfo(EditorsSkins.GetSkin(ViewInfo.Provider)[EditorsSkins.SkinToggleSwitchThumb], toggleItem.ThumbBounds);
			info.State = vi.CalcState();
			info.ImageIndex = 0;
			if((info.State & ObjectState.Hot) != 0) info.ImageIndex = 1;
			if((info.State & ObjectState.Pressed) != 0) info.ImageIndex = 2;
			if((info.State & ObjectState.Disabled) != 0) info.ImageIndex = 3;
			return info;
		}
		protected virtual Rectangle GetToggleContentBounds(Graphics graphics, RibbonItemViewInfo item) {
			SkinElementInfo info = GetToggleBackgroundElementInfo(item);
			return ObjectPainter.GetObjectClientRectangle(graphics, SkinElementPainter.Default, info);
		}
		public virtual void CalcToggleSwitchItemViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			RibbonToggleSwitchItemViewInfo toggleItem = (RibbonToggleSwitchItemViewInfo)item;
			BarToggleSwitchItemLink link = (BarToggleSwitchItemLink)item.Item;
			toggleItem.CaptionBounds = new Rectangle(new Point(item.Bounds.Location.X + CaptionHGlyphIndent, item.Bounds.Location.Y + (item.Bounds.Height - toggleItem.CaptionBounds.Height) / 2), new Size(toggleItem.CaptionBounds.Size.Width + 2 * CaptionHGlyphIndent, toggleItem.CaptionBounds.Size.Height));
			toggleItem.ToggleBounds = new Rectangle(new Point(item.Bounds.Right - CaptionHGlyphIndent - toggleItem.ToggleBounds.Size.Width, item.Bounds.Location.Y), new Size(toggleItem.ToggleBounds.Size.Width, item.Bounds.Height));
			toggleItem.ToggleContentBounds = GetToggleContentBounds(graphics, item);
			toggleItem.ThumbBounds = GetToggleBounds(item);
			if(ViewInfo.IsRightToLeft) {
				toggleItem.CaptionBounds = BarUtilites.ConvertBoundsToRTL(toggleItem.CaptionBounds, toggleItem.Bounds);
				toggleItem.ToggleBounds = BarUtilites.ConvertBoundsToRTL(toggleItem.ToggleBounds, toggleItem.Bounds);
				toggleItem.ToggleContentBounds = BarUtilites.ConvertBoundsToRTL(toggleItem.ToggleContentBounds, toggleItem.Bounds);
				toggleItem.ThumbBounds = BarUtilites.ConvertBoundsToRTL(toggleItem.ThumbBounds, toggleItem.Bounds);
			}
		}
		protected virtual Rectangle GetToggleBounds(RibbonItemViewInfo item) {
			BarToggleSwitchItemLink link = (BarToggleSwitchItemLink)item.Item;
			RibbonToggleSwitchItemViewInfo toggleItem = (RibbonToggleSwitchItemViewInfo)item;
			int width = EditorsSkins.GetSkin(ViewInfo.Manager.GetController().LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinToggleSwitch].Properties.GetInteger("SwitchWidth");
			Size toggleSize = new Size(width * toggleItem.ToggleContentBounds.Width / 100, toggleItem.ToggleContentBounds.Height);
			int x = link.Item.Checked ? toggleItem.ToggleContentBounds.Right - toggleSize.Width : toggleItem.ToggleContentBounds.X;
			return new Rectangle(new Point(x, toggleItem.ToggleContentBounds.Y), toggleSize);
		}
		public virtual Size CalcToggleSwitchItemSize(Graphics graphics, RibbonItemViewInfo item) {
			RibbonToggleSwitchItemViewInfo toggleItem = (RibbonToggleSwitchItemViewInfo)item;
			Size textSize = CalcLineTextSize(graphics, item.GetPaintAppearance(), item.Text);
			toggleItem.CaptionBounds = new Rectangle(Point.Empty, textSize);
			toggleItem.ToggleBounds = new Rectangle(Point.Empty, new Size(CalcToggleWidth(), 0));
			int width = textSize.Width;
			width += 3 * CaptionHGlyphIndent + toggleItem.ToggleBounds.Width;
			if(!item.GlyphBounds.Size.IsEmpty)
				width += CaptionHGlyphIndent + item.GlyphBounds.Width;
			int toggleHeight = CalcToggleHeight(graphics, item);
			return new Size(width, Math.Max(toggleHeight, item.ViewInfo.ButtonHeight));
		}
		protected virtual Size CalcToggleThumbSize(Graphics graphics, RibbonItemViewInfo item) {
			SkinElementInfo info = GetToggleElementInfo(item);
			Size sz = ObjectPainter.CalcObjectMinBounds(graphics, SkinElementPainter.Default, info).Size;
			return sz;
		}
		protected virtual int CalcToggleHeight(Graphics graphics, RibbonItemViewInfo item) {
			SkinElementInfo backInfo = GetToggleBackgroundElementInfo(item);
			return ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, backInfo, new Rectangle(Point.Empty, CalcToggleThumbSize(graphics, item))).Height;
		}
		protected virtual int CalcToggleWidth() {
			return 66;
		}
		public virtual void CalcEditItemViewInfo(Graphics graphics, RibbonItemViewInfo item) {
			graphics = GInfo.AddGraphics(graphics);
			try {
				RibbonEditItemViewInfo editItem = (RibbonEditItemViewInfo)item;
				BarEditItemLink link = ((BarEditItemLink)editItem.Item);
				Rectangle editItemBounds = item.Bounds;
				if(!link.ActAsButtonGroup) {
					editItemBounds.Width -= EditorIndent * 2;
					editItemBounds.X += EditorIndent;
				}
				Rectangle editBounds = editItemBounds, glyph = Rectangle.Empty;
				editItem.CaptionBounds = Rectangle.Empty;
				if(item.IsImageExists) {
					glyph = editBounds;
					glyph.Size = item.ImageSize;
					glyph.Y += (editBounds.Height - glyph.Height) / 2;
					editBounds.X = glyph.Right + CaptionHGlyphIndent;
					editBounds.Width = editItemBounds.Right - editBounds.X;
				}
				editItem.GlyphBounds = glyph;
				Rectangle caption = editItemBounds;
				if(!string.IsNullOrEmpty(item.Text) && item.AllowedStyles != RibbonItemStyles.SmallWithoutText) {
					if(!glyph.IsEmpty) caption.X = glyph.Right + CaptionHGlyphIndent;
					caption.Width = CalcSingleLineTextWidth(graphics, item);
					editItem.CaptionBounds = caption;	
					editBounds.X = caption.Right + CaptionHEditIndent;
					editBounds.Width = editItemBounds.Right - editBounds.X;
				}
				if(editBounds.Width < 1) editBounds = Rectangle.Empty;
				int linkWidth = CalcEditLinkWidthByTouch((editItem.Item as BarEditItemLink).Width);
				if((editItem.Item as BarEditItemLink).Ribbon.AutoSizeItems && linkWidth < editBounds.Width) {
					editBounds.X = editBounds.Right - linkWidth;
					if(!string.IsNullOrEmpty(item.Text)) {
						caption.Width = editBounds.X - CaptionHEditIndent - caption.X;
						editItem.CaptionBounds = caption;
					}
					editBounds.Width = linkWidth;
				}
				editItem.EditViewInfo.Bounds = editBounds;
				if(editBounds.IsEmpty) return;
				editItem.UpdateEditLookAndFeel();
				editBounds.Height = GetEditHeight(graphics, editItem);
				editBounds.Y += (editItemBounds.Height - editBounds.Height) / 2;
				if((editItem.Item as BarEditItemLink).GetCaptionAlignment() == HorzAlignment.Far) {
					glyph.X += editBounds.Width + CaptionHEditIndent;
					caption.X += editBounds.Width + CaptionHEditIndent;
					editBounds.X = editItemBounds.X;
					editItem.CaptionBounds = caption;
					editItem.GlyphBounds = glyph;
				}
				editItem.EditViewInfo.Bounds = editBounds;
				if(((BarEditItemLink)editItem.Item).EditorActive) {
					ViewInfo.SuppressInvalidate = true;
					ViewInfo.Manager.ActiveEditor.Bounds = editBounds;
					ViewInfo.SuppressInvalidate = false;
				}
				editItem.UpdateEditViewInfo(graphics, MousePosition);
				editItem.HtmlStringInfo = CalcHtmlStringInfo(graphics, editItem);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual int CalcEditLinkWidthByTouch(int editLinkWidth) {
			if(((ISkinProviderEx)ViewInfo.Provider).GetTouchUI())
				return (int)(editLinkWidth * ((ISkinProviderEx)ViewInfo.Provider).GetTouchScaleFactor());
			return editLinkWidth;
		}
		public virtual StringInfo CalcHtmlStringInfo(Graphics g, RibbonItemViewInfo itemInfo) {
			TextOptions opt = new TextOptions(itemInfo.Appearance);
			Rectangle captionRect = new Rectangle(itemInfo.CaptionBounds.Location, itemInfo.CaptionBounds.Size);
			if(opt.HotkeyPrefix == HKeyPrefix.Default) opt.HotkeyPrefix = HKeyPrefix.Hide;
			if(itemInfo.IsLargeButton) {
				opt.VAlignment = VertAlignment.Top;
				opt.HAlignment = HorzAlignment.Center;
				opt.WordWrap = itemInfo.ShouldWrapText ? WordWrap.Wrap : WordWrap.NoWrap;
			}
			else if(!StringPainter.Default.IsSimpleString(itemInfo.Text)) captionRect.Y -= 1;
			return StringPainter.Default.Calculate(g, itemInfo.Appearance, opt, itemInfo.Text, captionRect);
		}		
		public virtual Point MousePosition {
			get {
				if(ViewInfo.OwnerControl.IsHandleCreated) return ViewInfo.OwnerControl.PointToClient(Control.MousePosition);
				return Point.Empty;
			}
		}
		public virtual int CalcButtonGroupHeight(Graphics graphics) {
			graphics = CheckGraphics(graphics);
			int height = CalcButtonHeight(graphics);
			return height;
		}
		public virtual int CalcSingleLargeButtonHeight(Graphics graphics) {
			return CalcLargeButtonHeightCore(graphics, true);
		}
		protected virtual int CalcLargeButtonHeightCore(Graphics graphics, bool singleLine) {
			graphics = CheckGraphics(graphics);
			Size client = ViewInfo.LargeImageSize;
			int textHeight = GetLargeButtonTextHeight(singleLine);
			client.Height = client.Height + textHeight + CaptionVGlyphIndent;
			return ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, GetLargeButtonElementInfo(null), new Rectangle(Point.Empty, client)).Height;
		}
		public virtual int CalcLargeButtonHeight(Graphics graphics) {
			return CalcLargeButtonHeightCore(graphics, false);
		}
		public virtual int CalcLargeSplitButtonHeight(Graphics graphics) {
			graphics = CheckGraphics(graphics);
			Size client = ViewInfo.LargeImageSize;
			int textHeight = GetLargeButtonTextHeight();
			int height = ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, GetLargeSplitButtonElementInfo(null), new Rectangle(Point.Empty, client)).Height;
			height += ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, GetLargeSplitButtonElementInfo2(null), new Rectangle(Point.Empty, new Size(16, textHeight))).Height;
			return height;
		}
		public virtual int CalcSplitButtonHeight(Graphics graphics) {
			graphics = CheckGraphics(graphics);
			Size client = ViewInfo.ImageSize;
			client.Height = Math.Max(client.Height, ViewInfo.ButtonTextHeight);
			return ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, GetSplitButtonElementInfo(null), new Rectangle(Point.Empty, client)).Height;
		}
		public virtual int CalcButtonHeight(Graphics graphics) {
			graphics = CheckGraphics(graphics);
			Size client = ViewInfo.ImageSize;
			client.Height = Math.Max(client.Height, ViewInfo.ButtonTextHeight);
			return ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, GetButtonElementInfo(null), new Rectangle(Point.Empty, client)).Height;
		}
		public virtual int CalcEditHeight(Graphics graphics) {
			return Math.Max(ViewInfo.ButtonTextHeight, ViewInfo.Manager.EditorHelper.CalcDefaultMinHeight(graphics, ViewInfo.PaintAppearance.Editor));
		}
		public virtual Size CalcButtonArrowSize(Graphics graphics) {
			return ObjectPainter.CalcObjectMinBounds(graphics, SkinElementPainter.Default, GetArrowElementInfo()).Size;
		}
		public virtual Size CalcVertSeparatorSize(Graphics graphics, RibbonItemViewInfo item) {
			RibbonSeparatorItemViewInfo separator = (RibbonSeparatorItemViewInfo)item;
			SkinElementInfo info = GetVertSeparatorElementInfo(item);
			Size size = ObjectPainter.CalcObjectMinBounds(graphics, SkinElementPainter.Default, info).Size;
			size.Height = separator.OwnerContentBounds.Height;
			return size;
		}
		public virtual int CalcButtonGroupSeparatorWidth(Graphics graphics) {
			SkinElementInfo info = GetButtonGroupSeparatorElementInfo();
			return ObjectPainter.CalcObjectMinBounds(graphics, SkinElementPainter.Default, info).Width;
		}
		public virtual Size CalcLargeDropDownButtonSize(Graphics graphics, RibbonItemViewInfo item) {
			return CalcLargeButtonSizeCore(graphics, item, true);
		}
		public virtual Size CalcLargeButtonSize(Graphics graphics, RibbonItemViewInfo item) {
			return CalcLargeButtonSizeCore(graphics, item, false);
		}
		protected virtual int GetButtonWidthByLevel(RibbonItemViewInfo item) {
			BarItemLink link = item.Item as BarItemLink;
			if(link == null) return 0;
			RibbonButtonItemViewInfo buttonInfo = item as RibbonButtonItemViewInfo;
			RibbonStaticItemViewInfo staticInfo = item as RibbonStaticItemViewInfo;
			int width = 0;
			if(staticInfo != null) width = link.Item.SmallWithTextWidth;
			else if(buttonInfo != null) {
				if(buttonInfo.CurrentLevel == RibbonItemStyles.Large) width = link.Item.LargeWidth;
				else if(buttonInfo.CurrentLevel == RibbonItemStyles.SmallWithText) width = link.Item.SmallWithTextWidth;
				else if(buttonInfo.CurrentLevel == RibbonItemStyles.SmallWithoutText) width = link.Item.SmallWithoutTextWidth;
			}
			if(width == 0) return width;
			return Math.Max(width, 16);
		}
		protected virtual bool ShouldWrapLargeButtonText(Graphics graphics, RibbonItemViewInfo item, bool drawArrow, SkinElementInfo info) {
			if(!ShouldWrapItemText(item)) return false;
			if(ViewInfo.LargeButtonTextLinesCount == 1) return false;
			int width = GetButtonWidthByLevel(item);
			int arrowWidth = (drawArrow ? ArrowIndent + ViewInfo.ButtonArrowSize.Width : 0);
			Size client = ViewInfo.LargeImageSize;
			if(width == 0) return true;
			int singleLineWidth = Math.Max(client.Width, CalcLineTextSize(graphics, item, item.Text).Width + arrowWidth);
			info.Bounds = new Rectangle(Point.Empty, new Size(width, client.Height));
			return singleLineWidth > ObjectPainter.GetObjectClientRectangle(graphics, SkinElementPainter.Default, info).Width;
		}
		protected virtual string[] WrapText(RibbonItemViewInfo item) {
			if(item.ShouldWrapText) 
				return ViewInfo.TextWrapper.WrapString(item.Text);
			return new string[] { string.Empty, item.Text };
		}
		protected virtual bool ShouldWrapItemText(RibbonItemViewInfo item) {
			return  (!item.Appearance.Options.UseTextOptions || item.Appearance.TextOptions.WordWrap != WordWrap.NoWrap) && !(item.Owner is RibbonMiniToolbarViewInfo);
		}
		protected bool HasSecondTextLine(RibbonItemViewInfo item) {
			return ViewInfo.TextWrapper.WrapString(item.Text)[1].Length > 0;
		}
		protected virtual Size CalcLargeButtonSizeCore(Graphics graphics, RibbonItemViewInfo item, bool drawArrow) {
			graphics = CheckGraphics(graphics);
			Size client = ViewInfo.LargeImageSize;
			int width = GetButtonWidthByLevel(item);
			int arrowWidth = (drawArrow ?  ArrowIndent + ViewInfo.ButtonArrowSize.Width : 0);
			if(width != 0) {
				item.ShouldWrapText = ShouldWrapLargeButtonText(graphics, item, drawArrow, GetLargeButtonElementInfo(item));
				return new Size(width, ViewInfo.LargeButtonHeight);
			}
			else {
				item.ShouldWrapText = HasSecondTextLine(item) && ShouldWrapItemText(item);
			}
			int textWidth = CalcLargeButtonItemTextWidth(graphics, item, arrowWidth);
			client.Width = Math.Max(textWidth, client.Width);
			Size res = ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, GetLargeButtonElementInfo(item), new Rectangle(Point.Empty, client)).Size;
			res.Width = Math.Max(ViewInfo.LargeButtonMinWidth, res.Width);
			if(item.ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice)
				res.Width += arrowWidth;
			IRibbonGroupInfo groupInfo = item.Owner as IRibbonGroupInfo;
			res.Height = groupInfo != null? groupInfo.LargeRibbonButtonHeight: ViewInfo.LargeButtonHeight;
			if(width != 0) res.Width = width;
			return res;
		}
		protected internal virtual int CalcLargeButtonItemTextWidth(Graphics graphics, RibbonItemViewInfo item, int arrowWidth) {
			string[] text = WrapText(item);
			if(item.ShouldWrapText || item.ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice)
				return Math.Max(CalcLineTextSize(graphics, item, text[0]).Width, CalcLineTextSize(graphics, item, text[1]).Width + (item.ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice ? 0 : arrowWidth));
			if(item.Item.IsAllowHtmlText) return Math.Max(StringPainter.Default.Calculate(graphics, item.Appearance, item.Text, 0).Bounds.Width, arrowWidth);
			return Math.Max(CalcLineTextSize(graphics, item, item.Text).Width, arrowWidth);
		}
		public virtual Size CalcLargeSplitButtonSize(Graphics graphics, RibbonItemViewInfo item) {
			graphics = CheckGraphics(graphics);
			Size client = ViewInfo.LargeImageSize;
			int width = GetButtonWidthByLevel(item);
			if(width != 0) {
				item.ShouldWrapText = ShouldWrapLargeButtonText(graphics, item, true, GetLargeSplitButtonElementInfo2(item));
				IRibbonGroupInfo groupInfo = item.Owner as IRibbonGroupInfo;
				return new Size(width, groupInfo != null? groupInfo.LargeRibbonButtonHeight: ViewInfo.LargeButtonHeight);
			}
			else {
				item.ShouldWrapText = HasSecondTextLine(item) && ShouldWrapItemText(item);
			}	
			int textWidth = CalcLargeSplitButtonTextWidth(graphics, item);
			Size button = ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, GetLargeSplitButtonElementInfo(item), new Rectangle(Point.Empty, client)).Size;
			Size drop = ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, GetLargeSplitButtonElementInfo2(item), new Rectangle(0, 0, textWidth, ViewInfo.ButtonTextHeight)).Size;
			Size res = Size.Empty;
			res.Width = Math.Max(ViewInfo.LargeButtonMinWidth, Math.Max(button.Width, drop.Width));
			res.Height = ViewInfo.LargeButtonHeight;
			if(width != 0) res.Width = width;
			return res;
		}
		protected internal virtual int CalcLargeSplitButtonTextWidth(Graphics graphics, RibbonItemViewInfo item) {
			string[] text = WrapText(item);
			int arrowWidth = ArrowIndent + ViewInfo.ButtonArrowSize.Width;
			if(item.ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice)
				return CalcLineTextSize(graphics, item, item.Text).Width;
			if(item.ShouldWrapText)
				return Math.Max(CalcLineTextSize(graphics, item, text[0]).Width, CalcLineTextSize(graphics, item, text[1]).Width + arrowWidth);
			return Math.Max(CalcLineTextSize(graphics, item, item.Text).Width, arrowWidth);
		}
		public virtual Size CalcButtonGroupSize(Graphics graphics, RibbonItemViewInfo item) {
			RibbonButtonGroupItemViewInfo group = (RibbonButtonGroupItemViewInfo)item;
			graphics = CheckGraphics(graphics);
			Size content = Size.Empty;
			for(int n = 0; n < group.Items.Count; n++) {
				content.Width += group.Items[n].GetInfo().CalcSize(graphics, group.Items[n]).Width;
			}
			if(group.Items.Count > 0)
				content.Width += (group.Items.Count - 1) * CalcButtonGroupSeparatorWidth(graphics);
			if(content.Width == 0)
				content.Width = ViewInfo.ButtonGroupHeight;
			content.Width = ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, GetButtonGroupElementInfo(item), new Rectangle(Point.Empty, content)).Width;
			content.Height = ViewInfo.ButtonGroupHeight; 
			return content;
		}
		public virtual Size CalcStaticItemSize(Graphics graphics, RibbonItemViewInfo item) {  
			Size sz = CalcButtonSizeCore(graphics, item, true, false);
			BarStaticItemLink link = item.Item as BarStaticItemLink;
			if(link != null)
				sz.Width += link.Item.LeftIndent + link.Item.RightIndent;
			return sz;
		}
		public virtual Size CalcSplitButtonSize(Graphics graphics, RibbonItemViewInfo item) { return CalcSplitButtonSizeCore(graphics, item, true); }
		public virtual Size CalcSplitButtonNoTextSize(Graphics graphics, RibbonItemViewInfo item) { return CalcSplitButtonSizeCore(graphics, item, false); }
		public virtual Size CalcButtonNoTextSize(Graphics graphics, RibbonItemViewInfo item) { return CalcButtonSizeCore(graphics, item, false, false); }
		public virtual Size CalcButtonSize(Graphics graphics, RibbonItemViewInfo item) { return CalcButtonSizeCore(graphics, item, true, false); }
		public virtual Size CalcDropDownButtonNoTextSize(Graphics graphics, RibbonItemViewInfo item) { return CalcButtonSizeCore(graphics, item, false, ((RibbonDropDownItemViewInfo)item).AllowDrawDropDownArrow); }
		public virtual Size CalcDropDownButtonSize(Graphics graphics, RibbonItemViewInfo item) { return CalcButtonSizeCore(graphics, item, true, ((RibbonDropDownItemViewInfo)item).AllowDrawDropDownArrow); }
		protected virtual Size CalcSplitButtonSizeCore(Graphics graphics, RibbonItemViewInfo item, bool withText) {
			graphics = CheckGraphics(graphics);
			Size client = item.ImageSize;
			if(withText) {
				if(!item.IsImageExists)
					client = Size.Empty;
				client.Width = client.Width + CaptionHGlyphIndent + CalcSingleLineTextWidth(graphics, item);
			}
			Size size = ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, GetSplitButtonElementInfo(item), new Rectangle(Point.Empty, client)).Size;
			size.Height = ViewInfo.ButtonHeight;
			SkinElementInfo arrowInfo = new SkinElementInfo(RibbonSkins.GetSkin(item.ViewInfo.Provider)[RibbonSkins.SkinButtonArrow], Rectangle.Empty);
			Rectangle arrowRect = ObjectPainter.CalcObjectMinBounds(graphics, SkinElementPainter.Default, arrowInfo);
			size.Width += ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, GetSplitButtonElementInfo2(item), arrowRect).Width;
			int width = GetButtonWidthByLevel(item);
			if(width != 0) size.Width = width;
			return size;
		}
		protected virtual Size CalcButtonSizeCore(Graphics graphics, RibbonItemViewInfo item, bool withText, bool withArrow) {
			graphics = CheckGraphics(graphics);
			Size client = item.ImageSize;
			if(withText) {
				if(!item.IsImageExists)
					client = Size.Empty;
				else
					client.Width += CaptionHGlyphIndent;
				client.Width = client.Width + CalcSingleLineTextWidth(graphics, item);
			}
			if(withArrow)
				client.Width += ArrowIndent + ViewInfo.ButtonArrowSize.Width;
			client.Height = Math.Max(ViewInfo.ButtonHeight, CalcLineTextSize(graphics, item, item.Text).Height);
			client = ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, GetButtonElementInfo(item), new Rectangle(Point.Empty, client)).Size;
			if(!item.CalculateOwnHeight)
				client.Height = ViewInfo.ButtonHeight;
			int width = GetButtonWidthByLevel(item);
			if(width != 0) client.Width = width;
			return client;			
		}
		protected virtual BarEditItemLink GetEditItemLink(RibbonEditItemViewInfo editItem) {
			return editItem.Item as BarEditItemLink;
		}
		protected virtual BarEditItem GetEditItem(RibbonEditItemViewInfo editItem) {
			BarEditItemLink link = GetEditItemLink(editItem);
			return link == null ? null : link.Item;
		}
		public virtual object GetEditItemValue(RibbonEditItemViewInfo editItem) {
			BarEditItem item = GetEditItem(editItem);
			return (item == null ? null : item.EditValue);
		}
		protected int GetScaledWidth(int width) {
			if(!ViewInfo.Manager.GetScaleEditors())
				return width;
			float fDpi = BarUtilites.GetScreenDPI();
			return (int)(width * fDpi / 96.0);
		}
		protected virtual Size GetEditItemSize(RibbonEditItemViewInfo editItem) {
			BarEditItem item = GetEditItem(editItem);
			BarEditItemLink link = (BarEditItemLink)editItem.Item;
			if(item == null) return new Size(16, 0);
			int width = GetScaledWidth(CalcEditLinkWidthByTouch(link.EditWidth));
			Size res = new Size(width, ShouldApplyEditHeight(editItem)? item.EditHeight: ViewInfo.EditorHeight);
			if(res.Height < 0) res.Height = 0;
			RibbonViewInfo vi = ViewInfo as RibbonViewInfo;
			if(vi != null && vi.Header.PageHeaderItems.IndexOf(editItem) != -1)
				res.Height = Math.Min(res.Height, vi.Header.PageHeaderItemHeight);
			return res;
		}
		bool ShouldApplyEditHeight(RibbonItemViewInfo item) {
			BarEditItemLink link = item.Item as BarEditItemLink;
			if(link == null || link.Item.EditHeight == -1) return false;
			if(item.Owner is RibbonPageGroupViewInfo) return true;
			RibbonStatusBarViewInfo sbi = item.Owner as RibbonStatusBarViewInfo;
			return sbi != null && sbi.StatusBar.AutoHeight;
		}
		protected virtual int EditorIndent { 
			get {
				SkinElementInfo info = new SkinElementInfo(GetElement(RibbonSkins.SkinButton));
				return info.Element.ContentMargins.Left;
			} 
		}
		public virtual Size CalcEditItemSize(Graphics graphics, RibbonItemViewInfo item) {
			RibbonEditItemViewInfo editItem = (RibbonEditItemViewInfo)item;
			BarEditItemLink link = (editItem.Item as BarEditItemLink);
			graphics = CheckGraphics(graphics);
			int height = ViewInfo.EditorHeight;
			if(ShouldApplyEditHeight(item)) height = link.Item.EditHeight;
			Size editSize = new Size(GetEditItemSize(editItem).Width, height);
			int width = editSize.Width;
			if(item.IsImageExists) width += item.ImageSize.Width;
			if(item.Text != string.Empty && item.AllowedStyles != RibbonItemStyles.SmallWithoutText) {
				width += CalcSingleLineTextWidth(graphics, item) + CaptionHEditIndent;
				if(item.IsImageExists) width += CaptionHGlyphIndent;
			}
			if(!link.ActAsButtonGroup)
				width += EditorIndent * 2;
			return new Size(width, editSize.Height);
		}
		protected virtual bool ShouldDrawSelection(BarItemLink link) {
			return !(link is RibbonGalleryBarItemLink && (ViewInfo.DesignTimeManager.DragItem is GalleryItem || ViewInfo.DesignTimeManager.DragItem is GalleryItemGroup));
		}
		protected virtual BarItemLink GetSelectedLink(BarItemLink link) {
			if(link.Holder is BarButtonGroup && link.ClonedFromLink != null) return link.ClonedFromLink;
			return link;
		}
		public virtual void DrawSelection(GraphicsCache cache, RibbonItemViewInfo item) {
			BarItemLink link = item.Item as BarItemLink;
			if(link != null && link.Item != null) {
				if(ViewInfo.IsDesignMode && ViewInfo.DesignTimeManager.IsComponentSelected(link.Item)) {
					if(!ShouldDrawSelection(link)) return;
					if(link.Item.LinkProvider == null || link.Item.LinkProvider.Link == GetSelectedLink(link))
						ViewInfo.DesignTimeManager.DrawSelection(cache, item.Bounds, Color.Magenta);
				}
			}
		}
		internal virtual Color GetEditItemBackColor(ObjectState state) { 
			return RibbonSkins.GetSkin(ViewInfo.Provider).Colors[RibbonSkins.OptColorEditorBackground];
		}
		public virtual void DrawEditor(GraphicsCache cache, RibbonEditItemViewInfo editItem) {
			if(editItem.EditBounds.IsEmpty) return;
			XPaint oldPaint = null;
			ControlGraphicsInfoArgs infoArgs = new ControlGraphicsInfoArgs(editItem.EditViewInfo, cache, editItem.EditBounds, ShouldDrawTextOnGlass(editItem));
			if(!ViewInfo.OwnerControl.Enabled)
				infoArgs.ViewInfo.State = ObjectState.Disabled;
			Rectangle maxBounds = infoArgs.Cache.ClipInfo.MaximumBounds;
			Rectangle clipBounds = FromRectangleF(infoArgs.Cache.Graphics.ClipBounds);
			try {
				if(!object.Equals(maxBounds, clipBounds))
					infoArgs.Cache.ClipInfo.MaximumBounds = Rectangle.Intersect(maxBounds, clipBounds);
				editItem.EditPainter.Draw(infoArgs);
				if(oldPaint != null) cache.Paint = oldPaint;
				DrawSelection(cache, editItem);
				DrawReduceOperationSelection(cache, editItem);
			}
			finally {
				infoArgs.Cache.ClipInfo.MaximumBounds = maxBounds;
			}
		}
		public virtual void DrawEditItem(GraphicsCache cache, RibbonItemViewInfo item) {
			CheckItemViewInfo(cache, item);
			DrawGlyph(cache, item);
			DrawText(cache, item);
			RibbonEditItemViewInfo editItem = (RibbonEditItemViewInfo)item;
			DrawEditor(cache, editItem);
		}
		protected Rectangle FromRectangleF(RectangleF rect) {
			return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}
		public virtual void DrawEditItemText(GraphicsCache cache, RibbonItemViewInfo item) {
			if(item.AllowedStyles == RibbonItemStyles.SmallWithoutText)
				return;
			DrawCaption(cache, item);
		}
		bool HasBackgroundColor(AppearanceObject obj) {
			return (obj.BackColor != Color.Empty && obj.BackColor != Color.Transparent) ||
				(obj.BackColor2 != Color.Empty && obj.BackColor2 != Color.Transparent);
		}
		public virtual void DrawPageGroupButtonText(GraphicsCache cache, RibbonItemViewInfo item) {
			RibbonGroupItemViewInfo vi = (RibbonGroupItemViewInfo)item;
			GraphicsClipState state = null;
			if(vi.ShouldDrawDropDownGlyph && vi.ShouldExcludeGlyph) {
				state = cache.ClipInfo.SaveClip();
				cache.ClipInfo.ExcludeClip(vi.TabletDropDownGlyphBounds);
			}
			if(vi.ShouldDrawText)
				DrawCaption(cache, item);
			if(vi.ShouldDrawDropDownGlyph) {
				cache.ClipInfo.RestoreClip(state);
			}
		}
		public virtual void DrawPageGroupButton(GraphicsCache cache, RibbonItemViewInfo item) {
			RibbonGroupItemViewInfo vi = (RibbonGroupItemViewInfo)item;
			GraphicsClipState state = null;
			CheckItemViewInfo(cache, item);
			DrawItemBackground(cache, item);
			if(vi.ShouldDrawDropDownGlyph && vi.ShouldExcludeGlyph) {
				state = cache.ClipInfo.SaveClip();
				cache.ClipInfo.ExcludeClip(vi.TabletDropDownGlyphBounds);
			}
			if(vi.ShouldDrawGlyph)
				DrawGlyph(cache, item);
			if(vi.ShouldDrawText)
				DrawText(cache, item);
			if(vi.ShouldDrawDropDownGlyph) {
				cache.ClipInfo.RestoreClip(state);
				var attributes = ImageColorizer.GetColoredAttributes(item.GetPaintAppearance().GetForeColor());
				cache.Graphics.DrawImage(vi.GetDropDownGlyph(), vi.GetDropDownGlyphBounds(), 0, 0, vi.GetDropDownGlyphBounds().Width, vi.GetDropDownGlyphBounds().Height, GraphicsUnit.Pixel, attributes);
			}
			DrawReduceOperationSelection(cache, item);
		}
		public virtual void DrawButton(GraphicsCache cache, RibbonItemViewInfo item) {
			var checkItem = item as RibbonCheckItemViewInfo;
			CheckItemViewInfo(cache, item);
			DrawItemBackground(cache, item);
			DrawGlyph(cache, item);
			DrawText(cache, item); 
			if(checkItem == null || checkItem.GetCheckBoxVisibility() == CheckBoxVisibility.None)
				DrawSelection(cache, item);
			DrawReduceOperationSelection(cache, item);
			if(checkItem != null && checkItem.GetCheckBoxVisibility() != CheckBoxVisibility.None) DrawCheckBox(cache, item as RibbonCheckItemViewInfo);
		}
		protected virtual void DrawItemBackground(GraphicsCache cache, RibbonItemViewInfo item, Rectangle rect, SkinElementInfo info, AppearanceObject app) {
			var checkItem = item as RibbonCheckItemViewInfo;
			if(checkItem == null || checkItem.GetCheckBoxVisibility() == CheckBoxVisibility.None) {
				if(HasBackgroundColor(app))
					app.DrawBackground(cache, rect);
				else
					ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
			}
		}
		protected internal virtual void DrawItemBackground(GraphicsCache cache, RibbonItemViewInfo item) {
			DrawItemBackground(cache, item, item.Bounds, item.GetItemInfo(), item.Appearance);
		}
		protected internal virtual void DrawItemDropDownBackground(GraphicsCache cache, RibbonItemViewInfo item) { 
			RibbonSplitButtonItemViewInfo splitItem = item as RibbonSplitButtonItemViewInfo;
			if(splitItem == null)
				return;
			DrawItemBackground(cache, item, splitItem.DropButtonBounds, splitItem.GetDropDownInfo(), splitItem.DropDownAppearance);
		}
		protected void DrawReduceOperationSelection(GraphicsCache cache, RibbonItemViewInfo item) {
			if(RibbonReduceOperationHelper.Ribbon != ViewInfo.OwnerControl)
				return;
			if(RibbonReduceOperationHelper.SelectedOperation == null || RibbonReduceOperationHelper.SelectedOperation.Operation == ReduceOperationType.CollapseGroup)
				return;
			if(item.OwnerPageGroup == null || item.OwnerPageGroup.PageGroup != RibbonReduceOperationHelper.SelectedOperation.Group)
				return;
			int linkIndex = item.OwnerPageGroup.PageGroup.ItemLinks.IndexOf((BarItemLink)item.Item);
			if(RibbonReduceOperationHelper.SelectedOperation.Operation == ReduceOperationType.Gallery) {
				if(linkIndex != RibbonReduceOperationHelper.SelectedOperation.ItemLinkIndex)
					return;
			}
			else if(RibbonReduceOperationHelper.SelectedOperation.Operation == ReduceOperationType.LargeButtons ||
				RibbonReduceOperationHelper.SelectedOperation.Operation == ReduceOperationType.SmallButtonsWithText) {
				if(linkIndex < RibbonReduceOperationHelper.SelectedOperation.ItemLinkIndex ||
					linkIndex >= RibbonReduceOperationHelper.SelectedOperation.ItemLinkIndex + RibbonReduceOperationHelper.SelectedOperation.ItemLinksCount)
					return;
			}
			else if(RibbonReduceOperationHelper.SelectedOperation.Operation == ReduceOperationType.ButtonGroups) {
				if(!IsLinkInTheSameButtonGroups(RibbonReduceOperationHelper.SelectedOperation.Group, RibbonReduceOperationHelper.SelectedOperation.ItemLinkIndex, linkIndex))
					return;
			}
			ViewInfo.DesignTimeManager.DrawSelection(cache, item.Bounds, Color.Magenta);
		}
		protected virtual bool IsLinkInTheSameButtonGroups(RibbonPageGroup group, int buttonGroupLinkIndex, int linkIndex) {
			int startLinkIndex = GetButtonGroupStartIndex(group, buttonGroupLinkIndex);
			int count = GetButtonGroupCount(group, startLinkIndex);
			return linkIndex >= startLinkIndex && linkIndex < startLinkIndex + count;
		}
		private int GetButtonGroupCount(RibbonPageGroup group, int startLinkIndex) {
			int res = 0;
			for(int i = startLinkIndex; i < group.ItemLinks.Count; i++) {
				if(group.ItemLinks[i].ActAsButtonGroup)
					res++;
				else
					break;
			}
			return res;
		}
		private int GetButtonGroupStartIndex(RibbonPageGroup group, int buttonGroupLinkIndex) {
			for(int i = buttonGroupLinkIndex; i >= 0; i--) {
				if(!group.ItemLinks[i].ActAsButtonGroup)
					return i + 1;
			}
			return 0;
		}
		protected void DrawText(GraphicsCache cache, RibbonItemViewInfo item) {
			DrawItemText(cache, item);
		}
		public virtual void DrawButtonText(GraphicsCache cache, RibbonItemViewInfo item) {
			DrawCaption(cache, item);
		}
		public virtual void DrawDropDownButtonNoText(GraphicsCache cache, RibbonItemViewInfo item) {
			DrawDropDownButtonCore(cache, item, false, ((RibbonDropDownItemViewInfo)item).AllowDrawDropDownArrow);
		}
		public virtual void DrawDropDownButton(GraphicsCache cache, RibbonItemViewInfo item) {
			DrawDropDownButtonCore(cache, item, true, ((RibbonDropDownItemViewInfo)item).AllowDrawDropDownArrow);
		}
		public virtual void DrawDropDownButtonText(GraphicsCache cache, RibbonItemViewInfo item) {
			bool drawArrow = ((RibbonDropDownItemViewInfo)item).AllowDrawDropDownArrow;
			if(drawArrow)
				DrawCaptionWithArrow(cache, item, true);
			else {
				DrawCaption(cache, item);
			}
		}
		protected virtual void DrawDropDownButtonCore(GraphicsCache cache, RibbonItemViewInfo item, bool drawText, bool drawArrow) {
			CheckItemViewInfo(cache, item);
			DrawItemBackground(cache, item);
			DrawGlyph(cache, item);
			if(drawText)
				DrawText(cache, item);
			else
				if(drawArrow) DrawCaptionWithArrow(cache, item, false);
			DrawSelection(cache, item);
			DrawReduceOperationSelection(cache, item);
		}
		public virtual void DrawLargeDropDownButton(GraphicsCache cache, RibbonItemViewInfo item) {
			CheckItemViewInfo(cache, item);
			DrawItemBackground(cache, item);
			DrawLargeGlyph(cache, item);
			DrawText(cache, item); 
			DrawSelection(cache, item);
			DrawReduceOperationSelection(cache, item);
		}
		public virtual void DrawLargeDropDownButtonText(GraphicsCache cache, RibbonItemViewInfo item) {
			DrawLargeCaption(cache, item, ((RibbonDropDownItemViewInfo)item).AllowDrawDropDownArrow);
		}
		protected virtual bool ShouldDrawWrappedCaption(RibbonItemViewInfo item) {
			bool shouldWrap = item.ShouldWrapText;
			if(!HasItemDefaultSize(item))
				if(!shouldWrap && ShouldWrapItemText(item) && HasSecondTextLine(item))
					shouldWrap = false;
				else shouldWrap = true;
			else {
				if(ShouldWrapItemText(item))
					shouldWrap = true;
			}
			return ViewInfo.LargeButtonTextLinesCount > 1 && shouldWrap;
		}
		protected bool HasItemDefaultSize(RibbonItemViewInfo item) {
			return GetButtonWidthByLevel(item) == 0;
		}
		protected virtual void DrawLargeCaption(GraphicsCache cache, RibbonItemViewInfo item, bool drawArrow) {
			if(ShouldDrawWrappedCaption(item))
				DrawWrappedCaption(cache, item, drawArrow);
			else {
				if(drawArrow) 
					DrawCaptionWithArrow(cache, item, true);
				else
					DrawCaption(cache, item);
			}
		}
		public virtual void DrawLargeButton(GraphicsCache cache, RibbonItemViewInfo item) {
			CheckItemViewInfo(cache, item);
			DrawItemBackground(cache, item);
			DrawLargeGlyph(cache, item);
			DrawText(cache, item); 
			DrawSelection(cache, item);
			DrawReduceOperationSelection(cache, item);
		}
		public virtual void DrawLargeButtonText(GraphicsCache cache, RibbonItemViewInfo item) {
			DrawLargeCaption(cache, item, false);
		}
		public virtual void DrawLargeGroupDropDownButton(GraphicsCache cache, RibbonItemViewInfo item) {
			CheckItemViewInfo(cache, item);
			DrawItemBackground(cache, item);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, item.GetItemInfo());
			DrawLargeGroupGlyph(cache, item);
			DrawText(cache, item); 
		}
		public virtual void DrawLargeGroupDropDownButtonText(GraphicsCache cache, RibbonItemViewInfo item) {
			DrawWrappedCaption(cache, item, true);
		}
		protected virtual void DrawLargeGroupGlyph(GraphicsCache cache, RibbonItemViewInfo item) {
			GlyphInfo info = GetGlyphInfo(item, ObjectState.Normal);
			if(!info.IsImageExits) return;
			SkinElementInfo sl = new SkinElementInfo(GetElement(RibbonSkins.SkinTabPanelGroupMinimizedGlyph), item.GlyphBounds);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, sl);
			Rectangle bounds = RectangleHelper.GetCenterBounds(item.GlyphBounds, ViewInfo.ImageSize);
			info.DrawGlyph(cache, bounds, ViewInfo.OwnerControl.Enabled);
		}
		public virtual void DrawLargeSplitButton(GraphicsCache cache, RibbonItemViewInfo item) {
			RibbonSplitButtonItemViewInfo splitItem = (RibbonSplitButtonItemViewInfo)item;
			CheckItemViewInfo(cache, item);
			DrawItemBackground(cache, item, item.Bounds, splitItem.GetItemInfo(), item.Appearance);
			DrawItemBackground(cache, item, splitItem.DropButtonBounds, splitItem.GetDropDownInfo(), splitItem.DropDownAppearance);
			DrawLargeGlyph(cache, item);
			DrawText(cache, item); 
			DrawSelection(cache, item);
			DrawReduceOperationSelection(cache, item);
		}
		public virtual void DrawLargeSplitButtonText(GraphicsCache cache, RibbonItemViewInfo item) {
			DrawLargeCaption(cache, item, true);
		}
		public virtual void DrawSplitButton(GraphicsCache cache, RibbonItemViewInfo item) {
			RibbonSplitButtonItemViewInfo splitItem = (RibbonSplitButtonItemViewInfo)item;
			CheckItemViewInfo(cache, item);
			SkinElementInfo dropInfo = splitItem.GetDropDownInfo();
			DrawItemBackground(cache, item, item.Bounds, splitItem.GetItemInfo(), item.Appearance);
			DrawItemBackground(cache, item, splitItem.DropButtonBounds, splitItem.GetDropDownInfo(), splitItem.DropDownAppearance);
			Rectangle rect = ObjectPainter.GetObjectClientRectangle(cache.Graphics, SkinElementPainter.Default, dropInfo);
			DrawArrow(cache, RectangleHelper.GetCenterBounds(rect, ViewInfo.ButtonArrowSize), item.CalcState());
			DrawGlyph(cache, item);
			DrawText(cache, item); 
			DrawSelection(cache, item);
			DrawReduceOperationSelection(cache, item);
		}
		public virtual void DrawSplitButtonText(GraphicsCache cache, RibbonItemViewInfo item) {
			DrawCaption(cache, item);
		}
		public virtual void DrawVertSeparator(GraphicsCache cache, RibbonItemViewInfo item) {
			CheckItemViewInfo(cache, item);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, item.GetItemInfo());
		}
		public virtual void DrawButtonGroup(GraphicsCache cache, RibbonItemViewInfo item) {
			CheckItemViewInfo(cache, item);
			RibbonButtonGroupItemViewInfo group = (RibbonButtonGroupItemViewInfo)item;
			SkinElementInfo sep = GetButtonGroupSeparatorElementInfo();
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, group.GetItemInfo());
			Rectangle client = ObjectPainter.GetObjectClientRectangle(cache.Graphics, SkinElementPainter.Default, group.GetItemInfo());
			for(int n = 0; n < group.Items.Count; n++) {
				RibbonItemViewInfo gi = group.Items[n];
				DrawItem(cache, gi);
				if(n != group.Items.Count - 1) {
					Rectangle bounds = gi.Bounds;
					bounds.X = bounds.Right;
					bounds.Width = CalcButtonGroupSeparatorWidth(cache.Graphics);
					bounds.Y = client.Y;
					bounds.Height = client.Height;
					sep.Bounds = bounds;
					ObjectPainter.DrawObject(cache, SkinElementPainter.Default, sep);
				}
			}
			DrawSelection(cache, item);
			DrawReduceOperationSelection(cache, item);
		}
		protected virtual void CheckItemViewInfo(GraphicsCache cache, RibbonItemViewInfo item) {
			item.CheckViewInfo(cache.Graphics);
		}
		protected virtual int CalcSingleLineTextWidth(Graphics graphics, RibbonItemViewInfo item) {
			return CalcLineTextSize(graphics, item, ViewInfo.TextWrapper.UnWrapString(item.Text)).Width;
		}
		public class GlyphInfo {
			public int ImageIndex = -1;
			public Image Glyph = null;
			public object Images = null;
			public ImageAttributes ImageAttributes { get; set; }
			public GlyphInfo(object images, int imageIndex, Image glyph, ImageAttributes attributes) {
				this.Images = images;
				this.ImageIndex = imageIndex;
				this.Glyph = glyph;
				ImageAttributes = attributes;
			}
			public bool IsImageExits { get { return Glyph != null || ImageCollection.IsImageListImageExists(Images, ImageIndex); } }
			public void DrawGlyph(GraphicsCache cache, Rectangle bounds, bool enabled) {
				Image glyph = GetGlyph();
				if(glyph == null)
					return;
				Rectangle rect = ImageLayoutHelper.GetImageBounds(bounds, GetGlyph().Size, ImageLayoutMode.Squeeze);
				if(ImageAttributes != null)
					ImageCollection.DrawImageListImage(cache, Glyph, Images, ImageIndex, rect, ImageAttributes);
				else 
					ImageCollection.DrawImageListImage(cache, Glyph, Images, ImageIndex, rect, enabled);
			}
			public Image GetGlyph() {
				if(Glyph != null)
					return Glyph;
				return ImageCollection.GetImageListImage(Images, ImageIndex);
			}
		}
		protected internal virtual GlyphInfo GetLargeGlyphInfo(RibbonItemViewInfo item, ObjectState state) {
			BarItemLink link = item.Item as BarItemLink;
			BarLargeButtonItemLink largeLink = item.Item as BarLargeButtonItemLink;
			ImageAttributes attr = link.Item.GetAllowGlyphSkinning() ? ImageColorizer.GetColoredAttributes(item.Appearance.ForeColor) : null;
			if(link != null && link.IsLargeImageExist) {
				if(state == ObjectState.Disabled) {
					GlyphInfo res = new GlyphInfo(link.Item.LargeImages, link.Item.LargeImageIndexDisabled, link.Item.LargeGlyphDisabled, attr);
					return res;
				}
				if(largeLink != null && state == ObjectState.Hot) {
					GlyphInfo res = new GlyphInfo(largeLink.Item.LargeImages, largeLink.Item.LargeImageIndexHot, largeLink.Item.LargeGlyphHot, attr);
					if(res.IsImageExits) return res;
				}
				return new GlyphInfo(link.Item.LargeImages, link.Item.LargeImageIndex, link.GetLargeGlyph(item.ViewInfo.LargeImageSize, state), attr);
			}
			return GetGlyphInfo(item, state);
		}
		protected internal virtual GlyphInfo GetGlyphInfo(RibbonItemViewInfo item, ObjectState state) {
			BarItemLink link = item.Item as BarItemLink;
			ImageAttributes attr = link.Item.GetAllowGlyphSkinning() ? ImageColorizer.GetColoredAttributes(item.Appearance.ForeColor) : null;
			if(link != null && (!item.IsInHeaderArea || item.ShowImageInToolbar)) {
				if(state == ObjectState.Disabled) return new GlyphInfo(link.Item.Images, link.Item.ImageIndexDisabled, link.Item.GlyphDisabled, attr);
				GlyphInfo res = new GlyphInfo(link.Item.Images, link.ImageIndex, link.GetGlyph(item.ViewInfo.ImageSize, state), attr);
				if(!res.IsImageExits) return new GlyphInfo(null, -1, item.ExtraGlyph, attr);
				return res;
			}
			return new GlyphInfo(null, -1, null, attr);
		}
		protected virtual void DrawLargeGlyphDisabled(GraphicsCache cache, RibbonItemViewInfo item) {
			GlyphInfo info = GetLargeGlyphInfo(item, ObjectState.Disabled);
			if(info.IsImageExits) {
				info.DrawGlyph(cache, item.GlyphBounds, true);
				return;
			}
			info = GetLargeGlyphInfo(item, ObjectState.Normal);
			info.DrawGlyph(cache, item.GlyphBounds, false);
		}
		protected virtual void DrawLargeGlyph(GraphicsCache cache, RibbonItemViewInfo item) {
			if(!item.Enabled || !ViewInfo.OwnerControl.Enabled) {
				DrawLargeGlyphDisabled(cache, item);
				return;
			}
			GetLargeGlyphInfo(item, item.CalcState()).DrawGlyph(cache, item.GlyphBounds, true);
		}
		protected virtual void DrawGlyphDisabled(GraphicsCache cache, RibbonItemViewInfo item) {
			GlyphInfo info = GetGlyphInfo(item, ObjectState.Disabled);
			if(info.IsImageExits) {
				info.DrawGlyph(cache, item.GlyphBounds, true);
				return;
			}
			info = GetGlyphInfo(item, ObjectState.Normal);
			info.DrawGlyph(cache, item.GlyphBounds, false);
		}
		protected virtual void DrawGlyph(GraphicsCache cache, RibbonItemViewInfo item) {
			if(!item.Enabled || !ViewInfo.OwnerControl.Enabled) {
				DrawGlyphDisabled(cache, item);
				return;
			}
			GetGlyphInfo(item, item.CalcState()).DrawGlyph(cache, item.GlyphBounds, true);
		}
		protected internal virtual void DrawArrow(GraphicsCache cache, Rectangle bounds, ObjectState state) {
			if(bounds == Rectangle.Empty) return;
			SkinElementInfo info = GetArrowElementInfo();
			info.Bounds = bounds;
			info.ImageIndex = -1;
			info.State = state;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		protected virtual void DrawCaptionWithArrow(GraphicsCache cache, RibbonItemViewInfo item, bool drawText) {
			Rectangle caption = item.CaptionBounds;
			DrawCaptionWithArrow(cache, item, caption, drawText ? ViewInfo.TextWrapper.UnWrapString(item.Text) : string.Empty, false);
		}
		protected virtual void DrawCaption(GraphicsCache cache, RibbonItemViewInfo item) {
			Rectangle caption = item.CaptionBounds;
			if(!caption.IsEmpty) {
				if(ShouldDrawTextOnGlass(item))
					DrawItemTextOnGlass(cache, item);
				else {
					HorzAlignment halign = item.TextAlignment;
					if(!item.ShouldWrapText && item.IsLargeButton)
						halign = HorzAlignment.Center;
					VertAlignment valign = item.IsLargeButton ? VertAlignment.Top : VertAlignment.Center;
					item.GetPaintAppearance().DrawString(cache, ViewInfo.TextWrapper.UnWrapString(item.Text), caption, GetStringFormat(item, halign, valign));
				}
			}
		}
		protected void DrawWrappedCaption(GraphicsCache cache, RibbonItemViewInfo item) {
			DrawWrappedCaption(cache, item, false);
		}
		protected virtual void DrawWrappedCaption(GraphicsCache cache, RibbonItemViewInfo item, bool withArrow) {
			Rectangle captionBounds = item.CaptionBounds;
			string[] lines = ViewInfo.TextWrapper.WrapString(item.Text, item.ShouldWrapText);
			captionBounds.Height = ViewInfo.ButtonTextHeight;
			AppearanceObject appearance = item.GetPaintAppearance();
			appearance.DrawString(cache, lines[0], captionBounds, GetStringFormat(item));
			captionBounds.Y += captionBounds.Height + LineIndent;
			if(withArrow)
				DrawCaptionWithArrow(cache, item, captionBounds, lines[1], true);
			else
				appearance.DrawString(cache, lines[1], captionBounds, GetStringFormat(item));
		}
		protected virtual bool ShouldLayoutButtonTextAndArrow(RibbonButtonItemViewInfo buttonInfo) {
			return buttonInfo != null && buttonInfo.CurrentLevel != RibbonItemStyles.Large;
		}
		protected virtual Rectangle LayoutButtonText(GraphicsCache cache, Rectangle bounds, RibbonItemViewInfo item, string text, bool withArrow) {
			RibbonButtonItemViewInfo buttonInfo = item as RibbonButtonItemViewInfo;
			AppearanceObject app = item.GetPaintAppearance();
			Size size = CalcLineTextSize(cache.Graphics, item, text);
			Rectangle rect = bounds;
			bool shouldSubtractArrowWidth = rect.Width < size.Width;
			if(size.Width > 0)
				rect.Width = Math.Min(rect.Width, size.Width);
			int totalArrowWidth = item.ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice? 0: ViewInfo.ButtonArrowSize.Width + ArrowIndent;
			if(ShouldLayoutButtonTextAndArrow(buttonInfo)) {
				if(shouldSubtractArrowWidth)
					rect.Width = Math.Min(rect.Width - totalArrowWidth, size.Width);
				if(app.TextOptions.HAlignment == HorzAlignment.Center)
					rect.X += (bounds.Width - (totalArrowWidth + rect.Width)) / 2;
				else if(app.TextOptions.HAlignment == HorzAlignment.Far)
					rect.X += (bounds.Width - (totalArrowWidth + rect.Width));
			}
			else {
				if(withArrow)
					rect.X = bounds.X + (bounds.Width - (totalArrowWidth + rect.Width)) / 2;
				else
					rect.X = bounds.X + (bounds.Width - rect.Width) / 2;
			}
			if(ViewInfo.IsRightToLeft)
				rect = BarUtilites.ConvertBoundsToRTL(rect, bounds);
			return rect;
		}
		protected virtual Rectangle LayoutArrow(Rectangle bounds, RibbonItemViewInfo item, Rectangle textBounds) {
			RibbonButtonItemViewInfo buttonInfo = item as RibbonButtonItemViewInfo;
			AppearanceObject app = item.GetPaintAppearance();
			Rectangle rect = textBounds;
			if(ShouldLayoutButtonTextAndArrow(buttonInfo)) {
				if(app.TextOptions.HAlignment == HorzAlignment.Center) rect.X = ViewInfo.IsRightToLeft ? rect.Left : rect.Right + ArrowIndent;
				else rect.X = ViewInfo.IsRightToLeft ? bounds.Left : bounds.Right - ViewInfo.ButtonArrowSize.Width;
			}
			else {
				if(item.ShouldWrapText)
					rect.X = ViewInfo.IsRightToLeft ? rect.Left - ArrowIndent - ViewInfo.ButtonArrowSize.Width : rect.Right + ArrowIndent;
				else
					rect.X = rect.X + (rect.Width - ViewInfo.ButtonArrowSize.Width) / 2;
			}
			rect.Width = ViewInfo.ButtonArrowSize.Width;
			if(item.ShouldWrapText || ShouldLayoutButtonTextAndArrow(buttonInfo))
				rect.Y += (rect.Height - ViewInfo.ButtonArrowSize.Height) / 2;
			else
				rect.Y += ViewInfo.ButtonTextHeight + LineIndent + (ViewInfo.ButtonTextHeight - ViewInfo.ButtonArrowSize.Height) / 2;
			rect.Height = ViewInfo.ButtonArrowSize.Height;
			return rect;
		}
		protected virtual void DrawCaptionWithArrow(GraphicsCache cache, RibbonItemViewInfo item, Rectangle bounds, string text, bool withArrow) {
			Rectangle arrow = RectangleHelper.GetCenterBounds(bounds, ViewInfo.ButtonArrowSize);
			if(text != string.Empty) {
				Rectangle rect = LayoutButtonText(cache, bounds, item, text, withArrow);
				HorzAlignment halign = item.TextAlignment;
				if(!item.ShouldWrapText && item.IsLargeButton)
					halign = HorzAlignment.Center;
				VertAlignment valign = item.IsLargeButton ? VertAlignment.Top : VertAlignment.Center;
				if(item.Appearance.TextOptions.VAlignment != VertAlignment.Default)
					valign = item.Appearance.TextOptions.VAlignment;
				item.GetPaintAppearance().DrawString(cache, text, rect, GetStringFormat(item, HorzAlignment.Center, valign));
				arrow = LayoutArrow(bounds, item, rect);
			}
			else if(item.ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice) {
				arrow = LayoutArrow(bounds, item, bounds);
			}
			DrawArrow(cache, arrow, item.CalcState());
		}
		protected StringFormat GetStringFormat(RibbonItemViewInfo item, HorzAlignment horz) { return GetStringFormat(item.GetPaintAppearance(), horz); }
		protected StringFormat GetStringFormat(RibbonItemViewInfo item, HorzAlignment horz, VertAlignment vert) { return GetStringFormat(item.GetPaintAppearance(), horz, vert); }
		protected StringFormat GetStringFormat(RibbonItemViewInfo item) { return GetStringFormat(item, HorzAlignment.Center); }
		protected StringFormat GetStringFormat(AppearanceObject appearance) { return GetStringFormat(appearance, HorzAlignment.Center); }
		protected StringFormat GetStringFormat(AppearanceObject appearance, HorzAlignment horz) {
			return GetStringFormat(appearance, horz, VertAlignment.Default);
		}
		protected StringFormat GetStringFormat(AppearanceObject appearance, HorzAlignment horz, VertAlignment vert) {
			return appearance.GetStringFormat(new TextOptions(horz, vert, WordWrap.NoWrap, Trimming.None, HKeyPrefix.Hide));
		}
		protected internal virtual void DrawCheckBox(GraphicsCache cache, RibbonCheckItemViewInfo item) {
			var linkInfo = item;
			if(linkInfo == null) return;
			var state = item.CalcState();
			var checkInfo = new CheckObjectInfoArgs(cache, AppearanceObject.ControlAppearance);
			checkInfo.Bounds = linkInfo.Rects[DevExpress.XtraBars.ViewInfo.BarLinkParts.Checkbox];
			checkInfo.CheckState = GetCheckState(state);
			checkInfo.State = GetObjectState(state);
			checkInfo.CheckStyle = ((BarCheckItem)((BarCheckItemLink)item.Item).Item).GetCheckStyle();
			ObjectPainter.CalcObjectBounds(cache.Graphics, linkInfo.CheckPainter, checkInfo);
			ObjectPainter.DrawObject(cache, linkInfo.CheckPainter, checkInfo);
		}
		CheckState GetCheckState(ObjectState state) {
			if((state & ObjectState.Selected) != 0) return CheckState.Checked;
			return CheckState.Unchecked;
		}
		ObjectState GetObjectState(ObjectState state) {
			if((state & ObjectState.Disabled) != 0) return ObjectState.Disabled;
			else if((state & ObjectState.Pressed) != 0) return ObjectState.Pressed;
			else if((state & ObjectState.Hot) != 0) return ObjectState.Hot;
			return ObjectState.Normal;
		}
	}
	public class RibbonItemStatusBarViewInfoCalculator : RibbonItemViewInfoCalculator {
		public RibbonItemStatusBarViewInfoCalculator(BaseRibbonViewInfo viewInfo)
			: base(viewInfo) {
		}
		public override SkinElementInfo GetEditElementInfo(RibbonItemViewInfo item) {
			return GetButtonElementInfo(item);
		}
		public override SkinElementInfo GetStaticElementInfo(RibbonItemViewInfo item) {
			return GetButtonElementInfo(item);
		}
		public override SkinElementInfo GetButtonElementInfo(RibbonItemViewInfo item) {
			if(item != null && item.OwnerButtonGroup != null) return new SkinElementInfo(GetElement(RibbonSkins.SkinButtonGroupButton));
			return new SkinElementInfo(GetElement(RibbonSkins.SkinStatusBarButton));
		}
		public override SkinElementInfo GetVertSeparatorElementInfo(RibbonItemViewInfo item) {
			return new SkinElementInfo(GetElement(RibbonSkins.SkinStatusBarSeparator));
		}
	}
}
