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
using System.Text;
namespace DevExpress.Persistent.Base.General {
	public enum WeekOfMonth {
		None,
		First,
		Second,
		Third,
		Fourth,
		Last
	}
	public enum WeekDays {
		EveryDay = 0x7f,
		Friday = 0x20,
		Monday = 2,
		Saturday = 0x40,
		Sunday = 1,
		Thursday = 0x10,
		Tuesday = 4,
		Wednesday = 8,
		WeekendDays = 0x41,
		WorkDays = 0x3e
	}
	public enum RecurrenceType {
		Daily,
		Weekly,
		Monthly,
		Yearly,
		Minutely,
		Hourly
	}
	public enum RecurrenceRange {
		NoEndDate,
		OccurrenceCount,
		EndByDate
	}
	public interface IRecurrenceInfo {
		bool AllDay { get;set;}
		int DayNumber { get;set;}
		TimeSpan Duration { get;set;}
		DateTime End { get;set;}
		Guid Id { get;}
		int Month { get;set;}
		int OccurrenceCount { get;set;}
		int Periodicity { get;set;}
		RecurrenceRange Range { get;set;}
		DateTime Start { get;set;}
		RecurrenceType Type { get;set;}
		WeekDays WeekDays { get;set;}
		WeekOfMonth WeekOfMonth { get;set;}
	}
}
