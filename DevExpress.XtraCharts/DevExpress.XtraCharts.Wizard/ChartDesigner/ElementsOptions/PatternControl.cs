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
using DevExpress.XtraCharts.Design;
using DevExpress.LookAndFeel;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class PatternControl : XtraUserControl, IModelBinded {
		string value;
		Dictionary<Type, Func<object, string[]>> placeholdersFunc;
		DesignerChartElementModelBase model;
		public event EventHandler ValueChanged;
		public string Value {
			get { return value; }
			set {
				if (!string.Equals(value, this.value)) {
					this.value = value;
					OnValueChanged();
				}
			}
		}
		public PatternControl() {
			InitializeComponent();
			button.TextChanged += button_TextChanged;
			placeholdersFunc = new Dictionary<Type, Func<object, string[]>>();
			placeholdersFunc.Add(typeof(AxisLabel), (x) => { return PatternEditorUtils.GetAvailablePlaceholdersForAxisLabel(x as AxisLabel); });
		}
		void button_TextChanged(object sender, EventArgs e) {
			Value = button.Text;
		}
		void OnValueChanged() {
			button.Text = value;
			if (ValueChanged != null)
				ValueChanged(this, new EventArgs());
		}
		Form CreateForm() {
			if (model == null)
				return null;
			object instance = model.ChartElement;
			string[] placeholders = GetPlaceholders(instance);
			if (placeholders == null)
				return null;
			PatternEditorForm form = new PatternEditorForm(Value, placeholders, PatternEditorUtils.CreatePatternValuesSource(instance));
			ISupportLookAndFeel supportLookAndFeel = Form.ActiveForm as ISupportLookAndFeel;
			if (supportLookAndFeel != null)
				form.LookAndFeel.ParentLookAndFeel = supportLookAndFeel.LookAndFeel;
			return form;
		}
		string[] GetPlaceholders(object instance) {
			if (typeof(SeriesLabelBase).IsAssignableFrom(instance.GetType()))
				return PatternEditorUtils.GetAvailablePlaceholdersForLabel(instance as SeriesLabelBase);
			if (placeholdersFunc.ContainsKey(instance.GetType()))
				return placeholdersFunc[instance.GetType()](instance);
			return null;
		}
		private void button_ButtonClick(object sender, XtraEditors.Controls.ButtonPressedEventArgs e) {
			Form form = CreateForm();
			if (form == null)
				return;
			form.ShowDialog();
			if (form.DialogResult == DialogResult.OK)
				Value = ((PatternEditorForm)form).Pattern;
		}
		void IModelBinded.SetModel(DesignerChartElementModelBase model) { this.model = model; }
	}
}
