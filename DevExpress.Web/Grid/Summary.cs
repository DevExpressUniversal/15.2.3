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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Data;
using DevExpress.Data.IO;
using DevExpress.Data.Summary;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class ASPxSummaryItemBase : CollectionItem, DevExpress.Utils.Design.ICaptionSupport, ISummaryItem {
		public ASPxSummaryItemBase() { }
		public ASPxSummaryItemBase(string fieldName, SummaryItemType summaryType) {
			FieldName = fieldName;
			SummaryType = summaryType;
		}
		protected internal SummaryItemType SummaryType {
			get { return (SummaryItemType)GetIntProperty("SummaryType", (int)SummaryItemType.None); }
			set {
				if(SummaryType == value) return;
				SetIntProperty("SummaryType", (int)SummaryItemType.None, (int)value);
				OnSummaryChanged();
			}
		}
		protected internal virtual string FieldName {
			get { return GetStringProperty("FieldName", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				SetStringProperty("FieldName", string.Empty, value);
				OnSummaryChanged();
			}
		}
		protected internal bool Visible {
			get { return GetBoolProperty("Visible", true); }
			set {
				if(Visible == value)
					return;
				SetBoolProperty("Visible", true, value);
				OnSummaryChanged();
			}
		}
		protected internal virtual string DisplayFormat {
			get { return GetStringProperty("DisplayFormat", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				SetStringProperty("DisplayFormat", string.Empty, value);
				OnSummaryChanged();
			}
		}
		protected virtual string ValueDisplayFormat {
			get { return GetStringProperty("ValueDisplayFormat", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				SetStringProperty("ValueDisplayFormat", string.Empty, value);
				OnSummaryChanged();
			}
		}
		protected internal string Tag {
			get { return GetStringProperty("Tag", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				SetStringProperty("Tag", string.Empty, value);
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			ASPxSummaryItemBase item = source as ASPxSummaryItemBase;
			Tag = item.Tag;
			DisplayFormat = item.DisplayFormat;
			ValueDisplayFormat = item.ValueDisplayFormat;
			FieldName = item.FieldName;
			SummaryType = item.SummaryType;
			Visible = item.Visible;
		}
		protected internal string GenerateDisplayText(string defaultFormat, object value, IWebGridColumn column) {
			string format = DisplayFormat;
			if(String.IsNullOrEmpty(format)) {
				format = defaultFormat;
				value = GenerateValueDisplayText(column, value);
			}
			return String.Format(ResolveDisplayFormat(format), value, column != null ? column.ToString() : String.Empty);
		}
		protected string GenerateValueDisplayText(IWebGridColumn column, object value) {
			return String.Format(ResolveDisplayFormat(GetValueDisplayFormat(column)), value);
		}
		protected string GetValueDisplayFormat(IWebGridColumn column) {
			var format = ValueDisplayFormat;
			if(!String.IsNullOrEmpty(format))
				return format;
			if(SummaryType == SummaryItemType.Count) {
				format = GetDefaultValueDisplayFormat();
			} else {
				format = InferValueDisplayFormat(column);
				if(String.IsNullOrEmpty(format))
					format = GetDefaultValueDisplayFormat();
			}
			return format;
		}
		protected internal string GetDefaultDisplayFormatForSameColumn() {
			switch(SummaryType) {
				case SummaryItemType.Sum:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Summary_Sum);
				case SummaryItemType.Min:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Summary_Min);
				case SummaryItemType.Max:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Summary_Max);
				case SummaryItemType.Average:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Summary_Average);
				case SummaryItemType.Count:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Summary_Count);
			}
			return String.Empty;
		}
		protected internal string GetDefaultDisplayFormatForOtherColumn() {
			switch(SummaryType) {
				case SummaryItemType.Sum:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Summary_Sum_OtherColumn);
				case SummaryItemType.Min:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Summary_Min_OtherColumn);
				case SummaryItemType.Max:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Summary_Max_OtherColumn);
				case SummaryItemType.Average:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Summary_Average_OtherColumn);
				case SummaryItemType.Count:
					return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Summary_Count);
			}
			return String.Empty;
		}
		protected string GetDefaultValueDisplayFormat() {
			switch(SummaryType) {
				case SummaryItemType.Average:
				case SummaryItemType.Sum:
					return "0.##";
				case SummaryItemType.Count:
					return "N0";
			}
			return String.Empty;
		}
		protected internal string GetSummaryDisplayText(IWebGridColumn column, object value) {
			return GenerateDisplayText(GetDefaultDisplayFormatForOtherColumn(), value, column);
		}
		string InferValueDisplayFormat(IWebGridColumn column) {
			IWebGridDataColumn dc = column as IWebGridDataColumn;
			if(dc != null && dc.PropertiesEdit != null)
				return dc.PropertiesEdit.DisplayFormatString;
			return String.Empty;
		}
		internal string ResolveDisplayFormat(string format) {
			if(String.IsNullOrEmpty(format))
				return "{0}";
			if(!format.Contains("{"))
				format = "{0:" + format + "}";
			return format;
		}
		protected void OnSummaryChanged() {
			if(IsLoading() || Collection == null)
				return;
			OnSummaryChangedCore();
		}
		protected virtual void OnSummaryChangedCore() { }
		protected internal virtual void Load(TypedBinaryReader reader) {
			FieldName = reader.ReadObject<string>();
			SummaryType = (SummaryItemType)reader.ReadObject<int>();
			DisplayFormat = reader.ReadObject<string>();
			ValueDisplayFormat = reader.ReadObject<string>();
			Tag = reader.ReadObject<string>();
			Visible = reader.ReadObject<bool>();
		}
		protected internal virtual void Save(TypedBinaryWriter writer) {
			writer.WriteObject(FieldName);
			writer.WriteObject((int)SummaryType);
			writer.WriteObject(DisplayFormat);
			writer.WriteObject(ValueDisplayFormat);
			writer.WriteObject(Tag);
			writer.WriteObject(Visible);
		}
		string DevExpress.Utils.Design.ICaptionSupport.Caption {
			get {
				if(SummaryType == SummaryItemType.None)
					return string.Empty;
				string res = SummaryType.ToString();
				if(!string.IsNullOrEmpty(FieldName))
					res = FieldName + " (" + res + ")";
				return res;
			}
		}
		string ISummaryItem.DisplayFormat { get { return DisplayFormat; } set { DisplayFormat = value; } }
		string ISummaryItem.FieldName { get { return FieldName; } }
		SummaryItemType ISummaryItem.SummaryType { get { return SummaryType; } }
	}
	public abstract class ASPxGridSummaryItemCollectionBase<T> : DevExpress.Web.Collection<T> where T : ASPxSummaryItemBase, new(){
		public event CollectionChangeEventHandler SummaryChanged;
		public ASPxGridSummaryItemCollectionBase(IWebControlObject webControlObject)
			: base(webControlObject) { }
		protected T this[string fieldName] { get { return this.FirstOrDefault(i => i.FieldName == fieldName); } }
		protected T this[string fieldName, SummaryItemType summaryType] { get { return this.FirstOrDefault(i => i.FieldName == fieldName && i.SummaryType == summaryType); } }
		protected virtual T CreateSummaryItem(SummaryItemType summaryType, string fieldName) {
			var item = new T();
			item.FieldName = fieldName;
			item.SummaryType = summaryType;
			return item;
		}
		protected T Add(SummaryItemType summaryType, string fieldName) {
			return AddInternal(CreateSummaryItem(summaryType, fieldName));
		}
		protected internal List<T> GetActiveItems() {
			return this.Where(i => i.SummaryType != SummaryItemType.None).ToList();
		}
		protected internal void OnSummaryChanged(ASPxSummaryItemBase item) {
			if(IsLoading || Owner == null)
				return;
			if(SummaryChanged != null)
				SummaryChanged(this, new CollectionChangeEventArgs(CollectionChangeAction.Refresh, item));
		}
		protected override void OnChanged() {
			base.OnChanged();
			OnSummaryChanged(null);
		}
		public override string ToString() { return string.Empty; }
	}
}
