#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.XtraPrinting;
using System.Drawing.Printing;
using DevExpress.Compatibility.System.Drawing.Printing;
namespace DevExpress.DashboardCommon.Printing {
	public enum ItemSizeMode {
		None = 0,
		Stretch = 1,
		Zoom = 2,
		FitWidth = 3
	}
	public enum FilterStatePresentation { None, After, AfterAndSplitPage }
	public enum DashboardExportMode { SingleItem, EntireDashboard };
	public enum DashboardExportFormat { PDF, Image, Excel };
	public class PageOptions {
		public PaperOptions PaperOptions { get; set; }
		public ScalingOptions ScalingOptions { get; set; }
	}
	public class PaperOptions { 
		public PaperKind PaperKind { get; set; }
		public bool Landscape { get; set; }
		public bool UseCustomMargins { get; set; }
		public Margins CustomMargins { get; set; }
	}
	public class ScalingOptions {
		public float ScaleFactor { get; set; }
		public int AutoFitPageCount { get; set; }
	}
	public class AutomaticPageOptions {
		public bool AutoRotate { get; set; }
		public bool AutoFitToPageSize { get; set; }
	}
	public class ItemContentOptions {
		public ItemSizeMode SizeMode { get; set; }
		public ArrangerOptions ArrangerOptions { get; set; }
		public HeadersOptions HeadersOptions { get; set; }
	}
	public class ArrangerOptions {
		public bool AutoArrangeContent { get; set; }
	}
	public class HeadersOptions {
		public bool PrintHeadersOnEveryPage { get; set; }
	}
	public class DashboardReportOptions {
		public PageOptions PageOptions { get; set; }
		public AutomaticPageOptions AutoPageOptions { get; set; }
		public FormatOptions FormatOptions { get; set; }
		public DocumentContentOptions DocumentContentOptions { get; set; }
		public ItemContentOptions ItemContentOptions { get; set; }
		public string FileName { get; set; }
		public static DashboardReportOptions CreateDefault() {
			return new DashboardReportOptions {
				PageOptions = new PageOptions {
					PaperOptions = new PaperOptions(),
					ScalingOptions = new ScalingOptions()
				},
				AutoPageOptions = new AutomaticPageOptions(),
				DocumentContentOptions = new DocumentContentOptions(),
				ItemContentOptions = new ItemContentOptions {
					ArrangerOptions = new ArrangerOptions(),
					HeadersOptions = new HeadersOptions()
				},
				FormatOptions = new FormatOptions {
					ImageOptions = new ImageExportOptions(),
					PdfOptions = new PdfExportOptions(),
					ExcelOptions = new ExcelExportOptions()
				}
			};
		}
	}
	public class DocumentContentOptions {
		public bool ShowTitle { get; set; }
		public string Title { get; set; }
		public FilterStatePresentation FilterStatePresentation { get; set; }
	}
	public class FormatOptions {
		public DashboardExportFormat Format { get; set; }
		public PdfExportOptions PdfOptions { get; set; }
		public ImageExportOptions ImageOptions { get; set; }
		public ExcelExportOptions ExcelOptions { get; set; }
	}
}
