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
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.UI {
	#region RepositoryItemAppointmentResource
	[
	UserRepositoryItem("RegisterAppointmentResourceEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemAppointmentResource : StorageBindedRepositoryItemImageComboBox {
		#region Fields
		const bool defaultShowEmptyResource = true;
		SchedulerControl control;
		SchedulerColorSchemaCollection resourceColors;
		NotificationCollectionChangedListener<SchedulerColorSchema> resourceColorsListener;
		bool showEmptyResource = defaultShowEmptyResource;
		#endregion
		static RepositoryItemAppointmentResource() { RegisterAppointmentResourceEdit(); }
		public RepositoryItemAppointmentResource() {
			resourceColors = new SchedulerColorSchemaCollection();
			resourceColorsListener = new NotificationCollectionChangedListener<SchedulerColorSchema>(resourceColors);
			SubscribeResourceColorsEvents();
			RefreshData();
		}
		public static void RegisterAppointmentResourceEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraScheduler.Bitmaps256.resourcesEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(AppointmentResourceEdit).Name, typeof(AppointmentResourceEdit), typeof(RepositoryItemAppointmentResource), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(AppointmentResourceEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		protected internal SchedulerColorSchemaCollection ResourceColors { get { return resourceColors; } }
		protected internal NotificationCollectionChangedListener<SchedulerColorSchema> ResourceColorsListener { get { return resourceColorsListener; } }
		#region SchedulerControl
		[DefaultValue(null), Category(SRCategoryNames.Scheduler), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerControl SchedulerControl {
			get { return control; }
			set {
				if (control == value)
					return;
				if (control != null)
					UnsubscribeControlEvents();
				if (resourceColorsListener != null) {
					UnsubscribeResourceColorsEvents();
					resourceColorsListener.Dispose();
					resourceColorsListener = null;
				}
				control = value;
				if (control != null) {
					SubscribeControlEvents();
					resourceColors = control.ActualResourceColorSchemas;
				} else {
					resourceColors = new SchedulerColorSchemaCollection();
				}
				resourceColorsListener = new NotificationCollectionChangedListener<SchedulerColorSchema>(resourceColors);
				SubscribeResourceColorsEvents();
				RefreshData();
			}
		}
		#endregion
		#region ShowEmptyResource
		[DefaultValue(defaultShowEmptyResource)]
		public bool ShowEmptyResource {
			get { return showEmptyResource; }
			set {
				if (showEmptyResource == value)
					return;
				showEmptyResource = value;
				RefreshData();
			}
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (control != null) {
						UnsubscribeControlEvents();
						control = null;
					}
					if (resourceColorsListener != null) {
						UnsubscribeResourceColorsEvents();
						resourceColorsListener.Dispose();
						resourceColorsListener = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected internal virtual void SubscribeResourceColorsEvents() {
			resourceColorsListener.Changed += new EventHandler(OnResourceColorsChanged);
		}
		protected internal virtual void UnsubscribeResourceColorsEvents() {
			resourceColorsListener.Changed -= new EventHandler(OnResourceColorsChanged);
		}
		protected internal virtual void OnResourceColorsChanged(object sender, EventArgs args) {
			RefreshData();
		}
		protected internal virtual void SubscribeControlEvents() {
			if (control != null)
				control.BeforeDispose += new EventHandler(OnBeforeControlDispose);
		}
		protected internal virtual void UnsubscribeControlEvents() {
			if (control != null)
				control.BeforeDispose -= new EventHandler(OnBeforeControlDispose);
		}
		protected internal virtual void OnBeforeControlDispose(object sender, EventArgs e) {
			SchedulerControl = null;
		}
		protected internal override void SubscribeStorageEvents() {
			if (Storage == null)
				return;
			base.SubscribeStorageEvents();
			IInternalSchedulerStorageBase internalStorage = (IInternalSchedulerStorageBase)Storage;
			internalStorage.InternalResourcesChanged += new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesDeleted += new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesInserted += new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourceCollectionCleared += new EventHandler(OnResourceCollectionChanged);
			internalStorage.InternalResourceCollectionLoaded += new EventHandler(OnResourceCollectionChanged);
		}
		protected internal override void UnsubscribeStorageEvents() {
			if (Storage == null)
				return;
			base.UnsubscribeStorageEvents();
			IInternalSchedulerStorageBase internalStorage = (IInternalSchedulerStorageBase)Storage;
			internalStorage.InternalResourcesChanged -= new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesDeleted -= new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourcesInserted -= new PersistentObjectsEventHandler(OnResourcesChanged);
			internalStorage.InternalResourceCollectionCleared -= new EventHandler(OnResourceCollectionChanged);
			internalStorage.InternalResourceCollectionLoaded -= new EventHandler(OnResourceCollectionChanged);
		}
		protected internal virtual void OnResourcesChanged(object sender, PersistentObjectsEventArgs e) {
			RefreshData();
		}
		protected internal virtual void OnResourceCollectionChanged(object sender, EventArgs e) {
			RefreshData();
		}
		public override void RefreshData() {
			ResourceBaseCollection resourceCollection;
			if (Storage == null)
				resourceCollection = new ResourceBaseCollection();
			else {
				resourceCollection = ((IInternalSchedulerStorageBase)Storage).GetVisibleResources(true);
			}
			BeginUpdate();
			try {
				ClearData();
				SmallImages = CreateSmallImageList();
				Rectangle r = CreateItemImageRectangle();
				int indexOffset;
				if (ShowEmptyResource) {
					AddItem(ResourceBase.Empty, r, 0, 0);
					indexOffset = 1;
				} else
					indexOffset = 0;
				int count = resourceCollection.Count;
				for (int i = 0; i < count; i++)
					AddItem(resourceCollection[i], r, i + indexOffset, i);
			} finally {
				EndUpdate();
			}
		}
		protected internal virtual void AddItem(Resource resource, Rectangle r, int index, int resourceIndex) {
			Bitmap bmp = CreateBitmapForResource(r, resource, resourceIndex);
			ImageCollection.AddImage(SmallImages, bmp);
			Items.Add(new ImageComboBoxItem(resource.Caption, resource.Id, index));
		}
		protected internal virtual Bitmap CreateBitmapForResource(Rectangle r, Resource resource, int resourceIndex) {
			Bitmap bmp = new Bitmap(r.Width, r.Height);
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (Brush br = CreateResourceBrush(resource, resourceIndex)) {
					gr.FillRectangle(br, r);
				}
				gr.FillRectangle(Brushes.Black, RectUtils.GetTopSideRect(r, 1));
				gr.FillRectangle(Brushes.Black, RectUtils.GetLeftSideRect(r, 1));
				gr.FillRectangle(Brushes.Black, RectUtils.GetRightSideRect(r, 1));
				gr.FillRectangle(Brushes.Black, RectUtils.GetBottomSideRect(r, 1));
			}
			return bmp;
		}
		protected internal virtual Brush CreateResourceBrush(Resource resource, int resourceIndex) {
			if (resource.GetColor() != Color.Empty)
				return new SolidBrush(resource.GetColor());
			else {
				Color color = resourceColors.GetSchema(resourceIndex).CellLight;
				return new SolidBrush(color);
			}
		}
	}
	#endregion
	#region AppointmentResourceEdit
	[
	DXToolboxItem(DXToolboxItemKind.Regular),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "resourcesEdit.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuiteComboBoxEditDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("An image combo box control used to select a resource in appointment editing dialogs.")
	]
	public class AppointmentResourceEdit : ImageComboBoxEdit {
		static AppointmentResourceEdit() {
			RepositoryItemAppointmentResource.RegisterAppointmentResourceEdit();
		}
		public AppointmentResourceEdit() {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object ResourceId { get { return EditValue; } set { EditValue = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object EditValue { get { return base.EditValue; } set { base.EditValue = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentResourceEditEditorTypeName")]
#endif
		public override string EditorTypeName { get { return GetType().Name; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("AppointmentResourceEditProperties"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemAppointmentResource Properties { get { return base.Properties as RepositoryItemAppointmentResource; } }
		#region Storage
		[
DefaultValue(null), Category(SRCategoryNames.Scheduler)]
		public ISchedulerStorage Storage {
			get {
				if (Properties != null)
					return Properties.Storage;
				else
					return null;
			}
			set {
				if (Properties != null)
					Properties.Storage = value;
			}
		}
		#endregion
		#region SchedulerControl
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("AppointmentResourceEditSchedulerControl"),
#endif
DefaultValue(null), Category(SRCategoryNames.Scheduler), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerControl SchedulerControl {
			get {
				if (Properties != null)
					return Properties.SchedulerControl;
				else
					return null;
			}
			set {
				if (Properties != null)
					Properties.SchedulerControl = value;
			}
		}
		#endregion
		#endregion
		#region Events
		static object onResourceIdChanged = new object();
		public event EventHandler ResourceIdChanged {
			add { Events.AddHandler(onResourceIdChanged, value); }
			remove { Events.RemoveHandler(onResourceIdChanged, value); }
		}
		protected virtual void RaiseResourceIdChanged() {
			EventHandler handler = Events[onResourceIdChanged] as EventHandler;
			if (handler == null)
				return;
			handler(this, EventArgs.Empty);
		}
		#endregion
		public virtual void RefreshData() {
			if (Properties != null)
				Properties.RefreshData();
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			RaiseResourceIdChanged();
		}
	}
	#endregion
}
