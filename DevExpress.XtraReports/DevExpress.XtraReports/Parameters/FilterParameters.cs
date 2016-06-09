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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using DevExpress.Data.Filtering;
namespace DevExpress.XtraReports.Native.Parameters {
	using DevExpress.XtraReports.Parameters;
	using System.Text.RegularExpressions;
	using DevExpress.XtraReports.UI;
	public class ReportParameterCollection : ParameterCollection {
		XtraReport report;
		internal override bool IsLoading { get { return report == null || report.Loading; } }
		public ReportParameterCollection(XtraReport report) {
			this.report = report;
		}
		protected internal override string Serialize(object value) {
			string result;
			return SerializationService.SerializeObject(value, out result, report) ? result : string.Empty;
		}
		protected internal override bool Deserialize(string value, string typeName, out object result) {
			return SerializationService.DeserializeObject(value, typeName, out result, report);
		}
	}
	public class ParametersReplacer : ClientCriteriaVisitorBase {
		#region static
		public const string ParametersName = "Parameters";
		static readonly Regex inOperandRegex = new Regex(@"^(.*)\[" + ParametersReplacer.ParametersName + @"\.(.*?)\](.*)$");
		internal static string GetParameterName(string s) {
			string[] items = s.Split('.');
			return items.Length == 2 && items[0] == ParametersReplacer.ParametersName ? items[1] : null;
		}
		internal static string GetParameterFormattedName(string name) {
			return string.Concat("[", GetParameterFullName(name), "]");
		}
		internal static string GetParameterFullName(string name) {
			return string.Concat(ParametersReplacer.ParametersName, ".", name);
		}
		#endregion
		public static string UpgradeFilterString(string filterString) {
			CriteriaOperator criteriaOperator = CriteriaOperator.Parse(filterString);
			if(ReferenceEquals(criteriaOperator, null))
				return filterString;
			(new ParametersReplacer()).Process(criteriaOperator);
			return criteriaOperator.ToString();
		}
		public override CriteriaOperator Visit(OperandProperty theOperand) {
			string parameterName = GetParameterName(theOperand.PropertyName);
			if(!string.IsNullOrEmpty(parameterName)) {
				return new OperandParameter(parameterName);
			}
			return theOperand;
		}
		public override CriteriaOperator Visit(OperandValue theOperand) {
			string value = theOperand.Value as string;
			if(value != null) {
				Match match = inOperandRegex.Match(value);
				if(match.Success) { 
					string prefix = match.Groups[1].Value;
					string parameterName =  match.Groups[2].Value;
					string suffix = match.Groups[3].Value;
					if(string.IsNullOrEmpty(prefix) && string.IsNullOrEmpty(suffix))
						return new OperandParameter(parameterName);
					return new FunctionOperator(FunctionOperatorType.Concat, CreateConcatParams(prefix, parameterName, suffix));
				}
			}
			return theOperand;
		}
		static CriteriaOperator[] CreateConcatParams(string prefix, string parameterName, string suffix) {
			List<CriteriaOperator> concatParams = new List<CriteriaOperator>();
			if(!string.IsNullOrEmpty(prefix))
				concatParams.Add(new OperandValue(prefix));
			concatParams.Add(new OperandParameter(parameterName));
			if(!string.IsNullOrEmpty(suffix))
				concatParams.Add(new OperandValue(suffix));
			return concatParams.ToArray();
		}
	}
}
