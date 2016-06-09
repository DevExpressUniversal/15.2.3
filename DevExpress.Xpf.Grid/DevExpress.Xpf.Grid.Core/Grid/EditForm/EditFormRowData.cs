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

using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Editors.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Xpf.Grid.EditForm {
	public class EditFormRowData {
		public static Func<EditFormRowData> Factory { get { return ViewModelSource.Factory(() => new EditFormRowData()); } }
		protected EditFormRowData() {
			ActualEditFormOwner = EmptyEditFormOwner.Instance;
		}
		public virtual IEditFormOwner EditFormOwner { get; set; }
		IEditFormOwner ActualEditFormOwner { get; set; }
		public virtual IList<EditFormCellDataBase> EditFormCellData { get; protected set; }
		public EditFormLayoutSettings LayoutSettings { get; protected set; }
		public bool IsModified { get; protected set; }
		public virtual bool ShowUpdateCancelButtons { get; protected set; }
		public virtual object Source { get; protected set; }
		bool canShowUpdateCancelPanelCore = true;
		internal bool CanShowUpdateCancelButtons {
			get { return canShowUpdateCancelPanelCore; }
			set {
				if(canShowUpdateCancelPanelCore != value) {
					canShowUpdateCancelPanelCore = value;
					RefreshUpdateCancelPanel();
				}
			}
		}
		protected void OnEditFormOwnerChanged() {
			ActualEditFormOwner = EditFormOwner ?? EmptyEditFormOwner.Instance;
			UnsubscribeFromData();
			RefreshUpdateCancelPanel();
			Source = ActualEditFormOwner.Source;
			int columnCount = ActualEditFormOwner.ColumnCount > 0 ? ActualEditFormOwner.ColumnCount : 1;
			List<EditFormCellDataBase> dataList = CreateCellData(columnCount);
			EditFormLayoutCalculator calculator = CreateLayoutCalculator(columnCount);
			UpdatePositions(dataList, calculator);
			LayoutSettings = new EditFormLayoutSettings(calculator.LayoutSettings.ColumnCount * 2, calculator.LayoutSettings.RowCount);
			EditFormCellData = dataList;
		}
		Dictionary<string, EditFormCellData> editorDataCache = new Dictionary<string, EditFormCellData>();
		List<EditFormCellDataBase> CreateCellData(int columnCount) {
			List<EditFormCellDataBase> dataList = new List<EditFormCellDataBase>();
			IEnumerable<EditFormColumnSource> sourceList = ActualEditFormOwner.CreateEditFormColumnSource().OrderBy(x => x.VisibleIndex);
			int currentVisibleIndex = 0;
			editorDataCache = new Dictionary<string, EditFormCellData>();
			foreach(EditFormColumnSource source in sourceList) {
				if(!source.Visible)
					continue;
				CoerceSource(source, columnCount);
				EditFormCaptionData captionData = CreateCaptionData();
				captionData.Assign(source);
				dataList.Add(captionData);
				EditFormCellData cellData = CreateCellData();
				cellData.Assign(source);
				cellData.VisibleIndex = currentVisibleIndex;
				currentVisibleIndex++;
				cellData.Value = EditFormOwner.GetValue(cellData);
				Validate(cellData);
				cellData.RowData = this;
				cellData.ValueChangedEvent += OnCellValueChanged;
				if(!string.IsNullOrEmpty(cellData.FieldName))
					editorDataCache.Add(cellData.FieldName, cellData);
				dataList.Add(cellData);
			}
			return dataList;
		}
		void CoerceSource(EditFormColumnSource source, int columnCount) {
			if(!source.ColumnSpan.HasValue)
				source.ColumnSpan = DefaultSpanHelper.CalcDefaultColumnSpan(source, columnCount);
			if(!source.RowSpan.HasValue)
				source.RowSpan = DefaultSpanHelper.CalcDefaultRowSpan(source);
			if(source.ColumnSpan > columnCount)
				source.ColumnSpan = columnCount;
		}
		internal protected virtual EditFormLayoutCalculator CreateLayoutCalculator(int columnCount) {
			return new EditFormLayoutCalculator() { ColumnCount = columnCount };
		}
		internal protected virtual EditFormCellData CreateCellData() {
			return new EditFormCellData();
		}
		internal protected virtual EditFormCaptionData CreateCaptionData() {
			return new EditFormCaptionData();
		}
		void UpdatePositions(IList<EditFormCellDataBase> items, EditFormLayoutCalculator calculator) {
			List<IEditFormLayoutItem> packedItems = new List<IEditFormLayoutItem>();
			IEditFormLayoutItem caption = null;
			IEditFormLayoutItem editor = null;
			foreach(IEditFormLayoutItem item in items) {
				if(item.ItemType == EditFormLayoutItemType.Caption)
					caption = item;
				else if(item.ItemType == EditFormLayoutItemType.Editor)
					editor = item;
				if(caption != null && editor != null) {
					packedItems.Add(new CaptionedLayoutItem(caption, editor));
					caption = null;
					editor = null;
				}
			}
			calculator.SetPositions(packedItems);
		}
		IList<EditFormCellData> GetEditorData() {
			if(EditFormCellData == null)
				return null;
			return EditFormCellData.OfType<EditFormCellData>().ToList();
		}
		internal EditFormCellData GetEditorData(string fieldName) {
			if(string.IsNullOrEmpty(fieldName))
				return null;
			EditFormCellData data = null;
			editorDataCache.TryGetValue(fieldName, out data);
			return data;
		}
		void UnsubscribeFromData() {
			IList<EditFormCellData> dataList = GetEditorData();
			if(dataList == null)
				return;
			foreach(EditFormCellData data in dataList) {
				data.ValueChangedEvent -= OnCellValueChanged;
				data.RowData = null;
			}
		}
		void OnCellValueChanged(object sender, EventArgs e) {
			EditFormCellData data = (EditFormCellData)sender;
			IsModified = true;
			Validate(data);
			if(EditFormOwner.EditMode == EditFormPostMode.Immediate)
				CommitCore(data);
		}
		void CommitCore(EditFormCellData data) {
			if(!data.ReadOnly)
				ActualEditFormOwner.SetValue(data);
		}
		void Validate(EditFormCellData data) {
			data.ValidationError = ActualEditFormOwner.Validate(data);
		}
		void RefreshUpdateCancelPanel() {
			ShowUpdateCancelButtons = CanShowUpdateCancelButtons && ActualEditFormOwner.ShowUpdateCancelButtons;
		}
		public void Commit() {
			TryCommit();
		}
		internal bool TryCommit() {
			IList<EditFormCellData> dataList = GetEditorData();
			if(dataList == null)
				return true;
			if(dataList.Any(x => x.ValidationError != null))
				return false;
			bool hasCommitErrors = false;
			foreach(EditFormCellData data in dataList) {
				CommitCore(data);
				if(data.ValidationError != null)
					hasCommitErrors = true;
			}
			if(hasCommitErrors)
				return false;
			else {
				Close();
				return true;
			}
		}
		public void Cancel() {
			IList<EditFormCellData> dataList = GetEditorData();
			if(dataList == null)
				return;
			if(EditFormOwner.EditMode == EditFormPostMode.Immediate) {
				foreach(EditFormCellData data in dataList)
					data.ResetValue();
			}
			Close();
		}
		public void Close() {
			ActualEditFormOwner.OnInlineFormClosed();
		}
	}
	internal static class DefaultSpanHelper {
		static Dictionary<Type, int> defaultColumnSpans = CreateDefaultColumnSpans();
		static Dictionary<Type, int> defaultRowSpans = CreateDefaultRowSpans();
		const int AllColumnsSpan = int.MinValue;
		static Dictionary<Type, int> CreateDefaultColumnSpans() {
			var spans = new Dictionary<Type, int>();
			spans.Add(typeof(MemoEditSettings), AllColumnsSpan);
			spans.Add(typeof(ImageEditSettings), 2);
			return spans;
		}
		static Dictionary<Type, int> CreateDefaultRowSpans() {
			var spans = new Dictionary<Type, int>();
			spans.Add(typeof(MemoEditSettings), 3);
			spans.Add(typeof(ImageEditSettings), 3);
			return spans;
		}
		public static int CalcDefaultColumnSpan(EditFormColumnSource source, int columnCount) {
			int columnSpan = GetDefaultSpan(source, defaultColumnSpans);
			if(columnSpan == AllColumnsSpan)
				columnSpan = columnCount;
			return Math.Min(columnCount, columnSpan);
		}
		public static int CalcDefaultRowSpan(EditFormColumnSource source) {
			return GetDefaultSpan(source, defaultRowSpans);
		}
		static int GetDefaultSpan(EditFormColumnSource source, Dictionary<Type, int> registeredSpans) {
			Type settingsType = null;
			if(source != null && source.EditSettings != null)
				settingsType = source.EditSettings.GetType();
			int span = 1;
			if(settingsType != null)
				registeredSpans.TryGetValue(settingsType, out span);
			return span;
		}
	}
}
