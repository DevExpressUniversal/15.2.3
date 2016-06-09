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

using System.Windows.Automation.Provider;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Grid.Automation {
	public class GroupFooterAutomationPeer : GridControlVirtualElementAutomationPeerBase {
		Dictionary<int, GroupFooterSummaryAutomationPeer> summaryPeers;
		public GroupFooterAutomationPeer(int rowHandle, GridControl dataControl)
			: base(dataControl) {
			RowHandle = rowHandle;
			summaryPeers = new Dictionary<int, GroupFooterSummaryAutomationPeer>();
		}
		protected int RowHandle { get; set; }
		protected TableView TableView { get { return DataControl.viewCore as TableView; } }
		protected GridControl GridControl { get { return DataControl as GridControl; } }
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> list = new List<AutomationPeer>();
			for(int i = 0; i < TableView.VisibleColumnsCore.Count; i++) {
				if(HasSummaries(TableView.VisibleColumnsCore[i]))
					list.Add(GetGroupFooterSummaryPeer(i));
			}
			return list;
		}
		protected virtual bool HasSummaries(ColumnBase column) {
			return column.GroupSummariesCore.Count > 0;
		}
		protected AutomationPeer GetGroupFooterSummaryPeer(int columnIndex) {
			GroupFooterSummaryAutomationPeer peer = null;
			if(!summaryPeers.TryGetValue(columnIndex, out peer)) {
				peer = new GroupFooterSummaryAutomationPeer(GridControl, RowHandle, columnIndex);
				summaryPeers[columnIndex] = peer;
			}
			return peer;
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Pane;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			return null;
		}
		protected override string GetNameCore() {
			return "GroupFooterRow";
		}
		protected override FrameworkElement GetFrameworkElement() {
			return TableView.GetGroupFooterRowElementByRowHandle(RowHandle);
		}
#if SL
		protected override string GetLocalizedControlTypeCore() {
			return string.Empty;
		}
#endif
	}
	public class GroupFooterSummaryAutomationPeer : GridControlVirtualElementAutomationPeerBase {
		public GroupFooterSummaryAutomationPeer(GridControl dataControl, int rowHandle, int columnIndex)
			: base(dataControl) {
			RowHandle = rowHandle;
			ColumnIndex = columnIndex;
		}
		public int RowHandle { get; private set; }
		public int ColumnIndex { get; private set; }
		protected TableView TableView { get { return DataControl.viewCore as TableView; } }
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Custom;
		}
		protected override FrameworkElement GetFrameworkElement() {
			return TableView.GetGroupFooterSummaryElementByRowHandleAndColumn(RowHandle, TableView.VisibleColumnsCore[ColumnIndex]);
		}
		protected override string GetNameCore() {
			return TableView.GetGroupSummaryText(TableView.VisibleColumnsCore[ColumnIndex], RowHandle, false);
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			return new List<AutomationPeer>();
		}
		public override object GetPattern(PatternInterface patternInterface) {
			return null;
		}
#if SL
		protected override string GetLocalizedControlTypeCore() {
			return string.Empty;
		}
#endif
	}
}
