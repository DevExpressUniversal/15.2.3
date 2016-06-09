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
using DevExpress.XtraPrinting;
using System.ComponentModel;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.DocumentServices.ServiceModel.Client;
using System.ServiceModel;
using DevExpress.XtraPrinting.Native;
using System.Drawing;
using DevExpress.Printing.Core.ReportServer.Services;
using DevExpress.ReportServer.ServiceModel.Client;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.XtraReports;
using System.Collections.Generic;
using DevExpress.XtraReports.UI;
using DevExpress.DocumentServices.ServiceModel;
using DevExpress.XtraPrinting.Native.DrillDown;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.ReportServer.Printing {
	[
	Designer("DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesigner," + AssemblyInfo.SRAssemblyPrintingDesign),
	ToolboxBitmap(typeof(DevExpress.Printing.ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "RemoteDocumentSource.bmp"),
	ToolboxItem(false)
	]
	public class RemoteDocumentSource : Component, IReport {
		#region inner classes 
		public enum ContractType {
			IAsyncReportService,
			IReportServerFacadeAsync,
			IAuthenticationService
		}
		#endregion
		#region fields and properties
		readonly string authenticationPattern = "_Authentication";
		readonly string useSSLPattern = "_SSL";
		readonly string formsAuthenticationPattern = "_Forms";
		readonly string winAuthenticationPattern = "_Win";
		readonly string reportServerFacadePath = "ReportServerFacade.svc";
		readonly string winAuthenticationServicePath = "WindowsAuthentication/AuthenticationService.svc";
		readonly string formsAuthenticationServicePath = "AuthenticationService.svc";
		RemotePrintingSystem ps;
		string reportName;
		InstanceIdentity reportIdentity;
		string serviceUri;
		bool needToLogin = true;
		AuthenticationType authenticationType;
		string endpointConfigurationPrefix;
		FormsAuthenticationEndpointBehavior behavior;
		DefaultValueParameterContainer parameters;
		[DefaultValue(null),
		Category(NativeSR.CatPrinting)]
		public string ServiceUri {
			get {
				return serviceUri;
			}
			set {
				if(!string.IsNullOrEmpty(value))
					TryCreateEndpointAddress(value);
				if(serviceUri == value)
					return;
				serviceUri = value;
				needToLogin = true;
			}
		}
		[DefaultValue(null),
		Browsable(false),
		Category(NativeSR.CatPrinting)]
		public string ReportName {
			get {
				return reportName;
			}
			set {
				if(!string.IsNullOrEmpty(value) && ReportIdentity != null)
					ExceptionHelper.ThrowInvalidOperationException("Use either report name or report identity, but not both of them.");
				else
					reportName = value;
			}
		}
		[DefaultValue(null),
		Browsable(false),
		Category(NativeSR.CatPrinting)]
		public InstanceIdentity ReportIdentity {
			get {
				return reportIdentity;
			}
			set {
				if(value != null && !string.IsNullOrEmpty(ReportName))
					ExceptionHelper.ThrowInvalidOperationException("Use either report name or report identity, but not both of them.");
				else
					reportIdentity = value;
			}
		}
		[DefaultValue(AuthenticationType.None)]
		[Category(NativeSR.CatPrinting)]
		public AuthenticationType AuthenticationType {
			get {
				return authenticationType;
			}
			set {
				if(authenticationType == value)
					return;
				authenticationType = value;
				needToLogin = true;
			}
		}
		[Category(NativeSR.CatPrinting)]
		[DefaultValue(null)]
		public string EndpointConfigurationPrefix {
			get {
				return endpointConfigurationPrefix;
			}
			set {
				endpointConfigurationPrefix = value;
			}
		}
		protected bool NeedToLogin {
			get {
				return needToLogin && AuthenticationType != Printing.AuthenticationType.None;
			}
		}
		ICredentialsService CredentialsService { get { return PrintingSystem.GetService<ICredentialsService>(); } }
		protected RemotePrintingSystem PrintingSystem {
			get {
				return ((IDocumentSource)this).PrintingSystemBase as RemotePrintingSystem;
			}
		}
		PrintingSystemBase IDocumentSource.PrintingSystemBase {
			get {
				if(ps == null) {
					ps = CreatePrintingSystem();
				}
				return ps;
			}
		}
		IPrintingSystem ILink.PrintingSystem {
			get { return PrintingSystem; }
		}
		#endregion
		#region events
		[Category(NativeSR.CatPrinting)]
		public event CredentialsDemandedEventHandler ReportServerCredentialsDemanded;
		[Category(NativeSR.CatPrinting)]
		public event EventHandler<ReportServiceClientDemandedEventArgs> ReportServiceClientDemanded;
		[Category(NativeSR.CatPrinting)]
		public event EventHandler<AuthenticationServiceClientDemandedEventArgs> AuthenticationServiceClientDemanded;
		[Category(NativeSR.CatPrinting)]
		public event EventHandler<ParametersRequestEventArgs> ParametersRequestBeforeShow;
		[Category(NativeSR.CatPrinting)]
		public event EventHandler<ParametersRequestValueChangedEventArgs> ParametersRequestValueChanged;
		#endregion
		#region methods
		RemotePrintingSystem CreatePrintingSystem() {
			var printingSystem = new RemotePrintingSystem();
			printingSystem.AddService<IReport>(this);
			return printingSystem;
		}
		void ILink.CreateDocument(bool buildPagesInBackground) {
			CreateDocumentCore();
		}
		public void CreateDocument() {
			Clear();
			CreateDocumentCore();
		}
		public void CreateDocument(DefaultValueParameterContainer defaultParameterValues) {
			Clear();
			this.parameters = defaultParameterValues;
			CreateDocumentCore();
		}
		void CreateDocumentCore() {
			if(ReportIdentity == null && string.IsNullOrWhiteSpace(ReportName)) {
				RaiseCreateDocumentError(new InvalidOperationException("It is impossible to create a document because neither ReportIdentity nor ReportName properties are set."));
			}
			if(reportIdentity == null)
				reportIdentity = new ReportNameIdentity(ReportName);
			if(AuthenticationType == Printing.AuthenticationType.None || !NeedToLogin)
				RequestRemoteDocument();
			else Login();
		}
		void Clear() {
			parameters = null;
		}
		void Login() {
			if(AuthenticationType == Printing.AuthenticationType.Forms)
				RequestCredentials();
			else
				LoginCore(null, null);
		}
		void LoginCore(string userName, string password) {
			var authenticationClient = CreateAuthenticationClient();
			authenticationClient.Login(userName, password, null, args => {
				if(args.Error != null) {
					RaiseCreateDocumentError(args.Error);
					return;
				}
				if(args.Result) {
					needToLogin = false;
					RequestRemoteDocument();
				} else
					RaiseCreateDocumentError(new Exception("Invalid User Credentials."));
			});
		}
		void RequestRemoteDocument() {
			IReportServiceClient client;
			try {
				client = CreateReportServiceClient();
			} catch(Exception e) {
				PrintingSystem.OnCreateDocumentException(new ExceptionEventArgs(e));
				return;
			}
			PrintingSystem.EnsureClient(client);
			try {
				PrintingSystem.RequestRemoteDocument(ReportIdentity, parameters ?? new DefaultValueParameterContainer());
			} catch(Exception error) {
				RaiseCreateDocumentError(error);
			}
		}
		void OnLoginCompleted(bool succeeded, Exception error) {
			if(error != null) {
				RaiseCreateDocumentError(error);
				return;
			}
			if(succeeded)
				RequestRemoteDocument();
			else
				RaiseCreateDocumentError(new Exception("Invalid user name or password."));
		}
		void RequestCredentials() {
			var args = new CredentialsEventArgs();
			if(ReportServerCredentialsDemanded != null) {
				ReportServerCredentialsDemanded(this, args);
				if(args.Handled) {
					LoginCore(args.UserName, args.Password);
					return;
				}
			}
			if(CredentialsService != null) {
				CredentialsService.RequestCredentialsFailed += CredentialsService_RequestCredentialsFailed;
				CredentialsService.RequestCredentials(LoginCore);
				return;
			}
			RaiseCreateDocumentError(new InvalidOperationException("ReportServer credentials are not specified."));
		}
		void CredentialsService_RequestCredentialsFailed(object sender, EventArgs e) {
			CredentialsService.RequestCredentialsFailed -= CredentialsService_RequestCredentialsFailed;
		}
		IAuthenticationServiceClient CreateAuthenticationClient() {
			if(AuthenticationServiceClientDemanded != null) {
				var args = new AuthenticationServiceClientDemandedEventArgs();
				AuthenticationServiceClientDemanded(this, args);
				if(args.Client != null)
					return args.Client;
			}
			AuthenticationServiceClientFactory factory;
			if(string.IsNullOrEmpty(EndpointConfigurationPrefix)) {
				behavior = new FormsAuthenticationEndpointBehavior();
				factory = new AuthenticationServiceClientFactory(GetEndpointAddress(true), AuthenticationType);
				factory.ChannelFactory.Endpoint.Behaviors.Add(behavior);
			} else {
				var endpointAddress = GetEndpointAddress(true);
				factory = new AuthenticationServiceClientFactory(GetEndpointName(ContractType.IAuthenticationService, endpointAddress), endpointAddress);
			}
			return factory.Create();
		}
		protected virtual IReportServiceClient CreateReportServiceClient() {
			if(ReportServiceClientDemanded != null) {
				var args = new ReportServiceClientDemandedEventArgs();
				ReportServiceClientDemanded(this, args);
				if(args.Client != null)
					return args.Client;
			}
			var endpointAddress = new EndpointAddress(ServiceUri);
			string endpointName;
			IReportServiceClientFactory factory;
			if(AuthenticationType == Printing.AuthenticationType.None) {
				factory = new ReportServiceClientFactory(new EndpointAddress(ServiceUri));
			} else {
				if(string.IsNullOrEmpty(EndpointConfigurationPrefix)) {
					factory = new ReportServerClientFactory(GetEndpointAddress(false));
					((ReportServerClientFactory)factory).ChannelFactory.Endpoint.Behaviors.Add(behavior);
				} else {
					endpointName = GetEndpointName(ContractType.IReportServerFacadeAsync, endpointAddress);
					if(string.IsNullOrEmpty(endpointName))
						throw new InvalidOperationException("Endpoint configuration name are not specified.");
					factory = new ReportServerClientFactory(GetEndpointName(ContractType.IAsyncReportService, endpointAddress), GetEndpointAddress(false));
				}
			}
			return factory.Create();
		}
		protected virtual string GetEndpointName(ContractType contractType, EndpointAddress endpointAddress) {
			if(AuthenticationType == Printing.AuthenticationType.None)
				return string.Empty;
			var configurationName = EndpointConfigurationPrefix;
			if(contractType == ContractType.IAuthenticationService) {
				configurationName += authenticationPattern;
				if(AuthenticationType == Printing.AuthenticationType.Windows)
					configurationName += winAuthenticationPattern;
				else if(AuthenticationType == Printing.AuthenticationType.Forms)
					configurationName += formsAuthenticationPattern;
			}
			if(CheckUseSSL(ServiceUri))
				configurationName += useSSLPattern;
			return configurationName;
		}
		protected virtual EndpointAddress GetEndpointAddress(bool isAuthenticationServiceEnpoint) {
			var baseUri = new Uri(ServiceUri);
			if(isAuthenticationServiceEnpoint) {
				return AuthenticationType == Printing.AuthenticationType.Windows
					? new EndpointAddress(new Uri(baseUri, winAuthenticationServicePath))
					: new EndpointAddress(new Uri(baseUri, formsAuthenticationServicePath));
			} else {
				return new EndpointAddress(new Uri(baseUri, reportServerFacadePath));
			}
		}
		static bool CheckUseSSL(string url) {
			var uri = new Uri(url);
			return uri.Scheme == Uri.UriSchemeHttps;
		}
		static EndpointAddress TryCreateEndpointAddress(string uri) {
			return new EndpointAddress(uri);
		}
		void RaiseCreateDocumentError(Exception e) {
			PrintingSystem.OnCreateDocumentException(new ExceptionEventArgs(e));
		}
		#endregion
		#region IReport Members
		void IReport.StopPageBuilding() {
			throw new NotImplementedException();
		}
		XtraPrinting.Drawing.Watermark IReport.Watermark {
			get { throw new NotImplementedException(); }
		}
		bool IReport.IsDisposed {
			get { return false; }
		}
		bool IReport.IsMetric {
			get { throw new NotImplementedException(); }
		}
		bool IReport.ShowPreviewMarginLines {
			get { throw new NotImplementedException(); }
		}
		void IReport.ApplyPageSettings(XtraPageSettingsBase destSettings) {
			throw new NotImplementedException();
		}
		void IReport.UpdatePageSettings(XtraPageSettingsBase sourceSettings, string paperName) {
			throw new NotImplementedException();
		}
		void IReport.RaiseParametersRequestBeforeShow(IList<XtraReports.Parameters.ParameterInfo> parametersInfo) {
			if (ParametersRequestBeforeShow != null)
				ParametersRequestBeforeShow(this, new ParametersRequestEventArgs(parametersInfo));
		}
		void IReport.RaiseParametersRequestValueChanged(IList<XtraReports.Parameters.ParameterInfo> parametersInfo, XtraReports.Parameters.ParameterInfo changedParameterInfo) {
			if (ParametersRequestValueChanged != null)
				ParametersRequestValueChanged(this, new ParametersRequestValueChangedEventArgs(parametersInfo, changedParameterInfo));
		}
		void IReport.RaiseParametersRequestSubmit(IList<XtraReports.Parameters.ParameterInfo> parametersInfo, bool createDocument) {
			try {
				PrintingSystem.SubmitParameters();
			} catch(Exception error) {
				RaiseCreateDocumentError(error);
			}
		}
		IReportPrintTool printTool;
		XtraReports.UI.IReportPrintTool IReport.PrintTool {
			get {
				return printTool;
			}
			set {
				printTool = value;
			}
		}
		bool IReport.RequestParameters {
			get { throw new NotImplementedException(); }
		}
		void IReport.CollectParameters(IList<XtraReports.Parameters.Parameter> list, Predicate<XtraReports.Parameters.Parameter> condition) {
			throw new NotImplementedException();
		}
		void IReport.ReleasePrintingSystem() {
			var oldPS = PrintingSystem;
			ps = null;
			((RemoteDocument)oldPS.Document).CloneDocumentData((RemoteDocument)this.PrintingSystem.Document);
		}
		event EventHandler<XtraReports.Parameters.ParametersRequestEventArgs> IReport.ParametersRequestSubmit {
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		#endregion
		#region IServiceProvider Members
		IDrillDownServiceBase drillDownService;
		object IServiceProvider.GetService(Type serviceType) {
			if(serviceType == typeof(IDrillDownServiceBase))
				return drillDownService ?? (drillDownService = new RemoteDrillDownService());
			return base.GetService(serviceType);
		}
		#endregion
		#region IExtensionsProvider Members
		IDictionary<String, String> extensions = new Dictionary<string, string>();
		IDictionary<string, string> XtraReports.Native.IExtensionsProvider.Extensions {
			get { return extensions; }
		}
		#endregion
	}
	public enum AuthenticationType {
		None = 0,
		Windows = 1,
		Forms = 2,
		Guest = 3
	}
}
