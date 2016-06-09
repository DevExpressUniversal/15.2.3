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
using System.Linq;
using DevExpress.Snap;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.Snap.Extensions.Localization;
using DevExpress.Snap.Extensions.UI;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraCharts.UI;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Menu;
using DevExpress.XtraRichEdit.UI;
using IChartBarItem = DevExpress.XtraBars.Commands.IControlCommandBarItem<DevExpress.XtraCharts.Native.IChartContainer, DevExpress.XtraCharts.Commands.ChartCommandId>;
using IRichEditBarItem = DevExpress.XtraBars.Commands.IControlCommandBarItem<DevExpress.XtraRichEdit.RichEditControl, DevExpress.XtraRichEdit.Commands.RichEditCommandId>;
using DevExpress.Utils;
namespace DevExpress.Snap.Extensions {
	[
	Designer("DevExpress.Snap.Design.SnapBarControllerDesigner," + AssemblyInfo.SRAssemblySnapDesign), 
	DXToolboxItem(false),
	]
	public class SnapBarController : RichEditBarController {
		ChartContainerProxy chartContainerProxy;
		ISelectedFieldService selectedFieldService;
		ContextRibbonPageCategoryCollection contextPageCategories;
		TableToolsRibbonPageCategory tableTools;
		FloatingPictureToolsRibbonPageCategory floatingPictureTools;
		HeaderFooterToolsRibbonPageCategory headerFooterTools;
		ChartRibbonPageCategory chartTools;
		DataToolsRibbonPageCategory dataTools;
		CalculatedFieldBase mergefieldField;
		bool popupMenuShowing;
		bool? dataToolsListBecomeVisible;
		bool? dataToolsFieldBecomeVisible;
		bool? dataToolsGroupBecomeVisible;
		bool? chartToolsBecomeVisible;
		public SnapBarController() {
			this.contextPageCategories = new ContextRibbonPageCategoryCollection();
			contextPageCategories.CollectionChanged += OnContextPageCategoriesChanged;
		}		
		[DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public SnapControl SnapControl {
			get { return (SnapControl)Control; }
			set {				
				Control = value;
				if (Control != null && Control.IsHandleCreated)
					Control.BeginInvoke(new System.Action(UpdateRibbonPagesVisibilityBySelection));				
			}
		}
		protected override void UpdateBarItem(BarItem item) {
			UpdateBarItemControl(item);
			base.UpdateBarItem(item);
		}
		RibbonControl ribbonControl;
		SnapDockManager snapDockManager;
		[Browsable(false)]
		public RibbonControl RibbonControl { 
			get { return ribbonControl; } 
			set { 
				ribbonControl = value;
				if (ribbonControl != null) {
					ribbonControl.Manager.DockManager = snapDockManager;
				}
			} 
		}
		[Browsable(false)]
		public SnapDockManager SnapDockManager { 
			get { return snapDockManager; } 
			set { 
				snapDockManager = value;
				if (RibbonControl != null)
					RibbonControl.Manager.DockManager = snapDockManager;
			} 
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, -1, XtraSerializationFlags.Cached)
		]
		public ContextRibbonPageCategoryCollection ContextPageCategories { get { return contextPageCategories; } }
		TableToolsRibbonPageCategory TableTools {
			get {
				if (tableTools == null)
					InitTableTools();
				return tableTools;
			}
		}
		DataToolsRibbonPageCategory DataTools {
			get {
				if(dataTools == null)
					InitDataTools();
				return dataTools;
			}
		}
		AppearanceRibbonPage DataToolsAppearance { get { return GetDataToolsPage(SnapExtensionsStringId.Caption_Appearance) as AppearanceRibbonPage; } }
		SNMergeFieldToolsRibbonPage DataToolsField { get { return GetDataToolsPage(SnapExtensionsStringId.Caption_SNMergeFieldDesign) as SNMergeFieldToolsRibbonPage; } }
		GroupToolsRibbonPage DataToolsGroup { get { return GetDataToolsPage(SnapExtensionsStringId.Caption_GroupDesign) as GroupToolsRibbonPage; } }
		ListToolsRibbonPage DataToolsList { get { return GetDataToolsPage(SnapExtensionsStringId.Caption_SNListDesign) as ListToolsRibbonPage; } }
		RibbonPage GetDataToolsPage(SnapExtensionsStringId stringId) {
			DataToolsRibbonPageCategory category = DataTools;
			if(category == null) return null;
			RibbonControl owner = category.Ribbon.MergeOwner;
			if(owner != null) {
				category = owner.MergedCategories[category.Text] as DataToolsRibbonPageCategory;
				if(category == null) return null;
			}
			string localizedText = SnapExtensionsLocalizer.GetString(stringId);
			foreach (RibbonPage page in category.Pages) {
				if (string.Compare(page.Text, localizedText, true) == 0)
					return page;
			}
			return null;
		}
		FloatingPictureToolsRibbonPageCategory FloatingPictureTools {
			get {
				if (floatingPictureTools == null)
					InitFloatingPictureTools();
				return floatingPictureTools;
			}
		}
		HeaderFooterToolsRibbonPageCategory HeaderFooterTools {
			get {
				if (headerFooterTools == null)
					InitHeaderFooterTools();
				return headerFooterTools;
			}
		}
		ChartRibbonPageCategory ChartTools {
			get {
				if (chartTools == null)
					InitChartTools();
				return chartTools;
			}
		}
		void InitDataTools() {
			foreach(ContextRibbonPageCategoryItem category in ContextPageCategories) {
				dataTools = category.PageCategory as DataToolsRibbonPageCategory;
				if(dataTools != null) return;
			}
		}
		void InitTableTools() {
			foreach (var category in ContextPageCategories) {
				if (category.PageCategory is TableToolsRibbonPageCategory) {
					tableTools = (TableToolsRibbonPageCategory)category.PageCategory;
					return;
				}
			}
		}
		void InitFloatingPictureTools() {
			foreach (var category in ContextPageCategories) {
				if (category.PageCategory is FloatingPictureToolsRibbonPageCategory) {
					floatingPictureTools = (FloatingPictureToolsRibbonPageCategory)category.PageCategory;
					return;
				}
			}
		}
		void InitHeaderFooterTools() {
			foreach (var category in ContextPageCategories) {
				if (category.PageCategory is HeaderFooterToolsRibbonPageCategory) {
					headerFooterTools = (HeaderFooterToolsRibbonPageCategory)category.PageCategory;
					return;
				}
			}
		}
		void InitChartTools() {
			foreach (var category in ContextPageCategories) {
				if (category.PageCategory is ChartRibbonPageCategory) {
					chartTools = (ChartRibbonPageCategory)category.PageCategory;
					chartTools.Color = DXColor.FromArgb(0xff, 0xe9, 0x4c, 0x4c);
					return;
				}
			}
		}
		void OnContextPageCategoriesChanged(object sender, EventArgs e) {
			this.tableTools = null;
			this.floatingPictureTools = null;
			this.headerFooterTools = null;
			this.chartTools = null;
			this.dataTools = null;
			if(SnapControl != null && !SnapControl.IsDisposed && !DesignMode)
				UpdateRibbonPagesVisibilityBySelection();
		}
		void UpdateBarItemControl(BarItem item) {
			IRichEditBarItem richEditItem = item as IRichEditBarItem;
			if(richEditItem != null) {
				richEditItem.Control = Control;
				return;
			}
			IChartBarItem chartBarItem = item as IChartBarItem;
			if(chartBarItem != null) {
				chartBarItem.Control = null; 
				chartBarItem.Control = chartContainerProxy;
			}
		}
		protected override void SetControlCore(RichEditControl value) {
			base.SetControlCore(value);
			UpdateChartContainer();
			UpdateSelectedFieldService();
		}
		void UpdateChartContainer() {
			if(SnapControl == null)
				return;
			if(chartContainerProxy != null)
				chartContainerProxy.Dispose();
			chartContainerProxy = new ChartContainerProxy();
		}
		void UpdateSelectedFieldService() {
			if (SnapControl == null || Site != null)
				return;
			if (this.selectedFieldService != null)
				this.selectedFieldService.FieldSelectionChanged -= HandleFieldSelectionChanged;
			this.selectedFieldService = SnapControl.GetService<ISelectedFieldService>();
			this.selectedFieldService.FieldSelectionChanged += HandleFieldSelectionChanged;
		}
		void HandleFieldSelectionChanged(object sender, FieldSelectionChangedEventArgs e) {
			SNChartField chartField = e.ParsedField as SNChartField;
			if (chartField != null) {
				chartContainerProxy.UpdateFieldChangerService(e.FieldInfo);
				chartContainerProxy.SetInnerContainer(chartField.GetChartContainer(SnapControl.DocumentModel, e.FieldInfo));
			} else
				chartContainerProxy.ClearInnerContainer();
			foreach(BarItem item in BarItems.OfType<IChartBarItem>())
				UpdateBarItem(item);
		}
		protected override void Dispose(bool disposing) {
			if (disposing && chartContainerProxy != null) {
				chartContainerProxy.Dispose();
				chartContainerProxy = null;
			}
			base.Dispose(disposing);
			if (disposing) {
				foreach (var category in ContextPageCategories)
					category.Dispose();
				ContextPageCategories.Clear();
			}
		}		
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			if (SnapControl == null || Site != null)
				return;
			if(!Control.IsHandleCreated)
				Control.HandleCreated += Control_HandleCreated;
			SnapControl.PopupMenuShowing += new PopupMenuShowingEventHandler(SnapControl_PopupMenuShowing);
			SnapControl.SelectionChanged += new System.EventHandler(SelectionChanged);
			ISelectedFieldService service = SnapControl.GetService<ISelectedFieldService>();
			service.FieldSelectionChanged += new System.EventHandler<FieldSelectionChangedEventArgs>(FieldSelectionChanged);
			SnapControl.StartHeaderFooterEditing += new HeaderFooterEditingEventHandler(StartHeaderFooterEditing);
			SnapControl.FinishHeaderFooterEditing += new HeaderFooterEditingEventHandler(FinishHeaderFooterEditing);
			SnapControl.UpdateUI += OnUpdateUI;
		}		
		void Control_HandleCreated(object sender, System.EventArgs e) {
			Control.BeginInvoke(new System.Action(UpdateRibbonPagesVisibilityBySelection));
		}
		void SnapControl_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
			e.Menu.CloseUp += new System.EventHandler(Menu_CloseUp);
			popupMenuShowing = true;
		}
		void Menu_CloseUp(object sender, System.EventArgs e) {
			popupMenuShowing = false;
			if (mergefieldField != null) {
				UpdateRibbonPagesVisibility(mergefieldField);
				mergefieldField = null;
			}
			RichEditPopupMenu popupMenu = sender as RichEditPopupMenu;
			if (popupMenu != null) {
				popupMenu.CloseUp -= Menu_CloseUp;
			}
		}		
		protected override void UnsubscribeControlEvents() {
			base.UnsubscribeControlEvents();
			if(this.selectedFieldService != null)
				this.selectedFieldService.FieldSelectionChanged -= HandleFieldSelectionChanged;
			if (SnapControl == null)
				return;
			Control.HandleCreated -= Control_HandleCreated;
			SnapControl.PopupMenuShowing -= SnapControl_PopupMenuShowing;
			SnapControl.SelectionChanged -= SelectionChanged;
			ISelectedFieldService service = SnapControl.GetService<ISelectedFieldService>();
			service.FieldSelectionChanged -= FieldSelectionChanged;
			SnapControl.StartHeaderFooterEditing -= StartHeaderFooterEditing;
			SnapControl.FinishHeaderFooterEditing -= FinishHeaderFooterEditing;
			SnapControl.UpdateUI -= OnUpdateUI;
		}
		void SelectionChanged(object sender, System.EventArgs e) {
			if (TableTools != null)
				TableTools.Visible = SnapControl.IsSelectionInTable();
			if (FloatingPictureTools != null)
				FloatingPictureTools.Visible = SnapControl.IsFloatingObjectSelected;
		}
		void FieldSelectionChanged(object sender, FieldSelectionChangedEventArgs e) {
			if (popupMenuShowing) {
				this.mergefieldField = e.ParsedField;
				return;
			}
			UpdateRibbonPagesVisibility(e.ParsedField);
		}
		void UpdateRibbonPagesVisibility(CalculatedFieldBase mergefieldField) {
			ResetVisibilityFlags();
			SetDataToolsVisibility();
			SetChartToolsVisibility(mergefieldField);
			RibbonPage selectedPage = GetSelectedPage();
			if(selectedPage == null) return;
			selectedPage.Ribbon.SelectedPage = selectedPage;
		}
		void ResetVisibilityFlags() {
			if (chartToolsBecomeVisible != null)
				chartToolsBecomeVisible = false;
			if(dataToolsFieldBecomeVisible != null)
				dataToolsFieldBecomeVisible = false;
			if(dataToolsGroupBecomeVisible != null)
				dataToolsGroupBecomeVisible = false;
			if (dataToolsListBecomeVisible != null)
				dataToolsListBecomeVisible = false;
		}
		RibbonPage GetSelectedPage() {
			if (chartToolsBecomeVisible == true)
				return ChartTools.Pages[0];
			else if (dataToolsGroupBecomeVisible == true)
				return DataToolsGroup;
			else if (dataToolsListBecomeVisible == true)
				return DataToolsList;
			else if (dataToolsFieldBecomeVisible == true)
				return DataToolsField;
			return null;
		}
		void SetDataToolsVisibility() {
			if (DataTools == null)
				return;
			bool inSnList = SnapControl.IsSNFieldSelected() || SnapControl.IsSelectionInSNList();
			bool themeToolsVisible = false;
			bool fieldToolsVisible = false;
			bool groupToolsVisible = false;
			bool listToolsVisible = false;
			SnapObjectModelController controller = new SnapObjectModelController(SnapControl.DocumentModel.ActivePieceTable);
			SnapBookmark bookmark = controller.GetSelectedBookmark();
			AppearanceRibbonPage dataToolsAppearance = DataToolsAppearance;
			if (dataToolsAppearance != null) {
				themeToolsVisible = true;
				dataToolsAppearance.Visible = themeToolsVisible;
			}
			SNMergeFieldToolsRibbonPage dataToolsField = DataToolsField;
			ListToolsRibbonPage dataToolsList = DataToolsList;
			if (dataToolsList != null) {
				listToolsVisible = inSnList && HasEnabledItems(dataToolsList);
				bool shouldSwitchOnListTools = dataToolsFieldBecomeVisible == null || !dataToolsList.Visible || dataToolsList.Ribbon.SelectedPage == dataToolsField;
				dataToolsListBecomeVisible = listToolsVisible && SnapObjectModelController.IsBookmarkCorrespondsToListHeaderFooter(bookmark) && shouldSwitchOnListTools;
				dataToolsList.Visible = listToolsVisible;
			}
			if (dataToolsField != null) {
				fieldToolsVisible = inSnList && HasEnabledItems(dataToolsField);
				bool shouldSwitchOnFieldTools = dataToolsFieldBecomeVisible == null || !dataToolsField.Visible || (SnapObjectModelController.IsBookmarkCorrespondsToDataRow(bookmark) && dataToolsField.Ribbon.SelectedPage == dataToolsList);
				dataToolsFieldBecomeVisible = fieldToolsVisible && shouldSwitchOnFieldTools;
				dataToolsField.Visible = fieldToolsVisible;
			}
			GroupToolsRibbonPage dataToolsGroup = DataToolsGroup;
			if (dataToolsGroup != null) {
				groupToolsVisible = SnapObjectModelController.IsBookmarkCorrespondsToGroup(bookmark) && HasEnabledItems(dataToolsGroup);
				dataToolsGroupBecomeVisible = groupToolsVisible && (dataToolsGroupBecomeVisible == null || !dataToolsGroup.Visible);
				dataToolsGroup.Visible = groupToolsVisible;
			}
			DataTools.Visible = themeToolsVisible || fieldToolsVisible || groupToolsVisible || listToolsVisible;
		}
		void SetChartToolsVisibility(CalculatedFieldBase parsedField) {
			if (ChartTools == null)
				return;
			bool parsedFieldIsChartField = parsedField is SNChartField;
			chartToolsBecomeVisible = !ChartTools.Visible && parsedFieldIsChartField;
			ChartTools.Visible = parsedFieldIsChartField;
		}
		void OnUpdateUI(object sender, System.EventArgs e) {
			if (popupMenuShowing) {
				SnapFieldInfo fieldInfo = FieldsHelper.GetSelectedField(SnapControl.DocumentModel);
				if (fieldInfo != null)
					this.mergefieldField = fieldInfo.ParsedInfo;
				else
					UpdateRibbonPagesVisibility(null);
			}
		}
		void UpdateRibbonPagesVisibilityBySelection() {
			SnapFieldInfo fieldInfo = FieldsHelper.GetSelectedField(SnapControl.DocumentModel);
			if (fieldInfo == null)
				UpdateRibbonPagesVisibility(null);
			else
				UpdateRibbonPagesVisibility(fieldInfo.ParsedInfo);
			SelectionChanged(SnapControl, System.EventArgs.Empty);
		}
		bool HasEnabledItems(RibbonPageCategory category) {
			if (category != null) {
				foreach (RibbonPage page in category.Pages) {
					if(HasEnabledItems(page))
						return true;
				}
			}
			return false;
		}
		bool HasEnabledItems(RibbonPage page) {
			foreach(RibbonPageGroup group in page.Groups) {
				foreach(BarItemLink link in group.ItemLinks) {
					ICommandBarItem commandBarItem = link.Item as ICommandBarItem;
					if(commandBarItem != null) {
						commandBarItem.UpdateVisibility();
						if(link.Enabled)
							return true;
					}
				}
			} 
			return false;
		}
		void StartHeaderFooterEditing(object sender, HeaderFooterEditingEventArgs e) {
			if (HeaderFooterTools != null) {
				HeaderFooterTools.Visible = true;
				if(RibbonControl != null)
				RibbonControl.SelectedPage = HeaderFooterTools.Pages[0];
			}
		}
		void FinishHeaderFooterEditing(object sender, HeaderFooterEditingEventArgs e) {
			if (HeaderFooterTools != null)
				HeaderFooterTools.Visible = false;
		}
	}
	public class ContextRibbonPageCategoryCollection : Collection<ContextRibbonPageCategoryItem> {
		protected internal event EventHandler CollectionChanged;
		protected virtual void RaiseCollectionChanged() {
			if (CollectionChanged != null)
				CollectionChanged(this, EventArgs.Empty);			
		}
		protected override void ClearItems() {
			base.ClearItems();
			RaiseCollectionChanged();
		}
		protected override void InsertItem(int index, ContextRibbonPageCategoryItem item) {
			base.InsertItem(index, item);
			RaiseCollectionChanged();
		}
		protected override void RemoveItem(int index) {
			base.RemoveItem(index);
			RaiseCollectionChanged();
		}
		protected override void SetItem(int index, ContextRibbonPageCategoryItem item) {
			base.SetItem(index, item);
			RaiseCollectionChanged();
		}
	}
}
