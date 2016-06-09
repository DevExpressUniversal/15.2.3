#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Xml.Linq;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native.Services {
	public static class DataSourcesJSContentGeneratorLogic {
		public static string GenerateDataSourceInfoJson(object dataSource, IDictionary<string, string> reportExtensions) {
			XElement dataSourceXml = DataSourceToXml(dataSource, reportExtensions);
			return JsonConverter.XmlToJson(dataSourceXml);
		}
		public static XtraReport CreateReportWithExtensions(IDictionary<string, string> reportExtensions) {
			var report = new XtraReport();
			if(reportExtensions != null) {
				foreach(var pair in reportExtensions) {
					report.Extensions[pair.Key] = pair.Value;
				}
			}
			return report;
		}
		static XElement DataSourceToXml(object dataSource, IDictionary<string, string> reportExtensions) {
			using(var report = CreateReportWithExtensions(reportExtensions)) {
				AssignAndFillObjectStorage(report, dataSource);
				var document = SerializeToXml(report);
				return GetDataSourceAsXml(document);
			}
		}
		static XElement GetDataSourceAsXml(XDocument document) {
			var root = document.Root;
			var storage = root.Element("ObjectStorage")
				?? root.Element("ComponentStorage");
			var element = storage.Elements().FirstOrDefault();
			CleanAttributes(element);
			return element;
		}
		static void AssignAndFillObjectStorage(XtraReport report, object dataSource) {
			report.DataSource = dataSource;
			var serializationContext = new XtraReportsSerializationContext { RootObject = report };
			serializationContext.ReferencedObjects.Add(dataSource);
			serializationContext.FillObjectStorage();
		}
		static XDocument SerializeToXml(XtraReport report) {
			var serializer = new XtraReportsXmlSerializer(report);
			using(var stream = new MemoryStream()) {
				serializer.SerializeObject(report, stream, "");
				stream.Position = 0;
				return XDocument.Load(stream);
			}
		}
		static void CleanAttributes(XElement element) {
			var refAttribute = element.Attribute("Ref");
			if(refAttribute != null) {
				refAttribute.Remove();
			}
		}
	}
}
