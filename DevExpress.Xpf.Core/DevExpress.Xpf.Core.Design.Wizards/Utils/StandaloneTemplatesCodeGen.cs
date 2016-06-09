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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.Design.Mvvm;
using DevExpress.Design.Mvvm.Wizards.UI;
using DevExpress.Entity.Model;
using DevExpress.Entity.ProjectModel;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.DataModel;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData;
namespace DevExpress.Xpf.Core.Design.Wizards.Utils {
	public class StandaloneWcfEdmInfoProvider : IWcfEdmInfoProvider {
		public string[] WcfResourceNames { get; set; }
		Stream IWcfEdmInfoProvider.GetEdmItemCollectionStream(Type clrType) {
			List<string> wcfResources = new List<string>();
			if(WcfResourceNames != null)
				wcfResources.AddRange(WcfResourceNames);
			if(!wcfResources.Contains(clrType.Name))
				wcfResources.Add(clrType.Name);
			string edmxName = clrType.Assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith("edmx") && x.Contains("Service_References") && wcfResources.Any(name => x.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0));
			return clrType.Assembly.GetManifestResourceStream(edmxName);
		}
		string IWcfEdmInfoProvider.GetSourceUrl(Type clrType) {
			List<string> wcfResources = new List<string>();
			if(WcfResourceNames != null)
				wcfResources.AddRange(WcfResourceNames);
			if(!wcfResources.Contains(clrType.Name))
				wcfResources.Add(clrType.Name);
			string edmxName = clrType.Assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith("datasvcmap") && x.Contains("Service_References") && wcfResources.Any(name => x.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0));
			Stream stream = clrType.Assembly.GetManifestResourceStream(edmxName);
			try {
				return WcfManifestResourceEdmInfoProvider.GetSourceUrl(stream);
			} finally {
				stream.Close();
			}
		}
	}
	public class StandaloneTemplatesCodeGen : TemplatesCodeGen, ITemplatesPlatform {
		struct AssemblyReferenceData {
			public string Name { get; set; }
			public bool CopyLocal { get; set; }
			public override string ToString() {
				return string.IsNullOrEmpty(Name) ? "null" : Name;
			}
		}
		List<AssemblyReferenceData> references = new List<AssemblyReferenceData>();
		bool resXGenerated;
		StandaloneWcfEdmInfoProvider wcfEdmInfoProvider = new StandaloneWcfEdmInfoProvider();
		public StandaloneTemplatesCodeGen(Type sqlProviderAssemblyType, Assembly projectAssembly, params Assembly[] solutionAssemblies)
			: base(new ServiceCache()) {
			ServiceCache.Register<IUndoManager, BaseUndoManager>(() => new BaseUndoManager());
			ServiceCache.Register<ISolutionTypesProvider>(() => {
				var typesProvider = new SolutionTypesProviderConsole(projectAssembly.FullName, solutionAssemblies.Select(a => a.FullName).ToArray());
				typesProvider.Add(sqlProviderAssemblyType);
				typesProvider.AddRange(new Assembly[] { projectAssembly }.Concat(solutionAssemblies).SelectMany(a => a.GetTypes()));
				return typesProvider;
			});
			ServiceCache.Register<IEntityFrameworkModel, EntityFrameworkModel>(() => new EntityFrameworkModel(ServiceContainer));
			ServiceCache.Register<IDataAccessLayerService, DataAccesLayerService>(() => new DataAccesLayerService(ServiceContainer));
			ServiceCache.Register<IViewModelLayerService, ViewModelLayerService>(() => new ViewModelLayerService(ServiceContainer));
			ServiceCache.Register<ITemplatesCodeGen>(() => this);
			ServiceCache.Register<ITemplatesPlatform>(() => this);
		}
		public void GenerateDocumentsView(IDataModel dataModel, PlatformType platformType,TaskType taskType,ViewModelType viewModelType, UIType uiType, DocumentsGenerationOptions options, params string[] entityNames) {
			var context = new TemplateGenerationContext(
				platformType,
				taskType,
				uiType,
				dataModel.Entities.Where(e => entityNames.Contains(e.ElementType.Type.Name)),
				dataModel.DbContainer,
				false);
			GenerateDocumentsViewCore(context, new[] { uiType }, dataModel, false, options);
		}
		public void GenerateDocumentsViewForAllEntities(IDataModel dataModel, PlatformType platformType, TaskType taskType, ViewModelType viewModelType, UIType uiType, DocumentsGenerationOptions options) {
			var context = new TemplateGenerationContext(
				platformType,
				taskType,
				uiType,
				dataModel.Entities,
				dataModel.DbContainer,
				false);
			GenerateDocumentsViewCore(context, new[] { uiType }, dataModel, false, options);
		}
		protected EntityFrameworkModel EntityFrameworkModel { get { return (EntityFrameworkModel)ServiceContainer.GetService(typeof(IEntityFrameworkModel)); } }
		protected ServiceCache ServiceCache { get { return (ServiceCache)base.ServiceContainer; } }
		public new System.ComponentModel.Design.IServiceContainer ServiceContainer { get { return base.ServiceContainer; } }
		protected override ITemplatesPlatform GetTemplatesPlatform() {
			return this;
		}
		public string TargetFolder { get; set; }
		public string DefaultNamespace { get; set; }
		public bool IsVisualBasic { get; set; }
		public bool WithoutDesignTime { get; set; }
		#region ITemplatesPlatform        
		void ITemplatesPlatform.AddAssemblyReference(string referenceName, bool copyLocal) {
			if(string.IsNullOrEmpty(referenceName) || references.Any(ri => ri.Name == referenceName))
				return;
			references.Add(new AssemblyReferenceData() { Name = referenceName, CopyLocal = copyLocal });
		}
		bool ITemplatesPlatform.AddFileAndDependencies(string fileName, bool rebuildNeeded, IDictionary<string, string> properties, params string[] dependencies) {
			if(fileName.EndsWith("CommonResources.resx")) {
				if(resXGenerated || rebuildNeeded || properties.Count != 1 || properties["CustomTool"] != "ResXFileCodeGenerator" || dependencies.Length != 1 || !dependencies[0].EndsWith("CommonResources.Designer.cs"))
					throw new NotSupportedException();
				resXGenerated = true;
			}
			return true;
		}
		MvvmProjectItem ITemplatesPlatform.AddFromFile(string fileName, bool rebuildNeeded) {
			return new MvvmProjectItem();
		}
		string ITemplatesPlatform.GetDefaultNamespace() {
			return DefaultNamespace;
		}
		string ITemplatesPlatform.GetProjectNamespace() {
			return DefaultNamespace;
		}
		string ITemplatesPlatform.GetTargetFolder() {
			return TargetFolder;
		}
		string ITemplatesPlatform.GetProjectFolder() {
			return TargetFolder;
		}
		string ITemplatesPlatform.FormatXaml(string xaml) {
			return TemplatingUtils.FormatXaml(xaml);
		}
		bool ITemplatesPlatform.RewriteExistingFiles {
			get { return true; }
		}
		bool ITemplatesPlatform.IsItemExist(string uniqueName) {
			return false;
		}
		bool ITemplatesPlatform.IsVisualBasic {
			get { return IsVisualBasic; }
		}
		string ITemplatesPlatform.GetEmbeddedResourcesPathOverride(string fileFolder) {
			return null;
		}
		void ITemplatesPlatform.AddLicXFile() { }
		IWcfEdmInfoProvider ITemplatesPlatform.WcfEdmInfoProvider { get { return wcfEdmInfoProvider; } }
		#endregion
		#region hooks
		public IEnumerable<T4TemplateHook> DebugLogHooks { get; set; }
		public IEnumerable<T4TemplateHook> EntityViewHooks { get; set; }
		public IEnumerable<T4TemplateHook> EntityViewModelHooks { get; set; }
		public IEnumerable<T4TemplateHook> CollectionViewHooks { get; set; }
		public IEnumerable<T4TemplateHook> CollectionViewModelHooks { get; set; }
		public IEnumerable<T4TemplateHook> DocumentManagerViewHooks { get; set; }
		public IEnumerable<T4TemplateHook> DocumentManagerViewModelHooks { get; set; }
		protected override IEnumerable<T4TemplateHook> GetDebugLogHooks() {
			return DebugLogHooks;
		}
		protected override IEnumerable<T4TemplateHook> GetEntityViewHooks() {
			return EntityViewHooks;
		}
		protected override IEnumerable<T4TemplateHook> GetEntityViewModelHooks() {
			return EntityViewModelHooks;
		}
		protected override IEnumerable<T4TemplateHook> GetCollectionViewHooks() {
			return CollectionViewHooks;
		}
		protected override IEnumerable<T4TemplateHook> GetCollectionViewModelHooks() {
			return CollectionViewModelHooks;
		}
		protected override IEnumerable<T4TemplateHook> GetDocumentManagerViewHooks() {
			return DocumentManagerViewHooks;
		}
		protected override IEnumerable<T4TemplateHook> GetDocumentManagerViewModelHooks() {
			return DocumentManagerViewModelHooks;
		}
		#endregion
	}
}
