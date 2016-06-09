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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.Utils.Menu;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Menu;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	public partial class DraggableDataFieldsBrowser : DataFieldsBrowser, IDraggableDataFieldsBrowserView {
		Point lastMouseDownLocation;
		DataField lastMouseDownDataField;
		event EventHandler<DataFieldStartDragEventArgs> dataFieldStartDrag;
		event EventHandler calculatedFieldAdd;
		event EventHandler<DataFieldEventArgs> dataFieldDelete;
		event EventHandler<DataFieldEventArgs> dataFieldRename;
		event EventHandler<DataFieldBeginRenameEventArgs> dataFieldRenamed;
		event EventHandler dataFieldAfterRenamed;
		event EventHandler<DataFieldEventArgs> dataFieldEdit;
		event EventHandler<CalculatedFieldEditTypeEventArgs> calculatedFieldEditType;
		event EventHandler<DataFieldActionsAvailabilityEventArgs> dataFieldActionsAvailability;
		public DraggableDataFieldsBrowser() {
			InitializeComponent();
			TreeList.MouseMove += treeList_MouseMove;
			TreeList.MouseDown += treeList_MouseDown;
			TreeList.PopupMenuShowing += treeList_PopupMenuShowing;
			TreeList.CellValueChanged += treeList_CellValueChanged;
			TreeList.HiddenEditor += treeList_HiddenEditor;
			TreeList.KeyDown += treeList_KeyDown;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}
		void treeList_MouseDown(object sender, MouseEventArgs e) {
			lastMouseDownLocation = e.Location;
			lastMouseDownDataField = GetDataField(e.Location);
		}
		void treeList_MouseMove(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.None && lastMouseDownLocation != e.Location) {
				DataField dataField = GetDataField(e.Location);
				if (dataField != null && dataField == lastMouseDownDataField) {
					if(dataFieldStartDrag != null) {
						dataFieldStartDrag(this, new DataFieldStartDragEventArgs(dataField, e.Location));
						lastMouseDownDataField = null;
					}
				}
			}
		}
		void treeList_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
			TreeListNode selectedNode = ((TreeListNodeMenu)e.Menu).Node;
			DataField selectedField = GetDataField(selectedNode);
			DataFieldActionsAvailabilityEventArgs availability = null;
			if(dataFieldActionsAvailability != null) {
				availability = new DataFieldActionsAvailabilityEventArgs(selectedField);
				dataFieldActionsAvailability(this, availability);
			}
			if (availability != null) {
				if (availability.AddCalculatedField) {
					e.Menu.Items.Add(new DXMenuItem(DashboardWinLocalizer.GetString(DashboardWinStringId.CommandAddCalculatedFieldCaption), (s1, e1) => {
						RaiseCalculatedFieldAdd();
					}));
				}
				if (availability.EditDataField) {
					e.Menu.Items.Add(new DXMenuItem(DashboardWinLocalizer.GetString(DashboardWinStringId.DataFieldBrowserCalculatedFieldExpressionEditorMenuItem), (s1, e1) => {
						RaiseDataFieldEdit(selectedField);
					}) {
						BeginGroup = true 
					});
				}
				if (availability.EditCalculatedFieldType) {
					DXSubMenuItem subItem = new DXSubMenuItem(DashboardWinLocalizer.GetString(DashboardWinStringId.DataFieldBrowserCalculatedFieldTypeMenuItem));
					foreach (CalculatedFieldType type in Enum.GetValues(typeof(CalculatedFieldType))) {
						subItem.Items.Add(new DXMenuCheckItem(type.GetLocalizedName(), availability.CalculatedFieldType == type, GetCalculatedFieldTypeImage(type), (s1, e1) => {
							CalculatedFieldType newType = (CalculatedFieldType)((DXMenuCheckItem)s1).Tag;
							if(calculatedFieldEditType != null)
								calculatedFieldEditType(this, new CalculatedFieldEditTypeEventArgs(selectedField, newType));
						}) {
							Tag = type
						});
					}
					e.Menu.Items.Add(subItem);
				}
				if (availability.RenameDataField) { 
					e.Menu.Items.Add(new DXMenuItem(DashboardWinLocalizer.GetString(DashboardWinStringId.DataFieldBrowserRenameCalculatedFieldMenuItem), (s1, e1) => {
						TreeList.FocusedNode = selectedNode;
						RaiseDataFieldRename(selectedField);
					}));
				}
				if (availability.DeleteDataField) {
					e.Menu.Items.Add(new DXMenuItem(DashboardWinLocalizer.GetString(DashboardWinStringId.DataFieldBrowserRemoveCalculatedFieldMenuItem), (s1, e1) => {
						RaiseDataFieldDelete(selectedField);
					}) {
						BeginGroup = true,
						Image = ImageHelper.GetImage("Bars.DeleteItem_16x16")
					});
				}
			}
		}
		void treeList_CellValueChanged(object sender, CellValueChangedEventArgs e) {
			if (dataFieldRenamed != null)
				dataFieldRenamed(this, new DataFieldBeginRenameEventArgs(GetDataField(e.Node), e.Value as string));
		}
		void treeList_HiddenEditor(object sender, EventArgs e) {
			TreeList.OptionsBehavior.Editable = false;
			if (dataFieldAfterRenamed != null)
				dataFieldAfterRenamed(this, EventArgs.Empty);
		}
		void treeList_KeyDown(object sender, KeyEventArgs e) {
			if(e.Handled || TreeList.FocusedNode == null)
				return;
			DataField selectedField = GetDataField(TreeList.FocusedNode);
			switch(e.KeyCode) {
				case Keys.Delete:
					RaiseDataFieldDelete(selectedField);
					e.Handled = true;
					break;
				case Keys.F2:
					RaiseDataFieldRename(selectedField);
					e.Handled = true;
					break;
				case Keys.F4:
					RaiseDataFieldEdit(selectedField);
					e.Handled = true;
					break;
				default:
					break;
			}
		}
		void RaiseCalculatedFieldAdd() {
			if (calculatedFieldAdd != null)
				calculatedFieldAdd(this, EventArgs.Empty);
		}
		void RaiseDataFieldDelete(DataField selectedField) {
			if (dataFieldDelete != null)
				dataFieldDelete(this, new DataFieldEventArgs(selectedField));
		}
		void RaiseDataFieldRename(DataField selectedField) {
			if (dataFieldRename != null)
				dataFieldRename(this, new DataFieldEventArgs(selectedField));
		}
		void RaiseDataFieldEdit(DataField selectedField) {
			if (dataFieldEdit != null)
				dataFieldEdit(this, new DataFieldEventArgs(selectedField));
		}
		Image GetCalculatedFieldTypeImage(CalculatedFieldType type) {
			int index = DataFieldsBrowserItem.GetImageIndex(DataFieldsBrowserItem.CalculatedFieldTypeToDataFieldsBrowserGroupType(type));
			if (index > 0 && ImageList.Images.Count > index)
				return ImageList.Images[index];
			return null;
		}		
		#region IDraggableDataFieldsBrowserView members
		event EventHandler<DataFieldStartDragEventArgs> IDraggableDataFieldsBrowserView.DataFieldStartDrag {
			add { dataFieldStartDrag += value; }
			remove { dataFieldStartDrag -= value; }
		}
		event EventHandler IDraggableDataFieldsBrowserView.CalculatedFieldAdd {
			add { calculatedFieldAdd += value; }
			remove { calculatedFieldAdd -= value; }
		}
		event EventHandler<DataFieldEventArgs> IDraggableDataFieldsBrowserView.DataFieldDelete {
			add { dataFieldDelete += value; }
			remove { dataFieldDelete -= value; }
		}
		event EventHandler<DataFieldEventArgs> IDraggableDataFieldsBrowserView.DataFieldRename {
			add { dataFieldRename += value; }
			remove { dataFieldRename -= value; }
		}
		event EventHandler<DataFieldBeginRenameEventArgs> IDraggableDataFieldsBrowserView.DataFieldRenamed {
			add { dataFieldRenamed += value; }
			remove { dataFieldRenamed -= value; }
		}
		event EventHandler IDraggableDataFieldsBrowserView.DataFieldAfterRenamed {
			add { dataFieldAfterRenamed += value; }
			remove { dataFieldAfterRenamed -= value; }
		}
		event EventHandler<DataFieldEventArgs> IDraggableDataFieldsBrowserView.DataFieldEdit {
			add { dataFieldEdit += value; }
			remove { dataFieldEdit -= value; }
		}
		event EventHandler<CalculatedFieldEditTypeEventArgs> IDraggableDataFieldsBrowserView.CalculatedFieldEditType {
			add { calculatedFieldEditType += value; }
			remove { calculatedFieldEditType -= value; }
		}
		event EventHandler<DataFieldActionsAvailabilityEventArgs> IDraggableDataFieldsBrowserView.DataFieldActionsAvailability {
			add { dataFieldActionsAvailability += value; }
			remove { dataFieldActionsAvailability -= value; }
		}
		void IDraggableDataFieldsBrowserView.RenameSelectedDataField() {
			TreeList.OptionsBehavior.Editable = true;
			TreeList.ShowEditor();
		}
		#endregion
	}
}
