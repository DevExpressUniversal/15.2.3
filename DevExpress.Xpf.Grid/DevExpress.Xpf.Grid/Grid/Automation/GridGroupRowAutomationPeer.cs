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
using System.Windows.Automation;
using System.Windows.Controls;
using System.Collections.Generic;
namespace DevExpress.Xpf.Grid.Automation {
	public class GridGroupRowAutomationPeer : GridRowAutomationPeer, IExpandCollapseProvider {
		public GridGroupRowAutomationPeer(int rowHandle, GridControl grid)
			: base(rowHandle, grid) {
		}
		protected new GridControl DataControl { get { return base.DataControl as GridControl; } }
		#region IExpandCollapseProvider Members
		void IExpandCollapseProvider.Collapse() {
			DataControl.CollapseGroupRow(RowHandle);
		}
		void IExpandCollapseProvider.Expand() {
			DataControl.ExpandGroupRow(RowHandle);
		}
		ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState {
			get { return DataControl.IsGroupRowExpanded(RowHandle) ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed; }
		}
		#endregion
		protected override void Invoke() {
			DataControl.ChangeGroupExpanded(DataControl.GetRowVisibleIndexByHandle(RowHandle));
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.ExpandCollapse)
				return this;
			return base.GetPattern(patternInterface);
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			if(UIAutomationPeer != null)
				return UIAutomationPeer.GetChildren();
			return new List<AutomationPeer>();
		}
		protected internal override GridCellAutomationPeer GetCellPeer(int columnIndex, bool force = false) {
			if(!force) return null;
			return base.GetCellPeer(columnIndex, force);
		}
		protected override object GetBoundObject() {
			return DataControl.DataView.GetGroupDisplayValue(RowHandle);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Group;
		}
		protected override bool HasKeyboardFocusCore() {
			return DataControl.DataView.FocusedRowHandle == RowHandle;
		}
	}
}
