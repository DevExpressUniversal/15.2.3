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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Win.Editors {
	#region Obsolete 15.2
	[Obsolete("Use StaticTextViewItem instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class StaticTextDetailItem : StaticTextViewItem {
		public StaticTextDetailItem(IModelStaticText model, Type objectType): base(objectType, model) {
		}
	}
	#endregion
	public class StaticTextViewItem : StaticText, IHtmlFormattingSupport {
		private bool htmlFormattingEnabled;
		private void ApplyHtmlFormatting() {
			if(Label != null) {
				Label.AllowHtmlString = htmlFormattingEnabled;
			}
		}
		protected override void OnControlCreated() {
			base.OnControlCreated();
			ApplyHtmlFormatting();
		}
		protected override object CreateControlCore() {
			LabelControl result = new LabelControl();
			result.Dock = DockStyle.Fill;
			result.Text = Text;
			result.Tag = EasyTestTagHelper.FormatTestField(Model.Id);
			result.AutoSizeMode = LabelAutoSizeMode.Vertical;
			result.AutoSize = true;
			switch(Model.HorizontalAlign) {
				case StaticHorizontalAlign.Center:
					result.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
					break;
				case StaticHorizontalAlign.Left:
					result.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
					break;
				case StaticHorizontalAlign.Right:
					result.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
					break;
			};
			return result;
		}
		protected override void OnTextChanged(string text) {
			base.OnTextChanged(text);
			if(Label != null) {
				Label.Text = text;
			}
		}
		public StaticTextViewItem(Type objectType, IModelStaticText model) : base(model, objectType) { }
		#region IHtmlFormattingSupport Members
		public void SetHtmlFormattingEnabled(bool htmlFormattingEnabled) {
			this.htmlFormattingEnabled = htmlFormattingEnabled;
			ApplyHtmlFormatting();
		}
		#endregion
		protected override FontStyle FontStyle {
			get {
				if(Label != null) {
					return Label.Font.Style;
				}
				return FontStyle.Regular;
			}
			set {
				if(Label != null) {
					Label.Font = new System.Drawing.Font(Label.Font, value);
				}
			}
		}
		protected override Color FontColor {
			get {
				if(Label != null) {
					return Label.ForeColor;
				}
				return Color.Empty;
			}
			set {
				if(Label != null) {
					Label.ForeColor = value;
				}
			}
		}
		protected override Color BackColor {
			get {
				if(Label != null) {
					return Label.BackColor;
				}
				return Color.Empty;
			}
			set {
				if(Label != null) {
					Label.BackColor = value;
				}
			}
		}
		protected override void ResetBackColor() {
			if(Label != null) {
				Label.Appearance.BackColor = Color.Empty;
			}
		}
		protected override void ResetFontColor() {
			if(Label != null) {
				Label.Appearance.ForeColor = Color.Empty;
			}
		}
		protected override void ResetFontStyle() {
			if(Label != null) {
				if(!DevExpress.Utils.AppearanceObject.DefaultFont.Equals(Label.Font)) {
					Label.Font = DevExpress.Utils.AppearanceObject.DefaultFont; 
				}
			}
		}
		public LabelControl Label {
			get { return Control != null ? (LabelControl)Control : null; }
		}
	}
	#region Obsolete 15.2
	[Obsolete("Use StaticImageViewItem instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class StaticImageDetailItem : StaticImageViewItem {
		public StaticImageDetailItem(IModelStaticImage model, Type objectType)
			: base(objectType, model) {
		}
	}
	#endregion
	public class StaticImageViewItem : StaticImage {
		protected override object CreateControlCore() {
			PictureEdit result = new PictureEdit();
			result.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			result.BackColor = Color.Transparent;
			result.Properties.AllowFocused = false;
			result.Properties.ShowMenu = false;
			SetupControl(result, ImageName, Model.SizeMode);
			return result;
		}
		protected override void OnImageChanged(string imageName) {
			base.OnImageChanged(imageName);
			PictureEdit pictureEdit = Control as PictureEdit;
			if(pictureEdit != null) {
				SetupControl(pictureEdit, imageName, Model.SizeMode);
			}
		}
		private void SetupControl(PictureEdit pictureEdit, string imageName, ImageSizeMode sizeMode) {
			pictureEdit.Image = ImageLoader.Instance.GetImageInfo(imageName).Image;
			if(pictureEdit.Image == null) {
				pictureEdit.Image = new PictureBox().ErrorImage;
			}
			pictureEdit.Properties.PictureAlignment = AlignmentHelper.GetAlignment(Model.HorizontalAlign, Model.VerticalAlign);
			switch(sizeMode) {
				case ImageSizeMode.AutoSize: {
						pictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
						Size pictureEditSize = new Size(pictureEdit.Image.Size.Width + 2, pictureEdit.Image.Size.Height + 2); 
						pictureEdit.MaximumSize = pictureEditSize;
						pictureEdit.MinimumSize = pictureEditSize;
						break;
					}
				case ImageSizeMode.CenterImage: {
						pictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
						pictureEdit.Properties.PictureAlignment = ContentAlignment.MiddleCenter;
						pictureEdit.MaximumSize = Size.Empty;
						break;
					}
				case ImageSizeMode.Normal: {
						pictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
						pictureEdit.MaximumSize = Size.Empty;
						break;
					}
				case ImageSizeMode.StretchImage: {
						pictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
						pictureEdit.MaximumSize = Size.Empty;
						break;
					}
				case ImageSizeMode.Zoom: {
						pictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
						pictureEdit.MaximumSize = Size.Empty;
						break;
					}
			}
			pictureEdit.Size = pictureEdit.Image.Size;
		}
		public StaticImageViewItem(Type objectType, IModelStaticImage model) : base(model, objectType) { }
	}
	public class WinActionContainerViewItem : ActionContainerViewItem {
		private List<ActionBase> deferredActions;
		private ButtonsContainer container;
		protected override object CreateControlCore() {
			container = new ButtonsContainer();
			container.Orientation = Model.Orientation;
			container.PaintStyle = Model.PaintStyle;
			if(Model.ActionContainer != null) {
				container.ContainerId = Model.ActionContainer.Id;
			}
			RegisterDeferredActions();
			return container;
		}
		private void RegisterDeferredActions() {
			foreach(ActionBase action in deferredActions) {
				Register(action);
			}
			deferredActions.Clear();
		}
		public WinActionContainerViewItem(IModelActionContainerViewItem model, Type objectType)
			: base(model, objectType) {
			deferredActions = new List<ActionBase>();
		}
		public override string ContainerId {
			get {
				if(container == null) {
					if(Model.ActionContainer != null) {
						return Model.ActionContainer.Id;
					}
					return null;
				}
				return container.ContainerId;
			}
			set { container.ContainerId = value; }
		}
		public override void Clear() {
			base.Clear();
			if(container != null) {
				container.BeginUpdate();
				container.Clear();
				container.EndUpdate();
			}
			deferredActions.Clear();
			UpdateEmptyViewItemVisibility();
		}
		public override void Register(ActionBase action) {
			if(container == null) {
				deferredActions.Add(action);
			}
			else {
				container.Register(action);
			}
			base.Register(action);
		}
		public override ReadOnlyCollection<ActionBase> Actions {
			get {
				if(container == null) {
					return deferredActions.AsReadOnly();
				}
				return container.Actions;
			}
		}
		public override void BeginUpdate() {
			if(container != null) {
				container.BeginUpdate();
			}
		}
		public override void EndUpdate() {
			if(container != null) {
				container.EndUpdate();
			}
		}
		public ButtonsContainer Container {
			get { return container; }
		}
	}
}
