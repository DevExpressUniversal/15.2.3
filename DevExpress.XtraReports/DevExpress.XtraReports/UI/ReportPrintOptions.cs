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
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Localization;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.UI {
	[
	TypeConverter(typeof(LocalizableObjectConverter)),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.ReportPrintOptions"),
	]
	public class ReportPrintOptions {
		const int defaultDetailCountAtDesignTime = 0;
		const int defaultDetailCount = 0;
		const int defaultDetailCountOnEmptyDataSource = 1;
		const bool defaultPrintOnEmptyDataSource = true;
		bool printOnEmptyDataSource = true;
		int detailCount = -1;
		int blankDetailCount = 0;
		int detailCountAtDesignTime = -1;
		int detailCountOnEmptyDataSource = 1;
		bool designTimeActivity;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ActivateDesignTimeProperties(bool activity) {
			this.designTimeActivity = activity;
		}
		internal int ActualDetailCount {
			get { return designTimeActivity && detailCountAtDesignTime != -1 ? DetailCountAtDesignTime : DetailCount; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.ReportPrintOptions.DetailCountAtDesignTime"),
		XtraSerializableProperty,
		DefaultValue(defaultDetailCountAtDesignTime),
		]
		public int DetailCountAtDesignTime {
			get { return Math.Max(0, detailCountAtDesignTime); }
			set { detailCountAtDesignTime = Math.Max(-1, value); }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.ReportPrintOptions.BlankDetailCount"),
		XtraSerializableProperty,
		DefaultValue(0),
		]
		public int BlankDetailCount {
			get { return blankDetailCount; }
			set { blankDetailCount = Math.Max(0, value); }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.ReportPrintOptions.DetailCount"),
		XtraSerializableProperty,
		DefaultValue(defaultDetailCount),
		]
		public int DetailCount { 
			get { return Math.Max(0, detailCount); } 
			set { detailCount = Math.Max(-1, value); } 
		}
		internal void EnsureDetailCount(int value) {
			if(detailCount == -1)
				DetailCount = value;
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.ReportPrintOptions.DetailCountOnEmptyDataSource"),
		DefaultValue(defaultDetailCountOnEmptyDataSource),
		XtraSerializableProperty,
		]
		public int DetailCountOnEmptyDataSource { 
			get { return detailCountOnEmptyDataSource; } 
			set { detailCountOnEmptyDataSource = value; } 
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.ReportPrintOptions.PrintOnEmptyDataSource"),
		DefaultValue(defaultPrintOnEmptyDataSource),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All),
		]
		public bool PrintOnEmptyDataSource {
			get { return printOnEmptyDataSource; }
			set { printOnEmptyDataSource = value; }
		}
		internal bool ShouldSerialize() {
			return DetailCountAtDesignTime != defaultDetailCountAtDesignTime ||
				DetailCount != defaultDetailCount ||
				BlankDetailCount != 0 ||
				DetailCountOnEmptyDataSource != defaultDetailCountOnEmptyDataSource ||
				PrintOnEmptyDataSource != defaultPrintOnEmptyDataSource;
		}
	}
}
