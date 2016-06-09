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
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.ReportServer.Printing;
using DevExpress.ReportServer.ServiceModel.Client;
using EnvDTE;
using VSLangProj;
namespace DevExpress.XtraPrinting.Design {
	public static class ReferenceHelper {
		public static void AddReference(IServiceProvider provider, Assembly assembly) {
			ProjectItem projectItem = (ProjectItem)provider.GetService(typeof(ProjectItem));
			AddReferences(projectItem, assembly);
		}
		public static void AddReferences(ProjectItem projectItem, params Assembly[] assemblies) {
			try {
				Project project = projectItem.ContainingProject;
				VSProject vsProject = (VSProject)project.Object;
				foreach(var assembly in assemblies) {
					vsProject.References.Add(assembly.GetName().Name);
				}
			} catch { }
		}
	}
	static class EndpointHelper {
		public static bool UseSSL(EndpointAddress address) {
			return UseSSL(address.Uri);
		}
		public static bool UseSSL(string address) {
			Uri uri = new Uri(address);
			return UseSSL(uri);
		}
		public static bool UseSSL(Uri uri) {
			return uri.Scheme == Uri.UriSchemeHttps;
		}
		public static Uri GetEndpointUri(string baseAddress, string serviceName) {
			var baseUri = new Uri(baseAddress, UriKind.Absolute);
			return new Uri(baseUri, serviceName);
		}
		public static EndpointAddress GetEndpointAddress(string baseAddress, string serviceName) {
			return new EndpointAddress(GetEndpointUri(baseAddress, serviceName));
		}
	}
	internal class ConfigFileHelper {
		string fileName;
		string filePath = null;
		string PreviousFilePath {
			get {
				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
				return assembly != null ? Path.Combine(Path.GetDirectoryName(assembly.Location), fileName) : string.Empty;
			}
		}
		string FilePath {
			get {
				if(filePath == null) {
					string folderPath = GetFolderPath();
					filePath = !string.IsNullOrEmpty(folderPath) ? Path.Combine(folderPath, fileName) : string.Empty;
				}
				return filePath;
			}
		}
		public ConfigFileHelper(string fileName) {
			this.fileName = fileName;
		}
		public string GetLoadFilePath() {
			return File.Exists(PreviousFilePath) ? PreviousFilePath :
				File.Exists(FilePath) ? FilePath :
				string.Empty;
		}
		public string GetSaveFilePath() {
			return !string.IsNullOrEmpty(FilePath) ? FilePath :
				!string.IsNullOrEmpty(PreviousFilePath) ? PreviousFilePath :
				string.Empty;
		}
		public void DeletePreviousFile() {
			if(File.Exists(FilePath) && File.Exists(PreviousFilePath))
				try {
					File.Delete(PreviousFilePath);
				} catch {
				}
		}
		static string GetFolderPath() {
			try {
				var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"DevExpress/XtraPrinting.Design");
				if(!string.IsNullOrEmpty(path) && !Directory.Exists(path))
					Directory.CreateDirectory(path);
				return path;
			} catch {
				return string.Empty;
			}
		}
	}
	class ReportServerConnectionHelper {
		FormsAuthenticationEndpointBehavior behavior;
		public void CreateClientAndPing(string url, bool isWindowsAuthentication, Action<IReportServerClient, AsyncCompletedEventArgs> afterPing) {
			EndpointAddress endpointAddress = EndpointHelper.GetEndpointAddress(url, "ReportServerFacade.svc");
			var factory = new ReportServerClientFactory(endpointAddress);
			factory.ChannelFactory.Endpoint.Behaviors.Add(behavior);
			IReportServerClient client = factory.Create();
			client.Ping(args => afterPing(client, args), null);
		}
		public void Login(string url, AuthenticationType authenticationType, string userName, string password, Action<ScalarOperationCompletedEventArgs<bool>> onLoginCompleted) {
			var endpointAddress = EndpointHelper.GetEndpointAddress(url,
				authenticationType == AuthenticationType.Windows ? "WindowsAuthentication/AuthenticationService.svc" : "AuthenticationService.svc");
			behavior = new FormsAuthenticationEndpointBehavior();
			AuthenticationServiceClientFactory factory = new AuthenticationServiceClientFactory(endpointAddress, authenticationType);
			factory.ChannelFactory.Endpoint.Behaviors.Add(behavior);
			var client = factory.Create();
			client.Login(userName, password, null, onLoginCompleted);
		}
	}
}
