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
using DevExpress.XtraSpellChecker;
using System.Collections.ObjectModel;
using System.Windows;
using System;
using DevExpress.XtraSpellChecker.Localization;
using DevExpress.Xpf.SpellChecker.Native;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using DevExpress.Xpf.Editors;
using DevExpress.XtraSpellChecker.Parser;
using System.Reflection;
using DevExpress.XtraSpellChecker.Strategies;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.SpellChecker.Forms {
	#region SpellingOutlookStyleBaseCommand
	public abstract class SpellingOutlookStyleBaseCommand : UICommand<SpellingControlBase> {
		protected SpellingOutlookStyleBaseCommand(SpellingControlBase control)
			: base(control) {
		}
		protected abstract SpellCheckOperation Operation { get; }
		protected SearchStrategy SearchStrategy { get { return Control.SpellChecker.SearchStrategy; } }
		protected override bool CanExecuteCore(object parameter) {
			return true;
		}
		protected override void ExecuteCore(object parameter) {
			SearchStrategy.DoSpellCheckOperation(Operation, Control.Suggestion);
			if (Operation != SpellCheckOperation.Cancel && Operation != SpellCheckOperation.Options)
				SearchStrategy.SetState(StrategyState.Checking);
			Control.SpellingFormResult = Operation;
		}
	}
	#endregion
	#region SpellingOutlookStyleChangeCommand
	public class SpellingOutlookStyleChangeCommand : SpellingOutlookStyleBaseCommand {
		public SpellingOutlookStyleChangeCommand(SpellingControlBase control)
			: base(control) {
		}
		protected override SpellCheckOperation Operation { get { return SpellCheckOperation.Change; } }
		protected override bool CanExecuteCore(object parameter) {
			return Control.CanDoChange;
		}
	}
	#endregion
	#region SpellingOutlookStyleChangeAllCommand
	public class SpellingOutlookStyleChangeAllCommand : SpellingOutlookStyleChangeCommand {
		public SpellingOutlookStyleChangeAllCommand(SpellingControlBase control)
			: base(control) {
		}
		protected override SpellCheckOperation Operation { get { return SpellCheckOperation.ChangeAll; } }
		protected override bool CanExecuteCore(object parameter) {
			return Control.CanDoChange;
		}
	}
	#endregion
	#region SpellingOutlookStyleDeleteCommand
	public class SpellingOutlookStyleDeleteCommand : SpellingOutlookStyleBaseCommand {
		public SpellingOutlookStyleDeleteCommand(SpellingControlBase control)
			: base(control) {
		}
		protected override SpellCheckOperation Operation { get { return SpellCheckOperation.Delete; } }
	}
	#endregion
	#region SpellingOutlookStyleIgnoreCommand
	public class SpellingOutlookStyleIgnoreCommand : SpellingOutlookStyleBaseCommand {
		public SpellingOutlookStyleIgnoreCommand(SpellingControlBase control)
			: base(control) {
		}
		protected override SpellCheckOperation Operation { get { return SpellCheckOperation.Ignore; } }
	}
	#endregion
	#region SpellingOutlookStyleIgnoreAllCommand
	public class SpellingOutlookStyleIgnoreAllCommand : SpellingOutlookStyleBaseCommand {
		public SpellingOutlookStyleIgnoreAllCommand(SpellingControlBase control)
			: base(control) {
		}
		protected override SpellCheckOperation Operation { get { return SpellCheckOperation.IgnoreAll; } }
	}
	#endregion
	#region SpellingOutlookStyleAddToDictionaryCommand
	public class SpellingOutlookStyleAddToDictionaryCommand : SpellingOutlookStyleBaseCommand {
		public SpellingOutlookStyleAddToDictionaryCommand(SpellingControlBase control)
			: base(control) {
		}
		protected override SpellCheckOperation Operation { get { return SpellCheckOperation.AddToDictionary; } }
		protected override bool CanExecuteCore(object parameter) {
			return Control.IsCustomDictionaryExits;
		}
	}
	#endregion
	#region SpellingOutlookStyleCancelCommand
	public class SpellingOutlookStyleCancelCommand : SpellingOutlookStyleBaseCommand {
		public SpellingOutlookStyleCancelCommand(SpellingControlBase control)
			: base(control) {
		}
		protected override SpellCheckOperation Operation { get { return SpellCheckOperation.Cancel; } }
	}
	#endregion
	#region SpellingOutlookStyleUndoCommand
	public class SpellingOutlookStyleUndoCommand : SpellingOutlookStyleBaseCommand {
		public SpellingOutlookStyleUndoCommand(SpellingControlBase control)
			: base(control) {
		}
		protected override SpellCheckOperation Operation { get { return SpellCheckOperation.Undo; } }
		protected override bool CanExecuteCore(object parameter) {
			return Control.IsUndoAvailable;
		}
	}
	#endregion
	#region SpellingOutlookStyleShowOptionsCommand
	public class SpellingOutlookStyleShowOptionsCommand : SpellingOutlookStyleBaseCommand {
		public SpellingOutlookStyleShowOptionsCommand(SpellingControlBase control)
			: base(control) {
		}
		protected override SpellCheckOperation Operation { get { return SpellCheckOperation.Options; } }
	}
	#endregion
	#region SpellingWordStyleChangeCommand
	public class SpellingWordStyleChangeCommand : SpellingOutlookStyleChangeCommand {
		public SpellingWordStyleChangeCommand(SpellingControlBase control)
			: base(control) {
		}
		protected override void ExecuteCore(object parameter) {
			if (String.IsNullOrEmpty(Control.Suggestion) || Control.SpellChecker.SearchStrategy.DictionaryHelper.FindWord(Control.Suggestion))
				base.ExecuteCore(parameter);
			else {
				string title = GetProductName();
				string text = SpellCheckerLocalizer.Active.GetLocalizedString(SpellCheckerStringId.Msg_CanUseCurrentWord);
#if !SL
				DevExpress.XtraSpellChecker.Forms.DialogHelper.ShowDialog(title, text, Control, DoSpellCheckOperation, MessageBoxButton.YesNo);
#else
				DevExpress.XtraSpellChecker.Forms.DialogHelper.ShowDialog(title, text, Control, DoSpellCheckOperation, DXMessageBoxButton.YesNo);
#endif
			}
		}
		void DoSpellCheckOperation(bool? dialogResult) {
			if (dialogResult.Value)
				Control.SpellChecker.SearchStrategy.DoSpellCheckOperation(Operation, Control.Suggestion);
			else
				Control.SpellChecker.SearchStrategy.DoSpellCheckOperation(SpellCheckOperation.Recheck, Control.Suggestion);
			Control.SpellChecker.SearchStrategy.SetState(DevExpress.XtraSpellChecker.Strategies.StrategyState.Checking);
			Control.SpellingFormResult = Operation;
		}
		string GetProductName() {
#if !SL
			object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
#else
			object[] attributes = Application.Current.GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
#endif
			if (attributes.Length > 0) {
				AssemblyProductAttribute productAttribute = attributes[0] as AssemblyProductAttribute;
				return productAttribute != null ? productAttribute.Product : String.Empty;
			}
			return String.Empty;
		}
	}
	#endregion
	#region SpellingWordStyleChangeAllCommand
	public class SpellingWordStyleChangeAllCommand : SpellingWordStyleChangeCommand {
		public SpellingWordStyleChangeAllCommand(SpellingControlBase control)
			: base(control) {
		}
		protected override SpellCheckOperation Operation { get { return SpellCheckOperation.ChangeAll; } }
	}
	#endregion
	#region SpellingControlBase
	[DXToolboxBrowsable(false)]
	public abstract class SpellingControlBase : SpellCheckerControlBase {
		SpellCheckOperation spellingFormResult;
		ObservableCollection<string> suggestions = new ObservableCollection<string>();
		FrameworkElement layoutRoot;
		protected SpellingControlBase() { }
		protected SpellingControlBase(SpellChecker spellChecker) {
			SpellChecker = spellChecker;
			Initialize();
		}
		#region Properties
		public virtual ObservableCollection<string> Suggestions { get { return suggestions; } }
		public SpellCheckOperation SpellingFormResult {
			get { return spellingFormResult; }
			set {
				spellingFormResult = value;
				RaiseSpellCheckFormResultChanged();
			}
		}
		#region ChangeCommand
		public static readonly DependencyProperty ChangeCommandProperty = CreateChangeCommandProperty();
		static DependencyProperty CreateChangeCommandProperty() {
			return DependencyProperty.Register("ChangeCommand", typeof(ICommand), typeof(SpellingControlBase), new PropertyMetadata(null));
		}
		public ICommand ChangeCommand {
			get { return (ICommand)GetValue(ChangeCommandProperty); }
			set { SetValue(ChangeCommandProperty, value); }
		}
		#endregion
		#region ChangeAllCommand
		public static readonly DependencyProperty ChangeAllCommandProperty = CreateChangeAllCommandProperty();
		static DependencyProperty CreateChangeAllCommandProperty() {
			return DependencyProperty.Register("ChangeAllCommand", typeof(ICommand), typeof(SpellingControlBase), new PropertyMetadata(null));
		}
		public ICommand ChangeAllCommand {
			get { return (ICommand)GetValue(ChangeAllCommandProperty); }
			set { SetValue(ChangeAllCommandProperty, value); }
		}
		#endregion
		#region DeleteCommand
		public static readonly DependencyProperty DeleteCommandProperty = CreateDeleteCommandProperty();
		static DependencyProperty CreateDeleteCommandProperty() {
			return DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(SpellingControlBase), new PropertyMetadata(null));
		}
		public ICommand DeleteCommand {
			get { return (ICommand)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		#endregion
		#region IgnoreCommand
		public static readonly DependencyProperty IgnoreCommandProperty = CreateIgnoreCommandProperty();
		static DependencyProperty CreateIgnoreCommandProperty() {
			return DependencyProperty.Register("IgnoreCommand", typeof(ICommand), typeof(SpellingControlBase), new PropertyMetadata(null));
		}
		public ICommand IgnoreCommand {
			get { return (ICommand)GetValue(IgnoreCommandProperty); }
			set { SetValue(IgnoreCommandProperty, value); }
		}
		#endregion
		#region IgnoreAllCommand
		public static readonly DependencyProperty IgnoreAllCommandProperty = CreateIgnoreAllCommandProperty();
		static DependencyProperty CreateIgnoreAllCommandProperty() {
			return DependencyProperty.Register("IgnoreAllCommand", typeof(ICommand), typeof(SpellingControlBase), new PropertyMetadata(null));
		}
		public ICommand IgnoreAllCommand {
			get { return (ICommand)GetValue(IgnoreAllCommandProperty); }
			set { SetValue(IgnoreAllCommandProperty, value); }
		}
		#endregion
		#region AddToDictionaryCommand
		public static readonly DependencyProperty AddToDictionaryCommandProperty = CreateAddToDictionaryCommandProperty();
		static DependencyProperty CreateAddToDictionaryCommandProperty() {
			return DependencyProperty.Register("AddToDictionaryCommand", typeof(ICommand), typeof(SpellingControlBase), new PropertyMetadata(null));
		}
		public ICommand AddToDictionaryCommand {
			get { return (ICommand)GetValue(AddToDictionaryCommandProperty); }
			set { SetValue(AddToDictionaryCommandProperty, value); }
		}
		#endregion
		#region CancelCommand
		public static readonly DependencyProperty CancelCommandProperty = CreateCancelCommandProperty();
		static DependencyProperty CreateCancelCommandProperty() {
			return DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(SpellingControlBase), new PropertyMetadata(null));
		}
		public ICommand CancelCommand {
			get { return (ICommand)GetValue(CancelCommandProperty); }
			set { SetValue(CancelCommandProperty, value); }
		}
		#endregion
		#region UndoCommand
		public static readonly DependencyProperty UndoCommandProperty = CreateUndoCommandProperty();
		static DependencyProperty CreateUndoCommandProperty() {
			return DependencyProperty.Register("UndoCommand", typeof(ICommand), typeof(SpellingControlBase), new PropertyMetadata(null));
		}
		public ICommand UndoCommand {
			get { return (ICommand)GetValue(UndoCommandProperty); }
			set { SetValue(UndoCommandProperty, value); }
		}
		#endregion
		#region ShowOptionsCommand
		public static readonly DependencyProperty ShowOptionsCommandProperty = CreateShowOptionsCommandProperty();
		static DependencyProperty CreateShowOptionsCommandProperty() {
			return DependencyProperty.Register("ShowOptionsCommand", typeof(ICommand), typeof(SpellingControlBase), new PropertyMetadata(null));
		}
		public ICommand ShowOptionsCommand {
			get { return (ICommand)GetValue(ShowOptionsCommandProperty); }
			set { SetValue(ShowOptionsCommandProperty, value); }
		}
		#endregion
		#region Suggestion
		public static readonly DependencyProperty SuggestionProperty = CreateSuggestionProperty();
		static DependencyProperty CreateSuggestionProperty() {
			return DependencyProperty.Register("Suggestion", typeof(string), typeof(SpellingControlBase), new PropertyMetadata(String.Empty, OnSuggestionPropertyChanged));
		}
		static void OnSuggestionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SpellingControlBase)d).OnSuggestionChanged((string)e.OldValue, (string)e.NewValue);
		}
		public string Suggestion {
			get { return (string)GetValue(SuggestionProperty); }
			set { SetValue(SuggestionProperty, value); }
		}
		#endregion
		#region WrongWord
		public static readonly DependencyProperty WrongWordProperty = CreateWrongWordProperty();
		static DependencyProperty CreateWrongWordProperty() {
			return DependencyProperty.Register("WrongWord", typeof(string), typeof(SpellingControlBase), new PropertyMetadata(String.Empty));
		}
		public string WrongWord {
			get { return (string)GetValue(WrongWordProperty); }
			set { SetValue(WrongWordProperty, value); }
		}
		#endregion
		#region IsUndoAvailable
		public static readonly DependencyProperty IsUndoAvailableProperty = CreateIsUndoAvailableProperty();
		static DependencyProperty CreateIsUndoAvailableProperty() {
			return DependencyProperty.Register("IsUndoAvailable", typeof(bool), typeof(SpellingControlBase), new PropertyMetadata(false, OnIsUndoAvailableChanged));
		}
		static void OnIsUndoAvailableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellingOutlookStyleUndoCommand undoCommand = ((SpellingControlBase)d).UndoCommand as SpellingOutlookStyleUndoCommand;
			if (undoCommand != null)
				undoCommand.RaiseCanExecuteChanged();
		}
		public bool IsUndoAvailable {
			get { return (bool)GetValue(IsUndoAvailableProperty); }
			set { SetValue(IsUndoAvailableProperty, value); }
		}
		#endregion
		#region HasSuggestions
		public static readonly DependencyProperty HasSuggestionsProperty = CreateHasSuggestionsProperty();
		static DependencyProperty CreateHasSuggestionsProperty() {
			return DependencyProperty.Register("HasSuggestions", typeof(bool), typeof(SpellingControlBase), new PropertyMetadata(false));
		}
		public bool HasSuggestions {
			get { return (bool)GetValue(HasSuggestionsProperty); }
			set { SetValue(HasSuggestionsProperty, value); }
		}
		#endregion
		#region IsCustomDictionaryExits
		public static readonly DependencyProperty IsCustomDictionaryExitsProperty = CreateIsCustomDictionaryExitsProperty();
		static DependencyProperty CreateIsCustomDictionaryExitsProperty() {
			return DependencyProperty.Register("IsCustomDictionaryExits", typeof(bool), typeof(SpellingControlBase), new PropertyMetadata(true, OnIsCustomDictionaryExistsPropertyChanged));
		}
		static void OnIsCustomDictionaryExistsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellingControlBase control = d as SpellingControlBase;
			if (control == null)
				return;
			SpellingOutlookStyleBaseCommand addToDictionaryCommand = control.AddToDictionaryCommand as SpellingOutlookStyleBaseCommand;
			if (addToDictionaryCommand != null)
				addToDictionaryCommand.RaiseCanExecuteChanged();
		}
		public bool IsCustomDictionaryExits {
			get { return (bool)GetValue(IsCustomDictionaryExitsProperty); }
			set { SetValue(IsCustomDictionaryExitsProperty, value); }
		}
		#endregion
		#region CanDoChange
		public static readonly DependencyProperty CanDoChangeProperty = CreateCanDoChangeProperty();
		static DependencyProperty CreateCanDoChangeProperty() {
			return DependencyProperty.Register("CanDoChange", typeof(bool), typeof(SpellingControlBase), new PropertyMetadata(true, OnCanDoChangePropertyChanged));
		}
		static void OnCanDoChangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellingControlBase control = d as SpellingControlBase;
			if (control == null)
				return;
			SpellingOutlookStyleBaseCommand changeCommand = control.ChangeCommand as SpellingOutlookStyleBaseCommand;
			if (changeCommand != null)
				changeCommand.RaiseCanExecuteChanged();
			SpellingOutlookStyleBaseCommand changeAllCommand = control.ChangeAllCommand as SpellingOutlookStyleBaseCommand;
			if (changeAllCommand != null)
				changeAllCommand.RaiseCanExecuteChanged();
		}
		public bool CanDoChange {
			get { return (bool)GetValue(CanDoChangeProperty); }
			set { SetValue(CanDoChangeProperty, value); }
		}
		#endregion
		#endregion
		#region Events
		#region ResultChanged
		EventHandler resultChanged;
		public event EventHandler ResultChanged { add { resultChanged += value; } remove { resultChanged -= value; } }
		protected virtual void RaiseSpellCheckFormResultChanged() {
			EventHandler handler = resultChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this.layoutRoot = GetTemplateChild("LayoutRoot") as FrameworkElement;
		}
		protected override void Initialize() {
			ChangeCommand = new SpellingOutlookStyleChangeCommand(this);
			ChangeAllCommand = new SpellingOutlookStyleChangeAllCommand(this);
			DeleteCommand = new SpellingOutlookStyleDeleteCommand(this);
			IgnoreCommand = new SpellingOutlookStyleIgnoreCommand(this);
			IgnoreAllCommand = new SpellingOutlookStyleIgnoreAllCommand(this);
			AddToDictionaryCommand = new SpellingOutlookStyleAddToDictionaryCommand(this);
			CancelCommand = new SpellingOutlookStyleCancelCommand(this);
			UndoCommand = new SpellingOutlookStyleUndoCommand(this);
			ShowOptionsCommand = new SpellingOutlookStyleShowOptionsCommand(this);
		}
		void OnSuggestionChanged(string oldValue, string newValue) {
			CanDoChange = !String.IsNullOrEmpty(newValue) && !String.Equals(WrongWord, newValue, StringComparison.Ordinal);
			if (!String.IsNullOrEmpty(newValue))
				GoToState("NotInDictionaryWordError_ChangeWord");
			else
				GoToState("NotInDictionaryWordError_DeleteWord");
		}
		public void PopulateFormSuggestions(SuggestionCollection suggestions) {
			Suggestions.Clear();
			if (suggestions != null && suggestions.Count > 0) {
				int count = suggestions.Count;
				for (int i = 0; i < count; i++)
					Suggestions.Add(suggestions[i].Suggestion);
				HasSuggestions = true;
			}
			else {
				Suggestions.Add(SpellCheckerLocalizer.GetString(SpellCheckerStringId.Form_Spelling_NoSuggestions));
				HasSuggestions = false;
			}
		}
		internal void GoToState(string stateName) {
			try {
#if !SL
				if (this.layoutRoot == null)
					ApplyTemplate();
				VisualStateManager.GoToElementState(this.layoutRoot, stateName, false);
#else
				VisualStateManager.GoToState(this, stateName, false);
#endif
			}
			catch { }
		}
	}
	#endregion
	#region SpellingOutlookStyleControl
	[DXToolboxBrowsable(false)]
	[TemplatePart(Name = "LayoutRoot", Type = typeof(FrameworkElement))]
	public class SpellingOutlookStyleControl : SpellingControlBase {
		public SpellingOutlookStyleControl() : base() { }
		public SpellingOutlookStyleControl(SpellChecker spellChecker)
			: base(spellChecker) {
			DefaultStyleKey = typeof(SpellingOutlookStyleControl);
		}
		#region Obsolete methods
		[Obsolete("This method overload has become obsolete. To change wrong word, use ChangeCommand property.")]
		protected virtual void DoChange() {
			ChangeCommand.Execute(null);
		}
		[Obsolete("This method overload has become obsolete. To delete wrong word, use DeleteCommand property.")]
		protected virtual void DoDelete() {
			DeleteCommand.Execute(null);
		}
		[Obsolete("This method overload has become obsolete. To change all occurrence of the word in the text, use ChangeAllCommand property.")]
		protected virtual void DoChangeAll() {
			ChangeAllCommand.Execute(null);
		}
		[Obsolete("This method overload has become obsolete. To ignore wrong word, use IgnoreCommand property.")]
		protected virtual void DoIgnore() {
			IgnoreCommand.Execute(null);
		}
		[Obsolete("This method overload has become obsolete. To ignore all occurrence of the word in the text, use IgnoreAllCommand property.")]
		protected virtual void DoIgnoreAll() {
			IgnoreAllCommand.Execute(null);
		}
		[Obsolete("This method overload has become obsolete. To add your word in dictionary, use AddToDictionaryCommand property.")]
		protected virtual void DoAddToDictionary() {
			AddToDictionaryCommand.Execute(null);
		}
		[Obsolete("This method overload has become obsolete.To cancel, use CancelCommand property.")]
		protected virtual void DoCancel() {
			CancelCommand.Execute(null);
		}
		[Obsolete("This method overload has become obsolete. To undo your changes, use UdnoCommand property.")]
		protected virtual void DoUndo() {
			UndoCommand.Execute(null);
		}
		protected virtual void DoSuggest() {
		}
		[Obsolete("This method overload has become obsolete. To show options, use ShowOptionsCommand property.")]
		protected virtual void DoShowOptions() {
			ShowOptionsCommand.Execute(null);
		}
		#endregion
	}
	#endregion
	#region SpellingWordStyleControl
	[DXToolboxBrowsable(false)]
	[TemplatePart(Name = "LayoutRoot", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "FormEditor", Type = typeof(TextEdit))]
	public class SpellingWordStyleControl : SpellingControlBase {
		TextEdit editControl;
		public SpellingWordStyleControl() : base() { }
		public SpellingWordStyleControl(SpellChecker spellChecker)
			: base(spellChecker) {
			DefaultStyleKey = typeof(SpellingWordStyleControl);
		}
		public TextEdit EditControl { get { return editControl; } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this.editControl = GetTemplateChild("FormEditor") as TextEdit;
		}
		protected override void Initialize() {
			base.Initialize();
			ChangeCommand = new SpellingWordStyleChangeCommand(this);
			ChangeAllCommand = new SpellingWordStyleChangeAllCommand(this);
		}
	}
	#endregion
}
