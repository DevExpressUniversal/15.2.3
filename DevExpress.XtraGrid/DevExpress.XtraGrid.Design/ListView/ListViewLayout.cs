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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Frames;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.WinExplorer;
namespace DevExpress.XtraGrid.Design.ListView {
	[ToolboxItem(false)]
	public class ListViewLayout : LayoutsBase {
		EditingGridInfo gridInfo;
		SplitContainerControl splitContainer;
		ListViewLayoutOptionsSelector optionsControl;
		GridControl sampleGrid;
		BaseView sampleGridView;
		WinExplorerDTViewOptions dtOptions;
		IComponentChangeService componentChangeServiceCore;
		public ListViewLayout()
			: base(6) {
			this.gridInfo = null;
		}
		protected virtual WinExplorerDTViewOptions CreateOptionsObject() {
			return new WinExplorerDTViewOptions(View);
		}
		protected GridControl SampleGrid { get { return sampleGrid; } }
		protected SplitContainerControl SplitContainer { get { return splitContainer; } }
		protected ListViewLayoutOptionsSelector OptionsControl { get { return optionsControl; } }
		protected WinExplorerView SampleGridView { get { return sampleGridView as WinExplorerView; } }
		protected EditingGridInfo GridInfo { get { return gridInfo; } }
		protected WinExplorerDTViewOptions DTOptions { get { return dtOptions; } }
		protected WinExplorerView View { get { return GridInfo != null ? GridInfo.SelectedView as WinExplorerView : null; } }
		protected override void SetControlDataSource(DataView dataView) {
			SampleGrid.DataSource = dataView;
			if(((ColumnView)EditingView).Columns.Count == 0)
				sampleGridView.PopulateColumns();
			else SetLayoutChanged(false);
		}
		ComponentCollection GetCurrentComponents() {
			if(EditingView.GridControl.Container != null) return EditingView.GridControl.Container.Components;
			if(EditingView.Container != null) return EditingView.Container.Components;
			return null;
		}
		protected BaseView EditingView { get { return EditingObject as BaseView; } }
		protected virtual ListViewLayoutOptionsSelector CreateOptionsControl() {
			return new ListViewLayoutOptionsSelector();
		}
		protected virtual void SyncWinExplorerViewOptions() {
			if(SampleGridView == null || View == null) return;
			SampleGridView.BeginUpdate();
			try {
				SampleGridView.Assign(View, false);
			}
			finally {
				SampleGridView.EndUpdate();
			}
		}
		protected virtual void RefreshViews() {
			SyncWinExplorerViewOptions();
			OnViewChanged();
		}
		protected virtual void OnOptionsControlSelectedObjectChanged(object sender, SelectedObjectChangedEventArgs e) {
			RefreshViews();
		}
		protected virtual void OnDesignTimeOptionPropertyChanged(object sender, PropertyChangedEventArgs e) {
			RefreshViews();
		}
		protected virtual void OnViewChanged() {
			if(View != null && ComponentChangeService != null) {
				ComponentChangeService.OnComponentChanged(View, null, null, null);
			}
		}
		protected virtual GridControl CreateSampleGrid() {
			return GridAssign.CreateGridControlAssign(EditingView.GridControl, EditingView);
		}
		protected IComponentChangeService ComponentChangeService { get { return componentChangeServiceCore; } }
		#region Layout Members
		protected override void ApplyLayouts() {
		}
		protected override void RestoreLayoutFromXml(string fileName) {
			sampleGridView.RestoreLayoutFromXml(fileName, OptionsLayoutBase.FullLayout);
		}
		protected override void SaveLayoutToXml(string fileName) {
			try {
				sampleGridView.SaveLayoutToXml(fileName, OptionsLayoutBase.FullLayout);
			}
			catch(Exception ex) {
				XtraMessageBox.Show(ex.Message, ex.Source);
			}
		}
		protected override Control CreatePreviewControl() {
			this.gridInfo = InfoObject as EditingGridInfo;
			this.componentChangeServiceCore = GridInfo.EditingGrid.InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
			this.dtOptions = CreateOptionsObject();
			this.splitContainer = new SplitContainerControl();
			DTOptions.PropertyChanged += OnDesignTimeOptionPropertyChanged;
			this.optionsControl = CreateOptionsControl();
			SplitContainer.Dock = DockStyle.Fill;
			this.sampleGrid = CreateSampleGrid();
			SplitContainer.Panel1.Controls.Add(OptionsControl);
			SplitContainer.Panel2.Controls.Add(SampleGrid);
			SplitContainer.SplitterPosition = 400;
			OptionsControl.Dock = DockStyle.Fill;
			OptionsControl.Name = "optionsControl";
			SampleGrid.Dock = DockStyle.Fill;
			SampleGrid.Tag = "Design";
			OptionsControl.AssignGridInfo(GridInfo);
			OptionsControl.SelectObject(DTOptions);
			OptionsControl.SelectedObjectChanged += OnOptionsControlSelectedObjectChanged;
			OptionsControl.MovePropertyGridSplitterTo(120);
			this.sampleGridView = SampleGrid.MainView;
			if(dataSet != null)
				SampleGrid.DataSource = dataSet.Tables[tableName];
			else
				SampleGrid.DataSource = EditingView.DataSource;
			SyncWinExplorerViewOptions();
			ChangeColumnSelectorButtonVisibility(View != null);
			SampleGrid.MainView.Layout += (sender, e) => SetLayoutChanged(true);
			return SplitContainer;
		}
		protected override void HideColumnsCustomization() {
		}
		protected override void ShowColumnsCustomization() {
		}
		protected override DBAdapter CreateDBAdapter() {
			ArrayList adapters = new ArrayList();
			ComponentCollection components = GetCurrentComponents();
			if(components == null) return null;
			foreach(object comp in components)
				adapters.Add(comp);
			return new DBAdapter(adapters, EditingView.GridControl.DataSource, EditingView.GridControl.DataMember);
		}
		protected override DataTable CreateDataTableAdapter() {
			if(EditingView.GridControl.DataSource == null) return null;
			try {
				CurrencyManager manager = EditingView.GridControl.BindingContext[EditingView.GridControl.DataSource, EditingView.GridControl.DataMember] as CurrencyManager;
				if(manager == null) return null;
				DataView dv = manager.List as DataView;
				if(dv != null) return dv.Table;
			}
			catch {
			}
			return null;
		}
		#endregion
		#region Dispose
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(OptionsControl != null) {
					OptionsControl.SelectedObjectChanged -= OnOptionsControlSelectedObjectChanged;
					OptionsControl.Dispose();
					this.optionsControl = null;
				}
				if(SampleGrid != null) {
					SampleGrid.Dispose();
					this.sampleGrid = null;
				}
				if(SplitContainer != null) {
					SplitContainer.Dispose();
					this.splitContainer = null;
				}
				if(DTOptions != null) {
					DTOptions.PropertyChanged -= OnDesignTimeOptionPropertyChanged;
					this.dtOptions = null;
				}
			}
			base.Dispose(disposing);
		}
		#endregion
		protected override bool AllowAutoHideTabHeader { get { return true; } }
		protected override string DescriptionText { get { return "Some Description"; } }
	}
	public class WinExplorerDTViewOptions : INotifyPropertyChanged {
		WinExplorerView view;
		public WinExplorerDTViewOptions(WinExplorerView view) {
			this.view = view;
		}
		[DisplayName("WinExplorerView Columns"), DXCategory(CategoryName.Layout), TypeConverter(typeof(ExpandableObjectConverter))]
		public WinExplorerViewColumns Columns {
			get { return View.ColumnSet; }
		}
		[DXCategory(CategoryName.Layout)]
		public WinExplorerViewStyle Style {
			get { return View.OptionsView.Style; }
			set {
				if(View.OptionsView.Style == value)
					return;
				View.OptionsView.Style = value;
				OnItemTypeChanged();
			}
		}
		protected virtual void OnItemTypeChanged() {
			RaisePropertyChanged("ItemType");
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		protected WinExplorerView View { get { return view; } }
	}
}
