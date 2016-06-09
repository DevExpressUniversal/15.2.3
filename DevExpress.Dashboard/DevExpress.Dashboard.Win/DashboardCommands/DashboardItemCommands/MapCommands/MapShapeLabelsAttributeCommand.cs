#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.Utils.Commands;
namespace DevExpress.DashboardWin.Commands {
	public abstract class MapShapeLabelsAttributeCommandBase : DashboardItemCommand<MapDashboardItem> {
		public override string ImageName { get { return "ShapeLabels"; } }
		protected MapShapeLabelsAttributeCommandBase(DashboardDesigner control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = Control.SelectedDashboardItem is MapDashboardItem;
		}
		protected abstract Form CreateCommandForm(DashboardDesigner designer, MapDashboardItem mapItem);
		protected override void ExecuteInternal(ICommandUIState state) {
			DashboardDesigner designer = Control;
			MapDashboardItem mapItem = designer.SelectedDashboardItem as MapDashboardItem;
			if(mapItem != null) {
				using(Form form = CreateCommandForm(designer, mapItem)) {
					form.ShowDialog(designer.FindForm());
				}
			}
		}
	}
	public class MapShapeTitleAttributeCommand : MapShapeLabelsAttributeCommandBase {
		public override DashboardCommandId Id { get { return DashboardCommandId.MapShapeTitleAttributeCommand; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandMapShapeTitleAttributeCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandMapShapeTitleAttributeDescription; } }
		public MapShapeTitleAttributeCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override Form CreateCommandForm(DashboardDesigner designer, MapDashboardItem mapItem) {
			return new MapTitleForm(designer, mapItem);
		}
	}
	public class ChoroplethMapShapeLabelsAttributeCommand : MapShapeLabelsAttributeCommandBase {
		public override DashboardCommandId Id { get { return DashboardCommandId.ChoroplethMapShapeLabelsAttributeCommand; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandChoroplethMapShapeLabelsAttributeCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandChoroplethMapShapeLabelsAttributeDescription; } }
		public ChoroplethMapShapeLabelsAttributeCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override Form CreateCommandForm(DashboardDesigner designer, MapDashboardItem mapItem) {
			return new ChoroplethMapTitleForm(designer, (ChoroplethMapDashboardItem)mapItem);
		}
	}
}
