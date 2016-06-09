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
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.UI {
	#region StorageBindedImageComboBoxEdit (abstract class)
	[
	DXToolboxItem(false),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public abstract class StorageBindedImageComboBoxEdit : ImageComboBoxEdit {
		SchedulerStorage storage;
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (storage != null) {
						UnsubscribeStorageEvents();
						storage = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("StorageBindedImageComboBoxEditStorage"),
#endif
DefaultValue(null), Category(SRCategoryNames.Scheduler)]
		public SchedulerStorage Storage {
			get { return storage; }
			set {
				if (storage == value)
					return;
				if (storage != null)
					UnsubscribeStorageEvents();
				storage = value;
				if (storage != null) {
					SubscribeStorageEvents();
					RefreshData();
				}
				else
					ClearData();
			}
		}
		protected internal virtual void SubscribeStorageEvents() {
			if (storage != null)
				storage.BeforeDispose += new EventHandler(OnBeforeStorageDispose);
		}
		protected internal virtual void UnsubscribeStorageEvents() {
			if (storage != null)
				storage.BeforeDispose -= new EventHandler(OnBeforeStorageDispose);
		}
		protected internal virtual void OnBeforeStorageDispose(object sender, EventArgs e) {
			Storage = null;
		}
		protected internal virtual void ClearData() {
			Properties.Items.Clear();
		}
		protected internal virtual ImageList CreateSmallImageList() {
			return RepositoryImageComboBoxFillHelperBase.CreateSmallImageList();
		}
		protected internal virtual Rectangle CreateItemImageRectangle() {
			return RepositoryImageComboBoxFillHelperBase.CreateItemImageRectangle();
		}
		protected internal abstract void RefreshData();
	}
	#endregion
	#region StorageBindedRepositoryItemImageComboBox (abstract class)
	[
	DXToolboxItem(false),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public abstract class StorageBindedRepositoryItemImageComboBox : RepositoryItemImageComboBox {
		ISchedulerStorage storage;
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (storage != null) {
						UnsubscribeStorageEvents();
						storage = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#region Properties
		#region Storage
		[DefaultValue(null), Category(SRCategoryNames.Scheduler), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NotifyParentProperty(true)]
		public ISchedulerStorage Storage {
			get { return storage; }
			set {
				if (storage == value)
					return;
				if (storage != null)
					UnsubscribeStorageEvents();
				storage = value;
				if (storage != null) {
					SubscribeStorageEvents();
					RefreshData();
				}
				else
					ClearData();
			}
		}
		#endregion
		#endregion
		protected internal virtual void SubscribeStorageEvents() {
			if (storage != null)
				((IInternalSchedulerStorageBase)storage).BeforeDispose += new EventHandler(OnBeforeStorageDispose);
		}
		protected internal virtual void UnsubscribeStorageEvents() {
			if (storage != null)
				((IInternalSchedulerStorageBase)storage).BeforeDispose -= new EventHandler(OnBeforeStorageDispose);
		}
		protected internal virtual void OnBeforeStorageDispose(object sender, EventArgs e) {
			Storage = null;
		}
		protected internal virtual void ClearData() {
			Items.Clear();
		}
		protected internal virtual ImageList CreateSmallImageList() {
			return RepositoryImageComboBoxFillHelperBase.CreateSmallImageList();
		}
		protected internal virtual Rectangle CreateItemImageRectangle() {
			return RepositoryImageComboBoxFillHelperBase.CreateItemImageRectangle();
		}
		public abstract void RefreshData();
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region RepositoryImageComboBoxFillHelperBase
	public static class RepositoryImageComboBoxFillHelperBase {
		const int size = 16;
		const int offset = 0;
		public static ImageList CreateSmallImageList() {
			ImageList result = new ImageList();
			result.ColorDepth = ColorDepth.Depth24Bit;
			result.ImageSize = new Size(size, size);
			result.TransparentColor = Color.Transparent;
			return result;
		}
		public static Rectangle CreateItemImageRectangle() {
			return new Rectangle(offset, offset, size - 2 * offset, size - 2 * offset);
		}
	}
	#endregion
}
