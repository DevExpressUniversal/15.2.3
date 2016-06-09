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

namespace DevExpress.Utils.MVVM {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Linq;
	using System.Linq.Expressions;
	public interface ISupportItemsSource<TItem> : IComponent {
		Expression<Func<IEnumerable<TItem>>> Selector { get; }
	}
	public interface IItemsControl<TContainer> : ISupportItemsSource<TContainer> {
		bool IsContainerForItem(TContainer container, object item);
		TContainer CreateContainerForItem(object item);
		void PrepareContainerForItem(TContainer container, object item);
		void ClearContainerForItem(TContainer container, object item);
	}
	public static class ItemsSourceExtension {
		internal static Expression<Func<TTarget, IEnumerable<TItem>>> GetItemsSourceSelector<TTarget, TItem>(TTarget target) where TTarget : class, ISupportItemsSource<TItem> {
			return GetItemsSourceSelector<TTarget, TItem>(target, target.Selector);
		}
		internal static Expression<Func<TTarget, IEnumerable<TItem>>> GetItemsSourceSelector<TTarget, TItem>(TTarget target, LambdaExpression selector) {
			var t = Expression.Parameter(typeof(TTarget));
			return Expression.Lambda<Func<TTarget, IEnumerable<TItem>>>(Expression.MakeMemberAccess(t, ExpressionHelper.GetMember(selector)), t);
		}
		public static IDisposable SetItemsSourceBinding<TTarget, TItem, TViewModel, TModel>(
			this TTarget target, Expression<Func<TTarget, IEnumerable<TItem>>> targetSelector,
			System.Windows.Forms.Control container, Expression<Func<TViewModel, IEnumerable<TModel>>> collectionSelector,
			Func<TItem, TModel, bool> match, Func<TModel, TItem> create, Action<TItem, TModel> clear = null, Action<TItem, TModel> prepare = null)
			where TTarget : class, IComponent
			where TViewModel : class {
			return new ItemsSource<TTarget, TItem, TViewModel, TModel>(target, targetSelector, MVVMContext.FromControl(container), collectionSelector, match, create, clear, prepare);
		}
		public static IDisposable SetItemsSourceBinding<TTarget, TItem, TViewModel, TModel>(
			this TTarget target, Expression<Func<TTarget, IEnumerable<TItem>>> targetSelector,
			MVVMContext mvvmContext, Expression<Func<TViewModel, IEnumerable<TModel>>> collectionSelector,
			Func<TItem, TModel, bool> match, Func<TModel, TItem> create, Action<TItem, TModel> clear = null, Action<TItem, TModel> prepare = null)
			where TTarget : class, IComponent
			where TViewModel : class {
			return new ItemsSource<TTarget, TItem, TViewModel, TModel>(target, targetSelector, mvvmContext, collectionSelector, match, create, clear, prepare);
		}
#if DEBUGTEST
		internal
#endif
		sealed class ItemsSource<TTarget, TItem, TViewModel, TModel> : IDisposable
			where TTarget : class, IComponent
			where TViewModel : class {
			IDisposable triggerRef;
			TTarget target;
			MVVMContext mvvmContext;
			Func<TTarget, IEnumerable<TItem>> itemsAccessor;
			Func<TItem, TModel, bool> match;
			Func<TModel, TItem> create;
			Action<TItem, TModel> clear;
			Action<TItem, TModel> prepare;
			public ItemsSource(TTarget target,
				Expression<Func<TTarget, IEnumerable<TItem>>> itemsSelector,
				MVVMContext mvvmContext, Expression<Func<TViewModel, IEnumerable<TModel>>> collectionSelector,
				Func<TItem, TModel, bool> match, Func<TModel, TItem> create, Action<TItem, TModel> clear, Action<TItem, TModel> prepare) {
				if(mvvmContext != null && target != null) {
					this.match = match;
					this.create = create;
					this.clear = clear ?? ((item, modelItem) =>
					{
						IDisposable disposable = item as IDisposable;
						if(disposable != null) disposable.Dispose();
					});
					this.prepare = prepare ?? ((item, modelItem) => { });
					this.target = target;
					this.mvvmContext = mvvmContext;
					this.mvvmContext.Disposed += mvvmContext_Disposed;
					this.target.Disposed += target_Disposed;
					TViewModel viewModel = mvvmContext.GetViewModel<TViewModel>();
					ITriggerAction triggerAction;
					triggerRef = this.mvvmContext.Register(
						BindingHelper.SetNPCTrigger<TViewModel, IEnumerable<TModel>>(viewModel,
						collectionSelector, SetCollection,
						out triggerAction));
					itemsAccessor = CreateItemsAccessor(itemsSelector);
					UpdateTarget(collectionSelector, triggerAction);
				}
			}
#if DEBUGTEST
			internal ItemsSource(TTarget target,
				Expression<Func<TTarget, IEnumerable<TItem>>> itemsSelector,
				TViewModel viewModel, Expression<Func<TViewModel, IEnumerable<TModel>>> collectionSelector,
				Func<TItem, TModel, bool> match, Func<TModel, TItem> create, Action<TItem, TModel> clear, Action<TItem, TModel> prepare) {
				if(viewModel != null && target != null) {
					this.match = match;
					this.create = create;
					this.clear = clear ?? ((item, modelItem) =>
					{
						IDisposable disposable = item as IDisposable;
						if(disposable != null) disposable.Dispose();
					});
					this.prepare = prepare ?? ((item, modelItem) => { });
					this.target = target;
					this.target.Disposed += target_Disposed;
					ITriggerAction triggerAction;
					triggerRef = BindingHelper.SetNPCTrigger<TViewModel, IEnumerable<TModel>>(viewModel,
						collectionSelector, SetCollection,
						out triggerAction);
					itemsAccessor = CreateItemsAccessor(itemsSelector);
					UpdateTarget(collectionSelector, triggerAction);
				}
			}
#endif
			Func<TTarget, IEnumerable<TItem>> CreateItemsAccessor(Expression<Func<TTarget, IEnumerable<TItem>>> itemsSelector) {
				return GetItemsSourceSelector<TTarget, TItem>(target, itemsSelector).Compile();
			}
			IEnumerable<TModel> collection;
			List<TModel> modelItems;
			void SetCollection(IEnumerable<TModel> value) {
				if(collection != value) {
					UnsubscribeCollectionChanged(collection);
					OnCollectionChanged(collection, value);
					collection = value;
					SubscribeCollectionChanged(collection);
				}
				else OnUpdateItems((collection ?? empty).ToArray());
			}
			bool HasItemChangedSubscriptionViaBindingList {
				get { return (collection is IBindingList) && typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(TModel)); }
			}
			void SubscribeItemChanged(TModel modelItem) {
				if(HasItemChangedSubscriptionViaBindingList) return;
				INotifyPropertyChanged inpc = modelItem as INotifyPropertyChanged;
				if(inpc != null) inpc.PropertyChanged += modelItem_PropertyChanged;
			}
			void UnsubscribeItemChanged(TModel modelItem) {
				if(HasItemChangedSubscriptionViaBindingList) return;
				INotifyPropertyChanged inpc = modelItem as INotifyPropertyChanged;
				if(inpc != null) inpc.PropertyChanged -= modelItem_PropertyChanged;
			}
			void SubscribeCollectionChanged(IEnumerable<TModel> collection) {
				var notifyCollection = collection as INotifyCollectionChanged;
				if(notifyCollection != null)
					notifyCollection.CollectionChanged += collection_CollectionChanged;
				var bindingList = collection as IBindingList;
				if(bindingList != null)
					bindingList.ListChanged += bindingList_ListChanged;
			}
			void UnsubscribeCollectionChanged(IEnumerable<TModel> collection) {
				var notifyCollection = collection as INotifyCollectionChanged;
				if(notifyCollection != null)
					notifyCollection.CollectionChanged -= collection_CollectionChanged;
				var bindingList = collection as IBindingList;
				if(bindingList != null)
					bindingList.ListChanged -= bindingList_ListChanged;
			}
			void bindingList_ListChanged(object sender, ListChangedEventArgs e) {
				var array = (sender as IEnumerable<TModel>).ToArray();
				switch(e.ListChangedType) {
					case ListChangedType.ItemAdded:
						OnAddItems(new TModel[] { array[e.NewIndex] });
						break;
					case ListChangedType.ItemDeleted:
						OnRemoveItems(new TModel[] { modelItems[e.NewIndex] });
						break;
					case ListChangedType.ItemChanged:
						OnUpdateItems(new TModel[] { array[e.NewIndex] });
						break;
					case ListChangedType.Reset:
						OnUpdateItems(array);
						break;
					case ListChangedType.ItemMoved:
						int newIndex = e.NewIndex;
						TModel item = modelItems[e.OldIndex];
						modelItems.RemoveAt(e.OldIndex);
						if(newIndex > e.OldIndex)
							newIndex -= 1;
						modelItems.Insert(newIndex, item);
						break;
				}
			}
			void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
				switch(e.Action) {
					case NotifyCollectionChangedAction.Add:
						OnAddItems(e.NewItems.OfType<TModel>().ToArray());
						break;
					case NotifyCollectionChangedAction.Remove:
						OnRemoveItems(e.OldItems.OfType<TModel>().ToArray());
						break;
					case NotifyCollectionChangedAction.Reset:
						var collection = sender as IEnumerable<TModel>;
						OnUpdateItems(collection.ToArray());
						break;
					case NotifyCollectionChangedAction.Replace:
						OnRemoveItems(e.OldItems.OfType<TModel>().ToArray());
						OnAddItems(e.NewItems.OfType<TModel>().ToArray());
						break;
					case NotifyCollectionChangedAction.Move:
						int newIndex = e.NewStartingIndex;
						List<TModel> items = modelItems
							.Skip(e.OldStartingIndex).Take(e.OldItems.Count).ToList();
						modelItems.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
						if(newIndex > e.OldStartingIndex)
							newIndex -= e.OldItems.Count;
						modelItems.InsertRange(newIndex, items);
						break;
				}
			}
			void modelItem_PropertyChanged(object sender, PropertyChangedEventArgs e) {
				OnUpdateItems(new TModel[] { (TModel)sender });
			}
			static IEnumerable<TModel> empty = new TModel[0];
			void OnCollectionChanged(IEnumerable<TModel> prevValue, IEnumerable<TModel> value) {
				modelItems = new List<TModel>();
				OnRemoveItems((prevValue ?? empty).Except(value ?? empty).ToArray());
				OnAddItems((value ?? empty).Except(prevValue ?? empty).ToArray());
			}
			void OnAddItems(TModel[] collection) {
				if(!CheckItemsAccessible(collection)) return;
				var items = itemsAccessor(target);
				for(int i = 0; i < collection.Length; i++)
					AddItem(items, create(collection[i]), collection[i]);
			}
			void OnRemoveItems(TModel[] collection) {
				if(!CheckItemsAccessible(collection)) return;
				var items = itemsAccessor(target);
				for(int i = 0; i < collection.Length; i++) {
					TItem item = items.FirstOrDefault(e => match(e, collection[i]));
					if(item != null) RemoveItem(items, item, collection[i]);
				}
			}
			void OnUpdateItems(TModel[] collection) {
				if(!CheckItemsAccessible(collection)) return;
				var items = itemsAccessor(target);
				for(int i = 0; i < collection.Length; i++) {
					TItem item = items.FirstOrDefault(e => match(e, collection[i]));
					if(item != null) prepare(item, collection[i]);
				}
			}
			bool CheckItemsAccessible(TModel[] collection) {
				return (collection != null) && (collection.Length > 0) && (target != null);
			}
			void AddItem(IEnumerable<TItem> items, TItem item, TModel modelItem) {
				if(item == null) return;
				modelItems.Add(modelItem);
				SubscribeItemChanged(modelItem);
				IList<TItem> genericList = items as IList<TItem>;
				if(genericList != null)
					genericList.Add(item);
				else {
					ICollection<TItem> genericCollection = items as ICollection<TItem>;
					if(genericCollection != null)
						genericCollection.Add(item);
					else {
						IList list = items as IList;
						if(list != null)
							list.Add(item);
					}
				}
				prepare(item, modelItem);
			}
			void RemoveItem(IEnumerable<TItem> items, TItem item, TModel modelItem) {
				if(item == null) return;
				modelItems.Remove(modelItem);
				UnsubscribeItemChanged(modelItem);
				IList<TItem> genericList = items as IList<TItem>;
				if(genericList != null)
					genericList.Remove(item);
				else {
					ICollection<TItem> genericCollection = items as ICollection<TItem>;
					if(genericCollection != null)
						genericCollection.Remove(item);
					else {
						IList list = items as IList;
						if(list != null)
							list.Remove(item);
					}
				}
				clear(item, modelItem);
			}
			void UpdateTarget(Expression<Func<TViewModel, IEnumerable<TModel>>> collectionSelector, ITriggerAction triggerAction) {
				INotifyPropertyChangedTrigger trigger = BindingHelper.GetNPCTrigger(triggerRef);
				trigger.Execute(ExpressionHelper.GetPropertyName(collectionSelector), triggerAction);
			}
			void IDisposable.Dispose() {
				OnDisposing();
				GC.SuppressFinalize(this);
			}
			void OnDisposing() {
				if(target != null)
					target.Disposed -= target_Disposed;
				if(mvvmContext != null)
					mvvmContext.Disposed -= mvvmContext_Disposed;
				SetCollection(null);
				Ref.Dispose(ref triggerRef);
				this.mvvmContext = null;
				this.target = null;
				this.prepare = null;
				this.match = null;
				this.create = null;
				this.clear = null;
			}
			void mvvmContext_Disposed(object sender, EventArgs e) {
				((IDisposable)this).Dispose();
			}
			void target_Disposed(object sender, EventArgs e) {
				((IDisposable)this).Dispose();
			}
		}
	}
}
#if DEBUGTEST
namespace DevExpress.Utils.MVVM.Tests {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using NUnit.Framework;
	#region TestClasses
	class NPCBase : INotifyPropertyChanged {
		PropertyChangedEventHandler pc;
		internal int pcCount;
		protected void RaisePropertyChanged(string propertyName) {
			if(pc != null) pc(this, new PropertyChangedEventArgs(propertyName));
		}
		public event PropertyChangedEventHandler PropertyChanged {
			add { pc += value; pcCount++; }
			remove { pc -= value; pcCount--; }
		}
	}
	interface ITestEntity {
		string Name { get; set; }
		int GetPCCount();
	}
	abstract class TestEntitiesViewModel_Base<TEntity> : NPCBase where TEntity : ITestEntity {
		Func<string, TEntity> create;
		protected TestEntitiesViewModel_Base(Func<string, TEntity> create) {
			this.create = create;
			entitiesCore = CreateEntities();
		}
		protected abstract ICollection<TEntity> CreateEntities();
		ICollection<TEntity> entitiesCore;
		public ICollection<TEntity> Entities {
			get { return entitiesCore; }
			set {
				if(entitiesCore == value) return;
				entitiesCore = value;
				RaisePropertyChanged("Entities");
			}
		}
		internal TEntity Add() {
			var last = Entities.LastOrDefault();
			string c = (last != null) ? char.ConvertFromUtf32(char.ConvertToUtf32(last.Name, 0) + 1) : "A";
			var entity = create(c + gen.ToString());
			Entities.Add(entity);
			return entity;
		}
		internal void Remove() {
			if(Entities.Count > 0)
				Entities.Remove(Entities.Last());
		}
		internal int gen = 0;
		internal void Update() {
			gen++;
			foreach(TEntity entity in Entities)
				entity.Name = entity.Name.Substring(0, 1) + gen.ToString();
		}
	}
	class TestEntitiesViewModel_BindingList<TEntity> : TestEntitiesViewModel_Base<TEntity>
		where TEntity : ITestEntity {
		public TestEntitiesViewModel_BindingList(Func<string, TEntity> create)
			: base(create) {
		}
		protected override ICollection<TEntity> CreateEntities() {
			return new BindingList<TEntity>();
		}
	}
	class TestEntitiesViewModel_ObservableCollection<TEntity> : TestEntitiesViewModel_Base<TEntity>
		where TEntity : ITestEntity {
		public TestEntitiesViewModel_ObservableCollection(Func<string, TEntity> create)
			: base(create) {
		}
		protected override ICollection<TEntity> CreateEntities() {
			return new System.Collections.ObjectModel.ObservableCollection<TEntity>();
		}
	}
	class TestEntity_NPC : NPCBase, ITestEntity {
		string name;
		public string Name {
			get { return name; }
			set {
				if(name == value) return;
				name = value;
				RaisePropertyChanged("Name");
			}
		}
		public static TestEntity_NPC Create(string name) {
			return new TestEntity_NPC() { Name = name };
		}
		int ITestEntity.GetPCCount() {
			return pcCount;
		}
	}
	abstract class TestEntityBase : ITestEntity {
		string name;
		public string Name {
			get { return name; }
			set {
				if(name == value) return;
				name = value;
				OnNameChanged();
			}
		}
		public abstract int GetPCCount();
		protected abstract void OnNameChanged();
	}
	class TestEntity_NPC_Implicit : TestEntityBase, INotifyPropertyChanged {
		public override int GetPCCount() {
			return pcCount;
		}
		protected override void OnNameChanged() {
			RaisePropertyChanged("Name");
		}
		PropertyChangedEventHandler pc;
		int pcCount;
		void RaisePropertyChanged(string propertyName) {
			if(pc != null)
				pc(this, new PropertyChangedEventArgs(propertyName));
		}
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { pc += value; pcCount++; }
			remove { pc -= value; pcCount--; }
		}
		public static TestEntityBase Create(string name) {
			return new TestEntity_NPC_Implicit() { Name = name };
		}
	}
	class TestItemsControl : System.Windows.Forms.Control {
		TestItemsControlItemCollection itemsCore;
		public TestItemsControl() {
			itemsCore = new TestItemsControlItemCollection();
		}
		public TestItemsControlItemCollection Items {
			get { return itemsCore; }
		}
	}
	class TestItemsControlItem : IDisposable {
		public object Tag { get; set; }
		public string Text { get; set; }
		void IDisposable.Dispose() {
			IsDisposed = true;
			GC.SuppressFinalize(this);
		}
		internal bool IsDisposed;
	}
	class TestItemsControlItemCollection : DXCollectionBase<TestItemsControlItem> {
		public TestItemsControlItem this[int index] {
			get { return List[index]; }
		}
	}
	#endregion TestClasses
	[TestFixture]
	public class ItemsSourceExtensionTests {
		[Test]
		public void Test00_BindingList_NPC() {
			var viewTarget = new TestItemsControl();
			var viewModel = new TestEntitiesViewModel_BindingList<TestEntity_NPC>(TestEntity_NPC.Create);
			using(var source = new ItemsSourceExtension.ItemsSource<TestItemsControl, TestItemsControlItem, TestEntitiesViewModel_BindingList<TestEntity_NPC>, TestEntity_NPC>(
				viewTarget, t => t.Items, viewModel, x => x.Entities,
				(item, entity) => item.Tag == entity,
				(entity) => new TestItemsControlItem() { Tag = entity },
				null,
				(item, entity) => item.Text = entity.Name)) {
				DoTest_AddUpdateRemove<TestEntity_NPC>(viewTarget, viewModel);
				DoTest_AddRemoveMultiple<TestEntity_NPC>(viewTarget, viewModel);
			}
		}
		[Test]
		public void Test00_BindingList_NPC_Implicit() {
			var viewTarget = new TestItemsControl();
			var viewModel = new TestEntitiesViewModel_BindingList<TestEntityBase>(TestEntity_NPC_Implicit.Create);
			using(var source = new ItemsSourceExtension.ItemsSource<TestItemsControl, TestItemsControlItem, TestEntitiesViewModel_BindingList<TestEntityBase>, TestEntityBase>(
				viewTarget, t => t.Items, viewModel, x => x.Entities,
				(item, entity) => item.Tag == entity,
				(entity) => new TestItemsControlItem() { Tag = entity },
				null,
				(item, entity) => item.Text = entity.Name)) {
				DoTest_AddUpdateRemove<TestEntityBase>(viewTarget, viewModel);
				DoTest_AddRemoveMultiple<TestEntityBase>(viewTarget, viewModel);
			}
		}
		[Test]
		public void Test01_ObservableCollection_NPC() {
			var viewTarget = new TestItemsControl();
			var viewModel = new TestEntitiesViewModel_ObservableCollection<TestEntity_NPC>(TestEntity_NPC.Create);
			using(var source = new ItemsSourceExtension.ItemsSource<TestItemsControl, TestItemsControlItem, TestEntitiesViewModel_ObservableCollection<TestEntity_NPC>, TestEntity_NPC>(
				viewTarget, t => t.Items, viewModel, x => x.Entities,
				(item, entity) => item.Tag == entity,
				(entity) => new TestItemsControlItem() { Tag = entity },
				null,
				(item, entity) => item.Text = entity.Name)) {
				DoTest_AddUpdateRemove<TestEntity_NPC>(viewTarget, viewModel);
				DoTest_AddRemoveMultiple<TestEntity_NPC>(viewTarget, viewModel);
			}
		}
		[Test]
		public void Test01_ObservableCollection_NPC_Implicit() {
			var viewTarget = new TestItemsControl();
			var viewModel = new TestEntitiesViewModel_ObservableCollection<TestEntityBase>(TestEntity_NPC_Implicit.Create);
			using(var source = new ItemsSourceExtension.ItemsSource<TestItemsControl, TestItemsControlItem, TestEntitiesViewModel_ObservableCollection<TestEntityBase>, TestEntityBase>(
				viewTarget, t => t.Items, viewModel, x => x.Entities,
				(item, entity) => item.Tag == entity,
				(entity) => new TestItemsControlItem() { Tag = entity },
				null,
				(item, entity) => item.Text = entity.Name)) {
				DoTest_AddUpdateRemove<TestEntityBase>(viewTarget, viewModel);
				DoTest_AddRemoveMultiple<TestEntityBase>(viewTarget, viewModel);
			}
		}
		static void DoTest_AddUpdateRemove<TEntity>(TestItemsControl viewTarget, TestEntitiesViewModel_Base<TEntity> viewModel)
			where TEntity : ITestEntity {
			viewModel.gen = 0;
			Assert.AreEqual(0, viewTarget.Items.Count);
			var newEntity = viewModel.Add();
			Assert.AreEqual(1, viewTarget.Items.Count);
			Assert.AreEqual(1, newEntity.GetPCCount());
			var newItem = viewTarget.Items[0];
			Assert.AreEqual("A0", newItem.Text);
			Assert.AreEqual(viewModel.Entities.ElementAt(0), newItem.Tag);
			viewModel.Update();
			Assert.AreEqual(1, newEntity.GetPCCount());
			Assert.AreEqual(1, viewTarget.Items.Count);
			Assert.AreEqual(newItem, viewTarget.Items[0]);
			Assert.AreEqual("A1", newItem.Text);
			viewModel.Remove();
			Assert.AreEqual(0, viewTarget.Items.Count);
			Assert.AreEqual(0, newEntity.GetPCCount());
			Assert.IsTrue(newItem.IsDisposed);
		}
		static void DoTest_AddRemoveMultiple<TEntity>(TestItemsControl viewTarget, TestEntitiesViewModel_Base<TEntity> viewModel)
			where TEntity : ITestEntity {
			viewModel.gen = 0;
			Assert.AreEqual(0, viewTarget.Items.Count);
			var modelItem0 = viewModel.Add();
			var modelItem1 = viewModel.Add();
			var modelItem2 = viewModel.Add();
			Assert.AreEqual(3, viewTarget.Items.Count);
			var item0 = viewTarget.Items[0];
			var item1 = viewTarget.Items[1];
			var item2 = viewTarget.Items[2];
			Assert.AreEqual("A0", item0.Text);
			Assert.AreEqual(modelItem0, item0.Tag);
			Assert.AreEqual("B0", item1.Text);
			Assert.AreEqual(modelItem1, item1.Tag);
			Assert.AreEqual("C0", item2.Text);
			Assert.AreEqual(modelItem2, item2.Tag);
			viewModel.Entities.Remove(modelItem1);
			Assert.AreEqual(2, viewTarget.Items.Count);
			Assert.AreEqual(0, modelItem1.GetPCCount());
			Assert.IsTrue(item1.IsDisposed);
			Assert.AreEqual(0, viewTarget.Items.IndexOf(item0));
			Assert.AreEqual(1, viewTarget.Items.IndexOf(item2));
			viewModel.Entities.Remove(modelItem0);
			viewModel.Entities.Remove(modelItem2);
			Assert.AreEqual(0, viewTarget.Items.Count);
			Assert.AreEqual(0, modelItem0.GetPCCount());
			Assert.AreEqual(0, modelItem2.GetPCCount());
			Assert.IsTrue(item0.IsDisposed);
			Assert.IsTrue(item2.IsDisposed);
		}
	}
}
#endif
