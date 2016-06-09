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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Sql {
	public class DBSchema {
		internal static DBSchema MergeDbSchema(DBSchema x, DBSchema y) {
			if(x == null)
				return y;
			if(y == null)
				return x;
			IEnumerable<DBStoredProcedure> dbStoredProcedures = x.StoredProcedures ?? Enumerable.Empty<DBStoredProcedure>();
			return new DBSchema(x.Tables.Union(y.Tables).OrderBy(table => table.Name).ToArray(),
				x.Views.Union(y.Views).OrderBy(view => view.Name).ToArray(),
				dbStoredProcedures.Union(y.StoredProcedures ?? Enumerable.Empty<DBStoredProcedure>()).OrderBy(procedure => procedure.Name).ToArray());
		}
		const string xmlDBSchema = "DBSchema";
		const string xmlTables = "Tables";
		const string xmlViews = "Views";
		const string xmlProcedures = "Procedures";
		internal const string xmlProcedure = "DBStoredProcedure";
		DBTable[] tables = new DBTable[0];
		DBTable[] views = new DBTable[0];
		DBStoredProcedure[] storedProcedures = new DBStoredProcedure[0];
		public DBSchema(DBTable[] tables, DBTable[] views, DBStoredProcedure[] storedProcedures) {
			this.storedProcedures = storedProcedures;
			this.tables = tables;
			this.views = views;
		}
		public DBSchema(DBTable[] tables, DBTable[] views)
			: this(tables, views, new DBStoredProcedure[0]) {
		}
		public DBSchema() {
		}
		public DBTable[] Tables { get { return this.tables; } }
		public DBTable[] Views { get { return this.views; } }
		public DBStoredProcedure[] StoredProcedures { get { return this.storedProcedures; } }
		public void SaveToXml(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			XElement element = new XElement(xmlDBSchema);
			XElement elementTables = new XElement(xmlTables);
			SaveToXml(elementTables, this.tables);
			element.Add(elementTables);
			XElement elementViews = new XElement(xmlViews);
			SaveToXml(elementViews, this.views);
			element.Add(elementViews);
			XElement elementProcedures = new XElement(xmlProcedures);
			SaveToXml(elementProcedures, this.storedProcedures);
			element.Add(elementProcedures);
#if DXPORTABLE
			XmlWriterSettings settings = new XmlWriterSettings();
			var writer = XmlWriter.Create(stream, settings);
#else
			XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8) { Formatting = Formatting.Indented };
#endif
			try {
				new XDocument(element).WriteTo(writer);
			}
			finally {
				writer.Flush();
			}
		}
		public void LoadFromXml(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
#if DXPORTABLE
			XmlReaderSettings settings = new XmlReaderSettings();
			var reader = XmlReader.Create(stream, settings);
#else
			XmlTextReader reader = new XmlTextReader(stream);
#endif
			XDocument document = XDocument.Load(reader);
			if(document == null)
				throw new XmlException();
			XElement rootElement = document.Root;
			if(rootElement == null || rootElement.Name != xmlDBSchema)
				throw new XmlException();
			XElement elementTables = rootElement.Element(xmlTables);
			if(elementTables != null)
				this.tables = GetTables(elementTables);
			else
				this.tables = new DBTable[0];
			XElement elementViews = rootElement.Element(xmlViews);
			if(elementViews != null)
				this.views = GetTables(elementViews);
			else
				this.views = new DBTable[0];
			XElement elementProcedures = rootElement.Element(xmlProcedures);
			if(elementProcedures != null)
				this.storedProcedures = GetProcedures(elementProcedures);
			else
				this.storedProcedures = new DBStoredProcedure[0];
		}
		static XmlAttributeOverrides GetOverrides() {
			XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
			return attrOverrides;
		}
		static DBTable[] GetTables(XElement element) {
			List<DBTable> listTables = new List<DBTable>();
			foreach(var tableElement in element.Elements()) {
				if(tableElement.Name == "DBTable" || tableElement.Name == "DBTableWithAlias")
					listTables.Add((DBTable)new XmlSerializer(typeof(DBTable), GetOverrides()).Deserialize(tableElement.CreateReader()));
			}
			return listTables.ToArray();
		}
		static DBStoredProcedure[] GetProcedures(XElement element) {
			List<DBStoredProcedure> listTables = new List<DBStoredProcedure>();
			foreach(var storedProcElement in element.Elements()) {
				if(storedProcElement.Name == xmlProcedure)
					listTables.Add(GetProcedure(storedProcElement));
			}
			return listTables.ToArray();
		}
		internal static DBStoredProcedure GetProcedure(XElement storedProcElement) {
			return (DBStoredProcedure)new XmlSerializer(typeof(DBStoredProcedure), GetOverrides()).Deserialize(storedProcElement.CreateReader());
		}
		static void SaveToXml(XElement element, IEnumerable<DBTable> tables) {
			foreach(DBTable table in tables) {
				XmlSerializer xmlSerializer = new XmlSerializer(table.GetType(), GetOverrides());
				using(var stream = new MemoryStream()) {
					xmlSerializer.Serialize(stream, table);
					stream.Position = 0;
					using(XmlReader reader = XmlReader.Create(stream)) {
						XElement e = XElement.Load(reader);
						element.Add(e);
					}
				}
			}
		}
		static void SaveToXml(XElement element, IEnumerable<DBStoredProcedure> procedures) {
			foreach(DBStoredProcedure procedure in procedures) {
				SaveToXml(element, procedure);
			}
		}
		internal static void SaveToXml(XElement element, DBStoredProcedure procedure) {
			XmlSerializer xmlSerializer = new XmlSerializer(procedure.GetType(), GetOverrides());
			using(var stream = new MemoryStream()) {
				xmlSerializer.Serialize(stream, procedure);
				stream.Position = 0;
				using(XmlReader reader = XmlReader.Create(stream)) {
					XElement storedProcElement = XElement.Load(reader);
					element.Add(storedProcElement);
				}
			}
		}
	}
}
