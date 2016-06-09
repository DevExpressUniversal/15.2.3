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
using System.Text.RegularExpressions;
using System.Xml;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Presenters {
	public class SetReportServiceReportNamePresenter<TView> : RemoteDocumentSourcePagePresenterBase<IPageView>, ISetReportServiceReportNamePresenter
		where TView : ISetReportServiceReportNameView {
		const string ServiceElementName = "Service";
		readonly ConfigFileHelper configFileHelper = new ConfigFileHelper("Services.xml");
		readonly ReportTypesProvider reportTypesProvider;
		List<string> servers;
		protected new ISetReportServiceReportNameView View { get { return (ISetReportServiceReportNameView)base.View; } }
		public override bool FinishEnabled {
			get { return Validate(); }
		}
		public override bool MoveNextEnabled {
			get { return false; }
		}
		public SetReportServiceReportNamePresenter(ISetReportServiceReportNameView view, ReportTypesProvider reportTypesProvider)
			: base(view) {
			this.reportTypesProvider = reportTypesProvider;
		}
		protected bool Validate() {
			return ValidateUri() && !string.IsNullOrWhiteSpace(View.ReportName);
		}
		void FillServers() {
			servers = new List<string>();
			string filePath = configFileHelper.GetLoadFilePath();
			if(!string.IsNullOrEmpty(filePath)) {
				LoadServiceList(filePath);
				View.FillServices(servers);
			}
		}
		void LoadServiceList(string fileName) {
			XmlDocument xmlDocument = new XmlDocument();
			try {
				xmlDocument.Load(fileName);
			} catch { }
			XmlNodeList nodes = xmlDocument.GetElementsByTagName(ServiceElementName);
			foreach(XmlNode node in nodes) {
				var uri = node["Uri"].InnerText;
				var lastChar = uri[uri.Length - 1].ToString();
				if(lastChar == "/" || lastChar == "\\")
					uri = uri.Substring(0, uri.Length - 1);
				if(servers.Contains(uri))
					continue;
				servers.Add(node["Uri"].InnerText);
			}
		}
		void SaveServerList() {
			string filePath = configFileHelper.GetSaveFilePath();
			if(string.IsNullOrEmpty(filePath))
				return;
			XmlTextWriter xwriter = new XmlTextWriter(filePath, System.Text.Encoding.UTF8);
			xwriter.Formatting = Formatting.Indented;
			xwriter.WriteStartDocument(true);
			xwriter.WriteStartElement("Servers");
			foreach(string server in servers) {
				xwriter.WriteStartElement(ServiceElementName);
				xwriter.WriteElementString("Uri", server);
				xwriter.WriteEndElement();
			}
			xwriter.WriteEndElement();
			xwriter.WriteEndDocument();
			xwriter.Close();
			configFileHelper.DeletePreviousFile();
		}
		void AddNewServer() {
			Uri uri;
			var activeServer = View.ServiceUri;
			var lastChar = activeServer[activeServer.Length - 1].ToString();
			if(lastChar == "/" || lastChar == "\\")
				activeServer = activeServer.Substring(0, activeServer.Length - 1);
			if(Uri.TryCreate(activeServer, UriKind.Absolute, out uri)) {
				if(!servers.Contains(uri.AbsoluteUri)) {
					servers.Add(uri.OriginalString);
					servers.Sort();
					View.FillServices(servers);
				}
			}
			View.ServiceUri = activeServer;
		}
		public override void Begin() {
			FillServers();
			View.FillReports(reportTypesProvider.GetTypeNames());
			View.ReportStorageParametersChanged += View_ReportStorageParametersChanged;
			View.ServiceUri = Model.ServiceUri;
			View.ReportName = Model.ReportName;
		}
		public override void Commit() {
			var address = UrlHelper.AppendProtocolIfNeeded(View.ServiceUri);
			if(View.ServiceUri != address)
				View.ServiceUri = address;
			Model.ServiceUri = View.ServiceUri;
			Model.ReportName = View.ReportName;
			Model.AuthenticationType = ReportServer.Printing.AuthenticationType.None;
			SaveServerList();
			View.ReportStorageParametersChanged -= View_ReportStorageParametersChanged;
		}
		public override Type GetNextPageType() {
			return null;
		}
		public override void ValidatePage(Action ifValidAction) {
			var address = UrlHelper.AppendProtocolIfNeeded(View.ServiceUri);
			if(View.ServiceUri != address) {
				View.ServiceUri = address;
			}
			AddNewServer();
			ifValidAction();
		}
		void View_ReportStorageParametersChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		bool ValidateUri() {
			var address = UrlHelper.AppendProtocolIfNeeded(View.ServiceUri);
			return ValidateEndpointAddress(address);
		}
		static bool ValidateEndpointAddress(string address) {
			if(string.IsNullOrWhiteSpace(address))
				return false;
			address = address.Replace('\\', '/');
			Uri uri;
			return Uri.TryCreate(address, UriKind.Absolute, out uri) ? Regex.IsMatch(uri.Scheme, @"http(s?)", RegexOptions.IgnoreCase) : false;
		}
	}
}
