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

using System.Collections.Generic;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.XtraSpreadsheet.Forms {
	public class ManageDataSourcesViewModel : ManageDataSourceViewModelBase {
		#region Properties
		public int SelectedItemIndex { get; set; }
		public bool CanRemove { get { return DataSources.Count > 0; } }
		internal List<DataComponentInfo> DataInfos { get; set; }
		public bool WasChanges { get; private set; }
		internal List<SpreadsheetParameter> OldParameters { get; private set; }
		internal int NewDefault { get; private set; }
		#endregion
		public ManageDataSourcesViewModel(ISpreadsheetControl control) : base(control) {
			DataComponentInfoList infos = DocumentModel.DataComponentInfos;
			DataInfos = new List<DataComponentInfo>();
			DataInfos.AddRange(infos);
			OldParameters = new List<SpreadsheetParameter>();
			OldParameters.AddRange(DocumentModel.MailMergeParameters.InnerList);
			WasChanges = false;
			NewDefault = DocumentModel.DataComponentInfos.DefaultIndex;
		}
		public void PerformRemove(int index) {
			DataSources.RemoveAt(index);
			DataInfos.RemoveAt(index);
			WasChanges = true;
			if(index == NewDefault)
				NewDefault = -1;
			if(index < NewDefault)
				NewDefault--;
			OnPropertyChanged("DataSources");
			OnPropertyChanged("CanRemove");
		}
		public void PerformAdd() {
			SpreadsheetControl.ShowAddDataSourceForm(AddDataSourceCallBack);
		}
		void AddDataSourceCallBack(object dataSource) {
			IDataComponent dataComponent = dataSource as IDataComponent;
			if(dataComponent == null)
				return;
			DataComponentInfo newInfo = DataComponentInfo.CreateFromIDataComponent(dataComponent);
			DataInfos.Add(newInfo);
			string newText = PopulateDataSource(newInfo);
			DataSources.Add(newText);
			WasChanges = true;
			OnPropertyChanged("DataSources");
			OnPropertyChanged("CanRemove");
		}
		public void PerformEditDataSource(IDataComponent dataSource, int index) {
			DataInfos[index] = DataComponentInfo.CreateFromIDataComponent(dataSource);
			string newText = PopulateDataSource(DataInfos[index]);
			DataSources[index] = newText;
			WasChanges = true;
			OnPropertyChanged("DataSources");
		}
		public IDataComponent GetDataComponent(int index) {
			return DataInfos[index].TryToLoadDataComponentFromXml();
		}
		public void ApplyChanges() {
			MailMergeManageDataSourcesCommand command = new MailMergeManageDataSourcesCommand(SpreadsheetControl);
			command.ApplyChanges(this);
		}
	}
}
