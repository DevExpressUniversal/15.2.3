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
using System.Collections;
using DevExpress.XtraReports.UI;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraPrinting;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UserDesigner.Native;
using System.Collections.Generic;
using DevExpress.XtraReports.Design.MouseTargets;
using DevExpress.XtraReports.Design.Behaviours;
using DevExpress.XtraPrinting.Native;
using System.Linq;
namespace DevExpress.XtraReports.Design {
	[DesignerBehaviour(typeof(TableDesignerBehaviour))]
	public class XRTableDesigner : XRControlDesigner {
		Dictionary<XRTableCell, object> mergedCells;
		public override bool CanDragInReportExplorer { get { return true; } }
		protected XRTable Table { get { return Component as XRTable; } }
		protected XRTableRow FirstRow { get { return Table.Rows.FirstRow; } }
		protected XRTableRow LastRow { get { return Table.Rows.LastRow; } }
		public Dictionary<XRTableCell, object> MergedCells {
			get {
				if(mergedCells == null) CreateMergedCellsDictionary();
				return mergedCells;
			}
		}
		public XRTableDesigner() { }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			Verbs.Clear();
			IIdleService serv = GetService(typeof(IIdleService)) as IIdleService;
			serv.Idle += HandleGrabTick;
			Table.TableModifier = new DesignTableModifier(Table, this.DesignerHost);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				IIdleService serv = GetService(typeof(IIdleService)) as IIdleService;
				if(serv != null)
					serv.Idle -= HandleGrabTick;
				Table.TableModifier = null;
			}
			base.Dispose(disposing);
		}
		public override void OnComponentChanged(ComponentChangedEventArgs e) {
			base.OnComponentChanged(e);
			ClearMergedCells();
		}
		protected override void CorrectControlHeight() {
		}
		internal virtual void CreateDefaultTable(RectangleF tableBounds) {
			changeService.OnComponentChanging(Table, null);
			Table.BeginInit();
			if(tableBounds != RectangleF.Empty)
				Table.BoundsF = tableBounds;
			if(Table.Rows.Count > 0) {
				Table.EndInit();
				return;
			}
			Table.InsertRowBelow(null);
			for(int i = 0; i < 3; i++)
				Table.Rows[0].InsertCellByIndex(null, new XRTableCell(), i, false, false);
			Table.EndInit();
			changeService.OnComponentChanged(Table, null, null, null);
		}
		internal void AddTablesToContainer(List<XRTable> tables) {
			foreach(XRTable table in tables) {
				XRControl parent = table.Parent;
				if(parent != null) {
					RaiseComponentChanging(parent, ReportDesignerHelper.GetDefaultCollectionName(parent));
					AddTableToContainer(table);
					RaiseComponentChanged(parent);
				}
			}
		}
		protected void AddTableToContainer(XRTable table) {
			fDesignerHost.AddToContainer(table, false);
			foreach(XRTableRow row in table.Rows) {
				DesignToolHelper.AddToContainer(fDesignerHost, row);
				foreach(XRTableCell cell in row.Cells)
					DesignToolHelper.AddToContainer(fDesignerHost, cell);
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRControlBookmarkDesignerActionList(this));
		}
		public override IEnumerator GetEnumerator() {
			return CreateEnumerator(this.XRControl as XRTable);
		}
		IEnumerator CreateEnumerator(XRTable control) {
			ArrayList list = new ArrayList();
			foreach(XRTableRow row in control.Controls) {
				foreach(XRTableCell cell in row.Controls)
					if(cell.RowSpan > 1)
						list.Add(cell);
			}
			list.AddRange(control.Controls);
			return list.GetEnumerator();
		}
		void CreateMergedCellsDictionary() {
			mergedCells = new Dictionary<XRTableCell, object>();
			foreach(XRTableRow row in Table.Controls) {
				foreach(XRTableCell cell in row.Controls)
					RowSpanHelper.MergeCells(cell, mergedCells);
			}
		}
		public void ClearMergedCells() {
			mergedCells = null;
		}
	}
	public class XRTableRowDesigner : XRControlDesigner {
		class HeightPropertyDescriptor : PropertyDescriptorWrapper {
			XRTableRowDesigner designer;
			public HeightPropertyDescriptor(PropertyDescriptor oldPropertyDescriptor, XRTableRowDesigner designer)
				: base(oldPropertyDescriptor) {
				this.designer = designer;
			}
			public override void SetValue(object component, object value) {
				designer.HeightF = (float)value;
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRControl.Height"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(SingleTypeConverter)),
		CategoryAttribute("Layout"),
		Browsable(true)
		]
		public float HeightF {
			get { return Row.HeightF; }
			set {
				RectangleF bounds = Row.BoundsF;
				bounds.Height = value;
				SetBounds(bounds);
			}
		}
		protected XRTableRow Row { get { return Component as XRTableRow; } }
		protected XRTable Table { get { return Row.Table; } }
		public XRTableRowDesigner() { }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			Row.TableRowModifier = new DesignTableRowModifier(Row, this.DesignerHost);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Row.TableRowModifier = null;
			}
			base.Dispose(disposing);
		}
		protected override XRControl FindParent() {
			return null;
		}
		public override string GetStatus() {
			try {
				return String.Format("{0}.{1} {{ {6}:{2},{3} {7}:{4},{5} }}",
					Table.Site.Name,
					Component.Site.Name,
					(int)Math.Round(XRControl.LeftF),
					(int)Math.Round(XRControl.TopF),
					(int)Math.Round(XRControl.WidthF),
					(int)Math.Round(XRControl.HeightF),
					ReportLocalizer.GetString(ReportStringId.DesignerStatus_Location), ReportLocalizer.GetString(ReportStringId.DesignerStatus_Size));
			} catch { return string.Empty; }
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			properties[XRComponentPropertyNames.Height] = new HeightPropertyDescriptor(TypeDescriptor.GetProperties(this)[XRComponentPropertyNames.Height], this);
		}
		public override bool ShouldSnapBounds {
			get { return false; }
		}
		public override bool CanCutControl {
			get { return false; }
		}
		protected override SelectionRules GetSelectionRulesCore() {
			ICollection comps = selectionService.GetSelectedComponents();
			foreach(object item in comps)
				if(item is XRControl && !item.Equals(XRControl) && ((XRControl)item).Parent == XRControl.Parent) return SelectionRules.None;
			return SelectionRules.TopSizeable | SelectionRules.BottomSizeable;
		}
		public void SetSpecifiedRowBounds(RectangleF rect) {
			RowBoundsSetter.SetSpecifiedBounds(XRControl, rect, changeService);
		}
	}
	[MouseTarget(typeof(TableCellMouseTarget))]
	public class XRTableCellDesigner : XRTextControlDesigner {
		class WidthPropertyDescriptor : PropertyDescriptorWrapper {
			XRTableCellDesigner designer;
			public WidthPropertyDescriptor(PropertyDescriptor oldPropertyDescriptor, XRTableCellDesigner designer)
				: base(oldPropertyDescriptor) {
				this.designer = designer;
			}
			public override void SetValue(object component, object value) {
				designer.WidthF = (float)value;
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRControl.Width"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(SingleTypeConverter)),
		CategoryAttribute("Layout"),
		Browsable(true)
		]
		public float WidthF {
			get { return Cell.WidthF; }
			set {
				RectangleF bounds = Cell.BoundsF;
				bounds.Width = value;
				SetBounds(bounds);
			}
		}
		public override bool CanDragInReportExplorer {
			get {
				return Row.Cells.Count > 1;
			}
		}
		public override RectangleF ControlBoundsRelativeToBand {
			get {
				RectangleF result = base.ControlBoundsRelativeToBand;
				if(Cell.RowSpan > 1) result.Height = RowSpanHelper.CalculateMergedCellHeight(Cell, TableMergedCells);
				return result;
			}
		} 
		protected XRTableCell Cell { get { return Component as XRTableCell; } }
		protected XRTableRow Row { get { return Cell.Row; } }
		protected XRTable Table { get { return Row != null ? Row.Table : null; } }
		XRTableDesigner TableDesigner { get { return (XRTableDesigner)DesignerHost.GetDesigner(Table); } }
		Dictionary<XRTableCell, object> TableMergedCells { get { return TableDesigner.MergedCells; } }
		bool IsMerged { get { return TableMergedCells.ContainsKey(Cell) && TableMergedCells.Count > 1; } }
		public XRTableCellDesigner() { }
		protected override XRControl FindParent() {
			return null;
		}
		public override string GetStatus() {
			try {
				if(XRControl != null && Row != null && Row.Site != null && Component != null && Component.Site != null)
					return String.Format("{0}.{1} {{ {6}:{2},{3} {7}:{4},{5} }}",
						Row.Site.Name,
						Component.Site.Name,
						(int)Math.Round(XRControl.LeftF),
						(int)Math.Round(XRControl.TopF),
						(int)Math.Round(XRControl.WidthF),
						(int)Math.Round(XRControl.HeightF),
						ReportLocalizer.GetString(ReportStringId.DesignerStatus_Location),
						ReportLocalizer.GetString(ReportStringId.DesignerStatus_Size));
			} catch { }
			return string.Empty;
		}
		public override void OnComponentChanged(ComponentChangedEventArgs e) {
			base.OnComponentChanged(e);
			if(fEditor != null) fEditor.UpdateProperties(null);
			if(Table != null && TableDesigner != null) TableDesigner.ClearMergedCells();
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			properties[XRComponentPropertyNames.Width] = new WidthPropertyDescriptor(TypeDescriptor.GetProperties(this)[XRComponentPropertyNames.Width], this);
		}
		protected override SelectionRules GetSelectionRulesCore() {
			ICollection comps = selectionService.GetSelectedComponents();
			foreach(object item in comps) {
				if(item is XRControl && !item.Equals(XRControl) && ((XRControl)item).Parent == XRControl.Parent)
					return SelectionRules.None;
			}
			return Row.Cells.Count == 1 ? SelectionRules.None :
				SelectionRules.LeftSizeable | SelectionRules.RightSizeable;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRLabelDesignerActionList1(this));
			list.Add(new XRFormattingControlDesignerActionList(this));
			list.Add(new XRLabelDesignerActionList3(this));
		}
		protected override InplaceTextEditorBase GetInplaceEditor(string text, bool selectAll) {
			return new InplaceTextEditor(fDesignerHost, Cell, text, selectAll);
		}
		public override bool ShouldSnapBounds {
			get { return false; }
		}
		public override bool CanCutControl {
			get { return false; }
		}	   
		public override bool CanHaveChildren {
			get { return IsMerged ? false : base.CanHaveChildren; }
		}
		public override bool CanAddControl(Type type) {
			return IsMerged ? false : base.CanAddControl(type);
		}
		protected override bool SetFormatStringCore(string propName, string formatString) {
			if(Cell.HasSummary) {
				Cell.Summary.FormatString = formatString;
				return true;
			}
			return base.SetFormatStringCore(propName, formatString);
		}
		public override RectangleF GetBounds(Band band, GraphicsUnit unit) {
			RectangleF result = base.GetBounds(band, unit);
			if(!TableMergedCells.ContainsKey(Cell)) return result;
			if(Cell.RowSpan < 2)
				return RectangleF.Empty;
			result.Height = GraphicsUnitConverter.Convert(RowSpanHelper.CalculateMergedCellHeight(Cell, TableMergedCells), Cell.Dpi, GraphicsDpi.Document);
			return result;
		}
		public List<XRTableCell> GetColumnCells(XRTableCell baseCell) {
			List<XRTableCell> cells = new List<XRTableCell>();
			foreach(XRTableRow row in Table.Rows) {
				foreach(XRTableCell cell in row.Cells)
					if((FloatsComparer.Default.FirstEqualsSecond(cell.LeftF, baseCell.LeftF) 
						&& FloatsComparer.Default.FirstEqualsSecond(cell.RightF, baseCell.RightF)
						&& (!TableMergedCells.ContainsKey(cell) || cell.RowSpan > 1)))
						cells.Add(cell);
			}
			return cells;
		}
		public void SetSpecifiedCellBounds(RectangleF rect) {
			CellBoundsSetter.SetSpecifiedBounds(XRControl, rect, changeService);
		}
		public void SetProportionalCellBounds(RectangleF rect) {
			CellBoundsSetter.SetProportionalBounds(XRControl, rect, changeService);
		}
	}
}
