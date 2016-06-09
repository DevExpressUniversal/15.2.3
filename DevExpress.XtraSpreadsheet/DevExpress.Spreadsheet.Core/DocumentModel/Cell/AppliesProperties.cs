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

using DevExpress.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	public partial class Cell {
		#region Applies.ApplyNumberFormat
		public bool ApplyNumberFormat {
			get { return FormatInfo.ApplyNumberFormat; }
			set {
				if (FormatInfo.ApplyNumberFormat == value)
					return;
				SetPropertyValue(SetApplyNumberFormatCore, value);
			}
		}
		DocumentModelChangeActions SetApplyNumberFormatCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyNumberFormat = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyFont
		public bool ApplyFont {
			get { return FormatInfo.ApplyFont; }
			set {
				if (FormatInfo.ApplyFont == value)
					return;
				SetPropertyValue(SetApplyFontCore, value);
			}
		}
		DocumentModelChangeActions SetApplyFontCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyFont = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyFill
		public bool ApplyFill {
			get { return FormatInfo.ApplyFill; }
			set {
				if (FormatInfo.ApplyFill == value)
					return;
				SetPropertyValue(SetApplyFillCore, value);
			}
		}
		DocumentModelChangeActions SetApplyFillCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyFill = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyBorder
		public bool ApplyBorder {
			get { return FormatInfo.ApplyBorder; }
			set {
				if (FormatInfo.ApplyBorder == value)
					return;
				SetPropertyValue(SetApplyBorderCore, value);
			}
		}
		DocumentModelChangeActions SetApplyBorderCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyBorder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyAlignment
		public bool ApplyAlignment {
			get { return FormatInfo.ApplyAlignment; }
			set {
				if (FormatInfo.ApplyAlignment == value)
					return;
				SetPropertyValue(SetApplyAlignmentCore, value);
			}
		}
		DocumentModelChangeActions SetApplyAlignmentCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyAlignment = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyProtection
		public bool ApplyProtection {
			get { return FormatInfo.ApplyProtection; }
			set {
				if (FormatInfo.ApplyProtection == value)
					return;
				SetPropertyValue(SetApplyProtectionCore, value);
			}
		}
		DocumentModelChangeActions SetApplyProtectionCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyProtection = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IActualApplyInfo Members
		public IActualApplyInfo ActualApplyInfo { get { return FormatInfo.ActualApplyInfo; } }  
		#endregion
	}
}
