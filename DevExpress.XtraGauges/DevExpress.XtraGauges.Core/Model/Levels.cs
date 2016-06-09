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
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.XtraGauges.Core.Model {
	public class BaseScaleLevelProvider : BaseShapeProvider<LevelShapeSetType>, IScaleLevel {
		BaseShape barStartShapeCore;
		BaseShape barPackedShapeCore;
		BaseShape barEmptyShapeCore;
		BaseShape barEndShapeCore;
		public BaseScaleLevelProvider(OwnerChangedAction ownerChanged)
			: base(ownerChanged) {
		}
		protected override LevelShapeSetType DefaultShapeType {
			get { return LevelShapeSetType.Default; }
		}
		protected override BaseShape GetShape(LevelShapeSetType value) {
			return ScaleLevelShapeFactory.GetDefaultLevelShape(value);
		}
		protected override void DestroyShape() {
			base.DestroyShape();
			DestroyShapeCore(ref barStartShapeCore);
			DestroyShapeCore(ref barPackedShapeCore);
			DestroyShapeCore(ref barEmptyShapeCore);
			DestroyShapeCore(ref barEndShapeCore);
		}
		protected override void SetShape(BaseShape value) {
			ComplexShape template = value as ComplexShape;
			SetShapeCore(ref barStartShapeCore, template.Collection[PredefinedShapeNames.BarStart]);
			SetShapeCore(ref barPackedShapeCore, template.Collection[PredefinedShapeNames.BarPacked]);
			SetShapeCore(ref barEmptyShapeCore, template.Collection[PredefinedShapeNames.BarEmpty]);
			SetShapeCore(ref barEndShapeCore, template.Collection[PredefinedShapeNames.BarEnd]);
			base.SetShape(value);
		}
		public IScale Scale {
			get { return null; }
		}
		public BaseShape BarStartShape {
			get { return barStartShapeCore; }
		}
		public BaseShape BarPackedShape {
			get { return barPackedShapeCore; }
		}
		public BaseShape BarEmptyShape {
			get { return barEmptyShapeCore; }
		}
		public BaseShape BarEndShape {
			get { return barEndShapeCore; }
		}
	}
	public class LinearScaleLevel : ValueIndicatorComponent<BaseScaleLevelProvider>,
		IScaleLevel, ILinearScalePointer, ISupportAssign<LinearScaleLevel> {
		public LinearScaleLevel() : base() { }
		public LinearScaleLevel(string name) : base(name) { }
		protected override IScale GetDefaultScaleCore() {
			return ScaleFactory.EmptyLinearScale;
		}
		protected override BaseScaleLevelProvider CreateProvider() {
			return new BaseScaleLevelProvider(OnScaleDependentComponentChanged);
		}
		protected override void CalculateScaleDependentComponent() {
			BaseShape bar = Shapes[PredefinedShapeNames.BarPacked];
			BaseShape empty = Shapes[PredefinedShapeNames.BarEmpty];
			RectangleF box = new RectangleF(bar.BoundingBox.Left, empty.BoundingBox.Top, bar.BoundingBox.Width, bar.BoundingBox.Height + empty.BoundingBox.Height);
			float barPercent = bar.BoundingBox.Height / box.Height;
			float etubePercent = empty.BoundingBox.Height / box.Height;
			PointF shapePt1 = new PointF(box.Left + box.Width * 0.5f, box.Top);
			PointF shapePt2 = new PointF(box.Left + box.Width * 0.5f, box.Top + box.Height);
			Transform = MathHelper.CalcMorphTransform(shapePt1, shapePt2, LinearScale.EndPoint, LinearScale.StartPoint);
			float p = LinearScale.ValueToPercent(ActualValue);
			Matrix barTransform = new Matrix(1f, 0, 0, p / barPercent, 0, box.Bottom - box.Height * p - bar.BoundingBox.Top);
			bar.Accept(
					delegate(BaseShape s) {
						s.BeginUpdate();
						s.Transform.Multiply(barTransform);
						s.EndUpdate();
					}
				);
			barTransform.Dispose();
			Matrix emptyTransform = new Matrix(1f, 0, 0, (1f - p) / etubePercent, 0, 0);
			empty.Accept(
					delegate(BaseShape s) {
						s.BeginUpdate();
						s.Transform.Multiply(emptyTransform);
						s.EndUpdate();
					}
				);
			emptyTransform.Dispose();
		}
		#region Properties
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleLevelScale")]
#endif
		public IScale Scale {
			get { return ScaleCore; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleLevelLinearScale")]
#endif
		public ILinearScale LinearScale {
			get { return ScaleCore as ILinearScale; }
			set { ScaleCore = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleLevelBarStartShape"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShape BarStartShape {
			get { return Provider.BarStartShape; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleLevelBarPackedShape"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShape BarPackedShape {
			get { return Provider.BarPackedShape; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleLevelBarEmptyShape"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShape BarEmptyShape {
			get { return Provider.BarEmptyShape; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleLevelBarEndShape"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseShape BarEndShape {
			get { return Provider.BarEndShape; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleLevelShapeType"),
#endif
DefaultValue(LevelShapeSetType.Default)]
		[XtraSerializableProperty]
		public LevelShapeSetType ShapeType {
			get { return Provider.ShapeType; }
			set { Provider.ShapeType = value; }
		}
		#endregion Properties
		protected override void LoadShape() {
			Shapes.Add(Provider.BarStartShape.Clone());
			Shapes.Add(Provider.BarPackedShape.Clone());
			Shapes.Add(Provider.BarEmptyShape.Clone());
			Shapes.Add(Provider.BarEndShape.Clone());
		}
		public void Assign(LinearScaleLevel source) {
			BeginUpdate();
			AssignPrimitiveProperties(source);
			if(source != null) {
				this.LinearScale = source.LinearScale;
				this.ShapeType = source.ShapeType;
				this.Value = source.Value;
			}
			EndUpdate();
		}
		public bool IsDifferFrom(LinearScaleLevel source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
					(this.LinearScale != source.LinearScale) ||
					(this.ShapeType != source.ShapeType) ||
					(this.Value != source.Value);
		}
	}
}
