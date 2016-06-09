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
using System.Linq;
using System.Windows;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Serialization;
namespace DevExpress.Xpf.LayoutControl.Serialization {
	public interface ISerializableItem : ISerializableCollectionItem {
		FrameworkElement Element { get; }
	}
	public interface ISerializableCollectionItem {
		string Name { get; set; }
		string TypeName { get; }
		string ParentName { get; set; }
		string ParentCollectionName { get; set; }
	}
	public class SerializableItem : ISerializableItem, ISupportInitialize {
		#region static
		[XtraSerializableProperty, Core.Serialization.XtraResetProperty(Core.Serialization.ResetPropertyMode.None)]
		public static string GetTypeName(DependencyObject obj) {
			return (string)obj.GetValue(TypeNameProperty);
		}
		public static void SetTypeName(DependencyObject obj, string value) {
			obj.SetValue(TypeNameProperty, value);
		}
		public static readonly DependencyProperty TypeNameProperty =
			DependencyProperty.RegisterAttached("TypeName", typeof(string), typeof(SerializableItem), new PropertyMetadata(string.Empty));
		[XtraSerializableProperty]
		public static string GetParentName(DependencyObject obj) {
			return (string)obj.GetValue(ParentNameProperty);
		}
		public static void SetParentName(DependencyObject obj, string value) {
			obj.SetValue(ParentNameProperty, value);
		}
		public static readonly DependencyProperty ParentNameProperty =
			DependencyProperty.RegisterAttached("ParentName", typeof(string), typeof(SerializableItem), new PropertyMetadata(string.Empty));
		[XtraSerializableProperty]
		public static string GetParentCollectionName(DependencyObject obj) {
			return (string)obj.GetValue(ParentCollectionNameProperty);
		}
		public static void SetParentCollectionName(DependencyObject obj, string value) {
			obj.SetValue(ParentCollectionNameProperty, value);
		}
		public static readonly DependencyProperty ParentCollectionNameProperty =
			DependencyProperty.RegisterAttached("ParentCollectionName", typeof(string), typeof(SerializableItem), new PropertyMetadata(string.Empty));
		#endregion
		[XtraSerializableProperty]
		public string Name { get; set; }
		[XtraSerializableProperty]
		public string TypeName { get; set; }
		[XtraSerializableProperty]
		public string ParentName { get; set; }
		[XtraSerializableProperty]
		public string ParentCollectionName { get; set; }
		[XtraSerializableProperty]
		public HorizontalAlignment HorizontalAlignment { get; set; }
		[XtraSerializableProperty]
		public VerticalAlignment VerticalAlignment { get; set; }
		[XtraSerializableProperty]
		public double Width { get; set; }
		[XtraSerializableProperty]
		public double Height { get; set; }
		public FrameworkElement Element { get; set; }
		public void BeginInit() { }
		public void EndInit() { }
	}
	public class SerializableItemCollection :
		List<ISerializableItem> {
	}
	public interface ISerializationController : IDisposable {
		LayoutControl Container { get; }
		SerializableItemCollection Items { get; set; }
		bool IsDeserializing { get; }
		void SaveLayout(object path);
		void RestoreLayout(object path);
		void OnClearCollection(XtraItemRoutedEventArgs e);
		object OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e);
		object OnFindCollectionItem(XtraFindCollectionItemEventArgs e);
	}
	public class SerializationController : ISerializationController {
		#region static
		public static void ClearLayout(LayoutControl container) {
			FrameworkElement[] items = container.GetLogicalChildren(false).ToArray();
			for(int i = 0; i < items.Length; i++) {
				ClearLayoutRecursively(items[i]);
			}
			items = container.AvailableItems.ToArray();
			for(int i = 0; i < items.Length; i++) {
				ClearLayoutRecursively(items[i]);
			}
			container.AvailableItems.Clear();
		}
		private static void ClearLayoutRecursively(FrameworkElement item) {
			item.RemoveFromVisualTree();
			LayoutControlBase nestedContainer = item as LayoutControlBase;
			if(nestedContainer != null) {
				FrameworkElement[] items = nestedContainer.GetLogicalChildren(false).ToArray();
				for(int i = 0; i < items.Length; i++) {
					ClearLayoutRecursively(items[i]);
				}
			}
		}
		public static ISerializationController GetSerializationController(DependencyObject dObj) {
			ISerializationController attached = GetSerializationController(dObj as LayoutControl);
			if(attached != null) return attached;
			LayoutControl container = GetContainer(dObj);
			return GetSerializationController(container);
		}
		static LayoutControl GetContainer(DependencyObject dObj) {
			return dObj as LayoutControl;
		}
		static ISerializationController GetSerializationController(LayoutControl container) {
			return (container != null) ? container.SerializationController : null;
		}
		#endregion
		const string AvailableItemsCollectionName = "AvailableItems";
		bool isDisposingCore = false;
		LayoutControl containerCore;
		public const string SpecialNameSignature = "$";
		public const string AvailableItemsSignature = SpecialNameSignature + AvailableItemsCollectionName;
		public const string DefaultID = "LayoutControl";
		List<string> originallyNamedItems;
		public SerializationController(LayoutControl container) {
			this.containerCore = container;
			SubscribeEvents();
		}
		void IDisposable.Dispose() {
			if(!IsDisposing) {
				this.isDisposingCore = true;
				OnDisposing();
			}
			GC.SuppressFinalize(this);
		}
		void SubscribeEvents() {
			DXSerializer.AddStartSerializingHandler(Container, OnStartSerializing);
			DXSerializer.AddEndSerializingHandler(Container, OnEndSerializing);
			DXSerializer.AddStartDeserializingHandler(Container, OnStartDeserializing);
			DXSerializer.AddEndDeserializingHandler(Container, OnEndDeserializing);
		}
		void UnSubscribeEvents() {
			DXSerializer.RemoveStartSerializingHandler(Container, OnStartSerializing);
			DXSerializer.RemoveEndSerializingHandler(Container, OnEndSerializing);
			DXSerializer.RemoveStartDeserializingHandler(Container, OnStartDeserializing);
			DXSerializer.RemoveEndDeserializingHandler(Container, OnEndDeserializing);
		}
		protected bool IsDisposing {
			get { return isDisposingCore; }
		}
		int startDeserializing = 0;
		public bool IsDeserializing {
			get { return startDeserializing > 0; }
		}
		protected void OnDisposing() {
			UnSubscribeEvents();
			ResetSerializableItems();
			containerCore = null;
		}
		public LayoutControl Container {
			get { return containerCore; }
		}
		protected virtual string GetAppName() {
			return "LayoutControl";
		}
		#region Save/Restore
		public void SaveLayout(object path) {
			DXSerializer.SerializeSingleObject(Container, path, GetAppName());
		}
		public void RestoreLayout(object path) {
			DXSerializer.DeserializeSingleObject(Container, path, GetAppName());
		}
		protected virtual void OnStartSerializing(object sender, RoutedEventArgs e) {
			BeginSaveLayout();
		}
		protected virtual void OnEndSerializing(object sender, RoutedEventArgs e) {
			EndSaveLayout();
		}
		protected virtual void OnStartDeserializing(object sender, StartDeserializingEventArgs e) {
			BeginRestoreLayout();
		}
		protected virtual void OnEndDeserializing(object sender, EndDeserializingEventArgs e) {
			EndRestoreLayout();
		}
		internal void BeginSaveLayout() {
			CollectSerializableItems();
		}
		internal void EndSaveLayout() {
			ResetSerializableItems();
		}
		internal void BeginRestoreLayout() {
			startDeserializing++;
			CollectSerializableItems();
			Items.Clear();
		}
		internal void EndRestoreLayout() {
			RestoreItems();
			startDeserializing--;
			UpdateContainer();
			ResetSerializableItems();
		}
		void UpdateContainer() {
			Container.InvalidateVisual();
		}
		#endregion Save/Restore        
		protected void CollectOriginallyNamedItems() {
			originallyNamedItems = new List<string>();
			CollectElementName(Container);
			CollectOriginallyNamedNestedItems(Container);
			foreach(var availableItem in Container.AvailableItems) {
				CollectElementName(availableItem);
				LayoutGroup layoutGroup = availableItem as LayoutGroup;
				if(layoutGroup != null) {
					CollectOriginallyNamedNestedItems(layoutGroup);
				}
			}
		}
		protected void CollectOriginallyNamedNestedItems(LayoutGroup nestedGroup) {
			FrameworkElements elements = nestedGroup.GetLogicalChildren(false);
			foreach(FrameworkElement child in elements) {
				CollectElementName(child);
				LayoutGroup layoutGroup = child as LayoutGroup;
				if(layoutGroup != null) {
					CollectOriginallyNamedNestedItems(layoutGroup);
				}
			}
		}
		protected void CollectElementName(FrameworkElement element) {
			if(!string.IsNullOrEmpty(element.Name)) {
				originallyNamedItems.Add(element.Name);					
			}
		}
		public SerializableItemCollection Items { get; set; }
		protected void CollectSerializableItems() {
			CollectOriginallyNamedItems();
			Items = new SerializableItemCollection();
			NamedItems = new Dictionary<string, ISerializableItem>();
			itemsWithInvalidNames = new List<ISerializableItem>();
			CollectSerializableNestedItems(Container);
			foreach(var availableItem in Container.AvailableItems) {
				SerializeElement(availableItem, AvailableItemsCollectionName);
				LayoutGroup layoutGroup = availableItem as LayoutGroup;
				if(layoutGroup != null) {
					CollectSerializableNestedItems(layoutGroup);
				}
			}
			PrepareSerializableItems();
		}
		protected void CollectSerializableNestedItems(LayoutGroup nestedGroup) {
			FrameworkElements elements = nestedGroup.GetLogicalChildren(false);
			foreach(FrameworkElement child in elements) {
				SerializeElement(child);
				LayoutGroup layoutGroup = child as LayoutGroup;
				if(layoutGroup != null) {
					CollectSerializableNestedItems(layoutGroup);
				}
			}
		}
		protected void SerializeElement(FrameworkElement element, string ParentCollectionName = null) {
			ISerializableItem srlzItem = element as ISerializableItem;
			if(srlzItem != null) {
				srlzItem.ParentCollectionName = ParentCollectionName;
				srlzItem.Name = element.Name;
				srlzItem.ParentName = ((FrameworkElement)element.Parent).Name;
				if(IsInvalidName(srlzItem.Name)) {
					EnsureUniqueName(NamedItems, srlzItem);
				}
			}
			else {
				srlzItem = new SerializableItem()
				{
					Name = element.Name,
					TypeName = element.GetType().Name,
					ParentName = ((FrameworkElement)element.Parent).Name,
					ParentCollectionName = ParentCollectionName,
					Height = element.Height,
					Width = element.Width,
					HorizontalAlignment = element.HorizontalAlignment,
					VerticalAlignment = element.VerticalAlignment,
					Element = element
				};
				if(IsInvalidName(srlzItem.Name)) {
					EnsureUniqueName(NamedItems, srlzItem);
				}
			}
			Items.Add(srlzItem);
		}
		protected void ResetSerializableItems() {
			Items = null;
			NamedItems = null;
			NewItems = null;
			TabIndexes = null;
			itemsWithInvalidNames = null;
		}
		internal Dictionary<string, ISerializableItem> NamedItems { get; private set; }
		internal List<ISerializableItem> NewItems { get; private set; }
		internal List<ISerializableItem> itemsWithInvalidNames { get; private set; }
		protected void RestoreItems() {
			ClearLayout(Container);
			Container.AvailableItems.Clear();
			BeginInitItems(NewItems);
			ProcessNewlyCreatedItems(NewItems);
			RestoreLayoutRelations();
			EndInitItems(NewItems);
			EndInitItems();
		}
		void BeginInitItems(List<ISerializableItem> newItems) {
			if(newItems == null) return;
			foreach(ISerializableItem item in newItems) {
				ISupportInitialize supportInitialize = item as ISupportInitialize;
				supportInitialize.Do(x => x.BeginInit());
			}
		}
		void EndInitItems(List<ISerializableItem> newItems) {
			if(newItems == null) return;
			foreach(ISerializableItem item in newItems) {
				ISupportInitialize supportInitialize = item as ISupportInitialize;
				supportInitialize.Do(x => x.EndInit());
			}
		}
		void EndInitItems() {
			foreach(ISerializableItem item in Items) {
				SerializableItem serializableItem = item as SerializableItem;
				if(serializableItem != null && serializableItem.Element != null) {
					serializableItem.Element.Height = serializableItem.Height;
					serializableItem.Element.Width = serializableItem.Width;
					serializableItem.Element.VerticalAlignment = serializableItem.VerticalAlignment;
					serializableItem.Element.HorizontalAlignment = serializableItem.HorizontalAlignment;
				}
			}
		}
		protected void PrepareSerializableItems() {
			foreach(ISerializableItem item in Items) {
				if(!IsInvalidName(item.Name)) {
					if(!NamedItems.ContainsKey(item.Name)) {
						NamedItems.Add(item.Name, item);
					}
				}
				else {
					itemsWithInvalidNames.Add(item);
				}
			}
			foreach(ISerializableItem item in itemsWithInvalidNames) {
				EnsureUniqueName(NamedItems, item);
			}
		}
		protected Dictionary<LayoutGroup, int> TabIndexes { get; private set; }
		protected void RestoreLayoutRelations() {
			List<ISerializableItem> restored = new List<ISerializableItem>();
			List<ISerializableItem> notRestored = new List<ISerializableItem>();
			foreach(ISerializableItem item in Items) {
				if(item.Element != null) {
					if(item.ParentName == Container.Name) {
						Container.Children.Add(item.Element);
						restored.Add(item);
					} 
					else {
						if(item.ParentCollectionName == AvailableItemsCollectionName) {
							Container.AvailableItems.Add(item.Element);
							restored.Add(item);
						} 
						else {
							ISerializableItem parent;
							if(NamedItems.TryGetValue(item.ParentName, out parent)) {
								LayoutControlBase parentGroup = parent as LayoutControlBase;
								if(parentGroup != null) {
									parentGroup.Children.Add(item.Element);
									restored.Add(item);
								}
							}
						}
					}
				}
			}
			ProcessNotRestoredItems(notRestored);
			CheckRestoredItems(restored);
		}
		protected virtual void CheckRestoredItems(List<ISerializableItem> restored) {
			bool addNewItems = RestoreLayoutOptions.GetAddNewItems(Container);
			foreach(var pair in NamedItems) {
				ISerializableItem oldItem = pair.Value as ISerializableItem;
				if(oldItem != null && !restored.Contains(oldItem) && oldItem.ParentCollectionName != AvailableItemsCollectionName) {
					if(addNewItems) {
						if(pair.Value.Element != null) {
							Container.AvailableItems.Add(pair.Value.Element);
						}
					}
				}
			}
		}
		protected void ProcessNewlyCreatedItems(List<ISerializableItem> newItems) {
			if(newItems == null)
				return;
			foreach(ISerializableItem item in newItems) {
				EnsureUniqueName(NamedItems, item);
			}
		}
		protected virtual void ProcessNotRestoredItems(List<ISerializableItem> notRestored) { }
		protected bool IsInvalidName(string name) {
			return string.IsNullOrEmpty(name) || name.StartsWith(SpecialNameSignature);
		}
		protected void EnsureUniqueName(Dictionary<string, ISerializableItem> namedItems, ISerializableItem item) {
			string name = item.Name;
			bool invalid = IsInvalidName(name);
			if(invalid || namedItems.ContainsKey(name) || originallyNamedItems.Contains(name)) {
				AssignUniqueName(item);
			}
			namedItems.Add(item.Name, item);
		}
		protected void AssignUniqueName(ISerializableItem item) {
			string newName = item.Name;
			if(string.IsNullOrEmpty(newName) || NamedItems.Keys.Contains(newName) || originallyNamedItems.Contains(newName)) {
				item.Name = GetUniqueName("_layoutItem", 1);
				item.Element.Name = item.Name;
			}
		}
		string GetUniqueName(string prefix, int initialValue) {
			int count = initialValue;
			while(true) {
				string name = prefix + count++;
				if(!NamedItems.Keys.Contains(name) && !originallyNamedItems.Contains(name))
					return name;
			}
		}
		public virtual void OnClearCollection(XtraItemRoutedEventArgs e) { }
		public virtual object OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			ISerializableItem resultItem = null;
			if(e.CollectionName == "Items") {
				XtraPropertyInfo info = e.Item.ChildProperties["Name"];
				XtraPropertyInfo infoType = e.Item.ChildProperties["TypeName"] ?? e.Item.ChildProperties["SerializableItem.TypeName"];
				if(info == null || infoType == null)
					return null;
				resultItem = CreateItem(info, infoType);
				if(resultItem != null) {
					if(NewItems == null)
						NewItems = new List<ISerializableItem>();
					NewItems.Add(resultItem);
					Items.Add(resultItem);
				}
			}
			return resultItem;
		}
		public virtual object OnFindCollectionItem(XtraFindCollectionItemEventArgs e) {
			if(e.CollectionName != "Items")
				return null;
			XtraPropertyInfo info = e.Item.ChildProperties["Name"];
			XtraPropertyInfo infoType = e.Item.ChildProperties["TypeName"] ?? e.Item.ChildProperties["SerializableItem.TypeName"];
			if(info == null || infoType == null)
				return null;
			ISerializableItem item = FindItem(NamedItems, (string)info.Value, (string)infoType.Value);
			if(item != null)
				Items.Add(item);
			return item;
		}
		protected ISerializableItem FindItem(Dictionary<string, ISerializableItem> namedItems, string name, string type) {
			ISerializableItem item;
			if(namedItems.TryGetValue(name, out item)) {
				if(item.TypeName != type) {
					namedItems.Remove(name);
					item = null;
				}
			}
			return item;
		}
		protected ISerializableItem CreateItem(XtraPropertyInfo info, XtraPropertyInfo infoType) {
			string typeStr = (string)infoType.Value;
			string prefix = "DevExpress.Xpf.LayoutControl.";
			if(typeStr.StartsWith(prefix))
				typeStr = typeStr.Remove(0, prefix.Length);
			if(CanRemoveOldItem(info, typeStr))
				return null;
			return CreateItemByType(info, typeStr);
		}
		protected virtual bool CanRemoveOldItem(XtraPropertyInfo info, string typeStr) {
			switch(typeStr) {
				case "LayoutItem":
				case "DataLayoutItem":
					return RestoreLayoutOptions.GetRemoveOldItems(Container);
			}
			return false;
		}
		protected virtual ISerializableItem CreateItemByType(XtraPropertyInfo info, string typeStr) {
			switch(typeStr) {
				case "LayoutGroup":
					return new LayoutGroup();
				case "LayoutItem":
					return new LayoutItem();
				case "DataLayoutItem":
					return new DataLayoutItem();
			}
			return null;
		}
	}
	public class LayoutControlSerializationProviderBase : SerializationProvider {
		static ISerializationController GetController(object obj) {
			return SerializationController.GetSerializationController(obj as DependencyObject);
		}
		protected override void OnCustomGetSerializableProperties(DependencyObject dObj, CustomGetSerializablePropertiesEventArgs e) {
			base.OnCustomGetSerializableProperties(dObj, e);
			DXSerializable dxSerializable = new DXSerializable();
			e.SetPropertySerializable("HorizontalAlignment", dxSerializable);
			e.SetPropertySerializable("VerticalAlignment", dxSerializable);
			e.SetPropertySerializable("Width", dxSerializable);
			e.SetPropertySerializable("Height", dxSerializable);
			e.SetPropertySerializable("Name", dxSerializable);
		}
		protected override void OnClearCollection(XtraItemRoutedEventArgs e) {
			ISerializationController Controller = GetController(e.Source);
			Controller.OnClearCollection(e);
		}
		protected override object OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			ISerializationController Controller = GetController(e.Source);
			return Controller.OnCreateCollectionItem(e);
		}
		protected override object OnFindCollectionItem(XtraFindCollectionItemEventArgs e) {
			ISerializationController Controller = GetController(e.Source);
			return Controller.OnFindCollectionItem(e);
		}
	}
	public class RestoreLayoutOptions {
		public static readonly DependencyProperty AddNewItemsProperty =
			DependencyProperty.RegisterAttached("AddNewItems", typeof(bool), typeof(RestoreLayoutOptions), new PropertyMetadata(true));
		public static readonly DependencyProperty RemoveOldItemsProperty =
			DependencyProperty.RegisterAttached("RemoveOldItems", typeof(bool), typeof(RestoreLayoutOptions), new PropertyMetadata(true));
		public static bool GetAddNewItems(DependencyObject obj) {
			return (bool)obj.GetValue(AddNewItemsProperty);
		}
		public static void SetAddNewItems(DependencyObject obj, bool value) {
			obj.SetValue(AddNewItemsProperty, value);
		}
		public static bool GetRemoveOldItems(DependencyObject obj) {
			return (bool)obj.GetValue(RemoveOldItemsProperty);
		}
		public static void SetRemoveOldItems(DependencyObject obj, bool value) {
			obj.SetValue(RemoveOldItemsProperty, value);
		}
	}
}
