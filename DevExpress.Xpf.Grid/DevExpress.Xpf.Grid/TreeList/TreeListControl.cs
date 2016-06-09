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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using DevExpress.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Data;
using System.Windows.Markup;
using System.Collections;
using System.Diagnostics;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Automation.Provider;
using System.Windows.Automation;
using DevExpress.Xpf.Grid.Helpers;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Utils.Native;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Serialization;
using System.Collections.ObjectModel;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Xpf.Editors.Helpers;
using System.IO;
using DevExpress.Core;
using DevExpress.Utils;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.XtraGrid;
using System.Collections.Specialized;
using DevExpress.Mvvm.Native;
#if !SL
using DevExpress.Xpf.Grid.Automation;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Data.Native;
using System.Windows.Data;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using WarningException = System.Exception;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using Visual = System.Windows.UIElement;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using FrameworkContentElement = System.Windows.DependencyObject;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using RoutedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
using System.Windows.Data;
using DevExpress.Xpf.Grid.Automation;
#endif
namespace DevExpress.Xpf.Grid {
	[DXToolboxBrowsable]
#if !SL
	[DevExpress.Utils.Design.DataAccess.DataAccessMetadata("All", SupportedProcessingModes = "Simple")]
#endif
	public class TreeListControl : GridDataControlBase {
		#region static
		public static readonly DependencyProperty ViewProperty;
		static readonly DependencyPropertyKey BandsLayoutPropertyKey;
		public static readonly DependencyProperty BandsLayoutProperty;
		public static readonly RoutedEvent CopyingToClipboardEvent;
		public static readonly RoutedEvent SelectionChangedEvent;
		static TreeListControl() {
#if !SL
#endif
			Type ownerType = typeof(TreeListControl);
			ViewProperty = DependencyPropertyManager.Register("View", typeof(TreeListView), ownerType, new FrameworkPropertyMetadata(null, OnViewChanged, OnCoerceView));
			BandsLayoutPropertyKey = DependencyPropertyManager.RegisterReadOnly("BandsLayout", typeof(TreeListControlBandsLayout), ownerType, new PropertyMetadata(null, (d, e) => ((TreeListControl)d).OnBandsLayoutChanged(e.OldValue as BandsLayoutBase, e.NewValue as BandsLayoutBase)));
			BandsLayoutProperty = BandsLayoutPropertyKey.DependencyProperty;
			CopyingToClipboardEvent = EventManager.RegisterRoutedEvent("CopyingToClipboard", RoutingStrategy.Direct, typeof(TreeListCopyingToClipboardEventHandler), ownerType);
			SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Direct, typeof(TreeListSelectionChangedEventHandler), ownerType);
#if !SL
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.SelectAll, (d, e) => ((TreeListControl)d).SelectAllMasterDetail(), (d, e) => ((TreeListControl)d).CanSelectAll(d, e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Copy, (d, e) => ((TreeListControl)d).Copy(), (d, e) => ((TreeListControl)d).CanCopy(d, e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Paste, (d, e) => ((TreeListControl)d).Paste(), (d, e) => ((TreeListControl)d).CanPaste(d, e)));
#endif
		}
		internal static TreeListView CreateDefaultTreeListView() {
			return new TreeListView();
		}
		#endregion
		#region public properties
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListControlView"),
#endif
 Category(Categories.View), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public TreeListView View {
			get { return (TreeListView)base.GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public TreeListControlBandsLayout BandsLayout {
			get { return (TreeListControlBandsLayout)GetValue(BandsLayoutProperty); }
			internal set { this.SetValue(BandsLayoutPropertyKey, value); }
		}
		[Category(Categories.Data), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(true, false, false), GridUIProperty, XtraResetProperty]
		public TreeListSortInfoCollection SortInfo { get { return (TreeListSortInfoCollection)SortInfoCore; } }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListControlTotalSummary"),
#endif
 Category(Categories.Data), XtraSerializableProperty(true, false, false), GridUIProperty, XtraResetProperty]
		public TreeListSummaryItemCollection TotalSummary { get { return (TreeListSummaryItemCollection)TotalSummaryCore; } }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListControlColumns"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(Categories.Data), XtraSerializableProperty(true, true, true), GridStoreAlwaysProperty]
		public TreeListColumnCollection Columns { get { return (TreeListColumnCollection)ColumnsCore; } }
		#endregion
		#region events
		[Category(Categories.OptionsCopy)]
		public event TreeListCopyingToClipboardEventHandler CopyingToClipboard {
			add { AddHandler(CopyingToClipboardEvent, value); }
			remove { RemoveHandler(CopyingToClipboardEvent, value); }
		}
		[Category(Categories.OptionsSelection)]
		public event TreeListSelectionChangedEventHandler SelectionChanged {
			add { AddHandler(SelectionChangedEvent, value); }
			remove { RemoveHandler(SelectionChangedEvent, value); }
		}
		#endregion
		protected internal override IDesignTimeAdornerBase EmptyDesignTimeAdorner { get { return EmptyDesignTimeAdornerBase.Instance; } }
		internal override DataViewBase DataView { get { return viewCore; } set { View = (TreeListView)value; } }
		internal override DetailDescriptorBase DetailDescriptorCore { get { return null; } }
		public TreeListControl() : this(null) { }
		internal TreeListControl(IDataControlOriginationElement dataControlOriginationElement) : base(dataControlOriginationElement) {
			this.SetDefaultStyleKey(typeof(TreeListControl));
			DXSerializer.SetSerializationIDDefault(this, GetType().Name);
		}
		internal override void ValidateDataProvider(DataViewBase newValue) {
			if(View != null)
				fDataProvider = View.DataProviderBase;
		}
		#region factory methods
		internal override IColumnCollection CreateColumns() {
			return new TreeListColumnCollection(this);
		}
		internal protected override Type ColumnType { get { return typeof(TreeListColumn); } }
		internal protected override Type BandType { get { return typeof(TreeListControlBand); } }
		internal protected override BandBase CreateBand() {
			return new TreeListControlBand();
		}
		internal override SortInfoCollectionBase CreateSortInfo() {
			return new TreeListSortInfoCollection();
		}
		internal override ISummaryItemOwner CreateSummariesCollection(SummaryItemCollectionType collectionType) {
			return new TreeListSummaryItemCollection(this);
		}
		protected internal override DataViewBase CreateDefaultView() {
			return CreateDefaultTreeListView();
		}
		protected override DataProviderBase CreateDataProvider() {
			return new EmptyDataProvider();
		}
		internal override SummaryItemBase CreateSummaryItem() {
			return new TreeListSummaryItem();
		}
		internal override ModelGridColumnsGeneratorBase CreateSmartModelGridColumnsGenerator(ColumnCreatorBase creator, bool applyOnlyForSmartColumns, bool skipXamlGenerationProperties) {
			return new SmartModelGridColumnsGenerator(creator, applyOnlyForSmartColumns, skipXamlGenerationProperties);
		}
		#endregion
		#region data controller API
		public object GetCellValue(int rowHandle, TreeListColumn column) {
			return GetCellValueCore(rowHandle, column);
		}
		public string GetCellDisplayText(int rowHandle, TreeListColumn column) {
			return GetCellDisplayTextCore(rowHandle, column);
		}
		#endregion
		#region grouping stubs
		protected internal override IList<DevExpress.Xpf.Grid.SummaryItemBase> GetGroupSummaries() {
			throw new NotImplementedException();
		}
		internal override object GetGroupSummaryValue(int rowHandle, int summaryItemIndex) {
			throw new NotImplementedException();
		}
		#endregion
		#region multiselect
		public void SelectRange(TreeListNode startNode, TreeListNode endNode) { 
			View.Do(view => view.SelectRangeCore(startNode, endNode));
		}
		public void SelectItem(TreeListNode node) {
			View.Do(view => view.SelectNodeCore(node));
		}
		public void UnselectItem(TreeListNode node) {
			View.Do(view => view.UnselectNodeCore(node));
		}
		public TreeListNode[] GetSelectedNodes() {
			int[] selectedRows = GetSelectedRowHandles();
			TreeListNode[] selectedNodes = new TreeListNode[selectedRows.Length];
			if(View == null)
				return selectedNodes;
			for(int i = 0; i < selectedRows.Length; i++)
				selectedNodes[i] = View.GetNodeByRowHandle(selectedRows[i]);
			return selectedNodes;
		}
		protected void CanSelectAll(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = View.IsMultiSelection;
		}
		#endregion
		#region clipboard
		public void CopyRowsToClipboard(IEnumerable<TreeListNode> nodes) {
			View.Do(view => view.CopyRowsToClipboardCore(nodes));
		}
		public void CopyRangeToClipboard(TreeListNode startNode, TreeListNode endNode) {
			View.Do(view => view.CopyRangeToClipboardCore(startNode, endNode));
		}
		protected internal override bool RaiseCopyingToClipboard(CopyingToClipboardEventArgsBase e) {
			e.RoutedEvent = TreeListControl.CopyingToClipboardEvent;
			RaiseEvent(e);
			return e.Handled;
		}
#if !SL
		protected void Copy() {
			CopyToClipboard();
		}
		protected void Paste() {
			if(!RaisePastingFromClipboard())
				View.RaisePastingFromClipboard();
		}
		protected void CanCopy(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = View.CanCopyRows();
		}
		protected void CanPaste(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}
#endif
		#endregion
		#region serialization
		internal protected override bool GetAddNewColumns() {
			return TreeListSerializationOptions.GetAddNewColumns(this);
		}
		internal protected override bool GetRemoveOldColumns() {
			return TreeListSerializationOptions.GetRemoveOldColumns(this);
		}
		protected override string GetSerializationAppName() {
			return typeof(TreeListControl).Name;
		}
		#endregion
		#region automation
		protected override PeerCacheBase CreatePeerCache() {
			return new PeerCache();
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			if(AutomationPeer == null)
				AutomationPeer = new TreeListControlAutomationPeer(this);
			return AutomationPeer;
		}
		#endregion
		protected internal override void RaiseSelectionChanged(GridSelectionChangedEventArgs e) {
			e.RoutedEvent = TreeListControl.SelectionChangedEvent;
			RaiseEvent(e);
		}
		protected internal override BandsLayoutBase BandsLayoutCore { get { return BandsLayout; } set { BandsLayout = (TreeListControlBandsLayout)value; } }
		protected override IBandsCollection CreateBands() {
			return new BandCollection<TreeListControlBand>();
		}
		protected override BandsLayoutBase CreateBandsLayout() {
			return new TreeListControlBandsLayout();
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListControlBands"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(Categories.Data), XtraSerializableProperty(true, true, true), GridStoreAlwaysProperty]
		public ObservableCollectionCore<TreeListControlBand> Bands { get { return (ObservableCollectionCore<TreeListControlBand>)BandsCore; } }
		protected override object GetItemsSource() {
			if(DesignerProperties.GetIsInDesignMode(this))
				return GridControlHelper.GetDesignTimeSource(this);
			return base.GetItemsSource();
		}
	}
	#region tree list control classes
	public class TreeListSummaryItemCollection : SummaryItemCollectionBase<TreeListSummaryItem> {
		public TreeListSummaryItemCollection(DataControlBase dataControl) : base(dataControl, SummaryItemCollectionType.Total) { }
	}
	public class TreeListSummaryItem : SummaryItemBase {
		public TreeListSummaryItem() { }
	}
	public class TreeListSortInfoCollection : SortInfoCollectionBase {
		public TreeListSortInfoCollection() { }
	}
	public class TreeListColumnCollection : ColumnCollectionBase<TreeListColumn> {
		public TreeListColumnCollection(TreeListControl treeList)
			: base(treeList) {
		}
	}
	public class TreeListColumn : GridColumnBase {
		public TreeListColumn() { }
		protected internal override void OnValidation(GridRowValidationEventArgs e) {
			((TreeListColumn)GetEventTargetColumn()).OnValidationCore(e);
		}
		void OnValidationCore(GridRowValidationEventArgs e) {
			if (Validate != null)
				Validate(this, (TreeListCellValidationEventArgs)e);
		}
		public event TreeListCellValidationEventHandler Validate;
	}
	#endregion
}
