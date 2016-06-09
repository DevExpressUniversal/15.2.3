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
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Server;
using System.Text;
using DevExpress.DashboardCommon.Native;
using System;
namespace DevExpress.DashboardCommon.Service {
	public class InitializeOperation : DashboardServiceOperation<InitializeSessionArgs> {
		public static string GetInitialContext(SessionSettings settings) {
			return BinarySerializer.Serialize(new DashboardSessionState { DashboardState = new DashboardState(), DataVersion = String.Empty, SessionSettings = settings });
		}
		public static string GetInitialContext(string dashboardId, SessionSettings settings) {
			return BinarySerializer.Serialize(
				new DashboardSessionState {
					DashboardId = dashboardId,
					DashboardState = new DashboardState(),
					DataVersion = String.Empty,
					SessionSettings = settings
				});
		}
		protected override string Context {
			get { return GetInitialContext(Args.DashboardId, Args.Settings); }
		}
		public InitializeOperation(IDashboardServer server, IDashboardServiceAdminHandlers admin, InitializeSessionArgs args)
			: base(server, admin, args) {
		}
		protected override DashboardServerResult RequestSession() {
			return Server.GetSession(null, Args.Settings.SessionTimeout);
		}
		protected override IList<AffectedItemInfo> ExecuteInternal(bool dataBindingApplied, DashboardServiceResult result, DashboardServerResult serverResult, DashboardSessionState sessionState) {
			InitializeResult initializeResult = (InitializeResult)result;
			DashboardSession session = serverResult.Session;
			initializeResult.SessionId = serverResult.SessionId.ToString();
			initializeResult.RootPane = session.GetRootPane();
			initializeResult.DashboardParameters = session.GetParametersViewModel();
			initializeResult.TitleViewModel = session.GetTitleViewModel();
			initializeResult.ExportData = session.GetExportData();
			initializeResult.Localization = new ViewModel.DashboardLocalizationViewModel() {
				ClearMasterFilter = DashboardLocalizer.GetString(DashboardStringId.ActionClearMasterFilter),
				ClearSelection = DashboardLocalizer.GetString(DashboardStringId.ActionClearSelection),
				ElementSelection = DashboardLocalizer.GetString(DashboardStringId.ActionOtherValues),
				DrillUp = DashboardLocalizer.GetString(DashboardStringId.ActionDrillUp),
				ExportTemplate = DashboardLocalizer.GetString(DashboardStringId.ActionExportTemplate),
				ExportTo = DashboardLocalizer.GetString(DashboardStringId.ActionExportTo),
				ExportToPdf = DashboardLocalizer.GetString(DashboardStringId.ActionExportToPdf),
				ExportToExcel = DashboardLocalizer.GetString(DashboardStringId.ActionExportToExcel),
				ExportToImage = DashboardLocalizer.GetString(DashboardStringId.ActionExportToImage),
				AllowMultiselection = DashboardLocalizer.GetString(DashboardStringId.ActionAllowMultiselection),
				DashboardNullValue = DashboardLocalizer.GetString(DashboardStringId.DashboardNullValue),
				DashboardOthersValue = DashboardLocalizer.GetString(DashboardStringId.TopNOthersValue),
				DashboardErrorValue = DashboardLocalizer.GetString(DashboardStringId.DashboardErrorValue),
				DateTimeQuarterFormat = DashboardLocalizer.GetString(DashboardStringId.DateTimeQuarterFormat),
				Loading = DashboardLocalizer.GetString(DashboardStringId.MessageLoading),
				ParametersFormCaption = DashboardLocalizer.GetString(DashboardStringId.ParametersFormCaption),
				ParametersSelectorText = DashboardLocalizer.GetString(DashboardStringId.ParametersSelectorText),
				ButtonOK = DashboardLocalizer.GetString(DashboardStringId.ButtonOK),
				ButtonCancel = DashboardLocalizer.GetString(DashboardStringId.ButtonCancel),
				ButtonReset = DashboardLocalizer.GetString(DashboardStringId.ButtonReset),
				ButtonSubmit = DashboardLocalizer.GetString(DashboardStringId.ButtonSubmit),
				ButtonExport = DashboardLocalizer.GetString(DashboardStringId.ButtonExport),
				GridResetColumnWidths = DashboardLocalizer.GetString(DashboardStringId.GridResetColumnWidths),
				GridSortAscending = DashboardLocalizer.GetString(DashboardStringId.GridSortAscending),
				GridSortDescending = DashboardLocalizer.GetString(DashboardStringId.GridSortDescending),
				GridClearSorting = DashboardLocalizer.GetString(DashboardStringId.GridClearSorting),
				PivotGridTotal = DashboardLocalizer.GetString(DashboardStringId.PivotGridTotal),
				PivotGridGrandTotal = DashboardLocalizer.GetString(DashboardStringId.PivotGridGrandTotal),
				ChartTotalValue = DashboardLocalizer.GetString(DashboardStringId.ChartTotalValue),
				PageLayout = DashboardLocalizer.GetString(DashboardStringId.PageLayout),
				PageLayoutAuto = DashboardLocalizer.GetString(DashboardStringId.PageLayoutAuto),
				PageLayoutPortrait = DashboardLocalizer.GetString(DashboardStringId.PageLayoutPortrait),
				PageLayoutLandscape = DashboardLocalizer.GetString(DashboardStringId.PageLayoutLandscape),
				PaperKind = DashboardLocalizer.GetString(DashboardStringId.PaperKind),
				PaperKindLetter = DashboardLocalizer.GetString(DashboardStringId.PaperKindLetter),
				PaperKindLegal = DashboardLocalizer.GetString(DashboardStringId.PaperKindLegal),
				PaperKindExecutive = DashboardLocalizer.GetString(DashboardStringId.PaperKindExecutive),
				PaperKindA5 = DashboardLocalizer.GetString(DashboardStringId.PaperKindA5),
				PaperKindA4 = DashboardLocalizer.GetString(DashboardStringId.PaperKindA4),
				PaperKindA3 = DashboardLocalizer.GetString(DashboardStringId.PaperKindA3),
				ScaleMode = DashboardLocalizer.GetString(DashboardStringId.ScaleMode),
				ScaleModeNone = DashboardLocalizer.GetString(DashboardStringId.ScaleModeNone),
				ScaleModeUseScaleFactor = DashboardLocalizer.GetString(DashboardStringId.ScaleModeUseScaleFactor),
				ScaleModeAutoFitToPageWidth = DashboardLocalizer.GetString(DashboardStringId.ScaleModeAutoFitToPageWidth),
				AutoFitPageCount = DashboardLocalizer.GetString(DashboardStringId.AutoFitPageCount),
				ScaleFactor = DashboardLocalizer.GetString(DashboardStringId.ScaleFactor),
				PrintHeadersOnEveryPage = DashboardLocalizer.GetString(DashboardStringId.PrintHeadersOnEveryPage),
				FitToPageWidth = DashboardLocalizer.GetString(DashboardStringId.FitToPageWidth),
				SizeMode = DashboardLocalizer.GetString(DashboardStringId.SizeMode),
				SizeModeNone = DashboardLocalizer.GetString(DashboardStringId.SizeModeNone),
				SizeModeStretch = DashboardLocalizer.GetString(DashboardStringId.SizeModeStretch),
				SizeModeZoom = DashboardLocalizer.GetString(DashboardStringId.SizeModeZoom),
				AutoArrangeContent = DashboardLocalizer.GetString(DashboardStringId.AutoArrangeContent),
				ImageFormat = DashboardLocalizer.GetString(DashboardStringId.ImageFormat),
				ExcelFormat = DashboardLocalizer.GetString(DashboardStringId.ExcelFormat),
				Resolution = DashboardLocalizer.GetString(DashboardStringId.Resolution),
				ShowTitle = DashboardLocalizer.GetString(DashboardStringId.ShowTitle),
				Title = DashboardLocalizer.GetString(DashboardStringId.Title),
				CsvValueSeparator = DashboardLocalizer.GetString(DashboardStringId.CsvValueSeparator),
				FileName = DashboardLocalizer.GetString(DashboardStringId.FileName),
				FilterStatePresentation = DashboardLocalizer.GetString(DashboardStringId.FilterStatePresentation),
				FilterStatePresentationNone = DashboardLocalizer.GetString(DashboardStringId.FilterStatePresentationNone),
				FilterStatePresentationAfter = DashboardLocalizer.GetString(DashboardStringId.FilterStatePresentationAfter),
				FilterStatePresentationAfterAndSplitPage = DashboardLocalizer.GetString(DashboardStringId.FilterStatePresentationAfterAndSplitPage),
				MessageGridHasNoData = DashboardLocalizer.GetString(DashboardStringId.MessageGridHasNoData),
				MessagePivotHasNoData = DashboardLocalizer.GetString(DashboardStringId.MessagePivotHasNoData),
				SparklineTooltipStartValue = DashboardLocalizer.GetString(DashboardStringId.SparklineTooltipStartValue),
				SparklineTooltipEndValue = DashboardLocalizer.GetString(DashboardStringId.SparklineTooltipEndValue),
				SparklineTooltipMinValue = DashboardLocalizer.GetString(DashboardStringId.SparklineTooltipMinValue),
				SparklineTooltipMaxValue = DashboardLocalizer.GetString(DashboardStringId.SparklineTooltipMaxValue),
				OpenCaption = DashboardLocalizer.GetString(DashboardStringId.OpenCaption),
				HighCaption = DashboardLocalizer.GetString(DashboardStringId.HighCaption),
				LowCaption = DashboardLocalizer.GetString(DashboardStringId.LowCaption),
				CloseCaption = DashboardLocalizer.GetString(DashboardStringId.CloseCaption),
				InitialExtent = DashboardLocalizer.GetString(DashboardStringId.InitialExtent),
				FilterElementShowAllItem = DashboardLocalizer.GetString(DashboardStringId.FilterElementShowAllItem),
				NumericFormatUnitSymbolThousands =  DashboardLocalizer.GetString(DashboardStringId.NumericFormatUnitSymbolThousands),
				NumericFormatUnitSymbolMillions = DashboardLocalizer.GetString(DashboardStringId.NumericFormatUnitSymbolMillions),
				NumericFormatUnitSymbolBillions = DashboardLocalizer.GetString(DashboardStringId.NumericFormatUnitSymbolBillions)
			};
			if(session.DataLoaderErrors != null && session.DataLoaderErrors.Count > 0) {
				bool handleServerErrors = Args.Settings.HandleServerErrors;
				StringBuilder builder = new StringBuilder();
				string format = " - {0}";
				builder.Append(session.DataLoaderErrors[0].DataSourceName);
				if(handleServerErrors)
					builder.AppendFormat(format, session.DataLoaderErrors[0].Message);
				for(int i = 1; i < session.DataLoaderErrors.Count; i++) {
					builder.AppendLine();
					builder.Append(session.DataLoaderErrors[i].DataSourceName);
					if(handleServerErrors)
						builder.AppendFormat(format, session.DataLoaderErrors[i].Message);
				}
				string errorMessage = string.Format(DashboardLocalizer.GetString(DashboardStringId.LoadingDataError), builder.ToString());
				initializeResult.Error = new DashboardConnectionException(errorMessage);
				initializeResult.LoadingDataErrorMessage = errorMessage;
			}
			List<AffectedItemInfo> affectedItems = new List<AffectedItemInfo>();
			FillAffectedItemInfo(affectedItems, session.GetItemNames(), ContentType.FullContent);
			FillAffectedItemInfo(affectedItems, session.GetGroupNames(), ContentType.ViewModel | ContentType.CaptionViewModel);		 
			return affectedItems;
		}		
		void FillAffectedItemInfo(List<AffectedItemInfo> affectedItems, IEnumerable<string> names, ContentType contentType) {
			foreach(string name in names)
				affectedItems.Add(new AffectedItemInfo(name, contentType));
		}
	}	
}
