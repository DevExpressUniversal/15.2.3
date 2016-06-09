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
using System.Diagnostics;
using System.Xml.Linq;
using DevExpress.DataAccess;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Forms {
	public class ManageDataSourceViewModelBase : ViewModelBase {
		#region Fields
		readonly List<string> dataSources;
		readonly ISpreadsheetControl spreadsheetControl;
		#endregion
		#region Properties
		public ISpreadsheetControl SpreadsheetControl { get { return spreadsheetControl; } }
		public DocumentModel DocumentModel { get { return SpreadsheetControl.Document.Model.DocumentModel; } }
		public List<string> DataSources { get { return dataSources; } }
		#endregion
		public ManageDataSourceViewModelBase(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "spreadsheetControl");
			this.spreadsheetControl = control;
			dataSources = new List<string>();
			PopulateDataSourcesTable();
		}
		void PopulateDataSourcesTable() {
			foreach(DataComponentInfo dataComponent in DocumentModel.DataComponentInfos) {
				string result = PopulateDataSource(dataComponent);
				if(!String.IsNullOrEmpty(result))
					DataSources.Add(result);
			}
		}
		protected string PopulateDataSource(DataComponentInfo dataComponent) {
			XElement xmlContent = dataComponent.XmlContent;
			XAttribute typeAttribute = xmlContent.Attribute(XName.Get("TypeName"));
			if(typeAttribute == null)
				return String.Empty;
			if(typeAttribute.Value == "DevExpress.DataAccess.Sql.SqlDataSource") {
				return PopulateSqlDataSource(xmlContent);
			}
			if(typeAttribute.Value == "DevExpress.DataAccess.EntityFramework.EFDataSource") {
				return PopulateEFDataSource(xmlContent);
			}
			if(typeAttribute.Value == "DevExpress.DataAccess.ObjectBinding.ObjectDataSource") {
				return PopulateObjectDataSource(xmlContent);
			}
			IDataComponent unknownDataComponent = dataComponent.TryToLoadDataComponentFromXml();
			return unknownDataComponent == null ? typeAttribute.Value : unknownDataComponent.Name;
		}
		string PopulateObjectDataSource(XElement xmlContent) {
			XElement objectDataSourceElement = xmlContent.Element(XName.Get("ObjectDataSource"));
			Debug.Assert(objectDataSourceElement != null, "objectDataSourceElement != null");
			return PopulateObjectDataSourceCore(objectDataSourceElement);
		}
		string PopulateObjectDataSourceCore(XElement xmlContent) {
			XElement dataSourceName = xmlContent.Element(XName.Get("Name"));
			if(dataSourceName != null && !String.IsNullOrEmpty(dataSourceName.Value)) {
				return dataSourceName.Value;
			}
			XElement innerDataType = xmlContent.Element(XName.Get("DataSource"));
			Debug.Assert(innerDataType != null, "innerDataType!=null");
			XAttribute nameAttribute = innerDataType.Attribute(XName.Get("Type"));
			Debug.Assert(nameAttribute != null, "nameAttribute != null");
			return nameAttribute.Value.Split(',')[0];
		}
		string PopulateEFDataSource(XElement xmlContent) {
			XElement efDataSourceElement = xmlContent.Element(XName.Get("EFDataSource"));
			Debug.Assert(efDataSourceElement != null, "efDataSourceElement != null");
			return PopulateEFDataSourceCore(efDataSourceElement);
		}
		string PopulateEFDataSourceCore(XElement xmlContent) {
			XAttribute dataSourceNameAttribute = xmlContent.Attribute(XName.Get("Name"));
			if(dataSourceNameAttribute != null && !string.IsNullOrEmpty(dataSourceNameAttribute.Value)) {
				return dataSourceNameAttribute.Value;
			}
			XAttribute nameAttribute = xmlContent.Attribute(XName.Get("ConnectionStringName"));
			if(nameAttribute != null && !string.IsNullOrEmpty(nameAttribute.Value)) {
				string name = nameAttribute.Value;
				return name;
			}
			return "EntityFrameworkDataSource";
		}
		string PopulateSqlDataSource(XElement xmlContent) {
			XElement sqlDataSourceElement = xmlContent.Element(XName.Get("SqlDataSource"));
			return sqlDataSourceElement != null ? PopulateSqlDataSourceCore(sqlDataSourceElement) : String.Empty;
		}		
		string PopulateSqlDataSourceCore(XElement xmlContent) {
			XElement dataSourceNameElement = xmlContent.Element(XName.Get("Name"));
			if(dataSourceNameElement != null && !string.IsNullOrEmpty(dataSourceNameElement.Value)) {
				return dataSourceNameElement.Value;
			}
			XElement connection = xmlContent.Element(XName.Get("Connection"));
			Debug.Assert(connection != null, "connection != null");
			XAttribute nameAttribute = connection.Attribute(XName.Get("Name"));
			Debug.Assert(nameAttribute != null, "nameAttribute != null");
			string name = nameAttribute.Value;
			name += GetQueriesName(xmlContent);
			return name;
		}
		static string GetQueriesName(XElement xmlContent) {
			List<string> queriesNames = new List<string>();
			IEnumerable<XElement> queries = xmlContent.Elements(XName.Get("Query"));
			foreach(XElement query in queries) {
				XAttribute queryName = query.Attribute(XName.Get("Name"));
				if(queryName == null)
					continue;
				queriesNames.Add(queryName.Value);
			}
			string result = "[" + String.Join(",", queriesNames) + "]";
			return result;
		}
	}
	public class SelectDataSourceViewModel : ManageDataSourceViewModelBase {
		public SelectDataSourceViewModel(ISpreadsheetControl control) : base(control) {
			SelectedItemIndex = DocumentModel.DataComponentInfos.DefaultIndex;
		}
		#region Properties
		public bool CanSelectApplicationDataSource { get { return DocumentModel.DataComponentInfos.NonIDataComponentDataSource != null && DocumentModel.MailMergeDataSource is IDataComponent; } }
		public int SelectedItemIndex { get; set; }
		#endregion
		public void ApplyChanges() {
			MailMergeSelectDataSourceCommand command = new MailMergeSelectDataSourceCommand(SpreadsheetControl);
			command.ApplyChanges(this);
		}
		public void SelectApplicationDataSource() {
			MailMergeSelectDataSourceCommand command = new MailMergeSelectDataSourceCommand(SpreadsheetControl);
			command.SelectApplicationDataSource();
		}
	}
}
