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
using System.Linq;
using DevExpress.Mvvm.Native;
using System.ComponentModel;
using System.Collections.Generic;
namespace DevExpress.Xpf.Core.Native {
	public interface IRenderPropertyContext {
		void Attach(INamescope scope, IElementHost elementHost, IPropertyChangedListener listener, RenderTriggerContextBase context);
		void Detach();
		void Reset();
	}
	public interface IRenderConditionContext : IRenderPropertyContext {
		bool IsValid { get; }
	}
	public abstract class RenderPropertyContextBase : IRenderPropertyContext {
		public RenderPropertyBase Factory { get; private set; }
		public IElementHost ElementHost { get; private set; }
		public INamescope Namescope { get; private set; }
		public IPropertyChangedListener PropertyChangedListener { get; private set; }
		public RenderTriggerContextBase TriggerContext { get; private set; }
		protected bool IsAttached { get; private set; }
		protected RenderPropertyContextBase(RenderPropertyBase factory) {
			this.Factory = factory;
		}
		public void Attach(INamescope scope, IElementHost elementHost, IPropertyChangedListener listener, RenderTriggerContextBase context) {
			ElementHost = elementHost;
			Namescope = scope;
			PropertyChangedListener = listener;
			TriggerContext = context;
			AttachOverride(scope, elementHost, listener, context);
			IsAttached = true;
		}
		public void Detach() {
			if (!IsAttached)
				return;
			IsAttached = false;
			DetachOverride();
			ElementHost = null;
			PropertyChangedListener = null;
			Namescope = null;
			TriggerContext = null;			
		}
		public virtual void Reset() {
			var namescope = Namescope;
			var elementHost = ElementHost;
			var triggetContext = TriggerContext;
			var listener = PropertyChangedListener;
			Detach();
			Attach(namescope, elementHost, listener, triggetContext);
			triggetContext.Invalidate();
		}
		protected abstract void AttachOverride(INamescope scope, IElementHost elementHost, IPropertyChangedListener listener, RenderTriggerContextBase context);
		protected abstract void DetachOverride();
	}
	public class RenderPropertyChangedListenerContext : RenderPropertyContextBase {
		public new RenderPropertyChangedListener Factory { get { return base.Factory as RenderPropertyChangedListener; } }
		public DependencyPropertyDescriptor Descriptor { get; private set; }
		protected object CurrentDescriptorSource { get; private set; }
		protected string CurrentPropertyName { get; private set; }
		protected bool IsInitialized { get; private set; }
		public bool HasDescriptor { get { return Descriptor != null; } }
		public RenderPropertyChangedListenerContext(RenderPropertyChangedListener factory)
			: base(factory) {
		}
		protected override void AttachOverride(INamescope scope, IElementHost elementHost, IPropertyChangedListener listener, RenderTriggerContextBase context) {
			InitializeDescriptorSource();
			InitializeDescriptor();			
			listener.SubscribeValueChangedAsync(this);
		}
		void InitializeDescriptorSource() {
			CurrentDescriptorSource = GetDescriptorSource(ElementHost);
		}
		public void SubscribeValue() {
			SubscribeValueChanged(CurrentDescriptorSource);
		}
		public void UnsubscribeValue() {
			UnsubscribeValueChanged(CurrentDescriptorSource);
		}
		protected override void DetachOverride() {
			PropertyChangedListener.UnsubscribeValueChanged(CurrentDescriptorSource, this);
			UnInitializeDescriptor();
			UnInitializeDescriptorSource();
		}
		public void InitializeDescriptor() {
			if (IsInitialized)
				return;
			InitializeDescriptorOverride();
			IsInitialized = true;
		}
		public void UnInitializeDescriptor() {
			Descriptor = null;
			IsInitialized = false;
		}
		public void UnInitializeDescriptorSource() {
			CurrentDescriptorSource = null;
		}
		protected virtual void InitializeDescriptorOverride() {
			if (CurrentDescriptorSource == null || (Equals(Factory.Property, null) && Equals(Factory.DependencyProperty, null))) {
				CurrentPropertyName = null;
				Descriptor = null;
				return;
			}
			var type = CurrentDescriptorSource.GetType();
			CurrentPropertyName = Factory.Property;
			Descriptor = Factory.DependencyProperty == null ? DependencyPropertyDescriptor.FromName(Factory.Property, type, type) : DependencyPropertyDescriptor.FromProperty(Factory.DependencyProperty, type);
		}
		protected virtual object GetDescriptorSource(IElementHost elementHost) {
			object result = null;
			bool throwIfNull = true;
			switch (Factory.ValueSource) {
				case RenderValueSource.DataContext:
					result = (Factory.TargetName.With(x => Namescope.GetElement(x)) ?? Namescope.RootElement).With(x => x.DataContext);
					throwIfNull = false;
					break;
				case RenderValueSource.ElementName:
					result = Factory.SourceName.With(x => Namescope.GetElement(x));
					break;
				case RenderValueSource.TemplatedParent:
					result = elementHost.TemplatedParent;
					throwIfNull = false;
					break;
				default:
					throw new NotImplementedException();
			}
			if (result == null && throwIfNull)
				throw new ArgumentException(String.Format("Cannot find element with name '{0}'", Factory.TargetName));
			else
				return result;
		}
		protected void SubscribeValueChanged(object target) {
			CurrentDescriptorSource = target;
			if (target == null)
				return;
			PropertyChangedListener.SubscribeValueChanged(target, this);
		}
		protected void UnsubscribeValueChanged(object target) {
			if (target == null)
				return;
			PropertyChangedListener.UnsubscribeValueChanged(target, this);
		}
		public virtual void PreviewValueChanged(object sender, EventArgs args) { }
		public virtual void ValueChanged(object sender, EventArgs args) {
			TriggerContext.Invalidate();
		}
		public object GetValue() {
			return Descriptor.Return(x => x.GetValue(CurrentDescriptorSource), () => CurrentDescriptorSource.If(s => Factory.Property == null && Factory.DependencyProperty == null));
		}
		public override void Reset() {
			if (GetDescriptorSource(ElementHost) == CurrentDescriptorSource)
				return;
			base.Reset();
		}
	}
	public class ValueChangedStorage {
		#region innerClasses
		struct ValueChangedStorageKey {
			readonly WeakReference targetReference;
			readonly DependencyPropertyDescriptor descriptor;
			readonly int hCode;
			public object Target { get { return targetReference.With(x => x.Target); } }
			public DependencyPropertyDescriptor Descriptor { get { return descriptor; } }
			public ValueChangedStorageKey(object target, DependencyPropertyDescriptor descriptor) {
				targetReference = new WeakReference(target);
				this.descriptor = descriptor;
				hCode = (target.GetHashCode() * 397) ^ descriptor.GetHashCode();
			}
			public override bool Equals(object obj) {
				return obj.GetHashCode() == hCode;
			}
			public override int GetHashCode() {
				return hCode;
			}
		}
		class ValueChangedStorageRecord : IDisposable {
			readonly object target;
			readonly DependencyPropertyDescriptor descriptor;
			readonly EventHandler handler;
			readonly List<RenderPropertyChangedListenerContext> contexts = new List<RenderPropertyChangedListenerContext>();			
			public bool IsActive { get { return contexts.Count > 0; } }
			public ValueChangedStorageRecord(object target, DependencyPropertyDescriptor descriptor) {
				this.target = target;
				this.descriptor = descriptor;
				handler = new EventHandler(OnValueChanged);
				descriptor.AddValueChanged(target, handler);				
			}
			void OnValueChanged(object sender, EventArgs args) {
				var invocationList = contexts.ToList();
				foreach (var context in invocationList) {
					context.PreviewValueChanged(sender, args);
				}
				foreach (var context in invocationList) {
					context.ValueChanged(sender, args);
				}
			}
			public void AddListener(RenderPropertyChangedListenerContext context) {				
				contexts.Add(context);
				OnValueChanged(target, EventArgs.Empty);
			}
			public void RemoveListener(RenderPropertyChangedListenerContext context) {
				contexts.Remove(context);
			}
			public void Dispose() {
				contexts.Clear();
				descriptor.RemoveValueChanged(target, handler);
			}
		}
		#endregion
		readonly Dictionary<ValueChangedStorageKey, ValueChangedStorageRecord> handlers;
		public ValueChangedStorage() {
			handlers = new Dictionary<ValueChangedStorageKey, ValueChangedStorageRecord>();
		}
		public void SubscribeValueChanged(object target, RenderPropertyChangedListenerContext context) {
			var descriptor = context.Descriptor;
			if (descriptor == null)
				return;
			var key = new ValueChangedStorageKey(target, descriptor);
			ValueChangedStorageRecord record = null;
			if (!handlers.TryGetValue(key, out record)) {
				record = new ValueChangedStorageRecord(target, descriptor);
				handlers[key] = record;
			}
			record.AddListener(context);
		}
		public void UnsubscribeValueChanged(object target, RenderPropertyChangedListenerContext context) {
			DependencyPropertyDescriptor descriptor = context.Descriptor;
			if (descriptor == null)
				return;
			var key = new ValueChangedStorageKey(target, descriptor);
			ValueChangedStorageRecord record = null;
			if (handlers.TryGetValue(key, out record)) {
				record.RemoveListener(context);
				if (!record.IsActive) {
					record.Dispose();
					handlers.Remove(key);
				}
			}
		}
	}
	public class RenderConditionContext : RenderPropertyChangedListenerContext, IRenderConditionContext {
		public new RenderCondition Factory { get { return base.Factory as RenderCondition; } }
		public bool IsValid { get; private set; }
		public object ConvertedValue { get; private set; }
		public RenderConditionContext(RenderCondition factory)
			: base(factory) {
			this.IsValid = Factory.FallbackIsValid;
		}
		protected override void AttachOverride(INamescope scope, IElementHost elementHost, IPropertyChangedListener listener, RenderTriggerContextBase context) {
			base.AttachOverride(scope, elementHost, listener, context);
			UpdateIsValid(CurrentDescriptorSource);
		}
		protected override void DetachOverride() {
			base.DetachOverride();
			ConvertedValue = null;
		}
		protected override void InitializeDescriptorOverride() {
			base.InitializeDescriptorOverride();
			ConvertedValue = Descriptor != null ? RenderTriggerHelper.GetConvertedValue(Descriptor.PropertyType, Factory.Value) : Factory.Value;
		}
		public override void PreviewValueChanged(object sender, EventArgs args) {
			base.PreviewValueChanged(sender, args);
			UpdateIsValid(sender);
		}
		void UpdateIsValid(object source) {
			if (Descriptor == null) {
				IsValid = Factory.FallbackIsValid;
				return;
			}
			var value = Descriptor.GetValue(source);
			IsValid = object.Equals(value, ConvertedValue);
		}
	}
	public class RenderConditionGroupContext : RenderPropertyContextBase, IRenderConditionContext {
		readonly List<IRenderConditionContext> conditionContexts;
		public new RenderConditionGroup Factory { get { return base.Factory as RenderConditionGroup; } }
		public RenderConditionGroupContext(RenderConditionGroup factory)
			: base(factory) {
			conditionContexts = new List<IRenderConditionContext>(factory.Conditions.Count);
			foreach (var condition in Factory.Conditions) {
				conditionContexts.Add(condition.CreateContext() as IRenderConditionContext);
			}
		}
		protected override void AttachOverride(INamescope scope, IElementHost elementHost, IPropertyChangedListener listener, RenderTriggerContextBase context) {
			foreach (var conditionContext in conditionContexts)
				conditionContext.Attach(scope, elementHost, listener, context);
		}
		protected override void DetachOverride() {
			foreach (var conditionContext in conditionContexts)
				conditionContext.Detach();
		}
		public bool IsValid { get { return CalculateIsValid(); } }
		protected virtual bool CalculateIsValid() {
			return Factory.Operator == RenderConditionGroupOperator.And ? CalculateIsValidAnd() : CalculateIsValidOr();
		}
		protected virtual bool CalculateIsValidOr() {
			return conditionContexts.Any(context => context.IsValid);
		}
		protected virtual bool CalculateIsValidAnd() {
			return conditionContexts.All(context => context.IsValid);
		}
	}
}
