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
using DevExpress.XtraGrid.Drawing;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Design;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors.Helpers;
namespace DevExpress.XtraGrid.Views.Card.Drawing {
	public class CardViewDrawArgs : ViewDrawArgs {
		public CardViewDrawArgs(GraphicsCache graphicsCache, CardViewInfo viewInfo, Rectangle bounds) : base(graphicsCache, viewInfo, bounds) {
		}
		public virtual new CardViewInfo ViewInfo  { get { return base.ViewInfo as CardViewInfo; } }
	}
	public class CardPainter : ColumnViewPainter {
		CardElementsPainter elementsPainter;
		public CardPainter(CardView view) : base(view) {
			this.elementsPainter = (view.PaintStyle as CardPaintStyle).CreateElementsPainter(view);
		}
		public CardElementsPainter ElementsPainter  { get { return elementsPainter; } }
		public new CardView View  { get { return base.View as CardView; } }
		protected virtual void DoAfterDraw(CardViewDrawArgs e) {
			e.ViewInfo.Graphics = null;
		}
		protected virtual void DoBeforeDraw(CardViewDrawArgs e) {
			e.ViewInfo.Graphics = e.Graphics;
		}
		protected virtual void DrawFieldCaption(CardViewDrawArgs e, CardFieldInfo fi, bool focused) {
			if(fi.CaptionBounds.IsEmpty) return;
			RowCellCustomDrawEventArgs cs = new RowCellCustomDrawEventArgs(e.Cache, fi.CaptionBounds, 
				fi.PaintAppearanceCaption, fi.Card.RowHandle, fi.Column, fi.FieldCaption, fi.FieldCaption);
			cs.SetDefaultDraw(() => {
				DrawFieldCaptionCore(e, fi, cs);
			});
			e.ViewInfo.View.RaiseCustomDrawCardFieldCaption(cs);
			cs.DefaultDraw();
		}
		void DrawFieldCaptionCore(CardViewDrawArgs e, CardFieldInfo fi, RowCellCustomDrawEventArgs cs) {
			Rectangle r = Rectangle.Inflate(fi.CaptionBounds, 0, 1);
			r.X = fi.Bounds.X;
			r.Width = fi.CaptionBounds.Right - r.X;
			cs.Appearance.DrawBackground(e.Cache, r);
			Rectangle fr = fi.CaptionBounds;
			fr.Inflate(-1, 0);
			cs.Appearance.DrawString(e.Cache, cs.DisplayText, fr);
			if(cs.Column != null && cs.Column.GetSelectedInDesigner())
				BaseDesignTimeManager.DrawDesignSelection(e.Cache, fi.CaptionBounds, cs.Appearance.BackColor);
		}
		protected virtual void DrawFieldValue(CardViewDrawArgs e, CardFieldInfo fi) {
			if(e.ViewInfo.PaintAnimatedItems && !(fi.EditViewInfo is IAnimatedItem)) return;
			if(e.ViewInfo.PaintAnimatedItems) {
				e.ViewInfo.PaintAppearance.EmptySpace.DrawBackground(e.Cache, (fi.EditViewInfo as IAnimatedItem).AnimationBounds);
			}
			RowCellCustomDrawEventArgs cs = new RowCellCustomDrawEventArgs(e.Cache, fi);
			cs.SetDefaultDraw(() => {
				DrawFieldValueCore(e, fi, cs);
			});
			View.RaiseCustomDrawCardFieldValue(cs);
			cs.DefaultDraw();
		}
		private void DrawFieldValueCore(CardViewDrawArgs e, CardFieldInfo fi, RowCellCustomDrawEventArgs cs) {
			if(cs.changed) fi.EditViewInfo.SetDisplayText(cs.DisplayText, cs.CellValue);
			Rectangle r = Rectangle.Inflate(fi.ValueBounds, 0, 1);
			r.Width = fi.Bounds.Right - r.X;
			cs.Appearance.DrawBackground(e.Cache, r);
			if(fi.EditViewInfo != null) {
				var clip = e.Cache.ClipInfo.SaveAndSetClip(fi.ValueBounds);
				try {
					fi.EditViewInfo.MatchedString = "";
					if(View.IsAllowHighlightFind(fi.Column)) {
						fi.EditViewInfo.UseHighlightSearchAppearance = true;
						fi.EditViewInfo.MatchedStringUseContains = true;
						fi.EditViewInfo.MatchedString = View.GetFindMatchedText(fi.Column.FieldName, fi.EditViewInfo.DisplayText);
					}
					DrawAppearanceMethod drawContent = (cache, appearance) => {
						View.GridControl.EditorHelper.DrawCellEdit(e, fi.Editor, fi.EditViewInfo, appearance, Point.Empty);
					};
					if(fi.Card.FormatInfo.ApplyDrawFormat(e.Cache, fi.Column, fi.ValueBounds, fi.Card.RowHandle, fi.EditViewInfo, drawContent, cs.Appearance)) return;
					drawContent(e.Cache, cs.Appearance);
				}
				finally {
					e.Cache.ClipInfo.RestoreClipRelease(clip);
				}
			}
		}
		protected virtual void DrawField(CardViewDrawArgs e, CardFieldInfo fi, bool focused) {
			RowCellCustomDrawEventArgs cs = new RowCellCustomDrawEventArgs(e.Cache, fi.Bounds, fi.PaintAppearanceCaption, fi.Card.RowHandle, fi.Column, fi.EditViewInfo.DisplayText, fi.FieldCaption);
			e.ViewInfo.View.RaiseCustomDrawField(cs);
			if(cs.Handled) return;
			if(!e.ViewInfo.PaintAnimatedItems) {
				DrawFieldCaption(e, fi, focused);
			}
			DrawFieldValue(e, fi);
			if(!e.ViewInfo.PaintAnimatedItems)
			if(focused)
				e.Paint.DrawFocusRectangle(e.Graphics, fi.BorderBounds, e.ViewInfo.PaintAppearance.Card.ForeColor, e.ViewInfo.PaintAppearance.Card.BackColor);
		}
		protected virtual void DrawCardCaptionBackground(CardViewDrawArgs e, CardInfo ci, AppearanceObject appearance) {
			CardObjectInfoArgs info = new CardObjectInfoArgs(e.ViewInfo, ci);
			info.Bounds = ci.CaptionInfo.Bounds;
			ObjectPainter.DrawObject(e.Cache, ElementsPainter.CardCaption, info);
		}
		protected virtual void DrawCardCaption(CardViewDrawArgs e, CardInfo ci) {
			if(ci.CaptionInfo.Bounds.IsEmpty) return;
			AppearanceObject appearance = ci.CaptionInfo.PaintAppearance;
			CardCaptionCustomDrawEventArgs cs = new CardCaptionCustomDrawEventArgs(e.Cache, ci, appearance);
			cs.SetDefaultDraw(() => {
				DrawCardCaptionCore(e, ci, cs);
			});
			e.ViewInfo.View.RaiseCustomDrawCardCaption(cs);
			cs.DefaultDraw();
		}
		private void DrawCardCaptionCore(CardViewDrawArgs e, CardInfo ci, CardCaptionCustomDrawEventArgs cs) {
			DrawCardCaptionBackground(e, ci, cs.Appearance);
			Rectangle r = ci.CaptionInfo.Bounds;
			CardCaptionViewInfo ciInfo = ci.CaptionInfo;
			if(!ciInfo.ErrorIconBounds.IsEmpty) {
				Rectangle image = ciInfo.ErrorIconBounds;
				image.Size = BaseEdit.DefaultErrorIcon.Size;
				image.X += (ciInfo.ErrorIconBounds.Width - image.Width) / 2;
				image.Y += (ciInfo.ErrorIconBounds.Height - image.Height) / 2;
				e.Graphics.DrawImage(ciInfo.ErrorIcon, image);
			}
			DrawCardExpandButton(e, ci.CaptionInfo, cs.Appearance);
			DrawCardImage(e, ci.CaptionInfo, cs.Appearance);
			DrawCardText(e, cs, ciInfo);
		}
		protected virtual void DrawCardText(CardViewDrawArgs e, CardCaptionCustomDrawEventArgs cs, CardCaptionViewInfo ciInfo) {
			int containsIndex; string matchedText;
			if(View.TryGetMatchedTextFromCaption(ciInfo.CardCaption, out matchedText, out containsIndex)) {
				DrawHighlightString(e.Cache, cs.Appearance, ciInfo.CardCaption, ciInfo.CaptionTextBounds, matchedText, containsIndex);
			}
			else cs.Appearance.DrawString(e.Cache, ciInfo.CardCaption, ciInfo.CaptionTextBounds);
		}
		protected virtual void DrawHighlightString(GraphicsCache cache, AppearanceObject appearance, string text, Rectangle bounds, string highlightText, int containsIndex) {
			AppearanceDefault highlight = LookAndFeelHelper.GetHighlightSearchAppearance(View.LookAndFeel, false);
			cache.Paint.DrawMultiColorString(cache, bounds, text, highlightText, appearance, appearance.GetTextOptions().GetStringFormat(),
				highlight.ForeColor, highlight.BackColor, false, containsIndex);
		}
		protected virtual void DrawCardImage(CardViewDrawArgs e, CardCaptionViewInfo captionInfo, AppearanceObject appearance) {
			if(captionInfo.ImageBounds.IsEmpty) return;
			CardCaptionImageEventArgs args = new CardCaptionImageEventArgs(captionInfo.Card.RowHandle, View.Images);
			View.RaiseCustomCardCaptionImage(args);
			args.DrawImage(e, captionInfo.ImageBounds);
		}
		protected virtual void DrawCardExpandButton(CardViewDrawArgs e, CardCaptionViewInfo captionInfo, AppearanceObject appearance) {
			if(captionInfo.ExpandButtonBounds.IsEmpty) return;
			ObjectState state = CalcObjectState(e, captionInfo, CardHitTest.CardExpandButton, CardState.CardExpandButtonPressed);
			ExplorerBarOpenCloseButtonInfoArgs info = new ExplorerBarOpenCloseButtonInfoArgs(e.Cache, captionInfo.ExpandButtonBounds, 
				e.ViewInfo.PaintAppearance.CardExpandButton, state, !View.GetCardCollapsed(captionInfo.Card.RowHandle));
			info.BackAppearance = appearance;
			ElementsPainter.CardExpandButton.DrawObject(info);
		}
		ObjectState CalcObjectState(CardViewDrawArgs e, CardCaptionViewInfo captionInfo, CardHitTest hitTest, CardState cardState) {
			ObjectState state = ObjectState.Normal;
			if(IsCard(captionInfo.Card, e.ViewInfo.SelectionInfo.HotTrackedInfo)) {
				if(e.ViewInfo.SelectionInfo.HotTrackedInfo.RowHandle == captionInfo.Card.RowHandle && e.ViewInfo.SelectionInfo.HotTrackedInfo.HitTest == hitTest)
					state |= ObjectState.Hot;
			}
			if(IsCard(captionInfo.Card, e.ViewInfo.SelectionInfo.PressedInfo) && View.State == cardState) 
				state |= ObjectState.Pressed;
			return state;
		}
		ObjectState CalcObjectState(CardViewDrawArgs e, CardHitTest hitTest, CardState cardState) {
			ObjectState state = ObjectState.Normal;
			if(e.ViewInfo.SelectionInfo.HotTrackedInfo.HitTest == hitTest) state |= ObjectState.Hot;
			if(View.State == cardState) state |= ObjectState.Pressed;
			return state;
		}
		bool IsCard(CardInfo ci, CardHitInfo hitInfo) {
			return hitInfo != null && hitInfo.RowHandle == ci.RowHandle;
		}
		protected virtual void DrawCardButton(CardViewDrawArgs e, CardInfo ci, Rectangle bounds, bool isUpButton) {
			if(bounds.IsEmpty) return;
			CardHitTest validHitTest = isUpButton ? CardHitTest.CardUpButton : CardHitTest.CardDownButton;
			bool enabled = true;
			EditorButtonObjectInfoArgs info = new EditorButtonObjectInfoArgs(e.Cache, e.ViewInfo.CardScrollButton, null);
			info.State = ObjectState.Normal;
			if(e.ViewInfo.SelectionInfo.HotTrackedInfo.RowHandle == ci.RowHandle) {
				if(e.ViewInfo.SelectionInfo.HotTrackedInfo.HitTest == validHitTest) 
					info.State = ObjectState.Hot;
			}
			if(ci.RowHandle == e.ViewInfo.View.FocusedRowHandle) {
				if((e.ViewInfo.View.State == CardState.CardDownButtonPressed && !isUpButton) ||
					(e.ViewInfo.View.State == CardState.CardUpButtonPressed && isUpButton)) {
					info.State = ObjectState.Pressed;
				}
				if(isUpButton) {
					if(e.ViewInfo.View.FocusedCardTopFieldIndex == 0) 
						enabled = false;
				} else {
					if(!ci.CanScrollDown) 
						enabled = false;
				}
			} else {
				if(isUpButton) enabled = false;
				else {
					if(!ci.CanScrollDown) enabled = false;
				}
			}
			e.ViewInfo.CardScrollButton.Enabled = enabled;
			if(!enabled) 
				info.State = ObjectState.Disabled;
			if(isUpButton)
				e.ViewInfo.CardScrollButton.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Up;
			else
				e.ViewInfo.CardScrollButton.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Down;
			info.Bounds = bounds;
			ObjectPainter.DrawObject(e.Cache, isUpButton ? e.ViewInfo.ScrollUpPainter : e.ViewInfo.ScrollDownPainter, info);
		}
		protected virtual void DrawCardButtons(CardViewDrawArgs e, CardInfo ci) {
			DrawCardButton(e, ci, ci.UpButtonBounds, true);
			DrawCardButton(e, ci, ci.DownButtonBounds, false);
		}
		protected virtual void DrawCard(CardViewDrawArgs e, CardInfo ci) {
			if(!IsNeedDrawRect(e, ci.Bounds)) return;
			if(!e.ViewInfo.PaintAnimatedItems) {
				e.ViewInfo.UpdateBeforePaint(ci);
				DrawCardBackground(e, ci);
				DrawCardCaption(e, ci);
				DrawCardButtons(e, ci);
			}
			else { 
			}
			foreach(CardFieldInfo fi in ci.Fields) {
				DrawField(e, fi, 
					((ci.State & GridRowCellState.FocusedAndGridFocused) == GridRowCellState.FocusedAndGridFocused) && 
					fi.Column == View.FocusedColumn);
			}
		}
		protected virtual void DrawCardBackground(CardViewDrawArgs e, CardInfo ci) {
			ObjectPainter.DrawObject(e.Cache, ElementsPainter.Card, new CardObjectInfoArgs(e.ViewInfo, ci));
		}
		protected virtual void DrawCards(CardViewDrawArgs e) {
			if(!e.ViewInfo.PaintAnimatedItems) {
				DrawIndents(e, e.ViewInfo.Indents, null);
			}
			foreach(CardInfo ci in e.ViewInfo.Cards) {
				DrawCard(e, ci);
			}
			if(!e.ViewInfo.PaintAnimatedItems)
			if(e.ViewInfo.View.RowCount == 0) 
				e.ViewInfo.View.RaiseCustomDrawEmptyForeground(new CustomDrawEventArgs(e.Cache, e.ViewInfo.ViewRects.Client, e.ViewInfo.PaintAppearance.EmptySpace));
		}
		protected virtual void DrawButtons(CardViewDrawArgs e) {
			if(!e.ViewInfo.ViewRects.QuickCustomize.IsEmpty) {
				e.ViewInfo.QuickCustomizeButton.Bounds = e.ViewInfo.ViewRects.QuickCustomize;
				e.ViewInfo.QuickCustomizeButton.State = CalcObjectState(e, CardHitTest.QuickCustomizeButton, CardState.QuickCustomizeButtonPressed);
				if(View.CustomizationForm != null) e.ViewInfo.QuickCustomizeButton.State |= ObjectState.Pressed;
				ObjectPainter.DrawObject(e.Cache, ElementsPainter.Buttons, e.ViewInfo.QuickCustomizeButton);
			}
			if(!e.ViewInfo.ViewRects.CloseZoom.IsEmpty) {
				e.ViewInfo.CloseZoomButton.Bounds = e.ViewInfo.ViewRects.CloseZoom;
				e.ViewInfo.CloseZoomButton.State = CalcObjectState(e, CardHitTest.CloseZoomButton, CardState.CloseZoomButtonPressed);
				ObjectPainter.DrawObject(e.Cache, ElementsPainter.Buttons, e.ViewInfo.CloseZoomButton);
			}
		}
		public override void Draw(ViewDrawArgs ee) {
			CardViewDrawArgs e = ee as CardViewDrawArgs;
			GraphicsClipState clip = null;
			e.ViewInfo.PaintAnimatedItems = false;
			if(!e.ViewInfo.PaintAnimatedItems) {
				HideSizerLine();
				DoBeforeDraw(e);
				clip = e.Cache.ClipInfo.SaveAndSetClip(e.ViewInfo.ViewRects.Bounds);
				DrawBorder(e, e.ViewInfo.ViewRects.Bounds);
				DrawViewCaption(e);
				DrawFilterPanel(e);
				e.Cache.ClipInfo.RestoreClipRelease(clip);
				clip = e.Cache.ClipInfo.SaveAndSetClip(e.ViewInfo.ViewRects.Client, true, true);
			}
			try {
				if(!e.ViewInfo.PaintAnimatedItems) {
					e.ViewInfo.PaintAppearance.EmptySpace.DrawBackground(e.Cache, e.ViewInfo.ViewRects.Client);
					DrawButtons(e);
				}
				DrawCards(e);
			}
			finally {
				if(!e.ViewInfo.PaintAnimatedItems) {
					e.Cache.ClipInfo.RestoreClipRelease(clip);
					DoAfterDraw(e);
				}
			}
			if(!e.ViewInfo.PaintAnimatedItems) {
				base.Draw(e);
			}
			e.ViewInfo.PaintAnimatedItems = true;
		}
		public override void DrawSizerLine() {
			CardViewInfo viewInfo = View.ViewInfo as CardViewInfo;
			int startX, startY, endX, endY;
			startX = startY = endX = endY = -1000;
			if(View.State == CardState.Sizing) {
				startX = fCurrentSizerPos;
				startY = viewInfo.ViewRects.Cards.Y;
				endX = fCurrentSizerPos;
				endY = viewInfo.ViewRects.Cards.Bottom;
				if(startX > viewInfo.ViewRects.Cards.Right) return;
				if(startX < viewInfo.ViewRects.Cards.Left)
					return;
				Point startPoint = new Point(startX, startY),
					endPoint = new Point(endX, endY);
				SplitterLineHelper.Default.DrawReversibleLine(View.GridControl.Handle, startPoint, endPoint);
			}
		}
		protected override ObjectState CalcFilterTextState(ViewDrawArgs e) { 
			if(View.MRUFilterPopup != null) return ObjectState.Pressed;
			return CalcObjectState(e as CardViewDrawArgs, CardHitTest.FilterPanelText, CardState.FilterPanelTextPressed);
		}
		protected override ObjectState CalcFilterCloseButtonState(ViewDrawArgs e) { 
			return CalcObjectState(e as CardViewDrawArgs, CardHitTest.FilterPanelCloseButton, CardState.FilterPanelCloseButtonPressed);
		}
		protected override ObjectState CalcFilterActiveButtonState(ViewDrawArgs e) { 
			return CalcObjectState(e as CardViewDrawArgs, CardHitTest.FilterPanelActiveButton, CardState.FilterPanelActiveButtonPressed);
		}
		protected override ObjectState CalcFilterMRUButtonState(ViewDrawArgs e) { 
			if(View.MRUFilterPopup != null) return ObjectState.Pressed;
			return CalcObjectState(e as CardViewDrawArgs, CardHitTest.FilterPanelMRUButton, CardState.FilterPanelMRUButtonPressed);
		}
		protected override ObjectState CalcFilterCustomizeButtonState(ViewDrawArgs e) {
			return CalcObjectState(e as CardViewDrawArgs, CardHitTest.FilterPanelCustomizeButton, CardState.FilterPanelCustomizeButtonPressed);
		}
	}
	public class CardObjectInfoArgs : StyleObjectInfoArgs {
		CardInfo card;
		CardViewInfo viewInfo;
		public CardObjectInfoArgs(CardViewInfo viewInfo, CardInfo card) {
			this.card = card;
			this.viewInfo = viewInfo;
			this.SetAppearance(card.CaptionInfo.PaintAppearance);
			this.Bounds = Card.Bounds;
		}
		public Rectangle CardClientBounds {
			get {
				Rectangle r = Card.Bounds;
				if(!Card.CaptionInfo.Bounds.IsEmpty) {
					r.Y = Card.CaptionInfo.Bounds.Bottom - 1;
					r.Height = Card.Bounds.Bottom - r.Y;
				}
				return r;
			}
		}
		public CardInfo Card { get { return card; } }
		public CardViewInfo ViewInfo { get { return viewInfo; } }
	}
	public class CardObjectPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			CardObjectInfoArgs ee = (CardObjectInfoArgs)e;
			AppearanceObject card = ee.ViewInfo.PaintAppearance.Card;
			e.Cache.Paint.DrawRectangle(e.Graphics, card.GetBorderPen(e.Cache), ee.Card.Bounds);
			ee.ViewInfo.PaintAppearance.Card.FillRectangle(e.Cache, ee.Card.ClientBounds);
			AppearanceObject border = ee.Card.CaptionInfo.PaintAppearance;
			e.Cache.Paint.DrawRectangle(e.Graphics, border.GetBorderPen(e.Cache), ee.Card.Bounds);
		}
	}
	public class CardObjectOfficePainter : CardObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			CardObjectInfoArgs ee = (CardObjectInfoArgs)e;
			AppearanceObject card = ee.ViewInfo.PaintAppearance.Card;
			AppearanceObject border = ee.Card.CaptionInfo.PaintAppearance;
			Rectangle r = ee.CardClientBounds;
			e.Cache.Paint.DrawRectangle(e.Graphics, card.GetBorderPen(e.Cache), r);
			ee.ViewInfo.PaintAppearance.Card.FillRectangle(e.Cache, ee.Card.ClientBounds);
			e.Cache.Paint.DrawRectangle(e.Graphics, border.GetBorderPen(e.Cache), r);
		}
	}
	public class CardCaptionPainter : StyleObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			if(e.Bounds.IsEmpty) return;
			GetStyle(e).FillRectangle(e.Cache, e.Bounds);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { 
			Rectangle r = e.Bounds;
			r.Inflate(-2, -1);
			return r;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { 
			client.Inflate(2, 1); 
			return client;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) { 
			return Rectangle.Empty;
		}
	}
	public class CardCaptionOfficePainter : CardCaptionPainter {
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			client.Inflate(2, 1);
			client.Height += 1;
			return client;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			r.Inflate(-2, -1);
			return r;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			if(e.Bounds.IsEmpty) return;
			Rectangle r = e.Bounds;
			r.Inflate(1, 0);
			using(Region reg = new Region(new Rectangle(r.X, r.Y + 2, r.Width, r.Height - 2))) {
				reg.Union(new Rectangle(r.X + 2, r.Y, r.Width - 4, 1));
				reg.Union(new Rectangle(r.X + 1, r.Y + 1, r.Width - 2, 1));
				Brush brush = GetStyle(e).GetBackBrush(e.Cache, r);
				e.Graphics.FillRegion(brush, reg);
			}
		}
	}
	public class CardElementsPainter {
		ObjectPainter cardExpandButton, buttons, filterPanel, upButton, downButton, card, cardCaption, viewCaption;
		BaseView view;
		public CardElementsPainter(BaseView view) {
			this.view = view;
			this.viewCaption = CreateViewCaptionPainter();
			this.cardExpandButton = CreateCardExpandButtonPainter();
			this.filterPanel = CreateFilterPanelPainter();
			this.buttons = CreateButtonsPainter();
			this.upButton = CreateUpButtonPainter();
			this.downButton = CreateDownButtonPainter();
			this.cardCaption = CreateCardCaptionPainter();
			this.card = CreateCardPainter();
		}
		public UserLookAndFeel ElementsLookAndFeel  { get { return View.ElementsLookAndFeel; } }
		public virtual BaseView View  { get { return view; } }
		public virtual bool AllowCustomButtonPainter { get { return true; } }
		protected virtual ObjectPainter CreateCardCaptionPainter() { return new CardCaptionPainter(); }
		protected virtual ObjectPainter CreateCardPainter() { return new CardObjectPainter(); }
		protected virtual ObjectPainter CreateViewCaptionPainter() { return new GridViewCaptionPainter(View); }
		protected virtual ObjectPainter CreateButtonsPainter() { return EditorButtonHelper.GetPainter(BorderStyles.Default, ElementsLookAndFeel); }
		protected virtual ObjectPainter CreateCardExpandButtonPainter() { return new ExplorerBarOpenCloseButtonObjectPainter(); }
		protected virtual ObjectPainter CreateUpButtonPainter() { return CreateButtonsPainter(); }
		protected virtual ObjectPainter CreateDownButtonPainter() { return CreateButtonsPainter(); }
		protected virtual ObjectPainter CreateFilterPanelPainter() { return new GridFilterPanelPainter(EditorButtonHelper.GetPainter(BorderStyles.Default, ElementsLookAndFeel), CheckPainterHelper.GetPainter(ElementsLookAndFeel)); }
		public ObjectPainter Card { get { return card; } }
		public ObjectPainter CardCaption { get { return cardCaption; } }
		public ObjectPainter FilterPanel { get { return filterPanel; } }
		public ObjectPainter ViewCaption { get { return viewCaption; } }
		public ObjectPainter CardExpandButton { get { return cardExpandButton; } }
		public ObjectPainter Buttons { get { return buttons; } }
		public ObjectPainter UpButton { get { return upButton; } }
		public ObjectPainter DownButton { get { return downButton; } }
	}
	public class CardElementsPainterOffice : CardElementsPainter {
		public CardElementsPainterOffice(BaseView view) : base(view) { }
		protected override ObjectPainter CreateCardExpandButtonPainter() {
			return new AdvExplorerBarOpenCloseButtonObjectPainter();
		}
		protected override ObjectPainter CreateCardCaptionPainter() { return new CardCaptionOfficePainter(); }
		protected override ObjectPainter CreateCardPainter() { return new CardObjectOfficePainter(); }
	}
	public class CardElementsPainterOffice2003 : CardElementsPainterOffice {
		public CardElementsPainterOffice2003(BaseView view) : base(view) { }
		protected override ObjectPainter CreateCardExpandButtonPainter() {
			return new AdvExplorerBarOpenCloseButtonObjectPainter();
		}
		protected override ObjectPainter CreateCardCaptionPainter() { return new CardCaptionOfficePainter(); }
		protected override ObjectPainter CreateCardPainter() { return new CardObjectOfficePainter(); }
	}
}
