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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using DevExpress.Data;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.DataAccess.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.DataAccess.UI.Localization;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public class ConfigureSqlParametersPage : DataSourceWizardPage, IConfigureParametersPageView {
		public static ConfigureSqlParametersPage Create(DataSourceWizardModelBase model, HierarchyCollection<IParameter, IParameterService> reportParameters) {
			return ViewModelSource.Create(() => new ConfigureSqlParametersPage(model, reportParameters));
		}
		protected ConfigureSqlParametersPage(DataSourceWizardModelBase model, HierarchyCollection<IParameter, IParameterService> reportParameters)
			: base(model) {
			this.parameters = new ObservableCollection<ParameterViewModel>();
			this.nameGenerator = new PrefixNameGenerator("Parameter", 1);
			this.containsName = name => parameters.Any(x => x.Name == name);
			this.reportParameters = reportParameters;
		}
		bool IConfigureParametersPageView.ConfirmQueryExecution() {
			MessageResult result = MessageResult.None;
			model.Parameters.DoWithMessageBoxService(x => {
				result = x.ShowMessage(DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardConfirmExecutionMessage), DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardTitleODS), MessageButton.OKCancel);
			});
			return result == MessageResult.OK;
		}
		IEnumerable<IParameter> IConfigureParametersPageView.GetParameters() {
			return parameters;
		}
		readonly ObservableCollection<ParameterViewModel> parameters;
		readonly PrefixNameGenerator nameGenerator;
		readonly Predicate<string> containsName;
		Func<object> getPreviewDataFunc;
		public IEnumerable<IParameter> Parameters { get { return parameters; } }
		public bool FixedParameters { get; protected set; }
		public ParameterViewModel SelectedParameter { get; set; }
		readonly ObservableCollection<IParameter> reportParameters;
		public ObservableCollection<IParameter> ReportParameters { get { return reportParameters; } }
		void IConfigureParametersPageView.Initialize(IEnumerable<IParameter> parameters, bool previewDataRowLimit, Func<object> getPreviewDataFunc, bool fixedParameters) {
			foreach(var parameter in parameters)
				this.parameters.Add(ParameterViewModel.Create(parameter));
			SelectedParameter = this.parameters.FirstOrDefault();
			FixedParameters = fixedParameters;
			this.getPreviewDataFunc = getPreviewDataFunc;
		}
		void IConfigureParametersPageView.ShowDuplicatingColumnNameError(string columnName) {
			model.Parameters.DoWithMessageBoxService(x => x.ShowMessage(string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardDuplicatingColumnNameMessage), columnName), DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxErrorTitle), MessageButton.OK, MessageIcon.Error));
		}
		public void Preview() {
			if(getPreviewDataFunc == null)
				return;
			object dataPreview = getPreviewDataFunc();
			if(dataPreview == null)
				return;
			model.Parameters.DoWithPreviewDialogService(dialog => dialog.ShowDialog(MessageButton.OK, DataAccessUILocalizer.GetString(DataAccessUIStringId.DataPreviewForm_Title), dataPreview));
		}
		public void Add() {
			parameters.Add(ParameterViewModel.Create(new Parameter() { Name = nameGenerator.GenerateNameFromStart(containsName), Type = typeof(Int32), Value = ParametersHelper.GetDefaultValue(typeof(Int32)) }));
		}
		public void Remove() {
			parameters.Remove(SelectedParameter);
		}
	}
}
