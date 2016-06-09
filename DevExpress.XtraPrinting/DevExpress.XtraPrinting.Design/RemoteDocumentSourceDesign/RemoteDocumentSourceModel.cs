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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.WizardFramework;
using DevExpress.ReportServer.Printing;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign {
	public class RemoteDocumentSourceModel : IWizardModel {
		public RemoteDocumentSourceModel() {
		}
		public RemoteDocumentSourceModel(RemoteDocumentSourceModel remoteDocumentSourceModel) {
			this.DocumentSourceType = remoteDocumentSourceModel.DocumentSourceType;
			this.ServiceUri = remoteDocumentSourceModel.ServiceUri;
			this.AuthenticationType = remoteDocumentSourceModel.AuthenticationType;
			this.UserName = remoteDocumentSourceModel.UserName;
			this.Password = remoteDocumentSourceModel.Password;
			this.Client = remoteDocumentSourceModel.Client;
			this.ReportId = remoteDocumentSourceModel.ReportId;
			this.ReportName = remoteDocumentSourceModel.ReportName;
			this.Endpoint = remoteDocumentSourceModel.Endpoint;
			this.GenerateEndpoints = remoteDocumentSourceModel.GenerateEndpoints;
		}
		public RemoteDocumentSourceType DocumentSourceType { get; set; }
		public object Clone() {
			return new RemoteDocumentSourceModel(this);
		}
		public string ServiceUri { get; set; }
		[DefaultValue(AuthenticationType.None)]
		public AuthenticationType AuthenticationType { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public ReportServer.ServiceModel.Client.IReportServerClient Client { get; set; }
		public int ReportId { get; set; }
		public string Endpoint { get; set; }
		public string ReportName { get; set; }
		public bool GenerateEndpoints { get; set; }
		public override bool Equals(object obj) {
			var model = obj as RemoteDocumentSourceModel;
			return model != null
				&& DocumentSourceType == model.DocumentSourceType
				&& ServiceUri == model.ServiceUri
				&& AuthenticationType == model.AuthenticationType
				&& UserName == model.UserName
				&& Password == model.Password
				&& ReportId == model.ReportId
				&& ReportName == model.ReportName
				&& Endpoint == model.Endpoint
				&& GenerateEndpoints == model.GenerateEndpoints;
		}
		public override int GetHashCode() {
			return 0;
		}
		internal void Clear() {
			this.ServiceUri = string.Empty;
			this.AuthenticationType = AuthenticationType.None;
			this.UserName = string.Empty;
			this.Password = string.Empty;
			this.ReportName = string.Empty;
			this.ReportId = -1;
			this.Endpoint = string.Empty;
			this.GenerateEndpoints = false;
		}
	}
}
