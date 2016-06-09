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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraLayout;
using System.IO;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraSplashScreen;
using System.Net;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
namespace DevExpress.DXperience.Demos.CodeDemo {
	[ToolboxItem(false)]
	public partial class CodeExampleControlBase :TutorialControlBase {
		public CodeExampleControlBase() {
			PathToExampleRunner = TryGetPathToExampleRunner();
			MSBuildPath = TryGetPathToMSBuild();
			if(PathToExampleRunner == string.Empty || MSBuildPath == string.Empty){
			}
			InitializeComponent();
			CodeExamplesURLs = FillCodeExamples();
			FillTreeListNode();
			webClient = new WebClient();
			webClient.DownloadFileCompleted += OnDownloadExampleCompleted;
		}
		public CodeExampleControlBase(string Uri) : this() {
			CodeExamplesURLs = new CodeExampleURI[] { new CodeExampleURI(Uri, "Test", "Test") };
			FillTreeListNode();
}
		string PathToExampleRunner;
		protected CodeExampleURI[] CodeExamplesURLs;
		WebClient webClient;
		string MSBuildPath;
		string TryGetPathToMSBuild() {
			string path = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + "MSBuild.exe";
			if(File.Exists(path)) return path;
			return "";
		}
		string TryGetPathToExampleRunner() {
			string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\DevExpress\CodeCentral Example Runner\DXCodeCentralExampleRunner.exe";
			if(File.Exists(path)) return path;
			path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\DevExpress\CodeCentral Example Runner\DXCodeCentralExampleRunner.exe";
			if(File.Exists(path)) return path;
			if(MessageBox.Show("Would you like to download DXCodeCentralExampleRunner?"
					, "DXCodeCentralExampleRunner", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
				using(WebClient Client = new WebClient()) {
					SplashScreenManager.ShowDefaultProgressSplashScreen("Download DevExpress.ExampleRunner.Setup.msi");
					SplashScreenManager.SetDefaultProgressSplashScreenValue(false, 10);
					FileInfo file = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "DevExpress.ExampleRunner.Setup.msi"));
					SplashScreenManager.SetDefaultProgressSplashScreenValue(false, 30);
					Client.DownloadFile("https://www.devexpress.com/Support/Center/Attachment/GetExampleRunner", file.FullName);
					SplashScreenManager.SetDefaultProgressSplashScreenValue(false, 100);
					SplashScreenManager.CloseDefaultSplashScreen();
					Process p = Process.Start(file.FullName);
					p.WaitForExit();
				}
			}
			path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\DevExpress\CodeCentral Example Runner\DXCodeCentralExampleRunner.exe";
			if(File.Exists(path)) return path;
			path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\DevExpress\CodeCentral Example Runner\DXCodeCentralExampleRunner.exe";
			if(File.Exists(path)) return path;
			return "";
		} 
		protected virtual CodeExampleURI[] FillCodeExamples() {
			CodeExampleURI[] result = new CodeExampleURI[] {};
			return result;
		}
		void FillTreeListNode() {
			foreach(CodeExampleURI item in CodeExamplesURLs) {
				codeTreeList.AppendNode(new object[] { item.ExampleName, item.Uri, item.Description }, null);
			}
		}
	   string GetRandomDirectory() {
			string result = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			DirectoryInfo dInfo = Directory.CreateDirectory(result);
			return dInfo.FullName;
		}
	   private void simpleButton1_Click(object sender, EventArgs e) {
		   rootContainer.Controls.Clear();
		   if(codeTreeList.FocusedNode == null) return;
		   StartDownloadExample(codeTreeList.FocusedNode);
	   }
	   void StartDownloadExample(TreeListNode focused) {
		   string uriDxSample = focused.GetValue(1) as string;
		   Uri uriToDxSample;
		   if(!Uri.TryCreate(uriDxSample, UriKind.Absolute, out uriToDxSample)) return;
		   codeExampleName.Text = focused.GetValue(0) as string;
		   string pathToSample = Path.GetTempPath() + "\\dxSample.dxSample";
		   SplashScreenManager.ShowDefaultProgressSplashScreen("Download");
		   SplashScreenManager.SetDefaultProgressSplashScreenValue(false, 0);
		   webClient.DownloadFileAsync(uriToDxSample, pathToSample);
	   }
	   void OnDownloadExampleCompleted(object sender, AsyncCompletedEventArgs e) {
		   SplashScreenManager.SetDefaultProgressSplashScreenValue(false, 33);
		   string tempPath = GetRandomDirectory();
		   string pathToSLN = GetPathToSln(Path.GetTempPath() + "\\dxSample.dxSample");
		   if(pathToSLN == String.Empty) return;
		   CompileExample(tempPath, pathToSLN);
		   SplashScreenManager.SetDefaultProgressSplashScreenValue(false, 66);
		   CreateInstanceAndAddToPanel(tempPath);
		   SplashScreenManager.CloseDefaultSplashScreen();
	   }
	   void CompileExample(string tempPath, string pathToSLN) {
		   string arguments = String.Format(@"""{0}"" /t:Build /p:OutDir={1}", pathToSLN, tempPath);
		   ProcessStartInfo psi = new ProcessStartInfo(MSBuildPath, arguments) {
			   CreateNoWindow = true,
			   UseShellExecute = false,
		   };
		   Process process = Process.Start(psi);
		   process.WaitForExit();
	   }
	   void CreateInstanceAndAddToPanel(string tempPath) {
		   string pathToExe = Directory.GetFiles(tempPath, "*.exe")[0];
		   Assembly assembly = Assembly.LoadFile(pathToExe);
		   Type[] types = assembly.GetExportedTypes();
		   Form f1 = assembly.CreateInstance(types.First(q => typeof(Form).IsAssignableFrom(q)).FullName) as Form;
		   SplashScreenManager.SetDefaultProgressSplashScreenValue(false, 80);
		   ArrayList controls = new ArrayList(f1.Controls);
		   foreach(Control control in controls) {
			   control.Parent = rootContainer;
		   }
		   SplashScreenManager.SetDefaultProgressSplashScreenValue(false, 100);
		   f1.Dispose();
	   }
	   string GetPathToSln(string pathToDxSample) {
		   string output = CallExampleRunner(pathToDxSample);
		   if(output == String.Empty) return String.Empty;
		   DirectoryInfo dirInfo = new DirectoryInfo(output);
		   FileInfo[] fi = dirInfo.GetFiles("*.sln", SearchOption.AllDirectories);
		   if(fi.Count() == 0) return String.Empty;
		   return fi[0].FullName;
	   }
	   string CallExampleRunner(string pathToDxSample) {
		   ProcessStartInfo psi = new ProcessStartInfo(PathToExampleRunner, String.Format(@"/silence {0} {1}", pathToDxSample, AssemblyInfo.Version)) {
			   CreateNoWindow = true,
			   WindowStyle = ProcessWindowStyle.Hidden,
			   UseShellExecute = false,
			   RedirectStandardOutput = true
		   };
		   Process process = Process.Start(psi);
		   process.WaitForExit();
		   string output = process.StandardOutput.ReadToEnd();
		   return output;
	   }
	}
	public class CodeExampleURI {
		public CodeExampleURI(string uri, string description, string exampleName) {
			this.Uri = uri;
			this.Description = description;
			this.ExampleName = exampleName;
		}
		private string exampleName;
		public string ExampleName {
			get {
				return exampleName;
			}
			set {
				exampleName = value;
			}
		}
		private string descriptionCore;
		public string Description {
			get {
				return descriptionCore;
			}
			set {
				descriptionCore = value;
			}
		}
		private string uriCore;
		public string Uri {
			get {
				return uriCore;
			}
			set {
				uriCore = value;
			}
		}
	}
}
