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
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Resources;
using System.Globalization;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraReports.Localization 
{
	public static class ReportStringIdExtensions {
		public static string GetString(this ReportStringId id) {
			return ReportLocalizer.GetString(id);
		}
		public static string GetString(this ReportStringId id, params object[] args) {
			return string.Format(ReportLocalizer.GetString(id), args);
		}
	}
	public static class XRDesignBarManagerBarNames {
		public const string
			MainMenu = "Main Menu",
			Toolbar = "Toolbar",
			FormattingToolbar = "Formatting Toolbar",
			LayoutToolbar = "Layout Toolbar",
			StatusBar = "Status Bar",
			ZoomBar = "Zoom Toolbar",
			Toolbox = "Toolbox";
	}
	[ToolboxItem(false)]
	public partial class ReportLocalizer : XtraLocalizer<ReportStringId> {
		public static readonly ReportLocalizer Default = new ReportLocalizer();
		public static new XtraLocalizer<ReportStringId> Active {
			get { return XtraLocalizer<ReportStringId>.Active; }
			set { XtraLocalizer<ReportStringId>.Active = value; }
		}
		static ReportLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<ReportStringId>(CreateDefaultLocalizer()));
		}
		public static XtraLocalizer<ReportStringId> CreateDefaultLocalizer() {
			return new ReportResLocalizer();
		}
		public static string GetString(ReportStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<ReportStringId> CreateResXLocalizer() {
			return new ReportResLocalizer();
		}
		protected override void PopulateStringTable() {
			AddStrings();
		}
	}
	[ToolboxItem(false)]
	public class ReportResLocalizer : XtraResXLocalizer<ReportStringId> {
		public ReportResLocalizer()
			: base(new ReportLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.XtraReports.LocalizationRes", typeof(ReportResLocalizer).Assembly);
		}
	}
}
