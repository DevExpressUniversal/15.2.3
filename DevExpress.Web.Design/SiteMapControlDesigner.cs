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
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Windows.Forms.Design;
using System.Xml;
using DevExpress.Utils.Design;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxSiteMapControlDesignerBase : ASPxHierarchicalDataWebControlDesigner {
		private ASPxSiteMapControlBase fSiteMapControlBase = null;
		private SiteMapProvider fSampleSiteMapProvider = null;
		protected SiteMapProvider SampleSiteMapProvider {
			get {
				if (fSampleSiteMapProvider == null)
					fSampleSiteMapProvider = new SampleSiteMapProvider(DesignerHost);
				return fSampleSiteMapProvider;
			}
		}
		protected internal IServiceProvider ServiceProvider {
			get { return (IServiceProvider)this.GetService(typeof(IServiceProvider)); }
		}
		protected internal ASPxSiteMapControlBase SiteMapControlBase {
			get { return fSiteMapControlBase; }
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Columns", "Columns");
		}
		public override void Initialize(IComponent component) {
			fSiteMapControlBase = (ASPxSiteMapControlBase)component;
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new SiteMapDesignerActionListBase(this);
		}
		public override bool HasClientSideEvents() {
			return false;
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new SiteMapCommonFormDesigner(fSiteMapControlBase, DesignerHost)));
		}
		protected override void CreateDataSource(string propertyName) {
			System.Web.UI.Design.ControlDesigner.InvokeTransactedChange(Component, new TransactedChangeCallback(CreateDataSourceCallback), propertyName, StringResources.DataControl_CreateDataSourceTransaction);
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			ASPxSiteMapControlBase siteMapControl = dataControl as ASPxSiteMapControlBase;
			base.DataBind(siteMapControl);
		}
		protected override IHierarchicalEnumerable GetSampleDataSource() {
			SiteMapNodeCollection nodes = new SiteMapNodeCollection();
			nodes.Add(SampleSiteMapProvider.RootNode);
			return nodes;
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			PropertyDescriptor descriptor = (PropertyDescriptor)properties["DataSource"];
			descriptor = TypeDescriptor.CreateProperty(GetType(), descriptor, new Attribute[] { new TypeConverterAttribute(typeof(SiteMapDataSourceConverter)) });
			properties["DataSource"] = descriptor;
			descriptor = (PropertyDescriptor)properties["DataSourceID"];
			descriptor = TypeDescriptor.CreateProperty(GetType(), descriptor, new Attribute[] { new TypeConverterAttribute(typeof(SiteMapDataSourceIDConverter)) });
			properties["DataSourceID"] = descriptor;
		}
		protected internal override string[] GetDataBindingSchemaFields() {
			return new string[3] { "Url", "Title", "Description" };
		}
		protected internal override Type GetDataBindingSchemaItemType() {
			return typeof(SiteMapNode);
		}
		protected Style GetTemplateStyle(int level, string templateName) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(fSiteMapControlBase.GetControlStyle());
			switch (templateName) {
				case "NodeTemplate":
					style.CopyFrom(fSiteMapControlBase.GetNodeStyle(level));
					break;
				case "ColumnSeparatorTemplate":
					style.CopyFrom(fSiteMapControlBase.GetColumnSeparatorStyle());
					break;
			}
			return style;
		}
		private bool CreateDataSourceCallback(object context) {
			string id = "";
			System.Windows.Forms.DialogResult result = ShowCreateDataSourceDialog(this, typeof(SiteMapDataSource), true, out id);
			if (!string.IsNullOrEmpty(id))
				DataSourceID = id;
			return (result == System.Windows.Forms.DialogResult.OK);
		}
	}
	public class ASPxSiteMapControlDesigner : ASPxSiteMapControlDesignerBase {
		private static string[] fControlTemplateNames = new string[] { "NodeTemplate", "NodeTextTemplate", "ColumnSeparatorTemplate" };
		private static string fLevelPropertyTemplateCaption = "LevelProperties[{0}]";
		private static string[] fLevelPropertyTemplateNames = new string[] { "NodeTemplate", "NodeTextTemplate" };
		public ASPxSiteMapControl SiteMapControl {
			get { return SiteMapControlBase as ASPxSiteMapControl; }
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("LevelProperties", "LevelProperties");
		}
		protected override TemplateGroupCollection CreateTemplateGroups() {
			TemplateGroupCollection templateGroups = base.CreateTemplateGroups();
			for(int i = 0; i < SiteMapControl.LevelProperties.Count; i++) {
				LevelProperties levelProperty = SiteMapControl.LevelProperties[i];
				TemplateGroup templateGroup = new TemplateGroup(string.Format(fLevelPropertyTemplateCaption, i));
				for(int j = 0; j < fLevelPropertyTemplateNames.Length; j++) {
					TemplateDefinition templateDefinition = new TemplateDefinition(this, fLevelPropertyTemplateNames[j],
						levelProperty, fLevelPropertyTemplateNames[j],
						GetTemplateStyle(i, fLevelPropertyTemplateNames[j]));
					templateDefinition.SupportsDataBinding = true;
					templateGroup.AddTemplateDefinition(templateDefinition);
				}
				templateGroups.Add(templateGroup);
			}
			for(int i = 0; i < fControlTemplateNames.Length; i++) {
				TemplateGroup templateGroup = new TemplateGroup(fControlTemplateNames[i]);
				TemplateDefinition templateDefinition = new TemplateDefinition(this, fControlTemplateNames[i],
					Component, fControlTemplateNames[i], GetTemplateStyle(-1, fControlTemplateNames[i]));
				templateDefinition.SupportsDataBinding = true;
				templateGroup.AddTemplateDefinition(templateDefinition);
				templateGroups.Add(templateGroup);
			}
			return templateGroups;
		}
	}
	public class SiteMapDesignerActionListBase : ASPxWebControlDesignerActionList {
		private ASPxSiteMapControlDesignerBase fSiteMapControlDesignerBase;
		public byte ColumnCount {
			get { return fSiteMapControlDesignerBase.SiteMapControlBase.ColumnCount; }
			set {
				IComponent component = fSiteMapControlDesignerBase.Component;
				TypeDescriptor.GetProperties(component)["ColumnCount"].SetValue(component, value); 
			}
		}
		public SiteMapDesignerActionListBase(ASPxSiteMapControlDesignerBase siteMapControlDesignerBase)
			: base(siteMapControlDesignerBase) {
			fSiteMapControlDesignerBase = siteMapControlDesignerBase;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("ColumnCount",
				StringResources.SiteMapControlActionList_EditColumnCount,
				StringResources.SiteMapControlActionList_EditColumnCount,
				StringResources.SiteMapControlActionList_EditColumnCountDescription));
			return collection;
		}
	}
	public class SiteMapDataWebControlActionList : HierarchicalDataWebControlActionList {
		[TypeConverter(typeof(SiteMapDataSourceIDConverter))]
		public override string DataSourceID {
			get { return base.DataSourceID; }
			set { base.DataSourceID = value; }
		}
		public SiteMapDataWebControlActionList(ASPxHierarchicalDataWebControlDesigner controlDesigner, IHierarchicalDataSourceDesigner dataSourceDesigner)
			: base(controlDesigner, dataSourceDesigner) {
		}
	}
	public class NodeIndexUtils {
		public static List<int> CreateNodeIndexList(SiteMapColumn column) {
			return CreateNodeIndexList(column.SiteMapControl, column.Index);
		}
		public static List<int> CreateNodeIndexList(ITypeDescriptorContext context) {
			SiteMapColumn column = GetColumn(context);
			ASPxSiteMapControlBase siteMapControl = column.SiteMapControl;
			IDesignerHost service = (IDesignerHost)siteMapControl.Site.GetService(typeof(IDesignerHost));
			if(service != null) {
				ASPxSiteMapControlDesignerBase designer = service.GetDesigner(siteMapControl) as ASPxSiteMapControlDesignerBase;
				if (designer != null && designer.ViewControl is ASPxSiteMapControlBase)
					siteMapControl = (ASPxSiteMapControlBase)designer.ViewControl;
			}
			return CreateNodeIndexList(siteMapControl, column.Index);
		}
		public static SiteMapColumn GetColumn(ITypeDescriptorContext context) {
			return context != null ? context.Instance as SiteMapColumn : null;
		}
		static public List<int> CreateNodeIndexList(ASPxSiteMapControlBase siteMapControl, int columnIndex) {
			List<int> columnIndexList = new List<int>();
			columnIndexList.Add(-1);
			if ((siteMapControl.RootNodes != null) && (siteMapControl.RootNodes.Count > 0)) {
				if(columnIndex == 0)
					columnIndexList.Add(0);
				else {
					int firstNodeIndex = NodeIndexUtils.GetPrevColumn(siteMapControl.Columns[columnIndex]).StartingNodeIndex;
					if (firstNodeIndex < 0)
						firstNodeIndex = 0;
					for (int i = firstNodeIndex + 1; i < siteMapControl.RootNodes.Count; i++) {
						columnIndexList.Add(i);
					}
				}
			}
			return columnIndexList;
		}
		static SiteMapColumn GetPrevColumn(SiteMapColumn column) {
			if ((column != null) && (column.Index > 0))
				return (column.Collection as SiteMapColumnCollection)[column.Index - 1];
			else
				return null;
		}
	}
	public class NodeIndexEditor : UITypeEditor {
		private System.Windows.Forms.ListBox fListBox = null;
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object obj) {
			if (context != null && provider != null && context.PropertyDescriptor != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) {
					fListBox = new UIListBox(edSvc);
					SiteMapColumn column = NodeIndexUtils.GetColumn(context);
					FillListBox(fListBox, column);
					fListBox.SelectedIndex = GetSeletedItemIndex(column);
					edSvc.DropDownControl(fListBox);
					if (fListBox.SelectedIndex != -1) {
						column.StartingNodeIndex = GetNodeIndexBySelectedIndex(column, fListBox.SelectedIndex);
						IComponentChangeService chSrv = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
						chSrv.OnComponentChanged(column.SiteMapControl, null, null, null);
					}
					fListBox = null;
					edSvc = null;
				}
			}
			return obj;
		}
		protected void FillListBox(System.Windows.Forms.ListBox listBox, SiteMapColumn column) {
			List<int> indexes = NodeIndexUtils.CreateNodeIndexList(column);
			ASPxSiteMapControlBase siteMapControl = column.SiteMapControl;
			for (int i = 0; i < indexes.Count; i++) {
				int nodeIndex = indexes[i];
				if (nodeIndex == -1)
					listBox.Items.Add("-1");
				else
					listBox.Items.Add(nodeIndex.ToString() + "-" + 
						siteMapControl.RootNodes[nodeIndex].Title);
			}
		}
		protected int GetSeletedItemIndex(SiteMapColumn column) {
			List<int> indexes = NodeIndexUtils.CreateNodeIndexList(column);
			return indexes.IndexOf(column.StartingNodeIndex);
		}
		private int GetNodeIndexBySelectedIndex(SiteMapColumn column, int selectedIndex) {
			List<int> indexes = NodeIndexUtils.CreateNodeIndexList(column);
			return indexes[selectedIndex];
		}
	}
	public enum ProviderDataState { Empty, Normal,Sample };
	public class DesignTimeSiteMapProvider : UnboundSiteMapProviderBase {
		private IDesignerHost fDesignerHost = null;
		private string fException = "";
		private ProviderDataState fProviderDataState = ProviderDataState.Normal;
		public override SiteMapNode CurrentNode {
			get {
				SiteMapNode ret = FindSiteMapNode(GetDocumentAppRelativeUrl());
				if (ret == null)
					ret = base.CurrentNode;
				return ret;
			}
		}
		public string Exception {
			get {
				string str = fException;
				fException = "";
				return str;
			}
			set { fException = value; }
		}
		protected internal ProviderDataState DataState {
			get { return fProviderDataState; }
			set { fProviderDataState = value; }
		}
		public DesignTimeSiteMapProvider(IDesignerHost host)
			: base() {
			if (host == null)
				throw new ArgumentNullException(StringResources.InvalidDesignerHost);
			fDesignerHost = host;
			SiteMapFileName = DefaultSiteMapFileName;
		}
		public void ReBuild() {
			try {
				fProviderDataState = ProviderDataState.Normal;
				Clear();
				RefreshHierarchy();
			}
			catch {
				fProviderDataState = ProviderDataState.Sample;
			}
		}
		protected internal string GetExceptionString() {
			string str = fException;
			fException = "";
			return str;
		}
		protected internal override string MapPath(string virtualPath) {
			return DesignUtils.MapPath(fDesignerHost, virtualPath);
		}
		protected override SiteMapNode CreateRootNode() {
			try {
				return base.CreateRootNode();
			}
			catch(Exception e) {
				fException = e.Message;
				return CreateSampleSiteMap();
			}
		}
		protected internal override void HandleResourceAttribute(XmlNode xmlNode, ref NameValueCollection collection, string attrName,
			ref string attrValueString, bool allowImplicitResource) {
			attrValueString = HandleResourceAttributeInternal(attrValueString);
		}
		protected internal SiteMapNode CreateSampleSiteMap() {
			Clear();
			fProviderDataState = ProviderDataState.Sample;
			UnboundSiteMapNode rootNode = new UnboundSiteMapNode(this, StringResources.UnboundProvider_RootNode_Key,
				StringResources.UnboundProvider_RootNode_Url, StringResources.UnboundProvider_RootNode_Title);
			UnboundSiteMapNode firstCategNode = CreateNode("", string.Format(StringResources.UnboundProvider_Category, "1"));
			AddNode(firstCategNode, rootNode);
			for (int p = 0; p < 2; p++) {
				UnboundSiteMapNode parentNode = CreateNode(string.Format(StringResources.UnboundProvider_ParentNode_Url, p),
					string.Format(StringResources.UnboundProvider_ParentNode_Title, p));
				AddNode(parentNode, firstCategNode);
				for (int n = 0; n < 2; n++) {
					UnboundSiteMapNode childNode = CreateNode(string.Format(StringResources.UnboundProvider_ChildNode_Url, " " + n),
						string.Format(StringResources.UnboundProvider_ChildNode_Title, " " + n));
					AddNode(childNode, parentNode);
				}
			}
			UnboundSiteMapNode secondCategNode = CreateNode("", string.Format(StringResources.UnboundProvider_Category, "2"));
			AddNode(secondCategNode, rootNode);
			for (int n = 0; n < 2; n++) {
				UnboundSiteMapNode childNode = CreateNode(string.Format(StringResources.UnboundProvider_ChildNode_Url, " " + n),
					string.Format(StringResources.UnboundProvider_ChildNode_Title, " " + n));
				AddNode(childNode, secondCategNode);
			}
			return rootNode;
		}
		protected string GetDocumentAppRelativeUrl() {
			if (fDesignerHost.RootComponent != null) {
				WebFormsRootDesigner designer = fDesignerHost.GetDesigner(fDesignerHost.RootComponent) as WebFormsRootDesigner;
				if (designer != null)
					return designer.DocumentUrl;
			}
			return "";
		}
		private string HandleResourceAttributeInternal(string attrValueString) {
			if(attrValueString != "") {
				string resourceAttrs = attrValueString.TrimStart(new char[] { ' ' });
				if((resourceAttrs.Length <= 10) ||
					!resourceAttrs.ToLower(CultureInfo.InvariantCulture).StartsWith("$resources:", StringComparison.Ordinal))
					return attrValueString;
				int index = resourceAttrs.IndexOf(',');
				if(index != -1) {
					index = resourceAttrs.IndexOf(',', index + 1);
					if(index != -1)
						return resourceAttrs.Substring(index + 1);
				}
			}
			return "";
		}
	}
	public class SampleSiteMapProvider : DesignTimeSiteMapProvider {
		public SampleSiteMapProvider(IDesignerHost host)
			: base(host) {
			SiteMapFileName = "";
		}
		protected override SiteMapNode CreateRootNode() {
			return CreateSampleSiteMap();
		}
	}
	public class SiteMapCommonFormDesigner : CommonFormDesigner {
		public SiteMapCommonFormDesigner(ASPxSiteMapControlBase siteMap, IServiceProvider provider)
			: base(siteMap, provider) {
		}
		ASPxSiteMapControlBase SiteMap { get { return Control as ASPxSiteMapControlBase; } }
		protected override void CreateMainGroupItems() {
			if(SiteMap is ASPxSiteMapControl) 
				MainGroup.Add(CreateDesignerItem("LevelProperties", "Level Properties", typeof(ItemsEditorFrame), SiteMap, ItemsItemImageIndex, new SiteMapLevelsEditorOwner((ASPxSiteMapControl)SiteMap, Provider)));
			MainGroup.Add(CreateDesignerItem("Columns", "Columns", typeof(ItemsEditorFrame), SiteMap, ColumnsItemImageIndex, new SiteMapColumnsEditorOwner(SiteMap, Provider)));
		}
	}
	public class SiteMapLevelsEditorOwner : FlatCollectionItemsOwner<LevelProperties> {
		public SiteMapLevelsEditorOwner(ASPxSiteMapControl siteMap, IServiceProvider provider)
			: base(siteMap, provider, siteMap.LevelProperties, "Level Properties") {
		}
	}
	public class SiteMapColumnsEditorOwner : FlatCollectionItemsOwner<SiteMapColumn> {
		public SiteMapColumnsEditorOwner(ASPxSiteMapControlBase siteMap, IServiceProvider provider)
			: base(siteMap, provider, siteMap.Columns, "Columns") {
		}
		protected internal SiteMapColumnsEditorOwner(SiteMapColumnCollection columns)
			: base(null, null, columns, "Columns") {
		}
		public int ColumnsAmount { get { return Items.Count; } }
		protected override List<DesignEditorMenuRootItemActionType> GetToolbarActionTypes() {
			var result = base.GetToolbarActionTypes();
			result.Add(DesignEditorMenuRootItemActionType.SetItemsAmount);
			return result;
		}
	}
}
