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
using DevExpress.ExpressApp.Web.TestScripts;
namespace DevExpress.ExpressApp.TreeListEditors.Web {
	public class JSASPxTreeListEditorTestControl : IJScriptTestControl {
		public const string ClassName = "ASPxTreeListEditor";
		private TableTestControlScriptsDeclaration scriptsDeclaration = new TableTestControlScriptsDeclaration();
		public JSASPxTreeListEditorTestControl() {
			scriptsDeclaration.GetColumnsCaptionsFunctionBody =
@"var result = '';
var treeList = window[this.control.id];
var headersRow = treeList.GetHeaderRow();
for(var i = 0; i < headersRow.cells.length; i++) {
	var caption = this.RemoveLineBrakes(this.Trim(headersRow.cells[i].innerText));
	if(caption != '' && caption != ' ') { //Skip action columns
		result += caption + ';';
	}
}
if(result.length > 0) {
	result = result.substring(0, result.length - 1);
}
return result;";
			scriptsDeclaration.GetTableRowCountFunctionBody =
@"var treeList = window[this.control.id];
return treeList.GetVisibleNodeKeys().length;";
			scriptsDeclaration.GetCellValueFunctionBody =
@"var treeList = window[this.control.id];
var result = '';
if(treeList) {
	var rowResult = '';
	var rows = treeList.GetAllDataRows();
	var rowCount = rows.length;
	if(row > rowCount - 1) {
		this.LogOperationError('The grid contains: ' + rowCount + ' rows');
		return result;
	}
	var level = this.GetNodeLevel(row);
	var currentLevel = level;
	var columnIndex = this.GetColumnIndexByColumnCaption(columnCaption);
	if(columnIndex == -1) {
		this.LogOperationError('Cannot find the ' + columnCaption + ' column.');
		return;
	}
	while(currentLevel >= 0) {
		while(currentLevel != this.GetNodeLevel(row)) {
			row--;
		}
		var rowElement = rows[row];
		var isCellFound = false;
		if(rowElement) {
			var cell = rowElement.cells[2 + currentLevel + columnIndex];
			var innerText = this.RemoveLineBrakes(this.Trim(cell.innerText));
			if(cell.innerText.replace(' ', '') == '' && cell.childNodes.length > 0) {
				rowResult = this.GetCheckBoxStateOrImageTitle(cell.childNodes[0]);
				if(!rowResult) rowResult = '';
			}			
			else {
				rowResult = innerText;
				if(rowResult == ' ') rowResult = '';
			}
		}
		result = rowResult + result;
		if(currentLevel > 0) {
			result = '.' + result;
		}
		currentLevel--;
	}
}
return result;";
			scriptsDeclaration.IsSupportSelectionBody = @"return true;";
			scriptsDeclaration.IsRowSelectedFunctionBody =
@"var treeList = window[this.control.id];
return treeList.IsNodeSelected(treeList.GetVisibleNodeKeys()[row]);";
			scriptsDeclaration.SelectRowFunctionBody =
@"var treeList = window[this.control.id];
var nodeKeys = treeList.GetVisibleNodeKeys();
if(row > nodeKeys.length - 1) {
	this.LogOperationError( 'The grid contains: ' + nodeKeys.length + ' rows');
	return;
}
treeList.SelectNode(nodeKeys[row]);
if(treeList.RaiseSelectionChanged()) {
	treeList.SendDummyCommand(true);
}";
			scriptsDeclaration.UnselectRowFunctionBody =
@"var treeList = window[this.control.id];
var nodeKeys = treeList.GetVisibleNodeKeys();
if(row > nodeKeys.length - 1) {
	this.LogOperationError('The grid contains: ' + nodeKeys.length + ' rows');
	return;
}
treeList.SelectNode(nodeKeys[row], false);
if(treeList.RaiseSelectionChanged()) {
	treeList.SendDummyCommand(true);
}";
			scriptsDeclaration.ClearSelectionFunctionBody =
@"var treeList = window[this.control.id];
var nodeKeys = treeList.GetVisibleNodeKeys();
for(var i = 0; i < nodeKeys.length; i++) {
	treeList.SelectNode(nodeKeys[i], false);
}
if(treeList.RaiseSelectionChanged()) {
	treeList.SendDummyCommand(true);
}";
			scriptsDeclaration.ExecuteActionFunctionBody =
@"var action = this.GetAction(actionName, row, column);
if(action) {
	action.click();
}";
			scriptsDeclaration.GetCellControlFunctionBody = @"var treeList = window[this.control.id];";
			scriptsDeclaration.ExpandGroupsFunctionBody =
@"var treeList = window[this.control.id];
treeList.ExpandAll();";
			scriptsDeclaration.AdditionalFunctions = new JSFunctionDeclaration[] { 
				new JSFunctionDeclaration("GetCheckBoxStateOrImageTitle", "cell",
@"if(cell && cell.tagName && (cell.tagName.toUpperCase() == 'IMG' || (cell.tagName.toUpperCase() == 'INPUT' && cell.type && cell.type == 'checkbox'))) {
	if(cell.tagName.toUpperCase() == 'IMG' && !IsNull(cell.title)) {
		return cell.title != '' ? cell.title : cell.alt;
	}
	if(cell.tagName.toUpperCase() == 'INPUT' && cell.type == 'checkbox') {
		return cell.checked ? 'True' : 'False';
	}
}
else {
	for(var i=0;i<cell.childNodes.length;i++) {
		var result = this.GetCheckBoxStateOrImageTitle(cell.childNodes[i]);
		if(result) {
			return result;
		}
	}
}"),
				new JSFunctionDeclaration("GetNodeLevel", "row",
@"var treeList = window[this.control.id];
var level = -1;
if(treeList) {
	var rows = treeList.GetAllDataRows();
	var rowCount = rows.length;
	if(row > rowCount - 1) {
		this.LogOperationError( 'The grid contains: ' + rowCount + ' rows');
		return result;
	}
	var rowElement = rows[row];
	var isCellFound = false;
	if(rowElement) {
		for(var i = 0; i < rowElement.cells.length; i++) {
			level++;
			var cell = rowElement.cells[i];
			if(cell.className.indexOf('dxtl__I') < 0) {
				break;
			}
		}
		level -= 2;
	}
}
return level;"),
				new JSFunctionDeclaration("Trim", "sString",
@"while(sString.substring(0,1) == ' ') {
	sString = sString.substring(1, sString.length);
}
while(sString.substring(sString.length-1, sString.length) == ' ') {
	sString = sString.substring(0,sString.length-1);
}
sString = sString.replace(/^(\r\n)+/g,'');
sString = sString.replace(/(\r\n)+$/g,'');
return sString;"),
				new JSFunctionDeclaration("RemoveLineBrakes", "str",
@"str = str.replace(/\r/g,'');
str = str.replace(/ \n+/g,' ');
str = str.replace(/\n+ /g,' ');
str = str.replace(/\n+/g,' ');
str = str.replace(/ (<BR>)+/g,' ');
str = str.replace(/(<BR>)+ /g,' ');
str = str.replace(/(<BR>)+/g,' ');
return str;"),
				new JSFunctionDeclaration("GetAction", "actionName, row, column",
@"var treeList = window[this.control.id];
var rows = treeList.GetAllDataRows();
var rowCount = rows.length;
if(row > rowCount - 1) {
	this.LogOperationError( 'The grid contains: ' + rowCount + ' rows');
	return;
}
if((actionName == null) || (actionName == '')) {
	var level = this.GetNodeLevel(row);
	rows[row].cells[level + 2 + column].click();
	return;
}
var foundActionElement;
var commandCell = rows[row].cells[rows[row].cells.length - 1];
for(var i = 0; i < commandCell.children.length; i++) {
	var child = commandCell.children[i];
	var childText = this.Trim(child.innerText);
	var childAction = '';
	if(childText != '') {
		childAction = childText;
	}
	else if(child.alt && this.Trim(child.alt) != '') {
		childAction = this.Trim(child.alt);
	}
	if(childAction == actionName) {
		foundActionElement = child;
	}
}
if(!foundActionElement) {
	this.LogOperationError( 'The row doesn\'t contain the \'' + actionName + '\' action');
	return;
}
return foundActionElement;"),
				new JSFunctionDeclaration("CheckAction", "actionName, row, column", "this.GetAction(actionName, row, column);"),
				new JSFunctionDeclaration("GetColumnIndexByColumnCaption", "columnCaption",
@"var columnCaptions = this.GetColumnsCaptions().split(';');
var columnIndex = -1;
for(var i = 0; i < columnCaptions.length; i++) {
	if(columnCaptions[i] == columnCaption) {
		columnIndex = i;
		break;
	}
}
return columnIndex;"),
			};
		}
		#region IJScriptTestControl Members
		string IJScriptTestControl.JScriptClassName {
			get { return ClassName; }
		}
		TestScriptsDeclarationBase IJScriptTestControl.ScriptsDeclaration {
			get { return scriptsDeclaration; }
		}
		#endregion
	}
}
