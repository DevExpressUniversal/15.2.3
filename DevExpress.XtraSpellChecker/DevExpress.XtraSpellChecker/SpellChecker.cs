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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using DevExpress.Data;
using DevExpress.Utils.Serializing;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Forms;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.XtraSpellChecker.Strategies;
using DevExpress.XtraSpellChecker.Rules;
using DevExpress.XtraSpellChecker.Controls;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.XtraEditors;
using DevExpress.XtraSpellChecker.Localization;
using DevExpress.Utils.Menu;
using System.Collections.Generic;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using System.Globalization;
using DevExpress.Utils;
using System.IO;
using System.Reflection;
namespace DevExpress.XtraSpellChecker {
	public enum UnderlineStyle { WavyLine, Line };
	[
	DXToolboxItem(true),
	ToolboxBitmap(typeof(SpellChecker), DevExpress.Utils.ControlConstants.BitmapPath + "SpellChecker.bmp"),
	Designer("DevExpress.XtraSpellChecker.Design.SpellCheckerDesigner," + AssemblyInfo.SRAssemblySpellCheckerDesignFull),
	ProvideProperty("CanCheckText", typeof(Control)),
	ProvideProperty("SpellCheckerOptions", typeof(Control)),
	ProvideProperty("ShowSpellCheckMenu", typeof(TextEdit)),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabComponents)
]
	public class SpellChecker : SpellCheckerBase, System.ComponentModel.IExtenderProvider, ISupportLookAndFeel, ISpellChecker {
		SpellingFormsManager formsManager = null;
		SpellCheckClientRouterBase router = null;
		WeakKeyDictionary<object, bool> canCheckTexts = null;
		WeakKeyDictionary<object, OptionsSpelling> spellCheckerOptions = null;
		WeakKeyDictionary<object, bool> showSpellCheckMenus = null;
		Control checkedControl = null;
		ContainerSearchStrategyBase parentStrategy;
		SpellCheckMode spellCheckMode = SpellCheckMode.OnDemand;
		CheckAsYouTypeOptions checkAsYouTypeOptions = null;
		CheckAsYouTypeManager manager = null;
		SpellCheckerMenuManager menuManager = null;
		bool isCheckAsYouTypeMode = false;
		bool isOptionsRestoring = false;
		Control parentContainer = null;
		FormFocusSpy spy = null;
		UserLookAndFeel lookAndFeel = null;
		#region About
		static SpellChecker() {
		}
		#endregion
		public static void About() {
		}
		public SpellChecker()
			: base() {
			formsManager = CreateFormsManager();
			canCheckTexts = new WeakKeyDictionary<object, bool>();
			spellCheckerOptions = new WeakKeyDictionary<object, OptionsSpelling>();
			showSpellCheckMenus = new WeakKeyDictionary<object, bool>();
			checkAsYouTypeOptions = CreateCheckAsYouTypeOptions();
			lookAndFeel = new UserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
		}
		public SpellChecker(IContainer container)
			: this() {
			container.Add(this);
		}
		protected virtual CheckAsYouTypeOptions CreateCheckAsYouTypeOptions() {
			return new CheckAsYouTypeOptions();
		}
		protected virtual SpellingFormsManager CreateFormsManager() {
			return new SpellingFormsManager(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(router != null)
					Router.Dispose();
				FormsManager.Dispose();
				if(LookAndFeel != null) {
					LookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelChanged);
					LookAndFeel.Dispose();
				}
				DisposeManager();
				if(menuManager != null)
					menuManager.Dispose();
				checkedControl = null;
				if(spy != null)
					spy.Dispose();
				parentContainer = null;
				ClearIgnoreListTable();
			}
			base.Dispose(disposing);
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public SpellingFormsManager FormsManager { get { return formsManager; } }
		[
#if !SL
	DevExpressXtraSpellCheckerLocalizedDescription("SpellCheckerParentContainer"),
#endif
Category(SRCategryNames.SpellChecker)]
		public Control ParentContainer
		{
			get { return parentContainer; }
			set
			{
				if (parentContainer != value)
					parentContainer = value;
				if(!DesignMode)
					Spy.Form = ParentContainer;
			}
		}
		protected internal bool IsDesignMode { get { return DesignMode; } }
		protected FormFocusSpy Spy {
			get {
				if(spy == null)
					spy = new FormFocusSpy(ParentContainer, this);
				return spy;
			}
		}
		protected virtual bool CanSetEditControlProperty(Control control) {
			return control == null || SpellCheckTextControllersManager.Default.IsClassRegistered(control.GetType());
		}
		protected internal void SetEditControl(object value) {
			EditControl = value as Control;
		}
		protected internal bool CanCheckControlQuiet(Control control) {
			return SpellCheckTextControllersManager.Default.IsClassRegistered(control.GetType());
		}
		bool IsControlEditable(Control control) {
			bool result = false;
			ISpellCheckTextControlController controller = SpellCheckTextControllersManager.Default.GetSpellCheckTextControlController(control);
			try {
				result = !controller.IsReadOnly;
			}
			finally {
				controller.Dispose();
			}
			return result;
		}
		public virtual bool CanCheck(Control control) {
			if(control == null)
				return false;
			if(!CanCheckControlQuiet(control))
				return false;
			return !IsChecking && GetCanCheckText(control) && IsControlEditable(control);
		}
		protected internal virtual bool IsCheckAsYouTypeMode {
			get { return SpellCheckMode == SpellCheckMode.AsYouType && ParentStrategy == null && isCheckAsYouTypeMode && !IsTextMode; }
		}
		protected internal virtual bool IsCheckAsYouTypeAndDialogMode {
			get { return SpellCheckMode == SpellCheckMode.AsYouType && ParentStrategy == null && !isCheckAsYouTypeMode && !IsTextMode; }
		}
		protected internal bool IsRichTextEdit(Control control) {
			return control != null && (IsRichTextEditType(control.GetType()) || IsDerivedClassFromRichTextEdit(control));
		}
		protected internal bool IsRichTextEditType(Type type) {
			return type.ToString() == "DevExpress.XtraRichEdit.RichEditControl";
		}
		protected internal bool IsDerivedClassFromRichTextEdit(Control control) {
			Type type = control.GetType();
			while (type.BaseType != null) {
				type = type.BaseType;
				if (IsRichTextEditType(type))
					return true;
			}
			return false;
		}
		protected virtual bool ShouldUseCheckAsYouTypeMode(Control fControl) {
			bool result = SpellCheckMode == SpellCheckMode.AsYouType && !IsTextMode;
			if(!result)
				return false;
			if (CheckAsYouTypeOptions.ShowSpellCheckForm)
				if (fControl != null && IsRichTextEdit(fControl))
					result = false;
				else
					result = result && (manager == null || Manager.EditControl != fControl);
			return result;
		}
		protected internal virtual new Control EditControl {
			get { return base.EditControl as Control; }
			set {
				if(ShouldUseCheckAsYouTypeMode(value) && !(value is CustomSpellCheckMemoEdit))
					DisposeManager();
				base.EditControl = value;
			}
		}
		protected override void OnEditControlModified() {
			if((EditControl is CustomSpellCheckMemoEdit || IsMsWordFormUsed()) && !IsCheckAsYouTypeMode) {
				if(SearchStrategy.TextController is IDisposable) {
					(SearchStrategy.TextController as IDisposable).Dispose();
					SearchStrategy.TextController = null;
				}
				if(CheckedControl == null && EditControl is CustomSpellCheckMemoEdit) {  
					ISpellCheckTextControlController originalController = null;
					SearchStrategy.TextController = CreateTextController(originalController);
					return;
				}
				if(CheckedControl != null && CheckedControl != EditControl) {
					ISpellCheckTextControlController originalController = CreateTextController(CheckedControl);
					SearchStrategy.TextController = CreateTextController(originalController);
				}
			}
			else {
				if(ParentStrategy != null && SpellCheckMode == SpellCheckMode.AsYouType) { 
					if(EditControl is Control) {
						Control editControl = (Control)EditControl;
						Form f = editControl.FindForm();
						if (f != null && !editControl.Focused) {
							f.ActiveControl = null;
							Spy.StopWatch(editControl);
							editControl.BeginInvoke(new Action(() => editControl.Focus()));
							Spy.StartWatch(editControl);
						}
					}
				}
				base.OnEditControlModified();
			}
		}
		protected internal virtual Control CheckedControl {
			get { return checkedControl; }
			set { checkedControl = value; }
		}
		protected override ISpellCheckTextController CreateTextController() {
			return CreateTextController(EditControl);
		}
		protected virtual ISpellCheckTextControlController CreateTextController(ISpellCheckTextControlController originalController) {
			return SpellCheckTextControllersManager.Default.GetSpellCheckTextControlController(EditControl, originalController);
		}
		protected virtual ISpellCheckTextControlController CreateTextController(Control fControl) {
			return SpellCheckTextControllersManager.Default.GetSpellCheckTextControlController(fControl);
		}
		protected virtual bool ShouldCheckSelectedFirst(ISpellCheckTextControlController textController) {
			return OptionsSpelling.IsCheckSelectedTextFirst() && textController.HasSelection && SpellingFormType != SpellingFormType.Word;
		}
		protected virtual bool ShouldCheckFromCursorPos(ISpellCheckTextControlController textController) {
			return OptionsSpelling.IsCheckFromCursorPos();
		}
		protected override SearchStrategy CreateSearchStrategy(object editControl) {
			ISpellCheckTextControlController textController = CreateTextController() as ISpellCheckTextControlController;
			if(textController != null) {
				if(ShouldCheckSelectedFirst(textController))
					return new WinPartTextSearchStrategy(this, textController, textController.SelectionStart, textController.SelectionFinish);
				if(ShouldCheckFromCursorPos(textController))
					return new WinPartSilentTextSearchStrategy(this, textController, textController.SelectionStart);
				return new WinSearchStrategy(this, textController);
			}
			else
				return new WinSearchStrategy(this, new EmptyEditControlTextControlController());
		}
		protected internal new SearchStrategy SearchStrategy {
			get { return base.SearchStrategy; }
			set { base.SearchStrategy = value; }
		}
		protected virtual ContainerSearchStrategyBase ParentStrategy {
			get { return this.parentStrategy; }
			set { this.parentStrategy = value; }
		}
		public IIgnoreList GetIgnoreList(Control control) {
			return GetIgnoreListCore(control);
		}
		protected internal override IIgnoreList GetIgnoreListCore() {
			if (Manager != null && CanUseIgnoreListForControl(Manager.EditControl))
				return GetIgnoreListCore(Manager.EditControl);
			return base.GetIgnoreListCore();
		}
		bool CanUseIgnoreListForControl(Control control) {
			return control != null &&
				(EditControl == null || control == EditControl) &&
				IgnoreListTable.ContainsKey(control);
		}
		void SpellCheckerBase_Disposed(object sender, EventArgs e) {
			UnregisterIgnoreList(sender);
		}
		protected override void SubscribeToControlEvents(object control) {
			Control controlInstance = control as Control;
			if (controlInstance != null)
				controlInstance.Disposed += SpellCheckerBase_Disposed;
		}
		protected override void UnSubscribeFromControlEvents(object control) {
			Control controlInstance = control as Control;
			if (controlInstance != null)
				controlInstance.Disposed -= SpellCheckerBase_Disposed;
		}
		protected override DictionaryCollection GetSharedDictionariesCollection() {
			return SharedDictionaryStorage.SharedDictionaries;
		}
		protected internal DictionaryCollection SharedDictionaries {
			get { return GetSharedDictionariesCollection(); }
		}
		protected virtual SearchStrategy CreateTextSearchStrategy(object editControl, ContainerSearchStrategyBase parentStrategy) {
			ISpellCheckTextControlController textController = CreateTextController() as ISpellCheckTextControlController;
			if(textController != null) {
				return new WinSearchStrategy(this, textController, parentStrategy);
			}
			else
				return new SimpleTextSearchStrategy(this, new SimpleTextController());
		}
		protected internal virtual bool IsMsWordFormUsed() {
			bool result = SpellingFormType == SpellingFormType.Word;
			return result || (FormsManager.IsSpellCheckFormVisible() && FormsManager.SpellCheckForm is SpellingWordStyleForm);
		}
		protected virtual SpellCheckClientRouterBase GetClientRouterInstance() {
			if(IsMsWordFormUsed())
				return new SpellCheckMSWordLikeClientRouter(SearchStrategy);
			else
				return new SpellCheckMSOutlookLikeClientRouter(SearchStrategy);
		}
		protected virtual SpellCheckClientRouterBase Router {
			get { return router; }
			set {
				if(Router != null) {
					Router.Dispose();
					router = null;
				}
				router = value;
			}
		}
		protected virtual SpellCheckerMenuManager MenuManager {
			get {
				if(menuManager == null)
					menuManager = CreateMenuManager();
				return menuManager;
			}
		}
		protected virtual SpellCheckerMenuManager CreateMenuManager() {
			return new SpellCheckerMenuManager(this);
		}
		protected internal override int SuggestionCount {
			get {
				return CheckAsYouTypeOptions.SuggestionCount;
			}
		}
		public override List<SpellCheckerCommand> GetCommandsByError(SpellCheckErrorBase error) {
			if(IsCheckAsYouTypeAndDialogMode || IsCheckAsYouTypeMode)
				return GetCommandsByError(Manager.OperationsManager, error);
			else
				return GetCommandsByError(SearchStrategy.OperationsManager, error);
		}
		#region Check Control
		protected override void DoBeforeCheck() {
			isCheckAsYouTypeMode = false;
			base.DoBeforeCheck();
			if(IsTextMode)
				DisposeManager();
			if(ParentStrategy == null)
				FormsManager.DestroyAllForms();
				Router = GetClientRouterInstance();
		}
		protected internal override void DoAfterCheck(SearchStrategy strategy) {
			if(Router != null) {
				Router.Dispose();
				Router = null;
			}
			if(SearchStrategy.TextController is IDisposable)
				(SearchStrategy.TextController as IDisposable).Dispose();
			SearchStrategy.TextController = null;
			if(ParentStrategy == null)
				FormsManager.DestroyAllForms();
			DestroySearchStrategy();
			base.DoAfterCheck(strategy);
		}
		protected virtual void DoBeforeCheck(Control editControl) {
			this.CheckedControl = EditControl = editControl;
			DoBeforeCheck();
			OptionsSpelling.CombineOptions(GetSpellCheckerOptions(EditControl));
		}
		protected internal virtual void DoAfterCheck(Control editControl) {
			SetSpellCheckerOptions(CheckedControl, OptionsSpelling);
			CheckedControl = null;
			DoAfterCheck(SearchStrategy);
		}
		protected virtual void CheckAsYouType(Control fEditControl) {
			if(EditControl != fEditControl) {
				Manager.Dispose();
				manager = null;
			}
			if(Manager.IsDisposing)
				manager = null;
			Manager.Check(fEditControl);
		}
		public virtual void Check(Control editControl) {
			try {
				if (!CanCheck(editControl))
					return;
				if (ShouldUseCheckAsYouTypeMode(editControl)) {
					isCheckAsYouTypeMode = true;
					CheckAsYouType(editControl);
				}
				else {
					DoBeforeCheck(editControl);
					try {
						SearchStrategy.Check();
					}
					finally {
						DoAfterCheck(editControl);
					}
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		void ISpellChecker.Check(object obj) {
			try {
				Control control = obj as Control;
				if (control != null)
					this.Check(control);
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		protected internal virtual void CheckControlWhenGotFocus(Control editControl) {
			try {
				if (!CanCheck(editControl))
					return;
				DoBeforeCheck(editControl);
				try {
					isCheckAsYouTypeMode = true;
					CheckAsYouType(editControl);
				}
				finally {
					if (SearchStrategy.TextController is IDisposable)
						(SearchStrategy.TextController as IDisposable).Dispose();
					SearchStrategy.TextController = null;
					if (!IsCheckAsYouTypeMode)
						DoAfterCheck(editControl);
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		public virtual void Check(ISpellCheckTextControlController controller) {
			try {
				if (CanCheck()) {
					DoBeforeCheck();
					try {
						SearchStrategy.Check();
					}
					finally {
						SearchStrategy.TextController = null;
						DoAfterCheck(SearchStrategy);
					}
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		#endregion
		#region CheckContainer
		public virtual void CheckContainer(Control container) {
			IContainerControl containerControl = ParentContainer as IContainerControl;
			Control activeControl = null;
			if (containerControl != null)
				activeControl = containerControl.ActiveControl;
			this.isCheckAsYouTypeMode = false;
			try {
				ParentStrategy = new WinControlContainerSearchStrategy(this, container);
				ParentStrategy.Check();
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
			finally {
				DoAfterCheck(EditControl);
				FormsManager.DestroyAllForms();
				ParentStrategy.Dispose();
				ParentStrategy = null;
				if (containerControl != null)
					containerControl.ActiveControl = activeControl;
			}
		}
		public virtual void CheckContainer() {
			if (ParentContainer != null)
				CheckContainer(ParentContainer);
		}
		protected internal override void Check(object editControl, ContainerSearchStrategyBase parentStrategy) {
			try {
				EditControl = editControl as Control;
				if (EditControl == null || !CanCheck(EditControl)) {
					parentStrategy.FinishControlChecking();
					return;
				}
				DoBeforeCheck(EditControl);
				try {
					SearchStrategy = CreateTextSearchStrategy(editControl, parentStrategy);
					Router = GetClientRouterInstance();
					SearchStrategy.AfterCheck += new EventHandler(SearchStrategy_AfterCheck);
					SearchStrategy.Check();
				}
				finally {
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		void SearchStrategy_AfterCheck(object sender, EventArgs e) {
			OnStrategyAfterCheck();
		}
		private void OnStrategyAfterCheck() {
			SearchStrategy.AfterCheck -= new EventHandler(SearchStrategy_AfterCheck);
			Router.Dispose();
			if(SearchStrategy.TextController is IDisposable)
				((IDisposable)SearchStrategy.TextController).Dispose();
			SearchStrategy.TextController = null;
			DestroySearchStrategy();
			RestoreComponentOptions();
			EditControl = null;
			CheckedControl = null;
			ParentStrategy.FinishControlChecking();
		}
#endregion
		[
#if !SL
	DevExpressXtraSpellCheckerLocalizedDescription("SpellCheckerSpellCheckMode"),
#endif
		Category(SRCategryNames.SpellChecker),
		DefaultValue(SpellCheckMode.OnDemand)
		]
		public SpellCheckMode SpellCheckMode
		{
			get { return this.spellCheckMode; }
			set
			{
				this.spellCheckMode = value;
				RaiseSpellCheckModeChanged();
				if (SpellCheckMode != SpellCheckMode.AsYouType && manager != null)
					DisposeManager();
			}
		}
		#region LookAndFeel
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraSpellCheckerLocalizedDescription("SpellCheckerLookAndFeel"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Category("Appearance")]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		protected virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			FormsManager.LookAndFeelChanged();
		}
		bool ISupportLookAndFeel.IgnoreChildren {
			get { return true; }
		}
		#endregion
		[
#if !SL
	DevExpressXtraSpellCheckerLocalizedDescription("SpellCheckerCheckAsYouTypeOptions"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Category(SRCategryNames.SpellChecker),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public CheckAsYouTypeOptions CheckAsYouTypeOptions
		{
			get { return checkAsYouTypeOptions; }
		}
		protected virtual CheckAsYouTypeManager CreateCheckAsYouTypeManager() {
			return new CheckAsYouTypeManager(this);
		}
		protected internal virtual CheckAsYouTypeManager Manager {
			get {
				if(manager == null)
					manager = CreateCheckAsYouTypeManager();
				return manager;
			}
		}
		protected internal virtual void DisposeManager() {
			if(manager != null) {
				Manager.Dispose();
				manager = null;
			}
		}
		protected internal override bool CanHandleErrorVisually() {
			return true;
		}
		protected override OptionsSpellingBase CreateOptionsSpelling() {
			return new OptionsSpelling();
		}
		[
#if !SL
	DevExpressXtraSpellCheckerLocalizedDescription("SpellCheckerOptionsSpelling"),
#endif
			Category(SRCategryNames.SpellChecker),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
			XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public virtual new OptionsSpelling OptionsSpelling
		{
			get { return base.OptionsSpelling as OptionsSpelling; }
		}
		internal bool IsOptionsRestoring { get { return isOptionsRestoring; } }
		protected override void RestoreComponentOptions() {
			isOptionsRestoring = true;
			base.RestoreComponentOptions();
			isOptionsRestoring = false;
		}
		protected override void OnDictionaryNotLoaded(ISpellCheckerDictionary dict) {
			string message = SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MsgNotLoadedDictionaryException);
			throw new NotLoadedDictionaryException(String.Format(message, dict.Culture.Name));
		}
		protected override void OnCultureChanged() {
			base.OnCultureChanged();
			if (manager != null && Manager.EditControl != null) {
				Control c = manager.EditControl;
				CheckAsYouTypeManager m = Manager;
				manager = null;
				m.Dispose();
				CheckAsYouType(c);
			}
			RaiseCultureChanged();
		}
		#region SpellingFormShowing
		private static readonly object spellingFormShowing = new object();
		[
#if !SL
	DevExpressXtraSpellCheckerLocalizedDescription("SpellCheckerSpellingFormShowing"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event SpellingFormShowingEventHandler SpellingFormShowing
		{
			add { Events.AddHandler(spellingFormShowing, value); }
			remove { Events.RemoveHandler(spellingFormShowing, value); }
		}
		protected internal virtual void OnSpellingFormShowing(SpellingFormShowingEventArgs e) {
			RaiseSpellingFormShowing(e);
		}
		protected virtual void RaiseSpellingFormShowing(SpellingFormShowingEventArgs e) {
			SpellingFormShowingEventHandler handler = (SpellingFormShowingEventHandler)this.Events[spellingFormShowing];
			if(handler != null)
				handler(this, e);
		}
		#endregion
		#region OptionsFormShowing
		private static readonly object optionsFormShowing = new object();
		[
#if !SL
	DevExpressXtraSpellCheckerLocalizedDescription("SpellCheckerOptionsFormShowing"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event FormShowingEventHandler OptionsFormShowing
		{
			add { Events.AddHandler(optionsFormShowing, value); }
			remove { Events.RemoveHandler(optionsFormShowing, value); }
		}
		protected internal virtual void OnOptionsFormShowing(FormShowingEventArgs e) {
			RaiseOptionsFormShowing(e);
		}
		protected virtual void RaiseOptionsFormShowing(FormShowingEventArgs e) {
			FormShowingEventHandler handler = (FormShowingEventHandler)this.Events[optionsFormShowing];
			if(handler != null)
				handler(this, e);
		}
		#endregion
		#region CheckCompleteFormShowing
		private static readonly object checkCompleteFormShowing = new object();
		[
#if !SL
	DevExpressXtraSpellCheckerLocalizedDescription("SpellCheckerCheckCompleteFormShowing"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event FormShowingEventHandler CheckCompleteFormShowing
		{
			add { Events.AddHandler(checkCompleteFormShowing, value); }
			remove { Events.RemoveHandler(checkCompleteFormShowing, value); }
		}
		protected internal virtual void OnCheckCompleteFormShowing(FormShowingEventArgs e) {
			RaiseCheckCompleteFormShowing(e);
		}
		protected virtual void RaiseCheckCompleteFormShowing(FormShowingEventArgs e) {
			FormShowingEventHandler handler = (FormShowingEventHandler)this.Events[checkCompleteFormShowing];
			if(handler != null)
				handler(this, e);
		}
		#endregion
		#region PrepareContextMenu
		private static readonly object prepareContextMenu = new object();
		[Category(SRCategryNames.SpellChecker)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'PopupMenuShowing' instead", true)]
		public event PrepareContextMenuEventHandler PrepareContextMenu {
			add { Events.AddHandler(prepareContextMenu, value); }
			remove { Events.RemoveHandler(prepareContextMenu, value); }
		}
		[Obsolete("Please use the RaisePopupMenuShowing instead.")]
		protected virtual void RaisePrepareContextMenu(PrepareContextMenuEventArgs e) {
			PrepareContextMenuEventHandler handler = (PrepareContextMenuEventHandler)this.Events[prepareContextMenu];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region PopupMenuShowing
		private static readonly object onPopupMenuShowing = new object();
		[
#if !SL
	DevExpressXtraSpellCheckerLocalizedDescription("SpellCheckerPopupMenuShowing"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event PopupMenuShowingEventHandler PopupMenuShowing
		{
			add { Events.AddHandler(onPopupMenuShowing, value); }
			remove { Events.RemoveHandler(onPopupMenuShowing, value); }
		}
		protected virtual void RaisePopupMenuShowing(PopupMenuShowingEventArgs e) {
			PopupMenuShowingEventHandler handler = (PopupMenuShowingEventHandler)this.Events[onPopupMenuShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region IExtenderProvider Members
		protected WeakKeyDictionary<object, bool> CanCheckTexts { get { return canCheckTexts; } }
		protected WeakKeyDictionary<object, OptionsSpelling> SpellCheckerOptions { get { return spellCheckerOptions; } }
		protected WeakKeyDictionary<object, bool> ShowSpellCheckMenus { get { return showSpellCheckMenus; } }
		protected OptionsSpelling CreateDefaultOptions() {
			return new OptionsSpelling();
		}
		bool IExtenderProvider.CanExtend(object extendee) {
			Control control = extendee as Control;
			return control != null && SpellCheckTextControllersManager.Default.IsClassRegistered(control.GetType());
		}
		[DefaultValue(true), EditorBrowsable(EditorBrowsableState.Never)]
		public bool GetCanCheckText(Control control) {
			if(control == null) return false;
			if(!CanCheckTexts.ContainsKey(control))
				return true;
			return (bool)CanCheckTexts[control];
		}
		public void SetCanCheckText(Control control, bool canCheckText) {
			if(control == null) return;
			if(CanCheckTexts.ContainsKey(control))
				CanCheckTexts[control] = canCheckText;
			else {
				CanCheckTexts.Add(control, canCheckText);
				if(canCheckText)
					SubscribeToTextEditEvent(control);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool GetShowSpellCheckMenu(Control control) {
			if(control == null || !CanSetShowSpellCheckMenu(control))
				return false;
			if(!ShowSpellCheckMenus.ContainsKey(control))
				SetShowSpellCheckMenu(control, true);
			return (bool)ShowSpellCheckMenus[control];
		}
		protected virtual bool CanSetShowSpellCheckMenu(Control control) {
			return control is DevExpress.XtraEditors.TextEdit;
		}
		public void SetShowSpellCheckMenu(Control control, bool showSpellCheckMenu) {
			if(control == null || !CanSetShowSpellCheckMenu(control))
				return;
			if(!ShowSpellCheckMenus.ContainsKey(control))
				ShowSpellCheckMenus.Add(control, showSpellCheckMenu);
			else
				UnsubscribeFromTextEditEvent(control);
			if(showSpellCheckMenu)  
				SubscribeToTextEditEvent(control);
			ShowSpellCheckMenus[control] = showSpellCheckMenu;
		}
		#region Menu
		protected internal virtual void UnsubscribeFromTextEditEvent(Control control) {
			TextEdit textEdit = control as TextEdit;
			if (textEdit != null)
				textEdit.Properties.BeforeShowMenu -= Properties_BeforeShowMenu;
		}
		protected internal virtual void SubscribeToTextEditEvent(Control control) {
			TextEdit textEdit = control as TextEdit;
			if (textEdit != null)
				textEdit.Properties.BeforeShowMenu += Properties_BeforeShowMenu;
		}
		protected virtual SpellCheckErrorBase GetErrorByCursorPos(int pos) {
			if(Manager.EditControl != null)
				return Manager.GetErrorByCursorPos(pos);
			else
				return null;
		}
		protected virtual SpellCheckErrorBase GetClickedError(Point clientPos) {
			if(Manager.EditControl != null)
				return Manager.GetErrorByClientPosition(clientPos);
			else
				return null;
		}
		protected virtual SpellCheckErrorBase GetErrorByCursorPos() {
			if(Manager.EditControl != null)
				return Manager.GetErrorByCursorPosition();
			else
				return null;
		}
		public virtual SpellCheckErrorBase CalcError(Point p) {
			if(p.X == -1 && p.Y == -1)
				return GetErrorByCursorPos();
			else
				return GetClickedError(p);
		}
		void Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e) {
			Control popupControl = sender as Control;
			SpellCheckErrorBase error = null;
			if ((IsCheckAsYouTypeMode || IsCheckAsYouTypeAndDialogMode) && popupControl == Manager.EditControl && IsControlEditable(popupControl)) {
				Point p = e.Location;
				error = CalcError(p);
			}
			MenuManager.UpdateMenuItems(e.Menu, error, popupControl);
			PopupMenuShowingEventArgs args = new PopupMenuShowingEventArgs(e.Menu, e.Location);
			RaisePopupMenuShowing(args);
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public OptionsSpelling GetSpellCheckerOptions(Control control) {
			try {
				if (control == null) return null;
				if (!SpellCheckerOptions.ContainsKey(control)) {
					SpellCheckerOptions.Add(control, CreateDefaultOptions());
				}
				return SpellCheckerOptions[control] as OptionsSpelling;
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
			return null;
		}
		public void SetSpellCheckerOptions(Control control, OptionsSpelling options) {
			try {
				if (control == null) return;
				if (options == null)
					SpellCheckerOptions.Remove(control);
				else
					if (SpellCheckerOptions.ContainsKey(control))
						(SpellCheckerOptions[control] as OptionsSpelling).Assign(options);
					else
						SpellCheckerOptions.Add(control, options);
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		#endregion
		#region ISpellChecker Members
		bool ISpellChecker.IsChecking { get { return IsChecking; } }
		IOptionsSpellings ISpellChecker.GetOptions(object control) {
			Control checkedControl = control as Control;
			if (checkedControl != null)
				return GetSpellCheckerOptions(checkedControl);
			return null;
		}
		#region SpellCheckModeChanged event
		EventHandler spellCheckModeChanged;
		event EventHandler ISpellChecker.SpellCheckModeChanged { add { spellCheckModeChanged += value; } remove { spellCheckModeChanged -= value; } }
		void RaiseSpellCheckModeChanged() {
			EventHandler handler = this.spellCheckModeChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region CultureChanged event
		EventHandler cultureChanged;
		event EventHandler ISpellChecker.CultureChanged { add { cultureChanged += value; } remove { cultureChanged -= value; } }
		void RaiseCultureChanged() {
			EventHandler handler = this.cultureChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region CustomDictionaryChanged event
		EventHandler onCustomDictionaryChanged;
		[EditorBrowsable(EditorBrowsableState.Never)]
		event EventHandler ISpellChecker.CustomDictionaryChanged { add { onCustomDictionaryChanged += value; } remove { onCustomDictionaryChanged -= value; } }
		protected internal void RaiseCustomDictionaryChanged() {
			EventHandler handler = onCustomDictionaryChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		ICheckSpellingResult ISpellChecker.CheckText(object editor, string text, int index, CultureInfo culture) {
			try {
				Control control = editor as Control;
				if (control != null) {
					SaveComponentOptions();
					OptionsSpelling.CombineOptions(GetSpellCheckerOptions(control));
				}
				try {
					return CheckText(text, index, culture);
				}
				finally {
					if (control != null)
						RestoreComponentOptions();
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
			return null;
		}
		ISpellingErrorInfo ISpellChecker.Check(object control, ISpellCheckTextController controller, Position from, Position to) {
			try {
				Control checkedControl = control as Control;
				if (checkedControl != null) {
					SaveComponentOptions();
					OptionsSpelling.CombineOptions(GetSpellCheckerOptions(checkedControl));
				}
				try {
					return Check(controller, from, to);
				}
				finally {
					if (checkedControl != null)
						RestoreComponentOptions();
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
			return null;
		}
		bool ISpellChecker.CanAddToDictionary() {
			return CanAddToDictionary(Culture);
		}
		void ISpellChecker.AddToDictionary(string word) {
			AddToDictionary(word, Culture);
		}
		void ISpellChecker.AddToDictionary(string word, CultureInfo culture) {
			AddToDictionary(word, culture);
		}
		bool ISpellChecker.CanAddToDictionary(CultureInfo culture) {
			return CanAddToDictionary(culture);
		}
		void ISpellChecker.Ignore(object control, string word, Position start, Position end) {
			Ignore(control, word, start, end);
		}
		void ISpellChecker.IgnoreAll(object control, string word) {
			IgnoreAll(control, word);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		void ISpellChecker.RegisterIgnoreList(object control, IIgnoreList ignoreList) {
			RegisterIgnoreList(control, ignoreList);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		void ISpellChecker.UnregisterIgnoreList(object control) {
			UnregisterIgnoreList(control);
		}
		string[] ISpellChecker.GetSuggestions(string word, CultureInfo culture) {
			return GetSuggestions(word, culture).ToStringArray();
		}
		#endregion
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class OptionsSpelling : OptionsSpellingBase {
		protected internal virtual new bool IsCheckFromCursorPos() {
			return base.IsCheckFromCursorPos();
		}
		protected internal virtual new bool IsCheckSelectedTextFirst() {
			return base.IsCheckSelectedTextFirst();
		}
		protected internal virtual new bool IsIgnoreUpperCaseWords() {
			return base.IsIgnoreUpperCaseWords();
		}
		protected internal virtual new bool IsIgnoreRepeatedWords() {
			return base.IsIgnoreRepeatedWords();
		}
		protected internal virtual new bool IsIgnoreMixedCaseWords() {
			return base.IsIgnoreMixedCaseWords();
		}
		protected internal virtual new bool IsIgnoreWordsWithNumbers() {
			return base.IsIgnoreWordsWithNumbers();
		}
		protected internal virtual new bool IsIgnoreUrls() {
			return base.IsIgnoreUrls();
		}
		protected internal virtual new bool IsIgnoreEmails() {
			return base.IsIgnoreEmails();
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class CheckAsYouTypeOptions {
		int suggestionCount = 5;
		Color color = Color.Red;
		UnderlineStyle underlineStyle = UnderlineStyle.WavyLine;
		bool showSpellCheckForm = true;
		bool checkControlsInParentContainer = false;
		#region SuggestionCount
		[
#if !SL
	DevExpressXtraSpellCheckerLocalizedDescription("CheckAsYouTypeOptionsSuggestionCount"),
#endif
DefaultValue(5), XtraSerializableProperty()]
		public int SuggestionCount
		{
			get { return suggestionCount; }
			set
			{
				if (suggestionCount != value)
				{
					suggestionCount = value;
				}
			}
		}
		protected internal bool ShouldSerializeSuggestionsCount() {
			return SuggestionCount != 5;
		}
		#endregion
		#region ShowSpellCheckForm
		[
#if !SL
	DevExpressXtraSpellCheckerLocalizedDescription("CheckAsYouTypeOptionsShowSpellCheckForm"),
#endif
DefaultValue(true), XtraSerializableProperty()]
		public bool ShowSpellCheckForm
		{
			get { return showSpellCheckForm; }
			set
			{
				if (showSpellCheckForm != value)
				{
					showSpellCheckForm = value;
				}
			}
		}
		protected internal bool ShouldSerializeShowSpellCheckForm() {
			return !ShowSpellCheckForm;
		}
		#endregion
		#region Color
		[
#if !SL
	DevExpressXtraSpellCheckerLocalizedDescription("CheckAsYouTypeOptionsColor"),
#endif
XtraSerializableProperty()]
		public Color Color
		{
			get { return color; }
			set
			{
				if (color != value)
				{
					color = value;
					OnOptionChanged();
				}
			}
		}
		protected internal bool ShouldSerializeColor() {
			return Color != Color.Red;
		}
		#endregion
		#region ErrorStyle
		[
#if !SL
	DevExpressXtraSpellCheckerLocalizedDescription("CheckAsYouTypeOptionsUnderlineStyle"),
#endif
DefaultValue(UnderlineStyle.WavyLine), XtraSerializableProperty()]
		public UnderlineStyle UnderlineStyle
		{
			get { return underlineStyle; }
			set
			{
				if (underlineStyle != value)
				{
					underlineStyle = value;
					OnOptionChanged();
				}
			}
		}
		protected internal bool ShouldSerializeErrorStyle() {
			return UnderlineStyle != UnderlineStyle.WavyLine;
		}
		#endregion
		#region CheckControlsInParentContainer
		[
#if !SL
	DevExpressXtraSpellCheckerLocalizedDescription("CheckAsYouTypeOptionsCheckControlsInParentContainer"),
#endif
DefaultValue(false), XtraSerializableProperty()]
		public bool CheckControlsInParentContainer
		{
			get { return checkControlsInParentContainer; }
			set
			{
				if (checkControlsInParentContainer != value)
				{
					checkControlsInParentContainer = value;
					OnOptionChanged();
				}
			}
		}
		protected internal bool ShouldSerializeCheckControlsInParentContainer() {
			return CheckControlsInParentContainer;
		}
		#endregion
		#region OptionChanged
		[Browsable(false)]
		public event EventHandler OptionChanged;
		protected virtual void RaiseOptionsChangedHandler(EventArgs e) {
			if(OptionChanged != null)
				OptionChanged(this, e);
		}
		protected virtual void OnOptionChanged() {
			RaiseOptionsChangedHandler(EventArgs.Empty);
		}
		#endregion
	}
	public class WinSpellCheckerChangeCommand : SpellCheckerChangeCommand {
		SpellChecker spellChecker;
		public WinSpellCheckerChangeCommand(TextOperationsManager operationsManager, SpellChecker spellChecker)
			: base(operationsManager) {
			this.spellChecker = spellChecker;
		}
		protected new CheckAsYouTypeOperationsManager OperationsManager { get { return base.OperationsManager as CheckAsYouTypeOperationsManager; } }
		protected override void DoCommandCore() {
			if(SpellChecker.IsCheckAsYouTypeMode || SpellChecker.IsCheckAsYouTypeAndDialogMode)
				OperationsManager.ReplaceWord(this.Start, this.Finish, this.Suggestion);
			else
				base.DoCommandCore();
		}
		public SpellChecker SpellChecker {
			get { return spellChecker; }
		}
		public override string Caption {
			get {
				return this.Suggestion;
			}
		}
	}
	public class WinSpellCheckerDeleteCommand : SpellCheckerChangeCommand {
		SpellChecker spellChecker;
		public WinSpellCheckerDeleteCommand(TextOperationsManager operationsManager, SpellChecker spellChecker)
			: base(operationsManager) {
			this.spellChecker = spellChecker;
		}
		protected new CheckAsYouTypeOperationsManager OperationsManager { get { return base.OperationsManager as CheckAsYouTypeOperationsManager; } }
		protected override void DoCommandCore() {
			if(SpellChecker.IsCheckAsYouTypeMode || SpellChecker.IsCheckAsYouTypeAndDialogMode)
				OperationsManager.Delete(this.Start, this.Finish);
			else
				base.DoCommandCore();
		}
		public SpellChecker SpellChecker {
			get { return spellChecker; }
		}
		public override string Caption {
			get {
				return SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MnuDeleteRepeatedWord);
			}
		}
	}
	public class WinSpellCheckerAddToDictionaryCommand : SpellCheckerCommand {
		public WinSpellCheckerAddToDictionaryCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected new CheckAsYouTypeOperationsManager OperationsManager { get { return base.OperationsManager as CheckAsYouTypeOperationsManager; } }
		protected override void DoCommandCore() {
			OperationsManager.AddToDictionary(Start, Finish, Word, Culture);
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.AddToDictionary; } }
		public override string Caption {
			get {
				return SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MnuAddtoDictionaryCaption);
			}
		}
		public override bool Enabled {
			get {
				return OperationsManager.CanAddAddToDictionaryItem(Culture);
			}
		}
	}
	public class WinSpellCheckerIgnoreCommand : SpellCheckerCommand {
		public WinSpellCheckerIgnoreCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.IgnoreOnce(Start, Finish, this.Word);
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.Ignore; } }
		public override string Caption {
			get {
				return SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MnuIgnoreRepeatedWord);
			}
		}
	}
	public class WinSpellCheckerIgnoreAllCommand : SpellCheckerCommand {
		public WinSpellCheckerIgnoreAllCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.IgnoreAll(Word);
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.IgnoreAll; } }
		public override string Caption {
			get {
				return SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MnuIgnoreAllItemCaption);
			}
		}
	}
	public class WinSpellCheckerNoSuggestionsCommand : SpellCheckerNoSuggestionsCommand {
		public WinSpellCheckerNoSuggestionsCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		public override string Caption {
			get {
				return SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MnuNoSuggestionsCaption);
			}
		}
	}
}
namespace DevExpress.XtraSpellChecker.Native {
	public abstract class SpellCheckerHandlerBase : IDisposable {
		SpellChecker spellChecker;
		protected SpellCheckerHandlerBase(SpellChecker spellChecker) {
			this.spellChecker = spellChecker;
		}
		public SpellChecker SpellChecker { get { return this.spellChecker; } }
		protected abstract void DoBeforeCheck();
		protected abstract void DoCheckCore();
		protected abstract void DoAfterCheck();
		public virtual void Check() {
			DoBeforeCheck();
			try {
				DoCheckCore();
			}
			finally {
				DoAfterCheck();
			}
		}
		public void Dispose() {
			spellChecker = null;
		}
	}
	public class SpellCheckerOnDemandHandler : SpellCheckerHandlerBase {
		public SpellCheckerOnDemandHandler(SpellChecker spellChecker) : base(spellChecker) { }
		protected override void DoBeforeCheck() {
		}
		protected override void DoCheckCore() {
		}
		protected override void DoAfterCheck() {
		}
	}
	public class FormFocusSpy : IDisposable {
		Control form = null;
		SpellChecker spellChecker = null;
		public FormFocusSpy(Control form, SpellChecker spellChecker) {
			this.spellChecker = spellChecker;
			this.Form = form;
			SpellChecker.CheckAsYouTypeOptions.OptionChanged += OnOptionChanged;
		}
		public Control Form {
			get { return form; }
			set {
				if (Object.ReferenceEquals(form, value))
					return;
				Control oldValue = Form;
				form = value;
				OnContainerChanged(oldValue, Form);
			}
		}
		public SpellChecker SpellChecker { get { return spellChecker; } }
		void OnOptionChanged(object sender, EventArgs e) {
				ProcessFormControls(Form);
		}
		void OnContainerChanged(Control oldValue, Control newValue) {
			if (oldValue != null)
				UnsubscribeControlFromEvents(oldValue);
			if (newValue != null)
				ProcessFormControls(Form);
		}
		protected virtual bool CanCheckControl(Control control) {
			return spellChecker.CanCheckControlQuiet(control);
		}
		protected virtual void ProcessFormControlsCore(Control container) {
			if (container == null)
				return;
			ProcessContainerControlsRecursive(container);
		}
		void ProcessContainerControlsRecursive(Control container) {
			if (CanCheckControl(container)) {
				UnsubscribeFromControlEvents(container);
				if (SpellChecker.CheckAsYouTypeOptions.CheckControlsInParentContainer)
					SubscribeToControlEvents(container);
				SpellChecker.UnsubscribeFromTextEditEvent(container);
				if (SpellChecker.GetShowSpellCheckMenu(container))
					SpellChecker.SubscribeToTextEditEvent(container);
			}
			else {
				UnsubscribeContainerFromOwnEvents(container);
				SubscribeContainerToOwnEvents(container);
				int childrenCount = container.Controls.Count;
				for (int i = 0; i < childrenCount; i++)
					ProcessContainerControlsRecursive(container.Controls[i]);
			}
		}
		void UnsubscribeFromContainerControlsEventsRecursive(Control container) {
			if (CanCheckControl(container)) {
				UnsubscribeFromControlEvents(container);
				SpellChecker.UnsubscribeFromTextEditEvent(container);
			}
			else {
				UnsubscribeContainerFromOwnEvents(container);
				int childrenCount = container.Controls.Count;
				for (int i = 0; i < childrenCount; i++)
					UnsubscribeFromContainerControlsEventsRecursive(container.Controls[i]);
			}
		}
		void SubscribeToControlEvents(Control control) {
			control.Enter += OnControlGotFocus;
			control.VisibleChanged += OnControlVisibleChanged;
			control.Disposed += OnControlDisposed;
		}
		void UnsubscribeFromControlEvents(Control control) {
			control.Enter -= OnControlGotFocus;
			control.VisibleChanged -= OnControlVisibleChanged;
			control.Disposed -= OnControlDisposed;
		}
		internal void StopWatch(Control control) {
			if (Form.IsChild(control))
				UnsubscribeFromControlEvents(control);
		}
		internal void StartWatch(Control control) {
			if (Form.IsChild(control))
				SubscribeToControlEvents(control);
		}
		public virtual void ProcessFormControls(Control container) {
			ProcessFormControlsCore(container);
		}
		void OnControlGotFocus(object sender, EventArgs e) {
			if(SpellChecker.SpellCheckMode == SpellCheckMode.AsYouType)
				SpellChecker.CheckControlWhenGotFocus((Control)sender);
		}
		void OnControlVisibleChanged(object sender, EventArgs e) {
			Control control = sender as Control;
			if (!control.Visible)
				DisposeCheckAsYouTypeManager(control);
		}
		void OnControlDisposed(object sender, EventArgs e) {
			Control control = sender as Control;
			UnsubscribeFromControlEvents(control);
			DisposeCheckAsYouTypeManager(control);
		}
		void DisposeCheckAsYouTypeManager(Control control) {
			if (SpellChecker == null || SpellChecker.SpellCheckMode != SpellCheckMode.AsYouType)
				return;
			if (Object.ReferenceEquals(control, SpellChecker.Manager.EditControl))
				SpellChecker.DisposeManager();
		}
		void SubscribeContainerToOwnEvents(Control container) {
			container.ControlAdded += OnControlAdded;
			container.ControlRemoved += OnControlRemoved;
			container.Disposed += OnContainerDisposed;
		}
		void UnsubscribeContainerFromOwnEvents(Control container) {
			container.ControlAdded -= OnControlAdded;
			container.ControlRemoved -= OnControlRemoved;
			container.Disposed -= OnContainerDisposed;
		}
		void OnContainerDisposed(object sender, EventArgs e) {
			Control container = sender as Control;
			if (container != null)
				UnsubscribeContainerFromEvents(container);
		}
		protected virtual void OnControlRemoved(object sender, ControlEventArgs e) {
			UnsubscribeControlFromEvents(e.Control);
		}
		protected virtual void OnControlAdded(object sender, ControlEventArgs e) {
			ProcessContainerControlsRecursive(e.Control);
		}
		protected virtual void UnsubscribeControlFromEvents(Control control) {
			if (CanCheckControl(control)) {
				UnsubscribeFromControlEvents(control);
				SpellChecker.UnsubscribeFromTextEditEvent(control);
			}
			else
				UnsubscribeContainerFromEvents(control);
		}
		void UnsubscribeContainerFromEvents(Control control) {
			UnsubscribeContainerFromOwnEvents(control);
			int childrenCount = control.Controls.Count;
			for (int i = 0; i < childrenCount; i++)
				UnsubscribeControlFromEvents(control.Controls[i]);
		}
		#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (Form != null) {
					UnsubscribeControlFromEvents(Form);
					form = null;
				}
				if (SpellChecker != null) {
					SpellChecker.CheckAsYouTypeOptions.OptionChanged -= OnOptionChanged;
					spellChecker = null;
				}
			}
		}
		#endregion
	}
	internal static class ControlExtensions {
		public static bool IsChild(this Control parent, Control child) {
			if (parent == null)
				return false;
			while (child != null) {
				if (Object.ReferenceEquals(parent, child.Parent))
					return true;
				child = child.Parent;
			}
			return false;
		}
	}
	public class SpellCheckerMenuManager : IDisposable {
		SpellChecker spellChecker = null;
		Control popupControl = null;
		public SpellCheckerMenuManager(SpellChecker spellChecker) {
			this.spellChecker = spellChecker;
		}
		protected virtual SpellChecker SpellChecker { get { return this.spellChecker; } }
		protected CheckAsYouTypeManager Manager { get { return SpellChecker.Manager; } }
		protected virtual bool IsSpellCheckMenuItem(DXMenuItem item) {
			return item.Tag != null && (item.Tag is SpellCheckErrorBase || string.Compare(item.Tag.ToString(), "SpellCheck") == 0);
		}
		protected virtual void RemoveSpellCheckMenuItems(DXPopupMenu menu) {
			for(int i = menu.Items.Count - 1;i >= 0;i--) {
				if(IsSpellCheckMenuItem(menu.Items[i]))
					menu.Items.RemoveAt(i);
			}
			menu.Items[0].BeginGroup = false;
		}
		protected virtual void AddSpellCheckMenuItems(DXPopupMenu menu, SpellCheckErrorBase error, Control popupControl) {
			if(error != null) {
				AddErrorMenuItems(menu, error);
				SetBeginGroupForFirstStandardItem(menu, error);
			}
			if (SpellChecker.SpellCheckMode != SpellCheckMode.AsYouType || SpellChecker.CheckAsYouTypeOptions.ShowSpellCheckForm)
				AddSpellCheckMenuItem(menu, popupControl);
		}
		protected virtual int CalcFirstMenuItemIndex(SpellCheckErrorBase error) {
			if(error.Result == SpellCheckOperation.Change)
				return GetSuggestionMenuItemCount(error) == 0 ? 3 : GetSuggestionMenuItemCount(error) + 2;
			else
				if(error.Result == SpellCheckOperation.Delete)
					return 2;
			return int.MaxValue;
		}
		protected virtual void SetBeginGroupForFirstStandardItem(DXPopupMenu menu, SpellCheckErrorBase error) {
			int index = CalcFirstMenuItemIndex(error);
			if(menu.Items.Count > index)
				menu.Items[index].BeginGroup = true;
		}
		protected virtual DXMenuItem CreateMenuItem(string caption, EventHandler clickHandler, bool beginGroup, Image image, object error) {
			DXMenuItem result = new DXMenuItem(caption, clickHandler);
			result.BeginGroup = beginGroup;
			result.Image = image;
			result.Tag = error;
			return result;
		}
		protected virtual void AddMenuItem(DXPopupMenu menu, DXMenuItem menuItem, int index) {
			menu.Items.Insert(index, menuItem);
		}
		protected virtual int GetSuggestionMenuItemCount(SpellCheckErrorBase error) {
			int count = SpellChecker.CheckAsYouTypeOptions.SuggestionCount == -1 ? error.Suggestions.Count : SpellChecker.CheckAsYouTypeOptions.SuggestionCount;
			if(count > error.Suggestions.Count)
				count = error.Suggestions.Count;
			return count;
		}
		protected virtual void AddSuggestionMenuItems(DXPopupMenu menu, SpellCheckErrorBase error) {
			int count = GetSuggestionMenuItemCount(error);
			for(int i = 0;i < count;i++) {
				DXMenuItem item = CreateMenuItem(error.Suggestions[i].Suggestion, new EventHandler(OnSuggestionMenuItemClick), false, null, error);
				menu.Items.Insert(i, item);
			}
		}
		protected virtual void AddNoSuggestionMenuItem(DXPopupMenu menu) {
			DXMenuItem item = CreateMenuItem(SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MnuNoSuggestionsCaption), null, false, null, "SpellCheck");
			item.Enabled = false;
			menu.Items.Insert(0, item);
		}
		protected virtual void AddIgnoreAllMenuItem(DXPopupMenu menu, SpellCheckErrorBase error) {
			int menuItemCount = GetSuggestionMenuItemCount(error);
			DXMenuItem item = CreateMenuItem(SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MnuIgnoreAllItemCaption), new EventHandler(OnIgnoreAllMenuItemClick), true, null, error);
			menu.Items.Insert(menuItemCount == 0 ? 1 : menuItemCount, item);
		}
		protected virtual void AddAddToDictionaryMenuItem(DXPopupMenu menu, SpellCheckErrorBase error) {
			DXMenuItem item = CreateMenuItem(SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MnuAddtoDictionaryCaption), new EventHandler(OnAddToDictionaryMenuItemClick), false, null, error);
			item.Enabled = spellChecker.SearchStrategy.DictionaryHelper.GetCustomDictionary(SpellChecker.Culture) != null;
			menu.Items.Insert(GetSuggestionMenuItemCount(error) == 0 ? 2 : GetSuggestionMenuItemCount(error) + 1, item);
		}
		protected virtual void AddErrorMenuItems(DXPopupMenu menu, SpellCheckErrorBase error) {
			if(error.RulesController.IsNotInDictionaryWordError(error)) {
				if(GetSuggestionMenuItemCount(error) > 0)
					AddSuggestionMenuItems(menu, error);
				else
					AddNoSuggestionMenuItem(menu);
				AddIgnoreAllMenuItem(menu, error);
				AddAddToDictionaryMenuItem(menu, error);
			}
			else
				if(error.RulesController.IsRepeatedWordError(error)) {
					DXMenuItem item = CreateMenuItem(SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MnuDeleteRepeatedWord), new EventHandler(OnDeleteMenuItemClick), false, null, error);
					menu.Items.Insert(0, item);
					item = CreateMenuItem(SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MnuIgnoreRepeatedWord), new EventHandler(OnIgnoreRepeatedWordMenuItemClick), false, null, error);
					menu.Items.Insert(1, item);
				}
		}
		protected virtual void AddSpellCheckMenuItem(DXPopupMenu menu, Control popupControl) {
			DXMenuItem item = null;
			Icon icon = ResourceImageHelper.CreateIconFromResources("DevExpress.XtraSpellChecker.Images.SpellChecker.ico", System.Reflection.Assembly.GetExecutingAssembly());
			try {
				Bitmap bitMap = icon.ToBitmap();
				item = CreateMenuItem(SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.MnuItemCaption), new EventHandler(OnSpellCheckMenuItemClick), true, bitMap, "SpellCheck");
				item.Enabled = !String.IsNullOrEmpty(popupControl.Text) && SpellChecker.GetCanCheckText(popupControl) && SpellChecker.CanCheck(popupControl);
			}
			finally {
				icon.Dispose();
			}
			AddMenuItem(menu, item, menu.Items.Count);
			item.Tag = "SpellCheck";
		}
		protected Control PopupControl { get { return popupControl; } set { popupControl = value; } }
		public virtual void UpdateMenuItems(DXPopupMenu menu, SpellCheckErrorBase error, Control popupControl) {
			menu.Items.BeginUpdate();
			try {
				PopupControl = popupControl;
				RemoveSpellCheckMenuItems(menu);
				AddSpellCheckMenuItems(menu, error, popupControl);
			}
			finally {
				menu.Items.EndUpdate();
			}
		}
		public void Dispose() {
			popupControl = null;
			spellChecker = null;
		}
		#region EventHandlers
		protected virtual void OnSpellCheckMenuItemClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			item.Collection.Remove(item);
			item.Dispose();
			SpellChecker.Check(PopupControl);
		}
		protected virtual void OnSuggestionMenuItemClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			SpellCheckErrorBase currentError = (SpellCheckErrorBase)item.Tag;
			currentError.Suggestion = item.Caption;
			Manager.OperationsManager.ReplaceWord(currentError);
		}
		protected virtual void OnDeleteMenuItemClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			SpellCheckErrorBase currentError = (SpellCheckErrorBase)item.Tag;
			Manager.OperationsManager.Delete(currentError);
		}
		protected virtual void OnIgnoreRepeatedWordMenuItemClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			SpellCheckErrorBase currentError = (SpellCheckErrorBase)item.Tag;
			Manager.OperationsManager.IgnoreOnce(currentError.StartPosition, currentError.FinishPosition, currentError.WrongWord);
		}
		protected virtual void OnIgnoreAllMenuItemClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			SpellCheckErrorBase currentError = (SpellCheckErrorBase)item.Tag;
			Manager.OperationsManager.IgnoreAllWord(currentError.WrongWord);
		}
		protected virtual void OnAddToDictionaryMenuItemClick(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			SpellCheckErrorBase currentError = (SpellCheckErrorBase)item.Tag;
			Manager.OperationsManager.AddToDictionary(currentError.WrongWord, currentError.Culture);
		}
		#endregion
	}
}
namespace DevExpress.XtraSpellChecker.Forms {
	public class SpellingFormsManager : IDisposable {
		SpellChecker spellChecker = null;
		FormsCollection storedForms = new FormsCollection();
		SpellingFormBase spellCheckForm;
		SpellingOptionsForm optionsForm = null;
		CustomDictionariesForm editDictionaryForm = null;
		SpellingFormHelper spellingFormHelper = null;
		public SpellingFormsManager(SpellChecker spellChecker) {
			this.spellChecker = spellChecker;
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		protected virtual FormsCollection StoredForms { get { return storedForms; } }
		public SpellChecker SpellChecker { get { return spellChecker; } }
		protected internal virtual SpellingFormHelper SpellingFormHelper {
			get {
				if(spellingFormHelper == null ||
					spellingFormHelper != null && spellingFormHelper.SpellCheckForm != SpellCheckForm) {
					DestroySpellingFormHelper();
					spellingFormHelper = CreateSpellingFormHelper();
				}
				return spellingFormHelper;
			}
		}
		private void DestroySpellingFormHelper() {
			if(spellingFormHelper != null) {
				spellingFormHelper.Dispose();
				spellingFormHelper = null;
			}
		}
		protected virtual SpellingFormHelper CreateSpellingFormHelper() {
			if(SpellCheckForm.GetType() == typeof(SpellingWordStyleForm))
				return CreateSpellingWordStyleFormHelper();
			return CreateSpellingOutlookStyleFormHelper();
		}
		protected virtual SpellingWordStyleFormHelper CreateSpellingWordStyleFormHelper() {
			return new SpellingWordStyleFormHelper(SpellCheckForm as SpellingWordStyleForm);
		}
		protected virtual SpellingOutlookStyleFormHelper CreateSpellingOutlookStyleFormHelper() {
			return new SpellingOutlookStyleFormHelper(SpellCheckForm as SpellingOutlookStyleForm);
		}
		protected virtual bool IsFormStored(Form form) {
			if(form == null)
				return false;
			for(int i = 0;i < StoredForms.Count;i++)
				if(StoredForms[i].Name.Equals(form.GetType().ToString()))
					return true;
			return false;
		}
		public virtual SpellingFormBase SpellCheckForm {
			get {
				if(spellCheckForm == null && IsChecking())
					spellCheckForm = CreateSpellCheckForm();
				return spellCheckForm;
			}
		}
		public virtual bool IsSpellCheckFormVisible() {
			return spellCheckForm != null && !spellCheckForm.IsDisposed && SpellCheckForm.Visible;
		}
		public virtual SpellingOptionsForm OptionsForm {
			get {
				if(optionsForm == null)
					optionsForm = CreateOptionsForm();
				return optionsForm;
			}
		}
		public virtual CustomDictionariesForm EditDictionaryForm {
			get { return editDictionaryForm; }
		}
		protected virtual SpellingFormBase CreateSpellCheckForm() {
			SpellingFormBase form = null;
			if (SpellChecker.SpellingFormType == SpellingFormType.Outlook)
				form = CreateMSOutlookLikeSpellCheckForm();
			else
				if (SpellChecker.IsMsWordFormUsed())
					form = CreateMSWordLikeSpellCheckForm();
				else
					throw new Exception("CUSTOM FORM");
			form.LookAndFeel.Assign(SpellChecker.LookAndFeel);
			return form;
		}
		bool IsChecking() {
			return SpellChecker.SearchStrategy != null && SpellChecker.SearchStrategy.State != StrategyState.None;
		}
		protected virtual SpellingFormBase CreateMSWordLikeSpellCheckForm() {
			return new SpellingWordStyleForm(SpellChecker);
		}
		protected virtual SpellingFormBase CreateMSOutlookLikeSpellCheckForm() {
			return new SpellingOutlookStyleForm(SpellChecker);
		}
		protected virtual SpellingOptionsForm CreateOptionsForm() {
			SpellingOptionsForm form = new SpellingOptionsForm(SpellChecker);
			form.LookAndFeel.Assign(SpellChecker.LookAndFeel);
			return form;
		}
		protected virtual CustomDictionariesForm CreateDictionaryForm(SpellCheckerCustomDictionary dictionary) {
			CustomDictionariesForm form = new CustomDictionariesForm(SpellChecker, dictionary);
			form.LookAndFeel.Assign(SpellChecker.LookAndFeel);
			return form;
		}
		protected virtual FormSerializationInfo ConstructFormSerializartionInfo(Form form) {
			return new FormSerializationInfo(form.GetType().ToString(), form.Left, form.Top, form.Width, form.Height);
		}
		public virtual void AddForm(Form form) {
			if(form == null) return;
			if(IsFormStored(form))
				RemoveForm(form);
			StoredForms.Add(ConstructFormSerializartionInfo(form));
		}
		public virtual void RemoveForm(Form form) {
			for(int i = 0;i < StoredForms.Count;i++)
				if(StoredForms[i].Name.Equals(form.GetType().ToString())) {
					StoredForms.RemoveAt(i);
					break;
				}
		}
		public FormSerializationInfo GetFormSerializationInfo(Form form) {
			if(form == null) return null;
			for(int i = 0;i < StoredForms.Count;i++)
				if(StoredForms[i].Name.Equals(form.GetType().ToString()))
					return StoredForms[i];
			return null;
		}
		protected virtual FormSerializationInfo XtraCreateStoredFormsItem(XtraItemEventArgs e) {
			return StoredForms.CreateStoredFormsItem(e);
		}
		#region Disposing
		public void Dispose() {
			DestroySpellingFormHelper();
			DestroyAllForms();
		}
		public virtual void DestroyAllForms() {
			DestroyOptionsForm();
			DestroyDictionaryForm();
			DestroySpellCheckForm();
		}
		private void DestroySpellCheckForm() {
			if(spellCheckForm != null && !SpellCheckForm.IsDisposed)
				SpellCheckForm.Dispose();
			spellCheckForm = null;
		}
		private void DestroyDictionaryForm() {
			if(editDictionaryForm != null && !EditDictionaryForm.IsDisposed)
				EditDictionaryForm.Dispose();
			editDictionaryForm = null;
		}
		private void DestroyOptionsForm() {
			if(optionsForm != null && !OptionsForm.IsDisposed)
				OptionsForm.Dispose();
			optionsForm = null;
		}
		#endregion
		#region FormsShowing
		public void ShowSpellCheckForm() {
			if(CanShowSpellCheckForm())
				ShowSpellCheckFormCore();
		}
		protected virtual IWin32Window GetDialogOwner() {
			if(SpellChecker.EditControl != null) {
				Form form = SpellChecker.EditControl.FindForm();
				if(form != SpellCheckForm)
					return form;
			}
			return null;
		}
		protected internal IWin32Window DialogOwner {
			get { return GetDialogOwner(); }
		}
		protected virtual bool CanShowSpellCheckForm() {
			SpellingFormShowingEventArgs e = new SpellingFormShowingEventArgs(string.Empty, null); 
			SpellChecker.OnSpellingFormShowing(e);
			return !e.Handled;
		}
		protected virtual void ShowSpellCheckFormCore() {
			SpellCheckForm.Init();
			if(!SpellCheckForm.Visible)
				DevExpress.Utils.Internal.FormTouchUIAdapter.ShowDialog(SpellCheckForm, DialogOwner);
		}
		public DialogResult ShowOptionsForm() {
			if(optionsForm != null && !optionsForm.Visible)
				DestroyOptionsForm();
			if(OptionsForm.Visible)
				return DialogResult.Cancel;
			if(CanShowOptionsForm())
				return ShowOptionsFormCore();
			return DialogResult.Cancel;
		}
		protected virtual bool CanShowOptionsForm() {
			FormShowingEventArgs e = new FormShowingEventArgs();
			SpellChecker.OnOptionsFormShowing(e);
			return !e.Handled;
		}
		protected virtual DialogResult ShowOptionsFormCore() {
			OptionsForm.Init();
			if(!OptionsForm.Visible)
				return DevExpress.Utils.Internal.FormTouchUIAdapter.ShowDialog(OptionsForm, DialogOwner);
			return DialogResult.Cancel;
		}
		public void ShowEditDictionaryForm(SpellCheckerCustomDictionary dictionary) {
			ShowEditDictionaryFormCore(dictionary);
		}
		protected virtual void ShowEditDictionaryFormCore(SpellCheckerCustomDictionary dictionary) {
			editDictionaryForm = CreateDictionaryForm(dictionary);
			EditDictionaryForm.Init();
			DialogResult result = DevExpress.Utils.Internal.FormTouchUIAdapter.ShowDialog(EditDictionaryForm, DialogOwner);
			if (result == DialogResult.OK && EditDictionaryForm.IsModified)
				SpellChecker.RaiseCustomDictionaryChanged();
		}
		#endregion
		public virtual void HideSpellCheckForm(SpellingFormType spellingFormType) {
			if (SpellCheckForm.DialogResult == DialogResult.None)
				SpellCheckForm.DialogResult = DialogResult.OK;
		}
		public virtual void LookAndFeelChanged() {
			if(spellCheckForm != null)
				spellCheckForm.LookAndFeel.Assign(SpellChecker.LookAndFeel);
			if(optionsForm != null)
				optionsForm.LookAndFeel.Assign(SpellChecker.LookAndFeel);
			if(editDictionaryForm != null)
				editDictionaryForm.LookAndFeel.Assign(SpellChecker.LookAndFeel);
		}
	}
	public class FormsCollection : List<FormSerializationInfo> {
		int GetIntValueFromObject(object v) {
			try {
				return Convert.ToInt32(v);
			}
			catch(InvalidCastException) {
				return 0;
			}
		}
		protected internal virtual FormSerializationInfo CreateStoredFormsItem(XtraItemEventArgs e) {
			string name = string.Empty;
			int left = 0;
			int top = 0;
			int width = 0;
			int height = 0;
			for(int i = 0;i < e.Item.ChildProperties.Count;i++) {
				if(string.Compare(e.Item.ChildProperties[i].Name, "Name", true) == 0) {
					if(e.Item.ChildProperties[i].Value != null)
						name = e.Item.ChildProperties[i].Value.ToString();
					continue;
				}
				if(string.Compare(e.Item.ChildProperties[i].Name, "Left", true) == 0) {
					left = GetIntValueFromObject(e.Item.ChildProperties[i].Value);
					continue;
				}
				if(string.Compare(e.Item.ChildProperties[i].Name, "Top", true) == 0) {
					top = GetIntValueFromObject(e.Item.ChildProperties[i].Value);
					continue;
				}
				if(string.Compare(e.Item.ChildProperties[i].Name, "Width", true) == 0) {
					width = GetIntValueFromObject(e.Item.ChildProperties[i].Value);
					continue;
				}
				if(string.Compare(e.Item.ChildProperties[i].Name, "Height", true) == 0) {
					height = GetIntValueFromObject(e.Item.ChildProperties[i].Value);
					continue;
				}
			}
			FormSerializationInfo info = new FormSerializationInfo(name, left, top, width, height);
			Add(info);
			return info;
		}
	}
	public class FormSerializationInfo {
		string name;
		int left;
		int top;
		int width;
		int height;
		public FormSerializationInfo(string name, int left, int top, int width, int height) {
			this.name = name;
			this.left = left;
			this.top = top;
			this.width = width;
			this.height = height;
		}
		[XtraSerializableProperty()]
		public string Name { get { return name; } }
		[XtraSerializableProperty()]
		public int Left { get { return left; } }
		[XtraSerializableProperty()]
		public int Top { get { return top; } }
		[XtraSerializableProperty()]
		public int Width { get { return width; } }
		[XtraSerializableProperty()]
		public int Height { get { return height; } }
	}
}
