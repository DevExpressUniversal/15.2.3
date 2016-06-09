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

using System.Drawing.Design;
using System.ComponentModel;
using System;
using System.Windows.Forms.Design;
using DevExpress.XtraPrinting.Native.WinControls;
using DevExpress.LookAndFeel;
using System.ComponentModel.Design;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.XtraPrinting.Design {
	public class PdfPasswordSecurityOptionsEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			PdfPasswordSecurityOptions options = value as PdfPasswordSecurityOptions;
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if(edSvc != null && options != null) {
				PdfPasswordSecurityEditorForm form = new PdfPasswordSecurityEditorForm();
				form.Init(options);
				LookAndFeelProviderHelper.SetParentLookAndFeel(form, provider);
				if(edSvc.ShowDialog(form) == System.Windows.Forms.DialogResult.OK && !form.PdfPasswordSecurityOptions.Equals(options)) {
					IComponentChangeService componentChangeService = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
					if(componentChangeService != null && context.Instance != null && context.PropertyDescriptor != null)
						componentChangeService.OnComponentChanging(context.Instance, context.PropertyDescriptor);
					options.Assign(form.PdfPasswordSecurityOptions);
					if(componentChangeService != null && context.Instance != null && context.PropertyDescriptor != null)
						componentChangeService.OnComponentChanged(context.Instance, context.PropertyDescriptor, null, null);
				}
				form.Dispose();
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
}
