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

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(XYDiagramTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class XYDiagram : XYDiagram2D {
		readonly AxisX axisX;
		readonly AxisY axisY;
		readonly SecondaryAxisXCollection secondaryAxesX;
		readonly SecondaryAxisYCollection secondaryAxesY;
		bool rotated;
		protected virtual bool DefaultRotatedValue { get { return false; } }
		protected internal override bool ActualRotated { get { return rotated; } }
		protected internal override Axis2D ActualAxisX { get { return axisX; } }
		protected internal override Axis2D ActualAxisY { get { return axisY; } }
		protected internal override SecondaryAxisCollection ActualSecondaryAxesX { get { return secondaryAxesX; } }
		protected internal override SecondaryAxisCollection ActualSecondaryAxesY { get { return secondaryAxesY; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramAxisX"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram.AxisX"),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public AxisX AxisX { get { return axisX; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramAxisY"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram.AxisY"),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public AxisY AxisY { get { return axisY; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramSecondaryAxesX"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram.SecondaryAxesX"),
		TypeConverter(typeof(CollectionTypeConverter)),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.SecondaryAxisXCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public SecondaryAxisXCollection SecondaryAxesX { get { return secondaryAxesX; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramSecondaryAxesY"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram.SecondaryAxesY"),
		TypeConverter(typeof(CollectionTypeConverter)),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.SecondaryAxisYCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public SecondaryAxisYCollection SecondaryAxesY { get { return secondaryAxesY; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramRotated"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram.Rotated"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public virtual bool Rotated {
			get { return rotated; }
			set {
				if (value != rotated) {
					SendNotification(new ElementWillChangeNotification(this));
					rotated = value;
					RaiseControlChanged();
				}
			}
		}
		public XYDiagram()
			: base() {
			axisX = CreateAxisX();
			axisY = new AxisY(this);
			secondaryAxesX = new SecondaryAxisXCollection(this);
			secondaryAxesY = new SecondaryAxisYCollection(this);
			rotated = DefaultRotatedValue;
		}
		#region XtraSeriealization
		protected override void XtraSetIndexCollectionItem(string propertyName, object item) {
			switch (propertyName) {
				case "SecondaryAxesX":
					SecondaryAxesX.Add((SecondaryAxisX)item);
					break;
				case "SecondaryAxesY":
					SecondaryAxesY.Add((SecondaryAxisY)item);
					break;
				default:
					base.XtraSetIndexCollectionItem(propertyName, item);
					break;
			}
		}
		protected override object XtraCreateCollectionItem(string propertyName) {
			switch (propertyName) {
				case "SecondaryAxesX":
					return new SecondaryAxisX();
				case "SecondaryAxesY":
					return new SecondaryAxisY();
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
				case "Rotated":
					return ShouldSerializeRotated();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeAxisX() {
			return axisX.ShouldSerialize();
		}
		bool ShouldSerializeAxisY() {
			return axisY.ShouldSerialize();
		}
		bool ShouldSerializeRotated() {
			return rotated != DefaultRotatedValue;
		}
		void ResetRotated() {
			Rotated = DefaultRotatedValue;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeRotated() || ShouldSerializeAxisX() || ShouldSerializeAxisY() ||
				secondaryAxesX.Count > 0 || secondaryAxesY.Count > 0;
		}
		#endregion
		protected virtual AxisX CreateAxisX() {
			return new AxisX(this);
		}
		protected override ChartElement CreateObjectForClone() {
			return new XYDiagram();
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
		protected override void OnOwnerChanged(ChartElement oldOwner, ChartElement newOwner) {
			base.OnOwnerChanged(oldOwner, newOwner);
			List<IAxisData> list = new List<IAxisData>();
			list.Add(AxisX);
			list.Add(AxisY);
			RaiseControlChanged(new AxisCollectionBatchUpdateInfo(this, ChartCollectionOperation.InsertItem, null, 0, list, 0));
		}
		protected void SetRotated(bool rotated) {
			this.rotated = rotated;
		}
		protected internal override void BeginZooming() {
			base.BeginZooming();
			SendNotification(new ElementWillChangeNotification(this));
		}
		protected internal override XYDiagramMappingBase CreateDiagramMapping(XYDiagramMappingContainer container, AxisIntervalLayout layoutX, AxisIntervalLayout layoutY) {
			return new XYDiagramMapping(container, layoutX, layoutY);
		}
		protected internal override bool Contains(object obj) {
			return
				base.Contains(obj) ||
				obj == this.axisX ||
				obj == this.axisY ||
				this.axisX.Contains(obj) ||
				this.axisY.Contains(obj) ||
				this.secondaryAxesX.ContainsWithChildren(obj) ||
				this.secondaryAxesY.ContainsWithChildren(obj);
		}
		protected internal override void OnEndLoading() {
			base.OnEndLoading();
			axisX.OnEndLoading();
			axisY.OnEndLoading();
			foreach (SecondaryAxisX secondaryAxisX in secondaryAxesX)
				secondaryAxisX.OnEndLoading();
			foreach (SecondaryAxisY secondaryAxisY in secondaryAxesY)
				secondaryAxisY.OnEndLoading();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			XYDiagram diagram = obj as XYDiagram;
			if (diagram == null)
				return;
			axisX.Assign(diagram.axisX);
			axisY.Assign(diagram.axisY);
			secondaryAxesX.Assign(diagram.secondaryAxesX);
			secondaryAxesY.Assign(diagram.secondaryAxesY);
			rotated = diagram.rotated;
		}
	}
}
