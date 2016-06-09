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
using System.ComponentModel.Design;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraTreeList;
using DevExpress.Data.Browsing;
using System.Globalization;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.XtraReports.Design.BindingMapper {
	public partial class BindingMapperForm : ReportsEditorFormBase {
		static class FieldNames { 
			public const string IsValid = "IsValid";
			public const string Destination = "Destination";
		}
		readonly static CriteriaOperator isValidOperator = new BinaryOperator(FieldNames.IsValid, false);
		readonly CheckBoxColumnBehavior behavior = new CheckBoxColumnBehavior();
		bool isCheckBoxColumnBehaviorAttached;
		public bool ShowOnlyInvalidBindings { get { return treeList1.ActiveFilterEnabled; } }
		public BindingMapperForm() {
			InitializeComponent();
		}
		internal BindingMapperForm(XtraReport report, IServiceProvider serviceProvider, IList<BindingMapInfo> bindingMapInfos) : base(serviceProvider) {
			InitializeComponent();
			behavior.Attach(treeList1, btOK, checkColumn);
			isCheckBoxColumnBehaviorAttached = true;
			treeList1.Columns[FieldNames.Destination].ColumnEdit = new RepositoryItemDropDownEdit(report, serviceProvider, CreatePicker, CheckFocusedRow);
			treeList1.DataSource = bindingMapInfos;
			treeList1.ActiveFilterCriteria = isValidOperator;
			treeList1.ActiveFilterEnabled = false;
			treeList1.Enabled = bindingMapInfos.Count > 0;
		}
		public void SetLookAndFeel(IServiceProvider serviceProvider) {
			DesignLookAndFeelHelper.SetParentLookAndFeel(treeList1, serviceProvider);
		}
		DesignTreeListBindingPicker CreatePicker(DesignBindingInfo bindingInfo, IServiceProvider serviceProvider, PopupContainerEdit edit) {
			IDataSourceCollectionProvider dataSourceCollectionProvider = (IDataSourceCollectionProvider)serviceProvider.GetService(typeof(IDataSourceCollectionProvider));
			if(dataSourceCollectionProvider == null)
				return null;
			DesignTreeListBindingPicker picker = bindingInfo.CreatePicker();
			picker.Start(dataSourceCollectionProvider.GetDataSourceCollection(null), new PopupEditorService(edit, serviceProvider), bindingInfo.DesignBinding);
			return picker;
		}
		void CheckFocusedRow() {
			treeList1.FocusedNode.SetValue(0, true);
			behavior.OnRowChecked();
		}
		private void treeList1_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e) {
			if(e.Column != null && e.Column.Name == "sourceColumn") {
				bool isValid = (bool)e.Node.GetValue(2);
				if(!isValid) {
					e.EditViewInfo.ErrorIconText = ReportLocalizer.GetString(ReportStringId.BindingMapperForm_InvalidBindingWarning);
					e.EditViewInfo.ShowErrorIcon = true;
					e.EditViewInfo.FillBackground = true;
					e.EditViewInfo.ErrorIcon = DXErrorProvider.GetErrorIconInternal(ErrorType.Warning);
					e.EditViewInfo.CalcViewInfo(e.Graphics);
				}
			}
		}
		private void treeList1_CustomFilterDisplayText(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e) {
			CriteriaOperator criteriaOperator = e.Value as CriteriaOperator;
			if(object.ReferenceEquals(criteriaOperator, isValidOperator)) {
				e.Value = ReportLocalizer.GetString(ReportStringId.BindingMapperForm_ShowOnlyInvalidBindings);
				e.Handled = true;
			}
		}
	}
}
