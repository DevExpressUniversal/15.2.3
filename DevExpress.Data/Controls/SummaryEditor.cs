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

using DevExpress.Compatibility.System.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace DevExpress.Data.Summary {
	public interface ISummaryItem {
		string FieldName { get; }
		SummaryItemType SummaryType { get; }
		string DisplayFormat { get; set; }
	}
	public interface ISummaryItemsOwner {
		List<ISummaryItem> GetItems();
		void SetItems(List<ISummaryItem> items);
		ISummaryItem CreateItem(string fieldName, SummaryItemType summaryType);
		List<string> GetFieldNames();
		string GetCaptionByFieldName(string fieldName);
		Type GetTypeByFieldName(string fieldName);
	}
	public class SummaryItemsEditorController {
		ISummaryItemsOwner itemsOwner;
		List<ISummaryItem> items;
		List<ISummaryItem> initialItems;
		List<SummaryEditorUIItem> uiItems;
		public SummaryItemsEditorController(ISummaryItemsOwner itemsOwner) {
			if(itemsOwner == null) throw new ArgumentNullException("itemsOwner");
			this.itemsOwner = itemsOwner;
			this.initialItems = ItemsOwner.GetItems();
			this.items = new List<ISummaryItem>(initialItems);
			CreateItemWithCountSummary();   
			CreateUIItems();
		}
		public List<ISummaryItem> Items { get { return items; } }
		protected List<SummaryEditorUIItem> UIItems { get { return uiItems; } }
		public int Count { get { return uiItems.Count; } }
		public SummaryEditorUIItem this[int index] { get { return uiItems[index]; } }
		public SummaryEditorUIItem this[string fieldName] {
			get {
				foreach(SummaryEditorUIItem item in uiItems) {
					if(item.FieldName == fieldName) return item;
				}
				return null;
			}
		}
		public bool HasCountByEmptyField {
			get { return HasSummary(string.Empty, SummaryItemType.Count); }
			set { SetSummary(string.Empty, SummaryItemType.Count, value); }
		}
		protected ISummaryItemsOwner ItemsOwner { get { return itemsOwner; } }
		protected List<ISummaryItem> InitialItems { get { return initialItems; } }
		public virtual void AddSummary(string fieldName, SummaryItemType summaryType) {
			if(!CanApplySummary(summaryType, fieldName)) {
				throw new Exception(string.Format("Can not apply summary '{0}' to field '{1}'", summaryType.ToString(), fieldName));
			}
			if(!HasSummary(fieldName, summaryType)) {
				ISummaryItem item = FindSummaryItem(fieldName, summaryType, InitialItems);
				if(item == null) {
					item = ItemsOwner.CreateItem(fieldName, summaryType);
				}
				Items.Add(item);
			}
		}
		public virtual void RemoveSummary(string fieldName, SummaryItemType summaryType) {
			ISummaryItem item = FindSummaryItem(fieldName, summaryType, Items);
			if(item != null) {
				Items.Remove(item);
			}
		}
		public void SetSummary(string fieldName, SummaryItemType summaryType, bool value) {
			if(value) {
				AddSummary(fieldName, summaryType);
			}
			else {
				RemoveSummary(fieldName, summaryType);
			}
		}
		public virtual bool HasSummary(SummaryItemType summaryType) {
			foreach(ISummaryItem item in Items) {
				if(item.SummaryType == summaryType)
					return true;
			}
			return false;
		}
		public bool HasSummary(string fieldName, SummaryItemType summaryType) {
			return FindSummaryItem(fieldName, summaryType, Items) != null;
		}
		public bool HasSummary(string fieldName) {
			foreach(ISummaryItem item in Items) {
				if(item.FieldName == fieldName && IsGroupSummaryItem(item)) {
					switch(item.SummaryType) {
						case SummaryItemType.Average:
						case SummaryItemType.Count:
						case SummaryItemType.Max:
						case SummaryItemType.Min:
						case SummaryItemType.Sum: return true;
					}
				}
			}
			return false;
		}
		public void Apply() {
			ItemsOwner.SetItems(Items);
		}
		public List<SummaryEditorOrderUIItem> CreateOrderItems() {
			List<SummaryEditorOrderUIItem> list = new List<SummaryEditorOrderUIItem>();
			foreach(ISummaryItem item in Items) {
				if(IsGroupSummaryItem(item))
					list.Add(new SummaryEditorOrderUIItem(this, item, GetSummaryItemCaption(item)));
			}
			return list;
		}
		protected virtual string GetTextBySummaryType(SummaryItemType summaryType) {
			return Enum.GetName(typeof(SummaryItemType), summaryType);
		}
		protected virtual void CreateItemWithCountSummary() {
			List<ISummaryItem> itemsWithCountSummary = new List<ISummaryItem>();
			foreach(ISummaryItem item in Items) {
				if(item.SummaryType == SummaryItemType.Count) {
					itemsWithCountSummary.Add(item);
				}
			}
			if(itemsWithCountSummary.Count > 0) {
				int index = Items.IndexOf(itemsWithCountSummary[0]);
				foreach(ISummaryItem item in itemsWithCountSummary) {
					Items.Remove(item);
				}
				Items.Insert(index, ItemsOwner.CreateItem(string.Empty, SummaryItemType.Count));
			}
		}
		public bool CanApplySummary(SummaryItemType summaryType, string fieldName) {
			if(string.IsNullOrEmpty(fieldName) && summaryType == SummaryItemType.Count) return true;
			if(string.IsNullOrEmpty(fieldName)) return false;
			Type objectType = ItemsOwner.GetTypeByFieldName(fieldName);
			return SummaryItemTypeHelper.CanApplySummary(summaryType, objectType);
		}
		protected virtual string GetSummaryItemCaption(ISummaryItem item) {
			if(string.IsNullOrEmpty(item.FieldName) && item.SummaryType == SummaryItemType.Count) return GetTextBySummaryType(item.SummaryType);
			if(string.IsNullOrEmpty(item.FieldName)) throw new Exception(string.Format("Can not get caption of empty field with summary '{0}'", item.SummaryType));
			return string.Format("{0} - {1}", ItemsOwner.GetCaptionByFieldName(item.FieldName), GetTextBySummaryType(item.SummaryType));
		}
		protected virtual bool IsGroupSummaryItem(ISummaryItem item) {
			return true;
		}
		protected ISummaryItem FindSummaryItem(string fieldName, SummaryItemType summaryType, List<ISummaryItem> list) {
			foreach(ISummaryItem item in list) {
				if(TestItemAlignment(item) && item.FieldName == fieldName && item.SummaryType == summaryType && IsGroupSummaryItem(item))
					return item;
			}
			return null;
		}
		protected virtual bool TestItemAlignment(ISummaryItem item) {
			return true;
		}
		void CreateUIItems() {
			this.uiItems = new List<SummaryEditorUIItem>();
			foreach(string fieldName in ItemsOwner.GetFieldNames()) {
				this.uiItems.Add(new SummaryEditorUIItem(this, fieldName, ItemsOwner.GetCaptionByFieldName(fieldName), ItemsOwner.GetTypeByFieldName(fieldName)));
			}
			this.uiItems.Sort(UIItemCompare);
		}
		int UIItemCompare(SummaryEditorUIItem item1, SummaryEditorUIItem item2) {
			return string.Compare(item1.ToString(), item2.ToString());
		}
	}
	public class SummaryEditorUIItem : INotifyPropertyChanged {
		SummaryItemsEditorController controller;
		string fieldName;
		string caption;
		Type type;
		public SummaryEditorUIItem(SummaryItemsEditorController controller, string fieldName, string caption, Type type) {
			this.controller = controller;
			this.fieldName = fieldName;
			this.caption = caption;
			this.type = type;
		}
		public bool this[SummaryItemType summaryType] {
			get { return Controller.HasSummary(FieldName, summaryType); }
			set {
				Controller.SetSummary(FieldName, summaryType, value);
				RaisePropertyChanged("HasSummary");
			}
		}
		public string FieldName { get { return fieldName; } }
		public string Caption { get { return caption; } }
		public Type Type { get { return type; } }
		public bool HasSummary { get { return Controller.HasSummary(fieldName); } }
		protected SummaryItemsEditorController Controller { get { return controller; } }
		public override string ToString() { return Caption; }
		public bool CanDoSummary(SummaryItemType summaryType) {
			return SummaryItemTypeHelper.CanApplySummary(summaryType, Type);
		}
		public event PropertyChangedEventHandler PropertyChanged;
		void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	public class SummaryEditorOrderUIItem {
		SummaryItemsEditorController controller;
		ISummaryItem item;
		string caption;
		public SummaryEditorOrderUIItem(SummaryItemsEditorController controller, ISummaryItem item, string caption) {
			this.controller = controller;
			this.item = item;
			this.caption = caption;
		}
		public string Caption {
			get { return caption; }
			protected set { caption = value; }
		}
		public override string ToString() {
			return Caption;
		}
		protected virtual bool IsCanUp() {
			return Index > 0;
		}
		protected virtual bool IsCanDown() {
			return Index < controller.Items.Count - 1;
		}
		public bool CanUp { get { return IsCanUp(); } }
		public bool CanDown { get { return IsCanDown(); } }
		public virtual void MoveUp() {
			if(!CanUp) return;
			controller.Items.Reverse(Index - 1, 2);
		}
		public virtual void MoveDown() {
			if(!CanDown) return;
			controller.Items.Reverse(Index, 2);
		}
		public ISummaryItem Item { get { return item; } }
		public int Index { get { return controller.Items.IndexOf(Item); } }
		protected SummaryItemsEditorController Controller { get { return controller; } }
	}
	public class SummaryItemsSerializer {
		const char EscapeCharacter = '\\';
		public const char SummaryItemDelimiter = ';';
		public const char SummaryTypeDelimiter = ':';
		public const char SummaryDisplayFormatDelimiter = ',';
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static char SummaryItemDelimeter { get { return SummaryItemDelimiter; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static char SummaryTypeDelimeter { get { return SummaryTypeDelimiter; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static char SummaryDisplayFormatDelimeter { get { return SummaryDisplayFormatDelimiter; } }
		static readonly string EscapedEscapeCharacter = string.Concat(EscapeCharacter, EscapeCharacter);
		static readonly string EscapedSummaryItemDelimeter = string.Concat(EscapeCharacter, SummaryItemDelimiter);
		ISummaryItemsOwner itemsOwner;
		public SummaryItemsSerializer(ISummaryItemsOwner itemsOwner) {
			this.itemsOwner = itemsOwner;
		}
		protected ISummaryItemsOwner ItemsOwner { get { return itemsOwner; } }
		public virtual void Deserialize(string data) {
			List<ISummaryItem> summaryItems = new List<ISummaryItem>();
			foreach(string item in SplitAndDecodeData(data)) {
				summaryItems.Add(DeserializeSummaryItem(item));
			}
			ItemsOwner.SetItems(summaryItems);
		}
		public virtual string Serialize() {
			StringBuilder sb = new StringBuilder();
			foreach(ISummaryItem summaryItem in ItemsOwner.GetItems()) {
				if(sb.Length > 0) {
					sb.Append(SummaryItemDelimeter);
				}
				if(string.IsNullOrEmpty(summaryItem.FieldName) && summaryItem.SummaryType == SummaryItemType.Count) {
					sb.Append(summaryItem.SummaryType.ToString());
				}
				else {
					sb.Append(summaryItem.FieldName + SummaryTypeDelimeter + summaryItem.SummaryType.ToString());
				}
				if(!string.IsNullOrEmpty(summaryItem.DisplayFormat)) {
					sb.Append(SummaryDisplayFormatDelimeter + EncodeItem(summaryItem.DisplayFormat));
				}
			}
			return sb.ToString();
		}
		protected ISummaryItem DeserializeSummaryItem(string data) {
			string fieldName;
			SummaryItemType summaryType;
			string displayFormat;
			DeserializeSummaryItem(data, out fieldName, out summaryType, out displayFormat);
			ISummaryItem summaryItem = ItemsOwner.CreateItem(fieldName, summaryType);
			if(!string.IsNullOrEmpty(displayFormat)) {
				summaryItem.DisplayFormat = displayFormat;
			}
			return summaryItem;
		}
		protected virtual void DeserializeSummaryItem(string data, out string fieldName, out SummaryItemType summaryType, out string displayFormat) {
			displayFormat = SplitData(ref data, SummaryDisplayFormatDelimeter);
			fieldName = GetFieldName(data);
			string summaryTypeText = SplitData(ref data, SummaryTypeDelimeter);
			if(string.IsNullOrEmpty(summaryTypeText)) {
				summaryTypeText = data;
			}
			if(ItemsOwnerNotContainsField(fieldName)) {
				throw new Exception(string.Format("The field '{0}' is not found.", fieldName));
			}
			summaryType = (SummaryItemType)Enum.Parse(typeof(SummaryItemType), summaryTypeText, false);
			Type objectType = !string.IsNullOrEmpty(fieldName) ? ItemsOwner.GetTypeByFieldName(fieldName) : null;
			if(!string.IsNullOrEmpty(fieldName) && objectType == null) return;
			if(!SummaryItemTypeHelper.CanApplySummary(summaryType, objectType)) {
				throw new Exception(string.Format("The summary type '{0}' is not valid for field '{1}'.", summaryType, fieldName));
			}
		}
		protected bool ItemsOwnerNotContainsField(string fieldName) {
			return !string.IsNullOrEmpty(fieldName) && ItemsOwner.GetFieldNames().IndexOf(fieldName) < 0;
		}
		protected string GetFieldName(string data) {
			string fieldName;
			string summaryTypeText = SplitData(ref data, SummaryTypeDelimeter);
			if(string.IsNullOrEmpty(summaryTypeText)) {
				summaryTypeText = data;
				fieldName = string.Empty;
			}
			else {
				fieldName = data;
			}
			return fieldName;
		}
		string SplitData(ref string data, char splitter) {
			int index = data.IndexOf(splitter);
			string result = string.Empty;
			if(index >= 0) {
				result = data.Substring(index + 1);
				data = data.Substring(0, index);
			}
			return result;
		}
		string EncodeItem(string toEncode) {
			return toEncode.Replace(new string(EscapeCharacter, 1), EscapedEscapeCharacter).Replace(new string(SummaryItemDelimeter, 1), EscapedSummaryItemDelimeter);
		}
		string DecodeItem(string toDecode) {
			return toDecode.Replace(EscapedEscapeCharacter, new string(EscapeCharacter, 1)).Replace(EscapedSummaryItemDelimeter, new string(SummaryItemDelimeter, 1));
		}
		protected string[] SplitAndDecodeData(string data) {
			List<string> result = new List<string>();
			int itemStartIndex = 0;
			for(int i = 0; i < data.Length; i++) {
				if(data[i] == EscapeCharacter) {
					i++;
					if(i == data.Length || (data[i] != EscapeCharacter && data[i] != SummaryItemDelimeter)) {
						throw new ArgumentException(string.Format("The {0} data has an excessive escape character.", data), "data");
					}
				}
				else if(data[i] == SummaryItemDelimeter) {
					if(itemStartIndex != i) {
						result.Add(DecodeItem(data.Substring(itemStartIndex, i - itemStartIndex)));
					}
					itemStartIndex = i + 1;
				}
			}
			if(itemStartIndex < data.Length) {
				result.Add(DecodeItem(data.Substring(itemStartIndex)));
			}
			return result.ToArray();
		}
	}
}
