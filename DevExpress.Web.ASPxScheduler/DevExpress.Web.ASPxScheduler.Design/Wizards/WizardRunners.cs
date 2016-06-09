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
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Design;
using DevExpress.XtraScheduler.Design.Wizards;
using System.ComponentModel;
namespace DevExpress.Web.ASPxScheduler.Design.Wizards {
	#region ASPxSetupAppointmentStorageWizardRunner
	public class ASPxSetupAppointmentStorageWizardRunner : SetupAppointmentStorageWizardRunner {
		WizPageAddInsertSupport addInsertSupportWizard;
		public ASPxSetupAppointmentStorageWizardRunner(IDesignerHost host, IPersistentObjectStorage<Appointment> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
			this.addInsertSupportWizard = new WizPageAddInsertSupport((ASPxAppointmentStorage)objectStorage);
		}
		protected internal WizPageAddInsertSupport AddInsertSupportWizard { get { return addInsertSupportWizard; } }
		protected internal override SetupCustomFieldMappingsWizard<Appointment> CreateCustomFieldMappingsWizard(IDesignerHost host, IPersistentObjectStorage<Appointment> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new ASPxSetupAppointmentCustomFieldMappingsWizard(host, objectStorage, dataFieldsProvider);
		}
		protected internal override void AppendWizardPages(SchedulerWizardForm form) {
			base.AppendWizardPages(form);
			form.Controls.Add(AddInsertSupportWizard);
		}
		protected override void SetupMappingsWizardExtension(SetupMappingsWizard<Appointment> setupMappingsWizard) {
		}
	}
	#endregion
	#region ASPxSetupResourceStorageWizardRunner
	public class ASPxSetupResourceStorageWizardRunner : SetupResourceStorageWizardRunner {
		public ASPxSetupResourceStorageWizardRunner(IDesignerHost host, IPersistentObjectStorage<Resource> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		protected internal override SetupCustomFieldMappingsWizard<Resource> CreateCustomFieldMappingsWizard(IDesignerHost host, IPersistentObjectStorage<Resource> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new ASPxSetupResourceCustomFieldMappingsWizard(host, objectStorage, dataFieldsProvider);
		}
	}
	#endregion
	public class ASPxSetupAppointmentBaseMappingsWizardRunner : SetupAppointmentBaseMappingsWizardRunner {
		public ASPxSetupAppointmentBaseMappingsWizardRunner(IDesignerHost host, IPersistentObjectStorage<Appointment> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		protected override void SetupMappingsWizardExtension(SetupMappingsWizard<Appointment> setupMappingsWizard) {
		}
	}
	public class ASPxSetupResourceBaseMappingsWizardRunner : SetupResourceBaseMappingsWizardRunner {
		public ASPxSetupResourceBaseMappingsWizardRunner(IDesignerHost host, IPersistentObjectStorage<Resource> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		protected override void SetupMappingsWizardExtension(SetupMappingsWizard<Resource> setupMappingsWizard) {
		}
	}
	#region AddFormTemplatesWizardRunner
	public class AddFormTemplatesWizardRunner {
		AddFormTemplatesWizard wizard;
		string[] newAddedFiles;
		string targetFolder;
		public AddFormTemplatesWizardRunner(IDesignerHost host, string targetFolder, string[] newAddedFiles) {
			this.newAddedFiles = newAddedFiles;
			this.targetFolder = targetFolder;
			this.wizard = new AddFormTemplatesWizard(host);
		}
		public DialogResult Run() {
			using (ASPxSchedulerNotifyWizardForm form = new ASPxSchedulerNotifyWizardForm()) {
				form.Text = "Add ASPxScheduler Form Templates";
				form.Controls.AddRange(new Control[] {
													new WizPageAddFormTemplates(wizard, targetFolder, newAddedFiles)
												});
				DialogResult result = form.ShowDialog();
				return result;
			}
		}
	}
	#endregion
	#region ASPxAppointmentDataSourceTriggeredWizardRunner
	public class ASPxAppointmentDataSourceTriggeredWizardRunner : AppointmentDataSourceTriggeredWizardRunner {
		public ASPxAppointmentDataSourceTriggeredWizardRunner(ISchedulerStorageBase storage, IDesignerHost host, IDesignerLoadCompleteNotifier loadCompleteNotifier, Control controlForInvoke)
			: base(storage, host, loadCompleteNotifier, controlForInvoke) {
		}
		protected internal override IDataFieldsProvider CreateDataFieldsProvider() {
			SchedulerDesignerProvider<Appointment> provider = new SchedulerDesignerProvider<Appointment>(DesignerHost);
			ASPxSchedulerDesigner designer = provider.GetSchedulerDesigner(ObjectStorage);
			if (designer != null)
				return new ASPxDataFieldsProvider(designer.AppointmentDataSourceViewSchemaAccessor);
			else
				return base.CreateDataFieldsProvider();
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EmptyUndoSupport();
		}
		protected internal override IComponent GetEditedComponent() {
			SchedulerDesignerProvider<Appointment> provider = new SchedulerDesignerProvider<Appointment>(DesignerHost);
			IComponent control = provider.GetScheduler(ObjectStorage);
			if (control != null)
				return control;
			else
				return base.GetEditedComponent();
		}
		protected internal override SetupObjectStorageWizardRunner<Appointment> CreateWizardRunner(IDataFieldsProvider dataFieldsProvider) {
			return new ASPxSetupAppointmentStorageWizardRunner(DesignerHost, ObjectStorage, dataFieldsProvider);
		}
	}
	#endregion
	#region ASPxResourceDataSourceTriggeredWizardRunner
	public class ASPxResourceDataSourceTriggeredWizardRunner : ResourceDataSourceTriggeredWizardRunner {
		public ASPxResourceDataSourceTriggeredWizardRunner(ISchedulerStorageBase storage, IDesignerHost host, IDesignerLoadCompleteNotifier loadCompleteNotifier, Control controlForInvoke)
			: base(storage, host, loadCompleteNotifier, controlForInvoke) {
		}
		protected internal override IDataFieldsProvider CreateDataFieldsProvider() {
			SchedulerDesignerProvider<Resource> provider = new SchedulerDesignerProvider<Resource>(DesignerHost);
			ASPxSchedulerDesigner designer = provider.GetSchedulerDesigner(ObjectStorage);
			if (designer != null)
				return new ASPxDataFieldsProvider(designer.ResourceDataSourceViewSchemaAccessor);
			else
				return base.CreateDataFieldsProvider();
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EmptyUndoSupport();
		}
		protected internal override IComponent GetEditedComponent() {
			SchedulerDesignerProvider<Resource> provider = new SchedulerDesignerProvider<Resource>(DesignerHost);
			IComponent control = provider.GetScheduler(ObjectStorage);
			if (control != null)
				return control;
			else
				return base.GetEditedComponent();
		}
	}
	#endregion
}
