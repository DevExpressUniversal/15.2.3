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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Utils.Design;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using EnvDTE;
using VSLangProj;
using DevExpress.Web.ASPxScheduler.Design.Wizards;
using DevExpress.XtraScheduler.Design.Wizards;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Design;
using DevExpress.XtraScheduler.Localization;
using System.Reflection;
using DevExpress.Utils;
namespace DevExpress.Web.ASPxScheduler.Design {
	#region XtraSerializationTypeDescriptionProvider
	public class XtraSerializationTypeDescriptionProvider : TypeDescriptionProvider {
		TypeDescriptionProvider parent;
		public XtraSerializationTypeDescriptionProvider(TypeDescriptionProvider parent)
			: base(parent) {
			this.parent = parent;
		}
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
			ICustomTypeDescriptor parentTypeDescriptor = parent.GetTypeDescriptor(objectType, instance);
			return new XtraSerializationTypeDescriptor(parentTypeDescriptor, objectType, instance);
		}
		public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance) {
			return base.GetExtendedTypeDescriptor(instance);
		}
	}
	#endregion
	#region XtraSerializationTypeDescriptor
	public class XtraSerializationTypeDescriptor : CustomTypeDescriptor {
		readonly ICustomTypeDescriptor parentTypeDescriptor;
		readonly object instance;
		public XtraSerializationTypeDescriptor(ICustomTypeDescriptor parentTypeDescriptor, Type objectType, object instance)
			: base(parentTypeDescriptor) {
			this.parentTypeDescriptor = parentTypeDescriptor;
			this.instance = instance;
		}
		public override PropertyDescriptorCollection GetProperties() {
			return this.GetProperties(null);
		}
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
			PropertyDescriptorCollection properties = parentTypeDescriptor.GetProperties(attributes);
			if (IsSerializationRequest()) {
				TypeDescriptor.Refresh(instance);
				return PreparePropertiesForSerialization(properties);
			}
			else
				return properties;
		}
		protected internal static bool IsSerializationRequest() {
			string stack = String.Empty;
			try {
				stack = Environment.StackTrace;
			}
			catch (Exception ex) {
				stack = ex.StackTrace;
			}
			if (stack.Contains("ControlSerializer"))
				return true;
			else
				return false;
		}
		protected internal virtual PropertyDescriptorCollection PreparePropertiesForSerialization(PropertyDescriptorCollection properties) {
			WebSerializationPropertyDescriptorModifier serializationModifier = new WebSerializationPropertyDescriptorModifier();
			List<PropertyDescriptor> result = new List<PropertyDescriptor>(properties.Count);
			foreach (PropertyDescriptor pd in properties) {
				PropertyDescriptor descriptor = serializationModifier.Modify(pd);
				if (instance == null || descriptor.ShouldSerializeValue(instance))
					result.Add(descriptor);
				else {
					descriptor = serializationModifier.SuppressSerialization(descriptor);
					result.Add(descriptor);
				}
			}
			return new PropertyDescriptorCollection(result.ToArray());
		}
	}
	#endregion
	#region ASPxSchedulerDataSourceViewSchemaAccessor (abstract class)
	public abstract class ASPxSchedulerDataSourceViewSchemaAccessor : IDataSourceViewSchemaAccessor {
		readonly ASPxSchedulerDesigner designer;
		protected ASPxSchedulerDataSourceViewSchemaAccessor(ASPxSchedulerDesigner designer) {
			Guard.ArgumentNotNull(designer, "designer");
			this.designer = designer;
		}
		#region Properties
		protected internal ASPxScheduler SchedulerControl { get { return designer.SchedulerControl; } }
		protected internal abstract string DataSourceID { get; }
		protected internal abstract string DataSourceMember { get; }
		protected internal abstract object DataSource { get; }
		#endregion
		#region IDataSourceViewSchemaAccessor Members
		public object DataSourceViewSchema { get { return GetDataSourceViewSchema(); } set { } }
		#endregion
		protected internal virtual IDataSourceViewSchema GetDataSourceViewSchema() {
			IDesignerHost host = designer.DesignerHost;
			if (host == null)
				return null;
			IComponent dataSource = LookupDataSourceComponent();
			if (dataSource == null)
				return null;
			IDataSourceDesigner dataSourceDesigner = host.GetDesigner(dataSource) as IDataSourceDesigner;
			if (dataSourceDesigner == null)
				return null;
			DesignerDataSourceView view = dataSourceDesigner.GetView(DataSourceMember);
			if (view == null)
				return null;
			if (view.Schema == null) {
				if (dataSourceDesigner.CanRefreshSchema)
					dataSourceDesigner.RefreshSchema(true);
			}
			return view.Schema;
		}
		protected internal virtual IComponent LookupDataSourceComponent() {
			IDesignerHost host = designer.DesignerHost;
			if (host == null)
				return null;
			IComponent result = DataSource as IComponent;
			if (result == null)
				result = host.Container.Components[DataSourceID];
			return result;
		}
	}
	#endregion
	#region AppointmentDataSourceViewSchemaAccessor
	public class AppointmentDataSourceViewSchemaAccessor : ASPxSchedulerDataSourceViewSchemaAccessor {
		public AppointmentDataSourceViewSchemaAccessor(ASPxSchedulerDesigner designer)
			: base(designer) {
		}
		#region Properties
		protected internal override string DataSourceID { get { return SchedulerControl.AppointmentDataSourceID; } }
		protected internal override string DataSourceMember { get { return SchedulerControl.AppointmentDataMember; } }
		protected internal override object DataSource { get { return SchedulerControl.AppointmentDataSource; } }
		#endregion
	}
	#endregion
	#region ResourceDataSourceViewSchemaAccessor
	public class ResourceDataSourceViewSchemaAccessor : ASPxSchedulerDataSourceViewSchemaAccessor {
		public ResourceDataSourceViewSchemaAccessor(ASPxSchedulerDesigner designer)
			: base(designer) {
		}
		#region Properties
		protected internal override string DataSourceID { get { return SchedulerControl.ResourceDataSourceID; } }
		protected internal override string DataSourceMember { get { return SchedulerControl.ResourceDataMember; } }
		protected internal override object DataSource { get { return SchedulerControl.ResourceDataSource; } }
		#endregion
	}
	#endregion
	public static class ASPxSchedulerReferenceAssembliesNames {
		static readonly string[] names = new string[] { AssemblyInfo.SRAssemblyData,
				AssemblyInfo.SRAssemblySchedulerCore,
				AssemblyInfo.SRAssemblyWeb,
				AssemblyInfo.SRAssemblySchedulerWeb};
		public static string[] Names { get { return names; } }
	}
	#region ASPxSchedulerDesignSR
	public static class ASPxSchedulerDesignSR {
		public const string AppointmentDataSourceID = "AppointmentDataSourceID";
		public const string ResourceDataSourceID = "ResourceDataSourceID";
		public const string MasterControlID = "MasterControlID";
		public const string SetAppointmentDataSourceIDTransaction = "SetAppointmentDataSourceIDTransaction";
		public const string SetResourceDataSourceIDTransaction = "SetResourceDataSourceIDTransaction";
		public const string SetMasterControlIDTransaction = "SetMasterControlIDTransaction";
		public const string ConfigureAppointmentDataVerb = "Appointments Data Source:";
		public const string ConfigureResourceDataVerb = "Resources Data Source:";
		public const string CopyDefaultDialogFormsToTheProjectText = @"This action will extract default dialog forms from resources and copy them into DevExpress folder of the project. 
The attributes OptionForms and OptionsToolTips of the SchedulerControl will be updated to point to the local copies of the forms. 
Do you want to continue?";
	}
	#endregion
	#region ASPxSchedulerDesigner
	public class ASPxSchedulerDesigner : ASPxDataWebControlDesigner, IDesignerLoadCompleteNotifier {
		#region Fields
		public static FormsInfo FormsInfo = new SchedulerFormsInfo();
		public static FormsInfo ToolTipFormsInfo = new SchedulerToolTipFormsInfo();
		AppointmentDataSourceViewSchemaAccessor appointmentDataSourceViewSchemaAccessor;
		ResourceDataSourceViewSchemaAccessor resourceDataSourceViewSchemaAccessor;
		System.Windows.Forms.Control controlForInvoke;
		#endregion
		static ASPxSchedulerDesigner() {
			TypeDescriptor.AddProvider(new XtraSerializationTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(ASPxScheduler))), typeof(ASPxScheduler));
			TypeDescriptor.AddProvider(new XtraSerializationTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(SchedulerViewBase))), typeof(SchedulerViewBase));
			TypeDescriptor.AddProvider(new XtraSerializationTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(TimeRuler))), typeof(TimeRuler));
			TypeDescriptor.AddProvider(new XtraSerializationTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(AppointmentStorageBase))), typeof(AppointmentStorageBase));
			TypeDescriptor.AddProvider(new XtraSerializationTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(AppointmentStatusCollection))), typeof(AppointmentStatusCollection));
			TypeDescriptor.AddProvider(new XtraSerializationTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(AppointmentLabelCollection))), typeof(AppointmentLabelCollection));
			TypeDescriptor.AddProvider(new XtraSerializationTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(SchedulerColumnPadding))), typeof(SchedulerColumnPadding));
		}
		public ASPxSchedulerDesigner() {
			this.appointmentDataSourceViewSchemaAccessor = new AppointmentDataSourceViewSchemaAccessor(this);
			this.resourceDataSourceViewSchemaAccessor = new ResourceDataSourceViewSchemaAccessor(this);
		}
		#region Properties
		protected internal ASPxScheduler SchedulerControl { get { return Component as ASPxScheduler; } }
		protected new ASPxScheduler DataControl { get { return (ASPxScheduler)base.DataControl; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new ASPxAppointmentsDataActionList(this, DataSourceDesigner));
			list.Add(new ASPxResourcesDataActionList(this, DataSourceDesigner));
			base.RegisterActionLists(list);
		}
		internal AppointmentDataSourceViewSchemaAccessor AppointmentDataSourceViewSchemaAccessor { get { return appointmentDataSourceViewSchemaAccessor; } }
		internal ResourceDataSourceViewSchemaAccessor ResourceDataSourceViewSchemaAccessor { get { return resourceDataSourceViewSchemaAccessor; } }
		public string AppointmentDataSourceID {
			get { return DataControl.AppointmentDataSourceID; }
			set {
				if (value != AppointmentDataSourceID) {
					SetDataSourceID(ASPxSchedulerDesignSR.AppointmentDataSourceID, value);
				}
			}
		}
		public string ResourceDataSourceID {
			get { return DataControl.ResourceDataSourceID; }
			set {
				if (value != ResourceDataSourceID) {
					SetDataSourceID(ASPxSchedulerDesignSR.ResourceDataSourceID, value);
				}
			}
		}
		public override bool HasCopyDefaultDialogFormsToTheProject() {
			return true;
		}
		#endregion
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			EnsureReferences(ASPxSchedulerReferenceAssembliesNames.Names);				
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
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
			RegisterDataControlsTagPrefix();
			controlForInvoke = new System.Windows.Forms.Control();
			controlForInvoke.CreateControl();
			new ASPxAppointmentDataSourceTriggeredWizardRunner(SchedulerControl.Storage, DesignerHost, this, controlForInvoke);
			new ASPxResourceDataSourceTriggeredWizardRunner(SchedulerControl.Storage, DesignerHost, this, controlForInvoke);
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			PerformPrefilterProperty(properties, ASPxSchedulerDesignSR.AppointmentDataSourceID, typeof(DataSourceIDConverter));
			PerformPrefilterProperty(properties, ASPxSchedulerDesignSR.ResourceDataSourceID, typeof(DataSourceIDConverter));
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			TemplateGroup controlTemplateGroup = CreateTemplateGroup("General templates", SchedulerControl.Templates);
			templateGroups.Add(controlTemplateGroup);
			SchedulerViewRepository views = SchedulerControl.Views;
			int count = views.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewBase view = views[i];
				string groupName = view.Type.ToString() + "View templates";
				TemplateGroup viewGroup = CreateTemplateGroup(groupName, views[i].InnerTemplates);
				templateGroups.Add(viewGroup);
			}
			return templateGroups;
		}
		TemplateGroup CreateTemplateGroup(string groupName, SchedulerTemplates templates) {
			TemplateGroup templateGroup = new TemplateGroup(groupName);
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(templates);
			properties.Sort();
			int count = properties.Count;
			for (int i = 0; i < count; i++) {
				PropertyDescriptor property = properties[i];
				EditorBrowsableAttribute editorBrowsable = properties[i].Attributes[typeof(EditorBrowsableAttribute)] as EditorBrowsableAttribute;
				if (editorBrowsable != null && editorBrowsable.State == EditorBrowsableState.Never)
					continue;
				string template = property.Name;
				TemplateDefinition templateDefinition = new TemplateDefinition(this, template, templates, template);
				templateDefinition.SupportsDataBinding = true;
				templateGroup.AddTemplateDefinition(templateDefinition);
			}
			return templateGroup;
		}
		protected internal virtual void RegisterDataControlsTagPrefix() {
			TagPrefixHelper.RegisterTagPrefix(RootDesigner, typeof(TimeRuler));
		}
		public override void ShowAbout() {
			ASPxSchedulerAboutDialogHelper.ShowAbout(Component.Site);
		}
		protected override FormsInfo[] GetFormsInfoArray() {
			return new FormsInfo[] { FormsInfo, ToolTipFormsInfo };
		}
		protected override bool NeedConfirmDialog() {
			return true;
		}
		protected override Object GetControlSettingsForms(FormsInfo formsInfo) {
			if (formsInfo == FormsInfo)
				return SchedulerControl.OptionsForms;
			else if (formsInfo == ToolTipFormsInfo)
				return SchedulerControl.OptionsToolTips;
			return base.GetControlSettingsForms(formsInfo);
		}
		protected override void NotifyNewFilesAdded(string[] newFiles) {
			if (newFiles != null && newFiles.Length >= 0) {
				string targetFolder = CommonUtils.GetDefaultFormsFolder(Component.GetType());
				AddFormTemplatesWizardRunner runner = new AddFormTemplatesWizardRunner(DesignerHost, targetFolder, newFiles);
				runner.Run();
			}
		}
		protected override void OnDesignerLoadComplete(object sender, EventArgs e) {
			base.OnDesignerLoadComplete(sender, e);
			RaiseLoadComplete();
			IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (changeService != null) {
				changeService.ComponentChanging += new ComponentChangingEventHandler(OnChangeServiceComponentChanging);
				changeService.ComponentChanged += new ComponentChangedEventHandler(OnChangeServiceComponentChanged);
			}
		}
		protected internal virtual void OnChangeServiceComponentChanging(object sender, ComponentChangingEventArgs e) {
			if (e.Component == this.Component)
				RaiseTransactionOpened();
		}
		protected internal virtual void OnChangeServiceComponentChanged(object sender, ComponentChangedEventArgs e) {
			if (e.Component == this.Component)
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
		protected override bool NeedCopyFormsOnInitialize() {
			return false;
		}
		protected override FormsManager CreateFormsManager() {
			return new ASPxSchedulerFormsManager(GetFormsInfoArray(), NeedConfirmDialog(), DesignerHost);
		}
		public override bool HasCommonDesigner() {
			return false;
		}
	}
	public class SchedulerFormsInfo : FormsInfo {
		public override string ControlName { get { return "ASPxScheduler"; } }
		public override bool NeedCopyDesignerFileFromResource { get { return false; } }
		public override string[] FormNames { get { return ASPxScheduler.FormNames; } }
		public override Type Type { get { return typeof(ASPxScheduler); } }
	}
	public class SchedulerToolTipFormsInfo : FormsInfo {
		public override string ControlName { get { return "ASPxScheduler"; } }
		public override bool NeedCopyDesignerFileFromResource { get { return false; } }
		public override string[] FormNames { get { return ASPxScheduler.ToolTipsFormNames; } }
		public override Type Type { get { return typeof(ASPxScheduler); } }
	}
	#endregion
	#region ASPxObjectStorageDataActionList<T>
	public abstract class ASPxObjectStorageDataActionList<T> : DesignerActionList where T : IPersistentObject {
		#region Fields
		readonly ASPxSchedulerDesigner designer;
		readonly ASPxDataFieldsProvider dataFieldsProvider;
		#endregion
		protected ASPxObjectStorageDataActionList(ASPxSchedulerDesigner designer, IDataSourceDesigner dataSourceDesigner)
			: base(designer.Component) {
			Guard.ArgumentNotNull(designer, "designer");
			this.designer = designer;
			this.dataFieldsProvider = new ASPxDataFieldsProvider(SchemaAccessor);
		}
		#region Properties
		protected internal ASPxSchedulerDesigner Designer { get { return designer; } }
		public override bool AutoShow { get { return true; } set { } }
		protected internal ASPxScheduler SchedulerControl { get { return (ASPxScheduler)Designer.Component; } }
		protected internal ASPxDataFieldsProvider DataFieldsProvider { get { return dataFieldsProvider; } }
		protected internal abstract IPersistentObjectStorage<T> ObjectStorage { get; }
		protected internal abstract ASPxSchedulerDataSourceViewSchemaAccessor SchemaAccessor { get; }
		protected internal abstract string TransactionDescription { get; }
		#endregion
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			if (!DataFieldsProvider.UnboundMode) {
				string mappingsWizardCaption = SchedulerLocalizer.GetString(SchedulerStringId.Caption_MappingsWizard);
				string checkMappingsCaption = SchedulerLocalizer.GetString(SchedulerStringId.Caption_CheckMappings);
				;
				result.Add(new DesignerActionMethodItem(this, "RunMappingsWizard", mappingsWizardCaption, StringResources.DataControl_DataActionGroup, mappingsWizardCaption, false));
				result.Add(new DesignerActionMethodItem(this, "CheckMappings", checkMappingsCaption, StringResources.DataControl_DataActionGroup, checkMappingsCaption, false));
			}
			return result;
		}
		public virtual void CheckMappings() {
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (host == null)
				return;
			MappingsChecker<T> checker = CreateMappingsChecker();
			checker.CheckMappings(ObjectStorage, host, dataFieldsProvider);
		}
		public virtual void RunMappingsWizard() {
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (host == null)
				return;
			SetupObjectStorageWizardRunner<T> wizardRunner = CreateWizardRunner(host);
			using (IDisposable undoSupport = new EmptyUndoSupport()) {
				DesignTimeTransactionHelper.InvokeTransactedChange(host, SchedulerControl, RunWizardCore, wizardRunner, TransactionDescription, null);
			}
		}
		protected internal bool RunWizardCore(object context) {
			SetupObjectStorageWizardRunner<T> wizardRunner = (SetupObjectStorageWizardRunner<T>)context;
			return wizardRunner.Run() == DialogResult.OK;
		}
		protected internal abstract SetupObjectStorageWizardRunner<T> CreateWizardRunner(IDesignerHost host);
		protected internal abstract MappingsChecker<T> CreateMappingsChecker();
	}
	#endregion
	#region ASPxAppointmentsDataActionList
	public class ASPxAppointmentsDataActionList : ASPxObjectStorageDataActionList<Appointment> {
		public ASPxAppointmentsDataActionList(ASPxSchedulerDesigner designer, IDataSourceDesigner dataSourceDesigner)
			: base(designer, dataSourceDesigner) {
		}
		#region Properties
		#region AppointmentDataSourceID
		[TypeConverter(typeof(DataSourceIDConverter))]
		public string AppointmentDataSourceID {
			get {
				if (string.IsNullOrEmpty(Designer.AppointmentDataSourceID))
					return SystemDesignSRHelper.GetDataControlNoDataSource();
				return Designer.AppointmentDataSourceID;
			}
			set {
				ControlDesigner.InvokeTransactedChange(SchedulerControl, new TransactedChangeCallback(SetAppointmentDataSourceIDCallback), value, ASPxSchedulerDesignSR.SetAppointmentDataSourceIDTransaction);
			}
		}
		#endregion
		protected internal override IPersistentObjectStorage<Appointment> ObjectStorage { get { return SchedulerControl.Storage.Appointments; } }
		protected internal override ASPxSchedulerDataSourceViewSchemaAccessor SchemaAccessor { get { return Designer.AppointmentDataSourceViewSchemaAccessor; } }
		protected internal override string TransactionDescription { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_ModifyAppointmentStorageTransactionDescription); } }
		#endregion
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = base.GetSortedActionItems();
			DesignerActionItem item = DesignUtils.CreateDesignerPropertyItem(Designer, ASPxSchedulerDesignSR.AppointmentDataSourceID, SchedulerControl, ASPxSchedulerDesignSR.ConfigureAppointmentDataVerb);
			if (item != null)
				result.Insert(0, item);
			return result;
		}
		private bool SetAppointmentDataSourceIDCallback(object context) {
			PropertyDescriptor descriptor = TypeDescriptor.GetProperties(SchedulerControl)[ASPxSchedulerDesignSR.AppointmentDataSourceID];
			descriptor.SetValue(SchedulerControl, context);
			return true;
		}
		protected internal override SetupObjectStorageWizardRunner<Appointment> CreateWizardRunner(IDesignerHost host) {
			return new ASPxSetupAppointmentStorageWizardRunner(host, ObjectStorage, DataFieldsProvider);
		}
		protected internal override MappingsChecker<Appointment> CreateMappingsChecker() {
			return new AppointmentMappingsChecker();
		}
	}
	#endregion
	#region ASPxResourcesDataActionList
	public class ASPxResourcesDataActionList : ASPxObjectStorageDataActionList<Resource> {
		public ASPxResourcesDataActionList(ASPxSchedulerDesigner designer, IDataSourceDesigner dataSourceDesigner)
			: base(designer, dataSourceDesigner) {
		}
		#region Properties
		#region ResourceDataSourceID
		[TypeConverter(typeof(DataSourceIDConverter))]
		public string ResourceDataSourceID {
			get {
				if (string.IsNullOrEmpty(Designer.ResourceDataSourceID))
					return SystemDesignSRHelper.GetDataControlNoDataSource();
				return Designer.ResourceDataSourceID;
			}
			set {
				ControlDesigner.InvokeTransactedChange(SchedulerControl, new TransactedChangeCallback(SetResourceDataSourceIDCallback), value, ASPxSchedulerDesignSR.SetResourceDataSourceIDTransaction);
			}
		}
		#endregion
		protected internal override IPersistentObjectStorage<Resource> ObjectStorage { get { return SchedulerControl.Storage.Resources; } }
		protected internal override ASPxSchedulerDataSourceViewSchemaAccessor SchemaAccessor { get { return Designer.ResourceDataSourceViewSchemaAccessor; } }
		protected internal override string TransactionDescription { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_ModifyResourceStorageTransactionDescription); } }
		#endregion
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = base.GetSortedActionItems();
			DesignerActionItem item = DesignUtils.CreateDesignerPropertyItem(Designer, ASPxSchedulerDesignSR.ResourceDataSourceID, SchedulerControl, ASPxSchedulerDesignSR.ConfigureResourceDataVerb);
			if (item != null)
				result.Insert(0, item);
			return result;
		}
		private bool SetResourceDataSourceIDCallback(object context) {
			PropertyDescriptor descriptor = TypeDescriptor.GetProperties(SchedulerControl)[ASPxSchedulerDesignSR.ResourceDataSourceID];
			descriptor.SetValue(SchedulerControl, context);
			return true;
		}
		protected internal override SetupObjectStorageWizardRunner<Resource> CreateWizardRunner(IDesignerHost host) {
			return new ASPxSetupResourceStorageWizardRunner(host, ObjectStorage, DataFieldsProvider);
		}
		protected internal override MappingsChecker<Resource> CreateMappingsChecker() {
			return new ResourceMappingsChecker();
		}
	}
	#endregion
	public class ASPxSchedulerFormsManager : FormsManager {
		public ASPxSchedulerFormsManager(FormsInfo[] formsInfoArray, bool needConfirmDialog, IServiceProvider provider)
			: base(formsInfoArray, needConfirmDialog, provider) {
		}
		protected override string GetCopyDefaultDialogFormsToTheProjectTextTemplate() {
			return ASPxSchedulerDesignSR.CopyDefaultDialogFormsToTheProjectText;
		}
	}
}
