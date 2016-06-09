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

using System.CodeDom.Compiler;
using System.Collections.Generic;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.ReportDesigner.Native.DataContracts;
using System.IO;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native.Services {
	public class ReportScriptsService : IReportScriptsService {
		public ScriptsErrorModel[] Validate(string reportLayout) {
			XtraReport report = ReportLayoutJsonSerializer.CreateReportFromJson(reportLayout);
			var errors = new List<ScriptsErrorModel>();
			foreach(CompilerError error in report.ValidateScripts()) {
				errors.Add(new ScriptsErrorModel() {
					Column = error.Column,
					ErrorNumber = error.ErrorNumber,
					ErrorText = error.ErrorText,
					IsWarning = error.IsWarning,
					Line = error.Line
				});
			}
			return errors.ToArray();
		}
		public ScriptsCompleteModel[] GetCompleters(ReportScriptsIntellisenseContract model) {
			XtraReport report = ReportLayoutJsonSerializer.CreateReportFromJson(model.Report);
			var codeDomReport = "";
			using(var stream = new MemoryStream()) {
				report.SaveLayout(stream, false);
				stream.Position = 0;
				using(var reader = new StreamReader(stream))
					codeDomReport = reader.ReadToEnd();
			}
			var completers = new List<ScriptsCompleteModel>();
			var context = new ReportIntelliSenseContext(codeDomReport, model.Script, model.Line, model.Column);
			foreach(var completion in IntelliSense.Instance.CollectCompletions(context))
				completers.Add(new ScriptsCompleteModel() {
					Meta = completion.Meta,
					Name = completion.Name,
					Value = completion.Value
				});
			return completers.ToArray();
		}
	}
}
