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
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Design.Wizards;
using DevExpress.XtraScheduler.Localization;
namespace DevExpress.XtraScheduler.Design {
	#region PersistentObjectStorageTypeEditor<T> (abstract class)
	public abstract class PersistentObjectStorageTypeEditor<T> : UITypeEditor where T : IPersistentObject {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return (context != null && context.Instance != null) ? UITypeEditorEditStyle.Modal : base.GetEditStyle();
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return (context != null && context.Instance != null) ? false : base.GetPaintValueSupported(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (provider != null && context != null) {
				IDesignerHost host = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
				if (host != null) {
					PersistentObjectStorage<T> objectStorage = value as PersistentObjectStorage<T>;
					if (objectStorage != null) {
						SetupObjectStorageWizardRunner<T> wizardRunner = CreateWizardRunner(host, objectStorage, CreateDataFieldsProvider(objectStorage, provider));
						using (IDisposable undoSupport = CreateUndoSupportObject()) {
							DesignTimeTransactionHelper.InvokeTransactedChange(host, GetEditedComponent(objectStorage, provider), RunWizard, wizardRunner, TransactionDescription, null);
						}
					}
				}
			}
			return value;
		}
		protected internal bool RunWizard(object context) {
			SetupObjectStorageWizardRunner<T> wizardRunner = (SetupObjectStorageWizardRunner<T>)context;
			return wizardRunner.Run() == DialogResult.OK;
		}
		protected internal virtual IDataFieldsProvider CreateDataFieldsProvider(IPersistentObjectStorage<T> objectStorage, IServiceProvider serviceProvider) {
			return new DefaultDataFieldsProvider<T>(objectStorage);
		}
		protected internal virtual IComponent GetEditedComponent(IPersistentObjectStorage<T> objectStorage, IServiceProvider serviceProvider) {
			return (IComponent)objectStorage.Storage;
		}
		protected internal abstract IDisposable CreateUndoSupportObject();
		protected internal abstract SetupObjectStorageWizardRunner<T> CreateWizardRunner(IDesignerHost host, IPersistentObjectStorage<T> objectStorage, IDataFieldsProvider dataFieldsProvider);
		protected internal abstract string TransactionDescription { get; }
	}
	#endregion
	#region AppointmentStorageTypeEditor
	public class AppointmentStorageTypeEditor : PersistentObjectStorageTypeEditor<Appointment> {
		protected internal override string TransactionDescription { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_ModifyAppointmentStorageTransactionDescription); } }
		protected internal override SetupObjectStorageWizardRunner<Appointment> CreateWizardRunner(IDesignerHost host, IPersistentObjectStorage<Appointment> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new SetupAppointmentStorageWizardRunner(host, objectStorage, dataFieldsProvider);
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EnsureInnerObjectUndoRedoSupport(typeof(AppointmentMappingInfo));
		}
	}
	#endregion
	#region ResourceStorageTypeEditor
	public class ResourceStorageTypeEditor : PersistentObjectStorageTypeEditor<Resource> {
		protected internal override string TransactionDescription { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_ModifyResourceStorageTransactionDescription); } }
		protected internal override SetupObjectStorageWizardRunner<Resource> CreateWizardRunner(IDesignerHost host, IPersistentObjectStorage<Resource> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new SetupResourceStorageWizardRunner(host, objectStorage, dataFieldsProvider);
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EnsureInnerObjectUndoRedoSupport(typeof(ResourceMappingInfo));
		}
	}
	#endregion
	#region AppointmentDependencyStorageTypeEditor
	public class AppointmentDependencyStorageTypeEditor : PersistentObjectStorageTypeEditor<AppointmentDependency> {
		protected internal override string TransactionDescription { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_ModifyAppointmentDependencyStorageTransactionDescription); } }
		protected internal override SetupObjectStorageWizardRunner<AppointmentDependency> CreateWizardRunner(IDesignerHost host, IPersistentObjectStorage<AppointmentDependency> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new SetupAppointmentDependencyStorageWizardRunner(host, objectStorage, dataFieldsProvider);
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EnsureInnerObjectUndoRedoSupport(typeof(AppointmentDependencyMappingInfo));
		}
	}
	#endregion
}
