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
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Xml;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Designer.Utils;
namespace DevExpress.XtraEditors.FeatureBrowser {
	public interface IFeatureBrowserPageOwner {
		bool GotoFeatureName(string gotoFeatureName); 
		void Goto(string gotoName, string gotoValue);
	}
	public abstract class FeatureBrowserSampleControlCustomization {
		object sampleObject;
		public FeatureBrowserSampleControlCustomization() {}
		public object SampleObject { get { return sampleObject; } set { sampleObject = value; } }
		public abstract void Setup();
	}
	public class FeatureBrowserFormFeatureSelectedEventArgs : EventArgs {
		bool cancel;
		string featureName;
		public FeatureBrowserFormFeatureSelectedEventArgs(string featureName) {
			this.cancel = true;
			this.featureName = featureName;
		}
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		public string FeatureName { get { return featureName; } }
	}
	public delegate void FeatureBrowserFormFeatureSelectedEvent(object sender, FeatureBrowserFormFeatureSelectedEventArgs e);
	[ToolboxItem(false)]
	public class FeatureBrowserFormBase : XtraUserControl, IEmbeddedFrameOwner, IFeatureBrowserPageOwner {
		public const string FeatureBrowserSettings = "Software\\Developer Express\\Designer\\FeatureBrowser\\";
		protected DevExpress.XtraEditors.SplitContainerControl pnlMain;
		private System.ComponentModel.Container components = null;
		protected FeatureBrowserDefaultPageBase defaultPageDesigner;
		object sourceObject;
		object inspectedObject;
		FeatureBrowserItem featureBrowserItem;
		Label lbSeparator;
		object[] sampleObjects;
		object[] inspectedSampleObjects;
		MessagePanelControl panelTopMessage;
		public FeatureBrowserFormBase() {
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			InitializeComponent();
			this.lbSeparator = new Label();
			this.lbSeparator.Parent = this;
			this.lbSeparator.AutoSize = false;
			this.lbSeparator.Size = new Size(0, 18);
			this.lbSeparator.Dock = DockStyle.Top;
			this.panelTopMessage = new MessagePanelControl(true);
			this.panelTopMessage.Parent = this;
			this.panelTopMessage.Dock = DockStyle.Top;
			this.panelTopMessage.ItemClick += new LabelInfoItemClickEvent(OnLabelInfoItemClick);
			this.panelTopMessage.VisibleChanged += OnPanelTopMessageVisibleChanged;
			ShowMessage("PropertyGridFiltering", "Note that the property grid is filtered. It's showing only the properties and events that are related to the currently selected feature.");
			this.defaultPageDesigner = new FeatureBrowserDefaultPageBase();
			this.defaultPageDesigner.Parent = this.pnlMain.Panel1;
			this.defaultPageDesigner.Dock = DockStyle.Fill;
			this.sourceObject = null;
			this.featureBrowserItem = null;
			this.inspectedObject = null;
			this.sampleObjects = null;
			this.inspectedSampleObjects = null;
		}
		static readonly object featureSelected = new object();
		public event FeatureBrowserFormFeatureSelectedEvent FeatureSelected {
			add { Events.AddHandler(featureSelected, value); }
			remove { Events.RemoveHandler(featureSelected, value); }
		}
		public void SetFeatureProperties(object sourceObject, FeatureBrowserItem featureBrowserItem) {
			this.sourceObject = sourceObject;
			this.featureBrowserItem = featureBrowserItem;
			OnFeatureNameChanged();
			OnSourceObjectChanged();
			OnSampleControlCustomization();
			this.panelTopMessage.FeatureChanged();
			ShowMessage("PropertyGridFiltering", "Note that the property grid is filtered. It's showing only the properties and events that are related to the currently selected feature.");
		}
		public object SourceObject { get { return sourceObject; } }
		public string SourceProperty { get { return FeatureBrowserItem != null ? FeatureBrowserItem.SourceProperty : string.Empty; } }
		public object InspectedObject { get { return inspectedObject; } }
		public FeatureBrowserItem FeatureBrowserItem { get { return featureBrowserItem; } }
		public FeatureBrowserItemPageCollection Pages { get { return FeatureBrowserItem != null ? FeatureBrowserItem.Pages : null; } }
		public FeatureBrowserItemPage DefaultPage { get { return Pages != null ? Pages[string.Empty] : null; } }
		public string Description { get { return DefaultPage != null ? DefaultPage.Description : string.Empty; } }
		public string[] FilteredProperties { get { return DefaultPage != null ? DefaultPage.Properties : new string[] {}; } }
		public object[] SampleObjects { get { return sampleObjects; } }
		public object[] InspectedSampleObjects { get { return inspectedSampleObjects; } }
		public virtual string FeatureId { get { return FeatureBrowserItem != null ? FeatureBrowserItem.Id : string.Empty; } }
		protected virtual string GeneralInfoCaption { get { return FeatureBrowserItem != null ? FeatureBrowserItem.Name : string.Empty; } }
		protected virtual void OnFeatureNameChanged() {
		}
		void IEmbeddedFrameOwner.SourceObjectChanged(IEmbeddedFrame frame, PropertyValueChangedEventArgs e) {
			OnSourceObjectChanged(frame, e);
		}
		bool IFeatureBrowserPageOwner.GotoFeatureName(string gotoFeatureName) {
			return OnLabelInfoItemClickGotoFeatureName(gotoFeatureName);
		}
		void IFeatureBrowserPageOwner.Goto(string gotoName, string gotoValue) {
			OnLabelInfoItemClickGoto(gotoName, gotoValue);
		}
		protected virtual void OnSourceObjectChanged(IEmbeddedFrame frame, PropertyValueChangedEventArgs e) {
		}
		protected void ShowMessage(string id, string text) {
			ShowMessage(id, text, false);
		}
		void OnPanelTopMessageVisibleChanged(object sender, EventArgs e) {
			lbSeparator.Visible = this.panelTopMessage.Visible;
		}
		protected void ShowMessage(string id, string text, bool closeOnFeatureChanging) {
			this.panelTopMessage.Show(id, text, closeOnFeatureChanging);
			lbSeparator.Visible = this.panelTopMessage.Visible;
		}
		void OnSampleControlCustomization() {
			if(SampleObjects == null || SampleObjects.Length == 0 || 
				FeatureBrowserItem == null || FeatureBrowserItem.SampleControlCustomization == null) return;
			FeatureBrowserSampleControlCustomization sampleCustomization = null;
			if(FeatureBrowserItem.SampleControlCustomization != null) {
				ConstructorInfo constructorInfoObj = FeatureBrowserItem.SampleControlCustomization.GetConstructor(Type.EmptyTypes);
				sampleCustomization = constructorInfoObj.Invoke(null) as FeatureBrowserSampleControlCustomization;
			}
			if(sampleCustomization != null) {
				sampleCustomization.SampleObject = SampleObjects[0];
				sampleCustomization.Setup();
			}
		}
		protected virtual void OnSourceObjectChanged() {
			if(SourceObject == null || FeatureBrowserItem == null) return;
			this.sampleObjects = CreateSampleObjects();
			SetInspectedObjects();
			SetupEmbeddedFrame(this.defaultPageDesigner as IEmbeddedFrame, Pages[String.Empty], InspectedObject, InspectedSampleObjects, Pages[String.Empty].Description);
		}
		protected virtual object[] CreateSampleObjects() {
			return null;
		}
		protected void SetupEmbeddedFrame(IEmbeddedFrame frame, FeatureBrowserItemPage page, object srcObject, object[] sampleObjects, string description) {
			EmbeddedFrameInit init = new EmbeddedFrameInit(this, srcObject, description);
			init.Properties = page.Properties;
			init.SampleObjects = sampleObjects;
			init.ExpandAllProperties = page.ExpandAll;
			init.ExpandedPropertiesOnStart = page.ExpandedPropertiesOnStart;
			init.SelectedPropertyOnStart = page.SelectedPropertyOnStart;
			frame.InitEmbeddedFrame(init);
			frame.RefreshPropertyGrid();
		}
		void SetInspectedObjects() {
			inspectedObject = sourceObject;
			inspectedSampleObjects = SampleObjects;
			if(SourceProperty == "")  return;
			inspectedObject = new ObjectValueGetter(inspectedObject).GetValue(SourceProperty);
			if(InspectedSampleObjects != null) {
				for(int i = 0; i < InspectedSampleObjects.Length; i ++)
					InspectedSampleObjects[i] = new ObjectValueGetter(InspectedSampleObjects[i]).GetValue(SourceProperty);
			}
		}
		void OnLabelInfoItemClick(object sender, LabelInfoItemClickEventArgs e) {
			FeatureLabelInfoText featureInfo = e.InfoText.Tag as FeatureLabelInfoText;
			if(featureInfo != null) {
				OnLabelInfoItemClickGotoFeatureName(featureInfo.GotoFeatureName);
			}
		}
		protected virtual bool OnLabelInfoItemClickGotoFeatureName(string gotoFeatureName) {
			if(gotoFeatureName == "") return false;
			FeatureBrowserFormFeatureSelectedEvent handler = (FeatureBrowserFormFeatureSelectedEvent)this.Events[featureSelected];
			if(handler != null) {
				FeatureBrowserFormFeatureSelectedEventArgs e = new FeatureBrowserFormFeatureSelectedEventArgs(gotoFeatureName);
				handler(this, e);
				return !e.Cancel;
			} else return false;
		}
		protected virtual void OnLabelInfoItemClickGoto(string gotoName, string gotoValue) {
		}
		protected string GetFullPathByPropertyGridItem(GridItem gridItem) {
			if(gridItem == null) return string.Empty;
			if(gridItem.ToString().IndexOf("ImmutablePropertyDescriptor") > -1) return string.Empty;
			string fullPath = string.Empty;
			GridItem item = gridItem;
			while(item != null && item.GridItemType == GridItemType.Property) {
				if(fullPath != string.Empty) 
					fullPath = '.' + fullPath;
				fullPath = item.Label + fullPath;
				item = item.Parent;
			}
			return fullPath;
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.pnlMain = new DevExpress.XtraEditors.SplitContainerControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			this.SuspendLayout();
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Horizontal = false;
			this.pnlMain.Location = new System.Drawing.Point(1, 1);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMain.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMain.Size = new System.Drawing.Size(630, 590);
			this.pnlMain.SplitterPosition = 270;
			this.pnlMain.TabIndex = 4;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.pnlMain});
			this.DockPadding.All = 1;
			this.Name = "FeatureBrowserFormBase";
			this.Size = new System.Drawing.Size(632, 592);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
	}
	class MessagePanelControl : XtraPanel {
		CheckEdit chEdit;
		SimpleButton button;
		FeatureLabelInfo info;
		string currentId;
		bool closeOnFeatureChanging;
		public MessagePanelControl() : this(false) { }
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			BackColor = DevExpress.Skins.CommonSkins.GetSkin(LookAndFeel).Colors["Info"];
		}
		public MessagePanelControl(bool visible) {
			Height = 70;
			Padding = new Padding(8, 8, 5, 5);
			this.info = new FeatureLabelInfo();
			this.info.Parent = this;
			this.info.Height = 50;
			this.info.AutoHeight = true;
			this.info.Font = new Font(Font.FontFamily, Font.Size, FontStyle.Regular);
			this.info.Dock = DockStyle.Top;
			this.info.ItemClick += new LabelInfoItemClickEvent(OnInfoItemClick);
			this.info.Resize += new EventHandler(OnInfoResize);
			this.info.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			this.chEdit = new CheckEdit();
			this.chEdit.Parent = this;
			this.chEdit.Text = "Don't display this message again.";
			this.chEdit.Width = this.chEdit.CalcBestSize().Width;
			this.chEdit.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
			this.chEdit.Left = 7;
			this.button = new SimpleButton();
			this.button.Parent = this;
			this.button.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
			this.button.Text = "Close";
			this.button.Left = this.ClientSize.Width - this.button.Width - 8;
			this.button.Click += new EventHandler(OnButtonClick);
			this.button.Size = new System.Drawing.Size(73, 30);
			Visible = visible;
			this.currentId = string.Empty;
			this.closeOnFeatureChanging = false;
			SetHeight();
			this.chEdit.Top = this.info.Bottom + 8;
			this.button.Top = this.info.Bottom + 3;
		}
		static readonly object itemClick = new object();
		public event LabelInfoItemClickEvent ItemClick {
			add { Events.AddHandler(itemClick, value); }
			remove { Events.RemoveHandler(itemClick, value); }
		}
		protected virtual void OnItemClick(LabelInfoItemClickEventArgs e) {
			LabelInfoItemClickEvent handler = (LabelInfoItemClickEvent)this.Events[itemClick];
			if(handler != null) handler(this, e);
		}
		protected void OnInfoItemClick(object sender, LabelInfoItemClickEventArgs e) {
			OnItemClick(e);
		}
		protected void OnInfoResize(object sender, EventArgs e) {
			SetHeight();
		}			
		public void Show(string id, string text, bool closeOnFeatureChanging) {
			if(!IsIdVisible(id)) {
				Visible = false;
				return;
			}
			this.currentId = id;
			this.info.Text = text;
			this.closeOnFeatureChanging = closeOnFeatureChanging;
			Visible = true;
			SetHeight();
		}
		public void FeatureChanged() {
			if(this.closeOnFeatureChanging)
				HideMessage();
		}
		bool IsIdVisible(string id) {
			PropertyStore ps = new PropertyStore(FeatureBrowserFormBase.FeatureBrowserSettings);
			ps.Restore();
			return ps.RestoreBoolProperty(id, true);
		}
		void OnButtonClick(object sender, EventArgs e) {
			if(this.chEdit.Checked) {
				PropertyStore ps = new PropertyStore(FeatureBrowserFormBase.FeatureBrowserSettings);
				ps.AddProperty(this.currentId, false);
				ps.Store();
			}
			HideMessage();
		}
		void HideMessage() {
			Visible = false;
			this.info.Texts.Clear();
			this.currentId = string.Empty;
		}
		void SetHeight() {
			int height = this.info.Height + this.button.Height + 20;
			if(Height != height)
				Height = height;
		}
	}
}
