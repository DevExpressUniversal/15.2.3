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
using System.Linq;
using DevExpress.Data;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.DataAccess.UI.Localization;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public class ConfigureEFStoredProceduresPage : DataSourceWizardPage, IConfigureEFStoredProceduresPageView {
		public static ConfigureEFStoredProceduresPage Create(DataSourceWizardModelBase model, HierarchyCollection<IParameter, IParameterService> reportParameters) {
			return ViewModelSource.Create(() => new ConfigureEFStoredProceduresPage(model, reportParameters));
		}
		protected ConfigureEFStoredProceduresPage(DataSourceWizardModelBase model, HierarchyCollection<IParameter, IParameterService> reportParameters)
			: base(model) {
			this.procedures = new ObservableCollection<StoredProcedureViewInfo>();
			this.reportParameters = reportParameters;
			AddIsEnabled = true;
		}
		void IConfigureEFStoredProceduresPageView.Initialize(IEnumerable<StoredProcedureViewInfo> procedures, Func<object> getPreviewDataFunc) {
			this.procedures.Clear();
			Parameters = Enumerable.Empty<IParameter>();
			foreach(var procedure in procedures)
				this.procedures.Add(procedure);
			this.getPreviewDataFunc = getPreviewDataFunc;
		}
		void IConfigureEFStoredProceduresPageView.AddToList(IEnumerable<StoredProcedureViewInfo> procedures) {
			foreach(var procedure in procedures)
				this.procedures.Add(procedure);
		}
		IEnumerable<StoredProcedureViewInfo> IConfigureEFStoredProceduresPageView.ChooseProceduresToAdd(IEnumerable<StoredProcedureViewInfo> available) {
			var viewModel = new ChooseEFStoredProceduresViewModel(available);
			IEnumerable<StoredProcedureViewInfo> procedures = Enumerable.Empty<StoredProcedureViewInfo>();
			model.Parameters.DoWithChooseEFStoredProceduresDialogService(dialog => {
				if(dialog.ShowDialog(MessageButton.OKCancel, DataAccessUILocalizer.GetString(DataAccessUIStringId.ChooseEFStoredProceduresDialog), viewModel) == MessageResult.OK)
					procedures = viewModel.EditValue == null ? Enumerable.Empty<StoredProcedureViewInfo>() : viewModel.EditValue.Cast<StoredProcedureViewInfo>();
				else
					procedures = Enumerable.Empty<StoredProcedureViewInfo>();
			});
			return procedures;
		}
		Func<object> getPreviewDataFunc;
		public virtual StoredProcedureViewInfo SelectedItem { get; set; }
		protected void OnSelectedItemChanged() {
			if(SelectedItem == null)
				return;
			Parameters = SelectedItem.StoredProcedure.Parameters.Select(ParameterViewModel.Create);
		}
		readonly ObservableCollection<StoredProcedureViewInfo> procedures;
		public IEnumerable<StoredProcedureViewInfo> Procedures { get { return procedures; } }
		public virtual bool AddIsEnabled { get; protected set; }
		public virtual IEnumerable<IParameter> Parameters { get; protected set; }
		readonly ObservableCollection<IParameter> reportParameters;
		public ObservableCollection<IParameter> ReportParameters { get { return reportParameters; } }
		EventHandler removeClick;
		event EventHandler IConfigureEFStoredProceduresPageView.RemoveClick {
			add { removeClick += value; }
			remove { removeClick -= value; }
		}
		EventHandler addClick;
		event EventHandler IConfigureEFStoredProceduresPageView.AddClick {
			add { addClick += value; }
			remove { addClick -= value; }
		}
		void IConfigureEFStoredProceduresPageView.RemoveFromList(StoredProcedureViewInfo procedure) {
			procedures.Remove(procedure);
		}
		void IConfigureEFStoredProceduresPageView.SetAddEnabled(bool value) {
			AddIsEnabled = value;
		}
		public void Add() {
			addClick.Do(x => x(this, EventArgs.Empty));
		}
		public void Remove() {
			removeClick.Do(x => x(this, EventArgs.Empty));
		}
		public void Preview() {
			model.Parameters.DoWithPreviewDialogService(dialog => dialog.ShowDialog(MessageButton.OK, DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Preview), getPreviewDataFunc()));
		}
	}
}
