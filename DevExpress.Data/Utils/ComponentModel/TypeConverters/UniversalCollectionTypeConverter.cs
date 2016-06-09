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
using System.Text;
using System.ComponentModel;
using System.Collections;
namespace DevExpress.Utils.Design {
	public class UniversalCollectionTypeConverter : TypeConverter {
		public UniversalCollectionTypeConverter() {
		}
		protected string GetObjectCaption(object obj) {
			if(obj == null) return "<null>";
			ICaptionSupport caption = obj as ICaptionSupport;
			if(caption != null) return caption.Caption;
			PropertyDescriptor pd = TypeDescriptor.GetProperties(obj)["Caption"];
			if(pd != null) return pd.GetValue(obj).ToString();
			return obj.ToString();
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return true; }
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			IList list = value as IList;
			if(list != null && list.Count > 0) {
				PropertyDescriptor[] pArray = new PropertyDescriptor[list.Count];
				for(int n = 0; n < list.Count; n++) {
					object obj = list[n];
					string caption = GetObjectCaption(obj);
					string text = string.Format(list.Count > 9 ? "{0:d2}" : "{0}", n);
					if(caption != null && caption.Length > 0)
						text += " - " + caption;
					pArray[n] = new UniversalCollectionPropertyDescriptor(obj, text);
				}
				return new PropertyDescriptorCollection(pArray);
			}
			return null;
		}
	}
	public class UniversalCollectionPropertyDescriptor : PropertyDescriptor {
		object source;
		string caption;
		public UniversalCollectionPropertyDescriptor(object source, string caption)
			: base(caption, new Attribute[] { new NotifyParentPropertyAttribute(true) }) {
			this.source = source;
			this.caption = caption;
		}
		protected object Source { get { return source; } }
		protected string Caption { get { return caption; } }
		public override bool IsReadOnly { get { return false; } }
		public override string Category { get { return "Item"; } }
		public override Type PropertyType { get { return Source.GetType(); } }
		public override Type ComponentType { get { return Source.GetType(); } }
		public override void ResetValue(object component) { }
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) { return Source; }
		public override void SetValue(object component, object value) { }
		public override bool ShouldSerializeValue(object component) { return false; }
	}   
}
