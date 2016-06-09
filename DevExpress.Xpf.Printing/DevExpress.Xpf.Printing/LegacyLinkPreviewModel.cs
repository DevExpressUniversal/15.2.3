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
using System.Windows;
using System.Windows.Interop;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Printing.Parameters.Models;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.XamlExport;
namespace DevExpress.Xpf.Printing {
#if SL
	public abstract
#else
	public
#endif
	class LegacyLinkPreviewModel : PrintingSystemPreviewModel {
		#region Fields & Properties
		readonly IScaleService scaleService;
		ILink link;
		public ILink Link {
			get { return link; }
			set {
				if(link == value)
					return;
				OnSourceChanging();
				link = value;
				OnSourceChanged();
			}
		}
		public override bool IsEmptyDocument {
			get { return !IsCreating && PageCount == 0; }
		}
		public override ParametersModel ParametersModel {
			get { return null; }
		}
		protected internal override PrintingSystemBase PrintingSystem {
			get {
				if(link == null)
					return null;
				return (PrintingSystemBase)link.PrintingSystem;
			}
		}
		protected override ExportOptions DocumentExportOptions {
			get { return PrintingSystem.ExportOptions; }
			set { throw new NotImplementedException(); }
		}
		protected override AvailableExportModes DocumentExportModes {
			get { throw new NotImplementedException(); }
		}
		protected override List<ExportOptionKind> DocumentHiddenOptions {
			get { throw new NotImplementedException(); }
		}
		#endregion
		#region Constructors
		public LegacyLinkPreviewModel()
			: this(null) {
		}
		public LegacyLinkPreviewModel(ILink link)
			: this(link,
				   new PageSettingsConfiguratorService(),
				   new PrintService(),
				   new ExportSendService(),
				   new HighlightingService(),
				   new ScaleService()) {
			OnSourceChanged();
		}
		internal LegacyLinkPreviewModel(ILink link,
										IPageSettingsConfiguratorService pageSettingsConfiguratorService,
										IPrintService printService,
										IExportSendService exportSendService,
										IHighlightingService highlightService,
										IScaleService scaleService)
			: base(pageSettingsConfiguratorService, printService, exportSendService, highlightService) {
			this.link = link;
			this.scaleService = scaleService;
			OnSourceChanged();
		}
		#endregion
		#region Methods
		protected override void CreateDocument(bool buildPagesInBackground) {
			link.CreateDocument(buildPagesInBackground);
		}
		protected override FrameworkElement VisualizePage(int pageIndex) {
			PageVisualizer strategy = new BrickPageVisualizer(TextMeasurementSystem.GdiPlus);
			return strategy.Visualize((PSPage)PrintingSystem.Pages[pageIndex], pageIndex, PrintingSystem.Pages.Count);
		}
		protected override bool CanShowParametersPanel(object parameter) {
			return false;
		}
		protected override void HookPrintingSystem() {
			base.HookPrintingSystem();
			PrintingSystem.ReplaceService(typeof(BackgroundPageBuildEngineStrategy), new DispatcherPageBuildStrategy());
		}
		protected override void Export(ExportFormat format) {
			throw new NotImplementedException();
		}
		protected override bool CanSetWatermark(object parameter) {
			return false;
		}
		protected override void SetWatermark(object parameter) {
			throw new NotImplementedException();
		}
		protected override bool CanScale(object parameter) {
#if SL
			return BuildPagesComplete && PrintingSystem.Document.CanChangePageSettings && !IsExporting;
#else
			return BuildPagesComplete && PrintingSystem.Document.CanChangePageSettings && !IsExporting && !IsSaving;
#endif
		}
		protected override void Scale(object parameter) {
#if SL
			scaleService.Scale(this);
#else
			scaleService.Scale(this, DialogService.GetParentWindow());
#endif
		}
		#endregion
	}
}
