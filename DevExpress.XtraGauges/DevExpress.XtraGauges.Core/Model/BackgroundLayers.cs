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
	public abstract class BaseScaleBackgroundLayerProvider : BaseLayerProvider<BackgroundLayerShapeType> {
		public BaseScaleBackgroundLayerProvider(OwnerChangedAction layerChanged)
			: base(layerChanged) {
		}
		protected override BaseShape GetShape(BackgroundLayerShapeType value) {
			return BackgroundLayerShapeFactory.GetDefaultLayerShape(value);
		}
	}
	public class ArcScaleBackgroundLayerProvider : BaseScaleBackgroundLayerProvider {
		public ArcScaleBackgroundLayerProvider(OwnerChangedAction layerChanged)
			: base(layerChanged) {
		}
		protected override BackgroundLayerShapeType DefaultShapeType {
			get { return BackgroundLayerShapeType.CircularFull_Default; }
		}
	}
	public class LinearScaleBackgroundLayerProvider : BaseScaleBackgroundLayerProvider {
		public LinearScaleBackgroundLayerProvider(OwnerChangedAction layerChanged)
			: base(layerChanged) {
		}
		protected override BackgroundLayerShapeType DefaultShapeType {
			get { return BackgroundLayerShapeType.Linear_Default; }
		}
	}
	public class ArcScaleBackgroundLayer : LayerComponent<ArcScaleBackgroundLayerProvider>,
		IArcScaleBackgroundLayer, ISupportAssign<ArcScaleBackgroundLayer> {
		PointF2D scaleCenterPosCore;
		public ArcScaleBackgroundLayer() : base() { }
		public ArcScaleBackgroundLayer(string name) : base(name) { }
		protected override void OnCreate() {
			base.OnCreate();
			this.scaleCenterPosCore = new PointF2D(0.5f, 0.5f);
		}
		protected override IScale GetDefaultScaleCore() {
			return ScaleFactory.EmptyArcScale;
		}
		protected override ArcScaleBackgroundLayerProvider CreateProvider() {
			return new ArcScaleBackgroundLayerProvider(OnScaleDependentComponentChanged);
		}
		protected override void CalculateScaleDependentComponent() {
			BaseShape backgroundLayerShape = Shapes[PredefinedShapeNames.BackgroundLayer] as BaseShape;
			RectangleF box = backgroundLayerShape.BoundingBox;
			PointF p = new PointF(box.Left + box.Width * ScaleCenterPos.X, box.Top + box.Height * ScaleCenterPos.Y);
			SizeF scale = new SizeF(Size.Width / box.Width, Size.Height / box.Height);
			Transform = new Matrix(scale.Width, 0.0f, 0.0f, scale.Height, ArcScale.Center.X - p.X * scale.Width, ArcScale.Center.Y - p.Y * scale.Height);
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleBackgroundLayerScaleCenterPos"),
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
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleBackgroundLayerSize"),
#endif
 XtraSerializableProperty]
		public SizeF Size {
			get { return Provider.Size; }
			set { Provider.Size = value; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleBackgroundLayerScale")]
#endif
		public IScale Scale {
			get { return ScaleCore; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleBackgroundLayerArcScale")]
#endif
		public IArcScale ArcScale {
			get { return ScaleCore as IArcScale; }
			set { ScaleCore = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleBackgroundLayerShape"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShape Shape {
			get { return Provider.Shape; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleBackgroundLayerShapeType"),
#endif
DefaultValue(BackgroundLayerShapeType.CircularFull_Default)]
		[XtraSerializableProperty]
		public BackgroundLayerShapeType ShapeType {
			get { return Provider.ShapeType; }
			set { Provider.ShapeType = value; }
		}
		internal bool ShouldSerializeScaleCenterPos() {
			return ScaleCenterPos != new PointF2D(0.5f, 0.5f);
		}
		internal bool ShouldSerializeSize() {
			return Size != new SizeF(250, 250);
		}
		internal void ResetScaleCenterPos() {
			ScaleCenterPos = new PointF2D(0.5f, 0.5f);
		}
		internal void ResetSize() {
			Size = new SizeF(250, 250);
		}
		public void Assign(ArcScaleBackgroundLayer source) {
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
		public bool IsDifferFrom(ArcScaleBackgroundLayer source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
					(this.ArcScale != source.ArcScale) ||
					(this.ScaleCenterPos != source.ScaleCenterPos) ||
					(this.Size != source.Size) ||
					(this.ShapeType != source.ShapeType);
		}
	}
	public class LinearScaleBackgroundLayer : LayerComponent<LinearScaleBackgroundLayerProvider>,
		ILinearScaleBackgroundLayer, ISupportAssign<LinearScaleBackgroundLayer> {
		PointF2D scaleStartPosCore;
		PointF2D scaleEndPosCore;
		public LinearScaleBackgroundLayer() : base() { }
		public LinearScaleBackgroundLayer(string name) : base(name) { }
		protected override void OnCreate() {
			base.OnCreate();
			this.scaleStartPosCore = new PointF2D(0.5f, 0.86f);
			this.scaleEndPosCore = new PointF2D(0.5f, 0.14f);
		}
		protected override IScale GetDefaultScaleCore() {
			return ScaleFactory.EmptyLinearScale;
		}
		protected override LinearScaleBackgroundLayerProvider CreateProvider() {
			return new LinearScaleBackgroundLayerProvider(OnScaleDependentComponentChanged);
		}
		protected override void CalculateScaleDependentComponent() {
			if(IsDisposing) return;
			BaseShape backgroundLayerShape = Shapes[PredefinedShapeNames.BackgroundLayer];
			RectangleF box = backgroundLayerShape.BoundingBox;
			PointF shapePt1 = new PointF(box.Left + box.Width * ScaleStartPos.X, box.Top + box.Height * ScaleStartPos.Y);
			PointF shapePt2 = new PointF(box.Left + box.Width * ScaleEndPos.X, box.Top + box.Height * ScaleEndPos.Y);
			Transform = MathHelper.CalcMorphTransform(shapePt1, shapePt2, LinearScale.StartPoint, LinearScale.EndPoint);
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleBackgroundLayerScaleStartPos"),
#endif
XtraSerializableProperty]
		public PointF2D ScaleStartPos {
			get { return scaleStartPosCore; }
			set {
				if(ScaleStartPos == value) return;
				scaleStartPosCore = value;
				Provider.PerformOwnerChanged("ScaleStartPos");
			}
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleBackgroundLayerScaleEndPos"),
#endif
XtraSerializableProperty]
		public PointF2D ScaleEndPos {
			get { return scaleEndPosCore; }
			set {
				if(ScaleEndPos == value) return;
				scaleEndPosCore = value;
				Provider.PerformOwnerChanged("ScaleEndPos");
			}
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleBackgroundLayerScale")]
#endif
		public IScale Scale {
			get { return ScaleCore; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleBackgroundLayerLinearScale")]
#endif
		public ILinearScale LinearScale {
			get { return ScaleCore as ILinearScale; }
			set { ScaleCore = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleBackgroundLayerShape"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShape Shape {
			get { return Provider.Shape; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleBackgroundLayerShapeType"),
#endif
DefaultValue(BackgroundLayerShapeType.Linear_Default)]
		[XtraSerializableProperty]
		public BackgroundLayerShapeType ShapeType {
			get { return Provider.ShapeType; }
			set { Provider.ShapeType = value; }
		}
		internal bool ShouldSerializeScaleStartPos() {
			return ScaleStartPos != new PointF2D(0.5f, 0.86f);
		}
		internal bool ShouldSerializeScaleEndPos() {
			return ScaleEndPos != new PointF2D(0.5f, 0.14f);
		}
		internal void ResetScaleStartPos() {
			ScaleStartPos = new PointF2D(0.5f, 0.86f);
		}
		internal void ResetScaleEndPos() {
			ScaleEndPos = new PointF2D(0.5f, 0.14f);
		}
		public void Assign(LinearScaleBackgroundLayer source) {
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
		public bool IsDifferFrom(LinearScaleBackgroundLayer source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
					(this.LinearScale != source.LinearScale) ||
					(this.ScaleStartPos != source.ScaleStartPos) ||
					(this.ScaleEndPos != source.ScaleEndPos) ||
					(this.ShapeType != source.ShapeType);
		}
	}
}
