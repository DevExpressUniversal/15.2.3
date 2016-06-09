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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using DevExpress.Services.Internal;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Menu;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.SyntaxEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Office.Drawing;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.SyntaxEdit {
	#region SyntaxEditControl
	[DXToolboxItem(false)]
	public class SimpleEditControl : RichEditControl {
		public event EventHandler InnerSelectionChanged {
			add {
				DocumentModel.InnerSelectionChanged += value;
			}
			remove {
				DocumentModel.InnerSelectionChanged -= value;
			}
	   }
		public SimpleEditControl()
			: this(false) {
		}
		protected SimpleEditControl(bool skipInitialize)
			: base(false) {
			if(!skipInitialize)
				Initialize();
		}
		public void SelectAllReversed() {
			DocumentModel.BeginUpdate();
			try {
				DocumentModel.Selection.Start = DocumentModel.ActivePieceTable.DocumentEndLogPosition;
				DocumentModel.Selection.End = DocumentModel.ActivePieceTable.DocumentStartLogPosition;
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		public int GetCharIndexFromPoint(Point point) {
			Point clientPoint = PointToClient(point);
			int x = ActiveView.DocumentLayout.UnitConverter.PixelsToLayoutUnits(clientPoint.X);
			int y = ActiveView.DocumentLayout.UnitConverter.PixelsToLayoutUnits(clientPoint.Y);
			RichEditHitTestRequest request = new RichEditHitTestRequest(ActiveView.DocumentModel.ActivePieceTable) {
				PhysicalPoint = clientPoint,
				LogicalPoint = new Point(x, y),
				Accuracy = HitTestAccuracy.NearestCharacter,
				DetailsLevel = DocumentLayoutDetailsLevel.Character
			};
			RichEditHitTestResult result = new RichEditHitTestResult(ActiveView.DocumentLayout, ActiveView.DocumentModel.ActivePieceTable);
			ActiveView.HitTest(ActiveView.DocumentLayout.Pages.First, request, result);
			DocumentModelPosition position = PositionConverter.ToDocumentModelPosition(ActiveView.DocumentModel.ActivePieceTable, result.Character.StartPos);
			return ((IConvertToInt<DocumentLogPosition>)position.LogPosition).ToInt();
		}
		protected void Initialize() {
			InitializeComponent();
			AdjustDocument();
			SubscribeEvents();
		}
		protected internal virtual void InitializeComponent() {
			this.ActiveViewType = RichEditViewType.Simple;
			this.Views.SimpleView.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
			this.Options.VerticalScrollbar.Visibility = RichEditScrollbarVisibility.Visible;
			this.Options.HorizontalScrollbar.Visibility = RichEditScrollbarVisibility.Hidden;
			this.BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder;
		}
		protected virtual void AdjustDocument() {
		}
		protected virtual void SubscribeEvents() {
			this.PopupMenuShowing += OnPopupMenuShowing;
			this.DocumentModel.DocumentCleared += OnDocumentModelCleared;
		}
		void OnDocumentModelCleared(object sender, EventArgs e) {
			AdjustDocument();
		}
		protected virtual void UnsubscribeEvents() {
			this.PopupMenuShowing -= OnPopupMenuShowing;
			this.DocumentModel.DocumentCleared -= OnDocumentModelCleared;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeEvents();
			}
			base.Dispose(disposing);
		}
		void OnPopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
			DXMenuItemCollection items = e.Menu.Items;
			items.Clear();
			items.Add(CreateItem(RichEditCommandId.CutSelection));
			items.Add(CreateItem(RichEditCommandId.CopySelection));
			items.Add(CreateItem(RichEditCommandId.PasteSelection));
			items.Add(CreateItem(RichEditCommandId.Find, true));
			items.Add(CreateItem(RichEditCommandId.Replace));
		}
		DXMenuItem CreateItem(RichEditCommandId commandID) {
			return CreateItem(commandID, false);
		}
		DXMenuItem CreateItem(RichEditCommandId commandID, bool beginGroup) {
			RichEditMenuItemCommandWinAdapter adapter = new RichEditMenuItemCommandWinAdapter(this.CreateCommand(commandID));
			DXMenuItem item = (DXMenuItem)adapter.CreateMenuItem(DXMenuItemPriority.Normal);
			item.BeginGroup = beginGroup;
			return item;
		}
	}
	public class SyntaxEditControl : SimpleEditControl, ISyntaxEditControl {
		#region fields
		readonly ISyntaxColors syntaxColors;
		ISyntaxHelper syntaxHelper;
		#endregion
		public SyntaxEditControl(ISyntaxColors syntaxColors) : base(true) {
			this.syntaxColors = syntaxColors;
			Initialize();
		}
		#region properties
		public ISyntaxColors SyntaxColors { get { return syntaxColors; } }
		public ISyntaxHelper SyntaxHelper {
			get { return syntaxHelper; }
			set {
				syntaxHelper = value;
				ReplaceService<IContentChangedNotificationService>(syntaxHelper);
				ReplaceService<ISyntaxCheckService>(syntaxHelper);
				if (InnerControl != null)
					InnerControl.OnSpellCheckerChanged();
			}
		}
		#endregion
		protected internal override void InitializeComponent() {
			ReplaceService<IRichEditCommandFactoryService>(new SyntaxEditControlCommandFactoryService(this, GetService<IRichEditCommandFactoryService>()));
			ReplaceService<ICommandUIStateManagerService>(new SyntaxEditorCommandUIStateManagerService());
			this.Font = new Font("Courier New", 10);
			this.Options.HorizontalRuler.Visibility = RichEditRulerVisibility.Hidden;
			this.ActiveViewType = RichEditViewType.Draft;
			this.Views.DraftView.Padding = new System.Windows.Forms.Padding(75, 0, 0, 0);
			this.Views.DraftView.AllowDisplayLineNumbers = true;
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			this.DocumentLoaded += OnDocumentContentLoaded;
			this.CustomDrawActiveView += OnCustomDrawActiveView;
			this.LookAndFeel.StyleChanged += OnStyleChanged;
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			this.DocumentLoaded -= OnDocumentContentLoaded;
			this.CustomDrawActiveView -= OnCustomDrawActiveView;
			this.LookAndFeel.StyleChanged -= OnStyleChanged;
		}
		#region custom drawing
		protected internal override RichEditViewRepository CreateViewRepository() {
			return new SyntaxEditorRichEditViewRepository(this);
		}
		#endregion
		void OnStyleChanged(object sender, EventArgs e) {
			this.Views.DraftView.BackColor = SyntaxColors.BackgroundColor;
			NotifyContentChanged();
		}
		void OnDocumentContentLoaded(object sender, EventArgs e) {
			NotifyContentChanged();
		}
		void OnCustomDrawActiveView(object sender, RichEditViewCustomDrawEventArgs e) {
			int left = -10;
			int bottom = (int)e.Cache.Graphics.ClipBounds.Bottom;
			Pen pen = new Pen(SyntaxColors.LineColor);
			pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			e.Cache.Graphics.DrawLine(pen, new Point(left, 0), new Point(left, bottom));
			if (SyntaxHelper != null)
				SyntaxHelper.DrawHighlightMatchingBrackets(e.Cache.Graphics);
		}
		protected override void AdjustDocument() {
			this.Document.Sections[0].Page.Width = 80000;
			this.Document.Sections[0].Page.PaperKind = PaperKind.Custom;
			this.Document.Sections[0].LineNumbering.CountBy = 1;
			this.Document.Sections[0].LineNumbering.RestartType = DevExpress.XtraRichEdit.API.Native.LineNumberingRestart.Continuous;
			this.Document.CharacterStyles["Line Number"].FontName = this.Font.Name;
			this.Document.CharacterStyles["Line Number"].ForeColor = SyntaxColors.LineNumbersColor;
			this.Document.CharacterStyles["Line Number"].FontSize = this.Font.Size;
			this.Views.DraftView.BackColor = SyntaxColors.BackgroundColor;
		}
		public void SetCaretPosition(int line, int column) {
			this.Document.CaretPosition = this.Document.CreatePosition(this.Document.Paragraphs[line].Range.Start.ToInt() + column);
			this.ScrollToCaret();
			this.Focus();
		}
		internal void NotifyContentChanged() {
			if (SyntaxHelper != null)
				SyntaxHelper.NotifyContentChanged();
		}
		protected internal void ShowErrorsCore(CompilerErrorCollection errors) {
			if (SyntaxHelper != null)
				SyntaxHelper.MarkErrors(errors);
		}
	}
	#endregion
	public class SyntaxEditorRichEditViewRepository : WinFormsRichEditViewRepository {
		public SyntaxEditorRichEditViewRepository(RichEditControl control)
			: base(control) {
		}
		protected internal override DraftView CreateDraftView() {
			return new SyntaxEditorDraftView(Control);
		}
	}
	public class SyntaxEditorDraftView : DraftView {
		public SyntaxEditorDraftView(IRichEditControl control)
			: base(control) {
		}
		protected internal override DocumentLayoutExporter CreateDocumentLayoutExporter(Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, DevExpress.XtraRichEdit.Internal.PrintLayout.PageViewInfo pageViewInfo, Rectangle bounds) {
			return new SyntaxEditDocumentLayoutExporter(((SyntaxEditControl)this.Control), this.DocumentLayout.Measurer, this.DocumentModel, painter, adapter, bounds);
		}
	}
}
