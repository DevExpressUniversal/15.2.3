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
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Templates;
namespace DevExpress.ExpressApp.Web.TestScripts {
	public class JSFunctionDeclaration {
		public JSFunctionDeclaration(string name, string parameters, string body) {
			Name = name;
			Params = parameters;
			Body = body;
		}
		public string Name { get; set; }
		public string Params { get; set; }
		public string Body { get; set; }
	}
	public class TestScriptsDeclarationBase {
		private List<string> additionalParametersDeclaration = new List<string>();
		private JSFunctionDeclaration[] additionalFunctions;
		private string CorrectIndent(string newIndent, string paragraph) {
			string[] strings = paragraph.Split(new char[] { '\n' });
			StringBuilder builder = new StringBuilder();
			int leadingWSCount = -1;
			for(int i = 0; i < strings.Length; i++) {
				string currentLine = strings[i].TrimEnd(null);
				if(!string.IsNullOrWhiteSpace(currentLine)) {
					if(leadingWSCount == -1) {
						leadingWSCount = currentLine.Length - currentLine.TrimStart(null).Length;
					}
					if(currentLine.Length - currentLine.TrimStart(null).Length >= leadingWSCount) {
						currentLine = currentLine.Substring(leadingWSCount);
					}
					builder.Append(newIndent).AppendLine(currentLine);
				}
			}
			return builder.ToString();
		}
		private string GetParameterDeclarations(ICollection<string> parameters) {
			if(parameters.Count == 0) {
				return "";
			}
			return ", " + string.Join(", ", parameters);
		}
		private string GetParameterMembers(ICollection<string> parameters) {
			if(parameters.Count == 0) {
				return "";
			}
			StringBuilder builder = new StringBuilder();
			foreach(string parameter in parameters) {
				builder.AppendFormat("	this.{0} = {0};", parameter).AppendLine();
			}
			return builder.ToString();
		}
		private string GetFunctionDeclarations(ICollection<JSFunctionDeclaration> functionDeclarations) {
			if(functionDeclarations.Count == 0) {
				return "";
			}
			StringBuilder builder = new StringBuilder();
			foreach(JSFunctionDeclaration declaration in functionDeclarations) {
				builder.Append(GetFunctionDeclaration(declaration));
			}
			return builder.ToString();
		}
		private string GetFunctionDeclaration(JSFunctionDeclaration functionDeclaration) {
			string requireInit = (functionDeclaration.Name == "InitControl") ? "false" : "true";
			return string.Format(
@"	this.{0} = function({1}) {{
		return this.CallWithLog('{0}', {2}, function() {{
{3}		}});
	}}
", functionDeclaration.Name, functionDeclaration.Params, requireInit, CorrectIndent("			", functionDeclaration.Body));
		}
		protected virtual ICollection<string> GetAdditionalParametersDeclaration() {
			return additionalParametersDeclaration;
		}
		protected virtual List<JSFunctionDeclaration> GetFunctionDeclarations() {
			if(additionalFunctions != null) {
				return new List<JSFunctionDeclaration>(additionalFunctions);
			}
			return new List<JSFunctionDeclaration>();
		}
		public string GetJScript(string jsClassName) {
			List<string> parameters = new List<string>(AdditionalParametersDeclaration);
			return string.Format(
@"/*		{0}		*/
function {0}(id, caption{1}) {{
	this.className = '{0}';
	this.inherit = TestControlBase;
	this.inherit(id, caption);
{2}{3}}}
", jsClassName, GetParameterDeclarations(parameters), GetParameterMembers(parameters), GetFunctionDeclarations(GetFunctionDeclarations()));
		}
		public ICollection<string> AdditionalParametersDeclaration {
			get { return GetAdditionalParametersDeclaration(); }
		}
		public JSFunctionDeclaration[] AdditionalFunctions {
			get { return additionalFunctions; }
			set { additionalFunctions = value; }
		}
	}
	public class StandardTestControlScriptsDeclaration : TestScriptsDeclarationBase, ISupportAdditionalParametersTestControl {
		public const string autoPostBackParamName = "autoPostBack";
		private string isEnabledFunctionBody = @"return !this.control.disabled;";
		private string getTextFunctionBody =
@"if(this.control.value) {
	return this.control.value;
}
return this.control.innerText;";
		private string setTextFunctionBody =
@"if(!IsNull(this.control.readOnly) && this.control.readOnly) {
	this.LogOperationError('The ""' + this.caption + '"" editor is readonly.');
	return;
}
this.control.value = value;";
		private string actFunctionBody;
		protected override ICollection<string> GetAdditionalParametersDeclaration() {
			ICollection<string> result = base.GetAdditionalParametersDeclaration();
			if(SetTextFunctionBody.Contains(autoPostBackParamName) && !result.Contains(autoPostBackParamName)) {
				result.Add(autoPostBackParamName);
			}
			return result;
		}
		protected override List<JSFunctionDeclaration> GetFunctionDeclarations() {
			List<JSFunctionDeclaration> result = base.GetFunctionDeclarations();
			if(!string.IsNullOrEmpty(IsEnabledFunctionBody)) {
				result.Add(new JSFunctionDeclaration("IsEnabled", "", IsEnabledFunctionBody));
			}
			if(!string.IsNullOrEmpty(GetTextFunctionBody)) {
				result.Add(new JSFunctionDeclaration("GetText", "", GetTextFunctionBody));
			}
			if(!string.IsNullOrEmpty(SetTextFunctionBody)) {
				result.Add(new JSFunctionDeclaration("SetText", "value", SetTextFunctionBody));
			}
			if(!string.IsNullOrEmpty(ActFunctionBody)) {
				result.Add(new JSFunctionDeclaration("Act", "value", ActFunctionBody));
			}
			return result;
		}
		public string IsEnabledFunctionBody {
			get { return isEnabledFunctionBody; }
			set { isEnabledFunctionBody = value; }
		}
		public string GetTextFunctionBody {
			get { return getTextFunctionBody; }
			set { getTextFunctionBody = value; }
		}
		public string SetTextFunctionBody {
			get { return setTextFunctionBody; }
			set { setTextFunctionBody = value; }
		}
		public string ActFunctionBody {
			get { return actFunctionBody; }
			set { actFunctionBody = value; }
		}
		#region ISupportAdditionalParametersTestControl Members
		public virtual ICollection<string> GetAdditionalParameters(object control) {
			List<string> result = new List<string>();
			if(GetAdditionalParametersDeclaration().Contains(autoPostBackParamName)) {
				bool autoPostBack = false;
				if(control is WebPropertyEditor) {
					WebPropertyEditor webPropertyEditor = (WebPropertyEditor)control;
					autoPostBack = webPropertyEditor.ImmediatePostData;
				}
				result.Add(autoPostBack ? "true" : "false");
			}
			return result;
		}
		#endregion
	}
	public class ASPxStandardTestControlScriptsDeclaration : StandardTestControlScriptsDeclaration {
		public const string defaultAspxBeforeSetTextFunctionBody =
@"if(this.control.inputElement.readOnly) {
	this.LogOperationError('The ""' + this.caption + '"" editor is readonly.');
	return;
}";
		public const string defaultAspxAfterSetTextFunctionBody =
@"if(this.control.RaiseValueChangedEvent) {
	this.control.RaiseValueChangedEvent();
}
else if(this." + autoPostBackParamName + @") {
	window." + XafCallbackManager.CallbackControlID + @".PerformCallback('');
}";
		public const string defaultAspxSetTextFunctionBody =
defaultAspxBeforeSetTextFunctionBody + @"
this.control.SetValue(value);
" + defaultAspxAfterSetTextFunctionBody;
		private string initControlFunctionBody =
@"var controlId = this.id.replace(/\$/g,'_');
this.control = window[controlId];
var tmpControl = document.getElementById(controlId);
if(!tmpControl && this.control) {
	tmpControl = document.getElementById(this.control.name);
	if(!tmpControl) {
		//the control doesn't exist in markup
		this.control = null;
	}
}
if(this.control) {
	return;
}
var f = this.inherit.prototype.baseInitControl;
f.call(this);
if(this.error) {
	return;
}";
		protected override List<JSFunctionDeclaration> GetFunctionDeclarations() {
			List<JSFunctionDeclaration> result = base.GetFunctionDeclarations();
			result.Add(new JSFunctionDeclaration("InitControl", "value", initControlFunctionBody));
			result.Add(new JSFunctionDeclaration("GetClientControlById", "id", "return window[id];"));
			return result;
		}
		public ASPxStandardTestControlScriptsDeclaration() {
			IsEnabledFunctionBody =
@"var hasMainElementMethod = false;
var isMainElementEnabled = false;
if(this.control.GetMainElement) {
	hasMainElementMethod = true;
	isMainElementEnabled = !this.control.GetMainElement().isDisabled;
}
var hasGetEnabledMethod = false;
var isEnabled = false;
if(this.control.GetEnabled) {
	hasGetEnabledMethod = true;
	isEnabled = this.control.GetEnabled();
}
//B150245
if(hasGetEnabledMethod && hasMainElementMethod && isEnabled && !isMainElementEnabled){
	return isMainElementEnabled;
}
if(hasGetEnabledMethod) {
	return isEnabled;
}
if(this.control.enabled != true && this.control.enabled != false) {
	if(this.control.GetInputElement) {
		return !this.control.GetInputElement().isDisabled;
	} else {
		return false;
	}
}
return this.control.enabled;";
		}
		public string InitControlFunctionBody {
			get { return initControlFunctionBody; }
			set { initControlFunctionBody = value; }
		}
	}
	public class ASPxButtonTestControlScriptsDeclaration : ASPxStandardTestControlScriptsDeclaration, IJScriptTestControl {
		protected override List<JSFunctionDeclaration> GetFunctionDeclarations() {
			List<JSFunctionDeclaration> result = base.GetFunctionDeclarations();
			result.Add(new JSFunctionDeclaration("GetHint", null, "return this.control.GetMainElement().title;"));
			return result;
		}
		public ASPxButtonTestControlScriptsDeclaration() {
			IsEnabledFunctionBody = @"return this.control.GetEnabled();";
			ActFunctionBody =
@"var aspxButton = window[this.id];
if(!aspxButton) {
	this.LogOperationError('Client-side API is disabled for the ' + this.caption);
	return;
}
aspxButton.DoClick();";
		}
		#region IJScriptTestControl Members
		public virtual string JScriptClassName {
			get { return "ASPxButton"; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get { return this; }
		}
		#endregion
	}
	public class ASPxTabTestControlScriptsDeclaration : ASPxStandardTestControlScriptsDeclaration, IJScriptTestControl {
		public ASPxTabTestControlScriptsDeclaration() {
			IsEnabledFunctionBody = @"return true;";
			ActFunctionBody =
@"var title = '';
if(this.caption.lastIndexOf('.') != -1) {
	title = this.caption.substr(this.caption.lastIndexOf('.') + 1);
}
else {
	title = this.caption;
}
var executableControl = null;
var aspxControl = window[this.id];
var tabsToSearch = aspxControl.tabs;
for(var i = 0; i < tabsToSearch.length; i++) {
	if(tabsToSearch[i].GetVisible() && ASPx.Str.DecodeHtml(tabsToSearch[i].GetText()) == title) {
		executableControl = aspxControl.GetTab(i);
		break;
	}
}
if(executableControl) {
	aspxControl.SetActiveTab(executableControl);
}
else {
	this.LogOperationError('The ""' + this.caption + '"" Action does not contain the ""' + value + '"" item');
}";
		}
		#region IJScriptTestControl Members
		public virtual string JScriptClassName {
			get { return "ASPxTab"; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get { return this; }
		}
		#endregion
	}
	public class ASPxPopupWindowButtonTestControlScriptsDeclaration : ASPxButtonTestControlScriptsDeclaration {
		protected override List<JSFunctionDeclaration> GetFunctionDeclarations() {
			List<JSFunctionDeclaration> result = base.GetFunctionDeclarations();
			result.Add(new JSFunctionDeclaration("GetIsPopupWindow", null, "return true;"));
			return result;
		}
		#region IJScriptTestControl Members
		public override string JScriptClassName {
			get { return "ASPxPopupWindowButton"; }
		}
		#endregion
	}
	public class TableTestControlScriptsDeclaration : TestScriptsDeclarationBase {
		private string isRowSelectedFunctionBody;
		private string clearSelectionFunctionBody;
		private string getCellValueFunctionBody;
		private string getColumnIndexFunctionBody;
		private string getColumnsCaptionsFunctionBody;
		private string selectRowFunctionBody;
		private string unselectRowFunctionBody;
		private string setCellValueFunctionBody;
		private string getTableRowCountFunctionBody;
		private string getCellControlFunctionBody;
		private string beginEditFunctionBody;
		private string endEditFunctionBody;
		private string expandGroups;
		private string isGrouped;
		private string executeActionFunctionBody;
		private string isSupportSelectionBody;
		protected override List<JSFunctionDeclaration> GetFunctionDeclarations() {
			List<JSFunctionDeclaration> result = base.GetFunctionDeclarations();
			if(GetTableRowCountFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("GetTableRowCount", "", GetTableRowCountFunctionBody));
			}
			if(isSupportSelectionBody != null) {
				result.Add(new JSFunctionDeclaration("IsSupportSelection", "", IsSupportSelectionBody));
			}
			if(ClearSelectionFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("ClearSelection", "", ClearSelectionFunctionBody));
			}
			if(ExecuteActionFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("ExecuteAction", "actionName, row, column", ExecuteActionFunctionBody));
			}
			if(SelectRowFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("SelectRow", "row", SelectRowFunctionBody));
			}
			if(SelectRowFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("UnselectRow", "row", UnselectRowFunctionBody));
			}
			if(IsRowSelectedFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("IsRowSelected", "row", IsRowSelectedFunctionBody));
			}
			if(GetCellValueFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("GetCellValue", "row, columnCaption", GetCellValueFunctionBody));
			}
			if(GetColumnIndexFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("GetColumnIndex", "columnCaption", GetColumnIndexFunctionBody));
			}
			if(GetColumnsCaptionsFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("GetColumnsCaptions", "", GetColumnsCaptionsFunctionBody));
			}
			if(BeginEditFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("BeginEdit", "row", BeginEditFunctionBody));
			}
			if(GetCellControlFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("GetCellControl", "row, columnCaption", GetCellControlFunctionBody));
			}
			if(EndEditFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("EndEdit", "", EndEditFunctionBody));
			}
			if(SetCellValueFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("SetCellValue", "row, columnCaption, value", SetCellValueFunctionBody));
			}
			if(ExpandGroupsFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("ExpandGroups", null, ExpandGroupsFunctionBody));
			}
			if(IsGroupedFunctionBody != null) {
				result.Add(new JSFunctionDeclaration("IsGrouped", null, IsGroupedFunctionBody));
			}
			return result;
		}
		public string ExpandGroupsFunctionBody {
			get { return expandGroups; }
			set { expandGroups = value; }
		}
		public string IsGroupedFunctionBody {
			get { return isGrouped; }
			set { isGrouped = value; }
		}
		public string BeginEditFunctionBody {
			get { return beginEditFunctionBody; }
			set { beginEditFunctionBody = value; }
		}
		public string GetCellControlFunctionBody {
			get { return getCellControlFunctionBody; }
			set { getCellControlFunctionBody = value; }
		}
		public string EndEditFunctionBody {
			get { return endEditFunctionBody; }
			set { endEditFunctionBody = value; }
		}
		public string GetCellValueFunctionBody {
			get { return getCellValueFunctionBody; }
			set { getCellValueFunctionBody = value; }
		}
		public string ExecuteActionFunctionBody {
			get { return executeActionFunctionBody; }
			set { executeActionFunctionBody = value; }
		}
		public string GetColumnIndexFunctionBody {
			get { return getColumnIndexFunctionBody; }
			set { getColumnIndexFunctionBody = value; }
		}
		public string GetColumnsCaptionsFunctionBody {
			get { return getColumnsCaptionsFunctionBody; }
			set { getColumnsCaptionsFunctionBody = value; }
		}
		public string GetTableRowCountFunctionBody {
			get { return getTableRowCountFunctionBody; }
			set { getTableRowCountFunctionBody = value; }
		}
		public string ClearSelectionFunctionBody {
			get { return clearSelectionFunctionBody; }
			set { clearSelectionFunctionBody = value; }
		}
		public string SelectRowFunctionBody {
			get { return selectRowFunctionBody; }
			set { selectRowFunctionBody = value; }
		}
		public string UnselectRowFunctionBody {
			get { return unselectRowFunctionBody; }
			set { unselectRowFunctionBody = value; }
		}
		public string IsRowSelectedFunctionBody {
			get { return isRowSelectedFunctionBody; }
			set { isRowSelectedFunctionBody = value; }
		}
		public string IsSupportSelectionBody {
			get { return isSupportSelectionBody; }
			set { isSupportSelectionBody = value; }
		}
		public string SetCellValueFunctionBody {
			get { return setCellValueFunctionBody; }
			set { setCellValueFunctionBody = value; }
		}
	}
}
