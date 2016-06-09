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

using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using System.Drawing;
using System.ComponentModel.Design;
using DevExpress.XtraReports.Localization;
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting;
using DevExpress.XtraPivotGrid;
namespace DevExpress.XtraReports.Design {
	public class XRPivotGridDesignerActionList1 : DataContainerDesignerActionList {
		XRPivotGrid PivotGrid {
			get { return (XRPivotGrid)Component; }
		}
		[
		Editor(typeof(DevExpress.XtraPivotGrid.Design.OLAPConnectionEditor), typeof(System.Drawing.Design.UITypeEditor))
		]
		public string OLAPConnectionString {
			get { return PivotGrid.OLAPConnectionString; }
			set { SetPropertyValue("OLAPConnectionString", value); }
		}
		public XRPivotGridDesignerActionList1(XRComponentDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "OLAPConnectionString", "OLAPConnectionString");
		}
	}
	public class XRPivotGridDesignerActionList2 : XRControlBaseDesignerActionList {
		PivotArea area = PivotArea.RowArea;
		public PivotArea Area {
			get { return area; }
			set { area = value; }
		}
		public XRPivotGridDesignerActionList2(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			actionItems.Add(CreatePropertyItem("Area", ReportStringId.STag_Name_FieldArea));
			DesignerActionItem actionItem = new DesignerActionMethodItem(this, "OnAddFieldToArea", DesignSR.Verb_AddFieldToArea, string.Empty, string.Empty, false);
			actionItems.Add(actionItem);
			actionItem = new DesignerActionMethodItem(this, "OnRunDesigner", DesignSR.Verb_RunDesigner, string.Empty, string.Empty, true);
			actionItems.Add(actionItem);
		}
		protected void OnRunDesigner() {
			((XRPivotGridDesigner)designer).RunDesigner();
		}
		protected void OnAddFieldToArea() {
			((XRPivotGridDesigner)designer).AddFieldToArea(this.Area);
		}
	}
}
