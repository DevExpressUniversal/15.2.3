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

#if SILVERLIGHT
extern alias Platform;
#endif
using System.Collections.Generic;
using DevExpress.Xpf.Core.Design;
using System.Windows;
#if SILVERLIGHT
using AssemblyInfo = Platform::AssemblyInfo;
using PivotGridControl = Platform.DevExpress.Xpf.PivotGrid.PivotGridControl;
using PivotGridField = Platform.DevExpress.Xpf.PivotGrid.PivotGridField;
using IPivotOLAPDataSource = Platform.DevExpress.XtraPivotGrid.Data.IPivotOLAPDataSource;
using PivotGridWpfData = Platform.DevExpress.Xpf.PivotGrid.Internal.PivotGridWpfData;
using PivotGridData = Platform.DevExpress.XtraPivotGrid.Data.PivotGridData;
using FieldArea = Platform.DevExpress.Xpf.PivotGrid.FieldArea;
using System;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Mvvm.Native;
#else
using DevExpress.Design.SmartTags;
using DevExpress.XtraPivotGrid.Data;
using Microsoft.Windows.Design.Model;
using System;
using DevExpress.Mvvm.Native;
#endif
namespace DevExpress.Xpf.PivotGrid.Design {
	class GenerateFieldsActionProvider : PivotGridControlActionProviderBase {
		public GenerateFieldsActionProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override string GetCommandText() {
			return SR.GenerateFieldsCommandText;
		}
		protected override void OnCommandExecute(object param) {
			if(DataControl == null || ModelItem == null)
				return;
			if(object.ReferenceEquals(DataControl.DataSource, null) && string.IsNullOrEmpty(DataControl.OlapConnectionString)) {
				MessageBox.Show(SR.CantPopulateFieldsMessage, SR.CantPopulateFieldsCaption);
				return;
			}
			if(GetPivotGridFields().Count > 0) {
				if(MessageBox.Show(SR.ShouldClearExistingFieldsMessage, SR.ShouldClearExistingFieldsCaption, MessageBoxButton.YesNo) == MessageBoxResult.No) {
					return;
				}
			}
			GetFieldList((f) => {
				PerformEditAction(SR.PopulateFieldsDescription, () => RetrieveFields(f));
			});
		}
		void GetFieldList(Action<PivotGridData> action) {
			if(action == null)
				return;
			if(DataControl.DataSource != null)
				action(PivotGridControl.GetData(DataControl));
			else {
				OlapFieldsPopulator.GetDataAsync(DataControl.OlapConnectionString, () => PivotGridControl.GetData(DataControl).PivotGrid.Dispatcher.CheckAccess())
					.WithFunc(t => t.Do(r => action(r)));
			}
		}
		void RetrieveFields(PivotGridData data) {
			string[] fields = data.GetFieldList();
			if(fields == null)
				return;
			GetPivotGridFields().Clear();
			foreach(string fieldName in fields) {
				ModelItem item = ModelFactory.CreateItem(ModelItem.Context, FieldType, CreateOptions.InitializeDefaults, null);
				FieldArea area = fieldName == null || !fieldName.StartsWith("[Measures].") ? FieldArea.FilterArea : FieldArea.DataArea;
				item.Properties["Area"].SetValue(area);
				item.Properties["FieldName"].SetValue(fieldName);
				string caption = data.GetHierarchyCaption(fieldName);
				if(!string.IsNullOrEmpty(caption) && caption != fieldName)
					item.Properties["Caption"].SetValue(caption);
				GetPivotGridFields().Add(item);
			}
		}
	}
}
