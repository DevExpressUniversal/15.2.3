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
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Ribbon;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Menu;
using System.ComponentModel;
namespace DevExpress.DashboardWin.Native {
	public partial class DashboardForm : XtraForm, IDXMenuManagerProvider {
		BarManager barManager;
		protected virtual BarAndDockingController Controller { get { return null;} }
		protected virtual RibbonControl Ribbon { get { return null; } }
		protected virtual GalleryControl GalleryControl { get { return null; } }
		protected virtual BarManager BarManager { get { return null; } }
		protected virtual DockManager DockManager { get { return null; } }
		public DashboardForm() {
			InitializeComponent();
			barManager = new BarManager(Container ?? new Container());
			barManager.Form = this;
			BarAndDockingController barController = new BarAndDockingController();
			barController.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			barManager.Controller = barController;
		}
		public DashboardForm(UserLookAndFeel lookAndFeel) : this() {
			DashboardWinHelper.SetParentLookAndFeel(this, lookAndFeel);
		}
		protected virtual void DisposeInternal(bool disposing) { 
		}
		IDXMenuManager IDXMenuManagerProvider.MenuManager {
			get {
				if(BarManager != null)
					return BarManager;
				if(Ribbon != null)
					return Ribbon;
				return barManager;
			}
		}
	}
}
