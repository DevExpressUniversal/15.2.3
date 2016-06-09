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
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraCharts;
using System.IO;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using System.Collections;
namespace DevExpress.ExpressApp.Chart.Win {
	public class ChartSettingsEditor : UITypeEditor {
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value) {
			ChartControl chartControl = new ChartControl();
			if(!string.IsNullOrEmpty((string)value)) {
				MemoryStream loadStream = new MemoryStream(Encoding.UTF8.GetBytes((string)value));
				chartControl.LoadFromStream(loadStream);
			}
			if(context != null && context.Instance != null) {
				IModelListView modelListView = ((ModelNode)context.Instance).Parent as IModelListView;
				if(modelListView != null && modelListView.ModelClass != null) {
					chartControl.DataSource = DevExpress.ExpressApp.Editors.CriteriaPropertyEditorHelper.CreateFilterControlDataSource(modelListView.ModelClass.TypeInfo.Type, null);
				}
			}
			if(WinChartControlContainer.ChartDesignerType == ChartDesignerType.ChartWizard) {
				ChartWizard wizard = new ChartWizard(chartControl);
				wizard.ShowDialog();
			}
			else {
				XtraCharts.Designer.ChartDesigner designer = new XtraCharts.Designer.ChartDesigner(chartControl);
				designer.ShowDialog();
			}
			MemoryStream saveStream = new MemoryStream();
			chartControl.SaveToStream(saveStream);
			return Encoding.UTF8.GetString(saveStream.ToArray());
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
}
