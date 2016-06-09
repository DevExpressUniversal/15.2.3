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

using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DevExpress.XtraSpellChecker;
using System.Globalization;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.Xpf.SpellChecker.Native;
using System;
using DevExpress.XtraSpellChecker.Strategies;
namespace DevExpress.Xpf.SpellChecker.Forms {
	#region SpellingOptionsApplyCommand
	public class SpellingOptionsApplyCommand : UICommand<SpellingOptionsControl> {
		public SpellingOptionsApplyCommand(SpellingOptionsControl control)
			: base(control) {
		}
		protected override bool CanExecuteCore(object parameter) {
			return Control.IsModified;
		}
		protected override void ExecuteCore(object parameter) {
			OptionsSpelling options = Control.SpellChecker.OptionsSpelling;
			options.BeginUpdate();
			try {
				options.IgnoreUpperCaseWords = BoolDefaultBooleanConverter.ConvertBoolToDefaultBoolean(Control.IgnoreUpperCase);
				options.IgnoreEmails = BoolDefaultBooleanConverter.ConvertBoolToDefaultBoolean(Control.IgnoreEmails);
				options.IgnoreWordsWithNumbers = BoolDefaultBooleanConverter.ConvertBoolToDefaultBoolean(Control.IgnoreWordsWithDigits);
				options.IgnoreUri = BoolDefaultBooleanConverter.ConvertBoolToDefaultBoolean(Control.IgnoreUri);
				options.IgnoreMixedCaseWords = BoolDefaultBooleanConverter.ConvertBoolToDefaultBoolean(Control.IgnoreMixedCaseWords);
				options.IgnoreRepeatedWords = BoolDefaultBooleanConverter.ConvertBoolToDefaultBoolean(Control.IgnoreRepeatedWords);
			}
			finally {
				options.EndUpdate();
			}
			Control.SpellChecker.Culture = Control.CurrentLanguage.Culture;
			SpellChecker.SetSpellCheckerOptions(Control.SpellChecker.EditControl, options);
			Control.IsModified = false;
		}
	}
	#endregion
	#region SpellingOptionsOkCommand
	public class SpellingOptionsOkCommand : SpellingOptionsApplyCommand {
		public SpellingOptionsOkCommand(SpellingOptionsControl control)
			: base(control) {
		}
		protected override bool CanExecuteCore(object parameter) {
			return true;
		}
		protected override void ExecuteCore(object parameter) {
			base.ExecuteCore(parameter);
			Control.SpellChecker.FormsManager.HideOptionsForm();
			SearchStrategy strategy = Control.SpellChecker.SearchStrategy;
			strategy.CurrentPosition = strategy.TextController.GetWordStartPosition(strategy.WordStartPosition);
			Control.SpellChecker.SearchStrategy.SetState(StrategyState.Checking);
		}
	}
	#endregion
	#region SpellingOptionsEditCommand
	public class SpellingOptionsEditCommand : UICommand<SpellingOptionsControl> {
		public SpellingOptionsEditCommand(SpellingOptionsControl control)
			: base(control) {
		}
		protected override bool CanExecuteCore(object parameter) {
			return Control.CustomDictionary != null;
		}
		protected override void ExecuteCore(object parameter) {
			Control.EditCustomDictionary();
		}
	}
	#endregion
	#region SpellingOptionsCancelCommand
	public class SpellingOptionsCancelCommand : UICommand<SpellingOptionsControl> {
		public SpellingOptionsCancelCommand(SpellingOptionsControl control)
			: base(control) {
		}
		protected override bool CanExecuteCore(object parameter) {
			return true;
		}
		protected override void ExecuteCore(object parameter) {
			Control.SpellChecker.FormsManager.HideOptionsForm();
		}
	}
	#endregion
	#region LanguageItem
	public class LanguageItem {
		CultureInfo culture;
		public LanguageItem(CultureInfo culture) {
			this.culture = culture;
		}
		public CultureInfo Culture { get { return culture; } }
		public override string ToString() {
			return culture.DisplayName;
		}
		public override bool Equals(object obj) {
			LanguageItem item = obj as LanguageItem;
			if (item != null)
				return culture.Equals(item.Culture);
			else
				return false;
		}
		public override int GetHashCode() {
			return Culture.GetHashCode();
		}
	}
	#endregion
	#region SpellingOptionsControl
	[DXToolboxBrowsable(false)]
	public class SpellingOptionsControl : SpellCheckerControlBase {
		ObservableCollection<LanguageItem> languages = new ObservableCollection<LanguageItem>();
		bool isUpdated = true;
		public SpellingOptionsControl() : base() { }
		public SpellingOptionsControl(SpellChecker spellChecker)
			: base(spellChecker) {
			DefaultStyleKey = typeof(SpellingOptionsControl);
			Initialize();
		}
		#region Properties
		internal SpellCheckerCustomDictionary CustomDictionary {
			get { return SpellChecker.DictionaryHelper.GetCustomDictionary(SpellChecker.SearchStrategy.ActualCulture); }
		}
		#region OkCommand
		public static readonly DependencyProperty OkCommandProperty = CreateOkCommandProperty();
		static DependencyProperty CreateOkCommandProperty() {
			return DependencyProperty.Register("OkCommand", typeof(ICommand), typeof(SpellingOptionsControl), new PropertyMetadata(null));
		}
		public ICommand OkCommand {
			get { return (ICommand)GetValue(OkCommandProperty); }
			set { SetValue(OkCommandProperty, value); }
		}
		#endregion
		#region ApplyCommand
		public static readonly DependencyProperty ApplyCommandProperty = CreateApplyCommandProperty();
		static DependencyProperty CreateApplyCommandProperty() {
			return DependencyProperty.Register("ApplyCommand", typeof(ICommand), typeof(SpellingOptionsControl), new PropertyMetadata(null));
		}
		public ICommand ApplyCommand {
			get { return (ICommand)GetValue(ApplyCommandProperty); }
			set { SetValue(ApplyCommandProperty, value); }
		}
		#endregion
		#region EditCommand
		public static readonly DependencyProperty EditCommandProperty = CreateEditCommand();
		static DependencyProperty CreateEditCommand() {
			return DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(SpellingOptionsControl), new PropertyMetadata(null));
		}
		public ICommand EditCommand {
			get { return (ICommand)GetValue(EditCommandProperty); }
			set { SetValue(EditCommandProperty, value); }
		}
		#endregion
		#region CancelCommand
		public static readonly DependencyProperty CancelCommandProperty = CreateCancelCommand();
		static DependencyProperty CreateCancelCommand() {
			return DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(SpellingOptionsControl), new PropertyMetadata(null));
		}
		public ICommand CancelCommand {
			get { return (ICommand)GetValue(CancelCommandProperty); }
			set { SetValue(CancelCommandProperty, value); }
		}
		#endregion
		#region IgnoreEmails
		public static readonly DependencyProperty IgnoreEmailsProperty = CreateIgnoreEmailsProperty();
		static DependencyProperty CreateIgnoreEmailsProperty() {
			return DependencyProperty.Register("IgnoreEmails", typeof(bool), typeof(SpellingOptionsControl), new PropertyMetadata(false, OnIgnoreEmailsChanged));
		}
		static void OnIgnoreEmailsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SpellingOptionsControl)d).OnPropertyChanged();
		}
		public bool IgnoreEmails {
			get { return (bool)GetValue(IgnoreEmailsProperty); }
			set { SetValue(IgnoreEmailsProperty, value); }
		}
		#endregion
		#region IgnoreUrls
		[Obsolete("This property has become obsolete. Use the IgnoreUriProperty property instead.")]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static readonly DependencyProperty IgnoreUrlsProperty = CreateIgnoreUrlsProperty();
		static DependencyProperty CreateIgnoreUrlsProperty() {
			return DependencyProperty.Register("IgnoreUrls", typeof(bool), typeof(SpellingOptionsControl), new PropertyMetadata(false, OnIgnoreUrlsChanged));
		}
		static void OnIgnoreUrlsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SpellingOptionsControl)d).OnPropertyChanged();
		}
		[Obsolete("This property has become obsolete. Use the IgnoreUri property instead.")]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		[System.ComponentModel.Browsable(false)]
		public bool IgnoreUrls {
#pragma warning disable 0618
			get { return (bool)GetValue(IgnoreUrlsProperty); }
			set { SetValue(IgnoreUrlsProperty, value); }
#pragma warning restore 0618
		}
		#endregion
		#region IgnoreUri
		public static readonly DependencyProperty IgnoreUriProperty = CreateIgnoreUriProperty();
		static DependencyProperty CreateIgnoreUriProperty() {
			return DependencyProperty.Register("IgnoreUri", typeof(bool), typeof(SpellingOptionsControl), new PropertyMetadata(false, OnIgnoreUriChanged));
		}
		static void OnIgnoreUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SpellingOptionsControl)d).OnPropertyChanged();
		}
		public bool IgnoreUri {
			get { return (bool)GetValue(IgnoreUriProperty); }
			set { SetValue(IgnoreUriProperty, value); }
		}
		#endregion
		#region IgnoreUpperCase
		public static readonly DependencyProperty IgnoreUpperCaseProperty = CreateIgnoreUpperCaseProperty();
		static DependencyProperty CreateIgnoreUpperCaseProperty() {
			return DependencyProperty.Register("IgnoreUpperCase", typeof(bool), typeof(SpellingOptionsControl), new PropertyMetadata(false, OnIgnoreUpperCaseChanged));
		}
		static void OnIgnoreUpperCaseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SpellingOptionsControl)d).OnPropertyChanged();
		}
		public bool IgnoreUpperCase {
			get { return (bool)GetValue(IgnoreUpperCaseProperty); }
			set { SetValue(IgnoreUpperCaseProperty, value); }
		}
		#endregion
		#region IgnoreWordsWithDigits
		public static readonly DependencyProperty IgnoreWordsWithDigitsProperty = CreateIgnoreWordsWithDigitsProperty();
		static DependencyProperty CreateIgnoreWordsWithDigitsProperty() {
			return DependencyProperty.Register("IgnoreWordsWithDigits", typeof(bool), typeof(SpellingOptionsControl), new PropertyMetadata(false, OnIgnoreWordsWithDigitsChanged));
		}
		static void OnIgnoreWordsWithDigitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SpellingOptionsControl)d).OnPropertyChanged();
		}
		public bool IgnoreWordsWithDigits {
			get { return (bool)GetValue(IgnoreWordsWithDigitsProperty); }
			set { SetValue(IgnoreWordsWithDigitsProperty, value); }
		}
		#endregion
		#region IgnoreMixedCaseWords
		public static readonly DependencyProperty IgnoreMixedCaseWordsProperty = CreateIgnoreMixedCaseWordsProperty();
		static DependencyProperty CreateIgnoreMixedCaseWordsProperty() {
			return DependencyProperty.Register("IgnoreMixedCaseWords", typeof(bool), typeof(SpellingOptionsControl), new PropertyMetadata(false, OnIgnoreMixedCaseWordsChanged));
		}
		static void OnIgnoreMixedCaseWordsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SpellingOptionsControl)d).OnPropertyChanged();
		}
		public bool IgnoreMixedCaseWords {
			get { return (bool)GetValue(IgnoreMixedCaseWordsProperty); }
			set { SetValue(IgnoreMixedCaseWordsProperty, value); }
		}
		#endregion
		#region IgnoreRepeatedWords
		public static readonly DependencyProperty IgnoreRepeatedWordsProperty = CreateIgnoreRepeatedWordsProperty();
		static DependencyProperty CreateIgnoreRepeatedWordsProperty() {
			return DependencyProperty.Register("IgnoreRepeatedWords", typeof(bool), typeof(SpellingOptionsControl), new PropertyMetadata(false, OnIgnoreRepeatedWordsChanged));
		}
		static void OnIgnoreRepeatedWordsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SpellingOptionsControl)d).OnPropertyChanged();
		}
		public bool IgnoreRepeatedWords {
			get { return (bool)GetValue(IgnoreRepeatedWordsProperty); }
			set { SetValue(IgnoreRepeatedWordsProperty, value); }
		}
		#endregion
		#region Languages
		public ObservableCollection<LanguageItem> Languages { get { return languages; } }
		#endregion
		#region IsModified
		public static readonly DependencyProperty IsModifiedProperty = CreateIsModifiedProperty();
		static DependencyProperty CreateIsModifiedProperty() {
			return DependencyProperty.Register("IsModified", typeof(bool), typeof(SpellingOptionsControl), new PropertyMetadata(false, OnIsModifiedChanged));
		}
		static void OnIsModifiedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellingOptionsApplyCommand applyCommand = ((SpellingOptionsControl)d).ApplyCommand as SpellingOptionsApplyCommand;
			if (applyCommand != null)
				applyCommand.RaiseCanExecuteChanged();
		}
		public bool IsModified {
			get { return (bool)GetValue(IsModifiedProperty); }
			set { SetValue(IsModifiedProperty, value); }
		}
		#endregion
		#region CurrentLanguage
		public static readonly DependencyProperty CurrentLanguageProperty = CreateCurrentLanguageProperty();
		static DependencyProperty CreateCurrentLanguageProperty() {
			return DependencyProperty.Register("CurrentLanguage", typeof(LanguageItem), typeof(SpellingOptionsControl), new PropertyMetadata(null, OnCurrentLanguageChanged));
		}
		static void OnCurrentLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellingOptionsControl control = d as SpellingOptionsControl;
			control.OnPropertyChanged();
			control.OnCurrentLanguageChanged(e.OldValue as LanguageItem, e.NewValue as LanguageItem);
		}
		public LanguageItem CurrentLanguage {
			get { return (LanguageItem)GetValue(CurrentLanguageProperty); }
			set { SetValue(CurrentLanguageProperty, value); }
		}
		#endregion
		#endregion
		internal void OnPropertyChanged() {
			if (this.isUpdated)
				IsModified = true;
		}
		void BeginUpdate() {
			this.isUpdated = false;
		}
		void EndUpdate() {
			this.isUpdated = true;
		}
		protected override void Initialize() {
			OkCommand = new SpellingOptionsOkCommand(this);
			ApplyCommand = new SpellingOptionsApplyCommand(this);
			EditCommand = new SpellingOptionsEditCommand(this);
			CancelCommand = new SpellingOptionsCancelCommand(this);
		}
		protected override void OnSpellCheckerChanged() {
			BeginUpdate();
			try {
				UpdateOptions();
				PopulateLanguages();
			}
			finally {
				EndUpdate();
			}
		}
		void UpdateOptions() {
			OptionsSpelling options = SpellChecker.OptionsSpelling;
			IgnoreEmails = options.IsIgnoreEmails();
			IgnoreWordsWithDigits = options.IsIgnoreWordsWithNumbers();
			IgnoreUpperCase = options.IsIgnoreUpperCaseWords();
			IgnoreUri = options.IsIgnoreUri();
			IgnoreMixedCaseWords = options.IsIgnoreMixedCaseWords();
			IgnoreRepeatedWords = options.IsIgnoreRepeatedWords();
		}
		void OnCurrentLanguageChanged(LanguageItem oldValue, LanguageItem newValue) {
			SpellingOptionsEditCommand editCommand = EditCommand as SpellingOptionsEditCommand;
			if(editCommand != null)
				editCommand.RaiseCanExecuteChanged();
		}
		protected virtual void PopulateLanguages() {
			PopulateLanguages(SpellChecker.Dictionaries);
			LanguageItem currentLanguage = new LanguageItem(SpellChecker.SearchStrategy.ActualCulture);
			int index = Languages.IndexOf(currentLanguage);
			if (index < 0) {
				Languages.Insert(0, currentLanguage);
				index = 0;
			}
			CurrentLanguage = Languages[index];
		}
		void PopulateLanguages(DictionaryCollection dictionaryCollection) {
			Languages.Clear();
			int count = dictionaryCollection.Count;
			for (int i = 0; i < count; i++) {
				LanguageItem item = new LanguageItem(dictionaryCollection[i].Culture);
				if (!Languages.Contains(item))
					Languages.Add(item);
			}
			if (Languages.Count > 1)
				Languages.Add(new LanguageItem(CultureInfo.InvariantCulture));
		}
		[Obsolete("This method overload has become obsolete. To apply your changes, use ApplyCommand property.")]
		protected virtual void ApplyOptions() {
			ApplyCommand.Execute(null);
		}
		protected internal virtual void EditCustomDictionary() {
			SpellChecker.FormsManager.ShowEditDictionaryForm(CustomDictionary);
		}
	}
	#endregion
}
