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
	using System.ComponentModel;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	public interface IPropertyChangedTrigger {
		void Unregister(ITriggerAction action);
		void Register(ITriggerAction action);
		void Execute(ITriggerAction action = null);
	}
	public interface INotifyPropertyChangedTrigger {
		void Unregister(string propertyName, ITriggerAction action);
		void Register(Expression<Func<object, object>> selectorExpression, ITriggerAction action);
		void Execute(string propertyName, ITriggerAction action = null);
	}
	public interface ITriggerAction {
		bool CanExecute(object value);
		void Execute(object value);
		bool IsExecuting { get; }
	}
	public interface IPropertyBinding : IDisposable {
		bool IsOneWay { get; }
		bool IsTwoWay { get; }
		void UpdateTarget();
		void UpdateSource();
	}
	public interface ISourceChangeAware {
		void SourceChanged(object source);
	}
	static class BindingHelper {
		internal static IPropertyBinding SetBinding<TTarget, TValue>(TTarget dest, Expression<Func<TTarget, TValue>> selectorExpression,
			object source, Type sourceType, string path)
			where TTarget : class {
			return new SourceChangeTracker(path, source, sourceType,
				(src, srcType, property) => new Binding<TTarget, TValue>(src, srcType, property.Name, dest, selectorExpression));
		}
		internal static IPropertyBinding SetBinding<TTarget, TValue, TSource, TSourceValue>(TTarget dest, Expression<Func<TTarget, TValue>> selectorExpression,
			object source, Type sourceType, string path, Func<TSourceValue, TValue> convert, Func<TValue, TSourceValue> convertBack)
			where TTarget : class {
			return new SourceChangeTracker(path, source, sourceType,
				(src, srcType, property) => new Binding<TTarget, TValue>(src, srcType, property.Name, dest, selectorExpression,
					Binding<TTarget, TValue>.GetConverter(convert),
					Binding<TTarget, TValue>.GetBackConverter(convertBack)));
		}
		internal static IPropertyBinding SetBinding<TSourceEventArgs, TTarget, TValue>(TTarget target, Expression<Func<TTarget, TValue>> selectorExpression,
			object source, string sourceEventName, Func<TSourceEventArgs, TValue> eventArgsConverter)
			where TSourceEventArgs : EventArgs
			where TTarget : class {
			string path = ExpressionHelper.GetPath(selectorExpression);
			if(NestedPropertiesHelper.HasRootPath(path))
				return new SourceChangeTracker(path, target, typeof(TTarget),
						(actualTarget, actualTargetType, property) => new PCEBindingOneWay<TSourceEventArgs, TValue>(source, sourceEventName, eventArgsConverter,
							actualTarget, actualTargetType, property.Name));
			return new PCEBindingOneWay<TSourceEventArgs, TTarget, TValue>(source, sourceEventName, eventArgsConverter, target, selectorExpression);
		}
		internal static IPropertyBinding SetBinding<TSourceEventArgs, TSource, TTarget, TValue>(TTarget target, Expression<Func<TTarget, TValue>> selectorExpression,
			TSource source, string sourceEventName, Func<TSourceEventArgs, TValue> eventArgsConverter, Action<TSource, TValue> updateSourceAction = null)
			where TSourceEventArgs : EventArgs
			where TTarget : class {
			string path = ExpressionHelper.GetPath(selectorExpression);
			if(NestedPropertiesHelper.HasRootPath(path))
				return new SourceChangeTracker(path, target, typeof(TTarget),
						(actualTarget, actualTargetType, property) => new PCEBindingTwoWay<TSourceEventArgs, TSource, TValue>(source, sourceEventName, eventArgsConverter,
							actualTarget, actualTargetType, property.Name, updateSourceAction));
			return new PCEBindingTwoWay<TSourceEventArgs, TSource, TTarget, TValue>(source, sourceEventName, eventArgsConverter, target, selectorExpression, updateSourceAction);
		}
		class SourceChangeTracker : IPropertyBinding {
			IPropertyBinding bindingCore;
			DisposableObjectsContainer container;
			public SourceChangeTracker(string path, object source, Type sourceType, Func<object, Type, PropertyDescriptor, IPropertyBinding> createBinding) {
				this.container = new DisposableObjectsContainer();
				this.bindingCore = CreateBinding(path, source, sourceType, createBinding);
				TrackSourceChanges(bindingCore as ISourceChangeAware, path, source, sourceType);
			}
			IPropertyBinding CreateBinding(string path, object source, Type sourceType, Func<object, Type, PropertyDescriptor, IPropertyBinding> createBinding) {
				var property = NestedPropertiesHelper.GetProperty(path, ref source, ref sourceType);
				return createBinding(source, sourceType, property);
			}
			void IDisposable.Dispose() {
				Ref.Dispose(ref bindingCore);
				Ref.Dispose(ref container);
				GC.SuppressFinalize(this);
			}
			bool IPropertyBinding.IsOneWay {
				get { return bindingCore.IsOneWay; }
			}
			bool IPropertyBinding.IsTwoWay {
				get { return bindingCore.IsTwoWay; }
			}
			void IPropertyBinding.UpdateTarget() {
				bindingCore.UpdateTarget();
			}
			void IPropertyBinding.UpdateSource() {
				bindingCore.UpdateSource();
			}
			void TrackSourceChanges(ISourceChangeAware binding, string path, object source, Type sourceType) {
				if(binding == null) return;
				do {
					string rootPath = NestedPropertiesHelper.GetRootPath(ref path);
					if(string.IsNullOrEmpty(rootPath))
						break;
					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(sourceType);
					PropertyDescriptor rootDescriptor = properties[rootPath];
					if(rootDescriptor != null) {
						var propertySelector = ExpressionHelper.Accessor<object>(sourceType, rootDescriptor.Name);
						var sourceChangeAction = new SourceChangeAction(path, rootDescriptor.PropertyType, binding);
						var triggerRef = SetTrigger(source, sourceType, propertySelector, sourceChangeAction);
						if(triggerRef != null)
							container.Register(triggerRef);
						source = rootDescriptor.GetValue(source);
						sourceType = rootDescriptor.PropertyType;
					}
				}
				while(true);
			}
			IDisposable SetTrigger(object source, Type sourceType, Expression<Func<object, object>> propertySelector, SourceChangeAction triggerAction) {
				if(source is INotifyPropertyChanged)
					return SetNPCTriggerCore(source, propertySelector, triggerAction);
				if(MemberInfoHelper.HasChangedEvent(sourceType, ExpressionHelper.GetPropertyName(propertySelector)))
					return SetCLRTriggerCore(source, propertySelector, triggerAction);
				return null;
			}
			sealed class SourceChangeAction : ITriggerAction {
				string path;
				Type sourceType;
				ISourceChangeAware bindingEx;
				public SourceChangeAction(string path, Type sourceType, ISourceChangeAware binding) {
					this.path = path;
					this.sourceType = sourceType;
					this.bindingEx = binding;
				}
				bool ITriggerAction.CanExecute(object value) {
					return bindingEx != null;
				}
				void ITriggerAction.Execute(object value) {
					executing++;
					bindingEx.SourceChanged(NestedPropertiesHelper.GetSource(path, value, sourceType));
					executing--;
				}
				int executing;
				bool ITriggerAction.IsExecuting {
					get { return executing > 0; }
				}
			}
		}
		#region Bindings
		class PCEBindingOneWay<TSourceEventArgs, TValue> : IPropertyBinding, ISourceChangeAware
			where TSourceEventArgs : EventArgs {
			ITriggerAction updateTarget;
			IDisposable sourceTriggerRef;
			string propertyName;
			Action<object> initializationRoutine;
			public PCEBindingOneWay(object source, string sourceEventName, Func<TSourceEventArgs, TValue> eventArgsConverter, object target, Type targetType, string targetPropertyName) {
				this.propertyName = targetPropertyName;
				if(eventArgsConverter != null) {
					initializationRoutine = (tgt) =>
					{
						var targetProperty = targetType.GetProperty(targetPropertyName);
						this.updateTarget = new TriggerAction(target, targetType, targetProperty, null);
						this.sourceTriggerRef = SetPCETriggerCore<TSourceEventArgs, TValue>(source, sourceEventName, eventArgsConverter, updateTarget);
						UpdateTargetCore();
						initializationRoutine = null;
					};
					EnsureTargetTriggerInitialized(target);
				}
			}
			void EnsureTargetTriggerInitialized(object target) {
				if(target != null && initializationRoutine != null)
					initializationRoutine(target);
			}
			bool IPropertyBinding.IsOneWay {
				get { return (sourceTriggerRef != null); }
			}
			bool IPropertyBinding.IsTwoWay {
				get { return false; }
			}
			void ISourceChangeAware.SourceChanged(object target) {
				EnsureTargetTriggerInitialized(target);
				if(updateTarget != null)
					((TriggerAction)updateTarget).ChangeTarget(target);
			}
			void IPropertyBinding.UpdateTarget() {
				UpdateTargetCore();
			}
			void IPropertyBinding.UpdateSource() {
			}
			void UpdateTargetCore() {
				if(sourceTriggerRef == null) return;
				((TriggerRefCounterBase)sourceTriggerRef).ExecuteActions(propertyName, updateTarget);
			}
			void IDisposable.Dispose() {
				if(sourceTriggerRef != null)
					((TriggerRefCounterBase)sourceTriggerRef).Release(updateTarget);
				Ref.Dispose(ref sourceTriggerRef);
			}
		}
		class PCEBindingOneWay<TSourceEventArgs, TTarget, TValue> : IPropertyBinding
			where TTarget : class
			where TSourceEventArgs : EventArgs {
			ITriggerAction updateTarget;
			IDisposable sourceTriggerRef;
			string propertyName;
			public PCEBindingOneWay(object source, string sourceEventName, Func<TSourceEventArgs, TValue> eventArgsConverter, TTarget target, Expression<Func<TTarget, TValue>> selectorExpression) {
				this.propertyName = ExpressionHelper.GetPropertyName(selectorExpression);
				if(eventArgsConverter != null) {
					this.updateTarget = CreateTriggerAction<TTarget, TValue>(target, selectorExpression);
					this.sourceTriggerRef = SetPCETriggerCore<TSourceEventArgs, TValue>(source, sourceEventName, eventArgsConverter, updateTarget);
					UpdateTargetCore();
				}
			}
			bool IPropertyBinding.IsOneWay {
				get { return (sourceTriggerRef != null); }
			}
			bool IPropertyBinding.IsTwoWay {
				get { return false; }
			}
			void IPropertyBinding.UpdateTarget() {
				UpdateTargetCore();
			}
			void IPropertyBinding.UpdateSource() {
			}
			void UpdateTargetCore() {
				if(sourceTriggerRef == null) return;
				((TriggerRefCounterBase)sourceTriggerRef).ExecuteActions(propertyName, updateTarget);
			}
			void IDisposable.Dispose() {
				if(sourceTriggerRef != null)
					((TriggerRefCounterBase)sourceTriggerRef).Release(updateTarget);
				Ref.Dispose(ref sourceTriggerRef);
			}
		}
		class PCEBindingTwoWay<TSourceEventArgs, TSource, TValue> : IPropertyBinding, ISourceChangeAware
			where TSourceEventArgs : EventArgs {
			ITriggerAction updateTarget;
			ITriggerAction updateSource;
			IDisposable sourceTriggerRef;
			IDisposable targetTriggerRef;
			string propertyName;
			Action<object> initializationRoutine;
			public PCEBindingTwoWay(TSource source, string sourceEventName, Func<TSourceEventArgs, TValue> eventArgsConverter,
				object target, Type targetType, string targetPropertyName, Action<TSource, TValue> updateSourceAction) {
				this.propertyName = targetPropertyName;
				if(eventArgsConverter != null) {
					initializationRoutine = (tgt) =>
					{
						var targetProperty = targetType.GetProperty(targetPropertyName);
						this.updateTarget = new TriggerAction(tgt, targetType, targetProperty, null);
						this.sourceTriggerRef = SetPCETriggerCore<TSourceEventArgs, TValue>(source, sourceEventName, eventArgsConverter, updateTarget);
						if(updateSourceAction != null) {
							if(typeof(INotifyPropertyChanged).IsAssignableFrom(targetType) || MemberInfoHelper.HasChangedEvent(targetType, propertyName)) {
								updateSource = CreateCallbackTriggerAction<TValue>((value) => updateSourceAction(source, value), updateTarget);
								targetTriggerRef = SetTrigger(tgt, targetType, ExpressionHelper.Accessor<object>(targetType, targetProperty), updateSource);
							}
						}
						UpdateSourceCore();
						initializationRoutine = null;
					};
					EnsureTargetTriggerInitialized(target);
				}
			}
			void EnsureTargetTriggerInitialized(object target) {
				if(target != null && initializationRoutine != null)
					initializationRoutine(target);
			}
			IDisposable SetTrigger(object target, Type targetType, Expression<Func<object, object>> propertySelector, ITriggerAction triggerAction) {
				if(target is INotifyPropertyChanged)
					return SetNPCTriggerCore(target, propertySelector, triggerAction);
				if(MemberInfoHelper.HasChangedEvent(targetType, ExpressionHelper.GetPropertyName(propertySelector)))
					return SetCLRTriggerCore(target, propertySelector, triggerAction);
				return null;
			}
			bool IPropertyBinding.IsOneWay {
				get { return (sourceTriggerRef != null) && (targetTriggerRef == null); }
			}
			bool IPropertyBinding.IsTwoWay {
				get { return (sourceTriggerRef != null) && (targetTriggerRef != null); }
			}
			void ISourceChangeAware.SourceChanged(object target) {
				EnsureTargetTriggerInitialized(target);
				if(updateTarget != null)
					((TriggerAction)updateTarget).ChangeTarget(target);
				var triggerRef = targetTriggerRef as TriggerRefCounterBase;
				if(triggerRef != null) {
					if(triggerRef.Update(target))
						triggerRef.ExecuteActions(propertyName, updateSource);
				}
			}
			void IPropertyBinding.UpdateTarget() {
				UpdateTargetCore();
			}
			void IPropertyBinding.UpdateSource() {
				UpdateSourceCore();
			}
			void UpdateTargetCore() {
				if(sourceTriggerRef == null) return;
				((TriggerRefCounterBase)sourceTriggerRef).ExecuteActions(propertyName, updateTarget);
			}
			void UpdateSourceCore() {
				if(targetTriggerRef == null) return;
				((TriggerRefCounterBase)targetTriggerRef).ExecuteActions(propertyName, updateSource);
			}
			void IDisposable.Dispose() {
				if(sourceTriggerRef != null)
					((TriggerRefCounterBase)sourceTriggerRef).Release(updateTarget);
				Ref.Dispose(ref sourceTriggerRef);
				if(targetTriggerRef != null)
					((TriggerRefCounterBase)targetTriggerRef).Release(updateSource, propertyName);
				Ref.Dispose(ref targetTriggerRef);
			}
		}
		class PCEBindingTwoWay<TSourceEventArgs, TSource, TTarget, TValue> : IPropertyBinding, ISourceChangeAware
			where TTarget : class
			where TSourceEventArgs : EventArgs {
			ITriggerAction updateTarget;
			ITriggerAction updateSource;
			IDisposable sourceTriggerRef;
			IDisposable targetTriggerRef;
			string propertyName;
			public PCEBindingTwoWay(TSource source, string sourceEventName, Func<TSourceEventArgs, TValue> eventArgsConverter, TTarget target,
				Expression<Func<TTarget, TValue>> selectorExpression, Action<TSource, TValue> updateSourceAction) {
				this.propertyName = ExpressionHelper.GetPropertyName(selectorExpression);
				if(eventArgsConverter != null) {
					this.updateTarget = CreateTriggerAction<TTarget, TValue>(target, selectorExpression);
					this.sourceTriggerRef = SetPCETriggerCore<TSourceEventArgs, TValue>(source, sourceEventName, eventArgsConverter, updateTarget);
					if(updateSourceAction != null) {
						Type targetType = target.GetType();
						if(typeof(INotifyPropertyChanged).IsAssignableFrom(targetType) || MemberInfoHelper.HasChangedEvent(targetType, propertyName)) {
							updateSource = CreateCallbackTriggerAction<TValue>((value) => updateSourceAction(source, value), updateTarget);
							var targetSelector = ExpressionHelper.Accessor<TTarget>(typeof(TTarget), ExpressionHelper.GetMember(selectorExpression));
							targetTriggerRef = SetTrigger<TTarget>(target, targetSelector, updateSource);
						}
					}
					UpdateSourceCore();
				}
			}
			bool IPropertyBinding.IsOneWay {
				get { return (sourceTriggerRef != null) && (targetTriggerRef == null); }
			}
			bool IPropertyBinding.IsTwoWay {
				get { return (sourceTriggerRef != null) && (targetTriggerRef != null); }
			}
			void ISourceChangeAware.SourceChanged(object target) {
				if(updateTarget != null)
					((TriggerAction<TTarget, TValue>)updateTarget).ChangeTarget(target);
				var triggerRef = targetTriggerRef as TriggerRefCounterBase;
				if(triggerRef != null) {
					if(triggerRef.Update(target))
						triggerRef.ExecuteActions(propertyName, updateSource);
				}
			}
			void IPropertyBinding.UpdateTarget() {
				UpdateTargetCore();
			}
			void IPropertyBinding.UpdateSource() {
				UpdateSourceCore();
			}
			void UpdateTargetCore() {
				if(sourceTriggerRef == null) return;
				((TriggerRefCounterBase)sourceTriggerRef).ExecuteActions(propertyName, updateTarget);
			}
			void UpdateSourceCore() {
				if(targetTriggerRef == null) return;
				((TriggerRefCounterBase)targetTriggerRef).ExecuteActions(propertyName, updateSource);
			}
			void IDisposable.Dispose() {
				if(sourceTriggerRef != null)
					((TriggerRefCounterBase)sourceTriggerRef).Release(updateTarget);
				Ref.Dispose(ref sourceTriggerRef);
				if(targetTriggerRef != null)
					((TriggerRefCounterBase)targetTriggerRef).Release(updateSource, propertyName);
				Ref.Dispose(ref targetTriggerRef);
			}
		}
		class Binding<TTarget, TValue> : IPropertyBinding, ISourceChangeAware
			where TTarget : class {
			IDisposable sourceTriggerRef;
			IDisposable targetTriggerRef;
			ITriggerAction updateTarget;
			ITriggerAction updateSource;
			string sourcePropertyName;
			string targetPropertyName;
			Action<object> initializationRoutine;
			public Binding(object source, Type sourceType, string propertyName, TTarget target, Expression<Func<TTarget, TValue>> selectorExpression,
				Func<object, TValue> convert = null, Func<object, object> convertBack = null) {
				this.sourcePropertyName = propertyName;
				this.updateTarget = CreateTriggerAction<TTarget, TValue>(target, selectorExpression, convert);
				var sourceProperty = sourceType.GetProperty(propertyName);
				if(sourceProperty != null) {
					initializationRoutine = (src) =>
					{
						var sourcePropertySelector = ExpressionHelper.Accessor<object>(sourceType, sourceProperty);
						sourceTriggerRef = SetNPCTriggerCore(src, sourcePropertySelector, updateTarget);
						Type targetType = target.GetType();
						targetPropertyName = ExpressionHelper.GetPropertyName(selectorExpression);
						if(typeof(INotifyPropertyChanged).IsAssignableFrom(targetType) || MemberInfoHelper.HasChangedEvent(targetType, targetPropertyName)) {
							this.updateSource = new TriggerAction(src, sourceType, sourceProperty, updateTarget, convertBack);
							var targetSelector = ExpressionHelper.Accessor<TTarget>(typeof(TTarget), ExpressionHelper.GetMember(selectorExpression));
							targetTriggerRef = SetTrigger<TTarget>(target, targetSelector, updateSource);
						}
						UpdateTargetCore();
						this.initializationRoutine = null;
					};
					EnsureSourceTriggerInitialized(source);
				}
			}
			void EnsureSourceTriggerInitialized(object source) {
				if(source != null && initializationRoutine != null)
					initializationRoutine(source);
			}
			internal static Func<object, TValue> GetConverter<TSourceValue>(Func<TSourceValue, TValue> convert) {
				if(convert == null) {
					if(typeof(TValue).IsAssignableFrom(typeof(TSourceValue)))
						return (srcVal) => (TValue)srcVal;
					if(typeof(TValue) == typeof(string))
						return (srcVal) => (TValue)((srcVal != null) ? (object)srcVal.ToString() : null);
					if(typeof(TValue).IsPrimitive)
						return (srcVal) => (TValue)Convert.ChangeType(srcVal, typeof(TValue));
					return null;
				}
				return (srcVal) => convert((TSourceValue)srcVal);
			}
			internal static Func<object, object> GetBackConverter<TSourceValue>(Func<TValue, TSourceValue> convertBack) {
				if(convertBack == null) {
					if(typeof(TSourceValue).IsAssignableFrom(typeof(TValue)))
						return (val) => val;
					if(typeof(TSourceValue) == typeof(string))
						return (val) => ((val != null) ? (object)val.ToString() : null);
					if(typeof(TSourceValue).IsPrimitive)
						return (val) => (TSourceValue)Convert.ChangeType(val, typeof(TSourceValue));
					if(typeof(TSourceValue).IsEnum && typeof(TValue) == typeof(string))
						return (val) => Enum.GetNames(typeof(TSourceValue)).FirstOrDefault(s => s == (string)val);
					return null;
				}
				return (val) => (TSourceValue)convertBack((TValue)val);
			}
			void IPropertyBinding.UpdateTarget() {
				UpdateTargetCore();
			}
			void IPropertyBinding.UpdateSource() {
				UpdateSourceCore();
			}
			bool IPropertyBinding.IsOneWay {
				get { return (sourceTriggerRef != null) && (targetTriggerRef == null); }
			}
			bool IPropertyBinding.IsTwoWay {
				get { return (sourceTriggerRef != null) && (targetTriggerRef != null); }
			}
			void ISourceChangeAware.SourceChanged(object source) {
				EnsureSourceTriggerInitialized(source);
				if(updateSource != null)
					((TriggerAction)updateSource).ChangeTarget(source);
				var triggerRef = sourceTriggerRef as TriggerRefCounterBase;
				if(triggerRef != null) {
					if(triggerRef.Update(source))
						triggerRef.ExecuteActions(sourcePropertyName, updateTarget);
				}
			}
			void UpdateSourceCore() {
				if(targetTriggerRef == null) return;
				((TriggerRefCounterBase)targetTriggerRef).ExecuteActions(targetPropertyName, updateSource);
			}
			void UpdateTargetCore() {
				if(sourceTriggerRef == null) return;
				((TriggerRefCounterBase)sourceTriggerRef).ExecuteActions(sourcePropertyName, updateTarget);
			}
			void IDisposable.Dispose() {
				if(targetTriggerRef != null)
					((TriggerRefCounterBase)targetTriggerRef).Release(updateSource, targetPropertyName);
				if(sourceTriggerRef != null)
					((TriggerRefCounterBase)sourceTriggerRef).Release(updateTarget, sourcePropertyName);
				Ref.Dispose(ref sourceTriggerRef);
				Ref.Dispose(ref targetTriggerRef);
			}
		}
		#endregion Bindings
		internal static IDisposable SetNPCTrigger<TSource, TValue>(TSource source, Expression<Func<TSource, TValue>> selectorExpression, Action<TValue> action = null) {
			return SetNPCTriggerCore(source, ExpressionHelper.Box(selectorExpression), CreateTriggerAction(action));
		}
		internal static IDisposable SetNPCTrigger<TSource, TValue>(TSource source, Expression<Func<TSource, TValue>> selectorExpression, Action<TValue> action, out ITriggerAction triggerAction) {
			return SetNPCTriggerCore(source, ExpressionHelper.Box(selectorExpression), triggerAction = CreateTriggerAction(action));
		}
		internal static INPCPropertyChangedTrigger GetNPCTrigger(IDisposable triggerRef) {
			return ((TriggerRefCounterBase)triggerRef).trigger as INPCPropertyChangedTrigger;
		}
		internal static IDisposable SetTrigger<TSource>(TSource source, Expression<Func<TSource, object>> selectorExpression,
			ITriggerAction action = null, bool forceCLRPropertyChanged = false) where TSource : class {
			if(source is INotifyPropertyChanged && !forceCLRPropertyChanged)
				return SetNPCTriggerCore(source, ExpressionHelper.Box(selectorExpression), action);
			else
				return SetCLRTriggerCore(source, ExpressionHelper.Box(selectorExpression), action);
		}
		#region SetTrigger Core
		static WeakKeyDictionary<object, IDictionary<object, TriggerRefCounterBase>> triggers = new WeakKeyDictionary<object, IDictionary<object, TriggerRefCounterBase>>();
		internal static IDisposable SetNPCTriggerCore(object source, Expression<Func<object, object>> sourcePropertySelector, ITriggerAction action = null) {
			return SetTriggerCore(source,
				() => typeof(INPCPropertyChangedTrigger),
				() => new INPCPropertyChangedTrigger(), sourcePropertySelector, action);
		}
		internal static IDisposable SetCLRTriggerCore(object source, Expression<Func<object, object>> sourcePropertySelector, ITriggerAction action = null) {
			return SetTriggerCore(source,
				() => CLRPropertyChangedTrigger.GetKey(sourcePropertySelector),
				() => new CLRPropertyChangedTrigger(sourcePropertySelector), action);
		}
#if DEBUGTEST
		internal
#endif
 static IDisposable SetPCETriggerCore<TEventArgs, TValue>(object source, string eventName, Func<TEventArgs, TValue> converter, ITriggerAction action = null)
			where TEventArgs : EventArgs {
			return SetTriggerCore(source,
				() => PropertyChangedEventTrigger<TEventArgs, TValue>.GetKey(eventName),
				() => new PropertyChangedEventTrigger<TEventArgs, TValue>(eventName, converter), action);
		}
		static IDisposable SetTriggerCore(object source, Func<object> getKey, Func<EventTriggerBase> createTrigger, Expression<Func<object, object>> selector, ITriggerAction action = null) {
			return CreateTriggerRef(source, getKey, createTrigger, (list, trigger, key) => new NPCTriggerRefCounter(list, trigger, key)).AddRef(source, action, selector);
		}
		static IDisposable SetTriggerCore(object source, Func<object> getKey, Func<EventTriggerBase> createTrigger, ITriggerAction action = null) {
			return CreateTriggerRef(source, getKey, createTrigger, (list, trigger, key) => new PCETriggerRefCounter(list, trigger, key)).AddRef(source, action);
		}
		#endregion SetTrigger
		#region Trigger Reference-Counting
		static TriggerRefCounterBase CreateTriggerRef(object source, Func<object> getKey, Func<EventTriggerBase> createTrigger,
			Func<IDictionary<object, TriggerRefCounterBase>, EventTriggerBase, object, TriggerRefCounterBase> createCommand) {
			IDictionary<object, TriggerRefCounterBase> tList;
			if(!triggers.TryGetValue(source, out tList)) {
				tList = new Dictionary<object, TriggerRefCounterBase>();
				triggers.Add(source, tList);
			}
			TriggerRefCounterBase command;
			object key = getKey();
			if(!tList.TryGetValue(key, out command)) {
				EventTriggerBase trigger = createTrigger();
				command = createCommand(tList, trigger, key);
			}
			return command;
		}
		abstract class TriggerRefCounterBase : IDisposable {
			internal EventTriggerBase trigger;
			IDictionary<object, TriggerRefCounterBase> list;
			int refCount;
			object key;
			protected TriggerRefCounterBase(IDictionary<object, TriggerRefCounterBase> list, EventTriggerBase trigger, object key) {
				if(trigger != null) {
					this.list = list;
					this.trigger = trigger;
					this.key = key;
				}
			}
			void IDisposable.Dispose() {
				ReleaseCore();
			}
			public bool Update(object source) {
				if(trigger == null)
					return false;
				object oldSource = trigger.Source;
				trigger.Source = source;
				return !object.Equals(oldSource, source);
			}
			protected void AddRefCore(object source) {
				if(0 == refCount++) {
					list.Add(key, this);
					if(trigger != null)
						trigger.Source = source;
				}
			}
			void ReleaseCore() {
				if(--refCount == 0) {
					list.Remove(key);
					if(trigger != null)
						trigger.Source = null;
				}
			}
			public IDisposable AddRef(object source, ITriggerAction action, params object[] parameters) {
				AddRefCore(source);
				if(action != null)
					RegisterAction(action, parameters);
				return this;
			}
			public void Release(ITriggerAction action, params object[] parameters) {
				if(action != null)
					UnregisterAction(action, parameters);
			}
			protected object GetTrigger() {
				return trigger;
			}
			public abstract void ExecuteActions(string propertyName, ITriggerAction action);
			protected abstract void RegisterAction(ITriggerAction action, params object[] parameters);
			protected abstract void UnregisterAction(ITriggerAction action, params object[] parameters);
		}
		sealed class NPCTriggerRefCounter : TriggerRefCounterBase {
			internal NPCTriggerRefCounter(IDictionary<object, TriggerRefCounterBase> list, EventTriggerBase trigger, object key)
				: base(list, trigger, key) {
			}
			protected override void UnregisterAction(ITriggerAction action, params object[] parameters) {
				INotifyPropertyChangedTrigger npcTrigger = GetTrigger() as INotifyPropertyChangedTrigger;
				if(npcTrigger != null)
					npcTrigger.Unregister((string)parameters[0], action);
			}
			protected override void RegisterAction(ITriggerAction action, params object[] parameters) {
				INotifyPropertyChangedTrigger npcTrigger = GetTrigger() as INotifyPropertyChangedTrigger;
				if(npcTrigger != null)
					npcTrigger.Register((Expression<Func<object, object>>)parameters[0], action);
			}
			public override void ExecuteActions(string propertyName, ITriggerAction action) {
				INotifyPropertyChangedTrigger pcTrigger = GetTrigger() as INotifyPropertyChangedTrigger;
				if(pcTrigger != null)
					pcTrigger.Execute(propertyName, action);
			}
		}
		sealed class PCETriggerRefCounter : TriggerRefCounterBase {
			internal PCETriggerRefCounter(IDictionary<object, TriggerRefCounterBase> list, EventTriggerBase trigger, object key)
				: base(list, trigger, key) {
			}
			protected override void UnregisterAction(ITriggerAction action, params object[] parameters) {
				IPropertyChangedTrigger pcTrigger = GetTrigger() as IPropertyChangedTrigger;
				if(pcTrigger != null)
					pcTrigger.Unregister(action);
			}
			protected override void RegisterAction(ITriggerAction action, params object[] parameters) {
				IPropertyChangedTrigger pcTrigger = GetTrigger() as IPropertyChangedTrigger;
				if(pcTrigger != null)
					pcTrigger.Register(action);
			}
			public override void ExecuteActions(string propertyName, ITriggerAction action) {
				IPropertyChangedTrigger pcTrigger = GetTrigger() as IPropertyChangedTrigger;
				if(pcTrigger != null)
					pcTrigger.Execute(action);
			}
		}
		#endregion Trigger Reference-Counting
		#region Trigger Actions
		internal static ITriggerAction CreateTriggerAction<TTarget, TValue>(TTarget target, Expression<Func<TTarget, TValue>> selectorExpression, Func<object, TValue> valueConverter = null) {
			return new TriggerAction<TTarget, TValue>(target, selectorExpression, valueConverter);
		}
		internal static ITriggerAction CreateTriggerAction<TTarget, T>(TTarget target, Expression<Action<T>> commandSelector) {
			return CreateTriggerAction(CommandHelper.GetCommandProperty(commandSelector, target));
		}
		internal static ITriggerAction CreateTriggerAction<TTarget>(TTarget target, Expression<Action> commandSelector) {
			return CreateTriggerAction(CommandHelper.GetCommandProperty(commandSelector, target));
		}
		internal static ITriggerAction CreateTriggerAction(object command) {
			return new CommandTriggerAction(command);
		}
		internal static ITriggerAction CreateTriggerAction<TValue>(Action<TValue> action) {
			return new DelegateTriggerAction<TValue>(action);
		}
		internal static ITriggerAction CreateCallbackTriggerAction<TValue>(Action<TValue> action, ITriggerAction parentAction) {
			return new CallbackTriggerAction<TValue>(action, parentAction);
		}
		sealed class TriggerAction<TTarget, TValue> : ITriggerAction {
			WeakReference tRef;
			Action<TTarget, TValue> setValue;
			Func<object, TValue> valueConverter;
			public TriggerAction(TTarget target, Expression<Func<TTarget, TValue>> selectorExpression, Func<object, TValue> valueConverter) {
				this.tRef = new WeakReference(target);
				this.valueConverter = valueConverter;
				var memberExpression = (MemberExpression)selectorExpression.Body;
				var setMethod = ((PropertyInfo)memberExpression.Member).GetSetMethod();
				if(setMethod != null) {
					var t = Expression.Parameter(typeof(TTarget), "t");
					var value = Expression.Parameter(typeof(TValue), "value");
					setValue = Expression.Lambda<Action<TTarget, TValue>>(
									Expression.Call(t, setMethod, value), t, value
								).Compile();
				}
			}
			bool ITriggerAction.CanExecute(object value) {
				if(setValue == null)
					return false;
				if(valueConverter != null)
					return true;
				if(object.ReferenceEquals(value, null))
					return typeof(TValue).IsClass;
				return value is TValue;
			}
			void ITriggerAction.Execute(object value) {
				TTarget target = (TTarget)tRef.Target;
				if(target != null) {
					executing++;
					if(valueConverter != null)
						value = valueConverter(value);
					setValue(target, (TValue)value);
					executing--;
				}
			}
			int executing;
			bool ITriggerAction.IsExecuting {
				get { return executing > 0; }
			}
			internal void ChangeTarget(object source) {
				tRef.Target = source;
			}
		}
		sealed class TriggerAction : ITriggerAction {
			WeakReference tRef;
			Type valueType;
			Action<object, object> setValue;
			Func<object, object> valueConverter;
			ITriggerAction parentTriggerAction;
			public TriggerAction(object target, Type targetType, PropertyInfo member, ITriggerAction parentTriggerAction, Func<object, object> valueConverter = null) {
				this.tRef = new WeakReference(target);
				this.valueConverter = valueConverter;
				this.parentTriggerAction = parentTriggerAction;
				this.valueType = member.PropertyType;
				var setMethod = member.GetSetMethod();
				if(setMethod != null) {
					var t = Expression.Parameter(typeof(object), "t");
					var value = Expression.Parameter(typeof(object), "value");
					setValue = Expression.Lambda<Action<object, object>>(
									Expression.Call(
										Expression.Convert(t, targetType), setMethod, Expression.Convert(value, valueType)
									), t, value).Compile();
				}
			}
			bool ITriggerAction.CanExecute(object value) {
				if(parentTriggerAction != null && parentTriggerAction.IsExecuting)
					return false;
				if(setValue == null)
					return false;
				if(valueConverter != null)
					return true;
				if(object.ReferenceEquals(value, null))
					return valueType.IsClass;
				return valueType.IsAssignableFrom(value.GetType());
			}
			void ITriggerAction.Execute(object value) {
				object target = tRef.Target;
				if(target != null) {
					executing++;
					if(valueConverter != null)
						value = valueConverter(value);
					setValue(target, value);
					executing--;
				}
			}
			int executing;
			bool ITriggerAction.IsExecuting {
				get { return executing > 0; }
			}
			internal void ChangeTarget(object target) {
				tRef.Target = target;
			}
		}
		sealed class DelegateTriggerAction<TValue> : ITriggerAction {
			Action<TValue> action;
			public DelegateTriggerAction(Action<TValue> action) {
				this.action = action;
			}
			bool ITriggerAction.CanExecute(object value) {
				if(action == null)
					return false;
				if(object.ReferenceEquals(value, null))
					return typeof(TValue).IsClass;
				return value is TValue;
			}
			void ITriggerAction.Execute(object value) {
				executing++;
				action((TValue)value);
				executing--;
			}
			int executing;
			bool ITriggerAction.IsExecuting {
				get { return executing > 0; }
			}
		}
		sealed class CallbackTriggerAction<TValue> : ITriggerAction {
			Action<TValue> action;
			ITriggerAction parentAction;
			public CallbackTriggerAction(Action<TValue> action, ITriggerAction parentAction) {
				this.parentAction = parentAction;
				this.action = action;
			}
			bool ITriggerAction.CanExecute(object value) {
				if(parentAction != null && parentAction.IsExecuting)
					return false;
				if(action == null)
					return false;
				if(object.ReferenceEquals(value, null))
					return typeof(TValue).IsClass;
				return value is TValue;
			}
			void ITriggerAction.Execute(object value) {
				executing++;
				action((TValue)value);
				executing--;
			}
			int executing;
			bool ITriggerAction.IsExecuting {
				get { return executing > 0; }
			}
		}
		sealed class CommandTriggerAction : ITriggerAction {
			object command;
			Func<object, object, bool> canExecute;
			Action<object, object> execute;
			public CommandTriggerAction(object command) {
				this.command = command;
				if(command != null)
					Initialize(command.GetType());
			}
			void Initialize(Type commandType) {
				var executeMethod = MemberInfoHelper.GetMethodInfo(commandType, "Execute", MemberInfoHelper.SingleObjectParameterTypes);
				if(executeMethod != null) {
					var commandObject = Expression.Parameter(typeof(object), "command");
					var parameter = Expression.Parameter(typeof(object), "parameter");
					var command = Expression.TypeAs(commandObject, commandType);
					this.execute = Expression.Lambda<Action<object, object>>(
								Expression.Call(command, executeMethod, parameter),
								commandObject, parameter
							).Compile();
					var canExecuteMethod = MemberInfoHelper.GetMethodInfo(commandType, "CanExecute", MemberInfoHelper.SingleObjectParameterTypes);
					if(canExecuteMethod != null) {
						this.canExecute = Expression.Lambda<Func<object, object, bool>>(
								Expression.Call(command, canExecuteMethod, parameter),
								commandObject, parameter
							).Compile();
					}
				}
			}
			bool ITriggerAction.CanExecute(object value) {
				return (execute != null) && ((canExecute == null) || canExecute(command, value));
			}
			void ITriggerAction.Execute(object value) {
				executing++;
				execute(command, value);
				executing--;
			}
			int executing;
			bool ITriggerAction.IsExecuting {
				get { return executing > 0; }
			}
		}
		#endregion Trigger Actions
	}
	#region Nested Properties
	static class NestedPropertiesHelper {
		public static bool HasRootPath(string path) {
			if(string.IsNullOrEmpty(path)) return false;
			return path.IndexOf('.') > 0;
		}
		public static string GetRootPath(ref string path) {
			if(string.IsNullOrEmpty(path)) return null;
			int pathSeparatorPos = path.IndexOf('.');
			if(pathSeparatorPos > 0) {
				string rootPath = path.Substring(0, pathSeparatorPos);
				path = path.Substring(pathSeparatorPos + 1);
				return rootPath;
			}
			return null;
		}
		public static PropertyDescriptor GetProperty(string path, ref object source, ref Type sourceType) {
			return GetProperty(path, TypeDescriptor.GetProperties(sourceType), ref source, ref sourceType);
		}
		public static PropertyDescriptor GetProperty(string path, PropertyDescriptorCollection properties, ref object source, ref Type sourceType) {
			if(string.IsNullOrEmpty(path)) return null;
			string rootPath = GetRootPath(ref path);
			if(!string.IsNullOrEmpty(rootPath)) {
				PropertyDescriptor rootDescriptor = properties[rootPath];
				if(rootDescriptor != null) {
					source = rootDescriptor.GetValue(source);
					sourceType = rootDescriptor.PropertyType;
					return GetProperty(path, rootDescriptor.GetChildProperties(), ref source, ref sourceType);
				}
				return GetCollectionItemProperty(path, properties, rootPath, ref source, ref sourceType);
			}
			return properties[path];
		}
		static PropertyDescriptor GetCollectionDescriptor(string path, PropertyDescriptorCollection properties, out int index) {
			index = -1;
			int openBracketPos = path.IndexOf('[');
			if(openBracketPos > 0) {
				string collectionPath = path.Substring(0, openBracketPos);
				int closeBracketPos = path.IndexOf(']');
				int indexPos = openBracketPos + 1;
				string indexStr = path.Substring(indexPos, closeBracketPos - indexPos);
				int.TryParse(indexStr, out index);
				return properties[collectionPath];
			}
			return null;
		}
		static PropertyDescriptor GetCollectionItemProperty(string path, PropertyDescriptorCollection properties, string rootPath, ref object source, ref Type sourceType) {
			int index;
			PropertyDescriptor collectionDescriptor = GetCollectionDescriptor(rootPath, properties, out index);
			if(collectionDescriptor != null) {
				Type collectionItemType = GetCollectionItemType(collectionDescriptor.PropertyType);
				if(collectionItemType != null) {
					PropertyDescriptorCollection itemProperties = TypeDescriptor.GetProperties((Type)collectionItemType);
					return GetProperty(path, itemProperties, ref source, ref sourceType);
				}
			}
			return null;
		}
		static Type GetCollectionItemType(Type collectionType) {
			if(collectionType == null) return null;
			if(collectionType.IsGenericType) {
				Type[] args = collectionType.GetGenericArguments();
				if(args.Length == 1)
					return args[0];
				return null;
			}
			return GetCollectionItemType(collectionType.BaseType);
		}
		internal static object GetSource(string path, object source, Type sourceType) {
			string rootPath = GetRootPath(ref path);
			if(!string.IsNullOrEmpty(rootPath)) {
				var properties = TypeDescriptor.GetProperties(sourceType);
				PropertyDescriptor rootDescriptor = properties[rootPath];
				if(rootDescriptor != null)
					return GetSource(path, rootDescriptor.GetValue(source), rootDescriptor.PropertyType);
			}
			return source;
		}
	}
	#endregion Nested Properties
	#region Triggers
	public class PropertyChangedTriggerActions : List<ITriggerAction> {
		internal void Execute(object value, ITriggerAction action = null) {
			if(action != null)
				ExecuteCore(action, value);
			else {
				using(var e = ((IList<ITriggerAction>)this).GetEnumerator()) {
					while(e.MoveNext())
						ExecuteCore(e.Current, value);
				}
			}
		}
		static void ExecuteCore(ITriggerAction action, object value) {
			if(action.CanExecute(value))
				action.Execute(value);
		}
	}
	abstract class PropertyChangedEventTriggerBase<TEventArgs> : EventTriggerBase<TEventArgs>, IPropertyChangedTrigger
		where TEventArgs : EventArgs {
		PropertyChangedTriggerActions actionsCore;
		protected PropertyChangedEventTriggerBase(string eventName)
			: base(eventName) {
			actionsCore = new PropertyChangedTriggerActions();
		}
		protected sealed override void OnEvent() {
			ExecuteActionsCore();
		}
		void ExecuteActionsCore(ITriggerAction action = null) {
			if(!CanProcessEvent()) return;
			object value = GetValue();
			actionsCore.Execute(value, action);
		}
		protected abstract bool CanProcessEvent();
		protected abstract object GetValue();
		#region IPropertyChangedTrigger
		void IPropertyChangedTrigger.Register(ITriggerAction action) {
			actionsCore.Add(action);
		}
		void IPropertyChangedTrigger.Unregister(ITriggerAction action) {
			actionsCore.Remove(action);
		}
		void IPropertyChangedTrigger.Execute(ITriggerAction action) {
			ExecuteActionsCore(action);
		}
		#endregion
	}
	sealed class PropertyChangedEventTrigger<TEventArgs, TValue> : PropertyChangedEventTriggerBase<TEventArgs>
		where TEventArgs : EventArgs {
		Func<TEventArgs, TValue> converter;
		public PropertyChangedEventTrigger(string eventName, Func<TEventArgs, TValue> converter)
			: base(eventName) {
			this.converter = converter;
		}
		protected override bool CanProcessEvent() {
			return Args != null && (converter != null);
		}
		protected sealed override object GetValue() {
			return converter(Args);
		}
		internal static object GetKey(string eventName) {
			return typeof(PropertyChangedEventTrigger<TEventArgs, TValue>).Name + "." + eventName;
		}
	}
	sealed class CLRPropertyChangedTrigger : PropertyChangedEventTriggerBase<EventArgs> {
		Expression<Func<object, object>> sourcePropertySelector;
		Func<object, object> accessor;
		public CLRPropertyChangedTrigger(Expression<Func<object, object>> sourcePropertySelector)
			: base(ExpressionHelper.GetPropertyName(sourcePropertySelector) + "Changed") {
			this.sourcePropertySelector = sourcePropertySelector;
		}
		protected override bool CanProcessEvent() {
			return (sourcePropertySelector != null);
		}
		protected sealed override object GetValue() {
			if(accessor == null)
				accessor = sourcePropertySelector.Compile();
			return accessor(Source);
		}
		internal static object GetKey(Expression<Func<object, object>> sourcePropertySelector) {
			return typeof(CLRPropertyChangedTrigger).Name + "." + ExpressionHelper.GetPropertyName(sourcePropertySelector) + "Changed";
		}
	}
	sealed class INPCPropertyChangedTrigger : EventTriggerBase<PropertyChangedEventArgs>, INotifyPropertyChangedTrigger {
		IDictionary<string, Func<object, object>> accessorsMap;
		IDictionary<string, PropertyChangedTriggerActions> actionsMap;
		public INPCPropertyChangedTrigger()
			: base("PropertyChanged") {
			actionsMap = new Dictionary<string, PropertyChangedTriggerActions>();
			accessorsMap = new Dictionary<string, Func<object, object>>();
		}
		protected sealed override void OnEvent() {
			ExecuteActionsCore(Args.PropertyName);
		}
		void ExecuteActionsCore(string propertyName, ITriggerAction action = null) {
			PropertyChangedTriggerActions actions;
			if((propertyName != null) && actionsMap.TryGetValue(propertyName, out actions)) {
				object value = accessorsMap[propertyName](Source);
				actions.Execute(value, action);
			}
		}
		#region INotifyPropertyChangedTrigger
		void INotifyPropertyChangedTrigger.Register(Expression<Func<object, object>> selectorExpression, ITriggerAction action) {
			string propertyName = ExpressionHelper.GetPropertyName(selectorExpression);
			Func<object, object> accessor;
			if(!accessorsMap.TryGetValue(propertyName, out accessor)) {
				accessor = selectorExpression.Compile();
				accessorsMap.Add(propertyName, accessor);
			}
			PropertyChangedTriggerActions actions;
			if(!actionsMap.TryGetValue(propertyName, out actions)) {
				actions = new PropertyChangedTriggerActions();
				actionsMap.Add(propertyName, actions);
			}
			actions.Add(action);
		}
		void INotifyPropertyChangedTrigger.Unregister(string propertyName, ITriggerAction action) {
			PropertyChangedTriggerActions actions;
			if(actionsMap.TryGetValue(propertyName, out actions))
				actions.Remove(action);
		}
		void INotifyPropertyChangedTrigger.Execute(string propertyName, ITriggerAction action) {
			ExecuteActionsCore(propertyName, action);
		}
		#endregion INotifyPropertyChangedTrigger
	}
	#endregion Triggers
	public static class ExpressionHelper {
		public static string GetPath<T>(Expression<Func<T>> expression) {
			return GetPath(expression);
		}
		public static string GetPropertyName<T>(Expression<Func<T>> expression) {
			return GetPropertyName((LambdaExpression)expression);
		}
		public static string GetPath(MemberExpression memberExpression, bool checkPathAndThrow = true) {
			Expression expression = memberExpression;
			var sb = new System.Text.StringBuilder();
			while(IsPropertyExpression(memberExpression)) {
				if(sb.Length > 0)
					sb.Insert(0, '.');
				sb.Insert(0, memberExpression.Member.Name);
				if(IsEndPathExpression(memberExpression.Expression))
					break;
				memberExpression = memberExpression.Expression as MemberExpression;
				if(memberExpression == null && checkPathAndThrow)
					throw new ArgumentException("The binding path must contain (nested) properties only: " + expression.ToString());
			}
			return sb.ToString();
		}
		public static string GetPath(LambdaExpression expression, bool checkPathAndThrow = true) {
			return GetPath(GetMemberExpression(expression), checkPathAndThrow);
		}
		public static string GetPropertyName(LambdaExpression expression) {
			MemberExpression memberExpression = GetMemberExpression(expression);
			if(IsPropertyExpression(memberExpression.Expression as MemberExpression))
				throw new ArgumentException("Expression: " + expression.ToString());
			return memberExpression.Member.Name;
		}
		static bool IsEndPathExpression(Expression expression) {
			if(expression is ParameterExpression || expression is ConstantExpression)
				return true;
			if(expression is UnaryExpression && ((UnaryExpression)expression).Operand is ConstantExpression)
				return true;
			return false;
		}
		static bool IsPropertyExpression(MemberExpression expression) {
			return (expression != null) && (expression.Member.MemberType == MemberTypes.Property);
		}
		static MemberExpression GetMemberExpression(LambdaExpression expression) {
			if(expression == null)
				throw new ArgumentNullException("expression");
			Expression body = expression.Body;
			if(body is UnaryExpression)
				body = ((UnaryExpression)body).Operand;
			MemberExpression memberExpression = body as MemberExpression;
			if(memberExpression == null)
				throw new ArgumentException("Expression: " + expression.ToString());
			return memberExpression;
		}
		internal static MemberInfo GetMember(LambdaExpression selector) {
			return GetMemberExpression(selector).Member;
		}
		static Expression CheckMemberType(MemberExpression accessor) {
			return accessor.Type.IsValueType ? (Expression)Expression.Convert(accessor, typeof(object)) : accessor;
		}
		internal static Expression<Func<object>> Accessor(Type sourceType, MemberInfo sourceProperty) {
			var source = Expression.Parameter(sourceType, "source");
			var accessor = Expression.MakeMemberAccess(source, sourceProperty);
			return Expression.Lambda<Func<object>>(CheckMemberType(accessor), source);
		}
		internal static Expression<Func<T, object>> Accessor<T>(Type sourceType, string sourceProperty) {
			return Accessor<T>(sourceType, sourceType.GetProperty(sourceProperty));
		}
		internal static Expression<Func<T, object>> Accessor<T>(Type sourceType, MemberInfo sourceProperty) {
			var s = Expression.Parameter(typeof(T), "s");
			var accessor = Expression.MakeMemberAccess(Expression.Convert(s, sourceType), sourceProperty);
			return Expression.Lambda<Func<T, object>>(CheckMemberType(accessor), s);
		}
		internal static Expression<Func<object, object>> Box<TSource>(Expression<Func<TSource, object>> memberExpression) {
			var source = Expression.Parameter(typeof(object), "source");
			var accessor = Expression.MakeMemberAccess(
				Expression.Convert(source, typeof(TSource)), GetMember(memberExpression));
			return Expression.Lambda<Func<object, object>>(CheckMemberType(accessor), source);
		}
		internal static Expression<Func<object, object>> Box<TSource, TValue>(Expression<Func<TSource, TValue>> memberExpression) {
			var source = Expression.Parameter(typeof(object), "source");
			var accessor = Expression.MakeMemberAccess(
				Expression.Convert(source, typeof(TSource)), GetMember(memberExpression));
			return Expression.Lambda<Func<object, object>>(CheckMemberType(accessor), source);
		}
		internal static Expression<Action> Reduce<TViewModel>(Expression<Action<TViewModel>> commandSelector, TViewModel viewModel) {
			return (commandSelector != null) ? new ReduceVisitor(commandSelector, GetInstanceExpression<TViewModel>(viewModel)).ReduceToAction() : null;
		}
		internal static Expression<Action<T>> Reduce<TViewModel, T>(Expression<Action<TViewModel, T>> commandSelector, TViewModel viewModel) {
			return (commandSelector != null) ? new ReduceVisitor<T>(commandSelector, GetInstanceExpression<TViewModel>(viewModel)).ReduceToAction() : null;
		}
		internal static Func<T> ReduceAndCompile<TViewModel, T>(Expression<Func<TViewModel, T>> commandParameterSelector, TViewModel viewModel) {
			return (commandParameterSelector != null) ? new ReduceVisitor<T>(commandParameterSelector, GetInstanceExpression<TViewModel>(viewModel)).ReduceToFunc().Compile() : null;
		}
		internal static Func<object> ReduceBoxAndCompile<TViewModel, TValue>(Expression<Func<TViewModel, TValue>> commandParameterSelector, TViewModel viewModel) {
			return (commandParameterSelector != null) ? new ReduceVisitor<object>(commandParameterSelector, GetInstanceExpression<TViewModel>(viewModel)).ReduceToFunc().Compile() : null;
		}
		static UnaryExpression GetInstanceExpression<TViewModel>(TViewModel viewModel) {
			return Expression.Convert(Expression.Constant(viewModel, typeof(TViewModel)), typeof(TViewModel));
		}
		#region Visitors
		class ReduceVisitor : ExpressionVisitor {
			protected LambdaExpression root;
			Expression expression;
			public ReduceVisitor(LambdaExpression root, Expression target) {
				this.root = root;
				this.expression = target;
			}
			protected override Expression VisitParameter(ParameterExpression p) {
				return (p == root.Parameters[0]) ? expression : base.VisitParameter(p);
			}
			internal Expression<Action> ReduceToAction() {
				return Expression.Lambda<Action>(Visit(root.Body));
			}
		}
		class ReduceVisitor<T> : ReduceVisitor {
			public ReduceVisitor(LambdaExpression root, Expression target)
				: base(root, target) {
			}
			internal Expression<Func<T>> ReduceToFunc() {
				return Expression.Lambda<Func<T>>(Visit(root.Body));
			}
			internal new Expression<Action<T>> ReduceToAction() {
				return Expression.Lambda<Action<T>>(Visit(root.Body), root.Parameters[1]);
			}
		}
		#endregion Visitors
	}
}
#if DEBUGTEST
namespace DevExpress.Utils.MVVM.Tests {
	using System;
	using System.ComponentModel;
	using System.Linq.Expressions;
	using NUnit.Framework;
	#region Test classes
	class TestBindingSource : TestEventSource, INotifyPropertyChanged {
		string nameCore;
		public string Name {
			get { return nameCore; }
			set {
				if(nameCore == value) return;
				nameCore = value;
				OnNameChanged();
			}
		}
		void OnNameChanged() {
			if(NameChanged != null)
				NameChanged(this, EventArgs.Empty);
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs("Name"));
		}
		public event EventHandler NameChanged;
		public event PropertyChangedEventHandler PropertyChanged;
	}
	class TestTarget {
		public virtual bool IsLoaded { get; set; }
		string nameCore;
		public string TargetName {
			get { return nameCore; }
			set {
				if(nameCore == value) return;
				nameCore = value;
				OnTargetNameChanged();
			}
		}
		protected virtual void OnTargetNameChanged() { }
	}
	class TestTarget_CLR : TestTarget {
		public event EventHandler TargetNameChanged;
		protected override void OnTargetNameChanged() {
			if(TargetNameChanged != null)
				TargetNameChanged(this, EventArgs.Empty);
		}
	}
	class TestTarget_NPC : TestTarget, INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		protected override void OnTargetNameChanged() {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs("TargetName"));
		}
	}
	class TestTarget_Event : TestTarget {
		bool loaded;
		public override bool IsLoaded {
			get { return loaded; }
			set {
				if(loaded == value) return;
				loaded = value;
				OnIsLoadedChanged();
			}
		}
		public event EventHandler IsLoadedChanged;
		protected virtual void OnIsLoadedChanged() {
			loadedChangedCount++;
			if(IsLoadedChanged != null)
				IsLoadedChanged(this, EventArgs.Empty);
		}
		internal int loadedChangedCount;
	}
	#endregion Test classes
	[TestFixture]
	public class BindingHelperTests {
		[Test]
		public void Test00_PropertyChangedEventTrigger_SetTrigger() {
			TestBindingSource source = new TestBindingSource();
			TestTarget target = new TestTarget();
			Assert.IsFalse(target.IsLoaded);
			using(BindingHelper.SetPCETriggerCore<EventArgs, bool>(source, "Loaded", (e) => true, BindingHelper.CreateTriggerAction(target, x => x.IsLoaded))) {
				source.RaiseLoaded();
				Assert.IsTrue(target.IsLoaded);
			}
		}
		[Test]
		public void Test00_CLRPropertyChangedTrigger_SetTrigger() {
			TestBindingSource source = new TestBindingSource();
			TestTarget target = new TestTarget();
			using(BindingHelper.SetTrigger(source, (s) => s.Name, BindingHelper.CreateTriggerAction(target, x => x.TargetName), true)) {
				source.Name = "Test";
				Assert.AreEqual(source.Name, target.TargetName);
			}
		}
		[Test]
		public void Test00_INPCPropertyChangedTrigger_SetTrigger() {
			TestBindingSource source = new TestBindingSource();
			TestTarget target = new TestTarget();
			using(BindingHelper.SetTrigger(source, (s) => s.Name, BindingHelper.CreateTriggerAction(target, x => x.TargetName), false)) {
				source.Name = "Test";
				Assert.AreEqual(source.Name, target.TargetName);
			}
		}
		[Test]
		public void Test01_SetBinding_OneWay() {
			TestBindingSource source = new TestBindingSource() { Name = "Start" };
			TestTarget target = new TestTarget();
			using(BindingHelper.SetBinding(target, (t) => t.TargetName, source, typeof(TestBindingSource), "Name")) {
				Assert.AreEqual(source.Name, target.TargetName);
				source.Name = "Test";
				Assert.AreEqual(source.Name, target.TargetName);
				target.TargetName = "Name";
				Assert.AreNotEqual(source.Name, target.TargetName);
			}
		}
		[Test]
		public void Test01_SetBinding_OneWay_Event() {
			TestBindingSource source = new TestBindingSource();
			TestTarget target = new TestTarget();
			Assert.IsFalse(target.IsLoaded);
			using(BindingHelper.SetBinding<EventArgs, TestTarget, bool>(target, x => x.IsLoaded, source, "Loaded", (e) => true)) {
				Assert.IsFalse(target.IsLoaded);
				source.RaiseLoaded();
				Assert.IsTrue(target.IsLoaded);
			}
		}
		[Test]
		public void Test01_SetBinding_TwoWay_NPC() {
			TestBindingSource source = new TestBindingSource() { Name = "Start" };
			TestTarget target = new TestTarget_NPC();
			using(BindingHelper.SetBinding(target, (t) => t.TargetName, source, typeof(TestBindingSource), "Name")) {
				Assert.AreEqual(source.Name, target.TargetName);
				source.Name = "Test";
				Assert.AreEqual(source.Name, target.TargetName);
				target.TargetName = "Name";
				Assert.AreEqual(target.TargetName, source.Name);
			}
		}
		[Test]
		public void Test01_SetBinding_TwoWay_CLR() {
			TestBindingSource source = new TestBindingSource() { Name = "Start" };
			TestTarget target = new TestTarget_CLR();
			using(BindingHelper.SetBinding(target, (t) => t.TargetName, source, typeof(TestBindingSource), "Name")) {
				Assert.AreEqual(source.Name, target.TargetName);
				source.Name = "Test";
				Assert.AreEqual(source.Name, target.TargetName);
				target.TargetName = "Name";
				Assert.AreEqual(target.TargetName, source.Name);
			}
		}
		[Test]
		public void Test01_SetBinding_TwoWayEventFromSource() {
			TestBindingSource source = new TestBindingSource();
			TestTarget_Event target = new TestTarget_Event();
			int callbackCount = 0;
			using(BindingHelper.SetBinding<EventArgs, TestBindingSource, TestTarget_Event, bool>(target, x => x.IsLoaded, source, "Loaded",
				(e) => true, (s, v) => callbackCount++)) {
				Assert.IsFalse(target.IsLoaded);
				Assert.AreEqual(1, callbackCount);
				Assert.AreEqual(0, source.eventCount);
				Assert.AreEqual(0, target.loadedChangedCount);
				source.RaiseLoaded();
				Assert.IsTrue(target.IsLoaded);
				Assert.AreEqual(1, callbackCount);
				Assert.AreEqual(1, source.eventCount);
				Assert.AreEqual(1, target.loadedChangedCount);
				target.IsLoaded = false;
				Assert.AreEqual(2, callbackCount);
				Assert.AreEqual(1, source.eventCount);
				Assert.AreEqual(2, target.loadedChangedCount);
			}
		}
	}
	[TestFixture]
	public class BindingHelperTests_MultiTargetBinding {
		IPropertyBinding binding1;
		IPropertyBinding binding2;
		[TearDown]
		public void TearDown() {
			Ref.Dispose(ref binding1);
			Ref.Dispose(ref binding2);
		}
		[Test]
		public void Test01_SetBinding_OneWay_MultiTarget() {
			TestBindingSource source = new TestBindingSource() { Name = "Start" };
			TestTarget target1 = new TestTarget();
			TestTarget target2 = new TestTarget();
			binding1 = BindingHelper.SetBinding(target1, (t) => t.TargetName, source, typeof(TestBindingSource), "Name");
			binding2 = BindingHelper.SetBinding(target2, (t) => t.TargetName, source, typeof(TestBindingSource), "Name");
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreEqual(source.Name, target2.TargetName);
			source.Name = "Test";
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreEqual(source.Name, target2.TargetName);
			target1.TargetName = "Name1";
			Assert.AreNotEqual(source.Name, target1.TargetName);
			target2.TargetName = "Name2";
			Assert.AreNotEqual(source.Name, target2.TargetName);
		}
		[Test]
		public void Test01_SetBinding_OneWay_MultiTarget_Unbind() {
			TestBindingSource source = new TestBindingSource() { Name = "Start" };
			TestTarget target1 = new TestTarget();
			TestTarget target2 = new TestTarget();
			binding1 = BindingHelper.SetBinding(target1, (t) => t.TargetName, source, typeof(TestBindingSource), "Name");
			binding2 = BindingHelper.SetBinding(target2, (t) => t.TargetName, source, typeof(TestBindingSource), "Name");
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreEqual(source.Name, target2.TargetName);
			source.Name = "Test";
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreEqual(source.Name, target2.TargetName);
			Ref.Dispose(ref binding2);
			source.Name = "Test2";
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreNotEqual(source.Name, target2.TargetName);
		}
		[Test]
		public void Test02_SetBinding_OneWay_Event_MultiTarget() {
			TestBindingSource source = new TestBindingSource();
			TestTarget target1 = new TestTarget();
			TestTarget target2 = new TestTarget();
			Assert.IsFalse(target1.IsLoaded);
			Assert.IsFalse(target2.IsLoaded);
			binding1 = BindingHelper.SetBinding<EventArgs, TestTarget, bool>(target1, x => x.IsLoaded, source, "Loaded", (e) => true);
			binding2 = BindingHelper.SetBinding<EventArgs, TestTarget, bool>(target2, x => x.IsLoaded, source, "Loaded", (e) => true);
			Assert.IsFalse(target1.IsLoaded);
			Assert.IsFalse(target2.IsLoaded);
			source.RaiseLoaded();
			Assert.IsTrue(target1.IsLoaded);
			Assert.IsTrue(target2.IsLoaded);
		}
		[Test]
		public void Test02_SetBinding_OneWay_Event_MultiTarget_Unbind() {
			TestBindingSource source = new TestBindingSource();
			TestTarget target1 = new TestTarget();
			TestTarget target2 = new TestTarget();
			Assert.IsFalse(target1.IsLoaded);
			Assert.IsFalse(target2.IsLoaded);
			binding1 = BindingHelper.SetBinding<EventArgs, TestTarget, bool>(target1, x => x.IsLoaded, source, "Loaded", (e) => true);
			binding2 = BindingHelper.SetBinding<EventArgs, TestTarget, bool>(target2, x => x.IsLoaded, source, "Loaded", (e) => true);
			Assert.IsFalse(target1.IsLoaded);
			Assert.IsFalse(target2.IsLoaded);
			source.RaiseLoaded();
			Assert.IsTrue(target1.IsLoaded);
			Assert.IsTrue(target2.IsLoaded);
			target1.IsLoaded = false;
			target2.IsLoaded = false;
			Ref.Dispose(ref binding2);
			source.RaiseLoaded();
			Assert.IsTrue(target1.IsLoaded);
			Assert.IsFalse(target2.IsLoaded);
		}
		[Test]
		public void Test02_SetBinding_TwoWay_NPC_MultiTarget() {
			TestBindingSource source = new TestBindingSource() { Name = "Start" };
			TestTarget target1 = new TestTarget_NPC();
			TestTarget target2 = new TestTarget_NPC();
			binding1 = BindingHelper.SetBinding(target1, (t) => t.TargetName, source, typeof(TestBindingSource), "Name");
			binding2 = BindingHelper.SetBinding(target2, (t) => t.TargetName, source, typeof(TestBindingSource), "Name");
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreEqual(source.Name, target2.TargetName);
			source.Name = "Test";
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreEqual(source.Name, target2.TargetName);
			target1.TargetName = "Name";
			Assert.AreEqual(target1.TargetName, source.Name);
			Assert.AreEqual(target1.TargetName, target2.TargetName);
		}
		[Test]
		public void Test02_SetBinding_TwoWay_NPC_MultiTarget_Unbing() {
			TestBindingSource source = new TestBindingSource() { Name = "Start" };
			TestTarget target1 = new TestTarget_NPC();
			TestTarget target2 = new TestTarget_NPC();
			binding1 = BindingHelper.SetBinding(target1, (t) => t.TargetName, source, typeof(TestBindingSource), "Name");
			binding2 = BindingHelper.SetBinding(target2, (t) => t.TargetName, source, typeof(TestBindingSource), "Name");
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreEqual(source.Name, target2.TargetName);
			source.Name = "Test";
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreEqual(source.Name, target2.TargetName);
			target1.TargetName = "Name";
			Assert.AreEqual(target1.TargetName, source.Name);
			Assert.AreEqual(target1.TargetName, target2.TargetName);
			Ref.Dispose(ref binding2);
			source.Name = "Test2";
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreNotEqual(source.Name, target2.TargetName);
			Assert.AreNotEqual(target1.TargetName, target2.TargetName);
		}
		[Test]
		public void Test02_SetBinding_TwoWay_CLR_MultiTarget() {
			TestBindingSource source = new TestBindingSource() { Name = "Start" };
			TestTarget target1 = new TestTarget_CLR();
			TestTarget target2 = new TestTarget_CLR();
			binding1 = BindingHelper.SetBinding(target1, (t) => t.TargetName, source, typeof(TestBindingSource), "Name");
			binding2 = BindingHelper.SetBinding(target2, (t) => t.TargetName, source, typeof(TestBindingSource), "Name");
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreEqual(source.Name, target2.TargetName);
			source.Name = "Test";
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreEqual(source.Name, target2.TargetName);
			target1.TargetName = "Name";
			Assert.AreEqual(target1.TargetName, source.Name);
			Assert.AreEqual(target1.TargetName, target2.TargetName);
		}
		[Test]
		public void Test02_SetBinding_TwoWay_CLR_MultiTarget_Unbind() {
			TestBindingSource source = new TestBindingSource() { Name = "Start" };
			TestTarget target1 = new TestTarget_CLR();
			TestTarget target2 = new TestTarget_CLR();
			binding1 = BindingHelper.SetBinding(target1, (t) => t.TargetName, source, typeof(TestBindingSource), "Name");
			binding2 = BindingHelper.SetBinding(target2, (t) => t.TargetName, source, typeof(TestBindingSource), "Name");
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreEqual(source.Name, target2.TargetName);
			source.Name = "Test";
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreEqual(source.Name, target2.TargetName);
			target1.TargetName = "Name";
			Assert.AreEqual(target1.TargetName, source.Name);
			Assert.AreEqual(target1.TargetName, target2.TargetName);
			Ref.Dispose(ref binding2);
			source.Name = "Test2";
			Assert.AreEqual(source.Name, target1.TargetName);
			Assert.AreNotEqual(source.Name, target2.TargetName);
			Assert.AreNotEqual(target1.TargetName, target2.TargetName);
		}
	}
	[TestFixture]
	public class BindingHelperTests_Dependencies {
		#region TestClasses
		class Control {
			public Control() {
				isEnabled = true;
			}
			bool isEnabled;
			public bool Enabled {
				get { return isEnabled; }
				set {
					if(isEnabled == value) return;
					isEnabled = value;
					if(EnabledChanged != null)
						EnabledChanged(this, EventArgs.Empty);
				}
			}
			public event EventHandler EnabledChanged;
		}
		class TextEditor : Control {
			string editValue;
			public string EditValue {
				get { return editValue; }
				set {
					if(editValue == value) return;
					editValue = value;
					if(EditValueChanged != null)
						EditValueChanged(this, EventArgs.Empty);
				}
			}
			public event EventHandler EditValueChanged;
		}
		class CheckEditor : Control {
			bool isChecked;
			public bool Checked {
				get { return isChecked; }
				set {
					if(isChecked == value) return;
					isChecked = value;
					if(CheckedChanged != null)
						CheckedChanged(this, EventArgs.Empty);
				}
			}
			public event EventHandler CheckedChanged;
		}
		class ViewModel : INotifyPropertyChanged {
			string textCore;
			public string Text {
				get { return textCore; }
				set {
					if(textCore == value) return;
					textCore = value;
					RaisePropertyChanged("Text");
				}
			}
			bool isActiveCore;
			public bool IsActive {
				get { return isActiveCore; }
				set {
					if(isActiveCore == value) return;
					isActiveCore = value;
					RaisePropertyChanged("IsActive");
				}
			}
			public event PropertyChangedEventHandler PropertyChanged;
			void RaisePropertyChanged(string name) {
				if(PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
		#endregion TestClasses
		DisposableObjectsContainer c;
		[SetUp]
		public void SetUp() {
			c = new DisposableObjectsContainer();
		}
		[TearDown]
		public void TearDown() {
			Ref.Dispose(ref c);
		}
		[Test]
		public void Test00_T224520() {
			CheckEditor cEdit = new CheckEditor();
			TextEditor tEdit = new TextEditor();
			ViewModel vm = new ViewModel();
			c.Register(BindingHelper.SetBinding(cEdit, ce => ce.Checked, vm, typeof(ViewModel), "IsActive"));
			c.Register(BindingHelper.SetBinding(tEdit, t => t.Enabled, vm, typeof(ViewModel), "IsActive"));
			c.Register(BindingHelper.SetBinding(tEdit, t => t.EditValue, vm, typeof(ViewModel), "Text"));
			Assert.IsFalse(cEdit.Checked);
			Assert.IsFalse(tEdit.Enabled);
			Assert.IsNull(tEdit.EditValue);
			vm.IsActive = true;
			Assert.IsTrue(cEdit.Checked);
			Assert.IsTrue(tEdit.Enabled);
			Assert.IsNull(tEdit.EditValue);
			tEdit.EditValue = "!!!";
			Assert.AreEqual("!!!", vm.Text);
		}
	}
	[TestFixture]
	public class BindingHelperTests_NestedBinding {
		#region TestClasses
		class ViewModelBase : INotifyPropertyChanged {
			protected void Set<T>(System.Linq.Expressions.Expression<Func<T>> selector, ref T field, T value) {
				if(object.Equals(field, value)) return;
				field = value;
				RaisePropertyChanged(ExpressionHelper.GetPropertyName(selector));
			}
			protected void RaisePropertyChanged(string propertyName) {
				PropertyChangedEventHandler handler = PropertyChanged;
				if(handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
			}
			public event PropertyChangedEventHandler PropertyChanged;
		}
		class ChildViewModel : ViewModelBase {
			string nameCore;
			public string Name {
				get { return nameCore; }
				set { Set(() => Name, ref nameCore, value); }
			}
		}
		class ParentViewModel : ViewModelBase {
			ChildViewModel childCore;
			public ChildViewModel Child {
				get { return childCore; }
				set { Set(() => Child, ref childCore, value); }
			}
			public ChildViewModel GetChild() { return childCore; }
		}
		class ScopeViewModel : ViewModelBase {
			ParentViewModel nodeCore;
			public ParentViewModel Scope {
				get { return nodeCore; }
				set { Set(() => Scope, ref nodeCore, value); }
			}
		}
		class Target {
			public string Name { get; set; }
		}
		class SourceWithEvent {
			public event EventHandler<SourceNameEventArgs> NameChanged;
			string nameStr;
			public string NameStr {
				get { return nameStr; }
				set {
					if(nameStr == value) return;
					nameStr = value;
					RaiseNameChanged();
				}
			}
			internal void RaiseNameChanged() {
				if(NameChanged != null) NameChanged(this, new SourceNameEventArgs(nameStr));
			}
		}
		class SourceNameEventArgs : EventArgs {
			public SourceNameEventArgs(string str) {
				this.Str = str;
			}
			public string Str { get; private set; }
		}
		#endregion TestClasses
		[Test]
		public void Test00_NestedUpdateProperty() {
			Target target = new Target();
			ParentViewModel parent = new ParentViewModel()
			{
				Child = new ChildViewModel() { Name = "Child" }
			};
			using(BindingHelper.SetBinding(target, (t) => t.Name, parent, typeof(ParentViewModel), "Child.Name")) {
				Assert.AreEqual("Child", target.Name);
				parent.Child.Name = "Test";
				Assert.AreEqual("Test", target.Name);
			}
		}
		[Test]
		public void Test00_NestedUpdateSource() {
			Target target = new Target();
			ParentViewModel parent = new ParentViewModel()
			{
				Child = new ChildViewModel() { Name = "Child" }
			};
			using(var b = BindingHelper.SetBinding(target, (t) => t.Name, parent, typeof(ParentViewModel), "Child.Name")) {
				Assert.AreEqual("Child", target.Name);
				parent.Child = new ChildViewModel() { Name = "Test" };
				Assert.AreEqual("Test", target.Name);
			}
		}
		[Test]
		public void Test00_NestedUpdateSource_MultiLevel() {
			Target target = new Target();
			ScopeViewModel root = new ScopeViewModel()
			{
				Scope = new ParentViewModel()
				{
					Child = new ChildViewModel() { Name = "Child" },
				}
			};
			using(var b = BindingHelper.SetBinding(target, (t) => t.Name, root, typeof(ScopeViewModel), "Scope.Child.Name")) {
				Assert.AreEqual("Child", target.Name);
				root.Scope.Child.Name = "Test";
				Assert.AreEqual("Test", target.Name);
				root.Scope.Child = new ChildViewModel() { Name = "Child2" };
				Assert.AreEqual("Child2", target.Name);
				root.Scope = new ParentViewModel() { Child = new ChildViewModel() { Name = "Child3" } };
				Assert.AreEqual("Child3", target.Name);
			}
		}
		[Test]
		public void Test00_NestedUpdateSource_LazyInitialization_SingleLevel() {
			Target target = new Target();
			ParentViewModel parent = new ParentViewModel();
			using(var b = BindingHelper.SetBinding(target, (t) => t.Name, parent, typeof(ParentViewModel), "Child.Name")) {
				Assert.IsNull(target.Name);
				parent.Child = new ChildViewModel() { Name = "Child" };
				Assert.AreEqual("Child", target.Name);
				parent.Child.Name = "Test";
				Assert.AreEqual("Test", target.Name);
				parent.Child = new ChildViewModel() { Name = "Child2" };
				Assert.AreEqual("Child2", target.Name);
			}
		}
		[Test]
		public void Test00_NestedUpdateProperty_EventFromSource_OnweWay() {
			SourceWithEvent source = new SourceWithEvent();
			ParentViewModel parent = new ParentViewModel()
			{
				Child = new ChildViewModel() { Name = "Child" }
			};
			using(BindingHelper.SetBinding<SourceNameEventArgs, ParentViewModel, string>(parent, (p) => p.Child.Name,
				source, "NameChanged", (e) => e.Str)) {
				Assert.IsNull(source.NameStr);
				Assert.AreEqual("Child", parent.Child.Name);
				source.RaiseNameChanged();
				Assert.IsNull(parent.Child.Name);
			}
		}
		[Test]
		public void Test00_NestedUpdateProperty_EventFromSource_OnweWay_Lazy() {
			SourceWithEvent source = new SourceWithEvent();
			ParentViewModel parent = new ParentViewModel();
			using(BindingHelper.SetBinding<SourceNameEventArgs, ParentViewModel, string>(parent, (p) => p.Child.Name,
				source, "NameChanged", (e) => e.Str)) {
				Assert.IsNull(source.NameStr);
				parent.Child = new ChildViewModel() { Name = "Child" };
				source.RaiseNameChanged();
				Assert.IsNull(parent.Child.Name);
			}
		}
		[Test]
		public void Test00_NestedUpdateProperty_EventFromSource_TwoWay() {
			SourceWithEvent source = new SourceWithEvent();
			ParentViewModel parent = new ParentViewModel()
			{
				Child = new ChildViewModel() { Name = "Child" }
			};
			using(BindingHelper.SetBinding<SourceNameEventArgs, SourceWithEvent, ParentViewModel, string>(parent, (p) => p.Child.Name,
				source, "NameChanged", (e) => e.Str, (src, value) => src.NameStr = value)) {
				Assert.AreEqual("Child", source.NameStr);
				parent.Child.Name = "Test";
				Assert.AreEqual("Test", source.NameStr);
				source.NameStr = "Child";
				Assert.AreEqual("Child", parent.Child.Name);
			}
		}
		[Test]
		public void Test00_NestedUpdateProperty_EventFromSource_TwoWay_MultiLevel() {
			SourceWithEvent source = new SourceWithEvent();
			ParentViewModel parent = new ParentViewModel()
			{
				Child = new ChildViewModel() { Name = "Child" }
			};
			using(BindingHelper.SetBinding<SourceNameEventArgs, SourceWithEvent, ParentViewModel, string>(parent, (p) => p.Child.Name,
				source, "NameChanged", (e) => e.Str, (src, value) => src.NameStr = value)) {
				Assert.AreEqual("Child", source.NameStr);
				parent.Child = new ChildViewModel() { Name = "Test" };
				Assert.AreEqual("Test", source.NameStr);
				source.NameStr = "Child";
				Assert.AreEqual("Child", parent.Child.Name);
			}
		}
		[Test]
		public void Test00_NestedUpdateProperty_EventFromSource_TwoWay_Lazy() {
			SourceWithEvent source = new SourceWithEvent();
			ParentViewModel parent = new ParentViewModel();
			using(BindingHelper.SetBinding<SourceNameEventArgs, SourceWithEvent, ParentViewModel, string>(parent, (p) => p.Child.Name,
				source, "NameChanged", (e) => e.Str, (src, value) => src.NameStr = value)) {
				Assert.IsNull(source.NameStr);
				parent.Child = new ChildViewModel() { Name = "Test" };
				Assert.AreEqual("Test", source.NameStr);
			}
		}
		[Test]
		public void Test00_Nested_Throw_When_Illegal_Path() {
			try {
				Expression<Func<ParentViewModel, string>> legalPath = x => x.Child.Name;
				ExpressionHelper.GetPath(legalPath);
			}
			catch(ArgumentException) { Assert.Fail(); }
			try {
				Expression<Func<ParentViewModel, string>> illegalPath = x => x.GetChild().Name;
				ExpressionHelper.GetPath(illegalPath);
				Assert.Fail();
			}
			catch(ArgumentException) { }
		}
	}
}
#endif
