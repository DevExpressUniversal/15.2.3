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

using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using DevExpress.Xpf.Design;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Core.Design.SmartTags;
namespace DevExpress.Xpf.Map.Design {
	internal class MapStructureItem : INotifyPropertyChanged {
		readonly ObservableCollection<MapStructureItem> elements;
		readonly ModelItem modelItem;
		readonly string name;
		public string Name {
			get {
				return name;
			}
		}
		public ModelItem ModelItem {
			get {
				return modelItem;
			}
		}
		public ObservableCollection<MapStructureItem> Elements {
			get {
				return elements;
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public MapStructureItem(ModelItem modelItem) {
			this.modelItem = modelItem;
			this.name = (string)modelItem.Properties["Name"].ComputedValue;
			if (string.IsNullOrEmpty(this.name))
				this.name = "[" + modelItem.ItemType.Name + "]";
			this.elements = new ObservableCollection<MapStructureItem>();
		}
		void RaisePropertyChanged(string name) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		public MapStructureItem FindElement(ModelItem modelItem) {
			foreach (MapStructureItem element in elements)
				if (element.ModelItem == modelItem)
					return element;
			return null;
		}
	}
	internal class MapStructureModel : INotifyPropertyChanged {
		readonly MapStructureItem root;
		MapStructureItem selection;
		public MapStructureItem Root {
			get {
				return root;
			}
		}
		public MapStructureItem[] TreeBinding {
			get {
				return new MapStructureItem[] { Root };
			}
		}
		public MapStructureItem Selection {
			get {
				return selection;
			}
			set {
				if (selection != value) {
					selection = value;
					RaisePropertyChanged("Selection");
					if (selection != null)
						SelectionOperations.Select(Root.ModelItem.Context, selection.ModelItem);
				}
			}
		}
		public ModelItem SelectedModel {
			set {
				if (value != null) {
					if (Root.ModelItem == value)
						Selection = Root;
					else
						Selection = Root.FindElement(value);
				}
				Selection = null;
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public MapStructureModel(ModelItem root) {
			this.root = new MapStructureItem(root);
			UpdateLayers();
		}
		void RaisePropertyChanged(string name) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		public void UpdateLayers() {
			Root.Elements.Clear();
			foreach (ModelItem layerItem in Root.ModelItem.Properties["Layers"].Collection)
				Root.Elements.Add(new MapStructureItem(layerItem));
		}
	}
	internal class TreeViewAllowTwoWaySelection {
		public static object GetSelectedItem(DependencyObject obj) {
			return (object)obj.GetValue(SelectedItemProperty);
		}
		public static void SetSelectedItem(DependencyObject obj, object value) {
			obj.SetValue(SelectedItemProperty, value);
		}
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.RegisterAttached("SelectedItem", typeof(object), typeof(TreeViewAllowTwoWaySelection), new PropertyMetadata(new object(), TreeViewSelectedItemChanged));
		static void TreeViewSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			TreeView treeView = sender as TreeView;
			if (treeView == null) {
				return;
			}
			treeView.SelectedItemChanged -= new RoutedPropertyChangedEventHandler<object>(OnSelectedItemChanged);
			treeView.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(OnSelectedItemChanged);
			TreeViewItem thisItem = treeView.ItemContainerGenerator.ContainerFromItem(e.NewValue) as TreeViewItem;
			if (thisItem != null) {
				thisItem.IsSelected = true;
				return;
			}
			for (int i = 0; i < treeView.Items.Count; i++)
				SelectItem(e.NewValue, treeView.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem);
		}
		static void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			TreeView treeView = sender as TreeView;
			SetSelectedItem(treeView, e.NewValue);
		}
		static bool SelectItem(object o, TreeViewItem parentItem) {
			if (parentItem == null || parentItem.Items == null || parentItem.ItemContainerGenerator == null)
				return false;
			bool isExpanded = parentItem.IsExpanded;
			if (!isExpanded) {
				parentItem.IsExpanded = true;
				parentItem.UpdateLayout();
			}
			TreeViewItem item = parentItem.ItemContainerGenerator.ContainerFromItem(o) as TreeViewItem;
			if (item != null) {
				item.IsSelected = true;
				return true;
			}
			bool wasFound = false;
			for (int i = 0; i < parentItem.Items.Count; i++) {
				TreeViewItem itm = parentItem.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
				var found = SelectItem(o, itm);
				if (!found && item != null)
					itm.IsExpanded = false;
				else
					wasFound = true;
			}
			return wasFound;
		}
	} 
	[RequiresService(typeof(ModelService))]
	[UsesItemPolicy(typeof(MapStructurePolicy))]
	internal class MapStructureAdornerProvider : DXAdornerProviderBase {
		MapStructureModel model;
		MapStructureAdorner adorner;
		protected AdornerPanel AdornerPanel {
			get {
				return myPanel;
			}
		}
		protected MapStructureAdorner Adorner {
			get {
				return adorner;
			}
		}
		void OnModelChanged(object sender, ModelChangedEventArgs e) {
			model.UpdateLayers();
		}
		void OnSelectionChanged(Selection item) {
			model.SelectedModel = item.PrimarySelection;
		}
		protected override Control CreateHookPanel() {
			adorner = new MapStructureAdorner();
			return adorner;
		}
		protected override void ConfigurePlacements(AdornerPlacementCollection placements) {
			placements.SizeRelativeToAdornerDesiredHeight(1.0, 0);
			placements.SizeRelativeToAdornerDesiredWidth(1.0, 0);
			placements.PositionRelativeToAdornerWidth(-1.0, -6);
			placements.PositionRelativeToContentHeight(0.0, 6.0);
		}
		protected override void Activate(ModelItem item) {
			base.Activate(item);
			AdornerPanel.HorizontalAlignment = HorizontalAlignment.Right;
			AdornerPanel.VerticalAlignment = VerticalAlignment.Top;
			AdornerPanel.IsEnabled = true;
			AdornerPanel.IsContentFocusable = true;
			model = new MapStructureModel(AdornedElement);
			Adorner.DataContext = model;
			Context.Services.GetRequiredService<ModelService>().ModelChanged += OnModelChanged;
			AdornedElement.Context.Items.Subscribe<Selection>(new SubscribeContextCallback<Selection>(this.OnSelectionChanged));
		}
		protected override void Deactivate() {
			Context.Services.GetRequiredService<ModelService>().ModelChanged -= OnModelChanged;
			base.Deactivate();
		}
	}
	internal class MapStructurePolicy : SelectionPolicy {
		protected override IEnumerable<ModelItem> GetPolicyItems(Selection selection) {
			ModelItem primarySelection = selection.PrimarySelection;
			if (primarySelection != null) {
				ModelItem mapControl = BarManagerDesignTimeHelper.FindParentByType<MapControl>(primarySelection);
				if (mapControl != null)
					return new ModelItem[] { mapControl };
			}
			return base.GetPolicyItems(selection);
		}
	}
}
