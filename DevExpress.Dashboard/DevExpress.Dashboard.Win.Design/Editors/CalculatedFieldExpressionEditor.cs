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
using DevExpress.DashboardWin.Native;
using DevExpress.Data.Utils;
using DevExpress.LookAndFeel.DesignService;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Design {
	public class CalculatedFieldExpressionEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			try {
				SelectedContextService selectedContextService = context.GetService(typeof(SelectedContextService)) as SelectedContextService;
				IDashboardDataSource dataSource = selectedContextService.GetContextDataSource();
				if (dataSource != null) {					
					DashboardDesigner designer = provider.GetService<SelectedContextService>().Designer;
					CalculatedField calculatedField = (CalculatedField)context.Instance;
					string dataMember = calculatedField.DataMember;
					DataSourceInfo dataSourceInfo = new DataSourceInfo(dataSource, dataMember);
					using (CalculatedFieldExpressionEditorForm form = new CalculatedFieldExpressionEditorForm(calculatedField, dataSourceInfo, designer.Dashboard.Parameters, designer.ServiceProvider)) {
						ILookAndFeelService lookAndFeelService = context.GetService<ILookAndFeelService>();
						if (lookAndFeelService != null)
							form.LookAndFeel.ParentLookAndFeel = lookAndFeelService.LookAndFeel;
						if (form.ShowDialog(designer.FindForm()) == DialogResult.OK) {
							value = form.Expression;
						}
					}
				}
				return value;
			}
			catch {
				return value;
			}
		}
	}
}
