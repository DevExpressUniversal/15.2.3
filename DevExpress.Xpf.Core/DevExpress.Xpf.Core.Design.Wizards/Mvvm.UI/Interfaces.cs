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
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Design.Mvvm.EntityFramework;
using System.Text;
using DevExpress.Entity.ProjectModel;
using DevExpress.Entity.Model;
using DevExpress.Design.Mvvm.Wizards.UI;
using DevExpress.Xpf.Internal.EntityFrameworkWrappers;
using DevExpress.Mvvm.Native;
namespace DevExpress.Design.Mvvm {
	public interface IViewModelInfoBase : IHasName {
		string AssemblyName { get; }
		string Namespace { get; }
		bool IsSolutionType { get; }
		bool IsLocalType { get; }
	}
	public interface IViewModelInfo : IViewModelInfoBase {
		ViewModelType ViewModelType { get; }
		string EntityTypeName { get; }
		string DefaultViewFolderName { get; }
		string EntityTypeFullName { get; }
		bool Activated { get; }
	}
	public interface IViewModelLayerService {
		IEnumerable<IViewModelInfo> GetViewModelTypes();
		string GetViewModelName(string typeName, ViewModelType modelType);
		string GetViewName(string typeName, string viewModelName, ViewType viewType);
	}
	public class TemplateGenerationContext {
		public TemplateGenerationContext(
			PlatformType platformType, 
			TaskType taskType, 
			UIType uiType, 
			IEnumerable<IEntitySetInfo> selectedTables,
			IDbContainerInfo dbContainer,
			bool withoutDesignTime)
		{
			PlatformType = platformType;
			TaskType = taskType;
			UiType = uiType;
			SelectedTables = selectedTables;
			DbContainer = dbContainer;
			WithoutDesignTime = withoutDesignTime;
		}
		public TemplateGenerationContext(MvvmConstructorContext context)
			: this(context.PlatformType,
			  context.TaskType,
			  context.SelectedUIType,
			  context.SelectedTables,
			  context.SelectedDataModel.With(x => x.DbContainer),
			  context.WithoutDesignTime) { }
		public PlatformType PlatformType { get; private set; }
		public TaskType TaskType { get; private set; }
		public UIType UiType { get; private set; }
		public IEnumerable<IEntitySetInfo> SelectedTables { get; private set; }
		public bool WithoutDesignTime { get; private set; }
		public IDbContainerInfo DbContainer { get; set; }
		internal MetadataWorkspaceRuntimeWrapper MetadataWorkspace {
			get { return DbContainer.With(x => MetadataWorkspaceRuntimeWrapper.Wrap(x.MetadataWorkspace)); } 
		}
	}
	public interface ITemplatesCodeGen {
		void GenerateView(TemplateGenerationContext context, IViewModelInfo viewModel, string viewName, string folderName, ViewType viewType, bool async);
		IViewModelInfo GenerateViewModel(TemplateGenerationContext context, ViewModelType viewNodelType, string viewModelName, string folderName, IDataModel dataModel, IEntitySetInfo entity, bool async);
		IDataModel GenerateEntityDataModel(TemplateGenerationContext context, IDbContainerInfo container, bool async);
		void GenerateDocumentsView(TemplateGenerationContext context, IDbContainerInfo dbContainerInfo, ViewModelType viewModelType, bool async);
		void GenerateDocumentsView(TemplateGenerationContext context, IDataModel dataModel, ViewModelType viewModelType, bool async);
	}
	public interface IWizardRunContext {
		IDictionary<string, string> ReplacementsDictionary { get; }
		object AutomationObject { get; }
		IEnumerable<object> CustomParams { get; }
	}
	public interface ILogItem {		
		void OutText(System.Windows.Documents.BlockCollection blockCollection);
		void OutText(StringBuilder sb);
	}
	public interface ILogServices {
		IEnumerable<ILogItem> GetItems();
	}
}
