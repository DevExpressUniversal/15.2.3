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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Design.MouseTargets;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design {
	[MouseTarget(typeof(XRTableOfContentsMouseTarget))]
	public class XRTableOfContentsDesigner : XRTextControlBaseDesigner {
		protected XRTableOfContents TableOfContents {
			get { return Component as XRTableOfContents; }
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRTableOfContentsDesignerActionList(this));
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			TableOfContents.Controls.UpdateLayout();
		}
		protected override string[] GetFilteredProperties() {
			return new string[] { XRComponentPropertyNames.FormattingRules, XRComponentPropertyNames.DataBindings, XRComponentPropertyNames.NavigateUrl, 
				XRComponentPropertyNames.Target,  XRComponentPropertyNames.Text ,XRComponentPropertyNames.XmlDataPath, XRComponentPropertyNames.WordWrap,
				XRComponentPropertyNames.TextAlignment, XRComponentPropertyNames.Bookmark, XRComponentPropertyNames.BookmarkParent,
				XRComponentPropertyNames.Scripts, XRComponentPropertyNames.SnapLineMargin, XRComponentPropertyNames.XlsxFormatString, XRComponentPropertyNames.AnchorVertical,
				XRComponentPropertyNames.KeepTogether, XRComponentPropertyNames.Font
			};
		}
		protected override void PreFilterEvents(System.Collections.IDictionary events) {
			base.PreFilterEvents(events);
			events.Clear();
		}
		protected override SelectionRules GetSelectionRulesCore() {
			return SelectionRules.BottomSizeable | SelectionRules.Moveable;
		}
		protected override InplaceTextEditorBase GetInplaceEditor(string text, bool selectAll) {
			return new InplaceTableOfContentsEditor(fDesignerHost, TableOfContents, TableOfContents.LevelTitle.Text, selectAll);
		}
		public override RectangleF GetEditorScreenBounds() {
			RectangleF rectangle = base.GetEditorScreenBounds();
			rectangle.Height = ZoomService.GetInstance(Component.Site).ToScaledPixels(TableOfContents.LevelTitle.Height - 2f, TableOfContents.Dpi);
			return rectangle;
		}
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			if(TableOfContents.Parent == null)
				InsertXRTableOfContentsIntoBand(BandKind.ReportHeader, BandCommands.InsertReportHeaderBand);
			if(TableOfContents.Parent == null)
				InsertXRTableOfContentsIntoBand(BandKind.ReportFooter, BandCommands.InsertReportFooterBand);
			if(TableOfContents.Parent == null) {
				Exception ex = new Exception(ReportStringId.Msg_InvalidXrTocInstance.GetString());
				NotificationService.ShowException<XtraReport>(LookAndFeelProviderHelper.GetLookAndFeel(fDesignerHost), fDesignerHost.GetOwnerWindow(), ex);
				throw ex;
			}
			base.InitializeNewComponent(defaultValues);
		}
		protected override void InitializeParent() {
			base.InitializeParent();
			if(TableOfContents.Parent == null)
				throw new XRTableOfContentsException(ReportStringId.Msg_InvalidXrTocInstanceInBand.GetString());
		}
		void InsertXRTableOfContentsIntoBand(BandKind bandKind, CommandID command) {
			if(RootReport.Bands.CanAdd(bandKind)) {
				var menuCommandService = fDesignerHost.GetService<IMenuCommandService>();
				ISelectionService serv = GetService(typeof(ISelectionService)) as ISelectionService;
				serv.SetSelectedComponents(new object[] { RootReport }, SelectionTypes.Replace);
				menuCommandService.GlobalInvoke(command);
			}
			var band = RootReport.Bands[bandKind];
			if(band.CanAddControl(typeof(XRTableOfContents), TableOfContents)) {
				if(band.Controls.Count != 0)
					TableOfContents.TopF = Math.Max(TableOfContents.TopF, band.Controls.OfType<XRControl>().Max(item => item.BottomF));
				band.HeightF += Math.Max(0f, TableOfContents.BottomF - band.HeightF);
				XRControlDesignerBase.RaiseCollectionChanging(band, changeService);
				band.Controls.Add(TableOfContents);
				XRControlDesignerBase.RaiseCollectionChanged(band, changeService);
			}
		}
	}
	public class XRTableOfContentsDesignerActionList : XRControlBaseDesignerActionList {
		[Editor(typeof(XRTableOfContentsLevelCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public XRTableOfContentsLevelCollection Levels {
			get { return ((XRTableOfContents)Component).Levels; }
		}
		public XRTableOfContentsDesignerActionList(XRTableOfContentsDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "Levels", "Levels");
		}
	}
	class XRTableOfContentsMouseTarget : ControlMouseTarget {
		XRTableOfContentsSplitService tableOfContentsSplitService;
		XRTableOfContentsLevelBase hitLevel;
		Point startPos = Point.Empty;
		new XRTableOfContentsDesigner Designer {
			get { return (XRTableOfContentsDesigner)base.Designer; }
		}
		XRTableOfContents TableOfContents {
			get { return Designer.Component as XRTableOfContents; }
		}
		Cursor SplitCursor {
			get { return Cursors.SizeNS; }
		}
		public XRTableOfContentsMouseTarget(XRControl xrControl, IServiceProvider servProvider)
			: base(xrControl, servProvider) {
		}
		public override bool ContainsPoint(Point pt, BandViewInfo viewInfo) {
			return hitLevel != null || base.ContainsPoint(pt, viewInfo);
		}
		public override void HandleMouseDown(object sender, BandMouseEventArgs e) {
			if(Designer.IsInplaceEditingMode)
				Designer.CommitInplaceEditor();
			else if(XRControlDesignerBase.CursorEquals(SplitCursor) && hitLevel != null) {
				if(tableOfContentsSplitService != null)
					tableOfContentsSplitService.EndServiceInternal(true);
				tableOfContentsSplitService = new XRTableOfContentsSplitService(this.Host, () => {
					RectangleF rect = BandViewSvc.GetControlScreenBounds(TableOfContents);
					rect.Y = Control.MousePosition.Y;
					rect.Height = 2;
					return new RectangleF[] { rect };
				});
				startPos = Control.MousePosition;
				tableOfContentsSplitService.Start(BandViewSvc.View, null);
				return;
			}
			base.HandleMouseDown(sender, e);
		}
		public override void HandleMouseUp(object sender, BandMouseEventArgs e) {
			if(XRControlDesignerBase.CursorEquals(SplitCursor) && hitLevel != null) {
				IComponentChangeService serv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				PropertyDescriptor memberDescriptor = XRAccessor.GetPropertyDescriptor(TableOfContents, "Levels");
				serv.OnComponentChanging(TableOfContents, memberDescriptor);
				float delta = 0;
				if(startPos.Y != 0)
					delta = Control.MousePosition.Y - startPos.Y;
				hitLevel.Height = Math.Max(0, hitLevel.Height + XRConvert.Convert(delta, GraphicsDpi.Pixel, TableOfContents.Dpi));
				serv.OnComponentChanged(TableOfContents, memberDescriptor, null, null);
				hitLevel = null;
			}
			base.HandleMouseUp(sender, e);
		}
		protected override Cursor GetCursor(Point pt) {
			hitLevel = null;
			ISelectionService serv = GetService(typeof(ISelectionService)) as ISelectionService;
			if(!serv.GetComponentSelected(TableOfContents) || serv.SelectionCount > 1 || Designer.IsInplaceEditingMode)
				return base.GetCursor(pt);
			const int delta = 3;
			Cursor result = null;
			RectangleF controlBounds = BandViewSvc.GetControlViewClientBounds(TableOfContents);
			TableOfContents.ForEachLevel((level, levelBounds) => {
				if(level != TableOfContents.LevelDefault) {
					levelBounds = ZoomService.GetInstance(TableOfContents.Site).ToScaledPixels(levelBounds, TableOfContents.Dpi);
					levelBounds.Offset(controlBounds.Location);
					if(Math.Abs(pt.Y - levelBounds.Bottom) < delta) {
						result = SplitCursor;
						hitLevel = level;
						return true;
					}
				}
				return false;
			});
			return result ?? base.GetCursor(pt);
		}
		public override void HandleDoubleClick(object sender, BandMouseEventArgs e) {
			base.HandleDoubleClick(sender, e);
			Designer.ShowInplaceEditor(true);
		}
	}
	class XRTableOfContentsSplitService : CapturePaintService {
		Func<RectangleF[]> getRects;
		public XRTableOfContentsSplitService(IDesignerHost host, Func<RectangleF[]> getRects)
			: base(host) {
			this.getRects = getRects;
		}
		protected override RectangleF[] GetRects() {
			return getRects();
		}
	}
	class XRTableOfContentsLevelCollectionEditor : DevExpress.Utils.UI.CollectionEditor {
		public XRTableOfContentsLevelCollectionEditor(Type type)
			: base(type) {
		}
		protected override Utils.UI.CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			return new TableOfContentsLevelCollectionEditorForm(serviceProvider, this);
		}
		protected override string GetItemName(object item, int index) {
			return String.Format("{0} {1}", base.GetItemName(item, index), index + 1);
		}
	}
	public class InplaceTableOfContentsEditor : InplaceTextEditor {
		XRTableOfContents Owner { get { return xrControl as XRTableOfContents; } }
		public InplaceTableOfContentsEditor(IDesignerHost designerHost, XRFieldEmbeddableControl control, string text, bool selectAll)
			: base(designerHost, control, text, selectAll) { }
		protected override void CommitChanges() {
			try {
				if(Owner != null) {
					PropertyDescriptor textPropertyDescriptor = TypeDescriptor.GetProperties(Owner.LevelTitle)["Text"];
					if(textPropertyDescriptor != null)
						textPropertyDescriptor.SetValue(Owner.LevelTitle, Control.Text);
				}
			} catch { }
		}
		protected override void UpdateText() {
			Control.Text = Owner.LevelTitle.Text;
		}
	}
	public class XRTableOfContentsException : Exception {
		public XRTableOfContentsException() {}
		public XRTableOfContentsException(string message) : base(message) { }
	}
}
