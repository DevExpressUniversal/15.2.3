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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.SpellChecker.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Forms;
using DevExpress.XtraSpellChecker.Localization;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.XtraSpellChecker.Rules;
using DevExpress.XtraSpellChecker.Strategies;
using DevExpress.Xpf.Core;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using System.Threading.Tasks;
using PlatformSpecificRoutedEventHandler = System.Windows.RoutedEventHandler;
using PlatformSpecificRoutedEventArgs = System.Windows.RoutedEventArgs;
namespace DevExpress.Xpf.SpellChecker {
	[DXToolboxBrowsable(false)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724")]
	public class SpellChecker : SpellCheckerBase, ISpellChecker {
		SpellingFormsManager formsManager = null;
		SpellCheckClientRouterBase router = null;
		Control checkedControl = null;
		ContainerSearchStrategyBase parentStrategy;
		Control parentContainer = null;
		SpellCheckMode spellCheckMode;
		static WeakKeyDictionary<DependencyObject, OptionsSpelling> spellCheckerOptions = new WeakKeyDictionary<DependencyObject, OptionsSpelling>();
		bool keepFocusOnControl = true;
		public SpellChecker()
			: base() {
		}
		protected virtual SpellingFormsManager CreateFormsManager() {
			return new SpellingFormsManager(this);
		}
#if !SL
	[DevExpressXpfSpellCheckerLocalizedDescription("SpellCheckerFormsManager")]
#endif
		public SpellingFormsManager FormsManager {
			get {
				if (formsManager == null)
					formsManager = CreateFormsManager();
				return formsManager;
			}
		}
		[
#if !SL
	DevExpressXpfSpellCheckerLocalizedDescription("SpellCheckerParentContainer"),
#endif
 Category(SRCategryNames.SpellChecker)]
		public Control ParentContainer {
			get { return parentContainer; }
			set {
				if (parentContainer != value)
					parentContainer = value;
			}
		}
#if !SL
	[DevExpressXpfSpellCheckerLocalizedDescription("SpellCheckerIsChecking")]
#endif
		public new bool IsChecking { get { return base.IsChecking; } }
#if !SL
	[DevExpressXpfSpellCheckerLocalizedDescription("SpellCheckerSpellCheckMode")]
#endif
		public SpellCheckMode SpellCheckMode {
			get {
				return spellCheckMode;
			}
			set {
				if (SpellCheckMode == value)
					return;
				spellCheckMode = value;
				RaiseSpellCheckModeChanged();
			}
		}
		[DefaultValue(true)]
		public bool KeepFocusOnControl { get { return keepFocusOnControl; } set { keepFocusOnControl = value; } }
		static WeakKeyDictionary<DependencyObject, OptionsSpelling> SpellCheckerOptions { get { return spellCheckerOptions; } }
		#region SpellCheckModeChanged
		EventHandler onSpellCheckModeChanged;
		public event EventHandler SpellCheckModeChanged { add { onSpellCheckModeChanged += value; } remove { onSpellCheckModeChanged -= value; } }
		void RaiseSpellCheckModeChanged() {
			EventHandler handler = onSpellCheckModeChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region CultureChanged
		EventHandler onCultureChanged;
		public event EventHandler CultureChanged { add { onCultureChanged += value; } remove { onCultureChanged -= value; } }
		void RaiseCultureChanged() {
			EventHandler handler = onCultureChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		public IIgnoreList GetIgnoreList(Control control) {
			return base.GetIgnoreListCore(control);
		}
		protected override void OnCultureChanged() {
			base.OnCultureChanged();
			RaiseCultureChanged();
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
		protected internal new bool IsStrategyExists {
			get { return base.IsStrategyExists; }
		}
		internal static OptionsSpelling GetSpellCheckerOptions(DependencyObject obj) {
			if (obj == null)
				return null;
			OptionsSpelling result;
			if (!SpellCheckerOptions.TryGetValue(obj, out result)) {
				result = new OptionsSpelling();
				SpellCheckerOptions.Add(obj, result);
			}
			return result;
		}
		protected internal string CalculateOptionsKeys(Control control) {
			return control.GetType().FullName + control.GetHashCode().ToString();
		}
		protected virtual OptionsSpelling CreateDefaultOptions() {
			return new OptionsSpelling();
		}
		internal static void SetSpellCheckerOptions(DependencyObject obj, OptionsSpelling options) {
			if (obj == null)
				return;
			if (options == null)
				SpellCheckerOptions.Remove(obj);
			else {
				if (SpellCheckerOptions.ContainsKey(obj))
					SpellCheckerOptions[obj].Assign(options);
				else
					SpellCheckerOptions.Add(obj, options);
			}
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
			if (control == null)
				return false;
			if (!CanCheckControlQuiet(control))
				return false;
			return !IsChecking && IsControlEditable(control);
		}
		protected internal virtual new Control EditControl {
			get { return base.EditControl as Control; }
			set {
				base.EditControl = value;
			}
		}
		protected override void OnEditControlModified() {
			base.OnEditControlModified();
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
			if (textController != null) {
				if (ShouldCheckSelectedFirst(textController))
					return new AgPartTextSearchStrategy(this, textController, textController.SelectionStart, textController.SelectionFinish);
				else
					if (ShouldCheckFromCursorPos(textController))
						return new AgPartSilentTextSearchStrategy(this, textController, textController.SelectionStart);
					else
						return new AgSearchStrategy(this, textController);
			}
			else
				return new AgSearchStrategy(this, new EmptyEditControlTextControlController());
		}
		protected virtual ContainerSearchStrategyBase ParentStrategy {
			get { return this.parentStrategy; }
			set { this.parentStrategy = value; }
		}
		protected virtual SearchStrategy CreateTextSearchStrategy(object editControl, ContainerSearchStrategyBase parentStrategy) {
			ISpellCheckTextControlController textController = CreateTextController() as ISpellCheckTextControlController;
			if (textController != null) {
				return new AgSearchStrategy(this, textController, parentStrategy);
			}
			else
				return new SimpleTextSearchStrategy(this, new SimpleTextController());
		}
		protected internal virtual bool IsMsWordFormUsed() {
			bool result = SpellingFormType == SpellingFormType.Word;
			return result;
		}
		protected virtual SpellCheckClientRouterBase GetClientRouterInstance() {
			if (IsMsWordFormUsed())
				return new SpellCheckMSWordLikeClientRouter(SearchStrategy);
			else
				return new SpellCheckMSOutlookLikeClientRouter(SearchStrategy);
		}
		protected virtual SpellCheckClientRouterBase Router {
			get { return router; }
			set {
				if (Router != null) {
					Router.Dispose();
					router = null;
				}
				router = value;
			}
		}
		protected internal override int SuggestionCount {
			get {
				return 5;
			}
		}
		public override List<SpellCheckerCommand> GetCommandsByError(SpellCheckErrorBase error) {
			return GetCommandsByError(SearchStrategy.OperationsManager, error);
		}
		protected override void OnDictionaryNotLoaded(ISpellCheckerDictionary dict) {
			string message = SpellCheckerLocalizer.GetString(SpellCheckerStringId.Msg_NotLoadedDictionaryException);
			throw new NotLoadedDictionaryException(String.Format(message, dict.Culture.Name));
		}
		#region Check Control
		protected override void DoBeforeCheck() {
			base.DoBeforeCheck();
			Router = GetClientRouterInstance();
			System.Globalization.CultureInfo editorCulture = EditControl != null ? SpellingSettings.GetCulture(EditControl) : null;
			SearchStrategy.Culture = editorCulture != null ? editorCulture : Culture;
		}
		protected internal override void DoAfterCheck(SearchStrategy strategy) {
			if (!strategy.ThreadChecking) {
				CheckedControl = null;
				if (Router != null) {
					Router.Dispose();
					Router = null;
				}
				if (SearchStrategy.TextController is IDisposable)
					(SearchStrategy.TextController as IDisposable).Dispose();
				SearchStrategy.TextController = null;
				DestroySearchStrategy();
				base.DoAfterCheck(strategy);
			}
			else {
				strategy.Dispose();
			}
		}
		protected internal virtual void DoAfterCheck() {
			SetSpellCheckerOptions(CheckedControl, OptionsSpelling);
			CheckedControl = null;
			DoAfterCheck(SearchStrategy);
		}
		protected internal virtual void DoAfterCheck(Control editControl) {
			DoAfterCheck();
		}
		protected virtual void DoBeforeCheck(Control editControl) {
			EditControl = editControl;
			CheckedControl = editControl;
			DoBeforeCheck();
			OptionsSpelling.CombineOptions(GetSpellCheckerOptions(EditControl));
		}
		public virtual void Check(Control editControl) {
			try {
				if (!CanCheck(editControl))
					return;
				SearchStrategy.AfterCheck -= OnAfterCheck;
				DoBeforeCheck(editControl);
				SearchStrategy.AfterCheck += OnAfterCheck;
				SearchStrategy.Check();
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		public new Task<string> Check(string text) {
			try {
				SearchStrategy.AfterCheck -= OnAfterCheck;
				DoBeforeCheck();
				AgSearchStrategy strategy = SearchStrategy as AgSearchStrategy;
				if (strategy == null)
					return null;
				strategy.AfterCheck += OnAfterCheck;
				strategy.Text = text;
				return strategy.CheckAndGetResult();
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
				return null;
			}
		}
		void OnAfterCheck(object sender, EventArgs e) {
			SearchStrategy searchStrategy = sender as SearchStrategy;
			searchStrategy.AfterCheck -= OnAfterCheck;
			if (Object.ReferenceEquals(searchStrategy, SearchStrategy))
				DoAfterCheck();
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
		public virtual void Check(ISpellCheckTextControlController controller) {
			try {
				if (!CanCheck())
					return;
				DoBeforeCheck();
				try {
					SearchStrategy.Check();
				}
				finally {
					SearchStrategy.TextController = null;
					DoAfterCheck(SearchStrategy);
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		#endregion
		#region CheckContainer
		public virtual void CheckContainer(FrameworkElement container) {
			try {
				XpfControlContainerSearchStrategy strategy = new XpfControlContainerSearchStrategy(this, container);
				strategy.AfterCheck += new EventHandler(ContainerSearchStrategy_AfterCheck);
				ParentStrategy = strategy;
				ParentStrategy.Check();
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		void ContainerSearchStrategy_AfterCheck(object sender, EventArgs e) {
			try {
				XpfControlContainerSearchStrategy searchStrategy = (XpfControlContainerSearchStrategy)sender;
				searchStrategy.AfterCheck -= new EventHandler(ContainerSearchStrategy_AfterCheck);
				searchStrategy.Dispose();
				ParentStrategy = null;
			}
			catch (Exception ex) {
				if (HandleException(ex))
					throw;
			}
		}
		public virtual void CheckContainer() {
			try {
				if (ParentContainer != null)
					CheckContainer(ParentContainer);
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		protected internal override void Check(object editControl, ContainerSearchStrategyBase parentStrategy) {
			try {
				EditControl = editControl as Control;
				if (EditControl == null || !CanCheck(EditControl)) {
					parentStrategy.FinishControlChecking();
					return;
				}
				base.DoBeforeCheck();
				CheckedControl = EditControl;
				OptionsSpelling.CombineOptions(GetSpellCheckerOptions(EditControl));
				try {
					SetSearchStrategyCore(CreateTextSearchStrategy(editControl, parentStrategy));
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
			try {
				SearchStrategy.AfterCheck -= new EventHandler(SearchStrategy_AfterCheck);
				DoAfterCheck(EditControl);
				ParentStrategy.FinishControlChecking();
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		#endregion
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Category(SRCategryNames.SpellChecker)
		]
		protected internal override bool CanHandleErrorVisually() {
			return true;
		}
		protected internal void SLOnFinishCheckingMainPart(FinishCheckingMainPartEventArgs e) {
			base.OnFinishCheckingMainPart(e);
		}
		protected internal bool IsOnFinishCheckingMainPartAssigned() {
			return base.IsOnFinishCheckingMainPartEventAssigned();
		}
		protected override OptionsSpellingBase CreateOptionsSpelling() {
			return new OptionsSpelling();
		}
		[
#if !SL
	DevExpressXpfSpellCheckerLocalizedDescription("SpellCheckerOptionsSpelling"),
#endif
			Category(SRCategryNames.SpellChecker),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
			XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public virtual new OptionsSpelling OptionsSpelling {
			get { return base.OptionsSpelling as OptionsSpelling; }
		}
		#region SpellingFormShowing
		SpellingFormShowingEventHandler spellingFormShowing = null;
		[Category(SRCategryNames.SpellChecker)]
		public event SpellingFormShowingEventHandler SpellingFormShowing {
			add { spellingFormShowing += value; }
			remove { spellingFormShowing -= value; }
		}
		protected internal virtual void OnSpellingFormShowing(SpellingFormShowingEventArgs e) {
			RaiseSpellingFormShowing(e);
		}
		protected virtual void RaiseSpellingFormShowing(SpellingFormShowingEventArgs e) {
			SpellingFormShowingEventHandler handler = spellingFormShowing;
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region OptionsFormShowing
		FormShowingEventHandler optionsFormShowing = null;
		[Category(SRCategryNames.SpellChecker)]
		public event FormShowingEventHandler OptionsFormShowing {
			add { optionsFormShowing += value; }
			remove { optionsFormShowing -= value; }
		}
		protected internal virtual void OnOptionsFormShowing(FormShowingEventArgs e) {
			RaiseOptionsFormShowing(e);
		}
		protected virtual void RaiseOptionsFormShowing(FormShowingEventArgs e) {
			FormShowingEventHandler handler = optionsFormShowing;
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region CheckCompleteFormShowing
		FormShowingEventHandler checkCompleteFormShowing = null;
		[Category(SRCategryNames.SpellChecker)]
		public event FormShowingEventHandler CheckCompleteFormShowing {
			add { checkCompleteFormShowing += value; }
			remove { checkCompleteFormShowing -= value; }
		}
		protected internal virtual void OnCheckCompleteFormShowing(FormShowingEventArgs e) {
			RaiseCheckCompleteFormShowing(e);
		}
		protected virtual void RaiseCheckCompleteFormShowing(FormShowingEventArgs e) {
			FormShowingEventHandler handler = checkCompleteFormShowing;
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region ISpellChecker Members
		ICheckSpellingResult ISpellChecker.CheckText(object control, string text, int index, System.Globalization.CultureInfo culture) {
			try {
				SaveComponentOptions();
				OptionsSpelling.CombineOptions(GetSpellCheckerOptions(control as Control));
				try {
					return CheckText(text, index, culture);
				}
				finally {
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
				SaveComponentOptions();
				OptionsSpelling.CombineOptions(GetSpellCheckerOptions(control as Control));
				try {
					return Check(controller, from, to);
				}
				finally {
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
		void ISpellChecker.AddToDictionary(string word, System.Globalization.CultureInfo culture) {
			AddToDictionary(word, culture);
		}
		bool ISpellChecker.CanAddToDictionary(System.Globalization.CultureInfo culture) {
			return CanAddToDictionary(culture);
		}
		void ISpellChecker.Ignore(object control, string word, Position start, Position end) {
			Ignore(control, word, start, end);
		}
		void ISpellChecker.IgnoreAll(object control, string word) {
			IgnoreAll(control, word);
		}
		void ISpellChecker.RegisterIgnoreList(object control, IIgnoreList ignoreList) {
			RegisterIgnoreList(control, ignoreList);
		}
		void ISpellChecker.UnregisterIgnoreList(object control) {
			UnregisterIgnoreList(control);
		}
		IOptionsSpellings ISpellChecker.GetOptions(object control) {
			try {
				Control checkedControl = control as Control;
				if (checkedControl != null)
					return GetSpellCheckerOptions(checkedControl);
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
			return null;
		}
		string[] ISpellChecker.GetSuggestions(string word, System.Globalization.CultureInfo culture) {
			return GetSuggestions(word, culture).ToStringArray();
		}
		#region CustomDictionaryChanged
		EventHandler onCustomDictionaryChanged;
		event EventHandler ISpellChecker.CustomDictionaryChanged { add { onCustomDictionaryChanged += value; } remove { onCustomDictionaryChanged -= value; } }
		protected internal void RaiseCustomDictionaryChanged() {
			if (onCustomDictionaryChanged != null)
				onCustomDictionaryChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
	}
	public class OptionsSpelling : OptionsSpellingBase, INotifyPropertyChanged {
		#region INotifyPropertyChanged.PropertyChanged
		PropertyChangedEventHandler onPropertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		void OnPropertyChanged(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		#region CheckFromCursorPos
		public override DefaultBoolean CheckFromCursorPos {
			get { return base.CheckFromCursorPos; }
			set {
				if (value != CheckFromCursorPos) {
					base.CheckFromCursorPos = value;
					OnPropertyChanged("CheckFromCursorPos");
				}
			}
		}
		#endregion
		#region CheckSelectedTextFirst
		public override DefaultBoolean CheckSelectedTextFirst {
			get { return base.CheckSelectedTextFirst; }
			set {
				if (value != CheckSelectedTextFirst) {
					base.CheckSelectedTextFirst = value;
					OnPropertyChanged("CheckSelectedTextFirst");
				}
			}
		}
		#endregion
		#region IgnoreEmails
		public override DefaultBoolean IgnoreEmails {
			get { return base.IgnoreEmails; }
			set {
				if (value != IgnoreEmails) {
					base.IgnoreEmails = value;
					OnPropertyChanged("IgnoreEmails");
				}
			}
		}
		#endregion
		#region IgnoreMarkupTags
		public override DefaultBoolean IgnoreMarkupTags {
			get { return base.IgnoreMarkupTags; }
			set {
				if (value != IgnoreMarkupTags) {
					base.IgnoreMarkupTags = value;
					OnPropertyChanged("IgnoreMarkupTags");
				}
			}
		}
		#endregion
		#region IgnoreMixedCaseWords
		public override DefaultBoolean IgnoreMixedCaseWords {
			get { return base.IgnoreMixedCaseWords; }
			set {
				if (value != IgnoreMixedCaseWords) {
					base.IgnoreMixedCaseWords = value;
					OnPropertyChanged("IgnoreMixedCaseWords");
				}
			}
		}
		#endregion
		#region IgnoreRepeatedWords
		public override DefaultBoolean IgnoreRepeatedWords {
			get { return base.IgnoreRepeatedWords; }
			set {
				if (value != IgnoreRepeatedWords) {
					base.IgnoreRepeatedWords = value;
					OnPropertyChanged("IgnoreRepeatedWords");
				}
			}
		}
		#endregion
		#region IgnoreUpperCaseWords
		public override DefaultBoolean IgnoreUpperCaseWords {
			get { return base.IgnoreUpperCaseWords; }
			set {
				if (value != IgnoreUpperCaseWords) {
					base.IgnoreUpperCaseWords = value;
					OnPropertyChanged("IgnoreUpperCaseWords");
				}
			}
		}
		#endregion
		#region IgnoreUrls
		[Obsolete("This property has become obsolete. Use the IgnoreUri property instead.")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override DefaultBoolean IgnoreUrls {
#pragma warning disable 0618
			get { return base.IgnoreUrls; }
			set {
				if (value != IgnoreUrls) {
					base.IgnoreUrls = value;
					OnPropertyChanged("IgnoreUrls");
				}
			}
#pragma warning restore 0618
		}
		#endregion
		#region IgnoreUri
		public override DefaultBoolean IgnoreUri {
			get { return base.IgnoreUri; }
			set {
				if (value != IgnoreUri) {
					base.IgnoreUri = value;
					OnPropertyChanged("IgnoreUri");
				}
			}
		}
		#endregion
		#region IgnoreWordsWithNumbers
		public override DefaultBoolean IgnoreWordsWithNumbers {
			get { return base.IgnoreWordsWithNumbers; }
			set {
				if (value != IgnoreWordsWithNumbers) {
					base.IgnoreWordsWithNumbers = value;
					OnPropertyChanged("IgnoreWordsWithNumbers");
				}
			}
		}
		#endregion
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
		[Obsolete("This method has become obsolete. Use the IsIgnoreUri method instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal virtual new bool IsIgnoreUrls() {
			return base.IsIgnoreUrls();
		}
		protected internal virtual new bool IsIgnoreUri() {
			return base.IsIgnoreUri();
		}
		protected internal virtual new bool IsIgnoreEmails() {
			return base.IsIgnoreEmails();
		}
	}
	public class AgSpellCheckerChangeCommand : SpellCheckerChangeCommand {
		SpellChecker spellChecker;
		public AgSpellCheckerChangeCommand(TextOperationsManager operationsManager, SpellChecker spellChecker)
			: base(operationsManager) {
			this.spellChecker = spellChecker;
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
	public class AgSpellCheckerDeleteCommand : SpellCheckerChangeCommand {
		SpellChecker spellChecker;
		public AgSpellCheckerDeleteCommand(TextOperationsManager operationsManager, SpellChecker spellChecker)
			: base(operationsManager) {
			this.spellChecker = spellChecker;
		}
		public SpellChecker SpellChecker {
			get { return spellChecker; }
		}
		public override string Caption {
			get {
				SpellCheckerResLocalizer localizer = new SpellCheckerResLocalizer();
				return localizer.GetLocalizedString(SpellCheckerStringId.Menu_DeleteRepeatedWord);
			}
		}
	}
	public class AgSpellCheckerAddToDictionaryCommand : SpellCheckerCommand {
		public AgSpellCheckerAddToDictionaryCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.AddToDictionary; } }
		public override string Caption {
			get {
				SpellCheckerResLocalizer localizer = new SpellCheckerResLocalizer();
				return localizer.GetLocalizedString(SpellCheckerStringId.Menu_AddToDictionaryCaption);
			}
		}
		public override bool Enabled {
			get {
				return false;
			}
		}
	}
	public class AgSpellCheckerIgnoreCommand : SpellCheckerCommand {
		public AgSpellCheckerIgnoreCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.IgnoreOnce(Start, Finish, this.Word);
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.Ignore; } }
		public override string Caption {
			get {
				SpellCheckerResLocalizer localizer = new SpellCheckerResLocalizer();
				return localizer.GetLocalizedString(SpellCheckerStringId.Menu_IgnoreRepeatedWord);
			}
		}
	}
	public class AgSpellCheckerIgnoreAllCommand : SpellCheckerCommand {
		public AgSpellCheckerIgnoreAllCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.IgnoreAll(Word);
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.IgnoreAll; } }
		public override string Caption {
			get {
				SpellCheckerResLocalizer localizer = new SpellCheckerResLocalizer();
				return localizer.GetLocalizedString(SpellCheckerStringId.Menu_IgnoreAllItemCaption);
			}
		}
	}
	public class AgSpellCheckerNoSuggestionsCommand : SpellCheckerNoSuggestionsCommand {
		public AgSpellCheckerNoSuggestionsCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		public override string Caption {
			get {
				SpellCheckerResLocalizer localizer = new SpellCheckerResLocalizer();
				return localizer.GetLocalizedString(SpellCheckerStringId.Menu_NoSuggestionsCaption);
			}
		}
	}
}
namespace DevExpress.XtraSpellChecker.Native {
	public abstract class SpellCheckerHandlerBase : IDisposable {
		DevExpress.Xpf.SpellChecker.SpellChecker spellChecker;
		protected SpellCheckerHandlerBase(DevExpress.Xpf.SpellChecker.SpellChecker spellChecker) {
			this.spellChecker = spellChecker;
		}
		public DevExpress.Xpf.SpellChecker.SpellChecker SpellChecker { get { return this.spellChecker; } }
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
		public SpellCheckerOnDemandHandler(DevExpress.Xpf.SpellChecker.SpellChecker spellChecker) : base(spellChecker) { }
		protected override void DoBeforeCheck() {
		}
		protected override void DoCheckCore() {
		}
		protected override void DoAfterCheck() {
		}
	}
}
namespace DevExpress.XtraSpellChecker.Forms {
	static class WinAPI {
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		internal static extern IntPtr GetActiveWindow();
	}
	#region SpellingFormsManager
	public class SpellingFormsManager : IDisposable {
		#region Fields
		DevExpress.Xpf.SpellChecker.SpellChecker spellChecker;
		SpellingFormHelper spellingFormHelper;
		SpellingControlBase spellCheckForm;
		SpellingOptionsControl optionsForm;
		CustomDictionaryControl editDictionaryForm;
		#endregion
		public SpellingFormsManager(DevExpress.Xpf.SpellChecker.SpellChecker spellChecker) {
			this.spellChecker = spellChecker;
		}
		#region Properties
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public DevExpress.Xpf.SpellChecker.SpellChecker SpellChecker { get { return spellChecker; } }
		protected internal virtual SpellingFormHelper SpellingFormHelper {
			get {
				if (spellingFormHelper == null || spellingFormHelper != null && spellingFormHelper.SpellCheckForm != SpellCheckForm) {
					DestroySpellingFormHelper();
					spellingFormHelper = CreateSpellingFormHelper();
				}
				return spellingFormHelper;
			}
		}
		public virtual SpellingControlBase SpellCheckForm {
			get {
				if (spellCheckForm == null)
					spellCheckForm = CreateSpellCheckForm();
				return spellCheckForm;
			}
		}
		public virtual SpellingOptionsControl OptionsForm {
			get {
				if (optionsForm == null)
					optionsForm = CreateOptionsForm();
				return optionsForm;
			}
		}
		public virtual CustomDictionaryControl EditDictionaryForm {
			get {
				if (editDictionaryForm == null)
					editDictionaryForm = CreateDictionaryForm(spellChecker.DictionaryHelper.GetCustomDictionary(SpellChecker.SearchStrategy.ActualCulture));
				return editDictionaryForm;
			}
		}
		internal bool IsSpellCheckWindowOpen { get { return spellCheckForm != null && FloatingContainer.GetDialogOwner(spellCheckForm) != null; } }
		internal bool IsOptionsWindowOpen { get { return optionsForm != null && FloatingContainer.GetDialogOwner(optionsForm) != null; } }
		internal bool IsEditDictionaryWindowOpen { get { return editDictionaryForm != null && FloatingContainer.GetDialogOwner(editDictionaryForm) != null; } }
		#endregion
		#region Events
		#region OptionsFormClosed
		WindowClosedEventHandler optionsFormClosedEventHandler;
		public event WindowClosedEventHandler OptionsFormClosed { add { optionsFormClosedEventHandler += value; } remove { optionsFormClosedEventHandler -= value; } }
		protected internal virtual void RaiseOptionsFormClosed(bool dialogResult) {
			WindowClosedEventHandler handler = optionsFormClosedEventHandler;
			if (handler != null)
				handler(this, new WindowClosedEventArgs(dialogResult));
		}
		#endregion
		#region SpellCheckFormResultChanged
		SpellingFormResultChangedEventHandler spellingFormResultChanged;
		public event SpellingFormResultChangedEventHandler SpellCheckFormResultChanged { add { spellingFormResultChanged += value; } remove { spellingFormResultChanged -= value; } }
		protected virtual void RaiseSpellCheckFormResultChanged(SpellingFormResultChangedEventArgs e) {
			SpellingFormResultChangedEventHandler handler = spellingFormResultChanged;
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region SpellingOutlookStyleControlCreated
		SpellingOutlookStyleEventHandler onSpellingOutlookStyleControlCreated;
		public event SpellingOutlookStyleEventHandler SpellingOutlookStyleControlCreated {
			add { onSpellingOutlookStyleControlCreated += value; }
			remove { onSpellingOutlookStyleControlCreated -= value; }
		}
		protected virtual void OnSpellingOutlookStyleControlCreated(SpellingOutlookStyleEventArgs e) {
			if (onSpellingOutlookStyleControlCreated != null)
				onSpellingOutlookStyleControlCreated(this, e);
		}
		#endregion
		#region SpellingWordStyleControlCreated
		SpellingWordStyleEventHandler onSpellingWordStyleControlCreated;
		public event SpellingWordStyleEventHandler SpellingWordStyleControlCreated {
			add { onSpellingWordStyleControlCreated += value; }
			remove { onSpellingWordStyleControlCreated -= value; }
		}
		protected virtual void OnSpellingWordStyleControlCreated(SpellingWordStyleEventArgs e) {
			if (onSpellingWordStyleControlCreated != null)
				onSpellingWordStyleControlCreated(this, e);
		}
		#endregion
		#region OptionsWindowShowing
		SpellingOptionsEventHandler onSpellingOptionsControlCreated;
		public event SpellingOptionsEventHandler SpellingOptionsControlCreated { add { onSpellingOptionsControlCreated += value; } remove { onSpellingOptionsControlCreated -= value; } }
		protected virtual void OnSpellingOptionsControlCreated(SpellingOptionsEventArgs e) {
			if (onSpellingOptionsControlCreated != null)
				onSpellingOptionsControlCreated(this, e);
		}
		#endregion
		#region EditDictionaryWindowShowing
		CustomDictionaryEventHandler onCustomDictionaryControlCreated;
		public event CustomDictionaryEventHandler CustomDictionaryControlCreated { add { onCustomDictionaryControlCreated += value; } remove { onCustomDictionaryControlCreated -= value; } }
		protected virtual void OnCustomDictionaryControlCreated(CustomDictionaryEventArgs e) {
			if (onCustomDictionaryControlCreated != null)
				onCustomDictionaryControlCreated(this, e);
		}
		#endregion
		#endregion
		protected virtual void DestroySpellingFormHelper() {
			if (spellingFormHelper != null) {
				spellingFormHelper.Dispose();
				spellingFormHelper = null;
			}
		}
		protected virtual SpellingFormHelper CreateSpellingFormHelper() {
			if (SpellChecker.SpellingFormType == SpellingFormType.Outlook)
				return CreateSpellingOutlookStyleFormHelper();
			return CreateSpellingWordStyleFormHelper();
		}
		protected virtual SpellingWordStyleFormHelper CreateSpellingWordStyleFormHelper() {
			return new SpellingWordStyleFormHelper(SpellCheckForm as SpellingWordStyleControl);
		}
		protected virtual SpellingOutlookStyleFormHelper CreateSpellingOutlookStyleFormHelper() {
			return new SpellingOutlookStyleFormHelper(SpellCheckForm as SpellingOutlookStyleControl);
		}
		protected virtual bool IsFormStored(object form) {
			return false;
		}
		protected virtual void ShowOptionsWindow() {
			string caption = SpellCheckerLocalizer.GetString(SpellCheckerStringId.Form_SpellingOptions_Caption);
			SpellingOptionsEventArgs args = new SpellingOptionsEventArgs(OptionsForm);
			OnSpellingOptionsControlCreated(args);
			if (args.Control != null) {
				optionsForm = args.Control as SpellingOptionsControl;
			}
			ShowWindow(SpellCheckForm, OptionsForm, OnOptionsWindowHidden, caption, false, true);
		}
		protected virtual void ShowEditDictionaryWindow() {
			string caption = SpellCheckerLocalizer.GetString(SpellCheckerStringId.Form_EditCustomDictionary_Caption);
			CustomDictionaryEventArgs args = new CustomDictionaryEventArgs(EditDictionaryForm);
			OnCustomDictionaryControlCreated(args);
			if (args.Control != null)
				editDictionaryForm = args.Control as CustomDictionaryControl;
			ShowWindow(OptionsForm, EditDictionaryForm, OnEditDictioanryWindowHidden, caption, true, true);
		}
		protected virtual void ShowSpellCheckWindow(bool showModal) {
			string caption = SpellCheckerLocalizer.GetString(SpellCheckerStringId.Form_Spelling_Caption);
			SpellingControlBaseEventArgs args = null;
			if (SpellChecker.SpellingFormType == SpellingFormType.Outlook) {
				args = new SpellingOutlookStyleEventArgs(SpellCheckForm as SpellingOutlookStyleControl);
				OnSpellingOutlookStyleControlCreated(args as SpellingOutlookStyleEventArgs);
			}
			else {
				args = new SpellingWordStyleEventArgs(SpellCheckForm as SpellingWordStyleControl);
				OnSpellingWordStyleControlCreated(args as SpellingWordStyleEventArgs);
			}
			if (args.Control != null && !SpellCheckForm.Equals(args.Control))
				spellCheckForm = args.Control;
			ShowWindow(GetWidnowOwner(), SpellCheckForm, OnSpellCheckWindowHidden, caption, true, showModal);
		}
		protected virtual SpellingControlBase CreateSpellCheckForm() {
			if (SpellChecker.SpellingFormType == SpellingFormType.Outlook)
				return new SpellingOutlookStyleControl(spellChecker);
			return new SpellingWordStyleControl(spellChecker);
		}
		void OnSpellCheckFormResultChanged(object sender, EventArgs e) {
			SpellingControlBase form = sender as SpellingControlBase;
			string fSuggestion = form.SpellingFormResult == SpellCheckOperation.Cancel ? string.Empty : form.Suggestion;
			RaiseSpellCheckFormResultChanged(new SpellingFormResultChangedEventArgs(form.SpellingFormResult, fSuggestion));
		}
		public virtual bool IsSpellCheckFormVisible() {
			return false;
		}
		protected virtual object CreateMSWordLikeSpellCheckForm() {
			return null;
		}
		protected virtual object CreateMSOutlookLikeSpellCheckForm() {
			return null;
		}
		protected virtual SpellingOptionsControl CreateOptionsForm() {
			return new SpellingOptionsControl(spellChecker);
		}
		protected virtual CustomDictionaryControl CreateDictionaryForm(SpellCheckerCustomDictionary dictionary) {
			return new CustomDictionaryControl(SpellChecker, dictionary);
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
			spellCheckForm = null;
		}
		private void DestroyDictionaryForm() {
			editDictionaryForm = null;
		}
		private void DestroyOptionsForm() {
			optionsForm = null;
		}
		#endregion
		#region FormsShowing
		public void ShowSpellCheckForm() {
			SpellingFormShowingEventArgs e = new SpellingFormShowingEventArgs(string.Empty, null); 
			SpellChecker.OnSpellingFormShowing(e);
			if (e.Handled)
				return;
			ShowSpellCheckFormCore();
		}
		protected virtual void ShowSpellCheckFormCore() {
			Control editControl = SpellChecker.EditControl;
			bool keepFocusOnEditor = SpellChecker.KeepFocusOnControl && SpellChecker.SpellingFormType == SpellingFormType.Outlook;
			if (!IsControlActive(SpellCheckForm))
				ShowSpellCheckWindow(!keepFocusOnEditor);
			if (editControl != null && keepFocusOnEditor)
				editControl.Focus();
		}
		public virtual void ShowOptionsForm() {
			FormShowingEventArgs e = new FormShowingEventArgs();
			SpellChecker.OnOptionsFormShowing(e);
			if (e.Handled)
				return;
			if (!IsControlActive(OptionsForm))
				ShowOptionsWindow();
		}
		public void ShowEditDictionaryForm(SpellCheckerCustomDictionary dictionary) {
			if (!IsControlActive(EditDictionaryForm))
				ShowEditDictionaryWindow();
		}
		bool IsControlActive(DependencyObject obj) {
			return FloatingContainer.GetDialogOwner(obj) != null;
		}
		protected internal virtual FrameworkElement GetWidnowOwner() {
			FrameworkElement result = SpellChecker.EditControl;
			if (result != null) {
				do {
					if (result is ILogicalOwner)
						return result;
					result = result.GetParent() as FrameworkElement;
				} while (result != null);
				return result == null ? SpellChecker.EditControl : result;
			}
			else
				return GetVisualRoot();
		}
		protected internal FrameworkElement GetVisualRoot() {
			return GetActiveWindow();
		}
		Window GetActiveWindow() {
			IntPtr hWnd = WinAPI.GetActiveWindow();
			if (hWnd == IntPtr.Zero)
				return null;
			System.Windows.Interop.HwndSource wndSource = System.Windows.Interop.HwndSource.FromHwnd(hWnd);
			return wndSource != null ? wndSource.RootVisual as Window : null;
		}
		void OnSpellCheckWindowHidden(bool? dialogResult) {
			if (!dialogResult.HasValue || dialogResult.Value == false)
				SpellCheckForm.CancelCommand.Execute(null);
		}
		bool ShouldDoCancelOperation(StrategyState state) {
			return state != StrategyState.None && state != StrategyState.Stop && state != StrategyState.UserStop;
		}
		void OnOptionsWindowHidden(bool? dialogResult) {
			bool result = dialogResult.HasValue ? dialogResult.Value : false;
			RaiseOptionsFormClosed(result);
			if (!result)
				OnOptionsWindowHiddenCore();
		}
		void OnEditDictioanryWindowHidden(bool? dialogResult) {
			if (EditDictionaryForm.NeedSave()) {
				SpellCheckerCustomDictionary dictionary = EditDictionaryForm.Dictionary;
				dictionary.Clear();
				string[] words = EditDictionaryForm.Text.Split(new string[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
				dictionary.AddWords(words);
				SpellChecker.RaiseCustomDictionaryChanged();
			}
			bool result = dialogResult.HasValue ? dialogResult.Value : false;
			if (!result)
				OnEditDictioanryWindowHiddenCore();
		}
		protected internal virtual void ShowWindow(FrameworkElement owner, FrameworkElement content, DialogClosedDelegate onClosed, string caption, bool allowSizing, bool showModal) {
			FloatingContainerParameters parameters = new FloatingContainerParameters();
			Window activeWindow = GetActiveWindow();
			parameters.ClosedDelegate = (dialogResult) => {
				if (activeWindow != null)
					activeWindow.Activate();
				onClosed(dialogResult);
			};
			parameters.Title = caption;
			parameters.CloseOnEscape = true;
			parameters.AllowSizing = allowSizing;
			if (owner == null || !IsControlActive(owner))
				owner = GetWidnowOwner();
			if (IsWpfHostedInWin(owner)) {
				parameters.ContainerFocusable = showModal;
				if (showModal) {
					parameters.ShowModal = true;
					FloatingContainer.ShowDialog(content, owner, Size.Empty, parameters, owner);
				}
				else {
					parameters.ShowModal = false;
					FloatingContainer.ShowDialog(content, owner, Size.Empty, parameters);
				}
			}
			else
				FloatingContainer.ShowDialog(content, owner, Size.Empty, parameters);
		}
		bool IsWpfHostedInWin(FrameworkElement element) {
			System.Windows.Interop.HwndSource source = PresentationSource.FromDependencyObject(element) as System.Windows.Interop.HwndSource;
			if (source != null) {
				System.Windows.Forms.Control parent = System.Windows.Forms.Control.FromChildHandle(source.Handle);
				return parent == null ? false : parent.GetType().Name == "ElementHost";
			}
			return false;
		}
		#endregion
		void CloseWindow(DependencyObject control, bool dialogResult) {
			IDialogOwner dialogOwner = FloatingContainer.GetDialogOwner(control);
			if (dialogOwner != null) {
				dialogOwner.CloseDialog(dialogResult);
				OnWindowHiddenCore(control as SpellCheckerControlBase);
			}
		}
		public virtual void HideSpellCheckForm(SpellingFormType spellingFormType) {
			if (spellCheckForm != null) {
				CloseWindow(spellCheckForm, true);
				OnSpellCheckWindowHidden(true);
				spellCheckForm = null;
			}
		}
		public void HideOptionsForm() {
			if (optionsForm != null) {
				CloseWindow(optionsForm, true);
				OnOptionsWindowHidden(true);
				optionsForm = null;
			}
		}
		public void HideEditDictionaryForm() {
			if (editDictionaryForm != null)
				CloseWindow(editDictionaryForm, false);
		}
		protected virtual void OnSpellCheckWindowHiddenCore() {
			if (this.spellCheckForm != null) {
				OnWindowHiddenCore(this.spellCheckForm);
				this.spellCheckForm = null;
			}
		}
		protected virtual void OnOptionsWindowHiddenCore() {
			if (this.optionsForm != null) {
				OnWindowHiddenCore(this.optionsForm);
				this.optionsForm = null;
			}
		}
		protected virtual void OnEditDictioanryWindowHiddenCore() {
			if (this.editDictionaryForm != null) {
				OnWindowHiddenCore(this.editDictionaryForm);
				this.editDictionaryForm = null;
			}
		}
		protected virtual void OnWindowHiddenCore(SpellCheckerControlBase control) {
			ContentControl parent = control.Parent as ContentControl;
			if (parent != null)
				parent.Content = null;
			FloatingContainer.SetDialogOwner(control, null);
		}
	}
	#endregion
}
