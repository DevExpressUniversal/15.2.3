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
using System.Web.UI;
using System.Collections.Generic;
using System.Collections;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using DevExpress.Web;
using DevExpress.Web;
using DevExpress.Web;
using DevExpress.Web.Rendering;
using DevExpress.SharePoint.Internal;
using System.Xml;
using DevExpress.Web.Internal;
using DevExpress.Data;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System.Web.Security;
using System.Web;
using DevExpress.Web.ASPxHtmlEditor;
using System.Linq;
using System.Data;
using DevExpress.Web;
using System.Text;
using System.Globalization;
using Microsoft.SharePoint.Administration;
using System.IO;
using System.Collections.Specialized;
using Microsoft.SharePoint.Client;
using System.Xml.Xsl;
namespace DevExpress.SharePoint.Internal {
	public class SPViewInfoInfo : SPViewInfoBase {
		public SPViewInfoInfo(SPView view)
			: base(view.SchemaXml) {
		}
		public bool IsGroupedField(SPField field) {
			return IsGroupedField(field.InternalName) || IsGroupedField(field.Title);
		}
		public int GetGroupIndex(SPField field) {
			int ret = GetGroupIndex(field.InternalName);
			if(ret == -1)
				ret = GetGroupIndex(field.Title);
			return ret;
		}
		public ColumnSortOrder GetGroupOrder(SPField field) {
			ColumnSortOrder ret = GetGroupOrder(field.InternalName);
			if(ret == ColumnSortOrder.None)
				ret = GetGroupOrder(field.Title);
			return ret;
		}
		public bool IsSortedField(SPField field) {
			return IsSortedField(field.InternalName) || IsSortedField(field.Title);
		}
		public int GetSortIndex(SPField field) {
			int ret = GetSortIndex(field.InternalName);
			if(ret == -1)
				ret = GetSortIndex(field.Title);
			return ret;
		}
		public ColumnSortOrder GetSortOrder(SPField field) {
			ColumnSortOrder ret = GetSortOrder(field.InternalName);
			if(ret == ColumnSortOrder.None)
				ret = GetSortOrder(field.Title);
			return ret;
		}
		public bool IsSummaryExist(SPField field) {
			return IsSummaryExist(field.InternalName) || IsSummaryExist(field.Title);
		}
		public SummaryItemType GetSummaryType(SPField field) {
			SummaryItemType ret = GetSummaryType(field.InternalName);
			if(ret == SummaryItemType.None)
				ret = GetSummaryType(field.Title);
			return ret;
		}
	}
	public static class UCharHelpre {
		public const string quote = "\\u0027";
		public const string doubleQuote = "\\u0022";
		public const string startBrace = "\\u00257B";
		public const string endBrace = "\\u00257D";
	}
	public static class SPxListViewUtils {
		public static bool IsAnnonymous(SPUser user) {
			return user == null || string.IsNullOrEmpty(user.LoginName);
		}
		public static string GetRibbonCreateCommand(SPList currentList) {
			string command = string.Empty;
			switch(currentList.BaseTemplate) {
				case SPListTemplateType.TasksWithTimelineAndHierarchy:
				case SPListTemplateType.Tasks:
				command = "SP.Ribbon.TasksWebPartPageComponent";
				break;
				case SPListTemplateType.DocumentLibrary:
				command = "SP.Ribbon.DocLibWebPartPageComponent";
				break;
				default:
				command = "SP.Ribbon.GenericListWebPartPageComponent";
				break;
			}
			return command;
		}
		public static string GetCustomButtonClickHandlerScript(SPxGridView grid) {
			return
				"function spxOnCustomButtonClick(s, e){\n" +
				"  e.processOnServer = true;\n" +
				"  if (e.buttonID == 'Delete'){\n" +
				"    e.processOnServer = false;\n" +
				"    s.DeleteGridRow(e.visibleIndex);\n" +
				"  }  \n" +
				"}\n";
		}
		public static GridViewDataColumn CreateSPxGridViewColumnInstance(SPField currentField) {
			GridViewDataColumn column = null;
			column = new GridViewDataColumn();
			column.UnboundType = UnboundColumnType.String;
			column.PropertiesEdit = new DevExpress.Web.TextBoxProperties();
			column.PropertiesEdit.EncodeHtml = false;
			column.Settings.SortMode = XtraGrid.ColumnSortMode.Value;
			if(IsFieldAllowMultipleValues(currentField))
				column.Settings.HeaderFilterMode = HeaderFilterMode.CheckedList;
			return column;
		}
		public static bool IsFieldAllowMultipleValues(SPField currentField) {
			bool allow = false;
			if(currentField is SPFieldLookup)
				if(((SPFieldLookup)currentField).AllowMultipleValues)
					allow = true;
			if((currentField is SPFieldMultiChoice) || (currentField is SPFieldMultiColumn))
				allow = true;
			return allow;
		}
		public static SPView GetSPListView(SPList list, HttpContext context) {
			if(list == null)
				return null;
			SPView curView = GetCurrentPageSPViewForList(list, context);
			if(curView == null)
				curView = list.DefaultView;
			if(curView == null && list.Views.Count > 0)
				curView = list.Views[0];
			return curView;
		}
		public static SPList GetCurrentPageList(HttpContext context) {
			SPList curList = null;
			try {
				curList = SPControl.GetContextWeb(context).GetListFromUrl(HttpContext.Current.Request.Url.AbsoluteUri);
			} catch { }
			return curList;
		}
		public static SPView GetCurrentPageSPViewForList(SPList list, HttpContext context) {
			if(list == null)
				return null;
			SPView curView = null;
			SPList curPageList = GetCurrentPageList(context);
			if(curPageList != null)
				curView = curPageList.ID == list.ID ? SPContext.Current.ViewContext.View : null;
			return curView;
		}
		public static List<KeyValuePair<int, string>> GetDistinctValuesForSPFieldMultiLookUpColumn(SPList currentList, SPField currentField) {
			Dictionary<int, string> uniqueValues = new Dictionary<int, string>();
			if(currentField is SPFieldLookup)
				if(((SPFieldLookup)currentField).AllowMultipleValues) {
					SPListItemCollection col = GetItemCollectionWithNonEmptyValuesByField(currentList, currentField);
					if(col != null)
						if(col.Count > 0)
							foreach(SPListItem tmpItem in col) {
								SPFieldLookupValueCollection lookUpCollection = null;
								if(tmpItem[currentField.Id] != null)
									lookUpCollection = new SPFieldLookupValueCollection(tmpItem[currentField.Id].ToString());
								if(lookUpCollection != null)
									foreach(SPFieldLookupValue lookUp in lookUpCollection)
										if(!uniqueValues.ContainsKey(lookUp.LookupId))
											uniqueValues.Add(lookUp.LookupId, lookUp.LookupValue);
							}
				}
			List<KeyValuePair<int, string>> sortedUniqueValues = (from kv in uniqueValues orderby kv.Value select kv).ToList();
			return sortedUniqueValues;
		}
		public static List<string> GetDistinctValuesForSPFieldMultiChoiceColumn(SPList currentList, SPField currentField) {
			List<string> uniqueValues = new List<string>();
			if(currentField is SPFieldMultiChoice) {
				SPListItemCollection col = GetItemCollectionWithNonEmptyValuesByField(currentList, currentField);
				if(col != null)
					if(col.Count > 0)
						foreach(SPListItem tmpItem in col) {
							SPFieldMultiChoiceValue choiceCollection = null;
							if(tmpItem[currentField.Id] != null)
								choiceCollection = new SPFieldMultiChoiceValue(tmpItem[currentField.Id].ToString());
							if(choiceCollection != null)
								for(int i = 0; i < choiceCollection.Count; i++)
									if(uniqueValues.IndexOf(choiceCollection[i]) < 0)
										uniqueValues.Add(choiceCollection[i]);
						}
			}
			uniqueValues.Sort();
			return uniqueValues;
		}
		public static DataTable GetDistinctValuesForSPFielLookUpColumn(SPList currentList, SPField currentField) {
			DataTable uniqueValues = null;
			if(currentField is SPFieldLookup) {
				SPListItemCollection col = GetItemCollectionWithNonEmptyValuesByField(currentList, currentField);
				if(col != null)
					if(col.Count > 0) {
						DataTable tb = SPFieldXmlExtensions.GetFullDataTable(col);
						DataView tbView = new DataView(tb);
						uniqueValues = tbView.ToTable(true, currentField.InternalName);
					}
			}
			uniqueValues.DefaultView.Sort = string.Format("{0} ASC", currentField.InternalName);
			return uniqueValues;
		}
		public static SPListItemCollection GetItemCollectionWithNonEmptyValuesByField(SPList currentList, SPField currentField) {
			SPListItemCollection coll = null;
			SPQuery qry = new SPQuery();
			qry.Folder = currentList.RootFolder;
			qry.Query = string.Format("<Where>" +
											"<IsNotNull>" +
												"<FieldRef Name='{0}'/>" +
											"</IsNotNull>" +
									  "</Where>" +
									  "<OrderBy>" +
											"<FieldRef Name='{0}'/>" +
									  "</OrderBy>", currentField.InternalName);
			qry.ViewAttributes = "Scope='RecursiveAll' ModerationType='Moderator'";
			coll = currentList.GetItems(qry);
			return coll;
		}
		public static bool DoesUserHavePermissionsToEditListItem(SPListItem currentItem) {
			return currentItem.DoesUserHavePermissions(SPBasePermissions.EditListItems);
		}
		public static bool DoesUserHavePermissionsToDeleteListItem(SPListItem currentItem) {
			return currentItem.DoesUserHavePermissions(SPBasePermissions.DeleteListItems);
		}
		public static bool DoesUserHavePermissionsToVioewVersionsListItem(SPListItem currentItem) {
			return currentItem.DoesUserHavePermissions(SPBasePermissions.ViewVersions);
		}
		public static string GetItemDisplayFormUrl(SPList currentList) {
			return currentList.Forms[PAGETYPE.PAGE_DISPLAYFORM].ServerRelativeUrl + "?ID=clientRowID";
		}
		public static string GetItemEditFormUrl(SPList currentList) {
			return currentList.Forms[PAGETYPE.PAGE_EDITFORM].ServerRelativeUrl + "?ID=clientRowID";
		}
		public static string GetItemViewVersionFormUrl(SPList currentList, string sourceUrl) {
			sourceUrl = HttpUtility.UrlEncode(sourceUrl);
			if(!string.IsNullOrEmpty(sourceUrl))
				return string.Format("/{0}/Versions.aspx?List={1}&ID=clientRowID&Source={2}", SPUtility.ContextLayoutsFolder, currentList.ID.ToString(), sourceUrl);
			return string.Format("/{0}/Versions.aspx?List={1}&ID=clientRowID", SPUtility.ContextLayoutsFolder, currentList.ID.ToString());
		}
		public static string GetItemAlertMeFormUrl(SPList currentList, string sourceUrl) {
			sourceUrl = HttpUtility.UrlEncode(sourceUrl);
			if(!string.IsNullOrEmpty(sourceUrl))
				return string.Format("/{0}/SubNew.aspx?List={1}&ID=clientRowID&Source={2}", SPUtility.ContextLayoutsFolder, currentList.ID.ToString(), sourceUrl);
			return string.Format("/{0}/SubNew.aspx?List={1}&ID=clientRowID", SPUtility.ContextLayoutsFolder, currentList.ID.ToString());
		}
		public static string GetItemWorkflowFormUrl(SPList currentList, string sourceUrl) {
			sourceUrl = HttpUtility.UrlEncode(sourceUrl);
			if(!string.IsNullOrEmpty(sourceUrl))
				return string.Format("/{0}/Workflow.aspx?List={1}&ID=clientRowID&Source={2}", SPUtility.ContextLayoutsFolder, currentList.ID.ToString(), sourceUrl);
			return string.Format("/{0}/Workflow.aspx?List={1}&ID=clientRowID", SPUtility.ContextLayoutsFolder, currentList.ID.ToString());
		}
	}
	public static partial class SPFieldXmlExtensions {
		static string sFromRowsetToRegularXmlXslt =
				"<xsl:stylesheet version=\"1.0\" " +
				 "xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" " +
				 "xmlns:s=\"uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882\" " +
				 "xmlns:z=\"#RowsetSchema\">" +
			 "<s:Schema id=\"RowsetSchema\"/>" +
			 "<xsl:output method=\"xml\" omit-xml-declaration=\"yes\" />" +
			 "<xsl:template match=\"/\">" +
			  "<xsl:text disable-output-escaping=\"yes\">&lt;rows&gt;</xsl:text>" +
			  "<xsl:apply-templates select=\"//z:row\"/>" +
			  "<xsl:text disable-output-escaping=\"yes\">&lt;/rows&gt;</xsl:text>" +
			 "</xsl:template>" +
			 "<xsl:template match=\"z:row\">" +
			  "<xsl:text disable-output-escaping=\"yes\">&lt;row&gt;</xsl:text>" +
			  "<xsl:apply-templates select=\"@*\"/>" +
			  "<xsl:text disable-output-escaping=\"yes\">&lt;/row&gt;</xsl:text>" +
			 "</xsl:template>" +
			 "<xsl:template match=\"@*\">" +
			  "<xsl:text disable-output-escaping=\"yes\">&lt;</xsl:text>" +
			  "<xsl:value-of select=\"substring-after(name(), 'ows_')\"/>" +
			  "<xsl:text disable-output-escaping=\"yes\">&gt;</xsl:text>" +
			  "<xsl:value-of select=\".\"/>" +
			  "<xsl:text disable-output-escaping=\"yes\">&lt;/</xsl:text>" +
			  "<xsl:value-of select=\"substring-after(name(), 'ows_')\"/>" +
			  "<xsl:text disable-output-escaping=\"yes\">&gt;</xsl:text>" +
			 "</xsl:template>" +
			"</xsl:stylesheet>";
		public static DataTable GetFullDataTable(SPListItemCollection itemCollection) {
			if(itemCollection != null) {
				DataSet ds = new DataSet();
				string xmlData = ConvertZRowToRegularXml(itemCollection.Xml);
				if(string.IsNullOrEmpty(xmlData))
					return null;
				using(System.IO.StringReader sr = new System.IO.StringReader(xmlData)) {
					ds.ReadXml(sr, XmlReadMode.Auto);
					if(ds.Tables.Count == 0)
						return null;
					return ds.Tables[0];
				}
			}
			return null;
		}
		private static string ConvertZRowToRegularXml(string zRowData) {
			zRowData = zRowData.Replace("Etag", "ows_Etag");
			XslCompiledTransform transform = new XslCompiledTransform();
			XmlDocument tidyXsl = new XmlDocument();
			try {
				tidyXsl.LoadXml(sFromRowsetToRegularXmlXslt);
				transform.Load(tidyXsl);
				using(System.IO.StringWriter sw = new System.IO.StringWriter()) {
					using(XmlTextWriter tw = new XmlTextWriter(sw)) {
						using(System.IO.StringReader srZRow = new System.IO.StringReader(zRowData)) {
							using(XmlTextReader xtrZRow = new XmlTextReader(srZRow)) {
								transform.Transform(xtrZRow, null, tw);
								return sw.ToString();
							}
						}
					}
				}
			} catch {
				return null;
			}
		}
	}
}
