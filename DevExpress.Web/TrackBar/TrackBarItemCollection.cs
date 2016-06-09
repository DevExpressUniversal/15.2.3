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
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class TrackBarItemCollection : ListEditItemCollection {
		public TrackBarItemCollection()
			: base() {
		}
		public TrackBarItemCollection(IWebControlObject owner)
			: base(owner) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("TrackBarItemCollectionItem")]
#endif
		public new TrackBarItem this[int index] {
			get { return (GetItem(index) as TrackBarItem); }
		}
		protected internal string Serialize() {
			object[] itemCollection = new object[Count];
			for(int i = 0; i < this.Count; i++)
				itemCollection[i] = new object[] { this[i].Value ?? this[i].Text, this[i].Text, this[i].ToolTip };
			return HtmlConvertor.ToJSON(itemCollection);
		}
		protected TrackBarProperties TrackBarProperties {
			get {
				return Owner as TrackBarProperties;
			}
		}
		protected override internal Type ItemValueType {
			get {
				return TrackBarProperties != null ? TrackBarProperties.ValueType : null;
			}
		}
		public override ListEditItemBase Add(string text, object value, string toolTip) {
			TrackBarItem item = new TrackBarItem(text, value, toolTip);
			Add(item);
			return item;
		}
		protected override ListEditItemBase CreateItem() {
			return new TrackBarItem();
		}
		public void Add(TrackBarItem item) {
			base.Add(item);
		}
		public new TrackBarItem Add() {
			return CreateItem() as TrackBarItem;
		}
		public new virtual TrackBarItem Add(string text) {
			return Add(text, text);
		}
		public new virtual TrackBarItem Add(string text, object value) {
			TrackBarItem item = CreateItem() as TrackBarItem;
			item.Text = text;
			item.Value = value;
			Add(item);
			return item;
		}
		public new TrackBarItem FindByValue(object value) {
			int index = IndexOfValue(value);
			return index != -1 ? this[index] : null;
		}
		public new TrackBarItem FindByText(string text) {
			return base.FindByTextInternal(text) as TrackBarItem;
		}
		public new TrackBarItem FindByTextWithTrim(string text) {
			return base.FindByTextWithTrimInternal(text) as TrackBarItem;
		}
		internal TrackBarItem Find(Predicate<TrackBarItem> predicate) {
			foreach(TrackBarItem item in this)
				if(predicate(item))
					return item;
			return null;
		}
		internal string GetTextByValue(object value) {
			TrackBarItem item = Find(delegate(TrackBarItem itm) {
				return CommonUtils.AreEqual(itm.Value, value, ConvertEmptyStringToNull);
			});
			return item != null ? item.Text : null; 
		}
		protected override Type GetKnownType() {
			return typeof(TrackBarItem);
		}
	}
	[ControlBuilder(typeof(TrackBarItemBuilder))]
	public class TrackBarItem : ListEditItemBase {
		public TrackBarItem ()
			: base() {
		}
		public TrackBarItem (string text)
			: this(text, text) {
		}
		public TrackBarItem (string text, object value) {
			Text = text;
			Value = value;
		}
		public TrackBarItem(string text, object value, string toolTip)
			: this(text, value) {
				ToolTip = toolTip;
		}
		public override void Assign(CollectionItem source) {
			TrackBarItem item = source as TrackBarItem;
			if(item != null)
				ToolTip = item.ToolTip;
			base.Assign(source);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarItemText"),
#endif
		Localizable(true), Bindable(true), DefaultValue(""), RefreshProperties(RefreshProperties.All),
		NotifyParentProperty(true), AutoFormatDisable]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarItemToolTip"),
#endif
		Localizable(true), Bindable(true), DefaultValue(""), RefreshProperties(RefreshProperties.All),
		NotifyParentProperty(true), AutoFormatDisable]
		public string ToolTip {
			get { return GetStringProperty("ToolTip", ""); }
			set { SetStringProperty("ToolTip", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TrackBarItemValue"),
#endif
		DefaultValue(null), NotifyParentProperty(true), RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(StringToObjectTypeConverter))]
		public override object Value {
			get { return base.Value; }
			set { base.Value = value; }
		}
	}
}
namespace DevExpress.Web.Internal {
	public class TrackBarItemBuilder : ListEditItemBuilder {
	}
	public delegate string GetToolTipField();
	public class TrackBarDataHelper : EditDataHelper {
		private GetToolTipField getToolTipField = null;
		public TrackBarDataHelper(IEditDataHelperOwner owner, GetToolTipField getToolTipField)
			: base(owner) {
				this.getToolTipField = getToolTipField;
		}
		protected override void AddNewItemToCollection(object dataItem, string text, object value, string imageUrl) {
			TrackBarItem item = (Items as TrackBarItemCollection).Add(text, value);
			item.ToolTip = GetDataItemToolTip(dataItem, getToolTipField(), DesignMode);
		}
		public static string GetDataItemToolTip(object dataItem, string toolTipFieldName, bool designMode) {
			return DataUtils.GetFieldValue(dataItem, toolTipFieldName, toolTipFieldName != "", designMode, "").ToString();
		}
	}
}
