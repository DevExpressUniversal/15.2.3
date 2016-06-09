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

using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	public abstract class Axis3D : AxisBase {
		RectangleFillStyle3D interlacedFillStyle;
		XYDiagram3DAppearance DiagramAppearance { get { return CommonUtils.GetActualAppearance(this).XYDiagram3DAppearance; } }
		internal Color ActualInterlacedColor {
			get {
				Color interlacedColor = InterlacedColor;
				return interlacedColor.IsEmpty ? DiagramAppearance.InterlacedColor : interlacedColor;
			}
		}
		internal RectangleFillStyle3D ActualInterlacedFillStyle {
			get {
				return interlacedFillStyle.FillMode == FillMode3D.Empty ?
					(RectangleFillStyle3D)DiagramAppearance.InterlacedFillStyle : interlacedFillStyle;
			}
		}
		protected override int DefaultMinorCount { get { return 4; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		NonTestableProperty
		]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis3DInterlacedFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis3D.InterlacedFillStyle"),
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleFillStyle3D InterlacedFillStyle { get { return interlacedFillStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis3DLabel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis3D.Label"),
		Category("Elements"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public new AxisLabel3D Label { get { return ActualLabel as AxisLabel3D; } }
		protected Axis3D(string name, XYDiagram3D diagram)
			: base(name, diagram) {
			interlacedFillStyle = new RectangleFillStyle3D(this);
		}
		protected override void Dispose(bool disposing) {
			if (disposing && interlacedFillStyle != null) {
				interlacedFillStyle.Dispose();
				interlacedFillStyle = null;
			}
			base.Dispose(disposing);
		}
		protected override AxisLabel CreateAxisLabel() {
			return new AxisLabel3D(this);
		}
		protected internal abstract DiagramPoint GetDiagramPoint(XYDiagram3DCoordsCalculator coordsCalculator, double axisArgument, double axisValue);
		protected internal abstract DiagramPoint GetLabelPoint(XYDiagram3DCoordsCalculator coordsCalculator, double value, int offset);
		protected internal abstract NearTextPosition GetNearTextPosition(XYDiagram3DCoordsCalculator coordsCalculator);
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Axis3D axis = obj as Axis3D;
			if (axis != null)
				interlacedFillStyle.Assign(axis.interlacedFillStyle);
		}
		public override string ToString() {
			return "(Axis)";
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeLabel() {
			return ActualLabel.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeLabel();
		}
		#endregion
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class AxisX3D : Axis3D {
		protected internal override bool IsValuesAxis { get { return false; } }
		protected override int GridSpacingFactor { get { return 50; } }
		protected internal override bool IsVertical { get { return false; } }
		internal AxisX3D(XYDiagram3D diagram)
			: base(ChartLocalizer.GetString(ChartStringId.PrimaryAxisXName), diagram) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new AxisX3D(null);
		}
		protected override AxisRange CreateAxisRange(RangeDataBase wholeAxisRange, RangeDataBase visibleAxisRange) {
			return new AxisXRange(this);
		}
		protected override GridLines CreateGridLines() {
			return new GridLinesX(this);
		}
		protected internal override DiagramPoint GetDiagramPoint(XYDiagram3DCoordsCalculator coordsCalculator, double axisArgument, double axisValue) {
			return coordsCalculator.GetDiagramPointForDiagram(axisArgument, axisValue);
		}
		protected internal override NearTextPosition GetNearTextPosition(XYDiagram3DCoordsCalculator coordsCalculator) {
			AxisLabel3D label3d = Label as AxisLabel3D;
			if (label3d != null && label3d.Position != AxisLabel3DPosition.Auto)
				return (NearTextPosition)label3d.Position;
			DiagramPoint zPoint = coordsCalculator.Project(DiagramPoint.Zero);
			DiagramPoint yPoint = coordsCalculator.Project(new DiagramPoint(0, coordsCalculator.Height, 0));
			return yPoint.Y >= zPoint.Y ? NearTextPosition.Top : NearTextPosition.Bottom;
		}
		protected internal override DiagramPoint GetLabelPoint(XYDiagram3DCoordsCalculator coordsCalculator, double value, int offset) {
			DiagramPoint point = coordsCalculator.GetDiagramPointForDiagram(value, double.NegativeInfinity);
			BoxPlane planes = coordsCalculator.CalcVisiblePlanes();
			point.Y += offset - coordsCalculator.Diagram.PlaneDepthFixed;
			if ((planes & BoxPlane.Back) == BoxPlane.Back)
				point.Z += coordsCalculator.Depth;
			return point;
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class AxisY3D : Axis3D {
		protected internal override bool IsValuesAxis { get { return true; } }
		protected override int GridSpacingFactor { get { return 30; } }
		protected override bool DefaultInterlaced { get { return true; } }
		protected internal override bool IsVertical { get { return true; } }
		internal AxisY3D(XYDiagram3D diagram)
			: base(ChartLocalizer.GetString(ChartStringId.PrimaryAxisYName), diagram) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new AxisY3D(null);
		}
		protected override AxisRange CreateAxisRange(RangeDataBase wholeAxisRange, RangeDataBase visibleAxisRange) {
			return new AxisYRange(this);
		}
		protected override GridLines CreateGridLines() {
			return new GridLinesY(this);
		}
		protected internal override DiagramPoint GetDiagramPoint(XYDiagram3DCoordsCalculator coordsCalculator, double axisArgument, double axisValue) {
			return coordsCalculator.GetDiagramPointForDiagram(axisValue, axisArgument);
		}
		protected internal override NearTextPosition GetNearTextPosition(XYDiagram3DCoordsCalculator coordsCalculator) {
			AxisLabel3D label3d = Label as AxisLabel3D;
			if (label3d != null && label3d.Position != AxisLabel3DPosition.Auto)
				return (NearTextPosition)label3d.Position;
			BoxPlane planes = coordsCalculator.CalcVisiblePlanes();
			NearTextPosition position = (planes & BoxPlane.Right) == BoxPlane.Right ? NearTextPosition.Right : NearTextPosition.Left;
			DiagramPoint zPoint = coordsCalculator.Project(DiagramPoint.Zero);
			DiagramPoint xPoint = coordsCalculator.Project(new DiagramPoint(coordsCalculator.Width, 0, 0));
			if (xPoint.X >= zPoint.X)
				return position;
			return position == NearTextPosition.Right ? NearTextPosition.Left : NearTextPosition.Right;
		}
		protected internal override DiagramPoint GetLabelPoint(XYDiagram3DCoordsCalculator coordsCalculator, double value, int offset) {
			DiagramPoint point = coordsCalculator.GetDiagramPointForDiagram(double.NegativeInfinity, value);
			BoxPlane planes = coordsCalculator.CalcVisiblePlanes();
			if ((planes & BoxPlane.Right) == BoxPlane.Right)
				point.X += coordsCalculator.Width - coordsCalculator.Diagram.PlaneDepthFixed - offset;
			else
				point.X += offset - coordsCalculator.Diagram.PlaneDepthFixed;
			if ((planes & BoxPlane.Back) == BoxPlane.Back)
				point.Z += coordsCalculator.Depth;
			return point;
		}
	}
}
