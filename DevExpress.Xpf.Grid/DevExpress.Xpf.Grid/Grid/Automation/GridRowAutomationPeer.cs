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
namespace DevExpress.Xpf.Grid.Automation {
	public class GridRowAutomationPeer : GridControlVirtualElementAutomationPeerBase, IInvokeProvider, IScrollItemProvider, ISelectionItemProvider {
		Dictionary<int, GridCellAutomationPeer> cellPeers;
		protected int RowHandle { get; set; }
		public GridRowAutomationPeer(int rowHandle, DataControlBase dataControl)
			: base(dataControl) {
			RowHandle = rowHandle;
			cellPeers = new Dictionary<int, GridCellAutomationPeer>();
		}
		protected override FrameworkElement GetFrameworkElement() {
			return DataControl.viewCore.GetRowElementByRowHandle(RowHandle);
		}
		protected override bool HasKeyboardFocusCore() {
			return DataControl.DataView.NavigationStyle == GridViewNavigationStyle.Row && DataControl.DataView.FocusedRowHandle == RowHandle;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> list = new List<AutomationPeer>();
			for(int i = 0; i < DataControl.viewCore.VisibleColumnsCore.Count; i++) {
				list.Add(GetCellPeer(i));
			}
			return list;
		}
		protected virtual internal GridCellAutomationPeer GetCellPeer(int columnIndex, bool force = false) {
			GridCellAutomationPeer peer = null;
			if(!cellPeers.TryGetValue(columnIndex, out peer)) {
				peer = new GridCellAutomationPeer(RowHandle, columnIndex, DataControl);
				cellPeers[columnIndex] = peer;
			}
			return peer;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Invoke)
				return this;
			if(patternInterface == PatternInterface.ScrollItem)
				return this;
			if(patternInterface == PatternInterface.SelectionItem)
				return this;
			return null;
		}
		protected override string GetNameCore() {
			if(DataControl == null) return string.Empty;
			object value = GetBoundObject();
			return value != null ? value.ToString() : string.Empty;
		}
		protected virtual object GetBoundObject() {
			return DataControl.GetRow(RowHandle);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.DataItem;
		}
#if SL
		protected override string GetLocalizedControlTypeCore() {
			return string.Empty;
		}
#endif
		#region IInvokeProvider Members
		void IInvokeProvider.Invoke() {
			Invoke();
		}
		#endregion
		protected virtual void Invoke() {
			DataControl.viewCore.SetFocusedRowHandle(RowHandle);
		}
		#region IScrollItemProvider Members
		void IScrollItemProvider.ScrollIntoView() {
			DataControl.viewCore.SetFocusedRowHandle(RowHandle);
		}
		#endregion
		#region ISelectionItemProvider Members
		void ISelectionItemProvider.AddToSelection() {
			DataControl.DataView.SelectRowCore(RowHandle);
		}
		bool ISelectionItemProvider.IsSelected {
			get { return DataControl.DataView.IsRowSelected(RowHandle); }
		}
		void ISelectionItemProvider.RemoveFromSelection() {
			DataControl.DataView.UnselectRowCore(RowHandle);
		}
		void ISelectionItemProvider.Select() {
			DataControl.DataView.SetFocusedRowHandle(RowHandle);
			if(DataControl.SelectionMode == MultiSelectMode.Row) {
				DataControl.BeginSelection();
				DataControl.UnselectAll();
				DataControl.DataView.SelectRowCore(RowHandle);
				DataControl.EndSelection();
			} else if(DataControl.SelectionMode == MultiSelectMode.MultipleRow) {
				DataControl.DataView.SelectRowCore(RowHandle);
			}
		}
		IRawElementProviderSimple ISelectionItemProvider.SelectionContainer {
			get { return base.ProviderFromPeer(DataControl.AutomationPeer); }
		}
		#endregion
	}
}
