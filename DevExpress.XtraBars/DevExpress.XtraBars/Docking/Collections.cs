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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking.Helpers;
namespace DevExpress.XtraBars.Docking {
	public class ChangedCollectionBase : CollectionBase {
		protected internal event CollectionChangeEventHandler Changed;
		int updateCounter;
		public ChangedCollectionBase() {
			this.Changed = null;
			this.updateCounter = 0;
		}
		protected bool IsLocked { get { return (updateCounter != 0); } }
		protected void BeginUpdate() { updateCounter++; }
		protected void EndUpdate() {
			CancelUpdate();
			if(updateCounter == 0)
				OnChanged(CollectionChangeAction.Refresh, null);
		}
		protected void CancelUpdate() { updateCounter--; }
		protected override void OnInsertComplete(int index, object value) {
			OnChanged(CollectionChangeAction.Add, value);
		}
		protected override void OnRemoveComplete(int index, object value) {
			OnChanged(CollectionChangeAction.Remove, value);
		}
		protected override void OnClearComplete() {
			OnChanged(CollectionChangeAction.Refresh, null);
		}
		protected virtual void OnChanged(CollectionChangeAction action, object element) {
			if(!IsLocked && Changed != null)
				Changed(this, new CollectionChangeEventArgs(action, element));
		}
	}
	public class DockPanelCollection : ChangedCollectionBase, IEnumerable<DockPanel> {
		internal DockPanelCollection() { }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DockPanelCollectionItem")]
#endif
		public DockPanel this[int index] { get { return (InnerList[index] as DockPanel); } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DockPanelCollectionItem")]
#endif
		public DockPanel this[string name] {
			get {
				for(int i = 0; i < Count; i++) {
					if(this[i].Name == name)
						return this[i];
				}
				return null;
			}
		}
		protected internal virtual void Update(DockPanel[] panels) {
			BeginUpdate();
			try {
				InnerList.Clear();
				AddRange(panels);
			}
			finally {
				EndUpdate();
			}
		}
		public void AddRange(DockPanel[] panels) {
			BeginUpdate();
			try {
				for(int i = 0; i < panels.Length; i++)
					List.Add(panels[i]);
			}
			finally {
				EndUpdate();
			}
		}
		public int IndexOf(DockPanel panel) {
			return InnerList.IndexOf(panel);
		}
		public bool Contains(DockPanel panel) {
			return (IndexOf(panel) != -1);
		}
		protected internal void Assign(XtraDeserializeInfoCollection collection) {
			InnerList.Clear();
			for(int i = collection.Count - 1; i > -1; i--)
				List.Add(collection[i].Panel);
		}
		protected override void OnRemoveComplete(int index, object value) {
			((DockPanel)value).Dispose();
			base.OnRemoveComplete(index, value);
		}
		protected override void OnClear() {
			BeginUpdate();
			try {
				for(int i = Count - 1; i > -1; i--)
					RemoveAt(i);
			}
			finally {
				CancelUpdate();
			}
		}
		protected internal void UpdateFloatFormsVisibility() {
			for(int i = 0; i < Count; i++) {
				this[i].UpdateFloatFormVisibility();
			}
		}
		#region IEnumerable<DockPanel> Members
		IEnumerator<DockPanel> IEnumerable<DockPanel>.GetEnumerator() {
			foreach(DockPanel panel in InnerList)
				yield return panel;
		}
		#endregion
	}
	public class ReadOnlyPanelCollection : ReadOnlyCollectionBase, IEnumerable<DockPanel> {
		protected internal int Add(DockPanel panel) { return InnerList.Add(panel); }
		protected internal void Remove(DockPanel panel) { InnerList.Remove(panel); }
		protected internal void Assign(ReadOnlyPanelCollection collection) {
			InnerList.Clear();
			for(int i = 0; i < collection.Count; i++)
				Add(collection[i]);
		}
		public int IndexOf(DockPanel panel) { return InnerList.IndexOf(panel); }
		public bool Contains(DockPanel panel) { return InnerList.Contains(panel); }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("ReadOnlyPanelCollectionItem")]
#endif
		public DockPanel this[int index] { get { return (DockPanel)InnerList[index]; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("ReadOnlyPanelCollectionItem")]
#endif
		public DockPanel this[string name] {
			get {
				for(int i = 0; i < Count; i++) {
					if(this[i].Name == name)
						return this[i];
				}
				return null;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("ReadOnlyPanelCollectionItem")]
#endif
		public DockPanel this[Guid id] {
			get {
				for(int i = 0; i < Count; i++) {
					if(this[i].ID.Equals(id))
						return this[i];
				}
				return null;
			}
		}
		#region IEnumerable<DockPanel> Members
		IEnumerator<DockPanel> IEnumerable<DockPanel>.GetEnumerator() {
			foreach(DockPanel panel in InnerList)
				yield return panel;
		}
		#endregion
	}
	public class AutoHideContainerCollection : CollectionBase {
		DockLayoutManager manager;
		internal AutoHideContainerCollection(DockLayoutManager manager) {
			this.manager = manager;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("AutoHideContainerCollectionItem")]
#endif
		public AutoHideContainer this[int index] { get { return (InnerList[index] as AutoHideContainer); } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("AutoHideContainerCollectionItem")]
#endif
		public AutoHideContainer this[DockingStyle dock] {
			get {
				for(int i = 0; i < Count; i++) {
					if(this[i].Dock == (DockStyle)dock)
						return this[i];
				}
				return null;
			}
		}
		public bool Contains(AutoHideContainer container) {
			return InnerList.Contains(container);
		}
		protected DockLayoutManager Manager { get { return manager; } }
		protected internal void Deserialize(DockLayoutManager layoutManager) {
			foreach(AutoHideContainer container in new ArrayList(this))
				container.OnDeserialize(layoutManager);
		}
		protected internal int Add(AutoHideContainer container) {
			return List.Add(container);
		}
		protected internal void Remove(AutoHideContainer container) {
			List.Remove(container);
		}
		public void AddRange(AutoHideContainer[] autoHideContainers) {
			for(int i = 0; i < autoHideContainers.Length; i++)
				Add(autoHideContainers[i]);
		}
		protected internal void SetRange(AutoHideContainer[] autoHideContainers) {
			InnerList.Clear();
			AddRange(autoHideContainers);
		}
		protected override void OnClear() {
			for(int i = Count - 1; i > -1; i--)
				this[i].Dispose();
		}
		protected internal void LayoutChanged() {
			if(Manager.DockManager.IsDeserializing) return;
			for(int i = 0; i < Count; i++) {
				AutoHideContainer container = this[i];
				container.ContainerSize = Manager.CalcAutoHidePanelSize(container);
				container.LayoutChanged();
			}
		}
		protected internal void BeginUpdate() {
			for(int i = 0; i < Count; i++)
				this[i].BeginUpdate();
		}
		protected internal void CancelUpdate() {
			for(int i = 0; i < Count; i++)
				this[i].CancelUpdate();
		}
	}
	public class DockManagerCollection : ReadOnlyCollectionBase {
		public DockManager this[int index] { get { return (DockManager)InnerList[index]; } }
		public int IndexOf(DockManager manager) { return InnerList.IndexOf(manager); }
		public bool Contains(DockManager manager) { return (IndexOf(manager) != -1); }
		internal void RegisterDockManager(DockManager manager) {
			lock(InnerList.SyncRoot) {
				if(Contains(manager)) return;
				InnerList.Add(manager);
			}
		}
		internal DockManager FromControl(Control control) {
			lock(InnerList.SyncRoot) {
				foreach(DockManager dockManager in InnerList) {
					if(dockManager.Form == control) return dockManager;
				}
				return null;
			}
		}
		internal void UnregisterDockManager(DockManager manager) {
			lock(InnerList.SyncRoot) {
				if(!Contains(manager)) return;
				InnerList.Remove(manager);
			}
		}
	}
}
namespace DevExpress.XtraBars.Docking.Helpers {
	public class ResizeZoneCollection : CollectionBase {
		public ResizeZone this[int index] { get { return (InnerList[index] as ResizeZone); } }
		public int Add(ResizeZone resizeZone) { return InnerList.Add(resizeZone); }
		public void Remove(ResizeZone resizeZone) { InnerList.Remove(resizeZone); }
	}
	public class DockZoneCollection : CollectionBase {
		public DockZone this[int index] { get { return (InnerList[index] as DockZone); } }
		public DockZone this[Point screenPoint] {
			get {
				for(int i = 0; i < Count; i++) {
					if(this[i].Contains(screenPoint))
						return this[i];
				}
				return null;
			}
		}
		public void Insert(int index, DockZone dockZone) {
			InnerList.Insert(index, dockZone);
		}
		public DockZone this[DockingStyle ds] {
			get {
				for(int i = Count - 1; i >= 0; i--) {
					if(this[i].DockSide == ds)
						return this[i];
				}
				return null;
			}
		}
		public DockZone this[Type type] {
			get {
				for(int i = 0; i < Count; i++) {
					if(this[i].GetType().IsSubclassOf(type))
						return this[i];
				}
				return null;
			}
		}
		protected internal int GetDockIndex(DockZone dockZone) {
			int index = 0;
			for(int i = 0; i < Count; i++) {
				DockZone item = this[i];
				if(item.DockSide != dockZone.DockSide) continue;
				if(item == dockZone) return index;
				index++;
			}
			return LayoutConsts.InvalidIndex;
		}
		public int Add(DockZone dockZone) { return InnerList.Add(dockZone); }
		public void Remove(DockZone dockZone) { InnerList.Remove(dockZone); }
	}
	public class XtraDeserializeInfoCollection : CollectionBase, IComparer {
		public XtraDeserializeInfo this[int index] { get { return (XtraDeserializeInfo)InnerList[index]; } }
		public int Add(XtraDeserializeInfo value) { return InnerList.Add(value); }
		public void Sort() {
			InnerList.Sort(this);
		}
		int IComparer.Compare(object x, object y) {
			XtraDeserializeInfo xInfo = (XtraDeserializeInfo)x;
			XtraDeserializeInfo yInfo = (XtraDeserializeInfo)y;
			return (xInfo.ZIndex - yInfo.ZIndex);
		}
	}
	public class XtraDeserializeChildInfoCollection : SortedList {
		public XtraDeserializeInfoCollection this[int parentID] {
			get { return (XtraDeserializeInfoCollection)base[parentID]; }
			set { base[parentID] = value; }
		}
		public void Add(XtraDeserializeInfo value) {
			XtraDeserializeInfoCollection item = this[value.ParentID];
			if(item == null) {
				item = new XtraDeserializeInfoCollection();
				this[value.ParentID] = item;
			}
			item.Add(value);
		}
		public int IndexOf(XtraDeserializeInfoCollection value) { return base.IndexOfValue(value); }
	}
	public class XtraDeserializeAutoHideInfoCollection : SortedList {
		public XtraDeserializeAutoHideInfoCollection() : base(new XtraAutoHideInfoComparer(), DockConsts.AutoHideContainerMaxCount) { }
		public XtraDeserializeInfoCollection this[DockingStyle dock] {
			get { return (XtraDeserializeInfoCollection)base[dock]; }
			set { base[dock] = value; }
		}
		public void Add(XtraDeserializeInfo value) {
			XtraDeserializeInfoCollection item = this[value.AutoHideDock];
			if(item == null) {
				item = new XtraDeserializeInfoCollection();
				this[value.AutoHideDock] = item;
			}
			item.Add(value);
		}
		class XtraAutoHideInfoComparer : IComparer {
			int IComparer.Compare(object x, object y) {
				DockingStyle xDock = (DockingStyle)x;
				DockingStyle yDock = (DockingStyle)y;
				return ((int)yDock - (int)xDock);
			}
		}
	}
}
