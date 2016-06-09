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
using System.Windows;
using System.Windows.Input;
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows.Controls;
namespace DevExpress.Xpf.Scheduler.UI {
	public partial class AppointmentInplaceEditor : AppointmentInplaceEditorBase {
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AppointmentInplaceEditor() {
			InitializeComponent();
		}
		public AppointmentInplaceEditor(SchedulerControl control, Appointment apt)
			: base(control, apt) {
			InitializeComponent();
		}
		public override void Activate() {
			base.Activate();
			Dispatcher.BeginInvoke(new Action(() => {
				textEditor.Focus();
				if (IsNewAppointment) {
					textEditor.SelectionStart = textEditor.Text.Length;
					textEditor.SelectionLength = 0;
				}
				else
					textEditor.SelectAll();
			}));
		}
	}
}
