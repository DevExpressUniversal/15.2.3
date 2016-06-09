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
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.Persistent.Validation {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class CodeRuleAttribute : Attribute {
		private Type declaringClass;
		public CodeRuleAttribute() : base() { }
		public void SetDeclaringClass(Type declaringClass) {
			if(declaringClass == null) {
				throw new ArgumentNullException("declaringClass");
			}
			if(!typeof(IRule).IsAssignableFrom(declaringClass)) {
				throw new ArgumentOutOfRangeException("declaringClass");
			}
			this.declaringClass = declaringClass;
		}
		public IRule CreateRule() {
			if(this.declaringClass == null) {
				throw new InvalidOperationException("declaringClass is null");
			}
			IRule resultRule = (IRule)TypeHelper.CreateInstance(declaringClass);
			if(string.IsNullOrEmpty(resultRule.Properties.Id)) {
				resultRule.Properties.Id = resultRule.GetType().Name;
				if(resultRule.Properties.TargetType != null) {
					resultRule.Properties.Id += string.Format("_{0}", resultRule.Properties.TargetType.Name);
				}
			}
			return resultRule;
		}
	}
}
