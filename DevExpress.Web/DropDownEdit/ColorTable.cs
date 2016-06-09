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
using System.Text;
using System.ComponentModel;
using DevExpress.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using System.Collections;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Web.Design;
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)
]
	public class ColorTable : ItemPickerBase {
		protected internal const string ColorTableScriptResourceName = ASPxColorEdit.EditScriptsResourcePath + "ColorTable.js";
		public const int DefaultColumnCount = 8;
		public static object[] DefaultColorTableItems {
			get {
				return new object[] {
					"#000000", "#993300", "#333300", "#003300", "#003366", "#000080", "#333399", "#333333",
					"#800000", "#FF6600", "#808000", "#008000", "#008080", "#0000FF", "#666699", "#808080",
					"#FF0000", "#FF9900", "#99CC00", "#339966", "#33CCCC", "#3366FF", "#800080", "#999999",
					"#FF00FF", "#FFCC00", "#FFFF00", "#00FF00", "#00FFFF", "#00CCFF", "#993366", "#C0C0C0",
					"#FF99CC", "#FFCC99", "#FFFF99", "#CCFFCC", "#CCFFFF", "#99CCFF", "#CC99FF", "#FFFFFF" };
			}
		}
		private bool usedInDropDown = false;
		public ColorTable()
			: this(null) {
		}
		protected internal ColorTable(ASPxWebControl ownerControl)
			: this(ownerControl, null) {
		}
		protected internal ColorTable(ASPxWebControl ownerControl, ColorEditItemCollection colorTableCollection)
			: base(ownerControl) {
			if(colorTableCollection == null)
				InitializeDefaultColorTable();
			else
				Items.Assign(colorTableCollection);
		}
		protected new ColorEditItemCollection Items {
			get {
				return base.Items as ColorEditItemCollection;
			}
		}
		protected void InitializeDefaultColorTable() {
			Items.AddRange(DefaultColorTableItems);
			ColumnCount = DefaultColumnCount;
		}
		[Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public ColorTableClientSideEvents ClientSideEvents {
			get { return (ColorTableClientSideEvents)base.ClientSideEventsInternal; }
		}
		[Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		public new ColorTableCellStyle TableCellStyle {
			get { return (ColorTableCellStyle)base.TableCellStyle; }
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(new ColorTableControl(this));
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ColorTableClientSideEvents();
		}
		protected new ColorTableStyles Styles {
			get { return StylesInternal as ColorTableStyles; }
		}
		protected override Collection CreateItemsCollection() {
			return new ColorEditItemCollection();
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.ColorTable";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ColorTable), ColorTableScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(ColumnCount != DefaultColumnCount)
				stb.AppendFormat("{0}.colorColCount = {1};\r\n", localVarName, ColumnCount);
			string[] itemsArray = Items.ToArray();
			if(itemsArray.Length != 0 && !System.Linq.Enumerable.SequenceEqual(itemsArray, DefaultColorTableItems))
				stb.AppendFormat("{0}.colorValues = {1};\r\n", localVarName, HtmlConvertor.ToJSON(itemsArray));
			stb.AppendFormat("{0}.colorTableCellStyleCssClassName = '{1}';\n", localVarName, GetTableCellStyle().CssClass);
			string colorTableCellStyleValue = GetTableCellStyle().GetStyleAttributes(Page).Value;
			if(!string.IsNullOrEmpty(colorTableCellStyleValue)) {
				stb.AppendFormat("{0}.colorTableCellStyleCssText = {1};\n", localVarName,
					HtmlConvertor.ToScript(colorTableCellStyleValue));
			}
			stb.AppendFormat("{0}.colorTableCellDivStyleCssClassName = '{1}';\n", localVarName, GetColorTableCellDivStyle().CssClass);
			string colorTableDivStyleValue = GetColorTableCellDivStyle().GetStyleAttributes(Page).Value;
			if(!string.IsNullOrEmpty(colorTableDivStyleValue)) {
				stb.AppendFormat("{0}.colorTableCellDivStyleCssText = {1};\n", localVarName,
					HtmlConvertor.ToScript(colorTableDivStyleValue));
			}
			if(UsedInDropDown)
				stb.AppendFormat("{0}.usedInDropDown = true;\n", localVarName);
			StateScriptRenderHelper helper = new StateScriptRenderHelper(Page, ClientID);
			AppearanceStyleBase style = GetColorTableCellSelectedStyle();
			for(int i = 0; i < RowCount * ColumnCount; i++)
				helper.AddStyle(style, GetItemCellID(i), IsEnabled());
			helper.GetCreateSelectedScript(stb);
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override StylesBase CreateStyles() {
			return new ColorTableStyles(this);
		}
		protected internal AppearanceStyle GetColorTableCellDivStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultColorTableCellDivStyle());
			style.CopyFrom(TableCellStyle.ColorTableDivStyle);
			return style;
		}
		protected internal AppearanceStyle GetColorTableCellSelectedStyle() {
			AppearanceStyle ret = new AppearanceStyle();
			ret.CopyFrom(Styles.GetDefaultColorTableCellSelectedStyle());
			ret.CopyFrom(TableCellStyle.SelectedStyle);
			return ret;
		}
		protected override bool HasHoverScripts() {
			return IsEnabled();
		}
		protected internal bool UsedInDropDown {
			get { return usedInDropDown; }
			set { usedInDropDown = value; }
		}
		protected override ItemPickerTableCellStyle CreateTableCellStyles() {
			return new ColorTableCellStyle();
		}
		public static string[] DeserializeColors(string serializedColors) {
			return serializedColors.Split(',');
		}
		public static string SerializeColors(ColorEditItemCollection items) {
			return string.Join(",", items.ToArray());
		}
	}
	[ToolboxItem(false)
]
	public class ColorTableControl : ItemPickerBaseControl {
		public ColorTableControl(ItemPickerBase itemsTableContol)
			: base(itemsTableContol) {
		}
		protected ColorTable ColorTable {
			get { return base.ItemPicker as ColorTable; }
		}
		protected override bool AllowRenderCell(int index) {
			return index < ColorTable.Items.Count;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
		}
		protected override void CreateItemsTableCellContent(TableCell cell, int index) {
			WebControl div = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			cell.Controls.Add(div);
		}
	}
	public class ColorTableClientSideEvents : ClientSideEvents {
		public ColorTableClientSideEvents()
			: base() {
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ColorChanged {
			get { return GetEventHandler("ColorChanged"); }
			set { SetEventHandler("ColorChanged", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("ColorChanged");
		}
	}
	public class ColorTableStyles : ItemPickerStyles {
		public ColorTableStyles(ColorTable colorTable)
			: base(colorTable) {
		}
		protected override string GetControlClassName() {
			return "ColorTable";
		}
		protected internal AppearanceStyle GetDefaultColorTableCellDivStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(string.Format("{0}CellDiv", GetControlClassName())));
			return style;
		}
		protected internal AppearanceStyle GetDefaultColorTableCellSelectedStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(string.Format("{0}CellSelected", GetControlClassName())));
			return style;
		}
	}
}
namespace DevExpress.Web {
	public class ColorEditItem : CollectionItem {
		public ColorEditItem()
			: this(Color.Empty) {
		}
		public ColorEditItem(Color colorValue)
			: base() {
			Color = colorValue;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditItemColor"),
#endif
		DefaultValue(typeof(Color), ""), NotifyParentProperty(true), RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(WebColorConverter))]
		public Color Color
		{
			get { return GetColorProperty("Color", Color.Empty); }
			set { SetColorProperty("Color", Color.Empty, value); }
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			ColorEditItem src = source as ColorEditItem;
			if(src != null)
				Color = src.Color;
		}
		public override string ToString() {
			Color color = (Color != Color.Empty) ? Color : Color.Empty;
			return ColorUtils.ToHexColor(color);
		}
		protected override string GetDesignTimeCaption() {
			var result = ToString();
			return string.IsNullOrEmpty(result) ? GetType().Name : result;
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class ColorEditItemCollection : Collection {
		public ColorEditItemCollection() 
			: base() {
		}
		public ColorEditItemCollection(ICollection collection)
			: this(null, collection) {
		}
		public ColorEditItemCollection(IWebControlObject owner)
			: base(owner) {
		}
		public ColorEditItemCollection(IWebControlObject owner, ICollection collection)
			: base(owner) {
			AddRange(collection);
		}
		public ColorEditItem Add(Color colorValue) {
			ColorEditItem colorItem = new ColorEditItem(colorValue);
			base.Add(colorItem);
			return colorItem;
		}
		public void Add(ColorEditItem item) {
			base.Add(item);
		}
		public void AddRange(ICollection collection) {
			foreach(object objItem in collection) {
				ColorEditItem item = objItem as ColorEditItem;
				if(item != null)
					Add(CloneItem(item));
				else {
					if(objItem is Color)
						Add(new ColorEditItem((Color)objItem));
					else
						Add(ColorUtils.ValueToColor(objItem));
				}
			}
		}
		public virtual void CreateDefaultItems(bool clearExistingCollection) {
			if(clearExistingCollection)
				Clear();
			AddRange(ColorTable.DefaultColorTableItems);
		}
		protected internal string[] ToArray() {
			string[] array = new string[this.Count];
			for(int i = 0; i < this.Count; i++) {
				object color = ((ColorEditItem)GetItem(i)).Color;
				array[i] = ColorUtils.ToHexColor((Color)color);
			}
			return array;
		}
		public ColorEditItem this[int index] {
			get { return GetItem(index) as ColorEditItem; }
			set { 
				(GetItem(index) as ColorEditItem).Color = value.Color; 
				if (Owner != null)
					Owner.LayoutChanged();
			}
		}
		protected internal void SetColor(int index, string colorValue) {
			int colorsCount = this.Count;
			if(index >= colorsCount)
				for(int i = 0; i < index - colorsCount + 1; i++)
					Add(new ColorEditItem(Color.Empty));
			((ColorEditItem)GetItem(index)).Color = ColorUtils.ValueToColor(colorValue);
		}
		protected override Type GetKnownType() {
			return typeof(ColorEditItem);
		}
	}
}
