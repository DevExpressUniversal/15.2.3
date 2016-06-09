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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Utils;
using System.ComponentModel.Design;
using DevExpress.Data.Utils;
using System.ComponentModel;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public abstract class CalculatedFieldHistoryItem : HistoryItem {
		readonly CalculatedField field;
		readonly IDashboardDataSource dataSource;
		public override string Caption { get { return string.Format(DashboardWinLocalizer.GetString(CaptionId), field.Name); } }
		protected CalculatedFieldCollection Collection { get { return dataSource != null ? dataSource.CalculatedFields : null; } }
		protected abstract DashboardWinStringId CaptionId { get; }
		protected CalculatedField Field { get { return field; } }
		protected CalculatedFieldHistoryItem(CalculatedField field, IDashboardDataSource dataSource) {
			Guard.ArgumentNotNull(field, "field");
			this.field = field;
			this.dataSource = dataSource;
		}
		public override void Redo() {
			IComponentChangeService changeService = null;
			PropertyDescriptor collectionProperty = null;
			DesignerTransaction transaction = null;
			if (dataSource != null) {
				Dashboard dashboard = dataSource.GetDataSourceInternal().Dashboard;
				IDesignerHost designerHost = null;
				if (dashboard != null)
					designerHost = dashboard.GetService<IDesignerHost>();				
				if (designerHost != null)
					changeService = designerHost.GetService<IComponentChangeService>();
				if (changeService != null) {
					transaction = designerHost.CreateTransaction(Caption);
					collectionProperty = Helper.GetProperty(dataSource, "CalculatedFields");					
					changeService.OnComponentChanging(dataSource, collectionProperty);
				}
			}
			try {
				base.Redo();
				if (changeService != null) {
					changeService.OnComponentChanged(dataSource, collectionProperty, null, null);
					transaction.Commit();
				}
			}
			catch {
				if (transaction != null)
					transaction.Cancel();
			}
		}
	}
}
