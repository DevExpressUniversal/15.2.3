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
using System.Text;
using DevExpress.XtraReports.UI;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraReports.Localization;
using System.ComponentModel.Design;
namespace DevExpress.XtraReports.Design {
	public class FormattingRuleSheetEditorForm : StyleSheetEditorForm {
		static string noRuleSelected;
		static string moreThanOneRule;
		static string ruleNamePreviewPostfix;
		public FormattingRuleSheet RuleSheet { get { return Report.FormattingRuleSheet; } }
		protected override bool VisibleSaveLoadButtons { get { return false; } }
		protected override string ReportPropertyName { get { return XRComponentPropertyNames.FormattingRuleSheet; } }
		public FormattingRuleSheetEditorForm(XtraReport report, IServiceProvider provider)
			: base(report, provider) {
		}
		protected override void InitializeStrings() {
			noRuleSelected = ReportLocalizer.GetString(ReportStringId.FRSForm_Msg_NoRuleSelected);
			moreThanOneRule = ReportLocalizer.GetString(ReportStringId.FRSForm_Msg_MoreThanOneRule);
			ruleNamePreviewPostfix = string.Empty;
			BtnCancel.Text = ReportLocalizer.GetString(ReportStringId.SSForm_Btn_Close);
			this.Text = ReportLocalizer.GetString(ReportStringId.FRSForm_Caption);
			BtAdd.ToolTip = ReportLocalizer.GetString(ReportStringId.FRSForm_TTip_AddRule);
			BtRemove.ToolTip = ReportLocalizer.GetString(ReportStringId.FRSForm_TTip_RemoveRule);
			BtClear.ToolTip = ReportLocalizer.GetString(ReportStringId.FRSForm_TTip_ClearRules);
			BtPurge.ToolTip = ReportLocalizer.GetString(ReportStringId.FRSForm_TTip_PurgeRules);
		}
		protected override void SetItemsListBoxSelectionOnInitialize() {
			SetItemsListBoxSelection(GetSelectedItemIndex(typeof(FormattingRule), x => ((FormattingRule)x).Name));
		}
		protected override string[] GetItemsNames(XtraReport report) {
			return RuleSheet.GetNames();
		}
		protected override bool IsAttachedObject(object item) {
			return Report.IsAttachedRule((FormattingRule)item);
		}
		protected override void PaintPreview(System.Windows.Forms.PaintEventArgs e) {
			Graphics gr = e.Graphics;
			string text = String.Empty;
			FormattingRule rule = null;
			if(LboxItems.SelectedItems.Count == 0)
				text = noRuleSelected;
			else if(LboxItems.SelectedItems.Count > 1)
				text = moreThanOneRule;
			else {
				text = (string)LboxItems.SelectedItem;
				rule = RuleSheet[text];
				text += ruleNamePreviewPostfix;
			}
			DrawPreview(gr, PnlPreview.ClientRectangle, text, (XRControlStyle)(rule != null? rule.Formatting: null));
		}
		protected override string AddItem() {
			FormattingRule rule = new FormattingRule();
			RuleSheet.Add(rule);
			DesignerHost.Container.Add(rule);
			return rule.Name;
		}
		protected override List<string> GetSheetList() {
			return new List<string>(RuleSheet.Count);
		}
		protected override void DestroyItem(string name) {
			DesignerHost.DestroyComponent(RuleSheet[name]);
		}
		protected override void ClearItems() {
			FormattingRule[] rules = new FormattingRule[RuleSheet.Count];
			RuleSheet.CopyTo(rules, 0);
			foreach(FormattingRule rule in rules) {
				DesignerHost.DestroyComponent(rule);
			}
		}
		protected override object GetObjectByName(string name) {
			return RuleSheet[name];
		}
		protected override ISite GetObjectSite(object item) {
			if(item is FormattingRule)
				return ((FormattingRule)item).Site;
			return null;
		}
		protected override string GetComponentName(object component) {
			FormattingRule rule = component as FormattingRule;
			return rule == null ? string.Empty : rule.Name;
		}
	}
}
