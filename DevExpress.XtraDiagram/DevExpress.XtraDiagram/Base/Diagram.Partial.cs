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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Adorners;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.Diagram.Core.Base;
using System.Windows.Forms;
using DevExpress.Diagram.Core.Native;
using DevExpress.XtraEditors;
using DevExpress.Diagram.Core.Localization;
using DevExpress.XtraDiagram.Base;
namespace DevExpress.XtraDiagram {
	public partial class DiagramControl : IXtraDiagramControl {
		public static readonly DiagramItemTypeRegistrator ItemTypeRegistrator = new DiagramItemTypeRegistrator();
		DiagramController IDiagramControl.Controller { get { return Controller; } }
		double IDiagramControl.ZoomFactor {
			get { return OptionsView.ZoomFactor; }
			set { OptionsView.ZoomFactor = value; }
		}
		SelectionModel<IDiagramItem> IDiagramControl.SelectionModel {
			get { return SelectionModel; }
			set { SelectionModel = value; }
		}
		double IDiagramControl.BringIntoViewMargin { get { return OptionsBehavior.BringIntoViewMargin; } }
		SelectionToolsModel<IDiagramItem> IDiagramControl.SelectionToolsModel {
			get { return SelectionToolsModel; }
			set { SelectionToolsModel = value; }
		}
		SelectionToolsModel<IDiagramItem> IDiagramControl.RootToolsModel {
			get { return RootToolsModel; }
			set { RootToolsModel = value; }
		}
		bool IDiagramControl.SnapToGrid { get { return OptionsBehavior.SnapToGrid; } }
		bool IDiagramControl.SnapToItems { get { return OptionsBehavior.SnapToItems; } }
		bool IDiagramControl.CanUndo {
			get { return CanUndo; }
			set { CanUndo = value; }
		}
		bool IDiagramControl.CanRedo {
			get { return CanRedo; }
			set { CanRedo = value; }
		}
		DiagramTheme IDiagramControl.Theme {
			get { return OptionsView.Theme; }
			set { OptionsView.Theme = value; }
		}
		double IDiagramControl.MinDragDistance { get { return OptionsBehavior.MinDragDistance; } }
		bool IDiagramControl.HasChanges {
			get { return HasChanges; }
			set { HasChanges = value; }
		}
		Size IDiagramControl.PageSize {
			get { return OptionsView.PageSize.ToPlatformSize(); }
			set { OptionsView.PageSize = value.ToWinSize(); }
		}
		void IDiagramControl.ShowPopupMenu(DiagramMenuPlacement placement) {
			ShowDiagramMenu(placement);
		}
		void IDiagramControl.DestroyPopupMenu() {
		}
		ResizingMode IDiagramControl.ResizingMode { get { return OptionsBehavior.ResizingMode; } }
		void IDiagramControl.RaiseSelectionChanged() {
			RaiseSelectionChanged();
		}
		bool IDiagramControl.AllowEmptySelection { get { return OptionsBehavior.AllowEmptySelection; } }
		void IDiagramControl.MoveFocus(LogicalDirection direction) {
			throw new NotImplementedException();
		}
		double IDiagramControl.SnapToItemsDistance { get { return OptionsBehavior.SnapToItemsDistance; } }
		IAdornerFactory IDiagramControl.CreateAdornerFactory() {
			return new DiagramAdornerFactory(this);
		}
		MeasureUnit IDiagramControl.MeasureUnit { get { return OptionsView.MeasureUnit; } }
		Size? IDiagramControl.GridSize {
			get { return OptionsView.GridSize != null ? (Size?)OptionsView.GridSize.Value.ToPlatformSize() : null; }
		}
		DiagramCommandsBase IDiagramControl.CreateCommands() {
			return diagramCommands;
		}
		IDiagramRoot IDiagramControl.CreateRootItem() { return Page; }
		IDiagramConnector IDiagramControl.CreateConnector() {
			return CreateDefaultConnector();
		}
		void IDiagramControl.UpdateLayout() {
		}
		void IDiagramControl.ApplyLastBackgroundColor() {
			ApplyBackgroundColor();
		}
		void IDiagramControl.ApplyLastForegroundColor() {
			ApplyForegroundColor();
		}
		void IDiagramControl.ApplyLastStrokeColor() {
			ApplyStrokeColor();
		}
		void IDiagramControl.DisplayItemProperties() {
			DisplayItemProperties();
		}
		void IDiagramControl.SetPageSize() {
			SetPageSize();
		}
		IDiagramShape IDiagramControl.CreateShape() {
			return CreateDefaultShape();
		}
		IDiagramContainer IDiagramControl.CreateContainer() {
			return new DiagramContainer();
		}
		void IXtraDiagramControl.LayoutChanged(bool appearanceChanged) {
			LayoutChanged(appearanceChanged);
			UpdateUI(DiagramIdleTask.UpdatePropertyGridTask);
		}
		DiagramTool IDiagramControl.ActiveTool { get { return OptionsBehavior.ActiveTool; } }
		Thickness IDiagramControl.ScrollMargin {
			get { return OptionsView.ScrollMargin.ToThickness(); }
		}
		DiagramItemTypeRegistrator IDiagramControl.ItemTypeRegistrator { get { return ItemTypeRegistrator; } }
		protected virtual void OnGridSizeChanged() {
		}
		double IDiagramControl.GlueToItemDistance { get { return OptionsBehavior.GlueToItemDistance; } }
		double IDiagramControl.GlueToConnectionPointDistance { get { return OptionsBehavior.GlueToConnectionPointDistance; } }
		Diagram.Core.Routing.RoutingStrategy IDiagramControl.GetRoutingStrategy(ConnectorType connectorType) {
			return Controller.GetRoutingStrategy(connectorType);
		}
		string IDiagramControl.ShowOpenFileDialog() {
			IWin32Window owner = Win32.GetOwnerWindow();
			using (OpenFileDialog dlg = new OpenFileDialog()) {
				dlg.Filter = Controller.FileDialogFilter;
				if (dlg.ShowDialog(owner) == DialogResult.OK) {
					return dlg.FileName;
				}
			}
			return string.Empty;
		}
		string IDiagramControl.ShowSaveFileDialog() {
			IWin32Window owner = Win32.GetOwnerWindow();
			using (SaveFileDialog dlg = new SaveFileDialog()) {
				dlg.ValidateNames = true;
				dlg.FileName = Controller.DefaultFileName;
				dlg.Filter = Controller.FileDialogFilter;
				dlg.OverwritePrompt = true;
				if (dlg.ShowDialog(owner) == DialogResult.OK) {
					return dlg.FileName;
				}
			}
			return string.Empty;
		}
		void IDiagramControl.ShowMessage(string message, bool errorFlag) {
			IWin32Window owner = Win32.GetOwnerWindow();
			Form form = owner as Form;
			string caption = form != null ? form.Text : string.Empty;
			if(errorFlag) {
				XtraMessageBox.Show(owner, message, string.IsNullOrEmpty(caption) ? DiagramControlLocalizer.GetString(DiagramControlStringId.DiagramNotification_DefaultCaption) : caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else {
				XtraMessageBox.Show(owner, message, string.IsNullOrEmpty(caption) ? DiagramControlLocalizer.GetString(DiagramControlStringId.DiagramNotification_DefaultCaption) : caption, MessageBoxButtons.OK);
			}
		}
		protected virtual void ApplyBackgroundColor() { }
		protected virtual void ApplyForegroundColor() { }
		protected virtual void ApplyStrokeColor() { }
		protected virtual void DisplayItemProperties() { }
		protected virtual void SetPageSize() { }
	}
}
