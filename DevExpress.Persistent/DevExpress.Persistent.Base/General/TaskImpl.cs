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
namespace DevExpress.Persistent.Base.General {
	public class TaskImpl {
		private bool isLoading;
		private string subject;
		private string description;
		private DateTime dueDate;
		private DateTime startDate;
		private TaskStatus status = TaskStatus.NotStarted;
		private Int32 percentCompleted;
		private DateTime dateCompleted;
		private void CheckDateCompleted() {
			if(Status == TaskStatus.Completed) {
				dateCompleted = DateTime.Now;
			}
			else {
				dateCompleted = DateTime.MinValue;
			}
		}
		public void MarkCompleted() {
			Status = TaskStatus.Completed;
		}
		public string Subject {
			get { return subject; }
			set { subject = value; }
		}
		public string Description {
			get { return description; }
			set { description = value; }
		}
		public DateTime DueDate {
			get { return dueDate; }
			set { dueDate = value; }
		}
		public DateTime StartDate {
			get { return startDate; }
			set { startDate = value; }
		}
		public TaskStatus Status {
			get { return status; }
			set {
				if(status != value) {
					status = value;
					if(IsLoading)
						return;
					switch(status) {
						case TaskStatus.NotStarted:
							percentCompleted = 0;
							break;
						case TaskStatus.Completed:
							percentCompleted = 100;
							break;
						case TaskStatus.InProgress:
							if(percentCompleted == 100)
								percentCompleted = 75;
							break;
						case TaskStatus.WaitingForSomeoneElse:
						case TaskStatus.Deferred:
							if(percentCompleted == 100)
								percentCompleted = 0;
							break;
					}
					CheckDateCompleted();
				}
			}
		}
		public Int32 PercentCompleted {
			get { return percentCompleted; }
			set {
				if(percentCompleted != value) {
					percentCompleted = value;
					if(IsLoading)
						return;
					if(percentCompleted == 100)
						status = TaskStatus.Completed;
					if(percentCompleted == 0)
						status = TaskStatus.NotStarted;
					if((percentCompleted > 0) && (percentCompleted < 100))
						status = TaskStatus.InProgress;
					CheckDateCompleted();
				}
			}
		}
		public DateTime DateCompleted {
			get { return dateCompleted; }
			set {
				if(isLoading) {
					dateCompleted = value;
				}
			}
		}
		public bool IsLoading {
			get { return isLoading; }
			set { isLoading = value; }
		}
	}
}
