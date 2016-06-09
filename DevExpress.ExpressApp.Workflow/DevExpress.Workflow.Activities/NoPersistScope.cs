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
using System.ComponentModel;
using DevExpress.ExpressApp;
using System.Windows.Markup;
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.Workflow.Activities {
	public abstract class NoPersistScopeBase : NativeActivity {
		private Collection<Variable> variables;
		private Variable<NoPersistHandle> noPersistHandle;
		protected override void CacheMetadata(NativeActivityMetadata metadata) {
			foreach(Variable variable in this.Variables) {
				metadata.AddVariable(variable);
			}
			base.CacheMetadata(metadata);
			metadata.AddImplementationVariable(this.noPersistHandle);
		}
		protected abstract void ExecuteCore(NativeActivityContext context);
		protected override void Execute(NativeActivityContext context) {
			EnterNoPersistScope(context);
			ExecuteCore(context);
		}	
		protected void EnterNoPersistScope(NativeActivityContext context) {
			NoPersistHandle noPersistHandle = this.noPersistHandle.Get(context);
			noPersistHandle.Enter(context);
		}
		public NoPersistScopeBase() {
			this.noPersistHandle = new Variable<NoPersistHandle>();
		}
		[Browsable(false)]
		public Collection<Variable> Variables {
			get {
				if(this.variables == null) {
					this.variables = new Collection<Variable>();
				}
				return this.variables;
			}
		}
	}
	[ContentProperty("Activities")]
	[ToolboxBitmap(typeof(ObjectSpaceTransactionScope), "Images.NoPersistScope.bmp")]
	[ToolboxTabName(ActivitiesAssemblyInfo.DXActivitiesTabName)]
	public class NoPersistScope : NoPersistScopeBase {
		private Collection<Activity> activities;
		private Variable<int> lastIndexHint = new Variable<int>();
		private CompletionCallback onChildComplete;
		public NoPersistScope() {
			this.onChildComplete = new CompletionCallback(this.InternalExecute);
		}
		protected virtual void OnChildActivitiesExecutionCompleted(NativeActivityContext context) {
		}
		protected void ScheduleChildActivities(NativeActivityContext context) {
			if((this.activities != null) && (this.Activities.Count > 0)) {
				Activity activity = this.Activities[0];
				context.ScheduleActivity(activity, this.onChildComplete);
			}
		}
		protected override void ExecuteCore(NativeActivityContext context) {
			ScheduleChildActivities(context);
		}
		protected override void CacheMetadata(NativeActivityMetadata metadata) {
			metadata.SetChildrenCollection(this.Activities);
			metadata.AddImplementationVariable(this.lastIndexHint);
			base.CacheMetadata(metadata);
		}
		private void InternalExecute(NativeActivityContext context, ActivityInstance completedInstance) {
			int index = this.lastIndexHint.Get(context);
			if((index >= this.Activities.Count) || (this.Activities[index] != completedInstance.Activity)) {
				index = this.Activities.IndexOf(completedInstance.Activity);
			}
			int num2 = index + 1;
			if(num2 != this.Activities.Count) {
				Activity activity = this.Activities[num2];
				context.ScheduleActivity(activity, this.onChildComplete);
				this.lastIndexHint.Set(context, num2);
			}
			else {
				OnChildActivitiesExecutionCompleted(context);
			}
		}
		[DependsOn("Variables")]
		public Collection<Activity> Activities {
			get {
				if(this.activities == null) {
					this.activities = new Collection<Activity>();
				}
				return this.activities;
			}
		}
	}
}
