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

using DevExpress.DocumentView;
using DevExpress.DocumentView.Controls;
using DevExpress.DocumentView.Controls.Thumbnails;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraPrinting.Control.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Preview;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraPrinting.Control {
	public class ThumbnailsController : DockPanelController {
		ThumbnailsViewer thumbnailsControl;
		protected override bool CanBeVisible {
			get {
				return printControl.Document != null && !printControl.DocumentIsCreating && !printControl.Document.IsEmpty ; 
			}
		}
		public ThumbnailsController(PrintControl printControl, DockPanel savedParent)
			: base(printControl, PrintingSystemCommand.Thumbnails, PreviewStringId.TB_TTip_Thumbnails, null) {
		}
		protected override void InitializeDockPanel(DockPanel dockPanel) {
			dockPanel.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
			dockPanel.ID = new System.Guid("5bf88ca5-827f-4710-9823-c0d4730e7fc3");
			dockPanel.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Left;
			dockPanel.SavedIndex = 2;
			dockPanel.Size = new System.Drawing.Size(200, 344);
			dockPanel.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
		}
		protected override void InitializeDockPanel_Container(ControlContainer dockPanel_Container) {
			base.InitializeDockPanel_Container(dockPanel_Container);
			thumbnailsControl = new ThumbnailsViewer();
			dockPanel_Container.Controls.Add(thumbnailsControl);
			thumbnailsControl.BorderStyle = BorderStyle.None;
			thumbnailsControl.Dock = System.Windows.Forms.DockStyle.Fill;
			thumbnailsControl.Name = "thumbnailsControl";
			thumbnailsControl.Size = new System.Drawing.Size(194, 321);
			thumbnailsControl.TabIndex = 1;
			thumbnailsControl.Visible = false;
			thumbnailsControl.Document = printControl.Document;
			thumbnailsControl.LookAndFeel.ParentLookAndFeel = printControl.LookAndFeel;
			thumbnailsControl.SelectedPageChanged += thumbnailsControl_SelectedPageChanged;
			printControl.SelectedPageChanged += printControl_SelectedPageChanged;
			thumbnailsControl.SizeChanged += thumbnailsControl_SizeChanged;
		}
		void thumbnailsControl_SizeChanged(object sender, EventArgs e) {
			thumbnailsControl.Zoom = thumbnailsControl.Zoom;
		}
		void printControl_SelectedPageChanged(object sender, EventArgs e) {
			if(CanBeVisible && ReferenceEquals(thumbnailsControl.Document, printControl.Document) && thumbnailsControl.Visible)
				thumbnailsControl.ShowPage(thumbnailsControl.Document.Pages[printControl.SelectedPageIndex]);
		}
		public void UpdateDocument() {
			if(!printControl.DocumentIsCreating)
				thumbnailsControl.Document = printControl.Document;
			UpdateVisibility();
		}
		public void InvalidateViewer() {
			thumbnailsControl.ViewControl.Invalidate();
		}
		protected override void UpdateVisibilityCore(DockVisibility dockVisibility) {
			base.UpdateVisibilityCore(dockVisibility);
			if(DockPanel.DockManager != null)
				thumbnailsControl.Visible = DockPanel.Visibility == DockVisibility.Visible;
		}
		void thumbnailsControl_SelectedPageChanged(object sender, EventArgs e) {
			printControl.ShowPage(printControl.Document.Pages[thumbnailsControl.SelectedPage.Index]);
		}
		public override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				if(thumbnailsControl != null) {
					thumbnailsControl.SelectedPageChanged -= new EventHandler(this.thumbnailsControl_SelectedPageChanged);
					thumbnailsControl.Dispose();
					thumbnailsControl = null;
				}
				if(printControl != null)
					printControl.SelectedPageChanged -= new EventHandler(this.printControl_SelectedPageChanged);
			}
		}
	}
}
