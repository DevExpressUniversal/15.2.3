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
using DevExpress.XtraPrinting.Native;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraPrinting.HtmlExport.Controls;
#if SL
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.XtraPrinting.Export {
#if !SL && !DXPORTABLE
	public interface ITableExportProvider {
		ExportContext ExportContext { get; }
		BrickViewData CurrentData { get; }
		void SetCellText(object textValue, string hyperLink);
		void SetCellImage(Image image, string imageSrc, ImageSizeMode sizeMode, ImageAlignment align, Rectangle bounds, Size imageSize, PaddingInfo padding, string hyperLink);
		void SetCellShape(Color lineColor, DevExpress.XtraReports.UI.LineDirection lineDirection, System.Drawing.Drawing2D.DashStyle lineStyle, float lineWidth, PaddingInfo padding, string hyperlink);
	}
	public interface IHtmlExportProvider : ITableExportProvider {
		HtmlExportContext HtmlExportContext { get; }
		void SetNavigationUrl(VisualBrick brick);
		void SetAnchor(string anchorName);
		void RaiseHtmlItemCreated(VisualBrick brick);
		DXHtmlContainerControl CurrentCell { get; }
		Rectangle CurrentCellBounds { get; }
	}
	public interface IRtfExportProvider : ITableExportProvider {
		RtfExportContext RtfExportContext { get; }
		void SetContent(string content);
		void SetAnchor(string anchorName);
		void SetPageInfo(PageInfo pageInfo, int startPageNumber, string textValue, string hyperLink);
	}
	public interface IXlsExportProvider : ITableExportProvider {
		XlsExportContext XlsExportContext { get; }
		void SetCellData(object data);
	}
#endif
	public interface ITableCell {
		bool ShouldApplyPadding { get; }
		string FormatString { get; }
		string XlsxFormatString { get; }
		object TextValue { get; }
		BrickModifier Modifier { get; }
		DefaultBoolean XlsExportNativeFormat { get; }
		string Url { get; }
	}
}
#if DXPORTABLE
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	class EnsureNamespaceExistsInAssembly { }
}
#endif
