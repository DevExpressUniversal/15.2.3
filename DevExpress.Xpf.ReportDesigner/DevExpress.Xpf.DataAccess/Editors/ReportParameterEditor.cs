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

using DevExpress.Data;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.DataAccess.DataSourceWizard;
using DevExpress.Xpf.DataAccess.Native;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraReports.Parameters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace DevExpress.Xpf.DataAccess.Editors {	
	public class ReportParameterEditor : Control {
		public static readonly DependencyProperty ParameterTypeProperty;
		public static readonly DependencyProperty ReportParameterProperty;
		static readonly DependencyPropertyKey ReportParameterPropertyKey;
		public static readonly DependencyProperty ReportParametersProperty;
		static ReportParameterEditor() {
			DependencyPropertyRegistrator<ReportParameterEditor>.New()
				.Register(d => d.ParameterType, out ParameterTypeProperty, null, d => d.OnParameterTypeChanged())
				.Register(d => d.ReportParameters, out ReportParametersProperty, null)
				.RegisterReadOnly(d => d.ReportParameter, out ReportParameterPropertyKey, out ReportParameterProperty, null)
				.OverrideDefaultStyleKey()
			;
		}
		public ReportParameterEditor() {
			this.saveCommand = DelegateCommandFactory.Create(() => ReportParameters.Add(ReportParameter));
		}
		readonly ICommand saveCommand;
		public ICommand SaveCommand { get { return saveCommand; } }
		public ICollection<IParameter> ReportParameters {
			get { return (ICollection<IParameter>)GetValue(ReportParametersProperty); }
			set { SetValue(ReportParametersProperty, value); }
		}
		public Type ParameterType {
			get { return (Type)GetValue(ParameterTypeProperty); }
			set { SetValue(ParameterTypeProperty, value); }
		}
		public IParameter ReportParameter {
			get { return (IParameter)GetValue(ReportParameterProperty); }
			private set { SetValue(ReportParameterPropertyKey, value); }
		}
		void OnParameterTypeChanged() {
			if(ParameterType != null)
				ReportParameter = new ParameterWithoutLookUpSettings { Name = "parameter", Type = ParameterType };
		}
	}
	public class ParameterWithoutLookUpSettings : Parameter {
		[Browsable(false)]
		public override LookUpSettings LookUpSettings { get; set; }
	}
}
