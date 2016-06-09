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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using DevExpress.XtraNavBar;
using DevExpress.XtraNavBar.ViewInfo;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using System.Windows.Forms;
namespace DevExpress.XtraNavBar.Design {
	public sealed class BaseViewInfoRegistratorTypeConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == typeof(InstanceDescriptor) && value is BaseViewInfoRegistrator) {
				ConstructorInfo ctor;
				ctor = value.GetType().GetConstructor(new Type[] {});
				if(ctor != null)
					return new InstanceDescriptor(ctor, null);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public sealed class StandardSkinViewInfoRegistratorTypeConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			DevExpress.Skins.ISkinProvider info = value as DevExpress.Skins.ISkinProvider;
			if(destinationType == typeof(InstanceDescriptor) && info != null) {
				ConstructorInfo ctor;
				ctor = value.GetType().GetConstructor(new Type[] {typeof(string)});
				if(ctor != null)
					return new InstanceDescriptor(ctor, new object[] { info.SkinName });
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class NavBarItemLinkTypeConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == typeof(InstanceDescriptor) && value is NavBarItemLink) {
				NavBarItemLink link = value as NavBarItemLink;
				ConstructorInfo ctor;
				ctor = value.GetType().GetConstructor(new Type[] {typeof(NavBarItem)});
				if(ctor != null)
					return new InstanceDescriptor(ctor, new object[] { link.Item} );
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class NavBarViewNamesConverter : TypeConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			NavBarControl navBar = context.Instance as NavBarControl;
			if(navBar == null) return null;
			NavBarControlDesigner.PopulateDesignTimeViews(navBar);
			ArrayList list = new ArrayList();
			list.Add(NavBarControl.DefaultPaintStyleName);
			foreach(BaseViewInfoRegistrator info in navBar.AvailableNavBarViews) {
				list.Add(info.ViewName);
			}
			return new StandardValuesCollection(list);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return context != null && context.Instance != null;
		}
	}
	public class CollapsedNavPaneContentControlTypeConverter : ComponentConverter {
		public CollapsedNavPaneContentControlTypeConverter()
			: base(typeof(Control)) {
		}
		protected override bool IsValueAllowed(ITypeDescriptorContext context, object value) {
			if(value is Form || value is NavBarControl || value is NavBarGroupControlContainer) return false;
			return base.IsValueAllowed(context, value);
		}
	}
}
