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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Printing.PreviewControl.Native.Models;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.Xpf.Printing.PreviewControl.Native {
	public interface IDocumentViewModel {
		event EventHandler DocumentCreated;
		event EventHandler StartDocumentCreation;
		event ExceptionEventHandler DocumentException;
		bool HasBookmarks { get; }
		bool IsLoaded { get; }
		bool IsCreating { get; }
		bool IsCreated { get; }
		bool CanChangePageSettings { get; }
		string DeafultFileName { get; }
		string InitialDirectory { get; }
		ExportFormat DefaultExportFormat { get; set; }
		ExportFormat DefaultSendFormat { get; set; }
		Watermark Watermark { get; }
		XtraPageSettingsBase PageSettings { get; }
		ObservableCollection<PageViewModel> Pages { get; }
		void CreateDocument();
		IEnumerable<PreviewBookmarkNode> GetBookmarks();
		void MarkBrick(PreviewBookmarkNode bookmark);
		void ResetMarkedBricks();
		void Print(PrintOptionsViewModel model);
		void PrintDirect(string printerName = null);
		void Export(ExportOptionsViewModel options);
		void Send(SendOptionsViewModel options);
		void Save(string filePath);
		void Scale(ScaleOptionsViewModel model);
		void SetWatermark(XtraPrinting.Native.XpfWatermark xpfWatermark);
		BrickPagePair PerformSearch(TextSearchParameter parameter);
		void StopPageBuilding();
	}
	public interface IReportDocument : IDocumentViewModel {
		void Submit(IList<Parameter> parameters);
	}
	public interface IProgressSettings : INotifyPropertyChanged {
		bool InProgress { get; }
		ProgressType ProgressType { get; }
		int ProgressPosition { get; }
	}
	public enum ProgressType {
		Default,
		Marquee
	}
}
