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
using System.Text;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public class UploadControlButtonPropertiesBase : PropertiesBase, IPropertiesOwner {
		private string defaultText = "";
		private ImagePropertiesBase imageInternal = null;
		protected internal ImagePropertiesBase ImageInternal {
			get { return imageInternal; }
		}
		[
		DefaultValue(ImagePosition.Left), NotifyParentProperty(true), AutoFormatEnable]
		public virtual ImagePosition ImagePosition {
			get { return (ImagePosition)GetEnumProperty("ImagePosition", ImagePosition.Left); }
			set {
				SetEnumProperty("ImagePosition", ImagePosition.Left, value);
				Changed();
			}
		}
		[
		NotifyParentProperty(true), Localizable(true), AutoFormatEnable]
		public virtual string Text {
			get { return GetStringProperty("Text", defaultText); }
			set {
				SetStringProperty("Text", defaultText, value);
				Changed();
			}
		}
		public UploadControlButtonPropertiesBase() {
		}
		public UploadControlButtonPropertiesBase(IPropertiesOwner owner, string defaultText)
			: base(owner) {
			this.defaultText = defaultText;
			this.imageInternal = CreateImageProperties(owner);
		}
		protected virtual ImagePropertiesBase CreateImageProperties(IPropertiesOwner owner) {
			return new ImagePropertiesBase(owner);
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if(source is UploadControlButtonPropertiesBase) {
					UploadControlButtonPropertiesBase src = source as UploadControlButtonPropertiesBase;
					ImageInternal.Assign(src.ImageInternal);
					ImagePosition = src.ImagePosition;
					Text = src.Text;
				}
			} finally {
				EndUpdate();
			}
		}
		protected internal virtual string GetButtonIDSuffix() {
			return "";
		}
		protected bool ShouldSerializeText() {
			return Text != defaultText;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { ImageInternal };
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	[AutoFormatUrlPropertyClass]
	public class UploadControlButtonProperties : UploadControlButtonPropertiesBase {
		[
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ImagePropertiesEx Image {
			get { return (ImagePropertiesEx)base.ImageInternal; }
		}
		public UploadControlButtonProperties()
			: base() {
		}
		public UploadControlButtonProperties(IPropertiesOwner owner, string defaultText)
			: base(owner, defaultText) {
		}
		protected override ImagePropertiesBase CreateImageProperties(IPropertiesOwner owner) {
			return new ImagePropertiesEx(owner);
		}
	}
	public class AddButtonProperties : UploadControlButtonProperties {
		public AddButtonProperties(IPropertiesOwner owner)
			: base(owner, ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_AddButton)) {
		}
		protected internal override string GetButtonIDSuffix() {
			return "Add";
		}
	}
	[AutoFormatUrlPropertyClass]
	public class BrowseButtonProperties : UploadControlButtonPropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("BrowseButtonPropertiesImage"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ButtonImageProperties Image {
			get { return (ButtonImageProperties)base.ImageInternal; }
		}
		public BrowseButtonProperties()
			: base() {
		}
		public BrowseButtonProperties(IPropertiesOwner owner)
			: base(owner, ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_BrowseButton)) {
		}
		protected internal override string GetButtonIDSuffix() {
			return "Browse";
		}
		protected override ImagePropertiesBase CreateImageProperties(IPropertiesOwner owner) {
			return new ButtonImageProperties(owner);
		}
	}
	public class RemoveButtonProperties : UploadControlButtonProperties {
		public RemoveButtonProperties(IPropertiesOwner owner)
			: base(owner, ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_RemoveButton)) {
		}
		protected internal override string GetButtonIDSuffix() {
			return "Remove";
		}
	}
	public class UploadButtonProperties : UploadControlButtonProperties {
		public UploadButtonProperties(IPropertiesOwner owner)
			: base(owner, ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_UploadButton)) {
		}
		protected internal override string GetButtonIDSuffix() {
			return "Upload";
		}
	}
	public class CancelButtonProperties : UploadControlButtonProperties {
		public CancelButtonProperties(IPropertiesOwner owner)
			: base(owner, ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_CancelButton)) {
		}
		protected internal override string GetButtonIDSuffix() {
			return "Cancel";
		}
	}
	public class RemoveRowButtonProperties : UploadControlButtonProperties
	{
		public RemoveRowButtonProperties(IPropertiesOwner owner)
			: base(owner, ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_CancelButton)) {
		}
		protected internal override string GetButtonIDSuffix() {
			return "RemoveRow";
		}
	}
}
