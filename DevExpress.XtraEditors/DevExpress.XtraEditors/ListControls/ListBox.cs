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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ListControls;
using DevExpress.XtraEditors.ToolboxIcons;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraEditors {
	[DXToolboxItem(DXToolboxItemKind.Free), DefaultProperty("Items"),
	 Description("Displays a list of items."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon)
	]
	[DevExpress.Utils.Design.DataAccess.DataAccessMetadata("All", SupportedProcessingModes = "Simple", EnableDirectBinding = false)]
	[ListBoxControlCustomBindingProperties]
	[ToolboxBitmap(typeof(ToolboxIconsRootNS), "ListBoxControl")]
	public class ListBoxControl : BaseListBoxControl {
		public ListBoxControl()
			: base() {
			Items.ListChanged += new ListChangedEventHandler(ItemCollectionChanged);
		}
		protected virtual void ItemCollectionChanged(object sender, ListChangedEventArgs e) {
			OnListChanged(this, e);
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new BaseListBoxViewInfo(this);
		}
		protected override BaseControlPainter CreatePainter() {
			return new BaseListBoxPainter();
		}
		protected override ListBoxItemCollection CreateItemsCollection() {
			return new ListBoxItemCollection();
		}
		protected override void DoSort() { new ListBoxItemSorter(Items, this).DoSort(); }
		[Localizable(true), DXCategory(CategoryName.Data), Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor)), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ListBoxControlItems"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ListBoxItemCollection Items { get { return ItemsCore; } }
		protected class ListBoxControlCustomBindingPropertiesAttribute : ListControlCustomBindingPropertiesAttribute {
			public ListBoxControlCustomBindingPropertiesAttribute()
				: base("ListBoxControl") {
			}
		}
	}
}
