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
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.EF.Utils {
	public class ParseCriteriaScope : IDisposable {
		private const String objectTag = "EFObject";
		[ThreadStatic]
		private static IObjectSpace currentObjectSpace;
		private static ITypesInfo typesInfo;
		private IObjectSpace objectSpace;
		private IObjectSpace prevObjectSpace;
		private Boolean isDisposed;
		private static void CriteriaOperator_UserValueToString(Object sender, UserValueProcessingEventArgs e) {
			if(!e.Handled && (e.Value != null)) {
				IObjectSpace objectSpace = null;
				if(e.Value is IObjectSpaceLink) {
					objectSpace = ((IObjectSpaceLink)e.Value).ObjectSpace;
				}
				if(objectSpace == null) {
					objectSpace = ParseCriteriaScope.currentObjectSpace;
				}
				if(objectSpace != null) {
					e.Data = objectSpace.GetObjectHandle(e.Value);
					e.Tag = objectTag;
					e.Handled = true;
				}
				else if(typesInfo != null) {
					ITypeInfo typeInfo = typesInfo.FindTypeInfo(e.Value.GetType());
					if((typeInfo != null) && (((TypeInfo)typeInfo).Source is EFTypeInfoSource)) {
						e.Data = ObjectHandleHelper.CreateObjectHandle(typesInfo, typeInfo.Type, EFObjectSpace.GetKeyValueAsString(typesInfo, e.Value));
						e.Tag = objectTag;
						e.Handled = true;
					}
				}
			}
		}
		private static void CriteriaOperator_UserValueParse(Object sender, UserValueProcessingEventArgs e) {
			if(!e.Handled && !String.IsNullOrWhiteSpace(e.Data) && (e.Tag == objectTag) && (ParseCriteriaScope.currentObjectSpace != null)) {
				try {
					e.Value = ParseCriteriaScope.currentObjectSpace.GetObjectByHandle(e.Data);
					e.Handled = true;
				}
				catch {
					e.Value = e.Data;
				}
			}
		}
		static ParseCriteriaScope() {
			CriteriaOperator.UserValueToString += CriteriaOperator_UserValueToString;
			CriteriaOperator.UserValueParse += CriteriaOperator_UserValueParse;
		}
		public ParseCriteriaScope(IObjectSpace objectSpace) {
			this.objectSpace = objectSpace;
			prevObjectSpace = ParseCriteriaScope.currentObjectSpace;
			ParseCriteriaScope.currentObjectSpace = objectSpace;
		}
		public void Dispose() {
			if(!isDisposed) {
				isDisposed = true;
				if(ParseCriteriaScope.currentObjectSpace != objectSpace) {
					throw new InvalidOperationException("Incorrect ParseCriteriaScope usage detected.");
				}
				ParseCriteriaScope.currentObjectSpace = prevObjectSpace;
				prevObjectSpace = null;
				objectSpace = null;
			}
		}
		public static void Init(ITypesInfo typesInfo) {
			ParseCriteriaScope.typesInfo = typesInfo;
		}
	}
}
