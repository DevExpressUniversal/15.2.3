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

using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public class SetVisibilityInPanesParameter {
		readonly XYDiagramPaneBase pane;
		readonly bool visible;
		public XYDiagramPaneBase Pane { get { return pane; } }
		public bool Visible { get { return visible; } }
		public SetVisibilityInPanesParameter(XYDiagramPaneBase pane, bool visible) {
			this.pane = pane;
			this.visible = visible;
		}
	}
	public class SetVisibilityInPanesCommand : ChartCommandBase {
		readonly Axis2D axis;
		public SetVisibilityInPanesCommand(CommandManager commandManager, Axis2D axis)
			: base(commandManager) {
			this.axis = axis;
		}
		void RefreshChart() {
			((IChartElementWizardAccess)axis).RaiseControlChanged();
		}
		public override bool CanExecute(object parameter) {
			return true;
		}
		public override HistoryItem ExecuteInternal(object parameter) {
			SetVisibilityInPanesParameter visibilityInPanes = parameter as SetVisibilityInPanesParameter;
			if (visibilityInPanes != null) {
				XYDiagramPaneBase pane = visibilityInPanes.Pane;
				bool oldVisibility = (bool)axis.VisibilityInPanes[pane];
				bool newVisibility = visibilityInPanes.Visible;
				axis.VisibilityInPanes[pane] = newVisibility;
				return new HistoryItem(this, oldVisibility, newVisibility, pane);
			}
			return null;
		}
		public override void UndoInternal(HistoryItem historyItem) {
			XYDiagramPaneBase pane = (XYDiagramPaneBase)historyItem.Parameter;
			axis.VisibilityInPanes[pane] = (bool)historyItem.OldValue;
			RefreshChart();
		}
		public override void RedoInternal(HistoryItem historyItem) {
			XYDiagramPaneBase pane = (XYDiagramPaneBase)historyItem.Parameter;
			axis.VisibilityInPanes[pane] = (bool)historyItem.NewValue;
			RefreshChart();
		}
	}
}
