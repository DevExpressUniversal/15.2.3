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
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using System.ComponentModel;
using DevExpress.Mvvm.Native;
using System.Windows.Threading;
using DevExpress.Xpf.Core.Internal;
using System.Collections.Generic;
namespace DevExpress.Xpf.Core {
	internal class ItemsAttachedBehaviorProperties {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static object GetSource(DependencyObject obj) { return (object)obj.GetValue(SourceProperty); }
		public static void SetSource(DependencyObject obj, object value) { obj.SetValue(SourceProperty, value); }
		public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(object), typeof(ItemsAttachedBehaviorProperties), new PropertyMetadata(null));
	}
	public class ItemsAttachedBehaviorCore<TContainer, TItem> : IWeakEventListener
		where TContainer : DependencyObject
		where TItem : DependencyObject {
		static readonly ReflectionHelper reflectionHelper = new ReflectionHelper();
		[Flags]
		enum ResetProperties { None = 0x0, DataContext = 0x1, Style = 0x2 }
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ResetProperty = DependencyProperty.RegisterAttached("Reset", typeof(ResetProperties), typeof(ItemsAttachedBehaviorCore<TContainer, TItem>), new PropertyMetadata(ResetProperties.None));		
		public static TItem CreateItem(DependencyObject o, DependencyProperty itemsAttachedBehaviorStoreProperty, object dataContext) {
			ItemsAttachedBehaviorCore<TContainer, TItem> behavior = GetBehavior(o, itemsAttachedBehaviorStoreProperty);
			return behavior == null ? null : behavior.CreateItem(dataContext);
		}
		public static void OnItemsGeneratorTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e,
			DependencyProperty itemsAttachedBehaviorStoreProperty) {
			ItemsAttachedBehaviorCore<TContainer, TItem> behavior = GetBehavior(d, itemsAttachedBehaviorStoreProperty);
			if(behavior != null)
				behavior.Populate();
		}
		public static void OnItemsSourcePropertyChanged(
			DependencyObject d,
			DependencyPropertyChangedEventArgs e, 
			DependencyProperty itemsAttachedBehaviorStoreProperty,
			DependencyProperty itemGeneratorTemplateProperty, 
			DependencyProperty itemGeneratorTemplateSelectorProperty,
			DependencyProperty itemGeneratorStyleProperty, 
			Func<TContainer, IList> getTargetFunction,
			Func<TContainer, TItem> createItemDelegate,
			Action<int, object> insertItemAction = null,
			Action<TItem> setDefaultBindingAction = null,
			ISupportInitialize supportInitialize = null,
			Action<TItem, object> linkItemWithSourceAction = null,
			bool useDefaultTemplateSelector = true,
			bool useDefaultTemplateValidation = true,
			Func<TItem, bool> customClear = null
			) {
			ItemsAttachedBehaviorCore<TContainer, TItem> oldBehaviour = GetBehavior(d, itemsAttachedBehaviorStoreProperty);
			if(oldBehaviour != null)
				oldBehaviour.Source = null;
			var behaviour = new ItemsAttachedBehaviorCore<TContainer, TItem>(getTargetFunction, createItemDelegate, itemGeneratorTemplateProperty, 
				itemGeneratorTemplateSelectorProperty, itemGeneratorStyleProperty, e.NewValue as IEnumerable, setDefaultBindingAction, useDefaultTemplateSelector, useDefaultTemplateValidation, customClear);
			d.SetValue(itemsAttachedBehaviorStoreProperty, behaviour);
			behaviour.insertItemAction = insertItemAction;
			behaviour.supportInitialize = supportInitialize;
			behaviour.linkItemWithSourceAction = linkItemWithSourceAction;
			behaviour.ConnectToContainer((TContainer)d);
		}
		public static void OnTargetItemPositionChanged(DependencyObject o, DependencyProperty itemsAttachedBehaviorStoreProperty,
			int oldPosition, int newPosition) {
			ItemsAttachedBehaviorCore<TContainer, TItem> behavior = GetBehavior(o, itemsAttachedBehaviorStoreProperty);
			if (behavior != null)
				behavior.MoveSourceItem(oldPosition, newPosition);
		}
		static ItemsAttachedBehaviorCore<TContainer, TItem> GetBehavior(DependencyObject o, DependencyProperty itemsAttachedBehaviorStoreProperty) {
			return o.GetValue(itemsAttachedBehaviorStoreProperty) as ItemsAttachedBehaviorCore<TContainer, TItem>;
		}
		readonly Func<TContainer, IList> getTargetFunction;
		readonly Func<TContainer, TItem> createItemDelegate;
		readonly Action<TItem> setDefaultBindingAction;
		readonly bool useDefaultTemplateSelector;
		readonly bool useDefaultTemplateValidation;
		readonly Func<TItem, bool> customClear;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		readonly DependencyProperty itemGeneratorTemplateProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		readonly DependencyProperty itemGeneratorTemplateSelectorProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		readonly DependencyProperty itemGeneratorStyleProperty;
#if !SL
		readonly Dispatcher dispatcher;
#endif
		ItemsAttachedBehaviorCore(Func<TContainer, IList> getTargetFunction, Func<TContainer, TItem> createItemDelegate, 
			DependencyProperty itemGeneratorTemplateProperty, DependencyProperty itemGeneratorTemplateSelectorProperty,
			DependencyProperty itemGeneratorStyleProperty, IEnumerable source, Action<TItem> setDefaultBindingAction, bool useDefaultTemplateSelector, bool useDefaultTemplateValidation, Func<TItem, bool> customClear) {
			this.getTargetFunction = getTargetFunction;
			this.setDefaultBindingAction = setDefaultBindingAction;
			this.createItemDelegate = createItemDelegate;
			this.useDefaultTemplateSelector = useDefaultTemplateSelector;
			this.useDefaultTemplateValidation = useDefaultTemplateValidation;
			this.customClear = customClear;
			this.itemGeneratorTemplateProperty = itemGeneratorTemplateProperty;
			this.itemGeneratorTemplateSelectorProperty = itemGeneratorTemplateSelectorProperty;
			this.itemGeneratorStyleProperty = itemGeneratorStyleProperty;
			this.Source = (!(source is IList) && source != null) ? new EnumerableObservableWrapper<object>(source) : source as IList;
#if !SL
			dispatcher = Dispatcher.CurrentDispatcher;
#endif
		}
		TContainer Control { get; set; }
		Action<int, object> insertItemAction;
		Action<TItem, object> linkItemWithSourceAction;
		ISupportInitialize supportInitialize;
		bool lockSynchronization;
		IList source;
		IList Source {
			get { return source; }
			set {
				UnsubscribeSource();
				source = value;
				SubscribeSource();
			}
		}
		IList Target {
			get {
				if (Control == null)
					return null;
				return getTargetFunction(Control);
			}
		}
		void ConnectToContainer(TContainer d) {
			Control = d;
			Populate();
		}
		bool IsTemplateValid(DataTemplate template) {
			if (!useDefaultTemplateValidation)
				return true;
			Dictionary<int, Type> types = new Dictionary<int,Type>();
			if (template.VisualTree == null)
				types = reflectionHelper.GetPropertyValue<Dictionary<int, Type>>(template, "ChildTypeFromChildIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			else {
				var vtElement = template.VisualTree;
				int i = 1;
				bool skip = false;
				while (vtElement != null) {
					if (!skip) {
						types.Add(i++, vtElement.Type);
						if (vtElement.FirstChild != null) {
							vtElement = vtElement.FirstChild;
							continue;
						}
					}
					skip = false;
					if (vtElement.NextSibling != null) {
						vtElement = vtElement.NextSibling;
						continue;
					}
					vtElement = vtElement.Parent;
					skip = true;
				}
			}
			var count = types.Count;
			if (count < 1) return false;
			if (typeof(TItem).IsAssignableFrom(types[1]))
				return true;
			if (count < 2) return false;
			if (typeof(ContentControl).IsAssignableFrom(types[1]) || typeof(ContentPresenter).IsAssignableFrom(types[1]))
				if (typeof(TItem).IsAssignableFrom(types[2]))
					return true;
			return false;
		}
		protected virtual TItem CreateItem(object item) {
			Style style = itemGeneratorStyleProperty != null ? Control.GetValue(itemGeneratorStyleProperty) as Style : null;
			DataTemplate template;
			DataTemplateSelector templateSelector = itemGeneratorTemplateSelectorProperty != null ? Control.GetValue(itemGeneratorTemplateSelectorProperty) as DataTemplateSelector : null;
			if (templateSelector != null)
				template = templateSelector.SelectTemplate(item, Control);
			else
				template = itemGeneratorTemplateProperty != null ? Control.GetValue(itemGeneratorTemplateProperty) as DataTemplate : null;
			if (template == null && useDefaultTemplateSelector) {
				template = DefaultTemplateSelector.Instance.SelectTemplate(item, Control).If(IsTemplateValid);
			}
			if (template == null && style == null && !(item is TItem) && this.setDefaultBindingAction == null)
				return null;
			TItem loadedItem = TemplateHelper.LoadFromTemplate<TItem>(template);
			if(loadedItem == null) {
				if (item is TItem)
					loadedItem = item as TItem;
				else {
					loadedItem = createItemDelegate(Control);
					if (this.setDefaultBindingAction != null)
						this.setDefaultBindingAction(loadedItem);
				}
			}
			bool assignDataContextAndStyleProperties = loadedItem is FrameworkElement
#if !SL
				|| loadedItem is FrameworkContentElement;
#endif
				;
			if (assignDataContextAndStyleProperties) {
				ResetProperties rValue = ResetProperties.None;
				if (loadedItem != item) {
					rValue |= !Equals(item, loadedItem.GetValue(FrameworkElement.DataContextProperty)) ? ResetProperties.DataContext : ResetProperties.None;
					loadedItem.SetValue(FrameworkElement.DataContextProperty, item);
				}					
				if (style != null && loadedItem.ReadLocalValue(FrameworkElement.StyleProperty) == DependencyProperty.UnsetValue) {
					loadedItem.SetValue(FrameworkElement.StyleProperty, style);
					rValue |= ResetProperties.Style;
				}
				loadedItem.SetValue(ResetProperty, rValue);
			}
			if (linkItemWithSourceAction != null) {
				linkItemWithSourceAction(loadedItem, item);
			}
			DependencyObjectExtensions.SetDataContext(loadedItem, item);
			if (loadedItem != null)
				ItemsAttachedBehaviorProperties.SetSource(loadedItem, item);
			return loadedItem;
		}
		protected virtual void ClearItem(object item) {
			if (!(item is FrameworkElement || item is FrameworkContentElement))
				return;
			if (customClear != null && customClear(item as TItem))
				return;
			var dObj = ((DependencyObject)item);
			var rProps = (ResetProperties)dObj.GetValue(ResetProperty);
			if (rProps.HasFlag(ResetProperties.DataContext)) {
				dObj.ClearValue(FrameworkElement.DataContextProperty);
			}
			if (rProps.HasFlag(ResetProperties.Style)) {
				dObj.ClearValue(FrameworkElement.StyleProperty);
			}
		}
		void MoveSourceItem(int oldPosition, int newPosition) {
			if (Source == null)
				return;
			object item = Source[oldPosition];
			lockSynchronization = true;
			try {
				Source.RemoveAt(oldPosition);
				Source.Insert(newPosition, item);
			}
			finally {
				lockSynchronization = false;
			}
		}
		void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
#if !SL
			if(dispatcher.CheckAccess())
#endif
				OnSourceCollectionChangedCore(sender, e);
#if !SL
			else
				dispatcher.Invoke(new Action(() => OnSourceCollectionChangedCore(sender, e)));
#endif
		}
		void OnSourceCollectionChangedCore(object sender, NotifyCollectionChangedEventArgs e) {
			if (Control == null || lockSynchronization)
				return;
			lockSynchronization = true;
			SyncCollectionHelper.SyncCollection(e, Target, Source, CreateItem, insertItemAction, supportInitialize, ClearItem);
			lockSynchronization = false;
		}
		void Populate() {
			SyncCollectionHelper.PopulateCore(Target, Source, CreateItem, insertItemAction, supportInitialize, ClearItem);
		}
		void SubscribeSource() {
			INotifyCollectionChanged collection = Source as INotifyCollectionChanged;
			if(collection != null) {
				CollectionChangedEventManager.AddListener(collection, this);
			}
		}
		void UnsubscribeSource() {
			INotifyCollectionChanged collection = Source as INotifyCollectionChanged;
			if(collection != null) {
				CollectionChangedEventManager.RemoveListener(collection, this);
			}
		}
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(CollectionChangedEventManager)) {
				OnSourceCollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);
				return true;
			}
			return false;
		}
	}
}
namespace DevExpress.Xpf.Core.Native {
#if SL
	public class ResourceCollectionSyncronizer<T> where T : FrameworkElement {
		readonly T owner;
		readonly IList sourceCollection;
		readonly Func<T, bool> getAllowAddElementsToVisualTreeDelegate;
		readonly string holderName;
		ResourceDictionary Resources { get { return owner.Resources; } }
		public ResourceCollectionSyncronizer(T owner, IList sourceCollection, Func<T, bool> getAllowAddElementsToVisualTreeDelegate, string holderName) {
			this.owner = owner;
			this.sourceCollection = sourceCollection;
			this.getAllowAddElementsToVisualTreeDelegate = getAllowAddElementsToVisualTreeDelegate;
			this.holderName = holderName;
		}
		public IList Holder { get { return FrameworkElementExtensions.GetResourcesHolder(owner, holderName); } }
		void AddElementToHolder(IList holder, FrameworkElement element) {
			if(!string.IsNullOrEmpty(element.Name))
				foreach(FrameworkElement col in holder)
					if(col.Name == element.Name)
						throw new ArgumentException("Multiple items have the same Name. Please specify unique names.");
			holder.Add(element);
		}
		public void SynchronizeResources(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if(!getAllowAddElementsToVisualTreeDelegate(owner))
				return;
			IList holder = Holder;
			switch(e.Action) {
				case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					if(e.NewItems != null)
						foreach(FrameworkElement element in e.NewItems)
							AddElementToHolder(holder, element);
					if(e.OldItems != null)
						foreach(FrameworkElement element in e.OldItems)
							holder.Remove(element);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
					for(int i = holder.Count - 1; i >= 0; i--) {
						object element = holder[i];
						if(!sourceCollection.Contains(element))
							holder.Remove(element);
					}
					foreach(FrameworkElement element in sourceCollection)
						if(!holder.Contains(element))
							AddElementToHolder(holder, element);
					break;
			}
		}
		public void OnAllowAddElementToVisualTreeChanged() {
			IList holder = Holder;
			holder.Clear();
			if(getAllowAddElementsToVisualTreeDelegate(owner))
				foreach(FrameworkElement element in sourceCollection)
					holder.Add(element);
		}
	}
#endif
}
