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

using System.Collections;
using System.Drawing;
using System;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Card.Drawing;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing.Animation;
using System.Drawing.Imaging;
using System.Collections.Generic;
using DevExpress.Skins;
namespace DevExpress.XtraGrid.Views.Card.ViewInfo {
	public class CardFieldId : CellId
	{
		public CardFieldId(BaseView view, CardFieldInfo info) : base(view.GetRow(info.Card.RowHandle), info.Column) {}
	}
	public class CardCaptionViewInfo {
		public Rectangle Bounds, CaptionTextBounds, ErrorIconBounds, ExpandButtonBounds, ImageBounds;
		public string CardCaption, ErrorText;
		internal Image ErrorIcon; 
		public AppearanceObject PaintAppearance = null;
		CardInfo card;
		public CardCaptionViewInfo(CardInfo card) {
			this.card = card;
			this.ImageBounds = this.Bounds = this.CaptionTextBounds = this.ErrorIconBounds = this.ExpandButtonBounds = Rectangle.Empty;
			this.CardCaption = this.ErrorText = string.Empty;
		}
		public CardInfo Card { get { return card; } }
		public CardView View { get { return Card.ViewInfo.View; } }
		protected virtual void CalcCardErrorInfo() {
			string error;
			DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType;
			View.GetColumnError(Card.RowHandle, null, out error, out errorType);
			ErrorText = error;
			ErrorIconBounds = Rectangle.Empty;
			if(ErrorText != null && ErrorText.Length > 0) {
				if(CaptionTextBounds.IsEmpty || CaptionTextBounds.Width < BaseEdit.DefaultErrorIcon.Size.Width + 5) return;
				ErrorIconBounds = CaptionTextBounds;
				ErrorIconBounds.Width = BaseEdit.DefaultErrorIcon.Size.Width + 4;
				CaptionTextBounds.X += ErrorIconBounds.Width + 2;
				CaptionTextBounds.Width -= (ErrorIconBounds.Width + 2);
				ErrorIcon = DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider.GetErrorIconInternal(errorType);
			}
		}
		protected virtual void CalcCardImageBounds() {
			ImageBounds = Rectangle.Empty;
			CardCaptionImageEventArgs e = new CardCaptionImageEventArgs(Card.RowHandle, View.Images);
			View.RaiseCustomCardCaptionImage(e);
			Size imageSize = e.ImageSize;
			if(!imageSize.IsEmpty) {
				if(CaptionTextBounds.IsEmpty || CaptionTextBounds.Width < imageSize.Width + 4) return;
				ImageBounds = CaptionTextBounds;
				ImageBounds.Size = imageSize;
				ImageBounds.Y += (CaptionTextBounds.Height - imageSize.Height) / 2;
				if(IsRightToLeft) {
					ImageBounds.X = CaptionTextBounds.Right - ImageBounds.Width;
					CaptionTextBounds.Width -= (ImageBounds.Width + 2);
				}
				else {
					CaptionTextBounds.X += ImageBounds.Width + 2;
					CaptionTextBounds.Width -= (ImageBounds.Width + 2);
				}
			}
		}
		bool IsRightToLeft { get { return View.IsRightToLeft; } }
		public void UpdateCardCaptionBounds(Rectangle clientBounds) {
			CardCaption = View.GetCardCaption(Card.RowHandle);
			ExpandButtonBounds = CaptionTextBounds = Bounds = ErrorIconBounds = Rectangle.Empty;
			if(!View.OptionsView.ShowCardCaption) return;
			Rectangle r = clientBounds;
			r.Height = Card.ViewInfo.CalcCardCaptionHeight(Card);
			Bounds = r;
			CardObjectInfoArgs cardInfo = new CardObjectInfoArgs(View.ViewInfo, Card);
			cardInfo.Bounds = r;
			r = ObjectPainter.GetObjectClientRectangle(this.View.ViewInfo.GInfo.Graphics, View.ViewInfo.ElementsPainter.CardCaption, cardInfo);
			int rightIndent = Bounds.Right - r.Right;
			CaptionTextBounds = r;
			if(View.OptionsView.ShowCardExpandButton) {
				if(Bounds.Width > CardExpandButtonSize.Width + 8) {
					r.Y += (r.Height - CardExpandButtonSize.Height) / 2;
					if(IsRightToLeft) {
						r.Size = CardExpandButtonSize;
						r.X += GetButtonIndent();
						ExpandButtonBounds = r;
						var bounds = CaptionTextBounds;
						CaptionTextBounds.X = r.Right + rightIndent;
						CaptionTextBounds.Width = bounds.Right - CaptionTextBounds.X;
					}
					else {
						r.X = r.Right - (CardExpandButtonSize.Width + GetButtonIndent());
						r.Size = CardExpandButtonSize;
						ExpandButtonBounds = r;
						CaptionTextBounds.Width = r.X - CaptionTextBounds.X - rightIndent;
					}
				}
			}
			CalcCardImageBounds();
			CalcCardErrorInfo();
		}
		int GetButtonIndent() {
			if(!IsSkinned) return 8;
			return GridSkins.GetSkin(View)[GridSkins.SkinCardCaption].Properties.GetInteger(GridSkins.OptCardCaptionExpandButtonIndent);
		}
		public bool IsSkinned { get { return Card.ViewInfo.IsSkinned; } }
		public Size CardExpandButtonSize { get { return Card.ViewInfo.CardExpandButtonSize; } }
	}
	public class CardInfo : IDisposable, IRowConditionFormatProvider {
		public Rectangle Bounds, ClientBounds,
			UpButtonBounds, DownButtonBounds;
		public bool CanScrollDown;
		int rowHandle, visibleIndex;
		Point position;
		ConditionInfo conditionInfo;
		RowFormatRuleInfo formatInfo;
		CardFieldInfoCollection fields = null;
		CardCaptionViewInfo captionInfo;
		CardViewInfo viewInfo;
		GridRowCellState state;
		public CardInfo(CardViewInfo viewInfo) {
			this.state = GridRowCellState.Dirty;
			this.viewInfo = viewInfo;
			this.captionInfo = CreateCaptionInfo();
			this.conditionInfo = new ConditionInfo();
			this.formatInfo = new RowFormatRuleInfo(viewInfo.View);
			this.rowHandle = 0;
			this.visibleIndex = 0;
			UpButtonBounds = DownButtonBounds = Bounds = ClientBounds = Rectangle.Empty;
			CanScrollDown = false;
		}
		public virtual void Dispose() {
			if(fields != null) fields.Clear();
		}
		public GridRowCellState State { get { return state; } set { state = value; } }
		public CardViewInfo ViewInfo { get { return viewInfo; } }
		protected virtual CardCaptionViewInfo CreateCaptionInfo() { return new CardCaptionViewInfo(this); }
		public ConditionInfo ConditionInfo { get { return conditionInfo; } }
		public RowFormatRuleInfo FormatInfo { get { return formatInfo; } }
		public CardCaptionViewInfo CaptionInfo { get { return captionInfo; } }
		public CardFieldInfoCollection Fields { 
			get { 
				if(fields == null) fields = new CardFieldInfoCollection();
				return fields;
			}
		}
		public AppearanceObjectEx CardFormatAppearance {
			get {
				var res = FormatInfo.RowAppearance;
				if(res != null) return res;
				return ConditionInfo.RowConditionAppearance;
			}
		}
		protected internal AppearanceObjectEx GetConditionAppearance(GridColumn column) {
			var res = FormatInfo.GetCellAppearance(column);
			if(res != null) return res;
			return ConditionInfo.GetCellAppearance(column);
		}
		public bool IsSpecialCard { get { return RowHandle == GridControl.NewItemRowHandle; } }
		public int VisibleIndex { get { return visibleIndex; } set { visibleIndex = value; } }
		public int RowHandle { get { return rowHandle; } set { rowHandle = value; } }
		public Point Position { get { return position; } set { position = value; } }
	}
	public class CardFieldInfo : IDisposable {
		GridRowCellState state;
		CardInfo card;
		BaseEditViewInfo editViewInfo = null;
		public Rectangle Bounds, BorderBounds, CaptionBounds, ValueBounds;
		public RepositoryItem Editor;
		public AppearanceObject PaintAppearanceCaption, PaintAppearanceValue;
		public int MinHeight, MaxHeight, Height;
		public GridColumn Column;
		public bool IsReady = false;
		public CardFieldInfo(AppearanceObject paintAppearanceCaption, AppearanceObject paintAppearanceValue, CardInfo card, GridColumn column) {
			this.state = GridRowCellState.Dirty;
			this.Height = -1;
			this.MinHeight = this.MaxHeight = 0;
			this.card = card;
			this.Column = column;
			BorderBounds = Bounds = CaptionBounds = ValueBounds = Rectangle.Empty;
			this.Editor = null;
			this.PaintAppearanceCaption = paintAppearanceCaption;
			this.PaintAppearanceValue = paintAppearanceValue;
		}
		public GridRowCellState State { get { return state; } set { state = value; } }
		public CardInfo Card { get { return card; } }
		public string FieldCaption { 
			get { return GetFieldCaption(Column); }
		}
		public virtual void Dispose() {
			EditViewInfo = null;
		}
		public static string GetFieldCaption(GridColumn column) {
			if(column.GetTextCaption() == string.Empty) return string.Empty;
			return column.GetTextCaption() + ":";
		}
		public BaseEditViewInfo EditViewInfo {
			get { return editViewInfo; }
			set {
				if(EditViewInfo == value) return;
				if(editViewInfo != null) editViewInfo.Dispose();
				editViewInfo = value;
			}
		}
	}
	internal class NullCardViewInfo : CardViewInfo {
		public NullCardViewInfo(CardView view) : base(view) { }
		public override bool IsReady { get { return false; } set { } }
		public override bool IsNull { get { return true; } }
	}
	public class CardViewInfo : ColumnViewInfo, ISupportXtraAnimation {
		CardInfoCollection cards;
		CardViewRects viewRects;
		GridRowCollection visibleRows;
		EditorButtonObjectInfoArgs quickCustomizeButton, closeZoomButton;
		public EditorButton CardScrollButton;
		public IndentInfoCollection Indents;
		public Graphics Graphics;
		public int CardFieldCaptionWidth,
				   CardWidth,
				   CardScrollButtonHeight;
		public Size CardExpandButtonSize;
		ObjectPainter scrollUpPainter, scrollDownPainter;
		public CardViewInfo(CardView view) : base(view) {
			this.cards = new CardInfoCollection();
			this.viewRects = new CardViewRects();
			this.visibleRows = new GridRowCollection();
			Indents = new IndentInfoCollection();
			CardScrollButton = new EditorButton();
			CardScrollButton.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Up;
			CardScrollButtonHeight = 8;
			CreateButtons();
			UpdateButtonsText();
			Clear();
		}
		protected virtual int MaxLoadRows { get { return View.MaxLoadRows; } }
		public override Rectangle ViewCaptionBounds { get { return ViewRects.ViewCaption; } }
		public override Rectangle Bounds { get { return ViewRects.Bounds; } }
		public override Rectangle ClientBounds { get { return ViewRects.Client; } }
		void CreateButtons() {
			this.quickCustomizeButton = new EditorButtonObjectInfoArgs(new EditorButton(ButtonPredefines.Glyph), null);
			this.quickCustomizeButton.BuiltIn = false;
			if(View.IsZoomedView) {
				this.closeZoomButton = new EditorButtonObjectInfoArgs(new EditorButton(ButtonPredefines.Close), null);
			} else {
				this.closeZoomButton = new EditorButtonObjectInfoArgs(new EditorButton(ButtonPredefines.Glyph), null);
				this.closeZoomButton.Button.Image = Grid.Drawing.GridPainter.Indicator.Images[Grid.Drawing.GridPainter.IndicatorZoom];
			}
			this.closeZoomButton.BuiltIn = false;
		}
		void UpdateButtonsText() {
			this.quickCustomizeButton.Button.Caption = GridLocalizer.Active.GetLocalizedString(GridStringId.CardViewQuickCustomizationButton);
		}
		public EditorButtonObjectInfoArgs QuickCustomizeButton { get { return quickCustomizeButton; } }
		public EditorButtonObjectInfoArgs CloseZoomButton { get { return closeZoomButton; } }
		public GridRowCollection VisibleRows { get { return visibleRows; } }
		public new CardViewAppearances PaintAppearance { get { return base.PaintAppearance as CardViewAppearances; } }
		protected override BaseAppearanceCollection CreatePaintAppearances() { return new CardViewAppearances(View); }
		public new CardSelectionInfo SelectionInfo { get { return base.SelectionInfo as CardSelectionInfo; } }
		protected override BaseSelectionInfo CreateSelectionInfo() {
			return new CardSelectionInfo(View);
		}
		protected override void UpdatePainters() {
			base.UpdatePainters();
			if(ElementsPainter == null || ElementsPainter.AllowCustomButtonPainter) {
				this.scrollDownPainter = this.scrollUpPainter = EditorButtonHelper.GetPainter(View.CardScrollButtonBorderStyle, View.ElementsLookAndFeel);
			} else {
				this.scrollDownPainter = ElementsPainter.DownButton;
				this.scrollUpPainter = ElementsPainter.UpButton;
			}
		}
		public override ObjectPainter FilterPanelPainter { get { return ElementsPainter.FilterPanel; } }
		public override ObjectPainter ViewCaptionPainter { get { return ElementsPainter.ViewCaption; } }
		public CardInfoCollection Cards { get { return cards; } }
		public CardViewRects ViewRects { get { return viewRects; } }
		public virtual ObjectPainter ScrollUpPainter { get { return scrollUpPainter; } }
		public virtual ObjectPainter ScrollDownPainter { get { return scrollDownPainter; } }
		public new CardView View { get { return base.View as CardView; } }
		public override void Clear() {
			Indents.Clear();
			Cards.Clear();
			Graphics = null;
			IsReady = false;
			CardFieldCaptionWidth = 0;
		}
		public int GetScrollableColumnsCount(int rowHandle) {
			if(View.OptionsView.ShowEmptyFields || !View.IsValidRowHandle(rowHandle)) return View.VisibleColumns.Count;
			return GetVisibleColumnsIndexes(rowHandle).Count;
		}
		public List<int> GetVisibleColumnsIndexes(int rowHandle) {
			List<int> res = new List<int>();
			if(View == null) return res;
			if(View.OptionsView.ShowEmptyFields || !View.IsValidRowHandle(rowHandle)) {
				for(int n = 0; n < View.VisibleColumns.Count; n++) {
					res.Add(n);
				}
				return res;
			}
			for(int n = 0; n < View.VisibleColumns.Count; n++) {
				object val = View.GetRowCellValue(rowHandle, View.VisibleColumns[n]);
				if(val == null || val == DBNull.Value) continue; 
				res.Add(n);
			}
			return res;
		}
		protected Size CardBorderSize {
			get { return new Size(1, 1); }
		}
		protected internal virtual int CalcCardCaptionHeight(CardInfo ci) {
			if(!View.OptionsView.ShowCardCaption) return 0;
			CardCaptionImageEventArgs e = new CardCaptionImageEventArgs(ci.RowHandle, View.Images);
			View.RaiseCustomCardCaptionImage(e);
			int height = Math.Max(CalcMaxHeight(new AppearanceObject[] {PaintAppearance.CardCaption, PaintAppearance.FocusedCardCaption}), 
				BaseEdit.DefaultErrorIcon.Height);
			height = Math.Max(e.ImageSize.Height, Math.Max(CardExpandButtonSize.Height, height));
			height = ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, ElementsPainter.CardCaption, new CardObjectInfoArgs(this, ci), new Rectangle(0, 0, 100, height)).Height;
			return height;
		}
		protected int CalcFieldHeight(CardFieldInfo fi) {
			if(fi.Height > 0) return fi.Height;
			UpdateFieldAppearance(fi);
			Graphics g = GInfo.AddGraphics(null);
			fi.Card.FormatInfo.ApplyContextImage(GInfo.Cache, fi.Column, fi.ValueBounds, fi.Card.RowHandle, fi.EditViewInfo);
			int maxH = 10;
			try {
				maxH = Math.Max(maxH, CalcMaxHeight(new AppearanceObject[] { fi.PaintAppearanceCaption, fi.PaintAppearanceValue }) + 2);
				maxH = Math.Max(fi.EditViewInfo.CalcMinHeight(g) + 2, maxH);
				fi.MaxHeight = fi.MinHeight = maxH;
				if(View.OptionsBehavior.FieldAutoHeight) 
					fi.MaxHeight = maxH = CalcFieldAutoHeight(g, fi, maxH);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			maxH = View.RaiseCalcFieldHeight(new FieldHeightEventArgs(fi.Card.RowHandle, maxH, fi.Column));
			return maxH;
		}
		protected virtual int CalcFieldAutoHeight(Graphics g, CardFieldInfo fi, int maxH) {
			int result = 0;
			IHeightAdaptable ah = fi.EditViewInfo as IHeightAdaptable;
			if(ah != null) {
				UpdateFieldAppearance(fi);
				fi.EditViewInfo.PaintAppearance = fi.PaintAppearanceValue;
				fi.EditViewInfo.EditValue = View.GetRowCellValue(fi.Card.RowHandle, fi.Column);
				fi.EditViewInfo.SetDisplayText(View.RaiseCustomColumnDisplayText(fi.Card.RowHandle, fi.Column, fi.EditViewInfo.EditValue, fi.EditViewInfo.DisplayText, false));
				fi.State = GridRowCellState.Dirty;
				result = ah.CalcHeight(GInfo.Cache, fi.ValueBounds.Width);
			}
			return Math.Max(result + 2, maxH);
		}
		protected override void UpdateEditViewInfo(BaseEditViewInfo vi) {
			base.UpdateEditViewInfo(vi);
			vi.AllowTextToolTip = View.OptionsView.ShowFieldHints;
		}
		public virtual void UpdateBeforePaint(CardInfo ci) {
			bool conditionChanged = UpdateRowConditionAndFormat(ci.RowHandle, ci);
			if(conditionChanged) {
				ci.State = GridRowCellState.Dirty;
				foreach(CardFieldInfo fi in ci.Fields) {
					fi.State = GridRowCellState.Dirty;
				}
			}
			UpdateCardAppearance(ci);
			foreach(CardFieldInfo fi in ci.Fields) {
				RequestFieldEditViewInfo(fi);
				UpdateFieldAppearance(fi);
			}
		}
		public virtual void UpdateFieldAppearance(CardFieldInfo fi) {
			if(fi.State != GridRowCellState.Dirty) return;
			AppearanceObject[] mixCaption = new AppearanceObject[] { fi.Column.AppearanceHeader, PaintAppearance.FieldCaption };
			if(fi.PaintAppearanceCaption == null || fi.PaintAppearanceCaption == PaintAppearance.FieldCaption) 
				fi.PaintAppearanceCaption = new FrozenAppearance();
			AppearanceHelper.Combine(fi.PaintAppearanceCaption, mixCaption);
			AppearanceObjectEx condition = fi.Card.GetConditionAppearance(fi.Column), column;
			AppearanceObject columnLow, columnHigh, conditionLow, conditionHigh;
			if(fi.PaintAppearanceValue == null || fi.PaintAppearanceValue == PaintAppearance.FieldValue) 
				fi.PaintAppearanceValue = new FrozenAppearance();
			column = fi.Column.AppearanceCell;
			columnLow = columnHigh = conditionLow = conditionHigh = null;
			if(column != null) {
				if(column.Options.HighPriority)
					columnHigh = column;
				else
					columnLow = column;
			}
			if(condition != null) {
				if(condition.Options.HighPriority)
					conditionHigh = condition;
				else
					conditionLow = condition;
			}
			AppearanceObject[] mix = new AppearanceObject[] { columnHigh, conditionHigh, columnLow, conditionLow, PaintAppearance.FieldValue };
			AppearanceHelper.Combine(fi.PaintAppearanceValue, mix);
			if(fi.PaintAppearanceValue.HAlignment == HorzAlignment.Default) {
				fi.PaintAppearanceValue.TextOptions.HAlignment = View.GetRowCellDefaultAlignment(fi.Card.RowHandle, fi.Column, fi.Column.DefaultValueAlignment);
			}
			fi.State = GridRowCellState.Default;
			if(fi.EditViewInfo != null) fi.EditViewInfo.PaintAppearance = fi.PaintAppearanceValue;
		}
		protected void UpdateCardAppearance(CardInfo ci) { UpdateCardAppearance(ci, false); }
		protected internal virtual void UpdateCardAppearance(CardInfo ci, bool always) {
			GridRowCellState state = CalcRowStateCore(ci.RowHandle);
			if(state == ci.State && !always) return;
			ci.State = state;
			if(ci.CaptionInfo.PaintAppearance == null || ci.CaptionInfo.PaintAppearance == PaintAppearance.CardCaption) 
				ci.CaptionInfo.PaintAppearance = new AppearanceObject();
			AppearanceObject focused, selected, card;
			card = PaintAppearance.CardCaption;
			selected = PaintAppearance.SelectedCardCaption;
			focused = null;
			if((state & GridRowCellState.GridFocused) == 0) selected = PaintAppearance.HideSelectionCardCaption;
			if((state & GridRowCellState.Selected) == 0) selected = null;
			if((state & GridRowCellState.Focused) != 0) {
				focused = PaintAppearance.FocusedCardCaption;
				selected = null;
				if((state & GridRowCellState.GridFocused) == 0) {
					focused = PaintAppearance.HideSelectionCardCaption;
				} 
			}
			if((state & GridRowCellState.Focused) != 0) {
				if(View.IsMultiSelect && (state & GridRowCellState.Selected) == 0) focused = null;
			}
			AppearanceObject[] mix = new AppearanceObject[] { focused, selected, card };
			AppearanceHelper.Combine(ci.CaptionInfo.PaintAppearance, mix);
			if((state & GridRowCellState.Selected) == 0 && (state & GridRowCellState.FocusedAndGridFocused) == GridRowCellState.FocusedAndGridFocused) {
				if(View.OptionsSelection.MultiSelect) {
					ci.CaptionInfo.PaintAppearance.BackColor = PaintAppearance.CardCaption.BackColor;
					ci.CaptionInfo.PaintAppearance.BackColor2 = PaintAppearance.CardCaption.BackColor2;
					ci.CaptionInfo.PaintAppearance.GradientMode = PaintAppearance.CardCaption.GradientMode;
				}
			}
		}
		protected internal void RequestFieldEditViewInfo(CardFieldInfo fi) {
			if(!fi.IsReady && fi.EditViewInfo != null) {
				UpdateFieldData(fi);
				Point point = (GridControl != null && GridControl.IsHandleCreated ? View.PointToClient(Control.MousePosition) : EmptyPoint);
				fi.Card.FormatInfo.ApplyContextImage(GInfo.Cache, fi.Column, fi.ValueBounds, fi.Card.RowHandle, fi.EditViewInfo);
				fi.EditViewInfo.PaintAppearance = fi.PaintAppearanceValue;
				fi.EditViewInfo.UpdateBoundValues(View.DataController, fi.Card.RowHandle);
				fi.EditViewInfo.CalcViewInfo(GInfo.Graphics, Control.MouseButtons, point, fi.ValueBounds);
				CardFieldId id = new CardFieldId(View, fi);
				if(ShouldAddItem(fi.EditViewInfo, id))
					AddAnimatedItem(id, fi.EditViewInfo);
				fi.IsReady = true;
			}
		}
		protected void CalcCardFieldInfo(CardFieldInfo fi, int top) {
			int fieldHeight = -1;
			if(fi.Editor == null) {
				fi.Editor = View.GetRowCellRepositoryItem(fi.Card.RowHandle, fi.Column);
				CreateEditViewInfo(fi);
			} else {
				fieldHeight = fi.Bounds.Height;
			}
			fi.Bounds = fi.Card.ClientBounds;
			fi.Bounds.Y = top;
			fi.Bounds.Height = 18;
			CalcCardFieldRects(fi);
			if(fieldHeight == -1) fieldHeight = CalcFieldHeight(fi);
			fi.Bounds.Height = fieldHeight;
			CalcCardFieldRects(fi);
		}
		protected virtual void CalcCardFieldRects(CardFieldInfo fi) {
			Rectangle r = fi.BorderBounds = fi.Bounds;
			fi.BorderBounds.Inflate(-1, 0);
			r.Inflate(-2, -1);
			if(IsRightToLeft) {
				fi.CaptionBounds = r;
				fi.CaptionBounds.Width = Math.Min(CardFieldCaptionWidth, r.Width - 2);
				fi.CaptionBounds.X = r.Right - fi.CaptionBounds.Width;
				if(!View.OptionsView.ShowFieldCaptions) fi.CaptionBounds.Width = 0;
				fi.ValueBounds = r;
				if(fi.CaptionBounds.Width > 0) {
					fi.ValueBounds.Width = fi.CaptionBounds.X - 1 - fi.ValueBounds.X;
				}
			}
			else {
			fi.CaptionBounds = r;
			fi.CaptionBounds.Width = Math.Min(CardFieldCaptionWidth, r.Width - 2);
			if(!View.OptionsView.ShowFieldCaptions) fi.CaptionBounds.Width = 0;
			fi.ValueBounds = fi.CaptionBounds;
			fi.ValueBounds.X = fi.CaptionBounds.Right;
			fi.ValueBounds.Width = r.Right - fi.ValueBounds.X - 1;
		}
		}
		protected internal void SetCardInfoDirty(CardInfo ci) {
			if(ci.CaptionInfo != null)
				ci.CaptionInfo.CardCaption = View.GetCardCaption(ci.RowHandle);
			ci.State = GridRowCellState.Dirty;
			foreach(CardFieldInfo fi in ci.Fields) {
				fi.IsReady = false;
				fi.State = GridRowCellState.Dirty;
			}
		}
		protected virtual void UpdateFieldData(CardFieldInfo fi) {
			if(fi.EditViewInfo == null) return;
			object fieldValue = View.GetRowCellValue(fi.Card.RowHandle, fi.Column);
			string error;
			DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType;
			View.GetColumnError(fi.Card.RowHandle, fi.Column, out error, out errorType);
			fi.EditViewInfo.ErrorIconText = error;
			fi.EditViewInfo.ShowErrorIcon = fi.EditViewInfo.ErrorIconText != null && fi.EditViewInfo.ErrorIconText.Length > 0;
			if(fi.EditViewInfo.ShowErrorIcon)
				fi.EditViewInfo.ErrorIcon = DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider.GetErrorIconInternal(errorType);
			UpdateEditViewInfo(fi.EditViewInfo, fi.Column, fi.Card.RowHandle);
			fi.EditViewInfo.FillBackground = true;
			fi.EditViewInfo.EditValue = fieldValue;
			fi.EditViewInfo.SetDisplayText(View.RaiseCustomColumnDisplayText(fi.Card.RowHandle, fi.Column, fi.EditViewInfo.EditValue, fi.EditViewInfo.DisplayText, false));
		}
		protected int CardInterval { 
			get { return View.CardInterval; }
		}
		protected int SeparatorLineWidth { get { return (View.OptionsView.ShowLines ? 2 : 0); } }
		protected void CalcCardRects(CardInfo ci) {
			ci.ClientBounds = Rectangle.Inflate(ci.Bounds, -CardBorderSize.Width, -CardBorderSize.Height);
			ci.CaptionInfo.Bounds = Rectangle.Empty;
			ci.CanScrollDown = false;
		}
		protected int CalcCardTotalHeight(CardInfo ci, bool drawScrollButtons) {
			return CalcCardNonDataHeight(ci, drawScrollButtons) + CalcCardDataHeight(ci);
		}
		protected int CalcCardNonDataHeight(CardInfo ci, bool drawScrollButtons) {
			int height = CardBorderSize.Height * 2 + CalcCardCaptionHeight(ci);
			if(drawScrollButtons) height += CardScrollButtonHeight * 2;
			return height;
		}
		protected int CalcCardDataHeight(CardInfo ci) {
			if(View.GetCardCollapsed(ci.RowHandle)) return 0;
			int height = 0;
			CardFieldInfo fi = new CardFieldInfo(PaintAppearance.FieldCaption, PaintAppearance.FieldValue, ci, null);
			fi.Bounds = fi.Card.ClientBounds;
			CalcCardFieldRects(fi);
			for(int n = 0; n < View.VisibleColumns.Count; n++) {
				fi.Column = View.VisibleColumns[n];
				if(!CanShowField(fi)) continue;
				fi.Editor = View.GetRowCellRepositoryItem(ci.RowHandle, fi.Column);
				if(fi.Editor != null) {
					CreateEditViewInfo(fi);
					height += CalcFieldHeight(fi);
					fi.EditViewInfo = null;
				}
			}
			fi.Dispose();
			return height;
		}
		protected virtual void CreateEditViewInfo(CardFieldInfo fi) {
			if(fi.Editor == null) return;
			BaseEditViewInfo viewInfo = fi.Editor.CreateViewInfo();
			viewInfo.RightToLeft = View.IsRightToLeft; 
			UpdateEditViewInfo(viewInfo);
			viewInfo.Tag = fi.Card;
			fi.EditViewInfo = viewInfo;
		}
		protected virtual bool CanShowField(CardFieldInfo fi) {
			if(View.OptionsView.ShowEmptyFields) return true;
			object fieldValue = View.GetRowCellValue(fi.Card.RowHandle, fi.Column);
			return fieldValue != null && fieldValue != DBNull.Value; 
		}
		protected void CalcCardInfo(CardInfo ci, int maxHeight, bool drawScrollButtons) {
			CalcCardRects(ci);
			Rectangle r = ci.ClientBounds;
			ci.CaptionInfo.UpdateCardCaptionBounds(Rectangle.Inflate(ci.Bounds, -CardBorderSize.Width, -CardBorderSize.Height));
			r.Height -= ci.CaptionInfo.Bounds.Height;
			r.Y += ci.CaptionInfo.Bounds.Height;
			ci.ClientBounds = r;
			if(View.GetCardCollapsed(ci.RowHandle)) return;
			int nonDataHeight = CalcCardNonDataHeight(ci, drawScrollButtons);
			int lastFieldTop = r.Top;
			if(drawScrollButtons) {
				ci.DownButtonBounds = ci.UpButtonBounds = r;
				ci.DownButtonBounds.Height = ci.UpButtonBounds.Height = CardScrollButtonHeight;
				lastFieldTop += ci.UpButtonBounds.Height;
			}
			maxHeight -= nonDataHeight;
			int fieldsHeight = CalcCardFieldsInfo(ci, lastFieldTop, maxHeight, ref ci.CanScrollDown);
			if(drawScrollButtons) {
				ci.DownButtonBounds.Y = lastFieldTop + fieldsHeight;
			}
			ci.Bounds.Height = fieldsHeight + nonDataHeight;
			ci.ClientBounds.Height = (ci.Bounds.Bottom - CardBorderSize.Height) - ci.ClientBounds.Top;
		}
		protected virtual bool ShouldProcessCard(CardInfo info) {
			if(View.OptionsView.GetAnimationType() == GridAnimationType.AnimateFocusedItem && info.RowHandle != View.FocusedRowHandle) return false;
			if(info.VisibleIndex < 0 || info.Fields.Count == 0) return false;
			return true;
		}
		protected override BaseEditViewInfo HasItem(CellId id) {
			if(id == null) return null;
			for(int i = 0; i < Cards.Count; i++) {
				if(!ShouldProcessCard(Cards[i])) continue;
				for(int j = 0; j < Cards[i].Fields.Count; j++) {
					CardFieldId fieldId = new CardFieldId(View, Cards[i].Fields[j]);
					if(Cards[i].Fields[j].EditViewInfo != null && id == fieldId) return Cards[i].Fields[j].EditViewInfo;
				}
			}
			return null;
		}
		protected override void AddAnimatedItems() {
			if(View.OptionsView.GetAnimationType() == GridAnimationType.NeverAnimate) return;
			for(int i = 0; i < Cards.Count; i++) {
				if(!ShouldProcessCard(Cards[i])) continue;
				AddAnimatedItems(Cards[i]);
			}
		}
		protected internal virtual void AddAnimatedItems(CardInfo info) {
			IAnimatedItem item;
			for(int j = 0; j < info.Fields.Count; j++) {
				item = info.Fields[j].EditViewInfo as IAnimatedItem;
				CardFieldId id = new CardFieldId(View, info.Fields[j]);
				if(item != null && ShouldAddItem(info.Fields[j].EditViewInfo, id))
					AddAnimatedItem(id, info.Fields[j].EditViewInfo);
			}
		}
		protected override bool ShouldStopAnimation(IAnimatedItem item) {
			if(View.OptionsView.GetAnimationType() == GridAnimationType.NeverAnimate) return true;
			BaseEditViewInfo vi = item as BaseEditViewInfo;
			if(vi == null) return false;
			CardInfo info = vi.Tag as CardInfo;
			if(info.RowHandle != View.FocusedRowHandle && View.OptionsView.GetAnimationType() == GridAnimationType.AnimateFocusedItem) return true;
			return false;
		}
		protected internal int CalcCardFieldsInfo(CardInfo ci, int lastFieldTop, int maxDetailHeight, ref bool canScrollDown) {
			UpdateRowConditionAndFormat(ci.RowHandle, ci);
			int height = 0, startColumn = 0;
			if(maxDetailHeight != 9999 && ci.RowHandle == View.FocusedRowHandle) { 
				int topIndex =View.FocusedCardTopFieldIndex; 
				List<int> res = GetVisibleColumnsIndexes(View.FocusedRowHandle);
				if(topIndex < res.Count) startColumn = res[topIndex];
			}
			for(int n = startColumn; n < View.VisibleColumns.Count; n++) {
				CardFieldInfo fi = new CardFieldInfo(PaintAppearance.FieldCaption, PaintAppearance.FieldValue, ci, View.VisibleColumns[n]);
				if(!CanShowField(fi) && (maxDetailHeight != 9999 || !View.OptionsPrint.PrintEmptyFields)) continue;
				CalcCardFieldInfo(fi, lastFieldTop);
				height += fi.Bounds.Height;
				lastFieldTop = fi.Bounds.Bottom;
				if(height > maxDetailHeight && View.VertScrollVisibility != ScrollVisibility.Never) {
					height -= fi.Bounds.Height;
					canScrollDown = true;
					break;
				}
				ci.Fields.Add(fi);
			}
			return height;
		}
		protected int CalcCardX(int horzIndex) {
			return horzIndex * (CardWidth + CardInterval + SeparatorLineWidth);
		}
		protected internal int CalcFullyVisibleCardsCount() {
			int maxIndex = CalcMaxCardHorzIndex();
			if(maxIndex < 0) return 0;
			int count = CalcRowsInColumn(maxIndex);
			if(count == 0) return 0;
			if(CalcCardX(maxIndex + 1) > ViewRects.Cards.Right) {
				return Cards.Count - count;
			}
			return Cards.Count;
		}
		protected internal int CalcRowsInColumn(int horzIndex) {
			int res = 0;
			foreach(CardInfo ci in Cards) {
				if(ci.Position.X == horzIndex) res ++;
			}
			return res;
		}
		protected internal int CalcMaxCardHorzIndex() { 
			int result = -1;
			foreach(CardInfo ci in Cards) {
				result = Math.Max(ci.Position.X, result);
			}
			return result;
		}
		protected void CalcSeparatorsInfo() {
			if(Cards.Count == 0) return;
			int maxHorzIndex = CalcMaxCardHorzIndex();
			Rectangle r = ViewRects.Cards;
			int lineWidth = SeparatorLineWidth;
			if(lineWidth == 0) return;
			int horzIndex = 0;
			for(;;) {
				bool needBreak = horzIndex == maxHorzIndex;
				r.X = ViewRects.Cards.Left + CalcCardX(horzIndex + 1) - CardInterval / 2 - lineWidth;
				if(r.X > ViewRects.Cards.Right) break;
				if(IsRightToLeft)
					r.X = ViewRects.Cards.Width - r.X;
				r.Width = lineWidth;
				Indents.Add(new IndentInfo(null, r, PaintAppearance.SeparatorLine));
				if(needBreak) break;
				horzIndex ++;
			}
		}
		protected void CalcCardsDrawInfo() {
			if(this.loadVisibleRows) 
				this.visibleRows = LoadVisibleRows(View.TopLeftCardIndex);
			int maxHorzCards = View.MaximumCardColumns == -1 && View.OptionsBehavior.AutoHorzWidth ? 1 : View.MaximumCardColumns;
			int horzIndex = 0, vertIndex = 0;
			int topCoord = ViewRects.Cards.Top,
				leftCoord = ViewRects.Cards.Left;
			int directionDelta = IsRightToLeft ? -1 : 1;
			if(IsRightToLeft) {
				leftCoord = ViewRects.Cards.Right;
			}
			for(int n = 0; n < VisibleRows.Count; n++) {
				bool bCanVertExit = View.MaximumCardRows != -1 && View.MaximumCardRows <= vertIndex, drawScrollButtons = false;
				GridRow row = VisibleRows[n];
				CardInfo ci = new CardInfo(this);
				ci.Bounds = new Rectangle(leftCoord + directionDelta * CalcCardX(horzIndex) + (IsRightToLeft ? -CardWidth : 0), topCoord, CardWidth, 100);
				CalcCardRects(ci);
				ci.RowHandle = row.RowHandle;
				ci.VisibleIndex = row.VisibleIndex;
				int maxCardHeight = ViewRects.Cards.Bottom - topCoord;
				Size size = new Size(ci.Bounds.Width, CalcCardTotalHeight(ci, View.VertScrollVisibility == ScrollVisibility.Always));
				if(vertIndex > 0 && (bCanVertExit || (topCoord + size.Height > ViewRects.Cards.Bottom))) {
					horzIndex ++;
					vertIndex = 0;
					topCoord = ViewRects.Cards.Top;
					int nextLeft = leftCoord + directionDelta * CalcCardX(horzIndex) + (IsRightToLeft ? -CardWidth : 0);
					if(IsRightToLeft) {
						if(nextLeft < ViewRects.Cards.Left) break;
					}
					else {
						if(nextLeft > ViewRects.Cards.Right) break;
					}
					bCanVertExit = true;
				}
				if(!AllowPartitalVisibility) {
					if(vertIndex == 0 && horzIndex > 0) {
						int nextLeft = leftCoord + directionDelta * CalcCardX(horzIndex + 1) + (IsRightToLeft ? -CardWidth : 0);
						if(IsRightToLeft) {
							if(nextLeft < ViewRects.Cards.Left) break;
						}
						else {
							if(nextLeft > ViewRects.Cards.Right) break;
						}
					}
				}
				bool bCanHorzExit = (maxHorzCards != -1 && horzIndex == maxHorzCards);
				if(bCanHorzExit) break;
				ci.Bounds = new Rectangle(leftCoord + directionDelta * CalcCardX(horzIndex) + (IsRightToLeft ? -CardWidth : 0), topCoord, CardWidth, size.Height);
				ci.Position = new Point(horzIndex, vertIndex);
				maxCardHeight = ViewRects.Cards.Bottom - topCoord;
				if(View.VertScrollVisibility != ScrollVisibility.Never) {
					drawScrollButtons = true;
					if(View.VertScrollVisibility == ScrollVisibility.Auto) {
						drawScrollButtons = ((ci.RowHandle == View.FocusedRowHandle && View.FocusedCardTopFieldIndex > 0) || maxCardHeight < size.Height);
					}
				} else
					drawScrollButtons = false;
				CalcCardInfo(ci, maxCardHeight, drawScrollButtons);
				Cards.Add(ci);
				vertIndex++;
				topCoord += size.Height + CardInterval;
			}
		}
		protected int CalcCardWidth() {
			int result = ScaleHorizontal(View.CardWidth);
			if(View.OptionsBehavior.AutoHorzWidth) {
				int mr = View.MaximumCardColumns;
				if(View.RowCount < mr) mr = View.RowCount;
				if(mr < 1) mr = 1;
				result = (ViewRects.Cards.Width - CardInterval) / mr - (CardInterval + SeparatorLineWidth);
			}
			return result;
		}
		public override void PrepareCalcRealViewHeight(Rectangle viewRect, BaseViewInfo oldViewInfo) {
			if(oldViewInfo != null) {
				this.IsReady = oldViewInfo.IsReady;
			}
			CalcViewRects(viewRect);
		}
		public override int CalcRealViewHeight(Rectangle viewRect) {
			int result = viewRect.Height;
			int minBottom = -1;
			StartRealHeightCalculate();
			try {
				Calc(null, viewRect);
				foreach(CardInfo ci in Cards) {
					minBottom = Math.Max(ci.Bounds.Bottom, minBottom);
				}
				if(minBottom == -1) {
					result = ViewRects.Bounds.Height - ViewRects.Cards.Height;
				}
				else {
					result = ViewRects.Bounds.Height - ViewRects.Cards.Height + (minBottom - ViewRects.Cards.Top) + CardInterval;
				}
			}
			finally {
				EndRealHeightCalculate();
			}
			return result;
		}
		protected internal virtual void CalcConstants() {
			CardScrollButton.Appearance.Assign(PaintAppearance.CardButton);
			CardScrollButtonHeight = Math.Max(12, ScrollUpPainter.CalcObjectMinBounds(new EditorButtonObjectInfoArgs(GInfo.Cache, CardScrollButton, null)).Height);
			CardFieldCaptionWidth = CalcCardFieldCaptionWidth();
			CardWidth = CalcCardWidth(); 
			CardExpandButtonSize = CalcCardExpandButtonSize();
		}
		public CardElementsPainter ElementsPainter { get { return View.Painter == null ? null : (View.Painter as CardPainter).ElementsPainter; } }
		protected virtual Size CalcCardExpandButtonSize() {
			if(!View.OptionsView.ShowCardExpandButton) return Size.Empty;
			return CalcObjectSize(ElementsPainter.CardExpandButton, new ExplorerBarOpenCloseButtonInfoArgs(null, Rectangle.Empty, null, ObjectState.Normal, true));
		}
		public override void Calc(Graphics g, Rectangle bounds) {
			if(IsNull) return;
			base.CalcViewInfo();
			CreateButtons();
			UpdateButtonsText();
			ViewRects.Clear();
			GInfo.AddGraphics(g);
			try {
				Clear();
				CalcViewRects(bounds);
				CalcConstants();
				UpdateTabControl();
				CalcFilterDrawInfo();
				CalcCardsDrawInfo();
				CalcSeparatorsInfo();
				IsReady = true;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		bool loadVisibleRows = true;
		bool allowPartitalVisibility = true;
		protected bool AllowPartitalVisibility { get { return allowPartitalVisibility; } }
		public virtual int CalcBestTopCard(int topRowIndex) {
			if(topRowIndex < View.RowCount - MaxLoadRows) return topRowIndex;
			int newTopRowIndex = topRowIndex;
			Graphics g = GInfo.AddGraphics(null);
			try {
				this.allowPartitalVisibility = false;
				this.loadVisibleRows = false;
				this.visibleRows = new GridRowCollection();
				Calc(null, View.ViewRect);
				this.visibleRows = LoadVisibleRows(View.RowCount - 1, true);
				Cards.Clear();
				CalcCardsDrawInfo();
				CardInfo card = Cards.Count == 0 ? null : Cards[Cards.Count - 1];
				if(card != null) {
					int index = View.GetVisibleIndex(card.RowHandle);
					if(index >= 0) newTopRowIndex = index;
				}
			} finally {
				GInfo.ReleaseGraphics();
			}
			return Math.Min(topRowIndex, newTopRowIndex);;
		}
		protected override void UpdateTabControl() {
			if(IsRealHeightCalculate) return;
			if(!ShowTabControl) {
				TabControl.Bounds = Rectangle.Empty;
				return;
			} 
			TabControl.Bounds = CalcBorderRect(ViewRects.Bounds);
		}
		const int QuickCustomizeButtonIndent = 10;
		protected virtual void CalcViewRects(Rectangle viewRect) {
			FilterPanel.Bounds = Rectangle.Empty;
			ViewRects.Bounds = viewRect;
			Rectangle client = CalcClientRect();
			ViewRects.Clear();
			ViewRects.Bounds = viewRect;
			if(View.IsShowFilterPanel) {
				int height = GetFilterPanelHeight();
				if(client.Height > height) {
					FilterPanel.Bounds = new Rectangle(client.X, client.Bottom - height, client.Width, height);
					client.Height -= height;
				}
			}
			if(View.OptionsView.ShowViewCaption) {
				int height = CalcViewCaptionHeight(client);
				ViewRects.ViewCaption = new Rectangle(client.X, client.Y, client.Width, height);
				client.Y += height;
				client.Height -= height;
				if(client.Height < 0) client.Height = 0;
			}
			client = UpdateFindControlVisibility(client, true);
			ViewRects.Client = client;
			Rectangle r = Rectangle.Inflate(ViewRects.Client, -CardInterval, -CardInterval);
			if(IsRightToLeft) {
				r.X -= CardInterval;
			}
			r.Width += CardInterval;
			r.Height += CardInterval;
			ViewRects.Cards = r;
			CalcButtonsRects();
		}
		protected virtual void CalcButtonsRects() {
			bool updateCardsBounds = false, 
				showButtons = View.OptionsView.ShowQuickCustomizeButton | (View.IsZoomedView || View.IsDetailView);
			if(!showButtons) return;
			Rectangle client = ViewRects.Client;
			Size quick = CalcObjectSize(ElementsPainter.Buttons, QuickCustomizeButton),
				zoom = CalcObjectSize(ElementsPainter.Buttons, CloseZoomButton);
			int maxHeight = Math.Max(quick.Height, zoom.Height);
			Rectangle buttons = new Rectangle(client.X + QuickCustomizeButtonIndent, client.Y + QuickCustomizeButtonIndent / 2,
											  client.Width - QuickCustomizeButtonIndent  - 2, maxHeight);
			if(client.Height < buttons.Height + QuickCustomizeButtonIndent / 2) return;
			if((View.IsZoomedView || (View.IsDetailView && View.ParentView != null && View.ParentView.IsAllowZoomDetail))) {
				if(buttons.Width > zoom.Width + QuickCustomizeButtonIndent) {
					ViewRects.CloseZoom = new Rectangle(buttons.Right - zoom.Width, buttons.Y, zoom.Width, buttons.Height);
					buttons.Width -= zoom.Width + QuickCustomizeButtonIndent;
					updateCardsBounds = true;
				}
			}
			if(View.OptionsView.ShowQuickCustomizeButton) {
				if(buttons.Width > quick.Width + QuickCustomizeButtonIndent) {
					if(IsRightToLeft)
						ViewRects.QuickCustomize = new Rectangle(buttons.Right - quick.Width, buttons.Y, quick.Width, buttons.Height);
					else
					ViewRects.QuickCustomize = new Rectangle(buttons.X, buttons.Y, quick.Width, buttons.Height);
					updateCardsBounds = true;
				}
			}
			if(updateCardsBounds) {
				Rectangle r = ViewRects.Cards;
				r.Height -= buttons.Height;
				r.Y += buttons.Height;
				ViewRects.Cards = r;
			}
		}
		protected int CalcCardFieldCaptionWidth() {
			if(!View.OptionsView.ShowFieldCaptions) return 0;
			return CalcCardFieldCaptionWidth(PaintAppearance.FieldCaption);
		}
		protected internal virtual void UpdateRowIndexes(int newTopRowIndex) {
			for(int n = 0; n < Cards.Count; n++) {
				CardInfo card = Cards[n];
				if(card.IsSpecialCard) continue;
				card.VisibleIndex = newTopRowIndex ++;
				card.RowHandle = View.GetVisibleRowHandle(card.VisibleIndex);
			}
		}
		protected internal int CalcCardFieldCaptionWidth(AppearanceObject captionAppearance) {
			int result = 0;
			Graphics g = GInfo.AddGraphics(null);
			try {
				foreach(GridColumn column in View.VisibleColumns) {
					using(FrozenAppearance measureAppearance = new FrozenAppearance()) {
						AppearanceHelper.Combine(measureAppearance, new AppearanceObject[] { column.AppearanceHeader, captionAppearance });
						result = Math.Max(Convert.ToInt32(measureAppearance.CalcTextSize(g,
							CardFieldInfo.GetFieldCaption(column), 0).Width), result);
					}
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return result + 4; 
		}
		protected Rectangle CalcClientRect() {
			Rectangle clBounds = CalcScrollRect();
			if(View.IsNeededHScrollBar && !View.ScrollInfo.IsOverlapScrollBar)
				clBounds.Height = Math.Max(0, clBounds.Height - View.ScrollInfo.HScrollSize);
			if(View.IsNeededVScrollBar && !View.ScrollInfo.IsOverlapScrollBar)
				clBounds.Width = Math.Max(0, clBounds.Width - View.ScrollInfo.VScrollSize);
			return clBounds;
		}
		protected virtual Rectangle CalcTabClientRect() {
			Rectangle r = CalcTabClientRect(CalcBorderRect(ViewRects.Bounds));
			return r;
		}
		public virtual Rectangle CalcScrollRect() {
			return CalcTabClientRect();
		}
		protected internal GridRowCollection LoadVisibleRows(int topRowIndex) { return LoadVisibleRows(topRowIndex, false); }
		protected virtual GridRowCollection LoadVisibleRows(int topRowIndex, bool reversive) {
			int rowCount = 0;
			GridRowCollection rows = new GridRowCollection();
			if(View.GetVisibleRowHandle(topRowIndex) == DevExpress.Data.DataController.InvalidRow)
				topRowIndex = DevExpress.Data.DataController.InvalidRow;
			while(topRowIndex != DevExpress.Data.DataController.InvalidRow) { 
				int rowHandle = View.GetVisibleRowHandle(topRowIndex);
				rows.Add(rowHandle, topRowIndex, 0, 0, View.GetRowKey(rowHandle), false);
				rowCount ++;
				if(reversive) 
					topRowIndex = View.GetPrevVisibleRow(topRowIndex);
				else
					topRowIndex = View.GetNextVisibleRow(topRowIndex);
				if(rowCount > MaxLoadRows) break;
			}
			return rows;
		}
		#region CalcHitInfo
		public virtual CardHitInfo CalcHitInfo(Point pt) {
			CardHitInfo hi = new CardHitInfo();
			hi.View = View;
			hi.HitPoint = pt;
			if(!IsReady || IsDataDirty) return hi;
			if(CheckMasterTabHitTest(hi, pt)) {
				hi.HitTest = CardHitTest.MasterTabPageHeader;
				return hi;
			}
			if(hi.CheckAndSetHitTest(ViewRects.ViewCaption, pt, CardHitTest.ViewCaption)) return hi;
			if(hi.CheckAndSetHitTest(ViewRects.QuickCustomize, pt, CardHitTest.QuickCustomizeButton)) return hi;
			if(hi.CheckAndSetHitTest(ViewRects.CloseZoom, pt, CardHitTest.CloseZoomButton)) return hi;
			if(hi.CheckAndSetHitTest(FilterPanel.Bounds, pt, CardHitTest.FilterPanel)) {
				hi.CheckAndSetHitTest(FilterPanel.CloseButtonInfo.Bounds, pt, CardHitTest.FilterPanelCloseButton);
				hi.CheckAndSetHitTest(FilterPanel.MRUButtonInfo.Bounds, pt, CardHitTest.FilterPanelMRUButton);
				hi.CheckAndSetHitTest(FilterPanel.CustomizeButtonInfo.Bounds, pt, CardHitTest.FilterPanelCustomizeButton);
				hi.CheckAndSetHitTest(FilterPanel.ActiveButtonInfo.Bounds, pt, CardHitTest.FilterPanelActiveButton);
				hi.CheckAndSetHitTest(FilterPanel.TextBounds, pt, CardHitTest.FilterPanelText);
				return hi;
			}
			if(GridDrawing.PtInRect(ViewRects.Cards, pt)) {
				Rectangle r;
				foreach(CardInfo ci in Cards) {
					r = ci.Bounds;
					if(hi.CheckAndSetHitTest(r, pt, CardHitTest.Card)) {
						hi.RowHandle = ci.RowHandle;
						if(hi.CheckAndSetHitTest(ci.CaptionInfo.ErrorIconBounds, pt, CardHitTest.CardCaptionErrorIcon)) return hi;
						if(hi.CheckAndSetHitTest(ci.CaptionInfo.Bounds, pt, CardHitTest.CardCaption)) {
							hi.CheckAndSetHitTest(ci.CaptionInfo.ExpandButtonBounds, pt, CardHitTest.CardExpandButton);
							return hi;
						}
						if(hi.CheckAndSetHitTest(ci.UpButtonBounds, pt, CardHitTest.CardUpButton)) return hi;
						if(hi.CheckAndSetHitTest(ci.DownButtonBounds, pt, CardHitTest.CardDownButton)) return hi;
						if(GridDrawing.PtInRect(ci.ClientBounds, pt)) {
							foreach(CardFieldInfo fi in ci.Fields) {
								if(GridDrawing.PtInRect(fi.Bounds, pt)) {
									hi.Column = fi.Column;
									hi.HitTest = CardHitTest.Field;
									hi.CheckAndSetHitTest(fi.CaptionBounds, pt, CardHitTest.FieldCaption);
									hi.CheckAndSetHitTest(fi.ValueBounds, pt, CardHitTest.FieldValue);
									return hi;
								}
							}
						}
						return hi;
					}
					if(SeparatorLineWidth > 0) {
						r.Y = ViewRects.Cards.Top + CardInterval;
						r.X = r.Right + CardInterval / 2 - 1;
						r.Width = SeparatorLineWidth + 1;
						r.Height = ViewRects.Cards.Height - CardInterval;
						if(hi.CheckAndSetHitTest(r, pt, CardHitTest.Separator)) return hi;
					}
				}
			}
			return hi;
		}
		#endregion
	}
	#region HitTest
	public enum  CardHitTest { None, Card, CardCaption, CardExpandButton, Field, FieldCaption, FieldValue, Separator, CardUpButton, 
		CardDownButton, CardCaptionErrorIcon, QuickCustomizeButton, CloseZoomButton, FilterPanel,
		FilterPanelCloseButton, FilterPanelActiveButton, FilterPanelText, FilterPanelMRUButton, FilterPanelCustomizeButton, ViewCaption, MasterTabPageHeader
	};
	public class CardHitInfo : BaseHitInfo {
		static CardHitInfo emptyInfo;
		internal static CardHitInfo EmptyInfo {
			get { 
				if(emptyInfo == null) emptyInfo = new CardHitInfo();
				return emptyInfo;
			}
		}
		GridColumn column;
		int rowHandle;
		CardHitTest hitTest;
		public CardHitInfo() {
			Clear();
		}
		protected bool ContainsPoint(Rectangle bounds, Point p) {
			return !bounds.IsEmpty && bounds.Contains(p);
		}
		protected internal bool CheckAndSetHitTest(Rectangle bounds, Point p, CardHitTest hitTest) {
			if(ContainsPoint(bounds, p)) {
				this.hitTest = hitTest;
				return true;
			}
			return false;
		}
		public new CardView View { get { return (CardView)base.View; } set { base.View = value; } }
		public override void Clear() {
			base.Clear();
			this.column = null;
			this.rowHandle = DevExpress.Data.DataController.InvalidRow;
			this.hitTest = CardHitTest.None;
		}
		public GridColumn Column { get { return column; } set { column = value; } }
		public int RowHandle { get { return rowHandle; } set { rowHandle = value; } }
		public CardHitTest HitTest { get { return hitTest; } set { hitTest = value; } }
		public bool InField {
			get {
				return IsValid && (HitTest == CardHitTest.Field ||
					HitTest == CardHitTest.FieldCaption || 
					HitTest == CardHitTest.FieldValue);
			}
		}
		public virtual bool InFilterPanel {
			get { 
				return IsValid && (HitTest == CardHitTest.FilterPanel || HitTest == CardHitTest.FilterPanelActiveButton ||
				  HitTest == CardHitTest.FilterPanelCloseButton || HitTest == CardHitTest.FilterPanelMRUButton ||
					HitTest == CardHitTest.FilterPanelText || HitTest == CardHitTest.FilterPanelCustomizeButton);
			}
		}
		public bool InCard {
			get {
				return IsValid && !InFilterPanel && (HitTest != CardHitTest.None &&
					HitTest != CardHitTest.Separator);
			}
		}
		public bool InCardCaption {
			get {
				return IsValid && (
					HitTest == CardHitTest.CardCaption || HitTest == CardHitTest.CardCaptionErrorIcon ||
					HitTest == CardHitTest.CardExpandButton);
			}
		}
		internal bool InCardContent {
			get {
				return InCard && !InCardCaption;
			}
		}
		public bool InCardButtons {
			get {
				return IsValid && (HitTest == CardHitTest.CardUpButton ||
					HitTest == CardHitTest.CardDownButton);
			}
		}
		public virtual bool IsEquals(CardHitInfo e) {
			return this.RowHandle == e.RowHandle && this.HitTest == e.HitTest;
		}
		protected internal override int HitTestInt { get { return (int)HitTest; } }
	}
	#endregion
	public class CardViewRects {
		Rectangle bounds, client, cards, closeZoom, quickCustomize, viewCaption;
		public CardViewRects() {
			Clear();
		}
		public void Clear() {
			this.viewCaption = this.closeZoom = this.quickCustomize = this.bounds = this.client = this.cards = Rectangle.Empty;
		}
		public Rectangle ViewCaption { get { return viewCaption; } set { viewCaption = value; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle Client { get { return client; } set { client = value; } }
		public Rectangle Cards { get { return cards; } set { cards = value; } }
		public Rectangle CloseZoom { get { return closeZoom; } set { closeZoom = value; } }
		public Rectangle QuickCustomize { get { return quickCustomize; } set { quickCustomize = value; } }
	}
	#region collections
	public class CardFieldInfoCollection : CollectionBase {
		public CardFieldInfoCollection() {
		}
		public CardFieldInfo this[int index] { get { return List[index] as CardFieldInfo; } }
		public CardFieldInfo this[GridColumn column] { 
			get {
				foreach(CardFieldInfo cfi in this) {
					if(cfi.Column == column) return cfi;
				}
				return null;
			}
		}
		protected override void OnRemoveComplete(int position, object item) {
			CardFieldInfo fi = item as CardFieldInfo;
			fi.Dispose();
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
		}
		public int IndexOf(CardFieldInfo field) { return List.IndexOf(field); }
		public int Add(CardFieldInfo field) { return List.Add(field); }
		public CardFieldInfo FieldInfoByColumn(GridColumn column) {
			foreach(CardFieldInfo fi in this) {
				if(fi.Column == column) return fi;
			}
			return null;
		}
	}
	public class CardInfoCollection : CollectionBase {
		public CardInfo this[int index] { get { return List[index] as CardInfo; } }
		public int Add(CardInfo info) { return List.Add(info); }
		protected override void OnRemoveComplete(int position, object item) {
			CardInfo ci = item as CardInfo;
			ci.Dispose();
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
		}
		public int IndexOf(CardInfo card) { return List.IndexOf(card); }
		public CardInfo CardInfoByRow(int rowHandle) {
			foreach(CardInfo ci in this) {
				if(ci.RowHandle == rowHandle) return ci;
			}
			return null;
		}
		public CardInfo GetInfo(int x, int y) {
			foreach(CardInfo ci in this) {
				if(ci.Bounds.Contains(x, y)) return ci;
			}
			return null;
		}
		public CardFieldInfo CardFieldInfoBy(int rowHandle, GridColumn column) {
			CardInfo ci = CardInfoByRow(rowHandle);
			if(ci != null) return ci.Fields.FieldInfoByColumn(column);
			return null;
		}
	}
	#endregion
}
