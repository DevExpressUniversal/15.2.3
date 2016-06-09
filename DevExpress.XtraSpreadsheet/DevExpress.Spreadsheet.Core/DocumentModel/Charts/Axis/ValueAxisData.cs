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
	#region ValueAxis
	public class ValueAxis : AxisMMUnitsBase, ICloneable<ValueAxis>, ISupportsCopyFrom<ValueAxis> {
		#region Fields
		DisplayUnitOptions displayUnit;
		#endregion
		public ValueAxis(IChart parent)
			: base(parent) {
			this.displayUnit = new DisplayUnitOptions(parent);
		}
		#region Properties
		public override AxisDataType AxisType { get { return AxisDataType.Value; } }
		#region CrossBetween
		public AxisCrossBetween CrossBetween {
			get { return Info.CrossBetween; }
			set {
				if (CrossBetween == value)
					return;
				SetPropertyValue(SetCrossBetweenCore, value);
			}
		}
		DocumentModelChangeActions SetCrossBetweenCore(AxisInfo info, AxisCrossBetween value) {
			info.CrossBetween = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public DisplayUnitOptions DisplayUnit { get { return displayUnit; } }
		#endregion
		#region ICloneable<ValueAxis> Members
		public ValueAxis Clone() {
			ValueAxis result = new ValueAxis(this.Parent);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ValueAxis> Members
		public void CopyFrom(ValueAxis value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			this.displayUnit.CopyFrom(value.displayUnit);
		}
		#endregion
		protected override AxisBase CloneToCore(IChart parent) {
			ValueAxis result = new ValueAxis(parent);
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
			if (axisValue is string)
				return axisValue.ToString();
			if (axisValue is double) {
				try {
					double value = Convert.ToDouble(axisValue);
					return NumberFormat.Formatter.Format(GetValueInDisplayUnits(value), DocumentModel.DataContext).Text;
				}
				catch { }
			}
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
		double GetValueInDisplayUnits(double value) {
			switch (DisplayUnit.UnitType) {
				case DisplayUnitType.Hundreds:
					return value / 1E+2;
				case DisplayUnitType.Thousands:
					return value / 1E+3;
				case DisplayUnitType.TenThousands:
					return value / 1E+4;
				case DisplayUnitType.HundredThousands:
					return value / 1E+5;
				case DisplayUnitType.Millions:
					return value / 1E+6;
				case DisplayUnitType.TenMillions:
					return value / 1E+7;
				case DisplayUnitType.HundredMillions:
					return value / 1E+8;
				case DisplayUnitType.Billions:
					return value / 1E+9;
				case DisplayUnitType.Trillions:
					return value / 1E+12;
				case DisplayUnitType.Custom:
					return value / DisplayUnit.CustomUnit;
			}
			return value;
		}
		public override void ResetToStyle() {
			base.ResetToStyle();
			DisplayUnit.ResetToStyle();
		}
		protected internal override void CopyLayout(AxisBase value, bool percentStacked) {
			ValueAxis other = value as ValueAxis;
			if (other == null)
				return;
			if (!percentStacked)
				CopyLayoutCore(other);
			else {
				MajorTickMark = other.MajorTickMark;
				MinorTickMark = other.MinorTickMark;
				TickLabelPos = other.TickLabelPos;
				ShowMajorGridlines = other.ShowMajorGridlines;
				ShowMinorGridlines = other.ShowMinorGridlines;
				MajorGridlines.CopyFrom(other.MajorGridlines);
				MinorGridlines.CopyFrom(other.MinorGridlines);
				ShapeProperties.CopyFrom(other.ShapeProperties);
				TextProperties.CopyFrom(other.TextProperties);
				CrossBetween = other.CrossBetween;
			}
		}
		protected override void CopyLayoutCore(AxisBase value) {
			ValueAxis other = value as ValueAxis;
			if (other != null) {
				base.CopyLayoutCore(value);
				CrossBetween = other.CrossBetween;
				this.displayUnit.CopyFrom(other.displayUnit);
			}
		}
		#region Notifications
		public override void OnRangeInserting(InsertRangeNotificationContext context) {
			DisplayUnit.OnRangeInserting(context);
		}
		public override void OnRangeRemoving(RemoveRangeNotificationContext context) {
			DisplayUnit.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
}
