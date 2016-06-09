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
using System.Windows.Forms;
using DevExpress.Data.Utils;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraReports.Design.Tools;
namespace DevExpress.XtraReports.UserDesigner.Native {
	public class ReportToolItem : IToolItem {
#if DEBUGTEST
		public Control Test_Control {
			get { return control; }
		}
		public void Test_SetController(TreeListController controller) {
			this.controller = controller;
			Controller.CaptureTreeList(control);
		}
#endif
		Control control;
		TreeListController controller;
		Guid kind;
		IServiceProvider serviceProvider;
		TreeListController Controller {
			get {
				if(controller == null)
					this.controller = ((ISupportController)control).CreateController(serviceProvider);
				return controller;
			}
		}
		public ReportToolItem(IServiceProvider serviceProvider, Guid kind, Control control) {
			this.serviceProvider = serviceProvider;
			this.kind = kind;
			this.control = control;
		}
		public Guid Kind {
			get { return kind; }
		}
		public void InitTool() {
			Controller.CaptureTreeList(control);
		}
		public void UpdateView() {
			if(Controller == null)
				return;
			Controller.CaptureTreeList(control);
			Controller.UpdateTreeList();
		}
		public void Hide() {
			Controller.ClearTreeList();
		}
		public virtual void Close() {
		}
		public void ShowNoActivate() {
		}
		public void ShowActivate() {
			DockPanel dockPanel = GetDockPanel(control);
			if(dockPanel != null && !dockPanel.IsDisposed)
				dockPanel.Show();
		}
		static DockPanel GetDockPanel(Control control) {
			Control parent = control.Parent;
			for(; ; ) {
				if(parent == null || parent is DockPanel)
					return (DockPanel)parent;
				parent = parent.Parent;
			}
		}
		public virtual void Dispose() {
			if (controller != null) 
				controller.Dispose();
		}
	}
}
