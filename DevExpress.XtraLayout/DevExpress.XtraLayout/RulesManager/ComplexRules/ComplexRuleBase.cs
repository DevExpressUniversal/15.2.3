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

using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.ComponentModel;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public partial class ComplexRuleBase : XtraUserControl, IControlRuleBase {
		public ComplexRuleBase() {
			InitializeComponent();
			InitializeSubmenu();
			InitLocalizationText();
		}
		void InitLocalizationText() {
			lciComplexRule.Text = Localizer.Active.GetLocalizedString(StringId.ManageRuleComplexRuleBaseFormatStyle);
		}
		void InitializeSubmenu() {
			cmbComplexRule.Properties.Items.AddRange(new object[] {
				new RuleTypeDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleColorScale2),
					RuleType = RuleType.ColorScale2
				},
				new RuleTypeDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleColorScale3),
					RuleType = RuleType.ColorScale3
				},
				new RuleTypeDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBar),
					RuleType = RuleType.DataBar
				},
				new RuleTypeDisplayObject() {
					Description = Localizer.Active.GetLocalizedString(StringId.ManageRuleIconSets),
					RuleType = RuleType.IconSet
				}
			});
		}
		public PanelControl PnlFormatSetting {
			get { return pnlFormatSetting; }
		}
		void cmbComplexRule_SelectedIndexChanged(object sender, EventArgs e) {
			if(ParentForm == null) return;
			var parent = ((NewRuleForm)ParentForm);
			parent.FormatRule.RuleType = ((RuleTypeDisplayObject)cmbComplexRule.SelectedItem).RuleType;
			parent.SetControls();
		}
	}
}
