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
using System.ComponentModel.Design;
using DevExpress.DashboardCommon;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Native;
using DevExpress.Utils.UI;
namespace DevExpress.DashboardWin.Native {
	public class DashboardParameterCollectionEditor : ParameterCollectionEditor {		
		ParameterChangesCollection parametersChanged = null;
		public DashboardParameterCollectionEditor(Type type)
			: base(type) {
		}
		protected override DesignerTransaction BeginTransaction() {
			DesignerTransaction transaction = base.BeginTransaction();
			Dashboard dashboard = Context.Instance as Dashboard;
			parametersChanged = new ParameterChangesCollection(dashboard.Parameters);
			return transaction;
		}
		protected override void EndTransaction(DesignerTransaction transaction, bool isChanged) {
			base.EndTransaction(transaction, isChanged);
			DashboardDesigner designer = Context.GetService<SelectedContextService>().Designer;
			Dashboard dashboard = Context.Instance as Dashboard;
			parametersChanged.ApplyChanges(dashboard.Parameters);
			DashboardParametersHistoryItem item = new DashboardParametersHistoryItem(dashboard.Parameters, parametersChanged);
			if(isChanged) {
				if(!item.IsEmpty)
					designer.History.RedoAndAdd(item);
			} else {
				item.ApplyParametersState(designer, false);
			}
		}
		protected override object CreateInstance(Type itemType) {
			DashboardParameter param = base.CreateInstance(itemType) as DashboardParameter;
			param.Name = ParameterDescriptionProvider.GenerateName(Items);
			return param;
		}
		protected override CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			CollectionEditorFormBase form =  base.CreateCollectionForm(serviceProvider);
			form.SortProperties = false;
			return form;
		}
	}
}
