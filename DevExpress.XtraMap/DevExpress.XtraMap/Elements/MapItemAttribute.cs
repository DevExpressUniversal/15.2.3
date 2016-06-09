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
using DevExpress.Map.Native;
using DevExpress.Utils;
namespace DevExpress.XtraMap {
	public class MapItemAttribute : IMapItemAttribute, ISupportObjectChanged {
		internal static MapItemAttribute Create(IMapItemAttribute sourceAttribute) {
			return new MapItemAttribute() { Name = sourceAttribute.Name, Type = sourceAttribute.Type, Value = sourceAttribute.Value };
		}
		string name = "";
		Type type;
		object itemValue;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemAttributeName"),
#endif
		DefaultValue(null)]
		public string Name {
			get { return name; }
			set {
				if(name == value)
					return;
				name = value;
				RaiseChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemAttributeType"),
#endif
		DefaultValue(null)]
		public Type Type {
			get { return type; }
			set {
				if(type == value)
					return;
				type = value;
				RaiseChanged();
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapItemAttributeValue"),
#endif
		DefaultValue(null)]
		public object Value {
			get { return itemValue; }
			set {
				if(itemValue == value)
					return;
				itemValue = value;
				RaiseChanged();
			}
		}
		#region ISupportObjectChanged implementation
		EventHandler onChanged;
		event EventHandler ISupportObjectChanged.Changed {
			add { onChanged += value; }
			remove { onChanged -= value; }
		}
		protected internal void RaiseChanged() {
			if(onChanged != null) onChanged(this, EventArgs.Empty);
		}
		#endregion
	}
	public class MapItemAttributeCollection : NamedItemNotificationCollection<MapItemAttribute> {
		protected override string GetItemName(MapItemAttribute item) {
			return item.Name;
		}
	}
}
