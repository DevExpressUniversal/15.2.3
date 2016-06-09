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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Web;
using DevExpress.XtraReports.Web.Native.DocumentViewer.ParametersPanel;
using DevExpress.XtraReports.Web.Native.ParametersPanel;
namespace DevExpress.XtraReports.Web.Native.DocumentViewer {
	[ToolboxItem(false)]
	public class DocumentViewerReportParametersPanel : ReportParametersPanel {
		internal const string CascadeLookupsCallbackName = "cascadeLookups";
		protected override void PrepareControlHierarchyStyles() {
		}
		protected override void BeforeOnLoad() {
		}
		protected override void AfterOnLoad() {
		}
		protected override void InitParameterInfo(ASPxParameterInfo info) {
			base.InitParameterInfo(info);
			var autoCompleteBox = info.EditorInformation as ASPxAutoCompleteBoxBase;
			if(info.SupportCascadeLookup && autoCompleteBox != null) {
				info.UseCascadeLookup = true;
			}
		}
		#region callback
		IEnumerable<CallbackEventState> callbackEventHandlers;
		internal void RaiseEditorsCallbackEventCore(string eventArgument) {
			var callbackEventProcessor = ResolveCallbackEventProcessor();
			callbackEventHandlers = callbackEventProcessor.Process(eventArgument, ParametersInfo);
		}
		internal object GetCascadeLookupsCallbackResultCore() {
			return callbackEventHandlers.ToDictionary(
				x => x.ParameterPath,
				x => x.CallbackEventHandler.GetCallbackResult());
		}
		#endregion
	}
}
