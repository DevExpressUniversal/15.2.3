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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Frames;
using DevExpress.Utils;
namespace DevExpress.XtraEditors.Designer.Utils {
	[ToolboxItem(false)] 
	public class XtraFrame : XtraUserControl {
		private object editingObject;
		private object infoObject;
		private bool havepg = false;
		public LabelControl lbCaption;
		public PanelControl pnlMain;
		private int id;
		protected DevExpress.XtraEditors.PanelControl horzSplitter;
		DesignerItem designerItem = null;
		DescriptionPanel pnlDescription = null;
		static ImageCollection designerImages16 = null;
		static ImageCollection designerImages12 = null;
		static ImageCollection buttonPanelImages = null;
		static Image findImage = null;
		public event RefreshWizardEventHandler RefreshWizard;
		public event MouseEventHandler ShowMenu;
		public const int DesignerImages16AddIndex = 0;
		public const int DesignerImages16InsertIndex = 1;
		public const int DesignerImages16RemoveIndex = 2;
		public const int DesignerImages16RetrieveIndex = 3;
		public const int DesignerImages16LoadIndex = 4;
		public const int DesignerImages16SaveIndex = 5;
		public const int DesignerImages16PlusIndex = 6;
		public const int DesignerImages16MinusIndex = 7;
		public const int DesignerImages16ResetIndex = 8;
		public const int DesignerImages16ApplyIndex = 9;
		public const int DesignerImages16LeftIndex = 10;
		public const int DesignerImages16RightIndex = 11;
		public const int DesignerImages16UpIndex = 12;
		public const int DesignerImages16DownIndex = 13;
		public static ImageCollection DesignerImages16 {
			get {
				if(designerImages16 == null)
					designerImages16 = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.Designer.Utils.Resource.designer16x16.png", typeof(XtraFrame).Assembly, new Size(16, 16));
				return designerImages16;
			}
		}
		public static Image FindImage {
			get {
				if(findImage == null)
					findImage = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Designer.Utils.Resource.Find_16x16.png", typeof(XtraFrame).Assembly);
				return findImage;
			}
		}
		public static ImageCollection DesignerImages12 {
			get {
				if(designerImages12 == null)
					designerImages12 = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.Designer.Utils.Resource.designer12x12.png", typeof(XtraFrame).Assembly, new Size(16, 16));
				return designerImages12;
			}
		}
		public static ImageCollection ButtonPanelImages {
			get {
				if(buttonPanelImages == null)
					buttonPanelImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.Designer.Utils.Resource.buttonPanelImages.png", typeof(XtraFrame).Assembly, new Size(22, 22));
				return buttonPanelImages;
			}
		}
		public Component EditingComponent { get { return EditingObject as Component; } }
		public object EditingObject { get { return editingObject; } }
		public object InfoObject { get { return infoObject; } }
		public virtual object GetComponentService(Type type) {
			return EditingComponent == null || EditingComponent.Site == null ? null : EditingComponent.Site.GetService(type);
		}
		protected virtual bool AllowGlobalStore { get { return true; } }
		public void StoreProperties(PropertyStore globalStore) {
			StoreGlobalProperties(globalStore);
			PropertyStore store = new PropertyStore(globalStore, GetPropertyStoreName());
			StoreLocalProperties(store);
			if(store.IsEmpty) return;
			globalStore.AddProperty(GetPropertyStoreName(), store);
		}
		public void RestoreProperties(PropertyStore globalStore) {
			RestoreGlobalProperties(globalStore);
			PropertyStore store = globalStore.RestoreProperties(GetPropertyStoreName());
			if(store == null) return;
			RestoreLocalProperties(store);
		}
		public virtual void StoreGlobalProperties(PropertyStore globalStore) {}
		public virtual void RestoreGlobalProperties(PropertyStore globalStore) {}
		public virtual void StoreLocalProperties(PropertyStore localStore) {}
		public virtual void RestoreLocalProperties(PropertyStore localStore) {}
		public virtual string GetPropertyStoreName() {
			if(DesignerItem != null) return string.Format("XF_{0}_{1}", DesignerItem.Group.Caption, DesignerItem.Caption);
			return string.Format("XF_{0}", ID);
		}
		public XtraFrame() : this(0) {
		}
		public XtraFrame(int i) {
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			InitializeComponent();
			ID = i;
		}
		protected virtual DockStyle DescriptionPanelDock { get { return DockStyle.Top; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DesignerItem DesignerItem { get { return designerItem; } set { designerItem = value; } }
		public int ID {
			get { return id;}
			set { id = value; }
		}
		public bool HavePG {
			get{return havepg;}
			set{havepg = value;}
		}
		public void InitFrame() {
			InitFrame("");
		}
		public void InitFrame(string caption) {
			InitFrame(caption, null);
		}
		public void InitFrame(string caption, Image img) {
			InitFrame(null, caption, img);
		}
		public void InitFrame(object editingObject, string caption, Image img) {
			InitFrame(editingObject, null, caption, img);
		}
		public void InitFrame(object editingObject, object infoObject, string caption, Image img) {
			this.editingObject = editingObject;
			this.infoObject = infoObject;
			lbCaption.Text = caption;
			lbCaption.Visible = lbCaption.Text != string.Empty;
			UpdateDescriptionPanel();
			InitComponent();
			InitImages();
			DoInitFrame();
		}
		public virtual void DoInitFrame() {}
		public virtual void InitComponent() {}
		protected virtual void InitImages() {}
		public virtual void RefreshFrame(bool b) {}
		protected virtual string DescriptionText { get { return string.Empty; } }
		protected virtual string GetDescriptionTextCore() {
			return DescriptionText;
		}
		protected void UpdateDescriptionPanel() { 
			if(GetDescriptionTextCore() != string.Empty) {
				if(this.pnlDescription == null) 
					SetupDescriptionPanel();
				this.pnlDescription.Text = GetDescriptionTextCore();
				this.horzSplitter.SendToBack();
				this.pnlDescription.SendToBack();
				this.lbCaption.SendToBack();
			} else 
				if(this.pnlDescription != null) {
				this.pnlDescription.Dispose();
				this.pnlDescription = null;
			}
		}
		void SetupDescriptionPanel() {
			if(GetDescriptionTextCore() == string.Empty) return;
			this.pnlDescription = new DescriptionPanel();
			this.pnlDescription.Parent = this;
			this.pnlDescription.Dock = DescriptionPanelDock;
			this.pnlDescription.Height = 20;
		}
		protected void RaiseRefreshWizard(string viewName, string condition) {
			if(RefreshWizard != null)
				RefreshWizard(this, new RefreshWizardEventArgs(viewName, condition));
		}
		protected void RaiseShowPGMenu(object sender, MouseEventArgs e) {
			if(ShowMenu != null)
				ShowMenu(sender, e);
		}
		protected void SetPanelBkColor(Control pnl) {
			SetPanelBkColor(pnl, true);
		}
		protected void SetPanelBkColor(Control pnl, bool allColors) {
			pnl.BackColor = Color.White;
			if(allColors)
				foreach(Control c in pnl.Controls)
					if(c is Button)
						c.BackColor = SystemColors.Control;
			SetTransparentBitmap(pnl);
		}
		protected void SetTransparentBitmap(Control pnl) {
			foreach(object obj in pnl.Controls) 
				if(obj is Button) {
					Button btn = (Button)obj;
					if(btn.Image != null)
						((Bitmap)btn.Image).MakeTransparent();
				} 
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.lbCaption = new DevExpress.XtraEditors.LabelControl();
			this.pnlMain = new DevExpress.XtraEditors.PanelControl();
			this.horzSplitter = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.SuspendLayout();
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lbCaption.Dock = System.Windows.Forms.DockStyle.Top;
			this.lbCaption.Location = new System.Drawing.Point(0, 0);
			this.lbCaption.Margin = new System.Windows.Forms.Padding(0);
			this.lbCaption.Name = "lbCaption";
			this.lbCaption.Size = new System.Drawing.Size(400, 44);
			this.lbCaption.TabIndex = 0;
			this.lbCaption.Appearance.TextOptions.VAlignment = VertAlignment.Bottom;
			this.pnlMain.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Location = new System.Drawing.Point(0, 42);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Size = new System.Drawing.Size(400, 234);
			this.pnlMain.TabIndex = 1;
			this.horzSplitter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.horzSplitter.Dock = System.Windows.Forms.DockStyle.Top;
			this.horzSplitter.Location = new System.Drawing.Point(0, 38);
			this.horzSplitter.Name = "horzSplitter";
			this.horzSplitter.Size = new System.Drawing.Size(400, 4);
			this.horzSplitter.TabIndex = 2;
			this.Controls.Add(this.pnlMain);
			this.Controls.Add(this.horzSplitter);
			this.Controls.Add(this.lbCaption);
			this.Name = "XtraFrame";
			this.Size = new System.Drawing.Size(400, 276);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		public bool SelectItem(ListBox lb, int n) {
			bool b = true;
			if(lb.Items.Count > n)
				lb.SelectedIndex = n;
			else if(lb.Items.Count > 0)
				lb.SelectedIndex = lb.Items.Count - 1;
			else
				b = false;
			return b;
		}
		public virtual void EndInitialize() { }
	}
	public delegate void RefreshWizardEventHandler(object sender, RefreshWizardEventArgs e);
	public class RefreshWizardEventArgs : EventArgs {
		string name = "";
		string condition = "";
		public string Name {
			get { return name; }
			set { name = value; }
		}
		public string Condition {
			get { return condition; }
			set { condition = value; }
		}
		public RefreshWizardEventArgs(string name, string condition) {
			this.name = name;
			this.condition = condition;
		}
	}
}
