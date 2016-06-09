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

using DevExpress.Utils.Serializing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	public abstract class PrintStyleWithDateRange : SchedulerPrintStyle {
		protected internal PrintStyleWithDateRange(bool registerProperties, bool baseStyle)
			: base(registerProperties, baseStyle) {
		}
		#region UseDefaultRange
		static readonly object useDefaultRangeProperty = new object();
		[DefaultValue(true)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue, 2)]
		public bool UseDefaultRange {
			get { return (bool)GetPropertyValue(useDefaultRangeProperty); }
			set { SetPropertyValue(useDefaultRangeProperty, value); }
		}
		#endregion
		#region StartRangeDate
		static readonly object startRangeDateProperty = new object();
		[XtraSerializableProperty(XtraSerializationVisibility.Visible, XtraSerializationFlags.DefaultValue, 0)]
		public DateTime StartRangeDate {
			get { return (DateTime)GetPropertyValue(startRangeDateProperty); }
			set {
				UseDefaultRange = false;
				SetPropertyValue(startRangeDateProperty, value);
			}
		}
		internal bool ShouldSerializeStartRangeDate() {
			return (!UseDefaultRange) && StartRangeDate != DateTime.MinValue;
		}
		internal bool XtraShouldSerializeStartRangeDate() {
			return ShouldSerializeStartRangeDate();
		}
		internal void ResetStartRangeDate() {
			SetPropertyValue(startRangeDateProperty, DateTime.MinValue);
		}
		#endregion
		#region EndRangeDate
		static readonly object endRangeDateProperty = new object();
		[XtraSerializableProperty(XtraSerializationVisibility.Visible, XtraSerializationFlags.DefaultValue, 1)]
		public DateTime EndRangeDate {
			get { return (DateTime)GetPropertyValue(endRangeDateProperty); }
			set {
				UseDefaultRange = false;
				SetPropertyValue(endRangeDateProperty, value);
			}
		}
		internal bool ShouldSerializeEndRangeDate() {
			return (!UseDefaultRange) && EndRangeDate != DateTime.MinValue;
		}
		internal bool XtraShouldSerializeEndRangeDate() {
			return (!UseDefaultRange) && EndRangeDate != DateTime.MinValue;
		}
		internal void ResetEndRangeDate() {
			SetPropertyValue(endRangeDateProperty, DateTime.MinValue);
		}
		#endregion
		protected internal override void RegisterProperties() {
			base.RegisterProperties();
			RegisterProperty(useDefaultRangeProperty, true);
			RegisterProperty(startRangeDateProperty, DateTime.MinValue);
			RegisterProperty(endRangeDateProperty, DateTime.MinValue);
		}
		public void SetDefaultRange(DateTime start, DateTime end) {
			if (UseDefaultRange) {
				StartRangeDate = start;
				EndRangeDate = end;
				UseDefaultRange = true;
			}
		}
	}
}
