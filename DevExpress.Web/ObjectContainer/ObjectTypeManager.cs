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
using System.IO;
using System.Text;
namespace DevExpress.Web.Internal {
	public static class MimeTypeManager {
		private static Dictionary<string, string> fTypes = new Dictionary<string, string>();
		private static Dictionary<string, string> fFixableTypes = new Dictionary<string, string>();
		public static string GetType(string url, bool onlyFixable) {
			string ext = Path.GetExtension(url).ToLower();
			string type = "";
			if (onlyFixable) {
				if (fFixableTypes.ContainsKey(ext))
					type = fFixableTypes[ext];
			}
			else {
				if (fTypes.ContainsKey(ext))
					type = fTypes[ext];
			}
			return type;
		}
		static MimeTypeManager() {
			fTypes[".aif"] = "audio/x-aiff";
			fTypes[".aifc"] = "audio/x-aiff";
			fTypes[".aiff"] = "audio/x-aiff";
			fTypes[".au"] = "audio/basic";
			fTypes[".m4a"] = "audio/mpeg";
			fTypes[".mid"] = "audio/mid";
			fTypes[".midi"] = "audio/midi";
			fTypes[".mp2"] = "audio/mpeg";
			fTypes[".mp3"] = "audio/mp3";
			fTypes[".mpa"] = "audio/mpeg";
			fTypes[".rmi"] = "audio/mid";
			fTypes[".snd"] = "audio/basic";
			fTypes[".wav"] = "audio/x-wav";
			fTypes[".wax"] = "audio/x-ms-wax";
			fTypes[".wma"] = "audio/x-ms-wma";
			fTypes[".bmp"] = "image/bmp";
			fTypes[".gif"] = "image/gif";
			fTypes[".ico"] = "image/x-icon";
			fTypes[".jpe"] = "image/jpeg";
			fTypes[".jpeg"] = "image/jpeg";
			fTypes[".jpg"] = "image/jpeg";
			fTypes[".png"] = "image/png";
			fTypes[".asf"] = "video/x-ms-asf";
			fTypes[".asx"] = "video/x-ms-asf";
			fTypes[".avi"] = "video/x-msvideo";
			fTypes[".mp4"] = "video/mp4";
			fTypes[".m4v"] = "video/x-m4v";
			fTypes[".mpe"] = "video/mpeg";
			fTypes[".mpeg"] = "video/mpeg";
			fTypes[".mpg"] = "video/mpeg";
			fTypes[".wm"] = "video/x-ms-wm";
			fTypes[".wmv"] = "video/x-ms-wmv";
			fTypes[".wmx"] = "video/x-ms-wmx";
			fTypes[".wvx"] = "video/x-ms-wvx";
			fTypes[".mov"] = "video/quicktime";
			fTypes[".swf"] = "application/x-shockwave-flash";
			fTypes[".swfl"] = "application/x-shockwave-flash";
			fFixableTypes[".gif"] = "image/gif";
			fFixableTypes[".jpe"] = "image/jpeg";
			fFixableTypes[".jpeg"] = "image/jpeg";
			fFixableTypes[".jpg"] = "image/jpeg";
		}
	}
	public static class ObjectTypeManager {
		private static Dictionary<string, ObjectType> fTypes = new Dictionary<string, ObjectType>();
		public static ObjectType GetTypeByMimeType(string type) {
			ObjectType objectType = ObjectType.Image; 
			if (fTypes.ContainsKey(type))
				objectType = fTypes[type];
			switch (objectType) {
				case ObjectType.Html5Video:
					if(IsOldIE())
						objectType = ObjectType.Video;
					break;
				case ObjectType.Html5Audio:
					if(IsOldIE())
						objectType = ObjectType.Audio;
					break;
			}
			return objectType;
		}
		static bool IsOldIE() {
			return RenderUtils.Browser.IsIE && RenderUtils.Browser.MajorVersion <= 8;
		}
		public static ObjectType GetTypeByUrl(string url, bool onlyFixable) {
			return GetTypeByMimeType(MimeTypeManager.GetType(url, onlyFixable));
		}
		public static bool HasType(string url, bool onlyFixable) {
			return MimeTypeManager.GetType(url, onlyFixable) != "";
		}
		static ObjectTypeManager() {
			fTypes["audio/basic"] = ObjectType.Audio;
			fTypes["audio/mpeg"] = ObjectType.Audio;
			fTypes["audio/midi"] = ObjectType.Audio;
			fTypes["audio/mid"] = ObjectType.Audio;
			fTypes["audio/x-aiff"] = ObjectType.Audio;
			fTypes["audio/x-ms-wax"] = ObjectType.Audio;
			fTypes["audio/x-ms-wma"] = ObjectType.Audio;
			fTypes["audio/x-wav"] = ObjectType.Audio;
			fTypes["audio/mp3"] = ObjectType.Html5Audio;
			fTypes["video/mp4"] = ObjectType.Html5Video;
			fTypes["video/x-m4v"] = ObjectType.Video;
			fTypes["video/mpeg"] = ObjectType.Video;
			fTypes["video/x-ms-asf"] = ObjectType.Video;
			fTypes["video/x-ms-wm"] = ObjectType.Video;
			fTypes["video/x-ms-wma"] = ObjectType.Video;
			fTypes["video/x-ms-wmv"] = ObjectType.Video;
			fTypes["video/x-ms-wmx"] = ObjectType.Video;
			fTypes["video/x-ms-wvx"] = ObjectType.Video;
			fTypes["video/x-msvideo"] = ObjectType.Video;
			fTypes["image/bmp"] = ObjectType.Image;
			fTypes["image/gif"] = ObjectType.Image;
			fTypes["image/jpeg"] = ObjectType.Image;
			fTypes["image/png"] = ObjectType.Image;
			fTypes["image/x-icon"] = ObjectType.Image;
			fTypes["application/x-shockwave-flash"] = ObjectType.Flash;
			fTypes["video/quicktime"] = ObjectType.QuickTime;
		}
	}
}
