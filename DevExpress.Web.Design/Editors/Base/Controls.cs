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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.Web.Design {
	public enum CheckTableGeneratorType { FillByContent = 0, FillByColumnIndex = 1 }
	public class CheckPropertiesDescriptor : XtraPanel {
		protected const int GroupItemHeight = 34;
		const float FontSizeGroupItems = 9.5f;
		List<CheckEdit> tabsCheckBoxes;
		int rowCount = 3;
		public CheckPropertiesDescriptor(CheckPropertiesModel propertiesModel) {
			PropertiesModel = propertiesModel;
			CheckGeneratorType = CheckTableGeneratorType.FillByContent;
		}
		LabelControl LabelCaption { get; set; }
		CheckPropertiesModel PropertiesModel { get; set; }
		List<CheckEdit> TabsCheckBoxes {
			get {
				if(tabsCheckBoxes == null)
					tabsCheckBoxes = new List<CheckEdit>();
				return tabsCheckBoxes;
			}
		}
		public int RowCount { get { return rowCount; } set { rowCount = value; } }
		public virtual CheckTableGeneratorType CheckGeneratorType { get; set; }
		public virtual int TopPadding { get; set; }
		public void Create(Control owner) {
			Create(owner, true);
		}
		public void Create(Control owner, bool showCaption) {
			Parent = owner;
			var captionText = showCaption ? PropertiesModel.PropertiesCaption : string.Empty;
			LabelCaption = CreateLabelCaption(this, "LabelCheckGroup", captionText, new Point(40, 10));
			var size = DesignTimeFormHelper.GetTextSize(LabelCaption, captionText,LabelCaption.Font);
			size.Height += LabelCaption.Top;
			size.Width += LabelCaption.Left;
			var checkProperties = PropertiesModel.SettingsCheckGroup;
			for(var i = 0; i < checkProperties.Count; ++i) {
				var checkBox = AddCheckBoxItem(checkProperties[i], i);;
				checkBox.Checked = PropertiesModel.GetPropertyItemValue(i);
				checkBox.CheckedChanged += (s, e) => { PropertiesModel.SetPropertyItemValue((int)checkBox.Tag, checkBox.Checked); };
				size.Width = Math.Max(size.Width, checkBox.Right + 10);
				size.Height = Math.Max(size.Height, checkBox.Bottom + 10);
			}
			Size = size;
		}
		int col = -1, row = 0;
		CheckEdit AddCheckBoxItem(CheckPropertyItem checkItemInfo, int itemIndex) {
			if(CheckGeneratorType == CheckTableGeneratorType.FillByContent) {
				col = itemIndex / RowCount;
				if(itemIndex % RowCount == 0)
					row = 0;
			} else {
				if(col != checkItemInfo.ColumnIndex) {
					col = checkItemInfo.ColumnIndex;
					row = 0;
				}
			}
			if(TabsCheckBoxes.Count <= itemIndex) {
				TabsCheckBoxes.Add(CreateCheckEdit(this, checkItemInfo.PropertyName, checkItemInfo.Text, itemIndex,
					new Point(LabelCaption.Left + col * 140 + 12, LabelCaption.Bottom + TopPadding + 14 + row * GroupItemHeight)));
			}
			++row;
			return TabsCheckBoxes[itemIndex];
		}
		LabelControl CreateLabelCaption(Control parent, string name, string text, Point location) {
			var labelCaption = DesignTimeFormHelper.CreateLabel(parent, name, text, location);
			labelCaption.Font = new Font(labelCaption.Font.FontFamily, 10.5f, FontStyle.Bold);
			labelCaption.Width = DesignTimeFormHelper.GetTextWidth(labelCaption, labelCaption.Text, labelCaption.Font) + 10;
			return labelCaption;
		}
		CheckEdit CreateCheckEdit(Control parent, string name, string text, int itemIndex, Point location) {
			var checkBox = new CheckEdit() { Name = name, Text = text, Tag = itemIndex };
			checkBox.Parent = parent;
			checkBox.Font = new Font(checkBox.Font.FontFamily, FontSizeGroupItems);
			checkBox.Height = 30;
			checkBox.Width = DesignTimeFormHelper.GetTextWidth(checkBox, text, checkBox.Font) + 32;
			checkBox.Location = location;
			checkBox.BringToFront();
			return checkBox;
		}
	}
	public abstract class CheckPropertiesModel : IOwnerEditingProperty {
		CheckPropertyItemCollection tabsCheckGroup;
		public CheckPropertiesModel(ASPxWebControl webControl, string caption) {
			PropertiesCaption = caption;
			WebControl = webControl;
			FillTabsCheckBoxGroup();
		}
		public string PropertiesCaption { get; set; }
		protected ASPxWebControl WebControl { get; private set; }
		public CheckPropertyItemCollection SettingsCheckGroup {
			get {
				if(tabsCheckGroup == null)
					tabsCheckGroup = new CheckPropertyItemCollection(PropertiesCaption);
				return tabsCheckGroup;
			}
		}
		protected abstract void FillTabsCheckBoxGroup();
		public void SetPropertyItemValue(int index, bool isChecked) {
			((IOwnerEditingProperty)this).ItemsChanged = true;
			SettingsCheckGroup[index].PropertyValue = isChecked;
		}
		public bool GetPropertyItemValue(int index) {
			return Convert.ToBoolean(SettingsCheckGroup[index].PropertyValue);
		}
		object IOwnerEditingProperty.PropertyInstance { get { return "Settings"; } }
		bool IOwnerEditingProperty.ItemsChanged { get; set; }
		void IOwnerEditingProperty.BeforeClosed() { }
		void IOwnerEditingProperty.SaveUndo() { }
		void IOwnerEditingProperty.SaveChanges() { }
		void IOwnerEditingProperty.UndoChanges() {
			foreach(CheckPropertyItem propertyItem in SettingsCheckGroup)
				propertyItem.UndoPropertyValue();
		}
	}
	public class CheckPropertyItemCollection : Collection<CheckPropertyItem> {
		public CheckPropertyItemCollection(string name) {
			Name = name;
		}
		string Name { get; set; }
		public void Add(object propertyOwner, string text, string propertyName) {
			Add(propertyOwner, text, propertyName, -1);
		}
		public void Add(object propertyOwner, string text, string propertyName, int columnIndex) {
			Add(new CheckPropertyItem(propertyOwner, text, propertyName, columnIndex));
		}
		public void Add(object propertyOwner, string text, CheckPropertyItem.OnGetPropertyValue propertyValueGetter, CheckPropertyItem.OnSetProperty propertySetter, int columnIndex) {
			Add(new CheckPropertyItem(propertyOwner, text, columnIndex, propertyValueGetter, propertySetter));
		}
		public void Add(object propertyOwner, string text, string propertyName, Type propertyType) {
			Add(propertyOwner, text, propertyName, propertyType, -1);
		}
		public void Add(object propertyOwner, string text, string propertyName, Type propertyType, int columnIndex) {
			Add(new CheckPropertyItem(propertyOwner, text, propertyName, propertyType, columnIndex));
		}
	}
	public class CheckPropertyItem : CollectionItem {
		Type type = typeof(bool);
		public delegate void OnSetProperty(object value);
		public delegate object OnGetPropertyValue();
		public CheckPropertyItem(object propertyOwner, string text, string propertyName, Type type)
			: this(propertyOwner, text, propertyName, type, -1) {
		}
		public CheckPropertyItem(object propertyOwner, string text, string propertyName, Type type, int columnIndex)
			: this(propertyOwner, text, propertyName) {
			Type = type;
			ColumnIndex = columnIndex;
		}
		public CheckPropertyItem(object propertyOwner, string text, string propertyName)
			: this(propertyOwner, text, propertyName, -1) {
		}
		public CheckPropertyItem(object propertyOwner, string text, string propertyName, int columnIndex) {
			Text = text;
			ColumnIndex = columnIndex;
			PropertyName = propertyName;
			PropertyOwner = propertyOwner;
			UndoValue = FeatureBrowserHelper.GetPropertyValue(PropertyOwner, PropertyName);
		}
		public CheckPropertyItem(object propertyOwner, string text, int columnIndex, OnGetPropertyValue propertyValueGetter, OnSetProperty propertySetter) {
			Text = text;
			ColumnIndex = columnIndex;
			PropertyOwner = propertyOwner;
			PropertyValueGetter = propertyValueGetter;
			PropertySetter = propertySetter;
			UndoValue = FeatureBrowserHelper.GetPropertyValue(PropertyOwner, PropertyName);
		}
		public Type Type { get { return type; } set { type = value; } }
		public string Text { get; private set; }
		public string PropertyName { get; private set; }
		public bool PropertyValue { get { return GetPropertyValue(); } set { SetPropertyValue(value); } }
		public int ColumnIndex { get; private set; }
		protected OnSetProperty PropertySetter { get; set; }
		protected OnGetPropertyValue PropertyValueGetter { get; set; }
		void SetPropertyValue(bool value) {
			var propertyValue = ConvertBoolToPorpertyValue(value);
			if(propertyValue == null)
				return;
			SetProperty(propertyValue);
		}
		bool GetPropertyValue() {
			var value = PropertyValueGetter != null ? PropertyValueGetter() : FeatureBrowserHelper.GetPropertyValue(PropertyOwner, PropertyName);
			return ConvertPorpertyValueToBool(value);
		}
		bool ConvertPorpertyValueToBool(object value) {
			bool result = false;
			if(Type == typeof(bool))
				return Convert.ToBoolean(value);
			if(Type == typeof(DefaultBoolean)) {
				result = ((DefaultBoolean)value) == DefaultBoolean.True ? true : false;
			} else if(Type == typeof(AutoBoolean)) {
				result = ((AutoBoolean)value) == AutoBoolean.True ? true : false;
			}
			return result;
		}
		object ConvertBoolToPorpertyValue(bool value) {
			if(Type == typeof(bool))
				return value;
			if(Type == typeof(DefaultBoolean))
				return value ? DefaultBoolean.True : DefaultBoolean.False;
			if(Type == typeof(AutoBoolean))
				return value ? AutoBoolean.True : AutoBoolean.False;
			return null;
		}
		object UndoValue { get; set; }
		object PropertyOwner { get; set; }
		public override string ToString() {
			return Text;
		}
		public void UndoPropertyValue() {
			SetProperty(UndoValue);
		}
		void SetProperty(object value) {
			if(!string.IsNullOrEmpty(PropertyName))
				FeatureBrowserHelper.SetPropertyValue(PropertyOwner, PropertyName, value);
			else if(PropertySetter != null)
				PropertySetter(value);
		}
	}
}
