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
using System.Windows.Automation;
using System.Windows.Controls.Primitives;
namespace DevExpress.Xpf.Grid.Automation {
	public abstract class GridDataControlAutomationPeer : DataControlAutomationPeer {
		HeaderPanelAutomationPeerBase headerPanelPeer;
		DataPanelAutomationPeer dataPanelPeer;
		FooterPanelAutomationPeer footerPanelPeer;
		bool isDataPanelPeerChildrenCacheDirty;
		protected abstract HeaderPanelAutomationPeerBase CreateHeaderPeer();
		public GridDataControlAutomationPeer(DataControlBase dataControl) : base(dataControl) { }
		public HeaderPanelAutomationPeerBase HeaderPanelPeer { get { return this.headerPanelPeer; } }
		public DataPanelAutomationPeer DataPanelPeer {
			get {
				if(this.isDataPanelPeerChildrenCacheDirty && this.dataPanelPeer != null) {
					this.dataPanelPeer.ResetChildrenCachePlatformIndependent();
					this.isDataPanelPeerChildrenCacheDirty = false;
				}
				return this.dataPanelPeer;
			}
		}
		public FooterPanelAutomationPeer FooterPanelPeer { get { return this.footerPanelPeer; } }
		protected internal override void ResetDataPanelPeer() {
			this.isDataPanelPeerChildrenCacheDirty = true;
		}
		protected internal override void ResetPeers() {
			this.dataPanelPeer = null;
			this.headerPanelPeer = null;
			this.footerPanelPeer = null;
		}
		protected internal override void ResetHeadersChildrenCache() {
#if !SL
			if(HeaderPanelPeer != null) {
				List<AutomationPeer> headersPeers = HeaderPanelPeer.GetChildren();
				foreach(AutomationPeer header in headersPeers)
					header.ResetChildrenCache();
			}
#endif
		}
		protected internal override void ResetDataPanelPeerCache() {
			if(!isDataPanelPeerChildrenCacheDirty) return;
#if !SL
			ResetChildrenCache();
			if(DataPanelPeer == null) return;
			DataPanelPeer.ResetChildrenCachePlatformIndependent();
#endif
			this.isDataPanelPeerChildrenCacheDirty = false;
		}
		protected internal override void ResetDataPanelChildrenForce() {
			if(DataPanelPeer == null) return;
			DataPanelPeer.ResetChildrenCachePlatformIndependent();
		}
		protected virtual DataPanelAutomationPeer CreateDataPanelPeer() {
			UIElement dataPanelUIElement = FindObjectInVisualTreeByType(DataControl, typeof(DataPresenter)) as UIElement;
#if !SL
			if(DataControl.DataView is CardView)
				dataPanelUIElement = FindObjectInVisualTreeByType(DataControl, typeof(CardDataPresenter)) as UIElement;
#endif
			if(dataPanelUIElement == null) return null;
			DataPanelAutomationPeer peer = (DataPanelAutomationPeer)FrameworkElementAutomationPeer.CreatePeerForElement(dataPanelUIElement);
			return peer;
		}
		protected virtual FooterPanelAutomationPeer CreateFooterPeer() {
			FrameworkElement footersPanel = GridControlAutomationPeerHelper.GetFooterPanelUIElement(DataControl);
			return footersPanel != null ? new FooterPanelAutomationPeer(DataControl, footersPanel) : null;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			if(DataControl.DataView.ShowColumnHeaders) {
				if(this.headerPanelPeer == null)
					this.headerPanelPeer = CreateHeaderPeer();
				children.Add(HeaderPanelPeer);
			}
			this.dataPanelPeer = CreateDataPanelPeer();
			if(DataPanelPeer != null) children.Add(DataPanelPeer);
			if(DataControl.DataView.ShowTotalSummary) {
				if(this.footerPanelPeer == null)
					this.footerPanelPeer = CreateFooterPeer();
				if(footerPanelPeer != null)
					children.Add(FooterPanelPeer);
			} else
				this.footerPanelPeer = null;
			return children;
		}
		protected internal override AutomationPeer GetCellPeer(int rowHandle, ColumnBase column, bool force = false) {
			GridRowAutomationPeer peer = GetRowPeer(rowHandle) as GridRowAutomationPeer;
			if(peer == null) return null;
			return peer.GetCellPeer(column.VisibleIndex, force);
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Grid)
				return this;
			if(patternInterface == PatternInterface.Scroll && DataControl.DataView.DataPresenter != null) {
				FrameworkElement scrollViewer = ((IScrollInfo)DataControl.DataView.DataPresenter).ScrollOwner;
				if(scrollViewer != null) {
					return FrameworkElementAutomationPeer.CreatePeerForElement(scrollViewer) as IScrollProvider;
				}
			}
			return base.GetPattern(patternInterface);
		}
	}
	public class GridControlAutomationPeer : GridDataControlAutomationPeer, IGridProvider {
		public GridControlAutomationPeer(GridControl gridControl)
			: base(gridControl) {
			gridControl.AutomationPeer = this;
		}
		public GridControl GridControl { get { return DataControl as GridControl; } }
		public new HeaderPanelAutomationPeer HeaderPanelPeer { get { return (HeaderPanelAutomationPeer)base.HeaderPanelPeer; } }
		protected override HeaderPanelAutomationPeerBase CreateHeaderPeer() {
			return new HeaderPanelAutomationPeer(GridControl);
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.DataGrid;
		}
		protected internal override AutomationPeer CreateRowPeer(int rowHandle) {
			if(GridControl.IsGroupRowHandle(rowHandle))
				return new GridGroupRowAutomationPeer(rowHandle, GridControl);
			if(GridControl.View is TreeListView)
				return new TreeListRowAutomationPeer(rowHandle, GridControl);
#if !SL
			if(GridControl.View is TableView)
				return new GridRowAutomationPeer(rowHandle, GridControl);
			return new CardRowAutomationPeer(rowHandle, GridControl);
#else
			return new GridRowAutomationPeer(rowHandle, GridControl);
#endif
		}
		protected internal override AutomationPeer GetGroupFooterAutomationPeer(int rowHandle) {
			AutomationPeer peer = null;
			if(!GridControl.LogicalPeerCache.GroupFooterRows.TryGetValue(rowHandle, out peer)) {
				peer = CreateGroupFooterAutomationPeer(rowHandle);
				GridControl.LogicalPeerCache.GroupFooterRows[rowHandle] = peer;
			}
			return peer;
		}
		protected virtual AutomationPeer CreateGroupFooterAutomationPeer(int rowHandle) {
			return new GroupFooterAutomationPeer(rowHandle, GridControl);  
		}
		protected override string ControlName { get { return "GridControl"; } }
	}
}
