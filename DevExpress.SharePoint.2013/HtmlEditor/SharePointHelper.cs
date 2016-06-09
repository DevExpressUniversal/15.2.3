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

using Microsoft.SharePoint;
using DevExpress.Web;
using System.Web.UI;
using System.Collections.Generic;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using System.Collections;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint.WebPartPages;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using System.Web;
using System.IO;
using System;
using Microsoft.SharePoint.Administration;
namespace DevExpress.SharePoint.Internal {
	public static class SharePointHelper {
		public static bool IsFeatureEnabled(string featureGUID, HttpContext context) {
			Guid guid = new Guid(featureGUID);
			SPFeature feature = GetFeature(guid, context);
			return ((feature != null) && (feature.Definition != null));
		}
		public static SPFeature GetFeature(Guid guid, HttpContext context) {
			SPWeb webSite = SPControl.GetContextWeb(context);
			SPSite site = SPControl.GetContextSite(context);
			SPFeature feature = webSite.Features[guid];
			if ((feature == null) || (feature.Definition == null) && (webSite.ParentWeb != null))
				feature = webSite.ParentWeb.Features[guid];
			if ((feature == null) || (feature.Definition == null))
				feature = site.Features[guid];
			if ((feature == null) || (feature.Definition == null))
				feature = site.WebApplication.Features[guid];
			if ((feature != null) && (feature.Definition != null))
				return feature;
			return SPWebService.ContentService.Features[guid];
		}
		public static bool DoesUserHavePermissionToUpload(string folderPath, HttpContext context) {
			bool ret = false;
			SPSite contextSite = SPControl.GetContextSite(context);
			SPFolder folder = GetSPFolderFromPath(folderPath, contextSite);
			if (folder != null) {
				SPList list = folder.ParentWeb.Lists[folder.ParentListId];
				ret = list.DoesUserHavePermissions(SPBasePermissions.EmptyMask | SPBasePermissions.AddListItems);
			}
			return ret;
		}
		public static string SaveFileToSPFolder(HttpContext context, byte[] fileContent, string name, string uploadFolder) {
			string fileName = "";
			SPSite contextSite = SPControl.GetContextSite(context);
			SPFolder folder = GetSPFolderFromPath(uploadFolder.Substring(0, uploadFolder.LastIndexOf("/")), contextSite);
			if (folder != null) {
				StoreFileInSPCore(folder, name, fileContent);
				fileName = name;
			}
			return fileName;
		}
		private static void StoreFileInSPCore(SPFolder folder, string name, byte[] content) {
			SPWeb parentWeb = folder.ParentWeb;
			parentWeb.AllowUnsafeUpdates = true;
			try {
				SPFile file = folder.Files[name];
				file.CheckOut();
				file.SaveBinary(content);
				file.CheckIn("updated by SPxHtmlEditor");
			} catch (ArgumentException) {
				folder.Files.Add(name, content);
			}
			PublishFileChanges(folder, name);
		}
		private static void PublishFileChanges(SPFolder folder, string name) {
			SPList list = folder.ParentWeb.Lists[folder.ParentListId];
			try {
				if (folder.Files[name].CheckOutStatus != SPFile.SPCheckOutStatus.None)
					folder.Files[name].CheckIn("updated by SPxHtmlEditor");
				if (list.EnableModeration)
					folder.Files[name].Approve("aproved by SPxHtmlEditor");
				if (list.EnableMinorVersions)
					folder.Files[name].Publish("published by SPxHtmlEditor");
			} catch (Exception) { }
		}
		private static  SPFolder GetSPFolderFromPath(string path, SPSite contextSite) {
			SPFolder folder = null;
			try {
				if (path.StartsWith(contextSite.ServerRelativeUrl))
					path = path.Remove(0, contextSite.ServerRelativeUrl.Length);
				if (path.EndsWith("/"))
					path = path.Remove(path.Length - 1);
				string[] strArray = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				SPWeb web = contextSite.OpenWeb();
				string serverRelativeUrl = web.ServerRelativeUrl;
				if (!serverRelativeUrl.EndsWith("/"))
					serverRelativeUrl = serverRelativeUrl + "/";
				int index = 0;
				while (web.Exists && (index < strArray.Length)) {
					serverRelativeUrl = serverRelativeUrl + strArray[index++] + "/";
					web = contextSite.OpenWeb(serverRelativeUrl);
				}
				index--;
				serverRelativeUrl = serverRelativeUrl.Remove((serverRelativeUrl.Length - 1) - strArray[index].Length);
				web.Dispose();
				web = contextSite.OpenWeb(serverRelativeUrl);
				web.AllowUnsafeUpdates = true;
				serverRelativeUrl = "";
				for (int i = index; i < strArray.Length; i++)
					serverRelativeUrl = serverRelativeUrl + strArray[i] + "/";
				if (serverRelativeUrl.EndsWith("/"))
					serverRelativeUrl = serverRelativeUrl.Remove(serverRelativeUrl.Length - 1);
				folder = web.GetFolder(serverRelativeUrl);
				if ((folder != null) && !folder.Exists) folder = null;
				web.Dispose();
			} catch (Exception) { }
			return folder;
		}
	}
}
