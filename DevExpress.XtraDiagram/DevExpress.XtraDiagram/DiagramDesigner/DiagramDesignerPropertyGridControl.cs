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
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraDiagram.Utils;
using DevExpress.Diagram.Core.Localization;
namespace DevExpress.XtraDiagram.Designer {
	[ToolboxItem(false)]
	public partial class DiagramDesignerPropertyGridControl : XtraUserControl {
		public DiagramDesignerPropertyGridControl() {
			InitializeComponent();
			InitializeOptionsPanel();
		}
		void InitializeOptionsPanel() {
			this.biAlphabeticalView.Tag = ViewType.Alphabetical;
			this.biCategorizedView.Tag = ViewType.Categorized;
			this.biAlphabeticalView.SuperTip = BarUtils.CreateTooltip(GetAlphabeticalViewItemHint());
			this.biCategorizedView.SuperTip = BarUtils.CreateTooltip(GetCategorizedViewItemHint());
			this.biAlphabeticalView.Glyph = GetAlphabeticalViewImage();
			this.biCategorizedView.Glyph = GetCategorizedViewImage();
		}
		Image GetAlphabeticalViewImage() {
			return ImageUtils.LoadImage("AlphabeticalView_16x16.png");
		}
		string GetAlphabeticalViewItemHint() { return DiagramControlLocalizer.Active.GetLocalizedString(DiagramControlStringId.PropertyGridView_Alphabetical); }
		Image GetCategorizedViewImage() {
			return ImageUtils.LoadImage("CategorizedView_16x16.png");
		}
		string GetCategorizedViewItemHint() { return DiagramControlLocalizer.Active.GetLocalizedString(DiagramControlStringId.PropertyGridView_Categorized); }
		void OnViewTypeChanged(object sender, ItemClickEventArgs e) {
			BarButtonItem item = (BarButtonItem)e.Item;
			if(item.Down) {
				UpdateViewType((ViewType)item.Tag);
			}
		}
		BaseEdit activeEditor = null;
		void OnShownEditor(object sender, EventArgs e) {
			BaseEdit edit = PropertyGrid.ActiveEditor;
			if(edit != null) {
				edit.EditValueChanged -= OnEditorEditValueChanged;
				edit.EditValueChanged += OnEditorEditValueChanged;
				this.activeEditor = edit;
			}
		}
		void OnHiddenEditor(object sender, EventArgs e) {
			BaseEdit edit = this.activeEditor;
			if(edit != null) {
				edit.EditValueChanged -= OnEditorEditValueChanged;
			}
			this.activeEditor = null;
		}
		protected BaseEdit ActiveEditor { get { return activeEditor; } }
		void OnEditorEditValueChanged(object sender, EventArgs e) {
			if(ActiveEditor is ComboBoxEdit) PropertyGrid.PostEditor();
		}
		void UpdateViewType(ViewType viewType) {
			PropertyGrid.OptionsView.ShowRootCategories = (viewType == ViewType.Categorized);
		}
		public PropertyGridControl PropertyGrid { get { return propertyGrid; } }
		#region View Type
		public enum ViewType {
			Categorized,
			Alphabetical
		}
		#endregion
	}
}
