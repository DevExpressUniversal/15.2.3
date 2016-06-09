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

using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxScheduler {
	public class PopupFormStyles : StylesBase {
		public const string ControlStyleStyleName = "ControlStyle";
		public const string CloseButtonStyleName = "CloseButton";
		public const string ContentStyleName = "Content";
		public const string HeaderStyleName = "Header";
		public const string ModalBackgroundStyleName = "ModalBackground";
		public PopupFormStyles(ISkinOwner owner) : base(owner) {
		}
		protected override void PopulateStyleInfoList(System.Collections.Generic.List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(CloseButtonStyleName, delegate() { return new PopupWindowButtonStyle(); } ));
			list.Add(new StyleInfo(ContentStyleName, delegate() { return new PopupWindowContentStyle(); } ));
			list.Add(new StyleInfo(HeaderStyleName, delegate() { return new PopupWindowStyle(); } ));
			list.Add(new StyleInfo(ControlStyleStyleName, delegate() { return new AppearanceStyle(); } ));
			list.Add(new StyleInfo(ModalBackgroundStyleName, delegate() { return new PopupControlModalBackgroundStyle(); } ));
		}
		#region Properties
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public PopupWindowButtonStyle CloseButton {
			get { return (PopupWindowButtonStyle)GetStyle(CloseButtonStyleName); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public PopupWindowContentStyle Content {
			get { return (PopupWindowContentStyle)GetStyle(ContentStyleName); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public PopupWindowStyle Header {
			get { return (PopupWindowStyle)GetStyle(HeaderStyleName); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public AppearanceStyle ControlStyle {
			get { return (AppearanceStyle)GetStyle(ControlStyleStyleName); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public PopupControlModalBackgroundStyle ModalBackground {
			get { return (PopupControlModalBackgroundStyle)GetStyle(ModalBackgroundStyleName); }
		}
		#endregion
	}
}
