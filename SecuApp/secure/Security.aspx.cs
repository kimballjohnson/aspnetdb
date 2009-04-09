/*
This software is provided under GPL v3 license http://www.gnu.org/licenses/gpl-3.0.txt.
*/
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Security;

namespace org.bsodhi.SecuApp.secure
{
    public partial class Security : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                Session.Add("OrigAppName", Membership.ApplicationName);
                Session.Add("SelectedAppName", Membership.ApplicationName);
            }
            SelectedAppName.Text = ""+Session["SelectedAppName"];
        }
        /// <summary>
        /// 
        /// </summary>
        private void SetupForAction()
        {
            if (AppName.SelectedIndex != -1 && AppNameNew.Text.Trim().Length == 0)
            {
                Session.Add("SelectedAppName", AppName.SelectedItem.Text);
                InitSelectSQL();
            }
            else if (AppNameNew.Text.Trim().Length > 0)
            {
                Session.Add("SelectedAppName", AppNameNew.Text.Trim());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reset"></param>
        private void ChangeApp(bool reset)
        {
            Roles.ApplicationName = Membership.ApplicationName =
                ""+((reset) ? Session["OrigAppName"] : Session["SelectedAppName"]);
        }
        /// <summary>
        /// 
        /// </summary>
        private void ResetWizard()
        {
            ChangeApp(true);
            MultiView1.ActiveViewIndex = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        public void InitSelectSQL()
        {
            AssigneeDS.SelectCommand = "SELECT UserId, UserName FROM vw_aspnet_Users u, vw_aspnet_Applications a WHERE a.ApplicationId = u.ApplicationId AND a.ApplicationName='" + Session["SelectedAppName"] + "'";
            RolesDS.SelectCommand = "SELECT RoleName FROM vw_aspnet_Roles u, vw_aspnet_Applications a WHERE a.ApplicationId = u.ApplicationId AND a.ApplicationName='" + Session["SelectedAppName"] + "'";
            AssigneeDS.Select(new DataSourceSelectArguments());
            RolesDS.Select(new DataSourceSelectArguments());
        }
        /// <summary>
        /// 
        /// </summary>
        protected void CreateUser()
        {
            string passwd = Membership.GeneratePassword(5, 2);
            MembershipUser u = Membership.CreateUser(UserId.Text, passwd, Email.Text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string[] GetSelectedRoles(ListBox list)
        {
            int[] selInds = list.GetSelectedIndices();
            string[] myRoles = new string[selInds.Length];
            for (int ind = 0; ind < myRoles.Length; ind++)
            {
                myRoles[ind] = list.Items[ind].Value;
            }
            return myRoles;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ChangeApp(false);
                if (TaskSelection.SelectedValue.Equals("Create User"))
                {
                    CreateUser();
                }
                else if (TaskSelection.SelectedValue.Equals("Create Role"))
                {
                    Roles.CreateRole(RoleName.Text.Trim());
                }
                else if (TaskSelection.SelectedValue.Equals("Assign Roles"))
                {
                    foreach (ListItem itm in AssignedRoles.Items)
                    {
                        bool inRole = Roles.IsUserInRole(Assignee.SelectedItem.Text, itm.Value);
                        if (itm.Selected && !inRole)
                        {
                            Roles.AddUserToRole(Assignee.SelectedItem.Text, itm.Value);
                        }
                        else if (!itm.Selected && inRole)
                        {
                            Roles.RemoveUserFromRole(Assignee.SelectedItem.Text, itm.Value);
                        }
                    }                    
                }
                else if (TaskSelection.SelectedValue.Equals("Delete Roles"))
                {
                    foreach (ListItem item in RolesToBeDeleted.Items)
                    {
                        if (item.Selected) Roles.DeleteRole(item.Value, true);
                    }
                    InitSelectSQL();
                }
                else if (TaskSelection.SelectedValue.Equals("Modify User"))
                {
                    if (UserModifyAction.SelectedValue.Equals("Lock"))
                    {
                        Membership.GetUser(UserToModify.SelectedItem.Text).IsApproved = false;
                    }
                    else if (UserModifyAction.SelectedValue.Equals("Unlock"))
                    {
                        Membership.GetUser(UserToModify.SelectedItem.Text).UnlockUser();
                        Membership.GetUser(UserToModify.SelectedItem.Text).IsApproved = true;
                    }
                    else if (UserModifyAction.SelectedValue.Equals("Reset Password"))
                    {
                        string newPasswd = Membership.GetUser(UserToModify.SelectedItem.Text).ResetPassword();
                    }
                    else
                    {
                        StatusMsg.Text = "\nPlease select appropriate action to perform on the user!";
                    }
                }
                else
                {
                    StatusMsg.Text = "\nPlease select a task from the dropdown!";
                }
            }
            finally
            {
                ChangeApp(true);
            }
            MultiView1.ActiveViewIndex = 1;// Take the user back to task selection
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Assignee_SelectedIndexChanged(object sender, EventArgs e)
        {
            StatusMsg.Text = "\nSelected user: " + Assignee.SelectedItem.Text + ". Roles: ";
            try
            {
                ChangeApp(false);
                string[] currRoles = Roles.GetRolesForUser(Assignee.SelectedItem.Text);
                foreach (ListItem it in AssignedRoles.Items)
                {
                    foreach (string r in currRoles)
                    {
                        it.Selected = it.Text.Trim().ToUpper().Equals(r.Trim().ToUpper());
                        if (it.Selected)
                        {
                            StatusMsg.Text += (r + ", ");
                            break;
                        }
                    }
                }
            }
            finally
            {
                ChangeApp(true);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AppSelectBtn_Click(object sender, EventArgs e)
        {
            SetupForAction();
            MultiView1.ActiveViewIndex = 1; // Show the task selection view
            SelectedAppName.Text = ""+Session["SelectedAppName"];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TaskSelectBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ChangeApp(false);
                InitSelectSQL();
                MultiView1.ActiveViewIndex = TaskSelection.SelectedIndex + 1;
                StatusMsg.Text += "\nSelectd task: " + TaskSelection.SelectedItem.Text;
            }
            finally
            {
                ChangeApp(true);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelBtn_Click(object sender, EventArgs e)
        {
            ResetWizard();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AppSelectLink_Click(object sender, EventArgs e)
        {
            MultiView1.ActiveViewIndex = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TaskSelectLink_Click(object sender, EventArgs e)
        {
            MultiView1.ActiveViewIndex = 1;
        }
    
    }
}
