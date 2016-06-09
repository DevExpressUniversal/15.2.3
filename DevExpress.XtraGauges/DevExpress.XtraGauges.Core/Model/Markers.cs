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

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.XtraGauges.Core.Model {
	public class BaseMarkerProvider : BaseShapeProvider<MarkerPointerShapeType>, IMarker {
		float shapeOffsetCore;
		FactorF2D shapeScaleCore;
		public BaseMarkerProvider(OwnerChangedAction markerChanged)
			: base(markerChanged) {
		}
		protected override MarkerPointerShapeType DefaultShapeType {
			get { return MarkerPointerShapeType.Default; }
		}
		protected override BaseShape GetShape(MarkerPointerShapeType value) {
			return MarkerPointerShapeFactory.GetDefaultMarkerShape(value);
		}
		protected override void OnCreate() {
			this.shapeOffsetCore = 0f;
			this.shapeScaleCore = new FactorF2D(1f, 1f);
			base.OnCreate();
		}
		public IScale Scale {
			get { return null; }
		}
		public float ShapeOffset {
			get { return shapeOffsetCore; }
			set {
				if(ShapeOffset == value) return;
				shapeOffsetCore = value;
				OnObjectChanged("ShapeOffset");
			}
		}
		public FactorF2D ShapeScale {
			get { return shapeScaleCore; }
			set {
				if(ShapeScale == value) return;
				shapeScaleCore = value;
				OnObjectChanged("ShapeScale");
			}
		}
		public void Assign(IMarker source) {
			BeginUpdate();
			if(source != null) {
				this.ShapeType = source.ShapeType;
				this.shapeOffsetCore = source.ShapeOffset;
				this.shapeScaleCore = source.ShapeScale;
			}
			EndUpdate();
		}
		public bool IsDifferFrom(IMarker source) {
			return (source == null) ? true :
				(this.ShapeType != source.ShapeType) ||
				(this.shapeOffsetCore != source.ShapeOffset) ||
				(this.shapeScaleCore != source.ShapeScale);
		}
	}
	public class ArcScaleMarker : ValueIndicatorComponent<BaseMarkerProvider>,
		IArcScalePointer, IMarker, ISupportAssign<ArcScaleMarker> {
		public ArcScaleMarker() { }
		public ArcScaleMarker(string name)
			: base(name) {
		}
		protected override IScale GetDefaultScaleCore() {
			return ScaleFactory.EmptyArcScale;
		}
		protected override BaseMarkerProvider CreateProvider() {
			return new BaseMarkerProvider(OnScaleDependentComponentChanged);
		}
		protected override void CalculateScaleDependentComponent() {
			Self.SetViewInfoDirty();
			float range = ArcScale.EndAngle - ArcScale.StartAngle;
			float alpha = ArcScale.StartAngle + ArcScale.ValueToPercent(ActualValue) * range;
			BaseShape markerShape = Shapes[PredefinedShapeNames.Marker];
			ShadingHelper.ProcessShape(markerShape, Shader, Enabled);
			PointF origin = MathHelper.GetRadiusVector(ArcScale.RadiusX, ArcScale.RadiusY, alpha);
			PointF orientation = MathHelper.GetRadiusVector(ArcScale.RadiusX * 2f, ArcScale.RadiusY * 2f, alpha);
			SizeF offset = new SizeF(ArcScale.Center.X, ArcScale.Center.Y);
			float sx = -ShapeOffset / (float)MathHelper.CalcVectorLength(origin, orientation);
			SizeF shapeOffset = new SizeF((origin.X - orientation.X) * sx, (origin.Y - orientation.Y) * sx);
			PointF shapeOrigin = origin + shapeOffset + offset;
			PointF shapeOrientation = orientation + shapeOffset + offset;
			Matrix transform = MathHelper.CalcTransform(shapeOrigin, shapeOrientation, ShapeScale);
			markerShape.Accept(
					delegate(BaseShape s) {
						s.BeginUpdate();
						Self.ResetCache(CacheKeys.ShapePathCacheName(markerShape, s));
						s.Transform.Multiply(transform, MatrixOrder.Append);
						s.CancelUpdate();
					}
				);
			transform.Dispose();
		}
		#region Properties
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleMarkerScale")]
#endif
		public IScale Scale {
			get { return ScaleCore; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleMarkerArcScale")]
#endif
		public IArcScale ArcScale {
			get { return ScaleCore as IArcScale; }
			set { ScaleCore = value; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleMarkerShape")]
#endif
		public BaseShape Shape {
			get { return Provider.Shape; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleMarkerShapeType"),
#endif
 DefaultValue(MarkerPointerShapeType.Default)]
		[XtraSerializableProperty]
		public MarkerPointerShapeType ShapeType {
			get { return Provider.ShapeType; }
			set { Provider.ShapeType = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleMarkerShapeScale"),
#endif
 XtraSerializableProperty]
		public FactorF2D ShapeScale {
			get { return Provider.ShapeScale; }
			set { Provider.ShapeScale = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleMarkerShapeOffset"),
#endif
 XtraSerializableProperty, DefaultValue(0f)]
		public float ShapeOffset {
			get { return Provider.ShapeOffset; }
			set { Provider.ShapeOffset = value; }
		}
		#endregion Properties
		public void Assign(ArcScaleMarker source) {
			BeginUpdate();
			AssignPrimitiveProperties(source);
			if(source != null) {
				this.ArcScale = source.ArcScale;
				this.Value = source.Value;
				Provider.Assign(source);
			}
			EndUpdate();
		}
		public bool IsDifferFrom(ArcScaleMarker source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
				(this.ArcScale != source.ArcScale) || Provider.IsDifferFrom(source) ||
				(this.Value != source.Value);
		}
		internal bool ShouldSerializeShapeScale() {
			return ShapeScale != new FactorF2D(1f, 1f);
		}
		internal void ResetShapeScale() {
			ShapeScale = new FactorF2D(1f, 1f);
		}
	}
	public class LinearScaleMarker : ValueIndicatorComponent<BaseMarkerProvider>,
		ILinearScalePointer, IMarker, ISupportAssign<LinearScaleMarker> {
		public LinearScaleMarker() { }
		public LinearScaleMarker(string name) : base(name) { }
		protected override IScale GetDefaultScaleCore() {
			return ScaleFactory.EmptyLinearScale;
		}
		protected override BaseMarkerProvider CreateProvider() {
			return new BaseMarkerProvider(OnScaleDependentComponentChanged);
		}
		protected override void CalculateScaleDependentComponent() {
			Self.SetViewInfoDirty();
			BaseShape markerShape = Shapes[PredefinedShapeNames.Marker];
			ShadingHelper.ProcessShape(markerShape, Shader, Enabled);
			RectangleF box = markerShape.BoundingBox;
			PointF scaleVector = new PointF(LinearScale.EndPoint.X - LinearScale.StartPoint.X, LinearScale.EndPoint.Y - LinearScale.StartPoint.Y);
			PointF origin = LinearScale.PercentToPoint(LinearScale.ValueToPercent(ActualValue));
			PointF orientation = origin + new SizeF(-scaleVector.Y, scaleVector.X);
			float sx = -ShapeOffset / (float)MathHelper.CalcVectorLength(origin, orientation);
			SizeF shapeOffset = new SizeF((origin.X - orientation.X) * sx, (origin.Y - orientation.Y) * sx);
			Matrix transform = MathHelper.CalcTransform(origin + shapeOffset, orientation + shapeOffset, ShapeScale);
			markerShape.Accept(
					delegate(BaseShape s) {
						s.BeginUpdate();
						s.Transform.Multiply(transform, MatrixOrder.Append);
						s.EndUpdate();
					}
				);
			transform.Dispose();
		}
		#region Properties
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleMarkerScale")]
#endif
		public IScale Scale {
			get { return ScaleCore; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleMarkerLinearScale")]
#endif
		public ILinearScale LinearScale {
			get { return ScaleCore as ILinearScale; }
			set { ScaleCore = value; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleMarkerShape")]
#endif
		public BaseShape Shape {
			get { return Provider.Shape; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleMarkerShapeType"),
#endif
 DefaultValue(MarkerPointerShapeType.Default)]
		[XtraSerializableProperty]
		public MarkerPointerShapeType ShapeType {
			get { return Provider.ShapeType; }
			set { Provider.ShapeType = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleMarkerShapeScale"),
#endif
 XtraSerializableProperty]
		public FactorF2D ShapeScale {
			get { return Provider.ShapeScale; }
			set { Provider.ShapeScale = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleMarkerShapeOffset"),
#endif
 XtraSerializableProperty, DefaultValue(0f)]
		public float ShapeOffset {
			get { return Provider.ShapeOffset; }
			set { Provider.ShapeOffset = value; }
		}
		#endregion Properties
		public void Assign(LinearScaleMarker source) {
			BeginUpdate();
			AssignPrimitiveProperties(source);
			if(source != null) {
				this.LinearScale = source.LinearScale;
				this.Value = source.Value;
				Provider.Assign(source);
			}
			EndUpdate();
		}
		public bool IsDifferFrom(LinearScaleMarker source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
				(this.LinearScale != source.LinearScale) || Provider.IsDifferFrom(source) ||
				(this.Value != source.Value);
		}
		internal bool ShouldSerializeShapeScale() {
			return ShapeScale != new FactorF2D(1f, 1f);
		}
		internal void ResetShapeScale() {
			ShapeScale = new FactorF2D(1f, 1f);
		}
	}
}
