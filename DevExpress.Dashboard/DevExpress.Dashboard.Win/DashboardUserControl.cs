#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class DashboardUserControl : XtraUserControl {
		BarAndDockingController controller;
		internal bool IsVSDesignMode { get { return DevExpress.DashboardCommon.Native.Helper.IsComponentVSDesignMode(this); } }
		protected virtual BarAndDockingController BarAndDockingController { get { return null; } }
		protected virtual RibbonControl Ribbon { get { return null; } }
		protected virtual BarManager BarManager { get { return null; } }
		protected virtual DockManager DockManager { get { return null; } }
		protected virtual IEnumerable<object> Children { get { return null; } }
		BarAndDockingController Controller {
			get {
				if(controller == null) {
					controller = new BarAndDockingController();
					controller.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				}
				return controller;
			}
		}
		public DashboardUserControl() {
			InitializeComponent();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(BarAndDockingController != null)
				BarAndDockingController.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			BarManager barManager = BarManager;
			if(barManager != null)
				if(barManager.Controller != null)
					barManager.Controller.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				else
					barManager.Controller = Controller;
			RibbonControl ribbon = Ribbon;
			if(ribbon != null)
				if(ribbon.Controller != null)
					ribbon.Controller.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				else
					ribbon.Controller = Controller;
			DockManager dockManager = DockManager;
			if(dockManager != null)
				if(dockManager.Controller != null)
					dockManager.Controller.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				else
					dockManager.Controller = Controller;
			if(Children != null)
				foreach(object child in Children)
					DashboardWinHelper.SetParentLookAndFeel(child, LookAndFeel);
		}
	}
}
