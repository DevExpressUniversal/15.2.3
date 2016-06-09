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

#define FAST_WEAK_EVENTS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Internal;
using DevExpress.Utils;
#if !SILVERLIGHT
using DevExpress.Data.Helpers;
#else
using DevExpress.XtraPrinting.Stubs;
#endif
namespace DevExpress.Utils {
	#region WeakEventHandler<TArgs, TBaseHandler>
	public class WeakEventHandler<TArgs, TBaseHandler> where TArgs : EventArgs {
		protected static IWeakEventHandlerStrategy<TArgs> CreateStrategy() {
#if !SILVERLIGHT && !DXPORTABLE
			if(SecurityHelper.IsPartialTrust)
				return new NonWeakEventHandlerStrategy<TArgs>();
			else
#if FAST_WEAK_EVENTS
				return new WeakEventHandlerStrategy<TArgs, TBaseHandler>();
#else
				return new WeakEventHandlerDefaultStrategy<TArgs>();
#endif
#else
			return new NonWeakEventHandlerStrategy<TArgs>();
#endif
		}
		readonly IWeakEventHandlerStrategy<TArgs> strategy;
		public WeakEventHandler()
			: this(CreateStrategy()) { }
		protected internal WeakEventHandler(IWeakEventHandlerStrategy<TArgs> strategy) {
			this.strategy = strategy;
		}
		protected internal bool IsEmpty { get { return strategy.IsEmpty; } }
		protected internal void Add(Delegate target) {
			strategy.Add(target);
		}
		protected internal void Remove(Delegate target) {
			strategy.Remove(target);
		}
		protected internal void Purge() {
			strategy.Purge();
		}
		public void Raise(object sender, TArgs args) {
			strategy.Raise(sender, args);
		}
		public static WeakEventHandler<TArgs, TBaseHandler> operator +(WeakEventHandler<TArgs, TBaseHandler> target, Delegate value) {
			if (target == null)
				target = new WeakEventHandler<TArgs, TBaseHandler>();
			else
				target.Purge();
			target.Add(value);
			return target;
		}
		public static WeakEventHandler<TArgs, TBaseHandler> operator -(WeakEventHandler<TArgs, TBaseHandler> target, Delegate value) {
			if (target == null)
				return null;
			target.Remove(value);
			if (target.IsEmpty)
				return null;
			else
				return target;
		}
	}
	#endregion
	#region WeakEventHandler<TArgs, TBaseHandler>
	public class PublicWeakEventHandler<TArgs, TBaseHandler> : WeakEventHandler<TArgs, TBaseHandler> where TArgs : EventArgs {
		protected static new IWeakEventHandlerStrategy<TArgs> CreateStrategy() {
#if !SILVERLIGHT && !DXPORTABLE
			if(SecurityHelper.IsPartialTrust)
				return new WeakEventHandlerMediumTrustStrategy<TArgs>();
			else
				return WeakEventHandler<TArgs, TBaseHandler>.CreateStrategy();
#else
			return new WeakEventHandlerMediumTrustStrategy<TArgs>();
#endif
		}
		public PublicWeakEventHandler()
			: base(CreateStrategy()) { }
		public static PublicWeakEventHandler<TArgs, TBaseHandler> operator +(PublicWeakEventHandler<TArgs, TBaseHandler> target, Delegate value) {
			if(target == null)
				target = new PublicWeakEventHandler<TArgs, TBaseHandler>();
			else
				target.Purge();
			target.Add(value);
			return target;
		}
		public static PublicWeakEventHandler<TArgs, TBaseHandler> operator -(PublicWeakEventHandler<TArgs, TBaseHandler> target, Delegate value) {
			if(target == null)
				return null;
			target.Remove(value);
			if(target.IsEmpty)
				return null;
			else
				return target;
		}
	}
	#endregion
	#region WeakKeyDictionary<TKey, TValue>
	public class WeakKeyDictionary<TKey, TValue> where TKey : class {
		readonly List<WeakReference> keys = new List<WeakReference>();
		readonly List<TValue> values = new List<TValue>();
		public IList Keys { get { return keys; } }
		public TValue this[TKey key] {
			get {
				int index = FindEntry(key);
				if (index >= 0)
					return values[index];
				throw new KeyNotFoundException();
			}
			set {
				this.Insert(key, value, false);
			}
		}
		public int Count { get { return keys.Count; } }
		public void Add(TKey key, TValue value) {
			Insert(key, value, true);
		}
		public bool Remove(TKey key) {
			if (key == null)
				throw new ArgumentException();
			int index = this.FindEntry(key);
			if (index >= 0) {
				keys.RemoveAt(index);
				values.RemoveAt(index);
				return true;
			}
			return false;
		}
		public bool ContainsKey(TKey key) {
			return FindEntry(key) >= 0;
		}
		public bool ContainsValue(TValue value) {
			return values.IndexOf(value) >= 0;
		}
		public void Clear() {
			keys.Clear();
			values.Clear();
		}
		void Insert(TKey key, TValue value, bool add) {
			if (key == null)
				throw new ArgumentException();
			int index = this.FindEntry(key);
			if (index >= 0) {
				if (add)
					throw new ArgumentException();
				values[index] = value;
				return;
			}
			keys.Add(new WeakReference(key));
			values.Add(value);
		}
		public bool TryGetValue(TKey key, out TValue value) {
			int index = FindEntry(key);
			if (index >= 0) {
				value = values[index];
				return true;
			}
			value = default(TValue);
			return false;
		}
		int FindEntry(TKey key) {
			int result = -1;
			for (int i = Count - 1; i >= 0; i--) {
				WeakReference currentKey = keys[i];
				TKey target = (TKey)currentKey.Target;
				if (target == null) {
					keys.RemoveAt(i);
					values.RemoveAt(i);
					result--;
				}
				else {
					if (Object.Equals(target, key))
						result = i;
				}
			}
			return result < 0 ? -1 : result;
		}
	}
	#endregion
	#region GenericEventListenerWrapper
	public abstract class GenericEventListenerWrapper<T, U>
		where T : class
		where U : class {
	   readonly WeakReference instanceReference;
		U eventSource;
		protected GenericEventListenerWrapper(T listenerInstance, U eventSource) {
			this.eventSource = eventSource;
			Guard.ArgumentNotNull(listenerInstance, "listenerInstance");
			this.instanceReference = new WeakReference(listenerInstance);
			SubscribeEvents();
		}
		protected WeakReference ListenerReference { get { return instanceReference; } }
		public T ListenerInstance { get { return ListenerReference.Target as T; } }
		public U EventSource { get { return eventSource; } }
		public bool IsAlive() { return ListenerReference.IsAlive && ListenerInstance != null; }
		protected abstract void SubscribeEvents();
		protected abstract void UnsubscribeEvents();
		public virtual void CleanUp() {
			UnsubscribeEvents();
			ResetEventSource();
			ResetListenerInstance();
		}
		protected virtual void ResetEventSource() {
			this.eventSource = null;
		}
		protected virtual void ResetListenerInstance() {
			ListenerReference.Target = null;
		}
	}
	#endregion
}
namespace DevExpress.Internal {
	#region IWeakEventHandlerStrategy<TArgs> (abstract class)
	public interface IWeakEventHandlerStrategy<TArgs> where TArgs : EventArgs {
		void Add(Delegate target);
		void Remove(Delegate target);
		void Raise(object sender, TArgs args);
		void Purge();
		bool IsEmpty { get; }
	}
	#endregion
#if FAST_WEAK_EVENTS
	public abstract class WeakEventHandlerBase<THandler> {
		readonly List<WeakEvent> events = new List<WeakEvent>();
		readonly MethodInfo createMethod;
		readonly Type eventType;
		#region Inner Classes
		protected struct CreateData {
			public Delegate Invoker;
			public CreateDelegateHandler Creator;
		}
		protected delegate THandler CreateDelegateHandler(WeakReference e, Delegate method);
		sealed class WeakEvent {
			MethodInfo method;
			THandler handler;
			WeakReference reference;
			public WeakEvent(WeakReference target, MethodInfo method, THandler handler) {
				this.method = method;
				this.reference = target;
				this.handler = handler;
			}
			public THandler Handler { get { return handler; } }
			public bool Equals(Delegate target) {
				Delegate tt = target;
				return method == tt.GetMethodInfo() && ((reference == null && tt.Target == ((Delegate)(object)handler).Target) || tt.Target == reference.Target);
			}
			public bool CanPurge { get { return reference != null && !reference.IsAlive; } }
		}
		#endregion
		protected WeakEventHandlerBase(Type eventType) {
			this.eventType = eventType;
			this.createMethod = GetType().GetMethod("CreateDelegate", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
		}
		public bool HasSubscribers { get { return events.Count > 0; } }
		protected abstract Dictionary<MethodInfo, CreateData> Creators { get; }
		protected void AddEvent(Delegate value) {
			events.Add(CreateDynamicEvent(value));
		}
		protected void PurgeEvents() {
			for (int i = events.Count - 1; i >= 0; i--) {
				if (events[i].CanPurge)
					events.RemoveAt(i);
			}
		}
		protected void RemoveEvent(Delegate value) {
			for (int i = events.Count - 1; i >= 0; i--) {
				if (events[i].Equals(value)) {
					events.RemoveAt(i);
					return;
				}
			}
		}
		WeakEvent CreateDynamicEvent(Delegate target) {
			Delegate tt = target;
			CreateData data;
			MethodInfo mi = tt.GetMethodInfo();
			Dictionary<MethodInfo, CreateData> creators = Creators;
			if (!creators.TryGetValue(mi, out data)) {
				if (tt.Target == null)
					data.Invoker = mi.CreateDelegate(typeof(THandler));
				else {
					Type t = mi.DeclaringType;
					data.Creator = (CreateDelegateHandler)createMethod.MakeGenericMethod(t).CreateDelegate(typeof(CreateDelegateHandler), null);
					List<Type> args = new List<Type>(typeof(THandler).GetGenericArguments());
					args.Add(t);
					data.Invoker = mi.CreateDelegate(eventType.MakeGenericType(args.ToArray()), null);
				}
				creators.Add(mi, data);
			}
			WeakEvent e;
			if (tt.Target == null)
				e = new WeakEvent(null, mi, (THandler)(object)data.Invoker);
			else {
				WeakReference r = new WeakReference(tt.Target);
				e = new WeakEvent(r, mi, data.Creator(r, data.Invoker));
			}
			return e;
		}
		protected delegate void Invoker(THandler target);
		protected void Invoke(Invoker invoker) {
			foreach (WeakEvent rec in events.ToArray()) {
				invoker(rec.Handler);
			}
		}
	}
	public class WeakEventHandler<Arg1, Arg2, TBaseHandler> : WeakEventHandlerBase<WeakEventHandler<Arg1, Arg2, TBaseHandler>.TargetEventHandler> {
		[ThreadStatic]
		static Dictionary<MethodInfo, CreateData> creators = new Dictionary<MethodInfo, CreateData>();
		delegate void EventHandler<T>(T target, Arg1 a1, Arg2 a2);
		public delegate void TargetEventHandler(Arg1 a1, Arg2 a2);
		public WeakEventHandler()
			: base(typeof(EventHandler<>)) {
		}
		protected override Dictionary<MethodInfo, CreateData> Creators {
			get {
				if (creators == null)
					creators = new Dictionary<MethodInfo, CreateData>();
				return creators;
			}
		}
		static protected TargetEventHandler CreateDelegate<T>(WeakReference e, Delegate method) {
			EventHandler<T> handler = (EventHandler<T>)method;
			return delegate(Arg1 a1, Arg2 a2)
			{
				object target = e.Target;
				if (target != null)
					handler((T)target, a1, a2);
			};
		}
		public void Invoke(Arg1 a1, Arg2 a2) {
			Invoke(delegate(TargetEventHandler target) { target(a1, a2); });
		}
	}
	public class WeakEventHandlerStrategy<TArgs, TBaseHandler> : WeakEventHandler<object, TArgs, TBaseHandler>, IWeakEventHandlerStrategy<TArgs> where TArgs : EventArgs {
		#region IWeakEventHandlerStrategy<TArgs> Members
		public bool IsEmpty { get { return !HasSubscribers; } }
		public void Add(Delegate target) {
			AddEvent(target);
		}
		public void Remove(Delegate target) {
			RemoveEvent(target);
		}
		public void Raise(object sender, TArgs args) {
			Invoke(delegate(TargetEventHandler target) { target(sender, args); });
		}
		public void Purge() {
			PurgeEvents();
		}
		#endregion
	}
#else
	#region IEventHandlerRecord<TArgs>
	public interface IEventHandlerRecord<TArgs> where TArgs : EventArgs {
		bool IsEquals(StaticDelegate<TArgs> target);
		void Invoke(object sender, TArgs args);
		bool IsAlive { get; }
	}
	#endregion
	public delegate void StaticDelegate<TArgs>(object sender, TArgs args) where TArgs : EventArgs;
	public delegate void StaticEventHandlerStub<TArgs, TTarget>(TTarget target, object sender, TArgs args) where TArgs : EventArgs;
	#region WeakEventHandlerRecord<TArgs, TTarget>
	public class WeakEventHandlerRecord<TArgs, TTarget> : WeakReference, IEventHandlerRecord<TArgs> where TArgs : EventArgs {
		readonly StaticEventHandlerStub<TArgs, TTarget> stubHandler;
		readonly MethodInfo originalMethodInfo;
		public WeakEventHandlerRecord(StaticDelegate<TArgs> handler)
			: base(handler.Target) {
			this.originalMethodInfo = handler.Method;
			Type type = typeof(StaticEventHandlerStub<TArgs, TTarget>);
			this.stubHandler = (StaticEventHandlerStub<TArgs, TTarget>)Delegate.CreateDelegate(type, null, handler.Method);
		}
		bool IEventHandlerRecord<TArgs>.IsAlive { get { return this.IsAlive; } }
		public bool IsEquals(StaticDelegate<TArgs> target) {
			return target.Target == this.Target && originalMethodInfo == target.Method;
		}
		public void Invoke(object sender, TArgs args) {
			stubHandler((TTarget)Target, sender, args);
		}
	}
	#endregion
	#region StaticEventHandlerRecord<TArgs>
	public class StaticEventHandlerRecord<TArgs> : IEventHandlerRecord<TArgs> where TArgs: EventArgs {
		readonly StaticDelegate<TArgs> actualHandler;
		public StaticEventHandlerRecord(StaticDelegate<TArgs> handler) {
			this.actualHandler = handler;
		}
		bool IEventHandlerRecord<TArgs>.IsAlive { get { return true; } }
		public bool IsEquals(StaticDelegate<TArgs> target) {
			return target.Target == null && actualHandler.Method == target.Method;
		}
		public void Invoke(object sender, TArgs args) {
			actualHandler(sender, args);
		}
	}
	#endregion
	#region WeakEventHandlerDefaultStrategy<TArgs>
	public class WeakEventHandlerDefaultStrategy<TArgs> : IWeakEventHandlerStrategy<TArgs> where TArgs : EventArgs {
		readonly List<IEventHandlerRecord<TArgs>> handlerRecords;
		public WeakEventHandlerDefaultStrategy() {
			this.handlerRecords = new List<IEventHandlerRecord<TArgs>>();
		}
		public bool IsEmpty { get { return handlerRecords.Count <= 0; } }
		public void Add(Delegate target) {
			StaticDelegate<TArgs> handler = (StaticDelegate<TArgs>)Delegate.CreateDelegate(typeof(StaticDelegate<TArgs>), target.Target, target.Method);
			System.Diagnostics.Debug.Assert(handler.Target == target.Target);
			System.Diagnostics.Debug.Assert(handler.Method == target.Method);
			AddCore(handler);
		}
		public void Remove(Delegate target) {
			StaticDelegate<TArgs> handler = (StaticDelegate<TArgs>)Delegate.CreateDelegate(typeof(StaticDelegate<TArgs>), target.Target, target.Method);
			System.Diagnostics.Debug.Assert(handler.Target == target.Target);
			System.Diagnostics.Debug.Assert(handler.Method == target.Method);
			RemoveCore(handler);
		}
		public void Raise(object sender, TArgs args) {
			bool shouldPurge = false;
			IEventHandlerRecord<TArgs>[] handlers = handlerRecords.ToArray();
			int count = handlers.Length;
			for (int i = 0; i < count; i++) {
				IEventHandlerRecord<TArgs> handler = handlers[i];
				if (handler.IsAlive)
					handler.Invoke(sender, args);
				else
					shouldPurge = true;
			}
			if (shouldPurge)
				Purge();
		}
		public void Purge() {
			for (int i = handlerRecords.Count - 1; i >= 0; i--)
				if (!handlerRecords[i].IsAlive)
					handlerRecords.RemoveAt(i);
		}
		void AddCore(StaticDelegate<TArgs> target) {
			if (target.Target != null)
				handlerRecords.Add(CreateWeakHandlerRecord(target));
			else
				handlerRecords.Add(CreateStaticHandlerRecord(target));
		}
		void RemoveCore(StaticDelegate<TArgs> target) {
			for (int i = handlerRecords.Count - 1; i >= 0; i--) {
				IEventHandlerRecord<TArgs> handler = handlerRecords[i];
				if (handler.IsEquals(target) || !handler.IsAlive)
					handlerRecords.RemoveAt(i);
			}
		}
		protected internal virtual IEventHandlerRecord<TArgs> CreateStaticHandlerRecord(StaticDelegate<TArgs> target) {
			return new StaticEventHandlerRecord<TArgs>(target);
		}
		protected internal virtual IEventHandlerRecord<TArgs> CreateWeakHandlerRecord(StaticDelegate<TArgs> target) {
			Type eventHandlerRecordType = typeof(WeakEventHandlerRecord<,>).MakeGenericType(typeof(TArgs), target.Target.GetType());
			return (IEventHandlerRecord<TArgs>)Activator.CreateInstance(eventHandlerRecordType, target);
		}
	}
	#endregion
#endif
	#region WeakEventHandlerMediumTrustStrategy<TArgs>
	public class WeakEventHandlerMediumTrustStrategy<TArgs> : IWeakEventHandlerStrategy<TArgs> where TArgs : EventArgs {
		List<HandlerRecord> handlers;
		public bool IsEmpty { get { return handlers == null || handlers.Count == 0; } }
		public void Add(Delegate target) {
			if(handlers == null)
				handlers = new List<HandlerRecord>();
			handlers.Add(new HandlerRecord(target));
		}
		public void Remove(Delegate target) {
			if(handlers == null) return;
			for(int i = handlers.Count - 1; i >= 0; i--) {
				HandlerRecord handler = handlers[i];
				if(!handler.IsAlive) {
					handlers.RemoveAt(i);
					continue;
				}
				if(handler.Equals(target)) {
					handlers.RemoveAt(i);
					return;
				}
			}
		}
		public void Raise(object sender, TArgs args) {
			if(handlers == null) return;
			for(int i = 0; i < handlers.Count; i++) {
				HandlerRecord handler = handlers[i];
				if(handler.IsAlive)
					handler.Invoke(sender, args);
				else {
					handlers.RemoveAt(i);
					i--;
				}
			}
		}
		public void Purge() {
			handlers.Clear();
		}
		class HandlerRecord {
			readonly WeakReference targetRef;
			readonly MethodInfo methodInfo;
			public HandlerRecord(Delegate handler) {
				this.targetRef = new WeakReference(handler.Target);
				this.methodInfo = handler.GetMethodInfo();
				if(!this.methodInfo.IsPublic)
					throw new ArgumentException("Non-public method can't be used for weak events in the medium trust environment");
			}
			public bool IsAlive { get { return targetRef.IsAlive; } }
			public void Invoke(object sender, TArgs args) {
				if(!IsAlive)
					throw new Exception("Object doesn't exist any more");
				methodInfo.Invoke(targetRef.Target, new object[] { sender, args });
			}
			public bool Equals(Delegate target) {
				if(!IsAlive) return false;
				return targetRef.Target == target.Target && methodInfo == target.GetMethodInfo();
			}
		}
	}
	#endregion
	#region NonWeakEventHandlerStrategy<TArgs>
	public class NonWeakEventHandlerStrategy<TArgs> : IWeakEventHandlerStrategy<TArgs> where TArgs : EventArgs {
		Delegate handler;
		public bool IsEmpty { get { return handler == null; } }
		public void Add(Delegate target) {
			handler = Delegate.Combine(handler, target);
		}
		public void Remove(Delegate target) {
			handler = Delegate.Remove(handler, target);
		}
		public void Raise(object sender, TArgs args) {
			if(handler != null)
				handler.DynamicInvoke(sender, args);
		}
		public void Purge() {
		}
	}
	#endregion
}
