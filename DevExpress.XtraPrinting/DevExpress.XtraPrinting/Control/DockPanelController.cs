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
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Collections;
using System.Drawing.Imaging;
using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Control.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.WinControls;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraPrinting.Control.Native {
	public abstract class DockPanelController : IDisposable {
		protected DevExpress.XtraBars.Docking.ControlContainer dockPanel_Container;
		DevExpress.XtraBars.Docking.DockPanel dockPanel;
		protected bool fPanelVisible;
		protected PrintControl printControl;
		PrintingSystemCommand printingSystemCommand;
		protected DockManager DockManager { get { return printControl.DockManager; } }
		public DevExpress.XtraBars.Docking.DockPanel DockPanel { get { return dockPanel; } }
		public bool PanelVisible {
			get { return CanBeVisible && fPanelVisible; }
			set {
				fPanelVisible = value;
				UpdateVisibility();
			}
		}
		protected abstract bool CanBeVisible { get; }
		public DockPanelController(PrintControl printControl, PrintingSystemCommand printingSystemCommand, PreviewStringId stringId, DockPanel savedParent) {
			this.printControl = printControl;
			this.printingSystemCommand = printingSystemCommand;
			#region initialize
			dockPanel = new DevExpress.XtraBars.Docking.DockPanel();
			dockPanel_Container = new DevExpress.XtraBars.Docking.ControlContainer();
			printControl.SuspendLayout();
			dockPanel.SuspendLayout();
			dockPanel_Container.SuspendLayout();
			dockPanel.Controls.Add(this.dockPanel_Container);
			InitializeDockPanel(dockPanel);
			dockPanel.SavedParent = savedParent;
			dockPanel.ClosedPanel += new DevExpress.XtraBars.Docking.DockPanelEventHandler(this.dockPanel_ClosedPanel);
			InitializeDockPanel_Container(dockPanel_Container);
			printControl.ResumeLayout(false);
			dockPanel.ResumeLayout(false);
			dockPanel_Container.ResumeLayout(false);
			#endregion
			printControl.DockManagerCreated += new EventHandler(OnDockManagerCreated);
			dockPanel.Text = PreviewLocalizer.GetString(stringId);
		}
		protected void SetImage(System.Drawing.Image image) {
			DevExpress.Utils.Images images = ((DevExpress.Utils.ImageCollection)DockManager.Images).Images;
			images.Add(image);
			DockPanel.ImageIndex = images.Count - 1;
		}
		protected abstract void InitializeDockPanel(DockPanel dockPanel);
		protected virtual void InitializeDockPanel_Container(ControlContainer dockPanel_Container) {
			dockPanel_Container.Location = new System.Drawing.Point(3, 20);
			dockPanel_Container.Name = "dockPanel1_Container";
			dockPanel_Container.Size = new System.Drawing.Size(194, 321);
			dockPanel_Container.TabIndex = 0;
		}
		#region IDisposable implementation
		public virtual void Dispose(bool disposing) {
			if(disposing) {
				if(dockPanel != null) {
					dockPanel.ClosedPanel -= new DevExpress.XtraBars.Docking.DockPanelEventHandler(this.dockPanel_ClosedPanel);
					dockPanel = null;
				}
				if(printControl != null) {
					printControl.DockManagerCreated -= new EventHandler(OnDockManagerCreated);
					printControl = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~DockPanelController() {
			Dispose(false);
		}
		#endregion
		protected internal void UpdateVisibility() {
			if(!printControl.DesignMode) {
				UpdateVisibilityCore(PanelVisible ? DockVisibility.Visible : DockVisibility.Hidden);
			}
		}
		protected virtual void UpdateVisibilityCore(DockVisibility dockVisibility) {
			if(dockPanel.DockManager != null && dockVisibility != dockPanel.Visibility) {
				dockPanel.Visibility = dockVisibility;
			}
		}
		protected virtual void OnDockManagerCreated(object sender, EventArgs e) {
			DockManager.HiddenPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] { this.dockPanel });
			printControl.DockManager.Load += new EventHandler(DockManager_Load);
		}
		void DockManager_Load(object sender, EventArgs e) {
			printControl.DockManager.Load -= new EventHandler(DockManager_Load);
			UpdateVisibility();
		}
		void dockPanel_ClosedPanel(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e) {
			printControl.ExecCommand(printingSystemCommand, new object[] { false });
		}
	}
}
