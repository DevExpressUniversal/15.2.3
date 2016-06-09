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
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows;
using DevExpress.DemoData.Utils;
namespace DevExpress.DemoData.Helpers {
	public enum Language { English, Japan, Hadza, Sandawe }
	public static class LanguageHelper {
		static Dictionary<string, Language> languages = new Dictionary<string, Language>();
		static Dictionary<Language, string> isoNames = new Dictionary<Language, string>();
		static LanguageHelper() {
			RegisterLanguage("en", Language.English);
			RegisterLanguage("ja", Language.Japan);
			RegisterLanguage("hts", Language.Hadza);
			RegisterLanguage("sad", Language.Sandawe);
		}
		public static Language GetLanguageFromCulture(CultureInfo culture) {
			Language language;
			return languages.TryGetValue(culture.TwoLetterISOLanguageName, out language) ? language : Language.English;
		}
		public static string GetISONameFromLanguage(Language language) {
			return isoNames[language];
		}
		static void RegisterLanguage(string isoName, Language language) {
			languages.Add(isoName, language);
			isoNames.Add(language, isoName);
		}
	}
	public abstract class LString : INotifyPropertyChanged {
		class DispatcherContainer : DependencyObject { }
		class LStringEmpty : LString {
			protected override string GetText() {
				return string.Empty;
			}
		}
		class LStringValue : LString {
			Func<Language, string> resolveText;
			public LStringValue(Func<Language, string> resolveText) {
				this.resolveText = resolveText;
			}
			protected override string GetText() {
				return this.resolveText(CurrentLanguage);
			}
		}
		class LStringConcat : LString {
			LString[] sources;
			public LStringConcat(LString[] sources) {
				this.sources = sources;
			}
			protected override string GetText() {
				StringBuilder text = new StringBuilder();
				foreach(LString source in sources) {
					text.Append(source.Text);
				}
				return text.ToString();
			}
		}
		class LStringFormat : LString {
			LString format;
			LString[] args;
			public LStringFormat(LString format, LString[] args) {
				this.format = format;
				this.args = args;
			}
			protected override string GetText() {
				return string.Format(CultureInfo.InvariantCulture, this.format.Text, this.args);
			}
		}
		class LStringInvariantFormat : LString {
			LString format;
			string[] args;
			public LStringInvariantFormat(LString format, string[] args) {
				this.format = format;
				this.args = args;
			}
			protected override string GetText() {
				return string.Format(CultureInfo.InvariantCulture, this.format.Text, this.args);
			}
		}
		class LStringConvert : LString {
			LString source;
			Func<string, string> converter;
			public LStringConvert(LString source, Func<string, string> converter) {
				if(source == null) throw new ArgumentNullException("source");
				if(converter == null) throw new ArgumentNullException("converter");
				this.source = source;
				this.converter = converter;
			}
			protected override string GetText() {
				return this.converter(this.source.Text);
			}
		}
		class LStringHTMLConvert : LString {
			LString source;
			public LStringHTMLConvert(LString source) {
				this.source = source;
			}
			protected override string GetText() {
				return ProcessHTMLSymbols(this.source.Text);
			}
			String ProcessHTMLSymbols(String str) {
				string text = str.Replace("&lt;", "<");
				text = text.Replace("&gt;", ">");
				return text;
			}
		}
		static LinkedList<WeakReference> allStrings = new LinkedList<WeakReference>();
		static object mainLock = new object();
		static bool cancel = false;
		static object workLock = new object();
		static Language currentLanguage;
		static ManualResetEvent updateStarted = new ManualResetEvent(true);
		static LString empty = new LStringEmpty();
		public static Language CurrentLanguage {
			get { return currentLanguage; }
			set {
				lock(mainLock) {
					if(currentLanguage == value) return;
					cancel = true;
					lock(workLock) {
						cancel = false;
						currentLanguage = value;
						updateStarted.Reset();
						BackgroundHelper.DoInBackground(DoUpdate, null, 0);
					}
				}
			}
		}
		public static void WaitWhileUpdatingInProgress() {
			lock(mainLock) {
				updateStarted.WaitOne();
				lock(workLock) { }
			}
		}
		public static LString Empty { get { return empty; } }
		public static LString Get(Func<Language, string> resolveText) {
			return GetString(new LStringValue(resolveText));
		}
		public static LString Concat(params LString[] args) {
			return GetString(new LStringConcat(args));
		}
		public static LString Format(LString format, params LString[] args) {
			return GetString(new LStringFormat(format, args));
		}
		public static LString Format(LString format, params string[] args) {
			return GetString(new LStringInvariantFormat(format, args));
		}
		public static LString Convert(LString source, Func<string, string> converter) {
			return GetString(new LStringConvert(source, converter));
		}
		public static LString ConvertHTML(LString source) {
			if (source.Text.Contains("&lt;") && source.Text.Contains("&gt;")) {
				LString str = new LStringHTMLConvert(source);
				str.Update();
				return str;
			}
			return source;
		}
		static LString GetString(LString s) {
			lock(workLock) {
				allStrings.AddLast(new WeakReference(s));
				s.Update();
				return s;
			}
		}
		static void DoUpdate() {
			lock(workLock) {
				updateStarted.Set();
				LinkedListNode<WeakReference> next = null;
				for(LinkedListNode<WeakReference> node = allStrings.First; node != null; node = next) {
					if(cancel) {
						return;
					}
					next = node.Next;
					LString s = (LString)node.Value.Target;
					if(s == null) {
						allStrings.Remove(node);
						continue;
					}
					s.Update();
				}
			}
		}
		object dispatcherLock = new object();
		DispatcherContainer dispatcher;
		string text;
		string textAsync;
		public LString() {
			Text = string.Empty;
		}
		public string Text {
			get { return text; }
			private set {
				if(text == value)
					return;
				text = value;
				OnPropertyChanged("Text");
			}
		}
		public string TextAsync {
			get {
				lock(dispatcherLock) {
					if(dispatcher == null) {
						dispatcher = new DispatcherContainer();
						textAsync = Text;
					}
					return textAsync;
				}
			}
			private set {
				if (textAsync == value)
					return;
				textAsync = value;
				OnPropertyChanged("TextAsync");
			}
		}
		public event EventHandler TextChanged;
		public void Update() {
			Text = GetText();
		}
		protected abstract string GetText();
		void RaiseTextChanged() {
			if(TextChanged != null)
				TextChanged(this, EventArgs.Empty);
			lock(dispatcherLock) {
				if(dispatcher == null) return;
				dispatcher.Dispatcher.BeginInvoke((Action)UpdateTextAsync);
			}
		}
		void UpdateTextAsync() {
			TextAsync = Text;
		}
		public override string ToString() {
			return Text;
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
