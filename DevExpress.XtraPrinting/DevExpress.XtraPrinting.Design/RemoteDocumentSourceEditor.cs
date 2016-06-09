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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.ReportServer.Printing;
using DevExpress.ReportServer.ServiceModel.Client;
using DevExpress.ReportServer.ServiceModel.DataContracts;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign;
using DevExpress.XtraPrinting.Native;
using System.Collections.ObjectModel;
namespace DevExpress.XtraPrinting.Design {
	class RemoteDocumentSourceEditor2 : UITypeEditor {
		AppConfigHelper configHelper;
		RemoteDocumentSource RemoteDocumentSource { get; set; }
		string editingValue;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			if(context != null && context.Instance != null && provider != null) {
				editingValue = objValue == null ? string.Empty : objValue.ToString();
				RemoteDocumentSource remoteDocumentSource = context.Container.Components.OfType<RemoteDocumentSource>().FirstOrDefault();
				if(remoteDocumentSource != null)
					RemoteDocumentSource = remoteDocumentSource;
				else
					return objValue;
				configHelper = new AppConfigHelper(RemoteDocumentSource.Site);
				var launcher = new WizardLauncherService<RemoteDocumentSourceModel>(CreateModel());
				if(launcher.Start(provider, configHelper) == DialogResult.OK) {
					ProcessModel(launcher.WizardModel);
				}
			}
			return editingValue;
		}
		static object GetPropertyValue(object component, string propertyName) {
			return TypeDescriptor.GetProperties(component).Find(propertyName, true).GetValue(component);
		}
		static void SetPropertyValue(object component, string propertyName, object value) {
			TypeDescriptor.GetProperties(component).Find(propertyName, true).SetValue(component, value);
		}
		RemoteDocumentSourceModel CreateModel() {
			var model = new RemoteDocumentSourceModel();
			model.ServiceUri = RemoteDocumentSource.ServiceUri;
			model.Endpoint = RemoteDocumentSource.EndpointConfigurationPrefix;
			model.AuthenticationType = RemoteDocumentSource.AuthenticationType;
			model.ReportName = editingValue;
			if(RemoteDocumentSource.AuthenticationType != AuthenticationType.None) {
				model.DocumentSourceType = RemoteDocumentSourceType.ReportServer;
				model.UserName = GetPropertyValue(RemoteDocumentSource, "UserName") as string ?? string.Empty;
				model.Password = GetPropertyValue(RemoteDocumentSource, "Password") as string ?? string.Empty;
				if(RemoteDocumentSource.ReportIdentity != null && RemoteDocumentSource.ReportIdentity is ReportIdentity)
					model.ReportId = ((ReportIdentity)RemoteDocumentSource.ReportIdentity).Id;
			} else {
				model.DocumentSourceType = RemoteDocumentSourceType.ReportService;
			}
			return model;
		}
		void ProcessModel(RemoteDocumentSourceModel model) {
			ReferenceHelper.AddReference(RemoteDocumentSource.Site, typeof(EndpointAddress).Assembly);
			ReferenceHelper.AddReference(RemoteDocumentSource.Site, typeof(DevExpress.DocumentServices.ServiceModel.Client.ReportServiceClientFactory).Assembly);
			ConfigureDocumentSource(model);
		}
		void ConfigureDocumentSource(RemoteDocumentSourceModel model) {
			SetPropertyValue(RemoteDocumentSource, "ServiceUri", model.ServiceUri);
			SetPropertyValue(RemoteDocumentSource, "AuthenticationType", model.AuthenticationType);
			if(model.AuthenticationType != AuthenticationType.None) {
				SetPropertyValue(RemoteDocumentSource, "UserName", model.UserName);
				SetPropertyValue(RemoteDocumentSource, "Password", model.Password);
				SetPropertyValue(RemoteDocumentSource, "ReportIdentity", new ReportIdentity(model.ReportId));
				if(model.GenerateEndpoints) {
					SetPropertyValue(RemoteDocumentSource, "EndpointConfigurationPrefix", model.Endpoint);
					configHelper.ConfigureEndpoints(model.Endpoint);
				} else {
					SetPropertyValue(RemoteDocumentSource, "EndpointConfigurationPrefix", null);
				}
			} else {
				SetPropertyValue(RemoteDocumentSource, "ReportName", model.ReportName);
			}
			editingValue = model.ReportName;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
}
