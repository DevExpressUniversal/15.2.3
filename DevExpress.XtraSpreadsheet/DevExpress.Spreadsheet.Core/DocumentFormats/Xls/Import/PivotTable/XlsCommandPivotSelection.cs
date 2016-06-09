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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandPivotSelection  -- SxSelect --
	public class XlsCommandPivotSelection : XlsCommandBase {
		#region Fields
		private enum SelectionFlags {
			CountClick = 0, 
			LabelOnly = 1, 
			DataOnly = 2, 
			ToggleDataHeader = 3, 
			SelectionClick = 4, 
			Extendable = 5, 
		}
		BitwiseContainer flag = new BitwiseContainer(16, new int[] { 5 });
		int startLines;
		int minIndex = 0;
		int maxIndex = 0xFFFF;
		#endregion
		#region Properties
		public ViewPaneType ActivePane { get; set; }
		public PivotTableAxis SelectionAxis { get; set; }
		public int Dimension { get; set; }
		public int StartLines {
			get { return startLines; }
			set {
				if (value >= Minimum && value <= Maximum)
					startLines = value;
				else
					throw new ArgumentException("MUST be greater than or equal to the iLiMin field and less than or equal to the iLiMax field");
			} 
		}
		public int CountSelection { get; set; }
		public int Minimum {
			get { return minIndex; } 
			set {
				if (value <= maxIndex)
					minIndex = value;
				else
					throw new ArgumentException("MUST be greater than or equal to zero and less than or equal to the iLiMax field.");
			} 
		}
		public int Maximum {
			get { return maxIndex; }
			set { 
				if(value >= minIndex)
					maxIndex = value;
				else
					throw new ArgumentException("MUST be greater than or equal to zero. MUST be greater than or equal to the iLiMin field");
			}
		}
		public int ActiveRow { get; set; }
		public int ActiveColumn { get; set; }
		public int PreviousRow { get; set; }
		public int PreviousColumn { get; set; }
		public int CountClick {
			get { return flag.GetIntValue((int)SelectionFlags.CountClick); }
			set { flag.SetIntValue((int)SelectionFlags.CountClick, value); }
		}
		public bool IsLabelOnly {
			get { return flag.GetBoolValue((int)SelectionFlags.LabelOnly); }
			set { flag.SetBoolValue((int)SelectionFlags.LabelOnly, value); }
		}
		public bool IsDataOnly {
			get { return flag.GetBoolValue((int)SelectionFlags.DataOnly); }
			set { flag.SetBoolValue((int)SelectionFlags.DataOnly, value); }
		}
		public bool IsToggleDataHeader {
			get { return flag.GetBoolValue((int)SelectionFlags.ToggleDataHeader); }
			set { flag.SetBoolValue((int)SelectionFlags.ToggleDataHeader, value); }
		}
		public bool IsSelectionClick {
			get { return flag.GetBoolValue((int)SelectionFlags.SelectionClick); }
			set { flag.SetBoolValue((int)SelectionFlags.SelectionClick, value); }
		}
		public bool IsExtendable {
			get { return flag.GetBoolValue((int)SelectionFlags.Extendable); }
			set { flag.SetBoolValue((int)SelectionFlags.Extendable, value); }
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			reader.ReadInt16(); 
			switch ((reader.ReadInt16() & 0xFF)) { 
				case 3: ActivePane = ViewPaneType.BottomRight;
					break;
				case 2: ActivePane = ViewPaneType.TopRight;
					break;
				case 1: ActivePane = ViewPaneType.BottomLeft;
					break;
				case 0: ActivePane = ViewPaneType.TopLeft;
					break;
			}
			SelectionAxis = (PivotTableAxis)reader.ReadUInt16(); 
			Dimension = reader.ReadUInt16();					 
			int begin = reader.ReadUInt16();
			CountSelection = reader.ReadUInt16();
			Minimum = reader.ReadUInt16();
			Maximum = reader.ReadUInt16();
			StartLines = begin;
			ActiveRow = reader.ReadUInt16();
			ActiveColumn = reader.ReadUInt16();
			PreviousRow = reader.ReadUInt16();
			PreviousColumn = reader.ReadUInt16();
			flag.ShortContainer = reader.ReadInt16();
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null) {
				builder.IsPivotFormat = false;
				builder.IsPivotSelection = false;
				ModelWorksheetView viewOptions = contentBuilder.CurrentSheet.ActiveView;
				if (ActivePane == viewOptions.ActivePaneType) {
					PivotSelection pSelection = contentBuilder.CurrentSheet.PivotSelection;
					builder.IsPivotSelection = true;
					pSelection.Pane = ActivePane;
					pSelection.Axis = SelectionAxis;
					pSelection.Start = StartLines;
					pSelection.Maximum = Maximum;
					pSelection.Minimum = Minimum;
					pSelection.ActiveRow = ActiveRow;
					pSelection.ActiveColumn = ActiveColumn;
					pSelection.PreviousRow = PreviousRow;
					pSelection.PreviousColumn = PreviousColumn;
					pSelection.CountClick = CountClick;
					pSelection.IsLabel = IsLabelOnly;
					pSelection.IsDataSelection = IsDataOnly;
					pSelection.IsShowHeader = IsToggleDataHeader;
					pSelection.IsExtendable = IsExtendable;
					pSelection.Dimension = Dimension;
					pSelection.CountSelection = CountSelection;
				}
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((short)0);
			int active = 0;
			switch (ActivePane) {
				case ViewPaneType.BottomRight: active = 3;
					break;
				case ViewPaneType.TopRight: active = 2;
					break;
				case ViewPaneType.BottomLeft: active = 1;
					break;
				case ViewPaneType.TopLeft: active = 0;
					break;
			}
			writer.Write((short)active);
			writer.Write((short)SelectionAxis);
			writer.Write((ushort)Dimension);
			writer.Write((ushort)startLines);
			writer.Write((ushort)CountSelection);
			writer.Write((ushort)Minimum);
			writer.Write((ushort)Maximum);
			writer.Write((ushort)ActiveRow);
			writer.Write((ushort)ActiveColumn);
			writer.Write((ushort)PreviousRow);
			writer.Write((ushort)PreviousColumn);
			writer.Write((ushort)flag.ShortContainer);
		}
		protected override short GetSize() {
			return (short)26;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
