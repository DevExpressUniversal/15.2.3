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
namespace DevExpress.XtraGauges.Core.Model {
	public class BaseSpindleCapProvider : BaseLayerProvider<SpindleCapShapeType> {
		public BaseSpindleCapProvider(OwnerChangedAction spindleCapChanged)
			: base(spindleCapChanged) {
		}
		protected override SizeF InitSize() {
			return new SizeF(40f, 40f);
		}
		protected override SpindleCapShapeType DefaultShapeType {
			get { return SpindleCapShapeType.CircularFull_Default; }
		}
		protected override BaseShape GetShape(SpindleCapShapeType value) {
			return SpindleCapShapeFactory.GetDefaultSpindleCapShape(value);
		}
	}
	public class ArcScaleSpindleCap : LayerComponent<BaseSpindleCapProvider>,
		ISpindleCap, IArcScaleComponent, ISupportAssign<ArcScaleSpindleCap> {
		public ArcScaleSpindleCap() : base() { }
		public ArcScaleSpindleCap(string name) : base(name) { }
		protected override bool AllowCacheRenderOperation {
			get { return false; }
		}
		protected override IScale GetDefaultScaleCore() {
			return ScaleFactory.EmptyArcScale;
		}
		protected override BaseSpindleCapProvider CreateProvider() {
			return new BaseSpindleCapProvider(OnScaleDependentComponentChanged);
		}
		protected override void CalculateScaleDependentComponent() {
			if(ShapeType == SpindleCapShapeType.Empty) return;
			BaseShape spindleCapShape = Shapes[PredefinedShapeNames.SpindleCap];
			RectangleF box = spindleCapShape.BoundingBox;
			PointF p = new PointF(box.Left + box.Width / 2f, box.Top + box.Height / 2f);
			SizeF scale = new SizeF(Size.Width / box.Width, Size.Height / box.Height);
			Transform = new Matrix(scale.Width, 0.0f, 0.0f, scale.Height, ArcScale.Center.X - p.X * scale.Width, ArcScale.Center.Y - p.Y * scale.Height);
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleSpindleCapSize"),
#endif
 XtraSerializableProperty]
		public SizeF Size {
			get { return Provider.Size; }
			set { Provider.Size = value; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleSpindleCapScale")]
#endif
		public IScale Scale {
			get { return ScaleCore; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleSpindleCapArcScale")]
#endif
		public IArcScale ArcScale {
			get { return ScaleCore as IArcScale; }
			set { ScaleCore = value; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleSpindleCapShape")]
#endif
		public BaseShape Shape {
			get { return Provider.Shape; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleSpindleCapShapeType")]
#endif
		[XtraSerializableProperty, DefaultValue(SpindleCapShapeType.CircularFull_Default)]
		public SpindleCapShapeType ShapeType {
			get { return Provider.ShapeType; }
			set { Provider.ShapeType = value; }
		}
		public void Assign(ArcScaleSpindleCap source) {
			BeginUpdate();
			AssignPrimitiveProperties(source);
			if(source != null) {
				this.ArcScale = source.ArcScale;
				this.ShapeType = source.ShapeType;
				this.Size = source.Size;
			}
			EndUpdate();
		}
		public bool IsDifferFrom(ArcScaleSpindleCap source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
				(this.ArcScale != source.ArcScale) ||
				(this.ShapeType != source.ShapeType) ||
				(this.Size != source.Size);
		}
		internal bool ShouldSerializeSize() {
			return Size != new SizeF(40, 40);
		}
		internal void ResetSize() {
			Size = new SizeF(40, 40);
		}
	}
}
