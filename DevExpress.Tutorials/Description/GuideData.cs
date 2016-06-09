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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using DevExpress.Utils.Text;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.Description.Controls {
	public delegate Control GuideGetCurrentModuleDelegate();
	public interface IGuideDescriptionProvider {
		bool Enabled { get; }
		void UpdateDescriptions(List<GuideControlDescription> templates);
	}
	public class GuideGenerator {
		GuideGetCurrentModuleDelegate getCurrentModule;
		public virtual void CreateWhatsThisItem(RibbonControl ribbon, GuideGetCurrentModuleDelegate getCurrentModule) {
			this.getCurrentModule = getCurrentModule;
			var item = new BarButtonItem() { Caption = "What's this" };
			ribbon.Items.Add(item);
			item.ItemClick += OnWhatsThis;
			ribbon.ToolbarLocation = RibbonQuickAccessToolbarLocation.Above;
			ribbon.Toolbar.ItemLinks.Add(item);
			item.Hint = "That's super hint";
			item.SuperTip = new Utils.SuperToolTip();
			item.SuperTip.AllowHtmlText = Utils.DefaultBoolean.True;
			item.SuperTip.Items.AddTitle("What's this");
			item.SuperTip.Items.Add("Click here if you want learn more on controls used in demo");
			if(ribbon.FindForm() != null) ribbon.FindForm().Load += (sender, e) => {
				if(item.Links.Count > 0) item.Links[0].ShowHint();
			};
		}
		class BarWhatsThisItem : BarButtonItem {
			protected override BarItemLink CreateLinkCore(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) {
				return new BarWhatsThisItemLink(ALinks, AItem, ALinkedObject);
			}
		}
		class BarWhatsThisItemLink : BarButtonItemLink {
			public BarWhatsThisItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
			}
			protected override Utils.ToolTipControlInfo GetToolTipInfo(XtraBars.Ribbon.ViewInfo.RibbonHitInfo hi, Point point) {
				var res = base.GetToolTipInfo(hi, point);
				res.ImmediateToolTip = true;
				return res;
			}
			bool hintVisible = false;
			Timer hintTimer = null;
			protected override void OnBeforeShowHint(Utils.ToolTipControllerShowEventArgs e) {
				e.Show = false;
				if(ScreenBounds.IsEmpty || hintVisible) return;
				hintVisible = true;
				var link = this;
				var ribbon = Ribbon;
				NoteHint.NoteWindow nw = new NoteHint.NoteWindow() { HintPosition = NoteHint.NoteHintPosition.Down, HintContent = new DevExpress.Tutorials.Description.Hint.GuideDescription() };
				new System.Windows.Interop.WindowInteropHelper(nw).Owner = ((Form)ribbon.FindForm()).Handle;
				nw.ShowHtmlCloseButton = true;
				Rectangle display = link.ScreenBounds;
				nw.ShowActivated = false;
				nw.Show(display);
				ItemClickEventHandler click = null;
				click = (s1, e1) => {
					DoHide(ribbon, nw, click);
				};
				nw.IsVisibleChanged += (s2, e2) => {
					if(!((System.Windows.Window)s2).IsVisible) DoHide(ribbon, nw, click);
				};
				ribbon.ItemClick += click;
				hintTimer = new Timer();
				int ticks = 0;
				hintTimer.Interval = 100;
				hintTimer.Tick += (s3, e3) => {
					if(ticks++ == 70) {
						DoHide(ribbon, nw, click);
						return;
					}
					if(ScreenBounds != display) DoHide(ribbon, nw, click);
				};
				hintTimer.Start();
			}
			void DoHide(RibbonControl ribbon, DevExpress.NoteHint.NoteWindow window, ItemClickEventHandler handler) {
				hintVisible = false;
				ribbon.ItemClick -= handler;
				window.Hide();
				if(hintTimer != null) hintTimer.Dispose();
				hintTimer = null;
			}
		}
		public virtual void CreateWhatsThisItemAlt(RibbonControl ribbon, GuideGetCurrentModuleDelegate getCurrentModule) {
			this.getCurrentModule = getCurrentModule;
			var item = new BarWhatsThisItem() { Caption = "What's this", Hint = "" };
			item.SuperTip = new Utils.SuperToolTip();
			ribbon.Items.Add(item);
			item.ItemClick += OnWhatsThis;
			ribbon.ToolbarLocation = RibbonQuickAccessToolbarLocation.Above;
			ribbon.Toolbar.ItemLinks.Add(item);
			if(ribbon.FindForm() != null) ribbon.FindForm().Load += (sender, e) => {
				if(item.Links.Count == 0) return;
				var link = (item.Links[0]);
				link.ShowHint();
			};
		}
		void DoHide(RibbonControl ribbon, DevExpress.NoteHint.NoteWindow window, ItemClickEventHandler handler) {
			ribbon.ItemClick -= handler;
			window.Hide();
		}
		public bool AllowEditorDescriptions = false;
		void OnWhatsThis(object sender, ItemClickEventArgs e) {
			if(getCurrentModule == null) return;
			Control module = getCurrentModule();
			IGuideDescriptionProvider provider = module as IGuideDescriptionProvider;
			if(module == null || (provider != null && !provider.Enabled)) return;
			GuideControl gc = new GuideControlEx();
			List<GuideControlDescription> list = GenerateTemplateDescriptions();
			if(provider != null) provider.UpdateDescriptions(list);
			gc.Init(list  , module);
			gc.Show();
		}
		protected virtual List<GuideControlDescription> GenerateTemplateDescriptions() {
			List<GuideControlDescription> list = new List<GuideControlDescription>();
			list = DevExpress.Demos.XmlSerialization.XMLSerializer<List<GuideControlDescription>>.LoadXmlFromResources(typeof(GuideGenerator).Assembly, @"DevExpress.Tutorials.Description.Guide.xml", null);
			return list;
		}
		#region by code
		private void GenerateByCode(List<GuideControlDescription> list) {
			list.Add(new GuideControlDescription() {
				ControlTypeName = "DevExpress.XtraGrid.Controls.FindControl",
				AllowParseChildren = false,
				Description =
	@"<b>Find Panel</b>
Enables MS Outlook style search in the Grid Control. <href=https://www.devexpress.com/Products/NET/Controls/WinForms/Grid/>Learn more.</href>

Toolbox Item: <b>GridControl</b>
Included in subscriptions: <href=https://www.devexpress.com/Subscriptions/>WinForms, DXperience, Universal</href>
"
			});
			list.Add(new GuideControlDescription() {
				ControlTypeName = "DevExpress.XtraGrid.GridControl:GridView",
				Description =
@"
<b>Grid View</b>
Emulates MS Outlook Mail view or MS Access Data Table view.  <href=https://www.devexpress.com/Products/NET/Controls/WinForms/Grid/>Learn more.</href>

Toolbox Item: <b>GridControl</b>
Included in subscriptions: <href=https://www.devexpress.com/Subscriptions/>WinForms, DXperience, Universal</href>
"
			});
			list.Add(new GuideControlDescription() {
				ControlTypeName = "DevExpress.XtraGrid.GridControl:CardView",
				Description =
@"
<b>Card View</b>
Emulates MS Outlook Contacts view. <href=https://www.devexpress.com/Products/NET/Controls/WinForms/Grid/>Learn more.</href>

Toolbox Item: <b>GridControl</b>
Included in subscriptions: <href=https://www.devexpress.com/Subscriptions/>WinForms, DXperience, Universal</href>
"
			});
			list.Add(new GuideControlDescription() {
				ControlTypeName = "DevExpress.XtraGrid.GridControl:CardView",
				Description =
@"
<b>Layout View</b>
Emulates MS Outlook Contacts view. <href=https://www.devexpress.com/Products/NET/Controls/WinForms/Grid/>Learn more.</href>

Toolbox Item: <b>GridControl</b>
Included in subscriptions: <href=https://www.devexpress.com/Subscriptions/>WinForms, DXperience, Universal</href>
"
			});
			list.Add(new GuideControlDescription() {
				ControlTypeName = "DevExpress.XtraGrid.GridControl",
				Description =
@"
<b>Grid View</b>
Emulates MS Outlook Mail view or MS Access Data Table view.  <href=https://www.devexpress.com/Products/NET/Controls/WinForms/Grid/>Learn more.</href>

Toolbox Item: <b>GridControl</b>
Included in subscriptions: <href=https://www.devexpress.com/Subscriptions/>WinForms, DXperience, Universal</href>
"
			});
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraBars.Ribbon.RibbonControl", HighlightUseInsideBounds = true });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraBars.Ribbon.RibbonStatusBar", HighlightUseInsideBounds = true });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraNavBar.NavBarControl" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraSpreadsheet.SpreadsheetControl" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraScheduler.SchedulerControl" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraScheduler.DateNavigator" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraTreeList.TreeList" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraEditors.RangeControl" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraCharts.ChartControl" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.ProductsDemo.Win.PivotTileControl" }); 
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraPivotGrid.PivotGridControl" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraMap.MapControl" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraPdfViewer.PdfViewer", AllowParseChildren = false });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.DocumentView.Controls.ViewControl" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraPrinting.Native.WinControls.BookmarkTreeView" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraGauges.Win.GaugeControl" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.Snap.SnapControl" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.Snap.Extensions.UI.FieldListDockPanel" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.Snap.Extensions.UI.ReportExplorerDockPanel" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraRichEdit.RichEditControl" });
			if(AllowEditorDescriptions) GenerateEditors(list);
		}
		#endregion
		protected virtual void GenerateEditors(List<GuideControlDescription> list) {
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraEditors.LabelControl" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraEditors.SimpleButton" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraEditors.TextEdit" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraEditors.ComboBoxEdit" });
			list.Add(new GuideControlDescription() { ControlTypeName = "DevExpress.XtraEditors.PictureEdit" });
		}
	}
	public class GuideControlDescription : IStringImageProvider {
		public GuideControlDescription() {
			AllowParseChildren = true;
		}
		[DefaultValue(false)]
		public bool HighlightUseInsideBounds { get; set; }
		[DefaultValue(true)]
		public bool AllowParseChildren { get; set; }
		[DefaultValue(null)]
		public string ControlName { get; set; }
		[DefaultValue(null)]
		public string ControlTypeName { get; set; }
		[XmlIgnore, Browsable(false)]
		public Type ControlType { get; set; }
		public string Description { get; set; }
		[DefaultValue(null), XmlIgnore]
		public Image DescriptionPicture { get; set; }
		[XmlIgnore, Browsable(false)]
		public Control AssociatedControl { get; set; }
		[XmlIgnore]
		internal Rectangle ControlBounds { get; set; }
		[XmlIgnore]
		internal Rectangle ScreenBounds { get; set; }
		[XmlIgnore]
		internal bool ControlVisible { get; set; }
		public override string ToString() {
			string res = GetTypeName();
			string name = AssociatedControl != null && !string.IsNullOrEmpty(AssociatedControl.Name) ? AssociatedControl.Name : ControlName;
			if(!string.IsNullOrEmpty(name)) res += string.Format(" [{0}]", name);
			return res;
		}
		[XmlIgnore, Browsable(false)]
		public virtual bool IsValidNow { get { return AssociatedControl != null && ControlVisible; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("DescriptionPicture")]
		public byte[] DescriptionPictureSerialized {
			get { 
				if(DescriptionPicture == null) return null;
				using(MemoryStream ms = new MemoryStream()) {
					DescriptionPicture.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					return ms.ToArray();
				}
			}
			set { 
				if(value == null) {
					DescriptionPicture = null;
				}
				else {
					using(MemoryStream ms = new MemoryStream(value)) {
						DescriptionPicture = new Bitmap(ms);
					}
				}
			}
		}
		internal GuideControlDescription Clone() {
			GuideControlDescription res = new GuideControlDescription() {
				ControlName = ControlName,
				ControlTypeName = ControlTypeName,
				ControlType = ControlType,
				Description = Description,
				DescriptionPicture = DescriptionPicture,
				HighlightUseInsideBounds = HighlightUseInsideBounds,
				AllowParseChildren = AllowParseChildren
			};
			return res;
		}
		internal string GetTypeName() {
			if(!string.IsNullOrEmpty(ControlTypeName)) return ControlTypeName;
			if(AssociatedControl != null) return AssociatedControl.GetType().FullName;
			if(ControlType != null) return ControlType.FullName;
			return "";
		}
		#region IStringImageProvider Members
		Image IStringImageProvider.GetImage(string id) {
			if(DescriptionPicture != null) return DescriptionPicture;
			return null;
		}
		#endregion
	}
	public interface IGuideForm {
		void Dispose();
		bool Visible { get; }
		void Show(GuideControl owner, GuideControlDescription description);
		event EventHandler FormClosed;
		bool IsHandle(IntPtr intPtr);
	}
}
