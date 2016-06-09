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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.DropDownEdit {
	public class ColorTablesControl : ASPxInternalWebControl {
		protected internal static string ColorTableIdPostfix = "CT";
		public ColorTablesControl(ColorNestedControl colorNestedControl)
			: base() {
			ColorNestedControl = colorNestedControl;
		}
		protected WebControl MainDiv { get; private set; }
		protected ColorNestedControl ColorNestedControl { get; private set; }
		protected ColorTable ColorTable { get; private set; }
		protected CustomColorsControl CustomColorsControl { get; private set; }
		protected AutomaticColorItemControl AutomaticColorItemControl { get; private set; }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			MainDiv = null;
			ColorTable = null;
			CustomColorsControl = null;
		}
		protected override void CreateControlHierarchy() {
			MainDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			MainDiv.ID = ColorNestedControl.GetColorTablesID();
			if(ColorNestedControl.EnableAutomaticColorItem)
				CreateAutomaticColorItemContol(MainDiv);
			CreateColorTable(MainDiv);
			if(ColorNestedControl.EnableCustomColors)
				CreateCustomColorsContol(MainDiv);
			Controls.Add(MainDiv);
		}
		protected void CreateAutomaticColorItemContol(WebControl MainDiv) {
			AutomaticColorItemControl = new AutomaticColorItemControl(ColorNestedControl);
			MainDiv.Controls.Add(AutomaticColorItemControl);
		}
		protected void CreateColorTable(WebControl MainDiv) {
			ColorTable = new ColorTable(ColorNestedControl, ColorNestedControl.Properties.GetColorTableItems());
			ColorTable.ColumnCount = ColorNestedControl.GetColumnCountForRender();
			ColorTable.ID = ColorTableIdPostfix;
			ColorTable.ParentSkinOwner = ColorNestedControl;
			ColorTable.UsedInDropDown = true;
			MainDiv.Controls.Add(ColorTable);
		}
		protected void CreateCustomColorsContol(WebControl MainDiv) {
			CustomColorsControl = new CustomColorsControl(ColorNestedControl);
			MainDiv.Controls.Add(CustomColorsControl);
		}
		protected override void PrepareControlHierarchy() {
			PrepareMainDiv();
			PrepareColorTable();
		}
		protected virtual void PrepareMainDiv() {
			ColorNestedControl.GetColorTablesMainDivStyle().AssignToControl(MainDiv);
		}
		protected virtual void PrepareColorTable() {
			ColorTable.TableCellStyle.CopyFrom(ColorNestedControl.GetColorTableCellStyle());
		}
	}
	public class CustomColorsControl : ASPxInternalWebControl {
		protected internal static string CustomColorTableIdPostfix = "UCT";
		protected internal static string CustomColorButtonIdPostfix = "CCB";
		public CustomColorsControl(ColorNestedControl colorNestedControl)
			: base() {
			ColorNestedControl = colorNestedControl;
		}
		protected ColorNestedControl ColorNestedControl { get; private set; }
		protected WebControl CustomColorDiv { get; private set; }
		protected LiteralControl CustomColorButtonLiteralControl { get; private set; }
		protected CustomColorTable CustomColorTable { get; private set; }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			CustomColorTable = null;
			CustomColorDiv = null;
		}
		protected override void CreateControlHierarchy() {
			CreateCustomColorButton();
			CreateCustomColorTable();
		}
		protected void CreateCustomColorTable() {
			CustomColorTable = new CustomColorTable(ColorNestedControl, ColorNestedControl.CustomColorTableItems);
			CustomColorTable.ColumnCount = ColorNestedControl.ColumnCount;
			CustomColorTable.ID = CustomColorTableIdPostfix;
			CustomColorTable.ParentSkinOwner = ColorNestedControl;
			CustomColorTable.UsedInDropDown = true;
			Controls.Add(CustomColorTable);
		}
		protected void CreateCustomColorButton() {
			CustomColorDiv = RenderUtils.CreateDiv();
			CustomColorButtonLiteralControl = RenderUtils.CreateLiteralControl();
			CustomColorDiv.Controls.Add(CustomColorButtonLiteralControl);
			CustomColorDiv.ID = CustomColorButtonIdPostfix;
			Controls.Add(CustomColorDiv);
		}
		protected override void PrepareControlHierarchy() {
			PrepareCustomColorButton();
			PrepareCustomColorTable();
		}
		protected virtual void PrepareCustomColorButton() {
			ColorNestedControl.GetCustomColorButtonStyle().AssignToControl(CustomColorDiv);
			CustomColorButtonLiteralControl.Text = ColorNestedControl.HtmlEncode(ColorNestedControl.CustomColorButtonText);
		}
		protected virtual void PrepareCustomColorTable() {
			CustomColorTable.TableCellStyle.CopyFrom(ColorNestedControl.GetColorTableCellStyle());
		}
	}
	public class AutomaticColorItemControl: ASPxInternalWebControl {
		public AutomaticColorItemControl(ColorNestedControl colorNestedControl)
			: base() {
			ColorNestedControl = colorNestedControl;
		}
		protected ColorNestedControl ColorNestedControl { get; private set; }
		protected WebControl MainDiv { get; private set; }
		protected WebControl SelectionFrame { get; private set; }
		protected WebControl ColorItem { get; private set; }
		protected WebControl Caption { get; private set; }
		protected LiteralControl LiteralControl { get; private set; }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			MainDiv = null;
		}
		protected override void CreateControlHierarchy() {
			CreateColorCell();
			CreateCaption();
			CreateMainDiv();
		}
		protected void CreateMainDiv() {
			MainDiv = RenderUtils.CreateDiv(EditorStyles.AutomaticColorItemSystemClassName);
			MainDiv.ID = ColorNestedControl.GetAutomaticColorItemId();
			MainDiv.Controls.Add(SelectionFrame);
			MainDiv.Controls.Add(Caption);
			Controls.Add(MainDiv);
		}
		protected void CreateColorCell() {
			SelectionFrame = RenderUtils.CreateDiv();
			SelectionFrame.ID = ColorNestedControl.GetAutomaticColorItemSelectionFrameId();
			ColorItem = RenderUtils.CreateDiv();
			SelectionFrame.Controls.Add(ColorItem);
		}
		protected void CreateCaption() {
			Caption = RenderUtils.CreateLabel();
			LiteralControl = RenderUtils.CreateLiteralControl();
			Caption.Controls.Add(LiteralControl);
		}
		protected override void PrepareControlHierarchy() {
			ColorNestedControl.GetAutomaticColorItemStyle().AssignToControl(MainDiv);
			ColorNestedControl.GetAutomaticColorItemCellStyle().AssignToControl(SelectionFrame, true);
			ColorNestedControl.GetAutomaticColorItemCellDivStyle().AssignToControl(ColorItem);			
			LiteralControl.Text = ColorNestedControl.HtmlEncode(ColorNestedControl.AutomaticColorItemCaption);
			ColorItem.BackColor = ColorNestedControl.AutomaticColor;
		}
	}
}
