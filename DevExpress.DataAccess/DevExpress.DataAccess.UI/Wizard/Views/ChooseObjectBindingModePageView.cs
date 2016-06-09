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
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	public partial class ChooseObjectBindingModePageView :  WizardViewBase, IChooseObjectBindingModePageView {
		public ChooseObjectBindingModePageView() {
			InitializeComponent();
			LocalizeComponent();
		}
		#region Overrides of WizardViewBase
		public override string HeaderDescription { get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectBindingMode); } }
		#endregion
		#region Implementation of IChooseObjectBindingModePageView
		public bool SchemaOnly {
			get { return radioButtonSchemaOnly.Checked; }
			set {
				if(value == SchemaOnly)
					return;
				CheckEdit edit = value ? radioButtonSchemaOnly : radioButtonActualData;
				edit.Focus();
				edit.Checked = true;
				RaiseChanged();
			}
		}
		public event EventHandler Changed;
		#endregion
		protected void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		void LocalizeComponent() {
			radioButtonSchemaOnly.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectBindingMode_RetrieveSchema);
			radioButtonActualData.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectBindingMode_RetrieveData);
			labelSchemaOnly.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectBindingMode_RetrieveSchemaDescription);
			labelActualData.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectBindingMode_RetrieveDataDescription);
		}
		private void radioSchemaOnly_CheckedChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		private void radioActualData_CheckedChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		private void labelSchemaOnly_Click(object sender, EventArgs e) { radioButtonSchemaOnly.Checked = true; }
		private void labelActualData_Click(object sender, EventArgs e) { radioButtonActualData.Checked = true; }
		private void radioSchemaOnly_TabStopChanged(object sender, EventArgs e) {
			if(!radioButtonSchemaOnly.TabStop)
				radioButtonSchemaOnly.TabStop = true;
		}
		private void radioActualData_TabStopChanged(object sender, EventArgs e) {
			if(!radioButtonActualData.TabStop)
				radioButtonActualData.TabStop = true;
		}
	}
}
