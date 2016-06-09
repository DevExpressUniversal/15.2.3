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
using DevExpress.Utils;
namespace DevExpress.XtraScheduler.UI {
	#region RepositoryItemAppointmentLabel
	[UserRepositoryItem("RegisterAppointmentLabelEdit"), System.Runtime.InteropServices.ComVisible(false)]
	public class RepositoryItemAppointmentLabel : StorageUserInterfaceObjectRepositoryItemImageComboBox<IAppointmentLabel> {
		static RepositoryItemAppointmentLabel() {
			RegisterAppointmentLabelEdit();
		}
		public RepositoryItemAppointmentLabel() {
		}
		public static void RegisterAppointmentLabelEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraScheduler.Bitmaps256.appointmentLabelEdit.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(AppointmentLabelEdit).Name, typeof(AppointmentLabelEdit), typeof(RepositoryItemAppointmentLabel), typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), true, img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#region Properties
		internal static string InternalEditorTypeName {
			get { return typeof(AppointmentLabelEdit).Name; }
		}
		public override string EditorTypeName {
			get { return InternalEditorTypeName; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items {
			get { return base.Items; }
		}
		protected internal override UserInterfaceObjectWinCollection<IAppointmentLabel> Collection {
			get {
				SchedulerStorage schedulerStorage = Storage as SchedulerStorage;
				if ( schedulerStorage == null )
					return null;
				if ( schedulerStorage.Appointments == null )
					return null;
				return schedulerStorage.Appointments.Labels;
			}
		}
		#endregion
	}
	#endregion
	#region AppointmentLabelEdit
	[
	DXToolboxItem(DXToolboxItemKind.Regular),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	ToolboxBitmap(typeof(SchedulerControl), DevExpress.Utils.ControlConstants.BitmapPath + "appointmentLabelEdit.bmp"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuiteComboBoxEditDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	Description("An image combo box control used to select appointment labels in appointment editing dialogs.")
	]
	public class AppointmentLabelEdit : ImageComboBoxEdit {
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppointmentLabel Label {
			get {
				object obj = base.EditValue;
				if ( IsNullValue(obj) )
					return null;
				return (AppointmentLabel)obj;
			}
			set {
				base.EditValue = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object EditValue { get { return base.EditValue; } set { base.EditValue = value; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentLabelEditEditorTypeName")]
#endif
		public override string EditorTypeName { get { return GetType().Name; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("AppointmentLabelEditProperties"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemAppointmentLabel Properties { get { return base.Properties as RepositoryItemAppointmentLabel; } }
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
		#region LabelChanged
		static object onLabelChanged = new object();
		public event EventHandler LabelChanged {
			add {
				Events.AddHandler(onLabelChanged, value);
			}
			remove {
				Events.RemoveHandler(onLabelChanged, value);
			}
		}
		void RaiseLabelChanged() {
			EventHandler handler = Events[onLabelChanged] as EventHandler;
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
		static AppointmentLabelEdit() {
			RepositoryItemAppointmentLabel.RegisterAppointmentLabelEdit();
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			RaiseLabelChanged();
		}
	}
	#endregion
}
