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
using DevExpress.XtraPrinting;
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.PageBuilder;
using System.Collections.Generic;
namespace DevExpress.XtraPrintingLinks {
	[DefaultProperty("Links")]
	public class CompositeLinkBase : LinkBase {
		private class InnerLinkCollection : LinkCollection {
			private CompositeLinkBase owner;
			internal PrintingSystemBase PrintingSystem {
				get { return ps; }
				set {
					ps = value;
					foreach(LinkBase link in this)
						link.PrintingSystemBase = ps;
				}
			}
			internal InnerLinkCollection(CompositeLinkBase owner)
				: base() {
				this.owner = owner;
			}
			protected override void OnInsertComplete(int index, object item) {
				base.OnInsertComplete(index, item);
				((LinkBase)item).Owner = owner;
			}
		}
		private int breakSpace;
		private InnerLinkCollection links;
		List<IDisposable> garbageContainer = new List<IDisposable>();
		[
		Category(NativeSR.CatPrinting),
#if !SL
	DevExpressPrintingCoreLocalizedDescription("CompositeLinkBaseBreakSpace"),
#endif
		DefaultValue(0)
		]
		public int BreakSpace {
			get { return breakSpace; }
			set { breakSpace = value; }
		}
		[
		Category(NativeSR.CatPrinting),
#if !SL
	DevExpressPrintingCoreLocalizedDescription("CompositeLinkBaseLinks"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraPrintingLinks.Design.LinkSelectionEditor," + AssemblyInfo.SRAssemblyPrintingDesign, typeof(System.Drawing.Design.UITypeEditor))
		]
		public LinkCollection Links {
			get {
				if(links == null) {
					links = new InnerLinkCollection(this);
				}
				return links;
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("CompositeLinkBasePrintingSystemBase")]
#endif
		public override PrintingSystemBase PrintingSystemBase {
			get { return ps; }
			set {
				ps = value;
				((InnerLinkCollection)Links).PrintingSystem = ps;
			}
		}
		protected override BrickModifier InternalSkipArea {
			get { return BrickModifier.None; }
		}
		public CompositeLinkBase()
			: base() {
		}
		public CompositeLinkBase(PrintingSystemBase ps)
			: base(ps) {
			((InnerLinkCollection)Links).PrintingSystem = ps;
		}
		public CompositeLinkBase(System.ComponentModel.IContainer container)
			: base(container) {
		}
		public void CreatePageForEachLink() {
			this.PrintingSystemBase.Begin();
			this.PrintingSystemBase.End(this);
			ClearGarbage();
			foreach(LinkBase link in Links) {
				PrintingSystemBase ps = new PrintingSystemBase();
				garbageContainer.Add(ps);
				ps.SetDocument(new SinglePageLinkDocument(ps, delegate() { }));
				link.Owner = null;
				link.CreateDocument(ps);
				link.Owner = this;
				if(ps.CancelPending) {
					this.PrintingSystemBase.Cancel();
					break;
				}
				if(ps.PageCount > 0)
					this.PrintingSystemBase.Pages.Add(ps.Pages[0]);
			}
		}
		void ClearGarbage() {
			foreach(IDisposable item in garbageContainer)
				item.Dispose();
			garbageContainer.Clear();
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				ClearGarbage();
			base.Dispose(disposing);
		}
		protected void AddSubreport(LinkBase link, PointF offset) {
			link.PrintingSystemBase = ps;
			BrickModifier saveSkipArea = link.SkipArea;
			link.SkipArea = this.SkipArea;
			link.AddSubreport(offset);
			link.SkipArea = saveSkipArea;
		}
		protected override void CreateDetail(BrickGraphics gr) {
			if(Links.Count == 0)
				throw new Exception("The Links property value must not be empty");
			for(int i = 0; i < Links.Count; i++) {
				Links[i].PrintingSystemBase = ps;
				AddSubreport(Links[i], i == 0 ? PointF.Empty : new PointF(0, breakSpace));
			}
		}
	}
}
