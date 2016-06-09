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
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Designer.Native {
	public class DockTargetItem {
		readonly DesignerChartElementModelBase dockTargetModel;
		readonly IDockTarget dockTarget;
		public DesignerChartElementModelBase DockTargetModel { get { return dockTargetModel; } }
		public DockTargetItem(DesignerChartElementModelBase dockTargetModel) {
			this.dockTargetModel = dockTargetModel;
			this.dockTarget = dockTargetModel != null ? dockTargetModel.ChartElement as IDockTarget : null;
		}
		public override string ToString() {
			return AnnotationHelper.GetDockTargetName(dockTarget);
		}
		public override bool Equals(object obj) {
			DockTargetItem tagretItem = obj as DockTargetItem;
			if (tagretItem == null)
				return false;
			return object.ReferenceEquals(tagretItem.dockTarget, dockTarget);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public class DockTargetModelUITypeEditor : UITypeEditor {
		IWindowsFormsEditorService editorService;
		void OnListBoxClick(object sender, EventArgs e) {
			this.editorService.CloseDropDown();
		}
		void AddDockTargetItem(List<DockTargetItem> dockTargetItems, DesignerChartElementModelBase model) {
			dockTargetItems.Add(new DockTargetItem(model));
		}
		List<DockTargetItem> GetDockTargetItems(DesignerDiagramModel diagram) {
			List<DockTargetItem> dockTargetItems = new List<DockTargetItem>();
			AddDockTargetItem(dockTargetItems, null);
			XYDiagram2DModel xyDiagramModel = diagram as XYDiagram2DModel;
			if (xyDiagramModel != null) {
				AddDockTargetItem(dockTargetItems, xyDiagramModel.DefaultPane);
				foreach (XYDiagramPaneModel additionalPane in xyDiagramModel.Panes)
					AddDockTargetItem(dockTargetItems, additionalPane);
			}
			return dockTargetItems;
		}
		void InitializeListBox(ListBoxControl listBox, List<DockTargetItem> items, object selectedValue) {
			DockTargetItem selectedItem = new DockTargetItem(selectedValue as DesignerChartElementModelBase);
			foreach (DockTargetItem item in items) {
				listBox.Items.Add(item);
				if (selectedItem.Equals(item))
					listBox.SelectedItem = item;
			}
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			this.editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			DesignerChartElementModelBase model = (DesignerChartElementModelBase)context.Instance;
			DesignerChartModel chartModel = model.FindParent<DesignerChartModel>();
			ListBoxControl listBox = new ListBoxControl();
			listBox.Click += OnListBoxClick;
			InitializeListBox(listBox, GetDockTargetItems(chartModel.Diagram), value);
			this.editorService.DropDownControl(listBox);
			return ((DockTargetItem)listBox.SelectedItem).DockTargetModel;
		}
	}
	public class DockTargetModelTypeConverter : TypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string)) {
				DesignerChartElementModelBase model = value as DesignerChartElementModelBase;
				IDockTarget dockTarget = model != null ? model.ChartElement as IDockTarget : null;
				return AnnotationHelper.GetDockTargetName(dockTarget);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
