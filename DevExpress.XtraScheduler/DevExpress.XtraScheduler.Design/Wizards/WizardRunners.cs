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
using System.Windows.Forms;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Design.Wizards {
	#region SetupBaseMappingsWizardRunner<T> (abstract class)
	public abstract class SetupBaseMappingsWizardRunner<T> where T : IPersistentObject {
		readonly SetupMappingsWizard<T> setupMappingsWizard;
		protected SetupBaseMappingsWizardRunner(IDesignerHost host, IPersistentObjectStorage<T> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			this.setupMappingsWizard = new SetupMappingsWizard<T>(host, objectStorage, dataFieldsProvider);
			SetupMappingsWizardExtension(this.setupMappingsWizard);
		}
		protected virtual void SetupMappingsWizardExtension(SetupMappingsWizard<T> setupMappingsWizard) {
		}
		protected internal abstract string FormCaption { get; }
		public DialogResult Run() {
			using (SchedulerWizardForm form = new SchedulerWizardForm()) {
				form.Text = FormCaption;
				WizPageSetupMappings<T> wzPageSetupMappings = new WizPageSetupMappings<T>(setupMappingsWizard);
				wzPageSetupMappings.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
				form.Controls.AddRange(new Control[] { wzPageSetupMappings });
				return form.ShowDialog();
			}
		}
	}
	#endregion
	#region SetupAppointmentBaseMappingsWizardRunner
	public class SetupAppointmentBaseMappingsWizardRunner : SetupBaseMappingsWizardRunner<Appointment> {
		public SetupAppointmentBaseMappingsWizardRunner(IDesignerHost host, IPersistentObjectStorage<Appointment> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		protected internal override string FormCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_SetupAppointmentMappings); } }
		protected override void SetupMappingsWizardExtension(SetupMappingsWizard<Appointment> setupMappingsWizard) {
			base.SetupMappingsWizardExtension(setupMappingsWizard);
			GanttViewMappingWarningRestrictionLogic restriction = new GanttViewMappingWarningRestrictionLogic();
			restriction.InitializeState(setupMappingsWizard.ObjectStorage);
			setupMappingsWizard.SetupExtension(restriction);
		}
	}
	#endregion
	#region SetupResourceBaseMappingsWizardRunner
	public class SetupResourceBaseMappingsWizardRunner : SetupBaseMappingsWizardRunner<Resource> {
		public SetupResourceBaseMappingsWizardRunner(IDesignerHost host, IPersistentObjectStorage<Resource> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		protected internal override string FormCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_SetupResourceMappings); } }
	}
	#endregion
	#region SetupAppointmentDependencyBaseMappingsWizardRunner
	public class SetupAppointmentDependencyBaseMappingsWizardRunner : SetupBaseMappingsWizardRunner<AppointmentDependency> {
		public SetupAppointmentDependencyBaseMappingsWizardRunner(IDesignerHost host, IPersistentObjectStorage<AppointmentDependency> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		protected internal override string FormCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_SetupDependencyMappings); } }
	}
	#endregion
	#region SetupObjectStorageWizardRunner<T> (abstract class)
	public abstract class SetupObjectStorageWizardRunner<T> where T : IPersistentObject {
		#region Fields
		SetupMappingsWizard<T> setupMappingsWizard;
		SetupCustomFieldMappingsWizard<T> setupCustomFieldMappingsWizard;
		#endregion
		protected SetupObjectStorageWizardRunner(IDesignerHost host, IPersistentObjectStorage<T> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			this.setupMappingsWizard = new SetupMappingsWizard<T>(host, objectStorage, dataFieldsProvider);
			this.setupCustomFieldMappingsWizard = CreateCustomFieldMappingsWizard(host, objectStorage, dataFieldsProvider);
			SetupMappingsWizardExtension(this.setupMappingsWizard);
		}
		#region Properties
		protected internal SetupMappingsWizard<T> SetupMappingsWizard { get { return setupMappingsWizard; } }
		protected internal SetupCustomFieldMappingsWizard<T> SetupCustomFieldMappingsWizard { get { return setupCustomFieldMappingsWizard; } }
		protected internal abstract string FormCaption { get; }
		#endregion
		public DialogResult Run() {
			using (SchedulerWizardForm form = new SchedulerWizardForm()) {
				form.Text = FormCaption;
				AppendWizardPages(form);
				return form.ShowDialog();
			}
		}
		protected virtual void SetupMappingsWizardExtension(SetupMappingsWizard<T> setupMappingsWizard) {
		}
		protected internal virtual void AppendWizardPages(SchedulerWizardForm form) {
			WizPageSetupMappings<T> wzPageSetupMappings = new WizPageSetupMappings<T>(SetupMappingsWizard);
			wzPageSetupMappings.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			WizPageSetupCustomMappings<T> wizPageSetupCustomMappings = new WizPageSetupCustomMappings<T>(SetupCustomFieldMappingsWizard);
			wizPageSetupCustomMappings.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			form.Controls.AddRange(new Control[] { wzPageSetupMappings, wizPageSetupCustomMappings });
		}
		protected internal abstract SetupCustomFieldMappingsWizard<T> CreateCustomFieldMappingsWizard(IDesignerHost host, IPersistentObjectStorage<T> objectStorage, IDataFieldsProvider dataFieldsProvider);
	}
	#endregion
	#region SetupAppointmentStorageWizardRunner
	public class SetupAppointmentStorageWizardRunner : SetupObjectStorageWizardRunner<Appointment> {
		public SetupAppointmentStorageWizardRunner(IDesignerHost host, IPersistentObjectStorage<Appointment> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		protected internal override string FormCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_SetupAppointmentStorage); } }
		protected internal override SetupCustomFieldMappingsWizard<Appointment> CreateCustomFieldMappingsWizard(IDesignerHost host, IPersistentObjectStorage<Appointment> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new SetupAppointmentCustomFieldMappingsWizard(host, objectStorage, dataFieldsProvider);
		}
		protected override void SetupMappingsWizardExtension(SetupMappingsWizard<Appointment> setupMappingsWizard) {
			base.SetupMappingsWizardExtension(setupMappingsWizard);
			GanttViewMappingWarningRestrictionLogic restriction = new GanttViewMappingWarningRestrictionLogic();
			restriction.InitializeState(setupMappingsWizard.ObjectStorage);
			setupMappingsWizard.SetupExtension(restriction);
		}
	}
	#endregion
	#region SetupResourceStorageWizardRunner
	public class SetupResourceStorageWizardRunner : SetupObjectStorageWizardRunner<Resource> {
		public SetupResourceStorageWizardRunner(IDesignerHost host, IPersistentObjectStorage<Resource> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		protected internal override string FormCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_SetupResourceStorage); } }
		protected internal override SetupCustomFieldMappingsWizard<Resource> CreateCustomFieldMappingsWizard(IDesignerHost host, IPersistentObjectStorage<Resource> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new SetupResourceCustomFieldMappingsWizard(host, objectStorage, dataFieldsProvider);
		}
	}
	#endregion
	#region SetupAppointmentDependencyStorageWizardRunner
	public class SetupAppointmentDependencyStorageWizardRunner : SetupObjectStorageWizardRunner<AppointmentDependency> {
		public SetupAppointmentDependencyStorageWizardRunner(IDesignerHost host, IPersistentObjectStorage<AppointmentDependency> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		protected internal override string FormCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_SetupAppointmentDependencyStorage); } }
		protected internal override SetupCustomFieldMappingsWizard<AppointmentDependency> CreateCustomFieldMappingsWizard(IDesignerHost host, IPersistentObjectStorage<AppointmentDependency> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new SetupAppointmentDependencyCustomFieldMappingsWizard(host, objectStorage, dataFieldsProvider);
		}
	}
	#endregion
	#region IDesignerLoadCompleteNotifier
	public interface IDesignerLoadCompleteNotifier {
		event EventHandler LoadComplete;
		event EventHandler TransactionOpened;
		event EventHandler TransactionClosed;
	}
	#endregion
	#region DataSourceTriggeredWizardRunner<T> (abstract class)
	public abstract class DataSourceTriggeredWizardRunner<T> where T : IPersistentObject {
		#region Fields
		readonly ISchedulerStorageBase storage;
		readonly IDesignerHost host;
		readonly Control controlForInvoke;
		readonly IDataFieldsProvider dataFieldsProvider;
		readonly IDesignerLoadCompleteNotifier loadCompleteNotifier;
		#endregion
		protected DataSourceTriggeredWizardRunner(ISchedulerStorageBase storage, IDesignerHost host, IDesignerLoadCompleteNotifier loadCompleteNotifier, Control controlForInvoke) {
			Guard.ArgumentNotNull(storage, "storage");
			Guard.ArgumentNotNull(host, "host");
			Guard.ArgumentNotNull(loadCompleteNotifier, "loadCompleteNotifier");
			Guard.ArgumentNotNull(controlForInvoke, "controlForInvoke");
			this.storage = storage;
			this.host = host;
			this.controlForInvoke = controlForInvoke;
			this.dataFieldsProvider = CreateDataFieldsProvider();
			this.loadCompleteNotifier = loadCompleteNotifier;
			loadCompleteNotifier.LoadComplete += new EventHandler(OnDesignerLoadComplete);
		}
		#region Properties
		protected internal abstract IPersistentObjectStorage<T> ObjectStorage { get; }
		protected internal abstract string TransactionDescription { get; }
		protected internal ISchedulerStorageBase Storage { get { return storage; } }
		protected internal IDesignerHost DesignerHost { get { return host; } }
		#endregion
		protected internal virtual void OnDesignerLoadComplete(object sender, EventArgs e) {
			loadCompleteNotifier.TransactionOpened += new EventHandler(OnHostTransactionOpened);
			loadCompleteNotifier.TransactionClosed += new EventHandler(OnHostTransactionClosed);
		}
		bool unboundBeforeTransaction;
		bool unboundAfterTransaction;
		protected internal virtual void OnHostTransactionOpened(object sender, EventArgs e) {
			unboundBeforeTransaction = dataFieldsProvider.UnboundMode;
		}
		protected void OnHostTransactionClosed(object sender, EventArgs e) {
			unboundAfterTransaction = dataFieldsProvider.UnboundMode;
			if (ShouldRunWizard())
				controlForInvoke.BeginInvoke(new EventHandler(RunWizard), this, EventArgs.Empty);
			unboundBeforeTransaction = false;
			unboundAfterTransaction = false;
		}
		protected internal virtual bool ShouldRunWizard() {
			if (unboundBeforeTransaction && !unboundAfterTransaction) {
				MappingCollection mappings = new MappingCollection();
				((IInternalPersistentObjectStorage<T>)ObjectStorage).AppendMappings(mappings);
				mappings.AddRange(ObjectStorage.CustomFieldMappings);
				return mappings.Count <= 0;
			} else
				return false;
		}
		protected internal virtual IDataFieldsProvider CreateDataFieldsProvider() {
			return new DefaultDataFieldsProvider<T>(ObjectStorage);
		}
		protected internal virtual IComponent GetEditedComponent() {
			return (IComponent)ObjectStorage.Storage;
		}
		protected internal abstract IDisposable CreateUndoSupportObject();
		protected internal abstract SetupObjectStorageWizardRunner<T> CreateWizardRunner(IDataFieldsProvider dataFieldsProvider);
		protected internal virtual void RunWizard(object sender, EventArgs e) {
			SetupObjectStorageWizardRunner<T> wizardRunner = CreateWizardRunner(dataFieldsProvider);
			using (IDisposable undoSupport = CreateUndoSupportObject()) {
				DesignTimeTransactionHelper.InvokeTransactedChange(host, GetEditedComponent(), RunWizardCore, wizardRunner, TransactionDescription, null);
			}
		}
		protected internal bool RunWizardCore(object context) {
			SetupObjectStorageWizardRunner<T> wizardRunner = (SetupObjectStorageWizardRunner<T>)context;
			return wizardRunner.Run() == DialogResult.OK;
		}
	}
	#endregion
	#region AppointmentDataSourceTriggeredWizardRunner
	public class AppointmentDataSourceTriggeredWizardRunner : DataSourceTriggeredWizardRunner<Appointment> {
		public AppointmentDataSourceTriggeredWizardRunner(ISchedulerStorageBase storage, IDesignerHost host, IDesignerLoadCompleteNotifier loadCompleteNotifier, Control controlForInvoke)
			: base(storage, host, loadCompleteNotifier, controlForInvoke) {
		}
		protected internal override IPersistentObjectStorage<Appointment> ObjectStorage { get { return Storage.Appointments; } }
		protected internal override string TransactionDescription { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_ModifyAppointmentMappingsTransactionDescription); } }
		protected internal override SetupObjectStorageWizardRunner<Appointment> CreateWizardRunner(IDataFieldsProvider dataFieldsProvider) {
			return new SetupAppointmentStorageWizardRunner(DesignerHost, ObjectStorage, dataFieldsProvider);
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EnsureInnerObjectUndoRedoSupport(typeof(AppointmentMappingInfo));
		}
	}
	#endregion
	#region AppointmentDependencyDataSourceTriggeredWizardRunner
	public class AppointmentDependencyDataSourceTriggeredWizardRunner : DataSourceTriggeredWizardRunner<AppointmentDependency> {
		public AppointmentDependencyDataSourceTriggeredWizardRunner(ISchedulerStorageBase storage, IDesignerHost host, IDesignerLoadCompleteNotifier loadCompleteNotifier, Control controlForInvoke)
			: base(storage, host, loadCompleteNotifier, controlForInvoke) {
		}
		protected internal override IPersistentObjectStorage<AppointmentDependency> ObjectStorage { get { return Storage.AppointmentDependencies; } }
		protected internal override string TransactionDescription { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_ModifyAppointmentDependencyMappingsTransactionDescription); } }
		protected internal override SetupObjectStorageWizardRunner<AppointmentDependency> CreateWizardRunner(IDataFieldsProvider dataFieldsProvider) {
			return new SetupAppointmentDependencyStorageWizardRunner(DesignerHost, ObjectStorage, dataFieldsProvider);
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EnsureInnerObjectUndoRedoSupport(typeof(AppointmentDependencyMappingInfo));
		}
	}
	#endregion
	#region ResourceDataSourceTriggeredWizardRunner
	public class ResourceDataSourceTriggeredWizardRunner : DataSourceTriggeredWizardRunner<Resource> {
		public ResourceDataSourceTriggeredWizardRunner(ISchedulerStorageBase storage, IDesignerHost host, IDesignerLoadCompleteNotifier loadCompleteNotifier, Control controlForInvoke)
			: base(storage, host, loadCompleteNotifier, controlForInvoke) {
		}
		protected internal override IPersistentObjectStorage<Resource> ObjectStorage { get { return Storage.Resources; } }
		protected internal override string TransactionDescription { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_ModifyResourceMappingsTransactionDescription); } }
		protected internal override SetupObjectStorageWizardRunner<Resource> CreateWizardRunner(IDataFieldsProvider dataFieldsProvider) {
			return new SetupResourceStorageWizardRunner(DesignerHost, ObjectStorage, dataFieldsProvider);
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EnsureInnerObjectUndoRedoSupport(typeof(AppointmentMappingInfo));
		}
	}
	#endregion
}
