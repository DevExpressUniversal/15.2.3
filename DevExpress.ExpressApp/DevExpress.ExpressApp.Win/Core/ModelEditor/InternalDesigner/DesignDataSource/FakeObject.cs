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
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor.InternalDesigner {
	internal class FakeObject {
		private int displayValueIndex;
		private int key;
		public FakeObject(int displayValueIndex) {
			this.displayValueIndex = displayValueIndex;
			this.key = GetHashCode();
		}
		public int DisplayValueIndex {
			get { return displayValueIndex; }
		}
		public int Key {
			get { return key; }
		}
		public object GetValue(IMemberInfo memberInfo) {
			Type memberType = memberInfo.MemberType;
			if(memberType == typeof(string)) {
				return memberInfo.Name + " " + DisplayValueIndex.ToString();
			}
			if(memberType == typeof(int)) {
				return DisplayValueIndex;
			}
			if(memberType == typeof(Int64)) {
				return (Int64)DisplayValueIndex;
			}
			if(memberType == typeof(double)) {
				return (double)DisplayValueIndex;
			}
			if(memberType == typeof(float)) {
				return (float)DisplayValueIndex;
			}
			if(memberType == typeof(bool)) {
				return DisplayValueIndex % 2 == 0;
			}
			return null;
		}
	}
}
