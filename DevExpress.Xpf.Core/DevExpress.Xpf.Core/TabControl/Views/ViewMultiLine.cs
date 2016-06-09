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

using System.ComponentModel;
using System.Windows;
namespace DevExpress.Xpf.Core {
	public class TabControlMultiLineView : TabControlViewBase {
		public static readonly DependencyProperty FixedHeadersProperty = DependencyProperty.Register("FixedHeaders", typeof(bool), typeof(TabControlMultiLineView),
			new PropertyMetadata(false, (d, e) => ((TabControlMultiLineView)d).UpdateViewProperties()));
		public static readonly DependencyProperty HeaderAutoFillProperty = DependencyProperty.Register("HeaderAutoFill", typeof(bool), typeof(TabControlMultiLineView), 
			new PropertyMetadata(false, (d, e) => ((TabControlMultiLineView)d).UpdateViewProperties()));
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TabControlMultiLineViewFixedHeaders")]
#endif
		public bool FixedHeaders { get { return (bool)GetValue(FixedHeadersProperty); } set { SetValue(FixedHeadersProperty, value); } }
		public bool HeaderAutoFill { get { return (bool)GetValue(HeaderAutoFillProperty); } set { SetValue(HeaderAutoFillProperty, value); } }
	}
}
