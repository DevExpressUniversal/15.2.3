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
namespace DevExpress.XtraReports.Design {
	public class XRCrossBandControlDesignerActionList : XRControlBaseDesignerActionList {
		public VerticalAnchorStyles AnchorVertical {
			get { return ((XRCrossBandControl)Component).AnchorVertical; }
			set { SetPropertyValue("AnchorVertical", value); }
		}
		public XRCrossBandControlDesignerActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "AnchorVertical", "AnchorVertical");
		}
	}
	public class XRCrossBandLineDesignerActionList : XRCrossBandControlDesignerActionList {
		XRCrossBandLine CrossBandLine { get { return (XRCrossBandLine)Component; } }
		public Color ForeColor {
			get { return CrossBandLine.ForeColor; }
			set { SetPropertyValue("ForeColor", value); }
		}
		[TypeConverter(typeof(DevExpress.Utils.Design.DashStyleTypeConverter))]
		public DashStyle LineStyle {
			get { return CrossBandLine.LineStyle; }
			set { SetPropertyValue("LineStyle", value); }
		}
		public XRCrossBandLineDesignerActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "ForeColor", "ForeColor");
			AddPropertyItem(actionItems, "LineStyle", "LineStyle");
		}
	}
	public class XRCrossBandBoxDesignerActionList : XRCrossBandControlDesignerActionList {
		XRCrossBandBox CrossBandBox { get { return (XRCrossBandBox)Component; } }
		public Color BorderColor {
			get { return CrossBandBox.BorderColor; }
			set { SetPropertyValue("BorderColor", value); }
		}
		public float BorderWidth {
			get { return CrossBandBox.BorderWidth; }
			set { SetPropertyValue("BorderWidth", value); }
		}
		[
		Editor(typeof(DevExpress.XtraReports.Design.BordersEditor), typeof(System.Drawing.Design.UITypeEditor)),
		]
		public BorderSide Borders {
			get { return CrossBandBox.Borders; }
			set { SetPropertyValue("Borders", value); }
		}
		public XRCrossBandBoxDesignerActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "Borders", "Borders");
			AddPropertyItem(actionItems, "BorderColor", "BorderColor");
			AddPropertyItem(actionItems, "BorderWidth", "BorderWidth");
		}
	}
}
