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
namespace DevExpress.XtraPrinting.Native.ExportOptionsControllers {
	public abstract class TextExportOptionsControllerBase : ExportOptionsControllerBase {
		protected override void CollectLineControllers(ExportOptionsBase options, List<ExportOptionKind> hiddenOptions, List<BaseLineController> list) {
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Text.Encoding, typeof(PSDropDownLineController), ExportOptionKind.TextEncoding);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Text.TextExportMode, typeof(PSDropDownLineController), ExportOptionKind.TextExportMode);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Text.Separator, typeof(PSTextLineController), ExportOptionKind.TextSeparator);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Text.QuoteStringsWithSeparators, typeof(PSBooleanLineController), ExportOptionKind.TextQuoteStringsWithSeparators);
		}
	}
}
