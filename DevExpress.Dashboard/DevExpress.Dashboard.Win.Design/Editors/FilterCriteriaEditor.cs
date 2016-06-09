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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Design {
	public class DashboardItemFilterCriteriaEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			SelectedContextService selectedContextService = context.GetService(typeof(SelectedContextService)) as SelectedContextService;
			DataDashboardItem dashboardItem = context.Instance as DataDashboardItem;
			if(dashboardItem != null) {
				IComponentChangeService changeService = (IComponentChangeService)provider.GetService(typeof(IComponentChangeService));
				PropertyDescriptor criteriaDescriptor = TypeDescriptor.GetProperties(dashboardItem)["FilterString"];
				changeService.OnComponentChanging(dashboardItem, criteriaDescriptor);
				selectedContextService.Designer.ShowFilterForm(dashboardItem);
				changeService.OnComponentChanged(dashboardItem, criteriaDescriptor, value, dashboardItem.FilterString);
				return dashboardItem.FilterString;
			}
			else {
				return value;
			}
		}
	}
	public class DataSourceFilterCriteriaEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			SelectedContextService selectedContextService = context.GetService(typeof(SelectedContextService)) as SelectedContextService;
			IDashboardDataSource dataSource = context.Instance as IDashboardDataSource;
			if(dataSource != null) {
				IComponentChangeService changeService = (IComponentChangeService)provider.GetService(typeof(IComponentChangeService));
				PropertyDescriptor criteriaDescriptor = TypeDescriptor.GetProperties(dataSource)["Filter"];
				changeService.OnComponentChanging(dataSource, criteriaDescriptor);
				selectedContextService.Designer.ShowFilterForm(dataSource);
				changeService.OnComponentChanged(dataSource, criteriaDescriptor, value, dataSource.Filter);
				return dataSource.Filter;
			}
			else {
				return value;
			}
		}
	}
}
