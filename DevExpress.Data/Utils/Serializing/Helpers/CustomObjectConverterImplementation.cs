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
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.Utils.Serializing.Helpers {
	class OneTypeCustomObjectConverter : IOneTypeObjectConverter {
		Type type;
		ICustomObjectConverter customObjectConverter;
		public OneTypeCustomObjectConverter(Type type, ICustomObjectConverter customObjectConverter) {
			this.type = type;
			this.customObjectConverter = customObjectConverter;
		}
		#region IOneTypeObjectConverter Members
		public Type Type {
			get { return type; }
		}
		public string ToString(object obj) {
			return customObjectConverter.ToString(Type, obj);
		}
		public object FromString(string str) {
			return customObjectConverter.FromString(Type, str);
		}
		#endregion
	}
	class CustomObjectConverters : ObjectConverters {
		ICustomObjectConverter customConverter;
		public CustomObjectConverters(ICustomObjectConverter customConverter) {
			this.customConverter = customConverter;
		}
		protected ICustomObjectConverter CustomConverter {
			get { return customConverter; }
		}
		public override bool IsConverterExists(Type type) {
			if(CustomConverter.CanConvert(type))
				return true;
			return base.IsConverterExists(type);
		}
		public IOneTypeObjectConverter GetCustomConverter(Type type) {
			if(CustomConverter.CanConvert(type))
				return new OneTypeCustomObjectConverter(type, CustomConverter);
			return null;
		}
		public override IOneTypeObjectConverter GetConverter(Type type) {
			IOneTypeObjectConverter customConverter = GetCustomConverter(type);
			if(customConverter != null)
				return customConverter;
			if(base.IsConverterExists(type))
				return Converters[type];
			return null;
		}
	}
	class CustomObjectConverterImplementation : ObjectConverterImplementation {
		ICustomObjectConverter customConverter;
		CustomObjectConverters customConverters;
		public CustomObjectConverterImplementation(ICustomObjectConverter customConverter) {
			this.customConverter = customConverter;
			this.customConverters = new CustomObjectConverters(customConverter);
			ObjectConverter.Instance.CopyConvertersTo(this);
		}
		protected override ObjectConverters Converters {
			get { return customConverters; }
		}
	}
}
