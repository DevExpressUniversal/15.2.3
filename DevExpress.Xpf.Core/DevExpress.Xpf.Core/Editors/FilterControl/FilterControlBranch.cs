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
using System.Windows.Controls;
using System.Windows;
using DevExpress.Data.Filtering.Helpers;
using System.Windows.Input;
using DevExpress.Data.Filtering;
using System.Windows.Data;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm.Native;
using System.Linq;
#if !SL
using System.Windows.Markup;
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Editors.Filtering {
	public interface IFilterControlNavigationItem {
		UIElement Child { get; }
		bool ShowPopupMenu();
	}
	public interface IFilterControlNavigationNode {
		IList<UIElement> Children { get; }
		IFilterControlNavigationNode ParentNode { get; }
		IList<IFilterControlNavigationNode> SubNodes { get; }
		void ProcessKeyDown(KeyEventArgs e, UIElement focusedChild);
		bool ShowPopupMenu(UIElement child);
	}
	public abstract partial class FilterControlNodeBase : Control, IFilterControlNavigationNode {
		internal PopupMenu ButtonMenu { get; set; }
		public FilterControlNodeBase() {
			ButtonMenu = new PopupMenu();
			ButtonMenu.IsBranchHeader = true;
			ButtonMenu.ItemClickBehaviour = PopupItemClickBehaviour.CloseCurrentBranch;
			BarNameScope.SetIsScopeOwner(ButtonMenu, true);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Node.SetVisualNode(this);
		}
		protected void CreatePopupMenu(ContentControl button) {
			if(Node == null || Node.Owner == null || Node.Owner.FilterColumns == null)
				return;
			AddItemsToPopupMenu(button);
		}
		protected virtual void AddItemsToPopupMenu(ContentControl button) {
		}
		protected BarButtonItem AddItemToPopupMenu(ContentControl button, string name, object content, ICommand command, object parameter, string imageName) {
			return AddItemToPopupMenu(button, name, content, null, null, command, parameter, imageName);
		}
		protected BarSubItem AddSubMenuToPopupMenu(ContentControl button, string name, object content) {
			BarSubItem item = new BarSubItem() { Name = name, Content = content, IsPrivate = true };
			ButtonMenu.Items.Add(item);
			return item;
		}
		protected BarButtonItem AddItemToPopupMenu(ILinksHolder menu, ContentControl button, string name, object content, ICommand command, object parameter, string imageName) {
			return AddItemToPopupMenu(menu, button, name, content, null, null, command, parameter, imageName);
		}
		protected BarButtonItem AddItemToPopupMenu(ContentControl button, string name, object content, DataTemplate template, DataTemplateSelector templateSelector, ICommand command, object parameter, string imageName) {
			return AddItemToPopupMenu(ButtonMenu, button, name, content, template, templateSelector, command, parameter, imageName);
		}
		protected BarButtonItem AddItemToPopupMenu(ILinksHolder menu, ContentControl button, string name, object content, DataTemplate template, DataTemplateSelector templateSelector, ICommand command, object parameter, string imageName) {
			BarButtonItem barButtonItem = new BarButtonItem() { 
				Name = name, 
				IsPrivate = true, 
				CommandParameter = parameter, 
				Command = command, 
				Content = content, 
				ContentTemplate = template,
			};
#if !SL
			barButtonItem.ContentTemplateSelector = templateSelector;
#endif            
			if(!string.IsNullOrEmpty(imageName))
				barButtonItem.Glyph = ImageHelper.CreateImageFromCoreEmbeddedResource("Editors.Images.FilterControl." + imageName + ".png");
			if(menu == ButtonMenu)
				menu.Items.Add(barButtonItem);
			else
				menu.Items.Add(barButtonItem);
			return barButtonItem;
		}
		protected BarItemLinkSeparator AddSeparatorToPopupMenu() {
			return AddSeparatorToPopupMenu(ButtonMenu);
		}
		protected BarItemLinkSeparator AddSeparatorToPopupMenu(ILinksHolder menu) {
			BarItemLinkSeparator separator = new BarItemLinkSeparator() { };
			menu.Links.Add(separator);
			return separator;
		}
		internal void ButtonMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			ContentControl button = sender as ContentControl;
			button.Focus();
			try {
				if (ClosePopup()) {
					return;
				}
			} finally {
				ButtonMenu.Items.Clear();
			}			
			CreatePopupMenu(button);
			ButtonMenu.Opened += new EventHandler(MenuOpened);
			ButtonMenu.Closed += new EventHandler(MenuClosed);
			ButtonMenu.ShowPopup(button);
		}
		protected bool ClosePopup() {
			if(ButtonMenu.IsOpen) {
				ButtonMenu.ClosePopup();
				return true;
			}
			return false;
		}
		protected void RemoveButtonMouseUpEventHandler(ContentControl button) {
			if(button != null)
				button.RemoveMouseUpHandler(ButtonMouseUp);
		}
		internal void MenuOpened(object sender, EventArgs e) {
			ButtonMenu.Opened -= new EventHandler(MenuOpened);
		}
		internal void MenuClosed(object sender, EventArgs e) {
			ButtonMenu.Closed -= new EventHandler(MenuClosed);
			ButtonMenu.Items.Clear();
		}
		public NodeBase Node { get { return DataContext as NodeBase; } }
		protected virtual IList<UIElement> NavigationChildrenCore { get { return new List<UIElement>(); } }
		protected IList<UIElement> NavigationChildren {
			get {
				IList<UIElement> result = NavigationChildrenCore;
				for(int i = result.Count - 1; i >= 0; i--)
					if(!UIElementHelper.IsVisible(result[i]))
						result.RemoveAt(i);
				return result;
			}
		}
		protected virtual IFilterControlNavigationNode NavigationParentNode {
			get {
				GroupNode parentNode = (GroupNode)Node.ParentNode;
				return (parentNode != null) ? parentNode.VisualNode : null;
			}
		}
		protected virtual IList<IFilterControlNavigationNode> NavigationSubNodes { get { return new List<IFilterControlNavigationNode>(); } }
		protected virtual void NavigationProcessKeyDown(KeyEventArgs e, UIElement focusedChild) {
			if(FilterControlKeyboardHelper.IsDeleteKey(e)) {
				if(NavigationParentNode == null) return;
				Node.Owner.RemoveNode(Node);
				e.Handled = true;
			}
		}
		protected virtual bool NavigationShowPopupMenu(UIElement child) {
			ButtonMouseUp(child, null);
			return true;
		}
		#region IFilterControlNavigationNode Members
		IList<UIElement> IFilterControlNavigationNode.Children {
			get { return NavigationChildren; }
		}
		IFilterControlNavigationNode IFilterControlNavigationNode.ParentNode {
			get { return NavigationParentNode; }
		}
		IList<IFilterControlNavigationNode> IFilterControlNavigationNode.SubNodes {
			get { return NavigationSubNodes; }
		}
		void IFilterControlNavigationNode.ProcessKeyDown(KeyEventArgs e, UIElement focusedChild) {
			NavigationProcessKeyDown(e, focusedChild);
		}
		bool IFilterControlNavigationNode.ShowPopupMenu(UIElement child) {
			return NavigationShowPopupMenu(child);
		}
		#endregion
	}
	public class FilterControlGroupNode : FilterControlNodeBase {
		void ChangeTypeNode(GroupType groupType) {
			GroupNode.NodeType = groupType;
		}
		internal void AddCondtionNode() {
			ClauseNode addedNode = GroupNode.Owner.AddClauseNode(GroupNode);
			if(addedNode != null)
				GroupNode.Owner.FocusNodeChild(addedNode, 0);
		}
		internal void AddGroupNode() {
			NodeBase addedNode = GroupNode.Owner.AddGroup(GroupNode);
			if(addedNode != null)
				GroupNode.Owner.FocusNodeChild(addedNode, 0);
		}
		void RemoveNode() {
			GroupNode.Owner.RemoveNode(GroupNode);
		}
		void ClearAllNodes() {
			GroupNode.Owner.ClearAll();
		}
		void PressCommandButton() {
			ShowOnlyCommandMenu = true;
			ButtonMouseUp(TypeButton, null);
		}
		internal GroupNode GroupNode { get { return Node as GroupNode; } }
		ContentControl TypeButton { get; set; }
		Button AddButton { get; set; }
		Button CommandsButton { get; set; }
		bool ShowGroupCommandsIcon { get { return GroupNode.Owner.ShowGroupCommandsIcon; } }
		bool ShowOnlyCommandMenu { get; set; }
		public ICommand AddCondtionNodeCommand { get; private set; }
		public ICommand PressCommandButtonCommand { get; private set; }
		public ICommand ChangeTypeNodeCommand { get; private set; }
		public ICommand AddGroupNodeCommand { get; private set; }
		public ICommand RemoveNodeCommand { get; private set; }
		public ICommand ClearAllNodesCommand { get; private set; }
		public FilterControlGroupNode()
			: base() {
			this.SetDefaultStyleKey(typeof(FilterControlGroupNode));
			ShowOnlyCommandMenu = false;
			AddCondtionNodeCommand = DelegateCommandFactory.Create<object>(obj => AddCondtionNode(), false);
			PressCommandButtonCommand = DelegateCommandFactory.Create<object>(obj => PressCommandButton(), false);
			ChangeTypeNodeCommand = DelegateCommandFactory.Create<GroupType>(groupType => ChangeTypeNode(groupType), false);
			AddGroupNodeCommand = DelegateCommandFactory.Create<object>(obj => AddGroupNode(), false);
			RemoveNodeCommand = DelegateCommandFactory.Create<object>(obj => RemoveNode(), false);
			ClearAllNodesCommand = DelegateCommandFactory.Create<object>(obj => ClearAllNodes(), false);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			RemoveButtonMouseUpEventHandler(TypeButton);
			TypeButton = GetTemplateChild("PART_TypeControl") as ContentControl;
			TypeButton.AddMouseUpHandler(ButtonMouseUp);
			AddButton = GetTemplateChild("PART_AddButton") as Button;
			AddButton.Command = AddCondtionNodeCommand;
			CommandsButton = GetTemplateChild("PART_GroupCommandsButton") as Button;
		}
		protected override void AddItemsToPopupMenu(ContentControl button) {
			base.AddItemsToPopupMenu(button);
			if(!ShowGroupCommandsIcon || (ShowGroupCommandsIcon && !ShowOnlyCommandMenu)) {
				AddTypeItems(button);
			}
			if(!ShowGroupCommandsIcon) {
				AddSeparatorToPopupMenu();
			}
			if(!ShowGroupCommandsIcon || (ShowGroupCommandsIcon && ShowOnlyCommandMenu)) {
				AddCommandsItems(button);
			}
			ShowOnlyCommandMenu = false;
		}
		void AddTypeItems(ContentControl button) {
			AddItemToPopupMenu(button, "And", EditorLocalizer.GetString(EditorStringId.FilterGroupAnd), ChangeTypeNodeCommand, GroupType.And, "And");
			AddItemToPopupMenu(button, "Or", EditorLocalizer.GetString(EditorStringId.FilterGroupOr), ChangeTypeNodeCommand, GroupType.Or, "Or");
			if(!Node.Owner.SupportDomainDataSource) {
				AddItemToPopupMenu(button, "NotAnd", EditorLocalizer.GetString(EditorStringId.FilterGroupNotAnd), ChangeTypeNodeCommand, GroupType.NotAnd, "NotAnd");
				AddItemToPopupMenu(button, "NotOr", EditorLocalizer.GetString(EditorStringId.FilterGroupNotOr), ChangeTypeNodeCommand, GroupType.NotOr, "NotOr");
			}
		}
		void AddCommandsItems(ContentControl button) {
			AddItemToPopupMenu(button, "AddCondtion", EditorLocalizer.GetString(EditorStringId.FilterGroupAddCondition), AddCondtionNodeCommand, null, "AddCondition");
			if(!Node.Owner.SupportDomainDataSource) {
				AddItemToPopupMenu(button, "AddGroup", EditorLocalizer.GetString(EditorStringId.FilterGroupAddGroup), AddGroupNodeCommand, null, "AddGroup");
			}
			AddSeparatorToPopupMenu();
			if(GroupNode.ParentNode != null) {
				AddItemToPopupMenu(button, "RemoveGroup", EditorLocalizer.GetString(EditorStringId.FilterGroupRemoveGroup), RemoveNodeCommand, null, "RemoveGroup");
			} else {
				AddItemToPopupMenu(button, "ClearAll", EditorLocalizer.GetString(EditorStringId.FilterGroupClearAll), ClearAllNodesCommand, null, "ClearAll");
			}
		}
		protected override IList<UIElement> NavigationChildrenCore {
			get {
				List<UIElement> result = new List<UIElement>();
				if(TypeButton != null)
					result.Add(TypeButton);
				return result;
			}
		}
		protected override IList<IFilterControlNavigationNode> NavigationSubNodes {
			get {
				IList<INode> subNodes = GroupNode.SubNodes;
				List<IFilterControlNavigationNode> result = new List<IFilterControlNavigationNode>();
				foreach(INode node in subNodes)
					result.Add(((NodeBase)node).VisualNode);
				return result;
			}
		}
		protected override void NavigationProcessKeyDown(KeyEventArgs e, UIElement focusedChild) {
			if(FilterControlKeyboardHelper.IsAddKey(e)) {
				AddCondtionNode();
				e.Handled = true;
			} else
				base.NavigationProcessKeyDown(e, focusedChild);
		}
	}
	public class FilterControlClauseNode : FilterControlNodeBase {
		public OperandsCount SecondOperandsCount {
			get { return (OperandsCount)GetValue(SecondOperandsCountProperty); }
			set { SetValue(SecondOperandsCountProperty, value); }
		}
		public ControlTemplate SecondOperandsOneTemplate {
			get { return (ControlTemplate)GetValue(SecondOperandsOneTemplateProperty); }
			set { SetValue(SecondOperandsOneTemplateProperty, value); }
		}
		public ControlTemplate SecondOperandsTwoTemplate {
			get { return (ControlTemplate)GetValue(SecondOperandsTwoTemplateProperty); }
			set { SetValue(SecondOperandsTwoTemplateProperty, value); }
		}
		public ControlTemplate SecondOperandsSeveralTemplate {
			get { return (ControlTemplate)GetValue(SecondOperandsSeveralTemplateProperty); }
			set { SetValue(SecondOperandsSeveralTemplateProperty, value); }
		}
		public ControlTemplate SecondOperandsLocalDateTimeTemplate {
			get { return (ControlTemplate)GetValue(SecondOperandsLocalDateTimeTemplateProperty); }
			set { SetValue(SecondOperandsLocalDateTimeTemplateProperty, value); }
		}
		public static readonly DependencyProperty SecondOperandsCountProperty;
		public static readonly DependencyProperty SecondOperandsOneTemplateProperty;
		public static readonly DependencyProperty SecondOperandsTwoTemplateProperty;
		public static readonly DependencyProperty SecondOperandsSeveralTemplateProperty;
		public static readonly DependencyProperty SecondOperandsLocalDateTimeTemplateProperty;
		static FilterControlClauseNode() {
			Type ownerType = typeof(FilterControlClauseNode);
			SecondOperandsCountProperty = DependencyPropertyManager.Register("SecondOperandsCount", typeof(OperandsCount), ownerType, new PropertyMetadata(OperandsCount.None, (d, e) => ((FilterControlClauseNode)d).UpdateSecondOperandsTemplate()));
			SecondOperandsOneTemplateProperty = DependencyPropertyManager.Register("SecondOperandsOneTemplate", typeof(ControlTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((FilterControlClauseNode)d).UpdateSecondOperandsTemplate()));
			SecondOperandsTwoTemplateProperty = DependencyPropertyManager.Register("SecondOperandsTwoTemplate", typeof(ControlTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((FilterControlClauseNode)d).UpdateSecondOperandsTemplate()));
			SecondOperandsSeveralTemplateProperty = DependencyPropertyManager.Register("SecondOperandsSeveralTemplate", typeof(ControlTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((FilterControlClauseNode)d).UpdateSecondOperandsTemplate()));
			SecondOperandsLocalDateTimeTemplateProperty = DependencyProperty.Register("SecondOperandsLocalDateTimeTemplate", typeof(ControlTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((FilterControlClauseNode)d).UpdateSecondOperandsTemplate()));
		}
		void ChangeFirstOperandNode(string parameter) {
			ClauseNode.FirstOperand = new OperandProperty(parameter);
			FilterColumn column = ClauseNode.Owner.GetColumnByFieldName(parameter);
			if(!((column != null) && column.IsValidClause(ClauseNode.Operation)))
				ClauseNode.Operation = ClauseNode.Owner.GetDefaultOperation(parameter);
			foreach(FilterControlEditor editor in ClauseNode.Editors) {
				editor.ResetEditor();
			}
			FocusNextNavigationChild(FirstOperandButton);
		}
		void ChangeOperationNode(ClauseType clauseType) {
			ClauseNode.Operation = clauseType;
			FocusNextNavigationChild(OperationButton);
		}
		internal void SetClauseNodeType(ClauseType type) {
			ChangeOperationNode(type);
		}
		void AddOperandNode() {
			int index = NavigationChildren.Count;
			ClauseNode.AddAdditionalOperand(new OperandValue());
			Node.Owner.FocusNodeChild(Node, index);
		}
		void RemoveNode() {
			ClauseNode.Owner.RemoveNode(ClauseNode);
		}
		ContentControl FirstOperandButton { get; set; }
		ContentControl OperationButton { get; set; }
		Button DeleteButton { get; set; }
		ContentControl SecondOperandsControl { get; set; }
		public ICommand RemoveNodeCommand { get; private set; }
		public ICommand ChangeFirstOperandNodeCommand { get; private set; }
		public ICommand ChangeOperationNodeCommand { get; private set; }
		public ICommand AddOperandNodeCommand { get; private set; }
		public ICommand ChangeLocalDateTimeFunctionTypeCommand { get; private set; }
		internal ClauseNode ClauseNode { get { return Node as ClauseNode; } }
		protected internal PopupMenu LocalDateTimeMenu { get; set; }
		public FilterControlClauseNode() {
			this.SetDefaultStyleKey(typeof(FilterControlClauseNode));
			RemoveNodeCommand = DelegateCommandFactory.Create<object>(obj => RemoveNode(), false);
			ChangeFirstOperandNodeCommand = DelegateCommandFactory.Create<string>(str => ChangeFirstOperandNode(str), false);
			ChangeOperationNodeCommand = DelegateCommandFactory.Create<ClauseType>(clauseType => ChangeOperationNode(clauseType), false);
			AddOperandNodeCommand = DelegateCommandFactory.Create<object>(obj => AddOperandNode(), false);
			ChangeLocalDateTimeFunctionTypeCommand = DelegateCommandFactory.Create<FunctionOperatorType>(x => ChangeLocalDateTimeFunctionType(x));
			LocalDateTimeMenu = new PopupMenu();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			RemoveButtonMouseUpEventHandler(FirstOperandButton);
			FirstOperandButton = GetTemplateChild("PART_FirstOperand") as ContentControl;
			FirstOperandButton.AddMouseUpHandler(ButtonMouseUp);
			RemoveButtonMouseUpEventHandler(OperationButton);
			OperationButton = GetTemplateChild("PART_Operation") as ContentControl;
			OperationButton.AddMouseUpHandler(ButtonMouseUp);
			SecondOperandsControl = GetTemplateChild("SecondOperandsControl") as ContentControl;
			DeleteButton = GetTemplateChild("PART_Delete") as Button;
			DeleteButton.Command = RemoveNodeCommand;
			if(ClauseNode.IsLocalDateTimeFunction) {
				SecondOperandsControl.AddMouseUpHandler(LocalDateTimeButtonClicked);
			}
			UpdateSecondOperandsTemplate();
		}
		bool IsDateTimeOperatorClause(ClauseType type) {
			return type >= ClauseType.IsBeyondThisYear; 
		}
		protected override void AddItemsToPopupMenu(ContentControl button) {
			base.AddItemsToPopupMenu(button);
			if(button == OperationButton) {
				AddOperationButtonItemsToPopupMenu(OperationButton);
			} else {
				AddFirstOperandButtonItemsToPopupMenu(FirstOperandButton);
			}
		}
		List<BarItem> AddFirstOperandButtonItemsToPopupMenu(ContentControl button) {
			int i = 0;
			List<BarItem> res = new List<BarItem>();
			ICommand barItemCommand = changeColumnCommand ?? ChangeFirstOperandNodeCommand;
			foreach(FilterColumn filtercolumn in ClauseNode.Owner.FilterColumns) {
				BarItem item = AddItemToPopupMenu(button, "Item" + i.ToString(), filtercolumn.ColumnCaption, filtercolumn.HeaderTemplate, filtercolumn.HeaderTemplateSelector, barItemCommand, filtercolumn.FieldName, string.Empty);
				res.Add(item);
				i++;
			}
			return res;
		}
		List<BarItem> AddOperationButtonItemsToPopupMenu(ContentControl button) {
			List<BarItem> res = new List<BarItem>();
			List<ClauseType> mainList = ClauseNode.Owner.GetListOperationsByTypes(ClauseNode.FirstOperand.PropertyName);
			List<ClauseType> dateTimeOperatorsList = new List<ClauseType>();
			foreach(ClauseType type in mainList) {
				if(IsDateTimeOperatorClause(type)) {
					if(ClauseNode.Owner.ShowDateTimeOperators) 
						dateTimeOperatorsList.Add(type);
				}
				else {
					BarItem item = AddItemToPopupMenu(button, type.ToString(), OperationHelper.GetMenuStringByType(type), ChangeOperationNodeCommand, type, type.ToString());
					res.Add(item);
				}
			}
			if(dateTimeOperatorsList.Count > 0) {
				BarSubItem dateTimeOperators = AddSubMenuToPopupMenu(button, "DateTimeOperatorsSubMenu", EditorLocalizer.GetString(EditorStringId.FilterDateTimeOperatorMenuCaption));
				res.Add(dateTimeOperators);
				foreach(ClauseType type in dateTimeOperatorsList)
					AddItemToPopupMenu(dateTimeOperators, button, type.ToString(), OperationHelper.GetMenuStringByType(type), ChangeOperationNodeCommand, type, string.Empty);
			}
			return res;
		}
		protected override IList<UIElement> NavigationChildrenCore {
			get {
				List<UIElement> result = new List<UIElement>();
				if(FirstOperandButton != null)
					result.Add(FirstOperandButton);
				if(OperationButton != null)
					result.Add(OperationButton);
				if (ClauseNode == null)
					return result;
				foreach(FilterControlEditor editor in ClauseNode.Editors) {
					UIElement child = ((IFilterControlNavigationItem)editor).Child;
					if(child != null)
						result.Add(child);
				}
				return result;
			}
		}
		protected override void NavigationProcessKeyDown(KeyEventArgs e, UIElement focusedChild) {
			if(FilterControlKeyboardHelper.IsAddKey(e))
				NavigationProcessAddKey(e, focusedChild);
			else if(FilterControlKeyboardHelper.IsDeleteKey(e) && ClauseNode.SecondOperandsCount == OperandsCount.Several && FilterControlKeyboardHelper.GetParentEditor(focusedChild) != null)
				NavigationProcessDeleteKey(e, focusedChild);
			else
				base.NavigationProcessKeyDown(e, focusedChild);
		}
		void NavigationProcessAddKey(KeyEventArgs e, UIElement focusedChild) {
			int editorsCount = ClauseNode.Editors.Count;
			IList<UIElement> children = NavigationChildren;
			int index = children.IndexOf(focusedChild);
			if(ClauseNode.SecondOperandsCount == OperandsCount.Several &&
					(FilterControlKeyboardHelper.GetParentEditor(focusedChild) != null || index == children.Count - editorsCount - 1)) {
				AddOperandNodeCommand.Execute(null);
				e.Handled = true;
			} else
				NavigationParentNode.ProcessKeyDown(e, null);
		}
		void NavigationProcessDeleteKey(KeyEventArgs e, UIElement focusedChild) {
			FilterControl owner = Node.Owner;
			int firstEditorIndex = NavigationChildren.IndexOf(((IFilterControlNavigationItem)ClauseNode.Editors[0]).Child);
			int index = FilterControlKeyboardHelper.GetParentEditor(focusedChild).Index;
			int editorsCount = ClauseNode.Editors.Count;
			UIElement childToFocus;
			if(editorsCount == 1)
				childToFocus = NavigationChildren[firstEditorIndex - 1];
			else if(index == editorsCount - 1)
				childToFocus = ClauseNode.Editors[editorsCount - 2];
			else
				childToFocus = ClauseNode.Editors[index + 1];
			ClauseNode.RemoveAdditionalOperandAt(index);
			owner.FocusElement(childToFocus);
			e.Handled = true;
		}
		ICommand changeColumnCommand = null;
		internal void CreateColumnsMenu(ContentControl button, ICommand command) {
			changeColumnCommand = command;
			ButtonMouseUp(button, null);
			changeColumnCommand = null;
		}
		void FocusNextNavigationChild(UIElement child) {
			IList<UIElement> navigationChildren = NavigationChildren;
			int index = navigationChildren.IndexOf(child);
			Node.Owner.FocusNodeChild(Node, index + 1);
		}
		void UpdateSecondOperandsTemplate() {
			if(SecondOperandsControl != null)
				SecondOperandsControl.Template = GetSecondOperandsTemplate();
		}
		ControlTemplate GetSecondOperandsTemplate() {
			switch(SecondOperandsCount) {
				case OperandsCount.One:
					return SecondOperandsOneTemplate;
				case OperandsCount.Two:
					return SecondOperandsTwoTemplate;
				case OperandsCount.Several:
					return SecondOperandsSeveralTemplate;
				case OperandsCount.OneLocalDateTime:
					return SecondOperandsLocalDateTimeTemplate;
				default:
					return null;
			}
		}
		void ChangeLocalDateTimeFunctionType(FunctionOperatorType newType) {
			ClauseNode.ResetAdditionalOperand(new FunctionOperator(newType), 0);
		}
		protected internal void LocalDateTimeButtonClicked(object sender, MouseButtonEventArgs e) {
			var bars = CreateBars();
			RepopulateMenu(bars);
			ShowMenu(sender);
		}
		IEnumerable<BarButtonItem> CreateBars() {
			return LocalDateTimeFuncs.Select((x, i) => new BarButtonItem {
				Name = "Item" + i,
				Command = ChangeLocalDateTimeFunctionTypeCommand,
				CommandParameter = x,
				Content = LocalaizableCriteriaToStringProcessor.Process(new FunctionOperator(x)),
			});
		}
		void RepopulateMenu(IEnumerable<BarButtonItem> bars) {
			LocalDateTimeMenu.Items.Clear();
			bars.ForEach(x => LocalDateTimeMenu.Items.Add(x));
		}
		void ShowMenu(object elem) {
			LocalDateTimeMenu.ShowPopup(elem as UIElement);
		}
		static readonly FunctionOperatorType[] LocalDateTimeFuncs = new[] {
			FunctionOperatorType.LocalDateTimeThisYear,
			FunctionOperatorType.LocalDateTimeThisMonth,
			FunctionOperatorType.LocalDateTimeLastWeek,
			FunctionOperatorType.LocalDateTimeThisWeek,
			FunctionOperatorType.LocalDateTimeYesterday,
			FunctionOperatorType.LocalDateTimeToday,
			FunctionOperatorType.LocalDateTimeTomorrow,
			FunctionOperatorType.LocalDateTimeDayAfterTomorrow,
			FunctionOperatorType.LocalDateTimeNextWeek,
			FunctionOperatorType.LocalDateTimeTwoWeeksAway,
			FunctionOperatorType.LocalDateTimeNextMonth,
			FunctionOperatorType.LocalDateTimeNextYear,
		};
	}
	public class AdditionalOperandsItemsControl : ItemsControl {
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			BindingOperations.SetBinding(element, FrameworkElement.TagProperty, new Binding("DataContext") { Source = this });
		}
	}
	public class OperationStringConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return OperationHelper.GetMenuStringByType((ClauseType)value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class GroupTypeConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			string localizationId = "FilterGroup" + ((GroupType)value).ToString();
			if(Enum.IsDefined(typeof(EditorStringId), localizationId))
				return EditorLocalizer.GetString(localizationId);
			return string.Empty;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public static class FilterControlNavigationHelper {
		public enum FilterControlNavigationDirection { Left, Up, Right, Down }
		public static UIElement GetChildToFocus(IFilterControlNavigationNode rootNode, IFilterControlNavigationNode currentNode, UIElement currentChild, FilterControlNavigationDirection direction) {
			if(currentNode == null) {
				if(rootNode.Children.Count > 0)
					return rootNode.Children[0];
				currentNode = rootNode;
				direction = FilterControlNavigationDirection.Down;
			}
			IFilterControlNavigationNode node = currentNode;
			UIElement child = currentChild;
			switch(direction) {
				case FilterControlNavigationDirection.Left:
					for(; ; ) {
						FindChildLeft(ref node, ref child);
						if(child == currentChild) break;
						if(child != null)
							return child;
					};
					break;
				case FilterControlNavigationDirection.Up:
					for(; ; ) {
						node = GetNodeUp(node);
						if(node == currentNode) break;
						IList<UIElement> children = node.Children;
						if(children.Count > 0)
							return children[0];
					};
					break;
				case FilterControlNavigationDirection.Right:
					for(; ; ) {
						FindChildRight(ref node, ref child);
						if(child == currentChild) break;
						if(child != null)
							return child;
					};
					break;
				case FilterControlNavigationDirection.Down:
					for(; ; ) {
						node = GetNodeDown(node, true);
						if(node == currentNode) break;
						IList<UIElement> children = node.Children;
						if(children.Count > 0)
							return children[0];
					};
					break;
			}
			return null;
		}
		static void FindChildLeft(ref IFilterControlNavigationNode currentNode, ref UIElement currentChild) {
			IList<UIElement> children = currentNode.Children;
			int index = children.IndexOf(currentChild);
			if(index > 0) {
				currentChild = children[index - 1];
				return;
			}
			currentNode = GetNodeUp(currentNode);
			children = currentNode.Children;
			currentChild = (children.Count > 0) ? children[children.Count - 1] : null;
		}
		static void FindChildRight(ref IFilterControlNavigationNode currentNode, ref UIElement currentChild) {
			IList<UIElement> children = currentNode.Children;
			int index = children.IndexOf(currentChild);
			if(index < children.Count - 1) {
				currentChild = children[index + 1];
				return;
			}
			currentNode = GetNodeDown(currentNode, true);
			children = currentNode.Children;
			currentChild = (children.Count > 0) ? children[0] : null;
		}
		static IFilterControlNavigationNode GetBottomMostNode(IFilterControlNavigationNode currentNode) {
			IList<IFilterControlNavigationNode> subNodes = currentNode.SubNodes;
			if(subNodes.Count > 0)
				return GetBottomMostNode(subNodes[subNodes.Count - 1]);
			return currentNode;
		}
		static IFilterControlNavigationNode GetNodeDown(IFilterControlNavigationNode currentNode, bool canReturnChild) {
			if(canReturnChild && currentNode.SubNodes.Count > 0)
				return currentNode.SubNodes[0];
			if(currentNode.ParentNode == null) return currentNode;
			IList<IFilterControlNavigationNode> parentSubNodes = currentNode.ParentNode.SubNodes;
			int index = parentSubNodes.IndexOf(currentNode);
			if(index < parentSubNodes.Count - 1)
				return parentSubNodes[index + 1];
			return GetNodeDown(currentNode.ParentNode, false);
		}
		static IFilterControlNavigationNode GetNodeUp(IFilterControlNavigationNode currentNode) {
			if(currentNode.ParentNode == null)
				return GetBottomMostNode(currentNode);
			IList<IFilterControlNavigationNode> parentSubNodes = currentNode.ParentNode.SubNodes;
			int index = parentSubNodes.IndexOf(currentNode);
			if(index > 0)
				return GetBottomMostNode(parentSubNodes[index - 1]);
			return currentNode.ParentNode;
		}
	}
	internal static class FilterControlKeyboardHelper {
		public static FilterControlEditor GetParentEditor(UIElement child) {
			return LayoutHelper.FindLayoutOrVisualParentObject<FilterControlEditor>(child);
		}
		public static bool IsAddKey(KeyEventArgs e) {
			ModifierKeys keyboardModifiers = ModifierKeysHelper.GetKeyboardModifiers(e);
			return (e.Key == Key.Insert || e.Key == Key.Add) && keyboardModifiers == ModifierKeys.None ||
				e.Key == Key.Insert && keyboardModifiers == ModifierKeys.Shift;
		}
		public static bool IsDeleteKey(KeyEventArgs e) {
			ModifierKeys keyboardModifiers = ModifierKeysHelper.GetKeyboardModifiers(e);
			return (e.Key == Key.Delete || e.Key == Key.Subtract) && keyboardModifiers == ModifierKeys.None ||
				e.Key == Key.Delete && keyboardModifiers == ModifierKeys.Shift;
		}
		public static bool IsNavigationKey(KeyEventArgs e, bool isRTL, out FilterControlNavigationHelper.FilterControlNavigationDirection navigationDirection) {
			if(ModifierKeysHelper.NoModifiers(ModifierKeysHelper.GetKeyboardModifiers(e)) || ModifierKeysHelper.IsCtrlPressed(ModifierKeysHelper.GetKeyboardModifiers(e)))
				switch(e.Key) {
					case Key.Left:
						navigationDirection = isRTL ? FilterControlNavigationHelper.FilterControlNavigationDirection.Right : FilterControlNavigationHelper.FilterControlNavigationDirection.Left;
						return true;
					case Key.Up:
						navigationDirection = FilterControlNavigationHelper.FilterControlNavigationDirection.Up;
						return true;
					case Key.Right:
						navigationDirection = isRTL ? FilterControlNavigationHelper.FilterControlNavigationDirection.Left : FilterControlNavigationHelper.FilterControlNavigationDirection.Right;
						return true;
					case Key.Down:
						navigationDirection = FilterControlNavigationHelper.FilterControlNavigationDirection.Down;
						return true;
					case Key.Tab:
						navigationDirection = FilterControlNavigationHelper.FilterControlNavigationDirection.Right;
						return true;
				}
			if (e.Key == Key.Tab && ModifierKeysHelper.IsShiftPressed(ModifierKeysHelper.GetKeyboardModifiers(e))) {
				navigationDirection = FilterControlNavigationHelper.FilterControlNavigationDirection.Left;
				return true;
			}
			navigationDirection = FilterControlNavigationHelper.FilterControlNavigationDirection.Left;
			return false;
		}
		public static bool IsShowMenuKey(System.Windows.Input.KeyEventArgs e) {
			ModifierKeys keyboardModifiers = ModifierKeysHelper.GetKeyboardModifiers(e);
			return e.Key == Key.Space && !ModifierKeysHelper.IsAltPressed(keyboardModifiers) ||
#if !SL
				e.Key == Key.Apps && keyboardModifiers == ModifierKeys.None ||
#endif
				e.Key == Key.Enter && (keyboardModifiers == ModifierKeys.None || keyboardModifiers == ModifierKeys.Shift);
		}
		public static bool IsTabKey(System.Windows.Input.KeyEventArgs e, out bool isShiftPressed) {
			ModifierKeys keyboardModifiers = ModifierKeysHelper.GetKeyboardModifiers(e);
			isShiftPressed = keyboardModifiers == ModifierKeys.Shift;
			return e.Key == Key.Tab && (keyboardModifiers == ModifierKeys.None || isShiftPressed);
		}
	}
	public class FieldInOperationButtonControl : XPFContentControl {
		public static readonly DependencyProperty NodeProperty =
			DependencyPropertyManager.Register("Node", typeof(ClauseNode), typeof(FieldInOperationButtonControl), new PropertyMetadata(null, (d, e) => ((FieldInOperationButtonControl)d).UpdateColumnCaption()));
		public static readonly DependencyProperty OperatorPropertyNameProperty =
			DependencyPropertyManager.Register("OperatorPropertyName", typeof(string), typeof(FieldInOperationButtonControl), new PropertyMetadata(null, (d, e) => ((FieldInOperationButtonControl)d).UpdateColumnCaption()));
		public static readonly DependencyProperty ColumnCaptionProperty =
			DependencyPropertyManager.Register("ColumnCaption", typeof(object), typeof(FieldInOperationButtonControl), new PropertyMetadata(null));
		public ClauseNode Node {
			get { return (ClauseNode)GetValue(NodeProperty); }
			set { SetValue(NodeProperty, value); }
		}
		public string OperatorPropertyName {
			get { return (string)GetValue(OperatorPropertyNameProperty); }
			set { SetValue(OperatorPropertyNameProperty, value); }
		}
		public object ColumnCaption {
			get { return (object)GetValue(ColumnCaptionProperty); }
			set { SetValue(ColumnCaptionProperty, value); }
		}
		void UpdateColumnCaption() {
			if(OperatorPropertyName == null || Node == null) {
				ColumnCaption = string.Empty;
			} else {
				FilterColumn column = Node.Owner.GetColumnByFieldName(OperatorPropertyName);
				if(column != null)
					ColumnCaption = column.ColumnCaption;
			}
		}
	}
}
