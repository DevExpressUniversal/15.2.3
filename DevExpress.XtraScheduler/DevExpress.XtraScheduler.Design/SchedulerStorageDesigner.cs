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
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraScheduler.Design.Wizards;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Design {
	#region SchedulerStorageDesigner
	public class SchedulerStorageDesigner : BaseComponentDesigner, IServiceProvider, IDesignerLoadCompleteNotifier {
		#region Fields
		readonly DesignerVerbCollection verbs;
		Control controlForInvoke;
		#endregion
		public SchedulerStorageDesigner() {
			SchedulerRepositoryItemsRegistrator.Register();
			this.verbs = new DesignerVerbCollection(new DesignerVerb[] { new DesignerVerb("About...", OnAboutClick) });
		}
		#region Properties
		public SchedulerStorage Storage { get { return Component as SchedulerStorage; } }
		protected override bool AllowHookDebugMode { get { return true; } }
		public override DesignerVerbCollection DXVerbs { get { return verbs; } }
		#endregion
		void OnAboutClick(object sender, EventArgs e) {
			SchedulerControl.About();
		}
		object IServiceProvider.GetService(Type type) {
			return base.GetService(type);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new SchedulerStorageAppointmentsActionList(this.Component));
			list.Add(new SchedulerStorageResourcesActionList(this.Component));
			list.Add(new SchedulerStorageAppointmentDependenciesActionList(this.Component));
			base.RegisterActionLists(list);
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(SchedulerControl.About));
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			controlForInvoke = new Control();
			controlForInvoke.CreateControl();
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (host != null) {
				host.LoadComplete += new EventHandler(OnHostLoadComplete);
				new AppointmentDataSourceTriggeredWizardRunner(Storage, host, this, controlForInvoke); 
				new ResourceDataSourceTriggeredWizardRunner(Storage, host, this, controlForInvoke); 
				new AppointmentDependencyDataSourceTriggeredWizardRunner(Storage, host, this, controlForInvoke); 
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					controlForInvoke.Dispose();
					controlForInvoke = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal virtual void OnHostLoadComplete(object sender, EventArgs e) {
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (host != null) {
				host.TransactionOpened += new EventHandler(OnHostTransactionOpened);
				host.TransactionClosed += new DesignerTransactionCloseEventHandler(OnHostTransactionClosed);
			}
			RaiseLoadComplete();
		}
		protected internal virtual void OnHostTransactionOpened(object sender, EventArgs e) {
			RaiseTransactionOpened();
		}
		protected internal virtual void OnHostTransactionClosed(object sender, DesignerTransactionCloseEventArgs e) {
			RaiseTransactionClosed();
		}
		#region IDesignerLoadCompleteNotifier Members
		#region LoadComplete
		EventHandler onLoadComplete;
		event EventHandler IDesignerLoadCompleteNotifier.LoadComplete { add { onLoadComplete += value; } remove { onLoadComplete -= value; } }
		protected internal virtual void RaiseLoadComplete() {
			if (onLoadComplete != null)
				onLoadComplete(this, EventArgs.Empty);
		}
		#endregion
		#region TransactionOpened
		EventHandler onTransactionOpened;
		event EventHandler IDesignerLoadCompleteNotifier.TransactionOpened { add { onTransactionOpened += value; } remove { onTransactionOpened -= value; } }
		protected internal virtual void RaiseTransactionOpened() {
			if (onTransactionOpened != null)
				onTransactionOpened(this, EventArgs.Empty);
		}
		#endregion
		#region TransactionClosed
		EventHandler onTransactionClosed;
		event EventHandler IDesignerLoadCompleteNotifier.TransactionClosed { add { onTransactionClosed += value; } remove { onTransactionClosed -= value; } }
		protected internal virtual void RaiseTransactionClosed() {
			if (onTransactionClosed != null)
				onTransactionClosed(this, EventArgs.Empty);
		}
		#endregion
		#endregion
	}
	#endregion
	#region SchedulerStorageActionList<T> (abstract class)
	public abstract class SchedulerStorageActionList<T> : DesignerActionList where T : IPersistentObject {
		readonly string prefix;
		protected SchedulerStorageActionList(IComponent component, string prefix)
			: base(component) {
			this.prefix = prefix;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			if (ObjectStorage != null) {
				result.Add(new DesignerActionPropertyItem("DataSource", prefix + " Data Source"));
				if (!ObjectStorage.UnboundMode) {
					result.Add(new DesignerActionMethodItem(this, "RunMappingsWizard", SchedulerLocalizer.GetString(SchedulerStringId.Caption_MappingsWizard), false));
					result.Add(new DesignerActionMethodItem(this, "CheckMappings", SchedulerLocalizer.GetString(SchedulerStringId.Caption_CheckMappings), false));
				}
			}
			return result;
		}
		[AttributeProvider(typeof(IListSource))]
		public object DataSource {
			get { return ObjectStorage.DataSource; }
			set {
				using (ComponentChangeHelper helper = new ComponentChangeHelper((Component)Storage, "Setting DataSource")) {
					ObjectStorage.DataSource = value;
				}
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		[Editor(ControlConstants.DataMemberEditor, typeof(System.Drawing.Design.UITypeEditor))]
		public string DataMember {
			get { return ObjectStorage.DataMember; }
			set {
				using (ComponentChangeHelper helper = new ComponentChangeHelper((Component)Storage, "Setting DataMember")) {
					ObjectStorage.DataMember = value;
				}
			}
		}
		public virtual void CheckMappings() {
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (host == null)
				return;
			DefaultDataFieldsProvider<T> dataFieldsProvider = new DefaultDataFieldsProvider<T>(ObjectStorage);
			MappingsChecker<T> checker = CreateMappingsChecker();
			checker.CheckMappings(ObjectStorage, host, dataFieldsProvider);
		}
		public virtual void RunMappingsWizard() {
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (host == null)
				return;
			SetupObjectStorageWizardRunner<T> wizardRunner = CreateWizardRunner(host, ObjectStorage, new DefaultDataFieldsProvider<T>(ObjectStorage));
			using (IDisposable undoSupport = CreateUndoSupportObject()) {
				DesignTimeTransactionHelper.InvokeTransactedChange(host, (Component)Storage, RunWizard, wizardRunner, TransactionDescription, null);
			}
		}
		protected internal bool RunWizard(object context) {
			SetupObjectStorageWizardRunner<T> wizardRunner = (SetupObjectStorageWizardRunner<T>)context;
			return wizardRunner.Run() == DialogResult.OK;
		}
		protected internal abstract SetupObjectStorageWizardRunner<T> CreateWizardRunner(IDesignerHost host, IPersistentObjectStorage<T> objectStorage, IDataFieldsProvider dataFieldsProvider);
		protected abstract IPersistentObjectStorage<T> ObjectStorage { get; }
		protected abstract MappingsChecker<T> CreateMappingsChecker();
		protected abstract ISchedulerStorageBase Storage { get; }
		protected internal abstract string TransactionDescription { get; }
		protected internal abstract IDisposable CreateUndoSupportObject();
	}
	#endregion
	#region SchedulerStorageAppointmentsActionList
	public class SchedulerStorageAppointmentsActionList : SchedulerStorageActionList<Appointment> {
		public SchedulerStorageAppointmentsActionList(IComponent component)
			: base(component, "Appointments ") {
		}
		protected override IPersistentObjectStorage<Appointment> ObjectStorage { get { return ((SchedulerStorage)Component).Appointments; } }
		protected override ISchedulerStorageBase Storage { get { return (SchedulerStorage)Component; } }
		protected internal override string TransactionDescription { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_ModifyAppointmentStorageTransactionDescription); } }
		protected internal override SetupObjectStorageWizardRunner<Appointment> CreateWizardRunner(IDesignerHost host, IPersistentObjectStorage<Appointment> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new SetupAppointmentStorageWizardRunner(host, objectStorage, dataFieldsProvider);
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EnsureInnerObjectUndoRedoSupport(typeof(AppointmentMappingInfo));
		}
		protected override MappingsChecker<Appointment> CreateMappingsChecker() {
			return new AppointmentMappingsChecker();
		}
	}
	#endregion
	#region SchedulerStorageResourcesActionList
	public class SchedulerStorageResourcesActionList : SchedulerStorageActionList<Resource> {
		public SchedulerStorageResourcesActionList(IComponent component)
			: base(component, "Resources ") {
		}
		protected override IPersistentObjectStorage<Resource> ObjectStorage { get { return ((SchedulerStorage)Component).Resources; } }
		protected override ISchedulerStorageBase Storage { get { return (SchedulerStorage)Component; } }
		protected internal override string TransactionDescription { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_ModifyResourceStorageTransactionDescription); } }
		protected internal override SetupObjectStorageWizardRunner<Resource> CreateWizardRunner(IDesignerHost host, IPersistentObjectStorage<Resource> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new SetupResourceStorageWizardRunner(host, objectStorage, dataFieldsProvider);
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EnsureInnerObjectUndoRedoSupport(typeof(ResourceMappingInfo));
		}
		protected override MappingsChecker<Resource> CreateMappingsChecker() {
			return new ResourceMappingsChecker();
		}
	}
	#endregion
	#region SchedulerStorageAppointmentDependenciesActionList
	public class SchedulerStorageAppointmentDependenciesActionList : SchedulerStorageActionList<AppointmentDependency> {
		public SchedulerStorageAppointmentDependenciesActionList(IComponent component)
			: base(component, "AppointmentDependencies ") {
		}
		protected override IPersistentObjectStorage<AppointmentDependency> ObjectStorage { get { return ((SchedulerStorage)Component).AppointmentDependencies; } }
		protected override ISchedulerStorageBase Storage { get { return (SchedulerStorage)Component; } }
		protected internal override string TransactionDescription { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_ModifyAppointmentDependencyStorageTransactionDescription); } }
		protected internal override SetupObjectStorageWizardRunner<AppointmentDependency> CreateWizardRunner(IDesignerHost host, IPersistentObjectStorage<AppointmentDependency> objectStorage, IDataFieldsProvider dataFieldsProvider) {
			return new SetupAppointmentDependencyStorageWizardRunner(host, objectStorage, dataFieldsProvider);
		}
		protected internal override IDisposable CreateUndoSupportObject() {
			return new EnsureInnerObjectUndoRedoSupport(typeof(AppointmentDependencyMappingInfo));
		}
		protected override MappingsChecker<AppointmentDependency> CreateMappingsChecker() {
			return new AppointmentDependencyMappingsChecker();
		}
	}
	#endregion
	#region MappingsChecker<T> (abstact, helper class)
	public abstract class MappingsChecker<T> where T : IPersistentObject {
		public void CheckMappings(IPersistentObjectStorage<T> objectStorage, IDesignerHost host, IDataFieldsProvider dataFieldsProvider) {
			SetupMappingsWizard<T> baseMappingsWizard = new SetupMappingsWizard<T>(host, objectStorage, dataFieldsProvider);
			SetupCustomFieldMappingsWizard<T> customMappingsWizard = CreateCustomMappingsWizard(objectStorage, host, dataFieldsProvider);
			customMappingsWizard.Initialize();
			StringBuilder sb = new StringBuilder();
			MappingCollection mappings = new MappingCollection();
			((IInternalPersistentObjectStorage<T>)objectStorage).AppendMappings(mappings);
			mappings.AddRange(objectStorage.CustomFieldMappings);
			bool result1 = baseMappingsWizard.ValidateMappingsCore(sb, mappings);
			bool result2 = customMappingsWizard.ValidateMappings(sb);
			string caption = SchedulerLocalizer.GetString(SchedulerStringId.Caption_MappingsValidation);
			if (!result1 || !result2)
				XtraMessageBox.Show(sb.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
			else
				XtraMessageBox.Show(SchedulerLocalizer.GetString(SchedulerStringId.Msg_MappingsCheckPassedOk), caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		protected internal abstract SetupCustomFieldMappingsWizard<T> CreateCustomMappingsWizard(IPersistentObjectStorage<T> objectStorage, IDesignerHost host, IDataFieldsProvider dataFieldsProvider);
	}
	#endregion
	#region AppointmentMappingsChecker
	public class AppointmentMappingsChecker : MappingsChecker<Appointment> {
		protected internal override SetupCustomFieldMappingsWizard<Appointment> CreateCustomMappingsWizard(IPersistentObjectStorage<Appointment> objectStorage, IDesignerHost host, IDataFieldsProvider dataFieldsProvider) {
			return new SetupAppointmentCustomFieldMappingsWizard(host, objectStorage, dataFieldsProvider);
		}
	}
	#endregion
	#region ResourceMappingsChecker
	public class ResourceMappingsChecker : MappingsChecker<Resource> {
		protected internal override SetupCustomFieldMappingsWizard<Resource> CreateCustomMappingsWizard(IPersistentObjectStorage<Resource> objectStorage, IDesignerHost host, IDataFieldsProvider dataFieldsProvider) {
			return new SetupResourceCustomFieldMappingsWizard(host, objectStorage, dataFieldsProvider);
		}
	}
	#endregion
	#region AppointmentDependencyMappingsChecker
	public class AppointmentDependencyMappingsChecker : MappingsChecker<AppointmentDependency> {
		protected internal override SetupCustomFieldMappingsWizard<AppointmentDependency> CreateCustomMappingsWizard(IPersistentObjectStorage<AppointmentDependency> objectStorage, IDesignerHost host, IDataFieldsProvider dataFieldsProvider) {
			return new SetupAppointmentDependencyCustomFieldMappingsWizard(host, objectStorage, dataFieldsProvider);
		}
	}
	#endregion
}
