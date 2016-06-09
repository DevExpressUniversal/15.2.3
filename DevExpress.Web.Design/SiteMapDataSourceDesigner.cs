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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Design;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design.WebControls;
using System.Web.UI.Design;
using System.Windows.Forms;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using EnvDTE;
namespace DevExpress.Web.Design {
	using DevExpress.Web.Design.Utils;
	public class ASPxSiteMapDataSourceDesigner : SiteMapDataSourceDesigner {
		private IDesignerHost fDesignerHost = null;
		private EditableSiteMapProvider fEditableSiteMapProvider = null;
		private ASPxSiteMapDataSource fSiteMapDataSource = null;
		private DesignTimeSiteMapProvider fDesignTimeSiteMapProvider = null;
		public override DesignerActionListCollection ActionLists {
			get {
				DesignerActionListCollection collection = new DesignerActionListCollection();
				collection.AddRange(base.ActionLists);
				collection.Add(new SiteMapDataSourceDesignerActionList(this));
				return collection;
			}
		}
		protected internal ASPxSiteMapDataSource SiteMapDataSource {
			get { return fSiteMapDataSource; }
		}
		protected internal SiteMapProvider DesignTimeSiteMapProvider {
			get {
				if (fDesignTimeSiteMapProvider == null) {
					fDesignTimeSiteMapProvider = new DesignTimeSiteMapProvider(fDesignerHost);
					fDesignTimeSiteMapProvider.SiteMapFileName = fSiteMapDataSource.GetSiteMapFileName();
				}
				else
					fDesignTimeSiteMapProvider.SiteMapFileName = fSiteMapDataSource.GetSiteMapFileName();
				return fDesignTimeSiteMapProvider;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			fSiteMapDataSource = (ASPxSiteMapDataSource)component;
			fDesignerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
		}
		public override DesignerHierarchicalDataSourceView GetView(string viewPath) {
			return new SiteMapDesignerHierarchicalDataSourceViewInternal(this, viewPath);
		}
		public void CreateNewSiteMapFile() {
			string newFileName = GetNewSiteMapFileName();
			CreateSiteMapFile(newFileName);
			fSiteMapDataSource.SiteMapFileName = newFileName;
			RefreshSchema(true);
			EditSiteMapFile();
		}
		public void EditSiteMapFile() {
			DialogResult result = ShowSiteMapNodesDesigner();
			if (result != DialogResult.Cancel) 
				RefreshSchema(true);			
		}
		protected void CreateSiteMapFile(string fileName) {
			EnvDTE.ProjectItem projectItem = (EnvDTE.ProjectItem)GetService(typeof(EnvDTE.ProjectItem));
			IWebApplication webApp = (IWebApplication)GetService(typeof(IWebApplication));
			DesignerUtils.AddNewSiteMapFileToProject(projectItem.ContainingProject,
				webApp, fileName, true);
		}
		public void ShowAboutForm() {
			AboutDialogHelper.ShowSiteMapAbout(Component.Site);
		}
		public string GetFormCaption() {
			return "Editing " + SiteMapDataSource.GetSiteMapFileName();
		}
		public void ShowAbout() {
		}
		protected DialogResult ShowSiteMapNodesDesigner() {
			SiteMapNodeCollection siteMapNodes = CreateEditingSiteMapNodeCollection();
			SiteMapNodesEditorForm form = new SiteMapNodesEditorForm(Component, null, Component.Site, siteMapNodes, fEditableSiteMapProvider, fSiteMapDataSource.GetSiteMapFileName());
			return ShowDialog(form);
		}
		protected System.Windows.Forms.DialogResult ShowDialog(System.Windows.Forms.Form form) {
			return DesignUtils.ShowDialog(Component.Site, form);
		}
		protected internal bool IsExistSiteMapFile() {
			IWebApplication webApplication = (IWebApplication)Component.Site.GetService(typeof(IWebApplication));		  
			return DesignerUtils.IsExistProjectItem(fSiteMapDataSource.GetSiteMapFileName(), 
				webApplication);
		}
		private EditableSiteMapProvider CreateEditingSiteMapProvider() {
			IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			DesignTimeSiteMapProvider desigTimeProvider = new DesignTimeSiteMapProvider(host);
			desigTimeProvider.SiteMapFileName = SiteMapDataSource.GetSiteMapFileName();
			desigTimeProvider.ReBuild();
			EditableSiteMapProvider ret = new EditableSiteMapProvider(host, desigTimeProvider.DataState, SiteMapDataSource.GetSiteMapFileName());
			ret.Exception = desigTimeProvider.Exception;		 
			return ret;
		}
		private SiteMapNodeCollection CreateEditingSiteMapNodeCollection() {
			SiteMapNodeCollection newCollection = null;
			fEditableSiteMapProvider = CreateEditingSiteMapProvider();
			newCollection = new SiteMapNodeCollection(fEditableSiteMapProvider.RootNode);
			return newCollection;
		}
		private string GetNewSiteMapFileName() {
			IWebApplication webApp = (IWebApplication)GetService(typeof(IWebApplication));
			string[] fileNames = Directory.GetFiles(webApp.RootProjectItem.PhysicalPath, "web*.sitemap");
			List<string> files = new List<string>();
			for (int i = 0; i < fileNames.Length; i++)
				files.Add(Path.GetFileName(fileNames[i]));
			return "~/" + DesignerUtils.GetNextSiteMapFileName(files.ToArray());
		}
	}
	public class SiteMapDataSourceDesignerActionList : DesignerActionList {
		private ASPxSiteMapDataSourceDesigner fSiteMapDataSourceDesigner;
		public override bool AutoShow {
			get { return true; }
			set { }
		}
		public SiteMapDataSourceDesignerActionList(ASPxSiteMapDataSourceDesigner siteMapDataSourceDesigner)
			: base(siteMapDataSourceDesigner.Component) {
			fSiteMapDataSourceDesigner = siteMapDataSourceDesigner;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = new DesignerActionItemCollection();
			collection.Add(new DesignerActionMethodItem(this, "CreateNewSiteMap",
				StringResources.SiteMapControlActionList_Create,
				StringResources.ActionList_MiscCategory,
				StringResources.SiteMapControlActionList_CreateSiteMapDescription, true));
			collection.Add(new DesignerActionMethodItem(this, "EditSiteMap",
				StringResources.SiteMapControlActionList_Editor,
				StringResources.ActionList_MiscCategory,
				StringResources.SiteMapControlActionList_Description, true));
			collection.Add(new DesignerActionMethodItem(this, "ShowAbout",
				StringResources.ActionList_About,
				StringResources.ActionList_MiscCategory, "", false));
			return collection;
		}
		protected void CreateNewSiteMap() {
			fSiteMapDataSourceDesigner.CreateNewSiteMapFile();
		}
		protected void EditSiteMap() {
			fSiteMapDataSourceDesigner.EditSiteMapFile();
		}
		protected void ShowAbout() {
			fSiteMapDataSourceDesigner.ShowAbout();
		}
	}
	public class SiteMapDesignerHierarchicalDataSourceViewInternal : SiteMapDesignerHierarchicalDataSourceView {
		ASPxSiteMapDataSourceDesigner fSiteMapDataSourceDesigner = null;
		public SiteMapDesignerHierarchicalDataSourceViewInternal(ASPxSiteMapDataSourceDesigner owner, string viewPath)
			: base(owner, viewPath) {
			fSiteMapDataSourceDesigner = owner;
		}
		public override IHierarchicalEnumerable GetDesignTimeData(out bool isSampleData) {
			IHierarchicalEnumerable ret = null;
			string siteMapProvider = fSiteMapDataSourceDesigner.SiteMapDataSource.SiteMapProvider;
			string startingNodeUrl = fSiteMapDataSourceDesigner.SiteMapDataSource.StartingNodeUrl;
			fSiteMapDataSourceDesigner.SiteMapDataSource.Provider = fSiteMapDataSourceDesigner.DesignTimeSiteMapProvider;
			try {
				fSiteMapDataSourceDesigner.SiteMapDataSource.StartingNodeUrl = null;
				ret = (fSiteMapDataSourceDesigner.SiteMapDataSource as IHierarchicalDataSource).GetHierarchicalView(base.Path).Select();
				isSampleData = false;
			}
			finally {
				fSiteMapDataSourceDesigner.SiteMapDataSource.StartingNodeUrl = startingNodeUrl;
				fSiteMapDataSourceDesigner.SiteMapDataSource.SiteMapProvider = siteMapProvider;
			}
			return ret;
		}
	}
	public class EditableSiteMapProvider : DesignTimeSiteMapProvider {
		UrlInfo fUrlInfo = null;
		public UrlInfo UrlInfo {
			get { return fUrlInfo; }
		}
		public EditableSiteMapProvider(IDesignerHost host)
			: base(host) {
		}
		public EditableSiteMapProvider(IDesignerHost host, ProviderDataState providerDataState, string siteMapFileName)
			: base(host) {
			DataState = providerDataState;
			fUrlInfo = CreateUrlInfo();
			if (providerDataState != ProviderDataState.Normal)
				SiteMapFileName = "";
			else {
				SiteMapFileName = siteMapFileName;
				ReBuild();
			}
		}
		public void DecreaseIndent(SiteMapNode node) {
			if ((node.ParentNode != null) && (node.ParentNode.ParentNode != null)) {
				int parentIndex = node.ParentNode.ParentNode.ChildNodes.IndexOf(node.ParentNode);
				SiteMapNode parentNode = node.ParentNode.ParentNode;
				RemoveNode(node);
				GetChildNodeCollection(node, parentNode).Insert(parentIndex + 1, node);
				fUrlInfo.AddString(node.Url);
			}
		}
		public void IncreaseIndent(SiteMapNode node) {
			if ((node.ParentNode != null) && (node.PreviousSibling != null))
				AddSiteMapNode(node, node.PreviousSibling);
		}
		public void InsertSiteMapNode(int index, SiteMapNode parentNode, SiteMapNode insertingNode) {
			if ((parentNode.HasChildNodes) && (index < parentNode.ChildNodes.Count)) {
				AddSiteMapNode(insertingNode, parentNode);
				RemoveNode(insertingNode);
				GetChildNodeCollection(insertingNode, parentNode).Insert(index, insertingNode);
				fUrlInfo.AddString(insertingNode.Url);
			}
		}
		public void MoveDownSiteMapNode(SiteMapNode node) {
			if ((node.ParentNode != null) && (node.NextSibling != null)) {
				SiteMapNode parentNode = node.ParentNode;
				int oldIndex = GetChildNodeCollection(node, parentNode).IndexOf(node.NextSibling);
				RemoveNode(node);
				GetChildNodeCollection(node, parentNode).Insert(oldIndex, node);
				fUrlInfo.AddString(node.Url);
			}
		}
		public void MoveUpSiteMapNode(SiteMapNode node) {
			if ((node.ParentNode != null) && (node.PreviousSibling != null)) {
				SiteMapNode parentNode = node.ParentNode;
				int oldIndex = GetChildNodeCollection(node, parentNode).IndexOf(node.PreviousSibling);
				RemoveNode(node);
				GetChildNodeCollection(node, parentNode).Insert(oldIndex, node);
				fUrlInfo.AddString(node.Url);
			}
		}
		public void ReplaceToNewParent(SiteMapNode newParent, SiteMapNode node) {
			if ((newParent != null) && (node != null) &&
				(node != newParent) && (!IsNodeIsChildOfParent(node, newParent))) {
				RemoveNode(node);
				GetChildNodeCollection(node, newParent).Add(node);
				fUrlInfo.AddString(node.Url);
			}
		}
		protected internal override bool NeedPrepareUrl() {
			return false;
		}
		protected override void AddNode(SiteMapNode node, SiteMapNode parentNode) {
			base.AddNode(node, parentNode);
			if (DataState != ProviderDataState.Sample)
				fUrlInfo.AddString(node.Url);
		}
		protected override void ClearInternal() {
			fUrlInfo.ClearAll();
			base.ClearInternal();
		}
		protected override void RemoveNode(SiteMapNode node) {
			fUrlInfo.DeleteString(node.Url);
			SiteMapNodeCollection nodes = node.GetAllNodes();
			foreach (SiteMapNode curNode in nodes)
				fUrlInfo.DeleteString(curNode.Url);
			base.RemoveNode(node);
		}
		protected override SiteMapNode CreateRootNode() {
			SiteMapNode ret = base.CreateRootNode();
			if (DataState != ProviderDataState.Sample)
				UrlInfo.AddString(ret.Url);
			return ret;
		}
		private UrlInfo CreateUrlInfo() {
			List<string> strs = new List<string>();
			return new UrlInfo(strs.ToArray());
		}
		private bool IsNodeIsChildOfParent(SiteMapNode node, SiteMapNode parentNode) {
			SiteMapNode currentParentNode = node.ParentNode;
			while (currentParentNode != null) {
				if (currentParentNode == parentNode)
					return true;
				currentParentNode = currentParentNode.ParentNode;
			}
			return false;
		}
	}
	public class ScanSiteMapNode : UnboundSiteMapNode {
		public ScanSiteMapNode(SiteMapProvider provider, string key)
			: this(provider, key, null, null, null, null, null, null, null) {
		}
		public ScanSiteMapNode(SiteMapProvider provider, string key, string url)
			: this(provider, key, url, null, null, null, null, null, null) {
		}
		public ScanSiteMapNode(SiteMapProvider provider, string key, string url, string title)
			: this(provider, key, url, title, null, null, null, null, null) {
		}
		public ScanSiteMapNode(SiteMapProvider provider, string key, string url, string title, string description)
			: this(provider, key, url, title, description, null, null, null, null) {
		}
		public ScanSiteMapNode(SiteMapProvider provider, string key, string url, string title, string description, IList roles, NameValueCollection attributes, NameValueCollection explicitResourceKeys, string implicitResourceKey)
			: base(provider, key, url, title, description, roles, attributes, explicitResourceKeys, implicitResourceKey) {
		}
		public override string ToString() {
			return GetText(true);
		}
		public string GetText(bool isShowPageUrl) {
			string ret = "";
			if ((Title == "") && (Url == ""))
				ret = "SiteMapNode";
			else {
				if ((Path.GetFileName(Url) != "") && (isShowPageUrl)) 
					ret = "[" + Path.GetFileName(Url) + "] ";
				if (Title != "")
					ret = ret + Title;
				if (ret == "")
					ret = "SiteMapNode";
			}
			return ret;
		}
	}
	public class ScanSiteMapProvider : EditableSiteMapProvider {
		public ScanSiteMapProvider(IDesignerHost host)
			: base(host) {
		}
		public ScanSiteMapProvider(IDesignerHost host, ProviderDataState providerDataState)
			: base(host, providerDataState, "") {
		}
		protected override UnboundSiteMapNode CreateNodeInternal(string url, string title, string description, string key, IList roles, NameValueCollection attributes) {
			return new ScanSiteMapNode(this, key, url, title, description, null, attributes, null, null);
		}
	}
}
