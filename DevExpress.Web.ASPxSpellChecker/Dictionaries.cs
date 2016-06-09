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
using DevExpress.XtraSpellChecker;
using System.Globalization;
using System.ComponentModel;
using System.Text;
using System.Web.Caching;
using System.Web;
using System.Collections;
using System.Web.SessionState;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.Web.ASPxSpellChecker.Native;
using System.Drawing.Design;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.IO;
namespace DevExpress.Web.ASPxSpellChecker {
	public abstract class WebDictionary : CollectionItem {
		private ISpellCheckerDictionary dictionary;
		string generatedCacheKey = string.Empty;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal ISpellCheckerDictionary DictionaryInternal {
			get {
				if (dictionary == null)
					dictionary = CreateDictionary();
				return dictionary;
			}
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("WebDictionaryDictionaryPath"),
#endif
DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string DictionaryPath {
			get { return GetStringProperty("DictionaryPath", ""); }
			set {
				SetStringProperty("DictionaryPath", "", value);
				if (!IsDesignMode()) {
					DictionaryInternal.DictionaryPath = GetPhysicalPath(value);
				}
			}
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("WebDictionaryCacheKey"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public virtual string CacheKey {
			get { return GetStringProperty("CacheKey", ""); }
			set {
				SetStringProperty("CacheKey", "", value);
			}
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("WebDictionaryCulture"),
#endif
		DefaultValue(null), NotifyParentProperty(true)]
		public virtual CultureInfo Culture {
			get { return (CultureInfo)GetObjectProperty("Culture", null); }
			set {
				if (CultureInfo.Equals(value, CultureInfo.InvariantCulture))
					value = null;
				SetObjectProperty("Culture", null, value);
				if (!IsDesignMode()) {
					DictionaryInternal.Culture = GetCulture();
				}
			}
		}
		public CultureInfo GetCulture() {
			if (Culture == null)
				return CultureInfo.InvariantCulture;
			return Culture;
		}
		[Browsable(false)]
		public virtual bool CanCacheDictionary { get { return true; } }
		[Browsable(false)]
		public virtual bool IsCustomDictionary { get { return false; } }
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			WebDictionary src = source as WebDictionary;
			if (src != null)
				AssignCore(src);
		}
		protected virtual void AssignCore(WebDictionary src) {
			DictionaryPath = src.DictionaryPath;
			CacheKey = src.CacheKey;
			Culture = src.Culture;
		}
		[Browsable(false), 
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("WebDictionaryWordCount")
#else
	Description("")
#endif
]
		public int WordCount {
			get { return this.DictionaryInternal.WordCount; }
		}
		protected override bool IsDesignMode() {
			return base.IsDesignMode() || HttpContext.Current == null;
		}
		protected virtual ISpellCheckerDictionary CreateDictionary() {
			ISpellCheckerDictionary dictionary = CreateDictionaryCore();
			AssignDictionaryProperties(dictionary);
			return dictionary;
		}
		protected abstract ISpellCheckerDictionary CreateDictionaryCore();
		protected virtual void AssignDictionaryProperties(ISpellCheckerDictionary dictionary) {
			dictionary.Culture = GetCulture();
		}
		protected string GetPhysicalPath(string path) {
			return UrlUtils.GetPhysicalPath(path);
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			DictionaryInternal.DictionaryPath = GetPhysicalPath(DictionaryPath);
			DictionaryInternal.Culture = Culture;
		}
		protected virtual SpellCheckerDictionaryProvider CreateDictionaryProvider() {
			return new SpellCheckerDictionaryProvider(this);
		}
		public ISpellCheckerDictionary GetCachedDictionary() {
			SpellCheckerDictionaryProvider provider = CreateDictionaryProvider();
			return provider.GetDictionary();
		}
		public virtual string GetCacheKey() {
			if (!string.IsNullOrEmpty(CacheKey))
				return CacheKey;
			return GeneratedCacheKey;
		}
		protected virtual string GeneratedCacheKey {
			get {
				if (String.IsNullOrEmpty(generatedCacheKey))
					generatedCacheKey = GenerateCacheKey();
				return generatedCacheKey;
			}
		}
		protected virtual string GenerateCacheKey() {
			string result = "";
			WebControl control = this.Collection.Owner as WebControl;
			if (control.Page != null)
				result = control.Page.GetType().Name + "_" + control.ClientID;
			else
				result = control.ClientID;
			result += "_" + this.Index.ToString();
			return result;
		}
		public void RemoveFromCache() {
			SpellCheckerDictionaryProvider provider = CreateDictionaryProvider();
			provider.RemoveFromCache();
		}
		protected internal void AssignDictionary(ISpellCheckerDictionary dictionary) {
			this.dictionary = dictionary;
		}
	}
	public abstract class ASPxSpellCheckerDictionaryBase : WebDictionary {
		private const string DefaultEncoding = "(Default)";
		Encoding encoding;
		protected internal static Dictionary<string, string> encodingNames = null;
		static ASPxSpellCheckerDictionaryBase() {
			encodingNames = new Dictionary<string, string>();
			encodingNames.Add(DefaultEncoding, Encoding.Default.EncodingName);
			EncodingInfo[] infos = Encoding.GetEncodings();
			for(int i = 0;i < infos.Length;i++)
				if(!encodingNames.ContainsKey(infos[i].DisplayName))
					encodingNames.Add(infos[i].DisplayName, infos[i].Name);
		}
		protected internal new SpellCheckerDictionaryBase DictionaryInternal { get { return (SpellCheckerDictionaryBase)base.DictionaryInternal; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Encoding Encoding {
			get {
				if (this.encoding == null)
					if (!string.IsNullOrEmpty(EncodingName))
						if (EncodingName != DefaultEncoding)
							this.encoding = Encoding.GetEncoding(encodingNames[EncodingName]);
						else
							this.encoding = Encoding.Default;
				return this.encoding; 
			}
			set {
				this.encoding = value;
				this.EncodingName = this.encoding != null ? this.encoding.EncodingName : string.Empty;
			}
		}
		public virtual Encoding GetEncoding() {
			Encoding result = Encoding;
			if(result == null)
				result = System.Text.Encoding.Default;
			return result;
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerDictionaryBaseEncodingName"),
#endif
 DefaultValue(DefaultEncoding),
		TypeConverter("DevExpress.Web.ASPxSpellChecker.Design.EncodingNameTypeConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), 
		NotifyParentProperty(true), Localizable(false)
		]
		public virtual string EncodingName {
			get {
				return GetStringProperty("EncodingName", DefaultEncoding);
			}
			set {
				this.encoding = null;
				SetStringProperty("EncodingName", DefaultEncoding, value);
				if (!IsDesignMode())
					DictionaryInternal.Encoding = GetEncoding();
			}
		}
		protected string AlphabetPathInternal {
			get { return GetStringProperty("AlphabetPath", ""); }
			set {
				SetStringProperty("AlphabetPath", "", value);
				if(!IsDesignMode()) {
					DictionaryInternal.AlphabetPath = GetPhysicalPath(value);
				}
			}
		}
		protected override void AssignCore(WebDictionary src) {
			base.AssignCore(src);
			ASPxSpellCheckerDictionaryBase dic = src as ASPxSpellCheckerDictionaryBase;
			if (dic != null) {
				Encoding = dic.Encoding;
				AlphabetPathInternal = dic.AlphabetPathInternal;
			}
		}
		public string this[int index] {
			get { return this.DictionaryInternal[index]; }
		}
		protected override void AssignDictionaryProperties(ISpellCheckerDictionary dictionary) {
			base.AssignDictionaryProperties(dictionary);
			((SpellCheckerDictionaryBase)dictionary).Encoding = GetEncoding();
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			DictionaryInternal.Encoding = Encoding;
			DictionaryInternal.AlphabetPath = GetPhysicalPath(AlphabetPathInternal);
		}
	}
	public class ASPxSpellCheckerDictionary : ASPxSpellCheckerDictionaryBase {
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerDictionaryAlphabetPath"),
#endif
DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string AlphabetPath {
			get { return AlphabetPathInternal; }
			set { AlphabetPathInternal = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SpellCheckerDictionary Dictionary {
			get { return (SpellCheckerDictionary)DictionaryInternal; }
		}
		protected override ISpellCheckerDictionary CreateDictionaryCore() {
			return new SpellCheckerDictionary();
		}
	}
	public class ASPxSpellCheckerCustomDictionary : ASPxSpellCheckerDictionary {
		bool loadedFromStream;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new SpellCheckerCustomDictionary Dictionary {
			get { return (SpellCheckerCustomDictionary)DictionaryInternal; }
		}
		public virtual void AddWord(string word) {
			Dictionary.AddWord(word);
		}
		public virtual void AddWords(string[] words) {
			Dictionary.AddWords(words);
		}
		public virtual void LoadFromStream(Stream dictionaryStream, Stream alphabetStream) {
			try {
				Dictionary.Load(dictionaryStream, alphabetStream);
				loadedFromStream = true;
			}
			catch { }
		}
#if !SL
	[DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerCustomDictionaryIsCustomDictionary")]
#endif
		public override bool IsCustomDictionary {
			get { return true; }
		}
		protected override ISpellCheckerDictionary CreateDictionaryCore() {
			return new SpellCheckerCachedCustomDictionary();
		}
		protected override SpellCheckerDictionaryProvider CreateDictionaryProvider() {
			return new SpellCheckerCustomDictionaryProvider(this);
		}
		protected bool LoadedFromStream { get { return loadedFromStream; } }
		protected override void AssignCore(WebDictionary src) {
			base.AssignCore(src);
			ASPxSpellCheckerCustomDictionary dic = src as ASPxSpellCheckerCustomDictionary;
			if(dic != null && dic.LoadedFromStream)
				AssignDictionary(dic.DictionaryInternal);
		}
	}
	public class ASPxSpellCheckerISpellDictionary : ASPxSpellCheckerDictionaryBase {
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerISpellDictionaryAlphabetPath"),
#endif
DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string AlphabetPath {
			get { return AlphabetPathInternal; }
			set { AlphabetPathInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerISpellDictionaryGrammarPath"),
#endif
DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string GrammarPath {
			get { return GetStringProperty("GrammarPath", ""); }
			set {
				SetStringProperty("GrammarPath", "", value);
				if(!IsDesignMode()) {
					Dictionary.GrammarPath = GetPhysicalPath(value);
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SpellCheckerISpellDictionary Dictionary {
			get { return (SpellCheckerISpellDictionary)DictionaryInternal; }
		}
		protected override void AssignCore(WebDictionary src) {
			base.AssignCore(src);
			ASPxSpellCheckerISpellDictionary dic = src as ASPxSpellCheckerISpellDictionary;
			if (dic != null)
				GrammarPath = dic.GrammarPath;
		}
		protected override ISpellCheckerDictionary CreateDictionaryCore() {
			return new SpellCheckerISpellDictionary();
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			Dictionary.GrammarPath = GetPhysicalPath(GrammarPath);
		}
	}
	public class ASPxSpellCheckerOpenOfficeDictionary : ASPxSpellCheckerISpellDictionary {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string AlphabetPath {
			get { return AlphabetPathInternal; }
			set { AlphabetPathInternal = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new SpellCheckerOpenOfficeDictionary Dictionary {
			get { return (SpellCheckerOpenOfficeDictionary)DictionaryInternal; }
		}
		protected override ISpellCheckerDictionary CreateDictionaryCore() {
			return new SpellCheckerOpenOfficeDictionary();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string EncodingName {
			get {return base.EncodingName; }
			set { base.EncodingName = value;}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Encoding Encoding {
			get { return base.Encoding; }
			set {
				base.Encoding = value;
			}
		}
		protected override void AssignDictionaryProperties(ISpellCheckerDictionary dictionary) {
			dictionary.Culture = GetCulture();
		}
	}
	public class ASPxHunspellDictionary : WebDictionary {
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxHunspellDictionaryGrammarPath"),
#endif
DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string GrammarPath {
			get { return GetStringProperty("GrammarPath", ""); }
			set {
				SetStringProperty("GrammarPath", "", value);
				if (!IsDesignMode()) {
					Dictionary.GrammarPath = GetPhysicalPath(value);
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public HunspellDictionary Dictionary {
			get { return (HunspellDictionary)DictionaryInternal; }
		}
		protected override void AssignCore(WebDictionary src) {
			base.AssignCore(src);
			ASPxHunspellDictionary dic = src as ASPxHunspellDictionary;
			if (dic != null)
				GrammarPath = dic.GrammarPath;
		}
		protected override ISpellCheckerDictionary CreateDictionaryCore() {
			return new HunspellDictionary();
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			Dictionary.GrammarPath = GetPhysicalPath(GrammarPath);
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class SpellCheckerDictionaryCollection : Collection<WebDictionary> {
		public SpellCheckerDictionaryCollection(IWebControlObject webControlObject)
			: base(webControlObject) {
		}
		public override string ToString() { return string.Empty; }
		[Browsable(false)]
		public ASPxSpellChecker SpellChecker { get { return Owner as ASPxSpellChecker; } }
		protected IList List { get { return this; } }
	}
}
namespace DevExpress.XtraSpellChecker {
	public class SpellCheckerCachedCustomDictionary : SpellCheckerCustomDictionary {
		public SpellCheckerCachedCustomDictionary() : base() {
		}
		public SpellCheckerCachedCustomDictionary(string dictionaryPath, CultureInfo culture) : base(dictionaryPath, culture) { }
		public override void SaveAs(string fileName) {}
		protected override void SaveToFile() { }
	}
}
namespace DevExpress.Web.ASPxSpellChecker.Native {
	public class SpellCheckerDictionaryProvider {
		WebDictionary webDictionary;
		public SpellCheckerDictionaryProvider(WebDictionary dictionary) {
			this.webDictionary = dictionary;
		}
		protected virtual WebDictionary WebDictionary {
			get { return this.webDictionary; }
		}
		protected virtual ISpellCheckerDictionary Dictionary {
			get {return WebDictionary.DictionaryInternal; }
		}
		protected Cache Cache {
			get { return HttpContext.Current.Cache; }
		}
		public virtual ISpellCheckerDictionary GetDictionary() { 
			if(!IsDictionaryStored) {
				StoreDictionary();
			}
			return GetDictionaryCore();
		}
		protected virtual void StoreDictionary() {
			if (WebDictionary.CanCacheDictionary) {
				Dictionary.Load();
				if(Dictionary.Loaded && !string.IsNullOrEmpty(Dictionary.DictionaryPath)) {
					CacheDependency cacheDependency = new CacheDependency(Dictionary.DictionaryPath);
					Cache.Add(WebDictionary.GetCacheKey(), Dictionary, cacheDependency, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.NotRemovable, null); 
				}
			}
		}
		protected virtual ISpellCheckerDictionary GetDictionaryCore() {
			if(IsDictionaryStored) {
				ISpellCheckerDictionary result = GetDictionaryInternal();
				if(DictionariesAreEqual(result, Dictionary))
					return result;
				else
					StoreDictionary();
			}
			else
				StoreDictionary();
			return Dictionary;
		}
		protected virtual ISpellCheckerDictionary GetDictionaryInternal() {
			return Cache[WebDictionary.GetCacheKey()] as ISpellCheckerDictionary;
		}
		protected virtual bool DictionariesAreEqual(ISpellCheckerDictionary dict1, ISpellCheckerDictionary dict2) {
			if (Object.ReferenceEquals(dict1, dict2))
				return true;
			if (dict1 == null || dict2 == null)
				return false;
			return dict1.GetType().Equals(dict2.GetType()) && dict1.DictionaryPath == dict2.DictionaryPath && CultureInfo.Equals(dict1.Culture, dict2.Culture);
		}
		protected virtual void SaveWordsToDictionaryCore(string[] words) { throw new NotSupportedException("Only a custom dictionary can be edited"); }
		protected virtual void SaveWordToDictionaryCore(string word) { throw new NotSupportedException("Only a custom dictionary can be edited"); }
		public virtual bool IsDictionaryStored {
			get {
				if (!WebDictionary.CanCacheDictionary)
					return false;
				return Cache[WebDictionary.GetCacheKey()] != null;
			}
		}
		public void SaveWordToDictionary(string word) {
			if(CanSaveWordsToDictionary)
				SaveWordToDictionaryCore(word);
		}
		public void SaveWordsToDictionary(string[] words) {
			if(CanSaveWordsToDictionary)
				SaveWordsToDictionaryCore(words);
		}
		protected virtual bool CanSaveWordsToDictionary {
			get {return WebDictionary.IsCustomDictionary;}
		}
		public virtual void RemoveFromCache() {
			Cache.Remove(WebDictionary.GetCacheKey());
		}
	}
	public class SpellCheckerCustomDictionaryProvider : SpellCheckerDictionaryProvider {
		public SpellCheckerCustomDictionaryProvider(WebDictionary dictionary) : base(dictionary) { }
		protected HttpSessionState Session { get { return HttpContext.Current.Session; } }
		public new SpellCheckerCustomDictionary Dictionary { get { return base.Dictionary as SpellCheckerCustomDictionary; } }
		protected override void StoreDictionary() {
			if(Dictionary.CanCacheDictionary)
				StoreDictionaryInSession();
		}
		protected virtual void StoreDictionaryInSession() {
			if(!Dictionary.Loaded)
				Dictionary.Load();
			if(Dictionary.Loaded)
				Session[WebDictionary.GetCacheKey()] = Dictionary;
		}
		protected override ISpellCheckerDictionary GetDictionaryInternal() {
			return Session[WebDictionary.GetCacheKey()] as ISpellCheckerDictionary;
		}
		public override bool IsDictionaryStored {
			get {return Session[WebDictionary.GetCacheKey()] != null;}
		}
		protected override void SaveWordToDictionaryCore(string word) {
			SpellCheckerCustomDictionary dict = GetDictionary() as SpellCheckerCustomDictionary;
			dict.AddWord(word);
			StoreDictionary();
		}
		protected override void SaveWordsToDictionaryCore(string[] words) {
			SpellCheckerCustomDictionary dict = GetDictionary() as SpellCheckerCustomDictionary;
			dict.AddWords(words);
			StoreDictionary();
		}
		public override void RemoveFromCache() {
			Session.Remove(WebDictionary.GetCacheKey());
		}
	}
}
