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
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Security {
	internal interface IPersistentType { }
	public class PermissionTargetBusinessClassListConverter : LocalizedClassInfoTypeConverter {
		public static Type CommonBaseObjectType = typeof(object);
		public static Type PersistentBaseObjectType = typeof(IPersistentType);
		public static string CommonBaseObjectTypeCaption = "All Business Classes";
		public static string PersistentBaseObjectTypeCaption = "All Persistent Business Classes";
		public const string CommonBaseObjectTypeItemName = "AllBusinessClasses";
		public const string PersistentBaseObjectTypeItemName = "AllPersistentBusinessClasses";
		public const string SecurityLocalizationGroupName = "Security";
		public PermissionTargetBusinessClassListConverter()
			: base() {
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object val, Type destType) {
			if(destType == typeof(string)) {
				Type classInfo = val as Type;
				if(classInfo == CommonBaseObjectType) {
					return CaptionHelper.GetLocalizedText(SecurityLocalizationGroupName, CommonBaseObjectTypeItemName, CommonBaseObjectTypeCaption);
				}
				else if(classInfo == PersistentBaseObjectType) {
					return CaptionHelper.GetLocalizedText(SecurityLocalizationGroupName, PersistentBaseObjectTypeItemName, PersistentBaseObjectTypeCaption);
				}
			}
			return base.ConvertTo(context, culture, val, destType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object val) {
			if(val != null) {
				string caption = val.ToString();
				if(caption == CaptionHelper.GetLocalizedText(SecurityLocalizationGroupName, CommonBaseObjectTypeItemName, CommonBaseObjectTypeCaption)) {
					return CommonBaseObjectType;
				}
				else if(caption == CaptionHelper.GetLocalizedText(SecurityLocalizationGroupName, PersistentBaseObjectTypeItemName, PersistentBaseObjectTypeCaption)) {
					return PersistentBaseObjectType;
				}
			}
			return base.ConvertFrom(context, culture, val);
		}
		public override void AddCustomItems(List<Type> list) {
			list.Insert(0, CommonBaseObjectType);
			list.Insert(1, PersistentBaseObjectType);
		}
	}
}
