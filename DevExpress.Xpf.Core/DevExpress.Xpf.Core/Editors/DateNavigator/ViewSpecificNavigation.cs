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
using System.Linq;
using System.Text;
using DevExpress.Xpf.Editors.DateNavigator;
namespace DevExpress.Xpf.Editors.DateNavigator {
	public abstract class ViewSpecificNavigationLogic {
		public virtual DateTime NextPage(DateTime date, int calendarCount) {
			return GetPage(date, calendarCount);
		}
		public virtual DateTime PreviousPage(DateTime date, int calendarCount) {
			return GetPage(date, -calendarCount);
		}
		public abstract DateTime GetPage(DateTime dt, int offset);
		public abstract DateTime MoveLeft(DateTime dt);
		public abstract DateTime MoveDown(DateTime dt);
		public abstract DateTime MoveUp(DateTime dt);
		public abstract DateTime MoveRight(DateTime dt);
		protected virtual DateTime Move(DateTime date, Func<DateTime, DateTime> changeHandler) {
			try {
				return changeHandler(date);
			}
			catch (ArgumentOutOfRangeException) {
				return date;
			}
		}
	}
	public class MonthNavigationLogic : ViewSpecificNavigationLogic {
		public override DateTime MoveLeft(DateTime date) {
			return Move(date, (dt) => dt.AddDays(-1));
		}
		public override DateTime MoveRight(DateTime date) {
			return Move(date, (dt) => dt.AddDays(1));
		}
		public override DateTime MoveUp(DateTime date) {
			return Move(date, (dt) => dt.AddDays(-7));
		}
		public override DateTime MoveDown(DateTime date) {
			return Move(date, (dt) => dt.AddDays(7));
		}
		public override DateTime NextPage(DateTime date, int calendarCount) {
			return GetPage(date, 1);
		}
		public override DateTime PreviousPage(DateTime date, int calendarCount) {
			return GetPage(date, -1);
		}
		public override DateTime GetPage(DateTime date, int offset) {
			return Move(date, (dt) => dt.AddMonths(offset));
		}
	}
	public class YearNavigationLogic : ViewSpecificNavigationLogic {
		public override DateTime MoveLeft(DateTime date) {
			return Move(date, (dt => dt.AddMonths(-1)));
		}
		public override DateTime MoveRight(DateTime date) {
			return Move(date, (dt => dt.AddMonths(1)));
		}
		public override DateTime MoveDown(DateTime date) {
			return Move(date, (dt => dt.AddMonths(4)));
		}
		public override DateTime MoveUp(DateTime date) {
			return Move(date, (dt => dt.AddMonths(-4)));
		}
		public override DateTime GetPage(DateTime date, int offset) {
			return Move(date, (dt => dt.AddMonths(offset * 12)));
		}
	}
	public class YearsNavigationLogic : ViewSpecificNavigationLogic {
		public override DateTime MoveLeft(DateTime date) {
			return Move(date, (dt => dt.AddYears(-1)));
		}
		public override DateTime MoveRight(DateTime date) {
			return Move(date, (dt => dt.AddYears(1)));
		}
		public override DateTime MoveDown(DateTime date) {
			return Move(date, (dt => dt.AddYears(4)));
		}
		public override DateTime MoveUp(DateTime date) {
			return Move(date, (dt => dt.AddYears(-4)));
		}
		public override DateTime GetPage(DateTime date, int offset) {
			return Move(date, (dt => dt.AddYears(offset * 10)));
		}
	}
	public class YearsGroupNavigationLogic : ViewSpecificNavigationLogic {
		public override DateTime MoveLeft(DateTime date) {
			return Move(date, (dt => dt.AddYears(-10)));
		}
		public override DateTime MoveRight(DateTime date) {
			return Move(date, (dt => dt.AddYears(10)));
		}
		public override DateTime MoveDown(DateTime date) {
			return Move(date, (dt => dt.AddYears(40)));
		}
		public override DateTime MoveUp(DateTime date) {
			return Move(date, (dt => dt.AddYears(-40)));
		}
		public override DateTime GetPage(DateTime date, int offset) {
			return Move(date, (dt => dt.AddYears(offset * 100)));
		}
	}
}
