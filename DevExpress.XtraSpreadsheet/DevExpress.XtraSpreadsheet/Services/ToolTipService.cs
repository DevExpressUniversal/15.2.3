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
using System.Text;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.XtraSpreadsheet.Mouse;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraBars;
using System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Services {
	public interface IToolTipService {
		ToolTipControlInfo CalculateToolTipInfo(Point point);
	}
}
namespace DevExpress.XtraSpreadsheet.Services.Implementation {
	#region FormulaBarToolTipService
	public class FormulaBarToolTipService : IToolTipService {
		readonly SpreadsheetFormulaBarControl control;
		public FormulaBarToolTipService(SpreadsheetFormulaBarControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public SpreadsheetFormulaBarControl Control { get { return control; } }
		public ToolTipControlInfo CalculateToolTipInfo(Point point) {
			ToolTipControlInfo result = new ToolTipControlInfo();
			result.Object = Control;
			if (!((IToolTipControlClient)Control).ShowToolTips)
				return result;
			result.ToolTipLocation = ToolTipLocation.RightBottom;
			return result;
		}
	}
	#endregion
	#region FormulaBarCellInplaceEditorToolTipService
	public class FormulaBarCellInplaceEditorToolTipService : DevExpress.XtraRichEdit.Services.IToolTipService {
		readonly FormulaBarCellInplaceEditor control;
		public FormulaBarCellInplaceEditorToolTipService(FormulaBarCellInplaceEditor control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public FormulaBarCellInplaceEditor Control { get { return control; } }
		public ToolTipControlInfo CalculateToolTipInfo(Point point) {
			ToolTipControlInfo result = new ToolTipControlInfo();
			result.Object = Control;
			if (!((IToolTipControlClient)Control).ShowToolTips)
				return result;
			result.ToolTipLocation = ToolTipLocation.RightBottom;
			result.ToolTipType = ToolTipType.SuperTip;
			result.SuperTip = CreateSuperTip();
			return result;
		}
		SuperToolTip CreateSuperTip() {
			SuperToolTip superTip = new SuperToolTip();
			ToolTipItem text = new ToolTipItem();
			text.Text = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Tooltip_FormulaBar);
			superTip.Items.Add(text);
			return superTip;
		}
	}
	#endregion
	#region NameBoxToolTipService
	public class NameBoxToolTipService : IToolTipService {
		readonly SpreadsheetNameBoxControl control;
		public NameBoxToolTipService(SpreadsheetNameBoxControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public SpreadsheetNameBoxControl Control { get { return control; } }
		public ToolTipControlInfo CalculateToolTipInfo(Point point) {
			ToolTipControlInfo result = new ToolTipControlInfo();
			result.Object = Control;
			if (!((IToolTipControlClient)Control).ShowToolTips)
				return result;
			result.ToolTipLocation = ToolTipLocation.RightBottom;
			result.ToolTipType = ToolTipType.SuperTip;
			result.SuperTip = CreateSuperTip();
			return result;
		}
		SuperToolTip CreateSuperTip() {
			SuperToolTip superTip = new SuperToolTip();
			ToolTipItem text = new ToolTipItem();
			text.Text = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Tooltip_NameBox);
			superTip.Items.Add(text);
			return superTip;
		}
	}
	#endregion
	#region FormulaBarExpandButtonToolTipService
	public class FormulaBarExpandButtonToolTipService : IToolTipService {
		readonly FormulaBarExpandButton control;
		public FormulaBarExpandButtonToolTipService(FormulaBarExpandButton control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public FormulaBarExpandButton Control { get { return control; } }
		public ToolTipControlInfo CalculateToolTipInfo(Point point) {
			ToolTipControlInfo result = new ToolTipControlInfo();
			result.Object = Control;
			if (!((IToolTipControlClient)Control).ShowToolTips)
				return result;
			result.ToolTipLocation = ToolTipLocation.RightBottom;
			result.ToolTipType = ToolTipType.SuperTip;
			result.SuperTip = CreateSuperTip();
			return result;
		}
		SuperToolTip CreateSuperTip() {
			SuperToolTip superTip = new SuperToolTip();
			ToolTipItem text = new ToolTipItem();
			text.Text = GetText();
			superTip.Items.Add(text);
			return superTip;
		}
		string GetText() {
			string description = GetDescription();
			BarShortcut shortcut = new BarShortcut(Control.Shortcut);
			if (shortcut.Key == Keys.None)
				return description;
			return String.Format("{0} ({1})", description, shortcut.ToString());
		}
		string GetDescription() {
			if (Control.IsExpand)
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Tooltip_CollapseFormulaBar);
			return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Tooltip_ExpandFormulaBar);
		}
	}
	#endregion
	#region ToolTipService
	public class ToolTipService : IToolTipService {
		readonly SpreadsheetControl control;
		public ToolTipService(SpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public SpreadsheetControl Control { get { return control; } }
		public ToolTipControlInfo CalculateToolTipInfo(Point point) {
			SpreadsheetMouseHandler mouseHandler = Control.InnerControl.MouseHandler as SpreadsheetMouseHandler;
			if (mouseHandler == null)
				return null;
			object activeObject = mouseHandler.ActiveObject;
			if (activeObject == null)
				return null;
			ToolTipControlInfo info = CalculateToolTipInfo(activeObject);
			if (info != null)
				info.ToolTipPosition = CalculateToolTipLocation(Control.GetPhysicalPoint(point));
			return info;
		}
		protected Point CalculateToolTipLocation(Point physicalPoint) {
			Point screenPosition = Control.DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(physicalPoint, Control.DpiX, Control.DpiY);
			return Control.PointToScreen(screenPosition);
		}
		public ToolTipControlInfo CalculateToolTipInfo(object activeObject) {
			IHyperlinkViewInfo hyperlink = activeObject as IHyperlinkViewInfo;
			if (hyperlink == null) {
				IDrawingObject drawing = activeObject as IDrawingObject;
				if (drawing == null)
					return null;
				hyperlink = drawing.DrawingObject;
			}
			if (String.IsNullOrEmpty(hyperlink.TargetUri)) 
				return null;
			ToolTipControlInfo result = new ToolTipControlInfo();
			result.Object = hyperlink;
			if (!((IToolTipControlClient)Control).ShowToolTips)
				return result;
			result.ToolTipLocation = ToolTipLocation.RightTop;
			result.ToolTipType = ToolTipType.SuperTip;
			result.SuperTip = CreateSuperTip(hyperlink);
			return result;
		}
		SuperToolTip CreateSuperTip(IHyperlinkViewInfo hyperlink) {
			SuperToolTip superTip = new SuperToolTip();
			ToolTipItem text = new ToolTipItem();
			text.Text = HyperlinkToolTipCalculatior.GetActualToolTip(hyperlink);
			superTip.Items.Add(text);
			return superTip;
		}
	}
	#endregion
}
