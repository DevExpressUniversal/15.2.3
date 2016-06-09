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

using DevExpress.Utils.Serializing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	public abstract class PrintStyleWithResourceOptions : PrintStyleWithAppointmentHeight {
		protected internal PrintStyleWithResourceOptions(bool registerProperties, bool baseStyle)
			: base(registerProperties, baseStyle) {
		}
		static readonly object resourceOptionsProperty = new object();
		[XtraSerializableProperty(XtraSerializationVisibility.Content),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ResourceOptions ResourceOptions {
			get { return (ResourceOptions)GetPropertyValue(resourceOptionsProperty); }
			set {
				ResourceOptions resourceOptions = ResourceOptions;
				if (Object.ReferenceEquals(resourceOptions, value))
					return;
				SetPropertyValue(resourceOptionsProperty, value.Clone());
			}
		}
		bool ShouldSerializeResourceOptions() {
			ResourceOptions defaultResourcesOptions = new ResourceOptions();
			return !ResourceOptions.IsEqual(defaultResourcesOptions);
		}
		protected bool XtraShouldSerializeResourceOptions() {
			ResourceOptions defaultResourcesOptions = new ResourceOptions();
			return !ResourceOptions.IsEqual(defaultResourcesOptions);
		}
		void ResetResourceOptions() {
			ResourceOptions = new ResourceOptions();
		}
		protected internal override void RegisterProperties() {
			base.RegisterProperties();
			RegisterProperty(resourceOptionsProperty, new ResourceOptions());
		}
	}
}
