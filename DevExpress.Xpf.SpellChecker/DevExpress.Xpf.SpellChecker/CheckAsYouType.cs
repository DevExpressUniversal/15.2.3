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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Resources;
using System.Windows.Threading;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Office.Internal;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.SpellChecker.Native;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Localization;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.XtraSpellChecker.Rules;
using DevExpress.XtraSpellChecker.Strategies;
using WebRequest = System.Net.WebRequest;
using WebResponse = System.Net.WebResponse;
namespace DevExpress.Xpf.SpellChecker {
	public enum UnderlineStyle { WavyLine, Line, Point };
	#region SpellingSettings
	public class SpellingSettings : DependencyObject {
		#region SpellCheckerProperty
		public static readonly DependencyProperty SpellCheckerProperty = DependencyProperty.RegisterAttached("SpellChecker", typeof(SpellChecker), typeof(SpellingSettings), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnSpellCheckerPropertyChanged), new CoerceValueCallback(OnCoerceSpellChecker)));
		static void OnSpellCheckerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CheckAsYouTypeBehavior behavior = BehaviorHelper.TryGetBehavior(d);
			if (behavior != null)
				behavior.OnSpellCheckerChanged((SpellChecker)e.OldValue, (SpellChecker)e.NewValue);
		}
		static object OnCoerceSpellChecker(DependencyObject d, object value) {
			if (d is DevExpress.Xpf.Core.FloatingContainer)
				return null;
			if (d is TextBox && DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<TextEditBase>(d) != null)
				return null;
			if (d.GetType().ToString() == "DevExpress.Xpf.RichEdit.Controls.Internal.KeyCodeConverter")
				return null;
			return value;
		}
		public static SpellChecker GetSpellChecker(DependencyObject obj) {
			return (SpellChecker)obj.GetValue(SpellCheckerProperty);
		}
		public static void SetSpellChecker(DependencyObject obj, SpellChecker value) {
			obj.SetValue(SpellCheckerProperty, value);
		}
		#endregion
		#region CheckAsYouTypeProperty
		public static readonly DependencyProperty CheckAsYouTypeProperty =
			DependencyProperty.RegisterAttached("CheckAsYouType", typeof(bool), typeof(SpellingSettings), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnCheckAsYouTypePropertyChanged), new CoerceValueCallback(OnCoerceCheckAsYouType)));
		static void OnCheckAsYouTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if ((bool)e.NewValue)
				BehaviorHelper.AddBehavior(d);
			else
				BehaviorHelper.RemoveBehavior(d);
		}
		static object OnCoerceCheckAsYouType(DependencyObject d, object value) {
			if (d is DevExpress.Xpf.SpellChecker.Forms.SpellCheckerControlBase)
				return false;
			if (d is TextBox && DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<TextEditBase>(d) != null)
				return false;
			if (d.GetType().ToString() == "DevExpress.Xpf.RichEdit.Controls.Internal.KeyCodeConverter")
				return false;
			return value;
		}
		public static bool GetCheckAsYouType(DependencyObject obj) {
			return (bool)obj.GetValue(CheckAsYouTypeProperty);
		}
		public static void SetCheckAsYouType(DependencyObject obj, bool value) {
			obj.SetValue(CheckAsYouTypeProperty, value);
		}
		#endregion
		#region UnderlineColorProperty
		public static readonly DependencyProperty UnderlineColorProperty = DependencyProperty.RegisterAttached("UnderlineColor", typeof(Color), typeof(SpellingSettings), new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnUnderlineStyleOrColorChanged)));
		public static Color GetUnderlineColor(DependencyObject obj) {
			return (Color)obj.GetValue(UnderlineColorProperty);
		}
		public static void SetUnderlineColor(DependencyObject obj, Color value) {
			obj.SetValue(UnderlineColorProperty, value);
		}
		#endregion
		#region UnderlineStyleProperty
		public static readonly DependencyProperty UnderlineStyleProperty = DependencyProperty.RegisterAttached("UnderlineStyle", typeof(UnderlineStyle), typeof(SpellingSettings), new FrameworkPropertyMetadata(UnderlineStyle.WavyLine, FrameworkPropertyMetadataOptions.Inherits, OnUnderlineStyleOrColorChanged));
		public static UnderlineStyle GetUnderlineStyle(DependencyObject obj) {
			return (UnderlineStyle)obj.GetValue(UnderlineStyleProperty);
		}
		public static void SetUnderlineStyle(DependencyObject obj, UnderlineStyle value) {
			obj.SetValue(UnderlineStyleProperty, value);
		}
		#endregion
		#region ShowSpellCheckMenuProperty
		public static readonly DependencyProperty ShowSpellCheckMenuProperty = DependencyProperty.RegisterAttached("ShowSpellCheckMenu", typeof(bool), typeof(SpellingSettings), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
		public static bool GetShowSpellCheckMenu(DependencyObject obj) {
			return (bool)obj.GetValue(ShowSpellCheckMenuProperty);
		}
		public static void SetShowSpellCheckMenu(DependencyObject obj, bool value) {
			obj.SetValue(ShowSpellCheckMenuProperty, value);
		}
		#endregion
		#region CultureProperty
		public static readonly DependencyProperty CultureProperty = DependencyProperty.RegisterAttached("Culture", typeof(CultureInfo), typeof(SpellingSettings), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		public static CultureInfo GetCulture(DependencyObject obj) {
			return (CultureInfo)obj.GetValue(CultureProperty);
		}
		public static void SetCulture(DependencyObject obj, CultureInfo value) {
			obj.SetValue(CultureProperty, value);
		}
		#endregion
		#region IgnoreEmailsProperty
		public static readonly DependencyProperty IgnoreEmailsProperty = DependencyProperty.RegisterAttached("IgnoreEmails", typeof(DefaultBoolean), typeof(SpellingSettings), new FrameworkPropertyMetadata(DefaultBoolean.True, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnIgnoreEmailsChanged)));
		static void OnIgnoreEmailsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellChecker.GetSpellCheckerOptions(d).IgnoreEmails = (DefaultBoolean)e.NewValue;
		}
		public static DefaultBoolean GetIgnoreEmails(DependencyObject obj) {
			return (DefaultBoolean)obj.GetValue(IgnoreEmailsProperty);
		}
		public static void SetIgnoreEmails(DependencyObject obj, DefaultBoolean value) {
			obj.SetValue(IgnoreEmailsProperty, value);
		}
		#endregion
		#region IgnoreMixedCaseWordsProperty
		public static readonly DependencyProperty IgnoreMixedCaseWordsProperty = DependencyProperty.RegisterAttached("IgnoreMixedCaseWords", typeof(DefaultBoolean), typeof(SpellingSettings), new FrameworkPropertyMetadata(DefaultBoolean.True, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnIgnoreMixedCaseWordsChanged)));
		static void OnIgnoreMixedCaseWordsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellChecker.GetSpellCheckerOptions(d).IgnoreMixedCaseWords = (DefaultBoolean)e.NewValue;
		}
		public static DefaultBoolean GetIgnoreMixedCaseWords(DependencyObject obj) {
			return (DefaultBoolean)obj.GetValue(IgnoreMixedCaseWordsProperty);
		}
		public static void SetIgnoreMixedCaseWords(DependencyObject obj, DefaultBoolean value) {
			obj.SetValue(IgnoreMixedCaseWordsProperty, value);
		}
		#endregion
		#region IgnoreRepeatedWordsProperty
		public static readonly DependencyProperty IgnoreRepeatedWordsProperty = DependencyProperty.RegisterAttached("IgnoreRepeatedWords", typeof(DefaultBoolean), typeof(SpellingSettings), new FrameworkPropertyMetadata(DefaultBoolean.False, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnIgnoreRepeatedWords)));
		static void OnIgnoreRepeatedWords(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellChecker.GetSpellCheckerOptions(d).IgnoreRepeatedWords = (DefaultBoolean)e.NewValue;
		}
		public static DefaultBoolean GetIgnoreRepeatedWords(DependencyObject obj) {
			return (DefaultBoolean)obj.GetValue(IgnoreRepeatedWordsProperty);
		}
		public static void SetIgnoreRepeatedWords(DependencyObject obj, DefaultBoolean value) {
			obj.SetValue(IgnoreRepeatedWordsProperty, value);
		}
		#endregion
		#region IgnoreUpperCaseWordsProperty
		public static readonly DependencyProperty IgnoreUpperCaseWordsProperty = DependencyProperty.RegisterAttached("IgnoreUpperCaseWords", typeof(DefaultBoolean), typeof(SpellingSettings), new FrameworkPropertyMetadata(DefaultBoolean.True, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnIgnoreUpperCaseWordsChanged)));
		static void OnIgnoreUpperCaseWordsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellChecker.GetSpellCheckerOptions(d).IgnoreUpperCaseWords = (DefaultBoolean)e.NewValue;
		}
		public static DefaultBoolean GetIgnoreUpperCaseWords(DependencyObject obj) {
			return (DefaultBoolean)obj.GetValue(IgnoreUpperCaseWordsProperty);
		}
		public static void SetIgnoreUpperCaseWords(DependencyObject obj, DefaultBoolean value) {
			obj.SetValue(IgnoreUpperCaseWordsProperty, value);
		}
		#endregion
		#region IgnoreUrlsProperty
		[Obsolete("This property has become obsolete. Use the IgnoreUriProperty property instead.")]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static readonly DependencyProperty IgnoreUrlsProperty = DependencyProperty.RegisterAttached("IgnoreUrls", typeof(DefaultBoolean), typeof(SpellingSettings), new FrameworkPropertyMetadata(DefaultBoolean.True, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnIgnoreUrlsChanged)));
		static void OnIgnoreUrlsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
#pragma warning disable 0618
			SpellChecker.GetSpellCheckerOptions(d).IgnoreUrls = (DefaultBoolean)e.NewValue;
#pragma warning restore 0618
		}
		[Obsolete("This method has become obsolete. Use the GetIgnoreUri method instead.")]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static DefaultBoolean GetIgnoreUrls(DependencyObject obj) {
#pragma warning disable 0618
			return (DefaultBoolean)obj.GetValue(IgnoreUrlsProperty);
#pragma warning restore 0618
		}
		[Obsolete("This method has become obsolete. Use the SetIgnoreUri method instead.")]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public static void SetIgnoreUrls(DependencyObject obj, DefaultBoolean value) {
#pragma warning disable 0618
			obj.SetValue(IgnoreUrlsProperty, value);
#pragma warning restore 0618
		}
		#endregion
		#region IgnoreUriProperty
		public static readonly DependencyProperty IgnoreUriProperty = DependencyProperty.RegisterAttached("IgnoreUri", typeof(DefaultBoolean), typeof(SpellingSettings), new FrameworkPropertyMetadata(DefaultBoolean.True, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnIgnoreUriChanged)));
		static void OnIgnoreUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellChecker.GetSpellCheckerOptions(d).IgnoreUri = (DefaultBoolean)e.NewValue;
		}
		public static DefaultBoolean GetIgnoreUri(DependencyObject obj) {
			return (DefaultBoolean)obj.GetValue(IgnoreUriProperty);
		}
		public static void SetIgnoreUri(DependencyObject obj, DefaultBoolean value) {
			obj.SetValue(IgnoreUriProperty, value);
		}
		#endregion
		#region IgnoreWordsWithNumbersProperty
		public static readonly DependencyProperty IgnoreWordsWithNumbersProperty = DependencyProperty.RegisterAttached("IgnoreWordsWithNumbers", typeof(DefaultBoolean), typeof(SpellingSettings), new FrameworkPropertyMetadata(DefaultBoolean.True, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnIgnoreWordsWithNumbers)));
		static void OnIgnoreWordsWithNumbers(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellChecker.GetSpellCheckerOptions(d).IgnoreWordsWithNumbers = (DefaultBoolean)e.NewValue;
		}
		public static DefaultBoolean GetIgnoreWordsWithNumbers(DependencyObject obj) {
			return (DefaultBoolean)obj.GetValue(IgnoreWordsWithNumbersProperty);
		}
		public static void SetIgnoreWordsWithNumbers(DependencyObject obj, DefaultBoolean value) {
			obj.SetValue(IgnoreWordsWithNumbersProperty, value);
		}
		#endregion
		#region DictionarySourceCollectionProperty
		public static readonly DependencyProperty DictionarySourceCollectionProperty = DependencyProperty.RegisterAttached("DictionarySourceCollection", typeof(DictionarySourceCollection), typeof(SpellingSettings), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnDictionarySourceCollectionChanged)));
		static void OnDictionarySourceCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DictionarySourceCollection newDictionarySource = (DictionarySourceCollection)e.NewValue;
			if (newDictionarySource != null && newDictionarySource.Owner == null)
				newDictionarySource.Owner = d;
			CheckAsYouTypeBehavior behavior = BehaviorHelper.TryGetBehavior(d);
			if (behavior != null)
				behavior.OnDictionarySourceCollectionChanged((DictionarySourceCollection)e.OldValue, newDictionarySource);
		}
		public static DictionarySourceCollection GetDictionarySourceCollection(DependencyObject obj) {
			DictionarySourceCollection result = (DictionarySourceCollection)obj.GetValue(DictionarySourceCollectionProperty);
			if (result == null)
				return null;
			foreach (DictionarySourceBase source in result)
				if (source.Culture == null)
					source.Culture = GetCulture(obj);
			return result;
		}
		public static void SetDictionarySourceCollection(DependencyObject obj, DictionarySourceCollection value) {
			obj.SetValue(DictionarySourceCollectionProperty, value);
		}
		#endregion
		static void OnUnderlineStyleOrColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CheckAsYouTypeBehavior behavior = BehaviorHelper.TryGetBehavior(d);
			if (behavior != null)
				behavior.Check(0, 0);
		}
	}
	#endregion
	public class DictionarySourceBindingHelper : Freezable {
		public static readonly DependencyProperty DataContextProperty = DependencyProperty.Register("DataContext", typeof(object), typeof(DictionarySourceBindingHelper), new PropertyMetadata(null));
		public object DataContext {
			get { return (object)GetValue(DataContextProperty); }
			set { SetValue(DataContextProperty, value); }
		}
		protected override Freezable CreateInstanceCore() {
			return new DictionarySourceBindingHelper();
		}
	}
	public class DictionarySourceCollection : ObservableCollection<DictionarySourceBase> {
		internal DependencyObject Owner { get; set; }
	}
	#region DictionarySourceBase
	public abstract class DictionarySourceBase : DependencyObject {
		#region Culture
		public static readonly DependencyProperty CultureProperty = DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(DictionarySourceBase), new PropertyMetadata(OnCultureChanged));
		static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DictionarySourceBase)d).OnCultureChanged();
		}
		public CultureInfo Culture {
			get { return (CultureInfo)GetValue(CultureProperty); }
			set { SetValue(CultureProperty, value); }
		}
		#endregion
		#region DictionaryUri
		public static readonly DependencyProperty DictionaryUriProperty = DependencyProperty.Register("DictionaryUri", typeof(Uri), typeof(DictionarySourceBase), new PropertyMetadata(ValidateUri));
		protected static void ValidateUri(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
				return;
			UriValidator.Validate((Uri)e.NewValue);
		}
		public Uri DictionaryUri {
			get { return (Uri)GetValue(DictionaryUriProperty); }
			set { SetValue(DictionaryUriProperty, value); }
		}
		#endregion
		protected abstract void OnCultureChanged();
		internal abstract ISpellCheckerDictionary GetDictionaryInstance();
		internal virtual Stream GetStream(Uri uri) {
			if (uri == null)
				return null;
			Stream result = GetResourceStream(uri);
			if (result == null)
				result = GetContentStream(uri);
			if (result == null)
				result = GetRemoteStream(uri);
			return result;
		}
		#region GetResourceStream
		Stream GetResourceStream(Uri uri) {
			StreamResourceInfo info = null;
			try {
				info = Application.GetResourceStream(uri);
			}
			catch { }
			return info != null ? info.Stream : null;
		}
		#endregion
		#region GetContentStream
		Stream GetContentStream(Uri uri) {
			StreamResourceInfo info = null;
			try {
				info = Application.GetContentStream(uri);
			}
			catch { }
			return info != null ? info.Stream : null;
		}
		#endregion
		#region GetRemoteStream
		Stream GetRemoteStream(Uri uri) {
			StreamResourceInfo info = null;
			try {
				info = Application.GetRemoteStream(uri);
			}
			catch { }
			return info != null ? info.Stream : null;
		}
		#endregion
	}
	#endregion
	#region DictionarySource<TDictionary>
	public abstract class DictionarySource<TDictionary> : DictionarySourceBase where TDictionary : DevExpress.XtraSpellChecker.DictionaryBase {
		TDictionary dictionary = null;
		protected DictionarySource() {
			Dictionary = CreateDictionary();
			Dictionary.DictionaryLoading += OnDictionaryLoading;
		}
		internal TDictionary Dictionary { get { return dictionary; } set { dictionary = value; } }
		internal abstract void LoadCore(Stream dictionaryStream);
		internal abstract TDictionary CreateDictionary();
		protected override void OnCultureChanged() {
			Dictionary.Culture = Culture;
		}
		internal override ISpellCheckerDictionary GetDictionaryInstance() {
			return Dictionary;
		}
		void OnDictionaryLoading(object sender, DictionaryLoadingEventAgrs e) {
			Load();
			e.Handled = true;
		}
		void Load() {
			Stream dictionaryStream = GetStream(DictionaryUri);
			if (dictionaryStream == null)
				UriValidator.RaiseResourceNotFoundException(DictionaryUri);
			try {
				LoadCore(dictionaryStream);
			}
			finally {
				dictionaryStream.Close();
			}
		}
	}
	#endregion
	#region SpellCheckerDictionarySource
	public class SpellCheckerDictionarySource : DictionarySource<SpellCheckerDictionary> {
		public SpellCheckerDictionarySource()
			: base() {
		}
		#region AlphabetUri
		public static readonly DependencyProperty AlphabetUriProperty = DependencyProperty.Register("AlphabetUri", typeof(Uri), typeof(SpellCheckerDictionarySource), new PropertyMetadata(ValidateUri));
		public Uri AlphabetUri {
			get { return (Uri)GetValue(AlphabetUriProperty); }
			set { SetValue(AlphabetUriProperty, value); }
		}
		#endregion
		#region CaseSensitive
		public static readonly DependencyProperty CaseSensitiveProperty = DependencyProperty.Register("CaseSensitive", typeof(bool), typeof(SpellCheckerDictionarySource), new PropertyMetadata(false, OnCaseSensitiveChanged));
		static void OnCaseSensitiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellCheckerDictionarySource source = (SpellCheckerDictionarySource)d;
			if (source.Dictionary != null)
				source.Dictionary.CaseSensitive = (bool)e.NewValue;
		}
		public bool CaseSensitive {
			get { return (bool)GetValue(CaseSensitiveProperty); }
			set { SetValue(CaseSensitiveProperty, value); }
		}
		#endregion
		#region Encoding
		public static readonly DependencyProperty EncodingProperty = DependencyProperty.Register("Encoding", typeof(Encoding), typeof(SpellCheckerDictionarySource), new PropertyMetadata(OnEncodingChanged));
		static void OnEncodingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellCheckerDictionarySource source = (SpellCheckerDictionarySource)d;
			if (source.Dictionary != null)
				source.Dictionary.Encoding = (Encoding)e.NewValue;
		}
		public Encoding Encoding {
			get { return (Encoding)GetValue(EncodingProperty); }
			set { SetValue(EncodingProperty, value); }
		}
		#endregion
		internal override void LoadCore(Stream dictionaryStream) {
			Stream alphabetStream = GetStream(AlphabetUri);
			if (alphabetStream == null)
				UriValidator.RaiseResourceNotFoundException(AlphabetUri);
			try {
				Dictionary.Load(dictionaryStream, alphabetStream);
			}
			finally {
				alphabetStream.Close();
			}
		}
		internal override SpellCheckerDictionary CreateDictionary() {
			return new SpellCheckerDictionary();
		}
	}
	#endregion
	#region SpellCheckerCustomDictionarySource
	public class SpellCheckerCustomDictionarySource : SpellCheckerDictionarySource {
		public SpellCheckerCustomDictionarySource()
			: base() {
		}
		internal override SpellCheckerDictionary CreateDictionary() {
			return new SpellCheckerCustomDictionary();
		}
	}
	#endregion
	#region SpellCheckerISpellDictionarySource
	public class SpellCheckerISpellDictionarySource : DictionarySource<SpellCheckerISpellDictionary> {
		public SpellCheckerISpellDictionarySource()
			: base() {
		}
		#region GrammarUri
		public static readonly DependencyProperty GrammarUriProperty = DependencyProperty.Register("GrammarUri", typeof(Uri), typeof(SpellCheckerISpellDictionarySource), new PropertyMetadata(ValidateUri));
		public Uri GrammarUri {
			get { return (Uri)GetValue(GrammarUriProperty); }
			set { SetValue(GrammarUriProperty, value); }
		}
		#endregion
		#region AlphabetUri
		public static readonly DependencyProperty AlphabetUriProperty = DependencyProperty.Register("AlphabetUri", typeof(Uri), typeof(SpellCheckerISpellDictionarySource), new PropertyMetadata(ValidateUri));
		public Uri AlphabetUri {
			get { return (Uri)GetValue(AlphabetUriProperty); }
			set { SetValue(AlphabetUriProperty, value); }
		}
		#endregion
		#region CaseSensitive
		public static readonly DependencyProperty CaseSensitiveProperty = DependencyProperty.Register("CaseSensitive", typeof(bool), typeof(SpellCheckerISpellDictionarySource), new PropertyMetadata(false, OnCaseSensitiveChanged));
		static void OnCaseSensitiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellCheckerISpellDictionarySource source = (SpellCheckerISpellDictionarySource)d;
			if (source.Dictionary != null)
				source.Dictionary.CaseSensitive = (bool)e.NewValue;
		}
		public bool CaseSensitive {
			get { return (bool)GetValue(CaseSensitiveProperty); }
			set { SetValue(CaseSensitiveProperty, value); }
		}
		#endregion
		#region Encoding
		public static readonly DependencyProperty EncodingProperty = DependencyProperty.Register("Encoding", typeof(Encoding), typeof(SpellCheckerISpellDictionarySource), new PropertyMetadata(null, OnEncodingChanged));
		static void OnEncodingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpellCheckerISpellDictionarySource source = (SpellCheckerISpellDictionarySource)d;
			if (source.Dictionary != null) {
				Encoding encoding = (Encoding)e.NewValue;
				if (encoding != null)
					source.Dictionary.Encoding = encoding;
			}
		}
		public Encoding Encoding {
			get { return (Encoding)GetValue(EncodingProperty); }
			set { SetValue(EncodingProperty, value); }
		}
		#endregion
		internal override void LoadCore(Stream dictionaryStream) {
			Stream grammarStream = GetStream(GrammarUri);
			if (grammarStream == null)
				UriValidator.RaiseResourceNotFoundException(GrammarUri);
			Stream alphabetStream = GetStream(AlphabetUri);
			try {
				Dictionary.LoadFromStream(dictionaryStream, grammarStream, alphabetStream);
			}
			finally {
				grammarStream.Close();
				if (alphabetStream != null)
					alphabetStream.Close();
			}
		}
		internal override SpellCheckerISpellDictionary CreateDictionary() {
			return new SpellCheckerISpellDictionary();
		}
	}
	#endregion
	#region SpellCheckerOpenOfficeDictionarySource
	public class SpellCheckerOpenOfficeDictionarySource : SpellCheckerISpellDictionarySource {
		public SpellCheckerOpenOfficeDictionarySource()
			: base() {
		}
		internal override SpellCheckerISpellDictionary CreateDictionary() {
			return new SpellCheckerOpenOfficeDictionary();
		}
	}
	#endregion
	#region HunspellDictionarySource
	public class HunspellDictionarySource : DictionarySource<HunspellDictionary> {
		public HunspellDictionarySource()
			: base() {
		}
		#region GrammarUri
		public static readonly DependencyProperty GrammarUriProperty = DependencyProperty.Register("GrammarUri", typeof(Uri), typeof(HunspellDictionarySource), new PropertyMetadata(ValidateUri));
		public Uri GrammarUri {
			get { return (Uri)GetValue(GrammarUriProperty); }
			set { SetValue(GrammarUriProperty, value); }
		}
		#endregion
		internal override void LoadCore(Stream dictionaryStream) {
			Stream grammarStream = GetStream(GrammarUri);
			if (grammarStream == null)
				UriValidator.RaiseResourceNotFoundException(GrammarUri);
			try {
				Dictionary.LoadFromStream(dictionaryStream, grammarStream);
			}
			finally {
				grammarStream.Close();
			}
		}
		internal override HunspellDictionary CreateDictionary() {
			return new HunspellDictionary();
		}
	}
	#endregion
	public class ResourceNotFoundException : Exception {
		public ResourceNotFoundException(Uri resource)
			: base(String.Format("Resource \"{0}\" not found.", resource)) {
		}
	}
}
namespace DevExpress.Xpf.SpellChecker.Native {
	internal interface IEditControlAdapter {
		string Text { get; }
		Dispatcher Dispatcher { get; }
		bool IsReadOnly { get; set; }
		int SelectionStart { get; }
		int SelectionLength { get; }
		Point GetPointFromCharacterIndex(int index);
		Point GetLineStartPositionFromCharacterIndex(int index);
		Point GetLineFinishPositionFromCharacterIndex(int index);
		Position GetFirstVisibleCharacterPosition();
		Position GetLastVisibleCharacterPosition();
		int GetPrevLineStartIndexFromCharacterIndex(int index);
		int GetLineFinishIndexFromCharacterIndex(int index);
		bool IsCharactersInSameLine(int firstCharIndex, int secondCharIndex);
		Position GetPositionFromPoint(Point point);
		int ConvertToInnerPosition(int position);
		int GetLinesCount();
		string GetOriginalText();
		bool IsTextStartPosition(int position);
	}
	#region EditControlAdapterBase
	abstract class EditControlAdapterBase : IEditControlAdapter {
		readonly Control editor;
		protected EditControlAdapterBase(Control editor) {
			this.editor = editor;
		}
		public abstract string Text { get; }
		public abstract bool IsReadOnly { get; set; }
		public Dispatcher Dispatcher { get { return Editor.Dispatcher; } }
		public abstract int SelectionStart { get; }
		public abstract int SelectionLength { get; }
		internal Control Editor { get { return editor; } }
		public abstract Position GetFirstVisibleCharacterPosition();
		public abstract Position GetLastVisibleCharacterPosition();
		public abstract int GetPrevLineStartIndexFromCharacterIndex(int index);
		public abstract int GetLineFinishIndexFromCharacterIndex(int index);
		public abstract Point GetPointFromCharacterIndex(int index);
		public abstract Point GetLineStartPositionFromCharacterIndex(int index);
		public abstract Point GetLineFinishPositionFromCharacterIndex(int index);
		public abstract bool IsCharactersInSameLine(int firstCharIndex, int secondCharIndex);
		public abstract Position GetPositionFromPoint(Point point);
		public abstract int GetLinesCount();
		public virtual string GetOriginalText() {
			return Text;
		}
		public virtual int ConvertToInnerPosition(int position) {
			return position;
		}
		public virtual bool IsTextStartPosition(int position) {
			return position == 0;
		}
		protected double GetHorizontalScrollBarHeight() {
			ScrollViewer viewer = DevExpress.Xpf.Core.Native.LayoutHelper.FindElementByType<ScrollViewer>(Editor);
			return viewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible ? SystemParameters.HorizontalScrollBarHeight : 0;
		}
	}
	#endregion
	#region TextBoxAdapter
	class TextBoxAdapter : EditControlAdapterBase {
		public TextBoxAdapter(TextBox editor)
			: base(editor) {
		}
		internal new TextBox Editor { get { return base.Editor as TextBox; } }
		public override string Text { get { return Editor.Text; } }
		public override bool IsReadOnly { get { return Editor.IsReadOnly; } set { Editor.IsReadOnly = value; } }
		public override int SelectionStart { get { return Editor.SelectionStart; } }
		public override int SelectionLength { get { return Editor.SelectionLength; } }
		int GetLineStartIndex(int index) {
			int lineIndex = Editor.GetLineIndexFromCharacterIndex(index);
			return Editor.GetCharacterIndexFromLineIndex(lineIndex);
		}
		int GetLineFinishIndex(int characterIndex) {
			int lineIndex = Editor.GetLineIndexFromCharacterIndex(characterIndex);
			int firstCharIndex = Editor.GetCharacterIndexFromLineIndex(lineIndex);
			int lineLength = Editor.GetLineLength(lineIndex);
			int result = firstCharIndex + lineLength - 1;
			string line = Text.Substring(firstCharIndex, result - firstCharIndex + 1);
			if (line.Contains(Environment.NewLine))
				result -= Environment.NewLine.Length;
			return result;
		}
		public override Position GetFirstVisibleCharacterPosition() {
			Editor.UpdateLayout();
			if (String.IsNullOrEmpty(Text))
				return null;
			int firstVisibleLineIndex = Editor.GetFirstVisibleLineIndex();
			if (firstVisibleLineIndex < 0)
				return null;
			int firstVisibleCharacterIndex = Editor.GetCharacterIndexFromLineIndex(firstVisibleLineIndex);
			Rect rect = Editor.GetRectFromCharacterIndex(firstVisibleCharacterIndex);
			if (ErrorBoxesCalculator.CompareCoordinates(rect.Bottom, Editor.ActualHeight - Editor.Padding.Bottom) > 0)
				return null;
			return new IntPosition(firstVisibleCharacterIndex);
		}
		public override Position GetLastVisibleCharacterPosition() {
			Editor.UpdateLayout();
			if (String.IsNullOrEmpty(Text))
				return null;
			int characterIndex = GetLastVisibleLineStartIndex();
			if (characterIndex < 0)
				return null;
			int lastVisibleCharacterIndex = GetLineFinishIndex(characterIndex);
			if (lastVisibleCharacterIndex == Text.Length - 1 && Char.IsLetterOrDigit(Text[lastVisibleCharacterIndex]))
				lastVisibleCharacterIndex = Text.Length;
			return new IntPosition(lastVisibleCharacterIndex);
		}
		int GetLastVisibleLineStartIndex() {
			int result = 0;
			int lastVisibleLine = Editor.GetLastVisibleLineIndex();
			if (lastVisibleLine < 0)
				return -1;
			result = Editor.GetCharacterIndexFromLineIndex(lastVisibleLine);
			Rect rect = Editor.GetRectFromCharacterIndex(result);
			double horizontalScrollBarHeight = GetHorizontalScrollBarHeight();
			while (ErrorBoxesCalculator.CompareCoordinates(rect.Bottom, Editor.ActualHeight - Editor.Padding.Bottom - horizontalScrollBarHeight) > 0) {
				if (lastVisibleLine < 0)
					return -1;
				result = Editor.GetCharacterIndexFromLineIndex(lastVisibleLine);
				rect = Editor.GetRectFromCharacterIndex(result);
				lastVisibleLine--;
			}
			return result;
		}
		public override int GetPrevLineStartIndexFromCharacterIndex(int index) {
			Editor.UpdateLayout();
			int lineStart = GetLineStartIndex(index);
			return GetLineStartIndex(Math.Abs(lineStart - 1));
		}
		public override int GetLineFinishIndexFromCharacterIndex(int index) {
			Editor.UpdateLayout();
			return GetLineFinishIndex(index);
		}
		public override Point GetPointFromCharacterIndex(int index) {
			Editor.UpdateLayout();
			return Editor.GetRectFromCharacterIndex(index).BottomLeft;
		}
		public override Point GetLineStartPositionFromCharacterIndex(int index) {
			Editor.UpdateLayout();
			return Editor.GetRectFromCharacterIndex(GetLineStartIndex(index)).BottomLeft;
		}
		public override Point GetLineFinishPositionFromCharacterIndex(int index) {
			Editor.UpdateLayout();
			int lastCharIndex = GetLineFinishIndex(index);
			return Editor.GetRectFromCharacterIndex(lastCharIndex).BottomRight;
		}
		public override bool IsCharactersInSameLine(int firstCharIndex, int secondCharIndex) {
			return Editor.GetLineIndexFromCharacterIndex(firstCharIndex) == Editor.GetLineIndexFromCharacterIndex(secondCharIndex);
		}
		public override Position GetPositionFromPoint(Point point) {
			return new IntPosition(Editor.GetCharacterIndexFromPoint(point, true));
		}
		public override int GetLinesCount() {
			if (String.IsNullOrEmpty(Text))
				return 0;
			return Editor.GetLineIndexFromCharacterIndex(Text.Length - 1) + 1;
		}
	}
	#endregion
	#region TextEditAdapter
	class TextEditAdapter : EditControlAdapterBase {
		IEditControlAdapter innerAdapter;
		public TextEditAdapter(TextEdit editor)
			: base(editor) {
			this.innerAdapter = new TextBoxAdapter(InnerEditor);
		}
		new TextEdit Editor { get { return base.Editor as TextEdit; } }
		internal TextBox InnerEditor { get { return Editor.EditCore as TextBox; } }
		IEditControlAdapter InnerAdapter {
			get {
				if (((TextBoxAdapter)innerAdapter).Editor != InnerEditor)
					innerAdapter = new TextBoxAdapter(InnerEditor);
				return innerAdapter;
			}
		}
		public override string Text {
			get {
				string result = Editor.Text;
				return result != null ? result : String.Empty;
			}
		}
		public override bool IsReadOnly { get { return Editor.IsReadOnly; } set { Editor.IsReadOnly = value; } }
		public override int SelectionStart { get { return Editor.SelectionStart; } }
		public override int SelectionLength { get { return Editor.SelectionLength; } }
		public override Position GetFirstVisibleCharacterPosition() {
			return InnerAdapter.GetFirstVisibleCharacterPosition();
		}
		public override Position GetLastVisibleCharacterPosition() {
			return InnerAdapter.GetLastVisibleCharacterPosition();
		}
		public override int GetPrevLineStartIndexFromCharacterIndex(int index) {
			return InnerAdapter.GetPrevLineStartIndexFromCharacterIndex(index);
		}
		public override int GetLineFinishIndexFromCharacterIndex(int index) {
			return InnerAdapter.GetLineFinishIndexFromCharacterIndex(index);
		}
		public override Point GetPointFromCharacterIndex(int index) {
			return InnerEditor.TranslatePoint(InnerAdapter.GetPointFromCharacterIndex(index), Editor);
		}
		public override Point GetLineStartPositionFromCharacterIndex(int index) {
			return InnerEditor.TranslatePoint(InnerAdapter.GetLineStartPositionFromCharacterIndex(index), Editor);
		}
		public override Point GetLineFinishPositionFromCharacterIndex(int index) {
			return InnerEditor.TranslatePoint(InnerAdapter.GetLineFinishPositionFromCharacterIndex(index), Editor);
		}
		public override bool IsCharactersInSameLine(int firstCharIndex, int secondCharIndex) {
			return InnerAdapter.IsCharactersInSameLine(firstCharIndex, secondCharIndex);
		}
		public override Position GetPositionFromPoint(Point point) {
			return InnerAdapter.GetPositionFromPoint(Editor.TranslatePoint(point, InnerEditor));
		}
		public override int GetLinesCount() {
			return InnerAdapter.GetLinesCount();
		}
	}
	#endregion
	#region RichTextBoxAdapter
	class RichTextBoxAdapter : EditControlAdapterBase {
		AsYouTypeRichTextBoxHelper helper;
		public RichTextBoxAdapter(RichTextBox editor)
			: base(editor) {
			this.helper = CreateHelper();
		}
		new RichTextBox Editor { get { return base.Editor as RichTextBox; } }
		internal AsYouTypeRichTextBoxHelper Helper { get { return helper; } }
		public override string Text { get { return Helper.GetText(); } }
		public override bool IsReadOnly { get { return Editor.IsReadOnly; } set { Editor.IsReadOnly = value; } }
		public override int SelectionStart {
			get {
				Helper.Invalidate(false);
				return Helper.GetOffset(Editor.Document.ContentStart, Editor.Selection.Start);
			}
		}
		public override int SelectionLength {
			get {
				Helper.Invalidate(false);
				int selectionEnd = Helper.GetOffset(Editor.Document.ContentStart, Editor.Selection.End);
				return selectionEnd - SelectionStart;
			}
		}
		public override string GetOriginalText() {
			return new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd).Text;
		}
		protected virtual AsYouTypeRichTextBoxHelper CreateHelper() {
			return new AsYouTypeRichTextBoxHelper(Editor);
		}
		TextPointer GetPointerFromCharacterIndex(TextPointer start, int index) {
			return Helper.GetPointerFromCharacterIndex(start, index, LogicalDirection.Forward);
		}
		public override Position GetFirstVisibleCharacterPosition() {
			Helper.Invalidate(true);
			if (String.IsNullOrEmpty(Text))
				return null;
			TextPointer characterPointer = Editor.GetPositionFromPoint(new Point(Editor.Padding.Left, Editor.Padding.Top), true);
			Rect rect = characterPointer.GetCharacterRect(LogicalDirection.Forward);
			while (rect.Top < Editor.Padding.Top) {
				characterPointer = characterPointer.GetLineStartPosition(1);
				rect = characterPointer.GetCharacterRect(LogicalDirection.Forward);
			}
			int offset = Helper.GetOffset(Editor.Document.ContentStart, characterPointer);
			if (rect.Bottom > Editor.ActualHeight - Editor.Padding.Bottom)
				return null;
			Helper.CachePosition(characterPointer, offset, true);
			return new IntPosition(offset);
		}
		public override Position GetLastVisibleCharacterPosition() {
			if (String.IsNullOrEmpty(Text))
				return null;
			Helper.Invalidate(false);
			double x = Editor.ActualWidth - Editor.Padding.Left - Editor.Padding.Right;
			double y = Editor.ActualHeight - Editor.Padding.Top - Editor.Padding.Bottom;
			TextPointer lastVisibleCharacter = Editor.GetPositionFromPoint(new Point(x, y), true);
			Rect rect = lastVisibleCharacter.GetCharacterRect(LogicalDirection.Forward);
			double horizontalScrollBarHeight = GetHorizontalScrollBarHeight();
			while (rect.Bottom > y - horizontalScrollBarHeight) {
				lastVisibleCharacter = lastVisibleCharacter.GetLineStartPosition(0);
				lastVisibleCharacter = lastVisibleCharacter.GetNextInsertionPosition(LogicalDirection.Backward);
				if (lastVisibleCharacter == null)
					return null;
				rect = lastVisibleCharacter.GetCharacterRect(LogicalDirection.Forward);
			}
			return new IntPosition(Helper.GetOffset(Editor.Document.ContentStart, lastVisibleCharacter));
		}
		public override int GetPrevLineStartIndexFromCharacterIndex(int index) {
			TextPointer pointer = GetPointerFromCharacterIndex(Editor.Document.ContentStart, index);
			if (pointer == null)
				return 0;
			TextPointer prevLineStart = pointer.GetLineStartPosition(-1);
			if (prevLineStart == null)
				prevLineStart = pointer.GetLineStartPosition(0);
			int result = Helper.GetOffset(Editor.Document.ContentStart, prevLineStart);
			return result;
		}
		public override int GetLineFinishIndexFromCharacterIndex(int index) {
			TextPointer pointer = GetPointerFromCharacterIndex(Editor.Document.ContentStart, index);
			if (pointer == null)
				return Text.Length;
			TextPointer nextLinePointer = pointer.GetLineStartPosition(1);
			if (nextLinePointer == null)
				return Text.Length;
			return Helper.GetOffset(Editor.Document.ContentStart, nextLinePointer.GetNextInsertionPosition(LogicalDirection.Backward));
		}
		public override Point GetPointFromCharacterIndex(int index) {
			TextPointer resultPointer = GetPointerFromCharacterIndex(Editor.Document.ContentStart, index);
			Helper.CachePosition(resultPointer, index, false);
			return resultPointer.GetCharacterRect(LogicalDirection.Forward).BottomLeft;
		}
		public override Point GetLineStartPositionFromCharacterIndex(int index) {
			TextPointer pointer = GetPointerFromCharacterIndex(Editor.Document.ContentStart, index);
			return pointer.GetLineStartPosition(0).GetCharacterRect(LogicalDirection.Forward).BottomLeft;
		}
		public override Point GetLineFinishPositionFromCharacterIndex(int index) {
			TextPointer pointer = GetPointerFromCharacterIndex(Editor.Document.ContentStart, index);
			return pointer.GetLineStartPosition(1).GetCharacterRect(LogicalDirection.Backward).BottomRight;
		}
		public override bool IsCharactersInSameLine(int firstCharIndex, int secondCharIndex) {
			TextPointer p1 = GetPointerFromCharacterIndex(Editor.Document.ContentStart, firstCharIndex);
			TextPointer p2 = GetPointerFromCharacterIndex(Editor.Document.ContentStart, secondCharIndex);
			return p1.GetLineStartPosition(0).CompareTo(p2.GetLineStartPosition(0)) == 0;
		}
		public override Position GetPositionFromPoint(Point point) {
			Helper.Invalidate(false);
			int offset = Editor.Document.ContentStart.GetOffsetToPosition(Editor.GetPositionFromPoint(point, true));
			return new IntPosition(ConvertToInnerPosition(offset));
		}
		public override int ConvertToInnerPosition(int position) {
			TextPointer pointer = Editor.Document.ContentStart.GetPositionAtOffset(position);
			if (pointer == null)
				pointer = Editor.Document.ContentEnd;
			Helper.Invalidate(false);
			return Helper.GetOffset(Editor.Document.ContentStart, pointer);
		}
		public override int GetLinesCount() {
			TextPointer pointer = Editor.Document.ContentStart.GetNextContextPosition(LogicalDirection.Forward);
			if (pointer == null)
				return 0;
			int result;
			pointer.GetLineStartPosition(int.MaxValue, out result);
			return result + 1;
		}
		public override bool IsTextStartPosition(int position) {
			if (position < 0)
				return false;
			TextPointer startPointer = Editor.Document.ContentStart.GetInsertionPosition(LogicalDirection.Forward);
			TextPointer currentPointer = Editor.Document.ContentStart.GetPositionAtOffset(position);
			return currentPointer.CompareTo(startPointer) <= 0;
		}
	}
	#endregion
	#region AsYouTypeRichTextBoxHelper
	class AsYouTypeRichTextBoxHelper : RichTextBoxHelper {
		PositionStack stack;
		internal AsYouTypeRichTextBoxHelper(RichTextBox richTextBox)
			: base(richTextBox) {
			this.stack = new PositionStack();
		}
		PositionStack Stack { get { return stack; } }
		public override TextPointer GetPointerFromCharacterIndex(TextPointer position, int index, LogicalDirection direction) {
			if (Stack.Count == 0)
				return base.GetPointerFromCharacterIndex(position, index, direction);
			RichPosition pos = Stack.Pop();
			return base.GetPointerFromCharacterIndex(pos.Pointer, index - pos.Position, direction);
		}
		internal override int GetOffset(TextPointer start, TextPointer finish) {
			int result = GetOffset(finish);
			return result >= 0 ? result : base.GetOffset(start, finish);
		}
		int GetOffset(TextPointer pointer) {
			if (Stack.Count > 0) {
				RichPosition pos = Stack.Pop();
				return base.GetOffset(pos.Pointer, pointer) + pos.Position;
			}
			return -1;
		}
		internal virtual void CachePosition(TextPointer pointer, int position, bool isFirstVisible) {
			Stack.Push(new RichPosition(pointer, position, isFirstVisible));
		}
		internal virtual void Invalidate(bool full) {
			if (full)
				Stack.Clear();
			else
				while (Stack.Count > 1)
					Stack.Pop();
		}
		class RichPosition {
			TextPointer pointer;
			int position;
			bool isFirstVisible;
			internal RichPosition(TextPointer pointer, int position, bool isFirstVisible) {
				this.pointer = pointer;
				this.position = position;
				this.isFirstVisible = isFirstVisible;
			}
			internal TextPointer Pointer { get { return pointer; } }
			internal int Position { get { return position; } }
			internal bool IsFirstVisible { get { return isFirstVisible; } }
		}
		class PositionStack : Stack<RichPosition> {
			internal new RichPosition Pop() {
				RichPosition pos = Peek();
				return pos.IsFirstVisible ? pos : base.Pop();
			}
		}
	}
	#endregion
	#region AsyncCheckingSpellCheckerState
	class AsyncCheckingSpellCheckerState : CheckingSpellCheckerState {
		public AsyncCheckingSpellCheckerState(AsyncSearchStrategy strategy)
			: base(strategy) {
		}
		new AsyncSearchStrategy SearchStrategy { get { return base.SearchStrategy as AsyncSearchStrategy; } }
		new IIgnoreList IgnoreList { get { return SearchStrategy.IgnoreList; } }
		protected override bool ShouldProcessWordWithoutChecking() {
			return IgnoreList.Contains(SearchStrategy.WordStartPosition, SearchStrategy.CurrentPosition, SearchStrategy.CheckedWord);
		}
		protected virtual bool ShouldStopChecking() {
			return false;
		}
		protected override StrategyState DoOperationCore() {
			if (ShouldStopChecking())
				return StrategyState.Stop;
			return base.DoOperationCore();
		}
	}
	#endregion
	#region AsyncSearchStrategy
	class AsyncSearchStrategy : AsyncSearchStrategyBase {
		public AsyncSearchStrategy(SpellCheckerBase spellChecker)
			: base(spellChecker) {
		}
		protected internal new OptionsSpelling OptionsSpelling { get { return base.OptionsSpelling as OptionsSpelling; } }
		internal new IIgnoreList IgnoreList { get { return SpellChecker.GetIgnoreListCore(EditControl); } }
		internal Control EditControl { get; set; }
		protected override SpellCheckerStateBase CreateAsyncCheckingState(StrategyState state) {
			return new AsyncCheckingSpellCheckerState(this);
		}
		protected override SpellCheckerRulesController CreateAsyncRulesController() {
			return new AsyncSpellCheckerRulesController(this, OptionsSpelling);
		}
		protected override OptionsSpellingBase CreateOptionsSpelling() {
			return new OptionsSpelling();
		}
		protected internal override void OnAfterCheck(StopCheckingReason reason) {
			RaiseAfterCheck();
			Reset();
		}
	}
	#endregion
	#region AsyncSpellCheckerRulesController
	class AsyncSpellCheckerRulesController : SpellCheckerRulesController {
		public AsyncSpellCheckerRulesController(AsyncSearchStrategy strategy, OptionsSpellingBase optionsSpelling)
			: base(strategy, optionsSpelling) {
		}
		protected override SpellCheckerErrorClassManager CreateErrorClassManager() {
			return new AsyncSpellCheckerErrorClassManager();
		}
		protected override void OnSpellCheckerOptionsChanged(object sender, EventArgs e) {
			lock (this) {
				base.OnSpellCheckerOptionsChanged(sender, e);
			}
		}
	}
	#endregion
	#region CheckAsYouTypeAdorner
	class CheckAsYouTypeAdorner : Adorner {
		#region Fields
		const double WavyLineHeight = 3.0;
		TranslateTransform transform;
		RectangleGeometry clipping;
		SolidColorBrush lineBrush;
		Pen pen;
		DrawingGroup wavyLineDrawing;
		#endregion
		public CheckAsYouTypeAdorner(Control editor)
			: base(editor) {
			this.transform = new TranslateTransform(0, 0);
			this.clipping = new RectangleGeometry();
			ErrorBoxes = new List<ErrorBox>();
			this.lineBrush = new SolidColorBrush();
			this.pen = new Pen(this.lineBrush, 1);
			this.wavyLineDrawing = CreateWavyLineDrawing();
		}
		#region Properties
		internal TranslateTransform Transform { get { return transform; } }
		internal RectangleGeometry Clipping { get { return clipping; } }
		internal List<ErrorBox> ErrorBoxes { get; set; }
		SolidColorBrush LineBrush { get { return lineBrush; } }
		Pen Pen { get { return pen; } }
		DrawingGroup WavyLineDrawing { get { return wavyLineDrawing; } }
		#endregion
		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			if (ErrorBoxes == null)
				return;
			SetDefaultTransformAndClipping(drawingContext);
			DrawErrorBoxes(drawingContext);
		}
		void DrawErrorBoxes(DrawingContext drawingContext) {
			LineBrush.Color = SpellingSettings.GetUnderlineColor(AdornedElement);
			UnderlineStyle underlineStyle = SpellingSettings.GetUnderlineStyle(AdornedElement);
			int errorsCount = ErrorBoxes.Count;
			for (int i = 0; i < errorsCount; i++) {
				Point start = ErrorBoxes[i].Start;
				Point finish = ErrorBoxes[i].Finish;
				if (underlineStyle == UnderlineStyle.WavyLine)
					DrawWavyLine(drawingContext, start, finish);
				else if (underlineStyle == UnderlineStyle.Line)
					drawingContext.DrawLine(Pen, start, finish);
				else
					DrawPoints(drawingContext, start, finish);
			}
		}
		void DrawPoints(DrawingContext drawingContext, Point from, Point to) {
			Pen p = new System.Windows.Media.Pen(LineBrush, 1);
			p.DashStyle = new DashStyle(new double[] { 1, 3 }, 0);
			drawingContext.DrawLine(p, from, to);
		}
		void DrawWavyLine(DrawingContext drawingContext, Point from, Point to) {
			DrawingBrush brush = new DrawingBrush(WavyLineDrawing);
			brush.TileMode = TileMode.Tile;
			brush.ViewportUnits = BrushMappingMode.Absolute;
			brush.Viewport = new Rect(from.X, from.Y - 0.6, WavyLineHeight * 2.0, WavyLineHeight + 1.3);
			drawingContext.DrawRectangle(brush, null, new Rect(from.X, from.Y, to.X - from.X, WavyLineHeight));
		}
		void SetDefaultTransformAndClipping(DrawingContext drawingContext) {
			Transform.X = 0;
			Transform.Y = 0;
			Control control = (Control)AdornedElement;
			double height = control.ActualHeight == 0 ? 0 : control.ActualHeight - control.Padding.Bottom - control.Padding.Top;
			double width = control.ActualWidth == 0 ? 0 : control.ActualWidth - control.Padding.Right - control.Padding.Left;
			Clipping.Rect = new Rect(control.Padding.Left, control.Padding.Top, width, height);
			Clipping.Transform = (Transform)Transform.Inverse;
			drawingContext.PushTransform(Transform);
			drawingContext.PushClip(Clipping);
		}
		DrawingGroup CreateWavyLineDrawing() {
			DrawingGroup result = new DrawingGroup();
			GeometryDrawing firstLine = new GeometryDrawing(null, Pen, new LineGeometry(new Point(0, 0), new Point(WavyLineHeight, WavyLineHeight)));
			GeometryDrawing secondLine = new GeometryDrawing(null, Pen, new LineGeometry(new Point(WavyLineHeight, WavyLineHeight), new Point(WavyLineHeight * 2, 0)));
			result.Children.Add(firstLine);
			result.Children.Add(secondLine);
			return result;
		}
	}
	#endregion
	#region ErrorBox
	class ErrorBox : IEquatable<ErrorBox> {
		Point start;
		Point finish;
		public ErrorBox(Point start, Point finish) {
			this.start = start;
			this.finish = finish;
		}
		public Point Start { get { return start; } }
		public Point Finish { get { return finish; } }
		public bool Equals(ErrorBox value) {
			return LayoutDoubleHelper.AreClose(Start, value.Start) && LayoutDoubleHelper.AreClose(Finish, value.Finish);
		}
	}
	#endregion
	#region EditorEventsManager
	class EditorEventsManager : IDisposable {
		#region Fields
		internal const int ScrollingInterval = 250;
		internal const int ResizingInterval = 150;
		readonly Control editor;
		DispatcherTimer scrollingTimer;
		DispatcherTimer resizingTimer;
		int resizeCounter = 0;
		bool isResizing;
		int updateCount = 0;
		List<TextChange> cachedChanges;
		#endregion
		public EditorEventsManager(Control editor) {
			this.editor = editor;
			this.scrollingTimer = new DispatcherTimer();
			this.scrollingTimer.Interval = new TimeSpan(0, 0, 0, 0, ScrollingInterval);
			this.resizingTimer = new DispatcherTimer();
			this.resizingTimer.Interval = new TimeSpan(0, 0, 0, 0, ResizingInterval);
			this.cachedChanges = new List<TextChange>();
			SubscribeToEvents();
		}
		#region Properties
		protected Control Editor { get { return editor; } }
		DispatcherTimer ScrollingTimer { get { return scrollingTimer; } }
		DispatcherTimer ResizingTimer { get { return resizingTimer; } }
		bool IsResizing { get { return isResizing; } set { isResizing = value; } }
		internal List<TextChange> CachedChanges { get { return cachedChanges; } }
		#endregion
		#region Events
		#region Scrolling
		ScrollChangedEventHandler scrolling;
		public event ScrollChangedEventHandler Scrolling { add { scrolling += value; } remove { scrolling -= value; } }
		protected virtual void RaiseScrolling(ScrollChangedEventArgs e) {
			if (scrolling != null && IsInLogicalTree())
				scrolling(this, e);
		}
		#endregion
		#region Scrolled
		EventHandler scrolled;
		public event EventHandler Scrolled { add { scrolled += value; } remove { scrolled -= value; } }
		protected virtual void RaiseScrolled() {
			if (scrolled != null && IsInLogicalTree())
				scrolled(this, EventArgs.Empty);
		}
		#endregion
		#region BeginResize
		EventHandler beginResize;
		public event EventHandler BeginResize { add { beginResize += value; } remove { beginResize -= value; } }
		protected virtual void RaiseBeginResize() {
			if (beginResize != null && IsInLogicalTree())
				beginResize(this, EventArgs.Empty);
		}
		#endregion
		#region EndResize
		EventHandler endResize;
		public event EventHandler EndResize { add { endResize += value; } remove { endResize -= value; } }
		protected virtual void RaiseEndResize() {
			if (endResize != null && IsInLogicalTree())
				endResize(this, EventArgs.Empty);
		}
		#endregion
		#region TextChanged
		EditorTextChangedEventHandler textChanged;
		public event EditorTextChangedEventHandler TextChanged { add { textChanged += value; } remove { textChanged -= value; } }
		protected virtual void RaiseTextChanged(System.Windows.Controls.TextChangedEventArgs e) {
			if (textChanged != null && IsInLogicalTree()) {
				CachedChanges.AddRange(e.Changes);
				EditorTextChangedEventArgs args = new EditorTextChangedEventArgs(CachedChanges);
				textChanged(this, args);
			}
		}
		#endregion
		#region FontChanged
		EventHandler fontChanged;
		public event EventHandler FontChanged { add { fontChanged += value; } remove { fontChanged -= value; } }
		protected virtual void RaiseFontChanged() {
			if (fontChanged != null && IsInLogicalTree())
				fontChanged(this, EventArgs.Empty);
		}
		#endregion
		#region CultureChanged
		EventHandler cultureChanged;
		public event EventHandler CultureChanged { add { cultureChanged += value; } remove { cultureChanged -= value; } }
		protected virtual void RaiseCultureChanged() {
			if (cultureChanged != null && IsInLogicalTree())
				cultureChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ShowSpellCheckMenuChanged
		EventHandler showSpellCheckMenuChanged;
		public event EventHandler ShowSpellCheckMenuChanged { add { showSpellCheckMenuChanged += value; } remove { showSpellCheckMenuChanged -= value; } }
		protected virtual void RaiseShowSpellCheckMenuChanged() {
			if (showSpellCheckMenuChanged != null && IsInLogicalTree())
				showSpellCheckMenuChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ContextMenuChanged
		EventHandler contextMenuChanged;
		public event EventHandler ContextMenuChanged { add { contextMenuChanged += value; } remove { contextMenuChanged -= value; } }
		protected virtual void RaiseContextMenuChanged() {
			if (contextMenuChanged != null && IsInLogicalTree())
				contextMenuChanged(this, EventArgs.Empty);
		}
		#endregion
		#region FirstVisiblePositionChanged
		EventHandler firstVisiblePositionChanged;
		public event EventHandler FirstVisiblePositionChanged { add { firstVisiblePositionChanged += value; } remove { firstVisiblePositionChanged -= value; } }
		protected virtual void RaiseFirstVisiblePositionChanged() {
			if (firstVisiblePositionChanged != null && IsInLogicalTree())
				firstVisiblePositionChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		void SubscribeToEvents() {
			Editor.SizeChanged += OnResizing;
			ScrollingTimer.Tick += OnScrollingTimeElapsed;
			ResizingTimer.Tick += OnResizingTimeElapsed;
			Editor.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnScrollChanging));
			Editor.AddHandler(TextBoxBase.TextChangedEvent, new System.Windows.Controls.TextChangedEventHandler(OnTextChanged));
			DependencyPropertyChangeHandler.AddHandler(Editor, "FontFamily", OnFontChanging);
			DependencyPropertyChangeHandler.AddHandler(Editor, "FontSize", OnFontChanging);
			DependencyPropertyChangeHandler.AddHandler(Editor, "FontStretch", OnFontChanging);
			DependencyPropertyChangeHandler.AddHandler(Editor, "FontWeight", OnFontChanging);
			DependencyPropertyChangeHandler.AddHandler(Editor, "FontStyle", OnFontChanging);
			DependencyPropertyChangeHandler.AddHandler(Editor, "ContextMenu", OnContextMenuChanged);
			DependencyPropertyChangeHandler.AddHandler(Editor, SpellingSettings.ShowSpellCheckMenuProperty, OnShowSpellCheckMenuChanged);
			DependencyPropertyChangeHandler.AddHandler(Editor, SpellingSettings.CultureProperty, OnCultureChanged);
			DependencyPropertyChangeHandler.AddHandler(Editor, BarManager.DXContextMenuProperty, OnContextMenuChanged);
		}
		void UnsubscrideFromEvents() {
			if (ScrollingTimer != null)
				ScrollingTimer.Tick -= OnScrollingTimeElapsed;
			if (ResizingTimer != null)
				ResizingTimer.Tick -= OnResizingTimeElapsed;
			Editor.SizeChanged -= OnResizing;
			Editor.RemoveHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnScrollChanging));
			Editor.RemoveHandler(TextBoxBase.TextChangedEvent, new System.Windows.Controls.TextChangedEventHandler(OnTextChanged));
			DependencyPropertyChangeHandler.RemoveHandler(Editor, "FontFamily");
			DependencyPropertyChangeHandler.RemoveHandler(Editor, "FontSize");
			DependencyPropertyChangeHandler.RemoveHandler(Editor, "FontStretch");
			DependencyPropertyChangeHandler.RemoveHandler(Editor, "FontWeight");
			DependencyPropertyChangeHandler.RemoveHandler(Editor, "FontStyle");
			DependencyPropertyChangeHandler.RemoveHandler(Editor, "ContextMenu");
			DependencyPropertyChangeHandler.RemoveHandler(Editor, SpellingSettings.ShowSpellCheckMenuProperty);
			DependencyPropertyChangeHandler.RemoveHandler(Editor, SpellingSettings.CultureProperty);
			DependencyPropertyChangeHandler.RemoveHandler(Editor, BarManager.DXContextMenuProperty);
		}
		bool IsInLogicalTree() {
			return Editor.Parent != null;
		}
		internal void BeginUpdateMenu() {
			this.updateCount++;
		}
		internal void EndUpdateMenu() {
			this.updateCount--;
		}
		void OnContextMenuChanged() {
			if (this.updateCount == 0)
				RaiseContextMenuChanged();
		}
		void OnTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
			RaiseTextChanged(e);
		}
		void OnCultureChanged() {
			RaiseCultureChanged();
		}
		void OnShowSpellCheckMenuChanged() {
			RaiseShowSpellCheckMenuChanged();
		}
		void OnFontChanging() {
			RaiseFirstVisiblePositionChanged();
			RaiseFontChanged();
		}
		void OnResizingTimeElapsed(object sender, EventArgs e) {
			ResizingTimer.Stop();
			this.resizeCounter = 0;
			RaiseFirstVisiblePositionChanged();
			RaiseEndResize();
			IsResizing = false;
		}
		void OnResizing(object sender, SizeChangedEventArgs e) {
			IsResizing = true;
			if (ResizingTimer.IsEnabled)
				ResizingTimer.Stop();
			this.resizeCounter++;
			if (resizeCounter == 1)
				RaiseBeginResize();
			ResizingTimer.Start();
		}
		void OnScrollChanging(object sender, ScrollChangedEventArgs e) {
			if (IsResizing || (e.HorizontalChange == 0 && e.VerticalChange == 0))
				return;
			if (ScrollingTimer.IsEnabled)
				ScrollingTimer.Stop();
			RaiseScrolling(e);
			ScrollingTimer.Start();
		}
		void OnScrollingTimeElapsed(object sender, EventArgs e) {
			ScrollingTimer.Stop();
			RaiseFirstVisiblePositionChanged();
			RaiseScrolled();
		}
		public void Dispose() {
			UnsubscrideFromEvents();
			this.scrollingTimer = null;
			this.resizingTimer = null;
		}
	}
	#endregion
	#region ErrorBoxesIterator
	class ErrorBoxesIterator {
		List<ErrorBox> items;
		int currentIndex;
		public ErrorBoxesIterator(List<ErrorBox> items) {
			this.items = items;
		}
		List<ErrorBox> Items { get { return items; } }
		public int CurrentIndex { get { return currentIndex; } }
		public ErrorBox CurrentItem { get { return CurrentIndex < Items.Count ? Items[CurrentIndex] : null; } }
		public void MoveToEnd() {
			this.currentIndex = Items.Count;
		}
		public bool MoveNext() {
			if (this.currentIndex >= Items.Count)
				return false;
			this.currentIndex++;
			return true;
		}
		public void Reset() {
			this.currentIndex = 0;
		}
	}
	#endregion
	#region ErrorBoxesCalculator
	class ErrorBoxesCalculator {
		#region Fields
		const int BoxesCalculationInterval = 30;
		IEditControlAdapter adapter;
		List<ErrorBox> cachedBoxes;
		List<ErrorBox> currentBoxes;
		CancellationTokenSource tokenSource;
		int cachedLinesCount;
		string processingText;
		#endregion
		public ErrorBoxesCalculator(IEditControlAdapter adapter) {
			this.adapter = adapter;
			this.currentBoxes = new List<ErrorBox>();
		}
		#region Properties
		IEditControlAdapter Adapter { get { return adapter; } }
		internal List<ErrorBox> CachedBoxes { get { return cachedBoxes; } private set { cachedBoxes = value; } }
		internal List<ErrorBox> CurrentBoxes { get { return currentBoxes; } }
		protected virtual DispatcherPriority CalculationPriority { get { return DispatcherPriority.ApplicationIdle; } }
		CancellationTokenSource TokenSource { get { return tokenSource; } set { tokenSource = value; } }
		CancellationToken Token { get { return TokenSource.Token; } }
		int CachedLinesCount { get { return cachedLinesCount; } set { cachedLinesCount = value; } }
		string ProcessingText { get { return processingText; } set { processingText = value; } }
		#endregion
		public static int CompareCoordinates(double val1, double val2) {
			double v1 = Math.Round(val1, MidpointRounding.AwayFromZero);
			double v2 = Math.Round(val2, MidpointRounding.AwayFromZero);
			if (v1 > v2)
				return 1;
			if (v1 < v2)
				return -1;
			return 0;
		}
		#region CalculateErrorBoxes
		public void CalculateErrorBoxes(SpellCheckErrorBase[] errors, int changingStart, int changingFinish, Action<List<ErrorBox>> callback) {
			CurrentBoxes.Clear();
			if (TokenSource != null)
				TokenSource.Cancel();
			if (CachedBoxes != null && CachedBoxes.Count > 0) {
				ErrorBoxesIterator iterator = new ErrorBoxesIterator(CachedBoxes);
				CalculateBoxesBeforeChangedText(errors, changingStart, iterator);
				CalculateBoxesAtChangedText(errors, changingFinish, iterator);
				CalculateBoxesAfterChangedText(errors, iterator);
				SaveCurrentBoxesToCache(errors.Length);
				callback(CurrentBoxes);
			}
			else
				RecalculateAllBoxes(errors, callback);
			CachedLinesCount = Adapter.GetLinesCount();
		}
		#endregion
		#region RecalculateAllBoxes
		void RecalculateAllBoxes(SpellCheckErrorBase[] errors, Action<List<ErrorBox>> callback) {
			ProcessingText = Adapter.Text;
			Adapter.Dispatcher.BeginInvoke(new Action(() => {
				TokenSource = new CancellationTokenSource();
				RecalculateAllBoxesCore(errors, 0, callback);
			}), CalculationPriority);
		}
		#endregion
		#region RecalculateAllBoxesCore
		void RecalculateAllBoxesCore(SpellCheckErrorBase[] errors, int index, Action<List<ErrorBox>> callback) {
			if (!ProcessingText.Equals(Adapter.Text, StringComparison.Ordinal))
				return;
			int i = index;
			System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
			while (watch.ElapsedMilliseconds < BoxesCalculationInterval && i < errors.Length) {
				if (Token.IsCancellationRequested)
					return;
				int start = errors[i].StartPosition.ToInt();
				int finish = errors[i].FinishPosition.ToInt();
				ErrorBox[] boxes = GetErrorBoxesFromCharactersPositions(start, finish);
				if (boxes != null)
					CurrentBoxes.AddRange(boxes);
				i++;
			}
			watch.Stop();
			callback(CurrentBoxes);
			if (i < errors.Length)
				Adapter.Dispatcher.BeginInvoke(new Action(() => { RecalculateAllBoxesCore(errors, i, callback); }), CalculationPriority);
			else
				SaveCurrentBoxesToCache(errors.Length);
		}
		#endregion
		#region CalculateBoxesBeforeChangedText
		protected virtual void CalculateBoxesBeforeChangedText(SpellCheckErrorBase[] errors, int changingStart, ErrorBoxesIterator iterator) {
			int prevLineStartIndex = Adapter.GetPrevLineStartIndexFromCharacterIndex(changingStart);
			for (int i = 0; i < errors.Length; i++) {
				int finish = errors[i].FinishPosition.ToInt();
				if (prevLineStartIndex <= finish)
					return;
				CurrentBoxes.Add(iterator.CurrentItem);
				iterator.MoveNext();
			}
		}
		#endregion
		#region CalculateBoxesAtChangedText
		protected virtual void CalculateBoxesAtChangedText(SpellCheckErrorBase[] errors, int changingFinish, ErrorBoxesIterator iterator) {
			int currentLineFinishIndex = Adapter.GetLineFinishIndexFromCharacterIndex(changingFinish);
			int currentLinesCount = Adapter.GetLinesCount();
			for (int i = iterator.CurrentIndex; i < errors.Length; i++) {
				int start = errors[i].StartPosition.ToInt();
				int finish = errors[i].FinishPosition.ToInt();
				ErrorBox[] boxes = GetErrorBoxesFromCharactersPositions(start, finish);
				if (boxes == null)
					continue;
				if (start <= currentLineFinishIndex)
					CurrentBoxes.AddRange(boxes);
				else {
					for (int j = 0; j < boxes.Length; j++) {
						if (CachedLinesCount != 0 && CachedLinesCount == currentLinesCount && BoxContainsInCache(boxes[j], iterator))
							return;
						CurrentBoxes.Add(boxes[j]);
					}
				}
			}
			iterator.MoveToEnd();
		}
		#endregion
		#region CalculateBoxesAfterChangedText
		protected virtual void CalculateBoxesAfterChangedText(SpellCheckErrorBase[] errors, ErrorBoxesIterator iterator) {
			if (iterator.CurrentIndex == CachedBoxes.Count || errors.Length == 0)
				return;
			CurrentBoxes.AddRange(CachedBoxes.GetRange(iterator.CurrentIndex, CachedBoxes.Count - iterator.CurrentIndex));
		}
		#endregion
		#region BoxContainsInCache
		bool BoxContainsInCache(ErrorBox box, ErrorBoxesIterator iterator) {
			iterator.Reset();
			do {
				if (iterator.CurrentItem != null && iterator.CurrentItem.Equals(box))
					return true;
			} while (iterator.MoveNext());
			return false;
		}
		#endregion
		#region GetErrorBoxesFromCharactersPositions
		ErrorBox[] GetErrorBoxesFromCharactersPositions(int startPosition, int finishPosition) {
			Point start = Adapter.GetPointFromCharacterIndex(startPosition);
			Point finish = Adapter.GetPointFromCharacterIndex(finishPosition);
			if (start.X < 0)
				start.X = ((EditControlAdapterBase)Adapter).Editor.Padding.Left;
			bool areSameYCoordinates = CompareCoordinates(start.Y, finish.Y) == 0;
			if (finish.X < 0 || (finish.X < start.X && areSameYCoordinates))
				return null;
			if (areSameYCoordinates || Adapter.IsCharactersInSameLine(startPosition, finishPosition))
				return new ErrorBox[] { new ErrorBox(start, finish) };
			Point firstBoxStart = start;
			Point firstBoxFinish = Adapter.GetLineFinishPositionFromCharacterIndex(startPosition);
			Point secondBoxStart = Adapter.GetLineStartPositionFromCharacterIndex(finishPosition);
			Point secondBoxFinish = finish;
			return new ErrorBox[] { new ErrorBox(firstBoxStart, firstBoxFinish), new ErrorBox(secondBoxStart, secondBoxFinish) };
		}
		#endregion
		#region SaveCurrentBoxesToCache
		void SaveCurrentBoxesToCache(int errorsCount) {
			if (CurrentBoxes.Count >= errorsCount)
				CachedBoxes = new List<ErrorBox>(CurrentBoxes);
		}
		#endregion
	}
	#endregion
	#region CheckAsYouTypeBehavior
	abstract class CheckAsYouTypeBehavior : Behavior<Control> {
		#region Fields
		internal const int CheckDelay = 30;
		EditorEventsManager eventsManager;
		CheckAsYouTypeAdorner adorner;
		IEditControlAdapter adapter;
		ContextMenuManagerBase menuManager;
		AsyncSearchStrategy searchStrategy;
		CheckAsYouTypeOperationManager operationManager;
		DispatcherTimer checkingTimer;
		ErrorBoxesCalculator calculator;
		int calculateBoxesFrom;
		int calculateBoxesTo;
		DispatcherPriority delegatesPriorityForProcessing;
		bool isAdornerExists;
		SpellChecker spellChecker;
		bool isInternalSpellCheckerUsed;
		#endregion
		#region Properties
		internal Control Editor { get { return AssociatedObject; } }
		internal EditorEventsManager EventsManager { get { return eventsManager; } }
		internal SpellChecker SpellChecker { get { return spellChecker; } }
		internal IEditControlAdapter Adapter { get { return adapter; } }
		internal CheckAsYouTypeAdorner Adorner { get { return adorner; } }
		OptionsSpelling Options { get { return SpellChecker.GetSpellCheckerOptions(Editor); } }
		internal AsyncSearchStrategy SearchStrategy {
			get { return searchStrategy; }
			private set {
				if (searchStrategy == value)
					return;
				searchStrategy = value;
				OnSearchStrategyChanged();
			}
		}
		protected bool IsActivated { get; set; }
		internal CheckAsYouTypeOperationManager OperationManager { get { return operationManager; } }
		internal ContextMenuManagerBase MenuManager { get { return menuManager; } }
		DispatcherTimer CheckingTimer { get { return checkingTimer; } }
		protected virtual DispatcherPriority CheckingPriority { get { return DispatcherPriority.ApplicationIdle; } }
		protected ErrorBoxesCalculator Calculator { get { return calculator; } }
		bool IsAdornerExists { get { return isAdornerExists; } set { isAdornerExists = value; } }
		internal bool IsInternalSpellCheckerUsed { get { return isInternalSpellCheckerUsed; } private set { isInternalSpellCheckerUsed = value; } }
		bool CanPerformAsyncOperations { get { return IsActivated && IsAdornerExists; } }
		#endregion
		protected abstract IEditControlAdapter CreateAdapter();
		void OnSearchStrategyChanged() {
			this.operationManager = new CheckAsYouTypeOperationManager(this);
		}
		#region SubscribeToEvents
		void SubscribeToEvents() {
			EventsManager.Scrolling += OnScrolling;
			EventsManager.EndResize += OnEndResize;
			EventsManager.Scrolled += OnScrolled;
			EventsManager.BeginResize += OnBeginResize;
			EventsManager.TextChanged += OnTextChanged;
			EventsManager.FontChanged += OnFontChanged;
			EventsManager.ShowSpellCheckMenuChanged += OnShowSpellCheckMenuChanged;
			EventsManager.CultureChanged += OnCultureChanged;
			EventsManager.ContextMenuChanged += OnContextMenuChanged;
			EventsManager.FirstVisiblePositionChanged += OnFirstVisiblePositionChanged;
			Options.OptionChanged += OnOptionChanged;
			CheckingTimer.Tick += OnCheckingTimerTick;
			SubscribeToSpellCheckerEvents();
		}
		#endregion
		#region UnsubscribeFromEvents
		void UnsubscribeFromEvents() {
			EventsManager.Scrolling -= OnScrolling;
			EventsManager.EndResize -= OnEndResize;
			EventsManager.Scrolled -= OnScrolled;
			EventsManager.TextChanged -= OnTextChanged;
			EventsManager.BeginResize -= OnBeginResize;
			EventsManager.FontChanged -= OnFontChanged;
			EventsManager.ShowSpellCheckMenuChanged -= OnShowSpellCheckMenuChanged;
			EventsManager.CultureChanged -= OnCultureChanged;
			EventsManager.ContextMenuChanged -= OnContextMenuChanged;
			EventsManager.FirstVisiblePositionChanged -= OnFirstVisiblePositionChanged;
			Options.OptionChanged -= OnOptionChanged;
			CheckingTimer.Tick -= OnCheckingTimerTick;
		}
		#endregion
		#region EventsHandlers
		internal void OnDictionarySourceCollectionChanged(DictionarySourceCollection oldCollection, DictionarySourceCollection newCollection) {
			if (oldCollection != null)
				oldCollection.CollectionChanged -= OnDictionarySourceCollectionItemsChanged;
			if (newCollection != null)
				newCollection.CollectionChanged += OnDictionarySourceCollectionItemsChanged;
			if (SpellChecker == null)
				return;
			if (oldCollection != null)
				DictionarySourceHelper.RemoveDictionaries(SpellChecker, oldCollection);
			if (newCollection != null)
				SpellChecker.Dictionaries.AddRange(DictionarySourceHelper.GetDictionaries(newCollection));
			Check();
		}
		void OnDictionarySourceCollectionItemsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			switch (e.Action) {
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					SpellChecker.Dictionaries.AddRange(DictionarySourceHelper.GetDictionaries(e.NewItems));
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					DictionarySourceHelper.RemoveDictionaries(SpellChecker, e.OldItems);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
					DictionarySourceHelper.RemoveDictionaries(SpellChecker, e.OldItems);
					SpellChecker.Dictionaries.AddRange(DictionarySourceHelper.GetDictionaries(e.NewItems));
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
					DictionarySourceCollection sourceCollection = (DictionarySourceCollection)sender;
					OnDictionarySourceCollectionChanged(sourceCollection, sourceCollection);
					break;
				default:
					break;
			}
			Check();
		}
		void OnFirstVisiblePositionChanged(object sender, EventArgs e) {
			CalculateFirstVisiblePosition();
		}
		void OnContextMenuChanged(object sender, EventArgs e) {
			if (!SpellingSettings.GetShowSpellCheckMenu(Editor))
				return;
			EventsManager.BeginUpdateMenu();
			if (Editor.ContextMenu != null && MenuManager is DXContextMenuManager) {
				MenuManager.Dispose();
				this.menuManager = CreateMenuManager();
			}
			if (Editor.ContextMenu == null && MenuManager is NativeContextMenuManager) {
				MenuManager.Dispose();
				this.menuManager = CreateMenuManager();
			}
			EventsManager.EndUpdateMenu();
		}
		void OnCheckingTimerTick(object sender, EventArgs e) {
			CheckingTimer.Stop();
			CheckCore();
		}
		void OnCultureChanged(object sender, EventArgs e) {
			Check();
		}
		void OnShowSpellCheckMenuChanged(object sender, EventArgs e) {
			if (SpellingSettings.GetShowSpellCheckMenu(Editor))
				MenuManager.Activate();
			else
				MenuManager.Deactivate();
		}
		void OnSpellCheckModeChanged(object sender, EventArgs e) {
			if (SpellChecker.SpellCheckMode == SpellCheckMode.OnDemand)
				Deactivate();
			else
				Activate();
		}
		void OnCustomDictionaryChanged(object sender, EventArgs e) {
			Check();
		}
		void OnOptionChanged(object sender, EventArgs e) {
			Check();
		}
		void OnBeginResize(object sender, EventArgs e) {
			RemoveAdorner();
		}
		void OnTextChanged(object sender, EditorTextChangedEventArgs e) {
			int textLength = Adapter.GetOriginalText().Length;
			int resultStart = e.Changes.Count > 0 ? int.MaxValue : 0;
			int resultFinish = e.Changes.Count > 0 ? 0 : textLength;
			foreach (TextChange change in e.Changes) {
				int start = change.AddedLength == 0 ? change.Offset - change.RemovedLength : change.Offset;
				if (start < resultStart)
					resultStart = start;
				int finish = change.AddedLength == 0 ? change.Offset : change.Offset + change.AddedLength;
				if (resultFinish < finish)
					resultFinish = finish;
			}
			resultFinish = Math.Min(resultFinish, textLength);
			resultStart = Math.Max(0, resultStart);
			Check(resultStart, resultFinish);
		}
		void OnEndResize(object sender, EventArgs e) {
			AddAdorner();
			Check(DispatcherPriority.Loaded);
		}
		void OnScrolling(object sender, ScrollChangedEventArgs e) {
			Adorner.Transform.X -= e.HorizontalChange;
			Adorner.Transform.Y -= e.VerticalChange;
			Adorner.Clipping.Transform = (Transform)Adorner.Transform.Inverse;
		}
		void OnScrolled(object sender, EventArgs e) {
			Check();
		}
		void OnFontChanged(object sender, EventArgs e) {
			Check();
		}
		internal void OnSpellCheckerChanged(SpellChecker oldSpellChecker, SpellChecker newSpellChecker) {
			if (this.spellChecker != null) {
				UnsubscribeFromSpellCheckerEvents();
				DisposeSearchStrategy();
			}
			if (newSpellChecker == null) {
				Deactivate();
				return;
			}
			this.spellChecker = newSpellChecker;
			IsInternalSpellCheckerUsed = false;
			SubscribeToSpellCheckerEvents();
			if (!Editor.IsLoaded)
				return;
			if (!IsActivated)
				Activate();
			else {
				SearchStrategy = CreateSearchStrategy();
				Check();
			}
		}
		void SubscribeToSpellCheckerEvents() {
			UnsubscribeFromSpellCheckerEvents();
			((ISpellChecker)this.spellChecker).CustomDictionaryChanged += OnCustomDictionaryChanged;
			this.spellChecker.SpellCheckModeChanged += OnSpellCheckModeChanged;
			this.spellChecker.CultureChanged += OnCultureChanged;
		}
		void UnsubscribeFromSpellCheckerEvents() {
			((ISpellChecker)this.spellChecker).CustomDictionaryChanged -= OnCustomDictionaryChanged;
			this.spellChecker.SpellCheckModeChanged -= OnSpellCheckModeChanged;
			this.spellChecker.CultureChanged -= OnCultureChanged;
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			Deactivate();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			Activate();
		}
		#endregion
		#region OnAttached
		protected override void OnAttached() {
			base.OnAttached();
			OnAttachedCore();
		}
		protected virtual void OnAttachedCore() {
			Editor.Loaded += OnLoaded;
			Editor.Unloaded += OnUnloaded;
			if (CanActivate())
				Activate();
		}
		protected virtual bool CanActivate() {
			return !IsActivated && Editor.IsLoaded;
		}
		#endregion
		#region OnDetaching
		protected override void OnDetaching() {
			base.OnDetaching();
			OnDetachingCore();
		}
		protected virtual void OnDetachingCore() {
			Deactivate();
			Editor.Loaded -= OnLoaded;
			Editor.Unloaded -= OnUnloaded;
		}
		#endregion
		#region Activate
		protected virtual void Activate() {
			if (!CanActivate())
				return;
			this.spellChecker = SpellingSettings.GetSpellChecker(Editor);
			IsInternalSpellCheckerUsed = false;
			DictionarySourceCollection dictionarySourceCollection = SpellingSettings.GetDictionarySourceCollection(Editor);
			if (SpellChecker == null) {
				this.spellChecker = new SpellChecker();
				this.spellChecker.SpellCheckMode = SpellCheckMode.AsYouType;
				this.spellChecker.LoadOnDemand = true;
				if (dictionarySourceCollection != null && dictionarySourceCollection.Count == 1 && dictionarySourceCollection[0].Culture != null)
					spellChecker.Culture = dictionarySourceCollection[0].Culture;
				IsInternalSpellCheckerUsed = true;
			}
			if (dictionarySourceCollection != null)
				SpellChecker.Dictionaries.AddRange(DictionarySourceHelper.GetDictionaries(dictionarySourceCollection));
			if (SpellChecker == null)
				return;
			this.eventsManager = new EditorEventsManager(Editor);
			this.adapter = CreateAdapter();
			this.checkingTimer = new DispatcherTimer(CheckingPriority);
			this.checkingTimer.Interval = new TimeSpan(0, 0, 0, 0, CheckDelay);
			AddAdorner();
			SearchStrategy = CreateSearchStrategy();
			this.menuManager = CreateMenuManager();
			this.calculator = CreateErrorBoxesCalculator();
			SubscribeToEvents();
			IsActivated = true;
			Check();
		}
		#endregion
		#region Deactivate
		protected virtual void Deactivate() {
			if (!IsActivated)
				return;
			UnsubscribeFromEvents();
			RemoveAdorner();
			if (this.menuManager != null) {
				this.menuManager.Dispose();
				this.menuManager = null;
			}
			if (this.eventsManager != null) {
				this.eventsManager.Dispose();
				this.eventsManager = null;
			}
			DisposeSearchStrategy();
			if (this.operationManager != null) {
				this.operationManager.Dispose();
				this.operationManager = null;
			}
			this.checkingTimer = null;
			this.calculator = null;
			DictionarySourceCollection sources = SpellingSettings.GetDictionarySourceCollection(Editor);
			if (sources != null && sources.Owner == Editor)
				DictionarySourceHelper.RemoveDictionaries(SpellChecker, sources);
			if (IsInternalSpellCheckerUsed) {
				this.spellChecker.Dispose();
				this.spellChecker = null;
			}
			this.adapter = null;
			IsActivated = false;
		}
		#endregion
		#region CreateSearchStrategy
		protected virtual AsyncSearchStrategy CreateSearchStrategy() {
			return new AsyncSearchStrategy(SpellChecker);
		}
		#endregion
		#region CreateErrorBoxesCalculator
		protected virtual ErrorBoxesCalculator CreateErrorBoxesCalculator() {
			return new ErrorBoxesCalculator(Adapter);
		}
		#endregion
		#region DisposeSearchStrategy
		protected virtual void DisposeSearchStrategy() {
			if (this.searchStrategy != null) {
				this.searchStrategy.Dispose();
				this.searchStrategy = null;
			}
		}
		#endregion
		#region CreateMenuManager
		protected virtual ContextMenuManagerBase CreateMenuManager() {
			if (Editor.ContextMenu != null)
				return new NativeContextMenuManager(this);
			else
				return new DXContextMenuManager(this);
		}
		#endregion
		#region AddAdorner
		protected virtual void AddAdorner() {
			this.adorner = new CheckAsYouTypeAdorner(Editor);
			Binding opacityBinding = new Binding("Opacity");
			opacityBinding.Source = Editor;
			opacityBinding.Mode = BindingMode.OneWay;
			Adorner.SetBinding(UIElement.OpacityProperty, opacityBinding);
			MultiBinding visibilityBinding = new MultiBinding();
			visibilityBinding.Bindings.Add(new Binding("IsVisible") { Source = Editor });
			visibilityBinding.Bindings.Add(new Binding("Visibility") { Source = Editor });
			visibilityBinding.Converter = new VisibilityConverter();
			visibilityBinding.Mode = BindingMode.OneWay;
			Adorner.SetBinding(UIElement.VisibilityProperty, visibilityBinding);
			AdornerLayer layer = AdornerLayer.GetAdornerLayer(Editor);
			if (layer != null) {
				layer.Add(Adorner);
				IsAdornerExists = true;
			}
		}
		#endregion
		#region RemoveAdorner
		protected virtual void RemoveAdorner() {
			if (Adorner == null)
				return;
			AdornerLayer layer = AdornerLayer.GetAdornerLayer(Editor);
			if (layer != null)
				layer.Remove(Adorner);
			this.adorner = null;
			IsAdornerExists = false;
		}
		#endregion
		#region Check
		protected virtual void Check(int changingStart, int changingFinish, DispatcherPriority priority) {
			if (!IsActivated)
				return;
			SearchStrategy.ClearErrorList();
			if (IsAdornerExists && String.IsNullOrEmpty(Adapter.GetOriginalText())) {
				Adorner.ErrorBoxes.Clear();
				Adorner.InvalidateVisual();
			}
			CheckingTimer.Stop();
			this.calculateBoxesFrom = changingStart;
			this.calculateBoxesTo = changingFinish;
			this.delegatesPriorityForProcessing = priority;
			CheckingTimer.Start();
		}
		internal void Check(int changingStart, int changingFinish) {
			Check(changingStart, changingFinish, DispatcherPriority.Background);
		}
		internal void Check(DispatcherPriority priority) {
			if (!IsActivated)
				return;
			if (Calculator.CachedBoxes != null)
				Calculator.CachedBoxes.Clear();
			Check(0, 0, priority);
		}
		internal void Check() {
			Check(DispatcherPriority.Background);
		}
		#endregion
		#region CheckCore
		void CheckCore() {
			EventsManager.CachedChanges.Clear();
			if (!CanPerformAsyncOperations || SpellChecker.SpellCheckMode == SpellCheckMode.OnDemand)
				return;
			SearchStrategy.Text = Adapter.Text;
			if (SearchStrategy.StartPosition == null || Adapter.IsTextStartPosition(this.calculateBoxesFrom))
				CalculateFirstVisiblePosition();
			SearchStrategy.FinishPosition = Adapter.GetLastVisibleCharacterPosition();
			if (SearchStrategy.StartPosition == null || SearchStrategy.FinishPosition == null) {
				Adorner.InvalidateVisual();
				return;
			}
			CultureInfo editorCulture = SpellingSettings.GetCulture(Editor);
			SearchStrategy.Culture = editorCulture != null ? editorCulture : SpellChecker.Culture;
			SearchStrategy.EditControl = Editor;
			lock (SpellChecker) {
				SpellChecker.LoadDictionaries(SearchStrategy.ActualCulture);
			}
			OptionsSpelling actualOptions = new OptionsSpelling();
			actualOptions.Assign(SpellChecker.OptionsSpelling);
			actualOptions.CombineOptions(SpellChecker.GetSpellCheckerOptions(Editor));
			SearchStrategy.OptionsSpelling.Assign(actualOptions);
			SearchStrategy.Check();
			CalculateErrorBoxes();
		}
		#endregion
		#region CalculateErrorBoxes
		protected virtual void CalculateErrorBoxes() {
			Calculator.CalculateErrorBoxes((SpellCheckErrorBase[])SearchStrategy.ErrorList.ToArray(typeof(SpellCheckErrorBase)), Adapter.ConvertToInnerPosition(this.calculateBoxesFrom), Adapter.ConvertToInnerPosition(this.calculateBoxesTo), DrawBoxes);
		}
		#endregion
		#region DrawBoxes
		protected virtual void DrawBoxes(List<ErrorBox> errors) {
			if (!CanPerformAsyncOperations)
				return;
			Adorner.ErrorBoxes = errors;
			Adorner.InvalidateVisual();
			EventsHelper.DoEvents(this.delegatesPriorityForProcessing);
		}
		#endregion
		#region CalculateFirstVisiblePosition
		protected virtual void CalculateFirstVisiblePosition() {
			SearchStrategy.StartPosition = Adapter.GetFirstVisibleCharacterPosition();
		}
		#endregion
	}
	#endregion
	#region TextBoxCheckAsYouTypeBehavior
	class TextBoxCheckAsYouTypeBehavior : CheckAsYouTypeBehavior {
		new TextBox Editor { get { return base.Editor as TextBox; } }
		protected override IEditControlAdapter CreateAdapter() {
			return new TextBoxAdapter(Editor);
		}
	}
	#endregion
	#region RichTextBoxCheckAsYouTypeBehavior
	class RichTextBoxCheckAsYouTypeBehavior : CheckAsYouTypeBehavior {
		new RichTextBox Editor { get { return base.Editor as RichTextBox; } }
		protected override IEditControlAdapter CreateAdapter() {
			return new RichTextBoxAdapter(Editor);
		}
	}
	#endregion
	#region TextEditCheckAsYouTypeBehavior
	class TextEditCheckAsYouTypeBehavior : CheckAsYouTypeBehavior {
		protected new TextEdit Editor { get { return base.Editor as TextEdit; } }
		protected override IEditControlAdapter CreateAdapter() {
			return new TextEditAdapter(Editor);
		}
		protected override void OnAttachedCore() {
			DependencyPropertyChangeHandler.AddHandler(Editor, TextEdit.IsEditorActiveProperty, OnInnerEditorStateChanged);
			base.OnAttachedCore();
		}
		protected override void OnDetachingCore() {
			DependencyPropertyChangeHandler.RemoveHandler(Editor, TextEdit.IsEditorActiveProperty); ;
			base.OnDetachingCore();
		}
		protected override bool CanActivate() {
			return base.CanActivate() && Editor.IsEditorActive && Editor.EditCore != null;
		}
		void OnInnerEditorStateChanged() {
			if (Editor.IsEditorActive)
				Activate();
			else
				Deactivate();
		}
	}
	#endregion
	#region EventsHelper
	static class EventsHelper {
		public static void DoEvents(DispatcherPriority priority) {
			DispatcherFrame frame = new DispatcherFrame();
			Dispatcher.CurrentDispatcher.BeginInvoke(priority, new DispatcherOperationCallback(ExitFrame), frame);
			if (!frame.Dispatcher.HasShutdownFinished)
				Dispatcher.PushFrame(frame);
		}
		public static void DoEvents() {
			DoEvents(DispatcherPriority.Background);
		}
		static object ExitFrame(object frame) {
			((DispatcherFrame)frame).Continue = false;
			return null;
		}
		public static void Wait(int milliseconds) {
			System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
			while (sw.ElapsedMilliseconds < milliseconds) {
				EventsHelper.DoEvents();
			}
			sw.Stop();
		}
	}
	#endregion
	#region BehaviorHelper
	static class BehaviorHelper {
		static Dictionary<Type, Type> registredTypes = new Dictionary<Type, Type>();
		static BehaviorHelper() {
			Register(typeof(TextBox), typeof(TextBoxCheckAsYouTypeBehavior));
			Register(typeof(RichTextBox), typeof(RichTextBoxCheckAsYouTypeBehavior));
			Register(typeof(TextEdit), typeof(TextEditCheckAsYouTypeBehavior));
		}
		#region Register
		static internal void Register(Type controlType, Type behaviorType) {
			if (IsRegistered(controlType))
				return;
			registredTypes.Add(controlType, behaviorType);
		}
		#endregion
		#region Unregister
		static internal void Unregister(Type controlType) {
			if (!IsRegistered(controlType))
				return;
			registredTypes.Remove(controlType);
		}
		#endregion
		#region IsRegistered
		static bool IsRegistered(Type controlType) {
			return registredTypes.ContainsKey(controlType);
		}
		#endregion
		#region CreateBehavior
		static CheckAsYouTypeBehavior CreateBehavior(Type controlType) {
			if (controlType == null || !IsRegistered(controlType))
				return null;
			Type behaviorType = registredTypes[controlType];
			return (CheckAsYouTypeBehavior)behaviorType.GetConstructor(Type.EmptyTypes).Invoke(null);
		}
		#endregion
		#region AddBehavior
		internal static void AddBehavior(DependencyObject control) {
			CheckAsYouTypeBehavior result = TryGetBehavior(control);
			if (result != null)
				return;
			result = CreateBehavior(control.GetType());
			if (result == null)
				return;
			Interaction.GetBehaviors(control).Add(result);
		}
		#endregion
		#region RemoveBehavior
		internal static void RemoveBehavior(DependencyObject control) {
			Behavior checkAsYouTypeBehavior = TryGetBehavior(control);
			if (checkAsYouTypeBehavior != null)
				Interaction.GetBehaviors(control).Remove(checkAsYouTypeBehavior);
		}
		#endregion
		#region TryGetBehavior
		internal static CheckAsYouTypeBehavior TryGetBehavior(DependencyObject control) {
			BehaviorCollection behaviors = (BehaviorCollection)control.GetValue(Interaction.BehaviorsProperty);
			if (behaviors == null)
				return null;
			for (int i = 0; i < behaviors.Count; i++) {
				if (IsCheckAsYouTypeBehavior(behaviors[i].GetType()))
					return (CheckAsYouTypeBehavior)behaviors[i];
			}
			return null;
		}
		#endregion
		#region IsCheckAsYouTypeBehavior
		static bool IsCheckAsYouTypeBehavior(Type behaviorType) {
			return registredTypes.ContainsValue(behaviorType);
		}
		#endregion
	}
	#endregion
	#region DictionarySourceHelper
	static class DictionarySourceHelper {
		internal static DictionaryCollection GetDictionaries(IEnumerable sources) {
			DictionaryCollection result = new DictionaryCollection();
			foreach (DictionarySourceBase source in sources) {
				result.Add(source.GetDictionaryInstance());
			}
			return result;
		}
		internal static void RemoveDictionaries(SpellChecker spellChecker, IEnumerable sources) {
			foreach (DictionarySourceBase source in sources) {
				ISpellCheckerDictionary dictionary = source.GetDictionaryInstance();
				if (dictionary != null)
					spellChecker.Dictionaries.Remove(dictionary);
			}
		}
	}
	#endregion
	#region CheckAsYouTypeOperationManager
	class CheckAsYouTypeOperationManager : AgTextOperationsManager, IDisposable {
		ISpellCheckTextControlController textController;
		CheckAsYouTypeBehavior behavior;
		public CheckAsYouTypeOperationManager(CheckAsYouTypeBehavior behavior)
			: base(behavior.SearchStrategy) {
			this.behavior = behavior;
			bool controlIsReadOnly = Behavior.Adapter.IsReadOnly;
			this.textController = SpellCheckTextControllersManager.Default.GetSpellCheckTextControlController(Behavior.Editor);
			Behavior.Adapter.IsReadOnly = controlIsReadOnly;
		}
		new AsyncSearchStrategy SearchStrategy { get { return base.SearchStrategy as AsyncSearchStrategy; } }
		CheckAsYouTypeBehavior Behavior { get { return behavior; } }
		public new ISpellCheckTextControlController TextController { get { return textController; } }
		internal void DoOperation(SpellCheckOperation operation, int start, int finish) {
			switch (operation) {
				case SpellCheckOperation.AddToDictionary:
					Behavior.Check(start, finish);
					break;
				case SpellCheckOperation.Change:
					Behavior.Check(start, finish);
					break;
				case SpellCheckOperation.ChangeAll:
					Behavior.Check();
					break;
				case SpellCheckOperation.Delete:
					Behavior.Check(start, finish);
					break;
				case SpellCheckOperation.Ignore:
					Behavior.Check(start, finish);
					break;
				case SpellCheckOperation.IgnoreAll:
					Behavior.Check();
					break;
				case SpellCheckOperation.SilentChange:
					Behavior.Check(start, finish);
					break;
				case SpellCheckOperation.SilentIgnore:
					Behavior.Check(start, finish);
					break;
				case SpellCheckOperation.Undo:
					Behavior.Check();
					break;
				default:
					break;
			}
		}
		public override void AddToDictionary(string word, Position start, Position finish) {
			base.AddToDictionary(word, start, finish);
			DoOperation(SpellCheckOperation.AddToDictionary, start.ToInt(), finish.ToInt());
		}
		public override void Delete(Position start, Position finish) {
			TextController.Text = TextController.EditControlText;
			Position prevPosition = TextController.GetPrevPosition(start);
			TextController.DeleteWord(ref start, ref finish);
			TextController.Select(prevPosition, prevPosition);
			DoOperation(SpellCheckOperation.Delete, start.ToInt(), start.ToInt());
		}
		public override void IgnoreAll(string word) {
			SearchStrategy.IgnoreList.Add(word);
			DoOperation(SpellCheckOperation.IgnoreAll, 0, 0);
		}
		public override void IgnoreOnce(Position start, Position finish, string word) {
			SearchStrategy.IgnoreList.Add(start, finish, word);
			DoOperation(SpellCheckOperation.Ignore, start.ToInt(), finish.ToInt());
		}
		public override bool Replace(Position start, Position finish, string newWord) {
			TextController.Text = TextController.EditControlText;
			if (TextController.ReplaceWord(start, finish, newWord)) {
				Position selectionStart = Position.Add(start, TextController.GetTextLength(newWord));
				TextController.Select(selectionStart, selectionStart);
				DoOperation(SpellCheckOperation.Change, start.ToInt(), selectionStart.ToInt());
				return true;
			}
			return false;
		}
		public override void Undo() {
			base.Undo();
			DoOperation(SpellCheckOperation.Undo, 0, 0);
		}
		public void Dispose() {
			if (this.textController != null) {
				this.textController.Dispose();
				this.textController = null;
			}
		}
	}
	#endregion
	#region ContextMenuManagerBase
	abstract class ContextMenuManagerBase : IDisposable {
		#region Fields
		CheckAsYouTypeBehavior behavior;
		ContextMenu oldEditorContextMenu;
		PopupMenu oldDXContextMenu;
		List<object> spellCheckMenuItems = new List<object>();
		int itemsCount;
		bool canUseSpellCheckMenu;
		bool isActivated;
		#endregion
		protected ContextMenuManagerBase(CheckAsYouTypeBehavior behavior) {
			this.behavior = behavior;
			Activate();
		}
		#region Properties
		protected internal abstract IList Items { get; }
		protected CheckAsYouTypeBehavior Behavior { get { return behavior; } }
		protected Control Editor { get { return Behavior.AssociatedObject as Control; } }
		TextOperationsManager OperationManager { get { return Behavior.OperationManager; } }
		protected ContextMenu OldContextMenu { get { return oldEditorContextMenu; } }
		protected PopupMenu OldDXContextMenu { get { return oldDXContextMenu; } }
		List<object> SpellCheckMenuItems { get { return spellCheckMenuItems; } }
		protected virtual bool CanUseSpellCheckMenu { get { return canUseSpellCheckMenu; } }
		protected bool IsActivated { get { return isActivated; } }
		#endregion
		protected abstract object CreateMenuItem(string text, SpellCheckErrorBase error, Action<SpellCheckErrorBase> onClick);
		protected abstract object CreateCheckSpellingMenuItem();
		protected abstract object CreateNoSuggestionsMenuItem();
		protected abstract object CreateSeparatorMenuItem();
		protected abstract object CreateAddToDictionaryMenuItem(SpellCheckErrorBase error);
		protected abstract void ActivateCore();
		protected abstract void DeactivateCore();
		protected void ClearMenu() {
			if (OldContextMenu == null && OldDXContextMenu == null)
				Items.Clear();
			else {
				foreach (object item in SpellCheckMenuItems)
					Items.Remove(item);
			}
			SpellCheckMenuItems.Clear();
			this.itemsCount = 0;
		}
		protected virtual void SubscribeToEvents() {
			Editor.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
		}
		protected virtual void UnsubscribeFromEvents() {
			Editor.PreviewMouseRightButtonDown -= OnPreviewMouseRightButtonDown;
		}
		protected virtual void OnPreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			this.canUseSpellCheckMenu = Behavior.Adapter.SelectionLength == 0;
		}
		internal void Activate() {
			if (IsActivated)
				return;
			this.oldEditorContextMenu = Editor.ContextMenu;
			this.oldDXContextMenu = GetDXContextMenu();
			ActivateCore();
			SubscribeToEvents();
			this.isActivated = true;
		}
		internal void Deactivate() {
			if (!IsActivated)
				return;
			UnsubscribeFromEvents();
			ClearMenu();
			DeactivateCore();
			this.oldDXContextMenu = null;
			this.oldEditorContextMenu = null;
			this.isActivated = false;
		}
		PopupMenu GetDXContextMenu() {
			IPopupControl popup = BarManager.GetDXContextMenu(Editor);
			return popup != null ? (PopupMenu)popup.Popup : null;
		}
		protected void AddMenuItem(object item) {
			Items.Insert(this.itemsCount, item);
			SpellCheckMenuItems.Add(item);
			this.itemsCount++;
		}
		protected virtual void AddDeleteRepeatedWordMenuItem(SpellCheckErrorBase error) {
			object item = CreateMenuItem(SpellCheckerLocalizer.GetString(SpellCheckerStringId.Menu_DeleteRepeatedWord), error, OnDeleteRepeatedWordMenuItemClick);
			AddMenuItem(item);
		}
		void AddNoSuggestionsMenuItem() {
			AddMenuItem(CreateNoSuggestionsMenuItem());
		}
		void AddIgnoreMenuItem(SpellCheckErrorBase error) {
			object item = CreateMenuItem(SpellCheckerLocalizer.GetString(SpellCheckerStringId.Menu_IgnoreRepeatedWord), error, OnIgnoreMenuItemClick);
			AddMenuItem(item);
		}
		void AddIgnoreAllMenuItem(SpellCheckErrorBase error) {
			object item = CreateMenuItem(SpellCheckerLocalizer.GetString(SpellCheckerStringId.Menu_IgnoreAllItemCaption), error, OnIgnoreAllMenuItemClick);
			AddMenuItem(item);
		}
		void AddAddToDictionaryMenuItem(SpellCheckErrorBase error) {
			AddMenuItem(CreateAddToDictionaryMenuItem(error));
		}
		void AddSuggestionMenuItem(string suggestion, SpellCheckErrorBase error) {
			object item = CreateMenuItem(suggestion, error, (err) => {
				err.Suggestion = suggestion;
				OnSuggestionMenuItemClick(err);
			});
			AddMenuItem(item);
		}
		void AddSeparatorMenuItem() {
			AddMenuItem(CreateSeparatorMenuItem());
		}
		void AddSpellingMenuItem() {
			AddMenuItem(CreateCheckSpellingMenuItem());
		}
		protected ImageSource GetCheckSpellingImage() {
			System.Windows.Media.Imaging.BitmapImage image = new System.Windows.Media.Imaging.BitmapImage();
			using (System.IO.Stream imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.Xpf.SpellChecker.images.SpellChecker.ico")) {
				image.BeginInit();
				image.StreamSource = imageStream;
				image.EndInit();
			}
			return image;
		}
		protected virtual SpellCheckErrorBase GetSpellCheckError(Position position) {
			int errorsCount = Behavior.SearchStrategy.ErrorList.Count;
			for (int i = 0; i < errorsCount; i++) {
				SpellCheckErrorBase error = (SpellCheckErrorBase)Behavior.SearchStrategy.ErrorList[i];
				if (Position.IsGreaterOrEqual(position, error.StartPosition) && Position.IsLessOrEqual(position, error.FinishPosition))
					return error;
			}
			return null;
		}
		SpellCheckErrorBase GetErrorByCaretPosition() {
			return GetSpellCheckError(new IntPosition(Behavior.Adapter.SelectionStart));
		}
		protected internal void PopulateMenu() {
			ClearMenu();
			if (!SpellingSettings.GetShowSpellCheckMenu(Behavior.Editor) || !CanUseSpellCheckMenu)
				return;
			SpellCheckErrorBase error = GetErrorByCaretPosition();
			if (error == null)
				return;
			if (error.RulesController.IsRepeatedWordError(error)) {
				AddDeleteRepeatedWordMenuItem(error);
				AddIgnoreMenuItem(error);
			}
			else if (error.RulesController.IsNotInDictionaryWordError(error)) {
				AddSuggestions(error);
				AddIgnoreMenuItem(error);
				AddIgnoreAllMenuItem(error);
				AddAddToDictionaryMenuItem(error);
			}
			AddSpellingMenuItem();
			if (OldContextMenu != null || OldDXContextMenu != null)
				AddSeparatorMenuItem();
		}
		void AddSuggestions(SpellCheckErrorBase error) {
			if (error.Suggestion == null || error.Suggestions.Count == 0)
				AddNoSuggestionsMenuItem();
			else {
				int suggestionsCount = error.Suggestions.Count;
				for (int i = 0; i < suggestionsCount; i++) {
					AddSuggestionMenuItem(error.Suggestions[i].Suggestion, error);
				}
			}
			AddSeparatorMenuItem();
		}
		protected void OnSuggestionMenuItemClick(SpellCheckErrorBase error) {
			OperationManager.Replace(error.StartPosition, error.FinishPosition, error.Suggestion);
		}
		protected void OnDeleteRepeatedWordMenuItemClick(SpellCheckErrorBase error) {
			OperationManager.Delete(error.StartPosition, error.FinishPosition);
		}
		protected void OnIgnoreMenuItemClick(SpellCheckErrorBase error) {
			OperationManager.IgnoreOnce(error.StartPosition, error.FinishPosition, error.WrongWord);
		}
		protected void OnIgnoreAllMenuItemClick(SpellCheckErrorBase error) {
			OperationManager.IgnoreAll(error.WrongWord);
		}
		protected void OnAddToDictionaryMenuItemClick(SpellCheckErrorBase error) {
			OperationManager.AddToDictionary(error.WrongWord, error.StartPosition, error.FinishPosition);
		}
		protected virtual void Dispose(bool disposing) {
			Deactivate();
		}
		public void Dispose() {
			Dispose(true);
		}
	}
	#endregion
	#region DXContextMenuManager
	class DXContextMenuManager : ContextMenuManagerBase {
		PopupMenu menu;
		public DXContextMenuManager(CheckAsYouTypeBehavior behavior)
			: base(behavior) {
		}
		PopupMenu Menu { get { return menu; } }
		protected internal override IList Items { get { return Menu.Items; } }
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			Menu.Opening += OnOpening;
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			Menu.Opening -= OnOpening;
		}
		void OnOpening(object sender, System.ComponentModel.CancelEventArgs e) {
			PopulateMenu();
		}
		protected override void OnPreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			base.OnPreviewMouseRightButtonDown(sender, e);
			Behavior.EventsManager.BeginUpdateMenu();
			if ((GetErrorByCursorPosition(e.GetPosition(Editor)) == null || !CanUseSpellCheckMenu) && OldDXContextMenu == null)
				BarManager.SetDXContextMenu(Editor, null);
			else if (BarManager.GetDXContextMenu(Editor) == null)
				BarManager.SetDXContextMenu(Editor, Menu);
			Behavior.EventsManager.EndUpdateMenu();
		}
		SpellCheckErrorBase GetErrorByCursorPosition(Point point) {
			return GetSpellCheckError(Behavior.Adapter.GetPositionFromPoint(point));
		}
		protected override void ActivateCore() {
			this.menu = OldDXContextMenu;
			BaseEdit dxEditor = Editor as BaseEdit;
			if (dxEditor != null)
				dxEditor.EditCore.ContextMenu = null;
			if (Menu == null) {
				this.menu = new PopupMenu();
				BarManager.SetDXContextMenu(Editor, Menu);
			}
		}
		protected override void DeactivateCore() {
			if (OldDXContextMenu == null)
				BarManager.SetDXContextMenu(Editor, null);
		}
		protected override object CreateMenuItem(string text, SpellCheckErrorBase error, Action<SpellCheckErrorBase> onClick) {
			BarButtonItem result = new BarButtonItem();
			result.Content = text;
			if (onClick != null)
				result.ItemClick += (s, e) => { onClick(error); };
			return result;
		}
		protected override object CreateCheckSpellingMenuItem() {
			BarButtonItem item = (BarButtonItem)CreateMenuItem(SpellCheckerLocalizer.GetString(SpellCheckerStringId.Menu_ItemCaption), null, (err) => { Behavior.SpellChecker.Check(Editor); });
			item.BarItemDisplayMode = BarItemDisplayMode.ContentAndGlyph;
			item.Glyph = GetCheckSpellingImage();
			return item;
		}
		protected override object CreateNoSuggestionsMenuItem() {
			BarButtonItem item = (BarButtonItem)CreateMenuItem(SpellCheckerLocalizer.GetString(SpellCheckerStringId.Menu_NoSuggestionsCaption), null, null);
			item.IsEnabled = false;
			return item;
		}
		protected override object CreateSeparatorMenuItem() {
			return new BarItemSeparator();
		}
		protected override object CreateAddToDictionaryMenuItem(SpellCheckErrorBase error) {
			BarButtonItem item = (BarButtonItem)CreateMenuItem(SpellCheckerLocalizer.GetString(SpellCheckerStringId.Menu_AddToDictionaryCaption), error, OnAddToDictionaryMenuItemClick);
			item.IsEnabled = Behavior.SpellChecker.DictionaryHelper.GetCustomDictionary(Behavior.SearchStrategy.ActualCulture) != null;
			return item;
		}
	}
	#endregion
	#region NativeContextMenuManager
	class NativeContextMenuManager : ContextMenuManagerBase {
		ContextMenu menu;
		internal NativeContextMenuManager(CheckAsYouTypeBehavior behavior)
			: base(behavior) {
		}
		ContextMenu Menu { get { return menu; } }
		protected internal override IList Items { get { return Menu.Items; } }
		void OnContextMenuOpening(object sender, ContextMenuEventArgs e) {
			PopulateMenu();
		}
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			Editor.ContextMenuOpening += OnContextMenuOpening;
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			Editor.ContextMenuOpening -= OnContextMenuOpening;
		}
		protected override void ActivateCore() {
			this.menu = Editor.ContextMenu;
		}
		protected override void DeactivateCore() {
			this.menu = null;
		}
		protected override object CreateMenuItem(string text, SpellCheckErrorBase error, Action<SpellCheckErrorBase> onClick) {
			MenuItem result = new MenuItem();
			result.Header = text;
			result.Click += (s, e) => { onClick(error); };
			return result;
		}
		protected override object CreateCheckSpellingMenuItem() {
			MenuItem result = (MenuItem)CreateMenuItem(SpellCheckerLocalizer.GetString(SpellCheckerStringId.Menu_ItemCaption), null, (err) => { Behavior.SpellChecker.Check(Editor); });
			Image img = new Image();
			img.BeginInit();
			img.Source = GetCheckSpellingImage();
			img.EndInit();
			result.Icon = img;
			return result;
		}
		protected override object CreateNoSuggestionsMenuItem() {
			MenuItem result = (MenuItem)CreateMenuItem(SpellCheckerLocalizer.GetString(SpellCheckerStringId.Menu_NoSuggestionsCaption), null, null);
			result.IsEnabled = false;
			return result;
		}
		protected override object CreateSeparatorMenuItem() {
			return new Separator();
		}
		protected override object CreateAddToDictionaryMenuItem(SpellCheckErrorBase error) {
			MenuItem result = (MenuItem)CreateMenuItem(SpellCheckerLocalizer.GetString(SpellCheckerStringId.Menu_AddToDictionaryCaption), error, OnAddToDictionaryMenuItemClick);
			result.IsEnabled = Behavior.SpellChecker.DictionaryHelper.GetCustomDictionary(Behavior.SearchStrategy.ActualCulture) != null;
			return result;
		}
	}
	#endregion
	#region UriValidator
	static class UriValidator {
		public static void Validate(Uri uri) {
			if (uri == null)
				return;
			Stream stream = null;
			try {
				WebRequest request = CreateWebRequest(uri);
				WebResponse response = request.GetResponse();
				stream = response.GetResponseStream();
				if (response != null)
					response.Close();
			}
			catch { }
			if (stream == null)
				RaiseResourceNotFoundException(uri);
			else
				stream.Close();
		}
		public static void RaiseResourceNotFoundException(Uri uri) {
			throw new ResourceNotFoundException(uri);
		}
		static WebRequest CreateWebRequest(Uri uri) {
			if (String.Compare(uri.Scheme, System.IO.Packaging.PackUriHelper.UriSchemePack, StringComparison.Ordinal) == 0) {
				MethodInfo info = typeof(System.IO.Packaging.PackWebRequestFactory).GetMethod("CreateWebRequest", BindingFlags.Static | BindingFlags.NonPublic);
				return (WebRequest)info.Invoke(null, new object[] { uri });
			}
			if (uri.IsFile)
				uri = new Uri(uri.GetLeftPart(UriPartial.Path));
			return WebRequest.Create(uri);
		}
	}
	#endregion
	#region VisibilityConverter
	class VisibilityConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			bool isVisible = (bool)values[0];
			Visibility visibility = (Visibility)values[1];
			if (visibility != Visibility.Visible)
				return visibility;
			return isVisible ? Visibility.Visible : Visibility.Hidden;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	#endregion
	class EditorTextChangedEventArgs : EventArgs {
		ICollection<TextChange> changes;
		internal EditorTextChangedEventArgs(ICollection<TextChange> changes) {
			this.changes = changes;
		}
		internal ICollection<TextChange> Changes { get { return changes; } }
	}
	delegate void EditorTextChangedEventHandler(object sender, EditorTextChangedEventArgs args);
}
