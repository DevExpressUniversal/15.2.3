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
using System.Linq;
using System.Text;
using System.Activities;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Validation;
using DevExpress.Persistent.Base;
using System.ComponentModel;
using DevExpress.Workflow.Utils;
using DevExpress.ExpressApp.Editors;
using System.Activities.XamlIntegration;
using System.IO;
using DevExpress.ExpressApp.Workflow.StartWorkflowConditions;
using System.Activities.Validation;
using DevExpress.Utils;
using System.Collections;
namespace DevExpress.ExpressApp.Workflow {
	public class ActivityXamlValidator : IActivityXamlValidator {
		public ActivityXamlValidator() { }
		public bool IsValidActivityXaml(string xaml) {
			try {
				ActivityXamlServices.Load(new StringReader(xaml));
				return true;
			}
			catch {
				return false;
			}
		}
	}
	public static class RunningWorkflowInstanceInfoHelper {
		private static string ConvertKeyToString(object objectKey) {
			if(objectKey == null) {
				return null;
			}
			ICollection elements = objectKey as ICollection;
			if(elements == null) {
				elements = new object[] { objectKey };
			}
			if(elements.Count == 0) {
				return null;
			}
			string result = string.Empty;
			foreach(object obj in elements) {
				string subResult = SimpleKeyToString(obj);
				if(result.Length > 0) result += ',';
				result += subResult;
			}
			return result;			
		}
		private static string SimpleKeyToString(object objectKey) {
			if(objectKey == null) {
				throw new ArgumentNullException();
			}
			string typeString;
			string val;
			if(objectKey is Guid) {
				typeString = "Guid";
				val = ((Guid)objectKey).ToString();
			}
			else if(objectKey is IConvertible) {
				typeString = Type.GetTypeCode(objectKey.GetType()).ToString();
				val = Convert.ToString(objectKey, System.Globalization.CultureInfo.InvariantCulture);
			}
			else {
				throw new ArgumentException();
			}
			object q = new DevExpress.Data.Filtering.OperandProperty(typeString);
			object w = new DevExpress.Data.Filtering.OperandValue(val);
			return q.ToString() + w.ToString();
		}		
		public static string MakeTargetObjectHandle(object objectKey) {
			return ConvertKeyToString(objectKey);
		}
		public static CriteriaOperator CreateSelectInstancesCriteria(object objectKey) {
			return new BinaryOperator("TargetObjectHandle", MakeTargetObjectHandle(objectKey));
		}
		public static CriteriaOperator CreateSelectInstancesCriteria(string targetWorkflowUniqueId) {
			return new FunctionOperator(FunctionOperatorType.StartsWith, new OperandProperty("WorkflowUniqueId"), new OperandValue(targetWorkflowUniqueId));
		}
		public static CriteriaOperator CreateSelectInstanceCriteria(object objectKey, string targetWorkflowUniqueId) {
			return new GroupOperator(
				CreateSelectInstancesCriteria(objectKey),
				CreateSelectInstancesCriteria(targetWorkflowUniqueId));
		}
	}	
}
