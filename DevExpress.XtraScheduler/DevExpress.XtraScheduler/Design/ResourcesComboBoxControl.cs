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
using System.Reflection;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
namespace DevExpress.XtraScheduler.UI {
	#region RepositoryItemResourcesComboBox
	[
	UserRepositoryItem("RegisterResourcesComboBoxControl"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemResourcesComboBox : RepositoryItemImageComboBox {
		static RepositoryItemResourcesComboBox() { RegisterResourcesComboBoxControl(); }
		public RepositoryItemResourcesComboBox() {
		}
		public static void RegisterResourcesComboBoxControl() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraScheduler.Bitmaps256.ResourcesComboBox.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(typeof(ResourcesComboBoxControl).Name, typeof(ResourcesComboBoxControl), typeof(RepositoryItemResourcesComboBox), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, img));
		}
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(ResourcesComboBoxControl).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		#endregion
	}
	#endregion
	#region ResourcesComboBoxControl
	[
	DXToolboxItem(DXToolboxItemKind.Regular),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "ResourcesComboBox.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuiteImageComboBoxDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("A combo box control used to filter resources within the SchedulerControl.")
	]
	public class ResourcesComboBoxControl : ImageComboBoxEdit, IResourceFilterControl {
		#region Fields
		internal static readonly object noneItemObject = new Object();
		internal static readonly object allItemObject = new Object();
		const bool defaultShowAllResourcesItem = true;
		const bool defaultShowNoneResourcesItem = true;
		ResourceFilterController controller;
		bool showAllResourcesItem = defaultShowAllResourcesItem;
		bool showNoneResourcesItem = defaultShowNoneResourcesItem;
		#endregion
		static ResourcesComboBoxControl() {
			RepositoryItemResourcesComboBox.RegisterResourcesComboBoxControl();
		}
		public ResourcesComboBoxControl() {
			this.controller = CreateResourceFilterController();
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ResourcesComboBoxControlSchedulerControl"),
#endif
Category(SRCategoryNames.Scheduler), DefaultValue(null)]
		public SchedulerControl SchedulerControl { get { return (SchedulerControl)Controller.InnerControlOwner; } set { Controller.InnerControlOwner = value; } }
		internal ResourceFilterController Controller { get { return controller; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ResourcesComboBoxControlShowAllResourcesItem"),
#endif
DefaultValue(defaultShowAllResourcesItem)]
		public bool ShowAllResourcesItem {
			get { return showAllResourcesItem; }
			set {
				if (showAllResourcesItem == value)
					return;
				showAllResourcesItem = value;
				ResetResourcesItemsCore(Controller.AvailableResources);
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ResourcesComboBoxControlShowNoneResourcesItem"),
#endif
DefaultValue(defaultShowNoneResourcesItem)]
		public bool ShowNoneResourcesItem {
			get { return showNoneResourcesItem; }
			set {
				if (showNoneResourcesItem == value)
					return;
				showNoneResourcesItem = value;
				ResetResourcesItemsCore(Controller.AvailableResources);
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("ResourcesComboBoxControlProperties"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemResourcesComboBox Properties { get { return base.Properties as RepositoryItemResourcesComboBox; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object EditValue { get { return base.EditValue; } set { base.EditValue = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("ResourcesComboBoxControlEditorTypeName")]
#endif
public override string EditorTypeName { get { return GetType().Name; } }
		#endregion
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					UnsubscribeSelectedIndexChanged();
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
		protected internal virtual ResourceFilterController CreateResourceFilterController() {
			return new ResourceFilterController(this);
		}
		protected internal virtual void SubscribeSelectedIndexChanged() {
			this.SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);
		}
		protected internal virtual void UnsubscribeSelectedIndexChanged() {
			this.SelectedIndexChanged -= new EventHandler(OnSelectedIndexChanged);
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
			BeginUpdate();
			try {
				UnsubscribeSelectedIndexChanged();
				try {
					InitComboBoxItems(resources);
				}
				finally {
					SubscribeSelectedIndexChanged();
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void ResourceVisibleChangedCore(ResourceBaseCollection resources) {
			ResetResourcesItemsCore(resources);
		}
		protected internal virtual void InitComboBoxItems(ResourceBaseCollection resources) {
			ImageComboBoxItemCollection items = Properties.Items;
			items.Clear();
			Resource visibleResource = null;
			if (ShowAllResourcesItem)
				items.Add(new ImageComboBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ResourceAll), allItemObject));
			if (ShowNoneResourcesItem)
				items.Add(new ImageComboBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ResourceNone), noneItemObject));
			string text = String.Empty;
			int visibleCount = 0;
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = resources[i];
				items.Add(new ImageComboBoxItem(resource.Caption, resource));
				if (resource.Visible) {
					text += visibleCount > 0 ? ", " : String.Empty;
					text += resource.Caption;
					visibleCount++;
					visibleResource = resource;
				}
			}
			SetEditValue(visibleCount, count, visibleResource, text);
		}
		protected internal virtual void SetEditValue(int visibleResourceCount, int availableResourceCount, Resource visibleResource, string resourcesNames) {
			if (visibleResourceCount == 1)
				this.EditValue = visibleResource;
			else if (visibleResourceCount == 0) {
				if (ShowNoneResourcesItem)
					this.EditValue = noneItemObject;
				else
					SetNullEditValue(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ResourceNone));
			}
			else if (visibleResourceCount == availableResourceCount) {
				if (ShowAllResourcesItem)
					this.EditValue = allItemObject;
				else
					SetNullEditValue(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ResourceAll));
			}
			else
				SetNullEditValue(resourcesNames);
		}
		protected internal virtual void SetNullEditValue(string nullValueText) {
			this.Properties.NullText = nullValueText;
			this.EditValue = null;
		}
		protected internal virtual void OnSelectedIndexChanged(object sender, EventArgs e) {
			Controller.BeginUpdate();
			try {
				bool allResourcesVisible = Object.ReferenceEquals(EditValue, allItemObject);
				Controller.SetAllResourcesVisible(allResourcesVisible);
				Resource resource = EditValue as Resource;
				if (resource != null)
					Controller.SetResourceVisible(resource, true);
			}
			finally {
				Controller.EndUpdate();
			}
		}
	}
	#endregion
}
