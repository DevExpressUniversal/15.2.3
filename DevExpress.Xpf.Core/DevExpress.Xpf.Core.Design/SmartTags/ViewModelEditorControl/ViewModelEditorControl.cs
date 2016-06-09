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
using DevExpress.Xpf.Utils;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Design.SmartTags;
using System.Globalization;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public sealed class ViewModelEditorControl : NodeSelectorControlBase<ViewModelEditorMainViewModel, ViewModelEditorMainView>, IViewModelEditorControl {
		#region Dependency Properties
		public static readonly DependencyProperty SolutionDataProviderProperty =
			DependencyProperty.Register("SolutionDataProvider", typeof(INodeSelectorDataProvider), typeof(ViewModelEditorControl), new PropertyMetadata(null,
				(d, e) => ((ViewModelEditorControl)d).OnDataProviderChanged(e)));
		public static readonly DependencyProperty ProjectDataProviderProperty =
			DependencyProperty.Register("ProjectDataProvider", typeof(INodeSelectorDataProvider), typeof(ViewModelEditorControl), new PropertyMetadata(null,
				(d, e) => ((ViewModelEditorControl)d).OnDataProviderChanged(e)));
		#endregion
		public ViewModelEditorControl() {
			InitializeComponent();
		}
		public INodeSelectorDataProvider SolutionDataProvider { get { return (INodeSelectorDataProvider)GetValue(SolutionDataProviderProperty); } set { SetValue(SolutionDataProviderProperty, value); } }
		public INodeSelectorDataProvider ProjectDataProvider { get { return (INodeSelectorDataProvider)GetValue(ProjectDataProviderProperty); } set { SetValue(ProjectDataProviderProperty, value); } }
		protected override ViewModelEditorMainViewModel CreateMainViewModel() { return new ViewModelEditorMainViewModel(this); }
	}
}
