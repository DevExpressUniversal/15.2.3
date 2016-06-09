#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.Utils.Localization;
using DevExpress.Web.Localization;
namespace DevExpress.DashboardWeb.Localization {
	public enum DashboardWebStringId {
		IncompatibleBrowser
	}
	public class DashboardWebResLocalizer : ASPxResLocalizerBase<DashboardWebStringId> {
		protected override string GlobalResourceAssemblyName { get { return AssemblyInfo.SRAssemblyDashboardWeb; } }
		protected override string ResxName { get { return "DevExpress.DashboardWeb.LocalizationRes"; } }
		public DashboardWebResLocalizer(DashboardWebLocalizer localizer)
			: base(localizer) {
		}
		public DashboardWebResLocalizer()
			: this(new DashboardWebLocalizer()) {
		}
	}
	public class DashboardWebLocalizer : XtraLocalizer<DashboardWebStringId> {
		static DashboardWebLocalizer() {
			ASPxActiveLocalizerProvider<DashboardWebStringId> provider = new ASPxActiveLocalizerProvider<DashboardWebStringId>(CreateResLocalizerInstance);
			SetActiveLocalizerProvider(provider);
		}
		static XtraLocalizer<DashboardWebStringId> CreateResLocalizerInstance() {
			return new DashboardWebResLocalizer();
		}
		public static string GetString(DashboardWebStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<DashboardWebStringId> CreateResXLocalizer() {
			return CreateResLocalizerInstance();
		}
		protected override void PopulateStringTable() {
			AddString(DashboardWebStringId.IncompatibleBrowser, "Dashboard Viewer: The current browser version is not supported.<br/><br/>Currently, the following browsers are supported: Internet Explorer 8, Chrome, Safari, FireFox, Opera, Android 3+.");
		}
	}
}
