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
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.ComponentModel;
using DevExpress.Accessibility;
using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Localization;
using DevExpress.Utils;
using DevExpress.XtraEditors.Container;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Printing;
using DevExpress.XtraGrid.Views.Card.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.Data.Selection;
using System.Collections.Generic;
using DevExpress.Skins;
namespace DevExpress.XtraGrid.Views.Card {
	[DesignTimeVisible(false), ToolboxItem(false)]
	public class CardView : ColumnView, IDataControllerValidationSupport, IAccessibleGrid {
		Rectangle viewRect;
		ScrollVisibility vertScrollVisibility;
		int maximumCardRows, cardInterval, maximumCardColumns, topLeftCardIndex,
			cardWidth, printMaximumCardColumns, focusedCardTopFieldIndex, lockTopFieldChanging;
		CardState state;
		string cardCaptionFormat;
		CardFieldInfo editingField;
		ScrollInfo scrollInfo;
		DevExpress.XtraEditors.Controls.BorderStyles cardScrollButtonBorderStyle;
		private static readonly object customCardCaptionImage = new object();
		private static readonly object customDrawCardCaption = new object();
		private static readonly object customDrawCardFieldCaption = new object(), customDrawCardFieldValue = new object(), customDrawCardField = new object();
		private static readonly object cardExpanding = new object();
		private static readonly object cardExpanded = new object();
		private static readonly object cardCollapsing = new object();
		private static readonly object cardCollapsed = new object();
		private static readonly object calcFieldHeight = new object();
		private static readonly object topLeftCardChanged = new object();
		CardQuickCustomizationForm customizationForm = null;
		bool newRowEditing = false;
		public CardView(GridControl ownerGrid) : this() {
			SetGridControl(ownerGrid);
		}
		public CardView() {
			this.DataController.ValidationClient = this;
			this.cardScrollButtonBorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default; 
			this.focusedCardTopFieldIndex = 0;
			this.cardInterval = 10;
			this.vertScrollVisibility = ScrollVisibility.Never;
			this.cardCaptionFormat = string.Empty;
			this.editingField = null;
			this.printMaximumCardColumns = -1;
			this.state = CardState.Normal;
			this.cardWidth = 200;
			this.topLeftCardIndex = 0;
			this.viewRect = Rectangle.Empty;
			this.maximumCardRows = -1;
			this.maximumCardColumns = -1;
			this.scrollInfo = CreateScrollInfo();
			this.scrollInfo.HScroll_ValueChanged += new EventHandler(OnHScroll);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				scrollInfo.Dispose();
				DestroyCustomizationForm();
			}
			base.Dispose(disposing);
		}
		protected internal virtual int MaxLoadRows { get { return 200; } }
		protected override BaseViewInfo CreateNullViewInfo() { return new NullCardViewInfo(this); }
		protected override bool CanLeaveFocusOnTab(bool moveForward) {
			return false;
		}
		protected override void LeaveFocusOnTab(bool moveForward) { GridControl.ProcessControlTab(moveForward); }
		public override void Assign(BaseView v, bool copyEvents) {
			if(v == null) return;
			BeginUpdate();
			try {
				base.Assign(v, copyEvents);
				CardView cv = v as CardView;
				if(cv != null) {
					SyncCardProperties(cv);
					if(copyEvents) {
						Events.AddHandler(customCardCaptionImage, cv.Events[customCardCaptionImage]);
						Events.AddHandler(customDrawCardCaption, cv.Events[customDrawCardCaption]);
						Events.AddHandler(customDrawCardField, cv.Events[customDrawCardField]);
						Events.AddHandler(customDrawCardFieldCaption, cv.Events[customDrawCardFieldCaption]);
						Events.AddHandler(customDrawCardFieldValue, cv.Events[customDrawCardFieldValue]);
						Events.AddHandler(topLeftCardChanged, cv.Events[topLeftCardChanged]);
						Events.AddHandler(cardCollapsed, cv.Events[cardCollapsed]);
						Events.AddHandler(cardCollapsing, cv.Events[cardCollapsing]);
						Events.AddHandler(cardExpanded, cv.Events[cardExpanded]);
						Events.AddHandler(cardExpanding, cv.Events[cardExpanding]);
						Events.AddHandler(calcFieldHeight, cv.Events[calcFieldHeight]);
					}
				}
			}
			finally {
				EndUpdate();
			}
		}
		public new CardHitInfo CalcHitInfo(Point pt) { return base.CalcHitInfo(pt) as CardHitInfo; }
		public new CardHitInfo CalcHitInfo(int x, int y) { return CalcHitInfo(new Point(x, y)); }
		protected override BaseHitInfo CalcHitInfoCore(Point pt) { return ViewInfo.CalcHitInfo(pt); }
		public void CollapseAll() { SetCardCollapsedAll(true); }
		public override DevExpress.XtraGrid.Export.BaseExportLink CreateExportLink(DevExpress.XtraExport.IExportProvider provider) {
			return new DevExpress.XtraGrid.Export.CardViewExportLink(this, provider);
		}
		public void ExpandAll() { SetCardCollapsedAll(false); }
		List<GridColumn> cardCaptionFormatColumns;
		internal bool TryGetMatchedTextFromCaption(string text, out string matchedText, out int containsIndex) {
			matchedText = null; containsIndex = 0;
			if(cardCaptionFormatColumns == null)
				return false;
			foreach(GridColumn column in cardCaptionFormatColumns) {
				matchedText = GetFindMatchedText(column, text);
				if(!string.IsNullOrEmpty(matchedText)) {
					containsIndex = text.ToLower().IndexOf(matchedText);
					break;
				}
			}
			return !string.IsNullOrEmpty(matchedText);
		}
		protected virtual IEnumerable<GridColumn> GetCardCaptionFormatColumns() {
			return GetFormatColumns(CardCaptionFormat, 1);
		}
		protected override List<IDataColumnInfo> GetFindToColumnsCollection() {
			cardCaptionFormatColumns = new List<GridColumn>();
			List<IDataColumnInfo> res = base.GetFindToColumnsCollection();
			if(!string.IsNullOrEmpty(CardCaptionFormat)) {
				foreach(GridColumn column in GetCardCaptionFormatColumns()) {
					if(ContainsIDataColumnInfoForFilter(res, column) || !IsAllowFindColumn(column)) continue;
					cardCaptionFormatColumns.Add(column);
					res.Add(CreateIDataColumnInfoForFilter(column));
				}
			}
			return res;
		}
		public virtual string GetCardCaption(int rowHandle) {
			if(IsNewItemRow(rowHandle)) return GridLocalizer.Active.GetLocalizedString(GridStringId.CardViewNewCard);
			object[] val = new object[Columns.Count + 1];
			val[0] = (rowHandle + 1).ToString();
			for(int n = 0; n < Columns.Count; n++) {
				val[n + 1] = GetRowCellDisplayText(rowHandle, Columns[n]);
			}
			string s = string.Empty;
			try {
				string format = CardCaptionFormat;
				if(string.IsNullOrEmpty(format)) format = GridLocalizer.Active.GetLocalizedString(GridStringId.CardViewCaptionFormat);
				if(!string.IsNullOrEmpty(format))
					s = String.Format(format, val);
			}
			catch {
				return "";
			}
			return s;
		}
		public bool GetCardCollapsed(int rowHandle) {
			if(!OptionsBehavior.AllowExpandCollapse) return false;
			return CardSelection.GetCollapsed(rowHandle);
		}
		public override void ShowFilterPopup(GridColumn column) {
			CardFieldInfo fi = ViewInfo.Cards.CardFieldInfoBy(FocusedRowHandle, column);
			if(fi == null) return;
			Focus();
			ShowFilterPopup(column, fi.ValueBounds.IsEmpty ? fi.Bounds : fi.ValueBounds, GridControl, fi);
		}
		public override void HideEditor() {
			if(!IsEditing) return;
			base.HideEditor();
			if(editingField != null) {
				ViewInfo.PaintAnimatedItems = false;
				InvalidateRect(editingField.Bounds);
			}
			editingField = null;
			state = CardState.Normal;
			UpdateNavigator();
		}
		public void InvalidateCard(int rowHandle) {
			if(ViewInfo == null) return;
			CardInfo ci = ViewInfo.Cards.CardInfoByRow(rowHandle);
			if(ci != null) {
				ViewInfo.PaintAnimatedItems = false;
				InvalidateRect(ci.Bounds);
			}
		}
		protected internal override void Draw(GraphicsCache e) {
			if(e.Graphics.ClipBounds == ViewInfo.ViewRects.Bounds) ViewInfo.PaintAnimatedItems = false;
			base.Draw(e);
		}
		public void InvalidateCardField(int rowHandle, GridColumn column) {
			CardFieldInfo fi = ViewInfo.Cards.CardFieldInfoBy(rowHandle, column);
			if(fi != null) {
				ViewInfo.PaintAnimatedItems = false;
				InvalidateRect(fi.Bounds); 
			}
		}
		public override void InvalidateHitObject(DevExpress.XtraGrid.Views.Base.ViewInfo.BaseHitInfo hit) {
			CardHitInfo hitInfo = hit as CardHitInfo;
			if(!hitInfo.IsValid) return;
			ViewInfo.PaintAnimatedItems = false;
			if(hitInfo.InFilterPanel) {
				InvalidateRect(ViewInfo.FilterPanel.Bounds);
				return;
			}
			if(hitInfo.HitTest == CardHitTest.QuickCustomizeButton) {
				InvalidateRect(ViewInfo.ViewRects.QuickCustomize); 
				return;
			}
			if(hitInfo.HitTest == CardHitTest.CloseZoomButton) {
				InvalidateRect(ViewInfo.ViewRects.CloseZoom); 
				return;
			}
			if(hitInfo.RowHandle == GridControl.InvalidRowHandle) return;
			CardInfo ci = ViewInfo.Cards.CardInfoByRow(hitInfo.RowHandle);
			if(ci == null) return;
			CardFieldInfo fi = ci.Fields.FieldInfoByColumn(hitInfo.Column);
			switch(hitInfo.HitTest) {
				case CardHitTest.CardDownButton : InvalidateRect(ci.DownButtonBounds); break;
				case CardHitTest.CardUpButton : InvalidateRect(ci.UpButtonBounds); break;
				case CardHitTest.CardExpandButton : 
				case CardHitTest.CardCaption : InvalidateRect(ci.CaptionInfo.Bounds); break;
				case CardHitTest.Field : 
				case CardHitTest.FieldCaption : 
				case CardHitTest.FieldValue : if(fi != null) InvalidateRect(fi.Bounds); break;
				default:
					InvalidateRect(ci.Bounds);
					break;
			}
		}
		public CardVisibleState IsCardVisible(int rowHandle) {
			CardInfo ci = ViewInfo.Cards.CardInfoByRow(rowHandle);
			if(ci == null) return CardVisibleState.Hidden;
			if(ci.Bounds.Right > ViewInfo.ViewRects.Cards.Right) 
				return CardVisibleState.Partially;
			return CardVisibleState.Visible;
		}
		public override void LayoutChanged() {
			if(!CalculateLayout()) return;
			base.LayoutChanged();
			UpdateCustomizationForm();
		}
		protected internal override bool PostEditor(bool causeValidation) {
			if(ActiveEditor == null || !EditingValueModified || editingField == null) return true;
			if(causeValidation && !ValidateEditor()) return false;
			object value = ExtractEditingValue(editingField.Column, EditingValue);
			SetRowCellValueCore(FocusedRowHandle, editingField.Column, value, true);
			return true;
		}
		public void ExpandCard(int rowHandle) { SetCardCollapsed(rowHandle, false); }
		public void CollapseCard(int rowHandle) { SetCardCollapsed(rowHandle, true); }
		public void SetCardCollapsed(int rowHandle, bool collapsed) {
			if(!OptionsBehavior.AllowExpandCollapse) return;
			if(GetCardCollapsed(rowHandle) != collapsed) {
				bool allow = collapsed ? RaiseCardCollapsing(rowHandle) : RaiseCardExpanding(rowHandle);
				if(!allow) return;
				CardSelection.SetCollapsed(rowHandle, collapsed);
				if(collapsed) FocusedColumn = null;
				LayoutChangedSynchronized();
				TopLeftCardIndex = CheckTopRowIndex(TopLeftCardIndex);
				if(collapsed)
					RaiseCardCollapsed(rowHandle);
				else
					RaiseCardExpanded(rowHandle);
			}
		}
		public override void ShowEditor() {
			if(GetCardCollapsed(FocusedRowHandle)) return;
			CloseEditor();
			if(State != CardState.Normal || !GetCanShowEditor(FocusedColumn)) return;
			CardFieldInfo fi = ViewInfo.Cards.CardFieldInfoBy(FocusedRowHandle, FocusedColumn);
			if(fi == null) return;
			if(fi.ValueBounds.Width > 6) {
				ActivateEditor(fi);
			}
		}
		public override void SynchronizeVisual(BaseView viewSource) {
			if(viewSource == null) return;
			BeginSynchronization();
			BeginUpdate();
			try {
				base.SynchronizeVisual(viewSource);
				CardView cv = viewSource as CardView;
				if(cv == null) return;
				SyncCardProperties(cv);
			}
			finally {
				EndUpdate();
				EndSynchronization();
			}
		}
		protected internal override bool CanShowPopupObjects { 
			get { 
				if(base.CanShowPopupObjects) return true;
				return CustomizationForm != null;
			} 
		}
		protected internal override void AddToGridControl() {
			if(GridControl != null) {
				ScrollInfo.AddControls(GridControl);
			}
			base.AddToGridControl();
		}
		protected internal override void RemoveFromGridControl() {
			if(GridControl != null) {
				ScrollInfo.RemoveControls(GridControl);
			}
			base.RemoveFromGridControl();
		}
		protected virtual int CalcValidFocusedCardTopFieldIndex(int newValue) {
			if(FocusedRowHandle == GridControl.InvalidRowHandle) return 0;
			if(VertScrollVisibility == ScrollVisibility.Never) return 0;
			int maxCount = ViewInfo.GetScrollableColumnsCount(FocusedRowHandle);
			if(newValue >= maxCount) newValue = maxCount - 1;
			newValue = Math.Max(newValue, 0);
			return newValue;
		}
		protected internal override void OnColumnSortInfoCollectionChanged(CollectionChangeEventArgs e) {
			base.OnColumnSortInfoCollectionChanged(e);
			if(SortInfo.IsLockUpdate) return;
			TopLeftCardIndex = CheckTopRowIndex(TopLeftCardIndex);
		}
		protected int CheckTopRowIndex(int newTopRowIndex) { return CheckTopRowIndex(newTopRowIndex, false); }
		protected override void DoInternalLayout() {
			base.DoInternalLayout();
			ViewInfo.UpdateAnimatedItems();
		}
		protected override internal void DoMouseSortColumn(GridColumn column, Keys key) {
			base.DoMouseSortColumn(column, key);
			TopLeftCardIndex = CheckTopRowIndex(TopLeftCardIndex);
		}
		protected virtual int CheckTopRowIndex(int newTopRowIndex, bool force) {
			if(IsLoading || (IsLockUpdate && !force)) return newTopRowIndex;
			if(newTopRowIndex < 0) newTopRowIndex = 0;
			if(newTopRowIndex == 0 || ViewRect.IsEmpty || BaseInfo == null) return newTopRowIndex;
			if(!ViewDisposing) {
				CardViewInfo vi = BaseInfo.CreateViewInfo(this) as CardViewInfo;
				newTopRowIndex = vi.CalcBestTopCard(newTopRowIndex);
			}
			return newTopRowIndex;
		}
		protected override BaseViewAppearanceCollection CreateAppearancesPrint() { return new CardViewPrintAppearances(this); }
		void ResetAppearancePrint() { AppearancePrint.Reset(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewAppearancePrint"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance)
		]
		public new CardViewPrintAppearances AppearancePrint { get { return base.AppearancePrint as CardViewPrintAppearances; } }
		protected override BaseViewAppearanceCollection CreateAppearances() { return new CardViewAppearances(this); }
		protected override BaseGridController CreateDataController() {
			return new CardViewDataController();
		}
		protected override ColumnViewOptionsBehavior CreateOptionsBehavior() { return new CardOptionsBehavior(this); }
		protected override ViewPrintOptionsBase CreateOptionsPrint() { return new CardOptionsPrint(); }
		protected override ColumnViewOptionsView CreateOptionsView() { return new CardOptionsView(); }
		protected override BaseViewPrintInfo CreatePrintInfoInstance(PrintInfoArgs args) {
			return new CardViewPrintInfo(args);
		}
		protected virtual ScrollInfo CreateScrollInfo() {
			return 	new ScrollInfo(this);
		}
		protected override void DoChangeFocusedRow(int currentRowHandle, int newRowHandle, bool raiseUpdateCurrentRow) {
			if(!CheckCanLeaveRow(currentRowHandle, raiseUpdateCurrentRow)) return;
			newRowHandle = CheckRowHandle(currentRowHandle, newRowHandle);
			if(currentRowHandle == newRowHandle && IsValidRowHandle(DataController.CurrentControllerRow)) return;
			int prevFocused = currentRowHandle;
			SetFocusedRowHandleCore(newRowHandle);
			UpdateRowViewInfo(prevFocused);
			InvalidateCard(prevFocused);
			MakeRowVisibleCore(FocusedRowHandle, true);
			UpdateRowViewInfo(FocusedRowHandle);
			DataController.CurrentControllerRow = FocusedRowHandle;
			FocusedColumn = null;
			if(FocusedCardTopFieldIndex != 0) {
				focusedCardTopFieldIndex = 0;
				LayoutChangedSynchronized();
			}
			base.DoChangeFocusedRow(prevFocused, FocusedRowHandle, raiseUpdateCurrentRow);
		}
		protected override void DoAfterFocusedColumnChanged(GridColumn prevFocusedColumn, GridColumn focusedColumn) {
			InvalidateCard(FocusedRowHandle);
			base.DoAfterFocusedColumnChanged(prevFocusedColumn, focusedColumn);
			UpdateRowViewInfo(FocusedRowHandle);
			InvalidateCard(FocusedRowHandle);
		}
		protected virtual void DoTopLeftCardIndexChanged() {
			LayoutChangedSynchronized();
			EventHandler handler = (EventHandler)this.Events[topLeftCardChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected override BaseHitInfo GetHintObjectInfo() { 
			return ViewInfo.SelectionInfo.HotTrackedInfo; 
		}
		protected override void MakeRowVisibleCore(int rowHandle, bool invalidate) {
			CardVisibleState cs = IsCardVisible(rowHandle);
			if(cs == CardVisibleState.Visible) {
				if(invalidate) InvalidateCard(rowHandle);
				return;
			}
			if(cs == CardVisibleState.Partially) {
				if(invalidate) InvalidateCard(rowHandle);
				if(ViewInfo.CalcMaxCardHorzIndex() < 1) { 
					return;
				}
				TopLeftCardIndex += CalcHSmallChange();
				return;
			}
			int vIndex = GetVisibleIndex(rowHandle);
			if(vIndex >= 0) TopLeftCardIndex = vIndex;
		}
		protected override void OnColumnAdded(GridColumn column) {
			base.OnColumnAdded(column);
			DestroyCustomizationForm();
		}
		protected override void OnColumnDeleted(GridColumn column) {
			BeginUpdate();
			try {
				base.OnColumnDeleted(column);
				DestroyCustomizationForm();
			} finally {
				EndUpdate();
			}
		}
		protected override void OnGridControlChanged(GridControl prevControl) {
			base.OnGridControlChanged(prevControl);
			ScrollInfo.RemoveControls(prevControl);
			ScrollInfo.AddControls(GridControl);
		}
		protected override object GetCellInfo(BaseHitInfo hitInfo) { 
			CardHitInfo card = hitInfo as CardHitInfo;
			if(card != null && card.InField) {
				return ViewInfo.Cards.CardFieldInfoBy(card.RowHandle, card.Column);
			}
			return null;
		}
		protected override BaseEditViewInfo GetCellEditInfo(object cellInfo) { 
			CardFieldInfo fi = cellInfo as CardFieldInfo;
			if(fi != null) return fi.EditViewInfo;
			return null; 
		}
		protected internal override bool OnCheckHotTrackMouseMove(BaseHitInfo hitInfo) {
			CardHitInfo hit = hitInfo as CardHitInfo;
			if(hit.InField) return OnCellMouseMove(hitInfo);
			return true;
		}
		protected override void OnHotTrackEnter(BaseHitInfo hitInfo) {
			CardHitInfo hit = hitInfo as CardHitInfo;
			bool needInvalidate = false;
			if(hit.InField) OnCellMouseEnter(hitInfo);
			else 
				needInvalidate = true;
			if(needInvalidate)
				InvalidateHitObject(hit);
		}
		protected override void OnHotTrackLeave(BaseHitInfo hitInfo) {
			CardHitInfo hit = hitInfo as CardHitInfo;
			bool needInvalidate = false;
			if(hit.InField) OnCellMouseLeave(hitInfo);
			else 
				needInvalidate = true;
			if(needInvalidate) {
				InvalidateHitObject(hit);
			}
		}
		protected internal override bool IsScrollingState {
			get { return State == CardState.Scrolling; }
		}
		protected internal override BaseViewOfficeScroller CreateScroller() { return new CardViewOfficeScroller(this); }
		protected internal override void SetScrollingState() { 
			SetState(CardState.Scrolling);
		}
		protected virtual void OnViewStyles_Changed(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		protected override void RefreshRow(int rowHandle, bool updateEditor, bool updateEditorValue) {
			UpdateRowViewInfo(rowHandle);
			InvalidateCard(rowHandle);
			base.RefreshRow(rowHandle, updateEditor, updateEditorValue);
		}
		protected void SetCardCollapsedAll(bool collapsed) {
			if(!OptionsBehavior.AllowExpandCollapse) return;
			int rowHandle = InvalidRowHandle;
			bool allow = collapsed ? RaiseCardCollapsing(rowHandle) : RaiseCardExpanding(rowHandle);
			if(!allow) return;
			if(collapsed)
				CardSelection.CollapseAll();
			else
				CardSelection.ExpandAll();
			LayoutChangedSynchronized();
			TopLeftCardIndex = CheckTopRowIndex(TopLeftCardIndex);
			if(collapsed)
				RaiseCardCollapsed(rowHandle);
			else
				RaiseCardExpanded(rowHandle);
		}
		protected override void SetEditingState() {
			state = CardState.Editing;
		}
		protected override void SetRowCellValueCore(int rowHandle, GridColumn column, object value, bool fromEditor) {
			base.SetRowCellValueCore(rowHandle, column, value, fromEditor);
			UpdateRowViewInfo(rowHandle);
			InvalidateCardField(rowHandle, column);
		}
		protected internal override void OnActionScroll(ScrollNotifyAction action) {
			if(!CanActionScroll(action)) return;
			ScrollInfo.OnAction(action);
		}
		protected override void SetViewRect(Rectangle newValue) {
			if(ViewRect == newValue) return;
			if(ViewDisposing) {
				viewRect = Rectangle.Empty;
				return;
			}
			DestroyCustomizationForm();
			Rectangle prev = viewRect;
			viewRect = newValue;
			fUpdateSize ++;
			try {
				if(prev.IsEmpty) CheckSynchronize();
				BeginUpdate();
				try {
					if(this.lockTopFieldChanging == 0) FocusedCardTopFieldIndex = 0;
					TopLeftCardIndex = CheckTopRowIndex(TopLeftCardIndex, true); 
				} finally {
					EndUpdateCore(true);
				}
			}
			finally {
				fUpdateSize --;
			}
		}
		protected override bool UpdateCellHotInfo(BaseHitInfo hitInfo, Point hitPoint) {
			CardHitInfo hit = hitInfo as CardHitInfo;
			CardFieldInfo fi = ViewInfo.Cards.CardFieldInfoBy(hit.RowHandle, hit.Column);
			if(fi == null || fi.EditViewInfo == null) return false;
			if(fi.Editor == null || !fi.EditViewInfo.IsRequiredUpdateOnMouseMove) return false;
			if(fi.EditViewInfo.UpdateObjectState(MouseButtons.None, hitPoint)) return true;
			return false;
		}
		protected void UpdateCustomizationForm() {
			if(CustomizationForm != null) CustomizationForm.UpdateColumns();
		}
		protected void UpdateHScrollBar() {
			scrollInfo.HScrollVisible = IsNeededHScrollBar;
			if(scrollInfo.HScrollVisible) {
				ScrollArgs args = new ScrollArgs();
				int smallChange = 1, largeChange = 1;
				if(ViewInfo.IsReady) {
					smallChange = CalcHSmallChange();
					largeChange = Math.Max(smallChange, ViewInfo.CalcFullyVisibleCardsCount());
				}
				args.Minimum = 0;
				args.Maximum = Math.Max(RowCount > 0 ? RowCount - 1 : 0, 0);
				args.Enabled = true;
				args.LargeChange = largeChange;
				args.SmallChange = smallChange;
				args.Value = TopLeftCardIndex;
				args.Value = Math.Max(Math.Min(args.Value, args.Maximum), 0);
				if(args.LargeChange > args.Maximum && args.Value == 0) {
					args.Maximum = 0;
				}
				if(args.Maximum == 0) {
					args.Enabled = false;
				}
				args.Enabled &= OptionsView.ShowHorzScrollBar;
				args.Value = Math.Max(args.Value, 0);
				scrollInfo.HScrollArgs = args;
			}
		}
		protected virtual void UpdateRowViewInfo(int rowHandle) {
			if(ViewInfo == null) return;
			CardInfo ci =  ViewInfo.Cards.CardInfoByRow(rowHandle);
			if(ci != null) {
				ViewInfo.SetCardInfoDirty(ci);
				if(OptionsView.GetAnimationType() == GridAnimationType.AnimateFocusedItem && ci.RowHandle == FocusedRowHandle)
					ViewInfo.AddAnimatedItems(ci);
			}
		}
		protected override void UpdateScrollBars() {
			UpdateHScrollBar();
			UpdateVScrollBar();
			ScrollInfo.ClientRect = ViewInfo.CalcScrollRect();
			ScrollInfo.UpdateScrollRects();
		}
		protected void UpdateVScrollBar() {
			scrollInfo.VScrollVisible = IsNeededVScrollBar;
		}
		protected override void VisualClientUpdateRow(int controllerRowHandle) {
			RefreshRow(controllerRowHandle, false);
			if(!OptionsView.ShowEmptyFields) {
				LayoutChangedSynchronized();
			}
		}
		protected override void VisualClientUpdateScrollBar() {
			UpdateScrollBars();
		}
		protected internal void ActivateEditor(CardFieldInfo fi) {
			if(fi == null || !fi.Column.OptionsColumn.AllowEdit) {
				return;
			}
			Rectangle bounds = ViewInfo.CheckBounds(fi.ValueBounds, ViewInfo.ViewRects.Cards);
			if(bounds.Width < 1 || bounds.Height < 1) return;
			this.editingField = fi;
			ViewInfo.PaintAnimatedItems = false;
			InvalidateRect(this.editingField.Bounds);
			ViewInfo.UpdateBeforePaint(fi.Card);
			AppearanceObject appearance = new AppearanceObject(), appearanceFocused = GetFocusedAppearance();
			AppearanceHelper.Combine(appearance, new AppearanceObject[] { fi.Editor.Appearance, appearanceFocused, fi.PaintAppearanceValue });
			UpdateEditor(fi.Editor, new UpdateEditorInfoArgs(GetColumnReadOnly(fi.Column), bounds, appearance, 
				fi.EditViewInfo.EditValue, 
				ElementsLookAndFeel, 
				fi.EditViewInfo.ErrorIconText, 
				fi.EditViewInfo.ErrorIcon, 
				IsRightToLeft));
		}
		AppearanceObject GetFocusedAppearance() {
			if(!PaintStyle.IsSkin) return null;
			AppearanceObject res = new AppearanceObject();
			Skin skin = GridSkins.GetSkin(this);
			if(skin.Colors.Contains(GridSkins.OptColorCardFocusedFieldBackColor))
				res.BackColor = skin.Colors[GridSkins.OptColorCardFocusedFieldBackColor];
			if(skin.Colors.Contains(GridSkins.OptColorCardFocusedFieldForeColor))
				res.ForeColor = skin.Colors[GridSkins.OptColorCardFocusedFieldForeColor];
			return res;
		}
		protected internal int CalcHSmallChange() {
			if(ViewInfo.IsReady) return Math.Max(1, ViewInfo.CalcRowsInColumn(0));
			return 1;
		}
		protected internal override int CalcRealViewHeight(Rectangle viewRect) {
			if(RequireSynchronization != SynchronizationMode.None) {
				CheckSynchronize();
				calculatedRealViewHeight = -1;
			}
			if(calculatedRealViewHeight != -1 && viewRect.Size == ViewRect.Size)
				return calculatedRealViewHeight;
			int result = viewRect.Height;
			BaseViewInfo tempViewInfo = new CardViewInfo(this),
				oldViewInfo = ViewInfo, copyFromInfo = ViewInfo;
			this.fViewInfo = tempViewInfo;
			RefreshVisibleColumnsList();
			try {
				ViewInfo.PrepareCalcRealViewHeight(viewRect, copyFromInfo);
				ViewInfo.LoadVisibleRows(TopLeftCardIndex);
				result = ViewInfo.CalcRealViewHeight(viewRect);
			}
			catch {
			}
			copyFromInfo = ViewInfo;
			this.fViewInfo = oldViewInfo;
			calculatedRealViewHeight = result;
			return result;
		}
		protected internal override void CheckInfo() {
			base.CheckInfo();
			if(GridControl == null) return;
			ScrollInfo.UpdateLookAndFeel(ElementsLookAndFeel);
		}
		protected internal override ViewDrawArgs CreateDrawArgs(DXPaintEventArgs e, GraphicsCache cache) {
			if(cache == null) cache = new GraphicsCache(e, Painter.Paint);
			return new CardViewDrawArgs(cache, ViewInfo, ViewInfo.ViewRects.Bounds);
		}
		protected internal override void CreateHandles() {
			base.CreateHandles();
			if(IsLoading) return;
			if(ScrollInfo != null && GridControl.IsHandleCreated) ScrollInfo.CreateHandles();
		}
		protected new CardViewPrintInfo PrintInfo { get { return base.PrintInfo as CardViewPrintInfo; } }
		protected internal void DestroyCustomizationForm() {
			if(CustomizationForm == null) return;
			CustomizationForm = null;
			ViewInfo.PaintAnimatedItems = false;
			InvalidateRect(ViewInfo.ViewRects.QuickCustomize);
		}
		protected override ColumnFilterInfo DoCustomFilter(GridColumn column, ColumnFilterInfo filterInfo) {
			DestroyCustomizationForm();
			return base.DoCustomFilter(column, filterInfo);
		}
		protected internal void DoMoveFocusedColumn(int delta, Keys byKey) {
			if(FocusedRowHandle == DevExpress.Data.DataController.InvalidRow)
				FocusedRowHandle = GetVisibleRowHandle(0);
			CardInfo ci = ViewInfo.Cards.CardInfoByRow(FocusedRowHandle);
			if(FocusedRowHandle == DevExpress.Data.DataController.InvalidRow || ci == null) return;
			if(GetCardCollapsed(ci.RowHandle)) {
				DoMoveFocusedRow(delta, new KeyEventArgs(byKey));
				return;
			}
			CardFieldInfo fi = ViewInfo.Cards.CardFieldInfoBy(FocusedRowHandle, FocusedColumn);
			int curIndex = ci.Fields.IndexOf(fi);
			if(curIndex < 0 && !OptionsView.ShowCardCaption) curIndex = 0;
			if(delta > 0) 
				curIndex ++;
			else
				curIndex --;
			if(curIndex >= ci.Fields.Count || curIndex < -1 || (curIndex == -1 && FocusedCardTopFieldIndex > 0) || (curIndex == -1 && !OptionsView.ShowCardCaption)) {
				if(delta > 0) {
					if(ci.CanScrollDown) {
						FocusedCardTopFieldIndex ++;
						ci = ViewInfo.Cards.CardInfoByRow(FocusedRowHandle);
						if(ci != null && ci.Fields.Count > 0) {
							fi = ci.Fields[ci.Fields.Count - 1];
							FocusedColumn = fi.Column;
						}
						return;
					}
				} else {
					if(FocusedCardTopFieldIndex > 0) {
						FocusedCardTopFieldIndex --;
						ci = ViewInfo.Cards.CardInfoByRow(FocusedRowHandle);
						if(ci != null && ci.Fields.Count > 0) {
							fi = ci.Fields[0];
							FocusedColumn = fi.Column;
						}
						return;
					}
				}
				DoMoveFocusedRow(curIndex > 0 ? 1 : -1, new KeyEventArgs(byKey));
				return;
			}
			if(curIndex > -1) {
				fi = ci.Fields[curIndex];
				FocusedColumn = fi.Column;
			}
			else
				FocusedColumn = null;
		}
		protected internal override void DoMoveFocusedRow(int delta, KeyEventArgs e) {
			int prevFocusedHandle = FocusedRowHandle;
			if(IsNewItemRow(FocusedRowHandle) && !CheckCanLeaveCurrentRow(true)) return;
			try {
				if(FocusedRowHandle == DevExpress.Data.DataController.InvalidRow) {
					FocusedRowHandle = GetVisibleRowHandle(0);
					return;
				}
				if(e.KeyCode == Keys.Home) {
					FocusedRowHandle = GetVisibleRowHandle(0);
					return;
				}
				if(e.KeyCode == Keys.End) {
					FocusedRowHandle = GetVisibleRowHandle(RowCount - 1);
					return;
				}
				int fv = DataController.GetVisibleIndex(FocusedRowHandle);
				fv += delta;
				if(IsDetailView && !IsZoomedView && (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)) {
					if(fv < 0) {
						if(!CheckCanLeaveRow(FocusedRowHandle, true)) return;
						GridControl.FocusedView = ParentView;
						return;
					}
					if(fv >= DataController.VisibleCount) {
						BaseView detail = ParentView.CanMoveFocusedRow(1);
						if(detail != null && detail.IsVisible) {
							if(!CheckCanLeaveRow(FocusedRowHandle, true)) return;
							detail.DoMoveFocusedRow(1, new KeyEventArgs(Keys.None));
							GridControl.FocusedView = detail;
							return;
						}
					}
				}
				if(fv < 0) fv = 0;
				if(fv >= DataController.VisibleCount) fv = DataController.VisibleCount - 1;
				FocusedRowHandle = GetVisibleRowHandle(fv);
			} finally {
				DoAfterMoveFocusedRow(e, prevFocusedHandle, null, null);
			}
		}
		protected override ToolTipControlInfo GetToolTipObjectInfoCore(GraphicsCache cache, Point p) {
			CardHitInfo ht = GetHintObjectInfo() as CardHitInfo;
			if(ht == null) return null;
			if(ht.HitTest == CardHitTest.CardCaptionErrorIcon) {
				CardInfo ci = ViewInfo.Cards.CardInfoByRow(ht.RowHandle);
				if(ci != null && ci.CaptionInfo.ErrorText != null && ci.CaptionInfo.ErrorText.Length > 0) { 
					ToolTipControlInfo res = new ToolTipControlInfo(string.Format("Error:{0}", ci.RowHandle), ci.CaptionInfo.ErrorText, true, ToolTipIconType.None);
					res.ToolTipImage = ci.CaptionInfo.ErrorIcon;
					return res;
				}
			}
			if(ht.InField) {
				CardFieldInfo fi = ViewInfo.Cards.CardFieldInfoBy(ht.RowHandle, ht.Column);
				if(fi != null) return GetCellEditToolTipInfo(fi.EditViewInfo, p, ht.RowHandle, ht.Column);
			}
			return null;
		}
		protected internal override void OnChildLayoutChanged(BaseView childView) {
		}
		protected internal override void OnColumnVisibleIndexChanged(GridColumn column) {
			BeginUpdate();
			try {
				bool prevVisible = VisibleColumns.Contains(column);
				base.OnColumnVisibleIndexChanged(column);
				if(VisibleColumns.Contains(column) != prevVisible)
					TopLeftCardIndex = CheckTopRowIndex(TopLeftCardIndex, true);
			} 
			finally {
				EndUpdate();
			}
		}
		protected internal override void OnEmbeddedNavigatorSizeChanged() {
			if(ViewDisposing || !IsInitialized) return;
			base.OnEmbeddedNavigatorSizeChanged();
			ScrollInfo.LayoutChanged();
		}
		protected internal override void OnFilterPopupCloseUp(GridColumn column) {
			base.OnFilterPopupCloseUp(column);
			UpdateCustomizationForm();
		}
		protected internal override void OnGotFocus() {
			if(InternalFocusLock != 0) return;
			base.OnGotFocus();
			Invalidate();
		}
		protected internal override void OnLostFocus() {
			if(InternalFocusLock != 0) return;
			base.OnLostFocus();
			Invalidate();
		}
		protected override object GetSelectedData() { 
			int[] rows = GetSelectedRows();
			if(rows == null) return null;
			ArrayList list = new ArrayList();
			for(int n = 0; n < rows.Length; n++) {
				list.Add(GetRow(rows[n]));
			}
			return list;
		}
		protected override string GetText() { 
			int[] rows = GetSelectedRows();
			if(rows == null) return string.Empty;
			StringBuilder sb = new StringBuilder();
			for(int n = 0; n < rows.Length; n++) {
				if(!GetCardText(sb, rows[n])) continue;
				sb.Append(crlf);
			}
			return sb.ToString();
		}
		protected virtual bool GetCardText(StringBuilder sb, int rowHandle) {
			if(!IsValidRowHandle(rowHandle)) return false;
			sb.Append(GetCardCaption(rowHandle));
			sb.Append(crlf);
			for(int n = 0; n < VisibleColumns.Count; n++) {
				sb.Append(GetRowCellDisplayText(rowHandle, VisibleColumns[n]));
				sb.Append(crlf);
			}
			return true;
		}
		protected internal override void OnVisibleChanged() {
			base.OnVisibleChanged();
			if(GridControl != null && GridControl.Visible) {
				ScrollInfo.UpdateVisibility();
			}
		}
		protected internal override void SetDefaultState() {
			SetState(CardState.Normal);
		}
		protected internal void SetState(CardState newValue) {
			if(State == newValue) return;
			CardState prevState = state;
			if(state == CardState.Sizing) 
				Painter.StopSizing();
			if(IsEditing) 
				CloseEditor();
			if(State == CardState.Scrolling) StopScrolling();
			if(newValue == CardState.CardUpButtonPressed) {
				if(FocusedCardTopFieldIndex == 0) newValue = CardState.Normal;
			}
			if(newValue == CardState.CardDownButtonPressed) {
				CardInfo ci = ViewInfo.Cards.CardInfoByRow(FocusedRowHandle);
				if(ci == null || !ci.CanScrollDown) newValue = CardState.Normal; 
			}
			state = newValue;
			if(IsDefaultState) 
				ViewInfo.SelectionInfo.ClearPressedInfo();
			else {
				HideHint();
			}
			UpdateNavigator();
			Invalidate();
			FireChanged();
		}
		protected virtual CardQuickCustomizationForm CreateCardQuickCustomizationForm() {
			return new CardQuickCustomizationForm(this);
		}
		protected internal void ShowQuickCustomization() {
			if(ViewInfo.ViewRects.QuickCustomize.IsEmpty) return;
			CustomizationForm = CreateCardQuickCustomizationForm();
			CustomizationForm.Init();
			CustomizationForm.CreateColumns();
			if(CustomizationForm.CanShow) 
				CustomizationForm.Show(ViewInfo.ViewRects.QuickCustomize);
			else
				DestroyCustomizationForm();
		}
		void IDataControllerValidationSupport.OnBeginCurrentRowEdit() { }
		void IDataControllerValidationSupport.OnCurrentRowUpdated(ControllerRowEventArgs e) { 
			RaiseRowUpdated(new RowObjectEventArgs(e.RowHandle, e.Row));
		}
		void IDataControllerValidationSupport.OnControllerItemChanged(ListChangedEventArgs e) { 
			if(OptionsBehavior.AutoFocusNewCard && e.ListChangedType == ListChangedType.ItemAdded) {
				if(IsValidRowHandle(e.NewIndex)) FocusedRowHandle = e.NewIndex;
			}
			if(e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted) {
				ClearPrevSelectionInfo();
				ClearColumnErrors();
				DataController.SyncCurrentRow();
		}
		}
		protected override void OnCurrentControllerRowChanged(CurrentRowEventArgs e) { 
			FocusedRowHandle = DataController.CurrentControllerRow;
			MakeRowVisibleCore(FocusedRowHandle, true);
		}
		void IDataControllerValidationSupport.OnEndNewItemRow() { 
			this.newRowEditing = false;
			LayoutChangedSynchronized();
			RefreshRow(CurrencyDataController.NewItemRow);
		}
		void IDataControllerValidationSupport.OnPostCellException(ControllerRowCellExceptionEventArgs e) { 
			throw e.Exception;
		}
		void IDataControllerValidationSupport.OnPostRowException(ControllerRowExceptionEventArgs e) { 
			OnPostRowException(e);
		}
		void IDataControllerValidationSupport.OnStartNewItemRow() {
			if(DesignMode) return;
			this.newRowEditing = true;
			DoChangeFocusedRowInternal(DevExpress.Data.CurrencyDataController.NewItemRow, false);
			if(FocusedRowHandle == CurrencyDataController.NewItemRow)
				DataController.BeginCurrentRowEdit();
			RaiseInitNewRow(new InitNewRowEventArgs(CurrencyDataController.NewItemRow));
			if(RowCount == 1 || IsCardVisible(CurrencyDataController.NewItemRow) != CardVisibleState.Visible) LayoutChangedSynchronized();
		}
		void IDataControllerValidationSupport.OnValidatingCurrentRow(ValidateControllerRowEventArgs e) { 
			if(e.RowHandle != FocusedRowHandle) return;
			OnValidatingCurrentRow(e);
		}
		void OnHScroll(object sender, EventArgs e) {
			try {
				GridControl.EditorHelper.BeginAllowHideException();
				TopLeftCardIndex = scrollInfo.HScrollPosition;
			}
			catch(HideException) {
				UpdateHScrollBar();
			}
			finally {
				GridControl.EditorHelper.EndAllowHideException();
			}
		}
		void SyncCardProperties(CardView sourceView) {
			this.PrintMaximumCardColumns = sourceView.PrintMaximumCardColumns;
			this.MaximumCardRows = sourceView.MaximumCardRows;
			this.MaximumCardColumns = sourceView.MaximumCardColumns;
			this.CardWidth = sourceView.CardWidth;
			this.CardCaptionFormat = sourceView.CardCaptionFormat;
			this.CardInterval = sourceView.CardInterval;
			this.VertScrollVisibility = sourceView.VertScrollVisibility;
			this.CardScrollButtonBorderStyle = sourceView.CardScrollButtonBorderStyle;
		}
		internal void DoCardsSizing(Point p) {
			int newPosition = -1000;
			if(State == CardState.Sizing) {
				if(p.X < ViewInfo.ViewRects.Cards.Left) {
					p.X = ViewInfo.ViewRects.Cards.Left;
				}
				newPosition = p.X;
			}
			if(newPosition != Painter.CurrentSizerPos) {
				Painter.HideSizerLine();
				Painter.CurrentSizerPos = newPosition;
				Painter.ShowSizerLine();
			}
		}
		internal void DoMoveFocusedRowHorz(int delta, Keys byKey) {
			CardInfo ci = ViewInfo.Cards.CardInfoByRow(FocusedRowHandle);
			if(ci == null) return;
			int prevFocusedHandle = FocusedRowHandle;
			bool edgeCard = false;
			if(delta < 0 && ci.Position.X == 0) edgeCard = true;
			if(delta > 0 && ci.Position.X == ViewInfo.CalcMaxCardHorzIndex()) edgeCard = true;
			try {
				if(edgeCard) {
					int sm = CalcHSmallChange() * delta;
					TopLeftCardIndex += sm;
					FocusedRowHandle += sm;
					return;
				}
				int destHorz = ci.Position.X + (delta < 0 ? -1 : 1);
				CardInfo dest = null;
				int y = (ci.CaptionInfo.Bounds.IsEmpty ? ci.Bounds.Y : ci.CaptionInfo.Bounds.Y);
				foreach(CardInfo c in ViewInfo.Cards) {
					if(c.Position.X == destHorz) {
						if(c.Bounds.Top <= y && 
							c.Bounds.Bottom >= y) {
							dest = c;
							break;
						}
						if(c.Bounds.Top <= y) 
							dest = c;
					}
				}
				if(dest != null)
					FocusedRowHandle = dest.RowHandle;
			} 
			finally {
				DoAfterMoveFocusedRow(new KeyEventArgs(byKey), prevFocusedHandle, null, null);
			}
		}
		internal void EndCardsSizing() {
			Painter.HideSizerLine();
			if(Painter.StartSizerPos != Painter.CurrentSizerPos) {
				int newSize = 0;
				if(IsRightToLeft) {
					newSize = Painter.StartSizerPos - Painter.CurrentSizerPos;
				}
				else {
					newSize = Painter.CurrentSizerPos - Painter.StartSizerPos;
				}
				CardWidth += ViewInfo.DeScaleHorizontal(newSize);
			}
			Painter.StopSizing();
		}
		internal void StartCardsSizing(Point p) {
			if(!CanResizeCards) return; 
			Painter.HideSizerLine();
			Painter.CurrentSizerPos = Painter.StartSizerPos = p.X;
			SetState(CardState.Sizing);
			Painter.ShowSizerLine();
			SetCursor(Cursors.SizeWE);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new CardViewAppearances PaintAppearance { get { return base.PaintAppearance as CardViewAppearances; } }
		void ResetAppearance() { Appearance.Reset(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewAppearance"),
#endif
 DXCategory(CategoryName.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance)
		]
		public new CardViewAppearances Appearance { get { return base.Appearance as CardViewAppearances; } }
		[Browsable(false)]
		public bool CanResizeCards { get { return OptionsBehavior.Sizeable && !OptionsBehavior.AutoHorzWidth; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewCardCaptionFormat"),
#endif
 DefaultValue(""), XtraSerializableProperty(), DXCategory(CategoryName.Appearance), Localizable(true)]
		public string CardCaptionFormat {
			get { return cardCaptionFormat; }
			set {
				if(value == null) value = string.Empty;
				if(CardCaptionFormat == value) return;
				cardCaptionFormat = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewCardInterval"),
#endif
 DefaultValue(10), XtraSerializableProperty()]
		public virtual int CardInterval {
			get { return cardInterval; }
			set {
				value = Math.Max(4, Math.Abs(value & 0x7e)); 
				if(CardInterval == value) return;
				cardInterval = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewCardScrollButtonBorderStyle"),
#endif
 DefaultValue(DevExpress.XtraEditors.Controls.BorderStyles.Default), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public DevExpress.XtraEditors.Controls.BorderStyles CardScrollButtonBorderStyle {
			get { return cardScrollButtonBorderStyle; }
			set {
				if(CardScrollButtonBorderStyle == value) return;
				cardScrollButtonBorderStyle = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewCardWidth"),
#endif
 DefaultValue(200), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public int CardWidth {
			get { return cardWidth; }
			set {
				if(value < 10) value = 10;
				if(value == CardWidth) return;
				cardWidth = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public virtual int FocusedCardTopFieldIndex {
			get {
				focusedCardTopFieldIndex = CalcValidFocusedCardTopFieldIndex(focusedCardTopFieldIndex);
				return focusedCardTopFieldIndex;
			}
			set {
				value = CalcValidFocusedCardTopFieldIndex(value);
				if(value == FocusedCardTopFieldIndex) return;
				focusedCardTopFieldIndex = value;
				this.lockTopFieldChanging++;
				try {
				OnPropertiesChanged();
			}
				finally {
					this.lockTopFieldChanging--;
				}
			}
		}
		[Browsable(false)]
		public override bool IsDefaultState {
			get { return State == CardState.Normal; }
		}
		[Browsable(false)]
		public override bool IsEditing {
			get { return State == CardState.Editing;}
		}
		[Browsable(false)]
		public override bool IsSizingState {
			get { return State == CardState.Sizing;}
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("CardViewIsVisible")]
#endif
		public override bool IsVisible {
			get { return ViewRect.X > -10000 && !ViewRect.IsEmpty; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewMaximumCardColumns"),
#endif
 DefaultValue(-1), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public int MaximumCardColumns {
			get { return maximumCardColumns; }
			set {
				if(value < 1) value = -1;
				if(MaximumCardColumns != value) {
					maximumCardColumns = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewMaximumCardRows"),
#endif
 DefaultValue(-1), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public int MaximumCardRows {
			get { return maximumCardRows; }
			set {
				if(value < 1) value = -1;
				if(MaximumCardRows != value) {
					maximumCardRows = value;
					OnPropertiesChanged();
				}
			}
		}
		internal bool ShouldSerializeOptionsBehavior() { return OptionsBehavior.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewOptionsBehavior"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new CardOptionsBehavior OptionsBehavior { get { return base.OptionsBehavior as CardOptionsBehavior; } }
		internal bool ShouldSerializeOptionsPrint() { return OptionsPrint.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewOptionsPrint"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new CardOptionsPrint OptionsPrint {
			get { return (CardOptionsPrint)base.OptionsPrint; }
		}
		internal bool ShouldSerializeOptionsView() { return OptionsView.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewOptionsView"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public new CardOptionsView OptionsView { get { return base.OptionsView as CardOptionsView; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewPrintMaximumCardColumns"),
#endif
 DefaultValue(-1), XtraSerializableProperty(), DXCategory(CategoryName.Printing)]
		public int PrintMaximumCardColumns {
			get { return printMaximumCardColumns; }
			set {
				if(value < 1) value = -1;
				printMaximumCardColumns = value;
			}
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("CardViewRowCount")]
#endif
		public override int RowCount {
			get {
				if(DesignMode) return 2;
				return DataController.VisibleCount + (IsNewRowEditing ? 1 : 0);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CardState State { 
			get { return state; } 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TopLeftCardIndex {
			get { 
				topLeftCardIndex = Math.Min(topLeftCardIndex, Math.Max(0, RowCount - 1)); 
				return topLeftCardIndex;
			}
			set {
				if(TopLeftCardIndex != value) {
					CloseEditor();
					topLeftCardIndex = CheckTopRowIndex(value);
					DoTopLeftCardIndexChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewVertScrollVisibility"),
#endif
 DefaultValue(ScrollVisibility.Never), XtraSerializableProperty(), DXCategory(CategoryName.Behavior)]
		public ScrollVisibility VertScrollVisibility {
			get { return vertScrollVisibility; }
			set {
				if(VertScrollVisibility == value) return;
				vertScrollVisibility = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public override Rectangle ViewRect {
			get { return viewRect; }
		}
		protected override bool IsAutoHeight { get { return OptionsBehavior.FieldAutoHeight; } }
		protected internal override int ScrollPageSize { get { return ViewInfo.Cards.Count; } }
		protected override string ViewName { get { return "CardView"; } }
		protected override int VisualClientPageRowCount { get { return ViewInfo.Cards.Count + 1; } }
		protected override int VisualClientTopRowIndex { get { return TopLeftCardIndex; } }
		protected internal CardQuickCustomizationForm CustomizationForm {
			get { return customizationForm; }
			set {
				if(CustomizationForm == value) return;
				if(customizationForm != null) customizationForm.Dispose();
				customizationForm = value;
			}
		}
		protected internal new CardViewDataController DataController { get { return base.DataController as CardViewDataController; } }
		internal CardSelectionController CardSelection { get { return DataController.Selection as CardSelectionController; } }
		protected virtual internal bool IsNeededHScrollBar { 
			get { 
				if(GridControl != null && GridControl.UseEmbeddedNavigator && GridControl.DefaultView == this) return true;
				return OptionsView.ShowHorzScrollBar; 
			}
		}
		public override int GetVisibleIndex(int rowHandle) {
			if(rowHandle == CurrencyDataController.NewItemRow) {
				if(IsNewRowEditing) return RowCount - 1;
			}
			return base.GetVisibleIndex(rowHandle);
		}
		public override int GetVisibleRowHandle(int rowVisibleIndex) {
			if(DesignMode) return base.GetVisibleRowHandle(rowVisibleIndex);
			if(rowVisibleIndex == DataController.VisibleCount && IsNewRowEditing) return CurrencyDataController.NewItemRow;
			return base.GetVisibleRowHandle(rowVisibleIndex);
		}
		protected internal bool IsNeededVScrollBar { get { return false; } }
		protected internal virtual bool IsNewRowEditing { get { return newRowEditing; } }
		protected internal override bool IsRecreateOnMarginChanged { get { return OptionsPrint.AutoHorzWidth; } }
		protected internal override bool IsSupportPrinting { get { return true; } }
		protected internal override UserControl PrintDesigner { 
			get { 
				UserControl ctrl = new UserControl();
				DevExpress.XtraGrid.Frames.CardViewPrinting cvp = new DevExpress.XtraGrid.Frames.CardViewPrinting();
				cvp.InitFrame(this, "CardView", new Bitmap(16, 16));
				cvp.AutoApply = false;
				cvp.HideCaption();
				cvp.Dock = DockStyle.Fill;
				ctrl.Controls.Add(cvp);
				ctrl.Dock = DockStyle.Fill;
				ctrl.Visible = true;
				ctrl.Size = cvp.UserControlSize;
				return ctrl; 
			} 
		}
		protected internal ScrollInfo ScrollInfo { get { return scrollInfo; } }
		protected internal new CardViewInfo ViewInfo { get { return base.ViewInfo as CardViewInfo; } }
		IBoundControl IDataControllerValidationSupport.BoundControl { get { return GridControl; } }
		int IAccessibleGrid.HeaderCount { get {	return 0; } }
		int IAccessibleGrid.RowCount { get { return RowCount; } }
		int IAccessibleGrid.SelectedRow { get { return RowHandle2AccessibleIndex(FocusedRowHandle);	} }
		ScrollBarBase IAccessibleGrid.HScroll { get { return ScrollInfo.HScrollVisible ? ScrollInfo.HScroll : null; } }
		ScrollBarBase IAccessibleGrid.VScroll { get { return ScrollInfo.VScrollVisible ? ScrollInfo.VScroll : null; } }
		IAccessibleGridRow IAccessibleGrid.GetRow(int index) { 
			int rowHandle = AccessibleIndex2RowHandle(index);
			if(!IsValidRowHandle(rowHandle)) return null;
			return new DevExpress.XtraGrid.Accessibility.CardAccessibleRow(this, rowHandle);
		} 
		int IAccessibleGrid.FindRow(int x, int y) { 
			CardInfo ci = ViewInfo.Cards.GetInfo(x, y);
			if(ci != null) return RowHandle2AccessibleIndex(ci.RowHandle);
			return -1;
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() { return new DevExpress.XtraGrid.Accessibility.CardViewAccessibleObject(this); }
#region Events
		protected virtual void RaiseCardCollapsed(int rowHandle) {
			RowEventHandler handler = (RowEventHandler)this.Events[cardCollapsed];
			if(handler != null) {
				RowEventArgs e = new RowEventArgs(rowHandle);
				handler(this, e);
			}
		}
		protected virtual bool RaiseCardCollapsing(int rowHandle) {
			RowAllowEventHandler handler = (RowAllowEventHandler)this.Events[cardCollapsing];
			if(handler != null) {
				RowAllowEventArgs e = new RowAllowEventArgs(rowHandle, true);
				handler(this, e);
				return e.Allow;
			}
			return true;
		}
		protected virtual void RaiseCardExpanded(int rowHandle) {
			RowEventHandler handler = (RowEventHandler)this.Events[cardExpanded];
			if(handler != null) {
				RowEventArgs e = new RowEventArgs(rowHandle);
				handler(this, e);
			}
		}
		protected virtual bool RaiseCardExpanding(int rowHandle) {
			RowAllowEventHandler handler = (RowAllowEventHandler)this.Events[cardExpanding];
			if(handler != null) {
				RowAllowEventArgs e = new RowAllowEventArgs(rowHandle, true);
				handler(this, e);
				return e.Allow;
			}
			return true;
		}
		protected internal virtual int RaiseCalcFieldHeight(FieldHeightEventArgs e) {
			FieldHeightEventHandler handler = (FieldHeightEventHandler)this.Events[calcFieldHeight];
			if(handler != null) handler(this, e);
			return e.FieldHeight;
		}
		protected internal void RaiseCustomCardCaptionImage(CardCaptionImageEventArgs e) {
			CardCaptionImageEventHandler handler = (CardCaptionImageEventHandler)this.Events[customCardCaptionImage];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomDrawCardCaption(CardCaptionCustomDrawEventArgs e) {
			CardCaptionCustomDrawEventHandler handler = (CardCaptionCustomDrawEventHandler)this.Events[customDrawCardCaption];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomDrawCardFieldCaption(RowCellCustomDrawEventArgs e) {
			RowCellCustomDrawEventHandler handler = (RowCellCustomDrawEventHandler)this.Events[customDrawCardFieldCaption];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomDrawCardFieldValue(RowCellCustomDrawEventArgs e) {
			RowCellCustomDrawEventHandler handler = (RowCellCustomDrawEventHandler)this.Events[customDrawCardFieldValue];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomDrawField(RowCellCustomDrawEventArgs e) {
			RowCellCustomDrawEventHandler handler = (RowCellCustomDrawEventHandler)this.Events[customDrawCardField];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewCardExpanded"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event RowEventHandler CardExpanded {
			add { this.Events.AddHandler(cardExpanded, value); }
			remove { this.Events.RemoveHandler(cardExpanded, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewCardCollapsed"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event RowEventHandler CardCollapsed {
			add { this.Events.AddHandler(cardCollapsed, value); }
			remove { this.Events.RemoveHandler(cardCollapsed, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewCardCollapsing"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event RowAllowEventHandler CardCollapsing {
			add { this.Events.AddHandler(cardCollapsing, value); }
			remove { this.Events.RemoveHandler(cardCollapsing, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewCardExpanding"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event RowAllowEventHandler CardExpanding {
			add { this.Events.AddHandler(cardExpanding, value); }
			remove { this.Events.RemoveHandler(cardExpanding, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewCalcFieldHeight"),
#endif
 DXCategory(CategoryName.Appearance)]
		public event FieldHeightEventHandler CalcFieldHeight {
			add { this.Events.AddHandler(calcFieldHeight, value); }
			remove { this.Events.RemoveHandler(calcFieldHeight, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewTopLeftCardChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event EventHandler TopLeftCardChanged {
			add { this.Events.AddHandler(topLeftCardChanged, value); }
			remove { this.Events.RemoveHandler(topLeftCardChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewCustomCardCaptionImage"),
#endif
 DXCategory(CategoryName.Appearance)]
		public event CardCaptionImageEventHandler CustomCardCaptionImage {
			add { this.Events.AddHandler(customCardCaptionImage, value); }
			remove { this.Events.RemoveHandler(customCardCaptionImage, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewCustomDrawCardCaption"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event CardCaptionCustomDrawEventHandler CustomDrawCardCaption {
			add { this.Events.AddHandler(customDrawCardCaption, value); }
			remove { this.Events.RemoveHandler(customDrawCardCaption, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewCustomDrawCardFieldCaption"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event RowCellCustomDrawEventHandler CustomDrawCardFieldCaption {
			add { this.Events.AddHandler(customDrawCardFieldCaption, value); }
			remove { this.Events.RemoveHandler(customDrawCardFieldCaption, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewCustomDrawCardField"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event RowCellCustomDrawEventHandler CustomDrawCardField {
			add { this.Events.AddHandler(customDrawCardField, value); }
			remove { this.Events.RemoveHandler(customDrawCardField, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardViewCustomDrawCardFieldValue"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event RowCellCustomDrawEventHandler CustomDrawCardFieldValue {
			add { this.Events.AddHandler(customDrawCardFieldValue, value); }
			remove { this.Events.RemoveHandler(customDrawCardFieldValue, value); }
		}
#endregion Events
	}
}
