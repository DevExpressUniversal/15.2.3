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
using System.Windows.Controls;
using System.Collections.Generic;
namespace DevExpress.Xpf.Grid.Automation {
	public class HeaderPanelScrollViewerAutomationPeerBase : ScrollViewerAutomationPeer, IAutomationPeerCreator {
		DataControlBase dataControl;
		public HeaderPanelScrollViewerAutomationPeerBase(DataControlBase dataControl, ScrollViewer viewer) : base(viewer) {
			this.dataControl = dataControl;
		}
		protected DataControlBase DataControl { get { return dataControl; } }
		#region IAutomationPeerCreator Members
		protected virtual AutomationPeer CreatePeerCore(DependencyObject obj) {
			if(obj is GridColumnHeader) return CreateColumnHeaderAutomationPeer((GridColumnHeader)obj);
			return DataControlAutomationPeerBase.CreatePeerDefault(obj);
		}
		protected virtual ColumnHeaderAutomationPeerBase CreateColumnHeaderAutomationPeer(GridColumnHeader header) {
			return new ColumnHeaderAutomationPeerBase(DataControl, header);
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			return DataControlAutomationPeerBase.GetUIChildrenCore(Owner, this);
		}
		AutomationPeer IAutomationPeerCreator.CreatePeer(DependencyObject obj) {
			AutomationPeer peer = DataControl.PeerCache.GetPeer(obj);
			if(peer != null) return peer;
			peer = CreatePeerCore(obj);
			DataControl.PeerCache.AddPeer(obj, peer, true);
			return peer;
		}
		#endregion
	}
	public class HeaderPanelScrollViewerAutomationPeer : HeaderPanelScrollViewerAutomationPeerBase {
		public HeaderPanelScrollViewerAutomationPeer(GridControl gridControl, ScrollViewer viewer)
			: base(gridControl, viewer) {
		}
		protected new GridControl DataControl { get { return base.DataControl as GridControl; } }
		protected override ColumnHeaderAutomationPeerBase CreateColumnHeaderAutomationPeer(GridColumnHeader header) {
			return new ColumnHeaderAutomationPeer(DataControl, header);
		}
	}
}
