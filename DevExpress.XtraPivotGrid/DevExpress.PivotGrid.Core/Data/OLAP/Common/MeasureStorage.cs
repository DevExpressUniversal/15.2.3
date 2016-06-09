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
using DevExpress.Data.IO;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP {
	public struct OLAPCellValue {
		public static explicit operator PivotCellValue(OLAPCellValue val) {
			return PivotCellValue.Create(val.Value,  OLAPFormatHelper.Format(val));
		}
		public static OLAPCellValue Empty {
			get {
				return new OLAPCellValue();
			}
		}
		public object Value { get; set; }
		public string FormatString { get; set; }
		public int Locale { get; set; }
	}
	abstract class OlapMeasuresStorage : MeasuresStorage {
		protected static void SaveCellToStream(TypedBinaryWriter writer, int index, OLAPCellValue cell) {
			writer.Write(index);
			writer.WriteTypedObject(cell.Value);
			writer.WriteNullableString(cell.FormatString);
			writer.Write(cell.Locale);
		}
		public virtual void LoadFromStream(TypedBinaryReader reader, List<IQueryMetadataColumn> columnIndexes, int summaryCount) {
			for(int i = 0; i < summaryCount; i++) {
				int columnIndex = reader.ReadInt32();
				object value = reader.ReadTypedObject();
				string format = OLAPFormatHelper.InternSimple(reader.ReadNullableString());
				int locale = reader.ReadInt32();
				this.SetFormattedValue(columnIndexes[columnIndex], value, format, locale);
			}
		}
		public void SetFormattedValue(IQueryMetadataColumn column, IOLAPCell cell) {
			if(cell == null)
				return;
			try {
				this.SetFormattedValue(column, cell.Value, cell.FormatString, cell.Locale);
			} catch {
				this.SetFormattedValue(column, PivotCellValue.ErrorValue.Value, null, -1);
			}
		}
	}
	class OneMeasureStorage : OlapMeasuresStorage {
		IQueryMetadataColumn measure1;
		OLAPCellValue cell1;
		public override int Count { get { return HasMeasureCell1 ? 1 : 0; } }
		protected bool HasMeasureCell1 { get { return measure1 != null; } }
		public override PivotCellValue GetPivotCellValue(IQueryMetadataColumn measure) {
			return measure1 == measure ? (PivotCellValue)cell1 : null;
		}
		public override object GetValue(IQueryMetadataColumn measure) {
			return measure1 == measure ? cell1.Value : null;
		}
		public override bool SetFormattedValue(IQueryMetadataColumn measure, object value, string format, int locale) {
			if(measure1 == null || measure1 == measure) {
				if(value == null) {
					measure1 = null;
					return true;
				}
				measure1 = measure;
				cell1.Value = value;
				cell1.FormatString = format;
				cell1.Locale = locale;
				return true;
			}
			return false;
		}
		public override void SaveToStream(MeasureStorageKeepHelperBase helper, TypedBinaryWriter writer, Dictionary<IQueryMetadataColumn, int> columnIndexes) {
			writer.Write(Count);
			if(HasMeasureCell1)
				SaveCellToStream(writer, columnIndexes[measure1], cell1);
		}
		internal override void Remove(IQueryMetadataColumn column) {
			if(measure1 == column) {
				measure1 = null;
			}
		}
	}
	class TwoMeasuresStorage : OneMeasureStorage {
		IQueryMetadataColumn measure2;
		OLAPCellValue cell2;
		public override int Count { get { return HasMeasureCell2 ? base.Count + 1 : base.Count; } }
		protected bool HasMeasureCell2 { get { return measure2 != null; } }
		public override PivotCellValue GetPivotCellValue(IQueryMetadataColumn measure) {
			return this.measure2 == measure ? (PivotCellValue)cell2 : base.GetPivotCellValue(measure);
		}
		public override object GetValue(IQueryMetadataColumn measure) {
			return this.measure2 == measure ? cell2.Value : base.GetValue(measure);
		}
		public override bool SetFormattedValue(IQueryMetadataColumn measure, object value, string format, int locale) {
			if(measure2 == measure) {
				if(value == null) {
					measure2 = null;
					return true;
				}
				cell2.Value = value;
				cell2.FormatString = format;
				cell2.Locale = locale;
				return true;
			}
			if(base.SetFormattedValue(measure, value, format, locale))
				return true;
			if(measure2 == null) {
				if(value == null)
					return true;
				measure2 = measure;
				cell2.Value = value;
				cell2.FormatString = format;
				cell2.Locale = locale;
				return true;
			}
			return false;
		}
		public override void SaveToStream(MeasureStorageKeepHelperBase helper, TypedBinaryWriter writer, Dictionary<IQueryMetadataColumn, int> columnIndexes) {
			base.SaveToStream(helper, writer, columnIndexes);
			if(HasMeasureCell2)
				SaveCellToStream(writer, columnIndexes[measure2], cell2);
		}
		internal override void Remove(IQueryMetadataColumn column) {
			if(measure2 == column) {
				measure2 = null;
			} else
				base.Remove(column);
		}
	}
	class ThreeMeasuresStorage : TwoMeasuresStorage {
		IQueryMetadataColumn measure3;
		OLAPCellValue cell3;
		public override int Count { get { return HasMeasureCell3 ? base.Count + 1 : base.Count; } }
		protected bool HasMeasureCell3 { get { return measure3 != null; } }
		public override PivotCellValue GetPivotCellValue(IQueryMetadataColumn measure) {
			return measure3 == measure ? (PivotCellValue)cell3 : base.GetPivotCellValue(measure);
		}
		public override object GetValue(IQueryMetadataColumn measure) {
			 return measure3 == measure ? cell3.Value : base.GetValue(measure);
		}
		public override bool SetFormattedValue(IQueryMetadataColumn measure, object value, string format, int locale) {
			if(measure3 == measure) {
				if(value == null) {
					measure3 = null;
					return true;
				}
				cell3.Value = value;
				cell3.FormatString = format;
				cell3.Locale = locale;
				return true;
			}
			if(base.SetFormattedValue(measure, value, format, locale))
				return true;
			if(measure3 == null) {
				if(value == null)
					return true;
				measure3 = measure;
				cell3.Value = value;
				cell3.FormatString = format;
				cell3.Locale = locale;
				return true;
			}
			return false;
		}
		public override void SaveToStream(MeasureStorageKeepHelperBase helper, TypedBinaryWriter writer, Dictionary<IQueryMetadataColumn, int> columnIndexes) {
			base.SaveToStream(helper, writer, columnIndexes);
			if(HasMeasureCell3)
				SaveCellToStream(writer, columnIndexes[measure3], cell3);
		}
		internal override void Remove(IQueryMetadataColumn column) {
			if(measure3 == column) {
				measure3 = null;
			} else {
				base.Remove(column);
			}
		}
	}
	class MultipleMeasuresStorage : OlapMeasuresStorage {
		readonly Dictionary<IQueryMetadataColumn, OLAPCellValue> summaries;
		public MultipleMeasuresStorage() {
			summaries = new Dictionary<IQueryMetadataColumn, OLAPCellValue>();
		}
		public override int Count { get { return summaries.Count; } }
		public override PivotCellValue GetPivotCellValue(IQueryMetadataColumn measure) {
			OLAPCellValue res;
			return summaries.TryGetValue(measure, out res) ? (PivotCellValue)res : null;
		}
		public override object GetValue(IQueryMetadataColumn measure) {
			OLAPCellValue res;
			return summaries.TryGetValue(measure, out res) ? res.Value : null;
		}
		public override bool SetFormattedValue(IQueryMetadataColumn measure, object value, string format, int locale) {
			if(value == null) {
				summaries.Remove(measure);
				return true;
			}
			summaries[measure] = new OLAPCellValue() { Value = value, FormatString = format, Locale = locale };
			return true;
		}
		public override void SaveToStream(MeasureStorageKeepHelperBase helper, TypedBinaryWriter writer, Dictionary<IQueryMetadataColumn, int> columnIndexes) {
			writer.Write(Count);
			foreach(KeyValuePair<IQueryMetadataColumn, OLAPCellValue> column in summaries) {
				SaveCellToStream(writer, columnIndexes[column.Key], column.Value);
			}
		}
		internal override void Remove(IQueryMetadataColumn column) {
			summaries.Remove(column);
		}
	}
}
