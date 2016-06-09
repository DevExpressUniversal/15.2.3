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
	public abstract class HtmlExportOptionsControllerBase : ExportOptionsControllerBase {
		protected override void CollectLineControllers(ExportOptionsBase options, List<ExportOptionKind> hiddenOptions, List<BaseLineController> list) {
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Html.ExportMode, typeof(PSDropDownLineController), ExportOptionKind.HtmlExportMode);
			AddPageRangeLineControllerToList(hiddenOptions, options, list, ExportOptionKind.HtmlPageRange);
			AddEmptySpaceToList(list);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Html.PageBorderColor, typeof(PSColorLineController), ExportOptionKind.HtmlPageBorderColor);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Html.PageBorderWidth, typeof(PSNumericLineController), ExportOptionKind.HtmlPageBorderWidth);
			AddEmptySpaceToList(list);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Html.Title, typeof(PSTextLineController), ExportOptionKind.HtmlTitle);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Html.CharacterSet, typeof(PSDropDownLineController), ExportOptionKind.HtmlCharacterSet);
			AddEmptySpaceToList(list);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Html.RemoveSecondarySymbols, typeof(PSBooleanLineController), ExportOptionKind.HtmlRemoveSecondarySymbols);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Html.TableLayout, typeof(PSBooleanLineController), ExportOptionKind.HtmlTableLayout);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Html.ExportWatermarks, typeof(PSBooleanLineController), ExportOptionKind.HtmlExportWatermarks); 
		}
		public override bool ValidateInputFileName(ExportOptionsBase options) {
			return ((HtmlExportOptionsBase)options).ExportMode != HtmlExportMode.DifferentFiles;
		}
		protected override Type GetExportModeType() {
			return typeof(HtmlExportMode);
		}
		protected override string ExportModePropertyName {
			get {
				return ExportOptionsPropertiesNames.Html.ExportMode;
			}
		}
	}
}
