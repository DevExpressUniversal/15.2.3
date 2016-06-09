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

using DevExpress.Design.UI;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using AttributeHelper = DevExpress.Xpf.Core.Design.SmartTags.AttributeHelper;
using DevExpress.Mvvm.UI.Interactivity;
namespace DevExpress.Xpf.Core.Design.Services {
	public class DXServiceInfo {
		public DXTypeInfo TypeInfo { get; private set; }
		public string ToolTip {
			get {
				if(string.IsNullOrEmpty(toolTip))
					toolTip = BehaviorInfoHelper.GetToolTip(string.Format("{0}.{1}", TypeInfo.NameSpace, TypeInfo.Name));
				return toolTip;
			}
		}
		public object Icon {
			get {
				if(icon == null) {
					icon = BehaviorInfoHelper.GetIcon(TypeInfo.Name);
				}
				return icon;
			}
		}
		public DXServiceInfo(DXTypeInfo typeInfo) {
			if(typeInfo == null)
				throw new ArgumentNullException("typeInfo");
			TypeInfo = typeInfo;
		}
		internal DXServiceInfo(Type type) : this(DXTypeInfo.FromType(type)) { }
		public bool GetIsTypeApplicable(Type type) {
			Type behaviorType = TypeInfo.ResolveType();
			if(behaviorType == null)
				return false;
			IEnumerable<TargetTypeAttribute> attributes = AttributeHelper.GetAttributes<TargetTypeAttribute>(behaviorType);
			if(attributes.Count() == 0)
				return true;
			bool applicable = false;
			foreach(var attribute in attributes) {
				if(attribute.IsTargetType) {
					if(!applicable && attribute.TargetType.IsAssignableFrom(type))
						applicable = true;
				} else if(attribute.TargetType.IsAssignableFrom(type))
					return false;
			}
			return applicable;
		}
		string toolTip;
		object icon;
	}
}
