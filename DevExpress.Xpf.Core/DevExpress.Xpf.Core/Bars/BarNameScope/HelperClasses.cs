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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Windows.Media;
using System.Linq;
using DevExpress.Xpf.Core;
using System.Reflection;
using System.Windows.Threading;
using System.ComponentModel;
using System.Globalization;
namespace DevExpress.Xpf.Bars.Native {
	abstract class DecoratorDataBase {
		public Type DecoratorType { get; private set; }
		public Type ServiceType { get; private set; }
		protected DecoratorDataBase(Type decoratorType, Type serviceType) {
			this.DecoratorType = decoratorType;
			this.ServiceType = serviceType;
		}
		public abstract IBarNameScopeDecorator CreateDecorator();
		public abstract object CreateService(object decorator);
		public abstract object GetNullService();
	}
	class DecoratorData<TDecorator> : DecoratorDataBase where TDecorator : IBarNameScopeDecorator {
		Func<TDecorator> CreateDecoratorCallback { get; set; }
		public DecoratorData(Func<TDecorator> createDecoratorCallback)
			: base(typeof(TDecorator), null) {
			this.CreateDecoratorCallback = createDecoratorCallback;
		}
		public override IBarNameScopeDecorator CreateDecorator() { return CreateDecoratorCallback(); }
		public override object CreateService(object decorator) { throw new NotSupportedException(); }
		public override object GetNullService() { throw new NotSupportedException(); }
	}
	class DecoratorData<TDecorator, TDecoratorService> : DecoratorDataBase where TDecorator : IBarNameScopeDecorator {
		Func<TDecorator> CreateDecoratorCallback { get; set; }
		Func<TDecorator, TDecoratorService> CreateServiceCallback { get; set; }
		Func<TDecoratorService> GetNullServiceCallback { get; set; }
		public DecoratorData(
			Func<TDecorator> createDecoratorCallback,
			Func<TDecorator, TDecoratorService> createServiceCallback,
			Func<TDecoratorService> getNullServiceCallback)
			: base(typeof(TDecorator), typeof(TDecoratorService)) {
			CreateDecoratorCallback = createDecoratorCallback;
			CreateServiceCallback = createServiceCallback;
			GetNullServiceCallback = getNullServiceCallback;
		}
		public override IBarNameScopeDecorator CreateDecorator() { return CreateDecoratorCallback(); }
		public override object CreateService(object decorator) { return CreateServiceCallback((TDecorator)decorator); }
		public override object GetNullService() { return GetNullServiceCallback(); }
	}
	public static class LogicalTreeWrapper {
		static readonly Action<Popup, UIElement, UIElement> updatePlacementTargetRegistration;
		static readonly Action<FrameworkElement> clearInheritanceContext;
		static readonly Action<DependencyObject, DependencyObject, DependencyObject> addOrRemoveHasLoadedChangeHandlerFlag;
		static readonly Action<DependencyObject, DependencyObject, DependencyProperty> addInheritanceContext;
		static LogicalTreeWrapper() {
			addInheritanceContext = ReflectionHelper.CreateInstanceMethodHandler<Action<DependencyObject, DependencyObject, DependencyProperty>>(
				null, 
				"AddInheritanceContext",
				BindingFlags.Instance | BindingFlags.NonPublic,
				typeof(DependencyObject)
				);
			clearInheritanceContext = ReflectionHelper.CreateInstanceMethodHandler<Action<FrameworkElement>>(
				null,
				"ClearInheritanceContext",
				BindingFlags.Instance | BindingFlags.NonPublic,
				typeof(FrameworkElement)
				);
			updatePlacementTargetRegistration = ReflectionHelper.CreateInstanceMethodHandler<Popup, Action<Popup, UIElement, UIElement>>(
				null,
				"UpdatePlacementTargetRegistration",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
				);
			var broadcastEventHelperType = typeof(FrameworkElement).Assembly.GetType("System.Windows.BroadcastEventHelper");
			addOrRemoveHasLoadedChangeHandlerFlag = ReflectionHelper.CreateInstanceMethodHandler<Action<DependencyObject, DependencyObject, DependencyObject>>(null, "AddOrRemoveHasLoadedChangeHandlerFlag", BindingFlags.NonPublic | BindingFlags.Static, broadcastEventHelperType);
		}
		public static void UpdatePlacementTargetRegistration(Popup popup, UIElement oldValue, UIElement newValue) {
			bool popupHasParent = LogicalTreeHelper.GetParent(popup) != null;
			updatePlacementTargetRegistration(popup, oldValue, newValue);
			if (!popupHasParent)
				AddOrRemoveHasLoadedChangeHandlerFlag(popup, oldValue, newValue);
		}
		public static void AddOrRemoveHasLoadedChangeHandlerFlag(DependencyObject d, DependencyObject oldParent, DependencyObject newParent) {
			if (oldParent != null && newParent != null) {
				addOrRemoveHasLoadedChangeHandlerFlag(d, oldParent, null);
				addOrRemoveHasLoadedChangeHandlerFlag(d, null, newParent);
			} else {
				addOrRemoveHasLoadedChangeHandlerFlag(d, oldParent, newParent);
			}
		}
		class NodeWrapper : Popup, ILogicalChildrenContainer {
			static NodeWrapper() {
				PlacementTargetProperty.OverrideMetadata(typeof(NodeWrapper), new FrameworkPropertyMetadata(null, (d, e) => ((NodeWrapper)d).OnPlacementTargetChanged(e)));
			}
			protected virtual void OnPlacementTargetChanged(DependencyPropertyChangedEventArgs e) {
				if (e.OldValue != null)
					clearInheritanceContext(this);
				if (e.NewValue != null)
					addInheritanceContext(this, e.NewValue as DependencyObject, VisualBrush.VisualProperty);
				UpdatePlacementTargetRegistration(this, e.OldValue as UIElement, e.NewValue as UIElement);
			}
			public NodeWrapper(UIElement root) {
				this.PlacementTarget = root;				
			}
			List<object> children = new List<object>();
			void ILogicalChildrenContainer.AddLogicalChild(object child) {
				children.Add(child);
				AddLogicalChild(child);
			}
			void ILogicalChildrenContainer.RemoveLogicalChild(object child) {
				children.Remove(child);
				RemoveLogicalChild(child);
				if (children.Count == 0)
					SetNodeWrapper(PlacementTarget, null);
			}
			protected override IEnumerator LogicalChildren {
				get {
					return children.GetEnumerator();
				}
			}
			public void ClearLogicalChildren() {
				while (children.Count > 0)
					RemoveLogicalChild(children.First());
			}
		}
		public static UIElement GetRoot(DependencyObject obj) { return (UIElement)obj.GetValue(RootProperty); }
		public static void SetRoot(DependencyObject obj, UIElement value) { obj.SetValue(RootProperty, value); }
		public static ILogicalChildrenContainer GetLogicalChildrenContainer(UIElement element) {
			return GetNodeWrapper(element);
		}
		public static void ClearChildren(UIElement root) {
			var wrapper = GetNodeWrapper(root);
			if (wrapper == null)
				return;
			wrapper.ClearLogicalChildren();
		}
		static NodeWrapper GetNodeWrapper(DependencyObject obj) { return (NodeWrapper)obj.GetValue(NodeWrapperProperty); }
		static void SetNodeWrapper(DependencyObject obj, NodeWrapper value) { obj.SetValue(NodeWrapperProperty, value); }
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty NodeWrapperProperty = DependencyPropertyManager.RegisterAttached("NodeWrapper", typeof(NodeWrapper), typeof(LogicalTreeWrapper), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnNodeWrapperPropertyChanged)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty RootProperty = DependencyPropertyManager.RegisterAttached("Root", typeof(UIElement), typeof(LogicalTreeWrapper), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnRootPropertyChanged)));
		static void OnRootPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var newValue = e.NewValue as UIElement;
			var oldValue = e.OldValue as UIElement;
			if (oldValue != null) {
				((ILogicalChildrenContainer)GetNodeWrapper(oldValue)).Do(x => x.RemoveLogicalChild(d));
			}
			NodeWrapper wrapper = newValue.With(GetNodeWrapper) ?? newValue.With(x => new NodeWrapper(x)).Do(x => SetNodeWrapper(newValue, x));
			if (wrapper == null)
				return;
			((ILogicalChildrenContainer)wrapper).AddLogicalChild(d);
		}
		static void OnNodeWrapperPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			NodeWrapper oldValue = e.OldValue as NodeWrapper;
			if (oldValue != null)
				oldValue.PlacementTarget = null;
		}
	}
	public static class TreeHelper {
		static Func<DependencyObject, List<System.Windows.Controls.Primitives.Popup>> getRegisteredPopups;
		static TreeHelper() {
			var rpf_ = typeof(System.Windows.Controls.Primitives.Popup).GetField("RegisteredPopupsField", BindingFlags.NonPublic | BindingFlags.Static);
			var rpf = rpf_.GetValue(null);
			var getRegisteredPopups_ = ReflectionHelper.CreateInstanceMethodHandler<Func<object, DependencyObject, List<System.Windows.Controls.Primitives.Popup>>>(rpf, "GetValue", BindingFlags.Public | BindingFlags.Instance, rpf_.FieldType);
			getRegisteredPopups = new Func<DependencyObject, List<System.Windows.Controls.Primitives.Popup>>((target) => getRegisteredPopups_(rpf, target));
		}
		static IEnumerable<DependencyObject> GetParentsIncludeSelf(DependencyObject element, bool logicalTreeFirst) {
			DependencyObject candidate;
			while (element != null) {
				yield return element;
				candidate = null;
				if (logicalTreeFirst) {
					candidate = LogicalTreeHelper.GetParent(element);
					if (candidate == null && element is Visual)
						candidate = VisualTreeHelper.GetParent((Visual)element);
					if (candidate == null && element is Popup)
						candidate = ((Popup)element).PlacementTarget;
				} else {
					if (element is Visual)
						candidate = VisualTreeHelper.GetParent((Visual)element);
					if (candidate == null)
						candidate = LogicalTreeHelper.GetParent(element);
					if (candidate == null && element is Popup)
						candidate = ((Popup)element).PlacementTarget;
				}
				element = candidate;				
			}
		}
		public static IEnumerable<DependencyObject> GetParents(DependencyObject element, bool includeSelf = true, bool logicalTreeFirst = true) {
			foreach (var parent in GetParentsIncludeSelf(element, logicalTreeFirst))
				if (parent == element && !includeSelf)
					continue;
				else
					yield return parent;
		}
		public static IEnumerable<TResult> GetParents<TResult>(DependencyObject element, bool includeSelf = true, bool logicalTreeFirst = true) {
			return GetParents(element, includeSelf, logicalTreeFirst).OfType<TResult>();
		}
		public static IEnumerable<DependencyObject> GetParents(DependencyObject element, Func<DependencyObject, bool> predicate, bool includeSelf = true, bool logicalTreeFirst = true) {
			return GetParents(element, includeSelf, logicalTreeFirst).Where(predicate);
		}
		public static IEnumerable<TResult> GetParents<TResult>(DependencyObject element, Func<TResult, bool> predicate, bool includeSelf = true, bool logicalTreeFirst = true) {
			return GetParents<TResult>(element, includeSelf, logicalTreeFirst).Where(predicate);
		}
		public static DependencyObject GetParent(DependencyObject element, Func<DependencyObject, bool> predicate, bool includeSelf = true, bool logicalTreeFirst = true) {
			return GetParents(element, predicate, includeSelf, logicalTreeFirst).FirstOrDefault();
		}
		public static TResult GetParent<TResult>(DependencyObject element, Func<TResult, bool> predicate = null, bool includeSelf = true, bool logicalTreeFirst = true) {
			return GetParents<TResult>(element, predicate ?? new Func<TResult, bool>(x => true), includeSelf, logicalTreeFirst).FirstOrDefault();
		}
		public static TResult GetChild<TResult>(DependencyObject root, Func<TResult, bool> predicate = null) where TResult : DependencyObject {
			return GetChildren(root, predicate).FirstOrDefault();
		}
		public static IEnumerable<TResult> GetChildren<TResult>(DependencyObject root, Func<TResult, bool> predicate = null) where TResult : DependencyObject {
			var result = EnumerateDescendantsImpl(root, new HashSet<DependencyObject>()).OfType<TResult>();
			if (predicate == null)
				return result;
			return result.Where(predicate);
		}
		static IEnumerable<DependencyObject> EnumerateDescendantsImpl(DependencyObject dObj, HashSet<DependencyObject> enumeratedElements) {
			yield return dObj;
			if (enumeratedElements.Contains(dObj))
				yield break;
			enumeratedElements.Add(dObj);
			foreach (var element in EnumerateChildren(dObj)) {
				foreach (var desc in EnumerateDescendantsImpl(element, enumeratedElements)) {
					yield return desc;
				}
			}
		}
		static IEnumerable<DependencyObject> EnumerateChildren(DependencyObject dObj) {
			if (dObj is Visual) {
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dObj); i++)
					yield return VisualTreeHelper.GetChild(dObj, i);
			}
			foreach (var child in LogicalTreeHelper.GetChildren(dObj)) {
				if (child is DependencyObject)
					yield return child as DependencyObject;
			}
			var registeredPopups = dObj is FrameworkElement ? getRegisteredPopups(dObj) : null;
			if (registeredPopups != null)
				foreach (var popup in registeredPopups)
					yield return popup;
		}
	}
	public class LockableValueStorage<TValue> {
		TValue currentValue;
		TValue lockedValue;
		bool hasLockedValue;
		readonly bool setLastOnUnlock;
		readonly Locker locker;
		public LockableValueStorage(bool setLastOnUnlock){
			this.locker = new Locker();
			this.hasLockedValue = false;
			this.setLastOnUnlock = setLastOnUnlock;
			this.currentValue = default(TValue);
			this.lockedValue = default(TValue);
			ValueChanged = new ValueChangedEventHandler<TValue>((o, a) => { });
			ValueChanging = new ValueChangedEventHandler<TValue>((o, a) => { });
			locker.Unlocked += OnUnlocked;			
		}
		public LockableValueStorage() : this(true) { }
		void OnUnlocked(object sender, EventArgs e) {
			if (!hasLockedValue)
				return;
			var lValue = lockedValue;
			lockedValue = default(TValue);
			hasLockedValue = false;
			SetValue(lValue);
		}
		public void SetValue(TValue value, bool silent = false) {
			if (locker.IsLocked) {
				if (setLastOnUnlock) {
					lockedValue = value;
					hasLockedValue = true;
				}				
				return;
			}
			if (Equals(currentValue, value))
				return;
			if (silent) {
				currentValue = value;
				return;
			}
			ValueChanging(this, new ValueChangedEventArgs<TValue>(currentValue, value));
			var oldValue = currentValue;
			currentValue = value;
			ValueChanged(this, new ValueChangedEventArgs<TValue>(oldValue, value));
		}
		public TValue GetValue() { return currentValue; }
		public IDisposable Lock() { return locker.Lock(); }
		public void Unlock() { locker.Unlock(); }
		public event ValueChangedEventHandler<TValue> ValueChanged;
		public event ValueChangedEventHandler<TValue> ValueChanging;
	}	
	public class ElementMergingBehaviorTypeConverter : EnumConverter {
		public ElementMergingBehaviorTypeConverter() : base(typeof(ElementMergingBehavior)) {
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(bool))
				return true;
			if (destinationType == typeof(bool?))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(bool))
				return true;
			if (sourceType == typeof(bool?))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			ElementMergingBehavior evalue = (ElementMergingBehavior)value;
			if (destinationType == typeof(bool))
				return evalue == ElementMergingBehavior.All || evalue == ElementMergingBehavior.InternalWithExternal;
			if (destinationType == typeof(bool?)) {
				switch (evalue) {
					case ElementMergingBehavior.Undefined:
						return null;
					case ElementMergingBehavior.InternalWithInternal:
					case ElementMergingBehavior.None:
						return false;
					case ElementMergingBehavior.All:
					case ElementMergingBehavior.InternalWithExternal:
						return true;
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (value is bool) {
				return (bool)value ? ElementMergingBehavior.InternalWithExternal : ElementMergingBehavior.None;
			}
			if (value is bool?) {
				return ((bool?)value).Return(x => x.Value ? ElementMergingBehavior.InternalWithExternal : ElementMergingBehavior.None, () => ElementMergingBehavior.InternalWithInternal);
			}
			if(value is string) {
				bool result;
				if (bool.TryParse((string)value, out result)) {
					return (bool)result ? ElementMergingBehavior.InternalWithExternal : ElementMergingBehavior.None;
				}				
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
namespace DevExpress.Xpf.Core.Internal {
	public static class ContextLayoutManagerHelper {
		static readonly object lObject = new object();
		static volatile Type clmType;
		static Type lelType;
		static Func<object, object> clmGetter;
		static Action<object> clmUpdateLayout;
		static Func<object, object, object> lelAdd;
		static Action<object, object> lelRemove;
		static Func<object, object> clmGetLel;		
		static object ContextLayoutManager { get { return GetLayoutManagerInstance(); } }
		static Action<Visual, Visual> propagateResumeLayout;
		static Action<Visual> propagateSuspendLayout;
		static Hashtable GetLayoutUpdatedListItems(DependencyObject obj) { return (Hashtable)obj.GetValue(LayoutUpdatedListItemsProperty); }
		static void SetLayoutUpdatedListItems(DependencyObject obj, Hashtable value) { obj.SetValue(LayoutUpdatedListItemsProperty, value); }
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty LayoutUpdatedListItemsProperty = DependencyPropertyManager.RegisterAttached("LayoutUpdatedListItems", typeof(Hashtable), typeof(ContextLayoutManagerHelper), new FrameworkPropertyMetadata(null));
		static ContextLayoutManagerHelper() {
			propagateSuspendLayout = ReflectionHelper.CreateInstanceMethodHandler<Action<Visual>>(null, "PropagateSuspendLayout", BindingFlags.NonPublic | BindingFlags.Static, typeof(UIElement));
			propagateResumeLayout = ReflectionHelper.CreateInstanceMethodHandler<Action<Visual, Visual>>(null, "PropagateResumeLayout", BindingFlags.NonPublic | BindingFlags.Static, typeof(UIElement));
		}
		static object GetLayoutManagerInstance() {
			if (clmType == null) {
				lock (lObject)
				{
					if (clmType == null) {
						var clmType2 = typeof(UIElement).Assembly.GetType("System.Windows.ContextLayoutManager");						
						clmGetter = ReflectionHelper.CreateInstanceMethodHandler<Func<object, object>>(null, "From", BindingFlags.Static | BindingFlags.NonPublic, clmType2);
						clmUpdateLayout = ReflectionHelper.CreateInstanceMethodHandler<Action<object>>(null, "UpdateLayout", BindingFlags.Instance | BindingFlags.NonPublic, clmType2);
						clmGetLel = ReflectionHelper.CreateInstanceMethodHandler<Func<object, object>>(null, "get_LayoutEvents", BindingFlags.Instance | BindingFlags.NonPublic, clmType2);
						lelType = typeof(UIElement).Assembly.GetType("System.Windows.LayoutEventList");
						lelAdd = ReflectionHelper.CreateInstanceMethodHandler<Func<object, object, object>>(null, "Add", BindingFlags.Instance | BindingFlags.NonPublic, lelType);
						lelRemove = ReflectionHelper.CreateInstanceMethodHandler<Action<object, object>>(null, "Remove", BindingFlags.Instance | BindingFlags.NonPublic, lelType);						
						clmType = clmType2;
					}
				}
			}
			return clmGetter(Dispatcher.CurrentDispatcher);
		}
		public static void UpdateLayout() {
			var lm = ContextLayoutManager;
			if (lm == null)
				return;
			clmUpdateLayout(lm);
		}
		public static void AddLayoutUpdatedHandler(EventHandler handler) {
			var lm = ContextLayoutManager;
			if (lm == null)
				return;
			var lel = clmGetLel(lm);
			lelAdd(lel, handler);
		}
		public static void AddLayoutUpdatedHandler(DependencyObject dObj, EventHandler handler) {
			if(dObj is UIElement) {
				((UIElement)dObj).LayoutUpdated += handler;
				return;
			}
			var lm = ContextLayoutManager;
			if (lm == null)
				return;
			var layoutEvents = clmGetLel(lm);
			var listItem = lelAdd(layoutEvents, handler);
			var hashTable = GetLayoutUpdatedListItems(dObj);
			if (hashTable == null) {
				hashTable = new Hashtable(2);
				SetLayoutUpdatedListItems(dObj, hashTable);
			}
			hashTable.Add(handler, listItem);
		}
		public static void RemoveLayoutUpdatedHandler(DependencyObject dObj, EventHandler handler) {
			if (dObj is UIElement) {
				((UIElement)dObj).LayoutUpdated -= handler;
				return;
			}
			var hashTable = GetLayoutUpdatedListItems(dObj);
			if (hashTable == null)
				return;
			var lm = ContextLayoutManager;
			if (lm == null)
				return;
			var layoutEvents = clmGetLel(lm);
			var listItem = hashTable[handler];
			if (listItem == null)
				return;
			hashTable.Remove(handler);
			lelRemove(layoutEvents, listItem);
		}
		class SuspendLayoutData : IDisposable {
			readonly UIElement visual;
			public SuspendLayoutData(UIElement visual) {
				this.visual = visual;
				propagateSuspendLayout(visual);
			}
			public void Dispose() {
				propagateResumeLayout(VisualTreeHelper.GetParent(visual) as UIElement, visual);
				visual.InvalidateMeasure();
				UpdateLayout();
			}
		}
		public static IDisposable SuspendLayout(UIElement v) {
			return new SuspendLayoutData(v);
		}
	}
}
