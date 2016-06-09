﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region FilterNumberCommandGroup
	public class FilterNumberCommandGroup : DataSpecificFilterCommandGroupBase {
		public FilterNumberCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFilterNumberFiltersCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterNumberFilters; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterNumberFiltersDescription; } }
		#endregion
		protected internal override bool IsValidDataForFilter(SuggestedFilterTypeInfo filterTypeInfo) {
			return filterTypeInfo.IsNumberFilter;
		}
	}
	#endregion
}
