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
using DevExpress.Office.Utils;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandPivotViewExtTag  -- QsiSXTag --
	public class XlsCommandPivotViewExtTag : XlsCommandBase {
		#region Fields
		private enum FirstExtTag {
			PivotView = 0,		  
			EnableRefresh = 2,	  
			Invalid = 3,			
			OlapPivotView = 4,	  
		}
		private enum SecondExtTag {
			VerSxLastUpdated = 0,
			VerSxUpdatableMin = 1
		}
		private enum View9Save {
			DisableNoData = 0,	  
			HideTotAnnotation = 1,  
			IncludeEmptyRow = 3,	
			IncludeEmptyColumn = 4, 
		}
		private enum QsiFuture {
			PreserveFormat = 0,	 
			AutoFit = 1,			
			ExternalDataList = 4,   
			QueryTableList = 6,	 
			DummyList = 7,		  
		}
		BitwiseContainer first = new BitwiseContainer(32, new int[] { 1, 15 });
		BitwiseContainer second = new BitwiseContainer(16, new int[] { 8, 8 });
		BitwiseContainer viewNineSave = new BitwiseContainer(32);
		BitwiseContainer queryTableFuture = new BitwiseContainer(32);
		XLUnicodeString name = new XLUnicodeString();
		#endregion
		#region Properties
		public int OldHeader {
			get { return 0x0802; }
			set {
				if (value != 0x0802)
					throw new ArgumentException("The frtHeaderOld.rt field MUST be 0x0802");
			}
		}
		public bool IsPivotTable {
			get { return first.GetBoolValue((int)FirstExtTag.PivotView); }
			set { first.SetBoolValue((int)FirstExtTag.PivotView, value); }
		}
		public bool IsEnableRefresh {
			get { return first.GetBoolValue((int)FirstExtTag.EnableRefresh); }
			set { first.SetBoolValue((int)FirstExtTag.EnableRefresh, value); }
		}
		public bool IsInvalid {
			get { return first.GetBoolValue((int)FirstExtTag.Invalid); }
			set { first.SetBoolValue((int)FirstExtTag.Invalid, value); }
		}
		public bool IsOlapPivotTable {
			get { return first.GetBoolValue((int)FirstExtTag.OlapPivotView); }
			set {
				if (!IsPivotTable && value)
					throw new ArgumentException("MUST be equal to 0 because fSx is 0");
				first.SetBoolValue((int)FirstExtTag.OlapPivotView, value);
			}
		}
		public bool IsDisableNoData {
			get { return viewNineSave.GetBoolValue((int)View9Save.DisableNoData); }
			set { viewNineSave.SetBoolValue((int)View9Save.DisableNoData, value); }
		}
		public bool IsHideTotAnnotation {
			get { return viewNineSave.GetBoolValue((int)View9Save.HideTotAnnotation); }
			set { viewNineSave.SetBoolValue((int)View9Save.HideTotAnnotation, value); }
		}
		public bool IsIncludeEmptyRow {
			get { return viewNineSave.GetBoolValue((int)View9Save.IncludeEmptyRow); }
			set { viewNineSave.SetBoolValue((int)View9Save.IncludeEmptyRow, value); }
		}
		public bool IsIncludeEmptyColumn {
			get { return viewNineSave.GetBoolValue((int)View9Save.IncludeEmptyColumn); }
			set { viewNineSave.SetBoolValue((int)View9Save.IncludeEmptyColumn, value); }
		}
		public bool IsPreserveFormat {
			get { return queryTableFuture.GetBoolValue((int)QsiFuture.PreserveFormat); }
			set { queryTableFuture.SetBoolValue((int)QsiFuture.PreserveFormat, value); }
		}
		public bool IsAutoFit {
			get { return queryTableFuture.GetBoolValue((int)QsiFuture.AutoFit); }
			set { queryTableFuture.SetBoolValue((int)QsiFuture.AutoFit, value); }
		}
		public bool IsExternalDataList {
			get { return queryTableFuture.GetBoolValue((int)QsiFuture.ExternalDataList); }
			set { queryTableFuture.SetBoolValue((int)QsiFuture.ExternalDataList, value); }
		}
		public bool IsQueryTableList {
			get { return queryTableFuture.GetBoolValue((int)QsiFuture.QueryTableList); }
			set { queryTableFuture.SetBoolValue((int)QsiFuture.QueryTableList, value); }
		}
		public bool IsDummyList {
			get { return queryTableFuture.GetBoolValue((int)QsiFuture.DummyList); }
			set { queryTableFuture.SetBoolValue((int)QsiFuture.DummyList, value); }
		}
		public int VersionLastUpdated {
			get { return second.GetIntValue((int)SecondExtTag.VerSxLastUpdated); }
			set { second.SetIntValue((int)SecondExtTag.VerSxLastUpdated, value); }
		}
		public int LowRecalculateVersion {
			get { return second.GetIntValue((int)SecondExtTag.VerSxUpdatableMin); }
			set { second.SetIntValue((int)SecondExtTag.VerSxUpdatableMin, value); }
		}
		public string Name {
			get { return name.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "Name");
				name.Value = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			OldHeader = reader.ReadInt32();
			first.IntContainer = reader.ReadInt32();
			if (IsPivotTable)
				viewNineSave.IntContainer = reader.ReadInt32();
			else
				queryTableFuture.IntContainer = reader.ReadInt32();
			second.IntContainer = reader.ReadUInt16();
			reader.ReadInt16();	 
			name = XLUnicodeString.FromStream(reader);
			reader.ReadInt16();	 
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null && IsPivotTable) {
				builder.PivotTable.SetAsteriskTotals(IsHideTotAnnotation);
				builder.PivotTable.ShowDropZones = !IsDisableNoData;
				builder.PivotTable.MinRefreshableVersion = (byte)LowRecalculateVersion;
				builder.PivotTable.UpdatedVersion = (byte)VersionLastUpdated;
				builder.PivotTable.SetShowEmptyRow(IsIncludeEmptyRow);
				builder.PivotTable.SetShowEmptyColumn(IsIncludeEmptyColumn);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(OldHeader);
			writer.Write(first.IntContainer);
			if (IsPivotTable)
				writer.Write(viewNineSave.IntContainer);
			else
				writer.Write(queryTableFuture.IntContainer);
			writer.Write((ushort)second.IntContainer);
			writer.Write((ushort)0x10);
			name.Write(writer);
			writer.Write((ushort)0);
		}
		protected override short GetSize() {
			int result = 18;
			result += name.Length;
			return (short)result;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
