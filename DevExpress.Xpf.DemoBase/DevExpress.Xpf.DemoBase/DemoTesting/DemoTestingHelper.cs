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
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace DevExpress.Xpf.DemoBase.DemoTesting {
	public static class CheckDirectivesHelper {
		class ErrorInfo {
			public string Text { get; set; }
			public int Line { get; set; }
		}
		public static string CheckXAML(string xaml) {
			IList<ErrorInfo> errors = GetErrors(xaml);
			if(errors.Count == 0)
				return null;
			StringBuilder result = new StringBuilder();
			result.Append("wpf2sl directives detected: ");
			foreach(ErrorInfo info in errors) {
				result.Append(string.Format("'{0}' (line {1}), ", info.Text, info.Line));
			}
			return result.ToString();
		}
		static IList<ErrorInfo> GetErrors(string source) {
			List<ErrorInfo> result = new List<ErrorInfo>();
			string[] lines = source.Split('\n');
			for(int i = 0; i < lines.Length; i++) {
				string error = GetXamlError(lines[i]);
				if(!string.IsNullOrEmpty(error))
					result.Add(new ErrorInfo() { Line = i + 1, Text = error });
			}
			return result;
		}
		static string[] XamlDirectives = new string[] {
			"<!--BEGIN WPF-->",
			"<!--END WPF-->",
			"<!--ONLY SL",
			"ONLY SL-->",
			"<!--BEGIN REPLACE",
			"<!--END REPLACE",
			"<!--BEGIN TOSLONLY REPLACE",
			"<!--END TOSLONLY REPLACE",
			"<!--PROCESS SETTERS WITH BINDING-->",
		};
		static string GetXamlError(string line) {
			foreach(string directive in XamlDirectives) {
				if(line.Contains(directive))
					return directive;
			}
			return null;
		}
	}
	public static class DemoTestingHelper {
		static object lastException = null;
		public static IWPFDemoTestService Service { get; private set; }
		internal static FrameworkElement RootElementInstance { get; set; }
		internal static string TestFixtureTypeName { get; set; }
		public static void TestComplete() {
			Service.TestComplete();
		}
		internal static void IAmAlive(string currentState) {
			Service.IAmAlive(currentState);
			System.Diagnostics.Debug.WriteLine(currentState);
		}
		public static void Log(string message, params object[] paremeters) {
			if (!IsLogging)
				return;
			if (Service == null)
				return;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(message);
			for(int i = 0; i < paremeters.Count() - 1; i++) {
				stringBuilder.Append(", ");
				stringBuilder.Append(paremeters[i]);
			}
			if(paremeters.Count() > 0) {
				stringBuilder.Append(", ");
				stringBuilder.Append(paremeters[paremeters.Count() - 1]);
			}
			stringBuilder.Append(" - ");
			stringBuilder.Append(DateTime.Now);
			stringBuilder.Append("\r\n");
			Logs += stringBuilder.ToString();
		}
		public static string Logs { get; set; }
		public static bool IsTesting { get; private set; }
		public static bool IsLogging { get; set; }
		static BaseDemoTestingFixture fixture;
		public static void StartTestingIfNeeded(IDemoBaseTesting demoBaseTesting, Assembly entryAssembly) {
			FrameworkElement root;
			root = Application.Current.MainWindow;
			StartTestingIfNeeded(demoBaseTesting, entryAssembly, root);
		}
		public static void StartTestingIfNeeded(IDemoBaseTesting demoBaseTesting, Assembly entryAssembly, FrameworkElement rootElementInstance) {
			if(demoBaseTesting == null || string.IsNullOrEmpty(DemoTestingHelper.TestFixtureTypeName)) return;
			RootElementInstance = rootElementInstance;
			try {
				Type fixtureType = entryAssembly.GetTypes().FirstOrDefault(t => t.Name == TestFixtureTypeName);
				if(fixtureType == null) {
					fixtureType = entryAssembly.GetType(TestFixtureTypeName);
				}
				fixture = (BaseDemoTestingFixture)Activator.CreateInstance(fixtureType);
				fixture.DemoBaseTesting = demoBaseTesting;
				IsTesting = true;
				System.Windows.Application.Current.DispatcherUnhandledException += (sender, e) => OnUnhandledException(sender, e.Exception);
				AppDomainHelper.AddUnhandledExceptionHandler(OnUnhandledException);
				fixture.Start();
			} catch(Exception ex) {
				MessageBox.Show(string.Format("Unable to load the '{0}' test fixture type:\n{1}", TestFixtureTypeName, ex.ToString()));
			}
		}
		static void OnUnhandledException(object sender, object exception) {
			if(exception == lastException) return;
			lastException = exception;
			Service.OnError("unhandled exception: " + GetExceptionDetails(exception));
		}
		static string GetExceptionDetails(object exceptionObject) {
			Exception exception = exceptionObject as Exception;
			if(exception == null) return "Unknown exception";
			StringBuilder s = new StringBuilder();
			for(Exception e = exception; e != null; e = e.InnerException)
				s.AppendLine(e.Message);
			s.AppendLine();
			s.AppendLine(exception.StackTrace);
			if(DemoTestingHelper.Logs != null)
				s.AppendLine(DemoTestingHelper.Logs);
			return s.ToString();
		}
		public static void ProcessArgs(string[] args) {
			foreach(string arg in args) {
				if(arg.Length == 0 || arg[0] == '-') continue;
				if(arg == "test") {
					PrepareServerSideTesting();
				} else {
					PrepareLocalTesting(arg);
				}
				break;
			}
		}
		public static void PrepareXBAPTesting(string serviceUri) {
			PrepareServerSideTesting(serviceUri);
		}
		public static void PrepareXBAPTesting(Type fixtureType) {
			if(fixtureType == null)
				PrepareServerSideTesting(ServiceHelper.ServiceUri);
			else
				PrepareLocalTesting(fixtureType.FullName);
		}
		static void PrepareLocalTesting(string fixtureTypeName) {
			SetService(new EmptyTestService(fixtureTypeName));
		}
		static void PrepareServerSideTesting(string serviceUri) {
			IWPFDemoTestService serviceCandidate = null;
			try {
				serviceCandidate = TestServiceHelper.GetTestService(serviceUri);
				SetService(TestServiceHelper.GetTestService(serviceUri));
				TestFixtureTypeName = Service.GetFixtureClassName();
			} catch { }
		}
		static void PrepareServerSideTesting() {
			PrepareServerSideTesting(ServiceHelper.ServiceUri);
		}
		static void SetService(IWPFDemoTestService serviceCandidate) {
			Service = serviceCandidate;
			TestFixtureTypeName = Service.GetFixtureClassName();
		}
	}
}
