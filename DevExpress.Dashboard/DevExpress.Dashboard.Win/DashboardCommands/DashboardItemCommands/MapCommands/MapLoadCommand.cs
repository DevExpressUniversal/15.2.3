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

using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.Utils.Commands;
namespace DevExpress.DashboardWin.Commands {
	public class MapDefaultLoadCommandGroup : DashboardItemCommand<MapDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.None; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandMapDefaultShapefileCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandMapDefaultShapefileDescription; } }
		public override string ImageName { get { return "DefaultMap"; } }
		public MapDefaultLoadCommandGroup(DashboardDesigner control)
			: base(control) {
		}
		protected override void ExecuteInternal(ICommandUIState state) {
		}
	}
	public abstract class MapShapefileCommandBase : DashboardItemCommand<MapDashboardItem> {
		protected MapShapefileCommandBase(DashboardDesigner control)
			: base(control) {
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			MapDashboardItem mapDashboardItem = DashboardItem;
			if(mapDashboardItem != null) {
				MapLoadHistoryItem historyItem = GetHistoryItem(mapDashboardItem);
				if(historyItem != null) {
					historyItem.Redo(Control);
					Control.History.Add(historyItem);
				}
			}
		}
		protected abstract MapLoadHistoryItem GetHistoryItem(MapDashboardItem mapDashboardItem);
	}
	public abstract class MapLoadCommandBase : MapShapefileCommandBase {
		protected MapLoadCommandBase(DashboardDesigner control)
			: base(control) {
		}
		protected override MapLoadHistoryItem GetHistoryItem(MapDashboardItem mapDashboardItem) {
			using(OpenFileDialog dialog = new OpenFileDialog()) {
				dialog.Filter = FileFilters.SHP;
				if(dialog.ShowDialog(Control.FindForm()) == DialogResult.OK)
					return GetHistoryItem(mapDashboardItem, dialog.FileName);
			}
			return null;
		}
		protected abstract MapLoadHistoryItem GetHistoryItem(MapDashboardItem mapDashboardItem, string fileName);
	}
	public class MapLoadCommand : MapLoadCommandBase {
		public override DashboardCommandId Id { get { return DashboardCommandId.MapLoad; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandMapLoadCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandMapLoadDescription; } }
		public override string ImageName { get { return "LoadMap"; } }
		public MapLoadCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override MapLoadHistoryItem GetHistoryItem(MapDashboardItem mapDashboardItem, string fileName) {
			return new MapLoadHistoryItem(mapDashboardItem, fileName);
		}
	}
	public class MapImportCommand : MapLoadCommandBase {
		public override DashboardCommandId Id { get { return DashboardCommandId.MapImport; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandMapImportCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandMapImportDescription; } }
		public override string ImageName { get { return "ImportMap"; } }
		public MapImportCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override MapLoadHistoryItem GetHistoryItem(MapDashboardItem mapDashboardItem, string fileName) {
			byte[] shapeData = File.ReadAllBytes(fileName);
			string dbfUrl = fileName.ToLower().Replace(".shp", ".dbf");
			byte[] attributeData = File.Exists(dbfUrl) ? File.ReadAllBytes(dbfUrl) : null;
			return new MapLoadHistoryItem(mapDashboardItem, shapeData, attributeData);
		}
	}
	public class MapDefaultShapefileCommand : DashboardItemCommand<MapDashboardItem> {
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandMapDefaultShapefileCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandMapDefaultShapefileDescription; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.MapDefaultShapefile; } }
		public override string ImageName { get { return "DefaultMap"; } }
		public MapDefaultShapefileCommand(DashboardDesigner designer)
			: base(designer) {
		}
		protected override void ExecuteInternal(ICommandUIState state) {
		}
	}
	public abstract class MapDefaultLoadCommand : MapShapefileCommandBase {
		protected abstract ShapefileArea Area { get; }
		protected MapDefaultLoadCommand(DashboardDesigner designer)
			: base(designer) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			MapDashboardItem mapItem = DashboardItem;
			state.Checked = mapItem != null ? mapItem.Area == Area : false;
		}
		protected override MapLoadHistoryItem GetHistoryItem(MapDashboardItem mapDashboardItem) {
			return new MapLoadHistoryItem(mapDashboardItem, Area);
		}
	}
	public class MapWorldCountriesLoadCommand : MapDefaultLoadCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.MapWorldCountries; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandMapWorldCountriesCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandMapWorldCountriesDescription; } }
		protected override ShapefileArea Area { get { return ShapefileArea.WorldCountries; } }
		public MapWorldCountriesLoadCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class MapEuropeLoadCommand : MapDefaultLoadCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.MapEurope; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandMapEuropeCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandMapEuropeDescription; } }
		protected override ShapefileArea Area { get { return ShapefileArea.Europe; } }
		public MapEuropeLoadCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class MapAsiaLoadCommand : MapDefaultLoadCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.MapAsia; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandMapAsiaCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandMapAsiaDescription; } }
		protected override ShapefileArea Area { get { return ShapefileArea.Asia; } }
		public MapAsiaLoadCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class MapNorthAmericaLoadCommand : MapDefaultLoadCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.MapNorthAmerica; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandMapNorthAmericaCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandMapNorthAmericaDescription; } }
		protected override ShapefileArea Area { get { return ShapefileArea.NorthAmerica; } }
		public MapNorthAmericaLoadCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class MapSouthAmericaLoadCommand : MapDefaultLoadCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.MapSouthAmerica; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandMapSouthAmericaCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandMapSouthAmericaDescription; } }
		protected override ShapefileArea Area { get { return ShapefileArea.SouthAmerica; } }
		public MapSouthAmericaLoadCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class MapAfricaLoadCommand : MapDefaultLoadCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.MapAfrica; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandMapAfricaCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandMapAfricaDescription; } }
		protected override ShapefileArea Area { get { return ShapefileArea.Africa; } }
		public MapAfricaLoadCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class MapUSALoadCommand : MapDefaultLoadCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.MapUSA; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandMapUSACaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandMapUSADescription; } }
		protected override ShapefileArea Area { get { return ShapefileArea.USA; } }
		public MapUSALoadCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class MapCanadaLoadCommand : MapDefaultLoadCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.MapCanada; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandMapCanadaCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandMapCanadaDescription; } }
		protected override ShapefileArea Area { get { return ShapefileArea.Canada; } }
		public MapCanadaLoadCommand(DashboardDesigner control)
			: base(control) {
		}
	}
}
