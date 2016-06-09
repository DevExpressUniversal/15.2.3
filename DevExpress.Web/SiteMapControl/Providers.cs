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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Xml;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	public class UnboundSiteMapNode : SiteMapNode, IHierarchyData {
		[
#if !SL
	DevExpressWebLocalizedDescription("UnboundSiteMapNodeUrl"),
#endif
 UrlProperty, 
		Editor("DevExpress.Web.Design.SiteMapNodeUrlEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public override string Url {
			get { return base.Url; }
			set {
				UnboundSiteMapProvider provider = Provider as UnboundSiteMapProvider;
				if(provider != null && provider.HashtableContains(this)) {
					provider.RemoveFromHashtable(this);
					base.Url = value;
					provider.AddToHashtable(this);
				} else
					base.Url = value;
			}
		}
		protected internal NameValueCollection AttributeColletion {
			get { return Attributes; }
		}
		public UnboundSiteMapNode(SiteMapProvider provider, string key)
			: this(provider, key, null, null, null, null, null, null, null) {
		}
		public UnboundSiteMapNode(SiteMapProvider provider, string key, string url)
			: this(provider, key, url, null, null, null, null, null, null) {
		}
		public UnboundSiteMapNode(SiteMapProvider provider, string key, string url, string title)
			: this(provider, key, url, title, null, null, null, null, null) {
		}
		public UnboundSiteMapNode(SiteMapProvider provider, string key, string url, string title, string description)
			: this(provider, key, url, title, description, null, null, null, null) {
		}
		public UnboundSiteMapNode(SiteMapProvider provider, string key, string url, string title, string description, IList roles, NameValueCollection attributes, NameValueCollection explicitResourceKeys, string implicitResourceKey)
			: base(provider, key, url, title, description, roles, attributes, explicitResourceKeys, implicitResourceKey) {
		}
		public override string ToString() {
			string ret = "";
			if ((Title == "") && (Url == ""))
				ret = "SiteMapNode";
			else {
				if (Title != "")
					ret = Title;
				else {
					if (Path.GetFileName(Url) != "") 
						ret = "[" + Path.GetFileName(Url) + "] " + ret;
					else
						ret = "[" + Path.GetDirectoryName(Url) + "] " + ret;
				}
			}
			return ret;
		}
		string IHierarchyData.Path { get { return Url == "" ? Key : Url; } }
	}
	public class UnboundSiteMapProviderBase : SiteMapProvider {
		protected internal static string DefaultSiteMapFileName = "~/web.sitemap";
		private const string DefaultRootNodeTitle = "RootNode";
		private const string DefaultRootNodeUrl = "";
		private IDictionary fChildNodeCollectionHashTable = null;
		private bool fEnableRoles = true;
		private IDictionary fParentNodeHashTable = null;
		private string fSiteMapFileName = "";
		protected SiteMapNode fRootNode = null;
#if !SL
	[DevExpressWebLocalizedDescription("UnboundSiteMapProviderBaseEnableRoles")]
#endif
		public bool EnableRoles {
			get { return fEnableRoles; }
			set {
				fEnableRoles = value;
				EnableRolesChanged();
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("UnboundSiteMapProviderBaseSiteMapFileName")]
#endif
		public string SiteMapFileName {
			get { return fSiteMapFileName; }
			set {
				fSiteMapFileName = value;
				ResourceKey = GetResourceKey(value);
				fRootNode = null; 
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("UnboundSiteMapProviderBaseRootNode")]
#endif
		public override SiteMapNode RootNode {
			get {
				fRootNode = GetRootNodeCore();
				return ReturnNodeIfAccessible(fRootNode);
			}
		}
		protected IDictionary ChildNodeCollectionHashTable {
			get {
				if (fChildNodeCollectionHashTable == null)
					fChildNodeCollectionHashTable = new Hashtable();
				return fChildNodeCollectionHashTable;
			}
		}
		protected IDictionary ParentNodeHashTable {
			get {
				if (fParentNodeHashTable == null)
					fParentNodeHashTable = new Hashtable();
				return fParentNodeHashTable;
			}
		}
		public UnboundSiteMapProviderBase() {
		}
		public UnboundSiteMapProviderBase(string rootNodeUrl)
			: this(rootNodeUrl, "", "") {
		}
		public UnboundSiteMapProviderBase(string rootNodeUrl, string rootNodeTitle)
			: this(rootNodeUrl, rootNodeTitle, "") {
		}
		public UnboundSiteMapProviderBase(string rootNodeUrl, string rootNodeTitle, string rootNodeDescription)
			: this() {
			RootNode.Title = rootNodeTitle;
			RootNode.Url = rootNodeUrl;
			RootNode.Description = rootNodeDescription;
		}
		public override void Initialize(string name, NameValueCollection attributes) {
			if(attributes != null)
				SiteMapFileName = attributes["siteMapFile"];			
			base.Initialize(name, attributes);
		}
		public override SiteMapNodeCollection GetChildNodes(SiteMapNode node) {
			if (node == null)
				throw new ArgumentNullException(StringResources.InvalidNode);
			SiteMapNodeCollection childCollection = (SiteMapNodeCollection)ChildNodeCollectionHashTable[node];
			if (childCollection == null)
				return SiteMapNodeCollection.ReadOnly(new SiteMapNodeCollection());
			if (!SecurityTrimmingEnabled)
				return SiteMapNodeCollection.ReadOnly(childCollection);
			SiteMapNodeCollection trimmingChildCollection = new SiteMapNodeCollection(childCollection.Count);
			foreach (SiteMapNode trimmingNode in childCollection) {
				if (IsAccessibleToUser(trimmingNode))
					trimmingChildCollection.Add(trimmingNode);
			}
			return SiteMapNodeCollection.ReadOnly(trimmingChildCollection);
		}
		public override SiteMapNode GetParentNode(SiteMapNode childNode) {
			if (childNode == null)
				throw new ArgumentNullException(StringResources.InvalidNode);
			SiteMapNode parentNode = (SiteMapNode)ParentNodeHashTable[childNode];
			return ReturnNodeIfAccessible(parentNode);
		}
		public override SiteMapNode FindSiteMapNode(string rawUrl) {
			rawUrl = rawUrl.Trim();
			if (rawUrl == "")
				return null;
			if (UrlUtils.IsAppRelativePath(rawUrl))
				rawUrl = UrlUtils.MakeVirtualPathAppAbsolute(rawUrl);
			return FindSiteMapNodeRecursive(RootNode, rawUrl);
		}
		public void AddSiteMapNode(SiteMapNode node, SiteMapNode parentNode) {
			if (node == null || parentNode == null)
				throw new ArgumentNullException(StringResources.InvalidNode);
			if (!(node is UnboundSiteMapNode) || !(parentNode is UnboundSiteMapNode))
				throw new ArgumentException(string.Format(StringResources.UnboundProvider_CanOnlyCreatedByMethod, node.Title));
			if (node.Provider != this)
				throw new ArgumentException(string.Format(StringResources.UnboundProvider_CanNotAddNode, node.Title));
			if (parentNode.Provider != this)
				throw new ArgumentException(string.Format(StringResources.UnboundProvider_CanNotAddNode, parentNode.Title));
			AddNode(node, parentNode);
		}
		public void AddSiteMapNode(SiteMapNode node) {
			AddNode(node, RootNode);
		}
		public void Clear() {
			ClearInternal();
		}
		public UnboundSiteMapNode CloneSiteMapNode(SiteMapNode node) {
			if (!(node is UnboundSiteMapNode))
				throw new ArgumentNullException(string.Format(StringResources.UnboundProvider_CanOnlyCreatedByMethod, node.Title));
			return new UnboundSiteMapNode(this, Guid.NewGuid().ToString(), node.Url, node.Title,
				node.Description, node.Roles, (node as UnboundSiteMapNode).AttributeColletion, null, null);
		}
		public UnboundSiteMapNode CreateNode(string url, string title) {
			return CreateNode(url, title, "");
		}
		public UnboundSiteMapNode CreateNode(string url, string title, string description) {
			return CreateNodeInternal(url, title, description, Guid.NewGuid().ToString(), null, null);
		}
		public UnboundSiteMapNode CreateNode(string url, string title, string description, IList roles) {
			return CreateNodeInternal(url, title, description, Guid.NewGuid().ToString(), roles, null);
		}
		public UnboundSiteMapNode CreateNode(string url, string title, string description, IList roles, NameValueCollection attributes) {
			return CreateNodeInternal(url, title, description, Guid.NewGuid().ToString(), roles, attributes);
		}
		public void RemoveSiteMapNode(SiteMapNode node) {
			RemoveNode(node);
		}
		public SiteMapNode LoadFromFile(string siteMapFileName) {
			if (!File.Exists(MapPath(siteMapFileName)))
				throw new ConfigurationErrorsException(string.Format(StringResources.UnboundProvider_MissingSiteMapFile, siteMapFileName));
			SMPXmlReader xmlReader = new SMPXmlReader(this);
			ResourceKey = GetResourceKey(siteMapFileName);
			fRootNode = xmlReader.LoadFromXml(MapPath(siteMapFileName));
			return fRootNode;
		}
		public SiteMapNode LoadFromStream(Stream stream) {
			SMPXmlReader xmlReader = new SMPXmlReader(this);
			fRootNode = xmlReader.LoadFromXmlStream(stream);
			return fRootNode;
		}
		public void SaveToFile(string siteMapFileName) {
			SMPXmlWriter xmlWriter = new SMPXmlWriter(this);
			xmlWriter.SaveToXml(siteMapFileName);
		}
		public void SaveToStream(Stream outStream) {
			SMPXmlWriter xmlWriter = new SMPXmlWriter(this);
			xmlWriter.SaveToStream(outStream);
		}
		protected override void AddNode(SiteMapNode node, SiteMapNode parentNode) {
			if (node == null)
				throw new ArgumentNullException(StringResources.InvalidNode);
			if (ParentNodeHashTable[node] != null)
				RemoveNode(node);
			SiteMapNodeCollection childNodes = GetChildNodeCollection(node, parentNode);
			childNodes.Add(node);
		}
		protected override SiteMapNode GetRootNodeCore() {
			if (fRootNode == null)
				fRootNode = CreateRootNode();
			return fRootNode;
		}
		protected override void RemoveNode(SiteMapNode node) {
			if (node == null)
				throw new ArgumentNullException(StringResources.InvalidNode);
			if (node.Provider != this)
				throw new ArgumentNullException(string.Format(StringResources.UnboundProvider_CanNotAddNode, node.Title));
			SiteMapNode parentNode = (SiteMapNode)ParentNodeHashTable[node];
			if (parentNode != null) {
				SiteMapNodeCollection childNodeCollection = (SiteMapNodeCollection)ChildNodeCollectionHashTable[parentNode];
				if (childNodeCollection != null)
					childNodeCollection.Remove(node);
			}
			if (ParentNodeHashTable.Contains(node))
				ParentNodeHashTable.Remove(node);
		}
		protected virtual void ClearInternal() {
			if (fChildNodeCollectionHashTable != null)
				fChildNodeCollectionHashTable.Clear();
			if (fParentNodeHashTable != null)
				fParentNodeHashTable.Clear();
		}
		protected virtual UnboundSiteMapNode CreateNodeInternal(string url, string title, string description, string key, IList roles, NameValueCollection attributes) {
			NameValueCollection attr = new NameValueCollection();
			attr.Add("title", title);
			attr.Add("url", url);
			attr.Add("description", description);
			if (attributes != null)
				attr.Add(attributes);
			return new UnboundSiteMapNode(this, key, url, title, description, roles, attr, null, null);
		}
		protected virtual SiteMapNode CreateRootNode() {
			if (!string.IsNullOrEmpty(SiteMapFileName)) {
				string fileName = MapPath(SiteMapFileName);
				return LoadFromFile(fileName);
			}
			else
				return CreateNode(DefaultRootNodeUrl, DefaultRootNodeTitle);
		}
		protected void RefreshHierarchy() {
			fRootNode = CreateRootNode();
		}
		protected internal virtual string MapPath(string virtualPath) {
			if (!UrlUtils.IsAbsolutePhysicalPath(virtualPath)) {
				if (!UrlUtils.IsAppRelativePath(virtualPath))
					virtualPath = "~" + "/" + virtualPath;
				return HostingEnvironment.MapPath(virtualPath);
			}
			else
				return virtualPath;
		}
		protected internal virtual bool NeedPrepareUrl() {
			return true;
		}
		protected internal virtual void HandleResourceAttribute(XmlNode xmlNode, ref NameValueCollection collection, 
			string attrName, ref string attrValueString, bool allowImplicitResource) {
			if(attrValueString != "") {
				string resourceAttr = attrValueString.TrimStart(new char[] { ' ' });
				if((resourceAttr != null) && (resourceAttr.Length > 10) &&
					resourceAttr.ToLower(CultureInfo.InvariantCulture).StartsWith("$resources:", StringComparison.Ordinal)) {
					if(!allowImplicitResource)
						throw new ConfigurationErrorsException("XmlSiteMapProvider_multiple_resource_definition", xmlNode);
					string resourcesString = resourceAttr.Substring(11);
					if(resourcesString.Length == 0)
						throw new ConfigurationErrorsException("XmlSiteMapProvider_resourceKey_cannot_be_empty", xmlNode);
					string className = "";
					string keyName = "";
					int length = resourcesString.IndexOf(',');
					if(length == -1)
						throw new ConfigurationErrorsException("XmlSiteMapProvider_invalid_resource_key", xmlNode);
					className = resourcesString.Substring(0, length);
					keyName = resourcesString.Substring(length + 1);
					int index = keyName.IndexOf(',');
					if(index != -1) {
						attrValueString = keyName.Substring(index + 1);
						keyName = keyName.Substring(0, index);
					} else
						attrValueString = null;
					if(collection == null)
						collection = new NameValueCollection();
					collection.Add(attrName, className.Trim());
					collection.Add(attrName, keyName.Trim());
				}
			}
		}
		protected SiteMapNode FindSiteMapNodeRecursive(SiteMapNode parentNode, string rawUrl) {
			SiteMapNode resultNode = null;
			string nodeUrl = PrepareUrl(parentNode.Url);
			if (string.Compare(nodeUrl, rawUrl, true) == 0)
				resultNode = parentNode;
			else {
				SiteMapNodeCollection collection = (SiteMapNodeCollection)ChildNodeCollectionHashTable[parentNode];
				if (collection != null)
					foreach (SiteMapNode node in collection) {
						SiteMapNode foundNode = FindSiteMapNodeRecursive(node, rawUrl);
						if (foundNode != null) {
							resultNode = foundNode;
							break;
						}
					}
			}
			return ReturnNodeIfAccessible(resultNode);
		}
		protected internal string PrepareUrl(string url, bool lowerCase) {
			string ret = url.Trim();
			if(!string.IsNullOrEmpty(ret) && !UrlUtils.IsAbsolutePhysicalPath(ret) && UrlUtils.IsRelativeUrl(ret)) {
				if(!string.IsNullOrEmpty(UrlUtils.AppDomainAppVirtualPathString))
					ret = UrlUtils.Combine(UrlUtils.AppDomainAppVirtualPathString, UrlUtils.AppDomainAppVirtualPathString, ret);
			}
			if(lowerCase)
				ret = ret.ToLower();
			return ret;
		}
		protected internal string PrepareUrl(string url) {
			return PrepareUrl(url, false);
		}
		protected bool IsAccessibleToUser(SiteMapNode node) {
			return node.IsAccessibleToUser(HttpContext.Current);
		}
		protected SiteMapNodeCollection GetChildNodeCollection(SiteMapNode node, SiteMapNode parentNode) {
			ParentNodeHashTable[node] = parentNode;
			if (ChildNodeCollectionHashTable[parentNode] == null)
				ChildNodeCollectionHashTable[parentNode] = new SiteMapNodeCollection();
			return ((SiteMapNodeCollection)ChildNodeCollectionHashTable[parentNode]);
		}
		protected SiteMapNode ReturnNodeIfAccessible(SiteMapNode node) {
			if (HttpContext.Current == null) 
				return node;
			else
				return (node != null) && node.IsAccessibleToUser(HttpContext.Current) ? node : null;
		}
		private void EnableRolesChanged() {
			NameValueCollection attributes = new NameValueCollection();
			attributes.Add("securityTrimmingEnabled", EnableRoles ? "true" : "false");
			base.Initialize(Guid.NewGuid().ToString(), attributes);
		}
		private string GetResourceKey(string siteMapFile) {
			return Path.GetFileName(siteMapFile);			
		}
	}
	public class UnboundSiteMapProvider : UnboundSiteMapProviderBase {
		private Hashtable fKeyTable = null;
		private Hashtable fUrlTable = null;
		protected Hashtable KeyTable {
			get {
				if (fKeyTable == null)
					fKeyTable = new Hashtable();
				return fKeyTable;
			}
		}
		protected Hashtable UrlTable {
			get {
				if (fUrlTable == null)
					fUrlTable = new Hashtable();
				return fUrlTable;
			}
		}
		public UnboundSiteMapProvider() {
		}
		public UnboundSiteMapProvider(string rootNodeUrl)
			: base(rootNodeUrl, "", "") {
		}
		public UnboundSiteMapProvider(string rootNodeUrl, string rootNodeTitle)
			: base(rootNodeUrl, rootNodeTitle, "") {
		}
		public UnboundSiteMapProvider(string rootNodeUrl, string rootNodeTitle, string rootNodeDescription)
			: base(rootNodeUrl, rootNodeTitle, rootNodeDescription) {
		}
		public override SiteMapNode FindSiteMapNode(string rawUrl) {
			if (rawUrl == null)
				throw new ArgumentNullException("rawUrl");
			if (RootNode != null) { } 
			string url = PrepareUrl(rawUrl, true);
			SiteMapNode node = (url == PrepareUrl(RootNode.Url, true)) ? RootNode : (SiteMapNode)UrlTable[url];
			return ReturnNodeIfAccessible(node);
		}
		public override SiteMapNode FindSiteMapNodeFromKey(string key) {
			SiteMapNode ret = base.FindSiteMapNodeFromKey(key);
			if (ret == null)
				ret = (key == RootNode.Key) ? RootNode : (SiteMapNode)KeyTable[key];
			return ReturnNodeIfAccessible(ret);
		}
		protected override void AddNode(SiteMapNode node, SiteMapNode parentNode) {
			AddToHashtable(node);
			base.AddNode(node, parentNode);
		}
		protected override void ClearInternal() {
			KeyTable.Clear();
			UrlTable.Clear();
			base.ClearInternal();
		}
		protected override void RemoveNode(SiteMapNode node) {
			RemoveFromHashtable(node);
			base.RemoveNode(node);
		}
		protected internal void AddToHashtable(SiteMapNode node) {
			if (!KeyTable.Contains(node.Key))
				KeyTable[node.Key] = node;
			string url = node.Url;
			if (url != "") {
				url = PrepareUrl(node.Url, true);
				if(UrlTable[url] != null)
					throw new InvalidOperationException(StringResources.SiteMapControl_DuplicatedUrl);
				if (url != "")
					UrlTable[url] = node;
			}
		}
		protected internal void RemoveFromHashtable(SiteMapNode node) {
			if (KeyTable.Contains(node.Key))
				KeyTable.Remove(node.Key);
			if ((node.Url != "") && (UrlTable.Contains(node.Url)))
				UrlTable.Remove(node.Url);
		}
		protected internal bool HashtableContains(SiteMapNode node) {
			return KeyTable.Contains(node.Key);
		}
	}
}
namespace DevExpress.Web.Internal {
	using DevExpress.Web.Internal;
	using DevExpress.Web;
	public class SMPXmlConsts {
		public static string SiteMapTagName = "siteMap";
		public static string SiteMapNodeTagName = "siteMapNode";
	}
	public class SMPXmlWriter {
		private static string XmlVersion = "1.0";
		private static string Xmlns = "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0";
		private UnboundSiteMapProviderBase fProvider = null;
		private XmlNode fRootDocNode = null;
		private Dictionary<SiteMapNode, XmlNode> fNodes = null;
		public SMPXmlWriter(UnboundSiteMapProviderBase provider) {
			fProvider = provider;
			fNodes = new Dictionary<SiteMapNode,XmlNode>();
		}
		public void SaveToStream(Stream outStream) {
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.InnerXml = GetFormattedContentString();
			xmlDocument.Save(outStream);
		}
		public void SaveToXml(string xmlFileName) {
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.InnerXml = GetFormattedContentString();
			FileAttributes attributes = File.GetAttributes(xmlFileName);
			File.SetAttributes(xmlFileName, FileAttributes.Normal);
			xmlDocument.Save(xmlFileName);
			File.SetAttributes(xmlFileName, attributes);
		}
		public string GetFormattedContentString() {
			XmlDocument xmlDocument = new XmlDocument();
			XmlDeclaration docDeclareNode = xmlDocument.CreateXmlDeclaration(XmlVersion, System.Text.Encoding.UTF8.BodyName, null);
			xmlDocument.AppendChild(docDeclareNode);
			fRootDocNode = xmlDocument.CreateNode(XmlNodeType.Element, SMPXmlConsts.SiteMapTagName, null);
			xmlDocument.AppendChild(fRootDocNode);
			xmlDocument.DocumentElement.SetAttribute("xmlns", Xmlns);
			AddNodeToXmlDocument(xmlDocument, fProvider.RootNode, null);
			AddToXmlDocRecursive(xmlDocument, fProvider.RootNode);
			return CommonUtils.FormatXmlDocumentText(xmlDocument);
		}
		protected void AddNodeToXmlDocument(XmlDocument xmlDocument, SiteMapNode node, SiteMapNode parentNode) {
			UnboundSiteMapNode unboundNode = node as UnboundSiteMapNode;
			if (unboundNode == null)
				throw new ArgumentException(string.Format(StringResources.UnboundProvider_CanOnlyCreatedByMethod, node.Title)); 
			XmlNode xmlParentNode = null;
			if (parentNode != null)
				xmlParentNode = fNodes[parentNode];
			if (xmlParentNode == null)
				xmlParentNode = xmlDocument.DocumentElement;
			XmlNode xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, SMPXmlConsts.SiteMapNodeTagName, xmlParentNode.NamespaceURI);
			XmlAttribute attrUrl = xmlDocument.CreateAttribute("url");
			attrUrl.Value = node.Url;
			XmlAttribute attrTitle = xmlDocument.CreateAttribute("title");
			attrTitle.Value = node.Title;
			XmlAttribute attrDescription = xmlDocument.CreateAttribute("description");
			attrDescription.Value = node.Description;
			xmlNode.Attributes.Append(attrUrl);
			xmlNode.Attributes.Append(attrTitle);
			if (attrDescription.Value != "")
				xmlNode.Attributes.Append(attrDescription);
			if (unboundNode.AttributeColletion != null) {
				foreach (string attrName in unboundNode.AttributeColletion.AllKeys) {
					if ((attrName.ToLower() != "url") && (attrName.ToLower() != "title") &&
						(attrName.ToLower() != "description")) {
						XmlAttribute attr = xmlDocument.CreateAttribute(attrName);
						attr.Value = unboundNode.AttributeColletion[attrName];
						xmlNode.Attributes.Append(attr);
					}
				}
			}
			xmlParentNode.AppendChild(xmlNode);
			fNodes.Add(node, xmlNode);
		}
		protected void AddToXmlDocRecursive(XmlDocument xmlDocument, SiteMapNode parentNode) {
			if (parentNode.HasChildNodes) {
				foreach (SiteMapNode node in parentNode.ChildNodes) {
					AddNodeToXmlDocument(xmlDocument, node, parentNode);
					AddToXmlDocRecursive(xmlDocument, node);
				}
			}
		}
		protected internal XmlNode GetXmlNodeByAttributeRecursive(XmlNode parent, string nodeUrl, string nodeTitle) {
			XmlNode foundNode = null;
			if (parent.HasChildNodes) {
				foreach (XmlNode sourceXmlNode in parent.ChildNodes) {
					if (string.Equals(sourceXmlNode.Name, SMPXmlConsts.SiteMapNodeTagName, StringComparison.Ordinal)) {
						XmlAttribute attrUrl = sourceXmlNode.Attributes["url"];
						XmlAttribute attrTitle = sourceXmlNode.Attributes["title"];
						if ((nodeUrl == attrUrl.Value) && (nodeTitle == attrTitle.Value)) 
							return sourceXmlNode;						
						else
							foundNode = GetXmlNodeByAttributeRecursive(sourceXmlNode, nodeUrl, nodeTitle);
						if (foundNode != null) break;
					}
				}
			}
			return foundNode;
		}
	}
	public class SMPXmlReader {
		private readonly char[] Separators = new char[] { ';', ',' };
		UnboundSiteMapProviderBase fProvider = null;
		public SMPXmlReader(UnboundSiteMapProviderBase provider) {
			fProvider = provider;
		}
		public SiteMapNode LoadFromXmlStream(Stream stream) {
			SiteMapNode rootNode = null;
			XmlDocument xmlDoc = new XmlDocument();
			XmlReader xmlReader = new XmlTextReader(stream);
			xmlDoc.Load(xmlReader);
			XmlNode rootXmlNode = GetRootSiteMapXmlNode(xmlDoc);
			if (rootXmlNode == null)
				throw new ConfigurationErrorsException(StringResources.UnboundProvider_MissingTopElement);
			string enableLocalizationString = GetAttributeValue(rootXmlNode, "enableLocalization");
			if(enableLocalizationString != "")
				fProvider.EnableLocalization = bool.Parse(enableLocalizationString);
			XmlNode nextChildXmlNode = null;
			foreach (XmlNode childXmlNode in rootXmlNode.ChildNodes) {
				if (childXmlNode.NodeType == XmlNodeType.Element) {
					if (childXmlNode.Name != SMPXmlConsts.SiteMapNodeTagName)
						throw new ConfigurationErrorsException(StringResources.UnboundProvider_OnlySiteMapNodeAllowed);
					if (nextChildXmlNode != null)
						throw new ConfigurationErrorsException(StringResources.UnboundProvider_OnlyOneSiteMapNodeAtTop);
					nextChildXmlNode = childXmlNode;
				}
			}
			if (nextChildXmlNode == null)
				throw new ConfigurationErrorsException(StringResources.UnboundProvider_OnlyOneSiteMapNodeAtTop);
			fProvider.Clear();
			rootNode = CreateSiteMapNodesRecursive(nextChildXmlNode, null);
			return rootNode;
		}
		public SiteMapNode LoadFromXml(string siteMapFileName) {
			SiteMapNode rootNode = null;
			XmlDocument xmlDoc = new XmlDocument();
			using (Stream siteMapFileStream = File.OpenRead(siteMapFileName)) {
				try {
					rootNode = LoadFromXmlStream(siteMapFileStream);
				} catch {
					throw new ConfigurationErrorsException(string.Format(StringResources.SiteMapControl_InvalidSiteMapFile, Path.GetFileName(siteMapFileName)));
				}
			}
			return rootNode;
		}
		private SiteMapNode CreateSiteMapNodesRecursive(XmlNode childXmlNode, SiteMapNode parentNode) {
			SiteMapNode node = null;
			if (childXmlNode.NodeType != XmlNodeType.Comment) {
				node = CreateSiteMapNodeByXmlNode(childXmlNode, parentNode);
				foreach (XmlNode xmlNode in childXmlNode.ChildNodes)
					CreateSiteMapNodesRecursive(xmlNode, node);
			}
			return node;
		}
		private SiteMapNode CreateSiteMapNodeByXmlNode(XmlNode childXmlNode, SiteMapNode parentNode) {
			if(childXmlNode.Name != SMPXmlConsts.SiteMapNodeTagName)
				throw new ConfigurationErrorsException(StringResources.UnboundProvider_OnlySiteMapNodeAllowed);
			string key = Guid.NewGuid().ToString();
			string url = GetAttributeValue(childXmlNode, "url");
			string title = GetAttributeValue(childXmlNode, "title");
			string description = GetAttributeValue(childXmlNode, "description");
			string roles = GetAttributeValue(childXmlNode, "roles");
			ArrayList rolesList = new ArrayList();
			if(roles != null) {
				if(roles.IndexOf('?') != -1)
					throw new ConfigurationErrorsException(StringResources.SiteMapControl_AuthenticationRuleNamesCantContainChar);
				string[] rolesArray = roles.Split(Separators);
				for(int num = 0; num < rolesArray.Length; num++) {
					string role = (rolesArray[num]).Trim();
					if(role.Length > 0)
						rolesList.Add(role);
				}
			}
			string resourceKey = GetAttributeValue(childXmlNode, "resourceKey");
			NameValueCollection attrCollection = new NameValueCollection();
			foreach(XmlAttribute nodeAttribute in childXmlNode.Attributes)
				if(nodeAttribute.Value != "")
					attrCollection.Add(nodeAttribute.Name, nodeAttribute.Value);
			NameValueCollection explicitAttrs = null;
			if(fProvider.EnableLocalization)
				explicitAttrs = GetExplicitResources(childXmlNode, ref title, ref description);
			if(fProvider.NeedPrepareUrl())
				url = fProvider.PrepareUrl(url);
			UnboundSiteMapNode siteMapNode = new UnboundSiteMapNode(fProvider, key, url,
				title, description, rolesList, attrCollection, explicitAttrs, resourceKey);
			if(parentNode != null)
				fProvider.AddSiteMapNode(siteMapNode, parentNode);
			return siteMapNode;
		}
		private string GetAttributeValue(XmlNode xmlNode, string attribName) {
			XmlAttribute xmlAttribute = xmlNode.Attributes[attribName];
			return xmlAttribute != null ? xmlAttribute.Value : "";
		}
		private XmlNode GetRootSiteMapXmlNode(XmlDocument siteMapXmlDoc) {
			foreach(XmlNode sourceXmlNode in siteMapXmlDoc.ChildNodes) {
				if(sourceXmlNode.Name == SMPXmlConsts.SiteMapTagName)
					return sourceXmlNode;
			}
			return null;
		}
		private NameValueCollection GetExplicitResources(XmlNode xmlNode, ref string title, ref string description) {
			string resourceKey = GetAttributeValue(xmlNode, "resourceKey");
			NameValueCollection collection = null;
			if((resourceKey != "") && !this.ValidateResource(fProvider.ResourceKey, resourceKey + ".title"))
				resourceKey = "";
			bool allowImplicitResource = resourceKey == "";
			fProvider.HandleResourceAttribute(xmlNode, ref collection, "title", ref title, 
				allowImplicitResource);
			fProvider.HandleResourceAttribute(xmlNode, ref collection, "description", ref description, 
				allowImplicitResource);
			return collection;
		}
		private bool ValidateResource(string classKey, string resourceKey) {
			try {
				HttpContext.GetGlobalResourceObject(classKey, resourceKey);
			} catch(MissingManifestResourceException) {
				return false;
			}
			return true;
		}
	}
}
