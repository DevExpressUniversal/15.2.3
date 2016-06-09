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
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Localization;
using System.Drawing;
namespace DevExpress.XtraReports.UserDesigner {
	[
	ToolboxItem(false),
	]
	public class XRDesignReportExplorer : ReportTreeView, IDesignControl {
		XRDesignTreeListItemHelper treeListItemHelper;
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignReportExplorerXRDesignPanel"),
#endif
		DefaultValue(null),
		]
		public XRDesignPanel XRDesignPanel {
			get { return treeListItemHelper.XRDesignPanel; }
			set { treeListItemHelper.XRDesignPanel = value;}
		}
		protected override DevExpress.XtraReports.UI.XtraReport XtraReport {
			get { return treeListItemHelper.XtraReport; }
		}
		public XRDesignReportExplorer(): base(null) {
			this.treeListItemHelper = new XRDesignTreeListItemHelper(ReportToolItemKindHelper.ReportExplorer, this);
		}
		protected override void Dispose(bool disposing) {
			if(treeListItemHelper != null) {
				treeListItemHelper.DoDispose(disposing);
				treeListItemHelper = null;
			}
			base.Dispose(disposing);
		}
	}
}
