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
namespace DevExpress.Xpf.Editors {
	public class DateTimePickerData {
		public string Text { get; set; }
		public DateTime Value { get; set; }
		public DateTimePart DateTimePart { get; set; }
		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((DateTimePickerData)obj);
		}
		protected bool Equals(DateTimePickerData other) {
			switch (DateTimePart) {
				case DateTimePart.Day:
					return Value.Day.Equals(other.Value.Day) && Value.Month.Equals(other.Value.Month) && Value.Year.Equals(other.Value.Year);
				case DateTimePart.Hour12:
					return (Value.Hour % 12).Equals(other.Value.Hour % 12);
				case DateTimePart.AmPm:
					return (Value.Hour > 11 && other.Value.Hour > 11) || (Value.Hour <= 11 && other.Value.Hour <= 11);
				case DateTimePart.Hour24:
					return Value.Hour.Equals(other.Value.Hour);
				case DateTimePart.Millisecond:
					return Value.Hour.Equals(other.Value.Millisecond);
				case DateTimePart.Minute:
					return Value.Minute.Equals(other.Value.Minute);
				case DateTimePart.Month:
					return Value.Month.Equals(other.Value.Month);
				case DateTimePart.Period:
				case DateTimePart.PeriodOfEra:
					return false;
				case DateTimePart.Second:
					return Value.Second.Equals(other.Value.Second);
				case DateTimePart.Year:
					return Value.Year.Equals(other.Value.Year);
			}
			return Value.Equals(other.Value);
		}
		public override int GetHashCode() {
			return Value.GetHashCode();
		}
	}
}
