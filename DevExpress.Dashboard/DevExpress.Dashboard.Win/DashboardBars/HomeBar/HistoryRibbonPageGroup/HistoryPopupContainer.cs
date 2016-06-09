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
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.DashboardWin.Bars;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class HistoryPopupContainer : PopupControlContainer {
		readonly HistoryBarItem barItem;
		readonly bool redoOperations;
		readonly HistoryPopupControl popupControl;
		public bool Commit { get { return popupControl.Commit; } }
		public int OperationIndex { get { return popupControl.OperationIndex; } }
		public HistoryPopupContainer(HistoryBarItem barItem, bool redoOperations) {
			this.barItem = barItem;
			this.redoOperations = redoOperations;
			popupControl = new HistoryPopupControl();
			popupControl.Dock = DockStyle.Fill;
			Controls.Add(popupControl);
		}
		protected override void OnPopup() {
			base.OnPopup();
			ClientSize = popupControl.OnPopup(barItem, redoOperations);
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			popupControl.Focus();
		}
	}
}
