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
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.Workflow.Utils;
namespace DevExpress.Workflow.Activities {
	public abstract class TransactionalObjectSpaceActivityBase : NativeActivity {
		private TransactionalGetObjectSpace getObjectSpace;
		private Variable<IObjectSpace> transactionObjectSpace;
		private void OnGetObjectSpaceCompleted(NativeActivityContext context, ActivityInstance instance, IObjectSpace result) {
			Guard.ArgumentNotNull(result, "result");
			transactionObjectSpace.Set(context, result);
			if(this.InnerActivity != null) {
				context.ScheduleActivity(InnerActivity);
			}
		}
		protected bool useReflectionMetadata = true;
		protected override void CacheMetadata(NativeActivityMetadata metadata) {
			if(useReflectionMetadata) {
				base.CacheMetadata(metadata);
			}
			metadata.AddImplementationChild(getObjectSpace);
			metadata.AddImplementationChild(InnerActivity);
			metadata.AddImplementationVariable(TransactionObjectSpace);
		}
		protected override void Execute(NativeActivityContext context) {
			context.ScheduleActivity(getObjectSpace, new CompletionCallback<IObjectSpace>(OnGetObjectSpaceCompleted));
		}
		public TransactionalObjectSpaceActivityBase() {
#if !DebugTest
			this.Constraints.Add(ConstraintHelper.VerifyParent<ObjectSpaceTransactionScope>(this));
#endif
			transactionObjectSpace = new Variable<IObjectSpace>("transactionObjectSpace");
			getObjectSpace = new TransactionalGetObjectSpace {
				Result = new OutArgument<IObjectSpace>(transactionObjectSpace)
			};
		}
		protected Variable<IObjectSpace> TransactionObjectSpace {
			get {
				return transactionObjectSpace;
			}
		}
		protected Activity InnerActivity { get; set; }
	}
	public abstract class TransactionalObjectSpaceActivityBase<T>  : NativeActivity<T> {
		private TransactionalGetObjectSpace getObjectSpace;
		private Variable<IObjectSpace> transactionObjectSpace;
		private void OnGetObjectSpaceCompleted(NativeActivityContext context, ActivityInstance instance, IObjectSpace result) {
			Guard.ArgumentNotNull(result, "result");
			transactionObjectSpace.Set(context, result);
			if(this.InnerActivity != null) {
				context.ScheduleActivity(InnerActivity, new CompletionCallback(OnInnerActivityCompleted));
			}
		}
		protected virtual void OnInnerActivityCompleted(NativeActivityContext context, ActivityInstance instance) { }
		protected bool useReflectionMetadata = true;
		protected override void CacheMetadata(NativeActivityMetadata metadata) {
			if(useReflectionMetadata) {
				base.CacheMetadata(metadata);
			}
			metadata.AddImplementationChild(getObjectSpace);
			metadata.AddImplementationChild(InnerActivity);
			metadata.AddImplementationVariable(TransactionObjectSpace);
		}
		protected override void Execute(NativeActivityContext context) {
			context.ScheduleActivity(getObjectSpace, new CompletionCallback<IObjectSpace>(OnGetObjectSpaceCompleted));
		}
		public TransactionalObjectSpaceActivityBase() {
#if !DebugTest
			this.Constraints.Add(ConstraintHelper.VerifyParent<ObjectSpaceTransactionScope>(this));
#endif
			transactionObjectSpace = new Variable<IObjectSpace>("transactionObjectSpace");
			getObjectSpace = new TransactionalGetObjectSpace {
				Result = new OutArgument<IObjectSpace>(transactionObjectSpace)
			};
		}
		protected Variable<IObjectSpace> TransactionObjectSpace {
			get {
				return transactionObjectSpace;
			}
		}
		protected Activity InnerActivity { get; set; }
#if DebugTest
		public void SetGetObjectSpaceActivity(TransactionalGetObjectSpace getObjectSpaceActivity) {
			this.getObjectSpace = getObjectSpaceActivity;
		}
#endif
	}
}
