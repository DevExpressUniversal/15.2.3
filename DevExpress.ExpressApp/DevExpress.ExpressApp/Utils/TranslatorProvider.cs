#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
namespace DevExpress.ExpressApp.Utils {
	public interface ITranslatorProvider {
		string Caption { get; }
		string Description { get; }
		string[] GetLanguages();
		string[] Translate(string[] text, string sourceLanguageCode, string desinationLanguageCode);
		event EventHandler<TranslationProgressEventArgs> TranslationProgressChanged;
	}
	public class TranslationProgressEventArgs : CancelEventArgs {
		private decimal progress;
		public TranslationProgressEventArgs(decimal progress, bool cancel)
			: base(cancel) {
			this.progress = progress;
		}
		public decimal Progress {
			get {
				return progress;
			}
		}
	}
	public abstract class TranslatorProviderBase : ITranslatorProvider {
		private string separator;
		private int textBlockSize;
		public TranslatorProviderBase(string separator, int textBlockSize) {
			this.separator = separator;
			this.textBlockSize = textBlockSize;
		}
		private List<string> CreateTranslationBlocks(IEnumerable<string> atomicTexts) {
			List<string> translateTextBlocks = new List<string>();
			string translateTextBlock = string.Empty;
			foreach (string atomicText in atomicTexts) {
				if (translateTextBlock.Length + atomicText.Length + separator.Length < textBlockSize) {
					translateTextBlock = string.IsNullOrEmpty(translateTextBlock) ? string.Empty : translateTextBlock + separator;
					translateTextBlock += atomicText;
				}
				else {
					translateTextBlocks.Add(translateTextBlock);
					translateTextBlock = atomicText;
				}
			}
			if (translateTextBlock.Length > 0) {
				translateTextBlocks.Add(translateTextBlock);
			}
			return translateTextBlocks;
		}
		private Dictionary<string, string> AtomicTranslate(ICollection<string> atomicTexts, string sourceLanguageCode, string desinationLanguageCode) {
			List<string> translateTextBlocks = CreateTranslationBlocks(atomicTexts);
			Dictionary<string, string> atomicTranslation = new Dictionary<string, string>();
			for (int iTranlateBlock = 0; iTranlateBlock < translateTextBlocks.Count; iTranlateBlock++) {
				string translatedTextBlock = Translate(translateTextBlocks[iTranlateBlock], sourceLanguageCode, desinationLanguageCode);
				if (!OnTranslationProgressChanged((decimal)(iTranlateBlock + 1) / translateTextBlocks.Count)) {
					return null;
				}
				string[] translateAtomicTexts = (translateTextBlocks[iTranlateBlock]).Split(new string[] { separator }, StringSplitOptions.None);
				string[] translatedAtomicTexts = translatedTextBlock.Split(new string[] { separator }, StringSplitOptions.None);
				if (translateAtomicTexts.Length == translatedAtomicTexts.Length)
					for (int i = 0; i < translateAtomicTexts.Length; i++)
						if (!atomicTranslation.ContainsKey(translateAtomicTexts[i]))
							atomicTranslation.Add(translateAtomicTexts[i], translatedAtomicTexts[i].Trim());
			}
			return atomicTranslation;
		}
		public string[] Translate(string[] texts, string sourceLanguageCode, string desinationLanguageCode) {
			Dictionary<string, List<string>> translateTexts = new Dictionary<string, List<string>>();
			foreach (string text in texts) {
				if (!translateTexts.ContainsKey(text)) {
					translateTexts.Add(text, CalculateAtomicTextBlocks(text));
				}
			}
			List<string> atomicTexts = new List<string>();
			foreach (List<string> translateText in translateTexts.Values) {
				atomicTexts.AddRange(translateText);
			}
			OnTranslationProgressChanged(0);
			Dictionary<string, string> atomicTranslation = AtomicTranslate(atomicTexts, sourceLanguageCode, desinationLanguageCode);
			OnTranslationProgressChanged(1);
			if (atomicTranslation == null) {
				return null;
			}
			List<string> resultTranslation = new List<string>();
			foreach (KeyValuePair<string, List<string>> translatedText in translateTexts) {
				resultTranslation.Add(GetTranslatedText(translatedText.Key, translatedText.Value, atomicTranslation).Trim());
			}
			return resultTranslation.ToArray();
		}
		protected virtual bool OnTranslationProgressChanged(decimal percent) {
			TranslationProgressEventArgs translationProgressEventArgs = new TranslationProgressEventArgs(percent, false);
			if (TranslationProgressChanged != null) {
				TranslationProgressChanged(this, translationProgressEventArgs);
			}
			return !translationProgressEventArgs.Cancel;
		}
		internal List<string> CalculateAtomicTextBlocks(string text) {
			List<string> result = new List<string>();
			IEnumerable<string> lineBlocks = text.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string lineBlock in lineBlocks) {
				foreach (string block in CalculateSentences(lineBlock)) {
					string trimBlock = block.Trim();
					if(!string.IsNullOrEmpty(trimBlock)) { 
						result.Add(trimBlock);
					}
				}
			}
			return result;
		}
		internal string GetTranslatedText(string text, IEnumerable<string> atomicTexts, Dictionary<string, string> translation) {
			string result = text;
			foreach (string textBlock in atomicTexts) {
				if (translation.ContainsKey(textBlock)) {
					result = result.Replace(textBlock, translation[textBlock]);
				}
			}
			return result;
		}
		public virtual IEnumerable<string> CalculateSentences(string text) {
			return new string[] { text };
		}
		public abstract string Translate(string text, string sourceLanguageCode, string desinationLanguageCode);
		public event EventHandler<TranslationProgressEventArgs> TranslationProgressChanged;
		public abstract string Caption { get; }
		public abstract string Description { get; }
		public abstract string[] GetLanguages();
	}
	public static class TranslatorProvider {
		private static ITranslatorProvider provider = null;
		public static void RegisterProvider(ITranslatorProvider provider) {
			TranslatorProvider.provider = provider;
		}
		public static ITranslatorProvider GetProvider() {
			return provider;
		}
	}
}
