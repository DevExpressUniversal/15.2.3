#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Web.TestScripts;
using System.Web.UI;
using DevExpress.ExpressApp.Actions;
using DevExpress.Web;
using System.ComponentModel;
using DevExpress.Web.Internal;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	[ToolboxItem(false)]
	public abstract class DropDownSingleChoiceActionControlBase : Table, INamingContainer {
		private ASPxLabel label;
		private ASPxComboBox comboBox;
		private bool isPrerendered;
		private bool clientEnabled = true;
		private void UpdateEnabled() {
			if(Label != null) {
				Label.ClientEnabled = ClientEnabled;
			}
			if(ComboBox != null) {
				ComboBox.ClientEnabled = ClientEnabled;
			}
		}
		protected void FillLabelCell(TableCell cell) {
			label = RenderHelper.CreateASPxLabel();
			label.CssClass = "Label";
			label.ID = "L";
			cell.Controls.Add(label);
		}
		protected void FillEditorCell(TableCell cell) {
			comboBox = RenderHelper.CreateASPxComboBox();
			comboBox.ID = "Cb";
			comboBox.EncodeHtml = true;
			comboBox.AutoPostBack = false;
			comboBox.EnableClientSideAPI = true;
			comboBox.ShowImageInEditBox = true;
			cell.Controls.Add(comboBox);
		}
		protected override void OnPreRender(EventArgs e) {
			isPrerendered = true;
			if (string.IsNullOrEmpty(label.Text)) {
				Rows[0].Cells[0].Visible = false;
			}
			base.OnPreRender(e);
		}
		protected override void Render(HtmlTextWriter writer) {
			if(!isPrerendered) {
				OnPreRender(EventArgs.Empty);
			}
			base.Render(writer);
			RenderUtils.WriteScriptHtml(writer, @"window." + ClientID + @" =  new DropDownSingleChoiceActionClientControl('" + ClientID + "');");
		}
		public bool ClientEnabled {
			get {
				return clientEnabled;
			}
			set {
				clientEnabled = value;
				UpdateEnabled();
			}
		}
		public ASPxLabel Label {
			get { return label; }
		}
		public ASPxComboBox ComboBox {
			get { return comboBox; }
		}
		public virtual void SetConfirmationMessage(string message) {
		}
		public DropDownSingleChoiceActionControlBase() {
			CellSpacing = 0;
			CellPadding = 0;
		}
	}
	[ToolboxItem(false)]
	public class DropDownSingleChoiceActionControlHorizontal : DropDownSingleChoiceActionControlBase {
		public DropDownSingleChoiceActionControlHorizontal() {
			Rows.Add(new TableRow());
			Rows[0].Cells.Add(new TableCell());
			Rows[0].Cells.Add(new TableCell());
			Rows[0].Cells[0].CssClass = "SingleChoiceActionItemLabel";
			Rows[0].Cells[1].CssClass = "SingleChoiceActionItemEditor";
			FillLabelCell(Rows[0].Cells[0]);
			FillEditorCell(Rows[0].Cells[1]);
		}
	}
	[ToolboxItem(false)]
	public class DropDownSingleChoiceActionControlVertical : DropDownSingleChoiceActionControlBase {
		public DropDownSingleChoiceActionControlVertical() {
			Rows.Add(new TableRow());
			Rows.Add(new TableRow());
			Rows[0].Cells.Add(new TableCell());
			Rows[1].Cells.Add(new TableCell());
			FillLabelCell(Rows[0].Cells[0]);
			FillEditorCell(Rows[1].Cells[0]);
		}
	}
	public class JSASPxDropDownSingleChoiceActionControl : IJScriptTestControl {
		public const string ClassName = "JSASPxDropDownSingleChoiceActionControl";
		public JSASPxDropDownSingleChoiceActionControl() { }
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				ASPxStandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.GetTextFunctionBody = @"                    
					return this.control.GetText();";
				result.ActFunctionBody = @"
                    if(this.control) {
                        this.control.Act(value);
					}
					else {
						this.LogOperationError('The item ' + value + ' is not found.');						
					}
				";
				result.IsEnabledFunctionBody = @"
					return this.control.GetEnabled();
                    ";
				result.SetTextFunctionBody = @"
					this.control.SetText(value);
				";
				result.AdditionalFunctions = new JSFunctionDeclaration[] {
					new JSFunctionDeclaration("IsActionItemVisible", "actionItemName", @"
                        var comboBox = this.control.GetComboBox();
                        for(var i = 0; i < comboBox.GetItemCount(); i++) {
                            if(comboBox.GetItem(i).text == actionItemName) {
                                return true; 
                            }
                        }
                        return false;
                    "),
					new JSFunctionDeclaration("IsActionItemEnabled", "actionItemName", @"
                        return this.IsActionItemVisible(actionItemName);
                    ")
				};
				return result;
			}
		}
		#endregion
	}
}
