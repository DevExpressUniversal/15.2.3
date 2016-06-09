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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors {
	public class TableCellAppearanceAdapter : IAppearanceFormat {
		private WebControl _control;
		private bool controlIsCalculated = false;
		public TableCellAppearanceAdapter(TableCell cell) {
			this.Cell = cell;
		}
		public TableCell Cell { get; private set; }
		private WebControl control {
			get {
				if(_control == null && !controlIsCalculated && Cell != null) {
					controlIsCalculated = true;
					_control = FindControlInTableExContainer(Cell.Controls[0]);
				}
				return _control;
			}
		}
		private WebControl FindControlInTableExContainer(Control container) {
			if(container != null && container.Controls != null && container.Controls.Count > 0) {
				TableEx table = null;
				if(container.Controls[0] is TableEx) {
					table = container.Controls[0] as TableEx;
				}
				else if(container.Controls[0] is TemplateContainerBase) {
					table = container.Controls[0].Controls[0] as TableEx;
				}
				if(table != null && table.Rows.Count > 0) {
					WebControl Cell = table.Rows[0].Cells.Count == 2 ? table.Rows[0].Cells[1] : table.Rows[0].Cells[0];
					if(Cell != null && Cell.Controls != null && Cell.Controls.Count > 0) {
						return Cell.Controls[0] as WebControl;
					}
				}
			}
			return null;
		}
		protected void SetCell(TableCell newValue) {
			Cell = newValue;
		}
		#region IAppearanceFormat Members
		public FontStyle FontStyle {
			get {
				if(control != null) {
					return RenderHelper.GetFontStyle(control);
				}
				else {
					if(Cell != null) {
						return RenderHelper.GetFontStyle(Cell);
					}
				}
				return FontStyle.Regular;
			}
			set {
				if(Cell != null) {
					RenderHelper.SetFontStyle(Cell, value);
				}
				if(control != null) {
					RenderHelper.SetFontStyle(control, value);
				}
			}
		}
		public Color FontColor {
			get {
				if(control == null) {
					if(Cell != null) {
						return Cell.ForeColor;
					}
					else {
						return Color.Empty;
					}
				}
				return control.ForeColor;
			}
			set {
				if(Cell != null) {
					Cell.ForeColor = value;
				}
				if(control != null) {
					control.ForeColor = value;
				}
			}
		}
		public Color BackColor {
			get {
				if(control == null) {
					if(Cell != null) {
						return Cell.BackColor;
					}
					else {
						return Color.Empty;
					}
				}
				return control.BackColor;
			}
			set {
				if(Cell != null) {
					Cell.BackColor = value;
				}
				if(control != null) {
					control.BackColor = value;
				}
			}
		}
		public void ResetFontStyle() { }
		public void ResetFontColor() { }
		public void ResetBackColor() { }
		#endregion
		#region Obsolete 15.2
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public TableCellAppearanceAdapter(TableCell cell, object data) : this(cell) { }
		#endregion
	}
}
