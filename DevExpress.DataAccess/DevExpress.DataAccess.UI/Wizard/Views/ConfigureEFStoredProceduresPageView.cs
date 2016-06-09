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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Native;
using DevExpress.DataAccess.UI.Native.EntityFramework;
using DevExpress.DataAccess.UI.Native.ParametersGrid;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	[ToolboxItem(false)]
	public partial class ConfigureEFStoredProceduresPageView : WizardViewBase, IConfigureEFStoredProceduresPageView {
		readonly IServiceProvider propertyGridServices;
		readonly IParameterService parameterService;
		readonly IRepositoryItemsProvider repositoryItemsProvider;
		StoredProcedureViewInfo selectedItem;
		Func<object> getPreviewDataFunc;
		public ConfigureEFStoredProceduresPageView(IServiceProvider propertyGridServices, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider, IWizardRunnerContext context) {
			InitializeComponent();
			LocalizeComponent();
			parametersGrid.BorderVisible = true;
			this.propertyGridServices = propertyGridServices;
			this.parameterService = parameterService;
			this.repositoryItemsProvider = repositoryItemsProvider;
			UpdateParametersGrid();
			parametersGrid.SetButtons(null, null, buttonPreview);
		}
		ConfigureEFStoredProceduresPageView() : this(null, null, DefaultRepositoryItemsProvider.Instance, null) { }
		#region IWizardPageView Members
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageConfigureStoredProcedures); }
		}
		#endregion
		#region Implementation of IConfigureEFStoredProceduresPageView
		public StoredProcedureViewInfo SelectedItem {
			get { return selectedItem; }
			protected set {
				if(selectedItem == value)
					return;
				ApplyParameters();
				selectedItem = value;
				if(listBoxProcedures.SelectedItem != value)
					listBoxProcedures.SelectedItem = value;
				UpdateControls();
			}
		}
		public void Initialize(IEnumerable<StoredProcedureViewInfo> procedures, Func<object> getPreviewDataFunc) {
			listBoxProcedures.Items.Clear();
			AddToList(procedures);
			this.getPreviewDataFunc = getPreviewDataFunc;
		}
		public void AddToList(IEnumerable<StoredProcedureViewInfo> procedures) {
			Guard.ArgumentNotNull(procedures, "procedures");
			object[] listItems = procedures as StoredProcedureViewInfo[] ?? procedures.Cast<object>().ToArray();
			listBoxProcedures.Items.AddRange(listItems);
			if(SelectedItem == null)
				SelectedItem = (StoredProcedureViewInfo)listItems.FirstOrDefault();
		}
		public void RemoveFromList(StoredProcedureViewInfo procedure) { listBoxProcedures.Items.Remove(procedure); }
		public IEnumerable<StoredProcedureViewInfo> ChooseProceduresToAdd(IEnumerable<StoredProcedureViewInfo> available) {
			IServiceContainer serviceProvider = new ServiceContainer();
			if(parameterService != null)
				serviceProvider.AddService(typeof(IParameterService), parameterService);
			serviceProvider.AddService(typeof(UserLookAndFeel), LookAndFeel);
			ChooseEFStoredProcedureForm formChooseProceduresToAdd = new ChooseEFStoredProcedureForm(serviceProvider);
			formChooseProceduresToAdd.Initialize(available);
			return formChooseProceduresToAdd.ShowDialog() == DialogResult.OK
				? formChooseProceduresToAdd.SelectedStoredProcedures
				: Enumerable.Empty<StoredProcedureViewInfo>();
		}
		public void SetAddEnabled(bool value) { buttonAdd.Enabled = value; }
		public event EventHandler AddClick;
		public event EventHandler RemoveClick;
		#endregion
		protected void RaiseAddClick() {
			if(AddClick != null)
				AddClick(this, EventArgs.Empty);
		}
		protected void RaiseRemoveClick() {
			if(RemoveClick != null)
				RemoveClick(this, EventArgs.Empty);
		}
		protected void ApplyParameters() {
			if(SelectedItem == null)
				return;
			SelectedItem.StoredProcedure.Parameters.Clear();
			SelectedItem.StoredProcedure.Parameters.AddRange(parametersGrid.Parameters.Select(EFParameter.FromIParameter));
		}
		protected void UpdateControls() {
			UpdateParametersGrid();
			bool en = SelectedItem != null;
			parametersGrid.Enabled = en;
			buttonRemove.Enabled = en;
			buttonPreview.Enabled = en;
		}
		void UpdateParametersGrid() {
			var itemsProvider = SelectedItem == null ? Enumerable.Empty<IParameter>() : SelectedItem.StoredProcedure.Parameters;
			var parametersGridModel = new ParametersGridModel(itemsProvider);
			var parametersGridViewModel = new ParametersGridViewModel(parametersGridModel, parameterService, propertyGridServices, getPreviewDataFunc, true, false);
			parametersGrid.Initialize(parametersGridViewModel, propertyGridServices, parameterService, repositoryItemsProvider);
		}
		void LocalizeComponent() {
			buttonAdd.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Add);
			buttonRemove.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Remove);
			buttonPreview.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Preview);
		}
		void buttonAdd_Click(object sender, EventArgs e) { RaiseAddClick(); }
		void buttonRemove_Click(object sender, EventArgs e) { RaiseRemoveClick(); }
		void listBoxProcedures_SelectedValueChanged(object sender, EventArgs e) {
			SelectedItem = (StoredProcedureViewInfo)listBoxProcedures.SelectedItem;
		}
		private void buttonPreview_Click(object sender, EventArgs e) {
			ApplyParameters();
		}
	}
}
