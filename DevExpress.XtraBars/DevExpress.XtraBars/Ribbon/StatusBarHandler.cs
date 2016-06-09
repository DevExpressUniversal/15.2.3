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
using System.Drawing;
using System.Threading;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Skins;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.ViewInfo;
namespace DevExpress.XtraBars.Ribbon.Handler {
	public class RibbonStatusBarHandler : BaseRibbonHandler {
		RibbonStatusBar statusBar;
		public RibbonStatusBarHandler(RibbonStatusBar statusBar) {
			this.statusBar = statusBar;
		}
		public override Control OwnerControl { get { return StatusBar; } }
		protected RibbonStatusBar StatusBar { get { return statusBar; } }
		protected override BaseRibbonViewInfo ViewInfo { get { return StatusBar.ViewInfo; } }
		protected override BarSelectionInfo SelectionInfo {
			get {
				if(StatusBar.Manager == null) return null;
				return StatusBar.Manager.SelectionInfo;
			}
		}
		protected override LinksNavigation CreateLinksNavigator() {
			if(StatusBar.Ribbon == null) return null;
			return new RibbonStatusBarNavigation(StatusBar.Ribbon); 
		}
		protected override bool IsDesignTime { get { return OwnerControl.Site != null && OwnerControl.Site.DesignMode; } }
		public override void OnMouseDown(DXMouseEventArgs e) {
			if(IsDesignTime) {
				if(ViewInfo.DesignTimeManager.ProcessMouseDown(e)) return;
			}
			base.OnMouseDown(e);
		}
		public override void OnMouseMove(DXMouseEventArgs e) {
			if(IsDesignTime) {
				if(ViewInfo.DesignTimeManager.ProcessMouseMove(e)) return;
			}
			base.OnMouseMove(e);
		}
	}
}
