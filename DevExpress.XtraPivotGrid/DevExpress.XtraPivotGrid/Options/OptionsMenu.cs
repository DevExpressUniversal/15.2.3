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
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraPivotGrid {
	public class PivotGridOptionsMenu : PivotGridOptionsBase {
		bool enableHeaderMenu;
		bool enableHeaderAreaMenu;
		bool enableFieldValueMenu;
		bool enableFormatRulesMenu;
		public PivotGridOptionsMenu() {
			this.enableHeaderMenu = true;
			this.enableHeaderAreaMenu = true;
			this.enableFieldValueMenu = true;
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsMenuEnableHeaderMenu"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool EnableHeaderMenu { get { return enableHeaderMenu; } set { enableHeaderMenu = value; } }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsMenuEnableHeaderAreaMenu"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool EnableHeaderAreaMenu { get { return enableHeaderAreaMenu; } set { enableHeaderAreaMenu = value; } }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsMenuEnableFieldValueMenu"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool EnableFieldValueMenu { get { return enableFieldValueMenu; } set { enableFieldValueMenu = value; } }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsMenuEnableFormatRulesMenu"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool EnableFormatRulesMenu { get { return enableFormatRulesMenu; } set { enableFormatRulesMenu = value; } }
	}
}
