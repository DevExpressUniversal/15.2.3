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
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region MailMergeDestination
	public class MailMergeDestination : DestinationBase {
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("mmconnectstr", OnConnectionStringKeyword);
			table.Add("mmquery", OnQueryKeyword);
			table.Add("mmdatasource", OnDataSourceKeyword);
			table.Add("mmodsofldmpdata", OnFieldMapDataKeyword);
			return table;
		}
		#endregion
		#region Keywords handlers
		static void OnConnectionStringKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			MailMergeProperties properties = importer.DocumentModel.MailMergeProperties;
			importer.Destination = new StringPropertyBaseDestination(importer, delegate(string value) {
				properties.ConnectionString = value;
			});
		}
		static void OnQueryKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			MailMergeProperties properties = importer.DocumentModel.MailMergeProperties;
			importer.Destination = new StringPropertyBaseDestination(importer, delegate(string value) {
				properties.Query = value;
			});
		}
		static void OnDataSourceKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			MailMergeProperties properties = importer.DocumentModel.MailMergeProperties;
			importer.Destination = new StringPropertyBaseDestination(importer, delegate(string value) {
				properties.DataSource = value;
			});
		}
		static void OnFieldMapDataKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new FieldMapDataDestination(importer);
		}
		#endregion
		public MailMergeDestination(RtfImporter importer)
			: base(importer) {
		}
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected override DestinationBase CreateClone() {
			return new MailMergeDestination(Importer);
		}
	}
	#endregion
	#region FieldMapDataDestination
	public class FieldMapDataDestination : DestinationBase {
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		#region CreateKeywordTable
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("mmfttypenull", OnNullFieldTypeKeyword);
			table.Add("mmfttypedbcolumn", OnColumnFieldTypeKeyword);
			table.Add("mmfttypeaddress", OnAddressFieldTypeKeyword);
			table.Add("mmfttypesalutation", OnSalutationFieldTypeKeyword);
			table.Add("mmfttypemapped", OnMappedFieldTypeKeyword);
			table.Add("mmfttypebarcode", OnBarcodeFieldTypeKeyword);
			table.Add("mmodsoname", OnColumnNameKeyword);
			table.Add("mmodsomappedname", OnMappedNameKeyword);
			table.Add("mmodsofmcolumn", OnColumnIndexKeyword);
			table.Add("mmodsodynaddr", OnDynamicAddressKeyword);
			table.Add("mmodsolid", OnLanguageIdKeyword);
			return table;
		}
		#endregion
		#region Keywords handlers
		static FieldMapData GetFieldMapDataForEdit(RtfImporter importer) {
			NotificationCollection<FieldMapData> fieldsMapData = importer.DocumentModel.MailMergeProperties.DataSourceObjectProperties.FieldsMapData;
			if (fieldsMapData.Count == 0)
				fieldsMapData.Add(new FieldMapData());
			return fieldsMapData[fieldsMapData.Count - 1];
		}
		static void OnNullFieldTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			FieldMapData fieldMapData = GetFieldMapDataForEdit(importer);
			fieldMapData.FieldType = MailMergeFieldType.Null;
		}
		static void OnColumnFieldTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			FieldMapData fieldMapData = GetFieldMapDataForEdit(importer);
			fieldMapData.FieldType = MailMergeFieldType.DbColumn;
		}
		static void OnAddressFieldTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnSalutationFieldTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnMappedFieldTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnBarcodeFieldTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnColumnNameKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			FieldMapData fieldMapData = GetFieldMapDataForEdit(importer);
			importer.Destination = new StringPropertyBaseDestination(importer, delegate(string value) {
				fieldMapData.ColumnName = value;
			});
		}
		static void OnMappedNameKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			FieldMapData fieldMapData = GetFieldMapDataForEdit(importer);
			importer.Destination = new StringPropertyBaseDestination(importer, delegate(string value) {
				fieldMapData.MappedName = value;
			});
		}
		static void OnColumnIndexKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			FieldMapData fieldMapData = GetFieldMapDataForEdit(importer);
			fieldMapData.ColumnIndex = hasParameter ? parameterValue : -1;
		}
		static void OnDynamicAddressKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 1;
			FieldMapData fieldMapData = GetFieldMapDataForEdit(importer);
			if (parameterValue == 0)
				fieldMapData.DynamicAddress = false;
			else
				fieldMapData.DynamicAddress = true;
		}
		static void OnLanguageIdKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue < 0)
				parameterValue = 0;
			FieldMapData fieldMapData = GetFieldMapDataForEdit(importer);
			fieldMapData.MergeFieldNameLanguageId = parameterValue;
		}
		#endregion
		public FieldMapDataDestination(RtfImporter importer)
			: base(importer) {
		}
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected override DestinationBase CreateClone() {
			return new FieldMapDataDestination(Importer);
		}
	}
	#endregion
	public delegate void PropertyModifier(string value);
	#region StringPropertyBaseDestination
	public class StringPropertyBaseDestination : DestinationBase {
		#region CreateControlCharTable
		static ControlCharTranslatorTable controlCharHT = CreateControlCharTable();
		static ControlCharTranslatorTable CreateControlCharTable() {
			ControlCharTranslatorTable table = new ControlCharTranslatorTable();
			table.Add('\\', OnEscapedChar);
			return table;
		}
		#endregion
		readonly StringBuilder value;
		readonly PropertyModifier modifier;
		public StringPropertyBaseDestination(RtfImporter importer, PropertyModifier modifier)
			: base(importer) {
			Guard.ArgumentNotNull(modifier, "modifier");
			this.modifier = modifier;
			this.value = new StringBuilder();
		}
		protected override ControlCharTranslatorTable ControlCharHT { get { return controlCharHT; } }
		protected override KeywordTranslatorTable KeywordHT { get { return null; } }
		protected override void ProcessCharCore(char ch) {
			this.value.Append(ch);
		}
		public override void AfterPopRtfState() {
			string value = this.value.ToString().Trim('"', ' ');
			modifier(value);
		}
		protected override DestinationBase CreateClone() {
			return new StringPropertyBaseDestination(Importer, modifier);
		}
	}
	#endregion
}
