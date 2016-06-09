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

using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Printing.Parameters.Models;
namespace DevExpress.Xpf.Printing.Parameters {
	[TemplatePart(Name = PART_parametersContainer, Type = typeof(ItemsControl))]
	[TemplatePart(Name = PART_submitButton, Type = typeof(Button))]
	[TemplatePart(Name = PART_resetButton, Type = typeof(Button))]
	public class ParametersPanel : Control {
		#region fields and properties
		const string PART_parametersContainer = "PART_parametersContainer";
		const string PART_submitButton = "PART_submitButton";
		const string PART_resetButton = "PART_resetButton";
		public static readonly DependencyProperty ParameterTemplateSelectorProperty =
			DependencyProperty.Register("ParameterTemplateSelector", typeof(DataTemplateSelector), typeof(ParametersPanel), new PropertyMetadata(new ParameterTemplateSelector()));
		public DataTemplateSelector ParameterTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ParameterTemplateSelectorProperty); }
			set { SetValue(ParameterTemplateSelectorProperty, value); }
		}
		public ParametersModel ParametersModel {
			get { return System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) ? null : (ParametersModel)GetValue(ParametersModelProperty); }
			set { SetValue(ParametersModelProperty, value); }
		}
		public static readonly DependencyProperty ParametersModelProperty =
			DependencyProperty.Register("ParametersModel", typeof(ParametersModel), typeof(ParametersPanel), new PropertyMetadata(null));
		#endregion
		#region ctor
		static ParametersPanel() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ParametersPanel), new FrameworkPropertyMetadata(typeof(ParametersPanel)));
		}
		#endregion
	}
}
