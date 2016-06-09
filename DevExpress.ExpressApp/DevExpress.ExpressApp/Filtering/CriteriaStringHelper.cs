#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Persistent.Base;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Editors;
using System.Collections;
namespace DevExpress.ExpressApp.Filtering {
	public static class CriteriaStringHelper {
		class CriteriaConverterProcessor : CriteriaProcessorBase {
			public const string ParameterPrefix = "@";
			private const string ThisParameterPrefix = "@this.";
			private const string ReadOnlyParameterPrefix = "@@";
			protected override void Process(OperandValue theOperand) {
				string operandStringValue = theOperand.Value as string;
				if(!string.IsNullOrEmpty(operandStringValue)) {
					if(operandStringValue.StartsWith(ReadOnlyParameterPrefix)) {
						string parameterName = operandStringValue.Substring(ReadOnlyParameterPrefix.Length);
						theOperand.Value = ConvertToFunction(parameterName);
					}
					else if(operandStringValue.ToLower().StartsWith(ThisParameterPrefix)) {
						string parameterName = operandStringValue.Substring(ThisParameterPrefix.Length);
						theOperand.Value = ConvertToNamedParameter(parameterName);
					}
					else if(operandStringValue.StartsWith(ParameterPrefix)) {
						string parameterName = operandStringValue.Substring(ParameterPrefix.Length);
						if(ParametersFactory.GetRegisteredParameterNames().Contains(parameterName)) {
							theOperand.Value = ConvertToFunction(parameterName);
						}
						else {
							theOperand.Value = ConvertToNamedParameter(parameterName);
						}
					}
				}
			}
			private object ConvertToFunction(string parameterName) {
				string uniqueToken = Guid.NewGuid().ToString();
				Functions.Add(uniqueToken, parameterName);
				return uniqueToken;
			}
			private object ConvertToNamedParameter(string parameterName) {
				string uniqueToken = Guid.NewGuid().ToString();
				NamedParameters.Add(uniqueToken, parameterName);
				return uniqueToken;
			}
			public Dictionary<string, string> Functions = new Dictionary<string, string>();
			public Dictionary<string, string> NamedParameters = new Dictionary<string, string>();
		}
		[System.ComponentModel.DefaultValue(true)]
		public static bool AutomaticallyConvertFromOldFormat = true;
		public static string ConvertFromOldFormat(string criteria, IObjectSpace objectSpace, ITypeInfo objectTypeInfo) {
			if(!AutomaticallyConvertFromOldFormat || string.IsNullOrEmpty(criteria)) {
				return criteria;
			}
			CriteriaOperator criteriaOperator = null;
			if(objectSpace != null) {
				criteriaOperator = objectSpace.ParseCriteria(criteria);
			}
			else {
				criteriaOperator = CriteriaOperator.Parse(criteria);
			}
			if(objectSpace != null) {
				new FilterWithObjectsProcessor(objectSpace).Process(criteriaOperator, FilterWithObjectsProcessorMode.StringToObject);
			}
			if(objectTypeInfo != null) {
				new EnumPropertyValueCriteriaProcessor(objectTypeInfo).Process(criteriaOperator);
			}
			CriteriaConverterProcessor converter = new CriteriaConverterProcessor();
			converter.Process(criteriaOperator);
			string result = criteriaOperator.ToString();
			foreach(string token in converter.Functions.Keys) {
				result = result.Replace("'" + token + "'", converter.Functions[token] + "()");
			}
			foreach(string token in converter.NamedParameters.Keys) {
				result = result.Replace("'" + token + "'", "?" + converter.NamedParameters[token]);
			}
			return result;
		}
		public static CriteriaOperator Parse(string criteria) {
			return CriteriaOperator.Parse(criteria);
		}
		public static CriteriaOperator Parse(string criteria, object currentObject) {
			Guard.ArgumentNotNull(currentObject, "currentObject");
			OperandValue[] criteriaParametersList;
			CriteriaOperator result = CriteriaOperator.Parse(criteria, out criteriaParametersList);
			ITypeInfo ti = XafTypesInfo.Instance.FindTypeInfo(currentObject.GetType());
			foreach(OperandValue operandValue in criteriaParametersList) {
				OperandParameter operandParameter = operandValue as OperandParameter;
				if(!object.ReferenceEquals(operandParameter, null)) {
					IMemberInfo mi = ti.FindMember(operandParameter.ParameterName);
					Guard.ArgumentNotNull(mi, "memberInfo - '" + operandParameter.ParameterName + "'");
					operandParameter.Value = mi.GetValue(currentObject);
				}
			}
			return result;
		}
	}
}
