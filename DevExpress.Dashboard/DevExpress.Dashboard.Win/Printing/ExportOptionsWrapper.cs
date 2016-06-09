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

using System;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Printing;
namespace DevExpress.DashboardWin.Native.Printing {
	public static class ExportOptionsWrapperKeys {
		public const string
			Landscape = "Landscape",
			PaperKind = "PaperKind",
			ScaleMode = "ScaleMode",
			ScaleFactor = "ScaleFactor",
			AutoFitPageCount = "AutoFitPageCount",
			AutoRotate = "AutoRotate",
			AutoFitToPageSize = "AutoFitToPageSize",
			ShowTitle = "ShowTitle",
			Title = "Title",
			FilterStatePresentation = "FilterStatePresentation",
			ExcelFormat = "ExcelFormat",
			CsvValueSeparator = "CsvValueSeparator",
			ImageFormat = "ImageFormat",
			ImageResolution = "Resolution",
			PrintHeadersOnEveryPage = "PrintHeadersOnEveryPage",
			ItemSizeMode = "ItemSizeMode",
			AutoArrangeContent = "AutoArrangeContent";
	}
	public class ExportOptionsWrapper {
		readonly ExtendedReportOptions opts;
		public ExportOptionsWrapper(ExtendedReportOptions opts) {
			this.opts = opts;
		}
		public object Get(string key) {
			object value = null;
			switch(key) {
				case ExportOptionsWrapperKeys.Landscape:
					value = opts.PaperOptions.Landscape;
					break;
				case ExportOptionsWrapperKeys.PaperKind:
					value = opts.PaperOptions.PaperKind;
					break;
				case ExportOptionsWrapperKeys.ScaleMode:
					value = opts.ScalingOptions.ScaleMode;
					break;
				case ExportOptionsWrapperKeys.ScaleFactor:
					value = opts.ScalingOptions.ScaleFactor;
					break;
				case ExportOptionsWrapperKeys.AutoFitPageCount:
					value = opts.ScalingOptions.AutoFitPageCount;
					break;
				case ExportOptionsWrapperKeys.AutoRotate:
					value = opts.AutoPageOptions.AutoRotate;
					break;
				case ExportOptionsWrapperKeys.AutoFitToPageSize:
					value = opts.AutoPageOptions.AutoFitToPageSize;
					break;
				case ExportOptionsWrapperKeys.ShowTitle:
					value = opts.DocumentContentOptions.ShowTitle;
					break;
				case ExportOptionsWrapperKeys.Title:
					value = opts.DocumentContentOptions.Title;
					break;
				case ExportOptionsWrapperKeys.FilterStatePresentation:
					value = opts.DocumentContentOptions.FilterStatePresentation;
					break;
				case ExportOptionsWrapperKeys.ExcelFormat:
					value = opts.FormatOptions.ExcelOptions.Format;
					break;
				case ExportOptionsWrapperKeys.CsvValueSeparator:
					value = opts.FormatOptions.ExcelOptions.CsvValueSeparator;
					break;
				case ExportOptionsWrapperKeys.ImageFormat:
					value = opts.FormatOptions.ImageOptions.Format;
					break;
				case ExportOptionsWrapperKeys.ImageResolution:
					value = opts.FormatOptions.ImageOptions.Resolution;
					break;
				case ExportOptionsWrapperKeys.PrintHeadersOnEveryPage:
					value = opts.ItemContentOptions.HeadersOptions.PrintHeadersOnEveryPage;
					break;
				case ExportOptionsWrapperKeys.ItemSizeMode:
					value = opts.ItemContentOptions.SizeMode;
					break;
				case ExportOptionsWrapperKeys.AutoArrangeContent:
					value = opts.ItemContentOptions.ArrangerOptions.AutoArrangeContent;
					break;
				default:
					throw new Exception("Incorrect key");
			}
			return value;
		}
		public void Set(string key, object value) {
			switch(key) {
				case ExportOptionsWrapperKeys.Landscape:
					opts.PaperOptions.Landscape = (bool)value;
					break;
				case ExportOptionsWrapperKeys.PaperKind:
					opts.PaperOptions.PaperKind = (PaperKind)value;
					break;
				case ExportOptionsWrapperKeys.ScaleMode:
					opts.ScalingOptions.ScaleMode = (ExtendedScaleMode)value;
					break;
				case ExportOptionsWrapperKeys.ScaleFactor:
					opts.ScalingOptions.ScaleFactor = (float)value;
					break;
				case ExportOptionsWrapperKeys.AutoFitPageCount:
					opts.ScalingOptions.AutoFitPageCount = (int)value;
					break;
				case ExportOptionsWrapperKeys.AutoRotate:
					opts.AutoPageOptions.AutoRotate = (bool)value;
					break;
				case ExportOptionsWrapperKeys.AutoFitToPageSize:
					opts.AutoPageOptions.AutoFitToPageSize = (bool)value;
					break;
				case ExportOptionsWrapperKeys.ShowTitle:
					opts.DocumentContentOptions.ShowTitle = (bool)value;
					break;
				case ExportOptionsWrapperKeys.Title:
					opts.DocumentContentOptions.Title = (string)value;
					break;
				case ExportOptionsWrapperKeys.FilterStatePresentation:
					opts.DocumentContentOptions.FilterStatePresentation = (FilterStatePresentation)value;
					break;
				case ExportOptionsWrapperKeys.ExcelFormat:
					opts.FormatOptions.ExcelOptions.Format = (ExcelFormat)value;
					break;
				case ExportOptionsWrapperKeys.CsvValueSeparator:
					opts.FormatOptions.ExcelOptions.CsvValueSeparator = (string)value;
					break;
				case ExportOptionsWrapperKeys.ImageFormat:
					opts.FormatOptions.ImageOptions.Format = (ImageFormat)value;
					break;
				case ExportOptionsWrapperKeys.ImageResolution:
					opts.FormatOptions.ImageOptions.Resolution = (int)value;
					break;
				case ExportOptionsWrapperKeys.PrintHeadersOnEveryPage:
					opts.ItemContentOptions.HeadersOptions.PrintHeadersOnEveryPage = (bool)value;
					break;
				case ExportOptionsWrapperKeys.ItemSizeMode:
					opts.ItemContentOptions.SizeMode = (ItemSizeMode)value;
					break;
				case ExportOptionsWrapperKeys.AutoArrangeContent:
					opts.ItemContentOptions.ArrangerOptions.AutoArrangeContent = (bool)value;
					break;
				default:
					throw new Exception("Incorrect key");
			}
		}
	}
}
