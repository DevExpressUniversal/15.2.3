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

using DevExpress.Utils.Menu;
using System.Drawing;
using System;
using DevExpress.XtraVerticalGrid.Rows;
using System.ComponentModel;
using DevExpress.XtraVerticalGrid.Localization;
using DevExpress.XtraVerticalGrid.Events;
using DevExpress.Utils;
namespace DevExpress.XtraVerticalGrid {
	public class VGridMenu : VGridMenuBase {
		public VGridMenu(VGridControl vGrid) : base(vGrid) { }
		new VGridControl VGrid { get { return (VGridControl)base.VGrid; } }
		protected override void ConfigureItems(DXPopupMenu menu) {
			base.ConfigureItems(menu);
			bool beginGroup = ShouldBeginGroup(menu);
			if(CurrentRow.Properties.ShowUnboundExpressionMenu) {
				DXMenuItem expressionEditorItem = new DXMenuItem(
					VGridLocalizer.Active.GetLocalizedString(VGridStringId.MenuRowPropertiesExpressionEditor),
					ShowExpressionEditor,
					VerticalGridMenuImages.RowProperties.Images[0]);
				if(beginGroup) {
					expressionEditorItem.BeginGroup = true;
				}
				menu.Items.Add(expressionEditorItem);
			}
		}
		protected override VGridOptionsMenu CreateOptions() {
			return new VGridOptionsMenu();
		}
		void ShowExpressionEditor(object sender, EventArgs e) {
			VGrid.ShowUnboundExpressionEditor(CurrentRowProperties);
		}
	}
	public class PGridMenu : VGridMenuBase {
		public PGridMenu(PropertyGridControl pGrid) : base(pGrid) { }
		PropertyGridControl PGrid { get { return (PropertyGridControl)VGrid; } }
		protected override void ConfigureItems(DXPopupMenu menu) {
			if(CurrentRowProperties != null && !CurrentRowProperties.Bindable)
				return;
			DXMenuItem resetItem = new DXMenuItem(VGridLocalizer.Active.GetLocalizedString(VGridStringId.MenuReset), ResetPropertyHandler);
			menu.Items.Add(resetItem);
			if(!PGrid.CanResetDefaultValue(CurrentRowProperties))
				resetItem.Enabled = false;
		}
		void ResetPropertyHandler(object sender, EventArgs e) {
			PGrid.ResetDefaultValue(CurrentRow);
			PGrid.InvalidateUpdate();
		}
		protected override VGridOptionsMenu CreateOptions() {
			return new PGridOptionsMenu();
		}
	}
	public abstract class VGridMenuBase {
		VGridOptionsMenu options;
		VGridControlBase vGrid;
		RowProperties properties;
		public VGridMenuBase(VGridControlBase vGrid) {
			this.vGrid = vGrid;
			this.options = CreateOptions();
		}
		public VGridOptionsMenu Options { get { return options; } }
		protected VGridControlBase VGrid { get { return vGrid; } }
		protected BaseRow CurrentRow { get { return CurrentRowProperties.Row; } }
		protected RowProperties CurrentRowProperties { get { return properties; } set { properties = value; } }
		internal protected void ShowMenu(Point point, RowProperties rowProperties) {
			if(!Options.EnableContextMenu)
				return;
			VGrid.CloseEditor();
			CurrentRowProperties = rowProperties;
			DXPopupMenu menu = new DXPopupMenu();
			ConfigureItems(menu);
			PopupMenuShowingEventArgs args = new PopupMenuShowingEventArgs(menu, CurrentRow);
			VGrid.RaiseShowMenu(args);
			if(menu.Items.Count == 0)
				return;
			VGrid.DoShowContextMenu(menu, point);
		}
		protected virtual void ConfigureItems(DXPopupMenu menu) { }
		protected abstract VGridOptionsMenu CreateOptions();
		protected virtual bool ShouldBeginGroup(DXPopupMenu menu) {
			return menu.Items.Count > 0;
		}
	}
	public class VerticalGridMenuImages {
		[ThreadStatic]
		static ImageCollection rowProperties = null;
		public static ImageCollection RowProperties {
			get {
				if(rowProperties == null)
					rowProperties = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraVerticalGrid.Images.RowPropertiesMenu.png", typeof(VerticalGridMenuImages).Assembly, new Size(16, 16));
				return rowProperties;
			}
		}
	}
}
