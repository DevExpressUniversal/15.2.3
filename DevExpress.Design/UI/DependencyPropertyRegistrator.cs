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

namespace DevExpress.Design.UI {
	using System.Windows;
	sealed class DependencyPropertyRegistrator<OwnerType> where OwnerType : class {
		public void Register<T>(string name, ref DependencyProperty property, T defValue) {
			Register<T>(name, ref property, defValue, null);
		}
		public void Register<T>(string name, ref DependencyProperty property, T defValue, PropertyChangedCallback changed) {
			PropertyMetadata metadata = (changed == null) ?
				new PropertyMetadata(defValue) : new PropertyMetadata(defValue, changed);
			property = DependencyProperty.Register(name, typeof(T), typeof(OwnerType), metadata);
		}
		public void RegisterAttached<T>(string name, ref DependencyProperty property, T defValue) {
			RegisterAttached<T>(name, ref property, defValue, null);
		}
		public void RegisterAttached<T>(string name, ref DependencyProperty property, T defValue, PropertyChangedCallback changed) {
			PropertyMetadata metadata = (changed == null) ?
				new PropertyMetadata(defValue) : new PropertyMetadata(defValue, changed);
			property = DependencyProperty.RegisterAttached(name, typeof(T), typeof(OwnerType), metadata);
		}
		public void OverrideDefaultStyleKey(DependencyProperty propertyKey) {
			propertyKey.OverrideMetadata(typeof(OwnerType), new FrameworkPropertyMetadata(typeof(OwnerType)));
		}
	}
}
