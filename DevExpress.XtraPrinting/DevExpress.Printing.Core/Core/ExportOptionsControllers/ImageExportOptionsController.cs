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

using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Lines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native.ExportOtions;
#if SILVERLIGHT
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.XtraPrinting.Native.ExportOptionsControllers {
	public class ImageExportOptionsController : ExportOptionsControllerBase {
		protected override string[] LocalizerStrings {
			get {
				return new string[] {
							PreviewLocalizer.GetString(PreviewStringId.SaveDlg_FilterBmp),
							PreviewLocalizer.GetString(PreviewStringId.SaveDlg_FilterGif),
							PreviewLocalizer.GetString(PreviewStringId.SaveDlg_FilterJpeg),
							PreviewLocalizer.GetString(PreviewStringId.SaveDlg_FilterPng),
							PreviewLocalizer.GetString(PreviewStringId.SaveDlg_FilterTiff),
							PreviewLocalizer.GetString(PreviewStringId.SaveDlg_FilterEmf),
							PreviewLocalizer.GetString(PreviewStringId.SaveDlg_FilterWmf),
			};
			}
		}
		public override string[] FileExtensions {
			get {
				return new string[] { ".bmp", ".gif", ".jpg", ".png", ".tiff", ".emf", ".wmf" };
			}
		}
		protected override Type ExportOptionsType { get { return typeof(ImageExportOptions); } }
		public override PreviewStringId CaptionStringId { get { return PreviewStringId.ExportOptionsForm_CaptionImage; } }
		public ImageExportOptionsController()
			: base() {
		}
		protected override Type GetExportModeType() {
			return typeof(ImageExportMode);
		}
		protected override string ExportModePropertyName {
			get {
				return ExportOptionsPropertiesNames.Image.ExportMode;
			}
		}
		public override string[] GetExportedFileNames(PrintingSystemBase ps, ExportOptionsBase options, string fileName) {
			string extension = Path.GetExtension(fileName).ToLower();
			ImageExportOptions newOptions = (ImageExportOptions)options;
			if(ExportOptionsHelper.ImageFormats.ContainsKey(extension))
				newOptions = ExportOptionsHelper.ChangeOldImageProperties(newOptions, ExportOptionsHelper.ImageFormats[extension]);
			return ps.ExportToImageInternal(fileName, newOptions);
		}
		protected override void CollectLineControllers(ExportOptionsBase options, List<ExportOptionKind> hiddenOptions, List<BaseLineController> list) {
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Image.Format, typeof(PSDropDownLineController), ExportOptionKind.ImageFormat);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Image.Resolution, typeof(PSNumericLineController), ExportOptionKind.ImageResolution);
			AddSeparatorToList(list);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Image.ExportMode, typeof(PSDropDownLineController), ExportOptionKind.ImageExportMode);
			AddPageRangeLineControllerToList(hiddenOptions, options, list, ExportOptionKind.ImagePageRange);
			AddEmptySpaceToList(list);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Image.PageBorderColor, typeof(PSColorLineController), ExportOptionKind.ImagePageBorderColor);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Image.PageBorderWidth, typeof(PSNumericLineController), ExportOptionKind.ImagePageBorderWidth);			
		}
		public override string GetFileExtension(ExportOptionsBase options) {
			foreach(string extension in ExportOptionsHelper.ImageFormats.Keys) {
				if(ExportOptionsHelper.ImageFormats[extension] == ((ImageExportOptions)options).Format)
					return extension;
			}
			return base.GetFileExtension(options);
		}
		public override int GetFilterIndex(ExportOptionsBase options) {
			string extension = GetFileExtension(options);
			for(int i = 0; i < FileExtensions.Length; i++) {
				if(string.Compare(FileExtensions[i], extension, StringComparison.InvariantCultureIgnoreCase) == 0)
					return i + 1;
			}
			return 1;
		}
		public override bool ValidateInputFileName(ExportOptionsBase options) {
			return ((ImageExportOptions)options).ExportMode != ImageExportMode.DifferentFiles;
		}
	}
}
