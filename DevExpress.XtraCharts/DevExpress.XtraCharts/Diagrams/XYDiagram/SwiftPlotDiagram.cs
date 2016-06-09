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
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(SwiftPlotDiagramTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SwiftPlotDiagram : XYDiagram2D, ISwiftPlotDiagram {
		readonly SwiftPlotDiagramAxisX axisX;
		readonly SwiftPlotDiagramAxisY axisY;
		readonly SwiftPlotDiagramSecondaryAxisXCollection secondaryAxesX;
		readonly SwiftPlotDiagramSecondaryAxisYCollection secondaryAxesY;
		protected internal override bool DependsOnBounds { get { return HasSmartAxisX; } }
		protected internal override bool ActualRotated { get { return false; } }
		protected internal override Axis2D ActualAxisX { get { return axisX; } }
		protected internal override Axis2D ActualAxisY { get { return axisY; } }
		protected internal override SecondaryAxisCollection ActualSecondaryAxesX { get { return secondaryAxesX; } }
		protected internal override SecondaryAxisCollection ActualSecondaryAxesY { get { return secondaryAxesY; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SwiftPlotDiagramAxisX"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SwiftPlotDiagram.AxisX"),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public SwiftPlotDiagramAxisX AxisX { get { return axisX; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SwiftPlotDiagramAxisY"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SwiftPlotDiagram.AxisY"),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public SwiftPlotDiagramAxisY AxisY { get { return axisY; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SwiftPlotDiagramSecondaryAxesX"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SwiftPlotDiagram.SecondaryAxesX"),
		TypeConverter(typeof(CollectionTypeConverter)),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.SwiftPlotDiagramSecondaryAxisXCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public SwiftPlotDiagramSecondaryAxisXCollection SecondaryAxesX { get { return secondaryAxesX; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SwiftPlotDiagramSecondaryAxesY"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SwiftPlotDiagram.SecondaryAxesY"),
		TypeConverter(typeof(CollectionTypeConverter)),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.SwiftPlotDiagramSecondaryAxisYCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public SwiftPlotDiagramSecondaryAxisYCollection SecondaryAxesY { get { return secondaryAxesY; } }
		public SwiftPlotDiagram()
			: base() {
			axisX = new SwiftPlotDiagramAxisX(this);
			axisY = new SwiftPlotDiagramAxisY(this);
			secondaryAxesX = new SwiftPlotDiagramSecondaryAxisXCollection(this);
			secondaryAxesY = new SwiftPlotDiagramSecondaryAxisYCollection(this);
		}
		protected override void XtraSetIndexCollectionItem(string propertyName, object item) {
			switch (propertyName) {
				case "SecondaryAxesX":
					SecondaryAxesX.Add((SwiftPlotDiagramSecondaryAxisX)item);
					break;
				case "SecondaryAxesY":
					SecondaryAxesY.Add((SwiftPlotDiagramSecondaryAxisY)item);
					break;
				default:
					base.XtraSetIndexCollectionItem(propertyName, item);
					break;
			}
		}
		protected override object XtraCreateCollectionItem(string propertyName) {
			switch (propertyName) {
				case "SecondaryAxesX":
					return new SwiftPlotDiagramSecondaryAxisX();
				case "SecondaryAxesY":
					return new SwiftPlotDiagramSecondaryAxisY();
				default:
					return base.XtraCreateCollectionItem(propertyName);
			}
		}
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "AxisX":
					return ShouldSerializeAxisX();
				case "AxisY":
					return ShouldSerializeAxisY();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		bool ShouldSerializeAxisX() {
			return axisX.ShouldSerialize();
		}
		bool ShouldSerializeAxisY() {
			return axisY.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeAxisX() || ShouldSerializeAxisY() || secondaryAxesX.Count > 0 || secondaryAxesY.Count > 0;
		}
		protected internal override XYDiagramMappingBase CreateDiagramMapping(XYDiagramMappingContainer container, AxisIntervalLayout layoutX, AxisIntervalLayout layoutY) {
			return new SwiftPlotDiagramMapping(container, layoutX, layoutY);
		}
		protected internal override bool Contains(object obj) {
			return
				base.Contains(obj) ||
				obj == axisX ||
				obj == axisY ||
				axisX.Contains(obj) ||
				axisY.Contains(obj) ||
				secondaryAxesX.ContainsWithChildren(obj) ||
				secondaryAxesY.ContainsWithChildren(obj);
		}
		protected override ChartElement CreateObjectForClone() {
			return new SwiftPlotDiagram();
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				axisX.Dispose();
				axisY.Dispose();
				secondaryAxesX.Dispose();
				secondaryAxesY.Dispose();
			}
			base.Dispose(disposing);
		}
		protected internal override void OnEndLoading() {
			base.OnEndLoading();
			axisX.OnEndLoading();
			axisY.OnEndLoading();
			foreach (SwiftPlotDiagramSecondaryAxisX secondaryAxisX in secondaryAxesX)
				secondaryAxisX.OnEndLoading();
			foreach (SwiftPlotDiagramSecondaryAxisY secondaryAxisY in secondaryAxesY)
				secondaryAxisY.OnEndLoading();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SwiftPlotDiagram diagram = obj as SwiftPlotDiagram;
			if (diagram == null)
				return;
			axisX.Assign(diagram.axisX);
			axisY.Assign(diagram.axisY);
			secondaryAxesX.Assign(diagram.secondaryAxesX);
			secondaryAxesY.Assign(diagram.secondaryAxesY);
		}
	}
}
