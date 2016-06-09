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
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class RichEditCallbackArgumentsReader {
		internal const string InternalCallbackPrefix = "IC-",
							  SaveImageToServerErrorCallbackPrefix = "REISE",
							  SaveImageToServerNewUrlCallbackPrefix = "REISU",
							  SaveImageToServerCallbackPrefix = "REITS",
							  FileManagerCallbackPrefix = "REFM",
							  ColumnsEditorCallbackPrefix = "RECE",
							  SymbolListCallbackPrefix = "RESL",
							  PerformCallbackPrefix = "REPC",
							  CommandCallbackPrefix = "REC";
		Dictionary<string, object> argumentsDictionary;
		public RichEditCallbackArgumentsReader(string callbackArgs) {
			CallbackArgs = callbackArgs;
		}
		protected string CallbackArgs { get; private set; }
		protected Dictionary<string, object> ArgumentsDictionary {
			get {
				if(this.argumentsDictionary == null) {
					this.argumentsDictionary = new Dictionary<string, object>();
					DictionarySerializer.Deserialize(CallbackArgs, this.argumentsDictionary);
				}
				return this.argumentsDictionary;
			}
		}
		public bool IsCustomCallback { get { return !CallbackArgs.StartsWith(InternalCallbackPrefix); } }
		public bool IsCommandCallback { get { return !string.IsNullOrEmpty(CommandCallbackArg); } }
		public string CommandCallbackArg { get { return GetInternalCallbackArgument(CommandCallbackPrefix); } }
		public bool IsPerformCallback { get { return ArgumentsDictionary.ContainsKey(PerformCallbackPrefix); } }
		public string PerformCallbackArg { get { return GetCallbackArgument(PerformCallbackPrefix); } }
		public bool IsDialogCallback { get { return !string.IsNullOrEmpty(DialogFormName); } }
		public string DialogFormName { get { return GetInternalCallbackArgument(RenderUtils.DialogFormCallbackStatus); } }
		public bool IsFileManagerCallback { get { return !string.IsNullOrEmpty(FileManagerCallbackData); } }
		public string FileManagerCallbackData { get { return GetInternalCallbackArgument(FileManagerCallbackPrefix); } }
		public bool IsColumnsEditorCallback { get { return !string.IsNullOrEmpty(ColumnsCount); } }
		public string ColumnsCount { get { return GetInternalCallbackArgument(ColumnsEditorCallbackPrefix); } }
		public bool IsUploadImageCallback { get { return !string.IsNullOrEmpty(ImageUrl); } }
		public string ImageUrl { get { return GetInternalCallbackArgument(SaveImageToServerCallbackPrefix); } }
		public bool IsSymbolListCallback { get { return !string.IsNullOrEmpty(SymbolFontName); } }
		public string SymbolFontName { get { return GetInternalCallbackArgument(SymbolListCallbackPrefix); } }
		string GetInternalCallbackArgument(string callbackPrefix) {
			return GetCallbackArgument(InternalCallbackPrefix + callbackPrefix);
		}
		string GetCallbackArgument(string callbackPrefix) {
			return ArgumentsDictionary.ContainsKey(callbackPrefix) ? (string)ArgumentsDictionary[callbackPrefix] : null;
		}
	}
	public enum DownloadImageErrorType { None, InvalidImageUrl, ImageFileSizeError, InternalError };
	public class RichEditDownloadImageHelper {
		ASPxRichEdit richedit;
		protected ASPxRichEdit RichEdit {
			get { return richedit; }
		}
		public RichEditDownloadImageHelper(ASPxRichEdit richedit) {
			this.richedit = richedit;
		}
		public string AddImageToCacheFromUrl(string url, ref DownloadImageErrorType errorType) {
			string ret = string.Empty;
			var regex = new Regex(@"data:image\/(.+);base64,(?<data>.*)");
			if(regex.IsMatch(url)) {
				using(var stream = new MemoryStream(Convert.FromBase64String(regex.Match(url).Groups["data"].Value))) {
					ret = RichEdit.AddImageToCache(stream);
				}
			}
			else {
				if(!UrlUtils.IsAbsoluteUrl(url)) {
					HttpRequest request = HttpContext.Current.Request;
					UriBuilder uriBuilder = new UriBuilder(request.Url);
					uriBuilder.Query = "";
					uriBuilder.Path = UrlUtils.Combine(UrlUtils.AppDomainAppVirtualPathString, request.Url.AbsolutePath, url);
					url = uriBuilder.Uri.AbsoluteUri;
				}
				WebResponse response = null;
				Stream responseStream = null;
				try {
					WebRequest request = WebRequest.Create(url);
					response = request.GetResponse();
					errorType = Validate(response);
					if(errorType == DownloadImageErrorType.None) {
						responseStream = response.GetResponseStream();
						MemoryStream stream = new MemoryStream();
						responseStream.CopyTo(stream);
						stream.Position = 0;
						ret = RichEdit.AddImageToCache(stream);
					}
				}
				catch(UriFormatException) {
					errorType = DownloadImageErrorType.InvalidImageUrl;
				}
				catch(Exception e) {
					ret = e.Message;
					if(e.InnerException != null)
						ret = e.InnerException.Message;
					errorType = DownloadImageErrorType.InternalError;
				}
				finally {
					if(responseStream != null)
						responseStream.Close();
					if(response != null)
						response.Close();
				}
			}
			return ret;
		}
		protected DownloadImageErrorType Validate(WebResponse response) {
			DownloadImageErrorType ret = DownloadImageErrorType.None;
			bool isValid = true;
			if(!IsAllowedContentType(response.ContentType)) {
				isValid = false;
				ret = DownloadImageErrorType.InvalidImageUrl;
			}
			if(isValid && !IsAllowedContentSize(response.ContentLength)) {
				ret = DownloadImageErrorType.ImageFileSizeError;
			}
			return ret;
		}
		protected bool IsAllowedContentType(string contentType) {
			return true;
		}
		protected bool IsAllowedContentSize(long size) {
			return true;
		}
	}
	public static class RichEditSymbolsManager {
		public static string GetResult(string fontName) {
			List<char> list = new List<char>();
			using(FontCache cache = new GdiPlusFontCache(new DocumentLayoutUnitDocumentConverter())) {
				FontCharacterSet sourceFontCharacterSet = cache.GetFontCharacterSet(fontName);
				if(sourceFontCharacterSet == null)
					return null;
				for(int i = 0; i < UInt16.MaxValue; i++) {
					char character = (char)i;
					bool isCharCategoryControl = Char.IsControl(character);
					bool isCharCategoryPrivateUse = Char.GetUnicodeCategory(character) == UnicodeCategory.PrivateUse;
					if(sourceFontCharacterSet.ContainsChar(character) && !isCharCategoryControl && !isCharCategoryPrivateUse)
						list.Add(character);
				}
				return HtmlConvertor.ToJSON(list);
			}
		}
	}
	public enum DownloadRequestType {
		PrintCurrentDocument = 0,
		DownloadCurrentDocument = 1,
		DownloadMergedDocument = 2
	}
	public enum ErrorMessageText {
		ModelIsChanged = 0,
		SessionHasExpired = 1,
		OpeningAndOverstoreImpossible = 2,
		ClipboardAccessDenied = 3,
		InnerException = 4,
		AuthException = 5,
		CantOpenFile = 6,
		CantSaveFile = 7,
		DocVariableException = 8,
		PathTooLongException = 9
	}
}
