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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class DateAxis : AxisMMUnitsBase, ICloneable<DateAxis>, ISupportsCopyFrom<DateAxis> {
		public DateAxis(IChart parent)
			: base(parent) {
		}
		#region Properties
		public override AxisDataType AxisType { get { return AxisDataType.Agrument; } }
		#region Auto
		public bool Auto {
			get { return Info.Auto; }
			set {
				if(Auto == value)
					return;
				SetPropertyValue(SetAutoCore, value);
			}
		}
		DocumentModelChangeActions SetAutoCore(AxisInfo info, bool value) {
			info.Auto = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region LabelOffset
		public int LabelOffset {
			get { return Info.LabelOffset; }
			set {
				if(LabelOffset == value)
					return;
				SetPropertyValue(SetLabelOffsetCore, value);
			}
		}
		DocumentModelChangeActions SetLabelOffsetCore(AxisInfo info, int value) {
			info.LabelOffset = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region BaseTimeUnit
		public TimeUnits BaseTimeUnit {
			get { return Info.BaseTimeUnit; }
			set {
				if(BaseTimeUnit == value)
					return;
				SetPropertyValue(SetBaseTimeUnitCore, value);
			}
		}
		DocumentModelChangeActions SetBaseTimeUnitCore(AxisInfo info, TimeUnits value) {
			info.BaseTimeUnit = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MajorTimeUnit
		public TimeUnits MajorTimeUnit {
			get { return Info.MajorTimeUnit; }
			set {
				if(MajorTimeUnit == value)
					return;
				SetPropertyValue(SetMajorTimeUnitCore, value);
			}
		}
		DocumentModelChangeActions SetMajorTimeUnitCore(AxisInfo info, TimeUnits value) {
			info.MajorTimeUnit = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MinorTimeUnit
		public TimeUnits MinorTimeUnit { 
			get { return Info.MinorTimeUnit; }
			set {
				if(MinorTimeUnit == value)
					return;
				SetPropertyValue(SetMinorTimeUnitCore, value);
			}
		}
		DocumentModelChangeActions SetMinorTimeUnitCore(AxisInfo info, TimeUnits value) {
			info.MinorTimeUnit = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		#region ICloneable<DateAxis> Members
		public DateAxis Clone() {
			DateAxis result = new DateAxis(this.Parent);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DateAxis> Members
		public void CopyFrom(DateAxis value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
		}
		#endregion
		protected override AxisBase CloneToCore(IChart parent) {
			DateAxis result = new DateAxis(parent);
			result.CopyFrom(this);
			return result;
		}
		public override void Visit(IAxisVisitor visitor) {
			visitor.Visit(this);
		}
		#region IAxisLabelFormatterCore Members
		public override string GetAxisLabelText(object axisValue) {
			if (axisValue == null)
				return string.Empty;
			if (axisValue is DateTime) {
				try {
					DateTime dateTimeValue = Convert.ToDateTime(axisValue);
					VariantValue value = this.DocumentModel.DataContext.FromDateTime(dateTimeValue);
					return NumberFormat.Formatter.Format(value, DocumentModel.DataContext).Text;
				}
				catch { }
			}
			return axisValue.ToString();
		}
		#endregion
		protected override void CopyLayoutCore(AxisBase value) {
			DateAxis other = value as DateAxis;
			if (other != null) {
				base.CopyLayoutCore(value);
				Auto = other.Auto;
				LabelOffset = other.LabelOffset;
				BaseTimeUnit = other.BaseTimeUnit;
				MajorTimeUnit = other.MajorTimeUnit;
				MinorTimeUnit = other.MinorTimeUnit;
			}
		}
		public TimeUnits GetTimeUnits() {
			TimeUnits timeUnits = MajorTimeUnit;
			if (timeUnits == TimeUnits.Auto)
				timeUnits = BaseTimeUnit;
			return timeUnits;
		}
	}
}
