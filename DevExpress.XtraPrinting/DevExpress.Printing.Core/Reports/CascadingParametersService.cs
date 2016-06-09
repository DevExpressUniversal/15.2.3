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
using System.Linq;
using DevExpress.Data.Filtering;
namespace DevExpress.XtraReports.Parameters {
	public class CascadingParametersService {
		public static IEnumerable<Parameter> GetDependentParameters(IEnumerable<Parameter> parameters, Parameter analizedParameter) {
			var parametersCollection = analizedParameter.Owner;
			if(parametersCollection == null) {
				throw new InvalidOperationException("Parameter is not in the ParameterCollection.");
			}
			return from p in parametersCollection.Cast<Parameter>()
				   where p != analizedParameter && IsDependentParameter(analizedParameter.Name, p)
				   select p;
		}
		public static bool IsDependentParameter(string parentParameterName, Parameter analizedParameter) {
			if(analizedParameter.LookUpSettings == null)
				return false;
			OperandValue[] operands;
			var criteria = CriteriaOperator.Parse(analizedParameter.LookUpSettings.FilterString, out operands);
			return !ReferenceEquals(criteria, null)
				&& !ReferenceEquals(operands.FirstOrDefault(x => x is OperandParameter && ((OperandParameter)x).ParameterName == parentParameterName), null);
		}
		public static IEnumerable<Parameter> GetMajorParameters(Parameter analizedParameter) {
			var parametersCollection = analizedParameter.Owner;
			if(parametersCollection == null) {
				throw new InvalidOperationException("Parameter is not in the ParameterCollection.");
			}
			var majorParameters = new List<Parameter>();
			if(analizedParameter.LookUpSettings == null || string.IsNullOrEmpty(analizedParameter.LookUpSettings.FilterString))
				return majorParameters;
			OperandValue[] operands;
			var criteria = CriteriaOperator.Parse(analizedParameter.LookUpSettings.FilterString, out operands);
			foreach(var parameter in parametersCollection.Cast<Parameter>().TakeWhile(x => x != analizedParameter)) {
				if(operands.OfType<OperandParameter>().Any(x => x.ParameterName == parameter.Name))
					majorParameters.Add(parameter);
			}
			return majorParameters;
		}
		public static bool ValidateFilterString(LookUpSettings settings, out string error) {
			var parametersCollection = settings.Parameter.Owner;
			if(parametersCollection == null) {
				error = "Parameter is not in the ParameterCollection.";
				return false;
			}
			OperandValue[] operands;
			CriteriaOperator criteria;
			try {
				criteria = CriteriaOperator.Parse(settings.FilterString, out operands);
			} catch(Exception e) {
				error = e.Message;
				return false;
			}
			foreach(var operandParameter in operands.OfType<OperandParameter>()) {
				var parameter = ((IEnumerable<Parameter>)settings.Parameter.Owner)
					.FirstOrDefault(x => x.Name == operandParameter.ParameterName);
				if(parameter == null) {
					error = string.Format("It is impossible to find a parameter which determines the filter string of Cascading Parameter. FilterString: {0}", settings.FilterString);
					return false;
				}
				if(settings.Parameter.Owner.IndexOf(parameter) >= settings.Parameter.Owner.IndexOf(settings.Parameter)) {
					error = string.Format("Cascading parameter can be only bound to parameters that are higher on the list of ParameterCollection. FilterString: {0}", settings.FilterString);
					return false;
				}
			}
			error = null;
			return true;
		}
		public static bool ValidateFilterStrings(IEnumerable<Parameter> parameters, out string error) {
			var cascadingParameters = parameters.Where(x=> x.HasCascadeLookUpSettings());
			foreach(var parameter in cascadingParameters) {
				if(!ValidateFilterString(parameter.LookUpSettings, out error))
					return false;
			}
			error = null;
			return true;
		}
	}
}
