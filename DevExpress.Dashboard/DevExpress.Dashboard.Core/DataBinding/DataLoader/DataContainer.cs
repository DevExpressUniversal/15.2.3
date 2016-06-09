#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.Utils;
namespace DevExpress.DataAccess {
	public class DataContainer<T, C>  where T : class, IDataSource, new() where C : DataConnectionBase, new()  { 
		const string xmlDataContainer = "DataContainer";
		const string xmlDataConnections = "DataConnections";
		const string xmlDataSources = "DataSources";
		const string xmlDataSource = "DataSource";
		public IDataSourceCollection DataSources { get; private set; }
		public IEnumerable<DataConnectionBase> DataConnections {
			get {
				List<DataConnectionBase> connections = new List<DataConnectionBase>();
				foreach (IDataSource dataSource in DataSources) {
					IDataProvider dataProvider = dataSource.DataProvider;
					if (dataProvider != null) {
						DataConnectionBase dataConnection = dataProvider.Connection;
						if (!connections.Contains(dataConnection))
							connections.Add(dataConnection);
					}
				}
				return connections;
			}
		}
		public DataContainer() {
			this.DataSources = new IDataSourceCollection();
		}
		public void SaveToXml(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			XElement element = new XElement(xmlDataContainer);
			SaveToXml(element);
			XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8) { Formatting = Formatting.Indented };
			try {
				new XDocument(element).WriteTo(writer);
			} finally {
				writer.Flush();
			}
		}
		public void SaveToXml(XElement element) {
			if (DataSources.Count > 0) {
				XElement dataConnectionsElement = null;
				foreach (DataConnectionBase dataConnection in DataConnections) {
					if (dataConnectionsElement == null)
						dataConnectionsElement = new XElement(xmlDataConnections);
					dataConnectionsElement.Add(dataConnection.SaveToXml());
				}
				if (dataConnectionsElement != null)
					element.Add(dataConnectionsElement);
				XElement dataSourcesElement = new XElement(xmlDataSources);
				foreach (IDataSource dataSource in DataSources)
					dataSourcesElement.Add(dataSource.SaveToXml());
				element.Add(dataSourcesElement);
			}
		}
		public void LoadFromXml(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			XmlTextReader reader = new XmlTextReader(stream);
			XDocument document = XDocument.Load(reader);
			if (document == null)
				throw new XmlException();
			XElement rootElement = document.Root;
			if (rootElement == null || rootElement.Name != xmlDataContainer)
				throw new XmlException();
			LoadFromXml(rootElement);
		}
		public void LoadFromXml(XElement element) {
			XElement dataSourcesElement = element.Element(xmlDataSources);
			if (dataSourcesElement != null) {
				List<DataConnectionBase> connections = new List<DataConnectionBase>();
				XElement dataConnectionsElement = element.Element(xmlDataConnections);
				if (dataConnectionsElement != null)
					foreach(XElement connectionElement in dataConnectionsElement.Elements(DataConnectionHelper.XmlDataConnection)) {
						C connection = new C();
						connection.LoadFromXml(connectionElement);
						connections.Add(connection);
					}
				DataSources.BeginUpdate();
				try {
					foreach (XElement dataSourceElement in dataSourcesElement.Elements(xmlDataSource)) {
						T dataSource = new T();
						dataSource.LoadFromXml(dataSourceElement);
						dataSource.SetConnection(connections);
						DataSources.Add(dataSource);
					}
				} finally {
					DataSources.EndUpdate();
				}
			}
		}		
	}
}
