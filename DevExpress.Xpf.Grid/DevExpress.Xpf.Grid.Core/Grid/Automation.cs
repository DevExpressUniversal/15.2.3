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
using System.Windows.Automation;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System;
namespace DevExpress.Xpf.Grid.Automation {
	public interface IAutomationPeerCreator {
		AutomationPeer CreatePeer(DependencyObject obj);
	}
	public abstract class PeerCacheBase {
		Dictionary<object, AutomationPeer> peers;
		public PeerCacheBase() {  }
		protected Dictionary<object, AutomationPeer> Peers {
			get {
				if(peers == null)
					peers = new Dictionary<object, AutomationPeer>();
				return peers;
			}
		}
		public AutomationPeer GetPeer(DependencyObject obj) {
			if(obj == null) return null;
			AutomationPeer peer = null;
			Peers.TryGetValue(obj, out peer);
			return peer;
		}
		public void AddPeer(DependencyObject obj, AutomationPeer peer, bool checkShouldAdd) {
			if(!checkShouldAdd || ShouldAddPeerToCache(peer))
				Peers.Add(obj, peer);
		}
		protected abstract bool ShouldAddPeerToCache(AutomationPeer peer);
	}
	public class LogicalPeerCache {
		public LogicalPeerCache() {
			DataRows = new Dictionary<int, AutomationPeer>();
			GroupFooterRows = new Dictionary<int, AutomationPeer>();
		}
		public Dictionary<int, AutomationPeer> DataRows { get; private set; }
		public Dictionary<int, AutomationPeer> GroupFooterRows { get; private set; }
		public void Clear() {
			DataRows.Clear();
			GroupFooterRows.Clear();
		}
	}
	public abstract class DataControlAutomationPeerBase : FrameworkElementAutomationPeer, IAutomationPeerCreator {
#if DEBUGTEST
		public DataControlBase DataControlInternal { get { return this.dataControl; } set { this.dataControl = value; } }
#endif
		DataControlBase dataControl;
		protected DataControlAutomationPeerBase(DataControlBase dataControl, FrameworkElement element)
			: base(element) {
			this.dataControl = dataControl;
		}
		public DataControlBase DataControl { get { return dataControl; } }
		public virtual AutomationPeer CreatePeerCore(DependencyObject obj) {
			return CreatePeerDefault(obj);
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			return null;
		}
		#region IAutomationPeerCreator Members
		AutomationPeer IAutomationPeerCreator.CreatePeer(DependencyObject obj) {
			AutomationPeer peer = DataControl.PeerCache.GetPeer(obj);
			if(peer != null) return peer;
			peer = CreatePeerCore(obj);
			DataControl.PeerCache.AddPeer(obj, peer, true);
			return peer;
		}
		#endregion
		protected List<AutomationPeer> GetUIChildrenCore(DependencyObject obj) {
			return DataControlAutomationPeerBase.GetUIChildrenCore(obj, this);
		}
		protected void GetUIChildrenCore(DependencyObject obj, ref List<AutomationPeer> children) {
			DataControlAutomationPeerBase.GetUIChildrenCore(obj, this, ref children);
		}
		#region static 
		public static List<AutomationPeer> GetUIChildrenCore(DependencyObject obj, IAutomationPeerCreator owner) {
			List<AutomationPeer> children = null;
			GetUIChildrenCore(obj, owner, ref children);
			return children;
		}
		public static void GetUIChildrenCore(DependencyObject obj, IAutomationPeerCreator owner, ref List<AutomationPeer> children) {
			if(obj == null) return;
			AutomationPeer peer = null;
			for(int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++) {
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				peer = null;
				if(child != null) {
					peer = owner.CreatePeer(child);
					if(peer != null) {
						if(children == null)
							children = new List<AutomationPeer>();
						children.Add(peer);
					}
				}
				if(peer == null)
					GetUIChildrenCore(child, owner, ref children);
			}
			return;
		}
		public static AutomationPeer CreatePeerDefault(DependencyObject obj) {
#if !SL
			if(obj is UIElement3D)
				return UIElement3DAutomationPeer.CreatePeerForElement(obj as UIElement3D);
			else if(obj is UIElement)
				return UIElementAutomationPeer.CreatePeerForElement(obj as UIElement);
#else
			if(obj is UIElement)
				return FrameworkElementAutomationPeer.CreatePeerForElement(obj as UIElement);
#endif
			return null;
		}
		public static DependencyObject FindObjectInVisualTree(DependencyObject root, string objectName) {
			VisualTreeEnumerator en = new VisualTreeEnumerator(root);
			while(en.MoveNext()) {
				if(en.Current is FrameworkElement && (en.Current as FrameworkElement).Name == objectName) return en.Current;
			}
			return null;
		}
		public static DependencyObject FindObjectInVisualTreeByType(DependencyObject root, Type objectType) {
			VisualTreeEnumerator en = new VisualTreeEnumerator(root);
			while(en.MoveNext()) {
				if((en.Current is FrameworkElement) && (objectType.IsAssignableFrom(en.Current.GetType())))
					return en.Current;
			}
			return null;
		}
		public static AutomationPeer FindParentAutomationPeerByType(AutomationPeer root, Type type) {
			AutomationPeer node = root;
			while(node != null && node.GetType() != type) {
				node = node.GetParent();
			}
			return node;
		}
		#endregion 
	}
	public abstract class DataControlAutomationPeer : DataControlAutomationPeerBase, IGridProvider
#if !SL
, IRawElementProviderSimple
#endif
	{
		public DataControlAutomationPeer(DataControlBase dataControl) : base(dataControl, dataControl) {  }
		protected internal List<AutomationPeer> GetRowPeers() {
			List<AutomationPeer> list = new List<AutomationPeer>();
			for(int i = DataControl.DataView.PageVisibleTopRowIndex; i < DataControl.DataView.PageVisibleTopRowIndex + DataControl.DataView.PageVisibleDataRowCount; i++) {
				object visibleIndexCore = DataControl.DataProviderBase.GetVisibleIndexByScrollIndex(i);
				if(visibleIndexCore is GroupSummaryRowKey) {
					list.Add(GetGroupFooterAutomationPeer(((GroupSummaryRowKey)visibleIndexCore).RowHandle.Value));
					continue;
				}
				list.Add(GetRowPeer(DataControl.GetRowHandleByVisibleIndexCore((int)visibleIndexCore)));
			}
			return list;
		}
		protected internal AutomationPeer GetRowPeer(int rowHandle) {
			AutomationPeer peer = null;
			if(!DataControl.LogicalPeerCache.DataRows.TryGetValue(rowHandle, out peer)) {
				peer = CreateRowPeer(rowHandle);
				DataControl.LogicalPeerCache.DataRows[rowHandle] = peer;
			}
			return peer;
		}
		protected internal void ClearLogicalPeerCache() {
			LogicalPeerCache logicalPeerCache = DataControl.logicalPeerCache;
			if(logicalPeerCache != null)
				logicalPeerCache.Clear();
		}
		protected internal virtual void ResetDataPanelChildrenForce() { }
		protected internal virtual void ResetDataPanelPeerCache() { }
		protected internal abstract AutomationPeer CreateRowPeer(int rowHandle);
		protected internal abstract AutomationPeer GetGroupFooterAutomationPeer(int rowHandle);
		protected internal abstract AutomationPeer GetCellPeer(int rowHandle, ColumnBase column, bool force = false);
		protected internal abstract void ResetDataPanelPeer();
		protected internal abstract void ResetHeadersChildrenCache();
		protected internal abstract void ResetPeers();
		#region IGridProvider Members
		int IGridProvider.ColumnCount {
			get { return DataControl.ColumnsCore.Count; }
		}
		IRawElementProviderSimple IGridProvider.GetItem(int row, int column) {
			if(column >= DataControl.viewCore.VisibleColumnsCore.Count) return null;
			int rowHandle = DataControl.GetRowHandleByVisibleIndexCore(row);
			DataControl.DataView.ScrollIntoView(rowHandle);
			DataControl.UpdateLayout();
			ResetDataPanelChildrenForce();
			AutomationPeer cellPeer = GetCellPeer(rowHandle, DataControl.viewCore.VisibleColumnsCore[column], true);
			return base.ProviderFromPeer(cellPeer);
		}
		int IGridProvider.RowCount {
			get { return DataControl.VisibleRowCount; }
		}
		#endregion
#if !SL
		#region IRawElementProviderSimple Members
		object IRawElementProviderSimple.GetPatternProvider(int patternId) {
			if(patternId == GridPatternIdentifiers.ColumnCountProperty.Id)
				return ((IGridProvider)this).ColumnCount;
			else if(patternId == GridPatternIdentifiers.RowCountProperty.Id)
				return ((IGridProvider)this).RowCount;
			return null;
		}
		object IRawElementProviderSimple.GetPropertyValue(int propertyId) {
			if(propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return "";
			else if(propertyId == AutomationElementIdentifiers.ClassNameProperty.Id)
				return ControlName;
			return null;
		}
		IRawElementProviderSimple IRawElementProviderSimple.HostRawElementProvider {
			get { return null; }
		}
		ProviderOptions IRawElementProviderSimple.ProviderOptions {
			get { return ProviderOptions.ClientSideProvider; }
		}
		#endregion
#endif
		protected abstract string ControlName { get; }
	}
	static class AutomationPeerExtensions {
		public static void ResetChildrenCachePlatformIndependent(this AutomationPeer peer) {
#if !SL
			peer.ResetChildrenCache();
#endif
		}
	}
}
