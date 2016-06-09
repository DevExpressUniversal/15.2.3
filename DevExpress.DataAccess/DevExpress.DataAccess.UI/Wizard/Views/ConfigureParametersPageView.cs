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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	[ToolboxItem(false)]
	public class ConfigureParametersPageView : ParametersPageViewBase, IConfigureParametersPageView {
		public ConfigureParametersPageView(IServiceProvider propertyGridServices, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider) 
			: base(propertyGridServices, parameterService, repositoryItemsProvider) {
			InitializeComponent();
		}
		ConfigureParametersPageView() : this(null, null, DefaultRepositoryItemsProvider.Instance) { }
		#region IConfigureParametersPageView Members
		void IConfigureParametersPageView.Initialize(IEnumerable<IParameter> parameters, bool previewDataRowLimit,
			Func<object> getPreviewDataFunc, bool fixedParameters) 
		{
			Initialize(parameters, previewDataRowLimit, getPreviewDataFunc, fixedParameters);
		}
		public virtual bool ConfirmQueryExecution() {
			return
				XtraMessageBox.Show(this.LookAndFeel, FindForm(),
					DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardConfirmExecutionMessage),
					FindForm().Text, MessageBoxButtons.OKCancel) ==
				DialogResult.OK;
		}
		public virtual void ShowDuplicatingColumnNameError(string columnName) {
			XtraMessageBox.Show(LookAndFeel, FindForm(),
				string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardDuplicatingColumnNameMessage), columnName),
				DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxErrorTitle), 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		#endregion
		#region IWizardPageView Members
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageConfigureParameters); }
		}
		#endregion
		private void InitializeComponent() {
			((ISupportInitialize)(this.layoutControlBase)).BeginInit();
			this.layoutControlBase.SuspendLayout();
			((ISupportInitialize)(this.layoutGroupBase)).BeginInit();
			((ISupportInitialize)(this.layoutItemFinishButton)).BeginInit();
			((ISupportInitialize)(this.layoutItemNextButton)).BeginInit();
			((ISupportInitialize)(this.layoutItemHeaderLabel)).BeginInit();
			((ISupportInitialize)(this.layoutItemSeparatorTop)).BeginInit();
			((ISupportInitialize)(this.layoutItemPreviousButton)).BeginInit();
			((ISupportInitialize)(this.panelBaseContent)).BeginInit();
			((ISupportInitialize)(this.layoutItemBaseContentPanel)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new Rectangle(1913, 156, 1091, 739);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.AutoScaleDimensions = new SizeF(6F, 13F);
			this.Name = "ConfigureParametersPageView";
			((ISupportInitialize)(this.layoutControlBase)).EndInit();
			this.layoutControlBase.ResumeLayout(false);
			((ISupportInitialize)(this.layoutGroupBase)).EndInit();
			((ISupportInitialize)(this.layoutItemFinishButton)).EndInit();
			((ISupportInitialize)(this.layoutItemNextButton)).EndInit();
			((ISupportInitialize)(this.layoutItemHeaderLabel)).EndInit();
			((ISupportInitialize)(this.layoutItemSeparatorTop)).EndInit();
			((ISupportInitialize)(this.layoutItemPreviousButton)).EndInit();
			((ISupportInitialize)(this.panelBaseContent)).EndInit();
			((ISupportInitialize)(this.layoutItemBaseContentPanel)).EndInit();
			this.ResumeLayout(false);
		}
	}
}
