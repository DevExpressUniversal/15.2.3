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
using System.Globalization;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
#if !DXPORTABLE
using System.Drawing.Design;
#endif
namespace DevExpress.XtraGauges.Core.Model {
	public abstract class BaseRange : BaseObjectEx, IRange {
		string nameCore;
		float startValueCore;
		float endValueCore;
		float shapeOffsetCore;
		float startThicknessCore;
		float endThicknessCore;
		BaseShape rangeShapeCore;
		public event EventHandler Enter;
		public event EventHandler Leave;
		public BaseRange()
			: base() {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.startValueCore = 0;
			this.endValueCore = 1;
			this.endThicknessCore = 10;
			this.startThicknessCore = 10;
			this.shapeOffsetCore = -25;
			this.rangeShapeCore = CreateRangeShape();
			Shape.Appearance.Changed += OnRangeAppearanceChanged;
		}
		protected override void OnDispose() {
			Enter = null;
			Leave = null;
			DestroyShape();
			base.OnDispose();
		}
		protected void DestroyShape() {
			if(Shape != null) {
				Shape.Appearance.Changed -= OnRangeAppearanceChanged;
				Ref.Dispose(ref rangeShapeCore);
			}
		}
		void OnRangeAppearanceChanged(object sender, EventArgs e) {
			if(IsUpdateLocked || IsDisposing) return;
			OnObjectChanged("AppearanceRange");
		}
		protected abstract BaseShape CreateRangeShape();
		int ISupportAcceptOrder.AcceptOrder {
			get { return -100; }
			set { }
		}
		[XtraSerializableProperty]
		public string Name {
			get { return nameCore; }
			set {
				nameCore = value;
				Shape.Name = value + "RangeShape";
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShape Shape {
			get { return rangeShapeCore; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseShapeAppearance AppearanceRange {
			get { return Shape.Appearance; }
		}
		[XtraSerializableProperty, Category("Geometry")]
		[DefaultValue(0f)]
		public float StartValue {
			get { return startValueCore; }
			set {
				if(StartValue == value) return;
				startValueCore = value;
				OnObjectChanged("StartValue");
			}
		}
		[XtraSerializableProperty, Category("Geometry")]
		[DefaultValue(1f)]
		public float EndValue {
			get { return endValueCore; }
			set {
				if(EndValue == value) return;
				endValueCore = value;
				OnObjectChanged("EndValue");
			}
		}
		[XtraSerializableProperty, Category("Geometry")]
		[DefaultValue(10f)]
		public float StartThickness {
			get { return startThicknessCore; }
			set {
				if(StartThickness == value) return;
				startThicknessCore = value;
				OnObjectChanged("StartThickness");
			}
		}
		[XtraSerializableProperty, Category("Geometry")]
		[DefaultValue(10f)]
		public float EndThickness {
			get { return endThicknessCore; }
			set {
				if(EndThickness == value) return;
				endThicknessCore = value;
				OnObjectChanged("EndThickness");
			}
		}
		[XtraSerializableProperty, Category("Geometry")]
		[DefaultValue(-25f)]
		public float ShapeOffset {
			get { return shapeOffsetCore; }
			set {
				if(ShapeOffset == value) return;
				shapeOffsetCore = value;
				OnObjectChanged("ShapeOffset");
			}
		}
		protected override void OnUpdateObjectCore() {
			UpdateRangeShape(Shape);
			base.OnUpdateObjectCore();
		}
		protected internal abstract void UpdateRangeShape(BaseShape shape);
		protected override void CopyToCore(BaseObject clone) {
			BaseRange clonedRange = clone as BaseRange;
			if(clonedRange != null) {
				clonedRange.nameCore = this.Name;
				clonedRange.startValueCore = this.StartValue;
				clonedRange.endValueCore = this.EndValue;
				clonedRange.startThicknessCore = this.StartThickness;
				clonedRange.endThicknessCore = this.EndThickness;
				clonedRange.shapeOffsetCore = this.ShapeOffset;
				clonedRange.rangeShapeCore = this.Shape.Clone() as BaseShape;
				clonedRange.Shape.Appearance.Changed += clonedRange.OnRangeAppearanceChanged;
			}
		}
		protected void RaiseEnter(){
			if(Enter != null) Enter(this, EventArgs.Empty);
		}
		protected void RaiseLeave() {
			if(Leave != null) Leave(this, EventArgs.Empty);
		}
		public void Assign(IRange source) {
			BeginUpdate();
			AssignCore(source as BaseRange);
			EndUpdate();
		}
		public bool IsDifferFrom(IRange source) {
			return IsDifferFromCore(source as BaseRange);
		}
		protected virtual void AssignCore(BaseRange source) {
			if(source != null) {
				this.nameCore = source.Name;
				this.startValueCore = source.StartValue;
				this.endValueCore = source.EndValue;
				this.startThicknessCore = source.StartThickness;
				this.endThicknessCore = source.EndThickness;
				this.shapeOffsetCore = source.ShapeOffset;
				this.rangeShapeCore = source.Shape.Clone() as BaseShape;
			}
		}
		protected virtual bool IsDifferFromCore(BaseRange source) {
			return (source == null) ? true :
				(this.nameCore != source.Name) ||
				(this.startValueCore != source.StartValue) ||
				(this.endValueCore != source.EndValue) ||
				(this.startThicknessCore != source.StartThickness) ||
				(this.endThicknessCore != source.EndThickness) ||
				(this.shapeOffsetCore != source.ShapeOffset) ||
				(this.AppearanceRange.IsDifferFrom(source.AppearanceRange));
		}
		internal bool ShouldSerializeAppearanceRange() {
			return AppearanceRange.ShouldSerialize();
		}
		internal void ResetAppearanceRange() {
			AppearanceRange.Reset();
		}
	}
	public abstract class ScaleRange : BaseRange, IScaleRange {
		IScale scaleCore;
		float? startPercentCore;
		float? endPercentCore; 
		protected override void OnCreate() {
			base.OnCreate();
		}
		protected override void OnDispose() {
			if(Scale != null) {
				Scale.ValueChanged -= OnScaleValueChanged;
				scaleCore = null;
			}
			base.OnDispose();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IScale Scale {
			get { return scaleCore; }
		}
		internal virtual void OnAttachScale(IScale scale) {
			this.scaleCore = scale;
			SubscribeScaleEvents();
		}
		internal virtual void OnDetachScale() {
			UnSubscribeScaleEvents();
			this.scaleCore = null;
		}
		void SubscribeScaleEvents() {
			if(Scale != null) Scale.ValueChanged += OnScaleValueChanged;
		}
		void UnSubscribeScaleEvents() {
			if(Scale != null) Scale.ValueChanged -= OnScaleValueChanged;
		}
		void OnScaleValueChanged(object sender, EventArgs e) {
			ValueChangedEventArgs vea = e as ValueChangedEventArgs;
			if(vea == null) return;
			float start = CalcValue(StartPercent, StartValue);
			float end = CalcValue(EndPercent, EndValue);
			float min = Math.Min(start, end);
			float max = Math.Max(start, end);
			bool fEnterFromMin = (vea.PrevValue <= min) && (vea.Value > min) && (vea.Value < max);
			bool fEnterFromMax = (vea.PrevValue >= max) && (vea.Value < max) && (vea.Value > min);
			bool fLeaveOverMin = (vea.PrevValue > min) && (vea.PrevValue < max) && (vea.Value <= min);
			bool fLeaveOverMax = (vea.PrevValue < max) && (vea.PrevValue > min) && (vea.Value >= max);
			if(fEnterFromMax || fEnterFromMin) RaiseEnter();
			if(fLeaveOverMin || fLeaveOverMax) RaiseLeave();
		}
		[XtraSerializableProperty, Category("Geometry")]
		[DefaultValue(null)]
		public float? StartPercent {
			get { return startPercentCore; }
			set {
				if(StartPercent == value) return;
				startPercentCore = value;
				OnObjectChanged("StartPercent");
			}
		}
		[XtraSerializableProperty, Category("Geometry")]
		[DefaultValue(null)]
		public float? EndPercent {
			get { return endPercentCore; }
			set {
				if(EndPercent == value) return;
				endPercentCore = value;
				OnObjectChanged("EndPercent");
			}
		}
		protected float CalcPercent(float? percent, float value) {
			return percent.HasValue ? percent.Value : ((IConvertibleScale)Scale).ValueToPercent(value);
		}
		protected float CalcValue(float? percent, float value) {
			return percent.HasValue ? ((IConvertibleScale)Scale).PercentToValue(percent.Value) : value;
		}
		protected override void AssignCore(BaseRange source) {
			base.AssignCore(source);
			ScaleRange scaleRange = source as ScaleRange;
			if(scaleRange != null) {
				this.startPercentCore = scaleRange.StartPercent;
				this.endPercentCore = scaleRange.EndPercent;
			}
		}
		protected override bool IsDifferFromCore(BaseRange source) {
			ScaleRange scaleRange = source as ScaleRange;
			return (scaleRange == null) ? true :
				(this.startPercentCore != scaleRange.StartPercent) ||
				(this.endPercentCore != scaleRange.EndPercent) ||
				base.IsDifferFromCore(source);
		}
	}
	public class ArcScaleRange : ScaleRange {
		protected override BaseObject CloneCore() {
			return new ArcScaleRange();
		}
		protected IArcScale ArcScale {
			get { return Scale as IArcScale; }
		}
		protected override BaseShape CreateRangeShape() {
			return new SectorRangeShape();
		}
		protected internal override void UpdateRangeShape(BaseShape shape) {
			SectorRangeShape rangeShape = shape as SectorRangeShape;
			if(rangeShape != null && ArcScale != null) {
				rangeShape.BeginUpdate();
				float dRx = ArcScale.RadiusX + ShapeOffset;
				float dRy = ArcScale.RadiusY + ShapeOffset;
				rangeShape.Box = new RectangleF(ArcScale.Center.X - dRx, ArcScale.Center.Y - dRy, dRx * 2f, dRy * 2f);
				rangeShape.StartAngle = ArcScale.StartAngle + (ArcScale.EndAngle - ArcScale.StartAngle) * CalcPercent(StartPercent, StartValue);
				rangeShape.EndAngle = ArcScale.StartAngle + (ArcScale.EndAngle - ArcScale.StartAngle) * CalcPercent(EndPercent, EndValue);				
				double startL = MathHelper.CalcVectorLength(PointF.Empty, MathHelper.GetRadiusVector(dRx, dRy, rangeShape.StartAngle));
				double endL = MathHelper.CalcVectorLength(PointF.Empty, MathHelper.GetRadiusVector(dRx, dRy, rangeShape.EndAngle));
				rangeShape.StartThickness = StartThickness / (float)startL;
				rangeShape.EndThickness = EndThickness / (float)endL;
				rangeShape.EndUpdate();
			}
		}
	}
	public class LinearScaleRange : ScaleRange {
		protected override BaseObject CloneCore() {
			return new LinearScaleRange();
		}
		protected ILinearScale LinearScale {
			get { return Scale as ILinearScale; }
		}
		protected override BaseShape CreateRangeShape() {
			return new PolygonShape(new PointF[4]);
		}
		protected internal override void UpdateRangeShape(BaseShape shape) {
			PolygonShape rangeShape = shape as PolygonShape;
			if(rangeShape != null && LinearScale != null) {
				rangeShape.BeginUpdate();
				PointF2D s = LinearScale.StartPoint;
				PointF2D e = LinearScale.EndPoint;
				PointF v = new PointF(e.X - s.X, e.Y - s.Y);
				float startPercent = CalcPercent(StartPercent, StartValue);
				float endPercent = CalcPercent(EndPercent, EndValue);
				if(float.IsInfinity(startPercent) || float.IsNaN(startPercent)) startPercent = 0;
				if(float.IsInfinity(endPercent) || float.IsNaN(endPercent)) endPercent = 0;
				PointF org1 = new PointF(s.X + v.X * startPercent, s.Y + v.Y * startPercent);
				PointF org2 = new PointF(s.X + v.X * endPercent, s.Y + v.Y * endPercent);
				float s0 = -ShapeOffset / (float)MathHelper.CalcVectorLength(v);
				float s1 = s0 - StartThickness / (float)MathHelper.CalcVectorLength(v);
				float s2 = s0 - EndThickness / (float)MathHelper.CalcVectorLength(v);
				rangeShape.Points[0] = org1 + new SizeF(v.Y * s1, -v.X * s1);
				rangeShape.Points[1] = org1 + new SizeF(v.Y * s0, -v.X * s0);
				rangeShape.Points[2] = org2 + new SizeF(v.Y * s0, -v.X * s0);
				rangeShape.Points[3] = org2 + new SizeF(v.Y * s2, -v.X * s2);
				rangeShape.EndUpdate();
			}
		}
	}
#if !DXPORTABLE
	[Editor("DevExpress.XtraGauges.Design.RangeCollectionEditor, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(UITypeEditor))]
#endif
	[TypeConverter(typeof(RangeCollectionObjectTypeConverter))]
	public abstract class RangeCollection : BaseChangeableList<IRange>, ISupportAssign<RangeCollection> {
		protected override void OnDispose() {
			DisposeRanges();
			base.OnDispose();
		}
		protected sealed override void OnBeforeElementAdded(IRange element) {
			string[] names = CollectionHelper.GetNames(this);
			if(String.IsNullOrEmpty(element.Name) || CollectionHelper.NamesContains(element.Name, names)) {
				element.Name = UniqueNameHelper.GetUniqueName("Range", names, names.Length);
			}
			base.OnBeforeElementAdded(element);
		}
		protected override void OnElementAdded(IRange element) {
			base.OnElementAdded(element);
			BaseRange range = element as BaseRange;
			if(range != null) {
				range.Changed += OnElementChanged;
				range.Disposed += OnElementDisposed;
			}
		}
		protected override void OnElementRemoved(IRange element) {
			BaseRange range = element as BaseRange;
			if(range != null) {
				range.Changed -= OnElementChanged;
				range.Disposed -= OnElementDisposed;
			}
			base.OnElementRemoved(element);
		}
		void OnElementDisposed(object sender, EventArgs ea) {
			RaiseCollectionChanged(new CollectionChangedEventArgs<IRange>(sender as IRange, ElementChangedType.ElementDisposed));
			if(List != null) Remove(sender as IRange);
		}
		void OnElementChanged(object sender, EventArgs ea) {
			RaiseCollectionChanged(new CollectionChangedEventArgs<IRange>(sender as IRange, ElementChangedType.ElementUpdated));
		}
		public void Assign(RangeCollection ranges) {
			DisposeRanges();
			for(int i = 0; i < ranges.Count; i++) {
				IRange range = CreateRange();
				range.Assign(ranges[i]);
				Add(range);
			}
		}
		void DisposeRanges() {
			List<IRange> exisingRanges = new List<IRange>(this);
			foreach(IRange r in exisingRanges) r.Dispose();
			Clear();
		}
		public bool IsDifferFrom(RangeCollection ranges) {
			if(ranges.Count != Count) return true;
			for(int i = 0; i < ranges.Count; i++) {
				if(this[i].IsDifferFrom(ranges[i])) return true;
			}
			return false;
		}
		protected internal abstract IRange CreateRange();
		protected internal virtual void UpdateRanges() {
			for(int i = 0; i < Count; i++) {
				BaseRange range = this[i] as BaseRange;
				if(range != null) range.UpdateRangeShape(range.Shape);
			}
		}
	}
	public abstract class ScaleRangeCollection : RangeCollection {
		IScale scaleCore;
		protected ScaleRangeCollection(IScale scale) {
			scaleCore = scale;
			if(Scale != null) Scale.ValueChanged += OnScaleValueChanged;
		}
		protected override void OnDispose() {
			if(Scale != null) {
				Scale.ValueChanged -= OnScaleValueChanged;
				scaleCore = null;
			}
			base.OnDispose();
		}
		void OnScaleValueChanged(object sender, EventArgs e) {
			UpdateRanges();
		}
		protected IScale Scale {
			get { return scaleCore; }
		}
		protected override void OnElementAdded(IRange element) {
			ScaleRange range = element as ScaleRange;
			if(range != null) range.OnAttachScale(Scale);
			base.OnElementAdded(element);
		}
		protected sealed override void OnElementRemoved(IRange element) {
			base.OnElementRemoved(element);
			ScaleRange range = element as ScaleRange;
			if(range != null) range.OnDetachScale();
		}
		protected internal override void UpdateRanges() {
			base.UpdateRanges();
			IRenderableElement p = Scale as IRenderableElement;
			if(p != null) {
				for(int i = 0; i < Count; i++) {
					BaseRange range = this[i] as BaseRange;
					if(range != null) {
						string key = range.Shape.Name;
						if(string.IsNullOrEmpty(key)) continue;
						BaseShape shape = p.Shapes[key];
						if(shape != null) range.UpdateRangeShape(shape);
					}
				}
			}
		}
	}
	public class ArcScaleRangeCollection : ScaleRangeCollection {
		public ArcScaleRangeCollection(IArcScale scale) : base(scale) { }
		protected internal override IRange CreateRange() {
			return new ArcScaleRange();
		}
	}
	public class LinearScaleRangeCollection : ScaleRangeCollection {
		public LinearScaleRangeCollection(ILinearScale scale) : base(scale) { }
		protected internal override IRange CreateRange() {
			return new LinearScaleRange();
		}
	}
	public class RangeCollectionObjectTypeConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return destType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			return "<Ranges...>";
		}
	}
}
