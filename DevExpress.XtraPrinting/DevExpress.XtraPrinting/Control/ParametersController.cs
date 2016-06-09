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
	public interface IContentBestSize {
		Size BestSize { get; }
	}
	public class ParametersController : DockPanelController {
		bool hasParameters;
		public bool HasParameters {
			get { return hasParameters && this.dockPanel_Container.Controls.Count > 0; }
			set { hasParameters = value; }
		}
		protected override bool CanBeVisible {
			get { return HasParameters; }
		}
		IContentBestSize PanelContent {
			get {
				if(this.dockPanel_Container.Controls.Count > 0)
					return dockPanel_Container.Controls[0] as IContentBestSize;
				return null;
			}
		}
		public ParametersController(PrintControl printControl, DockPanel savedParent)
			: base(printControl, PrintingSystemCommand.Parameters, PreviewStringId.TB_TTip_Parameters, savedParent) {
		}
		protected override void InitializeDockPanel(DockPanel dockPanel) {
			dockPanel.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
			dockPanel.ID = new Guid("999ECEBD-3DB7-45bd-B8F8-15967DFDF7EF");
			dockPanel.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
			dockPanel.SavedIndex = 1;
			dockPanel.Size = new System.Drawing.Size(200, 344);
			dockPanel.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
		}
		protected override void OnDockManagerCreated(object sender, EventArgs e) {
			Image image = ResourceImageHelper.CreateImageFromResources(typeof(ResFinder).Namespace + ".Images.Parameters.png", typeof(ResFinder).Assembly);
			SetImage(image);
			base.OnDockManagerCreated(sender, e);
		}
	}
}
