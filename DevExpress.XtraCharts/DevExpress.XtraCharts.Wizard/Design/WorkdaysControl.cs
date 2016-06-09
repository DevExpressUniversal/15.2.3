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

using System.Windows.Forms;
namespace DevExpress.XtraCharts.Design {
	public partial class WorkdaysControl : UserControl {
		public Weekday Workdays { 
			get {
				Weekday workdays = (Weekday)0;
				if (lbWeekDays.Items[0].CheckState == CheckState.Checked)
					workdays |= Weekday.Sunday;
				if (lbWeekDays.Items[1].CheckState == CheckState.Checked)
					workdays |= Weekday.Monday;
				if (lbWeekDays.Items[2].CheckState == CheckState.Checked)
					workdays |= Weekday.Tuesday;
				if (lbWeekDays.Items[3].CheckState == CheckState.Checked)
					workdays |= Weekday.Wednesday;
				if (lbWeekDays.Items[4].CheckState == CheckState.Checked)
					workdays |= Weekday.Thursday;
				if (lbWeekDays.Items[5].CheckState == CheckState.Checked)
					workdays |= Weekday.Friday;
				if (lbWeekDays.Items[6].CheckState == CheckState.Checked)
					workdays |= Weekday.Saturday;
				return workdays;
			}
		}
		WorkdaysControl() {
			InitializeComponent();
		}
		public WorkdaysControl(Weekday workdays) : this() {
			if ((workdays & Weekday.Sunday) == Weekday.Sunday)
				lbWeekDays.Items[0].CheckState = CheckState.Checked;
			if ((workdays & Weekday.Monday) == Weekday.Monday)
				lbWeekDays.Items[1].CheckState = CheckState.Checked;
			if ((workdays & Weekday.Tuesday) == Weekday.Tuesday)
				lbWeekDays.Items[2].CheckState = CheckState.Checked;
			if ((workdays & Weekday.Wednesday) == Weekday.Wednesday)
				lbWeekDays.Items[3].CheckState = CheckState.Checked;
			if ((workdays & Weekday.Thursday) == Weekday.Thursday)
				lbWeekDays.Items[4].CheckState = CheckState.Checked;
			if ((workdays & Weekday.Friday) == Weekday.Friday)
				lbWeekDays.Items[5].CheckState = CheckState.Checked;
			if ((workdays & Weekday.Saturday) == Weekday.Saturday)
				lbWeekDays.Items[6].CheckState = CheckState.Checked;
		}
	}
}
