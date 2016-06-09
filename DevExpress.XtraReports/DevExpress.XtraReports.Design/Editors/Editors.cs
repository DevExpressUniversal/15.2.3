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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraBars;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraPrinting.Design;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.XtraReports.Design {
	public class ReportNameEditor : TypePickEditor {
		protected override object CreateValue(IServiceProvider provider, string typeName) {
			return String.Compare(typeName, DesignSR.DataGridNoneString, true) == 0 ? "" :
				typeName;
		}
		protected override TypePickerBase CreateTypePicker(IServiceProvider provider) {
			return new WebReportSourcePicker();
		}
	}
	public class ScriptSecurityPermissionEditor : UITypeEditor {
		protected DialogResult ShowDialog(Form form, IServiceProvider provider) {
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			return edSvc == null ? form.ShowDialog() : edSvc.ShowDialog(form);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			if (provider != null) {
				try {
					ScriptSecurityPermissionsEditorForm editorForm = new ScriptSecurityPermissionsEditorForm(provider);
					DesignLookAndFeelHelper.SetParentLookAndFeel(editorForm, provider);
					try {
						editorForm.EditValue = (ScriptSecurityPermissionCollection)objValue;
						ShowDialog(editorForm, provider);
					} 
					finally {
						editorForm.Dispose();
					}
				} 
				catch {
				}
			}
			return objValue;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return (context != null && context.Instance != null) ?
				UITypeEditorEditStyle.Modal : base.GetEditStyle(context);
		}
	}
}
