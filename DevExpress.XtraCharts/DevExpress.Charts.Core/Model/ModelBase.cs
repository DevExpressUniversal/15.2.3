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

using DevExpress.Charts.Native;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Charts.Model {
	public enum DataMemberType { Argument, Value, ExtValue0, ExtValue1, ExtValue2  }
	public enum DirectionMode {
		Counterclockwise,
		Clockwise
	}
	public enum CircularDiagramStyle {
		Circle,
		Polygon
	}
	public enum ArgumentScaleType {
		Qualitative = Scale.Qualitative,
		Numerical = Scale.Numerical,
		DateTime = Scale.DateTime,
		Auto = Scale.Auto
	}
	public enum ValueScaleType {
		Numerical = Scale.Numerical,
		DateTime = Scale.DateTime
	}
	public abstract class ModelElement  {
		ModelElement parent;
		public ModelElement Parent { 
			get { return parent; } 
			set {
				if(Object.Equals(parent, value))
					return;
				this.parent = value;
			} 
		}
		protected ModelElement() { 
		}
		protected ModelElement(ModelElement parent) {
			this.parent = parent;
		}
		protected void UpdateElementParent(ModelElement element, ModelElement parent) {
			if(element != null) element.Parent = parent;
		} 
		protected internal virtual void NotifyParent(ModelElement element, string propertyName, object value) {
			if(Parent != null) Parent.NotifyParent(element, propertyName, value);
		}
	}
	public abstract class ModelElementCollection<T> : DXCollection<T> where T : ModelElement {
		ModelElement parent;
		protected ModelElement Parent { get { return parent; } }
		protected ModelElementCollection(ModelElement parent) {
			this.parent = parent;
		}
		protected override void OnInsertComplete(int index, T value) {
			base.OnInsertComplete(index, value);
			value.Parent = parent;
		}
		protected override void OnRemoveComplete(int index, T value) {
			base.OnRemoveComplete(index, value);
			value.Parent= null;
		}
		protected override void OnSetComplete(int index, T oldValue, T newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			oldValue.Parent = null;
			newValue.Parent = parent;
		}
		protected override bool OnClear() {
			for(int i = 0; i < Count; i++)
				this[i].Parent = null;
			return base.OnClear();
		}
	}
	public class UpdateInfo {
		public static readonly UpdateInfo Empty = new UpdateInfo();
		public bool Handled { get; set; }
		public ModelElement Element { get; private set; }
		public string PropertyName { get; private set; }
		public object Value { get; private set; }
		internal UpdateInfo() { 
		}
		public UpdateInfo(ModelElement element, string propertyName, object value) {
			Element = element;
			PropertyName = propertyName;
			Value = value;
		}
		public override string ToString() {
			string value = Value != null ? Value.ToString() : "(null)";
			return string.Format("Element='{0}' Property='{1}' Value='{2}'", Element.GetType().Name, PropertyName, value);
		}
	}
}
