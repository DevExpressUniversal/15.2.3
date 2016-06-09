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
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	using DevExpress.XtraEditors;
	public class FormatConditionDateOccuring : FormatConditionStyleBase {
		const string XmlDateType = "DateType";
		const FilterDateType DefaultDateType = FilterDateType.Yesterday;
		FilterDateType dateType = DefaultDateType;
		ExpressionEvaluator evaluator = null;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("FormatConditionDateOccuringDateType"),
#endif
		DefaultValue(DefaultDateType)
		]
		public FilterDateType DateType {
			get { return dateType;}
			set { SetDateTypeInternal(value, true); }
		}
		public void ResetCurrentDate() {
			OnChanged();
		}
		protected override FormatConditionBase CreateInstance() {
			return new FormatConditionDateOccuring();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			XmlHelper.Save(element, XmlDateType, DateType, DefaultDateType);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			XmlHelper.LoadEnum<FilterDateType>(element, XmlDateType, x => SetDateTypeInternal(x, false));
		}
		protected override void AssignCore(FormatConditionBase obj) {
			base.AssignCore(obj);
			var source = obj as FormatConditionDateOccuring;
			if(source != null) {
				DateType = source.DateType;
			}
		}
		protected override bool IsFitCore(IFormatConditionValueProvider valueProvider) {
			object value = valueProvider.GetValue(this);
			return CheckValue(value) && (evaluator != null) && (value is DateTime) ? evaluator.Fit((DateTime)value) : false;
		}
		void SetDateTypeInternal(FilterDateType dateType, bool forceChanged) {
			FilterDateType type = (~(FilterDateType.SpecificDate | FilterDateType.User)) & dateType;
			if(DateType != type) {
				this.dateType = type;
				UpdateExpressionEvaluator();
				if(forceChanged)
					OnChanged();
			}
		}
		void UpdateExpressionEvaluator() {
			if(DateType != FilterDateType.None) {
				CriteriaOperator criteria = FilterDateTypeHelper.ToCriteria(new OperandProperty("this"), DateType);
				this.evaluator = new ExpressionEvaluator(new PropertyDescriptorCollection(null), criteria);
			} else {
				this.evaluator = null;
			}
		}
	}
}
