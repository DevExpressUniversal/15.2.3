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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PlatformSize = System.Windows.Size;
namespace DevExpress.XtraDiagram.Extensions {
	public static class SizeExtensions {
		public static PlatformSize ToPlatformSize(this Size size) {
			return new PlatformSize(size.Width, size.Height);
		}
		public static PlatformSize ToPlatformSize(this SizeF size) {
			return new PlatformSize(size.Width, size.Height);
		}
		public static PlatformSize? ToPlatformNullableSize(this SizeF? size) {
			if(size == null) return null;
			return new PlatformSize(size.Value.Width, size.Value.Height);
		}
		public static Rectangle CreateRect(this Size size, Point centerPoint) {
			return new Rectangle(centerPoint.X - size.Width / 2, centerPoint.Y - size.Height / 2, size.Width, size.Height);
		}
		public static Size ApplyPadding(this Size size, Padding padding) {
			return new Size(size.Width + padding.Horizontal, size.Height + padding.Vertical);
		}
		public static Rectangle CreateRect(this Size size) {
			return new Rectangle(Point.Empty, size);
		}
	}
	public static class PlatformSizeExtensions {
		public static Size ToWinSize(this PlatformSize size) {
			return new Size((int)size.Width, (int)size.Height);
		}
	}
}
