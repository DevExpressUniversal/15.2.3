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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Server;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraPrinting;
using DevExpress.DashboardExport;
namespace DevExpress.DashboardWeb {
	static class RequestArgsParser {
		public static InitializeSessionArgs ParseInitalizeSessionArgs(Hashtable data) {
			string dashboardId = (string)data["DashboardId"];
			int requestMarker = Convert.ToInt32(data["RequestMarker"]);
			SessionSettings settings = new SessionSettings { };
			Hashtable settingsHash = (Hashtable)data["Settings"];
			if(settingsHash != null) {
				settings.CalculateHiddenTotals = (bool)settingsHash["calculateHiddenTotals"];
			}
			return new InitializeSessionArgs {
				DashboardId = dashboardId,
				RequestMarker = requestMarker,
				Settings = settings
			};
		}
		public static PerformActionArgs ParsePerformActionArgs(Hashtable data) {
			PerformActionArgs args = new PerformActionArgs();
			ParseSessionOperationArgs(args, data);
			ArrayList actionInfoList = (ArrayList)data["Actions"];
			IList<ActionInfo> actions = new List<ActionInfo>();
			foreach(Hashtable actionInfoHash in actionInfoList) {
				ArrayList parametersList = (ArrayList)actionInfoHash["Parameters"];
				object[] parameters = parametersList != null ? parametersList.ToArray() : null;
				ActionInfo actionInfo = new ActionInfo {
					ItemName = (string)actionInfoHash["ItemName"],
					ActionType = (ActionType)Enum.Parse(typeof(ActionType), (string)actionInfoHash["ActionName"], true),
					Parameters = parameters
				};
				actions.Add(actionInfo);
			}
			args.ActionInfo = actions;
			return args;
		}
		public static ReloadDataArgs ParseReloadDataArgs(Hashtable data) {
			ReloadDataArgs args = new ReloadDataArgs();
			ParseSessionOperationArgs(args, data);
			ArrayList parametersList = (ArrayList)data["DashboardParameters"];
			IList<DashboardParameterInfo> parameters = new List<DashboardParameterInfo>();
			if(parametersList != null) {
				foreach(Hashtable actionInfoHash in parametersList) {
					DashboardParameterInfo actionInfo = new DashboardParameterInfo {
						Name = (string)actionInfoHash["Name"],
						Value = actionInfoHash["Value"]
					};
					parameters.Add(actionInfo);
				}
			}
			args.Parameters = parameters;
			return args;
		}
		public static ExportArgs ParseExportArgs(Hashtable data) {
			ExportArgs args = new ExportArgs();
			ParseSessionOperationArgs(args, data);
			Hashtable exportInfoHash = (Hashtable)data["ExportInfo"];
			ExportInfo exportInfo = ParseExportInfo(exportInfoHash);
			MemoryStream stream = new MemoryStream();
			args.ExportInfo = exportInfo;
			args.Stream = stream;
			args.Exporter = DashboardExporter.Instance;
			return args;
		}
		static void ParseSessionOperationArgs(DashboardServiceSessionOperationArgs args, Hashtable data) {
			string sessionId = (string)data["SessionId"];
			int requestMarker = Convert.ToInt32(data["RequestMarker"]);
			object serverContextObj = data["Context"];
			string serverContext = serverContextObj != null ? (string)serverContextObj : null;
			Hashtable clientState = (Hashtable)data["ClientState"];
			args.SessionId = sessionId;
			args.RequestMarker = requestMarker;
			args.Context = serverContext;
			args.ClientState = clientState;
		}
		static ExportInfo ParseExportInfo(Hashtable exportInfoHash) {
			Hashtable clientStateHash = GetValue<Hashtable>(exportInfoHash, "ClientState", null),
				clientSizeHash = GetValue<Hashtable>(clientStateHash, "clientSize", null);
			Dictionary<string, ItemViewerClientState> itemViewersClientState = new Dictionary<string, ItemViewerClientState>();
			DashboardExportFormat format = (DashboardExportFormat)Enum.Parse(typeof(DashboardExportFormat), GetValue<string>(exportInfoHash, "Format", string.Empty), true);
			ExportInfo exportInfo = new ExportInfo() {
				ExportOptions = DashboardExportOptions.Parse(GetValue<string>(exportInfoHash, "ItemType", null),
					GetValue<Hashtable>(exportInfoHash, "DocumentOptions", null)),
				Mode = (DashboardExportMode)Enum.Parse(typeof(DashboardExportMode),
					GetValue<string>(exportInfoHash, "Mode", string.Empty), true),
				ViewerState = new ViewerState {
					Size = new Size(GetValue<int>(clientSizeHash, "width", 0),
						GetValue<int>(clientSizeHash, "height", 0)),
					ItemsState = itemViewersClientState,
					TitleHeight = GetValue<int>(clientStateHash, "titleHeight", 0)
				},
				GroupName = GetValue<string>(exportInfoHash, "GroupName", string.Empty)
			};
			exportInfo.ExportOptions.FileName = (string)exportInfoHash["FileName"];
			exportInfo.ExportOptions.FormatOptions.Format = format;
			if(format == DashboardExportFormat.PDF) {
				exportInfo.ExportOptions.FormatOptions.PdfOptions = new PdfExportOptions();
			}
			foreach(var state in (IEnumerable)clientStateHash["itemsState"]) {
				Hashtable itemStateHash = (Hashtable)state,
					positionHash = GetValue<Hashtable>(itemStateHash, "position", null),
					scrollHash = GetValue<Hashtable>(itemStateHash, "scroll", null),
					virtualSizeHash = (Hashtable)itemStateHash["virtualSize"];
				Point position = new Point(GetValue<int>(positionHash, "left", 0),
					GetValue<int>(positionHash, "top", 0));
				Size size = new Size(GetValue<int>(itemStateHash, "width", 0),
					GetValue<int>(itemStateHash, "height", 0));
				int headerHeight = GetValue<int>(itemStateHash, "headerHeight", 0),
					itemHeight = size.Height - headerHeight,
					scrollBarSize = GetValue<int>(scrollHash, "size", 0);
				ItemViewerClientState clientState = new ItemViewerClientState {
					ViewerArea = new ClientArea { Left = position.X, Top = position.Y + headerHeight, Width = size.Width, Height = itemHeight },
					CaptionArea = new ClientArea { Left = position.X, Top = position.Y, Width = size.Width, Height = headerHeight }
				};
				if(GetValue<bool>(scrollHash, "vertical", false)) {
					clientState.VScrollingState = CreateScrollingState(GetValue<int>(scrollHash, "top", 0),
						GetValue<ArrayList>(scrollHash, "topPath", null),
						GetValue<int>(virtualSizeHash, "height", itemHeight),
						scrollBarSize);
				}
				if(GetValue<bool>(scrollHash, "horizontal", false)) {
					clientState.HScrollingState = CreateScrollingState(GetValue<int>(scrollHash, "left", 0),
						GetValue<ArrayList>(scrollHash, "leftPath", null),
						GetValue<int>(virtualSizeHash, "width", size.Width),
						scrollBarSize);
				}
				Hashtable widthOptionsInfo = GetValue<Hashtable>(itemStateHash, "widthOptionsInfo", null);
				clientState.SpecificState = CreateSpecificState(GetValue<Hashtable>(itemStateHash, "itemMargin", null), GetValue<Hashtable>(itemStateHash, "viewport", null), widthOptionsInfo, 
					GetValue<Hashtable>(itemStateHash, "chartViewport", null));
				itemViewersClientState.Add(GetValue<string>(itemStateHash, "name", string.Empty), clientState);
			}
			return exportInfo;
		}
		static ScrollingState CreateScrollingState(int scrollTop, ArrayList scrollPath, int virtualSize, int scrollBarSize) {
			ScrollingState scrollingState = new ScrollingState {
				VirtualSize = virtualSize,
				ScrollBarSize = scrollBarSize
			};
			if(scrollPath != null) {
				scrollingState.PositionListSourceRow = scrollPath.ToArray();
			} else {
				scrollingState.PositionRatio = (double)scrollTop / virtualSize;
			}
			return scrollingState;
		}
		static Dictionary<string, object> CreateSpecificState(Hashtable itemMarginHash, Hashtable viewportHash, Hashtable widthOptionsInfoHash, Hashtable chartViewport) {
			Dictionary<string, object> specificState = new Dictionary<string, object>();
			specificState.Add("PivotColumnTotalsLocation", ExportPivotColumnTotalsLocation.Far);
			specificState.Add("PivotRowTotalsLocation", ExportPivotRowTotalsLocation.Far);
			if(itemMarginHash != null) {
				specificState.Add("CardMarginX", itemMarginHash["width"]);
				specificState.Add("CardMarginY", itemMarginHash["height"]);
			}
			if(viewportHash != null) {
				specificState.Add("MapViewportState", new MapViewportState {
					BottomLatitude = Helper.ConvertToDouble(viewportHash["BottomLatitude"]),
					TopLatitude = Helper.ConvertToDouble(viewportHash["TopLatitude"]),
					LeftLongitude = Helper.ConvertToDouble(viewportHash["LeftLongitude"]),
					RightLongitude = Helper.ConvertToDouble(viewportHash["RightLongitude"]),
					CenterPointLatitude = Helper.ConvertToDouble(viewportHash["CenterPointLatitude"]),
					CenterPointLongitude = Helper.ConvertToDouble(viewportHash["CenterPointLongitude"])
				});
			}
			if(widthOptionsInfoHash != null) {
				ColumnsWidthOptionsInfo widthOptionsInfo = new ColumnsWidthOptionsInfo();
				widthOptionsInfo.ColumnWidthMode = (GridColumnWidthMode)Enum.Parse(typeof(GridColumnWidthMode), GetValue<string>(widthOptionsInfoHash, "columnWidthMode", String.Empty), true);
				ArrayList columnsInfo = (ArrayList)widthOptionsInfoHash["columnsWidthInfo"];
				foreach(Hashtable columnInfoHash in columnsInfo) {
					widthOptionsInfo.ColumnsInfo.Add(new ColumnWidthOptionsInfo() {
						ActualIndex = GetValue<int>(columnInfoHash, "actualIndex", 0),
						DefaultBestCharacterCount = Helper.ConvertToDouble(columnInfoHash["defaultBestCharacterCount"]),
						ActualWidth = GetValue<int>(columnInfoHash, "actualWidth", 0),
						DisplayMode = (GridColumnDisplayMode)Enum.Parse(typeof(GridColumnDisplayMode), GetValue<string>(columnInfoHash, "displayMode", String.Empty), true),
						FixedWidth = Helper.ConvertToDouble(columnInfoHash["fixedWidth"]),
						InitialWidth = Helper.ConvertToDouble(columnInfoHash["initialWidth"]),
						MinWidth = GetValue<int>(columnInfoHash, "minWidth", 0),
						Weight = Helper.ConvertToDouble(columnInfoHash["weight"]),
						WidthType = (GridColumnFixedWidthType)Enum.Parse(typeof(GridColumnFixedWidthType), GetValue<string>(columnInfoHash, "widthType", String.Empty), true)
					});
				}
				specificState.Add("ColumnsWidthOptionsState", widthOptionsInfo);
			}
			if(chartViewport != null) {
				specificState.Add("ChartAxisXMinValue", GetValue<ArrayList>(chartViewport, "min", null));
				specificState.Add("ChartAxisXMaxValue", GetValue<ArrayList>(chartViewport, "max", null));
			}
			return specificState;
		}
		static TValue GetValue<TValue>(object obj, TValue defaultValue) {
			return obj == null ? defaultValue : (TValue)obj;
		}
		static TValue GetValue<TValue>(Hashtable hashtable, string name, TValue defaultValue) {
			if(hashtable != null) {
				return GetValue<TValue>(hashtable[name], defaultValue);
			} else {
				return defaultValue;
			}
		}
	}
}
