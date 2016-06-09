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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
namespace DevExpress.XtraScheduler.UI {
	#region ResourcesCheckedListBoxControl
	[
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "ResourcesCheckedListBox.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuiteControlDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A checked list box control used to filter resources within the SchedulerControl.")
	]
	[Docking(DockingBehavior.Ask)]
	public class ResourcesCheckedListBoxControl : CheckedListBoxControl, IResourceFilterControl {
		#region Fields
		ResourceFilterController controller;
		bool disposeController;
		#endregion
		public ResourcesCheckedListBoxControl() {
			this.controller = CreateResourceFilterController();
			this.disposeController = true;
		}
		public ResourcesCheckedListBoxControl(ResourceFilterController controller) {
			if (controller == null)
				Exceptions.ThrowArgumentException("controller", controller);
			this.controller = controller;
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ResourcesCheckedListBoxControlSchedulerControl"),
#endif
Category(SRCategoryNames.Scheduler), DefaultValue(null)]
		public SchedulerControl SchedulerControl {
			get { return (SchedulerControl)controller.InnerControlOwner; }
			set { controller.InnerControlOwner = value; }
		}
		internal ResourceFilterController Controller { get { return controller; } }
		internal bool DisposeController { get { return disposeController; } }
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (controller != null) {
						if (disposeController)
							controller.Dispose();
					}
					controller = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected internal virtual ResourceFilterController CreateResourceFilterController() {
			return new ResourceFilterController(this);
		}
		[Obsolete("You should use the 'IResourceFilterControl.ResetResourcesItems' instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetResourcesItems(ResourceBaseCollection resources) {
			ResetResourcesItemsCore(resources);
		}
		[Obsolete("You should use the 'IResourceFilterControl.ResourceVisibleChanged' instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ResourceVisibleChanged(ResourceBaseCollection resources) {
			ResourceVisibleChangedCore(resources);
		}
		void IResourceFilterControl.ResetResourcesItems(ResourceBaseCollection resources) {
			ResetResourcesItemsCore(resources);
		}
		void IResourceFilterControl.ResourceVisibleChanged(ResourceBaseCollection resources) {
			ResourceVisibleChangedCore(resources);
		}
		protected internal virtual void ResetResourcesItemsCore(ResourceBaseCollection resources) {
			object selectedResourceId = null;
			ResourceCheckedListBoxItem selectedItem = (ResourceCheckedListBoxItem)SelectedValue;
			if (selectedItem != null)
				selectedResourceId = selectedItem.Resource.Id;
			BeginUpdate();
			try {
				InitListBoxItems(resources);
			}
			finally {
				this.SelectedIndex = FindNewSelectedItemIndexByResourceId(selectedResourceId);
				EndUpdate();
			}
		}
		protected internal virtual void ResourceVisibleChangedCore(ResourceBaseCollection resources) {
			ResetResourcesItemsCore(resources);
		}
		protected internal virtual int FindNewSelectedItemIndexByResourceId(object resourceId) {
			int count = Items.Count;
			if (count <= 0)
				return this.SelectedIndex;
			for (int i = 0; i < count; i++) {
				ResourceCheckedListBoxItem selectedItem = (ResourceCheckedListBoxItem)Items[i].Value;
				if (Object.Equals(selectedItem.Resource.Id, resourceId))
					return i;
			}
			return 0;
		}
		protected internal virtual void InitListBoxItems(ResourceBaseCollection resources) {
			Items.Clear();
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = resources[i];
				Items.Add(new ResourceCheckedListBoxItem(resource), controller.GetResourceVisible(resource));
			}
		}
		protected override void OnSetItemCheckState(ItemCheckingEventArgs e) {
			base.OnSetItemCheckState(e);
			if (e.Cancel)
				return;
			ChangeResourceVisibility(e.Index, e.NewValue);
		}
		protected override void SetItemsChecked(bool check) {
			base.SetItemsChecked(check);
			Controller.BeginUpdate();
			try {
				int count = Items.Count;
				for (int i = 0; i < count; i++) {
					ChangeResourceVisibility(i, GetItemCheckState(i));
				}
			}
			finally {
				Controller.EndUpdate();
			}
		}
		protected internal virtual void ChangeResourceVisibility(int index, CheckState value) {
			CheckedListBoxItem checkItem = Items[index];
			ResourceCheckedListBoxItem item = (ResourceCheckedListBoxItem)checkItem.Value;
			bool check = (value == CheckState.Checked);
			Controller.SetResourceVisible(item.Resource, check);
		}
	}
	#endregion
	#region ResourceCheckedListBoxItem
	public class ResourceCheckedListBoxItem {
		Resource resource;
		public ResourceCheckedListBoxItem(Resource resource) {
			if (resource == null)
				Exceptions.ThrowArgumentException("resource", resource);
			this.resource = resource;
		}
		public Resource Resource { get { return resource; } }
		public override string ToString() {
			return resource.Caption;
		}
	}
	#endregion
}
