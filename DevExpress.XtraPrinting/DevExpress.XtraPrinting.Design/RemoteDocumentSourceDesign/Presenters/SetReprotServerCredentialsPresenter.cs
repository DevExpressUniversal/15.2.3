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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using DevExpress.ReportServer.Printing;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Presenters {
	class SetReportServerCredentialsPresenter<TView> : RemoteDocumentSourcePagePresenterBase<IPageView>
		where TView : ISetReportServerCredentialsView {
		readonly ReportServerConnectionHelper helper = new ReportServerConnectionHelper();
		readonly ConfigFileHelper configFileHelper = new ConfigFileHelper("Servers.xml");
		List<string> servers;
		Action ifValidAction;
		public override bool FinishEnabled {
			get { return false; }
		}
		public override bool MoveNextEnabled {
			get { return ValidateCredentials(); }
		}
		protected new ISetReportServerCredentialsView View { get { return (ISetReportServerCredentialsView)base.View; } }
		public SetReportServerCredentialsPresenter(ISetReportServerCredentialsView view)
			: base(view) {
		}
		public override void Begin() {
			FillServers();
			View.CredentialsChanged += View_CredentialsChanged;
			View.NavigateLinkClicked += View_NavigateLinkClicked;
			View.ServerUri = Model.ServiceUri;
			View.AuthenticationType = Model.AuthenticationType == AuthenticationType.None ? AuthenticationType.Windows : Model.AuthenticationType;
			View.UserName = Model.UserName;
			View.Password = Model.Password;
		}
		public override void Commit() {
			Model.ServiceUri = View.ServerUri;
			Model.AuthenticationType = View.AuthenticationType;
			Model.UserName = View.UserName;
			Model.Password = View.Password;
			SaveServerList();
			View.NavigateLinkClicked -= View_NavigateLinkClicked;
			View.CredentialsChanged -= View_CredentialsChanged;
		}
		public override Type GetNextPageType() {
			return typeof(ChooseRemoteReportPresenter<IChooseRemoteReportView>);
		}
		void View_NavigateLinkClicked(object sender, EventArgs e) {
			if(MoveNextEnabled)
				Process.Start(UrlHelper.AppendProtocolIfNeeded(View.ServerUri));
		}
		void View_CredentialsChanged(object sender, EventArgs e) {
			View.EnableLink(ValidateEndpointAddress());
			RaiseChanged();
		}
		bool ValidateCredentials() {
			return ValidateEndpointAddress() && ((View.AuthenticationType & (AuthenticationType.Windows | AuthenticationType.Guest)) > 0  ? true : !string.IsNullOrWhiteSpace(View.UserName) && !string.IsNullOrWhiteSpace(View.Password));
		}
		bool ValidateEndpointAddress() {
			var address = UrlHelper.AppendProtocolIfNeeded(View.ServerUri);
			if(string.IsNullOrWhiteSpace(address))
				return false;
			address = address.Replace('\\', '/');
			Uri uri;
			return Uri.TryCreate(address, UriKind.Absolute, out uri) ? Regex.IsMatch(uri.Scheme, @"http(s?)", RegexOptions.IgnoreCase) : false;
		}
		public override void ValidatePage(Action ifValidAction) {
			var address = UrlHelper.AppendProtocolIfNeeded(View.ServerUri);
			if(View.ServerUri != address)
				View.ServerUri = address;
			this.ifValidAction = ifValidAction;
			View.ShowWaitPanel(true);
			helper.Login(View.ServerUri, View.AuthenticationType, View.UserName, View.Password, args => {
				if(args.Error != null) {
					View.ShowWaitPanel(false);
					RaiseError(args.Error.Message);
					return;
				}
				if(args.Result) {
					helper.CreateClientAndPing(View.ServerUri, false, (client, pingArgs) => {
						if(pingArgs.Error != null) {
							RaiseError(pingArgs.Error.Message);
							return;
						}
						Model.Client = client;
						AddNewServer();
						View.ShowWaitPanel(false);
						ifValidAction();
					});
				} else {
					View.ShowWaitPanel(false);
					RaiseError("Access is denied.");
				}
			});
		}
		void FillServers() {
			servers = new List<string>();
			string filePath = configFileHelper.GetLoadFilePath();
			if(!string.IsNullOrEmpty(filePath)) {
				LoadServerList(filePath);
				View.FillServers(servers);
			}
		}
		void LoadServerList(string fileName) {
			XmlDocument xmlDocument = new XmlDocument();
			try {
				xmlDocument.Load(fileName);
			} catch { }
			XmlNodeList nodes = xmlDocument.GetElementsByTagName("Server");
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
				xwriter.WriteStartElement("Server");
				xwriter.WriteElementString("Uri", server);
				xwriter.WriteEndElement();
			}
			xwriter.WriteEndElement();
			xwriter.WriteEndDocument();
			xwriter.Close();
			configFileHelper.DeletePreviousFile();
		}
		void AddNewServer() {
			View.CredentialsChanged -= View_CredentialsChanged;
			Uri uri;
			var activeServer = View.ServerUri;
			var lastChar = activeServer[activeServer.Length - 1].ToString();
			if(lastChar == "/" || lastChar == "\\")
				activeServer = activeServer.Substring(0, activeServer.Length - 1);
			if(Uri.TryCreate(activeServer, UriKind.Absolute, out uri)) {
				if(!servers.Contains(uri.AbsoluteUri)) {
					View.FillServers(new List<string>());
					servers.Add(uri.OriginalString);
					servers.Sort();
					View.FillServers(servers);
				}
			}
			View.ServerUri = activeServer;
			View.CredentialsChanged -= View_CredentialsChanged;
		}
	}
}
