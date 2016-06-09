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
using System.Xml.Linq;
using DevExpress.Web.Internal;
using System.Reflection;
using EnvDTE;
using System.Web.UI.Design;
using System.Collections.Specialized;
namespace DevExpress.Web.Design {
	public class WebConfigManager {
		string configPhysicalPath = string.Empty;
		public static Dictionary<string, string> ConfigHashes = new Dictionary<string, string>();
		static Dictionary<string, DateTime> LastConfigTrackHits = new Dictionary<string, DateTime>();
		readonly TimeSpan TrackingDelay = new TimeSpan(0, 0, 1);
		public const string FileName = "~/web.config";
		const string HttpRuntimeSectionPath = "system.web/httpRuntime";
		protected internal WebConfigXmlHelper XmlHelper { get; set; }
		protected IServiceProvider ServiceProvider { get; set; }
		public WebConfigManager(IServiceProvider provider) {
			ServiceProvider = provider;
			XmlHelper = new WebConfigXmlHelper(new System.Xml.Linq.XDocument());
			if(ServiceProvider != null)
				InitializePhysicalPath();
		}
		public bool IsConfigTrackOutdated() {
			if(LastConfigTrackHits.ContainsKey(this.configPhysicalPath) &&
				DateTime.Now - LastConfigTrackHits[this.configPhysicalPath] < TrackingDelay) {
				LastConfigTrackHits[this.configPhysicalPath] = DateTime.Now;
				return false;
			}
			return true;
		}
		public bool IsHashChanged() {
			if(!ConfigHashes.ContainsKey(this.configPhysicalPath))
				return true;
			string configHash = DesignUtils.GetHash(XmlHelper.Document.ToString());
			if(configHash == ConfigHashes[this.configPhysicalPath]) {
				LastConfigTrackHits[this.configPhysicalPath] = DateTime.Now;
				return false;
			}
			return true;
		}
		public void UpdateConfigModificationsTrackingInfo() {
			string configHash = DesignUtils.GetHash(XmlHelper.Document.ToString());
			ConfigHashes[this.configPhysicalPath] = configHash;
			LastConfigTrackHits[this.configPhysicalPath] = DateTime.Now;
		}
		public System.Configuration.Configuration GetConfiguration(bool readOnly) {
			IWebApplication webApplication = (IWebApplication)ServiceProvider.GetService(typeof(IWebApplication));
			return webApplication.OpenWebConfiguration(readOnly);
		}
		public System.Configuration.Configuration GetConfiguration() {
			return GetConfiguration(false);
		}
		public NameValueCollection GetRuntimeSectionParametersFromWebConfig() {
			System.Configuration.Configuration config = GetConfiguration();
			System.Web.Configuration.HttpRuntimeSection runtimeSection = config.GetSection(HttpRuntimeSectionPath) as System.Web.Configuration.HttpRuntimeSection;
			NameValueCollection parameters = new NameValueCollection();
			if(runtimeSection != null) {
				try {
					parameters.Add("maxRequestLength", runtimeSection.MaxRequestLength.ToString());
					parameters.Add("executionTimeout", runtimeSection.ExecutionTimeout.TotalSeconds.ToString());
				}
				catch { }
			}
			else
				parameters = null;
			return parameters;
		}
		public NameValueCollection GetRequestLimitsSectionParametersFromWebConfig() {
			var node = XmlHelper["system.webServer/security/requestFiltering/requestLimits"];
			NameValueCollection parameters = new NameValueCollection();
			var attr = node.Attributes("maxAllowedContentLength").FirstOrDefault();
			if(attr != null)
				parameters.Add("maxAllowedContentLength", attr.Value);
			return parameters;
		}
		private void InitializePhysicalPath() {
			IWebApplication webApplication = (IWebApplication)ServiceProvider.GetService(typeof(IWebApplication));
			try {
				this.configPhysicalPath = EnvDTEHelper.GetPhysicalPathByUrl(FileName, webApplication);
			}
			catch {
				this.configPhysicalPath = string.Empty;
			}
		}
		public bool LoadWebConfigXmlDocument() {
			bool ret = true;
			IWebApplication webApplication = (IWebApplication)ServiceProvider.GetService(typeof(IWebApplication));
			try {
				if(!EnvDTEHelper.IsExistProjectItem(FileName, webApplication)) {
					AddWebConfigToSite();
					InitializePhysicalPath();
				}
				XmlHelper = new WebConfigXmlHelper(XDocument.Load(this.configPhysicalPath));
			}
			catch(Exception) {
				ret = false;
			}
			return ret;
		}
		public void AddWebConfigToSite() {
			EnvDTE.Project project = ((EnvDTE.ProjectItem)ServiceProvider.GetService(typeof(EnvDTE.ProjectItem))).ContainingProject;
			ProjectItem newItem = project.DTE.ItemOperations.AddNewItem(
				"Web Developer Project Files\\Visual C#\\Web Configuration File", "web.config");
			if(newItem.Document != null)
				newItem.Document.Close(vsSaveChanges.vsSaveChangesYes);
		}
		public void SaveWebConfig() {
			DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));
			object webConfigProjectItemObj = EnvDTEHelper.GetProjectItemByPath(FileName, ServiceProvider);
			if(webConfigProjectItemObj != null) {
				EnvDTE.ProjectItem webConfigProjectItem = webConfigProjectItemObj as EnvDTE.ProjectItem;
				ProjectItemState projItemState = EnvDTEHelper.GetProjectItemState(webConfigProjectItem);
				webConfigProjectItem.Open(Constants.vsViewKindPrimary).Activate();
				TextDocument textDocument = (TextDocument)dte.ActiveDocument.Object("TextDocument");
				EnvDTEHelper.InsertTextToDocument(textDocument, XmlHelper.Document.ToString());
				webConfigProjectItem.Save("");
				EnvDTEHelper.SetStateForProjectItem(webConfigProjectItem, projItemState);
			}
		}
	}
	public class WebConfigXmlHelper {
		const string UploadProgressHttpHandlerPagePath = "ASPxUploadProgressHandlerPage.ashx";
		const string UploadProgressHttpHandlerName = "ASPxUploadProgressHandler";
		const string HttpModuleHandlerPagePath = "DX.ashx";
		const string NewSectionName = "add";
		const string LocationSectionName = "location";
		const string ConfigSectionsSectionName = "configSections";
		const string SectionGroupSectionName = "sectionGroup";
		const string SectionSectionName = "section";
		const string HttpHandlerIIS7ValidationSectionName = "validation";
		const string TypeAttrName = "type";
		const string NameAttrName = "name";
		const string RequirePermissionAttrName = "requirePermission";
		const string VerbAttrName = "verb";
		const string PathAttrName = "path";
		const string KeyAttrName = "key";
		const string ValueAttrName = "value";
		const string ValidateAttrName = "validate";
		const string ConfigSourceAttrName = "configSource";
		const string ValidationNodeAttrName = "validateIntegratedModeConfiguration";
		const string AppSettingsSection = "appSettings";
		const string AppSettingsSectionPath = AppSettingsSection + "/" + NewSectionName;
		const string AssembliesSectionPath = "system.web/compilation/assemblies";
		const string BrowserLinkParamName = "vs:EnableBrowserLink";
		public WebConfigXmlHelper(XDocument doc) {
			Document = doc;
		}
		public IEnumerable<XElement> this[params string[] sections] {
			get {
				if(sections.Length == 1 && sections[0].Contains('/'))
					return this[sections[0].Split('/')];
				if(sections.Length > 1 && sections[0] == ConfigurationSectionNames.ConfigurationSectionName)
					return this[sections.Skip(1).ToArray()];
				IEnumerable<XElement> result = RootElement;
				if(sections.Length == 1 && string.IsNullOrEmpty(sections[0]))
					return result;
				for(int i = 0; i < sections.Length; i++)
					result = result.Elements(NS + sections[i]);
				if(sections[0] != LocationSectionName && !result.Any())
					return this[LocationSectionName + "/" + string.Join("/", sections)];
				return result;
			}
		}
		public XDocument Document { get; set; }
		protected IEnumerable<XElement> RootElement { get { return Document.Elements(NS + ConfigurationSectionNames.ConfigurationSectionName); } }
		protected XNamespace NS { get { return Document.Root.GetDefaultNamespace(); } }
		protected Dictionary<string, string> SectionToAppSettingEquivalence = new Dictionary<string, string>() {
			{CompressionConfigurationSection.EnableCallbackCompressionAttribute, ConfigurationSettings.EnableCallbackCompressionKey},
			{CompressionConfigurationSection.EnableHtmlCompressionAttribute, ConfigurationSettings.EnableHtmlCompressionKey},
			{CompressionConfigurationSection.EnableResourceCompressionAttribute, ConfigurationSettings.EnableResourceCompressionKey},
			{CompressionConfigurationSection.EnableResourceMergingAttribute, ConfigurationSettings.EnableResourceMergingKey},
			{ThemesConfigurationSection.EnableThemesAssemblyAttribute, ConfigurationSettings.EnableThemesAssemblyKey},
			{ErrorsConfigurationSection.ErrorPageUrlAttribute, ConfigurationSettings.ErrorPageUrlKey}
		};
		protected List<Type> SectionTypes = new List<Type>() {
			typeof(ThemesConfigurationSection),
			typeof(CompressionConfigurationSection),
			typeof(SettingsConfigurationSection),
			typeof(ErrorsConfigurationSection)
		};
		public string GetDxAttributeValue(string attribute, string sectionName, object defaultValue) {
			return GetAttributeValue(attribute, ConfigurationSectionNames.WebSectionGroupName + "/" + sectionName, defaultValue);
		}
		public string GetAttributeValue(string attribute, string sectionPath, object defaultValue) {
			var attr = (from node in this[sectionPath]
						select node.Attribute(attribute)).FirstOrDefault();
			return attr != null && !string.IsNullOrEmpty(attr.Value) ? attr.Value : defaultValue.ToString();
		}
		public bool SetDxAttributeValue(string attribute, string sectionName, bool value) {
			return SetDxAttributeValue(attribute, sectionName, value.ToString().ToLower());
		}
		public bool SetDxAttributeValue(string attribute, string sectionName, string value) {
			return SetAttributeValue(attribute, ConfigurationSectionNames.WebSectionGroupName + "/" + sectionName, value);
		}
		public void ResetVSBrowserLink() {
			var newNode = this[AppSettingsSectionPath].FirstOrDefault(n => CheckAttributeValue(n, KeyAttrName, BrowserLinkParamName));
			if(newNode == null) {
				newNode = CreateNode(NewSectionName, new XAttribute(KeyAttrName, BrowserLinkParamName), new XAttribute(ValueAttrName, false));
				AppendNode(newNode, AppSettingsSection);
			} else {
				if(!newNode.Attributes().Any(a => a.Name == ValueAttrName) || newNode.Attribute(ValueAttrName).Value.ToLower() != "true")
					newNode.SetAttributeValue(ValueAttrName, false);
			}
		}
		public bool SetAttributeValue(string attribute, string sectionPath, bool value) {
			return SetAttributeValue(attribute, sectionPath, value.ToString().ToLower());
		}
		public bool SetAttributeValue(string attribute, string sectionPath, string value) {
			var section = this[sectionPath].FirstOrDefault();
			if(section == null){
				if(HasConfigSource(sectionPath))
					return false;
				string[] sections = sectionPath.Split('/');
				section = new XElement(sections[sections.Length - 1]);
				AppendNode(section, string.Join("/", sections, 0, sections.Length - 1));
			}
			var attr = section.Attribute(attribute);
			if(attr == null) {
				attr = new XAttribute(attribute, value);
				section.Add(attr);
			}
			else if(attr.Value == value)
				return false;
			else
				attr.Value = value;
			return true;
		}
		protected XElement GetAppSettingNode(string sectionAttribute) {
			if(SectionToAppSettingEquivalence.ContainsKey(sectionAttribute)) {
				string key = SectionToAppSettingEquivalence[sectionAttribute];
				return this[AppSettingsSectionPath].Where(node => CheckAttributeValue(node, KeyAttrName, key)).FirstOrDefault();
			}
			return null;
		}
		public bool IsHttpModuleExists() {
			return IsHttpModuleExists(true) && IsHttpModuleExists(false) && 
				IsHttpModuleHandlerExists(true) && IsHttpModuleHandlerExists(false);
		}
		public bool IsHttpModuleExists(bool iis7) {
			string sectionPath = iis7 ? ConfigurationSectionNames.HttpModuleIIS7Section : ConfigurationSectionNames.HttpModuleSection;
			return HasConfigSource(sectionPath) || IsHandlerModuleRegistered(sectionPath, GetHttpModuleType());
		}
		public bool IsHttpHandlerExists() {
			return IsHttpHandlerExists(true) && IsHttpHandlerExists(false);
		}
		public bool IsHttpHandlerExists(bool iis7) {
			string sectionPath = iis7 ? ConfigurationSectionNames.HttpHandlerIIS7Section : ConfigurationSectionNames.HttpHandlerSection;
			if(HasConfigSource(sectionPath))
				return true;
			return IsHandlerModuleRegistered(sectionPath, GetHttpHandlerType());
		}
		public bool IsHttpModuleHandlerExists(bool iis7) {
			string sectionPath = iis7 ? ConfigurationSectionNames.HttpHandlerIIS7Section : ConfigurationSectionNames.HttpHandlerSection;
			if(HasConfigSource(sectionPath))
				return true;
			return IsHandlerModuleRegistered(sectionPath, GetHttpModuleType());
		}
		bool IsHandlerModuleRegistered(string sectionPath, string type) {
			return this[sectionPath + "/" + NewSectionName].Any(child => CheckAttributeValue(child, TypeAttrName, GetTypeNameVariants(type)));
		}
		public void RegisterHttpModule(bool iis7) {
			string sectionPath = iis7 ? ConfigurationSectionNames.HttpModuleIIS7Section : ConfigurationSectionNames.HttpModuleSection;
			if(!this.IsHttpModuleExists(iis7))
				AppendNode(CreateNode(NewSectionName,
					new XAttribute(TypeAttrName, GetHttpModuleType()),
					new XAttribute(NameAttrName, HttpUtils.HttpHandlerModuleName)
				), iis7 ? ConfigurationSectionNames.HttpModuleIIS7Section : ConfigurationSectionNames.HttpModuleSection);
			if(iis7) {
				var validationNodeAttr = this[ConfigurationSectionNames.HttpHandlerIIS7ValidationSection].Attributes(ValidationNodeAttrName).FirstOrDefault();
				if(validationNodeAttr == null) {
					validationNodeAttr = new XAttribute(ValidationNodeAttrName, "false");
					var validationNode = this[ConfigurationSectionNames.HttpHandlerIIS7ValidationSection].FirstOrDefault();
					if(validationNode == null) {
						validationNode = new XElement(NS + HttpHandlerIIS7ValidationSectionName, validationNodeAttr);
						AppendNode(validationNode, "system.webServer");
					}
				}
				else if(validationNodeAttr.Value != "false")
					validationNodeAttr.Value = "false";
			}
		}
		public void RegisterHttpModuleHandler(bool iis7) {
			if(!this.IsHttpModuleHandlerExists(iis7)) {
				XElement node = CreateNode(NewSectionName,
					new XAttribute(TypeAttrName, GetHttpModuleType()),
					new XAttribute(VerbAttrName, "GET"),
					new XAttribute(PathAttrName, HttpModuleHandlerPagePath)
				);
				if(iis7) {
					node.Add(
						new XAttribute(NameAttrName, HttpUtils.HttpHandlerModuleName),
						new XAttribute("preCondition", "integratedMode")
					);
				}
				else
					node.Add(
						new XAttribute(ValidateAttrName, "false")
					);
				AppendNode(node, iis7 ? ConfigurationSectionNames.HttpHandlerIIS7Section : ConfigurationSectionNames.HttpHandlerSection);
			}
		}
		public bool RegisterHttpHandler(bool iis7) {
			bool changed = false;
			if(!this.IsHttpHandlerExists(iis7)) {
				XElement node = CreateNode(NewSectionName,
					new XAttribute(TypeAttrName, GetHttpHandlerType()),
					new XAttribute(VerbAttrName, "GET,POST"),
					new XAttribute(PathAttrName, UploadProgressHttpHandlerPagePath)
				);
				if(iis7) {
					node.Add(
						new XAttribute(NameAttrName, UploadProgressHttpHandlerName),
						new XAttribute("preCondition", "integratedMode")
					);
				}
				else
					node.Add(
						new XAttribute(ValidateAttrName, "false")
					);
				AppendNode(node, iis7 ? ConfigurationSectionNames.HttpHandlerIIS7Section : ConfigurationSectionNames.HttpHandlerSection);
				changed = true;
			}
			return changed;
		}
		protected internal bool HasConfigSource(string sectionPath) {
			if(sectionPath.StartsWith(ConfigurationSectionNames.ConfigurationSectionName)) {
				if(sectionPath.Length == ConfigurationSectionNames.ConfigurationSectionName.Length)
					return false;
				return HasConfigSource(sectionPath.Substring(ConfigurationSectionNames.ConfigurationSectionName.Length + 1));
			}
			string[] sections = sectionPath.Split('/');
			IEnumerable<XElement> result = RootElement;
			for(int i = 0; i < sections.Length; i++) {
				result = result.Elements(NS + sections[i]);
				if(result.Attributes(ConfigSourceAttrName).Any())
					return true;
			}
			if(sections[0] != LocationSectionName)
				return HasConfigSource(LocationSectionName + "/" + sectionPath);
			return false;
		}
		public bool IsDxSectionsRegistered() {
			XElement sectionGroup = GetDxRegistrationSectionGroupNode();
			if(sectionGroup == null)
				return false;
			return !SectionTypes.Any(sectionType => !CheckRegistrationSectionNode(sectionType, sectionGroup));
		}
		public bool RegisterDxSections() {
			bool changed = false;
			XElement sectionGroup = GetDxRegistrationSectionGroupNode();
			if(sectionGroup == null) {
				XElement configSectionsNode = this[ConfigSectionsSectionName].FirstOrDefault();
				if(configSectionsNode == null) {
					configSectionsNode = CreateNode(ConfigSectionsSectionName);
					RootElement.FirstOrDefault().AddFirst(configSectionsNode);
				}
				sectionGroup = CreateNode(SectionGroupSectionName,
					new XAttribute(NameAttrName, ConfigurationSectionNames.WebSectionGroupName)
				);
				configSectionsNode.Add(sectionGroup);
				changed = true;
			}
			foreach(Type sectionType in SectionTypes) {
				if(!CheckRegistrationSectionNode(sectionType, sectionGroup)) {
					changed = true;
					RegisterDxSections(sectionType, sectionGroup);
				}
			}
			return changed;
		}
		XElement GetDxRegistrationSectionGroupNode() {
			return (from node in this[ConfigSectionsSectionName + "/" + SectionGroupSectionName]
					where CheckAttributeValue(node, NameAttrName, ConfigurationSectionNames.WebSectionGroupName)
					select node).FirstOrDefault();
		}
		protected void RegisterDxSections(Type sectionType, XElement sectionGroup) {
			string sectionName = GetSectionName(sectionType);
			sectionGroup.Add(CreateNode(SectionSectionName,
				new XAttribute(NameAttrName, sectionName),
				new XAttribute(TypeAttrName, sectionType.AssemblyQualifiedName),
				new XAttribute(RequirePermissionAttrName, "false")
			));
		}
		bool CheckRegistrationSectionNode(Type sectionType, XElement sectionGroup) {
			string sectionName = GetSectionName(sectionType);
			return sectionGroup.Elements(NS + SectionSectionName).Any(node => CheckAttributeValue(node, NameAttrName, sectionName) && CheckAttributeValue(node, "type", GetTypeNameVariants(sectionType.AssemblyQualifiedName)));
		}
		public bool UpdateDxSections() {
			bool updated = false;
			foreach(Type sectionType in SectionTypes) {
				updated = UpdateDxSection(sectionType) || updated;
			}
			return updated;
		}
		protected bool UpdateDxSection(Type type) {
			bool updated = false;
			string sectionName = GetSectionName(type);
			XElement section = this[ConfigurationSectionNames.WebSectionGroupName, sectionName].FirstOrDefault();
			if(section == null) {
				section = CreateNode(sectionName);
				AppendNode(section, ConfigurationSectionNames.WebSectionGroupName);
				updated = true;
			}
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
			foreach(PropertyInfo prop in properties) {
				var cp = GetConfigPropertyAttribute(prop);
				if(cp != null) {
					string value = null;
					XElement asElement = GetAppSettingNode(cp.Name);
					if(asElement != null) {
						if(asElement.Attribute(ValueAttrName) != null)
							value = asElement.Attribute(ValueAttrName).Value;
						asElement.Remove();
						updated = true;
					}
					var attr = section.Attribute(cp.Name);
					if(attr == null) {
						string defaultValueStr =  Convert.ToString(cp.DefaultValue);
						if(!(cp.DefaultValue is Enum))
							defaultValueStr = defaultValueStr.ToLower();
						var xaAttribute = new XAttribute(cp.Name, value ?? defaultValueStr);
						section.Add(xaAttribute);
						updated = true;
					}
					else if(value != null) {
						attr.Value = value;
						updated = true;
					}
				}
			}
			return updated;
		}
		public bool AddReference(string assemblyName) {
			if(this[AssembliesSectionPath].Elements(NS + NewSectionName).Any(e => CheckAttributeValue(e, "assembly", GetTypeNameVariants(assemblyName))))
				return false;
			if (HasConfigSource(AssembliesSectionPath))
				return false;
			AppendNode(CreateNode(NewSectionName, new XAttribute("assembly", assemblyName)), AssembliesSectionPath);
			return true;
		}
		public void AppendNode(XElement node, params string[] sections) {
			if(sections.Length == 1 && sections[0].Contains('/'))
				AppendNode(node, sections[0].Split('/'));
			else if(sections[0] == ConfigurationSectionNames.ConfigurationSectionName)
				AppendNode(node, sections.Skip(1).ToArray());
			else {
				IEnumerable<XElement> result = this[sections];
				if(!result.Any()) {
					result = RootElement;
					for(int i = 0; i < sections.Length; i++) {
						if(!result.Elements(NS + sections[i]).Any())
							result.First().Add(CreateNode(sections[i]));
						result = result.Elements(NS + sections[i]);
					}
				}
				result.First().Add(node);
			}
		}
		protected XElement CreateNode(string name, params object[] content) {
			return new XElement(NS + name, content);
		}
		static bool CheckAttributeValue(XElement node, string attrName, params string[] attrValue) {
			return node.Attributes(attrName).Any(attr => attr.Name == attrName && attrValue.Contains(attr.Value, StringComparer.InvariantCultureIgnoreCase));
		}
		static string[] GetTypeNameVariants(string fullTypeString) {
			var typeParts = fullTypeString.Split(',');
			var names = new List<string>();
			names.Add(fullTypeString);
			if(typeParts.Length > 1)
				names.Add(string.Join(",", typeParts.Take(1)));
			if(typeParts.Length > 2)
				names.Add(string.Join(",", typeParts.Take(2)));
			return names.ToArray();
		}
		public static string GetHttpModuleType() {
			return GetHttpModuleType(false);
		}
		public static string GetHttpModuleType(bool skipAssembly) {
			string typeName = typeof(ASPxHttpHandlerModule).AssemblyQualifiedName;
			return skipAssembly ? typeName.Remove(typeName.IndexOf(",")) : typeName;
		}
		public static string GetHttpHandlerType() {
			return GetHttpHandlerType(false);
		}
		public static string GetHttpHandlerType(bool skipAssembly) {
			string typeName = typeof(DevExpress.Web.ASPxUploadProgressHttpHandler).AssemblyQualifiedName;
			return skipAssembly ? typeName.Remove(typeName.IndexOf(",")) : typeName;
		}
		static System.Configuration.ConfigurationPropertyAttribute GetConfigPropertyAttribute(MemberInfo member) {
			var attribute = member.GetCustomAttributes(typeof(System.Configuration.ConfigurationPropertyAttribute), false).SingleOrDefault();
			return attribute as System.Configuration.ConfigurationPropertyAttribute;
		}
		static string GetSectionName(Type sectionType) {
			return sectionType.Name.Substring(0, sectionType.Name.Length - "ConfigurationSection".Length).ToLower();
		}
	}
}
