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
using System.Reflection;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
namespace DevExpress.Persistent.Base {
	public class DimensionPropertyExtractor : IDimensionPropertyExtractor {
		public static bool IsVisibleInAnalysis(IMemberInfo memberInfo) {
			return memberInfo != null ? !memberInfo.IsList : false;
		}
		public virtual string[] GetDimensionProperties(Type type, Predicate<Type> predicate) {
			if(type != null) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
				if(typeInfo != null) {
					List<string> propertyNames = new List<string>();
					foreach(IMemberInfo memberInfo in typeInfo.Members) {
						if(memberInfo.IsPublic && IsVisibleInAnalysis(memberInfo) && (predicate == null || predicate(memberInfo.Owner.Type))) {
							propertyNames.Add(memberInfo.Name);
						}
					}
					return propertyNames.ToArray();
				}
			}
			return new string[] { };
		}
		public virtual string[] GetDimensionProperties(Type type) {
			return GetDimensionProperties(type, null);
		}
		public static IDimensionPropertyExtractor Instance {
			get {
				IValueManager<IDimensionPropertyExtractor> manager = ValueManager.GetValueManager<IDimensionPropertyExtractor>("DimensionPropertyExtractor_IDimensionPropertyExtractor");
				if(manager.Value == null) {
					manager.Value = new DimensionPropertyExtractor();
				}
				return manager.Value; 
			}
			set {
				ValueManager.GetValueManager<IDimensionPropertyExtractor>("DimensionPropertyExtractor_IDimensionPropertyExtractor").Value = value; 
			}
		}
	}
}
