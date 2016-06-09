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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Utils;
namespace DevExpress.PivotGrid.CriteriaVisitors {
	public class SummaryLevelCriteriaValidator : IUniqueFieldNameGenerator {
		int counter = 0;
		public Type Validate(CriteriaOperator criteria, PropertyDescriptorCollection properties) {
			MixedSummaryLevelCriteriaVisitor visitor = new MixedSummaryLevelCriteriaVisitor(this);
			CriteriaOperator summaryCriteria = criteria.Accept(visitor);
			List<PropertyDescriptor> descr = new List<PropertyDescriptor>();
			foreach(KeyValuePair<CriteriaOperator, MixedSummaryLevelCriteriaVisitor.DataSourceCriteria> pair in visitor.SummaryLevel) {
				Type dataType = ValidateDataSourceCriteria(pair.Key, properties);
				foreach(MixedSummaryLevelCriteriaVisitor.SummaryCriteria sCriteria in pair.Value.SummaryCriterias) {
					descr.Add(new SimplePropertyDescriptor(sCriteria.SummaryLevelName, sCriteria.SummaryLevelName, sCriteria.GetDataType(dataType)));
					dataType = Nullable.GetUnderlyingType(dataType) ?? dataType;
					if(dataType != typeof(object))
						switch(sCriteria.SummaryType) {
							case PivotSummaryType.Custom:
								if(!sCriteria.CustomAggregate.IsValidOperandCount(1) || !sCriteria.CustomAggregate.IsValidOperandType(0, 1, dataType))
									throw new ArgumentException(sCriteria.CustomAggregate.GetType().FullName + "Can't accept operand type: " + dataType.FullName);
								break;
							case PivotSummaryType.Max:
							case PivotSummaryType.Min:
								if(!dataType.GetInterfaces().Contains(typeof(IComparable)))
									throw new ArgumentException(dataType.FullName + " is not IComparable");
								break;
							case PivotSummaryType.Average:
							case PivotSummaryType.StdDev:
							case PivotSummaryType.StdDevp:
							case PivotSummaryType.Sum:
							case PivotSummaryType.Var:
							case PivotSummaryType.Varp:
								Convert.ChangeType(DevExpress.XtraReports.Parameters.ParameterHelper.GetDefaultValue(dataType), typeof(double));
								break;
							case PivotSummaryType.Count:
								break;
						}
				}
			}
			return ValidateDataSourceCriteria(summaryCriteria, new PropertyDescriptorCollection(descr.ToArray()));
		}
		Type ValidateDataSourceCriteria(CriteriaOperator criteria, PropertyDescriptorCollection properties) {
			return CriteriaCompiler.ToLambda(criteria, new BaseRowStub.DataControllerCriteriaCompilerDescriptor(() => properties)).ReturnType;
		}
		public string Generate(string prefix) {
			return prefix + counter++;
		}
	}
}
