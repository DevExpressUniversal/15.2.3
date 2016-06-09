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
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Grid.Automation;
using DevExpress.Xpf.Grid.Helpers;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.GridData;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.Grid {
	[DXToolboxBrowsable]
	[DevExpress.Utils.Design.DataAccess.DataAccessMetadata("All", SupportedProcessingModes = "All")]
	public partial class GridControl : GridDataControlBase, IDataProviderOwner, IDataProviderEvents, IDataControllerVisualClient, ISupportInitialize, IDetailElement<DataControlBase> {
		public static readonly DependencyProperty ViewProperty;
		public static readonly RoutedEvent StartSortingEvent, EndSortingEvent;
		public static readonly RoutedEvent StartGroupingEvent, EndGroupingEvent;
		public static readonly RoutedEvent GroupRowExpandingEvent;
		public static readonly RoutedEvent GroupRowExpandedEvent;
		public static readonly RoutedEvent GroupRowCollapsingEvent;
		public static readonly RoutedEvent GroupRowCollapsedEvent;
		public static readonly RoutedEvent CustomGroupDisplayTextEvent;
		public static readonly RoutedEvent MasterRowExpandingEvent;
		public static readonly RoutedEvent MasterRowCollapsingEvent;
		public static readonly RoutedEvent MasterRowExpandedEvent;
		public static readonly RoutedEvent MasterRowCollapsedEvent;
		protected static readonly DependencyPropertyKey IsGroupedPropertyKey;
		public static readonly DependencyProperty IsGroupedProperty;
		static readonly DependencyPropertyKey ActualGroupCountPropertyKey;
		public static readonly DependencyProperty ActualGroupCountProperty;
		[Obsolete("Instead use the ItemsSource property. For detailed information, see the list of breaking changes in DXperience v2011 vol 1.")]
		public static readonly DependencyProperty DataSourceProperty;
		public static readonly DependencyProperty AutoExpandAllGroupsProperty;
		public static readonly DependencyProperty IsRecursiveExpandProperty;
		public static readonly DependencyProperty AllowLiveDataShapingProperty;
		static readonly DependencyPropertyKey IsAsyncOperationInProgressPropertyKey;
		public static readonly DependencyProperty IsAsyncOperationInProgressProperty;
		public static readonly RoutedEvent AsyncOperationStartedEvent;
		public static readonly RoutedEvent AsyncOperationCompletedEvent;
		public static readonly RoutedEvent CopyingToClipboardEvent;
		public static readonly RoutedEvent SelectionChangedEvent;
		public static readonly System.Windows.DependencyProperty GroupSummaryGeneratorTemplateProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly System.Windows.DependencyProperty GroupSummaryItemsAttachedBehaviorProperty;
		public static readonly System.Windows.DependencyProperty GroupSummarySourceProperty;
		public static readonly DependencyProperty DetailDescriptorProperty;
		static readonly DependencyPropertyKey BandsLayoutPropertyKey;
		public static readonly DependencyProperty BandsLayoutProperty;
		public static readonly DependencyProperty OptimizeSummaryCalculationProperty;
		static GridControl() {
#if !SL
#endif
			Type ownerType = typeof(GridControl);
#pragma warning disable 618
			DataSourceProperty = DependencyPropertyManager.Register("DataSource", typeof(object), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((GridControl)d).ItemsSource = e.NewValue));
#pragma warning restore 618
			ViewProperty = DependencyPropertyManager.Register("View", typeof(DataViewBase), ownerType, new FrameworkPropertyMetadata(null, OnViewChanged, OnCoerceView));
			AutoExpandAllGroupsProperty = DependencyPropertyManager.Register("AutoExpandAllGroups", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((GridControl)d).OnAutoExpandAllGroupsChanged()));
			IsRecursiveExpandProperty = DependencyPropertyManager.Register("IsRecursiveExpand", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			AllowLiveDataShapingProperty = DependencyPropertyManager.Register("AllowLiveDataShaping", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((GridControl)d).OnAllowLiveDataShapingChanged()));
			IsGroupedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsGrouped", typeof(bool), ownerType, new PropertyMetadata(false));
			IsGroupedProperty = IsGroupedPropertyKey.DependencyProperty;
			ActualGroupCountPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGroupCount", typeof(int), ownerType, new PropertyMetadata(0, (d, e) => ((GridControl)d).OnActualGroupCountChanged()));
			ActualGroupCountProperty = ActualGroupCountPropertyKey.DependencyProperty;
			StartSortingEvent = EventManager.RegisterRoutedEvent("StartSorting", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			EndSortingEvent = EventManager.RegisterRoutedEvent("EndSorting", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			StartGroupingEvent = EventManager.RegisterRoutedEvent("StartGrouping", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			EndGroupingEvent = EventManager.RegisterRoutedEvent("EndGrouping", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			GroupRowExpandingEvent = EventManager.RegisterRoutedEvent("GroupRowExpanding", RoutingStrategy.Direct, typeof(RowAllowEventHandler), ownerType);
			GroupRowExpandedEvent = EventManager.RegisterRoutedEvent("GroupRowExpanded", RoutingStrategy.Direct, typeof(RowEventHandler), ownerType);
			GroupRowCollapsingEvent = EventManager.RegisterRoutedEvent("GroupRowCollapsing", RoutingStrategy.Direct, typeof(RowAllowEventHandler), ownerType);
			GroupRowCollapsedEvent = EventManager.RegisterRoutedEvent("GroupRowCollapsed", RoutingStrategy.Direct, typeof(RowEventHandler), ownerType);
			MasterRowExpandingEvent = EventManager.RegisterRoutedEvent("MasterRowExpanding", RoutingStrategy.Direct, typeof(RowAllowEventHandler), ownerType);
			MasterRowCollapsingEvent = EventManager.RegisterRoutedEvent("MasterRowCollapsing", RoutingStrategy.Direct, typeof(RowAllowEventHandler), ownerType);
			MasterRowExpandedEvent = EventManager.RegisterRoutedEvent("MasterRowExpanded", RoutingStrategy.Direct, typeof(RowEventHandler), ownerType);
			MasterRowCollapsedEvent = EventManager.RegisterRoutedEvent("MasterRowCollapsed", RoutingStrategy.Direct, typeof(RowEventHandler), ownerType);
			CustomGroupDisplayTextEvent = EventManager.RegisterRoutedEvent("CustomGroupDisplayText", RoutingStrategy.Direct, typeof(CustomGroupDisplayTextEventHandler), ownerType);
			IsAsyncOperationInProgressPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsAsyncOperationInProgress", typeof(bool), ownerType, new PropertyMetadata(false, OnIsAsyncOperationInProgressChanged));
			IsAsyncOperationInProgressProperty = IsAsyncOperationInProgressPropertyKey.DependencyProperty;
			GroupSummaryGeneratorTemplateProperty = DependencyProperty.Register("GroupSummaryGeneratorTemplate", typeof(DataTemplate), ownerType, new System.Windows.PropertyMetadata(OnGroupSummaryItemsGeneratorTemplatePropertyChanged));
			GroupSummaryItemsAttachedBehaviorProperty = DependencyProperty.Register("GroupSummaryItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<DataControlBase, SummaryItemBase>), ownerType, new System.Windows.PropertyMetadata(null));
			GroupSummarySourceProperty = DependencyProperty.Register("GroupSummarySource", typeof(IEnumerable), ownerType, new System.Windows.PropertyMetadata((d, e) => ItemsAttachedBehaviorCore<DataControlBase, SummaryItemBase>.OnItemsSourcePropertyChanged(d, e, GroupSummaryItemsAttachedBehaviorProperty, GroupSummaryGeneratorTemplateProperty, null, null, grid => grid.GroupSummaryCore, grid => grid.CreateSummaryItem())));
			DetailDescriptorProperty = DependencyPropertyManager.Register("DetailDescriptor", typeof(DetailDescriptorBase), ownerType, new PropertyMetadata(null, (d, e) => ((GridControl)d).OnDetailDescriptorChanged((DetailDescriptorBase)e.OldValue)));
			AsyncOperationStartedEvent = EventManager.RegisterRoutedEvent("AsyncOperationStarted", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			AsyncOperationCompletedEvent = EventManager.RegisterRoutedEvent("AsyncOperationCompleted", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			BandsLayoutPropertyKey = DependencyPropertyManager.RegisterReadOnly("BandsLayout", typeof(GridControlBandsLayout), ownerType, new PropertyMetadata(null, (d, e) => ((GridControl)d).OnBandsLayoutChanged(e.OldValue as BandsLayoutBase, e.NewValue as BandsLayoutBase)));
			BandsLayoutProperty = BandsLayoutPropertyKey.DependencyProperty;
			OptimizeSummaryCalculationProperty = DependencyPropertyManager.Register("OptimizeSummaryCalculation", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((GridControl)d).OnOptimizeSummaryCalculationChanged()));
			CopyingToClipboardEvent = EventManager.RegisterRoutedEvent("CopyingToClipboard", RoutingStrategy.Direct, typeof(CopyingToClipboardEventHandler), ownerType);
			SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Direct, typeof(GridSelectionChangedEventHandler), ownerType);
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.SelectAll, (d, e) => ((GridControl)d).SelectAllMasterDetail(), (d, e) => ((GridControl)d).CanSelectAll(d, e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Copy, (d, e) => ((GridControl)d).Copy(), (d, e) => ((GridControl)d).CanCopy(d, e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Paste, (d, e) => ((GridControl)d).Paste(), (d, e) => ((GridControl)d).CanPaste(d, e)));
			DXSerializer.SerializationProviderProperty.OverrideMetadata(ownerType, new UIPropertyMetadata(new GridControlSerializationProvider()));
		}
		static void OnGroupSummaryItemsGeneratorTemplatePropertyChanged(System.Windows.DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<DataControlBase, SummaryItemBase>.OnItemsGeneratorTemplatePropertyChanged(d, e, GroupSummaryItemsAttachedBehaviorProperty);
		}
		static void OnIsAsyncOperationInProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GridControl)d).OnIsAsyncOperationInProgressChanged((bool)e.NewValue);
		}
		internal static GridViewBase CreateDefaultGridView() {
			return new TableView();
		}
		ObservableCollectionCore<GridColumn> groupedColumns;
		GridColumnDataEventHandler customUnboundColumnData;
		EventHandler<SubstituteFilterEventArgs> substituteFilter;
		EventHandler<SubstituteSortInfoEventArgs> substituteSortInfo;
		CustomSummaryEventHandler customSummary;
		CustomSummaryExistEventHandler customSummaryExists;
		CustomColumnSortEventHandler customColumnSort, customColumnGroup;
		RowFilterEventHandler customRowFilter;
		GridColumnSortData sortData;
		GridFilterData filterData;
		GridFilterData searchFilterData;
		readonly AsyncOperationWaitHandle asyncOpWaitHandle = new AsyncOperationWaitHandle();
		bool isAsyncOperationInProgress;
		internal IDesignTimeGridAdorner DesignTimeGridAdorner { get { return (IDesignTimeGridAdorner)DesignTimeAdorner; } }
		protected internal override IDesignTimeAdornerBase EmptyDesignTimeAdorner { get { return EmptyDesignTimeGridAdorner.Instance; } }
		public GridControl() : this(null) { }
		public GridControl(IDataControlOriginationElement dataControlOriginationElement) : base(dataControlOriginationElement) {
			this.SetDefaultStyleKey(typeof(GridControl));
			GroupSummarySortInfo.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnGroupSummarySortInfoChanged);
			GroupSummary.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnGroupSummaryCollectionChanged);
			visualClientUpdater = new VisualClientUpdater(this);
		}
		void OnGroupSummarySortInfoChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			OnSortInfoChanged(sender, e);
			GetOriginationDataControl().syncPropertyLocker.DoLockedActionIfNotLocked(() => {
				DataControlOriginationElementHelper.EnumerateDependentElemets<GridControl>(this, grid => (GridControl)grid, grid => CloneGroupSummarySortInfo(grid));
			});
		}
		#region hidden properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new Thickness BorderThickness { get { return base.BorderThickness; } set { base.BorderThickness = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new Brush Background { get { return base.Background; } set { base.Background = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new Brush BorderBrush { get { return base.BorderBrush; } set { base.BorderBrush = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new Brush Foreground { get { return base.Foreground; } set { base.Foreground = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new Thickness Padding { get { return base.Padding; } set { base.Padding = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new HorizontalAlignment HorizontalContentAlignment { get { return base.HorizontalContentAlignment; } set { base.HorizontalContentAlignment = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public new VerticalAlignment VerticalContentAlignment { get { return base.VerticalContentAlignment; } set { base.VerticalContentAlignment = value; } }
		#endregion
		protected internal GridDataProviderBase GridDataProvider { get { return (GridDataProviderBase)DataProviderBase; } }
#if DEBUGTEST
		protected internal GridDataProvider DataProvider { get { return (GridDataProvider)DataProviderBase; } }
#endif
		[CloneDetailMode(CloneDetailMode.Skip)]
		public IEnumerable GroupSummarySource {
			get { return (IEnumerable)GetValue(GroupSummarySourceProperty); }
			set { SetValue(GroupSummarySourceProperty, value); }
		}
		[CloneDetailMode(CloneDetailMode.Skip)]
		public DataTemplate GroupSummaryGeneratorTemplate {
			get { return (DataTemplate)GetValue(GroupSummaryGeneratorTemplateProperty); }
			set { SetValue(GroupSummaryGeneratorTemplateProperty, value); }
		}
		public event RoutedEventHandler AsyncOperationStarted {
			add { AddHandler(AsyncOperationStartedEvent, value); }
			remove { RemoveHandler(AsyncOperationStartedEvent, value); }
		}
		public event RoutedEventHandler AsyncOperationCompleted {
			add { AddHandler(AsyncOperationCompletedEvent, value); }
			remove { RemoveHandler(AsyncOperationCompletedEvent, value); }
		}
		[Category("Events")]
		public event GridColumnDataEventHandler CustomUnboundColumnData {
			add { customUnboundColumnData += value; }
			remove { customUnboundColumnData -= value; }
		}
		[Category("Events")]
		public event EventHandler<SubstituteFilterEventArgs> SubstituteFilter {
			add { substituteFilter += value; }
			remove { substituteFilter -= value; }
		}
		[Category("Events")]
		public event EventHandler<SubstituteSortInfoEventArgs> SubstituteSortInfo {
			add { substituteSortInfo += value; }
			remove { substituteSortInfo -= value; }
		}
		[Category("Events")]
		public event CustomSummaryEventHandler CustomSummary {
			add { customSummary += value; }
			remove { customSummary -= value; }
		}
		[Category("Events")]
		public event CustomSummaryExistEventHandler CustomSummaryExists {
			add { customSummaryExists += value; }
			remove { customSummaryExists -= value; }
		}
		[Category("Events")]
		public event CustomColumnSortEventHandler CustomColumnSort {
			add { customColumnSort += value; }
			remove { customColumnSort -= value; }
		}
		[Category("Events")]
		public event CustomColumnSortEventHandler CustomColumnGroup {
			add { customColumnGroup += value; }
			remove { customColumnGroup -= value; }
		}
		[Category("Events")]
		public event RowFilterEventHandler CustomRowFilter {
			add { 
				customRowFilter += value;
				if(!IsLoading) RefreshData();
			}
			remove { customRowFilter -= value; }
		}
		[Category("Events")]
		public event RoutedEventHandler StartSorting {
			add { AddHandler(StartSortingEvent, value); }
			remove { RemoveHandler(StartSortingEvent, value); }
		}
		[Category("Events")]
		public event RoutedEventHandler EndSorting {
			add { AddHandler(EndSortingEvent, value); }
			remove { RemoveHandler(EndSortingEvent, value); }
		}
		[Category("Events")]
		public event RoutedEventHandler StartGrouping {
			add { AddHandler(StartGroupingEvent, value); }
			remove { RemoveHandler(StartGroupingEvent, value); }
		}
		[Category("Events")]
		public event RoutedEventHandler EndGrouping {
			add { AddHandler(EndGroupingEvent, value); }
			remove { RemoveHandler(EndGroupingEvent, value); }
		}
		[Category("Behavior")]
		public event RowAllowEventHandler GroupRowExpanding {
			add { AddHandler(GroupRowExpandingEvent, value); }
			remove { RemoveHandler(GroupRowExpandingEvent, value); }
		}
		[Category("Behavior")]
		public event RowEventHandler GroupRowExpanded {
			add { AddHandler(GroupRowExpandedEvent, value); }
			remove { RemoveHandler(GroupRowExpandedEvent, value); }
		}
		[Category("Behavior")]
		public event RowAllowEventHandler GroupRowCollapsing {
			add { AddHandler(GroupRowCollapsingEvent, value); }
			remove { RemoveHandler(GroupRowCollapsingEvent, value); }
		}
		[Category("Behavior")]
		public event RowEventHandler GroupRowCollapsed {
			add { AddHandler(GroupRowCollapsedEvent, value); }
			remove { RemoveHandler(GroupRowCollapsedEvent, value); }
		}
		[Category("Behavior")]
		public event RowAllowEventHandler MasterRowExpanding {
			add { AddHandler(MasterRowExpandingEvent, value); }
			remove { RemoveHandler(MasterRowExpandingEvent, value); }
		}
		[Category("Behavior")]
		public event RowAllowEventHandler MasterRowCollapsing {
			add { AddHandler(MasterRowCollapsingEvent, value); }
			remove { RemoveHandler(MasterRowCollapsingEvent, value); }
		}
		[Category("Behavior")]
		public event RowEventHandler MasterRowExpanded {
			add { AddHandler(MasterRowExpandedEvent, value); }
			remove { RemoveHandler(MasterRowExpandedEvent, value); }
		}
		[Category("Behavior")]
		public event RowEventHandler MasterRowCollapsed {
			add { AddHandler(MasterRowCollapsedEvent, value); }
			remove { RemoveHandler(MasterRowCollapsedEvent, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridControlAutoExpandAllGroups"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AutoExpandAllGroups {
			get { return (bool)GetValue(AutoExpandAllGroupsProperty); }
			set { SetValue(AutoExpandAllGroupsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridControlIsRecursiveExpand"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool IsRecursiveExpand {
			get { return (bool)GetValue(IsRecursiveExpandProperty); }
			set { SetValue(IsRecursiveExpandProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridControlAllowLiveDataShaping"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool? AllowLiveDataShaping {
			get { return (bool?)GetValue(AllowLiveDataShapingProperty); }
			set { SetValue(AllowLiveDataShapingProperty, value); }
		}
		[Category(Categories.Data)]
		public event CustomGroupDisplayTextEventHandler CustomGroupDisplayText {
			add { AddHandler(CustomGroupDisplayTextEvent, value); }
			remove { RemoveHandler(CustomGroupDisplayTextEvent, value); }
		}
		[Category(Categories.Data)]
		public event CustomColumnDisplayTextEventHandler CustomColumnDisplayText;
		[Category(Categories.OptionsCopy)]
		public event CopyingToClipboardEventHandler CopyingToClipboard {
			add { AddHandler(CopyingToClipboardEvent, value); }
			remove { RemoveHandler(CopyingToClipboardEvent, value); }
		}
		[Category(Categories.OptionsSelection)]
		public event GridSelectionChangedEventHandler SelectionChanged {
			add { AddHandler(SelectionChangedEvent, value); }
			remove { RemoveHandler(SelectionChangedEvent, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridControlIsGrouped")]
#endif
		public bool IsGrouped { get { return (bool)GetValue(IsGroupedProperty); } }
		internal AsyncOperationWaitHandle AsyncWaitHandle { get { return this.asyncOpWaitHandle; } }
		protected internal override void SetIsGrouped(bool value) { 
			this.SetValue(IsGroupedPropertyKey, value);
			if(View != null)
				View.UpdateMasterDetailViewProperties();
		}
		protected virtual void RaiseCustomUnboundColumnData(GridColumnDataEventArgs e) {
			GetOriginationGridControl().RaiseCustomUnboundColumnDataCore(e);
		}
		void RaiseCustomUnboundColumnDataCore(GridColumnDataEventArgs e) {
			if(customUnboundColumnData != null)
				customUnboundColumnData(this, e);
		}
		protected virtual void RaiseSubstituteFilter(SubstituteFilterEventArgs e) {
			GetOriginationGridControl().RaiseSubstituteFilterCore(e);
		}
		void RaiseSubstituteFilterCore(SubstituteFilterEventArgs e) {
			if(substituteFilter != null)
				substituteFilter(this, e);
		}
		protected virtual void RaiseSubstituteSortInfo(SubstituteSortInfoEventArgs e) {
			GetOriginationGridControl().RaiseSubstituteSortInfoCore(e);
		}
		void RaiseSubstituteSortInfoCore(SubstituteSortInfoEventArgs e) {
			if(substituteSortInfo != null)
				substituteSortInfo(this, e);
		}
		protected virtual void RaiseCustomSummaryExists(object sender, CustomSummaryExistEventArgs e) {
			GetOriginationGridControl().RaiseCustomSummaryExistsCore(sender, e);
		}
		void RaiseCustomSummaryExistsCore(object sender, CustomSummaryExistEventArgs e) {
			if(customSummaryExists != null)
				customSummaryExists(sender, e);
		}
		protected virtual void RaiseCustomSummary(object sender, CustomSummaryEventArgs e) {
#if !SL
			ServiceSummaryItem serviceSummaryItem = e.Item as ServiceSummaryItem;
			if(serviceSummaryItem != null && serviceSummaryItem.SummaryType == SummaryItemType.Custom) {
				ProcessServiceSummary(this, e, serviceSummaryItem);
				return;
			}
#endif
			GetOriginationGridControl().RaiseCustomSummaryCore(sender, e);
		}
		void RaiseCustomSummaryCore(object sender, CustomSummaryEventArgs e) {
			if(customSummary != null)
				customSummary(sender, e);
		}
		protected internal virtual void RaiseCustomColumnSort(CustomColumnSortEventArgs e) {
			GetOriginationGridControl().RaiseCustomColumnSortCore(e);
		}
		protected internal override bool RaiseCopyingToClipboard(CopyingToClipboardEventArgsBase e) {
			e.RoutedEvent = GridControl.CopyingToClipboardEvent;
			RaiseEventInOriginationGrid(e);
			return e.Handled;
		}
		protected internal override void RaiseSelectionChanged(GridSelectionChangedEventArgs e) {
			e.RoutedEvent = GridControl.SelectionChangedEvent;
			RaiseEventInOriginationGrid(e);
		}
		void RaiseCustomColumnSortCore(CustomColumnSortEventArgs e) {
			if(customColumnSort != null)
				customColumnSort(this, e);
		}
		protected internal virtual void RaiseCustomColumnGroup(CustomColumnSortEventArgs e) {
			GetOriginationGridControl().RaiseCustomColumnGroupCore(e);
		}
		void RaiseCustomColumnGroupCore(CustomColumnSortEventArgs e) {
			if(customColumnGroup != null)
				customColumnGroup(this, e);
		}
		protected bool HasCustomRowFilter { get { return GetOriginationGridControl().customRowFilter != null; } }
		protected virtual int RaiseCustomRowFilter(int listSourceRowIndex, bool fit) {
			RowFilterEventArgs e = new RowFilterEventArgs(this, listSourceRowIndex, fit);
			GetOriginationGridControl().RaiseCustomRowFilterCore(e);
			if(e.Handled) return e.Visible ? 1 : 0;
			return -1;
		}
		void RaiseCustomRowFilterCore(RowFilterEventArgs e) {
			if(customRowFilter != null)
				customRowFilter(this, e);
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridControlIsAsyncOperationInProgress")]
#endif
		public bool IsAsyncOperationInProgress {
			get { return (bool)GetValue(IsAsyncOperationInProgressProperty); }
			private set { this.SetValue(IsAsyncOperationInProgressPropertyKey, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public DataController DataController { get { return DataProviderBase.DataController; } }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridControlGroupCount"),
#endif
 Category(Categories.Data), DefaultValue(0), XtraSerializableProperty, GridUIProperty, Browsable(false)]
		public int GroupCount {
			get { return SortInfo.GroupCount; }
			set { SortInfo.GroupCount = value; }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridControlActualGroupCount")]
#endif
		public int ActualGroupCount {
			get { return (int)GetValue(ActualGroupCountProperty); }
			private set { this.SetValue(ActualGroupCountPropertyKey, value); }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		XtraSerializableProperty(true, false, false, GridSerializationOrders.GridControl_GroupSummarySortInfo),
		GridUIProperty,
		XtraResetProperty,
		]
		public GridGroupSummarySortInfoCollection GroupSummarySortInfo { get { return GridDataProvider.GroupSummarySortInfo; } }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridControlGroupSummary"),
#endif
 Category(Categories.Data), XtraSerializableProperty(true, false, false), GridUIProperty, XtraResetProperty]
		public GridSummaryItemCollection GroupSummary { get { return (GridSummaryItemCollection)GroupSummaryCore; } }
		[Category(Categories.MasterDetail)]
		public DetailDescriptorBase DetailDescriptor {
			get { return (DetailDescriptorBase)GetValue(DetailDescriptorProperty); }
			set { SetValue(DetailDescriptorProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public GridControlBandsLayout BandsLayout {
			get { return (GridControlBandsLayout)GetValue(BandsLayoutProperty); }
			internal set { this.SetValue(BandsLayoutPropertyKey, value); }
		}
		[ Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool OptimizeSummaryCalculation {
			get { return (bool)GetValue(OptimizeSummaryCalculationProperty); }
			set { SetValue(OptimizeSummaryCalculationProperty, value); }
		}
		internal override DetailDescriptorBase DetailDescriptorCore { get { return DetailDescriptor; } }
		internal override int ActualGroupCountCore { get { return ActualGroupCount; } set { ActualGroupCount = value; } }
		internal IList<GridColumn> GroupedColumns {
			get {
				if(groupedColumns == null)
					RebuildGroupSortIndexesAndGroupedColumns();
				return groupedColumns;
			}
		}
		protected internal GridFilterData FilterData {
			get {
				if(filterData == null) {
					filterData = new GridFilterData(this);
					filterData.OnStart();
				}
				return filterData;
			}
		}
		protected internal GridFilterData SearchFilterData {
			get {
				if(searchFilterData == null) {
					if(View == null)
						return null;
					searchFilterData = new GridSearchFilterData(this);
					searchFilterData.OnStart();
				}
				return searchFilterData;
			}
		}
		protected internal GridColumnSortData SortData {
			get {
				if(sortData == null)
					sortData = new GridColumnSortData(this);
				return sortData;
			}
		}
		#region to override in TreeListControl
		[Obsolete("Instead use the ItemsSource property. For detailed information, see the list of breaking changes in DXperience v2011 vol 1.")]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), 
#if !SL
	DevExpressXpfGridLocalizedDescription("GridControlDataSource"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Bindable(true), Category(Categories.Data), CloneDetailMode(CloneDetailMode.Skip)]
#pragma warning disable 618
		public object DataSource {
			get { return GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}
#pragma warning restore 618
		[
Category(Categories.Data),
Browsable(false),
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
XtraSerializableProperty(true, false, false),
GridUIProperty,
XtraResetProperty,
]
		public GridSortInfoCollection SortInfo { get { return (GridSortInfoCollection)SortInfoCore; } }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridControlColumns"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(Categories.Data), XtraSerializableProperty(true, true, true), GridStoreAlwaysProperty]
		public GridColumnCollection Columns { get { return (GridColumnCollection)ColumnsCore; } }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridControlView"),
#endif
 Category(Categories.View), XtraSerializableProperty(XtraSerializationVisibility.Content), GridStoreAlwaysProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public DataViewBase View {
			get { return (DataViewBase)base.GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridControlTotalSummary"),
#endif
 Category(Categories.Data), XtraSerializableProperty(true, false, false), GridUIProperty, XtraResetProperty]
		public GridSummaryItemCollection TotalSummary { get { return (GridSummaryItemCollection)TotalSummaryCore; } }
		internal override DataViewBase DataView { get { return viewCore; } set { View = value; } }
		internal override SortInfoCollectionBase CreateSortInfo() {
			return new GridSortInfoCollection();
		}
		internal override ISummaryItemOwner CreateSummariesCollection(SummaryItemCollectionType collectionType) {
			return new GridSummaryItemCollection(this, collectionType);
		}
		internal override IColumnCollection CreateColumns() {
			return new GridColumnCollection(this);
		}
		protected override DataProviderBase CreateDataProvider() {
			return new GridDataProvider(this as IDataProviderOwner);
		}
		protected internal override DataViewBase CreateDefaultView() {
			return CreateDefaultGridView();
		}
		internal override void ValidateDataProvider(DataViewBase newValue) {
			if(View is TreeListView)
				fDataProvider = ((TreeListView)View).DataProviderBase;
			else if(DataProviderBase is TreeListDataProvider)
				fDataProvider = CreateDataProvider();
		}
		internal protected override Type ColumnType { get { return typeof(GridColumn); } }
		internal protected override Type BandType { get { return typeof(GridControlBand); } }
		internal protected override BandBase CreateBand() {
			return new GridControlBand();
		}
		protected internal override IList<DevExpress.Xpf.Grid.SummaryItemBase> GetGroupSummaries() {
			return new DevExpress.Utils.SimpleBridgeList<DevExpress.Xpf.Grid.SummaryItemBase, GridSummaryItem>(GroupSummary, item => item);
		}
		internal override SummaryItemBase CreateSummaryItem() {
			return new GridSummaryItem();
		}
		protected override void OnItemsSourceChanged(object oldValue, object newValue) {
			base.OnItemsSourceChanged(oldValue, newValue);
			if(View is GridViewBase)
				((GridViewBase)View).RefreshImmediateUpdateRowPositionProperty();
		}
		internal override ModelGridColumnsGeneratorBase CreateSmartModelGridColumnsGenerator(ColumnCreatorBase creator, bool applyOnlyForSmartColumns, bool skipXamlGenerationProperties) {
			return new SmartModelGridColumnsGenerator(creator, applyOnlyForSmartColumns, skipXamlGenerationProperties);
		}
		#endregion
		#region grid-specific stuff
		#region automation
		protected override AutomationPeer OnCreateAutomationPeer() {
			if(AutomationPeer == null) 
				AutomationPeer = new GridControlAutomationPeer(this);
			return AutomationPeer;
		}
		protected override PeerCacheBase CreatePeerCache() {
			return new PeerCache();
		}
		#endregion
		#region grouping
#if !DEBUGTEST
		protected internal 
#else
		public
#endif
		void ChangeGroupExpanded(int visibleIndex) {
			ChangeGroupExpandedCore(GetRowHandleByVisibleIndex(visibleIndex), false);
		}
		protected internal void ExpandGroupRowWithEvents(int rowHandle, bool recursive) {
			if(!RaiseGroupRowExpanding(rowHandle)) return;
			ChangeGroupExpandedCore(rowHandle, recursive);
			RaiseGroupRowExpanded(rowHandle);
		}
		protected internal void CollapseGroupRowWithEvents(int rowHandle, bool recursive) {
			if(!RaiseGroupRowCollapsing(rowHandle)) return;
			ChangeGroupExpandedCore(rowHandle, recursive);
			OnGroupRowCollapsed(rowHandle);
		}
#if !DEBUGTEST
		protected internal 
#else
		public
#endif
		void ChangeGroupExpandedCore(int rowHandle, bool recursive) {
			PurgeCache(GetRowVisibleIndexByHandle(rowHandle));
			View.SupressCacheCleanCountLocker.DoLockedAction(delegate {
				DataProviderBase.ChangeGroupExpanded(rowHandle, recursive);
			});
		}
		void PurgeCache(int visibleIndex) {
			if(!DataProviderBase.IsServerMode)
				return;
			List<int> itemsToRemove = new List<int>();
			foreach(KeyValuePair<int, DataRowNode> pair in DataView.Nodes) {
				if(GetRowVisibleIndexByHandleCore(pair.Key) > visibleIndex)
					itemsToRemove.Add(pair.Key);
			}
			foreach(int rowHandle in itemsToRemove) {
				DataView.Nodes.Remove(rowHandle);
			}
		}
		public void UpdateGroupSummary() {
			DataProviderBase.UpdateGroupSummary();
			if(View != null)
				View.UpdateGroupSummary();
		}
		internal override void ReassignGroupedColumns(List<ColumnBase> groupedColumnsList) {
			if(groupedColumns == null)
				groupedColumns = new ObservableCollectionCore<GridColumn>();
			IList<GridColumn> groupedColumnsListConverted = new DevExpress.Utils.SimpleBridgeList<GridColumn, ColumnBase>(groupedColumnsList);
			if(!ListHelper.AreEqual<GridColumn>(groupedColumns, groupedColumnsListConverted)) {
				needsDataReset = true;
				groupedColumns.Assign(groupedColumnsListConverted);
				DesignTimeGridAdorner.OnColumnsLayoutChanged();
			}
		}
		internal override void SyncSortBySummaryInfo() {
			foreach(GridColumn column in Columns) {
				column.IsSortedBySummary = false;
			}
			foreach(GridGroupSummarySortInfo item in GroupSummarySortInfo) {
				GridColumn column = Columns[item.FieldName];
				if(column != null) {
					column.IsSortedBySummary = true;
					column.SortOrder = GridSortInfo.GetColumnSortOrder(item.SortOrder);
				}
			}
		}
		internal void ApplyColumnGroupIndex(ColumnBase column) {
			ApplyGroupSortIndexIfNotLoading(column, ApplyColumnGroupIndexWithoutLoadingCheck);
		}
		void ApplyColumnGroupIndexWithoutLoadingCheck(ColumnBase column) {
			ApplyGroupSortIndexCore(column, col => GroupBy((GridColumn)col, GetActualSortOrder(col.SortOrder), ((GridColumn)col).GroupIndex));
		}
		internal override void CroupByCore(ColumnBase column) {
			GroupBy((GridColumn)column, column.SortOrder, column.GroupIndexCore);
		}
		internal override void ApplyGroupSortIndexIfNotLoadingCore(ColumnBase column) {
			ApplyGroupSortIndexIfNotLoading(column, ApplyColumnGroupIndex);
		}
		protected override void RebuildGroupedColumnsInfo(List<ColumnBase> groupedColumns) {
			base.RebuildGroupedColumnsInfo(groupedColumns);
			groupedColumns.Sort((column1, column2) => Comparer<int>.Default.Compare(column1.GroupIndexCore, column2.GroupIndexCore));
			groupedColumns.ForEach(ApplyColumnGroupIndexWithoutLoadingCheck);
		}
		void UpdateInvalidGroupCache() {
			InvalidGroupCache.Clear();
			if(DataProviderBase.CollectionViewSource == null || DataProviderBase.CollectionViewSource.GroupDescriptions == null) return;
			foreach(GroupDescription group in DataProviderBase.CollectionViewSource.GroupDescriptions) {
				PropertyGroupDescription description = group as PropertyGroupDescription;
				if(description == null) continue;
				if(Columns[description.PropertyName] == null)
					InvalidGroupCache.Add(description.PropertyName, description);
			}
		}
		#endregion
#if !SL
		protected void Copy() {
			if(DataView.MasterRootRowsContainer.FocusedView.ClipboardMode == ClipboardMode.Formatted)
				DataView.MasterRootRowsContainer.FocusedView.Do(dataView => dataView.SelectionStrategy.CopyMasterDetailToClipboard());
			else
				DataView.Do(dataView => dataView.SelectionStrategy.CopyMasterDetailToClipboard());
		}
		internal void CopyAllSelectedItemsToClipboard() {
			IEnumerable<KeyValuePair<DataControlBase, int>> selectedRows = GetSelectedRows();
			if(selectedRows.Count() > 0) {
				DataView.Do(view => view.CopyAllRowsToClipboardCore(selectedRows));
			}
			else if(View.FocusedView.FocusedRowHandle != InvalidRowHandle) {
				var focusedRow = new KeyValuePair<DataControlBase, int>(View.FocusedView.DataControl, View.FocusedView.FocusedRowHandle);
				DataView.Do(view => view.CopyAllRowsToClipboardCore(new List<KeyValuePair<DataControlBase, int>> { focusedRow }));
			}
		}
		internal IEnumerable<KeyValuePair<DataControlBase, int>> GetSelectedRows() {
			List<KeyValuePair<DataControlBase, int>> selectedRows = new List<KeyValuePair<DataControlBase,int>>();
			for(int visibleIndex = 0; visibleIndex < VisibleRowCount; visibleIndex++) {
				int rowHandle = GetRowHandleByVisibleIndex(visibleIndex);
				if(DataView.IsRowSelected(rowHandle))
					selectedRows.Add(new KeyValuePair<DataControlBase, int>(this, rowHandle));
				if(IsMasterRowExpanded(rowHandle)) {
					GridControl detailGrid = (GridControl)GetVisibleDetail(rowHandle);
					if(detailGrid != null)
						selectedRows.AddRange(detailGrid.GetSelectedRows());
				}
			}
			return selectedRows;
		}
		protected internal void Paste() {
			if(!RaisePastingFromClipboard())
				View.RaisePastingFromClipboard();
		}
		protected void CanCopy(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = View.CanCopyRows();
		}
		protected void CanPaste(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}
		protected void CanSelectAll(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}
#endif
		protected internal override void DestroyFilterData() {
			DestroyFilterDataInternal();
			DestroySearchFilterData();
		}
		void DestroyFilterDataInternal() {
			if(this.filterData == null) return;
			this.filterData.Dispose();
			this.filterData = null;
		}
		void DestroySearchFilterData() {
			if(this.searchFilterData == null) return;
			this.searchFilterData.Dispose();
			this.searchFilterData = null;
		}
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("This member is not intended to be used directly from your code.")]
		public void ForceClearData() {
			if(!IsUnloaded) {
				IsUnloaded = true;
				DataProviderBase.ForceClearData();
			}
		}
		internal override bool GetIsExpandButtonVisible() {
			TableView currentView = View as TableView;
			if(currentView != null) {
				return currentView.ActualShowDetailButtons;
			}
			return false;
		}
		internal override void ClearGroupSummarySortInfo() {
			GroupSummarySortInfo.ClearCore();
		}
		protected virtual void RaiseAsyncOperationBegin() {
			RoutedEventArgs e = new RoutedEventArgs() { RoutedEvent = AsyncOperationStartedEvent };
			CheckIsOriginationDataControl(); 
			RaiseEventInOriginationGrid(e);
		}
		protected virtual void RaiseAsyncOperationEnd() {
			RoutedEventArgs e = new RoutedEventArgs() { RoutedEvent = AsyncOperationCompletedEvent };
			CheckIsOriginationDataControl(); 
			RaiseEventInOriginationGrid(e);
		}
		protected internal virtual bool RaiseGroupRowExpanding(int rowHandle) {
			return RaiseRowAllowEvent(GroupRowExpandingEvent, rowHandle);
		}
		protected internal virtual void RaiseGroupRowExpanded(int rowHandle) {
			RaiseEventInOriginationGrid(new RowEventArgs(GroupRowExpandedEvent, (GridViewBase)View, rowHandle));
		}
		protected internal virtual bool RaiseGroupRowCollapsing(int rowHandle) {
			return RaiseRowAllowEvent(GroupRowCollapsingEvent, rowHandle);
		}
		protected internal virtual void RaiseGroupRowCollapsed(int rowHandle) {
			RaiseEventInOriginationGrid(new RowEventArgs(GroupRowCollapsedEvent, (GridViewBase)View, rowHandle));
		}
		protected internal virtual bool RaiseMasterRowExpanding(int rowHandle) {
			return RaiseRowAllowEvent(MasterRowExpandingEvent, rowHandle);
		}
		protected internal virtual bool RaiseMasterRowCollapsing(int rowHandle) {
			return RaiseRowAllowEvent(MasterRowCollapsingEvent, rowHandle);
		}
		protected internal virtual void RaiseMasterRowExpanded(int rowHandle) { 
			RaiseEventInOriginationGrid(new RowEventArgs(MasterRowExpandedEvent, (GridViewBase)View, rowHandle));
		}
		protected internal virtual void RaiseMasterRowCollapsed(int rowHandle) {
			RaiseEventInOriginationGrid(new RowEventArgs(MasterRowCollapsedEvent, (GridViewBase)View, rowHandle));
		}
		protected bool RaiseRowAllowEvent(RoutedEvent routedEvent, int rowHandle) {
			GridViewBase view = View as GridViewBase;
			if(view == null) return false;
			RowAllowEventArgs e = new RowAllowEventArgs(routedEvent, (GridViewBase)View, rowHandle);
			RaiseEventInOriginationGrid(e);
			return e.Allow;
		}
		internal void OnGroupRowCollapsed(int rowHandle) {
			if(View != null && View.AllowFixedGroupsCore && IsRowVisible(rowHandle))
				View.ScrollAnimationLocker.DoLockedAction(() => View.ScrollIntoView(rowHandle));
			RaiseGroupRowCollapsed(rowHandle);
		}
		void OnIsAsyncOperationInProgressChanged(bool newValue) {
			GridViewBase view = View as GridViewBase;
			if(view == null) return;
			this.isAsyncOperationInProgress = newValue;
			if(newValue) {
				view.SetWaitIndicator();
				RaiseAsyncOperationBegin();
			} else {
				view.ClearWaitIndicator();
				RaiseAsyncOperationEnd();
			}
			this.asyncOpWaitHandle.IsInterrupted = !newValue;			
		}
		internal void OnAllowLiveDataShapingChanged() {
			if(DataProviderBase.IsSelfManagedItemsSource)
				return;
			DataProviderBase.OnDataSourceChanged();
			if(View != null) {
				if(!DataProviderBase.SubscribeRowChangedForVisibleRows)
					View.UpdateRowData(rowData => rowData.UnsubcribePropertyChanged(rowData.Row));
				else
					View.UpdateRowData(rowData => rowData.SubcribePropertyChanged(rowData.Row));
			}
		}
		void OnOptimizeSummaryCalculationChanged() {
			DataProviderBase.OnDataSourceChanged();
		}
		void OnActualGroupCountChanged() {
			UpdateAllDetailViewIndents();
		}
		protected virtual void OnAutoExpandAllGroupsChanged() {
			if(DataProviderBase == null || DataProviderBase.AutoExpandAllGroups == AutoExpandAllGroups) return;
			DataProviderBase.AutoExpandAllGroups = AutoExpandAllGroups;
			if(AutoExpandAllGroups)
				ExpandAllGroups();
		}
		CustomColumnDisplayTextEventArgs customColumnDisplayTextEventArgs;
		protected internal virtual string RaiseCustomDisplayText(int? rowHandle, int? listSourceIndex, ColumnBase column, object value, string displayText) {
			if(GetOriginationGridControl().CustomColumnDisplayText == null)
				return displayText;
			if(customColumnDisplayTextEventArgs == null)
				customColumnDisplayTextEventArgs = new CustomColumnDisplayTextEventArgs();
			customColumnDisplayTextEventArgs.SetArgs((GridViewBase)View, rowHandle, listSourceIndex, (GridColumn)column, value, displayText);
			GetOriginationGridControl().RaiseCustomDisplayTextCore(customColumnDisplayTextEventArgs);
			return customColumnDisplayTextEventArgs.DisplayText;
		}
		protected internal virtual bool? RaiseCustomDisplayText(int? rowHandle, int? listSourceIndex, ColumnBase column, object value, string originalDisplayText, out string displayText) {
			displayText = RaiseCustomDisplayText(rowHandle, listSourceIndex, column, value, originalDisplayText);
			if(GetOriginationGridControl().CustomColumnDisplayText == null)
				return false;
			if(customColumnDisplayTextEventArgs.ShowAsNullText)
				return null;
			return true;
		}
		void RaiseCustomDisplayTextCore(CustomColumnDisplayTextEventArgs e) {
			CustomColumnDisplayText(this, e);
		}
		protected internal virtual string RaiseCustomGroupDisplayText(int rowHandle, GridColumn column, object value, string displayText) {
			CustomGroupDisplayTextEventArgs e = new CustomGroupDisplayTextEventArgs((GridViewBase)View, rowHandle, column, value, displayText);
			RaiseEventInOriginationGrid(e);
			return e.DisplayText;
		}
		#region IDataProviderOwner Members
		bool IDataProviderOwner.IsSynchronizedWithCurrentItem { get { return DataView != null && DataView.IsSynchronizedWithCurrentItem; } }
		void IDataProviderOwner.ValidateMasterDetailConsistency() {
			ValidateMasterDetailConsistency();
		}
		void IDataProviderOwner.OnSelectionChanged(DevExpress.Data.SelectionChangedEventArgs e) {
			View.OnSelectionChanged(e);
		}
		List<IColumnInfo> IDataProviderOwner.GetColumns() {
			List<IColumnInfo> list = new List<IColumnInfo>();
			foreach(GridColumn column in Columns) {
				list.Add(column as IColumnInfo);
			}
			return list;
		}
		IEnumerable<IColumnInfo> IDataProviderOwner.GetServiceUnboundColumns() {
			return viewCore.Return(x => x.ViewBehavior.GetServiceUnboundColumns(), () => Enumerable.Empty<IColumnInfo>());
		}
		IEnumerable<SummaryItemBase> IDataProviderOwner.GetServiceSummaries() {
			return viewCore.Return(x => x.ViewBehavior.GetServiceSummaries(), () => Enumerable.Empty<SummaryItemBase>());
		}
		bool IDataProviderOwner.IsDesignTime { get { return DesignerProperties.GetIsInDesignMode(this); } }
		void IDataProviderOwner.OnCurrentIndexChanged() {
			if(View == null || View.FocusedRowHandleChangedLocker.IsLocked || !View.IsFocusedView)
				return;
			if(View.IsNewItemRowHandle(View.FocusedRowHandle))
				DataProviderBase.CurrentControllerRow = DataControlBase.NewItemRowHandle;
			else if(!View.IsAutoFilterRowFocused)
				View.SetFocusOnCurrentControllerRow();
		}
		void IDataProviderOwner.OnCurrentIndexChanging(int newControllerRow) {
			if(View == null || View.FocusedRowHandle == newControllerRow || newControllerRow == NewItemRowHandle)
				return;
			CancelRowEditIfNeeded();
		}
		void IDataProviderOwner.OnCurrentRowChanged() {
			if(View != null && !object.Equals(CurrentItem, DataProviderBase.GetRowValue(View.FocusedRowHandle)) && !dataResetLocker.IsLocked && !View.FocusedRowHandleChangedLocker.IsLocked) {
				CancelRowEditIfNeeded();
				View.UpdateFocusedRowData();
			}
		}
		void CancelRowEditIfNeeded() {
			bool isEditing = DataProviderBase.IsCurrentRowEditing || View.IsEditing;
			bool isDataRowFocused = !View.IsNewItemRowFocused && !View.IsAutoFilterRowFocused;
			if(isEditing && isDataRowFocused)
				View.CancelRowEdit();
		}
		void IDataProviderOwner.OnItemChanged(ListChangedEventArgs e) {
			if(e.ListChangedType == ListChangedType.ItemChanged && DataProviderBase.CurrentControllerRow == GetRowHandleByListIndex(e.OldIndex))
				UpdateCurrentCellValue();
			if(View != null)
				View.SelectionStrategy.OnItemChanged(e);
		}
		bool IDataProviderOwner.HasCustomRowFilter() { return HasCustomRowFilter; }
		bool IDataProviderOwner.RequireDisplayText(DataColumnInfo column) {
			return FilterData.IsRequired(column);
		}
		string IDataProviderOwner.GetDisplayText(int listSourceIndex, DataColumnInfo column, object value, string columnName) {
			GridDataColumnInfo info = FilterData.GetInfo(column) as GridDataColumnInfo;
			if(info != null && info.Column.FieldName == columnName) return info.GetDisplayText(listSourceIndex, value);
			if(SearchFilterData != null) {
				info = SearchFilterData.GetInfo(column) as GridDataColumnInfo;
				if(info != null && columnName.StartsWith(DxFtsContainsHelper.DxFtsPropertyPrefix)) return info.GetDisplayText(listSourceIndex, value);
			}
			return string.Empty;
		}
		string[] IDataProviderOwner.GetFindToColumnNames() { 
			if(View == null)
				return null;
			GridControlColumnProviderBase columnProvider = GridControlColumnProviderBase.GetColumnProvider(this);
			if(columnProvider == null)
				return new List<string>().ToArray();
			return columnProvider.GetAllSearchColumns().ToArray();
		}
		bool IDataProviderOwner.RequireSortCell(DataColumnInfo sortColumn) {
			return SortData.IsRequired(sortColumn);
		}
		void IDataProviderOwner.OnListSourceChanged() {
			PopulateColumnsIfNeeded();
		}
		void IDataProviderOwner.OnStartNewItemRow() {
			TableView view = View as TableView;
			if(view != null)
				view.OnStartNewItemRow();
		}
		void IDataProviderOwner.OnEndNewItemRow() {
			TableView view = View as TableView;
			if(view != null)
				view.OnEndNewItemRow();
		}
		void IDataProviderOwner.RaiseCurrentRowUpdated(ControllerRowEventArgs e) {
			GridViewBase gridView = (GridViewBase)View;
			RowEventArgs eventArgs = new RowEventArgs(TableView.RowUpdatedEvent, gridView, e.RowHandle, e.Row);
			gridView.RaiseRowUpdated(eventArgs);
		}
		void IDataProviderOwner.RaiseCurrentRowCanceled(ControllerRowEventArgs e) {
			GridViewBase gridView = (GridViewBase)View;
			RowEventArgs eventArgs = new RowEventArgs(TableView.RowCanceledEvent, gridView, e.RowHandle);
			gridView.RaiseRowCanceled(eventArgs);
		}
		void IDataProviderOwner.RaiseValidatingCurrentRow(ValidateControllerRowEventArgs e) {
			GridViewBase gridView = (GridViewBase)View;
			GridRowValidationEventArgs eventArgs = new GridRowValidationEventArgs(this, e.Row, e.RowHandle, gridView);
			try {
				gridView.RaiseValidateRow(eventArgs);
			}
			catch(Exception exception) {
				SetRowStateError(e.RowHandle, new GridRowValidationError(exception.Message, exception, ErrorType.Default, e.RowHandle));
				throw exception;
			}
			if(eventArgs.IsValid)
				SetRowStateError(e.RowHandle, null);
			else {
				string errorText = eventArgs.ErrorContent != null ? eventArgs.ErrorContent.ToString() : string.Empty;
				SetRowStateError(e.RowHandle, new GridRowValidationError(errorText, null, ErrorType.Default, e.RowHandle));
				throw new WarningException(errorText);
			}
		}
		void IDataProviderOwner.OnPostRowException(ControllerRowExceptionEventArgs e) {
			((GridViewBase)View).OnPostRowException(e);
		}
		void IDataProviderOwner.SynchronizeGroupSortInfo(IList<IColumnInfo> sortList, int groupCount) {
			SynchronizeSortInfo(sortList, groupCount);
		}
		bool IDataProviderOwner.CanSortColumn(string fieldName) {
			return View != null && View.CanSortColumn(fieldName);
		}
		void IDataProviderOwner.RePopulateDataControllerColumns() {
			dataResetLocker.DoLockedAction(delegate {
				DataProviderBase.RePopulateColumns();
				if(View != null) View.UpdateColumnsViewInfo();
			});
		}
		void IDataProviderOwner.UpdateIsAsyncOperationInProgress(bool value) {
			IsAsyncOperationInProgress = value;
		}
		ColumnGroupInterval IDataProviderOwner.GetGroupInterval(string fieldName) {
			GridColumn column = Columns[fieldName];
			if(column == null) return ColumnGroupInterval.Default;
			return column.GroupInterval;
		}
		bool? IDataProviderOwner.AllowLiveDataShaping { get { return AllowLiveDataShaping; } }
		NewItemRowPosition IDataProviderOwner.NewItemRowPosition { 
			get 
			{
				TableView view = View as TableView;
				if(view != null)
					return view.NewItemRowPosition;
				return NewItemRowPosition.None;
			} 
		}
		bool IDataProviderOwner.ShowGroupSummaryFooter {
			get { return viewCore.ShowGroupSummaryFooter; }
		}
		Type IDataProviderOwner.ItemType { get { return DataProviderBase.ItemType; } }
		#endregion
		#region IDataProviderEvents Members
		object IDataProviderEvents.GetUnboundData(int listSourceRowIndex, string fieldName, object value) {
			GridColumn column = Columns[fieldName];
			if(column == null)
				return value;
			if(column.DisplayMemberBindingCalculator != null) {
				int rowHandle = GetRowHandleByListIndex(listSourceRowIndex);
				return column.DisplayMemberBindingCalculator.GetValue(rowHandle, listSourceRowIndex);
			}
			GridColumnDataEventArgs e = CreateArgs(listSourceRowIndex, fieldName, value, true);
			RaiseCustomUnboundColumnData(e);
			return e.Value;
		}
		void IDataProviderEvents.SetUnboundData(int listSourceRowIndex, string fieldName, object value) {
			GridColumn column = Columns[fieldName];
			if(column == null)
				return;
			if(column.DisplayMemberBindingCalculator != null) {
				int rowHandle = GetRowHandleByListIndex(listSourceRowIndex);
				column.DisplayMemberBindingCalculator.SetValue(rowHandle, value);
			}
			GridColumnDataEventArgs e = CreateArgs(listSourceRowIndex, fieldName, value, false);
			RaiseCustomUnboundColumnData(e);
		}
		void IDataProviderEvents.SubstituteFilter(SubstituteFilterEventArgs e) {
			RaiseSubstituteFilter(e);		
		}
		void IDataProviderEvents.SubstituteSortInfo(SubstituteSortInfoEventArgs e) {
			RaiseSubstituteSortInfo(e);
		}
		GridColumnDataEventArgs CreateArgs(int listSourceRowIndex, string fieldName, object value, bool isGetAction) {
			return new GridColumnDataEventArgs(this, Columns[fieldName], listSourceRowIndex, value, isGetAction);
		}
		void IDataProviderEvents.OnCustomSummaryExists(object sender, CustomSummaryExistEventArgs e) {
			RaiseCustomSummaryExists(sender, e);
		}
		void IDataProviderEvents.OnCustomSummary(object sender, CustomSummaryEventArgs e) {
			RaiseCustomSummary(sender, e);
		}
		int? IDataProviderEvents.OnCompareSortValues(int listSourceRowIndex1, int listSourceRowIndex2, object value1, object value2, DataColumnInfo sortColumn, ColumnSortOrder sortOrder) {
			GridDataColumnSortInfo info = SortData.GetSortInfo(sortColumn);
			if(info == null)
				return null;
			return info.CompareSortValues(listSourceRowIndex1, listSourceRowIndex2, value1, value2, sortOrder);
		}
		ExpressiveSortInfo.Cell IDataProviderEvents.GetSortCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType, ColumnSortOrder order) {
			GridDataColumnSortInfo info = SortData.GetSortInfo(dataColumnInfo);
			if(info == null)
				return null;
			return info.GetCompareSortValuesInfo(baseExtractorType, order);
		}
		int? IDataProviderEvents.OnCompareGroupValues(int listSourceRowIndex1, int listSourceRowIndex2, object value1, object value2, DataColumnInfo sortColumn) {
			GridDataColumnSortInfo info = SortData.GetSortInfo(sortColumn);
			if(info == null)
				return null;
			return info.CompareGroupValues(listSourceRowIndex1, listSourceRowIndex2, value1, value2);
		}
		ExpressiveSortInfo.Cell IDataProviderEvents.GetSortGroupCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType) {
			GridDataColumnSortInfo info = SortData.GetSortInfo(dataColumnInfo);
			if(info == null)
				return null;
			return info.GetCompareGroupValuesInfo(baseExtractorType);
		}
		void IDataProviderEvents.OnBeforeSorting() {
			if(!View.AutoScrollOnSorting) {
				View.ScrollIntoViewLocker.Lock();
			}
			SortData.OnStart();
			LockUpdateUntilSortingEndInAsyncServerMode();
			RaiseGridEventInOriginationGrid(StartSortingEvent);
		}
		void IDataProviderEvents.OnAfterSorting() {
			UnlockUpdateAfterSortingEndInAsyncServerMode();
			if(!View.AutoScrollOnSorting) {
				View.ScrollIntoViewLocker.Unlock();
			}
			RaiseGridEventInOriginationGrid(EndSortingEvent);
		}
		void IDataProviderEvents.OnBeforeGrouping() {
			RaiseGridEventInOriginationGrid(StartGroupingEvent);
		}
		void IDataProviderEvents.OnAfterGrouping() {
			RaiseGridEventInOriginationGrid(EndGroupingEvent);
		}
		bool? IDataProviderEvents.OnCustomRowFilter(int listSourceRowIndex, bool fit) {
			var mangled = RaiseCustomRowFilter(listSourceRowIndex, fit);
			if(mangled == -1)
				return null;
			else
				return mangled != 0;
		}
		bool IDataProviderEvents.OnShowingGroupFooter(int rowHandle, int level) {
			TableView view = View as TableView;
			if(view != null)
				return view.RaiseShowingGroupFooter(rowHandle, level);
			return false;
		}
		#endregion
		void LockUpdateUntilSortingEndInAsyncServerMode() {
			if(DataProviderBase.IsAsyncServerMode)
				LockUpdateLayout = true;
		}
		void UnlockUpdateAfterSortingEndInAsyncServerMode() {
			if(DataProviderBase.IsAsyncServerMode)
				LockUpdateLayout = false;
		}
		#region grouping and sorting API
		public void SortBy(GridColumn column) {
			SortByCore(column);
		}
		public void SortBy(GridColumn column, ColumnSortOrder sortedOrder) {
			SortByCore(column, sortedOrder);
		}
		public void SortBy(GridColumn column, ColumnSortOrder sortedOrder, int sortedIndex) {
			SortByCore(column, sortedOrder, sortedIndex);
		}
		public void GroupBy(string fieldName) {
			GroupBy(Columns[fieldName]);
		}
		public void GroupBy(GridColumn column) {
			GroupBy(column, defaultColumnSortOrder);
		}
		public void GroupBy(GridColumn column, int groupedIndex) {
			GroupBy(column, defaultColumnSortOrder, groupedIndex);
		}
		public void GroupBy(GridColumn column, ColumnSortOrder sortedOrder) {
			GroupBy(column, sortedOrder, GroupCount);
		}
		public void GroupBy(GridColumn column, ColumnSortOrder sortedOrder, int groupedIndex) {
			SortInfo.GroupByColumn(column.FieldName, groupedIndex, sortedOrder);
		}
		public void UngroupBy(string fieldName) {
			UngroupBy(Columns[fieldName]);
		}
		public void UngroupBy(GridColumn column) {
			SortInfo.UngroupByColumn(column.FieldName);
		}
		public void ClearGrouping() {
			SortInfo.BeginUpdate();
			try {
				foreach(GridColumn column in Columns) {
					if(column.IsGrouped)
						UngroupBy(column);
				}
			}
			finally {
				SortInfo.EndUpdate();
			}
		}
		protected override void GroupByColumn(ColumnBase column) {
			SortInfo.GroupByColumn(column.FieldName, GroupCount, column.SortOrder);
		}
		#endregion
		#region IDataControllerVisualClient Members
		void DoOnColumnsArePopulated(Action action) {
			if(Columns.Count > 0) {
				action();
				return;
			}
			NotifyCollectionChangedEventHandler handler = null;
			handler = (d, e) => {
				action();
				Columns.CollectionChanged -= handler;
			};
			Columns.CollectionChanged += handler;
		}
#if DEBUGTEST
		public int DataSyncCount { get; private set; }
#endif
		void IDataControllerVisualClient.RequireSynchronization(IDataSync dataSync) {
			DoOnColumnsArePopulated(() => {
#if DEBUGTEST
				DataSyncCount++;
#endif
				List<GridSortInfo> sort = new List<GridSortInfo>();
				bool exist = false;
				InvalidSortCache.Clear();
				foreach(ListSortInfo info in dataSync.Sort) {
					GridColumn col = Columns[info.PropertyName];
					if(col != null)
						sort.Add(new GridSortInfo(col.FieldName, info.SortDirection));
					else
						InvalidSortCache[info.PropertyName] = new GridSortInfo(info.PropertyName, info.SortDirection);
					if(info.PropertyName == DefaultSorting) exist = true;
				}
				UpdateInvalidGroupCache();
				if(!exist && !string.IsNullOrEmpty(DefaultSorting)) {
					GridColumn col = Columns[DefaultSorting];
					ListSortDirection dir = col == null ? ListSortDirection.Ascending :
											col.SortOrder == ColumnSortOrder.Descending ?
											ListSortDirection.Descending : ListSortDirection.Ascending;
					sort.Add(new GridSortInfo(DefaultSorting, dir));
					DataProviderBase.CollectionViewSource.SortDescriptions.Add(new SortDescription(DefaultSorting, dir));
				}
				BeginDataUpdate();
				try {
					SortInfo.ClearAndAddRange(dataSync.GroupCount, sort.ToArray());
					if(!dataSync.HasFilter && !object.ReferenceEquals(FilterCriteria, null))
						FilterCriteria = null;
				}
				finally {
					EndDataUpdate();
				}
			});
		}
		void IDataControllerVisualClient.ColumnsRenewed() {
			PopulateColumnsIfNeeded();
		}
		bool IDataControllerVisualClient.IsInitializing { get { return false; } }
		int IDataControllerVisualClient.PageRowCount { get { return viewCore != null ? viewCore.PageVisibleDataRowCount : DataProviderBase.VisibleCount; } }
		int IDataControllerVisualClient.VisibleRowCount { get { return -1; } }
		int IDataControllerVisualClient.TopRowIndex { get { return viewCore != null ? viewCore.PageVisibleTopRowIndex : 0; } }
		VisualClientUpdater visualClientUpdater;
		internal override bool BottomRowBelowOldVisibleRowCount { get { return ((IDataControllerVisualClient)this).TopRowIndex + ((IDataControllerVisualClient)this).PageRowCount >= View.RootNodeContainer.oldVisibleRowCount; } }
		class VisualClientUpdater {
			enum UpdateMode { None = 0, Scrollbar = 1, Rows = 2 }
			readonly GridControl grid;
			UpdateMode updateMode = UpdateMode.None;
			public VisualClientUpdater(GridControl grid) {
				this.grid = grid;
			}
			public void ScheduleUpdateScrollbar() {
				if(grid.BottomRowBelowOldVisibleRowCount) {
					ScheduleUpdateRows();
					return;
				}
				SetUpdateMode(UpdateMode.Scrollbar);
				EnqueueUpdateAction();
			}
			public void ScheduleUpdateRows() {
				SetUpdateMode(UpdateMode.Rows);
				EnqueueUpdateAction();
			}
			void SetUpdateMode(UpdateMode newUpdateMode) {
				updateMode = (UpdateMode)Math.Max((int)updateMode, (int)newUpdateMode);
				grid.InvalidateDetailScrollInfoCache();
			}
			void EnqueueUpdateAction() {
				if(grid.DataView != null && grid.DataView.UpdateActionEnqueued)
					return;
				if(grid.DataView == null || grid.DataView.RootDataPresenter == null || IsNewItemRowCommiting()) {
					Update();
					return;
				}
				grid.DataView.UpdateActionEnqueued = true;
				grid.viewCore.EnqueueImmediateAction(Update);
			}
			bool IsNewItemRowCommiting() {
				return grid.DataView.CommitEditingLocker.IsLocked && grid.DataView.IsBottomNewItemRowFocused;
			}
			public void Update() {
				grid.DataView.UpdateActionEnqueued = false;
				switch(updateMode) {
					case UpdateMode.None:
					case UpdateMode.Scrollbar:
						break;
					case UpdateMode.Rows:
						grid.UpdateRowsCore(false, false);
						break;
					default:
						break;
				}
				updateMode = UpdateMode.None;
			}
		}
#if DEBUGTEST
		internal int UpdateRowCount;
#endif        
		void IDataControllerVisualClient.UpdateRow(int controllerRowHandle) {
#if DEBUGTEST
			UpdateRowCount++;
#endif
			UpdateRowCore(controllerRowHandle);
		}
		void IDataControllerVisualClient.UpdateRowIndexes(int newTopRowIndex) {
			visualClientUpdater.ScheduleUpdateRows();
		}
		void IDataControllerVisualClient.UpdateRows(int topRowIndexDelta) {
			visualClientUpdater.ScheduleUpdateRows();
		}
		void IDataControllerVisualClient.UpdateScrollBar() {
			visualClientUpdater.ScheduleUpdateScrollbar();
		}
		void IDataControllerVisualClient.UpdateTotalSummary() {
			UpdateTotalSummaryCore();
			if(View != null && View.ViewBehavior.GetServiceSummaries().Any())
				visualClientUpdater.ScheduleUpdateRows();
		}
		void IDataControllerVisualClient.UpdateColumns() {
#if DEBUGTEST
			UpdateColumnsCount++;
#endif
			if(!ColumnsCore.IsLockUpdate)
				DataProviderBase.SynchronizeSummary();
			DataSourceChangingLocker.DoIfNotLocked(ClearAndNotify);
		}
		void IDataControllerVisualClient.RequestSynchronization() {
			RequestSynchronizationCore();
		}
#if DEBUGTEST
		internal int UpdateColumnsCount;
#endif
		void IDataControllerVisualClient.UpdateLayout() {
			UpdateLayoutCore();
			UpdateErrorPanel();		  
		}
		void UpdateErrorPanel() {
			ErrorPanel errorPanel = GetTemplateChild("PART_ErrorPanel") as ErrorPanel;
			if(errorPanel == null || DataController == null) 
				return;
			errorPanel.DataContext = String.Format(GridControlLocalizer.GetString(GridControlStringId.ErrorPanelTextFormatString), DataController.LastErrorText);
			errorPanel.Visibility = (DataController.LastErrorText == "" || !this.IsLoaded) ? Visibility.Collapsed : Visibility.Visible;	
		}
		protected override void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UpdateErrorPanel();
		}
#if SL
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateErrorPanel();
		}
#endif
		#endregion
		#region serialization
		protected override void OnDeserializeCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			switch(e.CollectionName) {
				case GridSerializationPropertiesNames.GroupSummary:
					GridSummaryItem groupSummaryItem = new GridSummaryItem();
					e.CollectionItem = groupSummaryItem;
					GroupSummary.Add(groupSummaryItem);
					return;
				case GridSerializationPropertiesNames.GroupSummarySortInfo:
					GridGroupSummarySortInfo groupSummarySortInfoItem = new GridGroupSummarySortInfoDeserializable();
					e.CollectionItem = groupSummarySortInfoItem;
					GroupSummarySortInfo.Add(groupSummarySortInfoItem);
					return;
			}
			base.OnDeserializeCreateCollectionItem(e);
		}
		protected override void BeginUpdateGroupSummary() {
			GroupSummary.BeginUpdate();
			GroupSummarySortInfo.BeginUpdate();
		}
		protected override void EndUpdateGroupSummary() {
			GroupSummarySortInfo.EndUpdate();
			GroupSummary.EndUpdate();
		}
		protected override void OnDeserializeEndBeforeRemoveSummary(int summaryIndex) {
			if(summaryIndex < GroupCount)
				GroupCount--;
		}
		internal protected override bool GetAddNewColumns() {
			return GridSerializationOptions.GetAddNewColumns(this);
		}
		internal protected override bool GetRemoveOldColumns() {
			return GridSerializationOptions.GetRemoveOldColumns(this);
		}
		protected override string GetSerializationAppName() {
			return typeof(GridControl).Name;
		}
		#endregion
		#region data controller API
		public void ExpandAllGroups() {
			if(!RaiseGroupRowExpanding(GridDataController.InvalidRow)) return;
			DataProviderBase.ExpandAll();
			ExpandCollapseAllDetailGroups(true);
			RaiseGroupRowExpanded(GridDataController.InvalidRow);
		}
		protected void ExpandCollapseAllDetailGroups(bool expanded) {
			if(View != null && View.HasClonedDetails) {
				DataControlOriginationElementHelper.EnumerateDependentElemets<GridControl>(this, grid => grid as GridControl, grid => {
					if(expanded)
						grid.ExpandAllGroups();
					else
						grid.CollapseAllGroups();
				});
			}
		}
		public void CollapseAllGroups() {
			if(!RaiseGroupRowCollapsing(GridDataController.InvalidRow)) return;
			DataProviderBase.CollapseAll();
			ExpandCollapseAllDetailGroups(false);
			RaiseGroupRowCollapsed(GridDataController.InvalidRow);
		}
		public bool ExpandGroupRow(int rowHandle) { return ExpandGroupRow(rowHandle, false); }
		public bool ExpandGroupRow(int rowHandle, bool recursive) {
			if(!IsGroupRowHandle(rowHandle) || DataProviderBase.IsGroupRowExpanded(rowHandle)) return false;
			ExpandGroupRowWithEvents(rowHandle, recursive);
			return true;
		}
		public bool CollapseGroupRow(int rowHandle) { return CollapseGroupRow(rowHandle, false); }
		public bool CollapseGroupRow(int rowHandle, bool recursive) {
			if(!IsGroupRowHandle(rowHandle) || !DataProviderBase.IsGroupRowExpanded(rowHandle)) return false;
			CollapseGroupRowWithEvents(rowHandle, recursive);
			return true;
		}
		public bool IsGroupRowExpanded(int rowHandle) {
			return DataProviderBase.IsGroupRowExpanded(rowHandle);
		}
		public int GetRowHandleByVisibleIndex(int visibleIndex) {
			return GetRowHandleByVisibleIndexCore(visibleIndex);
		}
		public int GetRowVisibleIndexByHandle(int rowHandle) {
			return GetRowVisibleIndexByHandleCore(rowHandle);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Instead use the GetListIndexByRowHandle method. For detailed information, see the list of breaking changes in DXperience v2012 vol 1.")]
		public int GetRowListIndex(int rowHandle) {
			return GetListIndexByRowHandle(rowHandle);
		}
		public int GetListIndexByRowHandle(int rowHandle) {
			return DataProviderBase.GetListIndexByRowHandle(rowHandle);
		}
		public int GetRowHandleByListIndex(int listIndex) {
			return DataProviderBase.GetRowHandleByListIndex(listIndex);
		}
		public object GetRowByListIndex(int listSourceRowIndex) {
			return DataProviderBase.GetRowByListIndex(listSourceRowIndex);
		}
		public object GetCellValueByListIndex(int listSourceRowIndex, GridColumn column) {
			return GetCellValueByListIndex(listSourceRowIndex, column.FieldName);
		}
		public object GetCellValueByListIndex(int listSourceRowIndex, string fieldName) {
			return DataProviderBase.GetCellValueByListIndex(listSourceRowIndex, fieldName);
		}
		public bool IsGroupRowHandle(int rowHandle) {
			return IsGroupRowHandleCore(rowHandle);
		}
		public bool IsValidRowHandle(int rowHandle) {
			return IsValidRowHandleCore(rowHandle);
		}
		public bool IsRowVisible(int rowHandle) {
			return IsRowVisibleCore(rowHandle);
		}
		public object GetFocusedRow() {
			return DataProviderBase.GetRowValue(View != null ? View.FocusedRowHandle : GridControl.InvalidRowHandle);
		}
		public int GetRowLevelByRowHandle(int rowHandle) {
			return DataProviderBase.GetRowLevelByControllerRow(rowHandle);
		}
		public int GetRowLevelByVisibleIndex(int visibleIndex) {
			return DataProviderBase.GetRowLevelByVisibleIndex(visibleIndex);
		}
		public object GetCellValue(int rowHandle, GridColumn column) {
			return GetCellValueCore(rowHandle, column);
		}
		public string GetCellDisplayText(int rowHandle, GridColumn column) {
			return GetCellDisplayTextCore(rowHandle, column);
		}
		public string GetCellDisplayTextByListIndex(int listSourceRowIndex, GridColumn column) {
			return View.GetColumnDisplayText(GetCellValueByListIndex(listSourceRowIndex, column), column);
		}
		public string GetCellDisplayTextByListIndex(int listSourceRowIndex, string fieldName) {
			return GetCellDisplayTextByListIndex(listSourceRowIndex, Columns[fieldName]);
		}
		public string GetFocusedRowCellDisplayText(GridColumn column) {
			return GetFocusedRowCellDisplayText(column.FieldName);
		}
		public string GetFocusedRowCellDisplayText(string fieldName) {
			return GetCellDisplayText(View.FocusedRowHandle, fieldName);
		}
		public object GetFocusedValue() {
			if(View == null || CurrentColumn == null)
				return null;
			return DataProviderBase.GetRowValue(View.FocusedRowHandle, CurrentColumn.FieldName);
		}
		public void SetFocusedRowCellValue(GridColumn column, object value) {
			SetCellValue(View.FocusedRowHandle, column, value);
		}
		public void SetFocusedRowCellValue(string fieldName, object value) {
			SetCellValue(View.FocusedRowHandle, fieldName, value);
		}
		public object GetFocusedRowCellValue(GridColumn column) {
			return GetCellValue(View.FocusedRowHandle, column);
		}
		public object GetFocusedRowCellValue(string fieldName) {
			return GetCellValue(View.FocusedRowHandle, fieldName);
		}
		public void SetCellValue(int rowHandle, GridColumn column, object value) {
			SetCellValue(rowHandle, column.FieldName, value);
		}
		public void SetCellValue(int rowHandle, string fieldName, object value) {
			SetCellValueCore(rowHandle, fieldName, value);
		}
		public object GetGroupRowValue(int rowHandle) {
			return DataProviderBase.GetGroupRowValue(rowHandle);
		}
		public object GetGroupRowValue(int rowHandle, GridColumn column) {
			return DataProviderBase.GetGroupRowValue(rowHandle, column);
		}
		public int GetChildRowCount(int rowHandle) {
			return DataProviderBase.GetChildRowCount(rowHandle);
		}
		public int GetChildRowHandle(int rowHandle, int childIndex) {
			return DataProviderBase.GetChildRowHandle(rowHandle, childIndex);
		}
		public int GetParentRowHandle(int rowHandle) {
			return DataProviderBase.GetParentRowHandle(rowHandle);
		}
		public object GetGroupSummaryValue(int rowHandle, GridSummaryItem item) {
			object value = null;
			DataProviderBase.TryGetGroupSummaryValue(rowHandle, item, out value);
			return value;
		}
		internal override object GetGroupSummaryValue(int rowHandle, int summaryItemIndex) {
			return GetGroupSummaryValue(rowHandle, GroupSummary[summaryItemIndex]);
		}
		public int GetDataRowHandleByGroupRowHandle(int groupRowHandle) {
			return DataProviderBase.GetControllerRowByGroupRow(groupRowHandle);
		}
		public int FindRowByValue(string fieldName, object value) {
			return DataProviderBase.FindRowByValue(fieldName, value); 
		}
		public int FindRowByValue(ColumnBase column, object value) {
			return FindRowByValue(column.FieldName, value);
		}
		#endregion
		#region Async API
		public Task<object> GetRowAsync(int rowHandle) {
			return new LoadRowAsyncOperation<object>(this, rowHandle,
				() => DataProviderBase.DataController.GetRow(rowHandle)
			).GetTask();
		}
		public Task<object> GetRowValueAsync(int rowHandle, string columnName) {
			return new LoadRowAsyncOperation<object>(this, rowHandle, () =>
				DataProviderBase.DataController.GetRowValue(rowHandle, columnName)
			).GetTask();
		}
		public Task<IList> GetRowsAsync(int startFrom, int count) {
			return new GetRowsAsyncOperation(this, startFrom, count).GetTask();
		}
		public Task<int> FindRowByValueAsync(string fieldName, object value) {
			return new FindRowByValueAsyncOperation(this, fieldName, value).GetTask();
		}
		public Task<int> FindRowByValueAsync(ColumnBase column, object value) {
			return FindRowByValueAsync(column.FieldName, value);
		}
		#endregion
		#region MasterDetail
		public bool IsMasterRowExpanded(int rowHandle, DetailDescriptorBase descriptor = null) {
			return MasterDetailProvider.IsMasterRowExpanded(rowHandle, descriptor);
		}
		public void SetMasterRowExpanded(int rowHandle, bool expand, DetailDescriptorBase descriptor = null) {
			MasterDetailProvider.SetMasterRowExpanded(rowHandle, expand, descriptor);
		}
		public void ExpandMasterRow(int rowHandle, DetailDescriptorBase descriptor = null) {
			SetMasterRowExpanded(rowHandle, true, descriptor);
		}
		public void CollapseMasterRow(int rowHandle, DetailDescriptorBase descriptor = null) {
			SetMasterRowExpanded(rowHandle, false, descriptor);
		}
		public DataControlBase GetVisibleDetail(int rowHandle) {
			return MasterDetailProvider.FindVisibleDetailDataControl(rowHandle);
		}
		public DetailDescriptorBase GetVisibleDetailDescriptor(int rowHandle) {
			return MasterDetailProvider.FindVisibleDetailDescriptor(rowHandle);
		}
		public DataControlBase GetDetail(int rowHandle, DataControlDetailDescriptor descriptor = null) {
			return MasterDetailProvider.FindDetailDataControl(rowHandle, descriptor);
		}
		public GridControl GetMasterGrid() {
			return GetMasterGridCore() as GridControl;
		}
		public int GetMasterRowHandle() {
			DataViewBase masterView = null;
			int masterVisibleIndex = -1;
			bool found = DataControlParent.FindMasterRow(out masterView, out masterVisibleIndex);
			return found ? masterView.DataControl.GetRowHandleByVisibleIndexCore(masterVisibleIndex) : InvalidRowHandle;
		}
		GridControl GetOriginationGridControl() {
			return (GridControl)GetOriginationDataControl();
		}
		protected internal override bool RaiseMasterRowExpandStateChanging(int rowHandle, bool isExpanded) {
			if(isExpanded)
				return RaiseMasterRowCollapsing(rowHandle);
			else
				return RaiseMasterRowExpanding(rowHandle);
		}
		protected internal override void RaiseMasterRowExpandStateChanged(int rowHandle, bool isExpanded) {
			if(isExpanded)
				RaiseMasterRowExpanded(rowHandle);
			else
				RaiseMasterRowCollapsed(rowHandle);
		}
		internal override void ThrowNotSupportedInDetailException() { }
		protected override void CloneGroupSummarySortInfo(DataControlBase dataControl) {
			GridControl gridControl = ((GridControl)dataControl);
			if(GroupSummarySortInfo.Count == 0 && gridControl.GroupSummarySortInfo.Count == 0)
				return;
			gridControl.GroupSummarySortInfo.Clear();
			CloneDetailHelper.CloneSimpleCollection<GridGroupSummarySortInfo>(GroupSummarySortInfo, gridControl.GroupSummarySortInfo, new object[] { gridControl });
		}
		#region IDetailElement<DataControlBase> Members
		DataControlBase IDetailElement<DataControlBase>.CreateNewInstance(params object[] args) {
			return (DataControlBase)Activator.CreateInstance(GetType(), new object[] { (IDataControlOriginationElement)args[0] });
		}
		#endregion
		#endregion
		protected override void UpdateHasDetailViews() {
			base.UpdateHasDetailViews();
			TableView view = View as TableView;
			if(view != null)
				view.UpdateHasDetailViews();
		}
		protected internal override BandsLayoutBase BandsLayoutCore { get { return BandsLayout; } set { BandsLayout = (GridControlBandsLayout)value; } }
		#endregion
		protected override void RequestSynchronizationCore() {
			if(GridDataProvider is GridDataProvider)
				((GridDataProvider)GridDataProvider).DisplayMemberBindingInitialize();
			base.RequestSynchronizationCore();
			if(View is GridViewBase)
				((GridViewBase)View).RefreshImmediateUpdateRowPositionProperty();
		}
		internal override void UpdateAllowPartialGrouping() {
			bool allowPartialGrouping = !DataProviderBase.IsAsyncServerMode && View.AllowPartialGroupingCore;
			if(DataController == null || DataController.AllowPartialGrouping == allowPartialGrouping) return;
			DataController.AllowPartialGrouping = allowPartialGrouping;
			RefreshData();
		}
		internal void DisableAllowPartialGrouping() {
			if(DataController == null) return;
			DataController.AllowPartialGrouping = false;
			RefreshData();
		}
		protected override IBandsCollection CreateBands() {
			return new BandCollection<GridControlBand>();
		}
		protected override BandsLayoutBase CreateBandsLayout() {
			return new GridControlBandsLayout();
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridControlBands"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(Categories.Data), XtraSerializableProperty(true, true, true), GridStoreAlwaysProperty]
		public ObservableCollectionCore<GridControlBand> Bands { get { return (ObservableCollectionCore<GridControlBand>)BandsCore; } }
		internal override void AttachToFormatConditions(FormatConditionChangeType changeType) {
			base.AttachToFormatConditions(changeType);
			visualClientUpdater.ScheduleUpdateRows();
		}
		protected override object GetItemsSource() {
			if(DesignerProperties.GetIsInDesignMode(this))
				return GridControlHelper.GetDesignTimeSource(this);
			return base.GetItemsSource();
		}
		internal override void SetCellValueCore(int rowHandle, string fieldName, object value) {
			if(TableView.IsCheckBoxSelectorColumn(fieldName))
				SetCheckBoxSelectorColumnValue(rowHandle, value);
			else
				base.SetCellValueCore(rowHandle, fieldName, value);
		}
		void SetCheckBoxSelectorColumnValue(int rowHandle, object value) {
			if((bool)value)
				DataView.SelectRowCore(rowHandle);
			else
				DataView.UnselectRowCore(rowHandle);
		}
		internal override object GetCellValueCore(int rowHandle, string fieldName) {
			if(TableView.IsCheckBoxSelectorColumn(fieldName))
				return GetCheckBoxSelectorColumnValue(rowHandle);
			return base.GetCellValueCore(rowHandle, fieldName);
		}
		bool GetCheckBoxSelectorColumnValue(int rowHandle) {
			return DataView.Return(dataView => dataView.IsRowSelected(rowHandle), () => false);
		}
	}
}
