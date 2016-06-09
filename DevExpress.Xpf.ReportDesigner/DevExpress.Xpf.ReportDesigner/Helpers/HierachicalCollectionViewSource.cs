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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Mvvm.Native;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using System.Collections;
using DevExpress.Xpf.Core;
using System.Windows.Data;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core.Native;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	interface IHierarchyCollectionViewSourceNodeOwner {
		HierarchyCollectionViewSource Root { get; }
		event EventHandler RootChanged;
		DataTemplate ItemTemplate { get; }
		event EventHandler ItemTemplateChanged;
		DataTemplateSelector ItemTemplateSelector { get; }
		event EventHandler ItemTemplateSelectorChanged;
	}
	class HierarchyCollectionViewSourceNodeCollection : ObservableCollection<HierarchyCollectionViewSourceNode> {
		IHierarchyCollectionViewSourceNodeOwner owner;
		public HierarchyCollectionViewSourceNodeCollection(IHierarchyCollectionViewSourceNodeOwner owner) {
			this.owner = owner;
		}
		protected override void ClearItems() {
			foreach(var item in this)
				item.Owner = null;
			base.ClearItems();
		}
		protected override void RemoveItem(int index) {
			this[index].Owner = null;
			base.RemoveItem(index);
		}
		protected override void InsertItem(int index, HierarchyCollectionViewSourceNode item) {
			base.InsertItem(index, item);
			item.Owner = owner;
		}
		protected override void SetItem(int index, HierarchyCollectionViewSourceNode item) {
			this[index].Owner = null;
			base.SetItem(index, item);
			item.Owner = owner;
		}
	}
	public class HierarchyCollectionViewSourceNode : FrameworkElement, IHierarchyCollectionViewSourceNodeOwner {
		public static readonly DependencyProperty ItemsSourceProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ItemTemplateProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ItemTemplateSelectorProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static DependencyProperty ItemsAttachedBehaviorStoreProperty;
		static HierarchyCollectionViewSourceNode() {
			ItemsAttachedBehaviorStoreProperty = DependencyProperty.Register("ItemsAttachedBehaviorStore", typeof(object), typeof(HierarchyCollectionViewSourceNode), new PropertyMetadata(null));
			DependencyPropertyRegistrator<HierarchyCollectionViewSourceNode>.New()
				.Register(d => d.ItemsSource, out ItemsSourceProperty, null, (d, e) =>
					ItemsAttachedBehaviorCore<HierarchyCollectionViewSourceNode, HierarchyCollectionViewSourceNode>.OnItemsSourcePropertyChanged(
						d, e, ItemsAttachedBehaviorStoreProperty, ItemTemplateProperty, ItemTemplateSelectorProperty, null,
						x => x.nodes, x => new HierarchyCollectionViewSourceNode()
					))
			;
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(HierarchyCollectionViewSourceNode), new PropertyMetadata(null, (d, e) => ((HierarchyCollectionViewSourceNode)d).OnItemTemplateChanged(e)));
			ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(HierarchyCollectionViewSourceNode), new PropertyMetadata(null, (d, e) => ((HierarchyCollectionViewSourceNode)d).OnItemTemplateSelectorChanged(e)));
		}
		HierarchyCollectionViewSourceNodeCollection nodes;
		public HierarchyCollectionViewSourceNode() {
			nodes = new HierarchyCollectionViewSourceNodeCollection(this);
		}
		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		IHierarchyCollectionViewSourceNodeOwner owner;
		internal IHierarchyCollectionViewSourceNodeOwner Owner {
			get { return owner; }
			set {
				if(owner != null) {
					owner.ItemTemplateChanged -= OnOwnerItemTemplateChanged;
					owner.ItemTemplateSelectorChanged -= OnOwnerItemTemplateSelectorChanged;
					owner.RootChanged -= OnOwnerRootChanged;
					Root = null;
					SetValue(ItemTemplateProperty, null);
					SetValue(ItemTemplateSelectorProperty, null);
				}
				owner = value;
				if(owner != null) {
					owner.RootChanged += OnOwnerRootChanged;
					owner.ItemTemplateChanged += OnOwnerItemTemplateChanged;
					owner.ItemTemplateSelectorChanged += OnOwnerItemTemplateSelectorChanged;
					OnOwnerItemTemplateChanged(owner, EventArgs.Empty);
					OnOwnerItemTemplateSelectorChanged(owner, EventArgs.Empty);
					OnOwnerRootChanged(owner, EventArgs.Empty);
				}
			}
		}
		void OnOwnerRootChanged(object sender, EventArgs e) {
			Root = Owner.Root;
		}
		void OnOwnerItemTemplateChanged(object sender, EventArgs e) {
			SetValue(ItemTemplateProperty, Owner.ItemTemplate);
		}
		void OnOwnerItemTemplateSelectorChanged(object sender, EventArgs e) {
			SetValue(ItemTemplateSelectorProperty, Owner.ItemTemplateSelector);
		}
		HierarchyCollectionViewSource root;
		HierarchyCollectionViewSource Root {
			get { return root; }
			set {
				if(root != null)
					root.DetachItem(this);
				root = value;
				if(root != null)
					root.AttachItem(this);
				if(rootChanged != null)
					rootChanged(this, EventArgs.Empty);
			}
		}
		void OnItemTemplateChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<HierarchyCollectionViewSource, HierarchyCollectionViewSourceNode>.OnItemsGeneratorTemplatePropertyChanged(this, e, ItemsAttachedBehaviorStoreProperty);
			if(itemTemplateChanged != null)
				itemTemplateChanged(this, EventArgs.Empty);
		}
		void OnItemTemplateSelectorChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<HierarchyCollectionViewSource, HierarchyCollectionViewSourceNode>.OnItemsGeneratorTemplatePropertyChanged(this, e, ItemsAttachedBehaviorStoreProperty);
			if(itemTemplateSelectorChanged != null)
				itemTemplateSelectorChanged(this, EventArgs.Empty);
		}
		EventHandler rootChanged;
		event EventHandler IHierarchyCollectionViewSourceNodeOwner.RootChanged {
			add { rootChanged += value; }
			remove { rootChanged -= value; }
		}
		HierarchyCollectionViewSource IHierarchyCollectionViewSourceNodeOwner.Root { get { return Root; } }
		DataTemplate IHierarchyCollectionViewSourceNodeOwner.ItemTemplate { get { return (DataTemplate)GetValue(ItemTemplateProperty); } }
		DataTemplateSelector IHierarchyCollectionViewSourceNodeOwner.ItemTemplateSelector { get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); } }
		EventHandler itemTemplateChanged;
		EventHandler itemTemplateSelectorChanged;
		event EventHandler IHierarchyCollectionViewSourceNodeOwner.ItemTemplateChanged {
			add { itemTemplateChanged += value; }
			remove { itemTemplateChanged -= value; }
		}
		event EventHandler IHierarchyCollectionViewSourceNodeOwner.ItemTemplateSelectorChanged {
			add { itemTemplateSelectorChanged += value; }
			remove { itemTemplateSelectorChanged -= value; }
		}
	}
	public class HierarchyCollectionViewSource : CollectionViewSource, IHierarchyCollectionViewSourceNodeOwner {
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static DependencyProperty ItemsAttachedBehaviorStoreProperty;
		static HierarchyCollectionViewSource() {
			ItemsAttachedBehaviorStoreProperty = DependencyProperty.Register("ItemsAttachedBehaviorStore", typeof(object), typeof(HierarchyCollectionViewSource), new PropertyMetadata(null));
			DependencyPropertyRegistrator<HierarchyCollectionViewSource>.New()
				.Register(d => d.ItemsSource, out ItemsSourceProperty, null, (d, e) =>
					ItemsAttachedBehaviorCore<HierarchyCollectionViewSource, HierarchyCollectionViewSourceNode>.OnItemsSourcePropertyChanged(
						d, e, ItemsAttachedBehaviorStoreProperty, ItemTemplateProperty, ItemTemplateSelectorProperty, null,
						x => x.nodes, x => new HierarchyCollectionViewSourceNode()
					))
				.Register(d => d.ItemTemplate, out ItemTemplateProperty, null, (d, e) => d.OnItemTemplateChanged(e))
				.Register(d => d.ItemTemplateSelector, out ItemTemplateSelectorProperty, null, (d, e) => d.OnItemTemplateSelectorChanged(e))
			;
		}
		ObservableCollection<object> list = new ObservableCollection<object>();
		HierarchyCollectionViewSourceNodeCollection nodes;
		public HierarchyCollectionViewSource() {
			nodes = new HierarchyCollectionViewSourceNodeCollection(this);
			Source = list;
		}
		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		internal void AttachItem(HierarchyCollectionViewSourceNode node) {
			list.Add(node.DataContext);
		}
		internal void DetachItem(HierarchyCollectionViewSourceNode node) {
			list.Remove(node.DataContext);
		}
		void OnItemTemplateChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<HierarchyCollectionViewSource, HierarchyCollectionViewSourceNode>.OnItemsGeneratorTemplatePropertyChanged(this, e, ItemsAttachedBehaviorStoreProperty);
			if(itemTemplateChanged != null)
				itemTemplateChanged(this, EventArgs.Empty);
		}
		void OnItemTemplateSelectorChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<HierarchyCollectionViewSource, HierarchyCollectionViewSourceNode>.OnItemsGeneratorTemplatePropertyChanged(this, e, ItemsAttachedBehaviorStoreProperty);
			if(itemTemplateSelectorChanged != null)
				itemTemplateSelectorChanged(this, EventArgs.Empty);
		}
		event EventHandler IHierarchyCollectionViewSourceNodeOwner.RootChanged { add { } remove { } }
		HierarchyCollectionViewSource IHierarchyCollectionViewSourceNodeOwner.Root { get { return this; } }
		DataTemplate IHierarchyCollectionViewSourceNodeOwner.ItemTemplate { get { return ItemTemplate; } }
		DataTemplateSelector IHierarchyCollectionViewSourceNodeOwner.ItemTemplateSelector { get { return ItemTemplateSelector; } }
		EventHandler itemTemplateChanged;
		EventHandler itemTemplateSelectorChanged;
		event EventHandler IHierarchyCollectionViewSourceNodeOwner.ItemTemplateChanged {
			add { itemTemplateChanged += value; }
			remove { itemTemplateChanged -= value; }
		}
		event EventHandler IHierarchyCollectionViewSourceNodeOwner.ItemTemplateSelectorChanged {
			add { itemTemplateSelectorChanged += value; }
			remove { itemTemplateSelectorChanged -= value; }
		}
	}
}
