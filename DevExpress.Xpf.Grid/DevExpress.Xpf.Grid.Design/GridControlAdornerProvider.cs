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

extern alias Platform;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Linq;
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Windows.Design.Policies;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Collections.Specialized;
using Microsoft.Windows.Design.Services;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows.Interop;
using System.Reflection;
using DevExpress.Xpf.Core.Design;
using DevExpress.Design.UI;
#if SILVERLIGHT
using DependencyObject = Platform::System.Windows.DependencyObject;
using DependencyProperty = Platform::System.Windows.DependencyProperty;
using PropertyMetadata = Platform::System.Windows.PropertyMetadata;
using Point = Platform::System.Windows.Point;
using RoutedEventHandler = Platform::System.Windows.RoutedEventHandler;
using RoutedEventArgs = Platform::System.Windows.RoutedEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using FrameworkElement = Platform::System.Windows.FrameworkElement;
using ToggleStateButton = DevExpress.Xpf.Core.Design.CoreUtils.ToggleStateButton;
using UIElement = Platform::System.Windows.FrameworkElement;
using LayoutHelper = Platform::DevExpress.Xpf.Core.Native.LayoutHelper;
using HitTestResult =  Platform::DevExpress.Xpf.Core.HitTestResult;
using HitTestResultBehavior = Platform::DevExpress.Xpf.Core.HitTestResultBehavior;
using HitTestFilterCallback = Platform::DevExpress.Xpf.Core.HitTestFilterCallback;
using HitTestResultCallback = Platform::DevExpress.Xpf.Core.HitTestResultCallback;
using HitTestFilterBehavior = Platform::DevExpress.Xpf.Core.HitTestFilterBehavior;
using PointHitTestParameters = Platform::DevExpress.Xpf.Core.PointHitTestParameters;
using DesignTimeSelectionControl = Platform::DevExpress.Xpf.Grid.DesignTimeSelectionControl;
using DesignerSerializationVisibility = Platform::System.ComponentModel.DesignerSerializationVisibility;
using DesignerSerializationVisibilityAttribute = Platform::System.ComponentModel.DesignerSerializationVisibilityAttribute;
using PropertyDescriptor = Platform::DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Xpf.Core.Design.CoreUtils;
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::DevExpress.Xpf.Grid;
using Platform::DevExpress.Xpf.Grid.Native;
using Platform::DevExpress.Data;
using Platform::DevExpress.Xpf.Bars.Helpers;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Xpf.Core.Native;
using Platform::DevExpress.Utils;
using Platform::DevExpress.Xpf.Core.Commands;
using Platform::DevExpress.Xpf.Core.WPFCompatibility;
using Platform::DevExpress.Xpf.Data.Native;
#else
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Data;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Commands;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Data.Native;
#endif
using System.Collections;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator;
using Platform::DevExpress.Entity.Model;
using DevExpress.Xpf.Core.Design.Editors;
namespace DevExpress.Xpf.Grid.Design {
	[FeatureConnector(typeof(DataControlAdornerFeatureConnector))]
	[UsesItemPolicy(typeof(DataControlPolicy))]
	public abstract partial class DataControlAdornerProvider : DXAdornerProviderBase, IDesignTimeAdornerBase {
		#region inner classes
		internal class DesignTimeColumnsPopulator : DefaultColumnsPopulator {
			SelectedCommandType generationType;
			public DesignTimeColumnsPopulator(IModelItem dataControl, IModelItemCollection columns, SelectedCommandType generationType, AllColumnsInfo columnsInfo)
				: base(dataControl, columns, columnsInfo) {
				this.generationType = generationType;
			}
			protected override void OnAddColumn(IModelItem column) {
				base.OnAddColumn(column);
#if SL
				((DataControlBase)DataControl.GetCurrentValue()).UpdateLayout();
#endif
			}
			protected override IModelItem CreateColumnFromColumnsGenerator(IEdmPropertyInfo propertyInfo) {
				return null;
			}
			public override IModelItem CreateColumn(IEdmPropertyInfo propertyInfo) {
				IModelItem column = base.CreateColumn(propertyInfo);
				if(generationType == SelectedCommandType.Smart)
					column.Properties["IsSmart"].SetValue(true);
				return column;
			}
		}
		#endregion
		[Browsable(false)]
		public static bool GetIsAdornerPanelExpanded(DependencyObject obj) {
			return (bool)obj.GetValue(IsAdornerPanelExpandedProperty);
		}
		public static void SetIsAdornerPanelExpanded(DependencyObject obj, bool value) {
			obj.SetValue(IsAdornerPanelExpandedProperty, value);
		}
		public static readonly DependencyProperty IsAdornerPanelExpandedProperty =
			DependencyProperty.RegisterAttached("IsAdornerPanelExpanded", typeof(bool), typeof(DataControlAdornerProvider), new PropertyMetadata(true));
		static string GetElementInfo(FrameworkElement element) {
			return GetElementInfo(element, element.Name);
		}
		internal static bool CanSelectColumn {
			get {
#if !SILVERLIGHT
				return !BlendHelper.IsInBlend;
#else
				return true;
#endif
			}
		}
#if !SILVERLIGHT
		static string GetElementInfo(FrameworkContentElement element) {
			return GetElementInfo(element, element.Name);
		}
#endif
		static string GetElementInfo(object element, string name) {
			string info = string.Format("({0})", element.GetType().Name);
			if(!string.IsNullOrEmpty(name)) {
				info += " " + name;
			}
			return info;
		}
		static void UpdateChangeInfo(List<ChangePropertyTypeInfo> info, bool isPropertySet, object originalValue) {
			if(originalValue == null)
				return;
			info.ForEach(changePropertyTypeInfo => changePropertyTypeInfo.IsEnabled = !isPropertySet || changePropertyTypeInfo.Type != originalValue.GetType());
		}
		List<GridColumnHeader> highlightedColumnHeaders;
		BandHeaderControl highlightedBandHeader;
		DataControlBase dataControl;
		internal DataControlBase DataControl {
			get {
				if(this.dataControl == null)
					this.dataControl = this.platformObject as DataControlBase ?? (AdornedElement != null ? AdornedElement.GetCurrentValue() as DataControlBase : null);
				return this.dataControl;
			}
		}
		GridControlAdornerPanel GridControlAdornerPanel { get { return (GridControlAdornerPanel)hookPanel; } }
		DesignTimeModel designTimeModel;
		protected IColumnCollection Columns { get { return GridControlHelper.GetColumns(DataControl); } }
		protected internal DataViewBase View { get { return GridControlHelper.GetView(DataControl); } }
		protected internal abstract Type ColumnType { get; }
		protected internal abstract Type ControlType { get; }
		Type[] availableViewTypes;
		protected internal Type[] AvailableViewTypes { get { return availableViewTypes; } }
		protected abstract Type[] GetAvailableViewTypes();
		public DataControlAdornerProvider() {
			RefreshDataControl();
			GridControlAdornerPanel.MouseLeftButtonDown += new MouseButtonEventHandler(GridControlAdornerPanel_MouseLeftButtonDown);
			GridControlAdornerPanel.MouseRightButtonDown += new MouseButtonEventHandler(GridControlAdornerPanel_MouseRightButtonDown);
			GridControlAdornerPanel.MouseMove += new MouseEventHandler(GridControlAdornerPanel_MouseMove);
			GridControlAdornerPanel.MouseLeftButtonUp += new MouseButtonEventHandler(GridControlAdornerPanel_MouseLeftButtonUp);
		}
		internal void ChangeViewType(Type type) {
			if(designTimeModel.IsViewSelected) {
				SelectGrid(); 
			}
			ChangeProperty(SR.ChangeViewDescription, AdornedElement, GridControl.ViewProperty, type);
		}
		internal void ChangeEditSettings(Type type) {
			ChangeProperty(SR.ChangeEditSettingsDescription, GetColumnSelectedObject(GridDesignTimeHelper.GetPrimarySelection(AdornedElement.Context)), ColumnBase.EditSettingsProperty, type);
		}
		void ChangeProperty(string description, ModelItem item, DependencyProperty property, Type type) {
			PerformEditAction(description, delegate {
				ModelProperty oldProperty = item.Properties[DesignHelper.GetPropertyName(property)];
				ModelItem newItem = GridDesignTimeHelper.CreateModelItem(item.Context, type, CreateOptions.None);
				if(oldProperty.IsSet) {
					ModelItem oldItem = oldProperty.Value;
					foreach(ModelProperty p in oldItem.Properties) {
						if(p.IsSet && !p.IsAttached && !p.IsCollection && newItem.Properties.Find(p.Name) != null) {
							try { newItem.Properties[p.Name].SetValue(p.Value); } 
							catch { }
						}
					}
				}
				item.Properties[DesignHelper.GetPropertyName(property)].SetValue(newItem);
			});
		}
		internal void SelectView() {
			if(designTimeModel.IsViewSet) {
				SelectElement(GetViewModelItem());
			}
		}
		internal void SelectGrid() {
			SelectElement(AdornedElement);
		}
		internal void SelectColumn() {
			ModelItem primarySelection = GridDesignTimeHelper.GetPrimarySelection(AdornedElement.Context);
			if(IsEditSettingsSelected(primarySelection))
				SelectElement(primarySelection.Parent);
		}
		internal void SelectBand() {
		}
		internal void SelectEditSettings() {
			if(designTimeModel.IsEditSettingsSet) {
				ModelItem primarySelection = GridDesignTimeHelper.GetPrimarySelection(AdornedElement.Context);
				if(IsColumnSelected(primarySelection))
					SelectElement(primarySelection.Properties[DesignHelper.GetPropertyName(ColumnBase.EditSettingsProperty)].Value);
			}
		}
		internal void DeleteSelectedColumn() {
			var cache = GridDesignTimeHelper.GetGroupSortColumns(DataControl);
			PerformEditAction(SR.DeleteColumnDescription, () => DeleteColumn(GridDesignTimeHelper.GetPrimarySelection(AdornedElement.Context)));
			GridControlHelper.FillBandsColumns(DataControl);
			GridDesignTimeHelper.ApplyGroupSortColumns(DataControl, cache);
		}
		internal void DeleteSelectedBand() {
			var cache = GridDesignTimeHelper.GetGroupSortColumns(DataControl);
			PerformEditAction(SR.DeleteBandDescription, () => GridDesignTimeHelper.DeleteBandAndMoveColumnsToRoot(GridDesignTimeHelper.GetPrimarySelection(AdornedElement.Context)));
			GridControlHelper.FillBandsColumns(DataControl);
			GridDesignTimeHelper.ApplyGroupSortColumns(DataControl, cache);
		}
		void DeleteColumn(ModelItem modelItem) {
			object parent = modelItem.Parent.GetCurrentValue();
			if(parent is BandBase) {
				modelItem.Parent.Properties["Columns"].Collection.Remove(modelItem);
				return;
			}
			GetGridColumns().Remove(GridDesignTimeHelper.GetPrimarySelection(AdornedElement.Context));
		}
		internal void AddColumn() {
			ModelItem columnItem = null;
			PerformEditAction(SR.AddColumnDescription, delegate {
				if(!AllColumnsSetInXaml()) {
					AdornedElement.Properties[DesignHelper.GetPropertyName(GridControl.AutoPopulateColumnsProperty)].ClearValue();
					Columns.Clear();
				}
				columnItem = GridDesignTimeHelper.CreateModelItem(AdornedElement.Context, ColumnType);
				GetGridColumns().Add(columnItem);
			});
			SelectElement(columnItem);
		}
		bool AllColumnsSetInXaml() {
			foreach(ColumnBase column in Columns) {
				if(GetColumnBaseModelItem(column) == null)
					return false;
			}
			return true;
		}
		void SelectElement(ModelItem item) {
			SelectionOperations.Select(AdornedElement.Context, item);
		}
		ModelItem GetViewModelItem() {
			ModelProperty viewModelProperty = GetViewModelProperty();
			return viewModelProperty != null ? viewModelProperty.Value : null;
		}
		ModelProperty GetViewModelProperty() {
			return AdornedElement.Properties.FirstOrDefault(mp => mp.Name == DesignHelper.GetPropertyName(GridControl.ViewProperty));
		}
		internal ModelItem GetColumnBaseModelItem(BaseColumn column) {
			if(column is ColumnBase)
				return GetColumnModelItem((ColumnBase)column);
			return GridDesignTimeHelper.GetBandModelItem(AdornedElement, (BandBase)column);
		}
		internal ModelItem GetColumnModelItem(ColumnBase column) {
			return GetColumnFromBands(column) ?? GetColumnModelItemCore(column);
		}
		internal ModelItem GetColumnFromBands(ColumnBase column) {
			if(column == null || AdornedElement == null)
				return null;
			ModelProperty bands = AdornedElement.Properties["Bands"];
			return GetColumnFromBandsCore(bands, column);
		}
		ModelItem GetColumnFromBandsCore(ModelProperty rootBand, ColumnBase column) {
			if(rootBand == null || column == null)
				return null;
			foreach(var b in rootBand.Collection) {
				ModelItemCollection columns = b.Properties["Columns"].Collection;
				foreach(var c in columns) {
					if(c.GetCurrentValue() == column)
						return c;
				}
				ModelItem result = GetColumnFromBandsCore(b.Properties["Bands"], column);
				if(result != null)
					return result;
			}
			return null;
		}
		ModelItem GetColumnModelItemCore(ColumnBase column) {
			int index = Columns.IndexOf(column);
			ModelItemCollection columnItems = GetGridColumns();
			return 0 <= index && index < columnItems.Count ? columnItems[index] : null;
		}
		ModelItemCollection GetGridColumns() {
			return GridDesignTimeHelper.GetGridColumnsCollection(AdornedElement);
		}
		void GridControlAdornerPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			BaseColumn hitColumn = GetHitColumn(GetGridMousePosition(e));
			bool reRaiseEvent = true;
			if(hitColumn != null) {
				ModelItem columnItem = GetColumnBaseModelItem(hitColumn);
				if(columnItem != null && GridDesignTimeHelper.GetPrimarySelection(AdornedElement.Context) != columnItem) {
					SelectionOperations.Select(AdornedElement.Context, columnItem);
					reRaiseEvent = false;
				}
			} else {
				SelectionOperations.Select(AdornedElement.Context, AdornedElement);
			}
			if(reRaiseEvent) {
#if !SILVERLIGHT
				ReraiseEventHelper.ReraiseEvent<MouseButtonEventArgs>(e, GetHitTestElement(e), FrameworkElement.PreviewMouseDownEvent, FrameworkElement.MouseDownEvent, ReraiseEventHelper.CloneMouseButtonEventArgs);
#else
#endif
			}
			e.Handled = true;
		}
		void ScrollToHeader(BaseColumn column) {
#if !SL
			if(View is CardView)
				return;
#endif
			GridColumn gridColumn = column as GridColumn;
			GridViewBase gridView = View as GridViewBase;
			if(gridColumn != null && gridView != null && column.Visible && gridColumn.IsGrouped && !gridView.ShowGroupedColumns)
				return;
			((ITableView)View).TableViewBehavior.MakeColumnVisible(column);
			DataPresenterBase dataPresenter = GridControlHelper.GetDataPresenter(View);
			if(dataPresenter == null)
				return;
			if(column.ActualHeaderWidth > dataPresenter.ScrollInfoCore.HorizontalScrollInfo.Viewport) {
				double offset = dataPresenter.ScrollInfoCore.HorizontalScrollInfo.Offset;
				dataPresenter.ScrollInfoCore.SetHorizontalOffsetForce(offset + column.ActualHeaderWidth - dataPresenter.ScrollInfoCore.HorizontalScrollInfo.Viewport);
			}
		}
		void GridControlAdornerPanel_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			BaseColumn hitColumn = GetHitColumn(GetGridMousePosition(e));
			if(hitColumn != null) {
				ModelItem columnItem = GetColumnBaseModelItem(hitColumn);
				if(columnItem != null && GridDesignTimeHelper.GetPrimarySelection(AdornedElement.Context) != columnItem) {
					SelectionOperations.Select(AdornedElement.Context, columnItem);
				}
			}
			else {
				SelectionOperations.Select(AdornedElement.Context, AdornedElement);
			}
			e.Handled = true;
		}
		BaseColumn GetHitColumn(Point point) {
			UIElement hitTestElement = null;
			IDataViewHitInfo hitInfo = CalcHitInfo(point, out hitTestElement);
			if(hitInfo != null && (hitInfo.InColumnHeader || hitInfo.InGroupPanel) && hitInfo.Column != null && CanSelectColumn) {
				return hitInfo.Column;
			}
			if(hitTestElement != null) {
				BandHeaderControl control = LayoutHelper.FindParentObject<BandHeaderControl>(hitTestElement);
				if(control != null)
					return control.DataContext as BandBase;
			}
			return null;
		}
		void GridControlAdornerPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
#if !SILVERLIGHT
			ReraiseEventHelper.ReraiseEvent<MouseButtonEventArgs>(e, GetHitTestElement(e), FrameworkElement.PreviewMouseUpEvent, FrameworkElement.MouseUpEvent, ReraiseEventHelper.CloneMouseButtonEventArgs);
#else
#endif
			e.Handled = true;
		}
		Point GetSilverlightHitTestPoint(Point point) {
#if !SILVERLIGHT
			return point;
#else
			Point location = LayoutHelper.GetRelativeElementRect(DataControl, LayoutHelper.FindRoot(DataControl) as UIElement).TopLeft();
			PointHelper.Offset(ref location, point.X, point.Y);
			return location;
#endif
		}
		void GridControlAdornerPanel_MouseMove(object sender, MouseEventArgs e) {
#if !SILVERLIGHT
			ReraiseEventHelper.ReraiseEvent<MouseEventArgs>(e, GetHitTestElement(e), FrameworkElement.PreviewMouseMoveEvent, FrameworkElement.MouseMoveEvent, ReraiseEventHelper.CloneMouseEventArgs);
#else
#endif
			e.Handled = true;
		}
		UIElement GetHitTestElement(MouseEventArgs e) {
			return GetHitTestElement(GetGridMousePosition(e));
		}
		UIElement GetHitTestElement(Point point) {
			HitTestResult hitResult = null;
			HitTestResultCallback resultCallback = delegate(HitTestResult result) {
				hitResult = result;
				return HitTestResultBehavior.Stop;
			};
#if !SILVERLIGHT
			VisualTreeHelper.HitTest(DataControl, HiTestFilter, resultCallback, new PointHitTestParameters(point));
#else
			if(DataControl.Parent != null)
				HitTestHelper.HitTest(DataControl, HiTestFilter, resultCallback, new PointHitTestParameters(GetSilverlightHitTestPoint(point)), false, false);
#endif
			if(hitResult == null)
				return null;
			DependencyObject visualHit = hitResult.VisualHit as DependencyObject;
			return visualHit != null ? LayoutHelper.FindParentObject<UIElement>(visualHit) : null;
		}
		HitTestFilterBehavior HiTestFilter(DependencyObject potentialHitTestTarget) {
				FrameworkElement element = potentialHitTestTarget as FrameworkElement;
				if(element == null || LayoutHelper.FindParentObject<DesignTimeSelectionControl>(element) != null || !UIElementHelper.IsVisibleInTree(element))
					return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
				return HitTestFilterBehavior.Continue;
		}
		Point GetGridMousePosition(MouseEventArgs e) {
			double zoom = DesignerView.FromContext(Context).ZoomLevel;
			Point position = DesignHelper.ConvertToPlatformPoint(e.GetPosition(hookPanel));
			position.X /= zoom;
			position.Y /= zoom;
			return position;
		}
		DataPresenterBase GetDataPresenter() {
			return GridControlHelper.GetDataPresenter(View);
		}
		void DestroyHeaderAdorner() {
			if(highlightedColumnHeaders != null) {
				highlightedColumnHeaders.ForEach(header => GridColumnHeaderBase.SetIsSelectedInDesignTime(header, false));
				highlightedColumnHeaders = null;
			}
			if(highlightedBandHeader != null)
				GridColumnHeaderBase.SetIsSelectedInDesignTime(highlightedBandHeader, false);
		}
		IDataViewHitInfo CalcHitInfo(Point point, out UIElement hitTestElement) {
			hitTestElement = GetHitTestElement(point);
			if(hitTestElement == null)
				return null;
			if(View is ITableView)
				return ((ITableView)View).CalcHitInfo(hitTestElement);
			return CalcHitInfoCore(hitTestElement);
		}
		protected virtual IDataViewHitInfo CalcHitInfoCore(UIElement hitTestElement) { return null; }
		protected override Control CreateHookPanel() {
			UpdateAvailableViewTypes();
			if(designTimeModel == null)
				designTimeModel = new DesignTimeModel(this);
			return new GridControlAdornerPanel() { DataContext = designTimeModel };
		}
		protected override void Activate(ModelItem item) {
			base.Activate(item);
			AdornedElement.Context.Items.Subscribe<Selection>(new SubscribeContextCallback<Selection>(this.OnSelectionChanged));
			SubscribeGridEvents();
			if(DataControl == null) return;
			DataControl.SelectedItem = null;
#if SILVERLIGHT
#endif
		}
		protected override void Deactivate() {
			UnsubscribeGridEvents();
			AdornedElement.Context.Items.Unsubscribe<Selection>(new SubscribeContextCallback<Selection>(this.OnSelectionChanged));
			base.Deactivate();
		}
		void SubscribeGridEvents() {
			if(DataControl == null)
				return;
			GridControlHelper.SetDesignTimeEventsListener(DataControl, this);
			DataControl.Unloaded += new RoutedEventHandler(OnGridControlUnloaded);
			UpdateDesignTimeInfoCore();
		}
		void UnsubscribeGridEvents() {
			if(DataControl == null)
				return;
			DataControl.Unloaded -= new RoutedEventHandler(OnGridControlUnloaded);
			GridControlHelper.SetDesignTimeEventsListener(DataControl, null);
		}
		void OnGridControlUnloaded(object sender, RoutedEventArgs e) {
			UnsubscribeGridEvents();
			RefreshPlatformObject();
			SubscribeGridEvents();
		}
		void OnSelectionChanged(Selection newSelection) {
			if(DataControl == null) return;
			UpdateDesignTimeInfoCore();
			RefreshHeadersAdorner();
		}
		void RefreshHeadersAdorner() {
			DestroyHeaderAdorner();
			ModelItem primarySelection = GridDesignTimeHelper.GetPrimarySelection(AdornedElement.Context);
			if(IsColumnSelected(primarySelection)) {
				RefreshColumnAdorner(primarySelection);
				return;
			}
			if(IsBandSelected(primarySelection)) {
				RefreshBandAdorner(primarySelection);
				return;
			}
		}
		void RefreshColumnAdorner(ModelItem primarySelection) {
			ColumnBase column = GetColumnBytModelItem(primarySelection);
			if(column == null)
				return;
			ScrollToHeader(column);
			GridColumnHeader[] headerElements = GridControlHelper.GetColumnHeaderElements(DataControl, column);
			if(headerElements == null)
				return;
			highlightedColumnHeaders = new List<GridColumnHeader>();
			foreach(GridColumnHeader headerElement in headerElements) {
				highlightedColumnHeaders.Add(headerElement);
				GridColumnHeaderBase.SetIsSelectedInDesignTime(headerElement, true);
			}
		}
		void RefreshBandAdorner(ModelItem primarySelection) {
			BandBase band = GetBandBytModelItem(primarySelection);
			if(band == null)
				return;
			ScrollToHeader(band);
			BandHeaderControl bandHeader = GridControlHelper.GetBandHeaderElement(DataControl, band);
			if(bandHeader == null)
				return;
			highlightedBandHeader = bandHeader;
			GridColumnHeaderBase.SetIsSelectedInDesignTime(highlightedBandHeader, true);
		}
		#region IDesignTimeGridAdorner Members
		bool IDesignTimeAdornerBase.SkipColumnXamlGenerationProperties { get; set; }
		void IDesignTimeAdornerBase.OnColumnMoved() {
			PerformEditAction(SR.ReorderColumnDescription, UpdateVisibleIndexes);
		}
		void IDesignTimeAdornerBase.OnColumnHeaderClick() {
			PerformEditAction(SR.ChangeSortingDescription, UpdateGroupSort);
		}
		void IDesignTimeAdornerBase.OnColumnResized() {
			PerformEditAction(SR.ChangeColumnWidthDescription, UpdateColumnWidths);
		}
		void IDesignTimeAdornerBase.UpdateDesignTimeInfo() {
			UpdateDesignTimeInfoCore();
		}
		DesignTimeBandMoveProvider bandMoveProvider = null;
		IBandMoveProvider IDesignTimeAdornerBase.BandMoveProvider {
			get {
				if(bandMoveProvider == null)
					bandMoveProvider = new DesignTimeBandMoveProvider(this);
				return bandMoveProvider; 
			}
		}
		DesignTimeColumnMoveToBandProvider columnMoveToBandProvider = null;
		IColumnMoveToBandProvider IDesignTimeAdornerBase.ColumnMoveToBandProvider {
			get {
				if(columnMoveToBandProvider == null)
					columnMoveToBandProvider = new DesignTimeColumnMoveToBandProvider(this);
				return columnMoveToBandProvider;
			}
		}
		bool IDesignTimeAdornerBase.ForceAllowUseColumnInFilterControl { get { return true; } }
		DataViewBase IDesignTimeAdornerBase.GetDefaultView(DataControlBase dataControl) {
			return CreateDefaultView();
		}
		protected abstract DataViewBase CreateDefaultView();
		void IDesignTimeAdornerBase.OnColumnsLayoutChanged() {
			DataControl.Dispatcher.BeginInvoke(new Action(RefreshHeadersAdorner)
#if !SILVERLIGHT
				, DispatcherPriority.Render
#endif
			);
		}
		bool IDesignTimeAdornerBase.IsSelectGridArea(Point point) {
			Point offset = LayoutHelper.GetRelativeElementRect(View, DataControl).Location();
			point.X += offset.X;
			point.Y += offset.Y;
			System.Windows.Media.HitTestResult hitTestResult = System.Windows.Media.VisualTreeHelper.HitTest(hookPanel, DesignHelper.ConvertFromPlatformPoint(point));
			return hitTestResult != null && hitTestResult.VisualHit != null ? GetHitColumn(point) == null && !WpfLayoutHelper.IsChildElementEx(GridControlAdornerPanel.controlPanel, hitTestResult.VisualHit) : true;
		}
		void IDesignTimeAdornerBase.InvalidateDataSource() {
			GridControlHelper.InvalidateDesignTimeDataSource(DataControl);
		}
		void IDesignTimeAdornerBase.RemoveGeneratedColumns(DataControlBase dataControl) {
			GridControlHelper.ClearAutoGeneratedBandsAndColumns(dataControl);
		}
		void IDesignTimeAdornerBase.UpdateVisibleIndexes(DataControlBase dataControl) {
#if !SL
			UpdateVisibleIndexes();
#endif
		}
		 Locker PopulateColumnsLocker = new Locker();
		void RefreshDataControl() {
			if(dataControl == null || dataControl.Parent != null)
				return;
			dataControl = null;
			RefreshPlatformObject();
			if(DataControl == null)
				return;
			GridControlHelper.SetDesignTimeEventsListener(DataControl, this);
			PopulateColumnsLocker.DoLockedActionIfNotLocked(() => GridControlHelper.InvalidateDesignTimeDataSource(DataControl));
		}
		void UpdateAvailableViewTypes() {
			availableViewTypes = GetAvailableViewTypes();
			if(designTimeModel != null)
				designTimeModel.UpdateChangeViewInfo(this);
		}
	#endregion
		void UpdateDesignTimeInfoCore() {
			RefreshDataControl();
			UpdateAvailableViewTypes();
			designTimeModel.IsPanelLeftAligned = GridControlHelper.GetIsDesignTimeAdornerPanelLeftAligned(View);
			designTimeModel.IsAdornerPanelExpanded = GetIsAdornerPanelExpanded(DataControl);
			bool isViewSet = false;
			ModelProperty viewProperty = GetViewModelProperty();
			if(viewProperty != null)
				isViewSet = viewProperty.IsSet;
			designTimeModel.IsViewSet = isViewSet;
			designTimeModel.GridViewInfo = GetElementInfo(View);
			designTimeModel.ViewIcon = View != null ? DesignTimeModel.GetImageByViewType(View.GetType()) : null;
			designTimeModel.GridControlInfo = GetElementInfo(DataControl);
			UpdateChangeInfo(designTimeModel.ChangeViewInfo, isViewSet, View);
			ModelItem primarySelection = GridDesignTimeHelper.GetPrimarySelection(AdornedElement.Context);
			designTimeModel.IsGridSelected = primarySelection == AdornedElement;
			designTimeModel.IsViewSelected = primarySelection == GetViewModelItem();
			bool isColumnSelected = IsColumnSelected(primarySelection);
			bool isEditSettingsSelected = IsEditSettingsSelected(primarySelection);
			ModelItem headerItem = GetColumnSelectedObject(primarySelection);
			designTimeModel.IsColumnSelected = isColumnSelected;
			designTimeModel.IsEditSettingsSelected = isEditSettingsSelected;
			designTimeModel.IsColumnControlPanelVisible = isColumnSelected || isEditSettingsSelected;
			if(designTimeModel.IsColumnControlPanelVisible) {
				bool isEditSettingsSet = headerItem.Properties[DesignHelper.GetPropertyName(ColumnBase.EditSettingsProperty)].IsSet;
				ColumnBase column = GetColumnBytModelItem(headerItem);
				if(column != null) {
					designTimeModel.GridColumnInfo = GetElementInfo(column);
					if(isEditSettingsSet && column.EditSettings != null) {
						designTimeModel.EditSettingsInfo = GetElementInfo(column.EditSettings, null);
						designTimeModel.EditSettingsIcon = column.EditSettings != null ? DesignTimeModel.GetImageByEditSettingsType(column.EditSettings.GetType()) : null;
					}
					designTimeModel.IsEditSettingsSet = isEditSettingsSet;
					UpdateChangeInfo(designTimeModel.ChangeEditSettingsInfo, isEditSettingsSet, column.EditSettings);
				}
			}
			bool isBandSelected = IsBandSelected(primarySelection);
			designTimeModel.IsBandSelected = isBandSelected;
			designTimeModel.IsBandControlPanelVisible = isBandSelected;
			if(designTimeModel.IsBandControlPanelVisible) {
				BandBase band = GetBandBytModelItem(headerItem);
				if(band != null)
					designTimeModel.GridBandInfo = GetElementInfo(band);
			}
		}
		ModelItem GetColumnSelectedObject(ModelItem primarySelection) {
			return IsEditSettingsSelected(primarySelection) ? primarySelection.Parent : primarySelection;
		}
		ColumnBase GetColumnBytModelItem(ModelItem columnItem) {
			if(IsOwnColumnItem(columnItem))
				return columnItem.GetCurrentValue() as ColumnBase;
			return null;
		}
		BandBase GetBandBytModelItem(ModelItem bandItem) {
			if(IsOwnBandItem(bandItem))
				return bandItem.GetCurrentValue() as BandBase;
			return null;
		}
		bool IsEditSettingsSelected(ModelItem primarySelection) {
			return primarySelection != null && primarySelection.Parent != null && IsOwnColumnItem(primarySelection.Parent);
		}
		bool IsColumnSelected(ModelItem primarySelection) {
			return primarySelection != null && IsOwnColumnItem(primarySelection);
		}
		bool IsBandSelected(ModelItem primarySelection) {
			return primarySelection != null && IsOwnBandItem(primarySelection);
		}
		bool IsOwnColumnItem(ModelItem columnItem) {
			return GetColumnModelItem(columnItem.GetCurrentValue() as ColumnBase) != null;
		}
		bool IsOwnBandItem(ModelItem bandItem) {
			return GridDesignTimeHelper.GetBandModelItem(AdornedElement, bandItem.GetCurrentValue() as BandBase) != null;
		}
		void UpdateColumnWidths() {
			GridControlHelper.BeginUpdateColumnsLayout(View);
			try {
				foreach(ColumnBase column in Columns) {
					ModelItem columnModelItem = GetColumnBaseModelItem(column);
					if(columnModelItem == null)
						continue;
					ModelProperty widthProperty = columnModelItem.Properties[DesignHelper.GetPropertyName(ColumnBase.WidthProperty)];
					if(double.IsNaN(column.ActualWidth)) {
						widthProperty.ClearValue();
					} else {
						widthProperty.SetValue(Math.Floor(column.ActualWidth));
					}
				}
			} finally {
				GridControlHelper.EndUpdateColumnsLayout(View, false);
			}
		}
		protected internal void UpdateVisibleIndexes() {
			UpdateBandVisibleIndexes();
			UpdateColumnVisibleIndexes();
		}
		protected void UpdateBandVisibleIndexes() {
			if(!IsBandsSet())
				return;
			GridDesignTimeHelper.GetBandModelItem(AdornedElement, (BandBase)GridControlHelper.GetVisibleBands(DataControl)[0]);
			BandBase[] visibleBands = GridControlHelper.GetVisibleBands(DataControl).ToArray();
			foreach (BandBase band in visibleBands) {
				ModelItem bandModelItem = GridDesignTimeHelper.GetBandModelItem(AdornedElement, (BandBase)band);
				if(bandModelItem == null)
					continue;
				bandModelItem.Properties[DesignHelper.GetPropertyName(BaseColumn.VisibleIndexProperty)].SetValue(band.VisibleIndex);
			}
		}
		bool IsBandsSet() {
			return AdornedElement.Properties["Bands"].Collection.Count > 0;
		}
		protected void UpdateColumnVisibleIndexes() {
			foreach(ColumnBase column in GridControlHelper.GetVisibleColumns(View)) {
				SetColumnVisibleIndexProperty(column, column.VisibleIndex);
				SetColumnRowProperty(column, BandBase.GetGridRow(column));
			}
		}
		internal void SetColumnVisibleIndexProperty(BaseColumn column, int value) {
			ModelItem columnModelItem = GetColumnBaseModelItem(column);
			if(columnModelItem == null)
				return;
			int collectionIndex = GetColumnModelCollectionIndex(columnModelItem);
			if(value == -1) {
				columnModelItem.Properties[DesignHelper.GetPropertyName(BaseColumn.VisibleIndexProperty)].ClearValue();
				return;
			}
			columnModelItem.Properties[DesignHelper.GetPropertyName(BaseColumn.VisibleIndexProperty)].SetValue(value);
		}
		int GetColumnModelCollectionIndex(ModelItem columnModelItem) {
			ModelItem parent = columnModelItem.Parent;
			if(parent == null)
				return -1;
			return parent.Properties["Columns"].Collection.IndexOf(columnModelItem);
		}
		internal void SetColumnRowProperty(BaseColumn column, int value) {
			ModelItem columnModelItem = GetColumnBaseModelItem(column);
			if(columnModelItem == null)
				return;
			PropertyIdentifier rowIdentifier = new PropertyIdentifier(typeof(BandBase), "GridRow");
			if(value != 0) {
				columnModelItem.Properties[rowIdentifier].SetValue(value);
				return;
			}
			if(columnModelItem.Properties[rowIdentifier].IsSet)
				columnModelItem.Properties[rowIdentifier].ClearValue();
		}
		protected void MoveBand(BandBase source, BandBase target) {
		}
		protected void UpdateGroupSort() {
			foreach(ColumnBase column in Columns) {
				ModelItem columnModelItem = GetColumnBaseModelItem(column);
				if(columnModelItem == null)
					continue;
				ModelProperty sortIndexProperty = columnModelItem.Properties[DesignHelper.GetPropertyName(ColumnBase.SortIndexProperty)];
				ModelProperty groupIndexProperty = GetGroupIndexProperty(columnModelItem);
				ModelProperty sortOrderProperty = columnModelItem.Properties[DesignHelper.GetPropertyName(ColumnBase.SortOrderProperty)];
				bool isGrouped = (column is GridColumn) && ((GridColumn)column).IsGrouped;
				if(isGrouped) {
					sortIndexProperty.ClearValue();
					if(groupIndexProperty != null)
						groupIndexProperty.SetValue(((GridColumn)column).GroupIndex);
					sortOrderProperty.SetValue(column.SortOrder);
				} else if(column.IsSorted) {
					sortIndexProperty.SetValue(column.SortIndex);
					if(groupIndexProperty != null)
						groupIndexProperty.ClearValue();
					sortOrderProperty.SetValue(column.SortOrder);
				} else {
					sortIndexProperty.ClearValue();
					if(groupIndexProperty != null)
						groupIndexProperty.ClearValue();
					sortOrderProperty.ClearValue();
				}
			}
		}
		protected abstract ModelProperty GetGroupIndexProperty(ModelItem columnModelItem);
		protected void PerformEditAction(string description, Action editAction) {
			using(ModelEditingScope batchedChangeRoot = AdornedElement.BeginEdit(description)) {
				editAction();
				batchedChangeRoot.Complete();
			}
		}
		public bool IsDesignTime { get { return true; } }
		IModelItem IDesignTimeAdornerBase.GetDataControlModelItem(DataControlBase dataControl) {
			return XpfModelItem.FromModelItem(AdornedElement);
		}
		IModelItem IDesignTimeAdornerBase.CreateModelItem(object obj, IModelItem parent) {
			XpfEditingContext xpfContext = parent.Context as XpfEditingContext;
			if(xpfContext == null)
				return null;
			EditingContext designContext = xpfContext.Value as EditingContext;
			if(designContext == null)
				return null;
			return XpfModelItem.FromModelItem(ModelFactory.CreateItem(designContext, obj));
		}
		public Type GetDefaultColumnType(ColumnBase column) {
			if(!DependencyObjectExtensions.IsPropertySet(column, ColumnBase.EditSettingsProperty))
				return typeof(object);
			return EditSettingsSourceTypeProvider.GetEditSettingsDefaultSourceType(column.EditSettings.GetType());
		}
	}
	public class GridControlAdornerProvider : DataControlAdornerProvider, IDesignTimeGridAdorner {
		protected internal override Type ColumnType { get { return typeof(GridColumn); } }
		protected internal override Type ControlType { get { return typeof(GridControl); } }
		protected override Type[] GetAvailableViewTypes() {
			return GridDesignTimeHelper.GetAvailableViewTypes(DataControl);
		}
		void IDesignTimeGridAdorner.OnColumnMovedGroup() {
			PerformEditAction(SR.ChangeGroupingDescription, delegate {
				UpdateVisibleIndexes();
				UpdateGroupSort();
			});
		}
		protected override IDataViewHitInfo CalcHitInfoCore(UIElement hitTestElement) {
#if !SILVERLIGHT
			if(View is CardView)
				return ((CardView)View).CalcHitInfo(hitTestElement);
#endif
			return null;
		}
		protected override DataViewBase CreateDefaultView() {
			return new TableView();
		}
		protected override ModelProperty GetGroupIndexProperty(ModelItem columnModelItem) {
			return columnModelItem.Properties[DesignHelper.GetPropertyName(GridColumn.GroupIndexProperty)];
		}
	}
	public class TreeListControlAdornerProvider : DataControlAdornerProvider {
		protected internal override Type ColumnType { get { return typeof(TreeListColumn); } }
		protected internal override Type ControlType { get { return typeof(TreeListControl); } }
		protected override DataViewBase CreateDefaultView() {
			return new TreeListView();
		}
		protected override ModelProperty GetGroupIndexProperty(ModelItem columnModelItem) {
			return null;
		}
		protected override Type[] GetAvailableViewTypes() {
			return GridDesignTimeHelper.TreeListAvailableViewTypes;
		}
	}
	public class DesignTimeColumnMoveToBandProvider : IColumnMoveToBandProvider {
		DataControlAdornerProvider provider;
		ModelEditingScope batchedChangeRoot;
		public DesignTimeColumnMoveToBandProvider(DataControlAdornerProvider provider) {
			this.provider = provider;
		}
		void IColumnMoveToBandProvider.StartMoving() {
			provider.SelectGrid();
			batchedChangeRoot = provider.AdornedElement.BeginEdit(SR.ReorderColumnDescription);
		}
		void IColumnMoveToBandProvider.EndMoving() {
			provider.UpdateVisibleIndexes();
			batchedChangeRoot.Complete();
			GridDesignTimeHelper.FillBandsColumnsAndRestoreGrouping(provider.DataControl);
			batchedChangeRoot.Dispose();
		}
		void IColumnMoveToBandProvider.SetRow(BaseColumn column, int value) {
			provider.SetColumnRowProperty(column, value);
			batchedChangeRoot.Update();
		}
		void IColumnMoveToBandProvider.SetVisibleIndex(ColumnBase column, int value) {
			provider.SetColumnVisibleIndexProperty(column, value);
			batchedChangeRoot.Update();
		}
		void IColumnMoveToBandProvider.MoveColumnToBand(BaseColumn column, BandBase from, BandBase target) {
			batchedChangeRoot.Update();
			ModelItem columnModel = provider.GetColumnModelItem(column as ColumnBase);
			ModelItem fromModel = GridDesignTimeHelper.GetBandModelItem(provider.AdornedElement, (BandBase)from);
			ModelItem targetModel = GridDesignTimeHelper.GetBandModelItem(provider.AdornedElement, (BandBase)target);
			if(columnModel == null || fromModel == null || targetModel == null)
				return;
			if(from != target) {
				targetModel.Properties["Columns"].Collection.Add(columnModel);
				batchedChangeRoot.Update();
			}
		}
	}
	public class DesignTimeBandMoveProvider : IBandMoveProvider {
		DataControlAdornerProvider provider;
		ModelEditingScope batchedChangeRoot;
		Dictionary<BandBase, ModelItem> bandsModelCache = new Dictionary<BandBase, ModelItem>();
		public DesignTimeBandMoveProvider(DataControlAdornerProvider provider) {
			this.provider = provider;
		}
		ModelItem GetBandModel(IBandsOwner bandCore) {
			if(bandCore is BandsLayoutBase || bandCore == null)
				return provider.AdornedElement;
			ModelItem result = GridDesignTimeHelper.GetBandModelItem(provider.AdornedElement, (BandBase)bandCore);
			return result ?? GetBandModelFrmoCache((BandBase)bandCore);
		}
		ModelItem GetBandModelFrmoCache(BandBase band) {
			foreach(var b in bandsModelCache) {
				if(b.Key == band)
					return b.Value;
			}
			return null;
		}
		public void SetVisibleIndex(BaseColumn source, int value) {
			provider.SetColumnVisibleIndexProperty(source, value);
			batchedChangeRoot.Update();
		}
		public void AddBand(IBandsOwner owner, ref BandBase band) {
			ModelItem ownerModel = GetBandModel(owner);
			ModelItem bandModel = GetBandModel(band);
			ModelItem newBandModel = CloneModelItemHelper.CloneItem(provider.AdornedElement.Context, bandModel);
			bandsModelCache.Remove(band);
			bandsModelCache.Add(newBandModel.GetCurrentValue() as BandBase, newBandModel);
			ownerModel.Properties["Bands"].Collection.Add(newBandModel);
			batchedChangeRoot.Update();
			band = newBandModel.GetCurrentValue() as BandBase;
		}
		public void MoveColumns(BandBase source, BandBase target) {
			ModelItem sourceModel = GetBandModel(source);
			ModelItem targetModel = GetBandModel(target);
			while(sourceModel.Properties["Columns"].Collection.Count > 0)
				targetModel.Properties["Columns"].Collection.Add(sourceModel.Properties["Columns"].Collection[0]);
			batchedChangeRoot.Update();
		}
		public void EndMovingBand() {
			bandsModelCache.Clear();
			provider.UpdateVisibleIndexes();
			batchedChangeRoot.Complete();
			GridDesignTimeHelper.FillBandsColumnsAndRestoreGrouping(provider.DataControl);
			batchedChangeRoot.Dispose();
		}
		public void RemoveBand(BandBase band) {
			ModelItem ownerModel = GetBandModel(GridControlHelper.GetBandOwner(band));
			ModelItem bandModel = GetBandModel(band);
			ownerModel.Properties["Bands"].Collection.Remove(bandModel);
			batchedChangeRoot.Update();
		}
		public void StartMovingBand() {
			provider.SelectGrid();
			foreach(var b in GridDesignTimeHelper.GetAllBandsModels(provider.AdornedElement))
				bandsModelCache.Add(b.GetCurrentValue() as BandBase, b);
			batchedChangeRoot = provider.AdornedElement.BeginEdit(SR.ReorderBandDescription);
		}
		public void MoveBands(BandBase source, BandBase target) {
			ModelItem sourceModel = GetBandModel(source);
			ModelItem targetModel = GetBandModel(target);
			ModelItemCollection c = targetModel.Properties["Bands"].Collection; 
			while(sourceModel.Properties["Bands"].Collection.Count > 0) {
				ModelItem modelItem = sourceModel.Properties["Bands"].Collection[0];
				sourceModel.Properties["Bands"].Collection.RemoveAt(0);
				targetModel.Properties["Bands"].Collection.Add(modelItem);
				batchedChangeRoot.Update();
			}
		}
	}
	internal static class CloneModelItemHelper {
		public static ModelItem CloneItem(EditingContext context, ModelItem sourceItem) {
			ModelItem newItem = GridDesignTimeHelper.CreateModelItem(context, sourceItem.ItemType);
			foreach(var prop in sourceItem.Properties) {
				if(!prop.IsBrowsable || !prop.IsSet || !IsDesignerSerializationVisible(prop))
					continue;
				PropertyIdentifier propId = prop.IsAttached ? new PropertyIdentifier(prop.AttachedOwnerType, prop.Name.Split('.').Last()) : new PropertyIdentifier(sourceItem.ItemType, prop.Name);
				if(propId.IsEmpty)
					continue;
				if(prop.IsCollection)
					UpdateCollection(context, newItem.Properties[propId], prop);
				else
					newItem.Properties[propId].SetValue(prop.Value);
			}
			newItem.Properties["Name"].ClearValue();
			return newItem;
		}
		static void UpdateCollection(EditingContext context, ModelProperty target, ModelProperty source) {
			foreach(var el in source.Collection)
				target.Collection.Add(CloneModelItemHelper.CloneItem(context, el));
		}
		static bool IsDesignerSerializationVisible(ModelProperty prop) {
			var p = prop.GetAttributes(typeof(DesignerSerializationVisibilityAttribute)).ToList();
			if(p.Count == 0)
				return true;
			return ((DesignerSerializationVisibilityAttribute)p[0]).Visibility != DesignerSerializationVisibility.Hidden;
		}
	}
	[UsesItemPolicy(typeof(SelectionPolicy))]
	class RemoveHeaderTaskProvider : DeleteItemTaskProviderBase {
		protected override void OnDeleteKeyPressed(ModelItem item) {
			base.OnDeleteKeyPressed(item);
			SelectionOperations.Select(item.Root.Context, item.Root);
			if(!item.ItemType.IsSubclassOf(typeof(BandBase)))
				DoEditAction(item, SR.DeleteColumnDescription, () => item.Parent.Properties["Columns"].Collection.Remove(item));
			else
				DoEditAction(item, SR.DeleteBandDescription, () => GridDesignTimeHelper.DeleteBandAndMoveColumnsToRoot(item));
		}
		void DoEditAction(ModelItem item, string description, Action action) {
			using(ModelEditingScope scope = item.Parent.BeginEdit(string.Format("Remove {0}", item.ItemType.Name))) {
				action();
				scope.Complete();
			}
		}
	}
}
