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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports.UI;
using DevExpress.Data.Browsing.Design;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.Xpf.Reports.UserDesigner.Layout.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtension;
namespace DevExpress.Xpf.Reports.UserDesigner.FieldList.Native {
	public class FieldListNodeXRTool : XRToolBase {
		readonly FieldListNodeBase<XRDiagramControl> fieldListNode;
		readonly Size defaultItemSize;
		public FieldListNodeXRTool(FieldListNodeBase<XRDiagramControl> fieldListNode)
			: base(fieldListNode.Owner) {
			this.fieldListNode = fieldListNode;
			defaultItemSize = IsListNode(fieldListNode) ? new Size(fieldListNode.Owner.PageSize.Width - fieldListNode.Owner.RightPadding, XRControlModelBase.DefaultSize.Height)
				: XRControlModelBase.DefaultSize;
		}
		public override Size DefaultItemSize { get { return defaultItemSize; } }
		public override string ToolName { get { return fieldListNode.DisplayName; } }
		public override string ToolId { get { return "FieldListNodeXRTool"; } }
		protected override DiagramItem CreateItemOverride(XRDiagramControl diagram) {
			XRControl xrControl;
			if(IsListNode(fieldListNode)) {
				xrControl = GetXRTable(fieldListNode);
			} else {
				xrControl = fieldListNode.TypeSpecifics == TypeSpecifics.Bool ? new XRCheckBox() : (XRControl)new XRLabel();
				xrControl.SetDataBindingAndInvalidate(fieldListNode.BindingData.GetXRBinding(ExpressionHelper.GetPropertyName((XRLabel x) => x.Text), string.Empty));
			}
			return diagram.ItemFactory(xrControl);
		}
		XRTable GetXRTable(FieldListNodeBase<XRDiagramControl> sourceNode) {
			var table = new XRTable();
			table.WidthF = BoundsConverter.ToFloat(defaultItemSize.Width, table.Dpi);
			table.HeightF = BoundsConverter.ToFloat(XRControlModelBase.DefaultSize.Height, table.Dpi);
			var tableRow = new XRTableRow() { WidthF = table.WidthF, HeightF = table.HeightF };
			foreach(var fieldListNode in sourceNode.Children) {
				if(IsListNode(fieldListNode)) continue;
				var tableCell = new XRTableCell();
				tableCell.SetDataBindingAndInvalidate(fieldListNode.BindingData.GetXRBinding(ExpressionHelper.GetPropertyName((XRTableCell x) => x.Text), string.Empty));
				tableRow.Controls.Add(tableCell);
			}
			table.Controls.Add(tableRow);
			return table;
		}
		bool IsListNode(FieldListNodeBase<XRDiagramControl> fieldListNode) {
		   return fieldListNode.TypeSpecifics == TypeSpecifics.ListSource || fieldListNode.TypeSpecifics == TypeSpecifics.List || fieldListNode.TypeSpecifics == TypeSpecifics.ListOfParameters;
		}
	}
}
