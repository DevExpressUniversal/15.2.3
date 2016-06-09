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

using System;
using System.Collections.Generic;
using DevExpress.XtraExport.Implementation;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xlsx {
	partial class XlsxDataAwareExporter {
		readonly Stack<XlGroup> groups = new Stack<XlGroup>();
		XlGroup currentGroup = null;
		public int CurrentOutlineLevel {
			get {
				if(this.currentGroup == null)
					return 0;
				return this.currentGroup.OutlineLevel;
			}
		}
		public IXlGroup BeginGroup() {
			XlGroup group = new XlGroup();
			if (currentGroup != null) {
				group.OutlineLevel = currentGroup.OutlineLevel;
				group.IsCollapsed = currentGroup.IsCollapsed;
			}
			else
				group.OutlineLevel = 1;
			groups.Push(group);
			this.currentGroup = group;
			return group;
		}
		public void EndGroup() {
			groups.Pop();
			if (groups.Count <= 0)
				currentGroup = null;
			else
				currentGroup = groups.Peek();
		}
	}
}
