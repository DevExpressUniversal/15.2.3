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

using DevExpress.Xpf.Utils.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.PropertyGrid.Themes {
	public enum VSViewRenderTemplateThemeKeys {
		RowControl,
		RowCommandButton,
		RowCollectionButton,
		Expander,
		HeaderHost,
		Separator,
	}
	public enum VSViewBrushThemeKeys {
		BorderBrush,
		Background_Header_Normal,
		Background_Editor_Normal,
		Foreground_Header_Normal,
		Foreground_Editor_Normal,
		Background_Header_Selected,
		Background_Editor_Selected,
		Foreground_Header_Selected,
		Foreground_Editor_Selected,
		Background_Header_SelectedInactive,
		Background_Editor_SelectedInactive,
		Foreground_Header_SelectedInactive,
		Foreground_Editor_SelectedInactive,
		Background_Header_SelectedInactiveReadOnly,
		Background_Editor_SelectedInactiveReadOnly,
		Foreground_Header_SelectedInactiveReadOnly,
		Foreground_Editor_SelectedInactiveReadOnly,
		Background_Header_SelectedReadOnly,
		Background_Editor_SelectedReadOnly,
		Foreground_Header_SelectedReadOnly,
		Foreground_Editor_SelectedReadOnly,
		Background_Header_ReadOnly,
		Background_Editor_ReadOnly,
		Foreground_Header_ReadOnly,
		Foreground_Editor_ReadOnly
	}
	public class VSViewBrushThemeKeyExtension : ThemeKeyExtensionBase<VSViewBrushThemeKeys> { }
	public class VSViewRenderTemplateThemeKeyExtension : ThemeKeyExtensionBase<VSViewRenderTemplateThemeKeys> { }
}
