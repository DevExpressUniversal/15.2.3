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

using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.DataAccess.DataSourceWizard;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Data;
using DevExpress.DataAccess.Wizard.Services;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages {
	public class ConfigureSqlParametersPageEx : ConfigureSqlParametersPage {
		public static new ConfigureSqlParametersPageEx Create(DataSourceWizardModelBase model, HierarchyCollection<IParameter, IParameterService> reportParameters) {
			return ViewModelSource.Create(() => new ConfigureSqlParametersPageEx(model, reportParameters));
		}
		protected ConfigureSqlParametersPageEx(DataSourceWizardModelBase model, HierarchyCollection<IParameter, IParameterService> reportParameters) : base(model, reportParameters) { }
		public override bool CanFinish {
			get { return false; }
			protected set { canGoForward = value; }
		}
		bool canGoForward = false;
		public override bool CanGoForward {
			get { return canGoForward; }
			protected set { }
		}
		protected override void NavigateToNextPage(WizardController wizardController) {
			ReportWizardModel reportWizardModel = ((ReportDataSourceWizardModel)model).ReportWizardModel;
			wizardController.NavigateTo(SelectColumnsPage.Create(reportWizardModel));
		}
	}
}
