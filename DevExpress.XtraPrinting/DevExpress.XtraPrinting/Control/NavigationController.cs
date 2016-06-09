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
using System.Drawing;
using System.Windows.Forms;
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
using DevExpress.XtraTreeList;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.Control.Native {
	public class NavigationController : DockPanelController {
		DevExpress.XtraPrinting.Native.WinControls.BookmarkTreeView bmTreeView;
		protected override bool CanBeVisible { 
			get { return printControl.DocumentHasBookmarks; }
		}
		#region tests
		#if DEBUGTEST
		public BookmarkTreeView BookmarkTreeView { get { return bmTreeView; } }
		#endif
		#endregion
		PrintControl PrintControl { get { return (PrintControl)printControl; } }
		public NavigationController(PrintControl printControl)
			: base(printControl, PrintingSystemCommand.DocumentMap, PreviewStringId.TB_TTip_Map, null) {
			fPanelVisible = true;
		}
		protected override void InitializeDockPanel(DockPanel dockPanel) {
			dockPanel.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
			dockPanel.ID = new System.Guid("6b2e64eb-afd0-4676-bc3d-eca7e99946aa");
			dockPanel.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Left;
			dockPanel.SavedIndex = 0;
			dockPanel.Size = new System.Drawing.Size(200, 344);
			dockPanel.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
		}
		protected override void InitializeDockPanel_Container(ControlContainer dockPanel_Container) {
			base.InitializeDockPanel_Container(dockPanel_Container);
			bmTreeView = new DevExpress.XtraPrinting.Native.WinControls.BookmarkTreeView();
			dockPanel_Container.Controls.Add(this.bmTreeView);
			bmTreeView.LookAndFeel.ParentLookAndFeel = printControl.LookAndFeel;
			bmTreeView.BackColor = System.Drawing.Color.White;
			bmTreeView.BorderStyle = BorderStyles.NoBorder;
			bmTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			bmTreeView.OptionsSelection.EnableAppearanceFocusedCell = false;
			bmTreeView.Name = "bmTreeView";
			bmTreeView.Size = new System.Drawing.Size(194, 321);
			bmTreeView.TabIndex = 1;
			bmTreeView.Visible = false;
			bmTreeView.AfterFocusNode += new NodeEventHandler(this.bmTreeView_AfterSelect);
		}
		public override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				if(bmTreeView != null) {
					bmTreeView.AfterFocusNode -= new NodeEventHandler(this.bmTreeView_AfterSelect);
					bmTreeView = null;
				}
			}
		}
		public void UpdateDocumentMap() {
			if(printControl.DocumentHasBookmarks && !printControl.DocumentIsCreating) {
				bmTreeView.FillNodes(PrintControl.PrintingSystem.Document);
			} else {
				bmTreeView.Nodes.Clear();
			}
			UpdateVisibility();
		}
		protected override void OnDockManagerCreated(object sender, EventArgs e) {
			System.Windows.Forms.ImageList imageList = ResourceImageHelper.CreateImageListFromResources(typeof(ResFinder).Namespace + ".Images.PrintPreviewBar.png", typeof(ResFinder).Assembly, new System.Drawing.Size(16, 16));
			SetImage(imageList.Images[19]);
			base.OnDockManagerCreated(sender, e);
		}
		protected override void UpdateVisibilityCore(DockVisibility dockVisibility) {
			base.UpdateVisibilityCore(dockVisibility);
			if(DockPanel.DockManager != null)
				bmTreeView.Visible = DockPanel.Visibility == DockVisibility.Visible;
		}
		void bmTreeView_AfterSelect(object sender, NodeEventArgs e) {
			BrickPageNode node = e.Node as BrickPageNode;
			if(node != null)
				node.ShowAssociatedBrick(PrintControl);
		}
	}
}
