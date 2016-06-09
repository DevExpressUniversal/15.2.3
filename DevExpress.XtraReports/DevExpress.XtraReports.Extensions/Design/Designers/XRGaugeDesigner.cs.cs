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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design {
	public class XRGaugeDesigner : XRControlDesigner {
		public XRGaugeDesigner()
			: base() {
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRGaugeDesignerActionList1(this));
			list.Add(new XRGaugeDesignerActionList2(this));
			list.Add(new XRGaugeDesignerActionList3(this));
			list.Add(new XRGaugeDesignerActionList4(this));
			list.Add(new XRGaugeDesignerActionList5(this));
		}
	}
	public class XRGaugeDesignerActionList1 : XRComponentDesignerActionList {
		XRGauge Gauge { get { return (XRGauge)Component; } }
		public DashboardGaugeTheme ViewTheme {
			get { return Gauge.ViewTheme; }
			set { SetPropertyValue("ViewTheme", value); }
		}
		[
		TypeConverter(typeof(DevExpress.XtraReports.Design.DashboardGaugeStyleConverter)),
		]
		public DashboardGaugeStyle ViewStyle {
			get { return Gauge.ViewStyle; }
			set { SetPropertyValue("ViewStyle", value); }
		}
		[
		RefreshProperties(RefreshProperties.All),
		]
		public DashboardGaugeType ViewType {
			get { return Gauge.ViewType; }
			set { SetPropertyValue("ViewType", value); }
		}
		public XRGaugeDesignerActionList1(XRGaugeDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "ViewType", "ViewType");
			AddPropertyItem(actionItems, "ViewStyle", "ViewStyle");
			AddPropertyItem(actionItems, "ViewTheme", "ViewTheme");
		}
	}
	public class XRGaugeDesignerActionList2 : XRControlBaseDesignerActionList {
		XRGauge Gauge { get { return (XRGauge)Component; } }
		public float? ActualValue {
			get { return Gauge.ActualValue; }
			set { SetPropertyValue("ActualValue", value); }
		}
		public DesignBinding DataBinding {
			get { return ControlDesigner.GetDesignBinding("ActualValue"); }
			set { ControlDesigner.SetBinding("ActualValue", value); }
		}
		public XRGaugeDesignerActionList2(XRGaugeDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "ActualValue", "ActualValue");
			actionItems.Add(CreatePropertyItem("DataBinding", ReportStringId.STag_Name_DataBinding));
		}
	}
	public class XRGaugeDesignerActionList3 : XRControlBaseDesignerActionList {
		XRGauge Gauge { get { return (XRGauge)Component; } }
		public float? TargetValue {
			get { return Gauge.TargetValue; }
			set { SetPropertyValue("TargetValue", value); }
		}
		public DesignBinding DataBinding {
			get { return ControlDesigner.GetDesignBinding("TargetValue"); }
			set { ControlDesigner.SetBinding("TargetValue", value); }
		}
		public XRGaugeDesignerActionList3(XRGaugeDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "TargetValue", "TargetValue");
			actionItems.Add(CreatePropertyItem("DataBinding", ReportStringId.STag_Name_DataBinding));
		}
	}
	public class XRGaugeDesignerActionList4 : XRControlBaseDesignerActionList {
		XRGauge Gauge { get { return (XRGauge)Component; } }
		public float? Minimum {
			get { return Gauge.Minimum; }
			set { SetPropertyValue("Minimum", value); }
		}
		public DesignBinding DataBinding {
			get { return ControlDesigner.GetDesignBinding("Minimum"); }
			set { ControlDesigner.SetBinding("Minimum", value); }
		}
		public XRGaugeDesignerActionList4(XRGaugeDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "Minimum", "Minimum");
			actionItems.Add(CreatePropertyItem("DataBinding", ReportStringId.STag_Name_DataBinding));
		}
	}
	public class XRGaugeDesignerActionList5 : XRControlBaseDesignerActionList {
		XRGauge Gauge { get { return (XRGauge)Component; } }
		public float? Maximum {
			get { return Gauge.Maximum; }
			set { SetPropertyValue("Maximum", value); }
		}
		public DesignBinding DataBinding {
			get { return ControlDesigner.GetDesignBinding("Maximum"); }
			set { ControlDesigner.SetBinding("Maximum", value); }
		}
		public XRGaugeDesignerActionList5(XRGaugeDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "Maximum", "Maximum");
			actionItems.Add(CreatePropertyItem("DataBinding", ReportStringId.STag_Name_DataBinding));
		}
	}
}
