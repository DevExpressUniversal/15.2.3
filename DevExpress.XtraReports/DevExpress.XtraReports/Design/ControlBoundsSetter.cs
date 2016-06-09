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
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraReports.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using System.Drawing;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.Design {
	public class ControlChanger { 
		IComponentChangeService changeServ;
		protected ControlChanger(IComponentChangeService changeServ) {
			this.changeServ = changeServ;
		}
		protected void RaiseComponentChanging(IComponent component, string propName) {
			if(changeServ != null && component != null)
				changeServ.OnComponentChanging(component, GetMember(component, propName));
		}
		protected void RaiseComponentChanged(IComponent component, string propName, object oldValue, object newValue) {
			if(changeServ != null && component != null)
				changeServ.OnComponentChanged(component, GetMember(component, propName), oldValue, newValue);
		}
		static MemberDescriptor GetMember(IComponent component, string propName) {
			return string.IsNullOrEmpty(propName) ? null : XRAccessor.GetPropertyDescriptor(component, propName);
		}
	}
	public class ControlBoundsSetter : ControlChanger  {
		static ControlBoundsSetter CreataInstance(Type controlType, IComponentChangeService changeServ) {
			if(typeof(XRTableCell).IsAssignableFrom(controlType))
				return new CellBoundsSetter(changeServ);
			if(typeof(XRTableRow).IsAssignableFrom(controlType))
				return new RowBoundsSetter(changeServ);
			return new ControlBoundsSetter(changeServ);
		}
		public static void SetBounds(XRControl control, RectangleF rect, IComponentChangeService changeServ) {
			CreataInstance(control.GetType(), changeServ).SetBounds(control, rect);
		}
		public static void SetSpecifiedBounds(XRControl control, RectangleF rect, IComponentChangeService changeServ) {
			CreataInstance(control.GetType(), changeServ).SetSpecifiedBounds(control, rect);
		}
		public static void SetProportionalBounds(XRControl control, RectangleF rect, IComponentChangeService changeServ) {
			CreataInstance(control.GetType(), changeServ).SetProportionalBounds(control, rect);
		}
		protected ControlBoundsSetter(IComponentChangeService changeServ) : base(changeServ) {
		}
		public virtual void SetBounds(XRControl control, RectangleF rect) {
			RaiseComponentChanging(control, XRComponentPropertyNames.Size);
			RaiseComponentChanging(control, XRComponentPropertyNames.Location);
			control.BoundsF = rect;
			RaiseComponentChanged(control, null, null, null);
		}
		public virtual void SetSpecifiedBounds(XRControl control, RectangleF rect) { }
		public virtual void SetProportionalBounds(XRControl control, RectangleF rect) { }
	}
	class CellBoundsSetter : ControlBoundsSetter {
		public CellBoundsSetter(IComponentChangeService changeServ)
			: base(changeServ) {
		}
		public override void SetProportionalBounds(XRControl control, RectangleF rect) {
			SetBounds(control, rect, ResizeBehaviour.ProportionalMode);
		}
		public override void SetSpecifiedBounds(XRControl control, RectangleF rect) {
			SetBounds(control, rect, ResizeBehaviour.SpecifiedMode);
		}
		public override void SetBounds(XRControl control, RectangleF rect) {
			SetBounds(control, rect, ResizeBehaviour.DefaultMode);
		}
		void SetBounds(XRControl control, RectangleF rect, ResizeBehaviour resizeMode) {
			XRTableCell cell = (XRTableCell)control;
			Dictionary<XRTableCell, double> weights = new Dictionary<XRTableCell, double>();
			XRTable table = cell.Row != null ? cell.Row.Table : null;
			RectangleF tableBounds = RectangleF.Empty;
			if(table != null) {
				tableBounds = cell.Row.Table.BoundsF;
				RaiseComponentChanging(table, XRComponentPropertyNames.Size);
				RaiseComponentChanging(table, XRComponentPropertyNames.Location);
			}
			EnumurateAllCells(cell, (item) => {
				weights.Add(item, item.Weight);
				RaiseComponentChanging(item, XRComponentPropertyNames.Weight);
			});
			switch(resizeMode) {
				case ResizeBehaviour.ProportionalMode:
					cell.SetProportionalBounds(rect.X, rect.Y, rect.Width, rect.Height, System.Windows.Forms.BoundsSpecified.All);
					break;
				case ResizeBehaviour.SpecifiedMode:
					cell.SetSpecifiedBounds(rect.X, rect.Y, rect.Width, rect.Height, System.Windows.Forms.BoundsSpecified.All);
					break;
				case ResizeBehaviour.DefaultMode :
					cell.BoundsF = rect;
					break;
			}
			if(table != null) {
				RaiseComponentChanged(table, XRComponentPropertyNames.Size, tableBounds.Size, table.SizeF);
				RaiseComponentChanged(table, XRComponentPropertyNames.Location, tableBounds.Location, table.LocationF);
			}
			EnumurateAllCells(cell, (item) => {
				double weight;
				if(weights.TryGetValue(item, out weight))
					RaiseComponentChanged(item, XRComponentPropertyNames.Weight, weight, item.Weight);
			});
			weights.Clear();
		}
		static void EnumurateAllCells(XRTableCell cell, Action1<XRTableCell> action) {
			XRTable table = cell.Row != null ? cell.Row.Table : null;
			if(table == null) {
				action(cell);
				return;
			}
			foreach(XRTableRow row in table.Rows)
				foreach(XRTableCell item in row.Cells)
					action(item);
		}
	}
	class RowBoundsSetter : ControlBoundsSetter {
		public RowBoundsSetter(IComponentChangeService changeServ)
			: base(changeServ) {
		}
		public override void SetSpecifiedBounds(XRControl control, RectangleF rect) {
			SetBounds(control, rect, ResizeBehaviour.SpecifiedMode);
		}
		public override void SetBounds(XRControl control, RectangleF rect) {
			SetBounds(control, rect, ResizeBehaviour.DefaultMode);
		}
		void SetBounds(XRControl control, RectangleF rect, ResizeBehaviour resizeMode) {
			XRTableRow row = (XRTableRow)control;
			if(row.Table != null) {
				RaiseComponentChanging(row.Table, XRComponentPropertyNames.Size);
				RaiseComponentChanging(row.Table, XRComponentPropertyNames.Location);
			}
			RaiseComponentChanging(row, XRComponentPropertyNames.Weight);
			double rowWeight = row.Weight;
			double prevRowWeight = default(double);
			if(row.Previous != null) {
				RaiseComponentChanging(row.Previous, XRComponentPropertyNames.Weight);
				prevRowWeight = row.Previous.Weight;
			}
			double nextRowWeight = default(double);
			if(row.Next != null) {
				RaiseComponentChanging(row.Next, XRComponentPropertyNames.Weight);
				nextRowWeight = row.Next.Weight;
			}
			RectangleF tableBounds = RectangleF.Empty;
			if(row.Table != null) tableBounds = row.Table.BoundsF;
			if(resizeMode == ResizeBehaviour.SpecifiedMode) {
				row.SetSpecifiedBounds(rect.X, rect.Y, rect.Width, rect.Height, System.Windows.Forms.BoundsSpecified.All);
			} else {
				row.BoundsF = rect;
			}
			if(row.Table != null) {
				RaiseComponentChanged(row.Table, XRComponentPropertyNames.Size, tableBounds.Size, row.Table.SizeF);
				RaiseComponentChanged(row.Table, XRComponentPropertyNames.Location, tableBounds.Location, row.Table.LocationF);
			}
			RaiseComponentChanged(row, XRComponentPropertyNames.Weight, rowWeight, row.Weight);
			if(row.Previous != null)
				RaiseComponentChanged(row.Previous, XRComponentPropertyNames.Weight, prevRowWeight, row.Previous.Weight);
			if(row.Next != null)
				RaiseComponentChanged(row.Next, XRComponentPropertyNames.Weight, nextRowWeight, row.Next.Weight);
		}
	}
}
