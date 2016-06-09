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
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using DevExpress.Office.Internal;
using DevExpress.Office.Utils;
using DevExpress.Web.Internal;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Web.ASPxSpreadsheet.Internal {
	public enum DownloadImageErrorType {
		None,
		InvalidImageUrl,
		ImageFileSizeError,
		InternalError
	};
	public class SpreadsheetDownloadHelper {
		internal const string TemporaryImageName = "tmp.png";
		internal const string RegExBase64ImageUrl = @"data:image\/(.+);base64,(?<data>.*)";
		internal const string RegExChartUrl = @"\/DXB\.axd\?s=(.+)DXCache=(.+)";
		internal const string RegExInternalImageUrl = @"\/DXS\.axd\?s=(.+)simg=(\d+)";
		protected ASPxSpreadsheet Spreadsheet {
			get;
			private set;
		}
		protected DocumentModel DocumentModel {
			get;
			private set;
		}
		internal DownloadImageErrorType ErrorType {
			get;
			private set;
		}
		internal StringBuilder Message {
			get;
			private set;
		}
		public SpreadsheetDownloadHelper(ASPxSpreadsheet spreadsheet) {
			this.Spreadsheet = spreadsheet;
			ClearErrorList();
		}
		public SpreadsheetDownloadHelper(DocumentModel model) {
			this.DocumentModel = model;
			ClearErrorList();
		}
		public string GetInsertImageUploadResult(string imageUrl) {
			return GetSaveImageFromUrlCallbackResult(imageUrl);
		}
		public void InsertImageFromUrlToDocumentModel(ICell cell, string url) {
			if(UrlResourceIsChart(url))
				return;
			Regex regex = new Regex(SpreadsheetDownloadHelper.RegExInternalImageUrl);
			if(regex.IsMatch(url)) {
				InsertImageFromCache(cell, int.Parse(regex.Match(url).Groups[2].Value));
			} else {
				using(MemoryStream stream = GetImageStreamFromUrl(url)) {
					if(ErrorType == DownloadImageErrorType.None && stream != null && stream.Length > 0) {
						DocumentModel.ActiveSheet.InsertPicture(ImportFromStream(stream, SpreadsheetDownloadHelper.TemporaryImageName), cell.Key);
					}
				}
			}
		}
		private void ClearErrorList() {
			this.ErrorType = DownloadImageErrorType.None;
			this.Message = new StringBuilder();
		}
		protected MemoryStream GetImageStreamFromUrl(string url) {
			ClearErrorList();
			MemoryStream stream = new MemoryStream();
			string fileName = string.Empty;
			Regex regex = new Regex(RegExBase64ImageUrl);
			if(regex.IsMatch(url)) {
				fileName = "tmp." + regex.Match(url).Groups[1].Value;
				stream = new MemoryStream(Convert.FromBase64String(regex.Match(url).Groups["data"].Value));
				ErrorType = Validate(Path.GetExtension(fileName), stream.Length);
			} else {
				url = PrepareUrl(url);
				WebResponse response = null;
				Stream responseStream = null;
				try {
					WebRequest request = WebRequest.Create(url);
					response = request.GetResponse();
					ErrorType = Validate(Path.GetExtension(response.ResponseUri.AbsoluteUri), response.ContentLength);
					if(ErrorType == DownloadImageErrorType.None) {
						responseStream = response.GetResponseStream();
						responseStream.CopyTo(stream);
						stream.Position = 0;
						fileName = Path.GetFileName(response.ResponseUri.AbsoluteUri);
					}
				} catch(UriFormatException) {
					ErrorType = DownloadImageErrorType.InvalidImageUrl;
				} catch(Exception e) {
					Message.Append(e.Message);
					if(e.InnerException != null)
						Message.Append(e.InnerException.Message);
					ErrorType = DownloadImageErrorType.InternalError;
				} finally {
					if(responseStream != null)
						responseStream.Close();
					if(response != null)
						response.Close();
				}
			}
			return stream;
		}
		protected void InsertImageFromCache(ICell cell, int imageCacheId) {
			var image = DocumentModel.ImageCache.GetImageByKey(imageCacheId);
			using(MemoryStream stream = new MemoryStream(image.GetImageBytes(DevExpress.Office.Utils.OfficeImageFormat.Png))) {
				if(stream != null && stream.Length > 0) {
					DocumentModel.ActiveSheet.InsertPicture(ImportFromStream(stream, SpreadsheetDownloadHelper.TemporaryImageName), cell.Key);
				}
			}
		}
		protected string PrepareUrl(string url) {
			if(!UrlUtils.IsAbsoluteUrl(url)) {
				HttpRequest request = HttpContext.Current.Request;
				UriBuilder uriBuilder = new UriBuilder(request.Url);
				uriBuilder.Query = "";
				uriBuilder.Path = UrlUtils.Combine(UrlUtils.AppDomainAppVirtualPathString, request.Url.AbsolutePath, url);
				url = uriBuilder.Uri.AbsoluteUri;
			}
			return url;
		}
		protected string GetSaveImageFromUrlCallbackResult(string imageUrl) {
			StringBuilder callbackResult = new StringBuilder(SpreadsheetDialogCallbackArgumentsReader.SaveImageToServerCallbackPrefix + RenderUtils.CallBackSeparator);
			string commandResult = string.Empty;
			using(MemoryStream stream = GetImageStreamFromUrl(imageUrl)) {
				if(stream != null && stream.Length > 0 && ErrorType == DownloadImageErrorType.None)
					commandResult = Spreadsheet.AddImage(stream, TemporaryImageName);
				switch(ErrorType) {
					case DownloadImageErrorType.None:
					callbackResult.Append(SpreadsheetDialogCallbackArgumentsReader.SaveImageToServerNewUrlCallbackPrefix +
						RenderUtils.CallBackSeparator + commandResult);
					break;
					case DownloadImageErrorType.ImageFileSizeError:
					callbackResult.Append(SpreadsheetDialogCallbackArgumentsReader.SaveImageToServerErrorCallbackPrefix +
						RenderUtils.CallBackSeparator + Message.ToString());
					break;
					case DownloadImageErrorType.InvalidImageUrl:
					callbackResult.Append(SpreadsheetDialogCallbackArgumentsReader.SaveImageToServerErrorCallbackPrefix +
						RenderUtils.CallBackSeparator + Message.ToString());
					break;
					case DownloadImageErrorType.InternalError:
					callbackResult.Append(SpreadsheetDialogCallbackArgumentsReader.SaveImageToServerErrorCallbackPrefix +
						RenderUtils.CallBackSeparator + commandResult + Message.ToString());
					break;
				}
			}
			return callbackResult.ToString();
		}
		protected DownloadImageErrorType Validate(string imageExt, long fileSzie) {
			DownloadImageErrorType ret = DownloadImageErrorType.None;
			bool isValid = true;
			if(!IsAllowedFileFormat(imageExt)) {
				isValid = false;
				ret = DownloadImageErrorType.InvalidImageUrl;
			}
			if(isValid && !IsAllowedFileSize(fileSzie)) {
				ret = DownloadImageErrorType.ImageFileSizeError;
			}
			return ret;
		}
		protected bool IsAllowedFileFormat(string contentType) {
			return true; 
		}
		protected bool IsAllowedFileSize(long size) {
			return true; 
		}
		protected OfficeImage ImportFromStream(Stream stream, string fileName) {
			PictureFormatsManagerService importManagerService = new PictureFormatsManagerService();
			SpreadsheetImageImportHelper importHelper = new SpreadsheetImageImportHelper(DocumentModel);
			IImporter<OfficeImageFormat, OfficeImage> importer = importHelper.AutodetectImporter(fileName, importManagerService);
			return importHelper.Import(stream, String.Empty, importer, null);
		}
		protected bool UrlResourceIsChart(string url) {
			Regex regex = new Regex(SpreadsheetDownloadHelper.RegExChartUrl);
			return regex.IsMatch(url);
		}
	}
}
