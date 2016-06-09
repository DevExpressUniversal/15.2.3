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
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
namespace DevExpress.Xpf.Bars {
	public class BarNameScopeTreeWalker {		
		[ThreadStatic]
		static Locker updateScopeLocker;
		[ThreadStatic]
		static List<DependencyObject> changedElements;
		[ThreadStatic]
		static Queue<List<DependencyObject>> processQueue;
		[ThreadStatic]
		static Queue<Action> afterUpdateQueue;
		static Locker UpdateScopeLocker { get { return updateScopeLocker ?? (updateScopeLocker = new Locker().Do(x => x.Unlocked += OnUpdateScopeLockerUnlocked)); } }		
		static List<DependencyObject> ChangedElements { get { return changedElements ?? (changedElements = new List<DependencyObject>()); } }
		static Queue<List<DependencyObject>> ProcessQueue { get { return processQueue ?? (processQueue = new Queue<List<DependencyObject>>()); } }
		static Queue<Action> AfterUpdateQueue { get { return afterUpdateQueue ?? (afterUpdateQueue = new Queue<Action>()); } }
		public static readonly DependencyProperty WalkerProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty PulseProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty InheritancePulseProperty;
		public static BarNameScopeTreeWalker GetWalker(DependencyObject obj) { return (BarNameScopeTreeWalker)obj.GetValue(WalkerProperty); }
		public static void SetWalker(DependencyObject obj, BarNameScopeTreeWalker value) { UpdateScopeLocker.DoLockedAction(() => obj.SetValue(WalkerProperty, value)); ProcessChangedElements(); }
		static BarNameScopeTreeWalker() {
			InheritancePulseProperty = DependencyPropertyManager.RegisterAttached("InheritancePulse", typeof(bool), typeof(BarNameScopeTreeWalker), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));
			PulseProperty = DependencyPropertyManager.RegisterAttached("Pulse", typeof(object), typeof(BarNameScopeTreeWalker), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior, new PropertyChangedCallback(OnPulsePropertyChanged)));
			WalkerProperty = DependencyPropertyManager.RegisterAttached("Walker", typeof(BarNameScopeTreeWalker), typeof(BarNameScopeTreeWalker), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior, OnWalkerPropertyChanged));			
		}		
		protected static void OnWalkerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ActOnWalkerChange(d);
		}
		protected static void OnPulsePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (e.NewValue.ReturnSuccess())
				ActOnWalkerChange(d);
		}
		static void ActOnWalkerChange(object element){
			var ns = element as DependencyObject;
			if (!(ns is IBarNameScopeSupport || BarNameScope.IsScopeTarget(ns)) || ChangedElements.Contains(ns))
				return;
			ChangedElements.Add(ns);
			ProcessChangedElements();
		}
		public static void Pulse(object d) {
			var dObj = d as DependencyObject;
			if (Equals(null, dObj))
				return;
			DependencyObject inheritanceRoot = null;
			if (GetWalker(dObj)==null && (inheritanceRoot = GetInheritanceRoot(dObj)).With(BarNameScope.GetScope) == null) {
				EnsureWalker(dObj, inheritanceRoot);
			}
			UpdateScopeLocker.DoLockedAction(() => { dObj.SetValue(PulseProperty, new object()); dObj.SetValue(PulseProperty, null); });
			ProcessChangedElements();
		}		
		static void ProcessChangedElements() {						
			UpdateScopeLocker.DoLockedActionIfNotLocked(ProcessChangedElementsLocked);
		}
		static void ProcessChangedElementsLocked() {
			ProcessQueue.Enqueue(ChangedElements.ToList());
			ChangedElements.Clear();
			while (ProcessQueue.Count>0) {
				var collection = ProcessQueue.Dequeue();
				for (int i = 0; i < collection.Count; i++) {
					DependencyObject element = collection[i];
					if (GetWalker(element) == null)
						BarNameScope.SetScope(element, null);
					BarNameScope oldScope = BarNameScope.GetScope(element);
					BarNameScope parentScope = BarNameScope.FindScopeTarget(element, false).With(BarNameScope.GetScope);
					if (BarNameScope.IsScopeTarget(element)) {
						if (oldScope == null || oldScope.Parent != parentScope)
							BarNameScope.CreateScope(element, parentScope);
					} else {
						BarNameScope.SetScope(element, parentScope);
					}
				}
			}
		}
		static void EnsureWalker(DependencyObject element, DependencyObject root) {
			if (GetWalker(element) != null)
				return;
			root = root ?? GetInheritanceRoot(element);
			if (root != null && GetWalker(root) == null) {
				SubscribePresentationSourceChanged(root);
				if (PresentationSource.FromDependencyObject(root) != null) {
					SetWalker(root, new BarNameScopeTreeWalker());
					return;
				}
			}
			if (element.IsInDesignTool() || !PresentationSource.CurrentSources.OfType<object>().Any())
				SubscribePresentationSourceChanged(element);
		}
		public static void EnsureWalker(DependencyObject element) {
			if (element == null)
				return;
			EnsureWalker(element, null);
		}
		public static void DoWhenUnlocked(Action action) {
			if (!UpdateScopeLocker.IsLocked) {
				action();
				return;
			}
			AfterUpdateQueue.Enqueue(action);
		}
		static void OnUpdateScopeLockerUnlocked(object sender, EventArgs e) {
			Action[] queue = new Action[AfterUpdateQueue.Count];
			AfterUpdateQueue.CopyTo(queue, 0);
			AfterUpdateQueue.Clear();
			foreach(var element in queue) {
				element();
			}
		}
		public static DependencyObject GetInheritanceRoot(DependencyObject dObj) {
			var parents = TreeHelper.GetParents(dObj).Reverse();
			if (PresentationSource.FromDependencyObject(parents.FirstOrDefault()) == null)
				return null;
			var root = parents.FirstOrDefault();
			DependencyObject previousElement = null;
			DependencyObject currentElement = null;
			var parentEnumerator = parents.GetEnumerator();
			try {
				root.Do(x => x.SetValue(InheritancePulseProperty, true));
				while (parentEnumerator.MoveNext()) {
					previousElement = currentElement;
					currentElement = parentEnumerator.Current;
					if ((bool)dObj.GetValue(InheritancePulseProperty)) {
						break;
					}
					if ((bool)currentElement.GetValue(InheritancePulseProperty))
						continue;
					if (previousElement != null) {
						if (LogicalTreeHelper.GetChildren(previousElement).OfType<object>().Contains(currentElement)) {
							return null;
						}
						var previousVisual = previousElement as Visual;
						if (previousVisual != null) {
							bool isChild = false;
							for (int i = 0; i < VisualTreeHelper.GetChildrenCount(previousVisual); i++) {
								if (VisualTreeHelper.GetChild(previousVisual, i) == currentElement) {
									isChild = true;
									break;
								}
							}
							if (!isChild)
								return null;
						}
					}
					root.Do(x => x.ClearValue(InheritancePulseProperty));
					root = currentElement;
					root.SetValue(InheritancePulseProperty, true);
				}
			} finally {
				root.Do(x => x.ClearValue(InheritancePulseProperty));
			}			
			return root;
		}
		static void SubscribePresentationSourceChanged(DependencyObject dObj) {
			if (dObj as IInputElement == null)
				return;
			UnsubscribePresentationSourceChanged(dObj);
			PresentationSource.AddSourceChangedHandler((IInputElement)dObj, OnPresentationSourceChanged);
		}
		static void UnsubscribePresentationSourceChanged(DependencyObject dObj) {
			if (dObj as IInputElement == null)
				return;
			PresentationSource.RemoveSourceChangedHandler((IInputElement)dObj, OnPresentationSourceChanged);
		}
		static void OnPresentationSourceChanged(object sender, SourceChangedEventArgs e) {
			var nsSource = ((DependencyObject)sender).IfNot(BarNameScope.IsScopeTarget).With(BarNameScope.GetScope).With(x => x.Target).With(PresentationSource.FromDependencyObject);
			if(nsSource!=null && Equals(nsSource, e.NewSource))
				return;
			Pulse(sender);
		}
	}
	public class ScopeChangedEventArgs : RoutedEventArgs {
		internal ScopeChangedEventArgs(RoutedEvent routedEvent, BarNameScope oldScope, BarNameScope newScope) : base(routedEvent) {
			this.NewScope = newScope;
			this.OldScope = oldScope;
		}
		public BarNameScope OldScope { get; private set; }
		public BarNameScope NewScope { get; private set; }
	}
	public delegate void ScopeChangedEventHandler(object sender, ScopeChangedEventArgs e);
	public class BarNameScope : IServiceProvider{
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ScopeProperty;
		public static readonly DependencyProperty IsScopeOwnerProperty;
		public static readonly RoutedEvent ScopeChangedEvent;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal static BarNameScope GetScope(DependencyObject obj) { return (BarNameScope)obj.GetValue(ScopeProperty); }
		internal static BarNameScope FindScope(DependencyObject obj) { return obj.With(FindScopeTarget).With(GetScope); }		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal static void SetScope(DependencyObject obj, BarNameScope value) { obj.SetValue(ScopeProperty, value); }
		public static bool GetIsScopeOwner(DependencyObject obj) { return (bool)obj.GetValue(IsScopeOwnerProperty); }
		public static void SetIsScopeOwner(DependencyObject obj, bool value) { obj.SetValue(IsScopeOwnerProperty, value); }		
#if DEBUGTEST
		public readonly System.Diagnostics.StackFrame[] ctor_StackTrace;
		static readonly Native.WeakList<BarNameScope> instances = new Native.WeakList<BarNameScope>();
		public bool scope_isAttached = true;
#endif
		static BarNameScope() {			
			ScopeProperty = DependencyPropertyManager.RegisterAttached("Scope", typeof(BarNameScope), typeof(BarNameScope), new FrameworkPropertyMetadata(null, OnScopeChanged));
			IsScopeOwnerProperty = DependencyProperty.RegisterAttached("IsScopeOwner", typeof(bool), typeof(BarNameScope), new PropertyMetadata(false, OnIsScopeOwnerChanged));
			ScopeChangedEvent = EventManager.RegisterRoutedEvent("ScopeChanged", RoutingStrategy.Direct, typeof(ScopeChangedEventHandler), typeof(BarNameScope));
			EventManager.RegisterClassHandler(typeof(Window), FrameworkElement.SizeChangedEvent, new RoutedEventHandler(OnWindowSizeChanged));
#if DEBUGTEST
			EventManager.RegisterClassHandler(typeof(Core.Tests.TestWindow), FrameworkElement.SizeChangedEvent, new RoutedEventHandler(OnWindowSizeChanged));
#endif
			RegisterDecorator(() => new RegistratorFactoryDecorator((scope, type, unique) => {
				var result = new ElementRegistrator(type, unique);
				scope.Registrators[type] = result;
				return result;
			}));
			RegisterDecorator<EventListenerDecorator, IEventListenerDecoratorService>(
				() => new EventListenerDecorator(),
				d => new EventListenerDecoratorService(d),
				() => new EventListenerDecoratorService(null)
				);
			RegisterDecorator<ItemToLinkBinder, IItemToLinkBinderService>(
				() => new ItemToLinkBinder(),
				d => new ItemToLinkBinderService(d),
				() => new ItemToLinkBinderService(null)
				);
			RegisterDecorator<BarToContainerNameBinder, IBarToContainerNameBinderService>(
				() => new BarToContainerNameBinder(),
				d => new BarToContainerNameBinderService(d),
				() => new BarToContainerNameBinderService(null)
				);
			RegisterDecorator<BarToContainerTypeBinder, IBarToContainerTypeBinderService>(
				() => new BarToContainerTypeBinder(),
				d => new BarToContainerTypeBinderService(d),
				() => new BarToContainerTypeBinderService(null)
				);
			RegisterDecorator<MergingElementBinder, IMergingService>(
				() => new MergingElementBinder(),
				d => new MergingService(d),
				() => new MergingService(null)
				);
			RegisterDecorator<RadioGroupStrategy, IRadioGroupService>(
				() => new RadioGroupStrategy(),
				d => new RadioGroupService(d),
				() => new RadioGroupService());
			RegisterDecorator<ItemCommandSourceStrategy, ICommandSourceService>(
				() => new ItemCommandSourceStrategy(),
				d => new CommandSourceService(d),
				() => new CommandSourceService());
			RegisterDecorator<CustomizationDecorator, ICustomizationService>(
				() => new CustomizationDecorator(),
				d => new CustomizationService(d),
				() => new CustomizationService(null));
#if DEBUGTEST
			RegisterDecorator<BarNameScopeDebugDecorator, IBarNameScopeDebugService>(
				() => new BarNameScopeDebugDecorator(),
				d => new BarNameScopeDebugService(d),
				() => new BarNameScopeDebugService());
#endif
			EnsureRegistrator();
			Utils.Themes.WindowTracker.Initialize();			
		}
		static void OnWindowSizeChanged(object sender, RoutedEventArgs e) {
			EnsureRegistrator((DependencyObject)sender);
		}		
		static readonly List<DecoratorDataBase> decoratorDatas = new List<DecoratorDataBase>();
		readonly Dictionary<object, ElementRegistrator> registrators;
		readonly DependencyObject target;
		readonly List<BarNameScope> children;
		readonly Dictionary<Type, IBarNameScopeDecorator> decorators = new Dictionary<Type, IBarNameScopeDecorator>();
		BarNameScope parent;
		ScopeTreeNode tree;
		Dictionary<object, ElementRegistrator> Registrators { get { return registrators; } }
		public ReadOnlyCollection<BarNameScope> Children { get { return children.AsReadOnly(); } }
		public DependencyObject Target { get { return target; } }
		public ElementRegistrator this[object element] { get { return Registrators[element]; } }
		public BarNameScope Parent { get { return parent; } }
		public ScopeTreeNode ScopeTree { get { return tree; } }
		BarNameScope(DependencyObject target, BarNameScope parent) {			
#if DEBUGTEST
			ctor_StackTrace = new System.Diagnostics.StackTrace().GetFrames();
			instances.Add(this);
#endif
			this.target = target;
			this.parent = parent;
			if (parent != null)
				parent.children.Add(this);
			this.registrators = new Dictionary<object, ElementRegistrator>();
			this.children = new List<BarNameScope>();
			foreach (var data in decoratorDatas) {
				var decorator = data.CreateDecorator();
				decorators.Add(data.DecoratorType, decorator);
				decorator.Attach(this);
			}
			tree = Native.ScopeTree.Attach(this);
		}		
		void Detach(BarNameScope child) {
			if (child != null)
				children.Remove(child);
		}
		void Detach() {
			while (Children.Count != 0)
				Children[0].Detach();
			foreach (var registrator in Registrators.Values) {
				registrator.Detach();
			}
			foreach (var decorator in decorators.Values)
				decorator.Detach();
			if (Equals(this, Target.GetValue(ScopeProperty)))
				Target.ClearValue(ScopeProperty);
			if (parent != null) {
				parent.Detach(this);
				parent = null;
			}
			Native.ScopeTree.Detach(this);
#if DEBUGTEST
			scope_isAttached = false;
#endif
		}
		public static void RegisterDecorator<TDecorator>(
			Func<TDecorator> createDecoratorCallback
			) where TDecorator : IBarNameScopeDecorator {
			decoratorDatas.RemoveAll(x => x.DecoratorType == typeof(TDecorator));
			if (createDecoratorCallback != null)
				decoratorDatas.Add(new DecoratorData<TDecorator>(createDecoratorCallback));
		}
		public static void RegisterDecorator<TDecorator, TDecoratorService>(
			Func<TDecorator> createDecoratorCallback,
			Func<TDecorator, TDecoratorService> createServiceCallback,
			Func<TDecoratorService> getNullServiceCallback
			) where TDecorator : IBarNameScopeDecorator {
			decoratorDatas.RemoveAll(x => x.DecoratorType == typeof(TDecorator));
			if (createDecoratorCallback != null)
				decoratorDatas.Add(new DecoratorData<TDecorator, TDecoratorService>(createDecoratorCallback, createServiceCallback, getNullServiceCallback));
		}
		public static T GetService<T>(object target) {
			return (T)GetService(typeof(T), target);
		}
		static object GetService(Type serviceType, object target) {
			var dTarget = target as DependencyObject;
			if (dTarget == null)
				return GetService(serviceType, (BarNameScope)null);
			var scope = GetScope(dTarget);
			if (scope != null)
				return GetService(serviceType, scope);
			if (BarNameScopeTreeWalker.GetWalker(dTarget) == null)
				return GetService(serviceType, (BarNameScope)null);
			return GetService(serviceType, TreeHelper.GetParent<DependencyObject>(dTarget, x => GetIsScopeOwner(x) || GetScope(x) != null, true, true).With(GetScope));
		}
		static object GetService(Type serviceType, BarNameScope scope) {
			if (scope != null && scope.Target != null && !scope.Target.CheckAccess())
				scope = null;
			if (serviceType == typeof(IElementRegistratorService))
				return new ElementRegistratorService(scope);
			foreach (var data in decoratorDatas) {
				if (serviceType == data.ServiceType)
					return scope == null ? data.GetNullService() : data.CreateService(scope.decorators[data.DecoratorType]);
			}
			return null;
		}
		public T GetService<T>() {
			return (T)((IServiceProvider)this).GetService(typeof(T));
		}
		object IServiceProvider.GetService(Type serviceType) {
			return GetService(serviceType, this);
		}
		public static void EnsureRegistrator() {
			foreach (var source in PresentationSource.CurrentSources.OfType<PresentationSource>().ToList()) {
				var rv = source.RootVisual;
				if (rv == null || !rv.Dispatcher.CheckAccess())
					continue;
				EnsureRegistrator(rv);
			}
		}
		public static void EnsureRegistrator(DependencyObject element) {
			BarNameScopeTreeWalker.EnsureWalker(element);			
		}
		public static bool IsInSameScope(object first, object second) {
			var fs = (first as DependencyObject).With(GetScope);
			var ss = (second as DependencyObject).With(GetScope);
			return Equals(fs, ss);
		}
		public static void CreateScope(DependencyObject root, BarNameScope parent) {
			SetScope(root, CreateInstance(root, parent));
		}
		static BarNameScope CreateInstance(DependencyObject root, BarNameScope parent) {
			if (root == null
				|| Equals(root, parent.With(x => x.Target))
				|| BarNameScopeTreeWalker.GetWalker(root)==null
				|| GetPresentationSourceEx(root) == null)
				return null;
			return new BarNameScope(root, parent);
		}	   
		static PresentationSource GetPresentationSourceEx(DependencyObject dObj) {
			return PresentationSource.FromDependencyObject(TreeHelper.GetParents(dObj).LastOrDefault());
		}		
		static void OnIsScopeOwnerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BarNameScopeTreeWalker.Pulse(d);
		}
		static void OnScopeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var newScope = (BarNameScope)e.NewValue;
			var oldScope = (BarNameScope)e.OldValue;
			RaiseScopeChanged(d, e);
			var nss = d as IBarNameScopeSupport;
			if (oldScope != null && Equals(oldScope.Target, d)) {
				oldScope.Detach();
			}			
			if (nss != null) {
				if (oldScope != null && nss != null) {
					foreach (var kp in EnumerateKeysAndNames(nss))
						oldScope[kp.Key].Remove(nss);
				}
				if (newScope != null) {					
					foreach (var kp in EnumerateKeysAndNames(nss))
						newScope[kp.Key].Add(nss);
				}
			}			
		}
		static void RaiseScopeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var args = new ScopeChangedEventArgs(ScopeChangedEvent, (BarNameScope)e.OldValue, (BarNameScope)e.NewValue);
			if (d is UIElement)
				((UIElement)d).RaiseEvent(args);
			if (d is ContentElement)
				((ContentElement)d).RaiseEvent(args);
			if (d is UIElement3D)
				((UIElement3D)d).RaiseEvent(args);
		}
		#region helpers
		internal static IEnumerable<KeyValuePair<object, object>> EnumerateKeysAndNames(IBarNameScopeSupport element) {
			if (element is IMultipleElementRegistratorSupport) {
				var ms = (IMultipleElementRegistratorSupport)element;
				foreach (var key in ms.RegistratorKeys) {
					yield return new KeyValuePair<object, object>(key, ms.GetName(key));
				}
			}
		}
		public static bool IsScopeTarget(object obj) {
			if (obj is INameScope)
				return true;
			if (obj is DependencyObject) {
				var dObj = (DependencyObject)obj;
				return BarNameScope.GetIsScopeOwner(dObj)
					|| IsRootVisual(dObj as Visual);
			}
			return false;
		}		
		static bool IsRootVisual(Visual obj) {
			if (obj == null)
				return false;
			foreach(PresentationSource source in PresentationSource.CurrentSources) {
				if (source.RootVisual == obj)
					return true;
			}
			return false;
		}
		public static DependencyObject FindScopeTarget(DependencyObject d) {
			return FindScopeTarget(d, true);
		}
		public static DependencyObject FindScopeTarget(DependencyObject d, bool includeSelf) {
			return TreeHelper.GetParent(d, IsScopeTarget, includeSelf);
		}				
		#endregion
	}
}
