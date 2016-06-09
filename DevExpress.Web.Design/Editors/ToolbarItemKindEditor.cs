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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;
using DevExpress.XtraReports.Web.Native;
namespace DevExpress.XtraReports.Web.Design {
	public abstract class ToolbarItemKindEditor : ObjectSelectorEditor {
		protected ReportToolbarItemKind[] standardValues;
		protected ToolbarItemKindEditor()
			: base(true) {
		}
		protected abstract ReportToolbarItemKind[] GetStandardValues();
		protected override void FillTreeWithData(ObjectSelectorEditor.Selector selector,
			ITypeDescriptorContext context, IServiceProvider provider) {
			selector.ShowLines = false;
			selector.ShowRootLines = false;
			selector.ShowPlusMinus = false;
			selector.Clear();
			this.standardValues = GetStandardValues();
			for(int i = 0; i < this.standardValues.Length; i++) {
				ReportToolbarItemKind value = this.standardValues[i];
				SelectorNode node = selector.AddNode(value.ToString(), value, null);
				if(Object.Equals(this.currValue, value))
					selector.SelectedNode = node;
			}
			selector.Sorted = true;
			selector.Height = (selector.Height / selector.ItemHeight + 1) * selector.ItemHeight;
		}
	}
	public class ButtonItemKindEditor : ToolbarItemKindEditor {
		protected override ReportToolbarItemKind[] GetStandardValues() {
			return new ReportToolbarItemKind[] { ReportToolbarItemKind.Custom, ReportToolbarItemKind.Search, ReportToolbarItemKind.PrintReport, 
				ReportToolbarItemKind.PrintPage, ReportToolbarItemKind.FirstPage,ReportToolbarItemKind.PreviousPage, ReportToolbarItemKind.NextPage,
				ReportToolbarItemKind.LastPage, ReportToolbarItemKind.SaveToDisk, ReportToolbarItemKind.SaveToWindow };
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null || context.Instance == null)
				return null;
			object newValue = base.EditValue(context, provider, value);
			ReportToolbarButton toolbarButton = context.Instance as ReportToolbarButton;
			if(toolbarButton != null) {
				toolbarButton.ToolTip = string.Empty;
				toolbarButton.ImageUrl = string.Empty;
				toolbarButton.ImageUrlDisabled = string.Empty;
			}
			return newValue;
		}
	}
	public class ComboBoxItemKindEditor : ToolbarItemKindEditor {
		protected override ReportToolbarItemKind[] GetStandardValues() {
			return new ReportToolbarItemKind[] { ReportToolbarItemKind.Custom, ReportToolbarItemKind.PageNumber, ReportToolbarItemKind.SaveFormat };
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null || context.Instance == null)
				return null;
			object newValue = base.EditValue(context, provider, value);
			ReportToolbarComboBox toolbarComboBox = context.Instance as ReportToolbarComboBox;
			if(toolbarComboBox != null) {
				if(Object.Equals(newValue, ReportToolbarItemKind.SaveFormat)) {
					toolbarComboBox.FillElements(ReportToolbarResources.ExportValues);
				} else {
					toolbarComboBox.Elements.Clear();
				}
			}
			return newValue;
		}
	}
	public class TextBoxItemKindEditor : ToolbarItemKindEditor {
		protected override ReportToolbarItemKind[] GetStandardValues() {
			return new ReportToolbarItemKind[] { ReportToolbarItemKind.Custom, ReportToolbarItemKind.PageCount };
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null || context.Instance == null)
				return null;
			object newValue = base.EditValue(context, provider, value);
			ReportToolbarTextBox toolbarTextBox = context.Instance as ReportToolbarTextBox;
			if(toolbarTextBox != null && Object.Equals(newValue, ReportToolbarItemKind.PageCount)) {
				toolbarTextBox.IsReadOnly = true;
				toolbarTextBox.Text = String.Empty;
			}
			return newValue;
		}
	}
	public class LabelItemKindEditor : ToolbarItemKindEditor {
		protected override ReportToolbarItemKind[] GetStandardValues() {
			return new ReportToolbarItemKind[] { ReportToolbarItemKind.Custom, ReportToolbarItemKind.PageLabel, ReportToolbarItemKind.OfLabel};
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context == null || context.Instance == null)
				return null;
			object newValue = base.EditValue(context, provider, value);
			ReportToolbarLabel reportToolbarLabel = context.Instance as ReportToolbarLabel;
			if(reportToolbarLabel != null && !Object.Equals(newValue, ReportToolbarItemKind.Custom))
				reportToolbarLabel.Text = String.Empty;
			return newValue;
		}
	}
}
