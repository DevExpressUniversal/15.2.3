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
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraGauges.Core.Model {
	public class BaseNeedleProvider : BaseShapeProvider<NeedleShapeType>, INeedle {
		float startOffsetCore;
		float endOffsetCore;
		public BaseNeedleProvider(OwnerChangedAction needleChanged)
			: base(needleChanged) {
		}
		protected override void OnCreate() {
			this.startOffsetCore = 0f;
			this.endOffsetCore = 0f;
			base.OnCreate();
		}
		protected override NeedleShapeType DefaultShapeType {
			get { return NeedleShapeType.Circular_Default; }
		}
		protected override BaseShape GetShape(NeedleShapeType value) {
			return NeedleShapeFactory.GetDefaultNeedleShape(value);
		}
		public IScale Scale {
			get { return null; }
		}
		public float StartOffset {
			get { return startOffsetCore; }
			set {
				if(StartOffset == value) return;
				startOffsetCore = value;
				OnObjectChanged("StartOffset");
			}
		}
		public float EndOffset {
			get { return endOffsetCore; }
			set {
				if(EndOffset == value) return;
				endOffsetCore = value;
				OnObjectChanged("EndOffset");
			}
		}
	}
	public class ArcScaleNeedle : ValueIndicatorComponent<BaseNeedleProvider>, 
		IArcScalePointer, INeedle, ISupportAssign<ArcScaleNeedle> {
		public ArcScaleNeedle() { }
		public ArcScaleNeedle(string name) : base(name) { }
		protected override IScale GetDefaultScaleCore() {
			return ScaleFactory.EmptyArcScale;
		}
		protected override BaseNeedleProvider CreateProvider() {
			return new BaseNeedleProvider(OnScaleDependentComponentChanged);
		}
		protected override void CalculateScaleDependentComponent() {
			BaseShape needleShape = Shapes[PredefinedShapeNames.Needle];
			RectangleF box = needleShape.BoundingBox;
			PointF shapePt1 = new PointF(box.Left, box.Top + box.Height / 2f);
			PointF shapePt2 = new PointF(box.Right, box.Top + box.Height / 2f);
			float range = ArcScale.EndAngle - ArcScale.StartAngle;
			float alpha = ArcScale.StartAngle + ArcScale.ValueToPercent(ActualValue) * range;
			SizeF offset = new SizeF(ArcScale.Center.X, ArcScale.Center.Y);
			if(ArcScale.RadiusX == 0)
				ArcScale.RadiusX = 0.0001F;
			if(ArcScale.RadiusY == 0)
				ArcScale.RadiusY = 0.0001F;
			PointF start = MathHelper.GetRadiusVector(StartOffset, StartOffset * ArcScale.RadiusY / ArcScale.RadiusX, alpha) + offset;
			PointF end = MathHelper.GetRadiusVector(ArcScale.RadiusX - EndOffset, ArcScale.RadiusY - EndOffset, alpha) + offset;
			Transform = MathHelper.CalcMorphTransform(shapePt1, shapePt2, start, end);
		}
		#region Properties
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleNeedleScale")]
#endif
		public IScale Scale {
			get { return ScaleCore; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleNeedleArcScale")]
#endif
		public IArcScale ArcScale {
			get { return ScaleCore as IArcScale; }
			set { ScaleCore = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleNeedleStartOffset"),
#endif
 DefaultValue(0f)]
		[XtraSerializableProperty]
		public float StartOffset {
			get { return Provider.StartOffset; }
			set { Provider.StartOffset = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleNeedleEndOffset"),
#endif
 DefaultValue(0f)]
		[XtraSerializableProperty]
		public float EndOffset {
			get { return Provider.EndOffset; }
			set { Provider.EndOffset = value; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleNeedleShape")]
#endif
		public BaseShape Shape {
			get { return Provider.Shape; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleNeedleShapeType"),
#endif
 DefaultValue(NeedleShapeType.Circular_Default)]
		[XtraSerializableProperty]
		public NeedleShapeType ShapeType {
			get { return Provider.ShapeType; }
			set { Provider.ShapeType = value; }
		}
		#endregion Properties
		public void Assign(ArcScaleNeedle source) {
			BeginUpdate();
			AssignPrimitiveProperties(source);
			if(source != null) {
				this.ArcScale = source.ArcScale;
				this.StartOffset = source.StartOffset;
				this.EndOffset = source.EndOffset;
				this.ShapeType = source.ShapeType;
				this.Value = source.Value;
			}
			EndUpdate();
		}
		public bool IsDifferFrom(ArcScaleNeedle source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
				(this.ArcScale != source.ArcScale) ||
				(this.StartOffset != source.StartOffset) ||
				(this.EndOffset != source.EndOffset) ||
				(this.ShapeType != source.ShapeType) ||
				(this.Value != source.Value);
		}
	}
}
