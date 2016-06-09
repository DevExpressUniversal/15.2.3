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
using System.Text;
namespace DevExpress.XtraExport {
	public enum XlsCellKind : byte {
		None,
		Integer,
		Double,
		BoolOrError,
		String
	}
	[CLSCompliant(false)]
	public class XlsCell {
		public XlsCellKind Kind;
		public ushort RecType;
		public string Text;
		public ushort XF;
		public double Num;
		public XlsCell() {
			Kind = XlsCellKind.None;
		}
		public int SSTIndex { get { return (int)Num; } set { Num = value; } }
		public bool BoolValue { get { return (SSTIndex & 1) == 1; } set { SSTIndex = value ? (SSTIndex | 1) : (SSTIndex & 0x100); } }
		public XlsCellErrType ErrValue { get { return (XlsCellErrType)(SSTIndex & 0xFF); } set { SSTIndex = (SSTIndex & 0x100) | ((int)value & 0xFF); } }
		public bool ErrFlag { get { return (SSTIndex & 0x100) == 0x100; } set { SSTIndex = value ? (SSTIndex | 0x100) : (SSTIndex & 0xFF); } }
		public ushort RecSize {
			get {
				switch(Kind) {
					case XlsCellKind.BoolOrError: return (ushort)(ErrFlag ? 8 : XlsConsts.BlankCellSize + 2);
					case XlsCellKind.None: return 6;
					case XlsCellKind.Double: return XlsConsts.BlankCellSize + 8;
					case XlsCellKind.Integer: return XlsConsts.BlankCellSize;
					case XlsCellKind.String: return (ushort)((Text.Length << 1) + 3 + 6);
				}
				throw new InvalidOperationException();
			}
		}
		public void WriteToStream(XlsStream stream, ushort row, ushort col) {
			stream.Write(BitConverter.GetBytes(RecType), 0, 2);
			stream.Write(BitConverter.GetBytes(RecSize), 0, 2);
			stream.Write(BitConverter.GetBytes(row), 0, 2);
			stream.Write(BitConverter.GetBytes(col), 0, 2);
			stream.Write(BitConverter.GetBytes(XF), 0, 2);
			switch(Kind) {
				case XlsCellKind.Integer:
					stream.Write(SSTIndex);
					break;
				case XlsCellKind.Double:
					stream.Write(DeleteNegativeZero(BitConverter.GetBytes(Num)), 0, 8);
					stream.Write((int)0);
					break;
				case XlsCellKind.BoolOrError:
					if(ErrFlag) {
						stream.Write((byte)ErrValue);
						stream.Write(ErrFlag);
					} else {
						stream.Write(BoolValue);
						stream.Write(ErrFlag);
						stream.Write((int)0);
					}
					break;
				case XlsCellKind.String:
					stream.Write((ushort)Text.Length);
					stream.Write(true);
					foreach(char ch in Text)
						stream.Write(BitConverter.GetBytes(ch), 0, 2);
					break;
			};
		}
		static byte[] DeleteNegativeZero(byte[] bytes) {
			if(bytes.Length != 8)
				return bytes;
			if(bytes[7] != 128)
				return bytes;
			for(int i = 0; i < 6; i++)
				if(bytes[i] != 0)
					return bytes;
			bytes[7] = 0;
			return bytes;
		}
	}
	[CLSCompliant(false)]
	public class XlsCellData {
		public static ushort PrepareCellStyle(ref ushort type) {
			ushort[] formats = { XlsConsts.DateTimeFormat, XlsConsts.DateFormat, XlsConsts.HourMinuteSecondFormat, XlsConsts.CurrencyNoneDecimalFormat };
			if((type & 0x1000) != 0) {
				ushort result = formats[type ^ 0x1000];
				type = XlsConsts.Number;
				return result == XlsConsts.DateFormat ? XlsConsts.DateTimeFormat : result;
			}
			else
				return 0;
		}
		XlsCell[] cellsList;
		int cellPerCol;
		SheetPictureCollection pictures;
		public XlsCellData() {
			pictures = new SheetPictureCollection();
		}
		public SheetPictureCollection Pictures { get { return pictures; } }
		public int FullSize {
			get {
				int result = 0;
				if(cellsList != null) {
					for(int i = 0; i < cellsList.Length; i++) {
						XlsCell cell = cellsList[i];
						if(cell.RecType != 0) {
							result += cell.RecSize;
							result += 4;
						}
					}
				}
				return result;
			}
		}
		public XlsCell[] CellsList {
			get {
				return cellsList;
			}
		}
		public ushort GetDateTimeFormat(ref double value_) {
			int val = (int)value_;
			if(val != 0 && val <= 60)
				value_--;
			ushort result = XlsConsts.DateTime;
			if(val == 0)
				result = XlsConsts.Time;
			else {
				if((value_ - val) == 0)
					result = XlsConsts.Date;
			}
			return result;
		}
		public XlsCell GetCell(int col, int row) {
			return cellsList != null ? cellsList[cellPerCol * col + row] : null;
		}
		public void SetCellDataBoolean(int col, int row, bool value_) {
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				cell.RecType = XlsConsts.BoolErr;
				cell.Kind = XlsCellKind.BoolOrError;
				cell.BoolValue = value_;
				cell.ErrFlag = false;
			}
		}
		public void SetCellDataError(int col, int row, XlsCellErrType error) {
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				cell.RecType = XlsConsts.BoolErr;
				cell.Kind = XlsCellKind.BoolOrError;
				cell.ErrValue = error;
				cell.ErrFlag = true;
			}
		}
		public void SetCellDataBlank(int col, int row) {
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				if(cell.RecType != XlsConsts.Blank) {
					cell.RecType = XlsConsts.Blank;
					cell.Kind = XlsCellKind.None;
				}
			}
		}
		public void SetCellDataDateTime(int col, int row, DateTime value_) {
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				double v = ExportUtils.ToOADate(value_);
				cell.RecType = GetDateTimeFormat(ref v);
				cell.Kind = XlsCellKind.Double;
				cell.Num = v;
			}
		}
		public void SetCellDataTimeSpan(int col, int row, TimeSpan value_) {
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				double v = (ExportUtils.TimeSpanStartDate.Add(value_)).ToOADate();
				cell.RecType = XlsConsts.Time;
				cell.Kind = XlsCellKind.Double;
				cell.Num = v;
			}
		}
		public void SetCellDataDouble(int col, int row, double value_) {
			if(double.IsNaN(value_)) {
				SetCellDataError(col, row, XlsCellErrType.NUM);
				return;
			} else if(double.IsInfinity(value_)) {
				SetCellDataError(col, row, XlsCellErrType.DIV_0);
				return;
			}
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				cell.RecType = XlsConsts.Number;
				cell.Kind = XlsCellKind.Double;
				cell.Num = value_;
			}
		}
		public void SetCellDataInteger(int col, int row, long value_) {
			SetCellDataDouble(col, row, value_);
		}
		public void SetCellDataSSTString(int col, int row, int index) {
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				cell.RecType = XlsConsts.LabelSST;
				cell.Kind = XlsCellKind.Integer;
				cell.SSTIndex = index;
			}
		}
		public void SetCellDataString(int col, int row, string text) {
			XlsCell cell = GetCell(col, row);
			if(cell != null) {
				cell.RecType = XlsConsts.Label;
				cell.Text = text;
				cell.Kind = XlsCellKind.String;
			}
		}
		public void SetCellDataImage(SheetPicture pic) {
			pictures.Add(pic);
			pic.XlsPicture.RefCount++;
		}
		public void SetRange(int colCount, int rowCount) {
			cellsList = new XlsCell[colCount * rowCount];
			cellPerCol = rowCount;
			for(int i = 0; i < colCount * rowCount; i++) {
				cellsList[i] = new XlsCell();
				cellsList[i].RecType = XlsConsts.Blank;
				cellsList[i].XF = XlsConsts.CountOfXFStyles;
			}
		}
		public void SaveToStream(XlsStream stream) {
			if(cellsList != null) {
				for(int i = 0; i < cellsList.Length; i++) {
					XlsCell cell = cellsList[i];
					ushort temp = XlsConsts.MergeState;
					temp = (ushort)~temp;
					cell.RecType &= temp;
					if(cell.RecType != 0)
						cell.WriteToStream(stream, (ushort)(i % cellPerCol), (ushort)(i / cellPerCol));
				}
			}
		}
	}
}
