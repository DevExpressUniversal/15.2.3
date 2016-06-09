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
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native.LayoutView;
namespace DevExpress.XtraReports.UI {
	[
	ListBindable(BindableSupport.No),
	TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter)),
	]
	public class XRControlCollectionBase : CollectionBase, IDisposable {
		#region Events
		CollectionChangeEventHandler onCollectionChanged;
		public event CollectionChangeEventHandler CollectionChanged {
			add { onCollectionChanged = System.Delegate.Combine(onCollectionChanged, value) as CollectionChangeEventHandler; }
			remove { onCollectionChanged = System.Delegate.Remove(onCollectionChanged, value) as CollectionChangeEventHandler; }
		}
		protected void OnCollectionChanged(CollectionChangeEventArgs e) {
			if(onCollectionChanged != null) onCollectionChanged(this, e);
		}
		EventHandler onBeforeClear;
		internal event EventHandler BeforeClear {
			add { onBeforeClear = System.Delegate.Combine(onBeforeClear, value) as EventHandler; }
			remove { onBeforeClear = System.Delegate.Remove(onBeforeClear, value) as EventHandler; }
		}
		void OnBeforeClear(EventArgs e) {
			if(onBeforeClear != null) onBeforeClear(this, e);
		}
		#endregion
		protected XRControl owner;
		protected ISite Site {
			get { return owner.Site; }
		}
		protected bool DesignMode {
			get { return Site != null && Site.DesignMode; }
		}
		public XRControlCollectionBase(XRControl owner) {
			this.owner = owner;
		}
		protected internal virtual bool CanAdd(Type controlType) {
			return true;
		}
		protected override void OnInsert(int index, object value) {
			if(value is XRTableOfContents && !(owner is BandPanel) && !CanPlacedXRToc((XRControl)owner))
				throw new ArgumentException(ReportLocalizer.GetString(ReportStringId.Msg_PlacingXrTocIntoIncorrectContainer));
			if(value is XRTableOfContents && (owner is BandPanel) && !CanPlacedXRToc(((BandPanel)owner).RealControl))
				throw new ArgumentException(ReportLocalizer.GetString(ReportStringId.Msg_PlacingXrTocIntoIncorrectContainer));
			base.OnInsert(index, value);
		}
		bool CanPlacedXRToc(XRControl owner) {
			return (owner is ReportHeaderBand) || (owner is ReportFooterBand);
		}
		protected override void OnInsertComplete(int index, object value) {
			OnInsertCompleteCore(index, value);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
		}
		protected virtual void OnInsertCompleteCore(int index, object value) {
			XRControl control = value as XRControl;
			if(control.ControlContainer != null)
				((IList)control.ControlContainer).Remove(control);
			control.AssignParent(owner, true);
		}
		protected override void OnClear() {
			if(Count != 0) {
				OnBeforeClear(EventArgs.Empty);
				OnClearCore();
			}
		}
		protected virtual void OnClearCore() {
			foreach(XRControl item in this) {
				if(ReferenceEquals(owner, item.Parent))
					item.AssignParent(null, false);
			}
		}
		protected override void OnClearComplete() {
			OnClearCompleteCore();
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected virtual void OnClearCompleteCore() {
		}
		protected override void OnRemoveComplete(int index, object value) {
			OnRemoveCompleteCore(index, value);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
		}
		protected virtual void OnRemoveCompleteCore(int index, object value) {
			((XRControl)value).AssignParent(null, false);
		}
		protected void AddToContainer(Component comp) {
			if(DesignMode && comp.Site == null) {
				if(comp is XRControl)
					DesignTool.AddToContainer(Site, comp, ((XRControl)comp).Name);
				else
					DesignTool.AddToContainer(Site, comp);
			}
		}
		protected void RemoveFromContainer(Component component) {
			if(DesignMode && component.Site != null) {
				DesignTool.RemoveFromContainer(Site, component);
			}
		}
		internal void SetChildIndexInternal(XRControl child, int newIndex) {
			if(InnerList.Contains(child)) {
				InnerList.Remove(child);
				InnerList.Insert(newIndex, child);
			}
		}
		public virtual void Dispose() {
			for(int i = 0; i < Count; i++) {
				XRControl control = InnerList[i] as XRControl;
				control.AssignParent(null, false);
				control.Dispose();
			}
			owner = null;
		}
	}
	public class XRControlCollection : XRControlCollectionBase {
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRControlCollectionItem")]
#endif
		public XRControl this[int index] {
			get { return (XRControl)List[index]; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRControlCollectionItem")]
#endif
		public XRControl this[string name] {
			get {
				for(int i = 0; i < Count; i++) {
					if(this[i] != null && this[i].Name == name)
						return this[i];
				}
				return null;
			}
		}
		public XRControlCollection(XRControl owner) : base(owner) {
		}
		protected override void OnInsert(int index, object value) {
			base.OnInsert(index, value);
			if(value is XRCrossBandControl)
				throw new ArgumentException("value");
		}
		public void AddRange(XRControl[] controls) {
			foreach(XRControl item in controls)
				Add(item);
		}
		public int Add(XRControl child) {
			int index = IndexOf(child);
			return index < 0 ? List.Add(child) : index;
		}
		internal void UpdateLayout() {
			for(int i = 0; i < Count; i++)
				this[i].UpdateLayout();
		}
		internal void CopyFrom(XRControl[] controls) {
			Clear();
			AddRange(controls);
		}
		public int IndexOf(XRControl item) {
			return List.IndexOf(item);
		}
		public bool Contains(XRControl item) {
			return List.Contains(item);
		}
		public void Remove(XRControl item) {
			List.Remove(item);
		}
		public void SetChildIndex(XRControl child, int newIndex) {
			SetChildIndexInternal(child, newIndex);
		}
	}
	[ListBindable(BindableSupport.No)]
	public class XRComponentCollection : Collection<IComponent> {
		public void AddRange(IComponent[] components) {
			foreach(IComponent component in components)
				Add(component);
		}
		protected override void InsertItem(int index, IComponent item) {
			if(Contains(item))
				return;
			base.InsertItem(index, item);
		}
	}
}
