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
using System.Drawing;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	public abstract class EditorSettings: SettingsBase {
		EditPropertiesBase properties;
		public EditorSettings() {
			this.properties = CreateProperties();
			ClientEnabled = true;
		}
		public bool ClientEnabled { get; set; }
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public EditPropertiesBase Properties { get { return properties; } }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() { 
			return null; 
		}
		protected override ImagesBase CreateImages() {
			return null;
		}
		protected override StylesBase CreateStyles() {
			return null;
		}
		protected abstract EditPropertiesBase CreateProperties();
	}
	public class BinaryImageEditSettings : EditorSettings {
		byte[] contentBytes;
		public object CallbackRouteValues { get { return Properties.CallbackRouteValues; } set { Properties.CallbackRouteValues = value; } }
		public byte[] ContentBytes {
			get { return contentBytes; }
			set {
				contentBytes = value;
				ContentBytesAssigned = true;
			}
		}
		protected internal bool ContentBytesAssigned { get; set; }
		public new MVCxBinaryImageEditProperties Properties { get { return (MVCxBinaryImageEditProperties)base.Properties; } }
		public override string ToolTip {
			get { return Properties.ToolTip; }
			set { Properties.ToolTip = value; }
		}
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxBinaryImageEditProperties();
		}
	}
	public class ButtonEditSettings : EditorSettings {
		string text;
		public AutoCompleteType AutoCompleteType { get; set; }
		public bool ReadOnly { get; set; }
		public string Text {
			get { return text; }
			set {
				text = value;
				TextAssigned = true;
			}
		}
		protected internal bool TextAssigned { get; set; }
		public new MVCxButtonEditProperties Properties { get { return (MVCxButtonEditProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool ShowModelErrors { get; set; }
		protected internal string ButtonTemplateContent { get; set; }
		protected internal Action<TemplateContainerBase> ButtonTemplateContentMethod { get; set; }
		public void SetButtonTemplateContent(Action<TemplateContainerBase> contentMethod) {
			ButtonTemplateContentMethod = contentMethod;
		}
		public void SetButtonTemplateContent(string content) {
			ButtonTemplateContent = content;
		}
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxButtonEditProperties();
		}
	}
	public class CalendarSettings : EditorSettings {
		DateTime selectedDate;
		CalendarSelection selectedDates = new CalendarSelection(null);
		SettingsLoadingPanel settingsLoadingPanel;
		public CalendarSettings() {
			this.settingsLoadingPanel = new SettingsLoadingPanel(null);
			LoadingPanelImage = new ImageProperties();
			LoadingPanelStyle = new LoadingPanelStyle();
			RenderIFrameForPopupElements = DefaultBoolean.Default;
		}
		public bool ReadOnly { get; set; }
		public DateTime SelectedDate {
			get { return selectedDate; }
			set {
				selectedDate = value;
				SelectedDateAssigned = true;
			}
		}
		protected internal bool SelectedDateAssigned { get; set; }
		public CalendarSelection SelectedDates {
			get { return selectedDates; }
		}
		public new MVCxCalendarProperties Properties { get { return (MVCxCalendarProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public DateTime VisibleDate { get; set; }
		public DefaultBoolean RenderIFrameForPopupElements { get; set; }
		public bool ShowModelErrors { get; set; }
		public SettingsLoadingPanel SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		public ImageProperties LoadingPanelImage { get; private set; }
		public LoadingPanelStyle LoadingPanelStyle { get; private set; }
		public object CallbackRouteValues { get; set; }
		public EventHandler<CalendarDayCellCreatedEventArgs> DayCellCreated { get; set; }
		public EventHandler<CalendarDayCellInitializeEventArgs> DayCellInitialize { get; set; }
		public EventHandler<CalendarDayCellPreparedEventArgs> DayCellPrepared { get; set; }
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxCalendarProperties();
		}
	}
	public class CheckBoxSettings : EditorSettings {
		bool checkedVal;
		public bool Checked {
			get { return checkedVal; }
			set {
				checkedVal = value;
				CheckedAssigned = true;
			}
		}
		protected internal bool CheckedAssigned { get; set; }
		public bool ReadOnly { get; set; }
		public string Text { get; set; }
		public new MVCxCheckBoxProperties Properties { get { return (MVCxCheckBoxProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool ShowModelErrors { get; set; }
		public bool Native { get; set; }
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxCheckBoxProperties();
		}
	}
	public class CheckBoxListSettings : EditorSettings {
		int selectedIndex;
		public int SelectedIndex {
			get { return selectedIndex; }
			set {
				selectedIndex = value;
				SelectedIndexAssigned = true;
			}
		}
		protected internal bool SelectedIndexAssigned { get; set; }
		public bool ReadOnly { get; set; }
		public new MVCxCheckBoxListProperties Properties { get { return (MVCxCheckBoxListProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool ShowModelErrors { get; set; }
		public bool Native { get; set; }
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxCheckBoxListProperties();
		}
	}
	public class ColorEditSettings : EditorSettings {
		Color color;
		public Color Color {
			get { return color; }
			set {
				color = value;
				ColorAssigned = true;
			}
		}
		protected internal bool ColorAssigned { get; set; }
		public bool ReadOnly { get; set; }
		public new MVCxColorEditProperties Properties { get { return (MVCxColorEditProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool ShowModelErrors { get; set; }
		protected internal string ButtonTemplateContent { get; set; }
		protected internal Action<TemplateContainerBase> ButtonTemplateContentMethod { get; set; }
		public void SetButtonTemplateContent(Action<TemplateContainerBase> contentMethod) {
			ButtonTemplateContentMethod = contentMethod;
		}
		public void SetButtonTemplateContent(string content) {
			ButtonTemplateContent = content;
		}
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxColorEditProperties();
		}
	}
	public class ComboBoxSettings : EditorSettings {
		int selectedIndex;
		SettingsLoadingPanel settingsLoadingPanel;
		public ComboBoxSettings() {
			this.settingsLoadingPanel = new SettingsLoadingPanel(null);
			LoadingPanelImage = new ImageProperties();
			LoadingPanelStyle = new LoadingPanelStyle();
		}
		public object CallbackRouteValues { get; set; }
		public bool ReadOnly { get; set; }
		public int SelectedIndex {
			get { return selectedIndex; }
			set {
				selectedIndex = value;
				SelectedIndexAssigned = true;
			}
		}
		protected internal bool SelectedIndexAssigned { get; set; }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int LoadingPanelDelay { get { return SettingsLoadingPanel.Delay; } set { SettingsLoadingPanel.Delay = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImagePosition LoadingPanelImagePosition { get { return SettingsLoadingPanel.ImagePosition; } set { SettingsLoadingPanel.ImagePosition = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string LoadingPanelText { get { return SettingsLoadingPanel.Text; } set { SettingsLoadingPanel.Text = value; } }
		public ImageProperties LoadingPanelImage { get; private set; }
		public LoadingPanelStyle LoadingPanelStyle { get; private set; }
		public new MVCxComboBoxProperties Properties { get { return (MVCxComboBoxProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public SettingsLoadingPanel SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		public bool ShowModelErrors { get; set; }
		protected internal string ButtonTemplateContent { get; set; }
		protected internal Action<TemplateContainerBase> ButtonTemplateContentMethod { get; set; }
		public void SetButtonTemplateContent(Action<TemplateContainerBase> contentMethod) {
			ButtonTemplateContentMethod = contentMethod;
		}
		public void SetButtonTemplateContent(string content) {
			ButtonTemplateContent = content;
		}
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxComboBoxProperties();
		}
	}
	public class DateEditSettings : EditorSettings {
		DateTime date;
		public DateTime Date {
			get { return date; }
			set {
				date = value;
				DateAssigned = true;
			}
		}
		protected internal bool DateAssigned { get; set; }
		public object CallbackRouteValues { get; set; }
		public bool ReadOnly { get; set; }
		public string PopupCalendarOwnerName { get; set; }
		public new MVCxDateEditProperties Properties { get { return (MVCxDateEditProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool ShowModelErrors { get; set; }
		public EventHandler<CalendarDayCellCreatedEventArgs> CalendarDayCellCreated { get; set; }
		public EventHandler<CalendarDayCellInitializeEventArgs> CalendarDayCellInitialize { get; set; }
		public EventHandler<CalendarDayCellPreparedEventArgs> CalendarDayCellPrepared { get; set; }
		protected internal string ButtonTemplateContent { get; set; }
		protected internal Action<TemplateContainerBase> ButtonTemplateContentMethod { get; set; }
		public void SetButtonTemplateContent(Action<TemplateContainerBase> contentMethod) {
			ButtonTemplateContentMethod = contentMethod;
		}
		public void SetButtonTemplateContent(string content) {
			ButtonTemplateContent = content;
		}
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxDateEditProperties();
		}
	}
	public class MVCxDateEditRangeSettings : DateEditRangeSettings {
		public MVCxDateEditRangeSettings(IPropertiesOwner owner) : base(owner) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new int CalendarColumnCount {
			get { return base.CalendarColumnCount; }
			set { }
		}
	}
	public class DropDownEditSettings : EditorSettings {
		string text;
		public DropDownEditSettings() {
			RenderIFrameForPopupElements = DefaultBoolean.Default;
		}
		public bool ReadOnly { get; set; }
		public string Text {
			get { return text; }
			set {
				text = value;
				TextAssigned = true;
			}
		}
		protected internal bool TextAssigned { get; set; }
		public new MVCxDropDownEditProperties Properties { get { return (MVCxDropDownEditProperties)base.Properties; } }
		public DefaultBoolean RenderIFrameForPopupElements { get; set; }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool ShowModelErrors { get; set; }
		protected internal string ButtonTemplateContent { get; set; }
		protected internal Action<TemplateContainerBase> ButtonTemplateContentMethod { get; set; }
		protected internal string DropDownWindowTemplateContent { get; set; }
		protected internal Action<TemplateContainerBase> DropDownWindowTemplateContentMethod { get; set; }
		public void SetButtonTemplateContent(Action<TemplateContainerBase> contentMethod) {
			ButtonTemplateContentMethod = contentMethod;
		}
		public void SetButtonTemplateContent(string content) {
			ButtonTemplateContent = content;
		}
		public void SetDropDownWindowTemplateContent(Action<TemplateContainerBase> contentMethod) {
			DropDownWindowTemplateContentMethod = contentMethod;
		}
		public void SetDropDownWindowTemplateContent(string content) {
			DropDownWindowTemplateContent = content;
		}
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxDropDownEditProperties();
		}
	}
	public class HyperLinkSettings : EditorSettings {
		string navigateUrl;
		public string NavigateUrl {
			get { return navigateUrl; }
			set {
				navigateUrl = value;
				NavigateUrlAssigned = true;
			}
		}
		protected internal bool NavigateUrlAssigned { get; set; }
		public new MVCxHyperLinkProperties Properties { get { return (MVCxHyperLinkProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxHyperLinkProperties();
		}
	}
	public class ImageEditSettings : EditorSettings {
		string imageUrl;
		public string ImageUrl {
			get { return imageUrl; }
			set {
				imageUrl = value;
				ImageUrlAssigned = true;
			}
		}
		protected internal bool ImageUrlAssigned { get; set; }
		public new MVCxImageEditProperties Properties { get { return (MVCxImageEditProperties)base.Properties; } }
		public override string ToolTip {
			get { return Properties.ToolTip; }
			set { Properties.ToolTip = value; }
		}
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxImageEditProperties();
		}
	}
	public class LabelSettings : EditorSettings {
		string text;
		public string AssociatedControlName { get; set; }
		public string Text {
			get { return text; }
			set {
				text = value;
				TextAssigned = true;
			}
		}
		protected internal bool TextAssigned { get; set; }
		public new MVCxLabelProperties Properties { get { return (MVCxLabelProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxLabelProperties();
		}
	}
	public class ListBoxSettings : EditorSettings {
		int selectedIndex;
		SettingsLoadingPanel settingsLoadingPanel;
		public ListBoxSettings() {
			this.settingsLoadingPanel = new SettingsLoadingPanel(null);
		}
		public object CallbackRouteValues { get; set; }
		public bool ReadOnly { get; set; }
		public int SelectedIndex {
			get { return selectedIndex; }
			set {
				selectedIndex = value;
				SelectedIndexAssigned = true;
			}
		}
		protected internal bool SelectedIndexAssigned { get; set; }
		public new MVCxListBoxProperties Properties { get { return (MVCxListBoxProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public SettingsLoadingPanel SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		public bool ShowModelErrors { get; set; }
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxListBoxProperties();
		}
	}
	public class MemoSettings : EditorSettings {
		string text;
		public bool ReadOnly { get; set; }
		public string Text {
			get { return text; }
			set {
				text = value;
				TextAssigned = true;
			}
		}
		protected internal bool TextAssigned { get; set; }
		public new MVCxMemoProperties Properties { get { return (MVCxMemoProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool ShowModelErrors { get; set; }
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxMemoProperties();
		}
	}
	public class ProgressBarSettings : EditorSettings {
		int position;
		public int Position {
			get { return position; }
			set {
				position = value;
				PositionAssigned = true;
			}
		}
		protected internal bool PositionAssigned { get; set; }
		public new MVCxProgressBarProperties Properties { get { return (MVCxProgressBarProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxProgressBarProperties();
		}
	}
	public class RadioButtonSettings : EditorSettings {
		bool checkedVal;
		public bool Checked {
			get { return checkedVal; }
			set {
				checkedVal = value;
				CheckedAssigned = true;
			}
		}
		protected internal bool CheckedAssigned { get; set; }
		public string GroupName { get; set; }
		public bool ReadOnly { get; set; }
		public new MVCxRadioButtonProperties Properties { get { return (MVCxRadioButtonProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public string Text { get; set; }
		public bool ShowModelErrors { get; set; }
		public bool Native { get; set; }
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxRadioButtonProperties();
		}
	}
	public class RadioButtonListSettings : EditorSettings {
		int selectedIndex;
		public int SelectedIndex {
			get { return selectedIndex; }
			set {
				selectedIndex = value;
				SelectedIndexAssigned = true;
			}
		}
		protected internal bool SelectedIndexAssigned { get; set; }
		public bool ReadOnly { get; set; }
		public new MVCxRadioButtonListProperties Properties { get { return (MVCxRadioButtonListProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool ShowModelErrors { get; set; }
		public bool Native { get; set; }
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxRadioButtonListProperties();
		}
	}
	public class SpinEditSettings : EditorSettings {
		decimal number;
		public decimal Number {
			get { return number; }
			set {
				number = value;
				NumberAssigned = true;
			}
		}
		protected internal bool NumberAssigned { get; set; }
		public bool ReadOnly { get; set; }
		public new MVCxSpinEditProperties Properties { get { return (MVCxSpinEditProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool ShowModelErrors { get; set; }
		protected internal string ButtonTemplateContent { get; set; }
		protected internal Action<TemplateContainerBase> ButtonTemplateContentMethod { get; set; }
		public void SetButtonTemplateContent(Action<TemplateContainerBase> contentMethod) {
			ButtonTemplateContentMethod = contentMethod;
		}
		public void SetButtonTemplateContent(string content) {
			ButtonTemplateContent = content;
		}
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxSpinEditProperties();
		}
	}
	public class TextBoxSettings : EditorSettings {
		string text;
		public AutoCompleteType AutoCompleteType { get; set; }
		public string Text {
			get { return text; }
			set {
				text = value;
				TextAssigned = true;
			}
		}
		protected internal bool TextAssigned { get; set; }
		public bool ReadOnly { get; set; }
		public new MVCxTextBoxProperties Properties { get { return (MVCxTextBoxProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool ShowModelErrors { get; set; }
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxTextBoxProperties();
		}
	}
	public class TimeEditSettings : EditorSettings {
		DateTime dateTime;
		public DateTime DateTime {
			get { return dateTime; }
			set {
				dateTime = value;
				DateTimeAssigned = true;
			}
		}
		protected internal bool DateTimeAssigned { get; set; }
		public bool ReadOnly { get; set; }
		public new MVCxTimeEditProperties Properties { get { return (MVCxTimeEditProperties)base.Properties; } }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool ShowModelErrors { get; set; }
		protected internal string ButtonTemplateContent { get; set; }
		protected internal Action<TemplateContainerBase> ButtonTemplateContentMethod { get; set; }
		public void SetButtonTemplateContent(Action<TemplateContainerBase> contentMethod) {
			ButtonTemplateContentMethod = contentMethod;
		}
		public void SetButtonTemplateContent(string content) {
			ButtonTemplateContent = content;
		}
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxTimeEditProperties();
		}
	}
	public class TokenBoxSettings : EditorSettings {
		SettingsLoadingPanel settingsLoadingPanel;
		public TokenBoxSettings() {
			this.settingsLoadingPanel = new SettingsLoadingPanel(null);
			LoadingPanelImage = new ImageProperties();
			LoadingPanelStyle = new LoadingPanelStyle();
		}
		public object CallbackRouteValues { get; set; }
		public new MVCxTokenBoxProperties Properties { get { return (MVCxTokenBoxProperties)base.Properties; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int LoadingPanelDelay { get { return SettingsLoadingPanel.Delay; } set { SettingsLoadingPanel.Delay = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImagePosition LoadingPanelImagePosition { get { return SettingsLoadingPanel.ImagePosition; } set { SettingsLoadingPanel.ImagePosition = value; } }
		[Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string LoadingPanelText { get { return SettingsLoadingPanel.Text; } set { SettingsLoadingPanel.Text = value; } }
		public ImageProperties LoadingPanelImage { get; private set; }
		public LoadingPanelStyle LoadingPanelStyle { get; private set; }
		public bool ReadOnly { get; set; }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public SettingsLoadingPanel SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		public bool ShowModelErrors { get; set; }
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxTokenBoxProperties();
		}
	}
	public class TrackBarSettings : EditorSettings {
		public decimal Position { get; set; }
		public decimal PositionStart { get { return Position; } set { Position = value; } }
		public decimal PositionEnd { get; set; }
		public bool ReadOnly { get; set; }
		public new MVCxTrackBarProperties Properties { get { return (MVCxTrackBarProperties)base.Properties; } }
		public bool ShowModelErrors { get; set; }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxTrackBarProperties();
		}
	}
}
