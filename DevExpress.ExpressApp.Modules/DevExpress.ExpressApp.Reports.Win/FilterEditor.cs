#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraFilterEditor;
namespace DevExpress.ExpressApp.Reports.Win {
	public class FilterEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			ReportFiltering reportFiltering = context.Instance as ReportFiltering;
			XafReport report = reportFiltering.Report;
			XafApplication application = reportFiltering.Application;
			if(report.DataType != null) {
				using(PopupForm form = new PopupForm()) {
					form.AutoShrink = false;
					form.ClientSize = new Size(500, 320);
					form.StartPosition = FormStartPosition.CenterScreen;
					SimpleAction okAction = new SimpleAction();
					okAction.ActionMeaning = ActionMeaning.Accept;
					okAction.Caption = CaptionHelper.GetLocalizedText("DialogButtons", "OK");
					form.ButtonsContainer.Register(okAction);
					SimpleAction cancelAction = new SimpleAction();
					cancelAction.ActionMeaning = ActionMeaning.Cancel;
					cancelAction.Caption = CaptionHelper.GetLocalizedText("DialogButtons", "Cancel");
					form.ButtonsContainer.Register(cancelAction);
					FilterEditorControl filterEditorControl = new FilterEditorControl();
					new FilterEditorControlHelper(application, report.ObjectSpace).Attach(filterEditorControl);
					filterEditorControl.SourceControl = report.DataSource;
					filterEditorControl.FilterString = (string)value;
					form.AddControl(filterEditorControl, CaptionHelper.GetLocalizedText("Captions", "FilterBuilder"));
					if(form.ShowDialog() == DialogResult.OK) {
						value = filterEditorControl.FilterString;
					}
				}
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
}
