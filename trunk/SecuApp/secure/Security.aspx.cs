using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Security;
using System.Configuration.Provider;

namespace org.bsodhi.SecuApp.secure
{
    /// <summary>
    /// Codebehind class for the page.
    /// </summary>
    public partial class Security : System.Web.UI.Page
    {
        /// <summary>
        /// We initialize few session variables that track the selected
        /// application whose roles and users are being manipulated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ButtonPanel.Visible = false;
            if(!IsPostBack)
            {
                Session.Add("OrigAppName", Membership.ApplicationName);
                Session.Add("SelectedAppName", Membership.ApplicationName);
            }
            SelectedAppName.Text = ""+Session["SelectedAppName"];
        }
        /// <summary>
        /// Setup the application name based on the selections in wizard. 
        /// Depending on what application is selected, the roles and users
        /// dropdowns are populated from that application.
        /// </summary>
        private void SetupApp()
        {
            if (AppName.SelectedIndex != 0 && AppNameNew.Text.Trim().Length == 0)
            {
                Session.Add("SelectedAppName", AppName.SelectedItem.Text);
                InitSelectSQL();
            }
            else if (AppNameNew.Text.Trim().Length > 0)
            {
                Session.Add("SelectedAppName", AppNameNew.Text.Trim());
            }
            else
            {
                Session.Add("SelectedAppName", Membership.ApplicationName);
            }
        }
        /// <summary>
        /// Switching between this application and the one being manipulated.
        /// </summary>
        /// <param name="reset"></param>
        private void ChangeApp(bool reset)
        {
            Roles.ApplicationName = Membership.ApplicationName =
                ""+((reset) ? Session["OrigAppName"] : Session["SelectedAppName"]);
        }
        /// <summary>
        /// Reset the UI to initial application selection view.
        /// </summary>
        private void ResetWizard()
        {
            ChangeApp(true);
            MultiView1.ActiveViewIndex = 0;
        }
        /// <summary>
        /// Initialize the SQL for loading various dropdowns.
        /// </summary>
        public void InitSelectSQL()
        {
            AssigneeDS.SelectCommand = "SELECT UserId, UserName FROM vw_aspnet_Users u, vw_aspnet_Applications a WHERE a.ApplicationId = u.ApplicationId AND a.ApplicationName='" + Session["SelectedAppName"] + "'";
            RolesDS.SelectCommand = "SELECT RoleName FROM vw_aspnet_Roles u, vw_aspnet_Applications a WHERE a.ApplicationId = u.ApplicationId AND a.ApplicationName='" + Session["SelectedAppName"] + "'";
            AssigneeDS.Select(new DataSourceSelectArguments());
            RolesDS.Select(new DataSourceSelectArguments());
        }
        /// <summary>
        /// Create a new user in the selected application using Membership API.
        /// </summary>
        protected void CreateUser()
        {
            string passwd = Membership.GeneratePassword(5, 2);
            MembershipUser u = Membership.CreateUser(UserId.Text, passwd, Email.Text);
        }
        /// <summary>
        /// Handler for the submit button when we apply all the modifications.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ChangeApp(false);// Switch to the selected application
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
                        if(Assignee.SelectedIndex == 0)
                        {
                            StatusMsg.Text = "Please select a user!";
                            break;
                        }
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
                    ModifyUser();
                }
                else
                {
                    StatusMsg.Text = "Please select a task from the dropdown!";
                }
            }
            catch (ProviderException pex)
            {
                StatusMsg.Text = "Operation failed: "+pex.Message;
            }
            finally
            {
                ChangeApp(true);
            }
            MultiView1.ActiveViewIndex = 1;// Take the user back to task selection
        }
        /// <summary>
        /// Modify the selected user.
        /// </summary>
        private void ModifyUser()
        {
            if (UserToModify.SelectedIndex == 0)
            {
                StatusMsg.Text = "Please select a user!";
            }
            else
            {
                if (UserModifyAction.SelectedValue.Equals("Deactivate"))
                {
                    Membership.GetUser(UserToModify.SelectedItem.Text).IsApproved = false;
                }
                else if (UserModifyAction.SelectedValue.Equals("Activate"))
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
                    StatusMsg.Text = "Please select appropriate action to perform on the user!";
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Assignee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(Assignee.SelectedIndex == 0) return;
            try
            {
                ChangeApp(false);
                string[] currRoles = Roles.GetRolesForUser(Assignee.SelectedItem.Text);
                foreach (ListItem it in AssignedRoles.Items)
                {
                    it.Selected = false;
                    foreach (string r in currRoles)
                    {
                        it.Selected = it.Text.Trim().ToUpper().Equals(r.Trim().ToUpper());
                        if (it.Selected)
                        {
                            break;
                        }
                    }
                }
                ButtonPanel.Visible = true;
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
            if (AppName.SelectedIndex == 0 && AppNameNew.Text.Trim().Length == 0)
            {
                StatusMsg.Text = "Please select an application!";
            }
            else
            {
                SetupApp();
                MultiView1.ActiveViewIndex = 1; // Show the task selection view
                SelectedAppName.Text = "" + Session["SelectedAppName"];
            }
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
                if (TaskSelection.SelectedIndex == 0)
                {
                    StatusMsg.Text = "Please select a task!";
                }
                else
                {
                    ChangeApp(false);
                    InitSelectSQL();
                    MultiView1.ActiveViewIndex = TaskSelection.SelectedIndex + 1;
                    StatusMsg.Text = "Selectd task: " + TaskSelection.SelectedItem.Text;
                    ButtonPanel.Visible = true;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AppName_DataBound(object sender, EventArgs e)
        {
            AppName.Items.Insert(0, "--Select--");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Assignee_DataBound(object sender, EventArgs e)
        {
            Assignee.Items.Insert(0, "--Select--");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UserToModify_DataBound(object sender, EventArgs e)
        {
            UserToModify.Items.Insert(0, "--Select--");
        }

    }
}
