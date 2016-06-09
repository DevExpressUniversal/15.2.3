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
using System.Collections;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Web.Editors {
	sealed class DisplayValueComparer : IComparer {
		private readonly LookupEditorHelper helper;
		private readonly string emptyValue;
		private IObjectSpace objectSpace;
		public DisplayValueComparer(LookupEditorHelper helper, string emptyValue, IObjectSpace objectSpace) {
			Guard.ArgumentNotNull(helper, "helper");
			this.helper = helper;
			this.emptyValue = emptyValue;
			this.objectSpace = objectSpace;
		}
		#region IComparer Members
		public int Compare(object x, object y) {
			string xString = helper.GetDisplayText(x, emptyValue, null, objectSpace);
			string yString = helper.GetDisplayText(y, emptyValue, null, objectSpace);
			if(xString == null) {
				return yString == null ? 0 : -1;
			}
			return xString.CompareTo(yString);
		}
		#endregion
	}
}
