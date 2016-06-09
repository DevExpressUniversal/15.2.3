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
using System.Collections.Specialized;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Web {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class RequestProcessorUrlKeyParameterHelper {
		public static void UpdateParameters(NameValueCollection parameters, object currentObject, IMemberInfo memberInfo) {
			string mediaDataKey = string.Empty;
			if(currentObject != null) {
				MediaDataObjectAttribute mediaDataAttribute = memberInfo.MemberTypeInfo.FindAttribute<MediaDataObjectAttribute>(false);
				if(mediaDataAttribute != null) {
					mediaDataKey = GetMediaDataKey(currentObject, memberInfo, mediaDataAttribute);
					if(string.IsNullOrEmpty(mediaDataKey)) {
						parameters.Clear(); 
					}
					else {
						parameters["PropertyName"] = memberInfo.Name + "." + mediaDataAttribute.MediaDataDataViewBindingProperty;
					}
				}
			}
			SetUrlKeyParameter(parameters, mediaDataKey);
		}
		private static string GetMediaDataKey(object currentObject, IMemberInfo memberInfo, MediaDataObjectAttribute mediaDataAttribute) {
			string mediaDataKey = string.Empty;
			string mediaDataKeyPropertyName = memberInfo.Name + "." + mediaDataAttribute.MediaDataKeyProperty;
			object mediaDataKeyValue = null;
			if(currentObject is XafDataViewRecord) {
				if(((XafDataViewRecord)currentObject).ContainsMember(mediaDataKeyPropertyName)) {
					mediaDataKeyValue = ((XafDataViewRecord)currentObject)[mediaDataKeyPropertyName];
				}
			}
			else {
				IMemberInfo mediaDataKeyProperty = memberInfo.Owner.FindMember(mediaDataKeyPropertyName);
				mediaDataKeyValue = mediaDataKeyProperty.GetValue(currentObject);
			}
			if(mediaDataKeyValue != null) {
				mediaDataKey = mediaDataKeyValue.ToString();
			}
			return mediaDataKey;
		}
		private static void SetUrlKeyParameter(NameValueCollection parameters, string mediaKey) {
			if(parameters.Count > 0) {
				if(!string.IsNullOrEmpty(mediaKey)) {
					parameters["mediaKey"] = mediaKey;
				}
				else {
					parameters["TimeStamp"] = DateTime.Now.GetHashCode().ToString();
				}
			}
		}
	}
}
