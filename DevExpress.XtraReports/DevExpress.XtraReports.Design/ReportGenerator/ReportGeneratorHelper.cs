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
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Data.XtraReports.ReportGeneration;
using DevExpress.XtraExport;
using DevExpress.XtraReports.UI;
using EnvDTE;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Printing;
using DevExpress.XtraReports.ReportGeneration.Wizard;
namespace DevExpress.XtraReports.Design.ReportGenerator{
	public class ReportGeneratorHelper{
		ReportGeneratorDesigner componentDesigner;
		Project componentProject;
		public ReportGeneratorHelper(ReportGeneratorDesigner componentDesigner){
			this.componentDesigner = componentDesigner;
		}
		internal IEnumerable<IComponent> SiteComponents{
			get { return componentDesigner.DesignerHost.AllComponents<Component>(); }
		}
		internal Project ComponentProject{
			get{
				if(componentProject == null) componentProject = GetProject(componentDesigner.Component.Site);
				return componentProject;
			}
		}
		List<string> reportNamesSet;
		internal List<string> ReportNamesSet{
			get {
				if(reportNamesSet == null){
					reportNamesSet = new List<string>();
				}
				return reportNamesSet;
			}
		}
		internal string DefaultFileName{
			get {
				string reportName = GetReportName();
				return reportName;
			}
		}
		List<EnvDTE.CodeClass> classes;
		internal List<EnvDTE.CodeClass> ProjectClasses{
			get {
				if(classes == null){
					classes = IterateProjectClasses();
				}
				return classes;
			}
		}
		private List<CodeClass> IterateProjectClasses(){
			var pclasses = new List<CodeClass>();
			var namespaces = new List<string>();
			var pItems = GetProjectItems(ComponentProject.ProjectItems);
			GetProjectNamespaces(pItems, namespaces);
			var codeNamespaces = ComponentProject.CodeModel.CodeElements.OfType<CodeNamespace>().Where(n => namespaces.Contains(n.FullName));
			foreach(CodeNamespace ns in codeNamespaces){
				foreach(CodeClass cc in ns.Members.OfType<CodeClass>()){
					pclasses.Add(cc);
					if(cc.Name.Contains(DefaultReportName) && !ReportNamesSet.Contains(cc.Name)){
						ReportNamesSet.Add(cc.Name);
					}
				}
			}
			return pclasses;
		}
		public List<ProjectItem> GetProjectItems(ProjectItems items){
			var res = new List<ProjectItem>();
			if (items == null) return res;
			foreach(ProjectItem item in items){
				res.Add(item);
				res.AddRange(GetProjectItems(item.ProjectItems));
			}
			return res;
		}
		string GetReportName(){
			var rClass = ProjectClasses.FirstOrDefault(c => ReportNamesSet.Contains(c.Name));
			if(rClass != null) return ConstructReportName();
			return ReportNamesSet.Count > 0 ? ReportNamesSet.Last() : DefaultReportName;
		}
		private string ConstructReportName(){
			bool contains = ReportNamesSet.Contains(DefaultReportName);
			if(!contains) return DefaultReportName; 
			for(int i = 1; i < ReportNamesSet.Count+1; i++){
				if(!ReportNamesSet.Contains(DefaultReportName + i))
					return DefaultReportName + i;
			}
			return string.Empty;
		}
		private static string DefaultReportName { get { return "MyReport"; } }
		static void GetProjectNamespaces(List<ProjectItem> pItems, List<string> namespaces){
			foreach(ProjectItem item in pItems){
				if(item.FileCodeModel == null) continue; 
				foreach(CodeElement element in item.FileCodeModel.CodeElements){
					if(element.Kind == vsCMElement.vsCMElementNamespace && !namespaces.Contains(element.FullName)){
						namespaces.Add(element.FullName);
					}
				}
			}
		}
		internal IDesignerHost DesignerHost { get { return componentDesigner.DesignerHost; } }
		internal string GetProjectPath(){
			return Path.GetDirectoryName(ComponentProject.FullName);
		}
		public void GenerateAndSave(ReportGridDataModel model){
			var report = model.Generate((IGridViewFactory<ColumnImplementer, DataRowImplementer>)model.View, model.Options);
			if(!string.IsNullOrEmpty(model.ReportFilePath))
				report.SaveLayout(model.ReportFilePath);
		}
		public void GenerateAndAddToProject(ReportGridDataModel model) {
			var dte = ComponentProject.DTE;
			var activeDocumentPath = Path.GetDirectoryName(dte.ActiveDocument.FullName);
			string reportTemplatePath = ConstructReportFilePath(activeDocumentPath, model.ReportFileName, GetFileExtension());
			ConstructReportTemplate(model, reportTemplatePath);
			var window = ComponentProject.ProjectItems.DTE.ItemOperations.OpenFile(reportTemplatePath, Constants.vsViewKindDesigner);
			if(window != null) {
				var reportDesignerHost = window.Object as IDesignerHost;
				var generatedReport = new XtraReport();
				ReportGenerationExtensions<ColumnImplementer, DataRowImplementer>.Generate(generatedReport, (IGridViewFactory<ColumnImplementer, DataRowImplementer>) model.View, model.Options);
				var saveReportFilePath = ConstructReportFilePath(activeDocumentPath, model.ReportFileName, "repx");
				generatedReport.SaveLayout(saveReportFilePath);
				if(reportDesignerHost != null) {
					var addedReport = (XtraReport) reportDesignerHost.RootComponent;
					Import.RepxConverter converter = new Import.RepxConverter();
					converter.Convert(saveReportFilePath, addedReport);
					SetReportDataSource(reportDesignerHost, (IGridViewFactory<ColumnImplementer, DataRowImplementer>) model.View);
					RestoreReportLayout(window, reportTemplatePath);
					File.Delete(saveReportFilePath);
				}
			}
		}
		void ConstructReportTemplate(ReportGridDataModel model, string reportFilePath) {
			CodeModel cm = ComponentProject.CodeModel;
			object[] bases = {typeof(XtraReport).Namespace + "." + typeof(XtraReport).Name};
			if(cm.Language == CodeModelLanguageConstants.vsCMLanguageCSharp) {
				var ns = cm.AddNamespace(GetReportNamespace(), reportFilePath, -1);
				ns.AddClass(model.ReportFileName, -1, bases, new object[] {}, vsCMAccess.vsCMAccessPublic);
			}
			if(cm.Language == CodeModelLanguageConstants.vsCMLanguageVB) {
				cm.AddClass(model.ReportFileName, reportFilePath, -1, bases, new object[] {}, vsCMAccess.vsCMAccessPublic);
			}
		}
		string GetReportNamespace() {
			var componentFormName = componentDesigner.DesignerHost.RootComponentClassName;
			return componentFormName.Split(Convert.ToChar("."))[0];
		}
		string ConstructReportFilePath(string location, string reportFileName, string fileExtension) {
			return location + "\\" + reportFileName + "." + fileExtension;
		}
		string GetFileExtension() {
			CodeModel cm = ComponentProject.CodeModel;
			switch(cm.Language) {
				case CodeModelLanguageConstants.vsCMLanguageCSharp:
					return "cs";
				case CodeModelLanguageConstants.vsCMLanguageVB:
					return "vb";
				default:
					throw new ArgumentException("Language not supported");
			}
		}
		void RestoreReportLayout(Window reportWindow, string filePath){
			reportWindow.Close(vsSaveChanges.vsSaveChangesYes);
			ComponentProject.ProjectItems.DTE.ItemOperations.OpenFile(filePath, Constants.vsViewKindDesigner);
		}
		void SetReportDataSource(IDesignerHost reportDesignerHost, IGridViewFactory<ColumnImplementer, DataRowImplementer> viewFactory) {
			var generatedReport = (XtraReport) reportDesignerHost.RootComponent;
			object dataSource = viewFactory.GetDataSource();
			string dataMember = viewFactory.GetDataMember();
			if(generatedReport != null){
				if(typeof(BindingSource).IsInstanceOfType(dataSource)){
					dataMember = ((BindingSource) dataSource).DataMember;
				}
				if(!typeof(DataAccess.Sql.SqlDataSource).IsInstanceOfType(dataSource)){ 
					generatedReport.DataSource = dataSource;
					generatedReport.DataAdapter = SearchTableAdapterByDataMember(dataMember);
				}
			}
		}
		IComponent SearchTableAdapterByDataMember(string dataMember){
			const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty;
			foreach(var component in SiteComponents){
				var adapterProperty = GetProperty(component, "Adapter", bindingFlags);
				if(adapterProperty != null){
					var adapterPropertyValue = adapterProperty.GetValue(component, null);
					var tableMappingsProperty = GetProperty(adapterPropertyValue, "TableMappings");
					var tableMappingPropertyValue = tableMappingsProperty.GetValue(adapterPropertyValue, null);
					var tables = tableMappingPropertyValue as System.Data.Common.DataTableMappingCollection;
					if(tables != null && tables.Count > 0){
						if(tables.IndexOfDataSetTable(dataMember) != -1)
							return component;
					}
				}
			}
			return null;
		}
		static PropertyInfo GetProperty(object src, string propertyName, BindingFlags bindingFlags){
			return src.GetType().GetProperty(propertyName, bindingFlags);
		}
		static PropertyInfo GetProperty(object src, string propertyName){
			return src.GetType().GetProperty(propertyName);
		}
		static Project GetProject(IServiceProvider provider){
			if(provider == null) return null;
			var item = provider.GetService(typeof(ProjectItem)) as ProjectItem;
			return item != null ? item.ContainingProject : null;
		}
		internal IEnumerable<GridView> GetViewSet() {
			var components = new List<GridView>();
			if(componentDesigner.ReportGenerator.Container != null) {
				foreach(var tComponent in componentDesigner.ReportGenerator.Container.Components) {
					if(tComponent is GridView) {
						if(!(tComponent is BandedGridView)) 
							components.Add((GridView) tComponent);
					}
				}
			}
			return components;
		}
	}
}
