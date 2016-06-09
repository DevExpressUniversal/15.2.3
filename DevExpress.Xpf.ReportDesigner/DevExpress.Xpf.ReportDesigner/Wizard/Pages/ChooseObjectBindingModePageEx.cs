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

using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.DataAccess.DataSourceWizard;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages {
	[POCOViewModel]
	public class ChooseObjectBindingModePageEx : ChooseObjectBindingModePage {
		public static new ChooseObjectBindingModePage Create(DataSourceWizardModelBase model) {
			return ViewModelSource.Create(() => new ChooseObjectBindingModePageEx(model));
		}
		bool canChooseColumns = false;
		protected ChooseObjectBindingModePageEx(DataSourceWizardModelBase model) : base(model) { }
		public override bool CanFinish {
			get { return false; }
			protected set { canChooseColumns = value; }
		}
		public override bool CanGoForward {
			get { return true; }
			protected set { }
		}
		protected override void NavigateToNextPage(WizardController wizardController) {
			var reportWizardModel = ((ReportDataSourceWizardModel)model).ReportWizardModel;
			if (canChooseColumns)
				wizardController.NavigateTo(SelectColumnsPage.Create(reportWizardModel));
			else
				base.NavigateToNextPage(wizardController);
		}
	}
}
