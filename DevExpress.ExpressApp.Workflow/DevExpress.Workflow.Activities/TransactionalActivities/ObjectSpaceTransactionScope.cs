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
using System.Collections.ObjectModel;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
using System.Data.Common;
using DevExpress.Persistent.Base;
using System.Windows.Markup;
using System.Drawing;
namespace DevExpress.Workflow.Activities {
	[DevExpress.Utils.ToolboxTabName(ActivitiesAssemblyInfo.DXActivitiesTabName)]
	[Category("DevExpress.TransactionalActivities")]
	[ToolboxBitmap(typeof(ObjectSpaceTransactionScope), "Images.ObjectSpaceTransactionScope.bmp")]
	public class ObjectSpaceTransactionScope : NoPersistScope {
		public const string ObjectSpacePropertyName = "LocalObjectSpace";
		Variable<IObjectSpace> objectSpaceImpl;
		Variable<string> connectionStringImpl;
		CreateObjectSpace createObjectSpace;
		protected CommitChanges commitChanges;
		private void OnEvaluateCreateObjectSpace(NativeActivityContext context, ActivityInstance instance, IObjectSpace result) {
			Guard.ArgumentNotNull(result, "result");
			context.Properties.Add(ObjectSpacePropertyName, result);
			ScheduleChildActivities(context);
		}
		protected virtual void OnCommitChangesExecutionCompleted(NativeActivityContext context, ActivityInstance instance) {
		}
		protected override void OnChildActivitiesExecutionCompleted(NativeActivityContext context) {
			if(AutoCommit.Get(context)) {
				context.ScheduleActivity(this.commitChanges, new CompletionCallback(OnCommitChangesExecutionCompleted));
			}
		}
		protected override void CacheMetadata(NativeActivityMetadata metadata) {
			base.CacheMetadata(metadata);
			metadata.AddImplementationVariable(objectSpaceImpl);
			metadata.AddImplementationVariable(connectionStringImpl);
			metadata.AddImplementationChild(createObjectSpace);
			metadata.AddImplementationChild(commitChanges);
		}
		protected override void ExecuteCore(NativeActivityContext context) {
			connectionStringImpl.Set(context, ConnectionString.Get(context));
			context.ScheduleActivity(createObjectSpace, new CompletionCallback<IObjectSpace>(OnEvaluateCreateObjectSpace));
		}
		protected void SetCreateObjectSpaceActivity(CreateObjectSpace createObjectSpace) {
			this.createObjectSpace = createObjectSpace;
			this.createObjectSpace.ConnectionString = new InArgument<string>(connectionStringImpl);
			this.createObjectSpace.Result = new OutArgument<IObjectSpace>(objectSpaceImpl);
		}
		protected void SetCommitChangesActivity(CommitChanges commitChanges) {
			this.commitChanges = commitChanges;
			this.commitChanges.ObjectSpace = new InArgument<IObjectSpace>(objectSpaceImpl);
		}
		public ObjectSpaceTransactionScope()
			: base() {
			objectSpaceImpl = new Variable<IObjectSpace>("objectSpaceImpl");
			connectionStringImpl = new Variable<string>("connectionStringImpl");
			SetCreateObjectSpaceActivity(new CreateObjectSpace());
			AutoCommit = new InArgument<bool>(true);
			SetCommitChangesActivity(new CommitChanges());
		}
		[DefaultValue(null)]
		public InArgument<string> ConnectionString { get; set; }
		[DefaultValue(true)]
		public InArgument<bool> AutoCommit { get; set; }
	}
}
