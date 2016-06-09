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
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.Web {
	public class ReportToolbarComboBox : ReportToolbarItem {
		ListElementCollection elements;
		public ReportToolbarComboBox() {
		}
		public ReportToolbarComboBox(ReportToolbarItemKind itemKind)
			: this() {
			ItemKind = itemKind;
		}
		#region properties
		[Category("Layout")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Unit), "")]
		public Unit Width {
			get { return GetUnitProperty("Width", Unit.Empty); }
			set { SetUnitProperty("Width", Unit.Empty, value); }
		}
		[DefaultValue((string)null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[MergableProperty(false)]
		[NotifyParentProperty(true)]
		public ListElementCollection Elements {
			get {
				if(elements == null) {
					elements = new ListElementCollection();
				}
				return elements;
			}
		}
		[Editor("DevExpress.XtraReports.Web.Design.ComboBoxItemKindEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		[SRCategory(ReportStringId.CatBehavior)]
		public new ReportToolbarItemKind ItemKind {
			get { return base.ItemKind; }
			set { base.ItemKind = value; }
		}
		#endregion
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { Elements };
		}
		public void FillElements(string[] values) {
			Elements.Assign(ListElementCollection.CreateInstance(values));
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var box = source as ReportToolbarComboBox;
			if(box != null) {
				Elements.Assign(box.Elements);
				Width = box.Width;
			}
		}
	}
}
