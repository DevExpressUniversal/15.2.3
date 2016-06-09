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
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TemplateWizard;
using EnvDTE;
using VSLangProj;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.ComponentModelHost;
using VSLangProj80;
using System.Runtime.InteropServices;
using NuGet.VisualStudio;
namespace DevExpress.Xpo.Design {
	class ODataServiceDesigner : IWizard {
		Dictionary<string, string> replacementsDictionary;
		static vsCMElement[] allowedKind = { vsCMElement.vsCMElementClass, vsCMElement.vsCMElementFunction, vsCMElement.vsCMElementNamespace };
		ProjectItem service;
		ProjectItem webConfig;
		bool notUseORMModel;
		bool notDownloadOData;
		string modelFileName;
		IVsPackageInstaller NuGetPackageInstaller;
		internal const string ODataError = "Cannot install 'ODataLib for OData' NuGet package because the NuGet Package Manager version 2.6 or later is not installed. Please install the latest version of the NuGet Package Manager and restart the wizard, or install ODataLib for OData manually.";
		public void BeforeOpeningFile(ProjectItem projectItem) {
			if(projectItem.Name.EndsWith(".svc.cs") || projectItem.Name.EndsWith(".svc.vb")) {
				service = projectItem;
			}
			if(projectItem.Name.EndsWith("Web.config")) {
				webConfig = projectItem;
			}
		}
		public void ProjectFinishedGenerating(Project project) {
		}
		public void ProjectItemFinishedGenerating(ProjectItem projectItem) {
		}
		public void RunFinished() {
			string foundNamespace = string.Empty;
			Dictionary<string, string> dataLayerCode = new Dictionary<string, string>();
			dataLayerCode.Add(PrjKind.prjKindCSharpProject, "DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();" +
				Environment.NewLine + "\t\t\tdict.GetDataStoreSchema(typeof({1}).Assembly);" +
				Environment.NewLine + "\t\t\tIDataLayer dataLayer = new ThreadSafeDataLayer(dict, global::{0}.ConnectionHelper.GetConnectionProvider(DevExpress.Xpo.DB.AutoCreateOption.SchemaAlreadyExists));" +
				Environment.NewLine + "\t\t\tXpoDefault.DataLayer = dataLayer;" +
				Environment.NewLine + "\t\t\tXpoDefault.Session = null;" +
				Environment.NewLine + "\t\t\treturn dataLayer;");
			dataLayerCode.Add(PrjKind.prjKindVBProject, "Dim dict As New DevExpress.Xpo.Metadata.ReflectionDictionary()" +
				Environment.NewLine + "\t\t\tdict.GetDataStoreSchema(GetType({1}).Assembly)" +
				Environment.NewLine + "\t\t\tDim dataLayer As new ThreadSafeDataLayer(dict, {0}.ConnectionHelper.GetConnectionProvider(DevExpress.Xpo.DB.AutoCreateOption.SchemaAlreadyExists))" +
				Environment.NewLine + "\t\t\tXpoDefault.DataLayer = dataLayer" +
				Environment.NewLine + "\t\t\tXpoDefault.Session = Nothing" +
				Environment.NewLine + "\t\t\tReturn dataLayer");
			Dictionary<string, string> dataLayerCodeForEmptyModel = new Dictionary<string, string>();
			dataLayerCodeForEmptyModel.Add(PrjKind.prjKindCSharpProject, "// Uncomment these code after saving the model" +
				Environment.NewLine + "\t\t\t// DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();" +
				Environment.NewLine + "\t\t\t// dict.GetDataStoreSchema(Assembly.GetCallingAssembly());" +
				Environment.NewLine + "\t\t\t// IDataLayer dataLayer = new ThreadSafeDataLayer(dict, DevExpress.Xpo.XpoDefault.GetConnectionProvider(ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.SchemaAlreadyExists));" +
				Environment.NewLine + "\t\t\t// XpoDefault.DataLayer = dataLayer;" +
				Environment.NewLine + "\t\t\t// XpoDefault.Session = null;" +
				Environment.NewLine + "\t\t\t// return dataLayer;" +
				Environment.NewLine + "\t\t\t return null;");
			dataLayerCodeForEmptyModel.Add(PrjKind.prjKindVBProject, "' Uncomment these code after saving the model" +
				Environment.NewLine + "\t\t\t' Dim dict As New DevExpress.Xpo.Metadata.ReflectionDictionary()" +
				Environment.NewLine + "\t\t\t' dict.GetDataStoreSchema(Assembly.GetCallingAssembly())" +
				Environment.NewLine + "\t\t\t' Dim dataLayer As new ThreadSafeDataLayer(dict, DevExpress.Xpo.XpoDefault.GetConnectionProvider(ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.SchemaAlreadyExists))" +
				Environment.NewLine + "\t\t\t' XpoDefault.DataLayer = dataLayer" +
				Environment.NewLine + "\t\t\t' XpoDefault.Session = Nothing" +
				Environment.NewLine + "\t\t\t' Return dataLayer" +
				Environment.NewLine + "\t\t\t Return Nothing");
			try {
				ProjectItem model = GetProjectItem(service.ContainingProject.ProjectItems, modelFileName);
				if(model != null) {
					if(notUseORMModel) {
						model.Delete();
						model = null;
					} else
						model.Open("{00000000-0000-0000-0000-000000000000}");
				}
				model = GetProjectItem(service.ContainingProject.ProjectItems, modelFileName);
				if(model != null) {
					if(model.Document != null)
						model.Document.Close(vsSaveChanges.vsSaveChangesYes);
					string dslModelFile = model.get_FileNames(0);
					if(File.Exists(dslModelFile)) {
						XmlTextReader reader = null;
						try {
							reader = new XmlTextReader(dslModelFile);
							reader.WhitespaceHandling = WhitespaceHandling.None;
							while(reader.Read())
								if(reader.NodeType == XmlNodeType.Element)
									if(reader.Name == "dataBaseObjectModel") {
										foundNamespace = reader.GetAttribute("namespace");
										break;
									}
						} catch { } finally {
							if(reader != null)
								reader.Close();
						}
					}
					string connectionHelperFile = "ConnectionHelper" + (model.ContainingProject.Kind == PrjKind.prjKindVBProject ? ".vb" : ".cs");
					if(GetProjectItem(model.ContainingProject.ProjectItems, connectionHelperFile) == null)
						foundNamespace = string.Empty;
				}
			} catch { }
			if(service != null) {
				try {
					if(service.Document == null)
						service.Open(EnvDTE.Constants.vsViewKindCode);
					FileCodeModel mainFCM = service.FileCodeModel;
					string className;
					if(!replacementsDictionary["$safeprojectname$"].EndsWith("Service"))
						className = replacementsDictionary["$safeprojectname$"] + "Service";
					else
						className = replacementsDictionary["$safeprojectname$"];
					CodeClass mainClass = GetServiceClass(mainFCM.CodeElements) as CodeClass;
					EditPoint methodEPStart;
					EditPoint methodEPEnd;
					if(mainClass == null) {
						TextDocument doc = service.Document.Object("TextDocument") as TextDocument;
						methodEPStart = doc.CreateEditPoint();
						methodEPStart.StartOfDocument();
						methodEPStart.FindPattern("static IDataLayer CreateDataLayer()");
						methodEPEnd = doc.CreateEditPoint(methodEPStart);
						methodEPEnd.FindPattern("}");
					} else {
						className = mainClass.Name;
						CodeFunction createDataLayerFunc = mainClass.Members.Item("CreateDataLayer") as CodeFunction;
						methodEPStart = createDataLayerFunc.StartPoint.CreateEditPoint();
						methodEPEnd = createDataLayerFunc.EndPoint.CreateEditPoint();
					}
					methodEPStart.LineDown(1);
					methodEPStart.StartOfLine();
					TextRanges ranges = null;
					bool result = methodEPStart.ReplacePattern(methodEPEnd, "$datalayer$", string.IsNullOrEmpty(foundNamespace) ?
						dataLayerCodeForEmptyModel[service.ContainingProject.Kind] :
						string.Format(dataLayerCode[service.ContainingProject.Kind], foundNamespace, className), 0, ref ranges);
					EditPoint variableEPStart;
					EditPoint variableEPEnd;
					if(mainClass == null) {
						TextDocument doc = service.Document.Object("TextDocument") as TextDocument;
						variableEPStart = doc.CreateEditPoint();
						variableEPStart.StartOfDocument();
						variableEPStart.FindPattern("serviceContext = new ");
						variableEPStart.StartOfLine();
						variableEPEnd = doc.CreateEditPoint(variableEPStart);
						variableEPEnd.EndOfLine();
					} else {
						CodeVariable var = mainClass.Members.Item("serviceContext") as CodeVariable;
						variableEPStart = var.StartPoint.CreateEditPoint();
						variableEPStart.StartOfLine();
						variableEPEnd = var.EndPoint.CreateEditPoint();
					}
					result = variableEPStart.ReplacePattern(variableEPEnd, "$datanamespace$",
						((string.IsNullOrEmpty(foundNamespace) || service.ContainingProject.Kind == PrjKind.prjKindVBProject) ? replacementsDictionary["$safeprojectname$"] : foundNamespace),
						0, ref ranges);
				} catch { }
			}
			if(webConfig != null && !notDownloadOData) {
				if(webConfig.Document != null)
					webConfig.Document.Close(vsSaveChanges.vsSaveChangesYes);
				string oDataVersion = AddODataToProject(webConfig.ContainingProject);
				replacementsDictionary["$ODataVersion$"] = string.IsNullOrEmpty(oDataVersion) ? "5.6.1.0" : oDataVersion;
				string configFile = webConfig.get_FileNames(0);
				try {
					XmlDocument doc = new XmlDocument();
					doc.Load(configFile);
					foreach(XmlNode node in doc.GetElementsByTagName("bindingRedirect")) {
						node.Attributes["newVersion"].Value = replacementsDictionary["$ODataVersion$"];
					}
					doc.Save(configFile);
				} catch { }
			}
		}
		public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams) {
			DevExpress.Skins.SkinManager.EnableFormSkins();
			DevExpress.LookAndFeel.LookAndFeelHelper.ForceDefaultLookAndFeelChanged();
			DevExpress.LookAndFeel.UserLookAndFeel.Default.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
			this.replacementsDictionary = replacementsDictionary;
			ODataVersionSelectForm versionForm = new ODataVersionSelectForm();
			NuGetPackageInstaller = GetNugetPackage((DTE)automationObject);
			if(NuGetPackageInstaller == null) {
				System.Windows.Forms.MessageBox.Show(ODataError);
				versionForm.DownloadOData = false;
			}
			DialogResult res = versionForm.ShowDialog();
			if(res == DialogResult.OK) {
				notDownloadOData = !versionForm.DownloadOData;
				notUseORMModel = !versionForm.UseORM;
				if(notDownloadOData) {
					replacementsDictionary["$ODataVersion$"] = versionForm.SelectedODataVersion;
				}
			} else {
				throw new WizardCancelledException();
			}
		}
		public bool ShouldAddProjectItem(string filePath) {
			if(filePath.EndsWith(".xpo")) {
				modelFileName = filePath;
			}
			return true;
		}
		static CodeElement GetServiceClass(CodeElements elements) {
			const vsCMElement kind = vsCMElement.vsCMElementClass;
			const string baseClassName = "XpoDataServiceV3";
			if(elements == null)
				return null;
			foreach(CodeElement element in elements) {
				if(!allowedKind.Contains(element.Kind))
					continue;
				CodeElement result = element;
				if(result != null && result.Kind == kind && ((CodeClass)result).Bases.OfType<CodeClass>().Any(c => c.Name == baseClassName))
					return result;
				result = GetServiceClass(element.Children);
				if(result != null)
					return result;
			}
			return null;
		}
		static ProjectItem GetProjectItem(ProjectItems prjItems, string name) {
			if(prjItems == null)
				return null;
			foreach(ProjectItem item in prjItems) {
				if(item.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile) {
					string itemFileName = Path.GetFileName((string)item.Properties.Item("FullPath").Value);
					if(itemFileName == name)
						return item;
				}
				ProjectItem result = GetProjectItem(item.ProjectItems, name);
				if(result != null)
					return result;
			}
			return null;
		}
		IVsPackageInstaller GetNugetPackage(DTE dte) {
			ServiceProvider serviceProvider = null;
			IVsPackageInstaller result = null;
			try {
				serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte);
				var componentModel = (IComponentModel)serviceProvider.GetService(typeof(SComponentModel));
				result = componentModel.GetService<IVsPackageInstaller>();
			} catch {
			} finally {
				if(serviceProvider != null)
					serviceProvider.Dispose();
			}
			return result;
		}
		string AddODataToProject(Project project) {
			project.DTE.StatusBar.Text = "Installing NuGet package ODataLib for OData";
			try {
				NuGetPackageInstaller.InstallPackage(@"https://www.nuget.org/api/v2/", project, "Microsoft.Data.Services", (Version)null, false);
			} catch(Exception) {
				System.Windows.Forms.MessageBox.Show(ODataError);
			} finally {
				project.DTE.StatusBar.Text = "";
			}
			Reference odataReference = ((VSProject2)project.Object).References.Find("Microsoft.Data.OData");
			return odataReference != null ? odataReference.Version : null;
		}
	}
}
namespace NuGet.VisualStudio {
	[ComImport, Guid("4F3B122B-A53B-432C-8D85-0FAFB8BE4FF4"), TypeIdentifier]
	public interface IVsPackageInstaller {
		void InstallPackage(string source, Project project, string packageId, Version version, bool ignoreDependencies);
	}
}
