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
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
namespace DevExpress.Persistent.BaseImpl {
	[DefaultProperty("Subject")]
	public class Task : BaseObject, ITask {
		private TaskImpl task = new TaskImpl();
		private Party assignedTo;
#if MediumTrust
		[Persistent("DateCompleted"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public DateTime dateCompleted {
			get { return task.DateCompleted; }
			set {
				DateTime oldValue = task.DateCompleted;
				task.DateCompleted = value;
				OnChanged("dateCompleted", oldValue, task.DateCompleted);
			}
		}
#else
		[Persistent("DateCompleted")]
		private DateTime dateCompleted {
			get { return task.DateCompleted; }
			set {
				DateTime oldValue = task.DateCompleted;
				task.DateCompleted = value;
				OnChanged("dateCompleted", oldValue, task.DateCompleted);
			}
		}
#endif
		public Task(Session session) : base(session) { }
		protected override void OnLoading() {
			task.IsLoading = true;
			base.OnLoading();
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			task.DateCompleted = dateCompleted;
			task.IsLoading = false;
		}
		[Action(ImageName = "State_Task_Completed")]
		public void MarkCompleted() {
			TaskStatus oldStatus = task.Status;
			task.MarkCompleted();
			OnChanged("Status", oldStatus, task.Status);
		}
		public string Subject {
			get { return task.Subject; }
			set {
				string oldValue = task.Subject;
				task.Subject = value;
				OnChanged("Subject", oldValue, task.Subject);
			}
		}
		[Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public string Description {
			get { return task.Description; }
			set {
				string oldValue = task.Description;
				task.Description = value;
				OnChanged("Description", oldValue, task.Description);
			}
		}
		public DateTime DueDate {
			get { return task.DueDate; }
			set {
				DateTime oldValue = task.DueDate;
				task.DueDate = value;
				OnChanged("DueDate", oldValue, task.DueDate);
			}
		}
		public DateTime StartDate {
			get { return task.StartDate; }
			set {
				DateTime oldValue = task.StartDate;
				task.StartDate = value;
				OnChanged("StartDate", oldValue, task.StartDate);
			}
		}
		public Party AssignedTo {
			get { return assignedTo; }
			set { SetPropertyValue("AssignedTo", ref assignedTo, value); }
		}
		public TaskStatus Status {
			get { return task.Status; }
			set {
				TaskStatus oldValue = task.Status;
				task.Status = value;
				OnChanged("Status", oldValue, task.Status);
			}
		}
		public Int32 PercentCompleted {
			get { return task.PercentCompleted; }
			set {
				Int32 oldValue = task.PercentCompleted;
				task.PercentCompleted = value;
				OnChanged("PercentCompleted", oldValue, task.PercentCompleted);
			}
		}
		public DateTime DateCompleted {
			get { return dateCompleted; }
		}
	}
}
