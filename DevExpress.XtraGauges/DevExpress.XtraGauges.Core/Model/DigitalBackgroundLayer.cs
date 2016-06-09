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
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraGauges.Core.Model {
	public class DigitalBackgroundLayerProvider : BaseLayerProvider<DigitalBackgroundShapeSetType> {
		BaseShape shapeNearCore;
		BaseShape shapeCenterCore;
		BaseShape shapeFarCore;
		public DigitalBackgroundLayerProvider(OwnerChangedAction layerChanged)
			: base(layerChanged) {
		}
		protected override DigitalBackgroundShapeSetType DefaultShapeType {
			get { return DigitalBackgroundShapeSetType.Default; }
		}
		protected override BaseShape GetShape(DigitalBackgroundShapeSetType value) {
			return DigitalBackgroundShapeFactory.GetDigitalBackgroundShape(value);
		}
		protected override void DestroyShape() {
			base.DestroyShape();
			Ref.Dispose(ref shapeNearCore);
			Ref.Dispose(ref shapeCenterCore);
			Ref.Dispose(ref shapeFarCore);
		}
		protected override void SetShape(BaseShape value) {
			ComplexShape template = value as ComplexShape;
			SetShapeCore(ref shapeNearCore, template.Collection[PredefinedShapeNames.DigitalBGNear]);
			SetShapeCore(ref shapeCenterCore, template.Collection[PredefinedShapeNames.DigitalBGCenter]);
			SetShapeCore(ref shapeFarCore, template.Collection[PredefinedShapeNames.DigitalBGFar]);
			base.SetShape(value);
		}
		public BaseShape ShapeNear {
			get { return shapeNearCore; }
		}
		public BaseShape ShapeCenter {
			get { return shapeCenterCore; }
		}
		public BaseShape ShapeFar {
			get { return shapeFarCore; }
		}
	}
	public class DigitalBackgroundLayer : ScaleIndependentLayerComponent<DigitalBackgroundLayerProvider>,
		IDigitalBackgroundLayer, ISupportAssign<DigitalBackgroundLayer> {
		PointF2D topLeftPointCore;
		PointF2D bottomRightPointCore;
		public DigitalBackgroundLayer() : base() { }
		public DigitalBackgroundLayer(string name) : base(name) { }
		protected override void OnCreate() {
			base.OnCreate();
			this.topLeftPointCore = PointF2D.Empty;
			this.bottomRightPointCore = new PointF2D(200, 100);
		}
		protected override DigitalBackgroundLayerProvider CreateProvider() {
			return new DigitalBackgroundLayerProvider(OnScaleIndependentComponentChanged);
		}
		const float MagicIntersectionFactor = 0.024f;
		protected override void CalculateScaleIndependentComponent() {
			BaseShape nearShape = Shapes[PredefinedShapeNames.DigitalBGNear];
			BaseShape centerShape = Shapes[PredefinedShapeNames.DigitalBGCenter];
			BaseShape farShape = Shapes[PredefinedShapeNames.DigitalBGFar];
			RectangleF box = centerShape.BoundingBox;
			RectangleF farBox = farShape.BoundingBox;
			SizeF size = new SizeF(BottomRight.X - TopLeft.X, BottomRight.Y - TopLeft.Y);
			SizeF scale = new SizeF(size.Width / box.Width, size.Height / box.Height);
			Transform = new Matrix(scale.Height, 0.0f, 0.0f, scale.Height, 
				TopLeft.X - box.Left * scale.Height, TopLeft.Y - box.Top * scale.Height);
			float w = size.Width / scale.Height;
			float f = MagicIntersectionFactor / (scale.Width / scale.Height);
			using(Matrix centerTransform = new Matrix(w / box.Width * (1.0f + f), 0, 0, 1f, -w * f * 0.5f, 0)) {
				centerShape.Accept(
						delegate(BaseShape s) {
							s.BeginUpdate();
							s.Transform.Multiply(centerTransform);
							s.EndUpdate();
						}
					);
			}
			using(Matrix farTransform = new Matrix(1f, 0, 0, 1f, w + (box.Left - farBox.Left), 0)) {
				farShape.Accept(
						delegate(BaseShape s) {
							s.BeginUpdate();
							s.Transform.Multiply(farTransform);
							s.EndUpdate();
						}
					);
			}
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("DigitalBackgroundLayerTopLeft"),
#endif
XtraSerializableProperty]
		public PointF2D TopLeft {
			get { return topLeftPointCore; }
			set {
				if(TopLeft == value) return;
				topLeftPointCore = value;
				Provider.PerformOwnerChanged("TopLeft");
			}
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("DigitalBackgroundLayerBottomRight"),
#endif
XtraSerializableProperty]
		public PointF2D BottomRight {
			get { return bottomRightPointCore; }
			set {
				if(BottomRight == value) return;
				bottomRightPointCore = value;
				Provider.PerformOwnerChanged("BottomRight");
			}
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("DigitalBackgroundLayerShape"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShape Shape {
			get { return Provider.Shape; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("DigitalBackgroundLayerShapeNear"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShape ShapeNear {
			get { return Provider.ShapeNear; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("DigitalBackgroundLayerShapeCenter"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShape ShapeCenter {
			get { return Provider.ShapeCenter; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("DigitalBackgroundLayerShapeFar"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShape ShapeFar {
			get { return Provider.ShapeFar; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("DigitalBackgroundLayerShapeType"),
#endif
DefaultValue(DigitalBackgroundShapeSetType.Default)]
		[XtraSerializableProperty]
		public DigitalBackgroundShapeSetType ShapeType {
			get { return Provider.ShapeType; }
			set { Provider.ShapeType = value; }
		}
		protected override void LoadShape() {
			Shapes.Add(Provider.ShapeNear.Clone());
			Shapes.Add(Provider.ShapeFar.Clone());
			Shapes.Add(Provider.ShapeCenter.Clone());
		}
		internal bool ShouldSerializeTopLeft() {
			return TopLeft != PointF2D.Empty;
		}
		internal bool ShouldSerializeBottomRight() {
			return BottomRight != new PointF2D(200, 100);
		}
		internal void ResetTopLeft() {
			TopLeft = PointF2D.Empty;
		}
		internal void ResetBottomRight() {
			BottomRight = new PointF2D(200, 100);
		}
		public void Assign(DigitalBackgroundLayer source) {
			BeginUpdate();
			AssignPrimitiveProperties(source);
			if(source != null) {
				this.ShapeType = source.ShapeType;
				this.TopLeft = source.TopLeft;
				this.BottomRight = source.BottomRight;
			}
			EndUpdate();
		}
		public bool IsDifferFrom(DigitalBackgroundLayer source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
					(this.TopLeft != source.TopLeft) ||
					(this.BottomRight != source.BottomRight) ||
					(this.ShapeType != source.ShapeType);
		}
	}
}
