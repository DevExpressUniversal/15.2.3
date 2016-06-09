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

using System;
using System.ComponentModel;
using DevExpress.DocumentServices.ServiceModel;
using DevExpress.ReportServer.Printing;
using DevExpress.Web;
using DevExpress.XtraReports.Web.Localization;
using Control = System.Web.UI.Control;
namespace DevExpress.XtraReports.Web.DocumentViewer {
	public class DocumentViewerRemoteSourceSettings : PropertiesBase, IPropertiesOwner {
		const string
			ReportIdName = "ReportId",
			ServerUriName = "ServerUri",
			ReportTypeNameName = "ReportTypeName",
			EndpointConfigurationNameName = "EndpointConfigurationName",
			AuthenticationTypeName = "AuthenticationType",
			TokenStorageModeName = "TokenStorageMode";
		const TokenStorageMode TokenStorageModeDefault = TokenStorageMode.Session;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const int DefaultReportId = -1;
		const AuthenticationType DefaultAuthenticationType = AuthenticationType.None;
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerRemoteSourceSettingsReportId")]
#endif
		[DefaultValue(DefaultReportId)]
		[AutoFormatDisable]
		[Localizable(false)]
		[NotifyParentProperty(true)]
		public int ReportId {
			get { return GetIntProperty(ReportIdName, DefaultReportId); }
			set {
				SetIntProperty(ReportIdName, DefaultReportId, value);
				base.Changed();
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerRemoteSourceSettingsReportTypeName")]
#endif
		[DefaultValue("")]
		[AutoFormatEnable]
		[Localizable(false)]
		[TypeConverter("DevExpress.Web.Design.Reports.Converters.ReportTypeNameConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		[NotifyParentProperty(true)]
		public string ReportTypeName {
			get { return GetStringProperty(ReportTypeNameName, ""); }
			set {
				SetStringProperty(ReportTypeNameName, "", value);
				base.Changed();
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerRemoteSourceSettingsAuthenticationType")]
#endif
		[AutoFormatDisable]
		[Localizable(false)]
		[NotifyParentProperty(true)]
		[DefaultValue(DefaultAuthenticationType)]
		public AuthenticationType AuthenticationType {
			get { return (AuthenticationType)GetEnumProperty(AuthenticationTypeName, DefaultAuthenticationType); }
			set {
				SetEnumProperty(AuthenticationTypeName, DefaultAuthenticationType, value);
				base.Changed();
			}
		}
		[DefaultValue("")]
		[AutoFormatEnable]
		[Localizable(false)]
		[NotifyParentProperty(true)]
		public string ServerUri {
			get { return GetStringProperty(ServerUriName, ""); }
			set {
				SetStringProperty(ServerUriName, "", value);
				base.Changed();
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerRemoteSourceSettingsEndpointConfigurationName")]
#endif
		[DefaultValue("")]
		[AutoFormatDisable]
		[Localizable(false)]
		[NotifyParentProperty(true)]
		public string EndpointConfigurationName {
			get { return GetStringProperty(EndpointConfigurationNameName, ""); }
			set {
				SetStringProperty(EndpointConfigurationNameName, "", value);
				base.Changed();
			}
		}
		[AutoFormatDisable]
		[Localizable(false)]
		[NotifyParentProperty(true)]
		[DefaultValue(TokenStorageModeDefault)]
		public TokenStorageMode TokenStorageMode {
			get { return (TokenStorageMode)GetEnumProperty(TokenStorageModeName, TokenStorageModeDefault); }
			set {
				SetEnumProperty(TokenStorageModeName, TokenStorageModeDefault, value);
				base.Changed();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public ITokenStorage CustomTokenStorage { get; set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerRemoteSourceSettingsRequestCredentials")]
#endif
		public event EventHandler<WebAuthenticatorLoginEventArgs> RequestCredentials;
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerRemoteSourceSettingsRequestParameters")]
#endif
		public event EventHandler<RequestParametersEventArgs> RequestParameters;
		internal Control OwnerControl {
			get { return base.Owner as Control; }
		}
		internal bool IsChanged {
			get {
				return !string.IsNullOrEmpty(ReportTypeName)
					|| ReportId != DefaultReportId
					|| !string.IsNullOrEmpty(ServerUri)
					|| !string.IsNullOrEmpty(EndpointConfigurationName)
					|| AuthenticationType != DefaultAuthenticationType;
			}
		}
		public DocumentViewerRemoteSourceSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Reset() {
			ReportId = DefaultReportId;
			EndpointConfigurationName = "";
			ServerUri = "";
			ReportTypeName = "";
			AuthenticationType = DefaultAuthenticationType;
			CustomTokenStorage = null;
		}
		#region IPropertiesOwner Members
		public void Changed(PropertiesBase properties) {
			base.Changed();
		}
		#endregion
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as DocumentViewerRemoteSourceSettings;
			if(src == null) {
				return;
			}
			ReportId = src.ReportId;
			ReportTypeName = src.ReportTypeName;
			EndpointConfigurationName = src.EndpointConfigurationName;
			ServerUri = src.ServerUri;
			AuthenticationType = src.AuthenticationType;
			CustomTokenStorage = src.CustomTokenStorage;
			RequestCredentials += src.RequestCredentials;
			RequestParameters += src.RequestParameters;
		}
		internal void RaiseRequestParameters(IParameterContainer parameters) {
			if(RequestParameters != null) {
				RequestParameters(this, new RequestParametersEventArgs(parameters));
			}
		}
		void RaiseRequestCredentials(WebAuthenticatorLoginEventArgs args) {
			if(RequestCredentials != null) {
				RequestCredentials(this, args);
			}
		}
		internal WebCredential GetWebCredentials() {
			var args = new WebAuthenticatorLoginEventArgs();
			RaiseRequestCredentials(args);
			if(!args.Handled || args.Credential == null) {
				throw new InvalidOperationException(ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RemoteRequestCredentials_Error));
			}
			return args.Credential;
		}
	}
}
