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
#if !SL && !WPF
using System.Drawing.Design;
#endif
using System.ComponentModel;
using System.IO;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.XtraSpellChecker.Rules;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.XtraSpellChecker.Strategies;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace DevExpress.XtraSpellChecker {
	public enum SpellingFormType { Outlook, Word }
	[DXToolboxItem(false)]
	public class SpellCheckerBase :
#if !SL
 Component, IXtraSerializableLayoutEx
#else
		System.Windows.Controls.Control
#endif
 {
		const int defaultLevenshteinDistance = 3;
		const int defaultSuggestionSearchTimeOut = -1;
		CultureInfo culture;
		object editControl = null;
		int levenshteinDistance = defaultLevenshteinDistance;
		int suggestionSearchTimeOut;
		DictionaryCollection dictionaries;
		OptionsSpellingBase optionsSpelling;
		bool useSharedDictionaries = true;
		SpellingFormType spellCheckFormType = SpellingFormType.Outlook;
		Dictionary<string, string> changeAllList = null;
		SearchStrategy searchStrategy = null;
		OptionsSpellingBase savedOptions = null;
		IIgnoreList defaultIgnoreList = null;
		int suggestionCount = 5;
		bool isTextMode = false;
		bool loadOnDemand = false;
		WeakKeyDictionary<object, IIgnoreList> ignoreListTable = new WeakKeyDictionary<object, IIgnoreList>();
		DictionaryHelper dictionaryHelper;
		public SpellCheckerBase() {
			culture = CultureInfo.CurrentCulture;
			this.optionsSpelling = CreateOptionsSpelling();
			this.dictionaries = CreateDictionaryCollection();
			this.changeAllList = new Dictionary<string, string>();
			this.dictionaryHelper = CreateDictionaryHelper();
			this.suggestionSearchTimeOut = defaultSuggestionSearchTimeOut;
		}
		#region Properties
		#region UseSharedDictionaries
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseUseSharedDictionaries"),
#endif
		Category(SRCategryNames.SpellChecker),
		DefaultValue(true),
		XtraSerializableProperty()
		]
		public bool UseSharedDictionaries { get { return useSharedDictionaries; } set { useSharedDictionaries = value; } }
		#endregion
		#region SpellingFormType
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseSpellingFormType"),
#endif
		Category(SRCategryNames.SpellChecker),
		DefaultValue(SpellingFormType.Outlook),
		XtraSerializableProperty()
		]
		public SpellingFormType SpellingFormType {
			get { return spellCheckFormType; }
			set {
				if (spellCheckFormType == value)
					return;
				spellCheckFormType = value;
			}
		}
		#endregion
		#region Dictionaries
#if !SL && !WPF
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseDictionaries"),
#endif
		Category(SRCategryNames.SpellChecker),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraSpellChecker.Design.DictionaryItemCollectionEditor," + AssemblyInfo.SRAssemblySpellCheckerDesign, typeof(UITypeEditor))
		]
#endif
		public DictionaryCollection Dictionaries { get { return dictionaries; } }
		#endregion
		#region LevenshteinDistance
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseLevenshteinDistance"),
#endif
		Category(SRCategryNames.SpellChecker),
		DefaultValue(defaultLevenshteinDistance)
		]
		public int LevenshteinDistance { 
			get { return levenshteinDistance; }
			set {
				if (value < 1)
					value = 1;
				levenshteinDistance = value;
			}
		}
		#endregion
		#region SuggestionSearchTimeOut
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseSuggestionSearchTimeOut")]
#endif
		[Category(SRCategryNames.SpellChecker)]
		[DefaultValue(defaultSuggestionSearchTimeOut)]
		public int SuggestionSearchTimeOut { get { return suggestionSearchTimeOut; } set { suggestionSearchTimeOut = value; } }
		#endregion
		#region Culture
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseCulture"),
#endif
		Category(SRCategryNames.SpellChecker)
		] 
		public CultureInfo Culture {
			get { return culture; }
			set {
				if (!culture.Equals(value)) {
					culture = value;
					OnCultureChanged();
				}
			}
		}
		#endregion
		#region OptionsSpelling
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseOptionsSpelling"),
#endif
		Category(SRCategryNames.SpellChecker),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public OptionsSpellingBase OptionsSpelling { get { return optionsSpelling; } }
		#endregion
		#region LoadOnDemand
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseLoadOnDemand"),
#endif
		DefaultValue(false)]
		public bool LoadOnDemand {
			get {
				return loadOnDemand;
			}
			set {
				loadOnDemand = value;
				OnLoadOnDemandChanged();
			}
		}
		#endregion
		#region IgnoreList
		[Category(SRCategryNames.SpellChecker), Browsable(false)]
		public IIgnoreList IgnoreList { get { return GetIgnoreListCore(); } }
		#endregion
		#region ChangeAllList
		[Category(SRCategryNames.SpellChecker), Browsable(false)]
		public Dictionary<string, string> ChangeAllList { get { return changeAllList; } }
		#endregion
		#region DictionaryHelper
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DictionaryHelper DictionaryHelper {
			get {
				return dictionaryHelper;
			}
		}
		#endregion
		#region SearchStrategy
		protected internal SearchStrategy SearchStrategy {  
			get {
				if (searchStrategy == null)
					SetSearchStrategyCore(CreateSearchStrategy(EditControl));
				return searchStrategy;
			}
			set { SetSearchStrategyCore(value); }
		}
		#endregion
		#region EditControl
		protected internal virtual object EditControl {
			get { return editControl; }
			set {
				if (editControl != value) {
					editControl = value;
					OnEditControlModified();
				}
			}
		}
		#endregion
		#region DefaultIgnoreList
		protected IIgnoreList DefaultIgnoreList {
			get {
				if (defaultIgnoreList == null)
					defaultIgnoreList = CreateDefaultIgnoreList();
				return defaultIgnoreList;
			}
		}
		#endregion
		#region SavedOptions
		protected virtual OptionsSpellingBase SavedOptions {
			get {
				if (savedOptions == null)
					savedOptions = CreateOptionsSpelling();
				return savedOptions;
			}
		}
		#endregion
		protected internal virtual int SuggestionCount { get { return suggestionCount; } set { suggestionCount = value; } }
		protected virtual bool IsChecking { get { return IsStrategyExists && SearchStrategy.State != StrategyState.None; } }
		protected internal WeakKeyDictionary<object, IIgnoreList> IgnoreListTable { get { return ignoreListTable; } }
		protected bool IsStrategyExists { get { return searchStrategy != null; } }
		#endregion
		#region Events
		#region PrepareSuggestions
		PrepareSuggestionsEventHandler onPrepareSuggestions;
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBasePrepareSuggestions"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event PrepareSuggestionsEventHandler PrepareSuggestions { add { onPrepareSuggestions += value; } remove { onPrepareSuggestions -= value; } }
		protected internal virtual void OnPrepareSuggestions(PrepareSuggestionsEventArgs e) {
			RaisePrepareSuggestions(e);
		}
		protected virtual void RaisePrepareSuggestions(PrepareSuggestionsEventArgs e) {
			if (onPrepareSuggestions != null)
				onPrepareSuggestions(this, e);
		}
		#endregion
		#region FinishCheckingMainPart
		FinishCheckingMainPartEventHandler onFinishCheckingMainPart;
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseFinishCheckingMainPart"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event FinishCheckingMainPartEventHandler FinishCheckingMainPart {
			add { onFinishCheckingMainPart += value; }
			remove { onFinishCheckingMainPart -= value; }
		}
		protected internal virtual void OnFinishCheckingMainPart(FinishCheckingMainPartEventArgs e) {
			RaiseFinishCheckingMainPart(e);
		}
		protected virtual void RaiseFinishCheckingMainPart(FinishCheckingMainPartEventArgs e) {
			if (IsOnFinishCheckingMainPartEventAssigned())
				onFinishCheckingMainPart(this, e);
		}
		protected internal bool IsOnFinishCheckingMainPartEventAssigned() {
			return onFinishCheckingMainPart != null;
		}
		#endregion
		#region RepeatedWordFound
		RepeatedWordFoundEventHandler onRepeatedWordFound;
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseRepeatedWordFound"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event RepeatedWordFoundEventHandler RepeatedWordFound {
			add { onRepeatedWordFound += value; }
			remove { onRepeatedWordFound -= value; }
		}
		protected internal virtual void OnRepeatedWordFound(RepeatedWordFoundEventArgs e) {
			RaiseRepeatedWordFound(e);
		}
		protected virtual void RaiseRepeatedWordFound(RepeatedWordFoundEventArgs e) {
			if (onRepeatedWordFound != null)
				onRepeatedWordFound(this, e);
		}
		#endregion
		#region NotInDictionaryWordFound
		NotInDictionaryWordFoundEventHandler onNotInDictionaryWordFound;
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseNotInDictionaryWordFound"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event NotInDictionaryWordFoundEventHandler NotInDictionaryWordFound {
			add { onNotInDictionaryWordFound += value; }
			remove { onNotInDictionaryWordFound -= value; }
		}
		protected internal virtual void OnNotInDictionaryWordFound(NotInDictionaryWordFoundEventArgs e) {
			RaiseNotInDictionaryWordFound(e);
		}
		protected virtual void RaiseNotInDictionaryWordFound(NotInDictionaryWordFoundEventArgs e) {
			if (onNotInDictionaryWordFound != null)
				onNotInDictionaryWordFound(this, e);
		}
		#endregion
		#region BeforeCheck
		BeforeCheckEventHandler onBeforeCheck;
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseBeforeCheck"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event BeforeCheckEventHandler BeforeCheck {
			add { onBeforeCheck += value; }
			remove { onBeforeCheck -= value; }
		}
		protected internal virtual void OnBeforeCheck(BeforeCheckEventArgs e) {
			RaiseBeforeCheck(e);
		}
		protected virtual void RaiseBeforeCheck(BeforeCheckEventArgs e) {
			if (onBeforeCheck != null)
				onBeforeCheck(this, e);
		}
		#endregion
		#region BeforeCheckWord
		BeforeCheckWordEventHandler onBeforeCheckWord;
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseBeforeCheckWord"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event BeforeCheckWordEventHandler BeforeCheckWord {
			add { onBeforeCheckWord += value; }
			remove { onBeforeCheckWord -= value; }
		}
		protected virtual void RaiseBeforeCheckWord(BeforeCheckWordEventArgs e) {
			if (onBeforeCheckWord != null)
				onBeforeCheckWord(this, e);
		}
		protected internal virtual void OnBeforeCheckWord(BeforeCheckWordEventArgs e) {
			RaiseBeforeCheckWord(e);
		}
		#endregion
		#region AfterCheckWord
		AfterCheckWordEventHandler onAfterCheckWord;
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseAfterCheckWord"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event AfterCheckWordEventHandler AfterCheckWord {
			add { onAfterCheckWord += value; }
			remove { onAfterCheckWord -= value; }
		}
		protected virtual void RaiseAfterCheckWord(AfterCheckWordEventArgs e) {
			if (onAfterCheckWord != null)
				onAfterCheckWord(this, e);
		}
		protected internal virtual void OnAfterCheckWord(AfterCheckWordEventArgs e) {
			RaiseAfterCheckWord(e);
		}
		#endregion
		#region AfterCheck
		AfterCheckEventHandler onAfterCheck;
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseAfterCheck"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event AfterCheckEventHandler AfterCheck {
			add { onAfterCheck += value; }
			remove { onAfterCheck -= value; }
		}
		protected internal virtual void OnAfterCheck(AfterCheckEventArgs e) {
			RaiseAfterCheck(e);
		}
		protected virtual void RaiseAfterCheck(AfterCheckEventArgs e) {
			if (onAfterCheck != null)
				onAfterCheck(this, e);
		}
		#endregion
		#region BeforeLoadDictionaries
		EventHandler onBeforeLoadDictionaries;
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseBeforeLoadDictionaries"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event EventHandler BeforeLoadDictionaries {
			add { onBeforeLoadDictionaries += value; }
			remove { onBeforeLoadDictionaries -= value; }
		}
		protected internal virtual void OnBeforeLoadDictionaries() {
			RaiseBeforeLoadDictionaries(EventArgs.Empty);
		}
		protected virtual void RaiseBeforeLoadDictionaries(EventArgs e) {
			if (onBeforeLoadDictionaries != null)
				onBeforeLoadDictionaries(this, e);
		}
		#endregion
		#region AfterLoadDictionaries
		EventHandler onAfterLoadDictionaries;
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseAfterLoadDictionaries"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event EventHandler AfterLoadDictionaries {
			add { onAfterLoadDictionaries += value; }
			remove { onAfterLoadDictionaries -= value; }
		}
		protected internal virtual void OnAfterLoadDictionaries() {
			RaiseAfterLoadDictionaries(EventArgs.Empty);
		}
		protected virtual void RaiseAfterLoadDictionaries(EventArgs e) {
			if (onAfterLoadDictionaries != null)
				onAfterLoadDictionaries(this, e);
		}
		#endregion
		#region WordAdded
		WordAddedEventHandler onWordAdded;
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseWordAdded"),
#endif
		Category(SRCategryNames.SpellChecker)]
		public event WordAddedEventHandler WordAdded { add { onWordAdded += value; } remove { onWordAdded -= value; } }
		protected virtual void RaiseWordAdded(WordAddedEventArgs e) {
			if (onWordAdded != null)
				onWordAdded(this, e);
		}
		protected internal virtual void OnWordAdded(string word) {
			WordAddedEventArgs e = new WordAddedEventArgs(word);
			RaiseWordAdded(e);
		}
		#endregion
		#region UnhandledException
		SpellCheckerUnhandledExceptionEventHandler onUnhandledException;
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerBaseUnhandledException")]
#endif
		public event SpellCheckerUnhandledExceptionEventHandler UnhandledException { add { onUnhandledException += value; } remove { onUnhandledException -= value; } }
		protected internal virtual bool RaiseUnhandledException(Exception e) {
			try {
				if (onUnhandledException != null) {
					SpellCheckerUnhandledExceptionEventArgs args = new SpellCheckerUnhandledExceptionEventArgs(e);
					onUnhandledException(this, args);
					return args.Handled;
				}
				else
					return false;
			}
			catch {
				return false;
			}
		}
		#endregion
		#endregion
		protected internal void RegisterIgnoreList(object control, IIgnoreList ignoreList) {
			try {
				Guard.ArgumentNotNull(ignoreList, "ignoreList");
				if (control != null && !IgnoreListTable.ContainsKey(control)) {
					SubscribeToControlEvents(control);
					IgnoreListTable.Add(control, ignoreList);
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		protected virtual void SubscribeToControlEvents(object control) {
		}
		protected virtual void UnSubscribeFromControlEvents(object control) {
		}
		protected internal virtual void UnregisterIgnoreList(object control) {
			try {
				Guard.ArgumentNotNull(control, "control");
				UnSubscribeFromControlEvents(control);
				if (IgnoreListTable.ContainsKey(control))
					IgnoreListTable.Remove(control);
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		protected void ClearIgnoreListTable() {
			if (IgnoreListTable != null) {
				IEnumerator enumerator = IgnoreListTable.Keys.GetEnumerator();
				List<object> keys = new List<object>();
				enumerator.Reset();
				while (enumerator.MoveNext())
					keys.Add(enumerator.Current);
				while (IgnoreListTable.Count > 0) {
					WeakReference key = (WeakReference)keys[0];
					if (key.IsAlive)
						UnregisterIgnoreList(key.Target);
					keys.RemoveAt(0);
				}
				IgnoreListTable.Clear();
				keys.Clear();
			}
		}
		protected internal virtual IIgnoreList GetIgnoreListCore(object control) {
			if (control == null)
				return DefaultIgnoreList;
			if (IgnoreListTable.ContainsKey(control))
				return IgnoreListTable[control];
			IIgnoreList result = CreateIgnoreList();
			RegisterIgnoreList(control, result);
			return result;
		}
		protected internal virtual IIgnoreList GetIgnoreListCore() {
			return GetIgnoreListCore(EditControl);
		}
		protected virtual IIgnoreList CreateDefaultIgnoreList() {
			return new IgnoreList();
		}
		protected virtual void OnEditControlModified() {
			RegisterIgnoreList(EditControl, CreateIgnoreList());
			if (searchStrategy == null)
				return;
			if (SearchStrategy.TextController is IDisposable)
				(SearchStrategy.TextController as IDisposable).Dispose();
			ISpellCheckTextController controller = CreateTextController();
			SearchStrategy.TextController = controller;
		}
		protected virtual IIgnoreList CreateIgnoreList() {
			return new IgnoreList();
		}
		protected virtual ISpellCheckTextController CreateTextController() {
			return new SimpleTextController();
		}
		protected void SetSearchStrategyCore(SearchStrategy value) {
			this.searchStrategy = value;
		}
		protected internal virtual bool CanHandleErrorVisually() {
			return false;
		}
		protected internal virtual DictionaryHelper CreateDictionaryHelper() {
			return new DictionaryHelper(this, GetSharedDictionariesCollection());
		}
		protected virtual void OnLoadOnDemandChanged() {
			if (!LoadOnDemand)
				LoadDictionaries(Culture);
		}
		protected internal void SetOptionsSpellingCore(OptionsSpellingBase optionsSpelling) {
			this.optionsSpelling = optionsSpelling;
		}
		protected virtual List<SpellCheckerCommand> GetCommandsByError(TextOperationsManager manager, SpellCheckErrorBase error) {
			if (error == null)
				return null;
			List<SpellCheckerCommand> result = new List<SpellCheckerCommand>();
			if (error is NotInDictionaryWordError) {
				if (error.Suggestions != null && error.Suggestions.Count > 0)
					CreateChangeCommands(manager, error, result);
				else
					result.Add(manager.GetCommand(SpellCheckOperation.None, string.Empty, error.WrongWord, error.StartPosition, error.FinishPosition, error.Culture));
				result.Add(manager.GetCommand(SpellCheckOperation.IgnoreAll, string.Empty, error.WrongWord, error.StartPosition, error.FinishPosition, error.Culture));
				result.Add(manager.GetCommand(SpellCheckOperation.AddToDictionary, string.Empty, error.WrongWord, error.StartPosition, error.FinishPosition, error.Culture));
			}
			if (error is RepeatedWordError) {
				result.Add(manager.GetCommand(SpellCheckOperation.Delete, string.Empty, error.WrongWord, error.StartPosition, error.FinishPosition, error.Culture));
				result.Add(manager.GetCommand(SpellCheckOperation.Ignore, string.Empty, error.WrongWord, error.StartPosition, error.FinishPosition, error.Culture));
			}
			return result;
		}
		void CreateChangeCommands(TextOperationsManager manager, SpellCheckErrorBase error, List<SpellCheckerCommand> result) {
			SuggestionCollection suggestions = error.Suggestions;
			int count = suggestions.Count;
			for (int i = 0; i < count; i++) {
				string suggestion = suggestions[i].Suggestion;
				if (!String.IsNullOrEmpty(suggestion))
					result.Add(manager.GetCommand(SpellCheckOperation.Change, suggestion, error.WrongWord, error.StartPosition, error.FinishPosition, error.Culture));
			}
		}
		public virtual List<SpellCheckerCommand> GetCommandsByError(SpellCheckErrorBase error) {
			return GetCommandsByError(SearchStrategy.OperationsManager, error);
		}
		public virtual SuggestionCollection GetSuggestions(string word, CultureInfo culture) {
			SuggestionCollection result = new SuggestionCollection();
			try {
				CultureInfo usedCulture = culture ?? Culture;
				SuggestionCollection suggestions = SuggestionsHelper.CalcSuggestions(word, DictionaryHelper, SuggestionSearchTimeOut, LevenshteinDistance, usedCulture);
				int maxCount = SuggestionCount;
				int count = suggestions.Count;
				for (int i = 0; i < count && result.Count < maxCount; i++)
					result.Add(suggestions[i]);
				OnPrepareSuggestions(new PrepareSuggestionsEventArgs(word, result));
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw e;
			}
			return result;
		}
		public virtual SuggestionCollection GetSuggestions(string word) {
			return GetSuggestions(word, Culture);
		}
		[System.Security.SecuritySafeCritical]
		protected virtual DictionaryCollection CreateDictionaryCollection() {
			return new DictionaryCollection();
		}
		protected virtual SearchStrategy CreateSearchStrategy(object editControl) {
			return new SimpleTextSearchStrategy(this, CreateTextController());
		}
		protected virtual SearchStrategy CreateSearchStrategy(ISpellCheckTextController textController) {
			return new SimpleTextSearchStrategy(this, textController);
		}
		protected virtual SearchStrategy CreateSearchStrategy(ISpellCheckTextController textController, ContainerSearchStrategyBase parentStrategy) {
			return new SimpleTextSearchStrategy(this, textController, parentStrategy);
		}
		protected virtual SearchStrategy CreateSearchStrategy(Position start, Position finish) {
			return new PartSimpleTextSearchStrategy(this, CreateTextController(), start, finish);
		}
		protected virtual OptionsSpellingBase CreateOptionsSpelling() {
			return new OptionsSpellingBase();
		}
		protected virtual void OnCultureChanged() {
			defaultIgnoreList = null;
			LoadDictionaries(Culture);
			SearchStrategy.Culture = Culture;
		}
		protected virtual bool CanCheck() {
			return !IsChecking;
		}
		#region ISpellChecker API functions
		protected virtual bool CanAddToDictionary(CultureInfo culture) {
			if (culture != null) {
				try {
					return DictionaryHelper.GetCustomDictionary(culture) != null;
				}
				catch (Exception e) {
					if (!HandleException(e))
						throw;
				}
			}
			return false;
		}
		protected virtual void AddToDictionary(string word, CultureInfo culture) {
			try {
				DictionaryHelper.AddWord(word, culture);
				RaiseWordAdded(new WordAddedEventArgs(word));
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		protected virtual void Ignore(object control, string word, Position start, Position end) {
			try {
				IIgnoreList ignoreList;
				if (!IgnoreListTable.TryGetValue(control, out ignoreList))
					return;
				ignoreList.Add(start, end, word);
				RaiseAfterCheckWord(new AfterCheckWordEventArgs(control, word, word, SpellCheckOperation.Ignore, start));
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		protected virtual void IgnoreAll(object control, string word) {
			try {
				IIgnoreList ignoreList;
				if (!IgnoreListTable.TryGetValue(control, out ignoreList))
					return;
				ignoreList.Add(word);
				RaiseAfterCheckWord(new AfterCheckWordEventArgs(control, word, word, SpellCheckOperation.IgnoreAll, Position.Null));
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		#endregion
		#region Check Methods
		protected internal virtual bool IsTextMode {
			get { return isTextMode; }
			set { isTextMode = value; }
		}
		public virtual string Check(string text) {
			string result = text;
			try {
				DoBeforeCheck();
				try {
					SearchStrategy.Text = text;
					SearchStrategy.Check();
					result = SearchStrategy.Text;
				}
				finally {
					DoAfterCheck(SearchStrategy);
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
			return result;
		}
		[Obsolete("Use the CheckText(string) to check text and the WrongWordRecord.GetSuggestions method to get suggestions instead.")]
		public virtual List<WrongWordRecord> CheckText(string text, bool needSuggestions) {
			return CheckText(text);
		}
		public virtual List<WrongWordRecord> CheckText(string text) {
			List<WrongWordRecord> result = null;
			try {
				IsTextMode = true;
				DoBeforeCheck();
				try {
					SearchStrategy.Text = text;
					SearchStrategy.Check();
					result = SearchStrategy.WrongWords;
				}
				finally {
					DoAfterCheck(SearchStrategy);
					IsTextMode = false;
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
			return result;
		}
		bool threadChecking = false;
		protected internal bool ThreadChecking {  
			get { return threadChecking; }
			set { threadChecking = value; }
		}
		ExceptionPosition richEditWordPosition;
		protected internal ExceptionPosition RichEditWordPosition {
			get { return richEditWordPosition; }
			set { richEditWordPosition = value; }
		}
		protected void SaveRichEditWordPosition(Position start, Position finish) {
			ExceptionPosition pos = new ExceptionPosition();
			pos.StartPosition = start;
			pos.FinishPosition = finish;
			RichEditWordPosition = pos;
		}
		public virtual bool IsMisspelledWord(string text, CultureInfo culture) {
			bool result = false;
			ThreadChecking = true;
			Culture = culture;
			IsTextMode = true;
			LoadDictionaries(culture);
			try {
				using (SimpleTextSearchStrategy str = new SimpleTextSearchStrategy(this, new SimpleTextController())) {
					str.Text = text;
					result = str.IsWordMisspelled();
				}
			}
			finally {
				ThreadChecking = false;
				IsTextMode = false;
			}
			return result;
		}
		protected ICheckSpellingResult CheckText(string text, int index, CultureInfo culture) {
			try {
				ThreadChecking = true;
				Culture = culture;
				IsTextMode = true;
				LoadDictionaries(culture);
				try {
					using (RichEditCheckStrategy str = new RichEditCheckStrategy(this, new SimpleTextController())) {
						str.ThreadChecking = true;
						str.Text = text;
						str.CurrentPosition = new IntPosition(index);
						return str.CheckText();
					}
				}
				finally {
					IsTextMode = false;
					ThreadChecking = false;
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
			return null;
		}
		protected ISpellingErrorInfo Check(ISpellCheckTextController controller, Position from, Position to) {
			try {
				ThreadChecking = true;
				Culture = culture;
				IsTextMode = true;
				LoadDictionaries(Culture);
				try {
					using (SimpleTextSearchStrategy str = new SimpleTextSearchStrategy(this, controller, from, to)) {
						str.NeedSuggestions = false;
						str.ThreadChecking = true;
						str.Check();
						str.TextController = null;
						if (str.ActiveError != null) {
							SpellingError error = (str.ActiveError is RepeatedWordError) ? SpellingError.Repeating : SpellingError.Misspelling;
							return new SpellingErrorInfo(str.ActiveError.WrongWord, str.ActiveError.StartPosition, str.ActiveError.FinishPosition, error);
						}
						else
							return null;
					}
				}
				finally {
					IsTextMode = false;
					ThreadChecking = false;
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
			return null;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool IsMisspelledWord(string text, CultureInfo culture, Position start, Position finish) {
			try {
				if (CanCheck()) {
					ThreadChecking = true;
					Culture = culture;
					IsTextMode = true;
					DoBeforeCheck();
					try {
						SearchStrategy.Text = text;
						return SearchStrategy.IsWordMisspelled();
					}
					finally {
						DoAfterCheck(SearchStrategy);
						ThreadChecking = false;
						IsTextMode = false;
					}
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
			return false;
		}
		public virtual string Check(string text, Position start, Position finish) {
			string result = text;
			try {
				if (CanCheck()) {
					IsTextMode = true;
					DoBeforeCheck();
					try {
						SearchStrategy = CreateSearchStrategy(start, finish);
						SearchStrategy.Text = text;
						SearchStrategy.Check();
						result = SearchStrategy.Text;
					}
					finally {
						DoAfterCheck(SearchStrategy);
						IsTextMode = false;
					}
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
			return result;
		}
		public virtual string Check(ISpellCheckTextController controller) {
			string result = string.Empty;
			try {
				if (!CanCheck())
					return result;
				DoBeforeCheck();
				try {
					SearchStrategy = CreateSearchStrategy(controller);
					SearchStrategy.Check();
					result = SearchStrategy.Text;
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
			return result;
		}
		protected internal virtual string Check(ISpellCheckTextController controller, ContainerSearchStrategyBase parentStrategy) {
			try {
				if (CanCheck()) {
					DoBeforeCheck();
					try {
						SearchStrategy = CreateSearchStrategy(controller, parentStrategy);
						SearchStrategy.Check();
					}
					finally {
						SearchStrategy.TextController = null;
						DoAfterCheck(SearchStrategy);
					}
					return SearchStrategy.Text;
				}
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
			return String.Empty;
		}
		protected internal virtual void Check(object control, ContainerSearchStrategyBase parentStrategy) { }
		protected virtual void RestoreComponentOptions() {
			OptionsSpelling.Assign(SavedOptions);
		}
		protected virtual void SaveComponentOptions() {
			SavedOptions.Assign(OptionsSpelling);
		}
		protected virtual void DoBeforeCheck() {
			LoadDictionaries(Culture);
			DestroySearchStrategy();
			SaveComponentOptions();
		}
		protected internal virtual void DoAfterCheck(SearchStrategy strategy) {
			DestroySearchStrategy();
			EditControl = null;
			RestoreComponentOptions();
		}
		#endregion
		#region LoadDictionaries
		protected virtual DictionaryCollection GetSharedDictionariesCollection() {
			return DictionaryCollection.Empty;
		}
		protected virtual bool ShouldLoadDictionaries() {
			for (int i = 0; i < Dictionaries.Count; i++)
				if (!Dictionaries[i].Loaded)
					return true;
			if (UseSharedDictionaries) {
				DictionaryCollection sharedCollection = GetSharedDictionariesCollection();
				for (int i = 0; i < sharedCollection.Count; i++)
					if (!sharedCollection[i].Loaded)
						return true;
			}
			return false;
		}
		protected internal virtual void LoadDictionaries(CultureInfo culture) {
			if (!ShouldLoadDictionaries()) return;
			OnBeforeLoadDictionaries();
			try {
				LoadDictionariesCore(Dictionaries, culture);
				if (UseSharedDictionaries)
					LoadDictionariesCore(GetSharedDictionariesCollection(), culture);
			}
			finally {
				OnAfterLoadDictionaries();
			}
		}
		protected virtual void LoadDictionariesCore(DictionaryCollection dictionaries, CultureInfo culture) {
			int count = dictionaries.Count;
			for (int i = 0; i < count; i++) {
				ISpellCheckerDictionary item = dictionaries[i];
				if (!item.Loaded && (item.Culture.Equals(culture) || !LoadOnDemand)) {
					try {
						item.Load();
					}
					catch (NotLoadedDictionaryException) {
						OnDictionaryNotLoaded(item);
					}
				}
			}
		}
		protected virtual void OnDictionaryNotLoaded(ISpellCheckerDictionary dict) {
		}
		#endregion
		#region Serialization
#if !SL
		public void SaveToXML(string xmlFile) {
			SaveToCore(new XmlXtraSerializer(), xmlFile);
		}
		public void SaveToRegistry(string path) {
			SaveToCore(new RegistryXtraSerializer(), path);
		}
		public void SaveToStream(Stream stream) {
			SaveToCore(new XmlXtraSerializer(), stream);
		}
		public void RestoreFromXML(string xmlFile) {
			RestoreCore(new XmlXtraSerializer(), xmlFile, null);
		}
		public void RestoreFromRegistry(string path) {
			RestoreCore(new RegistryXtraSerializer(), path, null);
		}
		public void RestoreFromStream(Stream stream) {
			RestoreCore(new XmlXtraSerializer(), stream, null);
		}
		protected virtual void SaveToCore(XtraSerializer serializer, object path) {
			Stream stream = path as Stream;
			if (stream != null)
				serializer.SerializeObject(this, stream, "SpellChecker", null);
			else
				serializer.SerializeObject(this, path.ToString(), "SpellChecker", null);
		}
		protected virtual void RestoreCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			Stream stream = path as Stream;
			if (stream != null)
				serializer.DeserializeObject(this, stream, "SpellChecker", null);
			else
				serializer.DeserializeObject(this, path.ToString(), "SpellChecker", null);
		}
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			return true;
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) { }
		protected virtual bool ShouldSerializeOptionsSpelling() { 
			return OptionsSpelling.ShouldSerialize();
		}
#endif
		#endregion
		protected virtual void DestroySearchStrategy() {
			if (IsStrategyExists) {
				searchStrategy.Dispose();
				searchStrategy = null;
			}
		}
		protected internal virtual bool HandleException(Exception e) {
			return RaiseUnhandledException(e);
		}
	}
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public class OptionsSpellingBase : IOptionsSpellings {
		int updateCount = 0;
		bool needRaiseChangedEvent = false;
		public void BeginUpdate() {
			needRaiseChangedEvent = false;
			updateCount++;
		}
		public void EndUpdate() {
			updateCount--;
			if (!IsLocked && needRaiseChangedEvent)
				RaiseOptionsChangedHandler(EventArgs.Empty);
		}
		private bool IsLocked { get { return updateCount != 0; } }
		#region Options
		DefaultBoolean ignoreWordsWithNumbers = DefaultBoolean.Default;
		DefaultBoolean ignoreUpperCaseWords = DefaultBoolean.Default;
		DefaultBoolean ignoreMixedCaseWords = DefaultBoolean.Default;
		DefaultBoolean ignoreEmails = DefaultBoolean.Default;
		DefaultBoolean ignoreUrls = DefaultBoolean.Default;
		DefaultBoolean ignoreUri = DefaultBoolean.Default;
		DefaultBoolean ignoreRepeatedWords = DefaultBoolean.Default;
		DefaultBoolean checkSelectedTextFirst = DefaultBoolean.Default;
		DefaultBoolean checkFromCursorPos = DefaultBoolean.Default;
		DefaultBoolean ignoreTagContent = DefaultBoolean.Default;
		#region IgnoreWordsWithNumbers
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("OptionsSpellingBaseIgnoreWordsWithNumbers"),
#endif
		DefaultValue(DefaultBoolean.Default),
		XtraSerializableProperty()
		]
		public virtual DefaultBoolean IgnoreWordsWithNumbers {
			get { return ignoreWordsWithNumbers; }
			set {
				if (ignoreWordsWithNumbers != value) {
					ignoreWordsWithNumbers = value;
					needRaiseChangedEvent = true;
					RaiseOptionsChangedHandler(EventArgs.Empty);
				}
			}
		}
		protected internal virtual bool IsIgnoreWordsWithNumbers() {
			if (IgnoreWordsWithNumbers == DefaultBoolean.False)
				return false;
			return true;
		}
		protected internal bool ShouldSerializeIgnoreWordsWithNumbers() {
			return IgnoreWordsWithNumbers != DefaultBoolean.Default;
		}
		#endregion
		#region IgnoreUpperCaseWords
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("OptionsSpellingBaseIgnoreUpperCaseWords"),
#endif
		DefaultValue(DefaultBoolean.Default),
		XtraSerializableProperty()
		]
		public virtual DefaultBoolean IgnoreUpperCaseWords {
			get { return ignoreUpperCaseWords; }
			set {
				if (ignoreUpperCaseWords != value) {
					ignoreUpperCaseWords = value;
					needRaiseChangedEvent = true;
					RaiseOptionsChangedHandler(EventArgs.Empty);
				}
			}
		}
		protected internal virtual bool IsIgnoreUpperCaseWords() {
			if (IgnoreUpperCaseWords == DefaultBoolean.False)
				return false;
			return true;
		}
		protected internal bool ShouldSerializeIgnoreUpperCaseWords() {
			return IgnoreUpperCaseWords != DefaultBoolean.Default;
		}
		#endregion
		#region IgnoreMixedCaseWords
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("OptionsSpellingBaseIgnoreMixedCaseWords"),
#endif
		DefaultValue(DefaultBoolean.Default),
		XtraSerializableProperty()
		]
		public virtual DefaultBoolean IgnoreMixedCaseWords {
			get { return ignoreMixedCaseWords; }
			set {
				if (ignoreMixedCaseWords != value) {
					ignoreMixedCaseWords = value;
					needRaiseChangedEvent = true;
					RaiseOptionsChangedHandler(EventArgs.Empty);
				}
			}
		}
		protected internal bool ShouldSerializeIgnoreMixedCaseWords() {
			return IgnoreMixedCaseWords != DefaultBoolean.Default;
		}
		protected internal virtual bool IsIgnoreMixedCaseWords() {
			if (IgnoreMixedCaseWords == DefaultBoolean.False)
				return false;
			return true;
		}
		#endregion
		#region IgnoreEmails
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("OptionsSpellingBaseIgnoreEmails"),
#endif
		DefaultValue(DefaultBoolean.Default),
		XtraSerializableProperty()
		]
		public virtual DefaultBoolean IgnoreEmails {
			get { return ignoreEmails; }
			set {
				if (ignoreEmails != value) {
					ignoreEmails = value;
					needRaiseChangedEvent = true;
					RaiseOptionsChangedHandler(EventArgs.Empty);
				}
			}
		}
		protected internal bool ShouldSerializeIgnoreEmails() {
			return IgnoreEmails != DefaultBoolean.Default;
		}
		protected internal virtual bool IsIgnoreEmails() {
			if (IgnoreEmails == DefaultBoolean.False)
				return false;
			return true;
		}
		#endregion
		#region IgnoreUrls
		[
		Obsolete("This property has become obsolete. Use the IgnoreUri property instead."),
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("OptionsSpellingBaseIgnoreUrls"),
#endif
		DefaultValue(DefaultBoolean.Default),
		XtraSerializableProperty()
		]
		public virtual DefaultBoolean IgnoreUrls {
			get { return ignoreUrls; }
			set {
				if (ignoreUrls != value) {
					ignoreUrls = value;
					needRaiseChangedEvent = true;
					RaiseOptionsChangedHandler(EventArgs.Empty);
				}
			}
		}
		protected internal bool ShouldSerializeIgnoreUrls() {
#pragma warning disable 0618
			return IgnoreUrls != DefaultBoolean.Default;
#pragma warning restore 0618
		}
		protected internal virtual bool IsIgnoreUrls() {
#pragma warning disable 0618
			if (IgnoreUrls == DefaultBoolean.False)
				return false;
			return true;
#pragma warning restore 0618
		}
		#endregion
		#region IgnoreUri
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("OptionsSpellingBaseIgnoreUri")]
#endif
		[XtraSerializableProperty()]
		[DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean IgnoreUri {
			get { return ignoreUri; }
			set {
				if (ignoreUri == value)
					return;
				ignoreUri = value;
				needRaiseChangedEvent = true;
				RaiseOptionsChangedHandler(EventArgs.Empty);
			}
		}
		protected internal bool ShouldSerializeIgnoreUri() {
			return IgnoreUri != DefaultBoolean.Default;
		}
		protected internal virtual bool IsIgnoreUri() {
			if (IgnoreUri == DefaultBoolean.False)
				return false;
			return true;
		}
		#endregion
		#region IgnoreRepeatedWords
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("OptionsSpellingBaseIgnoreRepeatedWords"),
#endif
		DefaultValue(DefaultBoolean.Default),
		XtraSerializableProperty()
		]
		public virtual DefaultBoolean IgnoreRepeatedWords {
			get { return ignoreRepeatedWords; }
			set {
				if (IgnoreRepeatedWords != value) {
					ignoreRepeatedWords = value;
					needRaiseChangedEvent = true;
					RaiseOptionsChangedHandler(EventArgs.Empty);
				}
			}
		}
		protected internal bool ShouldSerializeIgnoreRepeatedWords() {
			return IgnoreRepeatedWords != DefaultBoolean.Default;
		}
		protected internal virtual bool IsIgnoreRepeatedWords() {
			if (IgnoreRepeatedWords == DefaultBoolean.True)
				return true;
			return false;
		}
		#endregion
		#region CheckSelectedTextFirst
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("OptionsSpellingBaseCheckSelectedTextFirst"),
#endif
		DefaultValue(DefaultBoolean.Default),
		XtraSerializableProperty()
		]
		public virtual DefaultBoolean CheckSelectedTextFirst {
			get { return checkSelectedTextFirst; }
			set {
				if (CheckSelectedTextFirst != value) {
					checkSelectedTextFirst = value;
					needRaiseChangedEvent = true;
					RaiseOptionsChangedHandler(EventArgs.Empty);
				}
			}
		}
		protected internal bool ShouldSerializeCheckSelectedTextFirst() {
			return CheckSelectedTextFirst != DefaultBoolean.Default;
		}
		protected internal virtual bool IsCheckSelectedTextFirst() {
			if (CheckSelectedTextFirst == DefaultBoolean.False)
				return false;
			return true;
		}
		#endregion
		#region CheckFromCursorPos
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("OptionsSpellingBaseCheckFromCursorPos"),
#endif
		DefaultValue(DefaultBoolean.Default),
		XtraSerializableProperty()
		]
		public virtual DefaultBoolean CheckFromCursorPos {
			get { return checkFromCursorPos; }
			set {
				if (CheckFromCursorPos != value) {
					checkFromCursorPos = value;
					needRaiseChangedEvent = true;
					RaiseOptionsChangedHandler(EventArgs.Empty);
				}
			}
		}
		protected internal bool ShouldSerializeCheckFromCursorPos() {
			return CheckFromCursorPos != DefaultBoolean.Default;
		}
		protected internal virtual bool IsCheckFromCursorPos() {
			if (CheckFromCursorPos == DefaultBoolean.False)
				return false;
			return true;
		}
		#endregion
		#region IgnoreHtmlTagsContent
		[
		DefaultValue(DefaultBoolean.Default), Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty()
		]
		public virtual DefaultBoolean IgnoreMarkupTags {
			get { return ignoreTagContent; }
			set {
				if (IgnoreMarkupTags != value) {
					ignoreTagContent = value;
					needRaiseChangedEvent = true;
					RaiseOptionsChangedHandler(EventArgs.Empty);
				}
			}
		}
		protected internal bool ShouldSerializeIgnoreTagContent() {
			return IgnoreMarkupTags != DefaultBoolean.Default;
		}
		protected internal virtual bool IsIgnoreMarkupTags() {
			if (IgnoreMarkupTags == DefaultBoolean.True)
				return true;
			return false;
		}
		#endregion
		protected internal virtual bool ShouldSerialize() {
			return
				ShouldSerializeIgnoreWordsWithNumbers() ||
				ShouldSerializeIgnoreUpperCaseWords() ||
				ShouldSerializeIgnoreEmails() ||
				ShouldSerializeIgnoreUrls() ||
				ShouldSerializeIgnoreRepeatedWords() ||
				ShouldSerializeIgnoreMixedCaseWords() ||
				ShouldSerializeCheckFromCursorPos() ||
				ShouldSerializeCheckSelectedTextFirst() ||
				ShouldSerializeIgnoreTagContent() ||
				ShouldSerializeIgnoreUri();
		}
		#endregion
		[Browsable(false)]
		public event EventHandler OptionChanged;
		protected virtual void RaiseOptionsChangedHandler(EventArgs e) {
			if (!IsLocked && OptionChanged != null && needRaiseChangedEvent) {
				needRaiseChangedEvent = false;
				OptionChanged(this, e);
			}
		}
		public virtual void CombineOptions(OptionsSpellingBase controlOptions) {
			BeginUpdate();
			try {
				CheckSelectedTextFirst = controlOptions.CheckSelectedTextFirst == DefaultBoolean.Default ? CheckSelectedTextFirst : controlOptions.CheckSelectedTextFirst;
				CheckFromCursorPos = controlOptions.CheckFromCursorPos == DefaultBoolean.Default ? CheckFromCursorPos : controlOptions.CheckFromCursorPos;
				IgnoreEmails = controlOptions.IgnoreEmails == DefaultBoolean.Default ? IgnoreEmails : controlOptions.IgnoreEmails;
				IgnoreMixedCaseWords = controlOptions.IgnoreMixedCaseWords == DefaultBoolean.Default ? IgnoreMixedCaseWords : controlOptions.IgnoreMixedCaseWords;
				IgnoreRepeatedWords = controlOptions.IgnoreRepeatedWords == DefaultBoolean.Default ? IgnoreRepeatedWords : controlOptions.IgnoreRepeatedWords;
				IgnoreUpperCaseWords = controlOptions.IgnoreUpperCaseWords == DefaultBoolean.Default ? IgnoreUpperCaseWords : controlOptions.IgnoreUpperCaseWords;
				IgnoreUri = controlOptions.IgnoreUri == DefaultBoolean.Default ? IgnoreUri : controlOptions.IgnoreUri;
				IgnoreWordsWithNumbers = controlOptions.IgnoreWordsWithNumbers == DefaultBoolean.Default ? IgnoreWordsWithNumbers : controlOptions.IgnoreWordsWithNumbers;
				IgnoreMarkupTags = controlOptions.IgnoreMarkupTags == DefaultBoolean.Default ? IgnoreMarkupTags : controlOptions.IgnoreMarkupTags;
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void Assign(OptionsSpellingBase source) {
			BeginUpdate();
			try {
				this.CheckSelectedTextFirst = source.CheckSelectedTextFirst;
				this.CheckFromCursorPos = source.CheckFromCursorPos;
				this.IgnoreEmails = source.IgnoreEmails;
				this.IgnoreMixedCaseWords = source.IgnoreMixedCaseWords;
				this.IgnoreRepeatedWords = source.IgnoreRepeatedWords;
				this.IgnoreUpperCaseWords = source.IgnoreUpperCaseWords;
				this.IgnoreUri = source.IgnoreUri;
				this.IgnoreWordsWithNumbers = source.IgnoreWordsWithNumbers;
				this.IgnoreMarkupTags = source.IgnoreMarkupTags;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class SuggestionBase : IComparable, IComparable<SuggestionBase> {
		string suggestion = string.Empty;
		readonly int distance = -1;
		public SuggestionBase(string suggestion, int distance) {
			this.suggestion = suggestion;
			this.distance = distance;
		}
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SuggestionBaseSuggestion")]
#endif
		public string Suggestion { get { return this.suggestion; } set { this.suggestion = value; } }
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SuggestionBaseDistance")]
#endif
		public int Distance { get { return this.distance; } }
		public int CompareTo(object obj) {
			SuggestionBase suggestion = obj as SuggestionBase;
			if (suggestion == null)
				return int.MinValue;
			return CompareTo(suggestion);
		}
		public int CompareTo(SuggestionBase other) {
			return Distance.CompareTo(other.Distance);
		}
#if DEBUG || DEBUGTEST
		public override string ToString() {
			return String.Format("{0}, {1}", suggestion, distance);
		}
#endif
		public override int GetHashCode() {
			return suggestion.GetHashCode();
		}
		public override bool Equals(object obj) {
			SuggestionBase suggestion = obj as SuggestionBase;
			if (suggestion == null)
				return false;
			return Suggestion.Equals(suggestion.Suggestion, StringComparison.Ordinal);
		}
	}
	#region Commands
	public abstract class SpellCheckerCommand {
		string suggestion;
		string word;
		TextOperationsManager operationsManager;
		Position start;
		Position finish;
		CultureInfo culture;
		protected SpellCheckerCommand(TextOperationsManager operationsManager) {
			this.operationsManager = operationsManager;
		}
		protected abstract void DoCommandCore();
		public virtual void DoCommand() {
			InitializeOperationsManagerByCommand();
			DoCommandCore();
		}
		protected virtual void InitializeOperationsManagerByCommand() { }
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerCommandCaption")]
#endif
		public virtual string Caption {
			get { return string.Empty; }
		}
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerCommandEnabled")]
#endif
		public virtual bool Enabled {
			get { return true; }
		}
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerCommandOperation")]
#endif
		public SpellCheckOperation Operation { get { return SpellCheckOperation.None; } }
		protected TextOperationsManager OperationsManager { get { return this.operationsManager; } }
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerCommandFinish")]
#endif
		public Position Finish {
			get { return finish; }
			set { finish = value; }
		}
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerCommandStart")]
#endif
		public Position Start {
			get { return start; }
			set { start = value; }
		}
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerCommandSuggestion")]
#endif
		public string Suggestion { get { return this.suggestion; } set { this.suggestion = value; } }
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerCommandWord")]
#endif
		public string Word { get { return this.word; } set { this.word = value; } }
		public CultureInfo Culture { get { return culture; } set { culture = value; } }
	}
	public class SpellCheckerAddToDictionaryCommand : SpellCheckerCommand {
		public SpellCheckerAddToDictionaryCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.AddToDictionary(Word, Start, Finish);
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.AddToDictionary; } }
	}
	public class SpellCheckerChangeCommand : SpellCheckerCommand {
		public SpellCheckerChangeCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.Replace(Start, Finish, Suggestion);
		}
		protected override void InitializeOperationsManagerByCommand() {
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.Change; } }
	}
	public class SpellCheckerSilentChangeCommand : SpellCheckerCommand {
		public SpellCheckerSilentChangeCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.ChangeSilent(Start, Finish, Suggestion);
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.SilentChange; } }
	}
	public class SpellCheckerChangeAllCommand : SpellCheckerCommand {
		public SpellCheckerChangeAllCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.ChangeAll(Start, Finish, Suggestion);
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.ChangeAll; } }
	}
	public class SpellCheckerDeleteCommand : SpellCheckerCommand {
		public SpellCheckerDeleteCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.Delete(Start, Finish);
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.Delete; } }
	}
	public class SpellCheckerIgnoreCommand : SpellCheckerCommand {
		public SpellCheckerIgnoreCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.IgnoreOnce(Start, Finish, this.Word);
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.Ignore; } }
	}
	public class SpellCheckerSilentIgnoreCommand : SpellCheckerCommand {
		public SpellCheckerSilentIgnoreCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.IgnoreSilent(this.Word);
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.SilentIgnore; } }
	}
	public class SpellCheckerIgnoreAllCommand : SpellCheckerCommand {
		public SpellCheckerIgnoreAllCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.IgnoreAll(Word);
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.IgnoreAll; } }
	}
	public class SpellCheckerOptionsCommand : SpellCheckerCommand {
		public SpellCheckerOptionsCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.ShowOptionsDialog();
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.Options; } }
	}
	public class SpellCheckerRecheckCommand : SpellCheckerCommand {
		public SpellCheckerRecheckCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.Recheck();
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.Recheck; } }
	}
	public class SpellCheckerUndoCommand : SpellCheckerCommand {
		public SpellCheckerUndoCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.Undo();
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.Undo; } }
	}
	public class SpellCheckerCustomCommand : SpellCheckerCommand {
		public SpellCheckerCustomCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.DoCustomCommand();
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.Custom; } }
	}
	public class SpellCheckerCancelCommand : SpellCheckerCommand {
		public SpellCheckerCancelCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() {
			OperationsManager.Cancel();
		}
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.Cancel; } }
	}
	public class SpellCheckerNoSuggestionsCommand : SpellCheckerCommand {
		public SpellCheckerNoSuggestionsCommand(TextOperationsManager operationsManager) : base(operationsManager) { }
		protected override void DoCommandCore() { }
		public new SpellCheckOperation Operation { get { return SpellCheckOperation.Change; } }
		public override bool Enabled { get { return false; } }
	}
	#endregion
	public class CheckSpellingResult : ICheckSpellingResult {
		int index;
		int length;
		string text;
		CheckSpellingResultType result;
		public CheckSpellingResult(string text, int index, int length, CheckSpellingResultType result) {
			this.index = index;
			this.length = length;
			this.text = text;
			this.result = result;
		}
		public string Text { get { return text; } }
		public bool HasError { get { return Result != CheckSpellingResultType.Success; } }
		public int Index { get { return index; } }
		public int Length { get { return length; } }
		public string Value { get { return text.Substring(Index, Length); } }
		public CheckSpellingResultType Result { get { return result; } }
	}
	public class SpellingErrorInfo : ISpellingErrorInfo {
		readonly SpellingError error;
		readonly string word;
		readonly Position wordStartPosition;
		readonly Position wordEndPosition;
		internal SpellingErrorInfo(string word, Position wordStartPosition, Position wordEndPosition, SpellingError error) {
			this.word = word;
			this.wordStartPosition = wordStartPosition;
			this.wordEndPosition = wordEndPosition;
			this.error = error;
		}
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SpellingErrorInfoWord")]
#endif
		public string Word { get { return word; } }
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SpellingErrorInfoWordStartPosition")]
#endif
		public Position WordStartPosition { get { return wordStartPosition; } }
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SpellingErrorInfoWordEndPosition")]
#endif
		public Position WordEndPosition { get { return wordEndPosition; } }
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("SpellingErrorInfoError")]
#endif
		public SpellingError Error { get { return error; } }
	}
	public class NotLoadedDictionaryException : Exception {
		public NotLoadedDictionaryException(string message, Exception innerException)
			: base(message, innerException) {
		}
		public NotLoadedDictionaryException(string message)
			: base(message) {
		}
		public NotLoadedDictionaryException() {
		}
	}
	public class WrongWordRecord {
		readonly string word;
		readonly SpellCheckerBase spellChecker;
		readonly CultureInfo culture;
		readonly Position wordStartPosition;
		readonly Position wordEndPosition;
		internal WrongWordRecord(SpellCheckerBase spellChecker, CultureInfo culture, string word, Position wordStart, Position wordEnd) {
			Guard.ArgumentNotNull(spellChecker, "spellChecker");
			this.spellChecker = spellChecker;
			this.word = word;
			this.culture = culture;
			this.wordStartPosition = wordStart;
			this.wordEndPosition = wordEnd;
		}
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("WrongWordRecordWord")]
#endif
		public string Word { get { return word; } }
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("WrongWordRecordSuggestions"),
#endif
		Obsolete("Use the GetSuggestions method instead.")]
		public List<string> Suggestions { get { return GetSuggestions(); } }
		internal CultureInfo Culture { get { return culture; } }
		internal SpellCheckerBase SpellChecker { get { return spellChecker; } }
		public Position WordStartPosition { get { return wordStartPosition; } }
		public Position WordEndPosition { get { return wordEndPosition; } }
		public List<string> GetSuggestions() {
			List<string> result = new List<string>();
			SuggestionCollection suggestions = this.spellChecker.GetSuggestions(Word, this.culture);
			this.spellChecker.OnPrepareSuggestions(new PrepareSuggestionsEventArgs(Word, suggestions));
			int count = suggestions.Count;
			for (int i = 0; i < count; i++)
				result.Add(suggestions[i].Suggestion);
			return result;
		}
		public override string ToString() {
			return Word;
		}
	}
}
