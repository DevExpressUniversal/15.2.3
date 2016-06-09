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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using DevExpress.Web.ASPxHtmlEditor.Rendering;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	class HelperDownloadedFile : HelperPostedFileBase {
		readonly byte[] contentBytes;
		public HelperDownloadedFile(string fileName, string contentType, Stream stream) :
			base(fileName, stream.Length, contentType, null) {
			using(Stream str = stream)
				this.contentBytes = CommonUtils.GetBytesFromStream(str);
		}
		protected override Stream GetInputStreamInternal() {
			return new MemoryStream(this.contentBytes);
		}
		public override void SaveAs(string fileName, bool allowOverwrite) {
			using(FileStream fileStream = File.Open(fileName, allowOverwrite ? FileMode.OpenOrCreate : FileMode.CreateNew))
				fileStream.Write(this.contentBytes, 0, this.contentBytes.Length);
		}
	}
	class HelperFileUploadCompleteEventArgs : FileUploadCompleteEventArgs {
		public HelperFileUploadCompleteEventArgs(string fileName, string contentType, Stream stream)
			: base("", "", CreateUploadedFile(fileName, contentType, stream)) {
		}
		static UploadedFile CreateUploadedFile(string fileName, string contentType, Stream stream) {
			return new UploadedFile(new HelperDownloadedFile(fileName, contentType, stream));
		}
	}
	class HelperFileSavingEventArgs : FileSavingEventArgs {
		public HelperFileSavingEventArgs(string fileName, WebResponse response)
			: base(new HelperFileUploadCompleteEventArgs(fileName, response.ContentType, GetResponseStream(response))) {
		}
		static Stream GetResponseStream(WebResponse response) {
			using(Stream stream = response.GetResponseStream()) {
				MemoryStream result = new MemoryStream();
				stream.CopyTo(result);
				return result;
			}
		}
	}
	public abstract class DownloadHelperBase {
		protected ASPxHtmlEditor HtmlEditor { get; private set; }
		protected abstract string CallbackPrefix { get; }
		protected abstract string SuccessPrefix { get; }
		protected abstract string ErrorPrefix { get; }
		protected abstract string DefaultFileExtension { get; }
		protected abstract ASPxHtmlEditorUploadSettingsBase UploadSettings { get; }
		protected string DownloadFolder { get { return UploadSettings.UploadFolder; } }
		protected ASPxHtmlEditorUploadValidationSettingsBase ValidationSettings { get { return UploadSettings.ValidationSettingsInternal; } }
		public DownloadHelperBase(ASPxHtmlEditor htmlEditor) {
			HtmlEditor = htmlEditor;
		}
		public string SaveFile(string url) {
			return SaveFile(url, "");
		}
		public string SaveFile(string url, string possibleFileName) {
			try {
				using(WebResponse response = WebRequest.Create(GetPreparedUrl(url)).GetResponse()) {
					Validate(response, DownloadFolder);
					return GetSuccessfullResult(SaveDownloadedFile(response, possibleFileName));
				}
			} catch(UriFormatException) {
				return GetInvalidUrlErrorResult();
			} catch(Exception e) {
				return GetInternalErrorResult(e);
			}
		}
		string GetPreparedUrl(string url) {
			HttpRequest currentRequest = HttpContext.Current.Request;
			if(!string.IsNullOrEmpty(url) && url.StartsWith("//")) 
				url = string.Format("{0}:{1}", currentRequest.Url.Scheme, url);
			if(!UrlUtils.IsAbsoluteUrl(url)) {
				UriBuilder uriBuilder = new UriBuilder(currentRequest.Url);
				uriBuilder.Query = "";
				uriBuilder.Path = UrlUtils.Combine(UrlUtils.AppDomainAppVirtualPathString, currentRequest.Url.AbsolutePath, url);
				url = uriBuilder.Uri.AbsoluteUri;
			}
			return url;
		}
		protected abstract void RaiseFileSavingEvent(FileSavingEventArgs fileSavingArgs);
		string SaveDownloadedFile(WebResponse response, string possibleFileName) {
			string filePath = GetFilePath(GetFileName(response, possibleFileName));
			FileSavingEventArgs fileSavingArgs = new HelperFileSavingEventArgs(Path.GetFileName(filePath), response);
			RaiseFileSavingEvent(fileSavingArgs);
			if(!fileSavingArgs.IsValid)
				throw new Exception(fileSavingArgs.ErrorText);
			if(fileSavingArgs.Cancel)
				return fileSavingArgs.SavedFileUrl;
			SaveFile(fileSavingArgs.UploadedFile, filePath);
			string savedUrl = UrlUtils.MakeVirtualPathAppAbsolute(UrlUtils.GetAppRelativePath(filePath));
			return UrlProcessor.PrepareUrl(HtmlEditor, savedUrl, true);
		}
		protected internal string GetSuccessfullResult(string savedUrl) {
			return GetResult(SuccessPrefix + RenderUtils.CallBackSeparator + RemoveInvalidUrlCharacters(savedUrl));
		}
		protected internal string GetInvalidUrlErrorResult() {
			return GetErrorResult(ValidationSettings.InvalidUrlErrorText);
		}
		protected internal string GetInternalErrorResult(Exception e) {
			return GetErrorResult((e.InnerException ?? e).Message);
		}
		protected internal string GetErrorResult(string message) {
			return GetResult(ErrorPrefix + RenderUtils.CallBackSeparator + message);
		}
		protected internal string GetResult(string suffix) {
			return CallbackPrefix + RenderUtils.CallBackSeparator + suffix;
		}
		protected virtual void SaveFile(UploadedFile uploadedFile, string filePath) {
			uploadedFile.SaveAs(filePath);
		}
		protected string GetFilePath(string fileName) {
			return GetPhysicalFileName(DownloadFolder, fileName);
		}
		protected string GetFileName(WebResponse response, string possibleFileName) {
			return GetFileNameWithoutExtension(response.ResponseUri.AbsoluteUri, possibleFileName) + GetFileExtension(response);
		}
		protected string GetFileExtension(WebResponse response) {
			return GetFileExtension(response.ContentType);
		}
		protected internal string GetFileExtension(string contentType) {
			#region types <-> extensions
			switch(contentType) {
				case "application/octet-stream":
					return DefaultFileExtension;
				case "video/mpeg":
					return ".m1v";
				case "video/quicktime":
					return ".mov";
				case "video/x-matroska":
					return ".mkv";
				case "video/x-ms-wmv":
					return ".wmv";
				case "video/x-flv":
					return ".flv";
				case "audio/mpeg":
					return ".mp3";
				case "audio/vorbis":
					return ".ogg";
				case "audio/vnd.rn-realaudio":
					return ".ra";
				case "audio/vnd.wave":
				case "audio/x-wav":
					return ".wav";
				case "application/x-shockwave-flash":
					return ".swf";
				case "image/pjpeg":
					return ".jpeg";
				case "image/svg+xml":
					return ".svg";
				default:
					return GetType(contentType);
			}
			#endregion
		}
		private string GetType(string contentType) {
			string ret = "";
			string[] parts = contentType.Split('/');
			if(parts.Length > 1) {
				ret = parts[1];
				parts = parts[1].Split(';');
			}
			return "." + (parts.Length > 1 ? parts[0] : ret).ToLower();
		}
		protected bool IsAllowedContentType(string contentType) {
#pragma warning disable 618
			List<string> contentTypes = new List<string>(ValidationSettings.AllowedContentTypes);
#pragma warning restore 618
			if(contentTypes.Count > 0)
				return contentTypes.Contains(contentType);
			return true;
		}
		protected bool IsAllowedContentSize(long size) {
			if(ValidationSettings.MaxFileSize > 0)
				return size <= ValidationSettings.MaxFileSize;
			return true;
		}
		protected void Validate(WebResponse response, string folder) {
			if(!IsAllowedContentType(response.ContentType))
				throw new UriFormatException();
			if(!IsAllowedContentSize(response.ContentLength))
				throw new Exception(ValidationSettings.MaxFileSizeErrorText);
		}
		public static string GetPhysicalFileName(string folder, string fileName) {
			fileName = fileName.Replace("+", "[plus]");
			string fileNameWithoutExtension = UrlUtils.GetPhysicalPath(folder) + Path.GetFileNameWithoutExtension(fileName);
			string extension = Path.GetExtension(fileName);
			string result = fileNameWithoutExtension;
			int counter = 1;
			while(File.Exists(result + extension))
				result = string.Format("{0}({1})", fileNameWithoutExtension, counter++);
			return result + extension;
		}
		public static string RemoveInvalidUrlCharacters(string str) {
			return HttpUtility.UrlPathEncode(str);
		}
		protected internal string GetFileNameWithoutExtension(string url, string possibleFileName) {
			if(!string.IsNullOrEmpty(possibleFileName))
				return possibleFileName;
			Regex reg = new Regex(@"\A([A-Za-z0-9'~`!@#$%&amp;^_+=\(\){},\-\[\]\;])+?([ A-Za-z0-9'~` !@#$%&amp;^_+=\(\){},\-\[\];]|([.]))*?(?(3)(([ A-Za-z0-9'~`!@#$ %&amp;^_+=\(\){},\-\[\]\;]*?)([A-Za-z0-9'~`!@#$%&amp;^_+=\(\){},\-\[ \];])+\z)|(\z))");
			if(reg.Match(Path.GetFileName(url)).Length > 0)
				return Path.GetFileNameWithoutExtension(url);
			else
				return Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
		}
	}
	public class FlashDownloadHelper : DownloadHelperBase {
		protected override string CallbackPrefix { get { return ASPxHtmlEditorCallbackReader.SaveFlashToServerCallbackPrefix; } }
		protected override string ErrorPrefix { get { return ASPxHtmlEditorCallbackReader.SaveFlashToServerErrorCallbackPrefix; } }
		protected override string SuccessPrefix { get { return ASPxHtmlEditorCallbackReader.SaveFlashToServerNewUrlCallbackPrefix; } }
		protected override string DefaultFileExtension { get { return ".swf"; } }
		protected override ASPxHtmlEditorUploadSettingsBase UploadSettings { get { return HtmlEditor.SettingsDialogs.InsertFlashDialog.SettingsFlashUpload; } }
		public FlashDownloadHelper(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
		}
		protected override void RaiseFileSavingEvent(FileSavingEventArgs fileSavingArgs) {
			HtmlEditor.RaiseFlashFileSaving(fileSavingArgs);
		}
	}
	public class AudioDownloadHelper : DownloadHelperBase {
		protected override string CallbackPrefix { get { return ASPxHtmlEditorCallbackReader.SaveAudioToServerCallbackPrefix; } }
		protected override string ErrorPrefix { get { return ASPxHtmlEditorCallbackReader.SaveAudioToServerErrorCallbackPrefix; } }
		protected override string SuccessPrefix { get { return ASPxHtmlEditorCallbackReader.SaveAudioToServerNewUrlCallbackPrefix; } }
		protected override string DefaultFileExtension { get { return ".mp3"; } }
		protected override ASPxHtmlEditorUploadSettingsBase UploadSettings { get { return HtmlEditor.SettingsDialogs.InsertAudioDialog.SettingsAudioUpload; } }
		public AudioDownloadHelper(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
		}
		protected override void RaiseFileSavingEvent(FileSavingEventArgs fileSavingArgs) {
			HtmlEditor.RaiseAudioFileSaving(fileSavingArgs);
		}
	}
	public class VideoDownloadHelper : DownloadHelperBase {
		protected override string CallbackPrefix { get { return ASPxHtmlEditorCallbackReader.SaveVideoToServerCallbackPrefix; } }
		protected override string ErrorPrefix { get { return ASPxHtmlEditorCallbackReader.SaveVideoToServerErrorCallbackPrefix; } }
		protected override string SuccessPrefix { get { return ASPxHtmlEditorCallbackReader.SaveVideoToServerNewUrlCallbackPrefix; } }
		protected override string DefaultFileExtension { get { return ".mp4"; } }
		protected override ASPxHtmlEditorUploadSettingsBase UploadSettings { get { return HtmlEditor.SettingsDialogs.InsertVideoDialog.SettingsVideoUpload; } }
		public VideoDownloadHelper(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
		}
		protected override void RaiseFileSavingEvent(FileSavingEventArgs fileSavingArgs) {
			HtmlEditor.RaiseVideoFileSaving(fileSavingArgs);
		}
	}
	public class ImageDownloadHelper : DownloadHelperBase {
		protected override string CallbackPrefix { get { return ASPxHtmlEditorCallbackReader.SaveImageToServerCallbackPrefix; } }
		protected override string ErrorPrefix { get { return ASPxHtmlEditorCallbackReader.SaveImageToServerErrorCallbackPrefix; } }
		protected override string SuccessPrefix { get { return ASPxHtmlEditorCallbackReader.SaveImageToServerNewUrlCallbackPrefix; } }
		protected override string DefaultFileExtension { get { return ".jpg"; } }
		protected override ASPxHtmlEditorUploadSettingsBase UploadSettings { get { return HtmlEditor.SettingsDialogs.InsertImageDialog.SettingsImageUpload; } }
		public ImageDownloadHelper(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
		}
		protected override void RaiseFileSavingEvent(FileSavingEventArgs fileSavingArgs) {
			HtmlEditor.RaiseImageFileSaving(fileSavingArgs);
		}
	}
	public class ImageThumbnailDownloadHelper : ImageDownloadHelper {
		protected Size ThumbnailSize { get; private set; }
		public ImageThumbnailDownloadHelper(ASPxHtmlEditor htmlEditor, int thumbnailWidth, int thumbnailHeight)
			: base(htmlEditor) {
			ThumbnailSize = new Size(thumbnailWidth, thumbnailHeight);
		}
		protected override void SaveFile(UploadedFile file, string fileName) {
			using(Stream stream = file.FileContent) {
				Image image = Image.FromStream(stream);
				IImageResizer imageResizer = ImageUtilsHelper.GetImageResizer(ImageSizeMode.FitProportional);
				ThumbnailInfo thumbnailInfo = imageResizer.GetThumbnailInfo(image, ThumbnailSize, false);
				ImageUtils.SaveImage(image, thumbnailInfo, fileName, null);
			}
		}
		protected override void RaiseFileSavingEvent(FileSavingEventArgs fileSavingArgs) { }
	}
}
