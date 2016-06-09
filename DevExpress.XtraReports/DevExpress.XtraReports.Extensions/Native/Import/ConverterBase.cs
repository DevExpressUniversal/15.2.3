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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Localization;
using System.IO;
namespace DevExpress.XtraReports.Import
{
	public abstract class ConverterBase {
		static bool inAssemblyResolve;
		internal static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args) {
			if (inAssemblyResolve) 
				return null;
			inAssemblyResolve = true;
			Assembly asm = null;
			for (int i = 0; i < 10; i++)
				try {
#pragma warning disable 618
					asm = Assembly.LoadWithPartialName(Serialization.XRSerializer.GetShortAssemblyName(args.Name));
#pragma warning restore 618
					break;
				}
				catch {
				}
			inAssemblyResolve = false;
			return asm;
		}		
		protected IDesignerHost designerHost;
		protected XtraReport fTargetReport;
		protected string fileName;
		protected ConverterBase() {
		}
		public void Convert(string fileName, XtraReport report) {
			this.fileName = fileName;
			Convert(report);
		}
		public void Convert(XtraReport report) {
			fTargetReport = report;
			if (fTargetReport == null)
				return;
			designerHost = fTargetReport.Site == null ? null : (IDesignerHost)fTargetReport.Site.GetService(typeof(IDesignerHost));
			DesignerTransaction transaction = designerHost == null ? null : designerHost.CreateTransaction(TransactionDescription);
			SubscribeAssemblyResolveEvent();
			Design.ReportTabControl tabControl = null;
			try {
				ClearContent();
				if (designerHost != null) {
					Design.IBandViewInfoService svc = (Design.IBandViewInfoService)designerHost.GetService(typeof(Design.IBandViewInfoService));
					if (svc != null)
						svc.InvalidateViewInfo();
					tabControl = (Design.ReportTabControl)designerHost.GetService(typeof(Design.ReportTabControl));
					if (tabControl != null) {
						tabControl.UpdateStatusValue(ReportLocalizer.GetString(ReportStringId.Msg_ReportImporting));
						tabControl.Invalidate(true);
						tabControl.Update();
					}
				}
				GetBandByType(typeof(DetailBand));
				ResetProperties();
				ConvertInternal(report);
				if (transaction != null) {
					transaction.Commit();
					transaction = null;
				}
			} finally {
				if (tabControl != null)
					tabControl.UpdateStatusValue("");
				if (transaction != null)
					transaction.Cancel();
				UnsubsribeAssemblyResolveEvent();
			}
		}
		protected virtual void ResetProperties() {
			fTargetReport.DataSource = null;
			fTargetReport.DataAdapter = null;
			fTargetReport.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
		}
		protected virtual string TransactionDescription {
			get {
				return "Import from " + System.IO.Path.GetFileName(fileName);
			}
		}
		protected virtual void SubscribeAssemblyResolveEvent() {
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
		}   
		protected virtual void UnsubsribeAssemblyResolveEvent() {
			AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(MyResolveEventHandler);
		}
		void ClearContent() {
			ClearTargetReportBands();
			if (designerHost != null) {
				ArrayList components = new ArrayList(designerHost.Container.Components);
				foreach (IComponent component in components)
					if (component != designerHost.RootComponent) {
						try {
							designerHost.DestroyComponent(component);
						} catch { }
					}
			}
		}
		static BandKind GetBandKind(Type bandType) {
			if (bandType == typeof(DetailBand))
				return BandKind.Detail;
			if (bandType == typeof(DetailReportBand))
				return BandKind.DetailReport;
			if (bandType == typeof(GroupHeaderBand))
				return BandKind.GroupHeader;
			if (bandType == typeof(GroupFooterBand))
				return BandKind.GroupFooter;
			if (bandType == typeof(TopMarginBand))
				return BandKind.TopMargin;
			if (bandType == typeof(BottomMarginBand))
				return BandKind.BottomMargin;
			if (bandType == typeof(ReportHeaderBand))
				return BandKind.ReportHeader;
			if (bandType == typeof(ReportFooterBand))
				return BandKind.ReportFooter;
			if (bandType == typeof(PageHeaderBand))
				return BandKind.PageHeader;
			if (bandType == typeof(PageFooterBand))
				return BandKind.PageFooter;
			throw new ArgumentException("bandType");
		}
		protected Band GetBandByType(Type bandType) {
			Band band = fTargetReport.Bands.GetBandByType(bandType);
			if (band == null || bandType == typeof(GroupFooterBand) || bandType == typeof(GroupHeaderBand)) {
				if (designerHost == null)
					band = XtraReport.CreateBand(GetBandKind(bandType));
				else {
					band = (Band)designerHost.CreateComponent(bandType);
					SetControlName(band, Design.BandDesigner.CreateBandName(designerHost, band));
				}
				fTargetReport.Bands.Add(band);
			}
			return band;
		}
		static protected void SetControlName(XRControl ctrl, string name) {
			try {
				if (ctrl.Site != null)
					ctrl.Site.Name = name;
				else
					ctrl.Name = name;
			} catch {
			}
		}
		protected void ClearTargetReportBands() {
			ArrayList bands = new ArrayList(fTargetReport.Bands);
			foreach (Band band in bands)
				DestroyBand(band);
		}
		protected void DestroyBand(Band band) {
			if (designerHost == null) {
				foreach (XRControl control in band.Controls)
					control.Dispose();
				band.Controls.Clear();
				if(band.HasSubBands) {
					foreach(Band subBand in band.SubBands)
						DestroyBand(subBand);
					band.SubBands.Clear();
				}
				fTargetReport.Bands.Remove(band);
				band.Dispose();
			} else {
				while (band.Controls.Count > 0)
					designerHost.DestroyComponent(band.Controls[0]);
				if(band.HasSubBands) {
					while(band.SubBands.Count > 0)
						DestroyBand(band.SubBands[0]);
				}
				designerHost.DestroyComponent(band);
			}
		}
		protected abstract void ConvertInternal(XtraReport report);
#if DEBUGTEST
		internal void Test_ClearContent(XtraReport report, IDesignerHost host) {
			designerHost = host;
			fTargetReport = report;
			ClearContent();
		}
#endif
	}
}
