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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Xml;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Localization {
	public class XafResourceManager : ResourceManager {
		public const string ControlsLocalizationNodeName = "Controls";
		private readonly IXafResourceManagerParameters parameters;
		private readonly ResourceManager innerResourceManager;
		private readonly Dictionary<CultureInfo, XafResourceSet> resourceSetByCulture;
		private bool isSearchingResourceSet;
		public XafResourceManager(IXafResourceManagerParameters parameters)
			: base(parameters.LocalizationResourceName, parameters.LocalizationResourceAssembly, typeof(XafResourceSet)) {
			this.parameters = parameters;
			this.innerResourceManager = new ResourceManager(parameters.LocalizationResourceName, parameters.LocalizationResourceAssembly);
			this.resourceSetByCulture = new Dictionary<CultureInfo, XafResourceSet>();
		}
		public XafResourceManager(IXafResourceManagerParametersProvider parametersProvider)
			: this(parametersProvider.XafResourceManagerParameters) {
		}
		public override ResourceSet GetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents) {
			return InternalGetResourceSet(culture, createIfNotExists, tryParents);
		}
		protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents) {
			XafResourceSet result = null;
			if(!isSearchingResourceSet) {
				isSearchingResourceSet = true;
				try {
					if(!resourceSetByCulture.TryGetValue(culture, out result)) {
						lock(resourceSetByCulture) {
							if(!resourceSetByCulture.TryGetValue(culture, out result)) {
								ResourceSet innerResourceSet = null;
								if(parameters.LocalizationResourceName != XafResourceManagerParameters.FakeResourceName) {
									innerResourceSet = innerResourceManager.GetResourceSet(culture, createIfNotExists, tryParents);
								}
								else if(parameters.XmlResourceLocalizer != null) {
									innerResourceSet = new XmlResourceSet(parameters, culture);
								}
								result = new XafResourceSet(culture, parameters, innerResourceSet);
								resourceSetByCulture.Add(culture, result);
							}
						}
					}
				}
				finally {
					isSearchingResourceSet = false;
				}
			}
			return result;
		}
		public IEnumerable<DictionaryEntry> GetFilteredResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents) {
			List<DictionaryEntry> result = new List<DictionaryEntry>();
			ResourceSet resourceSet = GetResourceSet(culture, createIfNotExists, tryParents);
			foreach(DictionaryEntry entry in resourceSet) {
				if(entry.Value is string) {
					string key = entry.Key.ToString();
					if(!key.StartsWith(">>") && (parameters.ResourceItemNameFilter == null || parameters.ResourceItemNameFilter(key))) {
						result.Add(entry);
					}
				}
			}
			return result;
		}
		public void ReloadModelData() {
			foreach(XafResourceSet resourceSet in resourceSetByCulture.Values) {
				resourceSet.Reset();
			}
		}
		public override Type ResourceSetType {
			get { return typeof(XafResourceSet); }
		}
		public IXafResourceManagerParameters Parameters {
			get { return parameters; }
		}
	}
	sealed class AspectedValue {
		public AspectedValue() {
		}
		public AspectedValue(string aspect, string value) {
			Aspect = aspect;
			Value = value;
		}
		public override string ToString() {
			return string.Format("[{0}:{1}]", Aspect, Value);
		}
		public string Aspect { get; set; }
		public string Value { get; set; }
	}
	sealed class XafResourceSet : ResourceSet {
		private readonly CultureInfo culture;
		private readonly IXafResourceManagerParameters parameters;
		private readonly ResourceSet innerResourceSet;
		private bool isInitialized;
		public XafResourceSet(CultureInfo culture, IXafResourceManagerParameters parameters, ResourceSet resourceSet) {
			this.culture = culture;
			this.parameters = parameters;
			innerResourceSet = resourceSet;
			Reader = new XafResourceReader(culture, parameters);
			EnsureResources();
		}
		private void EnsureResources() {
			if(!isInitialized) {
				ReadResources();
				isInitialized = true;
			}
		}
		protected override void ReadResources() {
			IDictionaryEnumerator enumerator = Reader.GetEnumerator();
			Dictionary<object, AspectedValue> storedValues = new Dictionary<object, AspectedValue>();
			while(enumerator.MoveNext()) {
				AspectedValue aspectedValue = (AspectedValue)enumerator.Value;
				Table.Add(enumerator.Key, aspectedValue.Value);
				storedValues.Add(enumerator.Key, aspectedValue);
			}
			if(innerResourceSet != null) {
				enumerator = innerResourceSet.GetEnumerator();
				while(enumerator.MoveNext()) {
					if(!Table.ContainsKey(enumerator.Key)) {
						Table.Add(enumerator.Key, DefaultResourceReader.Active.GetObject(parameters, enumerator.Key as string, enumerator.Value));
					}
					else if(CaptionHelper.IsAncestorAspect(storedValues[enumerator.Key].Aspect, GetCurrentAspect())) {
						Table[enumerator.Key] = enumerator.Value;
					}
				}
			}
		}
		private string GetCurrentAspect() {
			string result = CaptionHelper.DefaultLanguage;
			if(ModelApplication != null) {
				string aspectName = CaptionHelper.GetAspectByCultureInfo(culture);
				if(((ModelApplicationBase)ModelApplication).Aspects.Contains(aspectName)) {
					result = aspectName;
				}
			}
			return result;
		}
		private IModelApplication ModelApplication {
			get { return parameters.ModelApplication; }
		}
		public override Type GetDefaultReader() {
			return typeof(XafResourceReader);
		}
		public override object GetObject(string name) {
			EnsureResources();
			return base.GetObject(name);
		}
		public override object GetObject(string name, bool ignoreCase) {
			EnsureResources();
			return base.GetObject(name, ignoreCase);
		}
		public override string GetString(string name) {
			EnsureResources();
			return base.GetString(name);
		}
		public override string GetString(string name, bool ignoreCase) {
			EnsureResources();
			return base.GetString(name, ignoreCase);
		}
		public void Reset() {
			Table.Clear();
			isInitialized = false;
		}
	}
	sealed class XafResourceReader : IResourceReader {
		private readonly CultureInfo culture;
		private readonly IXafResourceManagerParameters parameters;
		public XafResourceReader(CultureInfo culture, IXafResourceManagerParameters parameters) {
			this.culture = culture;
			this.parameters = parameters;
		}
		private IDictionaryEnumerator GetEnumerator() {
			Hashtable resultDictionary = new Hashtable();
			if(ModelApplication != null) {
				IModelLocalization localizationNode = ModelApplication.Localization;
				if(localizationNode != null) {
					IModelLocalizationGroup localizationGroupNode = FindLocalizationGroupNode(localizationNode, LocalizationGroupPath);
					if(localizationGroupNode != null) {
						int aspectIndex = ModelApplicationBase.GetAspectIndex(CaptionHelper.GetAspectByCultureInfo(culture));
						string aspect = ModelApplicationBase.GetAspect(aspectIndex);
						FillLocalizationDictionary(aspect, localizationGroupNode, resultDictionary);
					}
				}
			}
			return resultDictionary.GetEnumerator();
		}
		private IModelLocalizationGroup FindLocalizationGroupNode(IModelLocalization localizationNode, string[] localizationGroupPath) {
			IModelLocalizationGroup groupNode = null;
			if(localizationGroupPath.Length > 1) {
				foreach(string groupName in localizationGroupPath) {
					if(groupNode == null) {
						groupNode = localizationNode[groupName];
					}
					else {
						groupNode = (IModelLocalizationGroup)groupNode[groupName];
					}
					if(groupNode == null) {
						break;
					}
				}
			}
			return groupNode;
		}
		private void FillLocalizationDictionary(string aspect, IModelLocalizationGroup localizationGroupNode, Hashtable resultDictionary) {
			string currentAspect = ModelApplicationBase.CurrentAspect;
			try {
				ModelApplicationBase.SetCurrentAspect(aspect);
				foreach(IModelLocalizationItemBase node in localizationGroupNode) {
					if(node is IModelLocalizationItem) {
						string value = ((IModelLocalizationItem)node).Value;
						resultDictionary.Add(ResourceItemPrefix + node.Name, new AspectedValue(aspect, value));
					}
				}
			}
			finally {
				ModelApplicationBase.SetCurrentAspect(currentAspect);
			}
		}
		private IModelApplication ModelApplication {
			get { return parameters.ModelApplication; }
		}
		private ModelApplicationBase ModelApplicationBase {
			get { return (ModelApplicationBase)parameters.ModelApplication; }
		}
		private string ResourceItemPrefix {
			get { return parameters.ResourceItemPrefix; }
		}
		private string[] LocalizationGroupPath {
			get { return parameters.LocalizationGroupPath; }
		}
		void IResourceReader.Close() {
		}
		IDictionaryEnumerator IResourceReader.GetEnumerator() {
			return GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		void IDisposable.Dispose() {
		}
	}
	public class XmlResourceSet : ResourceSet {
		private IXafResourceManagerParameters parameters;
		public XmlResourceSet(IXafResourceManagerParameters parameters, CultureInfo culture) {
			this.parameters = parameters;
			this.Reader = new XmlResourceReader(parameters, culture);
			this.Table = new Hashtable();
			ReadResources();
		}
		protected override void ReadResources() {
			IDictionaryEnumerator enumerator = this.Reader.GetEnumerator();
			while(enumerator.MoveNext()) {
				AspectedValue aspectedValue = enumerator.Value as AspectedValue;
				Table.Add(enumerator.Key, aspectedValue.Value);
			}
		}
		public override Type GetDefaultReader() {
			return typeof(XmlResourceReader);
		}
	}
	internal class XmlResourceReader : IResourceReader {
		private IXafResourceManagerParameters parameters;
		private CultureInfo culture;
		public XmlResourceReader(IXafResourceManagerParameters parameters, CultureInfo culture) {
			this.parameters = parameters;
			this.culture = culture;
		}
		#region IResourceReader Members
		public void Close() {
		}
		public IDictionaryEnumerator GetEnumerator() {
			Hashtable dictionary = new Hashtable();
			if(culture.Equals(CultureInfo.InvariantCulture)) {
				XmlDocument xmlDocument = parameters.XmlResourceLocalizer.GetXmlResources();
				foreach(XmlNode node in xmlDocument.ChildNodes[1].ChildNodes) {
					string name = node.Attributes["name"].Value;
					string value = node.ChildNodes[0].ChildNodes[0].Value;
					dictionary.Add(name, new AspectedValue(CaptionHelper.DefaultLanguage, value));
				}
			}
			return dictionary.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
	}
	public class XafResourceManagerParameters : IXafResourceManagerParameters {
		public const string FakeResourceName = "FakeResource";
		private IModelApplication modelApplication;
		private string[] localizationGroupPath;
		private string localizationResourceName;
		private string resourceItemPrefix;
		private Assembly localizationResourceAssembly;
		private IXmlResourceLocalizer xmlResourceLocalizer;
		private Predicate<string> resourceItemNameFilter;
		public XafResourceManagerParameters(string localizationNodeName, string resourceItemPrefix, IXmlResourceLocalizer xmlResourceLocalizer)
			: this(null, null, resourceItemPrefix, xmlResourceLocalizer) {
			this.localizationGroupPath = new string[] { XafResourceManager.ControlsLocalizationNodeName, localizationNodeName };
		}
		public XafResourceManagerParameters(IModelApplication modelApplication, string[] localizationGroupPath, string resourceItemPrefix, IXmlResourceLocalizer xmlResourceLocalizer)
			: this(modelApplication, localizationGroupPath, FakeResourceName, resourceItemPrefix, typeof(XafResourceManagerParameters).Assembly) {
			this.xmlResourceLocalizer = xmlResourceLocalizer;
		}
		public XafResourceManagerParameters(string[] localizationGroupPath, string localizationResourceName, string resourceItemPrefix, Assembly localizationResourceAssembly)
			: this(null, localizationGroupPath, localizationResourceName, resourceItemPrefix, localizationResourceAssembly) {
		}
		public XafResourceManagerParameters(string[] localizationGroupPath, string localizationResourceName, string resourceItemPrefix, Assembly localizationResourceAssembly, Predicate<string> resourceItemNameFilter)
			: this(null, localizationGroupPath, localizationResourceName, resourceItemPrefix, localizationResourceAssembly, resourceItemNameFilter) {
		}
		public XafResourceManagerParameters(IModelApplication modelApplication, string[] localizationGroupPath, string localizationResourceName, string resourceItemPrefix, Assembly localizationResourceAssembly)
			: this(null, localizationGroupPath, localizationResourceName, resourceItemPrefix, localizationResourceAssembly, null) {
		}
		public XafResourceManagerParameters(IModelApplication modelApplication, string[] localizationGroupPath, string localizationResourceName, string resourceItemPrefix, Assembly localizationResourceAssembly, Predicate<string> resourceItemNameFilter) {
			this.modelApplication = modelApplication;
			this.localizationGroupPath = localizationGroupPath;
			this.localizationResourceName = localizationResourceName;
			this.resourceItemPrefix = resourceItemPrefix;
			this.localizationResourceAssembly = localizationResourceAssembly;
			this.resourceItemNameFilter = resourceItemNameFilter;
		}
		public XafResourceManagerParameters(string localizationNodeName, string localizationResourceName, string resourceItemPrefix, Assembly localizationResourceAssembly)
			: this((string[])null, localizationResourceName, resourceItemPrefix, localizationResourceAssembly) {
			this.localizationGroupPath = new string[] { XafResourceManager.ControlsLocalizationNodeName, localizationNodeName };
		}
		public override int GetHashCode() {
			string result = localizationGroupPath.ToString() + localizationResourceAssembly.FullName + localizationResourceName;
			return result.GetHashCode();
		}
		public override bool Equals(object obj) {
			XafResourceManagerParameters o = obj as XafResourceManagerParameters;
			if(o == null) {
				return false;
			}
			return localizationGroupPath.Equals(o.localizationGroupPath) && localizationResourceName == o.localizationResourceName && localizationResourceAssembly == o.localizationResourceAssembly;
		}
		#region IXafResourceManagerParameters Members
		public IModelApplication ModelApplication {
			get { return modelApplication; }
			set { modelApplication = value; }
		}
		public string[] LocalizationGroupPath {
			get { return localizationGroupPath; }
			set { localizationGroupPath = value; }
		}
		public string LocalizationResourceName {
			get { return localizationResourceName; }
			set { localizationResourceName = value; }
		}
		public string ResourceItemPrefix {
			get { return resourceItemPrefix; }
			set { resourceItemPrefix = value; }
		}
		public Assembly LocalizationResourceAssembly {
			get { return localizationResourceAssembly; }
			set { localizationResourceAssembly = value; }
		}
		public IXmlResourceLocalizer XmlResourceLocalizer {
			get { return xmlResourceLocalizer; }
		}
		public Predicate<string> ResourceItemNameFilter {
			get { return resourceItemNameFilter; }
			set { resourceItemNameFilter = value; }
		}
		#endregion
	}
	public class DefaultResourceReader {
		private static DefaultResourceReader active = new DefaultResourceReader();
		public virtual object GetObject(IXafResourceManagerParameters parameters, string key, object defaultValue) {
			return defaultValue;
		}
		public static DefaultResourceReader Active {
			get { return active; }
			set { active = value; }
		}
	}
}
