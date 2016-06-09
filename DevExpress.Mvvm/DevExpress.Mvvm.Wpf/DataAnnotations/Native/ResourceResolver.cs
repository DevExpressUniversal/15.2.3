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

using System.ComponentModel.DataAnnotations;
using System.Resources;
using System.Security;
namespace DevExpress.Mvvm.Native {
	[SecuritySafeCritical]
	public class DataAnnotationsResourcesResolver {
		static ResourceManager annotationsResourceManager;
		internal static ResourceManager AnnotationsResourceManager {
			get {
				if(object.ReferenceEquals(annotationsResourceManager, null)) {
					ResourceManager manager = new ResourceManager("System.ComponentModel.DataAnnotations.Resources.DataAnnotationsResources", typeof(ValidationAttribute).Assembly);
					annotationsResourceManager = manager;
				}
				return annotationsResourceManager;
			}
		}
		public static string MinLengthAttribute_ValidationError { get { return GetResourceString("MinLengthAttribute_ValidationError"); } }
		public static string MinLengthAttribute_InvalidMinLength { get { return GetResourceString("MinLengthAttribute_InvalidMinLength"); } }
		public static string MaxLengthAttribute_InvalidMaxLength { get { return GetResourceString("MaxLengthAttribute_InvalidMaxLength"); } }
		public static string MaxLengthAttribute_ValidationError { get { return GetResourceString("MaxLengthAttribute_ValidationError"); } }
		public static string PhoneAttribute_Invalid { get { return GetResourceString("PhoneAttribute_Invalid"); } }
		public static string CreditCardAttribute_Invalid { get { return GetResourceString("CreditCardAttribute_Invalid"); } }
		public static string EmailAddressAttribute_Invalid { get { return GetResourceString("EmailAddressAttribute_Invalid"); } }
		public static string UrlAttribute_Invalid { get { return GetResourceString("UrlAttribute_Invalid"); } }
		public static string RangeAttribute_ValidationError { get { return GetResourceString("RangeAttribute_ValidationError"); } }
		public static string RegexAttribute_ValidationError { get { return GetResourceString("RegexAttribute_ValidationError"); } }
		public static string CustomValidationAttribute_ValidationError { get { return GetResourceString("CustomValidationAttribute_ValidationError"); } }
		public static string RequiredAttribute_ValidationError { get { return GetResourceString("RequiredAttribute_ValidationError"); } }
		static string GetResourceString(string resourceName) {
			return AnnotationsResourceManager.GetString(resourceName) ?? DataAnnotationsResources.ResourceManager.GetString(resourceName);
		}
	}
}
