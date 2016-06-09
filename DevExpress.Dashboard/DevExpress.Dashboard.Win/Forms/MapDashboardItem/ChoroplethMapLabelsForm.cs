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
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Native {
	public partial class ChoroplethMapTitleForm : DashboardForm {
		readonly DashboardDesigner designer;
		readonly ChoroplethMapDashboardItem dashboardItem;
		readonly string currentShapeTitleAttributeName;
		readonly string currentTooltipAttributeName;
		ChoroplethMapShapeLabelsAttributeHistoryItem historyItem;
		public ChoroplethMapTitleForm() {
			InitializeComponent();
		}
		public ChoroplethMapTitleForm(DashboardDesigner designer, ChoroplethMapDashboardItem dashboardItem)
			: this() {
			this.designer = designer;
			this.dashboardItem = dashboardItem;
			this.currentShapeTitleAttributeName = dashboardItem.ShapeTitleAttributeName;
			this.currentTooltipAttributeName = dashboardItem.TooltipAttributeName;
			ResourceImageHelper.FillImageListFromResources(imageList, "Images.DataPickerImages.png", typeof(ResFinder));
			cbTitleAttribute.Initialize(designer, dashboardItem, imageList, dashboardItem.ShapeTitleAttributeName, true);
			cbTooltipAttribute.Initialize(designer, dashboardItem, imageList, dashboardItem.TooltipAttributeName ?? dashboardItem.AttributeName, false);
			ceUseBindingAttributeForTooltipText.Checked = dashboardItem.TooltipAttributeName == null;
			cbTitleAttribute.SelectedValueChanged += cbTitleAttribute_SelectedValueChanged;
			cbTooltipAttribute.SelectedValueChanged += cbTooltipAttribute_SelectedValueChanged;
		}
		void cbTitleAttribute_SelectedValueChanged(object sender, EventArgs e) {
			ActivateHistoryItem();
		}
		void cbTooltipAttribute_SelectedValueChanged(object sender, EventArgs e) {
			ActivateHistoryItem();
		}
		void ActivateHistoryItem() {
			string selectedAttribute = cbTitleAttribute.SelectedAttribute;
			string selectedToolTipAttribute = cbTooltipAttribute.Enabled? cbTooltipAttribute.SelectedAttribute : null;
			if(selectedToolTipAttribute == dashboardItem.AttributeName)
				selectedToolTipAttribute = null;
			historyItem = new ChoroplethMapShapeLabelsAttributeHistoryItem(dashboardItem, selectedAttribute, currentShapeTitleAttributeName, selectedToolTipAttribute, currentTooltipAttributeName);
			historyItem.Redo(designer);
		}
		void ceUseForDisplayText_CheckedChanged(object sender, EventArgs e) {
			cbTooltipAttribute.Enabled = !ceUseBindingAttributeForTooltipText.Checked;
			ActivateHistoryItem();
		}
		void btnOK_Click(object sender, EventArgs e) {
			if(historyItem != null)
				designer.History.Add(historyItem);
			DialogResult = DialogResult.OK;
		}
		void btnCancel_Click(object sender, EventArgs e) {
			if(historyItem != null)
				historyItem.Undo(designer);
		}
	}
}
