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

using System;
using System.Linq;
using System.Web;
using DevExpress.Web.WebClientUIControl.Internal;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using DevExpress.XtraReports.Web.Native.ClientControls;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native {
	public static class ReportDesignerInputLoader {
		public static ReportDesignerInput FromString(string argument) {
			if(string.IsNullOrEmpty(argument)) {
				return null;
			}
			var arguments = argument.Split(new[] { '&' }, 2)
				.Select(x => x.Split(new[] { '=' }, 2))
				.ToDictionary(x => x[0], x => x.Length > 1 ? x[1] : "");
			string reportLayoutJson;
			if(!arguments.TryGetValue("reportLayout", out reportLayoutJson)) {
				throw new InvalidOperationException("There is no 'reportLayout' argument");
			}
			string parameter;
			arguments.TryGetValue("arg", out parameter);
			string reportLayoutJsonDecoded = HttpUtility.UrlDecode(reportLayoutJson);
			byte[] reportLayoutXml = ReportLayoutJsonSerializer.LoadFromJsonAndReturnXml(reportLayoutJsonDecoded);
			return new ReportDesignerInput(reportLayoutXml, parameter);
		}
	}
}
