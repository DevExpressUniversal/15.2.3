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

using System.ComponentModel;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using System.Resources;
namespace DevExpress.XtraReports.Wizards3.Localization {
	[ToolboxItem(false)]
	public partial class ReportDesignerLocalizer : XtraLocalizer<ReportBoxDesignerStringId> {
		public static readonly ReportDesignerLocalizer Default = new ReportDesignerLocalizer();
		public static new XtraLocalizer<ReportBoxDesignerStringId> Active {
			get { return XtraLocalizer<ReportBoxDesignerStringId>.Active; }
			set { XtraLocalizer<ReportBoxDesignerStringId>.Active = value; }
		}
		static ReportDesignerLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<ReportBoxDesignerStringId>(CreateDefaultLocalizer()));
		}
		public static XtraLocalizer<ReportBoxDesignerStringId> CreateDefaultLocalizer() {
			return new ReportDesignerResLocalizer();
		}
		public static string GetString(ReportBoxDesignerStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<ReportBoxDesignerStringId> CreateResXLocalizer() {
			return new ReportDesignerResLocalizer();
		}
		protected override void PopulateStringTable() {
			AddStrings();
		}
	}
	[ToolboxItem(false)]
	public class ReportDesignerResLocalizer : XtraResXLocalizer<ReportBoxDesignerStringId> {
		public ReportDesignerResLocalizer()
			: base(new ReportDesignerLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.XtraReports.Wizards3.Localization.LocalizationRes", typeof(ReportDesignerResLocalizer).Assembly);
		}
	}
}
