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
using DevExpress.XtraRichEdit.Fields;
using System.ComponentModel;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.Utils.Design;
using DevExpress.Snap.Extensions.Localization;
namespace DevExpress.Snap.Extensions.Native.ActionLists {
	public class ContentTypeActionList : FieldActionList<MergefieldField> {
		static string GetFieldType(ContentType contentType) {
			switch (contentType) {
				case ContentType.BarCode: return SNBarCodeField.FieldType;
				case ContentType.Text: return SNTextField.FieldType;
				case ContentType.Image: return SNImageField.FieldType;
				case ContentType.CheckBox: return SNCheckBoxField.FieldType;
				case ContentType.Hyperlink: return SNHyperlinkField.FieldType;
				case ContentType.Sparkline: return SNSparklineField.FieldType;
			}
			throw new NotImplementedException();
		}
		static ContentType GetContentType(Type fieldType) {
			if (fieldType == typeof(SNBarCodeField))
				return ContentType.BarCode;
			if (fieldType == typeof(SNTextField))
				return ContentType.Text;
			if (fieldType == typeof(SNImageField))
				return ContentType.Image;
			if (fieldType == typeof(SNCheckBoxField))
				return ContentType.CheckBox;
			if (fieldType == typeof(SNHyperlinkField))
				return ContentType.Hyperlink;
			if (fieldType == typeof(SNSparklineField))
				return ContentType.Sparkline;
			throw new NotImplementedException();
		}
		public ContentTypeActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		[ResDisplayName(typeof(ResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.ActionList_ContentType")]
		public ContentType ContentType {
			get {
				return GetContentType(ParsedInfo.GetType());
			}
			set {
				if (ContentType != value) {
					ApplyNewValue((controller, newMode) => controller.UpdateFieldType(GetFieldType(value)), value);
					SNSmartTagService smartTagService = (SNSmartTagService)this.Component.Site.GetService(typeof(SNSmartTagService));
					smartTagService.UpdatePopup();
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "ContentType", "ContentType");
		}
	}
	[TypeConverter(typeof(EnumTypeConverter)), ResourceFinder(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum ContentType { Text, BarCode, Image, CheckBox, Hyperlink, Sparkline }
}
