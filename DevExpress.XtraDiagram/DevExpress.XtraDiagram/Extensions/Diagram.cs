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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.Diagram.Core.Localization;
using DevExpress.XtraDiagram.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.XtraDiagram.Extensions {
	public static class XtraDiagramExtensions {
		public static DiagramItem PrimarySelection(this DiagramControl diagram) {
			return (DiagramItem)diagram.Selection().PrimarySelection;
		}
		public static void ForEachSelectedItem(this DiagramControl diagram, Action<DiagramItem> handler) {
			foreach(DiagramItem item in diagram.SelectedItems()) {
				handler(item);
			}
		}
		public static bool ContainsSelection(this DiagramControl diagram) {
			return diagram.Selection().SelectedItems.Count > 0;
		}
		public static bool IsMultipleSelection(this DiagramControl diagram) {
			return diagram.Selection().SelectedItems.Count > 1;
		}
		public static DiagramShape CreateShape(this DiagramControl diagram, ShapeDescription shapeDesc, bool focusDiagram = true) {
			Rectangle rect = RectangleUtils.FitRect(diagram.Page.Size.CreateRect(), shapeDesc.DefaultSize.Width, shapeDesc.DefaultSize.Height);
			return CreateShape(diagram, shapeDesc, rect, focusDiagram);
		}
		public static DiagramShape CreateShape(this DiagramControl diagram, ShapeDescription shapeDesc, Rectangle bounds, bool focusDiagram = true) {
			DiagramShape newShape = new DiagramShape(shapeDesc, bounds);
			diagram.Items.Add(newShape);
			diagram.SelectItem(newShape);
			if(focusDiagram) diagram.Focus();
			return newShape;
		}
		public static void ConfirmSaveChanges(this DiagramControl diagram, Form ownerForm, CancelEventArgs e = null) {
			DialogResult dialogResult = ShowSaveChangesMessage(ownerForm);
			if(dialogResult == DialogResult.Cancel) {
				if(e != null) e.Cancel = true;
			}
			else {
				ownerForm.DialogResult = (dialogResult == DialogResult.Yes) ? DialogResult.OK : DialogResult.No;
			}
		}
		static DialogResult ShowSaveChangesMessage(Form ownerForm) {
			return XtraMessageBox.Show(ownerForm, DiagramControlLocalizer.Active.GetLocalizedString(DiagramControlStringId.DiagramDesigner_SaveChangesConfirmation), DiagramControlLocalizer.Active.GetLocalizedString(DiagramControlStringId.DiagramDesigner), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
		}
	}
}
