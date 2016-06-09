#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardWin.Native.Printing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraDashboardLayout;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	[
	DXToolboxItem(false),
	DashboardItemDesigner(typeof(DashboardItemDesigner))
	]
	public abstract class DashboardItemViewer : DashboardUserControl, IDataPointInfoProvider, IExportOptionsOwner, IDashboardExportItem {
		public static Dictionary<string, Type> Repository { get; private set; }
		static DashboardItemViewer() {
			Repository = new Dictionary<string, Type>();
			Repository.Add(DashboardItemType.Pivot, typeof(PivotDashboardItemViewer));
			Repository.Add(DashboardItemType.Grid, typeof(GridDashboardItemViewer));
			Repository.Add(DashboardItemType.Chart, typeof(ChartDashboardItemViewer));
			Repository.Add(DashboardItemType.ScatterChart, typeof(ScatterChartDashboardItemViewer));
			Repository.Add(DashboardItemType.Pie, typeof(PieDashboardItemViewer));
			Repository.Add(DashboardItemType.Gauge, typeof(GaugeDashboardItemViewer));
			Repository.Add(DashboardItemType.Card, typeof(CardDashboardItemViewer));
			Repository.Add(DashboardItemType.Image, typeof(ImageDashboardItemViewer));
			Repository.Add(DashboardItemType.Text, typeof(TextBoxDashboardItemViewer));
			Repository.Add(DashboardItemType.ChoroplethMap, typeof(ChoroplethMapDashboardItemViewer));
			Repository.Add(DashboardItemType.GeoPointMap, typeof(GeoPointMapDashboardItemViewer));
			Repository.Add(DashboardItemType.BubbleMap, typeof(BubbleMapDashboardItemViewer));
			Repository.Add(DashboardItemType.PieMap, typeof(PieMapDashboardItemViewer));
			Repository.Add(DashboardItemType.RangeFilter, typeof(RangeFilterDashboardItemViewer));
			Repository.Add(DashboardItemType.Combobox, typeof(ComboBoxDashboardItemViewer));
			Repository.Add(DashboardItemType.ListBox, typeof(ListBoxDashboardItemViewer));
			Repository.Add(DashboardItemType.TreeView, typeof(TreeViewDashboardItemViewer));
			Repository.Add(DashboardItemType.Group, typeof(DashboardItemGroupViewer));
		}
		protected static bool ContainsContent(DashboardPaneContent paneContent, ContentType value) {
			return (paneContent.ContentType & value) == value;
		}
		DashboardViewer dashboardViewer;
		IDashboardItemViewerContainer itemContainer;
		IDashboardItemDesignerProvider designerProvider;
		Control viewControl;
		DashboardItemViewModel viewModel;
		DashboardItemCaptionViewModel captionViewModel;
		ConditionalFormattingModel conditionalFormattingModel;
		IList<IList> selectedValues;
		IList<object> drillDownUniqueValues;
		IList<FormattableValue> drillDownValues;
		MultiDimensionalData multiDimensionalData;
		string[] axisNames;
		string itemType;
		public Control ViewControl { get { return viewControl; } }
		public string DashboardItemName { get { return Name; } }
		public Image CaptionImage { get { return ItemContainer.Image; } set { ItemContainer.Image = value; } }
		public string DashboardItemCaption { get { return captionViewModel != null ? captionViewModel.Caption : null; } }
		public ClientHierarchicalMetadata HierarchicalMetadata { get { return multiDimensionalData.Metadata; } }
		public MultiDimensionalData MultiDimensionalData { get { return multiDimensionalData; } protected set { multiDimensionalData = value; } }
		public DashboardItemViewModel ViewModel { get { return viewModel; } protected set { viewModel = value; } }
		public DashboardItemCaptionViewModel CaptionViewModel { get { return captionViewModel; } }
		public ConditionalFormattingModel ConditionalFormattingModel { get { return conditionalFormattingModel; } }
		public IList<IList> SelectedValues { get { return selectedValues; } }
		protected IDashboardItemViewerContainer ItemContainer { get { return itemContainer; } }
		protected IDashboardItemDesignerProvider DesignerProvider { get { return designerProvider; } }
		protected DashboardViewer DashboardViewer { get { return dashboardViewer; } }
		protected IList<object> DrillDownUniqueValues { get { return drillDownUniqueValues; } }
		protected string[] AxisNames { get { return axisNames; } }
		protected virtual bool SupportsTransparentBackColor { get { return false; } }
		protected DashboardServiceClient ServiceClient { get { return dashboardViewer.ServiceClient; } }
		protected bool IsDashboardVSDesignMode { get { return dashboardViewer.IsDashboardVSDesignMode; } }
		protected bool AllowPrintDashboardItems { get { return dashboardViewer.AllowPrintDashboardItems; } }
		protected virtual bool AllowPrintSingleItem { get { return true; } }
		protected virtual string ExportItemType { get { return itemType; } }
		bool ShowCaption { get { return captionViewModel != null && captionViewModel.ShowCaption; } }
		public event DashboardItemMouseActionEventHandler ItemClick;
		public event DashboardItemMouseActionEventHandler ItemDoubleClick;
		public event DashboardItemMouseActionEventHandler ItemMouseMove;
		public event DashboardItemMouseEventHandler ItemMouseEnter;
		public event DashboardItemMouseEventHandler ItemMouseLeave;
		public event DashboardItemMouseEventHandler ItemMouseHover;
		public event DashboardItemMouseActionEventHandler ItemMouseUp;
		public event DashboardItemMouseActionEventHandler ItemMouseDown;
		public event DashboardItemMouseEventHandler ItemMouseWheel;
		public event DashboardItemControlUpdatedEventHandler ControlUpdated;
		public event DashboardItemControlCreatedEventHandler ControlCreating;
		public event DashboardItemBeforeControlDisposedEventHandler BeforeControlDisposed;
		public void Initialize(string dashboardItemName, DashboardViewer dashboardViewer, IDashboardItemViewerContainer itemContainer, IDashboardItemDesignerProvider designerProvider) {
			Guard.ArgumentIsNotNullOrEmpty(dashboardItemName, "dashboardItemName");
			Guard.ArgumentNotNull(itemContainer, "itemContainer");
			Guard.ArgumentNotNull(designerProvider, "designerProvider");
			Name = dashboardItemName;
			this.itemType = itemContainer.Type;
			this.dashboardViewer = dashboardViewer;
			this.itemContainer = itemContainer;
			this.designerProvider = designerProvider;
			InitializeInternal();
		}
		public void InitializeViewControl() {
			viewControl = GetViewControl();
			if(viewControl != null) {
				viewControl.Parent = this;
				viewControl.Dock = DockStyle.Fill;
				viewControl.BringToFront();
				DashboardWinHelper.SetParentLookAndFeel(viewControl, LookAndFeel);
				PrepareViewControl();
				RaiseUnderlyingControlCreating();
			}
		}
		public virtual void OnLookAndFeelChanged() {
		}
		public void RefreshByPaneContent(DashboardPaneContent paneContent) {
			Guard.ArgumentNotNull(paneContent, "paneContent");
			bool containsViewModel = ContainsContent(paneContent, ContentType.ViewModel);
			bool containsCaptionViewModel = ContainsContent(paneContent, ContentType.CaptionViewModel);
			bool containsActionModel = ContainsContent(paneContent, ContentType.ActionModel);
			bool containsCompleteDataSource = ContainsContent(paneContent, ContentType.CompleteDataSource);
			bool containsConditionalFormattingModel = ContainsContent(paneContent, ContentType.ConditionalFormattingModel);
			bool containsPartialDataSource = ContainsContent(paneContent, ContentType.PartialDataSource);
			if(containsViewModel)
				viewModel = paneContent.ViewModel;
			if(containsCaptionViewModel)
				captionViewModel = paneContent.CaptionViewModel;
			if(containsConditionalFormattingModel)
				conditionalFormattingModel = paneContent.ConditionalFormattingModel;
			if(containsCompleteDataSource || containsPartialDataSource) {
				if(paneContent.ItemData != null) {
					ClientHierarchicalMetadata metaData = containsPartialDataSource ? HierarchicalMetadata
						: new ClientHierarchicalMetadata(paneContent.ItemData.MetaData);
					multiDimensionalData = new MultiDimensionalData(new HierarchicalDataParams {
						Storage = DataStorage.CreateWithDTO(paneContent.ItemData.DataStorageDTO),
						SortOrderSlices = paneContent.ItemData.SortOrderSlices
					}, metaData);
				}
			}
			axisNames = paneContent.AxisNames;
			selectedValues = paneContent.SelectedValues;
			drillDownUniqueValues = paneContent.DrillDownUniqueValues;
			drillDownValues = paneContent.DrillDownValues;
			if(containsViewModel && !containsCompleteDataSource)
				RefreshByViewModel();
			if(containsCompleteDataSource)
				RefreshByCompleteDataSource();
			if(containsCaptionViewModel)
				UpdateCaption();
			if(containsActionModel) {
				UpdateActionModel();
			}
			ViewerUpdated(paneContent);
			RefreshCaptionButtons();
		}
		void RefreshByViewModel() {
			UpdateViewerByViewModel();
			RaiseUnderlyingControlUpdated();
		}
		void RefreshByCompleteDataSource() {
			UpdateViewer();
			RaiseUnderlyingControlUpdated();
		}
		protected virtual void UpdateViewerByViewModel() {
			UpdateViewer();
		}
		public void RefreshCaptionButtons() {
			ItemContainer.RefreshCaptionButtons();
		}
		public void SetSelectedElementIndex(int elementIndex) {
			ServiceClient.SetSelectedElementIndex(DashboardItemName, elementIndex);
		}
		internal void ShowPrintPreview() {
			DashboardViewer.ShowPrintPreview(this, this);
		}
		internal void ShowExportForm(DashboardExportFormat format) {
			DashboardViewer.ShowExportForm(format, this, this);
		}
		protected virtual ExportInfo CreateExportInfo(DashboardReportOptions exportOptions) {
			ExportInfo exportInfo = new ExportInfo();
			exportInfo.GroupName = DashboardItemName;
			exportInfo.Mode = DashboardExportMode.SingleItem;
			exportInfo.ViewerState = new ViewerState();
			exportInfo.ViewerState.ItemsState = new Dictionary<string, ItemViewerClientState>();
			exportInfo.ViewerState.ItemsState.Add(Name, GetClientState());
			exportInfo.ExportOptions = exportOptions;
			return exportInfo;
		}
		protected internal virtual IList<DashboardItemCaptionButtonInfoCreator> GetButtonInfoCreators() {
			List<DashboardItemCaptionButtonInfoCreator> creators = new List<DashboardItemCaptionButtonInfoCreator>();
			creators.AddRange(GetDataButtonInfoCreators());
			creators.AddRange(GetExportButtonInfoCreators());
			return creators;
		}
		protected internal virtual IEnumerable<DashboardItemCaptionButtonInfoCreator> GetDataButtonInfoCreators() {
			return Enumerable.Empty<DashboardItemCaptionButtonInfoCreator>();
		}
		protected internal IList<DashboardItemCaptionButtonInfoCreator> GetExportButtonInfoCreators() {
			List<DashboardItemCaptionButtonInfoCreator> creators = new List<DashboardItemCaptionButtonInfoCreator>();
			if(AllowPrintDashboardItems && !IsDashboardVSDesignMode && AllowPrintSingleItem) {
				if(ViewModel.SupportDataAwareExport)
					creators.Add(new ExportItemToExcelBarItemsPopupMenuCreator());
				else
					creators.Add(new ExportItemBarItemsPopupMenuCreator());
			}
			return creators;
		}
		protected virtual void InitializePopupMenuCreatorsData(PopupMenuCreatorsData data, Point point) {
			if(IsDashboardVSDesignMode)
				data.ServiceCreators.Add(new SelectDashboardBarItemPopupMenuCreator());
			IDashboardLayoutControlItem selectedLayoutControlItem = dashboardViewer.SelectedLayoutItem;
			if(selectedLayoutControlItem != null && selectedLayoutControlItem.ParentGroup != null && selectedLayoutControlItem.ParentGroup.ItemViewer.ShowCaption == false)
				data.ServiceCreators.Add(new SelectDashboardItemGroupBarItemPopupMenuCreator());
		}
		internal PopupMenuCreatorsData CreatePopupMenuCreatorsData(Point point) {
			PopupMenuCreatorsData data = new PopupMenuCreatorsData();
			InitializePopupMenuCreatorsData(data, point);
			if(!data.UseViewerPopup)
				foreach (DashboardItemViewerPopupMenuCreator creator in GetButtonInfoCreators())
					data.Creators.Add(creator);
			return data;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UserLookAndFeel lookAndFeel = LookAndFeel;
				if(lookAndFeel != null)
					lookAndFeel.StyleChanged -= LookAndFeelStyleChanged;
			}
			base.Dispose(disposing);
		}
		protected virtual void UpdateViewer() {
		}
		protected virtual void UpdateCaption() {
			ItemContainer.UpdateCaption(captionViewModel, drillDownValues);
		}
		protected virtual void ViewerUpdated(DashboardPaneContent paneContent) { }
		protected virtual void UpdateActionModel() {
		}
		protected virtual Control GetUnderlyingControl() { return null; }
		protected virtual void InitializeInternal() {
			SetStyle(ControlStyles.SupportsTransparentBackColor, SupportsTransparentBackColor);
			UserLookAndFeel lookAndFeel = LookAndFeel;
			if (dashboardViewer != null)
				lookAndFeel.ParentLookAndFeel = dashboardViewer.LookAndFeel;
			lookAndFeel.StyleChanged += LookAndFeelStyleChanged;
			LookAndFeelChangedInternal();
		}
		protected virtual DataPointInfo GetDataPointInfo(Point location, bool onlyTargetAxes) {
			return null;
		}
		protected virtual ExtendedReportOptions GetActualExportOptions() {
			return dashboardViewer.GetExportOptionsRepository().GetActualOpts(DashboardItemName, GetDefaultExportOptions());
		}
		protected virtual ExtendedReportOptions GetDefaultExportOptions() {
			ExtendedReportOptions opts = dashboardViewer.GetDefaultReportOptions(itemType);
			DashboardPrintingOptions printingOpts = dashboardViewer.PrintingOptions;
			DefaultBoolean showTitle = printingOpts.DocumentContentOptions.ShowTitle;
			if(showTitle == DocumentContentPrintingOptions.DefaultShowTitle) {
#pragma warning disable 0612, 0618 // Obsolete
				showTitle = printingOpts.DashboardItemOptions.IncludeCaption;
#pragma warning restore 0612, 0618
			}
			opts.DocumentContentOptions.ShowTitle = showTitle.ToBoolean(ShowCaption);
			opts.DocumentContentOptions.Title = DashboardItemCaption;
			return opts;
		}
		protected abstract Control GetViewControl();
		protected abstract void PrepareViewControl();
		protected abstract void PrepareClientState(ItemViewerClientState state);
		protected void RaiseClick(Point location) {
			if(ItemClick != null)
				ItemClick(this, new DashboardItemMouseActionEventArgs(this, location));
		}
		protected void RaiseDoubleClick(Point location) {
			if(ItemDoubleClick != null)
				ItemDoubleClick(this, new DashboardItemMouseActionEventArgs(this, location));
		}
		protected void RaiseMouseEnter() {
			if(ItemMouseEnter != null)
				ItemMouseEnter(this, new DashboardItemMouseEventArgs(DashboardItemName));
		}
		protected void RaiseMouseLeave() {
			if(ItemMouseLeave != null)
				ItemMouseLeave(this, new DashboardItemMouseEventArgs(DashboardItemName));
		}
		protected void RaiseMouseMove(Point location) {
			if(ItemMouseMove != null)
				ItemMouseMove(this, new DashboardItemMouseActionEventArgs(this, location));
		}
		protected void RaiseMouseUp(Point location) {
			if(ItemMouseUp != null)
				ItemMouseUp(this, new DashboardItemMouseActionEventArgs(this, location));
		}
		protected void RaiseMouseDown(Point location) {
			if(ItemMouseDown != null)
				ItemMouseDown(this, new DashboardItemMouseActionEventArgs(this, location));
		}
		protected void RaiseMouseWheel() {
			if(ItemMouseWheel != null)
				ItemMouseWheel(this, new DashboardItemMouseEventArgs(DashboardItemName));
		}
		protected void RaiseMouseHover() {
			if(ItemMouseHover != null)
				ItemMouseHover(this, new DashboardItemMouseEventArgs(DashboardItemName));
		}
		protected void UpdateMultiDimensionalData(HierarchicalItemData itemData) {
			this.multiDimensionalData = new MultiDimensionalData(
				new HierarchicalDataParams {
					SortOrderSlices = itemData.SortOrderSlices,
					Storage = DataStorage.CreateWithDTO(itemData.DataStorageDTO)
				}, HierarchicalMetadata);
		}
		protected void RaiseBeforeUnderlyingControlDisposed() {
			Control control = GetUnderlyingControl();
			if(control != null && BeforeControlDisposed != null) {
				DashboardItemControlEventArgs e = new DashboardItemControlEventArgs(DashboardItemName, control);
				BeforeControlDisposed(this, e);
			}
		}
		protected ClientArea GetControlClientArea(Control control) {
			DashboardLayoutControl layoutControl = dashboardViewer.LayoutControl;
			if(!layoutControl.IsHandleCreated) {
				layoutControl.Handle.ToString();
			}
			Point location = layoutControl.PointToClient(control.PointToScreen(control.Location));
			Size clientSize = control.ClientSize;
			return new ClientArea { Left = location.X, Top = location.Y, Width = clientSize.Width, Height = clientSize.Height };
		}
		internal ClientArea GetControlClientArea() {
			return GetControlClientArea(viewControl);
		}
		void LookAndFeelStyleChanged(object sender, EventArgs e) {
			LookAndFeelChangedInternal();
			OnLookAndFeelChanged();
		}
		protected void RaiseUnderlyingControlUpdated() {
			Control control = GetUnderlyingControl();
			if(control != null && ControlUpdated != null) {
				DashboardItemControlEventArgs e = new DashboardItemControlEventArgs(DashboardItemName, control);
				ControlUpdated(this, e);
			}
		}
		void LookAndFeelChangedInternal() {
			Skin skin = DashboardSkins.GetSkin(LookAndFeel);
			SkinElement sel = null;
			string elementName;
			if(skin != null) {
				elementName = ShowCaption ? DashboardSkins.SkinDashboardItemTop : DashboardSkins.SkinDashboardItemPanel;
				sel = skin[elementName];
			}
			Skin commonSkins = CommonSkins.GetSkin(LookAndFeel);
			if(sel == null) {
				elementName = ShowCaption ? CommonSkins.SkinGroupPanelTop : CommonSkins.SkinGroupPanel;
				sel = commonSkins[elementName];
			}
			if(sel != null)
				if(sel.Color.SolidImageCenterColor.IsEmpty)
					BackColor = sel.Color.GetBackColor();
				else if(sel.Color.SolidImageCenterColor.IsSystemColor)
					BackColor = commonSkins.Colors.GetColor(sel.Color.SolidImageCenterColor.Name);
				else
					BackColor = sel.Color.SolidImageCenterColor;
			ItemContainer.UpdateLookAndFeel(LookAndFeel);
		}
		ItemViewerClientState GetClientState() {
			ItemViewerClientState state = new ItemViewerClientState();
			PrepareClientState(state);
			if(ShowCaption)
				state.CaptionArea = ItemContainer.CalcCaptionArea(state.ViewerArea);
			return state;
		}
		void RaiseUnderlyingControlCreating() {
			Control control = GetUnderlyingControl();
			if(control != null && ControlCreating != null) {
				DashboardItemControlEventArgs e = new DashboardItemControlEventArgs(DashboardItemName, control);
				ControlCreating(this, e);
			}
		}
		ExtendedReportOptions IExportOptionsOwner.GetDefault() {
			return GetDefaultExportOptions();
		}
		ExtendedReportOptions IExportOptionsOwner.GetActual() {
			return GetActualExportOptions();
		}
		void IExportOptionsOwner.Set(ExtendedReportOptions opts) {
			dashboardViewer.GetExportOptionsRepository().Add(DashboardItemName, GetDefaultExportOptions(), opts);
		}		
		DataPointInfo IDataPointInfoProvider.GetInfo(Point location) {
			return GetDataPointInfo(location, false);
		}
		ItemStateCollection IDashboardExportItem.GetItemStateCollection() {
			ItemStateCollection itemExportState = new ItemStateCollection();
			itemExportState.Add(DashboardItemName, GetClientState());
			return itemExportState;
		}
		ExportInfo IDashboardExportItem.CreateExportInfo(DashboardReportOptions exportOptions) { return CreateExportInfo(exportOptions); }
		string IDashboardExportItem.ExportItemType { get { return ExportItemType; } }
	}
}
