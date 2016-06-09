#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Skins;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.DashboardWin.DataSourceWizard {
	public partial class DashboardChooseDataSourceTypePageView : WizardViewBase, IChooseDataSourceTypePageView {
		readonly IWizardRunnerContext context;
		readonly Dictionary<DataSourceType, ListBoxItem> itemsRepository;
		readonly DataSourceTypes dataSourceTypes;
		public DashboardChooseDataSourceTypePageView() : this(null, null, null)  {
		}
		public DashboardChooseDataSourceTypePageView(IWizardRunnerContext context, ISupportedDataSourceTypesService dataSourceTypesService, DataSourceTypes dataSourceTypes) {
			this.context = context;
			this.dataSourceTypes = dataSourceTypes;
			InitializeComponent();			
			AddDataSourceTypes();
			ConfigureDescription();			
			itemsRepository = new Dictionary<DataSourceType, ListBoxItem>();
			foreach(ListBoxItem item in dataSourceTypesListBox.Items) {
				itemsRepository[(DataSourceType)item.Value] = item;
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			dataSourceTypesListBox.LookAndFeel.SetSkinStyle(this.LookAndFeel.ActiveSkinName);
		}
		#region IWizardPageView Members
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseDSType); }
		}
		#endregion
		public DataSourceType DataSourceType {
			get {
				ListBoxItem selectedItem = dataSourceTypesListBox.SelectedItem as ListBoxItem;
				return selectedItem != null ? (DataSourceType)selectedItem.Value : DataSourceType.Xpo;
			}
			set {
				ListBoxItem listBoxItem;
				if(itemsRepository.TryGetValue(value, out listBoxItem))
					dataSourceTypesListBox.SelectedItem = listBoxItem;				
			}
		}
		void AddDataSourceTypes() {
			dataSourceTypesListBox.BeginUpdate();
			if(dataSourceTypes.Contains(DataSourceType.Xpo)) {
				dataSourceTypesListBox.Items.Add(new CheckedListBoxItem {
					Value = DataSourceType.Xpo,
					Description = DashboardWinLocalizer.GetString(DashboardWinStringId.DataSourceTypeSql),
					Tag = DashboardWinLocalizer.GetString(DashboardWinStringId.DataSourceTypeSqlDescription)
				});
			}
			if(dataSourceTypes.Contains(DashboardDataSourceType.Olap)) {
				dataSourceTypesListBox.Items.Add(new CheckedListBoxItem {
					Value = DashboardDataSourceType.Olap,
					Description = DashboardWinLocalizer.GetString(DashboardWinStringId.DataSourceTypeOlap),
					Tag = DashboardWinLocalizer.GetString(DashboardWinStringId.DataSourceTypeOlapDescription)
				});
			}
				if(dataSourceTypes.Contains(DataSourceType.Entity)) {
					dataSourceTypesListBox.Items.Add(new CheckedListBoxItem {
						Value = DataSourceType.Entity,
						Description = DashboardWinLocalizer.GetString(DashboardWinStringId.DataSourceTypeEF),
						Tag = DashboardWinLocalizer.GetString(DashboardWinStringId.DataSourceTypeEFDescription)
					});
				}
				if(dataSourceTypes.Contains(DataSourceType.Object)) {
					dataSourceTypesListBox.Items.Add(new CheckedListBoxItem {
						Value = DataSourceType.Object,
						Description = DashboardWinLocalizer.GetString(DashboardWinStringId.DataSourceTypeObject),
						Tag = DashboardWinLocalizer.GetString(DashboardWinStringId.DataSourceTypeObjectDescription)
					});
				}
			if(dataSourceTypes.Contains(DashboardDataSourceType.XmlSchema)) {
				dataSourceTypesListBox.Items.Add(new CheckedListBoxItem {
					Value = DashboardDataSourceType.XmlSchema,
					Description = DashboardWinLocalizer.GetString(DashboardWinStringId.DataSourceTypeXmlSchema),
					Tag = DashboardWinLocalizer.GetString(DashboardWinStringId.DataSourceTypeXmlSchemaDescription)
				});
			}
			if(dataSourceTypes.Contains(DataSourceType.Excel)) {
				dataSourceTypesListBox.Items.Add(new CheckedListBoxItem {
					Value = DataSourceType.Excel,
					Description = DashboardWinLocalizer.GetString(DashboardWinStringId.DataSourceTypeExcel),
					Tag = DashboardWinLocalizer.GetString(DashboardWinStringId.DataSourceTypeExcelDescription)
				});
			}
			dataSourceTypesListBox.EndUpdate();
		}
		void ConfigureDescription() {
			description.ForeColor = CommonColors.GetSystemColor(CommonColors.DisabledText);
		}
		void DataSourceTypesListBoxSelectedIndexChanged(object sender, EventArgs e) {			
			description.Text = ((CheckedListBoxItem)dataSourceTypesListBox.SelectedItem).Tag.ToString();
		}
		void DataSourceTypesListBoxMouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e) {
			MoveForward();
		}
	}
}
