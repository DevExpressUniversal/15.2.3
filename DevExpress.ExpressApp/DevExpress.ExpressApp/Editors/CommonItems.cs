#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Editors {
	public enum StaticHorizontalAlign { NotSet, Left, Center, Right };
	public enum StaticVerticalAlign { NotSet, Top, Middle, Bottom };
	public abstract class StaticText : ViewItem, IAppearanceFormat {
		private IModelStaticText model;
		protected StaticText(IModelStaticText model, Type objectType)
			: base(objectType, model.Id) {
			this.model = model;
		}
		protected virtual void OnTextChanged(string text) { }
		public override bool IsCaptionVisible {
			get { return false; }
		}
		public override string Caption {
			get { return model.Caption; }
			set { model.Caption = value; }
		}
		public string Text {
			get { return model.Text; }
			set {
				model.Text = value;
				OnTextChanged(value);
			}
		}
		public IModelStaticText Model { get { return model; } }
		#region IAppearanceFormat Members
		protected abstract FontStyle FontStyle { get; set; }
		protected abstract Color FontColor { get; set; }
		protected abstract Color BackColor { get; set; }
		protected abstract void ResetFontStyle();
		protected abstract void ResetFontColor();
		protected abstract void ResetBackColor();
		FontStyle IAppearanceFormat.FontStyle {
			get {
				return FontStyle;
			}
			set {
				FontStyle = value;
			}
		}
		Color IAppearanceFormat.FontColor {
			get {
				return FontColor;
			}
			set {
				FontColor = value;
			}
		}
		Color IAppearanceFormat.BackColor {
			get {
				return BackColor;
			}
			set {
				BackColor = value;
			}
		}
		void IAppearanceFormat.ResetFontStyle() {
			ResetFontStyle();
		}
		void IAppearanceFormat.ResetFontColor() {
			ResetFontColor();
		}
		void IAppearanceFormat.ResetBackColor() {
			ResetBackColor();
		}
		#endregion
	}
	public abstract class StaticImage : ViewItem {
		private IModelStaticImage model;
		protected StaticImage(IModelStaticImage model, Type objectType)
			: base(objectType, model.Id) {
			this.model = model;
		}
		protected virtual void OnImageChanged(string imageName) { }
		public override bool IsCaptionVisible {
			get { return false; }
		}
		public override string Caption {
			get { return model.Caption; }
			set { model.Caption = value; }
		}
		public string ImageName {
			get { return model.ImageName; }
			set {
				model.ImageName = value;
				OnImageChanged(value);
			}
		}
		public IModelStaticImage Model { get { return model; } }
	}
	public abstract class ActionContainerViewItem : ViewItem, IActionContainer, IAppearanceVisibility, INotifyAppearanceVisibilityChanged {
		private IModelActionContainerViewItem model;
		private bool hasActiveActions;
		private ViewItemVisibility visibility;
		private void UpdateVisibility(ViewItemVisibility visibility, bool hasActiveActions) {
			ViewItemVisibility oldVisibility = ((IAppearanceVisibility)this).Visibility;
			this.visibility = visibility;
			this.hasActiveActions = hasActiveActions;
			if(((IAppearanceVisibility)this).Visibility != oldVisibility) {
				OnVisibilityChanged();
			}
		}
		private void OnVisibilityChanged() {
			if(VisibilityChanged != null) {
				VisibilityChanged(this, EventArgs.Empty);
			}
		}
		private bool HasActiveActions() {
			foreach(ActionBase action in Actions) {
				if(action.Active) {
					return true;
				}
			}
			return false;
		}
		private void UnsubscribeFromActions() {
			foreach(ActionBase action in Actions) {
				action.Active.ResultValueChanged -= Active_ResultValueChanged;
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			((View)sender).ControlsCreated -= View_ControlsCreated;
			UpdateEmptyViewItemVisibility();
		}
		private void Active_ResultValueChanged(object sender, BoolValueChangedEventArgs e) {
			UpdateEmptyViewItemVisibility();
		}
		protected override void OnControlCreated() {
			if(View != null) {
				View.ControlsCreated -= new EventHandler(View_ControlsCreated);
				View.ControlsCreated += new EventHandler(View_ControlsCreated);
			}
			base.OnControlCreated();
		}
		protected void UpdateEmptyViewItemVisibility() {
			UpdateVisibility(visibility, HasActiveActions());
		}
		protected ActionContainerViewItem(IModelActionContainerViewItem model, Type objectType)
			: base(objectType, model.Id) {
			this.model = model;
			visibility = ViewItemVisibility.Show;
		}
		public IModelActionContainerViewItem Model {
			get { return model; }
		}
		public override string Caption {
			get { return Model.Caption; }
			set { Model.Caption = value; }
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			UnsubscribeFromActions();
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public virtual void Clear() {
			UnsubscribeFromActions();
		}
		public virtual void ClearActions() {
			Clear(); 
		}
		public virtual void Register(ActionBase action) {
			action.Active.ResultValueChanged -= Active_ResultValueChanged;
			action.Active.ResultValueChanged += Active_ResultValueChanged;
			UpdateEmptyViewItemVisibility();
		}
		public abstract string ContainerId { get; set; }
		public abstract ReadOnlyCollection<ActionBase> Actions { get; }
		public abstract void BeginUpdate();
		public abstract void EndUpdate();
		#region IAppearanceVisibility Members
		ViewItemVisibility IAppearanceVisibility.Visibility {
			get {
				if(visibility == ViewItemVisibility.Show && !hasActiveActions) {
					return ViewItemVisibility.ShowEmptySpace;
				}
				return visibility;
			}
			set { UpdateVisibility(value, hasActiveActions); }
		}
		void IAppearanceVisibility.ResetVisibility() { }
		#endregion
		#region INotifyAppearanceVisibilityChanged Members
		public event EventHandler VisibilityChanged;
		#endregion
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ListEditorViewItem : ViewItem {
		private ListEditor editor;
		protected override object CreateControlCore() {
			return editor.CreateControls();
		}
		protected internal override void UpdateErrorMessage(ErrorMessages errorMessages) {
			editor.ErrorMessages.LoadMessages(errorMessages);
			base.UpdateErrorMessage(errorMessages);
		}
		public ListEditorViewItem(ListEditor editor)
			: base(editor.Model != null ? editor.Model.ModelClass.TypeInfo.Type : null, ListView.ListViewControlID) {
			this.editor = editor;
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(!unwireEventsOnly) {
				editor.BreakLinksToControls();
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public override bool IsCaptionVisible {
			get { return false; }
		}
		public ListEditor ListEditor {
			get { return editor; }
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	internal class NestedFrameItem : ViewItem, IFrameContainer {
		private Frame frame;
		protected override Object CreateControlCore() {
			if((frame != null) && (frame.View != null)) {
				frame.View.CreateControls();
				return frame.View.Control;
			}
			else {
				return null;
			}
		}
		protected internal void SetFrame(Frame frame) {
			this.frame = frame;
		}
		public NestedFrameItem(String id, Frame frame)
			: base((frame != null) && (frame.View is ObjectView) ? ((ObjectView)frame.View).ObjectTypeInfo.Type : null, id) {
			this.frame = frame;
		}
		public Frame Frame {
			get { return frame; }
		}
		void IFrameContainer.InitializeFrame() { }
	}
}
