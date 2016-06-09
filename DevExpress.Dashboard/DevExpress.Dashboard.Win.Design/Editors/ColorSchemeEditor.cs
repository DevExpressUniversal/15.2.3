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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.Data.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Design {
	public abstract class ColorSchemeEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			try {
				DashboardDesigner designer = provider.GetService<SelectedContextService>().Designer;
				EditColorSchemeFormController formController = GetColorSchemeFormController(context, provider);
				using(EditColorSchemeForm form = new EditColorSchemeForm(formController, null)) {
					VSLookAndFeelService.SetControlLookAndFeel(provider, form, false);
					form.Text = DashboardWinLocalizer.GetString(FormCaptionId);
					form.ShowDialog(designer.FindForm());
				}
				return value;
			}
			catch {
				return value;
			}
		}
		protected abstract EditColorSchemeFormController GetColorSchemeFormController(ITypeDescriptorContext context, IServiceProvider provider);
		protected abstract DashboardWinStringId FormCaptionId { get; }
	}
	public class DashboardColorSchemeEditor : ColorSchemeEditor {
		protected override EditColorSchemeFormController GetColorSchemeFormController(ITypeDescriptorContext context, IServiceProvider provider) {
			DashboardDesigner designer = provider.GetService<SelectedContextService>().Designer;
			return EditColorSchemeFormController.CreateGlobal(designer, designer.Dashboard);
		}
		protected override DashboardWinStringId FormCaptionId { get { return DashboardWinStringId.GlobalColorSchemeEditFormCaption; } }
	}
	public class DashboardItemColorSchemeEditor : ColorSchemeEditor {
		protected override EditColorSchemeFormController GetColorSchemeFormController(ITypeDescriptorContext context, IServiceProvider provider) {
			DashboardDesigner designer = provider.GetService<SelectedContextService>().Designer;
			DataDashboardItem dashboardItem = context.Instance as DataDashboardItem;
			if(dashboardItem != null)
				return EditColorSchemeFormController.CreateLocal(designer, designer.Dashboard, dashboardItem);
			return null;
		}
		protected override DashboardWinStringId FormCaptionId { get { return DashboardWinStringId.LocalColorSchemeEditFormCaption; } }
	}
}
