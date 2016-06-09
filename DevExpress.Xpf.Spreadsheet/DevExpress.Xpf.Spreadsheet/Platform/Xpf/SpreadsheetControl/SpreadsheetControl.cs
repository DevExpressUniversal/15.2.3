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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Internal;
using System.Windows.Controls;
using DevExpress.XtraSpreadsheet;
using System.Drawing;
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.Xpf.Spreadsheet.Localization;
using DevExpress.Xpf.Spreadsheet.UI;
using DevExpress.Utils.Controls;
using DevExpress.Xpf.Spreadsheet.Internal;
using System.Windows.Threading;
using DevExpress.XtraSpreadsheet.Mouse;
using System.Linq;
using System.Linq.Expressions;
#if SL
using PlatformIWin32Window = DevExpress.Xpf.Core.Native.IWin32Window;
using PlatformIndependentCursor = System.Windows.Input.Cursor;
#else
using PlatformIWin32Window = System.Windows.Forms.IWin32Window;
using PlatformIndependentCursor = System.Windows.Forms.Cursor;
using PlatformDialogResult = System.Windows.Forms.DialogResult;
using PlatformIndependentScrollEventArgs = System.Windows.Forms.ScrollEventArgs;
using DevExpress.Office.Internal;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Spreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
using System.IO;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.XtraSpreadsheet.Menu;
using DevExpress.Xpf.Spreadsheet.Menu;
using DevExpress.Xpf.Spreadsheet.Services;
using System.Windows.Media;
using DevExpress.Office.Services.Implementation;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.Xpf.Office.UI;
using DevExpress.Office;
using DevExpress.Data.Utils;
using DevExpress.XtraSpreadsheet.Layout;
using System.Collections;
#endif
namespace DevExpress.Xpf.Spreadsheet {
	[DXToolboxBrowsable(true)]
	[ToolboxTabName(AssemblyInfo.DXTabWpfSpreadsheet)]
	public partial class SpreadsheetControl : Control, PlatformIWin32Window, IDisposable,  ILogicalOwner, IOfficeFontSizeProvider {
		#region Const
		const string HorizontalScrollBarName = "HorizontalScrollBar";
		const string VerticalScrollBarName = "VerticalScrollBar";
		const string TabSelectorName = "TabSelector";
		#endregion
		public static readonly DependencyProperty GridLinesColorProperty;
		public static readonly DependencyProperty ActiveSheetIndexProperty;
		public static readonly DependencyProperty ActiveSheetNameProperty;
		public static readonly DependencyProperty CellTemplateSelectorProperty;
		public static readonly DependencyProperty CellTemplateProperty;
		protected static readonly DependencyPropertyKey SheetCountPropertyKey;
		public static readonly DependencyProperty SheetCountProperty;
		protected static readonly DependencyPropertyKey VisibleSheetNamesPropertyKey;
		public static readonly DependencyProperty VisibleSheetNamesProperty;
		public static readonly DependencyProperty ReadOnlyProperty;
		public static readonly DependencyProperty ModifiedProperty;
		public static readonly DependencyProperty UnitProperty;
		public static readonly DependencyProperty LayoutUnitProperty;
		public static readonly DependencyProperty SelectionProperty;
		public static readonly DependencyProperty SelectedCellProperty;
		public static readonly DependencyProperty SelectedShapeProperty;
		public static readonly DependencyProperty SelectedPictureProperty;
		public static readonly DependencyProperty MenuCustomizationsProperty;
		public static readonly DependencyProperty ShowTabSelectorProperty;
		public static readonly DependencyProperty HorizontalScrollbarVisibilityProperty;
		public static readonly DependencyProperty VerticalScrollbarVisibilityProperty;
		public static readonly DependencyProperty DocumentSourceProperty;
		public static readonly DependencyProperty OptionsProperty;
		public static readonly DependencyProperty ShowCellToolTipModeProperty;
		public static readonly DependencyProperty CellToolTipProperty;
		public static readonly DependencyProperty AcceptsTabProperty;
		public static readonly DependencyProperty AcceptsReturnProperty;
		public static readonly DependencyProperty AcceptsEscapeProperty;
#if !SL
		int threadIdleSubscribeCount;
		ThreadIdleWeakEventHandler<SpreadsheetControl> threadIdleHandler;
#endif
		static SpreadsheetControl() {
			Type ownerType = typeof(SpreadsheetControl);
			GridLinesColorProperty = DependencyProperty.Register("GridLinesColor", typeof(System.Windows.Media.Color), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetControl)d).OnGridLinesColorChanged()));
			ActiveSheetIndexProperty = DependencyProperty.Register("ActiveSheetIndex", typeof(int), ownerType,
				new FrameworkPropertyMetadata(-1, (d, e) => ((SpreadsheetControl)d).OnActiveSheetIndexChanged()));
			ActiveSheetNameProperty = DependencyProperty.Register("ActiveSheetName", typeof(string), ownerType,
				new FrameworkPropertyMetadata("", (d, e) => ((SpreadsheetControl)d).OnActiveSheetNameChanged()));
			CellTemplateSelectorProperty = DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), ownerType,
			   new FrameworkPropertyMetadata(null, (d, e) => ((SpreadsheetControl)d).OnCellTemplateSelectorChanged()));
			CellTemplateProperty = DependencyProperty.Register("CellTemplate", typeof(DataTemplate), ownerType,
			   new FrameworkPropertyMetadata(null, (d, e) => ((SpreadsheetControl)d).OnCellTemplateChanged()));
			SheetCountPropertyKey = DependencyProperty.RegisterReadOnly("SheetCount", typeof(int), ownerType, new FrameworkPropertyMetadata(0));
			SheetCountProperty = SheetCountPropertyKey.DependencyProperty;
			ReadOnlyProperty = DependencyProperty.Register("ReadOnly", typeof(bool), ownerType,
			   new FrameworkPropertyMetadata(false, (d, e) => ((SpreadsheetControl)d).OnReadOnlyChanged()));
			ModifiedProperty = DependencyProperty.Register("Modified", typeof(bool), ownerType,
			  new FrameworkPropertyMetadata(false, (d, e) => ((SpreadsheetControl)d).OnModifiedChanged()));
			UnitProperty = DependencyProperty.Register("Unit", typeof(DocumentUnit), ownerType,
			  new FrameworkPropertyMetadata(DocumentUnit.Document, (d, e) => ((SpreadsheetControl)d).OnUnitChanged()));
			LayoutUnitProperty = DependencyProperty.Register("LayoutUnit", typeof(DocumentLayoutUnit), ownerType,
			 new FrameworkPropertyMetadata(DocumentLayoutUnit.Document, (d, e) => ((SpreadsheetControl)d).OnLayoutUnitChanged()));
			SelectedCellProperty = DependencyProperty.Register("SelectedCell", typeof(Range), ownerType,
			 new FrameworkPropertyMetadata(null, (d, e) => ((SpreadsheetControl)d).OnSelectedCellChanged()));
			SelectionProperty = DependencyProperty.Register("Selection", typeof(Range), ownerType,
			new FrameworkPropertyMetadata(null, (d, e) => ((SpreadsheetControl)d).OnSelectionChanged()));
			SelectedShapeProperty = DependencyProperty.Register("SelectedShape", typeof(Shape), ownerType,
			 new FrameworkPropertyMetadata(null, (d, e) => ((SpreadsheetControl)d).OnSelectedShapeChanged()));
			SelectedPictureProperty = DependencyProperty.Register("SelectedPicture", typeof(DevExpress.Spreadsheet.Picture), ownerType,
			 new FrameworkPropertyMetadata(null, (d, e) => ((SpreadsheetControl)d).OnSelectedPictureChanged()));
			VisibleSheetNamesPropertyKey = DependencyProperty.RegisterReadOnly("VisibleSheetNames", typeof(IEnumerable<string>), ownerType, new FrameworkPropertyMetadata());
			VisibleSheetNamesProperty = VisibleSheetNamesPropertyKey.DependencyProperty;
			MenuCustomizationsProperty = DependencyProperty.Register("MenuCustomizations", typeof(ObservableCollection<SpreadsheetMenuCustomization>), ownerType,
				new FrameworkPropertyMetadata(null));
			ShowTabSelectorProperty = DependencyProperty.Register("ShowTabSelector", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			HorizontalScrollbarVisibilityProperty = DependencyProperty.Register("HorizontalScrollbarVisibility", typeof(SpreadsheetElementVisibility), ownerType,
				new FrameworkPropertyMetadata(SpreadsheetElementVisibility.Default, (d, e) => ((SpreadsheetControl)d).OnHorizontalScrollbarVisibilityChanged(e.NewValue)));
			VerticalScrollbarVisibilityProperty = DependencyProperty.Register("VerticalScrollbarVisibility", typeof(SpreadsheetElementVisibility), ownerType,
				new FrameworkPropertyMetadata(SpreadsheetElementVisibility.Default, (d, e) => ((SpreadsheetControl)d).OnVerticalScrollbarVisibilityChanged(e.NewValue)));
			DocumentSourceProperty = DependencyProperty.Register("DocumentSource", typeof(object), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((SpreadsheetControl)d).OnDocumentSourceChanged(e.OldValue, e.NewValue)));
			OptionsProperty = DependencyProperty.Register("Options", typeof(SpreadsheetControlOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetControl)d).OnOptionsChanged((SpreadsheetControlOptions)e.OldValue, (SpreadsheetControlOptions)e.NewValue)));
			ShowCellToolTipModeProperty = DependencyProperty.Register("ShowCellToolTipMode", typeof(ShowCellToolTipMode), ownerType, new FrameworkPropertyMetadata(ShowCellToolTipMode.Auto));
			CellToolTipProperty = DependencyProperty.Register("CellToolTip", typeof(object), ownerType);
			AcceptsTabProperty = DependencyProperty.Register("AcceptsTab", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			AcceptsReturnProperty = DependencyProperty.Register("AcceptsReturn", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			AcceptsEscapeProperty = DependencyProperty.Register("AcceptsEscape", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
		}
		public SpreadsheetControl() {
			this.MenuCustomizations = new ObservableCollection<SpreadsheetMenuCustomization>();
			this.DefaultStyleKey = typeof(SpreadsheetControl);
			this.accessor = new SpreadsheetControlAccessor(this);
			this.commandManager = new SpreadsheetControlBarCommandManager(this);
			this.innerControl = CreateInnerControl();
#if !SL
			this.threadIdleHandler = new ThreadIdleWeakEventHandler<SpreadsheetControl>(this, OnApplicationIdleHandler);
#endif
			this.ChartLayoutGalleryGroups = new ObservableCollection<ChartLayoutGalleryGroupInfo>();
			this.ChartLayoutGalleryGroups.Add(new ChartLayoutGalleryGroupInfo(this));
			BeginInitializeInnerControl(false);
			SubscribeOwnEvents();
			SubscribeInnerEvents();
			BindInnerControlOptions();
			this.InnerControl.EndInitialize(true);
			AddCommandFactoryService();
			CreateLayoutUnitHandler();
			PostponedActionHelper = new PostponedActionHelper();
			Options = new SpreadsheetControlOptions();
			InitializeMouse();
		}
		public bool ReadOnly {
			get { return (bool)GetValue(ReadOnlyProperty); }
			set { SetValue(ReadOnlyProperty, value); }
		}
		public bool Modified {
			get { return (bool)GetValue(ModifiedProperty); }
			set { SetValue(ModifiedProperty, value); }
		}
		public IClipboardManager Clipboard {
			get { return InnerControl.Clipboard; }
		}
		public DocumentUnit Unit {
			get { return (DocumentUnit)GetValue(UnitProperty); }
			set { SetValue(UnitProperty, value); }
		}
		[Browsable(false)]
		public Range SelectedCell {
			get { return (Range)GetValue(SelectedCellProperty); }
			set { SetValue(SelectedCellProperty, value); }
		}
		[Browsable(false)]
		public Range Selection {
			get { return (Range)GetValue(SelectionProperty); }
			set { SetValue(SelectionProperty, value); }
		}
		[Browsable(false)]
		public DevExpress.Spreadsheet.Shape SelectedShape {
			get { return (DevExpress.Spreadsheet.Shape)GetValue(SelectedShapeProperty); }
			set { SetValue(SelectedShapeProperty, value); }
		}
		[Browsable(false)]
		public DevExpress.Spreadsheet.Picture SelectedPicture {
			get { return (DevExpress.Spreadsheet.Picture)GetValue(SelectedPictureProperty); }
			set { SetValue(SelectedPictureProperty, value); }
		}
		public DocumentLayoutUnit LayoutUnit {
			get { return (DocumentLayoutUnit)GetValue(LayoutUnitProperty); }
			set { SetValue(LayoutUnitProperty, value); }
		}
		private List<IWorksheet> GetSheets() {
			return InnerControl.DocumentModel.GetSheets();
		}
		public int ActiveSheetIndex {
			get { return (int)GetValue(ActiveSheetIndexProperty); }
			set { SetValue(ActiveSheetIndexProperty, value); }
		}
		public string ActiveSheetName {
			get { return (string)GetValue(ActiveSheetNameProperty); }
			set { SetValue(ActiveSheetNameProperty, value); }
		}
		public DataTemplateSelector CellTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
			set { SetValue(CellTemplateSelectorProperty, value); }
		}
		public DataTemplate CellTemplate {
			get { return (DataTemplate)GetValue(CellTemplateProperty); }
			set { SetValue(CellTemplateProperty, value); }
		}
		public int SheetCount {
			get { return (int)GetValue(SheetCountProperty); }
			protected set { SetValue(SheetCountPropertyKey, value); }
		}
		public IEnumerable<string> VisibleSheetNames {
			get { return (IEnumerable<string>)GetValue(VisibleSheetNamesProperty); }
			protected set { SetValue(VisibleSheetNamesPropertyKey, value); }
		}
		public ObservableCollection<SpreadsheetMenuCustomization> MenuCustomizations {
			get { return (ObservableCollection<SpreadsheetMenuCustomization>)GetValue(MenuCustomizationsProperty); }
			set { SetValue(MenuCustomizationsProperty, value); }
		}
		public bool ShowTabSelector {
			get { return (bool)GetValue(ShowTabSelectorProperty); }
			set { SetValue(ShowTabSelectorProperty, value); }
		}
		public ShowCellToolTipMode ShowCellToolTipMode {
			get { return (ShowCellToolTipMode)GetValue(ShowCellToolTipModeProperty); }
			set { SetValue(ShowCellToolTipModeProperty, value); }
		}
		public object CellToolTip {
			get { return (object)GetValue(CellToolTipProperty); }
			set { SetValue(CellToolTipProperty, value); }
		}
		public SpreadsheetControlOptions Options {
			get { return (SpreadsheetControlOptions)GetValue(OptionsProperty); }
			set { SetValue(OptionsProperty, value); }
		}
		public bool AcceptsTab {
			get { return (bool)GetValue(AcceptsTabProperty); }
			set { SetValue(AcceptsTabProperty, value); }
		}
		public bool AcceptsReturn {
			get { return (bool)GetValue(AcceptsReturnProperty); }
			set { SetValue(AcceptsReturnProperty, value); }
		}
		public bool AcceptsEscape {
			get { return (bool)GetValue(AcceptsEscapeProperty); }
			set { SetValue(AcceptsEscapeProperty, value); }
		}
		void OnGridLinesColorChanged() {
			SetDocumentGridLinesColor();
			InnerControl.ResetDocumentLayout();
			Redraw();
		}
		private void OnLayoutUnitChanged() {
			InnerControl.LayoutUnit = LayoutUnit;
		}
		void DocumentModelLayoutUnitChanged() {
			if (LayoutUnit != InnerControl.LayoutUnit) LayoutUnit = InnerControl.LayoutUnit;
		}
		private void OnSelectionChanged() {
			SetInnerControlSelection(Selection);
		}
		private void SetInnerControlSelection(Range value) {
			InnerControl.SelectedApiRange = value;
		}
		private void OnSelectedCellChanged() {
			SetInnerControlSelectedCell(SelectedCell);
		}
		private void SetInnerControlSelectedCell(Range value) {
			InnerControl.SelectedApiCell = value;
		}
		private void OnSelectedShapeChanged() {
			SetInnerControlSelectedShape(SelectedShape);
		}
		private void SetInnerControlSelectedShape(Shape value) {
			InnerControl.SelectedApiShape = value;
		}
		private void OnSelectedPictureChanged() {
			SetInnerControlSelectedPicture(SelectedPicture);
		}
		private void SetInnerControlSelectedPicture(DevExpress.Spreadsheet.Picture value) {
			InnerControl.SelectedApiPicture = value;
		}
		private void OnUnitChanged() {
			SetInnerControlUnit(Unit);
		}
		private void SetInnerControlUnit(DocumentUnit value) {
			InnerControl.Unit = value;
		}
		private void OnModifiedChanged() {
			SetInnerControlModifed(Modified);
		}
		private void SetInnerControlModifed(bool value) {
			if (InnerControl.Modified != value)
				InnerControl.Modified = value;
		}
		private void OnReadOnlyChanged() {
			SetInnerControlReadOnly(ReadOnly);
		}
		private void SetInnerControlReadOnly(bool value) {
			if (InnerControl.IsReadOnly != value)
				InnerControl.IsReadOnly = value;
		}
		private void OnCellTemplateChanged() {
			Redraw();
		}
		private void OnCellTemplateSelectorChanged() {
			Redraw();
		}
		private void OnActiveSheetNameChanged() {
			if (IsInnerControlInitialized)
				SetActiveSheetByName();
			else
				PostponeAction(new Action(SetActiveSheetByName), null);
		}
		private void OnActiveSheetIndexChanged() {
			if (IsInnerControlInitialized)
				SetActiveSheetByIndex();
			else
				PostponeAction(new Action(SetActiveSheetByIndex), null);
		}
		private void SetActiveSheetByIndex() {
			List<IWorksheet> sheets = GetSheets();
			if (ActiveSheetIndex >= 0 && ActiveSheetIndex < sheets.Count)
				InnerControl.DocumentModel.ActiveSheetIndex = ActiveSheetIndex;
		}
		private void SetActiveSheetByName() {
			IWorksheet sheet = DocumentModel.GetSheetByName(ActiveSheetName);
			if (sheet != null) {
				InnerControl.DocumentModel.ActiveSheetIndex = GetSheets().IndexOf(sheet);
			}
		}
		void OnOptionsChanged(SpreadsheetControlOptions oldValue, SpreadsheetControlOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			DocumentOptions source = InnerControl.Options;
			if (newValue != null && source != null)
				newValue.SetSource(source);
		}
		public ICommand GetCommand(SpreadsheetCommandId id) {
			return new SpreadsheetWrappedCommand(this, id);
		}
		private void UpdateSheetCountAndNames() {
			if (this.DocumentModel != null) {
				List<IWorksheet> sheets = this.DocumentModel.GetSheets();
				if (sheets != null)
					SheetCount = sheets.Count;
				VisibleSheetNames = this.DocumentModel.GetVisibleSheetNames();
			}
		}
		#region Scrollbars visibility
		public SpreadsheetElementVisibility HorizontalScrollbarVisibility {
			get { return (SpreadsheetElementVisibility)GetValue(HorizontalScrollbarVisibilityProperty); }
			set { SetValue(HorizontalScrollbarVisibilityProperty, value); }
		}
		void OnHorizontalScrollbarVisibilityChanged(object newValue) {
			SpreadsheetElementVisibility visibility = (SpreadsheetElementVisibility)newValue;
			SetViewScrollbarVisibility(SetViewHorizontalScrollbarVisiblity, visibility);
		}
		void SetViewHorizontalScrollbarVisiblity(bool isVisible) {
			ViewControl.ShowHorizontalScrollbar = isVisible;
		}
		public SpreadsheetElementVisibility VerticalScrollbarVisibility {
			get { return (SpreadsheetElementVisibility)GetValue(VerticalScrollbarVisibilityProperty); }
			set { SetValue(VerticalScrollbarVisibilityProperty, value); }
		}
		void OnVerticalScrollbarVisibilityChanged(object newValue) {
			SpreadsheetElementVisibility visibility = (SpreadsheetElementVisibility)newValue;
			SetViewScrollbarVisibility(SetViewVerticalScrollbarVisibility, visibility);
		}
		void SetViewVerticalScrollbarVisibility(bool isVisible) {
			ViewControl.ShowVerticalScrollbar = isVisible;
		}
		public delegate void SetViewScrollbarVisibilityDelegate(bool isVisible);
		void SetViewScrollbarVisibility(SetViewScrollbarVisibilityDelegate action, SpreadsheetElementVisibility visibility) {
			bool isVisible = CalculateScrollbarVisibility(visibility);
			if (ViewControl != null)
				action(isVisible);
			else
				PostponeAction(action, new object[] { isVisible });
		}
		bool CalculateScrollbarVisibility(SpreadsheetElementVisibility visibility) {
			if (visibility == SpreadsheetElementVisibility.Default)
				return true;
			if (visibility == SpreadsheetElementVisibility.Hidden)
				return false;
			return true; 
		}
		#endregion
		public object DocumentSource {
			get { return GetValue(DocumentSourceProperty); }
			set { SetValue(DocumentSourceProperty, value); }
		}
		void OnDocumentSourceChanged(object oldValue, object newValue) {
			string fileName = newValue as string;
			if (!String.IsNullOrEmpty(fileName)) {
				LoadDocument(fileName);
				return;
			}
			SpreadsheetDocumentSource documentSource = newValue as SpreadsheetDocumentSource;
			if (documentSource != null)
				LoadDocument(documentSource);
		}
		void LoadDocument(SpreadsheetDocumentSource source) {
			LoadDocument(source.Stream, source.Format);
		}
		bool IsInnerControlInitialized { get; set; }
		WeakEventHandler<SpreadsheetControl, EventArgs, EventHandler> LayoutUnitChangedHandler { get; set; }
		SpreadsheetControlAccessor accessor;
		readonly SpreadsheetControlBarCommandManager commandManager;
		private void BindInnerControlOptions() {
		}
		private void CreateLayoutUnitHandler() {
			LayoutUnitChangedHandler = new WeakEventHandler<SpreadsheetControl, EventArgs, EventHandler>(this,
				(spreadsheet, sender, handler) => spreadsheet.DocumentModelLayoutUnitChanged(),
				(h, sender) => ((DocumentModel)sender).LayoutUnitChanged -= h.Handler,
				h => h.OnEvent);
		}
		private void SubscribeInnerEvents() {
			this.DocumentModel.InnerActiveSheetChanged += InnerControlActiveSheetChanged;
			this.DocumentModel.InnerSelectionChanged += InnerControlSelectionChanged;
			this.InnerControl.SheetRemoved += InnerControlSheetRemoved;
			this.InnerControl.SheetInserted += InnerControlSheetInserted;
			this.InnerControl.UnitChanged += InnerControlUnitChanged;
			this.InnerControl.ReadOnlyChanged += InnerControlReadOnlyChanged;
		}
		private void SubscribeOwnEvents() {
			this.Loaded += SpreadsheetControlLoaded;
			this.Unloaded += SpreadsheetControlUnloaded;
			this.SizeChanged += SpreadsheetControlSizeChanged;
			this.IsEnabledChanged += SpreadsheetControlIsEnabledChanged;
			this.ContextMenuOpening += OnContextMenuOpening;
		}
		void SpreadsheetControlIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if (ActiveView == null) return;
			if (ActiveView.HorizontalScrollController != null) {
				ActiveView.HorizontalScrollController.ScrollBarAdapter.Enabled = (bool)e.NewValue;
				ActiveView.HorizontalScrollController.ScrollBarAdapter.ApplyValuesToScrollBar();
			}
			if (ActiveView.VerticalScrollController != null) {
				ActiveView.VerticalScrollController.ScrollBarAdapter.Enabled = (bool)e.NewValue;
				ActiveView.VerticalScrollController.ScrollBarAdapter.ApplyValuesToScrollBar();
			}
		}
		EventHandler<PopupMenuShowingEventArgs> popupMenuShowing;
		public event EventHandler<PopupMenuShowingEventArgs> PopupMenuShowing {
			add { popupMenuShowing += value; }
			remove { popupMenuShowing -= value; }
		}
		void InnerControlSelectionChanged(object sender, EventArgs e) {
			SetSelectionFromInnerControl();
		}
		private void SetSelectionFromInnerControl() {
			if (SelectedCell != InnerControl.SelectedApiCell)
				SetCurrentValue(SelectedCellProperty, InnerControl.SelectedApiCell);
			if (Selection != InnerControl.SelectedApiRange)
				SetCurrentValue(SelectionProperty, InnerControl.SelectedApiRange);
			if (SelectedShape != InnerControl.SelectedApiShape)
				SetCurrentValue(SelectedShapeProperty, InnerControl.SelectedApiShape);
			if (SelectedPicture != InnerControl.SelectedApiPicture)
				SetCurrentValue(SelectedPictureProperty, InnerControl.SelectedApiPicture);
		}
		void InnerControlReadOnlyChanged(object sender, EventArgs e) {
			if (ReadOnly != InnerControl.IsReadOnly) SetCurrentValue(ReadOnlyProperty, InnerControl.IsReadOnly);
		}
		void InnerControlUnitChanged(object sender, EventArgs e) {
			if (Unit != InnerControl.Unit) SetCurrentValue(UnitProperty, InnerControl.Unit);
		}
		void InnerControlSheetInserted(object sender, SheetInsertedEventArgs e) {
			UpdateSheetCountAndNames();
		}
		void InnerControlSheetRemoved(object sender, SheetRemovedEventArgs e) {
			UpdateSheetCountAndNames();
		}
		private void BeginInitializeInnerControl(bool keepContent) {
			IsInnerControlInitialized = false;
			if (keepContent) {
				InnerControl.BeginInitialize(true);
			}
			else {
				BeginInitialize();
				InnerControl.SetInitialDocumentModelLayoutUnit(DefaultLayoutUnit);
			}
		}
		void InnerControlActiveSheetChanged(object sender, ActiveSheetChangedEventArgs e) {
			SetActiveSheetNameFromModel(e.NewActiveSheetName);
			SetActiveSheetIndexFromModel(e.NewActiveSheetName);
		}
		private void SetActiveSheetNameFromModel(string name) {
			ActiveSheetName = name;
		}
		private void SetActiveSheetIndexFromModel(string name) {
			ActiveSheetIndex = GetSheetIndex(name);
		}
		private int GetSheetIndex(string name) {
			IWorksheet sheet = DocumentModel.GetSheetByName(name);
			List<IWorksheet> sheets = GetSheets();
			return sheets.Count > 0 ? sheets.IndexOf(sheet) : -1;
		}
		void OnContextMenuOpening(object sender, ContextMenuEventArgs e) {
			e.Handled = ShowContextMenu();
		}
		internal bool ShowContextMenu() {
			DocumentCapability showPopupMenu = Options.Behavior.ShowPopupMenu;
			if (showPopupMenu == DocumentCapability.Disabled || showPopupMenu == DocumentCapability.Hidden)
				return false;
			ClosePopups();
			ISpreadsheetContextMenuService service = GetService<ISpreadsheetContextMenuService>();
			return service.ShowContextMenu(this);
		}
		protected virtual void ClosePopups() {
			PopupMenuManager.CloseAllPopups();
		}
		void SpreadsheetControlSizeChanged(object sender, SizeChangedEventArgs e) {
		}
		internal void OnViewportChanged(System.Drawing.Size size) {
			if (ActiveView == null)
				return;
			if (size == System.Drawing.Size.Empty)
				return;
			this.ResizeViewCore();
			OnResizeCore();
			Redraw();
		}
		void SpreadsheetControlUnloaded(object sender, RoutedEventArgs e) {
			if (IsInnerControlInitialized)
				InnerControl.HidePivotTableFieldsPanel();
#if !SL
			System.Windows.Interop.ComponentDispatcher.ThreadIdle -= threadIdleHandler.Handler;
			threadIdleSubscribeCount = Math.Max(0, threadIdleSubscribeCount - 1);
#endif
		}
		void SpreadsheetControlLoaded(object sender, RoutedEventArgs e) {
			if (IsInnerControlInitialized)
				InnerControl.ResetPivotTableFieldsPanelVisibility();
#if !SL
			if (threadIdleSubscribeCount <= 0) {
				System.Windows.Interop.ComponentDispatcher.ThreadIdle += threadIdleHandler.Handler;
				threadIdleSubscribeCount++;
			}
#endif
		}
		private void EndInitialization() {
			if (!IsInnerControlInitialized) {
				CreateGetureHelper();
				IsInnerControlInitialized = true;
				InitializeByTheme(); 
				DoInitializationOnLoadedCore();
				InitializeHeaderSize();
				UpdatePlatformSpecificProperties();
				UpdatePivotTableFieldsPanelVisibility();
#if !SL
				InternalUpdateBarManager();
#endif
#if SL && !DEBUG
#endif
				RaiseInnerControlInitialized();
				ExecutePostponedActions();
			}
			Redraw();
		}
#if !SL
		void OnApplicationIdleHandler(SpreadsheetControl control, object sender, EventArgs e) {
			if (control != null)
				control.OnApplicationIdle(sender, e);
		}
#endif
		void OnApplicationIdle(object sender, EventArgs e) {
			if (InnerControl != null)
				InnerControl.OnApplicationIdle();
		}
		GestureHelper gestureHelper;
		private void CreateGetureHelper() {
			if (gestureHelper != null) gestureHelper.Stop();
			else gestureHelper = new GestureHelper(this, this);
			gestureHelper.Start();
		}
		private void UpdatePlatformSpecificProperties() {
			UpdateSheetCountAndNames();
			UpdateSelectionAndSelectedCell();
		}
		private void UpdateSelectionAndSelectedCell() {
			SetSelectionFromInnerControl();
		}
		PostponedActionHelper PostponedActionHelper { get; set; }
		private void ExecutePostponedActions() {
			PostponedActionHelper.Execute();
		}
		private void PostponeAction(Delegate action, object[] p) {
			PostponedActionHelper.EnqueuePostponed(action, p);
		}
		void UpdatePivotTableFieldsPanelVisibility() {
			innerControl.UpdatePivotTableFieldsPanelVisibility();
		}
		#region Properties
		public ScrollBar VerticalScrollBar {
			get {
				return ViewControl != null ? ViewControl.GetVerticalScrollBar() : null;
			}
		}
		public ScrollBar HorizontalScrollBar {
			get {
				return ViewControl != null ? ViewControl.GetHorizontalScrollBar() : null;
			}
		}
		public System.Windows.Media.Color GridLinesColor {
			get { return (System.Windows.Media.Color)GetValue(GridLinesColorProperty); }
			set { SetValue(GridLinesColorProperty, value); }
		}
#if !SL
	[DevExpressXpfSpreadsheetLocalizedDescription("SpreadsheetControlViewBounds")]
#endif
		public Rectangle ViewBounds {
			get { return GetViewBounds(false); }
		}
		public Rectangle LayoutViewBounds {
			get { return GetViewBounds(true); }
		}
		Rectangle GetViewBounds(bool isLayoutView) {
			if (ViewControl == null) return new Rectangle(0, 0, 0, 0);
			System.Windows.Point p = GetViewControlPosition();
			System.Drawing.Size viewport = ViewControl.GetViewport();
			double viewportWidth = isLayoutView ? viewport.Width * DpiX / 96.0 : viewport.Width;
			double viewportHeight = isLayoutView ? viewport.Height * DpiY / 96.0 : viewport.Height;
			Rectangle result = new Rectangle((int)p.X, (int)p.Y, (int)viewportWidth, (int)viewportHeight);
			if (ActiveView != null)
				return ActiveView.DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(result, DpiX, DpiY);
			else
				return Units.PixelsToDocuments(result, DpiX, DpiY);
		}
		System.Windows.Point GetViewControlPosition() {
			return CanGetViewControlPosition() ? ViewControl.TransformToVisual(this).Transform(new System.Windows.Point(0, 0)) : new System.Windows.Point(0, 0);
		}
		bool CanGetViewControlPosition() {
			return ViewControl != null && LayoutHelper.FindLayoutOrVisualParentObject<SpreadsheetControl>(ViewControl) != null;
		}
		protected internal SpreadsheetControlAccessor Accessor { get { return this.accessor; } }
		protected internal SpreadsheetControlBarCommandManager CommandManager { get { return commandManager; } }
		#endregion
		#region ISpreadsheetControl Members
		bool ISpreadsheetControl.UseGdiPlus { get { return false; } }
		bool ISpreadsheetControl.IsHyperlinkActive() {
			return false;
		}
		IPlatformSpecificScrollBarAdapter ISpreadsheetControl.CreatePlatformSpecificScrollBarAdapter() {
			return new XpfScrollBarAdapter();
		}
		SpreadsheetMouseHandlerStrategyFactory ISpreadsheetControl.CreateMouseHandlerStrategyFactory() {
			return new XpfSpreadsheetMouseHandlerStrategyFactory();
		}
		protected internal virtual void AddServicesPlatformSpecific() {
			RemoveServices();
			AddCommandFactoryService();
			AddService(typeof(ISpreadsheetPrintingService), new SpreadsheetPrintingService());
			AddService(typeof(IChartControllerFactoryService), new ChartControllerFactoryService());
			AddService(typeof(IChartImageService), new WpfChartImageService());
			AddService(typeof(ISpreadsheetContextMenuService), new SpreadsheetContextMenuService(this));
			AddService(typeof(IMessageBoxService), new MessageBoxService());
			AddService(typeof(IFontCharacterSetService), new FontCharsetService());
			AddService(typeof(IPictureImageService), new PictureImageService());
		}
		private void AddCommandFactoryService() {
			AddService(typeof(ISpreadsheetCommandFactoryService), new XpfSpreadsheetCommandFactoryService(this));
		}
		private XpfCellInplaceEditor GetInplaceEditor() {
			return ViewControl.GetEditor();
		}
		protected internal virtual IOfficeScrollbar CreateHorizontalScrollBar() {
			if (HorizontalScrollBar != null) {
				return new SpreadsheetOfficeScrollBar(HorizontalScrollBar);
			}
			else
				return null;
		}
		protected internal virtual IOfficeScrollbar CreateVerticalScrollBar() {
			if (VerticalScrollBar != null) {
				return new SpreadsheetOfficeScrollBar(VerticalScrollBar);
			}
			else
				return null;
		}
		internal void MeasureAndRedraw() {
			ViewControl.ClearMeasureCache();
			Redraw();
		}
		SpreadsheetViewVerticalScrollController ISpreadsheetControl.CreateSpreadsheetViewVerticalScrollController(SpreadsheetView view) {
			return new XpfSpreadsheetViewVerticalScrollController(view);
		}
		SpreadsheetViewHorizontalScrollController ISpreadsheetControl.CreateSpreadsheetViewHorizontalScrollController(SpreadsheetView view) {
			return new XpfSpreadsheetViewHorizontalScrollController(view);
		}
		PlatformIndependentCursor ISpreadsheetControl.Cursor {
			get { return CursorsProvider.GetPlatformIndependentCursor(this.Cursor); }
			set { this.Cursor = GetSpecificCursor() ?? CursorsProvider.GetCursor(value); }
		}
		private Cursor GetSpecificCursor() {
			System.Windows.Point position = Mouse.GetPosition(this);
			Rect vBounds = GetScrollBarBounds(VerticalScrollBar);
			Rect hBounds = GetScrollBarBounds(HorizontalScrollBar);
			if (vBounds.Contains(position) || hBounds.Contains(position)) return Cursors.Arrow;
			return ViewControl.GetResizeThumbCursor(position);
		}
		private Rect GetScrollBarBounds(ScrollBar scrollBar) {
			if (scrollBar == null) return new Rect();
			return scrollBar.TransformToVisual(this).TransformBounds(new Rect(0, 0, scrollBar.ActualWidth, scrollBar.ActualHeight));
		}
		#endregion
		#region ILogicalOwner Members
#if SL
		List<object> logicalChildren = new List<object>();
		void ILogicalOwner.AddChild(object child) {
			logicalChildren.Add(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			logicalChildren.Remove(child);
		}
		bool ILogicalOwner.IsLoaded { get { return true; } }
		event KeyboardFocusChangedEventHandler IInputElement.PreviewGotKeyboardFocus { add {} remove {} }
		event KeyboardFocusChangedEventHandler IInputElement.PreviewLostKeyboardFocus { add { } remove { } }
#else
		List<object> logicalChildren = new List<object>();
		void ILogicalOwner.AddChild(object child) {
			logicalChildren.Add(child);
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			logicalChildren.Remove(child);
			RemoveLogicalChild(child);
		}
		protected override IEnumerator LogicalChildren {
			get { return new MergedEnumerator(base.LogicalChildren, logicalChildren.GetEnumerator()); }
		}
#endif
		#endregion
		#region IInnerSpreadsheetControlOwner Members
		IOfficeScrollbar IInnerSpreadsheetControlOwner.CreateHorizontalScrollBar() {
			return this.CreateHorizontalScrollBar();
		}
		IOfficeScrollbar IInnerSpreadsheetControlOwner.CreateVerticalScrollBar() {
			return this.CreateVerticalScrollBar();
		}
		void IInnerSpreadsheetControlOwner.Redraw() {
			Redraw();
		}
		void IInnerSpreadsheetControlOwner.Redraw(RefreshAction action) {
			Redraw(action);
		}
		void IInnerSpreadsheetControlOwner.ResizeView() {
			ResizeViewCore();
		}
		private void ResizeViewCore() {
			Rectangle normalizedViewBounds = ViewBounds;
			normalizedViewBounds.X = 0;
			normalizedViewBounds.Y = 0;
			if (ActiveView != null)
				ActiveView.OnResize(normalizedViewBounds);
		}
		void IInnerSpreadsheetControlOwner.OnOptionsChangedPlatformSpecific(BaseOptionChangedEventArgs e) {
			this.OnOptionsChangedPlatformSpecific(e);
		}
		ITabSelector IInnerSpreadsheetControlOwner.CreateTabSelector() {
			return CreateTabSelector();
		}
		private ITabSelector CreateTabSelector() {
			return GetTabSelector();
		}
		private ITabSelector GetTabSelector() {
			return ViewControl != null ? ViewControl.GetTabSelector() : null;
		}
		protected internal virtual void OnOptionsChangedPlatformSpecific(BaseOptionChangedEventArgs e) {
		}
		public void RedrawCore(RefreshAction action) {
			RedrawCore(action, true);
		}
		public void RedrawCore(RefreshAction action, bool doRedraw) {
			if (IsUpdateLocked) {
				ControlDeferredChanges.Redraw = true;
				ControlDeferredChanges.RedrawAction |= action;
				if (!doRedraw) return;
				try { }
				finally {
				}
			}
			else {
				if (ActiveView == null)
					return;
				DocumentRendering(action);
			}
		}
		protected internal virtual void Redraw() {
			if (IsInnerControlInitialized)
				Redraw(RefreshAction.AllDocument);
		}
		public void Redraw(RefreshAction action) {
			if (Dispatcher.CheckAccess())
				RedrawCore(action);
			else
				UpdateUIFromBackgroundThread(delegate { RedrawCore(action); });
		}
		void DocumentRendering(RefreshAction action) {
			if (InnerControl == null)
				return;
			try {
				RenderDocumentCore(action);
			}
			finally {
			}
		}
		private void RenderDocumentCore(RefreshAction action) {
			if (!Dispatcher.CheckAccess()) {
				XpfBackgroundWorker.InvokeInUIThread(delegate { RefreshView(action); }, InvokerType.Refresh);
			}
			else {
				RefreshView(action);
			}
		}
		protected internal virtual void RefreshView(RefreshAction refreshAction) {
			if (ViewControl != null && this.InnerControl != null) {
				ViewControl.Invalidate(this.InnerControl.DesignDocumentLayout);
			}
		}
		#endregion
		#region IInnerSpreadsheetDocumentServerOwner
		void IInnerSpreadsheetDocumentServerOwner.RaiseDeferredEvents(DocumentModelChangeActions changeActions) {
			InnerSpreadsheetControl innerControl = InnerControl;
			IThreadSyncService service = GetService<IThreadSyncService>();
			if (innerControl != null && service != null)
				service.EnqueueInvokeInUIThread(new Action(delegate() { innerControl.RaiseDeferredEventsCore(changeActions); }));
			else {
				Action<DocumentModelChangeActions> action = new Action<DocumentModelChangeActions>(((IInnerSpreadsheetDocumentServerOwner)this).RaiseDeferredEvents);
				PostponeAction(action, new object[] { changeActions });
			}
		}
		DocumentOptions IInnerSpreadsheetDocumentServerOwner.CreateOptions(InnerSpreadsheetDocumentServer documentServer) {
			return new SpreadsheetDocumentServerOptions(documentServer);
		}
		MeasurementAndDrawingStrategy IInnerSpreadsheetDocumentServerOwner.CreateMeasurementAndDrawingStrategy(DocumentModel documentModel) {
			return new XpfMeasurementAndDrawingStrategy(documentModel);
		}
		#endregion
		#region IDisposable implementation
		void IDisposable.Dispose() {
			if (innerControl != null) {
				UnsubscribeInnerControlEvents();
				innerControl.Dispose();
				innerControl = null;
			}
		}
		#endregion
		bool isTemplateApplied = false;
		SpreadsheetViewControl spreadsheetViewControl;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (isTemplateApplied)
				BeginInitializeInnerControl(true);
			isTemplateApplied = true;
			spreadsheetViewControl = CreateSpreadsheetViewControl();
			spreadsheetViewControl.Loaded += SpreadsheetViewControlLoaded;
			InitializeColors();
		}
		private void InitializeHeaderSize() {
			this.InnerControl.HeadersSize = ViewControl.GetHeaderSize();
		}
		protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint) {
			return base.MeasureOverride(constraint);
		}
		void SpreadsheetViewWorkbookInitialized(object sender, EventArgs e) {
			EndInitialization();
		}
		void spreadsheetViewControl_Initialized(object sender, EventArgs e) {
		}
		void SpreadsheetViewControlUnloaded(object sender, RoutedEventArgs e) {
		}
		void SpreadsheetViewControlLoaded(object sender, RoutedEventArgs e) {
			if (CanEndInitialization())
				EndInitialization();
		}
		private bool CanEndInitialization() {
			return ViewControl.GetHorizontalScrollBar() != null && ViewControl.GetVerticalScrollBar() != null && ViewControl.WorksheetControl != null;
		}
		private SpreadsheetViewControl CreateSpreadsheetViewControl() {
			return LayoutHelper.FindElementByType(this, typeof(SpreadsheetViewControl)) as SpreadsheetViewControl;
		}
		void DoInitializationOnLoadedCore() {
			OnPresenterLoaded();
		}
		protected internal void OnPresenterLoaded() {
			PerformDefferedUpdaterOnPresenterLoaded();
			DoInitializationOnPresenterLoaded();
		}
		protected virtual void PerformDefferedUpdaterOnPresenterLoaded() {
			DeferredBackgroundThreadUIUpdater deferredUpdater = BackgroundThreadUIUpdater as DeferredBackgroundThreadUIUpdater;
			if (InnerControl == null) return;
			InnerControl.BackgroundThreadUIUpdater = new BeginInvokeBackgroundThreadUIUpdater(new ControlThreadSyncService(this));
			if (deferredUpdater != null)
				PerformDeferredUIUpdates(deferredUpdater);
		}
		protected internal void DoInitializationOnPresenterLoaded() {
			BeginUpdate();
			try {
				EndInitialize();
				commandManager.UpdateBarItemsDefaultValues();
				commandManager.UpdateRibbonItemsDefaultValues();
			}
			finally {
				EndUpdate();
			}
			UpdateUI += OnUpdateUI;
			OnUpdateUI(this, EventArgs.Empty);
		}
		internal System.Drawing.Color BackColor {
			get { return DocumentModel.SkinBackColor; }
			set { DocumentModel.SkinBackColor = value; }
		}
		protected internal virtual void EndInitialize() {
			EndInitializeCommon();
		}
		private void InitializeByTheme() {
			InitializeHeaderSize();
			InitializeColors();
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if (e.Property == Control.ForegroundProperty) {
				if (DocumentModel != null) {
					SetDocumentForeColor();
					if (ViewControl != null) {
						ViewControl.ClearMeasureCache();
						Redraw();
					}
				}
			}
		}
		private void InitializeColors() {
			SetDocumentGridLinesColor();
			DocumentModel.SkinBackColor = XpfTypeConverter.ToPlatformIndependentColor(((SolidColorBrush)Background).Color);
			SetDocumentForeColor();
		}
		private void SetDocumentGridLinesColor() {
			DocumentModel.SkinGridlineColor = GetGridLinesColor();
		}
		private System.Drawing.Color GetGridLinesColor() {
			return GridLinesColor.Equals(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)) ? GetThemeColor() : XpfTypeConverter.ToPlatformIndependentColor(GridLinesColor);
		}
		private System.Drawing.Color GetThemeColor() {
			return XpfTypeConverter.ToPlatformIndependentColor(ViewControl.GetGridLinesColor());
		}
		private void SetDocumentForeColor() {
			SolidColorBrush brush = Foreground as SolidColorBrush;
			if (brush == null)
				return;
			DocumentModel.SkinForeColor = XpfTypeConverter.ToPlatformIndependentColor(brush.Color);
		}
		public void BeginInvoke(Action method) {
			if (Dispatcher.CheckAccess()) {
				method();
			}
			else
				Dispatcher.BeginInvoke(method, new object[0]);
		}
#if !SL
		IntPtr PlatformIWin32Window.Handle { get { return IntPtr.Zero; } }
#endif
		private void DisposeViewPainter() {
		}
		private void DisposeBackgroundPainter() {
		}
		private void CreateViewPainter(SpreadsheetView view) {
		}
		private void CreateBackgroundPainter(SpreadsheetView view) {
		}
		private void OnReadOnlyChangedPlatformSpecific() {
		}
		private void OnEmptyDocumentCreatedPlatformSpecific() {
		}
		DevExpress.XtraSpreadsheet.Model.Worksheet ActiveSheet { get { return InnerControl.DocumentModel.ActiveSheet; } }
		private void OnDocumentLoadedPlatformSpecific() {
			if (ActiveSheet != null) {
				SetActiveSheetNameFromModel(ActiveSheet.Name);
				SetActiveSheetIndexFromModel(ActiveSheet.Name);
				UpdateSheetCountAndNames();
			}
		}
		private void ApplyChangesCorePlatformSpecific(DocumentModelChangeActions changeActions) {
			if (IsInnerControlInitialized) {
				if ((changeActions & DocumentModelChangeActions.ResetHeaderLayout) != 0)
					OnDeferredResizeCore();
				if ((changeActions & DocumentModelChangeActions.Redraw) != 0)
					Redraw();
			}
			else {
				PostponedActionHelper.EnqueuePostponed(new Action<DocumentModelChangeActions>(ApplyChangesCorePlatformSpecific), new object[] { changeActions });
			}
		}
		protected internal virtual void OnDeferredResizeCore() {
			if (IsUpdateLocked)
				ControlDeferredChanges.Resize = true;
			else
				OnResizeCore();
		}
		void IInnerSpreadsheetControlOwner.OnResizeCore() {
			this.OnResizeCore();
		}
		protected internal virtual void OnResizeCore() {
			UpdateRulersCore();
			UpdateVerticalScrollBar(false);
		}
		private void UpdateVerticalScrollBar(bool avoidJump) {
			if (InnerControl != null)
				InnerControl.UpdateVerticalScrollBar(avoidJump);
		}
		protected internal virtual void UpdateRulersCore() {
		}
		private void OnInnerControlContentChangedPlatformSpecific(bool suppressBindingNotifications) { }
		private System.Drawing.Point GetPhysicalPoint(System.Drawing.Point point) {
			if (DocumentModel == null)
				return new System.Drawing.Point(0, 0);
			int x = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(point.X, DpiX);
			int y = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(point.Y, DpiY);
			x = (int)(x * DocumentModel.Dpi / 96.0) - ViewBounds.Left;
			y = (int)(y * DocumentModel.Dpi / 96.0) - ViewBounds.Top;
			return new System.Drawing.Point(x, y);
		}
		internal void SetFocus() {
			if (!InnerControl.IsAnyInplaceEditorActive && !InnerControl.IsDataValidationInplaceEditorActive)
				this.Focus();
		}
		protected internal void SubscribeInnerEventsPlatformSpecific() {
			this.IsEnabledChanged += OnIsEnabledChanged;
			InnerControl.DocumentModel.LayoutUnitChanged += LayoutUnitChangedHandler.Handler;
		}
		protected internal void UnsubscribeInnerEventsPlatformSpecific() {
			this.IsEnabledChanged -= OnIsEnabledChanged;
			InnerControl.DocumentModel.LayoutUnitChanged -= LayoutUnitChangedHandler.Handler;
		}
		private void RemoveServices() {
			if (GetService<ISpreadsheetCommandFactoryService>() != null)
				RemoveService(typeof(ISpreadsheetCommandFactoryService));
			if (GetService<ISpreadsheetPrintingService>() != null)
				RemoveService(typeof(ISpreadsheetPrintingService));
			if (GetService<IChartControllerFactoryService>() != null)
				RemoveService(typeof(IChartControllerFactoryService));
			if (GetService<IChartImageService>() != null)
				RemoveService(typeof(IChartImageService));
			if (GetService<ISpreadsheetContextMenuService>() != null)
				RemoveService(typeof(ISpreadsheetContextMenuService));
		}
		void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			OnEnabledChanged();
		}
		internal SpreadsheetView GetCurrentView() {
			return this.InnerControl.Views.NormalView;
		}
		internal SpreadsheetViewControl ViewControl { get { return spreadsheetViewControl; } }
		internal SpreadsheetHitTestType GetHitTest(System.Windows.Point pointRelativelySpreadsheetControl) {
			return ViewControl.GetHitTest(pointRelativelySpreadsheetControl);
		}
		internal Bars.BarManagerMenuController GetMenuController() {
			ISpreadsheetContextMenuService service = GetService<ISpreadsheetContextMenuService>();
			return service != null ? service.GetMenuController(this) : null;
		}
		EventHandler innerControlInitialized;
		internal event EventHandler InnerControlInitialized {
			add { innerControlInitialized += value; }
			remove { innerControlInitialized -= value; }
		}
		private void RaiseInnerControlInitialized() {
			if (innerControlInitialized != null)
				innerControlInitialized(this, EventArgs.Empty);
		}
		internal void OnPopupContextMenuShowing(PopupMenuShowingEventArgs args) {
			if (popupMenuShowing != null)
				popupMenuShowing(this, args);
		}
		#region IOfficeFontSizeProvider Members
		List<int> IOfficeFontSizeProvider.GetPredefinedFontSizes() {
			return InnerControl != null ? InnerControl.PredefinedFontSizeCollection.ToList() : new List<int>();
		}
		#endregion
	}
	public enum ShowCellToolTipMode { Never, Always, Auto }
}
