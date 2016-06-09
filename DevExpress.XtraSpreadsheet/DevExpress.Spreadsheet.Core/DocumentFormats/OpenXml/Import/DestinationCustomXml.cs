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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using DevExpress.DataAccess.Native;
using DevExpress.Office;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region CustomXmlDestination
	internal class CustomXmlDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		public CustomXmlDestination(SpreadsheetMLBaseImporter importer) : base(importer) {}
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("MailMerge", MailMergeCustomXmlDestination);
			return result;
		}
		#endregion
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination MailMergeCustomXmlDestination(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new MailMergeCustomXmlDestination(importer);
		}
		#endregion
	}
	#endregion
	#region MailMergeCustomXmlDestination
	internal class MailMergeCustomXmlDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		int defaultIndex;
		#endregion
		public MailMergeCustomXmlDestination(SpreadsheetMLBaseImporter importer) : base(importer) {}
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("DataSources", OnDataSources);
			result.Add("Parameters", OnParameters);
			return result;
		}
		#endregion
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnDataSources(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DataSourcesDestination(importer);
		}
		static Destination OnParameters(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ParametersDestination(importer);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			defaultIndex = Importer.GetIntegerValue(reader, "Default", -1);
		}
		public override void ProcessElementClose(XmlReader reader) {
			DocumentModel documentModel = Importer.DocumentModel;
			if(defaultIndex == -1 || defaultIndex >= documentModel.DataComponentInfos.Count)
				return;
			documentModel.DataComponentInfos.SetAsDataSource(defaultIndex);
		}
	}
	#endregion
	#region DataSourcesDestination
	internal class DataSourcesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			return result;
		}
		#endregion
		public DataSourcesDestination(SpreadsheetMLBaseImporter importer) : base(importer) {}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		protected override Destination ProcessCurrentElement(XmlReader reader) {
			DataComponentInfo info = new DataComponentInfo();
			XmlReader innerReader = reader.ReadSubtree();
			innerReader.ReadToFollowing(reader.LocalName);
			string xmlContent = innerReader.ReadOuterXml();
			info.XmlContent = XElement.Parse(xmlContent);
			Importer.DocumentModel.DataComponentInfos.Add(info);
			return null;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string newDataMember = Importer.ReadAttribute(reader, "DataMember");
			if(!String.IsNullOrEmpty(newDataMember))
				Importer.DocumentModel.MailMergeDataMember = newDataMember;
		}
	}
	#endregion
	#region ParametersDestination
	internal class ParametersDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("Parameter", OnParameter);
			return result;
		}
		#endregion
		public ParametersDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnParameter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ParameterDestination(importer);
		}
		#endregion
	}
	internal class ParameterDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		public ParameterDestination(SpreadsheetMLBaseImporter importer) : base(importer) {}
		public override void ProcessElementOpen(XmlReader reader) {
			string name = Importer.GetWpSTXString(reader, "Name");
			string type = Importer.GetWpSTXString(reader, "Type");
			string value = Importer.GetWpSTXString(reader, "Value");
			if(name == null || type == null || value == null)
				return;
			SpreadsheetParameter parameter = new SpreadsheetParameter();
			Type realType = Type.GetType(type, false);
			if(realType == null)
				return;
			object realValue = ParameterHelper.ConvertFrom(value, realType, null);
			parameter.Name = name;
			parameter.Type = realType;
			parameter.Value = realValue;
			Importer.DocumentModel.MailMergeParameters.Add(parameter);
		}
	}
	#endregion
}
