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
using System.Text;
using System.Xml.Linq;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native {
	public static class ReportRestorer {
		static string reportTemplate;
		static readonly object reportTemplateSync = new object();
		static string ReportTemplate {
			get {
				if(reportTemplate == null) {
					lock(reportTemplateSync) {
						if(reportTemplate == null) {
							reportTemplate = GenerateReportForTemplate();
						}
					}
				}
				return reportTemplate;
			}
		}
		public static XtraReport RestoreWithDataSource(string dataSourceJson, Dictionary<string, string> reportExtensions) {
			var reportDocument = XDocument.Parse(ReportTemplate);
			AssignExtensions(reportDocument, reportExtensions);
			if(!string.IsNullOrEmpty(dataSourceJson)) {
				AssignDataSource(reportDocument, dataSourceJson);
			}
			var xmlBytes = Encoding.UTF8.GetBytes(reportDocument.ToString());
			using(var stream = new MemoryStream(xmlBytes)) {
				var report = new XtraReport();
				report.LoadLayoutFromXml(stream);
				return report;
			}
		}
		static XDocument GetDataSourceDocument(string dataSourceJson) {
			var dataSourceXmlBytes = JsonConverter.JsonToXml(dataSourceJson, firstChildAsRoot: false);
			using(var stream = new MemoryStream(dataSourceXmlBytes)) {
				return XDocument.Load(stream);
			}
		}
		static void AssignExtensions(XDocument reportDocument, Dictionary<string, string> reportExtensions) {
			if(reportExtensions == null) {
				return;
			}
			var extensions = reportDocument.Root.Element("Extensions");
			int i = 1;
			foreach(var reportExtensionPair in reportExtensions) {
				var reportExtensionElement = new XElement("Item" + (i++).ToString());
				reportExtensionElement.SetAttributeValue("Key", reportExtensionPair.Key);
				reportExtensionElement.SetAttributeValue("Value", reportExtensionPair.Value);
				extensions.Add(reportExtensionElement);
			}
		}
		static void AssignDataSource(XDocument reportDocument, string dataSourceJson) {
			XDocument dataSourceDocument = GetDataSourceDocument(dataSourceJson);
			var dataSourceElement = new XElement(dataSourceDocument.Root) { Name = "Item1" };
			dataSourceElement.SetAttributeValue("Ref", "0");
			XElement objectStorage = reportDocument.Root.Element("ObjectStorage");
			objectStorage.Add(dataSourceElement);
		}
		static string GenerateReportForTemplate() {
			using(var report = new XtraReport())
			using(var reportStream = new MemoryStream()) {
				report.Extensions["key"] = "value";
				report.DataSource = new object();
				report.SaveLayoutToXml(reportStream);
				reportStream.Position = 0;
				var xml = XDocument.Load(reportStream);
				var extensions = xml.Root.Element("Extensions");
				extensions.RemoveAll();
				var objectStorage = xml.Root.Element("ObjectStorage");
				objectStorage.RemoveAll();
				return xml.ToString();
			}
		}
	}
}
