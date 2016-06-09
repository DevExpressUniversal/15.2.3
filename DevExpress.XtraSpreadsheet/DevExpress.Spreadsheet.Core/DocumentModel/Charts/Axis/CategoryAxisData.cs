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
	public class CategoryAxis : AxisTickBase, ICloneable<CategoryAxis>, ISupportsCopyFrom<CategoryAxis> {
		public CategoryAxis(IChart parent) 
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
		protected internal DocumentModelChangeActions SetAutoCore(AxisInfo info, bool value) {
			info.Auto = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region LabelAlign
		public LabelAlignment LabelAlign {
			get { return Info.LabelAlign; }
			set {
				if(LabelAlign == value)
					return;
				SetPropertyValue(SetLabelAlignCore, value);
			}
		}
		DocumentModelChangeActions SetLabelAlignCore(AxisInfo info, LabelAlignment value) {
			info.LabelAlign = value;
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
		#region NoMultilevelLabels
		public bool NoMultilevelLabels {
			get { return Info.NoMultilevelLabels; }
			set {
				if(NoMultilevelLabels == value)
					return;
				SetPropertyValue(SetNoMultilevelLabelsCore, value);
			}
		}
		DocumentModelChangeActions SetNoMultilevelLabelsCore(AxisInfo info, bool value) {
			info.NoMultilevelLabels = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		#region ICloneable<CategoryAxis> Members
		public CategoryAxis Clone() {
			CategoryAxis result = new CategoryAxis(Parent);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<CategoryAxis> Members
		public void CopyFrom(CategoryAxis value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
		}
		#endregion
		protected override AxisBase CloneToCore(IChart parent) {
			CategoryAxis result = new CategoryAxis(parent);
			result.CopyFrom(this);
			return result;
		}
		public override void Visit(IAxisVisitor visitor)
		{
			visitor.Visit(this);
		}
		protected override void CopyLayoutCore(AxisBase value) {
			CategoryAxis other = value as CategoryAxis;
			if (other != null) {
				base.CopyLayoutCore(value);
				Auto = other.Auto;
				LabelAlign = other.LabelAlign;
				LabelOffset = other.LabelOffset;
				NoMultilevelLabels = other.NoMultilevelLabels;
			}
		}
		public override string GetAxisLabelText(object axisValue) {
			if (axisValue == null)
				return string.Empty;
			if (axisValue is double) {
				try {
					double value = Convert.ToDouble(axisValue);
					return NumberFormat.Formatter.Format(value, DocumentModel.DataContext).Text;
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
	}
}
