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
using System.ComponentModel;
using DevExpress.XtraBars;
using DevExpress.XtraDashboardLayout;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
namespace DevExpress.DashboardWin.Bars {
	public abstract class HistoryBarItem : DashboardBarButtonItem {
		HistoryPopupContainer container;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override BarButtonStyle ButtonStyle { 
			get { return BarButtonStyle.DropDown; } 
			set {} 
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override PopupControl DropDownControl { 
			get { return container; } 
			set {} 
		}		
		protected abstract bool RedoOperations { get; } 
		protected HistoryBarItem() {
		}
		void OnCloseContainer() {
			if (container.Commit) {
				HistoryCommandExecutor.ExecuteHistoryCommandSet(Control, RedoOperations, container.OperationIndex);
			}
		}
		protected override void Initialize() {
			base.Initialize();
			container = new HistoryPopupContainer(this, RedoOperations);
			container.CloseUp += (sender, e) => OnCloseContainer();
		}
		protected override void OnControlUpdateUI(object sender, EventArgs e) {
			base.OnControlUpdateUI(sender, e);
			UpdateSuperTipAndShortCut();
		} 
	}
}
