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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Printing;
using DevExpress.XtraPrinting;
namespace DevExpress.DashboardWin.Native.Printing {
	public enum ExtendedScaleMode { None, UseScaleFactor, AutoFitToPageWidth }
	public class ExtendedScalingOptions {
		public ExtendedScaleMode ScaleMode { get; set; }
		public float ScaleFactor { get; set; }
		public int AutoFitPageCount { get; set; }
	}
	public class ExtendedReportOptions {
		public static ExtendedReportOptions Empty {
			get {
				return new ExtendedReportOptions {
					PaperOptions = new PaperOptions(),
					AutoPageOptions = new AutomaticPageOptions(),
					DocumentContentOptions = new DocumentContentOptions(),
					FormatOptions = new FormatOptions {
						ImageOptions = new ImageExportOptions(),
						PdfOptions = new PdfExportOptions(),
						ExcelOptions = new ExcelExportOptions()
					},
					ItemContentOptions = new ItemContentOptions {
						ArrangerOptions = new ArrangerOptions(),
						HeadersOptions = new HeadersOptions()
					},
					ScalingOptions = new ExtendedScalingOptions()
				};
			}
		}
		public AutomaticPageOptions AutoPageOptions { get; set; }
		public DocumentContentOptions DocumentContentOptions { get; set; }
		public string FileName { get; set; }
		public FormatOptions FormatOptions { get; set; }
		public ItemContentOptions ItemContentOptions { get; set; }
		public PaperOptions PaperOptions { get; set; }
		public ExtendedScalingOptions ScalingOptions { get; set; }
		public DashboardReportOptions GetOptions() {
			ScalingOptions scalingOpts = new ScalingOptions {
				ScaleFactor = 1.0f,
				AutoFitPageCount = 0
			};
			if(ScalingOptions.ScaleMode == ExtendedScaleMode.UseScaleFactor)
				scalingOpts.ScaleFactor = ScalingOptions.ScaleFactor;
			if(ScalingOptions.ScaleMode == ExtendedScaleMode.AutoFitToPageWidth)
				scalingOpts.AutoFitPageCount = ScalingOptions.AutoFitPageCount;
			return new DashboardReportOptions {
				AutoPageOptions = AutoPageOptions,
				DocumentContentOptions = DocumentContentOptions,
				FileName = FileName,
				FormatOptions = FormatOptions,
				ItemContentOptions = ItemContentOptions,
				PageOptions = new PageOptions {
					PaperOptions = PaperOptions,
					ScalingOptions = scalingOpts
				}
			};
		}
	}
}
