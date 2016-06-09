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
using DevExpress.Xpo;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using System.Activities.Statements;
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.Workflow.Activities {
	[System.ComponentModel.ToolboxItem(false)]
	[ToolboxBitmap(typeof(ObjectSpaceTransactionScope), "Images.TransactionalGetObjectByKey.bmp")]
	public class TransactionalGetObjectByKey<T> : TransactionalObjectSpaceActivityBase<T> {
		public TransactionalGetObjectByKey() {
			InnerActivity = new Assign<T> {
				To = new OutArgument<T>(env => Result.Get(env)),
				Value = new InArgument<T>(env => (T)TransactionObjectSpace.Get(env).GetObjectByKey<T>(Key.Get(env)))
			};
		}
		[RequiredArgument]
		public InArgument<object> Key { get; set; }
	}
	[System.ComponentModel.ToolboxItem(false)]
	[ToolboxBitmap(typeof(ObjectSpaceTransactionScope), "Images.TransactionalFindObjectByCriteria.bmp")]
	public class TransactionalFindObjectByCriteria<T> : TransactionalObjectSpaceActivityBase<T> {
		public TransactionalFindObjectByCriteria() {
			InnerActivity = new Assign<T> {
				To = new OutArgument<T>(env => Result.Get(env)),
				Value = new InArgument<T>(env => (T)TransactionObjectSpace.Get(env).FindObject<T>(CriteriaOperator.Parse(Criteria.Get(env)), true))
			};
		}
		public InArgument<string> Criteria { get; set; }
	}
	[System.ComponentModel.ToolboxItem(false)]
	[ToolboxBitmap(typeof(ObjectSpaceTransactionScope), "Images.TransactionalFindObjectByCriteria.bmp")]
	public class TransactionalGetObjectsByCriteria<T> : TransactionalObjectSpaceActivityBase<IList<T>> {
		public TransactionalGetObjectsByCriteria() {
			InnerActivity = new Assign<IList<T>>
			{
				To = new OutArgument<IList<T>>(env => Result.Get(env)),
				Value = new InArgument<IList<T>>(env => TransactionObjectSpace.Get(env).GetObjects<T>(CriteriaOperator.Parse(Criteria.Get(env))))
			};
		}
		public InArgument<string> Criteria { get; set; }
	}
	[Obsolete("Use 'TransactionalGetObjectsByCriteria<T>' instead.")]
	[System.ComponentModel.ToolboxItem(false)]
	public class TransactionalFindObjectsByCriteria<T> : TransactionalGetObjectsByCriteria<T> {
		public TransactionalFindObjectsByCriteria() : base() {
		}
	}
}
