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
using System.Text;
namespace DevExpress.XtraPrinting {
	public enum CommandVisibility {
		None = 0,
		Menu = 1,
		Toolbar = 2,
		All = Menu | Toolbar
	}
}
namespace DevExpress.XtraPrinting.Native {
	public enum Priority {
		Low = 0,
		High = 1,
	}
	public abstract class CommandSetBase {
		protected bool fCommandVisibilityChanged;
		private bool dirty;
		public bool Dirty {
			get { return dirty; }
			set { dirty = value; }
		}
		public event EventHandler CommandVisibilityChanged;
		public static CommandVisibility ToCommandVisibility(bool value) {
			return value ? CommandVisibility.All : CommandVisibility.None;
		}
		public void UpdateOpenAndClosePreviewCommands(bool enable) {
			EnableCommand(enable, PrintingSystemCommand.Open);
			EnableCommand(enable, PrintingSystemCommand.ClosePreview);
		}
		public void UpdateStopPageBuildingCommand(bool enable, PrintingSystemBase ps) {
			EnableCommand(enable, PrintingSystemCommand.StopPageBuilding);
			SetCommandVisibility(PrintingSystemCommand.StopPageBuilding, CommandSetBase.ToCommandVisibility(enable), Priority.High, ps);
		}
		public void EnableCommand(bool value, params PrintingSystemCommand[] commands) {
			foreach(PrintingSystemCommand command in commands)
				EnableCommand(value, command);
		}
		internal void SetCommandVisibility(PrintingSystemCommand command, CommandVisibility visibility, Priority priority, PrintingSystemBase ps) {
			SuspendRaiseCommandChanged();
			SetCommandVisibilityCore(command, visibility, priority, ps);
			ResumeRaiseCommandChanged();
		}
		internal void SetCommandVisibility(PrintingSystemCommand[] commands, CommandVisibility visibility, Priority priority, PrintingSystemBase ps) {
			SuspendRaiseCommandChanged();
			foreach(PrintingSystemCommand command in commands)
				SetCommandVisibilityCore(command, visibility, priority, ps);
			ResumeRaiseCommandChanged();
		}
		protected void SuspendRaiseCommandChanged() {
			fCommandVisibilityChanged = false;
		}
		protected void ResumeRaiseCommandChanged() {
			if(fCommandVisibilityChanged && CommandVisibilityChanged != null)
				CommandVisibilityChanged(this, EventArgs.Empty);
		}
		protected abstract bool ContainsCommand(PrintingSystemCommand command);
		protected abstract void EnableCommand(bool value, PrintingSystemCommand command);
		protected abstract void SetCommandVisibilityCore(PrintingSystemCommand command, CommandVisibility visibility, Priority priority, PrintingSystemBase ps);
		public abstract bool HasEnabledCommand(PrintingSystemCommand[] commands);
	}
	public static class CommandSetBaseExtentions {
		public static void UpdateCommands(this CommandSetBase commandSet, int pageIndex, float zoomFactor, float minZoomFactor, float maxZoomFactor, bool hasParameters, Document document) {
			PrintingSystemBase ps = document.PrintingSystem;
			int pageCount = document.Pages.Count;
			bool hasPages = pageCount > 0;
			bool hasBookmarks = document.BookmarkNodes.Count > 0;
			commandSet.EnableCommand(hasPages,
				PrintingSystemCommand.Watermark,
				PrintingSystemCommand.Pointer,
				PrintingSystemCommand.HandTool,
				PrintingSystemCommand.Magnifier,
				PrintingSystemCommand.Zoom,
				PrintingSystemCommand.ZoomTrackBar,
				PrintingSystemCommand.ZoomToPageWidth,
				PrintingSystemCommand.ZoomToTextWidth,
				PrintingSystemCommand.ZoomToWholePage,
				PrintingSystemCommand.ZoomToTwoPages,
				PrintingSystemCommand.FillBackground,
				PrintingSystemCommand.PageLayoutContinuous,
				PrintingSystemCommand.PageLayoutFacing,
				PrintingSystemCommand.GoToPage);
			commandSet.EnableCommand(pageCount > 1, PrintingSystemCommand.MultiplePages);
			commandSet.EnableCommand(hasPages && !document.IsCreating,
				PrintingSystemCommand.Print,
				PrintingSystemCommand.PrintDirect,
				PrintingSystemCommand.EditPageHF,
				PrintingSystemCommand.Customize,
				PrintingSystemCommand.Find,
				PrintingSystemCommand.ExportFile,
				PrintingSystemCommand.SendFile,
				PrintingSystemCommand.Save);
			commandSet.EnableCommand(!document.IsCreating, PrintingSystemCommand.SubmitParameters);
			commandSet.EnableCommand(hasPages && zoomFactor < maxZoomFactor,
				PrintingSystemCommand.ZoomIn);
			commandSet.EnableCommand(hasPages && zoomFactor > minZoomFactor,
				PrintingSystemCommand.ZoomOut);
			commandSet.EnableCommand(hasPages && !document.IsCreating, PSCommandHelper.PageExportCommands);
			commandSet.EnableCommand(hasPages && !document.IsCreating, PSCommandHelper.PageSendCommands);
			commandSet.EnableCommand(hasPages && !document.IsCreating, PSCommandHelper.ContinuousExportCommands);
			commandSet.EnableCommand(hasPages && !document.IsCreating, PSCommandHelper.ContinuousSendCommands);
			commandSet.EnableCommand(hasPages && !document.IsCreating && document.CanPerformContinuousExport, PSCommandHelper.AllowOnlyContinuousExportCommands);
			commandSet.EnableCommand(hasPages && !document.IsCreating && document.CanPerformContinuousExport, PSCommandHelper.AllowOnlyContinuousSendCommands);
			if(ps.IsNoneVisibleCommand(ps.ExportOptions.PrintPreview.DefaultExportFormat) || ps.ExportOptions.PrintPreview.DefaultExportFormat == PrintingSystemCommand.None)
				ps.ExportOptions.PrintPreview.DefaultExportFormat = ps.GetFirstVisibleCommand(PSCommandHelper.ExportCommands);
			if(ps.IsNoneVisibleCommand(ps.ExportOptions.PrintPreview.DefaultSendFormat) || ps.ExportOptions.PrintPreview.DefaultSendFormat == PrintingSystemCommand.None)
				ps.ExportOptions.PrintPreview.DefaultSendFormat = ps.GetFirstVisibleCommand(PSCommandHelper.SendCommands);
			commandSet.EnableCommand(ps.ExportOptions.PrintPreview.DefaultExportFormat != PrintingSystemCommand.None && commandSet.HasEnabledCommand(PSCommandHelper.ExportCommands), PrintingSystemCommand.ExportFile);
			commandSet.EnableCommand(ps.ExportOptions.PrintPreview.DefaultSendFormat != PrintingSystemCommand.None && commandSet.HasEnabledCommand(PSCommandHelper.SendCommands), PrintingSystemCommand.SendFile);
			commandSet.EnableCommand(pageCount > 1 && pageIndex > 0,
				PrintingSystemCommand.ShowPrevPage,
				PrintingSystemCommand.ShowFirstPage);
			commandSet.EnableCommand(pageCount > 1 && pageIndex < pageCount - 1,
				PrintingSystemCommand.ShowNextPage,
				PrintingSystemCommand.ShowLastPage);
			commandSet.EnableCommand(hasBookmarks, PrintingSystemCommand.DocumentMap);
			commandSet.EnableCommand(hasPages, PrintingSystemCommand.Thumbnails);
			commandSet.EnableCommand(document.CanRecreatePages,
				PrintingSystemCommand.PageSetup,
				PrintingSystemCommand.Scale,
				PrintingSystemCommand.PageMargins,
				PrintingSystemCommand.PageOrientation,
				PrintingSystemCommand.PaperSize
				);
			commandSet.EnableCommand(hasParameters, PrintingSystemCommand.Parameters);
			commandSet.SetCommandVisibility(PrintingSystemCommand.Parameters, CommandSetBase.ToCommandVisibility(hasParameters), Priority.Low, document.PrintingSystem);
			commandSet.SetCommandVisibility(PrintingSystemCommand.DocumentMap, CommandSetBase.ToCommandVisibility(hasBookmarks), Priority.Low, document.PrintingSystem);
			commandSet.SetCommandVisibility(PrintingSystemCommand.Thumbnails, CommandSetBase.ToCommandVisibility(hasPages && document.IsCreated), Priority.Low, document.PrintingSystem);
			commandSet.SetCommandVisibility(PrintingSystemCommand.ExportXps, CommandVisibility.None, Priority.High, document.PrintingSystem);
			commandSet.SetCommandVisibility(PrintingSystemCommand.SendXps, CommandVisibility.None, Priority.High, document.PrintingSystem);
		}
	}
}
