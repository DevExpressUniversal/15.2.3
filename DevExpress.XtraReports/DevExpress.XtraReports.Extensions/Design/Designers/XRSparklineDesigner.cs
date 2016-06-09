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
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Sparkline;
namespace DevExpress.XtraReports.Design {
	public class XRSparklineDesigner : XRControlDesigner {
		public XRSparklineDesigner()
			: base() {
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRSparklineDesignerActionList1(this));
			list.Add(new XRSparklineDesignerActionList2(this));
		}
	}
	public class XRSparklineDesignerActionList1 : DataContainerDesignerActionList {
		XRSparkline Sparkline { get { return (XRSparkline)Component; } }
		[
		Editor("DevExpress.XtraReports.Design.DataContainerFieldNameEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor)),
		]
		public string ValueMember {
			get { return Sparkline.ValueMember; }
			set { SetPropertyValue("ValueMember", value); }
		}
		public XRSparklineDesignerActionList1(XRComponentDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "ValueMember", "ValueMember");
		}
	}
	public class XRSparklineDesignerActionList2 : XRComponentDesignerActionList {
		XRSparkline Sparkline { get { return (XRSparkline)Component; } }
		[
		Editor("DevExpress.XtraEditors.Design.SparklineViewEditor, " + AssemblyInfo.SRAssemblyEditors + AssemblyInfo.FullAssemblyVersionExtension, typeof(UITypeEditor)),
		]
		public SparklineViewBase View {
			get { return Sparkline.View; }
			set { SetPropertyValue("View", value); }
		}
		public XRSparklineDesignerActionList2(XRComponentDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "View", "View");
		}
	}
}
