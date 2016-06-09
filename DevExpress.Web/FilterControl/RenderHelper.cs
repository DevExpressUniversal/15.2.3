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

using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using DevExpress.XtraEditors.Filtering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using DevExpress.Data;
namespace DevExpress.Web.FilterControl {
	public class WebFilterPropertyDisplayText {
		IFilterControlOwner filterOwner;
		public WebFilterPropertyDisplayText(IFilterControlOwner filterOwner) {
			this.filterOwner = filterOwner;
		}
		protected IFilterControlOwner FilterOwner { get { return filterOwner; } }
		public bool GetValueText(string propertyName, object value, out string displayText) {
			return GetValueText(propertyName, value, true, out displayText);
		}
		public bool GetValueText(string propertyName, object value, bool encodeValue, out string displayText) {
			var column = FilterOwner.GetFilterColumns()[propertyName];
			if(column != null && column.CorrespondingExternalColumn != null) {
				return FilterOwner.TryGetSpecialValueDisplayText(column.CorrespondingExternalColumn, value, encodeValue, out displayText);
			}
			displayText = value != null ? value.ToString() : string.Empty;
			return false;
		}
	}
	public class WebFilterCriteriaDisplayTextGenerator : WebFilterPropertyDisplayText, IDisplayCriteriaGeneratorNamesSource, ILocalaizableCriteriaToStringProcessorOpNamesSource {
		CriteriaOperator criteria;
		WebFilterLocalizerHelper localizer;
		public WebFilterCriteriaDisplayTextGenerator(IFilterControlOwner filterOwner, string filterExpression, bool encodeValue) : 
			this(filterOwner, CriteriaOperator.Parse(filterExpression), encodeValue) {
		}
		public WebFilterCriteriaDisplayTextGenerator(IFilterControlOwner filterOwner, CriteriaOperator criteria, bool encodeValue) : base(filterOwner) {
			EncodeValue = encodeValue;
			this.criteria = DisplayCriteriaGenerator.Process(this, criteria);
			this.localizer = new WebFilterLocalizerHelper();
		}
		public bool EncodeValue { get; private set; }
		protected CriteriaOperator Criteria { get { return criteria; } }
		protected WebFilterLocalizerHelper Localizer { get { return localizer; } }
		public override string ToString() {
			return LocalaizableCriteriaToStringProcessorCore.Process(this, Criteria);
		}
		protected virtual string GetFilterColumnName(string propertyName) {
			var column = FilterOwner.GetFilterColumns()[propertyName];
			if(column == null) return null;
			return column.CorrespondingExternalColumn != null ? column.DisplayName : column.GetFullDisplayName();
		}
		#region IDisplayCriteriaGeneratorNamesSource Members
		string IDisplayCriteriaGeneratorNamesSource.GetDisplayPropertyName(OperandProperty property) {
			if(string.IsNullOrEmpty(property.PropertyName)) return string.Empty;
			string columnName = GetFilterColumnName(property.PropertyName);
			return columnName != null ? columnName : property.PropertyName;
		}
		string IDisplayCriteriaGeneratorNamesSource.GetValueScreenText(OperandProperty property, object value) {
			string displayText;
			if(!object.ReferenceEquals(property, null) && GetValueText(property.PropertyName, value, EncodeValue, out displayText)) 
				return displayText;
			return value != null ? value.ToString() : string.Empty;
		}
		#endregion
		#region ILocalaizableCriteriaToStringProcessorOpNamesSource Members
		string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetBetweenString() {
			return Localizer.GetTextForOperation(ClauseType.Between);
		}
		string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetInString() {
			return Localizer.GetTextForOperation(ClauseType.AnyOf);
		}
		string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetIsNotNullString() {
			return Localizer.GetTextForOperation(ClauseType.IsNotNull);
		}
		string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetIsNullString() {
			return Localizer.GetTextForOperation(ClauseType.IsNull);
		}
		string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetNotLikeString() {
			return Localizer.GetTextForOperation(ClauseType.NotLike);
		}
		string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(Aggregate opType) {
			return opType.ToString();
		}
		string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(FunctionOperatorType opType) {
			switch(opType) { 
				case FunctionOperatorType.Contains: return Localizer.GetTextForOperation(ClauseType.Contains); 
				case FunctionOperatorType.StartsWith: return Localizer.GetTextForOperation(ClauseType.BeginsWith);
				case FunctionOperatorType.EndsWith: return Localizer.GetTextForOperation(ClauseType.EndsWith);
				case FunctionOperatorType.IsNull:
				case FunctionOperatorType.IsNullOrEmpty: 
					return Localizer.GetTextForOperation(ClauseType.IsNull);
			}
			return opType.ToString();
		}
		string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(BinaryOperatorType opType) {
			switch(opType) {
				case BinaryOperatorType.Equal: return Localizer.GetTextForOperation(ClauseType.Equals);
				case BinaryOperatorType.Greater: return Localizer.GetTextForOperation(ClauseType.Greater);
				case BinaryOperatorType.GreaterOrEqual: return Localizer.GetTextForOperation(ClauseType.GreaterOrEqual);
				case BinaryOperatorType.Less: return Localizer.GetTextForOperation(ClauseType.Less);
				case BinaryOperatorType.LessOrEqual: return Localizer.GetTextForOperation(ClauseType.LessOrEqual);
				case BinaryOperatorType.Like: return Localizer.GetTextForOperation(ClauseType.Like);
				case BinaryOperatorType.NotEqual: return Localizer.GetTextForOperation(ClauseType.DoesNotEqual);
			}
			return opType.ToString();
		}
		string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(UnaryOperatorType opType) {
			switch(opType) {
				case UnaryOperatorType.BitwiseNot:
				case UnaryOperatorType.Not:
					return ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.FilterControl_Not);
				case UnaryOperatorType.IsNull:
					return Localizer.GetTextForOperation(ClauseType.IsNull);
			}
			return opType.ToString();
		}
		string ILocalaizableCriteriaToStringProcessorOpNamesSource.GetString(GroupOperatorType opType) {
			if(opType == GroupOperatorType.And) {
				return Localizer.GetTextForGroup(GroupType.And);
			}
			return Localizer.GetTextForGroup(GroupType.Or);
		}
		#endregion
	}
	public class WebFilterLocalizerHelper {
		public WebFilterLocalizerHelper() { }
		public string GetString(ASPxEditorsStringId id) { return ASPxEditorsLocalizer.GetString(id); }
		string[] groupTypeStrings;
		string[] clauseTypeStrings;
		string[] aggregateTypeStrings;
		public string GetTextForGroup(GroupType groupType) {
			if(this.groupTypeStrings == null)
				this.groupTypeStrings = GetString(ASPxEditorsStringId.FilterControl_GroupType).Split(',');
			if((int)groupType >= this.groupTypeStrings.Length)
				return groupType.ToString();
			return this.groupTypeStrings[(int)groupType];
		}
		public string GetTextForOperation(ClauseType clauseType) {
			if(this.clauseTypeStrings == null)
				this.clauseTypeStrings = GetString(ASPxEditorsStringId.FilterControl_ClauseType).Split(',');
			if(clauseType == ClauseType.IsNullOrEmpty)
				clauseType = ClauseType.IsNull;
			if(clauseType == ClauseType.IsNotNullOrEmpty)
				clauseType = ClauseType.IsNotNull;
			if((int)clauseType >= this.clauseTypeStrings.Length)
				return clauseType.ToString();
			return this.clauseTypeStrings[(int)clauseType];
		}
		public string GetTextForAggregate(Aggregate aggregateType) {
			if(this.aggregateTypeStrings == null)
				this.aggregateTypeStrings = GetString(ASPxEditorsStringId.FilterControl_AggregateType).Split(',');
			if((int)aggregateType >= this.aggregateTypeStrings.Length)
				return aggregateType.ToString();
			return this.aggregateTypeStrings[(int)aggregateType];
		}
	}
	public class WebFilterControlRenderHelper {
		public const string PopupMenuFieldNameID = "FieldNamePopup";
		public const string PopupTreeViewFieldNameID = "FieldNameTreeViewPopup";
		public const string PopupMenuOperationID = "OperationPopup";
		public const string PopupMenuAggregateID = "AggregatePopup";
		public const string PopupMenuGroupID = "GroupPopup";
		public const string PageControlID = "FCPC";
		public const char JSDivideChar = '|';
		const string EditorID = "DXEdit";
		const string ValueLinkID = "DXValue";
		ASPxFilterControlBase filterControl;
		WebFilterLocalizerHelper localizer;
		WebFilterPropertyDisplayText propertyDisplayText;
		List<FilterControlEditorExtraHandler> extraHandlers = new List<FilterControlEditorExtraHandler>();
		public WebFilterControlRenderHelper(ASPxFilterControlBase filterControl) {
			this.filterControl = filterControl;
			this.localizer = new WebFilterLocalizerHelper();
			this.propertyDisplayText = new WebFilterPropertyDisplayText(FilterControl);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public WebFilterTreeModel Model { get { return FilterControl.Model; } }
		protected ASPxFilterControlBase FilterControl { get { return filterControl; } }
		public bool IsRightToLeft { get { return (FilterControl as ISkinOwner).IsRightToLeft(); } }
		protected WebFilterLocalizerHelper Localizer { get { return localizer; } }
		protected WebFilterPropertyDisplayText PropertyDisplayText { get { return propertyDisplayText; } }
		public IFilterControlOwner FilterOwner { get { return FilterControl; } }
		public ASPxWebControl ControlOwner { get { return FilterControl; } }
		public FilterControlStyles Styles { get { return FilterControl.RenderStyles; } }
		public FilterControlImages Images { get { return FilterControl.RenderImages; } }
		public bool EnablePopupMenuScrolling { get { return FilterControl.EnablePopupMenuScrolling; } }
		public string FilterValue { get { return FilterControl.FilterValue; } }
		public string AppliedFilterExpression { get { return FilterControl.AppliedFilterExpression; } }
		public string FilterExpression { get { return FilterControl.FilterExpression; } }
		public bool IsFilterExpressionValid() { return FilterControl.IsFilterExpressionValid(); }
		public int ColumnCount { get { return FilterOwner.GetFilterColumns().Count; } }
		public IFilterColumn GetColumn(int index) { return FilterOwner.GetFilterColumns()[index]; }
		public string ClientID { get { return FilterControl.ClientID; } }
		public bool Enabled { get { return FilterControl.Enabled; } }
		internal List<FilterControlEditorExtraHandler> ExtraHandlers { get { return extraHandlers; } }
		protected string GetEditorIndex(Node node, int valueIndex) {
			return (node.GetIndex() * 1000 + valueIndex).ToString();
		}
		protected string GetEditorId(Node node, int valueIndex) {
			return EditorID + GetEditorIndex(node, valueIndex);
		}
		public string GetValueLinkId(Node node, int valueIndex) {
			return ValueLinkID + GetEditorIndex(node, valueIndex);
		}
		public ASPxEditBase CreateEditor(ClauseNode node, int valueIndex, out string displayText, WebControl parent) {	 
			Type type = GetNodeOperandType(node, valueIndex);
			object value = GetValue(node, valueIndex);
			var editorProperties = CreateEditorProperties(node, valueIndex);
			var column = GetColumnByClauseNode(node) as FilterControlColumn;
			var editorCreateArgs = new FilterControlCriteriaValueEditorCreateEventArgs(column, editorProperties, value);
			RaiseCriteriaValueEditorCreate(editorCreateArgs);
			CreateEditControlArgs args = new CreateEditControlArgs(editorCreateArgs.Value, type, 
				FilterControl.GetImagesEditors(), FilterControl.GetStylesEditors(), FilterControl, 
				EditorInplaceMode.StandAlone, true);
			ASPxEditBase baseEditor = editorCreateArgs.EditorProperties is TokenBoxProperties
				? editorCreateArgs.EditorProperties.CreateEdit(args, false, () => new ASPxTextBox())
				: editorCreateArgs.EditorProperties.CreateEdit(args);
			if(FilterControl.SuppressEditorValidation)
				EditorsIntegrationHelper.DisableValidation(baseEditor);
			baseEditor.ID = GetEditorId(node, valueIndex);
			var editorInitializeArgs = new FilterControlCriteriaValueEditorInitializeEventArgs(column, baseEditor, editorCreateArgs.Value);
			RaiseCriteriaValueEditorInitialize(editorInitializeArgs);
			baseEditor.EnableClientSideAPI = true;
			baseEditor.EnableViewState = false;
			baseEditor.ClientVisible = false;
			AddExtraHandler(baseEditor, "LostFocus", "ASPx.FCEditorLostFocus");
			AddExtraHandler(baseEditor, "KeyDown", "ASPx.FCEditorKeyDown");
			AddExtraHandler(baseEditor, "KeyUp", "ASPx.FCEditorKeyUp");
			if(baseEditor.Width.IsEmpty)
				baseEditor.Width = Unit.Pixel(80);
			displayText = GetEditorDisplayText(baseEditor.Properties, node.FirstOperand.PropertyName, editorCreateArgs.Value, type, parent);
			if(FilterControl.DesignMode) baseEditor.Visible = false;
			return baseEditor;
		}
		void RaiseCriteriaValueEditorInitialize(FilterControlCriteriaValueEditorInitializeEventArgs args) {
			if(args.Column != null)
				(FilterControl as IFilterControlOwner).RaiseCriteriaValueEditorInitialize(args);
		}
		void RaiseCriteriaValueEditorCreate(FilterControlCriteriaValueEditorCreateEventArgs args) {
			if(args.Column != null)
				(FilterControl as IFilterControlOwner).RaiseCriteriaValueEditorCreate(args);
		}
		EditPropertiesBase CreateEditorProperties(ClauseNode node, int valueIndex) {
			if(IsNodeOperandDateTimeFunction(node, valueIndex))
				return CreateComboBoxProperties();
			return GetColumnEdit(node);
		}
		EditPropertiesBase CreateComboBoxProperties() {
			var comboBoxProperties = new ComboBoxProperties();
			comboBoxProperties.ValueType = typeof(string);
			comboBoxProperties.DataSource = Enum.GetNames(typeof(FunctionOperatorType)).Where(t => IsDateTimeFunctionOperatorType((FunctionOperatorType)Enum.Parse(typeof(FunctionOperatorType), t)));
			return comboBoxProperties;
		}
		object GetValue(ClauseNode node, int valueIndex) {
			var op = node.AdditionalOperands[valueIndex] as FunctionOperator;
			if(!ReferenceEquals(op, null) && IsDateTimeFunctionOperatorType(op.OperatorType))
				return op.OperatorType.ToString();
			return node.GetValue(valueIndex);
		}
		bool IsNodeOperandDateTimeFunction(ClauseNode node, int valueIndex) {
			var op = node.AdditionalOperands[valueIndex] as FunctionOperator;
			var column = GetColumnByClauseNode(node);
			return column != null && column.PropertyType == typeof(DateTime) && !ReferenceEquals(op, null) && IsDateTimeFunctionOperatorType(op.OperatorType);
		}
		bool IsDateTimeFunctionOperatorType(FunctionOperatorType operatorType) {
			return (int)operatorType >= (int)FunctionOperatorType.LocalDateTimeThisYear && (int)operatorType <= (int)FunctionOperatorType.LocalDateTimeNextYear;
		}
		void AddExtraHandler(ASPxEditBase editor, string eventName, string handlerScript) {
			if(String.IsNullOrEmpty(editor.GetClientSideEventHandler(eventName)))
				editor.SetClientSideEventHandler(eventName, handlerScript);
			else
				ExtraHandlers.Add(new FilterControlEditorExtraHandler(editor, eventName, handlerScript));			
		}
		protected virtual string GetEditorDisplayText(EditPropertiesBase editor, string propertyName, object value, Type type, WebControl parent) {
			string displayText;
			if(propertyDisplayText.GetValueText(propertyName, value, out displayText))
				return GetDisplayText(value, displayText);
			var args = new CreateDisplayControlArgs(value, type, null, null, FilterControl.GetImagesEditors(), FilterControl.GetStylesEditors(), parent, FilterControl.DesignMode);
			return GetDisplayText(value, editor.GetDisplayText(args));
		}
		string GetDisplayText(object value, string displayText) {
			if(string.IsNullOrEmpty(displayText) && value == null) return GetLocalizedString(ASPxEditorsStringId.FilterControl_EmptyEnter);
			if(value != null && value.ToString().Trim().Length < 1) return GetLocalizedString(ASPxEditorsStringId.FilterControl_Empty);
			return displayText.Replace("  ", " &nbsp;");
		}
		public string GetColumnDisplayName(string propertyName) {
			string columnName = GetColumnDisplayNameCore(propertyName);
			return columnName != null ? columnName : propertyName;
		}
		protected virtual string GetColumnDisplayNameCore(string propertyName) {
			var column = Model.FilterProperties[propertyName];
			if(column == null)
				return null;
			var displayName = column.GetFullDisplayName();
			if(string.IsNullOrEmpty(displayName))
				displayName = (column as IFilterColumn).DisplayName;
			return displayName;
		}
		public IFilterColumn GetColumnByClauseNode(ClauseNode node) {
			return FilterOwner.GetFilterColumns()[GetPropertyName(node)];
		}
		protected Type GetNodeOperandType(ClauseNode node, int valueIndex) {			
			if(IsNodeOperandDateTimeFunction(node, valueIndex))
				return typeof(string);
			IFilterColumn column = GetColumnByClauseNode(node);
			return column != null ? column.PropertyType : typeof(string);			
		}
		string GetPropertyName(ClauseNode node) {
			return node.Property != null ? node.Property.GetFullNameWithLists() : node.FirstOperand.PropertyName;
		}
		protected EditPropertiesBase GetColumnEdit(ClauseNode node) {
			var column = GetColumnByClauseNode(node);
			if(column == null) return EditRegistrationInfo.CreatePropertiesByDataType(typeof(string));
			if(column.PropertiesEdit != null) {
				if(column.PropertiesEdit is ColorEditProperties)
					return EditRegistrationInfo.CreatePropertiesByDataType(typeof(string));
				return column.PropertiesEdit;
			}
			return EditRegistrationInfo.CreatePropertiesByDataType(column.PropertyType);
		}
		public void AssignImageToControl(string imageName, Image image) {
			 GetImageProperties(imageName).AssignToControl(image, FilterControl.DesignMode);
		}
		public ImageProperties GetImageProperties(string imageName) {
			return Images.GetImageProperties(FilterControl.Page, imageName);
		}
		public string GetFilterControlContainerClientId() {
			return string.Format("{0}{1}", ClientID, FilterControl.ViewMode == FilterControlViewMode.VisualAndText ? "_" + PageControlID : string.Empty);
		}
		public string GetLocalizedString(ASPxEditorsStringId id) { return Localizer.GetString(id); }
		public string GetTextForGroup(GroupType groupType) { return Localizer.GetTextForGroup(groupType); }
		public string GetTextForOperation(ClauseType clauseType) { return Localizer.GetTextForOperation(clauseType); }
		public string GetTextForAggregate(Aggregate aggregate) { return Localizer.GetTextForAggregate(aggregate); }
		#region JavaScripts
		public string GetScriptForApplyOnClick(string ownerApplyFunc) {
			return string.Format("function(s, e) {{ {0} }}", ownerApplyFunc);
		}
		public string GetScriptForCancelOnClick(string ownerCloseFunc) {
			return string.Format("function(s, e) {{ {0} }}", ownerCloseFunc);
		}
		public string GetScriptForPopupMenuFieldNameOnItemClick() {
			var clientItemType = FilterControl.EnableColumnsTreeView ? "node" : "item";
			return string.Format("function(s, e) {{ ASPx.FCChangeFieldName('{0}', e.{1}.name); }}", ClientID, clientItemType);
		}
		public string GetScriptForPopupTreeViewCloseUp() {
			return string.Format("function(s, e) {{ ASPx.FCPopupTreeViewCollapseAll(s, e); }}");
		}
		public string GetScriptForPopupMenuOperationOnItemClick() {
			return string.Format("function(s, e) {{ ASPx.FCChangeOperation('{0}', e.item); }}", ClientID);
		}
		public string GetScriptForPopupMenuAggregateOnItemClick() {
			return string.Format("function(s, e) {{ ASPx.FCChangeAggregate('{0}', e.item.name); }}", ClientID);
		}
		public string GetScriptForPopupMenuGroupOnItemClick() {
			return string.Format("function(s, e) {{ ASPx.FCChangeGroup('{0}', e.item.name); }}", ClientID);
		}
		public string GetScriptForPropertyNameLink(ClauseNode node) {
			string propertyName = node.Property != null ? node.Property.GetFullName() : string.Empty;
			return GetScriptForPropertyNameLink(node, propertyName, "ASPx.FCChangeFieldName");
		}
		public string GetScriptForPropertyValueNameLink(ClauseNode node, int valueIndex) {
			var property = node.AdditionalOperands[valueIndex] as OperandProperty;
			if(ReferenceEquals(property, null)) return string.Empty;
			return GetScriptForPropertyNameLink(node, property.PropertyName, string.Format("(function(name, fieldName) {{ ASPx.FCChangeOperandPropertyValue(name,{0},{1},fieldName); }})", node.GetIndex(), valueIndex));
		}
		public string GetScriptForPropertyNameLink(ClauseNode node, string propertyName, string itemClickHandler) {
			var complexTypeListColumn = GetParentListProperty(node.Property) as FilterControlComplexTypeColumn;
			return GetScriptForPropertyNameLinkCore(node, propertyName, complexTypeListColumn, itemClickHandler);
		}
		public string GetScriptForAggregatePropertyNameLink(AggregateNode node) {
			var complexTypeListColumn = GetParentListProperty(node.AggregateProperty) as FilterControlComplexTypeColumn;
			return GetScriptForPropertyNameLinkCore(node, node.Property.GetFullName(), complexTypeListColumn, "ASPx.FCChangeAggregateProperty");
		}
		IBoundProperty GetParentListProperty(IBoundProperty boundProperty) {
			if(boundProperty == null)
				return null;
			IBoundProperty property = boundProperty;
			while(boundProperty.Parent != null) {
				boundProperty = boundProperty.Parent;
				if(boundProperty.IsList)
					return boundProperty;
			}
			return null;
		}
		public string GetScriptForPropertyNameLinkCore(ClauseNode node, string propertyName, FilterControlComplexTypeColumn complexTypeListColumn, string itemClickHandler) {
			ShowFieldNamePopupParams popupParams = new ShowFieldNamePopupParams();
			popupParams.SubMenuKey = complexTypeListColumn != null ? complexTypeListColumn.ListPropertyType : string.Empty;
			popupParams.SubMenuDepthLevel = complexTypeListColumn != null ? complexTypeListColumn.GetLevel() + 1 : 0;
			popupParams.ItemClickHandler = itemClickHandler;
			return string.Format("ASPx.FCShowFieldNamePopup('{0}',event,{1},'{2}','{3}')", ClientID, node.GetIndex(), propertyName, HtmlConvertor.ToJSON(popupParams, new JSONOptions() { AddRoundBrackets = false, UseDoubleQuotesMark = true }));
		}
		public string GetScriptForOperationLink(ClauseNode node) {
			var column = node.Property as IFilterColumn;
			string param = column != null ? column.ClauseClass.ToString().Substring(0, 1) : string.Empty;
			return string.Format("ASPx.FCShowOperationPopup('{0}',event,{1},'{2}','{3}')", ClientID, node.GetIndex(), param, GetFullColumnName(column));
		}
		public string GetScriptForAggregateLink(AggregateNode node) {
			var column = node.Property as IFilterColumn;
			var aggregateOperations = node.GetAvailableAggregateOperations();
			var param = GetAggregateClass(aggregateOperations).ToString().Substring(0,1);
			return string.Format("ASPx.FCShowAggregatePopup('{0}',event,{1},'{2}','{3}')", ClientID, node.GetIndex(), param, GetFullColumnName(column));
		}
		string GetFullColumnName(IFilterColumn column) {
			var property = column as IBoundProperty;
			return property != null ? property.GetFullNameWithLists() : string.Empty;
		}
		FilterColumnAggregateClass GetAggregateClass(List<Aggregate> aggregateOperations) {
			if(aggregateOperations.Contains(Aggregate.Sum) || aggregateOperations.Contains(Aggregate.Avg))
				return FilterColumnAggregateClass.SumAvg;
			if(aggregateOperations.Contains(Aggregate.Max) || aggregateOperations.Contains(Aggregate.Min))
				return FilterColumnAggregateClass.MaxMin;
			return FilterColumnAggregateClass.Common;
		}
		public string GetEmptyUrl() { return "javascript:;"; }
		public string GetScriptForGroupLink(int nodeIndex) {
			return string.Format("ASPx.FCShowGroupPopup('{0}',event,{1})", ClientID, nodeIndex);
		}
		public string GetScriptForRemoveNode(int nodeIndex) {
			return string.Format("ASPx.FCRemoveNode('{0}',{1})", ClientID, nodeIndex);
		}
		public string GetScriptForAddConditionNode(int nodeIndex) {
			return string.Format("ASPx.FCAddConditionNode('{0}',{1})", ClientID, nodeIndex);
		}
		public string GetScriptForAddNodeValue(int nodeIndex) {
			return string.Format("ASPx.FCAddValue('{0}',{1})", ClientID, nodeIndex);
		}
		public string GetScriptForNodeValueLink(Node node, int valueIndex) {
			return string.Format("ASPx.FCNodeValueClick('{0}',{1})", ClientID, GetEditorIndex(node, valueIndex));
		}
		public string GetScriptForChangeOperandType(int nodeIndex, int valueIndex) {
			return string.Format("ASPx.FCChangeOperandType('{0}',{1},{2})", ClientID, nodeIndex, valueIndex);
		}
		#endregion
		#region Styles
		public void AppendDefaultDXClassName(WebControl control) {
			RenderUtils.AppendDefaultDXClassName(control, FilterControlStyles.FilterControlPrefix);
		}
		public FilterControlTableStyle GetTableStyle() {
			return MergeStyles<FilterControlTableStyle>(Styles.CreateStyleByName("Table"), Styles.Table);
		}
		public FilterControlLinkStyle GetPropertyNameStyle() {  return GetLinkStyle("PropertyName", Styles.PropertyName); }
		public FilterControlLinkStyle GetGroupTypeStyle() { return GetLinkStyle("GroupType", Styles.GroupType); }
		public FilterControlLinkStyle GetOperationStyle() { return GetLinkStyle("Operation", Styles.Operation); }
		public FilterControlLinkStyle GetValueStyle() { return GetLinkStyle("Value", Styles.Value); }
		public FilterControlImageButtonStyle GetImageButtonStyle() {
			return MergeStyles<FilterControlImageButtonStyle>(Styles.ImageButton);
		}
		protected FilterControlLinkStyle GetLinkStyle(string styleName, FilterControlLinkStyle style) {
			return MergeStyles<FilterControlLinkStyle>(Styles.CreateStyleByName(styleName), style);
		}
		T MergeStyles<T>(params AppearanceStyleBase[] styles) where T : AppearanceStyleBase, new() {
			T target = new T();
			foreach(AppearanceStyleBase style in styles) {
				if(style == null)
					continue;
				target.CopyFrom(style);
			}
			return target;
		}
		#endregion
		public string GetCustomValueDisplayText(IFilterablePropertyInfo propertyInfo, object value, string displayText){
			var e = new FilterControlCustomValueDisplayTextEventArgs(propertyInfo, value, displayText);
			FilterOwner.RaiseCustomValueDisplayText(e);
			var text = e.DisplayText;
			if(e.EncodeHtml && text != displayText)
				text = System.Web.HttpUtility.HtmlEncode(text);
			return text;
		}
	}
	public class ShowFieldNamePopupParams {
		public string SubMenuKey { get; set; }
		public int SubMenuDepthLevel { get; set; }
		public string ItemClickHandler { get; set; }
	}
	internal struct FilterControlEditorExtraHandler {
		public ASPxEditBase Editor;
		public string EventName;
		public string HandlerScript;
		public FilterControlEditorExtraHandler(ASPxEditBase editor, string eventName, string handlerScript) {
			Editor = editor;
			EventName = eventName;
			HandlerScript = handlerScript;
		}
	}
}
