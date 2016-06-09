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
using System.ComponentModel;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports {
	public interface IReport : IDocumentSource, IComponent, IServiceProvider, IExtensionsProvider {
		void StopPageBuilding();
		Watermark Watermark { get; }
		[EditorBrowsable(EditorBrowsableState.Never), DXHelpExclude(true)]
		bool IsDisposed { get; }
		[EditorBrowsable(EditorBrowsableState.Never), DXHelpExclude(true)]
		bool IsMetric { get; }
		[EditorBrowsable(EditorBrowsableState.Never), DXHelpExclude(true)]
		bool ShowPreviewMarginLines { get; }
		[EditorBrowsable(EditorBrowsableState.Never), DXHelpExclude(true)]
		void ApplyPageSettings(XtraPageSettingsBase destSettings);
		[EditorBrowsable(EditorBrowsableState.Never), DXHelpExclude(true)]
		void UpdatePageSettings(XtraPageSettingsBase sourceSettings, string paperName);
		[EditorBrowsable(EditorBrowsableState.Never), DXHelpExclude(true)]
		void RaiseParametersRequestBeforeShow(IList<ParameterInfo> parametersInfo);
		[EditorBrowsable(EditorBrowsableState.Never), DXHelpExclude(true)]
		void RaiseParametersRequestValueChanged(IList<ParameterInfo> parametersInfo, ParameterInfo changedParameterInfo);
		[EditorBrowsable(EditorBrowsableState.Never), DXHelpExclude(true)]
		void RaiseParametersRequestSubmit(IList<ParameterInfo> parametersInfo, bool createDocument);
		[EditorBrowsable(EditorBrowsableState.Never), DXHelpExclude(true)]
		IReportPrintTool PrintTool { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never), DXHelpExclude(true)]
		bool RequestParameters { get; }
		[EditorBrowsable(EditorBrowsableState.Never), DXHelpExclude(true)]
		void CollectParameters(IList<Parameter> list, Predicate<Parameter> condition);
		[EditorBrowsable(EditorBrowsableState.Never), DXHelpExclude(true)]
		void ReleasePrintingSystem();
		[EditorBrowsable(EditorBrowsableState.Never), DXHelpExclude(true)]
		event EventHandler<ParametersRequestEventArgs> ParametersRequestSubmit;
	}
}
