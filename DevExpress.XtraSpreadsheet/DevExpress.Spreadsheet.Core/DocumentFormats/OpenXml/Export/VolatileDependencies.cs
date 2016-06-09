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
using System.Xml;
using DevExpress.Utils.Zip;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Translation tables
		internal static Dictionary<VolatileDependecyType, string> VolatileDependecyTypeTable = CreateVolatileDependecyTypeTable();
		internal static Dictionary<VolatileDependecyValueType, string> VolatileDependecyValueTypeTable = CreateVolatileDependecyValueTypeTable();
		static Dictionary<VolatileDependecyType, string> CreateVolatileDependecyTypeTable() {
			Dictionary<VolatileDependecyType, string> result = new Dictionary<VolatileDependecyType, string>();
			result.Add(VolatileDependecyType.OLAPFormulas, "olapFunctions");
			result.Add(VolatileDependecyType.RealTimeData, "realTimeData");
			return result;
		}
		static Dictionary<VolatileDependecyValueType, string> CreateVolatileDependecyValueTypeTable() {
			Dictionary<VolatileDependecyValueType, string> result = new Dictionary<VolatileDependecyValueType, string>();
			result.Add(VolatileDependecyValueType.Boolean, "b");
			result.Add(VolatileDependecyValueType.Error, "e");
			result.Add(VolatileDependecyValueType.RealNumber, "n");
			result.Add(VolatileDependecyValueType.String, "s");
			return result;
		}
		#endregion
		protected internal virtual bool ShouldExportVolatileDependencies() {
			return Workbook.RealTimeDataManager.Applications.Count > 0;
		}
		protected internal virtual void AddVolatileDependenciesPackageContent() {
			if (!ShouldExportVolatileDependencies())
				return;
			AddPackageContent(@"xl\volatileDependencies.xml", ExportVolatileDependencies());
		}
		protected internal virtual CompressedStream ExportVolatileDependencies() {
			return CreateXmlContent(GenerateVolatileDependenciesXmlContent);
		}
		void GenerateVolatileDependenciesXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateVolatileDependenciesContent();
		}
		void GenerateVolatileDependenciesContent() {
			WriteShStartElement("volTypes");
			try {
				GenerateRealTimeData();
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateRealTimeData() {
			WriteShStartElement("volType");
			try {
				WriteEnumValue<VolatileDependecyType>("type", VolatileDependecyType.RealTimeData, VolatileDependecyTypeTable);
				Dictionary<string, List<RealTimeDataApplication>> applications = new Dictionary<string, List<RealTimeDataApplication>>();
				foreach (RealTimeDataApplication application in Workbook.RealTimeDataManager.Applications.Values){
					List<RealTimeDataApplication> applicationGroup;
					if (!applications.TryGetValue(application.ApplicationId, out applicationGroup)){
						applicationGroup = new List<RealTimeDataApplication>();
						applications.Add(application.ApplicationId, applicationGroup);
					}
					applicationGroup.Add(application);
				}
				foreach (List<RealTimeDataApplication> applicationGroup in applications.Values)
					WriteApplicationGroup(applicationGroup);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteApplicationGroup(List<RealTimeDataApplication> applicationGroup) {
			if (applicationGroup.Count <= 0)
				return;
			WriteShStartElement("main");
			try {
				string appId = applicationGroup[0].ApplicationId;
				WriteStringValue("first", appId);
				foreach (RealTimeDataApplication application in applicationGroup)
					foreach (RealTimeTopic topic in application.Topics.Values)
						WriteTopic(topic, application.ServerName);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteTopic(RealTimeTopic topic, string serverName) {
			WriteShStartElement("tp");
			try {
				VolatileDependecyValueType valueType = GetVolatileDependencyValueType(topic.CachedValue);
				WriteEnumValue<VolatileDependecyValueType>("t", valueType, VolatileDependecyValueTypeTable);
				VariantValue value = topic.CachedValue;
				if (value.IsSharedString)
					value = value.GetTextValue(Workbook.SharedStringTable);
				ExportCellValue(value, true);
				WriteString("stp", null, serverName);
				foreach (string parameter in topic.Parameters)
					WriteString("stp", null, parameter);
				foreach (ICell referencedCell in topic.ReferencedCells)
					WriteCellReference(referencedCell);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteCellReference(ICell cell) {
			WriteShStartElement("tr");
			try {
				WriteCellPosition("r", cell.Position);
				WriteIntValue("s", cell.Worksheet.SheetId);
			}
			finally {
				WriteShEndElement();
			}
		}
		VolatileDependecyValueType GetVolatileDependencyValueType(VariantValue value) {
			if (value.IsError)
				return VolatileDependecyValueType.Error;
			if (value.IsBoolean)
				return VolatileDependecyValueType.Boolean;
			if (value.IsNumeric)
				return VolatileDependecyValueType.RealNumber;
			return VolatileDependecyValueType.String;
		}
	}
}
