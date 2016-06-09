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
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using DevExpress.XtraSpellChecker.Parser;
using System.Collections;
using DevExpress.XtraSpellChecker.Algorithms;
using DevExpress.Utils;
#if !SL
using DevExpress.Utils.Serializing;
#if !WPF
using System.Drawing;
using System.Drawing.Design;
#endif
using System.Web;
#endif
using System.Threading;
using System.Collections.Generic;
using System.Security.Permissions;
using DevExpress.XtraSpellChecker.Native;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
namespace DevExpress.XtraSpellChecker {
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public interface ISpellCheckerDictionary {
		bool Loaded { get; }
		CultureInfo Culture { get; set; }
		bool CaseSensitive { get; }
		string DictionaryPath { get; set; }
		int WordCount { get; }
		char[] AlphabetChars { get; }
		event EventHandler DictionaryLoaded;
		void Load();
		void Clear();
		bool FindWord(string word);
		bool Contains(string word);
	}
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public interface IDoubleMetaphoneIndexesProvider {
		DoubleMetaphoneIndexTable DoubleMetaphoneIndexes { get; }
		string GetWordByIndex(int index);
	}
	[Serializable]
	public class DoubleMetaphoneIndexTable : Dictionary<Metaphone, List<int>> {
		public DoubleMetaphoneIndexTable(IEqualityComparer<Metaphone> comparer)
			: base(comparer) {
		}
		protected DoubleMetaphoneIndexTable(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
	}
	#region DictionaryBase
	public abstract class DictionaryBase : ISpellCheckerDictionary {
		bool loaded;
		CultureInfo culture;
		string dictionaryPath;
		protected DictionaryBase(string dictionaryPath, CultureInfo culture)
			: this() {
			this.dictionaryPath = dictionaryPath;
			this.culture = culture;
		}
		protected DictionaryBase() {
			this.culture = CultureInfo.CurrentCulture;
		}
		#region DictionaryLoaded
		EventHandler dictionaryLoaded;
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("DictionaryBaseDictionaryLoaded")]
#endif
		public event EventHandler DictionaryLoaded { add { dictionaryLoaded += value; } remove { dictionaryLoaded -= value; } }
		protected virtual void RaiseDictionaryLoaded() {
			if (dictionaryLoaded != null)
				dictionaryLoaded(this, EventArgs.Empty);
		}
		#endregion
		#region DictionaryLoading
		DictionaryLoadingEventHandler dictionaryLoading;
		internal event DictionaryLoadingEventHandler DictionaryLoading { add { dictionaryLoading += value; } remove { dictionaryLoading -= value; } }
		internal virtual void RaiseDictionaryLoading(DictionaryLoadingEventAgrs args) {
			if (dictionaryLoading != null)
				dictionaryLoading(this, args);
		}
		#endregion
		public abstract int WordCount { get; }
		[Browsable(false)]
		public virtual bool Loaded { get { return loaded; } }
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("DictionaryBaseCulture")]
#endif
		public CultureInfo Culture { get { return culture; } set { culture = value; } }
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("DictionaryBaseDictionaryPath"),
#endif
		Editor("DevExpress.XtraSpellChecker.Design.FileNameEditor," + AssemblyInfo.SRAssemblySpellCheckerDesign, typeof(UITypeEditor))
		]
		public string DictionaryPath { get { return dictionaryPath; } set { dictionaryPath = value; } }
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("DictionaryBaseAlphabetChars")]
#endif
		public char[] AlphabetChars { get { return GetAlphabetChars(); } }
		bool ISpellCheckerDictionary.CaseSensitive { get { return GetCaseSensitive(); } }
		protected abstract bool GetCaseSensitive();
		protected abstract char[] GetAlphabetChars();
		protected abstract bool LoadCore();
		public abstract void Clear();
		public abstract bool FindWord(string word);
		public abstract bool Contains(string word);
		internal virtual Stream GetStreamFromPath(string path) {
			if (!CanOpenFile(path))
				return null;
			return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		}
		public void Load() {
			BeginLoad();
			DictionaryLoadingEventAgrs args = new DictionaryLoadingEventAgrs();
			RaiseDictionaryLoading(args);
			if (args.Handled)
				return;
			if (!LoadCore())
				return;
			EndLoad();
		}
		protected virtual void BeginLoad() {
			this.loaded = false;
			Clear();
		}
		protected virtual void EndLoad() {
			this.loaded = true;
			RaiseDictionaryLoaded();
		}
		protected virtual bool CanOpenFile(string path) {
			if (String.IsNullOrEmpty(path))
				return false;
			return File.Exists(path);
		}
	} 
	#endregion
	#region SpellCheckerDictionaryBase
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	[Serializable]
	public abstract class SpellCheckerDictionaryBase : DictionaryBase, IDoubleMetaphoneIndexesProvider, ICloseWordsCalculatorFactory {
		#region Fields
		string alphabetPath = string.Empty;
		char[] alphabet;
		IComparer<string> wordComparer = StringComparer.Ordinal;
		bool caseSensitive;
		List<string> wordList = new List<string>();
		DoubleMetaphoneIndexTable doubleMetaphoneIndexes = new DoubleMetaphoneIndexTable(new MetaphoneComparer());
		[NonSerialized]
		DoubleMetaphoneAlgorithm doubleMetaphoneAlgorithm;
		[NonSerialized]
		Encoding encoding;
		#endregion
		protected SpellCheckerDictionaryBase()
			: base() {
			SetDefaultEncoding();
		}
		protected SpellCheckerDictionaryBase(string dictionaryPath, CultureInfo culture)
			: base(dictionaryPath, culture) {
			SetDefaultEncoding();
		}
		#region Properties
		protected internal List<string> WordList { get { return wordList; } }
		public override int WordCount { get { return WordList.Count; } }
		public string this[int index] { get { return WordList[index]; } set { WordList[index] = value; } }
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerDictionaryBaseCaseSensitive"),
#endif
		DefaultValue(false)
		]
		public bool CaseSensitive {
			get { return caseSensitive; }
			set {
				if (caseSensitive != value) {
					caseSensitive = value;
					OnCaseSensitiveChanged();
				}
			}
		}
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerDictionaryBaseAlphabetPath"),
#endif
		Editor("DevExpress.XtraSpellChecker.Design.FileNameEditor," + AssemblyInfo.SRAssemblySpellCheckerDesign, typeof(UITypeEditor))
		]
		public virtual string AlphabetPath {
			get { return this.alphabetPath; }
			set { alphabetPath = value; }
		}
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerDictionaryBaseEncoding"),
#endif
		Editor("DevExpress.XtraSpellChecker.Design.EncodingsEditor," + AssemblyInfo.SRAssemblySpellCheckerDesign, typeof(UITypeEditor))
		]
		[TypeConverter("DevExpress.XtraSpellChecker.Design.EncodingTypeConverter," + AssemblyInfo.SRAssemblySpellCheckerDesign)]
		public Encoding Encoding {
			get { return encoding; }
			set {
				if (Encoding == value)
					return;
				encoding = value;
				OnEncodingChanged();
			}
		}
		protected IComparer<string> WordComparer { get { return wordComparer; } }
		#endregion
		protected virtual void SetDefaultEncoding() {
			this.encoding = System.Text.Encoding.Default;
		}
		protected override bool GetCaseSensitive() {
			return CaseSensitive;
		}
		protected override char[] GetAlphabetChars() {
			if (alphabet == null)
				return null;
			char[] result = new char[alphabet.Length];
			Array.Copy(alphabet, result, alphabet.Length);
			return result;
		}
		protected internal virtual void SetAlphabetChars(char[] alphabet) {
			this.alphabet = alphabet;
		}
		protected internal virtual void SortDictionary() {
			SortDictionaryCore();
			DoAfterSort();
		}
		protected void SetEncoding(Encoding encoding) {
			this.encoding = encoding;
		}
		protected virtual void DoAfterSort() {
			if (NeedFillDoubleMetaphoneIndexes())
				FillDoubleMetaphoneIndexes();
		}
		protected virtual void SortDictionaryCore() {
			WordList.Sort(WordComparer);
		}
		protected virtual void OnEncodingChanged() {
			if (Loaded)
				Load();
		}
		protected virtual void OnCaseSensitiveChanged() {
			if (!Loaded)
				return;
			if (CaseSensitive)
				Load();
			else
				ConvertToLower();
		}
		protected virtual void ConvertToLower() {
			for (int i = 0; i < WordCount; i++)
				this[i] = this[i].ToLower(Culture);
			SortDictionary();
		}
		protected virtual bool NeedFillDoubleMetaphoneIndexes() {
			return Culture.EnglishName.Contains("English");
		}
		protected virtual void FillDoubleMetaphoneIndexes() {
			doubleMetaphoneIndexes.Clear();
			for (int i = 0; i < WordCount; i++)
				AddWordToDoubleMetaphoneIndexes(WordList[i], i);
		}
		public override void Clear() {
			WordList.Clear();
			doubleMetaphoneIndexes.Clear();
		}
		protected internal virtual void AddWordCore(string word) {
			if (!CaseSensitive)
				word = word.ToLower(Culture);
			WordList.Add(word);
		}
		protected internal virtual void FillAlphabetCore(char[] chars) {
			if (chars == null)
				return;
			List<char> result = new List<char>();
			if (!CaseSensitive) {
				HashSet<char> charSet = new HashSet<char>();
				foreach (char ch in chars) {
					if (Char.IsLetter(ch))
						charSet.Add(Char.ToLower(ch));
				}
				result.AddRange(charSet);
			}
			else
				for (int i = 0; i < chars.Length; i++) {
					if (char.IsLetter(chars[i]))
						result.Add(chars[i]);
				}
			SetAlphabetChars(result.ToArray());
		}
		protected virtual void FillAlphabet() {
			if (CanOpenFile(AlphabetPath))
				FillAlphabetFromStream(GetStreamFromPath(AlphabetPath));
		}
		public virtual void FillAlphabetFromStream(Stream stream) {
			char[] chars = null;
			using (StreamReader reader = new StreamReader(stream, Encoding))
				chars = reader.ReadToEnd().ToCharArray();
			FillAlphabetCore(chars);
		}
		public override bool FindWord(string word) {
			string[] words = word.Split(new char[] { ' ' });
			bool result = true;
			for (int i = 0; i < words.Length; i++) {
				result = result && HasWordCore(words[i]);
				if (!result)
					break;
			}
			return result;
		}
		protected virtual bool HasWordCore(string word) {
			if (!CaseSensitive)
				return Contains(word.ToLower(Culture));
			if (Contains(word))
				return true;
			if (IsAllCaps(word)) {
				if (Contains(word.ToLower(Culture)))
					return true;
				return Contains(MakeInitialCharCaps(word));
			}
			if (IsInitCaps(word))
				return Contains(word.ToLower(Culture));
			return false;
		}
		[Obsolete("This method has become obsolete. To determine whether a word is in the dictionary use the 'FindWord' method.", true)]
		public virtual bool HasWord(string word) {
			return false;
		}
		public override bool Contains(string word) {
			return WordList.BinarySearch(word, WordComparer) >= 0;
		}
		bool IsAllCaps(string word) {
			return StringUtils.IsAllCaps(word, Culture);
		}
		bool IsInitCaps(string word) {
			return StringUtils.IsInitCaps(word, Culture);
		}
		string MakeInitialCharCaps(string word) {
			return StringUtils.MakeInitialCharCaps(word, Culture);
		}
		protected DoubleMetaphoneAlgorithm DoubleMetaphoneAlgorithm {
			get {
				if (doubleMetaphoneAlgorithm == null)
					doubleMetaphoneAlgorithm = new DoubleMetaphoneAlgorithm();
				return doubleMetaphoneAlgorithm;
			}
		}
		protected Metaphone GetDoubleMetaphoneKey(string word) {
			return DoubleMetaphoneAlgorithm.DoubleMetaphone(word, Culture).First;
		}
		protected void AddWordToDoubleMetaphoneIndexes(string word, int index) {
			Metaphone key = GetDoubleMetaphoneKey(word);
			List<int> list;
			if (!doubleMetaphoneIndexes.TryGetValue(key, out list)) {
				list = new List<int>();
				doubleMetaphoneIndexes.Add(key, list);
			}
			list.Add(index);
		}
		CloseWordsCalculatorBase ICloseWordsCalculatorFactory.Create() {
			return new CloseWordsCalculator(this);
		}
		#region ASP
		private string cacheKey;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string CacheKey {
			get { return cacheKey; }
			set { cacheKey = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanCacheDictionary { get { return true; } }
		#endregion
		#region IDoubleMetaphoneIndexesProvider Members
		DoubleMetaphoneIndexTable IDoubleMetaphoneIndexesProvider.DoubleMetaphoneIndexes { get { return doubleMetaphoneIndexes; } }
		string IDoubleMetaphoneIndexesProvider.GetWordByIndex(int index) {
			return (index < WordCount) ? WordList[index] : null;
		}
		#endregion
	} 
	#endregion
	#region SpellCheckerDictionary
	public class SpellCheckerDictionary : SpellCheckerDictionaryBase {
		public SpellCheckerDictionary() : base() { }
		public SpellCheckerDictionary(string dictionaryPath, CultureInfo culture) : base(dictionaryPath, culture) { }
		protected override bool LoadCore() {
			if (!CanOpenFile(DictionaryPath))
				return false;
			using (Stream stream = GetStreamFromPath(DictionaryPath)) {
				LoadFromStream(stream);
			}
			FillAlphabet();
			return true;
		}
		public void Load(Stream dictionaryStream, Stream alphabethStream) {
			BeginLoad();
			LoadFromStream(dictionaryStream);
			FillAlphabetFromStream(alphabethStream);
			EndLoad();
		}
		public void LoadFromStream(Stream stream) {
			using (StreamReader strReader = new StreamReader(stream, Encoding)) {
				if (WordCount > 0)
					Clear();
				string line = string.Empty;
				while ((line = strReader.ReadLine()) != null)
					AddWordCore(line);
			}
			SortDictionary();
		}
		[Obsolete("This method has become obsolete. To load an alphabet from a stream use the 'FillAlphabetFromStream' method. To obtain the loaded alphabet use the 'AlphabetChars' property.", true)]
		public char[] LoadAlphabetFromStream(Stream stream) {
			using (StreamReader reader = new StreamReader(stream, Encoding)) {
				return reader.ReadToEnd().ToCharArray();
			}
		}
	} 
	#endregion
	#region SpellCheckerCustomDictionary
	public class SpellCheckerCustomDictionary : SpellCheckerDictionary {
		int lockUpdate = 0;
		public SpellCheckerCustomDictionary()
			: base() {
		}
		public SpellCheckerCustomDictionary(string dictionaryPath, CultureInfo culture)
			: base(dictionaryPath, culture) {
		}
		public void BeginUpdate() {
			lockUpdate++;
		}
		public void EndUpdate() {
			if (--lockUpdate == 0)
				SortDictionary();
		}
		public bool IsLocked() { return lockUpdate != 0; }
		public virtual void AddWord(string word) {
			AddWordCore(word);
			if (!IsLocked()) {
				SortDictionary();
				Save();
			}
		}
		public virtual void AddWords(ICollection words) {
			BeginUpdate();
			try {
				foreach (string word in words) {
					AddWord(word);
				}
			}
			finally {
				EndUpdate();
				Save();
			}
		}
		public virtual void Save() {
			if (!Loaded) return;
			if (CanSaveToFile())
				SaveToFile();
		}
		bool CanSaveToFile() {
			return !String.IsNullOrEmpty(DictionaryPath);
		}
		protected virtual void SaveToFile() {
			using (StreamWriter dictionaryStream = new StreamWriter(DictionaryPath, false, Encoding)) {
				int count = WordCount;
				for (int i = 0; i < count; i++)
					dictionaryStream.WriteLine(WordList[i]);
			}
		}
		public virtual void SaveAs(string fileName) {
			DictionaryPath = fileName;
			Save();
		}
		protected override void BeginLoad() {
			if (CanOpenFile(DictionaryPath))
				base.BeginLoad();
		}
		protected override void OnCaseSensitiveChanged() {
			if (!CaseSensitive)
				ConvertToLower();
			else {
				Clear();
				Load();
			}
		}
		protected override bool LoadCore() {
			if (CanOpenFile(DictionaryPath)) {
				using (Stream stream = GetStreamFromPath(DictionaryPath)) {
					LoadFromStream(stream);
				}
			}
			FillAlphabet();
			return true;
		}
	} 
	#endregion
	#region SpellCheckerISpellDictionary
	public class SpellCheckerISpellDictionary : SpellCheckerDictionaryBase {
		string grammarPath = string.Empty;
		AffixISpellDecompressor decompressor;
		public SpellCheckerISpellDictionary()
			: base() {
			this.decompressor = CreateAffixDecompressor();
		}
		public SpellCheckerISpellDictionary(string dictionaryPath, string grammarPath, CultureInfo culture)
			: base(dictionaryPath, culture) {
			this.grammarPath = grammarPath;
			this.decompressor = CreateAffixDecompressor();
		}
		[
#if !SL
	DevExpressSpellCheckerCoreLocalizedDescription("SpellCheckerISpellDictionaryGrammarPath"),
#endif
		Editor("DevExpress.XtraSpellChecker.Design.FileNameEditor," + AssemblyInfo.SRAssemblySpellCheckerDesign, typeof(UITypeEditor))
		]
		public virtual string GrammarPath { get { return this.grammarPath; } set { grammarPath = value; } }
		protected AffixISpellDecompressor Decompressor { get { return decompressor; } }
		protected virtual AffixISpellDecompressor CreateAffixDecompressor() {
			return new AffixISpellDecompressor();
		}
		protected override bool LoadCore() {
			if (!CanOpenFile(DictionaryPath) || !CanOpenFile(GrammarPath))
				return false;
			bool result = Decompressor.DecompressDictionary(this);
			if (!result)
				return false;
			SortDictionary();
			FillAlphabet();
			return true;
		}
		public virtual void LoadFromStream(Stream dictionaryStream, Stream grammarStream, Stream alphabethStream) {
			BeginLoad();
			bool result = Decompressor.DecompressDictionary(this, dictionaryStream, grammarStream);
			if (!result)
				return;
			SortDictionary();
			if (alphabethStream != null)
				FillAlphabetFromStream(alphabethStream);
			EndLoad();
		}
	} 
	#endregion
	#region SpellCheckerOpenOfficeDictionary
	public class SpellCheckerOpenOfficeDictionary : SpellCheckerISpellDictionary {
		public SpellCheckerOpenOfficeDictionary() : base() { }
		public SpellCheckerOpenOfficeDictionary(string dictionaryPath, string grammarPath, CultureInfo culture)
			: base(dictionaryPath, grammarPath, culture) {
			SetEncoding();
		}
		protected override AffixISpellDecompressor CreateAffixDecompressor() {
			return new AffixOpenOfficeDecompressor();
		}
		protected virtual void SetEncoding() {
			using (Stream stream = GetStreamFromPath(GrammarPath)) {
				SetEncodingFromStream(stream);
			}
		}
		protected virtual void SetEncodingFromStream(Stream grammarStream) {
			Encoding fileEncoding = AffixOpenOfficeFileReader.GetEncodingFromStream(grammarStream);
			if (fileEncoding != null)
				Encoding = fileEncoding;
		}
		protected override bool LoadCore() {
			SetEncoding();
			return base.LoadCore();
		}
		public override void LoadFromStream(Stream dictionaryStream, Stream grammarStream, Stream alphabethStream) {
			SetEncodingFromStream(grammarStream);
			base.LoadFromStream(dictionaryStream, grammarStream, alphabethStream);
		}
	} 
	#endregion
	public class DictionaryHelper {
		#region Static
		static DictionaryTable CreateDefaultDictionaries() {
			DictionaryTable result = new DictionaryTable();
			result.Add(new CultureInfo("en-US"), CreateEnglishDictionary());
			return result;
		}
		static ISpellCheckerDictionary CreateEnglishDictionary() {
			SpellCheckerISpellDictionary dictionary = new SpellCheckerISpellDictionary();
			dictionary.Culture = new CultureInfo("en-US");
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			Stream dictionaryStream = assembly.GetManifestResourceStream("DevExpress.XtraSpellChecker.Core.Dictionary.american.xlg");
			Stream grammarStream = assembly.GetManifestResourceStream("DevExpress.XtraSpellChecker.Core.Dictionary.english.aff");
			string alphabeth = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			Stream alphabethStream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(alphabeth));
			try {
				dictionary.LoadFromStream(dictionaryStream, grammarStream, alphabethStream);
			}
			finally {
				dictionaryStream.Dispose();
				grammarStream.Dispose();
				alphabethStream.Dispose();
			}
			return dictionary;
		}
		static DictionaryTable DefaultDictionaries = CreateDefaultDictionaries();
		#endregion
		#region DictionaryIterator
		protected class DictionaryIterator : IEnumerator<ISpellCheckerDictionary> {
			readonly DictionaryHelper dictionaryHelper;
			readonly CultureInfo culture;
			int spellCheckerDictionaryIndex;
			int sharedDictionaryIndex;
			ISpellCheckerDictionary current;
			public DictionaryIterator(DictionaryHelper dictionaryHelper, CultureInfo culture) {
				this.dictionaryHelper = dictionaryHelper;
				this.culture = culture;
				Reset();
			}
			public DictionaryIterator(DictionaryHelper dictionaryHelper)
				: this(dictionaryHelper, dictionaryHelper.Culture) {
			}
			DictionaryHelper DictionaryHelper { get { return dictionaryHelper; } }
			DictionaryCollection Dictionaries { get { return DictionaryHelper.Dictionaries; } }
			DictionaryCollection SharedDictionaries { get { return DictionaryHelper.SharedDictionaries; } }
			bool UseSharedDictionaries { get { return DictionaryHelper.UseSharedDictionaries; } }
			CultureInfo Culture { get { return culture; } }
			public ISpellCheckerDictionary Current { get { return current; } }
			object IEnumerator.Current { get { return Current; } }
			public bool MoveNext() {
				while (++this.spellCheckerDictionaryIndex < Dictionaries.Count) {
					ISpellCheckerDictionary dictionary = Dictionaries[this.spellCheckerDictionaryIndex];
					if (!IsDictionaryFit(dictionary))
						continue;
					this.current = dictionary;
					return true;
				}
				if (UseSharedDictionaries) {
					while (++this.sharedDictionaryIndex < SharedDictionaries.Count) {
						ISpellCheckerDictionary dictionary = SharedDictionaries[this.sharedDictionaryIndex];
						if (!IsDictionaryFit(dictionary))
							continue;
						this.current = dictionary;
						return true;
					}
				}
				if (this.current == null) {
					ISpellCheckerDictionary dictionary;
					if (DefaultDictionaries.TryGetValue(Culture, out dictionary)) {
						this.current = dictionary;
						return true;
					}
				}
				else
					this.current = null;
				return false;
			}
			bool IsDictionaryFit(ISpellCheckerDictionary dictionary) {
				return dictionary.Culture.Equals(Culture) || dictionary.Culture.Equals(CultureInfo.InvariantCulture) || Culture.Equals(CultureInfo.InvariantCulture);
			}
			public void Reset() {
				this.spellCheckerDictionaryIndex = -1;
				this.sharedDictionaryIndex = -1;
				this.current = null;
			}
			public void Dispose() {
			}
		}
		#endregion
		#region Fields
		DoubleMetaphoneAlgorithm doubleMetaphoneAlgorithm;
		readonly DictionaryCollection sharedDictionaries;
		readonly SpellCheckerBase spellChecker;
		#endregion
		[Obsolete("This constructor has become obsolete. Use the constructor that takes the spellChecker and sharedDictionaries parameters.", true)]
		public DictionaryHelper(DictionaryCollection spellCheckerDictionaries, DictionaryCollection sharedDictionaries, CultureInfo culture, bool useShared) {
		}
		public DictionaryHelper(SpellCheckerBase spellChecker, DictionaryCollection sharedDictionaries) {
			this.spellChecker = spellChecker;
			this.sharedDictionaries = sharedDictionaries;
			this.doubleMetaphoneAlgorithm = new DoubleMetaphoneAlgorithm();
		}
#if !SL
	[DevExpressSpellCheckerCoreLocalizedDescription("DictionaryHelperCulture")]
#endif
		public CultureInfo Culture { get { return spellChecker.Culture; } }
		protected DoubleMetaphoneAlgorithm DoubleMetaphoneAlgorithm { get { return doubleMetaphoneAlgorithm; } }
		internal DictionaryCollection Dictionaries { get { return spellChecker.Dictionaries; } }
		DictionaryCollection SharedDictionaries { get { return sharedDictionaries; } }
		bool UseSharedDictionaries { get { return spellChecker.UseSharedDictionaries; } }
		public virtual bool FindWord(string word) {
			DictionaryIterator iterator = new DictionaryIterator(this);
			while (iterator.MoveNext())
				if (iterator.Current.FindWord(word))
					return true;
			return false;
		}
		public virtual bool FindWord(string word, CultureInfo culture) {
			DictionaryIterator iterator = new DictionaryIterator(this, culture);
			while (iterator.MoveNext())
				if (iterator.Current.FindWord(word))
					return true;
			return false;
		}
		protected internal virtual SuggestionCollection GetDoubleMetaphoneSuggs(string word, int maxDistance, CultureInfo culture) {
			SuggestionCollection suggestions = new SuggestionCollection();
			if (!CanUseDoubleMetaphone(culture))
				return suggestions;
			Metaphone key = DoubleMetaphoneAlgorithm.DoubleMetaphone(word, culture).First;
			DictionaryIterator iterator = new DictionaryIterator(this, culture);
			while (iterator.MoveNext()) {
				ISpellCheckerDictionary dictionary = iterator.Current;
				IDoubleMetaphoneIndexesProvider provider = dictionary as IDoubleMetaphoneIndexesProvider;
				if (provider == null)
					continue;
				suggestions.Merge(CalculateDoubleMetaphoneSuggestions(word, maxDistance, key, provider, culture));
			}
			return suggestions;
		}
		SuggestionCollection CalculateDoubleMetaphoneSuggestions(string word, int maxDistance, Metaphone key, IDoubleMetaphoneIndexesProvider provider, CultureInfo culture) {
			SuggestionCollection suggestions = new SuggestionCollection();
			List<int> dictList = null;
			if (!provider.DoubleMetaphoneIndexes.TryGetValue(key, out dictList))
				return suggestions;
			for (int i = dictList.Count - 1; i >= 0; i--) {
				int index = dictList[i];
				string suggestion = provider.GetWordByIndex(index);
				if (String.IsNullOrEmpty(suggestion))
					continue;
				int distance = LevenshteinAlgorithm.CalcDistance(word, suggestion, culture);
				if (distance <= maxDistance)
					suggestions.Add(new SuggestionBase(suggestion, distance));
			}
			return suggestions;
		}
		protected internal virtual SuggestionCollection GetNearMissSuggestions(string word, int distance, int timeOut, CultureInfo culture) {
			SuggestionCollection result = new SuggestionCollection();
			DictionaryIterator iterator = new DictionaryIterator(this, culture);
			SuggestionsCalculator calculator = new SuggestionsCalculator();
			while (iterator.MoveNext()) {
				ISpellCheckerDictionary dictionary = iterator.Current;
				SuggestionCollection suggestions = calculator.Calculate(dictionary, word, distance, timeOut);
				if (suggestions.Count > 0)
					result.AddRange(suggestions);
			}
			return result;
		}
		protected virtual bool CanUseDoubleMetaphone(CultureInfo culture) {
			return culture.EnglishName.Contains("English");
		}
		public virtual SpellCheckerCustomDictionary GetCustomDictionary() { 
			return GetCustomDictionary(Culture);
		}
		protected internal SpellCheckerCustomDictionary GetCustomDictionary(CultureInfo culture) {
			DictionaryIterator iterator = new DictionaryIterator(this, culture);
			while (iterator.MoveNext()) {
				SpellCheckerCustomDictionary dictionary = iterator.Current as SpellCheckerCustomDictionary;
				if (dictionary != null)
					return dictionary;
			}
			return null;
		}
		public virtual void AddWord(string word) {
			AddWord(word, Culture);
		}
		protected internal void AddWord(string word, CultureInfo culture) {
			SpellCheckerCustomDictionary dictionary = GetCustomDictionary(culture);
			if (dictionary != null)
				dictionary.AddWord(word);
		}
	}
	public class DictionaryTable : Dictionary<CultureInfo, ISpellCheckerDictionary> {
	}
}
namespace DevExpress.XtraSpellChecker.Native {
	internal interface ICloseWordsCalculatorFactory {
		CloseWordsCalculatorBase Create();
	}
	#region SpellCheckerCloseWordsCalculatorFactory
	internal static class CloseWordsCalculatorFactory {
		public static CloseWordsCalculatorBase CreateCalculator(ISpellCheckerDictionary dictionary) {
			ICloseWordsCalculatorFactory factory = dictionary as ICloseWordsCalculatorFactory;
			return (factory != null) ? factory.Create() : null;
		}
	}
	#endregion
	#region SuggestionsCalculator
	internal class SuggestionsCalculator {
		public SuggestionCollection Calculate(ISpellCheckerDictionary dictionary, string word, int distance, int timeOut) {
			SuggestionCollection result = CalculateSuggestionsCore(word, distance, timeOut, dictionary);
			return result;
		}
		SuggestionCollection CalculateSuggestionsCore(string word, int distance, int timeOut, ISpellCheckerDictionary dictionary) {
			SuggestionCollection suggestions = CalculateNearMissSuggestions(dictionary, word, distance, timeOut);
			if (suggestions.Count > 0)
				return suggestions;
			return CalculateCloseSuggestions(dictionary, word, distance);
		}
		SuggestionCollection CalculateNearMissSuggestions(ISpellCheckerDictionary dictionary, string word, int distance, int timeOut) {
			NearMissStrategy calculator = new NearMissStrategy(dictionary);
			SuggestionCollection result = new SuggestionCollection();
			if (dictionary.CaseSensitive) {
				WordCaseType caseType = StringUtils.GetCaseType(word);
				if (caseType == WordCaseType.InitUpper) {
					SuggestionCollection suggestions = calculator.GetSuggestions(word, distance, timeOut);
					result.Merge(suggestions);
					string smallCharsWord = StringUtils.MakeAllCharsSmall(word, dictionary.Culture);
					suggestions = calculator.GetSuggestions(smallCharsWord, distance, timeOut);
					result.Merge(suggestions);
				}
				else if (caseType == WordCaseType.Upper) {
					string smallCharsWord = StringUtils.MakeAllCharsSmall(word, dictionary.Culture);
					SuggestionCollection suggestions = calculator.GetSuggestions(smallCharsWord, distance, timeOut);
					result.Merge(suggestions);
					string initCapWord = StringUtils.MakeInitialCharCaps(word, dictionary.Culture);
					suggestions = calculator.GetSuggestions(initCapWord, distance, timeOut);
					result.Merge(suggestions);
				}
				else if (caseType == WordCaseType.InitUpperMixed) {
					string initCharSmallWord = StringUtils.MakeInitCharSmall(word, dictionary.Culture);
					SuggestionCollection suggestions = calculator.GetSuggestions(initCharSmallWord, distance, timeOut);
					result.Merge(suggestions);
					string smallCharsWord = StringUtils.MakeAllCharsSmall(word, dictionary.Culture);
					suggestions = calculator.GetSuggestions(smallCharsWord, distance, timeOut);
					result.Merge(suggestions);
				}
				else if (caseType == WordCaseType.Mixed) {
					string smallCharsWord = StringUtils.MakeAllCharsSmall(word, dictionary.Culture);
					SuggestionCollection suggestions = calculator.GetSuggestions(smallCharsWord, distance, timeOut);
					result.Merge(suggestions);
				}
				else {
					SuggestionCollection suggestions = calculator.GetSuggestions(word, distance, timeOut);
					result.Merge(suggestions);
				}
			}
			else {
				SuggestionCollection suggestions = calculator.GetSuggestions(word, distance, timeOut);
				result.Merge(suggestions);
			}
			return result;
		}
		SuggestionCollection CalculateCloseSuggestions(ISpellCheckerDictionary dictionary, string word, int distance) {
			CloseWordsCalculatorBase calculator = CloseWordsCalculatorFactory.CreateCalculator(dictionary);
			SuggestionCollection result = new SuggestionCollection();
			if (!dictionary.CaseSensitive || StringUtils.IsInitCaps(word, dictionary.Culture)) {
				string smallCharsWord = StringUtils.MakeAllCharsSmall(word, dictionary.Culture);
				SuggestionCollection suggestions = calculator.GetSuggestions(smallCharsWord, distance);
				result.Merge(suggestions);
			}
			else if (StringUtils.IsAllCaps(word, dictionary.Culture)) {
				string smallCharsWord = StringUtils.MakeAllCharsSmall(word, dictionary.Culture);
				SuggestionCollection suggestions = calculator.GetSuggestions(smallCharsWord, distance);
				result.Merge(suggestions);
				ToUpperCase(result, dictionary.Culture);
			}
			else {
				SuggestionCollection suggestions = calculator.GetSuggestions(word, distance);
				result.Merge(suggestions);
			}
			return result;
		}
		static void ToUpperCase(SuggestionCollection result, CultureInfo culture) {
			foreach (SuggestionBase suggestion in result)
				suggestion.Suggestion = StringUtils.MakeAllCharsCaps(suggestion.Suggestion, culture);
		}
	}
	#endregion
	#region CloseWordsCalculatorBase
	internal abstract class CloseWordsCalculatorBase {
		#region DescendingComparer
		class DescendingComparer : IComparer<float> {
			public int Compare(float x, float y) {
				return y.CompareTo(x);
			}
		}
		#endregion
		readonly ISpellCheckerDictionary dictionary;
		readonly DescendingComparer comparer;
		protected CloseWordsCalculatorBase(ISpellCheckerDictionary dictionary) {
			this.dictionary = dictionary;
			this.comparer = new DescendingComparer();
		}
		protected ISpellCheckerDictionary Dictionary { get { return dictionary; } }
		protected virtual IComparer<float> Comaprer { get { return comparer; } }
		protected string[] CalculateNearestWords(IList<string> words, string word) {
			return GetNearestItems(word, words, CalculateWordsComparisonRate, 10, 5);
		}
		protected SuggestionCollection CreateSuggestions(string word, string[] candidates, int maxDistance) {
			SuggestionCollection result = new SuggestionCollection();
			int count = candidates.Length;
			for (int i = 0; i < count; i++) {
				string candidate = candidates[i];
				int distance = LevenshteinAlgorithm.CalcDistance(word, candidate, Dictionary.Culture);
				if (distance <= maxDistance)
					result.Add(new SuggestionBase(candidate, distance));
			}
			return result;
		}
		protected internal T[] GetNearestItems<T>(string word, ICollection<T> items, Func<int, int, int, float> rateFunc, int maxCount, int maxDeltaLenght) {
			float[] keys = new float[maxCount];
			T[] values = new T[maxCount];
			int length = 0;
			foreach (T item in items) {
				if (item == null)
					continue;
				string candidate = item.ToString();
				if (String.IsNullOrEmpty(candidate))
					continue;
				int deltaLength = candidate.Length - word.Length;
				if (Math.Abs(deltaLength) >= maxDeltaLenght)
					continue;
				float rate = CalculateNGramRate(word, candidate, rateFunc);
				int index = Array.BinarySearch(keys, 0, length, rate, Comaprer);
				if (index < 0)
					index = ~index;
				int lengthToCopy = length - index;
				if (length == maxCount) {
					if (length == index)
						continue;
					lengthToCopy--;
				}
				else
					length++;
				if (lengthToCopy > 0) {
					Array.Copy(keys, index, keys, index + 1, lengthToCopy);
					Array.Copy(values, index, values, index + 1, lengthToCopy);
				}
				keys[index] = rate;
				values[index] = item;
			}
			T[] result = new T[length];
			Array.Copy(values, result, length);
			return result;
		}
		protected internal float CalculateNGramRate(string word, string candidate, Func<int, int, int, float> rateFunc) {
			int n = Math.Min(2, Math.Min(word.Length, candidate.Length));
			int[] wordNGrams = CreateNGramHash(word, n);
			int[] candidateNGrams = CreateNGramHash(candidate, n);
			int score = 0;
			int lastIndex = wordNGrams.Length - 1;
			for (int i = 0; i <= lastIndex; i++) {
				int nGramHash = wordNGrams[i];
				int index = Array.IndexOf(candidateNGrams, nGramHash);
				if (index < 0)
					continue;
				candidateNGrams[index] = 0;
				score++;
			}
			int l1 = wordNGrams.Length;
			int l2 = candidateNGrams.Length;
			return rateFunc(score, l1, l2);
		}
		protected internal float CalculateWordsComparisonRate(int score, int length1, int length2) {
			float fScore = score;
			float l1 = length1;
			float l2 = length2;
			return 2 * fScore / (l1 + l2);
		}
		protected internal int[] CreateNGramHash(string word, int n) {
			int length = n > 0 ? word.Length - (n - 1) : 0;
			int[] nGrams = new int[length];
			for (int i = 0; i < length; i++) {
				int hash = 0;
				for (int j = 0; j < n; j++)
					hash |= word[i + j] << ((n - 1 - j) * 16);
				nGrams[i] = hash;
			}
			return nGrams;
		}
		public abstract SuggestionCollection GetSuggestions(string word, int maxDistance);
	}
	#endregion
	#region CloseWordsCalculator
	internal class CloseWordsCalculator : CloseWordsCalculatorBase {
		public CloseWordsCalculator(SpellCheckerDictionaryBase dictionary)
			: base(dictionary) {
		}
		protected new SpellCheckerDictionaryBase Dictionary { get { return (SpellCheckerDictionaryBase)base.Dictionary; } }
		public override SuggestionCollection GetSuggestions(string word, int maxDistance) {
			string[] nearestWords = CalculateNearestWords(Dictionary.WordList, word);
			return CreateSuggestions(word, nearestWords, maxDistance);
		}
	}
	#endregion
	#region HunspellCloseWordsCalculator
	internal class HunspellCloseWordsCalculator : CloseWordsCalculatorBase {
		public HunspellCloseWordsCalculator(HunspellDictionary dictionary)
			: base(dictionary) {
		}
		protected new HunspellDictionary Dictionary { get { return (HunspellDictionary)base.Dictionary; } }
		public override SuggestionCollection GetSuggestions(string word, int maxDistance) {
			WordEntry[] nearestEntries = GetNearestItems(word, Dictionary.WordEntries.Values, CalculateIntermediateRate, 100, Int32.MaxValue);
			List<string> expandedRoots = ExpandRoots(nearestEntries);
			string[] nearestWords = CalculateNearestWords(expandedRoots, word);
			return CreateSuggestions(word, nearestWords, maxDistance);
		}
		float CalculateIntermediateRate(int score, int length1, int length2) {
			float fScore = score;
			float l1 = length1;
			float l2 = length2;
			return fScore / Math.Max(l1, l2);
		}
		List<string> ExpandRoots(WordEntry[] wordEntries) {
			List<string> reuslt = new List<string>();
			int count = wordEntries.Length;
			for (int i = 0; i < count; i++) {
				string[] words = Dictionary.Manager.ExpandWordRoot(wordEntries[i]);
				if (words.Length > 0)
					reuslt.AddRange(words);
			}
			return reuslt;
		}
	}
	#endregion
}
