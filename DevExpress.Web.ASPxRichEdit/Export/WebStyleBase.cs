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

using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Export {
	#region abstract WebStyleBase
	public abstract class WebStyleBase<T> : IHashtableProvider where T : StyleBase<T> {
		#region Fields
		string styleName;
		bool deleted;
		bool hidden;
		bool semihidden;
		string localizedStyleName;
		string parentStyleName;
		bool isDefault;
		#endregion
		protected WebStyleBase(T modelStyle) {
			this.styleName = modelStyle.StyleName;
			this.deleted = modelStyle.Deleted;
			this.hidden = modelStyle.Hidden;
			this.semihidden = modelStyle.Semihidden;
			this.localizedStyleName = modelStyle.LocalizedStyleName;
			if (modelStyle.Parent != null)
				this.parentStyleName = modelStyle.Parent.StyleName;
			this.isDefault = IsDefaultStyle(modelStyle);
			Initialize(modelStyle);
		}
		protected virtual void Initialize(T modelStyle) { }
		public void FillHashtable(Hashtable result) {
			result["styleName"] = styleName;
			result["deleted"] = deleted;
			result["hidden"] = hidden;
			result["parentStyleName"] = parentStyleName;
			result["semihidden"] = semihidden;
			result["localizedStyleName"] = localizedStyleName;
			result["isDefault"] = isDefault;
			FillHashtableCore(result);
		}
		protected abstract void FillHashtableCore(Hashtable result);
		protected abstract bool IsDefaultStyle(T modelStyle);
	}
	#endregion
}
