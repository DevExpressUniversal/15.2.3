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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;
using DevExpress.Xpf.Grid.TreeList;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.Native;
using System.Windows.Data;
using DevExpress.Xpf.Grid.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Automation;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Xpf.Grid {
	public class TreeListRowAutomationPeer : GridRowAutomationPeer, IExpandCollapseProvider {
		public TreeListRowAutomationPeer(int rowHandle, DataControlBase dataControl) : base(rowHandle, dataControl) {
		}
		protected TreeListView View { get { return DataControl.viewCore as TreeListView; } }
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.TreeItem;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.ExpandCollapse)
				return this;
			return base.GetPattern(patternInterface);
		}
		#region IExpandCollapseProvider Members
		void IExpandCollapseProvider.Collapse() {
			View.CollapseNode(RowHandle);
		}
		void IExpandCollapseProvider.Expand() {
			View.ExpandNode(RowHandle);
		}
		ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState {
			get {
				TreeListNode node = View.GetNodeByRowHandle(RowHandle);
				if(!node.HasVisibleChildren)
					return ExpandCollapseState.LeafNode;
				return node.IsExpanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
			}
		}
		#endregion
	}
	public class TreeListControlAutomationPeer : GridDataControlAutomationPeer {
		public TreeListControlAutomationPeer(TreeListControl treeListControl)
			: base(treeListControl) {
			treeListControl.AutomationPeer = this;
		}
		public TreeListControl TreeListControl { get { return DataControl as TreeListControl; } }
		protected override HeaderPanelAutomationPeerBase CreateHeaderPeer() {
			return new HeaderPanelAutomationPeerBase(TreeListControl);
		}
		protected internal override AutomationPeer CreateRowPeer(int rowHandle) {
			return new TreeListRowAutomationPeer(rowHandle, TreeListControl);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.Tree;
		}
		protected override string ControlName { get { return "TreeListControl"; } }
		protected internal override AutomationPeer GetGroupFooterAutomationPeer(int rowHandle) {
			throw new NotImplementedException();
		}
	}
}
