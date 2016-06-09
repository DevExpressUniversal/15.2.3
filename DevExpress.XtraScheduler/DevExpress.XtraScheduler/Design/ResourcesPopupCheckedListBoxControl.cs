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
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraScheduler.UI {
	#region ResourcesPopupCheckedListBoxControl
	[
	DXToolboxItem(DXToolboxItemKind.Regular),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "ResourcesPopupCheckedListBox.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuitePopupContainerEditDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A pop-up checked list box control used to filter resources within the SchedulerControl.")
	]
	public class ResourcesPopupCheckedListBoxControl : PopupContainerEdit, IResourceFilterControl{
		#region Fields
		ResourceFilterController controller;
		PopupContainerControl popupContainerControl;
		ResourcesCheckedListBoxControl resourcesCheckedListBoxControl;
		Size popupContainerSize = Size.Empty;
		#endregion
		public ResourcesPopupCheckedListBoxControl() {
			this.controller = CreateResourceFilterController();
			this.popupContainerControl = new PopupContainerControl();
			this.resourcesCheckedListBoxControl = CreateCheckedListBoxControl();
			this.resourcesCheckedListBoxControl.Dock = DockStyle.Fill;
			this.popupContainerControl.Controls.Add(resourcesCheckedListBoxControl);
			Properties.PopupControl = popupContainerControl;
			ResetResourcesItemsCore(controller.AvailableResources);
			SubscribePopupContainerSizeEvent();
		}
		#region Properties
		#region SchedulerControl
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ResourcesPopupCheckedListBoxControlSchedulerControl"),
#endif
		Category(SRCategoryNames.Scheduler), DefaultValue(null)]
		public SchedulerControl SchedulerControl {
			get { return (SchedulerControl)controller.InnerControlOwner; }
			set {
				controller.InnerControlOwner = value;
				ResourcesCheckedListBoxControl.SchedulerControl = value;
			}
		}
		#endregion
		#region PopupControl
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("ResourcesPopupCheckedListBoxControlPopupControl")]
#endif
		public PopupContainerControl PopupControl { get { return popupContainerControl; } }
		#endregion
		#region ResourcesCheckedListBoxControl
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ResourcesPopupCheckedListBoxControlResourcesCheckedListBoxControl"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public ResourcesCheckedListBoxControl ResourcesCheckedListBoxControl { get { return resourcesCheckedListBoxControl; } }
		#endregion
		internal Size PopupContainerSize { get { return popupContainerSize; } set { popupContainerSize = value; } }
		internal ResourceFilterController Controller { get { return controller; } }
		#endregion
		protected internal virtual ResourceFilterController CreateResourceFilterController() {
			return new ResourceFilterController(this);
		}
		protected internal virtual ResourcesCheckedListBoxControl CreateCheckedListBoxControl() {
			ResourcesCheckedListBoxControl result = new ResourcesCheckedListBoxControl();
			result.SchedulerControl = this.SchedulerControl;
			return result;
		}
		protected internal virtual void SubscribePopupContainerSizeEvent() {
			this.popupContainerControl.SizeChanged += new EventHandler(OnContainerSizeChanged);
		}
		protected internal virtual void UnsubscribePopupContainerSizeEvent() {
			this.popupContainerControl.SizeChanged -= new EventHandler(OnContainerSizeChanged);
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (popupContainerControl != null) {
						UnsubscribePopupContainerSizeEvent();
						popupContainerControl.Dispose();
						popupContainerControl = null;
					}
					if (resourcesCheckedListBoxControl != null) {
						resourcesCheckedListBoxControl.Dispose();
						resourcesCheckedListBoxControl = null;
					}
					if (controller != null) {
						controller.Dispose();
						controller = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal virtual void OnContainerSizeChanged(object sender, EventArgs e) {
			this.popupContainerSize = popupContainerControl.Size;
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
			SetVisibleResourcesText(resources);
		}
		protected internal virtual void ResourceVisibleChangedCore(ResourceBaseCollection resources) {
			ResetResourcesItemsCore(resources);
		}
		protected internal virtual void SetVisibleResourcesText(ResourceBaseCollection resources) {
			string text = String.Empty;
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = resources[i];
				if (controller.GetResourceVisible(resource)) {
					if (!String.IsNullOrEmpty(text))
						text += ", ";
					text += resource.Caption;
				}
			}
			if (String.IsNullOrEmpty(text))
				text = SchedulerLocalizer.GetString(SchedulerStringId.Caption_ResourceNone);
			this.EditValue = text;
		}
		protected override void DoShowPopup() {
			UnsubscribePopupContainerSizeEvent();
			try {
				if (popupContainerSize != Size.Empty)
					popupContainerControl.Size = popupContainerSize;
				else
					popupContainerControl.Width = this.Width;
				base.DoShowPopup();
			}
			finally {
				SubscribePopupContainerSizeEvent();
			}
		}
		protected override void DoClosePopup(PopupCloseMode closeMode) {
			base.DoClosePopup(PopupCloseMode.Normal);
		}
	}
	#endregion
}
