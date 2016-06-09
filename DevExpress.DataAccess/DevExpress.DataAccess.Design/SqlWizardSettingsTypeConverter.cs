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
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DataAccess.Design {
	public class SqlWizardSettingsTypeConverter : ExpandableObjectConverter {
		class EnableCustomSqlPropertyDescriptor : PropertyDescriptor {
			readonly PropertyDescriptor parentPropertyDescriptor;
			readonly IDesignerHost designerHost;
			readonly IWin32Window owner;
			public EnableCustomSqlPropertyDescriptor(PropertyDescriptor parentProperty, IDesignerHost designerHost)
				: base(parentProperty) {
					this.parentPropertyDescriptor = parentProperty;
				this.designerHost = designerHost;
				IUIService uiService = designerHost.GetService<IUIService>();
				owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			}
			public override bool CanResetValue(object component) {
				return parentPropertyDescriptor.CanResetValue(component);
			}
			public override object GetValue(object component) {
				return parentPropertyDescriptor.GetValue(component);
			}
			public override void ResetValue(object component) {
				parentPropertyDescriptor.ResetValue(component);
			}
			public override void SetValue(object component, object value) {
				if ((bool)value) {
					using (var lookAndFeel = VSLookAndFeelHelper.GetLookAndFeel(designerHost))
						if (XtraMessageBox.Show(lookAndFeel, owner, "The use of custom SQL queries may lead to security vulnerabilities.\r\nAre you sure to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
							parentPropertyDescriptor.SetValue(component, value);
				}
				else
					parentPropertyDescriptor.SetValue(component, value);
			}
			public override bool ShouldSerializeValue(object component) {
				return parentPropertyDescriptor.ShouldSerializeValue(component);
			}
			public override Type ComponentType {
				get { return parentPropertyDescriptor.ComponentType; }
			}
			public override bool IsReadOnly {
				get { return parentPropertyDescriptor.IsReadOnly; }
			}
			public override Type PropertyType {
				get { return parentPropertyDescriptor.PropertyType; }
			}
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			var properties = base.GetProperties(context, value, attributes);
			var designerHost = context.GetService<IDesignerHost>();
			var enableCustomSqlPropertyDescriptor = new EnableCustomSqlPropertyDescriptor(properties["EnableCustomSql"], designerHost);
			PropertyDescriptor[] propsArray = new PropertyDescriptor[properties.Count];
			for(int i =0; i < properties.Count; i++) {
				if(properties[i].Name == "EnableCustomSql")
					propsArray[i] = enableCustomSqlPropertyDescriptor;
				else {
					propsArray[i] = properties[i];
				}
			}
			return new PropertyDescriptorCollection(propsArray);
		}
	}
}
