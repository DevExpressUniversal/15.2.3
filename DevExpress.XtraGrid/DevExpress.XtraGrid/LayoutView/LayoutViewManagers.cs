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
using System.Text;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.XtraGrid.Views.Layout;
using System.Drawing;
using DevExpress.XtraGrid.Views;
using DevExpress.XtraLayout.Painting;
using System.Collections;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using System.Windows.Forms;
using DevExpress.XtraLayout;
using DevExpress.XtraGrid.Views.Layout.Drawing;
namespace DevExpress.XtraGrid.Views.Layout.Modes {
	public class RowInfo {
		public List<LayoutViewCard> Cards;
		public Size RowSize;
		public Rectangle RowRect;
		public float fHSpace;
		public float fVSpace;
		public int iMaxCardHeight;
		public RowInfo(Size rowSize) {
			this.Cards = new List<LayoutViewCard>();
			this.RowSize = rowSize;
		}
	}
	public struct RangeInfo {
		public int Begin { get; private set; }
		public int End { get; private set; }
		public RangeInfo(int begin, int end)
			: this() {
			this.Begin = Math.Min(begin, end);
			this.End = Math.Max(begin, end);
		}
		public int Length {
			get { return End - Begin; } 
		}
		public bool IsEmpty {
			get { return Length == 0; }
		}
		public void Union(RangeInfo r) {
			if(IsEmpty) {
				Begin = r.Begin;
				End = r.End;
			}
			else {
				Begin = Math.Min(Begin, r.Begin);
				End = Math.Max(End, r.End);
			}
		}
		public override string ToString() {
			return IsEmpty ? "Empty" : string.Format("[{0}..{1}]", Begin, End);
		}
	}
	public class ColumnInfo {
		public List<LayoutViewCard> Cards;
		public Size ColumnSize;
		public Rectangle ColumnRect;
		public float fHSpace;
		public float fVSpace;
		public int iMaxCardWidth;
		public ColumnInfo(Size columnSize) {
			this.Cards = new List<LayoutViewCard>();
			this.ColumnSize = columnSize;
		}
	}
	public class CardArrangeCache : IDisposable {
		public int iMaxCardWidth = 0;
		public int iMaxCardHeight = 0;
		public int currentHPan = 0;
		public int currentVPan = 0;
		public float fCardHSpace = 0;
		public float fCardVSpace = 0;
		public Size EmptySpace = Size.Empty;
		public Size CardsArea = Size.Empty;
		public List<RowInfo> Rows;
		public List<ColumnInfo> Columns;
		protected SortedList<int, LayoutViewCard> cacheCore;
		public int RecordCount { get { return cacheCore.Count; } }
		public IList<LayoutViewCard> Cards { get { return cacheCore.Values; } }
		public bool CanPanHorz {
			get { return EmptySpace.Width < 0; }
		}
		public bool CanPanVert {
			get { return EmptySpace.Height < 0; }
		}
		public bool PanForce;
		public CardArrangeCache() {
			this.Rows = new List<RowInfo>();
			this.Columns = new List<ColumnInfo>();
			this.cacheCore = new SortedList<int, LayoutViewCard>();
		}
		public void Dispose() {
			if(Rows != null) {
				Rows.Clear();
				Rows = null;
			}
			if(Columns != null) {
				Columns.Clear();
				Columns = null;
			}
			if(cacheCore != null) {
				cacheCore.Clear();
				cacheCore = null;
			}
		}
		public void Reset() {
			iMaxCardWidth = iMaxCardHeight = 0;
			currentHPan = currentVPan = 0;
			cacheCore.Clear();
			PreArrangeReset();
		}
		public void PreArrangeReset() {
			Rows.Clear();
			Columns.Clear();
			CardsArea = Size.Empty;
		}
		public void Cache(LayoutViewCard card, int recordIndex) {
			iMaxCardHeight = Math.Max(iMaxCardHeight, card.Height);
			iMaxCardWidth = Math.Max(iMaxCardWidth, card.Width);
			card.VisibleIndex = card.VisibleColumn = card.VisibleRow = -1;
			cacheCore.Add(recordIndex, card);
		}
		public List<ColumnInfo> GetColumns() {
			List<ColumnInfo> columns = new List<ColumnInfo>();
			foreach(RowInfo row in Rows) {
				foreach(LayoutViewCard card in row.Cards) {
					while(columns.Count <= card.VisibleColumn)
						columns.Add(new ColumnInfo(Size.Empty));
					columns[card.VisibleColumn].Cards.Add(card);
				}
			}
			return columns;
		}
		public List<RowInfo> GetRows() {
			List<RowInfo> rows = new List<RowInfo>();
			foreach(ColumnInfo column in Columns) {
				foreach(LayoutViewCard card in column.Cards) {
					while(rows.Count <= card.VisibleRow)
						rows.Add(new RowInfo(Size.Empty));
					rows[card.VisibleRow].Cards.Add(card);
				}
			}
			return rows;
		}
	}
	public abstract class LayoutViewBaseMode : ILayoutViewManager {
		List<Line> cardLinesCore = new List<Line>();
		List<Line> linesCore = new List<Line>();
		List<LayoutViewCard> visibleCardsCore = new List<LayoutViewCard>();
		ILayoutViewInfo ownerViewInfo;
		int iLastArrangedRecordIndex = -1;
		int lastFocusedRowHandle = -1;
		bool fDisposingInProgress = false;
		protected bool scrollPlacementUsed = false;
		protected internal CardArrangeCache ArrangeCache = null;
		public LayoutViewBaseMode(ILayoutViewInfo ownerViewInfo) {
			this.ownerViewInfo = ownerViewInfo;
			this.ArrangeCache = CreateArrangeCache();
		}
		void IDisposable.Dispose() {
			if(!fDisposingInProgress) {
				this.fDisposingInProgress = true;
				OnDispose();
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void OnDispose() {
			if(cardLinesCore != null) {
				cardLinesCore.Clear();
				cardLinesCore = null;
			}
			if(linesCore != null) {
				linesCore.Clear();
				linesCore = null;
			}
			if(visibleCardsCore != null) {
				ClearVisibleCards();
				visibleCardsCore = null;
			}
			if(ArrangeCache != null) {
				ArrangeCache.Dispose();
				ArrangeCache = null;
			}
			ownerViewInfo = null;
		}
		public int GetRealViewHeight() {
			if(VisibleCards.Count == 0) return ViewInfo.ViewRects.Bounds.Height;
			int unusedSpace = ViewInfo.ViewRects.CardsRect.Height - GetRealHeight();
			return ViewInfo.ViewRects.Bounds.Height - unusedSpace;
		}
		public virtual void OnGridLoadComplete() { }
		protected bool IsDisposingInProgress {
			get { return fDisposingInProgress; }
		}
		protected int MinVisibleCardThickness {
			get { return View != null ? View.OptionsView.PartialCardWrapThreshold : 10; }
		}
		public int SeparatorWidth {
			get { return View.OptionsView.ShowCardLines ? 2 : 0; }
		}
		protected int MinArrangeHSpacing {
			get { return View.CardHorzInterval; }
		}
		protected int MinArrangeVSpacing {
			get { return View.CardVertInterval; }
		}
		protected bool UseWholeCards {
			get { return (View != null) ? View.OptionsView.CardArrangeRule == LayoutCardArrangeRule.ShowWholeCards : true; }
		}
		public List<Line> CardLines {
			get { return cardLinesCore; }
		}
		public List<Line> Lines {
			get { return linesCore; }
		}
		protected virtual int MaxCardRows {
			get { return View != null ? View.OptionsMultiRecordMode.MaxCardRows : 0; }
		}
		protected virtual int MaxCardColumns {
			get { return View != null ? View.OptionsMultiRecordMode.MaxCardColumns : 0; }
		}
		protected void UpdateCardPosition(LayoutViewCard card) {
			card.ViewInfo.Offset = card.Location;
			card.ViewInfo.CalculateViewInfo();
			card.UpdateChildrenToBeRefactored();
		}
		protected virtual CardArrangeCache CreateArrangeCache() {
			return new CardArrangeCache();
		}
		protected internal LayoutView View {
			get { return ownerViewInfo.Owner as LayoutView; }
		}
		protected ILayoutViewInfo ViewInfo {
			get { return ownerViewInfo; }
		}
		public virtual bool NavigationHScrollNeed { get { return false; } }
		public virtual bool NavigationVScrollNeed { get { return false; } }
		public virtual int GetVSmallChange(bool forward) { return 1; }
		public virtual int GetHSmallChange(bool forward) { return 1; }
		public virtual int GetScrollableCardsCount() {
			return VisibleCards.Count;
		}
		public int CheckMinScrollChange(int newPos, bool forward) {
			if(VisibleCards.Count > 1) {
				LayoutViewCard first = forward ? VisibleCards[0] : VisibleCards[VisibleCards.Count - 1];
				LayoutViewCard last = forward ? VisibleCards[VisibleCards.Count - 1] : VisibleCards[0];
				if(!first.Expanded) {
					LayoutViewCard next = ViewInfo.InitializeCard(last.RowHandle + (forward ? 1 : -1));
					if(forward)
						newPos = Math.Max(CheckMinScrollChangeCore(forward, next), newPos);
					else
						newPos = Math.Min(CheckMinScrollChangeCore(forward, next), newPos);
					AddCardToCache(next);
				}
			}
			return newPos;
		}
		protected virtual int CheckMinScrollChangeCore(bool forward, LayoutViewCard nextCard) {
			return iLastArrangedRecordIndex + (forward ? 1 : -1);
		}
		protected int CalcScrollableCardsCount(IList<LayoutViewCard> cards, Rectangle rect) {
			int scrollableCards = cards.Count;
			if(scrollableCards > 1) {
				foreach(LayoutViewCard card in cards) {
					if(!rect.Contains(new Rectangle(card.Location, card.Size)))
						scrollableCards--;
				}
			}
			return scrollableCards;
		}
		public bool IsSideCaption {
			get {
				return View.TemplateCard.TextLocation == DevExpress.Utils.Locations.Left ||
					View.TemplateCard.TextLocation == DevExpress.Utils.Locations.Right;
			}
		}
		public virtual CardCollapsingMode CardCollapsingMode {
			get { return IsSideCaption ? CardCollapsingMode.Vertical : CardCollapsingMode.Horisontal; }
		}
		public virtual Size CollapsedCardSize {
			get { return GetCollapsedCardSizeDefault(); }
		}
		protected virtual Size GetCollapsedCardSizeDefault() {
			return new Size(200, 27);
		}
		public virtual LayoutViewMode ViewMode {
			get { return LayoutViewMode.SingleRecord; }
		}
		protected static int Round(float f) {
			return (f >= 0) ? (int)(f + 0.5f) : (int)(f - 0.5f);
		}
		protected bool IsPartiallyVisible(Rectangle view, bool horz, LayoutViewCard card) {
			Rectangle cardRect = new Rectangle(card.Location, card.Size);
			return horz ?
				(cardRect.Left < view.Left || cardRect.Right > view.Right) :
				(cardRect.Top < view.Top || cardRect.Bottom > view.Bottom);
		}
		protected static bool CheckDim(int dim, int avail, int spacing) {
			return dim <= (avail - spacing * 2);
		}
		protected static float GetSpacing(int cardsCount, float fSpace) {
			return (cardsCount > 0) ? (float)(cardsCount - 1) * fSpace : 0;
		}
		public virtual void ResetCache() { }
		public bool CanCardAreaPan {
			get { return ArrangeCache.CanPanHorz || ArrangeCache.CanPanVert || ArrangeCache.PanForce; }
		}
		public void Pan(int dx, int dy) {
			Rectangle cardsArea = CalcCardsArea();
			bool fHorizontal = cardsArea.Width > ViewInfo.ViewRects.CardsRect.Width;
			bool fVertical = cardsArea.Height > ViewInfo.ViewRects.CardsRect.Height;
			PanCardsAreaIfNeed(dx, dy, fHorizontal, fVertical);
		}
		protected void AddCardToCache(LayoutViewCard card) {
			if(card.Expanded)
				View.ExpandedCardCache.Push(card);
			else
				View.CollapsedCardCache.Push(card);
			card.Owner = null;
			card.RowHandle = GridControl.InvalidRowHandle;
			card.VisibleColumn = card.VisibleRow = card.VisibleIndex = -1;
		}
		public void ClearVisibleCards() {
			foreach(LayoutViewCard card in VisibleCards) 
				AddCardToCache(card);
			VisibleCards.Clear();
		}
		protected void AddItemsToVisibleCards(IList<LayoutViewCard> newCards) {
			foreach(LayoutViewCard card in newCards) card.Owner = View;
			VisibleCards.AddRange(newCards);
		}
		Rectangle lastArrangeArea;
		int lockArrangeCounter = 0;
		public bool IsArrangeLocked {
			get { return lockArrangeCounter > 0; }
		}
		public virtual void BeginArrange() {
			lockArrangeCounter++;
		}
		public virtual void EndArrange() {
			--lockArrangeCounter;
		}
		public virtual void Arrange(int currentRecordIndex) {
			if(IsArrangeLocked) return;
			BeginArrange();
			currentRecordIndex = View.CheckRecordIndex(currentRecordIndex);
			View.CorrectArrangeDirection();
			try {
				if(IfNeedArrange(currentRecordIndex)) {
					CardLines.Clear();
					Lines.Clear();
					ClearVisibleCards();
					ArrangeCache.Reset();
					CollectAllCardsWhichMayBeVisible(currentRecordIndex);
					if(ArrangeCache.RecordCount > 0) {
						AddItemsToVisibleCards(ArrangeCache.Cards);
						scrollPlacementUsed = false;
						ArrangeCache.EmptySpace = GetEmptySpace();
						if(!View.ViewInfo.IsRealHeightCalculate) {
							ArrangeVisibleCards();
							ArrangePostProcess();
							ArrangeRTL();
							((ILayoutControl)View).Invalidate();
						}
						iLastArrangedRecordIndex = currentRecordIndex;
						lastFocusedRowHandle = View.FocusedRowHandle;
						lastArrangeArea = ViewInfo.ViewRects.CardsRect;
					}
				}
				if(!View.ViewInfo.IsRealHeightCalculate) 
					AutoPanCardsArea();
			}
			finally {
				ViewInfo.NeedArrangeForce = false;
				View.ResetArrangeDirection();
				EndArrange();
			}
		}
		public List<LayoutViewCard> VisibleCards {
			get { return visibleCardsCore; }
		}
		protected virtual LayoutViewCard TryInsertCardInEmptySpaceAndCache(int recordIndex) {
			LayoutViewCard card = ViewInfo.InitializeCard(recordIndex);
			if(card != null) {
				ArrangeCache.EmptySpace = GetEmptySpace();
				bool fCanInsertCard = CanInsertCardByCurrentArrangeStrategy(ArrangeCache.EmptySpace, card);
				if(fCanInsertCard && CheckMaxCardRowsColumns()) {
					ArrangeCache.Cache(card, recordIndex);
					return card;
				}
				else AddCardToCache(card);
			}
			return null;
		}
		protected virtual bool CheckMaxCardRowsColumns() { return true; }
		protected virtual bool CanInsertCardByCurrentArrangeStrategy(Size emptySpaceSize, LayoutViewCard card) {
			card.IsPartiallyVisible = !CheckWholeCardPlacementAbility(emptySpaceSize, card);
			return !card.IsPartiallyVisible || CheckCardPlacementAbilityWithCardPan(emptySpaceSize, card);
		}
		protected virtual Size CheckCardMinSize(Size cardMinSize, bool collapsed) { return cardMinSize; }
		public virtual bool CheckMinMaxSizeConstraints(LayoutViewCard card) {
			bool collapsed = View.GetCardCollapsed(card.RowHandle);
			Size cardMinSize = card.MinSize;
			Size viewCardMinSize = CheckCardMinSize(View.CardMinSize, collapsed);
			int iMinWidth = Math.Max(viewCardMinSize.Width, cardMinSize.Width);
			int iMinHeight = Math.Max(viewCardMinSize.Height, cardMinSize.Height);
			if(collapsed && IsSideCaption && CardCollapsingMode == CardCollapsingMode.Vertical) {
				iMinWidth = Math.Max(viewCardMinSize.Height, cardMinSize.Height);
				iMinHeight = Math.Max(viewCardMinSize.Width, cardMinSize.Width);
			}
			if(card.Size.Width != iMinWidth || card.Size.Height != iMinHeight) {
				card.Size = new Size(iMinWidth, iMinHeight);
				return true;
			}
			return false;
		}
		public virtual bool CanStretchCardByWidth { 
			get { return false; } 
		}
		protected virtual bool CheckWholeCardPlacementAbility(Size space, LayoutViewCard card) {
			return card.Size.Width < (space.Width - MinArrangeHSpacing) && card.Size.Height < (space.Height - MinArrangeVSpacing);
		}
		protected virtual bool CheckCardPlacementAbilityWithCardPan(Size space, LayoutViewCard card) {
			if(!scrollPlacementUsed && (space.Width > 0 && space.Height > 0)) {
				scrollPlacementUsed = true;
				return true;
			}
			return false;
		}
		protected bool IfNeedArrange(int recordIndex) {
			if(View.ViewInfo.IsRealHeightCalculate) return true;
			bool fMayBeImagesInFields = (View.iCustomFieldCaptionEventHandlerCounter > 0);
			bool fMayBeCellEditChanged = (View.iCustomRowCellEditEventHandlerCounter > 0);
			bool fMayBeCardLayoutChanged = (View.iCustomCardLayoutEventHandlerCounter > 0);
			bool fHasChanges = (lastArrangeArea != ViewInfo.ViewRects.CardsRect) || (recordIndex != iLastArrangedRecordIndex);
			bool fHasRowDependentChanges = (recordIndex != iLastArrangedRecordIndex) || (View.FocusedRowHandle != lastFocusedRowHandle);
			if(recordIndex == iLastArrangedRecordIndex && (fMayBeImagesInFields || fMayBeCellEditChanged)) {
				int firstcard = View.ViewInfo.GetFirstCardRowHandle();
				int lastcard = View.ViewInfo.GetLastCardRowHandle();
				if(!fHasChanges && View.IsFocusedRowChangeLocked) {
					if(View.FocusedRowHandle >= firstcard && View.FocusedRowHandle <= lastcard) return false;
				}
				if(recordIndex == lastcard) {
					bool rowsCheck = (lastcard > View.OptionsMultiRecordMode.MaxCardRows) && View.OptionsMultiRecordMode.MaxCardRows != 0;
					bool columnsCheck = (lastcard > View.OptionsMultiRecordMode.MaxCardColumns) && View.OptionsMultiRecordMode.MaxCardColumns != 0;
					if(rowsCheck || columnsCheck) View.internalArrangeFromLeftToRight = false;
				}
			}
			return (View.IsInitialized && (!View.PanModeActive &&
					(fHasChanges || ViewInfo.NeedArrangeForce || ((fMayBeImagesInFields || fMayBeCellEditChanged || fMayBeCardLayoutChanged) && fHasRowDependentChanges))
				));
		}
		protected abstract void CollectAllCardsWhichMayBeVisible(int currentRecordIndex);
		protected abstract void ArrangeVisibleCards();
		protected abstract void ArrangePostProcess();
		void ArrangeRTL(){
			if(!View.IsRightToLeft) return;
			foreach(LayoutViewCard card in VisibleCards) {
				card.Position = ViewInfo.ViewRects.CheckRTL(card.Bounds, ViewInfo.ViewRects.CardsRect).Location;
			}
			foreach(Line cardLine in CardLines) {
				cardLine.BeginPoint = ViewInfo.ViewRects.CheckRTL(cardLine.BeginPoint, ViewInfo.ViewRects.CardsRect);
				cardLine.EndPoint = ViewInfo.ViewRects.CheckRTL(cardLine.EndPoint, ViewInfo.ViewRects.CardsRect);
			}
			foreach(Line line in Lines) {
				line.BeginPoint = ViewInfo.ViewRects.CheckRTL(line.BeginPoint, ViewInfo.ViewRects.CardsRect);
				line.EndPoint = ViewInfo.ViewRects.CheckRTL(line.EndPoint, ViewInfo.ViewRects.CardsRect);
			}
		}
		protected abstract Size GetEmptySpace();
		protected abstract int GetRealHeight();
		protected virtual void AutoPanCardsArea() {
			if(VisibleCards.Count > 0) {
				Rectangle viewRect = ViewInfo.ViewRects.CardsRect;
				LayoutViewCard firstCard = VisibleCards[0];
				LayoutViewCard lastCard = VisibleCards[VisibleCards.Count - 1];
				LayoutViewCard focusedCard = View.FindCardByRow(View.FocusedRowHandle);
				if(!UseWholeCards) {
					CorrectAutoPanByScroll(viewRect, firstCard, lastCard);
				}
				int dx = 0; int dy = 0; bool fByFocused = false;
				bool fAutoPanPerformed = CheckAutoPanByScroll(viewRect, ref dx, ref dy);
				if(!fAutoPanPerformed && focusedCard != null) {
					fAutoPanPerformed = fByFocused = CheckAutoPanByFocusedCard(viewRect, focusedCard, ref dx, ref dy);
					if(!UseWholeCards && focusedCard == lastCard && focusedCard != firstCard) {
						fAutoPanPerformed = false;
					}
					if(!UseWholeCards && focusedCard != lastCard && focusedCard != firstCard) {
						if(CardIsOutOfRect(viewRect, lastCard, dx, dy) || CardIsOutOfRect(viewRect, firstCard, dx, dy)) {
							fAutoPanPerformed = false;
						}
					}
				}
				if(!fAutoPanPerformed && !UseWholeCards) {
					fAutoPanPerformed = CheckAutoPanByPartialCard(viewRect, firstCard, lastCard, ref dx, ref dy) || fByFocused;
				}
				if(fAutoPanPerformed) PanCardsAreaIfNeed(dx, dy, dx != 0, dy != 0);
				foreach(LayoutViewCard card in VisibleCards) {
					card.IsPartiallyVisible = CalcCardIsPartiallyVisible(viewRect, card);   
				}
			}
		}
		void CorrectAutoPanByScroll(Rectangle viewRect, LayoutViewCard firstCard, LayoutViewCard lastCard) {
			bool fLastVisibleCardIsLast = (View.GetVisibleIndex(lastCard.RowHandle) == View.RecordCount - 1);
			bool fFirstVisibleCardIsFirst = (View.GetVisibleIndex(firstCard.RowHandle) == 0);
			if(fLastVisibleCardIsLast && fFirstVisibleCardIsFirst) {
				if(CalcCardIsPartiallyVisible(viewRect, firstCard)) View.fAutoPanForward = true;
				LayoutViewCard[] lastCards = GetLastCards(lastCard);
				foreach(LayoutViewCard last in lastCards) {
					if(CalcCardIsPartiallyVisible(viewRect, last)) {
						View.fAutoPanBackward = View.VisibleRecordIndex != 0;
						if(View.fAutoPanBackward)
							View.fAutoPanForward = false;
					}
				}
				View.fAutoPanByScroll = View.fAutoPanForward || View.fAutoPanBackward;
			}
			else {
				bool fFirstVisibleCardIsArrangeOrigin = (View.GetVisibleIndex(firstCard.RowHandle) == View.VisibleRecordIndex);
				if(View.fAutoPanForward && fLastVisibleCardIsLast && fFirstVisibleCardIsArrangeOrigin) {
					LayoutViewCard[] lastCards = GetLastCards(lastCard);
					foreach(LayoutViewCard last in lastCards) {
						if(CalcCardIsPartiallyVisible(viewRect, last)) {
							View.fAutoPanByScroll = false;
							break;
						}
					}
				}
			}
		}
		protected internal virtual LayoutViewCard[] GetTouchScrollCards(bool forward) {
			return GetTouchScrollCardsCore(forward, VisibleCards);
		}
		protected LayoutViewCard[] GetTouchScrollCardsCore(bool forward, List<LayoutViewCard> cards) {
			return forward ?
				new LayoutViewCard[] { cards[0] } :
				new LayoutViewCard[] { cards[cards.Count - 1] };
		}
		protected virtual LayoutViewCard[] GetLastCards(LayoutViewCard lastCard) {
			return new LayoutViewCard[] { lastCard };
		}
		protected internal bool AutoPanFieldCore(LayoutViewField field) {
			Rectangle viewRect = new Rectangle(
					ViewInfo.ViewRects.CardsRect.Left + MinArrangeHSpacing / 2,
					ViewInfo.ViewRects.CardsRect.Top + MinArrangeVSpacing / 2,
					ViewInfo.ViewRects.CardsRect.Width - MinArrangeHSpacing,
					ViewInfo.ViewRects.CardsRect.Height - MinArrangeVSpacing
				);
			int dx = 0; int dy = 0;
			bool fAutoPanPerformed = CheckAutoPanByFocusedItem(viewRect, field, ref dx, ref dy);
			if(fAutoPanPerformed) {
				PanCardsAreaIfNeed(dx, dy, dx != 0, dy != 0);
			}
			return fAutoPanPerformed;
		}
		bool CardIsOutOfRect(Rectangle rect, LayoutViewCard card, int dx, int dy) {
			Rectangle cardRect = new Rectangle(card.Position, card.Size + new Size(dx, dy));
			return !rect.IntersectsWith(cardRect);
		}
		bool CheckAutoPanByFocusedCard(Rectangle rect, LayoutViewCard focusedCard, ref int dx, ref int dy) {
			if(View.IsAutoPanByFocusedCardLocked) return false;
			Rectangle cardRect = new Rectangle(focusedCard.Position, focusedCard.Size);
			int[] w = (cardRect.Width > rect.Width) && UseLeftCardAlignment(focusedCard) ?
				new int[] { rect.Left - cardRect.Left, cardRect.Right - rect.Right } :
				new int[] { cardRect.Left - rect.Left, rect.Right - cardRect.Right };
			int[] h = (cardRect.Height > rect.Height) && UseTopCardAlignment(focusedCard) ?
				new int[] { rect.Top - cardRect.Top, cardRect.Bottom - rect.Bottom } :
				new int[] { cardRect.Top - rect.Top, rect.Bottom - cardRect.Bottom };
			bool fNeedAutoScroll = (w[0] * w[1] < 0) || (h[0] * h[1] < 0);
			if(fNeedAutoScroll) {
				if(w[0] * w[1] < 0) dx += (w[0] < 0) ? -w[0] : w[1];
				if(h[0] * h[1] < 0) dy += (h[0] < 0) ? -h[0] : h[1];
			}
			return fNeedAutoScroll;
		}
		protected bool UseTopCardAlignment(LayoutViewCard card) {
			var bInfo = card.ViewInfo.BorderInfo;
			if(bInfo == null || bInfo.BorderStyle == XtraEditors.Controls.BorderStyles.NoBorder)
				return true;
			return bInfo.CaptionLocation != Utils.Locations.Bottom;
		}
		protected bool UseLeftCardAlignment(LayoutViewCard card) {
			var bInfo = card.ViewInfo.BorderInfo;
			if(bInfo == null || bInfo.BorderStyle == XtraEditors.Controls.BorderStyles.NoBorder)
				return true;
			return bInfo.CaptionLocation != Utils.Locations.Right;
		}
		protected virtual bool IsHorizontal(bool partialWidth, bool partialHeight) {
			return true;
		}
		bool CheckAutoPanByScroll(Rectangle rect, ref int dx, ref int dy) {
			if(View.IsAutoPanByScrollLocked) return false;
			bool fNeedAutoScroll = View.fAutoPanByScroll;
			if(fNeedAutoScroll) {
				Rectangle cardsArea = CalcCardsArea();
				int w = 0; int h = 0;
				if(View.fAutoPanForward) {
					w = rect.Left - cardsArea.Left;
					h = rect.Top - cardsArea.Top;
				}
				if(View.fAutoPanBackward) {
					w = rect.Right - cardsArea.Right;
					h = rect.Bottom - cardsArea.Bottom;
				}
				bool partialWidth = cardsArea.Width > rect.Width;
				bool partialHeight = cardsArea.Height > rect.Height;
				dx += IsHorizontal(partialWidth, partialHeight) ? w : 0;
				dy += IsHorizontal(partialWidth, partialHeight) ? 0 : h;
				return dx != 0 || dy != 0;
			}
			return fNeedAutoScroll;
		}
		bool CheckAutoPanByPartialCard(Rectangle rect, LayoutViewCard firstCard, LayoutViewCard lastCard, ref int dx, ref int dy) {
			if(View.IsAutoPanByPartialCardLocked || View.PartialCardsSimpleScrolling) return false;
			bool fNeedAutoScroll = (lastCard.RowHandle == View.RecordCount - 1 && firstCard.RowHandle != 0);
			if(fNeedAutoScroll) {
				int w = rect.Right - (lastCard.Position.X + lastCard.Width);
				int h = rect.Bottom - (lastCard.Position.Y + lastCard.Height);
				dx += (firstCard.Bounds.Left + w < 0) ? w : 0;
				dy += (firstCard.Bounds.Top + h < 0) ? h : 0;
			}
			return fNeedAutoScroll;
		}
		bool CheckAutoPanByFocusedItem(Rectangle rect, BaseLayoutItem focusedItem, ref int dx, ref int dy) {
			if(focusedItem == null) return false;
			Rectangle itemRect = focusedItem.ViewInfo.BoundsRelativeToControl;
			itemRect.Width = Math.Min(rect.Width, itemRect.Width);
			itemRect.Height = Math.Min(rect.Height, itemRect.Height);
			int[] w = new int[] { itemRect.Left - rect.Left, rect.Right - itemRect.Right };
			int[] h = new int[] { itemRect.Top - rect.Top, rect.Bottom - itemRect.Bottom };
			bool fNeedAutoScroll = (w[0] * w[1] < 0) || (h[0] * h[1] < 0);
			if(fNeedAutoScroll) {
				if(w[0] * w[1] < 0) dx += (w[0] < 0) ? -w[0] : w[1];
				if(h[0] * h[1] < 0) dy += (h[0] < 0) ? -h[0] : h[1];
			}
			return fNeedAutoScroll;
		}
		protected internal void PanCardsAreaIfNeed(int dx, int dy, bool horizontal, bool vertical) {
			if(horizontal || vertical) {
				Rectangle viewRect = ViewInfo.ViewRects.CardsRect;
				int leftBound = viewRect.Left + MinArrangeHSpacing / 2;
				int rightBound = viewRect.Right - MinArrangeHSpacing / 2;
				int topBound = viewRect.Top + MinArrangeVSpacing / 2;
				int bottomBound = viewRect.Bottom - MinArrangeVSpacing / 2;
				Rectangle cardsArea = CalcCardsArea();
				int left = cardsArea.Left;
				int right = cardsArea.Right;
				int top = cardsArea.Top;
				int bottom = cardsArea.Bottom;
				int newDX = 0;
				if(horizontal) {
					if(dx > 0) {
						newDX = Math.Min(leftBound, left + dx) - left;
					}
					else {
						newDX = Math.Max(rightBound, right + dx) - right;
					}
				}
				int newDY = 0;
				if(vertical) {
					if(dy > 0) {
						newDY = Math.Min(topBound, top + dy) - top;
					}
					else {
						newDY = Math.Max(bottomBound, bottom + dy) - bottom;
					}
				}
				Size offset = new Size(newDX, newDY);
				foreach(LayoutViewCard card in VisibleCards) {
					card.Position += offset;
					UpdateCardPosition(card);
				}
				foreach(Line line in CardLines) {
					line.BeginPoint += offset;
					line.EndPoint += offset;
				}
				foreach(Line line in Lines) {
					line.BeginPoint += offset;
					line.EndPoint += offset;
				}
			}
		}
		protected virtual bool CalcCardIsPartiallyVisible(Rectangle rect, LayoutViewCard card) {
			return false;
		}
		public virtual bool CheckTouchScroll(int touchOffset, out int maxOffset) {
			RangeInfo start = new RangeInfo();
			RangeInfo end = new RangeInfo();
			CheckTouchScrollRanges(ref start, ref end);
			int spacing = GetTouchScrollSpacing(touchOffset > 0);
			if(touchOffset > 0)
				maxOffset = IsFirstCardInView() ? spacing : end.Length + spacing;
			else
				maxOffset = IsLastCardInView() ? spacing : start.Length + spacing;
			return (touchOffset > 0) ? (maxOffset < touchOffset) : (maxOffset < -touchOffset);
		}
		protected virtual int GetTouchScrollSpacing(bool forward) {
			return Round(NavigationHScrollNeed ? ArrangeCache.fCardHSpace : ArrangeCache.fCardVSpace);
		}
		public virtual int CalcTouchScrollChange(int touchOffset) {
			return 1;
		}
		protected virtual void CheckTouchScrollRanges(ref RangeInfo start, ref RangeInfo end) {
			if(NavigationHScrollNeed)
				CheckRow(ArrangeCache.Cards, ref start, ref end);
			else
				CheckColumn(ArrangeCache.Cards, ref start, ref end);
		}
		protected void CheckRow(IList<LayoutViewCard> cards, ref RangeInfo start, ref RangeInfo end) {
			if(cards.Count == 0) return;
			start.Union(new RangeInfo(cards[0].Bounds.Left, cards[0].Bounds.Right));
			end.Union(new RangeInfo(cards[cards.Count - 1].Bounds.Left, cards[cards.Count - 1].Bounds.Right));
		}
		protected void CheckColumn(IList<LayoutViewCard> cards, ref RangeInfo start, ref RangeInfo end) {
			if(cards.Count == 0) return;
			start.Union(new RangeInfo(cards[0].Bounds.Top, cards[0].Bounds.Bottom));
			end.Union(new RangeInfo(cards[cards.Count - 1].Bounds.Top, cards[cards.Count - 1].Bounds.Bottom));
		}
		protected Rectangle CalcCardsArea() {
			if(VisibleCards.Count == 0) return Rectangle.Empty;
			int left = VisibleCards[0].Position.X;
			int right = left;
			int top = VisibleCards[0].Position.Y;
			int bottom = top;
			foreach(LayoutViewCard card in VisibleCards) {
				left = Math.Min(left, card.Position.X);
				right = Math.Max(right, card.Position.X + card.Size.Width);
				top = Math.Min(top, card.Position.Y);
				bottom = Math.Max(bottom, card.Position.Y + card.Size.Height);
			}
			return new Rectangle(left, top, right - left, bottom - top);
		}
		protected void CollectSingleCard(int currentRecordIndex) {
			if(View.RecordCount > 0) TryInsertCardInEmptySpaceAndCache(currentRecordIndex);
		}
		protected void CollectAnyCards(int currentRecordIndex) {
			if(UseWholeCards)
				CollectAnyCards(currentRecordIndex, View.internalArrangeFromLeftToRight);
			else
				CollectAnyCards(currentRecordIndex, View.PartialCardsSimpleScrolling || View.internalArrangeFromLeftToRight);
		}
		protected void CollectAnyCards(int currentRecordIndex, bool fLeftToRight) {
			if(fLeftToRight) CollectAnyCardsFromLeftToRight(currentRecordIndex);
			else CollectAnyCardsFromRightToLeft(currentRecordIndex);
		}
		protected void CollectAnyCardsFromLeftToRight(int currentRecordIndex) {
			int iLastIndex = currentRecordIndex;
			bool fCanTryAddLast = (iLastIndex < View.RecordCount);
			while(true) {
				if(fCanTryAddLast) {
					if(TryInsertCardInEmptySpaceAndCache(iLastIndex) == null) break;
					if(iLastIndex < (ViewInfo.Owner.RecordCount - 1)) { iLastIndex++; } else fCanTryAddLast = false;
				}
				if(!fCanTryAddLast) break;
			}
			if(!View.PartialCardsSimpleScrolling && currentRecordIndex > 0 && CheckCanCollectAnyCardsYet()) 
				CollectAnyCardsFromRightToLeft(currentRecordIndex - 1);
		}
		protected virtual bool CheckCanCollectAnyCardsYet() {
			return ArrangeCache.EmptySpace.Width > 0 || ArrangeCache.EmptySpace.Height > 0;
		}
		protected void CollectAnyCardsFromRightToLeft(int currentRecordIndex) {
			int iFirstIndex = currentRecordIndex;
			bool fCanTryAddFirst = View.RecordCount > 0;
			while(true) {
				if(fCanTryAddFirst) {
					if(TryInsertCardInEmptySpaceAndCache(iFirstIndex) == null) break;
					if(iFirstIndex > 0) { iFirstIndex--; } else fCanTryAddFirst = false;
				}
				if(!fCanTryAddFirst) break;
			}
		}
		protected void CollectAnyCardsFromCenter(int currentRecordIndex) {
			int iFirstIndex = currentRecordIndex;
			int iLastIndex = iFirstIndex + 1;
			bool fCanTryAddFirst = (View.RecordCount > 0);
			bool fCanTryAddLast = (iLastIndex < View.RecordCount);
			while(true) {
				if(fCanTryAddFirst) {
					if(TryInsertCardInEmptySpaceAndCache(iFirstIndex) == null) break;
					if(iFirstIndex > 0) { iFirstIndex--; } else fCanTryAddFirst = false;
				}
				if(fCanTryAddLast) {
					if(TryInsertCardInEmptySpaceAndCache(iLastIndex) == null) break;
					if(iLastIndex < (ViewInfo.Owner.RecordCount - 1)) { iLastIndex++; } else fCanTryAddLast = false;
				}
				if(!fCanTryAddFirst && !fCanTryAddLast) break;
			}
		}
		protected List<LayoutViewCard> MakeListToArrange(int recordIndex, LayoutViewCard card) {
			List<LayoutViewCard> tryArrangeList = new List<LayoutViewCard>();
			if(ArrangeCache.RecordCount > 0) {
				if(recordIndex < ArrangeCache.Cards[0].RowHandle) {
					tryArrangeList.Add(card);
					tryArrangeList.AddRange(ArrangeCache.Cards);
				}
				else {
					tryArrangeList.AddRange(ArrangeCache.Cards);
					tryArrangeList.Add(card);
				}
			}
			else {
				tryArrangeList.Add(card);
			}
			return tryArrangeList;
		}
		protected bool IsLastCard(LayoutViewCard card) {
			return View.GetVisibleIndex(card.RowHandle) == View.RecordCount - 1;
		}
		protected bool IsFirstCard(LayoutViewCard card) {
			return View.GetVisibleIndex(card.RowHandle) == 0;
		}
		protected virtual bool IsFirstCardInView() {
			return IsFirstCard(ArrangeCache.Cards[0]);
		}
		protected virtual bool IsLastCardInView() {
			return IsLastCard(ArrangeCache.Cards[ArrangeCache.Cards.Count - 1]);
		}
		protected bool AllCardsInView(IList<LayoutViewCard> cards) {
			return IsFirstCard(cards[0]) && IsLastCard(cards[cards.Count - 1]);
		}
		protected bool IsBoundCard(LayoutViewCard card, bool allCardsInView) {
			return allCardsInView && (IsLastCard(card) || IsFirstCard(card));
		}
		protected internal Bitmap GetCardImage(LayoutViewCard card) {
			card.ViewInfo.CalculateViewInfo();
			card.UpdateChildrenToBeRefactored();
			Bitmap result = new Bitmap(card.Size.Width, card.Size.Height);
			var cardPainter = ((ILayoutControl)View).PaintStyle.GetPainter(card);
			card.Owner = View;
			using(Graphics imgGraphics = Graphics.FromImage(result)) {
				imgGraphics.InterpolationMode = View.OptionsCarouselMode.InterpolationMode;
				Rectangle imgRect = new Rectangle(0, 0, result.Width, result.Height);
				using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(imgGraphics, imgRect)) {
					using(GraphicsCache bufferedCache = new GraphicsCache(new DXPaintEventArgs(bg.Graphics, imgRect))) {
						View.PaintAppearance.ViewBackground.DrawBackground(bg.Graphics, bufferedCache, imgRect, false);
						View.DrawCard = card;
						card.ViewInfo.Cache = bufferedCache;
						View.ViewInfo.UpdateBeforePaint(card);
						cardPainter.DrawObject(card.ViewInfo);
						card.ViewInfo.Cache = null;
						card.Owner = null;
						View.DrawCard = null;
					}
					bg.Render();
				}
			}
			return result;
		}
	}
	public class LayoutViewSingleRecordMode : LayoutViewBaseMode {
		public LayoutViewSingleRecordMode(ILayoutViewInfo ownerView) : base(ownerView) { }
		public override LayoutViewMode ViewMode {
			get { return LayoutViewMode.SingleRecord; }
		}
		public override bool NavigationHScrollNeed { get { return true; } }
		public override bool NavigationVScrollNeed { get { return false; } }
		protected override void CollectAllCardsWhichMayBeVisible(int currentRecordIndex) {
			CollectSingleCard(currentRecordIndex);
		}
		protected override Size GetCollapsedCardSizeDefault() {
			Size collapsedSize = base.GetCollapsedCardSizeDefault();
			return new Size(View.CardMinSize.Width, collapsedSize.Height);
		}
		protected override void AutoPanCardsArea() {
		}
		public override bool CanStretchCardByWidth {
			get { return GetHStretch(); }
		}
		protected override Size GetEmptySpace() {
			Size cardSize = VisibleCards.Count > 0 ? VisibleCards[0].Size : Size.Empty;
			return ViewInfo.ViewRects.CardsRect.Size - cardSize;
		}
		protected override int GetRealHeight() {
			return VisibleCards.Count > 0 ? VisibleCards[0].Height + MinArrangeVSpacing * 2 : 0;
		}
		protected override void ArrangeVisibleCards() {
			ArrangeCard(VisibleCards[0], ViewInfo.ViewRects.CardsRect);
		}
		void ArrangeCard(LayoutViewCard card, Rectangle rect) {
			Size emptySpace = GetEmptySpace(card, rect.Size);
			ArrangeCache.fCardHSpace = GetHSpace(emptySpace.Width);
			ArrangeCache.fCardVSpace = GetVSpace(emptySpace.Height);
			float fCardLeft = GetCardLeft(rect.Left, emptySpace.Width, ArrangeCache.fCardHSpace);
			float fCardTop = GetCardTop(rect.Top, emptySpace.Height, ArrangeCache.fCardVSpace);
			card.Position = new Point(Round(fCardLeft), Round(fCardTop));
			card.VisibleRow = card.VisibleColumn = card.VisibleIndex = 0;
		}
		Size GetEmptySpace(LayoutViewCard card, Size size) {
			return size - card.Size;
		}
		protected virtual bool GetHStretch() {
			return View.OptionsSingleRecordMode.StretchCardToViewWidth;
		}
		protected virtual bool GetVStretch() {
			return View.OptionsSingleRecordMode.StretchCardToViewHeight;
		}
		float GetHSpace(float emptyWidth) {
			float fHSpace = GetHStretch() ? 0 : MinArrangeHSpacing;
			switch(View.OptionsView.ContentAlignment) {
				case ContentAlignment.TopCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.BottomCenter:
					fHSpace = Math.Max(emptyWidth * 0.5f, fHSpace);
					break;
			}
			return fHSpace;
		}
		float GetVSpace(float emptyHeight) {
			float fVSpace = GetVStretch() ? 0 : MinArrangeVSpacing;
			switch(View.OptionsView.ContentAlignment) {
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleRight:
					fVSpace = Math.Max(emptyHeight * 0.5f, fVSpace);
					break;
			}
			return fVSpace;
		}
		float GetCardLeft(int left, int empty, float fHSpace) {
			float fCardLeft = (float)left + fHSpace;
			switch(View.OptionsView.ContentAlignment) {
				case ContentAlignment.TopLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.BottomLeft:
					fCardLeft = (float)left + fHSpace * 0.5f;
					break;
				case ContentAlignment.TopRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.BottomRight:
					fCardLeft = (float)(left + empty) - fHSpace * 0.5f;
					break;
			}
			if(empty < 0) fCardLeft = (float)left + fHSpace * 0.5f;
			return fCardLeft;
		}
		float GetCardTop(int top, int space, float fVSpace) {
			float fCardTop = (float)top + fVSpace;
			switch(View.OptionsView.ContentAlignment) {
				case ContentAlignment.TopLeft:
				case ContentAlignment.TopCenter:
				case ContentAlignment.TopRight:
					fCardTop = (float)top + MinArrangeVSpacing;
					break;
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomRight:
					fCardTop = (float)(top + space - MinArrangeVSpacing);
					break;
			}
			return fCardTop;
		}
		protected override Size CheckCardMinSize(Size cardMinSize, bool collapsed) {
			Size viewSize = ViewInfo.ViewRects.CardsRect.Size;
			int w = cardMinSize.Width;
			int h = collapsed ? CollapsedCardSize.Height : cardMinSize.Height;
			if(GetHStretch()) w = Math.Max(viewSize.Width, w);
			if(GetVStretch()) h = Math.Max(viewSize.Height, h);
			return new Size(w, h);
		}
		protected override void ArrangePostProcess() {
			bool horzAlign = GetVStretch();
			bool vertAlign = GetHStretch();
			if(vertAlign && horzAlign || VisibleCards.Count == 0) return;
			CardsAlignment optionCardAlign = View.OptionsSingleRecordMode.CardAlignment;
			Rectangle viewRect = ViewInfo.ViewRects.CardsRect;
			LayoutViewCard card = VisibleCards[0];
			if(horzAlign) {
				int iNearPos = viewRect.Left;
				int iCenterPos = viewRect.Left + (viewRect.Width - card.Width) / 2;
				int iFarPos = viewRect.Right - card.Width;
				switch(optionCardAlign) {
					case CardsAlignment.Near: card.Position = new Point(iNearPos, card.Position.Y); break;
					case CardsAlignment.Center: card.Position = new Point(iCenterPos, card.Position.Y); break;
					case CardsAlignment.Far: card.Position = new Point(iFarPos, card.Position.Y); break;
				}
			}
			if(vertAlign) {
				int iNearPos = viewRect.Top;
				int iCenterPos = viewRect.Top + (viewRect.Height - card.Height) / 2;
				int iFarPos = viewRect.Bottom - card.Height;
				switch(optionCardAlign) {
					case CardsAlignment.Near: card.Position = new Point(card.Position.X, iNearPos); break;
					case CardsAlignment.Center: card.Position = new Point(card.Position.X, iCenterPos); break;
					case CardsAlignment.Far: card.Position = new Point(card.Position.X, iFarPos); break;
				}
			}
		}
	}
	public class LayoutViewRowMode : LayoutViewBaseMode {
		public LayoutViewRowMode(ILayoutViewInfo ownerView) : base(ownerView) { }
		public override LayoutViewMode ViewMode { get { return LayoutViewMode.Row; } }
		public override bool NavigationHScrollNeed { get { return true; } }
		public override bool NavigationVScrollNeed { get { return false; } }
		protected override void CollectAllCardsWhichMayBeVisible(int currentRecordIndex) {
			CollectAnyCards(currentRecordIndex);
		}
		protected virtual bool CanStretchCards() {
			return View.OptionsMultiRecordMode.StretchCardToViewHeight;
		}
		protected override bool CalcCardIsPartiallyVisible(Rectangle rect, LayoutViewCard card) {
			return IsPartiallyVisible(rect, true, card);
		}
		protected override int CheckMinScrollChangeCore(bool forward, LayoutViewCard nextCard) {
			int scrollWidth = nextCard.Size.Width;
			int startWidth = 0;
			if(forward) {
				for(int i = 0; i < VisibleCards.Count; i++) {
					if(startWidth > scrollWidth)
						return VisibleCards[0].RowHandle + i;
					startWidth += VisibleCards[i].Size.Width;
				}
			}
			else {
				for(int i = 0; i < VisibleCards.Count; i++) {
					if(startWidth > scrollWidth)
						return VisibleCards[VisibleCards.Count].RowHandle + i;
					startWidth += VisibleCards[VisibleCards.Count - i].Size.Width;
				}
			}
			return base.CheckMinScrollChangeCore(forward, nextCard);
		}
		public override int GetScrollableCardsCount() {
			int scrollableCards = VisibleCards.Count;
			if(scrollableCards > 1) {
				Rectangle rect = ViewInfo.ViewRects.CardsRect;
				LayoutViewCard first = VisibleCards[0];
				LayoutViewCard last = VisibleCards[scrollableCards - 1];
				if(IsPartiallyVisible(rect, true, first)) scrollableCards--;
				if(first != last && IsPartiallyVisible(rect, true, last)) scrollableCards--;
			}
			return scrollableCards;
		}
		protected override Size CheckCardMinSize(Size cardMinSize, bool collapsed) {
			Size viewSize = ViewInfo.ViewRects.CardsRect.Size;
			int w = collapsed ? CollapsedCardSize.Width : cardMinSize.Width;
			int h = cardMinSize.Height;
			if(CanStretchCards()) h = Math.Max(viewSize.Height, h);
			return new Size(w, h);
		}
		protected override Size GetEmptySpace() {
			int w = 0;
			float spacing = GetSpacing(ArrangeCache.Cards.Count, MinArrangeHSpacing + SeparatorWidth);
			foreach(LayoutViewCard c in ArrangeCache.Cards) w += c.Size.Width;
			return Size.Subtract(ViewInfo.ViewRects.CardsRect.Size, new Size(Round((float)w + spacing), 0));
		}
		protected override int GetRealHeight() {
			int h = 0;
			int vSpace = MinArrangeVSpacing;
			foreach(LayoutViewCard card in ArrangeCache.Cards)
				h = Math.Max(h, card.Height);
			return h > 0 ? h + vSpace * 2 : 0;
		}
		protected override bool CheckMaxCardRowsColumns() {
			bool result = true;
			if(MaxCardColumns > 0) result &= MaxCardColumns > ArrangeCache.Cards.Count;
			return result;
		}
		protected override bool CheckCanCollectAnyCardsYet() {
			return ArrangeCache.EmptySpace.Width > 0;
		}
		protected override bool CanInsertCardByCurrentArrangeStrategy(Size space, LayoutViewCard card) {
			if(!card.Expanded) card.Width = Math.Max(card.MinSize.Width, card.Width);
			return base.CanInsertCardByCurrentArrangeStrategy(space, card);
		}
		protected override bool CheckWholeCardPlacementAbility(Size space, LayoutViewCard card) {
			return CheckDim(card.Size.Width, space.Width, MinArrangeHSpacing + SeparatorWidth) &&
				  (card.Size.Height < (space.Height - MinArrangeVSpacing));
		}
		protected override bool CheckCardPlacementAbilityWithCardPan(Size space, LayoutViewCard card) {
			if(ArrangeCache.RecordCount == 0) return base.CheckCardPlacementAbilityWithCardPan(space, card);
			if(UseWholeCards) {
				return CheckDim(card.Size.Width, space.Width, MinArrangeHSpacing + SeparatorWidth);
			}
			return space.Width > MinVisibleCardThickness;
		}
		public override CardCollapsingMode CardCollapsingMode {
			get { return CardCollapsingMode.Vertical; }
		}
		public override Size CollapsedCardSize {
			get {
				Size defSize = GetCollapsedCardSizeDefault();
				return new Size(defSize.Width, Math.Max(ArrangeCache.iMaxCardHeight, defSize.Height));
			}
		}
		protected override Size GetCollapsedCardSizeDefault() {
			Size collapsedSize = base.GetCollapsedCardSizeDefault();
			return new Size(collapsedSize.Height, View.CardMinSize.Height);
		}
		protected override void ArrangeVisibleCards() {
			ArrangeRow(VisibleCards, ViewInfo.ViewRects.CardsRect, true, 0, 0);
		}
		protected override void ArrangePostProcess() {
			SizeF lineOffset = new SizeF(ArrangeCache.fCardHSpace * 0.5f, 0);
			PostProcessRow(
					VisibleCards, ViewInfo.ViewRects.CardsRect, ArrangeCache.iMaxCardHeight, lineOffset
				);
			ArrangeCache.PanForce = ArrangeCache.iMaxCardHeight > ViewInfo.ViewRects.CardsRect.Height;
		}
		protected void ArrangeRow(List<LayoutViewCard> cards, Rectangle rowRect, bool arrangeVertical, int rowNumber, int firstCard) {
			Size emptySpace = GetEmptySpace(cards, rowRect.Size);
			ArrangeCache.fCardHSpace = GetHSpace(cards.Count, emptySpace.Width);
			ArrangeCache.fCardVSpace = arrangeVertical ? GetVSpace(1, emptySpace.Height) : 0;
			float spacing = GetSpacing(cards.Count, ArrangeCache.fCardHSpace);
			float fCardLeft = GetCardsLeft(rowRect, UseWholeCards ? emptySpace.Width : -1, spacing, ArrangeCache.fCardHSpace);
			float fCardTop = arrangeVertical ? GetCardsTop(1, rowRect.Top, emptySpace.Height, ArrangeCache.fCardVSpace) : rowRect.Top;
			for(int i = 0; i < cards.Count; i++) {
				LayoutViewCard card = cards[i];
				card.Position = new Point(Round(fCardLeft), Round(fCardTop));
				fCardLeft += ((float)card.Width + ArrangeCache.fCardHSpace);
				card.VisibleIndex = firstCard + i;
				card.VisibleColumn = i;
				card.VisibleRow = rowNumber;
			}
		}
		protected Rectangle PostProcessRow(List<LayoutViewCard> cards, Rectangle rowRect, int maxCardHeight, SizeF lineOffset) {
			int counter = 0;
			foreach(LayoutViewCard card in cards) {
				if(!card.Expanded) card.Height = maxCardHeight;
				switch(View.OptionsView.CardsAlignment) {
					case CardsAlignment.Near:
						card.Position = new Point(card.Position.X, card.Position.Y); break;
					case CardsAlignment.Far:
						card.Position = new Point(card.Position.X, card.Position.Y + maxCardHeight - card.Height);
						break;
					case CardsAlignment.Center:
						card.Position = new Point(card.Position.X, card.Position.Y + (maxCardHeight - card.Height) / 2);
						break;
				}
				if(cards.Count > 1 && counter + 1 < cards.Count) {
					Point pt1 = new Point(Round((float)card.Bounds.Right + lineOffset.Width), Round((float)rowRect.Top - lineOffset.Height));
					Point pt2 = new Point(Round((float)card.Bounds.Right + lineOffset.Width), Round((float)rowRect.Bottom + lineOffset.Height));
					CardLines.Add(new Line(pt1, pt2, true));
				}
				counter++;
			}
			return rowRect;
		}
		protected Size GetRowSize(IList<LayoutViewCard> cards) {
			int w = 0; int hMax = 0;
			foreach(LayoutViewCard card in cards) {
				w += card.Size.Width;
				hMax = Math.Max(hMax, card.Size.Height);
			}
			return new Size(w, hMax);
		}
		Size GetEmptySpace(IList<LayoutViewCard> cards, Size rowSize) {
			Size rs = GetRowSize(cards);
			return new Size(rowSize.Width - rs.Width, rowSize.Height - rs.Height);
		}
		protected float GetHSpace(int cardsCount, float emptyWidth) {
			float fHSpace = MinArrangeHSpacing + SeparatorWidth;
			if(UseWholeCards) {
				switch(View.OptionsView.ContentAlignment) {
					case ContentAlignment.TopCenter:
					case ContentAlignment.MiddleCenter:
					case ContentAlignment.BottomCenter:
						fHSpace = Math.Max(emptyWidth / ((float)cardsCount + 1f), fHSpace);
						break;
				}
			}
			return fHSpace;
		}
		protected float GetVSpace(int rowCount, float emptyHeight) {
			float fVSpace = rowCount > 1 ? MinArrangeVSpacing + SeparatorWidth : MinArrangeVSpacing;
			if(CanStretchCards()) {
				if(rowCount == 1) return 0;
			}
			switch(View.OptionsView.ContentAlignment) {
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleRight:
					fVSpace = Math.Max(emptyHeight / ((float)rowCount + 1f), fVSpace);
					break;
			}
			return fVSpace;
		}
		protected float GetCardsLeft(Rectangle rowRect, int empty, float spacing, float fHSpace) {
			if(empty < 0) 
				return (float)rowRect.Left + fHSpace * 0.5f;
			float fCardLeft = (float)rowRect.Left + fHSpace;
			switch(View.OptionsView.ContentAlignment) {
				case ContentAlignment.TopLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.BottomLeft:
					fCardLeft = (float)rowRect.Left + fHSpace * 0.5f;
					break;
				case ContentAlignment.TopRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.BottomRight:
					float fCards = (float)rowRect.Width - empty;
					fCardLeft = (float)(rowRect.Right - (fCards + spacing)) - fHSpace * 0.5f;
					break;
			}
			return fCardLeft;
		}
		protected float GetCardsTop(int numRows, int rowTop, int space, float fVSpace) {
			float fCardTop = (float)rowTop + fVSpace;
			switch(View.OptionsView.ContentAlignment) {
				case ContentAlignment.TopLeft:
				case ContentAlignment.TopCenter:
				case ContentAlignment.TopRight:
					fCardTop = (float)rowTop + MinArrangeVSpacing;
					break;
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomRight:
					fCardTop = (float)(rowTop + space - MinArrangeVSpacing) - (float)(numRows - 1) * fVSpace;
					break;
			}
			return fCardTop;
		}
	}
	public class LayoutViewColumnMode : LayoutViewBaseMode {
		public LayoutViewColumnMode(ILayoutViewInfo ownerView) : base(ownerView) { }
		public override LayoutViewMode ViewMode { get { return LayoutViewMode.Column; } }
		public override bool NavigationHScrollNeed { get { return false; } }
		public override bool NavigationVScrollNeed { get { return true; } }
		public override CardCollapsingMode CardCollapsingMode {
			get { return CardCollapsingMode.Horisontal; }
		}
		protected override bool IsHorizontal(bool partialWidth, bool partialHeight) {
			return false;
		}
		protected override void CollectAllCardsWhichMayBeVisible(int currentRecordIndex) {
			CollectAnyCards(currentRecordIndex);
		}
		protected override bool CalcCardIsPartiallyVisible(Rectangle rect, LayoutViewCard card) {
			return IsPartiallyVisible(rect, false, card);
		}
		protected override int CheckMinScrollChangeCore(bool forward, LayoutViewCard nextCard) {
			int scrollHeight = nextCard.Size.Height;
			int startHeight = 0;
			if(forward) {
				for(int i = 0; i < VisibleCards.Count; i++) {
					if(startHeight > scrollHeight)
						return VisibleCards[0].RowHandle + i;
					startHeight += VisibleCards[i].Size.Height;
				}
			}
			else {
				for(int i = 0; i < VisibleCards.Count; i++) {
					if(startHeight > scrollHeight)
						return VisibleCards[VisibleCards.Count].RowHandle + i;
					startHeight += VisibleCards[VisibleCards.Count-i].Size.Height;
				}
			}
			return base.CheckMinScrollChangeCore(forward, nextCard);
		}
		public override int GetScrollableCardsCount() {
			int scrollableCards = VisibleCards.Count;
			if(scrollableCards > 1) {
				Rectangle rect = ViewInfo.ViewRects.CardsRect;
				LayoutViewCard first = VisibleCards[0];
				LayoutViewCard last = VisibleCards[scrollableCards - 1];
				if(IsPartiallyVisible(rect, false, first)) scrollableCards--;
				if(first != last && IsPartiallyVisible(rect, false, last)) scrollableCards--;
			}
			return scrollableCards;
		}
		protected override bool CheckCanCollectAnyCardsYet() {
			return ArrangeCache.EmptySpace.Height > 0;
		}
		public override bool CanStretchCardByWidth {
			get { return View.OptionsMultiRecordMode.StretchCardToViewWidth; }
		}
		protected virtual bool CanStretchCards() {
			return View.OptionsMultiRecordMode.StretchCardToViewWidth;
		}
		protected override Size CheckCardMinSize(Size cardMinSize, bool collapsed) {
			Size viewSize = ViewInfo.ViewRects.CardsRect.Size;
			int w = cardMinSize.Width;
			int h = collapsed ? CollapsedCardSize.Height : cardMinSize.Height;
			if(CanStretchCards()) w = Math.Max(viewSize.Width, w);
			return new Size(w, h);
		}
		protected override bool CheckMaxCardRowsColumns() {
			bool result = true;
			if(MaxCardRows > 0) result &= MaxCardRows > ArrangeCache.Cards.Count;
			return result;
		}
		protected override Size GetCollapsedCardSizeDefault() {
			Size collapsedSize = base.GetCollapsedCardSizeDefault();
			return new Size(View.CardMinSize.Width, collapsedSize.Height);
		}
		protected override Size GetEmptySpace() {
			int h = 0; float spacing = GetSpacing(ArrangeCache.Cards.Count, MinArrangeVSpacing + SeparatorWidth);
			foreach(LayoutViewCard card in ArrangeCache.Cards) h += card.Size.Height;
			return Size.Subtract(ViewInfo.ViewRects.CardsRect.Size, new Size(0, Round((float)h + spacing)));
		}
		protected override int GetRealHeight() {
			int h = 0;
			int vSpace = MinArrangeVSpacing + SeparatorWidth;
			float spacing = GetSpacing(ArrangeCache.Cards.Count, vSpace);
			foreach(LayoutViewCard card in ArrangeCache.Cards)
				h += card.Size.Height;
			return Round((float)h + spacing) + ((h > 0) ? vSpace * 2 : 0);
		}
		protected override bool CanInsertCardByCurrentArrangeStrategy(Size space, LayoutViewCard card) {
			if(!card.Expanded) card.Height = Math.Max(card.MinSize.Height, card.Height);
			return base.CanInsertCardByCurrentArrangeStrategy(space, card);
		}
		protected override bool CheckWholeCardPlacementAbility(Size space, LayoutViewCard card) {
			return CheckDim(card.Size.Height, space.Height, MinArrangeVSpacing + SeparatorWidth) &&
				 (card.Size.Width < (space.Width - MinArrangeHSpacing));
		}
		protected override bool CheckCardPlacementAbilityWithCardPan(Size space, LayoutViewCard card) {
			if(ArrangeCache.RecordCount == 0) return base.CheckCardPlacementAbilityWithCardPan(space, card);
			if(UseWholeCards) {
				return CheckDim(card.Size.Height, space.Height, MinArrangeVSpacing + SeparatorWidth);
			}
			return space.Height > MinVisibleCardThickness;
		}
		public override Size CollapsedCardSize {
			get {
				Size defSize = GetCollapsedCardSizeDefault();
				return new Size(Math.Max(ArrangeCache.iMaxCardWidth, defSize.Width), defSize.Height);
			}
		}
		protected override void ArrangeVisibleCards() {
			ArrangeColumn(VisibleCards, ViewInfo.ViewRects.CardsRect, true, 0, 0);
		}
		protected override void ArrangePostProcess() {
			SizeF lineOffset = new SizeF(0, ArrangeCache.fCardVSpace * 0.5f);
			PostProcessColumn(
					VisibleCards, ViewInfo.ViewRects.CardsRect, ArrangeCache.iMaxCardWidth, lineOffset
				);
			ArrangeCache.PanForce = ArrangeCache.iMaxCardWidth > ViewInfo.ViewRects.CardsRect.Width;
		}
		protected void ArrangeColumn(List<LayoutViewCard> cards, Rectangle columnRect, bool arrangeHorizontal, int columnNumber, int firstCard) {
			Size space = GetEmptySpace(cards, columnRect.Size);
			ArrangeCache.fCardHSpace = arrangeHorizontal ? GetHSpace(1, space.Width) : 0;
			ArrangeCache.fCardVSpace = GetVSpace(cards.Count, space.Height);
			float spacing = GetSpacing(cards.Count, ArrangeCache.fCardVSpace);
			float fCardLeft = arrangeHorizontal ? GetCardsLeft(1, columnRect.Left, space.Width, ArrangeCache.fCardHSpace) : columnRect.Left;
			float fCardTop = GetCardsTop(columnRect, UseWholeCards ? space.Height : -1, spacing, ArrangeCache.fCardVSpace);
			for(int i = 0; i < cards.Count; i++) {
				LayoutViewCard card = cards[i];
				card.Position = new Point(Round(fCardLeft), Round(fCardTop));
				fCardTop += (card.Height + ArrangeCache.fCardVSpace);
				card.VisibleIndex = firstCard + i;
				card.VisibleRow = i;
				card.VisibleColumn = columnNumber;
			}
		}
		protected void PostProcessColumn(List<LayoutViewCard> cards, Rectangle columnRect, int maxCardWidth, SizeF lineOffset) {
			int counter = 0;
			foreach(LayoutViewCard card in cards) {
				if(!card.Expanded) card.Width = maxCardWidth;
				switch(View.OptionsView.CardsAlignment) {
					case CardsAlignment.Near:
						card.Position = new Point(card.Position.X, card.Position.Y);
						break;
					case CardsAlignment.Far:
						card.Position = new Point(card.Position.X + maxCardWidth - card.Width, card.Position.Y);
						break;
					case CardsAlignment.Center:
						card.Position = new Point(card.Position.X + (maxCardWidth - card.Width) / 2, card.Position.Y);
						break;
				}
				if(cards.Count > 1 && counter + 1 < cards.Count) {
					Point pt1 = new Point(Round((float)columnRect.Left - lineOffset.Width), Round((float)card.Bounds.Bottom + lineOffset.Height));
					Point pt2 = new Point(Round((float)columnRect.Right + lineOffset.Width), Round((float)card.Bounds.Bottom + lineOffset.Height));
					CardLines.Add(new Line(pt1, pt2, false));
				}
				counter++;
			}
		}
		protected Size GetEmptySpace(IList<LayoutViewCard> cards, Size columnSize) {
			int h = 0; int wMax = 0;
			foreach(LayoutViewCard card in cards) {
				h += card.Size.Height;
				wMax = Math.Max(wMax, card.Size.Width);
			}
			return new Size(columnSize.Width - wMax, columnSize.Height - h);
		}
		protected float GetHSpace(int columnCount, int emptyWidth) {
			float fHSpace = columnCount > 1 ? MinArrangeHSpacing + SeparatorWidth : MinArrangeHSpacing;
			if(CanStretchCards()) {
				if(columnCount == 1) return 0;
			}
			switch(View.OptionsView.ContentAlignment) {
				case ContentAlignment.TopCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.BottomCenter:
					fHSpace = Math.Max(emptyWidth / ((float)columnCount + 1f), fHSpace);
					break;
			}
			return fHSpace;
		}
		protected float GetVSpace(int cardsCount, int emptyHeight) {
			float fVSpace = MinArrangeVSpacing + SeparatorWidth;
			if(UseWholeCards) {
				switch(View.OptionsView.ContentAlignment) {
					case ContentAlignment.MiddleLeft:
					case ContentAlignment.MiddleCenter:
					case ContentAlignment.MiddleRight:
						fVSpace = Math.Max((float)emptyHeight / ((float)cardsCount + 1f), fVSpace);
						break;
				}
			}
			return fVSpace;
		}
		protected float GetCardsLeft(int numColumns, int columnLeft, int emptyWidth, float fHSpace) {
			float fCardLeft = (float)columnLeft + fHSpace;
			switch(View.OptionsView.ContentAlignment) {
				case ContentAlignment.TopLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.BottomLeft:
					fCardLeft = (float)columnLeft + (float)MinArrangeHSpacing;
					break;
				case ContentAlignment.TopRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.BottomRight:
					fCardLeft = (float)(columnLeft + emptyWidth - MinArrangeHSpacing) - (float)(numColumns - 1) * fHSpace;
					break;
			}
			return fCardLeft;
		}
		protected float GetCardsTop(Rectangle columnRect, int empty, float spacing, float fVSpace) {
			if(empty < 0)
				return (float)(float)columnRect.Top + fVSpace * 0.5f;
			float fCardTop = (float)columnRect.Top + fVSpace;
			switch(View.OptionsView.ContentAlignment) {
				case ContentAlignment.TopLeft:
				case ContentAlignment.TopCenter:
				case ContentAlignment.TopRight:
					fCardTop = (float)columnRect.Top + fVSpace * 0.5f;
					break;
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomRight:
					float fCards = (float)columnRect.Height - empty;
					fCardTop = (float)(columnRect.Bottom - (fCards + spacing)) - fVSpace * 0.5f;
					break;
			}
			return fCardTop;
		}
	}
	public class LayoutViewMultiRowMode : LayoutViewRowMode {
		public LayoutViewMultiRowMode(ILayoutViewInfo ownerView) : base(ownerView) { }
		public override LayoutViewMode ViewMode { get { return LayoutViewMode.MultiRow; } }
		public override bool NavigationHScrollNeed {
			get {
				ScrollBarOrientation scrollOrientation = View.OptionsMultiRecordMode.MultiRowScrollBarOrientation;
				if(scrollOrientation == ScrollBarOrientation.Default) return base.NavigationHScrollNeed;
				else return scrollOrientation == ScrollBarOrientation.Horizontal;
			}
		}
		public override bool NavigationVScrollNeed {
			get {
				ScrollBarOrientation scrollOrientation = View.OptionsMultiRecordMode.MultiRowScrollBarOrientation;
				if(scrollOrientation == ScrollBarOrientation.Default) return base.NavigationVScrollNeed;
				else return scrollOrientation == ScrollBarOrientation.Vertical;
			}
		}
		public override int CalcTouchScrollChange(int touchOffset) {
			if(NavigationHScrollNeed) 
				return ArrangeCache.Rows.Count;
			return ArrangeCache.Rows[touchOffset < 0 ? 0 : ArrangeCache.Rows.Count - 1].Cards.Count;
		}
		protected override int GetTouchScrollSpacing(bool forward) {
			if(NavigationHScrollNeed) 
				return Round(ArrangeCache.fCardHSpace);
			else 
				return Round(ArrangeCache.Rows[forward ? 0 : ArrangeCache.Rows.Count - 1].fVSpace);
		}
		protected override void CheckTouchScrollRanges(ref RangeInfo start, ref RangeInfo end) {
			if(NavigationHScrollNeed) {
				foreach(RowInfo rowInfo in ArrangeCache.Rows)
					CheckRow(rowInfo.Cards, ref start, ref end);
			}
			else {
				var columns = ArrangeCache.GetColumns();
				foreach(ColumnInfo colInfo in columns)
					CheckColumn(colInfo.Cards, ref start, ref end);
			}
		}
		protected override bool IsFirstCardInView() {
			if(ArrangeCache.Rows.Count == 0) return false;
			RowInfo lastRow = ArrangeCache.Rows[0];
			return IsFirstCard(lastRow.Cards[0]);
		}
		protected override bool IsLastCardInView() {
			if(ArrangeCache.Rows.Count == 0) return false;
			RowInfo lastRow = ArrangeCache.Rows[ArrangeCache.Rows.Count - 1];
			return IsLastCard(lastRow.Cards[lastRow.Cards.Count - 1]);
		}
		protected override bool IsHorizontal(bool partialWidth, bool partialHeight) {
			if(NavigationVScrollNeed)
				return partialWidth && !partialHeight;
			return base.IsHorizontal(partialWidth, partialHeight);
		}
		protected internal override LayoutViewCard[] GetTouchScrollCards(bool forward) {
			if(NavigationHScrollNeed) {
				LayoutViewCard[] lastCards = new LayoutViewCard[ArrangeCache.Rows.Count];
				for(int i = 0; i < lastCards.Length; i++) {
					lastCards[i] = GetTouchScrollCardsCore(forward, ArrangeCache.Rows[i].Cards)[0];
				}
				return lastCards;
			}
			return ArrangeCache.Rows[forward ? 0 : ArrangeCache.Rows.Count - 1].Cards.ToArray();
		}
		protected override LayoutViewCard[] GetLastCards(LayoutViewCard lastCard) {
			if(ArrangeCache.Rows.Count == 1)
				return new LayoutViewCard[] { lastCard };
			LayoutViewCard[] lastCards = new LayoutViewCard[ArrangeCache.Rows.Count];
			for(int i = 0; i < lastCards.Length; i++) {
				List<LayoutViewCard> row = ArrangeCache.Rows[i].Cards;
				lastCards[i] = row[row.Count - 1];
			}
			return lastCards;
		}
		protected override bool CalcCardIsPartiallyVisible(Rectangle rect, LayoutViewCard card) {
			if(ArrangeCache.Rows.Count == 1)
				return base.CalcCardIsPartiallyVisible(rect, card);
			return !rect.Contains(new Rectangle(card.Location, card.Size));
		}
		public override int GetScrollableCardsCount() {
			if(ArrangeCache.Rows.Count == 1)
				return base.GetScrollableCardsCount();
			if(ArrangeCache.Rows.Count == VisibleCards.Count)
				return VisibleCards.Count;
			return CalcScrollableCardsCount(VisibleCards, ViewInfo.ViewRects.CardsRect);
		}
		public override int GetHSmallChange(bool forward) {
			if(!NavigationHScrollNeed) return 1;
			return GetRowSize(forward);
		}
		public override int GetVSmallChange(bool forward) {
			if(!NavigationVScrollNeed) return 1;
			return GetRowSize(forward);
		}
		int GetRowSize(bool last) {
			int rowCount = ArrangeCache.Rows.Count;
			if(rowCount == 1) return 1;
			int row = last && rowCount > 1 ? rowCount - 1 : 0;
			return rowCount > 0 ? CalcScrollableCardsCount(ArrangeCache.Rows[row].Cards, ViewInfo.ViewRects.CardsRect) : 1;
		}
		protected override bool CanStretchCards() {
			return base.CanStretchCards() && !(View.OptionsMultiRecordMode.MaxCardRows > 0);
		}
		protected override Size GetEmptySpace() {
			ArrangeCache.Reset();
			TryArrangeList(VisibleCards);
			foreach(RowInfo info in ArrangeCache.Rows) {
				ArrangeCache.CardsArea.Width = Math.Max(ArrangeCache.CardsArea.Width, info.RowSize.Width);
				ArrangeCache.CardsArea.Height += info.RowSize.Height;
			}
			return ViewInfo.ViewRects.CardsRect.Size - ArrangeCache.CardsArea;
		}
		protected override int GetRealHeight() {
			int h = 0;
			int vSpace = (ArrangeCache.Rows.Count > 1) ? MinArrangeVSpacing + SeparatorWidth : 0;
			foreach(RowInfo info in ArrangeCache.Rows) 
				h += info.RowSize.Height;
			return h > 0 ? h + vSpace * 2 : 0;
		}
		protected override LayoutViewCard TryInsertCardInEmptySpaceAndCache(int recordIndex) {
			LayoutViewCard card = ViewInfo.InitializeCard(recordIndex);
			if(card != null) {
				List<LayoutViewCard> cards = MakeListToArrange(recordIndex, card);
				bool fCanInsertCard = (cards.Count == TryArrangeList(cards)) && CheckMaxCardRows();
				if(fCanInsertCard) {
					ArrangeCache.Cache(card, recordIndex);
					return card;
				}
				else AddCardToCache(card);
			}
			return null;
		}
		protected virtual bool CheckMaxCardCountInRow(int currentRow) {
			bool result = true;
			if(MaxCardColumns > 0) {
				if(ArrangeCache.Rows.Count > currentRow) {
					result &= MaxCardColumns > ArrangeCache.Rows[currentRow].Cards.Count;
				}
			}
			return result;
		}
		protected virtual bool CheckMaxCardRows() {
			bool result = true;
			if(MaxCardRows > 0) result &= (MaxCardRows >= ArrangeCache.Rows.Count);
			return result;
		}
		protected virtual int TryArrangeList(List<LayoutViewCard> list) {
			ArrangeCache.PreArrangeReset();
			if(list.Count == 0) return 0;
			int iArrangedCount = 0;
			int iRowCount = 0;
			Size viewSize = ViewInfo.ViewRects.CardsRect.Size;
			Size currentRowSize = Size.Empty;
			if(list.Count == 1) {
				list[0].IsPartiallyVisible = false;
				DoPreArrangeRowCard(list[0], 0, 0, list[0].Size);
				return 1;
			}
			bool allCardsInView = AllCardsInView(list);
			foreach(LayoutViewCard card in list) {
				bool isBoundCard = IsBoundCard(card, allCardsInView);
				card.IsPartiallyVisible = false;
				if(!card.Expanded) card.Width = Math.Max(card.MinSize.Width, card.Width);
				Size cardCellSize = card.Size + new Size(MinArrangeHSpacing + SeparatorWidth, MinArrangeVSpacing + SeparatorWidth);
				if(!card.Expanded) {
					Size defSize = GetCollapsedCardSizeDefault();
					cardCellSize = new Size(card.Size.Width, defSize.Height) + new Size(MinArrangeHSpacing, MinArrangeVSpacing);
				}
				Size availSize = new Size(viewSize.Width - currentRowSize.Width, viewSize.Height);
				bool fCanInsertInCurrentRow = CanInsertCardInRow(isBoundCard, iRowCount, cardCellSize, availSize);
				if(fCanInsertInCurrentRow && CheckMaxCardCountInRow(iRowCount)) {
					currentRowSize.Width += cardCellSize.Width;
					currentRowSize.Height = Math.Max(currentRowSize.Height, cardCellSize.Height);
					card.IsPartiallyVisible = viewSize.Width < currentRowSize.Width;
					DoPreArrangeRowCard(card, iRowCount, iArrangedCount++, currentRowSize);
				}
				else {
					Size nextRowSize = new Size(viewSize.Width, viewSize.Height - currentRowSize.Height);
					bool fCanInsertInNextRow = CanInsertCardInRow(isBoundCard, iRowCount + 1, cardCellSize, nextRowSize);
					if(fCanInsertInNextRow) {
						iRowCount++;
						currentRowSize = cardCellSize;
						card.IsPartiallyVisible = false;
						DoPreArrangeRowCard(card, iRowCount, iArrangedCount++, currentRowSize);
						viewSize.Height = nextRowSize.Height;
					}
				}
			}
			return iArrangedCount;
		}
		void DoPreArrangeRowCard(LayoutViewCard card, int iRowCount, int index, Size curRowSize) {
			if(iRowCount == ArrangeCache.Rows.Count) ArrangeCache.Rows.Add(new RowInfo(curRowSize));
			ArrangeCache.Rows[iRowCount].Cards.Add(card);
			ArrangeCache.Rows[iRowCount].RowSize = curRowSize;
			ArrangeCache.Rows[iRowCount].iMaxCardHeight = Math.Max(ArrangeCache.Rows[iRowCount].iMaxCardHeight, card.Size.Height);
		}
		bool CanInsertCardInRow(bool isBoundCard, int iRow, Size cardCellSize, Size availRowSize) {
			bool canInsert = false;
			int spacing = MinArrangeHSpacing + SeparatorWidth;
			if(UseWholeCards) {
				canInsert = CheckDim(cardCellSize.Width - spacing, availRowSize.Width, spacing);
				if(iRow != 0) canInsert &= (availRowSize.Height - (cardCellSize.Height + spacing) > 0);
			}
			else {
				canInsert = availRowSize.Width - MinVisibleCardThickness > 0;
				if(iRow != 0) canInsert &= (availRowSize.Height - MinVisibleCardThickness > 0);
			}
			return canInsert;
		}
		protected override bool CheckCanCollectAnyCardsYet() {
			int rowCount = ArrangeCache.Rows.Count;
			return (rowCount > 0) ? ArrangeCache.Rows[rowCount - 1].RowSize.Width < ViewInfo.ViewRects.CardsRect.Width : false;
		}
		protected override void ArrangeVisibleCards() {
			Rectangle viewRect = ViewInfo.ViewRects.CardsRect;
			int numRows = ArrangeCache.Rows.Count;
			int space = GetEmptySpaceHeight(ArrangeCache.Rows, viewRect.Height);
			float fVSpace = GetVSpace(numRows, space);
			float rowTop = GetCardsTop(numRows, viewRect.Top, space, fVSpace);
			float lineOffset = fVSpace * 0.5f;
			int firstCard = 0;
			for(int i = 0; i < numRows; i++) {
				RowInfo info = ArrangeCache.Rows[i];
				info.RowRect = new Rectangle(viewRect.Left, Round(rowTop), viewRect.Width, info.iMaxCardHeight);
				ArrangeRow(info.Cards, info.RowRect, false, i, firstCard);
				info.fHSpace = ArrangeCache.fCardHSpace;
				info.fVSpace = fVSpace;
				rowTop += (info.iMaxCardHeight + fVSpace);
				firstCard += info.Cards.Count;
				if(numRows > 1 && (i + 1 < numRows)) {
					Point pt1 = new Point(viewRect.Left, Round(rowTop - lineOffset));
					Point pt2 = new Point(viewRect.Right + viewRect.Width, Round(rowTop - lineOffset));
					Lines.Add(new Line(pt1, pt2, false));
				}
			}
		}
		protected override void ArrangePostProcess() {
			float maxHSpace = 0;
			int numRows = ArrangeCache.Rows.Count;
			for(int i = 0; i < numRows; i++) {
				RowInfo info = ArrangeCache.Rows[i];
				SizeF lineOffset = new SizeF(info.fHSpace * 0.5f, info.fVSpace * 0.5f);
				PostProcessRow(info.Cards, info.RowRect, info.iMaxCardHeight, lineOffset);
				if(i + 1 < numRows) maxHSpace = Math.Max(maxHSpace, info.fHSpace);
			}
			if(numRows > 1) {
				RowInfo info = ArrangeCache.Rows[numRows - 1];
				if(maxHSpace >= info.fHSpace) return;
				float cardLeft = ViewInfo.ViewRects.CardsRect.Left + maxHSpace;
				foreach(LayoutViewCard card in info.Cards) {
					card.Position = new Point(Round(cardLeft), card.Position.Y);
					cardLeft += (card.Width + maxHSpace);
					int pos = card.VisibleIndex - (numRows - 1);
					if(pos >= 0 && pos < CardLines.Count) {
						CardLines[pos].BeginPoint = new Point(Round(cardLeft - maxHSpace * 0.5f), CardLines[pos].BeginPoint.Y);
						CardLines[pos].EndPoint = new Point(Round(cardLeft - maxHSpace * 0.5f), CardLines[pos].EndPoint.Y);
					}
				}
			}
		}
		int GetEmptySpaceHeight(IList<RowInfo> rows, int viewHeight) {
			for(int i = 0; i < rows.Count; i++) {
				viewHeight -= rows[i].iMaxCardHeight;
			}
			return viewHeight;
		}
	}
	public class LayoutViewMultiColumnMode : LayoutViewColumnMode {
		public LayoutViewMultiColumnMode(ILayoutViewInfo ownerView) : base(ownerView) { }
		public override LayoutViewMode ViewMode { get { return LayoutViewMode.MultiColumn; } }
		public override bool NavigationHScrollNeed {
			get {
				ScrollBarOrientation scrollOrientation = View.OptionsMultiRecordMode.MultiColumnScrollBarOrientation;
				if(scrollOrientation == ScrollBarOrientation.Default) return base.NavigationHScrollNeed;
				else return scrollOrientation == ScrollBarOrientation.Horizontal;
			}
		}
		public override bool NavigationVScrollNeed {
			get {
				ScrollBarOrientation scrollOrientation = View.OptionsMultiRecordMode.MultiColumnScrollBarOrientation;
				if(scrollOrientation == ScrollBarOrientation.Default) return base.NavigationVScrollNeed;
				else return scrollOrientation == ScrollBarOrientation.Vertical;
			}
		}
		public override int CalcTouchScrollChange(int touchOffset) {
			if(NavigationVScrollNeed)
				return ArrangeCache.Columns.Count;
			return ArrangeCache.Columns[(touchOffset < 0) ? 0 : ArrangeCache.Columns.Count - 1].Cards.Count;
		}
		protected override int GetTouchScrollSpacing(bool forward) {
			if(NavigationVScrollNeed)
				return Round(ArrangeCache.fCardVSpace);
			else
				return Round(ArrangeCache.Columns[forward ? 0 : ArrangeCache.Columns.Count - 1].fHSpace);
		}
		protected override void CheckTouchScrollRanges(ref RangeInfo start, ref RangeInfo end) {
			if(NavigationVScrollNeed) {
				foreach(ColumnInfo colInfo in ArrangeCache.Columns)
					CheckColumn(colInfo.Cards, ref start, ref end);
			}
			else {
				var rows = ArrangeCache.GetRows();
				foreach(RowInfo rowInfo in rows)
					CheckRow(rowInfo.Cards, ref start, ref end);
			}
		}
		protected override bool IsFirstCardInView() {
			if(ArrangeCache.Columns.Count == 0) return false;
			ColumnInfo firstColumn = ArrangeCache.Columns[0];
			return IsFirstCard(firstColumn.Cards[0]);
		}
		protected override bool IsLastCardInView() {
			if(ArrangeCache.Columns.Count == 0) return false;
			ColumnInfo lastColumn = ArrangeCache.Columns[ArrangeCache.Columns.Count - 1];
			return IsLastCard(lastColumn.Cards[lastColumn.Cards.Count - 1]);
		}
		protected override bool IsHorizontal(bool partialWidth, bool partialHeight) {
			if(NavigationHScrollNeed)
				return partialHeight && !partialWidth;
			return base.IsHorizontal(partialWidth, partialHeight);
		}
		protected internal override LayoutViewCard[] GetTouchScrollCards(bool forward) {
			if(NavigationVScrollNeed) {
				LayoutViewCard[] lastCards = new LayoutViewCard[ArrangeCache.Columns.Count];
				for(int i = 0; i < lastCards.Length; i++) {
					lastCards[i] = GetTouchScrollCardsCore(forward, ArrangeCache.Columns[i].Cards)[0];
				}
				return lastCards;
			}
			return ArrangeCache.Columns[forward ? 0 : ArrangeCache.Columns.Count - 1].Cards.ToArray();
		}
		protected override LayoutViewCard[] GetLastCards(LayoutViewCard lastCard) {
			if(ArrangeCache.Rows.Count == 1)
				return new LayoutViewCard[] { lastCard };
			LayoutViewCard[] lastCards = new LayoutViewCard[ArrangeCache.Columns.Count];
			for(int i = 0; i < lastCards.Length; i++) {
				List<LayoutViewCard> column = ArrangeCache.Columns[i].Cards;
				lastCards[i] = column[column.Count - 1];
			}
			return lastCards;
		}
		protected override bool CalcCardIsPartiallyVisible(Rectangle rect, LayoutViewCard card) {
			if(ArrangeCache.Columns.Count == 1) 
				return base.CalcCardIsPartiallyVisible(rect, card);
			return !rect.Contains(new Rectangle(card.Location, card.Size));
		}
		public override int GetScrollableCardsCount() {
			if(ArrangeCache.Columns.Count == 1)
				return base.GetScrollableCardsCount();
			if(ArrangeCache.Columns.Count == VisibleCards.Count)
				return VisibleCards.Count;
			return CalcScrollableCardsCount(VisibleCards, ViewInfo.ViewRects.CardsRect);
		}
		public override int GetHSmallChange(bool forward) {
			if(!NavigationHScrollNeed) return 1;
			return GetColumnSize(forward);
		}
		public override int GetVSmallChange(bool forward) {
			if(!NavigationVScrollNeed) return 1;
			return GetColumnSize(forward);
		}
		int GetColumnSize(bool last) {
			int colCount = ArrangeCache.Columns.Count;
			if(colCount == 1) return 1;
			int col = last && colCount > 1 ? colCount - 1 : 0;
			return colCount > 0 ? CalcScrollableCardsCount(ArrangeCache.Columns[col].Cards, ViewInfo.ViewRects.CardsRect) : 1;
		}
		protected override bool CanStretchCards() {
			return base.CanStretchCards() && !(View.OptionsMultiRecordMode.MaxCardColumns > 0);
		}
		protected override Size GetEmptySpace() {
			ArrangeCache.Reset();
			TryArrangeList(VisibleCards);
			foreach(ColumnInfo info in ArrangeCache.Columns) {
				ArrangeCache.CardsArea.Height = Math.Max(ArrangeCache.CardsArea.Height, info.ColumnSize.Height);
				ArrangeCache.CardsArea.Width += info.ColumnSize.Width;
			}
			return ViewInfo.ViewRects.CardsRect.Size - ArrangeCache.CardsArea;
		}
		protected override int GetRealHeight() {
			int h = 0;
			int vSpace = MinArrangeVSpacing + SeparatorWidth;
			foreach(ColumnInfo info in ArrangeCache.Columns)
				h = Math.Max(h, info.ColumnSize.Height);
			return h + ((h > 0) ? vSpace : 0);
		}
		protected override bool CheckCanCollectAnyCardsYet() {
			int colCount = ArrangeCache.Columns.Count;
			return (colCount > 0) ? ArrangeCache.Columns[colCount - 1].ColumnSize.Height < ViewInfo.ViewRects.CardsRect.Height : false;
		}
		protected virtual bool CheckMaxCardCountInColumn(int currentColumn) {
			bool result = true;
			if(MaxCardRows > 0) {
				if(ArrangeCache.Columns.Count > currentColumn) {
					result &= MaxCardRows > ArrangeCache.Columns[currentColumn].Cards.Count;
				}
			}
			return result;
		}
		protected override LayoutViewCard TryInsertCardInEmptySpaceAndCache(int recordIndex) {
			LayoutViewCard card = ViewInfo.InitializeCard(recordIndex);
			if(card != null) {
				List<LayoutViewCard> cards = MakeListToArrange(recordIndex, card);
				bool fCanInsertCard = (cards.Count == TryArrangeList(cards));
				if(fCanInsertCard) {
					ArrangeCache.Cache(card, recordIndex);
					return card;
				}
				else AddCardToCache(card);
			}
			return null;
		}
		protected virtual int TryArrangeList(List<LayoutViewCard> list) {
			ArrangeCache.PreArrangeReset();
			if(list.Count == 0) return 0;
			int iArrangedCount = 0;
			int iColumnCount = 0;
			Size viewSize = ViewInfo.ViewRects.CardsRect.Size;
			Size currentColumnSize = Size.Empty;
			if(list.Count == 1) {
				list[0].IsPartiallyVisible = false;
				DoPreArrangeColumnCard(list[0], 0, 0, list[0].Size);
				return 1;
			}
			int cardCellWidth = GetCardCellSize(list[0]).Width + SeparatorWidth;
			int columnsCount = GetMaxColumnsCount(viewSize.Width / cardCellWidth);
			if(list.Count <= columnsCount) {
				foreach(LayoutViewCard card in list) {
					currentColumnSize = GetCardCellSize(card);
					card.IsPartiallyVisible = false;
					DoPreArrangeColumnCard(card, iColumnCount++, iArrangedCount++, currentColumnSize);
				}
				return iArrangedCount;
			}
			bool allCardsInView = AllCardsInView(list);
			foreach(LayoutViewCard card in list) {
				Size cardCellSize = GetCardCellSize(card);
				Size availSize = new Size(viewSize.Width, viewSize.Height - currentColumnSize.Height);
				bool fCanInsertInCurrentColumn = CanInsertCardInColumn(iColumnCount, cardCellSize, availSize);
				if(fCanInsertInCurrentColumn && CheckMaxCardCountInColumn(iColumnCount)) {
					currentColumnSize.Height += cardCellSize.Height;
					currentColumnSize.Width = Math.Max(currentColumnSize.Width, cardCellSize.Width);
					card.IsPartiallyVisible = viewSize.Height < currentColumnSize.Height;
					DoPreArrangeColumnCard(card, iColumnCount, iArrangedCount++, currentColumnSize);
				}
				else {
					Size nextColumnSize = new Size(viewSize.Width - currentColumnSize.Width, viewSize.Height);
					bool fCanInsertInNextColumn = CanInsertCardInColumn(iColumnCount + 1, cardCellSize, nextColumnSize);
					if(fCanInsertInNextColumn) {
						iColumnCount++;
						currentColumnSize = cardCellSize;
						card.IsPartiallyVisible = false;
						DoPreArrangeColumnCard(card, iColumnCount, iArrangedCount++, currentColumnSize);
						viewSize.Width = nextColumnSize.Width;
					}
				}
			}
			return iArrangedCount;
		}
		Size GetCardCellSize(LayoutViewCard card) {
			if(!card.Expanded) card.Height = card.MinSize.Height;
			Size cardCellSize = card.Size + new Size(MinArrangeHSpacing + SeparatorWidth, MinArrangeVSpacing + SeparatorWidth);
			if(!card.Expanded) {
				Size defSize = GetCollapsedCardSizeDefault();
				cardCellSize = new Size(defSize.Width, card.Size.Height) + new Size(MinArrangeHSpacing, MinArrangeVSpacing);
			}
			return cardCellSize;
		}
		void DoPreArrangeColumnCard(LayoutViewCard card, int iColumnCount, int index, Size curColumnSize) {
			if(iColumnCount == ArrangeCache.Columns.Count) ArrangeCache.Columns.Add(new ColumnInfo(curColumnSize));
			ArrangeCache.Columns[iColumnCount].Cards.Add(card);
			ArrangeCache.Columns[iColumnCount].ColumnSize = curColumnSize;
			ArrangeCache.Columns[iColumnCount].iMaxCardWidth = Math.Max(ArrangeCache.Columns[iColumnCount].iMaxCardWidth, card.Size.Width);
		}
		int GetMaxColumnsCount(int columnsCount) {
			return (MaxCardColumns > 0) ? Math.Min(MaxCardColumns, columnsCount) : columnsCount;
		}
		bool CanInsertCardInColumn(int iColumn, Size cardCellSize, Size availColumnSize) {
			if((MaxCardColumns > 0) && (iColumn >= MaxCardColumns)) return false;
			bool canInsert = false;
			int spacing = MinArrangeVSpacing + SeparatorWidth;
			if(UseWholeCards) {
				canInsert = CheckDim(cardCellSize.Height - spacing, availColumnSize.Height, spacing);
				if(iColumn != 0) canInsert &= (availColumnSize.Width - (cardCellSize.Width + spacing) > 0);
			}
			else {
				canInsert = availColumnSize.Height - MinVisibleCardThickness > 0;
				if(iColumn != 0) canInsert &= (availColumnSize.Width - MinVisibleCardThickness > 0);
			}
			return canInsert;
		}
		protected override void ArrangeVisibleCards() {
			Rectangle viewRect = ViewInfo.ViewRects.CardsRect;
			int numColumns = ArrangeCache.Columns.Count;
			int space = GetEmptySpaceWidth(ArrangeCache.Columns, viewRect.Width);
			float fHSpace = GetHSpace(numColumns, space);
			float columnLeft = GetCardsLeft(numColumns, viewRect.Left, space, fHSpace);
			float lineOffset = fHSpace * 0.5f;
			int firstCard = 0;
			for(int i = 0; i < numColumns; i++) {
				ColumnInfo info = ArrangeCache.Columns[i];
				info.ColumnRect = new Rectangle(Round(columnLeft), viewRect.Top, info.iMaxCardWidth, viewRect.Height);
				ArrangeColumn(info.Cards, info.ColumnRect, false, i, firstCard);
				info.fVSpace = ArrangeCache.fCardVSpace;
				info.fHSpace = fHSpace;
				columnLeft += (info.iMaxCardWidth + fHSpace);
				firstCard += info.Cards.Count;
				if(numColumns > 1 && (i + 1 < numColumns)) {
					Point pt1 = new Point(Round(columnLeft - lineOffset), viewRect.Top);
					Point pt2 = new Point(Round(columnLeft - lineOffset), viewRect.Bottom);
					Lines.Add(new Line(pt1, pt2, true));
				}
			}
		}
		protected override void ArrangePostProcess() {
			float maxVSpace = 0;
			int numColumns = ArrangeCache.Columns.Count;
			for(int i = 0; i < numColumns; i++) {
				ColumnInfo info = ArrangeCache.Columns[i];
				SizeF lineOffset = new SizeF(info.fHSpace * 0.5f, info.fVSpace * 0.5f);
				PostProcessColumn(info.Cards, info.ColumnRect, info.iMaxCardWidth, lineOffset);
				if(i + 1 < numColumns) {
					maxVSpace = Math.Max(maxVSpace, info.fVSpace);
				}
			}
			if(numColumns > 1) {
				ColumnInfo info = ArrangeCache.Columns[numColumns - 1];
				if(maxVSpace >= info.fVSpace) return;
				float cardTop = ViewInfo.ViewRects.CardsRect.Top + maxVSpace;
				foreach(LayoutViewCard card in info.Cards) {
					card.Position = new Point(card.Position.X, Round(cardTop));
					cardTop += (card.Height + maxVSpace);
					int pos = card.VisibleIndex - (numColumns - 1);
					if(pos >= 0 && pos < CardLines.Count) {
						CardLines[pos].BeginPoint = new Point(CardLines[pos].BeginPoint.X, Round(cardTop - maxVSpace * 0.5f));
						CardLines[pos].EndPoint = new Point(CardLines[pos].EndPoint.X, Round(cardTop - maxVSpace * 0.5f));
					}
				}
			}
		}
		int GetEmptySpaceWidth(IList<ColumnInfo> columns, int viewWidth) {
			for(int i = 0; i < columns.Count; i++) {
				viewWidth -= columns[i].iMaxCardWidth;
			}
			return viewWidth;
		}
	}
	public class CardImageInfo {
		AnimatedLayoutViewMode viewMode;
		List<Image> images;
		float startAngle;
		public CardImageInfo(AnimatedLayoutViewMode viewMode) {
			this.viewMode = viewMode;
		}
		public List<Image> Images {
			get {
				if(images == null) images = new List<Image>();
				return images;
			}
		}
		public Image GetImage(Rectangle bounds) {
			if(Images.Count == 0 || Images[0] == null) return null;
			if(bounds.Width > Images[0].Width || bounds.Width == 0) return Images[0];
			int index = Images[0].Width / bounds.Width - 1;
			return Images[Math.Min(Math.Max(index, 0), Images.Count - 1)];
		}
		public void Destroy() {
			foreach(Image img in Images) {
				if(img != null) img.Dispose();
			}
			Images.Clear();
		}
		public void GenerateMipMap(int minSize) {
			if(Images.Count == 0 || Images[0] == null) return;
			int currWidth = Images[0].Width / 2, currHeight = Images[0].Height / 2;
			while(currWidth >= minSize && currHeight >= minSize) {
				Image img = new Bitmap(currWidth, currHeight);
				using(Graphics g = Graphics.FromImage(img)) {
					g.InterpolationMode = viewMode.View.OptionsCarouselMode.InterpolationMode;
					g.DrawImage(Images[0], new Rectangle(0, 0, img.Width, img.Height), 0, 0, Images[0].Width, Images[0].Height, GraphicsUnit.Pixel);
				}
				Images.Add(img);
				currWidth /= 2;
				currHeight /= 2;
			}
		}
		public Rectangle Bounds {
			get { return ViewMode.GetBounds(this); }
		}
		public int RowHandle;
		public float Alpha {
			get { return ViewMode.GetAlpha(this); }
		}
		public float ColorFade {
			get { return ViewMode.GetColorFade(this); }
		}
		public float SizeScale {
			get { return ViewMode.GetScale(this); }
		}
		public float StartAngle {
			get { return startAngle; }
			set { startAngle = value; }
		}
		public float Angle {
			get { return StartAngle + ViewMode.Offset; }
		}
		public AnimatedLayoutViewMode ViewMode {
			get { return viewMode; }
		}
		public override string ToString() {
			return string.Format("Card {0}, Angle:{1}", RowHandle, Angle);
		}
	}
	public class CardImageInfoCollection : CollectionBase {
		public virtual int Add(CardImageInfo img) { return List.Add(img); }
		public virtual void Insert(int index, CardImageInfo img) { List.Insert(index, img); }
		public virtual void RemoveFirst() {
			if(List.Count > 0) {
				CardImageInfo info = (CardImageInfo)List[0];
				List.RemoveAt(0);
				info.Destroy();
			}
		}
		public virtual void RemoveLast() {
			if(List.Count > 0) {
				CardImageInfo info = (CardImageInfo)List[Count - 1];
				List.RemoveAt(Count - 1);
				info.Destroy();
			}
		}
		public virtual CardImageInfo this[int index] {
			get { return List[index] as CardImageInfo; }
			set { List[index] = value; }
		}
	}
	public abstract class AnimatedLayoutViewMode : LayoutViewSingleRecordMode, ISupportXtraAnimation {
		CardImageInfoCollection cardImageInfoCollectionCore;
		int centerCardIndexCore;
		int prevRecordIndexCore;
		float offsetCore;
		bool fNeedDropCache = false;
		public AnimatedLayoutViewMode(ILayoutViewInfo ownerView)
			: base(ownerView) {
			this.cardImageInfoCollectionCore = new CardImageInfoCollection();
			this.prevRecordIndexCore = -1;
			this.centerCardIndexCore = 0;
		}
		protected override void OnDispose() {
			ForceStopAimation();
			if(cardImageInfoCollectionCore != null) {
				cardImageInfoCollectionCore.Clear();
				cardImageInfoCollectionCore = null;
			}
			base.OnDispose();
		}
		public abstract Rectangle GetBounds(CardImageInfo info);
		public abstract float GetScale(CardImageInfo info);
		public abstract float GetColorFade(CardImageInfo info);
		public abstract float GetAlpha(CardImageInfo info);
		protected int PrevRecordIndex {
			get { return prevRecordIndexCore; }
		}
		protected override void ArrangeVisibleCards() {
			scrollPlacementUsed = false;
			if(CardImageInfos == null || CardImageInfos.Count == 0) return;
			base.ArrangeVisibleCards();
		}
		protected override bool GetHStretch() {
			return View.OptionsCarouselMode.StretchCardToViewWidth;
		}
		protected override bool GetVStretch() {
			return View.OptionsCarouselMode.StretchCardToViewHeight;
		}
		public override void Arrange(int currentRecordIndex) {
			if(!IsAnimated) TrimAnimationCache();
			base.Arrange(currentRecordIndex);
			UpdateCardImages(fNeedDropCache);
			if(!IsAnimated) TrimAnimationCache();
			this.fNeedDropCache = false;
		}
		public override void ResetCache() {
			base.ResetCache();
			this.fNeedDropCache = true;
		}
		protected virtual void Shift(int delta) {
			if(delta < 0) ShiftLeft(-delta);
			else ShiftRight(delta);
		}
		protected int RealCardsCount {
			get { return Math.Min(View.OptionsCarouselMode.CardCount, View.RecordCount); }
		}
		public override void OnGridLoadComplete() {
			base.OnGridLoadComplete();
			EndAnimation();
		}
		protected virtual void ForceStopAimation() {
			XtraAnimator.RemoveObject(this);
			deltaCountCore = 0;
		}
		protected virtual bool ShouldAnimateCards(int delta) {
			if(PrevRecordIndex == -1) return false;
			if(DeltaCount * delta < 0) return false;
			if(Math.Abs(DeltaCount) >= RealCardsCount) return false;
			return true;
		}
		protected void UpdateCardImages(bool forceUpdate) {
			try {
				int delta = View.VisibleRecordIndex - PrevRecordIndex;
				if(CardImageInfos.Count == 0) InitializeCardImages();
				if(delta == 0 && !forceUpdate) return;
				if(ShouldAnimateCards(delta) && !forceUpdate) {
					Shift(delta);
					this.prevRecordIndexCore = View.VisibleRecordIndex;
					return;
				}
				ForceStopAimation();
				this.prevRecordIndexCore = View.VisibleRecordIndex;
				InitializeCardImages();
				if(CardImageInfos.Count == 0) return;
			}
			finally {
				UpdateVisibleCardPosition();
			}
		}
		protected void UpdateVisibleCardPosition() {
			if(VisibleCards.Count == 0 || CardImageInfos.Count == 0 || IsAnimated) return;
			LayoutViewCard card = VisibleCards[0];
			Rectangle cardBounds = CardImageInfos[CenterCardIndex].Bounds;
			Rectangle cardsRect = ViewInfo.ViewRects.CardsRect;
			Point proposedCardPos = cardBounds.Location;
			proposedCardPos.Y = Math.Max(cardsRect.Location.Y, proposedCardPos.Y);
			proposedCardPos.X = Math.Max(cardsRect.Location.X, proposedCardPos.X);
			Size offset = cardBounds.Size - card.Size;
			var finalCardPos = proposedCardPos + new Size(offset.Width / 2, offset.Height / 2);
			finalCardPos = CheckFitsVisibleArea(card, cardsRect, finalCardPos);
			card.Position = finalCardPos;
			UpdateCardPosition(card);
		}
		protected virtual Point CheckFitsVisibleArea(LayoutViewCard card, Rectangle cardsRect, Point finalCardPos) {
			return finalCardPos;
		}
		public int GetHotCardIndex(Point p) {
			if(IsAnimated) return -1;
			if(CardImageInfos.Count == 0) return -1;
			for(int i = CenterCardIndex; i >= 0; i--) {
				if(CardImageInfos[i].Bounds.Contains(p)) return View.VisibleRecordIndex - CenterCardIndex + i;
			}
			for(int i = CenterCardIndex + 1; i < CardImageInfos.Count; i++) {
				if(CardImageInfos[i].Bounds.Contains(p)) return View.VisibleRecordIndex - CenterCardIndex + i;
			}
			return -1;
		}
		protected Bitmap GetCardImageByIndex(int index, out int rowHandle) {
			LayoutViewCard card = ViewInfo.InitializeCard(index);
			try {
				rowHandle = card.RowHandle;
				return GetCardImage(card);
			}
			finally { AddCardToCache(card); }
		}
		int deltaCountCore = 0;
		public bool IsAnimated {
			get { return deltaCountCore != 0; }
		}
		protected internal int DeltaCount {
			get { return deltaCountCore; }
		}
		protected abstract void InitializeBeginValue(int cardIndex, int prevCardIndex);
		int GetRealLeftCardCount(int dcount) {
			return Math.Min(PrevRecordIndex, dcount);
		}
		int GetRealRightCardCount(int dcount) {
			return Math.Min(View.RecordCount - PrevRecordIndex - 1, dcount);
		}
		protected virtual void ShiftLeft(int dcount) {
			if(dcount == 0) return;
			CustomAnimationInfo info = XtraAnimator.Current.Get(this, this) as CustomAnimationInfo;
			int realCardsCount = GetRealLeftCardCount(dcount);
			int maxBefore = GetMaxBeforeCount();
			int animationCacheFirstCardIndex = PrevRecordIndex - CenterCardIndex - 1;
			int animationSize = realCardsCount;
			if(realCardsCount > maxBefore) {
				animationCacheFirstCardIndex = PrevRecordIndex - (realCardsCount - maxBefore);
				realCardsCount = maxBefore;
				animationSize = maxBefore + CenterCardIndex;
			}
			if(IsAnimated && info != null) {
				AppendCardsLeft(animationCacheFirstCardIndex - DeltaCount, realCardsCount);
				this.deltaCountCore = -animationSize;
				UpdateAnimateParams(info, -realCardsCount);
			}
			else {
				AppendCardsLeft(animationCacheFirstCardIndex, realCardsCount);
				this.deltaCountCore = -animationSize;
				InitializeBeginValue();
				if(View.GridControl.IsLoaded)
					StartAnimation();
			}
		}
		protected virtual void ShiftRight(int dcount) {
			CustomAnimationInfo info = XtraAnimator.Current.Get(this, this) as CustomAnimationInfo;
			int realCardsCount = GetRealRightCardCount(dcount);
			int maxAfter = GetMaxAfterCount();
			int animationCacheFirstCardIndex = PrevRecordIndex + (CardImageInfos.Count - CenterCardIndex);
			int animationSize = realCardsCount;
			if(realCardsCount > maxAfter) {
				animationCacheFirstCardIndex = PrevRecordIndex + (realCardsCount - maxAfter) + 1;
				realCardsCount = maxAfter;
				animationSize = maxAfter + (CardImageInfos.Count - CenterCardIndex) - 1;
			}
			if(IsAnimated && info != null) {
				AppendCardsRight(animationCacheFirstCardIndex - DeltaCount, realCardsCount);
				this.deltaCountCore = animationSize;
				UpdateAnimateParams(info, realCardsCount);
			}
			else {
				AppendCardsRight(animationCacheFirstCardIndex, realCardsCount);
				this.deltaCountCore = animationSize;
				InitializeBeginValue();
				if(View.GridControl.IsLoaded)
					StartAnimation();
			}
		}
		protected virtual void AppendCardsLeft(int beginIndex, int count) {
			for(int i = 0; i < count; i++) {
				if(beginIndex - i < 0) break;
				CardImageInfos.Insert(0, CreateCardImageInfo(beginIndex - i));
				InitializeBeginValue(0, 1);
				this.centerCardIndexCore++;
			}
		}
		protected virtual void AppendCardsRight(int beginIndex, int count) {
			for(int i = 0; i < count; i++) {
				if(beginIndex + i >= View.RecordCount) break;
				CardImageInfos.Add(CreateCardImageInfo(beginIndex + i));
				InitializeBeginValue(CardImageInfos.Count - 1, CardImageInfos.Count - 2);
			}
		}
		protected virtual void UpdateAnimateParams(CustomAnimationInfo info, int dcount) { }
		protected virtual void OnAnimation(BaseAnimationInfo info) { }
		Control ISupportXtraAnimation.OwnerControl { get { return View.GridControl; } }
		bool ISupportXtraAnimation.CanAnimate { get { return true; } }
		protected virtual void StartAnimation() {
			if(IsDisposingInProgress) return;
			Offset = 0;
			XtraAnimator.Current.AddObject(
					this, this,
					View.OptionsCarouselMode.FrameDelay,
					View.OptionsCarouselMode.FrameCount,
					OnAnimation
				);
		}
		protected internal virtual void EndAnimation() {
			if(CardImageInfos.Count == 0) return;
			CardImageInfo imageInfo = CardImageInfos[CenterCardIndex];
			CardImageInfos[CenterCardIndex] = CreateCardImageInfo(imageInfo.RowHandle);
			imageInfo.Destroy();
			CenterCardIndex += DeltaCount;
			TrimAnimationCache();
			CheckAnimationCacheCompleteness();
			this.deltaCountCore = 0;
			Offset = 0;
			InitializeBeginValue();
			UpdateVisibleCardPosition();
		}
		protected virtual void TrimAnimationCache() {
			int realLastRecordIndex = Math.Max(0, View.RecordCount - 1);
			int realVisibleRecordIndex = Math.Max(0, View.VisibleRecordIndex);
			if(realVisibleRecordIndex > realLastRecordIndex) realVisibleRecordIndex = realLastRecordIndex;
			int afterCount = (CardImageInfos.Count - CenterCardIndex) - 1;
			int maxAfter = Math.Min(GetMaxAfterCount(), realLastRecordIndex - realVisibleRecordIndex);
			while(afterCount > maxAfter) {
				CardImageInfos.RemoveLast();
				afterCount--;
			}
			int beforeCount = CenterCardIndex;
			int maxBefore = Math.Min(GetMaxBeforeCount(), realVisibleRecordIndex);
			while(beforeCount > maxBefore) {
				CardImageInfos.RemoveFirst();
				this.centerCardIndexCore--;
				beforeCount--;
			}
		}
		protected virtual void CheckAnimationCacheCompleteness() {
			int afterCount = (CardImageInfos.Count - CenterCardIndex) - 1;
			if(afterCount > 0) {
				for(int i = 0; i <= afterCount; i++) {
					CardImageInfo info = CardImageInfos[CenterCardIndex + i];
					if(info.RowHandle != View.GetVisibleRowHandle(View.VisibleRecordIndex + i)) {
						info.Destroy();
						info.Images.Add(GetCardImageByIndex(View.VisibleRecordIndex + i, out info.RowHandle));
						info.GenerateMipMap(32);
					}
				}
			}
			int maxAfter = Math.Min(GetMaxAfterCount(), (View.RecordCount - 1) - View.VisibleRecordIndex);
			if(afterCount >= 0 && afterCount < maxAfter) {
				AppendCardsRight(View.VisibleRecordIndex + afterCount + 1, maxAfter - afterCount);
			}
			int beforeCount = CenterCardIndex;
			if(beforeCount > 0) {
				for(int i = beforeCount; i > 0; i--) {
					CardImageInfo info = CardImageInfos[CenterCardIndex - i];
					if(info.RowHandle != View.GetVisibleRowHandle(View.VisibleRecordIndex - i)) {
						info.Destroy();
						info.Images.Add(GetCardImageByIndex(View.VisibleRecordIndex - i, out info.RowHandle));
						info.GenerateMipMap(32);
					}
				}
			}
			int maxBefore = Math.Min(GetMaxBeforeCount(), View.VisibleRecordIndex);
			if(beforeCount >= 0 && beforeCount < maxBefore) {
				this.AppendCardsLeft(View.VisibleRecordIndex - 1, maxBefore - beforeCount);
			}
		}
		protected virtual bool ShouldSynhronizeCardContent {
			get { return !IsAnimated; }
		}
		protected int GetMaxBeforeCount() {
			return View.OptionsCarouselMode.CardCount / 2;
		}
		protected int GetMaxAfterCount() {
			if(View.OptionsCarouselMode.CardCount % 2 == 0) return View.OptionsCarouselMode.CardCount / 2 - 1;
			return View.OptionsCarouselMode.CardCount / 2;
		}
		protected CardImageInfo CreateCardImageInfo(int index) {
			CardImageInfo info = new CardImageInfo(this);
			info.Images.Add(GetCardImageByIndex(index, out info.RowHandle));
			info.GenerateMipMap(32);
			return info;
		}
		public void InitializeCardImages() {
			foreach(CardImageInfo info in CardImageInfos) 
				info.Destroy();
			CardImageInfos.Clear();
			if(View.RowCount == 0) return;
			int startRecordIndex = View.VisibleRecordIndex - View.OptionsCarouselMode.CardCount / 2;
			if(startRecordIndex < 0) {
				startRecordIndex = 0;
				centerCardIndexCore = View.VisibleRecordIndex;
				if(centerCardIndexCore >= View.RowCount)
					centerCardIndexCore = Math.Max(0, View.RowCount - 1);
			}
			else centerCardIndexCore = View.OptionsCarouselMode.CardCount / 2;
			int afterCount = View.RowCount - View.VisibleRecordIndex;
			if(View.VisibleRecordIndex >= View.RowCount) 
				afterCount = 1;
			int realCardsCount = afterCount + CenterCardIndex;
			realCardsCount = Math.Min(realCardsCount, View.OptionsCarouselMode.CardCount);
			for(int i = startRecordIndex; i < startRecordIndex + realCardsCount; i++) {
				CardImageInfos.Add(CreateCardImageInfo(i));
			}
			InitializeBeginValue();
		}
		protected abstract void InitializeBeginValue();
		public float Offset {
			get { return offsetCore; }
			set { offsetCore = value; }
		}
		public int CenterCardIndex {
			get { return centerCardIndexCore; }
			set {
				if(CenterCardIndex == value) return;
				centerCardIndexCore = value;
				OnPropertiesChanged();
			}
		}
		internal CardImageInfoCollection CardImageInfos {
			get { return cardImageInfoCollectionCore; }
		}
		protected virtual void OnPropertiesChanged() {
			UpdateCardImages(false);
		}
	}
	public class LayoutViewCarouselMode : AnimatedLayoutViewMode {
		public LayoutViewCarouselMode(ILayoutViewInfo ownerView)
			: base(ownerView) { }
		int radius = 0;
		protected override int GetTouchScrollSpacing(bool forward) {
			return (NavigationHScrollNeed ? ArrangeCache.iMaxCardWidth : ArrangeCache.iMaxCardHeight);
		}
		public override bool CheckTouchScroll(int touchOffset, out int maxOffset) {
			bool res = base.CheckTouchScroll(touchOffset, out maxOffset);
			maxOffset = 0;
			return res;
		}
		protected override Point CheckFitsVisibleArea(LayoutViewCard card, Rectangle cardsRect, Point finalCardPos) {
			if(finalCardPos.Y + card.Height > cardsRect.Bottom && card.Height < cardsRect.Height) { finalCardPos.Y = finalCardPos.Y - (finalCardPos.Y + card.Height - cardsRect.Bottom); }
			if(finalCardPos.X + card.Width > cardsRect.Right && card.Width < cardsRect.Width) { finalCardPos.X = finalCardPos.X - (finalCardPos.X + card.Width - cardsRect.Right); }
			return finalCardPos;
		}
		protected override Size CheckCardMinSize(Size cardMinSize, bool collapsed) {
			Size viewSize = ViewInfo.ViewRects.CardsRect.Size;
			int w = cardMinSize.Width;
			int h = collapsed ? CollapsedCardSize.Height : cardMinSize.Height;
			if(GetVStretch()) h = Math.Max(viewSize.Height, h);
			if(collapsed && CardCollapsingMode == CardCollapsingMode.Vertical) {
				int maxHeight = -1;
				foreach(LayoutViewCard card in View.ExpandedCardCache) {
					maxHeight = Math.Max(card.Height, maxHeight);
				}
				if(maxHeight != -1)
					w = maxHeight;
			}
			return new Size(w, h);
		}
		protected override int GetRealHeight() {
			return ViewInfo.ViewRects.CardsRect.Height;
		}
		protected double AutoCalcRadius() {
			Size cardSize = View.CardMinSize;
			if(VisibleCards != null && VisibleCards.Count > 0) cardSize = VisibleCards[0].Size;
			Rectangle rect = ViewInfo.ViewRects.CardsRect;
			float rollAngle = View.OptionsCarouselMode.RollAngle;
			float elipseKoeff = View.OptionsCarouselMode.PitchAngle;
			double atanAlfaW = Math.Atan((double)rect.Height / (double)rect.Width) * _ToDegree;
			while(rollAngle < 0) rollAngle += 90;
			while(rollAngle > 90) rollAngle -= 90;
			double avaliableRadiusW = 0;
			double a;
			if(atanAlfaW >= rollAngle) {
				a = Math.Tan(rollAngle * _ToRadian) * rect.Width / 2;
				avaliableRadiusW = Math.Sqrt(a * a + rect.Width / 2 * rect.Width / 2);
			}
			else {
				a = Math.Tan((90 - rollAngle) * _ToRadian) * rect.Height / 2;
				avaliableRadiusW = Math.Sqrt(a * a + rect.Height / 2 * rect.Height / 2);
			}
			double atanAlfaH = Math.Atan((double)rect.Width / (double)rect.Height) * _ToDegree;
			double avaliableRadiusH = 0;
			double b;
			if(atanAlfaH >= rollAngle) {
				b = Math.Tan(rollAngle * _ToRadian) * rect.Height / 2;
				avaliableRadiusH = Math.Sqrt(b * b + rect.Height / 2 * rect.Height / 2);
			}
			else {
				b = Math.Tan((90 - rollAngle) * _ToRadian) * rect.Width / 2;
				avaliableRadiusH = Math.Sqrt(b * b + rect.Width / 2 * rect.Width / 2);
			}
			return Math.Min(avaliableRadiusW - cardSize.Width / 4, avaliableRadiusH);
		}
		public override void Arrange(int currentRecordIndex) {
			if(View.OptionsCarouselMode.Radius == 0) radius = (int)AutoCalcRadius();
			else radius = View.OptionsCarouselMode.Radius;
			UpdatedXYAxis();
			base.Arrange(currentRecordIndex);
		}
		public override LayoutViewMode ViewMode { get { return LayoutViewMode.Carousel; } }
		protected override void UpdateAnimateParams(CustomAnimationInfo info, int dcount) {
			int fc1 = info.FrameCount - info.CurrentFrame;
			int fc2 = Math.Min(View.OptionsCarouselMode.FrameCount, View.OptionsCarouselMode.FrameCount * 2 - fc1);
			float da2 = dcount * DeltaAngle;
			this.animatedAngle = (this.animatedAngle * fc1 + da2) / (fc1 + fc2);
			info.FrameCount += fc2;
		}
		float animatedAngle = 0.0f;
		protected float AnimatedAngle { get { return animatedAngle; } }
		protected override void OnAnimation(BaseAnimationInfo info) {
			if(info == null || IsDisposingInProgress) return;
			if(info.PrevFrame < 0) return;
			if(View == null || View.GridControl == null) return;
			float angle = AnimatedAngle * (info.CurrentFrame - info.PrevFrame);
			Offset -= angle;
			if(info.IsFinalFrame) {
				EndAnimation();
			}
			View.GridControl.Invalidate(ViewInfo.ViewRects.CardsRect);
		}
		protected override void StartAnimation() {
			this.animatedAngle = DeltaCount * DeltaAngle / View.OptionsCarouselMode.FrameCount;
			base.StartAnimation();
		}
		const float _TwoPi = (float)(2.0 * Math.PI);
		const float _HalfOfPi = (float)(0.5 * Math.PI);
		const float _OneAndHalfOfPi = (float)(1.5 * Math.PI);
		const double _ToRadian = (Math.PI / 180.0);
		const double _ToDegree = (180.0 / Math.PI);
		public float DeltaAngle { get { return _TwoPi / View.OptionsCarouselMode.CardCount; } }
		protected override void InitializeBeginValue() {
			if(CardImageInfos.Count == 0) return;
			int cardIndex;
			for(cardIndex = CenterCardIndex; cardIndex >= 0; cardIndex--) {
				CardImageInfos[cardIndex].StartAngle = _HalfOfPi - (CenterCardIndex - cardIndex) * DeltaAngle;
			}
			for(cardIndex = CenterCardIndex + 1; cardIndex < CardImageInfos.Count; cardIndex++) {
				CardImageInfos[cardIndex].StartAngle = _HalfOfPi + (cardIndex - CenterCardIndex) * DeltaAngle;
			}
		}
		protected internal virtual PointF Origin {
			get {
				return new PointF(
						ViewInfo.ViewRects.CardsRect.X + ViewInfo.ViewRects.CardsRect.Width / 2 + View.OptionsCarouselMode.CenterOffset.X,
						ViewInfo.ViewRects.CardsRect.Y + ViewInfo.ViewRects.CardsRect.Height / 2 + View.OptionsCarouselMode.CenterOffset.Y
					);
			}
		}
		protected override void OnPropertiesChanged() {
			base.OnPropertiesChanged();
			InitializeBeginValue();
		}
		PointF xAxis;
		PointF yAxis;
		protected void UpdatedXYAxis() {
			xAxis = new PointF((float)Math.Cos(View.OptionsCarouselMode.RollAngle), (float)Math.Sin(View.OptionsCarouselMode.RollAngle));
			yAxis = new PointF(-xAxis.Y, xAxis.X);
		}
		public override Rectangle GetBounds(CardImageInfo info) {
			Rectangle rect = Rectangle.Empty;
			rect.X = (int)(Origin.X + radius * (float)Math.Cos(info.Angle) + 0.5f);
			rect.Y = (int)(Origin.Y - radius * (float)Math.Sin(info.Angle) * Math.Cos(View.OptionsCarouselMode.PitchAngle) + 0.5f);
			if(info.Images[0] != null) {
				float scale = info.SizeScale;
				rect.Width = (int)(info.Images[0].Width * scale + 0.5f);
				rect.Height = (int)(info.Images[0].Height * scale + 0.5f);
			}
			else {
				rect.Width = VisibleCards[0].Width;
				rect.Height = VisibleCards[0].Width;
			}
			PointF p = new PointF(rect.X - Origin.X, rect.Y - Origin.Y);
			rect.X = (int)(Origin.X + p.X * xAxis.X + p.Y * yAxis.X);
			rect.Y = (int)(Origin.Y + p.X * xAxis.Y + p.Y * yAxis.Y);
			rect.Offset(-(int)(rect.Width * 0.5f + 0.5f), -(int)(rect.Height * 0.5f + 0.5f));
			return rect;
		}
		public override float GetScale(CardImageInfo info) {
			CardImageInfo currSelInfo = CardImageInfos[CenterCardIndex];
			int newSelIndex = CenterCardIndex + DeltaCount;
			CardImageInfo newSelInfo = newSelIndex < 0 || newSelIndex >= CardImageInfos.Count ? null : CardImageInfos[newSelIndex];
			if(currSelInfo == info || newSelInfo == info)
				return GetInterpolatedValue(1.0f, View.OptionsCarouselMode.BottomCardScale, info.Angle);
			return GetInterpolatedValue(1.0f, View.OptionsCarouselMode.BottomCardScale, DeltaAngle, info.Angle);
		}
		public override float GetColorFade(CardImageInfo info) { return GetInterpolatedValue(1.0f, 1.0f - View.OptionsCarouselMode.BottomCardFading, info.Angle); }
		public override float GetAlpha(CardImageInfo info) { return GetInterpolatedValue(1.0f, View.OptionsCarouselMode.BottomCardAlphaLevel, info.Angle); }
		float GetInterpolatedValue(float beginValue, float endValue, float angle) {
			if(angle >= _OneAndHalfOfPi || angle <= -_HalfOfPi) return endValue;
			return beginValue + (float)Math.Sqrt(Math.Sqrt(AngleToT(angle))) * (endValue - beginValue);
		}
		float GetInterpolatedValue(float beginValue, float endValue, float boundAngle, float angle) {
			if(angle >= _OneAndHalfOfPi || angle <= -_HalfOfPi) return endValue;
			float a = angle;
			if(Math.Abs(angle - _HalfOfPi) < boundAngle) {
				a = _HalfOfPi;
				if(angle < _HalfOfPi) boundAngle = -boundAngle;
				a += boundAngle;
			}
			return beginValue + (float)Math.Sqrt(Math.Sqrt(AngleToT(a))) * (endValue - beginValue);
		}
		float AngleToT(float a) {
			if(a >= _OneAndHalfOfPi || a <= -_HalfOfPi) return 1.0f;
			return (float)Math.Abs(a - _HalfOfPi) / (float)Math.PI;
		}
		protected override void InitializeBeginValue(int cardIndex, int prevCardIndex) {
			CardImageInfo info = CardImageInfos[cardIndex];
			CardImageInfo prevInfo = CardImageInfos[prevCardIndex];
			if(cardIndex < prevCardIndex) info.StartAngle = prevInfo.StartAngle + DeltaAngle;
			else info.StartAngle = prevInfo.StartAngle - DeltaAngle;
		}
	}
}
