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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet {
	public class DataComponentInfo : IEquatable<DataComponentInfo> {
		#region Properties
		public XElement XmlContent { get; set; }
		#endregion
		public DataComponentInfo Clone() {
			DataComponentInfo result = new DataComponentInfo();
			result.CopyFrom(this);
			return result;
		}
		void CopyFrom(DataComponentInfo other) {
			this.XmlContent = other.XmlContent;
		}
		public static DataComponentInfo CreateFromIDataComponent(IDataComponent dataComponent) {
			Guard.ArgumentNotNull(dataComponent, "dataComponent");
			DataComponentInfo result = new DataComponentInfo();
			result.XmlContent = new DataComponentHelper().SaveToXml(dataComponent);
			return result;
		}
		public bool Equals(DataComponentInfo other) {
			return Equals(other.XmlContent.ToString(), this.XmlContent.ToString());
		}
		public IDataComponent TryToLoadDataComponentFromXml() {
			IDataComponent result;
			try {
				result = new DataComponentHelper().LoadFromXml(XmlContent);
			}
			catch {
				return null;
			}
			return result;
		}
	}
	internal class DataComponentInfoList : IEnumerable<DataComponentInfo> {
		#region Fields
		readonly List<DataComponentInfo> innerList = new List<DataComponentInfo>();
		#endregion
		#region Properties
		List<DataComponentInfo> InnerList { get { return innerList; } }
		DocumentModel DocumentModel { get; set; }
		public int DefaultIndex { get; private set; }
		public int Count { get { return InnerList.Count; } }
		public object NonIDataComponentDataSource { get; private set; }
		public string NonIDataComponentDataMember { get; private set; }
		#endregion
		public DataComponentInfoList(DocumentModel model) {
			DocumentModel = model;
			DefaultIndex = -1;
			NonIDataComponentDataSource = null;
			NonIDataComponentDataMember = null;
		}
		public void Clear() {
			innerList.Clear();
			DefaultIndex = -1;
		}
		internal void InnerClear() {
			innerList.Clear();
		}
		public DataComponentInfo GetDataComponentInfoForReading(int index) {
			if(index < 0 || index >= InnerList.Count)
				return null;
			return InnerList[index].Clone();
		}
		public void Add(DataComponentInfo info) {
			Guard.ArgumentNotNull(info, "DataComponentInfo");
			DataComponentInfoAddHistoryItem historyItem = new DataComponentInfoAddHistoryItem(DocumentModel, info);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void AddRange(IEnumerable<DataComponentInfo> range) {
			InnerList.AddRange(range);
		}
		public void SetAsDataSource(int index) {
			if(index < 0 || index >= InnerList.Count)
				return;
			DataComponentInfo info = InnerList[index];
			IDataComponent dataComponent = info.TryToLoadDataComponentFromXml();
			if(dataComponent == null)
				return;
			if(DocumentModel.MailMergeDataSource != null && !(DocumentModel.MailMergeDataSource is IDataComponent)) {
				SaveApplicationDataSource();
			}
			PerformSetAsDataSource(dataComponent);
			DefaultIndex = index;
		}
		internal void SaveApplicationDataSource() {
			NonIDataComponentDataMember = DocumentModel.MailMergeDataMember;
			NonIDataComponentDataSource = DocumentModel.MailMergeDataSource;
		}
		internal void PerformSetAsDataSource(IDataComponent dataComponent) {
			DocumentModel.MailMergeDataSource = dataComponent;
		}
		public void TryToFillDataSource(IDataComponent dataComponent) {
			dataComponent.Fill(this.DocumentModel.MailMergeParameters.InnerList);
		}
		public void ReturnToApplicationDataSource() {
			DefaultIndex = -1;
			DocumentModel.MailMergeDataSource = NonIDataComponentDataSource;
			DocumentModel.MailMergeDataMember = NonIDataComponentDataMember;
		}
		public void UpdateActiveDataSource(IDataComponent dataSource) {
			if(dataSource == null)
				return;
			if(DefaultIndex < 0 || DefaultIndex >= InnerList.Count)
				return;
			DataComponentInfo info = DataComponentInfo.CreateFromIDataComponent(dataSource);
			InnerList[DefaultIndex] = info;
			PerformSetAsDataSource(dataSource);
		}
		internal void InnerAdd(DataComponentInfo info) {
			Guard.ArgumentNotNull(info, "DataComponentInfo");
			InnerList.Add(info);
		}
		internal void InnerRemove(int index) {
			InnerList.RemoveAt(index);
		}
		public IEnumerator<DataComponentInfo> GetEnumerator() {
			return innerList.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		internal DataComponentInfo GetDefaultDataComponentInfo() {
			return DefaultIndex < 0 || DefaultIndex >= InnerList.Count ? null : InnerList[DefaultIndex];
		}
		internal void ResetDataSource() {
			DocumentModel.MailMergeDataMember = null;
			DocumentModel.MailMergeDataSource = null;
			DefaultIndex = -1;
		}
	}
	#region DataComponentInfoHistory
	public class DataComponentInfoAddHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly DataComponentInfo info;
		int index;
		#endregion
		#region Properties
		DataComponentInfoList Infos { get { return ((DocumentModel) (DocumentModel)).DataComponentInfos; } }
		#endregion
		public DataComponentInfoAddHistoryItem(IDocumentModelPart documentModelPart, DataComponentInfo info) : base(documentModelPart) {
			this.info = info;
		}
		public override void Execute() {
			Infos.InnerAdd(info);
			index = Infos.Count - 1;
		}
		protected override void UndoCore() {
			Infos.InnerRemove(index);
		}
		protected override void RedoCore() {
			Infos.InnerAdd(info);
		}
	}
	public class SelectDataSourceHistoryItem : SpreadsheetHistoryItem {
		#region Properties
		int NewIndex { get; set; }
		int OldIndex { get; set; }
		DataComponentInfoList Infos { get { return ((DocumentModel) DocumentModel).DataComponentInfos; } }
		#endregion
		public SelectDataSourceHistoryItem(IDocumentModelPart documentModelPart, int newIndex) : base(documentModelPart) {
			NewIndex = newIndex;
			OldIndex = Infos.DefaultIndex;
		}
		protected override void UndoCore() {
			PerformSetDataSource(OldIndex);
		}
		protected override void RedoCore() {
			PerformSetDataSource(NewIndex);
		}
		void PerformSetDataSource(int index) {
			if(index == -1) {
				Infos.ReturnToApplicationDataSource();
			}
			else {
				Infos.SetAsDataSource(index);
			}
		}
	}
	public class ManageQueriesHistoryItem : SpreadsheetHistoryItem {
		#region Properties
		List<SqlQuery> NewQueries { get; set; }
		List<SqlQuery> OldQueries { get; set; }
		SqlDataSource DataSource { get { return Workbook.MailMergeDataSource as SqlDataSource; } }
		#endregion
		public ManageQueriesHistoryItem(IDocumentModelPart documentModelPart, List<SqlQuery> oldQueries) : base(documentModelPart) {
			Debug.Assert(DataSource != null);
			OldQueries = new List<SqlQuery>();
			OldQueries.AddRange(oldQueries);
			NewQueries = new List<SqlQuery>();
			NewQueries.AddRange(DataSource.Queries);
		}
		public override void Execute() {
			Workbook.DataComponentInfos.UpdateActiveDataSource(DataSource);
			Workbook.RaiseMailMergeQueriesChanged();
		}
		void PerformSetQueries(List<SqlQuery> queries) {
			DataSource.Queries.Clear();
			DataSource.Queries.AddRange(queries);
			DataSource.RebuildResultSchema();
			Workbook.DataComponentInfos.UpdateActiveDataSource(DataSource);
			Workbook.RaiseMailMergeQueriesChanged();
		}
		protected override void UndoCore() {
			PerformSetQueries(OldQueries);
		}
		protected override void RedoCore() {
			PerformSetQueries(NewQueries);
		}
	}
	public class ManageRelationsHistoryItem : SpreadsheetHistoryItem {
		#region Properties
		List<MasterDetailInfo> OldValues { get; set; }
		List<MasterDetailInfo> NewValues { get; set; }
		SqlDataSource DataSource { get { return Workbook.MailMergeDataSource as SqlDataSource; } }
		#endregion
		public ManageRelationsHistoryItem(IDocumentModelPart documentModelPart, List<MasterDetailInfo> oldRelations) : base(documentModelPart) {
			OldValues = new List<MasterDetailInfo>();
			OldValues.AddRange(oldRelations);
			NewValues = new List<MasterDetailInfo>();
			NewValues.AddRange(DataSource.Relations);
		}
		void PerformSetRelations(List<MasterDetailInfo> relations) {
			DataSource.Relations.Clear();
			foreach(MasterDetailInfo relation in relations) {
				DataSource.Relations.Add(relation);
			}
			DataSource.RebuildResultSchema();
			Workbook.DataComponentInfos.UpdateActiveDataSource(DataSource);
			Workbook.RaiseMailMergeRelationsChanged();
		}
		protected override void UndoCore() {
			PerformSetRelations(OldValues);
		}
		protected override void RedoCore() {
			PerformSetRelations(NewValues);
		}
	}
	public class ManageDataSourcesHistoryItem : SpreadsheetHistoryItem {
		#region Properties
		List<DataComponentInfo> OldDatas { get; set; }
		List<DataComponentInfo> NewDatas { get; set; }
		List<SpreadsheetParameter> OldParameters { get; set; }
		List<SpreadsheetParameter> NewParameters { get; set; }
		DataComponentInfoList Infos { get { return Workbook.DataComponentInfos; } }
		int NewIndex { get; set; }
		int OldIndex { get; set; }
		#endregion
		public ManageDataSourcesHistoryItem(IDocumentModelPart documentModelPart, List<DataComponentInfo> newDatas, List<SpreadsheetParameter> oldParameters, int newIndex) : base(documentModelPart) {
			OldDatas = new List<DataComponentInfo>();
			OldDatas.AddRange(Workbook.DataComponentInfos);
			NewDatas = new List<DataComponentInfo>();
			NewDatas.AddRange(newDatas);
			OldParameters = new List<SpreadsheetParameter>();
			OldParameters.AddRange(oldParameters);
			NewParameters = new List<SpreadsheetParameter>();
			NewParameters.AddRange(Workbook.MailMergeParameters.InnerList);
			NewIndex = newIndex;
			OldIndex = Workbook.DataComponentInfos.DefaultIndex;
		}
		public override void Execute() {
			Infos.InnerClear();
			Infos.AddRange(NewDatas);
			ChangeIndex();
		}
		void PerformSetDataSource(int index) {
			if(index == -1) {
				Infos.ReturnToApplicationDataSource();
			} else {
				Infos.SetAsDataSource(index);
			}
		}
		void PerformsSet(List<DataComponentInfo> datas, List<SpreadsheetParameter> parameters) {
			Infos.InnerClear();
			Infos.AddRange(datas);
			Workbook.MailMergeParameters.Clear();
			Workbook.MailMergeParameters.AddRange(parameters);
		}
		protected override void UndoCore() {
			PerformsSet(OldDatas, OldParameters);
			if(!(NewIndex == -1 && OldIndex == -1))
				PerformSetDataSource(OldIndex);
		}
		protected override void RedoCore() {
			PerformsSet(NewDatas, NewParameters);
			ChangeIndex();
		}
		void ChangeIndex() {
			if(NewIndex == -1) {
				if(OldIndex != -1) {
					Workbook.DataComponentInfos.ResetDataSource();
				}
			}
			else {
				PerformSetDataSource(NewIndex);
			}
		}
	}
	#endregion
}
