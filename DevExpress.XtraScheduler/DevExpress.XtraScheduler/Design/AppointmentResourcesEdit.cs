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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Accessibility;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraScheduler.UI.Accessibility;
namespace DevExpress.XtraScheduler.UI {
	#region AppointmentResourcesEditResourcesCheckedListBoxControl
	[DXToolboxItem(false)]
	public class AppointmentResourcesEditResourcesCheckedListBoxControl : ResourcesCheckedListBoxControl {
		protected internal override ResourceFilterController CreateResourceFilterController() {
			return new AppointmentResourcesEditResourceFilterController(this);
		}
	}
	#endregion
	#region AppointmentResourcesEdit
	[
	DXToolboxItem(DXToolboxItemKind.Regular),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "AppointmentResourcesEdit.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuitePopupContainerEditDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A pop-up checked list box control used to select multiple resources in appointment editing dialogs.")
	]
	public class AppointmentResourcesEdit : ResourcesPopupCheckedListBoxControl {
		#region Fields
		NotificationCollectionChangedListener<object> listener;
		AppointmentResourceIdCollection resourceIds;
		#endregion
		public AppointmentResourcesEdit() {
			this.resourceIds = new AppointmentResourceIdCollection();
			this.listener = new NotificationCollectionChangedListener<object>(resourceIds);
			SubscribeResourceIdsEvents();
			AppointmentResourcesEditResourceFilterController controller = (AppointmentResourcesEditResourceFilterController)Controller;
			controller.ResourceIds = this.resourceIds;
			AppointmentResourcesEditResourcesCheckedListBoxControl checkedListControl = (AppointmentResourcesEditResourcesCheckedListBoxControl)ResourcesCheckedListBoxControl;
			controller = (AppointmentResourcesEditResourceFilterController)checkedListControl.Controller;
			controller.ResourceIds = this.resourceIds;
			controller.BeginSetResourceVisibility += new EventHandler(OnBeginSetResourceVisibility);
			controller.EndSetResourceVisibility += new EventHandler(OnEndSetResourceVisibility);
		}
		#region Properties
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentResourcesEditResourceIds")]
#endif
		public AppointmentResourceIdCollection ResourceIds { 
			get {
				if (Site != null && Site.DesignMode) {
					return new AppointmentResourceIdCollection(false);
				}
				return resourceIds; 
			}
			set {
				ResourceIds.BeginUpdate();
				try {
					ResourceIds.Clear();
					if (value == null)
						return;
					ResourceIds.AddRange(value);
				} finally {
					ResourceIds.EndUpdate();
				}
			}
		}
		internal NotificationCollectionChangedListener<object> ResourceIdsListener { get { return listener; } }
		#endregion
		#region Events
		#region ResourceIdsChanged
		static object onResourceIdsChanged = new object();
		public event EventHandler ResourceIdsChanged {
			add { Events.AddHandler(onResourceIdsChanged, value); }
			remove { Events.RemoveHandler(onResourceIdsChanged, value); }
		}
		void RaiseResourceIdsChanged() {
			EventHandler handler = Events[onResourceIdsChanged] as EventHandler;
			if (handler == null)
				return;
			handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (ResourcesCheckedListBoxControl != null) {
						AppointmentResourcesEditResourcesCheckedListBoxControl checkedListControl = (AppointmentResourcesEditResourcesCheckedListBoxControl)ResourcesCheckedListBoxControl;
						AppointmentResourcesEditResourceFilterController controller = (AppointmentResourcesEditResourceFilterController)checkedListControl.Controller;
						controller.BeginSetResourceVisibility -= new EventHandler(OnBeginSetResourceVisibility);
						controller.EndSetResourceVisibility -= new EventHandler(OnEndSetResourceVisibility);
					}
					if (listener != null) {
						UnsubscribeResourceIdsEvents();
						listener.Dispose();
						listener = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected internal virtual void SubscribeResourceIdsEvents() {
			ResourceIdsListener.Changed += new EventHandler(OnResourceIdsChanged);
		}
		protected internal virtual void UnsubscribeResourceIdsEvents() {
			ResourceIdsListener.Changed -= new EventHandler(OnResourceIdsChanged);
		}
		void OnBeginSetResourceVisibility(object sender, EventArgs e) {
			UnsubscribeResourceIdsEvents();
		}
		void OnEndSetResourceVisibility(object sender, EventArgs e) {
			SubscribeResourceIdsEvents();
			ResetResourcesItemsCore(Controller.AvailableResources);
			RaiseResourceIdsChanged();
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new AppointmentResourcesEditAccessible(Properties);
		}
		protected internal virtual void OnResourceIdsChanged(object sender, EventArgs e) {
			ResetResourcesItemsCore(Controller.AvailableResources);
			ResourcesCheckedListBoxControl.ResetResourcesItemsCore(ResourcesCheckedListBoxControl.Controller.AvailableResources);
			RaiseResourceIdsChanged();
		}
		protected internal override ResourceFilterController CreateResourceFilterController() {
			return new AppointmentResourcesEditResourceFilterController(this);
		}
		protected internal override ResourcesCheckedListBoxControl CreateCheckedListBoxControl() {
			AppointmentResourcesEditResourcesCheckedListBoxControl result = new AppointmentResourcesEditResourcesCheckedListBoxControl();
			result.SchedulerControl = this.SchedulerControl;
			result.CheckOnClick = true;
			return result;
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.UI.Accessibility {
	public class AppointmentResourcesEditAccessible : PopupEditAccessible {
		public AppointmentResourcesEditAccessible(RepositoryItem item)
			: base(item) {
		}
		protected override string GetName() {
			return "AppointmentResourcesEdit";
		}
		protected override TextAccessible CreateTextAccessible() {
			return new AppointmentResourcesEditTextAccessible(Item);
		}
	}
	public class AppointmentResourcesEditTextAccessible : TextAccessible {
		public AppointmentResourcesEditTextAccessible(RepositoryItemTextEdit item)
			: base(item) {
		}
		protected override string GetName() {
			return "AppointmentResourcesEditText";
		}
	}
}
