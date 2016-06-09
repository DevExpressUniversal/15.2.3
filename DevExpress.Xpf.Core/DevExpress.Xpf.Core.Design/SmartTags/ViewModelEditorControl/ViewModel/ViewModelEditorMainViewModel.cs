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

namespace DevExpress.Xpf.Core.Design.SmartTags {
	public sealed class ViewModelEditorMainViewModel : NodeSelectorMainViewModelBase {
		bool showTypesFromActiveProjectOnly = true;
		public ViewModelEditorMainViewModel(IViewModelEditorControl control)
			: base(control) {
			control.DataProviderChanged += OnControlDataProviderChanged;
			OnControlDataProviderChanged(control, new ThePropertyChangedEventArgs<INodeSelectorDataProvider>(null, control.SolutionDataProvider));
			OnControlDataProviderChanged(control, new ThePropertyChangedEventArgs<INodeSelectorDataProvider>(null, control.ProjectDataProvider));
		}
		public IViewModelEditorControl Control { get { return (IViewModelEditorControl)ControlBase; } }
		public bool ShowTypesFromActiveProjectOnly {
			get { return showTypesFromActiveProjectOnly; }
			set { SetProperty(ref showTypesFromActiveProjectOnly, value, () => ShowTypesFromActiveProjectOnly, OnCurrentDataProviderChanged); }
		}
		protected override INodeSelectorDataProvider GetDataProvider() { return ShowTypesFromActiveProjectOnly ? Control.ProjectDataProvider : Control.SolutionDataProvider; }
	}
	public interface IViewModelEditorControl : INodeSelectorControlBase {
		INodeSelectorDataProvider SolutionDataProvider { get; }
		INodeSelectorDataProvider ProjectDataProvider { get; }
	}
}
