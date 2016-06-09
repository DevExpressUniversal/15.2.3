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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Localization;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public interface ICustomValueParser {
		object TryParse(string text, IFilterablePropertyInfo propertyInfo);
		string GetDisplayText(object value, IFilterablePropertyInfo propertyInfo);
	}
	public class CustomCreateDataColumnInfoListEventArgs : EventArgs {
		public IEnumerable<DataColumnInfo> List { get; set; }
	}
	public class CustomCreatePropertyEditorEventArgs : EventArgs {
		public ASPxPropertyEditor PropertyEditor { get; set; }
	}
	public class CustomCreatePropertyEditorModelEventArgs : EventArgs {
		public IModelMemberViewItem PropertyEditorModel { get; set; }
	}
	public class ASPxCriteriaPropertyEditor : WebPropertyEditor, IComplexViewItem, IDependentPropertyEditor, ITestable, ITestableContainer {
		private static bool showApplyButton;
		private List<ICustomValueParser> valueParsers;
		private ASPxFilterControl criteriaEdit;
		private CriteriaEditorHelper editorHelper;
		private bool filterExpressionUpdating;
		private List<IDisposable> disposablePropertyEditors = new List<IDisposable>();
		protected virtual IEnumerable<DataColumnInfo> CreateDataColumnInfoList() {
			CustomCreateDataColumnInfoListEventArgs args = new CustomCreateDataColumnInfoListEventArgs();
			if(CustomCreateDataColumnInfoList != null) {
				CustomCreateDataColumnInfoList(this, args);
			}
			if(args.List == null) {
				args.List = editorHelper.GetDataColumnInfos();
			}
			return args.List;
		}
		private void PopulateColumns(CriteriaEditorHelper editorHelper, List<ITestable> testableControls) {
			valueParsers.Add(new ReadOnlyParametersValueParser());
			criteriaEdit.Columns.Clear();
			foreach(DataColumnInfo columnInfo in CreateDataColumnInfoList()) {
				if(!CriteriaPropertyEditorHelper.IgnoredMemberTypes.Contains(columnInfo.Type)) {
					FilterControlColumn column = CreateFilterControlColumn(columnInfo, editorHelper, testableControls);
					if(column != null) {
						criteriaEdit.Columns.Add(column);
					}
				}
			}
		}
		protected ASPxPropertyEditor CreatePropertyEditor(bool needProtectedContent, IModelMemberViewItem modelDetailViewItem, ITypeInfo objectType, XafApplication application, IObjectSpace objectSpace) {
			CustomCreatePropertyEditorEventArgs args = new CustomCreatePropertyEditorEventArgs();
			if(CustomCreatePropertyEditor != null) {
				CustomCreatePropertyEditor(this, args);
			}
			if(args.PropertyEditor == null && modelDetailViewItem != null && modelDetailViewItem.PropertyEditorType != null) {
				args.PropertyEditor = editorHelper.Application.EditorFactory.CreateDetailViewEditor(needProtectedContent, modelDetailViewItem, objectType.Type, application, objectSpace) as ASPxPropertyEditor;
			}
			if(args.PropertyEditor != null) {
				args.PropertyEditor.ImmediatePostData = false;
			}
			return args.PropertyEditor;
		}
		private IModelMemberViewItem CreatePropertyEditorModel(IMemberInfo memberInfo, out ITypeInfo typeInfo) {
			typeInfo = null;
			CustomCreatePropertyEditorModelEventArgs args = new CustomCreatePropertyEditorModelEventArgs();
			if(CustomCreatePropertyEditorModel != null) {
				CustomCreatePropertyEditorModel(this, args);
			}
			if(args.PropertyEditorModel == null) {
				args.PropertyEditorModel = editorHelper.CreateColumnInfoNode(memberInfo, out typeInfo);
			}
			return args.PropertyEditorModel;
		}
		protected virtual FilterControlColumn CreateFilterControlColumn(DataColumnInfo columnInfo, CriteriaEditorHelper editorHelper, List<ITestable> testableControls) {
			FilterControlColumn result = null;
			IMemberInfo memberInfo = editorHelper.FilteredTypeInfo.FindMember(columnInfo.Name);
			if(memberInfo == null || !memberInfo.IsVisible) { 
				return null;
			}
			ITypeInfo typeInfo;
			IModelMemberViewItem editorInfo = CreatePropertyEditorModel(memberInfo, out typeInfo);
			ASPxPropertyEditor propertyEditor =
				CreatePropertyEditor(false, editorInfo, typeInfo, editorHelper.Application, editorHelper.ObjectSpace);
			if(propertyEditor is ITestable) {
				propertyEditor.ViewEditMode = ViewEditMode.Edit;
				propertyEditor.CreateControl();
				if(((ITestable)propertyEditor).TestControl != null) {
					if(!(propertyEditor is ASPxLookupPropertyEditor) && !SimpleTypes.IsClass(columnInfo.Type)) {
						testableControls.Add(new TestableUnknownClientIdWrapper((ITestable)propertyEditor));
					}
				}
			}
			if(propertyEditor is ICustomValueParser) {
				valueParsers.Add((ICustomValueParser)propertyEditor);
			}
			if(propertyEditor != null) {
				disposablePropertyEditors.Add(propertyEditor);
				if(propertyEditor is ASPxLookupPropertyEditor || SimpleTypes.IsClass(columnInfo.Type)) {
					FilterControlLookupEditColumn lookupColumn = new FilterControlLookupEditColumn();
					lookupColumn.PropertiesLookupEdit.ObjectSpace = editorHelper.ObjectSpace;
					lookupColumn.PropertiesLookupEdit.ObjectTypeInfo = editorHelper.FilteredTypeInfo;
					lookupColumn.PropertiesLookupEdit.MemberInfo = memberInfo;
					lookupColumn.PropertiesLookupEdit.Model = editorInfo;
					lookupColumn.PropertiesLookupEdit.Model.LookupEditorMode = LookupEditorMode.AllItems;
					lookupColumn.TestCaption = propertyEditor.TestCaption;
					testableControls.Add(new TestableUnknownClientIdWrapper((ITestable)lookupColumn));
					result = lookupColumn;
				}
				else {
					FilterControlPropertyEditorColumn propertyEditorColumn = new FilterControlPropertyEditorColumn(memberInfo.MemberType);
					propertyEditorColumn.PropertiesASPxPropertyEditor.ASPxPropertyEditor = propertyEditor;
					result = propertyEditorColumn;
				}
			}
			if(result == null) {
				result = CreateFilterControlColumnByType(columnInfo.Type);
			}
			result.PropertyName = columnInfo.Name.Replace("!", "");
			result.DisplayName = CaptionHelper.GetMemberCaption(editorHelper.FilteredTypeInfo, result.PropertyName);
			return result;
		}
		private FilterControlColumn CreateFilterControlColumnByType(Type type) {
			if(type.Equals(typeof(DateTime))) {
				return new FilterControlDateColumn();
			}
			if(type.Equals(typeof(bool))) {
				return new FilterControlCheckColumn();
			}
			if(type.Equals(typeof(int)) || type.Equals(typeof(decimal))) {
				return new FilterControlSpinEditColumn();
			}
			if(type.Equals(typeof(float)) || type.Equals(typeof(double))) {
				FilterControlSpinEditColumn floatEditColumn = new FilterControlSpinEditColumn();
				floatEditColumn.PropertiesSpinEdit.NumberType = SpinEditNumberType.Float;
				return floatEditColumn;
			}
			return new FilterControlTextColumn();
		}
		private void CreateFilterControl() {
			criteriaEdit = new ASPxFilterControl();
			criteriaEdit.Width = Unit.Percentage(100);
			criteriaEdit.ParseValue += new FilterControlParseValueEventHandler(criteriaEdit_ParseValue);
			criteriaEdit.PreRender += new EventHandler(criteriaEdit_PreRender);
			criteriaEdit.CustomValueDisplayText += new FilterControlCustomValueDisplayTextEventHandler(criteriaEdit_CustomValueDisplayText);
			criteriaEdit.EnablePopupMenuScrolling = true;
			DevExpress.ExpressApp.Web.RenderHelper.SetupASPxWebControl(criteriaEdit);
			criteriaEdit.BeforeGetCallbackResult += new EventHandler(criteriaEdit_BeforeGetCallbackResult);
			criteriaEdit.Load += new EventHandler(criteriaEdit_Load);
			criteriaEdit.Model.CreateCriteriaParseContext += Model_CreateCriteriaParseContext;
		}
		private void Model_CreateCriteriaParseContext(object sender, XtraEditors.Filtering.CreateCriteriaParseContextEventArgs e) {
			e.Context = editorHelper.ObjectSpace.CreateParseCriteriaScope();
		}
		private void CreateButtons(Panel panel) {
			ASPxButton applyButton = RenderHelper.CreateASPxButton();
			applyButton.Text = CaptionHelper.GetLocalizedText("DialogButtons", "Apply");
			panel.Controls.Add(applyButton);
		}
		private void criteriaEdit_Load(object sender, EventArgs e) {
			ICallbackManagerHolder holder = ((Control)sender).Page as ICallbackManagerHolder;
			if(holder != null) {
				holder.CallbackManager.PreRenderInternal += new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			}
		}
		private void CallbackManager_PreRenderInternal(object sender, EventArgs e) {
			((XafCallbackManager)sender).PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			if(criteriaEdit != null) {
				WritePatchedFilterString();
			}
		}
		protected override WebControl CreateEditModeControlCore() {
			WebControl result = null;
			CreateFilterControl();
			if(ShowApplyButton) {
				Panel panel = new Panel();
				panel.Controls.Add(criteriaEdit);
				CreateButtons(panel);
				result = panel;
			}
			else {
				result = criteriaEdit;
			}
			return result;
		}
		private void criteriaEdit_PreRender(object sender, EventArgs e) {
			if(((ASPxFilterControl)sender).Page != null && !((ASPxFilterControl)sender).Page.IsCallback) {
				WritePatchedFilterString();
			}
		}
		internal void WritePatchedFilterString() {
			using(editorHelper.ObjectSpace.CreateParseCriteriaScope()) {
				string patchedFilterString = new CriteriaLexerTokenHelper(criteriaEdit.FilterExpression).ConvertConstants(ConvertConstantValueToDisplayText);
				criteriaEdit.JSProperties["cpPatchedFilterString"] = new CriteriaLexerTokenHelper(patchedFilterString).ConvertProperties(true, ConvertPropertyNameToDisplayText);
			}
		}
		private string ConvertConstantValueToDisplayText(string listFieldName, string propertyName, string constValue, CriteriaLexerToken constToken) {
			string fullMemberName = String.IsNullOrEmpty(listFieldName) ? propertyName : listFieldName + "." + propertyName;
			FilterControlColumn column = criteriaEdit.Columns[fullMemberName];
			FilterControlLookupEditColumn lookupColumn = column as FilterControlLookupEditColumn;
			if(lookupColumn != null) {
				object value = null;
				if(constToken.CriteriaOperator is OperandValue) {
					value = ((OperandValue)constToken.CriteriaOperator).Value;
				}
				if(value == null) {
					return String.Empty;
				}
				ASPxComboBox comboBox = (ASPxComboBox)lookupColumn.PropertiesLookupEdit.CreateEdit(new CreateEditControlArgs(value, null, null, null, null, EditorInplaceMode.StandAlone, false));
				return '\'' + comboBox.Text + '\'';
			}
			return constValue;
		}
		private string ConvertPropertyNameToDisplayText(string listFieldName, string fieldName) {
			FilterControlColumn listColumn = !string.IsNullOrEmpty(listFieldName) ? criteriaEdit.Columns[listFieldName] : null;
			FilterControlColumn column = criteriaEdit.Columns[fieldName];
			if(column == null) {
				return fieldName;
			}
			string caption = column.DisplayName;
			while((column = (FilterControlColumn)((IBoundProperty)column).Parent) != listColumn) {
				caption = column.DisplayName + '.' + caption;
			}
			return caption;
		}
		private void criteriaEdit_CustomValueDisplayText(object sender, FilterControlCustomValueDisplayTextEventArgs e) {
			foreach(ICustomValueParser parser in valueParsers) {
				string displayText = parser.GetDisplayText(e.Value, e.PropertyInfo);
				if(!string.IsNullOrEmpty(displayText)) {
					e.DisplayText = displayText;
					return;
				}
			}
		}
		private void criteriaEdit_ParseValue(object sender, FilterControlParseValueEventArgs e) {
			foreach(ICustomValueParser parser in valueParsers) {
				object parsedValue = parser.TryParse(e.Text, e.PropertyInfo);
				if(parsedValue != null) {
					e.Value = parsedValue;
					e.Handled = true;
					return;
				}
			}
		}
		private void criteriaEdit_BeforeGetCallbackResult(object sender, EventArgs e) {
			filterExpressionUpdating = true;
			try {
				WritePatchedFilterString();
				EditValueChangedHandler(sender, e);
			}
			finally {
				filterExpressionUpdating = false;
			}
		}
		protected void OnTestableControlsCreated() {
			if(TestableControlsCreated != null) {
				TestableControlsCreated(this, EventArgs.Empty);
			}
		}
		protected override void ReadEditModeValueCore() {
			if(!filterExpressionUpdating) {
				Guard.ArgumentNotNull(editorHelper, "helper");
				criteriaEdit.FilterExpression = editorHelper.ConvertFromOldFormat((string)PropertyValue, CurrentObject);
				editorHelper.Owner = CurrentObject;
				testableControls.Clear();
				PopulateColumns(editorHelper, testableControls);
				if(testableControls.Count > 0) {
					OnTestableControlsCreated();
				}
				AllowEdit.SetItemValue(TheDataTypeIsDefined, editorHelper.FilteredTypeInfo != null);
			}
		}
		protected override object GetControlValueCore() {
			criteriaEdit.ApplyFilter();
			return editorHelper.ConvertFromOldFormat(criteriaEdit.FilterExpression, CurrentObject);
		}
		protected override string GetPropertyDisplayValue() {
			string result = ReflectionHelper.GetObjectDisplayText(PropertyValue);
			return editorHelper.ConvertFromOldFormat(result, CurrentObject);
		}
		public ASPxCriteriaPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			testableControls = new List<ITestable>();
			valueParsers = new List<ICustomValueParser>();
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(criteriaEdit != null) {
				ICallbackManagerHolder holder = criteriaEdit.Page as ICallbackManagerHolder;
				if(holder != null) {
					holder.CallbackManager.PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
				}
				foreach(IDisposable propertyEditor in disposablePropertyEditors) {
					propertyEditor.Dispose();
				}
				disposablePropertyEditors.Clear();
				valueParsers.Clear();
				criteriaEdit.Columns.Clear();
				criteriaEdit.ParseValue -= new FilterControlParseValueEventHandler(criteriaEdit_ParseValue);
				criteriaEdit.PreRender -= new EventHandler(criteriaEdit_PreRender);
				criteriaEdit.Load -= new EventHandler(criteriaEdit_Load);
				criteriaEdit.CustomValueDisplayText -= new FilterControlCustomValueDisplayTextEventHandler(criteriaEdit_CustomValueDisplayText);
				criteriaEdit.Model.CreateCriteriaParseContext -= Model_CreateCriteriaParseContext;
				if(!unwireEventsOnly) {
					criteriaEdit = null;
				}
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public static bool ShowApplyButton {
			get { return showApplyButton; }
			set { showApplyButton = value; }
		}
		public ASPxFilterControl FilterControl {
			get { return criteriaEdit; }
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			if(editorHelper == null) {
				editorHelper = new CriteriaEditorHelper(application, objectSpace, MemberInfo, ObjectTypeInfo);
			}
			editorHelper.SetupObjectSpace(objectSpace);
		}
		public event EventHandler<CustomCreateDataColumnInfoListEventArgs> CustomCreateDataColumnInfoList;
		public event EventHandler<CustomCreatePropertyEditorEventArgs> CustomCreatePropertyEditor;
		public event EventHandler<CustomCreatePropertyEditorModelEventArgs> CustomCreatePropertyEditorModel;
		#region IDependentPropertyEditor Members
		public IList<string> MasterProperties {
			get { return new string[] { editorHelper.CriteriaObjectTypeMemberInfo.Name }; }
		}
		#endregion
		#region ITestable Members
		string ITestable.TestCaption {
			get { return ((IModelViewItem)Model).Caption; }
		}
		string ITestable.ClientId {
			get { return Editor == null ? ClientId : Editor.ClientID; }
		}
		IJScriptTestControl ITestable.TestControl {
			get { return new JSASPxCriteriaPropertyEditorTestControl(); }
		}
		#endregion
		#region ITestableContainer Members
		private List<ITestable> testableControls;
		public ITestable[] GetTestableControls() {
			return testableControls.ToArray();
		}
		public event EventHandler TestableControlsCreated;
		#endregion
#if DebugTest
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void DebugTest_WritePatchedFilterString() {
			WritePatchedFilterString();
		}
#endif
	}
	[ToolboxItem(false)]
	public class ValueWithParametersEdit : ASPxComboBox, INamingContainer {
		private ASPxEditBase valueEdit;
		protected string GetHideValueEditScript() {
			return @"function xafVPEHideValueEdit(edit) {
				if(!ASPx.IsExists(edit.Filter)) return;
				var editorIndex = edit.Filter.editorIndex;				
				if(editorIndex < 0) return;
				var link = edit.Filter.GetChildElement(""DXValue"" + editorIndex);
				if(link != null) {
					link.style.display = """";
					edit.SetVisible(false);
				}
				edit.Filter.editorIndex = -1;
				edit.Filter.prevEditorValue = null;
			}";
		}
		protected string GetShowValueEditScript() {
			return @"function xafVPEShowValueEdit(owner, editor) {
				var editorIndex = owner.Filter.editorIndex;
				editor.Filter = owner.Filter;
				editor.filterEditorIndex = editorIndex;
				var link = owner.Filter.GetChildElement(""DXValue"" + editorIndex);
				if(link != null) {
					//owner.Filter.HideEditor(); //B145340
                    owner.SetVisible(false); 
					editor.SetVisible(true);
					editor.Focus();
					//link.style.display = ""none"";
					editor.Filter.prevEditorValue = editor.GetValue();
					editor.Filter.editorIndex = editorIndex;
				}
			}";
		}
		protected string GetValueEditLostFocusScript() {
			return @"function xafVPEValueEditLostFocus(s,e) {
				if(!ASPx.IsExists(s.Filter)) return;
				var editorIndex = s.Filter.editorIndex;				
				if(editorIndex < 0) return;
				var prevEditorValue = s.Filter.prevEditorValue;
				xafVPEHideValueEdit(s);
				if(prevEditorValue != s.GetValue()) {
                    var params = [s.Filter.GetValueIndexByEditor(editorIndex).toString(), s.GetValueString()];
					s.Filter.FilterCallback(""Value"", s.Filter.GetNodeIndexByEditor(editorIndex), params);
				}
			}";
		}
		protected string GetComboBoxLostFocusScript() {
			return @"function xafVPEComboBoxLostFocus(comboBox, e) {
                        if(comboBox.clientVisible) {
                            ASPx.FCEditorLostFocus(comboBox, e);
                        }
                    }";
		}
		protected string GetValueEditGotFocusScript() {
			return @"function xafVPEValueEditGotFocus(s,e) {				
				if(!ASPx.IsExists(s.Filter)) return;
				if(s.Filter.editorIndex < 0 && !s.filterEditorIndex) return;
				if(s.Filter.editorIndex < 0 && s.filterEditorIndex < 0) return;
				if(s.Filter.editorIndex < 0) {
					s.Filter.editorIndex = s.filterEditorIndex;
					s.Filter.prevEditorValue = s.GetValue();				
				}
				var link = s.Filter.GetChildElement(""DXValue"" + s.Filter.editorIndex);
				if(link != null) {
					link.style.display = ""none"";
				}
			}";
		}
		protected string GetValueEditKeyDownScript() {
			return @"function xafVPEValueEditKeyDown(s, e) {
				if(e.htmlEvent.keyCode == ASPxKey.Enter) {
					e.htmlEvent.cancelBubble = true;
					e.htmlEvent.returnValue = false;
					xafVPEValueEditLostFocus(s, e);
				}
				if(e.htmlEvent.keyCode == ASPxKey.Esc) {
					xafVPEHideValueEdit(s);
				}
			}";
		}
		protected override EditPropertiesBase CreateProperties() {
			return new ComboBoxProperties(this);
		}
		protected override void RegisterScriptBlocks() {
			string scriptBlock = GetHideValueEditScript() + GetShowValueEditScript() + GetValueEditLostFocusScript() + GetValueEditKeyDownScript() + GetValueEditGotFocusScript() + GetComboBoxLostFocusScript();
			RegisterScriptBlock("ValueEditEventHandlersAndExFunctions", RenderUtils.GetScriptHtml(scriptBlock));
			base.RegisterScriptBlocks();
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			if(valueEdit != null) {
				valueEdit.ClientVisible = false;
				valueEdit.ClientInstanceName = ClientID + "_ValueEditor";
				this.ClientSideEvents.CloseUp = string.Format(@"function(s, e)	{{
						window.setTimeout(function() {{
							if(s.GetSelectedIndex() == 0) {{
								xafVPEShowValueEdit(s, {0});
							}}
						}}, 10);					
					
				}}", valueEdit.ClientInstanceName);
				this.ClientSideEvents.SelectedIndexChanged = string.Format(@"function(s, e)	{{
					if(s.GetSelectedIndex() == 0) {{
						{0}.SetText("""");
						xafVPEShowValueEdit(s, {0});
					}}
				}}", valueEdit.ClientInstanceName);
				valueEdit.SetClientSideEventHandler("LostFocus", "xafVPEValueEditLostFocus");
				valueEdit.SetClientSideEventHandler("KeyDown", "xafVPEValueEditKeyDown");
				valueEdit.SetClientSideEventHandler("GotFocus", "xafVPEValueEditGotFocus");
				this.Controls.Add(valueEdit);
				this.SetClientSideEventHandler("LostFocus", "xafVPEComboBoxLostFocus");
			}
		}
		public ValueWithParametersEdit()
			: base() {
			RenderHelper.SetupASPxWebControl(this);
			this.EncodeHtml = false;
			this.EnableClientSideAPI = true;
		}
		public void SetParameters(IList<string> parameterList) {
			Items.Clear();
			Items.Add(ASPxCriteriaPropertyEditorLocalizer.Active.GetLocalizedString("EnterValueText"), null);
			foreach(string parameterName in parameterList) {
				Items.Add(CriteriaWrapper.ParameterPrefix + parameterName, CriteriaWrapper.ParameterPrefix + parameterName);
			}
		}
		public ASPxComboBox ValuesComboBox {
			get { return this; }
		}
		public ASPxEditBase ValueEdit {
			get {
				return valueEdit;
			}
			set {
				if(valueEdit == value)
					return;
				valueEdit = value;
			}
		}
		public static bool IsValueParametrized(object value) {
			return (value is string) && ((string)value).StartsWith(CriteriaWrapper.ParameterPrefix) && !FilterWithObjectsProcessor.IsObjectString((string)value);
		}
	}
	public class FilterControlPropertyEditorColumn : FilterControlEditColumn {
		private Type propertyType;
		protected override Type GetPropertyType() {
			return propertyType;
		}
		protected override EditPropertiesBase CreateEditProperties() {
			return new ASPxPropertyEditorProperties();
		}
		public FilterControlPropertyEditorColumn(Type propertyType) {
			this.propertyType = propertyType;
			ColumnType = GetColumnTypeByType(propertyType);
			if(ColumnType == FilterControlColumnType.String && propertyType != typeof(string)) {
				ColumnType = FilterControlColumnType.Default;
			}
		}
		public ASPxPropertyEditorProperties PropertiesASPxPropertyEditor {
			get { return (ASPxPropertyEditorProperties)this.PropertiesEdit; }
		}
	}
	public class ASPxPropertyEditorProperties : EditPropertiesBase {
		private ASPxPropertyEditor aspxPropertyEditor;
		private object editValue;
		private ASPxEditBase control;
		private CriteriaParameterListProvider parameterListProvider;
		private void propertyEditor_ValueRead(object sender, EventArgs e) {
			((ASPxEditBase)((ASPxPropertyEditor)sender).Editor).Value = editValue;
		}
		private void AllowEdit_ResultValueChanged(object sender, BoolValueChangedEventArgs e) {
			MakeControlEditable();
		}
		private void MakeControlEditable() {
			control.ClientEnabled = true;
			control.ReadOnly = false;
			if(control is ValueWithParametersEdit) {
				ValueWithParametersEdit valueWithParametersEdit = (ValueWithParametersEdit)control;
				if(valueWithParametersEdit.ValueField != null) {
					valueWithParametersEdit.ValueEdit.ClientEnabled = true;
					valueWithParametersEdit.ValueEdit.ReadOnly = false;
				}
			}
		}
		protected override void AssignInplaceProperties(CreateEditorArgs args) {
			base.AssignInplaceProperties(args);
			this.editValue = args.EditValue;
		}
		public string GetDisplayText(object value) {
			return aspxPropertyEditor != null ? string.Format(aspxPropertyEditor.DisplayFormat, value) : null;
		}
		public override string GetDisplayText(CreateDisplayControlArgs args, bool encode) {
			string result = GetDisplayText(args.EditValue);
			return result == null ? base.GetDisplayText(args, encode) : result;
		}
		protected override ASPxEditBase CreateEditInstance() {
			if(aspxPropertyEditor != null) {
				aspxPropertyEditor.AllowEdit.Clear();
				aspxPropertyEditor.ViewEditMode = ViewEditMode.Edit;
				aspxPropertyEditor.CreateControl();
				control = aspxPropertyEditor.Editor as ASPxEditBase;
				aspxPropertyEditor.AllowEdit.SetItemValue("In Criteria", false);
				aspxPropertyEditor.AllowEdit.ResultValueChanged += new EventHandler<BoolValueChangedEventArgs>(AllowEdit_ResultValueChanged);
			}
			if(control != null && aspxPropertyEditor != null) {
				control.Width = Unit.Empty;
				aspxPropertyEditor.ValueRead += new EventHandler(propertyEditor_ValueRead);
				MakeControlEditable();
			}
			if(control == null) {
				control = RenderHelper.CreateASPxTextBox();
			}
			if((editValue == null || ValueWithParametersEdit.IsValueParametrized(editValue)) && parameterListProvider.GetParameterNamesByDataType(aspxPropertyEditor.MemberInfo.MemberType).Count > 0) {
				ValueWithParametersEdit valueWithParametersEdit = new ValueWithParametersEdit();
				valueWithParametersEdit.ValueEdit = control;
				valueWithParametersEdit.SetParameters(parameterListProvider.GetParameterNamesByDataType(aspxPropertyEditor.MemberInfo.MemberType));
				control = valueWithParametersEdit;
			}
			if(!(control is ValueWithParametersEdit)) {
				control.JSProperties["cpHasNoValueWithParametersEdit"] = true;
			}
			control.Value = editValue;
			return control;
		}
		public ASPxEditBase Control {
			get { return control; }
		}
		public ASPxPropertyEditorProperties()
			: base() {
			parameterListProvider = new CriteriaParameterListProvider();
		}
		public ASPxPropertyEditor ASPxPropertyEditor {
			get { return aspxPropertyEditor; }
			set {
				if(aspxPropertyEditor == value)
					return;
				aspxPropertyEditor = value;
				DisplayFormatString = aspxPropertyEditor.DisplayFormat;
			}
		}
	}
	public class FilterControlLookupEditColumn : FilterControlEditColumn, ITestable {
		private string testCaption;
		protected override Type GetPropertyType() {
			return PropertiesLookupEdit.MemberInfo.MemberType;
		}
		protected override EditPropertiesBase CreateEditProperties() {
			return new LookupEditProperties();
		}
		private void Control_Unload(object sender, EventArgs e) {
			OnControlInitialized(sender as Control);
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		public FilterControlLookupEditColumn() : base() { }
		public LookupEditProperties PropertiesLookupEdit {
			get { return (LookupEditProperties)this.PropertiesEdit; }
		}
		public string TestCaption {
			get { return testCaption; }
			set { testCaption = value; }
		}
		#region ITestable Members
		string ITestable.TestCaption {
			get { return TestCaption; }
		}
		string ITestable.ClientId {
			get {
				if(PropertiesLookupEdit.Control == null) {
					PropertiesLookupEdit.CreateControlCore();
					PropertiesLookupEdit.Control.Unload += new EventHandler(Control_Unload);
				}
				return PropertiesLookupEdit.Control.ClientID;
			}
		}
		IJScriptTestControl ITestable.TestControl {
			get { return new JSASPxSimpleLookupTestControl(); }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public virtual TestControlType TestControlType {
			get {
				return TestControlType.Field;
			}
		}
		#endregion
	}
	public class CustomCreateItemsEventArgs : EventArgs {
		public IList Items { get; set; }
	}
	public class LookupEditProperties : EditPropertiesBase {
		private static int displayedItemsCount = 10000;
		private IModelMemberViewItem model;
		private IMemberInfo memberInfo;
		private ITypeInfo objectTypeInfo;
		private IObjectSpace objectSpace;
		private object editValue;
		private ASPxEdit control;
		private CriteriaParameterListProvider parameterListProvider;
		private void FillList(ASPxComboBox comboBox) {
			WebLookupEditorHelper helper = new WebLookupEditorHelper(WebApplication.Instance, objectSpace, memberInfo.MemberTypeInfo, model);
			comboBox.Items.Clear();
			comboBox.Items.Add(WebPropertyEditor.EmptyValue, null);
			comboBox.SelectedIndex = 0;
			ArrayList list = new ArrayList();
			IList collection = GetCollection(helper);
			for(int i = 0; i < Math.Min(collection.Count, displayedItemsCount); i++) {
				list.Add(collection[i]);
			}
			list.Sort(new DisplayValueComparer(helper, WebPropertyEditor.EmptyValue, objectSpace));
			FilterWithObjectsProcessor filterProcessor = new FilterWithObjectsProcessor(objectSpace);
			foreach(object obj in list) {
				comboBox.Items.Add(helper.GetEscapedDisplayText(obj, WebPropertyEditor.EmptyValue, string.Empty), filterProcessor.GetStringForObject(obj));
				if(filterProcessor.GetStringForObject(editValue) == filterProcessor.GetStringForObject(obj)) {
					comboBox.SelectedIndex = list.IndexOf(obj) + 1;
				}
			}
		}
		private IList GetCollection(WebLookupEditorHelper helper) {
			CustomCreateItemsEventArgs args = new CustomCreateItemsEventArgs();
			if(CustomCreateItems != null) {
				CustomCreateItems(this, args);
			}
			if(args.Items == null) {
				Frame frame = helper.Application.CreateFrame(TemplateContext.LookupControl);
				ListView listView = helper.CreateListView(null);
				frame.SetView(listView);
				args.Items = listView.CollectionSource.Collection as IList;
				if(args.Items == null) {
					IListSource listSource = listView.CollectionSource.Collection as IListSource;
					if(listSource != null) {
						args.Items = listSource.GetList();
					}
				}
			}
			return args.Items;
		}
		protected override void AssignInplaceProperties(CreateEditorArgs args) {
			base.AssignInplaceProperties(args);
			this.editValue = args.EditValue;
		}
		protected internal void CreateControlCore() {
			CreateEditInstance();
		}
		protected override ASPxEditBase CreateEditInstance() {
			control = null;
			ASPxComboBox comboBox = RenderHelper.CreateASPxComboBox();
			FillList(comboBox);
			control = comboBox;
			if(editValue == null || ValueWithParametersEdit.IsValueParametrized(editValue)) {
				IList<string> parameterNames = parameterListProvider.GetParameterNamesByDataType(MemberInfo.MemberType);
				if(parameterNames.Count > 0) {
					ValueWithParametersEdit valueWithParametersEdit = new ValueWithParametersEdit();
					valueWithParametersEdit.ValueEdit = control;
					valueWithParametersEdit.SetParameters(parameterNames);
					control = valueWithParametersEdit;
				}
			}
			return control;
		}
		public override EditorType GetEditorType() {
			return EditorType.Lookup;
		}
		public LookupEditProperties()
			: base() {
			parameterListProvider = new CriteriaParameterListProvider();
		}
		public static int DisplayedItemsCount {
			get { return displayedItemsCount; }
			set { displayedItemsCount = value; }
		}
		public ITypeInfo ObjectTypeInfo {
			get {
				return objectTypeInfo;
			}
			set {
				if(objectTypeInfo == value)
					return;
				objectTypeInfo = value;
			}
		}
		public IMemberInfo MemberInfo {
			get {
				return memberInfo;
			}
			set {
				if(memberInfo == value)
					return;
				memberInfo = value;
			}
		}
		public IObjectSpace ObjectSpace {
			get {
				return objectSpace;
			}
			set {
				if(objectSpace == value)
					return;
				objectSpace = value;
			}
		}
		public IModelMemberViewItem Model {
			get { return model; }
			set {
				if(model == value)
					return;
				model = value;
			}
		}
		public ASPxEdit Control {
			get { return control; }
		}
		public event EventHandler<CustomCreateItemsEventArgs> CustomCreateItems;
	}
	public class ReadOnlyParametersValueParser : ICustomValueParser {
		#region ICustomValueParser Members
		public object TryParse(string text, IFilterablePropertyInfo propertyInfo) {
			if(text.StartsWith(CriteriaWrapper.ParameterPrefix) || (propertyInfo != null && propertyInfo.PropertyType.IsEnum)) {
				return text;
			}
			return null;
		}
		public string GetDisplayText(object value, IFilterablePropertyInfo propertyInfo) {
			string result = string.Empty;
			FilterControlPropertyEditorColumn propertyEditorColumn = propertyInfo as FilterControlPropertyEditorColumn;
			if(propertyEditorColumn != null &&
				propertyEditorColumn.PropertiesASPxPropertyEditor.ASPxPropertyEditor != null &&
				!string.IsNullOrEmpty(propertyEditorColumn.PropertiesASPxPropertyEditor.ASPxPropertyEditor.DisplayFormat)) {
				string displayText = propertyEditorColumn.PropertiesASPxPropertyEditor.GetDisplayText(value);
				if(!String.IsNullOrEmpty(displayText)) {
					result = displayText;
				}
			}
			return result;
		}
		#endregion
	}
}
