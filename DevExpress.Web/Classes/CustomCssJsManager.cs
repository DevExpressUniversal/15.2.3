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
using System.Text;
using System.Web;
using DevExpress.Web.Internal;
using System.IO;
namespace DevExpress.Web {
	public class CustomCssJsManager {
		const string
			CssFolderParameterName = "cssfolder",
			CssFileParameterName = "cssfile",
			JsFolderParameterName = "jsfolder",
			JsFileParameterName = "jsfile",
			JsFileSetParameterName = "jsfileset",
			CssMimeType = "text/css",
			JsMimeType = "text/javascript",
			CssFileExtension = ".css",
			JsFileExtension = ".js";
		static string GetPhysicalPath(string path) {
			if(UrlUtils.IsAbsolutePhysicalPath(path))
				throw new Exception(@"An absolute physical path is not allowed. Use a virtual path instead, e.g., ""~\scripts\file1.js"".");
			if(!UrlUtils.IsAppRelativePath(path))
				path = "~/" + path;
			HttpRequest request = HttpUtils.GetRequest();
			string physicalPath = request.MapPath(path);
			if(!physicalPath.StartsWith(request.PhysicalApplicationPath))
				throw new Exception(@"Cannot use the specified path to exit above the application directory");
			return physicalPath;
		}
		static void WriteFileToResponse(string fileName, string allowedExtension) {
			if(Path.GetExtension(fileName).ToLowerInvariant() != allowedExtension) return;
			string content = File.ReadAllText(fileName);
			HttpResponse response = HttpUtils.GetResponse();
			response.Write(content);
			response.AddFileDependency(fileName);
		}
		static void WriteFolderToResponse(string directoryPath, string fileExtension) {
			const char AllFilesMaskToken = '*';
			string physicalPath = GetPhysicalPath(directoryPath);
			string[] fileNames = Directory.GetFiles(physicalPath, AllFilesMaskToken + fileExtension);
			foreach(string fileName in fileNames)
				WriteFileToResponse(fileName, fileExtension);
			HttpUtils.GetResponse().AddFileDependency(physicalPath);
		}
		static void WriteJsFileSetToResponse(string fileSetParam) {
			const char FileNameSeparator = ';';
			string[] fileNames = fileSetParam.Split(FileNameSeparator);
			foreach(string fileName in fileNames)
				WriteFileToResponse(GetPhysicalPath(fileName), JsFileExtension);
		}
		static void SetCaching() {
			HttpResponse response = HttpUtils.GetResponse();
			response.Cache.VaryByParams[CssFolderParameterName] = true;
			response.Cache.VaryByParams[CssFileParameterName] = true;
			response.Cache.VaryByParams[JsFolderParameterName] = true;
			response.Cache.VaryByParams[JsFileParameterName] = true;
			response.Cache.VaryByParams[JsFileSetParameterName] = true;
			response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
			response.Cache.SetOmitVaryStar(true);
			response.Cache.SetLastModifiedFromFileDependencies();
		}
		static void MakeResponse() {
			HttpRequest request = HttpUtils.GetRequest();
			HttpResponse response = HttpUtils.GetResponse();
			string cssFolderParameter = request.QueryString[CssFolderParameterName];
			if(!string.IsNullOrEmpty(cssFolderParameter)) {
				response.ContentType = CssMimeType;
				WriteFolderToResponse(cssFolderParameter, CssFileExtension);
			}
			string cssFileParameter = request.QueryString[CssFileParameterName];
			if(!string.IsNullOrEmpty(cssFileParameter)) {
				response.ContentType = CssMimeType;
				WriteFileToResponse(GetPhysicalPath(cssFileParameter), CssFileExtension);
			}
			string jsFolderParameter = request.QueryString[JsFolderParameterName];
			if(!string.IsNullOrEmpty(jsFolderParameter)) {
				response.ContentType = JsMimeType;
				WriteFolderToResponse(jsFolderParameter, JsFileExtension);
			}
			string jsFileParameter = request.QueryString[JsFileParameterName];
			if(!string.IsNullOrEmpty(jsFileParameter)) {
				response.ContentType = JsMimeType;
				WriteFileToResponse(GetPhysicalPath(jsFileParameter), JsFileExtension);
			}
			string jsFileSetParameter = request.QueryString[JsFileSetParameterName];
			if(!string.IsNullOrEmpty(jsFileSetParameter)) {
				response.ContentType = JsMimeType;
				WriteJsFileSetToResponse(jsFileSetParameter);
			}
		}
		public static void ProcessRequest() {
			HttpResponse response = HttpUtils.GetResponse();
			response.Clear();
			try {
				MakeResponse();
				SetCaching();
				if(ConfigurationSettings.EnableResourceCompression)
					HttpUtils.MakeResponseCompressed(true);
			} catch(Exception ex) {
				response.Clear();
				response.StatusCode = 500;
				response.StatusDescription = ex.Message;
			}
		}
	}
}
