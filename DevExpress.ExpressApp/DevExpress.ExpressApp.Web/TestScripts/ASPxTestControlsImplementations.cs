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

using System.Collections.Generic;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
namespace DevExpress.ExpressApp.Web.TestScripts {
	internal class JSASPxTextBoxTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxTextBox";
		private static StandardTestControlScriptsDeclaration scriptDeclatation;
		static JSASPxTextBoxTestControl() {
			scriptDeclatation = new ASPxStandardTestControlScriptsDeclaration();
			scriptDeclatation.GetTextFunctionBody = @"
			var value = this.control.GetValue();			
			return (value == null) ? '' : value;
			";
			scriptDeclatation.SetTextFunctionBody = @"
			//B34542
            if(!ASPx.Browser.IE || ASPx.Browser.Version < 10){
			    if(this.control.inputElement.maxLength < value.length){
				    this.LogOperationError('The maximum number of characters allowed by the editor is ' + this.control.inputElement.maxLength + '. You\'ve tried to input ' + value.length + ' characters.');
				    return;
			    }
            }
            else{
			    if(this.control.inputMaxLength < value.length){
				    this.LogOperationError('The maximum number of characters allowed by the editor is ' + this.control.inputMaxLength + '. You\'ve tried to input ' + value.length + ' characters.');
				    return;
			    }
            }
			" +
			ASPxStandardTestControlScriptsDeclaration.defaultAspxSetTextFunctionBody;
		}
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				return scriptDeclatation;
			}
		}
	}
	public class JSASPxDateTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxDate";
		private static StandardTestControlScriptsDeclaration scriptDeclatation;
		static JSASPxDateTestControl() {
			scriptDeclatation = new ASPxStandardTestControlScriptsDeclaration();
			scriptDeclatation.GetTextFunctionBody = @"
			var value = this.control.GetFormattedDate();
			return (value == null) ? '' : value;
			";
			scriptDeclatation.SetTextFunctionBody =
			ASPxStandardTestControlScriptsDeclaration.defaultAspxBeforeSetTextFunctionBody + @"
			this.control.SetText(value);
			ASPx.ETextChanged(this.control.name);" +
			ASPxStandardTestControlScriptsDeclaration.defaultAspxAfterSetTextFunctionBody;
			scriptDeclatation.ActFunctionBody =
				ASPxStandardTestControlScriptsDeclaration.defaultAspxBeforeSetTextFunctionBody +
				@"		if(value == 'Clear'	){
							this.control.SetText('');
							ASPx.ETextChanged(this.control.name);
						}" +
				ASPxStandardTestControlScriptsDeclaration.defaultAspxAfterSetTextFunctionBody
				;
		}
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get { return scriptDeclatation; }
		}
		#endregion
	}
	public class JSASPxSpinTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxSpin";
		private static StandardTestControlScriptsDeclaration scriptDeclatation;
		static JSASPxSpinTestControl() {
			scriptDeclatation = new ASPxStandardTestControlScriptsDeclaration();
			scriptDeclatation.GetTextFunctionBody = @"
			//TODO Bykov 
			// replace next line with 'var value = this.control.GetText();'
			var value = this.control.GetFormattedNumber(this.control.GetNumber());
			return (value == null) ? '' : '' + value;
			";
			scriptDeclatation.SetTextFunctionBody = ASPxStandardTestControlScriptsDeclaration.defaultAspxSetTextFunctionBody;
		}
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				return scriptDeclatation;
			}
		}
	}
	public class JSASPxComboBoxTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxComboBox";
		#region IJScriptTestControl Members
		public virtual string JScriptClassName {
			get { return ClassName; }
		}
		public virtual TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				ASPxStandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.GetTextFunctionBody = @"
					return this.control.GetText();";
				result.SetTextFunctionBody = @"
					var isFound = false;					
					for(i = 0; i < this.control.GetItemCount(); i++) {
						if(this.CompareString(this.control.GetItem(i).text, value)) {
							this.control.SetSelectedIndex(i);
							ASPx.EValueChanged(this.control.name);
							isFound = true;
							break;
						}
					}
					if (!isFound) {
						if(this.control.isDropDownListStyle) {
						this.LogOperationError('Cannot change the ' + this.caption + ' control\'s value. The list of available values doesn\'t contain the specified value');
						} else {
							" + ASPxStandardTestControlScriptsDeclaration.defaultAspxSetTextFunctionBody + @"
						}
					}";
				result.ActFunctionBody = @"
					this.SetText(value);";
				result.IsEnabledFunctionBody = @"
                    var result = this.control.GetEnabled();
                    if(result) {
                        var mainElement = this.control.GetMainElement();
                        if(mainElement) {
                            var parent = mainElement.parentElement;
                            while(parent) {
                                if(parent.disabled) {
                                    result = false;
                                    break;
                                }
                                parent = parent.parentElement;
                            }
                        }
                    }   
					return result;
                    ";
				return result;
			}
		}
		#endregion
	}
	public class JSASPxCheckBoxTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxCheckBox";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				ASPxStandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.GetTextFunctionBody = @"
				    return this.control.GetChecked() ? 'True' : 'False';
				";
				result.SetTextFunctionBody = @"
				    var stringValue = value.toLowerCase();
				    var preparedValue;
				    if(stringValue == 'true') {
                        preparedValue = true;
				    }
				    else if(stringValue == 'false') {
                        preparedValue = false;
				    }
				    else {
					    this.LogOperationError('A Boolean property can only be set to True or False. The ""' + value + '"" value is invalid.');
                        return;
				    }
				    this.control.SetChecked(preparedValue);				
				    this.control.RaiseValueChanged();								    
				";
				return result;
			}
		}
		#endregion
	}
	public class JSASPxRadioButtonTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxRadioButtonList";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.IsEnabledFunctionBody = @"
				return true;
				";
				result.SetTextFunctionBody = @"
				if(value == 'True'){
					ASPx.ChkOnClick(this.control.name);
				}
				else {
					this.LogError('It is impossible to set false value to radio button');
				}			
				";
				result.GetTextFunctionBody = @"
				return this.control.GetValue();
				";
				return result;
			}
		}
		#endregion
	}
	internal class JSASPxDefaultGridViewColumnTestControl : IJScriptTestControl {
		public const string ClassName = "DefaultGridViewColumn";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.AdditionalParametersDeclaration.Add("fieldName");
				result.GetTextFunctionBody = @"
                var rowEditElement = this.control.GetEditingRow(this.control);
                if(rowEditElement){
                    return this.control.GetEditValue(fieldName);
                }
                else {
                    this.LogOperationError('Error get cell text is not supported');
                }
				";
				result.SetTextFunctionBody = @" 
				    this.control.SetEditValue(fieldName, value);
				";
				result.IsEnabledFunctionBody = @"
                var rowEditElement = this.control.GetEditingRow(this.control);
                if(rowEditElement){
                    return this.control.GetEditor(fieldName).GetEnabled() && !this.control.GetEditor(fieldName).readOnly;
                }
                    return false;
                ";
				result.AdditionalFunctions = new JSFunctionDeclaration[] { new JSFunctionDeclaration("IsDefaultGet",null,@"
                ")};
				return result;
			}
		}
		#endregion
	}
	internal class JSASPxGridLookupTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxGridLookup";
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.GetTextFunctionBody = @"
                    return this.control.GetText();
                ";
				result.SetTextFunctionBody = @"
                    this.control.ShowDropDownArea();
                    var clientGridView = window[this.control.name + '_DDD_gv'];
                    if(clientGridView) {
                        var info = value.split(';');
                        var targetCellText;
                        var columnCaption;
                        if(info[1]){
                            columnCaption = info[0];
                            targetCellText = info[1];
                        }else{
                            targetCellText = value;
                        }

                        var rowCount = clientGridView.GetVisibleRowsOnPage();
                        this.LogTraceMessage('rowCount = ' + rowCount);
                        if((rowCount - 1) < 0) {
                            this.LogOperationError( 'The grid contains ' + rowCount + ' rows');
                            return;
                        }

                        var columnIndex = -1;
                        if(columnCaption){
                            var columnsText = '';
                            for(var i = 0; i < clientGridView.columns.length; i++) {
                                var column = clientGridView.columns[i];
                                if(column.fieldName == columnCaption){
                                    columnIndex = column.index;
                                    break;
                                }else{
                                    columnsText += column.fieldName;
                                    if(i + 1 < clientGridView.columns.length){
                                        columnsText += ',';
                                    }
                                }
                            }
                            if(columnIndex == -1){
                                this.LogOperationError( 'The grid not contains ' + columnCaption + ' column. Columns: ' + columnsText + ' ');
                            }
                        }else{
                            columnIndex = 0
                        }

                        var targetRowElement;
                        var cellTexts = '';
                        for(var i = 0; i < rowCount; i++) {
                            var rowElement = clientGridView.GetDataRow(clientGridView.GetTopVisibleIndex() + i);
                            var cellText = rowElement.cells[columnIndex].innerText;
                            if(cellText == targetCellText){
                                targetRowElement = rowElement;
                                break;
                            }else{
                                cellTexts += cellText;
                                if(i + 1 < rowCount){
                                    cellTexts += ',';
                                }
                            }
                        }

                        if(targetRowElement){
                            //clientGridView.SelectRow(clientGridView.GetTopVisibleIndex() + targetRowElement.rowIndex - 1, true);
                            targetRowElement.click();
                        }else{
                            this.LogOperationError( 'The grid not contains ' + targetCellText + ' cell. Cells: ' + cellTexts + ' ');
                        }
                    }
                ";
				result.ActFunctionBody = @"
                    if(!value){
                        try{
                            this.control.ShowDropDownArea();
                        }catch(e){
                            if(!this.IsEnabled()){
                                this.LogOperationError( 'The action: ShowDropDownArea is not executed. ');
                            }else{
                                throw e;
                            }
                        }
                    }else{
                        var button = this.control.GetButton(0);
                        if(button.title == value && !button.disabled){
                            try{
                                button.onclick();
                            }catch(e){
                                if(!this.IsEnabled()){
                                    this.LogOperationError( 'The action: ' + value + ' is not executed. ');
                                }else{
                                    throw e;
                                }
                            }               
                        }else{
                            this.LogOperationError( 'The action: ' + value + ' is not supported. ');
                        }
                    }
                ";
				return result;
			}
		}
	}
	internal class JSASPxSimpleLookupTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxSimpleLookup";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.GetTextFunctionBody = @"
                    this.InitListBox();
				    return this.listBox.GetText();
				";
				result.SetTextFunctionBody = @" 
                    this.InitListBox();
				    this.listBox.SetText(value);
				";
				result.ActFunctionBody = @"
				    if(value == undefined || value == null)	{
					    this.LogOperationError('The default Action is not defined for the ' + this.caption + ' control');
					    return;
				    }
                    var dropDown = window[this.id + '_DD'];
                    if(value == dropDown.cpNewButtonCaption) {
                        var button = dropDown.GetButton(0);
                        if(button) {
                            button.onclick();
                            return;
                        }
                    }
                    if(value == 'Clear') {
                        var button = dropDown.GetButton(1);
                        if(button) {
                            button.onclick();
                            return;
                        }
                    }
				    this.LogOperationError('The ""' + value + '"" Action is not defined for the ' + this.caption + ' control');
				";
				result.IsEnabledFunctionBody = @"
                    var dropDown = window[this.control.id + '_DD'];
                    if(dropDown) {
                        //ConditionalAppearance in GridView not used PropertyEditor, the rules applyed to grid
                        this.InitListBox();
                        return this.listBox.IsEnabled() && dropDown.enabled;
                    }
                    else {
                        return this.control.enabled;
                    }

                ";
				result.AdditionalFunctions = new JSFunctionDeclaration[] { new JSFunctionDeclaration("InitListBox", null ,@"
                    var f = this.inherit.prototype.baseInitControl;
                    f.call(this);
                    if(this.error) {
                        return;
                    }
                    var listBoxId = this.id + '_DD';
                    if(window[listBoxId]) {
                        this.listBox = new ASPxComboBox(listBoxId, this.caption);
                    }
                    else {
                        this.listBox = this.control;
                    }
                    this.listBox.targetErrorControl = this;
                ")};
				return result;
			}
		}
		#endregion
	}
	internal class JSASPxMemoTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxMemo";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.GetTextFunctionBody = @"	
					var value = this.control.GetValue();			
					return (value == null) ? '' : value;
					";
				result.SetTextFunctionBody = ASPxStandardTestControlScriptsDeclaration.defaultAspxSetTextFunctionBody;
				return result;
			}
		}
		#endregion
	}
	internal class JSASPxLookupProperytEditorTestControl : IJScriptTestControl, ISupportAdditionalParametersTestControl {
		public const string ClassName = "ASPxLookupPropertyEditor";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.AdditionalParametersDeclaration.Add("searchActionName");
				result.IsEnabledFunctionBody = @"
					return this.GetEditControl().IsEnabled();
				";
				result.SetTextFunctionBody = @"
                    this.GetEditControl().SetText(value);
				";
				result.GetTextFunctionBody = @"
					return this.GetEditControl().GetText();
				";
				result.ActFunctionBody = @"
				if(!value) {
					value = 'Find';
				}
                var buttonEditClientControl = window[this.id + '_Edit'];
                if(value == buttonEditClientControl.cpFindButtonCaption) {
                    var button = buttonEditClientControl.GetButton(0);
                    if(button) {
                        button.onclick();
                        return;
                    }
                }
                if(value == 'Clear') {
                    var button = buttonEditClientControl.GetButton(1);
                    if(button) {
                        button.onclick();
                        return;
                    }
                }
				this.LogOperationError('Unrecognized editor Action: ' + value);
				";
				result.AdditionalFunctions = new JSFunctionDeclaration[] {
					new JSFunctionDeclaration("GetEditControl", "", @"
					if(this.editControl == null) {
                        var editControlId = this.id + '_Edit';
                        if(window[editControlId]) {
                            this.editControl = new ASPxTextBox(editControlId, this.caption);
                        }
                    }
                    this.editControl.targetErrorControl = this;
					return this.editControl;
					"),
					new JSFunctionDeclaration("GetSearchActionName", "", @"
                        return this.searchActionName;
                    ")
				};
				return result;
			}
		}
		#endregion
		#region ISupportAdditionalParametersTestControl Members
		public ICollection<string> GetAdditionalParameters(object control) {
			return new string[] { "'" + ((ASPxLookupPropertyEditor)control).GetSearchActionName() + "'" };
		}
		#endregion
	}
	public class JSASPxButtonEditTestControl : IJScriptTestControl {	
		public const string ClassName = "ASPxButtonEdit";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		static JSASPxButtonEditTestControl() {
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.IsEnabledFunctionBody = @"
                    return !this.control.GetButton(0).isDisabled;
				";
				result.GetTextFunctionBody = @"
				return this.control.GetValue();
				";
				result.ActFunctionBody = @"
				if(this.control.GetButton(0).isDisabled){
					this.LogOperationError('Cannot execute the Edit Action for the ' + this.caption + ' control');
				}else{
					ASPx.BEClick(this.control.name, 0);
				}
				";
				return result;
			}
		}
		#endregion
	}
	public class JSASPxGridListEditorTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxGridListEditor";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				TableTestControlScriptsDeclaration result = new TableTestControlScriptsDeclaration();
				result.GetCellValueFunctionBody = @" 
                    this.traceMessages = '';
                    var result = '';
                    var clientGridView = window[this.control.id];
                    if(clientGridView) {
	                    var rowCount = this.GetTableRowCount();
                        this.LogTraceMessage('rowCount = ' + rowCount);
	                    if(row > rowCount - 1) {
		                    this.LogOperationError( 'The grid contains ' + rowCount + ' rows');
		                    return result;
	                    }
	                    var rowElement = clientGridView.GetEditingRow(clientGridView);
	                    var testControl = null;
	                    if(rowElement && (row == (rowElement.sectionRowIndex - 1))) {
		                    testControl = this.GetColumnEditor(rowElement.sectionRowIndex - 1, columnCaption);		
                            if(testControl) {
                                this.LogTraceMessage('{87C021B2-57DD-43B8-9FA5-37050C2B9CBA}');
		                        result = testControl.GetText();
		                        this.CheckEditorError(testControl);
                            }
                            else {
                                this.LogOperationError('Cannot find the ' + columnCaption + ' testControl.');
                            }
	                    }
	                    else {
		                    rowElement = clientGridView.GetDataRow(clientGridView.GetTopVisibleIndex() + row);
	                        if(rowElement) {
                                this.LogTraceMessage('{298C2DB0-F1BD-43F5-8C78-CA92BD377184}');
                                testControl = this.GetColumnEditor(clientGridView.GetTopVisibleIndex() + row, columnCaption);
                                if(testControl) {
    		                        if(testControl.className != '" + JSASPxDefaultGridViewColumnTestControl.ClassName + @"') {
                                        this.LogTraceMessage('{5262DF3A-F5F9-4DD7-B886-CA31D9D0372B}');
	        	                        result = testControl.GetText();
		                                this.CheckEditorError(testControl);
		                            }
		                            else {
                                        this.LogTraceMessage('The JS test control is " + JSASPxDefaultGridViewColumnTestControl.ClassName + @", get cell text.');
		                                result = this.GetCellText(rowElement, columnCaption);
		                            }
                                }
                                else {
                                    this.LogTraceMessage('The JS test control not fount, get cell text.');
                                    result = this.GetCellText(rowElement, columnCaption);                                    
                                }
    	                    }
                            else {
                                this.LogTraceMessage('The rowElement is not find. Maybe this property is a group.');
                            }
	                    }
                    }
                    else {
                        this.LogOperationError('eval returns undefined for ""' + this.control.id +'"".');
                    }
                    return result;
                ";
				result.SetCellValueFunctionBody = @"
                    var result = '';
                    var clientGridView = window[this.control.id];
                    if(clientGridView) {
	                    var rowCount = this.GetTableRowCount();
	                    if(row > rowCount - 1) {
		                    this.LogOperationError( 'The grid contains ' + rowCount + ' rows');
		                    return result;
	                    }			
	                    rowElement = clientGridView.GetEditingRow(clientGridView);
	                    if(rowElement) {			
		                    var testControl = this.GetColumnEditor(rowElement.sectionRowIndex - 1, columnCaption);		
		                    if(testControl) {
			                    testControl.SetText(value);
			                    isCellFound = true;
			                    this.CheckEditorError(testControl);
		                    }
	                    }
	                    else {
		                    this.LogOperationError('All the rows are read-only');
	                    }
                    }
               ";
				result.BeginEditFunctionBody = @"
                    this.InlineEdit(null, row, null, true);
                ";
				result.GetCellControlFunctionBody = @"
					var result = '';
					var clientGridView = window[this.control.id];
					if(clientGridView) {
						if(clientGridView.cpInlineEditMode === 'Batch') {
                            var testColumns = clientGridView.cpTestColumns;
                            var testColumn = testColumns.GetColumnByColumnCaption(columnCaption);
                            if(testColumn) {
                                var testControl = new TestBatchCellEditor(this.control.id, row, testColumn.fieldName);		
                                return testControl;
                            }
						}
						rowElement = clientGridView.GetEditingRow(clientGridView);
						if(rowElement) {
                            var rowIndex = rowElement.sectionRowIndex - 1;	
                            if(clientGridView.cpInlineEditMode == 'EditFormAndDisplayRow') {
                                rowIndex = rowIndex - 1;
                            }
                            if(clientGridView.cpInlineEditMode == 'Inline') {
                                var testColumns = clientGridView.cpTestColumns;
                                var testColumn = testColumns.GetColumnByColumnCaption(columnCaption);
                                if(testColumn) {
                                    var autoFilterEditor = this.GetAutoFilterEditorForColumn(testColumns, testColumn);
                                    if(autoFilterEditor && rowIndex > 0 && clientGridView.cpInlineEditMode == 'Inline') {
                                          rowIndex = rowIndex - 1;
                                    }
                                }
                            }
							var testControl = this.GetColumnEditor(rowIndex, columnCaption);		
							return testControl;
						}
						else {
							this.LogOperationError('All rows are read-only.');
						}
					}
				";
				result.EndEditFunctionBody = @"this.InlineUpdate(null, -1, null);";
				result.GetColumnIndexFunctionBody = @"
                    var clientGridView = window[this.control.id];
                    var testColumns = clientGridView.cpTestColumns;
                    return testColumns.GetColumnIndexByColumnCaption(columnCaption) + this.GetGroupColumnCount();
				";
				result.GetColumnsCaptionsFunctionBody = @"
                    var result = '';
                    var clientGridView = window[this.control.id];
                    var headersRow = clientGridView.GetHeaderRow(0);
                    for(var i = 0; i < headersRow.cells.length; i++) {
                        var columnHeaderCell = headersRow.cells[i];
                        if(!this.IsActionColumn(columnHeaderCell)) {
	                        var caption = this.RemoveLineBrakes(this.Trim(columnHeaderCell.innerText));
	                        if(result == '') {
		                        result = caption;
	                        } 
	                        else {
		                        result += ';' + caption;
	                        }
                        }
                    }
                    return result;
				";
				result.GetTableRowCountFunctionBody = @"				
					var clientGridView = window[this.control.id];
					return clientGridView.GetVisibleRowsOnPage();
				";
				result.ClearSelectionFunctionBody = @"this.SelectAll('false');";
				result.SelectRowFunctionBody = @"this.SetRowSelection(row, true);";
				result.UnselectRowFunctionBody = @"this.SetRowSelection(row, false);";
				result.IsRowSelectedFunctionBody = @"
					var rowCount = this.GetTableRowCount();
					if(row > rowCount - 1) {
						this.LogOperationError( 'The grid contains ' + rowCount + ' rows');
						return '';
					}			
					var clientGridView = window[this.control.id];
					return clientGridView._isRowSelected(clientGridView.GetTopVisibleIndex() + row)
				";
				result.IsSupportSelectionBody = @"
	                var clientGridView = window[this.control.id];
	                return clientGridView.cpCanSelectRows;
				";
				result.ExpandGroupsFunctionBody = @"
					var clientGridView = window[this.control.id];
					var rowCount = clientGridView.GetVisibleRowsOnPage();
					//B38638
					for(var i = 0; i < rowCount; i++) {
						if(clientGridView.IsGroupRow(i)) {
							clientGridView.ExpandAll();
							return;
						}
					}
				";
				result.IsGroupedFunctionBody = @"
					var clientGridView = window[this.control.id];
					var rowCount = clientGridView.GetVisibleRowsOnPage();
					//B38638
					for(var i = 0; i < rowCount; i++) {
						if(clientGridView.IsGroupRow(i)) {
							return true;
						}
					}
                    return false;
                ";
				result.ExecuteActionFunctionBody = @"
                    var action = this.GetAction(actionName, row, column);
                    if(action) {
                        if(action.Act) {
                            action.Act();
                        }
			            else if(action.DoClick) {
			                action.DoClick();
			            }
                        else {
                            action.click();
                        }
                    }
					";
				result.AdditionalFunctions = new JSFunctionDeclaration[] { 
#region ExecuteTable
								new JSFunctionDeclaration("InlineEdit", "param, rowIndex, columnIndex, isOptional", @"
					        var clientGridView = this.GetClientGridView(rowIndex);
                            if(clientGridView) {
                                if(clientGridView.cpInlineEditMode === 'Batch') { return; }
                                if(rowIndex == -1) {
                                    if(!clientGridView.GetEditingRow(clientGridView))
    					                this.LogOperationError('The grid editing row is undefined');
                                } else {
                                    var control = this.FindRowCommandControl(clientGridView, clientGridView.GetTopVisibleIndex() + rowIndex, 'Edit');
                                    if(control) {
                                        control.click();
                                    }
                                    else if(!isOptional) {
				                            this.LogOperationError('The \'Edit\' Action is not available');
                                    }
                                }
                            }
				        "),
						new JSFunctionDeclaration("InlineUpdate", "param, rowIndex, columnIndex", @"
					        var clientGridView = this.GetClientGridView(rowIndex);
                            if(clientGridView){
                                if(clientGridView.cpInlineEditMode === 'Batch') { 
                                    clientGridView.batchEditApi.EndEdit();
                                    return; 
                                }
                                var control = this.FindRowCommandControl(clientGridView, clientGridView.GetTopVisibleIndex() + rowIndex, 'Update');
                                if(control) {
                                    control.click();
                                }
                                else {
		                            this.LogOperationError('The \'Edit\' Action is not available');
                                }
                            }
				        "),
						new JSFunctionDeclaration("InlineCancel", "param, rowIndex, columnIndex", @"
					        var clientGridView = this.GetClientGridView(rowIndex);
                            if(clientGridView){
                                var control = this.FindRowCommandControl(clientGridView, clientGridView.GetTopVisibleIndex() + rowIndex, 'Cancel');
                                if(control) {
                                    control.click();
                                }
                                else {
		                            this.LogOperationError('The \'Edit\' Action is not available');
                                }
                            }
				        "),
						new JSFunctionDeclaration("InlineNew", null, @"
					        var clientGridView = this.GetClientGridView(-1);
                            if(clientGridView){
                                var control = this.FindRowCommandControl(clientGridView, this.GetGroupColumnCount(), 'New');
                                if(control) {
                                    control.click();
                                }
                                else {
		                            this.LogOperationError('The \'Edit\' Action is not available');
                                }
                            }
				        "),
						new JSFunctionDeclaration("BatchSave", null, @"
					        var clientGridView = this.GetClientGridView(-1);
                            if(clientGridView){
                                clientGridView.UpdateEdit();
                            }
				        "),
						new JSFunctionDeclaration("BatchCancel", null, @"
					        var clientGridView = this.GetClientGridView(-1);
                            if(clientGridView){
                                clientGridView.CancelEdit();
                            }
				        "),
						new JSFunctionDeclaration("DetailRowAction", "param, rowIndex", @"
                            var gridView = window[this.control.id];
                            if(param === 'Expand')
                                gridView.ExpandDetailRow(rowIndex);
                            else if(param === 'Collapse')
                                gridView.CollapseDetailRow(rowIndex);
                            else
                                this.LogOperationError('Unknown Detail Row Action is found: \'' + param + '\'');
				        "),
						new JSFunctionDeclaration("GetAutoFilterEditorForColumn", "testColumns, testColumn", @"
					        var clientGridView = this.GetClientGridView(-1);
                            if(clientGridView){
                                    var fieldName = testColumn.fieldName;
                                    var column = null;
                                    for(var x = 0; x<clientGridView.columns.length; x++) {
                                        if(clientGridView.columns[x].fieldName == fieldName){
                                            column = clientGridView.columns[x];
                                            break;
                                        }
                                    }
                                    if(column){
                                        return clientGridView.GetAutoFilterEditor(column);
                                    }else{
                                        return null;
                                    }
                            }
				        "),
						new JSFunctionDeclaration("SetTableFilter", "param, rowIndex, columnIndex", @"
					        var clientGridView = this.GetClientGridView(-1);
                            if(clientGridView) {
                                    var testColumns = clientGridView.cpTestColumns;
                                    var separator = param.indexOf('=');
                                    var columnCaption = param.substring(0, separator);
                                    var paramValue = param.substring(separator + 1, param.length);
                                    if(paramValue == '\'\'') {
                                        paramValue = '';
                                    }
                                    var fieldName = null;
                                    for(var x = 0; x < testColumns.columns.length; x++) {
                                        if(testColumns.columns[x].columnCaption == columnCaption){
                                            fieldName = testColumns.columns[x].fieldName;
                                            break;
                                        }
                                    }
                                    if(fieldName == null) {
                                        this.LogOperationError( 'The \''+ columnCaption + '\' column was found, but it is invisible.');
                                    }
                                    var column = null;
                                    for(var x = 0; x < clientGridView.columns.length; x++) {
                                        if(clientGridView.columns[x].fieldName == fieldName) {
                                            column = clientGridView.columns[x];
                                            break;
                                        }
                                    }
                                    var autoFilterEditor = clientGridView.GetAutoFilterEditor(column);
                                    if(autoFilterEditor) {
    			                        autoFilterEditor.SetValue(paramValue);
	    		                        this.CheckEditorError(autoFilterEditor);
                                        autoFilterEditor.RaiseValueChanged(true);
                                    }
                                    else {
                                        this.LogOperationError( 'The \''+ fildName + '\' column was found, but it is invisible.');
                                    }
                            }
				        "),
						new JSFunctionDeclaration("SortByColumn", "columnCaption", @"
                            var columnFound;
                            var clientGridView = window[this.control.id];
                            var testColumns = clientGridView.cpTestColumns;
                            for(var i = 0; i < clientGridView.columns.length; i++) {
                                var column = clientGridView.columns[i];
                                var fieldName = testColumns.GetColumnFieldNameByColumnCaption(columnCaption);
                                if(fieldName == '') {
                                    this.LogOperationError( 'The \''+ columnCaption + '\' column was not found.');
                                }
                                if(column.fieldName == fieldName) {
                                    if(this.GetColumnIndex(columnCaption) != -1) {
                                        clientGridView.SortBy(column);
                                    }
                                    else {
                                        this.LogOperationError( 'The \''+ columnCaption + '\' column was found, but it is invisible.');
                                    }
                                    columnFound = true;
                                }
                            }                                
                            if(!columnFound) {
                                this.LogOperationError( 'The grid does not contain the ' + columnCaption + ' column.');
                            }										                    
				        "),					
						new JSFunctionDeclaration("SelectAll", "checked", @"
                            if(!checked) {
                                checked = 'true';
                            }
                            var gridView = window[this.control.id];
                            if(gridView) {
                                if(checked.toLowerCase() === 'true') {
                                    gridView.SelectRows();
                                }
                                else {
                                    if(checked.toLowerCase() === 'false') {
                                        gridView.UnselectRows();
                                    }
                                    else {
                                        this.LogOperationError(checked + ' parameter is not allowed. Use True or False instead.');
                                    }
                                }
                            }
				        "),
						new JSFunctionDeclaration("SetPageSize", "size" , @"
                            var pageSizeMenu = window[this.control.id + '_DXPagerBottom_PSP'];
                            var availableItems = [];
                            for(var i=0;i< pageSizeMenu.rootItem.items.length;i++){
                                var item = pageSizeMenu.rootItem.items[i];
                                availableItems.push(item.name);
                                if(item.name == size) {
                                    pageSizeMenu.DoItemClick(item.indexPath);
                                    return;
                                }
                            }  
                            var availableItemsString = '';
                            for(var i=0;i< availableItems.length;i++){
                                availableItemsString += availableItems[i] + '\r\n';
                            }
                            this.LogOperationError('Page size: ' + size + ' is not found. Available items are: ' + availableItemsString );
				        "),
						new JSFunctionDeclaration("SetPage", "pageIndex", @"
                            pageIndex--;
                            if(pageIndex>= 0) {
                                var clientGridView = window[this.control.id];
                                clientGridView.GotoPage(pageIndex);	
                            }
                            else {
                                this.LogOperationError('Page index must greater than 1.');
                            }
				        "),
						 new JSFunctionDeclaration("ExpandGroup", "groupRowText", @"this.ProcessGroup(groupRowText, 0);"),
						 new JSFunctionDeclaration("CollapseGroup", "groupRowText", @"this.ProcessGroup(groupRowText, 1);"),
	#endregion                        
						 new JSFunctionDeclaration("GetClientGridView", "rowIndex", @"
                            var clientGridView = window[this.control.id];
					        if(!clientGridView) this.LogOperationError('It is impossible to perform action. The grid is empty');
					        var rowCount = this.GetTableRowCount();
					        if(rowIndex != -1 && rowIndex > rowCount - 1) {
                                this.LogOperationError('The grid contains ' + rowCount + ' rows');
						        return '';
					        }
                            return clientGridView;
                        "),
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
	                            if(this.control && this.control.id) {
		                            var clientGridView = window[this.control.id];
		                            eval(clientGridView.cpInitTestColumns);
	                            }
				        "),
						new JSFunctionDeclaration("GetCheckBoxStateOrImageTitle", "cell", @"
                            if(cell && cell.tagName && (cell.tagName.toUpperCase() == 'IMG' || (cell.tagName.toUpperCase() == 'INPUT' && cell.type && cell.type == 'checkbox'))) {
	                            if(cell.tagName.toUpperCase() == 'IMG' && !IsNull(cell.title)) {
		                            return cell.title != '' ? cell.title : cell.alt;
	                            }
	                            if(cell.tagName.toUpperCase() == 'INPUT' && cell.type == 'checkbox') {
		                            return cell.checked ? 'True' : 'False';
	                            }
                            }
                            else {
	                            for(var i=0;i<cell.childNodes.length;i++){
		                            var result = this.GetCheckBoxStateOrImageTitle(cell.childNodes[i]);
		                            if(result) {
			                            return result;
		                            }
	                            }
                            }
				        "),
						new JSFunctionDeclaration("IsActionColumn", "columnHeaderCell", @"
                            var result = false;
                            var clientGridView = window[this.control.id];
                            // var headersRow = document.getElementById(this.control.id + '_DXHeadersRow0');
                            if(columnHeaderCell.id) {
		                        var column = clientGridView.GetColumn(clientGridView.tryGetNumberFromEndOfString(columnHeaderCell.id).value);
		                        if(!column || !column.fieldName || column.fieldName == '') {
    			                    result =- true;
		                        }
                            } 
                            else {
   			                    result = true;
                            }
                            return result;
				        "),
						new JSFunctionDeclaration("GetActionColumnCount", null, @"
                            var clientGridView = window[this.control.id];
                            var count = 0;
                            if(clientGridView) {

	                            //Bug in clientGridView.GetColumnsCount()
	                            //var columnCount = clientGridView.GetColumnsCount();

	                            var headersRow = document.getElementById(this.control.id + '_DXHeadersRow0');
	                            var columnCount = headersRow.cells.length;
	                            for(var i = 0; i < columnCount; i++) {
                                    if(this.IsActionColumn(headersRow.cells[i]))
                                        count++;
	                            }
                            }
                            return count;
				        "),
						 new JSFunctionDeclaration("GetGroupColumnCount", null, @"
                            var clientGridView = window[this.control.id];
                            var count = 0;
                            if(clientGridView) {
                                var headersRow = document.getElementById(this.control.id + '_DXHeadersRow0');
                                var columnCount = headersRow.cells.length;
                                for(var i = 0; i < columnCount; i++) {
                                    if(headersRow.cells[i].id) {
    	                                var column = clientGridView.GetColumn(clientGridView.tryGetNumberFromEndOfString(headersRow.cells[i].id).value);
	                                    if(!column) {
		                                    count++;
	                                    }
                                    }
                                    else {
		                                count++;
                                    }
                                }
                            }
                            return count;
				        "),
						 new JSFunctionDeclaration("FindRowCommandControl", "clientGridView, rowIndex, commandName", @"
                            var dataRow = null;
                            if(rowIndex != -1) {
                                dataRow = clientGridView.GetDataRow(rowIndex);
                            }
                            if(!dataRow) {
                                if(clientGridView.cpInlineEditMode == 'EditFormAndDisplayRow' || clientGridView.cpInlineEditMode == 'EditForm') {
                                    var editFormTable = window[this.id + '_DXEFT'];
                                    dataRow = editFormTable.rows[editFormTable.rows.length-1];
                                }
                                else {
                                    dataRow = clientGridView.GetEditingRow(clientGridView);
                                }
                            }
                            if(!dataRow) dataRow = clientGridView.GetEditingRow(clientGridView);
                            if(!dataRow) dataRow = clientGridView.GetMainTable();
                            for(var i = 0; i < dataRow.cells.length; i++) {
                                var dataRowCell = dataRow.cells[i];
                                if(!dataRowCell.className) continue;
                                if(dataRowCell.className.indexOf('dxgvCommandColumn') != 0 && dataRowCell.className != 'dxgv') continue;
                                for(var j = 0; j < dataRowCell.childNodes.length; j++) {
	                                var currentColumnCommandName = dataRowCell.childNodes[j].innerText;
	                                if(!currentColumnCommandName || currentColumnCommandName == '') {
		                                currentColumnCommandName = dataRowCell.childNodes[j].alt;
	                                }
	                                if(currentColumnCommandName == commandName) {
		                                return dataRowCell.childNodes[j];
	                                }else {
	                                    if(dataRowCell.childNodes[j].childNodes) {
	                                        for(var t = 0; t < dataRowCell.childNodes[j].childNodes.length; t++) {
	                                            if(dataRowCell.childNodes[j].childNodes[t].alt == commandName) {
	                                                return dataRowCell.childNodes[j]
    	                                        }
	                                        }
	                                    }
	                                }
                                }
                            }
                            return null;
				        "),
						new JSFunctionDeclaration("FilterColumn", "columnCaption, filterCriteria", @"
                            var clientGridView = window[this.control.id];
                            var testColumns = clientGridView.cpTestColumns;
                            var columnIndex = testColumns.GetColumnIndexByColumnCaption(columnCaption);
                            if(columnIndex != -1) {
                                clientGridView.ApplyHeaderFilterByColumn(columnIndex, filterCriteria);
                            }
                            else {
                                this.LogOperationError( 'The grid does not contain the ' + columnCaption + ' column.');
                            }										                    
				        "),
						new JSFunctionDeclaration("GetButtonControl", "cell", @"
			                if(cell.childNodes.length > 0 && cell.childNodes[0].id) {
			                    var buttonControl = window[cell.childNodes[0].id + '_View_SHB'];
			                    return buttonControl;
			                }
			                return null;
                        "),
						new JSFunctionDeclaration("FindActionColumnIndex", "row, actionName", @"
                            var clientGridView = window[this.control.id];
                            var actionColumnCount = this.GetActionColumnCount();
                            for(var i = 0; i < actionColumnCount; i++) {
                                var actionColumnIndex = i - actionColumnCount + 1;
                                var cellValue = this.GetCellValue(row, actionColumnIndex);
                                if(cellValue == actionName) {
	                                return i;
                                }
                            }
			                var rowElement = clientGridView.GetDataRow(clientGridView.GetTopVisibleIndex() + row);
			                for(var i = 0; i < rowElement.cells.length - actionColumnCount; i++) {
			                    var cell = rowElement.cells[actionColumnCount + i];
			                    var buttonControl = this.GetButtonControl(cell);
			                    if(buttonControl && buttonControl.GetText() == actionName) {
			                        return actionColumnCount + i - 1;
			                    }
			                }
                            return -1;
				        "),
						new JSFunctionDeclaration("SetRowSelection", "row, isSelected", @"
                            var rowCount = this.GetTableRowCount();
                            if(row > rowCount - 1) {
                                this.LogOperationError( 'The grid contains ' + rowCount + ' rows');
                                return;
                            }
                            var clientGridView = window[this.control.id];
                            clientGridView.SelectRows(clientGridView.GetTopVisibleIndex() + row, isSelected);
			            "),
						 new JSFunctionDeclaration("ProcessGroup", "groupRowText, actionIndex", @"
                            var clientGridView = window[this.control.id];
                            var i = 0;
                            var foundedGroupRows = [];
                            while(clientGridView.GetItem(i)) {
                                var groupRow = clientGridView.GetGroupRow(i++);
                                if(groupRow) {
                                    var oldPreparedInnerText = this.RemoveLineBrakes(this.Trim(groupRow.innerText));
                                    var preparedInnerText = this.Trim(groupRow.textContent).replace(/(\n)*(\t)*/gm, '');
                                    foundedGroupRows.push(preparedInnerText);
                                    //var preparedTextContentForNewColumnMode = this.Trim(groupRow.textContent).replace('\n\t\t\n\t\t\t\n\t\t\t\t','').replace('\n\t\t\t\n\t\t\n\t','');
	                                if(preparedInnerText == groupRowText || oldPreparedInnerText == groupRowText) {
                                        switch(actionIndex) {
                                            case 0: clientGridView.ExpandRow(i - 1); break;
                                            case 1: clientGridView.CollapseRow(i - 1); break;
                                        }		                                
		                                return;
	                                }
                                }
                            }
                            this.LogOperationError( 'The grid doesn\'t contain the \'' + groupRowText + '\'' + ' group row. Found:\r\n' + foundedGroupRows.join('\r\n') + '\r\n');
                            return;
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
				        "),
						 new JSFunctionDeclaration("RemoveLineBrakes", "str", @"
                            str = str.replace(/\r/g,'');
                            str = str.replace(/ \n+/g,' ');
                            str = str.replace(/\n+ /g,' ');
                            str = str.replace(/\n+/g,' ');
                            str = str.replace(/ (<BR>)+/g,' ');
                            str = str.replace(/(<BR>)+ /g,' ');
                            str = str.replace(/(<BR>)+/g,' ');
                            return str;
				        "),
						 new JSFunctionDeclaration("ColumnEditorAction", "columnCaption, columnAction", @"
                            var clientGridView = window[this.control.id];
                            if(!clientGridView) {
                                this.LogOperationError('Cannot perform an Action. The grid doesn\'t contain any records');
                                return;
                            }
                            var rowElement = clientGridView.GetEditingRow(clientGridView);
                            if(!rowElement) {
                                this.LogOperationError('Cannot perform an Action. The edit row is disabled.');
                                return;
                            }
                            var columnIndex = this.GetColumnIndex(columnCaption);
                            if(columnIndex == -1) {
                                this.LogOperationError('Cannot find the ' + columnCaption + ' column.');
                                return;
                            }
                            var testControl = this.GetColumnEditor(rowElement.sectionRowIndex - 1, columnCaption);
                            if(testControl) {
                                testControl.Act(columnAction)
                                this.CheckEditorError(testControl);
                            }
				        "), 
						new JSFunctionDeclaration("GetColumnEditor", "rowIndex, columnCaption", @"
                            var clientGridView = window[this.control.id];
					        var testControl = this.GetColumnEditorCore(-2147483647, columnCaption);
					        if(!testControl) {
						        testControl = this.GetColumnEditorCore(clientGridView.GetTopVisibleIndex() + rowIndex, columnCaption);
					        }
					        if(!testControl) {
                                var columnIndex = this.GetColumnIndex(columnCaption);
                                if(columnIndex == -1) {
                                    this.LogOperationError('Cannot find the ' + columnCaption + ' column.');
                                    return;
                                }
                            }
                            return testControl;  					
				        "),
						new JSFunctionDeclaration("GetColumnEditorCore", "rowIndex, columnCaption", @"
                            try {
                                var clientGridView = window[this.control.id];
                                var testColumns = clientGridView.cpTestColumns;
                                var testColumn = testColumns.GetColumnByColumnCaption(columnCaption);
                                if(testColumn) {
                                    var editorCollectionId = 'cp' + testColumn.fieldName.replace(/\./g, '_') + testColumn.indexInGrid + '_RowIndexToEditorIdMap';
                                    this.LogTraceMessage('Get JS test control collection, id:' + editorCollectionId);
                                    var editorInfo = clientGridView[editorCollectionId][rowIndex];

                                    var testControl;
                                    var info;
                                    if(editorInfo) {
	                                    info = editorInfo.split(';');
	                                    var editorId = info[0];
	                                    var viewModeOnly = info[1];
	                                    testControl = testColumn.CreateTestControlWithId(editorId, viewModeOnly);
                                    }

                                    if(testColumn.editorTestClassNameEditMode == '" + JSASPxSimpleLookupTestControl.ClassName + @"') {
	                                    testControl.GetText();
	                                    if(testControl.error != null) {
		                                    var findLookupColumn = new TestColumn(testColumn.fieldName, '" + JSASPxLookupProperytEditorTestControl.ClassName +@"', testColumn.editorId.replace('$DropDown', ''), testColumn.indexInGrid);
		                                    var findLookupTestControl = findLookupColumn.CreateTestControlWithId(findLookupColumn.editorId);
		                                    findLookupTestControl.GetText();
		                                    if(findLookupTestControl.error == null) {
			                                    testControl = findLookupTestControl;
		                                    }
	                                    }
                                    }
                                    return testControl;
                                }
                                this.LogOperationError('Cannot find the editor for the ' + columnCaption + ' column.');
                            } 
                            catch(e) { }
                            return null;
				        "), 
						 new JSFunctionDeclaration("CheckEditorError", "testControl", @"
                            if(testControl.error != null) {
	                            this.LogOperationError('Error in the editor for the ' + testControl.caption + ' property. Error message: ' + testControl.error);
                            }
				        "),
						 new JSFunctionDeclaration("GetCellText", "rowElement, columnCaption", @"
                                this.LogTraceMessage('Column ' + columnCaption + ' editor was not found. Gettign text from cell.');

                                var clientGridView = window[this.control.id];
                                var testColumns = clientGridView.cpTestColumns;
                                var columnFieldName = testColumns.GetColumnByColumnCaption(columnCaption).fieldName;
                                var columnIndex = clientGridView.GetColumnByField(columnFieldName).index;
                                var cell = clientGridView.GetDataCell(rowElement, columnIndex);

                                // old version
                                /*
                                var cell = rowElement.cells[this.GetColumnIndex(columnCaption)];
                                if(!cell){
                                    cell = rowElement.cells[this.GetActionColumnCount() + columnCaption];   
                                }
                                */
                                //var innerText = cell.innerText.replace(/<!--(.*?)-->/gm, '');
                                //var innerText = cell.innerText.replace(/<!--(.*?(\n))+.*?-->/gm, '').replace(/(\n)*$/gm, '').replace(/(\n\t)*(\t)*/gm, '');
                                //var innerText = cell.innerText.replace(/<!--(.*?)-->/gm, '').replace(/(\n)*(\r)*$/gm, '').replace(/(\n )*$/gm, '').replace(/^(\n)*/gm, '');


                                var innerText = cell.innerText.replace(/<!--(.*?)-->/gm, '').replace(/(\n)*$/gm, '').replace(/(\n )*$/gm, '').replace(/^(\n)*/g, '');
                                innerText = innerText.replace(/^(\r)*/g, '').replace(/(\r)*$/g, '').replace(/^(\n)*/g, '');
                                innerText = this.Trim(innerText);

                                if(innerText == '') {
                                    if(cell.innerHTML.indexOf('edtCheckBoxChecked') != -1 || cell.innerHTML.indexOf('edtCheckBoxUnchecked') != -1) {
                                        result = (cell.innerHTML.indexOf('edtCheckBoxChecked') != -1) ? 'True' : 'False';
                                        //------->
                                        return result;
                                    }
                                }

		                        //innerText = this.Trim(this.RemoveLineBrakes(innerText));
		                        var buttonControl = this.GetButtonControl(cell);
		                        if(cell.innerText.replace(' ', '') == '' && cell.childNodes.length > 0) {
			                        result = this.GetCheckBoxStateOrImageTitle(cell.childNodes[0]);
			                        if(!result) result = '';
		                        }			
		                        else if(buttonControl) {
		                            result = buttonControl.GetText();
		                        }
		                        else {
			                        result = innerText;
			                        if(result == ' ') result = '';
		                        }
                                return result;
                        "),
						new JSFunctionDeclaration("GetAction", "actionName, row, column", @"
					        var clientGridView = window[this.control.id];
					        if(!clientGridView) this.LogOperationError('It is impossible to perform action. The grid is empty');
					        var rowCount = this.GetTableRowCount();
					        if(row > rowCount - 1) {
						        this.LogOperationError( 'The grid contains ' + rowCount + ' rows');
						        return;
					        }
					        if(actionName != '') {
						        var isFound = false;
						        var columnActionIndex = this.FindActionColumnIndex(row, actionName);
						        this.error = null;
						        this.operationError = false;
						        if(columnActionIndex != -1) {
							        var actionCell = clientGridView.GetDataRow(row).cells[columnActionIndex + 1];
							        for(i = 0; i < actionCell.childNodes.length; i++) {
								        var innerElement = actionCell.childNodes[i];
								        if(innerElement.tagName.toUpperCase() == 'A' && innerElement.innerText == actionName) {
									        return innerElement;
								        }
							        }
			                        var buttonControl = this.GetButtonControl(actionCell);
			                        if(buttonControl) {
			                            return buttonControl;
			                        }
							        return actionCell;
						        }
						        else {
							        var actionControl = TestControls.FindControl(1, this.caption + '.' + actionName);
							        var isRootListView = clientGridView.GetMainElement().id == 'Grid';
							        if(!actionControl && isRootListView) {
								        actionControl = TestControls.FindControl(1, actionName);
							        }
							        if(!actionControl) {
							           this.LogOperationError('The ' + actionName + ' Action is not found in the ""' + this.Caption + '"" table\'s ' + row + ' row');
							        } else {
								        if(!actionControl.IsEnabled()){
									        this.LogOperationError('The ' + actionControl.caption + ' Action is disabled');
									        return;
								        }
								        return actionControl;
							        }
						        }
					        //	this.SelectRow(row, false);
					        }
					        else {
						        if(clientGridView.GetDataRow(row).getAttribute('IsClickDisabled') != 'true') {
							        return clientGridView.GetDataRow(row);
						        } else {
							        this.LogOperationError('The Default Action is disabled');
						        }
					        }				        
                        "),
						new JSFunctionDeclaration("CheckAction", "actionName, row, column", @"
                            this.GetAction(actionName, row, column);
                        ")
						};
				return result;
			}
		}
		#endregion
	}
	public class ErrorInfoTestableContolWrapper : TestableContolWrapper {
		private string errorText;
		public ErrorInfoTestableContolWrapper(string clientId, string errorText) :
			base("ErrorInfo", clientId, new JSASPxGridListEditorErrorInfoTestControl(), TestControlType.Field) {
			this.errorText = errorText;
		}
		public string GetErrorText() {
			return errorText;
		}
	}
	public class JSASPxGridListEditorErrorInfoTestControl : IJScriptTestControl, ISupportAdditionalParametersTestControl {
		public const string ClassName = "ASPxGridListEditorErrorInfo";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.AdditionalParametersDeclaration.Add("errorText");
				result.GetTextFunctionBody = @"
                    if(this.errorText) {
					    return this.errorText.replace(/<br>/g, ""\r\n"");
                    }
                    return null;
				";
				return result;
			}
		}
		#endregion
		#region ISupportAdditionalParametersTestControl Members
		public ICollection<string> GetAdditionalParameters(object control) {
			string errorText = ((ErrorInfoTestableContolWrapper)control).GetErrorText();
			return new string[] { "'" + errorText + "'" };
		}
		#endregion
	}
	public class JSASPxHtmlPropertyEditorTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxHtmlPropertyEditor";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				ASPxStandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.GetTextFunctionBody = @"
					var value = this.control.GetHtml();
					return (value == null) ? '' : value;
					";
				result.IsEnabledFunctionBody = @"
					return !this.control.disabled; 
					";
				result.SetTextFunctionBody = @"
					this.control.SetHtml(value);
					";
				return result;
			}
		}
		#endregion
	}
	public class JSASPxMenuTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxMenu";
		public JSASPxMenuTestControl() { }
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				ASPxStandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.ActFunctionBody = @"
					var selectedItem = null;
					if(value) {
						var path = value.split('.');
						if(path.length == 0) {
							this.LogOperationError('Item is not specified.');
							return;
						}
						var currentItem = null;						
						var startIndex = 0;
						if(this.control.GetItemCount() == 1 && this.control.GetItem(0).GetText() != path[0]) {
							currentItem = this.control.GetItem(0);
						}
						//New action
						if(this.control.GetItemCount() == 2) {							
							var title = '';
							if(this.caption.lastIndexOf('.') != -1) {
								title = this.caption.substr(this.caption.lastIndexOf('.') + 1);
							}
							else {
								title = this.caption;
							}
							if((this.control.GetItem(0).GetText() == title) && (this.control.GetItem(0).GetItemCount() == 0) && (this.control.GetItem(1).GetText() == '')) {
								currentItem = this.control.GetItem(1);
							}
						}
						if(!currentItem) {
							for(var j = 0; j < this.control.GetItemCount(); j++) {
								if(this.control.GetItem(j).GetText() == path[0]) {
									currentItem = this.control.GetItem(j);
									startIndex++;
									break;
								}
							}	
						}
						if(currentItem) {
							var nextLevel = true;
							for(var i = startIndex; i < path.length && nextLevel; i++) {
								nextLevel = false;
								for(var j = 0; j < currentItem.GetItemCount(); j++) {
									if(currentItem.GetItem(j).GetText() == path[i]) {
										currentItem = currentItem.GetItem(j);
										nextLevel = true;
										break;
									}
								}
							}
							if(nextLevel) {
								selectedItem = currentItem;
							}
						}
					}
					else {
						selectedItem = this.control.GetItem(0);
					}
					if(selectedItem) {
						if(selectedItem.GetEnabled()) {
							this.control.DoItemClick(selectedItem.GetIndexPath(), false, null);
						}
						else {
							this.LogOperationError('The ""' + value + '"" item is disabled');
						}
					}
					else {
						this.LogOperationError('The ""' + value + '"" item is not found');
					}
				";
				result.IsEnabledFunctionBody = @"
					return this.control.GetItem(0).GetEnabled();
				";
				return result;
			}
		}
		#endregion
	}
	public class JSASPxMenuParametrizedActionItem : IJScriptTestControl {
		public const string ClassName = "ASPxMenuParametrizedActionItem";
		public JSASPxMenuParametrizedActionItem() { }
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				ASPxStandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.ActFunctionBody = @"
                    if(this.control) {
                        //TODO B30856
                        //if(this.button.GetEnabled()) {
                            this.control.SetText(value);
						    this.control.DoClick();
                        //}
                        //else {
                        //    this.LogOperationError('The ""' + value + '"" item is Disabled.');	
                        //}
					}
					else {
						this.LogOperationError('The ""' + value + '"" item is not found.');						
					}
				";
				result.IsEnabledFunctionBody = @"
					return this.control.GetEnabled();
                    ";
				return result;
			}
		}
		#endregion
	}
	public class JSASPxTestParametrizedActionControl : IJScriptTestControl, ISupportAdditionalParametersTestControl {
		public const string ClassName = "JSASPxTestParametrizedActionControl";
		public JSASPxTestParametrizedActionControl() { }
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				ASPxStandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.AdditionalParametersDeclaration.Add("menuId");
				result.AdditionalParametersDeclaration.Add("itemName");
				result.GetTextFunctionBody = @"
                    var editor = window[this.id + '_Ed'];
					return editor.GetText();
                ";
				result.ActFunctionBody = @"
                    if(this.control) {
                        var editor = window[this.id + '_Ed'];
						if(value != undefined && value!==':Clear') {
							this.SetText(value);
						}
                        else {
                            if(value === ':Clear') {
                                if(editor.cpClearButtonIndex != undefined) {
                                    editor.GetButton(editor.cpClearButtonIndex).onclick();
                                    return;
                                }
                            }
                        }
                        if(editor.cpActButtonIndex != undefined) {
                            editor.GetButton(editor.cpActButtonIndex).onclick();
                            return;
                        }
					}
					else {
						this.LogOperationError('The item ' + value + ' is not found.');						
					}
				";
				result.IsEnabledFunctionBody = @"
                    var editor = window[this.id + '_Ed'];
					return editor.GetEnabled();
                ";
				result.SetTextFunctionBody = @"
                    var editor = window[this.id + '_Ed'];
					editor.SetText(value);
                    if(editor.ParseValueCore){
                        editor.ParseValueCore(true);
                    }
				";
				result.AdditionalFunctions = new JSFunctionDeclaration[] { 
					new JSFunctionDeclaration("GetHint", null, @"
                    if(this.control) {
                        var editor = window[this.id + '_Ed'];
                        if(editor.cpActButtonIndex != undefined) {
                            return editor.GetButton(editor.cpActButtonIndex).title;
                        }
					}
					return '';
				    "),
					new JSFunctionDeclaration("IsVisible", null, @"
                        var menu = window[menuId];
                        if(menu) {
                            var menuItem = menu.GetItemByName(itemName);
                            if(menuItem) {
                                return menuItem.GetVisible();
                            }
                        }
                        return false;
                    "),
					new JSFunctionDeclaration("IsActionItemVisible", "actionItemName", @"
                        if(actionItemName === ':Clear') {
                            var editor = window[this.id + '_Ed'];
                            if(editor.cpClearButtonIndex != undefined) {
                                return editor.GetButton(editor.cpClearButtonIndex).style.display != 'none';
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
		#region ISupportAdditionalParametersTestControl Members
		public ICollection<string> GetAdditionalParameters(object control) {
			return new string[] { "'" + DevExpress.ExpressApp.Web.Templates.ActionContainers.JSUpdateScriptHelper.GetMenuId(((MenuActionItemBase)control).MenuItem.Menu) + "'", "'" + ((MenuActionItemBase)control).MenuItem.Name + "'" };
		}
		#endregion
	}
	public class JSASPxMenuActionsHolderItem : IJScriptTestControl, ISupportAdditionalParametersTestControl {
		public const string ClassName = "ASPxMenuHolder";
		public JSASPxMenuActionsHolderItem() { }
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				ASPxStandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.AdditionalParametersDeclaration.Add("actionName");
				result.AdditionalParametersDeclaration.Add("actionCaption");
				result.ActFunctionBody = @"
                    var action = this.GetAction();
                    if(action) {
                        var currentItem = action;
                        var actionItem = this.GetActionItem(action, value);
                        if(actionItem) {
                            currentItem = actionItem;
                        }
                        if(currentItem){
                            if(currentItem.GetVisible()) {
                                if(currentItem.GetEnabled()) {
						            this.control.DoItemClick(currentItem.GetIndexPath(), false, null);
                                }
                                else {
                                    this.LogOperationError('The ""' + value + '"" item is Disabled.');	
                                }
                            }
                            else {
                                this.LogOperationError('The ""' + value + '"" item is not Invisible.');	
                            }
                        }
                        else{
                             this.LogOperationError('The ""' + value + '"" item is not found.');
                        }
                    }
                    else {
						this.LogOperationError('The ""' + value + '"" item is not found.');						
                    }
				";
				result.IsEnabledFunctionBody = @"
                    var currentItem = this.GetAction();
                    if(currentItem) {
                        return currentItem.GetEnabled();
                    }
                    else {
                        this.LogOperationError('The ""' + actionCaption + '"" item is not found.');						
                    }
				";
				result.AdditionalFunctions = new JSFunctionDeclaration[] { 
					new JSFunctionDeclaration("GetHint", null, @"
					var actionName = this.actionName;
                    var currentItem = null;
					for(var j = 0; j < this.control.GetItemCount(); j++) {
						if(this.control.GetItem(j).name == actionName) {
							var menu = this.control.GetItem(j).menu;
        					return menu.GetItemElement(j).title;
						}
					}
					"),
					new JSFunctionDeclaration("GetAction", null, @"
                        return this.GetActionCore(this.control, this.actionName, true);
                    "),
					new JSFunctionDeclaration("GetActionCore", "itemProvider, actionName, needCheckDropdownContainer", @"
                    for(var j = 0; j < itemProvider.GetItemCount(); j++) {
                        var item = itemProvider.GetItem(j);
                        if(needCheckDropdownContainer && this.control.cpDropDownContainers && this.control.cpDropDownContainers.indexOf(item.name) > -1) {
                            var result = this.GetActionCore(item, actionName, false);
                            if(result) {
                                return result;
                            }
                        }
                        if(item.name == actionName) {
                            return item;
                        }
                    }
                    return null;
                    "),
					new JSFunctionDeclaration("GetActionItem", "action, actionItemCaption", @"
					if(action) {
                        var currentItem = null;
                        if(actionItemCaption && action.GetItemCount() > 0) {
                            var targetItem = null;
                            var itemsArray = actionItemCaption.split('.');
                            currentItem = action;
                            for(var j = 0; j<itemsArray.length; j++) {
                                for(var i = 0; i<currentItem.GetItemCount(); i++) {
                                    if(currentItem.GetItem(i).GetText() == itemsArray[j]) {
                                        targetItem = currentItem.GetItem(i);
                                        currentItem = targetItem;
                                        break;
                                    }
                                }  
                            }
                            //B152604
                            currentItem = targetItem;
                        }
                        //TODO B30856
                        if(currentItem && currentItem != action){
                            return currentItem;
                        }
					}
    				return null;
                    "),
					new JSFunctionDeclaration("IsActionItemVisible", "actionItemName", @"
                        var action = this.GetAction();
                        if(action) {
                            var actionItem = this.GetActionItem(action, actionItemName);
                            if(actionItem) {
                                return actionItem.GetVisible();
                            }
                        }
                        return false;
                    "),
					new JSFunctionDeclaration("IsActionItemEnabled", "actionItemName", @"
                        var action = this.GetAction();
                        if(action) {
                            var actionItem = this.GetActionItem(action, actionItemName);
                            if(actionItem) {
                                return actionItem.GetEnabled();
                            }
                        }
                        return false;
                    ")
				};
				return result;
			}
		}
		#endregion
		#region ISupportAdditionalParametersTestControl Members
		public ICollection<string> GetAdditionalParameters(object control) {
			return new string[] { "'" + ((MenuActionItemBase)control).MenuItem.Name + "'", "'" + ((ITestable)control).TestCaption + "'" };
		}
		#endregion
	}
	public class JSSchedulerEditorTestControl : IJScriptTestControl {
		public const string ClassName = "SchedulerEditor";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				TableTestControlScriptsDeclaration result = new TableTestControlScriptsDeclaration();
				result.ClearSelectionFunctionBody = @"
				this.control.appointmentSelection.ClearSelection();
				";
				result.SelectRowFunctionBody = @"
				var appointment = this.GetAppointmentAt(row);
				if(!this.control.appointmentSelection.IsAppointmentSelected(appointment.appointmentId)) {
					this.control.appointmentSelection.AddAppointmentToSelection(appointment.appointmentId);
				}
				";
				result.UnselectRowFunctionBody = @"
				var appointment = this.GetAppointmentAt(row);
				if(this.control.appointmentSelection.IsAppointmentSelected(appointment.appointmentId)) {
					this.control.appointmentSelection.RemoveAppointmentFromSelection(appointment.appointmentId);
				}
				";
				result.IsRowSelectedFunctionBody = @"
				var appointment = this.GetAppointmentAt(row);
				return this.control.appointmentSelection.IsAppointmentSelected(appointment.appointmentId);
				";
				result.GetCellValueFunctionBody = @"
                //value columnCaption contains column Index for this control :)
				var appointment = this.GetAppointmentAt(row);				
				if(columnCaption == this.startOnColumnIndex) {
					result = this.formatter.Format(appointment.getStartTime());
				}
				else if(columnCaption == this.endOnColumnIndex) {
					result = this.formatter.Format(appointment.getEndTime());
				}
				else if(columnCaption == this.subjectColumnIndex) {
                    var subjectColumnId;
                    for(var i = 0; i < appointment.contentDiv.childNodes.length; i++) {
                        var appointmentDivId = appointment.contentDiv.childNodes[i].id;
                        if(appointmentDivId && appointmentDivId.indexOf('appointmentDiv') >= 0) {
                            subjectColumnId = appointment.contentDiv.childNodes[i].id.replace('appointmentDiv', 'lblTitle')
                            break;
                        }
                    }
					result = window.document.getElementById(subjectColumnId).innerText;
				}
				return result;
				";
				result.GetCellControlFunctionBody = @"
				var info = new Object();
				info.SchedulerControl = this;
				info.Row = row;
                //value columnCaption contains column Index for this control :)
				info.Column = columnCaption;
				var result = new " + JSSchedulerInplaceEditorTestControl.ClassName + @"(id, info.Column);
				result.Info = info;
				return result;
				";
				result.GetColumnIndexFunctionBody = @"
				var result = -1;
				if(columnCaption == 'StartOn') {
					result = this.startOnColumnIndex;
				}
				if(columnCaption == 'EndOn') {
					result = this.endOnColumnIndex;
				}
				if(columnCaption == 'Subject') {
					result = this.subjectColumnIndex;
				}
				return result;
				";
				result.GetColumnsCaptionsFunctionBody = @"
				return 'StartOn;EndOn;Subject'
				";
				result.SetCellValueFunctionBody = @"
				var appointment = this.GetAppointmentAt(row);
                //value columnCaption contains column Index for this control :)
				if(columnCaption == this.startOnColumnIndex) {
					//this.SelectRow(row);
					var startTime = this.formatter.Parse(value);	
					this.control.GetAppointment(appointment.appointmentId).interval.SetStart(startTime);
					var params = 'APTSCHANGE|' + appointment.appointmentId;
					params += '?START='+  ASPx.SchedulerGlobals.DateTimeToMilliseconds(startTime);
					params += '?DURATION=' +  (appointment.getEndTime() - startTime);
					this.control.RaiseCallback(params);
					return;
				}
				else if(columnCaption == this.endOnColumnIndex) {
					//this.SelectRow(row);
					var endTime = this.formatter.Parse(value);
					this.control.GetAppointment(appointment.appointmentId).interval.SetEnd(endTime);
					var params = 'APTSCHANGE|' + appointment.appointmentId;
					params += '?START='+  ASPx.SchedulerGlobals.DateTimeToMilliseconds(appointment.getStartTime());
					params += '?DURATION=' +  (endTime - appointment.getStartTime());
					this.control.RaiseCallback(params);     
					return;
				}
				else if(columnCaption == this.subjectColumnIndex) {
					this.LogOperationError('You can not change the subject of an event');
				}
				";
				result.GetTableRowCountFunctionBody = @"
				return this.control.horizontalViewInfo.appointmentViewInfos.length + this.control.verticalViewInfo.appointmentViewInfos.length;
				";
				result.ExecuteActionFunctionBody = @"
					this.SelectRow(row);
					if(actionName != '') {
                        this.PopupMenuAction(actionName);
					}
				";
				result.AdditionalFunctions = new JSFunctionDeclaration[] { 
				new JSFunctionDeclaration("InitControl", null, @"
					this.startOnColumnIndex = 0;
					this.endOnColumnIndex = 1;
					this.subjectColumnIndex = 2;
					this.dateTimeFormatString = 'MM/dd/yyyy H:mm';
					this.dateFormatString = 'MM/dd/yyyy';
					this.formatter = new ASPx.DateFormatter();
					this.formatter.SetFormatString(this.dateTimeFormatString);
					this.control = window[this.id.replace('$', '_')];
					if(this.control) {
						return;
					}
					var f = this.inherit.prototype.baseInitControl;
					f.call(this);
					if(this.error) {
						return;
					}					
				"),
				new JSFunctionDeclaration("GetMenuItemElement", "menuItemName", @"
					var namePath = menuItemName.split('.');
					var menuSuffixes = new Array('_aptMenuBlock_SMAPT', '_viewMenuBlock_SMVIEW');
					for(var s = 0; s < menuSuffixes.length; s++) {
						var menuId = this.id + menuSuffixes[s];
						var menu = window[menuId];
						if(menu) {
							var indexesPath;
							var parentMenuItem = null;
							for(var i = 0; i < menu.GetItemCount(); i++) {
								if(menu.GetItemText(i) == namePath[0]) {
									parentMenuItem = menu.GetItem(i);
									indexesPath = i.toString();
									break;
								}
							}
							if(parentMenuItem) {
								var lastItemFound = true;
								for(var i = 1; i < namePath.length && lastItemFound; i++) {
									lastItemFound = false;
									for(var k = 0; k < parentMenuItem.GetItemCount() && !lastItemFound; k++) {
										if(parentMenuItem.GetItem(k).GetText() == namePath[i]) {
											parentMenuItem = parentMenuItem.GetItem(k);
											indexesPath += ASPx.ItemIndexSeparator + k.toString();
											lastItemFound = true;
										}
									}
								}
								if(lastItemFound) {
									var menuItem = menu.GetItemByIndexPath(indexesPath);
									if(!menuItem.GetEnabled()) {
										this.LogOperationError('The ' + menuItemName + ' item is not available.');
									}
									return menu.GetItemElement(indexesPath);
								}
							}
						}
					}
					return null;
				"),				
				new JSFunctionDeclaration("GetAppointmentAt", "row", @"
					var appointmentsArray = null;
					var correctedRowNum = row;
					if(row < this.control.horizontalViewInfo.appointmentViewInfos.length) {
						appointmentsArray = this.control.horizontalViewInfo.appointmentViewInfos;
					}
					else {
						appointmentsArray = this.control.verticalViewInfo.appointmentViewInfos;
						correctedRowNum = row - this.control.horizontalViewInfo.appointmentViewInfos.length;
					}
					return appointmentsArray[correctedRowNum];
				"),
				new JSFunctionDeclaration("FindActionColumnIndex", "row, actionName", @"
					var menuItemElement = this.GetMenuItemElement(actionName);
					return menuItemElement ? menuItemElement.rowIndex : -1;
				"),
				new JSFunctionDeclaration("SetRowSelection", "row, isSelected", @"					
					if(isSelected) {
						this.SelectRow(row);
					}
					else {
						var appointment = this.GetAppointmentAt(row);
						if(this.control.appointmentSelection.IsAppointmentSelected(appointment.appointmentId)) {
							this.control.appointmentSelection.RemoveAppointmentFromSelection(appointment.appointmentId);							
						}
					}
				"),
				new JSFunctionDeclaration("CheckAction", "actionName, row, column", @"
					this.SelectRow(row);
					if(actionName != '') {
                        this.GetPopupMenuAction(actionName);
					}
                "),
				 #region ExecuteTableAction
				new JSFunctionDeclaration("CheckAppointmentType", "type", @"
					var appointmentId = this.control.appointmentSelection.selectedAppointmentIds[0];
					var appointment = this.control.GetAppointmentById(appointmentId);
					if(appointment.appointmentType != type) {
						this.LogOperationError('AppointmentType is not ' + type + '. The actual type is ' + appointment.appointmentType);
					}
					
				"),
				new JSFunctionDeclaration("SetSelection", "paramValues", @"
                    var from = paramValues.getItem(0);
                    var to = paramValues.getItem(1);
					var fromTime = this.formatter.Parse(from);
					var toTime = this.formatter.Parse(to);
					if(fromTime && toTime) {
						var interval = new ASPxClientTimeInterval(fromTime, toTime - fromTime);
						this.control.SetSelectionInterval(interval);
					}
					else {
						if(!fromTime) {
							this.LogOperationError('Incorrect starting date/time is specified: ' + from);
						}
						if(!toTime) {
							this.LogOperationError('Incorrect ending date/time is specified: ' + to);
						}
					}
				"),
				 new JSFunctionDeclaration("PopupMenuAction", "menuItemName", @"
                    var action = this.GetPopupMenuAction(menuItemName);
                    if(action) {
					    action.click();
                    }
				"),
				 new JSFunctionDeclaration("GetPopupMenuAction", "menuItemName", @"	
					var menuItemElement = this.GetMenuItemElement(menuItemName);
					if(menuItemElement && !this.operationError) {
						return menuItemElement;
					}
                    if(!this.operationError) {
						this.LogOperationError('The ' + menuItemName + ' item is not found.');
					}
				"),
				new JSFunctionDeclaration("HandleDeleteDialog", "actionName", @"
					var buttons = ASPx.GetNodesByPartialClassName(deleteRecurrencePopupControl.windowElements[-1], 'dxbButton');
					var clientASPxButtons = new Array();
					for(var i = 0; i < buttons.length; i++) {
						clientASPxButtons[i] = window[buttons[i].id.replace('_B', '')];
					}
                    if(deleteRecurrencePopupControl.IsVisible()) {
					    if(actionName == 'Cancel') {
						    var cancelButton = (clientASPxButtons[0].GetText() == 'Cancel') ? clientASPxButtons[0] : clientASPxButtons[1];
						    cancelButton.DoClick();
					    }
					    else if(actionName == 'DeleteOccurrence' || actionName == 'DeleteSeries') {
						    var okButton = (clientASPxButtons[0].GetText() == 'OK') ? clientASPxButtons[0] : clientASPxButtons[1];
						    var radioButtonList = ASPx.GetNodesByPartialClassName(deleteRecurrencePopupControl.windowElements[-1], 'dxeRadioButtonList')
						    var clientRadioButtonList = ASPx.GetControlCollection().Get(radioButtonList[0].id);
						    if(actionName == 'DeleteOccurrence') {
							    clientRadioButtonList.SetSelectedIndex(0);
						    }
						    else if(actionName == 'DeleteSeries') {
							    clientRadioButtonList.SetSelectedIndex(1);
						    }
						    okButton.DoClick();
					    }
					    else {
						    this.LogOperationError('Unrecognized Action name: ' + actionName);
					    }
                    }
				"),				
				new JSFunctionDeclaration("SetCurrentDate", "dateString", @"
				var dateNavigator = window[this.id + '_DateNavigator'];
				var calendar = ASPx.GetControlCollection().Get(dateNavigator.calendarId);
				calendar.selection.Clear();
				try {
					calendar.selection.Add(this.formatter.Parse(dateString));
				}
				catch(e) {
					this.LogOperationError('Unable to parse \''+ dateString +'\' date with \'' + this.dateTimeFormatString +'\' mask.');
				}
				dateNavigator.OnSelectionChanged();
				this.control.RaiseSelectionChanged();
				"),
				new JSFunctionDeclaration("CheckCurrentDate", "dateString", @"
				var selectedInterval = this.control.GetSelectedInterval();
				var startDate = ASPxSchedulerDateTimeHelper.TruncToDate(selectedInterval.GetStart());				
				var date = ASPxSchedulerDateTimeHelper.TruncToDate(this.formatter.Parse(dateString));
				return (startDate - date) == 0;
				")
				 #endregion
				};
				return result;
			}
		}
		#endregion
	}
	public class JSASPxPopupCriteriaPropertyEditorTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxPopupCriteriaPropertyEditor";
		public JSASPxPopupCriteriaPropertyEditorTestControl() {
		}
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.ActFunctionBody = @"
				    var button = window[this.id + '_B0'];
                    button.onclick();
				";
				result.GetTextFunctionBody = @"
                    return window[this.id].GetText();
                ";
				return result;
			}
		}
		#endregion
	}
	public class JSSchedulerInplaceEditorTestControl : IJScriptTestControl {
		public const string ClassName = "SchedulerInplaceEditor";
		#region IJScriptTestControl Members
		string IJScriptTestControl.JScriptClassName {
			get { return ClassName; }
		}
		TestScriptsDeclarationBase IJScriptTestControl.ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.AdditionalFunctions = new JSFunctionDeclaration[] {
					new JSFunctionDeclaration("InitControl", null, @"")
				};
				result.IsEnabledFunctionBody = "return true";
				result.SetTextFunctionBody = @"
				this.Info.SchedulerControl.SetCellValue(this.Info.Row, this.Info.Column, value);
				";
				return result;
			}
		}
		#endregion
	}
	public class JSASPxCriteriaPropertyEditorTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxCriteriaPropertyEditor";
		public JSASPxCriteriaPropertyEditorTestControl() {
		}
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new StandardTestControlScriptsDeclaration();
				result.AdditionalParametersDeclaration.Add("criteriaEditor");
				result.GetTextFunctionBody = @"
                            if(criteriaEditor.GetAppliedFilterExpression != undefined){
                                return this.PatchFilterString(criteriaEditor.GetAppliedFilterExpression());
                            }else{
                                return criteriaEditor.innerText;
                            }
						";
				result.SetTextFunctionBody = @"this.LogOperationError('The ""' + this.caption + '"" editor is readonly.');";
				result.ActFunctionBody = @"
                            var param = value.split('|');
                            for (var i = 1; i < param.length; i++) {
                        	    param[i]  = '\'' + param[i].replace(/'/g, '\\\'') + '\'';
                            }
                            eval('this.' + param[0] + '(' + param.slice(1) + ');');
                        ";
				result.IsEnabledFunctionBody = @"return true;";
				result.AdditionalFunctions = new JSFunctionDeclaration[] {
					new JSFunctionDeclaration("PatchFilterString", "filterString", @"
                        return criteriaEditor.cpPatchedFilterString;
					"),
					new JSFunctionDeclaration("AddCondition", "groupIndex", @"
						this.ExecuteGroupOperation(""|AddCondition"", parseInt(groupIndex));
					"),
					new JSFunctionDeclaration("RemoveCriteria", "index", @"
						criteriaEditor.RemoveNode(parseInt(index) + 1);
					"),
					new JSFunctionDeclaration("InitControl", "", @"
						criteriaEditor = window[this.id];
					"),
					new JSFunctionDeclaration("CheckEditorType", "editorTypeName", @""),
					new JSFunctionDeclaration("CheckValue", "nodeIndex, expectedValue", @"
                        var postfix = nodeIndex + '000';
						var editorId = 'DXValue'+ postfix;
                        var valueEditorId = criteriaEditor.name + '_DXEdit' + postfix + '_ValueEditor';
                        var valueEditor = window[valueEditorId];
                        var actualValue = criteriaEditor.GetChildElement(editorId).innerText;
                        if(valueEditor) {
                            if(valueEditor.GetVisible && valueEditor.GetVisible() && valueEditor.GetText) {
                                actualValue = valueEditor.GetText();
                            }
                        }
                        if(actualValue =='' || actualValue == '<enter a value>') {
                            actualValue = '?';
                        }
                        if(actualValue != expectedValue) {
                            this.LogOperationError('Node value differs from the expected value. Expected: \'' + expectedValue + '\', but was: \'' + actualValue + '\'');
                        }
                    "),
					new JSFunctionDeclaration("ShowEditor", "index", @"
						criteriaEditor.ShowEditor(index);
					"),
					new JSFunctionDeclaration("ChangeGroupType", "groupIndex, newGroupCaption", @"
						this.ExecuteGroupOperation(newGroupCaption, parseInt(groupIndex));
					"),
					new JSFunctionDeclaration("AddGroup", "sourceGroupIndex", @"
						this.ExecuteGroupOperation(""|AddGroup"", sourceGroupIndex);
					"),
					new JSFunctionDeclaration("ExecuteGroupOperation", "operationName, groupIndex", @"
						var filterControlGroupPopup = window[this.id + '_GroupPopup'];
						criteriaEditor.ShowGroupPopup(0, groupIndex);
						var item = filterControlGroupPopup.GetItemByName(operationName);
						if(item) {
							filterControlGroupPopup.DoItemClick(item.index.toString(), '', '');
						}
						else {
							this.LogOperationError('Unable to find ' + operationName + ' item');
						}
					"),
					new JSFunctionDeclaration("ChangeOperation", "rowIndex, newOperationCaption", @"
                        var preparedCaption = newOperationCaption.replace(/ /g,'').toLowerCase();
						var filterControlOperationPopup = window[this.id + '_OperationPopup'];
						var rootItem = filterControlOperationPopup.rootItem;
						criteriaEditor.ShowOperationPopup(0, parseInt(rowIndex).toString());
						var item;
						for(var i = 0; i < rootItem.items.length; i++) {
							var shortItemName = rootItem.items[i].name.split('|')[1];
							if(shortItemName.toLowerCase() == preparedCaption) {
								item = rootItem.items[i];
								break;
							}
						}
						if(item) {
							filterControlOperationPopup.DoItemClick(item.index.toString(),'','');
						}
						else {
							this.LogOperationError('Unable to find ' + newOperationCaption + ' item');
						}
					"),
					new JSFunctionDeclaration("ChangeValue", "rowIndex, newValue", @"
						var postfix = rowIndex + '000';
						var editorId = this.id + '_DXEdit'+ postfix;
						var editor = window[editorId];
                        var valueEditor = criteriaEditor.GetChildElement('DXValue'+ postfix);
                        valueEditor.click();
						if(editor.isASPxClientTextEdit && editor.isDropDownListStyle) {
							if(newValue.indexOf('@')== 0 || newValue == '<enter a value>') {
                                var testControl = new ASPxComboBox(editor.name,'',false);
                                testControl.Act(newValue);
                                if(testControl.GetText() != newValue) {
                                    this.LogOperationError('Unable to find ' + newValue + ' item.');
                                }
                                editor.RaiseLostFocus();
							}
							else {
							    var valueEditorId = editorId +""_ValueEditor"";
							    var valueEditor = window[valueEditorId];
                                if(valueEditor && !editor.cpHasNoValueWithParametersEdit) {
                                    valueEditor.Filter = criteriaEditor;
                                    editor.RaiseCloseUp();
							        this.SetValueWithEditor(valueEditorId, newValue);
                                }
                                else {
                                    this.SetValueWithEditor(editorId, newValue);
                                }
							}
						}
						else {
							this.SetValueWithEditor(editorId, newValue);
						}
					"),
					new JSFunctionDeclaration("SetValueWithEditor","editorId, newValue",@"
							var editor = window[editorId];
							var fieldCaption = editor.GetMainElement().parentElement.parentElement.parentElement.childNodes[0].innerText;
                            if(!fieldCaption) {
							    fieldCaption = editor.GetMainElement().parentElement.parentElement.parentElement.childNodes[1].innerText;
                            }
                            fieldCaption = fieldCaption.replace(/ /g,'');
                            var isFound = false;
                            var propertyTestControl = TestControls.FindControl('Field', fieldCaption, '');
                            if(propertyTestControl != null) {
                                propertyTestControl.id = editorId;
								propertyTestControl.SetText(newValue);
                                isFound = true;
                            }
                            if(!isFound) {
                                this.LogOperationError('Cannot change value. The \'' + fieldCaption + '\' property was not found.');
                            }
							editor.RaiseLostFocus();
					"),
					new JSFunctionDeclaration("ChangeFieldName", "rowIndex, newValue", @"
                        var fieldName = newValue.replace(' ', '');
						var showPopupParams = { ItemClickHandler: 'ASPx.FCChangeFieldName', SubMenuKey: '', SubMenuDepthLevel: 0 };
						criteriaEditor.ShowFieldNamePopup('', parseInt(rowIndex), '', showPopupParams);
						var filterControlFieldNamePopup = window[this.id +'_FieldNamePopup'];
                        var item = filterControlFieldNamePopup.GetItemByName(fieldName);
                        if(item) {
						    var itemIndex = item.index;
						    filterControlFieldNamePopup.DoItemClick(itemIndex.toString(), '', '');
                        }
                        else {
                            this.LogOperationError('Cannot change field name. The \'' + newValue + '\' field name was not found.');
                        }
					")
				};
				return result;
			}
		}
		#endregion
	}
	public class NavigationLinksActionContainerJScriptTestControl : IJScriptTestControl {
		public string JScriptClassName {
			get { return "NavigationLinksActionContainer"; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.ActFunctionBody = @"
                var itemElement = this.GetItemElement(value);
                if(itemElement) {
					if(this.IsActionItemEnabled(value)) {
						itemElement.children[0].click();
					}
					else {
						this.LogOperationError('The ""' + value + '"" item of the ""' + this.caption + '"" Action is disabled');
					}
				}
                else {
				    this.LogOperationError('The ""' + this.caption + '"" Action does not contain the ""' + value + '"" item');
                }
				";
				result.AdditionalFunctions = new JSFunctionDeclaration[] {
					new JSFunctionDeclaration("GetItemElement", "value", @"
				        for(var i = 0; i < this.control.GetItemCount(); i++) {
                            var item = this.control.GetItem(i);
                            if(item.GetText() == value) {
                                return this.control.GetItemElement(i);
					        }
				        }
                        return null;
                    "),
					new JSFunctionDeclaration("GetItem", "value", @"
				        for(var i = 0; i < this.control.GetItemCount(); i++) {
                            var item = this.control.GetItem(i);
                            if(item.GetText() == value) {
                                return item;
					        }
				        }
                        return null;
                    "),
					new JSFunctionDeclaration("IsActionItemVisible", "actionItemName", @"
                        var item = this.GetItem(actionItemName);
                        return item ? item.GetVisible() : false;
                        
                    "),
					new JSFunctionDeclaration("IsActionItemEnabled", "actionItemName", @"
                        var item = this.GetItem(actionItemName);
                        return item ? item.GetEnabled() : false;
                    ")
				};
				return result;
			}
		}
	}
	internal class JSASPxCheckedListBoxEditorTestControl : IJScriptTestControl, ISupportAdditionalParametersTestControl {
		public const string ClassName = " ASPxCheckedListBoxStringPropertyEditor";
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				StandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.AdditionalParametersDeclaration.Add("checkList");
				result.GetTextFunctionBody = @"
                    return this.control.GetText();";
				result.SetTextFunctionBody = @"                     
                if(value) {
                    var texts = value.split(';');
                    var values = new Array(texts.length);
                    var ind = 0;
                    for(i = 0; i < this.checkList.GetItemCount(); i++) {
                        for(j = 0; j < texts.length; j++) {
                            if(this.CompareString(this.checkList.GetItem(i).text, texts[j])) {
                                values[ind] = i;
                                ind++;
                            }
                        }
                    }
                    this.checkList.SelectIndices(values);
                    ASPx.EValueChanged(this.checkList.name);
                }
                else {
                    this.checkList.UnselectAll();
                }
                ";
				result.ActFunctionBody = @"
					this.SetText(value);";
				result.IsEnabledFunctionBody = @"
                    return this.control.GetEnabled();
                    ";
				return result;
			}
		}
		#endregion
		#region ISupportAdditionalParametersTestControl Members
		public ICollection<string> GetAdditionalParameters(object control) {
			return new string[] { ((DevExpress.Web.ASPxDropDownEdit)((ASPxCheckedListBoxStringPropertyEditor)control).Editor).JSProperties["cpCheckListBoxId"].ToString() };
		}
		#endregion
	}
}
