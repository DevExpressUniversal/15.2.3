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
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	public abstract class BehaviorBase {
		object sourceCore;
		public object Source {
			get { return sourceCore; }
			internal set {
				if(sourceCore == value) return;
				if(sourceCore != null)
					OnDetach();
				this.sourceCore = value;
				this.sourceTypeCore = null;
				if(sourceCore != null) {
					sourceTypeCore = Source.GetType();
					OnAttach();
				}
			}
		}
		Type sourceTypeCore;
		public Type SourceType {
			get { return sourceTypeCore; }
		}
		public bool IsAttached {
			get { return sourceCore != null; }
		}
		protected abstract void OnAttach();
		protected abstract void OnDetach();
		#region Services
		internal IPOCOInterfaces POCOInterfaces;
		protected TService GetService<TService>()
			where TService : class {
			object serviceContainer = (POCOInterfaces != null) ? POCOInterfaces.GetServiceContainer(this) : null;
			return (serviceContainer != null) ? POCOInterfaces.GetService<TService>(serviceContainer) : null;
		}
		protected TService GetService<TService>(string key)
			where TService : class {
			object serviceContainer = (POCOInterfaces != null) ? POCOInterfaces.GetServiceContainer(this) : null;
			return (serviceContainer != null) ? POCOInterfaces.GetService<TService>(serviceContainer, key) : null;
		}
		#endregion Services
		#region ViewModel
		protected TViewModel GetViewModel<TViewModel>()
			where TViewModel : class {
			return (TViewModel)GetViewModel();
		}
		protected object GetViewModel() {
			return (POCOInterfaces != null) ? POCOInterfaces.GetParentViewModel(this) : null;
		}
		#endregion ViewModel
	}
	public abstract class EventTriggerBase : BehaviorBase {
		protected readonly string EventName;
		public EventTriggerBase(string eventName) {
			if(string.IsNullOrEmpty(eventName))
				throw new System.NotSupportedException("Event name should not be empty");
			this.EventName = eventName;
		}
		protected sealed override void OnAttach() {
			var eventInfo = GetEventInfo();
			if(eventInfo != null)
				Subscribe(eventInfo);
		}
		protected sealed override void OnDetach() {
			var eventInfo = GetEventInfo();
			if(eventInfo != null)
				Unsubscribe(eventInfo);
		}
		object argsCore;
		protected object Args {
			get { return argsCore; }
		}
		void SetArgs(object args) {
			this.argsCore = args;
		}
		void ResetArgs() {
			this.argsCore = null;
		}
		protected abstract void OnEvent();
		Delegate handlerDelegate;
		void Subscribe(EventInfo eventInfo) {
			HandlerExpressionBuilder builder = EventInfoHelper.GetBuilder(GetType(), eventInfo); 
			if(handlerDelegate == null) {
				handlerDelegate = builder.GetHandler(
					GetBeforeEventExpression(), GetOnEventExpression(), GetAfterEventExpression());
				builder.Subscribe(Source, handlerDelegate);
			}
		}
		void Unsubscribe(EventInfo eventInfo) {
			HandlerExpressionBuilder builder;
			if(EventInfoHelper.TryGetBuilder(GetType(), eventInfo, out builder))
				builder.Unsubscribe(Source, handlerDelegate);
			this.handlerDelegate = null;
		}
		MethodCallExpression GetBeforeEventExpression() {
			return ((Expression<Action<object>>)((e) => SetArgs(e))).Body as MethodCallExpression;
		}
		MethodCallExpression GetOnEventExpression() {
			return ((Expression<Action>)(() => OnEvent())).Body as MethodCallExpression;
		}
		MethodCallExpression GetAfterEventExpression() {
			return ((Expression<Action>)(() => ResetArgs())).Body as MethodCallExpression;
		}
		EventInfo GetEventInfo() {
			return MemberInfoHelper.GetEventInfo(SourceType, EventName, BindingFlags.Public | BindingFlags.Instance);
		}
	}
	public abstract class EventTriggerBase<TEventArgs> : EventTriggerBase
		where TEventArgs : EventArgs {
		protected EventTriggerBase(string eventName)
			: base(eventName) {
		}
		protected new TEventArgs Args { get { return base.Args as TEventArgs; } }
	}
	public static class BehaviorHelper {
		static IViewModelSource GetViewModelSource() {
			return POCOViewModelSourceProxy.Instance;
		}
		static IPOCOInterfaces GetPOCOInterfaces() {
			return POCOInterfacesProxy.Instance;
		}
		static object GetParentViewModel() {
			return null;
		}
		static IViewModelSource GetViewModelSource(MVVMContext context) {
			return (context != null) ? context.GetViewModelSource() : GetViewModelSource();
		}
		static IPOCOInterfaces GetPOCOInterfaces(MVVMContext context) {
			return (context != null) ? context.GetPOCOInterfaces() : GetPOCOInterfaces();
		}
		static object GetParentViewModel(MVVMContext context) {
			return (context != null) ? context.ViewModel : GetParentViewModel();
		}
		public static IDisposable Attach<TBehavior>(object source, MVVMContext context)
			where TBehavior : BehaviorBase {
			return (source != null) ? AttachCore<TBehavior>(source, GetParentViewModel(context), GetViewModelSource(context), GetPOCOInterfaces(context)) : null;
		}
		public static IDisposable Attach<TBehavior>(object source)
			where TBehavior : BehaviorBase {
			return (source != null) ? AttachCore<TBehavior>(source, GetParentViewModel(), GetViewModelSource(), GetPOCOInterfaces()) : null;
		}
		public static void Detach(object source) {
			List<BehaviorRefCounter> list;
			if((source != null) && behaviors.TryGetValue(source, out list)) {
				foreach(IDisposable command in list)
					command.Dispose();
				behaviors.Remove(source);
			}
		}
		public static void Detach<TBehavior>(object source) {
			List<BehaviorRefCounter> bList;
			if((source != null) && behaviors.TryGetValue(source, out bList)) {
				var list = bList.FindAll((b) => b.Match(typeof(TBehavior)));
				foreach(IDisposable command in list)
					command.Dispose();
				if(bList.Count == 0)
					behaviors.Remove(source);
			}
		}
		static WeakKeyDictionary<object, List<BehaviorRefCounter>> behaviors = new WeakKeyDictionary<object, List<BehaviorRefCounter>>();
		internal static IDisposable AttachCore<TBehavior>(object source, object parent, IViewModelSource viewModelSource, IPOCOInterfaces pocoInterfaces, params object[] parameters)
			where TBehavior : BehaviorBase {
			return AttachCore<TBehavior>(source, parent, viewModelSource, pocoInterfaces, null, parameters);
		}
		internal static IDisposable AttachCore<TBehavior>(object source, object parent, IViewModelSource viewModelSource, IPOCOInterfaces pocoInterfaces,
			Action<TBehavior> behaviorSettings, params object[] parameters)
			where TBehavior : BehaviorBase {
			List<BehaviorRefCounter> bList;
			if(!behaviors.TryGetValue(source, out bList)) {
				bList = new List<BehaviorRefCounter>();
				behaviors.Add(source, bList);
			}
			TBehavior behavior = CreateBehavior<TBehavior>(parent, viewModelSource, pocoInterfaces, parameters);
			if(behaviorSettings != null)
				behaviorSettings(behavior);
			return new BehaviorRefCounter(bList, behavior, source);
		}
		static TBehavior CreateBehavior<TBehavior>(object parent, IViewModelSource viewModelSource, IPOCOInterfaces pocoInterfaces, params object[] parameters)
			where TBehavior : BehaviorBase {
			TBehavior behavior = viewModelSource.Create(typeof(TBehavior), parameters) as TBehavior;
			behavior.POCOInterfaces = pocoInterfaces;
			if(pocoInterfaces != null && parent != null)
				pocoInterfaces.SetParentViewModel(behavior, parent);
			return behavior;
		}
		sealed class POCOViewModelSourceProxy : IViewModelSource {
			internal static IViewModelSource Instance = new POCOViewModelSourceProxy();
			object IViewModelSource.Create(Type viewModelType, params object[] parameters) {
				var viewModelSourceType = MVVMTypesResolver.Instance.GetViewModelSourceType();
				return ViewModelSourceProxy.Create(viewModelSourceType, viewModelType, parameters);
			}
		}
		sealed class BehaviorRefCounter : IDisposable {
			WeakReference wRef;
			List<BehaviorRefCounter> list;
			public BehaviorRefCounter(List<BehaviorRefCounter> list, BehaviorBase behavior, object source) {
				if(behavior != null) {
					this.list = list;
					this.wRef = new WeakReference(behavior);
					list.Add(this);
					behavior.Source = source;
				}
			}
			void IDisposable.Dispose() {
				BehaviorBase behavior = wRef.Target as BehaviorBase;
				if(behavior != null) behavior.Source = null;
				list.Remove(this);
			}
			public bool Match(Type type) {
				BehaviorBase behavior = wRef.Target as BehaviorBase;
				return (behavior == null) || type.IsAssignableFrom(behavior.GetType());
			}
		}
	}
}
#if DEBUGTEST
namespace DevExpress.Utils.MVVM.Tests {
	using System;
	using NUnit.Framework;
	#region Test classes
	class TestEventSource {
		public event EventHandler Loaded;
		public void RaiseLoaded() {
			if(Loaded != null)
				Loaded(this, EventArgs.Empty);
			eventCount++;
		}
		internal int eventCount;
	}
	class TestEventTrigger : EventTriggerBase<EventArgs> {
		public TestEventTrigger()
			: base("Loaded") {
		}
		public int eventCount;
		protected override void OnEvent() {
			eventCount++;
		}
	}
	class EventTriggerTests_ViewModelSource<T> : IViewModelSource
		where T : new() {
		public T instance;
		object IViewModelSource.Create(Type viewModelType, params object[] parameters) {
			instance = new T();
			return instance;
		}
	}
	#endregion Test classes
	[TestFixture]
	public class EventTriggerTests : MVVM.Tests.MVVMDependentTest {
		[Test]
		public void Bind_Loaded_Command() {
			TestEventSource source = new TestEventSource();
			TestEventTrigger trigger = new TestEventTrigger();
			trigger.Source = source;
			Assert.IsTrue(trigger.IsAttached);
			Assert.AreEqual(source, trigger.Source);
			Assert.AreEqual(typeof(TestEventSource), trigger.SourceType);
			Assert.AreEqual(0, trigger.eventCount);
			source.RaiseLoaded();
			Assert.AreEqual(1, trigger.eventCount);
			trigger.Source = null;
			Assert.IsNull(trigger.Source);
			Assert.IsNull(trigger.SourceType);
			Assert.IsFalse(trigger.IsAttached);
			source.RaiseLoaded();
			Assert.AreEqual(1, trigger.eventCount);
		}
		[Test]
		public void Bind_Behavior_Attach() {
			var vms = new EventTriggerTests_ViewModelSource<TestEventTrigger>();
			TestEventSource source = new TestEventSource();
			var detach = BehaviorHelper.AttachCore<TestEventTrigger>(source, null, vms, null);
			var trigger = vms.instance;
			Assert.IsTrue(trigger.IsAttached);
			Assert.AreEqual(source, trigger.Source);
			Assert.AreEqual(typeof(TestEventSource), trigger.SourceType);
			Assert.AreEqual(0, trigger.eventCount);
			source.RaiseLoaded();
			Assert.AreEqual(1, trigger.eventCount);
			detach.Dispose();
			Assert.IsNull(trigger.Source);
			Assert.IsNull(trigger.SourceType);
			Assert.IsFalse(trigger.IsAttached);
			source.RaiseLoaded();
			Assert.AreEqual(1, trigger.eventCount);
		}
		[Test]
		public void Bind_Behavior_Detach() {
			var vms = new EventTriggerTests_ViewModelSource<TestEventTrigger>();
			TestEventSource source = new TestEventSource();
			BehaviorHelper.AttachCore<TestEventTrigger>(source, null, vms, null);
			var trigger = vms.instance;
			Assert.IsTrue(trigger.IsAttached);
			Assert.AreEqual(source, trigger.Source);
			Assert.AreEqual(typeof(TestEventSource), trigger.SourceType);
			Assert.AreEqual(0, trigger.eventCount);
			source.RaiseLoaded();
			Assert.AreEqual(1, trigger.eventCount);
			BehaviorHelper.Detach<TestEventTrigger>(source);
			Assert.IsNull(trigger.Source);
			Assert.IsNull(trigger.SourceType);
			Assert.IsFalse(trigger.IsAttached);
			source.RaiseLoaded();
			Assert.AreEqual(1, trigger.eventCount);
		}
	}
}
#endif
