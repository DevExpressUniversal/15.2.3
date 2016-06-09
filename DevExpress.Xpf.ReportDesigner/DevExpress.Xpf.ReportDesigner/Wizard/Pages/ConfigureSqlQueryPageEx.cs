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
using System.ComponentModel;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages {
	public class ConfigureSqlQueryPageEx : ConfigureQueryPage {
		public static new ConfigureSqlQueryPageEx Create(DataSourceWizardModelBase model) {
			return ViewModelSource.Create(() => new ConfigureSqlQueryPageEx(model));
		}
		protected ConfigureSqlQueryPageEx(DataSourceWizardModelBase model) : base(model) { }
		protected bool IsSchemaReady {
			get; private set;
		}
		public override bool CanFinish {
			get { return false; }
			protected set {
				IsSchemaReady = value;
			}
		}
		bool baseGoForward = false;
		public override bool CanGoForward {
			get { return IsSchemaReady || baseGoForward; }
			protected set { baseGoForward = value; }
		}
		protected override void OnGoForward(CancelEventArgs e) {
			if(IsSchemaReady)
				OnFinish(e);
		}
		protected override void NavigateToNextPage(WizardController wizardController) {
			if (IsSchemaReady)
				wizardController.NavigateTo(SelectColumnsPage.Create(((ReportDataSourceWizardModel)model).ReportWizardModel));
			else
				base.NavigateToNextPage(wizardController);
		}
	}
}
