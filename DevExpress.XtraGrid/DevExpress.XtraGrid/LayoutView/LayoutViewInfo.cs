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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Utils;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Layout.Drawing;
using DevExpress.XtraGrid.Views.Layout.Events;
using DevExpress.XtraGrid.Views.Layout.Frames;
using DevExpress.XtraGrid.Views.Layout.Modes;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraGrid.Views.Layout.ViewInfo {
	internal class ColumnNamesMapper : BaseVisitor {
		LayoutViewCard cardCore;
		public LayoutViewCard Card {
			get { return cardCore; }
		}
		IDictionary<string, LayoutViewField> fields;
		public ColumnNamesMapper(LayoutViewCard card) {
			this.cardCore = card;
			fields = new Dictionary<string, LayoutViewField>();
			Card.Accept(this);
		}
		public LayoutViewField this[string columnName] {
			get {
				LayoutViewField result;
				return fields.TryGetValue(columnName, out result) ? result : null;
			}
		}
		public override void Visit(BaseLayoutItem item) {
			LayoutViewField field = item as LayoutViewField;
			if(field != null) fields.Add(field.ColumnName, field);
		}
	}
	internal class ColumnNameFinder : BaseVisitor {
		bool found;
		string columnName;
		public ColumnNameFinder(string columnName) {
			this.columnName = columnName;
		}
		public LayoutViewField Result { get; private set; }
		public override void Visit(BaseLayoutItem item) {
			if(found) return;
			LayoutViewField field = item as LayoutViewField;
			if(field != null && field.ColumnName == columnName) {
				Result = field;
				found = true;
			}
		}
	}
	internal class ItemNameFinder : BaseVisitor {
		bool found;
		string itemName;
		public ItemNameFinder(string itemName) {
			this.itemName = itemName;
		}
		public BaseLayoutItem Result { get; private set; }
		public override void Visit(BaseLayoutItem item) {
			if(found) return;
			if(item.Name == itemName) {
				Result = item;
				found = true;
			}
		}
	}
	internal class FieldInfoResetter : BaseVisitor {
		public override void Visit(BaseLayoutItem item) {
			if(item is LayoutViewField) {
				LayoutViewFieldInfo fieldInfo = item.ViewInfo as LayoutViewFieldInfo;
				fieldInfo.IsReady = false;
				fieldInfo.IsViewInfoCalculated = false;
				fieldInfo.GridRowCellState = GridRowCellState.Dirty;
				fieldInfo.RepositoryItemViewInfo.IsReady = false;
			}
		}
	}
	public class LayoutViewRects {
		Rectangle bounds, clientRect, headerRect, cardsRect, footerRect, viewCaption;
		Rectangle singleModeButton, rowModeButton, columnModeButton;
		Rectangle multiRowModeButton, multiColumnModeButton;
		Rectangle carouselModeButton;
		Rectangle separator;
		Rectangle panButton;
		Rectangle customizeButton;
		Rectangle closeZoomButton;
		public LayoutViewRects() {
			ClearRects();
			ClearButtons();
		}
		public void ClearRects() {
			viewCaption = bounds = clientRect = headerRect = cardsRect = footerRect = Rectangle.Empty;
		}
		public bool IsRightToLeft { get; set; }
		public void ClearButtons() {
			singleModeButton = rowModeButton = columnModeButton = Rectangle.Empty;
			multiRowModeButton = multiColumnModeButton = Rectangle.Empty;
			carouselModeButton = Rectangle.Empty;
			separator = Rectangle.Empty;
			closeZoomButton = customizeButton = panButton = Rectangle.Empty;
		}
		public Rectangle ViewCaption { get { return viewCaption; } set { viewCaption = value; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle ClientRect { get { return clientRect; } set { clientRect = value; } }
		public Rectangle HeaderRect { get { return headerRect; } set { headerRect = value; } }
		public Rectangle CardsRect { get { return cardsRect; } set { cardsRect = value; } }
		public Rectangle FooterRect { get { return footerRect; } set { footerRect = value; } }
		public Rectangle SingleModeButton { get { return singleModeButton; } set { singleModeButton = value; } }
		public Rectangle RowModeButton { get { return rowModeButton; } set { rowModeButton = value; } }
		public Rectangle ColumnModeButton { get { return columnModeButton; } set { columnModeButton = value; } }
		public Rectangle MultiRowModeButton { get { return multiRowModeButton; } set { multiRowModeButton = value; } }
		public Rectangle MultiColumnModeButton { get { return multiColumnModeButton; } set { multiColumnModeButton = value; } }
		public Rectangle CarouselModeButton { get { return carouselModeButton; } set { carouselModeButton = value; } }
		public Rectangle Separator { get { return separator; } set { separator = value; } }
		public Rectangle PanButton { get { return panButton; } set { panButton = value; } }
		public Rectangle CustomizeButton { get { return customizeButton; } set { customizeButton = value; } }
		public Rectangle CloseZoomButton { get { return closeZoomButton; } set { closeZoomButton = value; } }
		internal void UpdateButtonsRTL(Rectangle buttonsRect) {
			if(!IsRightToLeft) return;
			LayoutViewRTLHelper.UpdateRTLCore(ref singleModeButton, buttonsRect);
			LayoutViewRTLHelper.UpdateRTLCore(ref rowModeButton, buttonsRect);
			LayoutViewRTLHelper.UpdateRTLCore(ref columnModeButton, buttonsRect);
			LayoutViewRTLHelper.UpdateRTLCore(ref multiRowModeButton, buttonsRect);
			LayoutViewRTLHelper.UpdateRTLCore(ref multiColumnModeButton, buttonsRect);
			LayoutViewRTLHelper.UpdateRTLCore(ref carouselModeButton, buttonsRect);
			LayoutViewRTLHelper.UpdateRTLCore(ref separator, buttonsRect);
			LayoutViewRTLHelper.UpdateRTLCore(ref panButton, buttonsRect);
			LayoutViewRTLHelper.UpdateRTLCore(ref customizeButton, buttonsRect);
			LayoutViewRTLHelper.UpdateRTLCore(ref closeZoomButton, buttonsRect);
		}
		internal int CheckRTL(int pos, Rectangle bounds) {
			if(IsRightToLeft)
				LayoutViewRTLHelper.UpdateRTLCore(ref pos, bounds);
			return pos;
		}
		internal Point CheckRTL(Point pt, Rectangle bounds) {
			if(IsRightToLeft)
				LayoutViewRTLHelper.UpdateRTLCore(ref pt, bounds);
			return pt;
		}
		internal Rectangle CheckRTL(Rectangle r, Rectangle bounds) {
			if(IsRightToLeft)
				LayoutViewRTLHelper.UpdateRTLCore(ref r, bounds);
			return r;
		}
	}
	public interface ILayoutViewInfoOwner {
		LayoutViewCard CloneCardFromTemplate();
		int VisibleRecordIndex { get; set; }
		int RecordCount { get; }
	}
	public interface ILayoutViewInfo {
		ILayoutViewInfoOwner Owner { get; }
		LayoutViewMode ViewMode { get; }
		bool NavigationHScrollNeed { get; }
		bool NavigationVScrollNeed { get; }
		bool CanCardAreaPan { get; }
		CardCollapsingMode CardCollapsingMode { get; }
		bool NeedArrangeForce { get; set; }
		void UpdateView();
		LayoutViewCard InitializeCard(int recordIndex);
		List<LayoutViewCard> VisibleCards { get; }
		LayoutViewRects ViewRects { get; }
	}
	public enum CardCollapsingMode { Vertical, Horisontal }
	public interface ILayoutViewManager : IDisposable {
		bool NavigationHScrollNeed { get; }
		bool NavigationVScrollNeed { get; }
		int GetScrollableCardsCount();
		int GetVSmallChange(bool forward);
		int GetHSmallChange(bool forward);
		int CheckMinScrollChange(int newPos, bool forward);
		bool CanCardAreaPan { get; }
		LayoutViewMode ViewMode { get; }
		CardCollapsingMode CardCollapsingMode { get; }
		Size CollapsedCardSize { get; }
		void Arrange(int currentRecordIndex);
		void Pan(int dx, int dy);
		List<LayoutViewCard> VisibleCards { get; }
		List<Line> CardLines { get; }
		List<Line> Lines { get; }
		bool CheckMinMaxSizeConstraints(LayoutViewCard card);
		void ResetCache();
		void OnGridLoadComplete();
		bool CanStretchCardByWidth { get; }
		int GetRealViewHeight();
		void ClearVisibleCards();
		bool CheckTouchScroll(int touchOffset, out int maxOffset);
		int CalcTouchScrollChange(int touchOffset);
	}
	public class LayoutViewDrawArgs : ViewDrawArgs {
		public LayoutViewDrawArgs(GraphicsCache graphicsCache, LayoutViewInfo viewInfo, Rectangle bounds) :
			base(graphicsCache, viewInfo, bounds) { }
		public virtual new LayoutViewInfo ViewInfo {
			get { return base.ViewInfo as LayoutViewInfo; }
		}
	}
	internal class NullLayoutViewInfo : LayoutViewInfo {
		public NullLayoutViewInfo(LayoutView view) : base(view) { }
		public override bool IsNull { get { return true; } }
	}
	public class LayoutViewInfo : ColumnViewInfo, ILayoutViewInfo {
		LayoutViewRects rects;
		Rectangle bounds = Rectangle.Empty, client = Rectangle.Empty;
		public LayoutViewInfo(LayoutView view)
			: base(view) {
			this.owner = view as ILayoutViewInfoOwner;
			this.layoutManager = CreateManager(view.OptionsView.ViewMode);
			this.rects = new LayoutViewRects();
			rects.CardsRect = new Rectangle(Point.Empty, View.CardMinSize);
		}
		#region ILayoutViewInfo
		ILayoutViewInfoOwner owner = null;
		internal ILayoutViewManager layoutManager = null;
		bool needUpdateForce = false;
		public bool NeedArrangeForce {
			get { return needUpdateForce; }
			set { needUpdateForce = value; }
		}
		public void OnGridLoadComplete() {
			if(layoutManager != null)
				layoutManager.OnGridLoadComplete();
		}
		protected virtual LayoutViewSingleRecordMode CreateLayoutViewSingleMode() {
			return new LayoutViewSingleRecordMode(this);
		}
		protected virtual LayoutViewRowMode CreateLayoutViewRowMode() {
			return new LayoutViewRowMode(this);
		}
		protected virtual LayoutViewColumnMode CreateLayoutViewColumnMode() {
			return new LayoutViewColumnMode(this);
		}
		protected virtual LayoutViewMultiRowMode CreateLayoutViewMultiRowMode() {
			return new LayoutViewMultiRowMode(this);
		}
		protected virtual LayoutViewMultiColumnMode CreateLayoutViewMultiColumnMode() {
			return new LayoutViewMultiColumnMode(this);
		}
		protected virtual LayoutViewCarouselMode CreateLayoutViewCarouselMode() {
			return new LayoutViewCarouselMode(this);
		}
		public ILayoutViewInfoOwner Owner {
			get { return owner; }
		}
		public LayoutViewMode ViewMode {
			get { return layoutManager.ViewMode; }
		}
		public List<Line> GetLines() {
			List<Line> lines = new List<Line>();
			lines.AddRange(layoutManager.CardLines);
			lines.AddRange(layoutManager.Lines);
			return lines;
		}
		public bool CanCardAreaPan {
			get { return layoutManager.CanCardAreaPan; }
		}
		public bool CanCustomize {
			get { return true; }
		}
		protected virtual void CheckLayoutManager(LayoutViewMode value) {
			if(layoutManager == null || layoutManager.ViewMode != value) {
				using(new WaitCursor()) {
					if(layoutManager != null)
						layoutManager.Dispose();
					layoutManager = CreateManager(value);
					if(!IsRealHeightCalculate)
						Calc(GInfo.Graphics, View.ViewRect);
				}
			}
		}
		protected ILayoutViewManager CreateManager(LayoutViewMode value) {
			switch(value) {
				case LayoutViewMode.SingleRecord: return CreateLayoutViewSingleMode();
				case LayoutViewMode.Row: return CreateLayoutViewRowMode();
				case LayoutViewMode.Column: return CreateLayoutViewColumnMode();
				case LayoutViewMode.MultiRow: return CreateLayoutViewMultiRowMode();
				case LayoutViewMode.MultiColumn: return CreateLayoutViewMultiColumnMode();
				case LayoutViewMode.Carousel: return CreateLayoutViewCarouselMode();
			}
			return CreateLayoutViewSingleMode();
		}
		public CardCollapsingMode CardCollapsingMode {
			get {
				if(layoutManager != null) {
					return layoutManager.CardCollapsingMode;
				} return CardCollapsingMode.Horisontal;
			}
		}
		public void PanCardArea(int dx, int dy) {
			layoutManager.Pan(dx, dy);
		}
		public LayoutViewCard InitializeCard(int recordIndex) {
			View.implementorCore.AllowManageDesignSurfaceComponents = false;
			int rowHandle = View.GetVisibleRowHandle(recordIndex);
			bool cardCollapced = View.GetCardCollapsed(rowHandle);
			LayoutViewCard card = GetCardFromCache(!cardCollapced);
			View.implementorCore.AllowManageDesignSurfaceComponents = true;
			if(card != null) {
				View.implementorCore.TextAlignManager.Reset();
				card.BeginUpdate();
				if(cardCollapced) {
					card.AllowChangeTextLocationOnExpanding = (CardCollapsingMode == CardCollapsingMode.Vertical);
					card.Expanded = false;
				}
				card.ExpandButtonVisible = View.CalcCardExpandButtonVisibility();
				card.RowHandle = rowHandle;
				card.ResetFormatInfo();
				UpdateRowConditionAndFormat(rowHandle, card);
				card.Accept(new ViewInfoResetHelper());
				card.AllowBorderColorBlending = View.CanAllowBorderColorBlending(card);				
				UpdateCardAppearance(card);
				bool fMeasuring = ViewRects.CardsRect.Width > 0 && !cardCollapced && layoutManager.CanStretchCardByWidth;
				if(fMeasuring) {
					card.EndUpdate();
					UpdateCardData(card);
					card.Accept(new BeginMeasureVisitor());
					card.Size = new Size(ViewRects.CardsRect.Width, card.Size.Height);
					card.BeginUpdate();
				}
				else UpdateCardData(card);
				if(fMeasuring) {
					card.Accept(new EndMeasureVisitor());
					card.UpdateResizerConstraints();
				}
				if(cardCollapced) {
					card.Size = CheckCollapsedCardSize(layoutManager.CollapsedCardSize, card.MinSize);
				}
				if(!cardCollapced && !View.IsDifferencesProcessingLocked) {
					LayoutViewCardDifferences diff = View.GetCardDifferences(card.RowHandle);
					if(diff == null) diff = CardDifferenceController.CreateTemplateDifferences(View.TemplateCard);
					View.RaiseCustomCardLayout(new LayoutViewCustomCardLayoutEventArgs(card.RowHandle, diff));
					CardDifferenceController.ApplyDifferences(card, diff);
				}
				card.EndUpdate();
				card.UpdateResizerConstraints();
				if(layoutManager.CheckMinMaxSizeConstraints(card)) {
					card.Update();
				}
			}
			return card;
		}
		protected virtual LayoutViewCard GetCardFromCache(bool expanded) {
			LayoutViewCard card = null;
			if(expanded) {
				if(View.ExpandedCardCache.Count > 0) card = View.ExpandedCardCache.Pop();
			}
			else {
				if(View.CollapsedCardCache.Count > 0) card = View.CollapsedCardCache.Pop();
			}
			if(card != null) {
				card.BeginUpdate();
				card.Owner = View;
				card.AllowChangeTextLocationOnExpanding = false;
				(View as ILayoutControl).AppearanceController.SetDefaultAppearanceDirty(card);
				(View as ILayoutControl).AppearanceController.SetAppearanceDirty(card);
				card.Assign(View.TemplateCard);
				card.EndUpdate();
			}
			else {
				card = Owner.CloneCardFromTemplate();
				card.CheckRTL(View.IsRightToLeft);
			}
			if(card != null)
				SetCardViewInfoDirty(card);
			return card;
		}
		protected Size CheckCollapsedCardSize(Size cardMinSize, Size proposedSize) {
			return new Size(Math.Max(cardMinSize.Width, proposedSize.Width), Math.Max(cardMinSize.Height, proposedSize.Height));
		}
		public void UpdateView() {
			CheckLayoutManager(View.OptionsView.ViewMode);
			UpdateCore();
		}
		protected virtual void UpdateCore() {
			layoutManager.Arrange(Owner.VisibleRecordIndex);
		}
		public List<LayoutViewCard> VisibleCards { get { return layoutManager.VisibleCards; } }
		int cardFieldCaptionMaxWidth;
		public int CardFieldCaptionMaxWidth {
			get { return cardFieldCaptionMaxWidth; }
		}
		protected internal virtual void CalcConstants() {
			cardFieldCaptionMaxWidth = CalcCardFieldCaptionMaxWidth();
		}
		protected internal bool IsVisible(int rowHandle) {
			foreach(LayoutViewCard card in VisibleCards) {
				if(card.RowHandle == rowHandle) return true;
			}
			return false;
		}
		protected internal bool IsPartiallyVisible(int rowHandle) {
			foreach(LayoutViewCard card in VisibleCards) {
				if(card.RowHandle == rowHandle) return card.IsPartiallyVisible;
			}
			return false;
		}
		int GetDistance(LayoutViewCard card1, LayoutViewCard card2) {
			return (card1.Position.X - card2.Position.X) * (card1.Position.X - card2.Position.X) +
					(card1.Position.Y - card2.Position.Y) * (card1.Position.Y - card2.Position.Y);
		}
		protected internal bool CheckTouchScroll(int touchOffset, out int maxOffset) {
			return layoutManager.CheckTouchScroll(touchOffset, out maxOffset);
		}
		protected internal int CalcTouchScrollChange(int touchOffset) {
			return layoutManager.CalcTouchScrollChange(touchOffset);
		}
		protected internal int GetNextCard(int rowHandle, bool horz) {
			LayoutViewCard currentCard = null;
			int minDistance = int.MaxValue;
			int result = int.MinValue;
			int currentCardIndex = 0;
			if(VisibleCards.Count > 1) {
				for(int i = 0; i < VisibleCards.Count; i++) {
					LayoutViewCard card = VisibleCards[i];
					if(currentCard == null) {
						if(card.RowHandle == rowHandle) {
							currentCard = card;
							currentCardIndex = i;
						}
						continue;
					}
					if(card.IsPartiallyVisible) continue;
					if(horz && card.VisibleColumn <= currentCard.VisibleColumn) continue;
					if(!horz && card.VisibleRow <= currentCard.VisibleRow) continue;
					int distance = GetDistance(card, currentCard);
					if(distance < minDistance) {
						minDistance = distance;
						result = card.RowHandle;
					}
				}
			}
			return (result != int.MinValue) ? result : rowHandle + 1;
		}
		protected internal int GetPrevCard(int rowHandle, bool horz) {
			LayoutViewCard currentCard = null;
			int minDistance = int.MaxValue;
			int result = int.MinValue;
			if(VisibleCards.Count > 1) {
				for(int i = VisibleCards.Count - 1; i >= 0; i--) {
					LayoutViewCard card = VisibleCards[i];
					if(currentCard == null) {
						if(card.RowHandle == rowHandle) {
							currentCard = card;
						}
						continue;
					}
					if(card.IsPartiallyVisible) continue;
					if(horz && card.VisibleColumn >= currentCard.VisibleColumn) continue;
					if(!horz && card.VisibleRow >= currentCard.VisibleRow) continue;
					int distance = GetDistance(card, currentCard);
					if(distance < minDistance) {
						minDistance = distance;
						result = card.RowHandle;
					}
				}
			}
			return (result != int.MinValue) ? result : rowHandle - 1;
		}
		protected internal int GetFirstCardRowHandle() {
			int cardsCount = VisibleCards.Count;
			if(cardsCount > 0) {
				return VisibleCards[0].RowHandle;
			}
			return GridControl.InvalidRowHandle;
		}
		protected internal int GetLastCardRowHandle() {
			int cardsCount = VisibleCards.Count;
			if(cardsCount > 0) {
				return VisibleCards[cardsCount - 1].RowHandle;
			}
			return GridControl.InvalidRowHandle;
		}
		protected internal int CalcHSmallChange(bool forward) {
			return layoutManager.GetHSmallChange(forward);
		}
		protected internal int CalcVSmallChange(bool forward) {
			return layoutManager.GetVSmallChange(forward);
		}
		protected internal int CalcScrollableCardsCount() {
			return layoutManager.GetScrollableCardsCount();
		}
		protected internal int CheckNewScrollPos(int newPos, bool forward) {
			return layoutManager.CheckMinScrollChange(newPos, forward);
		}
		protected internal virtual int CalcCardCaptionMaxTextHeight() {
			if(!View.OptionsView.ShowCardCaption) return 0;
			try {
				int iMaxTextHeight = 0;
				Graphics g = GInfo.AddGraphics(null);
				iMaxTextHeight = Math.Max(iMaxTextHeight, PaintAppearance.CardCaption.CalcDefaultTextSize(g).Height);
				iMaxTextHeight = Math.Max(iMaxTextHeight, PaintAppearance.FocusedCardCaption.CalcDefaultTextSize(g).Height);
				iMaxTextHeight = Math.Max(iMaxTextHeight, PaintAppearance.SelectedCardCaption.CalcDefaultTextSize(g).Height);
				iMaxTextHeight = Math.Max(iMaxTextHeight, PaintAppearance.HideSelectionCardCaption.CalcDefaultTextSize(g).Height);
				return iMaxTextHeight;
			}
			finally { GInfo.ReleaseGraphics(); }
		}
		protected virtual int CalcCardFieldCaptionMaxWidth() {
			if(View != null && View.PaintAppearance != null) {
				return CalcCardFieldCaptionMaxWidth(View.PaintAppearance.FieldCaption);
			}
			return 0;
		}
		protected virtual int CalcCardFieldCaptionMaxWidth(AppearanceObject captionAppearance) {
			int result = 0;
			Graphics g = GInfo.AddGraphics(null);
			try {
				foreach(GridColumn column in View.Columns) {
					result = Math.Max(Convert.ToInt32(captionAppearance.CalcTextSize(g, View.GetFieldCaption(column), 0).Width), result);
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return result;
		}
		protected virtual void UpdateFieldData(BaseEditViewInfo editViewInfo, int rowHandle, GridColumn column) {
			if(editViewInfo == null) return;
			object fieldValue = View.GetRowCellValue(rowHandle, column);
			string error;
			DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType;
			View.GetColumnError(rowHandle, column, out error, out errorType);
			editViewInfo.ErrorIconText = error;
			editViewInfo.ShowErrorIcon = editViewInfo.ErrorIconText != null && editViewInfo.ErrorIconText.Length > 0;
			if(editViewInfo.ShowErrorIcon) {
				editViewInfo.ErrorIcon = DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider.GetErrorIconInternal(errorType);
			}
			UpdateEditViewInfo(editViewInfo, column, rowHandle);
			editViewInfo.FillBackground = true;
			editViewInfo.EditValue = fieldValue;
			editViewInfo.SetDisplayText(View.RaiseCustomColumnDisplayText(rowHandle, column, editViewInfo.EditValue, editViewInfo.DisplayText, false));
		}
		protected virtual void UpdateCardData(LayoutViewCard card) {
			card.Text = View.GetCardCaption(card.RowHandle);
			card.State = CalcRowStateCore(card.RowHandle);
			if(Owner.RecordCount > 0 && card.Items.Count > 0 && card.Expanded) {
				ColumnNamesMapper mapper = null;
				foreach(GridColumn column in View.Columns) {
					LayoutViewField field = FindFieldItemByColumnName(card, column.Name, ref mapper);
					if(field == null) continue;
					AppearanceObjectEx conditionApp = card.GetConditionAppearance(column);
					UpdateField(card, field, card.RowHandle, column, conditionApp, card.State);
				}
				UpdateLabelItems(card);
			}
		}
		protected void UpdateLabelItems(LayoutViewCard card) {
			List<BaseLayoutItem> items = new FlatItemsList().GetItemsList(card);
			foreach(BaseLayoutItem item in items) {
				if(item is SimpleLabelItem)
					UpdateLabelItem(item);
			}
		}
		void UpdateLabelItem(BaseLayoutItem item) {
			using(FrozenAppearance appearanceLabel = new FrozenAppearance()) {
				AppearanceHelper.Combine(appearanceLabel,
						new AppearanceObject[] { item.AppearanceItemCaption, 
							CheckItemForeAppearance(item, View.Appearance.FieldCaption, PaintAppearance.FieldCaption) 
						}
					);
				item.PaintAppearanceItemCaption.AssignInternal(appearanceLabel);
			}
		}
		protected void UpdateField(LayoutViewCard card, LayoutViewField field, int rowHandle, GridColumn column, AppearanceObjectEx condition, GridRowCellState rowState) {
			using(new SafeBaseLayoutItemChanger(field)) {
				ResetFieldLabel(field, rowHandle, column);
				UpdateFieldRepositoryItem(field, rowHandle, column);
				RequestEditViewInfo(card, field, rowHandle, column);
				UpdateFieldAppearance(field, rowHandle, column, condition, rowState);
				UpdateFieldLabel(field, rowHandle, column);
			}
		}
		protected void UpdateFieldRepositoryItem(LayoutViewField field, int rowHandle, GridColumn column) {
			RepositoryItem gridRepositoryItem = View.GetRowCellRepositoryItem(rowHandle, column);
			bool fIsNew = (gridRepositoryItem != field.RepositoryItem);
			if(fIsNew)
				field.RepositoryItem = gridRepositoryItem;
			CreateEditorViewInfoIfNeed(field, column, rowHandle, fIsNew);
		}
		protected virtual void ResetFieldLabel(LayoutViewField field, int rowHandle, GridColumn column) {
			LayoutViewField templateField = View.FindCardFieldByColumn(View.TemplateCard, column);
			if(templateField == null) return;
			if(templateField.Image != field.Image) field.Image = templateField.Image;
			if(templateField.ImageIndex != field.ImageIndex) field.ImageIndex = templateField.ImageIndex;
			if(templateField.ImageAlignment != field.ImageAlignment) field.ImageAlignment = templateField.ImageAlignment;
			if(templateField.ImageToTextDistance != field.ImageToTextDistance) field.ImageToTextDistance = templateField.ImageToTextDistance;
		}
		protected virtual void UpdateFieldLabel(LayoutViewField field, int rowHandle, GridColumn column) {
			LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
			LayoutViewFieldCaptionImageEventArgs e = new LayoutViewFieldCaptionImageEventArgs(rowHandle, column, View.Images);
			e.Image = field.Image;
			e.ImageIndex = field.ImageIndex;
			e.ImageToTextDistance = field.ImageToTextDistance;
			e.ImageAlignment = field.ImageAlignment;
			View.RaiseCustomFieldCaptionImage(e);
			fieldInfo.UpdateLabelAppearanceInternal(e);
		}
		protected override void UpdateEditViewInfo(BaseEditViewInfo vi) {
			base.UpdateEditViewInfo(vi);
			vi.AllowTextToolTip = View.OptionsView.ShowFieldHints;
		}
		protected override void AddAnimatedItems() {
			if(View.OptionsView.GetAnimationType() == GridAnimationType.NeverAnimate) return;
			for(int i = 0; i < VisibleCards.Count; i++) {
				LayoutViewCard card = VisibleCards[i];
				if(!ShouldProcessCard(card)) continue;
				AddAnimatedItems(card);
			}
		}
		protected virtual bool ShouldProcessCard(LayoutViewCard card) {
			if(View.OptionsView.GetAnimationType() == GridAnimationType.AnimateFocusedItem && card.RowHandle != View.FocusedRowHandle) return false;
			if(card.VisibleIndex < 0 || View.Columns.Count == 0) return false;
			return true;
		}
		protected internal virtual void AddAnimatedItems(LayoutViewCard card) {
			IAnimatedItem item;
			ColumnNamesMapper mapper = null;
			object row = null;
			foreach(LayoutViewColumn column in View.Columns) {
				LayoutViewField field = FindFieldItemByColumnName(card, column.Name, ref mapper);
				if(field == null || field.IsHidden) continue;
				LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
				item = fieldInfo.RepositoryItemViewInfo as IAnimatedItem;
				if(row == null)
					row = View.GetRow(card.RowHandle);
				CellId id = new CellId(row, column);
				if(item != null && ShouldAddItem(fieldInfo.RepositoryItemViewInfo, id))
					AddAnimatedItem(id, fieldInfo.RepositoryItemViewInfo);
			}
		}
		protected override BaseEditViewInfo HasItem(CellId id) {
			if(id == null) return null;
			for(int i = 0; i < VisibleCards.Count; i++) {
				LayoutViewCard card = VisibleCards[i];
				if(!ShouldProcessCard(card)) continue;
				ColumnNamesMapper mapper = null;
				object row = null;
				foreach(LayoutViewColumn column in View.Columns) {
					LayoutViewField field = FindFieldItemByColumnName(card, column.Name, ref mapper);
					if(field == null) continue;
					LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
					if(row == null)
						row = View.GetRow(card.RowHandle);
					CellId fieldId = new CellId(row, column);
					if(fieldInfo.RepositoryItemViewInfo != null && id == fieldId) return fieldInfo.RepositoryItemViewInfo;
				}
			}
			return null;
		}
		protected override bool ShouldStopAnimation(IAnimatedItem item) {
			GridAnimationType animationType = View.OptionsView.GetAnimationType();
			if(animationType == GridAnimationType.NeverAnimate) return true;
			BaseEditViewInfo editorInfo = item as BaseEditViewInfo;
			if(editorInfo == null) return false;
			return (animationType == GridAnimationType.AnimateFocusedItem) && !object.Equals(editorInfo.Tag, View.FocusedRowHandle);
		}
		internal LayoutViewField FindFieldItemByColumnName(LayoutViewCard card, string columnName, ref ColumnNamesMapper mapper) {
			if(mapper == null || mapper.Card != card)
				mapper = new ColumnNamesMapper(card);
			return mapper[columnName];
		}
		internal LayoutViewField FindFieldItemByColumnName(LayoutViewCard card, string columnName) {
			ColumnNameFinder finder = new ColumnNameFinder(columnName);
			card.Accept(finder);
			return finder.Result;
		}
		protected internal virtual Size GetFieldButtonSize() {
			if(Painter == null) return new Size(17, 17);
			return Painter.ElementsPainter.SortFilterButtons.GetFieldButtonSize();
		}
		int calculatedHeaderHeight = 0;
		protected internal virtual void CalcViewRects(Rectangle bounds) {
			ViewRects.IsRightToLeft = IsRightToLeft;
			int iHeaderDefaultHeight = (Painter != null) ?
				Painter.ElementsPainter.HeaderPanel.GetHeaderHeight() :
				LayoutViewHeaderObjectPainter.DefaultHeaderHeight;
			if(ViewRects.Bounds != bounds) NeedArrangeForce = true;
			int iClientPadding = 0;
			int iScrollPadding = 0;
			int iVScrollSpace = 0;
			int iHScrollSpace = 0;
			calculatedHeaderHeight = 0;
			if(View.ScrollInfo != null) {
				iVScrollSpace = (View.ScrollInfo.HScrollRect.Height == 0) ? 0 : View.ScrollInfo.HScrollRect.Height + iScrollPadding;
				iHScrollSpace = (View.ScrollInfo.VScrollRect.Width == 0) ? 0 : View.ScrollInfo.VScrollRect.Width + iScrollPadding;
			}
			Rectangle clientRect = new Rectangle(
					bounds.Left + iClientPadding,
					bounds.Top + iClientPadding,
					bounds.Width - iClientPadding * 2,
					bounds.Height - (iClientPadding * 2 + iVScrollSpace)
				);
			clientRect.Width = Math.Max(0, clientRect.Width);
			clientRect.Height = Math.Max(0, clientRect.Height);
			int iHeaderHeight = View.OptionsView.ShowHeaderPanel ? iHeaderDefaultHeight : 0;
			int iFooterHeight = View.IsShowFilterPanel ? GetFilterPanelHeight() : 0;
			int iCaptionHeight = View.OptionsView.ShowViewCaption ? CalcViewCaptionHeight(clientRect) : 0;
			Rectangle captionRect = new Rectangle(clientRect.Left, clientRect.Top, clientRect.Width, iCaptionHeight);
			iCaptionHeight +=
				UpdateFindControlVisibility(new Rectangle(clientRect.Left, clientRect.Top + iCaptionHeight, clientRect.Width, clientRect.Height - iCaptionHeight), true).Y -
				(clientRect.Top + iCaptionHeight);
			if((clientRect.Height - iFooterHeight - iHeaderHeight - iCaptionHeight - 10) <= 0) {
				calculatedHeaderHeight = iHeaderHeight;
				iHeaderHeight = 0;
			}
			Rectangle headerRect = new Rectangle(clientRect.Left, clientRect.Top + iCaptionHeight, clientRect.Width, iHeaderHeight);
			Rectangle footerRect = new Rectangle(clientRect.Left, clientRect.Bottom - iFooterHeight, clientRect.Width, iFooterHeight);
			Rectangle cardsRect = new Rectangle(
				IsRightToLeft ? clientRect.Left + iHScrollSpace : client.Left, 
				headerRect.Bottom,
				clientRect.Width - iHScrollSpace,
				clientRect.Height - iFooterHeight - iHeaderHeight - iCaptionHeight);
			System.Windows.Forms.Padding padding = (Painter != null) ?
				Painter.ElementsPainter.HeaderPanel.GetPanelContentMargins() :
				LayoutViewHeaderObjectPainter.DefaultPanelContentMargins;
			Rectangle buttonsRect = new Rectangle(headerRect.Left + padding.Left, headerRect.Top + padding.Top, headerRect.Width - padding.Horizontal, headerRect.Height - padding.Vertical);
			ViewRects.ClearButtons();
			if(iHeaderHeight > 0 && !View.IsDesignMode)
				CalcButtonRects(buttonsRect);
			ViewRects.ClearRects();
			ViewRects.Bounds = bounds;
			ViewRects.ClientRect = clientRect;
			ViewRects.ViewCaption = captionRect;
			ViewRects.HeaderRect = headerRect;
			ViewRects.FooterRect = footerRect;
			ViewRects.CardsRect = cardsRect;
			FilterPanel.Bounds = footerRect;
		}
		protected virtual void CalcButtonRects(Rectangle buttonsRect) {
			Size buttonSize = (Painter != null) ?
				Painter.ElementsPainter.HeaderPanel.GetButtonSize() :
				LayoutViewHeaderObjectPainter.DefaultButtonSize;
			Stack<Rectangle> btnRects = new Stack<Rectangle>();
			for(int i = 8; i >= 0; i--) btnRects.Push(
					new Rectangle(buttonsRect.Location + new Size(i * (buttonSize.Width + 1), 0), buttonSize)
				);
			if(View.OptionsHeaderPanel.ShowSingleModeButton) ViewRects.SingleModeButton = btnRects.Pop();
			if(View.OptionsHeaderPanel.ShowRowModeButton) ViewRects.RowModeButton = btnRects.Pop();
			if(View.OptionsHeaderPanel.ShowColumnModeButton) ViewRects.ColumnModeButton = btnRects.Pop();
			if(View.OptionsHeaderPanel.ShowMultiRowModeButton) ViewRects.MultiRowModeButton = btnRects.Pop();
			if(View.OptionsHeaderPanel.ShowMultiColumnModeButton) ViewRects.MultiColumnModeButton = btnRects.Pop();
			if(View.OptionsHeaderPanel.ShowCarouselModeButton) ViewRects.CarouselModeButton = btnRects.Pop();
			if(btnRects.Peek().Left != buttonsRect.Left) {
				Rectangle separatorRect = btnRects.Pop();
				ViewRects.Separator = new Rectangle(separatorRect.Location + new Size(5, 3), new Size(6, buttonSize.Height - 6));
			}
			if(View.OptionsBehavior.AllowPanCards && View.OptionsHeaderPanel.ShowPanButton) ViewRects.PanButton = btnRects.Pop();
			if(View.OptionsBehavior.AllowRuntimeCustomization && View.OptionsHeaderPanel.ShowCustomizeButton) ViewRects.CustomizeButton = btnRects.Pop();
			btnRects.Clear();
			btnRects = null;
			int clearBtnLeftBound = buttonsRect.Right;
			if(View.IsDetailView) {
				Rectangle closeZoomRect = new Rectangle(buttonsRect.Location + new Size(buttonsRect.Width - buttonSize.Width, 1), buttonSize);
				ViewRects.CloseZoomButton = closeZoomRect;
				clearBtnLeftBound = closeZoomRect.Left;
			}
			if(ViewRects.CustomizeButton.Right > clearBtnLeftBound)
				ViewRects.ClearButtons();
			else
				ViewRects.UpdateButtonsRTL(buttonsRect);
		}
		public bool NavigationHScrollNeed { get { return layoutManager.NavigationHScrollNeed; } }
		public bool NavigationVScrollNeed { get { return layoutManager.NavigationVScrollNeed; } }
		#endregion ILayoutViewInfo
		protected internal virtual void CreateEditorViewInfoIfNeed(LayoutViewField field, GridColumn column, int rowHandle, bool createForce) {
			if(field == null || field.RepositoryItem == null || field.ViewInfo == null) return;
			LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
			if(fieldInfo.RepositoryItemViewInfo == null || createForce) 
				fieldInfo.RepositoryItemViewInfo = field.RepositoryItem.CreateViewInfo();
			fieldInfo.RepositoryItemViewInfo.RightToLeft = View.IsRightToLeft;
			UpdateEditViewInfo(fieldInfo.RepositoryItemViewInfo, column, rowHandle);
			fieldInfo.RepositoryItemViewInfo.Tag = rowHandle;
		}
		protected internal virtual void SetCardViewInfoDirty(LayoutViewCard card) {
			card.State = GridRowCellState.Dirty;
			card.Accept(new FieldInfoResetter());
		}
		protected virtual void RequestEditViewInfo(LayoutViewCard card, LayoutViewField field, int rowHandle, GridColumn column) {
			LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
			if(fieldInfo != null && !fieldInfo.IsReady) {
				BaseEditViewInfo editorInfo = fieldInfo.RepositoryItemViewInfo;
				card.FormatInfo.ApplyContextImage(GInfo.Cache, column, editorInfo.Bounds, rowHandle, editorInfo);
				UpdateFieldData(editorInfo, rowHandle, column);
				RecalcFieldEditViewInfo(editorInfo, editorInfo.Bounds, rowHandle);
				CellId id = new CellId(View.GetRow(rowHandle), column);
				if(ShouldAddItem(editorInfo, id))
					AddAnimatedItem(id, editorInfo);
				fieldInfo.IsReady = true;
			}
		}
		protected internal int SeparatorLineWidth {
			get { return (View.OptionsView.ShowCardLines ? 2 : 0); }
		}
		internal bool fViewInfoCalculation = false;
		int watchDog = 0;
		public override void Calc(Graphics g, Rectangle bounds) {
			if(IsNull) return;
			base.CalcViewInfo();
			this.bounds = bounds;
			this.client = CalcTabClientRect();
			GInfo.AddGraphics(g);
			try {
				ResetFormatInfo();
				Clear();
				View.UpdateScrollBeforeArrange();
				CalcViewRects(client);
				CalcFilterDrawInfo();
				UpdateTabControl();
				if(fViewInfoCalculation) return;
				fViewInfoCalculation = true;
				CalcConstants();
				UpdateView();
				View.UpdateScrollAfterArrange();
				IsReady = true;
			}
			finally {
				GInfo.ReleaseGraphics();
				fViewInfoCalculation = false;
			}
			if(View.fInternalNeedRecalcView && watchDog < 30) {
				View.fInternalNeedRecalcView = false;
				View.LockAutoPanByScroll();
				View.LockAutoPanByPartialCard();
				watchDog++;
				Calc(g, bounds);
				watchDog--;
				View.UnLockAutoPanByScroll();
				View.UnLockAutoPanByPartialCard();
			}
		}
		protected override void UpdateTabControl() {
			if(IsRealHeightCalculate) return;
			if(TabControl != null) {
				if(!ShowTabControl) TabControl.Bounds = Rectangle.Empty;
				else TabControl.Bounds = CalcBorderRect(Bounds);
			}
		}
		public new LayoutView View { get { return base.View as LayoutView; } }
		public override Rectangle Bounds { get { return bounds; } }
		public override Rectangle ClientBounds { get { return client; } }
		public override Rectangle ViewCaptionBounds { get { return ViewRects.ViewCaption; } }
		protected virtual Rectangle CalcTabClientRect() {
			return CalcTabClientRect(CalcBorderRect(Bounds));
		}
		public virtual Rectangle CalcScrollRect() {
			Rectangle client = CalcTabClientRect();
			Rectangle cards = ViewRects.CardsRect;
			if(cards.Width == 0 || cards.Height == 0)
				return client;
			int footerHeight = View.ScrollInfo.VScrollVisible ? ViewRects.FooterRect.Height : 0;
			return new Rectangle(client.Left, cards.Top, client.Width, (client.Bottom - footerHeight) - cards.Top);
		}
		public virtual LayoutViewHitInfo CalcHitInfo(Point pt) {
			LayoutViewHitInfo hi = new LayoutViewHitInfo();
			hi.View = View;
			hi.HitPoint = pt;
			if(!IsReady) return hi;
			hi.HitTest = LayoutViewHitTest.None;
			if(CheckMasterTabHitTest(hi, pt)) {
				hi.HitTest = LayoutViewHitTest.MasterTabPageHeader;
				return hi;
			}
			if(hi.CheckAndSetHitTest(ViewRects.Bounds, pt, LayoutViewHitTest.Bounds)) {
				if(hi.CheckAndSetHitTest(FilterPanel.Bounds, pt, LayoutViewHitTest.FilterPanel)) {
					hi.CheckAndSetHitTest(FilterPanel.CloseButtonInfo.Bounds, pt, LayoutViewHitTest.FilterPanelCloseButton);
					hi.CheckAndSetHitTest(FilterPanel.MRUButtonInfo.Bounds, pt, LayoutViewHitTest.FilterPanelMRUButton);
					hi.CheckAndSetHitTest(FilterPanel.CustomizeButtonInfo.Bounds, pt, LayoutViewHitTest.FilterPanelCustomizeButton);
					hi.CheckAndSetHitTest(FilterPanel.ActiveButtonInfo.Bounds, pt, LayoutViewHitTest.FilterPanelActiveButton);
					hi.CheckAndSetHitTest(FilterPanel.TextBounds, pt, LayoutViewHitTest.FilterPanelText);
					return hi;
				}
				if(hi.CheckAndSetHitTest(ViewRects.ClientRect, pt, LayoutViewHitTest.ClientArea)) {
					bool inCaption = hi.CheckAndSetHitTest(ViewRects.ViewCaption, pt, LayoutViewHitTest.ViewCaption);
					bool inHeader = hi.CheckAndSetHitTest(ViewRects.HeaderRect, pt, LayoutViewHitTest.HeaderArea);
					bool inFooter = hi.CheckAndSetHitTest(ViewRects.FooterRect, pt, LayoutViewHitTest.FooterArea);
					bool inCardsArea = hi.CheckAndSetHitTest(ViewRects.CardsRect, pt, LayoutViewHitTest.CardsArea);
					if(inCaption) return hi;
					if(inHeader) {
						hi.CheckAndSetHitTest(ViewRects.SingleModeButton, pt, LayoutViewHitTest.SingleModeButton);
						hi.CheckAndSetHitTest(ViewRects.RowModeButton, pt, LayoutViewHitTest.RowModeButton);
						hi.CheckAndSetHitTest(ViewRects.ColumnModeButton, pt, LayoutViewHitTest.ColumnModeButton);
						hi.CheckAndSetHitTest(ViewRects.MultiRowModeButton, pt, LayoutViewHitTest.MultiRowModeButton);
						hi.CheckAndSetHitTest(ViewRects.MultiColumnModeButton, pt, LayoutViewHitTest.MultiColumnModeButton);
						hi.CheckAndSetHitTest(ViewRects.CarouselModeButton, pt, LayoutViewHitTest.CarouselModeButton);
						hi.CheckAndSetHitTest(ViewRects.PanButton, pt, LayoutViewHitTest.PanButton);
						hi.CheckAndSetHitTest(ViewRects.CustomizeButton, pt, LayoutViewHitTest.CustomizeButton);
						hi.CheckAndSetHitTest(ViewRects.CloseZoomButton, pt, LayoutViewHitTest.CloseZoomButton);
						return hi;
					}
					if(inFooter) return hi;
					if(inCardsArea && VisibleCards.Count > 0) {
						foreach(LayoutViewCard card in VisibleCards) {
							if(!hi.CheckAndSetHitTest(card.Bounds, pt, LayoutViewHitTest.Card)) continue;
							hi.HitCard = card;
							hi.RowHandle = card.RowHandle;
							hi.VisibleIndex = card.VisibleIndex;
							if(View.OptionsView.ShowCardCaption) {
								bool inCardCaption = hi.CheckAndSetHitTest(card.ViewInfo.BorderInfo.CaptionBounds, pt, LayoutViewHitTest.CardCaption);
								bool inCardExpandButton = hi.CheckAndSetHitTest(card.ViewInfo.BorderInfo.ButtonBounds, pt, LayoutViewHitTest.CardExpandButton);
								if(inCardCaption || inCardExpandButton) return hi;
							}
							if(card.Expanded) {
								DevExpress.XtraLayout.HitInfo.BaseLayoutItemHitInfo lhi = card.GetLayoutItemHitInfo(pt);
								if(lhi != null && lhi.Item != null) {
									hi.LayoutItemHitInfo = lhi;
									hi.LayoutItem = lhi.Item;
									hi.HitRect = lhi.Item.ViewInfo.BoundsRelativeToControl;
									hi.HitTest = LayoutViewHitTest.LayoutItem;
									if(lhi.Item is LayoutViewField) {
										LayoutViewField field = lhi.Item as LayoutViewField;
										LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
										bool inField = hi.CheckAndSetHitTest(fieldInfo.BoundsRelativeToControl, pt, LayoutViewHitTest.Field);
										hi.Column = field.Column;
										hi.HitField = field;
										hi.CheckAndSetHitTest(fieldInfo.TextAreaRelativeToControl, pt, LayoutViewHitTest.FieldCaption);
										hi.CheckAndSetHitTest(fieldInfo.RepositoryItemViewInfo.Bounds, pt, LayoutViewHitTest.FieldValue);
										if(hi.CheckAndSetHitTest(fieldInfo.PopupActionArea, pt, LayoutViewHitTest.FieldPopupActionArea)) {
											bool inSortButton = hi.CheckAndSetHitTest(fieldInfo.SortButton, pt, LayoutViewHitTest.FieldSortButton);
											bool inFilterButton = hi.CheckAndSetHitTest(fieldInfo.FilterButton, pt, LayoutViewHitTest.FieldFilterButton);
											hi.HitRect = fieldInfo.PopupActionArea;
											if(!inSortButton && !inFilterButton) {
												hi.CheckAndSetHitTest(fieldInfo.RepositoryItemViewInfo.Bounds, pt, LayoutViewHitTest.FieldValue);
											}
										}
									}
								}
							}
							return hi;
						}
					}
				}
			}
			return hi;
		}
		public virtual LayoutViewPainter Painter { get { return View.Painter as LayoutViewPainter; } }
		public override ObjectPainter FilterPanelPainter { get { return Painter.ElementsPainter.FilterPanel; } }
		public override ObjectPainter ViewCaptionPainter { get { return Painter.ElementsPainter.ViewCaption; } }
		public virtual ObjectPainter ButtonPainter { get { return Painter.ElementsPainter.EditorButton; } }
		public new LayoutViewAppearances PaintAppearance {
			get { return base.PaintAppearance as LayoutViewAppearances; }
		}
		protected override BaseAppearanceCollection CreatePaintAppearances() {
			return new LayoutViewAppearances(View);
		}
		public override void PrepareCalcRealViewHeight(Rectangle viewRect, BaseViewInfo oldViewInfo) {
			if(oldViewInfo != null) {
				this.IsReady = oldViewInfo.IsReady;
			}
			CalcViewRects(viewRect);
		}
		public override int CalcRealViewHeight(Rectangle viewRect) {
			int result = viewRect.Height;
			StartRealHeightCalculate();
			try {
				if(View.DetailAutoHeight) {
					Calc(null, viewRect);
					int realViewHeight = layoutManager.GetRealViewHeight();
					if(VisibleCards.Count > 0)
						realViewHeight += calculatedHeaderHeight;
					result = (bounds.Height - client.Height) + realViewHeight;
				}
			}
			finally {
				EndRealHeightCalculate();
			}
			return result;
		}
		protected internal void ClearVisibleCards() {
			layoutManager.ClearVisibleCards();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LayoutViewRects ViewRects { get { return rects; } }
		public new LayoutViewSelectionInfo SelectionInfo { get { return base.SelectionInfo as LayoutViewSelectionInfo; } }
		protected override BaseSelectionInfo CreateSelectionInfo() { return new LayoutViewSelectionInfo(View); }
		protected internal virtual void UpdateBeforePaint(LayoutViewCard card) {
			bool conditionChanged = UpdateRowConditionAndFormat(card.RowHandle, card);
			if(conditionChanged) SetCardViewInfoDirty(card);
			UpdateCardAppearance(card);
			if(card.Expanded)
				UpdateCardFieldsAppearance(card);
		}
		protected virtual void UpdateCardFieldsAppearance(LayoutViewCard card) {
			List<LayoutViewColumn> columns = new List<LayoutViewColumn>(View.Columns);
			ColumnNamesMapper mapper = null;
			foreach(GridColumn column in columns) {
				LayoutViewField field = FindFieldItemByColumnName(card, column.Name, ref mapper);
				if(field == null || field.IsHidden) continue;
				CheckRepositoryItem(field, card.RowHandle, column);
				RequestEditViewInfo(card, field, card.RowHandle, column);
				AppearanceObjectEx conditionApp = card.GetConditionAppearance(column);
				UpdateFieldAppearance(field, card.RowHandle, column, conditionApp, card.State);
			}
		}
		protected void CheckRepositoryItem(LayoutViewField field, int rowHandle, GridColumn column) {
			LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
			Rectangle bounds = fieldInfo.RepositoryItemViewInfo.Bounds;
			RepositoryItem gridEditor = View.GetRowCellRepositoryItem(rowHandle, column);
			if(gridEditor != null && gridEditor != fieldInfo.RepositoryItem) {
				fieldInfo.RepositoryItem = gridEditor;
				BaseEditViewInfo editInfo = gridEditor.CreateViewInfo();
				editInfo.RightToLeft = View.IsRightToLeft;
				fieldInfo.RepositoryItemViewInfo = editInfo;
				fieldInfo.GridRowCellState = GridRowCellState.Dirty;
				UpdateFieldData(editInfo, rowHandle, column);
				RecalcFieldEditViewInfo(editInfo, bounds, rowHandle);
			}
		}
		void RecalcFieldEditViewInfo(BaseEditViewInfo editInfo, Rectangle bounds, int rowHandle) {
			Point pt = (GridControl != null && GridControl.IsHandleCreated ?
				GridControl.PointToClient(Control.MousePosition) : EmptyPoint);
			editInfo.UpdateBoundValues(View.DataController, rowHandle);
			editInfo.CalcViewInfo(null, Control.MouseButtons, pt, bounds);
		}
		protected internal virtual void UpdateCardAppearance(LayoutViewCard card) {
			card.PaintAppearanceGroup.AppearanceItemCaption.BeginUpdate();
			GridRowCellState state = CalcRowStateCore(card.RowHandle);
			card.State = state;
			if(card.PaintAppearanceGroup.AppearanceItemCaption == null || card.PaintAppearanceGroup.AppearanceItemCaption == PaintAppearance.CardCaption)
				card.PaintAppearanceGroup.AppearanceItemCaption = new AppearanceObject();
			AppearanceObject focused, selected, default_;
			default_ = PaintAppearance.CardCaption;
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
			AppearanceObject[] mix = new AppearanceObject[] { card.FormatInfo.RowAppearance, focused, selected, default_ };
			using(AppearanceObject appearanceGroup = new AppearanceObject()) {
				AppearanceHelper.Combine(appearanceGroup, mix);
				LayoutViewCardStyleEventArgs eCardStyle = new LayoutViewCardStyleEventArgs(card.RowHandle, state, appearanceGroup);
				View.RaiseCustomCardStyle(eCardStyle);
				card.PaintAppearanceGroup.AppearanceItemCaption.AssignInternal(appearanceGroup);
			}
			card.PaintAppearanceGroup.AppearanceItemCaption.CancelUpdate();
			card.PaintAppearanceGroup.AppearanceGroup.AssignInternal(card.PaintAppearanceGroup.AppearanceItemCaption);
			LayoutViewCard templateCard = View.TemplateCard;
			LayoutViewCardCaptionImageEventArgs e = new LayoutViewCardCaptionImageEventArgs(card.RowHandle, View.Images,
				templateCard.CaptionImageIndex, templateCard.CaptionImage, templateCard.CaptionImageVisible);
			View.RaiseCustomCardCaptionImage(e);
			card.ViewInfo.UpdateBorderInternal(View.IsMultiSelect, e, ((LayoutViewPaintStyle)View.PaintStyle).GetAllowHotTrackCards(View), View.HotCardRowHandle == card.RowHandle);
		}
		protected virtual void UpdateFieldAppearance(LayoutViewField field, int rowHandle, GridColumn column, AppearanceObjectEx condition, GridRowCellState rowState) {
			LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
			if(fieldInfo.GridRowCellState == GridRowCellState.Dirty) {
				BaseEditViewInfo editorInfo = fieldInfo.RepositoryItemViewInfo;
				UpdateAppearanceItemCaption(field, rowHandle, column, rowState);
				UpdateAppearanceFieldValue(field, editorInfo, rowHandle, column, condition, rowState);
				field.PerformUpdateViewInfo();
				fieldInfo.GridRowCellState = GridRowCellState.Default;
			}
		}
		protected internal virtual void UpdateAppearanceFieldEditingValue(LayoutViewField field, BaseEditViewInfo editorInfo, int rowHandle, GridColumn column, GridRowCellState rowState) {
			FrozenAppearance appearanceEditingValue = new FrozenAppearance();
			AppearanceObject repositoryItemAppearance = (field != null) ? field.RepositoryItem.Appearance : null;
			AppearanceHelper.Combine(appearanceEditingValue, new AppearanceObject[] { repositoryItemAppearance, GetFocusedAppearance(), PaintAppearance.FieldEditingValue });
			CheckDefaultAlignment(appearanceEditingValue, rowHandle, column);
			LayoutViewFieldEditingValueStyleEventArgs ea = new LayoutViewFieldEditingValueStyleEventArgs(
					rowHandle, column, rowState, appearanceEditingValue
				);
			View.RaiseCustomFieldEditingValueStyle(ea);
			editorInfo.PaintAppearance = appearanceEditingValue;
		}
		AppearanceObject GetFocusedAppearance() {
			FrozenAppearance res = new FrozenAppearance();
			if(View.PaintStyle.IsSkin) {
				Skin skin = GridSkins.GetSkin(View);
				if(skin.Colors.Contains(GridSkins.OptColorLayoutCardFocusedFieldBackColor))
					res.BackColor = skin.Colors[GridSkins.OptColorLayoutCardFocusedFieldBackColor];
				if(skin.Colors.Contains(GridSkins.OptColorLayoutCardFocusedFieldForeColor))
					res.ForeColor = skin.Colors[GridSkins.OptColorLayoutCardFocusedFieldForeColor];
			}
			return res;
		}
		protected internal virtual void UpdateAppearanceFieldValue(LayoutViewField field, BaseEditViewInfo editorInfo, int rowHandle, GridColumn column, AppearanceObjectEx condition, GridRowCellState rowState) {
			AppearanceObjectEx columnApp;
			AppearanceObject columnLow, columnHigh, conditionLow, conditionHigh;
			if(editorInfo.PaintAppearance == null || editorInfo.PaintAppearance == PaintAppearance.FieldValue) {
				editorInfo.PaintAppearance = new FrozenAppearance();
			}
			columnApp = column.AppearanceCell;
			columnLow = columnHigh = conditionLow = conditionHigh = null;
			if(columnApp != null) {
				if(columnApp.Options.HighPriority)
					columnHigh = columnApp;
				else
					columnLow = columnApp;
			}
			if(condition != null) {
				if(condition.Options.HighPriority)
					conditionHigh = condition;
				else
					conditionLow = condition;
			}
			AppearanceObject[] mix = new AppearanceObject[] { 
				columnHigh, conditionHigh, columnLow, conditionLow, 
				CheckItemForeAppearance(field, View.Appearance.FieldValue, PaintAppearance.FieldValue)
			};
			FrozenAppearance appearanceFieldValue = new FrozenAppearance();
			AppearanceHelper.Combine(appearanceFieldValue, mix);
			CheckDefaultAlignment(appearanceFieldValue, rowHandle, column);
			LayoutViewFieldValueStyleEventArgs ea = new LayoutViewFieldValueStyleEventArgs(
					rowHandle, column, rowState, appearanceFieldValue
				);
			View.RaiseCustomFieldValueStyle(ea);
			editorInfo.PaintAppearance = appearanceFieldValue;
		}
		AppearanceObject CheckItemForeAppearance(BaseLayoutItem item, AppearanceObject appearance, AppearanceObject paintAppearance) {
			FrozenAppearance result = new FrozenAppearance(paintAppearance);
			if(!appearance.Options.UseForeColor)
				result.ForeColor = ((DevExpress.LookAndFeel.ITransparentBackgroundManager)View).GetForeColor(item);
			return result;
		}
		protected void CheckDefaultAlignment(AppearanceObject appearance, int rowHandle, GridColumn column) {
			if(appearance.HAlignment == HorzAlignment.Default) {
				appearance.TextOptions.HAlignment = View.GetRowCellDefaultAlignment(rowHandle, column, column.DefaultValueAlignment);
				appearance.Options.UseTextOptions |= (appearance.TextOptions.HAlignment != HorzAlignment.Default);
			}
		}
		protected internal virtual void UpdateAppearanceItemCaption(LayoutViewField field, int rowHandle, GridColumn column, GridRowCellState rowState) {
			field.PaintAppearanceItemCaption.BeginUpdate();
			if(field.PaintAppearanceItemCaption == null || field.PaintAppearanceItemCaption == PaintAppearance.FieldCaption)
				field.PaintAppearanceItemCaption.Reset();
			AppearanceObject[] mix = new AppearanceObject[] { 
				column.AppearanceHeader, 
				CheckItemForeAppearance(field, View.Appearance.FieldCaption, PaintAppearance.FieldCaption) 
			};
			using(AppearanceObject appearanceFieldCaption = new AppearanceObject()) {
				AppearanceHelper.Combine(appearanceFieldCaption, mix);
				LayoutViewFieldCaptionStyleEventArgs eFieldCaptionStyle = new LayoutViewFieldCaptionStyleEventArgs(rowHandle, column, rowState, appearanceFieldCaption);
				View.RaiseCustomFieldCaptionStyle(eFieldCaptionStyle);
				field.PaintAppearanceItemCaption.AssignInternal(appearanceFieldCaption);
			}
			field.PaintAppearanceItemCaption.CancelUpdate();
		}
		internal void ResetFormatInfo() {
			foreach(var card in VisibleCards) {
				card.ResetFormatInfo();
			}
		}
	}
	#region HitInfo
	public enum LayoutViewHitTest {
		None, Bounds, ClientArea, HeaderArea, FooterArea, CardsArea, ViewCaption,
		Card, CardCaption, CardExpandButton,
		Field, FieldCaption, FieldValue,
		FieldPopupActionArea, FieldSortButton, FieldFilterButton,
		LayoutItem,
		SingleModeButton, RowModeButton, ColumnModeButton,
		MultiRowModeButton, MultiColumnModeButton, CarouselModeButton,
		PanButton, CustomizeButton,
		CloseZoomButton,
		FilterPanel, FilterPanelCloseButton, FilterPanelActiveButton, FilterPanelText, FilterPanelMRUButton, FilterPanelCustomizeButton, MasterTabPageHeader
	};
	public class LayoutViewHitInfo : BaseHitInfo {
		static LayoutViewHitInfo emptyInfo;
		internal static LayoutViewHitInfo EmptyInfo {
			get {
				if(emptyInfo == null) emptyInfo = new LayoutViewHitInfo();
				return emptyInfo;
			}
		}
		int rowHandle;
		int visibleIndex;
		GridColumn column;
		LayoutViewHitTest hitTest;
		Rectangle hitRect;
		LayoutViewField hitField = null;
		LayoutViewCard hitCard = null;
		BaseLayoutItem layoutItemCore = null;
		DevExpress.XtraLayout.HitInfo.BaseLayoutItemHitInfo layoutItemHitInfoCore = null;
		public new LayoutView View { get { return (LayoutView)base.View; } set { base.View = value; } }
		public LayoutViewHitInfo() { Clear(); }
		protected bool ContainsPoint(Rectangle bounds, Point p) { return !bounds.IsEmpty && bounds.Contains(p); }
		protected internal bool CheckAndSetHitTest(Rectangle bounds, Point p, LayoutViewHitTest hitTest) {
			bool fContains = ContainsPoint(bounds, p);
			if(fContains) {
				this.hitTest = hitTest;
				this.hitRect = bounds;
			}
			return fContains;
		}
		public override void Clear() {
			base.Clear();
			this.hitField = null;
			this.column = null;
			this.rowHandle = DataController.InvalidRow;
			this.hitTest = LayoutViewHitTest.None;
			this.layoutItemCore = null;
			this.layoutItemHitInfoCore = null;
		}
		public GridColumn Column { get { return column; } set { column = value; } }
		public int RowHandle { get { return rowHandle; } set { rowHandle = value; } }
		public int VisibleIndex { get { return visibleIndex; } set { visibleIndex = value; } }
		public LayoutViewHitTest HitTest { get { return hitTest; } set { hitTest = value; } }
		public Rectangle HitRect { get { return hitRect; } set { hitRect = value; } }
		public LayoutViewField HitField { get { return hitField; } set { hitField = value; } }
		public LayoutViewCard HitCard { get { return hitCard; } set { hitCard = value; } }
		public BaseLayoutItem LayoutItem { get { return layoutItemCore; } set { layoutItemCore = value; } }
		protected internal DevExpress.XtraLayout.HitInfo.BaseLayoutItemHitInfo LayoutItemHitInfo { get { return layoutItemHitInfoCore; } set { layoutItemHitInfoCore = value; } }
		public virtual bool InBounds { get { return IsValid && HitTest != LayoutViewHitTest.None; } }
		public virtual bool InClientArea { get { return IsValid && HitTest != LayoutViewHitTest.None && HitTest != LayoutViewHitTest.Bounds; } }
		public virtual bool InHeaderArea { get { return IsValid && InClientArea && (HitTest == LayoutViewHitTest.HeaderArea || InHeaderButtons); } }
		public virtual bool InFooterArea { get { return IsValid && InClientArea && HitTest == LayoutViewHitTest.FooterArea; } }
		public virtual bool InCardsArea { get { return IsValid && InClientArea && !(InHeaderArea || InFilterPanel); } }
		public virtual bool InViewCaption { get { return IsValid && InClientArea && (HitTest == LayoutViewHitTest.ViewCaption); } }
		public virtual bool InCard { get { return IsValid && (InCardCaption || InField || InLayoutItem || HitTest == LayoutViewHitTest.Card); } }
		public virtual bool InCardCaption { get { return IsValid && (HitTest == LayoutViewHitTest.CardCaption || InCardExpandButton); } }
		public virtual bool InCardExpandButton { get { return IsValid && HitTest == LayoutViewHitTest.CardExpandButton; } }
		public virtual bool InField { get { return IsValid && (HitTest == LayoutViewHitTest.Field || InFieldCaption || InFieldValue); } }
		public virtual bool InFieldValue { get { return IsValid && (HitTest == LayoutViewHitTest.FieldValue || InFieldPopupActionArea); } }
		public virtual bool InFieldCaption { get { return IsValid && (HitTest == LayoutViewHitTest.FieldCaption); } }
		public virtual bool InFieldPopupActionArea { get { return IsValid && (HitTest == LayoutViewHitTest.FieldPopupActionArea || InFieldSortButton || InFieldFilterButton); } }
		public virtual bool InFieldSortButton { get { return IsValid && HitTest == LayoutViewHitTest.FieldSortButton; } }
		public virtual bool InFieldFilterButton { get { return IsValid && HitTest == LayoutViewHitTest.FieldFilterButton; } }
		public virtual bool InLayoutItem { get { return IsValid && HitTest == LayoutViewHitTest.LayoutItem; } }
		public virtual bool InHeaderButtons { get { return InSingleModeButton || InRowModeButton || InColumnModeButton || InPanButton || InCustomizeButton || InMultiRowModeButton || InMultiColumnModeButton || InCarouselModeButton || InCloseZoomButton; } }
		public virtual bool InSingleModeButton { get { return IsValid && (HitTest == LayoutViewHitTest.SingleModeButton); } }
		public virtual bool InRowModeButton { get { return IsValid && (HitTest == LayoutViewHitTest.RowModeButton); } }
		public virtual bool InColumnModeButton { get { return IsValid && (HitTest == LayoutViewHitTest.ColumnModeButton); } }
		public virtual bool InMultiRowModeButton { get { return IsValid && (HitTest == LayoutViewHitTest.MultiRowModeButton); } }
		public virtual bool InMultiColumnModeButton { get { return IsValid && (HitTest == LayoutViewHitTest.MultiColumnModeButton); } }
		public virtual bool InCarouselModeButton { get { return IsValid && (HitTest == LayoutViewHitTest.CarouselModeButton); } }
		public virtual bool InCloseZoomButton { get { return IsValid && (HitTest == LayoutViewHitTest.CloseZoomButton); } }
		public virtual bool InPanButton { get { return IsValid && (HitTest == LayoutViewHitTest.PanButton); } }
		public virtual bool InCustomizeButton { get { return IsValid && (HitTest == LayoutViewHitTest.CustomizeButton); } }
		public virtual bool InFilterPanel {
			get {
				return IsValid && (HitTest == LayoutViewHitTest.FilterPanel || HitTest == LayoutViewHitTest.FilterPanelActiveButton ||
				  HitTest == LayoutViewHitTest.FilterPanelCloseButton || HitTest == LayoutViewHitTest.FilterPanelMRUButton ||
					HitTest == LayoutViewHitTest.FilterPanelText || HitTest == LayoutViewHitTest.FilterPanelCustomizeButton);
			}
		}
		public virtual bool IsEquals(LayoutViewHitInfo hitInfo) {
			return this.RowHandle == hitInfo.RowHandle && this.HitTest == hitInfo.HitTest;
		}
		protected internal override int HitTestInt { get { return (int)HitTest; } }
	}
	#endregion
	class TouchOffsetAnimator : ISupportXtraAnimation {
		LayoutView view;
		public TouchOffsetAnimator(LayoutView view) {
			this.view = view;
		}
		bool ISupportXtraAnimation.CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return view.GridControl; }
		}
		int touchOffset;
		int maxOffset;
		bool hScroll;
		public void StartAnimation(int touchOffset, int maxOffset, bool hScroll) {
			XtraAnimator.RemoveObject(this);
			this.touchOffset = touchOffset;
			this.maxOffset = maxOffset;
			this.hScroll = hScroll;
			progress = 0.0;
			AnimationInProgress = true;
			XtraAnimator.Current.AddObject(this, this, 2500, 1000, OnAnimation);
		}
		public void StopAnimation(bool? needUpdate = null) {
			progress = 0.0;
			XtraAnimator.RemoveObject(this);
			OnStopAnimation(needUpdate.GetValueOrDefault(AnimationInProgress));
			AnimationInProgress = false;
		}
		void OnStopAnimation(bool update) {
			if(view != null) {
				view.touchOffset = null;
				view.prevTouchOffset = 0;
				view.Invalidate();
			}
			if(update && view.GridControl != null)
				view.GridControl.Update();
		}
		static IEasingFunction DefaultEasingFunction = new ExponentialEase();
		[Browsable(false)]
		public bool AnimationInProgress { get; private set; }
		double progress;
		void OnAnimation(BaseAnimationInfo info) {
			if(!AnimationInProgress) return;
			progress += ((double)(info.CurrentFrame - (info.PrevFrame < 0 ? 0 : info.PrevFrame)) / (double)info.FrameCount);
			OnAnimationCore(info, CalcEasingFunction(progress));
			if(info.IsFinalFrame) {
				bool needUpdate = false;
				if(Math.Abs(touchOffset) <= maxOffset) {
					if(Math.Abs(touchOffset) * 2 > maxOffset)
						needUpdate = view.DoNavigateRecordsByTouch(hScroll, touchOffset);
				}
				StopAnimation(needUpdate);
			}
		}
		void OnAnimationCore(BaseAnimationInfo info, double progress) {
			if(Math.Abs(touchOffset) > maxOffset)
				return;
			int offset = 0;
			if(Math.Abs(touchOffset) * 2 > maxOffset)
				offset = Math.Abs(touchOffset) + (int)Math.Round(progress * (double)(maxOffset - Math.Abs(touchOffset)));
			else
				offset = (int)Math.Round((1.0 - progress) * (double)Math.Abs(touchOffset));
			if(touchOffset < 0)
				offset = -offset;
			int d = offset - view.TouchScrollOffset;
			Point delta = hScroll ? new Point(d, 0) : new Point(0, d);
			view.touchOffset = offset;
			view.DoScrollView(delta, hScroll);
		}
		static double CalcEasingFunction(double progress) {
			return EaseHelper.Ease(EasingMode.EaseOut, DefaultEasingFunction, progress);
		}
	}
}
