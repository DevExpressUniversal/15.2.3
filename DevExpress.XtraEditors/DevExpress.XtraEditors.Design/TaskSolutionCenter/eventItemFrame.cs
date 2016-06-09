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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
namespace DevExpress.XtraEditors.Design.TasksSolution {
	public class EventItemFrame : LabelInfo, IStepItemFrame {
		TaskEvent taskEvent;
		object sourceObject;
		public EventItemFrame(TaskEvent taskEvent, object sourceObject, ITaskRunner taskRunner)	{
			this.taskEvent = taskEvent;
			this.sourceObject = sourceObject;
			Parent = taskRunner.Control;
			Dock = DockStyle.Fill;
			BringToFront();
			GenerateText();
		}
		protected TaskEvent TaskEvent { get { return taskEvent; } }
		protected object SourceObject { get { return sourceObject; } }
		bool IStepItemFrame.CanMoveNext { get { return true; } }
		void GenerateText() {
			Texts.Add("The Task Solution Center will generate the following event handlers:");
			GenerateText(TaskEvent);
		}
		void GenerateText(TaskEvent taskEvent) {
			if(taskEvent.Count > 0) {
				for(int i = 0; i < taskEvent.Count; i ++)
					GenerateText(taskEvent[i]);
			}
			Texts.Add(System.Environment.NewLine + System.Environment.NewLine + taskEvent.EventName, Color.Blue, false);
			if(GetEventDescription(taskEvent) != string.Empty) {
				Texts.Add(" - " + GetEventDescription(taskEvent));
			}
		}
		string GetEventDescription(TaskEvent taskEvent) {
			if(taskEvent.Description != string.Empty)
				return taskEvent.Description;
			EventDescriptor eventDescriptor = TypeDescriptor.GetEvents(SourceObject).Find(taskEvent.EventName, true);
			return eventDescriptor != null ? eventDescriptor.Description : string.Empty;
		}
	}
}
