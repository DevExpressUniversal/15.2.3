using System;
using System.Web.UI;
using DevExpress.XtraScheduler;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.ASPxScheduler.Internal;
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Web.ASPxScheduler.Localization;
using DevExpress.Utils;
using System.Web.UI.WebControls;

public partial class SchedulerMessageBox : SchedulerMessageBoxBase {
    public override string ClassName { get { return "ASPxSchedulerMessageBox"; } }
      
    protected override void OnLoad(EventArgs e) {
        base.OnLoad(e);
        Localize();
    }
    void Localize() {
        btnOk.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonOk);
        btnCancel.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonCancel);
        btnOk.Wrap = DefaultBoolean.False;
        btnCancel.Wrap = DefaultBoolean.False;
    }
    protected override ASPxEditBase[] GetChildEditors() {
        return new ASPxEditBase[] { };
    }
    protected override ASPxButton[] GetChildButtons() {
        ASPxButton[] buttons = new ASPxButton[] {
            btnOk, btnCancel
        };
        return buttons;
    }
    protected override Control[] GetChildControls() {
        return new Control[] { root, lblMessage };
    }
    protected override WebControl GetDefaultButton() {
        return btnOk;
    }
}
