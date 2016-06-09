#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Text;
using DevExpress.Data.PivotGrid;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.XtraPivotGrid;
using DevExpress.Data.Filtering;
namespace DevExpress.DashboardCommon.DataProcessing {
	public enum ExpressionMode {
		DataSourceLevel,
		SummaryLevel,
		AggregateFunction
	}
	public abstract class DataItemModel<TDataItem> : DataModelBase<TDataItem> where TDataItem : DataItemModel<TDataItem> {
		public string Name { get; private set; }
		public string DataMember { get; private set; }
		public string UnboundExpression { get; set; }
		public UnboundColumnType UnboundType { get; set; }
		public virtual ExpressionMode UnboundMode { get; set; }
		public string DrillDownName { get; set; }
		protected override bool ModelEquals(TDataItem other) {
			return Name == other.Name
				&& DataMember == other.DataMember
				&& UnboundExpression == other.UnboundExpression
				&& UnboundType == other.UnboundType
				&& UnboundMode == other.UnboundMode
				&& DrillDownName == other.DrillDownName;
		}
		protected DataItemModel(string dataMember, string name) {
			DXContract.Requires(!String.IsNullOrEmpty(name));
			this.DataMember = dataMember;
			this.Name = name;
		}
	}
	public class MeasureModel : DataItemModel<MeasureModel> {
		public SummaryType SummaryType { get; set; }
		public DataFieldType FieldType { get; set; }
		public bool DecimalSummary { get; set; }
		public bool IsVisible { get; set; }
		public MeasureModel(string dataMember, string name)
			: base(dataMember, name) {
			this.SummaryType = SummaryType.Sum;
			this.FieldType = DataFieldType.Decimal;
			this.IsVisible = true;
			this.DecimalSummary = false;
		}
		protected override int ModelHashCode() {
			return HashcodeHelper.GetCompositeHashCode<object>(Name, DataMember, UnboundExpression, UnboundType, UnboundMode, SummaryType,
				 FieldType, DecimalSummary, IsVisible, DrillDownName);
		}
		protected override bool ModelEquals(MeasureModel other) {
		 return base.ModelEquals(other)
				&& this.SummaryType == other.SummaryType
				&& FieldType == other.FieldType
				&& DecimalSummary == other.DecimalSummary
				&& IsVisible == other.IsVisible;
		}
		public override string ToString() {
			return Name;
		}
	}
}
