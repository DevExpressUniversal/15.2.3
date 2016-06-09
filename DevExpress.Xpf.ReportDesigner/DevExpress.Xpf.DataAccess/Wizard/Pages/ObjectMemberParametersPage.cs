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
using System.Linq;
using DevExpress.Data;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public class ObjectMemberParametersPage : DataSourceWizardPage, IObjectMemberParametersPageView {
		public static ObjectMemberParametersPage Create(DataSourceWizardModelBase model, HierarchyCollection<IParameter, IParameterService> reportParameters) {
			return ViewModelSource.Create(() => new ObjectMemberParametersPage(model, reportParameters));
		}
		protected ObjectMemberParametersPage(DataSourceWizardModelBase model, HierarchyCollection<IParameter, IParameterService> reportParameters)
			: base(model) {
			this.reportParameters = reportParameters;
		}
		public IEnumerable<ParameterViewModel> Parameters { get; private set; }
		readonly ObservableCollection<IParameter> reportParameters;
		public ObservableCollection<IParameter> ReportParameters { get { return reportParameters; } }
		IEnumerable<IParameter> IObjectMemberParametersPageView.GetParameters() {
			return Parameters;
		}
		void IObjectMemberParametersPageView.Initialize(IEnumerable<IParameter> parameters) {
			Parameters = parameters.Select(ParameterViewModel.Create).ToList().AsReadOnly();
		}
	}
}
