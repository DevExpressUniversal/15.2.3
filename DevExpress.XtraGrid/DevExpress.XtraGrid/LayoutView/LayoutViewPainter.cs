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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Layout.Events;
using DevExpress.XtraGrid.Views.Layout.Modes;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Painting;
using DevExpress.XtraLayout.Registrator;
namespace DevExpress.XtraGrid.Views.Layout.Drawing {
	public class LayoutViewSkinCardObjectPainter : SkinGroupObjectPainter {
		LayoutView viewCore = null;
		public LayoutViewSkinCardObjectPainter(IPanelControlOwner owner, ISkinProvider provider, LayoutView view)
			: base(owner, provider) {
			this.viewCore = view;
		}
		protected LayoutView View {
			get { return viewCore; }
		}
		protected bool GetAllowHotTrackCards() {
			return DevExpress.XtraGrid.Registrator.LayoutViewSkinPaintStyle.GetAllowHotTrackCards(Provider);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			GroupObjectInfoArgs args = e as GroupObjectInfoArgs;
			if(GetAllowHotTrackCards()) {
				args.StateIndex = ((e.State & ObjectState.Hot) == ObjectState.Hot) ? 1 : 0;
			}
			else {
				if((args.State & ObjectState.Selected) == 0)
					args.StateIndex = 0;
				else
					args.StateIndex = ((args.State & ObjectState.Hot) == ObjectState.Hot) ? 2 : 1; 
			}
			base.DrawObject(e);
		}
		public override void DrawVString(GraphicsCache cache, AppearanceObject appearance, string text, Rectangle bounds, int angle) {
			int containsIndex; string matchedText;
			if(View.TryGetMatchedTextFromCaption(text, out matchedText, out containsIndex))
				DrawHighlightVString(cache, appearance, text, bounds, matchedText, containsIndex, angle);
			else
				base.DrawVString(cache, appearance, text, bounds, angle);
		}
		protected virtual void DrawHighlightVString(GraphicsCache cache, AppearanceObject appearance, string text, Rectangle bounds, string highlightText, int containsIndex, int angle) {
			if(angle == 0)
				DrawHighlightVStringCore(cache, appearance, text, bounds, highlightText, containsIndex);
			else
				base.DrawVString(cache, appearance, text, bounds, angle); 
		}
		void DrawHighlightVStringCore(GraphicsCache cache, AppearanceObject appearance, string text, Rectangle bounds, string highlightText, int containsIndex) {
			AppearanceDefault highlight = LookAndFeelHelper.GetHighlightSearchAppearance(Provider, false);
			cache.Paint.DrawMultiColorString(cache, bounds, text, highlightText, appearance, appearance.GetTextOptions().GetStringFormat(),
				highlight.ForeColor, highlight.BackColor, false, containsIndex);
		}
		protected override Size CalcCaptionTextSize(GroupObjectInfoArgs info) {
			Size text = base.CalcCaptionTextSize(info);
			if(View.ViewInfo != null) text.Height = Math.Max(text.Height, View.ViewInfo.CalcCardCaptionMaxTextHeight());
			return text;
		}
		protected override TextOptions GetTextOptions() {
			if(View.Appearance != null) return View.Appearance.CardCaption.TextOptions;
			else return new TextOptions(HorzAlignment.Near, VertAlignment.Center, WordWrap.NoWrap, Trimming.None, HKeyPrefix.None);
		}
		protected override void DrawBackground(GroupObjectInfoArgs info) {
		}
		protected override ObjectPainter GetBorderPainter(ObjectInfoArgs e) {
			GroupObjectInfoArgs info = (GroupObjectInfoArgs)e;
			if(info.BorderStyle == BorderStyles.NoBorder && !info.ShowCaption)
				return new SkinCardEmptyBorderPainter(Provider, View);
			return new SkinCardBorderPainter(this, Provider, View);
		}
		protected override SkinElement GetPanelCaptionSkinElement(GroupObjectInfoArgs info) {
			return GetAllowHotTrackCards() ?
				GetCardCaptionElement(info) ?? base.GetPanelCaptionSkinElement(info) :
				base.GetPanelCaptionSkinElement(info);
		}
		protected virtual SkinElement GetCardCaptionElement(GroupObjectInfoArgs info) {
			string name = GridSkins.SkinLayoutViewCardCaption;
			if((info.State & ObjectState.Selected) != 0) {
				name = GridSkins.SkinLayoutViewCardCaptionSelected;
				if(!View.IsFocusedView)
					name = GridSkins.SkinLayoutViewCardCaptionHideSelection;
			}
			return GridSkins.GetSkin(Provider)[name];
		}
		protected override SkinElement GetExpandButtonSkinElement(GroupObjectInfoArgs info) {
			return GetCardExpandButtonElement() ?? base.GetExpandButtonSkinElement(info);
		}
		protected virtual SkinElement GetCardExpandButtonElement() {
			return GridSkins.GetSkin(Provider)[GridSkins.SkinLayoutViewCardExpandButton];
		}
	}
	public class SkinCardBorderPainter : SkinGroupBorderPainter {
		LayoutView viewCore = null;
		LayoutViewSkinCardObjectPainter groupPainter;
		public SkinCardBorderPainter(SkinGroupObjectPainter groupPainter, ISkinProvider provider, LayoutView view)
			: base(groupPainter, provider) {
			this.viewCore = view;
			this.groupPainter = groupPainter as LayoutViewSkinCardObjectPainter;
		}
		protected LayoutView View {
			get { return viewCore; }
		}
		protected bool GetAllowHotTrackCards() {
			return DevExpress.XtraGrid.Registrator.LayoutViewSkinPaintStyle.GetAllowHotTrackCards(Provider);
		}
		protected override SkinElement GetPanelSkinElement(GroupObjectInfoArgs info) {
			return GetAllowHotTrackCards() ?
				GetCardElement(info) ?? base.GetPanelSkinElement(info) :
				base.GetPanelSkinElement(info);
		}
		protected override void DrawObjectCore(SkinElementInfo info, ObjectInfoArgs e) {
			SkinGroupBorderObjectInfoArgs args = e as SkinGroupBorderObjectInfoArgs;
			if(GetAllowHotTrackCards())
				info.ImageIndex = ((args.GroupInfo.State & ObjectState.Hot) == ObjectState.Hot) ? 1 : 0;
			base.DrawObjectCore(info, e);
		}
		protected virtual SkinElement GetCardElement(GroupObjectInfoArgs info) {
			bool hasCaption = info.ShowCaption;
			string name = hasCaption ? GridSkins.SkinLayoutViewCard : GridSkins.SkinLayoutViewCardBorder;
			if((info.State & ObjectState.Selected) != 0) {
				name = hasCaption ? GridSkins.SkinLayoutViewCardSelected : GridSkins.SkinLayoutViewCardBorderSelected;
				if(!View.IsFocusedView)
					name = hasCaption ? GridSkins.SkinLayoutViewCardHideSelection : GridSkins.SkinLayoutViewCardBorderHideSelection;
			}
			return GridSkins.GetSkin(Provider)[name];
		}
	}
	public class SkinCardEmptyBorderPainter : SkinGroupEmptyBorderPainter {
		LayoutView viewCore;
		public SkinCardEmptyBorderPainter(ISkinProvider provider, LayoutView view)
			: base(provider) {
			this.viewCore = view;
		}
		protected LayoutView View {
			get { return viewCore; }
		}
		protected bool GetAllowHotTrackCards() {
			return DevExpress.XtraGrid.Registrator.LayoutViewSkinPaintStyle.GetAllowHotTrackCards(Provider);
		}
		protected override SkinElement GetNoBorderGroupSkinElement(GroupObjectInfoArgs info) {
			return GetAllowHotTrackCards() ?
				GetNoBorderCardElement(info) ?? base.GetNoBorderGroupSkinElement(info) :
				base.GetNoBorderGroupSkinElement(info);
		}
		protected virtual SkinElement GetNoBorderCardElement(GroupObjectInfoArgs info) {
			string name = GridSkins.SkinLayoutViewCardNoBorder;
			if((info.State & ObjectState.Selected) != 0) {
				name = GridSkins.SkinLayoutViewCardNoBorderSelected;
				if(!View.IsFocusedView)
					name = GridSkins.SkinLayoutViewCardNoBorderHideSelection;
			}
			return GridSkins.GetSkin(Provider)[name];
		}
		protected override void DrawNoBorderBackground(ObjectInfoArgs e, SkinElementInfo elementInfo) {
			if(GetAllowHotTrackCards()) {
				SkinGroupBorderObjectInfoArgs args = e as SkinGroupBorderObjectInfoArgs;
				elementInfo.ImageIndex = ((args.GroupInfo.State & ObjectState.Hot) == ObjectState.Hot) ? 1 : 0;
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, elementInfo);
			}
			else View.PaintAppearance.ViewBackground.FillRectangle(e.Cache, e.Bounds);
		}
	}
	public class FlatCardObjectPainter : LCFlatGroupObjectPainter {
		LayoutView viewCore = null;
		protected LayoutView View { get { return viewCore; } }
		public FlatCardObjectPainter(IPanelControlOwner owner, LayoutView view)
			: base(owner) {
			this.viewCore = view;
		}
		protected override void DrawBackground(GroupObjectInfoArgs info) {
			base.DrawBackground(info);
			AppearanceObject content = View.PaintAppearance.Card;
			content.DrawBackground(info.Cache, info.ClientBounds);
		}
		protected override void DrawBorder(GroupObjectInfoArgs info) {
		}
		protected override void DrawCaption(GroupObjectInfoArgs info) {
			if(info.CaptionBounds.IsEmpty) return;
			if(RaiseCustomDrawCaption(info)) return;
			DrawButtonsPanel(info);
			DrawVString(info.Cache, info.AppearanceCaption, info.Caption, info.TextBounds, GetRotateAngle(info));
		}
		protected override Size CalcCaptionTextSize(GroupObjectInfoArgs info) {
			Size text = base.CalcCaptionTextSize(info);
			if(View.ViewInfo != null)
				text.Height = Math.Max(text.Height, View.ViewInfo.CalcCardCaptionMaxTextHeight());
			return text;
		}
	}
	public class FlatCardGroupBorderPainter : LCFlatGroupObjectPainter {
		protected LayoutView viewCore = null;
		public FlatCardGroupBorderPainter(IPanelControlOwner owner, LayoutView view)
			: base(owner) {
			this.viewCore = view;
		}
		protected override void DrawBackground(GroupObjectInfoArgs info) {
			AppearanceObject content = viewCore.PaintAppearance.Card;
			content.DrawBackground(info.Cache, info.Bounds);
		}
		protected override TextOptions GetTextOptions() {
			if(viewCore.Appearance != null) return viewCore.Appearance.CardCaption.TextOptions;
			else return new TextOptions(HorzAlignment.Near, VertAlignment.Center, WordWrap.NoWrap, Trimming.None, HKeyPrefix.None);
		}
	}
	public class Office2003CardObjectPainter : GroupObjectPainter {
		protected LayoutView viewCore = null;
		public Office2003CardObjectPainter(IPanelControlOwner owner, LayoutView view) : base(owner) { this.viewCore = view; }
		protected override void DrawBackground(GroupObjectInfoArgs info) {
			AppearanceObject content = viewCore.PaintAppearance.Card;
			content.DrawBackground(info.Cache, info.ClientBounds);
		}
		protected override int CalcCaptionContentHeight(GroupObjectInfoArgs info) {
			int res = info.AppearanceCaption.CalcDefaultTextSize(info.Graphics).Height;
			if(viewCore.ViewInfo != null) {
				res = Math.Max(res, viewCore.ViewInfo.CalcCardCaptionMaxTextHeight());
			}
			int maxtextAndButton = Math.Max(res, CalcCaptionButtonSize(info).Height);
			return Math.Max(CalcCaptionImageSize(info).Height, maxtextAndButton) + 4;
		}
		protected override void DrawCaption(GroupObjectInfoArgs info) {
			if(info.CaptionBounds.IsEmpty) return;
			info.AppearanceCaption.DrawBackground(info.Cache, info.CaptionBounds);
			if(RaiseCustomDrawCaption(info)) return;
			AppearanceObject caption = CalcCaptionApearance(info); ;
			DrawVString(info.Cache, caption, info.Caption, info.TextBounds, GetRotateAngle(info));
			DrawButtonsPanel(info);
		}
		protected AppearanceObject CalcCaptionApearance(GroupObjectInfoArgs info) {
			AppearanceObject caption = viewCore.PaintAppearance.CardCaption;
			if(info.State == ObjectState.Selected) {
				caption = viewCore.PaintAppearance.FocusedCardCaption;
			}
			return caption;
		}
	}
	public class Office2003CardGroupBorderPainter : GroupObjectPainter {
		protected LayoutView viewCore = null;
		public Office2003CardGroupBorderPainter(IPanelControlOwner owner, LayoutView view) : base(owner) { this.viewCore = view; }
		protected override void DrawBackground(GroupObjectInfoArgs info) {
			AppearanceObject content = viewCore.PaintAppearance.Card;
			content.DrawBackground(info.Cache, info.Bounds);
		}
	}
	public abstract class LayoutViewCardPainter : LayoutControlGroupPainter {
		LayoutView viewCore;
		protected LayoutViewCardPainter(LayoutView view)
			: base() {
			this.viewCore = view;
		}
		public LayoutView View {
			get { return viewCore; }
		}
		public int CardHandle {
			get { return (View.DrawCard != null) ? View.DrawCard.RowHandle : GridControl.InvalidRowHandle; }
		}
		protected override void OnCustomDrawCaptionCore(GroupCaptionCustomDrawEventArgs e) {
			LayoutViewCustomDrawCardCaptionEventArgs ea = new LayoutViewCustomDrawCardCaptionEventArgs(CardHandle, e);
			View.RaiseCustomDrawCardCaption(ea);
			e.Handled = ea.Handled;
		}
	}
	public class LayoutViewFlatCardPainter : LayoutViewCardPainter {
		public LayoutViewFlatCardPainter(LayoutView view) : base(view) { }
		protected override ObjectPainter CreateBorderPainter(DevExpress.XtraLayout.ViewInfo.BaseViewInfo e) {
			LayoutViewCard card = e.Owner as LayoutViewCard;
			return ((LayoutViewFlatPaintStyle)card.CardPaintStyle).CreateCardBorderPainter(this, View);
		}
	}
	public class LayoutViewWindowsXPCardPainter : LayoutViewCardPainter {
		public LayoutViewWindowsXPCardPainter(LayoutView view) : base(view) { }
		protected override ObjectPainter CreateBorderPainter(DevExpress.XtraLayout.ViewInfo.BaseViewInfo e) {
			LayoutViewCard card = e.Owner as LayoutViewCard;
			return ((LayoutViewWindowsXPPaintStyle)card.CardPaintStyle).CreateCardBorderPainter(this, View);
		}
	}
	public class LayoutViewUltraFlatCardPainter : LayoutViewCardPainter {
		public LayoutViewUltraFlatCardPainter(LayoutView view) : base(view) { }
		protected override ObjectPainter CreateBorderPainter(DevExpress.XtraLayout.ViewInfo.BaseViewInfo e) {
			LayoutViewCard card = e.Owner as LayoutViewCard;
			return ((LayoutViewUltraFlatPaintStyle)card.CardPaintStyle).CreateCardBorderPainter(this, View);
		}
	}
	public class LayoutViewStyle3DCardPainter : LayoutViewCardPainter {
		public LayoutViewStyle3DCardPainter(LayoutView view) : base(view) { }
		protected override ObjectPainter CreateBorderPainter(DevExpress.XtraLayout.ViewInfo.BaseViewInfo e) {
			LayoutViewCard card = e.Owner as LayoutViewCard;
			return ((LayoutViewStyle3DPaintStyle)card.CardPaintStyle).CreateCardBorderPainter(this, View);
		}
	}
	public class LayoutViewOffice2003CardPainter : LayoutViewCardPainter {
		public LayoutViewOffice2003CardPainter(LayoutView view) : base(view) { }
		protected override ObjectPainter CreateBorderPainter(DevExpress.XtraLayout.ViewInfo.BaseViewInfo e) {
			LayoutViewCard card = e.Owner as LayoutViewCard;
			return ((LayoutViewOffice2003PaintStyle)card.CardPaintStyle).CreateCardBorderPainter(this, View);
		}
	}
	public class LayoutViewCardSkinPainter : LayoutViewCardPainter {
		public LayoutViewCardSkinPainter(LayoutView view) : base(view) { }
		protected override ObjectPainter CreateBorderPainter(DevExpress.XtraLayout.ViewInfo.BaseViewInfo e) {
			LayoutViewCard card = e.Owner as LayoutViewCard;
			return ((LayoutViewSkinPaintStyle)card.CardPaintStyle).CreateCardBorderPainter(this);
		}
	}
	public static class FieldCustomDrawHelper {
		public static bool CustomDrawText(DevExpress.XtraLayout.ViewInfo.BaseLayoutItemViewInfo e, LayoutView view, MethodInvoker defaultDraw) {
			LayoutRepositoryItemViewInfo viewInfo = e as LayoutRepositoryItemViewInfo;
			LayoutViewField field = viewInfo.Owner as LayoutViewField;
			if(field != null) {
				object editValue = viewInfo.RepositoryItemViewInfo.EditValue;
				RowCellCustomDrawEventArgs ea = new RowCellCustomDrawEventArgs(viewInfo.Cache, viewInfo.TextAreaRelativeToControl,
						viewInfo.Owner.PaintAppearanceItemCaption, view.DrawCard.RowHandle, field.Column,
						editValue, viewInfo.Owner.Text
					);
				ea.SetDefaultDraw(defaultDraw);
				view.RaiseCustomDrawFieldCaption(ea);
				if(editValue != ea.cellValue)
					viewInfo.RepositoryItemViewInfo.EditValue = ea.CellValue;
				viewInfo.LabelInfo.DisplayText = ea.DisplayText;
				if(ea.Handled) return true;
			}
			return false;
		}
		public static bool CustomDrawFieldValue(LayoutRepositoryItemViewInfo viewInfo, LayoutView view, out RowCellCustomDrawEventArgs ea, MethodInvoker defaultDraw) {
			LayoutViewField field = viewInfo.Owner as LayoutViewField;
			ea = null;
			if(field != null) {
				var editViewInfo = viewInfo.RepositoryItemViewInfo;
				object editValue = editViewInfo.EditValue;
				string displayText = view.RaiseCustomColumnDisplayText(
					view.DrawCard.RowHandle, field.Column, editViewInfo.EditValue, editViewInfo.DisplayText, false);
				ea = new RowCellCustomDrawEventArgs(viewInfo.Cache, editViewInfo.Bounds,
					editViewInfo.PaintAppearance, view.DrawCard.RowHandle, field.Column, editValue, displayText);
				ea.SetDefaultDraw(defaultDraw);
				view.RaiseCustomDrawFieldValue(ea);
				if(editValue != ea.cellValue)
					editViewInfo.EditValue = ea.CellValue;
				editViewInfo.SetDisplayText(ea.DisplayText);
				if(ea.Handled) return true;
			}
			return false;
		}
	}
	public class LayoutViewFieldPainter : LayoutRepositoryItemPainter {
		protected readonly LayoutView View;
		public LayoutViewFieldPainter(LayoutView view) {
			View = view;
		}
		protected override void DrawTextArea(DevExpress.XtraLayout.ViewInfo.BaseLayoutItemViewInfo e) {
			MethodInvoker defaultDraw = () => base.DrawTextArea(e);
			if(!FieldCustomDrawHelper.CustomDrawText(e, View, defaultDraw))
				defaultDraw();
		}
		protected override void DrawFieldValue(LayoutRepositoryItemViewInfo viewInfo) {
			RowCellCustomDrawEventArgs cellStyle = null;
			MethodInvoker defaultDraw = () => DrawFieldCore(viewInfo, cellStyle);
			if(!FieldCustomDrawHelper.CustomDrawFieldValue(viewInfo, View, out cellStyle, defaultDraw))
				defaultDraw();
		}
		void DrawFieldCore(LayoutRepositoryItemViewInfo viewInfo, RowCellCustomDrawEventArgs cellStyle) {
			cellStyle.Appearance.DrawBackground(viewInfo.Cache, viewInfo.ClientAreaRelativeToControl);
			UpdateFindHighlight(viewInfo);
			DrawAppearanceMethod drawMethod = (cache, appearance) =>
			{
				View.GridControl.EditorHelper.DrawCellEdit(
						viewInfo, viewInfo.RepositoryItem, viewInfo.RepositoryItemViewInfo, appearance, Point.Empty
					);
			};
			if(View.DrawCard == null || !View.DrawCard.CheckDrawFormat(viewInfo.Cache, cellStyle.Column, viewInfo.RepositoryItemViewInfo, drawMethod, cellStyle.Appearance))
				drawMethod(viewInfo.Cache, cellStyle.Appearance);
		}
		protected void UpdateFindHighlight(LayoutRepositoryItemViewInfo viewInfo) {
			LayoutViewField owner = viewInfo.Owner as LayoutViewField;
			if(owner == null || owner.Column == null) return;
			viewInfo.RepositoryItemViewInfo.MatchedString = "";
			if(View.IsAllowHighlightFind(owner.Column)) {
				viewInfo.RepositoryItemViewInfo.UseHighlightSearchAppearance = true;
				viewInfo.RepositoryItemViewInfo.MatchedStringUseContains = true;
				viewInfo.RepositoryItemViewInfo.MatchedString = View.GetFindMatchedText(owner.Column, viewInfo.RepositoryItemViewInfo.DisplayText);
			}
		}
	}
	public class LayoutViewFieldSkinPainter : LayoutRepositoryItemSkinPainter {
		protected readonly LayoutView View;
		public LayoutViewFieldSkinPainter(LayoutView view) {
			View = view;
		}
		protected override void DrawTextArea(DevExpress.XtraLayout.ViewInfo.BaseLayoutItemViewInfo e) {
			MethodInvoker defaultDraw = () => base.DrawTextArea(e);
			if(!FieldCustomDrawHelper.CustomDrawText(e, View, defaultDraw))
				defaultDraw();
		}
		protected override void DrawFieldValue(LayoutRepositoryItemViewInfo viewInfo) {
			RowCellCustomDrawEventArgs cellStyle = null;
			MethodInvoker defaultDraw = () => DrawFieldCore(viewInfo, cellStyle);
			if(!FieldCustomDrawHelper.CustomDrawFieldValue(viewInfo, View, out cellStyle, defaultDraw))
				defaultDraw();
		}
		void DrawFieldCore(LayoutRepositoryItemViewInfo viewInfo, RowCellCustomDrawEventArgs cellStyle) {
			cellStyle.Appearance.DrawBackground(viewInfo.Cache, viewInfo.ClientAreaRelativeToControl);
			UpdateFindHighlight(viewInfo);
			DrawAppearanceMethod drawMethod = (cache, appearance) =>
			{
				View.GridControl.EditorHelper.DrawCellEdit(
						viewInfo, viewInfo.RepositoryItem, viewInfo.RepositoryItemViewInfo, appearance, Point.Empty
					);
			};
			if(View.DrawCard == null || !View.DrawCard.CheckDrawFormat(viewInfo.Cache, cellStyle.Column, viewInfo.RepositoryItemViewInfo, drawMethod, cellStyle.Appearance))
				drawMethod(viewInfo.Cache, cellStyle.Appearance);
		}
		protected void UpdateFindHighlight(LayoutRepositoryItemViewInfo viewInfo) {
			LayoutViewField owner = viewInfo.Owner as LayoutViewField;
			if(owner == null || owner.Column == null) return;
			viewInfo.RepositoryItemViewInfo.MatchedString = "";
			if(View.IsAllowHighlightFind(owner.Column)) {
				viewInfo.RepositoryItemViewInfo.UseHighlightSearchAppearance = true;
				viewInfo.RepositoryItemViewInfo.MatchedStringUseContains = true;
				viewInfo.RepositoryItemViewInfo.MatchedString = View.GetFindMatchedText(owner.Column, viewInfo.RepositoryItemViewInfo.DisplayText);
			}
		}
	}
	public class LayoutViewFlatCardGroupPainter : LayoutControlGroupPainter {
		readonly LayoutView View;
		public LayoutViewFlatCardGroupPainter(LayoutView view) : base() { this.View = view; }
		protected override ObjectPainter CreateBorderPainter(DevExpress.XtraLayout.ViewInfo.BaseViewInfo e) {
			return new FlatCardGroupBorderPainter(null, View);
		}
	}
	public class LayoutViewOffice2003CardGroupPainter : LayoutControlGroupPainter {
		readonly LayoutView View;
		public LayoutViewOffice2003CardGroupPainter(LayoutView view) : base() { this.View = view; }
		protected override ObjectPainter CreateBorderPainter(DevExpress.XtraLayout.ViewInfo.BaseViewInfo e) {
			return new Office2003CardGroupBorderPainter(null, View);
		}
	}
	public class LayoutViewFlatPaintStyle : FlatPaintStyle {
		protected readonly LayoutView View;
		public LayoutViewFlatPaintStyle(ISupportLookAndFeel owner, LayoutView view) : base(owner) { this.View = view; }
		LayoutViewFlatCardPainter cardFlatPainter = null;
		LayoutViewFlatCardGroupPainter cardFlatCardGroupPainter = null;
		public virtual LayoutViewFlatCardPainter GetCardPainter() {
			if(cardFlatPainter == null) cardFlatPainter = new LayoutViewFlatCardPainter(View);
			return cardFlatPainter;
		}
		LayoutViewFieldPainter cardFieldPainter = null;
		public virtual LayoutViewFieldPainter GetFieldPainter() {
			if(cardFieldPainter == null) cardFieldPainter = new LayoutViewFieldPainter(View);
			return cardFieldPainter;
		}
		public virtual LayoutViewFlatCardGroupPainter GetCardGroupPainter() {
			if(cardFlatCardGroupPainter == null) cardFlatCardGroupPainter = new LayoutViewFlatCardGroupPainter(View);
			return cardFlatCardGroupPainter;
		}
		public override BaseLayoutItemPainter GetPainter(BaseLayoutItem item) {
			if(item is LayoutViewCard) return GetCardPainter();
			if(item is LayoutViewField) return GetFieldPainter();
			if(item is LayoutControlGroup) return GetCardGroupPainter();
			return base.GetPainter(item);
		}
		public virtual GroupObjectPainter CreateCardBorderPainter(IPanelControlOwner owner, LayoutView view) {
			return new FlatCardObjectPainter(owner, view);
		}
	}
	public class LayoutViewWindowsXPPaintStyle : LayoutWindowsXPPaintStyle {
		protected readonly LayoutView View;
		public LayoutViewWindowsXPPaintStyle(ISupportLookAndFeel owner, LayoutView view) : base(owner) { this.View = view; }
		LayoutViewWindowsXPCardPainter cardFlatPainter = null;
		LayoutViewFlatCardGroupPainter cardFlatCardGroupPainter = null;
		public virtual LayoutViewWindowsXPCardPainter GetCardPainter() {
			if(cardFlatPainter == null) cardFlatPainter = new LayoutViewWindowsXPCardPainter(View);
			return cardFlatPainter;
		}
		LayoutViewFieldPainter cardFieldPainter = null;
		public virtual LayoutViewFieldPainter GetFieldPainter() {
			if(cardFieldPainter == null) cardFieldPainter = new LayoutViewFieldPainter(View);
			return cardFieldPainter;
		}
		public virtual LayoutViewFlatCardGroupPainter GetCardGroupPainter() {
			if(cardFlatCardGroupPainter == null) cardFlatCardGroupPainter = new LayoutViewFlatCardGroupPainter(View);
			return cardFlatCardGroupPainter;
		}
		public override BaseLayoutItemPainter GetPainter(BaseLayoutItem item) {
			if(item is LayoutViewCard) return GetCardPainter();
			if(item is LayoutViewField) return GetFieldPainter();
			if(item is LayoutControlGroup) return GetCardGroupPainter();
			return base.GetPainter(item);
		}
		public virtual GroupObjectPainter CreateCardBorderPainter(IPanelControlOwner owner, LayoutView view) {
			return new FlatCardObjectPainter(owner, view);
		}
	}
	public class LayoutViewMixedXPPaintStyle : LayoutViewWindowsXPPaintStyle {
		public LayoutViewMixedXPPaintStyle(ISupportLookAndFeel owner, LayoutView view)
			: base(owner, view) {
		}
	}
	public class LayoutViewUltraFlatPaintStyle : UltraFlatPaintStyle {
		protected readonly LayoutView View;
		public LayoutViewUltraFlatPaintStyle(ISupportLookAndFeel owner, LayoutView view) : base(owner) { this.View = view; }
		protected LayoutViewUltraFlatCardPainter cardFlatPainter = null;
		protected LayoutViewFlatCardGroupPainter cardFlatCardGroupPainter = null;
		public virtual LayoutViewUltraFlatCardPainter GetCardPainter() {
			if(cardFlatPainter == null) cardFlatPainter = new LayoutViewUltraFlatCardPainter(View);
			return cardFlatPainter;
		}
		LayoutViewFieldPainter cardFieldPainter = null;
		public virtual LayoutViewFieldPainter GetFieldPainter() {
			if(cardFieldPainter == null) cardFieldPainter = new LayoutViewFieldPainter(View);
			return cardFieldPainter;
		}
		public virtual LayoutViewFlatCardGroupPainter GetCardGroupPainter() {
			if(cardFlatCardGroupPainter == null) cardFlatCardGroupPainter = new LayoutViewFlatCardGroupPainter(View);
			return cardFlatCardGroupPainter;
		}
		public override BaseLayoutItemPainter GetPainter(BaseLayoutItem item) {
			if(item is LayoutViewCard) return GetCardPainter();
			if(item is LayoutViewField) return GetFieldPainter();
			if(item is LayoutControlGroup) return GetCardGroupPainter();
			return base.GetPainter(item);
		}
		public virtual GroupObjectPainter CreateCardBorderPainter(IPanelControlOwner owner, LayoutView view) {
			return new FlatCardObjectPainter(owner, view);
		}
	}
	public class LayoutViewStyle3DPaintStyle : Style3DPaintStyle {
		protected readonly LayoutView View;
		public LayoutViewStyle3DPaintStyle(ISupportLookAndFeel owner, LayoutView view) : base(owner) { this.View = view; }
		protected LayoutViewStyle3DCardPainter cardFlatPainter = null;
		protected LayoutViewFlatCardGroupPainter cardFlatCardGroupPainter = null;
		public virtual LayoutViewStyle3DCardPainter GetCardPainter() {
			if(cardFlatPainter == null) cardFlatPainter = new LayoutViewStyle3DCardPainter(View);
			return cardFlatPainter;
		}
		LayoutViewFieldPainter cardFieldPainter = null;
		public virtual LayoutViewFieldPainter GetFieldPainter() {
			if(cardFieldPainter == null) cardFieldPainter = new LayoutViewFieldPainter(View);
			return cardFieldPainter;
		}
		public virtual LayoutViewFlatCardGroupPainter GetCardGroupPainter() {
			if(cardFlatCardGroupPainter == null) cardFlatCardGroupPainter = new LayoutViewFlatCardGroupPainter(View);
			return cardFlatCardGroupPainter;
		}
		public override BaseLayoutItemPainter GetPainter(BaseLayoutItem item) {
			if(item is LayoutViewCard) return GetCardPainter();
			if(item is LayoutViewField) return GetFieldPainter();
			if(item is LayoutControlGroup) return GetCardGroupPainter();
			return base.GetPainter(item);
		}
		public virtual GroupObjectPainter CreateCardBorderPainter(IPanelControlOwner owner, LayoutView view) {
			return new FlatCardObjectPainter(owner, view);
		}
	}
	public class LayoutViewOffice2003PaintStyle : LayoutOffice2003PaintStyle {
		protected readonly LayoutView View;
		public LayoutViewOffice2003PaintStyle(ISupportLookAndFeel owner, LayoutView view) : base(owner) { this.View = view; }
		LayoutViewOffice2003CardPainter cardFlatPainter = null;
		LayoutViewOffice2003CardGroupPainter cardFlatCardGroupPainter = null;
		public virtual LayoutViewOffice2003CardPainter GetCardPainter() {
			if(cardFlatPainter == null) cardFlatPainter = new LayoutViewOffice2003CardPainter(View);
			return cardFlatPainter;
		}
		LayoutViewFieldPainter cardFieldPainter = null;
		public virtual LayoutViewFieldPainter GetFieldPainter() {
			if(cardFieldPainter == null) cardFieldPainter = new LayoutViewFieldPainter(View);
			return cardFieldPainter;
		}
		public virtual LayoutViewOffice2003CardGroupPainter GetCardGroupPainter() {
			if(cardFlatCardGroupPainter == null) cardFlatCardGroupPainter = new LayoutViewOffice2003CardGroupPainter(View);
			return cardFlatCardGroupPainter;
		}
		public override BaseLayoutItemPainter GetPainter(BaseLayoutItem item) {
			if(item is LayoutViewCard) return GetCardPainter();
			if(item is LayoutViewField) return GetFieldPainter();
			if(item is LayoutControlGroup) return GetCardGroupPainter();
			return base.GetPainter(item);
		}
		public virtual GroupObjectPainter CreateCardBorderPainter(IPanelControlOwner owner, LayoutView view) {
			return new Office2003CardObjectPainter(owner, view);
		}
	}
	public class LayoutViewSkinPaintStyle : LayoutSkinPaintStyle {
		protected readonly LayoutView View;
		public LayoutViewSkinPaintStyle(ISupportLookAndFeel owner, LayoutView view)
			: base(owner) {
			this.View = view;
		}
		LayoutViewCardSkinPainter cardSkinPainter = null;
		public virtual LayoutViewCardSkinPainter GetCardPainter() {
			if(cardSkinPainter == null) cardSkinPainter = new LayoutViewCardSkinPainter(View);
			return cardSkinPainter;
		}
		LayoutViewFieldSkinPainter cardFieldPainter = null;
		public virtual LayoutViewFieldSkinPainter GetFieldPainter() {
			if(cardFieldPainter == null) cardFieldPainter = new LayoutViewFieldSkinPainter(View);
			return cardFieldPainter;
		}
		public override BaseLayoutItemPainter GetPainter(BaseLayoutItem item) {
			if(item is LayoutViewCard) return GetCardPainter();
			if(item is LayoutViewField) return GetFieldPainter();
			return base.GetPainter(item);
		}
		public virtual GroupObjectPainter CreateCardBorderPainter(IPanelControlOwner owner) {
			return new LayoutViewSkinCardObjectPainter(owner, LookAndFeel, View);
		}
	}
	public class LayoutViewPainter : ColumnViewPainter {
		protected LayoutViewElementsPainter elementsPainterCore;
		public LayoutViewPainter(BaseView view)
			: base(view) {
			this.elementsPainterCore = CreateElementsPainter(view);
		}
		protected virtual LayoutViewElementsPainter CreateElementsPainter(BaseView view) {
			return (view.PaintStyle as LayoutViewPaintStyle).CreateElementsPainter(view);
		}
		public virtual LayoutViewElementsPainter ElementsPainter {
			get { return elementsPainterCore; }
		}
		public new LayoutView View { 
			get { return base.View as LayoutView; } 
		}
		public override void Draw(ViewDrawArgs ee) {
			base.Draw(ee);
			LayoutViewDrawArgs e = ee as LayoutViewDrawArgs;
			DrawViewRects(e);
			Rectangle clipRect = e.ViewInfo.ViewRects.CardsRect;
			GraphicsClipState saveClip = e.Cache.ClipInfo.SaveAndSetClip(clipRect);
			try {
				if(!View.IsTouchScroll) {
					if(TouchLoadingAnimationInfo != null)
						TouchLoadingAnimationInfo.StopAnimation();
					PaintAnimatedCards(ee);
					LayoutViewCarouselMode mode = e.ViewInfo.layoutManager as LayoutViewCarouselMode;
					if(mode == null || (mode != null && !mode.IsAnimated)) {
						using(var pea = new PaintEventArgs(e.Graphics, clipRect)) {
							LayoutViewDrawArgs clippedDrawArgs = new LayoutViewDrawArgs(new GraphicsCache(pea), e.ViewInfo, clipRect);
							List<ButtonInfo> sortFilterButtons = new List<ButtonInfo>();
							foreach(LayoutViewCard layoutCard in e.ViewInfo.VisibleCards) {
								e.ViewInfo.UpdateBeforePaint(layoutCard);
								DrawCard(clippedDrawArgs, layoutCard);
								DrawFocus(e, layoutCard);
								if(IsHotCard(e, layoutCard)) {
									if(View.SortInfo.Count > 0) 
										PrepareActiveSortingButtons(layoutCard, e.ViewInfo.SelectionInfo.HotTrackedInfo, sortFilterButtons);
									if(CanShowFieldHeaderButtons(e, layoutCard)) 
										PrepareHotFieldHeaderButtons(e, sortFilterButtons, layoutCard);
								}
							}
							if(View.FilterPopup != null)
								PrepareActiveFilteringButtons(sortFilterButtons);
							DrawFieldHeaderButtons(e, sortFilterButtons);
							if(View.OptionsView.ShowCardLines) 
								DrawCardLines(e);
							DrawSelectionRect(e);
						}
					}
				}
				else DrawTouchCards(e);
			}
			finally { e.Cache.ClipInfo.RestoreClipRelease(saveClip); }
			if(View.IsShowFilterPanel) DrawFilterPanel(ee);
			if(View.OptionsView.ShowHeaderPanel) DrawHeader(e);
			if(View.RowCount == 0) View.RaiseCustomDrawEmptyForeground(
						new CustomDrawEventArgs(e.Cache, e.ViewInfo.ViewRects.CardsRect, e.ViewInfo.View.Appearance.Card)
					);
		}
		void DrawTouchCards(LayoutViewDrawArgs e) {
			if(e.ViewInfo.layoutManager is AnimatedLayoutViewMode)
				return;
			Point touchOffset = new Point(View.TouchScrollOffset, 0);
			if(!e.ViewInfo.NavigationHScrollNeed)
				touchOffset = new Point(0, View.TouchScrollOffset);
			Rectangle clip = Rectangle.Ceiling(e.Graphics.ClipBounds);
			foreach(LayoutViewCard card in e.ViewInfo.VisibleCards) {
				Rectangle r = card.Bounds;
				r.Offset(touchOffset);
				if(clip.IntersectsWith(r)) 
					DrawCardWithOffset(e, card, r.Location);
			}
			DrawCardLines(e, touchOffset);
			DrawCardLoadingIndicator(e, touchOffset, View.TouchScrollOffset > 0);
		}
		void DrawCardLoadingIndicator(LayoutViewDrawArgs e, Point touchOffset, bool forward) {
			loadingCard.Owner = View;
			Rectangle cards = e.ViewInfo.ViewRects.CardsRect;
			var touchCards = ((LayoutViewBaseMode)e.ViewInfo.layoutManager).GetTouchScrollCards(forward);
			int[] touchCardRowHandles = new int[touchCards.Length];
			int startRowHandle = touchCards[forward ? 0 : touchCards.Length - 1].RowHandle;
			for(int i = 0; i < touchCards.Length; i++) {
				int index = View.GetVisibleIndex(startRowHandle);
				touchCardRowHandles[i] = forward ?
					View.GetPrevVisibleRow(index) :
					View.GetNextVisibleRow(index);
				startRowHandle = touchCardRowHandles[i];
			}
			Rectangle clip = Rectangle.Ceiling(e.Graphics.ClipBounds);
			for(int i = 0; i < touchCardRowHandles.Length; i++) {
				int pos = forward ? (touchCards.Length - 1) - i : i;
				if(View.IsDataRow(touchCardRowHandles[pos])) {
					Rectangle r = touchCards[i].Bounds;
					if(e.ViewInfo.NavigationHScrollNeed)
						r.X = forward ? cards.Left - r.Width : cards.Right;
					else
						r.Y = forward ? cards.Top - r.Height : cards.Bottom;
					r.Offset(touchOffset);
					PrepareTouchCard(e, touchCardRowHandles[pos], r.Size);
					using(Image img = ((LayoutViewBaseMode)e.ViewInfo.layoutManager).GetCardImage(loadingCard))
						e.Graphics.DrawImage(img, r);
					if(TouchLoadingAnimationInfo != null) {
						if(!View.IsActualTouchScroll || !cards.IntersectsWith(r))
							TouchLoadingAnimationInfo.StopAnimation(i);
						else 
							TouchLoadingAnimationInfo.DrawAnimatedItem(e.Cache, r, i);
					}
				}
			}
			loadingCard.Owner = null;
		}
		LayoutViewCard loadingCard = new LayoutViewCard();
		void PrepareTouchCard(LayoutViewDrawArgs e, int rowHandle, Size size) {
			loadingCard.BeginUpdate();
			loadingCard.Text = View.GetCardCaption(rowHandle);
			loadingCard.Size = size;
			loadingCard.GroupBordersVisible = View.CalcCardGroupBordersVisibility();
			bool collapsed = View.GetCardCollapsed(rowHandle);
			loadingCard.AllowChangeTextLocationOnExpanding = collapsed && (e.ViewInfo.CardCollapsingMode == CardCollapsingMode.Vertical);
			loadingCard.Expanded = !collapsed;
			if(!loadingCard.GroupBordersVisible)
				loadingCard.AllowDrawBackground = View.OptionsView.ShowCardBorderIfCaptionHidden;
			loadingCard.EndUpdate();
		}
		TouchLoadingAnimation touchLoadingAnimationCore;
		TouchLoadingAnimation TouchLoadingAnimationInfo {
			get {
				if(touchLoadingAnimationCore == null) {
					if(View == null || View.GridControl == null) return null;
					touchLoadingAnimationCore = new TouchLoadingAnimation(View, View.GridControl);
				}
				return touchLoadingAnimationCore;
			}
		}
		class TouchLoadingAnimation {
			GridControl gridControl;
			LayoutView view;
			List<TouchLoadingAnimator> animators;
			public TouchLoadingAnimation(LayoutView view, GridControl gridControl) {
				this.view = view;
				this.gridControl = gridControl;
				this.animators = new List<TouchLoadingAnimator>();
			}
			Image imageCore = null;
			Image Image {
				get {
					if(this.imageCore != null)
						return this.imageCore;
					if(gridControl != null) {
						SkinElement element = CommonSkins.GetSkin(gridControl.LookAndFeel)[CommonSkins.SkinLoadingBig];
						if(element != null && element.Image != null && element.Image.Image != null)
							this.imageCore = element.Image.Image;
					}
					if(imageCore == null)
						imageCore = LoadingAnimator.LoadingImageBig;
					return this.imageCore;
				}
			}
			internal void DrawAnimatedItem(GraphicsCache graphicsCache, Rectangle r, int index) {
				while(index >= animators.Count)
					animators.Add(new TouchLoadingAnimator(this));
				animators[index].Bounds = r;
				animators[index].DrawAnimatedItem(graphicsCache, Rectangle.Inflate(r, -2, -2));
			}
			int canInvalidate;
			internal void StopAnimation() {
				canInvalidate++;
				foreach(var animator in animators)
					animator.StopAnimation();
				canInvalidate--;
			}
			internal void StopAnimation(int i) {
				canInvalidate++;
				if(i < animators.Count) 
					animators[i].StopAnimation();
				canInvalidate--;
			}
			internal bool IsAnimating(int i) {
				return (i < animators.Count) && animators[i].AnimationInProgress;
			}
			bool IsTouchScroll {
				get { return view.IsTouchScroll; }
			}
			Rectangle GetTouchScrollArea() {
				return view.GetTouchScrollArea();
			}
			Rectangle GetCardsArea() {
				return view.ViewInfo.ViewRects.CardsRect;
			}
			class TouchLoadingAnimator : LoadingAnimator {
				TouchLoadingAnimation owner;
				public TouchLoadingAnimator(TouchLoadingAnimation owner)
					: base(owner.gridControl, owner.Image) {
					this.owner = owner;
				}
				protected override void OnStop() {
					base.OnStop();
				}
				protected override void Invalidate() {
					if(owner.canInvalidate > 0) return;
					Rectangle bounds = InvalidateBounds.IsEmpty ? Bounds : InvalidateBounds;
					Rectangle cardsArea = owner.GetCardsArea();
					if(!cardsArea.IntersectsWith(bounds))
						return;
					if(owner.IsTouchScroll) {
						Rectangle scrollArea = owner.GetTouchScrollArea();
						if(scrollArea.IntersectsWith(bounds))
							return;
					}
					if(bounds.Height > 0 && bounds.Width > 0)
						Owner.Invalidate(bounds);
				}
			}
		}
		protected void DrawFieldHeaderButtons(LayoutViewDrawArgs e, List<ButtonInfo> sortFilterButtons) {
			if(sortFilterButtons.Count == 0) return;
			LayoutViewFieldHeaderObjectInfoArgs info = new LayoutViewFieldHeaderObjectInfoArgs(View);
			info.Buttons = sortFilterButtons.ToArray();
			ObjectPainter.DrawObject(e.Cache, ElementsPainter.SortFilterButtons, info);
		}
		protected void DrawSelectionRect(LayoutViewDrawArgs e) {
			if(View.CardSelectionRect.IsEmpty || !View.IsMultiSelect) return;
			Rectangle bounds = View.CardSelectionRect;
			int alpha = 32;
			Brush solidBrush = e.Cache.GetSolidBrush(Color.FromArgb(alpha, View.PaintAppearance.SelectionFrame.BackColor));
			e.Paint.DrawFocusRectangle(e.Graphics, bounds, View.PaintAppearance.SelectionFrame.ForeColor, View.PaintAppearance.SelectionFrame.BorderColor);
			bounds.Inflate(-1, -1);
			e.Cache.Paint.FillRectangle(e.Cache.Graphics, solidBrush, bounds);
		}
		protected void PrepareActiveFilteringButtons(List<ButtonInfo> sortFilterButtons) {
			LayoutViewCard card = View.FindCardByRow(View.FocusedRowHandle);
			if(card == null) return;
			if(View.CanFilterLayoutViewColumn(View.FilterPopup.Column)) {
				LayoutViewField field = View.FindCardFieldByColumn(card, View.FilterPopup.Column);
				LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
				PrepareFilterButton(sortFilterButtons, fieldInfo.FilterButton, ObjectState.Selected);
			}
		}
		protected void PrepareActiveSortingButtons(LayoutViewCard card, LayoutViewHitInfo hotInfo, List<ButtonInfo> sortFilterButtons) {
			foreach(GridColumnSortInfo sortInfo in View.SortInfo) {
				GridColumn column = sortInfo.Column;
				LayoutViewField field = View.FindCardFieldByColumn(card, column);
				if(!CanShowFieldHeaderButtons(field)) continue;
				if(IsHotColumn(column, hotInfo) && !IsInEditingField(column, card.RowHandle)) {
					LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
					PrepareSortButton(sortFilterButtons, column, fieldInfo.SortButton, ObjectState.Selected);
				}
			}
		}
		bool IsHotColumn(GridColumn column, LayoutViewHitInfo hotInfo) {
			return (hotInfo != null) && (hotInfo.Column != null) && (hotInfo.Column != column);
		}
		ImageAttributes imageAttributes = null;
		ImageAttributes ImageAttributes {
			get {
				if(imageAttributes == null) imageAttributes = new ImageAttributes();
				return imageAttributes;
			}
		}
		protected virtual ImageAttributes GetImageAttributes(CardImageInfo info, bool fadeAlpha, bool fadeColors) {
			float[][] array = new float[5][];
			float fade = info.ColorFade;
			float colorFactor = fadeColors ? fade : 1.0f;
			float colorFactor1 = fadeColors ? (1.0f - fade) : 0f;
			float alphaFactor = fadeAlpha ? info.Alpha : 1.0f;
			array[0] = new float[5] { colorFactor, 0.00f, 0.00f, 0.00f, 0 };
			array[1] = new float[5] { 0.00f, colorFactor, 0.00f, 0.00f, 0 };
			array[2] = new float[5] { 0.00f, 0.00f, colorFactor, 0.00f, 0 };
			array[3] = new float[5] { 0.00f, 0.00f, 0.00f, alphaFactor, 0 };
			array[4] = new float[5] { colorFactor1, colorFactor1, colorFactor1, 0, 0 };
			ColorMatrix matrix = new ColorMatrix(array);
			ImageAttributes.SetColorMatrix(matrix);
			return ImageAttributes;
		}
		protected void PaintAnimatedCards(ViewDrawArgs ee) {
			LayoutViewDrawArgs e = ee as LayoutViewDrawArgs;
			LayoutViewCarouselMode mode = e.ViewInfo.layoutManager as LayoutViewCarouselMode;
			if(mode == null) return;
			InterpolationMode oldSM = e.Graphics.InterpolationMode;
			e.Graphics.InterpolationMode = View.OptionsCarouselMode.InterpolationMode;
			int i;
			for(i = mode.CardImageInfos.Count - 1; i > mode.CenterCardIndex; i--)
				DrawImage(e.Graphics, i, mode);
			for(i = 0; i < mode.CenterCardIndex; i++)
				DrawImage(e.Graphics, i, mode);
			if(mode.IsAnimated)
				DrawImage(e.Graphics, mode.CenterCardIndex, mode);
			e.Graphics.InterpolationMode = oldSM;
		}
		protected virtual void DrawImage(Graphics g, int index, LayoutViewCarouselMode mode) {
			var info = mode.CardImageInfos[index];
			Rectangle imgBounds = info.ViewMode.GetBounds(info);
			Image img = info.GetImage(imgBounds);
			if(img == null || imgBounds.Width == 0 || imgBounds.Height == 0) return;
			g.DrawImage(img, imgBounds, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, GetImageAttributes(info, true, true));
		}
		protected virtual void DrawCard(LayoutViewDrawArgs e, LayoutViewCard card) {
			UpdateCardBorderInfoButtonState(e, card);
			DrawCardCore(e.Cache, card);
		}
		void DrawCardWithOffset(LayoutViewDrawArgs e, LayoutViewCard card, Point offset) {
			UpdateCardBorderInfoButtonState(e, card);
			Point savedOffset = card.ViewInfo.Offset;
			card.ViewInfo.Offset = offset;
			card.UpdateChildrenToBeRefactored();
			DrawCardCore(e.Cache, card);
			card.ViewInfo.Offset = savedOffset;
		}
		void UpdateCardBorderInfoButtonState(LayoutViewDrawArgs e, LayoutViewCard card) {
			if((card.ViewInfo.BorderInfo.ButtonState & ObjectState.Pressed) != 0) {
				card.ViewInfo.BorderInfo.ButtonState = ObjectState.Normal;
			}
			if(card.RowHandle == e.ViewInfo.SelectionInfo.PressedInfo.RowHandle) {
				card.ViewInfo.BorderInfo.ButtonState = CalcExpandButtonState(e);
			}
		}
		void DrawCardCore(GraphicsCache cache, LayoutViewCard card) {
			LayoutViewCardPainter cardPainter = (LayoutViewCardPainter)((ILayoutControl)View).PaintStyle.GetPainter(card);
			View.DrawCard = card;
			ObjectPainter.DrawObject(cache, cardPainter, card.ViewInfo);
			View.DrawCard = null;
		}
		bool IsHotCard(LayoutViewDrawArgs e, LayoutViewCard layoutCard) {
			return (e.ViewInfo.SelectionInfo.HotRowHandle == layoutCard.RowHandle);
		}
		bool CanShowFieldHeaderButtons(LayoutViewDrawArgs e, LayoutViewCard layoutCard) {
			return !View.IsDesignMode && !View.PanModeActive && !View.IsCustomizationMode;
		}
		bool CanShowFieldHeaderButtons(LayoutViewField field) {
			if(field == null) return false;
			LayoutGroup group = field.Parent;
			while(group != null) {
				if(!group.Expanded)
					return false;
				TabbedGroup tGroup = group.ParentTabbedGroup;
				if(tGroup != null && tGroup.SelectedTabPage != group)
					return false;
				group = group.Parent;
			}
			return true;
		}
		protected void DrawCardLines(LayoutViewDrawArgs e) {
			DrawCardLines(e, Point.Empty);
		}
		protected void DrawCardLines(LayoutViewDrawArgs e, Point offset) {
			List<Line> list = View.GetLines();
			int w = e.ViewInfo.SeparatorLineWidth;
			foreach(Line line in list) {
				using(AppearanceObject appearance = (AppearanceObject)View.PaintAppearance.SeparatorLine.Clone()) {
					LayoutViewCustomSeparatorStyleEventArgs ea = new LayoutViewCustomSeparatorStyleEventArgs(appearance, w, line.IsRowSeparator);
					View.RaiseCustomSeparatorStyle(ea);
					w = ea.Width;
					Rectangle lineRect = new Rectangle(
							offset.X + Math.Min(line.BeginPoint.X, line.EndPoint.X),
							offset.Y + Math.Min(line.BeginPoint.Y, line.EndPoint.Y),
							Math.Max(Math.Abs(line.EndPoint.X - line.BeginPoint.X), w),
							Math.Max(Math.Abs(line.EndPoint.Y - line.BeginPoint.Y), w)
						);
					if(lineRect.Width > lineRect.Height) lineRect.Location = new Point(lineRect.Location.X, lineRect.Location.Y - w / 2);
					else lineRect.Location = new Point(lineRect.Location.X - w / 2, lineRect.Location.Y);
					DrawIndentCore(e, new IndentInfo(null, lineRect, ea.Appearance), null);
				}
			}
		}
		protected virtual void DrawBackground(LayoutViewDrawArgs e) {
			e.ViewInfo.PaintAppearance.ViewBackground.DrawBackground(e.Cache, e.ViewInfo.ClientBounds);
		}
		protected virtual void DrawViewRects(LayoutViewDrawArgs e) {
			DrawBorder(e, e.ViewInfo.Bounds);
			DrawBackground(e);
			DrawViewCaption(e);
		}
		protected virtual void DrawFocus(LayoutViewDrawArgs e, LayoutViewCard layoutCard) {
			if((layoutCard.State & GridRowCellState.GridFocused) == 0) return;
			if(layoutCard.RowHandle == View.FocusedRowHandle && layoutCard.Expanded) {
				Rectangle focusRect = Rectangle.Empty;
				BaseLayoutItem focusedItem = View.FindItemByName(layoutCard, View.FocusedItemName);
				if(focusedItem is LayoutViewField) {
					LayoutViewField field = focusedItem as LayoutViewField;
					if(field != null && CanShowFocus(layoutCard, field)) {
						focusRect = field.ViewInfo.BoundsRelativeToControl;
					}
				}
				else if(focusedItem is TabbedControlGroup) {
					TabbedControlGroup tg = focusedItem as TabbedControlGroup;
					DevExpress.XtraTab.ViewInfo.BaseTabHeaderViewInfo hInfo = tg.ViewInfo.BorderInfo.Tab.Handler.ViewInfo.HeaderInfo;
					if(hInfo.Rows.LastRow.SelectedPage != null) {
						Rectangle headerRect = hInfo.Bounds;
						Rectangle buttonsRect = hInfo.ButtonsBounds;
						focusRect = hInfo.Rows.LastRow.SelectedPage.Focus;
						if(!headerRect.Contains(focusRect) || focusRect.IntersectsWith(buttonsRect)) focusRect = Rectangle.Empty;
						else focusRect.Inflate(2, 2);
					}
				}
				if(!focusRect.IsEmpty) {
					e.Paint.DrawFocusRectangle(e.Graphics, focusRect, Color.Black, Color.White);
				}
			}
		}
		protected virtual bool CanShowFocus(LayoutViewCard layoutCard, LayoutViewField focusedField) {
			return focusedField.Visible && View.OptionsView.FocusRectStyle != FocusRectStyle.None;
		}
		SkinElement headerBackground = null;
		SkinElement groupPanel = null;
		SkinElement headerSeparator = null;
		protected SkinElement HeaderSeparatorSkinElement {
			get {
				if(headerSeparator == null) headerSeparator = BarSkins.GetSkin(View.LookAndFeel.ActiveLookAndFeel)[BarSkins.SkinBarSeparator];
				return headerSeparator;
			}
		}
		protected SkinElement HeaderBackgroundSkinElement {
			get {
				if(headerBackground == null) headerBackground = GridSkins.GetSkin(View.LookAndFeel.ActiveLookAndFeel)[GridSkins.SkinHeader];
				return headerBackground;
			}
		}
		protected SkinElement GroupPanelSkinElement {
			get {
				if(groupPanel == null) groupPanel = GridSkins.GetSkin(View.LookAndFeel.ActiveLookAndFeel)[GridSkins.SkinCard];
				return groupPanel;
			}
		}
		protected virtual void DrawHeader(LayoutViewDrawArgs e) {
			if(!e.ViewInfo.ViewRects.HeaderRect.IsEmpty) {
				LayoutViewHeaderObjectInfoArgs info = new LayoutViewHeaderObjectInfoArgs(View);
				info.Bounds = e.ViewInfo.ViewRects.HeaderRect;
				info.SetAppearance(View.PaintAppearance.HeaderPanel);
				List<ButtonInfo> buttons = new List<ButtonInfo>();
				PrepareSingleModeButton(e, buttons);
				PrepareRowModeButton(e, buttons);
				PrepareColumnModeButton(e, buttons);
				PrepareMultiRowModeButton(e, buttons);
				PrepareMultiColumnModeButton(e, buttons);
				PrepareCarouselModeButton(e, buttons);
				if(View.OptionsBehavior.AllowPanCards) PreparePanButton(e, buttons);
				if(View.OptionsBehavior.AllowRuntimeCustomization) PrepareCustomizeButton(e, buttons);
				if(!View.fInternalLockActions) info.Buttons = buttons.ToArray();
				ObjectPainter.DrawObject(e.Cache, ElementsPainter.HeaderPanel, info);
				DrawHeaderSeparator(e);
				if(View.IsDetailView) DrawCloseZoomButton(e);
			}
		}
		protected virtual void DrawHeaderSeparator(LayoutViewDrawArgs e) {
			if(e.ViewInfo.ViewRects.Separator.IsEmpty) return;
			Rectangle rect = e.ViewInfo.ViewRects.Separator;
			int iSkinWidth = HeaderSeparatorSkinElement.Size.MinSize.Width;
			if(iSkinWidth > rect.Width) rect.Inflate(iSkinWidth - rect.Width, 0);
			SkinElementInfo info = new SkinElementInfo(HeaderSeparatorSkinElement, rect);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		protected virtual void PrepareSingleModeButton(LayoutViewDrawArgs e, List<ButtonInfo> buttons) {
			if(!e.ViewInfo.ViewRects.SingleModeButton.IsEmpty) {
				ButtonInfo buttonInfo = PrepareViewModeButtonInfo(LayoutViewMode.SingleRecord,
							e.ViewInfo.ViewRects.SingleModeButton, View.ViewHeaderIcons.Images[0],
							CalcSingleModeButtonState(e), View.OptionsHeaderPanel.EnableSingleModeButton
						);
				buttons.Add(buttonInfo);
			}
		}
		protected virtual void PrepareRowModeButton(LayoutViewDrawArgs e, List<ButtonInfo> buttons) {
			if(!e.ViewInfo.ViewRects.RowModeButton.IsEmpty) {
				ButtonInfo buttonInfo = PrepareViewModeButtonInfo(LayoutViewMode.Row,
							e.ViewInfo.ViewRects.RowModeButton, View.ViewHeaderIcons.Images[1],
							CalcRowModeButtonState(e), View.OptionsHeaderPanel.EnableRowModeButton
						);
				buttons.Add(buttonInfo);
			}
		}
		protected virtual void PrepareColumnModeButton(LayoutViewDrawArgs e, List<ButtonInfo> buttons) {
			if(!e.ViewInfo.ViewRects.ColumnModeButton.IsEmpty) {
				ButtonInfo buttonInfo = PrepareViewModeButtonInfo(LayoutViewMode.Column,
							e.ViewInfo.ViewRects.ColumnModeButton, View.ViewHeaderIcons.Images[2],
							CalcColumnModeButtonState(e), View.OptionsHeaderPanel.EnableColumnModeButton
						);
				buttons.Add(buttonInfo);
			}
		}
		protected virtual void PrepareMultiRowModeButton(LayoutViewDrawArgs e, List<ButtonInfo> buttons) {
			if(!e.ViewInfo.ViewRects.MultiRowModeButton.IsEmpty) {
				ButtonInfo buttonInfo = PrepareViewModeButtonInfo(LayoutViewMode.MultiRow,
							e.ViewInfo.ViewRects.MultiRowModeButton, View.ViewHeaderIcons.Images[3],
							CalcMultiRowModeButtonState(e), View.OptionsHeaderPanel.EnableMultiRowModeButton
						);
				buttons.Add(buttonInfo);
			}
		}
		protected virtual void PrepareMultiColumnModeButton(LayoutViewDrawArgs e, List<ButtonInfo> buttons) {
			if(!e.ViewInfo.ViewRects.MultiColumnModeButton.IsEmpty) {
				ButtonInfo buttonInfo = PrepareViewModeButtonInfo(LayoutViewMode.MultiColumn,
							e.ViewInfo.ViewRects.MultiColumnModeButton, View.ViewHeaderIcons.Images[4],
							CalcMultiColumnModeButtonState(e), View.OptionsHeaderPanel.EnableMultiColumnModeButton
						);
				buttons.Add(buttonInfo);
			}
		}
		protected virtual void PrepareCarouselModeButton(LayoutViewDrawArgs e, List<ButtonInfo> buttons) {
			if(!e.ViewInfo.ViewRects.CarouselModeButton.IsEmpty) {
				ButtonInfo buttonInfo = PrepareViewModeButtonInfo(LayoutViewMode.Carousel,
							e.ViewInfo.ViewRects.CarouselModeButton, View.ViewHeaderIcons.Images[6],
							CalcCarouselModeButtonState(e), View.OptionsHeaderPanel.EnableCarouselModeButton
						);
				buttons.Add(buttonInfo);
			}
		}
		ButtonInfo PrepareViewModeButtonInfo(LayoutViewMode mode, Rectangle bounds, Image image, ObjectState state, bool enabled) {
			ButtonInfo buttonInfo = new ButtonInfo(bounds, image);
			if(enabled) {
				if(View.OptionsView.ViewMode == mode) {
					if((state & ObjectState.Pressed) == 0) state |= ObjectState.Pressed;
				}
			}
			else state = ObjectState.Disabled;
			buttonInfo.State = state;
			return buttonInfo;
		}
		ButtonInfo PrepareViewModeButtonInfo(bool isMode, Rectangle bounds, Image image, ObjectState state, bool enabled) {
			ButtonInfo buttonInfo = new ButtonInfo(bounds, image);
			if(enabled) {
				if(isMode) {
					if((state & ObjectState.Pressed) == 0) state |= ObjectState.Pressed;
				}
			}
			else state = ObjectState.Disabled;
			buttonInfo.State = state;
			return buttonInfo;
		}
		protected virtual void PreparePanButton(LayoutViewDrawArgs e, List<ButtonInfo> buttons) {
			if(!e.ViewInfo.ViewRects.PanButton.IsEmpty) {
				ButtonInfo buttonInfo = PrepareViewModeButtonInfo(View.PanModeActive,
							e.ViewInfo.ViewRects.PanButton, View.ViewHeaderIcons.Images[5],
							CalcPanButtonState(e), View.OptionsHeaderPanel.EnablePanButton && View.ViewInfo.CanCardAreaPan
						);
				buttons.Add(buttonInfo);
			}
		}
		protected virtual void PrepareCustomizeButton(LayoutViewDrawArgs e, List<ButtonInfo> buttons) {
			if(!e.ViewInfo.ViewRects.CustomizeButton.IsEmpty) {
				ButtonInfo buttonInfo = PrepareViewModeButtonInfo(View.IsCustomizationMode,
							e.ViewInfo.ViewRects.CustomizeButton, View.ViewHeaderIcons.Images[7],
							CalcCustomizeButtonState(e), View.OptionsHeaderPanel.EnableCustomizeButton && View.ViewInfo.CanCustomize
						);
				buttons.Add(buttonInfo);
			}
		}
		protected virtual void DrawCloseZoomButton(LayoutViewDrawArgs e) {
			if(e.ViewInfo.ViewRects.CloseZoomButton.IsEmpty) return;
			bool isZoomed = View.IsZoomedView;
			EditorButtonObjectInfoArgs buttonInfo;
			if(isZoomed) {
				buttonInfo = new EditorButtonObjectInfoArgs(new EditorButton(ButtonPredefines.Close), null);
			}
			else {
				buttonInfo = new EditorButtonObjectInfoArgs(new EditorButton(ButtonPredefines.Glyph), null);
				buttonInfo.Button.Image = Grid.Drawing.GridPainter.Indicator.Images[Grid.Drawing.GridPainter.IndicatorZoom];
			}
			buttonInfo.Bounds = e.ViewInfo.ViewRects.CloseZoomButton;
			buttonInfo.State = CalcCloseZoomButtonState(e);
			if(isZoomed || buttonInfo.State != ObjectState.Normal) {
				ObjectPainter.DrawObject(e.Cache, ElementsPainter.EditorButton, buttonInfo);
			}
			else {
				Size imgOffset = buttonInfo.Bounds.Size - buttonInfo.Button.Image.Size;
				imgOffset = new Size(imgOffset.Width / 2, imgOffset.Height / 2);
				e.Paint.DrawImage(e.Graphics, buttonInfo.Button.Image, buttonInfo.Bounds.Location + imgOffset);
			}
		}
		bool IsInEditingField(GridColumn column, int rowHandle) {
			return (View.ActiveEditor != null) && (column == View.FocusedColumn) && (rowHandle == View.FocusedRowHandle);
		}
		protected virtual void PrepareHotFieldHeaderButtons(LayoutViewDrawArgs e, List<ButtonInfo> sortFilterButtons, LayoutViewCard layoutCard) {
			ObjectState fieldHeaderState = CalcFieldHeaderState(e);
			if((fieldHeaderState & ObjectState.Hot) != 0) {
				LayoutViewHitInfo hotInfo = e.ViewInfo.SelectionInfo.HotTrackedInfo;
				if(hotInfo != null && hotInfo.HitField != null) {
					if(IsInEditingField(hotInfo.Column, hotInfo.RowHandle)) return;
					if(!CanShowFieldHeaderButtons(hotInfo.HitField)) return;
					LayoutViewFieldInfo fieldInfo = hotInfo.HitField.ViewInfo as LayoutViewFieldInfo;
					if(View.CanSortLayoutViewColumn(hotInfo.Column)) {
						ObjectState sortBtnState = CalcFieldSortButtonState(e);
						PrepareSortButton(sortFilterButtons, hotInfo.Column, fieldInfo.SortButton, sortBtnState);
					}
					if(View.CanFilterLayoutViewColumn(hotInfo.Column)) {
						ObjectState filterBtnState = CalcFieldFilterButtonState(e);
						PrepareFilterButton(sortFilterButtons, fieldInfo.FilterButton, filterBtnState);
					}
				}
			}
		}
		protected virtual void PrepareSortButton(List<ButtonInfo> sortFilterButtons, GridColumn column, Rectangle btnBounds, ObjectState state) {
			if(btnBounds.IsEmpty) return;
			int glyphStartIndex = 4;
			Image btnImg = View.ViewPopupIcons.Images[1];
			if(column != null) {
				switch(column.SortOrder) {
					case DevExpress.Data.ColumnSortOrder.None:
						btnImg = View.ViewPopupIcons.Images[3];
						glyphStartIndex = 12;
						break;
					case DevExpress.Data.ColumnSortOrder.Descending:
						btnImg = View.ViewPopupIcons.Images[0];
						glyphStartIndex = 8;
						break;
				}
			}
			sortFilterButtons.Add(new ButtonInfo(btnBounds, btnImg, state, glyphStartIndex + ButtonInfo.CalcSortFilterButtonImageIndex(state)));
		}
		protected virtual void PrepareFilterButton(List<ButtonInfo> sortFilterButtons, Rectangle btnBounds, ObjectState state) {
			if(btnBounds.IsEmpty) return;
			sortFilterButtons.Add(
					new ButtonInfo(btnBounds, View.ViewPopupIcons.Images[2], state, ButtonInfo.CalcSortFilterButtonImageIndex(state))
				);
		}
		protected virtual ObjectState CalcExpandButtonState(ViewDrawArgs e) {
			ObjectState state = ObjectState.Normal;
			LayoutViewHitInfo hi = e.ViewInfo.SelectionInfo.PressedInfo as LayoutViewHitInfo;
			if(hi != null && hi.InCardExpandButton) {
				state |= ObjectState.Pressed;
			}
			return state;
		}
		protected override ObjectState CalcFilterTextState(ViewDrawArgs e) {
			if(View.MRUFilterPopup != null) return ObjectState.Pressed;
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.FilterPanelText, LayoutViewState.FilterPanelTextPressed);
		}
		protected override ObjectState CalcFilterCloseButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.FilterPanelCloseButton, LayoutViewState.FilterPanelCloseButtonPressed);
		}
		protected override ObjectState CalcFilterActiveButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.FilterPanelActiveButton, LayoutViewState.FilterPanelActiveButtonPressed);
		}
		protected override ObjectState CalcFilterMRUButtonState(ViewDrawArgs e) {
			if(View.MRUFilterPopup != null) return ObjectState.Pressed;
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.FilterPanelMRUButton, LayoutViewState.FilterPanelMRUButtonPressed);
		}
		protected override ObjectState CalcFilterCustomizeButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.FilterPanelCustomizeButton, LayoutViewState.FilterPanelCustomizeButtonPressed);
		}
		protected virtual ObjectState CalcFieldHeaderState(ViewDrawArgs e) {
			ObjectState state = ObjectState.Normal;
			LayoutViewHitInfo hi = e.ViewInfo.SelectionInfo.HotTrackedInfo as LayoutViewHitInfo;
			if(hi != null && hi.InField) {
				state |= ObjectState.Hot;
			}
			return state;
		}
		protected virtual ObjectState CalcFieldSortButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.FieldSortButton, LayoutViewState.FieldPopupActionSortPressed);
		}
		protected virtual ObjectState CalcFieldFilterButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.FieldFilterButton, LayoutViewState.FieldPopupActionFilterPressed);
		}
		protected virtual ObjectState CalcSingleModeButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.SingleModeButton, LayoutViewState.SingleCardModeButtonPressed);
		}
		protected virtual ObjectState CalcRowModeButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.RowModeButton, LayoutViewState.RowModeButtonPressed);
		}
		protected virtual ObjectState CalcColumnModeButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.ColumnModeButton, LayoutViewState.ColumnModeButtonPressed);
		}
		protected virtual ObjectState CalcMultiRowModeButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.MultiRowModeButton, LayoutViewState.MultiRowModeButtonPressed);
		}
		protected virtual ObjectState CalcMultiColumnModeButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.MultiColumnModeButton, LayoutViewState.MultiColumnModeButtonPressed);
		}
		protected virtual ObjectState CalcCarouselModeButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.CarouselModeButton, LayoutViewState.CarouselModeButtonPressed);
		}
		protected virtual ObjectState CalcPanButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.PanButton, LayoutViewState.PanButtonPressed);
		}
		protected virtual ObjectState CalcCustomizeButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.CustomizeButton, LayoutViewState.CustomizeButtonPressed);
		}
		protected virtual ObjectState CalcCloseZoomButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as LayoutViewDrawArgs, LayoutViewHitTest.CloseZoomButton, LayoutViewState.CloseZoomButtonPressed);
		}
		ObjectState CalcObjectState(LayoutViewDrawArgs e, LayoutViewHitTest hitTest, LayoutViewState cardState) {
			ObjectState state = ObjectState.Normal;
			if(e.ViewInfo.SelectionInfo.HotTrackedInfo.HitTest == hitTest) state |= ObjectState.Hot;
			if(View.State == cardState) state |= ObjectState.Pressed;
			return state;
		}
	}
	public class ButtonInfo {
		Rectangle boundsCore;
		Image imageCore;
		ObjectState stateCore;
		int glyphIndex;
		public ButtonInfo(Rectangle bounds, Image image)
			: this(bounds, image, ObjectState.Normal, -1) {
		}
		public ButtonInfo(Rectangle bounds, Image image, ObjectState state)
			: this(bounds, image, ObjectState.Normal, -1) {
		}
		public ButtonInfo(Rectangle bounds, Image image, ObjectState state, int glyph) {
			boundsCore = bounds;
			imageCore = image;
			stateCore = state;
			glyphIndex = glyph;
		}
		public Rectangle Bounds {
			get { return boundsCore; }
		}
		public Image Image {
			get { return imageCore; }
		}
		public ObjectState State {
			get { return stateCore; }
			set { stateCore = value; }
		}
		public int GlyphIndex {
			get { return glyphIndex; }
			set { glyphIndex = value; }
		}
		public Size GetImageSize() {
			return Image != null ? Image.Size : Size.Empty;
		}
		public Rectangle GetImageBounds() {
			Rectangle result = Rectangle.Empty;
			Size img = GetImageSize();
			if(!Bounds.IsEmpty && !img.IsEmpty) {
				Size offset = Bounds.Size - img;
				result = new Rectangle(
						Bounds.Left + offset.Width / 2, Bounds.Top + offset.Height / 2,
						img.Width, img.Height
					);
			}
			return result;
		}
		public static int CalcSortFilterButtonImageIndex(ObjectState state) {
			int index = 0;
			if(state == ObjectState.Hot) index = 1;
			if(state == ObjectState.Pressed) index = 2;
			if(state == ObjectState.Selected) index = 3;
			return index;
		}
	}
	public class LayoutViewHeaderObjectInfoArgs : StyleObjectInfoArgs {
		LayoutView viewCore;
		ButtonInfo[] buttons;
		public LayoutViewHeaderObjectInfoArgs(LayoutView view) {
			viewCore = view;
			buttons = Empty;
		}
		public LayoutView View { get { return viewCore; } }
		public ButtonInfo[] Buttons {
			get { return buttons; }
			set {
				if(value == null) value = Empty;
				buttons = value;
			}
		}
		static ButtonInfo[] Empty = new ButtonInfo[0];
	}
	public class LayoutViewHeaderObjectPainter : StyleObjectPainter {
		BorderPainter borderPainterCore;
		ButtonObjectPainter buttonPainterCore;
		public LayoutViewHeaderObjectPainter(BorderPainter border, ButtonObjectPainter button) {
			this.borderPainterCore = border;
			this.buttonPainterCore = button;
		}
		public BorderPainter BorderPainter {
			get { return borderPainterCore; }
		}
		public ButtonObjectPainter ButtonPainter {
			get { return buttonPainterCore; }
		}
		public override void DrawObject(ObjectInfoArgs e) {
			LayoutViewHeaderObjectInfoArgs infoArgs = e as LayoutViewHeaderObjectInfoArgs;
			DrawHeader(infoArgs);
			DrawButtons(infoArgs);
		}
		protected virtual void DrawHeader(LayoutViewHeaderObjectInfoArgs infoArgs) {
			LayoutViewInfo viewInfo = infoArgs.View.ViewInfo;
			Rectangle content = DrawElementBorder(infoArgs.Cache, infoArgs.Bounds, ObjectState.Normal);
			DrawHeaderBackground(infoArgs, content);
		}
		protected void DrawButtons(LayoutViewHeaderObjectInfoArgs infoArgs) {
			for(int i = 0; i < infoArgs.Buttons.Length; i++) {
				DrawButton(infoArgs, infoArgs.Buttons[i]);
			}
		}
		protected Rectangle DrawElementBorder(GraphicsCache cache, Rectangle elementBounds, ObjectState state) {
			BorderObjectInfoArgs borderInfo = new BorderObjectInfoArgs(cache, elementBounds, null);
			borderInfo.State = state;
			BorderPainter.DrawObject(borderInfo);
			return BorderPainter.GetObjectClientRectangle(borderInfo);
		}
		protected virtual void DrawHeaderBackground(LayoutViewHeaderObjectInfoArgs info, Rectangle bounds) {
			info.Appearance.DrawBackground(info.Graphics, info.Cache, bounds);
		}
		protected virtual void DrawButton(LayoutViewHeaderObjectInfoArgs infoArgs, ButtonInfo info) {
			DrawButtonBounds(infoArgs, info);
			DrawButtonImage(infoArgs.Graphics, info);
		}
		protected virtual void DrawButtonBounds(LayoutViewHeaderObjectInfoArgs infoArgs, ButtonInfo info) {
			if(info.State == ObjectState.Normal || info.State == ObjectState.Disabled) return;
			ButtonPainter.DrawObject(new StyleObjectInfoArgs(infoArgs.Cache, info.Bounds, null, info.State));
		}
		protected void DrawButtonImage(Graphics graphics, ButtonInfo info) {
			if(graphics == null || info.Image == null) return;
			if(info.State != ObjectState.Disabled) {
				graphics.DrawImageUnscaled(info.Image, info.GetImageBounds());
			}
			else {
				Size sz = info.GetImageSize();
				graphics.DrawImage(info.Image, info.GetImageBounds(),
						0, 0, sz.Width, sz.Height, GraphicsUnit.Pixel, DisabledAttributes
					);
			}
		}
		static ImageAttributes disabledAttributes;
		public static ImageAttributes DisabledAttributes {
			get {
				if(disabledAttributes == null) InitDisabledAttributes();
				return disabledAttributes;
			}
		}
		public static void InitDisabledAttributes() {
			ColorMatrix cm = new ColorMatrix();
			cm.Matrix00 = 1.0f;
			cm.Matrix11 = 1.0f;
			cm.Matrix22 = 1.0f;
			cm.Matrix33 = 0.2f;	 
			cm.Matrix44 = 1.0f;
			disabledAttributes = new ImageAttributes();
			disabledAttributes.SetColorMatrix(cm);
		}
		public int GetHeaderHeight() {
			return GetButtonSize().Height + GetPanelContentMargins().Vertical;
		}
		public virtual Padding GetPanelContentMargins() {
			return DefaultPanelContentMargins;
		}
		public virtual Size GetButtonSize() {
			return DefaultButtonSize;
		}
		public static Padding DefaultPanelContentMargins = new Padding(2);
		public static Size DefaultButtonSize = new Size(24, 22);
		public static int DefaultHeaderHeight =
			DefaultButtonSize.Height + DefaultPanelContentMargins.Vertical;
	}
	public class SkinLayoutViewHeaderObjectPainter : LayoutViewHeaderObjectPainter {
		ISkinProvider skinProviderCore;
		public SkinLayoutViewHeaderObjectPainter(ISkinProvider provider)
			: base(null, null) {
			skinProviderCore = provider;
		}
		public ISkinProvider SkinProvider {
			get { return skinProviderCore; }
		}
		protected override void DrawHeader(LayoutViewHeaderObjectInfoArgs infoArgs) {
			SkinElementInfo skinInfo = new SkinElementInfo(GetHeaderSkinElement(), infoArgs.Bounds);
			ObjectPainter.DrawObject(infoArgs.Cache, SkinElementPainter.Default, skinInfo);
		}
		protected override void DrawButtonBounds(LayoutViewHeaderObjectInfoArgs infoArgs, ButtonInfo info) {
			if(((info.State & ObjectState.Hot) == 0) && ((info.State & ObjectState.Pressed) == 0)) return;
			SkinElementInfo skinInfo = new SkinElementInfo(GetHeaderButtonSkinElement(), info.Bounds);
			if((info.State & ObjectState.Pressed) != 0) skinInfo.ImageIndex = 2;
			if((info.State & ObjectState.Hot) != 0 && (info.State & ObjectState.Pressed) != 0) skinInfo.ImageIndex = 3;
			ObjectPainter.DrawObject(infoArgs.Cache, SkinElementPainter.Default, skinInfo);
		}
		SkinElement GetHeaderButtonSkinElement() {
			return GetSkin(SkinProvider)[BarSkins.SkinLinkSelected];
		}
		SkinElement GetHeaderSkinElement() {
			return GetSkin(SkinProvider)[BarSkins.SkinBar];
		}
		Skin GetSkin(ISkinProvider provider) { return BarSkins.GetSkin(provider); }
		public override Padding GetPanelContentMargins() {
			SkinPaddingEdges p = GetHeaderSkinElement().ContentMargins;
			return new Padding(p.Left, p.Top, p.Right, p.Bottom);
		}
	}
	public class LayoutViewFieldHeaderObjectInfoArgs : LayoutViewHeaderObjectInfoArgs {
		public LayoutViewFieldHeaderObjectInfoArgs(LayoutView view)
			: base(view) {
		}
	}
	public class LayoutViewFieldHeaderObjectPainter : LayoutViewHeaderObjectPainter {
		public LayoutViewFieldHeaderObjectPainter(ButtonObjectPainter button)
			: base(null, button) {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DrawButtons(e as LayoutViewFieldHeaderObjectInfoArgs);
		}
		public virtual Size GetFieldButtonSize() {
			return new Size(17, 17);
		}
	}
	public class SkinLayoutViewFieldHeaderObjectPainter : LayoutViewFieldHeaderObjectPainter {
		ISkinProvider skinProviderCore;
		public SkinLayoutViewFieldHeaderObjectPainter(ISkinProvider provider)
			: base(null) {
			skinProviderCore = provider;
		}
		public ISkinProvider SkinProvider {
			get { return skinProviderCore; }
		}
		protected override void DrawButton(LayoutViewHeaderObjectInfoArgs infoArgs, ButtonInfo info) {
			SkinElementInfo skinInfo = new SkinElementInfo(GetButtonSkinElement(), info.Bounds);
			skinInfo.ImageIndex = ButtonInfo.CalcSortFilterButtonImageIndex(info.State);
			if(info.GlyphIndex != -1) skinInfo.GlyphIndex = info.GlyphIndex;
			ObjectPainter.DrawObject(infoArgs.Cache, SkinElementPainter.Default, skinInfo);
		}
		public override Size GetFieldButtonSize() {
			SkinElement element = GetButtonSkinElement();
			return (element == null) ? base.GetButtonSize() :
				SkinElementPainter.Default.CalcObjectMinBounds(new SkinElementInfo(element)).Size;
		}
		SkinElement GetButtonSkinElement() {
			return GetSkin(SkinProvider)[GridSkins.SkinFieldHeaderButton];
		}
		Skin GetSkin(ISkinProvider provider) { return GridSkins.GetSkin(provider); }
	}
	public class LayoutViewElementsPainter {
		ObjectPainter filterPanel;
		ObjectPainter editorButton;
		LayoutViewHeaderObjectPainter headerPanel;
		LayoutViewFieldHeaderObjectPainter fieldHeader;
		ObjectPainter viewCaption;
		protected BaseView viewCore;
		public LayoutViewElementsPainter(BaseView view) {
			this.viewCore = view;
			this.viewCaption = CreateViewCaptionPainter();
			this.filterPanel = CreateFilterPanelPainter();
			this.editorButton = CreateEditorButtonPainter();
			this.headerPanel = CreateHeaderPanelPainter();
			this.fieldHeader = CreateFieldHeaderPanelPainter();
		}
		public UserLookAndFeel ElementsLookAndFeel {
			get { return View.ElementsLookAndFeel; }
		}
		public virtual BaseView View {
			get { return viewCore; }
		}
		protected virtual ObjectPainter CreateViewCaptionPainter() {
			return new GridViewCaptionPainter(View);
		}
		protected virtual ObjectPainter CreateFilterPanelPainter() {
			return new GridFilterPanelPainter(
					EditorButtonHelper.GetPainter(BorderStyles.Default, ElementsLookAndFeel),
					CheckPainterHelper.GetPainter(ElementsLookAndFeel)
				);
		}
		protected virtual ObjectPainter CreateEditorButtonPainter() {
			return EditorButtonHelper.GetPainter(BorderStyles.Default, ElementsLookAndFeel);
		}
		protected virtual LayoutViewHeaderObjectPainter CreateHeaderPanelPainter() {
			return new LayoutViewHeaderObjectPainter(new FlatBorderPainter(), new FlatButtonObjectPainter());
		}
		protected virtual LayoutViewFieldHeaderObjectPainter CreateFieldHeaderPanelPainter() {
			return new LayoutViewFieldHeaderObjectPainter(new FlatButtonObjectPainter());
		}
		public ObjectPainter ViewCaption { get { return viewCaption; } }
		public ObjectPainter FilterPanel { get { return filterPanel; } }
		public ObjectPainter EditorButton { get { return editorButton; } }
		public LayoutViewHeaderObjectPainter HeaderPanel { get { return headerPanel; } }
		public LayoutViewFieldHeaderObjectPainter SortFilterButtons { get { return fieldHeader; } }
	}
	public class LayoutViewWindowsXPElementsPainter : LayoutViewElementsPainter {
		public LayoutViewWindowsXPElementsPainter(BaseView view) : base(view) { }
	}
	public class LayoutViewMixedXPElementsPainter : LayoutViewElementsPainter {
		public LayoutViewMixedXPElementsPainter(BaseView view) : base(view) { }
	}
	public class LayoutViewUltraFlatElementsPainter : LayoutViewElementsPainter {
		public LayoutViewUltraFlatElementsPainter(BaseView view) : base(view) { }
		protected override LayoutViewHeaderObjectPainter CreateHeaderPanelPainter() {
			return new LayoutViewHeaderObjectPainter(new UltraFlatBorderPainter(), new UltraFlatButtonObjectPainter());
		}
		protected override LayoutViewFieldHeaderObjectPainter CreateFieldHeaderPanelPainter() {
			return new LayoutViewFieldHeaderObjectPainter(new UltraFlatButtonObjectPainter());
		}
	}
	public class LayoutViewOffice2003ElementsPainter : LayoutViewElementsPainter {
		public LayoutViewOffice2003ElementsPainter(BaseView view) : base(view) { }
		protected override LayoutViewHeaderObjectPainter CreateHeaderPanelPainter() {
			return new LayoutViewHeaderObjectPainter(new Office2003BorderPainter(), new Office2003ButtonInBarsObjectPainter());
		}
		protected override LayoutViewFieldHeaderObjectPainter CreateFieldHeaderPanelPainter() {
			return new LayoutViewFieldHeaderObjectPainter(new Office2003ButtonInBarsObjectPainter());
		}
	}
	public class LayoutViewStyle3DElementsPainter : LayoutViewElementsPainter {
		public LayoutViewStyle3DElementsPainter(BaseView view) : base(view) { }
		protected override LayoutViewHeaderObjectPainter CreateHeaderPanelPainter() {
			return new LayoutViewHeaderObjectPainter(new Border3DRaisedPainter(), new Style3DButtonObjectPainter());
		}
		protected override LayoutViewFieldHeaderObjectPainter CreateFieldHeaderPanelPainter() {
			return new LayoutViewFieldHeaderObjectPainter(new Style3DButtonObjectPainter());
		}
	}
	public class LayoutViewSkinElementsPainter : LayoutViewElementsPainter {
		public LayoutViewSkinElementsPainter(BaseView view) : base(view) { }
		protected override ObjectPainter CreateFilterPanelPainter() {
			return new SkinGridFilterPanelPainter(View);
		}
		protected override ObjectPainter CreateViewCaptionPainter() {
			return new SkinGridViewCaptionPainter(View);
		}
		protected override ObjectPainter CreateEditorButtonPainter() {
			return new SkinEditorButtonPainter(View);
		}
		protected override LayoutViewHeaderObjectPainter CreateHeaderPanelPainter() {
			return new SkinLayoutViewHeaderObjectPainter(View);
		}
		protected override LayoutViewFieldHeaderObjectPainter CreateFieldHeaderPanelPainter() {
			return new SkinLayoutViewFieldHeaderObjectPainter(View);
		}
	}
}
