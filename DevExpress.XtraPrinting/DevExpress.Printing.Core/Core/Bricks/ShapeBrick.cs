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

using DevExpress.XtraPrinting.Native;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting.Shape;
using DevExpress.XtraPrinting.Shape.Native;
using DevExpress.Utils.Serializing;
using System.ComponentModel;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting {
	public interface IShapeBaseOwner {
		int Angle { get; set; }
	}
	[BrickExporter(typeof(ShapeBrickExporter))]
	public class ShapeBrick : VisualBrick, IShapeDrawingInfo, IShapeBaseOwner {
		ShapeBase shape;
		float lineWidth = 1;
		DashStyle lineStyle = DashStyle.Solid;
		int angle;
		Color fillColor = Color.Transparent;
		bool stretch;
		#region hidden properties
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override object TextValue { get { return base.TextValue; } set { base.TextValue = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string TextValueFormatString { get { return base.TextValueFormatString; } set { base.TextValueFormatString = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string XlsxFormatString { get { return base.XlsxFormatString; } set { base.XlsxFormatString = value; } }
		#endregion
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ShapeBrickShape"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public ShapeBase Shape { get { return shape; } set { shape = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ShapeBrickLineWidth"),
#endif
		XtraSerializableProperty,
		DefaultValue(1f)]
		public float LineWidth { get { return lineWidth; } set { lineWidth = ShapeHelper.ValidateRestrictedValue(value, 0, int.MaxValue, "LineWidth"); } }
		[
		XtraSerializableProperty,
		DefaultValue(DashStyle.Solid)]
		public DashStyle LineStyle { get { return lineStyle; } set { lineStyle = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ShapeBrickAngle"),
#endif
		XtraSerializableProperty,
		DefaultValue(0)]
		public int Angle { get { return angle; } set { angle = ShapeHelper.ValidateAngle(value); } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ShapeBrickStretch"),
#endif
		XtraSerializableProperty,
		DefaultValue(false)]
		public bool Stretch { get { return stretch; } set { stretch = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ShapeBrickFillColor"),
#endif
		XtraSerializableProperty]
		public Color FillColor { get { return fillColor; } set { fillColor = value; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ShapeBrickForeColor")]
#endif
		public Color ForeColor { get { return Style.ForeColor; } set { Style = BrickStyleHelper.ChangeForeColor(Style, value); } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("ShapeBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.Shape; } }
		public ShapeBrick()
			: this(NullBrickOwner.Instance) {
		}
		public ShapeBrick(IBrickOwner brickOwner)
			: base(brickOwner) {
			shape = ShapeFactory.DefaultFactory.CreateShape(this);
		}
		#region serialization
		protected override object CreateContentPropertyValue(XtraItemEventArgs e) {
			if(e.Item.Name == PrintingSystemSerializationNames.Shape)
				return ShapeFactory.CreateByType(BrickFactory.GetStringProperty(e, "ShapeName"));
			return base.CreateContentPropertyValue(e);
		}
		#endregion
		protected internal override void Scale(double scaleFactor) {
			base.Scale(scaleFactor);
			lineWidth = MathMethods.Scale(lineWidth, scaleFactor);
		}
		protected override bool ShouldSerializeCore(string propertyName) {
			if(propertyName == PrintingSystemSerializationNames.FillColor)
				return FillColor != Color.Transparent;
			return base.ShouldSerializeCore(propertyName);
		}
	}
}
