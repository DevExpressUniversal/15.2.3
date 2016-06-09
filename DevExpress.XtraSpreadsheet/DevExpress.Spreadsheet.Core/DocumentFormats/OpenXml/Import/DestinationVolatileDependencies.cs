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
using System.Globalization;
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DestinationVolatileDependencies
	public class VolatileDependenciesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("volType", OnVolatileDependecyType);
			return result;
		}
		#endregion
		public VolatileDependenciesDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static Destination OnVolatileDependecyType(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new VolatileDependecyTypeDestination(importer);
		}
	}
	#endregion
	#region VolatileDependecyTypeDestination
	public class VolatileDependecyTypeDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static Dictionary<string, VolatileDependecyType> volatileDependecyTypeTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.VolatileDependecyTypeTable);
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("main", OnVolatileDependecyServer);
			return result;
		}
		VolatileDependecyType dependencyType;
		public VolatileDependecyTypeDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			dependencyType = Importer.GetWpEnumValue<VolatileDependecyType>(reader, "type", volatileDependecyTypeTable, VolatileDependecyType.OLAPFormulas);
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static VolatileDependecyTypeDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (VolatileDependecyTypeDestination)importer.PeekDestination();
		}
		static Destination OnVolatileDependecyServer(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VolatileDependecyTypeDestination thisDestination = GetThis(importer);
			if (thisDestination.dependencyType == VolatileDependecyType.RealTimeData)
				return new VolatileDependecyServerDestination(importer);
			else
				return new EmptyDestination<SpreadsheetMLBaseImporter>(importer); 
		}
	}
	#endregion
	#region VolatileDependecyServerDestination
	public class VolatileDependecyServerDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("tp", OnVolatileDependecyTopic);
			return result;
		}
		readonly List<OpenXmlTopicDefinition> topicDefinitions;
		string applicationId;
		public VolatileDependecyServerDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			topicDefinitions = new List<OpenXmlTopicDefinition>();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			applicationId = Importer.ReadAttribute(reader, "first");
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (topicDefinitions.Count <= 0)
				return;
			DocumentModel documentModel = Importer.DocumentModel;
			RealTimeDataApplication application = null;
			foreach (OpenXmlTopicDefinition topicDefinition in topicDefinitions) {
				topicDefinition.PrepareServerName();
				if (application == null || string.Compare(application.ServerName, topicDefinition.ServerName) != 0)
					application = Importer.DocumentModel.RealTimeDataManager.PrepareApplication(applicationId, topicDefinition.ServerName, false);
				topicDefinition.PrepareTopic(application, documentModel.DataContext);
			}
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static VolatileDependecyServerDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (VolatileDependecyServerDestination)importer.PeekDestination();
		}
		static Destination OnVolatileDependecyTopic(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VolatileDependecyServerDestination thisDestination = GetThis(importer);
			OpenXmlTopicDefinition topicDefinition = new OpenXmlTopicDefinition();
			thisDestination.topicDefinitions.Add(topicDefinition);
			return new VolatileDependecyTopicDestination(importer, topicDefinition);
		}
	}
	#endregion
	#region VolatileDependecyTopicDestination
	public class VolatileDependecyTopicDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static Dictionary<string, VolatileDependecyValueType> volatileDependecyValueTypeTable = DictionaryUtils.CreateBackTranslationTable(DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.VolatileDependecyValueTypeTable);
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("stp", OnVolatileDependecyStrings);
			result.Add("tr", OnVolatileDependecyReferences);
			result.Add("v", OnVolatileDependecyCellValue);
			return result;
		}
		OpenXmlTopicDefinition topicDefinition;
		public VolatileDependecyTopicDestination(SpreadsheetMLBaseImporter importer, OpenXmlTopicDefinition topicDefinition)
			: base(importer) {
			this.topicDefinition = topicDefinition;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			topicDefinition.DependencyValueType = Importer.GetWpEnumValue<VolatileDependecyValueType>(reader, "t", volatileDependecyValueTypeTable, VolatileDependecyValueType.RealNumber);
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static VolatileDependecyTopicDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (VolatileDependecyTopicDestination)importer.PeekDestination();
		}
		static Destination OnVolatileDependecyStrings(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VolatileDependecyTopicDestination thisDestination = GetThis(importer);
			return new StringValueTagDestination(importer, delegate(string value) { thisDestination.topicDefinition.AddArgument(value); return true; });
		}
		static Destination OnVolatileDependecyReferences(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VolatileDependecyTopicDestination thisDestination = GetThis(importer);
			return new VolatileDependecyReferenceDestination(importer, thisDestination.topicDefinition);
		}
		static Destination OnVolatileDependecyCellValue(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			VolatileDependecyTopicDestination thisDestination = GetThis(importer);
			return new StringValueTagDestination(importer, delegate(string value) { thisDestination.topicDefinition.CellValue = value; return true; });
		}
	}
	#endregion
	#region VolatileDependecyReferenceDestination
	public class VolatileDependecyReferenceDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		OpenXmlTopicDefinition topicDefinition;
		public VolatileDependecyReferenceDestination(SpreadsheetMLBaseImporter importer, OpenXmlTopicDefinition topicDefinition)
			: base(importer) {
			this.topicDefinition = topicDefinition;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			string reference = Importer.ReadAttribute(reader, "r");
			CellPosition position = CellReferenceParser.Parse(reference);
			if (!position.IsValid)
				Importer.ThrowInvalidFile();
			int sheetId = Importer.GetWpSTIntegerValue(reader, "s");
			Worksheet targetSheet;
			if(!((OpenXmlImporter)Importer).SheetIdsTable.TryGetValue(sheetId, out targetSheet))
				Importer.ThrowInvalidFile();
			topicDefinition.ReferencedCells.Add(targetSheet[position]);
		}
	}
	#endregion
	#region OpenXmlTopicDefinition
	public class OpenXmlTopicDefinition {
		readonly List<ICell> referencedCells;
		readonly List<string> arguments;
		string serverName;
		string cellValue;
		public OpenXmlTopicDefinition() {
			arguments = new List<string>();
			referencedCells = new List<ICell>();
		}
		public string ServerName { get { return serverName; } set { serverName = value; } }
		public string CellValue { get { return cellValue; } set { cellValue = value; } }
		public List<ICell> ReferencedCells { get { return referencedCells; } }
		public VolatileDependecyValueType DependencyValueType { get; set; }
		internal List<string> Arguments { get { return arguments; } }
		public void AddArgument(string argument) {
			arguments.Add(argument);
		}
		public void PrepareServerName() {
			if (arguments.Count < 2)
				throw new ArgumentException();
			serverName = arguments[0];
		}
		public void PrepareTopic(RealTimeDataApplication application, WorkbookDataContext context) {
			string[] parameters = new string[arguments.Count - 1];
			arguments.CopyTo(1, parameters, 0, parameters.Length);
			RealTimeTopic topic = application.AddTopic(parameters);
			topic.ReferencedCells.AddRange(referencedCells);
			topic.CachedValue = PrepareCachedValue(context);
		}
		internal VariantValue PrepareCachedValue(WorkbookDataContext context) {
			switch (DependencyValueType) {
				case VolatileDependecyValueType.RealNumber:
					double doubleValue;
					if (Double.TryParse(CellValue, NumberStyles.Float, context.Culture, out doubleValue))
						return doubleValue;
					break;
				case VolatileDependecyValueType.Error:
					ICellError error;
					if (CellErrorFactory.TryCreateErrorByInvariantName(CellValue, out error))
						return error.Value;
					break;
				case VolatileDependecyValueType.Boolean:
					bool result;
					if (context.TryParseBoolean(cellValue, out result))
						return result;
					break;
				default:
					return cellValue;
			}
			return VariantValue.ErrorValueNotAvailable;
		}
	}
	#endregion
}
