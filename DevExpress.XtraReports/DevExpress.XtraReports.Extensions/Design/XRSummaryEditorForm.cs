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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native.Summary;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraReports.Design {
	public class XRSummaryEditorForm : SummaryEditorForm {
		XRBinding binding;
		public XRBinding Binding { get { return binding; } }
		public SummaryFunc Func {
			get {
				TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(SummaryFunc));
				return (SummaryFunc)typeConverter.ConvertFromString((string)cbSummaryFunction.EditValue);
			}
		}
		public SummaryRunning Running {
			get { return (SummaryRunning)radioGroup1.EditValue; }
		}
		protected override object CalcSummaryResult(ArrayList values) {
			return SummaryHelper.CalcResult(this.Func, values);
		}
		protected XRSummaryEditorForm() { 
		}
		public XRSummaryEditorForm(XRLabel label, IServiceProvider serviceProvider)
			: base(DesignBindingHelper.CreateDesignBinding(label.DataBindings["Text"]), label.Summary.Func, typeof(SummaryFunc), label.Summary.FormatString, label.Summary.IgnoreNullValues, label.Summary.Running, serviceProvider) {
			this.binding = label.DataBindings["Text"];
			PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(XRControl))[XRComponentPropertyNames.DataBindings];
			cbBoundField.Enabled = property != null && property.SerializationVisibility != DesignerSerializationVisibility.Hidden;
		}
		protected override void InitializeRunningSummaryRadioGroup(System.ComponentModel.ComponentResourceManager resources) {
			this.radioGroup1.Properties.Items.AddRange(new RadioGroupItem[] {
																				new RadioGroupItem(SummaryRunning.None, (string)(resources.GetObject("radioGroup1.Item0.Description"))),
																				new RadioGroupItem(SummaryRunning.Page, (string)(resources.GetObject("radioGroup1.Item1.Description"))),
																				new RadioGroupItem(SummaryRunning.Group, (string)(resources.GetObject("radioGroup1.Item2.Description"))),
																				new RadioGroupItem(SummaryRunning.Report, (string)(resources.GetObject("radioGroup1.Item3.Description")))});
		}
		protected override FormatStringEditorForm GetFormatStringEditorForm(string formatString, IServiceProvider serviceProvider) {
			return new XRFormatStringEditorForm(formatString, serviceProvider);
		}
	}
}
