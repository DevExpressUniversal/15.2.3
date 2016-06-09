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
using System.ComponentModel.Design;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraExport;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Export;
using DevExpress.XtraGrid.Frames;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Layout.Customization;
using DevExpress.XtraGrid.Views.Layout.Designer;
using DevExpress.XtraGrid.Views.Layout.Drawing;
using DevExpress.XtraGrid.Views.Layout.Events;
using DevExpress.XtraGrid.Views.Layout.Handler;
using DevExpress.XtraGrid.Views.Layout.Modes;
using DevExpress.XtraGrid.Views.Layout.Printing;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.XtraGrid.Views.Printing;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Adapters;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.Registrator;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.Utils.Design;
using DevExpress.XtraLayout.Customization.UserCustomization;
using DevExpress.Utils.Gesture;
namespace DevExpress.XtraGrid.Views.Layout {
	public enum LayoutViewState {
		Normal, Sizing, Editing, Scrolling,
		SingleCardModeButtonPressed, RowModeButtonPressed, ColumnModeButtonPressed,
		MultiRowModeButtonPressed, MultiColumnModeButtonPressed, CarouselModeButtonPressed,
		PanButtonPressed, CustomizeButtonPressed,
		FieldHotTracked,
		LayoutItemPressed,
		FieldPopupActionAreaVisible, FieldPopupActionSortPressed, FieldPopupActionFilterPressed,
		CardExpandButtonPressed,
		CloseZoomButtonPressed,
		FilterPanelCloseButtonPressed,
		FilterPanelActiveButtonPressed,
		FilterPanelTextPressed, FilterPanelMRUButtonPressed, FilterPanelCustomizeButtonPressed
	};
	public class Line {
		Point beginPoint;
		Point endPoint;
		bool isRowCore;
		public Line(Point beginPoint, Point endPoint, bool isRow) {
			this.beginPoint = beginPoint;
			this.endPoint = endPoint;
			this.isRowCore = isRow;
		}
		public Point BeginPoint { get { return beginPoint; } set { beginPoint = value; } }
		public Point EndPoint { get { return endPoint; } set { endPoint = value; } }
		public bool IsRowSeparator { get { return isRowCore; } }
	}
	[DesignTimeVisible(false), ToolboxItem(false)]
	[Designer("DevExpress.XtraGrid.Design.LayoutViewDesigner, " + AssemblyInfo.SRAssemblyGridDesign)]
	public class LayoutView : ColumnView, IDataControllerValidationSupport, ILayoutViewInfoOwner, ILayoutControlOwner, ILayoutControl, IXtraSerializable, ITransparentBackgroundManager {
		internal LayoutViewLayoutControlImplementor implementorCore = null;
		LayoutViewState state = LayoutViewState.Normal;
		Rectangle viewRect = Rectangle.Empty;
		ScrollInfo scrollInfo;
		LayoutViewHitInfo editingFieldInfo = null;
		LayoutViewCard templateCard = null;
		Size viewLayoutSize = Size.Empty;
		static readonly Size DefaultCardMinSize = new Size(200, 20);
		string cardCaptionFormatCore = string.Empty;
		string fieldCaptionFormatCore = string.Empty;
		LayoutViewOptionsItemText optionsItemText = null;
		LayoutViewOptionsSingleRecordMode optionsSingleRecordMode = null;
		LayoutViewOptionsMultiRecordMode optionsMultiRecordMode = null;
		LayoutViewOptionsCarouselMode optionsCarouselMode = null;
		LayoutViewOptionsCustomization optionsCustomization = null;
		LayoutViewOptionsHeaderPanel optionsHeaderPanel = null;
		Stack<LayoutViewCard> expandedCardCacheCore = null;
		Stack<LayoutViewCard> collapsedCardCacheCore = null;
		bool detailAutoHeightCore = true;
		internal static readonly object IsCustomizationRestoringInProgressProperty = new object();
		internal static readonly object DesignerAutoScaleFactor = new object();
		protected internal Stack<LayoutViewCard> ExpandedCardCache {
			get {
				if(expandedCardCacheCore == null) expandedCardCacheCore = new Stack<LayoutViewCard>();
				return expandedCardCacheCore;
			}
		}
		protected internal Stack<LayoutViewCard> CollapsedCardCache {
			get {
				if(collapsedCardCacheCore == null) collapsedCardCacheCore = new Stack<LayoutViewCard>();
				return collapsedCardCacheCore;
			}
		}
		private static readonly object customRowCellEdit = new object();
		private static readonly object customCardLayoutCore = new object();
		private static readonly object visibleRecordIndexChanged = new object();
		private static readonly object customCardCaptionImage = new object();
		private static readonly object customFieldCaptionImage = new object();
		private static readonly object customDrawCardCaption = new object();
		private static readonly object customDrawCardFieldValue = new object();
		private static readonly object customDrawCardFieldCaption = new object();
		private static readonly object cardStyle = new object();
		private static readonly object fieldCaptionStyle = new object();
		private static readonly object fieldValueStyle = new object();
		private static readonly object fieldEditingValueStyle = new object();
		private static readonly object separatorStyle = new object();
		private static readonly object cardClick = new object();
		private static readonly object fieldValueClick = new object();
		Size szCardMinSize;
		bool fPanMode = false;
		bool fCustomizationMode = false;
		Cursor saveCursor;
		Cursor viewPanCursor;
		[ThreadStatic]
		static ImageCollection viewHeaderIcons;
		[ThreadStatic]
		static ImageCollection viewPopupIcons;
		protected ILayoutControl ILayoutControl { get { return implementorCore; } }
		protected Image GetUnImplementedIcon() {
			Bitmap unImplementedIcon = new Bitmap(16, 16);
			Graphics.FromImage(unImplementedIcon).FillRectangle(Brushes.Red, 2, 2, 12, 12);
			return unImplementedIcon;
		}
		protected internal ImageCollection ViewHeaderIcons {
			get {
				if(viewHeaderIcons == null) {
					viewHeaderIcons = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Images.LayoutView.icons.png", typeof(LayoutView).Assembly, new Size(16, 16));
					viewHeaderIcons.AddImage(GetUnImplementedIcon());
				}
				return viewHeaderIcons;
			}
		}
		protected internal ImageCollection ViewPopupIcons {
			get {
				if(viewPopupIcons == null) {
					viewPopupIcons = new ImageCollection();
					viewPopupIcons.ImageSize = new Size(13, 13);
					viewPopupIcons.Images.Add(DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraGrid.Images.LayoutView.sort-down.png", typeof(LayoutView).Assembly));
					viewPopupIcons.Images.Add(DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraGrid.Images.LayoutView.sort-up.png", typeof(LayoutView).Assembly));
					viewPopupIcons.Images.Add(DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraGrid.Images.LayoutView.filter.png", typeof(LayoutView).Assembly));
					viewPopupIcons.Images.Add(DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraGrid.Images.LayoutView.sort-default.png", typeof(LayoutView).Assembly));
				}
				return viewPopupIcons;
			}
		}
		protected internal Cursor ViewPanCursor {
			get {
				if(viewPanCursor == null)
					viewPanCursor = DevExpress.Utils.ResourceImageHelper.CreateCursorFromResources("DevExpress.XtraGrid.Images.LayoutView.hand.cur", typeof(LayoutView).Assembly);
				return viewPanCursor;
			}
			set { viewPanCursor = value; }
		}
#if DEBUGTEST
		internal event EventHandler templateCardRaisedChangedInternal;
		internal event EventHandler templateCardRaisedChangingInternal;
		internal event EventHandler templateCardChangedInternal;
		internal event EventHandler templateCardChangingInternal;
#endif
		protected internal int iTemplateCardChangeCounter = 0;
		protected internal virtual void FireTemplateCardChanging() {
			try {
				if(iTemplateCardChangeCounter == 0) OnTemplateCardChanging(TemplateCard);
			}
			finally {
				iTemplateCardChangeCounter++;
#if DEBUGTEST
				if(templateCardRaisedChangingInternal != null) templateCardRaisedChangingInternal(TemplateCard, EventArgs.Empty);
#endif
			}
		}
		protected internal virtual void FireTemplateCardChanged() {
			try {
				if(--iTemplateCardChangeCounter == 0)
					OnTemplateCardChanged(TemplateCard);
			}
			finally {
#if DEBUGTEST
				if(templateCardRaisedChangedInternal != null) templateCardRaisedChangedInternal(TemplateCard, EventArgs.Empty);
#endif
			}
		}
		protected virtual void OnTemplateCardChanging(LayoutViewCard card) {
#if DEBUGTEST
			if(templateCardChangingInternal != null) templateCardChangingInternal(TemplateCard, EventArgs.Empty);
#endif
		}
		protected virtual void OnTemplateCardChanged(LayoutViewCard card) {
			if(IsInitialized && !fIsSerializing && !card.IsDisposingInProgress) {
				ResetCardsCache();
				((ILayoutViewDataController)DataController).CardDifferences.ResetAllDifferences();
				if(!implementorCore.DisposingFlag) {
					RefreshVisibleColumnsList();
					Refresh();
				}
			}
#if DEBUGTEST
			if(templateCardChangedInternal != null) templateCardChangedInternal(TemplateCard, EventArgs.Empty);
#endif
		}
		internal int isCardsCacheResetting = 0;
		protected internal virtual void ResetCardsCache() {
			isCardsCacheResetting++;
			DeepClearCardStack(ExpandedCardCache);
			DeepClearCardStack(CollapsedCardCache);
			if(ViewInfo != null) {
				DeepClearCardList(ViewInfo.VisibleCards);
				if(ViewInfo.layoutManager != null) ViewInfo.layoutManager.ResetCache();
				ViewInfo.NeedArrangeForce = true;
			}
			--isCardsCacheResetting;
		}
		void DeepClearCardStack(Stack<LayoutViewCard> stack) {
			DisposeCards(stack.ToArray());
			stack.Clear();
		}
		void DeepClearCardList(List<LayoutViewCard> list) {
			DisposeCards(list.ToArray());
			list.Clear();
		}
		void DisposeCards(LayoutViewCard[] cardsToDispose) {
			using(new UndoEngineHelper(this)) {
				for(int i = 0; i < cardsToDispose.Length; i++) {
					FlatItemsList flatter = new FlatItemsList();
					List<BaseLayoutItem> items = flatter.GetItemsList(cardsToDispose[i]);
					cardsToDispose[i].Dispose();
					foreach(BaseLayoutItem item in items) implementorCore.ClearReferences(item);
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool PanModeActive {
			get { return OptionsBehavior.AllowPanCards && fPanMode; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool IsCustomizationMode {
			get { return OptionsBehavior.AllowRuntimeCustomization && fCustomizationMode; }
		}
		protected void SetPanCursor() {
			if(GridControl != null) {
				if(fPanMode) {
					saveCursor = GridControl.Cursor;
					GridControl.Cursor = ViewPanCursor;
				}
				else {
					GridControl.Cursor = saveCursor;
				}
			}
		}
		protected internal override void OnActionScroll(ScrollNotifyAction action) {
			if(!CanActionScroll(action)) return;
			ScrollInfo.OnAction(action);
		}
		bool fTemplateCardAssigned = false;
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewTemplateCard"),
#endif
TypeConverter(typeof(ExpandableObjectConverter))]
		public LayoutViewCard TemplateCard {
			get { return templateCard; }
			set {
				if(value == null) return;
				FireTemplateCardChanging();
				ILayoutControl.RootGroup = value;
				fTemplateCardAssigned = true;
				FireTemplateCardChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 0, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdColumns)]
		public new LayoutViewColumnCollection Columns { get { return base.Columns as LayoutViewColumnCollection; } }
		protected override void Dispose(bool disposing) {
			try {
				if(!disposing) return;
				fViewDisposing = true;
				if(implementorCore.DisposingFlagInternal) return;
				if(GridControl != null) GridControl.ProcessGridKey -= new KeyEventHandler(OnProcessGridKey);
				ResetCardsCache();
				implementorCore.DisposingFlagInternal = true;
				if(TemplateCard != null)
					TemplateCard.DenyResetResizer();
				RemoveDesignComponents();
				implementorCore.DisposeUndoManagerCore();
				implementorCore.DisposeCustomizationFormCore();
				implementorCore.DestroyTimerHandler();
				implementorCore.DisposeHiddenItemsCore();
				implementorCore.DisposeHandlers();
				implementorCore.DisposeStyleControllerCore();
				implementorCore.DisposePaintStylesCore();
				implementorCore.DisposeAppearanceCore();
				implementorCore.DisposeLookAndFeelCore();
				implementorCore.DisposeFakeFocusContainerCore();
				implementorCore.DisposeScrollerCore();
				ILayoutControl.Items = null;
				((IDisposable)ILayoutControl.FocusHelper).Dispose();
				((IDisposable)ILayoutControl.ConstraintsManager).Dispose();
				UnSubscribeOptionEvents();
				DestroyOptions();
				if(customizationFormCore != null) {
					customizationFormCore.Dispose();
					customizationFormCore = null;
				}
				if(scrollInfo != null) {
					UnSubscribeScrollEvents();
					ScrollInfo.RemoveControls(GridControl);
					ScrollInfo.Dispose();
					scrollInfo = null;
				}
				DoubleClickChecker.Reset();
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected virtual void RemoveDesignComponents() {
			if(Site != null) {
				ArrayList componentsToRemove = new ArrayList((this as ILayoutControl).Components);
				ComponentCollection viewSiteComponents = Site.Container.Components;
				foreach(IComponent component in componentsToRemove) {
					BaseLayoutItem viewItem = component as BaseLayoutItem;
					if(viewItem == null) break;
					IComponent siteComponent = LayoutViewComponentsUpdateHelper.FindInExistingComponents(this, viewItem, viewSiteComponents);
					if(siteComponent != null) Site.Container.Remove(siteComponent);
				}
			}
			(this as ILayoutControl).Components.Clear();
			if(TemplateCard != null) TemplateCard.Accept(implementorCore.RemoveComponentHelper);
		}
		public override void RefreshData() {
			base.RefreshData();
			ResetCardsCache();
			Refresh();
		}
		protected override void OnDataController_DataSourceChanged(object sender, EventArgs e) {
			base.OnDataController_DataSourceChanged(sender, e);
			if(!IsDesignMode) RefreshData();
			if(!IsInitialized) return;
			fTemplateCardInitialized = fTemplateCardAssigned;
			TryInitializeTemplateCard();
		}
		protected internal void AddColumnToLayout(LayoutViewColumn column) {
			CheckTemplateCard();
			TemplateCard.AddItem(InitializeFieldByColumn(column, -1));
		}
		List<string> GetExistingFieldNames(LayoutViewField currentField) {
			List<string> names = new List<string>();
			if(Site != null) {
				foreach(IComponent component in Site.Container.Components) {
					LayoutViewField field = component as LayoutViewField;
					if(field == currentField) continue;
					string fieldName = (field != null) ? field.Name : null;
					if(!string.IsNullOrEmpty(fieldName)) names.Add(fieldName);
				}
			}
			else {
				foreach(LayoutViewColumn viewColumn in Columns) {
					string fieldName = null;
					if(viewColumn.IsFieldCreated) {
						if(viewColumn.LayoutViewField == currentField) continue;
						fieldName = viewColumn.LayoutViewField.Name;
					}
					if(!string.IsNullOrEmpty(fieldName)) names.Add(fieldName);
				}
			}
			return names;
		}
		protected bool RemoveZombieItem(LayoutViewField layoutItem) {
			if(layoutItem.Owner == null) {
				if(HiddenItems.Contains(layoutItem)) HiddenItems.RemoveAt(HiddenItems.IndexOf(layoutItem));
				return true;
			}
			return false;
		}
		protected internal void RemoveColumnFromLayout(LayoutViewColumn column) {
			if(TemplateCard == null || column == null || !column.IsFieldCreated) return;
			LayoutViewField layoutItem = column.LayoutViewField;
			if(!RemoveZombieItem(layoutItem)) {
				if(HiddenItems.Contains(layoutItem)) layoutItem.RestoreFromCustomization();
				if(layoutItem.Parent != null) layoutItem.Parent.Remove(layoutItem);
			}
			if(this.Columns.Count == 0) {
				TemplateCard.Clear();
				CardMinSize = ViewInfo.ScaleSize(DefaultCardMinSize);
			}
		}
		int lockDifferencesProcessingCounter = 0;
		protected internal bool IsDifferencesProcessingLocked {
			get { return lockDifferencesProcessingCounter > 0; }
		}
		internal int addColumnsRangeInProgress = 0;
		protected internal void BeginAddColumnsRange() {
			addColumnsRangeInProgress++;
			columnRange.Clear();
		}
		protected internal void EndAddColumnsRange() {
			AddColumnsRange();
			addColumnsRangeInProgress--;
		}
		protected internal bool IsAddColumnRange {
			get { return addColumnsRangeInProgress > 0; }
		}
		List<GridColumn> columnRange = new List<GridColumn>();
		public override void PopulateColumns() {
			using(new UndoEngineHelper(this)) {
				BeginAddColumnsRange();
				try {
					base.PopulateColumns();
				}
				finally { EndAddColumnsRange(); }
			}
		}
		protected internal override void OnLookAndFeelChanged() {
			base.OnLookAndFeelChanged();
			if(TemplateCard != null) {
				TemplateCard.ViewInfo.BorderInfo.ButtonsPanel.UpdateStyle();
				TemplateCard.ResetDefaultMinSize();
			}
		}
		protected void OnAddColumn(GridColumn column) {
			LayoutViewColumn viewColumn = column as LayoutViewColumn;
			AddColumnToLayout(viewColumn);
			UpdateTemplateCardLayout(viewColumn);
			HideColumnIfNeed(viewColumn);
			Refresh();
		}
		protected void OnAddColumnDelayed(GridColumn column) {
			columnRange.Add(column);
		}
		protected void AddColumnsRange() {
			if(columnRange == null || columnRange.Count == 0) return;
			Changing();
			List<LayoutViewField> items = new List<LayoutViewField>();
			CheckTemplateCard();
			TemplateCard.BeginInit();
			int count = 0;
			EnsureCardMinSize();
			foreach(LayoutViewColumn viewColumn in columnRange) {
				LayoutViewField field = InitializeFieldByColumn(viewColumn, count++);
				HideColumnIfNeed(viewColumn);
				items.Add(field);
			}
			columnRange.Clear();
			using(LayoutGroupGenerate groupGenerate = new LayoutGroupGenerate()) {
				if(OptionsView.DefaultColumnCount != 1) groupGenerate.LayoutMode = LayoutMode.Flow;
				TemplateCard.Add(groupGenerate);
				groupGenerate.Items.AddRange(items.ToArray());
				if(OptionsView.DefaultColumnCount != 1) SetColumnForGroup(groupGenerate, OptionsView.DefaultColumnCount == 0 ? GetAutoColumnCount(groupGenerate) : OptionsView.DefaultColumnCount);
				groupGenerate.Selected = true;
				TemplateCard.UngroupSelected();
			}
			TemplateCard.EndInit();
			TemplateCard.Update();
			(this as ILayoutControl).TextAlignManager.Reset();
			Refresh();
			Changed();
		}
		void EnsureCardMinSize() {
			ViewInfo.CalcConstants();
			int columnWidth = 20;
			foreach(LayoutViewColumn viewColumn in columnRange)
				columnWidth = Math.Max(columnWidth, viewColumn.GetBestWidth());
			int maxFieldSize = ViewInfo.CardFieldCaptionMaxWidth + OptionsItemText.TextToControlDistance + columnWidth;
			if(maxFieldSize > CardMinSize.Width)
				szCardMinSize.Width = maxFieldSize;
		}
		protected virtual void HideColumnIfNeed(LayoutViewColumn viewColumn) {
			if(!DesignMode && !IsLockUpdate) viewColumn.Visible = false;
		}
		protected virtual bool CanProcessColumnChanged(LayoutViewColumn viewColumn) {
			return !(viewInitializedCounter != 0 || IsDeserializing || viewColumn == null || (viewColumn.IsFieldCreated && viewColumn.LayoutViewField.IsDisposing));
		}
		protected override void OnColumnAdded(GridColumn column) {
			this.lockDifferencesProcessingCounter++;
			this.viewInitializedCounter++;
			base.OnColumnAdded(column);
			this.viewInitializedCounter--;
			this.lockDifferencesProcessingCounter--;
			LayoutViewColumn viewColumn = column as LayoutViewColumn;
			if(!CanProcessColumnChanged(viewColumn)) return;
			if(!IsAddColumnRange)
				OnAddColumn(column);
			else OnAddColumnDelayed(column);
		}
		protected override void OnColumnDeleted(GridColumn column) {
			LayoutViewColumn viewColumn = column as LayoutViewColumn;
			bool fCanRemove = CanProcessColumnChanged(viewColumn);
			if(fCanRemove) RemoveColumnFromLayout(viewColumn);
			lockDifferencesProcessingCounter++;
			base.OnColumnDeleted(column);
			if(fCanRemove) Refresh();
			lockDifferencesProcessingCounter--;
		}
		protected internal override void OnColumnChanged(GridColumn column) {
			base.OnColumnChanged(column);
			LayoutViewColumn viewColumn = column as LayoutViewColumn;
			if(!CanProcessColumnChanged(viewColumn)) return;
			UpdateTemplateCardLayout(viewColumn);
			Refresh();
		}
		protected internal void UpdateTemplateCardLayout(LayoutViewColumn column) {
			if(viewInitializedCounter != 0) return;
			(this as ILayoutControl).TextAlignManager.Reset();
			ViewInfo.CreateEditorViewInfoIfNeed(column.LayoutViewField, column, 0, true);
			if(TemplateCard != null) TemplateCard.Update();
		}
		protected internal override bool CanShowColumnInCustomizationForm(GridColumn col) {
			return base.CanShowColumnInCustomizationForm(col) ||
				(col != null ? col.OptionsColumn.ShowInCustomizationForm : false);
		}
		protected bool CheckRuntimeDataSourceChanged() {
			int newDataSourceHashCode = 0;
			if(DataController != null && DataController.DataSource != null) {
				newDataSourceHashCode = DataController.DataSource.GetHashCode();
			}
			if(dataSouceHashCode != 0) {
				return (newDataSourceHashCode != dataSouceHashCode) && (newDataSourceHashCode != 0);
			}
			else return false;
		}
		LayoutControlImplementor ISupportImplementor.Implementor { get { return implementorCore; } }
		private Control.ControlCollection controlCollection = null;
		private static readonly object layoutUpgrade = new object();
		private static readonly object groupExpandChanging = new object();
		private static readonly object groupExpandChanged = new object();
		private static readonly object uniqueNameRequest = new object();
		private static readonly object layoutUpdate = new object();
		private static readonly object beforeLoadLayout = new object();
		Rectangle ILayoutControlOwner.Bounds {
			get { return ViewInfo.ViewRects.CardsRect; }
		}
		void ILayoutControl.BestFit() {
			throw new NotImplementedException();
		}
		event EventHandler ILayoutControl.ItemSelectionChanged {
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		LongPressControl ILayoutControl.LongPressControl {
			get { throw new NotImplementedException(); }
		}
		UserInteractionHelper ILayoutControl.UserInteractionHelper {
			get { throw new NotImplementedException(); }
		}
		void ILayoutControl.RaiseSizeableChanged() { }
		Control.ControlCollection ILayoutControlOwner.Controls { get { return controlCollection; } }
		event LayoutGroupCancelEventHandler ILayoutControlOwner.GroupExpandChanging {
			add { this.Events.AddHandler(groupExpandChanging, value); }
			remove { this.Events.RemoveHandler(groupExpandChanging, value); }
		}
		event LayoutGroupEventHandler ILayoutControlOwner.GroupExpandChanged {
			add { this.Events.AddHandler(groupExpandChanged, value); }
			remove { this.Events.RemoveHandler(groupExpandChanged, value); }
		}
		event EventHandler ILayoutControlOwner.LayoutUpdate {
			add { this.Events.AddHandler(layoutUpdate, value); }
			remove { this.Events.RemoveHandler(layoutUpdate, value); }
		}
		event UniqueNameRequestHandler ILayoutControlOwner.RequestUniqueName {
			add { this.Events.AddHandler(uniqueNameRequest, value); }
			remove { this.Events.RemoveHandler(uniqueNameRequest, value); }
		}
		event UniqueNameRequestHandler ILayoutControlOwner.UniqueNameRequest {
			add { this.Events.AddHandler(uniqueNameRequest, value); }
			remove { this.Events.RemoveHandler(uniqueNameRequest, value); }
		}
		event EventHandler propertiesChangedCore;
		event EventHandler changedCore;
		event CancelEventHandler changingCore;
		event EventHandler itemSelectionChangedCore;
		event EventHandler itemAddedCore;
		event EventHandler itemRemovedCore;
		event EventHandler showCustomizationCore;
		event EventHandler hideCustomizationCore;
		event EventHandler showCustomizationCorePublic;
		event EventHandler hideCustomizationCorePublic;
		event DevExpress.XtraLayout.PopupMenuShowingEventHandler showContextMenuCore;
		event DevExpress.XtraLayout.PopupMenuShowingEventHandler showLayoutTreeViewContextMenuCore;
		event EventHandler updatedCore;
		event EventHandler ILayoutControlOwner.Changed {
			add { changedCore += value; }
			remove { changedCore -= value; }
		}
		event CancelEventHandler ILayoutControlOwner.Changing {
			add { changingCore += value; }
			remove { changingCore -= value; }
		}
		event EventHandler ILayoutControlOwner.ItemSelectionChanged {
			add { itemSelectionChangedCore += value; }
			remove { itemSelectionChangedCore -= value; }
		}
		event EventHandler ILayoutControlOwner.ItemAdded {
			add { itemAddedCore += value; }
			remove { itemAddedCore -= value; }
		}
		event EventHandler ILayoutControlOwner.ItemRemoved {
			add { itemRemovedCore += value; }
			remove { itemRemovedCore -= value; }
		}
		event EventHandler ILayoutControlOwner.ShowCustomization {
			add { showCustomizationCore += value; }
			remove { showCustomizationCore -= value; }
		}
		event EventHandler ILayoutControlOwner.HideCustomization {
			add { hideCustomizationCore += value; }
			remove { hideCustomizationCore -= value; }
		}
		event DevExpress.XtraLayout.PopupMenuShowingEventHandler ILayoutControlOwner.PopupMenuShowing {
			add { showContextMenuCore += value; }
			remove { showContextMenuCore -= value; }
		}
		event DevExpress.XtraLayout.PopupMenuShowingEventHandler ILayoutControlOwner.LayoutTreeViewPopupMenuShowing {
			add { showLayoutTreeViewContextMenuCore += value; }
			remove { showLayoutTreeViewContextMenuCore -= value; }
		}
		bool ILayoutControlOwner.IsDesignMode {
			get { return DesignMode; }
		}
		void ILayoutControlOwner.RaiseOwnerEvent_ShowCustomization(object sender, EventArgs e) {
			if(showCustomizationCore != null) showCustomizationCore(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_HideCustomization(object sender, EventArgs e) {
			if(hideCustomizationCore != null) hideCustomizationCore(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_PropertiesChanged(object sender, EventArgs e) {
			if(propertiesChangedCore != null) propertiesChangedCore(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_ItemSelectionChanged(object sender, EventArgs e) {
			if(itemSelectionChangedCore != null) itemSelectionChangedCore(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_ItemRemoved(object sender, EventArgs e) {
			if(itemRemovedCore != null) itemRemovedCore(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_ItemAdded(object sender, EventArgs e) {
			if(itemAddedCore != null) itemAddedCore(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_Updated(object sender, EventArgs e) {
			if(updatedCore != null) updatedCore(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_GroupExpandChanging(LayoutGroupCancelEventArgs e) {
			LayoutGroupCancelEventHandler handler = (LayoutGroupCancelEventHandler)this.Events[groupExpandChanging];
			if(handler != null) handler(this, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_GroupExpandChanged(LayoutGroupEventArgs e) {
			LayoutGroupEventHandler handler = (LayoutGroupEventHandler)this.Events[groupExpandChanged];
			if(handler != null) handler(this, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_ShowCustomizationMenu(DevExpress.XtraLayout.PopupMenuShowingEventArgs e) {
			if(showContextMenuCore != null) showContextMenuCore(this, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_ShowLayoutTreeViewContextMenu(DevExpress.XtraLayout.PopupMenuShowingEventArgs e) {
			if(showLayoutTreeViewContextMenuCore != null) showLayoutTreeViewContextMenuCore(this, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_LayoutUpdate(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[layoutUpdate];
			if(handler != null) handler(this, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_BeforeLoadLayout(object sender, LayoutAllowEventArgs e) {
			LayoutAllowEventHandler handler = (LayoutAllowEventHandler)this.Events[beforeLoadLayout];
			if(handler != null) handler(this, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_Changed(object sender, EventArgs e) {
			if(changedCore != null) changedCore(this, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_Changing(object sender, CancelEventArgs e) {
			if(changingCore != null) changingCore(this, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_UniqueNameRequest(object sender, UniqueNameRequestArgs e) {
			UniqueNameRequestHandler handler = (UniqueNameRequestHandler)this.Events[uniqueNameRequest];
			if(handler != null) handler(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_LayoutUpgrade(object sender, LayoutUpgradeEventArgs e) {
			LayoutUpgradeEventHandler handler = (LayoutUpgradeEventHandler)this.Events[layoutUpgrade];
			if(handler != null) handler(sender, e);
		}
		void ILayoutControlOwner.RaiseOwnerEvent_DefaultLayoutLoading(object sender, EventArgs e) {
		}
		void ILayoutControlOwner.RaiseOwnerEvent_DefaultLayoutLoaded(object sender, EventArgs e) {
		}
		RightButtonMenuManager ILayoutControlOwner.CreateRightButtonMenuManager() { return null; }
		LayoutControlCustomizeHandler ILayoutControlOwner.CreateLayoutControlCustomizeHandler() {
			return new LayoutControlCustomizeHandler(this);
		}
		LayoutControlHandler ILayoutControlOwner.CreateLayoutControlRuntimeHandler() {
			return new LayoutControlHandler(this);
		}
		UserCustomizationForm ILayoutControlOwner.CreateCustomizationForm() { return new CustomizationForm(); }
		ISupportLookAndFeel ILayoutControlOwner.GetISupportLookAndFeel() { return GridControl; }
		IStyleController ILayoutControlOwner.GetIStyleController() { return null; }
		bool IsInitializing {
			get { return viewInitializedCounter > 0; }
		}
		void ILayoutControl.BeginInit() {
			ILayoutControl.BeginInit();
		}
		void ILayoutControl.EndInit() {
			ILayoutControl.EndInit();
			(this as ILayoutControl).Size = templateCard.Size;
		}
		protected virtual void CreateILayoutControlImplementor() {
			implementorCore = new LayoutViewLayoutControlImplementor(this);
		}
		internal bool ShouldSerializeCardMinSize() { return szCardMinSize != DefaultCardMinSize; }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCardMinSize"),
#endif
		XtraSerializableProperty(), DXCategory(CategoryName.CardOptions)]
		public Size CardMinSize {
			get { return ScaleSize(szCardMinSize); }
			set {
				value = DeScaleSize(value);
				if(szCardMinSize == value) return;
				szCardMinSize = value;
				ResetCardsCache();
				OnPropertiesChanged();
			}
		}
		internal SizeF GetScaleFactor() {
			if(IsInitializing || IsDeserializing || fIsSerializing)
				return new SizeF(1f, 1f);
			object value = AttachedProperty.GetValue(GridControl, LayoutView.DesignerAutoScaleFactor);
			if(value != null)
				return (SizeF)value;
			return (GridControl != null) ? GridControl.ScaleFactor : new SizeF(1f, 1f);
		}
		internal int ScaleWidth(int width) {
			return DevExpress.Skins.RectangleHelper.ScaleHorizontal(width, GetScaleFactor().Width);
		}
		internal Size ScaleSize(Size size) {
			return DevExpress.Skins.RectangleHelper.ScaleSize(size, GetScaleFactor());
		}
		internal Size DeScaleSize(Size size) {
			return DevExpress.Skins.RectangleHelper.DeScaleSize(size, GetScaleFactor());
		}
		internal bool ShouldSerializeOptionsView() { return OptionsView.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsView"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public new LayoutViewOptionsView OptionsView { get { return base.OptionsView as LayoutViewOptionsView; } }
		internal bool ShouldSerializeOptionsBehavior() { return OptionsBehavior.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsBehavior"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public new LayoutViewOptionsBehavior OptionsBehavior { get { return base.OptionsBehavior as LayoutViewOptionsBehavior; } }
		bool ShouldSerializeOptionsItemText() { return OptionsItemText.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsItemText"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public LayoutViewOptionsItemText OptionsItemText { get { return optionsItemText; } }
		bool ShouldSerializeOptionsMultiRecordMode() { return OptionsMultiRecordMode.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsMultiRecordMode"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public LayoutViewOptionsMultiRecordMode OptionsMultiRecordMode { get { return optionsMultiRecordMode; } }
		bool ShouldSerializeOptionsSingleRecordMode() { return OptionsSingleRecordMode.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsSingleRecordMode"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public LayoutViewOptionsSingleRecordMode OptionsSingleRecordMode { get { return optionsSingleRecordMode; } }
		bool ShouldSerializeOptionsCarouselMode() { return OptionsCarouselMode.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCarouselMode"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public LayoutViewOptionsCarouselMode OptionsCarouselMode { get { return optionsCarouselMode; } }
		bool ShouldSerializeOptionsCustomization() { return OptionsCustomization.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsCustomization"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public LayoutViewOptionsCustomization OptionsCustomization { get { return optionsCustomization; } }
		bool ShouldSerializeOptionsHeaderPanel() { return OptionsHeaderPanel.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsHeaderPanel"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public LayoutViewOptionsHeaderPanel OptionsHeaderPanel { get { return optionsHeaderPanel; } }
		bool ShouldSerializeOptionsPrint() { return OptionsPrint.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewOptionsPrint"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public new LayoutViewOptionsPrint OptionsPrint { get { return (LayoutViewOptionsPrint)base.OptionsPrint; } }
		bool fTemplateCardInitialized = false;
		protected virtual void TryInitializeTemplateCard() {
			if(!fTemplateCardInitialized && DataController != null && DataController.IsReady) {
				InitializeTemplateCardDefault();
			}
		}
		int dataSouceHashCode = 0;
		protected internal virtual void InitializeTemplateCardDefault() {
			Changing();
			try {
				DoBeforeInitialize();
				LayoutViewCard card = new LayoutViewCard();
				TemplateCard = card;
				InitializeCardDefault(card);
				DoAfterInitialize();
				fTemplateCardInitialized = true;
			}
			finally {
				Changed();
			}
		}
		int lockFireChanged = 0;
		void Changing() {
			if(0 == lockFireChanged++) {
				FireTemplateCardChanging();
				if(!IsDesignMode) return;
				try {
					IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
					if(srv != null)
						srv.OnComponentChanging(this, null);
				}
				catch { }
			}
		}
		void Changed() {
			if(--lockFireChanged == 0) {
				if(IsDesignMode) {
					try {
						IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
						if(srv != null)
							srv.OnComponentChanged(this, null, null, null);
					}
					catch { };
				}
				FireTemplateCardChanged();
			}
		}
		protected virtual void InitializeCardDefault(LayoutViewCard card) {
			try {
				card.BeginInit();
				card.Name = (card.Site != null) ? card.Site.Name : CreateTemplateCardName();
				card.HeaderButtonsLocation = GroupElementLocation.AfterText;
				card.ExpandButtonVisible = CalcCardExpandButtonVisibility();
				card.GroupBordersVisible = CalcCardGroupBordersVisibility();
				if(!IsAddColumnRange) {
					List<BaseLayoutItem> cardItems = new List<BaseLayoutItem>();
					int iColumnCounter = 0;
					foreach(GridColumn column in this.Columns) {
						LayoutViewField field = InitializeFieldByColumn(column, iColumnCounter++);
						if(field != null) cardItems.Add(field);
					}
					if(OptionsView.DefaultColumnCount > 1) {
						using(LayoutGroupGenerate groupGenerate = new LayoutGroupGenerate()) {
							groupGenerate.LayoutMode = LayoutMode.Flow;
							card.Add(groupGenerate);
							groupGenerate.Items.AddRange(cardItems.ToArray());
							SetColumnForGroup(groupGenerate, GetAutoColumnCount(groupGenerate));
							groupGenerate.Selected = true;
							card.UngroupSelected();
							card.MaxSize = new Size(card.Width / 2, 0);
						}
					}
					else card.Items.AddRange(cardItems.ToArray());
				}
			}
			finally { card.EndInit(); }
		}
		const int TemplateCardMaxheight = 500;
		int GetAutoColumnCount(LayoutGroupGenerate groupGenerate) {
			int itemsHeight = groupGenerate.Items.ItemsBounds.Height;
			return itemsHeight / 300 + 1;
		}
		protected internal bool CanAllowBorderColorBlending(LayoutViewCard card) {
			return card.FormatInfo.RowAppearance != null || OptionsView.AllowBorderColorBlending;
		}
		protected internal virtual bool CalcCardExpandButtonVisibility() {
			return OptionsBehavior.AllowExpandCollapse && OptionsView.ShowCardExpandButton;
		}
		protected internal virtual bool CalcCardGroupBordersVisibility() {
			return OptionsView.ShowCardCaption;
		}
		protected virtual LayoutViewField InitializeFieldByColumn(GridColumn column, int iFieldIndex) {
			RepositoryItem gridRepositoryItem = GetRowCellRepositoryItem(0, column);
			LayoutViewColumn lvColumn = (LayoutViewColumn)column;
			if(!lvColumn.IsFieldCreated) {
				lvColumn.AssignLayoutViewField(new LayoutViewField());
			}
			LayoutViewField field = lvColumn.LayoutViewField;
			if(fViewInfo == null) fViewInfo = new LayoutViewInfo(this);
			if(gridRepositoryItem != null && gridRepositoryItem != field.RepositoryItem) field.RepositoryItem = gridRepositoryItem;
			ViewInfo.CreateEditorViewInfoIfNeed(field, column, 0, true);
			if(((LayoutViewFieldInfo)field.ViewInfo).RepositoryItemViewInfo is IHeightAdaptable)
				field.StartNewLine = true;
			InitializeFieldDefault(field, column, iFieldIndex);
			return field;
		}
		protected virtual void InitializeFieldDefault(LayoutViewField field, GridColumn column, int iFieldIndex) {
			ViewInfo.CalcConstants();
			using(new SafeBaseLayoutItemChanger(field)) {
				string fieldName = CreateFieldItemName(column);
				field.Name = UniqueItemNameCreator.CheckAndCreateUniqueFieldName(fieldName, fieldName + "_", field, GetExistingFieldNames(field), 1);
				if(iFieldIndex >= 0) field.Location = new Point(0, iFieldIndex * 26);
				field.Size = new Size(Math.Max(CardMinSize.Width, 1), 26);
				field.TextToControlDistance = OptionsItemText.TextToControlDistance;
				field.TextSize = new System.Drawing.Size(ViewInfo.CardFieldCaptionMaxWidth, 20);
				field.EditorPreferredWidth = CalculatePreferredWidth(field, column);
			}
		}
		protected virtual int CalculatePreferredWidth(LayoutViewField field, GridColumn column) {
			int iWidth = 10;
			if(!string.IsNullOrEmpty(column.GetCaption())) {
				iWidth = field.Width - (field.TextSize.Width + field.TextToControlDistance);
			}
			return iWidth;
		}
		protected virtual void DoBeforeInitialize() {
			if(GridControl != null && ViewInfo != null) {
				if(DataSource != null) dataSouceHashCode = DataSource.GetHashCode();
				ViewInfo.CalcViewRects(GridControl.Bounds);
				ViewInfo.CalcConstants();
			}
		}
		protected virtual void DoAfterInitialize() {
			TemplateCard.Accept(implementorCore.RemoveComponentHelper);
			TemplateCard.Accept(implementorCore.AddComponentHelper);
			(this as ILayoutControl).Size = TemplateCard.Size;
		}
		protected internal string CreateFieldItemName(GridColumn column) {
			return "layoutViewField_" + column.Name;
		}
		protected internal string CreateTemplateCardName() {
			return "layoutViewTemplateCard";
		}
		public virtual void Refresh() {
			if(ViewInfo != null) {
				ViewInfo.NeedArrangeForce = true;
				if(!IsLockUpdate) {
					ViewInfo.Calc(ViewInfo.GInfo.Graphics, ViewRect);
					Invalidate();
				}
			}
		}
		public override void Assign(BaseView v, bool copyEvents) {
			if(v == null) return;
			BeginInit();
			BeginUpdate();
			try {
				base.Assign(v, copyEvents);
				LayoutView lv = v as LayoutView;
				if(lv != null) {
					SyncOptions(lv);
					SyncLayoutControlProperties(lv);
					SyncProperties(lv);
					SyncCardProperties(lv);
					SynchronizeColumnsAndItems();
					if(copyEvents) {
						Events.AddHandler(customDrawCardCaption, lv.Events[customDrawCardCaption]);
						Events.AddHandler(customDrawCardFieldCaption, lv.Events[customDrawCardFieldCaption]);
						Events.AddHandler(customDrawCardFieldValue, lv.Events[customDrawCardFieldValue]);
						Events.AddHandler(cardStyle, lv.Events[cardStyle]);
						Events.AddHandler(fieldCaptionStyle, lv.Events[fieldCaptionStyle]);
						Events.AddHandler(fieldValueStyle, lv.Events[fieldValueStyle]);
						Events.AddHandler(fieldEditingValueStyle, lv.Events[fieldEditingValueStyle]);
						Events.AddHandler(customCardCaptionImage, lv.Events[customCardCaptionImage]);
						Events.AddHandler(customFieldCaptionImage, lv.Events[customFieldCaptionImage]);
						Events.AddHandler(customRowCellEdit, lv.Events[customRowCellEdit]);
						Events.AddHandler(customCardLayoutCore, lv.Events[customCardLayoutCore]);
						Events.AddHandler(visibleRecordIndexChanged, lv.Events[visibleRecordIndexChanged]);
						Events.AddHandler(cardCollapsed, lv.Events[cardCollapsed]);
						Events.AddHandler(cardCollapsing, lv.Events[cardCollapsing]);
						Events.AddHandler(cardExpanded, lv.Events[cardExpanded]);
						Events.AddHandler(cardExpanding, lv.Events[cardExpanding]);
						Events.AddHandler(cardExpanding, lv.Events[cardClick]);
						Events.AddHandler(cardExpanding, lv.Events[fieldValueClick]);
					}
				}
			}
			finally {
				EndUpdate();
				EndInit();
			}
		}
		protected internal override bool IsSupportPrinting { get { return true; } }
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
			for(int n = 0; n < Columns.Count; n++) {
				sb.Append(GetRowCellDisplayText(rowHandle, Columns[n]));
				sb.Append(crlf);
			}
			return true;
		}
		string captionFormatCore = null;
		internal string GetDefaultCardCaptionFormat() {
			string format = CardCaptionFormat;
			if(string.IsNullOrEmpty(format)) {
				if(captionFormatCore == null) captionFormatCore = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewCardCaptionFormat);
				format = captionFormatCore;
			}
			return format;
		}
		string fieldFormatCore = null;
		internal string GetDefaultFieldCaptionFormat() {
			string format = FieldCaptionFormat;
			if(string.IsNullOrEmpty(format)) {
				if(fieldFormatCore == null) fieldFormatCore = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewFieldCaptionFormat);
				format = fieldFormatCore;
			}
			return format;
		}
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
			return GetFormatColumns(CardCaptionFormat, 2);
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
			string caption = string.Empty;
			object[] val = new object[Columns.Count + 2];
			int iRecordNumber = RecordCount > 0 ? rowHandle + 1 : 0;
			val[0] = iRecordNumber.ToString();
			val[1] = RecordCount.ToString();
			for(int n = 0; n < Columns.Count; n++) {
				val[n + 2] = GetRowCellDisplayText(rowHandle, Columns[n]);
			}
			try {
				string format = GetDefaultCardCaptionFormat();
				if(!string.IsNullOrEmpty(format)) caption = string.Format(format, val);
			}
			catch { return string.Empty; }
			return caption;
		}
		public virtual string GetFieldCaption(GridColumn column) {
			string fieldCaption = string.Empty;
			string[] val = new string[2] { column.GetCaption(), column.FieldName };
			try {
				if(string.IsNullOrEmpty(val[0]) && string.IsNullOrEmpty(val[1]))
					return fieldCaption;
				string format = GetDefaultFieldCaptionFormat();
				if(!string.IsNullOrEmpty(format)) fieldCaption = string.Format(format, val);
			}
			catch { return column.GetCaption(); }
			return fieldCaption;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCardCaptionFormat"),
#endif
		DefaultValue(""), XtraSerializableProperty(), DXCategory(CategoryName.CardOptions), Localizable(true)]
		public string CardCaptionFormat {
			get { return cardCaptionFormatCore; }
			set {
				if(value == null) value = string.Empty;
				if(CardCaptionFormat == value) return;
				cardCaptionFormatCore = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewFieldCaptionFormat"),
#endif
		DefaultValue(""), XtraSerializableProperty(), DXCategory(CategoryName.CardOptions), Localizable(true)]
		public string FieldCaptionFormat {
			get { return fieldCaptionFormatCore; }
			set {
				if(value == null) value = string.Empty;
				if(FieldCaptionFormat == value) return;
				fieldCaptionFormatCore = value;
				(this as ILayoutControl).TextAlignManager.Reset();
				OnPropertiesChanged();
			}
		}
		int cardsHorizontalInterval = 2;
		int cardsVerticalInterval = 2;
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCardHorzInterval"),
#endif
 DefaultValue(2), XtraSerializableProperty(), DXCategory(CategoryName.CardOptions)]
		public int CardHorzInterval {
			get { return cardsHorizontalInterval; }
			set {
				if(CardHorzInterval != value) {
					cardsHorizontalInterval = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCardVertInterval"),
#endif
 DefaultValue(2), XtraSerializableProperty(), DXCategory(CategoryName.CardOptions)]
		public int CardVertInterval {
			get { return cardsVerticalInterval; }
			set {
				if(CardVertInterval != value) {
					cardsVerticalInterval = value;
					OnPropertiesChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewDetailAutoHeight"),
#endif
 DefaultValue(true), XtraSerializableProperty(), DXCategory(CategoryName.Behavior)]
		public bool DetailAutoHeight {
			get { return detailAutoHeightCore; }
			set {
				if(detailAutoHeightCore == value) return;
				detailAutoHeightCore = value;
				OnPropertiesChanged();
			}
		}
		#region ILayoutViewOwner
		int visibleCardIndex;
		LayoutViewCard ILayoutViewInfoOwner.CloneCardFromTemplate() {
			if(TemplateCard != null) {
				CheckTemplate();
				return (LayoutViewCard)TemplateCard.Clone(null, this);
			}
			return null;
		}
		protected void CheckTemplate() {
			iTemplateCardChangeCounter++;
			bool groupBorderVisible = CalcCardGroupBordersVisibility();
			if(TemplateCard.GroupBordersVisible != groupBorderVisible) {
				TemplateCard.GroupBordersVisible = groupBorderVisible;
			}
			iTemplateCardChangeCounter--;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int RecordCount { get { return RowCount; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int RowCount {
			get {
				if(DesignMode) return 2;
				return base.RowCount + (DataController.IsNewItemRowEditing ? 1 : 0);
			}
		}
		public override int GetVisibleIndex(int rowHandle) {
			if(rowHandle == CurrencyDataController.NewItemRow) {
				if(IsNewRowEditing) return RecordCount - 1;
			}
			return base.GetVisibleIndex(rowHandle);
		}
		protected internal void CheckCardDifferences(LayoutViewCard card) {
			((ILayoutViewDataController)DataController).CardDifferences.CheckCardDifferences(card, TemplateCard);
		}
		protected internal LayoutViewCardDifferences GetCardDifferences(int rowHandle) {
			return ((ILayoutViewDataController)DataController).CardDifferences.GetCardDifferences(rowHandle);
		}
		public override int GetVisibleRowHandle(int rowVisibleIndex) {
			if(DesignMode) return base.GetVisibleRowHandle(rowVisibleIndex);
			if(rowVisibleIndex == DataController.VisibleCount && IsNewRowEditing) return CurrencyDataController.NewItemRow;
			return base.GetVisibleRowHandle(rowVisibleIndex);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleRecordIndex {
			get { return visibleCardIndex; }
			set {
				value = CorrectValue(value);
				int prevValue = VisibleRecordIndex;
				visibleCardIndex = value;
				if(VisibleRecordIndex != prevValue) {
					RaiseVisibleRecordIndexChanged(new LayoutViewVisibleRecordIndexChangedEventArgs(VisibleRecordIndex, prevValue));
				}
				if(IsInitialized && !IsLockUpdate) {
					ViewInfo.Calc(ViewInfo.GInfo.Graphics, ViewRect);
					Invalidate();
				}
			}
		}
		protected int CorrectValue(int value) {
			if(value >= RecordCount) value = RecordCount - 1;
			if(value < 0) value = 0;
			return value;
		}
		protected internal virtual bool IsNewRowEditing {
			get { return internalNewRowEditing; }
		}
		#endregion ILayoutViewOwner
		public LayoutView() {
			InitializeDataController();
			CreateILayoutControlImplementor();
			InitCore();
			CreateToolTipStrings();
		}
		public LayoutView(GridControl ownerGrid)
			: this() {
			SetGridControl(ownerGrid);
			TryInitializeTemplateCard();
		}
		public override void DeleteSelectedRows() {
			ResetCardsCache();
			base.DeleteSelectedRows();
			Refresh();
		}
		protected override void OnMiscOptionChanged(object sender, BaseOptionChangedEventArgs e) { }
		protected void InitCore() {
			szCardMinSize = DefaultCardMinSize;
			controlCollection = new Control.ControlCollection(null);
			ILayoutControl.BeginInit();
			if(GridControl != null) implementorCore.InitializeLookAndFeelCore(GridControl);
			implementorCore.InitializeAppearanceCore();
			implementorCore.InitializePaintStylesCore();
			implementorCore.InitializeCollections();
			implementorCore.InitializeTimerHandler();
			implementorCore.InitializeFakeFocusContainerCore(this);
			ILayoutControl.EndInit();
			ILayoutControl.OptionsView.AllowHotTrack = OptionsView.AllowHotTrackFields;
			ILayoutControl.OptionsCustomizationForm.ShowLoadButton = false;
			ILayoutControl.OptionsCustomizationForm.ShowSaveButton = false;
			ILayoutControl.OptionsView.EnableIndentsInGroupsWithoutBorders = true;
			ILayoutControl.OptionsItemText.TextAlignMode = TextAlignMode.AlignInGroups;
			this.DataController.ValidationClient = this;
			CreateOptions();
			SubscribeOptionEvents();
			this.scrollInfo = CreateScrollInfo();
			SubscribeScrollEvents();
		}
		protected virtual void SubscribeScrollEvents() {
			this.scrollInfo.VScroll_ValueChanged += new EventHandler(scrollInfo_VScroll_ValueChanged);
			this.scrollInfo.HScroll_ValueChanged += new EventHandler(scrollInfo_HScroll_ValueChanged);
		}
		protected virtual void UnSubscribeScrollEvents() {
			this.scrollInfo.VScroll_ValueChanged -= new EventHandler(scrollInfo_VScroll_ValueChanged);
			this.scrollInfo.HScroll_ValueChanged -= new EventHandler(scrollInfo_HScroll_ValueChanged);
		}
		protected virtual void CreateOptions() {
			this.optionsItemText = CreateOptionsItemText();
			this.optionsSingleRecordMode = CreateOptionSingleRecordMode();
			this.optionsMultiRecordMode = CreateOptionMultiRecordMode();
			this.optionsCarouselMode = CreateOptionsCarouselMode();
			this.optionsCustomization = CreateOptionsCustomization();
			this.optionsHeaderPanel = CreateOptionsHeaderPanel();
		}
		protected virtual void SubscribeOptionEvents() {
			this.optionsItemText.Changed += new BaseOptionChangedEventHandler(OnOptionChanged);
			this.optionsSingleRecordMode.Changed += new BaseOptionChangedEventHandler(OnOptionChanged);
			this.optionsMultiRecordMode.Changed += new BaseOptionChangedEventHandler(OnOptionChanged);
			this.optionsCarouselMode.Changed += new BaseOptionChangedEventHandler(OnOptionChanged);
			this.optionsCustomization.Changed += new BaseOptionChangedEventHandler(OnOptionChanged);
			this.optionsHeaderPanel.Changed += new BaseOptionChangedEventHandler(OnOptionChanged);
		}
		protected virtual void DestroyOptions() {
		}
		protected virtual void UnSubscribeOptionEvents() {
			this.optionsItemText.Changed -= new BaseOptionChangedEventHandler(OnOptionChanged);
			this.optionsSingleRecordMode.Changed -= new BaseOptionChangedEventHandler(OnOptionChanged);
			this.optionsMultiRecordMode.Changed -= new BaseOptionChangedEventHandler(OnOptionChanged);
			this.optionsCarouselMode.Changed -= new BaseOptionChangedEventHandler(OnOptionChanged);
			this.optionsCustomization.Changed -= new BaseOptionChangedEventHandler(OnOptionChanged);
			this.optionsHeaderPanel.Changed -= new BaseOptionChangedEventHandler(OnOptionChanged);
		}
		protected internal void ResetPanModeIfNeed() {
			ResetPanModeIfNeed(true);
		}
		protected internal void ResetPanModeIfNeed(bool refresh) {
			if(PanModeActive) {
				SetPanMode(false);
				if(refresh) Refresh();
			}
		}
		protected internal void SetPanMode(bool value) {
			if(!OptionsBehavior.AllowPanCards) return;
			fPanMode = value;
			SetPanCursor();
			InvalidateRect(ViewInfo.ViewRects.PanButton);
		}
		protected internal bool CanActivatePanMode {
			get { return ViewInfo.CanCardAreaPan && State == LayoutViewState.Normal; }
		}
		protected internal bool CanActivateCustomization {
			get { return OptionsBehavior.AllowRuntimeCustomization && ViewInfo.CanCustomize && State == LayoutViewState.Normal; }
		}
		public virtual void PanModeSwitch() {
			if(CanActivatePanMode) SetPanMode(!PanModeActive);
		}
		LayoutStyleManager ILayoutControl.LayoutStyleManager { get { return null; } }
		LayoutViewCustomizationForm customizationFormCore = null;
		protected internal LayoutViewCustomizationForm CustomizationForm {
			get {
				if(customizationFormCore == null) customizationFormCore = CreateCustomizationForm();
				return customizationFormCore;
			}
		}
		protected virtual LayoutViewCustomizationForm CreateCustomizationForm() {
			DevExpress.Skins.SkinManager.EnableFormSkins();
			LayoutViewCustomizationForm form = CreateCustomizationFormCore();
			form.FormClosing += OnCustomizationFormClosing;
			return form;
		}
		protected virtual LayoutViewCustomizationForm CreateCustomizationFormCore() {
			return new LayoutViewCustomizationForm(this);
		}
		void OnCustomizationFormClosing(object sender, FormClosingEventArgs e) {
			RaiseEvent_HideCustomization(this, null);
			customizationFormCore.FormClosing -= OnCustomizationFormClosing;
			customizationFormCore = null;
			fCustomizationMode = false;
		}
		protected internal void ShowCustomizationForm(bool dialogMode) {
			if(!CanActivateCustomization) return;
			fCustomizationMode = true;
			try {
				InvalidateRect(ViewInfo.ViewRects.CustomizeButton);
				ShowCustomizationFormCore(dialogMode);
			}
			finally { if(dialogMode) fCustomizationMode = false; }
		}
		public virtual void ShowCustomizationForm() {
#if DEBUGTEST
			ShowCustomizationForm(false);
#else
			ShowCustomizationForm(true);
#endif
		}
		public virtual void HideCustomizationForm() {
			CloseCustomizationFormCore();
		}
		public event EventHandler ShowCustomization {
			add { showCustomizationCorePublic += value; }
			remove { showCustomizationCorePublic -= value; }
		}
		public event EventHandler HideCustomization {
			add { hideCustomizationCorePublic += value; }
			remove { hideCustomizationCorePublic -= value; }
		}
		protected void RaiseEvent_ShowCustomization(object sender, EventArgs e) {
			if(showCustomizationCorePublic != null) showCustomizationCorePublic(sender, e);
		}
		protected void RaiseEvent_HideCustomization(object sender, EventArgs e) {
			if(hideCustomizationCorePublic != null) hideCustomizationCorePublic(sender, e);
		}
		protected virtual void ShowCustomizationFormCore(bool dialog) {
			RaiseEvent_ShowCustomization(this, null);
			if(dialog) CustomizationForm.ShowDialog();
			else CustomizationForm.Show();
		}
		protected internal virtual void CloseCustomizationFormCore() {
			if(IsCustomizationMode && CustomizationForm.Visible) {
				CustomizationForm.Close();
				customizationFormCore = null;
				fCustomizationMode = false;
			}
		}
		protected internal int CheckRecordIndex(int currentRecordIndex) {
			if(currentRecordIndex < 0) currentRecordIndex = 0;
			if(currentRecordIndex >= RecordCount) currentRecordIndex = Math.Max(0, RecordCount - 1);
			if(!fInternalVisibleRecordIndexChange) {
				int prevVisibleIndex = VisibleRecordIndex;
				visibleCardIndex = currentRecordIndex;
				if(prevVisibleIndex != VisibleRecordIndex) {
					RaiseVisibleRecordIndexChanged(new LayoutViewVisibleRecordIndexChangedEventArgs(VisibleRecordIndex, prevVisibleIndex));
				}
			}
			return currentRecordIndex;
		}
		bool fInternalVisibleRecordIndexChange = false;
		protected internal void DoNavigateRecords(int iNewPos) {
			DoNavigateRecords(iNewPos, true);
			if(IsMultiSelect) SelectFocusedRowCore();
		}
		protected internal void DoNavigateRecords(int iNewPos, KeyEventArgs e) {
			int prevFocusedHandle = FocusedRowHandle;
			try {
				DoNavigateRecords(iNewPos, true);
			}
			finally { DoAfterMoveFocusedRow(e, prevFocusedHandle, null, null); }
		}
		protected internal void DoNavigateRecords(int iNewPos, bool fChangeFocusedRow) {
			fInternalVisibleRecordIndexChange = true;
			fInternalFocusedRowChange = true;
			try {
				bool hasCardsInViewInfo = ViewInfo.VisibleCards.Count > 0;
				if(iNewPos == GridControl.NewItemRowHandle) {
					internalArrangeFromLeftToRight = false;
					if(hasCardsInViewInfo) {
						if(ViewInfo.GetLastCardRowHandle() == iNewPos)
							return;
					}
					VisibleRecordIndex = GetVisibleIndex(iNewPos);
					return;
				}
				iNewPos = Math.Max(0, Math.Min(RecordCount - 1, iNewPos));
				bool fRefreshPending = fChangeFocusedRow && (FocusedRowHandle != iNewPos) || !hasCardsInViewInfo;
				int focusedRowChange = 0;
				if(fChangeFocusedRow) {
					if(FocusedRowHandle >= 0)
						focusedRowChange = iNewPos - FocusedRowHandle;
					FocusedRowHandle = iNewPos;
				}
				int iLastCard = hasCardsInViewInfo ? ViewInfo.GetLastCardRowHandle() : Math.Max(0, RecordCount - 1);
				int iFirstCard = hasCardsInViewInfo ? ViewInfo.GetFirstCardRowHandle() : 0;
				if(iNewPos >= iFirstCard && iNewPos <= iLastCard && !ViewInfo.IsPartiallyVisible(iNewPos)) {
					if(fRefreshPending && HasFocusedRowDependentEvents()) {
						if(!ScrollInfo.View.IsScrollingState) {
							internalArrangeFromLeftToRight = true;
							if(ViewInfo.NavigationVScrollNeed && ScrollInfo.VScrollVisible)
								visibleCardIndex = ScrollInfo.VScroll.Value;
							if(ViewInfo.NavigationHScrollNeed && ScrollInfo.HScrollVisible)
								visibleCardIndex = ScrollInfo.HScroll.Value;
						}
						Refresh();
					}
					return;
				}
				int iVisibleCard = VisibleRecordIndex;
				int positionChange = (iNewPos - iVisibleCard);
				int pageSize = ViewInfo.CalcScrollableCardsCount();
				if(Math.Abs(focusedRowChange) > 1 && Math.Abs(positionChange) > 1 && pageSize > 1) {
					int pageChange = (Math.Abs(positionChange) > pageSize) ?
						((Math.Abs(positionChange) / pageSize) * pageSize) : pageSize;
					if(iVisibleCard == iLastCard && positionChange > 0)
						pageChange += (pageSize - 1);
					if(iVisibleCard == iFirstCard && positionChange < 0)
						pageChange += (pageSize - 1);
					if(iNewPos < pageSize) {
						iNewPos = iFirstCard - pageChange;
						internalArrangeFromLeftToRight = true;
					}
					if(iNewPos > RecordCount - pageSize) {
						iNewPos = iLastCard + pageChange;
						internalArrangeFromLeftToRight = false;
					}
					if(iNewPos >= pageSize && iNewPos <= RecordCount - pageSize) {
						int pos = (positionChange > 0) ? iFirstCard + pageChange : iLastCard - pageChange;
						if(pos == iLastCard || pos == iFirstCard && ViewInfo.IsPartiallyVisible(pos))
							iNewPos = (positionChange > 0) ? iFirstCard : iLastCard;
						else
							iNewPos = (positionChange > 0) ? iFirstCard + pageChange : iLastCard - pageChange;
						internalArrangeFromLeftToRight = positionChange > 0;
					}
				}
				else {
					if(iLastCard != RecordCount - 1) {
						internalArrangeFromLeftToRight = iVisibleCard >= iNewPos;
					}
					if(!internalArrangeFromLeftToRight && iNewPos < iLastCard) iNewPos = iLastCard + 1;
					if(internalArrangeFromLeftToRight && iNewPos > iFirstCard && iNewPos != iLastCard) iNewPos = iFirstCard - 1;
				}
				BeginLockFocusedRowChange(); 
				VisibleRecordIndex = iNewPos;
				EndLockFocusedRowChange();
			}
			finally {
				ResetArrangeDirection();
				fInternalFocusedRowChange = false;
				fInternalVisibleRecordIndexChange = false;
			}
		}
		bool HasFocusedRowDependentEvents() {
			return
				(iCustomFieldCaptionEventHandlerCounter > 0) ||
				(iCustomRowCellEditEventHandlerCounter > 0) ||
				(iCustomCardLayoutEventHandlerCounter > 0);
		}
		internal int internalHScrollValueChange = 0;
		internal int internalVScrollValueChange = 0;
		internal int prevHScrollIndex = 0;
		internal int prevVScrollIndex = 0;
		internal bool internalArrangeFromLeftToRight = true;
		internal void ResetArrangeDirection() {
			internalArrangeFromLeftToRight = true;
		}
		void scrollInfo_VScroll_ValueChanged(object sender, EventArgs e) {
			GridControl.EditorHelper.BeginAllowHideException();
			internalVScrollValueChange++;
			LockAutoPanByFocusedCard();
			try {
				if(internalVScrollValueChange == 1) {
					ResetPanModeIfNeed(false);
					DoScrollVisibleIndex(scrollInfo.VScrollPosition, prevVScrollIndex);
					prevVScrollIndex = scrollInfo.VScrollPosition;
				}
			}
			catch(HideException) { UpdateVScrollRange(); }
			finally {
				UnLockAutoPanByFocusedCard();
				internalVScrollValueChange--;
				GridControl.EditorHelper.EndAllowHideException();
			}
		}
		void scrollInfo_HScroll_ValueChanged(object sender, EventArgs e) {
			GridControl.EditorHelper.BeginAllowHideException();
			internalHScrollValueChange++;
			LockAutoPanByFocusedCard();
			try {
				if(internalHScrollValueChange == 1) {
					ResetPanModeIfNeed(false);
					DoScrollVisibleIndex(scrollInfo.HScrollPosition, prevHScrollIndex);
					prevHScrollIndex = scrollInfo.HScrollPosition;
				}
			}
			catch(HideException) { UpdateHScrollRange(); }
			finally {
				UnLockAutoPanByFocusedCard();
				internalHScrollValueChange--;
				GridControl.EditorHelper.EndAllowHideException();
			}
		}
		protected void DoScrollVisibleIndex(int iNewScrollPos, int iPrevScrollPos) {
			bool forward = iNewScrollPos > iPrevScrollPos;
			VisibleRecordIndex = (forward && internalArrangeFromLeftToRight) ?
					CheckNewScrollPos(iNewScrollPos, forward) :
				(!forward && !internalArrangeFromLeftToRight) ?
					CheckNewScrollPos(iNewScrollPos, forward) : iNewScrollPos;
			fInternalFocusedRowChange = true;
			if(OptionsBehavior.AutoFocusCardOnScrolling) FocusedRowHandle = VisibleRecordIndex;
			fInternalFocusedRowChange = false;
		}
		protected int CheckNewScrollPos(int iNewScrollPos, bool forward) {
			return ViewInfo.CheckNewScrollPos(iNewScrollPos, forward);
		}
		protected override ColumnViewOptionsView CreateOptionsView() { return new LayoutViewOptionsView(this); }
		protected override ColumnViewOptionsBehavior CreateOptionsBehavior() {
			return new LayoutViewOptionsBehavior(this);
		}
		protected override bool CanCalculateLayout {
			get { return TemplateCard != null && base.CanCalculateLayout; }
		}
		protected internal override void OnGridLoadComplete() {
			base.OnGridLoadComplete();
			if(ViewInfo != null)
				ViewInfo.OnGridLoadComplete();
		}
		public override void LayoutChanged() {
			if(!CalculateLayout()) return;
			base.LayoutChanged();
		}
		protected internal override void SetGridControl(GridControl newControl) {
			base.SetGridControl(newControl);
			UpdateGridControl();
		}
		protected override void OnGridControlChanged(GridControl prevControl) {
			base.OnGridControlChanged(prevControl);
			if(ScrollInfo != null) {
				ScrollInfo.RemoveControls(prevControl);
				ScrollInfo.AddControls(GridControl);
			}
			if(prevControl != null) prevControl.ProcessGridKey -= new KeyEventHandler(OnProcessGridKey);
			if(GridControl != null) GridControl.ProcessGridKey += new KeyEventHandler(OnProcessGridKey);
		}
		protected bool IsTabPressed(Keys keys) {
			return (keys & Keys.KeyCode) == Keys.Tab;
		}
		protected void OnProcessGridKey(object sender, KeyEventArgs e) {
			if(IsTabPressed(e.KeyCode) && e.Control) {
				ChangeParentTabByCtrlTabKey(e.Shift);
			}
		}
		protected void UpdateGridControl() {
			if(GridControl != null && viewInitializedCounter == 0) {
				ILayoutControl.BeginInit();
				implementorCore.InitializeLookAndFeelCore(GridControl);
				implementorCore.InitializeScrollerCore(this);
				implementorCore.InitializePaintStylesCore();
				ILayoutControl.EndInit();
			}
		}
		int viewInitializedCounter = 0;
		public override void BeginInit() {
			base.BeginInit();
			viewInitializedCounter++;
		}
		public override void EndInit() {
			viewInitializedCounter--;
			base.EndInit();
		}
		protected override void OnEndInit() {
			base.OnEndInit();
			RefreshVisibleColumnsList();
			UpdateGridControl();
		}
		protected internal override void EndDataUpdateCore(bool sortOnly) {
			if(ViewInfo != null) ViewInfo.NeedArrangeForce = true;
			base.EndDataUpdateCore(sortOnly);
		}
		protected override void RefreshVisibleColumnsIndexes() {
		}
		protected internal override void RefreshVisibleColumnsList() {
			if(IsLoading) return;
			VisibleColumnsCore.ClearCore();
			List<LayoutViewColumn> columns = new List<LayoutViewColumn>();
			foreach(LayoutViewColumn column in Columns) {
				if(column == null) continue;
				if(column.Visible && CanShowColumn(column))
					columns.Add(column);
			}
			VisibleColumnsCore.AddRangeCore(columns);
		}
		protected override BaseViewInfo CreateNullViewInfo() { return new NullLayoutViewInfo(this); }
		protected internal override ViewDrawArgs CreateDrawArgs(DXPaintEventArgs e, GraphicsCache cache) {
			if(cache == null) cache = new GraphicsCache(e, Painter.Paint);
			return new LayoutViewDrawArgs(cache, ViewInfo, ViewInfo.Bounds);
		}
		protected override GridColumnCollection CreateColumnCollection() {
			return new LayoutViewColumnCollection(this);
		}
		public new LayoutViewHitInfo CalcHitInfo(Point pt) { return base.CalcHitInfo(pt) as LayoutViewHitInfo; }
		public new LayoutViewHitInfo CalcHitInfo(int x, int y) { return CalcHitInfo(new Point(x, y)); }
		protected override BaseHitInfo CalcHitInfoCore(Point pt) { return ViewInfo.CalcHitInfo(pt); }
		protected internal override int CalcRealViewHeight(Rectangle viewRect) {
			if(RequireSynchronization != SynchronizationMode.None) {
				CheckSynchronize();
				calculatedRealViewHeight = -1;
			}
			if(calculatedRealViewHeight != -1 && viewRect.Size == ViewRect.Size)
				return calculatedRealViewHeight;
			int result = viewRect.Height;
			LayoutViewInfo tempViewInfo = new LayoutViewInfo(this), oldViewInfo = ViewInfo, copyFromInfo = ViewInfo;
			this.fViewInfo = tempViewInfo;
			RefreshVisibleColumnsList();
			for(int i = 0; i < (oldViewInfo.IsReady ? 1 : 2); i++) {
				try {
					ViewInfo.PrepareCalcRealViewHeight(viewRect, copyFromInfo);
					result = ViewInfo.CalcRealViewHeight(viewRect);
				}
				catch { }
				copyFromInfo = ViewInfo;
			}
			this.fViewInfo = oldViewInfo;
			tempViewInfo.ClearVisibleCards();
			calculatedRealViewHeight = result;
			return result;
		}
		#region Printing
		protected override BaseViewPrintInfo CreatePrintInfoInstance(PrintInfoArgs args) {
			return new LayoutViewPrintInfo(args);
		}
		protected new LayoutViewPrintInfo PrintInfo { get { return base.PrintInfo as LayoutViewPrintInfo; } }
		#endregion
		protected internal override RepositoryItem GetRowCellRepositoryItem(int rowHandle, GridColumn column) {
			if(column == null) return null;
			RepositoryItem cellEdit = column.ColumnEdit;
			if(cellEdit != null && cellEdit.IsDisposed) cellEdit = null;
			if(cellEdit == null) cellEdit = GetColumnDefaultRepositoryItem(column);
			LayoutViewCustomRowCellEditEventArgs e = new LayoutViewCustomRowCellEditEventArgs(rowHandle, column, cellEdit);
			RaiseCustomRowCellEdit(e);
			if(e.RepositoryItem != null) cellEdit = e.RepositoryItem;
			return cellEdit;
		}
		protected internal void RaiseCustomDrawCardCaption(LayoutViewCustomDrawCardCaptionEventArgs e) {
			LayoutViewCustomDrawCardCaptionEventHandler handler = (LayoutViewCustomDrawCardCaptionEventHandler)this.Events[customDrawCardCaption];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomDrawFieldCaption(RowCellCustomDrawEventArgs e) {
			RowCellCustomDrawEventHandler handler = (RowCellCustomDrawEventHandler)this.Events[customDrawCardFieldCaption];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomDrawFieldValue(RowCellCustomDrawEventArgs e) {
			RowCellCustomDrawEventHandler handler = (RowCellCustomDrawEventHandler)this.Events[customDrawCardFieldValue];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomCardStyle(LayoutViewCardStyleEventArgs e) {
			LayoutViewCardStyleEventHandler handler = (LayoutViewCardStyleEventHandler)this.Events[cardStyle];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomFieldCaptionStyle(LayoutViewFieldCaptionStyleEventArgs e) {
			LayoutViewFieldCaptionStyleEventHandler handler = (LayoutViewFieldCaptionStyleEventHandler)this.Events[fieldCaptionStyle];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomFieldValueStyle(LayoutViewFieldValueStyleEventArgs e) {
			LayoutViewFieldValueStyleEventHandler handler = (LayoutViewFieldValueStyleEventHandler)this.Events[fieldValueStyle];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomFieldEditingValueStyle(LayoutViewFieldEditingValueStyleEventArgs e) {
			LayoutViewFieldEditingValueStyleEventHandler handler = (LayoutViewFieldEditingValueStyleEventHandler)this.Events[fieldEditingValueStyle];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomSeparatorStyle(LayoutViewCustomSeparatorStyleEventArgs e) {
			LayoutViewCustomSeparatorStyleEventHandler handler = (LayoutViewCustomSeparatorStyleEventHandler)this.Events[separatorStyle];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomFieldCaptionImage(LayoutViewFieldCaptionImageEventArgs e) {
			LayoutViewFieldCaptionImageEventHandler handler = (LayoutViewFieldCaptionImageEventHandler)this.Events[customFieldCaptionImage];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomCardCaptionImage(LayoutViewCardCaptionImageEventArgs e) {
			LayoutViewCardCaptionImageEventHandler handler = (LayoutViewCardCaptionImageEventHandler)this.Events[customCardCaptionImage];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomRowCellEdit(LayoutViewCustomRowCellEditEventArgs e) {
			LayoutViewCustomRowCellEditEventHandler handler = (LayoutViewCustomRowCellEditEventHandler)this.Events[customRowCellEdit];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseCustomCardLayout(LayoutViewCustomCardLayoutEventArgs e) {
			LayoutViewCustomCardLayoutEventHandler handler = (LayoutViewCustomCardLayoutEventHandler)this.Events[customCardLayoutCore];
			if(handler != null) handler(this, e);
		}
		protected internal void RaiseVisibleRecordIndexChanged(LayoutViewVisibleRecordIndexChangedEventArgs e) {
			LayoutViewVisibleRecordIndexChangedEventHandler handler = (LayoutViewVisibleRecordIndexChangedEventHandler)this.Events[visibleRecordIndexChanged];
			if(handler != null) handler(this, e);
		}
		protected internal string GetCarouselModeCardsAreaToolTip(Point p) {
			string toolTipText = string.Empty;
			LayoutViewCarouselMode mode = ViewInfo.layoutManager as LayoutViewCarouselMode;
			if(mode != null) {
				int hotCardIndex = mode.GetHotCardIndex(p);
				if(hotCardIndex != -1 && hotCardIndex != VisibleRecordIndex) {
					toolTipText = GetCardCaption(hotCardIndex);
				}
			}
			return toolTipText;
		}
		protected internal ToolTipControlInfo GetFieldToolTip(LayoutViewHitInfo hi) {
			ToolTipControlInfo toolTip = null;
			LayoutViewFieldInfo fieldInfo = hi.HitField.ViewInfo as LayoutViewFieldInfo;
			if(fieldInfo != null) {
				if(hi.InFieldCaption && !IsDesignMode) {
					toolTip = implementorCore.GetLayoutControlItemToolTipInfo(hi.HitPoint, hi.HitField);
				}
				if(toolTip == null) {
					BaseEditViewInfo editorInfo = fieldInfo.RepositoryItemViewInfo;
					toolTip = GetCellEditToolTipInfo(editorInfo, hi.HitPoint, hi.HitCard.RowHandle, hi.Column);
				}
			}
			return toolTip;
		}
		protected internal override ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			ToolTipControlInfo result = null;
			LayoutViewHitInfo hi = ViewInfo.CalcHitInfo(pt);
			string toolTipText = string.Empty;
			if(ViewInfo.ViewMode == LayoutViewMode.Carousel) toolTipText = GetCarouselModeCardsAreaToolTip(pt);
			if(hi.InHeaderArea && !fInternalLockActions) toolTipText = GetHeaderToolTipText(hi);
			if(hi.InCard) {
				if(hi.InField) result = GetFieldToolTip(hi);
				else if(hi.InCardCaption) toolTipText = GetCaptionToolTipText(hi);
				else {
					result = implementorCore.GetItemToolTipInfo(pt, hi.HitCard.GetLayoutItemHitInfo(pt));
				}
			}
			if(toolTipText != string.Empty) result = new ToolTipControlInfo(toolTipText, toolTipText);
			return result;
		}
		string singleModeToolTip = string.Empty;
		string rowModeToolTip = string.Empty;
		string columnModeToolTip = string.Empty;
		string multiRowModeToolTip = string.Empty;
		string multiColumnModeToolTip = string.Empty;
		string carouselModeToolTip = string.Empty;
		string panModeToolTip = string.Empty;
		string customizeBtnToolTip = string.Empty;
		string clozeZoomToolTipClose = string.Empty;
		string clozeZoomToolTipZoom = string.Empty;
		protected virtual void CreateToolTipStrings() {
			this.singleModeToolTip = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewSingleModeBtnHint);
			this.rowModeToolTip = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewRowModeBtnHint);
			this.columnModeToolTip = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewColumnModeBtnHint);
			this.multiRowModeToolTip = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewMultiRowModeBtnHint);
			this.multiColumnModeToolTip = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewMultiColumnModeBtnHint);
			this.carouselModeToolTip = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewCarouselModeBtnHint);
			this.panModeToolTip = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewPanBtnHint);
			this.customizeBtnToolTip = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewCustomizeBtnHint);
			this.clozeZoomToolTipClose = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewCloseZoomBtnHintClose);
			this.clozeZoomToolTipZoom = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewCloseZoomBtnHintZoom);
		}
		protected string GetCaptionToolTipText(LayoutViewHitInfo hi) {
			return GetCardCaption(hi.RowHandle);
		}
		protected string GetHeaderToolTipText(LayoutViewHitInfo hi) {
			string text = string.Empty;
			switch(hi.HitTest) {
				case LayoutViewHitTest.SingleModeButton: return singleModeToolTip;
				case LayoutViewHitTest.RowModeButton: return rowModeToolTip;
				case LayoutViewHitTest.ColumnModeButton: return columnModeToolTip;
				case LayoutViewHitTest.MultiRowModeButton: return multiRowModeToolTip;
				case LayoutViewHitTest.MultiColumnModeButton: return multiColumnModeToolTip;
				case LayoutViewHitTest.CarouselModeButton: return carouselModeToolTip;
				case LayoutViewHitTest.PanButton: return panModeToolTip;
				case LayoutViewHitTest.CustomizeButton: return customizeBtnToolTip;
				case LayoutViewHitTest.CloseZoomButton:
					if(IsZoomedView) return clozeZoomToolTipClose;
					return clozeZoomToolTipZoom;
			}
			return text;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Rectangle ViewRect { get { return viewRect; } }
		protected override void SetViewRect(Rectangle newValue) {
			if(newValue == ViewRect || fUpdateSize > 0) return;
			fUpdateSize++;
			try {
				bool fHorzChange = viewRect.Width != newValue.Width;
				bool fVertChange = viewRect.Height != newValue.Height;
				this.viewRect = newValue;
				AnimatedLayoutViewMode carouselMode = ViewInfo.layoutManager as AnimatedLayoutViewMode;
				if(carouselMode != null) {
					if((fHorzChange && OptionsCarouselMode.StretchCardToViewWidth) || (fVertChange && OptionsCarouselMode.StretchCardToViewHeight))
						carouselMode.ResetCache();
				}
				ResetPanModeIfNeed(false);
				LayoutChangedSynchronized();
			}
			finally { fUpdateSize--; }
		}
		protected internal override void OnChildLayoutChanged(BaseView childView) { }
#if !SL
	[DevExpressXtraGridLocalizedDescription("LayoutViewIsVisible")]
#endif
		public override bool IsVisible { get { return ViewRect.X > -10000 && !ViewRect.IsEmpty && ViewRect.Right > 0 && ViewRect.Bottom > 0; } }
		protected override string ViewName { get { return "LayoutView"; } }
		protected internal override int ScrollPageSize {
			get { return (ViewInfo != null) ? ViewInfo.CalcScrollableCardsCount() : base.ScrollPageSize; }
		}
		protected internal new LayoutViewInfo ViewInfo { get { return base.ViewInfo as LayoutViewInfo; } }
		protected internal new LayoutViewPainter Painter { get { return base.Painter as LayoutViewPainter; } }
		public override BaseExportLink CreateExportLink(IExportProvider provider) {
			return null;
		}
		protected internal bool fInternalLockActions = false;
#if DEBUGTEST
		protected internal bool fInternalAutoAnswerYesInDesigner = false;
#endif
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LayoutViewState State { get { return state; } }
		[Browsable(false)]
		public override bool IsDefaultState { get { return State == LayoutViewState.Normal; } }
		[Browsable(false)]
		public override bool IsEditing { get { return State == LayoutViewState.Editing; } }
		[Browsable(false)]
		public override bool IsSizingState { get { return State == LayoutViewState.Sizing; } }
		protected override void SetEditingState() { state = LayoutViewState.Editing; }
		protected internal override void SetDefaultState() { SetState(LayoutViewState.Normal); }
		protected internal void SetState(LayoutViewState newValue) {
			if(State == newValue) return;
			LayoutViewState prevState = state;
			if(state == LayoutViewState.Sizing)
				Painter.StopSizing();
			if(IsEditing)
				CloseEditor();
			if(State == LayoutViewState.Scrolling) StopScrolling();
			if(prevState == LayoutViewState.Normal && newValue != LayoutViewState.Editing) {
				if(CheckNeedUpdateRow(newValue) && !UpdateCurrentRow()) return; 
			}
			state = newValue;
			if(IsDefaultState)
				ViewInfo.SelectionInfo.ClearPressedInfo();
			else {
				HideHint();
			}
			UpdateNavigator();
			Invalidate();
			bool fromNormalToPressed = (state == LayoutViewState.LayoutItemPressed && prevState == LayoutViewState.Normal);
			bool fromPressedToNormal = (prevState == LayoutViewState.LayoutItemPressed && state == LayoutViewState.Normal);
			if(!fromNormalToPressed && !fromPressedToNormal) FireChanged();
		}
		bool CheckNeedUpdateRow(LayoutViewState state) {
			LayoutViewHitInfo hi = (Handler != null) ? (Handler as LayoutViewHandler).downPointHitInfo : null;
			if((hi != null) && (hi.RowHandle == FocusedRowHandle))
				return state != LayoutViewState.LayoutItemPressed;
			return true;
		}
		public override void ShowFilterPopup(GridColumn column) {
			if(!CanFilterLayoutViewColumn(column)) return;
			LayoutViewField field = FindCardFieldByRow(FocusedRowHandle, column);
			if(field == null) return;
			LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
			Rectangle fieldRect = fieldInfo.ClientAreaRelativeToControl;
			fieldRect.Location += new Size(0, fieldInfo.FilterButton.Height);
			fieldRect.Size = new Size(fieldRect.Width, 0);
			Focus();
			ShowFilterPopup(column, fieldRect, GridControl, fieldInfo);
		}
		protected override void OnApplyColumnsFilterComplete() {
			base.OnApplyColumnsFilterComplete();
			ResetCardsCache();
			Refresh();
		}
		protected override void OnActiveFilterEnabledChanged() {
			base.OnActiveFilterEnabledChanged();
			ResetCardsCache();
			Refresh();
			if(!ActiveFilterEnabled && ViewInfo != null) {
				int visibleCardsCount = ViewInfo.VisibleCards.Count;
				if(visibleCardsCount == RecordCount) {
					visibleCardIndex = RecordCount - 1;
					internalArrangeFromLeftToRight = false;
					Refresh();
				}
			}
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
		protected virtual void ProcessLayoutItemHotTrackEnter(LayoutViewHitInfo hi) {
			if(hi.LayoutItemHitInfo.IsTabbedGroup) {
				try {
					TabbedControlGroup tab = hi.LayoutItem as TabbedControlGroup;
					MouseEventArgs ea = new MouseEventArgs(MouseButtons.None, 0, hi.HitPoint.X, hi.HitPoint.Y, 0);
					tab.ViewInfo.BorderInfo.Tab.Handler.ProcessEvent(EventType.MouseEnter, ea);
					tab.ViewInfo.BorderInfo.Tab.Handler.ProcessEvent(EventType.MouseMove, ea);
				}
				catch { }
			}
		}
		protected virtual void ProcessLayoutItemHotTrackLeave(LayoutViewHitInfo hi) {
			if(hi.LayoutItemHitInfo.IsTabbedGroup) {
				try {
					TabbedControlGroup tab = hi.LayoutItem as TabbedControlGroup;
					MouseEventArgs ea = new MouseEventArgs(MouseButtons.None, 0, hi.HitPoint.X, hi.HitPoint.Y, 0);
					tab.ViewInfo.BorderInfo.Tab.Handler.ProcessEvent(EventType.MouseLeave, ea);
				}
				catch { }
			}
		}
		internal int HotCardRowHandle = GridControl.InvalidRowHandle;
		internal void SetHotCard(LayoutViewCard card) {
			if(IsTouchScroll) return;
			int row = GridControl.InvalidRowHandle;
			if(card != null)
				row = card.RowHandle;
			if(HotCardRowHandle == row) return;
			int prevRow = HotCardRowHandle;
			HotCardRowHandle = row;
			if(prevRow != GridControl.InvalidRowHandle)
				InvalidateCard(prevRow);
			if(card != null)
				InvalidateCard(card);
		}
		protected override void OnHotTrackEnter(BaseHitInfo hitInfo) {
			if(IsTouchScroll) return;
			LayoutViewHitInfo hit = hitInfo as LayoutViewHitInfo;
			if(hit.InCard) {
				if(hit.InField && !PanModeActive && !IsCustomizationMode) {
					if(hit.InFieldValue) OnCellMouseEnter(hitInfo);
					hit.HitField.ViewInfo.State |= ObjectState.Hot;
				}
				else if(hit.InCardExpandButton) {
					hit.HitCard.ViewInfo.BorderInfo.ButtonState |= ObjectState.Hot;
				}
				else if(hit.InLayoutItem) ProcessLayoutItemHotTrackEnter(hit);
			}
			InvalidateHitObject(hit);
		}
		protected override void OnHotTrackLeave(BaseHitInfo hitInfo) {
			if(IsTouchScroll) return;
			LayoutViewHitInfo hit = hitInfo as LayoutViewHitInfo;
			if(hit.InCard && !hit.HitCard.IsDisposing) {
				if(hit.InField && !hit.HitField.IsDisposing && !PanModeActive && !IsCustomizationMode) {
					if(hit.InFieldValue) OnCellMouseLeave(hitInfo);
					hit.HitField.ViewInfo.State = ObjectState.Normal;
				}
				else if(hit.InCardExpandButton) {
					hit.HitCard.ViewInfo.BorderInfo.ButtonState = ObjectState.Normal;
				}
				else if(hit.InLayoutItem) ProcessLayoutItemHotTrackLeave(hit);
			}
			InvalidateHitObject(hit);
		}
		protected internal override void OnEmbeddedNavigatorSizeChanged() {
			if(ViewDisposing || !IsInitialized) return;
			base.OnEmbeddedNavigatorSizeChanged();
			ScrollInfo.LayoutChanged();
		}
		public override void SynchronizeVisual(BaseView viewSource) {
			if(viewSource == null) return;
			BeginSynchronization();
			BeginUpdate();
			try {
				base.SynchronizeVisual(viewSource);
				LayoutView lgv = viewSource as LayoutView;
				if(lgv == null) return;
				SyncCardProperties(lgv);
			}
			finally {
				EndUpdate();
				EndSynchronization();
			}
		}
		void SyncLayoutControlProperties(LayoutView sourceView) {
			ILayoutControl.OptionsView.AllowHotTrack = sourceView.OptionsView.AllowHotTrackFields;
			ILayoutControl.OptionsView.DrawItemBorders = sourceView.OptionsView.ShowCardFieldBorders;
			ILayoutControl.OptionsItemText.TextAlignMode = sourceView.OptionsItemText.ConvertTo(sourceView.OptionsItemText.AlignMode);
			ILayoutControl.OptionsItemText.TextToControlDistance = sourceView.OptionsItemText.TextToControlDistance;
		}
		void SyncCardProperties(LayoutView sourceView) {
			SyncTemplateCard(sourceView);
			SyncHiddenItems(sourceView);
		}
		void SyncTemplateCard(LayoutView sourceView) {
			if(sourceView.TemplateCard != null) {
				LayoutViewCard prevTemplateCard = this.TemplateCard;
				this.TemplateCard = (LayoutViewCard)sourceView.TemplateCard.Clone(null, this);
				List<BaseLayoutItem> items = new FlatItemsList().GetItemsList(TemplateCard);
				foreach(BaseLayoutItem item in items) {
					LayoutViewField field = item as LayoutViewField;
					if(field != null && field.Column != null) {
						field.SetLayoutViewColumn(Columns.ColumnByName(field.Column.Name));
					}
				}
				if(prevTemplateCard != null)
					prevTemplateCard.Dispose();
			}
		}
		void SyncHiddenItems(LayoutView sourceView) {
			if(sourceView.HiddenItems != null) {
				HiddenItems.Assign(sourceView.HiddenItems);
			}
		}
		void SyncProperties(LayoutView sourceView) {
			this.FieldCaptionFormat = sourceView.FieldCaptionFormat;
			this.CardMinSize = sourceView.CardMinSize;
			this.CardHorzInterval = sourceView.CardHorzInterval;
			this.CardVertInterval = sourceView.CardVertInterval;
			this.CardCaptionFormat = sourceView.CardCaptionFormat;
			this.DetailAutoHeight = sourceView.DetailAutoHeight;
		}
		void SyncOptions(LayoutView sourceView) {
			this.OptionsItemText.Assign(sourceView.OptionsItemText);
			this.OptionsSingleRecordMode.Assign(sourceView.OptionsSingleRecordMode);
			this.OptionsMultiRecordMode.Assign(sourceView.OptionsMultiRecordMode);
			this.OptionsCarouselMode.Assign(sourceView.OptionsCarouselMode);
			this.OptionsCustomization.Assign(sourceView.OptionsCustomization);
			this.OptionsHeaderPanel.Assign(sourceView.OptionsHeaderPanel);
		}
		protected virtual void UpdateVisibleCardCaptions() {
			if(ViewInfo != null && FocusedRowHandle != RecordCount) {
				foreach(LayoutViewCard card in ViewInfo.VisibleCards) {
					card.Text = GetCardCaption(card.RowHandle);
				}
				if(GridControl != null) GridControl.Refresh();
			}
		}
		protected override void VisualClientUpdateRows(int topRowIndexDelta) {
			if(DataController.IsCurrentRowEditing) return;
			else {
				ResetCardsCache();
				Refresh();
			}
		}
		protected internal virtual void CorrectArrangeDirection() {
			if(RecordCount > 0 && VisibleRecordIndex == RecordCount - 1) internalArrangeFromLeftToRight = false;
		}
		protected override void VisualClientUpdateScrollBar() {
			if(IsLockUpdate) return;
			UpdateScrollBars();
			UpdateVisibleCardCaptions();
		}
		protected override int VisualClientTopRowIndex {
			get { return (ViewInfo != null) ? ViewInfo.GetFirstCardRowHandle() : 0; }
		}
		protected override int VisualClientPageRowCount {
			get { return (ViewInfo != null) ? ViewInfo.VisibleCards.Count + 1 : 1; }
		}
		protected internal virtual bool DoLeaveFocusOnTab(bool moveForward) {
			if(OptionsBehavior.FocusLeaveOnTab) {
				if(CanLeaveFocusOnTab(moveForward)) {
					LeaveFocusOnTab(moveForward);
					return true;
				}
			}
			return false;
		}
		protected override bool CanLeaveFocusOnTab(bool moveForward) {
			LayoutViewCard focusedCard = (FocusedRowHandle != GridControl.InvalidRowHandle) ? FindCardByRow(FocusedRowHandle) : null;
			List<string> itemsByFocus = (focusedCard != null) ? ArrangeItemNamesByFocus(focusedCard, true) : new List<string>();
			bool fFirstFieldInCardFocused = (itemsByFocus.IndexOf(FocusedItemName) == 0);
			bool fLastFieldInCardFocused = (itemsByFocus.Count > 0) && (itemsByFocus.IndexOf(FocusedItemName) == (itemsByFocus.Count - 1));
			bool fLeaveIfFocusNotAllowed = (itemsByFocus.Count == 0);
			bool fLeaveFromEmptyGrid = (RowCount == 0);
			bool fLeaveFromLastCard = (RowCount > 0) && (moveForward && (FocusedRowHandle == (RowCount - 1))) && fLastFieldInCardFocused;
			bool fLeaveFromFirstCard = (!moveForward && (FocusedRowHandle == 0)) && fFirstFieldInCardFocused;
			return fLeaveIfFocusNotAllowed || fLeaveFromEmptyGrid || fLeaveFromFirstCard || fLeaveFromLastCard;
		}
		protected override void LeaveFocusOnTab(bool moveForward) {
			GridControl.ProcessControlTab(moveForward);
		}
		protected internal override void OnVisibleChanged() {
			base.OnVisibleChanged();
			if(GridControl != null && GridControl.Visible) {
				ScrollInfo.UpdateVisibility();
			}
		}
		#region Appearances
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LayoutViewAppearances PaintAppearance { get { return base.PaintAppearance as LayoutViewAppearances; } }
		internal bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		void ResetAppearance() { Appearance.Reset(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewAppearance"),
#endif
 DXCategory(CategoryName.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance)]
		public new LayoutViewAppearances Appearance { get { return base.Appearance as LayoutViewAppearances; } }
		protected override BaseViewAppearanceCollection CreateAppearances() { return new LayoutViewAppearances(this); }
		internal bool ShouldSerializeAppearancePrint() { return AppearancePrint.ShouldSerialize(); }
		void ResetAppearancePrint() { AppearancePrint.Reset(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewAppearancePrint"),
#endif
 DXCategory(CategoryName.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance)]
		public new LayoutViewPrintAppearances AppearancePrint {
			get { return base.AppearancePrint as LayoutViewPrintAppearances; }
		}
		protected override BaseViewAppearanceCollection CreateAppearancesPrint() {
			return new LayoutViewPrintAppearances(this);
		}
		protected internal override UserControl PrintDesigner {
			get {
				UserControl ctrl = new UserControl();
				LayoutViewPrintDesigner printDesigner = new LayoutViewPrintDesigner();
				printDesigner.InitFrame(this, "LayoutView", new Bitmap(16, 16));
				printDesigner.AutoApply = false;
				printDesigner.HideCaption();
				printDesigner.Dock = DockStyle.Fill;
				ctrl.Controls.Add(printDesigner);
				ctrl.Dock = DockStyle.Fill;
				ctrl.Visible = true;
				ctrl.Size = printDesigner.UserControlSize;
				return ctrl;
			}
		}
		protected virtual LayoutViewOptionsHeaderPanel CreateOptionsHeaderPanel() { return new LayoutViewOptionsHeaderPanel(this); }
		protected virtual LayoutViewOptionsCustomization CreateOptionsCustomization() { return new LayoutViewOptionsCustomization(this); }
		protected virtual LayoutViewOptionsSingleRecordMode CreateOptionSingleRecordMode() { return new LayoutViewOptionsSingleRecordMode(this); }
		protected virtual LayoutViewOptionsMultiRecordMode CreateOptionMultiRecordMode() { return new LayoutViewOptionsMultiRecordMode(this); }
		protected virtual LayoutViewOptionsCarouselMode CreateOptionsCarouselMode() { return new LayoutViewOptionsCarouselMode(this); }
		protected virtual LayoutViewOptionsItemText CreateOptionsItemText() { return new LayoutViewOptionsItemText(this); }
		protected override ViewPrintOptionsBase CreateOptionsPrint() { return new LayoutViewOptionsPrint(); }
		#endregion Appearances
		#region ILayoutControl
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)]
		public HiddenItemsCollection HiddenItems {
			get { return ILayoutControl.HiddenItems; }
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 1000, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdColumns)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public ReadOnlyItemCollection Items {
			get { return ILayoutControl.Items; }
			set { ILayoutControl.Items = value; }
		}
		event EventHandler ILayoutControl.Changed {
			add { changedCore += value; }
			remove { changedCore -= value; }
		}
		event CancelEventHandler ILayoutControl.Changing {
			add { changingCore += value; }
			remove { changingCore -= value; }
		}
		bool ILayoutControl.AllowManageDesignSurfaceComponents {
			get { return ILayoutControl.AllowManageDesignSurfaceComponents; }
			set { ILayoutControl.AllowManageDesignSurfaceComponents = value; }
		}
		LayoutPaintStyle ILayoutControl.PaintStyle {
			get { return ILayoutControl.PaintStyle; }
		}
		LayoutControlGroup ILayoutControl.RootGroup {
			get { return ILayoutControl.RootGroup; }
			set { ILayoutControl.RootGroup = value; }
		}
		LayoutControlGroup ILayoutControlOwner.Root {
			get { return templateCard; }
			set { 
				templateCard = (LayoutViewCard)value;
				if(templateCard != null)
					templateCard.CheckRTL(ControlRightToLeft);
			}
		}
		RightButtonMenuManager ILayoutControl.CustomizationMenuManager {
			get { return ILayoutControl.CustomizationMenuManager; }
		}
		UserCustomizationForm ILayoutControl.CustomizationForm {
			get { return ILayoutControl.CustomizationForm; }
		}
		void ILayoutControl.UpdateFocusedElement(BaseLayoutItem item) {
			ILayoutControl.UpdateFocusedElement(item);
		}
		bool ILayoutControl.IsDeserializing {
			get { return ILayoutControl.IsDeserializing; }
			set { ILayoutControl.IsDeserializing = value; }
		}
		bool ILayoutControl.IsSerializing {
			get { return ILayoutControl.IsSerializing; }
		}
		bool ILayoutControl.AllowSetIsModified {
			get { return ILayoutControl.AllowSetIsModified; }
			set { ILayoutControl.AllowSetIsModified = value; }
		}
		bool ILayoutControl.AllowCustomizationMenu {
			get { return ILayoutControl.AllowCustomizationMenu; }
			set { ILayoutControl.AllowCustomizationMenu = value; }
		}
		CustomizationModes ILayoutControl.CustomizationMode {
			get { return CustomizationModes.Default; }
			set { }
		}
		bool ILayoutControl.SelectedChangedFlag {
			get { return ILayoutControl.SelectedChangedFlag; }
			set { ILayoutControl.SelectedChangedFlag = value; }
		}
		void ILayoutControl.SetCursor(System.Windows.Forms.Cursor cursor) {
			ILayoutControl.SetCursor(cursor);
		}
		Rectangle ILayoutControl.Bounds {
			get { return ILayoutControl.Bounds; }
		}
		DevExpress.XtraLayout.Scrolling.ScrollInfo ILayoutControl.Scroller {
			get { return ILayoutControl.Scroller; }
		}
		Control ILayoutControl.Control {
			get { return GridControl; }
		}
		bool ILayoutControl.AllowPaintEmptyRootGroupText { get { return false; } set { } }
		bool ILayoutControl.DesignMode {
			get { return ILayoutControl.DesignMode; }
		}
		OptionsCustomizationForm ILayoutControl.OptionsCustomizationForm {
			get { return ILayoutControl.OptionsCustomizationForm; }
		}
		UndoManager ILayoutControl.UndoManager {
			get { return ILayoutControl.UndoManager; }
		}
		LayoutAppearanceCollection ILayoutControl.Appearance {
			get { return ILayoutControl.Appearance; }
		}
		AppearanceController ILayoutControl.AppearanceController {
			get { return ILayoutControl.AppearanceController; }
			set { ILayoutControl.AppearanceController = value; }
		}
		bool ILayoutControl.LockUpdateOnChangeUICuesRequest {
			get { return ILayoutControl.LockUpdateOnChangeUICuesRequest; }
			set { ILayoutControl.LockUpdateOnChangeUICuesRequest = value; }
		}
		void ILayoutControl.OnChangeUICues() { ILayoutControl.OnChangeUICues(); }
		EnabledStateController ILayoutControl.EnabledStateController {
			get { return ILayoutControl.EnabledStateController; }
			set { ILayoutControl.EnabledStateController = value; }
		}
		SizeF ILayoutControl.AutoScaleFactor {
			get { return GetScaleFactor(); }
		}
		LayoutControlDefaultsPropertyBag ILayoutControl.DefaultValues {
			get { return ILayoutControl.DefaultValues; }
			set { ILayoutControl.DefaultValues = value; }
		}
		LayoutControlHandler ILayoutControl.ActiveHandler {
			get { return ILayoutControl.ActiveHandler; }
		}
		internal Color GetForeColor() {
			return PaintAppearance.ViewBackground.ForeColor;
		}
		Color ITransparentBackgroundManager.GetForeColor(object childObject) {
			return implementorCore.GetForeColor(childObject as BaseLayoutItem, GetForeColor());
		}
		Color ITransparentBackgroundManager.GetForeColor(Control childControl) {
			return implementorCore.GetForeColor(childControl, GetForeColor());
		}
		void ILayoutControlOwner.RaiseOwnerEvent_CloseButtonClick(object sender, LayoutGroupEventArgs ea) { }
		void ILayoutControl.FireCloseButtonClick(LayoutGroup component) {
			implementorCore.FireCloseButtonClick(component);
		}
		bool ILayoutControl.CheckIfControlIsInLayout(System.Windows.Forms.Control control) {
			return ILayoutControl.CheckIfControlIsInLayout(control);
		}
		bool ILayoutControl.EnableCustomizationMode {
			get { return ILayoutControl.EnableCustomizationMode; }
			set { ILayoutControl.EnableCustomizationMode = value; }
		}
		bool ILayoutControl.EnableCustomizationForm {
			get { return ILayoutControl.EnableCustomizationForm; }
			set { ILayoutControl.EnableCustomizationForm = value; }
		}
		LayoutGroup ILayoutControl.GetGroupAtPoint(Point point) {
			return ILayoutControl.GetGroupAtPoint(point);
		}
		void ILayoutControl.RaiseShowCustomizationMenu(DevExpress.XtraLayout.PopupMenuShowingEventArgs ma) {
			ILayoutControl.RaiseShowCustomizationMenu(ma);
		}
		void ILayoutControl.RaiseShowLayoutTreeViewContextMenu(DevExpress.XtraLayout.PopupMenuShowingEventArgs ma) {
			ILayoutControl.RaiseShowLayoutTreeViewContextMenu(ma);
		}
		bool ILayoutControl.FireChanging(IComponent component) {
			return ILayoutControl.FireChanging(component);
		}
		void ILayoutControl.FireChanged(IComponent component) {
			ILayoutControl.FireChanged(component);
		}
		void ILayoutControl.SelectionChanged(IComponent component) {
			ILayoutControl.SelectionChanged(component);
		}
		void ILayoutControl.AddComponent(BaseLayoutItem component) {
			ILayoutControl.AddComponent(component);
		}
		ConstraintsManager ILayoutControl.ConstraintsManager {
			get { return ILayoutControl.ConstraintsManager; }
		}
		void ILayoutControl.RemoveComponent(BaseLayoutItem component) {
			ILayoutControl.RemoveComponent(component);
		}
		void ILayoutControl.Invalidate() {
			if(ViewInfo != null) {
				foreach(LayoutViewCard card in ViewInfo.VisibleCards)
					card.InvalidateCore();
			}
		}
		void ILayoutControl.SetControlDefaults() {
			ILayoutControl.SetControlDefaults();
		}
		void ILayoutControl.SetControlDefaultsLast() {
			ILayoutControl.SetControlDefaultsLast();
		}
		bool ILayoutControl.DisposingFlag {
			get { return ILayoutControl.DisposingFlag; }
		}
		FocusHelperBase ILayoutControl.FocusHelper {
			get { return ILayoutControl.FocusHelper; }
			set { ILayoutControl.FocusHelper = value; }
		}
		int ILayoutControl.SelectedChangedCount {
			get { return ILayoutControl.SelectedChangedCount; }
			set { ILayoutControl.SelectedChangedCount = value; }
		}
		int ILayoutControl.UpdatedCount {
			get { return ILayoutControl.UpdatedCount; }
			set { ILayoutControl.UpdatedCount = value; }
		}
		LayoutGroup ILayoutControl.CreateLayoutGroup(LayoutGroup parent) {
			return ILayoutControl.CreateLayoutGroup(parent);
		}
		BaseLayoutItem ILayoutControl.CreateLayoutItem(LayoutGroup parent) {
			return ILayoutControl.CreateLayoutItem(parent);
		}
		TabbedGroup ILayoutControl.CreateTabbedGroup(LayoutGroup parent) {
			return ILayoutControl.CreateTabbedGroup(parent);
		}
		EmptySpaceItem ILayoutControl.CreateEmptySpaceItem(LayoutGroup parent) {
			return ILayoutControl.CreateEmptySpaceItem(parent);
		}
		SplitterItem ILayoutControl.CreateSplitterItem(LayoutGroup parent) {
			return ILayoutControl.CreateSplitterItem(parent);
		}
		FakeFocusContainer ILayoutControl.FakeFocusContainer {
			get { return ILayoutControl.FakeFocusContainer; }
		}
		TextAlignManager ILayoutControl.TextAlignManager {
			get { return ILayoutControl.TextAlignManager; }
		}
		LayoutGroupHandlerWithTabHelper ILayoutControl.CreateRootHandler(LayoutGroup group) {
			return ILayoutControl.CreateRootHandler(group);
		}
		void ILayoutControl.RegisterLayoutAdapter(ILayoutAdapter adapter) {
			ILayoutControl.RegisterLayoutAdapter(adapter);
		}
		void ILayoutControl.RaiseGroupExpandChanging(LayoutGroupCancelEventArgs e) {
			ILayoutControl.RaiseGroupExpandChanging(e);
		}
		void ILayoutControl.RaiseGroupExpandChanged(LayoutGroupEventArgs e) {
			ILayoutControl.RaiseGroupExpandChanged(e);
		}
		void ILayoutControl.SetIsModified(bool newVal) {
			ILayoutControl.SetIsModified(newVal);
		}
		EditorContainer ILayoutControl.GetEditorContainer() {
			return GridControl;
		}
		void ILayoutControl.FireItemRemoved(BaseLayoutItem component, EventArgs ea) {
			ILayoutControl.FireItemRemoved(component, ea);
		}
		bool ILayoutControl.InitializationFinished {
			get { return ILayoutControl.InitializationFinished; }
			set { ILayoutControl.InitializationFinished = value; }
		}
		bool ILayoutControl.ShouldArrangeTextSize {
			get { return ILayoutControl.ShouldArrangeTextSize; }
			set { ILayoutControl.ShouldArrangeTextSize = value; }
		}
		bool ILayoutControl.ShouldUpdateConstraints {
			get { return ILayoutControl.ShouldUpdateConstraints; }
			set { ILayoutControl.ShouldUpdateConstraints = value; }
		}
		bool ILayoutControl.ShouldResize {
			get { return ILayoutControl.ShouldResize; }
			set { ILayoutControl.ShouldResize = value; }
		}
		bool ILayoutControl.ShouldUpdateLookAndFeel {
			get { return ILayoutControl.ShouldUpdateLookAndFeel; }
			set { ILayoutControl.ShouldUpdateLookAndFeel = value; }
		}
		bool ILayoutControl.ShouldUpdateControlsLookAndFeel {
			get { return ILayoutControl.ShouldUpdateControlsLookAndFeel; }
			set { ILayoutControl.ShouldUpdateControlsLookAndFeel = value; }
		}
		bool ILayoutControl.ShouldUpdateControls {
			get { return ILayoutControl.ShouldUpdateControls; }
			set { ILayoutControl.ShouldUpdateControls = value; }
		}
		int ILayoutControl.DelayPainting {
			get { return ILayoutControl.DelayPainting; }
			set { ILayoutControl.DelayPainting = value; }
		}
		LayoutControlRoles ILayoutControl.ControlRole {
			get { return LayoutControlRoles.MainControl; }
			set { }
		}
		LayoutSerializationOptions ILayoutControl.OptionsSerialization {
			get { return ILayoutControl.OptionsSerialization; }
		}
		bool ILayoutControl.IsHidden(BaseLayoutItem item) {
			return ILayoutControl.IsHidden(item);
		}
		void ILayoutControl.ShowCustomizationForm() {
			ILayoutControl.ShowCustomizationForm();
		}
		void ILayoutControl.HideCustomizationForm() {
			ILayoutControl.HideCustomizationForm();
		}
		void ILayoutControl.RestoreDefaultLayout() {
			ILayoutControl.RestoreDefaultLayout();
		}
		SerializeableUserLookAndFeel ILayoutControl.LookAndFeel {
			get { return ILayoutControl.LookAndFeel; }
		}
		OptionsItemText ILayoutControl.OptionsItemText {
			get { return ILayoutControl.OptionsItemText; }
		}
		OptionsView ILayoutControl.OptionsView {
			get { return ILayoutControl.OptionsView; }
		}
		OptionsFocus ILayoutControl.OptionsFocus {
			get { return ILayoutControl.OptionsFocus; }
		}
		IDXMenuManager ILayoutControl.MenuManager {
			get { return ILayoutControl.MenuManager; }
			set { ILayoutControl.MenuManager = value; }
		}
		LayoutControlItem ILayoutControl.GetItemByControl(Control control) {
			return ILayoutControl.GetItemByControl(control);
		}
		object ILayoutControl.Images {
			get { return ILayoutControl.Images; }
			set { ILayoutControl.Images = value; }
		}
		bool ILayoutControl.ExceptionsThrown {
			get { return ILayoutControl.ExceptionsThrown; }
			set { ILayoutControl.ExceptionsThrown = value; }
		}
		void ILayoutControl.SaveLayoutToXml(string xmlFile) {
			ILayoutControl.SaveLayoutToXml(xmlFile);
		}
		void ILayoutControl.RestoreLayoutFromXml(string xmlFile) {
			ILayoutControl.RestoreLayoutFromXml(xmlFile);
		}
		void ILayoutControl.RebuildCustomization() {
			ILayoutControl.RebuildCustomization();
		}
		void ILayoutControl.UpdateChildControlsLookAndFeel() {
			ILayoutControl.UpdateChildControlsLookAndFeel();
		}
		bool ILayoutControl.IsUpdateLocked {
			get { return ILayoutControl.IsUpdateLocked; }
		}
		List<IComponent> ILayoutControl.Components { get { return ILayoutControl.Components; } }
		Dictionary<Control, LayoutControlItem> ILayoutControl.ControlsAndItems {
			get { return ILayoutControl.ControlsAndItems; }
			set { ILayoutControl.ControlsAndItems = value; }
		}
		void ILayoutControl.FireItemAdded(BaseLayoutItem component, EventArgs ea) {
			ILayoutControl.FireItemAdded(component, ea);
		}
		string ILayoutControl.GetUniqueName(BaseLayoutItem item) {
			return ILayoutControl.GetUniqueName(item);
		}
		IComparer ILayoutControl.HiddenItemsSortComparer {
			get { return ILayoutControl.HiddenItemsSortComparer; }
			set { ILayoutControl.HiddenItemsSortComparer = value; }
		}
		void ILayoutControl.HideSelectedItems() {
			ILayoutControl.HideSelectedItems();
		}
		Dictionary<string, BaseLayoutItem> ILayoutControl.ItemsAndNames {
			get { return ILayoutControl.ItemsAndNames; }
			set { ILayoutControl.ItemsAndNames = value; }
		}
		void ILayoutControl.SelectParentGroup() {
			ILayoutControl.SelectParentGroup();
		}
		Control ILayoutControl.Parent {
			get { return ILayoutControl.Parent; }
			set { ILayoutControl.Parent = value; }
		}
		int ILayoutControl.ClientHeight { get { return ILayoutControl.ClientHeight; } }
		int ILayoutControl.ClientWidth { get { return ILayoutControl.ClientWidth; } }
		Size ILayoutControl.Size {
			get { return ILayoutControl.Size; }
			set { ILayoutControl.Size = value; }
		}
		int ILayoutControl.Width {
			get { return ILayoutControl.Width; }
			set { ILayoutControl.Width = value; }
		}
		int ILayoutControl.Height {
			get { return ILayoutControl.Height; }
			set { ILayoutControl.Height = value; }
		}
		Control ILayoutControlOwner.Parent {
			get { return GridControl.Parent; }
			set { GridControl.Parent = value; }
		}
		int ILayoutControlOwner.ClientHeight { get { return viewLayoutSize.Height; } }
		int ILayoutControlOwner.ClientWidth { get { return viewLayoutSize.Width; } }
		Size ILayoutControlOwner.Size {
			get {
				if(viewLayoutSize.IsEmpty) { viewLayoutSize = CardMinSize; }
				return viewLayoutSize;
			}
			set { viewLayoutSize = value; }
		}
		int ILayoutControlOwner.Width {
			get { return viewLayoutSize.Width; }
			set { }
		}
		int ILayoutControlOwner.Height {
			get { return viewLayoutSize.Height; }
			set { }
		}
		#endregion
		private static readonly object cardExpanding = new object();
		private static readonly object cardExpanded = new object();
		private static readonly object cardCollapsing = new object();
		private static readonly object cardCollapsed = new object();
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCardExpanded"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event RowEventHandler CardExpanded {
			add { this.Events.AddHandler(cardExpanded, value); }
			remove { this.Events.RemoveHandler(cardExpanded, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCardCollapsed"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event RowEventHandler CardCollapsed {
			add { this.Events.AddHandler(cardCollapsed, value); }
			remove { this.Events.RemoveHandler(cardCollapsed, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCardCollapsing"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event RowAllowEventHandler CardCollapsing {
			add { this.Events.AddHandler(cardCollapsing, value); }
			remove { this.Events.RemoveHandler(cardCollapsing, value); }
		}
		internal int iCustomRowCellEditEventHandlerCounter = 0;
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCustomRowCellEdit"),
#endif
 DXCategory(CategoryName.Editor)]
		public event LayoutViewCustomRowCellEditEventHandler CustomRowCellEdit {
			add {
				this.Events.AddHandler(customRowCellEdit, value);
				iCustomRowCellEditEventHandlerCounter++;
			}
			remove {
				this.Events.RemoveHandler(customRowCellEdit, value);
				iCustomRowCellEditEventHandlerCounter--;
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCardExpanding"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event RowAllowEventHandler CardExpanding {
			add { this.Events.AddHandler(cardExpanding, value); }
			remove { this.Events.RemoveHandler(cardExpanding, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewVisibleRecordIndexChanged"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event LayoutViewVisibleRecordIndexChangedEventHandler VisibleRecordIndexChanged {
			add { this.Events.AddHandler(visibleRecordIndexChanged, value); }
			remove { this.Events.RemoveHandler(visibleRecordIndexChanged, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCustomCardCaptionImage"),
#endif
 DXCategory(CategoryName.Appearance)]
		public event LayoutViewCardCaptionImageEventHandler CustomCardCaptionImage {
			add { this.Events.AddHandler(customCardCaptionImage, value); }
			remove { this.Events.RemoveHandler(customCardCaptionImage, value); }
		}
		internal int iCustomCardLayoutEventHandlerCounter = 0;
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCustomCardLayout"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event LayoutViewCustomCardLayoutEventHandler CustomCardLayout {
			add {
				iCustomCardLayoutEventHandlerCounter++;
				this.Events.AddHandler(customCardLayoutCore, value);
			}
			remove {
				this.Events.RemoveHandler(customCardLayoutCore, value);
				iCustomCardLayoutEventHandlerCounter--;
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCustomDrawCardCaption"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event LayoutViewCustomDrawCardCaptionEventHandler CustomDrawCardCaption {
			add { this.Events.AddHandler(customDrawCardCaption, value); }
			remove { this.Events.RemoveHandler(customDrawCardCaption, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCustomDrawCardFieldCaption"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event RowCellCustomDrawEventHandler CustomDrawCardFieldCaption {
			add { this.Events.AddHandler(customDrawCardFieldCaption, value); }
			remove { this.Events.RemoveHandler(customDrawCardFieldCaption, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCustomDrawCardFieldValue"),
#endif
 DXCategory(CategoryName.CustomDraw)]
		public event RowCellCustomDrawEventHandler CustomDrawCardFieldValue {
			add { this.Events.AddHandler(customDrawCardFieldValue, value); }
			remove { this.Events.RemoveHandler(customDrawCardFieldValue, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCustomCardStyle"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event LayoutViewCardStyleEventHandler CustomCardStyle {
			add { this.Events.AddHandler(cardStyle, value); }
			remove { this.Events.RemoveHandler(cardStyle, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCustomFieldCaptionStyle"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event LayoutViewFieldCaptionStyleEventHandler CustomFieldCaptionStyle {
			add { this.Events.AddHandler(fieldCaptionStyle, value); }
			remove { this.Events.RemoveHandler(fieldCaptionStyle, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCustomFieldValueStyle"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event LayoutViewFieldValueStyleEventHandler CustomFieldValueStyle {
			add { this.Events.AddHandler(fieldValueStyle, value); }
			remove { this.Events.RemoveHandler(fieldValueStyle, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCustomSeparatorStyle"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event LayoutViewCustomSeparatorStyleEventHandler CustomSeparatorStyle {
			add { this.Events.AddHandler(separatorStyle, value); }
			remove { this.Events.RemoveHandler(separatorStyle, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCustomFieldEditingValueStyle"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event LayoutViewFieldEditingValueStyleEventHandler CustomFieldEditingValueStyle {
			add { this.Events.AddHandler(fieldEditingValueStyle, value); }
			remove { this.Events.RemoveHandler(fieldEditingValueStyle, value); }
		}
		internal int iCustomFieldCaptionEventHandlerCounter = 0;
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCustomFieldCaptionImage"),
#endif
 DXCategory(CategoryName.Appearance)]
		public event LayoutViewFieldCaptionImageEventHandler CustomFieldCaptionImage {
			add {
				iCustomFieldCaptionEventHandlerCounter++;
				this.Events.AddHandler(customFieldCaptionImage, value);
			}
			remove {
				this.Events.RemoveHandler(customFieldCaptionImage, value);
				iCustomFieldCaptionEventHandlerCounter--;
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewCardClick"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event CardClickEventHandler CardClick {
			add { this.Events.AddHandler(cardClick, value); }
			remove { this.Events.RemoveHandler(cardClick, value); }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("LayoutViewFieldValueClick"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event FieldValueClickEventHandler FieldValueClick {
			add { this.Events.AddHandler(fieldValueClick, value); }
			remove { this.Events.RemoveHandler(fieldValueClick, value); }
		}
		protected internal virtual List<Line> GetLines() {
			return ViewInfo.GetLines();
		}
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
		public void ExpandCard(int rowHandle) { SetCardCollapsed(rowHandle, false); }
		public void CollapseCard(int rowHandle) { SetCardCollapsed(rowHandle, true); }
		public void SetCardCollapsed(int rowHandle, bool collapsed) {
			if(!OptionsBehavior.AllowExpandCollapse) return;
			if(GetCardCollapsed(rowHandle) != collapsed) {
				bool allow = collapsed ? RaiseCardCollapsing(rowHandle) : RaiseCardExpanding(rowHandle);
				if(!allow) return;
				((ILayoutViewDataController)DataController).Selection.SetCollapsed(rowHandle, collapsed);
				ViewInfo.NeedArrangeForce = true;
				CheckBoundingCards(rowHandle);
				LayoutChangedSynchronized();
				if(collapsed) FocusedColumn = null;
				if(collapsed) RaiseCardCollapsed(rowHandle);
				else RaiseCardExpanded(rowHandle);
			}
		}
		protected override void SelectAnchorRangeCore(bool controlPressed, bool allowCells) {
			if(!IsMultiSelect) return;
			if(!IsValidRowHandle(SelectionAnchorRowHandle)) return;
			BeginSelection();
			try {
				base.SelectAnchorRangeCore(controlPressed, allowCells);
			}
			finally { EndSelection(); }
		}
		void CheckBoundingCards(int rowHandle) {
			bool fLast = (ViewInfo.GetLastCardRowHandle() == rowHandle);
			bool fFirst = (ViewInfo.GetFirstCardRowHandle() == rowHandle);
			if(fLast || fFirst) {
				this.visibleCardIndex = GetVisibleIndex(rowHandle);
				this.internalArrangeFromLeftToRight = !fLast || fFirst;
			}
		}
		public bool GetCardCollapsed(int rowHandle) {
			return ((ILayoutViewDataController)DataController).Selection.GetCollapsed(rowHandle);
		}
		protected override BaseGridController CreateDataController() {
			if(this.requireDataControllerType == DataControllerType.ServerMode)
				return new LayoutViewServerModeDataController();
			if(this.requireDataControllerType == DataControllerType.RegularNoCurrencyManager)
				return new LayoutViewDataControllerRegularNoCurrencyManager();
			return new LayoutViewDataController();
		}
		protected internal override bool OnCheckHotTrackMouseMove(BaseHitInfo hitInfo) {
			LayoutViewHitInfo hi = hitInfo as LayoutViewHitInfo;
			if(hi.InFieldValue && !PanModeActive && !IsCustomizationMode) return OnCellMouseMove(hitInfo);
			return true;
		}
		protected override object GetCellInfo(BaseHitInfo hitInfo) {
			LayoutViewHitInfo hi = hitInfo as LayoutViewHitInfo;
			return hi.InField ? hi.HitField : null;
		}
		protected override BaseEditViewInfo GetCellEditInfo(object cellInfo) {
			LayoutViewField field = cellInfo as LayoutViewField;
			LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
			return (fieldInfo != null) ? fieldInfo.RepositoryItemViewInfo : null;
		}
		protected override bool UpdateCellHotInfo(BaseHitInfo hitInfo, Point hitPoint) {
			LayoutViewHitInfo hi = hitInfo as LayoutViewHitInfo;
			if(hi.HitField == null || hi.HitField.ViewInfo == null) return false;
			LayoutViewField field = hi.HitField;
			LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
			BaseEditViewInfo editorInfo = fieldInfo.RepositoryItemViewInfo;
			if(!editorInfo.IsRequiredUpdateOnMouseMove) return false;
			if(editorInfo.UpdateObjectState(MouseButtons.None, hitPoint)) return true;
			return false;
		}
		public bool IsCardVisible(int rowHandle) {
			return FindCardByRow(rowHandle) != null;
		}
		protected override void MakeRowVisibleCore(int rowHandle, bool invalidate) {
			if(IsCardVisible(rowHandle)) {
				if(invalidate) InvalidateCard(rowHandle);
				return;
			}
			if(IsNewRowEditing) {
				ResetCardsCache();
				DoNavigateRecords(rowHandle, false);
			}
		}
		protected override void OnOptionChanged(object sender, BaseOptionChangedEventArgs e) {
			if(ViewInfo != null) 
				ViewInfo.NeedArrangeForce = true;
			switch(e.Name) {
				case "RecordCount":
				case "InterpolationMode":
					var animatedMode = ViewInfo.layoutManager as AnimatedLayoutViewMode;
					if(animatedMode != null)
						animatedMode.ResetCache();
					break;
				case "StretchCardToViewHeight":
				case "StretchCardToViewWidth":
					ResetCardsCache();
					break;
				case "ShowCardCaption":
					if(TemplateCard != null)
						TemplateCard.GroupBordersVisible = (bool)e.NewValue;
					break;
				case "ShowCardFieldBorders":
					ILayoutControl.OptionsView.DrawItemBorders = (bool)e.NewValue;
					break;
				case "AllowHotTrackFields":
					ILayoutControl.OptionsView.AllowHotTrack = (bool)e.NewValue;
					break;
				case "AlignMode":
					ILayoutControl.OptionsItemText.TextAlignMode = OptionsItemText.ConvertTo((FieldTextAlignMode)e.NewValue);
					break;
				case "TextToControlDistance":
					ILayoutControl.OptionsItemText.TextToControlDistance = (int)e.NewValue;
					break;
				case "ViewMode":
					if(OptionsView.ViewMode == LayoutViewMode.Carousel) {
						if((ViewInfo != null) && ViewInfo.IsVisible(FocusedRowHandle))
							this.visibleCardIndex = FocusedRowHandle;
					}
					bool stretchSingleCard = OptionsSingleRecordMode.StretchCardToViewHeight || OptionsSingleRecordMode.StretchCardToViewWidth;
					bool stretchMultipleCards = OptionsMultiRecordMode.StretchCardToViewHeight || OptionsMultiRecordMode.StretchCardToViewWidth;
					if(stretchSingleCard || stretchMultipleCards)
						ResetCardsCache();
					break;
			}
			ResetPanModeIfNeed();
			base.OnOptionChanged(sender, e);
		}
		protected virtual void OnViewStyles_Changed(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		protected override void OnRightToLeftChanged() {
			ResetCardsCache();
			if(TemplateCard != null)
				TemplateCard.CheckRTL(IsRightToLeft);
			base.OnRightToLeftChanged();
		}
		protected internal override void OnPropertiesChanged() {
			if(ViewInfo != null) ViewInfo.NeedArrangeForce = true;
			base.OnPropertiesChanged();
			ResetPanModeIfNeed();
		}
		protected override void DoAfterFocusedColumnChanged(GridColumn prevFocusedColumn, GridColumn focusedColumn) {
			InvalidateCard(FocusedRowHandle);
			base.DoAfterFocusedColumnChanged(prevFocusedColumn, focusedColumn);
			UpdateRowViewInfo(FocusedRowHandle);
			InvalidateCard(FocusedRowHandle);
		}
		protected virtual GridRowCellState CalcRowStateCore(int rowHandle) {
			GridRowCellState state = GridRowCellState.Default;
			if(IsFocusedView) state |= GridRowCellState.GridFocused;
			if(FocusedRowHandle == rowHandle) state |= GridRowCellState.Focused;
			if(IsRowSelected(rowHandle)) state |= GridRowCellState.Selected;
			return state;
		}
		protected internal override void MoveTo(int rowHandle, Keys byKey) {
			int index = GetVisibleIndex(rowHandle);
			DoNavigateRecords(index, true);
		}
		internal bool fInternalFocusedRowChange = false;
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
			if(!fInternalFocusedRowChange) LayoutChangedSynchronized();
			base.DoChangeFocusedRow(prevFocused, FocusedRowHandle, raiseUpdateCurrentRow);
		}
		public void InvalidateCardField(int rowHandle, GridColumn column) {
			LayoutViewField item = FindCardFieldByRow(rowHandle, column);
			if(item != null) {
				LayoutViewFieldInfo viewInfo = item.ViewInfo as LayoutViewFieldInfo;
				InvalidateRect(viewInfo.BoundsRelativeToControl);
			}
		}
		public void InvalidateCardCaption(int rowHandle) {
			LayoutViewCard card = FindCardByRow(rowHandle);
			if(card != null) {
				card.Text = GetCardCaption(rowHandle);
				InvalidateRect(card.ViewInfo.BorderInfo.CaptionBounds);
			}
		}
		public void InvalidateCard(LayoutViewCard card) {
			InvalidateRect(card.Bounds);
		}
		public void InvalidateCard(int rowHandle) {
			if(ViewInfo == null) return;
			LayoutViewCard card = FindCardByRow(rowHandle);
			if(card != null)
				InvalidateRect(card.Bounds);
		}
		protected override void SetRowCellValueCore(int rowHandle, GridColumn column, object value, bool fromEditor) {
			base.SetRowCellValueCore(rowHandle, column, value, fromEditor);
			UpdateRowViewInfo(rowHandle);
			InvalidateCardField(rowHandle, column);
			InvalidateCardCaption(rowHandle);
		}
		protected override void RefreshRow(int rowHandle, bool updateEditor, bool updateEditorValue) {
			UpdateRowViewInfo(rowHandle);
			InvalidateCard(rowHandle);
			base.RefreshRow(rowHandle, updateEditor, updateEditorValue);
		}
		protected virtual void UpdateRowViewInfo(int rowHandle) {
			if(ViewInfo == null) return;
			LayoutViewCard card = FindCardByRow(rowHandle);
			if(card != null) ViewInfo.SetCardViewInfoDirty(card);
		}
		protected virtual LayoutViewHitInfo CreateFieldHitInfo(int rowHandle, GridColumn column) {
			LayoutViewCard card = FindCardByRow(rowHandle);
			if(card != null) {
				LayoutViewHitInfo hi = new LayoutViewHitInfo();
				hi.HitCard = card;
				hi.RowHandle = card.RowHandle;
				hi.VisibleIndex = card.VisibleIndex;
				LayoutViewField field = FindCardFieldByColumn(card, column);
				if(field != null) {
					LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
					hi.HitTest = LayoutViewHitTest.FieldValue;
					hi.Column = column;
					hi.HitField = field;
					hi.HitPoint = fieldInfo.RepositoryItemViewInfo.Bounds.Location;
					hi.HitRect = fieldInfo.RepositoryItemViewInfo.Bounds;
					return hi;
				}
			}
			return null;
		}
		protected internal virtual LayoutViewField FindCardFieldByRow(int rowHandle, GridColumn column) {
			LayoutViewCard card = FindCardByRow(rowHandle);
			if(card != null) {
				return FindCardFieldByColumn(card, column);
			}
			return null;
		}
		protected internal virtual LayoutViewField FindCardFieldByColumn(LayoutViewCard card, GridColumn column) {
			if(card == null || column == null) return null;
			return ViewInfo.FindFieldItemByColumnName(card, column.Name);
		}
		protected internal virtual LayoutViewCard FindCardByRow(int rowHandle) {
			LayoutViewCard findedCard = null;
			foreach(LayoutViewCard card in ViewInfo.VisibleCards) {
				if(card.RowHandle == rowHandle) { return card; }
			}
			return findedCard;
		}
		protected internal override bool GetCanShowEditor(GridColumn column) {
			LayoutViewCarouselMode animatedMode = ViewInfo.layoutManager as LayoutViewCarouselMode;
			bool fRestrictWhileAnimation = ((animatedMode != null) && animatedMode.IsAnimated);
			return fRestrictWhileAnimation ? false : base.GetCanShowEditor(column);
		}
		public override void ShowEditor() {
			if(GetCardCollapsed(FocusedRowHandle)) return;
			CloseEditor();
			if(State != LayoutViewState.Normal || !GetCanShowEditor(FocusedColumn)) return;
			LayoutViewHitInfo focusedHitInfo = CreateFieldHitInfo(FocusedRowHandle, FocusedColumn);
			if(focusedHitInfo == null) return;
			if(focusedHitInfo.HitRect.Width > 6) {
				ActivateEditor(focusedHitInfo);
			}
		}
		protected internal void ActivateEditor(LayoutViewHitInfo hi) {
			if(!CanActivateEditor(hi)) return;
			LayoutViewFieldInfo fieldInfo = hi.HitField.ViewInfo as LayoutViewFieldInfo;
			BaseEditViewInfo editorInfo = fieldInfo.RepositoryItemViewInfo;
			Rectangle editorBounds = ViewInfo.CheckBounds(hi.HitRect, ViewInfo.ViewRects.CardsRect);
			if(editorBounds.Width < 1 || editorBounds.Height < 1) return;
			editingFieldInfo = hi;
			InvalidateRect(hi.HitRect);
			ViewInfo.UpdateBeforePaint(hi.HitCard);
			ViewInfo.UpdateAppearanceFieldEditingValue(hi.HitField, editorInfo, hi.RowHandle, hi.Column, hi.HitCard.State);
			hi.HitField.PerformUpdateViewInfo();
			hi.HitField.Update();
			Size clip = fieldInfo.RepositoryItemViewInfo.Bounds.Size;
			editorBounds.Size = new Size(
					Math.Min(clip.Width, editorBounds.Width),
					Math.Min(clip.Height, editorBounds.Height)
				);
			UpdateEditor(fieldInfo.RepositoryItem,
				new UpdateEditorInfoArgs(GetColumnReadOnly(hi.Column), editorBounds, editorInfo.PaintAppearance,
				editorInfo.EditValue,
				ElementsLookAndFeel,
				editorInfo.ErrorIconText,
				editorInfo.ErrorIcon,
				IsRightToLeft));
		}
		protected virtual bool CanActivateEditor(LayoutViewHitInfo hi) {
			return (hi != null) && (hi.Column != null) && hi.Column.OptionsColumn.AllowEdit;
		}
		public override void HideEditor() {
			if(!IsEditing) return;
			base.HideEditor();
			if(editingFieldInfo != null) {
				if(!editingFieldInfo.HitField.IsDisposing) {
					LayoutViewFieldInfo fieldinfo = editingFieldInfo.HitField.ViewInfo as LayoutViewFieldInfo;
					fieldinfo.GridRowCellState = GridRowCellState.Dirty;
					InvalidateRect(fieldinfo.BoundsRelativeToControl);
				}
			}
			editingFieldInfo = null;
			state = LayoutViewState.Normal;
			UpdateNavigator();
		}
		protected internal override bool PostEditor(bool causeValidation) {
			if(ActiveEditor == null || !EditingValueModified || editingFieldInfo == null) return true;
			if(causeValidation && !ValidateEditor()) return false;
			object value = ExtractEditingValue(editingFieldInfo.Column, EditingValue);
			SetRowCellValueCore(FocusedRowHandle, editingFieldInfo.Column, value, true);
			return true;
		}
		protected internal override void CheckInfo() {
			base.CheckInfo();
			if(GridControl == null) return;
			implementorCore.ResetLookAndFeel();
			ScrollInfo.UpdateLookAndFeel(ElementsLookAndFeel);
			implementorCore.LookAndFeel.Assign(ElementsLookAndFeel);
		}
		protected internal override GestureAllowArgs[] CheckAllowGestures(Point point) {
			touchOffset = null;
			prevTouchOffset = 0;
			touchStart = 0;
			if(CanTouchScroll()) {
				var args = new GestureAllowArgs() { GID = GID.PAN, AllowID = GestureHelper.GC_PAN_WITH_GUTTER | GestureHelper.GC_PAN_WITH_INERTIA };
				if(ViewInfo.NavigationHScrollNeed) {
					args.AllowID |= GestureHelper.GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY;
					args.BlockID |= GestureHelper.GC_PAN_WITH_SINGLE_FINGER_VERTICALLY;
					return new GestureAllowArgs[] { args };
				}
				if(ViewInfo.NavigationVScrollNeed) {
					args.AllowID |= GestureHelper.GC_PAN_WITH_SINGLE_FINGER_VERTICALLY;
					args.BlockID |= GestureHelper.GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY;
					return new GestureAllowArgs[] { args };
				}
			}
			return GestureAllowArgs.None;
		}
		protected virtual bool CanTouchScroll() {
			return (ViewInfo != null) && !(ViewInfo.layoutManager is AnimatedLayoutViewMode); 
		}
		int touchStart;
		internal int? touchOffset;
		internal int prevTouchOffset;
		internal int touchMaxOffset;
		protected internal bool IsTouchScroll {
			get { return touchOffset != null; }
		}
		protected internal bool IsActualTouchScroll {
			get { return touchOffset != null && TouchScrollOffset != 0 && TouchScrollOffset != prevTouchOffset; }
		}
		protected internal int TouchScrollOffset {
			get { return touchOffset.GetValueOrDefault(); }
		}
		protected internal Rectangle GetTouchScrollArea() {
			Rectangle cardsRect = ViewInfo.ViewRects.CardsRect;
			bool hScroll = ViewInfo.NavigationHScrollNeed;
			if(hScroll)
				cardsRect.Offset(touchOffset.Value, 0);
			else
				cardsRect.Offset(0, touchOffset.Value);
			cardsRect.Intersect(ViewInfo.ViewRects.CardsRect);
			return cardsRect;
		}
		TouchOffsetAnimator touchScrollAnimator;
		protected internal override void OnTouchScroll(GestureArgs info, Point delta, ref Point overPan) {
			bool hScroll = ViewInfo.NavigationHScrollNeed;
			if(info.IsBegin) {
				OnTouchScrollBegin(hScroll, info.Start.Point);
				return;
			}
			if(info.IsEnd) {
				OnTouchScrollEnd(hScroll);
				return;
			}
			if((hScroll && delta.X != 0) || (!hScroll && delta.Y != 0))
				OnTouchScrollDelta(hScroll, info.Current.Point, delta, ref overPan);
		}
		protected internal void OnTouchScrollBegin(bool hScroll, Point startPoint) {
			if(touchScrollAnimator != null)
				touchScrollAnimator.StopAnimation();
			if(hScroll)
				this.touchStart = startPoint.X;
			else
				this.touchStart = startPoint.Y;
			this.prevTouchOffset = 0;
			this.touchOffset = 0;
		}
		protected internal void OnTouchScrollDelta(bool hScroll, Point currentPoint, Point delta, ref Point overPan) {
			this.prevTouchOffset = TouchScrollOffset;
			this.touchOffset = (hScroll ? currentPoint.X : currentPoint.Y) - touchStart;
			bool touchScrollResult = ViewInfo.CheckTouchScroll(TouchScrollOffset, out touchMaxOffset);
			bool isTouchOverPan = Math.Abs(TouchScrollOffset) > touchMaxOffset;
			if(isTouchOverPan) {
				if(hScroll)
					overPan.X += delta.X;
				else
					overPan.Y += delta.Y;
			}
			else overPan = Point.Empty;
			if(touchScrollResult) {
				if(DoNavigateRecordsByTouch(hScroll, TouchScrollOffset)) {
					this.touchStart = hScroll ? currentPoint.X : currentPoint.Y;
					this.touchOffset = null;
					this.prevTouchOffset = 0;
					GridControl.Update();
					overPan = Point.Empty;
					return;
				}
				else {
					if(isTouchOverPan) {
						if(Math.Abs(prevTouchOffset) > touchMaxOffset)
							delta = Point.Empty;
						else {
							var offset = Math.Sign(TouchScrollOffset) * touchMaxOffset;
							if(hScroll)
								delta.X = offset - prevTouchOffset;
							else
								delta.Y = offset - prevTouchOffset;
						}
					}
					DoScrollView(delta, hScroll);
				}
			}
			else {
				if(!isTouchOverPan)
					DoScrollView(delta, hScroll);
				else overPan = Point.Empty;
			}
		}
		protected internal void OnTouchScrollEnd(bool hScroll) {
			if(touchScrollAnimator == null)
				touchScrollAnimator = new TouchOffsetAnimator(this);
			if(Math.Abs(TouchScrollOffset) < touchMaxOffset) {
				touchScrollAnimator.StartAnimation(TouchScrollOffset, touchMaxOffset, hScroll);
			}
			else {
				this.touchOffset = Math.Sign(TouchScrollOffset) * touchMaxOffset;
				touchScrollAnimator.StartAnimation(TouchScrollOffset, Math.Abs(touchMaxOffset * 2) + 1, hScroll);
			}
		}
		internal void DoScrollView(Point delta, bool hScroll) {
			Rectangle cardsRect = ViewInfo.ViewRects.CardsRect;
			if(hScroll) {
				if(delta.X != 0) {
					int x = (delta.X < 0) ? cardsRect.X - delta.X : cardsRect.X;
					Utils.Drawing.Helpers.WindowScroller.ScrollHorizontal(GridControl, cardsRect, x, delta.X);
				}
			}
			else {
				if(delta.Y != 0) {
					int y = (delta.Y < 0) ? cardsRect.Y - delta.Y : cardsRect.Y;
					Utils.Drawing.Helpers.WindowScroller.ScrollVertical(GridControl, cardsRect, y, delta.Y);
				}
			}
		}
		internal bool DoNavigateRecordsByTouch(bool hScroll, int touchOffset) {
			var prevPos = GetScrollPos(hScroll);
			var change = ViewInfo.CalcTouchScrollChange(touchOffset);
			int nextPos;
			if(touchOffset < 0)
				nextPos = ViewInfo.GetLastCardRowHandle() + change;
			else
				nextPos = ViewInfo.GetFirstCardRowHandle() - change;
			if(IsValidRowHandle(nextPos))
				DoNavigateRecords(nextPos, false);
			return GetScrollPos(hScroll) != prevPos;
		}
		int GetScrollPos(bool hScroll) {
			return hScroll ? ScrollInfo.HScrollPosition : ScrollInfo.VScrollPosition;
		}
		protected internal override BaseViewOfficeScroller CreateScroller() {
			return new LayoutViewScroller(this);
		}
		protected virtual ScrollInfo CreateScrollInfo() {
			return new LayoutViewScrollInfo(this);
		}
		protected internal ScrollInfo ScrollInfo {
			get { return scrollInfo; }
		}
		protected override void UpdateScrollBars() {
			if(ViewInfo != null && ScrollInfo != null) {
				int iHScrollHeight = ScrollInfo.HScrollRect.Height;
				int iVScrollWidth = ScrollInfo.VScrollRect.Width;
				if(ViewInfo.ClientBounds.Height < iHScrollHeight) {
					ScrollInfo.HScroll.Visible = false;
					iHScrollHeight = 0;
				}
				if(ViewInfo.ClientBounds.Width < iVScrollWidth) {
					ScrollInfo.VScroll.Visible = false;
					iVScrollWidth = 0;
				}
				UpdateScrollRanges();
				if(iHScrollHeight != ScrollInfo.HScrollRect.Height || iVScrollWidth != ScrollInfo.VScrollRect.Width)
					ViewInfo.NeedArrangeForce = true;
				if(!ViewInfo.IsRealHeightCalculate) {
					var scrollBounds = ViewInfo.CalcScrollRect();
					ScrollInfo.ClientRect = scrollBounds;
					Rectangle vscrollBounds = new Rectangle(scrollBounds.Right - ScrollInfo.VScrollSize, scrollBounds.Y, ScrollInfo.VScrollSize, scrollBounds.Height);
					if(IsRightToLeft) 
						vscrollBounds.X = scrollBounds.X;
					if(!ScrollInfo.IsOverlapHScrollBar) 
						vscrollBounds.Height = scrollBounds.Bottom - vscrollBounds.Y - (ScrollInfo.HScrollVisible ? ScrollInfo.HScrollSize : 0);
					ScrollInfo.VScrollSuggestedBounds = vscrollBounds;
					ScrollInfo.UpdateScrollRects();
				}
			}
		}
		Rectangle beforeHScrollRect;
		Rectangle beforeVScrollRect;
		internal bool fInternalNeedRecalcView;
		internal bool fAutoPanByScroll;
		internal bool fAutoPanForward, fAutoPanBackward;
		protected internal void UpdateScrollBeforeArrange() {
			fInternalNeedRecalcView = false;
			fAutoPanByScroll = false;
			fAutoPanBackward = fAutoPanForward = false;
			if(ScrollInfo != null) {
				beforeHScrollRect = ScrollInfo.HScrollRect;
				beforeVScrollRect = ScrollInfo.VScrollRect;
			}
		}
		protected internal void UpdateScrollAfterArrange() {
			UpdateScrollRanges();
			if(ScrollInfo != null) {
				fInternalNeedRecalcView = (beforeHScrollRect != ScrollInfo.HScrollRect) || (beforeVScrollRect != ScrollInfo.VScrollRect);
			}
			fAutoPanByScroll = false;
			fAutoPanBackward = fAutoPanForward = false;
		}
		protected virtual void UpdateScrollRanges() {
			if(ScrollInfo != null) {
				UpdateVScrollRange();
				UpdateHScrollRange();
			}
		}
		protected bool CanShowScroll() {
			return OptionsBehavior.ScrollVisibility != ScrollVisibility.Never;
		}
		protected bool CanAutoHideScroll() {
			return OptionsBehavior.ScrollVisibility == ScrollVisibility.Auto;
		}
		int fAutoPanByFocusedCardLocked;
		protected internal void LockAutoPanByFocusedCard() {
			fAutoPanByFocusedCardLocked++;
		}
		protected internal void UnLockAutoPanByFocusedCard() {
			fAutoPanByFocusedCardLocked--;
		}
		protected internal bool IsAutoPanByFocusedCardLocked {
			get { return fAutoPanByFocusedCardLocked > 0; }
		}
		int fAutoPanByScrollLocked;
		protected internal void LockAutoPanByScroll() {
			fAutoPanByScrollLocked++;
		}
		protected internal void UnLockAutoPanByScroll() {
			fAutoPanByScrollLocked--;
		}
		protected internal bool IsAutoPanByScrollLocked {
			get { return fAutoPanByScrollLocked > 0; }
		}
		int fAutoPanByPartialCardLocked;
		protected internal void LockAutoPanByPartialCard() {
			fAutoPanByPartialCardLocked++;
		}
		protected internal void UnLockAutoPanByPartialCard() {
			fAutoPanByPartialCardLocked--;
		}
		protected internal bool IsAutoPanByPartialCardLocked {
			get { return fAutoPanByPartialCardLocked > 0; }
		}
		protected internal bool PartialCardsSimpleScrolling {
			get {
				switch(OptionsView.ViewMode) {
					case LayoutViewMode.Row:
					case LayoutViewMode.Column:
						return OptionsView.PartialCardsSimpleScrolling != DefaultBoolean.False;
					case LayoutViewMode.MultiRow:
					case LayoutViewMode.MultiColumn:
						return OptionsView.PartialCardsSimpleScrolling == DefaultBoolean.True;
					default:
						return false;
				}
			}
		}
		protected virtual void UpdateHScrollRange() {
			internalHScrollValueChange++;
			try {
				bool fNeedScroll = CanShowScroll() && ViewInfo.NavigationHScrollNeed;
				bool fMayBeAutoHideScroll = CanAutoHideScroll() && !GridControl.UseEmbeddedNavigator;
				if(fNeedScroll) {
					ScrollArgs args = new ScrollArgs();
					int iScrollableCardsCount = ViewInfo.CalcScrollableCardsCount();
					int smallChange = Math.Max(ViewInfo.CalcHSmallChange(IsHScrollForward()), 1);
					int largeChange = Math.Max(smallChange, iScrollableCardsCount);
					args.FireEventsOnAssign = false;
					args.Minimum = 0;
					args.Maximum = Math.Max(RecordCount > 0 ? RecordCount - 1 : 0, 0);
					args.Enabled = true;
					args.LargeChange = largeChange;
					args.SmallChange = smallChange;
					args.Value = VisibleRecordIndex;
					if(iScrollableCardsCount > 0) {
						int firstIndex = GetVisibleIndex(ViewInfo.GetFirstCardRowHandle());
						int lastIndex = GetVisibleIndex(ViewInfo.GetLastCardRowHandle());
						bool snapToLast = (lastIndex == RecordCount - 1) && (firstIndex != 0);
						bool allInView = (firstIndex == 0) && (lastIndex == RecordCount - 1);
						if(allInView && iScrollableCardsCount < RecordCount) {
							fAutoPanForward = IsHScrollForward();
							fAutoPanBackward = prevHScrollIndex > ScrollInfo.HScrollPosition;
							if(prevHScrollIndex == ScrollInfo.HScrollPosition && internalHScrollValueChange == 2) {
								fAutoPanForward = (prevHScrollIndex == smallChange);
								fAutoPanBackward = (prevHScrollIndex == 0);
							}
							fAutoPanByScroll = fAutoPanForward || fAutoPanBackward;
							CheckScrollByFocusedRowChanged(args, firstIndex, lastIndex, smallChange);
						}
						else args.Value = firstIndex;
						if(!PartialCardsSimpleScrolling) {
							if(snapToLast) args.LargeChange = ViewInfo.VisibleCards.Count;
						}
					}
					args.Value = Math.Max(Math.Min(args.Value, args.Maximum), 0);
					if(args.LargeChange > args.Maximum && args.Value == 0) args.Maximum = 0;
					if(args.Maximum == 0) args.Enabled = false;
					args.Value = Math.Max(args.Value, 0);
					fMayBeAutoHideScroll &= !args.Enabled;
					ScrollInfo.HScrollArgs = args;
				}
				else {
					if(GridControl != null && GridControl.UseEmbeddedNavigator) {
						if(!ScrollInfo.HScrollVisible) ScrollInfo.HScrollVisible = true;
						ScrollArgs args = new ScrollArgs();
						args.Minimum = args.Maximum = args.Maximum = 0;
						args.Enabled = false;
						ScrollInfo.HScrollArgs = args;
						return;
					}
				}
				bool fScrollVisible = fNeedScroll && !fMayBeAutoHideScroll;
				if(ScrollInfo.HScrollVisible != fScrollVisible) ScrollInfo.HScrollVisible = fScrollVisible;
			}
			finally { internalHScrollValueChange--; }
		}
		internal int MouseWheelDelta = 0;
		protected virtual void UpdateVScrollRange() {
			internalVScrollValueChange++;
			try {
				bool fNeedScroll = CanShowScroll() && ViewInfo.NavigationVScrollNeed;
				bool fMayBeAutoHideScroll = CanAutoHideScroll();
				if(fNeedScroll) {
					ScrollArgs args = new ScrollArgs();
					int iScrollableCardsCount = ViewInfo.CalcScrollableCardsCount();
					int smallChange = Math.Max(ViewInfo.CalcVSmallChange(IsVScrollForward()), 1);
					int largeChange = Math.Max(smallChange, iScrollableCardsCount);
					args.FireEventsOnAssign = false;
					args.Minimum = 0;
					args.Maximum = Math.Max(RecordCount > 0 ? RecordCount - 1 : 0, 0);
					args.Enabled = true;
					args.LargeChange = largeChange;
					args.SmallChange = smallChange;
					args.Value = VisibleRecordIndex;
					if(iScrollableCardsCount > 0) {
						int firstIndex = GetVisibleIndex(ViewInfo.GetFirstCardRowHandle());
						int lastIndex = GetVisibleIndex(ViewInfo.GetLastCardRowHandle());
						bool snapToLast = (lastIndex == RecordCount - 1 && firstIndex != 0);
						bool allInView = (firstIndex == 0) && (lastIndex == RecordCount - 1);
						if(allInView && iScrollableCardsCount < RecordCount) {
							fAutoPanForward = IsVScrollForward();
							fAutoPanBackward = prevVScrollIndex > ScrollInfo.VScrollPosition;
							if(prevVScrollIndex == ScrollInfo.VScrollPosition && internalVScrollValueChange == 2) {
								fAutoPanForward = (prevVScrollIndex == 1);
								fAutoPanBackward = (prevVScrollIndex == 0);
							}
							fAutoPanByScroll = fAutoPanForward || fAutoPanBackward;
							CheckScrollByFocusedRowChanged(args, firstIndex, lastIndex, smallChange);
						}
						else args.Value = firstIndex;
						if(!PartialCardsSimpleScrolling) {
							if(snapToLast) args.LargeChange = ViewInfo.VisibleCards.Count;
						}
					}
					args.Value = Math.Max(Math.Min(args.Value, args.Maximum), 0);
					if(args.LargeChange > args.Maximum && args.Value == 0) args.Maximum = 0;
					if(args.Maximum == 0) args.Enabled = false;
					args.Value = Math.Max(args.Value, 0);
					fMayBeAutoHideScroll &= !args.Enabled;
					ScrollInfo.VScrollArgs = args;
				}
				bool fScrollVisible = fNeedScroll && !fMayBeAutoHideScroll;
				if(ScrollInfo.VScrollVisible != fScrollVisible) ScrollInfo.VScrollVisible = fScrollVisible;
			}
			finally { internalVScrollValueChange--; }
		}
		bool IsVScrollForward() {
			return (MouseWheelDelta == 0) ? (prevVScrollIndex < ScrollInfo.VScrollPosition) : MouseWheelDelta < 0;
		}
		bool IsHScrollForward() {
			return (MouseWheelDelta == 0) ? (prevHScrollIndex < ScrollInfo.HScrollPosition) : MouseWheelDelta < 0;
		}
		void CheckScrollByFocusedRowChanged(ScrollArgs args, int firstIndex, int lastIndex, int smallChange) {
			if(IsFocusedRowChangeLocked) {
				if(firstIndex == FocusedRowHandle) {
					args.Value = firstIndex;
					if(ViewInfo.VisibleCards[firstIndex].IsPartiallyVisible) {
						args.Value = firstIndex + smallChange;
					}
				}
				if(lastIndex == FocusedRowHandle) {
					args.Value = firstIndex + smallChange;
					if(ViewInfo.VisibleCards[lastIndex].IsPartiallyVisible) {
						args.Value = firstIndex;
					}
				}
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
		internal void UpdateScrollState() { UpdateScrollBars(); }
		public override void InvalidateHitObject(BaseHitInfo hitInfo) {
			if(!hitInfo.IsValid) return;
			LayoutViewHitInfo layoutViewHitInfo = hitInfo as LayoutViewHitInfo;
			if(layoutViewHitInfo.InField && !layoutViewHitInfo.HitField.IsDisposing) {
				Rectangle fieldRect = layoutViewHitInfo.HitField.ViewInfo.BoundsRelativeToControl;
				fieldRect.Inflate(2, 2);
				InvalidateRect(fieldRect);
			}
			else InvalidateRect(layoutViewHitInfo.HitRect);
		}
		protected internal virtual bool CanSortLayoutViewColumn(GridColumn column) {
			return OptionsCustomization.AllowSort && column.OptionsColumn.AllowSort != DefaultBoolean.False;
		}
		protected internal virtual bool CanFilterLayoutViewColumn(GridColumn column) {
			return OptionsCustomization.AllowFilter && IsColumnAllowFilter(column);
		}
		protected override void DoInternalLayout() {
			base.DoInternalLayout();
			ViewInfo.UpdateAnimatedItems();
		}
		protected override internal void DoMouseSortColumn(GridColumn column, Keys key) {
			if(!CanSortLayoutViewColumn(column)) return;
			base.DoMouseSortColumn(column, key);
			ResetCardsCache();
			Refresh();
		}
		protected internal override void DoMoveFocusedRow(int delta, KeyEventArgs e) {
			int prevFocusedHandle = FocusedRowHandle;
			try {
				base.DoMoveFocusedRow(delta, e);
				int iCurrentRow = FocusedRowHandle + delta;
				if(iCurrentRow + 1 > RecordCount) {
					iCurrentRow = Math.Max(0, RecordCount - 1);
				}
				else if(iCurrentRow < 0) {
					iCurrentRow = 0;
				}
				if(FocusedRowHandle != iCurrentRow)
					DoNavigateRecords(iCurrentRow);
			}
			finally {
				DoAfterMoveFocusedRow(e, prevFocusedHandle, null, null);
			}
		}
		protected internal virtual void DoMoveFocusedColumn(int delta, Keys byKey) {
			DoMoveFocusedItem(delta, byKey);
		}
		string focusedItemNameCore = null;
		protected internal virtual string FocusedItemName {
			get { return focusedItemNameCore; }
			set { focusedItemNameCore = value; }
		}
		Rectangle cardSelectionRectCore;
		protected internal virtual Rectangle CardSelectionRect {
			get { return cardSelectionRectCore; }
			set {
				if(CardSelectionRect == value) return;
				Rectangle oldRect = CardSelectionRect;
				cardSelectionRectCore = value;
				InvalidateSelectionRect(oldRect, value);
			}
		}
		protected void InvalidateSelectionRect(Rectangle oldRect, Rectangle newRect) {
			Rectangle realRect = new Rectangle(
					Math.Min(oldRect.Left, newRect.Left),
					Math.Min(oldRect.Top, newRect.Top),
					Math.Max(oldRect.Right, newRect.Right),
					Math.Max(oldRect.Bottom, newRect.Bottom)
				);
			InvalidateRect(realRect);
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("LayoutViewFocusedColumn")]
#endif
		public override GridColumn FocusedColumn {
			get { return base.FocusedColumn; }
			set {
				base.FocusedColumn = value;
				LayoutViewColumn column = value as LayoutViewColumn;
				if(column != null && column.IsFieldCreated) {
					FocusedItemName = column.LayoutViewField.Name;
				}
				else FocusedItemName = null;
			}
		}
		protected void AutoPanFocusedField(LayoutViewField field) {
			LayoutViewBaseMode mode = ViewInfo.layoutManager as LayoutViewBaseMode;
			if(mode != null && mode.AutoPanFieldCore(field)) Invalidate();
		}
		protected internal bool CanFocusColumn() {
			LayoutViewCard focusedCard = FindFocusedCard();
			if(focusedCard != null) {
				return ArrangeItemNamesByFocus(focusedCard).Count > 0;
			}
			return true;
		}
		protected internal virtual void DoMoveFocusedItem(int delta, Keys byKey) {
			bool fCardCollapced = GetCardCollapsed(FocusedRowHandle);
			if(fCardCollapced) {
				FocusedItemName = null;
				DoMoveFocusedRow(delta, new KeyEventArgs(byKey));
				return;
			}
			LayoutViewCard focusedCard = FindFocusedCard();
			if(focusedCard == null) {
				FocusedItemName = null;
				DoMoveFocusedRow(delta, new KeyEventArgs(byKey));
				return;
			}
			List<string> itemsByFocus = ArrangeItemNamesByFocus(focusedCard);
			if(!string.IsNullOrEmpty(FocusedItemName)) {
				int iItemsCount = itemsByFocus.Count;
				int iNewFocusedItemIndex = itemsByFocus.IndexOf(FocusedItemName) + delta;
				int iOldFocusedItemIndex = itemsByFocus.IndexOf(FocusedItemName);
				if(iNewFocusedItemIndex >= iItemsCount) {
					iNewFocusedItemIndex = Math.Max(iItemsCount - 1, 0);
					if(FocusedRowHandle != GridControl.NewItemRowHandle && (FocusedRowHandle + 1 < RecordCount)) {
						iNewFocusedItemIndex = 0;
						DoNavigateRecords(FocusedRowHandle + 1);
					}
					else iNewFocusedItemIndex = iOldFocusedItemIndex;
				}
				if(iNewFocusedItemIndex < 0) {
					iNewFocusedItemIndex = 0;
					if(FocusedRowHandle > 0) {
						iNewFocusedItemIndex = Math.Max(iItemsCount - 1, 0);
						DoNavigateRecords(FocusedRowHandle - 1);
					}
					else DoNavigateRecords(GetVisibleIndex(FocusedRowHandle) - 1);
				}
				focusedCard = FindCardByRow(FocusedRowHandle);
				FocusItemInCard(focusedCard, itemsByFocus, iNewFocusedItemIndex);
			}
			else {
				focusedCard = FindCardByRow(FocusedRowHandle);
				if(!fCardCollapced) FocusFirstItemInCard(focusedCard, itemsByFocus);
			}
		}
		protected internal bool ChangeParentTabByCtrlTabKey(bool invertDirection) {
			bool result = false;
			if(!string.IsNullOrEmpty(FocusedItemName)) {
				LayoutViewCard card = FindCardByRow(FocusedRowHandle);
				if(card == null) return false;
				BaseLayoutItem focusedItem = FindItemByName(card, FocusedItemName);
				if(focusedItem == null) return false;
				TabbedControlGroup tg = FindParentTabbedGroup(focusedItem);
				if(tg == null) return false;
				FocusedItemName = tg.Name;
				int iPagesCount = tg.TabPages.Count;
				if(iPagesCount > 0) {
					int iTab = tg.SelectedTabPageIndex;
					if(invertDirection) iTab--;
					else iTab++;
					if(iTab >= iPagesCount) iTab = 0;
					if(iTab < 0) iTab = iPagesCount - 1;
					tg.SelectedTabPageIndex = iTab;
					result = true;
				}
			}
			return result;
		}
		protected TabbedControlGroup FindParentTabbedGroup(BaseLayoutItem childItem) {
			TabbedControlGroup result = null;
			if(childItem == null) return null;
			BaseLayoutItem item = childItem;
			while(item != null) {
				if(item is TabbedControlGroup) break;
				if(item is LayoutGroup && ((LayoutGroup)item).ParentTabbedGroup != null) break;
				item = item.Parent;
			}
			if(item != null) {
				if(item.IsGroup) result = (item as LayoutGroup).ParentTabbedGroup as TabbedControlGroup;
				else result = item as TabbedControlGroup;
			}
			return result;
		}
		protected internal bool ChangeTabByKey(Keys byKey) {
			bool result = false;
			if(!string.IsNullOrEmpty(FocusedItemName)) {
				LayoutViewCard card = FindCardByRow(FocusedRowHandle);
				if(card == null) return false;
				TabbedControlGroup tg = FindItemByName(card, FocusedItemName) as TabbedControlGroup;
				if(tg == null) return false;
				int iPagesCount = tg.TabPages.Count;
				if(iPagesCount > 0) {
					int iTab = tg.SelectedTabPageIndex;
					if(byKey == Keys.Left) iTab--;
					if(byKey == Keys.Right) iTab++;
					if(iTab >= iPagesCount) iTab = 0;
					if(iTab < 0) iTab = iPagesCount - 1;
					tg.SelectedTabPageIndex = iTab;
					CheckCardDifferences(card);
					result = true;
				}
			}
			return result;
		}
		protected virtual List<string> ArrangeItemNamesByFocus(LayoutGroup group) {
			return ArrangeItemNamesByFocus(group, true);
		}
		protected virtual List<string> ArrangeItemNamesByFocus(LayoutGroup group, bool checkAllowFocus) {
			if(group == null) return null;
			LayoutControlWalker walker = new LayoutControlWalker(group);
			ArrayList list = walker.ArrangeElements(ILayoutControl.OptionsFocus);
			List<string> result = new List<string>();
			foreach(BaseLayoutItem item in list) {
				LayoutClassificationArgs arg = LayoutClassifier.Default.Classify(item);
				if(arg.IsRepositoryItem) {
					LayoutViewField field = arg.RepositoryItem as LayoutViewField;
					if(field != null && field.Visible) {
						if(checkAllowFocus && !CanFocusColumn(field.Column)) continue;
						result.Add(field.Name);
					}
				}
				if(arg.IsGroup) {
					result.AddRange(ArrangeItemNamesByFocus(arg.Group));
				}
				if(arg.IsTabbedGroup) {
					result.Add(item.Name);
					result.AddRange(ArrangeItemNamesByFocus(arg.TabbedGroup.SelectedTabPage));
				}
			}
			return result;
		}
		protected virtual BaseLayoutItem GetItemToFocusFromList(LayoutViewCard card, List<string> itemNamesArrangedByFocus, int index) {
			BaseLayoutItem firstFocusedItem = null;
			int iItemsCount = itemNamesArrangedByFocus.Count;
			if(card != null && iItemsCount > 0 && index >= 0 && index < iItemsCount) {
				firstFocusedItem = FindItemByName(card, itemNamesArrangedByFocus[index]);
			}
			return firstFocusedItem;
		}
		protected virtual List<string> ArrangeFieldNamesByFocus(LayoutGroup group) {
			return ArrangeFieldNamesByFocus(group, true);
		}
		protected virtual List<string> ArrangeFieldNamesByFocus(LayoutGroup group, bool checkAllowFocus) {
			if(group == null) return null;
			LayoutControlWalker walker = new LayoutControlWalker(group);
			ArrayList list = walker.ArrangeElements(ILayoutControl.OptionsFocus);
			List<string> result = new List<string>();
			foreach(BaseLayoutItem item in list) {
				LayoutClassificationArgs arg = LayoutClassifier.Default.Classify(item);
				if(arg.IsRepositoryItem) {
					LayoutViewField field = arg.RepositoryItem as LayoutViewField;
					if(field != null && field.Visible) {
						if(checkAllowFocus && !CanFocusColumn(field.Column)) continue;
						result.Add(field.FieldName);
					}
				}
				if(arg.IsGroup) {
					result.AddRange(ArrangeFieldNamesByFocus(arg.Group));
				}
				if(arg.IsTabbedGroup) {
					result.AddRange(ArrangeFieldNamesByFocus(arg.TabbedGroup.SelectedTabPage));
				}
			}
			return result;
		}
		protected virtual List<LayoutViewColumn> ArrangeColumnsByFocus(LayoutViewCard card, bool checkAllowFocus) {
			return ArrangeColumnsByFocusCore(card, checkAllowFocus);
		}
		protected virtual List<LayoutViewColumn> ArrangeColumnsByFocusCore(LayoutGroup group, bool checkAllowFocus) {
			if(group == null) return null;
			LayoutControlWalker walker = new LayoutControlWalker(group);
			ArrayList list = walker.ArrangeElements(ILayoutControl.OptionsFocus);
			List<LayoutViewColumn> result = new List<LayoutViewColumn>();
			foreach(BaseLayoutItem item in list) {
				LayoutClassificationArgs arg = LayoutClassifier.Default.Classify(item);
				if(arg.IsRepositoryItem) {
					LayoutViewField field = arg.RepositoryItem as LayoutViewField;
					if(field != null) {
						if(checkAllowFocus && !CanFocusColumn(field.Column)) continue;
						result.Add(field.Column);
					}
				}
				if(arg.IsGroup) result.AddRange(ArrangeColumnsByFocusCore(arg.Group, checkAllowFocus));
			}
			return result;
		}
		protected virtual LayoutViewCard FindFocusedCard() {
			if(FocusedRowHandle != DevExpress.Data.DataController.InvalidRow) {
				return FindCardByRow(FocusedRowHandle);
			}
			else {
				if(ViewInfo.VisibleCards.Count > 0) {
					LayoutViewCard focusedCard = ViewInfo.VisibleCards[0];
					List<string> itemsByFocus = ArrangeItemNamesByFocus(focusedCard);
					FocusedRowHandle = focusedCard.RowHandle;
					FocusFirstItemInCard(focusedCard, itemsByFocus);
				}
				return null;
			}
		}
		protected virtual void FocusItemInCard(LayoutViewCard focusedCard, List<string> itemsByFocus, int index) {
			BaseLayoutItem item = GetItemToFocusFromList(focusedCard, itemsByFocus, index);
			if(item is LayoutViewField) {
				base.FocusedColumn = (item as LayoutViewField).Column;
				FocusedItemName = item.Name;
				AutoPanFocusedField(item as LayoutViewField);
			}
			else {
				base.FocusedColumn = null;
				if(item != null) FocusedItemName = item.Name;
				else FocusedItemName = null;
			}
		}
		protected virtual void FocusFirstItemInCard(LayoutViewCard focusedCard, List<string> itemsByFocus) {
			FocusItemInCard(focusedCard, itemsByFocus, 0);
			InvalidateCard(focusedCard);
		}
		protected internal virtual LayoutViewColumn TryFocusCardColumn(LayoutViewCard card, LayoutViewColumn newFocusedColumn) {
			if(newFocusedColumn == null) return null;
			if(CanFocusColumn(newFocusedColumn)) return newFocusedColumn;
			List<LayoutViewColumn> list = ArrangeColumnsByFocus(card, false);
			LayoutViewColumn focusedColumn = null;
			LayoutViewColumn prevAllowedColumn = null;
			bool fTargetColumnFinded = false;
			foreach(LayoutViewColumn col in list) {
				bool canFocusColumn = CanFocusColumn(col);
				if(!fTargetColumnFinded) {
					if(canFocusColumn) prevAllowedColumn = col;
					if(col.FieldName == newFocusedColumn.FieldName) { fTargetColumnFinded = true; continue; }
				}
				else {
					if(canFocusColumn) { focusedColumn = col; break; }
				}
			}
			if(focusedColumn == null) focusedColumn = prevAllowedColumn;
			return focusedColumn;
		}
		protected virtual bool CanFocusColumn(LayoutViewColumn column) {
			return column.OptionsColumn.AllowFocus;
		}
		protected virtual bool CanFocusField(LayoutViewField field) {
			return CanFocusColumn(field.Column);
		}
		protected void CheckTemplateCard() {
			if(templateCard == null) {
				InitializeTemplateCardDefault();
			}
		}
		protected void ResetTemplateCardBeforeDeserialize() {
			TemplateCard.Accept(new ItemToDefaultState());
		}
		#region IXtraSerializable Members
		protected internal BaseLayoutItem FindItemByName(LayoutViewCard card, string name) {
			ItemNameFinder finder = new ItemNameFinder(name);
			card.Accept(finder);
			return finder.Result;
		}
		protected LayoutViewField FindFieldByColumn(LayoutViewColumn column, ICollection sourceCollection) {
			foreach(BaseLayoutItem item in sourceCollection) {
				LayoutViewField field = item as LayoutViewField;
				if(field != null && field.ColumnName == column.Name) {
					return field;
				}
			}
			return null;
		}
		protected virtual void SynchronizeColumnsAndItems() {
			if(Columns == null || Items == null) return;
			ArrayList listColumns = new ArrayList(Columns);
			ArrayList listItems = new ArrayList(Items);
			bool isDeserializingFromOtherView = (listItems.Count == 0) && (listColumns.Count > 0) && IsDeserializing;
			if(isDeserializingFromOtherView)
				implementorCore.ItemsAndNames.Clear();
			foreach(LayoutViewColumn column in listColumns) {
				LayoutViewField lvField = FindFieldByColumn(column, listItems);
				if(lvField != null) column.AssignLayoutViewField(lvField);
				else {
					if(IsDeserializing && !isDeserializingFromOtherView && (implementorCore.ItemsAndNames != null))
						AddColumnToItemsAndNames(column);
					else
						AddColumnToLayout(column);
				}
			}
			TemplateCard.Accept(implementorCore.RemoveComponentHelper);
			TemplateCard.Accept(implementorCore.AddComponentHelper);
		}
		void SetColumnForGroup(LayoutGroupGenerate groupGenerate, int ColumnCount) {
			if(ColumnCount == 0) ColumnCount = 1;
			groupGenerate.SetChildrenVisible(true);
			int width = groupGenerate.CellSize.Width * 6;
			groupGenerate.Width = groupGenerate.CellSize.Width * ColumnCount * 6 + 50;
			for(int i = 0; i < groupGenerate.Items.Count; i++) {
				if(groupGenerate[i].StartNewLine)
					groupGenerate[i].Size = new Size(groupGenerate.Width, groupGenerate[i].Height);
				else
					groupGenerate[i].Size = new Size(width, groupGenerate[i].Height);
			}
			try {
				groupGenerate.UpdateFlowItems();
				groupGenerate.LayoutMode = XtraLayout.Utils.LayoutMode.Regular;
			}
			catch { }
		}
		void AddColumnToItemsAndNames(LayoutViewColumn column) {
			LayoutViewField lvField = null;
			if(column.IsFieldCreated) {
				lvField = column.LayoutViewField;
				string itemName = lvField.Name;
				if(!implementorCore.ItemsAndNames.Remove(itemName)) {
					foreach(var entry in implementorCore.ItemsAndNames) {
						if(entry.Value != lvField) continue;
						itemName = entry.Key;
						break;
					}
					implementorCore.ItemsAndNames.Remove(itemName);
				}
			}
			lvField = InitializeFieldByColumn(column, -1);
			implementorCore.ItemsAndNames.Add(lvField.Name, lvField);
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			base.OnStartDeserializing(e);
			if(e.Allow) {
				CheckTemplateCard();
				ResetTemplateCardBeforeDeserialize();
			}
			implementorCore.OnStartDeserializing(e);
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			SynchronizeColumnsAndItems();
			implementorCore.OnEndDeserializing(restoredVersion);
			CheckFieldsWithoutColumns();
			ResetCardsCache();
			base.OnEndDeserializing(restoredVersion);
		}
		const string restoreFailedMsg = @"An attempt has been made to restore a field without the corresponding bound column: {0}. The LayoutView doesn't support this scenario. Set the LayoutView.OptionsBehavior.OverrideLayoutControlRestoreBehavior  and LayoutView.OptionsLayout.Columns.RemoveOldColumns properties to True.";
		void CheckFieldsWithoutColumns() {
			if(IsCustomizationRestoringInProgress) return;
			ArrayList listItems = new ArrayList(Items);
			foreach(BaseLayoutItem item in listItems) {
				LayoutViewField field = item as LayoutViewField;
				if(field != null && field.Column == null) {
					throw new NotSupportedException(string.Format(restoreFailedMsg, item.Name));
				}
			}
		}
		bool IsCustomizationRestoringInProgress {
			get {
				object value = AttachedProperty.GetValue(GridControl, LayoutView.IsCustomizationRestoringInProgressProperty);
				return (value != null) ? (bool)value : false;
			}
		}
		bool fIsSerializing = false;
		void IXtraSerializable.OnStartSerializing() {
			fIsSerializing = true;
			CheckTemplateCard();
			implementorCore.OnStartSerializing();
		}
		void IXtraSerializable.OnEndSerializing() {
			implementorCore.OnEndSerializing();
			fIsSerializing = false;
		}
		bool ILayoutControl.ShowKeyboardCues { get { return true; } }
		protected override void SaveLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			implementorCore.SaveLayoutCore(serializer, path, options);
		}
		protected override void RestoreLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			implementorCore.RestoreLayoutCore(serializer, path, options);
		}
		internal void XtraClearItems(XtraItemEventArgs e) {
			implementorCore.XtraClearItems(e);
		}
		internal object XtraFindItemsItem(XtraItemEventArgs e) {
			return implementorCore.XtraFindItemsItem(e);
		}
		internal object XtraCreateItemsItem(XtraItemEventArgs e) {
			return implementorCore.XtraCreateItemsItem(e);
		}
		#endregion
		#region IDataController_XXX_Support
		IBoundControl IDataControllerValidationSupport.BoundControl { get { return GridControl; } }
		void IDataControllerValidationSupport.OnBeginCurrentRowEdit() { }
		protected bool NeedResetCacheOnControllerItemChanged(ListChangedEventArgs e) {
			bool fNeedReset = false;
			switch(e.ListChangedType) {
				case ListChangedType.ItemAdded:
				case ListChangedType.ItemDeleted:
				case ListChangedType.Reset:
				case ListChangedType.PropertyDescriptorAdded:
				case ListChangedType.PropertyDescriptorDeleted:
				case ListChangedType.PropertyDescriptorChanged:
					fNeedReset = true;
					break;
			}
			return fNeedReset;
		}
		void IDataControllerValidationSupport.OnControllerItemChanged(ListChangedEventArgs e) {
			this.BeginUpdate();
			bool neeedEndUpdate = false;
			try {
				if(OptionsBehavior.AutoFocusNewCard && e.ListChangedType == ListChangedType.ItemAdded) {
					if(IsValidRowHandle(e.NewIndex)) FocusedRowHandle = e.NewIndex;
					CorrectVisibleRecordIndex();
					neeedEndUpdate = true;
				}
				if(NeedResetCacheOnControllerItemChanged(e)) {
					ClearPrevSelectionInfo();
					ClearColumnErrors();
					DataController.SyncCurrentRow();
					ResetCardsCache();
					neeedEndUpdate = true;
				}
				if(e.ListChangedType == ListChangedType.ItemDeleted) {
					CorrectVisibleRecordIndex(e);
				}
				if(e.ListChangedType == ListChangedType.ItemChanged) {
					InvalidateCard(e.NewIndex);
				}
			}
			finally {
				if(neeedEndUpdate) this.EndUpdate();
				else this.CancelUpdate();
			}
		}
		protected void CorrectVisibleRecordIndex(ListChangedEventArgs ea) {
			visibleCardIndex = ea.NewIndex;
			CorrectVisibleRecordIndex();
		}
		protected void CorrectVisibleRecordIndex() {
			if(FocusedRowHandle == RecordCount - 1) visibleCardIndex = RecordCount - 1;
		}
		void IDataControllerValidationSupport.OnCurrentRowUpdated(ControllerRowEventArgs e) {
			RaiseRowUpdated(new RowObjectEventArgs(e.RowHandle, e.Row));
		}
		bool internalNewRowEditing = false;
		void IDataControllerValidationSupport.OnStartNewItemRow() {
			if(DesignMode) return;
			this.BeginUpdate();
			this.internalNewRowEditing = true;
			internalArrangeFromLeftToRight = false;
			try {
				if(FocusedRowHandle != CurrencyDataController.NewItemRow)
					DoChangeFocusedRowInternal(DevExpress.Data.CurrencyDataController.NewItemRow, false);
				if(!IsCardVisible(FocusedRowHandle))
					MakeRowVisibleCore(FocusedRowHandle, true);
				if(FocusedRowHandle == CurrencyDataController.NewItemRow)
					DataController.BeginCurrentRowEdit();
			}
			finally {
				this.EndUpdate();
				ResetArrangeDirection();
			}
			RaiseInitNewRow(new InitNewRowEventArgs(CurrencyDataController.NewItemRow));
		}
		void IDataControllerValidationSupport.OnEndNewItemRow() {
			this.internalNewRowEditing = false;
			internalArrangeFromLeftToRight = false;
			this.BeginUpdate();
			try {
				LayoutViewCard card = FindCardByRow(CurrencyDataController.NewItemRow);
				if(card != null) {
					card.RowHandle = GetVisibleRowHandle(RecordCount - 1);
					InvalidateCard(card);
				}
				ResetCardsCache();
				if((FocusedRowHandle == InvalidRowHandle || FocusedRowHandle == CurrencyDataController.NewItemRow) && RecordCount > 0) FocusedRowHandle = RecordCount - 1;
				RefreshRow(CurrencyDataController.NewItemRow);
			}
			finally {
				this.EndUpdate();
				ResetArrangeDirection();
			}
		}
		void IDataControllerValidationSupport.OnPostCellException(ControllerRowCellExceptionEventArgs e) {
			throw e.Exception;
		}
		void IDataControllerValidationSupport.OnPostRowException(ControllerRowExceptionEventArgs e) {
			OnPostRowException(e);
		}
		void IDataControllerValidationSupport.OnValidatingCurrentRow(ValidateControllerRowEventArgs e) {
			if(e.RowHandle != FocusedRowHandle) return;
			OnValidatingCurrentRow(e);
		}
		protected override void OnCurrentControllerRowChanged(CurrentRowEventArgs e) {
			if(internalNewRowEditing) return;
			int dataControllerRow = DataController.CurrentControllerRow;
			if(!fInternalVisibleRecordIndexChange && !IsCardVisible(dataControllerRow)) visibleCardIndex = dataControllerRow;
			if(FocusedRowHandle != dataControllerRow) FocusedRowHandle = dataControllerRow;
			if(!IsCardVisible(FocusedRowHandle)) MakeRowVisibleCore(FocusedRowHandle, true);
			Invalidate();
		}
		#endregion
		protected internal bool CanRaiseRowCellUserEvents { get { return IsFocusedRowLoaded; } }
		internal void RaiseCardClick(CardClickEventArgs e) {
			CardClickEventHandler handler = (CardClickEventHandler)this.Events[cardClick];
			if(handler != null) handler(this, e);
		}
		internal void RaiseFieldValueClick(FieldValueClickEventArgs e) {
			FieldValueClickEventHandler handler = (FieldValueClickEventHandler)this.Events[fieldValueClick];
			if(handler != null) handler(this, e);
		}
		internal LayoutViewCard DrawCard { get; set; }
	}
	internal static class AttachedProperty {
		static object lockObj = new object();
		static Dictionary<PropertyKey, object> PropertyBag = new Dictionary<PropertyKey, object>();
		class PropertyKey {
			public readonly object Owner;
			public readonly object Property;
			internal PropertyKey(object owner, object property) {
				Owner = owner;
				Property = property;
			}
			public override bool Equals(object obj) {
				if(!(obj is PropertyKey)) return false;
				return ReferenceEquals(Owner, ((PropertyKey)obj).Owner) &&
					ReferenceEquals(Property, ((PropertyKey)obj).Property);
			}
			public override int GetHashCode() {
				return Property.GetHashCode() ^ Owner.GetHashCode();
			}
		}
		public static void SetValue(object owner, object property, object value) {
			if(object.ReferenceEquals(owner, null)) return;
			PropertyKey item = new PropertyKey(owner, property);
			lock(lockObj) {
				object result = null;
				if(PropertyBag.TryGetValue(item, out result)) {
					if(result != value) PropertyBag[item] = value;
					return;
				}
				PropertyBag.Add(item, value);
			}
		}
		public static object GetValue(object owner, object property) {
			if(object.ReferenceEquals(owner, null)) return null;
			lock(lockObj) {
				object value;
				PropertyBag.TryGetValue(new PropertyKey(owner, property), out value);
				return value;
			}
		}
		public static bool HasValue(object owner, object property) {
			if(object.ReferenceEquals(owner, null)) return false;
			lock(lockObj) {
				return PropertyBag.ContainsKey(new PropertyKey(owner, property));
			}
		}
		public static void ClearValue(object owner, object property) {
			if(object.ReferenceEquals(owner, null)) return;
			lock(lockObj) {
				PropertyBag.Remove(new PropertyKey(owner, property));
			}
		}
	}
	internal class LayoutGroupGenerate : LayoutControlGroup {
		public LayoutGroupGenerate()
			: base() {
			this.GroupBordersVisible = false;
		}
		public void SetChildrenVisible(bool visible) {
			base.UpdateChildren(visible);
		}
		public void UpdateFlowItems() {
			base.UpdateFlowItems(true);
		}
	}
}
