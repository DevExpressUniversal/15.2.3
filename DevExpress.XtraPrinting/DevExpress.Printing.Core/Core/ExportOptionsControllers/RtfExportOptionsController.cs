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
using System;
using DevExpress.XtraPrinting.Native.Lines;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native.ExportOtions;
using System.Collections.Generic;
#if SILVERLIGHT
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.XtraPrinting.Native.ExportOptionsControllers {
	public class RtfExportOptionsController : ExportOptionsControllerBase {
		protected override Type ExportOptionsType { get { return typeof(RtfExportOptions); } }
		public override PreviewStringId CaptionStringId { get { return PreviewStringId.ExportOptionsForm_CaptionRtf; } }
		protected override string[] LocalizerStrings { get { return new string[] { PreviewLocalizer.GetString(PreviewStringId.SaveDlg_FilterRtf) }; } }
		public override string[] FileExtensions { get { return new string[] { ".rtf" }; } }
		public RtfExportOptionsController() {
		}
		public override string[] GetExportedFileNames(PrintingSystemBase ps, ExportOptionsBase options, string fileName) {
			ps.ExportToRtf(fileName, (RtfExportOptions)options);
			return new string[] { fileName };
		}
		protected override void CollectLineControllers(ExportOptionsBase options, List<ExportOptionKind> hiddenOptions, System.Collections.Generic.List<DevExpress.XtraPrinting.Native.Lines.BaseLineController> list) {
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Rtf.ExportMode, typeof(PSDropDownLineController), ExportOptionKind.RtfExportMode);
			AddPageRangeLineControllerToList(hiddenOptions, options, list, ExportOptionKind.RtfPageRange);
			AddEmptySpaceToList(list);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Rtf.ExportWatermarks, typeof(PSBooleanLineController), ExportOptionKind.RtfExportWatermarks);			
		}
		protected override Type GetExportModeType() {
			return typeof(RtfExportMode);
		}
		protected override string ExportModePropertyName {
			get {
				return ExportOptionsPropertiesNames.Rtf.ExportMode;
			}
		}
	}
}
