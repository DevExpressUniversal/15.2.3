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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet {
	[Designer("DevExpress.XtraSpreadsheet.Design.SpreadsheetDockManagerDesigner," + AssemblyInfo.SRAssemblySpreadsheetDesign)]
	[DXToolboxItem(false)]
	[ToolboxBitmap(typeof(SpreadsheetDockManager), DevExpress.Utils.ControlConstants.BitmapPath + "SpreadsheetDockManager.bmp")]
	public class SpreadsheetDockManager : DockManager {
		DockPanelDesignType dockPanelDesignType;
		bool createTypedDockPanels;
		SpreadsheetControl spreadsheetControl;
		public SpreadsheetDockManager()
			: base() {
		}
		public SpreadsheetDockManager(ContainerControl form)
			: base(form) {
		}
		public SpreadsheetDockManager(IContainer container)
			: base(container) {
		}
		public SpreadsheetControl SpreadsheetControl {
			get { return spreadsheetControl; }
			set {
				spreadsheetControl = value;
				SetSpreadsheetControlProperty();
			}
		}
		void SetSpreadsheetControlProperty() {
			foreach(DockPanel panel in Panels) {
				FieldListDockPanel fieldListDockPanel = panel as FieldListDockPanel;
				if(fieldListDockPanel != null)
					fieldListDockPanel.SpreadsheetControl = SpreadsheetControl;
				MailMergeParametersDockPanel mailMergeParametersPanel = panel as MailMergeParametersDockPanel;
				if(mailMergeParametersPanel != null)
					mailMergeParametersPanel.SpreadsheetControl = SpreadsheetControl;
			}
		}
		public void InitializeCore() {
			Clear();
		}
		protected void SetDefaultPanels() {
			AddDockPanel(DockingStyle.Right);
		}
		protected DockPanel AddDockPanel(DockingStyle dockingStyle) {
			return AddPanel(dockingStyle);
		}
		protected override DockPanel CreateDockPanel(DockingStyle dock, bool createControlContainer) {
			if (!createControlContainer || !createTypedDockPanels) return base.CreateDockPanel(dock, createControlContainer);
			switch(dockPanelDesignType) {
				case DockPanelDesignType.FieldListDockPanel:
					return new FieldListDockPanel(this, dock);
				case DockPanelDesignType.MailMergeParametersDockPanel:
					return new MailMergeParametersDockPanel(this, dock);
				default:
					return base.CreateDockPanel(dock, createControlContainer);
			}			
		}
		protected override string GetDockPanelText(DockPanel panel) {
			if(panel is FieldListDockPanel || panel is MailMergeParametersDockPanel)
				return panel.Text;
			return base.GetDockPanelText(panel);
		}
		public void CreateFieldListDockPanel() {
			createTypedDockPanels = true;
			dockPanelDesignType = DockPanelDesignType.FieldListDockPanel;
			FieldListDockPanel fieldListDockPanel = (FieldListDockPanel) AddDockPanel(DockingStyle.Right);
			AddToContainer<MailMergeParametersDockPanel>(fieldListDockPanel);
			fieldListDockPanel.SpreadsheetControl = SpreadsheetControl;
			createTypedDockPanels = false;
		}
		public void CreateMailMergeParametersDockPanel() {
			createTypedDockPanels = true;
			dockPanelDesignType = DockPanelDesignType.MailMergeParametersDockPanel;
			MailMergeParametersDockPanel parametersDockPanel = (MailMergeParametersDockPanel) AddDockPanel(DockingStyle.Right);
			AddToContainer<FieldListDockPanel>(parametersDockPanel);
			parametersDockPanel.SpreadsheetControl = SpreadsheetControl;
			createTypedDockPanels = false;
		}
		void AddToContainer<T>(DockPanel parametersDockPanel) where T : DockPanel {
			if(Panels.Count == 2 && CheckDefaultParamsOnPanel<T>()) {
				parametersDockPanel.DockTo(Panels[0]);
			}
		}
		bool CheckDefaultParamsOnPanel<T>() where T : DockPanel {
			return Panels[0] is T && Panels[0].Dock == DockingStyle.Right && !Panels[0].Parent.GetType().IsAssignableFrom(typeof(DockPanel));
		}
		public bool Contains(Type dockPanelType) {
			foreach(DockPanel dockPanel in Panels) {
				if(dockPanel.GetType() == dockPanelType)
					return true;
			}
			return false;
		}
	}
	enum DockPanelDesignType {
		FieldListDockPanel,
		MailMergeParametersDockPanel
	}
	[DXToolboxItem(false)]
	public class FieldListDockPanel : DockPanel {
		SpreadsheetControl spreadsheetControl;
		SpreadsheetFieldListTreeView fieldList;
		[ DefaultValue(null)]
		public SpreadsheetControl SpreadsheetControl {
			get { return spreadsheetControl; }
			set {
				spreadsheetControl = value;
				if(fieldList != null)
					fieldList.SpreadsheetControl = value;
			}
		}
		string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldListDockPanel_Text); } }
		public override string Text { get { return base.Text; } set { base.Text = value; }}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			Control control = fieldList;
			if(control != null && control.Parent == null && ControlContainer != null)
				ControlContainer.Controls.Add(control);
		}
		protected override void CreateControlContainer() {
			InitializeControl();
			base.CreateControlContainer();
		}
		public FieldListDockPanel() {
			InitializeControl();
			Text = DefaultText;
		}
		public FieldListDockPanel(DockManager dockManager, DockingStyle dock) : base(true, dock, dockManager) {
			Dock = dock;
			InitializeControl();
			Text = DefaultText;
		}
		protected void InitializeControl() {
			if(fieldList == null) {
				fieldList = new SpreadsheetFieldListTreeView {
					Dock = DockStyle.Fill,
					SpreadsheetControl = SpreadsheetControl
				};
			}
			if (!DesignMode)
				fieldList.RefreshTreeList();
		}
		public override void ResetText() {
			Text = DefaultText;
		}
		protected virtual bool ShouldSerializeText() {
			return Text != DefaultText;
		}  
	}
}
