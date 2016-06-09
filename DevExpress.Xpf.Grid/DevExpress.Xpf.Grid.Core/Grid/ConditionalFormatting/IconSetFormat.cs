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

using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Data;
using DevExpress.Data;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using System.Linq;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using DevExpress.Mvvm.POCO;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Export.Xl;
using System.ComponentModel;
namespace DevExpress.Xpf.Grid.ConditionalFormatting {
	[Obsolete("Use the DevExpress.Xpf.Core.ConditionalFormatting.IconSetElement instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class IconSetElement : DevExpress.Xpf.Core.ConditionalFormatting.IconSetElement { }
	[Obsolete("Use the DevExpress.Xpf.Core.ConditionalFormatting.ThresholdComparisonType instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public enum ThresholdComparisonType {
		GreaterOrEqual,
		Greater,
	}
	[Obsolete("Use the DevExpress.Xpf.Core.ConditionalFormatting.IconSetElementCollection instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class IconSetElementCollection : DevExpress.Xpf.Core.ConditionalFormatting.IconSetElementCollection { }
	[Obsolete("Use the DevExpress.Xpf.Core.ConditionalFormatting.ConditionalFormattingValueType instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public enum ConditionalFormattingValueType {
		Percent,
		Number,
	}
	[Obsolete("Use the DevExpress.Xpf.Core.ConditionalFormatting.IconSetFormat instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class IconSetFormat : DevExpress.Xpf.Core.ConditionalFormatting.IconSetFormat { }
}
