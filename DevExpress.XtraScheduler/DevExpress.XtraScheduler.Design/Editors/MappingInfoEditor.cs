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
namespace DevExpress.XtraScheduler.Design {
	#region MappingInfoTypeEditor<T> (abstract class)
	public abstract class MappingInfoTypeEditor<T> : UITypeEditor where T : IPersistentObject {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return (context != null && context.Instance != null) ? UITypeEditorEditStyle.Modal : base.GetEditStyle();
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return (context != null && context.Instance != null) ? false : base.GetPaintValueSupported(context);
		}
		protected internal bool RunWizard(object context) {
			SetupBaseMappingsWizardRunner<T> wizardRunner = (SetupBaseMappingsWizardRunner<T>)context;
			return wizardRunner.Run() == DialogResult.OK;
		}
		protected internal virtual IDataFieldsProvider CreateDataFieldsProvider(IPersistentObjectStorage<T> objectStorage, IServiceProvider serviceProvider) {
			return new DefaultDataFieldsProvider<T>(objectStorage);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (provider != null && context != null) {
				IDesignerHost host = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
				if (host != null) {
					PersistentObjectStorage<T> objectStorage = context.Instance as PersistentObjectStorage<T>;
					if (objectStorage != null) {
						SetupBaseMappingsWizardRunner<T> wizardRunner = CreateWizardRunner(host, objectStorage, CreateDataFieldsProvider(objectStorage, provider));
						using (IDisposable undoSupport = CreateUndoSupportObject()) {
							DesignTimeTransactionHelper.InvokeTransactedChange(host, GetEditedComponent(objectStorage, provider), RunWizard, wizardRunner, TransactionDescription, null);
						}
					}
				}
			}
			return value;
		}
		protected internal virtual IComponent GetEditedComponent(IPersistentObjectStorage<T> objectStorage, IServiceProvider serviceProvider) {
			return objectStorage.Storage as IComponent;
		}
		protected internal abstract string TransactionDescription { get; }
		protected internal abstract IDisposable CreateUndoSupportObject();
		protected internal abstract SetupBaseMappingsWizardRunner<T> CreateWizardRunner(IDesignerHost host, IPersistentObjectStorage<T> objectStorage, IDataFieldsProvider dataFieldsProvider);
	}
	#endregion
	#region AppointmentMappingInfoTypeEditor
	public class AppointmentMappingInfoTypeEditor : MappingInfoTypeEditor<Appointment> {
		protected internal override string TransactionDescription { get { return "Modify Appointment Mappings"; } }
		protected internal override SetupBaseMappingsWizardRunner<Appointment> CreateWizardRunner(IDesignerHost host, IPersistentObjectStorage<Appointment> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new SetupAppointmentBaseMappingsWizardRunner(host, objectStorage, dataFieldsProvider);
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EnsureInnerObjectUndoRedoSupport(typeof(AppointmentMappingInfo));
		}
	}
	#endregion
	#region ResourceMappingInfoTypeEditor
	public class ResourceMappingInfoTypeEditor : MappingInfoTypeEditor<Resource> {
		protected internal override string TransactionDescription { get { return "Modify Resource Mappings"; } }
		protected internal override SetupBaseMappingsWizardRunner<Resource> CreateWizardRunner(IDesignerHost host, IPersistentObjectStorage<Resource> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new SetupResourceBaseMappingsWizardRunner(host, objectStorage, dataFieldsProvider);
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EnsureInnerObjectUndoRedoSupport(typeof(ResourceMappingInfo));
		}
	}
	#endregion
	#region AppointmentDependencyMappingInfoTypeEditor
	public class AppointmentDependencyMappingInfoTypeEditor : MappingInfoTypeEditor<AppointmentDependency> {
		protected internal override string TransactionDescription { get { return "Modify Appointment Dependency Mappings"; } }
		protected internal override SetupBaseMappingsWizardRunner<AppointmentDependency> CreateWizardRunner(IDesignerHost host, IPersistentObjectStorage<AppointmentDependency> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new SetupAppointmentDependencyBaseMappingsWizardRunner(host, objectStorage, dataFieldsProvider);
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EnsureInnerObjectUndoRedoSupport(typeof(AppointmentDependencyMappingInfo));
		}
	}
	#endregion
}
