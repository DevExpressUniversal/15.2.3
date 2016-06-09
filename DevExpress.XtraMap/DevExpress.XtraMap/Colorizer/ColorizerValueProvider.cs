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
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using DevExpress.Map.Native;
namespace DevExpress.XtraMap {
	public abstract class NamedAttributeProviderBase : ISupportObjectChanged {
		string attributeName = string.Empty;
		#region ISupportObjectChanged Members
		EventHandler onChanged;
		event EventHandler ISupportObjectChanged.Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected virtual void RaiseChanged() {
			if(onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		[
		DefaultValue(""), Category(SRCategoryNames.Data)]
		public string AttributeName {
			get { return attributeName; }
			set {
				if(attributeName == value)
					return;
				attributeName = value;
				RaiseChanged();
			}
		}
		protected virtual object ExtractAttributeValue(IMapItemAttribute attribute) {
			object value = attribute.Value;
			IList valueArr = value as IList;
			return valueArr != null ? ConvertFromValueArray(valueArr) : value;
		}
		protected object ConvertFromValueArray(IList list) {
			return (list != null && list.Count > 0) ? list[0] : null;
		}
		protected object GetAttributeValue(IMapItemAttributeOwner owner) {
			if(owner != null && !string.IsNullOrEmpty(AttributeName)) {
				IMapItemAttribute attr = owner.GetAttribute(AttributeName);
				if(attr != null)
					return ExtractAttributeValue(attr);
			}
			return null;
		}
	}
	public abstract class ColorizerValueProviderBase : IColorizerValueProvider {
		#region ISupportObjectChanged Members
		EventHandler onChanged;
		event EventHandler ISupportObjectChanged.Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected virtual void RaiseChanged() {
			if(onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		double IColorizerValueProvider.GetValue(object item) {
			return GetValue(item as IColorizerElement);
		}
		protected abstract double GetValue(IColorizerElement item);
	}
	public class ShapeAttributeValueProvider : ColorizerValueProviderBase {
		string attributeName = string.Empty;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("ShapeAttributeValueProviderAttributeName"),
#endif
		DefaultValue("")]
		public string AttributeName {
			get { return attributeName; }
			set {
				if (attributeName == value)
					return;
				attributeName = value;
				RaiseChanged();
			}
		}
		protected override double GetValue(IColorizerElement item) {
			MapShape shapeItem = item as MapShape;
			if(shapeItem == null) return double.NaN;
			MapItemAttribute attr = shapeItem.Attributes[AttributeName];
			if(attr != null && attr.Value != null)
				return Convert.ToDouble(attr.Value);
			return double.NaN;
		}
		public override string ToString() {
			return "(ShapeAttributeValueProvider)";
		}
	}
	public class MapBubbleValueProvider : ColorizerValueProviderBase {
		protected override double GetValue(IColorizerElement item) {
			MapBubble bubble = item as MapBubble;
			if(bubble == null) return double.NaN;
			return bubble.Value;
		}
		public override string ToString() {
			return "(BubbleValueProvider)";
		}
	}
	public class MapClusterValueProvider : ColorizerValueProviderBase {
		protected override double GetValue(IColorizerElement item) {
			MapItem mapItem = item as MapItem;
			return mapItem == null || mapItem.ClusteredItems == null ? 0 : mapItem.ClusteredItems.Count;
		}
		public override string ToString() {
			return "(MapClusterValueProvider)";
		}
	}
	public class AttributeItemKeyProvider : NamedAttributeProviderBase, IColorizerItemKeyProvider {
		object IColorizerItemKeyProvider.GetItemKey(object item) {
			return GetAttributeValue(item as IMapItemAttributeOwner);
		}
		public override string ToString() {
			return "(AttributeItemKeyProvider)";
		}
	}
	public class ArgumentItemKeyProvider : IColorizerItemKeyProvider {
		object IColorizerItemKeyProvider.GetItemKey(object item) {
			IKeyColorizerElement keyElement = item as IKeyColorizerElement;
			return keyElement != null ? keyElement.ColorItemKey : null;
		}
		public override string ToString() {
			return "(ArgumentItemKeyProvider)";
		}
	}
}
