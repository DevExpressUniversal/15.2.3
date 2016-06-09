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
using DevExpress.XtraEditors.Repository;
using System.Collections;
using DevExpress.XtraScheduler.Localization;
namespace DevExpress.XtraScheduler.UI {
	#region RepositoryItemRegisterAppointmentDependencyType
	[
	UserRepositoryItem("RegisterAppointmentDependencyTypeEdit"),
	System.Runtime.InteropServices.ComVisible(false)
	]
	public class RepositoryItemRegisterAppointmentDependencyType : RepositoryItemImageComboBox {
		static Hashtable displayNameHT = new Hashtable();
		static RepositoryItemRegisterAppointmentDependencyType() {
			RegisterAppointmentDependencyTypeEdit();
			displayNameHT[AppointmentDependencyType.FinishToStart] = SchedulerStringId.Caption_AppointmentDependencyTypeFinishToStart;
			displayNameHT[AppointmentDependencyType.StartToStart] = SchedulerStringId.Caption_AppointmentDependencyTypeStartToStart;
			displayNameHT[AppointmentDependencyType.FinishToFinish] = SchedulerStringId.Caption_AppointmentDependencyTypeFinishToFinish;
			displayNameHT[AppointmentDependencyType.StartToFinish] = SchedulerStringId.Caption_AppointmentDependencyTypeStartToFinish;			
		}
		public RepositoryItemRegisterAppointmentDependencyType() {
			InitItems();
		}
		#region RegisterAppointmentDependencyTypeEdit
		public static void RegisterAppointmentDependencyTypeEdit() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraScheduler.Bitmaps256.schedulercontrol.bmp", Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(typeof(AppointmentDependencyTypeEdit).Name, 
											 typeof(AppointmentDependencyTypeEdit), 
											 typeof(RepositoryItemRegisterAppointmentDependencyType), 
											 typeof(DevExpress.XtraEditors.ViewInfo.ImageComboBoxEditViewInfo), 
											 new DevExpress.XtraEditors.Drawing.ImageComboBoxEditPainter(), 
											 true, 
											 img);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		#endregion
		#region Properties
		internal static string InternalEditorTypeName { get { return typeof(AppointmentDependencyTypeEdit).Name; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageComboBoxItemCollection Items { get { return base.Items; } }
		#endregion
		protected internal virtual void InitItems() {
			BeginUpdate();
			try {
				Items.Clear();
				AppointmentDependencyType[] values = (AppointmentDependencyType[])Enum.GetValues(typeof(AppointmentDependencyType));
				int count = values.Length;
				for (int i = 0; i < count; i++)					
						Items.Add(new ImageComboBoxItem(GetDisplayName(values[i]), values[i], -1));
			}
			finally {
				CancelUpdate();
			}
		}
		static string GetDisplayName(AppointmentDependencyType val) {
			return SchedulerLocalizer.GetString((SchedulerStringId)displayNameHT[val]);
		}
	}
	#endregion
	#region AppointmentDependencyTypeEdit
	[
	DXToolboxItem(false),
	ToolboxTabName(AssemblyInfo.DXTabScheduling),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraScheduler.Design.XtraSchedulerSuiteComboBoxEditDesigner," + AssemblyInfo.SRAssemblySchedulerDesign),
	]
	public class AppointmentDependencyTypeEdit : ImageComboBoxEdit {
		static AppointmentDependencyTypeEdit() {
			RepositoryItemRegisterAppointmentDependencyType.RegisterAppointmentDependencyTypeEdit();
		}
		#region Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object EditValue { get { return base.EditValue; } set { base.EditValue = value; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("AppointmentDependencyTypeEditType"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppointmentDependencyType Type
		{
			get { return (AppointmentDependencyType)EditValue; }
			set { EditValue = value; }
		}
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentDependencyTypeEditEditorTypeName")]
#endif
		public override string EditorTypeName { get { return GetType().Name; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("AppointmentDependencyTypeEditProperties"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemRegisterAppointmentDependencyType Properties { get { return base.Properties as RepositoryItemRegisterAppointmentDependencyType; } }
		#endregion
	}
	#endregion
}
