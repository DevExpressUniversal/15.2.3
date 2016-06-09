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

namespace DevExpress.Utils.MVVM.Design {
	using System;
	using System.ComponentModel;
	using System.ComponentModel.Design.Serialization;
	using System.Globalization;
	using System.Reflection;
	public abstract class ServiceRegistrationExpressionConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) {
				RegistrationExpression re = value as RegistrationExpression;
				return new InstanceDescriptor(GetRegisterMethod(), re.GetSerializerParameters());
			}
			throw new NotSupportedException();
		}
		MethodInfo GetRegisterMethod() {
			return typeof(RegistrationExpression).GetMethod(GetRegisterMethodName());
		}
		protected abstract string GetRegisterMethodName();
	}
	public class DispatcherServiceRegistrationExpressionConverter : ServiceRegistrationExpressionConverter {
		protected override string GetRegisterMethodName() {
			return "RegisterDispatcherService";
		}
	}
	public class MessageBoxServiceRegistrationExpressionConverter : ServiceRegistrationExpressionConverter {
		protected override string GetRegisterMethodName() {
			return "RegisterMessageBoxService";
		}
	}
	public class DialogServiceRegistrationExpressionConverter : ServiceRegistrationExpressionConverter {
		protected override string GetRegisterMethodName() {
			return "RegisterDialogService";
		}
	}
	public class DocumentManagerServiceRegistrationExpressionConverter : ServiceRegistrationExpressionConverter {
		protected override string GetRegisterMethodName() {
			return "RegisterDocumentManagerService";
		}
	}
	public class WindowedDocumentManagerServiceRegistrationExpressionConverter : ServiceRegistrationExpressionConverter {
		protected override string GetRegisterMethodName() {
			return "RegisterWindowedDocumentManagerService";
		}
	}
	public class ConfirmationBehaviorRegistrationExpressionConverter : ServiceRegistrationExpressionConverter {
		protected override string GetRegisterMethodName() {
			return "RegisterConfirmation";
		}
	}
	public class EventToCommandBehaviorRegistrationExpressionConverter : ServiceRegistrationExpressionConverter {
		protected override string GetRegisterMethodName() {
			return "RegisterEventToCommand";
		}
	}
	public class EventToCommandBehaviorParameterizedRegistrationExpressionConverter : ServiceRegistrationExpressionConverter {
		protected override string GetRegisterMethodName() {
			return "RegisterEventToCommandParameterized";
		}
	}
	public class NotificationServiceRegistrationExpressionConverter : ServiceRegistrationExpressionConverter {
		protected override string GetRegisterMethodName() {
			return "RegisterNotificationService";
		}
	}
	public class SplashScreenServiceRegistrationExpressionConverter : ServiceRegistrationExpressionConverter {
		protected override string GetRegisterMethodName() {
			return "RegisterSplashScreenService";
		}
	}
	public class LayoutSerializationServiceRegistrationExpressionConverter : ServiceRegistrationExpressionConverter {
		protected override string GetRegisterMethodName() {
			return "RegisterLayoutSerializationService";
		}
	}
}
