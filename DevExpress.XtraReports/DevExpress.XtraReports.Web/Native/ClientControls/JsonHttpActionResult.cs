#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using System.Net;
using DevExpress.XtraReports.Web.Native.ClientControls.DataContracts;
namespace DevExpress.XtraReports.Web.Native.ClientControls {
	public static class JsonHttpActionResult {
		public static JsonHttpActionResult<T> Create<T>(T result) {
			return new JsonHttpActionResult<T>(result);
		}
		public static JsonHttpActionResult<T> CreateFromError<T>(string errorMessage) {
			var wrapper = new JsonResultWrapper<T> {
				Success = false,
				Error = errorMessage
			};
			return new JsonHttpActionResult<T>(wrapper, HttpStatusCode.InternalServerError);
		}
		public static JsonHttpActionResult<object> CreateFromError(string errorMessage) {
			var wrapper = new JsonResultWrapper<object> {
				Success = false,
				Error = errorMessage
			};
			return new JsonHttpActionResult<object>(wrapper, HttpStatusCode.InternalServerError);
		}
	}
	public class JsonHttpActionResult<T> : HttpActionResultBase {
		public JsonResultWrapper<T> Result { get; private set; }
		public JsonHttpActionResult(T result)
			: this(new JsonResultWrapper<T> { Success = true, Result = result }) {
		}
		public JsonHttpActionResult(JsonResultWrapper<T> result)
			: this(result, DefaultStatus) {
		}
		public JsonHttpActionResult(JsonResultWrapper<T> result, HttpStatusCode status)
			: base("application/json", status) {
			Result = result;
		}
		protected override void WriteCore(IHttpResponse response) {
			var jsonResult = JsonSerializer.Stringify(Result, Result.Result as IKnownTypes);
			response.Write(jsonResult);
		}
	}
}
