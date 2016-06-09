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
using System.ComponentModel;
using System.Linq;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Printing.Native.Lines;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.ExportOptionsControllers;
namespace DevExpress.Xpf.Printing.Native {
	public class ExportOptionsConfiguratorService : IExportOptionsConfiguratorService {
		public event EventHandler<AsyncCompletedEventArgs> Completed;
		public ExportOptionContext Context { get; set; }
		public void Configure(ExportOptionsBase options, PrintPreviewOptions previewOptions, AvailableExportModes availableExportModes, List<ExportOptionKind> hiddenOptions) {
			if(previewOptions.ShowOptionsBeforeExport)
				ShowConfigurationWindow(options, availableExportModes, hiddenOptions);
			else
				RaiseCompleted(false);
		}
		void ShowConfigurationWindow(ExportOptionsBase options, AvailableExportModes availableExportModes, List<ExportOptionKind> hiddenOptions) {
			ExportOptionsControllerBase controller = ExportOptionsControllerBase.GetControllerByOptions(options);
			ExportOptionsBase clonedOptions = ExportOptionsHelper.CloneOptions(options);
			var lines = controller.GetExportLines(clonedOptions, new LineFactory(), availableExportModes, hiddenOptions).Cast<LineBase>().ToArray();
			if(lines.Length > 0) {
				var linesWindow = new LinesWindow {
					Title = PreviewLocalizer.GetString(controller.CaptionStringId)
				};
				linesWindow.Closed += (o, e) => OnLinesWindowClosed(linesWindow, options, clonedOptions);
				linesWindow.SetLines(lines);
				linesWindow.ShowDialog();				
			}
		}
		void OnLinesWindowClosed(LinesWindow linesWindow, ExportOptionsBase options, ExportOptionsBase clonedOptions) {
#if SL
			bool cancelled = linesWindow.DialogResult != DialogResult.OK;
#else
			bool cancelled = linesWindow.DialogResult.HasValue ? !linesWindow.DialogResult.Value : true;
#endif
			if(!cancelled)
				options.Assign(clonedOptions);
			RaiseCompleted(cancelled);
		}
		void RaiseCompleted(bool cancelled) {
			if(Completed != null)
				Completed(this, new AsyncCompletedEventArgs(null, cancelled, null));
		}
	}
}
