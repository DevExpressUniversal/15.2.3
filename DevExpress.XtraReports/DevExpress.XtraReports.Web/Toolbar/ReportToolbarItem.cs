#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using DevExpress.Web;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.Web {
	[DefaultProperty("ItemKind")]
	public class ReportToolbarItem : CollectionItem {
		const string
			ItemKindName = "ItemKind",
			NameName = "Name";
		const ReportToolbarItemKind DefaultItemKind = ReportToolbarItemKind.Custom;
		public ReportToolbarItem() {
		}
		public ReportToolbarItem(ReportToolbarItemKind itemKind) {
			ItemKind = itemKind;
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarItemItemKind")]
#endif
		[DefaultValue(ReportToolbarItemKind.Custom)]
		[SRCategory(ReportStringId.CatBehavior)]
		[NotifyParentProperty(true)]
		public ReportToolbarItemKind ItemKind {
			get { return (ReportToolbarItemKind)GetEnumProperty(ItemKindName, DefaultItemKind); }
			set { SetEnumProperty(ItemKindName, DefaultItemKind, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarItemName")]
#endif
		[NotifyParentProperty(true)]
		[Localizable(true)]
		[TypeConverter("DevExpress.XtraReports.Web.Design.ToolbarItemNameConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public virtual string Name {
			get {
				if(ItemKind == ReportToolbarItemKind.Custom)
					return GetStringProperty(NameName, ItemKind.ToString());
				return ItemKind.ToString();
			}
			set {
				SetStringProperty(NameName, "", value);
			}
		}
		bool ShouldSerializeName() {
			return Name != ItemKind.ToString();
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var item = source as ReportToolbarItem;
			if(item != null) {
				Name = item.Name;
				ItemKind = item.ItemKind;
			}
		}
		public override string ToString() {
			return ItemKind != ReportToolbarItemKind.Custom
				? ItemKind.ToString()
				: GetType().Name;
		}
		protected internal virtual void ValidateProperties() {
		}
	}
}
