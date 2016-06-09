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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Markup;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using System.Collections;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Mvvm.UI.Interactivity;
using System.Windows.Data;
namespace DevExpress.Xpf.Bars {
	public enum CollectionActionKind { Insert, Remove, Replace }
	public interface ICollectionAction {
		CollectionActionKind Kind { get; }
		object Element { get; }
		IList Collection { get; }
		int Index { get; }
	}
	class CollectionActionWrapper : ICollectionAction {
		public CollectionActionWrapper(IBarManagerControllerAction action) : this(action, ((action as DependencyObject)).With(CollectionAction.GetContext)) { }
		public CollectionActionWrapper(IBarManagerControllerAction action, DependencyObject context) : this(action, action.Container, context) { }
		public CollectionActionWrapper(IBarManagerControllerAction action, IActionContainer controller, DependencyObject context) {
			var obj = action as DependencyObject;
			Kind = CollectionAction.GetKind(obj);
			var eSettings = ScopeSearchSettings.Local | ScopeSearchSettings.Ancestors;
			if (Kind == CollectionActionKind.Remove)
				eSettings = eSettings | ScopeSearchSettings.Descendants;
			Element = CollectionAction.GetElement(obj) ?? action.GetObject() ?? CollectionActionHelper.Instance.FindElementByName(context, CollectionAction.GetElementName(obj), controller, eSettings);
			Container = CollectionAction.GetContainer(obj) ?? CollectionActionHelper.Instance.FindElementByName(context, CollectionAction.GetContainerName(obj), controller, ScopeSearchSettings.Local | ScopeSearchSettings.Descendants) ?? CollectionActionHelper.Instance.GetDefaultContainer(Element, action, context);
			if (Element == null && Container != null && string.IsNullOrEmpty(CollectionAction.GetElementName(obj)))
				Element = context;
			if (Element != null && Container == null && string.IsNullOrEmpty(CollectionAction.GetElementName(obj)))
				Container = context;
			Collection = CollectionActionHelper.Instance.GetCollectionForElement(Container, Element, action);
			Index = CollectionAction.GetIndex(obj);
		}
		public DependencyObject Context { get; set; }
		public CollectionActionKind Kind { get; set; }
		public object Element { get; set; }
		public IList Collection { get; set; }
		public int Index { get; set; }
		public object Container { get; set; }
	}
	public class CollectionAction : BarManagerControllerActionBase, ICollectionAction {
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty KindProperty = DependencyPropertyManager.RegisterAttached("Kind", typeof(CollectionActionKind), typeof(CollectionAction), new FrameworkPropertyMetadata(CollectionActionKind.Insert, OnChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IndexProperty = DependencyPropertyManager.RegisterAttached("Index", typeof(int), typeof(CollectionAction), new FrameworkPropertyMetadata(-1, OnChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ContainerNameProperty = DependencyPropertyManager.RegisterAttached("ContainerName", typeof(string), typeof(CollectionAction), new FrameworkPropertyMetadata(null, OnChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ElementNameProperty = DependencyPropertyManager.RegisterAttached("ElementName", typeof(string), typeof(CollectionAction), new FrameworkPropertyMetadata(null, OnChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ContainerProperty = DependencyPropertyManager.RegisterAttached("Container", typeof(object), typeof(CollectionAction), new FrameworkPropertyMetadata(null, OnChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ElementProperty = DependencyPropertyManager.RegisterAttached("Element", typeof(object), typeof(CollectionAction), new FrameworkPropertyMetadata(null, OnChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty CollectionTagProperty = DependencyPropertyManager.RegisterAttached("CollectionTag", typeof(object), typeof(CollectionAction), new FrameworkPropertyMetadata(null, OnChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ContextProperty = DependencyPropertyManager.RegisterAttached("Context", typeof(DependencyObject), typeof(CollectionAction), new FrameworkPropertyMetadata(null, OnChanged));
		public static DependencyObject GetContext(DependencyObject obj) { return (DependencyObject)obj.GetValue(ContextProperty); }
		public static void SetContext(DependencyObject obj, DependencyObject value) { obj.SetValue(ContextProperty, value); }
		class ContextData : IDisposable {
			DependencyObject dObj;
			public ContextData(DependencyObject dObj, DependencyObject value) {
				this.dObj = dObj;
				SetContext(dObj, value);
			}
			void IDisposable.Dispose() {
				SetContext(dObj, null);
			}
		}
		public static IDisposable InitializeContext(DependencyObject obj, DependencyObject value) {
			return new ContextData(obj, value);
		}
		public static object GetCollectionTag(DependencyObject obj) { return obj.GetValue(CollectionTagProperty); }
		public static void SetCollectionTag(DependencyObject obj, object value) { obj.SetValue(CollectionTagProperty, value); }
		public static object GetElement(DependencyObject obj) { return (object)obj.GetValue(ElementProperty); }
		public static void SetElement(DependencyObject obj, object value) { obj.SetValue(ElementProperty, value); }
		public static object GetContainer(DependencyObject obj) { return (object)obj.GetValue(ContainerProperty); }
		public static void SetContainer(DependencyObject obj, object value) { obj.SetValue(ContainerProperty, value); }
		public static string GetElementName(DependencyObject obj) { return (string)obj.GetValue(ElementNameProperty); }
		public static void SetElementName(DependencyObject obj, string value) { obj.SetValue(ElementNameProperty, value); }
		public static string GetContainerName(DependencyObject obj) { return (string)obj.GetValue(ContainerNameProperty); }
		public static void SetContainerName(DependencyObject obj, string value) { obj.SetValue(ContainerNameProperty, value); }
		public static int GetIndex(DependencyObject obj) { return (int)obj.GetValue(IndexProperty); }
		public static void SetIndex(DependencyObject obj, int value) { obj.SetValue(IndexProperty, value); }
		public static CollectionActionKind GetKind(DependencyObject obj) { return (CollectionActionKind)obj.GetValue(KindProperty); }
		public static void SetKind(DependencyObject obj, CollectionActionKind value) { obj.SetValue(KindProperty, value); }
		static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var action = d as CollectionAction;
			if (action != null)
				action.OnChanged();
		}
		public object Element {
			get { return GetElement(this); }
			set { SetElement(this, value); }
		}
		public string ElementName {
			get { return GetElementName(this); }
			set { SetElementName(this, value); }
		}
		public new object Container {
			get { return GetContainer(this); }
			set { SetContainer(this, value); }
		}
		public string ContainerName {
			get { return GetContainerName(this); }
			set { SetContainerName(this, value); }
		}
		public int Index {
			get { return GetIndex(this); }
			set { SetIndex(this, value); }
		}
		public CollectionActionKind Kind {
			get { return GetKind(this); }
			set { SetKind(this, value); }
		}
		public object CollectionTag {
			get { return GetCollectionTag(this); }
			set { SetCollectionTag(this, value); }
		}
		void OnChanged() {
			wrapper = null;
		}
		CollectionActionWrapper wrapper;
		readonly Locker wrapperInitializationLocker = new Locker();
		CollectionActionWrapper Wrapper { get { return wrapper ?? (wrapper = CreateWrapper()); } }
		CollectionActionWrapper UnlockedWrapper {
			get {
				return wrapperInitializationLocker.IsLocked ? wrapper : Wrapper;
			}
		}
		CollectionActionWrapper CreateWrapper() {
			using (wrapperInitializationLocker.Lock())
				return new CollectionActionWrapper(this, GetContext(this));
		}
		CollectionActionKind ICollectionAction.Kind { get { return UnlockedWrapper.Return(x => x.Kind, () => CollectionActionKind.Insert); } }
		object ICollectionAction.Element { get { return UnlockedWrapper.Return(x => x.Element, () => null); } }
		IList ICollectionAction.Collection { get { return UnlockedWrapper.Return(x => x.Collection, () => null); } }
		int ICollectionAction.Index { get { return UnlockedWrapper.Return(x => x.Index, () => -1); } }
		public override object GetObjectCore() {
			return UnlockedWrapper.Return(x => x.Element, () => null);
		}
		protected override void ExecuteCore(DependencyObject context) {
			base.ExecuteCore(context);
			CollectionActionHelper.Execute((ICollectionAction)this);
		}
	}
	public class CollectionActionHelper {
		public static bool GetActionExecuted(DependencyObject obj) { return (bool)obj.GetValue(ActionExecutedProperty); }
		public static void SetActionExecuted(DependencyObject obj, bool value) { obj.SetValue(ActionExecutedProperty, value); }
		public static readonly DependencyProperty ActionExecutedProperty = DependencyProperty.RegisterAttached("ActionExecuted", typeof(bool), typeof(CollectionActionHelper), new PropertyMetadata(false));
		class CollectionGetterData {
			public Type ContainerType { get; private set; }
			public Type ElementType { get; private set; }
			public Func<IBarManagerControllerAction, object, IList> Getter { get; private set; }
			CollectionGetterData() { }
			public static CollectionGetterData Create<TContainer, TElement>(Func<IBarManagerControllerAction, object, IList> getter) {
				return new CollectionGetterData() { ContainerType = typeof(TContainer), ElementType = typeof(TElement), Getter = getter };
			}
		}
		class DefaultContainerGetterData {
			public Type ElementType { get; private set; }
			public Type ContainerType { get; private set; }
			public Func<IBarManagerControllerAction, object, DependencyObject, object> Getter { get; private set; }
			DefaultContainerGetterData() { }
			public static DefaultContainerGetterData Create<TElement, TContainer>(Func<IBarManagerControllerAction, object, DependencyObject, object> getter) {
				return new DefaultContainerGetterData() {
					ElementType = typeof(TElement),
					ContainerType = typeof(TContainer).If(x => x != typeof(object)).Return(x => x, () => typeof(void)),
					Getter = getter
				};
			}
		}
		class ParentGetterData {
			public Type ElementType { get;private set; }
			public Func<object, DependencyObject> Getter { get; private set; }
			public ParentGetterData(Type elementType, Func<object, DependencyObject> getter) {
				this.ElementType = elementType;
				this.Getter = getter;
			}
		}
		static CollectionActionHelper instance = new CollectionActionHelper();
		public static CollectionActionHelper Instance { get { return instance; } }
		readonly List<CollectionGetterData> collectionGetters;
		readonly List<DefaultContainerGetterData> containerGetters;
		readonly List<ParentGetterData> parentGetters;
		CollectionActionHelper() {
			collectionGetters = new List<CollectionGetterData>();
			containerGetters = new List<DefaultContainerGetterData>();
			parentGetters = new List<ParentGetterData>();
			RegisterParentGetter<IBarItem>(x => {
				DependencyObject parent = ((DependencyObject)x).With(LogicalTreeHelper.GetParent);
				var bParent = parent as Bar;
				if (bParent != null && bParent.Parent is ToolBarControlBase)
					parent = bParent.Parent;
				return parent;
			});
			RegisterDefaultContainerGetter<IBarItem, ToolBarControlBase>((action, _object, context) => context as ToolBarControlBase);
			RegisterDefaultContainerGetter<IBarItem, ILinksHolder>((action, _object, context) => context as ILinksHolder);
			RegisterDefaultContainerGetter<Bar>((action, bar) => action.GetRootContainer().GetBarManager());
			RegisterDefaultContainerGetter<BarItem>((action, bar) => (action as DependencyObject).With(CollectionAction.GetCollectionTag) != null ? null : action.GetRootContainer().GetBarManager());
			RegisterCollectionGetter<BarManager, Bar>((action, manager) => ((BarManager)manager).Bars);
			RegisterCollectionGetter<BarManager, BarItem>((action, manager) => ((BarManager)manager).Items);
			RegisterCollectionGetter<ILinksHolder, IBarItem>((action, holder) => ((ILinksHolder)holder).Items);
			RegisterCollectionGetter<ToolBarControlBase, IBarItem>((action, tbcontrol) => ((ToolBarControlBase)tbcontrol).Items);
		}
		static bool IsValidIndex(IList list, int index) {
			return index >= 0 && list.Count > index;
		}
		public static void Execute(IBarManagerControllerAction action) { Execute(new CollectionActionWrapper(action)); }
		public static void Execute(ICollectionAction action) {
			if (action.Collection == null) return;
			bool validIndex = IsValidIndex(action.Collection, action.Index);
			switch (action.Kind) {
				case CollectionActionKind.Insert:
					if (action.Element == null)
						return;
					if (validIndex)
						action.Collection.Insert(action.Index, action.Element);
					else
						action.Collection.Add(action.Element);
					return;
				case CollectionActionKind.Remove:
					if (!validIndex && (action.Element == null || !CanRemoveElement(action.Element)))
						return;
					if (action.Element != null)
						action.Collection.Remove(action.Element);
					else
						action.Collection.RemoveAt(action.Index);
					return;
				case CollectionActionKind.Replace:
					if (!validIndex || action.Element == null)
						return;
					action.Collection.RemoveAt(action.Index);
					action.Collection.Insert(action.Index, action.Element);
					return;
			}
		}
		public static bool CanRemoveElement(object element) {
			var dObj = element as DependencyObject;
			if (dObj == null)
				return true;
			var parentsToCheck = TreeHelper.GetParents<IControllerAction>(dObj, false);
			var isInActionContainer = parentsToCheck.Any(x => x is IActionContainer);
			if (!isInActionContainer || GetActionExecuted(dObj))
				return true;
			var parentActions = parentsToCheck.TakeWhile(x => !(x is IActionContainer));
			var rootParentExecuted = (parentActions.LastOrDefault() as DependencyObject).Return(GetActionExecuted, () => false);
			return rootParentExecuted;
		}
		public void RegisterParentGetter<TElement>(Func<TElement, DependencyObject> getter) {
			parentGetters.Add(new ParentGetterData(typeof(TElement), new Func<object, DependencyObject>(x => getter((TElement)x))));
		}
		public void RegisterCollectionGetter<TContainer, TElement>(Func<IBarManagerControllerAction, object, IList> getter) {
			collectionGetters.Add(CollectionGetterData.Create<TContainer, TElement>(getter));
		}
		public void RegisterDefaultContainerGetter<TElement>(Func<IBarManagerControllerAction, object, object> getter) {
			RegisterDefaultContainerGetter<TElement, object>((action, _object, context) => getter(action, _object));
		}
		public void RegisterDefaultContainerGetter<TElement, TContainer>(Func<IBarManagerControllerAction, object, DependencyObject, object> getter) {
			containerGetters.Add(DefaultContainerGetterData.Create<TElement, TContainer>(getter));
		}
		public object FindElementByName(DependencyObject context, string elementName, IActionContainer controller, ScopeSearchSettings searchSettings, Func<IFrameworkInputElement, bool> predicate = null) {
			return
				FindElementByNameCore(context, elementName, searchSettings, predicate) ??
				FindElementByNameCore(controller.With(x => controller.AssociatedObject), elementName, searchSettings, predicate) ??
				FindElementByNameCore(controller, elementName, searchSettings, predicate);
		}
		object FindElementByNameCore(object node, string elementName, ScopeSearchSettings searchSettings, Func<IFrameworkInputElement, bool> predicate) {
			if (predicate == null)
				return BarNameScope.GetService<IElementRegistratorService>(node).GetElements<IFrameworkInputElement>(elementName, searchSettings).FirstOrDefault();
			else
				return BarNameScope.GetService<IElementRegistratorService>(node).GetElements<IFrameworkInputElement>(elementName, searchSettings).FirstOrDefault(predicate);
		}
		DependencyObject GetParent(object obj) {
			if (obj == null)
				return null;
			var oType = obj.GetType();
			var getter = parentGetters.FirstOrDefault(x => x.ElementType.IsAssignableFrom(oType));
			if (getter == null)
				return null;
			return getter.Getter(obj);
		}
		public object GetDefaultContainer(object obj, IBarManagerControllerAction action, DependencyObject context) {
			var parent = GetParent(obj);
			object result = null;
			if (parent != null)
				result = GetDefaultContainerImpl(obj, action, parent);
			return result ?? GetDefaultContainerImpl(obj, action, context) ?? GetDefaultContainerImpl(obj, action, null);
		}
		object GetDefaultContainerImpl(object obj, IBarManagerControllerAction action, DependencyObject context) {
			if (obj == null)
				return null;
			var oType = obj.GetType();
			IEnumerable<DefaultContainerGetterData> contextFilteredGetters = containerGetters;
			if (context != null) {
				var cType = context.GetType();
				contextFilteredGetters = containerGetters.Where(x => x.ContainerType.IsAssignableFrom(cType));
			}
			var getters = contextFilteredGetters.Where(x => x.ElementType.IsAssignableFrom(oType));
			foreach (var getter in getters) {
				var result = getter.Getter(action, obj, context);
				if (result != null)
					return result;
			}
			return null;
		}
		public IList GetCollectionForElement<TContainer, TElement>(object container, object element, IBarManagerControllerAction action) {
			return GetCollectionForType(typeof(TContainer), typeof(TElement), container, action);
		}
		public IList GetCollectionForElement(object container, object element, IBarManagerControllerAction action) {
			var tType = container.With(x => x.GetType());
			var eType = element.With(x => x.GetType());
			return GetCollectionForType(tType, eType, container, action);
		}
		public IList GetCollectionForType(Type containerType, Type elementType, object container, IBarManagerControllerAction action) {
			if (containerType == null)
				return null;
			var getters = collectionGetters.Where(x => x.ContainerType.IsAssignableFrom(containerType) && (elementType == null || x.ElementType.IsAssignableFrom(elementType)));
			foreach (var getter in getters) {
				var result = getter.Getter(action, container);
				if (result != null)
					return result;
			}
			return null;
		}
		public static void SyncElementProperty(DependencyObject d, DependencyPropertyChangedEventArgs e) { SyncProperty(d, e, CollectionAction.ElementProperty); }
		public static void SyncElementNameProperty(DependencyObject d, DependencyPropertyChangedEventArgs e) { SyncProperty(d, e, CollectionAction.ElementNameProperty); }
		public static void SyncContainerProperty(DependencyObject d, DependencyPropertyChangedEventArgs e) { SyncProperty(d, e, CollectionAction.ContainerProperty); }
		public static void SyncContainerNameProperty(DependencyObject d, DependencyPropertyChangedEventArgs e) { SyncProperty(d, e, CollectionAction.ContainerNameProperty); }
		public static void SyncIndexProperty(DependencyObject d, DependencyPropertyChangedEventArgs e) { SyncProperty(d, e, CollectionAction.IndexProperty); }
		public static void SyncKindProperty(DependencyObject d, DependencyPropertyChangedEventArgs e) { SyncProperty(d, e, CollectionAction.KindProperty); }
		static void SyncProperty(DependencyObject d, DependencyPropertyChangedEventArgs e, DependencyProperty dependencyProperty) { d.SetValue(dependencyProperty, e.NewValue); }
	}
	[ContentProperty("Content")]
	public class InsertAction : CollectionAction {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty ContentProperty;		
		static InsertAction() {
			ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), typeof(InsertAction), new FrameworkPropertyMetadata(null));
			ElementProperty.OverrideMetadata(typeof(InsertAction), new FrameworkPropertyMetadata(new PropertyChangedCallback((d,e)=>((InsertAction)d).OnElementChanged(e))));
		}		
		public InsertAction() {
			base.Kind = CollectionActionKind.Insert;
			BindingOperations.SetBinding(this, ElementProperty, new Binding() { Path = new PropertyPath(ContentProperty), Source = this, Mode = BindingMode.TwoWay });
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new CollectionActionKind Kind { get { return CollectionActionKind.Insert; } set { } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object Content { get { return (object)GetValue(ContentProperty); } set { SetValue(ContentProperty, value); } }
		protected virtual void OnElementChanged(DependencyPropertyChangedEventArgs e) {
			var oDobj = e.OldValue as DependencyObject;
			var nDobj = e.NewValue as DependencyObject;
			if (oDobj != null && LogicalTreeHelper.GetParent(oDobj) == this)
				RemoveLogicalChild(oDobj);
			if (nDobj != null && LogicalTreeHelper.GetParent(nDobj) == null)
				AddLogicalChild(nDobj);
		}
		protected override IEnumerator LogicalChildren {
			get { return new SingleLogicalChildEnumerator(Content); }
		}
	}
	public class RemoveAction : CollectionAction {
		public RemoveAction() {
			base.Kind = CollectionActionKind.Remove;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new CollectionActionKind Kind { get { return CollectionActionKind.Remove; } set { } }
	}
	public class ReplaceAction : InsertAction {
		public ReplaceAction() {
			((CollectionAction)this).Kind = CollectionActionKind.Replace;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new CollectionActionKind Kind { get { return CollectionActionKind.Replace; } set { } }
	}
}
