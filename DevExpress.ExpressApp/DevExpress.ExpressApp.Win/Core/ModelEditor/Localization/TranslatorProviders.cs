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

using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class BingTranslatorProvider : TranslatorProviderBase {
		private const int timeout = 15000;
		public string appId = "65C7264047E3D58C91A7655FCD8F5E6892306B1B";
		private const string serviceUrl = "http://api.microsofttranslator.com/V1/Http.svc/";
		private string[] languages;
		public BingTranslatorProvider()
			: base("<br>", 2000) {
			languages = null;
		}
		private string BingTranslatorInvoke(string methodName, IEnumerable<KeyValuePair<string, string>> parameters) {
			return BingTranslatorInvoke(methodName, parameters, null);
		}
		private string BingTranslatorInvoke(string methodName, IEnumerable<KeyValuePair<string, string>> parameters, string postParameter) {
			UTF8Encoding encoding = new UTF8Encoding();
			string postData = "";
			foreach(KeyValuePair<string, string> pair in parameters) {
				postData += string.IsNullOrEmpty(postData) ? "?" : "&";
				postData += pair.Key + "=" + pair.Value;
			}
			Uri serviceUri = new Uri(serviceUrl + methodName + postData);
			System.Net.HttpWebRequest httpRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(serviceUri);
			httpRequest.Timeout = timeout;
			if(!string.IsNullOrEmpty(postParameter)) {
				httpRequest.Method = "POST";
				httpRequest.ContentType = "text/plain";
				using (Stream requestStream = httpRequest.GetRequestStream()) {
					byte[] bytes = Encoding.UTF8.GetBytes(postParameter);
					requestStream.Write(bytes, 0, bytes.Length);
				}
			}
			System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)httpRequest.GetResponse();
			String resp = null;
			using (StreamReader sReader = new StreamReader(response.GetResponseStream(), encoding)) {
				resp = sReader.ReadToEnd();
			}
			return resp; 
		}
		#region ITranslatorProvider Members
		public override string Caption {
			get { return "Microsoft Translator"; }
		}
		public override string Description {
			get { return "Powered by <b>Microsoft® Translator</b>"; }
		}
		public override string[] GetLanguages() {
			if (languages == null) {
				string result = BingTranslatorInvoke("GetLanguages", new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("appId", appId) });
				languages = result.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
			}
			return languages;
		}
		public string Detect(string text) {
			string result = BingTranslatorInvoke("Detect", 
				new KeyValuePair<string, string>[] { 
					new KeyValuePair<string, string>("appId", appId)
				},
				text
			);
			return result;
		}
		public override string Translate(string text, string sourceLanguageCode, string desinationLanguageCode) {
			if(string.IsNullOrEmpty(sourceLanguageCode)) {
				sourceLanguageCode = Detect(text);
			}
			string result = BingTranslatorInvoke("Translate",
				new KeyValuePair<string, string>[] { 
					new KeyValuePair<string, string>("appId", appId),
					new KeyValuePair<string, string>("from", sourceLanguageCode),
					new KeyValuePair<string, string>("to", desinationLanguageCode)
				},
				text
			);
			return result;
		}
		#endregion
		public override IEnumerable<string> CalculateSentences(string text) {
			string[] leftSeparators = new string[] { "\"", "^'", " '", "('", "{" };
			string[] rightSeparators = new string[] { "\"", "'", "'", "'", "}" };
			int index = 0;
			int leftSeparatorIndex = 0;
			int rightSeparatorIndex = 0;
			int iSeparator = 0;
			int leftSeparatorSize = 0;
			while(index < text.Length) {
				leftSeparatorIndex = -1;
				for(int i = 0; i < leftSeparators.Length; i++) {
					int separatorIndex = -1;
					if(leftSeparators[i][0] == '^') {
						separatorIndex = text.IndexOf(leftSeparators[i].Substring(1), index);
						if(separatorIndex == 0) {
							iSeparator = i;
							leftSeparatorIndex = separatorIndex;
							leftSeparatorSize = leftSeparators[i].Length - 1;
						}
					}
					else {
						separatorIndex = text.IndexOf(leftSeparators[i], index);
						if(separatorIndex >= 0 && (leftSeparatorIndex < 0 || separatorIndex < leftSeparatorIndex)) {
							iSeparator = i;
							leftSeparatorIndex = separatorIndex;
							leftSeparatorSize = leftSeparators[i].Length;
						}
					}
				}
				if(leftSeparatorIndex >= 0) {
					rightSeparatorIndex = text.IndexOf(rightSeparators[iSeparator], leftSeparatorIndex + leftSeparatorSize);
					if(rightSeparatorIndex >= 0) {
						string result = text.Substring(index, leftSeparatorIndex - index).Trim();
						if(result.Length > 0) {
							yield return result;
						}
						index = rightSeparatorIndex + rightSeparators[iSeparator].Length;
						continue;
					}
				}
				yield return text.Substring(index, text.Length - index).Trim();
				index = text.Length;
			}
		}
	}
}
