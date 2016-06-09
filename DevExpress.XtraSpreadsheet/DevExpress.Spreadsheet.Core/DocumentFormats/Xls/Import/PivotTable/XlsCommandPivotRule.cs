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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandPivotRule  -- SxRule --
	public class XlsCommandPivotRule : XlsCommandBase {
		#region Fields
		private enum RuleFirst {
			FieldPosition = 0, 
			FieldIndex = 1 
		}
		private enum RuleSecond {
			AxisType = 0,   
			AreaRuleRefers = 1, 
			AreaIncludedRule = 2, 
			DataOnly = 3, 
			LabelOnly = 4, 
			GrandRow = 5, 
			GrandColumn = 6, 
			GrandRowCreated = 7, 
			CacheBased = 8, 
			GrandColumnCreated = 9, 
		}
		BitwiseContainer first = new BitwiseContainer(16, new int[] { 8, 8 });
		BitwiseContainer second = new BitwiseContainer(16, new int[] { 4, 4 });
		int countFilter;
		ushort row;
		ushort column;
		CellRange range;
		#endregion
		#region Properties
		public int FieldPosition {
			get { return first.GetIntValue((int)RuleFirst.FieldPosition); }
			set {
				ValueChecker.CheckValue(value, 0, byte.MaxValue, "BasedPositionAxis");
				first.SetIntValue((int)RuleFirst.FieldPosition, value);
			}
		}
		public int FieldIndex {
			get { return first.GetIntValue((int)RuleFirst.FieldIndex); }
			set {
				ValueChecker.CheckValue(value, 0, byte.MaxValue, "RuleRefers");
				first.SetIntValue((int)RuleFirst.FieldIndex, value);
			}
		}
		public PivotTableAxis Axis {
			get { return (PivotTableAxis)second.GetIntValue((int)RuleSecond.AxisType); }
			set {
				second.SetIntValue((int)RuleSecond.AxisType, (int)value);
			}
		}
		public PivotAreaType TypeRule{
			get { return (PivotAreaType)second.GetIntValue((int)RuleSecond.AreaRuleRefers); }
			set {
				ValueChecker.CheckValue((int)value, 0, 6, "RuleRefers");
				second.SetIntValue((int)RuleSecond.AreaRuleRefers, (int)value);
				if ((int)value > 4) {
					IsDataOnly = false;
					IsLabelOnly = true;
				}
			}
		}
		public bool IsAreaIncludedRule {
			get { return second.GetBoolValue((int)RuleSecond.AreaIncludedRule); }
			set { second.SetBoolValue((int)RuleSecond.AreaIncludedRule, value); }
		}
		public bool IsDataOnly {
			get { return second.GetBoolValue((int)RuleSecond.DataOnly); }
			set {
				if (!IsLabelOnly || !value)
					second.SetBoolValue((int)RuleSecond.DataOnly, value);
				else
					throw new ArgumentException("MUST be false because fLabelOnly is true");
			}
		}
		public bool IsLabelOnly {
			get { return second.GetBoolValue((int)RuleSecond.LabelOnly); }
			set {
				if (!IsDataOnly || !value) {
					if (TypeRule < PivotAreaType.Button || value)
						second.SetBoolValue((int)RuleSecond.LabelOnly, value);
					else
						throw new ArgumentException("MUST be true because sxrType is equal to 0x5 or 0x6");
				}
				else
					throw new ArgumentException("MUST be false because fDataOnly is true");
			}
		}
		public bool IsGrandRow {
			get { return second.GetBoolValue((int)RuleSecond.GrandRow); }
			set { second.SetBoolValue((int)RuleSecond.GrandRow, value); }
		}
		public bool IsGrandColumn {
			get { return second.GetBoolValue((int)RuleSecond.GrandColumn); }
			set { second.SetBoolValue((int)RuleSecond.GrandColumn, value); }
		}
		public bool IsGrandRowCreated {
			get { return second.GetBoolValue((int)RuleSecond.GrandRowCreated); }
			set { second.SetBoolValue((int)RuleSecond.GrandRowCreated, value); }
		}
		public bool IsCacheBased {
			get { return second.GetBoolValue((int)RuleSecond.CacheBased); }
			set { second.SetBoolValue((int)RuleSecond.CacheBased, value); }
		}
		public bool IsGrandColumnCreated {
			get { return second.GetBoolValue((int)RuleSecond.GrandColumnCreated); }
			set { second.SetBoolValue((int)RuleSecond.GrandColumnCreated, value); }
		}
		public int CountFilter {
			get { return countFilter; }
			set {
				if (!(TypeRule == PivotAreaType.Normal || TypeRule == PivotAreaType.Data) && value != 0)
					throw new ArgumentException("MUST be zero if sxrType is neither 0x1 nor 0x2.");
				else
					countFilter = value;
			}
		}
		public CellRange Range { get { return range; } set { range = value;} }
		int FirstRow { get { return (row & 0xFF);}}
		int LastRow { get { return ((row >> 8) & 0xFF); } }
		int FirstColumn { get { return (column & 0xFF); } }
		int LastColumn { get { return ((column >> 8) & 0xFF); } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			first.IntContainer = (int)reader.ReadUInt16();
			second.IntContainer = (int)reader.ReadUInt16();
			reader.ReadUInt16(); 
			CountFilter = reader.ReadUInt16();
			if (IsAreaIncludedRule) {
				row = reader.ReadUInt16();
				column = reader.ReadUInt16();
				range = (CellRange)XlsCellRangeFactory.CreateCellRange(
							contentBuilder.CurrentSheet,
							new CellPosition(FirstColumn, FirstRow),
							new CellPosition(LastColumn, LastRow));
			}
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView build = contentBuilder.CurrentBuilderPivotView;
			if (build != null) { 
				PivotArea pivotArea = null;
				if (build.IsPivotSelection) 
					pivotArea = contentBuilder.CurrentSheet.PivotSelection.PivotArea;
				if (build.IsPivotFormat)
					pivotArea = build.Format.PivotArea;
				if (pivotArea != null) {
					SetupPivotArea(pivotArea);
					if (FieldIndex != 0xFF &&  FieldIndex != 0xFE)
						pivotArea.Field = FieldIndex;
				}
			}
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			DocumentModel documentModel = contentBuilder.DocumentModel;
			PivotCache pivotCache = documentModel.PivotCaches.Last;
			if (pivotCache.CalculatedItems == null)
				pivotCache.CalculatedItems = new PivotCacheCalculatedItemCollection();
			PivotCacheCalculatedItem calculatedItem = new PivotCacheCalculatedItem();
			PivotArea pivotArea = new PivotArea(documentModel);
			SetupPivotArea(pivotArea);
			pivotArea.IsOutline = false;
			if (FieldIndex < 0xFD)
				pivotArea.Field = FieldIndex;
			calculatedItem.PivotArea = pivotArea;
			pivotCache.CalculatedItems.Add(calculatedItem);
		}
		void SetupPivotArea(PivotArea pivotArea) {
			pivotArea.IsCacheIndex = IsCacheBased;
			pivotArea.IsGrandRow = IsGrandRow;
			pivotArea.IsGrandColumn = IsGrandColumn;
			pivotArea.IsLabelOnly = IsLabelOnly;
			pivotArea.IsDataOnly = IsDataOnly;
			pivotArea.FieldPosition = FieldPosition;
			pivotArea.Axis = Axis;
			pivotArea.Type = TypeRule;
			pivotArea.Range = Range;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)first.IntContainer);
			writer.Write((ushort)second.IntContainer);
			writer.Write((short)0);
			writer.Write((ushort)CountFilter);
			if (IsAreaIncludedRule) {
				writer.Write((ushort)(range.BottomRight.Row << 8 | range.TopLeft.Row));
				writer.Write((ushort)(range.BottomRight.Column << 8 | range.TopLeft.Column));
			}
		}
		protected override short GetSize() {
			int result = 8;
			if (IsAreaIncludedRule)
				result += 4;
			return (short)result;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
