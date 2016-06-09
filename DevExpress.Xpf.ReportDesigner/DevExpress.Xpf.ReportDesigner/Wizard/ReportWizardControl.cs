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
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.DataAccess.DataSourceWizard;
using DevExpress.Xpf.DataAccess.Native;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages;
using DevExpress.XtraReports.UI;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Xpf.Reports.UserDesigner.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard {
	public class ReportWizardControl : DataSourceWizardControlBase {
		public static readonly DependencyProperty ReportProperty;
		ReportWizardModel wizardViewModel;
		public XtraReport Report {
			get { return (XtraReport)GetValue(ReportProperty); }
			set { SetValue(ReportProperty, value); }
		}
		ReportWizardModel WizardViewModel {
			get { return wizardViewModel ?? (wizardViewModel = CreateModel()); }
		}
		static ReportWizardControl() {
			DependencyPropertyRegistrator<ReportWizardControl>.New()
				.OverrideMetadata(PageTemplateSelectorProperty, new ReportWizardPageTemplateSelector())
				.Register(x => x.Report, out ReportProperty, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);
		}
		public ReportWizardControl() {
			this.SetDefaultStyleKey(typeof(ReportWizardControl));
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			var wizard = (Wizard)GetTemplateChild(PART_Wizard);
			wizard.Do(x => {
				WizardViewModel.ReportWizardCompleted += OnCompleted;
				x.StartPageViewModel = ChooseReportTypePage.Create(WizardViewModel);
			});
		}
		void OnCompleted(object sender, ReportWizardCompletedEventArgs e) {
			((ReportWizardModel)sender).ReportWizardCompleted -= OnCompleted;
			SetCurrentValue(ReportProperty, e.Result);
		}
		ReportWizardModel CreateModel() {
			return ReportWizardModel.Create(new WizardModelParameters(
				DoWithMessageBoxService, 
				DoWithSplashScreenService, 
				DoWithOpenFileDialogService, 
				DoWithQueryBuilderDialogService, 
				DoWithPreviewDialogService, 
				DoWithPasswordDialogService,
				DoWithChooseEFStoredProceduresDialogService), EnableCustomSql);
		}
	}
	public sealed class WizardModelParameters {
		public WizardModelParameters(
			Action<Action<IMessageBoxService>> doWithMessageBoxServiceCallback,
			Action<Action<ISplashScreenService>> doWithSplashScreenServiceCallback,
			Action<Action<IOpenFileDialogService>> doWithOpenFileDialogServiceCallback,
			Action<Action<IDialogService>> doWithQueryBuilderDialogServiceCallback,
			Action<Action<IDialogService>> doWithPreviewDialogServiceCallback,
			Action<Action<IDialogService>> doWithPasswordDialogServiceCallback,
			Action<Action<IDialogService>> doWithChooseEFStoredProceduresCallback) {
			DoWithMessageBoxService = doWithMessageBoxServiceCallback;
			DoWithSplashScreenService = doWithSplashScreenServiceCallback;
			DoWithOpenFileDialogService = doWithOpenFileDialogServiceCallback;
			DoWithQueryBuilderDialogService = doWithQueryBuilderDialogServiceCallback;
			DoWithPreviewDialogService = doWithPreviewDialogServiceCallback;
			DoWithPasswordDialogService = doWithPasswordDialogServiceCallback;
			DoWithChooseEFStoredProceduresDialogService = doWithChooseEFStoredProceduresCallback;
		}
		public readonly Action<Action<IMessageBoxService>> DoWithMessageBoxService;
		public readonly Action<Action<ISplashScreenService>> DoWithSplashScreenService;
		public readonly Action<Action<IOpenFileDialogService>> DoWithOpenFileDialogService;
		public readonly Action<Action<IDialogService>> DoWithQueryBuilderDialogService;
		public readonly Action<Action<IDialogService>> DoWithPreviewDialogService;
		public readonly Action<Action<IDialogService>> DoWithPasswordDialogService;
		public readonly Action<Action<IDialogService>> DoWithChooseEFStoredProceduresDialogService;
	}
}
