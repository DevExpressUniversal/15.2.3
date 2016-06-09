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
using System.Reflection;
using System.Xml;
using DevExpress.DataAccess;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		const string DataSourceFileName = @"customXML/dataSources.xml";
		#region DataSources
		protected internal virtual bool ShouldExportDataSources() {
			return Workbook.DataComponentInfos.Count != 0;
		}
		protected internal virtual void AddDataSourcesPackageContent() {
			if(!ShouldExportDataSources() && !ShouldExportParameters())
				return;
			AddPackageContent(DataSourceFileName, ExportMailMerge());
			customXmlFileNames.Add("/" + DataSourceFileName);
		}
		protected internal virtual CompressedStream ExportMailMerge() {
			return CreateXmlContent(GenerateMailMergeCustomXmlContent, CreateXmlDataSourceWriterSettings());
		}
		protected internal virtual void GenerateMailMergeCustomXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			DocumentContentWriter.WriteStartElement("MailMerge");
			try {
				int index = Workbook.DataComponentInfos.DefaultIndex;
				if(index != -1)
					writer.WriteAttributeString("Default", index.ToString());
				if(ShouldExportDataSources())
					GenerateDataSourcesXmlContent(writer);
				if(ShouldExportParameters())
					GenerateParametersXmlContent(writer);
			}
			finally{
				DocumentContentWriter.WriteEndElement();
			}
		}
		protected internal virtual void GenerateDataSourcesXmlContent(XmlWriter writer) {
			writer.WriteStartElement("DataSources");
			try {
				if(Workbook.MailMergeDataSource is IDataComponent && Workbook.DataComponentInfos.DefaultIndex != -1 && !String.IsNullOrEmpty(Workbook.MailMergeDataMember)) {
					writer.WriteAttributeString("DataMember", Workbook.MailMergeDataMember);
				}
				foreach(DataComponentInfo dataComponent in Workbook.DataComponentInfos) {
					ExportDataComponent(dataComponent);
				}
			}
			finally {
				writer.WriteEndElement();
			}
		}
		protected internal virtual void ExportDataComponent(DataComponentInfo dataComponent) {
			DocumentContentWriter.WriteRaw(dataComponent.XmlContent.ToString());
		}
		protected internal XmlWriterSettings CreateXmlDataSourceWriterSettings() {
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = false;
			settings.Encoding = DXEncoding.UTF8NoByteOrderMarks;
			settings.CheckCharacters = true;
			settings.OmitXmlDeclaration = true;
			return settings;
		}
		#endregion
		#region Parameters
		protected internal virtual bool ShouldExportParameters() {
			return Workbook.MailMergeParameters.Count != 0;
		}
		protected internal virtual void GenerateParametersXmlContent(XmlWriter writer) {
			writer.WriteStartElement("Parameters");
			try {
				foreach(SpreadsheetParameter parameter in Workbook.MailMergeParameters) {
					ExportParameter(parameter);
				}
			}
			finally {
				writer.WriteEndElement();
			}
		}
		protected internal virtual void ExportParameter(SpreadsheetParameter parameter) {
			DocumentContentWriter.WriteStartElement("Parameter");
			try {
				string name = parameter.Name;
				string type = ShouldUseAssemblyQualifiedName(parameter.Type) ? parameter.Type.FullName : parameter.Type.AssemblyQualifiedName;
				string value = ParameterHelper.ConvertValueToString(parameter.Value);
				DocumentContentWriter.WriteAttributeString("Name", name);
				DocumentContentWriter.WriteAttributeString("Type", type);
				DocumentContentWriter.WriteAttributeString("Value", value);
			}
			finally {
				DocumentContentWriter.WriteEndElement();
			}
		}
		bool ShouldUseAssemblyQualifiedName(Type type) {
#if !DXPORTABLE
			return type.Module.ScopeName == "CommonLanguageRuntimeLibrary";
#else
			string name = type.GetTypeInfo().Module.Name;
			return name.StartsWithInvariantCultureIgnoreCase("mscorlib") || name == "CommonLanguageRuntimeLibrary" || name.StartsWithInvariantCultureIgnoreCase("System");
#endif
		}
#endregion
	}
}
