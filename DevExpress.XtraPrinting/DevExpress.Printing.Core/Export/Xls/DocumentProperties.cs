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
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xls {
	public partial class XlsDataAwareExporter {
		#region Document properties
		void ExportSummaryInformation() {
			OlePropertyStreamContent content = new OlePropertyStreamContent();
			GenerateSummary(content);
			content.Write(this.summaryInformationWriter);
		}
		void ExportDocumentSummaryInformation() {
			OlePropertyStreamContent content = new OlePropertyStreamContent();
			GenerateDocSummary(content);
			GenerateUserDefined(content);
			content.Write(this.docSummaryInformationWriter);
		}
		void GenerateSummary(OlePropertyStreamContent content) {
			OlePropertySetSummary propertySet = new OlePropertySetSummary();
			propertySet.Properties.Add(new OlePropertyCodePage() { Value = OlePropDefs.CodePageUnicode });
			if(!string.IsNullOrEmpty(documentProperties.Title))
				propertySet.Properties.Add(new OlePropertyTitle(OlePropDefs.VT_LPWSTR) { Value = documentProperties.Title });
			if(!string.IsNullOrEmpty(documentProperties.Subject))
				propertySet.Properties.Add(new OlePropertySubject(OlePropDefs.VT_LPWSTR) { Value = documentProperties.Subject });
			if(!string.IsNullOrEmpty(documentProperties.Author))
				propertySet.Properties.Add(new OlePropertyAuthor(OlePropDefs.VT_LPWSTR) { Value = documentProperties.Author });
			if(!string.IsNullOrEmpty(documentProperties.Keywords))
				propertySet.Properties.Add(new OlePropertyKeywords(OlePropDefs.VT_LPWSTR) { Value = documentProperties.Keywords });
			if(!string.IsNullOrEmpty(documentProperties.Description))
				propertySet.Properties.Add(new OlePropertyComments(OlePropDefs.VT_LPWSTR) { Value = documentProperties.Description });
			if(!string.IsNullOrEmpty(documentProperties.Author))
				propertySet.Properties.Add(new OlePropertyLastAuthor(OlePropDefs.VT_LPWSTR) { Value = documentProperties.Author });
			if(documentProperties.Created != DateTime.MinValue) {
				propertySet.Properties.Add(new OlePropertyCreated() { Value = documentProperties.Created });
				propertySet.Properties.Add(new OlePropertyModified() { Value = documentProperties.Created });
			}
			if(!string.IsNullOrEmpty(documentProperties.Application))
				propertySet.Properties.Add(new OlePropertyApplication(OlePropDefs.VT_LPWSTR) { Value = documentProperties.Application });
			propertySet.Properties.Add(new OlePropertyDocSecurity() { Value = (int)documentProperties.Security });
			content.PropertySets.Add(propertySet);
		}
		void GenerateDocSummary(OlePropertyStreamContent content) {
			OlePropertySetDocSummary propertySet = new OlePropertySetDocSummary();
			propertySet.Properties.Add(new OlePropertyCodePage() { Value = OlePropDefs.CodePageUnicode });
			if(!string.IsNullOrEmpty(documentProperties.Category))
				propertySet.Properties.Add(new OlePropertyCategory(OlePropDefs.VT_LPWSTR) { Value = documentProperties.Category });
			if(!string.IsNullOrEmpty(documentProperties.Manager))
				propertySet.Properties.Add(new OlePropertyManager(OlePropDefs.VT_LPWSTR) { Value = documentProperties.Manager });
			if(!string.IsNullOrEmpty(documentProperties.Company))
				propertySet.Properties.Add(new OlePropertyCompany(OlePropDefs.VT_LPWSTR) { Value = documentProperties.Company });
			if(!string.IsNullOrEmpty(documentProperties.Version)) {
				string[] parts = documentProperties.Version.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
				try {
					int version = Convert.ToInt32(parts[0]) << 16;
					propertySet.Properties.Add(new OlePropertyVersion() { Value = version });
				}
				catch { }
			}
			content.PropertySets.Add(propertySet);
		}
		void GenerateUserDefined(OlePropertyStreamContent content) {
			XlDocumentCustomProperties props = documentProperties.Custom;
			if(props.Count == 0)
				return;
			OlePropertySetUserDefined propertySet = new OlePropertySetUserDefined();
			OlePropertyDictionary dictionary = new OlePropertyDictionary();
			propertySet.Properties.Add(dictionary);
			propertySet.Properties.Add(new OlePropertyCodePage() { Value = OlePropDefs.CodePageUnicode });
			int propertyId = OlePropDefs.PID_NORMAL_MIN;
			foreach(string name in props.Names) {
				XlCustomPropertyValue value = props[name];
				if(value.Type == XlVariantValueType.Text)
					propertySet.Properties.Add(new OlePropertyString(propertyId, OlePropDefs.VT_LPWSTR) { Value = value.TextValue });
				else if(value.Type == XlVariantValueType.DateTime)
					propertySet.Properties.Add(new OlePropertyFileTime(propertyId) { Value = value.DateTimeValue });
				else if(value.Type == XlVariantValueType.Boolean)
					propertySet.Properties.Add(new OlePropertyBool(propertyId) { Value = value.BooleanValue });
				else if(value.Type == XlVariantValueType.Numeric) {
					double doubleValue = value.NumericValue;
					int intValue = (int)doubleValue;
					if(intValue == doubleValue)
						propertySet.Properties.Add(new OlePropertyInt32(propertyId) { Value = intValue });
					else
						propertySet.Properties.Add(new OlePropertyDouble(propertyId) { Value = doubleValue });
				}
				else
					continue;
				dictionary.Entries.Add(propertyId, name);
				propertyId++;
			}
			if(dictionary.Entries.Count > 0)
				content.PropertySets.Add(propertySet);
		}
		#endregion
	}
}
