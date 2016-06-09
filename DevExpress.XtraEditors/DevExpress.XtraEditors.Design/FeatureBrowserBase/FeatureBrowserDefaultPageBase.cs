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
namespace DevExpress.XtraEditors.FeatureBrowser {
	public class FeatureBrowserDefaultPageBase : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		FeatureLabelInfo labelInfo;
		SplitContainerControl splitContainer;
		PanelControl pnlFeatureInfo;
		public FeatureBrowserDefaultPageBase() {
			this.splMain.Dispose();
			this.splMain = null;
			this.pnlMain.Dispose();
			this.pnlMain = null;
			this.pnlControl.Visible = false;
			this.splitContainer = new SplitContainerControl();
			this.splitContainer.Parent = this;
			this.splitContainer.Dock = DockStyle.Fill;
			this.splitContainer.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
			this.splitContainer.SplitterPosition = Width * 2 / 5;
			this.splitContainer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.splitContainer.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.splitContainer.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.splitContainer.BringToFront();
			this.pnlFeatureInfo = new PanelControl();
			this.pnlFeatureInfo.Parent = this.splitContainer.Panel1;
			this.pnlFeatureInfo.Dock = DockStyle.Fill;
			this.labelInfo = new FeatureLabelInfo();
			this.labelInfo.Parent = pnlFeatureInfo;
			this.labelInfo.Dock = DockStyle.Fill;
			this.labelInfo.ItemClick += OnLabelInfoItemClick;
			this.pgMain.Parent = this.splitContainer.Panel2;
			this.pgMain.PropertySort = PropertySort.Alphabetical;
		}
		protected virtual void UpdateTextInfo() {
			this.labelInfo.Refresh();
		}
		protected override void OnPropertyGridPropertyValueChanged(object sender, System.Windows.Forms.PropertyValueChangedEventArgs e) {
			base.OnPropertyGridPropertyValueChanged(sender, e);
			UpdateTextInfo();
		}
		public override void DoInitFrame() {
			base.DoInitFrame();
			this.labelInfo.SourceObject = EditingObject;
			this.labelInfo.Text = EmbeddedFrameInit.Description;
			FillPropertyGrid();
		}
		protected override string GetDescriptionTextCore() {
			return string.Empty;
		}
		void OnLabelInfoItemClick(object sender, LabelInfoItemClickEventArgs e) {
			FeatureLabelInfoText featureInfo = e.InfoText.Tag as FeatureLabelInfoText;
			IFeatureBrowserPageOwner pageOwner = FrameOwner != null ? FrameOwner as IFeatureBrowserPageOwner : null;
			if(featureInfo != null) {
				if(!(pageOwner != null && pageOwner.GotoFeatureName(featureInfo.GotoFeatureName))) {
					if(featureInfo.GotoName != string.Empty) {
						if(pageOwner != null)
							pageOwner.Goto(featureInfo.GotoName, featureInfo.GotoValue);
					}
					OnLabelInfoItemClickSelectProperty(featureInfo.PropertyName);
				}
			}
		}
		void OnLabelInfoItemClickSelectProperty(string propertyName) {
			if(propertyName == "") return;
			if(!pgMain.SelectItem(propertyName)) return;
			if(pgMain.SelectedGridItem != null && pgMain.SelectedGridItem.PropertyDescriptor != null) {
				if(pgMain.SelectedGridItem.PropertyDescriptor.PropertyType == typeof(bool)) {
					if(pgMain.SelectedGridItem.Parent != null && pgMain.SelectedGridItem.Parent.Value != null)
						pgMain.SelectedGridItem.PropertyDescriptor.SetValue(pgMain.SelectedGridItem.Parent.Value, !(bool)pgMain.SelectedGridItem.Value);
					this.labelInfo.Refresh();						
					pgMain.SelectedGridItem.Select();
				}
			}
		}
		void FillPropertyGrid() {
			this.splitContainer.PanelVisibility  = IsPropertyGridVisible ? SplitPanelVisibility.Both : SplitPanelVisibility.Panel1;
			this.pnlFeatureInfo.BorderStyle = IsPropertyGridVisible ? DevExpress.XtraEditors.Controls.BorderStyles.Default : DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlFeatureInfo.PerformLayout();
			if(!IsPropertyGridVisible) return;
			FilterObject filterObject = new FilterObject(EditingObject, EmbeddedFrameInit.Properties);
			pgMain.ToolbarVisible = filterObject.Events.Count > 0;
			pgMain.CommandsVisibleIfAvailable = false;
		}
		protected bool IsPropertyGridVisible { 
			get { 
				return this.EmbeddedFrameInit != null && this.EmbeddedFrameInit.Properties != null && this.EmbeddedFrameInit.Properties.Length > 0; 
			} 
		}
		protected override void RefreshPropertyGridCore() {
			base.RefreshPropertyGridCore();
			ShowCorrectPropertyGridTabs();
		}
		void ShowCorrectPropertyGridTabs() {
			if(!pgMain.ToolbarVisible) return;
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(pgMain.SelectedObject);
			EventDescriptorCollection events = TypeDescriptor.GetEvents(pgMain.SelectedObject);
			if(properties.Count == 0 && events.Count > 0)
				pgMain.SelectEventsTab();
		}
	}
	public interface IFeatureLabelInfoText {
		string PropertyName { get; }
		Color Color { get; set; }
	}
	internal class FeatureLabelInfoText : IFeatureLabelInfoText {
		const string gotoIndicator = "goto:";
		const string gotoFeatureIndicator = "gotoFeature:";
		FeatureLabelInfo label;
		string propName;
		string propCondition;
		string text;
		string gotoName;
		string gotoFeatureName;
		string gotoValue;
		string originalText;
		Color color = Color.Empty;
		public FeatureLabelInfoText(FeatureLabelInfo label, string text) {
			this.label = label;
			this.text = text;
			this.originalText = text;
			this.propName = "";
			this.propCondition = "";
			this.gotoName = "";
			this.gotoFeatureName = "";
			this.gotoValue = "";
			CreatePropInfo();
		}
		public FeatureLabelInfo Label { get {	return label; } set { label = value; } }
		public object SourceObject { get {	return Label.SourceObject; } }
		public string Text { get {	return text; }  }
		public string OriginalText { get {	return originalText; }  }
		public string PropertyName { get { return propName; } }
		public bool IsPropertyTypeBoolean { 
			get { 
				PropertyDescriptor propertyDescriptor = new ObjectValueGetter(SourceObject).GetPropertyDescriptor(PropertyName);
				return propertyDescriptor != null ? propertyDescriptor.PropertyType == typeof(bool) : false;
			} 
		}
		public string GotoFeatureName { get { return gotoFeatureName; } }
		public string GotoName { get { return gotoName; } }
		public string GotoValue { get { return gotoValue; } }
		public Color Color {
			get {
				if(color.IsEmpty)
					color = IsTrue ? Color.FromArgb(0xf7, 0x94, 0x1e) : Color.FromArgb(0x39, 0x68, 0xb2);
				return color;
			}
			set { color = value; }
		}
		public object Value {
			get {
				return new ObjectValueGetter(SourceObject).GetValue(PropertyName);
			}
		}
		bool IsTrue {
			get {	
				if(SourceObject == null || propCondition == string.Empty) return false;
				return new ObjectCondition(SourceObject, propCondition).Run();
			}
		}
		void CreatePropInfo() {
			int start = text.IndexOf('{');
			int end = text.IndexOf('}');
			if(start < end) {
				gotoName = GetGotoValue(gotoIndicator, start, end);
				gotoFeatureName = GetGotoValue(gotoFeatureIndicator, start, end);
			}
			if(gotoName == "" && gotoFeatureName == "") {
				string prop = text;
				if(start < end) {
					prop = text.Substring(start + 1, end - start - 1);
					text = text.Remove(start, end - start + 1);
				}
				if(prop != "")
					CreatePropInfo(prop);
			}
		}
		string GetGotoValue(string gotoIndicatorString, int start, int end) {
			string value = "";
			int pos = text.IndexOf(gotoIndicatorString);
			if(pos > -1) {
				value = text.Substring(pos + gotoIndicatorString.Length, end - pos - gotoIndicatorString.Length);
				pos = value.IndexOf(',');
				if(pos > 0) {
					this.gotoValue = value.Substring(pos + 1, value.Length - pos - 1);
					value = value.Substring(0, pos);
				}
				text = text.Remove(start, end - start + 1);
			}
			return value;
		}
		void CreatePropInfo(string propText) {
			propName = propText;
			int valIndex = propText.IndexOf('(');
			if(valIndex < 0) return;
			propCondition = propText.Substring(valIndex, propText.Length - valIndex);
			propName = propText.Substring(0, valIndex);
			if(propName == string.Empty)
				propName = Text;
		}
	}
	public class FeatureLabelInfo : LabelInfo, ISupportLookAndFeel {
		object sourceObject;
		public FeatureLabelInfo() {
			this.sourceObject = null;
			userLookAndFeelCore = new LookAndFeel.Helpers.ControlUserLookAndFeel(this);
		}
		public object SourceObject { get { return sourceObject; } set { sourceObject = value; } }
		public new string Text {
			get { return base.Text; }
			set {
				CreateLabelInfoTexts(value);
			}
		}
		public override void Refresh() {
			this.SuspendTextChanges();
			for(int i = 0; i < Texts.Count; i ++)
				UpdateCheckState(Texts[i]);
			this.ResumeTextChanges(false);
			base.Refresh();
		}
		protected override Color GetColor(LabelInfoText info) {
			FeatureLabelInfoText infoText = info.Tag as FeatureLabelInfoText;
			if(infoText != null) {
				return infoText.Color;
			}
			else return info.Color == Color.Empty ? LookAndFeelHelper.GetSystemColorEx(LookAndFeel, SystemColors.ControlText) : info.Color;
		}
		void CreateLabelInfoTexts(string text) {
			SuspendTextChanges();
			try {
				Texts.Clear();
				char divChar = '[';
				int i = 0;
				string line = "";
				while(i < text.Length) {
					if(text[i] == divChar) {
						if(line != "") {
							AddLabelInfoText(line, divChar == ']');
						}
						divChar = divChar == '[' ? ']' : '[';
						line = "";
					} else line += text[i];
					i ++;
				}
				if(line != "") 
					AddLabelInfoText(line, false);
			}
			finally {
				ResumeTextChanges();
			}
		}
		void AddLabelInfoText(string text, bool active) {
			if(active) {
				FeatureLabelInfoText featureInfo = new FeatureLabelInfoText(this, text);
				LabelInfoText info = Texts.Add(featureInfo.Text, active);
				info.HasCheckBox = featureInfo.IsPropertyTypeBoolean;
				if(info.HasCheckBox && featureInfo.Value != null)
					info.CheckBoxValue = (bool)featureInfo.Value;
				info.Tag = featureInfo;
			} else Texts.Add(text, active);
		}
		void UpdateCheckState(LabelInfoText info) {
			if(!info.HasCheckBox) return;
			FeatureLabelInfoText featureInfo = info.Tag as FeatureLabelInfoText;
			if(featureInfo != null && featureInfo.Value != null)
				info.CheckBoxValue = Convert.ToBoolean(featureInfo.Value);
		}
		bool ISupportLookAndFeel.IgnoreChildren {
			get { return true; }
		}
		UserLookAndFeel userLookAndFeelCore;
		public UserLookAndFeel LookAndFeel {
			get { return userLookAndFeelCore; }
		}
	}
}
