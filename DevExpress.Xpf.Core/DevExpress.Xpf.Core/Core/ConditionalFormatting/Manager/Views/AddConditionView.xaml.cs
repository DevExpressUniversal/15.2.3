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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.ExpressionEditor;
using ConditionFormat = DevExpress.Xpf.Core.ConditionalFormatting.Format;
using DevExpress.Xpf.Core.ConditionalFormatting;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public partial class AddConditionView : UserControl {
		public AddConditionView() {
			InitializeComponent();
		}
	}
	public class FormatEditorOwnerTemplateSelector : DataTemplateSelector {
		public DataTemplate ContainTemplate { get; set; }
		public DataTemplate TopBottomTemplate { get; set; }
		public DataTemplate AboveBelowTemplate { get; set; }
		public DataTemplate FormulaTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			if(item is ContainViewModel)
				return ContainTemplate;
			if(item is AboveBelowViewModel)
				return AboveBelowTemplate;
			if(item is TopBottomViewModel)
				return TopBottomTemplate;
			if(item is FormulaViewModel)
				return FormulaTemplate;
			return null;
		}
	}
}
