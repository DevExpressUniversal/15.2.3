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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class PaneControl : ModelSelectControl {
		public XYDiagramPaneBaseModel Pane {
			get {
				return Element as XYDiagramPaneBaseModel;
			}
			set {
				Element = value as XYDiagramPaneBaseModel;
			}
		}
		public PaneControl()
			: base() {
			InitializeComponent();
		}
		public override void UpdateComboBox() {
			if (DiagramModel == null)
				return;
			BoxEdit.Properties.Items.Clear();
			ModelSelectControl.NamedModelPresentation currentPresentation = null;
			ModelSelectControl.NamedModelPresentation presentation = null;
			currentPresentation = AddPane(DiagramModel.DefaultPane);
			if (presentation != null)
				currentPresentation = presentation;
			foreach (XYDiagramPaneBaseModel pane in DiagramModel.Panes) {
				presentation = AddPane(pane);
				if (presentation != null)
					currentPresentation = presentation;
			}
			if (currentPresentation != null)
				BoxEdit.SelectedItem = currentPresentation;
		}
		ModelSelectControl.NamedModelPresentation AddPane(XYDiagramPaneBaseModel pane) {
			NamedModelPresentation presentation = new NamedModelPresentation(pane.Name, pane);
			BoxEdit.Properties.Items.Add(presentation);
			if (pane.Equals(Element))
				return presentation;
			return null;
		}
	}
}
