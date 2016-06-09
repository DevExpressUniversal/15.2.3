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
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraReports.UI;
using System.Windows.Forms.Design;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Design.Ruler;
using DevExpress.XtraReports.Localization;
using DevExpress.Utils.Design;
using DevExpress.XtraReports.UserDesigner;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.MouseTargets;
namespace DevExpress.XtraReports.Design {
	public interface IDesignFrame {
		bool Expanded { get; set; }
		string Text { get; }
		Band Band { get; } 
	} 
	[MouseTarget(typeof(BandMouseTarget))]
	public class BandDesigner : XRControlDesignerBase, IDesignFrame {
		public static string CreateBandName(IServiceProvider serviceProvider, Band band) {
			string baseName = band.BandKind.ToString();
			string name = baseName;
			int index = 1;
			if(band is GroupBand || band is SubBand) {
				name = String.Format("{0}{1}", baseName, index++);
			}
			INameCreationService nameServ = serviceProvider.GetService(typeof(INameCreationService)) as INameCreationService;
			while(nameServ.IsValidName(name) == false) {
				name = String.Format("{0}{1}", baseName, index++);
			}
			return name;
		}
		static string GetBandDescription(BandKind bandKind) {
			return ((bandKind & (BandKind.TopMargin | BandKind.BottomMargin | BandKind.PageHeader | BandKind.PageFooter)) > 0) ?
				ReportLocalizer.GetString(ReportStringId.BandDsg_QuantityPerPage) :
				((bandKind & (BandKind.ReportHeader | BandKind.ReportFooter)) > 0) ?
				ReportLocalizer.GetString(ReportStringId.BandDsg_QuantityPerReport) : String.Empty;
		}
		#region fields & properties
		private string text = "";
		CascadeChangeEventHelper<GroupBand> levelPropertyChangeHandler;
		DesignerVerbCollection verbs;
		public override DesignerVerbCollection Verbs {
			get { return verbs; }
		}
		string IDesignFrame.Text { 
			get {
				if(text.Length == 0 && Band.Parent != null) {
					text = Band.Site.Name;
					if(Band.Parent is XtraReport) {
						string s =  GetBandDescription(Band.BandKind);
						if(s.Length > 0) text += String.Format(" [{0}]", s);
					}
				}
				return text;
			}
		}
		public bool Expanded { 
			get { return Band.Expanded; }
			set {
				if(Expanded == value || fDesignerHost.IsDebugging())
					return;
				try {
					RaiseExpandedChanging(this);
					Band.Expanded = value;
					RaiseExpandedChanged(this);
				} catch(Exception ex) {
					if(ExceptionHelper.IsCriticalException(ex))
						throw;
				}
			}
		}
		public override Band Band {
			get { return Component as Band; }
		}
		public override bool CanDragInReportExplorer {
			get { return false; }
		}
		protected override bool CanAddToSelection {
			get {
				foreach(object obj in selectionService.GetSelectedComponents())
					if(!(obj is Band)) return false;
				return true;
			}
		}
		#endregion
		public BandDesigner()
			: base() {
			verbs = new DesignerVerbCollection();
			if(!(Band is SubBand))
				verbs.Add(CreateXRDesignerVerb(DesignSR.Verb_EditBands, ReportCommand.VerbEditBands));
		}
		protected internal override int AddComponent(IComponent c) {
			return c is SubBand ? Band.SubBands.Add((SubBand)c) :
				XRControl.Controls.Add((XRControl)c);
		}
		protected override bool TryGetCollectionName(IComponent c, out string name) {
			if(c is SubBand) {
				name = "SubBands";
				return true;
			}
			return base.TryGetCollectionName(c, out name);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			levelPropertyChangeHandler = new CascadeChangeEventHelper<GroupBand>(component, DesignerHost, XRComponentPropertyNames.Level);
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			if(fDesignerHost == null || fDesignerHost.Loading || Band.Report == null)
				return;
			if(Band.GetPreviousBand() is GroupHeaderBand)
				SetPropertyReadOnly(properties, "DrillDownControl", false);
			else
				SetPropertyReadOnly(properties, "DrillDownControl", true);
		}
		static void SetPropertyReadOnly(IDictionary properties, string propName, bool isReadOnly) {
			PropertyDescriptor prop = properties[propName] as PropertyDescriptor;
			if(prop != null)
				properties[propName] = TypeDescriptor.CreateProperty(prop.ComponentType, prop, new ReadOnlyAttribute(isReadOnly));
		}
		public override void OnComponentChanged(ComponentChangedEventArgs e) {
			base.OnComponentChanged(e);
			text = "";
		}
		public override string GetStatus() {
			return String.Format("{0} {{ {2}:{1} }}", Component.Site.Name, Band.HeightF, ReportLocalizer.GetString(ReportStringId.DesignerStatus_Height));
		}
		public override bool CanCutControl {
			get { return false; }
		}
		protected override XRControl FindParent() {
			return ReportDesigner.ActiveReport;
		}
		protected internal override bool CanAddBand(BandKind bandKind) {
			if(bandKind != BandKind.SubBand || (Band is MarginBand) || (Band is XtraReportBase) || (IsEUD && Band.LockedInUserDesigner))
				return false;
			return true;
		}
		public override SelectionRules GetSelectionRules() {
			return SelectionRules.BottomSizeable;
		}
		internal void SetBandHeight(float height) {
			if(Band.HeightF == height) return;
			DesignerTransaction trans = fDesignerHost.CreateTransaction(String.Format(DesignSR.TransFmt_ChangeHeight, Band.Site.Name));
			try {
				RaiseComponentChanging(Band, XRComponentPropertyNames.Height);
				float oldValue = Band.HeightF;
				Band.HeightF = height;
				RaiseComponentChanged(Band, XRComponentPropertyNames.Height, oldValue, height);
				trans.Commit();
			} catch {
				trans.Cancel();
			}
		}
		public override void SetSize(SizeF value, bool raiseChanged) {
			SetBandHeight(value.Height);
		}
		public override void SetRightBottom(PointF value, SizeF stepSize, RectangleSpecified specified, bool raiseChanged) {
			SetBandHeight(value.Y);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			if(Band is DetailBand) {
				list.Add(new DetailBandDesignerActionList1(this));
				list.Add(new DetailBandDesignerActionList2(this));
				list.Add(new XRFormattingControlDesignerActionList(this));
				list.Add(new DetailBandDesignerActionList(this));
			} else if(Band is GroupHeaderBand) {
				list.Add(new GroupHeaderBandDesignerActionList(this));
				list.Add(new XRFormattingControlDesignerActionList(this));
				list.Add(new GroupBandDesignerActionList(this));
			} else if(Band is GroupFooterBand) {
				list.Add(new GroupFooterBandDesignerActionList1(this));
				list.Add(new XRFormattingControlDesignerActionList(this));
				list.Add(new GroupFooterBandDesignerActionList2(this));
			} else if(Band is ReportFooterBand) {
				list.Add(new XRFormattingControlDesignerActionList(this));
				list.Add(new ReportFooterBandDesignerActionList(this));
			} else if(Band is PageBand) {
				list.Add(new PageBandActionList(this));
				list.Add(new XRFormattingControlDesignerActionList(this));
			} else {
				list.Add(new XRFormattingControlDesignerActionList(this));
				list.Add(new BandDesignerActionList1(this));
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing ) {
					if(levelPropertyChangeHandler != null) {
						levelPropertyChangeHandler.Dispose();
						levelPropertyChangeHandler = null;
					}
				} 
			} finally {
				base.Dispose(disposing);
			}
		}
	}
	public class SubBandDesigner : BandDesigner {
		SubBand SubBand {
			get { return (SubBand)Component; }
		}
		protected override XRControl FindParent() {
			return selectionService.PrimarySelection as Band;
		}
		protected internal override bool CanAddBand(BandKind bandKind) {
			return false;
		}
	}
	public class CascadeChangeEventHelper<ComponentType> : IDisposable {
		const string transactionName = "CascadeChangeTransaction";
		string propertyName;
		bool isChangingRised;
		IDesignerHost host;
		IComponent ownerComponent;
		DesignerTransaction transaction;		
		IComponentChangeService ChangeServ { get { return (IComponentChangeService)host.GetService(typeof(IComponentChangeService)); } }
		ComponentCollection Components { get { return host.Container.Components; } }
		public CascadeChangeEventHelper(IComponent ownerComponent, IDesignerHost host, string propertyName) {
			this.propertyName = propertyName;
			this.host = host;
			this.ownerComponent = ownerComponent;
			if(ownerComponent is ComponentType) {
				ChangeServ.ComponentChanging += new ComponentChangingEventHandler(OnComponentChanging);
				ChangeServ.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
			}
		}
		public void Dispose() {
			ChangeServ.ComponentChanging -= new ComponentChangingEventHandler(OnComponentChanging);
			ChangeServ.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
		}
		void OnComponentChanging(object sender, ComponentChangingEventArgs e) {
			if(e.Member == null || e.Member.Name != propertyName)
				return;
			HandleEvent(e.Component, true);
		}
		void OnComponentChanged(object sender, ComponentChangedEventArgs e) {
			if(!isChangingRised)
				return;
			HandleEvent(e.Component, false);
		}
		void HandleEvent(object eventComponent, bool isChanging) {
			if(host.TransactionDescription == transactionName || eventComponent != ownerComponent) 
				return;
			transaction = host.CreateTransaction(transactionName);
			try {
				foreach(IComponent component in Components) {
					if(component is ComponentType) {
						if(isChanging)
							XRControlDesignerBase.RaiseComponentChanging(ChangeServ, component, propertyName);
						else
							XRControlDesignerBase.RaiseComponentChanged(ChangeServ, component);
					}
				}
			} finally {
				transaction.Commit();
				isChangingRised = isChanging;
			}
		}
	}
}
