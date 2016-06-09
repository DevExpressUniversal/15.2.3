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
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using DevExpress.Utils;
namespace DevExpress.XtraScheduler.UI {
	#region RepositoryItemAppointmentStatus
	[UserRepositoryItem("RegisterAppointmentStatusEdit"), System.Runtime.InteropServices.ComVisible(false)]
	public class RepositoryItemAppointmentStatus : StorageUserInterfaceObjectRepositoryItemImageComboBox<IAppointmentStatus> {
		static RepositoryItemAppointmentStatus() {
			RegisterAppointmentStatusEdit();
		}
		public RepositoryItemAppointmentStatus() {
		}
		public static void RegisterAppointmentStatusEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraScheduler.Bitmaps256.appointmentStatusEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(AppointmentStatusEdit).Name, typeof(AppointmentStatusEdit), typeof(RepositoryItemAppointmentStatus), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#region Properties
		internal static string InternalEditorTypeName {
			get { return typeof(AppointmentStatusEdit).Name; }
		}
		public override string EditorTypeName {
			get { return InternalEditorTypeName; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items {
			get { return base.Items; }
		}
		protected internal override UserInterfaceObjectWinCollection<IAppointmentStatus> Collection {
			get {
				SchedulerStorage schedulerStorage = Storage as SchedulerStorage;
				if ( schedulerStorage == null )
					return null;
				if ( schedulerStorage.Appointments == null )
					return null;
				return schedulerStorage.Appointments.Statuses;
			}
		}
		#endregion
	}
	#endregion
	#region AppointmentStatusEdit
	[
	DXToolboxItem(DXToolboxItemKind.Regular),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "appointmentStatusEdit.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuiteComboBoxEditDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("An image combo box control used to select appointment status in appointment editing dialogs.")
	]
	public class AppointmentStatusEdit : ImageComboBoxEdit {
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppointmentStatus Status {
			get {
				object obj = base.EditValue;
				if (IsNullValue(obj))
					return null;
				return (AppointmentStatus)obj;
			}
			set {
				if (base.EditValue == value)
					return;
				base.EditValue = value;
				RaiseStatusChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object EditValue { get { return base.EditValue; } set { base.EditValue = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentStatusEditEditorTypeName")]
#endif
		public override string EditorTypeName { get { return GetType().Name; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("AppointmentStatusEditProperties"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemAppointmentStatus Properties { get { return base.Properties as RepositoryItemAppointmentStatus; } }
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
		#endregion
		#region Events
		#region StatusChanged
		static object onStatusChanged = new object();
		public event EventHandler StatusChanged {
			add { Events.AddHandler(onStatusChanged, value); }
			remove { Events.RemoveHandler(onStatusChanged, value); }
		}
		void RaiseStatusChanged() {
			EventHandler handler = Events[onStatusChanged] as EventHandler;
			if (handler == null)
				return;
			handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		public virtual void RefreshData() {
			if (Properties != null)
				Properties.RefreshData();
		}
		static AppointmentStatusEdit() {
			RepositoryItemAppointmentStatus.RegisterAppointmentStatusEdit();
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			RaiseStatusChanged();
		}
	}
	#endregion
}
