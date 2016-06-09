#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using DevExpress.XtraGrid;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraLayout;
using DevExpress.XtraNavBar;
using DevExpress.XtraTab;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.EasyTest.Framework;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DevExpress.Utils.Menu;
using DevExpress.XtraScheduler;
using DevExpress.XtraBars.Ribbon;
using DevExpress.ExpressApp.EasyTest.WinAdapter.Utils;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter {
	public class BarManagerFindStrategy : Singleton<BarManagerFindStrategy> {
		private void FindControls(Control parent, List<object> controls) {
			if(parent != null) {
				object compatibleContol = GetCompatibleContol(parent);
				if(compatibleContol != null && controls.IndexOf(compatibleContol) == -1) {
					controls.Add(compatibleContol);
				}
				else {
					foreach(Control childControl in parent.Controls) {
						FindControls(childControl, controls);
					}
				}
			}
		}
		private object GetCompatibleContol(Control control) {
			if(control is BarDockControl) {
				BarManager manager = (control as BarDockControl).Manager;
				if(manager != null && manager.Form != null && manager.Form.Visible) {
					return manager;
				}
			}
			else {
				if(control is RibbonControl) {
					BarManager manager = (control as RibbonControl).Manager;
					if(manager != null && manager.Form != null && manager.Form.Visible) {
						return manager;
					}
				}
			}
			return null;
		}
		private List<object> GetCompatibleControls(Control control) {
			List<object> controls = new List<object>();
			FindControls(control, controls);
			return controls;
		}
		public IList<BarManager> FindBarManagers(Control control) {
			List<object> objs = GetCompatibleControls(control);
			List<BarManager> result = new List<BarManager>();
			foreach(BarManager barManager in objs) {
				if(result.IndexOf(barManager) == -1) {
					if(barManager.Form == control) {
						result.Insert(0, barManager);
					}
					else {
						result.Add(barManager);
					}
				}
			}
			return result;
		}
		public BarManager FindPopupMenuBarManager(Control control) {
			EditorContainer container = control as EditorContainer;
			if(container != null) {
				return container.MenuManager as BarManager;
			}
			else {
				return null;
			}
		}
	}
}
