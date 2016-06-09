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

using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Export.Xl;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ICellAlignmentInfo
	public interface ICellAlignmentInfo {
		bool WrapText { get; set; }
		bool JustifyLastLine { get; set; }
		bool ShrinkToFit { get; set; }
		int TextRotation { get; set; }
		byte Indent { get; set; } 
		int RelativeIndent { get; set; }
		XlHorizontalAlignment Horizontal { get ; set ; }
		XlVerticalAlignment Vertical { get; set; }
		XlReadingOrder ReadingOrder { get; set; }
	}
	#endregion
	#region IActualCellAlignmentInfo
	public interface IActualCellAlignmentInfo {
		bool WrapText { get; }
		bool JustifyLastLine { get; }
		bool ShrinkToFit { get; }
		int TextRotation { get; }
		byte Indent { get; }
		int RelativeIndent { get; }
		XlHorizontalAlignment Horizontal { get; }
		XlVerticalAlignment Vertical { get; }
		XlReadingOrder ReadingOrder { get; }
	}
	#endregion
	#region CellAlignmentInfo
	public class CellAlignmentInfo : XlCellAlignment, ICloneable<CellAlignmentInfo>, ISupportsCopyFrom<CellAlignmentInfo>, ISupportsSizeOf {
		public new CellAlignmentInfo Clone() {
			CellAlignmentInfo result = new CellAlignmentInfo();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(CellAlignmentInfo value) {
			base.CopyFrom(value);
		}
		public void CopyFrom(IActualCellAlignmentInfo value) {
			this.HorizontalAlignment = value.Horizontal;
			this.Indent = value.Indent;
			this.JustifyLastLine = value.JustifyLastLine;
			this.ReadingOrder = value.ReadingOrder;
			this.RelativeIndent = value.RelativeIndent;
			this.ShrinkToFit = value.ShrinkToFit;
			this.TextRotation = value.TextRotation;
			this.VerticalAlignment = value.Vertical;
			this.WrapText = value.WrapText;
		}
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		protected override void CheckIndent(byte value) {
		}
		protected override void CheckRelativeIndent(int value) {
		}
		protected override void CheckTextRotation(int value) {
		}
#if DEBUGTEST
		public static bool CheckDefaults2(CellAlignmentInfo info) {
			bool result = true;
			result &= false == info.JustifyLastLine;
			result &= false == info.ShrinkToFit;
			result &= false == info.WrapText;
			result &= 0 == info.Indent;
			result &= 0 == info.RelativeIndent;
			result &= 0 == info.TextRotation;
			result &= XlHorizontalAlignment.General == info.HorizontalAlignment;
			result &= XlVerticalAlignment.Bottom == info.VerticalAlignment;
			result &= XlReadingOrder.Context == info.ReadingOrder;
			return result;
		}
#endif
	}
	#endregion
	#region CellAlignmentInfoCache
	public class CellAlignmentInfoCache : UniqueItemsCache<CellAlignmentInfo> {
		internal const int DefaultItemIndex = 0;
		public CellAlignmentInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override CellAlignmentInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new CellAlignmentInfo();
		}
#if DEBUGTEST
		public static bool CheckDefaults2(CellAlignmentInfoCache collection) {
			bool result = true;
			result &= collection != null;
			result &= collection.Count > 0;
			CellAlignmentInfo info = (CellAlignmentInfo)collection.DefaultItem;
			result &= ! info.JustifyLastLine;
			result &= ! info.ShrinkToFit;
			result &= ! info.WrapText;
			result &= 0 == info.Indent;
			result &= 0 == info.RelativeIndent;
			result &= 0 == info.TextRotation;
			result &= XlHorizontalAlignment.General == info.HorizontalAlignment;
			result &= XlVerticalAlignment.Bottom == info.VerticalAlignment;
			result &= XlReadingOrder.Context == info.ReadingOrder;
			return result;
		}
#endif
	}
	#endregion
}
