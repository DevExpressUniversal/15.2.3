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
	public class DigitalEffectLayerProvider : BaseLayerProvider<DigitalEffectShapeType> {
		public DigitalEffectLayerProvider(OwnerChangedAction layerChanged)
			: base(layerChanged) {
		}
		protected override DigitalEffectShapeType DefaultShapeType {
			get { return DigitalEffectShapeType.Default; }
		}
		protected override BaseShape GetShape(DigitalEffectShapeType value) {
			return DigitalEffectShapeFactory.GetDigitalEffectShape(value);
		}
	}
	public class DigitalEffectLayer : ScaleIndependentLayerComponent<DigitalEffectLayerProvider>,
		IDigitalEffectLayer, ISupportAssign<DigitalEffectLayer> {
		PointF2D topLeftPointCore;
		PointF2D bottomRightPointCore;
		public DigitalEffectLayer() : base() { }
		public DigitalEffectLayer(string name) : base(name) { }
		protected override void OnCreate() {
			base.OnCreate();
			this.topLeftPointCore = PointF2D.Empty;
			this.bottomRightPointCore = new PointF2D(200, 100);
		}
		protected override DigitalEffectLayerProvider CreateProvider() {
			return new DigitalEffectLayerProvider(OnScaleIndependentComponentChanged);
		}
		protected override void CalculateScaleIndependentComponent() {
			if(ShapeType == DigitalEffectShapeType.Empty) return;
			BaseShape shape = Shapes[PredefinedShapeNames.Effect];
			RectangleF box = shape.BoundingBox;
			SizeF size = new SizeF(BottomRight.X - TopLeft.X, BottomRight.Y - TopLeft.Y);
			SizeF scale = new SizeF(size.Width / box.Width, size.Height / box.Height);
			Transform = new Matrix(scale.Height, 0.0f, 0.0f, scale.Height, TopLeft.X - box.Left * scale.Height, TopLeft.Y - box.Top * scale.Height);
			float w = size.Width / scale.Height;
			Matrix transform = new Matrix(w / box.Width, 0, 0, 1f, 0, 0);
			shape.Accept(
					delegate(BaseShape s) {
						s.BeginUpdate();
						s.Transform.Multiply(transform);
						s.EndUpdate();
					}
				);
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("DigitalEffectLayerTopLeft"),
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
	DevExpressXtraGaugesCoreLocalizedDescription("DigitalEffectLayerBottomRight"),
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
	DevExpressXtraGaugesCoreLocalizedDescription("DigitalEffectLayerShape"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShape Shape {
			get { return Provider.Shape; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("DigitalEffectLayerShapeType"),
#endif
DefaultValue(DigitalEffectShapeType.Default)]
		[XtraSerializableProperty]
		public DigitalEffectShapeType ShapeType {
			get { return Provider.ShapeType; }
			set { Provider.ShapeType = value; }
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
		public void Assign(DigitalEffectLayer source) {
			BeginUpdate();
			AssignPrimitiveProperties(source);
			if(source != null) {
				this.ShapeType = source.ShapeType;
				this.TopLeft = source.TopLeft;
				this.BottomRight = source.BottomRight;
			}
			EndUpdate();
		}
		public bool IsDifferFrom(DigitalEffectLayer source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
					(this.TopLeft != source.TopLeft) ||
					(this.BottomRight != source.BottomRight) ||
					(this.ShapeType != source.ShapeType);
		}
	}
}
