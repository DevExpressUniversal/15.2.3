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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Diagram.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Toolbox {
	public class ReportDesignerToolboxControl : Control {
		public static readonly DependencyProperty ToolsProperty;
		public static readonly DependencyProperty SelectedToolProperty;
		static ReportDesignerToolboxControl() {
			DependencyPropertyRegistrator<ReportDesignerToolboxControl>.New()
				.Register(d => d.Tools, out ToolsProperty, null)
				.Register(d => d.SelectedTool, out SelectedToolProperty, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.OverrideDefaultStyleKey();
		}
		public IEnumerable<ToolViewModel> Tools {
			get { return (IEnumerable<ToolViewModel>)GetValue(ToolsProperty); }
			set { SetValue(ToolsProperty, value); }
		}
		public ToolViewModel SelectedTool {
			get { return (ToolViewModel)GetValue(SelectedToolProperty); }
			set { SetValue(SelectedToolProperty, value); }
		}
	}
}
