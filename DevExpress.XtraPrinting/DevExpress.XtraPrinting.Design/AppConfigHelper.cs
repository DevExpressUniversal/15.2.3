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
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using EnvDTE;
using EnvDTE80;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign;
using System.IO;
using DevExpress.ReportServer.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.Client;
using System.Globalization;
using System.Web.UI.Design;
namespace DevExpress.XtraPrinting.Design {
	public class AppConfigHelper : DevExpress.XtraPrinting.Design.IAppConfigHelper {
		const string authenticationBindingName = "AuthenticationBinding";
		const string serviceBindingName = "serviceBinding";
		const string authenticationBehaviorExtensionName = "AuthenticationBehaviorExtension";
		const string authenticationBehaviorName = "AuthenticationBehavior";
		const string asyncAuthenticationContract = "DevExpress.ReportServer.ServiceModel.Client.IAuthenticationServiceAsync";
		const string reportServerContract = "DevExpress.ReportServer.ServiceModel.Client.IReportServerFacadeAsync";
		const string AppConfigFileName = "App.config";
		readonly string authenticationPattern = "_Authentication";
		readonly string useSSLPattern = "_SSL";
		readonly string formsAuthenticationPattern = "_Forms";
		readonly string winAuthenticationPattern = "_Win";
		readonly IServiceProvider provider;
		readonly ProjectItem projectItem;
		readonly Dictionary<string, string> elements = new Dictionary<string, string>();
		System.Configuration.Configuration configuration;
		string endpointNamePrefix;
		string currentBindingName;
		ConfigurationSectionGroup ServiceModelGroup { get { return configuration == null ? null : configuration.GetSectionGroup("system.serviceModel"); } }
		ClientSection ClientSection { get { return ServiceModelGroup == null ? null : ServiceModelGroup.Sections["client"] as ClientSection; } }
		Project Project { get { return projectItem.ContainingProject; } }
		Solution2 Solution { get { return (Solution2)projectItem.DTE.Solution; } }
		public AppConfigHelper(IServiceProvider provider) {
			this.provider = provider;
			projectItem = provider.GetService<ProjectItem>();
			configuration = GetConfiguration();
		}
		public void ConfigureEndpoints(string endpointNamePrefix, bool configureBehavioursExtention) {
			this.endpointNamePrefix = endpointNamePrefix;
			EnsureConfigFile();
			if(ServiceModelGroup == null)
				return;
			ConfigureEndpointsCore(endpointNamePrefix, configureBehavioursExtention);
			configuration.Save();
			Project.ProjectItems.AddFromFile(configuration.FilePath);
			ForceSaveProjectItem(configuration.FilePath);
		}
		public void ConfigureEndpoints(string endpointNamePrefix) {
			ConfigureEndpoints(endpointNamePrefix, true);
		}
		void ConfigureEndpointsCore(string endpointNamePrefix, bool configureBehavioursExtention) {
			if(configureBehavioursExtention)
				ConfigureBehavioursExtention(endpointNamePrefix);
			ConfigureEndpoint(true, false, false);
			ConfigureEndpoint(true, true, false);
			ConfigureEndpoint(true, false, true);
			ConfigureEndpoint(true, true, true);
			ConfigureEndpoint(false, false, false);
			ConfigureEndpoint(false, false, true);
		}
		void ConfigureEndpoint(bool isAuthenticationServiceContract, bool isWindowsAuthentication, bool useSSL) {
			ConfigureBindings(endpointNamePrefix, isAuthenticationServiceContract, isWindowsAuthentication, useSSL);
			ConfigureClient(isAuthenticationServiceContract, isWindowsAuthentication, useSSL);
		}
		void ConfigureClient(bool isAuthenticationServiceContract, bool isWindowsAuthentication, bool useSSL) {
			var configurationName = GetUniqueEndpointName(GetBaseEndpointName(endpointNamePrefix, isAuthenticationServiceContract, isWindowsAuthentication, useSSL));
			var endpointElement = new ChannelEndpointElement {
				Name = configurationName,
				Binding = "basicHttpBinding"
			};
			if(isAuthenticationServiceContract) {
				endpointElement.Contract = asyncAuthenticationContract;
			} else {
				endpointElement.Contract = reportServerContract;
			}
			endpointElement.BindingConfiguration = currentBindingName;
			string behaviorConfiguration;
			if(elements.TryGetValue(authenticationBehaviorName, out behaviorConfiguration))
				endpointElement.BehaviorConfiguration = behaviorConfiguration;
			AddEndpointElement(endpointElement);
		}
		void ConfigureBindings(string endpointNamePrefix, bool isAuthenticationServiceContract, bool isWindowsAuthentication, bool useSSL) {
			string elementName = GetBindingName(endpointNamePrefix, isAuthenticationServiceContract, isWindowsAuthentication, useSSL);
			var bindingElement = new BasicHttpBindingElement(elementName);
			if(!isAuthenticationServiceContract) {
				bindingElement.MaxReceivedMessageSize = 33554432;
				var readerQuotas = bindingElement.ReaderQuotas;
				readerQuotas.MaxArrayLength =  8388608;
				readerQuotas.MaxBytesPerRead = 4096;
				readerQuotas.MaxNameTableCharCount = 16384;
				readerQuotas.MaxStringContentLength = 8388608;
			}
			bindingElement.AllowCookies = false;
			if(isWindowsAuthentication) {
				bindingElement.Security.Mode = useSSL ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.TransportCredentialOnly;
				bindingElement.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
			} else {
				bindingElement.Security.Mode = useSSL ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.None;
			}
			var keyElementName = isWindowsAuthentication
				? authenticationBindingName
				: serviceBindingName;
			AddBinding(bindingElement, keyElementName);
			currentBindingName = elementName;
		}
		void ConfigureBehavioursExtention(string prefix) {
			var extensions = (ExtensionsSection)ServiceModelGroup.Sections["extensions"];
			if(!extensions.BehaviorExtensions.ContainsKey(authenticationBehaviorExtensionName)) {
				var extension = new ExtensionElement(authenticationBehaviorExtensionName, typeof(FormsAuthenticationBehaviorExtension).AssemblyQualifiedName);
				extensions.BehaviorExtensions.Add(extension);
			}
			elements[authenticationBehaviorExtensionName] = authenticationBehaviorExtensionName;
			var behaviors = (BehaviorsSection)ServiceModelGroup.Sections["behaviors"];
			var behaviorName = authenticationBehaviorName;
			if(!behaviors.EndpointBehaviors.ContainsKey(behaviorName)) {
				var endpointBehaviour = new EndpointBehaviorElement(behaviorName);
				behaviors.EndpointBehaviors.Add(endpointBehaviour);
				endpointBehaviour.Add(new FormsAuthenticationBehaviorExtension());
			}
			elements[authenticationBehaviorName] = behaviorName;
		}
		public IList<string> GetEndpointNames(RemoteDocumentSourceType documentSourceType) {
			if(ClientSection == null) {
				return new List<string>();
			}
			return ClientSection
				.Endpoints
				.Cast<ChannelEndpointElement>()
				.Where(x => x.Contract == reportServerContract)
				.Select(x => x.Name).ToArray();
		}
		string GetBaseEndpointName(string endpointNamePrefix, bool isAuthenticationServiceContract, bool isWindowsAuthentication, bool useSSL) {
			string configurationName = endpointNamePrefix;
			if(isAuthenticationServiceContract) {
				configurationName += authenticationPattern;
				if(isWindowsAuthentication)
					configurationName += winAuthenticationPattern;
				else
					configurationName += formsAuthenticationPattern;
			}
			if(useSSL)
				configurationName += useSSLPattern;
			return configurationName;
		}
		public string GetUniqueEndpointName(string baseName) {
			if(ClientSection == null) {
				return baseName;
			}
			var endpoints = ClientSection.Endpoints.Cast<ChannelEndpointElement>();
			if(!endpoints.Any(x => x.Name == baseName)) {
				return baseName;
			}
			for(int i = 1; ; i++) {
				var name = baseName + i;
				if(!endpoints.Any(x => x.Name == name)) {
					return name;
				}
			}
			throw new InvalidOperationException();
		}
		string GetBindingName(string endpointNamePrefix, bool isAuthenticationServiceContract, bool isWindowsAuthentication, bool useSSL) {
			string name = endpointNamePrefix;
			if(isAuthenticationServiceContract)
				name += authenticationPattern;
			name += isWindowsAuthentication ? winAuthenticationPattern : formsAuthenticationPattern;
			if(useSSL)
				name += useSSLPattern;
			return name;
		}
		void AddBinding(BasicHttpBindingElement bindingElement, string rootBindingName) {
			var bindingsSection = (BindingsSection)ServiceModelGroup.Sections["bindings"];
			var bindings = bindingsSection.BasicHttpBinding.Bindings;
			if(bindings.ContainsKey(bindingElement.Name))
				bindings[bindingElement.Name] = bindingElement;
			else
				bindings.Add(bindingElement);
			elements[rootBindingName] = bindingElement.Name;
		}
		void AddEndpointElement(ChannelEndpointElement endpoint) {
			var endpointName = string.Format(CultureInfo.InvariantCulture, "contractType:{0};name:{1}", endpoint.Contract, endpoint.Name);
			var endpoints = ClientSection.Endpoints;
			if(endpoints.ContainsKey(endpointName))
				endpoints[endpointName] = endpoint;
			else
				endpoints.Add(endpoint);
		}
		void ForceSaveProjectItem(string path) {
			ProjectItem projectItem = Solution.FindProjectItem(path);
			if(projectItem != null && !projectItem.Saved) {
				projectItem.Save();
			}
		}
		string GetProjectDirectory() {
			return Project == null ? string.Empty : Path.GetDirectoryName(Project.FullName);
		}
		System.Configuration.Configuration GetConfiguration() {
			var configFilePath = Path.Combine(GetProjectDirectory(), AppConfigFileName);
			if(provider != null) {
				var webApplication = provider.GetService<IWebApplication>();
				if(webApplication != null) {
					var webConfiguration = webApplication.OpenWebConfiguration(true);
					configFilePath = webConfiguration.FilePath;
				}
			}
			if(!File.Exists(configFilePath)) {
				return null;
			}
			var machineConfiguration = System.Configuration.ConfigurationManager.OpenMachineConfiguration();
			var fileMap = new ExeConfigurationFileMap {
				MachineConfigFilename = machineConfiguration.FilePath,
				ExeConfigFilename = configFilePath
			};
			return System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
		}
		void EnsureConfigFile() {
			if(configuration != null) {
				return;
			}
			var filePath = Path.Combine(GetProjectDirectory(), AppConfigFileName);
			if(File.Exists(filePath)) {
				return;
			}
			var vsLanguage = projectItem != null
				&& projectItem.FileCodeModel != null
				&& string.Equals(projectItem.FileCodeModel.Language, CodeModelLanguageConstants.vsCMLanguageVB, StringComparison.Ordinal)
				? "VisualBasic"
				: "CSharp";
			var configTemplate = Solution.GetProjectItemTemplate("AppConfig.zip", vsLanguage);
			Project.ProjectItems.AddFromTemplate(configTemplate, filePath);
			configuration = GetConfiguration();
		}
	}
}
