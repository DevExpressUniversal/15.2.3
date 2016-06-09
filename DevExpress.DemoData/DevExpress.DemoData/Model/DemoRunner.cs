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

using DevExpress.DemoData.Core;
using DevExpress.DemoData.Helpers;
using DevExpress.Internal.DXWindow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.DemoData.Model {
	public enum CallingSite {
		DemoCenter, WpfDemoLauncher, WinDemoChooser
	}
	interface IDemoRunner {
		void Log(byte kind, byte platform, string action);
		void OpenLink(string uri);
		string OpenUri(string uri);
		bool PathExists(string path);
		string GetDemoFullPath(string path);
		string StartViaShell(string path, string[] arguments);
		string LaunchVS(string solutionPath, string[] files);
	}
	class DefaultDemoRunner : IDemoRunner {
		public void Log(byte kind, byte platform, string action) {
		}
		public void OpenLink(string uri) {
			DocumentPresenter.OpenTabLink(uri, OpenLinkType.Smart);
		}
		public string StartViaShell(string path, string[] arguments) {
			return StartApplicationHelper.Start(path, arguments, false);
		}
		public string OpenUri(string uri) {
			return StartApplicationHelper.Start(uri, null, true);
		}
		public bool PathExists(string path) {
			return path != null && (File.Exists(path) || Directory.Exists(path));
		}
		public string GetDemoFullPath(string demoName) {
			return StartApplicationHelper.GetDemoFullPath(demoName);
		}
		public string LaunchVS(string solutionPath, string[] files) {
			return StartApplicationHelper.StartVS(solutionPath, files);
		}
	}
	public interface IDemoRunnerMessageBox {
		MessageBoxHelperResult Show(string message, bool isError, bool showIgnoreButton);
	}
	public class DemoRunner {
		internal static IDemoRunner Runner = new DefaultDemoRunner();
		public static void TryStartExecutableAndShowErrorMessage(IExecutable executable, CallingSite from, IDemoRunnerMessageBox messageBox) {
			var requirements = TryStartExecutable(executable, from).OrderByDescending(r => r.Type);
			foreach(var r in requirements) {
				var canIgnore = ShowMessageBox(r.Message, r.Type, messageBox);
				if(!canIgnore || r.Type == RequirementCheckResultType.Error) {
					return;
				}
			}
		}
		static bool ShowMessageBox(string message, RequirementCheckResultType type, IDemoRunnerMessageBox messageBox) {
			var result = DevExpress.DemoData.Helpers.MessageBoxHelperResult.OK;
			DevExpress.DemoData.Helpers.BackgroundHelper.DoInMainThread(() => {
				var isError = type == RequirementCheckResultType.Error;
				result = messageBox.Show(message, isError, false);
			});
			if(result == DevExpress.DemoData.Helpers.MessageBoxHelperResult.Ignore || type != RequirementCheckResultType.Error) {
				return true;
			}
			return false;
		}
		static List<RequirementCheckResult> TryStartExecutable(IExecutable executable, CallingSite from) {
			var failedRequirements = executable.Requirements
				.Select(r => r.GetResult())
				.Where(r => r.Type != RequirementCheckResultType.Satisfied)
				.ToList();
			if(failedRequirements.Any())
				return failedRequirements;
			string site = from == CallingSite.DemoCenter ? "FromDemoCenter:" : "FromDemoLauncher:";
			string doEventFormattedMessage = string.Format(executable.DoEventMessage, site);
			string message = OpenLinkOrRunVS(executable.LaunchPath, executable.Arguments);
			if(!string.IsNullOrEmpty(message)) {
				string xamlMessage = "<Paragraph>" + message + "</Paragraph>";
				return new List<RequirementCheckResult> { new RequirementCheckResult(xamlMessage, RequirementCheckResultType.Error) };
			}
			return new List<RequirementCheckResult>();
		}
		public static bool OpenCSSolution(SimpleModule module, CallingSite from, bool showMessageBox = true) {
			return OpenCSSolution(module.Demo, from, showMessageBox, module.AssociatedFiles);
		}
		public static bool OpenVBSolution(SimpleModule module, CallingSite from, bool showMessageBox = true) {
			return OpenVBSolution(module.Demo, from, showMessageBox, module.AssociatedFiles);
		}
		public static bool OpenCSSolution(ReallifeDemo demo, CallingSite from, bool showMessageBox = true, string[] openFiles = null) {
			return OpenSolution(demo.CsSolutionPath, GetOpenSolutionMessage(demo.Name, from, false), demo.Platform, showMessageBox, FilterFilesByExtension(openFiles, ".vb"));
		}
		public static bool OpenCSSolution(Demo demo, CallingSite from, bool showMessageBox = true, string[] openFiles = null) {
			return OpenSolution(demo.CsSolutionPath, GetOpenSolutionMessage(demo.Name, from, false), demo.Product.Platform, showMessageBox, FilterFilesByExtension(openFiles, ".vb"));
		}
		public static bool OpenVBSolution(ReallifeDemo demo, CallingSite from, bool showMessageBox = true, string[] openFiles = null) {
			return OpenSolution(demo.VbSolutionPath, GetOpenSolutionMessage(demo.Name, from, true), demo.Platform, showMessageBox, FilterFilesByExtension(openFiles, ".cs"));
		}
		public static bool OpenVBSolution(Demo demo, CallingSite from, bool showMessageBox = true, string[] openFiles = null) {
			return OpenSolution(demo.VbSolutionPath, GetOpenSolutionMessage(demo.Name, from, true), demo.Product.Platform, showMessageBox, FilterFilesByExtension(openFiles, ".cs"));
		}
		internal static bool OpenSolution(string path, string message, Platform platform, bool showMessageBox, string[] openFiles) {
			return OpenLinkOrRunExecutableAndShowError(path, showMessageBox, openFiles);
		}
		static string[] FilterFilesByExtension(string[] files, string excludeExtension) {
			if(files == null)
				return null;
			return files.Where(f => !f.ToLower().EndsWith(excludeExtension)).ToArray();
		}
		static string GetOpenSolutionMessage(string name, CallingSite site, bool isVb) {
			string lang = isVb ? "VB" : "CS";
			string siteStr = site == CallingSite.DemoCenter ? "DemoCenter" : "DemoLauncher";
			return string.Format("Open{0}SolutionFrom{1}:{2}", lang, siteStr, name);
		}
		static bool OpenLinkOrRunExecutableAndShowError(string path, bool showMessageBox, string[] filesToOpen) {
			if(filesToOpen == null)
				filesToOpen = new string[0];
			filesToOpen = filesToOpen
				.Select(Runner.GetDemoFullPath)
				.Where(Runner.PathExists)
				.ToArray();
			string message = OpenLinkOrRunVS(path, filesToOpen);
			if(!string.IsNullOrEmpty(message)) {
				if(showMessageBox) {
					MessageBox.Show(string.Format("File {0} not found.", path), "Error", MessageBoxButton.OK, MessageBoxImage.Error); 
				}
				return false;
			}
			return true;
		}
		static string OpenLinkOrRunVS(string path, string[] filesToOpen) {
			if(ModuleUrlHelper.IsUrl(path)) {
				string correctModuleName = ModuleUrlHelper.CorrectURLByModuleName(path);
				string fileName = string.IsNullOrEmpty(correctModuleName) ? string.Empty : Runner.GetDemoFullPath(correctModuleName);
				if(!Runner.PathExists(fileName))
					return string.Format("File {0} not found.", fileName);
				try {
					fileName = WebDevServerHelper.GetProcessedDemoPath(fileName, path, false);
				} catch(Exception) {
					return "Can't start the Web Server.";
				}
				string virtualPath = filesToOpen.Any() ? filesToOpen.Aggregate("", (l, r) => l + "/" + r) : "";
				return Runner.OpenUri(fileName + virtualPath.TrimStart('/'));
			}
			if(path.StartsWith("OpenLink:")) {
				Runner.OpenLink(path.Substring("OpenLink:".Length));
				return null;
			}
			if(Path.GetExtension(path).ToLower() == ".exe")
				return Runner.StartViaShell(path, filesToOpen);
			return Runner.LaunchVS(path, filesToOpen);
		}
		public static void RunProject(string solutionPath, string[] files) {
			Runner.LaunchVS(solutionPath, files);
		}
	}
}
