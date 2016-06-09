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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.IO;
namespace DevExpress.XtraScheduler.Design {
	public partial class CustomAppointmentFormWizardForm : XtraForm {
		const string SRDefaultFormSubject = "Default";
		const string SROutlook2003FormSubject = "Outlook 2003 Style";
		const string SROutlook2007FormSubject = "Outlook 2007 Style";
		const string SROutlokkFromSubject = "Outlook Style";
		const string SRDefaultFormDescription = "Default layout is a simple form containing editors for the Subject, Location, Label, Start, End, AllDay, Resource, Status, Reminder and Description fields. It contains a button to invoke a Recurrence Dialog to edit the recurrence information.";
		const string SROutlook2003FormDescription = @"Outlook 2003 layout contains menu bar that provides commands that enables the end-user to save or delete an appointment, to check the spelling of the text fields and to invoke a Recurrence Dialog to edit the recurrence information.
The form contains editors for the Subject, Location, Label, Start, End, AllDay, Resource, Status, Reminder and Description fields.";
		const string SROutlook2007FormDescription = @"Outlook 2007 layout employs the Ribbon UI. It contains commands that enables the end-user to save or delete an appointment, to change the appointment's label or status, to check the spelling of the text fields and to invoke a Recurrence Dialog to edit the recurrence information.
The form contains editors for the Subject, Location, Start, End, AllDay, Resource, Reminder and Description fields.";
		const string SROutlookFormDescription = @"Outlook layout employs the Ribbon UI. It contains commands that enables the end-user to save or delete an appointment, to change the appointment's label or status or reminder and to invoke a Recurrence Dialog to edit the recurrence information.
The form contains editors for the Subject, Location, Start, End, AllDay, Resource and Description fields.";
		static Dictionary<CusomAppointmentFormType, FormInfo> formTypeDictionary;
		static CustomAppointmentFormWizardForm() {
			formTypeDictionary = new Dictionary<CusomAppointmentFormType, FormInfo>();
			RegisterFormType(CusomAppointmentFormType.OutlookStyle, SROutlokkFromSubject, SROutlookFormDescription, "OutlookAppointmentForm.png");
			RegisterFormType(CusomAppointmentFormType.Default, SRDefaultFormSubject, SRDefaultFormDescription, "AppointmentFormPreview.png");
		}
		static void RegisterFormType(CusomAppointmentFormType type, string subject, string description, string imageName) {
			FormInfo info = new FormInfo();
			info.ImageName = imageName;
			info.Subject = subject;
			info.Description = description;
			formTypeDictionary.Add(type, info);
		}
		static FormInfo GetFormInfo(CusomAppointmentFormType type) {
			return formTypeDictionary[type];
		}
		public CustomAppointmentFormWizardForm() {
			InitializeComponent();
			InitializeFormList();
			SubscribeEvents();
		}
		public CusomAppointmentFormType FormType { get { return (CusomAppointmentFormType)this.cbAppointmentForm.SelectedItem; } }
		void SubscribeEvents() {
			this.cbAppointmentForm.SelectedIndexChanged += OnCbAppointmentFormSelectedIndexChanged;
		}
		void InitializeFormList() {
			foreach (KeyValuePair<CusomAppointmentFormType, FormInfo> item in formTypeDictionary) {
				this.cbAppointmentForm.Properties.Items.Add(new ImageComboBoxItem(item.Value.Subject, item.Key));
			}
			this.cbAppointmentForm.SelectedItem = CusomAppointmentFormType.OutlookStyle;
			UpdatePreview();
		}
		void OnCbAppointmentFormSelectedIndexChanged(object sender, EventArgs e) {
			UpdatePreview();
		}
		void UpdatePreview() {
			if (DesignMode)
				return;
			FormInfo currentFormInfo = GetFormInfo(FormType);
			LoadImageFromResource(currentFormInfo.ImageName);
			this.lblDescription.Text = currentFormInfo.Description;
		}
		void LoadImageFromResource(string imageName) {
			string basePath = "DevExpress.XtraScheduler.Design.Images.";
			Stream stream = GetType().Assembly.GetManifestResourceStream(basePath + imageName);
			Image image = new Bitmap(stream);
			Image oldImage = this.pictureBox1.Image;
			this.pictureBox1.Image = image;
			if (oldImage != null)
				oldImage.Dispose();
		}
	}
	public class FormInfo {
		public string Subject { get; set; }
		public string ImageName { get; set; }
		public string Description { get; set; }
	}
}
