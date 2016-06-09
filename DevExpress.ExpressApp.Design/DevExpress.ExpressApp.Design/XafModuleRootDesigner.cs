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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Utils.About;
namespace DevExpress.ExpressApp.Design {
	[ToolboxItemFilter("Xaf", ToolboxItemFilterType.Require)]
	[ToolboxItemFilter("", ToolboxItemFilterType.Custom)]
	public class XafModuleRootDesigner : XafRootDesignerBase {
		private ModuleBase compiledModule;
		private String description;
		private DesignerVerb useExportedType;
		private Type GetRootComponentType() {
			Type rootType = DesignerHost.GetType(DesignerHost.RootComponentClassName);
			String warningMessage = String.Format("The '{0}' type was not found. Be sure project was built.", DesignerHost.RootComponentClassName);
			if(rootType == null) {
				AddWarning(warningMessage);
				throw new ArgumentException(warningMessage);
			}
			else {
				RemoveWarning(warningMessage);
			}
			return rootType;
		}
		private void SelectionService_SelectionChanged(object sender, EventArgs e) {
			UpdateUseExportedTypeVerbProperties();
		}
		private void UpdateUseExportedTypeVerbProperties() {
			if((SelectionService.PrimarySelection is XPClassProperiesProvider) && ((XPClassProperiesProvider)SelectionService.PrimarySelection).CanBeUsed) {
				ITypeInfo typeInfo = ((XPClassProperiesProvider)SelectionService.PrimarySelection).ClassInfo;
				useExportedType.Properties["ExportedType"] = typeInfo;
				useExportedType.Properties["Assembly"] = null;
				useExportedType.Checked = ExportedTypeHelper.IsAdditionalExportedType(typeInfo.Type, CompiledModule);
				useExportedType.Description = String.Format("Use {0}", typeInfo.Type.Name);
				useExportedType.Properties["Text"] = "Use Type in Application";
			}
			else if((SelectionService.PrimarySelection is AssemblyPropertiesProvider) && ((AssemblyPropertiesProvider)SelectionService.PrimarySelection).CanBeUsed) {
				Assembly assembly = ((AssemblyPropertiesProvider)SelectionService.PrimarySelection).Assembly;
				useExportedType.Properties["Assembly"] = assembly;
				useExportedType.Properties["ExportedType"] = null;
				useExportedType.Checked = ExportedTypeHelper.IsRegisteredAssembly(assembly, CompiledModule.IsExportedType, ExportedTypeHelper.GetExportedTypes(CompiledModule));
				useExportedType.Description = String.Format("Use {0}", assembly.GetName().Name);
				useExportedType.Properties["Text"] = "Use Assembly in Application";
			}
			else {
				useExportedType.Properties["Assembly"] = null;
				useExportedType.Properties["ExportedType"] = null;
				useExportedType.Visible = false;
				return;
			}
			useExportedType.Visible = true;
			if(!MenuCommandService.Verbs.Contains(useExportedType)) {
				MenuCommandService.AddVerb(useExportedType);
			}
		}
		private void DesignerVerb_useExportedType(object sender, EventArgs e) {
			DesignerVerb verb = (DesignerVerb)sender;
			if(verb.Properties["Assembly"] != null) {
				Assembly assembly = verb.Properties["Assembly"] as Assembly;
				if(assembly != null) {
					SwitchAssemblyUsage(assembly);
				}
			}
			else if(verb.Properties["ExportedType"] != null) {
				ITypeInfo classInfo = verb.Properties["ExportedType"] as ITypeInfo;
				if(classInfo != null) {
					SwitchExportedTypeUsage(classInfo.Type);
				}
			}
		}
		private Boolean IsDeclaredTypeOrRequired(Type type) {
			List<Type> declaredTypes = ExportedTypeHelper.GetExportedTypes(CompiledModule);
			if(!declaredTypes.Contains(type)) {
				return false;
			}
			foreach(Type additionalType in ExportedTypeHelper.GetExportedTypes(Module)) {
				declaredTypes.Remove(additionalType);
			}
			if(declaredTypes.Contains(type)) {
				return true;
			}
			foreach(Type declaredType in declaredTypes) {
				if(ExportedTypeHelper.GetRequiredTypes(declaredType, CompiledModule.IsExportedType).Contains(type)) {
					return true;
				}
			}
			return false;
		}
		private IList<Type> GetDependentExportedTypes(Type type) {
			List<Type> result = new List<Type>();
			List<Type> exportedTypes = ExportedTypeHelper.GetExportedTypes(CompiledModule);
			foreach(Type dependentType in ExportedTypeHelper.GetDependentTypes(type, CompiledModule.IsExportedType)) {
				if(exportedTypes.Contains(dependentType)) {
					result.Add(dependentType);
				}
			}
			result.Remove(type);
			return result;
		}
		private IList<Type> GetRequiredExportedTypes(Type type) {
			return ExportedTypeHelper.GetRequiredTypes(type, CompiledModule.IsExportedType);
		}
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			properties.Remove("Name");
		}
		protected override Control CreateView() {
			return new XafModuleRootDesignerGUI();
		}
		protected override void InitializeView() {
			if(Component != null) {
				Type rootType = GetRootComponentType();
				compiledModule = (ModuleBase)ReflectionHelper.CreateObject(rootType);
				foreach(Type moduleType in Module.RequiredModuleTypes) {
					if(!compiledModule.RequiredModuleTypes.Contains(moduleType)) {
						foreach(Type type in compiledModule.RequiredModuleTypes) {
							if(type.FullName == moduleType.FullName) {
								compiledModule.RequiredModuleTypes.Remove(type);
								break;
							}
						}
						compiledModule.RequiredModuleTypes.Add(moduleType);
					}
				}
				foreach(Type type in Module.AdditionalExportedTypes) {
					compiledModule.AdditionalExportedTypes.Add(type);
				}
				((XafModuleRootDesignerGUI)view).Initialize(this);
				SelectionService.SelectionChanged += new EventHandler(SelectionService_SelectionChanged);
				useExportedType = new DesignerVerb("Use", new EventHandler(DesignerVerb_useExportedType));
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				try {
					SelectionService.SelectionChanged -= new EventHandler(SelectionService_SelectionChanged);
				}
				catch { }
			}
			base.Dispose(disposing);
		}
		public override void AddModule(ToolboxItem item) {
			base.AddModule(item);
			using(DesignerTransaction trans = DesignerHost.CreateTransaction("Creating " + item.DisplayName)) {
				Type itemType = GetToolboxItemType(item);
				if(!CompiledModule.RequiredModuleTypes.Contains(itemType)) {
					PropertyDescriptor property = TypeDescriptor.GetProperties(Component)["RequiredModuleTypes"];
					ComponentChangeService.OnComponentChanging(Component, property);
					Module.RequiredModuleTypes.Add(itemType);
					ComponentChangeService.OnComponentChanged(Component, property, null, null);
					CompiledModule.RequiredModuleTypes.Add(itemType);
					ReferenceContainingAssembly(itemType);
					trans.Commit();
					ToolboxService.SelectedToolboxItemUsed();
					Module.RequiredModuleTypes.RaiseChanged();
					CompiledModule.RequiredModuleTypes.RaiseChanged();
				}
			}
		}
		public void SwitchExportedTypeUsage(Type exportedType) {
			try {
				if(IsDeclaredTypeOrRequired(exportedType)) {
					string message = string.Format("'{0}' type or a type that depends on it is added in code.", exportedType.FullName);
					MessageBox.Show(message, "Unable to remove type", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				using(DesignerTransaction trans = DesignerHost.CreateTransaction(String.Format("Change usage of exported type {0}", exportedType.Name))) {
					PropertyDescriptor exportedTypesProperty = TypeDescriptor.GetProperties(Component)["AdditionalExportedTypes"];
					ComponentChangeService.OnComponentChanging(Component, exportedTypesProperty);
					IList<Type> exportedTypes = ExportedTypeHelper.GetExportedTypes(CompiledModule);
					if(exportedTypes.Contains(exportedType)) {
						IList<Type> typesToRemove = GetDependentExportedTypes(exportedType);
						if(typesToRemove.Count > 0) {
							String message = String.Format("You are trying to remove the \"{0}\" type from the used exported types list.\nThis type has the following dependent types:", exportedType.FullName);
							foreach(Type type in typesToRemove) {
								message += String.Format("\n\"{0}\"", type.FullName);
							}
							message += "\nThe dependent types will be removed as well. Are you sure want to remove all these types?";
							if(MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
								Module.AdditionalExportedTypes.Remove(exportedType);
								CompiledModule.AdditionalExportedTypes.Remove(exportedType);
								foreach(Type toRemove in typesToRemove) {
									Module.AdditionalExportedTypes.Remove(toRemove);
									CompiledModule.AdditionalExportedTypes.Remove(toRemove);
								}
							}
						}
						else {
							Module.AdditionalExportedTypes.Remove(exportedType);
							CompiledModule.AdditionalExportedTypes.Remove(exportedType);
						}
					}
					else {
						Module.AdditionalExportedTypes.Add(exportedType);
						CompiledModule.AdditionalExportedTypes.Add(exportedType);
						foreach(Type newExportedType in GetRequiredExportedTypes(exportedType)) {
							Module.AdditionalExportedTypes.Add(newExportedType);
							CompiledModule.AdditionalExportedTypes.Add(newExportedType);
						}
					}
					ComponentChangeService.OnComponentChanged(Component, exportedTypesProperty, null, null);
					trans.Commit();
				}
			}
			catch(Exception e) {
				TraceException(e);
				throw;
			}
			finally {
				CompiledModule.AdditionalExportedTypes.RaiseChanged();
				UpdateUseExportedTypeVerbProperties();
			}
		}
		public void SwitchAssemblyUsage(Assembly assembly) {
			try {
				using(DesignerTransaction trans = DesignerHost.CreateTransaction(String.Format("Change usage of exported types assembly {0}", assembly.GetName().Name))) {
					PropertyDescriptor exportedTypeProperty = TypeDescriptor.GetProperties(Component)["AdditionalExportedTypes"];
					ComponentChangeService.OnComponentChanging(Component, exportedTypeProperty);
					if(ExportedTypeHelper.IsRegisteredAssembly(assembly, Module.IsExportedType, ExportedTypeHelper.GetExportedTypes(Module))) {
						foreach(Type type in ExportedTypeHelper.CollectExportedTypesFromAssembly(assembly, CompiledModule.IsExportedType)) {
							Module.AdditionalExportedTypes.Remove(type);
							CompiledModule.AdditionalExportedTypes.Remove(type);
						}
					}
					else {
						foreach(Type type in ExportedTypeHelper.CollectExportedTypesFromAssembly(assembly, CompiledModule.IsExportedType)) {
							Module.AdditionalExportedTypes.Add(type);
							CompiledModule.AdditionalExportedTypes.Add(type);
						}
					}
					ComponentChangeService.OnComponentChanged(Component, exportedTypeProperty, null, null);
					trans.Commit();
				}
			}
			catch(Exception e) {
				TraceException(e);
				throw;
			}
			finally {
				CompiledModule.AdditionalExportedTypes.RaiseChanged();
				UpdateUseExportedTypeVerbProperties();
			}
		}
		[DisplayName("Description")]
		public String Description {
			get { return description; }
			set { description = value; }
		}
		public ModuleBase Module {
			get { return (ModuleBase)Component; }
		}
		public ModuleBase CompiledModule {
			get { return compiledModule; }
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
		}
	}
	public class ExportedTypeHelper {
		private static readonly ITypesInfo typesInfo = XafTypesInfo.Instance;
		public static List<Type> GetRequiredTypes(Type forType, Predicate<Type> isExportedType) {
			List<Type> result = new List<Type>();
			ITypeInfo info = typesInfo.FindTypeInfo(forType);
			if(info != null && isExportedType(info.Type)) {
				foreach(ITypeInfo requiredType in info.GetRequiredTypes(delegate(ITypeInfo infoToCheck) { return isExportedType(infoToCheck.Type); })) {
					result.Add(requiredType.Type);
				}
			}
			return result;
		}
		public static List<Type> GetDependentTypes(Type forType, Predicate<Type> isExportedType) {
			List<Type> result = new List<Type>();
			ITypeInfo info = typesInfo.FindTypeInfo(forType);
			if(info != null && isExportedType(info.Type)) {
				foreach(ITypeInfo requiredType in info.GetDependentTypes(delegate(ITypeInfo infoToCheck) { return isExportedType(infoToCheck.Type); })) {
					result.Add(requiredType.Type);
				}
			}
			return result;
		}
		public static List<Type> GetAdditionalExportedTypes(ModuleBase module) {
			return new List<Type>(module.AdditionalExportedTypes);
		}
		public static Boolean IsAdditionalExportedType(Type type, ModuleBase module) {
			return GetAdditionalExportedTypes(module).Contains(type);
		}
		public static List<Type> GetExportedTypes(ModuleBase module) {
			return new List<Type>(module.GetExportedTypes());
		}
		public static List<Type> CollectExportedTypesFromAssembly(Assembly assembly, Predicate<Type> isExportedType) {
			List<Type> result = new List<Type>();
			IAssemblyInfo assemblyInfo = typesInfo.FindAssemblyInfo(assembly);
			if(assemblyInfo != null) {
				if(!assemblyInfo.AllTypesLoaded) {
					assemblyInfo.LoadTypes();
				}
				foreach(ITypeInfo typeInfo in assemblyInfo.Types) {
					Type type = typeInfo.Type;
					if(isExportedType(type) ) {
						result.Add(type);
					}
				}
			}
			return result;
		}
		public static Boolean IsRegisteredAssembly(Assembly assembly, Predicate<Type> isExportedType, IList<Type> moduleExportedTypes) {
			foreach(Type type in CollectExportedTypesFromAssembly(assembly, isExportedType)) {
				if(!moduleExportedTypes.Contains(type)) {
					return false;
				}
			}
			return true;
		}
		public static Boolean IsRequiredAssembly(Assembly assembly, ModuleBase module) {
			List<Type> typesFromAssembly = CollectExportedTypesFromAssembly(assembly, module.IsExportedType);
			foreach(Type type in GetExportedTypes(module)) {
				if(typesFromAssembly.Contains(type)) {
					return true;
				}
			}
			return false;
		}
	}
	public class ControllersHelper {
		public static List<Controller> CollectControllersFromAssembly(Assembly assembly) {
			Type[] controllerTypes = ControllersManager.CollectControllerTypesFromAssembly(assembly);
			List<Controller> controllerList = new List<Controller>(controllerTypes.Length);
			foreach(Type controllerType in controllerTypes) {
				Controller controller = Controller.Create(controllerType);
				controllerList.Add(controller);
			}
			return controllerList;
		}
	}
}
