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
using DevExpress.Web;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Web.Native.ParametersPanel {
	public class ReportParametersProvider : IParametersProvider {
		readonly XtraReport report;
		readonly ParametersEditorCreatorBase<ASPxEditBase> editorCreator;
		public ReportParametersProvider(XtraReport report, ParametersEditorCreatorBase<ASPxEditBase> editorCreator) {
			this.report = report;
			this.editorCreator = editorCreator;
		}
		#region IReportParametersProvider Members
		public ASPxParameterInfo[] GetParameters(Func<ParameterPath, ASPxEditBase, ASPxParameterInfo> map) {
			if(report == null) {
				return new ASPxParameterInfo[0];
			}
			report.ValidateScripts();
			var parametersCollector = new NestedParameterPathCollector();
			var parameterPaths = parametersCollector.EnumerateParameters(report);
			var parameterInfos = parameterPaths
				.Select(x => x.Parameter)
				.Select(ParameterInfoFactory.CreateWithoutEditor)
				.ToArray();
			((IReport)report).RaiseParametersRequestBeforeShow(parameterInfos);
			return parameterPaths
				.Select(x => map(x, CreateEditor(x)))
				.ToArray();
		}
		#endregion
		ASPxEditBase CreateEditor(ParameterPath parameterPath) {
			return editorCreator.CreateEditorByParameter(parameterPath.Parameter);
		}
	}
}
