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
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	internal class ASPxBooleanColumnCreator : ASPxEnumColumnCreator {
		public ASPxBooleanColumnCreator(IModelColumn columnInfo)
			: base(columnInfo) {
		}
		public override bool CanFormatPropertyValue {
			get {
				return false;
			}
		}
		protected override GridViewDataColumn CreateGridViewColumnCore() {
			GridViewDataColumn result = null;
			if(IsDefinedBoolImages || IsDefinedBoolCaption) {
				result = base.CreateGridViewColumnCore();
			}
			else {
				result = new GridViewDataCheckColumn();
				((GridViewDataCheckColumn)result).PropertiesCheckEdit.AllowGrayed = false;
			}
			return result;
		}
		protected override Type ValueType {
			get {
				return typeof(bool);
			}
		}
		protected override void SetupComboBoxItems(DevExpress.Web.Internal.IListEditItemsRequester control) {
			string trueText = true.ToString();
			string falseText = false.ToString();
			if(IsDefinedBoolCaption) {
				trueText = Model.CaptionForTrue;
				falseText = Model.CaptionForFalse;
			}
			if(IsDefinedBoolImages) {
				ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(Model.ImageForFalse);
				control.Items.Add(falseText, false, !imageInfo.IsUrlEmpty ? imageInfo.ImageUrl : string.Empty);
				imageInfo = ImageLoader.Instance.GetImageInfo(Model.ImageForTrue);
				control.Items.Add(trueText, true, !imageInfo.IsUrlEmpty ? imageInfo.ImageUrl : string.Empty);
			}
			else {
				control.Items.Add(falseText, false);
				control.Items.Add(trueText, true);
			}
		}
		protected bool IsDefinedBoolImages {
			get {
				return Model.ImageForFalse != null && Model.ImageForTrue != null;
			}
		}
		protected bool IsDefinedBoolCaption {
			get {
				return Model.CaptionForFalse != null && Model.CaptionForTrue != null;
			}
		}
	}
}
