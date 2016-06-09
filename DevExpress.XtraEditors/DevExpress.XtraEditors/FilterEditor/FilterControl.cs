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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Paint;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Accessibility;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors {
	public enum FilterEditorViewMode { Text, Visual, VisualAndText, TextAndVisual }
	public interface IFilterControl {
		void SetFilterColumnsCollection(FilterColumnCollection columns, IDXMenuManager manager);
		void SetDefaultColumn(FilterColumn column);
		void SetViewMode(FilterEditorViewMode view);
		CriteriaOperator FilterCriteria { get; set; }
		bool ShowOperandTypeIcon { get; set; }
		DevExpress.LookAndFeel.UserLookAndFeel LookAndFeel { get; }
		event FilterTextChangedEventHandler FilterTextChanged;
		bool IsFilterCriteriaValid { get; }
		bool UseMenuForOperandsAndOperators { get; set; }
		FilterControlAllowAggregateEditing AllowAggregateEditing { get; set; }
	}
	public interface IFilterControlGetModel {
		FilterTreeNodeModel Model { get; }
	}
	public delegate void FilterChangedEventHandler(object sender, FilterChangedEventArgs e);
	public delegate void FilterTextChangedEventHandler(object sender, FilterTextChangedEventArgs e);
	public delegate void FilterActiveEditorValidatingEventHandler(object sender, FilterActiveEditorValidatingEventArgs e);
	public class FilterTextChangedEventArgs : EventArgs {
		string text;
		bool isValid, canBeShownInTree;
		public FilterTextChangedEventArgs(string text, bool isValid, bool canBeShownInTree) {
			this.text = text;
			this.isValid = isValid;
			this.canBeShownInTree = canBeShownInTree;
		}
		public string Text { get { return text; } }
		public bool IsValid { get { return isValid; } }
		public bool CanBeShownInTree { get { return canBeShownInTree; } }
	}
	public class FilterActiveEditorValidatingEventArgs : CancelEventArgs {
		FilterColumn column;
		object oldValue;
		object newValue;
		ClauseNode node;
		public FilterActiveEditorValidatingEventArgs(FilterColumn column, ClauseNode node, object oldValue, object newValue) {
			this.column = column;
			this.node = node;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		public FilterColumn Column { get { return column; } }
		public ClauseNode Node { get { return node; } }
		public object NewValue { get { return newValue; } }
		public object OldValue { get { return oldValue; } }
	}
	internal class ForceItemClick {
		FilterControl owner = null;
		ElementType type = ElementType.None;
		Point p = Point.Empty;
		public ForceItemClick(FilterControl control) {
			this.owner = control;
		}
		public ElementType Type {
			get { return type; }
			set {
				if(type == value) return;
				bool isAvailable = IsAvailable;
				ElementType oldType = type;
				type = value;
				if(type == ElementType.None && isAvailable) {
					owner.DoEditorValidate();
				}
			}
		}
		public bool IsAvailable {
			get {
				return p != Point.Empty && (
					type == ElementType.Value ||
					type == ElementType.ItemCollection ||
					type == ElementType.NodeAction ||
					type == ElementType.NodeRemove ||
					type == ElementType.NodeAdd ||
					type == ElementType.FieldAction ||
					type == ElementType.CollectionAction);
			}
		}
		public void MouseMove(MouseEventArgs args) {
			if(!IsAvailable) return;
			if(Math.Abs(args.X - p.X) > 3 || Math.Abs(args.Y - p.Y) > 3)
				Type = ElementType.None;
		}
		internal bool BlockValidate(FilterLabelInfoTextViewInfo vi, MouseButtons MouseButtons, Point MousePosition) {
			if(MouseButtons != MouseButtons.Left) return false;
			if(vi == null || vi.InfoText == null) return false;
			NodeEditableElement element = vi.InfoText.Tag as NodeEditableElement;
			if(element == null || element.ElementType == ElementType.None) return false;
			type = element.ElementType;
			p = owner.PointToClient(new Point(MousePosition.X, MousePosition.Y));
			return IsAvailable;
		}
	}
	class FunctionInfo {
		internal FunctionInfo(StringId caption, FunctionOperatorType name)
			: this(caption.ToString(), name.ToString()) {
		}
		public FunctionInfo(string caption, string name) {
			Guard.ArgumentNotNull(caption, "caption");
			Guard.ArgumentNotNull(name, "name");
			this.Caption = caption;
			this.Name = name;
		}
		public string Caption;
		public string Name;
	}
	[DXToolboxItem(DXToolboxItemKind.Regular),
	 Description("Allows an end-user to construct complex filter criteria, and apply them to controls."),
	Designer("DevExpress.XtraEditors.Design.FilterControlDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	ToolboxTabName(AssemblyInfo.DXTabData), Docking(DockingBehavior.Ask),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "FilterControl")
	]
	public class FilterControl : BaseStyleControl, IFilterControl, IFilterControlGetModel {
		internal const string ChildrenSuffix = "...";
		bool criteriaChangedBeforeClick = false;
		FilterControlLabelInfo hotTrackLabelInfo;
		bool showEditors = false, showEditorOnFocus = false;
		AppearanceObject appearanceTreeLine;
		IDXMenuManager menuManager = null;
		BaseEdit activeEditor = null;
		Color groupColor, propertyColor, clauseColor, valueColor, emptyValueColor, disabledColor;
		[ThreadStatic]
		static ImageCollection nodeElements;
		[ThreadStatic]
		static ImageCollection clauseElements;
		[ThreadStatic]
		static ImageCollection groupElements;
		[ThreadStatic]
		static ImageCollection aggregateElements;
		bool refreshEditorPosition = false;
		bool suspendEditorCreate = false;
		bool activeItemInvalidate = false;
		bool showFunctions = true;
		bool showDateTimeOperators = true;
		bool useMenuForOperandsAndOperators = false;
		bool forceAction = false; 
		internal static Color[] DefaultColorValues = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Black, Color.Gray, Color.Gray };
		DevExpress.XtraEditors.VScrollBar vScrollBar;
		DevExpress.XtraEditors.HScrollBar hScrollBar;
		WinFilterTreeNodeModel model;
		private static readonly object filterChanged = new object();
		private static readonly object activeEditorValidating = new object();
		private static readonly object beforeCreateValueEditor = new object();
		private static readonly object beforeShowValueEditor = new object();
		private static readonly object disposeValueEditor = new object();
		private static readonly object showFilterControlMenu = new object();
		private static readonly object onPopupMenuShowing = new object();
		private static readonly object filterStringChanged = new object();
		private static readonly object createCustomRepositoryItem = new object();
		private static readonly object filterTextChanged = new object();
		public event EventHandler<CreateCriteriaParseContextEventArgs> CreateCriteriaParseContext {
			add { this.Model.CreateCriteriaParseContext += value; }
			remove { this.Model.CreateCriteriaParseContext -= value; }
		}
		public event EventHandler<CreateCriteriaCustomParseEventArgs> CreateCriteriaCustomParse {
			add { this.Model.CreateCriteriaCustomParse += value; }
			remove { this.Model.CreateCriteriaCustomParse -= value; }
		}
		public event FilterChangedEventHandler FilterChanged {
			add { this.Events.AddHandler(filterChanged, value); }
			remove { this.Events.RemoveHandler(filterChanged, value); }
		}
		public event FilterActiveEditorValidatingEventHandler ActiveEditorValidating {
			add { this.Events.AddHandler(activeEditorValidating, value); }
			remove { this.Events.RemoveHandler(activeEditorValidating, value); }
		}
		public event EventHandler<CreateCustomRepositoryItemEventArgs> CreateCustomRepositoryItem {
			add { this.Events.AddHandler(createCustomRepositoryItem, value); }
			remove { this.Events.RemoveHandler(createCustomRepositoryItem, value); }
		}
		public event EventHandler FilterStringChanged {
			add { this.Events.AddHandler(filterStringChanged, value); }
			remove { this.Events.RemoveHandler(filterStringChanged, value); }
		}
		public event ShowValueEditorEventHandler BeforeShowValueEditor {
			add { this.Events.AddHandler(beforeShowValueEditor, value); }
			remove { this.Events.RemoveHandler(beforeShowValueEditor, value); }
		}
		public event CreateValueEditorEventHandler BeforeCreateValueEditor {
			add { this.Events.AddHandler(beforeCreateValueEditor, value); }
			remove { this.Events.RemoveHandler(beforeCreateValueEditor, value); }
		}
		public event DisposeValueEditorEventHandler DisposeValueEditor {
			add { this.Events.AddHandler(disposeValueEditor, value); }
			remove { this.Events.RemoveHandler(disposeValueEditor, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'PopupMenuShowing' instead", false)]
		public event ShowFilterControlMenuEventHandler ShowFilterControlMenu {
			add { this.Events.AddHandler(showFilterControlMenu, value); }
			remove { this.Events.RemoveHandler(showFilterControlMenu, value); }
		}
		public event PopupMenuShowingEventHandler PopupMenuShowing {
			add { this.Events.AddHandler(onPopupMenuShowing, value); }
			remove { this.Events.RemoveHandler(onPopupMenuShowing, value); }
		}
		protected internal virtual void RaiseFilterStringChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[filterStringChanged];
			if(handler != null) handler(this, e);
			RaiseFilterTextChanged(e);
		}
		void RaiseFilterTextChanged(EventArgs e) {
			FilterTextChangedEventHandler handler = (FilterTextChangedEventHandler)this.Events[filterTextChanged];
			if(handler != null) handler(this, new FilterTextChangedEventArgs(this.FilterString, true, true));
		}
		protected internal virtual void RaiseFilterChanged(FilterChangedEventArgs e) {
			FilterChangedEventHandler handler = (FilterChangedEventHandler)this.Events[filterChanged];
			if(handler != null) handler(this, e);
			RaiseFilterStringChanged(EventArgs.Empty);
		}
		protected internal virtual void RaiseActiveEditorValidating(FilterActiveEditorValidatingEventArgs e) {
			FilterActiveEditorValidatingEventHandler handler = (FilterActiveEditorValidatingEventHandler)this.Events[activeEditorValidating];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseBeforeShowValueEditor(ShowValueEditorEventArgs e) {
			ShowValueEditorEventHandler handler = (ShowValueEditorEventHandler)this.Events[beforeShowValueEditor];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseBeforeCreateValueEditor(CreateValueEditorEventArgs e) {
			CreateValueEditorEventHandler handler = (CreateValueEditorEventHandler)this.Events[beforeCreateValueEditor];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseDisposeValueEditor(ValueEditorEventArgs e) {
			DisposeValueEditorEventHandler handler = (DisposeValueEditorEventHandler)this.Events[disposeValueEditor];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCreateCustomRepositoryItem(CreateCustomRepositoryItemEventArgs e) {
			EventHandler<CreateCustomRepositoryItemEventArgs> handler = (EventHandler<CreateCustomRepositoryItemEventArgs>)this.Events[createCustomRepositoryItem];
			if(handler != null) handler(this, e);
		}
		[Obsolete()]
		protected internal virtual void RaiseShowFilterControlMenu(ShowFilterControlMenuEventArgs e) {
			ShowFilterControlMenuEventHandler handler = (ShowFilterControlMenuEventHandler)this.Events[showFilterControlMenu];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaisePopupMenuShowing(PopupMenuShowingEventArgs e) {
			PopupMenuShowingEventHandler handler = (PopupMenuShowingEventHandler)this.Events[onPopupMenuShowing];
			if (handler != null) handler(this, e);
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new FilterControlAccessible(this);
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			FilterViewInfo.OnHandleCreated();
		}
		protected override Size DefaultSize { get { return new Size(200, 100); } }
		public FilterControl() {
			this.model = CreateModel();
			this.model.OnNotifyControl += new FilterTreeNodeModel.NotifyControlDelegate(OnModelChanged);
			this.SetStyle(ControlStyles.Selectable, true);
			this.SetStyle(ControlStyles.UserMouse, true);
			this.SetStyle(ControlStyles.ContainerControl, true);
			appearanceTreeLine = CreateAppearance();
			FilterViewInfo.UpdateAppearanceColors();
			if (Model != null) this.Model.CreateTree(null);
			InitScrollBars();
		}
		public void ApplyFilter() {
			Model.ApplyFilter();
		}
		protected virtual WinFilterTreeNodeModel CreateModel() { return new WinFilterTreeNodeModel(this); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public WinFilterTreeNodeModel Model { get { return model; } protected set { this.model = value; } }
		internal static ImageCollection NodeImages {
			get {
				if(nodeElements == null)
					nodeElements = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.FilterEditor.Images.NodeImages.png", typeof(FilterControl).Assembly, new Size(13, 13), Color.Magenta);
				return nodeElements;
			}
		}
		internal static ImageCollection ClauseImages {
			get {
				if(clauseElements == null)
					clauseElements = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.FilterEditor.Images.ClauseImages.png", typeof(FilterControl).Assembly, new Size(13, 13), Color.Magenta);
				return clauseElements;
			}
		}
		internal static ImageCollection AggregateImages {
			get {
				if(aggregateElements == null)
					aggregateElements = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.FilterEditor.Images.AggregateImages.png", typeof(FilterControl).Assembly, new Size(16, 16), Color.Magenta);
				return aggregateElements;
			}
		}
		internal static ImageCollection GroupImages {
			get {
				if(groupElements == null)
					groupElements = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.FilterEditor.Images.GroupImages.png", typeof(FilterControl).Assembly, new Size(13, 13), Color.Magenta);
				return groupElements;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public FilterColumn GetDefaultColumn() {
			return (FilterColumn)Model.GetDefaultProperty();
		}
		public void SetDefaultColumn(FilterColumn column) {
			Model.SetDefaultProperty(column);
		}
		void IFilterControl.SetViewMode(FilterEditorViewMode view) { }
		#region Editors
		[Browsable(false)]
		public BaseEdit ActiveEditor {
			get { return activeEditor; }
		}
		protected internal void DisposeActiveEditor() {
			DisposeActiveEditor(true);
		}
		protected internal void DisposeActiveEditor(bool raiseDisposeEvent) {
			if(activeEditor != null) {
				if(raiseDisposeEvent)
					RaiseDisposeValueEditor(new ValueEditorEventArgs(activeEditor));
				activeEditor.Leave -= new EventHandler(OnEditorLeave);
				activeEditor.Validated -= new EventHandler(GetOnEditorValidated());
				if(FocusedItemType == ElementType.Value) {
					activeEditor.Validating -= new CancelEventHandler(OnValueEditorValidating);
				}
				activeEditor.InplaceType = InplaceType.Grid;
				if(activeEditor.IsHandleCreated) {
					var editor = activeEditor;
					activeEditor.BeginInvoke(new MethodInvoker(() => {
						editor.DestroyHandleCore();
						editor.Dispose();
					}));
				}
				else {
					activeEditor.Dispose();
				}
				activeEditor.Properties.ResetEvents();
				activeEditor = null;
				invalidateScrollPosition = true;
				Refresh(true, false);
			}
		}
		protected internal virtual RepositoryItem GetRepositoryItem(ClauseNode node) {
			FilterColumn column = (FilterColumn)node.GetPropertyForEditing();
			if(column == null || column.ColumnEditor == null) return new RepositoryItemTextEdit();
			return column.ColumnEditor;
		}
		protected virtual void InitRepositoryProperties(RepositoryItem ri) {
			InitRepositoryPropertiesForFilterEdit(ri);
		}
		internal static void InitRepositoryPropertiesForFilterEdit(RepositoryItem ri) {
			ri.AllowFocused = true;
			ri.ReadOnly = false;
			RepositoryItemButtonEdit ribe = ri as RepositoryItemButtonEdit;
			if(ribe != null && ribe.ResetTextEditStyleToStandardInFilterControl)
				ribe.TextEditStyle = TextEditStyles.Standard;
			RepositoryItemTextEdit rite = ri as RepositoryItemTextEdit;
			if(rite != null) {
				rite.AllowNullInput = DefaultBoolean.False;
				rite.ValidateOnEnterKey = false;
			}
		}
		RepositoryItem GetRepositoryItemByType(ElementType type, ClauseNode node) {
			if(type == ElementType.ItemCollection) {
				FilterColumn column = (FilterColumn)node.Property;
				if(column != null) return column.CreateItemCollectionEditor();
			}
			return new RepositoryItemTextEdit();
		}
		bool CanCreateActiveValueEditor() {
			return FocusedItemType == ElementType.Value || FocusedItemType == ElementType.AdditionalOperandParameter || FocusedItemType == ElementType.ItemCollection;
		}
		protected internal void CreateActiveEditor() {
			if(activeEditor != null) return;
			if(FocusedItemType == ElementType.Operation) ShowClauseMenu();
			if(FocusedItemType == ElementType.AggregateOperation) ShowAggregateMenu();
			if(FocusedItemType == ElementType.Property) ShowPropertyMenu();
			if(FocusedItemType == ElementType.AggregateProperty) ShowAggregatedPropertyMenu();
			if(FocusedItemType == ElementType.AdditionalOperandProperty) ShowAdditionalOperandPropertyMenu();
			if(!CanCreateActiveValueEditor()) return;
			ClauseNode node = (ClauseNode)FocusInfo.Node;
			RepositoryItem repositoryItem = FocusedItemType == ElementType.Value ? GetRepositoryItem(node) : GetRepositoryItemByType(FocusedItemType, node);
			object currentValue = FocusInfo.GetCurrentValue();
			CreateValueEditorEventArgs createEditorArgs = new CreateValueEditorEventArgs(repositoryItem, currentValue);
			RaiseBeforeCreateValueEditor(createEditorArgs);
			activeEditor = CreateEditor(createEditorArgs);
			activeEditor.MenuManager = this.MenuManager;
			ShowValueEditorEventArgs args = new ShowValueEditorEventArgs(activeEditor, node, new OperandValue(currentValue), FocusInfo.ElementIndex);
			RaiseBeforeShowValueEditor(args);
			if(args.CustomRepositoryItem != null) {
				DisposeActiveEditor(false);
				activeEditor = CreateEditor(new CreateValueEditorEventArgs(args.CustomRepositoryItem, args.OperandValue.Value));
			}
			activeEditor.Show();
			createEditor = true;
			invalidateScrollPosition = true;
		}
		EventHandler GetOnEditorValidated() {
			if(FocusedItemType == ElementType.Operation) return OnClauseEditorValidated;
			if(FocusedItemType == ElementType.AggregateOperation) return OnAggregateEditorValidated;
			if(FocusedItemType == ElementType.Property || FocusedItemType == ElementType.AdditionalOperandProperty) return OnPropertyEditorValidated;
			return OnValueEditorValidated;
		}
		protected void ShowActiveEditorByMenu(DXPopupMenu menu) {
			if(FocusedItemType == ElementType.Operation) {
				ShowOperationsActiveEditorByMenu(menu);
			}
			if(FocusedItemType == ElementType.AggregateOperation) {
				ShowAggregateOperationsActiveEditorByMenu(menu);
			}
			if(FocusedItemType == ElementType.Property || FocusedItemType == ElementType.AdditionalOperandProperty || FocusedItemType == ElementType.AggregateProperty) {
				ShowPropertiesActiveEditorByMenu(menu);
			}
		}
		protected void ShowOperationsActiveEditorByMenu(DXPopupMenu menu) {
			if(FocusedItemType != ElementType.Operation) return;
			ShowOperationsActiveEditorByMenu(menu, FilterControl.ClauseImages, ((ClauseNode)FocusInfo.Node).Operation);
		}
		protected void ShowAggregateOperationsActiveEditorByMenu(DXPopupMenu menu) {
			if(FocusedItemType != ElementType.AggregateOperation) return;
			ShowOperationsActiveEditorByMenu(menu, FilterControl.AggregateImages, ((AggregateNode)FocusInfo.Node).Aggregate);
		}
		#region Combobox to draw Images
		class ImageComboBoxOperationEdit : ImageComboBoxEdit {
			public ImageComboBoxOperationEdit() { }
			protected override DevExpress.XtraEditors.Popup.PopupBaseForm CreatePopupForm() {
				return new PopupImageComboBoxOperationEditListBoxForm(this);
			}
		}
		class PopupImageComboBoxOperationEditListBoxForm : DevExpress.XtraEditors.Popup.PopupImageComboBoxEditListBoxForm {
			public PopupImageComboBoxOperationEditListBoxForm(ComboBoxEdit ownerEdit)
				: base(ownerEdit) {
			}
			protected override DevExpress.XtraEditors.Popup.PopupListBox CreateListBox() {
				return new PopupImageComboBoxOperationEditListBox(this);
			}
		}
		class PopupImageComboBoxOperationEditListBox : DevExpress.XtraEditors.Popup.PopupImageComboBoxEditListBox {
			public PopupImageComboBoxOperationEditListBox(DevExpress.XtraEditors.Popup.PopupListBoxForm ownerForm) : base(ownerForm) { }
			protected override BaseControlPainter CreatePainter() {
				return new PopupImageListBoxOperationPainter();
			}
		}
		class PopupImageListBoxOperationPainter : DevExpress.XtraEditors.Popup.PopupImageListBoxPainter {
			public PopupImageListBoxOperationPainter() { }
			protected override void DrawItemCore(ControlGraphicsInfoArgs info, BaseListBoxViewInfo.ItemInfo itemInfo, ListBoxDrawItemEventArgs e) {
				base.DrawItemCore(info, itemInfo, e);
				ImageComboBoxOperationItem operationItem = itemInfo.Item as ImageComboBoxOperationItem;
				if(operationItem != null && operationItem.Image != null) {
					ImageListBoxViewInfo.ImageItemInfo item = itemInfo as ImageListBoxViewInfo.ImageItemInfo;
					if(item != null) {
						Utils.Paint.XPaint.Graphics.DrawImage(e.Graphics, operationItem.Image, item.ImageRect);
					}
				}
			}
		}
		class ImageComboBoxOperationItem : ImageComboBoxItem {
			public ImageComboBoxOperationItem([Localizable(true)] string description, object value, int imageIndex) : base(description, value, imageIndex) {
			}
			public Image Image { get; set; }
		}
		#endregion
		void ShowOperationsActiveEditorByMenu(DXPopupMenu menu, object imageList, object editValue) {
			ImageComboBoxEdit imageCombo = new ImageComboBoxOperationEdit();
			imageCombo.Properties.DropDownRows = 20;
			imageCombo.Properties.SmallImages = imageList;
			foreach(DXMenuItem menuItem in menu.Items) {
				if (!menuItem.Visible) continue;
				int imageIndex = menuItem.Tag != null ? (int)menuItem.Tag : -1;
				ImageComboBoxOperationItem comboItem = new ImageComboBoxOperationItem(menuItem.Caption, menuItem.Tag, imageIndex);
				comboItem.Image = menuItem.Image;
				imageCombo.Properties.Items.Add(comboItem);
			}
			SetupOperationsAndPropertiesComboBox(imageCombo, editValue);
		}
		string GetFunctionOperatorName(FunctionOperator functionOperator) {
			if(functionOperator.OperatorType == FunctionOperatorType.Custom) {
				OperandValue operandValue = functionOperator.Operands[0] as OperandValue;
				if(!ReferenceEquals(null, operandValue))
					return operandValue.Value.ToString();
				return functionOperator.Operands[0].ToString();
			}
			return functionOperator.OperatorType.ToString();
		}
		ITreeSelectableItem FindTreeSelectableItem(List<ITreeSelectableItem> rootItems, string functionName) {
			foreach(ITreeSelectableItem item in rootItems) {
				var typed = item as FilterControlViewInfo.FunctionTreeSelectableItem;
				if(typed == null)
					continue;
				if (typed.FunctionName == functionName)
					return item;
				var foundInChildren = FindTreeSelectableItem(item.Children, functionName);
				if(foundInChildren != null)
					return foundInChildren;
			}
			return null;
		}
		protected void ShowPropertiesActiveEditorByMenu(DXPopupMenu menu) {
			if(FocusedItemType != ElementType.Property && FocusedItemType != ElementType.AdditionalOperandProperty && FocusedItemType != ElementType.AggregateProperty) return;
			TreeComboBoxEdit treeCombo = new TreeComboBoxEdit();
			treeCombo.Properties.DropDownRows = 15;
			List<ITreeSelectableItem> rootItems = new List<ITreeSelectableItem>();
			ITreeSelectableItem item = null;
			foreach(DXMenuItem menuItem in menu.Items) {
				if (!menuItem.Visible) continue;
				ITreeSelectableItem currentItem = menuItem.Tag as ITreeSelectableItem;
				if(currentItem == null) continue;
				if(item != currentItem) {
					rootItems.Add(currentItem);
					item = currentItem;
				}
			}
			treeCombo.RootItems = rootItems;
			SetupOperationsAndPropertiesComboBox(treeCombo, null);
			ITreeSelectableItem treeSelectableItem = (FilterColumn)FocusInfo.FocusedFilterProperty;
			if(treeSelectableItem == null) {
				CriteriaOperator criteriaOperator = FocusInfo.Node.Elements[FocusInfo.ElementIndex].AdditionalOperand;
				if(criteriaOperator is FunctionOperator) {
					FunctionOperator functionOperator = (FunctionOperator)criteriaOperator;
					string functionName = GetFunctionOperatorName(functionOperator);
					treeSelectableItem = FindTreeSelectableItem(rootItems, functionName);
				}
			}
			treeCombo.FocusedTreeItem = treeSelectableItem;
		}
		void SetupOperationsAndPropertiesComboBox(ImageComboBoxEdit imageCombo, object editValue) {
			SetUpEditor(imageCombo, editValue);
			imageCombo.Properties.AppearanceDropDown.Font = imageCombo.Properties.Appearance.Font;
			imageCombo.Properties.CloseUp += new CloseUpEventHandler(DoOperationEdtor_CloseUp);
			this.activeEditor = imageCombo;
			this.activeEditor.Show();
			if(IsRightToLeft && FocusedItem != null) {
				activeEditor.Location = new Point(activeEditor.Location.X + FocusedItem.TextElement.Width - activeEditor.Width, activeEditor.Location.Y);
			}
			imageCombo.ShowPopup();
			createEditor = true;
		}
		void DoOperationEdtor_CloseUp(object sender, CloseUpEventArgs e) {
			if(ActiveEditor == null) return;
			BeginInvoke(new MethodInvoker(delegate { DoEditorValidate(); }));
			forceAction = true;
		}
		protected BaseEdit CreateEditor(CreateValueEditorEventArgs e) {
			BaseEdit editor = e.RepositoryItem.CreateEditor();
			editor.Properties.Assign(e.RepositoryItem);
			EditorTweaking(editor);
			e.RepositoryItem.Tag = editor;
			SetUpEditor(editor, e.EditValue);
			return editor;
		}
		void EditorTweaking(BaseEdit edit) {
			if(edit is CheckedComboBoxEdit)
				((CheckedComboBoxEdit)edit).Properties.ForceUpdateEditValue = DefaultBoolean.True;
		}
		void SetUpEditor(BaseEdit editor, object editValue) {
			editor.Properties.Appearance.Assign(FilterViewInfo.PaintAppearance);
			editor.Properties.Appearance.ForeColor = AppearanceValueColor;
			InitRepositoryProperties(editor.Properties);
			editor.CausesValidation = false;
			Model[FocusInfo.Node].ClearViewInfo();
			if(FocusedItem != null) {
				editor.Bounds = LabelInfoHelper.GetEditorBoundsByElement(FocusedItem);
			}
			if(FocusedItemType == ElementType.ItemCollection)
				ItemCollectionHelper.SetNodeEditValue(editor, editValue);
			else
				editor.EditValue = editValue;
			editor.Parent = this;
			editor.Focus();
			prevEditorFocusInfo = FocusInfo;
			editor.Validated += new EventHandler(GetOnEditorValidated());
			if(FocusedItemType == ElementType.Value) {
				editor.Validating += new CancelEventHandler(OnValueEditorValidating);
			}
			editor.Leave += new EventHandler(OnEditorLeave);
			if(FocusedItemType == ElementType.ItemCollection)
				ItemCollectionHelper.InitEditor(editor);
		}
		ForceItemClick forceItemClick = null;
		ForceItemClick ForceItemClick {
			get {
				if(forceItemClick == null)
					forceItemClick = new ForceItemClick(this);
				return forceItemClick;
			}
		}
		void PreEditorValidate() {
			if(ActiveEditor == null) return;
			if(ForceItemClick.BlockValidate(HotTrackItem, MouseButtons, MousePosition)) {
				editorNode = null; 
				return;
			}
			DoEditorValidate();
			OnValidating(new CancelEventArgs()); 
		}
		internal void DoEditorValidate() {
			if(ActiveEditor != null) {
				if(ActiveEditor.DoValidate()) {
					DisposeActiveEditor();
				}
			}
		}
		ClauseNode editorNode = null;
		ClauseNode EditorNode {
			get {
				if(editorNode == null) return FocusInfo.Node as ClauseNode;
				return editorNode;
			}
		}
		void OnEditorLeave(object sender, EventArgs e) {
			editorNode = FocusInfo.Node as ClauseNode;
			PreEditorValidate();
		}
		List<object> GetCollectionValues(BaseEdit editor) {
			List<object> result = new List<object>();
			CheckedComboBoxEdit ccEdit = editor as CheckedComboBoxEdit;
			if(ccEdit == null) return result;
			for(int i = 0; i < ccEdit.Properties.Items.Count; i++) {
				if(ccEdit.Properties.Items[i].CheckState != CheckState.Checked) continue;
				result.Add(ccEdit.Properties.Items[i].Value);
			}
			return result;
		}
		object GetActiveEditorValue() {
			if(ActiveEditor == null) return null;
			return FocusedItemType == ElementType.ItemCollection ? GetCollectionValues(ActiveEditor) : ActiveEditor.EditValue;
		}
		void OnValueEditorValidating(object sender, CancelEventArgs e) {
			if(ActiveEditor == null) return;
			FilterActiveEditorValidatingEventArgs ee = new FilterActiveEditorValidatingEventArgs(
				FocusInfoFilterColumn, FocusInfo.Node as ClauseNode, FocusInfo.GetCurrentValue(), GetActiveEditorValue());
			RaiseActiveEditorValidating(ee);
			e.Cancel = ee.Cancel;
		}
		void OnValueEditorValidated(object sender, EventArgs e) {
			FocusInfo.ChangeElement(GetActiveEditorValue());
			DisposeActiveEditor();
			this.editorNode = null;
		}
		void OnPropertyEditorValidated(object sender, EventArgs e) {
			if(ActiveEditor == null) return;
			ITreeSelectableItemFunctionName functionName = ActiveEditor.EditValue as ITreeSelectableItemFunctionName;
			FilterColumn column = ActiveEditor.EditValue as FilterColumn;
			if(column != null) {
				MenuItemPropertyClick(column.FullName);
			}
			if(functionName != null) {
				MenuItemAdditionalOperandPropertyClick(functionName.FunctionName);
			}
			DisposeActiveEditor();
			this.editorNode = null;
		}
		void OnClauseEditorValidated(object sender, EventArgs e) {
			if(ActiveEditor == null) return;
			ChangeClauseTypeFromUI((ClauseType)ActiveEditor.EditValue);
			DisposeActiveEditor();
			this.editorNode = null;
		}
		void OnAggregateEditorValidated(object sender, EventArgs e) {
			if(ActiveEditor == null) return;
			ChangeAggregateTypeFromUI((Aggregate)ActiveEditor.EditValue);
			DisposeActiveEditor();
			this.editorNode = null;
		}
		[DefaultValue(null), TypeConverter(typeof(FilterControlConverter))]
		public object SourceControl { 
			get { return Model.SourceControl; } 
			set { 
				Model.SourceControl = value;
				if(value != null) {
					UpdateUseMenuForOperandsAndOperators();
				}
			}
		}
		int MaximumMenuItemThreshold = 300;
		void UpdateUseMenuForOperandsAndOperators() {
			if(!UseMenuForOperandsAndOperators)
				return;
			int totalItems = 0;
			const int maxLevel = 5;
			Action<IBoundProperty, int> visit = null;
			visit = (c, l) => {
				totalItems++;
				if(l == maxLevel || totalItems >= MaximumMenuItemThreshold || !c.HasChildren || c.Children == null)
					return;
				foreach(var child in c.Children) {
					visit(child, l + 1);
				}
			};
			if(FilterColumns != null) {
				foreach(IBoundProperty fc in FilterColumns) {
					visit(fc, 0);
				}
			}
			if(totalItems >= MaximumMenuItemThreshold) {
				UseMenuForOperandsAndOperators = false;
				Debug.WriteLine("UseMenuForOperandsAndOperators has been set to False because the property selection menu would contain too many items otherwise.");
			}
		}
		protected IFilterParametersOwner ParametersOwner { get { return Model.ParametersOwner; } }
		protected internal virtual bool ShowParameterTypeIcon { get { return Model.ShowParameterTypeIcon; } }
		internal void RefreshEditorPosition() {
			if(!refreshEditorPosition) return;
			if(ActiveEditor != null && FocusedItem != null)
				if(ActiveEditor.Bounds != LabelInfoHelper.GetEditorBoundsByElement(FocusedItem)) {
					ActiveEditor.Bounds = LabelInfoHelper.GetEditorBoundsByElement(FocusedItem);
				}
			refreshEditorPosition = false;
		}
		public void SetFilterColumnsCollection(FilterColumnCollection columns) {
			SetFilterColumnsCollection(columns, null);
		}
		public void SetFilterColumnsCollection(FilterColumnCollection columns, IDXMenuManager manager) {
			if(columns == null)
				columns = new FilterColumnCollection();
			if (FilterColumns != null)
				FilterColumns.Dispose();
			FilterColumns = columns;
			if(SortFilterColumns) FilterColumns.Sort();
			if(manager != null)
				SetMenuManager(manager);
		}
		[DefaultValue(null), DXCategory(CategoryName.BarManager)]
		public IDXMenuManager MenuManager {
			get { return menuManager; }
			set {
				if(MenuManager == value) return;
				menuManager = value;
			}
		}
		void SetMenuManager(IDXMenuManager manager) {
			menuManager = manager != null ? manager.Clone(FindForm()) : null;
		}
		#endregion
		event FilterTextChangedEventHandler IFilterControl.FilterTextChanged {
			add { this.Events.AddHandler(filterTextChanged, value); }
			remove { this.Events.RemoveHandler(filterTextChanged, value); }
		}
		bool IFilterControl.IsFilterCriteriaValid { get { return true; } }
		FilterTreeNodeModel IFilterControlGetModel.Model { get { return this.Model; } }
		#region Properties
		[Browsable(false)]
		public FilterColumnCollection FilterColumns {
			get { return (FilterColumnCollection)Model.FilterProperties; }
			private set { Model.FilterProperties = value; }
		}
		[Browsable(false)]
		protected internal GroupNode RootNode { get { return Model.RootNode; } }
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string FilterString {
			get { return Model.FilterString; }
			set {
				bool changed = Model.FilterString != value;
				Model.FilterString = value;
				if(changed)
					RaiseFilterStringChanged(EventArgs.Empty);
			}
		}
		[DefaultValue(true)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowCreateDefaultClause {
			get { return Model.AllowCreateDefaultClause; }
			set { Model.AllowCreateDefaultClause = value; }
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CriteriaOperator FilterCriteria {
			get { return Model.FilterCriteria; }
			set {
				bool equals = ReferenceEquals(value, null) ? ReferenceEquals(Model.FilterCriteria, null) : value.Equals(Model.FilterCriteria);
				Model.FilterCriteria = value;
				if(!equals)
					RaiseFilterChanged(new FilterChangedEventArgs(FilterChangedAction.RebuildWholeTree, null));
			}
		}
		[DefaultValue(20), SmartTagProperty("Level Indent", "")]
		public int LevelIndent {
			get { return FilterViewInfo.LevelIndent; }
			set {
				if(value < 0) value = 0;
				if(value > 100) value = 100;
				if(FilterViewInfo.LevelIndent == value) return;
				FilterViewInfo.LevelIndent = value;
				Refresh(true, false);
			}
		}
		[DefaultValue(0), SmartTagProperty("Node Separator Height", "")]
		public int NodeSeparatorHeight {
			get { return FilterViewInfo.NodeSeparatorHeight; }
			set {
				if(value < 0) value = 0;
				if(value > 10) value = 10;
				if(FilterViewInfo.NodeSeparatorHeight == value) return;
				FilterViewInfo.NodeSeparatorHeight = value;
				invalidateScrollPosition = true;
				Refresh(true, false);
			}
		}
		[DefaultValue(true)]
		public bool SortFilterColumns {
			get { return Model.SortProperties; }
			set { Model.SortProperties = value; }
		}
		[DefaultValue(true), Obsolete("Use the ShowOperandCustomFunctions property instead."), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), SmartTagProperty("Show Date Time Constants", "")]
		public bool ShowDateTimeConstants {
			get { return ShowOperandCustomFunctions; }
			set { ShowOperandCustomFunctions = value; }
		}
		[DefaultValue(true), SmartTagProperty("Show Operand Custom Functions", "")]
		public bool ShowOperandCustomFunctions {
			get { return showFunctions; }
			set { showFunctions = value; }
		}
		[DefaultValue(false), SmartTagProperty("Use Menu For Operands And Operators", "")]
		public bool UseMenuForOperandsAndOperators {
			get { return useMenuForOperandsAndOperators; }
			set { useMenuForOperandsAndOperators = value; }
		}
		[DefaultValue(true), SmartTagProperty("Show Date Time Operators", "")]
		public bool ShowDateTimeOperators {
			get { return showDateTimeOperators; }
			set { showDateTimeOperators = value; }
		}
		[DefaultValue(20), SmartTagProperty("Max Operands Count", "")]
		public int MaxOperandsCount {
			get { return Model.MaxOperandsCount; }
			set {
				Model.MaxOperandsCount = value;
			}
		}
		[DefaultValue(false), SmartTagProperty("Show Operand Type Icon", "")]
		public bool ShowOperandTypeIcon {
			get { return Model.ShowOperandTypeIcon; }
			set {
				Model.ShowOperandTypeIcon = value;
				RefreshTreeAfterNodeChange();
			}
		}
		[DefaultValue(false), SmartTagProperty("Show Group Commands Icon", "")]
		public bool ShowGroupCommandsIcon {
			get { return Model.ShowGroupCommandsIcon; }
			set { Model.ShowGroupCommandsIcon = value; }
		}
		[DefaultValue(FilterControlAllowAggregateEditing.No), SmartTagProperty("Allow Aggregate Editing", "")]
		public FilterControlAllowAggregateEditing AllowAggregateEditing {
			get { return Model.AllowAggregateEditing; }
			set { Model.AllowAggregateEditing = value; }
		}
		[DefaultValue(false), SmartTagProperty("Show Is Null Operators For Strings", "")]
		public bool ShowIsNullOperatorsForStrings {
			get { return Model.ShowIsNullOperatorsForStrings; }
			set { Model.ShowIsNullOperatorsForStrings = value; }
		}
		[DefaultValue(false)]
		internal bool ShowEditors {
			get { return showEditors; }
			set {
				if(showEditors == value) return;
				showEditors = value;
				Refresh(true, false);
			}
		}
		[DefaultValue(false)]
		internal bool ShowEditorOnFocus {
			get { return showEditorOnFocus; }
			set {
				if(showEditorOnFocus == value) return;
				showEditorOnFocus = value;
			}
		}
		[DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject AppearanceTreeLine {
			get { return appearanceTreeLine; }
		}
		bool ShouldSerializeAppearanceTreeLine() { return AppearanceTreeLine.ShouldSerialize(); }
		[Browsable(false)]
		public FilterControlViewInfo FilterViewInfo {
			get { return (FilterControlViewInfo)this.ViewInfo; }
		}
		[Browsable(false)]
		internal LabelInfoViewInfo HotTrackLabelInfoViewInfo {
			get {
				if(hotTrackLabelInfo != null)
					return hotTrackLabelInfo.ViewInfo;
				return null;
			}
		}
		[Browsable(false)]
		internal FilterLabelInfoTextViewInfo HotTrackItem {
			get {
				if(HotTrackLabelInfoViewInfo != null)
					return HotTrackLabelInfoViewInfo.ActiveItem as FilterLabelInfoTextViewInfo;
				return null;
			}
		}
		[Browsable(false)]
		internal FilterLabelInfoTextViewInfo FocusedItem {
			get {
				if (FocusInfo.Node == null || !Model.IsLabelRegistered(FocusInfo.Node)) return null;
				for (int i = 0; i < Model[FocusInfo.Node].ViewInfo.Count; i++) {
					FilterLabelInfoTextViewInfo element = Model[FocusInfo.Node].ViewInfo[i] as FilterLabelInfoTextViewInfo;
					NodeEditableElement nodeElement = element.InfoText.Tag as NodeEditableElement;
					if(nodeElement != null && nodeElement.CreateFocusInfo() == FocusInfo) {
						return element;
					}
				}
				return null;
			}
		}
		[Browsable(false)]
		internal ElementType FocusedItemType {
			get {  return FocusInfo.FocusedElementType; }
		}
		FilterControlFocusInfo prevEditorFocusInfo = new FilterControlFocusInfo();
		protected internal FilterControlFocusInfo FocusInfo {
			get { return Model.FocusInfo; }
			set { Model.FocusInfo = value; }
		}
		FilterColumn FocusInfoFilterColumn {
			get {
				ClauseNode node = FocusInfo.Node as ClauseNode;
				if(node == null) return null;
				return node.Property as FilterColumn;
			}
		}
		protected internal virtual void OnFocusedElementChanged() {
			DisposeActiveEditor();
			if(!suspendEditorCreate && ShowEditorOnFocus && (FocusedItemType == ElementType.Value || FocusedItemType == ElementType.AdditionalOperandParameter || FocusedItemType == ElementType.ItemCollection)) {
				refreshEditorPosition = prevEditorFocusInfo.Node == FocusInfo.Node;
				CreateActiveEditor();
			}
			MakeFocusedNodeVisible();
			Invalidate();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override IStyleController StyleController {
			get { return null; }
			set { }
		}
		#endregion
		#region Colors
		[DXCategory(CategoryName.Appearance)]
		public Color AppearanceGroupOperatorColor {
			get { return groupColor; }
			set {
				if(groupColor == value) return;
				groupColor = value;
				FilterViewInfo.UpdateAppearanceColors();
			}
		}
		bool ShouldSerializeAppearanceGroupOperatorColor() { return !AppearanceGroupOperatorColor.IsEmpty; }
		void ResetAppearanceGroupOperatorColor() { AppearanceGroupOperatorColor = Color.Empty; }
		[DXCategory(CategoryName.Appearance)]
		public Color AppearanceFieldNameColor {
			get { return propertyColor; }
			set {
				if(propertyColor == value) return;
				propertyColor = value;
				FilterViewInfo.UpdateAppearanceColors();
			}
		}
		bool ShouldSerializeAppearanceFieldNameColor() { return !AppearanceFieldNameColor.IsEmpty; }
		void ResetAppearanceFieldNameColor() { AppearanceFieldNameColor = Color.Empty; }
		[DXCategory(CategoryName.Appearance)]
		public Color AppearanceOperatorColor {
			get { return clauseColor; }
			set {
				if(clauseColor == value) return;
				clauseColor = value;
				FilterViewInfo.UpdateAppearanceColors();
			}
		}
		bool ShouldSerializeAppearanceOperatorColor() { return !AppearanceOperatorColor.IsEmpty; }
		void ResetAppearanceOperatorColor() { AppearanceOperatorColor = Color.Empty; }
		[DXCategory(CategoryName.Appearance)]
		public Color AppearanceValueColor {
			get { return valueColor; }
			set {
				if(valueColor == value) return;
				valueColor = value;
				FilterViewInfo.UpdateAppearanceColors();
			}
		}
		bool ShouldSerializeAppearanceValueColor() { return !AppearanceValueColor.IsEmpty; }
		void ResetAppearanceValueColor() { AppearanceValueColor = Color.Empty; }
		[DXCategory(CategoryName.Appearance)]
		public Color AppearanceEmptyValueColor {
			get { return emptyValueColor; }
			set {
				if(emptyValueColor == value) return;
				emptyValueColor = value;
				FilterViewInfo.UpdateAppearanceColors();
			}
		}
		bool ShouldSerializeAppearanceEmptyValueColor() { return !AppearanceEmptyValueColor.IsEmpty; }
		void ResetAppearanceEmptyValueColor() { AppearanceEmptyValueColor = Color.Empty; }
		[DXCategory(CategoryName.Appearance)]
		public Color AppearanceDisabledColor {
			get { return disabledColor; }
			set {
				if(disabledColor == value) return;
				disabledColor = value;
				FilterViewInfo.UpdateAppearanceColors();
			}
		}
		bool ShouldSerializeAppearanceDisabledColor() { return !AppearanceDisabledColor.IsEmpty; }
		void ResetAppearanceDisabledColor() { AppearanceDisabledColor = Color.Empty; }
		#endregion
		#region Resizeable
		protected override bool SizeableIsCaptionVisible {
			get {
				return false;
			}
		}
		protected override Size CalcSizeableMaxSize() {
			return Size.Empty;
		}
		protected override Size CalcSizeableMinSize() {
			return new Size(Model[this.RootNode].NodeWidth + FilterControlViewInfo.LeftIndent, Model[RootNode].Height + FilterControlViewInfo.TopIndent);
		}
		#endregion
		internal void SetActiveLabelInfo(int x, int y) {
			SetActiveLabelInfo(new Point(x, y));
		}
		internal void SetActiveLabelInfo(Point p) {
			FilterControlLabelInfo li = Model.GetLabelInfoByCoordinates(p.X, p.Y);
			if(li == hotTrackLabelInfo) return;
			if(hotTrackLabelInfo != null)
				hotTrackLabelInfo.ViewInfo.OnMouseLeave();
			hotTrackLabelInfo = li;
			if(hotTrackLabelInfo == null) this.Invalidate();
		}
		protected override void Dispose(bool disposing) {
			fDisposing = true;
			if(disposing) {
				DestroyAppearance(AppearanceTreeLine);
				if(FilterColumns != null)
					FilterColumns.Dispose();
				DisposeActiveEditor();
				Model.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void OnStyleChanged(object sender, EventArgs e) {
			base.OnStyleChanged(sender, e);
			FilterViewInfo.ClearLineBrush();
			FilterViewInfo.ClearViewInfo();
		}
		public Node CreateCriteriaByDefaultColumn() {
			return Model.CreateCriteriaByDefaultProperty();
		}
		void RefreshTreeAfterNodeChange() {
			invalidateScrollPosition = true;
			Refresh(true, false);
		}
		protected internal virtual void Refresh(bool clearNodesViewInfo, bool clearViewInfo) {
			if(clearViewInfo) FilterViewInfo.ClearViewInfo();
			if(clearNodesViewInfo) {
				Model[RootNode].ClearViewInfo();
				activeItemInvalidate = true;
			}
			Model[RootNode].UpdateBounds();
			LayoutChanged();
		}
		internal void UpdateHotTrackItem() {
			if(!activeItemInvalidate) return;
			activeItemInvalidate = false;
			UpdateHotTrackItem(this.PointToClient(MousePosition));
		}
		void UpdateHotTrackItem(Point p) {
			SetActiveLabelInfo(p);
			if(HotTrackLabelInfoViewInfo == null) {
				Cursor = Cursors.Arrow;
				return;
			}
			HotTrackLabelInfoViewInfo.OnMouseMove(p.X, p.Y);
			Cursor = HotTrackItem != null ? Cursors.Hand : Cursors.Arrow;
		}
		void AddElementCollection() {
			FocusInfo.ChangeElement(ElementType.CollectionAction);
		}
		void SwapPropertyValueActionMenu() {
			FocusInfo.ChangeElement(null);
		}
		protected virtual OperandProperty CreateDefaultProperty(IClauseNode node) {
			return ((ClauseNode)node).CreateDefaultProperty();
		}
		protected virtual OperandParameter CreateDefaultParameter(IClauseNode node) {
			return ((ClauseNode)node).CreateDefaultParameter();
		}
		#region BaseStyleControl Members
		protected override BaseControlPainter CreatePainter() {
			return new FilterControlPainter(this);
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new FilterControlViewInfo(this);
		}
		#endregion
		#region Events
		static readonly object itemClick = new object();
		static readonly object itemDoubleClick = new object();
		public event LabelInfoItemClickEvent ItemClick {
			add { Events.AddHandler(itemClick, value); }
			remove { Events.RemoveHandler(itemClick, value); }
		}
		public event LabelInfoItemClickEvent ItemDoubleClick {
			add { Events.AddHandler(itemDoubleClick, value); }
			remove { Events.RemoveHandler(itemDoubleClick, value); }
		}
		protected virtual void ShowElementMenu(ElementType type) {
			bool allowAction = false;
			if(downActiveItem != null && HotTrackItem != null)
				allowAction = downActiveItem.InfoText == HotTrackItem.InfoText || forceAction ||  criteriaChangedBeforeClick;
			criteriaChangedBeforeClick = false;
			forceAction = false;
			switch(type) {
				case ElementType.Property:
					ShowPropertyMenu();
					break;
				case ElementType.AdditionalOperandProperty:
					ShowAdditionalOperandPropertyMenu();
					break;
				case ElementType.AdditionalOperandParameter:
					ShowAdditionalOperandParameterMenu();
					break;
				case ElementType.Group:
					ShowGroupMenu();
					break;
				case ElementType.Operation:
					ShowClauseMenu();
					break;
				case ElementType.AggregateOperation:
					ShowAggregateMenu();
					break;
				case ElementType.AggregateProperty:
					ShowAggregatedPropertyMenu();
					break;
				case ElementType.ItemCollection:
				case ElementType.Value:
					CreateActiveEditor();
					break;
				case ElementType.NodeRemove:
					if(allowAction)
						OnRemoveNode(this, EventArgs.Empty);
					break;
				case ElementType.NodeAction:
					ShowNodeActionMenu();
					break;
				case ElementType.FieldAction:
					if(allowAction)
						FocusInfo.ChangeElement(null);
					break;
				case ElementType.CollectionAction:
					if(allowAction)
						AddElementCollection();
					break;
				case ElementType.NodeAdd:
					if(allowAction)
						((GroupNode)FocusInfo.Node).AddElement();
					break;
			}
		}
		protected virtual void RaiseItemClick(LabelInfoItemClickEventArgs e) {
			LabelInfoItemClickEvent handler = (LabelInfoItemClickEvent)Events[itemClick];
			if(handler != null) handler(this, e);
		}
		protected virtual void OnItemClick(LabelInfoItemClickEventArgs e) {
			RaiseItemClick(e);
			NodeEditableElement element = e.InfoText.Tag as NodeEditableElement;
			OnElementClick(element);
		}
		internal void OnElementClick(NodeEditableElement element) {
			if(element == null) return;
			if(element.ElementType == ElementType.FieldAction || element.ElementType == ElementType.CollectionAction)
				suspendEditorCreate = true;
			FocusInfo = element.CreateFocusInfo();
			suspendEditorCreate = false;
			ShowElementMenu(element.ElementType);
		}
		protected virtual void OnItemDoubleClick(LabelInfoItemClickEventArgs e) {
			LabelInfoItemClickEvent handler = (LabelInfoItemClickEvent)this.Events[itemDoubleClick];
			if(handler != null) handler(this, e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			ForceItemClick.MouseMove(e);
			UpdateHotTrackItem(new Point(e.X, e.Y));
		}
		FilterLabelInfoTextViewInfo downActiveItem = null;
		internal void SetMouseDown(MouseEventArgs e) {
			OnMouseDown(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			downActiveItem = HotTrackItem;
			if(HotTrackLabelInfoViewInfo == null) return;
			if(e.Button != MouseButtons.Left) return;
			HotTrackLabelInfoViewInfo.OnMouseDown(e.X, e.Y);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(HotTrackLabelInfoViewInfo == null || e.Button != MouseButtons.Left) return;
			HotTrackLabelInfoViewInfo.OnMouseUp(e.X, e.Y);
			if(HotTrackItem != null) {
				OnItemClick(new LabelInfoItemClickEventArgs(HotTrackItem.InfoText));
			}
		}
		protected override void OnMouseClick(MouseEventArgs e) {
			Update();
			UpdateHotTrackItem(e.Location);
			ForceItemClick.Type = ElementType.None;
			base.OnMouseClick(e);
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			if(HotTrackLabelInfoViewInfo == null) return;
			if(e.Button != MouseButtons.Left) return;
			if(HotTrackItem != null)
				OnItemDoubleClick(new LabelInfoItemClickEventArgs(HotTrackItem.InfoText));
		}
		protected override void OnMouseLeave(EventArgs e) {
			Model.ClearActiveItem();
			Cursor = Cursors.Arrow;
			base.OnMouseLeave(e);
		}
		protected override void OnResize(EventArgs e) {
			invalidateScrollPosition = true;
			base.OnResize(e);
			this.Invalidate();
			if(ViewInfo.RightToLeft) RefreshTreeAfterNodeChange();
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			LayoutChanged();
		}
		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
			LayoutChanged();
		}
		protected override bool IsInputKey(Keys keyData) {
			switch(keyData) {
				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
				case Keys.Enter:
				case Keys.Space:
					return true;
				case Keys.Escape:
					if(ActiveEditor != null)
							return true;
					break;
			}
			return base.IsInputKey(keyData);
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(IsInputKey(keyData))
				return false;
			else
				return base.ProcessDialogKey(keyData);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			ProcessKeys(e);
			if(e.Handled)
				return;
			if(ActiveEditor != null)
				return;
			if(e.Modifiers != Keys.None && e.KeyCode != Keys.ShiftKey)
				return;
			if(IsInputKey(e.KeyCode) || e.KeyCode == Keys.Escape)
				return;
			if (FocusedItem != null) {
				CreateActiveEditor();
			}
			if(ActiveEditor != null)
				ActiveEditor.SendKey(e);
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			if(e.Handled)
				return;
			switch(e.KeyChar) {
				case ' ':
				case '\r':
					if(ActiveEditor == null) {
						ShowElementMenu(FocusedItemType);
					}
					e.Handled = true;
					break;
				case '\x1b':
					e.Handled = true;
					break;
				default:
					if(ActiveEditor != null) {
						ActiveEditor.SendKey(lastKeyMessage, e);
					}
					break;
			}
		}
		internal object lastKeyMessage = null;
		protected override bool ProcessKeyMessage(ref Message m) {
			this.lastKeyMessage = DevExpress.XtraEditors.Senders.BaseSender.SaveMessage(ref m, lastKeyMessage);
			return base.ProcessKeyMessage(ref m);
		}
		protected override bool ProcessKeyPreview(ref Message m) {
			if(m.Msg == 0x0100 || m.Msg == 0x0104) {
				KeyEventArgs keys = new KeyEventArgs((Keys)((int)Control.ModifierKeys | (m.WParam.ToInt32() & (int)Keys.KeyCode)));
				if(ProcessKeys(keys))
					return true;
			}
			return base.ProcessKeyPreview(ref m);
		}
		protected virtual bool ProcessKeys(KeyEventArgs e) {
			if(e.Handled)
				return false;
			if(ActiveEditor != null) {
				if(ActiveEditor.IsNeededKey(e))
					return false;
			}
			switch(e.KeyData) {
				case Keys.Up:
					return DoKeyboardRefocus(e, this.FocusInfo.OnUp());
				case Keys.Down:
					return DoKeyboardRefocus(e, this.FocusInfo.OnDown());
				case Keys.Left:
					return DoKeyboardRefocus(e, IsRightToLeft ? this.FocusInfo.OnRight() : this.FocusInfo.OnLeft());
				case Keys.Right:
					return DoKeyboardRefocus(e, IsRightToLeft ? this.FocusInfo.OnLeft() : this.FocusInfo.OnRight());
				case Keys.Enter:
					if(ActiveEditor != null) {
						DoEditorValidate();
						return true;
					} else {
						return false;
					}
				case Keys.Apps:
				case Keys.Alt | Keys.Down:
					if(ActiveEditor == null) {
						e.Handled = true;
						ShowElementMenu(FocusedItemType);
						return true;
					}
					return false;
				case Keys.Escape:
					if(ActiveEditor != null) {
						e.Handled = true;
						DisposeActiveEditor();
						return true;
					}
					return false;
				case Keys.Insert:
				case Keys.Add:
				return DoAddElement();
				case Keys.Delete:
				case Keys.Subtract:
					return DoDeleteElement();
				case Keys.Control | Keys.Insert:
				case Keys.Control | Keys.C:
					return DoCopyElement();
				case Keys.Control | Keys.V:
				case Keys.Shift | Keys.Insert:
					return DoPasteElement();
				case Keys.Control | Keys.X:
				case Keys.Shift | Keys.Delete:
					return DoCutElement();
				case Keys.Control | Keys.Add:
					return DoAddGroup();
				case Keys.Control | Keys.Q:
					return DoSwapPropertyValue();
				default:
					return false;
			}
		}
		bool DoCopyElement() {
			Node node = FocusInfo.Node;
			CriteriaOperator op = Model.ToCriteria(node);
			if(ReferenceEquals(op, null))
				return false;
			try {
				Clipboard.SetDataObject(Model.CriteriaSerialize(op));
				return true;
			} catch {
				return false;
			}
		}
		bool DoPasteElement() {
			bool res = Model.DoPasteElement(DevExpress.XtraEditors.Mask.MaskBox.GetClipboardText());
			if (res) {
				RefreshTreeAfterNodeChange();
			}
			return res;
		}
		bool DoCutElement() {
			return DoCopyElement() && DoDeleteElement();
		}
		bool CanCopyElement() {
			return !ReferenceEquals(Model.ToCriteria(FocusInfo.Node), null);
		}
		bool CanPasteElement() {
			try {
				string clp = DevExpress.XtraEditors.Mask.MaskBox.GetClipboardText(); 
				return !ReferenceEquals(Model.CriteriaParse(clp), null);
			} catch {
				return false;
			}
		}
		bool DoAddElement() {
			return Model.DoAddElement();
		}
		bool DoDeleteElement() {
			OnRemoveNode(this, EventArgs.Empty);
			return true;
		}
		bool DoAddGroup() {
			ClauseNode node = FocusInfo.Node as ClauseNode;
			if(node == null)
				AddGroup((GroupNode)FocusInfo.Node);
			else
				AddGroup((GroupNode)((INode)node).ParentNode);
			return true;
		}
		bool DoSwapPropertyValue() {
			return Model.DoSwapPropertyValue();
		}
		bool DoKeyboardRefocus(KeyEventArgs e, FilterControlFocusInfo newFocusInfo) {
			e.Handled = true;
			if(ActiveEditor != null)
				ActiveEditor.DoValidate();
			this.FocusInfo = newFocusInfo;
			return true;
		}
		#endregion
		#region Property Actions
		internal void OnPropertyClick(object sender, EventArgs e) {
			MenuItemPropertyClick(FilterViewInfo.GetFieldNameByColumnMenuItem(sender as DXMenuItem));
		}
		protected internal void OnParameterClick(object sender, EventArgs e) {
			MenuItemParameterClick((sender as DXMenuItem).Caption);
		}
		internal void OnAddNewParameterClick(object sender, EventArgs e) {
			CreateActiveEditor();
		}
		internal void OnDateTimeConstantClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			if(item == null)
				return;
			MenuItemAdditionalOperandPropertyClick(((ITreeSelectableItemFunctionName)item.Tag).FunctionName);
		}
		protected virtual void MenuItemAdditionalOperandPropertyClick(string functionName) {
			FocusInfo.ChangeElement(functionName);
		}
		protected virtual void MenuItemPropertyClick(string propertyName) {
			FocusInfo.ChangeElement(propertyName);
		}
		protected virtual void MenuItemParameterClick(string parameterName){
			FocusInfo.ChangeElement(parameterName);
		}
		internal void OnGroupClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			if(item == null) return;
			FocusInfo.ChangeElement(item.Tag);
		}
		internal void OnClauseClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			if(item == null)
				return;
			ChangeClauseTypeFromUI((ClauseType)item.Tag);
		}
		internal void OnAggregateClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			if(item == null)
				return;
			ChangeAggregateTypeFromUI((Aggregate)item.Tag);
		}
		void ChangeClauseTypeFromUI(ClauseType type) {
			FocusInfo.ChangeElement(type);
		}
		void ChangeAggregateTypeFromUI(Aggregate type) {
			FocusInfo.ChangeElement(type);
		}
		internal void OnRemoveNode(object sender, EventArgs e) {
			FocusInfo.Node.DeleteElement();
		}
		internal void OnAddCondition(object sender, EventArgs e) {
			FocusInfo.Node.AddElement();
		}
		internal void OnAddGroup(object sender, EventArgs e) {
			AddGroup((GroupNode)FocusInfo.Node);
		}
		void AddGroup(GroupNode currentNode) {
			Model.AddGroup(currentNode);
		}
		internal void OnClearAll(object sender, EventArgs e) {
			RootNode.DeleteElement();
		}
		protected internal bool IsHaveToShowPopupEditor() {
			UpdateUseMenuForOperandsAndOperators();
			if (UseMenuForOperandsAndOperators) return false;
			return FocusedItemType == ElementType.Property || FocusedItemType == ElementType.AdditionalOperandProperty
				|| FocusedItemType == ElementType.Operation || FocusedItemType == ElementType.AggregateOperation || FocusedItemType == ElementType.AggregateProperty;
		}
		protected void ShowMenu(DXPopupMenu menu, FilterControlMenuType type) {
			if (FocusedItem == null) return;
			Point p = new Point(FocusedItem.ItemBounds.X + (IsRightToLeft ? FocusedItem.ItemBounds.Width : 0), FocusedItem.Bottom);
#pragma warning disable 612 // Obsolete
#pragma warning disable 618 // Obsolete
			ShowFilterControlMenuEventArgs e = new ShowFilterControlMenuEventArgs(FocusInfo.Node, FocusedItemType, type, menu, p);
			RaisePopupMenuShowing(e);
			RaiseShowFilterControlMenu(e);
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
			if (e.Cancel) return;
			if (IsHaveToShowPopupEditor()) {
				ShowActiveEditorByMenu(menu);
			} else {
				MenuManagerHelper.ShowMenu(menu, this.LookAndFeel, menuManager, this, e.Point);
			}
		}
		void ShowMenu(FilterControlMenuType type, Type columnFunctionType) {
			var menu = FilterViewInfo.GetMenu(type, columnFunctionType);
			ShowMenu(menu, type);
		}
		void ShowAdditionalOperandPropertyMenu() {
			FilterColumn column = FocusInfoFilterColumn;
			if(column != null && column.ColumnType != null) {
				ShowMenu(FilterControlMenuType.ColumnFunctions, column.ColumnType);
			}
			else ShowPropertyMenu();
		}
		protected virtual void ShowAdditionalOperandParameterMenu() {
			FilterColumn column = FocusInfoFilterColumn;
			DXPopupMenu menu = new DXPopupMenu();
			menu.IsRightToLeft = ViewInfo.RightToLeft;
			if(column != null) {
				foreach(IFilterParameter parameter in Model.GetParametersByType(column.ColumnType)) {
					if(CheckAdditionalOperandParameterAllowed(FocusInfo, parameter))
						menu.Items.Add(new DXMenuItem(parameter.Name, new EventHandler(OnParameterClick)));
				}
			}
			if (menu.Items.Count == 0 && Model.CanAddParameters) {
				CreateActiveEditor();
				return;
			}
			if(Model.CanAddParameters) {
				menu.Items.Add(new DXMenuItem(Localizer.Active.GetLocalizedString(StringId.FilterMenuAddNewParameter), new EventHandler(OnAddNewParameterClick)));
			}
			ShowMenu(menu, FilterControlMenuType.AdditionalOperandParameter);
		}
		protected virtual bool CheckAdditionalOperandParameterAllowed(FilterControlFocusInfo FocusInfo, IFilterParameter parameter) {
			return true;
		}
		void ShowPropertyMenu() {
			ShowMenu(FilterControlMenuType.Column, null);
		}
		void ShowAggregatedPropertyMenu() {
			ShowMenu(FilterControlMenuType.AggregateColumn, null);
		}
		void ShowGroupMenu() {
			ShowMenu(FilterControlMenuType.Group, null);
		}
		void ShowNodeActionMenu() {
			ShowMenu(FilterControlMenuType.NodeAction, null);
		}
		void ShowClauseMenu() {
			ShowMenu(FilterControlMenuType.Clause, null);
		}
		void ShowAggregateMenu() {
			ShowMenu(FilterControlMenuType.Aggregate, null);
		}
		#endregion
		#region ToolTips
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			FilterControlLabelInfo labelInfo = Model.GetLabelInfoByCoordinates(point.X, point.Y);
			if(labelInfo == null || labelInfo.ViewInfo == null) return null;
			ToolTipInfo info = GetToolTipInfo(labelInfo.ViewInfo.ActiveItem as FilterLabelInfoTextViewInfo);
			if(info == null) return null;
			return new ToolTipControlInfo(labelInfo.ViewInfo.ActiveItem, info.Text, info.Title, false, ToolTipIconType.None);
		}
		ToolTipInfo GetToolTipInfo(FilterLabelInfoTextViewInfo item) {
			if(item == null) return null;
			switch(item.FilterViewInfo.ActiveItemType) {
				case ElementType.NodeAdd:
					return new ToolTipInfo(Localizer.Active.GetLocalizedString(StringId.FilterToolTipNodeAdd), Localizer.Active.GetLocalizedString(StringId.FilterToolTipKeysAdd));
				case ElementType.NodeRemove:
					return new ToolTipInfo(Localizer.Active.GetLocalizedString(StringId.FilterToolTipNodeRemove), Localizer.Active.GetLocalizedString(StringId.FilterToolTipKeysRemove));
				case ElementType.NodeAction:
					return new ToolTipInfo(Localizer.Active.GetLocalizedString(StringId.FilterToolTipNodeAction), "");
				case ElementType.FieldAction:
					return new ToolTipInfo(Localizer.Active.GetLocalizedString(StringId.FilterToolTipValueType), "");
				case ElementType.CollectionAction:
					return new ToolTipInfo(Localizer.Active.GetLocalizedString(StringId.FilterToolTipElementAdd), Localizer.Active.GetLocalizedString(StringId.FilterToolTipKeysAdd));
			}
			return null;
		}
		class ToolTipInfo {
			string text, title;
			public ToolTipInfo(string text, string title) {
				this.text = text;
				this.title = title;
			}
			public string Text { get { return text; } }
			public string Title { get { return title; } }
		}
		#endregion
		#region ScrollBars
		bool postponedHorzScroll = false;
		bool suspendDisposeEditor = false;
		bool createEditor = false;
		bool invalidateScrollPosition = false;
		internal void InvalidateHorzScroll() {
			if(postponedHorzScroll) MakeFocusedNodeVisible();
			postponedHorzScroll = false;
			if(ActiveEditor != null) MakeFocusedActiveEditorVisible();
		}
		void MakeFocusedActiveEditorVisible() {
			if(!createEditor || suspendDisposeEditor) return;
			suspendDisposeEditor = true;
			int dh = Model[RootNode].Left;
			int dv = Model[RootNode].Top;
			if(HScrollJustification()) {
				ActiveEditor.Left = FocusedItem.ItemBounds.X + Model[RootNode].Left - dh;
			}
			else if(ActiveEditor.Left != FocusedItem.ItemBounds.X) 
				ActiveEditor.Left = FocusedItem.ItemBounds.X;
			if(VScrollJustification()) { 
				int top = FocusedItem.ItemBounds.Y + Model[RootNode].Top - dv;
				top -= (FilterViewInfo.RowHeight - FocusedItem.ItemBounds.Height) / 2;
				ActiveEditor.Top = top;
			} else
				UpdateEditorTop();
			suspendDisposeEditor = false;
			createEditor = false;
		}
		void UpdateEditorTop() {
			if(FocusedItem == null) return;
			Rectangle rect = LabelInfoHelper.GetEditorBoundsByElement(FocusedItem);
			if(ActiveEditor.Top != rect.Top)
				ActiveEditor.Top = rect.Top;
		}
		void MakeFocusedNodeVisible() {
			if(vScrollBar == null) return;
			VScrollJustification();
			HScrollJustification();
		}
		bool VScrollJustification() {
			if(!vScrollBar.Visible) return false;
			if(Model[FocusInfo.Node] == null) return false;
			int line = (Model[FocusInfo.Node].Top - Model[this.RootNode].Top) / VScrollStep;
			if(Model[FocusInfo.Node].Top + VScrollStep >= VScrollHeight) {
				this.vScrollBar.Value = line - VisibleRowCount + 1;
				return true;
			}
			if(Model[FocusInfo.Node].Top < 0) {
				this.vScrollBar.Value = line;
				return true;
			}
			return false;
		}
		bool HScrollJustification() {
			if(!hScrollBar.Visible) return false;
			if(FocusedItem == null) {
				postponedHorzScroll = true;
				return false;
			}
			int columnFirst = (FocusedItem.TextElement.X - Model[RootNode].Left) / HScrollStep;
			int columnLast = ((FocusedItem.TextElement.X + FocusedItem.TextElement.Width) - Model[RootNode].Left) / HScrollStep;
			if(FocusedItem.TextElement.X + FocusedItem.TextElement.Width >= HScrollWidth) {
				this.hScrollBar.Value = columnLast - VisibleColumnCount + 1;
				return true;
			}
			if(FocusedItem.TextElement.X < 0) {
				this.hScrollBar.Value = columnFirst;
				return true;
			}
			return false;
		}
		internal Rectangle ScrollSquareRectangle {
			get {
				if(VScrollVisible && HScrollVisible)
					return new Rectangle(
						this.ViewInfo.ClientRect.X + (IsRightToLeft ? 0 : this.ViewInfo.ClientRect.Width - SystemInformation.VerticalScrollBarWidth),
						this.ViewInfo.ClientRect.Y + this.ViewInfo.ClientRect.Height - SystemInformation.HorizontalScrollBarHeight,
						SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
				return Rectangle.Empty;
			}
		}
		bool isCalculatingScrollbasVisibility;
		bool IsCalculatingScrollbasVisibility { get { return isCalculatingScrollbasVisibility; } set { isCalculatingScrollbasVisibility = value; } }
		bool VScrollVisible {
			get { return !IsCalculatingScrollbasVisibility && this.vScrollBar.Visible; }
			set {
				if(VScrollVisible == value) return;
				this.vScrollBar.Visible = value;
				if(!value) this.vScrollBar.Value = 0;
				InvalidateScrollBarPosition(true);
				if(value && ActiveEditor == null)
					MakeFocusedNodeVisible();
			}
		}
		bool HScrollVisible {
			get { return !IsCalculatingScrollbasVisibility && this.hScrollBar.Visible; }
			set {
				if(HScrollVisible == value) return;
				this.hScrollBar.Visible = value;
				if(!value) this.hScrollBar.Value = 0;
				InvalidateScrollBarPosition(true);
				if(value && ActiveEditor == null)
					MakeFocusedNodeVisible();
			}
		}
		int VScrollHeight { get { return this.ViewInfo.ClientRect.Height - (HScrollVisible ? SystemInformation.HorizontalScrollBarHeight : 0); } }
		int HScrollWidth { get { return this.ViewInfo.ClientRect.Width - (VScrollVisible ? SystemInformation.VerticalScrollBarWidth : 0); } }
		int VScrollStep { get { return FilterViewInfo.NodeHeight; } }
		int HScrollStep { get { return 10; } }
		int VisibleRowCount { get { return (VScrollHeight - FilterControlViewInfo.TopIndent / 2) / VScrollStep; } }
		int RowCount { get { return (Model[RootNode].Height + FilterControlViewInfo.TopIndent) / VScrollStep; } }
		int VisibleColumnCount { get { return (HScrollWidth - FilterControlViewInfo.LeftIndent / 2) / HScrollStep; } }
		int ColumnCount { get { return (Model[RootNode].NodeWidth + FilterControlViewInfo.LeftIndent) / HScrollStep; } }
		int GetMaxValue(ScrollBarBase sb) {
			return sb.Maximum - sb.LargeChange + 1;
		}
		void CorrectScrollValue(int vScrollValue, int hScrollValue) {
			if(this.vScrollBar.Value > vScrollValue) this.vScrollBar.Value = vScrollValue;
			if(this.hScrollBar.Value > hScrollValue) this.hScrollBar.Value = hScrollValue;
		}
		internal void InvalidateScrollBarPosition() {
			InvalidateScrollBarPosition(false);
		}
		bool isInvalidatingScrollBarPosition = false;
		int GetScrollBarVisibility() { return (VScrollVisible ? 1 : 0) + (HScrollVisible ? 2 : 0); }
		void InvalidateScrollBarPosition(bool immediately) {
			if (!invalidateScrollPosition && !immediately || isInvalidatingScrollBarPosition) return;
			this.isInvalidatingScrollBarPosition = true;
			IsCalculatingScrollbasVisibility = true;
			bool isVertcalScrollBarVisible = RowCount > VisibleRowCount;
			bool IsHorizontalScrollBarVisible = ColumnCount > VisibleColumnCount;
			IsCalculatingScrollbasVisibility = false;
			int scrollBarVisibility = GetScrollBarVisibility();
			VScrollVisible = isVertcalScrollBarVisible;
			HScrollVisible = IsHorizontalScrollBarVisible;
			if (HScrollVisible) {
				VScrollVisible = RowCount > VisibleRowCount;
			}
			this.vScrollBar.Bounds = new Rectangle(this.ViewInfo.ClientRect.X + (IsRightToLeft ? 0 : this.ViewInfo.ClientRect.Width - SystemInformation.VerticalScrollBarWidth),
				this.ViewInfo.ClientRect.Y, SystemInformation.VerticalScrollBarWidth, VScrollHeight);
			this.hScrollBar.Bounds = new Rectangle(this.ViewInfo.ClientRect.X + (IsRightToLeft ? (VScrollVisible ? SystemInformation.VerticalScrollBarWidth : 0) : 0),
				this.ViewInfo.ClientRect.Y + this.ViewInfo.ClientRect.Height - SystemInformation.HorizontalScrollBarHeight,
				HScrollWidth, SystemInformation.HorizontalScrollBarHeight);
			this.vScrollBar.Maximum = RowCount;
			this.vScrollBar.LargeChange = VisibleRowCount + 1;
			this.hScrollBar.Maximum = ColumnCount;
			this.hScrollBar.LargeChange = VisibleColumnCount + 1;
			CorrectScrollValue(GetMaxValue(this.vScrollBar), GetMaxValue(this.hScrollBar));
			invalidateScrollPosition = false;
			this.isInvalidatingScrollBarPosition = false;
			if(scrollBarVisibility != GetScrollBarVisibility()) {
				DoScrollBarValueChanged();
			}
		}
		void InitScrollBars() {
			this.vScrollBar = new DevExpress.XtraEditors.VScrollBar();
			this.vScrollBar.Parent = this;
			this.vScrollBar.Minimum = 0;
			this.hScrollBar = new DevExpress.XtraEditors.HScrollBar();
			this.hScrollBar.Parent = this;
			this.hScrollBar.Minimum = 0;
			UpdateScrollLookAndFeel();
			this.vScrollBar.ValueChanged += new EventHandler(OnScrollBarValueChanged);
			this.hScrollBar.ValueChanged += new EventHandler(OnScrollBarValueChanged);
			invalidateScrollPosition = true;
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			FilterViewInfo.UpdateAppearanceColors();
		}
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			base.OnLookAndFeelChanged(sender, e);
			UpdateScrollLookAndFeel();
			FilterViewInfo.UpdateAppearanceColors();
		}
		void UpdateScrollLookAndFeel() {
			this.hScrollBar.LookAndFeel.Assign(this.LookAndFeel);
			this.vScrollBar.LookAndFeel.Assign(this.LookAndFeel);
		}
		void OnScrollBarValueChanged(object sender, EventArgs e) {
			DoScrollBarValueChanged();
		}
		void DoScrollBarValueChanged() {
			if(this.isInvalidatingScrollBarPosition) return;
			if(!suspendDisposeEditor & !this.createEditor) {
				DoEditorValidate();
			}
			int topIndent = FilterControlViewInfo.TopIndent;
			if(this.vScrollBar.Visible) topIndent -= this.vScrollBar.Value * VScrollStep;
			int leftIndent = FilterControlViewInfo.LeftIndent;
			if(this.hScrollBar.Visible) leftIndent -= this.hScrollBar.Value * HScrollStep; 
			Model[RootNode].Top = topIndent;
			Model[RootNode].Left = leftIndent;
			Refresh(true, false);
		}
		#endregion
		protected override void OnValidating(CancelEventArgs e) {
			base.OnValidating(e);
		}
		public void OnModelChanged(FilterChangedEventArgs info) {
			RefreshTreeAfterNodeChange();
			criteriaChangedBeforeClick = info.Action == FilterChangedAction.FilterStringChanged || info.Action == FilterChangedAction.RebuildWholeTree;
			if(info.CurrentNode != null) {
				RaiseFilterChanged(new FilterChangedEventArgs(info.Action, info.CurrentNode));
				return;
			}
		}
		protected override void OnRightToLeftChanged() {
			base.OnRightToLeftChanged();
			RefreshTreeAfterNodeChange();
		}
	}
}
namespace DevExpress.XtraEditors.Filtering {
	public enum FilterControlMenuType { Column, Group, Clause, NodeAction, ColumnFunctions, AdditionalOperandParameter, Aggregate, AggregateColumn };
	public delegate void ShowValueEditorEventHandler(object sender, ShowValueEditorEventArgs e);
	public delegate void CreateValueEditorEventHandler(object sender, CreateValueEditorEventArgs e);
	public delegate void DisposeValueEditorEventHandler(object sender, ValueEditorEventArgs e);
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
	[Obsolete("You should use the 'PopupMenuShowingEventHandler' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void ShowFilterControlMenuEventHandler(object sender, ShowFilterControlMenuEventArgs e);
	public class ValueEditorEventArgs : EventArgs {
		BaseEdit editor;
		public ValueEditorEventArgs(BaseEdit editor) {
			this.editor = editor;
		}
		public virtual BaseEdit Editor {
			get { return editor; }
		}
	}
	public class CreateValueEditorEventArgs : EventArgs {
		RepositoryItem item;
		object value;
		public CreateValueEditorEventArgs(RepositoryItem repositoryItem, object editValue) {
			this.item = repositoryItem;
			this.value = editValue;
		}
		public RepositoryItem RepositoryItem { get { return item; }}
		public object EditValue {
			get { return value; }
			set { this.value = value; }
		}
	}
	public class ShowValueEditorEventArgs : ValueEditorEventArgs {
		ClauseNode node;
		OperandValue value;
		int elementIndex;
		RepositoryItem item = null;
		public ShowValueEditorEventArgs(BaseEdit editor, ClauseNode node, OperandValue value, int elementIndex) : base(editor) {
			this.node = node;
			this.value = value;
			this.elementIndex = elementIndex;
		}
		public ClauseNode CurrentNode {
			get { return node; }
		}
		public OperandValue OperandValue {
			get { return value; }
		}
		public ClauseType Operation {
			get {
				return CurrentNode.Operation;
			}
		}
		public int FocusedElementIndex {
			get { return elementIndex; }
		}
		public RepositoryItem CustomRepositoryItem {
			get { return item; }
			set { item = value; }
		}
	}
	public class PopupMenuShowingEventArgs : CancelEventArgs {
		FilterControlMenuType type;
		DXPopupMenu menu;
		Node node;
		Point p;
		ElementType elementType;
		bool restoreMenu = false;
		public PopupMenuShowingEventArgs(Node node, ElementType elementType, FilterControlMenuType type, DXPopupMenu menu, Point p) {
			this.type = type;
			this.menu = menu;
			this.node = node;
			this.p = p;
			this.elementType = elementType;
		}
		public FilterControlMenuType MenuType {
			get { return type; }
		}
		public DXPopupMenu Menu {
			get { return menu; }
		}
		public Node CurrentNode {
			get { return node; }
		}
		public Point Point {
			get { return p; }
			set { p = value; }
		}
		public ElementType FocusedElementType {
			get { return elementType; }
		}
		public bool RestoreMenu {
			get { return restoreMenu; }
			set { restoreMenu = value; }
		}
	}
	public class CreateCustomRepositoryItemEventArgs : EventArgs {
		public CreateCustomRepositoryItemEventArgs(DataColumnInfoFilterColumn column) {
			this.Column = column;
		}
		public DataColumnInfoFilterColumn Column { get; private set; }
		public RepositoryItem RepositoryItem { get; set; }
	}
	[Obsolete("You should use the 'PopupMenuShowingEventArgs' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ShowFilterControlMenuEventArgs : PopupMenuShowingEventArgs {
		public ShowFilterControlMenuEventArgs(Node node, ElementType elementType, FilterControlMenuType type, DXPopupMenu menu, Point p)
			: base(node, elementType, type, menu, p) {
		}
	}
	internal class ItemCollectionHelper {
		internal static void InitEditor(BaseEdit editor) {
			CheckedComboBoxEdit ccEdit = editor as CheckedComboBoxEdit;
			if(ccEdit == null) return;
			ccEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			ccEdit.Properties.DropDownRows = 10;
			ccEdit.ShowPopup();
		}
		internal static void SetNodeEditValue(BaseEdit editor, object editValue) {
			CheckedComboBoxEdit cceEdit = editor as CheckedComboBoxEdit;
			if(cceEdit == null) return;
			List<object> list = editValue as List<object>;
			if(editValue == null) return;
			if(list.Count < 1) return;
			if(cceEdit is FilterCheckedComboBoxEdit) {
				((FilterCheckedComboBoxEdit)cceEdit).SetValues(list);
			}
			else {
				StringBuilder sb = new StringBuilder();
				string ret = string.Empty;
				sb.Append(string.Format("{0}", list[0]));
				for(int i = 1; i < list.Count; i++)
					sb.Append(cceEdit.Properties.SeparatorChar + string.Format(" {0}", list[i]));
				cceEdit.SetEditValue(ret.ToString());
			}
		}
	}
	public interface IFilteredComponentsProvider {
		ICollection GetFilteredComponents();
	}
	public class FilterControlConverter : ReferenceConverter {
		public FilterControlConverter()
			: base(typeof(IListSource)) {
		}
		protected virtual IContainer GetContainer(ITypeDescriptorContext context) {
			if(context == null) return null;
			if(context.Container != null) return context.Container;
			return null;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			IContainer container = GetContainer(context);
			if(container == null) return null;
			ArrayList array = new ArrayList();
			array.Add(null);
			foreach(IComponent component in container.Components) {
				if(component is IFilteredComponent || component is IFilteredXtraBindingList || component is DataTable || component is IBindingListView) {
					array.Add(component);
				}
				else if(component is IFilteredComponentsProvider) {
					IFilteredComponentsProvider provider = (IFilteredComponentsProvider)component;
					array.AddRange(provider.GetFilteredComponents());
				}
			}
			TypeConverter.StandardValuesCollection collection = base.GetStandardValues(context);
			foreach(object obj in collection) {
				if(obj is IFilteredXtraBindingList || obj is DataTable || obj is IBindingListView) {
					if(!array.Contains(obj))
						array.Add(obj);
				}
			}
			return new StandardValuesCollection(array);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	internal interface ITreeSelectableItemFunctionName {
		string FunctionName { get; }
	}
	public class FilterControlViewInfo : BaseStyleControlViewInfo {
		static Dictionary<Type, List<FunctionInfo>> knownFunctions;
		internal static int TopIndent = 3;
		internal static int LeftIndent = 5;
		int levelIndent = 20;
		int textIndentHeight = 0; 
		int nodeSeparator = 0; 
		int singleLineHeight = 0;
		internal int textIndent = 0;
		public static int EditorDefaultWidth = 100;
		AppearanceObject paintAppeareanceTreeLine;
		Brush lineBrush = null;
		DXPopupMenu menu;
		public Color GroupOperatorColor, FieldNameColor, OperatorColor, ValueColor, EmptyValueColor, DisabledColor;
		static FilterControlViewInfo() {
			knownFunctions = new Dictionary<Type, List<FunctionInfo>>();
			List<FunctionInfo> currentFunctionsInfo = new List<FunctionInfo>();
			currentFunctionsInfo.Add(new FunctionInfo(StringId.FilterCriteriaToStringFunctionLocalDateTimeThisYear, FunctionOperatorType.LocalDateTimeThisYear));
			currentFunctionsInfo.Add(new FunctionInfo(StringId.FilterCriteriaToStringFunctionLocalDateTimeThisMonth, FunctionOperatorType.LocalDateTimeThisMonth));
			currentFunctionsInfo.Add(new FunctionInfo(StringId.FilterCriteriaToStringFunctionLocalDateTimeLastWeek, FunctionOperatorType.LocalDateTimeLastWeek));
			currentFunctionsInfo.Add(new FunctionInfo(StringId.FilterCriteriaToStringFunctionLocalDateTimeThisWeek, FunctionOperatorType.LocalDateTimeThisWeek));
			currentFunctionsInfo.Add(new FunctionInfo(StringId.FilterCriteriaToStringFunctionLocalDateTimeYesterday, FunctionOperatorType.LocalDateTimeYesterday));
			currentFunctionsInfo.Add(new FunctionInfo(StringId.FilterCriteriaToStringFunctionLocalDateTimeToday, FunctionOperatorType.LocalDateTimeToday));
			currentFunctionsInfo.Add(new FunctionInfo(StringId.FilterCriteriaToStringFunctionLocalDateTimeNow, FunctionOperatorType.LocalDateTimeNow));
			currentFunctionsInfo.Add(new FunctionInfo(StringId.FilterCriteriaToStringFunctionLocalDateTimeTomorrow, FunctionOperatorType.LocalDateTimeTomorrow));
			currentFunctionsInfo.Add(new FunctionInfo(StringId.FilterCriteriaToStringFunctionLocalDateTimeDayAfterTomorrow, FunctionOperatorType.LocalDateTimeDayAfterTomorrow));
			currentFunctionsInfo.Add(new FunctionInfo(StringId.FilterCriteriaToStringFunctionLocalDateTimeNextWeek, FunctionOperatorType.LocalDateTimeNextWeek));
			currentFunctionsInfo.Add(new FunctionInfo(StringId.FilterCriteriaToStringFunctionLocalDateTimeTwoWeeksAway, FunctionOperatorType.LocalDateTimeTwoWeeksAway));
			currentFunctionsInfo.Add(new FunctionInfo(StringId.FilterCriteriaToStringFunctionLocalDateTimeNextMonth, FunctionOperatorType.LocalDateTimeNextMonth));
			currentFunctionsInfo.Add(new FunctionInfo(StringId.FilterCriteriaToStringFunctionLocalDateTimeNextYear, FunctionOperatorType.LocalDateTimeNextYear));
			knownFunctions.Add(typeof(DateTime), currentFunctionsInfo);
		}
		public FilterControlViewInfo(FilterControl owner)
			: base(owner) {
			this.paintAppeareanceTreeLine = new AppearanceObject();
		}
		protected WinFilterTreeNodeModel Model { get { return OwnerControl.Model; } }
		public void UpdateAppearanceColors() {
			if(OwnerControl == null) return;
			GroupOperatorColor = OwnerControl.AppearanceGroupOperatorColor;
			OperatorColor = OwnerControl.AppearanceOperatorColor;
			FieldNameColor = OwnerControl.AppearanceFieldNameColor;
			ValueColor = OwnerControl.AppearanceValueColor;
			EmptyValueColor = OwnerControl.AppearanceEmptyValueColor;
			DisabledColor = OwnerControl.AppearanceDisabledColor;
			if(LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin) {
				Skin skin = EditorsSkins.GetSkin(LookAndFeel);
				GroupOperatorColor = GetColor(GroupOperatorColor, skin, EditorsSkins.SkinFilterControlGroupOperatorTextColor);
				FieldNameColor = GetColor(FieldNameColor, skin, EditorsSkins.SkinFilterControlFieldNameTextColor);
				OperatorColor = GetColor(OperatorColor, skin, EditorsSkins.SkinFilterControlOperatorTextColor);
				ValueColor = GetColor(ValueColor, skin, EditorsSkins.SkinFilterControlValueTextColor);
				EmptyValueColor = GetColor(EmptyValueColor, skin, EditorsSkins.SkinFilterControlEmptyValueTextColor);
			}
			SetDefaultColorsIfEmpty();
			if(Model != null) {
				Model.RebuildElements();
			}
		}
		internal void SetDefaultColorsIfEmpty() {
			if (GroupOperatorColor.IsEmpty) GroupOperatorColor = FilterControl.DefaultColorValues[0];
			if (FieldNameColor.IsEmpty) FieldNameColor = FilterControl.DefaultColorValues[1];
			if (OperatorColor.IsEmpty) OperatorColor = FilterControl.DefaultColorValues[2];
			if (ValueColor.IsEmpty) ValueColor = FilterControl.DefaultColorValues[3];
			if (EmptyValueColor.IsEmpty) EmptyValueColor = FilterControl.DefaultColorValues[4];
			if(DisabledColor.IsEmpty) DisabledColor = FilterControl.DefaultColorValues[5];
		}
		Color GetColor(Color color, Skin skin, string objectName) {
			if(!color.IsEmpty) return color;
			return skin.Colors.GetColor(objectName);
		}
		public new FilterControl OwnerControl {
			get { return base.OwnerControl as FilterControl; }
		}
		public int NodeHeight { get { return RowHeight + nodeSeparator; } }
		internal int RowHeight { get { return SingleLineHeight + textIndent * 2; } }
		public int SingleLineHeight {
			get {
				if(singleLineHeight == 0) {
					int editorMinHeight = 0;
					GraphicsInfo.Default.AddGraphics(null);
					try {
						using(RepositoryItemButtonEdit be = new RepositoryItemButtonEdit()) {
							BaseEditViewInfo vi = be.CreateViewInfo();
							vi.PaintAppearance.Assign(PaintAppearance);
							editorMinHeight = vi.CalcMinHeight(GraphicsInfo.Default.Graphics);
						}
					} finally {
						GraphicsInfo.Default.ReleaseGraphics();
					}
					singleLineHeight = PaintAppearance.FontHeight;
					textIndent = (editorMinHeight - singleLineHeight) / 2 + textIndentHeight;
				}
				return singleLineHeight;
			}
		}
		public int LevelIndent {
			get { return levelIndent; }
			set {
				levelIndent = value;
			}
		}
		public int NodeSeparatorHeight {
			get { return nodeSeparator; }
			set {
				nodeSeparator = value;
			}
		}
		public Brush LineBrush {
			get {
				if(lineBrush == null)
					lineBrush = new HatchBrush(HatchStyle.Percent50, PaintAppearanceTreeLine.ForeColor, PaintAppearanceTreeLine.BackColor);
				return lineBrush;
			}
		}
		public override Size CalcBestFit(Graphics g) {
			if (OwnerControl.RootNode == null || Model[OwnerControl.RootNode] == null) return base.CalcBestFit(g);
			CalcViewInfo(g);
			return new Size(Model[OwnerControl.RootNode].NodeWidth + LeftIndent * 2,
				Model[OwnerControl.RootNode].Height + TopIndent * 2);
		}
		protected internal void ClearLineBrush() {
			if(lineBrush == null) return;
			lineBrush.Dispose();
			lineBrush = null;
		}
		public DXPopupMenu GetColumnMenu() {
			DXPopupMenu columnMenu = new DXPopupMenu();
			columnMenu.IsRightToLeft = this.RightToLeft;
			Node node = OwnerControl.FocusInfo.Node;
			List<ITreeSelectableItem> selectableitems = Model.GetTreeItemsByProperties(node.FilterProperties);
			foreach (ITreeSelectableItem column in selectableitems) {
				AddColumnMenuItem(columnMenu.Items, (FilterColumn)column);
			}
			return columnMenu;
		}
		public DXPopupMenu GetAggregateColumnMenu() {
			DXPopupMenu aggregateColumnMenu = new DXPopupMenu();
			aggregateColumnMenu.IsRightToLeft = this.RightToLeft;
			AggregateNode aggregateNode = OwnerControl.FocusInfo.Node as AggregateNode;
			if (aggregateNode != null) {
				foreach (FilterColumn column in aggregateNode.GetAvailableAggregateProperties()) {
					AddColumnMenuItem(aggregateColumnMenu.Items, column);
				}
			}
			return aggregateColumnMenu;
		}
		protected internal string GetFieldNameByColumnMenuItem(DXMenuItem item) {
			if(item == null) return string.Empty;
			return ((FilterColumn)item.Tag).FullName;
		}
		protected virtual void AddColumnMenuItem(DXMenuItemCollection menuItems, ITreeSelectableItem selectableItem) {
			if (selectableItem.AllowSelect) {
				AddColumnMenuItemAddColumnMenuItem(
					menuItems,
					(FilterColumn)selectableItem,
					new DXMenuItem(selectableItem.Text, new EventHandler(OwnerControl.OnPropertyClick), ((FilterColumn)selectableItem).Image));
			}
			if (selectableItem.Children != null && selectableItem.Children.Count > 0) {
				AddColumnMenuItemAddColumnMenuItem(
					menuItems,
					(FilterColumn)selectableItem,
					new DXSubMenuItem(selectableItem.Text + FilterControl.ChildrenSuffix, new EventHandler(OnBeforePopup)));
			}
		}
		protected void AddColumnMenuItemAddColumnMenuItem(DXMenuItemCollection menuItems, FilterColumn column, DXMenuItem item) {
			item.Tag = column;
			item.Appearance.TextOptions.HotkeyPrefix = HKeyPrefix.None;
			item.AppearanceHovered.TextOptions.HotkeyPrefix = HKeyPrefix.None;
			item.AppearanceDisabled.TextOptions.HotkeyPrefix = HKeyPrefix.None;
			menuItems.Add(item);
		}
		void OnBeforePopup(object sender, EventArgs e) {
			DXSubMenuItem subMenu = (DXSubMenuItem)sender;
			if(subMenu.Items.Count > 0) return;
			ITreeSelectableItem selectableItem = (ITreeSelectableItem)subMenu.Tag;
			foreach(FilterColumn child in selectableItem.Children) {
				AddColumnMenuItem(subMenu.Items, child);
			}
		}
		internal class FunctionTreeSelectableItem : ITreeSelectableItem, ITreeSelectableItemFunctionName {
			List<ITreeSelectableItem> chidren;
			ITreeSelectableItem parent;
			string text;
			string functionName;
			public FunctionTreeSelectableItem(ITreeSelectableItem parent, string text, string functionName) {
				this.chidren = new List<ITreeSelectableItem>();
				this.parent = parent;
				this.text = text;
				this.functionName = functionName;
			}
			public string FunctionName { get { return functionName; } }
			public bool AllowSelect { get { return !string.IsNullOrEmpty(FunctionName); } }
			public List<ITreeSelectableItem> Children { get { return chidren; } }
			public ITreeSelectableItem Parent { get { return parent; } }
			public string Text { get { return text; } }
			public void AddChild(ITreeSelectableItem child) {
				this.chidren.Add(child);
			}
		}
		void AddFunctionsToMenu(DXSubMenuItem menu, Type columnType, FunctionTreeSelectableItem parent) {
			foreach(FunctionInfo functionInfo in GetFunctions(columnType)) {
				string itemCaption = Localizer.GetLocalizedString(functionInfo.Caption);
				DXMenuItem item = new DXMenuItem(itemCaption, OwnerControl.OnDateTimeConstantClick);
				FunctionTreeSelectableItem treeItem = new FunctionTreeSelectableItem(parent, itemCaption, functionInfo.Name);
				if(parent != null) {
					parent.AddChild(treeItem);
				}
				item.Tag = treeItem;
				menu.Items.Add(item);
			}
		}
		[Obsolete]
		public DXPopupMenu ColumnAndDateTimeConstantMenu {
			get {
				return GetColumnFunctionsMenu(typeof(DateTime));
			}
		}
		public static void RemoveKnownFunction(string name, Type returnType) {
			if(knownFunctions.ContainsKey(returnType)) {
				foreach(FunctionInfo functionInfo in knownFunctions[returnType]) {
					if(functionInfo.Name == name) {
						knownFunctions[returnType].Remove(functionInfo);
						return;
					}
				}
			}
		}
		public static bool ContainsKnownFunction(string name, Type returnType) {
			if(knownFunctions.ContainsKey(returnType)) {
				 foreach(FunctionInfo functionInfo in knownFunctions[returnType]) {
					if(functionInfo.Name == name) {
						return true;
					}
				}
			}
			return false;
		}
		public static void AddKnownFunction(string caption, string name, Type returnType) {
			if(string.IsNullOrEmpty(caption)) {
				throw new ArgumentException("caption");
			}
			if(string.IsNullOrEmpty(name)) {
				throw new ArgumentException("name");
			}
			if(returnType == null) {
				throw new ArgumentNullException("returnType");
			}
			if(ContainsKnownFunction(name, returnType)) {
				throw new ArgumentException("There is already function '" + name + "' registered", "name");
			}
			if(!knownFunctions.ContainsKey(returnType)) {
				knownFunctions.Add(returnType, new List<FunctionInfo>());
			}
			knownFunctions[returnType].Add(new FunctionInfo(caption, name));
		}
		static bool HasFunctions(Type type) {
			return knownFunctions.ContainsKey(type);
		}
		static ReadOnlyCollection<FunctionInfo> GetFunctions(Type type) {
			if(knownFunctions.ContainsKey(type)) {
				return new ReadOnlyCollection<FunctionInfo>(knownFunctions[type]);
			}
			return new ReadOnlyCollection<FunctionInfo>(new FunctionInfo[0]);
		}
		internal static Type GetFunctionType(string name) {
			FunctionInfo info = GetInfo(name);
			foreach(var pair in knownFunctions) {
				if(pair.Value.Contains(info))
					return pair.Key;
			}
			return typeof(object);
		}
		static FunctionInfo GetInfo(string functionName) {
			foreach(List<FunctionInfo> info in knownFunctions.Values) {
				FunctionInfo customFunctionInfo = info.Find(finfo => finfo.Name == functionName.Substring(0, functionName.IndexOf('(')));
				if(customFunctionInfo != null)
					return customFunctionInfo;
			}
			return null;
		}
		public static string GetLocalizedFunctionName(FunctionOperatorType optype, FunctionOperator value) {
			FunctionInfo info = GetInfo(value.ToString());
			if (info == null) {
				if (value.OperatorType == FunctionOperatorType.Custom)
					return (string)((OperandValue)value.Operands[0]).Value;
				else
				return value.ToString();
			}
			if (optype == FunctionOperatorType.Custom || optype == FunctionOperatorType.CustomNonDeterministic) {
				return info.Caption;
			}
			return Localizer.GetLocalizedString(info.Caption);
		}
		public DXPopupMenu GetMenu(FilterControlMenuType menuType, Type columnFunctionType) {
			if(menu != null) {
				menu.Dispose();
				menu = null;
			}
			switch(menuType) {
				case FilterControlMenuType.AdditionalOperandParameter:
					Debug.Fail("");
					break;
				case FilterControlMenuType.Aggregate:
					menu = GetAggregateMenu();
					break;
				case FilterControlMenuType.AggregateColumn:
					menu = GetAggregateColumnMenu();
					break;
				case FilterControlMenuType.Group:
					menu = GetGroupMenu();
					break;
				case FilterControlMenuType.Clause:
					menu = GetClauseMenu();
					break;
				case FilterControlMenuType.Column:
					menu = GetColumnMenu();
					break;
				case FilterControlMenuType.NodeAction:
					menu = GetNodeActionMenu();
					break;
				case FilterControlMenuType.ColumnFunctions:
					menu = GetColumnFunctionsMenu(columnFunctionType);
					break;
				default:
					Debug.Assert(false);
					break;
			}
			return menu;
		}
		public DXPopupMenu GetColumnFunctionsMenu(Type columnType) {
			if(!OwnerControl.ShowOperandCustomFunctions) return GetColumnMenu();
			if(!HasFunctions(columnType)) {
				return GetColumnMenu();
			}
			if(!OwnerControl.ShowOperandTypeIcon) {
				DXPopupMenu menu = new DXPopupMenu();
				menu.IsRightToLeft = this.RightToLeft;
				AddFunctionsToMenu(menu, columnType, null);
				return menu;
			}
			DXPopupMenu newColumnMenu = GetColumnMenu();
			string subMenuCapton = string.Empty;
			if(columnType == typeof(DateTime)) {
				subMenuCapton = Localizer.Active.GetLocalizedString(StringId.FilterDateTimeConstantMenuCaption);
			} else {
				subMenuCapton = Localizer.Active.GetLocalizedString(StringId.FilterFunctionsMenuCaption);
			}
			DXSubMenuItem functionsSubMenu = new DXSubMenuItem(subMenuCapton);
			newColumnMenu.Items.Add(functionsSubMenu);
			FunctionTreeSelectableItem treeItem = new FunctionTreeSelectableItem(null, subMenuCapton, null);
			functionsSubMenu.Tag = treeItem;
			AddFunctionsToMenu(functionsSubMenu, columnType, treeItem);
			return newColumnMenu;
		}
		public DXPopupMenu GetGroupMenu() {
			DXPopupMenu groupMenu = new DXPopupMenu();
			groupMenu.IsRightToLeft = this.RightToLeft;
			foreach (GroupType type in Enum.GetValues(typeof(GroupType))) {
				string menuTitle = OperationHelper.GetMenuStringByType(type);
				DXMenuItem menuItem = new DXMenuItem(menuTitle, new EventHandler(OwnerControl.OnGroupClick), GetGroupImageByType(type));
				menuItem.Tag = type;
				groupMenu.Items.Add(menuItem);
			}
			if (!OwnerControl.ShowGroupCommandsIcon) {
				var nodeActionItems = GetMenu(FilterControlMenuType.NodeAction, null).Items;
				for (int i = 0; i < nodeActionItems.Count; i++) {
					DXMenuItem item = nodeActionItems[i];
					if (i == 0)
						item.BeginGroup = true;
					groupMenu.Items.Add(item);
				}
			}
			return groupMenu;
		}
		bool IsDateTimeOperatorClause(ClauseType type) {
			return type >= ClauseType.IsBeyondThisYear;
		}
		public DXPopupMenu GetClauseMenu() {
			DXPopupMenu clauseMenu = new DXPopupMenu();
			clauseMenu.IsRightToLeft = this.RightToLeft;
			DXSubMenuItem dateTimeOperators = null;
			ClauseNode node = (ClauseNode)OwnerControl.FocusInfo.Node;
			foreach (ClauseType type in node.GetAvailableOperations()) {
				string operationName = OperationHelper.GetMenuStringByType(type);
				if (IsDateTimeOperatorClause(type) && OwnerControl.ShowDateTimeOperators) {
					if (dateTimeOperators == null) {
						dateTimeOperators = OwnerControl.UseMenuForOperandsAndOperators ? CreateDateTimeOperators(clauseMenu) : clauseMenu;
					}
					DXMenuItem item = new DXMenuItem(operationName, new EventHandler(OwnerControl.OnClauseClick));
					item.Tag = type;
					dateTimeOperators.Items.Add(item);
				} else if (!IsDateTimeOperatorClause(type)) {
					DXMenuItem item = new DXMenuItem(operationName, new EventHandler(OwnerControl.OnClauseClick), GetClauseImageByType(type));
					item.Tag = type;
					clauseMenu.Items.Add(item);
				}
			}
			return clauseMenu;
		}
		DXPopupMenu GetAggregateMenu() {
			DXPopupMenu aggregateMenu = new DXPopupMenu();
			aggregateMenu.IsRightToLeft = this.RightToLeft;
			AggregateNode aggregateNode = OwnerControl.FocusInfo.Node as AggregateNode;
			if (aggregateNode != null) {
				foreach (Aggregate type in aggregateNode.GetAvailableAggregateOperations()) {
					DXMenuItem item = new DXMenuItem(OperationHelper.GetMenuStringByType(type), new EventHandler(OwnerControl.OnAggregateClick), GetAggregateImageByType(type));
					item.Tag = type;
					aggregateMenu.Items.Add(item);
				}
			}
			return aggregateMenu;
		}
		DXSubMenuItem CreateDateTimeOperators(DXPopupMenu menu) {
			DXSubMenuItem ret = new DXSubMenuItem(Localizer.Active.GetLocalizedString(StringId.FilterDateTimeOperatorMenuCaption));
			menu.Items.Add(ret);
			return ret;
		}
		private Image GetAggregateImageByType(Aggregate type) {
			return FilterControl.AggregateImages.Images[(int)type];
		}
		private Image GetClauseImageByType(ClauseType type) {
			if (IsDateTimeOperatorClause(type)) return null;
			return FilterControl.ClauseImages.Images[(int)type];
		}
		private Image GetGroupImageByType(GroupType type) {
			return FilterControl.GroupImages.Images[(int)type];
		}
		public DXPopupMenu GetNodeActionMenu() {
			DXPopupMenu nodeActionMenu = new DXPopupMenu();
			nodeActionMenu.IsRightToLeft = this.RightToLeft;
			DXMenuItem item = new DXMenuItem(Localizer.Active.GetLocalizedString(StringId.FilterMenuConditionAdd), new EventHandler(OwnerControl.OnAddCondition), FilterControl.GroupImages.Images[4]);
			nodeActionMenu.Items.Add(item);
			item = new DXMenuItem(Localizer.Active.GetLocalizedString(StringId.FilterMenuGroupAdd), new EventHandler(OwnerControl.OnAddGroup), FilterControl.GroupImages.Images[5]);
			nodeActionMenu.Items.Add(item);
			if (OwnerControl.RootNode.SubNodes.Count != 0) {
				if (OwnerControl.FocusInfo.Node == OwnerControl.RootNode)
					item = new DXMenuItem(Localizer.Active.GetLocalizedString(StringId.FilterMenuClearAll), new EventHandler(OwnerControl.OnClearAll), FilterControl.GroupImages.Images[6]);
				else
					item = new DXMenuItem(Localizer.Active.GetLocalizedString(StringId.FilterMenuRowRemove), new EventHandler(OwnerControl.OnRemoveNode), FilterControl.GroupImages.Images[7]);
				item.BeginGroup = true;
				nodeActionMenu.Items.Add(item);
			}
			return nodeActionMenu;
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			return new AppearanceDefault(GetSystemColor(SystemColors.WindowText), GetSystemColor(SystemColors.Window));
		}
		public AppearanceObject PaintAppearanceTreeLine { get { return paintAppeareanceTreeLine; } }
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			AppearanceHelper.Combine(PaintAppearanceTreeLine, new AppearanceObject[] { OwnerControl.AppearanceTreeLine },
				new AppearanceDefault(Color.Gray, Color.White));
			if(!viewInfoCreated) return;
			UpdateAppearanceColors();
			ClearViewInfo();
			viewInfoCreated = false;
		}
		public void ClearViewInfo() {
			singleLineHeight = 0;
		}
		bool viewInfoCreated = false;
		internal void OnHandleCreated() {
			viewInfoCreated = true;
		}
	}
	public class FilterControlPainter : BaseControlPainter {
		FilterControl owner;
		public FilterControlPainter(FilterControl owner)
			: base() {
			this.owner = owner;
		}
		bool IsRightToLeft { get { return Owner.FilterViewInfo.RightToLeft; } }
		public FilterControl Owner { get { return owner; } }
		protected WinFilterTreeNodeModel Model { get { return Owner.Model; } }
		public override void Draw(ControlGraphicsInfoArgs info) {
			Owner.Model.CalcSizes(info);
			DrawNode(Owner.RootNode, info);
			Owner.UpdateHotTrackItem();
			Owner.RefreshEditorPosition();
			Owner.InvalidateScrollBarPosition();
			Owner.InvalidateHorzScroll();
			DrawFocusRectangle(info);
			BorderHelper.GetPainter(owner.BorderStyle, owner.LookAndFeel).DrawObject(new BorderObjectInfoArgs(info.Cache, owner.FilterViewInfo.PaintAppearance, owner.FilterViewInfo.Bounds, owner.FilterViewInfo.State));
			DrawScrollSquareRectangle(owner.ScrollSquareRectangle, info);
		}
		protected void DrawNode(Node node, ControlGraphicsInfoArgs info) {
			DrawNodeLabel(node, info);
			DrawNodeChildren(node, info);
		}
		protected void DrawNodeChildren(Node node, ControlGraphicsInfoArgs info) {
			if(node == null) return;
			foreach(Node subNode in node.GetChildren())
				DrawNode(subNode, info);
			DrawTreeLines(node, info);
		}
		protected virtual void DrawNodeLabel(Node node, ControlGraphicsInfoArgs info) {
			if(Model[node]== null) return;
			Model[node].Paint(info);
		}
		protected virtual void DrawTreeLines(Node node, ControlGraphicsInfoArgs info) {
			if(node.GetChildren().Count == 0) return;
			int textWidht = ((FilterLabelInfoTextViewInfo)Model[node].ViewInfo[0]).ItemBounds.Width;
			int treeLineIndent = Math.Min(textWidht / 2, Owner.LevelIndent / 2);
			int x = Model[node].TextLocation.X + treeLineIndent + (IsRightToLeft ? textWidht + treeLineIndent : 0);
			int y = Model[node].NodeBounds.Bottom;
			int height = 0;
			if(node.GetChildren().Count > 0)
				height = Model[((Node)node.GetChildren()[node.GetChildren().Count - 1])].TextBounds.Top + Owner.FilterViewInfo.SingleLineHeight / 2 - y;
			XPaint.Graphics.FillRectangle(info.Graphics, Owner.FilterViewInfo.LineBrush, new Rectangle(x, y, 1, height));
			foreach(Node subNode in node.GetChildren()) {
				y = Model[subNode].TextBounds.Top + Owner.FilterViewInfo.SingleLineHeight / 2;
				int width = Model[subNode].Left - x;
				if(IsRightToLeft) width = x - Model[subNode].TextLocation.X - Model[subNode].TextBounds.Width;
				XPaint.Graphics.FillRectangle(info.Graphics, Owner.FilterViewInfo.LineBrush, new Rectangle(x - (IsRightToLeft ? width : 0), y, width, 1));
			}
		}
		public virtual void DrawFocusRectangle(ControlGraphicsInfoArgs info) {
			if(Owner.ActiveEditor != null) return;
			Color focusRectColor = Owner.FilterViewInfo.GetSystemColor(SystemColors.Control);
			if(Owner.Focused) focusRectColor = Owner.FilterViewInfo.GetSystemColor(SystemColors.WindowText);
			if(Owner.ShowEditors && (Owner.FocusedItemType == ElementType.Value || Owner.FocusedItemType == ElementType.ItemCollection)) return;
			if(Owner.FocusedItem != null) {
				Rectangle rect = Owner.FocusedItem.ItemBounds;
				rect.Inflate(2, 3);
				rect.Width--;
				info.Paint.DrawFocusRectangle(info.Graphics, rect, focusRectColor, Color.Empty);
			}
		}
		public virtual void DrawScrollSquareRectangle(Rectangle rect, ControlGraphicsInfoArgs info) {
			if(rect.IsEmpty) return;
			info.ViewInfo.PaintAppearance.FillRectangle(info.Cache, rect);
		}
	}
	class BindingListFilterProxy : BindingListFilterProxyBase, IFilteredComponent
	{
		FilterTreeNodeModel model;
		public BindingListFilterProxy(IBindingList dataSource, FilterTreeNodeModel model) : base(dataSource) {
			this.model = model;
		}
		IBoundPropertyCollection IFilteredComponent.CreateFilterColumnCollection() {
			return new DataColumnInfoFilterColumnCollection(DataSource, model);
		}
	}
	public class DataColumnInfoFilterColumn : FilterColumn {
		public readonly DataColumnInfo Column;
		List<IBoundProperty> children;
		RepositoryItem resolvedEditor;
		string resolvedCaption;
		Image resolvedImage = null;
		IBoundProperty parent = null;
		bool isAggregate = false;
		bool isList;
		bool childrenWereExpanded = false;
		public DataColumnInfoFilterColumn(DataColumnInfo column, bool isList)
			: base() {
			children = new List<IBoundProperty>();
			this.Column = column;
			resolvedCaption = this.Column.Caption;
			this.isList = isList;
		}
		public override List<IBoundProperty> Children {
			get {
				if (!childrenWereExpanded && Model != null) {
					children = Model.GetChildrenProperties(this);
					childrenWereExpanded = true;
				}
				return children;
			}
		}
		internal void SetIsAggregate(bool isAggregate){
			this.isAggregate = isAggregate;
		}
		public override bool IsList {
			get {
				return isList;
			}
		}
		public override bool IsAggregate { get { return isAggregate; } }
		public override IBoundProperty Parent { get { return parent; } set { parent = value; } }
		public override bool HasChildren{
			get {
				return true;
			}
		}
		public override RepositoryItem ColumnEditor {
			get {
				if(resolvedEditor == null)
					resolvedEditor = CreateRepository();
				return resolvedEditor;
			}
		}
		public override void Dispose() {
			base.Dispose();
			if(resolvedEditor != null) {
				resolvedEditor.Dispose();
				resolvedEditor = null;
			}
		}
		protected virtual RepositoryItem CreateRepository() {
			CreateCustomRepositoryItemEventArgs args = new CreateCustomRepositoryItemEventArgs(this);
			if(CreateCustomRepositoryItem != null) {
				CreateCustomRepositoryItem(this, args);
			}
			if(args.RepositoryItem != null) {
				return args.RepositoryItem;
			}
			if(ColumnType == typeof(Boolean) || ColumnType == typeof(Boolean?))
				return new RepositoryItemCheckEdit();
			if(ColumnType == typeof(DateTime) || ColumnType == typeof(DateTime?))
				return new RepositoryItemDateEdit();
			if(ColumnType.IsEnum)
				return CreateEnumRepositoryItem(ColumnType);
			return new RepositoryItemTextEdit();
		}
		RepositoryItem CreateEnumRepositoryItem(Type enumType) {
			if(!enumType.IsEnum) return new RepositoryItemTextEdit();
			RepositoryItemImageComboBox ret = new RepositoryItemImageComboBox();
			foreach(object item in Enum.GetValues(enumType))
				ret.Items.Add(new ImageComboBoxItem(item.ToString(), item, -1));
			return ret;
		}
		public override string ColumnCaption {
			get { return resolvedCaption; }
		}
		public override Type ColumnType {
			get { return this.Column.Type; }
		}
		public override string FieldName {
			get { return this.Column.Name; }
		}
		public override Image Image {
			get { return resolvedImage; }
		}
		public override void SetColumnEditor(RepositoryItem item) {
			resolvedEditor = item;
		}
		public override void SetColumnCaption(string caption) {
			if (resolvedCaption == caption) return;
			resolvedCaption = caption;
			RebuildModel();
		}
		public override void SetImage(Image image) {
			resolvedImage = image;
		}
		public event EventHandler<CreateCustomRepositoryItemEventArgs> CreateCustomRepositoryItem;
	}
	public class DataColumnInfoFilterColumnCollection : FilterColumnCollection {
		public DataColumnInfoFilterColumnCollection(DataColumnInfo[] columns) {
			Fill(columns);
		}
		public DataColumnInfoFilterColumnCollection(BindingContext context, object dataSource, string dataMember, FilterTreeNodeModel model) {
			Fill(new DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(context, dataSource, dataMember));
		}
		public DataColumnInfoFilterColumnCollection(object dataSource, FilterTreeNodeModel model) {
			Fill(new DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(null, dataSource, null));
		}
		protected virtual FilterColumn CreateFilterColumn(DataColumnInfo column) {
			return new DataColumnInfoFilterColumn(column, false);
		}
		protected virtual void Fill(DataColumnInfo[] columns) {
			if(columns == null) return;
			foreach(DataColumnInfo column in columns) {
				this.Add(CreateFilterColumn(column));
			}
		}
	}
	public class UnboundFilterColumn : FilterColumn {
		string columnCaption, fieldName;
		Image columnImage = null;
		Type columnType;
		RepositoryItem columnEdit;
		FilterColumnClauseClass clauseClass;
		public UnboundFilterColumn(string columnCaption, string fieldName, Type columnType, RepositoryItem columnEdit, FilterColumnClauseClass clauseClass) {
			this.columnEdit = columnEdit;
			this.clauseClass = clauseClass;
			this.columnCaption = columnCaption;
			this.columnType = columnType;
			this.fieldName = fieldName;
		}
		public override FilterColumnClauseClass ClauseClass {
			get { return clauseClass; }
		}
		public override RepositoryItem ColumnEditor {
			get { return columnEdit; }
		}
		public override string ColumnCaption {
			get { return columnCaption; }
		}
		public override Type ColumnType {
			get { return columnType; }
		}
		public override string FieldName {
			get { return fieldName; }
		}
		public override Image Image {
			get { return columnImage; }
		}
		public override void SetColumnEditor(RepositoryItem item) {
			columnEdit = item;
		}
		public override void SetColumnCaption(string caption) {
			if(ColumnCaption == caption) return;
			string oldCaption = ColumnCaption;
			this.columnCaption = caption;
			if(!string.IsNullOrEmpty(oldCaption)) {
				RebuildModel();
			}
		}
		public override void SetImage(Image image) {
			columnImage = image;
		}
	}
	public class FilterControlAccessibleBase : BaseAccessible, IDisposable {
		public FilterControlAccessibleBase(FilterControl filterControl)
			: base() {
			FilterControl = filterControl;
		}
		public virtual void Dispose() { }
		public FilterControl FilterControl { get; set; }
		protected override void OnChildrenCountChanged() {
			if(Children != null) {
				foreach(var child in Children) {
					if(child is IDisposable) {
						((IDisposable)child).Dispose();
					}
				}
			}
			if(GetChildCount() > 0)
				CreateCollection();
		}
	}
	public class FilterControlNodeListAccessible : FilterControlNodeAccessibleBase {
		public FilterControlNodeListAccessible(FilterControl filterControl, Node node) : base(filterControl, node) {
			FilterControl.Model.OnNotifyControl += new FilterTreeNodeModel.NotifyControlDelegate(Model_OnNotifyControl);
		}
		public override void Dispose() {
			FilterControl.Model.OnNotifyControl -= new FilterTreeNodeModel.NotifyControlDelegate(Model_OnNotifyControl);
		}
		void Model_OnNotifyControl(FilterChangedEventArgs info) {
			OnChildrenCountChanged();
		}
		protected override AccessibleRole GetRole() {
			return AccessibleRole.List;
		}
		protected override ChildrenInfo GetChildrenInfo() {
			return new ChildrenInfo(ChildType.Item, Node.GetChildren().Count);
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			foreach(Node node in Node.GetChildren()) {
				Children.Add(new FilterControlNodeAccessible(FilterControl, node));
			}
		}
	}
	public class FilterControlElementListAccessible : FilterControlNodeAccessibleBase {
		public FilterControlElementListAccessible(FilterControl filterControl, Node node) : base(filterControl, node) {
			FilterControl.Model.VisualChange += new WinFilterTreeNodeModel.OnVisualChangeDelegate(Model_VisualChange);
		}
		public override void Dispose() {
			FilterControl.Model.VisualChange -= new WinFilterTreeNodeModel.OnVisualChangeDelegate(Model_VisualChange);
		}
		void Model_VisualChange(FilterChangedActionInternal action, Node node) {
			if(node == Node)
				OnChildrenCountChanged();
		}
		protected override AccessibleRole GetRole() {
			return AccessibleRole.List;
		}
		protected override ChildrenInfo GetChildrenInfo() {
			return new ChildrenInfo(ChildType.Item, Node.Elements.Count);
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			Children.Clear();
			foreach(NodeEditableElement elem in Node.Elements) {
				Children.Add(new FilterControlElementAccessible(FilterControl, Node, elem));
			}
		}
		public override Rectangle ClientBounds {
			get {
				FilterControlLabelInfo li = FilterControl.Model[Node];
				return new Rectangle(li.ClientLocation, li.ClientSize);
			}
		}
	}
	public class FilterControlElementAccessible : FilterControlNodeAccessibleBase {
		public FilterControlElementAccessible(FilterControl filterControl, Node node, NodeEditableElement elem)
			: base(filterControl, node) {
			Element = elem;
		}
		public NodeEditableElement Element { get; set; }
		protected override AccessibleRole GetRole() {
			switch (Element.ElementType) {
				case ElementType.Property:
				case ElementType.Operation:
				case ElementType.Group:
				case ElementType.AdditionalOperandParameter:
				case ElementType.AdditionalOperandProperty:
					return AccessibleRole.ComboBox;
				case ElementType.NodeAdd:
				case ElementType.NodeRemove:
				case ElementType.FieldAction:
					return AccessibleRole.PushButton;
				case ElementType.Value:
					return AccessibleRole.Text;
			}
			return AccessibleRole.PushButton;
		}
		protected override void DoDefaultAction() {
			int x = ClientBounds.Left + 1;
			int y = ClientBounds.Top + 1;
			FilterControl.SetActiveLabelInfo(x, y);
			FilterControlLabelInfo labelInfo = FilterControl.Model[Element.Node];
			FilterControl.HotTrackLabelInfoViewInfo.ActiveItem = labelInfo.ViewInfo[GetRealIndex(labelInfo, Element.Index)];
			FilterControl.SetMouseDown(new MouseEventArgs(MouseButtons.Left, 1, x, y, 0));
			FilterControl.OnElementClick(Element);
		}
		public override void Notify(AccessibleEvents accEvent, int childIndex) {
			base.Notify(accEvent, childIndex);
		}
		int GetRealIndex(FilterControlLabelInfo flivi, int logicalIndex) {
			int realIndex = 0;
			while(logicalIndex >= 0) {
				if(((FilterLabelInfoTextViewInfo)flivi.ViewInfo[realIndex]).TextElement.Text == " ")
					logicalIndex++;
				realIndex++;
				logicalIndex--;
			};
			return realIndex - 1;
		}
		public override Rectangle ClientBounds {
			get {
				FilterControlLabelInfo labelInfo = FilterControl.Model[Element.Node];
				LabelInfoTextViewInfoBase asBase = labelInfo.ViewInfo[GetRealIndex(labelInfo, Element.Index)];
				FilterLabelInfoTextViewInfo label = asBase as FilterLabelInfoTextViewInfo;
				return label.ItemBounds;
			}
		}
		protected override string GetDefaultAction() {
			return GetName();
		}
		protected override AccessibleStates GetState() {
			if(Element.ElementType == ElementType.NodeAdd || Element.ElementType == ElementType.NodeRemove)
				return AccessibleStates.Default;
			AccessibleStates res = AccessibleStates.Focusable | AccessibleStates.Selectable;
			if(FilterControl.FocusInfo.Node == Node && Element.Index == FilterControl.FocusInfo.ElementIndex)
				res |= AccessibleStates.Focused;
			return res;
		}
		protected override string GetName() {
			switch (Element.ElementType) {
				case ElementType.FieldAction:
					return Localizer.Active.GetLocalizedString(StringId.FilterToolTipValueType);
				case ElementType.CollectionAction:
					return Localizer.Active.GetLocalizedString(StringId.FilterToolTipElementAdd);
				case ElementType.NodeAdd:
					return Localizer.Active.GetLocalizedString(StringId.FilterToolTipNodeAdd);
				case ElementType.NodeRemove:
					return Localizer.Active.GetLocalizedString(StringId.FilterToolTipNodeRemove);
			}
			return Element.Text;
		}
	}
	public class FilterControlNodeAccessibleBase : FilterControlAccessibleBase {
		public FilterControlNodeAccessibleBase(FilterControl filterControl, Node node)
			: base(filterControl) {
			Node = node;
		}
		public Node Node { get; set; }
	}
	public class FilterControlNodeAccessible : FilterControlNodeAccessibleBase {
		public FilterControlNodeAccessible(FilterControl filterControl, Node node)
			: base(filterControl, node) {
		}
		protected override string GetName() {
			return Node.Text;
		}
		protected override AccessibleRole GetRole() {
			return AccessibleRole.List;
		}
		protected override AccessibleStates GetState() {
			return AccessibleStates.Selectable | AccessibleStates.Focusable;
		}
		public override string Value {
			get {
				return GetName();
			}
		}
		protected override ChildrenInfo GetChildrenInfo() {
			return new ChildrenInfo(ChildType.Item, 2);
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			Children.Add(new FilterControlElementListAccessible(FilterControl, Node));
			Children.Add(new FilterControlNodeListAccessible(FilterControl, Node));
		}
	}
	public class FilterControlAccessible : FilterControlAccessibleBase {
		public FilterControlAccessible(FilterControl filterControl)
			: base(filterControl) {
		}
		protected override AccessibleRole GetRole() {
			return AccessibleRole.Pane;
		}
		protected override AccessibleStates GetState() {
			AccessibleStates res = AccessibleStates.Selectable | AccessibleStates.Focusable;
			if(FilterControl.Focused)
				res |= AccessibleStates.Focused;
			return res;
		}
		public override Rectangle ClientBounds {
			get {
				return FilterControl.ClientRectangle;
			}
		}
		protected override string GetName() {
			return FilterControl.Name;
		}
		public override Control GetOwnerControl() {
			return FilterControl;
		}
		protected override ChildrenInfo GetChildrenInfo() {
			return new ChildrenInfo("Node", 1);
		}
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			Children.Add(new FilterControlNodeAccessible(FilterControl, FilterControl.RootNode));
		}
	}
}
