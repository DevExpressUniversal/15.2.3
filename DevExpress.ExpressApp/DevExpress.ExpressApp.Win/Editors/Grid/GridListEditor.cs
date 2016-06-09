#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors.Grid;
using DevExpress.ExpressApp.Win.Model;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
namespace DevExpress.ExpressApp.Win.Editors {
	public enum FilterColumnsMode { AllProperties, ColumnsOnly }
	public enum AppearanceFocusedCellMode { Smart, Enabled, Disabled }
	public enum GridListEditorNewItemRowHandlingMode { NativeControl, XafController }
	public class GetColumnErrorEventArgs : HandledEventArgs {
		public object TargetObject { get; private set; }
		public string PropertyName { get; private set; }
		public ErrorType ErrorType { get; set; }
		public string Message { get; set; }
		public GetColumnErrorEventArgs(object target, string propertyName) {
			this.TargetObject = target;
			this.PropertyName = propertyName;
		}
	}
	public class GridListEditor : WinColumnsListEditor, ISupportNewItemRowPosition, ISupportFooter, ILookupListEditor, IGridListEditorTestable, ILookupEditProvider, IContextMenuTarget, IConfigurableLookupListEditor {
		private InternalXafWinFilterTreeNodeModel model;
		private NewItemRowPosition newItemRowPosition = NewItemRowPosition.None;
		private Boolean trackMousePosition;
		private TimeLatch moveRowFocusSpeedLimiter;
		private Boolean processSelectedItemBySingleClick;
		private bool selectedItemExecuting;
		private AppearanceFocusedCellMode appearanceFocusedCellMode = AppearanceFocusedCellMode.Smart;
		private RepositoryItem activeEditor;
		private int mouseDownTime;
		private int mouseUpTime;
		private bool contextMenuEnabled = true;
		private event EventHandler contextMenuEnabledChanged;
		private GridListEditorNewItemRowHandlingMode newItemRowHandlingMode;
		public static GridListEditorNewItemRowHandlingMode DefaultNewItemRowHandlingMode = GridListEditorNewItemRowHandlingMode.XafController;
		private IDisposable parseCriteriaScope; 
		public GridListEditor(IModelListView model)
			: base(model) {
			moveRowFocusSpeedLimiter = new TimeLatch();
			moveRowFocusSpeedLimiter.TimeoutInMilliseconds = 100;
			newItemRowHandlingMode = DefaultNewItemRowHandlingMode;
		}
		public GridView GridView {
			get {
				return (GridView)ColumnView;
			}
		}
		public GridListEditorNewItemRowHandlingMode NewItemRowHandlingMode {
			get { return newItemRowHandlingMode; }
			set {
				newItemRowHandlingMode = value;
				if(GridView is XafGridView) {
					((XafGridView)GridView).NewItemRowHandlingMode = value;
				}
				else if(GridView is XafBandedGridView) {
					((XafBandedGridView)GridView).NewItemRowHandlingMode = value;
				}
			}
		}
		public NewItemRowPosition NewItemRowPosition {
			get { return newItemRowPosition; }
			set {
				if(newItemRowPosition != value) {
					newItemRowPosition = value;
					SetNewItemRow(GridView);
				}
			}
		}
		public Boolean TrackMousePosition {
			get { return trackMousePosition; }
			set { trackMousePosition = value; }
		}
		public Boolean ProcessSelectedItemBySingleClick {
			get { return processSelectedItemBySingleClick; }
			set { processSelectedItemBySingleClick = value; }
		}
		public AppearanceFocusedCellMode AppearanceFocusedCellMode {
			get {
				return appearanceFocusedCellMode;
			}
			set {
				if(appearanceFocusedCellMode != value) {
					appearanceFocusedCellMode = value;
					UpdateAppearanceFocusedCell();
				}
			}
		}
		public override bool CanShowPopupMenu(Point position) {
			if(CanShowBands) {
				BandedGridHitTest hitTest = ((BandedGridView)GridView).CalcHitInfo(Grid.PointToClient(position)).HitTest;
				return
					 ((hitTest == BandedGridHitTest.Row)
					 || (hitTest == BandedGridHitTest.RowCell)
					 || (hitTest == BandedGridHitTest.EmptyRow)
					 || (hitTest == BandedGridHitTest.None));
			}
			else {
				GridHitTest hitTest = GridView.CalcHitInfo(Grid.PointToClient(position)).HitTest;
				return
					 ((hitTest == GridHitTest.Row)
					 || (hitTest == GridHitTest.RowCell)
					 || (hitTest == GridHitTest.EmptyRow)
					 || (hitTest == GridHitTest.None));
			}
		}
		public override object FocusedObject {
			get {
				object result = null;
				if(GridView != null) {
					result = XtraGridUtils.GetFocusedRowObject(CollectionSource, GridView);
				}
				return result;
			}
			set {
				if(value != null && value != DBNull.Value && GridView != null && DataSource != null) {
					int dataSourceIndex = List.IndexOf(value);
					if(dataSourceIndex >= 0 && object.ReferenceEquals(value, List[dataSourceIndex])) {
						XtraGridUtils.SelectRowByHandle(GridView, GridView.GetRowHandle(dataSourceIndex));
						if(XtraGridUtils.HasValidRowHandle(GridView)) {
							GridView.SetRowExpanded(GridView.FocusedRowHandle, true, true);
						}
					}
				}
			}
		}
		public override void StartIncrementalSearch(string searchString) {
			GridColumn defaultColumn = GetDefaultColumn();
			if(defaultColumn != null) {
				GridView.FocusedColumn = defaultColumn;
			}
			GridView.StartIncrementalSearch(searchString);
		}
		public override void ApplyModel() {
			if(Model != null) {
				CancelEventArgs args = new CancelEventArgs();
				OnModelApplying(args);
				if(!args.Cancel) {
					Grid.BeginUpdate();
					ColumnView.BeginInit();
					try {
						GridControlModelSynchronizer.ApplyModel(Model, this);
						UpdateAppearanceFocusedCell();
						base.ApplyModel();
					}
					finally {
						ColumnView.EndInit();
						Grid.EndUpdate();
					}
					OnModelApplied();
				}
			}
		}
		public override void SaveModel() {
			if((Model != null) && (GridView != null)) {
				CancelEventArgs args = new CancelEventArgs();
				OnModelSaving(args);
				if(!args.Cancel) {
					GridControlModelSynchronizer.SaveModel(Model, this);
					base.SaveModel();
					OnModelSaved();
				}
			}
		}
		void IConfigurableLookupListEditor.Setup() {
			new RemotingCompatibilityHelper().Attach(GridView);
		}
		protected override ColumnView CreateGridViewCore() {
			if(CanShowBands) {
				XafBandedGridView result = new XafBandedGridView();
				result.OptionsView.ColumnAutoWidth = true;
				result.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
				result.NewItemRowHandlingMode = newItemRowHandlingMode;
				return result;
			}
			else {
				XafGridView result = new XafGridView();
				result.NewItemRowHandlingMode = newItemRowHandlingMode;
				return result;
			}
		}
		protected override void SetupGridView() {
			base.SetupGridView();
			if(GridView.DataController != null) {
				GridView.DataController.AllowCurrentRowObjectForGroupRow = false;
			}
			SetNewItemRow(GridView);
			if(GridView is IXafGridView) {
				((IXafGridView)GridView).ErrorMessages = ErrorMessages;
			}
		}
		protected override void SubscribeGridViewEvents() {
			base.SubscribeGridViewEvents();
			GridView.RowCellStyle += new RowCellStyleEventHandler(gridView_RowCellStyle);
			GridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(gridView_ShowGridMenu);
			GridView.CalcPreviewText += new CalcPreviewTextEventHandler(gridView_CalcPreviewText);
			GridView.ShowCustomizationForm += new EventHandler(gridView_ShowCustomizationForm);
			GridView.HideCustomizationForm += new EventHandler(gridView_HideCustomizationForm);
			GridView.MouseMove += new MouseEventHandler(gridView_MouseMove);
			GridView.Click += new EventHandler(gridView_Click);
			GridView.MouseWheel += new MouseEventHandler(gridView_MouseWheel);
			GridView.ColumnChanged += new EventHandler(gridView_ColumnChanged);
			GridView.ShowingEditor += new CancelEventHandler(gridView_EditorShowing);
			GridView.HiddenEditor += new EventHandler(gridView_HiddenEditor);
			GridView.MouseDown += new MouseEventHandler(gridView_MouseDown);
			GridView.MouseUp += new MouseEventHandler(gridView_MouseUp);
			GridView.ShownEditor += new EventHandler(gridView_ShownEditor);
			GridView.Disposed += GridView_Disposed;
			IXafGridView xafGridView = GridView as IXafGridView;
			if(xafGridView != null) {
				xafGridView.GetGridViewColumnError += new EventHandler<GetGridViewColumnErrorEventArgs>(gridView_GetGridViewColumnError);
			}
			ICustomizeFilterColumns gridViewCustomizeFilterColumns = GridView as ICustomizeFilterColumns;
			if(gridViewCustomizeFilterColumns != null) {
				gridViewCustomizeFilterColumns.FilterEditorPopup += new EventHandler(gridView_FilterEditorPopup);
				gridViewCustomizeFilterColumns.FilterEditorClosed += new EventHandler(gridView_FilterEditorClosed);
				if(FilterColumnsMode == FilterColumnsMode.AllProperties) {
					gridViewCustomizeFilterColumns.CreateCustomFilterColumnCollection += new EventHandler<CreateCustomFilterColumnCollectionEventArgs>(gridview_CreateCustomFilterColumnCollection);
					gridViewCustomizeFilterColumns.CustomizeFilterFromFilterBuilder += new EventHandler<CustomizeFilterFromFilterBuilderEventArgs>(gridView_CustomizeFilterFromFilterBuilder);
				}
			}
		}
		protected override void UnsubscribeGridViewEvents() {
			base.UnsubscribeGridViewEvents();
			GridView.RowCellStyle -= new RowCellStyleEventHandler(gridView_RowCellStyle);
			GridView.PopupMenuShowing -= new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(gridView_ShowGridMenu);
			GridView.CalcPreviewText -= new CalcPreviewTextEventHandler(gridView_CalcPreviewText);
			GridView.ShowCustomizationForm -= new EventHandler(gridView_ShowCustomizationForm);
			GridView.HideCustomizationForm -= new EventHandler(gridView_HideCustomizationForm);
			GridView.InitNewRow -= new InitNewRowEventHandler(gridView_InitNewRow);
			GridView.MouseMove -= new MouseEventHandler(gridView_MouseMove);
			GridView.Click -= new EventHandler(gridView_Click);
			GridView.MouseWheel -= new MouseEventHandler(gridView_MouseWheel);
			GridView.ColumnChanged -= new EventHandler(gridView_ColumnChanged);
			GridView.ShowingEditor -= new CancelEventHandler(gridView_EditorShowing);
			GridView.HiddenEditor -= new EventHandler(gridView_HiddenEditor);
			GridView.MouseDown -= new MouseEventHandler(gridView_MouseDown);
			GridView.MouseUp -= new MouseEventHandler(gridView_MouseUp);
			GridView.ShownEditor -= new EventHandler(gridView_ShownEditor);
			GridView.Disposed -= GridView_Disposed;
			ISupportNewItemRow gridViewSupportNewItemRow = GridView as ISupportNewItemRow;
			if(gridViewSupportNewItemRow != null) {
				gridViewSupportNewItemRow.CancelNewRow -= new EventHandler(gridView_CancelNewRow);
			}
			ICustomizeFilterColumns gridViewCustomizeFilterColumns = GridView as ICustomizeFilterColumns;
			if(gridViewCustomizeFilterColumns != null) {
				gridViewCustomizeFilterColumns.FilterEditorPopup -= new EventHandler(gridView_FilterEditorPopup);
				gridViewCustomizeFilterColumns.FilterEditorClosed -= new EventHandler(gridView_FilterEditorClosed);
				gridViewCustomizeFilterColumns.CreateCustomFilterColumnCollection -= new EventHandler<CreateCustomFilterColumnCollectionEventArgs>(gridview_CreateCustomFilterColumnCollection);
				gridViewCustomizeFilterColumns.CustomizeFilterFromFilterBuilder -= new EventHandler<CustomizeFilterFromFilterBuilderEventArgs>(gridView_CustomizeFilterFromFilterBuilder);
			}
			IXafGridView xafGidView = GridView as IXafGridView;
			if(xafGidView != null) {
				xafGidView.GetGridViewColumnError -= new EventHandler<GetGridViewColumnErrorEventArgs>(gridView_GetGridViewColumnError);
			}
		}
		protected override void SubscribeToGridEvents() {
			base.SubscribeToGridEvents();
			Grid.DoubleClick += new EventHandler(grid_DoubleClick);
			Grid.Paint += new PaintEventHandler(grid_Paint);
		}
		protected override void UnsubscribeFromGridEvents() {
			base.UnsubscribeFromGridEvents();
			Grid.DoubleClick -= new EventHandler(grid_DoubleClick);
			Grid.Paint -= new PaintEventHandler(grid_Paint);
		}
		protected override void SetGridViewOptions() {
			base.SetGridViewOptions();
			GridView.OptionsBehavior.AllowIncrementalSearch = !AllowEdit || ReadOnlyEditors;
			GridView.OptionsSelection.EnableAppearanceFocusedCell = true;
			GridView.OptionsNavigation.AutoFocusNewRow = true;
			GridView.OptionsNavigation.AutoMoveRowFocus = true;
			GridView.OptionsView.ShowDetailButtons = false;
			GridView.OptionsDetail.EnableMasterViewMode = false;
			GridView.OptionsView.ShowIndicator = true;
			GridView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
			GridView.OptionsMenu.ShowGroupSummaryEditorItem = true;
		}
		protected override void SubscribeToDataControllerEvents() {
			base.SubscribeToDataControllerEvents();
			if(GridView.DataController is XafCurrencyDataController) {
				((XafCurrencyDataController)GridView.DataController).NewItemRowObjectCustomAdding += new HandledEventHandler(gridView_DataController_NewItemRowObjectAdding);
			}
		}
		protected override void UnsubscribeFromDataControllerEvents() {
			base.UnsubscribeFromDataControllerEvents();
			if(GridView.DataController is XafCurrencyDataController) {
				((XafCurrencyDataController)GridView.DataController).NewItemRowObjectCustomAdding -= new HandledEventHandler(gridView_DataController_NewItemRowObjectAdding);
			}
		}
		protected override void ApplyHtmlFormatting(bool htmlFormattingEnabled) {
			base.ApplyHtmlFormatting(htmlFormattingEnabled);
			GridView.OptionsView.AllowHtmlDrawHeaders = htmlFormattingEnabled;
			GridView.Appearance.HeaderPanel.TextOptions.WordWrap = htmlFormattingEnabled ? WordWrap.Wrap : WordWrap.Default;
		}
		protected override void OnProcessSelectedItem() {
			selectedItemExecuting = true;
			try {
				if((GridView != null) && (GridView.ActiveEditor != null)) {
					BindingHelper.EndCurrentEdit(Grid);
				}
				base.OnProcessSelectedItem();
			}
			finally {
				selectedItemExecuting = false;
			}
		}
		protected override void UpdateAllowEditGridViewAndColumnsOptions() {
			base.UpdateAllowEditGridViewAndColumnsOptions();
			GridView.OptionsBehavior.AllowIncrementalSearch = !AllowEdit || ReadOnlyEditors;
		}
		public override void BreakLinksToControls() {
			DisposeParseCriteriaScope();
			base.BreakLinksToControls();
		}
		protected virtual void CustomizeGridMenu(DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e) {
			if(e.MenuType == GridMenuType.Summary) {
				IGridColumnModelSynchronizer info = GetColumnModelSynchronizer(e.HitInfo.Column);
				if(info != null) {
					e.Allow = info.AllowSummaryChange;
				}
			}
		}
		protected virtual void ProcessMouseClick(EventArgs e) {
			if(!selectedItemExecuting) {
				if(GridView.FocusedRowHandle >= 0) {
					DXMouseEventArgs args = DXMouseEventArgs.GetMouseArgs(Grid, e);
					if(args.Button == MouseButtons.Left) {
						GridHitInfo hitInfo = GridView.CalcHitInfo(args.Location);
						if(hitInfo.InRow) {
							args.Handled = true;
							OnProcessSelectedItem();
						}
					}
				}
			}
		}
		protected virtual void ProcessEditorKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter) {
				if((ColumnView.ActiveEditor != null) && ColumnView.ActiveEditor.IsModified) {
					ColumnView.PostEditor();
					ColumnView.UpdateCurrentRow();
				}
			}
		}
		protected RepositoryItem CreateDefaultFilterControlRepositoryItem(DataColumnInfoFilterColumn column) {
			if(GridView != null && column != null) {
				if(column.Parent == null || !column.Parent.IsList) { 
					IMemberInfo memberInfo = ObjectTypeInfo.FindMember(column.FullName);
					if(memberInfo != null) {
						GridColumn gridColumn = GridView.Columns[memberInfo.BindingName]; 
						if(gridColumn != null) {
							return new GridFilterColumn(gridColumn).ColumnEditor;
						}
					}
				}
				if(RepositoryFactory != null && column.ColumnType != null) {
					return RepositoryFactory.CreateStandaloneRepositoryItem(column.ColumnType);
				}
			}
			return null;
		}
		protected static CriteriaOperator CustomizeFilterFromFilterBuilder(IEnumerable columns, FilterColumnsMode filterColumnsMode, CriteriaOperator criteria) {
			CriteriaOperator result = criteria;
			if(filterColumnsMode == FilterColumnsMode.AllProperties) {
				DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(columns, "GridView");
				List<string> existingLookupFieldNames = new List<string>();
				foreach(GridColumn col in columns) {
					if(!string.IsNullOrEmpty(col.FieldName) && col.FieldName.EndsWith("!")) {
						existingLookupFieldNames.Add(col.FieldName);
					}
				}
				if(existingLookupFieldNames.Count > 0) {
					result = CriteriaOperator.Clone(criteria);
					new PatchXpoSpecificFieldNameForGridCriteriaProcessor(existingLookupFieldNames).Process(result);
				}
			}
			return result;
		}
		private void gridView_CalcPreviewText(object sender, CalcPreviewTextEventArgs e) {
			if(GridView.Columns[GridView.PreviewFieldName] != null) {
				e.PreviewText = GridView.GetRowCellDisplayText(e.RowHandle, GridView.Columns[GridView.PreviewFieldName]);
			}
		}
		private void gridView_InitNewRow(object sender, InitNewRowEventArgs e) {
			OnNewObjectCreated();
		}
		private void gridView_CancelNewRow(object sender, EventArgs e) {
			OnNewObjectCanceled();
		}
		private void gridView_RowCellStyle(object sender, RowCellStyleEventArgs e) {
			if(e.RowHandle != GridControl.AutoFilterRowHandle) {
				IGridColumnModelSynchronizer info = GetColumnModelSynchronizer(e.Column);
				string propertyName = info != null ? info.PropertyName : e.Column.FieldName;
				OnCustomizeAppearance(propertyName, new GridViewRowCellStyleEventArgsAppearanceAdapter((GridView)sender, e), e.RowHandle);
			}
		}
		private void gridView_ShowGridMenu(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e) {
			CustomizeGridMenu(e);
		}
		private void gridView_HideCustomizationForm(object sender, EventArgs e) {
			OnEndCustomization();
		}
		private void gridView_ShowCustomizationForm(object sender, EventArgs e) {
			OnBeginCustomization();
		}
		private void gridView_FilterEditorPopup(object sender, EventArgs e) {
			CreateParseCriteriaScope();
			OnBeginCustomization();
		}
		private void gridView_FilterEditorClosed(object sender, EventArgs e) {
			OnEndCustomization();
			DisposeParseCriteriaScope();
		}
		private void gridview_CreateCustomFilterColumnCollection(object sender, CreateCustomFilterColumnCollectionEventArgs e) {
			if(Model != null && Model.DataAccessMode == CollectionSourceDataAccessMode.DataView) {
				return;
			}
			if(FilterColumnsMode == FilterColumnsMode.AllProperties) {
				if(model == null) {
					model = new InternalXafWinFilterTreeNodeModel();
					model.CreateCustomRepositoryItem += new EventHandler<DevExpress.XtraEditors.Filtering.CreateCustomRepositoryItemEventArgs>(Model_CreateCustomRepositoryItem);
					model.SourceControl = DevExpress.ExpressApp.Editors.CriteriaPropertyEditorHelper.CreateFilterControlDataSource(ObjectType, Application != null ? Application : null);
				}
				e.FilterColumnCollection = model.CreateFilterColumnCollection();
			}
		}
		private void gridView_GetGridViewColumnError(object sender, GetGridViewColumnErrorEventArgs e) {
			e.Handled = true;
			GridView gridView = (GridView)sender;
			object target = gridView.GetRow(e.RowHandle);
			string propertyName = "";			
			if(e.Column != null) {
				propertyName = e.Column.Name;
			}
			GetColumnErrorEventArgs args = new GetColumnErrorEventArgs(target, propertyName);			
			OnGetColumnError(args);
			if(args.Handled) {
				e.ErrorType = args.ErrorType;
				e.Message = args.Message;
			}
			else {
				ErrorMessage errorMessage = null;
				if(!string.IsNullOrEmpty(propertyName)) {
					errorMessage = ErrorMessages.GetMessage(propertyName, target);
				}
				else {
					errorMessage = ErrorMessages.GetMessages(target);
				}
				if(errorMessage != null) {
					e.ErrorType = XtraGridUtils.GetErrorTypeByName(errorMessage.Icon.ImageName);
					e.Message = errorMessage.Message;
				}
			}
		}
		private void gridView_CustomizeFilterFromFilterBuilder(object sender, CustomizeFilterFromFilterBuilderEventArgs e) {
			e.Criteria = CustomizeFilterFromFilterBuilder(GridView.Columns, FilterColumnsMode, e.Criteria);
		}
		private void gridView_DataController_NewItemRowObjectAdding(object sender, HandledEventArgs e) {
			e.Handled = OnNewObjectAdding() != null;
		}
		private void gridView_MouseMove(object sender, MouseEventArgs e) {
			if(trackMousePosition || scrollOnMouseMove) {
				GridHitInfo hitInfo = GridView.CalcHitInfo(e.Location);
				if(hitInfo.InRow) {
					Boolean isTimeIntervalExpired = moveRowFocusSpeedLimiter.IsTimeIntervalExpired;
					if(isTimeIntervalExpired) {
						moveRowFocusSpeedLimiter.ResetLastEventTime();
					}
					if(hitInfo.RowHandle == GridView.TopRowIndex) {
						if(scrollOnMouseMove && isTimeIntervalExpired && (GridView.TopRowIndex != 0)) {
							if(trackMousePosition) {
								SelectRowForMousePositionTracking(GridView.TopRowIndex - 1);
							}
							GridView.TopRowIndex--;
						}
						else if(trackMousePosition && (GridView.FocusedRowHandle != GridView.TopRowIndex)) {
							SelectRowForMousePositionTracking(GridView.TopRowIndex);
						}
					}
					else {
						RowVisibleState rowVisibleState = GridView.IsRowVisible(hitInfo.RowHandle);
						if(IsLastVisibleRow(hitInfo, rowVisibleState)) {
							if(scrollOnMouseMove && isTimeIntervalExpired && (hitInfo.RowHandle < GridView.RowCount - 1)) {
								GridView.TopRowIndex++;
								if(trackMousePosition) {
									if(rowVisibleState == RowVisibleState.Partially) {
										SelectRowForMousePositionTracking(hitInfo.RowHandle);
									}
									else {
										SelectRowForMousePositionTracking(hitInfo.RowHandle + 1);
									}
								}
							}
							else if(trackMousePosition && (GridView.FocusedRowHandle != hitInfo.RowHandle)) {
								if(rowVisibleState == RowVisibleState.Visible) {
									SelectRowForMousePositionTracking(hitInfo.RowHandle);
								}
								else if(rowVisibleState == RowVisibleState.Partially) {
									SetSkipMakeRowVisible(true);
									try {
										SelectRowForMousePositionTracking(hitInfo.RowHandle);
									}
									finally {
										SetSkipMakeRowVisible(false);
									}
								}
							}
						}
						else {
							if(trackMousePosition && (GridView.FocusedRowHandle != hitInfo.RowHandle)) {
								SelectRowForMousePositionTracking(hitInfo.RowHandle);
							}
						}
					}
				}
			}
		}
		private void gridView_Click(Object sender, EventArgs e) {
			if(processSelectedItemBySingleClick) {
				ProcessMouseClick(e);
			}
		}
		private void gridView_MouseWheel(object sender, MouseEventArgs e) {
			moveRowFocusSpeedLimiter.Reset();
		}
		private void gridView_ColumnChanged(object sender, EventArgs e) {
			UpdateAppearanceFocusedCell();
		}
		private void gridView_MouseDown(object sender, MouseEventArgs e) {
			GridView view = (GridView)sender;
			GridHitInfo hi = view.CalcHitInfo(new Point(e.X, e.Y));
			if(hi.RowHandle >= 0) {
				mouseDownTime = System.Environment.TickCount;
			}
			else {
				mouseDownTime = 0;
			}
		}
		private void GridView_Disposed(object sender, EventArgs e) {
			DisposeParseCriteriaScope();
		}
		private void gridView_EditorShowing(object sender, CancelEventArgs e) {
			activeEditor = null;
			IGridColumnModelSynchronizer info = GetColumnModelSynchronizer(ColumnView.FocusedColumn);
			string propertyName = info != null ? info.PropertyName : ColumnView.FocusedColumn.FieldName;
			OnCustomizeEnabled(propertyName, new GridViewCancelEventArgsAppearanceAdapter((GridView)sender, e), ColumnView.FocusedRowHandle);
			if(e.Cancel) {
				return;
			}
			OnCustomizeAppearance(propertyName, new GridViewCancelEventArgsAppearanceAdapter((GridView)sender, e), ColumnView.FocusedRowHandle);
			if(!e.Cancel) {
				RepositoryItem edit = ColumnView.FocusedColumn.ColumnEdit;
				if(edit != null) {
					OnCustomizeAppearance(propertyName, new GridViewCancelEventArgsAppearanceAdapterWithReset((GridView)sender, e, edit.Appearance), ColumnView.FocusedRowHandle);
					edit.MouseDown += new MouseEventHandler(Editor_MouseDown);
					edit.MouseUp += new MouseEventHandler(Editor_MouseUp);
					RepositoryItemButtonEdit buttonEdit = edit as RepositoryItemButtonEdit;
					if(buttonEdit != null) {
						buttonEdit.ButtonPressed += new ButtonPressedEventHandler(ButtonEdit_ButtonPressed);
					}
					RepositoryItemBaseSpinEdit spinEdit = edit as RepositoryItemBaseSpinEdit;
					if(spinEdit != null) {
						spinEdit.Spin += new SpinEventHandler(SpinEdit_Spin);
					}
					activeEditor = edit;
				}
			}
		}
		private void gridView_ShownEditor(object sender, EventArgs e) {
			SetContextMenuEnabled(false);
			if(PopupMenu != null) {
				PopupMenu.ResetPopupContextMenuSite();
			}
			LookupEdit editor = ColumnView.ActiveEditor as LookupEdit;
			if(editor != null) {
				OnLookupEditCreated(editor);
			}
		}
		private void gridView_HiddenEditor(object sender, EventArgs e) {
			SetContextMenuEnabled(true);
			if(PopupMenu != null) {
				PopupMenu.SetupPopupContextMenuSite();
			}
			LookupEdit editor = ColumnView.ActiveEditor as LookupEdit;
			if(editor != null) {
				OnLookupEditHide(editor);
			}
			if(activeEditor != null) {
				activeEditor.MouseDown -= new MouseEventHandler(Editor_MouseDown);
				activeEditor.MouseUp -= new MouseEventHandler(Editor_MouseUp);
				RepositoryItemButtonEdit buttonEdit = activeEditor as RepositoryItemButtonEdit;
				if(buttonEdit != null) {
					buttonEdit.ButtonPressed -= new ButtonPressedEventHandler(ButtonEdit_ButtonPressed);
				}
				RepositoryItemBaseSpinEdit spinEdit = activeEditor as RepositoryItemBaseSpinEdit;
				if(spinEdit != null) {
					spinEdit.Spin -= new SpinEventHandler(SpinEdit_Spin);
				}
				activeEditor.Appearance.Reset();
				activeEditor = null;
			}
		}
		private void gridView_MouseUp(object sender, MouseEventArgs e) {
			mouseUpTime = System.Environment.TickCount;
		}
		private void Editor_MouseDown(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				Int32 currentTime = System.Environment.TickCount;
				if((mouseDownTime <= mouseUpTime) && (mouseUpTime <= currentTime) && (currentTime - mouseDownTime < SystemInformation.DoubleClickTime)) {
					OnProcessSelectedItem();
					mouseDownTime = 0;
				}
			}
		}
		private void Editor_MouseUp(object sender, MouseEventArgs e) {
			mouseUpTime = System.Environment.TickCount;
		}
		private void SpinEdit_Spin(object sender, SpinEventArgs e) {
			mouseDownTime = 0;
		}
		private void ButtonEdit_ButtonPressed(object sender, ButtonPressedEventArgs e) {
			mouseDownTime = 0;
		}
		private void Model_CreateCustomRepositoryItem(object sender, DevExpress.XtraEditors.Filtering.CreateCustomRepositoryItemEventArgs e) {
			if(CreateCustomFilterEditorRepositoryItem != null) {
				CreateCustomFilterEditorRepositoryItem(this, e);
			}
			if(e.RepositoryItem == null) {
				e.RepositoryItem = CreateDefaultFilterControlRepositoryItem(e.Column);
			}
		}
		private void grid_DoubleClick(object sender, EventArgs e) {
			ProcessMouseClick(e);
		}
		private void grid_Paint(object sender, PaintEventArgs e) {
			Grid.Paint -= new PaintEventHandler(grid_Paint);
			UpdateAppearanceFocusedCell();
		}
		private void SetNewItemRow(GridView gridView) {
			if(gridView == null) {
				return;
			}
			gridView.InitNewRow -= new InitNewRowEventHandler(gridView_InitNewRow);
			if(gridView is ISupportNewItemRow) {
				((ISupportNewItemRow)gridView).CancelNewRow -= new EventHandler(gridView_CancelNewRow);
			}
			UnsubscribeFromDataControllerEvents();
			gridView.OptionsView.NewItemRowPosition = (DevExpress.XtraGrid.Views.Grid.NewItemRowPosition)Enum.Parse(typeof(DevExpress.XtraGrid.Views.Grid.NewItemRowPosition), newItemRowPosition.ToString());
			if(newItemRowPosition != NewItemRowPosition.None) {
				gridView.InitNewRow += new InitNewRowEventHandler(gridView_InitNewRow);
				if(gridView is ISupportNewItemRow) {
					((ISupportNewItemRow)gridView).CancelNewRow += new EventHandler(gridView_CancelNewRow);
				}
				SubscribeToDataControllerEvents();
			}
		}
		private Boolean IsLastVisibleRow(GridHitInfo hitInfo, RowVisibleState rowVisibleState) {
			Boolean result = false;
			if(hitInfo.RowHandle >= 0) {
				if(rowVisibleState == RowVisibleState.Partially) {
					result = true;
				}
				else if(rowVisibleState == RowVisibleState.Visible) {
					if(hitInfo.RowHandle < GridView.RowCount - 1) {
						RowVisibleState nextRowVisibleState = GridView.IsRowVisible(hitInfo.RowHandle + 1);
						if(nextRowVisibleState == RowVisibleState.Hidden) {
							result = true;
						}
					}
				}
			}
			return result;
		}
		private void SetSkipMakeRowVisible(bool skip) {
			IXafGridView xafGridView = GridView as IXafGridView;
			if(xafGridView != null) {
				xafGridView.SkipMakeRowVisible = skip;
			}
		}
		private void SelectRowForMousePositionTracking(Int32 rowHandle) {
			if(rowHandle != GridControl.AutoFilterRowHandle) {
				XtraGridUtils.SelectRowByHandle(GridView, rowHandle, (GridView.FocusedRowHandle != GridControl.AutoFilterRowHandle));
			}
		}
		private void UpdateAppearanceFocusedCell() {
			if(GridView != null) {
				switch(AppearanceFocusedCellMode) {
					case AppearanceFocusedCellMode.Smart:
						GridView.OptionsSelection.EnableAppearanceFocusedCell = GridView.VisibleColumns.Count > 1;
						break;
					case AppearanceFocusedCellMode.Enabled:
						GridView.OptionsSelection.EnableAppearanceFocusedCell = true;
						break;
					case AppearanceFocusedCellMode.Disabled:
						GridView.OptionsSelection.EnableAppearanceFocusedCell = false;
						break;
				}
			}
		}
		private void OnBeginCustomization() {
			if(BeginCustomization != null) {
				BeginCustomization(this, EventArgs.Empty);
			}
		}
		private void OnEndCustomization() {
			if(EndCustomization != null) {
				EndCustomization(this, EventArgs.Empty);
			}
		}
		private void CreateParseCriteriaScope() {
			if(CollectionSource != null && CollectionSource.ObjectSpace != null) {
				if(parseCriteriaScope != null) {
					throw new InvalidOperationException("Cannot create parse criteria scope. Another instance of parse criteria scope has already been created.");
				}
				parseCriteriaScope = CollectionSource.ObjectSpace.CreateParseCriteriaScope();
			}
		}
		private void DisposeParseCriteriaScope() {
			if(parseCriteriaScope != null) {
				parseCriteriaScope.Dispose();
				parseCriteriaScope = null;
			}
		}
		bool ISupportFooter.IsFooterVisible {
			get {
				return GridView.OptionsView.ShowFooter;
			}
			set {
				GridView.OptionsView.ShowFooter = value;
			}
		}
		void IGridListEditorTestable.CustomizeGridMenu(DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e) {
			CustomizeGridMenu(e);
		}
		public event EventHandler BeginCustomization;
		public event EventHandler EndCustomization;
		public event EventHandler<DevExpress.XtraEditors.Filtering.CreateCustomRepositoryItemEventArgs> CreateCustomFilterEditorRepositoryItem;
		public event EventHandler<GetColumnErrorEventArgs> GetColumnError;
		#region ILookupEditProvider Members
		private event EventHandler<LookupEditProviderEventArgs> lookupEditCreated;
		private event EventHandler<LookupEditProviderEventArgs> lookupEditHide;
		protected void OnGetColumnError(GetColumnErrorEventArgs args) {
			if(GetColumnError != null) {
				GetColumnError(this, args);
			}
		}
		protected void OnLookupEditCreated(LookupEdit control) {
			if(lookupEditCreated != null) {
				lookupEditCreated(this, new LookupEditProviderEventArgs(control));
			}
		}
		protected void OnLookupEditHide(LookupEdit control) {
			if(lookupEditHide != null) {
				lookupEditHide(this, new LookupEditProviderEventArgs(control));
			}
		}
		event EventHandler<LookupEditProviderEventArgs> ILookupEditProvider.ControlCreated {
			add { lookupEditCreated += value; }
			remove { lookupEditCreated -= value; }
		}
		event EventHandler<LookupEditProviderEventArgs> ILookupEditProvider.HideControl {
			add { lookupEditHide += value; }
			remove { lookupEditHide -= value; }
		}
		#endregion
		private void SetContextMenuEnabled(bool enabled) {
			if(contextMenuEnabled != enabled) {
				contextMenuEnabled = enabled;
				if(contextMenuEnabledChanged != null) {
					contextMenuEnabledChanged(this, EventArgs.Empty);
				}
			}
		}
		void IContextMenuTarget.SetMenuManager(IDXMenuManager menuManager) {
			if(Grid == null) {
				throw new InvalidOperationException("Cannot set the 'MenuManager' property because the 'GridListEditor.Grid' property is null");
			}
			Grid.MenuManager = menuManager;
		}
		bool IContextMenuTarget.CanShowContextMenu(Point position) {
			return CanShowPopupMenu(position);
		}
		Control IContextMenuTarget.ContextMenuSite {
			get { return Grid; }
		}
		bool IContextMenuTarget.ContextMenuEnabled {
			get { return contextMenuEnabled; }
		}
		event EventHandler IContextMenuTarget.ContextMenuEnabledChanged {
			add { contextMenuEnabledChanged += value; }
			remove { contextMenuEnabledChanged -= value; }
		}
		#region Obsolete 14.2
		private Boolean scrollOnMouseMove;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This method is not used.")]
		public Boolean ScrollOnMouseMove {
			get { return scrollOnMouseMove; }
			set { scrollOnMouseMove = value; }
		}
		#endregion
	}
	public interface IGridListEditorTestable {
		void CustomizeGridMenu(DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e);
	}
}
