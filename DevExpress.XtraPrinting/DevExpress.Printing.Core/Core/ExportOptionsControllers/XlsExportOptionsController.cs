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
using DevExpress.XtraPrinting.Native.ExportOtions;
using System.ComponentModel;
#if SILVERLIGHT
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.XtraPrinting.Native.ExportOptionsControllers {
	public abstract class XlsExportOptionsControllerBase : ExportOptionsControllerBase {
		protected override void CollectLineControllers(ExportOptionsBase options, List<ExportOptionKind> hiddenOptions, List<BaseLineController> list) {
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Xls.SheetName, typeof(PSTextLineController), ExportOptionKind.XlsSheetName);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Text.TextExportMode, typeof(PSDropDownLineController), ExportOptionKind.TextExportMode);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Xls.ShowGridLines, typeof(PSBooleanLineController), ExportOptionKind.XlsShowGridLines);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Xls.ExportHyperlinks, typeof(PSBooleanLineController), ExportOptionKind.XlsExportHyperlinks);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Xls.RawDataMode, typeof(PSBooleanLineController), ExportOptionKind.XlsRawDataMode);
		}
	}
	public class XlsExportOptionsController : XlsExportOptionsControllerBase {
		public XlsExportOptionsController() {
		}
		protected override Type ExportOptionsType { get { return typeof(XlsExportOptions); } }
		public override PreviewStringId CaptionStringId { get { return PreviewStringId.ExportOptionsForm_CaptionXls; } }
		protected override string[] LocalizerStrings { get { return new string[] { PreviewLocalizer.GetString(PreviewStringId.SaveDlg_FilterXls) }; } }
		public override string[] FileExtensions { get { return new string[] { ".xls" }; } }
		public override string[] GetExportedFileNames(PrintingSystemBase ps, ExportOptionsBase options, string fileName) {
			return ps.ExportToXlsInternal(fileName, (XlsExportOptions)options);
		}
		protected override void CollectLineControllers(ExportOptionsBase options, List<ExportOptionKind> hiddenOptions, List<BaseLineController> list) {
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Xls.ExportMode, typeof(PSDropDownLineController), ExportOptionKind.XlsExportMode);
			AddPageRangeLineControllerToList(hiddenOptions, options, list, ExportOptionKind.XlsPageRange);
			AddSeparatorToList(list);
			base.CollectLineControllers(options, hiddenOptions, list);
		}
		protected override Type GetExportModeType() {
			return typeof(XlsExportMode);
		}
		protected override string ExportModePropertyName {
			get {
				return ExportOptionsPropertiesNames.Xls.ExportMode;
			}
		}
	}
	public class XlsxExportOptionsController : XlsExportOptionsControllerBase {
		public XlsxExportOptionsController() {
		}
		protected override Type ExportOptionsType { get { return typeof(XlsxExportOptions); } }
		public override PreviewStringId CaptionStringId { get { return PreviewStringId.ExportOptionsForm_CaptionXlsx; } }
		protected override string[] LocalizerStrings { get { return new string[] { PreviewLocalizer.GetString(PreviewStringId.SaveDlg_FilterXlsx) }; } }
		public override string[] FileExtensions { get { return new string[] { ".xlsx" }; } }
		public override string[] GetExportedFileNames(PrintingSystemBase ps, ExportOptionsBase options, string fileName) {
			return ps.ExportToXlsxInternal(fileName, (XlsxExportOptions)options);
		}
		protected override void CollectLineControllers(ExportOptionsBase options, List<ExportOptionKind> hiddenOptions, List<BaseLineController> list) {
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Xls.ExportMode, typeof(PSDropDownLineController), ExportOptionKind.XlsxExportMode);
			AddPageRangeLineControllerToList(hiddenOptions, options, list, ExportOptionKind.XlsxPageRange);
			AddSeparatorToList(list);
			base.CollectLineControllers(options, hiddenOptions, list);
		}
		protected override Type GetExportModeType() {
			return typeof(XlsxExportMode);
		}
		protected override string ExportModePropertyName {
			get {
				return ExportOptionsPropertiesNames.Xls.ExportMode;
			}
		}
	}
}
