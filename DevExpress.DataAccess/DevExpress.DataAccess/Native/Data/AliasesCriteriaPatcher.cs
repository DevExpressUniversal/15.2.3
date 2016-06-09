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
using DevExpress.Data.Filtering;
namespace DevExpress.DataAccess.Native.Data {
	public class AliasesCriteriaPatcher : CriteriaPatcher {
		public static CriteriaOperator Process(CriteriaOperator criteria, string oldAlias, string newAlias) {
			AliasesCriteriaPatcher patcher = new AliasesCriteriaPatcher(oldAlias, newAlias);
			CriteriaOperator patchedCriteria = patcher.Process(criteria);
			return patcher.success ? patchedCriteria : criteria;
		}
		readonly string oldAlias;
		readonly string newAlias;
		bool success;
		AliasesCriteriaPatcher(string oldAlias, string newAlias) {
			this.oldAlias = oldAlias;
			this.newAlias = newAlias;
		}
		public override CriteriaOperator Visit(OperandProperty theOperand) {
			if(theOperand.PropertyName.IndexOf(this.oldAlias, StringComparison.Ordinal) != 0)
				return theOperand;
			this.success = true;
			return this.newAlias != null ? new OperandProperty(string.Concat(this.newAlias, theOperand.PropertyName.Remove(0, this.oldAlias.Length))) : null;
		}
	}
}
