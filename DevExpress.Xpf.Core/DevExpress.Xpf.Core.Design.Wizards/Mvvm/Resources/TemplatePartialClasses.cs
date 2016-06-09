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

using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
using System;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
using DevExpress.Mvvm.Native;
using System.Windows.Markup;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm {
	public interface IT4Template {
		void Write(string textToAppend);
		void WriteLine(string textToAppend);
		IDictionary<string, object> Session { get; set; }
		string TransformText();
	}
	public static class IT4TemplateExtensions {
		public static object ExpandMarkupExtension(this IT4Template template, MarkupExtension extension) {
			DevExpress.Xpf.Core.Native.ImageSourceHelper.RegisterPackScheme();
			return extension.ProvideValue(null);
		}
		public static void PasteUsingList(this IT4Template template, List<string> usingList, bool includeStandardUsings = true) {
			if(includeStandardUsings) {
				template.WriteLine("using System;");
				template.WriteLine("using System.Linq;");
				template.WriteLine("using System.Data;");
				template.WriteLine("using System.Linq.Expressions;");
				template.WriteLine("using System.Collections.Generic;");
			}
			if(usingList != null) {
				foreach(string item in usingList) {
					template.WriteLine(string.Format("using {0};", item));
				}
			}
		}
		public static void PasteAliasList(this IT4Template template, List<string> usingList, params string[] typeFullNames) {
			if(typeFullNames == null || typeFullNames.Length == 0)
				return;
			foreach(string name in typeFullNames) {
				if(IsAliasNeeded(usingList, name)) {
					string typeName = name.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last();
					template.WriteLine(string.Format("using {0} = {1};", typeName, name));
				}
			}
		}
		static bool IsAliasNeeded(List<string> usingList, string typeFullName) {
			if(string.IsNullOrEmpty(typeFullName))
				return false;
			try {
				string name = typeFullName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last();
				string[] predefinedNameSpaces = new string[] { "System", "Linq", "Data", "Entity", "Collections", "Generic", "ObjectModel" };
				if(predefinedNameSpaces.Contains(name))
					return true;
				if(usingList == null || usingList.Count == 0)
					return false;
				foreach(string namespaceName in usingList)
					if(namespaceName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).Contains(name))
						return true;
				return false;
			} catch {
				return false;
			}
		}
		public static string GetNamespace(this IT4Template template) {
			return template.GetTemplateInfo().Properties["Namespace"].ToString();
		}
		public static void PasteUsingList(this IT4Template template, params string[] additionalNamespaces) {
			List<string> usingList = template.GetTemplateInfo().UsingList;
			foreach(var item in additionalNamespaces) {
				if(!usingList.Contains(item))
					usingList.Add(item);
			}
			template.PasteUsingList(usingList);
		}
		public static T4TemplateInfo GetTemplateInfo(this IT4Template template) {
			return template.Session["T4TemplateInfo"] as T4TemplateInfo;
		}
		public static Func<T4TemplateInfo, bool> GetViewByNameFilter(string viewName) {
			return x => (string)x.GetProperty("ViewName") == viewName;
		}
		public static Func<T4TemplateInfo, bool> GetViewModelByNameFilter(string viewModelName) {
			return x => (string)x.GetProperty("ViewModelName") == viewModelName;
		}
		public static bool ExecuteDebugLogHook(this IT4Template template, string hookId) {
			return template.ExecuteHook(TemplatesCodeGen.STR_DebugLogHooks, hookId);
		}
		public static bool ExecuteEntityViewHook(this IT4Template template, string hookId) {
			return template.ExecuteHook(TemplatesCodeGen.STR_EntityViewHooks, hookId);
		}
		public static string ExecuteEntityViewModelHookAndReturnEmptyString(this IT4Template template, string hookId) {
			ExecuteEntityViewModelHook(template, hookId);
			return string.Empty;
		}
		public static bool ExecuteEntityViewModelHook(this IT4Template template, string hookId) {
			return template.ExecuteHook(TemplatesCodeGen.STR_EntityViewModelHooks, hookId);
		}
		public static bool ExecuteCollectionViewHook(this IT4Template template, string hookId) {
			return template.ExecuteHook(TemplatesCodeGen.STR_CollectionViewHooks, hookId);
		}
		public static bool ExecuteCollectionViewModelHook(this IT4Template template, string hookId) {
			return template.ExecuteHook(TemplatesCodeGen.STR_CollectionViewModelHooks, hookId);
		}
		public static bool ExecuteDocumentManagerViewHook(this IT4Template template, string hookId) {
			return template.ExecuteHook(TemplatesCodeGen.STR_DocumentManagerViewHooks, hookId);
		}
		public static bool ExecuteDocumentManagerViewModelHook(this IT4Template template, string hookId) {
			return template.ExecuteHook(TemplatesCodeGen.STR_DocumentManagerViewModelHooks, hookId);
		}
		public static bool ExecuteUnitOfWorkHook(this IT4Template template, string hookId) {
			return template.ExecuteHook(TemplatesCodeGen.STR_UnitOfWorkHooks, hookId);
		}
		static bool ExecuteHook(this IT4Template template, string hookCategory, string hookId) {
			var t4TemplateInfo = template.GetTemplateInfo();
			var hooks = t4TemplateInfo.Properties[hookCategory] as IEnumerable<T4TemplateHook>;
			if(hooks != null) {
				var hook = hooks.FirstOrDefault(x => x.Id == hookId && (x.Filter == null || x.Filter(t4TemplateInfo)));
				if(hook != null) {
					hook.Action(template);
					return true;
				}
			}
			return false;
		}
		public static string GetResxFilePath(this IT4Template template) {
			string path = template.GetTemplateInfo().Properties["EmbeddedResourcesPath"].ToString();
			return string.IsNullOrEmpty(path) ? string.Empty : path + ".";
		}
		public static void GenerateResxStrings(this IT4Template template) {
			foreach(var item in GetResourceStrings()) {
				template.Write(string.Format(
@"    <data name=""{0}"" xml:space=""preserve"">
        <value>{1}</value>
    </data>
", item.Key, item.Value));
			}
		}
		public static void GenerateResourceManagerStrings(this IT4Template template) {
			foreach(var item in GetResourceStrings()) {
				template.Write(string.Format(
@"
        /// <summary>
        ///   Looks up a localized string similar to {1}.
        /// </summary>
        internal static string {0} {{
            get {{
                return ResourceManager.GetString(""{0}"", resourceCulture);
            }}
        }}

", item.Key, item.Value));
			}
		}
		static IDictionary<string, string> GetResourceStrings() {
			var dict = new Dictionary<string, string>();
			dict["Confirmation_Delete"] = "Do you want to delete this {0}?";
			dict["Confirmation_Save"] = "Do you want to save changes?";
			dict["Confirmation_Reset"] = "Click OK to reload the entity and lose unsaved changes. Click Cancel to continue editing data.";
			dict["Confirmation_ResetLayout"] = "Are you sure you want to reset layout? Reopen this view for the new layout to take effect.";
			dict["Confirmation_Caption"] = "Confirmation";
			dict["Confirmation_SaveParent"] = "You need to save the parent {0} entity before adding a new record. Do you want to save it now?";
			dict["Warning_Caption"] = "Warning";
			dict["Warning_SomeFieldsContainInvalidData"] = "Some fields contain invalid data. Click OK to close the page and lose unsaved changes. Press Cancel to continue editing data.";
			dict["Exception_UpdateErrorCaption"] = "Update Error";
			dict["Exception_ValidationErrorCaption"] = "Validation Error";
			dict["Exception_DataServiceRequestErrorCaption"] = "DataService Request Error";
			dict["Entity_Changed"] = " *";
			dict["Entity_New"] = " (New)";
			dict["Entity_Deleted"] = " (Deleted)";
			dict["AddRemoveDetailEntities_SelectObjects"] = "Select objects to add";
			return dict;
		}
	}
}
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Views {
	partial class DocumentManagerViewTemplate : IT4Template { }
	partial class DocumentManagerView_WinUITemplate : IT4Template { }
	partial class DocumentManagerView_OutlookTemplate : IT4Template { }
	partial class DocumentManagerView_BrowserTemplate : IT4Template { }
	partial class PeekCollectionViewTemplate : IT4Template { }
	partial class ElementViewTemplate : IT4Template {
		public string GeneratedText { get { return GenerationEnvironment.ToString(); } }
	}
	partial class ElementView_WinUITemplate : IT4Template { }
	partial class RepositoryViewTemplate : IT4Template { }
	partial class RepositoryView_WinUITemplate : IT4Template { }
	partial class ViewCodeBehindTemplate : IT4Template { }
	partial class ViewTemplate : IT4Template { }
}
#region WinForms
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Views.WinForms.Standart {
	partial class ElementView_TabbedMDI :IT4Template { }
	partial class ElementView_TabbedMDIDesigner :IT4Template { }
	partial class CollectionView_TabbedMDI :IT4Template { }
	partial class CollectionView_TabbedMDIDesigner :IT4Template { }
	partial class DocumentManagerView_TabbedMDI :IT4Template { }
	partial class DocumentManagerView_TabbedMDIDesigner :IT4Template { }
}
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Views.WinForms.Outlook {
	partial class ElementView_Outlook :IT4Template { }
	partial class ElementView_OutlookDesigner :IT4Template { }
	partial class CollectionView_Outlook :IT4Template { }
	partial class CollectionView_OutlookDesigner :IT4Template { }
	partial class DocumentManagerView_Outlook :IT4Template { }
	partial class DocumentManagerView_OutlookDesigner :IT4Template { }
}
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Views.WinForms.WinUI {
	partial class ElementView_WinUI :IT4Template { }
	partial class ElementView_WinUIDesigner :IT4Template { }
	partial class CollectionView_WinUI :IT4Template { }
	partial class CollectionView_WinUIDesigner :IT4Template { }
	partial class DocumentManagerView_WinUI :IT4Template { }
	partial class DocumentManagerView_WinUIDesigner :IT4Template { }
}
#endregion
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.ViewModels {
	partial class DocumentManagerViewModelTemplate : IT4Template { }
	partial class EmptyViewModelTemplate : IT4Template { }
	partial class EntityEditViewModelTemplate : IT4Template { }
	partial class EntityRepositoryViewModelTemplate : IT4Template { }
}
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.DataModel {
	partial class UnitOfWorkSourceTemplate : IT4Template { }
	partial class UnitOfWorkRuntimeTemplate : IT4Template { }
	partial class UnitOfWorkTemplate : IT4Template { }
	partial class UnitOfWorkDesignTimeTemplate : IT4Template { }
}
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.ViewModel {
	partial class CollectionViewModelTemplate : IT4Template { }
	partial class AddRemoveDetailEntitiesViewModelTemplate : IT4Template { }
	partial class AddRemoveJunctionDetailEntitiesViewModelTemplate : IT4Template { }
	partial class DocumentManagerServiceExtensionsTemplate : IT4Template { }
	partial class InstantFeedbackCollectionViewModelTemplate : IT4Template { }
	partial class PeekCollectionViewModelTemplate : IT4Template { }
	partial class ISingleObjectViewModelTemplate : IT4Template { }
	partial class ReadOnlyCollectionViewModelTemplate : IT4Template { }
	partial class SingleObjectViewModelTemplate : IT4Template { }
	partial class MessagesTemplate : IT4Template { }
	partial class EntitiesViewModelTemplate : IT4Template { }
	partial class LookUpEntitiesViewModelTemplate : IT4Template { }
	partial class DocumentsViewModelTemplate : IT4Template { }
	partial class PersistentLayoutHelperTemplate : IT4Template { }
}
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common {
	partial class CommonResourcesTemplate : IT4Template { }
	partial class CommonResources_DesignerTemplate : IT4Template { }
	partial class LayoutSettingsTemplate : IT4Template { }
	partial class LayoutSettings_DesignerTemplate : IT4Template { }
}
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.Utils {
	partial class DbExtensionsTemplate : IT4Template { }
	partial class ExpressionHelperTemplate : IT4Template { }
}
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.DataModel.DesignTime {
	partial class DesignTimeReadOnlyRepositoryTemplate : IT4Template { }
	partial class DesignTimeRepositoryTemplate : IT4Template { }
	partial class DesignTimeUnitOfWorkTemplate : IT4Template { }
	partial class DesignTimeInstantFeedbackSourceTemplate : IT4Template { }
	partial class DesignTimeUnitOfWorkFactoryTemplate : IT4Template { }
}
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.DataModel {
	partial class DbExceptionsTemplate : IT4Template { }
	partial class UnitOfWorkBaseTemplate : IT4Template { }
	partial class EntityStateTemplate : IT4Template { }
	partial class IReadOnlyRepositoryTemplate : IT4Template { }
	partial class IRepositoryTemplate : IT4Template { }
	partial class IUnitOfWorkTemplate : IT4Template { }
}
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.DataModel.WCF {
	partial class DbExceptionsConverterTemplate : IT4Template { }
	partial class DbReadOnlyRepositoryTemplate : IT4Template { }
	partial class DbRepositoryTemplate : IT4Template { }
	partial class DbUnitOfWorkTemplate : IT4Template { }
	partial class QueryableWrapperTemplate : IT4Template { }
	partial class InstantFeedbackSourceTemplate : IT4Template { }
	partial class DbUnitOfWorkFactoryTemplate : IT4Template { }
}
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.DataModel.EntityFramework {
	partial class DbExceptionsConverterTemplate : IT4Template { }
	partial class DbReadOnlyRepositoryTemplate : IT4Template { }
	partial class DbRepositoryTemplate : IT4Template { }
	partial class DbUnitOfWorkTemplate : IT4Template { }
	partial class InstantFeedbackSourceTemplate : IT4Template { }
	partial class DbUnitOfWorkFactoryTemplate : IT4Template { }
}
