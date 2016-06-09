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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using DevExpress.SharePoint.Internal;
using DevExpress.Web.Internal;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
namespace DevExpress.SharePoint {
	[ToolboxData("<{0}:SPxListViewWebPart runat=server></{0}:SPxGridViewWebPart>")]
	[XmlRoot(Namespace = "DevExpress.SharePoint")]
	public class SPxListViewWebPart : SPxWebPart {
		private XsltListViewWebPart xslWP = new XsltListViewWebPart();
		const string QueryTemplate = "<Query>{0}</Query>";
		const string SPListKeyFieldName = "ID";
		const string InsertParameterName = "ListID";
		SPList currentList = null;
		SPView currentView = null;
		SPFolder currentRootFolder = null;
		SPxGridView grid;
		SPxListViewViewToolBar toolBar;
		Guid listID = Guid.Empty;
		bool showToolbar = true;
		[Browsable(false), WebPartStorage(Storage.Personal), XmlElement("ShowToolbar"), Description("ShowToolbar"), DefaultValue(true), FriendlyName("ShowToolbar")]
		public bool ShowToolbar {
			get { return showToolbar; }
			set {
				showToolbar = value;
				ResetControlHierarchy();
			}
		}
		[Browsable(false), WebPartStorage(Storage.Personal), XmlElement("CurrentListID"), Description("CurrentListID"), FriendlyName("CurrentListID")]
		public Guid ListID {
			get { return listID; }
			set {
				listID = value;
				ResetControlHierarchy();
			}
		}
		public bool ShouldSerializeShowToolbar() {
			return true;
		}
		protected internal SPList CurrentList {
			get {
				if(currentList == null) {
					if(ListID != Guid.Empty)
						currentList = Web.Lists[ListID];
					if(currentList == null) {
						try {
							currentList = SPxListViewUtils.GetCurrentPageList(Context);
						} catch { }
					}
				}
				return currentList;
			}
		}
		protected internal SPView CurrentView {
			get {
				if(currentView == null)
					currentView = SPxListViewUtils.GetSPListView(CurrentList, Context);
				return currentView;
			}
		}
		protected internal SPFolder CurrentRootFolder {
			get {
				if(currentRootFolder == null) {
					if(!string.IsNullOrEmpty(this.Page.Request.QueryString["RootFolder"]))
						currentRootFolder = Web.GetFolder(this.Page.Request.QueryString["RootFolder"]);
					else
						currentRootFolder = CurrentList.RootFolder;
				}
				return currentRootFolder;
			}
		}
		protected internal SPxGridView Grid { get { return grid; } }
		protected internal SPxListViewViewToolBar ToolBar { get { return toolBar; } }
		protected internal SPWeb Web { get { return SPControl.GetContextWeb(Context); } }
		public override EditorPartCollection CreateEditorParts()
		{
			List<EditorPart> collection = new List<EditorPart>();
			EditorPart editorPart = new SPxListViewToolPart() { ID = this.ID + "_ep" };
			collection.Add(editorPart);
			return new EditorPartCollection(base.CreateEditorParts(), collection);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.currentList = null;
			this.currentView = null;
			this.grid = null;
			this.toolBar = null;
		}
		protected override void CreateControlHierarchy() {
			if(CurrentList != null) {
				if(ShowToolbar)
					CreateToolbarControl(CurrentList);
				CreateGridViewControl(CurrentList);
				BindGridWithList(CurrentList);
			}
		}
		protected override bool HasContent() {
			return CurrentList != null;
		}
		protected override string GetDefaultMessage() {
			return GetOpenToolPaneLinkString(StringResources.WebPart_OpenToolPane) +
				StringResources.ListViewWebPart_DefaultMessage;
		}
		protected virtual void BindGridWithList(SPList list) {
			if(list == null)
				return;
			SPDataSource spDataSource = new SPDataSource();
			spDataSource.List = list;
			spDataSource.DataSourceMode = SPDataSourceMode.List;
			spDataSource.SelectCommand = string.Format(QueryTemplate, CurrentView.Query);
			Parameter listIDParam = new Parameter(InsertParameterName);
			listIDParam.DefaultValue = list.ID.ToString("B").ToUpper();
			spDataSource.InsertParameters.Add(listIDParam);
			Parameter listRootFolder = new Parameter("RootFolder");
			listRootFolder.DefaultValue = CurrentRootFolder.ServerRelativeUrl;
			spDataSource.SelectParameters.Add(listRootFolder);
			Grid.DataSource = spDataSource;
			Grid.DataBind();
		}
		protected void CreateGridViewControl(SPList list) {
			this.grid = new SPxGridView();
			Grid.KeyFieldName = SPListKeyFieldName;
			Grid.Web = Web;
			Grid.ID = "ASPxGridView";
			Grid.Width = Unit.Percentage(100);
			Grid.List = list;
			Controls.Add(Grid);
		}
		protected void CreateToolbarControl(SPList list) {
			try {
				SPContext context = SPContext.GetContext(Context, CurrentView.ID, list.ID, list.ParentWeb);
				this.toolBar = new SPxListViewViewToolBar();
				ToolBar.RenderContext = context;
				if(string.IsNullOrEmpty(CurrentView.ToolbarTemplateName))
					ToolBar.TemplateName = CurrentView.ToolbarTemplateName;
				if(!IsViewSelectorVisible())
					ToolBar.ShowViewSelector = false;
				Controls.Add(ToolBar);
			} catch { }
		}
		protected bool IsViewSelectorVisible() {
			return SPxListViewUtils.GetCurrentPageSPViewForList(CurrentList, Context) != null;
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			ClientScriptManager javaManager = this.Page.ClientScript;
			if(!javaManager.IsClientScriptBlockRegistered(this.Page.GetType(), "InitGridCommandItems"))
				javaManager.RegisterClientScriptBlock(this.Page.GetType(), "InitGridCommandItems", CreatrePopUpScriptBlock());
		}
		protected string CreatrePopUpScriptBlock() {
			string script = "<script type=text/javascript>" +
								"function InitGridCommandItems (grid) {\n" +
									"var popupMenu = ASPx.GetControlCollection().Get(grid.name + '_SPPopUpControl');\n" +
									"popupMenu.dxSPGRid = grid;\n" +
									"var popupElements = ASPx.GetChildNodesByClassName(grid.GetMainElement(), 'dxspg_ci');\n" +
									"for(var i = 0; i < popupElements.length; i++) {\n" +
									   "popupMenu.AddPopupElement(popupElements[i]);\n" +
									 "}\n" +
									 "popupMenu.PopUp.AddHandler(function(s, e) {\n" +
											"var currentPopUp = s.GetCurrentPopupElement();\n" +
											"var visibleIndex = currentPopUp.id.substring(currentPopUp.id.lastIndexOf('_')+1,currentPopUp.id.length);\n" +
											"popupMenu.dxSPGridId=visibleIndex;\n" +
											"var itemID=grid.GetRowKey(visibleIndex);\n" +
											"var itemCount = s.GetItemCount();\n" +
											"for(var j = 0; j < itemCount-1; j++) {\n" +
											"   var itemUrl = s.GetItem(j).GetNavigateUrl();\n" +
											"   itemUrl = itemUrl.split('clientRowID').join(itemID);\n" +
											"   s.GetItem(j).SetNavigateUrl(itemUrl);\n" +
											"}\n" +
											"var rowInfo = grid.cpListItemPermissions[itemID];\n" +
											"if(rowInfo!=null){\n" +
											"s.GetItemByName('btnEditItem').SetVisible(rowInfo['Edit']);\n" +
											"s.GetItemByName('btnDeleteItem').SetVisible(rowInfo['Delete']);\n" +
											"s.GetItemByName('btnWorkflow').SetVisible(rowInfo['Workflow']);\n" +
											"s.GetItemByName('btnVersionHistory').SetVisible(rowInfo['ViewVersions']);\n" +
											"}\n" +
									 "});\n" +
								"}\n" +
								"</script>";
			return script;
		}		
	}
}
