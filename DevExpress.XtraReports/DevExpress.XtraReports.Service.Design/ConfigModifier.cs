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
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using DevExpress.Internal;
using DevExpress.XtraReports.Service.ConfigSections.Native;
using EnvDTE;
using Configuration = System.Configuration.Configuration;
namespace DevExpress.XtraReports.Service.Design {
	public class ConfigModifier {
		const string contract = "DevExpress.XtraReports.Service.IReportService";
		readonly ElementNames elementNames = new ElementNames("ReportServiceBehavior", "ReportServiceBinding");
		readonly Configuration configuration;
		readonly Project project;
		readonly string dbFile;
		readonly string rootNamespace;
		readonly string safeItemRootName;
		string serviceName;
		string ServiceName {
			get {
				if(serviceName == null)
					serviceName = !string.IsNullOrEmpty(rootNamespace)
						? rootNamespace + "." + safeItemRootName
						: safeItemRootName;
				return serviceName;
			}
		}
		public ConfigModifier(Project project, Configuration configuration, string dbFile, string rootNamespace, string safeItemRootName) {
			this.project = project;
			this.configuration = configuration;
			this.dbFile = dbFile;
			this.rootNamespace = rootNamespace;
			this.safeItemRootName = safeItemRootName;
		}
		public void Modify() {
			string connection = DbEngineDetector.PatchConnectionString("XpoProvider=MSSqlServer;Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\" + dbFile + ";Integrated Security=True;User Instance=True;Connect Timeout=120");
			SetValue(configuration.ConnectionStrings.ConnectionStrings, ConfigurationDefaultConstants.ConfigSectionName, connection);
			ConfigureBindings();
			ConfigureBehaviors();
			ConfigureServices();
			configuration.Save();
			ForceSaveProjectItem(project.DTE.Solution, configuration.FilePath);
		}
		void ConfigureBindings() {
			ConfigurationSectionGroup group = configuration.GetSectionGroup("system.serviceModel");
			if(group == null)
				return;
			var bindings = (BindingsSection)group.Sections["bindings"];
			if(!bindings.BasicHttpBinding.Bindings.ContainsKey(elementNames.BindingName)) {
				int bufferSize = Data.Utils.ServiceModel.ServiceClientFactory<object, object>.DefaultBindingBufferSizeLimit;
				var binding = new BasicHttpBindingElement(elementNames.BindingName) {
					MaxReceivedMessageSize = bufferSize,
					TransferMode = TransferMode.Streamed
				};
				binding.ReaderQuotas.MaxArrayLength = bufferSize;
				bindings.BasicHttpBinding.Bindings.Add(binding);
			}
		}
		void ConfigureServices() {
			ConfigurationSectionGroup group = configuration.GetSectionGroup("system.serviceModel");
			if(group == null)
				return;
			var services = (ServicesSection)group.Sections["services"];
			if(services.Services.ContainsKey(ServiceName))
				return;
			var service = new ServiceElement(ServiceName) {
				BehaviorConfiguration = elementNames.BehaviorName
			};
			var endpoint = new ServiceEndpointElement {
				Binding = "basicHttpBinding",
				BindingConfiguration = elementNames.BindingName,
				Contract = contract
			};
			var mexEndpoint = new ServiceEndpointElement {
				Binding = "mexHttpBinding",
				Address = new Uri("mex", UriKind.Relative),
				Contract = "IMetadataExchange"
			};
			service.Endpoints.Add(endpoint);
			service.Endpoints.Add(mexEndpoint);
			services.Services.Add(service);
		}
		void ConfigureBehaviors() {
			ConfigurationSectionGroup group = configuration.GetSectionGroup("system.serviceModel");
			if(group == null)
				return;
			var behaviors = group.Sections["behaviors"] as BehaviorsSection;
			if(behaviors != null && !behaviors.ServiceBehaviors.ContainsKey(elementNames.BehaviorName)) {
				var behavior = new ServiceBehaviorElement(elementNames.BehaviorName);
				var metadataElement = new ServiceMetadataPublishingElement { HttpGetEnabled = true };
				behavior.Add(metadataElement);
				var debugElement = new ServiceDebugElement { IncludeExceptionDetailInFaults = false };
				behavior.Add(debugElement);
				behaviors.ServiceBehaviors.Add(behavior);
			}
		}
		static void SetValue(ConfigurationSectionGroupCollection settings, string groupName, string sectionName, ConfigurationSection section) {
			if(settings[groupName] == null) {
				var group = new ConfigurationSectionGroup();
				settings.Add(groupName, group);
				group.Sections.Add(sectionName, section);
			}
		}
		static void SetValue(ConnectionStringSettingsCollection settings, string name, string value) {
			if(settings[name] == null) {
				settings.Add(new ConnectionStringSettings(name, value));
			}
		}
		static void ForceSaveProjectItem(Solution solution, string path) {
			ProjectItem projectItem = solution.FindProjectItem(path);
			if(projectItem != null && !projectItem.Saved) {
				projectItem.Save();
			}
		}
	}
}
