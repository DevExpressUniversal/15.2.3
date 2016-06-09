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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	public class PreliminaryHeaderPrintInfo {
		int maxMonthCalendarCount;
		int monthCalendarCount;
		Size singleMonthSize;
		double scale;
		double textWidth;
		double textHeight;
		int headerHeight;
		DateNavigator dateNavigator;
		Rectangle availableTextBounds;
		SizeF firstLineSize;
		SizeF secondLineSize;
		string firstLineText;
		string secondLineText;
		public int MaxMonthCalendarCount { get { return maxMonthCalendarCount; } set { maxMonthCalendarCount = value; } }
		public int MonthCalendarCount { get { return monthCalendarCount; } set { monthCalendarCount = value; } }
		public Size SingleMonthSize { get { return singleMonthSize; } set { singleMonthSize = value; } }
		public double Scale { get { return scale; } set { scale = value; } }
		public double TextWidth { get { return textWidth; } set { textWidth = value; } }
		public double TextHeight { get { return textHeight; } set { textHeight = value; } }
		public int HeaderHeight { get { return headerHeight; } set { headerHeight = value; } }
		public DateNavigator DateNavigator { get { return dateNavigator; } set { dateNavigator = value; } }
		public Rectangle AvailableTextBounds { get { return availableTextBounds; } set { availableTextBounds = value; } }
		public string FirstLineText { get { return firstLineText; } set { firstLineText = value; } }
		public string SecondLineText { get { return secondLineText; } set { secondLineText = value; } }
		public SizeF FirstLineSize { get { return firstLineSize; } set { firstLineSize = value; } }
		public SizeF SecondLineSize { get { return secondLineSize; } set { secondLineSize = value; } }
	}
}
