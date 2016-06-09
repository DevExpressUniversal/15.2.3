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
using System.Text;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Web.Editors {
	public class WebAlignmentProvider {
		private static List<Type> alignedToCenterTypes = new List<Type>();
		static WebAlignmentProvider() {
			alignedToCenterTypes.AddRange(new Type[]  {				
				typeof(Boolean),
				typeof(Boolean?),
				typeof(System.Drawing.Image),
				typeof(byte[])
			});
		}
		private static bool IsNeedAlignToCenter(Type type) {
			foreach(Type needToCenterType in alignedToCenterTypes) {
				if(needToCenterType.IsAssignableFrom(type)) {
					return true;
				}
			}
			return false;
		}
		public static HorizontalAlign GetAlignment(Type type) {
			Alignment alignment;
			if(IsNeedAlignToCenter(type)) {
				alignment = Alignment.Center;
			}
			else {
				alignment = AlignmentProvider.GetAlignment(type);
			}
			HorizontalAlign result = HorizontalAlign.NotSet;
			switch(alignment) {
				case Alignment.Right:
					result = HorizontalAlign.Right;
					break;
				case Alignment.Left:
					result = HorizontalAlign.Left;
					break;
				case Alignment.Center:
					result = HorizontalAlign.Center;
					break;
				case Alignment.Justify:
					result = HorizontalAlign.Justify;
					break;
			}
			return result;
		}
	}
}
