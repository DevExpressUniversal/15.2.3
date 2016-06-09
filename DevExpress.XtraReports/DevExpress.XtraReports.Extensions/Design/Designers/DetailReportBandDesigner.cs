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
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using DevExpress.Data.Browsing.Design;
using System.ComponentModel.Design;
using System.Collections;
using DevExpress.Data.Browsing;
using DevExpress.XtraReports.Design.MouseTargets;
using DevExpress.XtraReports.UserDesigner;
namespace DevExpress.XtraReports.Design {
	public class DetailReportBandDesigner : XRControlDesignerBase, IDesignFrame {
		CascadeChangeEventHelper<DetailReportBand> levelPropertyChangeHandler;
		public override Band Band { get { return (Band)Component; } }
		public bool Expanded {
			get { return ((IDesignFrame)this).Band.Expanded; }
			set {
				if(Expanded == value)
					return;
				RaiseExpandedChanging(this);
				((IDesignFrame)this).Band.Expanded = value;
				RaiseExpandedChanged(this);
			}
		}
		public virtual string Text {
			get {
				using(DataContext dataContext = new DataContext(true)) {
					string dataMember = dataContext.GetDataMemberDisplayName(DetailReport.DataSource, String.Empty, DetailReport.DataMember);
					return DetailReport.DataMember.Length == 0 ? DetailReport.Site.Name :
						String.Format("{0} - \"{1}\"", DetailReport.Site.Name, dataMember);
				}
			}
		}
		public DetailReportBand DetailReport { get { return (DetailReportBand)Component; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new ReportBaseDesignerActionList(this));
			list.Add(new ReportBaseDesignerActionList2(this));
			list.Add(new XRFormattingControlDesignerActionList(this));
			list.Add(new ReportBaseDesignerActionList3(this));
		}
		public override bool CanDragInReportExplorer { get { return false; } }
		protected override bool CanDrop(Type type) {
			return false;
		}
		protected override XRControl FindParent() {
			return ReportDesigner.ActiveReport;
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			Verbs.Add(CreateXRDesignerVerb(DesignSR.Verb_EditBands, ReportCommand.VerbEditBands));
			levelPropertyChangeHandler = new CascadeChangeEventHelper<DetailReportBand>(component, DesignerHost, XRComponentPropertyNames.Level);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if(DetailReport.DataMember.Length == 0)
				return;
			if(DetailReport.DataSource == null && DetailReport.Parent != null)
				DetailReport.DataSource = ((XtraReportBase)DetailReport.Parent).DataSource;
			ValidateDataAdapter();
		}
		public override void OnComponentChanged(ComponentChangedEventArgs e) {
			base.OnComponentChanged(e);
			if(e.Member != null) {
				if(e.Member.Name == XRComponentPropertyNames.DataSource || e.Member.Name == XRComponentPropertyNames.DataMember)
					ValidateDataAdapter();
			}
		}
		protected virtual void ValidateDataAdapter() {
			DetailReport.DataAdapter = DataAdapterHelper.ValidateDataAdapter(fDesignerHost, DetailReport.DataSource, DetailReport.DataMember);
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(levelPropertyChangeHandler != null) {
						levelPropertyChangeHandler.Dispose();
						levelPropertyChangeHandler = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		protected internal override bool CanAddBand(BandKind bandKind) {
			if(IsEUD && DetailReport.LockedInUserDesigner)
				return false;
			return DetailReport.Bands.CanAdd(bandKind);
		}
	}
}
