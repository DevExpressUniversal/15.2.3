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

using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.API.Native;
using System.Drawing;
using System.ComponentModel;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.XtraTab;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraFilterEditor.IntelliSense;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using System;
using System.Linq;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Frames;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.XtraFilterEditor {
	public enum FilterEditorActiveView { Text, Visual }
	[DXToolboxItem(true),
	 ToolboxBitmap(typeof(RichEditControl), DevExpress.Utils.ControlConstants.BitmapPath + "FilterEditControl.bmp"),
	Description("Allows end-users to construct filter criteria in a text and visual tree-like forms, and apply them to controls."),
	 Designer("DevExpress.Utils.Design.BaseControlDesignerSimple, " + AssemblyInfo.SRAssemblyDesign),
	 ToolboxTabName(AssemblyInfo.DXTabData)
	]
	public class FilterEditorControl : BaseStyleControl, ISyntaxHighlightService, IDisposable, IFilterControl, IFilterControlGetModel {
		XtraTabControl tab;
		XtraTabPage editorPage, treePage;
		RichEditControl edit;
		FilterControl tree;
		FilterEditorViewMode viewMode;
		FilterEditorActiveView activeView;
		IFilterEditorIntelliSenseWindow intelliSenseWindow;
		Dictionary<string, string> tokenTextConverter;
		DocumentRange previousTokenRange;
		Control rootParent;
		internal int silentEditorChangedCounter;
		bool isModified = false;
		internal EditorTextConverter editorTextConverter;
		internal const int GroupColor = 0;
		internal const int PropertyColor = 1;
		internal const int ClauseColor = 2;
		internal const int ValueColor = 3;
		internal const int EmptyValueColor = 4;
		string originalFilterString = "";
		bool originalFilterStringIsDirty = true;
		internal static Color[] DefaultColorValues = new Color[] { Color.Red, Color.Blue,
			Color.Green, Color.Black, Color.Gray };
		private static readonly object filterTextChanged = new object();
		private static readonly object createCriteriaParseContext = new object();
		private static readonly object createCriteriaCustomParse = new object();
		private static readonly object createCustomRepositoryItem = new object();
		protected override Size DefaultSize { get { return new Size(200, 100); } }
		public FilterEditorControl() {
			editorTextConverter = new EditorTextConverter(this);
			ViewMode = FilterEditorViewMode.VisualAndText;
			ActiveView = FilterEditorActiveView.Visual;
			this.intelliSenseWindow = CreateIntelliSenseWindow();
			this.tokenTextConverter = null;
			PreviousTokenRange = null;
			rootParent = null;
			ParentChanged += new EventHandler(FilterEditorControl_ParentChanged);
		}
		protected override Size CalcSizeableMaxSize() {
			return Size.Empty;
		}
		protected override void OnCreateControl() {
			CreateControls();
			base.OnCreateControl();
		}
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("FilterEditorControlBeforeShowValueEditor")]
#endif
		public event ShowValueEditorEventHandler BeforeShowValueEditor {
			add { Tree.BeforeShowValueEditor += value; }
			remove { Tree.BeforeShowValueEditor -= value; }
		}
		bool IsControlsCreated { get { return this.tab != null; } }
		void CreateControls() {
			if(IsControlsCreated)
				return;
			this.tab = new XtraTabControl();
			Tab.Parent = this;
			Tab.Dock = DockStyle.Fill;
			Tab.ShowTabHeader = DefaultBoolean.True;
			this.treePage = Tab.TabPages.Add(Localizer.Active.GetLocalizedString(StringId.FilterEditorTabVisual));
			this.editorPage = Tab.TabPages.Add(Localizer.Active.GetLocalizedString(StringId.FilterEditorTabText));
			Tab.SelectedPageChanged += new TabPageChangedEventHandler(Tab_SelectedPageChanged);
			this.edit = CreateEditControl();
			this.tree = CreateTreeControl();
			this.tree.FilterStringChanged += new EventHandler(tree_FilterStringChanged);
			this.tree.CreateCriteriaParseContext += delegate(object sender, CreateCriteriaParseContextEventArgs e) {
				RaiseCreateCriteriaParseContext(e);
			};
			this.tree.CreateCriteriaCustomParse += delegate(object sender, CreateCriteriaCustomParseEventArgs e) {
				RaiseCreateCriteriaCustomParse(e);
			};
			this.tree.CreateCustomRepositoryItem += delegate(object sender, CreateCustomRepositoryItemEventArgs e) {
				RaiseCreateCustomRepositoryItem(e);
			};
			PrepareTreeControl(TreePage);
			PrepareEditControl(EditorPage);
			Tab.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			Editor.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			Tree.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			UpdateBorderStyle();
			OnViewModeChanged();
			OnActiveViewChanged();
		}
		protected bool IsModified { get { return isModified; } set { isModified = value; } }
		void tree_FilterStringChanged(object sender, EventArgs e) {
			IsModified = true;
		}
		#region FilterControl public members
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("FilterEditorControlFilterChanged")]
#endif
		public event FilterChangedEventHandler FilterChanged {
			add { Tree.FilterChanged += value; }
			remove { Tree.FilterChanged -= value; }
		}
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("FilterEditorControlActiveEditorValidating")]
#endif
		public event FilterActiveEditorValidatingEventHandler ActiveEditorValidating {
			add { Tree.ActiveEditorValidating += value; }
			remove { Tree.ActiveEditorValidating -= value; }
		}
		[DefaultValue(true)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowCreateDefaultClause {
			get { return Tree.AllowCreateDefaultClause; }
			set { Tree.AllowCreateDefaultClause = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public WinFilterTreeNodeModel Model {
			get { return Tree.Model; }
		}
		[Browsable(false)]
		public BaseEdit ActiveEditor { get { return Tree.ActiveEditor; } }
		public void SetFilterColumnsCollection(FilterColumnCollection columns) { Tree.SetFilterColumnsCollection(columns); }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlMenuManager"),
#endif
DefaultValue(null), Category(CategoryName.BarManager)]
		public IDXMenuManager MenuManager {
			get { return Tree.MenuManager; }
			set { Tree.MenuManager = value; }
		}
		public void ApplyFilter() {
			if(ActiveView == FilterEditorActiveView.Text) {
				bool isFilterStringValid, canBeDisplayedByTree;
				CriteriaOperator criteria = GetCriteriaFromEditorText(out isFilterStringValid, out canBeDisplayedByTree);
				if(canBeDisplayedByTree) {
					Tree.FilterCriteria = criteria;
				}
				IFilteredComponentBase filterComponent = Tree.SourceControl as IFilteredComponentBase;
				if(filterComponent != null) {
					filterComponent.RowCriteria = criteria;
				}
			} else {
				Tree.ApplyFilter();
			}
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlLevelIndent"),
#endif
DefaultValue(20)]
		public int LevelIndent {
			get { return Tree.LevelIndent; }
			set { Tree.LevelIndent = value; }
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlNodeSeparatorHeight"),
#endif
DefaultValue(0)]
		public int NodeSeparatorHeight {
			get { return Tree.NodeSeparatorHeight; }
			set { Tree.NodeSeparatorHeight = value; }
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlShowFunctions"),
#endif
DefaultValue(true)]
		public bool ShowFunctions {
			get { return Tree.ShowOperandCustomFunctions; }
			set { Tree.ShowOperandCustomFunctions = value; }
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlShowDateTimeOperators"),
#endif
DefaultValue(true)]
		public bool ShowDateTimeOperators {
			get { return Tree.ShowDateTimeOperators; }
			set { Tree.ShowDateTimeOperators = value; }
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlShowGroupCommandsIcon"),
#endif
DefaultValue(false)]
		public bool ShowGroupCommandsIcon {
			get { return Tree.ShowGroupCommandsIcon; }
			set { Tree.ShowGroupCommandsIcon = value; }
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlUseMenuForOperandsAndOperators"),
#endif
		DefaultValue(true)]
		public bool UseMenuForOperandsAndOperators
		{
			get { return Tree.UseMenuForOperandsAndOperators; }
			set { Tree.UseMenuForOperandsAndOperators = value; }
		}
		[Browsable(false)]
		public FilterControlViewInfo FilterViewInfo { get { return Tree.FilterViewInfo; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override IStyleController StyleController {
			get { return null; }
			set { }
		}
		protected internal override void OnPropertiesChanged() {
			base.OnPropertiesChanged();
			UpdateControlsFont();
		}
		void UpdateControlsFont() {
			if(Tree.Font != Font) Tree.Font = Font;
			if(Editor.Font != Font) { Editor.Font = Font; }
			if(Tab.AppearancePage.Header.Font != Font) { Tab.AppearancePage.Header.Font = Font; }
		}
		#region Appearance properties
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlBorderStyle"),
#endif
DefaultValue(BorderStyles.Default)]
		public override BorderStyles BorderStyle {
			get { return base.BorderStyle; }
			set {
				base.BorderStyle = value;
				UpdateBorderStyle();
			}
		}
		void UpdateBorderStyle() {
			if(!IsControlsCreated) return;
			if(ViewMode == FilterEditorViewMode.Visual)
				Tree.BorderStyle = BorderStyle;
			else if(ViewMode == FilterEditorViewMode.Text)
				Editor.BorderStyle = BorderStyle;
			else
				Tree.BorderStyle = Editor.BorderStyle = BorderStyles.NoBorder;
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlAppearanceTreeLine"),
#endif
		Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject AppearanceTreeLine { get { return Tree.AppearanceTreeLine; } }
		bool ShouldSerializeAppearanceTreeLine() { return AppearanceTreeLine.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlAppearanceGroupOperatorColor"),
#endif
Category("Appearance")]
		public Color AppearanceGroupOperatorColor {
			get { return Tree.AppearanceGroupOperatorColor; }
			set { Tree.AppearanceGroupOperatorColor = value; }
		}
		bool ShouldSerializeAppearanceGroupOperatorColor() { return AppearanceGroupOperatorColor != DefaultColorValues[GroupColor]; }
		void ResetAppearanceGroupOperatorColor() { AppearanceGroupOperatorColor = DefaultColorValues[GroupColor]; }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlAppearanceFieldNameColor"),
#endif
Category("Appearance")]
		public Color AppearanceFieldNameColor {
			get { return Tree.AppearanceFieldNameColor; }
			set { Tree.AppearanceFieldNameColor = value; }
		}
		bool ShouldSerializeAppearanceFieldNameColor() { return AppearanceFieldNameColor != DefaultColorValues[PropertyColor]; }
		void ResetAppearanceFieldNameColor() { AppearanceFieldNameColor = DefaultColorValues[PropertyColor]; }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlAppearanceOperatorColor"),
#endif
Category("Appearance")]
		public Color AppearanceOperatorColor {
			get { return Tree.AppearanceOperatorColor; }
			set { Tree.AppearanceOperatorColor = value; }
		}
		bool ShouldSerializeAppearanceOperatorColor() { return AppearanceOperatorColor != DefaultColorValues[ClauseColor]; }
		void ResetAppearanceOperatorColor() { AppearanceOperatorColor = DefaultColorValues[ClauseColor]; }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlAppearanceValueColor"),
#endif
Category("Appearance")]
		public Color AppearanceValueColor {
			get { return Tree.AppearanceValueColor; }
			set { Tree.AppearanceValueColor = value; }
		}
		bool ShouldSerializeAppearanceValueColor() { return AppearanceValueColor != DefaultColorValues[ValueColor]; }
		void ResetAppearanceValueColor() { AppearanceValueColor = DefaultColorValues[ValueColor]; }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlAppearanceEmptyValueColor"),
#endif
Category("Appearance")]
		public Color AppearanceEmptyValueColor {
			get { return Tree.AppearanceEmptyValueColor; }
			set { Tree.AppearanceEmptyValueColor = value; }
		}
		bool ShouldSerializeAppearanceEmptyValueColor() { return AppearanceEmptyValueColor != DefaultColorValues[EmptyValueColor]; }
		void ResetAppearanceEmptyValueColor() { AppearanceEmptyValueColor = DefaultColorValues[EmptyValueColor]; }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlViewMode"),
#endif
Category("Appearance")]
		public FilterEditorViewMode ViewMode {
			get { return viewMode; }
			set {
				if (ViewMode == value) return;
				this.viewMode = value;
				OnViewModeChanged();
				UpdateBorderStyle();
			}
		}
		bool ShouldSerializeViewMode() { return ViewMode != FilterEditorViewMode.VisualAndText; }
		void ResetViewMode() { ViewMode = FilterEditorViewMode.VisualAndText; }
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlActiveView"),
#endif
Category("Appearance")]
		public FilterEditorActiveView ActiveView {
			get { return activeView; }
			set {
				if (value == ActiveView) return;
				if (value == FilterEditorActiveView.Text && !CanHasEditor) {
					ViewMode = FilterEditorViewMode.Text;
				}
				if (value == FilterEditorActiveView.Visual && !CanHasTree) {
					ViewMode = FilterEditorViewMode.Visual;
				}
				activeView = value;
				OnActiveViewChanged();
			}
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlFont"),
#endif
Category("Appearance")]
		public override Font Font {
			get { return base.Font; }
			set {
				if (base.Font != value) { base.Font = value; }
			}
		}
		bool ShouldSerializeActiveView() { return ActiveView != FilterEditorActiveView.Visual; }
		void ResetActiveView() { ActiveView = FilterEditorActiveView.Visual; }
		#endregion
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("FilterEditorControlItemDoubleClick")]
#endif
		public event LabelInfoItemClickEvent ItemDoubleClick {
			add { Tree.ItemDoubleClick += value; }
			remove { Tree.ItemDoubleClick -= value; }
		}
		#endregion
		string OriginalFilterString {
			get {
				if(originalFilterStringIsDirty) {
					if(ActiveView == FilterEditorActiveView.Text) {
						CriteriaOperator criteria = editorTextConverter.ConvertFromEditor(EditorText);
						if(!ReferenceEquals(criteria, null))
							return Tree.Model.CriteriaSerialize(criteria);
						return EditorText;
					}
					return Tree.FilterString;
				}
				return originalFilterString;
			}
			set {
				originalFilterStringIsDirty = false;
				originalFilterString = value;
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string FilterString {
			get {
				return OriginalFilterString;
			}
			set {
				string displayText = editorTextConverter.ConvertToEditor(value);
				if(String.IsNullOrEmpty(displayText))
					displayText = value;
				SetEditorTextInSilent(displayText);
				ChangeToTextIfCantCreateTree(EditorText);
				try { 
					Tree.FilterString = value;
				} catch { }
				bool isFilterStringValid;
				bool treeCanBeDisplayed;
				CanShowTreePageOnEditorTextChanged(out isFilterStringValid, out treeCanBeDisplayed, value);
				if(!treeCanBeDisplayed && isFilterStringValid)
					OriginalFilterString = value;
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CriteriaOperator FilterCriteria {
			get {
				if(ActiveView == FilterEditorActiveView.Text) {
					bool isFilterStringValid, canBeDisplayedByTree;
					CriteriaOperator criteria = GetCriteriaFromEditorText(out isFilterStringValid, out canBeDisplayedByTree);
					return isFilterStringValid ? criteria : null;
				}
				return Tree.FilterCriteria;
			}
			set {
				if (!ReferenceEquals(value, null))
					ChangeToTextIfCantCreateTree(value.ToString());
					if(object.Equals(null, value)) {
						Node node = Tree.CreateCriteriaByDefaultColumn();
						if(node != null) {
							value = FilterControlHelpers.ToCriteria(node);
						}
					}
					Tree.FilterCriteria = value;
					if (!ReferenceEquals(value, null)) {
						bool isFilterStringValid;
						bool treeCanBeDisplayed;
						CanShowTreePageOnEditorTextChanged(out isFilterStringValid, out treeCanBeDisplayed, value.ToString());
						OriginalFilterString = value.ToString();
					}
				SetEditorTextInSilent(editorTextConverter.ConvertToEditor(OriginalFilterString));
				}
			}
		void ChangeToTextIfCantCreateTree(string editorText) {
			if (ActiveView == FilterEditorActiveView.Visual) {
				bool isFilterStringValid, treeCanBeDisplayed;
				CanShowTreePageOnEditorTextChanged(out isFilterStringValid, out treeCanBeDisplayed, editorText);
				if (!treeCanBeDisplayed)
					ActiveView = FilterEditorActiveView.Text;
			}
		}
		protected internal RichEditControl Editor { get { CreateControls(); return edit; } }
		protected internal XtraTabControl Tab { get { CreateControls(); return tab; } }
		[Browsable(false)]
		public FilterControl FilterControl { get { return tree; } }
		protected internal FilterControl Tree { get { CreateControls(); return tree; } }
		protected XtraTabPage TreePage { get { CreateControls(); return treePage; } }
		protected XtraTabPage EditorPage { get { CreateControls(); return editorPage; } }
		protected internal IFilterEditorIntelliSenseWindow IntelliSenseWindow { get { return intelliSenseWindow; } }
		protected DocumentRange PreviousTokenRange { get { return previousTokenRange; } set { previousTokenRange = value; } }
		protected int EditorCaretPosition { get { return Editor.Document.CaretPosition.ToInt(); } }
		protected bool IsMultiView {
			get {
				return ViewMode == FilterEditorViewMode.TextAndVisual ||
					ViewMode == FilterEditorViewMode.VisualAndText;
			}
		}
		protected bool CanHasTree { get { return ViewMode != FilterEditorViewMode.Text; } }
		protected bool CanHasEditor { get { return ViewMode != FilterEditorViewMode.Visual; } }
		protected internal string EditorText {
			get { return Editor.Text.Replace("\r\n","\n"); }
			set {
				originalFilterStringIsDirty = true;
				Editor.Text = value;
			}
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlSourceControl"),
#endif
DefaultValue(null), TypeConverter(typeof(FilterControlConverter))]
		public object SourceControl {
			get { return Tree.SourceControl; }
			set {
				Tree.SourceControl = value;
				SetEditorTextInSilent(editorTextConverter.ConvertToEditor(OriginalFilterString));
				bool isFilterStringValid;
				bool treeCanBeDisplayed;
				CanShowTreePageOnEditorTextChanged(out isFilterStringValid, out treeCanBeDisplayed, OriginalFilterString);
				if(treeCanBeDisplayed && ViewMode == FilterEditorViewMode.VisualAndText || ViewMode == FilterEditorViewMode.Visual) {
					ActiveView = FilterEditorActiveView.Visual;
				}
			}
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlSortFilterColumns"),
#endif
DefaultValue(true)]
		public bool SortFilterColumns {
			get { return Tree.SortFilterColumns; }
			set { Tree.SortFilterColumns = value; }
		}
		[Browsable(false)]
		public FilterColumnCollection FilterColumns { get { return Tree.FilterColumns; } }
		protected Dictionary<string, string> TokenTextConverter { get { return tokenTextConverter; } set { tokenTextConverter = value; } }
		protected Control RootParent { get { return rootParent; } set { rootParent = value; } }
		protected virtual RichEditControl CreateEditControl() {  return new CustomRichEditControl(this); }
		class FakeThreadSyncService : DevExpress.Office.Services.Implementation.IThreadSyncService {
			public void EnqueueInvokeInUIThread(Action action) {
			}
		}
		class CustomRichEditControl : RichEditControl {
			FilterEditorControl owner;
			public CustomRichEditControl(FilterEditorControl owner) {
				this.owner = owner;
				Options.DocumentCapabilities.Bookmarks = DocumentCapability.Disabled;
				Options.DocumentCapabilities.Tables = DocumentCapability.Disabled;
				Options.DocumentCapabilities.Sections = DocumentCapability.Disabled;
				Options.DocumentCapabilities.Numbering.Simple = DocumentCapability.Disabled;
				Options.DocumentCapabilities.Numbering.MultiLevel = DocumentCapability.Disabled;
				Options.DocumentCapabilities.Numbering.Bulleted = DocumentCapability.Disabled;
				Options.DocumentCapabilities.InlinePictures = DocumentCapability.Disabled;
				Options.DocumentCapabilities.HeadersFooters = DocumentCapability.Disabled;
				Options.DocumentCapabilities.Hyperlinks = DocumentCapability.Disabled;
				DevExpress.Office.Services.Implementation.IThreadSyncService oldService = GetService<DevExpress.Office.Services.Implementation.IThreadSyncService>();
				RemoveService(typeof(DevExpress.Office.Services.Implementation.IThreadSyncService));
				AddService(typeof(DevExpress.Office.Services.Implementation.IThreadSyncService), new FakeThreadSyncService());
				Options.Behavior.FontSource = RichEditBaseValueSource.Control;
				RemoveService(typeof(DevExpress.Office.Services.Implementation.IThreadSyncService));
				if (oldService != null)
					AddService(typeof(DevExpress.Office.Services.Implementation.IThreadSyncService), oldService);
			}
			protected internal override DevExpress.XtraRichEdit.Internal.InnerRichEditControl CreateInnerControl() {
				return new CustomInnerRichEditControl(this);
			}
			protected override void OnKeyPress(KeyPressEventArgs e) {
				base.OnKeyPress(e);
				this.owner.EditorKeyPressHandler(e);
			}
		}
		class CustomInnerRichEditControl : DevExpress.XtraRichEdit.Internal.InnerRichEditControl {
			public CustomInnerRichEditControl(DevExpress.XtraRichEdit.Internal.IInnerRichEditControlOwner owner) : base(owner) {
			}
			protected internal override DevExpress.XtraRichEdit.Keyboard.NormalKeyboardHandler CreateDefaultKeyboardHandler() {
				return new CustomNormalKeyboardHandler();
			}
			protected internal override void InitializeDefaultViewKeyboardHandlers(XtraRichEdit.Keyboard.NormalKeyboardHandler keyboardHandler, Utils.KeyboardHandler.IKeyHashProvider provider) {
				keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, RichEditCommandId.PreviousCharacter);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, RichEditCommandId.NextCharacter);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, RichEditCommandId.ExtendPreviousCharacter);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, RichEditCommandId.ExtendNextCharacter);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Control, RichEditCommandId.PreviousWord);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Control, RichEditCommandId.NextWord);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Control | Keys.Shift, RichEditCommandId.ExtendPreviousWord);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Control | Keys.Shift, RichEditCommandId.ExtendNextWord);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, RichEditCommandId.StartOfLine);
				keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, RichEditCommandId.EndOfLine);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Shift, RichEditCommandId.ExtendStartOfLine);
				keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Shift, RichEditCommandId.ExtendEndOfLine);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Control, RichEditCommandId.StartOfDocument);
				keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Control, RichEditCommandId.EndOfDocument);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Control | Keys.Shift, RichEditCommandId.ExtendStartOfDocument);
				keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Control | Keys.Shift, RichEditCommandId.ExtendEndOfDocument);
				keyboardHandler.RegisterKeyHandler(provider, Keys.V, Keys.Control, RichEditCommandId.PasteSelection);
				keyboardHandler.RegisterKeyHandler(provider, Keys.V, Keys.Control | Keys.Alt, RichEditCommandId.ShowPasteSpecialForm);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Insert, Keys.Shift, RichEditCommandId.PasteSelection);
				keyboardHandler.RegisterKeyHandler(provider, Keys.C, Keys.Control, RichEditCommandId.CopySelection);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Insert, Keys.Control, RichEditCommandId.CopySelection);
				keyboardHandler.RegisterKeyHandler(provider, Keys.X, Keys.Control, RichEditCommandId.CutSelection);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Delete, Keys.Shift, RichEditCommandId.CutSelection);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Delete, Keys.None, RichEditCommandId.Delete);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.None, RichEditCommandId.BackSpaceKey);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Shift, RichEditCommandId.BackSpaceKey);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Delete, Keys.Control, RichEditCommandId.DeleteWord);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Control, RichEditCommandId.DeleteWordBack);
				keyboardHandler.RegisterKeyHandler(provider, Keys.A, Keys.Control, RichEditCommandId.SelectAll);
				keyboardHandler.RegisterKeyHandler(provider, Keys.NumPad5, Keys.Control, RichEditCommandId.SelectAll);
#if !SL
				keyboardHandler.RegisterKeyHandler(provider, Keys.Clear, Keys.Control, RichEditCommandId.SelectAll);
#endif
				keyboardHandler.RegisterKeyHandler(provider, Keys.Z, Keys.Control, RichEditCommandId.Undo);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Y, Keys.Control, RichEditCommandId.Redo);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Alt, RichEditCommandId.Undo);
				keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Alt | Keys.Shift, RichEditCommandId.Redo);
			}
		}
		class CustomNormalKeyboardHandler : DevExpress.XtraRichEdit.Keyboard.NormalKeyboardHandler {
			protected internal override void FlushPendingTextInput() {
			}
			public override bool HandleKeyPress(char character, Keys modifier) {
				DevExpress.XtraRichEdit.Commands.InsertTextCommand command = new DevExpress.XtraRichEdit.Commands.InsertTextCommand(View.Control);
				command.Text = new String(character, 1);
				command.CommandSourceType = DevExpress.Utils.Commands.CommandSourceType.Keyboard;
				command.Execute();
				return true;
			}
		}
		protected override void OnValidating(CancelEventArgs e) {
			if (IsModified) {
				base.OnValidating(e);
				IsModified = false;
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (IntelliSenseWindow.IsShowing) {
						IntelliSenseWindow.Hide();
					}
					if (edit != null) {
						RichEditControl control = edit;
						edit = null;
						control.Dispose();
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected bool IsTabVisible { get { return ViewMode == FilterEditorViewMode.TextAndVisual || ViewMode == FilterEditorViewMode.VisualAndText; } }
		void OnViewModeChanged() {
			if(!IsControlsCreated) return;
			if (IsTabVisible) {
				Editor.Parent = editorPage;
				Tree.Parent = treePage;
				Editor.Visible = true;
				Tree.Visible = true;
				Tab.Visible = true;
				UpdateTab();
			} else {
				Tab.Visible = false;
				Editor.Visible = false;
				Tree.Visible = false;
				Editor.Parent = this;
				Tree.Parent = this;
				Editor.Visible = ViewMode == FilterEditorViewMode.Text;
				Tree.Visible = ViewMode == FilterEditorViewMode.Visual;
			}
			if(ViewMode == FilterEditorViewMode.Text || ViewMode == FilterEditorViewMode.TextAndVisual) {
				ActiveView = FilterEditorActiveView.Text;
			} else {
				ActiveView = FilterEditorActiveView.Visual;
			}
		}
		void OnActiveViewChanged() {
			if(!IsControlsCreated) return;
			if(IsTabVisible) {
				Tab.SelectedTabPage = ActiveView == FilterEditorActiveView.Text ? EditorPage : TreePage;
			}
			if(ActiveView == FilterEditorActiveView.Text) {
				if(IsTabVisible && TreePage.PageEnabled) {
					SetEditorTextInSilent(editorTextConverter.ConvertToEditor(Tree.FilterString));
				}
			} else {
				FilterColumn saveDefaultColumn = Tree.GetDefaultColumn();
				Tree.SetDefaultColumn(null);
				Tree.FilterCriteria = editorTextConverter.ConvertFromEditor(EditorText);
				Tree.SetDefaultColumn(saveDefaultColumn);
			}
		}
		internal class EditorTextConverter : DisplayNameVisitor {
			List<ConstantConverterItem> knownItems = new List<ConstantConverterItem>();
			FilterEditorControl control;
			public CriteriaOperator ConvertFromEditor(string editorText) {
				return Convert(editorText, true);
			}
			public string ConvertToEditor(string editorText) {
				knownItems.Clear();
				CriteriaOperator result = Convert(editorText, false);
				if(ReferenceEquals(result, null))
					return "";
				return result.ToString();
			}
			CriteriaOperator Convert(string editorText, bool fromEditor) {
				CriteriaOperator criteria = null;
				try {
					control.WrapParseCriteria(() => {
						criteria = control.Model.CriteriaParse(editorText);
						return true;
					});
				} catch { }
				if(ReferenceEquals(criteria, null))
					return null;
				FromEditor = fromEditor;
				Columns = control.FilterColumns.Cast<IBoundProperty>();
				criteria.Accept(this);
				return criteria;
			}
			public ConstantConverterItem GetConstantConverterItem(FilterColumn filterColumn, string displayText) {
				string filterColumnFullName = filterColumn.GetFullName();
				foreach(ConstantConverterItem item in knownItems) {
					if(item.DisplayText == displayText && item.FilterColumn.GetFullName() == filterColumnFullName) {
						return item;
					}
				}
				return null;
			}
			public EditorTextConverter(FilterEditorControl control) {
				this.control = control;
			}
			public void AddKnownItem(ConstantConverterItem item) {
				knownItems.Add(item);
			}
			public override void Visit(InOperator theOperator) {
				for(int i = 0; i < theOperator.Operands.Count; ++i) {
					if(theOperator.Operands[i] is ConstantValue && theOperator.LeftOperand is OperandProperty)
						((ConstantValue)theOperator.Operands[i]).Value =
							VisitValue((OperandProperty)theOperator.LeftOperand, ((ConstantValue)theOperator.Operands[i]).Value);
				}
				base.Visit(theOperator);
			}
			public override void Visit(BinaryOperator theOperator) {
				if(!ReferenceEquals(theOperator.LeftOperand, null) &&
				   !ReferenceEquals(theOperator.RightOperand, null)) {
					if(theOperator.RightOperand is ConstantValue &&
					   theOperator.LeftOperand is OperandProperty) {
						object value = ((ConstantValue)theOperator.RightOperand).Value;
						((ConstantValue)theOperator.RightOperand).Value = VisitValue((OperandProperty)theOperator.LeftOperand, value);
					}
				}
				base.Visit(theOperator);
			}
			object VisitValue(OperandProperty property, object value) {
				var column = GetCurrentProperty(property.PropertyName) as FilterColumn;
				if(column != null && column.ColumnEditor != null && column.ColumnEditor.IsFilterLookUp) {
					if(value == null)
						value = "";
					if(!FromEditor) {
						string displayText = column.ColumnEditor.GetDisplayText(value).Replace('\'', ' ');
						AddKnownItem(new ConstantConverterItem(column, displayText, control.SerializeFilterValue(column, value), value));
						return displayText;
					} else {
						var knownItem = knownItems.Find(item => item.DisplayText == value.ToString() &&
																item.FilterColumn.GetFullNameWithLists() == column.GetFullNameWithLists());
						if(knownItem != null) {
							return knownItem.Value;
						}
					}
				}
				return value;
			}
		}
		protected virtual void SetEditorTextInSilent(string value) {
			if(EditorText == value) return;
			this.silentEditorChangedCounter++;
			EditorText = value;
		}
		protected virtual IFilterEditorIntelliSenseWindow CreateIntelliSenseWindow() {
			IFilterEditorIntelliSenseWindow window = new FilterEditorIntelliSenseWindow();
			window.GotFocus += new System.EventHandler(Window_GotFocus);
			window.IntelliSenseListDoubleClick += new System.EventHandler(Window_IntelliSenseListDoubleClick);
			window.EditorClosed += new EventHandler(Window_EditorClosed);
			return window;
		}
		protected virtual void PrepareEditControl(Control parent) {
			Editor.Parent = parent;
			Editor.Dock = DockStyle.Fill;
			Editor.BorderStyle = BorderStyles.NoBorder;
			Editor.AddService(typeof(ISyntaxHighlightService), this);
			Editor.ActiveViewType = RichEditViewType.Simple;
			Editor.Options.VerticalScrollbar.Visibility = RichEditScrollbarVisibility.Hidden;
			Editor.Options.HorizontalScrollbar.Visibility = RichEditScrollbarVisibility.Hidden;
			Editor.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
			Editor.PopupMenuShowing += new DevExpress.XtraRichEdit.PopupMenuShowingEventHandler(Editor_PopupMenuShowing);
			Editor.ContentChanged += new System.EventHandler(Editor_ContentChanged);
			Editor.KeyDown += new KeyEventHandler(Editor_KeyDown);
			Editor.KeyUp += new KeyEventHandler(Editor_KeyUp);
			Editor.MouseDown += new MouseEventHandler(Editor_MouseDown);
			Editor.MouseDoubleClick += new MouseEventHandler(Editor_MouseDoubleClick);
			Editor.MouseWheel += new MouseEventHandler(Editor_MouseWheel);
			Editor.LostFocus += new EventHandler(Editor_LostFocus);
			Editor.EmptyDocumentCreated += new EventHandler(Editor_EmptyDocumentCreated);
			Editor.DocumentLoaded += new EventHandler(Editor_DocumentLoaded);
			Editor.TextChanged += new EventHandler(Editor_TextChanged);
		}
		void Editor_TextChanged(object sender, EventArgs e) {
			originalFilterStringIsDirty = true;
		}
		class RichEditClipboardContentMenuBuilder : DevExpress.XtraRichEdit.Menu.RichEditMenuBuilder {
			public RichEditClipboardContentMenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<DevExpress.XtraRichEdit.Commands.RichEditCommand, DevExpress.XtraRichEdit.Commands.RichEditCommandId> uiFactory)
				: base(control, uiFactory) {
			}
			public override void PopulatePopupMenu(IDXPopupMenu<DevExpress.XtraRichEdit.Commands.RichEditCommandId> menu) {
				DevExpress.XtraRichEdit.Internal.InnerRichEditControl innerControl = Control.InnerControl;
				AddMenuItem(menu, innerControl.CreateCommand(DevExpress.XtraRichEdit.Commands.RichEditCommandId.CutSelection)).BeginGroup = true;
				AddMenuItem(menu, innerControl.CreateCommand(DevExpress.XtraRichEdit.Commands.RichEditCommandId.CopySelection));
				AddMenuItem(menu, innerControl.CreateCommand(DevExpress.XtraRichEdit.Commands.RichEditCommandId.PasteSelection));
			}
		}
		void Editor_PopupMenuShowing(object sender, DevExpress.XtraRichEdit.PopupMenuShowingEventArgs e) {
			RichEditClipboardContentMenuBuilder builder = new RichEditClipboardContentMenuBuilder(Editor, new DevExpress.XtraRichEdit.Menu.WinFormsRichEditMenuBuilderUIFactory());
			e.Menu = (DevExpress.XtraRichEdit.Menu.RichEditPopupMenu)builder.CreatePopupMenu();
		}
		protected virtual FilterControl CreateTreeControl() { return new FilterControl(); }
		protected virtual void PrepareTreeControl(Control parent) {
			Tree.Parent = parent;
			Tree.Dock = DockStyle.Fill;
			Tree.BorderStyle = BorderStyles.NoBorder;
			Tree.FilterChanged += new FilterChangedEventHandler(Tree_FilterChanged);
		}
		void Tree_FilterChanged(object sender, FilterChangedEventArgs e) {
			if(e.Action == FilterChangedAction.RebuildWholeTree) 
				return;
			EditorText = Tree.FilterString;
			FilterEditorTextChangeHandler();
			originalFilterStringIsDirty = true;
		}
		protected virtual void SelectedPageChanged() {
			ActiveView = Tab.SelectedTabPage == TreePage ? FilterEditorActiveView.Visual : FilterEditorActiveView.Text;
		}
		protected internal virtual void FilterEditorTextChangeHandler() {
			if(this.silentEditorChangedCounter > 0) {
				this.silentEditorChangedCounter--;
				return;
			}
			IsModified = true;
			originalFilterStringIsDirty = true;
			bool isFilterStringValid, canBeDisplayedByTree;
			CanShowTreePageOnEditorTextChanged(out isFilterStringValid, out canBeDisplayedByTree, EditorText);
			if (PreviousTokenRange!= null) {
				if (PreviousTokenRange.Start != GetPreviousTokenRange().Start || PreviousTokenRange.Length == 0 && GetTokenRange().Length == 0) {
					PreviousTokenRange = null;
				}
			}
			if (this.ActiveView == FilterEditorActiveView.Text)
				RaiseFilterTextChanged(new FilterTextChangedEventArgs(FilterString, isFilterStringValid, canBeDisplayedByTree));
		}
		bool CanShowTreePageOnEditorTextChanged(out bool isFilterStringValid, out bool canBeDisplayedByTree, string editorText) {
			isFilterStringValid = false;
			canBeDisplayedByTree = false;
			bool hasTreePageBeEnabled = ViewMode == FilterEditorViewMode.Visual;
			try {
				GetCriteriaFromEditorText(out isFilterStringValid, out canBeDisplayedByTree, editorText);
				TreePage.PageEnabled = canBeDisplayedByTree || hasTreePageBeEnabled;
			} catch {
				TreePage.PageEnabled = hasTreePageBeEnabled;
			}
			if(!TreePage.PageEnabled) {
				ActiveView = FilterEditorActiveView.Text;
			}
			return TreePage.PageEnabled;
		}
		CriteriaOperator GetCriteriaFromEditorText(out bool isFilterStringValid, out bool canBeDisplayedByTree) {
			return GetCriteriaFromEditorText(out isFilterStringValid, out canBeDisplayedByTree, EditorText);
		}
		CriteriaOperator GetCriteriaFromEditorText(out bool isFilterStringValid, out bool canBeDisplayedByTree, string editorText) {
			isFilterStringValid = canBeDisplayedByTree = false;
			CriteriaOperator criteria = editorTextConverter.ConvertFromEditor(editorText);
			if(ReferenceEquals(criteria, null) && !String.IsNullOrEmpty(editorText)) {
				isFilterStringValid = false;
				canBeDisplayedByTree = false;
				return null;
			}
			isFilterStringValid = IsFilterStringValid(criteria);
			canBeDisplayedByTree = CanBeDisplayedByTree(criteria);
			return criteria;
		}
		protected virtual void FilterEditorParentChanged() {
			if (RootParent != null) {
				RootParent.Move -= new EventHandler(RootParent_Move);
			}
			RootParent = GetRootParent();
			RootParent.Move += new EventHandler(RootParent_Move);
		}
		protected virtual void UpdateTab() {
			Tab.ShowTabHeader = IsMultiView ? DefaultBoolean.True : DefaultBoolean.False;
			if(!IsMultiView) return;
			if(ViewMode == FilterEditorViewMode.TextAndVisual) {
				Tab.TabPages.Move(0, EditorPage);
			} else {
				Tab.TabPages.Move(0, TreePage);
			}
			Tab.TabIndex = 0;
		}
		#region Keyboard and mouse event handlers
		protected internal virtual void EditorKeyDownHandler(KeyEventArgs e) {
			if(!IsNavigationKey(e.KeyCode) && IsTookenConstantLookup()) {
				IntelliSenseWindow.Hide();
				if(GetTokenRange().End.ToInt() <= EditorCaretPosition) return;
				if(e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back) {
					if(!IsSelectionOutOfCurrentToken()) {
						ReplaceConstantToken(string.Empty);
					} else return;
				} else {
					ShowIntelliSenseWindow();
				}
				e.SuppressKeyPress = true;
				return;
			}
			if (e.Control && e.KeyCode == Keys.Space) {
				ForcedTokenReplacement();
				e.SuppressKeyPress = true;
				return;
			}
			if (!IntelliSenseWindow.IsShowing) return;
			switch (e.KeyCode) {
				case Keys.Escape:
					PreviousTokenRange = GetPreviousTokenRange();
					IntelliSenseWindow.Hide();
					break;
				case Keys.Up:
				case Keys.PageUp:
				case Keys.Home:
					IntelliSenseWindow.MoveUp(e.KeyCode);
					e.Handled = true;
					break;
				case Keys.Down:
				case Keys.PageDown:
				case Keys.End:
					IntelliSenseWindow.MoveDown(e.KeyCode);
					e.Handled = true;
					break;
				case Keys.Enter:
				case Keys.Space:
				case Keys.Tab:
					if (IntelliSenseWindow.SelectedIndex != -1) {
						ReplaceToken(IntelliSenseWindow.SelectedItem);
					}
					IntelliSenseWindow.Hide();
					e.Handled = true;
					break;
				case Keys.Left:
				case Keys.Back:
				case Keys.Right:
				case Keys.Delete:
					if (CursorOutOfRange(e.KeyCode != Keys.Right)) {
						IntelliSenseWindow.Hide();
					}
					break;
			}
		}
		bool IsSelectionOutOfCurrentToken() {
			DocumentRange range = GetTokenRange();
			DocumentRange selection = Editor.Document.Selection;
			if(range == null || selection == null || range.Length == 0 || selection.Length == 0) return false;
			int rangeStart = range.Start.ToInt();
			int rangeEnd = range.End.ToInt();
			int selStart = selection.Start.ToInt();
			int selEnd = selection.End.ToInt();
			return !(selStart >= rangeStart && selEnd <= rangeEnd);
		}
		bool IsNavigationKey(Keys keys) {
			return ((int)keys < (int)'0' || ((int)keys >= (int)Keys.F1 && (int)keys <=  (int)Keys.F24))
				&& (keys != Keys.Space) && (keys != Keys.Delete) && (keys != Keys.Back);
		}
		protected internal virtual void EditorKeyPressHandler(KeyPressEventArgs e) {
			if(CanShowIntelliSenseWindow()) {
				if (char.IsSeparator(e.KeyChar) && GetTokenType() != TokenType.Property) ShowIntelliSenseWindow();
				if ((char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == '[')) ShowIntelliSenseWindow();
			}
			if (e.KeyChar == '.') {
				if (IntelliSenseWindow.IsShowing && GetTokenType() == TokenType.Property) {
					IntelliSenseWindow.Hide();
					if (IntelliSenseWindow.SelectedIndex > -1) {
						ReplaceToken(IntelliSenseWindow.SelectedItem);
						Editor.BeginUpdate();
						Editor.Document.InsertText(Editor.Document.CreatePosition(EditorCaretPosition), ".");
						Editor.EndUpdate();
					}
				}
				if (GetTokenType() == TokenType.Property || GetListFilterColumnFromPosition(EditorCaretPosition) != null) {
					ShowIntelliSenseWindow();
				}
			}
		}
		protected internal virtual void EditorKeyUpHandler(KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Up:
				case Keys.Down:
				case Keys.PageUp:
				case Keys.PageDown:
				case Keys.Home:
				case Keys.End:
				case Keys.Enter:
				case Keys.Space:
					break;
				default:
					IntelliSenseWindow.SelectedIndex = GetSelectedItemPositionForIntelliSense();
					break;
			}
		}
		protected internal virtual void EditorMouseDownHandler(MouseEventArgs e) {
			if (IntelliSenseWindow.IsShowing) IntelliSenseWindow.Hide();
		}
		protected void EditorMouseDblClkHandler(MouseEventArgs e) {
			Point location = GetIntelliSenseWindowPosition(e);
			ShowIntelliSenseWindow(location);
			IntelliSenseWindow.SelectedIndex = GetSelectedItemPositionForIntelliSense();
		}
		protected internal virtual void EditorMouseWheelHandler(MouseEventArgs e) {
			if (e.Button == MouseButtons.None) {
				IntelliSenseWindow.ScrollItemsList(e.Delta);
			}
		}
		protected internal virtual void EditorLostFocusHandler() {
			if (IntelliSenseWindow.IsShowing && !IntelliSenseWindow.IsFocused  && !Editor.Focused) {
				IntelliSenseWindow.Hide();
			}
		}
		protected internal virtual void IntelliSenseWindowItemsListGotFocus() {
			Editor.Focus();
		}
		protected internal virtual void IntelliSenseWindowItemsListDblClk() {
			if (IntelliSenseWindow.SelectedIndex != -1) {
				ReplaceToken(IntelliSenseWindow.SelectedItem);
				IntelliSenseWindow.Hide();
			}
		}
		protected virtual string SerializeFilterValue(FilterColumn column, object value) {
			if(column != null) {
				value = DevExpress.Data.Helpers.FilterHelper.CorrectFilterValueType(column.ColumnType, value);
			}
			return new OperandValue(value).ToString();
		}
		protected internal virtual void IntelliSenseWindowEditorClosed() {
			object value = IntelliSenseWindow.SelectedValue;
			if(value != null) {
				FilterColumn column = GetIntelliSenseFilterColumn();
				string displayText = string.Empty;
				string filterText = SerializeFilterValue(column, value);
				if(column != null && IsFilterColumnLookup(column)) {
					displayText = ConvertConstantValueToDisplayText(column, value);
					editorTextConverter.AddKnownItem(new ConstantConverterItem(column, displayText, filterText, value));
				}
				DocumentRange range = GetTokenRange();
				int curPosition = range != null && range.Length > 0 ? range.Start.ToInt() : Editor.Document.CaretPosition.ToInt();
				ReplaceConstantToken(!string.IsNullOrEmpty(displayText) ? displayText : filterText);
				if(curPosition >= 0 && curPosition > Editor.Document.CaretPosition.ToInt()) {
					MoveCaretToPosition(curPosition);
				}
				range = GetTokenRange();
				if(range != null) {
					MoveCaretToPosition(range.End.ToInt());
				}
			}
		}
		protected internal virtual void RootParentControlMoveHandler() {
			IntelliSenseWindow.Hide();
		}
		#endregion
		#region Constant Conversion
		private delegate object Predicate();
		private object WrapParseCriteria(Predicate func) {
			CreateCriteriaParseContextEventArgs args = new CreateCriteriaParseContextEventArgs();
			RaiseCreateCriteriaParseContext(args);
			try {
				return func();
			}
			finally {
				if(args.Context != null) {
					args.Context.Dispose();
				}
			}
		}
		internal class ConstantConverterItem {
			FilterColumn filterColumn;
			string displayText;
			string filterText;
			object value;
			public ConstantConverterItem(FilterColumn filterColumn, string displayText, string filterText, object value) {
				this.filterColumn = filterColumn;
				this.displayText = displayText.Trim('\'');
				this.filterText = filterText.Trim('\'');
				this.value = value;
			}
			public FilterColumn FilterColumn { get { return filterColumn; } }
			public string DisplayText { get { return displayText; } }
			public string FilterText { get { return filterText; } }
			public object Value { get { return value; } }
		}
		protected virtual object DeserializeFilterValue(FilterColumn column, string constValue, object trialValue) {
			return trialValue;
		}
		protected virtual string ConvertConstantValueToDisplayText(FilterColumn column, string constValue, object value) {
			return ConvertConstantValueToDisplayText(column, value);
		}
		string ConvertConstantValueToDisplayText(FilterColumn column, object value) {
			if(value == null) return string.Empty;
			string strValue = column.ColumnEditor.GetDisplayText(value);
			if(!string.IsNullOrEmpty(strValue)) {
				strValue = strValue.Replace('\'', ' ');
			}
			return '\'' + strValue + '\'';
		}
		#endregion
		DevExpress.XtraRichEdit.Model.DocumentLogPosition Max(
			DevExpress.XtraRichEdit.Model.DocumentLogPosition a,
			DevExpress.XtraRichEdit.Model.DocumentLogPosition b)
		{
			return a > b ? b : a;
		}
		protected Point GetIntelliSenseWindowPosition() {
			var start = Editor.ActiveView.DocumentModel.Selection.Start;
			var end = Editor.ActiveView.DocumentModel.Selection.End;
			var max = Max(start, end);
			Editor.ActiveView.DocumentModel.Selection.End = max;
			Editor.ActiveView.DocumentModel.Selection.Start = max;
			Rectangle bounds = Editor.ActiveView.GetCursorBounds();
			return Editor.PointToScreen(new Point(bounds.Left, bounds.Bottom));
		}
		protected Point GetIntelliSenseWindowPosition(MouseEventArgs e) {
			MouseEventArgs convertedEventArgs = ConvertMouseEventArgs(e);
			Point pt = new Point(convertedEventArgs.X, convertedEventArgs.Y);
			Rectangle bounds = Editor.ActiveView.HitTestByPhysicalPoint(pt, DocumentLayoutDetailsLevel.Row);
			return bounds.IsEmpty ? new Point() : Editor.PointToScreen(new Point(e.X, bounds.Bottom));
		}
		protected virtual void ShowIntelliSenseWindow() {
			ShowIntelliSenseWindow(GetIntelliSenseWindowPosition());
		}
		protected virtual void ShowIntelliSenseWindow(Point location) {
			if (location.IsEmpty) return;
			List<string> items = GetIntelliSenseItems();
			FilterColumn  filterColumn = GetIntelliSenseFilterColumn();
			if(items.Count > 0 || filterColumn != null) {
				object propertiesValue = GetConstTokenEditValue(filterColumn);
				RepositoryItem properties = filterColumn != null ? filterColumn.ColumnEditor : null;
				IntelliSenseWindow.Show(properties, propertiesValue, items, location);
			}
		}
		protected virtual object GetConstTokenEditValue(FilterColumn filterColumn) {
			object result = CreateLexerHelper().GetTokenValue(EditorCaretPosition);
			if(result != null && IsFilterColumnLookup(filterColumn)) {
				ConstantConverterItem converterItem = editorTextConverter.GetConstantConverterItem(filterColumn, result.ToString());
				if(converterItem != null) return converterItem.Value;
			}
			return result;
		}
		protected virtual bool CanShowIntelliSenseWindow() {
			if (IntelliSenseWindow.IsShowing) return false;
			if (PreviousTokenRange == null) return true;
			return false;
		}
		protected virtual void ReplaceConstantToken(string text) {
			DocumentRange tokenRange = GetTokenRange();
			DocumentRange prevTokenRange = GetPreviousTokenRange();
			int tokenRangeStart = tokenRange.Start.ToInt();
			int rangePosition = EditorCaretPosition - tokenRangeStart;
			string oldText = Editor.Document.GetText(tokenRange);
			string prevText = prevTokenRange != null ? Editor.Document.GetText(prevTokenRange) : string.Empty;
			text = GetNewConstantToken(prevText, oldText, text, ref rangePosition);
			InsertTextToRange(tokenRange, text);
			if(rangePosition < 0) {
				DocumentRange nextTokenRange = GetNextTokenRange();
				int newPosition = nextTokenRange != null ? nextTokenRange.Start.ToInt() : tokenRange.End.ToInt();
				MoveCaretToPosition(newPosition > 0 ? newPosition : FilterString.Length + 1);
			} else {
				MoveCaretToPosition(tokenRangeStart + rangePosition);
			}
		}
		protected string GetNewConstantToken(string prevText, string oldText, string newText, ref int position) {
			if(!oldText.Trim().StartsWith("(")) {
				if(oldText.Contains(")")) {
					position = newText.Length + 1;
					return newText + ')';
				}
				if(oldText.Contains(",")) {
					position = newText.Length + 1;
					prevText = prevText.Trim();
					bool prevTextHasNotValue = !string.IsNullOrEmpty(prevText) && prevText.Length == 1 && (prevText[0] == '(' || prevText[0] == ',');
					return prevTextHasNotValue ? newText + ',' : ',' + newText;
				}
				position = -1;
				return newText;
			}
			int firstIndex = 0;
			while(oldText[firstIndex] != '(') firstIndex ++;
			if(position <= firstIndex) position = firstIndex + 1;
			if(position >= oldText.Length) {
				position = oldText.Length - 1;
				if(oldText[position] == '(') {
					oldText += ' ';
					position++;
				}
			}
			int startIndex = position;
			while(startIndex > firstIndex + 1 && oldText[startIndex - 1] != ',') {
				startIndex--;
			}
			int endIndex = position;
			while(endIndex < oldText.Length - 1 && oldText[endIndex + 1] != ',' && oldText[endIndex + 1] != ')') {
				endIndex++;
			}
			int count = oldText[endIndex] == ',' || oldText[endIndex] == ')' ? 0 : 1;
			count += endIndex - startIndex;
			if(count > 0) {
				oldText = oldText.Remove(startIndex, count);
			}
			newText = oldText.Insert(startIndex, newText);
			position = startIndex + newText.Length;
			return newText;
		}
		protected virtual void ReplaceToken(string text) {
			if (string.IsNullOrEmpty(text)) return;
			text = ConvertIntelliSenseTextToToken(text);
			DocumentRange tokenRange;
			if(GetNextTokenType() == FindNextTokenTypeForIntelliSense()) {
				tokenRange = GetRangeToReplaceRightToken(text);
			} else {
				tokenRange = GetRangeToReplaceLeftToken(text);
			}
			string oldText = Editor.Document.GetText(tokenRange);
			text = GetReplacementText(text, oldText, GetPreviousTokenText());
			int caretPosition = GetNewCaretPosition(tokenRange, ref text);
			if(caretPosition < 0 && oldText == "]" && IsInsideConditionOfListProperty()) {
				caretPosition = EditorCaretPosition + text.Length - 1;
			}
			InsertTextToRange(tokenRange, text);
			if(caretPosition >= 0) {
				MoveCaretToPosition(caretPosition);
			}
		}
		string GetReplacementText(string newStr, string oldStr, string oldPrevStr) {
			bool inListCondition = IsInsideConditionOfListProperty();
			if(oldStr == "(" || oldStr == "." || oldStr == "[" && inListCondition) return oldStr + newStr;
			if(inListCondition && oldStr == "]") return newStr + oldStr;
			return string.Format(newStr, oldPrevStr);
		}
		protected virtual void ForcedTokenReplacement() {
			if (!IntelliSenseWindow.IsShowing) {
				List<string> items = GetIntelliSenseItems();
				string tokenText = GetTokenText().Trim("[]".ToCharArray());
				tokenText = tokenText.Substring(tokenText.LastIndexOf('.') + 1);
				int position = GetSelectedItemPosition(items, tokenText);
				int similarItemsCount = 0;
				foreach(string item in items) {
					if (item.StartsWith(tokenText, System.StringComparison.OrdinalIgnoreCase)) similarItemsCount++;
				}
				if(position != -1 && similarItemsCount == 1 && items[position] != tokenText) {
					ReplaceToken(items[position]);
				} else {
					ShowIntelliSenseWindow();
					IntelliSenseWindow.SelectedIndex = GetSelectedItemPositionForIntelliSense();
				}
			}
		}
		protected DocumentRange GetRangeToReplaceLeftToken(string text) {
			DocumentRange currentTokenRange = GetTokenRange();
			if (text.Contains("{0}")) {
				DocumentRange prevTokenRange = GetPreviousTokenRange();
				currentTokenRange = Editor.Document.CreateRange(prevTokenRange.Start, EditorCaretPosition - prevTokenRange.Start.ToInt());
			}
			return currentTokenRange;
		}
		protected DocumentRange GetRangeToReplaceRightToken(string text) {
			if(EditorCaretPosition >= EditorText.Length) return GetRangeToReplaceLeftToken(text);
			int startPosition = GetTokenRange().Length != 0 ? GetTokenRange().Start.ToInt() : EditorCaretPosition;
			DocumentRange currentTokenRange = Editor.Document.CreateRange(startPosition, GetNextTokenRange().End.ToInt() - startPosition);
			if (text.Contains("{0}")) {
				DocumentRange prevTokenRange = GetPreviousTokenRange();
				currentTokenRange = Editor.Document.CreateRange(prevTokenRange.Start, GetNextTokenRange().End.ToInt() - prevTokenRange.Start.ToInt());
			}
			return currentTokenRange;
		}
		protected virtual string ConvertIntelliSenseTextToToken(string text) {
			string result = text;
			if (TokenTextConverter != null && TokenTextConverter.ContainsKey(text)) {
				if (!string.IsNullOrEmpty(TokenTextConverter[text])) {
					result = TokenTextConverter[text];
				}
			}
			return result;
		}
		protected virtual void InsertTextToRange(DocumentRange range, string text) {
			Editor.BeginUpdate();
			if (range.End.ToInt() >= EditorCaretPosition) {
				Editor.Document.Delete(range);
				Editor.Document.InsertText(range.Start, text);
			} else {
				Editor.Document.InsertText(Editor.Document.CreatePosition(EditorCaretPosition), text);
			}
			Editor.EndUpdate();
		}
		protected virtual int GetSelectedItemPositionForIntelliSense() {
			return GetSelectedItemPosition(IntelliSenseWindow.Items, GetTokenText());
		}
		protected virtual int GetSelectedItemPosition(List<string> items, string tokenText) {
			tokenText = tokenText.Trim("[]".ToCharArray());
			int index = tokenText.LastIndexOf(".");
			if (index > 0) {
				tokenText = tokenText.Substring(index + 1);
			}
			if (string.IsNullOrEmpty(tokenText) || items == null) return -1;
			string text = string.Empty;
			foreach (string item in items) {
				if (item.StartsWith(tokenText, System.StringComparison.OrdinalIgnoreCase)) {
					text = item;
					break;
				}
			}
			if (!string.IsNullOrEmpty(text)) return items.IndexOf(text);
			return -1;
		}
		protected bool CursorOutOfRange(bool movingLeft) {
			DocumentRange range = GetTokenRange();
			if (range.Start.ToInt() == 0 && range.End.ToInt() == 0 && EditorCaretPosition == 0) return true;
			if (range.Start.ToInt() < EditorCaretPosition && EditorCaretPosition < range.End.ToInt()) return false;
			if (range.Start.ToInt() == EditorCaretPosition && !movingLeft) return false;
			if (range.End.ToInt() == EditorCaretPosition && movingLeft) return false;
			return true;
		}
		#region IntelliSense items
		protected virtual List<string> GetIntelliSenseItems() {
			List<string> list = new List<string>();
			TokenType tokenType = FindNextTokenTypeForIntelliSense();
			switch (tokenType) {
				case TokenType.Aggregate:
				case TokenType.CompareOperator:
				case TokenType.Group:
				case TokenType.Property:
					TokenTextConverter = FillIntelliSenseItems(tokenType);
					if(TokenTextConverter != null) {
						list.AddRange(TokenTextConverter.Keys);
					}
					break;
			}
			if(tokenType == TokenType.Property && SortFilterColumns)
				list.Sort();
			return list;
		}
		protected bool IsTookenConstantLookup(CriteriaLexerToken token) {
			if(token.TokenType != TokenType.Constant) return false;
			return IsTookenConstantLookup(token.Position);
		}
		protected bool IsTookenConstantLookup() {
			if(GetTokenType() != TokenType.Constant) return false;
			return IsTookenConstantLookup(EditorCaretPosition);
		}
		bool IsTookenConstantLookup(int position) {
			return IsFilterColumnLookup(GetFilterColumnByTooken(position));
		}
		bool IsFilterColumnLookup(FilterColumn column) {
			return column != null && column.ColumnEditor != null && column.ColumnEditor.IsFilterLookUp;
		}
		protected FilterColumn GetIntelliSenseFilterColumn() {
			FilterColumn column = GetFilterColumnByTooken(EditorCaretPosition);
			if(column != null && column.ColumnEditor != null && column.ColumnEditor.GetType() != typeof(RepositoryItemTextEdit)) return column;
			return null;
		}
		protected FilterColumn GetFilterColumnByTooken(int position) {
			TokenType tokenType = FindNextTokenTypeForIntelliSense(position);
			if(tokenType == TokenType.Constant) {
				return GetFilterColumnFromPosition(position);
			}
			return null;
		}
		protected virtual FilterColumn GetFilterColumnByCaption(string caption) {
			if(string.IsNullOrEmpty(caption)) return null;
			caption = caption.Trim("[]".ToCharArray());
			FilterColumn filterColumn = FilterColumns.GetFilterColumnByCaption(caption);
			if (filterColumn != null) return filterColumn;
			int index = caption.LastIndexOf('.');
			if (index == -1) return null;
			return FilterColumns.GetFilterColumnByCaption(caption.Substring(0, index));
		}
		protected virtual string GetTokenByClauseType(ClauseType clause) {
			switch (clause) {
				case ClauseType.AnyOf: return "In (?)";
				case ClauseType.BeginsWith: return "Like '?%'";
				case ClauseType.Between: return "Between (?,)";
				case ClauseType.Contains: return "Like '%?%'";
				case ClauseType.DoesNotContain: return "Not {0} Like '%?%'";
				case ClauseType.DoesNotEqual: return "<>";
				case ClauseType.EndsWith: return "Like '%?'";
				case ClauseType.Equals: return "=";
				case ClauseType.Greater: return ">";
				case ClauseType.GreaterOrEqual: return ">=";
				case ClauseType.Less: return "<";
				case ClauseType.LessOrEqual: return "<=";
				case ClauseType.Like: return "Like";
				case ClauseType.NoneOf: return "Not {0} In (?)";
				case ClauseType.NotBetween: return "Not {0} Between (?,)";
				case ClauseType.NotLike: return "Not {0} Like";
				case ClauseType.IsNotNull: return "Is Not Null";
				case ClauseType.IsNull: return "Is Null";
				case ClauseType.IsBeyondThisYear: return "IsOutlookIntervalBeyondThisYear({0})";
				case ClauseType.IsLaterThisYear: return "IsOutlookIntervalLaterThisYear({0})";
				case ClauseType.IsLaterThisMonth: return "IsOutlookIntervalLaterThisMonth({0})";
				case ClauseType.IsNextWeek: return "IsOutlookIntervalNextWeek({0})";
				case ClauseType.IsLaterThisWeek: return "IsOutlookIntervalLaterThisWeek({0})";
				case ClauseType.IsTomorrow: return "IsOutlookIntervalTomorrow({0})";
				case ClauseType.IsToday: return "IsOutlookIntervalToday({0})";
				case ClauseType.IsYesterday: return "IsOutlookIntervalYesterday({0})";
				case ClauseType.IsEarlierThisWeek: return "IsOutlookIntervalEarlierThisWeek({0})";
				case ClauseType.IsLastWeek: return "IsOutlookIntervalLastWeek({0})";
				case ClauseType.IsEarlierThisMonth: return "IsOutlookIntervalEarlierThisMonth({0})";
				case ClauseType.IsEarlierThisYear: return "IsOutlookIntervalEarlierThisYear({0})";
				case ClauseType.IsPriorThisYear: return "IsOutlookIntervalPriorThisYear({0})";
			}
			return string.Empty;
		}
		protected virtual Dictionary<string, string> FillIntelliSenseItems(TokenType type) {
			Dictionary<string, string> items = new Dictionary<string, string>();
			switch (type) {
				case TokenType.CompareOperator:
					FilterColumn column = GetFilterColumnByCaption(GetPreviousTokenText());
					foreach(ClauseType clause in System.Enum.GetValues(typeof(ClauseType))) {
						if (column == null || !Model.IsValidClause(column, clause))
							continue;
						items.Add(OperationHelper.GetMenuStringByType(clause), GetTokenByClauseType(clause));
					}
					break;
				case TokenType.Group:
					items.Add(Localizer.Active.GetLocalizedString(StringId.FilterGroupOr), "Or");
					items.Add(Localizer.Active.GetLocalizedString(StringId.FilterGroupAnd), "And");
					break;
				case TokenType.Aggregate:
					items.Add("Average", "Avg(?)");
					items.Add("Count", "Count()");
					items.Add("Maximum", "Max(?)");
					items.Add("Minimum", "Min(?)");
					items.Add("Summary", "Sum(?)");
					break;
				case TokenType.Property:
					CriteriaLexerTokenHelper helper = CreateLexerHelper();
					helper.UpdateListProperty(true);
					CriteriaLexerToken token = helper.FindToken(EditorCaretPosition);
					if(token != null) {
						Dictionary<string, string> childProperties = null;
						if (string.IsNullOrEmpty(token.ListName)) {
							string propertyCaption = helper.GetTokenText(EditorCaretPosition);
							childProperties = GetChildPropertiesByCaption(propertyCaption);
						}
						else {
							childProperties = GetChildPropertiesByFilterColumn(FilterColumns[token.ListName]);
						}
						if (childProperties != null)
							return childProperties;
					}
					FilterColumn listColumn = GetListFilterColumnFromPosition(EditorCaretPosition);
					if(listColumn != null) {
						return GetChildPropertiesByFilterColumn(listColumn);
					}
					foreach (FilterColumn filterColumn in FilterColumns) {
						if(!items.ContainsKey(filterColumn.ColumnCaption)) {
							items.Add(filterColumn.ColumnCaption, GetIntelliSenseTokenFromFilterColumn(filterColumn));
						}
					}
					break;
			}
			return items;
		}
		Dictionary<string, string> GetChildPropertiesByCaption(string propertyCaption) {
			int index = propertyCaption.LastIndexOf('.');
			if(index == -1) return null;
			return GetChildPropertiesByFilterColumn(GetFilterColumnByCaption(propertyCaption.Substring(0, index)));
		}
		Dictionary<string, string> GetChildPropertiesByFilterColumn(FilterColumn  filterColumn) {
			if(filterColumn != null && filterColumn.HasChildren) {
				Dictionary<string, string> items = new Dictionary<string, string>();
				foreach(FilterColumn childFilterColumn in filterColumn.Children) {
					if(!items.ContainsKey(childFilterColumn.ColumnCaption)) {
						items.Add(childFilterColumn.ColumnCaption, GetIntelliSenseTokenFromFilterColumn(childFilterColumn));
					}
				}
				return items;
			}
			return null;
		}
		protected virtual string GetIntelliSenseTokenFromFilterColumn(FilterColumn filterColumn) {
			string caption = string.Empty;
			FilterColumn column = filterColumn;
			while ((column = (FilterColumn)column.Parent) != null && !column.IsList) {
				caption = column.ColumnCaption + "." + caption;
			}
			string result = string.Format(filterColumn.HasChildren && !filterColumn.IsList ? "[{0}" : "[{0}]", caption + filterColumn.ColumnCaption);
			if(filterColumn.IsList) {
				result += "[]";
			}
			return result;
		}
		#endregion
		protected virtual int GetNewCaretPosition(DocumentRange replacingRange, ref string text) {
			int offset = text.IndexOfAny("?".ToCharArray());
			if (offset == -1) return -1;
			text = text.Remove(offset, 1);
			return replacingRange.End.ToInt() < EditorCaretPosition ? EditorCaretPosition + offset : replacingRange.Start.ToInt() + offset;
		}
		protected virtual Control GetRootParent() {
			Control parentControl = this;
			while (parentControl.Parent != null) {
				parentControl = parentControl.Parent;
			}
			return parentControl;
		}
		protected virtual MouseEventArgs ConvertMouseEventArgs(MouseEventArgs screenMouseEventArgs) {
			DevExpress.Office.Layout.DocumentLayoutUnitConverter unitConverter = Editor.DocumentModel.LayoutUnitConverter;
			int x = unitConverter.PixelsToLayoutUnits(screenMouseEventArgs.X, Editor.DpiX) - Editor.ViewBounds.Left;
			int y = unitConverter.PixelsToLayoutUnits(screenMouseEventArgs.Y, Editor.DpiY) - Editor.ViewBounds.Top;
			return new MouseEventArgs(screenMouseEventArgs.Button, screenMouseEventArgs.Clicks, x, y, screenMouseEventArgs.Delta);
		}
		protected bool IsFilterStringValid(CriteriaOperator criteria) {
			ErrorsEvaluatorCriteriaValidator criteriaValidator = new ErrorsEvaluatorCriteriaEditorValidator(GetBoundProperties(),
				OnErrorsEvaluatorCriteriaEditorValidatorGetCorrectValue, OnErrorsEvaluatorCriteriaEditorValidatorGetValidatatedValue);
			criteriaValidator.Validate(criteria);
			return criteriaValidator.Count == 0;
		}
		protected List<IBoundProperty> GetBoundProperties() {
			List<IBoundProperty> list = new List<IBoundProperty>();
			foreach(FilterColumn column in FilterColumns) {
				list.Add(column);
			}
			return list;
		}
		bool OnErrorsEvaluatorCriteriaEditorValidatorGetCorrectValue(FilterColumn filterColumn, string curValue, out object newValue) {
			newValue = DeserializeFilterValue(filterColumn, curValue, curValue);
			return newValue != null && !(newValue is string);
		}
		bool OnErrorsEvaluatorCriteriaEditorValidatorGetValidatatedValue(FilterColumn filterColumn, string curValue, out object newValue) {
			newValue = curValue;
			if(!string.IsNullOrEmpty(curValue) && curValue.StartsWith("@")) {
				if(GetFilterValueByParameterName(curValue.Remove(0, 1), out newValue))
					return true;
			}
			return false;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
		protected virtual bool GetFilterValueByParameterName(string customParameterName, out object value) {
			value = null;
			return false;
		}
		protected virtual bool CanBeDisplayedByTree(CriteriaOperator criteria) {
			List<CriteriaOperator> skippedCriteriaOperator = new List<CriteriaOperator>();
			CriteriaToTreeProcessor.GetTree(new FilterControlNodesFactory(Tree.Model), criteria, skippedCriteriaOperator);
			if(skippedCriteriaOperator.Count != 0) {
				return false;
			}
			return true;
		}
		void MoveCaretToPosition(int caretPosition) {
			if (EditorCaretPosition == caretPosition) return;
			Editor.Document.CaretPosition = Editor.Document.CreatePosition(caretPosition);
		}
		#region Lexer wrapper
		CriteriaLexerTokenHelper CreateLexerHelper() {
			return (CriteriaLexerTokenHelper)WrapParseCriteria(delegate() {
				return new CriteriaLexerTokenHelper(EditorText);
			});
		}
		TokenType FindNextTokenTypeForIntelliSense() {
			return FindNextTokenTypeForIntelliSense(EditorCaretPosition);
		}
		TokenType FindNextTokenTypeForIntelliSense(int position) {
			if(string.IsNullOrEmpty(EditorText)) return TokenType.Property;
			return CreateLexerHelper().GetNextTokenType(position);
		}
		TokenType GetTokenType() {
			return CreateLexerHelper().GetTokenType(EditorCaretPosition);
		}
		TokenType GetNextTokenType() {
			return CreateLexerHelper().GetNeighborTokenType(EditorCaretPosition, false);
		}
		string GetTokenText() {
			return CreateLexerHelper().GetTokenText(EditorCaretPosition);
		}
		string GetPreviousTokenText() {
			return Editor.Document.GetText(GetPreviousTokenRange());
		}
		bool IsInsideConditionOfListProperty() {
			return IsInsideConditionOfListProperty(EditorCaretPosition);
		}
		bool IsInsideConditionOfListProperty(int position) {
			FilterColumn column = GetFilterColumnFromPosition(position);
			return column != null && (column.HasChildren || column.Parent != null);
		}
		FilterColumn GetFilterColumnFromPosition(int position) {
			return GetFilterColumnFromPosition(position, false);
		}
		FilterColumn GetListFilterColumnFromPosition(int position) {
			return GetFilterColumnFromPosition(position, true);
		}
		FilterColumn GetFilterColumnFromPosition(int position, bool isList) {
			CriteriaLexerTokenHelper helper = CreateLexerHelper();
			helper.UpdateListProperty(true);
			while(position > 0) {
				CriteriaLexerToken token = helper.FindToken(position);
				if(token == null) {
					position--;
					continue;
				}
				if(token.TokenType == TokenType.Group && !isList) return null;
				if(isList && !string.IsNullOrEmpty(token.ListName)) {
					FilterColumn result = FilterColumns[token.ListName];
					if(result != null)
						return result;
				}
				if(token.TokenType == TokenType.Property) {
					string fullName = string.IsNullOrEmpty(token.ListName) ? "" : token.ListName + ".";
					fullName += ((OperandProperty)token.CriteriaOperator).PropertyName;
					FilterColumn result = null;
					FilterColumn listColumn = null;
					if(!isList && token.Position > 0) {
						listColumn = GetFilterColumnFromPosition(token.Position - 1, true);
					}
					if (listColumn != null) {
						result = listColumn.GetChildByCaption(fullName);
					}
					if (result == null) {
						result = GetFilterColumnByCaption(fullName);
					}
					if(result != null) return !isList || result.IsList ? result : null;
				}
				position = token.Position - 1;
			}
			return null;
		}
		DocumentRange GetTokenRange() {
			int position, length;
			CreateLexerHelper().GetTokenPositionLength(EditorCaretPosition, out position, out length);
			return Editor.Document.CreateRange(position, length);
		}
		DocumentRange GetPreviousTokenRange() {
			int position, length;
			CreateLexerHelper().GetNeighborTokenRange(EditorCaretPosition, true, out position, out length);
			return Editor.Document.CreateRange(position, length);
		}
		DocumentRange GetNextTokenRange() {
			int position, length;
			CreateLexerHelper().GetNeighborTokenRange(EditorCaretPosition, false, out position, out length);
			return Editor.Document.CreateRange(position, length);
		}
		#endregion
		void Tab_SelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			SelectedPageChanged();
		}
		void ChangeParagraphLineSpacing() {
			if(Editor.Document.Paragraphs.Count == 0) return;
			this.silentEditorChangedCounter++;
			Editor.Document.Paragraphs[0].LineSpacingType = ParagraphLineSpacing.Sesquialteral;
		}
		void Editor_DocumentLoaded(object sender, EventArgs e) {
			ChangeParagraphLineSpacing();
		}
		void Editor_EmptyDocumentCreated(object sender, EventArgs e) {
			ChangeParagraphLineSpacing();
		}
		void Editor_ContentChanged(object sender, System.EventArgs e) {
			FilterEditorTextChangeHandler();
		}
		void Editor_KeyDown(object sender, KeyEventArgs e) {
			EditorKeyDownHandler(e);
		}
		void Editor_KeyUp(object sender, KeyEventArgs e) {
			EditorKeyUpHandler(e);
		}
		void Editor_MouseDown(object sender, MouseEventArgs e) {
			EditorMouseDownHandler(e);
		}
		void Editor_MouseDoubleClick(object sender, MouseEventArgs e) {
			EditorMouseDblClkHandler(e);
		}
		void Editor_MouseWheel(object sender, MouseEventArgs e) {
			EditorMouseWheelHandler(e);
		}
		void Window_GotFocus(object sender, System.EventArgs e) {
			IntelliSenseWindowItemsListGotFocus();
		}
		void Window_IntelliSenseListDoubleClick(object sender, System.EventArgs e) {
			IntelliSenseWindowItemsListDblClk();
		}
		void Window_EditorClosed(object sender, EventArgs e) {
			IntelliSenseWindowEditorClosed();
		}
		void Editor_LostFocus(object sender, EventArgs e) {
			EditorLostFocusHandler();
		}
		void RootParent_Move(object sender, EventArgs e) {
			RootParentControlMoveHandler();
		}
		void FilterEditorControl_ParentChanged(object sender, EventArgs e) {
			FilterEditorParentChanged();
		}
		bool IsValidProperty(CriteriaLexerToken token) {
			string propertyName = (token.CriteriaOperator as OperandProperty).PropertyName;
			IBoundProperty property = FilterColumns[propertyName];
			if (property == null) {
				property = FilterColumns.GetFilterColumnByCaption(propertyName);
			}
			if(property == null) {
				FilterColumn listColumn = GetListFilterColumnFromPosition(token.Position);
				if(listColumn != null) {
					property = FilterColumns.GetFilterColumnByCaption(propertyName, listColumn.Children);
				}
			}
			return property != null && !property.IsAggregate;
		}
		#region ISyntaxHighlightService Members
		void ISyntaxHighlightService.ForceExecute() {
			((ISyntaxHighlightService)this).Execute();
		}
		void ISyntaxHighlightService.Execute() {
			List<CriteriaLexerToken> tokens = CreateLexerHelper().TokenList;
			Tree.FilterViewInfo.UpdateAppearanceColors();
			if(tokens.Count == 0) {
				DocumentRange range = Editor.Document.CreateRange(0, 1);
				CharacterProperties props = CreateUpdateCharacters(range);
				Editor.Document.EndUpdateCharacters(props);
				return;
			}
			int lastPosition = 0;
			foreach(CriteriaLexerToken token in tokens) {
				DocumentRange range = Editor.Document.CreateRange(token.Position, token.Length);
				lastPosition = token.PositionEnd;
				CharacterProperties props = CreateUpdateCharacters(range);
				try {
					switch (token.TokenType) {
						case TokenType.Constant:
							props.ForeColor = AppearanceValueColor;
							if(IsTookenConstantLookup(token)) {
								props.BackColor = Color.LightGray;
							}
							break;
						case TokenType.Property:
							if (!IsValidProperty(token)) {
								props.Underline = UnderlineType.Wave;
								props.UnderlineColor = Color.Red;
							}
							props.ForeColor = Tree.FilterViewInfo.FieldNameColor;
							break;
						case TokenType.Group:
							props.ForeColor = Tree.FilterViewInfo.GroupOperatorColor;
							break;
						case TokenType.Aggregate:
							props.ForeColor = Tree.FilterViewInfo.OperatorColor;
							break;
					}
				} catch {
				}
				finally {
					Editor.Document.EndUpdateCharacters(props);
				}
			}
			if(lastPosition < EditorText.Length) {
				DocumentRange range = Editor.Document.CreateRange(lastPosition, EditorText.Length - lastPosition);
				CharacterProperties props = CreateUpdateCharacters(range);
				Editor.Document.EndUpdateCharacters(props);
			}
		}
		CharacterProperties CreateUpdateCharacters(DocumentRange range) {
			CharacterProperties props = Editor.Document.BeginUpdateCharacters(range);
			props.Underline = UnderlineType.None;
			props.BackColor = Color.Empty;
			return props;
		}
		#endregion
		#region IFilterControl Members
		[DefaultValue(false)]
		public void SetDefaultColumn(FilterColumn column) {
			Tree.SetDefaultColumn(column);
		}
		void IFilterControl.SetViewMode(FilterEditorViewMode view) {
			ViewMode = view;
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlShowOperandTypeIcon"),
#endif
DefaultValue(false)]
		public bool ShowOperandTypeIcon {
			get {
				if (ActiveView == FilterEditorActiveView.Text) return false;
				return Tree.ShowOperandTypeIcon;
			}
			set {
				if (Tree.ShowOperandTypeIcon != value) Tree.ShowOperandTypeIcon = value;
			}
		}
		public void SetFilterColumnsCollection(FilterColumnCollection columns, IDXMenuManager manager) {
			Tree.SetFilterColumnsCollection(columns, manager);
		}
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("FilterEditorControlFilterTextChanged")]
#endif
		public event FilterTextChangedEventHandler FilterTextChanged {
			add { this.Events.AddHandler(filterTextChanged, value); }
			remove { this.Events.RemoveHandler(filterTextChanged, value); }
		}
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("FilterEditorControlCreateCriteriaParseContext")]
#endif
		public event EventHandler<CreateCriteriaParseContextEventArgs> CreateCriteriaParseContext {
			add { this.Events.AddHandler(createCriteriaParseContext, value); }
			remove { this.Events.RemoveHandler(createCriteriaParseContext, value); }
		}
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("FilterEditorControlCreateCriteriaCustomParse")]
#endif
		public event EventHandler<CreateCriteriaCustomParseEventArgs> CreateCriteriaCustomParse {
			add { this.Events.AddHandler(createCriteriaCustomParse, value); }
			remove { this.Events.RemoveHandler(createCriteriaCustomParse, value); }
		}
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("FilterEditorControlCreateCustomRepositoryItem")]
#endif
		public event EventHandler<CreateCustomRepositoryItemEventArgs> CreateCustomRepositoryItem {
			add { this.Events.AddHandler(createCustomRepositoryItem, value); }
			remove { this.Events.RemoveHandler(createCustomRepositoryItem, value); }
		}
		[Browsable(false)]
		public bool IsFilterCriteriaValid {
			get {
				bool isFilterStringValid, canBeDisplayedByTree;
				GetCriteriaFromEditorText(out isFilterStringValid, out canBeDisplayedByTree);
				return isFilterStringValid;
			}
		}
		protected internal virtual void RaiseCreateCustomRepositoryItem(CreateCustomRepositoryItemEventArgs e) {
			EventHandler<CreateCustomRepositoryItemEventArgs> handler = (EventHandler<CreateCustomRepositoryItemEventArgs>)this.Events[createCustomRepositoryItem];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCreateCriteriaParseContext(CreateCriteriaParseContextEventArgs e) {
			EventHandler<CreateCriteriaParseContextEventArgs> handler = (EventHandler<CreateCriteriaParseContextEventArgs>)this.Events[createCriteriaParseContext];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseCreateCriteriaCustomParse(CreateCriteriaCustomParseEventArgs e) {
			EventHandler<CreateCriteriaCustomParseEventArgs> handler = (EventHandler<CreateCriteriaCustomParseEventArgs>)this.Events[createCriteriaCustomParse];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void RaiseFilterTextChanged(FilterTextChangedEventArgs e) {
			FilterTextChangedEventHandler handler = (FilterTextChangedEventHandler)this.Events[filterTextChanged];
			if (handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlAllowAggregateEditing"),
#endif
		DefaultValue(FilterControlAllowAggregateEditing.No)]
		public FilterControlAllowAggregateEditing AllowAggregateEditing
		{
			get { return Tree.AllowAggregateEditing; }
			set { Tree.AllowAggregateEditing = value; }
		}
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("FilterEditorControlShowIsNullOperatorsForStrings"),
#endif
		DefaultValue(false)]
		public bool ShowIsNullOperatorsForStrings
		{
			get { return Tree.ShowIsNullOperatorsForStrings; }
			set { Tree.ShowIsNullOperatorsForStrings = value; }
		}
		#endregion
		#region IFilterControlGetModel
		FilterTreeNodeModel IFilterControlGetModel.Model { get { return this.Model; } }
		#endregion
		protected override BaseControlPainter CreatePainter() {
			return new BaseControlPainter();
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new FilterEditorControlViewInfo(this);
		}
	}
	public class FilterEditorControlViewInfo : BaseStyleControlViewInfo {
		public FilterEditorControlViewInfo(BaseStyleControl owner) : base(owner) { }
		public new FilterEditorControl OwnerControl { get { return base.OwnerControl as FilterEditorControl; } }
		public override Size CalcBestFit(Graphics g) {
			if (OwnerControl.ViewMode == FilterEditorViewMode.Text) return new Size(75, 25); 
			Size size = OwnerControl.Tree.CalcBestSize();
			return OwnerControl.Tab.Visible ? OwnerControl.Tab.CalcSizeByPageClient(size) : size;
		}
	}
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007")]
	public delegate bool ErrorsEvaluatorCriteriaEditorValidatorGetCorrectValue(FilterColumn filterColumn, string curValue, out object newValue);
	public class ErrorsEvaluatorCriteriaEditorValidator : ErrorsEvaluatorCriteriaValidator {
		ErrorsEvaluatorCriteriaEditorValidatorGetCorrectValue getCorrectValue;
		ErrorsEvaluatorCriteriaEditorValidatorGetCorrectValue getValidationValue;
		public ErrorsEvaluatorCriteriaEditorValidator(List<IBoundProperty> properties, ErrorsEvaluatorCriteriaEditorValidatorGetCorrectValue getCorrectValue,
			ErrorsEvaluatorCriteriaEditorValidatorGetCorrectValue getValidationValue) : base(properties) {
			this.getCorrectValue = getCorrectValue;
			this.getValidationValue = getValidationValue;
		}
		protected override Type GetFilterColumnType(IBoundProperty filterProperty) {
			FilterColumn filterColumn = filterProperty as FilterColumn;
			if (filterColumn != null) {
				if (filterColumn.ClauseClass == FilterColumnClauseClass.String) return typeof(string);
				if (filterColumn.ClauseClass == FilterColumnClauseClass.DateTime) return typeof(DateTime);
			}
			return base.GetFilterColumnType(filterProperty);
		}
		protected override object GetCorrectFiltereColumnValue(IBoundProperty filterProperty, object value) {
			FilterColumn filterColumn = filterProperty as FilterColumn;
			if(filterColumn != null && this.getCorrectValue != null && value != null && value is string && GetFilterColumnType(filterColumn) != typeof(string)) {
				object newValue;
				if(this.getCorrectValue(filterColumn, value.ToString(), out newValue)) {
					return newValue;
				}
			}
			return base.GetCorrectFiltereColumnValue(filterProperty, value);
		}
		protected override object GetFilterColumnValueForValidation(IBoundProperty filterProperty, object value) {
			FilterColumn filterColumn = filterProperty as FilterColumn;
			if(filterColumn != null && this.getValidationValue != null && value != null && value is string && GetFilterColumnType(filterColumn) != typeof(string)) {
				object newValue;
				if(this.getValidationValue(filterColumn, value.ToString(), out newValue)) {
					return newValue;
				}
			}
			return base.GetFilterColumnValueForValidation(filterProperty, value);
		}
	}
}
