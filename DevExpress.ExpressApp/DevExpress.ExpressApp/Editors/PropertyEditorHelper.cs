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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Editors {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class PropertyEditorHelper {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string CalcDisplayFormat(IModelMemberViewItem model, IMemberInfo memberInfo) {
			string displayFormat = null;
			if(model != null) {
				string displayFormatFromModel = model.DisplayFormat;
				if(!string.IsNullOrEmpty(displayFormatFromModel)) {
					displayFormat = displayFormatFromModel;
				}
			}
			if(displayFormat == null && memberInfo != null) {
				displayFormat = FormattingProvider.GetDisplayFormat(memberInfo.MemberType);
			}
			return displayFormat;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string CalcEditMask(IModelMemberViewItem model, IMemberInfo memberInfo) {
			string editMask = null;
			if(model != null) {
				string editMaskFromModel = model.EditMask;
				if(!string.IsNullOrEmpty(editMaskFromModel)) {
					editMask = editMaskFromModel;
				}
			}
			if(editMask == null && memberInfo != null) {
				editMask = FormattingProvider.GetEditMask(memberInfo.MemberType);
			}
			return editMask;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static EditMaskType CalcEditMaskType(IModelMemberViewItem model, IMemberInfo memberInfo) {
			EditMaskType editMaskType = EditMaskType.Default;
			if(model != null) {
				editMaskType = model.EditMaskType;
			}
			else if(memberInfo != null) {
				editMaskType = FormattingProvider.GetEditMaskType(memberInfo.MemberType);
			}
			return editMaskType;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static int CalcMaxLengthCore(int initialMaxLength, int memberInfoValueMaxLength) {
			int result = initialMaxLength;
			if(result < 0) {
				result = 0;
			}
			if(memberInfoValueMaxLength > 0 && (result == 0 || result > memberInfoValueMaxLength)) {
				result = memberInfoValueMaxLength;
			}
			return result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static Type CalcUnderlyingType(IMemberInfo memberInfo) {
			ITypeInfo typeInfo = memberInfo.LastMember.MemberTypeInfo;
			if(typeInfo.IsNullable) {
				return typeInfo.UnderlyingTypeInfo.Type;
			}
			else {
				return typeInfo.Type;
			}
		}
	}
}
