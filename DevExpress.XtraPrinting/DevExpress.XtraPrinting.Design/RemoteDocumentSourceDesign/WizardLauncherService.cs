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
using System.ComponentModel.Design;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Utils;
using DevExpress.Data.WizardFramework;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Presenters;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign {
	public class WizardLauncherService<TModel> where TModel : RemoteDocumentSourceModel, new() {
		readonly RemoteDocumentSourceModel editingModel;
		RemoteDocumentSourceWizardView wizardView;
		RemoteDocumentSourceWizardPageFactory pageFactory;
		RemoteDocumentSourceWizard wizard;
		TModel fakeModel = null;
		protected Type StartPage { get { return typeof(ChooseDocumentSourceTypePresenter<IChooseDocumentSourceTypeView>); } }
		public RemoteDocumentSourceModel WizardModel {
			get {
				if(wizard != null) return wizard.GetResultModel();
				else return fakeModel;
			}
		}
		protected IWizardView WizardView { get { return wizardView; } }
		protected RemoteDocumentSourceWizardPageFactory PageFactory { get { return pageFactory; } }
		public WizardLauncherService(TModel editingModel) {
			this.editingModel = editingModel;
		}
		public DialogResult Start(IServiceProvider provider, IAppConfigHelper configHelper) {
			var windowsFormsEditorService = provider.GetService<IWindowsFormsEditorService>();
			var typeDiscoveryService = provider.GetService<ITypeDiscoveryService>();
			var reportTypesProvider = new ReportTypesProvider(typeDiscoveryService);
			wizardView = CreateWizardView(provider);
			pageFactory = CreatePageFactory(reportTypesProvider, configHelper);
			SubscribeWCFServerCertificateValidation();
			wizard = CreateWizard();
			wizard.SetStartPage(StartPage);
			wizard.Cancelled += OnWizard_Cancelled;
			wizard.Completed += OnWizard_Completed;
			return windowsFormsEditorService != null
				? windowsFormsEditorService.ShowDialog(wizardView)
				: wizardView.ShowDialog();
		}
		protected RemoteDocumentSourceWizard CreateWizard() {
			return new RemoteDocumentSourceWizard(WizardView, editingModel ?? new RemoteDocumentSourceModel(), PageFactory);
		}
		RemoteDocumentSourceWizardView CreateWizardView(IServiceProvider provider) {
			var view = new RemoteDocumentSourceWizardView();
			ILookAndFeelService lookAndFeelService = provider.GetService<ILookAndFeelService>();
			if(lookAndFeelService != null) {
				lookAndFeelService.InitializeRootLookAndFeel(view.LookAndFeel);
			}
			return view;
		}
		protected virtual RemoteDocumentSourceWizardPageFactory CreatePageFactory(ReportTypesProvider reportTypesProvider, IAppConfigHelper configHelper) {
			return new RemoteDocumentSourceWizardPageFactory(reportTypesProvider, configHelper);
		}
		protected virtual void OnWizard_Cancelled(object sender, EventArgs e) {
			wizard.Cancelled -= OnWizard_Cancelled;
			wizardView.DialogResult = DialogResult.Cancel;
			UnsubscribeWCFServerCertificateValidation();
			CloseWindow();
		}
		protected virtual void OnWizard_Completed(object sender, EventArgs e) {
			wizard.Completed -= OnWizard_Completed;
			wizardView.DialogResult = DialogResult.OK;
			UnsubscribeWCFServerCertificateValidation();
			CloseWindow();
		}
		protected void CloseWindow() {
			if(wizardView != null)
				wizardView.Close();
		}
		void SubscribeWCFServerCertificateValidation() {
			ServicePointManager.ServerCertificateValidationCallback += OnServerCertificateValidationCallback;
		}
		void UnsubscribeWCFServerCertificateValidation() {
			ServicePointManager.ServerCertificateValidationCallback -= OnServerCertificateValidationCallback;
		}
		bool OnServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
			return true;
		}
	}
}
