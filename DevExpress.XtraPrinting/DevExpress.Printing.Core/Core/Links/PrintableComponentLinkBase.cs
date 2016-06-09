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
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.DataNodes;
using DevExpress.XtraPrinting;
using System.IO;
using DevExpress.Utils;
namespace DevExpress.XtraPrintingLinks {
	[DefaultProperty("Component")]
	public class PrintableComponentLinkBase : LinkBase {
		private IPrintable printable;
		protected override string InnerPageHeader { get { return printable is IPrintHeaderFooter ? ((IPrintHeaderFooter)printable).InnerPageHeader : string.Empty; } }
		protected override string InnerPageFooter { get { return printable is IPrintHeaderFooter ? ((IPrintHeaderFooter)printable).InnerPageFooter : string.Empty; } }
		protected override string ReportHeader { 
			get {
				if(printable is IPrintHeaderFooter) {
					IPrintHeaderFooter printHF = (IPrintHeaderFooter)printable;
					if(!string.IsNullOrEmpty(printHF.ReportHeader))
						return printHF.ReportHeader;
				}
				return base.ReportHeader;
			}
		}
		protected override string ReportFooter { 
			get {
				if(printable is IPrintHeaderFooter) {
					IPrintHeaderFooter printHF = (IPrintHeaderFooter)printable;
					if(!string.IsNullOrEmpty(printHF.ReportFooter))
						return printHF.ReportFooter;
				}
				return base.ReportFooter; 
			}
		}
		public override Type PrintableObjectType {
			get { return typeof(IPrintable); }
		}
		[
		Category(NativeSR.CatPrinting),
		DefaultValue(null),
		]
		public virtual IPrintable Component {
			get { return printable; }
			set { printable = value; }
		}
		protected override BrickModifier InternalSkipArea {
			get {
				return BrickModifier.None;
			}
		}
		protected IPrintableEx ComponentEx { get { return printable as IPrintableEx; } }
		public PrintableComponentLinkBase()
			: base() {
		}
		public PrintableComponentLinkBase(PrintingSystemBase ps)
			: base(ps) {
		}
		public PrintableComponentLinkBase(System.ComponentModel.IContainer container)
			: base(container) {
		}
		public override void SetDataObject(object data) {
			if(data is IPrintable)
				printable = data as IPrintable;
		}
		public override void CreateDocument(bool buildPagesInBackground) {
			base.CreateDocument(buildPagesInBackground);
#if DEBUGTEST
			AssertIntersectedBricks();
#endif
		}
#if DEBUGTEST
		protected void AssertIntersectedBricks() {
			if(PrintingSystemBase == null || PrintableCreatesIntersectedBricks)
				return;
			PrintingSystemBase.AssertIntersectedBricks();
		}
		bool PrintableCreatesIntersectedBricks {
			get { return printable != null && printable.CreatesIntersectedBricks; }
		}
#endif
		public override void AddSubreport(PointF offset) {
			if(printable != null)
				base.AddSubreport(offset);
		}
		protected override void BeforeCreate() {
			if(Component == null)
				throw new NullReferenceException("The Component property value must not be null");
			base.BeforeCreate();
			ps.Graph.PageUnit = GraphicsUnit.Pixel;
			printable.Initialize(ps, this);
			ps.Document.CorrectImportBrickBounds = printable.CreatesIntersectedBricks;
			if(printable.HasPropertyEditor())
				EnableCommand(PrintingSystemCommand.Customize, true);
		}
		internal override void BeforeDestroy() {
			if(printable != null)
				printable.Finalize(ps, this);
			base.BeforeDestroy();
		}
		protected override void CreateMarginalHeader(BrickGraphics graph) {
			base.CreateMarginalHeader(graph);
			printable.CreateArea(SR.MarginalHeader, ps.Graph);
		}
		protected override void CreateMarginalFooter(BrickGraphics graph) {
			base.CreateMarginalFooter(graph);
			printable.CreateArea(SR.MarginalFooter, ps.Graph);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event CreateAreaEventHandler CreateInnerPageHeaderArea {
			add {} remove {}
		}
		protected override void CreateInnerPageHeader(BrickGraphics graph) {
			printable.CreateArea(SR.InnerPageHeader, ps.Graph);
			base.CreateInnerPageHeader(graph);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event CreateAreaEventHandler CreateInnerPageFooterArea {
			add { } remove { }
		}
		protected override void CreateInnerPageFooter(BrickGraphics graph) {
			printable.CreateArea(SR.InnerPageFooter, ps.Graph);
			base.CreateInnerPageFooter(graph);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event CreateAreaEventHandler CreateDetailHeaderArea {
			add { } remove { }
		}
		protected override void CreateDetailHeader(BrickGraphics graph) {
			printable.CreateArea(SR.DetailHeader, ps.Graph);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event CreateAreaEventHandler CreateDetailFooterArea {
			add { } remove { }
		}
		protected override void CreateDetailFooter(BrickGraphics graph) {
			printable.CreateArea(SR.DetailFooter, ps.Graph);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event CreateAreaEventHandler CreateDetailArea {
			add { } remove { }
		}
		protected override void CreateDetail(BrickGraphics graph) {
			printable.CreateArea(SR.Detail, ps.Graph);
		}
		protected override void CreateReportHeader(BrickGraphics graph) {
			printable.CreateArea(SR.ReportHeader, ps.Graph);
			base.CreateReportHeader(graph);
		}
		protected override void CreateReportFooter(BrickGraphics graph) {
			printable.CreateArea(SR.ReportFooter, ps.Graph);
			base.CreateReportFooter(graph);
		}
		protected override void DisableCommands() {
			base.DisableCommands();
			EnableCommand(PrintingSystemCommand.Customize, false);
		}
		protected override bool CanHandleCommandInternal(PrintingSystemCommand command, IPrintControl printControl) {
			return base.CanHandleCommandInternal(command, printControl) || command == PrintingSystemCommand.Customize;
		}
		protected override void OnStartActivity() {
			if(ComponentEx != null) ComponentEx.OnStartActivity();
		}
		protected override void OnEndActivity() {
			if(ComponentEx != null) ComponentEx.OnEndActivity();
		}
	}
}
