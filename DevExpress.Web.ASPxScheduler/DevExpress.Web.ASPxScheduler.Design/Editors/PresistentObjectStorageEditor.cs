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
using System.ComponentModel.Design;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Design;
using DevExpress.XtraScheduler.Design.Wizards;
using DevExpress.Web.ASPxScheduler.Design.Wizards;
namespace DevExpress.Web.ASPxScheduler.Design {
	#region ASPxAppointmentStorageTypeEditor
	public class ASPxAppointmentStorageTypeEditor : AppointmentStorageTypeEditor {
		protected internal override IDataFieldsProvider CreateDataFieldsProvider(IPersistentObjectStorage<Appointment> objectStorage, IServiceProvider serviceProvider) {
			SchedulerDesignerProvider<Appointment> provider = new SchedulerDesignerProvider<Appointment>(serviceProvider);
			ASPxSchedulerDesigner designer = provider.GetSchedulerDesigner(objectStorage);
			if (designer != null)
				return new ASPxDataFieldsProvider(designer.AppointmentDataSourceViewSchemaAccessor);
			else
				return base.CreateDataFieldsProvider(objectStorage, serviceProvider);
		}
		protected internal override SetupObjectStorageWizardRunner<Appointment> CreateWizardRunner(IDesignerHost host, IPersistentObjectStorage<Appointment> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new ASPxSetupAppointmentStorageWizardRunner(host, objectStorage, dataFieldsProvider);
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EmptyUndoSupport();
		}
		protected internal override IComponent GetEditedComponent(IPersistentObjectStorage<Appointment> objectStorage, IServiceProvider serviceProvider) {
			SchedulerDesignerProvider<Appointment> provider = new SchedulerDesignerProvider<Appointment>(serviceProvider);
			IComponent control = provider.GetScheduler(objectStorage);
			if (control != null)
				return control;
			else
				return base.GetEditedComponent(objectStorage, serviceProvider);
		}
	}
	#endregion
	#region ASPxResourceStorageTypeEditor
	public class ASPxResourceStorageTypeEditor : ResourceStorageTypeEditor {
		protected internal override IDataFieldsProvider CreateDataFieldsProvider(IPersistentObjectStorage<Resource> objectStorage, IServiceProvider serviceProvider) {
			SchedulerDesignerProvider<Resource> provider = new SchedulerDesignerProvider<Resource>(serviceProvider);
			ASPxSchedulerDesigner designer = provider.GetSchedulerDesigner(objectStorage);
			if (designer != null)
				return new ASPxDataFieldsProvider(designer.ResourceDataSourceViewSchemaAccessor);
			else
				return base.CreateDataFieldsProvider(objectStorage, serviceProvider);
		}
		protected internal override SetupObjectStorageWizardRunner<Resource> CreateWizardRunner(IDesignerHost host, IPersistentObjectStorage<Resource> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new ASPxSetupResourceStorageWizardRunner(host, objectStorage, dataFieldsProvider);
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EmptyUndoSupport();
		}
		protected internal override IComponent GetEditedComponent(IPersistentObjectStorage<Resource> objectStorage, IServiceProvider serviceProvider) {
			SchedulerDesignerProvider<Resource> provider = new SchedulerDesignerProvider<Resource>(serviceProvider);
			IComponent control = provider.GetScheduler(objectStorage);
			if (control != null)
				return control;
			else
				return base.GetEditedComponent(objectStorage, serviceProvider);
		}
	}
	#endregion
}
