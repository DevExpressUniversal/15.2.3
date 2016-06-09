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

using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon.Native {
	public class NamesCriteriaPatcher : CriteriaPatcher {
		public static string Process(string filterString, Dictionary<string, string> renamingMap) {
			return Process(filterString, renamingMap, true);
		}
		public static CriteriaOperator Process(CriteriaOperator criteria, Dictionary<string, string> renamingMap) {
			return Process(criteria, renamingMap, true);
		}
		public static string Process(string filterString, Dictionary<string, string> renamingMap, bool renameDataItems) {
			if(!string.IsNullOrEmpty(filterString) && renamingMap.Count > 0) {
				CriteriaOperator criteria = Process(CriteriaOperator.Parse(filterString), renamingMap, renameDataItems);
				return ReferenceEquals(criteria, null) ? null : CriteriaOperator.ToString(criteria);
			}
			return filterString;
		}
		public static CriteriaOperator Process(CriteriaOperator criteria, Dictionary<string, string> renamingMap, bool renameDataItems) {
			NamesCriteriaPatcher patcher = new NamesCriteriaPatcher(renamingMap, renameDataItems);
			CriteriaOperator patchedCriteria = patcher.Process(criteria);
			return patcher.success ? patchedCriteria : criteria;
		}
		readonly Dictionary<string, string> renamingMap;
		bool success;
		readonly bool renameDataItems;
		NamesCriteriaPatcher(Dictionary<string, string> renamingMap, bool renameDataItems) {
			this.renamingMap = renamingMap;
			this.renameDataItems = renameDataItems;
		}
		public override CriteriaOperator Visit(OperandValue theOperand) {
			if(renameDataItems)
				return base.Visit(theOperand);
			string newPropertyName;
			OperandParameter parameter = theOperand as OperandParameter;
			if(!ReferenceEquals(parameter, null) && renamingMap.TryGetValue(parameter.ParameterName, out newPropertyName)) {
				success = true;
				if(newPropertyName != null)
					return new OperandParameter(newPropertyName);
				return null;
			}
			return theOperand;
		}
		public override CriteriaOperator Visit(OperandProperty theOperand) {
			if(!renameDataItems)
				return base.Visit(theOperand);
			string newPropertyName;
			if(renamingMap.TryGetValue(theOperand.PropertyName, out newPropertyName)) {
				success = true;
				if(newPropertyName != null)
					return new OperandProperty(newPropertyName);
				return null;
			}
			return theOperand;
		}
	}
}
