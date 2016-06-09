﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.XtraScheduler.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	public class TriFoldPrintStyle : PrintStyleWithResourceOptions {
		protected internal TriFoldPrintStyle(bool registerProperties, bool baseStyle)
			: base(registerProperties, baseStyle) {
		}
		public TriFoldPrintStyle(bool baseStyle)
			: this(true, baseStyle) {
		}
		public TriFoldPrintStyle()
			: this(true) {
		}
		public override SchedulerPrintStyleKind Kind { get { return SchedulerPrintStyleKind.TriFold; } }
		const string bitmapName = "trifold";
		protected internal override SchedulerPrintStyle CreateInstance() {
			return new TriFoldPrintStyle(false, false);
		}
		#region properties and methods for UserInterfaceObject
		protected internal override string DefaultDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_TrifoldPrintStyle); } }
		protected override string GetStyleBitmapName() {
			return bitmapName;
		}
		#endregion
		#region LeftSection
		static readonly object leftSectionProperty = new object();
		[DefaultValue(PrintStyleSectionKind.DailyCalendar)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public PrintStyleSectionKind LeftSection {
			get { return (PrintStyleSectionKind)GetPropertyValue(leftSectionProperty); }
			set { SetPropertyValue(leftSectionProperty, value); }
		}
		#endregion
		#region MiddleSection
		static readonly object middleSectionProperty = new object();
		[DefaultValue(PrintStyleSectionKind.WeeklyCalendar)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public PrintStyleSectionKind MiddleSection {
			get { return (PrintStyleSectionKind)GetPropertyValue(middleSectionProperty); }
			set { SetPropertyValue(middleSectionProperty, value); }
		}
		#endregion
		#region RightSection
		static readonly object rightSectionProperty = new object();
		[DefaultValue(PrintStyleSectionKind.MonthlyCalendar)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public PrintStyleSectionKind RightSection {
			get { return (PrintStyleSectionKind)GetPropertyValue(rightSectionProperty); }
			set { SetPropertyValue(rightSectionProperty, value); }
		}
		#endregion
		protected internal override void RegisterProperties() {
			base.RegisterProperties();
			RegisterProperty(leftSectionProperty, PrintStyleSectionKind.DailyCalendar);
			RegisterProperty(middleSectionProperty, PrintStyleSectionKind.WeeklyCalendar);
			RegisterProperty(rightSectionProperty, PrintStyleSectionKind.MonthlyCalendar);
		}
	}
}
