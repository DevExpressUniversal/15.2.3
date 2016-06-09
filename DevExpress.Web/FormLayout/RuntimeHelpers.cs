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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
namespace DevExpress.Web.FormLayout.Internal.RuntimeHelpers {
	[ToolboxItem(false)]
	public abstract class DialogFormLayoutBase: ASPxFormLayout  {
		public abstract string GetLocalizedText(Enum value);
		public abstract string GetClientInstanceName(string name);
		public abstract string GetControlCssPrefix();
	}
	public static class DialogsHelper {
		public const string FullWidthCssClass = "dx-justification";
		public static void AddTab(this ASPxPageControl pageControl, string text, string name, Control control) {
			TabPage result = pageControl.TabPages.Add(text, name);
			result.Controls.Add(control);
		}
		public static LayoutItem InsertTableView(this LayoutItemCollection collection, string name = "", int colSpan = 1, params Control[] controls) {
			Table table = RenderUtils.CreateTable();
			TableRow tr = RenderUtils.CreateTableRow();
			table.Rows.Add(tr);
			for(int i = 0; i < controls.Length; i++) {
				TableCell currentCell = RenderUtils.CreateTableCell();
				currentCell.Controls.Add(controls[i]);
				tr.Cells.Add(currentCell);
			}
			LayoutItem result = collection.CreateItem(name, table);
			result.ShowCaption = string.IsNullOrEmpty(name) ? DefaultBoolean.False : DefaultBoolean.True;
			result.ColSpan = colSpan;
			string prefix = table.GetFormLayout().GetControlCssPrefix();
			table.CssClass = prefix + "dialogControlsWrapper" + string.Format(" {1}dcw{0}Col", controls.Length, prefix);
			return result;
		}
		public static LayoutItem InsertTemplate(this LayoutGroup group, ITemplate temlate) {
			if(temlate == null)
				return null;
			var div = RenderUtils.CreateDiv();
			temlate.InstantiateIn(div);
			LayoutItem item = group.Items.CreateItem("", div);
			item.ShowCaption = DefaultBoolean.False;
			item.ColSpan = group.ColCount;
			return item;
		}
		public static ASPxSpinEdit CreateSpinEdit(this LayoutItemCollection collection, string name, string cssClassName = FullWidthCssClass, bool showButtons = false,
			List<ASPxEditBase> buffer = null, string labelText = null, int colSpan = 1, LayoutItemCaptionLocation location = LayoutItemCaptionLocation.NotSet,
			bool showCaption = true, Enum pixelLabelText = null) {
			ASPxSpinEdit result = collection.CreateEditor<ASPxSpinEdit>(name, cssClassName, buffer: buffer, colSpan: colSpan, showCaption: showCaption, location: location);
			result.NumberType = SpinEditNumberType.Integer;
			result.MinValue = 0;
			result.MaxValue = 10000;
			result.AllowNull = true;
			result.ValidationSettings.Display = Display.None;
			result.SpinButtons.ShowIncrementButtons = showButtons;
			if(pixelLabelText != null)
				collection.CreateLabel(text: result.GetFormLayout().GetLocalizedText(pixelLabelText), buffer: buffer);
			return result;
		}
		public static ASPxLabel CreateLabel(this LayoutItemCollection collection, string text = "", List<ASPxEditBase> buffer = null, int colSpan = 1) {
			ASPxLabel label = new ASPxLabel();
			label.Text = text;
			LayoutItem item = collection.CreateItem("", label);
			item.ShowCaption = DefaultBoolean.False;
			item.ColSpan = colSpan;
			if(buffer != null)
				buffer.Add(label);
			return label;
		}
		public static ASPxCheckBox CreateCheckBox(this LayoutItemCollection collection, string name = "", int colSpan = 1, bool showCaption = false, List<ASPxEditBase> buffer = null) {
			return collection.CreateEditor<ASPxCheckBox>(name, showCaption: showCaption, colSpan: colSpan, buffer: buffer, cssClassName: "");
		}
		public static ASPxTextBox CreateTextBox(this LayoutItemCollection collection, string name = "", LayoutItemCaptionLocation location = LayoutItemCaptionLocation.NotSet,
			int colSpan = 1, bool clientVisible = true, bool showCaption = true, List<ASPxEditBase> buffer = null) {
			return collection.CreateEditor<ASPxTextBox>(name, location: location, colSpan: colSpan, clientVisible: clientVisible, showCaption: showCaption, buffer: buffer);
		}
		public static ASPxComboBox CreateComboBox(this LayoutItemCollection collection, string name = "", string cssClassName = FullWidthCssClass, int colSpan = 1, 
			LayoutItemCaptionLocation location = LayoutItemCaptionLocation.NotSet, bool showCaption = true, List<ASPxEditBase> buffer = null) {
			return collection.CreateEditor<ASPxComboBox>(name, colSpan: colSpan, buffer: buffer, cssClassName: cssClassName, showCaption: showCaption, location: location);
		}
		public static T CreateEditor<T>(this LayoutItemCollection collection, string name = "", string cssClassName = FullWidthCssClass,
			LayoutItemCaptionLocation location = LayoutItemCaptionLocation.NotSet, int colSpan = 1, bool clientVisible = true, bool showCaption = true,
			List<ASPxEditBase> buffer = null)
			where T : ASPxEditBase, new() {
			T result = new T();
			RenderUtils.AppendDefaultDXClassName(result, cssClassName);
			result.RootStyle.CssClass = cssClassName + " dx-dialogEditRoot";
			if(buffer != null)
				buffer.Add(result);
			LayoutItem item = collection.CreateItem(name, result);
			item.CaptionSettings.Location = location;
			item.ColSpan = colSpan;
			item.ShowCaption = showCaption ? DefaultBoolean.True : DefaultBoolean.False;
			item.ClientVisible = clientVisible;
			result.ClientInstanceName = result.GetFormLayout().GetClientInstanceName(name);
			return result;
		}
		public static LayoutGroup CreateGroup(this LayoutItemCollection collection, string name = null, bool showCaption = false) {
			var result = collection.Add<LayoutGroup>("", name);
			result.ShowCaption = showCaption ? DefaultBoolean.True : DefaultBoolean.False;
			result.GroupBoxStyle.Border.BorderStyle = BorderStyle.None;
			return result;
		}
		public static LayoutItem CreateItem(this LayoutItemCollection collection, string name, params Control[] controls) {
			var result = collection.Add<LayoutItem>("", name);
			var container = new LayoutItemNestedControlContainer();
			foreach(var control in controls)
				container.Controls.Add(control);
			result.LayoutItemNestedControlCollection.Add(container);
			return result;
		}
		public static void ApplyCommonSettings(this DialogFormLayoutBase formLayout) {
			string prefix = formLayout.GetControlCssPrefix();
			RenderUtils.AppendDefaultDXClassName(formLayout, prefix + "dialog");
			formLayout.Styles.LayoutGroup.CssClass = prefix + "dialogLG";
			formLayout.Styles.LayoutItem.CssClass = prefix + "dialogLI";
			formLayout.Styles.LayoutGroup.Cell.CssClass = prefix + "dialogLGC";
			formLayout.Styles.LayoutGroupBox.CssClass = prefix + "dialogLGB";
			formLayout.Styles.LayoutGroupBox.Caption.CssClass = prefix + "dialogLGBC";
			formLayout.Styles.LayoutItem.NestedControlCell.CssClass = prefix + "dialogLINC";
		}
		public static void AddEmptyItems(this DialogFormLayoutBase formLayout, params int[] colSpanArray) {
			colSpanArray = colSpanArray ?? new int[] { 1 };
			foreach(int colSpanValue in colSpanArray)
				formLayout.Items.Add<EmptyLayoutItem>("").ColSpan = colSpanValue;
		}
		public static void LocalizeField(this DialogFormLayoutBase formLayout, string name, Enum caption, string suffix = "") {
			formLayout.FindItemOrGroupByName(name).Caption = formLayout.GetLocalizedText(caption) + suffix;
		}
		static DialogFormLayoutBase GetFormLayout(this Control control) {
			if(control == null)
				return null;
			DialogFormLayoutBase formLayout = control as DialogFormLayoutBase;
			if(formLayout != null)
				return formLayout;
			return GetFormLayout(control.Parent);
		}
	}
}
