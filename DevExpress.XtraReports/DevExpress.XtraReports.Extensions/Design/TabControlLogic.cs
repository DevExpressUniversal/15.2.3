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
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Preview;
using System.Windows.Forms;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Control;
using System.ComponentModel.Design;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.Design {
	public abstract class TabControlLogicCreator {
		public abstract TabControlLogic CreateInstance(ReportTabControl tabControl, IServiceProvider servProvider);
	}
	public abstract class TabControlLogic {
		static int[] GetDesignRange() {
			List<int> range = new List<int>(new int[] {
					ZoomService.PredefinedZoomFactorsInPercents[0], 
					ZoomService.PredefinedZoomFactorsInPercents[ZoomService.PredefinedZoomFactorsInPercents.Length - 1]}
				);
			range.Sort(Comparer<int>.Default);
			return range.ToArray();
		}
		protected ReportTabControl tabControl;
		ZoomService zoomService;
		ZoomTrackBarEditItem zoomEditItem;
		int[] designRange = GetDesignRange();
		BarStaticItem zoomTextItem;
		IServiceProvider servProvider;
		protected BarStaticItem ZoomTextItem {
			get {
				if(zoomTextItem == null)
					zoomTextItem = GetZoomFactorTextStaticItem();
				return zoomTextItem;
			}
		}
		internal protected ZoomTrackBarEditItem ZoomEditItem {
			get {
				if(zoomEditItem == null)
					zoomEditItem = CreateZoomTrackBar();
				return zoomEditItem;
			}
		}
		protected ZoomService ZoomService {
			get {
				if(zoomService == null) {
					zoomService = CreateZoomService();
					if(zoomService != null)
						zoomService.ZoomChanged += new EventHandler(OnZoomChanged);
				}
				return zoomService;
			}
		}
		protected void OnZoomChanged(object sender, EventArgs e) {
			ApplyZoomFactor(ZoomService.ZoomFactor);
		}
		internal abstract BarManager BarManager { get; }
#if DEBUGTEST
		public virtual BarStaticItem DesignZoomItem {
			get { return null; }
		}
#endif
		public TabControlLogic(ReportTabControl tabControl, IServiceProvider servProvider) {
			this.tabControl = tabControl;
			this.servProvider = servProvider;
		}
		protected string GetStringById(ReportStringId id) {
			return ReportLocalizer.GetString(id);
		}
		public virtual void AddPage(Control control, ReportStringId id, ReportCommand command, bool beginGroup) {
			tabControl.AddPage(control, new XtraTabControl.TabPage(control, GetStringById(id), command));
		}
		public virtual void AddPage(ReportStringId id, ReportCommand command, bool beginGroup) {
			tabControl.AddPage(new XtraTabControl.TabPage(null, GetStringById(id), command));
		}
		public virtual void Initialize(IServiceProvider serviceProvider, bool isEUD) {
		}
		public virtual void UpdateStatus(string status) {
		}
		public virtual void OnDesignerVisible() {
			ForceZoomService();
			SetZoomItemsLocked(false);
			if(ZoomEditItem != null) {
				ZoomEditItem.Range = designRange;
				ApplyZoomFactor(ZoomService.ZoomFactor);
				ZoomEditItem.CommandExecuter = ExecCommand;
			}
			EnableCommand(PrintingSystemCommand.ZoomTrackBar, true);
		}
		public virtual void OnPreviewVisible() {
			SetZoomItemsLocked(false);
			InitZoomItem(tabControl.PreviewControl);
		}
		protected void InitZoomItem(PrintControl printControl) {
			if(ZoomEditItem != null) {
				ZoomEditItem.Range = ZoomTrackBarEditItem.DefaultRange;
				if(printControl != null)
					ZoomEditItem.CommandExecuter = printControl.ExecCommand;
			}
		}
		public virtual void OnScriptsVisible() {
			ApplyZoomFactor(1);
			EnableCommand(PrintingSystemCommand.ZoomTrackBar, false);
			SetZoomItemsLocked(true);
		}
		void SetZoomItemsLocked(bool locked) {
			if(ZoomEditItem != null) {
				ZoomEditItem.Locked = locked;
				SetBarItemLocked(this.ZoomEditItem, locked);
			}
			if(this.ZoomTextItem != null)
				SetBarItemLocked(this.ZoomTextItem, locked);
		}
		public virtual void OnBrowserVisible() {
			ApplyZoomFactor(1);
			EnableCommand(PrintingSystemCommand.ZoomTrackBar, false);
			SetZoomItemsLocked(true);
		}
		public virtual void OnBrowserUpdated() {
		}
		public virtual void Dispose() {
			if(zoomService != null) {
				zoomService.ZoomChanged -= new EventHandler(OnZoomChanged);
				zoomService = null;
			}
		}
		public virtual void Activate() {
		}
		public virtual void OnPrintControlCreated() {
		}
		void ExecCommand(PrintingSystemCommand command, object[] args) {
			if(ZoomService != null) {
				IDesignerHost host = servProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host != null)
					new DesignerHostExtensions(host).CommitInplaceEditors();
				ZoomService.ZoomFactor = Convert.ToSingle(args[0]);
			}
		}
		void ForceZoomService() {
			ZoomService ignore = ZoomService;
		}
		void ApplyZoomFactor(float zoom) {
			if(ZoomEditItem != null)
				ZoomEditItem.ApplyZoom(zoom);
			if(ZoomTextItem != null)
				ZoomTextItem.Caption = PreviewItemsLogicBase.ZoomFactorToString(zoom);
		}
		protected abstract ZoomTrackBarEditItem CreateZoomTrackBar();
		protected abstract ZoomService CreateZoomService();
		protected abstract BarStaticItem GetZoomFactorTextStaticItem();
		protected abstract void SetBarItemLocked(BarItem item, bool locked);
		protected abstract void EnableCommand(PrintingSystemCommand command, bool enabled);
	}
}
