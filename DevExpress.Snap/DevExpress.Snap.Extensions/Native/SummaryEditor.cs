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
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using DevExpress.Snap.Core.Native;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Fields;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Data;
using DevExpress.Snap.Core.Commands;
namespace DevExpress.Snap.Extensions.Native {
	public class SummaryEditor : UITypeEditor {
		[System.Security.SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		public SummaryEditor() { }
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			if (provider != null) {
				try {
					IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
					if (edSvc != null) {
						SnapFieldInfo fieldInfo = (SnapFieldInfo)provider.GetService(typeof(SnapFieldInfo));
						IParsedInfoProvider parsedInfoProvider = (IParsedInfoProvider)provider.GetService(typeof(IParsedInfoProvider));
						SNTextField textField = (SNTextField)parsedInfoProvider.GetParsedInfo();
						DesignBinding designBinding = FieldsHelper.GetFieldDesignBinding(fieldInfo.PieceTable.DocumentModel.DataSourceDispatcher, fieldInfo);
						SNSummaryEditorForm form = new SNSummaryEditorForm(designBinding, textField.SummaryRunning, textField.SummaryFunc, textField.FrameworkStringFormat, textField.SummariesIgnoreNullValues, provider);
						form.TopMost = true;
						if (edSvc.ShowDialog(form) == DialogResult.OK) {
							IFieldChanger fieldChanger = (IFieldChanger)provider.GetService(typeof(IFieldChanger));
							fieldChanger.ApplyNewValue((controller, newMode) => controller.SetArgument(0, newMode), DesignBindingHelper.GetDataMember(fieldInfo,  form.DesignBinding));
							if(form.Running == SummaryRunning.None)
								fieldChanger.ApplyNewValue((controller, newMode) => controller.RemoveSwitch(SNTextField.SummaryRunningSwitch), string.Empty);
							else
								fieldChanger.ApplyNewValue((controller, newMode) => controller.SetSwitch(SNTextField.SummaryRunningSwitch, newMode), Enum.GetName(typeof(SummaryRunning), form.Running));
							fieldChanger.ApplyNewValue((controller, newMode) => controller.SetSwitch(SNTextField.SummaryFuncSwitch, newMode), Enum.GetName(typeof(SummaryItemType), form.Func));
							fieldChanger.ApplyNewValue((controller, newMode) => controller.SetSwitch(SNTextField.SummariesIgnoreNullValuesSwitch, newMode), Convert.ToString(form.IgnoreNullValues));
							if (string.IsNullOrEmpty(form.FormatString))
								fieldChanger.ApplyNewValue((controller, newMode) => controller.RemoveSwitch(SNTextField.FrameworkStringFormatSwitch), string.Empty);
							else
								fieldChanger.ApplyNewValue((controller, newMode) => controller.SetSwitch(SNTextField.FrameworkStringFormatSwitch, newMode), form.FormatString);
						}
					}
				} catch { }
			}
			return objValue;
		}
		[System.Security.SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
}
