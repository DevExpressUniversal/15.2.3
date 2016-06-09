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
using System.Threading;
using System.Windows.Forms;
using System.IO;
using DevExpress.Xpo.Metadata;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Model;
using System.Collections.Generic;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Xpo;
namespace DevExpress.ExpressApp.ModelEditor {
	public class MainClass {
		static String usage = string.Format(@"DevExpress.ExpressApp.ModelEditor.{0}.exe [Path to the application configuration file] [Path to the application assembly file]", AssemblyInfo.VSuffixWithoutSeparator);
		private static ModelEditorForm modelEditorForm;
		static private void HandleException(Exception e) {
			Tracing.Tracer.LogError(e);
			Exception exceptionToShow = e;
			if(exceptionToShow is DesignerModelFactory_FindApplicationAssemblyException) {
				exceptionToShow = new Exception(e.Message + Environment.NewLine +
						"To avoid ambiguity, specify assembly file name as the second parameter." + Environment.NewLine + usage);
			}
			Messaging.GetMessaging(null).Show(ModelEditorForm.Title, exceptionToShow);
		}
		static private void OnException(object sender, ThreadExceptionEventArgs e) {
			HandleException(e.Exception);
		}
		private static string targetFileName = null;
		private static string diffsPath = null;
		private static string targetDllFileName = null;
		private static string deviceSpecificDifferencesStoreName = null;
		private static bool easyTestEnabled = false;
		private static bool IsDeviceSpecificModel(string targetDiffFileName) {
			return targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultMobileName + ModelDifferenceStore.ModelFileExtension) ||
				targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultTabletName + ModelDifferenceStore.ModelFileExtension) ||
				targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultDesktopName + ModelDifferenceStore.ModelFileExtension);
		}
		private static string GetDeviceSpecificModelDiffDefaultName(string targetDiffFileName) {
			if(targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultMobileName + ModelDifferenceStore.ModelFileExtension)) {
				return ModelDifferenceStore.AppDiffDefaultMobileName;
			}
			else {
				if(targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultTabletName + ModelDifferenceStore.ModelFileExtension)) {
					return ModelDifferenceStore.AppDiffDefaultTabletName;
				}
				else {
					if(targetDiffFileName.Contains(ModelDifferenceStore.AppDiffDefaultDesktopName + ModelDifferenceStore.ModelFileExtension)) {
						return ModelDifferenceStore.AppDiffDefaultDesktopName;
					}
				}
			}
			return null;
		}
		private static bool ParseCommandLine(string[] args) {
			string errorMessage = null;
			List<string> listArgs = new List<string>(args);
			if(listArgs.Remove("-EasyTest")) {
				easyTestEnabled = true;
				args = listArgs.ToArray();
			}
			if(args.Length > 2) {
				errorMessage = string.Format("Wrong command line argument count.");
			}
			else if(args.Length != 0) {
				if(args.Length == 1) {
					if(args[0] == "/?") {
						errorMessage = "";
					}
					else {
						targetFileName = args[0];
						if(Path.GetExtension(targetFileName).ToLower() != ".config") {
							errorMessage = "The application config file name isn't specified.";
						}
					}
				}
				if(args.Length == 2) {
					targetFileName = args[0];
					string targetFileExtension = Path.GetExtension(targetFileName).ToLower();
					if(targetFileExtension != ".dll" && Directory.Exists(args[1])) {
						errorMessage = "The module file name isn't specified.";
					}
					if(targetFileExtension == ".dll" && !Directory.Exists(args[1])) {
						errorMessage = String.Format("The '{0}' model differences folder couldn't be found.", args[1]);
					}
					if(targetFileExtension == ".config" && IsDeviceSpecificModel(args[1])) {
						deviceSpecificDifferencesStoreName = GetDeviceSpecificModelDiffDefaultName(args[1]);
					}
					else {
						if(targetFileExtension == ".config" && (Path.GetExtension(args[1]).ToLower() != ".dll" && Path.GetExtension(args[1]).ToLower() != ".exe")) {
							errorMessage = String.Format("Application assembly file name isn't specified.", args[1]);
						}
						if(targetFileExtension == ".dll") {
							diffsPath = args[1];
						}
						else if(targetFileExtension == ".config") {
							targetDllFileName = args[1];
						}
					}
				}
				if(!string.IsNullOrEmpty(targetFileName) && !File.Exists(targetFileName)) {
					errorMessage = String.Format("The '{0}' file couldn't be found.", targetFileName);
				}
			}
			if(errorMessage != null) {
				if(!string.IsNullOrEmpty(errorMessage)) {
					errorMessage += Environment.NewLine;
				}
				errorMessage += string.Format("Usage: {0}{1} <Application configuration file name> {0}, <Application configuration file name> <Application XAFML file name> {0} {1}, or <Application configuration file name> <Application assembly file name> {0}, or {0} {1} <Module assembly file name> <Model differences folder>",
				   Environment.NewLine, Path.GetFileName(Environment.GetCommandLineArgs()[0]));
				MessageBox.Show(errorMessage, ModelEditorForm.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
		private class StandaloneModelEditorModelLoader {
			string targetFileName;
			string diffsPath;
			FileModelStore fileModelStore;
			IModelApplication modelApplication;
			public StandaloneModelEditorModelLoader() {
				XafTypesInfo.HardReset();
				ImageLoader.Reset();
				XpoTypesInfoHelper.ForceInitialize();
			}
			public void LoadModel(string targetFileName, string modelDifferencesStorePath, string deviceSpecificDifferencesStoreName, string assembliesPath) {
				this.targetFileName = targetFileName;
				this.diffsPath = modelDifferencesStorePath;
				if(string.IsNullOrEmpty(assembliesPath)) {
					assembliesPath = System.IO.Path.GetDirectoryName(targetFileName);
					if(assembliesPath == "") {
						assembliesPath = Environment.CurrentDirectory;
					}
				}
				fileModelStore = null;
				DesignerModelFactory dmf = new DesignerModelFactory();
				modelApplication = null;
				if(dmf.IsApplication(targetFileName)) {
					if(string.IsNullOrEmpty(diffsPath)) {
						diffsPath = assembliesPath;
					}
					XafApplication application = dmf.CreateApplicationByConfigFile(targetFileName, targetDllFileName, ref assembliesPath);
					if(string.IsNullOrEmpty(deviceSpecificDifferencesStoreName)) {
						fileModelStore = dmf.CreateApplicationModelStore(diffsPath);
						modelApplication = dmf.CreateApplicationModel(application, dmf.CreateModulesManager(application, targetFileName, assembliesPath), targetFileName, fileModelStore);
					}
					else {
						FileModelStore baseModelStore = dmf.CreateApplicationModelStore(diffsPath);
						fileModelStore = dmf.CreateApplicationModelStore(diffsPath, deviceSpecificDifferencesStoreName);
						modelApplication = dmf.CreateApplicationModel(application, dmf.CreateModulesManager(application, targetFileName, assembliesPath), targetFileName, baseModelStore, fileModelStore);
					}
				}
				else {
					ModuleBase module = dmf.CreateModuleFromFile(targetFileName, assembliesPath);
					if(string.IsNullOrEmpty(diffsPath)) {
						diffsPath = assembliesPath;
					}
					fileModelStore = dmf.CreateModuleModelStore(diffsPath);
					modelApplication = dmf.CreateApplicationModel(module, dmf.CreateModulesManager(module, assembliesPath), fileModelStore);
				}
				CaptionHelper.Setup(modelApplication);
			}
			public string TargetFileName {
				get {
					return targetFileName;
				}
			}
			public string DiffsPath {
				get {
					return diffsPath;
				}
			}
			public FileModelStore FileModelStore {
				get {
					return fileModelStore;
				}
			}
			public IModelApplication ModelApplication {
				get {
					return modelApplication;
				}
			}
		}
		static bool isModelLoaded = false;
		static ModelEditorViewController controller;
		[STAThread]
		static void Main(string[] args) {
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += new ThreadExceptionEventHandler(OnException);
			if(ParseCommandLine(args)) {
				if(easyTestEnabled) {
					try {
						DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
					}
					catch(Exception) { }
				}
				try {
					DevExpress.ExpressApp.Model.Core.DesignerOnlyCalculator.IsRunFromDesigner = true;
					StandaloneModelEditorModelLoader loader = new StandaloneModelEditorModelLoader();
					if(!string.IsNullOrEmpty(targetFileName)) {
						loader.LoadModel(targetFileName, diffsPath, deviceSpecificDifferencesStoreName, null);
						isModelLoaded = true;
					}
					else {
						ImageLoader.Init(new ImageSource[] { new AssemblyResourceImageSource("DevExpress.ExpressApp.Images" + XafApplication.CurrentVersion), 
							new AssemblyResourceImageSource("DevExpress.ExpressApp.Win" + XafApplication.CurrentVersion)});
					}
					controller = new ModelEditorViewController(loader.ModelApplication, loader.FileModelStore);
					controller.IsStandalone = true;
					controller.CustomLoadModel += new EventHandler<CustomLoadModelEventArgs>(controller_CustomLoadModel);
					modelEditorForm = new ModelEditorForm(controller, new SettingsStorageOnRegistry(@"Software\Developer Express\eXpressApp Framework\Model Editor"));
					modelEditorForm.Shown += new EventHandler(modelEditorForm_Shown);
					if(loader.FileModelStore != null) {
						modelEditorForm.SetCaption(loader.FileModelStore.Name);
					}
					Application.Run(modelEditorForm);
				}
				catch(Exception exception) {
					HandleException(exception);
				}
			}
		}
		static void modelEditorForm_Shown(object sender, EventArgs e) {
			if(controller.IsStandalone && !isModelLoaded) {
				controller.OpenAction.DoExecute();
			}
		}
		static void controller_CustomLoadModel(object sender, CustomLoadModelEventArgs e) {
			OpenModelForm openModelForm = new OpenModelForm();
			if(openModelForm.ShowDialog() == DialogResult.OK) {
				modelEditorForm.Refresh();
				StandaloneModelEditorModelLoader loader = new StandaloneModelEditorModelLoader();
				try {
					loader.LoadModel(openModelForm.TargetFileName, openModelForm.ModelDifferencesStorePath, openModelForm.DeviceSpecificDifferencesStoreName, openModelForm.AssembliesPath);
					isModelLoaded = true;
				}
				catch(Exception exception) {
					HandleException(exception);
					return;
				}
				e.ModelApplication = loader.ModelApplication;
				e.ModelDifferenceStore = loader.FileModelStore;
				modelEditorForm.SetCaption(loader.FileModelStore.Name);
			}
		}
	}
}
