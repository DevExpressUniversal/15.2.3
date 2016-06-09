#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp {
	public class PropertyEditorDescriptorCollector {
		public IList<EditorDescriptor> Collect(Assembly assembly) {
			IList<EditorDescriptor> result = new List<EditorDescriptor>();
			foreach(Type type in ((TypesInfo)XafTypesInfo.Instance).GetAssemblyTypes(assembly)) {
				foreach(PropertyEditorAttribute attribute in AttributeHelper.GetAttributes<PropertyEditorAttribute>(type, false)) {
					result.Add(CreatePropertyEditorDescriptor(type, attribute));
				}
				foreach(ViewItemAttribute attribute in AttributeHelper.GetAttributes<ViewItemAttribute>(type, false)) {
					result.Add(CreateViewItemDescriptor(type, attribute));
				}
				foreach(ListEditorAttribute attribute in AttributeHelper.GetAttributes<ListEditorAttribute>(type, false)) {
					result.Add(CreateListEditorDescriptor(type, attribute));
				}
			}
			return result;
		}
		public PropertyEditorDescriptor CreatePropertyEditorDescriptor(Type propertyEditorType, PropertyEditorAttribute attribute) {
			return new PropertyEditorDescriptor(new AliasAndEditorTypeRegistration(attribute.alias, attribute.PropertyType, attribute.isDefaultAlias, propertyEditorType, attribute.IsDefaultEditor));
		}
		public ViewItemDescriptor CreateViewItemDescriptor(Type viewItemType, ViewItemAttribute attribute) {
			return new ViewItemDescriptor(new ViewItemRegistration(attribute.ModelNodeType, viewItemType, attribute.IsDefault));
		}
		public ListEditorDescriptor CreateListEditorDescriptor(Type listEditorType, ListEditorAttribute attribute) {
			return new ListEditorDescriptor(new AliasAndEditorTypeRegistration(listEditorType.Name, attribute.ListElementType, true, listEditorType, attribute.IsDefault));
		}
	}
	public interface ISupportSetup {
		void Setup(ApplicationModulesManager moduleManager);
	}
	[DXToolboxItem(true)]
	[ToolboxItemFilter("Xaf", ToolboxItemFilterType.Require)]
	[ToolboxBitmap(typeof(DevExpress.ExpressApp.ModuleBase), "Resources.Module.ico")]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Designer("DevExpress.ExpressApp.Design.XafModuleRootDesigner, DevExpress.ExpressApp.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IRootDesigner))]
	[Designer("DevExpress.ExpressApp.Design.XafModuleDesigner, DevExpress.ExpressApp.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IDesigner))]
	[DesignerSerializer("DevExpress.ExpressApp.Design.XafModuleSerializer, DevExpress.ExpressApp.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix,
		"System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	[DesignerCategory("Component")]
	public class ModuleBase : Component, ISupportSetup {
		public static IEnumerable<ModuleBase> EmptyModules = new ModuleBase[] { };
		private ModuleTypeList requiredModuleTypes;
		private String assemblyName;
		private ModelStoreBase diffsStore;
		private Version version;
		private String description = "";
		private String name;
		private Boolean isModuleBase;
		private List<Type> localizerTypes = new List<Type>();
		private ApplicationModulesManager moduleManager;
		private XafApplication application;
		private ExportedTypeCollection additionalExportedTypes;
		private ControllerTypeCollection additionalControllerTypes;
		private String modelDifferenceResourceName;
		protected override void Dispose(bool disposing) {
			application = null; 
			moduleManager = null; 
			base.Dispose(disposing);
		}
		protected internal virtual IEnumerable<Type> GetRegularTypes() {
			return AssemblyHelper.GetTypesFromAssembly(GetType().Assembly);
		}
		protected virtual ModuleTypeList GetRequiredModuleTypesCore() {
			ModuleTypeList result = new ModuleTypeList();
			if(GetType() != typeof(ModuleBase)) {
				result.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
			}
			return result;
		}
		protected virtual IEnumerable<Type> GetDeclaredExportedTypes() {
			if(isModuleBase) {
				return Type.EmptyTypes;
			}
			return ModuleHelper.CollectExportedTypesFromAssembly(GetType().Assembly, IsExportedType);
		}
		protected virtual IEnumerable<Type> GetDeclaredControllerTypes() {
			if(isModuleBase) {
				return Type.EmptyTypes;
			}
			return ModuleHelper.CollectControllerTypesFromAssembly(GetType().Assembly);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
			PropertyEditorDescriptorCollector propertyDescriptorCollector = new PropertyEditorDescriptorCollector();
			editorDescriptors.AddRange(propertyDescriptorCollector.Collect(GetType().Assembly));
		}
		public ModuleBase() {
			additionalExportedTypes = new ExportedTypeCollection();
			additionalControllerTypes = new ControllerTypeCollection();
			Type moduleType = GetType();
			isModuleBase = (moduleType == typeof(ModuleBase));
			if(isModuleBase) return;
			Assembly moduleAssembly = moduleType.Assembly;
			assemblyName = AssemblyHelper.GetName(moduleAssembly);
			version = AssemblyHelper.GetVersion(moduleAssembly);
			name = moduleType.Name;
			DescriptionAttribute[] attributes = AttributeHelper.GetAttributes<DescriptionAttribute>(moduleType, true);
			if(attributes.Length > 0) {
				Description = attributes[0].Description;
			}
		}
		public static Type GetRealModuleType(Type moduleType) {
			Guard.TypeArgumentIs(typeof(ModuleBase), moduleType, "moduleType");
			Type result = moduleType;
			if(TypeHelper.IsObsolete(result)) {
				foreach(Type type in ((TypesInfo)XafTypesInfo.Instance).GetAssemblyTypes(moduleType.Assembly)) {
					if(typeof(ModuleBase).IsAssignableFrom(type) && !TypeHelper.IsObsolete(type)) {
						result = type;
						break;
					}
				}
				if(result == moduleType) {
					ObsoleteAttribute obsolete = AttributeHelper.GetAttributes<ObsoleteAttribute>(moduleType, false)[0];
					throw new ArgumentException(obsolete.Message, moduleType.FullName);
				}
			}
			return result;
		}
		public static ModuleBase CreateModule(Type moduleType, ModelStoreBase diffsStore) {
			Type typeToCreate = GetRealModuleType(moduleType);
			ModuleBase result = (ModuleBase)TypeHelper.CreateInstance(typeToCreate);
			result.DiffsStore = diffsStore;
			return result;
		}
		public virtual void ExtendModelInterfaces(ModelInterfaceExtenders extenders) { }
		public virtual void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
			updaters.Add(new NamespacesLocalizationModelUpdater(GetExportedTypes()));
		}
		public virtual void AddModelNodeValidators(IModelNodeValidatorRegistrator validatorRegistrator) { 
		}
		public virtual void AddModelNodeUpdaters(IModelNodeUpdaterRegistrator updaterRegistrator) {
		}
		public virtual ICollection<Type> GetXafResourceLocalizerTypes() { return new List<Type>(); }
		public virtual void CustomizeTypesInfo(ITypesInfo typesInfo) { }
		public virtual void Setup(ApplicationModulesManager moduleManager) {
			ModuleManager = moduleManager;
		}
		public virtual void Setup(XafApplication application) {
			Application = application;
		}
		public virtual IList<PopupWindowShowAction> GetStartupActions() {
			return new List<PopupWindowShowAction>();
		}
		public virtual void CustomizeLogics(CustomLogics customLogics) { }
		public virtual Boolean IsExportedType(Type type) {
			return ExportedTypeHelpers.IsExportedType(type);
		}
		public virtual IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			List<ModuleUpdater> dbUpdaters = new List<ModuleUpdater>();
			Type[] updaterTypes = ((TypesInfo)XafTypesInfo.Instance).GetAssemblyTypes(GetType().Assembly, type => typeof(ModuleUpdater).IsAssignableFrom(type) && TypeHelper.CanCreateInstance(type, typeof(IObjectSpace), typeof(Version)));
			foreach(Type updaterType in updaterTypes) {
				dbUpdaters.Add((ModuleUpdater)TypeHelper.CreateInstance(updaterType, objectSpace, versionFromDB));
			}
			return dbUpdaters;
		}
		public IEnumerable<Type> GetExportedTypes() {
			ExportedTypeCollection exportedTypes = new ExportedTypeCollection();
			exportedTypes.AddRange(GetDeclaredExportedTypes());
			exportedTypes.AddRange(AdditionalExportedTypes);
			return ModuleHelper.CollectRequiredExportedTypes(exportedTypes, IsExportedType);
		}
		public IEnumerable<Type> GetControllerTypes() {
			ControllerTypeCollection controllerTypes = new ControllerTypeCollection();
			controllerTypes.AddRange(GetDeclaredControllerTypes());
			controllerTypes.AddRange(AdditionalControllerTypes);
			return controllerTypes;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModuleBaseModuleManager")]
#endif
		public ApplicationModulesManager ModuleManager {
			get { return moduleManager; }
			set { moduleManager = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModuleBaseApplication")]
#endif
		public XafApplication Application {
			get { return application; }
			set { application = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ModelStoreBase DiffsStore {
			get {
				if(diffsStore == null) {
					diffsStore = new ResourcesModelStore(GetType().Assembly, ModelDifferenceResourceName);
				}
				return diffsStore;
			}
			set { diffsStore = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModuleBaseName")]
#endif
		public virtual string Name {
			get { return name; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModuleBaseDescription")]
#endif
		public string Description {
			get { return description; }
			set { description = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ModuleBaseResourcesExportedToModel"),
#endif
 Editor("DevExpress.ExpressApp.Design.ResourceLocalizedTypesEditor, DevExpress.ExpressApp.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public List<Type> ResourcesExportedToModel {
			get { return localizerTypes; }
			set { localizerTypes = value; }
		}
		[Browsable(false)]
		public virtual Version Version {
			get { return version; }
		}
		[Browsable(false)]
		public string AssemblyName {
			get { return assemblyName; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ModuleTypeList RequiredModuleTypes {
			get {
				if(requiredModuleTypes == null) {
					requiredModuleTypes = GetRequiredModuleTypesCore();
				}
				return requiredModuleTypes;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ControllerTypeCollection AdditionalControllerTypes {
			get { return additionalControllerTypes; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ExportedTypeCollection AdditionalExportedTypes {
			get { return additionalExportedTypes; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public String ModelDifferenceResourceName {
			get { return modelDifferenceResourceName; }
			set { modelDifferenceResourceName = value; }
		}
	}
	public class TypeCollection : Collection<Type> {
		protected virtual void OnInsert(Type item) {
			if(item == null) {
				throw new ArgumentNullException("item");
			}
		}
		protected override void InsertItem(int index, Type item) {
			OnInsert(item);
			if(!Contains(item)) {
				base.InsertItem(index, item);
			}
		}
		protected override void SetItem(int index, Type item) {
			throw new NotSupportedException();
		}
		public void AddRange(IEnumerable<Type> collection) {
			foreach(Type type in collection) {
				Add(type);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseChanged() {
			if(Changed != null) {
				Changed(this, EventArgs.Empty);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler Changed;
	}
	public sealed class ExportedTypeCollection : TypeCollection { }
	public sealed class ControllerTypeCollection : TypeCollection {
		protected override void OnInsert(Type item) {
			base.OnInsert(item);
			Guard.TypeArgumentIs(typeof(Controller), item, "item");
		}
	}
	public class NamespacesLocalizationModelUpdater : ModelNodesGeneratorUpdater<ModelLocalizationNodesGenerator> {
		private const string NamespacesNodeName = "Namespaces";
		private readonly IEnumerable<Type> types;
		[Obsolete("Use 'NamespacesLocalizationModelUpdater(IEnumerable<Type> types)' instead.")]
		public NamespacesLocalizationModelUpdater(ModuleBase owner) {
			if(owner != null) {
				this.types = owner.GetExportedTypes();
			}
		}
		public NamespacesLocalizationModelUpdater(IEnumerable<Type> types) {
			this.types = types;
		}
		public override void UpdateNode(ModelNode node) {
			IModelLocalization localizationNode = (IModelLocalization)node;
			IModelLocalizationGroup namespacesGroup = localizationNode[NamespacesNodeName] as IModelLocalizationGroup ?? localizationNode.AddNode<IModelLocalizationGroup>(NamespacesNodeName);
			foreach(string namespaceName in GetNamespaces(types)) {
				if(namespacesGroup[namespaceName] == null) {
					IModelLocalizationItem namespaceItem = namespacesGroup.AddNode<IModelLocalizationItem>(namespaceName);
					namespaceItem.Value = GetNamespaceRepresentation(namespaceName);
				}
			}
		}
		private IEnumerable<string> GetNamespaces(IEnumerable<Type> types) {
			HashSet<string> namespaces = new HashSet<string>();
			if(types != null) {
				foreach(Type type in types) {
					namespaces.Add(type.Namespace);
				}
			}
			return namespaces;
		}
		private string GetNamespaceRepresentation(string namespaceName) {
			if(!string.IsNullOrEmpty(namespaceName)) {
				int lastDotPos = namespaceName.LastIndexOf('.');
				if(lastDotPos >= 0) {
					return namespaceName.Substring(lastDotPos + 1, namespaceName.Length - lastDotPos - 1);
				}
			}
			return namespaceName;
		}
	}
}
