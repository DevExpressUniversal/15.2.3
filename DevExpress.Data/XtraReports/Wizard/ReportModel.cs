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

using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.WizardFramework;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.Utils;
namespace DevExpress.Data.XtraReports.Wizard {
	public class ReportModel : IWizardModel {
		#region Fields and Properties
		public string DataSourceName { get; set; }
		public TableInfo DataMemberName { get; set; }
		public string[] Columns { get; set; }
		public HashSet<string>[] GroupingLevels { get; set; }
		public ReportType ReportType { get; set; }
		public bool Portrait { get; set; }
		public bool AdjustFieldWidth { get; set; }
		public HashSet<ColumnNameSummaryOptions> SummaryOptions { get; set; }
		public bool IgnoreNullValuesForSummary { get; set; }
		public ReportLayout Layout { get; set; }
		public ReportStyleId ReportStyleId { get; set; }
		public string ReportTitle { get; set; }
		public int LabelProductId { get; set; }
		public int LabelProductDetailId { get; set; }
		public CustomLabelInformation CustomLabelInformation { get; set; }
		#endregion
		#region ctor
		public ReportModel() {
			ReportType = Wizard.ReportType.Standard;
			Portrait = true;
			AdjustFieldWidth = true;
			Layout = ReportLayout.Columnar;
			GroupingLevels = new HashSet<string>[] { };
			SummaryOptions = new HashSet<ColumnNameSummaryOptions>();
		}
		protected ReportModel(ReportModel source) {
			DataSourceName = source.DataSourceName;
			if(source.DataMemberName != null)
				DataMemberName = (TableInfo)source.DataMemberName.Clone();
			if(source.Columns != null) {
				Columns = source.Columns.ToArray();
			}
			if(source.GroupingLevels != null) {
				var clonedGroupingLevels = new HashSet<string>[source.GroupingLevels.Length];
				for(int i = 0; i < source.GroupingLevels.Length; i++) {
					clonedGroupingLevels[i] = new HashSet<string>(source.GroupingLevels[i]);
				}
				GroupingLevels = clonedGroupingLevels;
			}
			ReportType = source.ReportType;
			Portrait = source.Portrait;
			AdjustFieldWidth = source.AdjustFieldWidth;
			if(source.SummaryOptions != null) {
				SummaryOptions = new HashSet<ColumnNameSummaryOptions>(source.SummaryOptions.Select(x => (ColumnNameSummaryOptions)x.Clone()));
			}
			IgnoreNullValuesForSummary = source.IgnoreNullValuesForSummary;
			Layout = source.Layout;
			ReportStyleId = source.ReportStyleId;
			ReportTitle = source.ReportTitle;
			LabelProductId = source.LabelProductId;
			LabelProductDetailId = source.LabelProductDetailId;
			if(source.CustomLabelInformation != null) {
				CustomLabelInformation = (CustomLabelInformation)source.CustomLabelInformation.Clone();
			}
		}
		#endregion
		public override bool Equals(object obj) {
			ReportModel other = obj as ReportModel;
			if(other == null)
				return false;
			if(!object.Equals(DataSourceName, other.DataSourceName))
				return false;
			if(!object.Equals(DataMemberName, other.DataMemberName))
				return false;
			if(!ArrayHelper.ArraysEqual(Columns, other.Columns))
				return false;
			if(!ArrayHelper.ArraysEqual(GroupingLevels, other.GroupingLevels, new HashSetEqualityComparer<string>()))
				return false;
			if(ReportType != other.ReportType)
				return false;
			if(Portrait != other.Portrait)
				return false;
			if(AdjustFieldWidth != other.AdjustFieldWidth)
				return false;
			if(!HashSetsEqual(SummaryOptions, other.SummaryOptions))
				return false;
			if(IgnoreNullValuesForSummary != other.IgnoreNullValuesForSummary)
				return false;
			if(Layout != other.Layout)
				return false;
			if(ReportStyleId != other.ReportStyleId)
				return false;
			if(!object.Equals(ReportTitle, other.ReportTitle))
				return false;
			if(LabelProductId != other.LabelProductId)
				return false;
			if(LabelProductDetailId != other.LabelProductDetailId)
				return false;
			if(!object.Equals(CustomLabelInformation, other.CustomLabelInformation))
				return false;
			return true;
		}
		public override int GetHashCode() {
			return 0;
		}
		public bool IsGrouped() {
			return GroupingLevels.Length > 0;
		}
		static bool HashSetsEqual<T>(HashSet<T> x, HashSet<T> y) {
			if(x == null ^ y == null)
				return false;
			if(x == null)
				return true;
			return x.SetEquals(y);
		}
		#region ICloneable Members
		public virtual object Clone() {
			return new ReportModel(this);
		}
		#endregion
	}
}
