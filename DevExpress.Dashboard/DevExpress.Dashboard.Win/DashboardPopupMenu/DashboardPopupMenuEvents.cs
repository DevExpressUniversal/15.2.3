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
using System.Drawing;
using DevExpress.DashboardWin.Native;
using DevExpress.XtraBars;
namespace DevExpress.DashboardWin {
	public enum DashboardArea { DashboardTitle, DashboardItemCaption, DashboardItem }
	public enum DashboardItemArea { None, DashboardItem, GridColumnHeader, GridColumnTotal }
	public enum DashboardButtonType {
		None,
		DrillUp,
		ClearMasterFilter,
		ClearSelection,
		Values,
		Export,
		Parameters,
		MapInitialExtent
	}
	public class DashboardPopupMenuShowingEventArgs : DashboardItemMouseHitTestEventArgs {
		PopupMenu menu;
		bool allow = true;
		DashboardArea area = DashboardArea.DashboardItem;
		DashboardItemArea itemArea = DashboardItemArea.None;
		DashboardButtonType buttonType = DashboardButtonType.None;
		public PopupMenu Menu { get { return menu; } set { menu = value; } }
		public bool Allow { get { return allow; } set { allow = value; } }
		public DashboardArea DashboardArea { get { return area; } internal set { area = value; } }
		public DashboardItemArea DashboardItemArea { get { return itemArea; } internal set { itemArea = value; } }
		public DashboardButtonType ButtonType { get { return buttonType; } internal set { buttonType = value; } }
		internal DashboardPopupMenuShowingEventArgs(DashboardItemViewer dashboardItemViewer, Point location)
			: base(dashboardItemViewer, location) {
		}
	}
	public delegate void DashboardPopupMenuShowingEventHandler(object sender, DashboardPopupMenuShowingEventArgs e);
}
