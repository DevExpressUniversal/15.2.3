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
using System.Globalization;
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.Snap.Core.API;
using System.Collections.Generic;
namespace DevExpress.Snap.Core.Import {
	#region DataSettingsDestination
	public class DataSettingsDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("calcFields", OnCalculatedFields);
			result.Add("params", OnParameters);
			return result;
		}
		public DataSettingsDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnCalculatedFields(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CalculatedFieldsDestination(importer);
		}
		static Destination OnParameters(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ParametersDestination(importer);
		}
	}
	#endregion
	#region CalculatedFieldsDestination
	public class CalculatedFieldsDestination : ElementDestination {
		readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("calculatedField", OnCalculatedFiled);
			return result;
		}
		public CalculatedFieldsDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnCalculatedFiled(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CalculatedFieldDestination((SnapImporter)importer);
		}
	}
	#endregion
	#region CalculatedFieldDestination
	public class CalculatedFieldDestination : SnapLeafElementDestination {
		public CalculatedFieldDestination(SnapImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string name = Importer.ReadDxStringAttr("name", reader);
			string dataMember = Importer.ReadDxStringAttr("dataMember", reader);
			CalculatedField calculatedField = new CalculatedField(name, dataMember);
			calculatedField.Expression = Importer.ReadDxStringAttr("expression", reader);
			calculatedField.DataSourceDispatcher = Importer.DocumentModel.DataSourceDispatcher;
			string fieldType = Importer.ReadDxStringAttr("fieldType", reader);
			if (fieldType != null)
				calculatedField.FieldType = (DevExpress.XtraReports.UI.FieldType)Enum.Parse(typeof(DevExpress.XtraReports.UI.FieldType), fieldType);
			string dataSourceName = Importer.ReadDxStringAttr("dataSource", reader);
			calculatedField.DataSourceName = dataSourceName;
			Importer.CalculatedFields.Add(calculatedField);
		}
	}
	#endregion
	#region ParametersDestination
	public class ParametersDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("parameter", OnParameter);
			return result;
		}
		public ParametersDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnParameter(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ParameterDestination((SnapImporter)importer);
		}
	}
	#endregion
	#region ParameterDestination
	public class ParameterDestination : SnapLeafElementDestination {
		public ParameterDestination(SnapImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string name = Importer.ReadDxStringAttr("name", reader);
			Parameter parameter = new Parameter();
			parameter.Name = name;
			string type = Importer.ReadDxStringAttr("type", reader);
			if (!String.IsNullOrEmpty(type))
				parameter.Type = Type.GetType(type);
			string value = Importer.ReadDxStringAttr("value", reader);
			if (!String.IsNullOrEmpty(value))
				parameter.Value = TypeDescriptor.GetConverter(parameter.Type).ConvertFromString(null, CultureInfo.InvariantCulture, value);
			Importer.DocumentModel.AddParameterWithReplace(parameter);
		}
	}
	#endregion
}
