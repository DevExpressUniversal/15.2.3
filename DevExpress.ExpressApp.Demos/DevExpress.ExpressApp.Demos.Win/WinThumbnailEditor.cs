#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Controls;
using System.Collections;
using DevExpress.ExpressApp;
using DevExpress.Utils.Menu;
using DevExpress.ExpressApp.Templates;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.ExpressApp.Model;
using System.Collections.Generic;
namespace DevExpress.ExpressApp.Demos.Win.PropertyEditors {
	public class WinThumbnailEditor : ListEditor, IDXPopupMenuHolder {
		public static string Alias = "Thumbnail";
		private ActionsDXPopupMenu popupMenu;
		private GridControl control;
		private LayoutView layoutView;
		private Object controlDataSource;
		private void dataSource_ListChanged(object sender, ListChangedEventArgs e) {
			Refresh();
		}
		private void control_MouseDoubleClick(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				OnProcessSelectedItem();
			}
		}
		private void control_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter) {
				OnProcessSelectedItem();
			}
		}
		private void layoutView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e) {
			OnSelectionChanged();
			OnFocusedObjectChanged();
		}
		protected override object CreateControlsCore() {
			LayoutViewColumn imageColumn;
			RepositoryItemTextEdit repositoryItemImageName;
			LayoutViewField layoutViewField_Image;
			LayoutViewColumn imageNameColumn;
			RepositoryItemPictureEdit repositoryItemImage;
			LayoutViewField layoutViewField_ImageName;
			LayoutViewCard layoutViewTemplateCard;
			control = new GridControl();
			layoutView = new LayoutView();
			imageColumn = new LayoutViewColumn();
			repositoryItemImage = new RepositoryItemPictureEdit();
			layoutViewField_Image = new LayoutViewField();
			imageNameColumn = new LayoutViewColumn();
			repositoryItemImageName = new RepositoryItemTextEdit();
			layoutViewField_ImageName = new LayoutViewField();
			layoutViewTemplateCard = new LayoutViewCard();
			control.Cursor = Cursors.Default;
			control.MainView = layoutView;
			control.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			repositoryItemImage,
			repositoryItemImageName});
			control.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {layoutView});
			layoutView.Appearance.FieldValue.Options.UseTextOptions = true;
			layoutView.Appearance.FieldValue.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			layoutView.Appearance.FieldValue.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			layoutView.CardMinSize = new System.Drawing.Size(50, 70);
			layoutView.DetailHeight = 53;
			layoutView.Columns.AddRange(new DevExpress.XtraGrid.Columns.LayoutViewColumn[] {
			imageColumn,
			imageNameColumn});
			layoutView.GridControl = control;
			layoutView.Name = "layoutView";
			layoutView.OptionsBehavior.AllowExpandCollapse = false;
			layoutView.OptionsBehavior.AllowPanCards = false;
			layoutView.OptionsBehavior.Editable = false;
			layoutView.OptionsCustomization.AllowFilter = false;
			layoutView.OptionsCustomization.AllowSort = false;
			layoutView.OptionsHeaderPanel.EnableCarouselModeButton = false;
			layoutView.OptionsHeaderPanel.EnableColumnModeButton = false;
			layoutView.OptionsHeaderPanel.EnableMultiColumnModeButton = false;
			layoutView.OptionsHeaderPanel.EnableMultiRowModeButton = false;
			layoutView.OptionsHeaderPanel.EnablePanButton = false;
			layoutView.OptionsHeaderPanel.EnableRowModeButton = false;
			layoutView.OptionsHeaderPanel.EnableSingleModeButton = false;
			layoutView.OptionsView.AllowHotTrackFields = false;
			layoutView.OptionsView.CardsAlignment = DevExpress.XtraGrid.Views.Layout.CardsAlignment.Near;
			layoutView.OptionsView.ShowCardBorderIfCaptionHidden = false;
			layoutView.OptionsView.ShowCardCaption = false;
			layoutView.OptionsView.ShowCardLines = false;
			layoutView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
			layoutView.OptionsView.ShowHeaderPanel = false;
			layoutView.OptionsView.ViewMode = DevExpress.XtraGrid.Views.Layout.LayoutViewMode.MultiColumn;
			layoutView.TemplateCard = layoutViewTemplateCard;
			imageColumn.Caption = "OriginalImage";
			imageColumn.ColumnEdit = repositoryItemImage;
			imageColumn.CustomizationCaption = "OriginalImage";
			imageColumn.FieldName = "OriginalImage";
			imageColumn.LayoutViewField = layoutViewField_Image;
			imageColumn.Name = "OriginalImage";
			repositoryItemImage.Name = "repositoryItemImage";
			repositoryItemImage.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
			repositoryItemImage.PictureAlignment = ContentAlignment.MiddleCenter;
			layoutViewField_Image.EditorPreferredWidth = 32;
			layoutViewField_Image.Location = new System.Drawing.Point(0, 0);
			layoutViewField_Image.Name = "layoutViewField_Image";
			layoutViewField_Image.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			layoutViewField_Image.Size = new System.Drawing.Size(32, 32);
			layoutViewField_Image.TextLocation = DevExpress.Utils.Locations.Bottom;
			layoutViewField_Image.TextSize = new System.Drawing.Size(0, 0);
			layoutViewField_Image.TextToControlDistance = 0;
			layoutViewField_Image.TextVisible = false;
			layoutViewField_Image.MinSize = layoutViewField_Image.MaxSize = new Size(32, 32);
			layoutViewField_Image.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			imageNameColumn.Caption = "ImageName";
			imageNameColumn.ColumnEdit = repositoryItemImageName;
			imageNameColumn.CustomizationCaption = "ImageName";
			imageNameColumn.FieldName = "ImageName";
			imageNameColumn.LayoutViewField = layoutViewField_ImageName;
			imageNameColumn.Name = "ImageName";
			imageNameColumn.SortIndex = 0;
			imageNameColumn.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
			repositoryItemImageName.AutoHeight = false;
			repositoryItemImageName.Name = "repositoryItemImageName";
			layoutViewField_ImageName.EditorPreferredWidth = 32;
			layoutViewField_ImageName.Location = new System.Drawing.Point(0, 32);
			layoutViewField_ImageName.Name = "layoutViewField_ImageName";
			layoutViewField_ImageName.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			layoutViewField_ImageName.Size = new System.Drawing.Size(32, 17);
			layoutViewField_ImageName.TextLocation = DevExpress.Utils.Locations.Bottom;
			layoutViewField_ImageName.TextSize = new System.Drawing.Size(0, 0);
			layoutViewField_ImageName.TextToControlDistance = 0;
			layoutViewField_ImageName.TextVisible = false;
			layoutViewTemplateCard.CustomizationFormText = "layoutViewTemplateCard";
			layoutViewTemplateCard.HeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
			layoutViewTemplateCard.GroupBordersVisible = false;
			layoutViewTemplateCard.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			layoutViewField_Image,
			layoutViewField_ImageName});
			layoutViewTemplateCard.Name = "layoutViewTemplateCard";
			layoutViewTemplateCard.Text = "layoutViewTemplateCard";
			layoutView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(layoutView_FocusedRowChanged);
			control.MouseDoubleClick += new MouseEventHandler(control_MouseDoubleClick);
			control.KeyDown += new System.Windows.Forms.KeyEventHandler(control_KeyDown);
			Refresh();
			return control;
		}
		protected override void AssignDataSourceToControl(Object dataSource) {
			if(controlDataSource != dataSource) {
				IBindingList oldBindable = controlDataSource as IBindingList;
				if(oldBindable != null) {
					oldBindable.ListChanged -= new ListChangedEventHandler(dataSource_ListChanged);
				}
				controlDataSource = dataSource;
				IBindingList bindable = controlDataSource as IBindingList;
				if(bindable != null) {
					bindable.ListChanged += new ListChangedEventHandler(dataSource_ListChanged);
				}
				Refresh();
			}
		}
		public WinThumbnailEditor(IModelListView model)
			: base(model) {
			popupMenu = new ActionsDXPopupMenu();
		}
		public override void Dispose() {
			controlDataSource = null;
			if(popupMenu != null) {
				popupMenu.Dispose();
				popupMenu = null;
			}
			base.Dispose();
		}
		public override void Refresh() {
			if(control == null) {
				return;
			}
			control.DataSource = controlDataSource;
		}
		public override IList GetSelectedObjects() {
			if(layoutView == null || DataSource == null)
				return new object[0] { };
			int[] selection = layoutView.GetSelectedRows();
			List<object> result = new List<object>();
			for(int i = 0; i < selection.Length; i++) {
				if(layoutView.IsDataRow(selection[i])) {
					result.Add(((IList)DataSource)[layoutView.GetDataSourceRowIndex(selection[i])]);
				}
			}
			return result;
		}
		public override void SaveModel() {
		}
		public override SelectionType SelectionType {
			get { return SelectionType.Full; }
		}
		public override object FocusedObject {
			get {
				if(layoutView != null) {
					return layoutView.GetFocusedRow();
				}
				return null;
			}
			set {
				if(layoutView != null) {
					layoutView.SetFocusedValue(value);
				}
			}
		}
		public override IContextMenuTemplate ContextMenuTemplate {
			get { return popupMenu; }
		}
		public override string[] RequiredProperties {
			get { return new string[] { "OriginalImage", "ImageName" }; }
		}
		#region IDXPopupMenuHolder Members
		public bool CanShowPopupMenu(System.Drawing.Point position) {
			return true;
		}
		public void SetMenuManager(IDXMenuManager manager) { }
		public Control PopupSite {
			get { return control; }
		}
		#endregion
	}
}
