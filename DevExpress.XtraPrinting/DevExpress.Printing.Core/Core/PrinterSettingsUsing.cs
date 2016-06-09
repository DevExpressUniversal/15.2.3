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
using System.Text;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Design;
namespace DevExpress.XtraPrinting {
	[
	TypeConverter(typeof(LocalizableObjectConverter)),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PrinterSettingsUsing"),
	]
	public class PrinterSettingsUsing {
		bool useMargins;
		bool usePaperKind;
		bool useLandscape;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrinterSettingsUsingUseMargins"),
#endif
	   DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PrinterSettingsUsing.UseMargins"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true),
		DefaultValue(false),
		RefreshProperties(RefreshProperties.All)
		]
		public bool UseMargins {
			get { return useMargins; }
			set { useMargins = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrinterSettingsUsingUsePaperKind"),
#endif
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PrinterSettingsUsing.UsePaperKind"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true),
		DefaultValue(false),
		RefreshProperties(RefreshProperties.All)
		]
		public bool UsePaperKind {
			get { return usePaperKind; }
			set { usePaperKind = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrinterSettingsUsingUseLandscape"),
#endif
		 DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PrinterSettingsUsing.UseLandscape"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true),
		DefaultValue(false),
		RefreshProperties(RefreshProperties.All)
		]
		public bool UseLandscape { get { return useLandscape; } set { useLandscape = value; } }
		[Browsable(false)]
		public bool AllSettingsUsed { get { return UseMargins && UsePaperKind && UseLandscape; } }
		[Browsable(false)]
		public bool AnySettingUsed { get { return UseMargins || UsePaperKind || UseLandscape; } }
		public PrinterSettingsUsing() {
		}
		public PrinterSettingsUsing(bool useMargins, bool usePaperKind, bool useLandscape) {
			this.useMargins = useMargins;
			this.usePaperKind = usePaperKind;
			this.useLandscape = useLandscape;
		}
		public PrinterSettingsUsing(PrinterSettingsUsing source)
			: this(source.useMargins, source.usePaperKind, source.useLandscape) {
		}
		internal bool ShouldSerialize() {
			return useLandscape != false || useMargins != false || usePaperKind != false;
		}
	}
}
