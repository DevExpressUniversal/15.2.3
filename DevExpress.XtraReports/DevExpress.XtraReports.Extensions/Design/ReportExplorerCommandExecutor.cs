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

using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraReports.Design {
	public class FormattingComponentCommandExecutor : CommandExecutorBase {
		readonly LockService lockService;
		XtraReport Report { get { return (XtraReport)designerHost.RootComponent; } }
		LockService LockService { get { return lockService; } }
		public FormattingComponentCommandExecutor(IDesignerHost designerHost)
			: base(designerHost) {
			lockService = LockService.GetInstance(designerHost);
		}
		public override void ExecCommand(CommandID cmdID) {
			if(designerHost.IsDebugging())
				return;
			if(cmdID == FormattingComponentCommands.AddStyle)
				AddStyle();
			else if(cmdID == FormattingComponentCommands.EditStyles)
				EditCollection(XRComponentPropertyNames.StyleSheet);
			else if(cmdID == FormattingComponentCommands.PurgeStyles)
				PurgeStyles();
			else if(cmdID == FormattingComponentCommands.ClearStyles)
				ClearStyles();
			else if(cmdID == FormattingComponentCommands.SelectControlsWithStyle)
				SelectControlsWithStyle();
			else if(cmdID == FormattingComponentCommands.AddFormattingRule)
				AddFormattingRule();
			else if(cmdID == FormattingComponentCommands.EditFormattingRules)
				EditCollection(XRComponentPropertyNames.FormattingRuleSheet);
			else if(cmdID == FormattingComponentCommands.PurgeFormattingRules)
				PurgeFormattingRules();
			else if(cmdID == FormattingComponentCommands.ClearFormattingRules)
				ClearFormattingRules();
			else if(cmdID == FormattingComponentCommands.SelectControlsWithFormattingRule)
				SelectControlsWithRule();
			else {
				if(this.parameters == null || this.parameters.Length < 2)
					return;
				XRControl primaryControl = parameters[0] as XRControl;
				FormattingDataObject[] data = parameters[1] as FormattingDataObject[];
				if(primaryControl == null || data == null)
					return;
				IDesigner designer = designerHost.GetDesigner(primaryControl);
				if(designer is XRControlDesignerBase) {
					if(cmdID.Equals(FormattingComponentCommands.AssignStyleToXRControl))
						((XRControlDesignerBase)designer).SetStyle("Style", (XRControlStyle)data[0].FormattingComponent);
					else if(cmdID.Equals(FormattingComponentCommands.AssignRuleToXRControl))
						((XRControlDesignerBase)designer).AddRule((FormattingRule)data[0].FormattingComponent);
				} else if(designer is ReportDesigner && cmdID.Equals(FormattingComponentCommands.AssignRuleToXRControl)) {
					AssignRuleToReport(primaryControl, (FormattingRule)data[0].FormattingComponent);
				}
			}
		}
		void AddStyle() {
			XRControlStyle style = new XRControlStyle();
			string description = String.Format(DesignSR.Trans_Add, typeof(XRControlStyle).Name);
			DesignerTransaction transaction = designerHost.CreateTransaction(description);
			try {
				PropertyDescriptor property = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(Report, XRComponentPropertyNames.StyleSheet);
				changeServ.OnComponentChanging(Report, property);
				DesignToolHelper.AddToContainer(designerHost, style);
				Report.StyleSheet.Add(style);
				changeServ.OnComponentChanged(Report, property, null, null);
			} finally {
				transaction.Commit();
			}
			selectionServ.SetSelectedComponents(new object[] { style });
		}
		void PurgeStyles() {
			ClearStyleCollection(false, Report.GetUnusedStyles().Where(x => LockService.CanDeleteComponent(x)).ToList());
		}
		void ClearStyles() {
			ClearStyleCollection(true, Report.StyleSheet.OfType<XRControlStyle>().Where(x => LockService.CanDeleteComponent(x)).ToList());
		}
		void ClearStyleCollection(bool showMessage, List<XRControlStyle> styles) {
			if(styles.Count == 0 ||
			showMessage ? XtraMessageBox.Show(DesignLookAndFeelHelper.GetLookAndFeel(designerHost),
				ReportLocalizer.GetString(ReportStringId.Msg_WarningRemoveStyles),
				ReportLocalizer.GetString(ReportStringId.UD_ReportDesigner),
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No : false)
				return;
			string description = String.Format(DesignSR.Trans_ChangeProp, XRComponentPropertyNames.StyleSheet);
			DesignerTransaction transaction = designerHost.CreateTransaction(description);
			try {
				PropertyDescriptor property = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(Report, XRComponentPropertyNames.StyleSheet);
				changeServ.OnComponentChanging(Report, property);
				foreach(XRControlStyle style in styles) {
					DesignToolHelper.RemoveFromContainer(designerHost, style);
				}
				changeServ.OnComponentChanged(Report, property, null, null);
			} finally {
				transaction.Commit();
			}
			selectionServ.SetSelectedComponents(new object[] { Report });
		}
		void AddFormattingRule() {
			FormattingRule formattingRule = new FormattingRule();
			string description = String.Format(DesignSR.Trans_Add, typeof(FormattingRule).Name);
			DesignerTransaction transaction = designerHost.CreateTransaction(description);
			try {
				PropertyDescriptor property = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(Report, XRComponentPropertyNames.FormattingRuleSheet);
				changeServ.OnComponentChanging(Report, property);
				DesignToolHelper.AddToContainer(designerHost, formattingRule);
				Report.FormattingRuleSheet.Add(formattingRule);
				changeServ.OnComponentChanged(Report, property, null, null);
			} finally {
				transaction.Commit();
			}
			selectionServ.SetSelectedComponents(new object[] { formattingRule });
		}
		void PurgeFormattingRules() {
			ClearFormattingRuleCollection(false, Report.GetUnusedRules().Where(x => LockService.CanDeleteComponent(x)).ToList());
		}
		void ClearFormattingRules() {
			ClearFormattingRuleCollection(true, Report.FormattingRuleSheet.Where(x => LockService.CanDeleteComponent(x)).ToList());
		}
		void ClearFormattingRuleCollection(bool showMessage, List<FormattingRule> formattingRules) {
			bool allowCustomLookAndFeel = XtraMessageBox.AllowCustomLookAndFeel;
			XtraMessageBox.AllowCustomLookAndFeel = true;
			try {
				if(formattingRules.Count == 0 ||
					showMessage ? XtraMessageBox.Show(DesignLookAndFeelHelper.GetLookAndFeel(designerHost),
					ReportLocalizer.GetString(ReportStringId.Msg_WarningRemoveFormattingRules),
				   ReportLocalizer.GetString(ReportStringId.UD_ReportDesigner),
				   MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No : false)
					return;
			} finally {
				XtraMessageBox.AllowCustomLookAndFeel = allowCustomLookAndFeel;
			}
			string description = String.Format(DesignSR.Trans_ChangeProp, XRComponentPropertyNames.FormattingRuleSheet);
			DesignerTransaction transaction = designerHost.CreateTransaction(description);
			try {
				PropertyDescriptor property = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(Report, XRComponentPropertyNames.FormattingRuleSheet);
				changeServ.OnComponentChanging(Report, property);
				foreach(FormattingRule formattingRule in formattingRules) {
					DesignToolHelper.RemoveFromContainer(designerHost, formattingRule);
				}
				changeServ.OnComponentChanged(Report, property, null, null);
			} finally {
				transaction.Commit();
			}
			selectionServ.SetSelectedComponents(new object[] { Report });
		}
		void EditCollection(string collectionName) {
			string description = String.Format(DesignSR.Trans_ChangeProp, collectionName);
			DesignerTransaction transaction = designerHost.CreateTransaction(description);
			try {
				PropertyDescriptor property = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(Report, collectionName);
				changeServ.OnComponentChanging(Report, property);
				EditorContextHelper.EditValue((ComponentDesigner)designerHost.GetDesigner(Report), Report, collectionName);
				changeServ.OnComponentChanged(Report, property, null, null);
			} finally {
				transaction.Commit();
			}
		}
		void SelectControlsWithStyle() {
			XRControlStyle style = selectionServ.PrimarySelection as XRControlStyle;
			List<XRControl> selectedControls = GetControlsByPredicate(x => x.StyleName == style.Name || x.OddStyleName == style.Name || x.EvenStyleName == style.Name);
			if(selectedControls.Count > 0)
				selectionServ.SetSelectedComponents(selectedControls);
		}
		void SelectControlsWithRule() {
			FormattingRule rule = selectionServ.PrimarySelection as FormattingRule;
			List<XRControl> selectedControls = GetControlsByPredicate(x => x.FormattingRules.Contains(rule));
			if(selectedControls.Count > 0)
				selectionServ.SetSelectedComponents(selectedControls);
		}
		List<XRControl> GetControlsByPredicate(Func<XRControl, bool> predicate) {
			return Report.AllControls<XRControl>().Where(predicate).ToList();
		}
		void AssignRuleToReport(XRControl report, FormattingRule rule) {
			if(report == null || report.FormattingRules.Contains(rule))
				return;
			DesignerTransaction transaction = designerHost.CreateTransaction(string.Format(DesignSR.TransFmt_ChangeFormattingRules, report.Name));
			try {
				RaiseComponentChanging(report, XRComponentPropertyNames.FormattingRules);
				report.FormattingRules.Add(rule);
				RaiseComponentChanged(report);
			} catch {
				transaction.Cancel();
			} finally {
				transaction.Commit();
			}
		}
	}
}
