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
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.TreeMap.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.TreeMap {
	[DXToolboxBrowsableAttribute,
	ContentProperty("DataAdapter")]
	public class TreeMapControl : Control, ITreeMapLayoutCalculator, INativeItemsCollector {
		public static readonly DependencyProperty LayoutAlgorithmProperty = DependencyProperty.Register("LayoutAlgorithm",
			typeof(TreeMapLayoutAlgorithmBase), typeof(TreeMapControl), new PropertyMetadata(null, LayoutAlgorithmPropertyChanged));
		public static readonly DependencyProperty DataAdapterProperty = DependencyProperty.Register("DataAdapter",
			typeof(TreeMapDataAdapterBase), typeof(TreeMapControl), new PropertyMetadata(null, DataAdapterPropertyChanged));
		public static readonly DependencyProperty ColorizerProperty = DependencyProperty.Register("Colorizer",
			typeof(TreeMapColorizerBase), typeof(TreeMapControl), new PropertyMetadata(ColorizerPropertyChanged));
		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem",
			typeof(object), typeof(TreeMapControl), new PropertyMetadata(null, OnSelectedItemChanged));
		public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems",
			typeof(IList), typeof(TreeMapControl), new PropertyMetadata(null, OnSelectedItemsChanged));
		public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register("SelectionMode",
			typeof(SelectionMode), typeof(TreeMapControl), new PropertyMetadata(SelectionMode.Extended));
		public static readonly DependencyProperty ToolTipTemplateProperty = DependencyProperty.Register("ToolTipTemplate",
			typeof(DataTemplate), typeof(TreeMapControl), new PropertyMetadata(null));
		public static readonly DependencyProperty ToolTipContentTemplateProperty = DependencyProperty.Register("ToolTipContentTemplate",
			typeof(DataTemplate), typeof(TreeMapControl), new PropertyMetadata(null));
		public static readonly DependencyProperty ToolTipPatternProperty = DependencyProperty.Register("ToolTipPattern",
			typeof(string), typeof(TreeMapControl), new PropertyMetadata("{L}: {V}", OnToolTipPatternPropertyChanged));
		public static readonly DependencyProperty ToolTipGroupContentTemplateProperty = DependencyProperty.Register("ToolTipGroupContentTemplate",
			typeof(DataTemplate), typeof(TreeMapControl), new PropertyMetadata(null));
		public static readonly DependencyProperty ToolTipGroupPatternProperty = DependencyProperty.Register("ToolTipGroupPattern",
			typeof(string), typeof(TreeMapControl), new PropertyMetadata("{L}: {V}", OnToolTipPatternPropertyChanged));
		public static readonly DependencyProperty ToolTipEnabledProperty = DependencyProperty.Register("ToolTipEnabled",
			typeof(bool), typeof(TreeMapControl), new PropertyMetadata(false, OnToolTipEnabledPropertyChanged));
		public static readonly DependencyProperty ToolTipOptionsProperty = DependencyProperty.Register("ToolTipOptions",
			typeof(ToolTipOptions), typeof(TreeMapControl), new PropertyMetadata(null, ToolTipOptionsPropertyChanged));
		public static readonly DependencyProperty EnableHighlightingProperty = DependencyProperty.Register("EnableHighlighting",
			typeof(bool), typeof(TreeMapControl), new PropertyMetadata(true, OnEnableHighlightingPropertyChanged));
		public static readonly DependencyProperty GroupHeaderContentTemplateProperty = DependencyProperty.Register("GroupHeaderContentTemplate",
			typeof(DataTemplate), typeof(TreeMapControl), new PropertyMetadata(TemplateChanged));
		public static readonly DependencyProperty LeafContentTemplateProperty = DependencyProperty.Register("LeafContentTemplate",
			typeof(DataTemplate), typeof(TreeMapControl), new PropertyMetadata(TemplateChanged));
		static void DataAdapterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ElementHelper.ChangeOwner(d, e);
			TreeMapControl treeMap = d as TreeMapControl;
			if (treeMap != null)
				treeMap.UpdateDataAdapter();
		}
		static void LayoutAlgorithmPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TreeMapControl map = d as TreeMapControl;
			if (map != null)
				map.ActualLayoutAlgorithm = e.NewValue as TreeMapLayoutAlgorithmBase;
		}
		static void ColorizerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ElementHelper.ChangeOwner(d, e);
			TreeMapControl map = d as TreeMapControl;
			if (map != null)
				map.ColorizeItems();
		}
		static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TreeMapControl treeMap = d as TreeMapControl;
			if (treeMap != null && e.NewValue != e.OldValue)
				treeMap.OnUpdateSelectedItem(e.NewValue);
		}
		static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TreeMapControl treeMap = d as TreeMapControl;
			if (treeMap != null)
				treeMap.OnUpdateSelectedItems(e.OldValue as IList, e.NewValue as IList);
		}
		static void OnToolTipPatternPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TreeMapControl treeMap = d as TreeMapControl;
			if (treeMap != null)
				treeMap.ViewController.HideToolTip();
		}
		static void OnEnableHighlightingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TreeMapControl treeMap = d as TreeMapControl;
			if (treeMap != null)
				treeMap.OnUpdateEnableHighlighting((bool)e.NewValue);
		}
		static void OnToolTipEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TreeMapControl treeMap = d as TreeMapControl;
			if (treeMap != null && !((bool)e.NewValue))
				treeMap.ViewController.HideToolTip();
		}
		static void ToolTipOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TreeMapControl treeMap = d as TreeMapControl;
			if (treeMap != null)
				treeMap.actualToolTipOptions = e.NewValue != null ? (ToolTipOptions)e.NewValue : new ToolTipOptions();
		}
		static void TemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TreeMapControl treeMap = d as TreeMapControl;
			if (treeMap != null && treeMap.ActualItems != null)
				treeMap.UpdateItemsTemplate(treeMap.ActualItems);
		}
		[Category("Data"), TypeConverter(typeof(ExpandableObjectConverter))]
		public TreeMapDataAdapterBase DataAdapter {
			get { return (TreeMapDataAdapterBase)GetValue(DataAdapterProperty); }
			set { SetValue(DataAdapterProperty, value); }
		}
		[Category("Layout"), TypeConverter(typeof(ExpandableObjectConverter))]
		public TreeMapLayoutAlgorithmBase LayoutAlgorithm {
			get { return (TreeMapLayoutAlgorithmBase)GetValue(LayoutAlgorithmProperty); }
			set { SetValue(LayoutAlgorithmProperty, value); }
		}
		[Category("Appearance"), TypeConverter(typeof(ExpandableObjectConverter))]
		public TreeMapColorizerBase Colorizer {
			get { return (TreeMapColorizerBase)GetValue(ColorizerProperty); }
			set { SetValue(ColorizerProperty, value); }
		}
		[Category("Behavior")]
		public SelectionMode SelectionMode {
			get { return (SelectionMode)GetValue(SelectionModeProperty); }
			set { SetValue(SelectionModeProperty, value); }
		}
		[NonCategorized]
		public object SelectedItem {
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		[NonCategorized]
		public IList SelectedItems {
			get { return (IList)GetValue(SelectedItemsProperty); }
			set { SetValue(SelectedItemsProperty, value); }
		}
		[NonCategorized]
		public DataTemplate ToolTipTemplate {
			get { return (DataTemplate)GetValue(ToolTipTemplateProperty); }
			set { SetValue(ToolTipTemplateProperty, value); }
		}
		[NonCategorized]
		public DataTemplate ToolTipContentTemplate {
			get { return (DataTemplate)GetValue(ToolTipContentTemplateProperty); }
			set { SetValue(ToolTipContentTemplateProperty, value); }
		}
		[NonCategorized]
		public DataTemplate ToolTipGroupContentTemplate {
			get { return (DataTemplate)GetValue(ToolTipGroupContentTemplateProperty); }
			set { SetValue(ToolTipGroupContentTemplateProperty, value); }
		}
		[Category("Appearance")]
		public string ToolTipPattern {
			get { return (string)GetValue(ToolTipPatternProperty); }
			set { SetValue(ToolTipPatternProperty, value); }
		}
		[Category("Appearance")]
		public string ToolTipGroupPattern {
			get { return (string)GetValue(ToolTipGroupPatternProperty); }
			set { SetValue(ToolTipGroupPatternProperty, value); }
		}
		[Category("Behavior")]
		public bool ToolTipEnabled {
			get { return (bool)GetValue(ToolTipEnabledProperty); }
			set { SetValue(ToolTipEnabledProperty, value); }
		}
		[Category("Behavior")]
		public ToolTipOptions ToolTipOptions {
			get { return (ToolTipOptions)GetValue(ToolTipOptionsProperty); }
			set { SetValue(ToolTipOptionsProperty, value); }
		}
		[Category("Behavior")]
		public bool EnableHighlighting {
			get { return (bool)GetValue(EnableHighlightingProperty); }
			set { SetValue(EnableHighlightingProperty, value); }
		}
		[NonCategorized]
		public DataTemplate GroupHeaderContentTemplate {
			get { return (DataTemplate)GetValue(GroupHeaderContentTemplateProperty); }
			set { SetValue(GroupHeaderContentTemplateProperty, value); }
		}
		[NonCategorized]
		public DataTemplate LeafContentTemplate {
			get { return (DataTemplate)GetValue(LeafContentTemplateProperty); }
			set { SetValue(LeafContentTemplateProperty, value); }
		}
		public event TreeMapSelectionChangedEventHandler SelectionChanged;
		readonly TreeMapLayoutAlgorithmBase defaultAlgorithm = new SquarifiedLayoutAlgorithm();
		readonly ColorizerController colorizerController;
		readonly SelectedItemsController selectionController;
		readonly ViewController viewController;
		readonly HitTestController hitTestController;
		readonly ToolTipInfo toolTipInfo;
		ToolTipOptions actualToolTipOptions;
		TreeMapLayoutAlgorithmBase actualLayoutAlgorithm;
		TreeMapPanel panel;
		TreeMapItemsControl itemsContainer;
		InteractionData interactionData;
		TreeMapLayoutAlgorithmBase ActualLayoutAlgorithm {
			get { return actualLayoutAlgorithm; }
			set {
					actualLayoutAlgorithm = value != null ? value : new SquarifiedLayoutAlgorithm();
					InvalidateLayout();
			}
		}
		TreeMapPanel Panel {
			get {
				if (panel == null && itemsContainer != null)
					panel = LayoutHelper.FindElementByType<TreeMapPanel>(itemsContainer);
				return panel;
			}
		}
		internal TreeMapItemCollection ActualItems { get { return DataAdapter != null ? DataAdapter.ItemsCollection : null; } }
		internal ColorizerController ColorizerController { get { return colorizerController; } }
		internal SelectedItemsController SelectionController { get { return selectionController; } }
		internal ViewController ViewController { get { return viewController; } }
		internal ToolTipOptions ActualToolTipOptions { get { return actualToolTipOptions; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ToolTipInfo ToolTipInfo {
			get { return toolTipInfo; }
		}
		public TreeMapControl() {
			DefaultStyleKey = typeof(TreeMapControl);
			actualLayoutAlgorithm = new SquarifiedLayoutAlgorithm();
			colorizerController = new ColorizerController(this);
			selectionController = new SelectedItemsController(this);
			viewController = new ViewController(this);
			hitTestController = new HitTestController(this);
			interactionData = new InteractionData(this);
			toolTipInfo = new ToolTipInfo();
			actualToolTipOptions = new ToolTipOptions();
			MouseLeftButtonUp += viewController.OnLeftButtonUp;
			MouseMove += viewController.OnMouseMove;
			MouseLeave += viewController.OnMouseLeave;
		}
		void ITreeMapLayoutCalculator.CalculateLayout(IList<ITreeMapLayoutItem> treeMapItems, Size constraint) {
			if (treeMapItems.Count > 0 && constraint.Height != 0 && constraint.Width != 0) {
				ActualLayoutAlgorithm.Calculate(treeMapItems, constraint);
				ViewController.HideToolTip();
			}
		}
		TreeMapItem INativeItemsCollector.ProcessNativeItem(object item, object parent) {
			ISupportNativeSource supportNativeSource = DataAdapter as ISupportNativeSource;
			if (supportNativeSource != null)
				return supportNativeSource.ProcessNativeItem(item, parent);
			return null;
		}
		void UpdateDataAdapter() {
			if (itemsContainer != null) {
				if (DataAdapter is ISupportNativeSource) {
					itemsContainer.ItemsSource = ((ISupportNativeSource)DataAdapter).GetNativeSource();
					Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => { InvalidateLayout(); }));
				}
				else
					itemsContainer.ItemsSource = ActualItems;
				ColorizeItems();
			}
		}
		void OnUpdateSelectedItem(object item) {
			selectionController.OnUpdateSelectedItem(item);
		}
		void OnUpdateSelectedItems(IList oldList, IList newList) {
			if ((oldList == null || oldList.Count == 0) && (newList == null || newList.Count == 0))
				return;
			selectionController.OnUpdateSelectedItems(newList);
		}
		void OnUpdateEnableHighlighting(bool enable) {
			ViewController.SetActualItemHighlighting(enable);
		}
		void UpdateItemsTemplate(TreeMapItemCollection items) {
			foreach (TreeMapItem item in items) {
				item.UpdateTemplate();
				if (item.IsGroup)
					UpdateItemsTemplate(item.Children);
			}
		}
		internal void InvalidateLayout() {
			if (Panel != null)
				Panel.InvalidateMeasure();
		}
		internal void ColorizeItems() {
			if (colorizerController != null)
				colorizerController.ColorizeItems();
		}
		internal void UpdateItemSelection(ModifierKeys keyModifiers, ITreeMapItem item) {
			SelectionController.UpdateItemSelection(SelectionMode, keyModifiers, item);
		}
		internal void ClearSelection() {
			SelectionController.ClearSelectedItems();
		}
		internal void RaiseSelectionChanged() {
			if (SelectionChanged != null && SelectionMode != SelectionMode.None)
				SelectionChanged(this, new TreeMapSelectionChangedEventArgs(SelectedItems as List<object>));
		}
		public TreeMapHitInfo CalcHitInfo(Point point) {
			return new TreeMapHitInfo(hitTestController, point);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			itemsContainer = GetTemplateChild("PART_ItemsContainer") as TreeMapItemsControl;
			if (itemsContainer != null)
				itemsContainer.Interaction = interactionData;
			UpdateDataAdapter();
			selectionController.UpdateActualItemsSelection(ActualItems);
		}
	}
}
namespace DevExpress.Xpf.TreeMap.Native {
	public class InteractionData {
		TreeMapControl treeMap;
		public ITreeMapLayoutCalculator LayoutCalculator { get { return treeMap; } }
		public INativeItemsCollector Collector { get { return treeMap; } }
		public InteractionData(TreeMapControl treeMap) {
			this.treeMap = treeMap;
		}
	}
}
