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
using DevExpress.XtraScheduler.Native;
using System.ComponentModel;
namespace DevExpress.XtraScheduler.Reporting {
	#region TextCustomizingEventHandler
	public delegate void TextCustomizingEventHandler(object sender, TextCustomizingEventArgs e);
	#endregion
	#region TextCustomizingEventArgs
	public class TextCustomizingEventArgs : EventArgs {
		string text;
		string secondText;
		public TextCustomizingEventArgs(string text) {
			this.text = text;
		}
		public string Text { get { return text; } set { text = value; } }
		internal string SecondText { get { return secondText; } set { secondText = value; } }
	}
	#endregion
	#region TimeIntervalTextCustomizingEventArgs
	public class TimeIntervalTextCustomizingEventArgs : TextCustomizingEventArgs {
		TimeInterval interval;
		public TimeIntervalTextCustomizingEventArgs(string text, string secondLineText, TimeInterval interval) : base(text) {
			if (interval == null)
				Exceptions.ThrowArgumentNullException("interval");
			this.interval = interval;
			SecondLineText = secondLineText;
		}
		public TimeInterval Interval { get { return interval; } }
		public string SecondLineText { get { return base.SecondText; } set { base.SecondText = value; } }
	}
	#endregion
	#region TimeIntervalTextCustomizingEventArgs
	public class ResourceTextCustomizingEventArgs : TextCustomizingEventArgs {
		ResourceBaseCollection resources;
		public ResourceTextCustomizingEventArgs(string text, ResourceBaseCollection resources)
			: base(text) {
			if (resources == null)
				Exceptions.ThrowArgumentNullException("resources");
			this.resources = resources;
		}
		public ResourceBaseCollection Resources { get { return resources; } }
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Reporting.Native {
	#region ReportViewControlCancelEventHandler
	public delegate void ReportViewControlCancelEventHandler(object sender, ReportViewControlCancelEventArgs e);
	#endregion
	#region ReportViewControlCancelEventArgs
	public class ReportViewControlCancelEventArgs : CancelEventArgs {
		ReportViewControlBase control;
		public ReportViewControlCancelEventArgs(ReportViewControlBase control) {
			this.control = control;
		}
		public ReportViewControlBase Control { get { return control; } }
	}
	#endregion
	#region ReportControlStateChangedEventHandler
	public delegate void ReportControlStateChangedEventHandler(object sender, ReportControlStateChangedEventArgs e);
	#endregion
	#region ReportControlStateChangedEventArgs
	public class ReportControlStateChangedEventArgs : EventArgs {
		ReportControlChangeType change;
		public ReportControlStateChangedEventArgs(ReportControlChangeType change) {
			this.change = change;
		}
		public ReportControlChangeType Change { get { return change; } }
	}
	#endregion
	#region AfterApplyReportControlChangesEventHandler
	public delegate void AfterApplyReportControlChangesEventHandler(object sender, AfterApplyReportControlChangesEventArgs e);
	#endregion
	#region AfterApplyReportControlChangesEventArgs
	public class AfterApplyReportControlChangesEventArgs : EventArgs {
		ReportControlChangeActions actions;
		public AfterApplyReportControlChangesEventArgs(ReportControlChangeActions actions) {
			this.actions = actions;
		}
		public ReportControlChangeActions Actions { get { return actions; } }
	}
	#endregion
	#region ReportSourceChangedEventHandler
	public delegate void ReportSourceChangedEventHandler(object sender, ReportControlStateChangedEventArgs e);
	#endregion
	#region ReportSourceChangedEventArgs
	public class ReportSourceChangedEventArgs : EventArgs {
		public ReportSourceChangedEventArgs() {
		}
	}
	#endregion
}
