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

using DevExpress.XtraPivotGrid.Customization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using PivotFieldsReadOnlyObservableCollection = System.Collections.ObjectModel.ReadOnlyObservableCollection<DevExpress.Xpf.PivotGrid.PivotGridField>;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using System.Windows.Media;
using System.Linq;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class TreeViewFieldsPresenter : TreeView {
		#region static staff
		public static readonly DependencyProperty ShowCheckBoxesProperty;
		public static readonly DependencyProperty GroupFieldsProperty;
		public static readonly DependencyProperty CoreFieldsProperty;
		static TreeViewFieldsPresenter() {
			Type ownerType = typeof(TreeViewFieldsPresenter);
			ShowCheckBoxesProperty = DependencyProperty.Register("ShowCheckBoxes", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((TreeViewFieldsPresenter)d).OnShowCheckBoxesPropertyChanged()));
			GroupFieldsProperty = DependencyProperty.Register("GroupFields", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((TreeViewFieldsPresenter)d).GroupFieldsChanged()));
			CoreFieldsProperty = DependencyProperty.Register("CoreFields", typeof(PivotFieldsReadOnlyObservableCollection), ownerType, new PropertyMetadata((d, e) => ((TreeViewFieldsPresenter)d).OnItemsChanged(e)));
		}
		protected virtual void OnShowCheckBoxesPropertyChanged() {
			EnsureHeadersShowCB(Items);
		}
		#endregion
		PivotCustomizationFieldsTree tree;
		PivotGridWpfData data;
		public TreeViewFieldsPresenter() {
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
		}
		public bool ShowCheckBoxes {
			get { return (bool)GetValue(ShowCheckBoxesProperty); }
			set { SetValue(ShowCheckBoxesProperty, value); }
		}
		public bool GroupFields {
			get { return (bool)GetValue(GroupFieldsProperty); }
			set { SetValue(GroupFieldsProperty, value); }
		}
		public PivotFieldsReadOnlyObservableCollection CoreFields {
			get { return (PivotFieldsReadOnlyObservableCollection)GetValue(CoreFieldsProperty); }
			set { SetValue(CoreFieldsProperty, value); }
		}	
		PivotGridWpfData Data {
			get {
				if(data == null)
					data = GetDataCore();
				return data;
			}
			set { data = value; }
		}
		PivotCustomizationFieldsTree Tree {
			get {
				if(tree == null && Data != null)
					tree = new PivotCustomizationFieldsTree(Data.FieldListFields, Data);
				return tree;
			}
		}
		protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e) {
			base.OnSelectedItemChanged(e);
			if(SelectedItem != null)
#if !SL
				OnItemsChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, SelectedItem, SelectedItem));
#else
			{ }
#endif
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateItems();
		}
#if SL
		bool IsLoaded { get; set; }
#endif
		void OnLoaded(object sender, RoutedEventArgs e) {
#if SL
			IsLoaded = true;
#endif
			EnsureSubscribed();
			UpdateItems();
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
#if SL
			IsLoaded = false;
#endif
			Unsubscribe();
		}
		void OnListsChanged(CustomizationFormFields customizationData) {
			UpdateItems();
		}
		void UpdateItems() {
			if(Tree == null) {
				Items.Clear();
				return;
			}
			Tree.CoreFields = CoreFields;
			Tree.Update(GroupFields);
			SynchronizeItems();
		}
		void GroupFieldsChanged() {
			Items.Clear();
			UpdateItems();
		}
		IVisualCustomizationTreeItem GetSubItemNode(TreeViewItem item) {
			return ((TreeViewFieldHeader)item.Header).Node;
		}
		void SynchronizeItems() {
			SynchronizeItemsCore(Tree.GetRootNode(), null);
		}
		void SynchronizeItemsCore(IVisualCustomizationTreeItem parentNode, TreeViewItem parent) {
			ItemCollection items = parent == null ? Items : parent.Items;
			int itemsCount = items.Count;
			List<IVisualCustomizationTreeItem> fields = parentNode.EnumerateChildren().Cast<IVisualCustomizationTreeItem>().ToList();
			int min = Math.Min(itemsCount, fields.Count);
			for(int i = 0; i < min; i++) {
				IVisualCustomizationTreeItem custItem = fields[i];
				TreeViewItem tvItem = (TreeViewItem)items[i];
				if(((TreeViewFieldHeader)tvItem.Header).Node != custItem) {
					if(!SetNode(tvItem, custItem)) {
						items.RemoveAt(i);
						tvItem = CreateTreeViewItem(custItem);
						items.Insert(i, tvItem);
					}
				}
				if(custItem.CanExpand)
					SynchronizeItemsCore(custItem, tvItem);
				else
					tvItem.Items.Clear();
			}
			if(items.Count > fields.Count)
				while(items.Count > fields.Count)
					items.RemoveAt(items.Count - 1);
			else
				for(int i = items.Count; i < fields.Count; i++) {
					IVisualCustomizationTreeItem custItem = fields[i];
					TreeViewItem treeViewItem = CreateTreeViewItem(custItem);
					items.Insert(i, treeViewItem);
					if(custItem.CanExpand)
						SynchronizeItemsCore(custItem, treeViewItem);
				}
		}
		TreeViewItem GetTreeViewItem(ItemCollection items, IVisualCustomizationTreeItem node) {
			foreach(TreeViewItem item in items) {
				if(GetSubItemNode(item).Equals(node))
					return item;
			}
			return null;
		}
		protected virtual TreeViewItem CreateTreeViewItem(IVisualCustomizationTreeItem node) {
			TreeViewItem treeViewItem;
			bool simple = node.Field == null || node.Parent != null && node.Parent.Parent != null;
			if(simple)
				treeViewItem = new TreeViewItem();
			else
				treeViewItem = new TreeViewItem() { Template = ListTreeViewItemTemplate, Background = ListTreeViewItenBackground };
			treeViewItem.Tag = simple;
			treeViewItem.Header = new TreeViewFieldHeader(node) { ShowCheckBox = node.Field != null && ShowCheckBoxes };
			RoutedEventHandler synchronizeNodeExpandState = (s, e) => {
				TreeViewItem item = (TreeViewItem)s;
				TreeViewFieldHeader header = (TreeViewFieldHeader)item.Header;
				header.Node.IsExpanded = item.IsExpanded;
#if !SL
				e.Handled = true;
#endif
			};
			treeViewItem.Expanded += synchronizeNodeExpandState;
			treeViewItem.Collapsed += synchronizeNodeExpandState;
			return treeViewItem;
		}
		bool SetNode(TreeViewItem item, IVisualCustomizationTreeItem node) {
			if(((bool)item.Tag) == (node.Field == null || node.Parent != null && node.Parent.Parent != null)) {
				TreeViewFieldHeader header = (TreeViewFieldHeader)item.Header;
				header.Node = node;
				header.ShowCheckBox = node.Field != null && ShowCheckBoxes;
				return true;
			} else {
				return false;
			}
		}
		PivotGridWpfData GetDataCore() {
			return PivotGridControl.GetDataFromAttachedPivot(this);
		}
		void EnsureHeadersShowCB(ItemCollection items) {
			for(int i = 0; i < items.Count; i++) {
				TreeViewItem item = (TreeViewItem)items[i];
				TreeViewFieldHeader header = item.Header as TreeViewFieldHeader;
				if(header != null)
					header.ShowCheckBox = header.Node.Field != null && ShowCheckBoxes;
				EnsureHeadersShowCB(item.Items);
			}
		}
		bool subscribed = false;
		void OnItemsChanged(DependencyPropertyChangedEventArgs e) {
			Unsubscribe(e.OldValue, true);
			if(IsLoaded)
				EnsureSubscribed();
			UpdateItems();
		}
		void Unsubscribe(object value = null, bool useValue = false) {
			if(!useValue)
				value = CoreFields;
			if(subscribed) {
				INotifyCollectionChanged incollection = value as INotifyCollectionChanged;
				if(incollection != null)
					incollection.CollectionChanged -= OnItemsChanged;
			}
			subscribed = false;
		}
		void EnsureSubscribed() {
			if(subscribed)
				return;
			if(CoreFields as INotifyCollectionChanged != null)
				((INotifyCollectionChanged)CoreFields).CollectionChanged += OnItemsChanged;
		}
		void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateItems();
		}
		SolidColorBrush listTreeViewItenBackground;
		SolidColorBrush ListTreeViewItenBackground {
			get {
				if(listTreeViewItenBackground == null)
					listTreeViewItenBackground = new SolidColorBrush(Colors.Transparent);
				return listTreeViewItenBackground;
			}
		}
		ControlTemplate listTreeViewItemTemplate;
		ControlTemplate ListTreeViewItemTemplate {
			get {
				if(listTreeViewItemTemplate == null)
					listTreeViewItemTemplate = XamlHelper.LoadObjectCore(@"<ControlTemplate
             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""    
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
"+
#if SL
			 @"xmlns:sdk=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk""" +
#endif
			 @"
                TargetType=""" + 
#if !SL
							   "{x:Type TreeViewItem}"
#else
							   "sdk:TreeViewItem"
#endif
					+ @""">
                    <ContentPresenter Content=""{TemplateBinding Header}"" HorizontalAlignment=""Stretch"" Margin=""10,2,4,2"" />
                </ControlTemplate>") as ControlTemplate;
				return listTreeViewItemTemplate;
			}
		}
	}
}
