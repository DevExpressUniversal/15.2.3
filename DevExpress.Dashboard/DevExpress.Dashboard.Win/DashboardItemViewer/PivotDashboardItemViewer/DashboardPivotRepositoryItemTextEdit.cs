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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.DashboardWin.Native {
	[UserRepositoryItem("RegisterDashboardPivotRepositoryItemTextEdit")]
	public class DashboardPivotRepositoryItemTextEdit : RepositoryItemTextEdit {
		static DashboardPivotRepositoryItemTextEdit() {
			RegisterDashboardPivotRepositoryItemTextEdit();
		}
		public static void RegisterDashboardPivotRepositoryItemTextEdit() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(TypeName,
			  typeof(TextEdit), typeof(DashboardPivotRepositoryItemTextEdit),
			  typeof(DashboardPivotTextEditViewInfo), new DashboardPivotTextEditPainter(), true));
		}
		const string TypeName = "DashboardPivotRepositoryItemTextEdit";
		DashboardPivotTextEditPainter painter;
		event EventHandler<PivotCustomDrawCellEventArgsBase> customDrawCell;
		event EventHandler<DisplayTextHiddenEventArgs> displayTextHidden;
		event EventHandler lookAndFeelChanged;
		PivotDrillDownDataSource drillDownDataSource;
		string valueFieldName;
		public override string EditorTypeName { get { return TypeName; } }
		protected override EditorClassInfo EditorClassInfo { get { return EditorRegistrationInfo.Default.Editors["TextEdit"]; } }
		public PivotDrillDownDataSource DrillDownDataSource { get { return drillDownDataSource; } }
		public string ValueFieldName { get { return valueFieldName; } }
		public event EventHandler<PivotCustomDrawCellEventArgsBase> CustomDrawCell {
			add { customDrawCell += value; }
			remove { customDrawCell -= value; }
		}
		public event EventHandler<DisplayTextHiddenEventArgs> DisplayTextHidden {
			add { displayTextHidden += value; }
			remove { displayTextHidden -= value; }
		}
		public event EventHandler LookAndFeelChanged {
			add { lookAndFeelChanged += value; }
			remove { lookAndFeelChanged -= value; }
		}
		public void Update(PivotDrillDownDataSource drillDownDataSource, string valueFieldName) {
			this.drillDownDataSource = drillDownDataSource;
			this.valueFieldName = valueFieldName;
		}
		public override BaseEditViewInfo CreateViewInfo() {
			return new DashboardPivotTextEditViewInfo(this);
		}
		public override BaseEditPainter CreatePainter() {
			painter = new DashboardPivotTextEditPainter();
			painter.CustomDrawCell += OnPainterCustomDrawCell;
			painter.DisplayTextHidden += OnPainterDisplayTextHidden;
			return painter;
		}
		void OnPainterCustomDrawCell(object sender, PivotCustomDrawCellEventArgsBase e) {
			if(customDrawCell != null)
				customDrawCell(this, e);
		}
		void OnPainterDisplayTextHidden(object sender, DisplayTextHiddenEventArgs e) {
			if(displayTextHidden != null)
				displayTextHidden(this, e);
		}
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			base.OnLookAndFeelChanged(sender, e);
			if(lookAndFeelChanged != null)
				lookAndFeelChanged(this, EventArgs.Empty);
		}
		protected override void Dispose(bool disposing) {
			if(painter != null) {
				painter.CustomDrawCell -= OnPainterCustomDrawCell;
				painter.DisplayTextHidden -= OnPainterDisplayTextHidden;
			}
			base.Dispose(disposing);
		}
	}
	public class DashboardPivotTextEditViewInfo : TextEditViewInfo {
		DashboardPivotRepositoryItemTextEdit editor;
		public PivotDrillDownDataSource DrillDownDataSource { get { return editor.DrillDownDataSource; } }
		public string ValueFieldName { get { return editor.ValueFieldName; } }
		public DashboardPivotTextEditViewInfo(DashboardPivotRepositoryItemTextEdit editor)
			: base(editor) {
			this.editor = editor;
		}
	}
	public class DashboardPivotTextEditPainter : TextEditPainter {
		event EventHandler<PivotCustomDrawCellEventArgsBase> customDrawCell;
		event EventHandler<DisplayTextHiddenEventArgs> displayTextHidden;
		public event EventHandler<PivotCustomDrawCellEventArgsBase> CustomDrawCell {
			add { customDrawCell += value; }
			remove { customDrawCell -= value; }
		}
		public event EventHandler<DisplayTextHiddenEventArgs> DisplayTextHidden {
			add { displayTextHidden += value; }
			remove { displayTextHidden -= value; }
		}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			DashboardPivotTextEditViewInfo viewInfo = info.ViewInfo as DashboardPivotTextEditViewInfo;
			AppearanceObject appearance = viewInfo.PaintAppearance;
			PivotDrillDownDataSource drillDownDataSource = viewInfo.DrillDownDataSource;
			PropertyDescriptorCollection properties = drillDownDataSource.GetItemProperties(null);
			PropertyDescriptor columnTag = properties["ColumnTag"];
			AxisPoint columnAxisPoint = columnTag.GetValue(drillDownDataSource[0]) as AxisPoint;
			PropertyDescriptor rowTag = properties["RowTag"];
			AxisPoint rowAxisPoint = rowTag.GetValue(drillDownDataSource[0]) as AxisPoint;
			PivotDashboardItemCustomDrawCellEventArgs args = new PivotDashboardItemCustomDrawCellEventArgs(columnAxisPoint, rowAxisPoint, viewInfo.ValueFieldName, appearance, true, DashboardWinHelper.IsDarkScheme(viewInfo.LookAndFeel), StyleSettingsContainerPainter.GetDefaultBackColor(viewInfo.LookAndFeel));
			if(customDrawCell != null)
				customDrawCell(this, args);
			appearance.FillRectangle(info.Cache, info.Bounds);
			StyleSettingsInfo styleSettings = args.StyleSettings;
			StringFormat format = appearance.GetStringFormat();
			if(styleSettings.Bar != null) {
				if(styleSettings.Bar.ShowBarOnly && displayTextHidden != null)
					displayTextHidden(this, new DisplayTextHiddenEventArgs(columnAxisPoint, rowAxisPoint, viewInfo.ValueFieldName));
				DashboardCellPainter.DrawContentWithBar(viewInfo, format, info.Cache, styleSettings.Bar, info.Bounds);
			}
			else if(styleSettings.Image != null)
				DashboardCellPainter.DrawContentWithIcon(viewInfo, info.Cache, format, styleSettings, info.Bounds);
			else
				base.DrawContent(info);
		}
	}
}
