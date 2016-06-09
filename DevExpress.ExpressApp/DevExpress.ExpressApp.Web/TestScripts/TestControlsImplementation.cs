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
using System.Web.UI;
using System.ComponentModel;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
namespace DevExpress.ExpressApp.Web.TestScripts {
	public class TestableUnknownClientIdWrapper : ITestable {
		private ITestable testable;
		public TestableUnknownClientIdWrapper(ITestable testable) {
			this.testable = testable;
			this.testable.ControlInitialized += new EventHandler<ControlInitializedEventArgs>(testable_ControlInitialized);
		}
		private void testable_ControlInitialized(object sender, ControlInitializedEventArgs e) {
			OnControlInitialized(e.Control);
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		#region ITestable Members
		public string TestCaption {
			get { return testable.TestCaption; }
		}
		public string ClientId {
			get { return !string.IsNullOrEmpty(testable.ClientId) ? testable.ClientId : "unknown"; }
		}
		public IJScriptTestControl TestControl {
			get { return testable.TestControl; }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public virtual TestControlType TestControlType {
			get {
				return testable.TestControlType;
			}
		}
		#endregion
	}
	public class JSDefaultTestControl : IJScriptTestControl {
		public const string ClassName = "DefaultControl";
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				return new StandardTestControlScriptsDeclaration();
			}
		}
	}
	public class JSTextBoxTestControl : IJScriptTestControl {
		public const string ClassName = "TextBox";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.SetTextFunctionBody = @"				
				if(!IsNull(this.control.isContentEditable)) {
					if(!this.control.isContentEditable) {
						this.LogOperationError('The ' + this.caption + ' editor is read-only.');
						return;
					}
				}
				this.control.value = value;
				";
				return result;
			}
		}
		#endregion
	}
	public class JSLabelTestControl : IJScriptTestControl {
		public const string ClassName = "Label";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.GetTextFunctionBody = @"
                //B153845 Bug Fix, kest
                if(this.control.tagName == 'TABLE') {
                    var label = this.FindLabel(this.control);
                    if(label) {
                        return this.FindLabel(this.control).innerText;
                    }
                }
				return this.Trim(this.control.innerText);
				";
				result.AdditionalFunctions = new JSFunctionDeclaration[] { 
					new JSFunctionDeclaration("FindLabel", "control", @"
                        var result;
                        if(control) {
                            for(var i=0;i<control.childNodes.length;i++) {
                                if(result) { return result; }
                                if(control.tagName == 'LABEL' || control.tagName == 'SPAN') {
                                    return control;
                                }
                                else {
                                    result = this.FindLabel(control.childNodes[i]);
                                }
                            }
                        }
                        return result;
                    "),
					 new JSFunctionDeclaration("Trim", "str", @"
                        while (str.substring(0,1) == ' ') {
                            str = str.substring(1, str.length);
                        }
                        while (str.substring(str.length-1, str.length) == ' ') {
                            str = str.substring(0,str.length-1);
                        }
                        str = str.replace(/^(\r\n)+/g,'');
                        str = str.replace(/(\r\n)+$/g,'');
                        return str;
			        ")
				};
				result.SetTextFunctionBody = @"				
			        this.LogOperationError('The text displayed by a Label control cannot be changed');
				";
				result.ActFunctionBody = @"
				//B38639
				if(!this.IsEnabled()) {
			        this.LogOperationError('The Label control is disabled');
				} else {
					//Commented because of (AB8572) bug fix 
					//Uncommented because of (B36753) bug fix 
						if(this.GetText() == '' || this.control.tagName.toUpperCase()!='A') {
						this.LogOperationError('No Actions are available');
					}
					else {
						this.control.click();				
					}
				}
				";
				return result;
			}
		}
		#endregion
	}
	public class JSCheckBoxTestControl : IJScriptTestControl {
		public const string ClassName = "CheckBox";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.GetTextFunctionBody = @"				
				return (this.control.checked) ? 'True' : 'False';
				";
				result.SetTextFunctionBody = @"				
				if(value != 'True' && value != 'False') {
					this.LogOperationError('Boolean properties can be set to either True or False. The ' + value + ' is not a valid value.');
				}				
				this.control.checked = (value == 'True') ? true : false;
				this.control.fireEvent('onchange');
				";
				result.AdditionalFunctions = new JSFunctionDeclaration[] { 
				new JSFunctionDeclaration("InitControl", null, @"					
					var f = this.inherit.prototype.baseInitControl;
					var oldId = this.id;
					this.id = this.id + 'Input';
                    try {
					    f.call(this);
                    } 
                    catch(ex) {
						this.error = null;
						this.operationError = false;
						this.id = oldId;
						f.call(this);
                    }
				")};
				return result;
			}
		}
		#endregion
	}
	public class JSDropDownListTestControl : IJScriptTestControl {
		public const string ClassName = "DropDownList";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.AdditionalParametersDeclaration.Add(StandardTestControlScriptsDeclaration.autoPostBackParamName);
				result.AdditionalFunctions = new JSFunctionDeclaration[] {
					new JSFunctionDeclaration("IsAutoPostBackMethod", "methodName", @"
				if(methodName == 'SetText' && " + StandardTestControlScriptsDeclaration.autoPostBackParamName + @") {
					return true;
				}
				return false;
				")};
				result.SetTextFunctionBody = @"
				var isFound = false;				
				for(var i = 0; i < this.control.options.length; i++) {
					var option = this.control.options[i];
					if (option.text == value) {
						isFound = true;
						this.control.selectedIndex = i;
						this.control.fireEvent('onchange');
						break;
					}
				}
				if (!isFound) {					
					this.LogOperationError('Cannot change the ' + this.caption + ' control\'s value. The list of available items does not contain the specified value');
				}
				";
				result.GetTextFunctionBody = @"				
				if(this.control.selectedIndex != -1) {
					return this.control.options[this.control.selectedIndex].innerText;
				}
				else {
					return '';
				}
				";
				result.ActFunctionBody = @"					
				this.SetText(value);
				";
				return result;
			}
		}
		#endregion
	}
	public class JSLookupEditTestControl : IJScriptTestControl {
		public const string ClassName = "LookupEdit";
		private static StandardTestControlScriptsDeclaration controlScriptsDeclaration;
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		static JSLookupEditTestControl() {
			controlScriptsDeclaration = new StandardTestControlScriptsDeclaration();
			controlScriptsDeclaration.IsEnabledFunctionBody = @"
					return !this.control.rows[0].cells[0].childNodes[0].childNodes[0].readOnly;
				";
			controlScriptsDeclaration.GetTextFunctionBody = @"
				return this.control.rows[0].cells[0].childNodes[0].childNodes[0].value;
				";
			controlScriptsDeclaration.ActFunctionBody = @"
				var button = this.control.rows[0].cells[1].childNodes[0];
				if(!button.disabled) {
					this.control.rows[0].cells[1].childNodes[0].click();
				} else {
					this.LogOperationError('Cannot execute the Edit Action in the ' + this.caption + ' control');
				}
				";
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				return controlScriptsDeclaration;
			}
		}
		#endregion
	}
	public class JSLookupDropDownEditTestControl : IJScriptTestControl {
		public const string ClassName = "LookupDropDownEdit";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.SetTextFunctionBody = @"					
				var isFound = false;				
				var control = this.control.rows[0].cells[0].childNodes[0];
				for(var i = 0; i < control.options.length; i++) {
					var option = control.options[i];
					if (option.text == value) {
						isFound = true;
						control.selectedIndex = i;
						control.fireEvent('onchange');
						break;
					}
				}
				if (!isFound) {						
					this.LogOperationError('Cannot change the ' + this.caption + ' control\'s value. The list of available values does not contain the specified value');
				}
				";
				result.GetTextFunctionBody = @"				
				var control = this.control.rows[0].cells[0].childNodes[0];
				return control.options[control.selectedIndex].text;
				";
				result.ActFunctionBody = @"				
				if(this.control.rows[0] && this.control.rows[0].cells[1] && this.control.rows[0].cells[1].childNodes[0] && this.control.rows[0].cells[1].childNodes[0].click) {
					this.control.rows[0].cells[1].childNodes[0].click();
				} else {
					this.LogOperationError('The ""' + value + '"" Action is inactive');
				}
				";
				return result;
			}
		}
		#endregion
	}
	public class JSGridEditorTestControl : IJScriptTestControl {
		public const string ClassName = "GridEditor";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				TableTestControlScriptsDeclaration result = new TableTestControlScriptsDeclaration();
				result.GetCellValueFunctionBody = @"				
				var table = this.control;
				result = '';
				if(table) {
					if(row + 1 >= table.rows.length) {
						var maxRows = table.rows.length - 1;
						this.LogOperationError( 'The grid contains ' + maxRows + ' rows');
						return result;
					}			
					var cell = table.rows[row + 1].cells[this.GetActionColumnCount() + column];
					if(cell.innerText == '' && cell.childNodes.length == 1) {
						var img = cell.childNodes[0];
						if(!IsNull(img.title)) {
							result = img.title;
						}
					}			
					else {
						result = table.rows[row + 1].cells[this.GetActionColumnCount() + column].innerText;
						if(result == ' ') result = '';
					}		
				}	
				return result;
				";
				result.GetColumnIndexFunctionBody = @"
				var table = this.control;
				if(table) {
					for(var i = this.GetActionColumnCount(); i < table.rows[0].cells.length; i++) {
						if(table.rows[0].cells[i].innerText == columnCaption) {
							return i - this.GetActionColumnCount();
						}
					}
				}
				else {
					return -1;
				}
				";
				result.GetColumnsCaptionsFunctionBody = @"
				var result = '';
				var table = this.control;
				if(table) {
					for(var i = this.GetActionColumnCount(); i < table.rows[0].cells.length; i++) {
						if(result == '') {
							result = table.rows[0].cells[i].innerText;
						} 
						else {
							result += ';' + table.rows[0].cells[i].innerText;
						}
					}
				}
				return result;
				";
				result.GetTableRowCountFunctionBody = @"				
				var table = this.control;
				if(table) {
				  return table.rows.length - 1;
				}
                return 0;
				";
				result.ExecuteActionFunctionBody = @"
                var action = this.GetAction(actionName, row, column);
                if(action) {
                    action.click();
                }
				";
				result.AdditionalFunctions = new JSFunctionDeclaration[] { 
				new JSFunctionDeclaration("InitControl", null, @"					
					var f = this.inherit.prototype.baseInitControl;
					f.call(this);
					if(this.error) {
						return;
					}
					if(this.control.tagName.toUpperCase() != 'TABLE') {
						this.control = null;
					} else {
						this.control = this.control;
					}
				"),
				new JSFunctionDeclaration("GetActionColumnCount", null, @"
				var table = this.control;
				if(table) {
					for(var i = 0; i < table.rows[0].cells.length; i++) {
						if(table.rows[0].cells[i].innerText != ' ') {
							return i;
						}
					}
				}
				return 0;
				"),				
				new JSFunctionDeclaration("SortByColumn", "columnCaption", @"
				var table = this.control;
				var isFound = false;
				if(table) {
					for(var i = this.GetActionColumnCount(); i < table.rows[0].cells.length; i++) {
						if(table.rows[0].cells[i].innerText == columnCaption) {
							isFound = true;
							table.rows[0].cells[i].childNodes[0].click();
							break;
						}
					}
				}
				if(!isFound) {
					this.LogOperationError( 'The grid does not contain the ' + columnCaption + ' column.');
				}										                    
				"),
				new JSFunctionDeclaration("CheckAction", "actionName, row, column", @"
                this.GetAction(actionName, row, column);
                "),
				new JSFunctionDeclaration("GetAction", "actionName, row, column", @"
				var table = this.control;
				if(!table) this.LogOperationError('Cannot perform an Action. The grid doesn\'t contain any records');
				if(row + 1 >= table.rows.length) {
					var maxRows = table.rows.length - 1;
					this.LogOperationError( 'The grid contains: ' + maxRows + ' rows');
					return;
				}
				this.callStack += '< action: ' + actionName + '\r\n';
				if(actionName != '') {					
					var isFound = false;
					for(var i = 0; i < this.GetActionColumnCount(); i++) {						
						if(table.rows[row + 1].cells[i].innerText != '' || table.rows[row + 1].cells[i].innerText == ' ') {							
							isFound = table.rows[row + 1].cells[i].innerText == actionName;
						}
						else if(table.rows[row + 1].cells[i].childNodes.length > 0) {
							isFound = table.rows[row + 1].cells[i].childNodes[0].childNodes[0].alt == actionName;
						}
						if(isFound) {
							return table.rows[row + 1].cells[i].childNodes[0];
						}
					}					
					if(!isFound) {
						this.LogOperationError( 'The ' + actionName + ' Action is not found in the ' + this.Caption + ' table\'s ' + row + ' row.');
					}
				}
				else {
					this.callStack += '< default ACTION \r\n';
                    var maxColumn = table.rows[row + 1].cells.length - this.GetActionColumnCount()
                    if(column >= maxColumn) {
                       this.LogOperationError( 'The grid contains ' + maxColumn + ' columns');
                    }
					this.callStack += '< row, column, text: ' + row + ', ' + column + ', ' + table.rows[row + 1].cells[column + this.GetActionColumnCount()].innerText + '\r\n';
					return table.rows[row + 1].cells[column + this.GetActionColumnCount()];
				}
				")
				};
				return result;
			}
		}
		#endregion
	}
	public class JSWebWorkflowEditorTestControl : IJScriptTestControl {
		public const string ClassName = "WebWorkflowEditor";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.GetTextFunctionBody = @"				
				var controlTable = this.control.childNodes[0];
				var textEdit = controlTable.rows[0].cells[0].childNodes[0];
				return textEdit.value;
				";
				result.SetTextFunctionBody = @"				
				var controlTable = this.control.childNodes[0];
				var textEdit = controlTable.rows[0].cells[0].childNodes[0];
				textEdit.value = value;
				textEdit.fireEvent('onchange');
				";
				result.ActFunctionBody = @"				
				var controlTable = this.control.childNodes[0];
				var buttons = controlTable.rows[0].cells[1].childNodes;
				var isFound = false;
				for(var i = 0; i < buttons.length; i++) {
					if(buttons[i].innerText == value) {
						isFound = true;
						buttons[i].fireEvent('onclick');
						break;
					}
				}
				if(!isFound) {
					this.LogOperationError( 'Cannot find the ' + value + ' Action.');
				}
				";
				return result;
			}
		}
		#endregion
	}
	public class JSMenuTestControl : IJScriptTestControl {
		public const string ClassName = "Menu";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.ActFunctionBody = @"
				var menuTable = this.control;
				if(IsNull(value)){
					var rootMenuItemTable = menuTable.rows[0].cells[0].childNodes[0];
					var rootMenuItem = rootMenuItemTable.rows[0].cells[0].childNodes[0];
					rootMenuItem.click();
				}
				else {				
					var menuItemsDiv = document.getElementById(this.id + 'n0Items');
					var menuItemsTable = menuItemsDiv.childNodes[0];
					for(var i = 0; i < menuItemsTable.rows.length; i++){
						var menuItem = menuItemsTable.rows[i].cells[0].childNodes[0];
						if(menuItem.innerText == value) {
							menuItem.childNodes[0].rows[0].cells[0].childNodes[0].click();
							break;
						}
					}
				}
				";
				return result;
			}
		}
		#endregion
	}
	public class JSGroupedListItemTestControl : IJScriptTestControl {
		public const string ClassName = "GroupedListItem";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.ActFunctionBody = @"				
				var tableControl = this.control;
				var isFound = false;
				for(var i = 0; i < tableControl.rows.length; i++) {
					if(tableControl.rows[i].cells[0].innerText == value) {
						isFound = true;
						if(!tableControl.rows[i].cells[0].childNodes[0].childNodes[0].disabled) {
							tableControl.rows[i].cells[0].childNodes[0].childNodes[0].click();
						}
						else {
							this.LogOperationError( 'The ' + value + ' item is disabled.');
						}
						break;
					}
				}
				if(!isFound) {
					this.LogOperationError( 'Cannot find the ' + value + ' item.');
				}
				";
				return result;
			}
		}
		#endregion
	}
	public class JSTextItemTestControl : IJScriptTestControl {
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return "TextItem"; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.GetTextFunctionBody = @"				
				return this.control.childNodes[0].value;
				";
				result.ActFunctionBody = @"				
				this.control.childNodes[0].value = value;
				this.control.childNodes[1].click();
				";
				return result;
			}
		}
		#endregion
	}
	public class JSButtonTestControl : IJScriptTestControl {
		#region IJScriptTestControl Members
		public const string ClassName = "Button";
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.GetTextFunctionBody = @"				
				return this.control.innerText;
				";
				result.ActFunctionBody = @"
				
				var button = this.control;
				if(this.control.tagName == 'SPAN') {
					button = this.control.childNodes[0];
				}
				if(button.disabled)
			        this.LogOperationError('No Actions are available');
				else
					button.click();
				";
				result.AdditionalFunctions = new JSFunctionDeclaration[] { 
					new JSFunctionDeclaration("GetHint", null, @"					
					return this.control.title;
					")
				};
				return result;
			}
		}
		#endregion
	}
	public class JSParametrizedActionTestControl : IJScriptTestControl {
		public string JScriptClassName {
			get { return "ParametrizedAction"; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				ASPxStandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.ActFunctionBody = @"
					if(!this.control.GetEnabled()) {
						this.LogOperationError('Cannot enter a parameter value into the ' + this.caption + ' Action. The element is disabled');
						return;
					}
					this.control.SetText(value);
					this.control.DoClick();
					";
				result.GetTextFunctionBody = @"
					return this.control.GetText();
					";
				return result; 
			}
		}
	}
	[ToolboxItem(false)]
	internal class BrowserNavigation : WebControl, ITestable, INamingContainer {
		protected override void OnUnload(EventArgs e) {
			OnControlInitialized(this);
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		#region ITestable Members
		string ITestable.TestCaption {
			get { return "BrowserNavigation"; }
		}
		string ITestable.ClientId {
			get { return "BrowserNavigation"; }
		}
		IJScriptTestControl ITestable.TestControl {
			get { return new JSBrowserNavigationTestControl(); }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public virtual TestControlType TestControlType {
			get {
				return TestControlType.Action;
			}
		}
		#endregion
	}
	internal class JSBrowserNavigationTestControl : IJScriptTestControl {
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return GetType().Name; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration scriptDeclaration = new StandardTestControlScriptsDeclaration();
				scriptDeclaration.ActFunctionBody = @"
				if(value == 'back')
					history.back();
				else if(value == 'forward')
					history.forward();
				else
					this.LogOperationError('Only the ""back"" and ""forward"" parameters are supported');
				";
				return scriptDeclaration;
			}
		}
		#endregion
	}	
}
