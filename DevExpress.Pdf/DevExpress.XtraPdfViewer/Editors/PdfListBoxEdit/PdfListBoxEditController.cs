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

using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfListBoxController : PdfEditorController {
		readonly PdfListBoxEdit listBox;
		readonly PdfListBoxSettings settings;
		public new PdfListBoxSettings Settings { get { return settings; } }
		protected override BaseControl Control { get { return listBox; } }
		protected override object Value {
			get {
				List<string> selectedValues = new List<string>();
				IList<PdfOptionsFormFieldOption> values = settings.Values;
				if (values != null) {
					int count = values.Count;
					foreach (int i in listBox.SelectedIndices)
						if (i >= 0 && i < count)
							selectedValues.Add(values[i].Text);
				}
				return new PdfListBoxEditValue(listBox.TopIndex, selectedValues);
			}
		}
		public PdfListBoxController(PdfViewer viewer, PdfListBoxSettings settings, IPdfViewerValueEditingCallBack callback)
			: base(viewer, settings, callback) {
			this.settings = settings;
			listBox = new PdfListBoxEdit(this);
		}
		public override void SetUp() {
			ListBoxItemCollection items = listBox.Items;
			items.BeginUpdate();
			IList<PdfOptionsFormFieldOption> values = settings.Values;
			if (values != null) {
				foreach (PdfOptionsFormFieldOption opt in values)
					items.Add(opt.ExportText);
				items.EndUpdate();
				if (settings.Multiselect) {
					listBox.SelectionMode = SelectionMode.MultiExtended;
					foreach (int idx in settings.SelectedIndices)
						listBox.SetSelected(idx, true);
				}
				else {
					listBox.SelectionMode = SelectionMode.One;
					if (settings.SelectedIndices.Count != 0)
						listBox.SelectedIndex = settings.SelectedIndices[0];
				}
			}
			base.SetUp();
			listBox.TopIndex = settings.TopIndex;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing)
				listBox.Dispose();
		}
	}
}
