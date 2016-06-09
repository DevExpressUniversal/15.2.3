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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.ViewInfo;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
namespace DevExpress.XtraLayout.Printing {
	public class LayoutGroupPrintViewInfo : LayoutGroupViewInfo {
		public LayoutGroupPrintViewInfo(LayoutGroupViewInfo vi)
			: base(vi.Owner) {
		}
	}
	public class LayoutControlPrinter : BaseVisitor, IDisposable {
		protected IPrintingSystem ps;
		protected ILink link;
		protected LayoutControl owner;
		protected BrickGraphics graph;
		protected Hashtable originalStateStore;
		public LayoutControlPrinter(LayoutControl owner) {
			this.owner = owner;
			globalX = 0;
			globalY = 0;
			originalStateStore = new Hashtable();
		}
		public SizeF ClientPageSize {
			get { return (SizeF)graph.ClientPageSize; }
		}
		LayoutGroupPrintViewInfo viewInfo;
		public virtual void Dispose() {
		}
		public void Release() {
		}
		const int indent = 10;
		protected int MaximumWidth {
			get {
				if(graph == null) return 1;
				SizeF size = (SizeF)graph.ClientPageSize;
				return (int)size.Width - indent;
			}
		}
		protected OptionsPrintControl GetOptionsPrint() {
			return GetOptionsPrint(this.viewInfo);
		}
		protected OptionsPrintControl GetOptionsPrint(LayoutGroupViewInfo viewInfo) {
			if(viewInfo == null) return null;
			if(viewInfo.Owner == null) return null;
			if(viewInfo.Owner.Owner == null) return null;
			if(!(viewInfo.Owner.Owner.Control is DevExpress.XtraLayout.LayoutControl)) return null;
			return ((LayoutControl)viewInfo.Owner.Owner.Control).OptionsPrint;
		}
		public virtual void Initialize(IPrintingSystem ps, ILink link, LayoutGroupViewInfo viewInfo) {
			if(this.ps != ps) this.ps = ps;
			this.link = link;
			this.viewInfo = new LayoutGroupPrintViewInfo(viewInfo);
			ViewInfo.CalculateViewInfo();
			OptionsPrintControl options = GetOptionsPrint();
			if(options != null && options.AllowFitToPage) {
				ps.AutoFitToPagesWidth = 1;
			}
			originalStateStore.Clear();
		}
		protected LayoutGroupPrintViewInfo ViewInfo { get { return viewInfo; } }
		Control oldParent;
		int oldWidth;
		int height = -1;
		Point oldLocation;
		DockStyle ds;
		bool allowFitPage = false;
		public virtual void CreateArea(string areaName, IBrickGraphics graph) {
			this.graph = (BrickGraphics)graph;
			switch(areaName) {
				case SR.MarginalHeader:
					if(allowFitPage) {
						if(owner.Root.Width > MaximumWidth && owner.Root.MinSize.Width < MaximumWidth) {
							oldParent = owner.Parent;
							oldWidth = owner.Width;
							oldLocation = owner.Location;
							ds = owner.Dock;
							owner.Parent = null;
							owner.Width = MaximumWidth;
							if(owner.Height < owner.Root.MinSize.Height) {
								height = owner.Height;
								owner.Height = owner.Root.MinSize.Height;
							}
							this.viewInfo = new LayoutGroupPrintViewInfo(owner.Root.ViewInfo);
							ViewInfo.CalculateViewInfo();
						}
					}
					CreateHeader();
					break;
				case SR.Detail:
					CreateDetails();
					break;
				case SR.DetailHeader:
					CreateDetailHeader();
					break;
				case SR.ReportFooter:
					if(allowFitPage) {
						if(oldParent != null) {
							owner.Parent = oldParent;
							owner.Width = oldWidth;
							if(height > 0) { owner.Height = height; }
							owner.Location = oldLocation;
							owner.Dock = ds;
							oldParent.Update();
						}
					}
					break;
			}
		}
		protected virtual void CreateHeader() { }
		protected virtual IVisualBrick CreateBrick(Rectangle bounds) {
			IVisualBrick brick = (IVisualBrick)ps.CreateBrick("TextBrick");
			((IBrickGraphics)graph).DrawBrick(brick, bounds);
			return brick;
		}
		protected virtual int globalX { get; set; }
		protected virtual int globalY { get; set; }
		public override void EndVisit(BaseLayoutItem item) {
			LayoutClassificationArgs lca = LayoutClassifier.Default.Classify(item);
			if(lca.IsTabbedGroup) {
				if(originalStateStore.Contains(lca.TabbedGroup)) {
					SelectTabPage(lca.TabbedGroup, originalStateStore[lca.TabbedGroup] as LayoutGroup);
					originalStateStore.Remove(lca.TabbedGroup);
				}
			}
		}
		protected virtual void SelectTabPage(TabbedGroup tGroup, LayoutGroup tabPage) {
			tGroup.SelectedTabPage = tabPage;
		}
		protected void UpdateChildren(BaseLayoutItem bli, bool visible) {
			bli.UpdateChildren(visible);
		}
		public override bool StartVisit(BaseLayoutItem item) {
			LayoutClassificationArgs lca = LayoutClassifier.Default.Classify(item);
			if(lca.IsGroup) {
				TabbedGroup parentTab = lca.Group.ParentTabbedGroup;
				if(parentTab != null && parentTab.SelectedTabPage != lca.Group) {
					SelectTabPage(parentTab, lca.Group);
					if(parentTab.SelectedTabPage != lca.Group)
						return false;
					if(parentTab.TabPages.IndexOf(lca.Group) > 0)
						globalY += parentTab.ViewInfo.BoundsRelativeToControl.Height;
				}
				if(!AllowPrintTabbedGroup(parentTab) && (parentTab != null) && parentTab.SelectedTabPage != lca.Group)
					return false;
			}
			if(lca.IsTabbedGroup) {
				originalStateStore.Add(lca.TabbedGroup, lca.TabbedGroup.SelectedTabPage);
			}
			return true;
		}
		public override IList ArrangeElements(BaseLayoutItem item) {
			LayoutGroup group = item as LayoutGroup;
			if(group != null) {
				DevExpress.XtraLayout.Customization.LayoutControlWalker walker = new DevExpress.XtraLayout.Customization.LayoutControlWalker(group);
				return walker.ArrangeElements(new OptionsFocus(MoveFocusDirection.AcrossThenDown, false));
			}
			return null;
		}
		protected void GetOfAmpersands(ITextBrick brick) {
			BrickStringFormat bsf = brick.StringFormat;
			bsf.Value.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
		}
		protected ITextBrick CreateTextBrick(String text, BrickStyle bStyle, Color backColor, HorzAlignment hAlignment) {
			return CreateTextBrick(text, bStyle, backColor, hAlignment, VertAlignment.Center, 0);
		}
		protected ITextBrick CreateTextBrick(String text, BrickStyle bStyle, Color backColor, HorzAlignment hAlignment, VertAlignment vAlignment, float angle) {
			ITextBrick brick;
			if(angle == 0)
				brick = (ITextBrick)ps.CreateBrick("TextBrick");
			else {
				brick = (ITextBrick)ps.CreateBrick("LabelBrick");
				((ILabelBrick)brick).Angle = angle;
			}
			brick.Text = StringPainter.Default.RemoveFormat(text);
			brick.Sides = BorderSide.None;
			brick.Style = bStyle;
			if(backColor != Color.Empty) brick.BackColor = backColor;
			if(bStyle.ForeColor == Color.Empty) brick.ForeColor = Color.Black;
			brick.HorzAlignment = hAlignment;
			brick.VertAlignment = vAlignment;
			brick.Padding = new PaddingInfo(0, 0, 0, 0);
			brick.Style.StringFormat.PrototypeKind = BrickStringFormatPrototypeKind.GenericTypographic;
			return brick;
		}
		protected virtual void DrawBrick(IVisualBrick brick, RectangleF rect) {
			rect.Width++;
			rect.Height++;
			rect.X += globalX;
			rect.Y += globalY;
			((IBrickGraphics)graph).DrawBrick(brick, rect);
		}
		protected bool AllowPrintTabbedGroup(TabbedGroup tGroup) {
			if(tGroup == null) return true;
			if(tGroup.Owner == null || tGroup.Owner.RootGroup == null) return false;
			BaseLayoutItem root = FindRoot(tGroup);
			return tGroup.Width == root.ViewInfo.ClientArea.Width;
		}
		protected BaseLayoutItem FindRoot(BaseLayoutItem item) {
			while(item.Parent != null) item = item.Parent;
			return item;
		}
		protected virtual void ProcessGroup(LayoutGroup group) {
			IVisualBrick brick;
			if(group.GroupBordersVisible && group.TextVisible && group.ParentTabbedGroup == null) {
				if(!group.Visible) return;
				RectangleF captionRect;
				GetGroupBrickAndCeptionRect(group, out brick, out captionRect);
				GetOfAmpersands(brick as ITextBrick);
				DrawBrick(brick, GetGroupCaptionRect(group, captionRect));
			}
			if(group.ParentTabbedGroup != null) {
				ProcessTabbedGroup((TabbedControlGroup)group.ParentTabbedGroup, group);
			}
		}
		protected virtual void GetGroupBrickAndCeptionRect(BaseLayoutItem bli, out IVisualBrick brick, out RectangleF captionRect) {
			LayoutGroup group = bli as LayoutGroup;
			if(group != null) {
				captionRect = new RectangleF(new Point(group.ViewInfo.BorderInfo.CaptionBounds.X,
		 group.ViewInfo.BorderInfo.CaptionBounds.Y),
		 group.ViewInfo.BorderInfo.CaptionBounds.Size);
			} else {
				captionRect = RectangleF.Empty;
			}
			string text = bli.Text;
			Locations textLocation = bli.TextLocation;
			AppearanceObject textAppearance;
			if(group != null) { textAppearance = group.PaintAppearanceGroup.AppearanceGroup; } else textAppearance = bli.PaintAppearanceItemCaption;
			if(group != null) ProcessGroupCaption(group, ref text, ref textAppearance);
			if(textLocation == Locations.Default || textLocation == Locations.Top || textLocation == Locations.Bottom) {
				brick = CreateTextBrick(text,
					AppearanceHelper.CreateBrick(textAppearance),
					GetGroupCaptionBackColor(textAppearance),
					textAppearance.TextOptions.HAlignment, textAppearance.TextOptions.VAlignment, 0);
			} else {
				if(group != null) { captionRect.Height = group.ViewInfo.ClientArea.Height; }
				brick = CreateTextBrick(text,
					AppearanceHelper.CreateBrick(textAppearance),
					GetGroupCaptionBackColor(textAppearance),
					textAppearance.TextOptions.HAlignment, textAppearance.TextOptions.VAlignment, textLocation == Locations.Right ? 270 : 90);
			}
		}
		protected virtual void ProcessGroupCaption(LayoutGroup group, ref string text, ref AppearanceObject textAppearance) { }
		protected virtual RectangleF GetGroupCaptionRect(LayoutGroup group, RectangleF captionRect) {
			return captionRect;
		}
		protected virtual Color GetGroupCaptionBackColor(AppearanceObject appearance) {
			Color color = appearance.BackColor;
			if(color == Color.Empty || color == Color.Transparent) color = Color.LightGray;
			return color;
		}
		protected virtual void ProcessTabbedGroup(TabbedControlGroup tgroup, LayoutGroup group) {
			IVisualBrick brick;
			if(!tgroup.Visible) return;
			Rectangle captionRect = captionRect = tgroup.ViewInfo.BorderInfo.Tab.ViewInfo.HeaderInfo.Bounds;
			if(tgroup.TextLocation == Locations.Top || tgroup.TextLocation == Locations.Bottom || tgroup.TextLocation == Locations.Default) {
				captionRect.Width = tgroup.ViewInfo.ClientArea.Width;
				captionRect.X += tgroup.ViewInfo.ClientArea.X;
			} else {
				captionRect.Height = tgroup.ViewInfo.ClientArea.Height;
			}
			brick = CreateTabbedBrick(group, tgroup);
			GetOfAmpersands(brick as ITextBrick);
			DrawBrick(brick, captionRect);
		}
		protected virtual IVisualBrick CreateTabbedBrick(LayoutGroup group, TabbedControlGroup tgroup) {
			IVisualBrick brick;
			brick = CreateTextBrick(group.Text, AppearanceHelper.CreateBrick(group.AppearanceGroup), Color.LightGray, HorzAlignment.Center);
			return brick;
		}
		protected virtual void ProcessLayoutControlItem(LayoutControlItem citem) {
			if(citem.Parent != null && !citem.Parent.Expanded) return;
			if(citem.TextVisible) PrintTextArea(citem);
			if(citem.ViewInfo.ClientAreaRelativeToControl.Width != 0 && citem.ViewInfo.ClientAreaRelativeToControl.Height != 0) {
				PrintControlArea(citem);
			}
		}
		protected virtual void PrintTextArea(LayoutControlItem citem) {
			ITextBrick textBrick = CreateTextBrick(
					citem.Text,
					AppearanceHelper.CreateBrick(citem.PaintAppearanceItemCaption),
					Color.Empty,
					citem.AppearanceItemCaption.TextOptions.HAlignment,
					citem.AppearanceItemCaption.TextOptions.VAlignment,
					0
				);
			GetOfAmpersands(textBrick);
			DrawBrick(textBrick, citem.ViewInfo.TextAreaRelativeToControl);
		}
		protected bool IsSingleLineControl(LayoutControlItem citem, BaseEdit edit) {
			if(citem.SizeConstraintsType == SizeConstraintsType.Custom) return false;
			if(edit is PictureEdit) return false;
			if(edit is MemoEdit) return false;
			if(edit is RadioGroup) return false;
			return true;
		}
		protected virtual void PrintControlArea(LayoutControlItem citem) {
			IVisualBrick contolBrick = null;
			if(citem.Control != null) {
				if(citem.Control is LabelControl) {
					contolBrick = CreateLabelControlBrick(citem, contolBrick);
				} else {
					if(citem.Control is BaseEdit) {
						contolBrick = CreateBaseEditBrick(citem, contolBrick);
					} else {
						contolBrick = CreateControlImageBrick(citem, contolBrick);
					}
				}
				contolBrick.Rect = citem.Control.Bounds;
				DrawBrick(contolBrick, citem.Control.Bounds);
			}
		}
		#region ControlBricks
		protected virtual IVisualBrick CreateControlImageBrick(LayoutControlItem citem, IVisualBrick contolBrick) {
			contolBrick = (IVisualBrick)ps.CreateBrick("ImageBrick");
			contolBrick.Sides = BorderSide.None;
			Bitmap image = new Bitmap(citem.ViewInfo.ClientArea.Width, citem.ViewInfo.ClientArea.Height);
			if(!(citem.Control is WebBrowserBase || citem.Control is Label)) {
				if(citem.Control.Controls != null && citem.Control.Controls.Count == 1 && citem.Control.Controls[0] is BaseEdit) {
					BaseEdit edit = (citem.Control.Controls[0] as BaseEdit);
					edit.GetViewInfo().UpdatePaintAppearance();			  
					return edit.Properties.GetBrick(new PrintCellHelperInfo(
							Color.Empty, ps, edit.EditValue,
							edit.GetViewInfo().PaintAppearance,
							edit.Properties.GetDisplayText(edit.Properties.DisplayFormat, edit.EditValue),
						   edit.Bounds, graph));
				}
				image = DevExpress.XtraReports.Native.XRControlPaint.GetControlImage(citem.Control, DevExpress.XtraReports.UI.WinControlDrawMethod_Utils.UseWMPrintRecursive, DevExpress.XtraReports.UI.WinControlImageType_Utils.Bitmap) as Bitmap;
			} else {
				citem.Control.DrawToBitmap(image, new Rectangle(Point.Empty, image.Size));
			}
			((IImageBrick)contolBrick).Image = image;
			return contolBrick;
		}
		protected virtual IVisualBrick CreateBaseEditBrick(LayoutControlItem citem, IVisualBrick contolBrick) {
			BaseEdit edit = citem.Control as BaseEdit;
			edit.GetViewInfo().UpdatePaintAppearance();
			AppearanceObject appearanceObj = edit.GetViewInfo().PaintAppearance;
			contolBrick = edit.Properties.GetBrick(
				new PrintCellHelperInfo(
						Color.Empty, ps, edit.EditValue,
						appearanceObj,
						edit.Properties.GetDisplayText(edit.Properties.DisplayFormat, edit.EditValue),
						edit.Bounds, graph)
					);
			if((contolBrick is ITextBrick) && IsSingleLineControl(citem, edit))
				((ITextBrick)contolBrick).VertAlignment = VertAlignment.Center;
			contolBrick.Sides = BorderSide.None;
			return contolBrick;
		}
		protected virtual IVisualBrick CreateLabelControlBrick(LayoutControlItem citem, IVisualBrick contolBrick) {
			LabelControl labelControl = citem.Control as LabelControl;
			AppearanceObject appearance = new AppearanceObject();
			appearance.Assign(labelControl.Appearance);
			if(appearance.TextOptions.WordWrap == WordWrap.Default) appearance.TextOptions.WordWrap = WordWrap.Wrap;
			contolBrick = CreateTextBrick(
			labelControl.Text,
			AppearanceHelper.CreateBrick(appearance),
			Color.Empty,
			citem.AppearanceItemCaption.TextOptions.HAlignment
		);
			return contolBrick;
		}
		#endregion
		public override void Visit(BaseLayoutItem item) {
			LayoutGroup group = item as LayoutGroup;
			LayoutControlItem citem = item as LayoutControlItem;
			if(group != null) ProcessGroup(group);
			if(citem != null && citem.Visible) ProcessLayoutControlItem(citem);
		}
		protected virtual void CreateDetails() { PrintGroupEx(owner.Root, new PointF(0, 0), null); }
		public virtual void PrintGroupEx(LayoutGroup group, PointF point, IBrickGraphics graph) {
			if(graph != null) this.graph = (BrickGraphics)graph;
			globalX = (int)point.X;
			globalY = (int)point.Y;
			if(group.ViewInfo.BoundsRelativeToControl.X < 0) globalX -= group.ViewInfo.BoundsRelativeToControl.X;
			if(group.ViewInfo.BoundsRelativeToControl.Y < 0) globalY -= group.ViewInfo.BoundsRelativeToControl.Y;
			group.Accept(this);
		}
		protected virtual void CreateDetailHeader() {
		}
		public virtual void AcceptChanges() {
		}
		public virtual void RejectChanges() {
		}
		public virtual void ShowHelp() {
		}
		public virtual bool SupportsHelp() { return false; }
		public virtual bool HasPropertyEditor() { return false; }
		protected internal virtual UserControl PropertyEditorControl {
			get {
				return null;
			}
		}
	}
	public class LayoutFlowPrinter : LayoutControlPrinter {
		public LayoutFlowPrinter(LayoutControl owner)
			: base(owner) {
		}
		FlowPrintingMap flowPrintingMap;
		IBasePrintable nestedPrintedControl = null;
		LayoutGroup currentProcessGroup;
		bool isPrintingNestedControl = false;
		BricksContainer bricksContainer = new BricksContainer();
		int xLocation = 0, yLocation = 0;
		Rectangle prevColumnRect;
		Size quantumSize;
		bool wasLinked = false;
		protected override int globalX { get { return 0; } }
		protected override int globalY { get { return 0; } }
		public override void Initialize(IPrintingSystem ps, ILink link, LayoutGroupViewInfo viewInfo) {
			if(isPrintingNestedControl) nestedPrintedControl.Initialize(this.ps, this.link);
			if(this.ps != ps) this.ps = ps;
			if(viewInfo != null && viewInfo.Owner != null && viewInfo.Owner.Owner != null && viewInfo.Owner.Owner.Control is DevExpress.XtraLayout.LayoutControl && GetOptionsPrint(viewInfo).AllowFitToPage)
				ps.AutoFitToPagesWidth = 1;
			this.link = (PrintableComponentLinkBase)link;
			new LayoutGroupPrintViewInfo(viewInfo).CalculateViewInfo();
			if(!isPrintingNestedControl) originalStateStore.Clear();
		}
		public override void CreateArea(string areaName, IBrickGraphics graph) {
			if(isPrintingNestedControl) {
				nestedPrintedControl.CreateArea(areaName, graph);
				return;
			}
			base.CreateArea(areaName, graph);
		}
		public override void PrintGroupEx(LayoutGroup group, PointF point, IBrickGraphics graph) {
			quantumSize = new Size((int)ClientPageSize.Width / 8, 1);
			base.PrintGroupEx(group, point, graph);
			flowPrintingMap = null;
		}
		protected override void DrawBrick(IVisualBrick brick, RectangleF rect) {
			DrawBrick(brick, rect, false,false);
		}
		bool CheckXafSplitContainer(object getIPrintableImplementerResult) {
			SplitContainerControl splitContainerControl =  getIPrintableImplementerResult as SplitContainerControl;
			if(splitContainerControl == null) return false;
			if(splitContainerControl.Panel1 == null || splitContainerControl.Panel1.Controls == null || splitContainerControl.Panel1.Controls.Count != 1) return false;
			if(splitContainerControl.Panel2 == null || splitContainerControl.Panel2.Controls == null || splitContainerControl.Panel2.Controls.Count != 1) return false;
			return true;
		}
		void PrintXafSplitContainerControl(SplitContainerControl splitContainerControl) {
			DrawIBasePrintableAllControls(splitContainerControl.Panel1.Controls);
			DrawIBasePrintableAllControls(splitContainerControl.Panel2.Controls);
			nestedPrintedControl = null;
		}
		void DrawIBasePrintableAllControls(Control.ControlCollection controls) {
			foreach(Control control in controls) {
				if(control is IBasePrintable) {
					nestedPrintedControl = control as IBasePrintable;
					DrawIBasePrintableControl();
				}
			}
		}
		void TryGetNestedPrintedControlForChild(LayoutControlItem citem) {
			Control control = citem.Control;
			if(control == null) return;
			Control.ControlCollection controls = control.Controls;
			if(controls != null) {
				if(controls.Count == 1 && controls[0] is IBasePrintable)
					nestedPrintedControl = controls[0] as IBasePrintable;
				else if(control is PanelControl) {
					Control pc;
					if(!OneControlVisible(controls, out pc)) return;
					TryGetNestedPrintedControl(pc);
				}
			}
		}
		bool OneControlVisible(Control.ControlCollection controls, out Control pc) {
			var controlsGen = controls.OfType<Control>();
			var cvcol = controlsGen.Select(e => e).Where(q => q.Visible);
			if(cvcol.Count() == 1) {
				pc = cvcol.First();
				return true;
			}
			pc = null;
			return false;
		}
		private IBasePrintable TryGetNestedPrintedControl(object getIPrintableImplementerResult) {
			if(getIPrintableImplementerResult is IBasePrintable)
				nestedPrintedControl = getIPrintableImplementerResult as IBasePrintable;
			else if(getIPrintableImplementerResult is IBasePrintableProvider) {
				nestedPrintedControl = ((IBasePrintableProvider)getIPrintableImplementerResult).GetIPrintableImplementer() as IBasePrintable;
			}
			return nestedPrintedControl;
		}
		protected override void PrintControlArea(LayoutControlItem citem) {
			nestedPrintedControl = citem.Control as IBasePrintable;
			if(nestedPrintedControl == null) {
				IBasePrintableProvider printableProvider = citem.Control as IBasePrintableProvider;
				if(printableProvider != null) {
					object getIPrintableImplementerResult = printableProvider.GetIPrintableImplementer();
					if(CheckXafSplitContainer(getIPrintableImplementerResult)) {
						PrintXafSplitContainerControl(getIPrintableImplementerResult as SplitContainerControl);
						return;
					}
				   nestedPrintedControl = TryGetNestedPrintedControl(getIPrintableImplementerResult);
				}
				if(nestedPrintedControl == null) TryGetNestedPrintedControlForChild(citem);
			}
			PrintControlAreaCore(citem);
		}
		void PrintControlAreaCore(LayoutControlItem citem) {
			if(nestedPrintedControl != null) {
				DrawIBasePrintableControl();
			} else {
				IVisualBrick contolBrick = null;
				if(citem.Control != null) {
					if(citem.Control is LabelControl) {
						contolBrick = CreateLabelControlBrick(citem, contolBrick);
					} else {
						if(citem.Control is BaseEdit) {
							contolBrick = CreateBaseEditBrick(citem, contolBrick);
						} else {
							contolBrick = CreateControlImageBrick(citem, contolBrick);
						}
					}
					bricksContainer.controlBrick = contolBrick;
					bricksContainer.controlRect = GetControlBounds(citem.Control, bricksContainer.textRect, out bricksContainer.startNewlineControl);
					bricksContainer.controlRectReal = citem.Control.Bounds;
				}
			}
		}
		protected override void PrintTextArea(LayoutControlItem citem) {
			AppearanceObject captionAppearance = GetTextPrintAppearance(citem);
			ITextBrick textBrick = CreateTextBrick(
					citem.Text,
					AppearanceHelper.CreateBrick(captionAppearance),
					Color.Empty,
					captionAppearance.TextOptions.HAlignment, captionAppearance.TextOptions.VAlignment, 0
				);
			GetOfAmpersands(textBrick);
			RectangleF drawRect = GetDrawRectangleForTextArea(citem, GetTextPrintAppearance(citem));
			bricksContainer.textBrick = textBrick;
			bricksContainer.textRect = drawRect;
			bricksContainer.startNewlineText = !(citem.Control is BaseEdit) || citem.Control is IBasePrintable;
		}
		protected override void ProcessLayoutControlItem(LayoutControlItem citem) {
			if(!citem.OptionsPrint.AllowPrint || !ParentPrintable(citem)) return;
			if(currentProcessGroup != null && citem.Parent != currentProcessGroup && flowPrintingMap != null && currentProcessGroup.GroupBordersVisible) {
				flowPrintingMap.SetNewLine(5);
				currentProcessGroup = null;
			}
			base.ProcessLayoutControlItem(citem);
			if(bricksContainer.controlBrick != null || bricksContainer.textBrick != null) {
				DrawBricksContainer(bricksContainer,citem);
			}
		}
		protected override void ProcessGroup(LayoutGroup group) {
			if(!ParentPrintable(group)) return;
			if(currentProcessGroup != null && currentProcessGroup != group && flowPrintingMap != null && group.GroupBordersVisible) {
				flowPrintingMap.SetNewLine(5);
			}
			IVisualBrick brick;
			if(group.GroupBordersVisible && group.TextVisible && group.ParentTabbedGroup == null && group.OptionsPrint.AllowPrintGroupCaption && group.OptionsPrint.AllowPrint) {
				if(!group.Visible) return;
				RectangleF captionRect;
				GetGroupBrickAndCeptionRect(group, out brick, out captionRect);
				GetOfAmpersands(brick as ITextBrick);
				DrawBrick(brick, GetGroupCaptionRect(group, captionRect));
			}
			if(group.ParentTabbedGroup != null) {
				ProcessTabbedGroup((TabbedControlGroup)group.ParentTabbedGroup, group);
			}
			if(!group.IsRoot) currentProcessGroup = group;
		}
		protected override void ProcessTabbedGroup(TabbedControlGroup tgroup, LayoutGroup group) {
			if(group.OptionsPrint.AllowPrintGroupCaption && group.OptionsPrint.AllowPrint)base.ProcessTabbedGroup(tgroup, group);
		}
		protected override void ProcessGroupCaption(LayoutGroup group, ref string text, ref AppearanceObject textAppearance) {
			textAppearance = GetGroupAppearance(group);
		}
		protected override IVisualBrick CreateTabbedBrick(LayoutGroup group, TabbedControlGroup tgroup) {
			IVisualBrick brick;
			AppearanceObject brickAppearance = GetGroupAppearance(group);
			brick = CreateTextBrick(group.Text, AppearanceHelper.CreateBrick(brickAppearance), Color.Empty, brickAppearance.TextOptions.HAlignment);
			return brick;
		}
		#region GetAppearance
		AppearanceObject GetGroupAppearance(LayoutGroup group) {
			 return group.OptionsPrint.GetGroupAppearance();
		}
		AppearanceObject GetTextPrintAppearance(LayoutControlItem citem) {
			return citem.OptionsPrint.GetItemAppearance();
		}
		#endregion
		#region ControlBricks
		void DrawIBasePrintableControl() {
			if(bricksContainer.textBrick != null) {
				DrawBrick(bricksContainer.textBrick, bricksContainer.textRect);
			}
			try {
				isPrintingNestedControl = true;
				PrintableComponentLinkBase link = (PrintableComponentLinkBase)this.link;
				link.AddSubreport(new PointF(0, 10));
			} finally {
				isPrintingNestedControl = false;
			}
			wasLinked = true;
			bricksContainer.Reset();
		}
		protected override IVisualBrick CreateControlImageBrick(LayoutControlItem citem, IVisualBrick contolBrick) {
			contolBrick = (IVisualBrick)ps.CreateBrick("ImageBrick");
			contolBrick.Sides = BorderSide.None;
			Bitmap image = new Bitmap(citem.ViewInfo.ClientArea.Width, citem.ViewInfo.ClientArea.Height);
			if(!(citem.Control is WebBrowserBase || citem.Control is Label)) {
				if(citem.Control.Controls != null && citem.Control.Controls.Count == 1 && citem.Control.Controls[0] is BaseEdit) {
					BaseEdit edit = (citem.Control.Controls[0] as BaseEdit);
					bool fake = false;
					return edit.Properties.GetBrick(new PrintCellHelperInfo(
							Color.Empty, ps, edit.EditValue,
							GetTextPrintAppearance(citem),
							edit.Properties.GetDisplayText(edit.Properties.DisplayFormat, edit.EditValue),
						   Rectangle.Round(GetControlBounds(edit, bricksContainer.textRect, out fake)), graph));
				}
				image = DevExpress.XtraReports.Native.XRControlPaint.GetControlImage(citem.Control, DevExpress.XtraReports.UI.WinControlDrawMethod_Utils.UseWMPrintRecursive, DevExpress.XtraReports.UI.WinControlImageType_Utils.Bitmap) as Bitmap;
			} else {
				citem.Control.DrawToBitmap(image, new Rectangle(Point.Empty, image.Size));
			}
			((IImageBrick)contolBrick).Image = image;
			return contolBrick;
		}
		protected override IVisualBrick CreateBaseEditBrick(LayoutControlItem citem, IVisualBrick contolBrick) {
			BaseEdit edit = citem.Control as BaseEdit;
			AppearanceObject appearanceObj = GetTextPrintAppearance(citem);
			appearanceObj.TextOptions.HAlignment = HorzAlignment.Near;
			bool fake = false;
			contolBrick = edit.Properties.GetBrick(
				new PrintCellHelperInfo(
						Color.Empty, ps, edit.EditValue,
						appearanceObj,
						edit.Properties.GetDisplayText(edit.Properties.DisplayFormat, edit.EditValue),
					   Rectangle.Round(GetControlBounds(citem.Control, bricksContainer.textRect, out fake)), graph)
					);
			if((contolBrick is ITextBrick) && IsSingleLineControl(citem, edit))
				((ITextBrick)contolBrick).VertAlignment = VertAlignment.Center;
			contolBrick.Sides = BorderSide.None;
			return contolBrick;
		}
		protected override IVisualBrick CreateLabelControlBrick(LayoutControlItem citem, IVisualBrick contolBrick) {
			LabelControl labelControl = citem.Control as LabelControl;
			AppearanceObject appearance = new AppearanceObject();
			appearance.Assign(labelControl.Appearance);
			if(appearance.TextOptions.WordWrap == WordWrap.Default) appearance.TextOptions.WordWrap = WordWrap.Wrap;
			contolBrick = CreateTextBrick(
			labelControl.Text,
			AppearanceHelper.CreateBrick(appearance),
			Color.Empty,
			citem.AppearanceItemCaption.TextOptions.HAlignment
		);
			return contolBrick;
		}
		#endregion
		protected void DrawBrick(IVisualBrick brick, RectangleF rect, bool startNewLine, bool column) {
			if(brick == null) return;
			rect = GetValidRecangleF(rect, startNewLine,column);
			RectangleF realRect = bricksContainer.controlRectReal;
			if(brick is IImageBrick && rect.Size.Width > realRect.Width && realRect.Width != 0) {
				rect.Size = realRect.Size;
			}
			((IBrickGraphics)graph).DrawBrick(brick, rect);
		}
		void DrawBricksContainer(BricksContainer bricksContainer, LayoutControlItem citem) {
			if(citem.Parent == currentProcessGroup && citem.Parent.DefaultLayoutType == LayoutType.Vertical && !citem.Parent.GroupBordersVisible && CheckParentColumn(citem.Parent)) {
				if(citem.Location == Point.Empty) {
					xLocation = yLocation = 0;
					DrawBrickContainerCore(bricksContainer, false);
					return;
				}
				DrawBrickContainerCore(bricksContainer, true);
			} else {
				DrawBrickContainerCore(bricksContainer, false);
			}
		}
		bool CheckParentColumn(LayoutGroup layoutGroup) {
			foreach(BaseLayoutItem bli in layoutGroup.Items) {
				if(bli.X != 0) return false;
			}
			return true;
		}
		void DrawBrickContainerCore(BricksContainer bricksContainer, bool column) {
			if(bricksContainer.startNewlineText || bricksContainer.startNewlineControl) {
				DrawBrick(bricksContainer.textBrick, bricksContainer.textRect, true, column);
				DrawBrick(bricksContainer.controlBrick, bricksContainer.controlRect, true, column);
				bricksContainer.Reset();
			} else {
				RectangleF validRect = RectangleF.Empty;
				if(bricksContainer.textRect != RectangleF.Empty && bricksContainer.controlRect != RectangleF.Empty)
					validRect = RectangleF.Union(bricksContainer.textRect, bricksContainer.controlRect);
				else {
					validRect = bricksContainer.textRect != RectangleF.Empty ? bricksContainer.textRect : bricksContainer.controlRect;
				}
				validRect = GetValidRecangleF(validRect, false, column);
				RectangleF textRect = validRect;
				textRect.Width = bricksContainer.textRect.Width;
				((IBrickGraphics)graph).DrawBrick(bricksContainer.textBrick, textRect);
				RectangleF controlRect = new RectangleF(textRect.Right, textRect.Top, validRect.Width - textRect.Width, validRect.Height);
				RectangleF realRect = bricksContainer.controlRectReal;
				if(bricksContainer.controlBrick is IImageBrick && controlRect.Size.Width > realRect.Width && realRect.Width != 0) {
					controlRect.Size = realRect.Size;
				}
				((IBrickGraphics)graph).DrawBrick(bricksContainer.controlBrick, controlRect);
				bricksContainer.Reset();
			}
		}
		#region GetRectangleF
		RectangleF GetValidRecangleF(RectangleF rect, bool startNewLine,bool column) {
			if(flowPrintingMap == null || wasLinked) {
				flowPrintingMap = new FlowPrintingMap((int)ClientPageSize.Width / quantumSize.Width, quantumSize.Width, quantumSize.Height);
				if(wasLinked) flowPrintingMap.SetNewLine(1);
				xLocation = yLocation = 0;
				wasLinked = false;
			}
			Size cellSize = new Size((int)rect.Width / flowPrintingMap.ItemsW + 1, (int)rect.Height / flowPrintingMap.ItemsH);
			if(cellSize.Width > flowPrintingMap.MapWidth) cellSize.Width = flowPrintingMap.MapWidth;
			Rectangle checkRectangle = new Rectangle(xLocation, yLocation, cellSize.Width * flowPrintingMap.ItemsW, cellSize.Height * flowPrintingMap.ItemsH);
			if(column) {
				checkRectangle.X = prevColumnRect.X;
				checkRectangle.Y = prevColumnRect.Bottom;
			}
			if(flowPrintingMap.CheckPrintRectangle(ref checkRectangle, startNewLine)) {
				rect = checkRectangle;
				xLocation += checkRectangle.Width;
			} else {
				xLocation = 0;
				yLocation = quantumSize.Height;
				checkRectangle = new Rectangle(xLocation, yLocation, cellSize.Width * quantumSize.Width, cellSize.Height * quantumSize.Height);
				int watchdog = 10000;
				while(!flowPrintingMap.CheckPrintRectangle(ref checkRectangle, false) && watchdog-- > 0) {
					yLocation += quantumSize.Height;
					checkRectangle = new Rectangle(xLocation, yLocation, cellSize.Width * quantumSize.Width, cellSize.Height * quantumSize.Height);
				}
				rect = checkRectangle;
				xLocation += checkRectangle.Width;
			}
			flowPrintingMap.FillMap(checkRectangle);
			prevColumnRect = checkRectangle;
			return rect;
		}
		RectangleF GetControlBounds(Control control, RectangleF rectangleF, out bool startNewLine) {
			startNewLine = false;
			if(control is BaseEdit && !(control is PictureEdit)) {
				SizeF result = ((BaseEdit)control).CalcBestSize();
				if(result.Width > ClientPageSize.Width || rectangleF.Width + result.Width > ClientPageSize.Width) {
					float area = result.Height * result.Width;
					result.Width = ClientPageSize.Width;
					result.Height = area / ClientPageSize.Width + 16;
					startNewLine = true;
				}
				if(result.Width < quantumSize.Width / 2 && bricksContainer.textRect != RectangleF.Empty && !bricksContainer.startNewlineText) {
					result.Width = quantumSize.Width / 2;
				}
				if(result.Width < quantumSize.Width && bricksContainer.textRect == RectangleF.Empty) {
					result.Width = quantumSize.Width;
				}
				return new RectangleF(
				   rectangleF == RectangleF.Empty ? control.Location : new PointF(rectangleF.Right, rectangleF.Top),
					result);
			}
			return control.Bounds;
		}
		protected override RectangleF GetGroupCaptionRect(LayoutGroup group, RectangleF captionRect) {
			return new RectangleF(0, 0, ClientPageSize.Width, captionRect.Height);
		}
		RectangleF GetDrawRectangleForTextArea(LayoutControlItem citem, AppearanceObject appObject) {
			if(citem.Control is BaseEdit && !(citem.Control is IBasePrintable)) {
				RectangleF returnRect = new RectangleF(citem.ViewInfo.TextAreaRelativeToControl.Location, citem.Owner.TextAlignManager.GetTextAutoSize(citem, appObject));
				returnRect.Width += GetTextToControlDistance(citem);
				if(returnRect.Width < quantumSize.Width / 2) returnRect.Width = quantumSize.Width / 2;
				return returnRect;
			}
			return new RectangleF(0, 0, ClientPageSize.Width, 26);
		}
		#endregion
		static int GetTextToControlDistance(LayoutControlItem citem) {
			if(citem.OptionsPrint.TextToControlDistance != -1) return citem.OptionsPrint.TextToControlDistance;
			LayoutGroup parent = citem.Parent;
			while(parent != null) {
				if(parent.OptionsPrint.TextToControlDistance != -1) return parent.OptionsPrint.TextToControlDistance;
				parent = parent.Parent;
			}
			if(citem.Owner is LayoutControl && (citem.Owner as LayoutControl).OptionsPrint.TextToControlDistance != -1) return (citem.Owner as LayoutControl).OptionsPrint.TextToControlDistance;
			return 5;
		}
		static bool ParentPrintable(BaseLayoutItem item) {
			LayoutGroup parent = item.Parent;
			while(parent != null) {
				if(!parent.OptionsPrint.AllowPrint) return false;
				parent = parent.Parent;
			}
			return true;
		}
	}
	internal class BricksContainer {
		internal BricksContainer() {
			Reset();
		}
		internal IVisualBrick textBrick;
		internal RectangleF textRect;
		internal bool startNewlineText;
		internal IVisualBrick controlBrick;
		internal RectangleF controlRect;
		internal RectangleF controlRectReal;
		internal bool startNewlineControl;
		internal void Reset() {
			textBrick = null;
			textRect = RectangleF.Empty;
			startNewlineText = false;
			controlBrick = null;
			controlRect = RectangleF.Empty;
			controlRectReal = RectangleF.Empty;
			startNewlineControl = false;
		}
	}
	internal class FlowPrintingMap{
		public FlowPrintingMap(int mapWidth, int parameterW, int parameterH) {
			mapWidthCore = mapWidth > 0 ? mapWidth : 1;
			Map = new int[mapWidthCore];
			itemsWCore = parameterW;
			itemsHCore = parameterH;
		}
		public int[] Map { get; set; }
		public int MapWidth { get { return mapWidthCore; } }
		public int ItemsW { get { return itemsWCore; } }
		public int ItemsH { get { return itemsHCore; } }
		int mapWidthCore, itemsWCore, itemsHCore;
		internal bool CheckPrintRectangle(ref Rectangle checkRectangle,bool shouldStartNewLine) {
			Rectangle mapRectangle = new Rectangle(checkRectangle.X / ItemsW, checkRectangle.Y / ItemsH, checkRectangle.Width / ItemsW, checkRectangle.Height / ItemsH);
			if(MapWidth < mapRectangle.X + mapRectangle.Width) return false;
			bool returnValue = false;
			int startPosition = mapRectangle.X;
			if(shouldStartNewLine) SetNewLine(0);
			for(int i = startPosition; i < MapWidth; i++) {
				bool FillRectangle = true;
				if(Map[i] <= mapRectangle.Y) {
					startPosition = i;
					for(int j = startPosition; j < startPosition + mapRectangle.Width; j++) {
						if(j >= MapWidth) {
							FillRectangle = false;
							break;
						}
						if(Map[j] - 1 >= mapRectangle.Y) FillRectangle = false;
					}
					if(FillRectangle) {
						mapRectangle.X = startPosition;
						checkRectangle.X = startPosition * ItemsW;
						returnValue = true;
						break;
					}
				}
			}
			return returnValue;
		}
		internal void FillMap(Rectangle checkRectangle) {
				Rectangle fillRect = new Rectangle(checkRectangle.X / ItemsW, checkRectangle.Y / ItemsH, checkRectangle.Width / ItemsW, checkRectangle.Height / ItemsH);
				for(int i = fillRect.X; i < fillRect.X + fillRect.Width; i++) {
					Map[i] = fillRect.Y + fillRect.Height;
				}
		}
		internal void SetNewLine(int delay) {
			int max = Map.Max() + delay;
			for(int k = 0; k < MapWidth; k++)
				Map[k] = max;
		}
		internal void ClearMap() {
			for(int i = 0; i < Map.Count(); i++) {
				Map[i] = 0;
			}   
		}
	}
}
