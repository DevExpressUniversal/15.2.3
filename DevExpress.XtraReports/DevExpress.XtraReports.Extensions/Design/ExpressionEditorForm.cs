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
using DevExpress.XtraReports.UI;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.XtraReports.Native;
using System.Windows.Forms;
using System.Collections;
using System.Data;
using DevExpress.XtraReports.UserDesigner.Native;
using System.ComponentModel.Design;
using DevExpress.XtraReports.Native.Data;
using DevExpress.Utils;
using System.Drawing;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.XtraReports.Localization;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraReports.Native.CalculatedFields;
using DevExpress.XtraEditors.Design;
using DevExpress.Data.ExpressionEditor;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.LookAndFeel.DesignService;
using System.Linq;
namespace DevExpress.XtraReports.Design {
	public abstract class ReportsExpressionEditorLogic : ExpressionEditorLogic {
		protected readonly ReportsExpressionEditorForm form;
		protected ReportsExpressionEditorLogic(IExpressionEditor editor, object contextInstance, ReportsExpressionEditorForm form)
			: base(editor, contextInstance) {
			this.form = form;
		}
		internal abstract object GetEffectiveDataSource();
		internal abstract string GetDataMember();
		protected internal override void FillFieldsTable(Dictionary<string, string> itemsTable) {
			form.ShowParametersEditor();
		}
		protected internal override void FillParametersTable(Dictionary<string, string> itemsTable) {
			DevExpress.XtraReports.Parameters.ParameterCollection parameters = form.report.Parameters;
			foreach(DevExpress.XtraReports.Parameters.Parameter parameter in parameters)
				itemsTable.Add(ParametersReplacer.GetParameterFormattedName(parameter.Name), editor.GetResourceString("Parameters Description Prefix") + parameter.Type.Name);
		}
		protected override void RefreshInputParameters() {
			form.HideParametersEditor();
			base.RefreshInputParameters();
		}
		protected override IList<FunctionEditorCategory> GetFunctionsTypeNames() {
			IList<FunctionEditorCategory> result = base.GetFunctionsTypeNames();
			result.Insert(1, FunctionEditorCategory.Aggregate);
			return result;
		}
		protected override string ConvertToCaption(string expression) {
			return contextInstance is IDisplayNamePropertyContainer ? ((IDisplayNamePropertyContainer)contextInstance).GetDisplayPropertyValue(expression) : base.ConvertToCaption(expression);
		}
		public override string ConvertToFields(string expression) {
			return contextInstance is IDisplayNamePropertyContainer ? ((IDisplayNamePropertyContainer)contextInstance).GetRealPropertyValue(expression) : base.ConvertToFields(expression);
		}
		protected override bool ValidateExpression() {
			if(!base.ValidateExpression())
				return false;
			if(string.IsNullOrEmpty(ExpressionMemoEdit.Text))
				return true;
			try {
				FunctionNameValidator.Validate(CriteriaOperator.Parse(ExpressionMemoEdit.Text, null));
			} catch(InvalidOperationException e) {
				editor.ShowError(e.Message);
				return false;
			}
			return true;
		}
	}
	public class CalculatedFieldExpressionEditorLogic : ReportsExpressionEditorLogic {
		public CalculatedFieldExpressionEditorLogic(IExpressionEditor editor, CalculatedField calculatedField, ReportsExpressionEditorForm form)
			: base(editor, calculatedField, form) {
		}
		CalculatedField CalculatedField { get { return (CalculatedField)contextInstance; } }
		protected override object[] GetListOfInputTypesObjects() {
			return new object[] {
			editor.GetResourceString("Functions.Text"),
			editor.GetResourceString("Operators.Text"),
			editor.GetResourceString("Fields.Text"),
			editor.GetResourceString("Constants.Text"),
			editor.GetResourceString("Parameters.Text")};
		}
		protected override string GetExpressionMemoEditText() {
			return CalculatedField.Expression;
		}
		internal override object GetEffectiveDataSource() {
			return CalculatedField.GetEffectiveDataSource();
		}
		internal override string GetDataMember() {
			return CalculatedField.DataMember;
		}
	}
	public class FormattingRuleConditionEditorLogic : ReportsExpressionEditorLogic {
		public FormattingRuleConditionEditorLogic(IExpressionEditor editor, FormattingRule formattingRule, ReportsExpressionEditorForm form)
			: base(editor, formattingRule, form) {
		}
		FormattingRule FormattingRule { get { return (FormattingRule)contextInstance; } }
		protected override object[] GetListOfInputTypesObjects() {
			return new object[] {
			editor.GetResourceString("Functions.Text"),
			editor.GetResourceString("Operators.Text"),
			editor.GetResourceString("Fields.Text"),
			editor.GetResourceString("Constants.Text"),
			editor.GetResourceString("Variables.Text"),
			editor.GetResourceString("Parameters.Text")};
		}
		protected override string GetExpressionMemoEditText() {
			return FormattingRule.Condition;
		}
		protected override bool ValidateExpression() {
			if(!base.ValidateExpression())
				return false;
			ConditionBooleanTypeValidator validator = new ConditionBooleanTypeValidator(GetEffectiveDataSource(), GetDataMember());
			if(validator.ValidateCondition(ExpressionMemoEdit.Text))
				return true;
			editor.ShowError(ReportLocalizer.GetString(ReportStringId.Msg_InvalidCondition));
			return false;
		}
		internal override object GetEffectiveDataSource() {
			return FormattingRule.GetEffectiveDataSource();
		}
		internal override string GetDataMember() {
			return FormattingRule.GetEffectiveDataMember();
		}
	}
	public abstract class ReportsExpressionEditorForm : ExpressionEditorForm {
		PopupFieldNamePicker fieldListEditor;
		internal XtraReport report;
		ReportsExpressionEditorLogic ReportsExpressionEditorLogic { get { return (ReportsExpressionEditorLogic)fEditorLogic; } }
		bool useCalculatedFields;
		protected ReportsExpressionEditorForm(object contextInstance, IDesignerHost designerHost, bool useCalculatedFields)
			: base(contextInstance, designerHost) {
			this.useCalculatedFields = useCalculatedFields;
			SetReport(designerHost);
			InitializeFieldListEditor();
			SubscribeEvents();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			UnsubscribeEvents();
		}
		protected override void SetParentLookAndFeel() {
			DesignLookAndFeelHelper.SetParentLookAndFeel(this, ServiceProvider);
		}
		void SetReport(IDesignerHost designerHost) {
			if(designerHost.RootComponent is XtraReport)
				report = (XtraReport)designerHost.RootComponent;
		}
		internal void ShowParametersEditor() {
			fieldListEditor.Visible = true;
			fieldListEditor.Bounds = GetParametersEditorBounds();
			fieldListEditor.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			HideParameters();
		}
		internal void HideParametersEditor() {
			if(fieldListEditor != null && fieldListEditor.Visible) {
				fieldListEditor.Visible = false;
				ShowParameters();
			}
		}
		void InitializeFieldListEditor() {
			fieldListEditor = new ExpressionFieldNamePicker(new DataContextOptions(true, useCalculatedFields));
			this.Controls.Add(fieldListEditor);
			fieldListEditor.AddNoneNode = false;
			fieldListEditor.Visible = false;
			fieldListEditor.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			object dataSource = ReportsExpressionEditorLogic.GetEffectiveDataSource();
			string dataMember = ReportsExpressionEditorLogic.GetDataMember();
			fieldListEditor.Start((IServiceProvider)this.ServiceProvider, dataSource, dataMember, null, null);
		}
		void SubscribeEvents() {
			fieldListEditor.MouseDoubleClick += new MouseEventHandler(fieldListEditor_MouseDoubleClick);
			fieldListEditor.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(fieldListEditor_FocusedNodeChanged);
		}
		void UnsubscribeEvents() {
			if(fieldListEditor != null) {
				fieldListEditor.MouseDoubleClick -= new MouseEventHandler(fieldListEditor_MouseDoubleClick);
				fieldListEditor.FocusedNodeChanged -= new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(fieldListEditor_FocusedNodeChanged);
			}
		}
		void fieldListEditor_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e) {
			DataMemberListNodeBase node = fieldListEditor.FocusedNode as DataMemberListNodeBase;
			if(node == null || node.Property == null)
				return;
			if(!node.HasChildren)
				SetDescription(GetResourceString("Fields Description Prefix") + node.Property.PropertyType.ToString());
			else SetDescription(string.Empty);
		}
		void fieldListEditor_MouseDoubleClick(object sender, MouseEventArgs e) {
			string columnName = GetColumnNameToInsert(ExpressionMemoEdit.Text, ExpressionMemoEdit.SelectionStart, fieldListEditor.GetSelectedDataMember(), (IDisplayNamePropertyContainer)ContextInstance);
			fEditorLogic.InsertTextInExpressionMemo(columnName);
		}
		static string GetColumnNameToInsert(string expression, int position, string selectedDataMember, IDisplayNamePropertyContainer displayNameProvider) {
			string displayName = displayNameProvider.GetDisplayName(selectedDataMember);
			if(!string.IsNullOrEmpty(displayName))
				selectedDataMember = displayName;
			string columnName = new FieldNameHelper(expression).GetFieldName(selectedDataMember, position);
			return MailMergeFieldInfo.WrapColumnInfoInBrackets(CriteriaFieldNameConverter.EscapeFieldName(columnName), string.Empty);
		}
#if DEBUGTEST
		public static string Test_GetColumnNameToInsert(string expression, int position, string selectedDataMember, IDisplayNamePropertyContainer displayNameProvider) {
			return GetColumnNameToInsert(expression, position, selectedDataMember, displayNameProvider);
		}
#endif
	}
	public class CalculatedFieldExpressionEditorForm : ReportsExpressionEditorForm {
		protected override string CaptionName { get { return "Expression.Text"; } }
		public CalculatedFieldExpressionEditorForm(object contextInstance, IDesignerHost designerHost)
			: base(contextInstance, designerHost, true) {
		}
		protected override ExpressionEditorLogic CreateExpressionEditorLogic() {
			return new CalculatedFieldExpressionEditorLogic(this, (CalculatedField)ContextInstance, this);
		}
	}
	public class FormattingRuleConditionEditorForm : ReportsExpressionEditorForm {
		protected override string CaptionName { get { return "Condition.Text"; } }
		public FormattingRuleConditionEditorForm(object contextInstance, IDesignerHost designerHost)
			: base(contextInstance, designerHost, true) {
		}
		protected override ExpressionEditorLogic CreateExpressionEditorLogic() {
			return new FormattingRuleConditionEditorLogic(this, (FormattingRule)ContextInstance, this);
		}
	}
	class ExpressionFieldNamePicker : PopupFieldNamePicker {
		public ExpressionFieldNamePicker(DataContextOptions options)
			: base(options) {
			OptionsBehavior.AllowExpandOnDblClick = false;
		}
		protected override bool CanCloseDropDown(DevExpress.XtraTreeList.Native.XtraListNode node) {
			return true;
		}
	}
	class FieldNameHelper {
		class Context {
			FieldNameHelper fieldHelper;
			int tokenIndex;
			public bool HasToken { get { return tokenIndex > 0; } }
			public TokenType TokenType { get { return TokenList[tokenIndex - 1].TokenType; } }
			public string TokenText { get { return GetTokenText(TokenList[tokenIndex - 1]); } }
			List<CriteriaLexerToken> TokenList { get { return fieldHelper.helper.TokenList; } }
			public Context(FieldNameHelper fieldHelper, int position) {
				this.fieldHelper = fieldHelper;
				SetPosition(position);
			}
			public int GetPosition() {
				return TokenList[tokenIndex - 1].Position + 1;
			}
			public void SetPosition(int position) {
				CriteriaLexerToken token = FindToken(position);
				tokenIndex = fieldHelper.helper.TokenList.IndexOf(token);
			}
			public void Move() {
				tokenIndex--;
			}
			string GetTokenText(CriteriaLexerToken token) {
				return fieldHelper.expression.Substring(token.Position, token.Length);
			}
			CriteriaLexerToken FindToken(int position) {
				int index = FindTokenIndex(position);
				return index > -1 ? TokenList[index] : null;
			}
			int FindTokenIndex(int position) {
				List<CriteriaLexerToken> tokenList = fieldHelper.helper.TokenList;
				for(int i = 0; i < TokenList.Count; i++) {
					if(TokenList[i].Position <= position && position < TokenList[i].PositionEnd || position < TokenList[i].Position) return i;
				}
				return -1;
			}
		}
		enum SearchState {
			Start,
			OpenParenthesis,
			Aggregate,
			Dot,
			Condition,
			Property,
			Error
		}
		const char OpeningBracket = '[';
		const char ClosingBracket = ']';
		CriteriaLexerTokenHelper helper;
		string expression;
		public FieldNameHelper(string expression) {
			helper = new CriteriaLexerTokenHelper(expression);
			this.expression = expression;
		}
		public string GetFieldName(string field, int position) {
			Context context = new Context(this, position);
			Stack<string> props = new Stack<string>();
			if(InAggregateExpression(context)) {
				string prop = FindAggregatePropertyForExpression(context);
				if(string.IsNullOrEmpty(prop))
					return field;
				props.Push(GetCorrectProperty(prop));
			} else {
				context.SetPosition(position);
			}
			while(context.HasToken) {
				string prop;
				position = context.GetPosition();
				if(InAggregateExpression(context)) {
					prop = FindAggregatePropertyForExpression(context);
					if(!string.IsNullOrEmpty(prop)) {
						props.Push(GetCorrectProperty(prop));
						continue;
					}
				} else {
					context.SetPosition(position);
					prop = FindAggregatePropertyForCondition(context);
					if(!string.IsNullOrEmpty(prop))
						props.Push(GetCorrectProperty(prop));
				}
			}
			string prefix = string.Join(".", props.ToArray()) + ".";
			if(field.StartsWith(prefix))
				return field.Substring(prefix.Length);
			return field;
		}
		static string ClearSquareBrackets(string s) {
			return (s[0] == OpeningBracket && s[s.Length - 1] == ClosingBracket) ? s.Substring(1, s.Length - 2) : s;
		}
		static string GetCorrectProperty(string str) {
			string result = str;
			int countOpeningBracket = str.Count(x => x == OpeningBracket);
			int countClosingBracket = str.Count(x => x == ClosingBracket);
			if(countOpeningBracket > countClosingBracket)
				result = str.Remove(str.IndexOf(OpeningBracket), 1).Trim();
			return ClearSquareBrackets(result);
		}
		static bool InAggregateExpression(Context context) {
			SearchState state = SearchState.Start;
			int nesting = 0;
			while(context.HasToken) {
				switch(state) {
					case SearchState.Start:
						if(context.TokenType == TokenType.OpenParenthesis)
							nesting--;
						else if(context.TokenType == TokenType.CloseParenthesis)
							nesting++;
						if(nesting == -1)
							state = SearchState.OpenParenthesis;
						break;
					case SearchState.OpenParenthesis:
						return context.TokenType == TokenType.Aggregate;
				}
				context.Move();
			}
			return false;
		}
		static string FindAggregatePropertyForExpression(Context context) {
			SearchState state = SearchState.Start;
			int nesting = 0;
			while(context.HasToken) {
				switch(state) {
					case SearchState.Start:
						if(context.TokenType == TokenType.Aggregate)
							state = SearchState.Aggregate;
						else
							state = SearchState.Error;
						break;
					case SearchState.Aggregate:
						if(context.TokenType == TokenType.Dot)
							state = SearchState.Dot;
						else
							state = SearchState.Error;
						break;
					case SearchState.Dot:
						if(context.TokenType == TokenType.Unknown && context.TokenText == "]") {
							state = SearchState.Condition;
							nesting++;
						} else if(context.TokenType == TokenType.Property)
							return context.TokenText;
						else
							state = SearchState.Error;
						break;
					case SearchState.Condition:
						if(context.TokenType == TokenType.Unknown)
							if(context.TokenText == "]")
								nesting++;
							else if(context.TokenText == "[")
								nesting--;
						if(nesting == 0)
							state = SearchState.Property;
						break;
					case SearchState.Property:
						if(context.TokenType == TokenType.Property)
							return context.TokenText;
						else
							state = SearchState.Error;
						break;
				}
				if(state == SearchState.Error)
					break;
				else
					context.Move();
			}
			return null;
		}
		static string FindAggregatePropertyForCondition(Context context) {
			SearchState state = SearchState.Condition;
			int nesting = 0;
			while(context.HasToken) {
				switch(state) {
					case SearchState.Condition:
						if(context.TokenType == TokenType.Unknown)
							if(context.TokenText == "]")
								nesting++;
							else if(context.TokenText == "[")
								nesting--;
						if(nesting == -1)
							state = SearchState.Property;
						break;
					case SearchState.Property:
						if(context.TokenType == TokenType.Property)
							return context.TokenText;
						else
							state = SearchState.Condition;
						break;
				}
				context.Move();
			}
			return null;
		}
	}
}
