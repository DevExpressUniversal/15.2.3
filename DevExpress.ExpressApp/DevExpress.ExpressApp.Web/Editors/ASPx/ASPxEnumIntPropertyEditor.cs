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
using System.Text;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.Web;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxEnumIntPropertyEditor<TEnum> : ASPxEnumPropertyEditor, ITestable {
		protected override object GetControlValueCore() {
			return (int)base.GetControlValueCore();
		}
		protected override Type GetEnumType() {
			return typeof(TEnum);
		}
		protected override string GetPropertyDisplayValue() {
			string caption = descriptor.GetCaption((int)PropertyValue);
			return string.IsNullOrEmpty(DisplayFormat) ? caption : string.Format(DisplayFormat, caption);
		}
		protected override void ReadEditModeValueCore() {
			VerifyValueBelongsToEnum(PropertyValue);
			Editor.SelectedIndex = (int)PropertyValue;
		}
		public ASPxEnumIntPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}
	}
}
