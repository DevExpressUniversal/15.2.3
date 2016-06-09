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
using System.Linq;
using System.Text;
using DevExpress.Xpf.PivotGrid.Internal;
using System.Windows;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.PivotGrid.Printing {
	public class PrintGroupHeader : GroupHeader {
		public PrintGroupHeader() { }
		protected override void SetDefaultStyleKey() {
			this.SetDefaultStyleKey(typeof(PrintGroupHeader));
		}
		protected override FieldHeader CreateHeader() {
			return new PrintFieldHeader() {
				BorderThickness = this.BorderThickness, Padding = this.Padding
			};
		}
		protected internal void EnsureHeadersBorder() {
			foreach(UIElement element in Panel.Children) {
				PrintFieldHeader header = element as PrintFieldHeader;
				if(header != null)
					SetHeaderBorder(header);
			}
		}
		void SetHeaderBorder(PrintFieldHeader header) {
			Thickness thickness = header.BorderThickness;
			thickness.Bottom = BorderThickness.Bottom;
			thickness.Top = BorderThickness.Top;
			header.BorderThickness = thickness;
		}
		protected override GroupCollapseButton CreateGroupCollapseButton() {
			return new PrintGroupCollapseButton();
		}
		protected override void CreateDragDropElementHelper() { }
		protected override void SubscribeEvents(PivotGridField field) { }
		protected override void UnsubscribeEvents(PivotGridField field) { }
		protected override bool? GetIsMustBindToFieldWidth() {
			return null;
		}
	}
	public class PrintGroupCollapseButton : GroupCollapseButton {
		public PrintGroupCollapseButton() : base() { }
		protected override void SetDefaultStyleKey() {
			this.SetDefaultStyleKey(typeof(PrintGroupCollapseButton));
		}
	}
}
