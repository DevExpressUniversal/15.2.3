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
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using EnvDTE;
using VSLangProj;
using System.Collections;
using DevExpress.ExpressApp.Utils;
using System.Drawing;
namespace DevExpress.ExpressApp.Design {
	public class ResourceLocalizedTypesEditor : BaseCollectionEditor {
		protected override ICollectionEditorForm CreateEditorForm(Type listType) {
			CheckListCollectionEditorForm editorForm = new CheckListCollectionEditorForm();
			editorForm.Text = "Resources exported to model";
			Image localizationImage = DesignImagesLoader.GetImage("Designer_Localization.ico");
			if(localizationImage != null) {
				editorForm.Icon = Icon.FromHandle(new Bitmap(localizationImage).GetHicon());
			}
			return editorForm;
		}
		protected override IList GetDataSource(ITypeDescriptorContext context) {
			IList<ModuleBase> modules;
			if(context.Instance is XafApplication) {
				modules = (context.Instance as XafApplication).Modules;
			}
			else if(context.Instance is ModuleBase) {
				ModuleBase moduleBase = (ModuleBase)context.Instance;
				modules = new List<ModuleBase>();
				ApplicationModulesManager.AddModuleIntoList(moduleBase, modules);
			}
			else {
				return null;
			}
			return (IList)GetAllLocalizerTypes(modules);
		}
		private ICollection<Type> GetAllLocalizerTypes(IList<ModuleBase> modules) {
			List<Type> result = new List<Type>();
			foreach(ModuleBase moduleBase in modules) {
				result.AddRange(moduleBase.GetXafResourceLocalizerTypes());
			}
			return result;
		}
		private VSProject GetActiveProject(IServiceProvider provider) {
			_DTE dte = (_DTE)provider.GetService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
			System.Array projects = dte.ActiveSolutionProjects as System.Array;
			Project project = projects.GetValue(0) as Project;
			return project.Object as VSLangProj.VSProject;
		}
		private bool IsProjectContainsType(VSProject project, Type type) {
			if(type == null || type == typeof(object)) {
				return true;
			}
			foreach(VSLangProj.Reference reference in project.References) {
				if(reference.Name == GetAssemblyName(type)) {
					return true;
				}
			}
			return false;
		}
		private string GetAssemblyName(Type type) {
			return type.Assembly.FullName.Split(',')[0];
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			IList result = (IList)base.EditValue(context, provider, value);
			if(result.Count > 0) {
				VSProject vsProject = GetActiveProject(provider);
				foreach(Type typeItem in result) {
					if(!IsProjectContainsType(vsProject, typeItem.BaseType)) {
						vsProject.References.Add(typeItem.BaseType.Assembly.FullName);
					}
				}
			}
			return result;
		}
	}
}
