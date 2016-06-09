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
using DevExpress.XtraEditors;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using DevExpress.XtraBars.Docking2010.Views;
using System.ComponentModel;
namespace DevExpress.XtraReports.UserDesigner.Native {
	public interface IXRDesignPanelContainer : DevExpress.LookAndFeel.ISupportLookAndFeel, IDisposable {
		XRDesignPanel DesignPanel { get; }
		XtraReport Report { get; }
		string ReportDisplayName { get; }
		bool IsDisposed { get; }
		void SynchText();
		void ShowActive();
	}
	public class XRDesignPanelForm : XtraForm, IXRDesignPanelContainer {
		readonly XRDesignPanel designPanel;
		public XRDesignPanel DesignPanel {
			get { return designPanel; }
		}
		public XtraReport Report {
			get { return designPanel.Report; }
		}
		public virtual string ReportDisplayName {
			get {
				return Report != null ? Report.EffectiveDisplayName : string.Empty;
			}
		}
		public XRDesignPanelForm(IServiceProvider serviceProvider) {
			designPanel = new DevExpress.XtraReports.UserDesigner.XRDesignPanel(serviceProvider);
			designPanel.ReportStateChanged += new ReportStateEventHandler(designPanel_ReportStateChanged);
			designPanel.Dock = DockStyle.Fill;
			Controls.Add(designPanel);
			ShowIcon = false;
			ImeMode = ImeMode.Inherit;
		}
		void designPanel_ReportStateChanged(object sender, ReportStateEventArgs e) {
			SynchText();
		}
		public void SynchText() {
			Text = Report != null ?
				XRDesignPanel.ApplyReportState(ReportDisplayName, designPanel.ReportState) :
				string.Empty;
		}
		protected override bool GetAllowMdiFormSkins() {
			return true;
		}
		public void ShowActive() { Show(); }
	}
	[ToolboxItem(false)]
	public class XRDesignPanelContainerControl : XtraUserControl, IXRDesignPanelContainer {
		internal static IXRDesignPanelContainer Create(IServiceProvider serviceProvider, XRTabbedMdiManager documentManager) {
			XRDesignPanelContainerControl control = new XRDesignPanelContainerControl(serviceProvider);
			BaseDocument document = documentManager.View.AddDocument(control);
			control.SetDocument(document);
			return control;
		}
		readonly XRDesignPanel designPanel;
		BaseDocument document;
		public XRDesignPanel DesignPanel {
			get { return designPanel; }
		}
		public XtraReport Report {
			get { return designPanel.Report; }
		}
		public virtual string ReportDisplayName {
			get {
				return Report != null ? Report.EffectiveDisplayName : string.Empty;
			}
		}
		void SetDocument(BaseDocument document) {
			this.document = document;
		}
		public XRDesignPanelContainerControl(IServiceProvider serviceProvider) {
			designPanel = new DevExpress.XtraReports.UserDesigner.XRDesignPanel(serviceProvider);
			designPanel.ReportStateChanged += new ReportStateEventHandler(designPanel_ReportStateChanged);
			designPanel.Dock = DockStyle.Fill;
			Controls.Add(designPanel);
			ImeMode = ImeMode.Inherit;
		}
		void designPanel_ReportStateChanged(object sender, ReportStateEventArgs e) {
			SynchText();
		}
		public void SynchText() {
			document.Caption = Text = Report != null ?
				XRDesignPanel.ApplyReportState(ReportDisplayName, designPanel.ReportState) :
				string.Empty;
		}
		public void ShowActive() { Focus(); }
	}
}
