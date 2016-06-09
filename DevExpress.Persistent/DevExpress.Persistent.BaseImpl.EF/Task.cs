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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
namespace DevExpress.Persistent.BaseImpl.EF {
	[DefaultProperty("Subject")]
	public class Task : ITask {
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public Nullable<DateTime> DateCompleted { get; protected set; }
		public String Subject { get; set; }
		[FieldSize(FieldSizeAttribute.Unlimited)]
		public String Description { get; set; }
		public Nullable<DateTime> DueDate { get; set; }
		public Nullable<DateTime> StartDate { get; set; }
		[Browsable(false)]
		public Int32 Status_Int { get; set; }
		public Int32 PercentCompleted { get; set; }
		public virtual Party AssignedTo { get; set; }
		[NotMapped]
		public TaskStatus Status {
			get { return (TaskStatus)Status_Int; }
			set {
				Status_Int = (Int32)value;
				if(value == TaskStatus.Completed) {
					DateCompleted = DateTime.Now;
				}
				else {
					DateCompleted = null;
				}
			}
		}
		DateTime ITask.DateCompleted {
			get {
				if(DateCompleted.HasValue) {
					return DateCompleted.Value;
				}
				else {
					return DateTime.MinValue;
				}
			}
		}
		DateTime ITask.DueDate {
			get {
				if(DueDate.HasValue) {
					return DueDate.Value;
				}
				else {
					return DateTime.MinValue;
				}
			}
			set { DueDate = value; }
		}
		DateTime ITask.StartDate {
			get {
				if(StartDate.HasValue) {
					return StartDate.Value;
				}
				else {
					return DateTime.MinValue;
				}
			}
			set { StartDate = value; }
		}
		[Action(ImageName = "State_Task_Completed")]
		public void MarkCompleted() {
			Status = TaskStatus.Completed;
		}
	}
}
