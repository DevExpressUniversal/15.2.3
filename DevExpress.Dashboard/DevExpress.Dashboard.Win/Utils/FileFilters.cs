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
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public static class FileFilters {
		const string BMPExtensions = "*.bmp;*.dib;*.rle";
		const string JPEGExtensions = "*.jpg;*.jpeg;*.jpe;*.jfif";
		const string PNGExtensions = "*.png";
		const string GIFExtensions = "*.gif";
		const string EMFExtensions = "*.emf";
		const string WMFExtensions = "*.wmf";
		const string TIFFExtensions = "*.tif;*.tiff";
		const string ICOExtensions = "*.ico";
		const string PDFExtensions = "*.pdf";
		const string SHPExtensions = "*.shp";
		const string CSVExtensions = "*.csv";
		const string XLSExtensions = "*.xls";
		const string XLSXExtensions = "*.xlsx";
		public const string Separator = "|";
		public static readonly string AllFiles = DashboardWinLocalizer.GetString(DashboardWinStringId.FileFilterAll) + " (*.*)|*.*";
		public static readonly string AllImageFiles = String.Format("{0} ({1};{2};{3};{4};{5};{6};{7};{8})|{1};{2};{3};{4};{5};{6};{7};{8}",
			DashboardWinLocalizer.GetString(DashboardWinStringId.FileFilterAllImages), 
			BMPExtensions, JPEGExtensions, PNGExtensions, GIFExtensions, EMFExtensions, WMFExtensions, TIFFExtensions, ICOExtensions);
		public static readonly string BMP = String.Format("BMP ({0})|{0}", BMPExtensions);
		public static readonly string JPEG = String.Format("JPEG ({0})|{0}", JPEGExtensions);
		public static readonly string PNG = String.Format("PNG ({0})|{0}", PNGExtensions);
		public static readonly string GIF = String.Format("GIF ({0})|{0}", GIFExtensions);
		public static readonly string EMF = String.Format("EMF ({0})|{0}", EMFExtensions);
		public static readonly string WMF = String.Format("WMF ({0})|{0}", WMFExtensions);
		public static readonly string TIFF = String.Format("TIFF ({0})|{0}", TIFFExtensions);
		public static readonly string ICO = String.Format("ICO ({0})|{0}", ICOExtensions);
		public static readonly string PDF = String.Format("PDF ({0})|{0}", PDFExtensions);
		public static readonly string SHP = String.Format("SHP ({0})|{0}", SHPExtensions);
		public static readonly string CSV = String.Format("CSV ({0})|{0}", CSVExtensions);
		public static readonly string XLS = String.Format("XLS ({0})|{0}", XLSExtensions);
		public static readonly string XLSX = String.Format("XLSX ({0})|{0}", XLSXExtensions);
		public static readonly string DashboardFiles = DashboardWinLocalizer.GetString(DashboardWinStringId.FileFilterDashboards) + " (*.xml)|*.xml";
	}
}
