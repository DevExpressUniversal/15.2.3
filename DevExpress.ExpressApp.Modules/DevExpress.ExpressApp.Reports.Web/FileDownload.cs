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
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Configuration;
namespace DevExpress.ExpressApp.Web.Core {
	public interface IFileDownloadSource {
		void WriteOutput(HttpResponse response);
	}
	public class FileDownloadHandler : IHttpHandler {
		private static Dictionary<string, IFileDownloadSource> registeredFiles = new Dictionary<string, IFileDownloadSource>();
		public const string DownloadPageUrl = "DownloadFile.aspx";
		public const string IdParamName = "Id";
		public bool IsReusable {
			get { return false; }
		}
		public void ProcessRequest(HttpContext context) {
			string sourceId = context.Request.Params[IdParamName];
			IFileDownloadSource source = null;
			if(!string.IsNullOrEmpty(sourceId) && registeredFiles.TryGetValue(sourceId, out source)) {
				registeredFiles.Remove(sourceId);
				source.WriteOutput(context.Response);
			}
		}
		public static string PutFileSourceAndGetDownloadUrl(IFileDownloadSource fileSource) {
			string sourceId = fileSource.GetHashCode().ToString();
			string urlResult = string.Format("{0}?{1}={2}", DownloadPageUrl, IdParamName, sourceId);
			registeredFiles.Add(sourceId, fileSource);
			return urlResult;
		}
	}
}
