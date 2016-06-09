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
using System.Text;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UserDesigner.Native;
using System.Windows.Forms;
namespace DevExpress.XtraReports.UserDesigner.Native {
	public class XRDesignTreeListItemHelper {
		XRDesignPanel xrDesignPanel;
		Guid itemKind;
		protected Control control;
		public XRDesignPanel XRDesignPanel {
			get { return xrDesignPanel; }
			set {
				if(xrDesignPanel != null)
					UnSubscribeDesignPanelEvents();
				xrDesignPanel = value;
				if(xrDesignPanel != null)
					SubscribeDesignPanelEvents();
			}
		}
		public DevExpress.XtraReports.UI.XtraReport XtraReport {
			get { return xrDesignPanel != null ? xrDesignPanel.Report : null; }
		}
		public XRDesignTreeListItemHelper(Guid itemKind, Control control) {
			this.itemKind = itemKind;
			this.control = control;
		}
		public void DoDispose(bool disposing) {
			if(disposing) {
				if(xrDesignPanel != null)
					UnSubscribeDesignPanelEvents();
			}
		}
		protected virtual void SubscribeDesignPanelEvents() {
			xrDesignPanel.DesignerHostLoading += new EventHandler(OnDesignerHostLoading);
		}
		protected virtual void UnSubscribeDesignPanelEvents() {
			xrDesignPanel.DesignerHostLoading -= new EventHandler(OnDesignerHostLoading);
		}
		void OnDesignerHostLoading(object sender, EventArgs e) {
			xrDesignPanel.ReportToolShell.AddToolItem(new ReportToolItem(this.xrDesignPanel, itemKind, control));
		}
	} 
}
