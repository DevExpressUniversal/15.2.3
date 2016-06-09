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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.ConditionalAppearance {
	public class CachedAppearanceRuleProperties : IAppearanceRuleProperties {
		[DefaultValue(false)]
		public static bool CopyProperties = false;
		public CachedAppearanceRuleProperties(IAppearanceRuleProperties properties) {
			this.AppearanceItemType = properties.AppearanceItemType;
			this.BackColor = properties.BackColor;
			this.Context = properties.Context;
			this.Criteria = properties.Criteria;
			this.DeclaringType = properties.DeclaringType;
			this.Enabled = properties.Enabled;
			this.FontColor = properties.FontColor;
			this.FontStyle = properties.FontStyle;
			this.Method = properties.Method;
			this.Priority = properties.Priority;
			this.TargetItems = properties.TargetItems;
			this.Visibility = properties.Visibility;
		}
		public string TargetItems { get; set; }
		public string AppearanceItemType { get; set; }
		public string Criteria { get; set; }
		public string Method { get; set; }
		public string Context { get; set; }
		public Type DeclaringType { get; set; }
		public int Priority { get; set; }
		public FontStyle? FontStyle { get; set; }
		public Color? FontColor { get; set; }
		public Color? BackColor { get; set; }
		public ViewItemVisibility? Visibility { get; set; }
		public bool? Enabled { get; set; }
	}
}
