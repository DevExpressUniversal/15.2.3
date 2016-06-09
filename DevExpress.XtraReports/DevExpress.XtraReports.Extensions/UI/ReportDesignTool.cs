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
using DevExpress.XtraReports.Native;
using DevExpress.LookAndFeel;
using DevExpress.XtraReports.UserDesigner;
using System.Windows.Forms;
using System.ComponentModel.Design;
namespace DevExpress.XtraReports.UI {
	public interface IReportDesignTool : IDisposable {
		void ShowDesigner();
		void ShowDesignerDialog();
		void ShowDesigner(UserLookAndFeel lookAndFeel);
		void ShowDesignerDialog(UserLookAndFeel lookAndFeel);
		void ShowDesigner(UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels);
		void ShowDesignerDialog(UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels);
		void ShowRibbonDesigner();
		void ShowRibbonDesignerDialog();
		void ShowRibbonDesigner(UserLookAndFeel lookAndFeel);
		void ShowRibbonDesignerDialog(UserLookAndFeel lookAndFeel);
		void ShowRibbonDesigner(UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels);
		void ShowRibbonDesignerDialog(UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels);
	}
	public class ReportDesignTool : IReportDesignTool {
		XtraReport report;
		IDesignForm designForm;
		IDesignForm designRibbonForm;
		public XtraReport Report {
			get { return report; }
		}
		public IDesignForm DesignForm {
			get {
				if(designForm == null || designForm.IsDisposed) {
					designForm = CreateDesignForm();
				}
				return designForm;
			}
		}
		public IDesignForm DesignRibbonForm {
			get {
				if(designRibbonForm == null || designRibbonForm.IsDisposed) {
					designRibbonForm = CreateDesignRibbonForm();
				}
				return designRibbonForm;
			}
		}
		protected virtual IDesignForm CreateDesignForm() {
			return new XRDesignForm();
		}
		protected virtual IDesignForm CreateDesignRibbonForm() {
			return new XRDesignRibbonForm();
		}
		public ReportDesignTool(XtraReport report) {
			if(report == null)
				throw new ArgumentNullException();
			this.report = report;
		}
		public virtual void Dispose() {
			DisposeControl(designForm);
			DisposeControl(designRibbonForm);
			if(((IServiceProvider)report).GetService(typeof(IReportDesignTool)) == this) {
				IServiceContainer serv = ((IServiceProvider)report).GetService(typeof(IServiceContainer)) as IServiceContainer;
				serv.RemoveService(typeof(IReportDesignTool));
			}
		}
		static void DisposeControl(IDesignForm control) {
			if(control != null && !control.IsDisposed)
				control.Dispose();
		}
		public void ShowDesigner() {
			ShowDesigner(null);
		}
		public void ShowDesignerDialog() {
			ShowDesignerDialog(null);
		}
		public void ShowDesigner(UserLookAndFeel lookAndFeel) {
			ShowDesigner(lookAndFeel, 0);
		}
		public void ShowDesignerDialog(UserLookAndFeel lookAndFeel) {
			ShowDesignerDialog(lookAndFeel, 0);
		}
		public void ShowDesigner(UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels) {
			ShowDesigner(DesignForm, lookAndFeel, hiddenPanels);
		}
		public void ShowDesignerDialog(UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels) {
			ShowDesignerDialog(DesignForm, lookAndFeel, hiddenPanels);
		}
		public void ShowRibbonDesigner() {
			ShowRibbonDesigner(null);
		}
		public void ShowRibbonDesignerDialog() {
			ShowRibbonDesignerDialog(null);
		}
		public void ShowRibbonDesigner(UserLookAndFeel lookAndFeel) {
			ShowRibbonDesigner(lookAndFeel, 0);
		}
		public void ShowRibbonDesignerDialog(UserLookAndFeel lookAndFeel) {
			ShowRibbonDesignerDialog(lookAndFeel, 0);
		}
		public void ShowRibbonDesigner(UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels) {
			ShowDesigner(DesignRibbonForm, lookAndFeel, hiddenPanels);
		}
		public void ShowRibbonDesignerDialog(UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels) {
			ShowDesignerDialog(DesignRibbonForm, lookAndFeel, hiddenPanels);
		}
		void ShowDesigner(IDesignForm form, UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels) {
			form.SetWindowVisibility(hiddenPanels, false);
			form.OpenReport(report, lookAndFeel);
#if DEBUGTEST
			DevExpress.XtraReports.Tests.FormHelper.ShowForm((Form)form);
#else
			form.Show();
#endif
		}
		void ShowDesignerDialog(IDesignForm form, UserLookAndFeel lookAndFeel, DesignDockPanelType hiddenPanels) {
			try {
				form.SetWindowVisibility(hiddenPanels, false);
				form.OpenReport(report, lookAndFeel);
				form.ShowDialog();
			} finally {
				form.Dispose();
			}
		}
	}
}
