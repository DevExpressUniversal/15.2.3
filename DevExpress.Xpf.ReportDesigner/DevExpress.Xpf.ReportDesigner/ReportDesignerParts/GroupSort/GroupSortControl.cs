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

using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Reports.UserDesigner.GroupSort.Native;
using System.Windows;
using DevExpress.Mvvm.UI.Native;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Reports.UserDesigner.FieldList;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.Xpf.Reports.UserDesigner.Editors;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.Xpf.Core;
using System.Diagnostics;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using System.ComponentModel;
using DevExpress.XtraReports.Design;
using DevExpress.Mvvm;
using DevExpress.Xpf.Grid;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Reports.UserDesigner.GroupSort {
	public class GroupSortControl : Control, IGroupSortFieldController {
		const string PART_TreeListView = "PART_TreeList";
		IObjectTracker reportTracker;
		IObjectTracker selectedObjectTracker;
		readonly Locker Locker = new Locker();
		FieldListNodeBase<XRDiagramControl> effectiveDataSource;
		readonly IEnumerable<string> trackedProperties = new[] {
			XRComponentPropertyNames.DataSource,
			XRComponentPropertyNames.DataMember,
			"GroupFields",
		};
		public ICommand AddGroupCommand { get; private set; }
		public ICommand AddSortCommand { get; private set; }
		public ICommand RemoveCommand { get; private set; }
		public ICommand MoveUpCommand { get; private set; }
		public ICommand MoveDownCommand { get; private set; }
		public ICommand SelectItemCommand { get; private set; }
		public static readonly DependencyProperty ReportModelProperty;
		public static readonly DependencyProperty SelectedXRModelProperty;
		public static readonly DependencyProperty SelectedItemProperty;
		static readonly DependencyPropertyKey ItemsPropertyKey;
		public static readonly DependencyProperty ItemsProperty;
		static readonly DependencyPropertyKey DataSourcePropertyKey;
		public static readonly DependencyProperty DataSourceProperty;
		static readonly Action<GroupSortControl, Action<IMessageBoxService>> messageBoxServiceAccessor;
		public static readonly DependencyProperty MessageBoxServiceTemplateProperty;
		public ObservableCollection<IGroupSortItem> Items {
			get { return (ObservableCollection<IGroupSortItem>)GetValue(ItemsProperty); }
			private set { SetValue(ItemsPropertyKey, value); }
		}
		public IGroupSortItem SelectedItem {
			get { return (IGroupSortItem)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public XtraReportModelBase ReportModel {
			get { return (XtraReportModelBase)GetValue(ReportModelProperty); }
			set { SetValue(ReportModelProperty, value); }
		}
		public XRModelBase SelectedXRModel {
			get { return (XRModelBase)GetValue(SelectedXRModelProperty); }
			set { SetValue(SelectedXRModelProperty, value); }
		}
		public IEnumerable<FieldListNodeBase<XRDiagramControl>> DataSource {
			get { return (IEnumerable<FieldListNodeBase<XRDiagramControl>>)GetValue(DataSourceProperty); }
			private set { SetValue(DataSourcePropertyKey, value); }
		}
		protected void DoWithMessageBoxService(Action<IMessageBoxService> action) { messageBoxServiceAccessor(this, action); }
		public DataTemplate MessageBoxServiceTemplate {
			get { return (DataTemplate)GetValue(MessageBoxServiceTemplateProperty); }
			set { SetValue(MessageBoxServiceTemplateProperty, value); }
		}
		XtraReportBase selectedReport;
		XtraReportBase SelectedReport {
			get { return selectedReport; }
			set {
				if(selectedReport == value)
					return;
				selectedReport = value;
				OnSelectedReportChanged();
			}
		}
		FieldListNodeBase<XRDiagramControl> EffectiveDataSource {
			get { return effectiveDataSource; }
			set {
				if(effectiveDataSource == value)
					return;
				effectiveDataSource = value;
				DataSource = effectiveDataSource.Return(x => new[] { x }, () => null);
			}
		}
		IGroupSortFieldController GroupSortController {
			get { return this; }
		}
		static GroupSortControl() {
			DependencyPropertyRegistrator<GroupSortControl>.New()
				.Register(owner => owner.SelectedItem, out SelectedItemProperty, null)
				.RegisterReadOnly(owner => owner.Items, out ItemsPropertyKey, out ItemsProperty, new ObservableCollection<IGroupSortItem>())
				.Register(owner => owner.ReportModel, out ReportModelProperty, null)
				.Register(owner => owner.SelectedXRModel, out SelectedXRModelProperty, null, owner => owner.OnSelectionChanged())
				.RegisterReadOnly(owner => owner.DataSource, out DataSourcePropertyKey, out DataSourceProperty, null)
				.RegisterServiceTemplateProperty(d => d.MessageBoxServiceTemplate, out MessageBoxServiceTemplateProperty, out messageBoxServiceAccessor)
				.OverrideDefaultStyleKey();
		}
		void OnSelectionChanged() {
			SelectedReport = GetReport();
			selectedObjectTracker.Do(tracker => tracker.ObjectPropertyChanged -= OnSelectedOjbectPropertyChanged);
			SelectedXRModel.Do(x => {
				Tracker.GetTracker(x.XRObject, out selectedObjectTracker);
				selectedObjectTracker.Do(tracker => tracker.ObjectPropertyChanged += OnSelectedOjbectPropertyChanged);
			});
		}
		void OnSelectedReportChanged() {
			reportTracker.Do(tracker => tracker.ObjectPropertyChanged -= OnReportPropertyChanged);
			SelectedReport.Do(x => {
				Tracker.GetTracker(x, out reportTracker);
				reportTracker.Do(tracker => tracker.ObjectPropertyChanged += OnReportPropertyChanged);
			});
			EffectiveDataSource = GetEffectiveDataSource();
			Items.Clear();
			GetGroupSortItems().ForEach(x => {
				x.Id = Items.Count;
				Items.Add(x);
			});
		}
		void OnSelectedOjbectPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(trackedProperties.Contains(e.PropertyName))
				UpdateGroupSortItems();
		}
		void UpdateAll() {
			EffectiveDataSource = GetEffectiveDataSource();
			UpdateGroupSortItems();
		}
		private void OnReportPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if(trackedProperties.Contains(e.PropertyName))
				UpdateAll();
		}
		public GroupSortControl() {
			InitializeCommands();
		}
		void InitializeCommands() {
			AddGroupCommand = DelegateCommandFactory.Create<FieldListNodeBase<XRDiagramControl>>(AddGroup, (x) => DataSource != null && x != null);
			AddSortCommand = DelegateCommandFactory.Create<FieldListNodeBase<XRDiagramControl>>(AddSort, (x) => DataSource != null && x != null);
			RemoveCommand = DelegateCommandFactory.Create(Remove, CanRemove);
			MoveUpCommand = DelegateCommandFactory.Create(MoveUp, ()=> SelectedItem.Return(x => x.Id > 0, ()=> false));
			MoveDownCommand = DelegateCommandFactory.Create(() => MoveDown(), () => SelectedItem.Return(x => x.Id < Items.Count - 1, () => false));
		}
		void AddSort(FieldListNodeBase<XRDiagramControl> field) {
			if (field.Children.Count() > 0)
				return;
			var groupField = new GroupSortAddStrategy(ReportModel).AddSort(ModifyStrategy.GetValidFieldName(EffectiveDataSource.DataMember, field.BindingData.Member));
			UpdateGroupSortItems();
		}
		void AddGroup(FieldListNodeBase<XRDiagramControl> field) {
			if (field.Children.Count() > 0)
				return;
			var groupField = new GroupSortAddStrategy(ReportModel).AddGroup(GetReport(), ModifyStrategy.GetValidFieldName(EffectiveDataSource.DataMember, field.BindingData.Member));
			UpdateGroupSortItems();
		}
		void UpdateGroupSortItems() {
			Items.Clear();
			GetGroupSortItems().ForEach(x => {
				x.Id = Items.Count;
				Items.Add(x);
			});
		}
		void MoveUp() {
			var parent = Items.Single(x => x.Id == SelectedItem.ParentId);
			var parentId = parent.Id;
			new GroupSortMovementStrategy(ReportModel).MoveUp(SelectedItem, parent);
			UpdateGroupSortItems();
			SelectedItem = Items.SingleOrDefault(x => x.Id == parentId);
		}
		void MoveDown() {
			var child = Items.Single(x => x.ParentId == SelectedItem.Id);
			var childId = child.Id;
			new GroupSortMovementStrategy(ReportModel).MoveDown(SelectedItem, child);
			UpdateGroupSortItems();
			SelectedItem = Items.SingleOrDefault(x => x.Id == childId);
		}
		void Remove() {
			var strategy = new GroupSortRemoveStrategy(ReportModel, SelectedReport);
			if (!strategy.CanRemove(SelectedItem, DoWithMessageBoxService))
				return;
			strategy.Remove(SelectedItem, Items.FirstOrDefault(x=> x.ParentId == SelectedItem.Id));
			UpdateGroupSortItems();
		}
		bool CanRemove() {
			return SelectedItem != null;
		}
		void Select(IGroupSortItem item) {
			SelectedItem = item;
		}
		XtraReportBase GetReport() {
			if (SelectedXRModel == null)
				return null;
			if (SelectedXRModel.XRObject is XtraReportBase)
				return (XtraReportBase)SelectedXRModel.XRObject;
			if (SelectedXRModel.XRObject is XRControl)
				return ((XRControl)SelectedXRModel.XRObject).Report;
			return ReportModel.Return(x => x.XRObject, () => null);
		}
		FieldListNodeBase<XRDiagramControl> GetEffectiveDataSource() {
			if(SelectedReport == null)
				return null;
			var dataMember = SelectedReport.DataMember;
			var dataSource = SelectedReport.GetEffectiveDataSource();
			var currentDataSourceModel = ReportModel.Return(r => r.DataSources.SingleOrDefault(x => x.DataSource == dataSource), null);
			if (currentDataSourceModel == null)
				return null;
			return string.IsNullOrEmpty(dataMember)
				? currentDataSourceModel
				: FindDataMember(currentDataSourceModel, dataMember);
		}
		IEnumerable<IGroupSortItem> GetGroupSortItems() {
			var report = GetReport();
			if (report == null || report.Bands == null)
				return Enumerable.Empty<IGroupSortItem>();
			var groupFields = XtraReportBase.CollectGroupFields(report.Groups, report.Bands[BandKind.Detail] as DetailBand);
			return groupFields.Select(x => GroupSortItem.Create(this, x, GetActualBindingData(x)));
		}
		BindingData GetActualBindingData(GroupField field) {
			if(EffectiveDataSource == null)
				return new BindingData(null, field.FieldName);
			var node = FindDataMember(EffectiveDataSource, string.IsNullOrEmpty(EffectiveDataSource.DataMember) ? field.FieldName : string.Concat(EffectiveDataSource.DataMember, ".", field.FieldName));
			return node.Return(x=> x.BindingData, ()=> null);
		}
		FieldListNodeBase<XRDiagramControl> FindDataMember(FieldListNodeBase<XRDiagramControl> currentDataSourceModel, string dataMember) {
			if(currentDataSourceModel == null)
				return null;
			var pathProvider = currentDataSourceModel.IsDataSourceNode ? (IHierarchicalPathProvider)new BindingDataHierarchicalPathProvider() : new GroupSortHierarchicalPathProviderBehavior() { EffectiveDataSource = currentDataSourceModel };
			HierarchicalDataLocator locator = new HierarchicalDataLocator(currentDataSourceModel.Yield(), "BindingData", "Children", null, pathProvider);
			BindingData bindingData = new BindingData(currentDataSourceModel.DataSource, dataMember);
			var node = locator.FindItemByValue(bindingData);
			return (FieldListNodeBase<XRDiagramControl>)node;
		}
		#region IGroupSortFieldController
		void IGroupSortFieldController.ChangeField(GroupField field, BindingData bindingData) {
			new GroupSortFieldModifyStrategy(ReportModel).ChangeField(field, ModifyStrategy.GetValidFieldName(EffectiveDataSource.DataMember, bindingData.Member));
		}
		void IGroupSortFieldController.ChangeSortOrder(GroupField field, XRColumnSortOrder sortOrder) {
			new GroupSortFieldModifyStrategy(ReportModel).ChangeSortOrder(field, sortOrder);
		}
		void IGroupSortFieldController.ShowGroupHeader(GroupField field, bool visibility) {
			var index = Items.IndexOf(SelectedItem);
			var strategy = new GroupSortHeaderFooterVisibilityStrategy(ReportModel, DoWithMessageBoxService, SelectedReport);
			if (visibility)
				strategy.ShowHeader(SelectedItem);
			else strategy.HideHeader(SelectedItem, Items.SingleOrDefault(x => x.ParentId == SelectedItem.Id));
			((GroupSortItem)SelectedItem).UpdateInternal();
		}
		void IGroupSortFieldController.ShowGroupFooter(GroupField field, bool visibility) {
			new GroupSortHeaderFooterVisibilityStrategy(ReportModel, DoWithMessageBoxService, SelectedReport).ChangeFooterVisibility(field, visibility);
		}
		#endregion
	}
	public class ShowBandEditorUpdateBehavior : Behavior<TreeListView> {
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.CellValueChanging += OnGroupSortItemProeprtyChanging;
		}
		void OnGroupSortItemProeprtyChanging(object sender, Grid.TreeList.TreeListCellValueChangedEventArgs e) {
			AssociatedObject.CellValueChanged += OnGroupSortItemPropertyChanged;
			AssociatedObject.CommitEditing();
		}
		void OnGroupSortItemPropertyChanged(object sender, Grid.TreeList.TreeListCellValueChangedEventArgs e) {
			AssociatedObject.CellValueChanged -= OnGroupSortItemPropertyChanged;
			if(e.Column.FieldName == "ShowHeader" || e.Column.FieldName == "ShowFooter") {
				Dispatcher.BeginInvoke((Action)(() => {
					var value = e.Row.GetType().GetProperty(e.Column.FieldName).GetValue(e.Row, null);
					AssociatedObject.DataControl.SetCellValueCore(AssociatedObject.FocusedRowHandle, e.Column.FieldName, value);
				}));
				e.Handled = true;
			}
		}
		protected override void OnDetaching() {
			AssociatedObject.CellValueChanging += OnGroupSortItemProeprtyChanging;
			AssociatedObject.CellValueChanged -= OnGroupSortItemPropertyChanged;
			base.OnDetaching();
		}
	}
}
