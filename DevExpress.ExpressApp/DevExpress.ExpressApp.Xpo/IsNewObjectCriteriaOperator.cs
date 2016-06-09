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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.Xpo {
	public class IsNewObjectCriteriaOperator : ICustomFunctionOperator {
		public const string OperatorName = "IsNewObject";
		private static readonly IsNewObjectCriteriaOperator instance = new IsNewObjectCriteriaOperator();
		public static void Register() {
			CustomFunctionOperatorHelper.Register(instance);
		}
		#region ICustomFunctionOperator Members
		public object Evaluate(params object[] operands) {
			if(operands == null || operands.Length != 1) {
				throw new ArgumentException();
			}
			object obj = operands[0];
			IObjectSpace objectSpace = XPObjectSpace.FindObjectSpaceByObject(obj);
			return objectSpace != null && objectSpace.IsNewObject(obj);
		}
		public string Name {
			get { return OperatorName; }
		}
		public System.Type ResultType(params System.Type[] operands) {
			return typeof(bool);
		}
		#endregion
	}
}
