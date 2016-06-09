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
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraGauges.Core.Model {
	public abstract class BaseScaleEffectLayerProvider : BaseLayerProvider<EffectLayerShapeType> {
		public BaseScaleEffectLayerProvider(OwnerChangedAction layerChanged)
			: base(layerChanged) {
		}
		protected override BaseShape GetShape(EffectLayerShapeType value) {
			return EffectLayerShapeFactory.GetDefaultLayerShape(value);
		}
	}
	public class ArcScaleEffectLayerProvider : BaseScaleEffectLayerProvider {
		public ArcScaleEffectLayerProvider(OwnerChangedAction layerChanged)
			: base(layerChanged) {
		}
		protected override SizeF InitSize() {
			return new SizeF(200, 100);
		}
		protected override EffectLayerShapeType DefaultShapeType {
			get { return EffectLayerShapeType.Circular_Default; }
		}
	}
	public class LinearScaleEffectLayerProvider : BaseScaleEffectLayerProvider {
		public LinearScaleEffectLayerProvider(OwnerChangedAction layerChanged)
			: base(layerChanged) {
		}
		protected override EffectLayerShapeType DefaultShapeType {
			get { return EffectLayerShapeType.Linear_Style6; }
		}
	}
	public class ArcScaleEffectLayer : LayerComponent<ArcScaleEffectLayerProvider>,
		IArcScaleEffectLayer, ISupportAssign<ArcScaleEffectLayer> {
		PointF2D scaleCenterPosCore;
		public ArcScaleEffectLayer() : base() { }
		public ArcScaleEffectLayer(string name) : base(name) { }
		protected override void OnCreate() {
			base.OnCreate();
			this.scaleCenterPosCore = new PointF2D(0.5f, 1.1f);
		}
		protected override bool AllowCacheRenderOperation {
			get { return false; }
		}
		protected override IScale GetDefaultScaleCore() {
			return ScaleFactory.EmptyArcScale;
		}
		protected override ArcScaleEffectLayerProvider CreateProvider() {
			return new ArcScaleEffectLayerProvider(OnScaleDependentComponentChanged);
		}
		protected override void CalculateScaleDependentComponent() {
			if(ShapeType == EffectLayerShapeType.Empty) return;
			BaseShape layerShape = Shapes[PredefinedShapeNames.Effect];
			RectangleF box = layerShape.BoundingBox;
			PointF p = new PointF(box.Left + box.Width * ScaleCenterPos.X, box.Top + box.Height * ScaleCenterPos.Y);
			SizeF scale = new SizeF(Size.Width / box.Width, Size.Height / box.Height);
			Transform = new Matrix(scale.Width, 0.0f, 0.0f, scale.Height, ArcScale.Center.X - p.X * scale.Width, ArcScale.Center.Y - p.Y * scale.Height);
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleEffectLayerScaleCenterPos"),
#endif
XtraSerializableProperty]
		public PointF2D ScaleCenterPos {
			get { return scaleCenterPosCore; }
			set {
				if(ScaleCenterPos == value) return;
				scaleCenterPosCore = value;
				Provider.PerformOwnerChanged("ScaleCenterPos");
			}
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleEffectLayerSize"),
#endif
XtraSerializableProperty]
		public SizeF Size {
			get { return Provider.Size; }
			set { Provider.Size = value; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleEffectLayerScale")]
#endif
		public IScale Scale {
			get { return ScaleCore; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleEffectLayerArcScale")]
#endif
		public IArcScale ArcScale {
			get { return ScaleCore as IArcScale; }
			set { ScaleCore = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleEffectLayerShape"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShape Shape {
			get { return Provider.Shape; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleEffectLayerShapeType"),
#endif
DefaultValue(EffectLayerShapeType.Circular_Default)]
		[XtraSerializableProperty]
		public EffectLayerShapeType ShapeType {
			get { return Provider.ShapeType; }
			set { Provider.ShapeType = value; }
		}
		internal bool ShouldSerializeScaleCenterPos() {
			return ScaleCenterPos != new PointF2D(0.5f, 1.1f);
		}
		internal bool ShouldSerializeSize() {
			return Size != new SizeF(200, 100);
		}
		internal void ResetScaleCenterPos() {
			ScaleCenterPos = new PointF2D(0.5f, 1.1f);
		}
		internal void ResetSize() {
			Size = new SizeF(200, 100);
		}
		public void Assign(ArcScaleEffectLayer source) {
			BeginUpdate();
			AssignPrimitiveProperties(source);
			if(source != null) {
				this.ArcScale = source.ArcScale;
				this.ScaleCenterPos = source.ScaleCenterPos;
				this.Size = source.Size;
				this.ShapeType = source.ShapeType;
			}
			EndUpdate();
		}
		public bool IsDifferFrom(ArcScaleEffectLayer source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
					(this.ArcScale != source.ArcScale) ||
					(this.ScaleCenterPos != source.ScaleCenterPos) ||
					(this.Size != source.Size) ||
					(this.ShapeType != source.ShapeType);
		}
	}
	public class LinearScaleEffectLayer : LayerComponent<LinearScaleEffectLayerProvider>,
		ILinearScaleEffectLayer, ISupportAssign<LinearScaleEffectLayer> {
		PointF2D layerStartPosCore;
		PointF2D layerEndPosCore;
		public LinearScaleEffectLayer() : base() { }
		public LinearScaleEffectLayer(string name) : base(name) { }
		protected override void OnCreate() {
			base.OnCreate();
			this.layerStartPosCore = new PointF2D(0.5f, 0.1f);
			this.layerEndPosCore = new PointF2D(0.5f, 0.9f);
		}
		protected override bool AllowCacheRenderOperation {
			get { return false; }
		}
		protected override IScale GetDefaultScaleCore() {
			return ScaleFactory.EmptyLinearScale;
		}
		protected override LinearScaleEffectLayerProvider CreateProvider() {
			return new LinearScaleEffectLayerProvider(OnScaleDependentComponentChanged);
		}
		protected override void CalculateScaleDependentComponent() {
			if(IsDisposing || ShapeType == EffectLayerShapeType.Empty) return;
			BaseShape layerShape = Shapes[PredefinedShapeNames.Effect];
			RectangleF box = layerShape.BoundingBox;
			PointF shapePt1 = new PointF(box.Left + box.Width * ScaleStartPos.X, box.Top + box.Height * ScaleStartPos.Y);
			PointF shapePt2 = new PointF(box.Left + box.Width * ScaleEndPos.X, box.Top + box.Height * ScaleEndPos.Y);
			Transform = MathHelper.CalcMorphTransform(shapePt1, shapePt2, LinearScale.EndPoint, LinearScale.StartPoint);
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleEffectLayerScaleStartPos"),
#endif
XtraSerializableProperty]
		public PointF2D ScaleStartPos {
			get { return layerStartPosCore; }
			set {
				if(ScaleStartPos == value) return;
				layerStartPosCore = value;
				Provider.PerformOwnerChanged("ScaleStartPos");
			}
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleEffectLayerScaleEndPos"),
#endif
XtraSerializableProperty]
		public PointF2D ScaleEndPos {
			get { return layerEndPosCore; }
			set {
				if(ScaleEndPos == value) return;
				layerEndPosCore = value;
				Provider.PerformOwnerChanged("ScaleEndPos");
			}
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleEffectLayerScale")]
#endif
		public IScale Scale {
			get { return ScaleCore; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleEffectLayerLinearScale")]
#endif
		public ILinearScale LinearScale {
			get { return ScaleCore as ILinearScale; }
			set { ScaleCore = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleEffectLayerShape"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShape Shape {
			get { return Provider.Shape; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleEffectLayerShapeType"),
#endif
DefaultValue(EffectLayerShapeType.Circular_Default)]
		[XtraSerializableProperty]
		public EffectLayerShapeType ShapeType {
			get { return Provider.ShapeType; }
			set { Provider.ShapeType = value; }
		}
		internal bool ShouldSerializeScaleStartPos() {
			return ScaleStartPos != new PointF2D(0.5f, 0.1f);
		}
		internal bool ShouldSerializeScaleEndPos() {
			return ScaleEndPos != new PointF2D(0.5f, 0.9f);
		}
		internal void ResetScaleStartPos() {
			ScaleStartPos = new PointF2D(0.5f, 0.1f);
		}
		internal void ResetScaleEndPos() {
			ScaleEndPos = new PointF2D(0.5f, 0.9f);
		}
		public void Assign(LinearScaleEffectLayer source) {
			BeginUpdate();
			AssignPrimitiveProperties(source);
			if(source != null) {
				this.LinearScale = source.LinearScale;
				this.ScaleStartPos = source.ScaleStartPos;
				this.ScaleEndPos = source.ScaleEndPos;
				this.ShapeType = source.ShapeType;
			}
			EndUpdate();
		}
		public bool IsDifferFrom(LinearScaleEffectLayer source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
					(this.LinearScale != source.LinearScale) ||
					(this.ScaleStartPos != source.ScaleStartPos) ||
					(this.ScaleEndPos != source.ScaleEndPos) ||
					(this.ShapeType != source.ShapeType);
		}
	}
}
