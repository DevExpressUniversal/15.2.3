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
using System.Drawing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraGauges.Core.Model {
	public delegate void OwnerChangedAction();
	public abstract class BaseProvider : BaseObject {
		OwnerChangedAction OwnerChanged;
		public BaseProvider(OwnerChangedAction ownerChanged) {
			this.OwnerChanged = ownerChanged;
		}
		protected override void OnDispose() {
			this.OwnerChanged = null;
			base.OnDispose();
		}
		protected override void OnUpdateObjectCore() {
			InvokeOwnerChanged();
		}
		protected void InvokeOwnerChanged() {
			if(OwnerChanged != null)
				OwnerChanged();
		}
		public void PerformOwnerChanged(string propertyName) {
			OnObjectChanged(propertyName);
		}
		protected object GetOwner() {
			return (OwnerChanged != null) ? OwnerChanged.Target : null;
		}
		protected internal void Update() {
			OnUpdateObjectCore();
		}
	}
	public abstract class BaseShapeProvider : BaseProvider, ICloneable {
		BaseShape shapeCore;
		protected BaseShapeProvider(OwnerChangedAction ownerChanged)
			: base(ownerChanged) {
			SetShape(CreateShape());
		}
		protected BaseShapeProvider(OwnerChangedAction ownerChanged, bool createShape)
			: base(ownerChanged) {
			if(createShape)
				SetShape(CreateShape());
		}
		protected override void OnDispose() {
			DestroyShape();
			base.OnDispose();
		}
		protected abstract BaseShape CreateShape();
		protected virtual void DestroyShape() {
			DestroyShapeCore(ref shapeCore);
		}
		protected virtual void SetShape(BaseShape value) {
			SetShapeCore(ref shapeCore, value);
		}
		protected virtual bool UseShapeChanged { 
			get { return true; } 
		}
		protected void SetShapeCore(ref BaseShape shape, BaseShape value) {
			shape = value;
			if(shape != null && UseShapeChanged)
				shape.Changed += OnShapeChanged;
		}
		protected void DestroyShapeCore(ref BaseShape shape) {
			if(shape != null && UseShapeChanged)
				shape.Changed -= OnShapeChanged;
			if(shape != null && !shape.IsEmpty)
				Ref.Dispose(ref shape);
		}
		void OnShapeChanged(object sender, EventArgs e) {
			OnObjectChanged("Shape");
		}
		[Browsable(false)]
		public BaseShape Shape {
			[System.Diagnostics.DebuggerStepThrough]
			get { return shapeCore; }
		}
		object ICloneable.Clone() {
			BaseObject clone = CloneCore();
			if(clone != null) {
				clone.BeginUpdate();
				CopyToCore(clone);
				clone.CancelUpdate();
			}
			return clone;
		}
		protected virtual BaseObject CloneCore() {
			return null;
		}
		protected virtual void CopyToCore(BaseObject clone) { }
	}
	public abstract class BaseShapeProvider<T> : BaseShapeProvider {
		T shapeTypeCore;
		protected BaseShapeProvider(OwnerChangedAction ownerChanged)
			: base(ownerChanged) {
			shapeTypeCore = DefaultShapeType;
		}
		protected BaseShapeProvider(OwnerChangedAction ownerChanged, bool createShape)
			: base(ownerChanged, createShape) {
			shapeTypeCore = DefaultShapeType;
		}
		protected BaseShapeProvider(OwnerChangedAction ownerChanged, T shapeType)
			: base(ownerChanged, false) {
			shapeTypeCore = shapeType;
			SetShape(GetShape(shapeType));
		}
		protected sealed override BaseShape CreateShape() {
			return GetShape(DefaultShapeType);
		}
		protected abstract T DefaultShapeType { get; }
		protected abstract BaseShape GetShape(T value);
		protected virtual void SetShapeTypeCore(T value) {
			SetShapeTypeCore(value, GetShape(value));
		}
		protected void AssignShape(BaseShapeProvider<T> provider) {
			SetShapeTypeCore(provider.ShapeType, provider.Shape.Clone());
		}
		protected void SetShapeTypeCore(T value, BaseShape shape) {
			DestroyShape();
			this.shapeTypeCore = value;
			SetShape(shape);
		}
		[DevExpress.Utils.Serializing.XtraSerializableProperty]
		public virtual T ShapeType {
			get { return shapeTypeCore; }
			set {
				if(object.Equals(ShapeType, value)) return;
				SetShapeTypeCore(value);
				OnObjectChanged("ShapeType");
			}
		}
		protected virtual bool ShouldSerializeShapeType() {
			return !object.Equals(ShapeType, default(T));
		}
		protected virtual void ResetShapeType() {
			ShapeType = default(T);
		}
	}
	public abstract class BaseLayerProvider<T> : BaseShapeProvider<T>, ILayer<T> {
		SizeF sizeCore;
		public BaseLayerProvider(OwnerChangedAction ownerChanged)
			: base(ownerChanged) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.sizeCore = InitSize();
		}
		protected virtual SizeF InitSize() {
			return new SizeF(250f, 250f);
		}
		public SizeF Size {
			get { return sizeCore; }
			set {
				if(Size == value) return;
				sizeCore = value;
				OnObjectChanged("Size");
			}
		}
	}
	public abstract class BaseScaleComponent : BaseLeafPrimitive {
		IScale scaleCore;
		protected BaseScaleComponent() : base() { }
		protected BaseScaleComponent(string name) : base(name) { }
		protected override void OnCreate() {
			base.OnCreate();
			scaleCore = GetDefaultScaleCore();
		}
		protected override void OnDispose() {
			UnsubscribeScaleEvents();
			base.OnDispose();
		}
		protected IScale ScaleCore {
			get { return scaleCore; }
			set {
				if(scaleCore == value || (value == null && !AllowNullScale)) return;
				SetScaleCore(value);
				OnScaleCoreChanged();
			}
		}
		bool AllowNullScale {
			get { return Site != null; }
		}
		void SetScaleCore(IScale value) {
			UnsubscribeScaleEvents();
			this.scaleCore = value ?? (Site != null ? GetDefaultScaleCore() : null);
			SubscribeScaleEvents();
		}
		protected abstract IScale GetDefaultScaleCore();
		protected abstract void OnScaleCoreChanged();
		protected virtual void SubscribeScaleEvents() { }
		protected virtual void UnsubscribeScaleEvents() { }
	}
	public abstract class BaseScaleDependentComponent<TProvider> : BaseScaleComponent
		where TProvider : BaseProvider {
		TProvider providerCore;
		protected BaseScaleDependentComponent() { }
		protected BaseScaleDependentComponent(string name) : base(name) { }
		protected override void OnCreate() {
			base.OnCreate();
			this.providerCore = CreateProvider();
		}
		protected override void OnDispose() {
			UnsubscribeScaleEvents();
			Ref.Dispose(ref providerCore);
			base.OnDispose();
		}
		protected abstract TProvider CreateProvider();
		protected IBaseScale BaseScale { 
			get { return ScaleCore as IBaseScale; } 
		}
		protected override void SubscribeScaleEvents() {
			if(ScaleCore != null && DependsOnValueBounds) {
				ScaleCore.ValueChanged += OnScaleValueChanged;
				ScaleCore.MinMaxValueChanged += OnScaleMinMaxValueChanged;
			}
			if(BaseScale != null) {
				BaseScale.GeometryChanged += OnScaleGeometryChanged;
				if(DependsOnValueBounds)
					BaseScale.Animating += OnScaleAnimating;
			}
		}
		protected override void UnsubscribeScaleEvents() {
			if(ScaleCore != null && DependsOnValueBounds) {
				ScaleCore.ValueChanged -= OnScaleValueChanged;
				ScaleCore.MinMaxValueChanged -= OnScaleMinMaxValueChanged;
			}
			if(BaseScale != null) {
				BaseScale.GeometryChanged -= OnScaleGeometryChanged;
				if(DependsOnValueBounds)
					BaseScale.Animating -= OnScaleAnimating;
			}
		}
		void OnScaleAnimating(object sender, EventArgs ea) {
			OnScaleDependentComponentChanged(ea);
		}
		void OnScaleValueChanged(object sender, EventArgs ea) {
			OnScaleDependentComponentChanged(ea);
		}
		void OnScaleMinMaxValueChanged(object sender, EventArgs ea) {
			OnScaleDependentComponentChanged(ea);
		}
		void OnScaleGeometryChanged(object sender, EventArgs ea) {
			OnScaleDependentComponentChanged(ea);
		}
		protected TProvider Provider {
			get { return providerCore; }
		}
		protected void OnScaleDependentComponentChanged(EventArgs e) {
			SetCalculationDelayed();
			RaiseChanged(e);
		}
		protected void OnScaleDependentComponentChanged() {
			SetCalculationDelayed();
			RaiseChanged(EventArgs.Empty);
		}
		protected sealed override void OnScaleCoreChanged() {
			OnScaleDependentComponentChanged();
		}
		protected sealed override void OnDelayedCalculation() {
			if(IsDisposing) return;
			OnShapesChanged();
			CalculateScaleDependentComponent();
		}
		public override void BeginUpdate() {
			base.BeginUpdate();
			Provider.BeginUpdate();
		}
		public override void CancelUpdate() {
			Provider.CancelUpdate();
			base.CancelUpdate();
		}
		public override void EndUpdate() {
			Provider.EndUpdate();
			base.EndUpdate();
		}
		public void Update() {
			if(!IsDisposing)
				Provider.Update();
		}
		protected abstract void CalculateScaleDependentComponent();
	}
	public abstract class BaseScaleIndependentComponent<TProvider> : BaseLeafPrimitive
		where TProvider : BaseProvider {
		TProvider providerCore;
		protected BaseScaleIndependentComponent() { }
		protected BaseScaleIndependentComponent(string name) : base(name) { }
		protected override void OnCreate() {
			base.OnCreate();
			this.providerCore = CreateProvider();
		}
		protected override void OnDispose() {
			Ref.Dispose(ref providerCore);
			base.OnDispose();
		}
		protected abstract TProvider CreateProvider();
		protected internal TProvider Provider {
			[System.Diagnostics.DebuggerStepThrough]
			get { return providerCore; }
		}
		protected void OnScaleIndependentComponentChanged() {
			SetCalculationDelayed();
			RaiseChanged(EventArgs.Empty);
		}
		protected sealed override void OnDelayedCalculation() {
			if(IsDisposing) return;
			PrepareDelayedCalculation();
			OnShapesChanged();
			CalculateScaleIndependentComponent();
		}
		protected virtual void PrepareDelayedCalculation() { }
		public override void BeginUpdate() {
			base.BeginUpdate();
			Provider.BeginUpdate();
		}
		public override void CancelUpdate() {
			Provider.CancelUpdate();
			base.CancelUpdate();
		}
		public override void EndUpdate() {
			Provider.EndUpdate();
			base.EndUpdate();
		}
		public void Update() {
			if(!IsDisposing)
				Provider.Update();
		}
		protected abstract void CalculateScaleIndependentComponent();
	}
	public abstract class ValueIndicatorComponent<TProvider> : BaseScaleDependentComponent<TProvider>
		where TProvider : BaseShapeProvider {
		protected ValueIndicatorComponent() { }
		protected ValueIndicatorComponent(string name) : base(name) { }
		protected sealed override bool DependsOnValueBounds {
			get { return true; }
		}
		protected sealed override void OnLoadShapes() {
			base.OnLoadShapes();
			LoadShape();
		}
		protected virtual void LoadShape() {
			Shapes.Add(Provider.Shape.Clone());
		}
		float? valueCore;
		[DefaultValue(null), DevExpress.Utils.Serializing.XtraSerializableProperty]
		public float? Value {
			get { return valueCore; }
			set {
				if(valueCore == value || float.IsNaN(value.GetValueOrDefault())) return;
				float prevValue = GetValue(Value);
				valueCore = value;
				OnScaleDependentComponentChanged(new ValueChangedEventArgs(prevValue, ActualValue));
			}
		}
		public float ActualValue {
			get { return GetValue(Value); }
		}
		protected float GetValue(float? value) {
			return value.HasValue ? value.Value : (ScaleCore.Value);
		}
	}
	public abstract class LayerComponent<TProvider> : BaseScaleDependentComponent<TProvider>
		where TProvider : BaseShapeProvider {
		protected LayerComponent() : base() { }
		protected LayerComponent(string name) : base(name) { }
		protected override bool AllowCacheRenderOperation {
			get { return true; }
		}
		protected sealed override bool DependsOnValueBounds {
			get { return false; }
		}
		protected sealed override void OnLoadShapes() {
			base.OnLoadShapes();
			LoadShape();
		}
		protected virtual void LoadShape() {
			Shapes.Add(Provider.Shape.Clone());
		}
	}
	public abstract class ScaleIndependentLayerComponent<TProvider> : BaseScaleIndependentComponent<TProvider>
		where TProvider : BaseShapeProvider {
		protected ScaleIndependentLayerComponent() { }
		protected ScaleIndependentLayerComponent(string name) : base(name) { }
		protected sealed override void OnLoadShapes() {
			base.OnLoadShapes();
			LoadShape();
		}
		protected override bool AllowCacheRenderOperation {
			get { return true; }
		}
		protected virtual void LoadShape() {
			Shapes.Add(Provider.Shape.Clone());
		}
	}
	public class CustomizationPointerPrimitive : BaseLeafPrimitive {
		protected override void OnLoadShapes() {
			base.OnLoadShapes();
			LoadModelShapes();
		}
		protected virtual void LoadModelShapes() {
			Shapes.Add(UniversalShapesFactory.GetShape(PredefinedShapeNames.ItemFrame));
		}
	}
	public static class ComponentCollectionExtention {
		public static bool TryDuplicateComponent<T>(BaseChangeableList<T> collection, IComponent component, out IComponent duplicate)
			where T : class, IComponent, ISupportAcceptOrder, IRenderableElement, ISupportAssign<T>, new() {
			bool compatibleComponent = component is T;
			if(compatibleComponent) {
				duplicate = new T() as IComponent;
				((T)duplicate).Assign(component as T);
			}
			else duplicate = null;
			return duplicate != null;
		}
		public static bool TryAddComponent<T>(BaseChangeableList<T> collection, IComponent component)
			where T : class, IComponent, ISupportAcceptOrder, IRenderableElement {
			bool compatibleComponent = component is T;
			if(compatibleComponent) collection.Add(component as T);
			return compatibleComponent;
		}
		public static void CollectNames<T>(BaseChangeableList<T> collection, List<string> names)
			where T : class, IComponent, ISupportAcceptOrder, IRenderableElement {
			for(int i = 0; i < collection.Count; i++) names.Add(collection[i].Name);
		}
		public static void DisableAppearance<T>(BaseChangeableList<T> collection)
					where T : class, IComponent, ISupportAcceptOrder, IRenderableElement {
			for(int i = 0; i < collection.Count; i++) {
				collection[i].Shader = BaseColorShader.Empty;
			}
		}
		public static void SetEnabled<T>(BaseChangeableList<T> collection, bool enabled)
					where T : class, IComponent, ISupportAcceptOrder, IRenderableElement {
			for(int i = 0; i < collection.Count; i++) collection[i].Enabled = enabled;
		}
		public static List<BaseElement<IRenderableElement>> FindScaleDependentElements<T>(BaseChangeableList<T> collection, IScale scale)
					where T : class, IComponent, ISupportAcceptOrder, IRenderableElement {
			List<BaseElement<IRenderableElement>> result = new List<BaseElement<IRenderableElement>>();
			for(int i = 0; i < collection.Count; i++) {
				IScaleComponent scaleComponent = collection[i] as IScaleComponent;
				if(scaleComponent != null && scaleComponent.Scale == scale) result.Add(collection[i] as BaseElement<IRenderableElement>);
			}
			return result;
		}
	}
}
