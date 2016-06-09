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

using DevExpress.DataAccess;
using DevExpress.Utils;
using DevExpress.Office.History;
using DevExpress.DataAccess.Native;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.History {
	#region DataSourceHistoryItemBase
	public abstract class DataSourceHistoryItemBase : HistoryItem {
		readonly IDataComponent dataSource;
		protected DataSourceHistoryItemBase(SnapDocumentModel documentModel, IDataComponent dataSource)
			: base(documentModel.ActivePieceTable) {
			this.dataSource = dataSource;
		}
		protected new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		protected IDataComponent DataSource { get { return dataSource; } }
	}
	#endregion
	#region AddDataSourceHistoryItem
	public class AddDataSourceHistoryItem : DataSourceHistoryItemBase {
		public AddDataSourceHistoryItem(SnapDocumentModel documentModel, IDataComponent dataSource)
			: base(documentModel, dataSource) { }
		protected override void RedoCore() {
			DocumentModel.BeginUpdateDataSource();
			DocumentModel.DataSources.Add(new DataSourceInfo(DataSource.Name, DataSource));
			DocumentModel.EndUpdateDataSource();
		}
		protected override void UndoCore() {
			DocumentModel.BeginUpdateDataSource();
			try {
				DocumentModel.DataSources.Remove(DataSource.Name);
			}
			finally {
				DocumentModel.EndUpdateDataSource();
			}
		}
	}
	#endregion
	#region ChangeDataSourceHistoryItem
	public class ChangeDataSourceHistoryItem : DataSourceHistoryItemBase {
		object cachedDataSource;
		DataSourceInfo changingDataSourceInfo;
		public ChangeDataSourceHistoryItem(SnapDocumentModel documentModel, IDataComponent dataSource, DataSourceInfo changingDataSourceInfo)
			: base(documentModel, dataSource) {
				Guard.ArgumentNotNull(changingDataSourceInfo, "changingDataSourceInfo");
				this.cachedDataSource = changingDataSourceInfo.DataSource;
				this.changingDataSourceInfo = changingDataSourceInfo;
		}
		protected override void RedoCore() {
			changingDataSourceInfo.DataSource = DataSource;
		}
		protected override void UndoCore() {
			changingDataSourceInfo.DataSource = cachedDataSource;
		}
	}
	#endregion
}
