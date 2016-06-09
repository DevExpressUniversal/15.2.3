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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using DevExpress.Web.Localization;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Web.FilterControl {
	static partial class CssClassName {
		internal const string ExpressionTreeLine = "dxfc-ln";
		internal const string ExpressionTreeElbow = "dxfc-elb";
		internal const string ExpressionTreeNode = "dxfc-nd";
		internal const string ExpressionTreeSubNode = "dxfc-subnd";
		internal const string ExpressionTreeItemEditor = "dxfc-editor";
		internal const string ExpressionTreeNodeFinish = "dxfc-clr";
		internal const string ExpressionTree = "dxfc-tree";
		internal const string TextTabEditor = "dxfc-memo";
		internal const string TextTabValidationSummary = "dxfc-validationSummary";
	}
	public class FilterExpressionTreeContainerControl : ASPxInternalWebControl {
		WebFilterControlRenderHelper renderHelper;
		WebFilterTreeModel model;
		public FilterExpressionTreeContainerControl(WebFilterControlRenderHelper renderHelper, WebFilterTreeModel model) {
			this.renderHelper = renderHelper;
			this.model = model;
		}
		protected WebFilterControlRenderHelper RenderHelper { get { return renderHelper; } }
		protected override void CreateControlHierarchy() {
			Controls.Add(new WebFilterExpressionTreeControl(RenderHelper, model));
		}
	}
	internal class WebFilterExpressionTreeControl : InternalHtmlControl {
		WebFilterTreeModel model;
		public WebFilterExpressionTreeControl(WebFilterControlRenderHelper renderHelper, WebFilterTreeModel model)
			: base(HtmlTextWriterTag.Ul) {
			this.RenderHelper = renderHelper;
			this.model = model;
		}
		protected WebFilterControlRenderHelper RenderHelper {
			get;
			private set;
		}
		protected override void CreateControlHierarchy() {
			RenderUtils.AppendDefaultDXClassName(this, CssClassName.ExpressionTree);
			AddNode(model.RootNode, this);
		}
		void AddNode(INode node, WebControl parent) {
			var nodeControl = WebFilterListItem.Create(node, RenderHelper);
			parent.Controls.Add(nodeControl);
			if(!IsLastChild(node))
				RenderUtils.AppendDefaultDXClassName(nodeControl, CssClassName.ExpressionTreeLine);
			AggregateNode aggregateNode = node as AggregateNode;
			GroupNode groupNode = node as GroupNode;
			if(aggregateNode != null) { 
				nodeControl.ChildrenList = RenderUtils.CreateList(ListType.Bulleted);
				if(aggregateNode.Property != null && aggregateNode.Property.HasChildren)
					AddNode(aggregateNode.AggregateCondition, nodeControl.ChildrenList);
			}
			else if(groupNode != null) {
				nodeControl.ChildrenList = nodeControl.ChildrenList ?? RenderUtils.CreateList(ListType.Bulleted);
				groupNode.SubNodes.ForEach(x => AddNode(x, nodeControl.ChildrenList));
			}
			RenderHelper.AppendDefaultDXClassName(parent);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderHelper.GetTableStyle().AssignToControl(this);
		}
		static bool IsLastChild(INode node) {
			return node.ParentNode == null || node.ParentNode.SubNodes.Last() == node;
		}
	}
	public class WebFilterListItem : InternalHtmlControl {
		protected InternalTable NodeControl;
		protected Node Node { get; private set; }
		protected WebFilterControlRenderHelper RenderHelper { get; private set; }
		protected internal WebControl ChildrenList { get; set; }
		private WebControl ElbowCell { get; set; }
		public WebFilterListItem(WebFilterControlRenderHelper renderHelper, Node node)
			: base(HtmlTextWriterTag.Li) {
			Node = node;
			RenderHelper = renderHelper;
		}
		public static WebFilterListItem Create(INode node, WebFilterControlRenderHelper renderHelper) {
			if(node is ClauseNode)
				return new WebFilterClauseListItem(renderHelper, node as Node);
			if(node is GroupNode)
				return new WebFilterGroupListItem(renderHelper, node as Node);
			throw new ArgumentException("Wrong type", "node");
		}
		protected override void CreateControlHierarchy() {
			CreateItemTable();
			AppendChildren();
			if(Node.ParentNode == null)
				return;
			RenderUtils.AppendDefaultDXClassName(ElbowCell, CssClassName.ExpressionTreeElbow);
			if(Node.ParentNode.ParentNode == null)
				return;
			RenderUtils.AppendDefaultDXClassName(this, CssClassName.ExpressionTreeSubNode);
		}
		void CreateItemTable() {
			WebControl table = RenderUtils.CreateWebControl(HtmlTextWriterTag.Table);
			WebControl tbody = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tbody);
			WebControl row = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
			ElbowCell = RenderUtils.CreateWebControl(HtmlTextWriterTag.Td);
			WebControl nodeCell = RenderUtils.CreateWebControl(HtmlTextWriterTag.Td);
			Controls.Add(table);
			table.Controls.Add(tbody);
			tbody.Controls.Add(row);
			row.Controls.Add(ElbowCell);
			row.Controls.Add(nodeCell);
			NodeControl = RenderUtils.CreateTable();
			NodeControl.Rows.Add(RenderUtils.CreateTableRow());
			RenderUtils.AppendDefaultDXClassName(NodeControl, CssClassName.ExpressionTreeNode);
			nodeCell.Controls.Add(NodeControl);
		}
		protected void AppendChildren() {
			if(ChildrenList == null)
				return;
			WebControl breakline = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			RenderUtils.AppendDefaultDXClassName(breakline, CssClassName.ExpressionTreeNodeFinish); 
			Controls.Add(breakline);
			Controls.Add(ChildrenList);
		}
		protected void AddNodeToken(WebControl control) {
			var cell = RenderUtils.CreateTableCell();
			cell.Controls.Add(control);
			NodeControl.Rows[0].Cells.Add(cell);
		}
	}
	public class WebFilterClauseListItem : WebFilterListItem {
		protected ClauseNode ClauseNode { get { return (ClauseNode) Node; } }
		protected int ValueCount { get { return ClauseNode.AdditionalOperands.Count; } }
		protected WebControl LCAnd { get; private set; }
		protected List<WebControl> Brackets { get; private set; }
		public WebFilterClauseListItem(WebFilterControlRenderHelper renderHelper, Node node)
			: base(renderHelper, node) {
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			var aggregateNode = ClauseNode as AggregateNode;
			AddNodeToken(new WebFilterPropertyHyperLink(RenderHelper, ClauseNode));
			if(aggregateNode != null)
				CreateControls(aggregateNode);
			else
				CreateControls();
			AddNodeToken(new WebFilterRemoveNode(RenderHelper, Node));
		}
		void CreateControls() {
			AddNodeToken(new WebFilterOperationHyperLink(RenderHelper, ClauseNode));
			CreateControlsCore();
		}
		void CreateControls(AggregateNode node) {
			AddNodeToken(new WebFilterAggregateHyperLink(RenderHelper, node));
			if(node.Aggregate == Aggregate.Exists)
				return;	  
			if(node.Aggregate != Aggregate.Count)
				AddNodeToken(new WebFilterAggregatePropertyHyperLink(RenderHelper, node));
			AddNodeToken(new WebFilterOperationHyperLink(RenderHelper, node));
			CreateControlsCore();
		}
		void CreateControlsCore() {
			if(ValueCount > 2)
				AddBracket('(');
			for(int i = 0; i < ValueCount; i++) {
				if(ValueCount > 2 && i > 0)
					AddBracket(',');
				var needCompareWithFieldName = ClauseNode.AdditionalOperands[i] is OperandProperty;
				if(needCompareWithFieldName)
					AddNodeToken(new WebFilterPropertyValueHyperLink(RenderHelper, ClauseNode, i));
				else
					AddNodeToken(new WebFilterClauseNodeValue(RenderHelper, ClauseNode, i));
				if(RenderHelper.FilterOwner.ShowOperandTypeButton)
					AddNodeToken(new WebFilterChangeOperandTypeIcon(RenderHelper, ClauseNode, needCompareWithFieldName, i));
				if(ValueCount == 2 && i == 0)
					AddAndOperator();
			}
			if(ValueCount > 2)
				AddBracket(')');
			if(ClauseNode.IsCollectionValues)
				AddNodeToken(new WebFilterAddNodeValue(RenderHelper, ClauseNode));
		}
		protected void AddAndOperator() {
			this.LCAnd = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			LiteralControl literalControl = RenderUtils.CreateLiteralControl(RenderHelper.GetLocalizedString(ASPxEditorsStringId.FilterControl_BetweenAnd));
			this.LCAnd.Controls.Add(literalControl);
			AddNodeToken(this.LCAnd);
		}
		protected void AddBracket(char symbol) {
			Brackets = Brackets ?? new List<WebControl>();
			WebControl span = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			LiteralControl literalControl = RenderUtils.CreateLiteralControl(symbol.ToString());
			span.Controls.Add(literalControl);
			AddNodeToken(span);
			Brackets.Add(span);
		}
	}
	public class WebFilterGroupListItem : WebFilterListItem {
		protected GroupNode GroupNode {
			get { return (GroupNode) Node; }
		}
		public WebFilterGroupListItem(WebFilterControlRenderHelper renderHelper, Node node)
			: base(renderHelper, node) {
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			AddNodeToken(new WebFilterGroupNodeTypeHyperLink(RenderHelper, GroupNode));
			AddNodeToken(new WebFilterAddConditionNode(RenderHelper, Node));
		}
	}
	public class WebFilterHyperLink : ASPxInternalWebControl {
		Node node;
		WebFilterControlRenderHelper renderHelper;
		HyperLink link;
		public WebFilterHyperLink(WebFilterControlRenderHelper renderHelper, Node node) {
			this.renderHelper = renderHelper;
			this.node = node;
		}
		protected WebFilterControlRenderHelper RenderHelper { get { return renderHelper; } }
		protected Node Node { get { return node; } }
		protected HyperLink Link { get { return link; } }
		protected override void CreateChildControls() {
			this.link = RenderUtils.CreateHyperLink();
			Controls.Add(link);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareLinkControl();
		}
		protected virtual void PrepareLinkControl() { }
	}
	public class WebFilterGroupNodeTypeHyperLink : WebFilterHyperLink {
		public WebFilterGroupNodeTypeHyperLink(WebFilterControlRenderHelper renderHelper, Node node)
			: base(renderHelper, node) {
		}
		protected GroupNode GroupNode { get { return (GroupNode)Node; } }
		protected override void PrepareLinkControl() {
			Link.Text = RenderHelper.GetTextForGroup(GroupNode.NodeType); 
			Link.NavigateUrl = RenderHelper.GetEmptyUrl();
			if(RenderHelper.Enabled)
				Link.Attributes["onclick"] = RenderHelper.GetScriptForGroupLink(Node.GetIndex());
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderHelper.GetGroupTypeStyle().AssignToControl(Link);
		}
	}
	public class WebFilterClauseNodeHyperLinkBase : WebFilterHyperLink {
		public WebFilterClauseNodeHyperLinkBase(WebFilterControlRenderHelper renderHelper, Node node)
			: base(renderHelper, node) {
		}
		protected ClauseNode ClauseNode { get { return (ClauseNode)Node; } }
	}
	public class WebFilterPropertyHyperLink : WebFilterClauseNodeHyperLinkBase {
		public WebFilterPropertyHyperLink(WebFilterControlRenderHelper renderHelper, Node node)
			: base(renderHelper, node) {
		}
		protected override void PrepareLinkControl() {
			Link.Text = RenderHelper.GetColumnDisplayName(ClauseNode.FirstOperand.PropertyName);
			if(RenderHelper.Enabled)
				Link.Attributes["onclick"] = RenderHelper.GetScriptForPropertyNameLink(ClauseNode);
			Link.NavigateUrl = RenderHelper.GetEmptyUrl();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderHelper.GetPropertyNameStyle().AssignToControl(Link);
		}
	}
	public class WebFilterAggregatePropertyHyperLink : WebFilterPropertyHyperLink {
		public WebFilterAggregatePropertyHyperLink(WebFilterControlRenderHelper renderHelper, Node node) : base(renderHelper, node) {}
		AggregateNode AggregateNode { get { return (AggregateNode)ClauseNode; } }
		protected override void PrepareLinkControl() {
			Link.Text = RenderHelper.GetColumnDisplayName(AggregateNode.AggregateProperty.Name);
			if(RenderHelper.Enabled)
				Link.Attributes["onclick"] = RenderHelper.GetScriptForAggregatePropertyNameLink(AggregateNode);
			Link.NavigateUrl = RenderHelper.GetEmptyUrl();
		}
	}
	public class WebFilterPropertyValueHyperLink : WebFilterPropertyHyperLink {
		int Index { get; set; }
		public WebFilterPropertyValueHyperLink(WebFilterControlRenderHelper renderHelper, Node node, int index) : base(renderHelper, node) {
			Index = index;
		}
		protected override void PrepareLinkControl() {
			Link.Text = RenderHelper.GetColumnDisplayName((ClauseNode.AdditionalOperands[Index] as OperandProperty).PropertyName);
			if(RenderHelper.Enabled)
				Link.Attributes["onclick"] = RenderHelper.GetScriptForPropertyValueNameLink(ClauseNode, Index);
			Link.NavigateUrl = RenderHelper.GetEmptyUrl();
		}
	}
	public class WebFilterOperationHyperLink : WebFilterClauseNodeHyperLinkBase {
		public WebFilterOperationHyperLink(WebFilterControlRenderHelper renderHelper, Node node)
			: base(renderHelper, node) {
		}
		protected override void PrepareLinkControl() {
			Link.Text = RenderHelper.GetTextForOperation(ClauseNode.Operation);
			Link.NavigateUrl = RenderHelper.GetEmptyUrl();
			if(RenderHelper.Enabled)
				Link.Attributes["onclick"] = RenderHelper.GetScriptForOperationLink(ClauseNode);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderHelper.GetOperationStyle().AssignToControl(Link);
		}
	}
	public class WebFilterAggregateHyperLink : WebFilterClauseNodeHyperLinkBase {
		public WebFilterAggregateHyperLink(WebFilterControlRenderHelper renderHelper, AggregateNode node)
			: base(renderHelper, node) {
		}
		AggregateNode AggregateNode { get { return (AggregateNode)ClauseNode; } }
		protected override void PrepareLinkControl() {
			Link.Text = RenderHelper.GetTextForAggregate(AggregateNode.Aggregate);
			Link.NavigateUrl = RenderHelper.GetEmptyUrl();
			if(RenderHelper.Enabled)
				Link.Attributes["onclick"] = RenderHelper.GetScriptForAggregateLink(AggregateNode);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderHelper.GetOperationStyle().AssignToControl(Link);
		}
	}
	public class WebFilterNodeControl : ASPxInternalWebControl {
		WebFilterControlRenderHelper renderHelper;
		Node node;
		public WebFilterNodeControl(WebFilterControlRenderHelper renderHelper, Node node) {
			this.renderHelper = renderHelper;
			this.node = node;
		}
		protected WebFilterControlRenderHelper RenderHelper { get { return renderHelper; } }
		protected Node Node { get { return node; } }
	}
	public class WebFilterClauseNodeValue : WebFilterNodeControl {
		int valueIndex;
		public WebFilterClauseNodeValue(WebFilterControlRenderHelper renderHelper, Node node, int valueIndex)
			: base(renderHelper, node) {
			this.valueIndex = valueIndex;
		}
		protected ClauseNode ClauseNode { get { return (ClauseNode)Node; } }
		protected int ValueIndex { get { return valueIndex; } }
		protected OperandValue Value { get { return (OperandValue)ClauseNode.AdditionalOperands[valueIndex]; } }
		protected override void CreateControlHierarchy() {
			string valueText;
			ASPxEditBase editor = RenderHelper.CreateEditor(ClauseNode, ValueIndex, out valueText, this);
			var editorContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			editorContainer.Controls.Add(editor);
			Controls.Add(editorContainer);
			RenderUtils.AppendDefaultDXClassName(editorContainer, CssClassName.ExpressionTreeItemEditor);
			if(!DesignMode) editor.DataBind(); 
			Controls.Add(new WebFilterClauseNodeLinkValue(RenderHelper, ClauseNode, valueText, ValueIndex));
		}
	}
	public class WebFilterRemoveNode : WebFilterNodeControl {
		public WebFilterRemoveNode(WebFilterControlRenderHelper renderHelper, Node node)
			: base(renderHelper, node) {
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(new WebFilterImage(RenderHelper, FilterControlImages.RemoveButtonName, FilterControlImages.RemoveButtonHotName,
					RenderHelper.GetScriptForRemoveNode, "R", Node.GetIndex(), 
					RenderHelper.GetLocalizedString(ASPxEditorsStringId.FilterControl_RemoveConditionHint)));
		}
	}
	public class WebFilterAddNodeValue : WebFilterNodeControl {
		public WebFilterAddNodeValue(WebFilterControlRenderHelper renderHelper, ClauseNode node)
			: base(renderHelper, node) {
		}
		protected ClauseNode ClauseNode { get { return (ClauseNode)Node; } }
		protected override void CreateControlHierarchy() {
			Controls.Add(new WebFilterImage(RenderHelper, FilterControlImages.AddButtonName, FilterControlImages.AddButtonHotName,
					RenderHelper.GetScriptForAddNodeValue, "V", Node.GetIndex(),
					RenderHelper.GetLocalizedString(ASPxEditorsStringId.FilterControl_AddValueHint)));
		}
	}
	public class WebFilterAddConditionNode : WebFilterNodeControl {
		public WebFilterAddConditionNode(WebFilterControlRenderHelper renderHelper, Node node)
			: base(renderHelper, node) {
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(new WebFilterImage(RenderHelper, FilterControlImages.AddButtonName, FilterControlImages.AddButtonHotName,
				RenderHelper.GetScriptForAddConditionNode, "C", Node.GetIndex(),
				RenderHelper.GetLocalizedString(ASPxEditorsStringId.FilterControl_AddConditionHint)));
		}
	}
	public class WebFilterClauseNodeLinkValue : WebFilterNodeControl {
		string value;
		int valueIndex;
		HyperLink link;
		public WebFilterClauseNodeLinkValue(WebFilterControlRenderHelper renderHelper, ClauseNode node, string value, int valueIndex)
			: base(renderHelper, node) {
			this.value = value;
			this.valueIndex = valueIndex;
		}
		ClauseNode ClauseNode { get { return (ClauseNode)Node; } }
		protected string Value { get { return value; } }
		protected int ValueIndex { get { return valueIndex; } }
		protected HyperLink Link { get { return link; } }
		protected override void CreateControlHierarchy() {
			this.link = RenderUtils.CreateHyperLink();
			Controls.Add(link);
			Link.NavigateUrl = RenderHelper.GetEmptyUrl();
			Link.ID = RenderHelper.GetValueLinkId(Node, ValueIndex);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			IFilterablePropertyInfo prop = RenderHelper.GetColumnByClauseNode(ClauseNode);
			Link.Text = RenderHelper.GetCustomValueDisplayText(prop, ClauseNode.GetValue(ValueIndex), Value);
			if(RenderHelper.Enabled)
				Link.Attributes["onclick"] = RenderHelper.GetScriptForNodeValueLink(Node, ValueIndex);
			RenderHelper.GetValueStyle().AssignToControl(Link);
		}
	}
	public delegate string WebFilterImageGetClick(int nodeIndex);
	public class WebFilterChangeOperandTypeIcon : WebFilterNodeControl {
		int ValueIndex { get; set; }
		bool NeedCompareWithFieldName { get; set; }
		public WebFilterChangeOperandTypeIcon(WebFilterControlRenderHelper renderHelper, Node node, bool needCompareWithFieldName, int valueIndex) : base(renderHelper, node) {
			NeedCompareWithFieldName = needCompareWithFieldName;
			ValueIndex = valueIndex;
		}
		protected override void CreateControlHierarchy() {
			WebFilterImageGetClick getScriptForChangeOperandType = nodeIndex => { return RenderHelper.GetScriptForChangeOperandType(nodeIndex, ValueIndex); };
			var operandTypeIcon = new WebFilterImage(RenderHelper, GetImageDefault(), GetImageHot(), getScriptForChangeOperandType, "O", Node.GetIndex(), RenderHelper.GetLocalizedString(ASPxEditorsStringId.FilterControl_ShowOperandTypeButtonHint));
			Controls.Add(operandTypeIcon);
		}
		string GetImageDefault() { return NeedCompareWithFieldName ? FilterControlImages.OperandTypeIconFieldName : FilterControlImages.OperandTypeIconValueName; }
		string GetImageHot() { return NeedCompareWithFieldName ? FilterControlImages.OperandTypeIconFieldHotName : FilterControlImages.OperandTypeIconValueHotName; }
	}
	public class WebFilterImage : ASPxInternalWebControl {
		Image image;
		WebFilterControlRenderHelper renderHelper;
		string imageName;
		string hotImageName;
		WebFilterImageGetClick onClick;
		string imagePrefix;
		int nodeIndex;
		public WebFilterImage(WebFilterControlRenderHelper renderHelper, string imageName, string hotImageName, 
			WebFilterImageGetClick onClick, string imagePrefix, int nodeIndex, string toolTip) {
			this.renderHelper = renderHelper;
			this.imageName = imageName;
			this.hotImageName = hotImageName;
			this.onClick = onClick;
			this.imagePrefix = imagePrefix;
			this.nodeIndex = nodeIndex;
			ToolTip = toolTip;
		}
		protected Image Image { get { return image; } }
		protected WebFilterControlRenderHelper RenderHelper { get { return renderHelper; } }
		protected string ImageName { get { return imageName; } }
		protected ImageProperties ImageProperties { get { return RenderHelper.GetImageProperties(ImageName); } }
		protected string HotImageName { get { return hotImageName; } }
		protected ImageProperties HotImageProperties { get { return RenderHelper.GetImageProperties(HotImageName); } }
		protected WebFilterImageGetClick OnClick { get { return onClick; } }
		protected string ImagePrefix { get { return imagePrefix; } }
		protected int NodeIndex { get { return nodeIndex; } }
		protected string ImageID { get { return "OPI" + ImagePrefix + NodeIndex; } }
		protected override void CreateControlHierarchy() {
			this.image = RenderUtils.CreateImage();
			Controls.Add(Image);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			ImageProperties.AssignToControl(Image, DesignMode);
			RenderHelper.GetImageButtonStyle().AssignToControl(Image);
			if(RenderHelper.Enabled)
				Image.Attributes["onclick"] = onClick(NodeIndex);
			RenderUtils.MergeImageWithItemToolTip(Image, ToolTip);
			Image.ID = ImageID;
		}
		protected override void Render(HtmlTextWriter writer) {
			if(HotImageProperties != null && !HotImageProperties.IsEmpty) {
				StringBuilder stb = new StringBuilder();
				StateScriptRenderHelper helper = new StateScriptRenderHelper(Page, RenderHelper.GetFilterControlContainerClientId());
				helper.AddStyle(null, ImageID, new string[0], HotImageProperties.GetScriptObject(Page), "", IsEnabled());
				if(RenderHelper.Enabled)
					helper.GetCreateHoverScript(stb);
				RenderUtils.WriteScriptHtml(writer, stb.ToString());
			}
			base.Render(writer);
		}
	}
	public class FilterPageControl :  ASPxInternalWebControl {
		protected const string TextTabEditorID = "TextTabEditor";
		protected const string TextTabValidationSummaryID = "TextTabValidationSummary";
		public FilterPageControl(ASPxFilterControlBase filterControl) {
			Owner = filterControl;
		}
		protected ASPxFilterControlBase Owner { get; set; }
		protected ASPxMemo TextTabEditor { get; set; }
		protected WebControl TextTabValidationSummary { get; set; }
		protected WebFilterTreeModel Model { get { return Owner.Model; } }
		protected override void CreateControlHierarchy() {
			var pageControl = CreatePageControl();
			TextTabEditor = CreateTextTabEditor();
			TextTabValidationSummary = CreateTextTabValidationSummary();
			CreateTabs(pageControl);
			Controls.Add(pageControl);
		}
		void CreateTabs(ASPxPageControl pageControl) {
			var tabVisualEditor = new TabPage(Owner.RenderHelper.GetLocalizedString(ASPxEditorsStringId.FilterControl_VisualFilterTabCaption));
			var tabTextEditor = new TabPage(Owner.RenderHelper.GetLocalizedString(ASPxEditorsStringId.FilterControl_TextFilterTabCaption));
			tabVisualEditor.Controls.Add(new FilterExpressionTreeContainerControl(Owner.RenderHelper, Model));
			tabTextEditor.Controls.Add(TextTabEditor);
			tabTextEditor.Controls.Add(TextTabValidationSummary);
			pageControl.TabPages.Add(tabVisualEditor);
			pageControl.TabPages.Add(tabTextEditor);
			pageControl.TabPages[0].ClientEnabled = Model.IsTextTabExpressionValid();
			if(Model.TextTabExpression != null)
				pageControl.ActiveTabIndex = 1;
		}
		protected override void PrepareControlHierarchy() {
			PrepareTextTabEditor();
			PrepareTextTabValidationSummary();
		}
		ASPxPageControl CreatePageControl() {
			var pageControl = new ASPxPageControl();
			pageControl.ID = WebFilterControlRenderHelper.PageControlID;
			pageControl.ParentSkinOwner = Owner;
			pageControl.Width = Unit.Percentage(100);
			return pageControl;
		}
		ASPxMemo CreateTextTabEditor() {
			var textTabEditor = new ASPxMemo();
			textTabEditor.ID = TextTabEditorID;
			textTabEditor.ParentSkinOwner = Owner;
			textTabEditor.Text = GetCorrectTextTabExpression();
			textTabEditor.Width = Unit.Percentage(100);
			return textTabEditor;
		}
		string GetCorrectTextTabExpression() {
			return Model.TextTabExpression != null ? Model.TextTabExpression : Model.FilterString;
		}
		WebControl CreateTextTabValidationSummary() {
			var textTabValidationSummary = RenderUtils.CreateList(ListType.Bulleted);
			textTabValidationSummary.ID = TextTabValidationSummaryID;
			Model.Errors.ForEach(err => textTabValidationSummary.Controls.Add(CreateErrorElement(err)));
			return textTabValidationSummary;
		}
		WebControl CreateErrorElement(CriteriaValidatorError err) {
			var li = RenderUtils.CreateListItem();
			var link = RenderUtils.CreateHyperLink();
			link.Text = err.Text;
			link.NavigateUrl = RenderUtils.AccessibilityEmptyUrl;
			li.Controls.Add(link);
			return li;
		}
		void PrepareTextTabEditor() {
			RenderUtils.AppendDefaultDXClassName(TextTabEditor, CssClassName.TextTabEditor);
		}
		void PrepareTextTabValidationSummary() {
			RenderUtils.AppendDefaultDXClassName(TextTabValidationSummary, CssClassName.TextTabValidationSummary);
		}
	}
}
