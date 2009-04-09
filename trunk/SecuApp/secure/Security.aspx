<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Security.aspx.cs" Inherits="org.bsodhi.SecuApp.secure.Security" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Security Setup</title>
    <link rel="Stylesheet" href="../Stylesheet1.css" />
    <style type="text/css">
        td
        {
            vertical-align: top;
        }
        .label
        {
            text-align: right;
        }
        .val
        {
            text-align: left;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div class="header">
            <table width="100%">
                <tr>
                    <td class="logoCell">
                        User and Roles Management
                    </td>
                    <td class="userStatusCell">
                        Welcome,
                        <asp:LoginName ID="LoginName1" runat="server" />
                        &nbsp;|
                        <asp:LoginStatus ID="LoginStatus1" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="content">
            <br />
            <asp:LinkButton ID="AppSelectLink" runat="server" Text="Select Application" OnClick="AppSelectLink_Click" />&nbsp;|
            <asp:LinkButton ID="TaskSelectLink" runat="server" Text="Select Task" OnClick="TaskSelectLink_Click" />
            <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <!-- Index 0 -->
                <asp:View ID="SelectAppView" runat="server">
                    <table>
                        <tr>
                            <td class="label">
                                Select Existing Application:
                            </td>
                            <td class="val">
                                <asp:DropDownList ID="AppName" runat="server" DataSourceID="SqlDataSource1" DataTextField="ApplicationName"
                                    DataValueField="ApplicationId">
                                </asp:DropDownList>
                            </td>
                            <td class="label">
                                Or Enter New:
                            </td>
                            <td class="val">
                                <asp:TextBox ID="AppNameNew" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Button ID="AppSelectBtn" runat="server" Text="Go" OnClick="AppSelectBtn_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:View>
                <!-- Index 1 -->
                <asp:View ID="TaskSelectionView" runat="server">
                    <table>
                        <tr>
                            <td class="label">
                                Select Task:
                            </td>
                            <td>
                                <asp:DropDownList ID="TaskSelection" runat="server">
                                    <asp:ListItem>--Select--</asp:ListItem>
                                    <asp:ListItem>Create User</asp:ListItem>
                                    <asp:ListItem>Create Role</asp:ListItem>
                                    <asp:ListItem>Assign Roles</asp:ListItem>
                                    <asp:ListItem>Delete Roles</asp:ListItem>
                                    <asp:ListItem>Modify User</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Button ID="TaskSelectBtn" runat="server" Text="Go" OnClick="TaskSelectBtn_Click" />
                            </td>
                            <td style="margin-left: 80px">
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </asp:View>
                <!-- Index 2 -->
                <asp:View ID="CreateUserView" runat="server">
                    <table>
                        <tr>
                            <td class="label">
                                User ID:
                            </td>
                            <td>
                                <asp:TextBox ID="UserId" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                Email:
                            </td>
                            <td>
                                <asp:TextBox ID="Email" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </asp:View>
                <!-- Index 3 -->
                <asp:View ID="CreateRoleView" runat="server">
                    <table>
                        <tr>
                            <td class="label">
                                Role Name:
                            </td>
                            <td>
                                <asp:TextBox ID="RoleName" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </asp:View>
                <!-- Index 4 -->
                <asp:View ID="AssignRolesView" runat="server">
                    <table>
                        <tr>
                            <td class="label">
                                Select User ID:
                            </td>
                            <td>
                                <asp:DropDownList ID="Assignee" runat="server" DataSourceID="AssigneeDS" DataTextField="UserName"
                                    DataValueField="UserId" AutoPostBack="True" OnSelectedIndexChanged="Assignee_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                Roles:
                            </td>
                            <td>
                                <asp:CheckBoxList ID="AssignedRoles" runat="server" DataSourceID="RolesDS" DataTextField="RoleName"
                                    DataValueField="RoleName">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                    </table>
                </asp:View>
                <!-- Index 5 -->
                <asp:View ID="DeleteRolesView" runat="server">
                    <table>
                        <tr>
                            <td class="label">
                                Select Roles To Be Deleted:
                            </td>
                            <td>
                                <asp:CheckBoxList ID="RolesToBeDeleted" runat="server" DataSourceID="RolesDS" DataTextField="RoleName"
                                    DataValueField="RoleName">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                    </table>
                </asp:View>
                <!-- Index 6 -->
                <asp:View ID="ManageUserView" runat="server">
                    <table>
                        <tr>
                            <td class="label">
                                Select User:
                            </td>
                            <td>
                                <asp:DropDownList ID="UserToModify" runat="server" DataSourceID="AssigneeDS" DataTextField="UserName"
                                    DataValueField="UserName">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                Select Action:
                            </td>
                            <td>
                                <asp:RadioButtonList ID="UserModifyAction" runat="server" RepeatDirection="Horizontal"
                                    RepeatLayout="Flow">
                                    <asp:ListItem Text="Lock" Value="Lock" />
                                    <asp:ListItem Text="Unlock" Value="Unlock" />
                                    <asp:ListItem Text="Reset Password" Value="Reset Password" />
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                    </table>
                </asp:View>
            </asp:MultiView>
            <br />
            <asp:Panel ID="ButtonPanel" runat="server">
                [<asp:LinkButton ID="CancelBtn" runat="server" OnClick="CancelBtn_Click">Cancel</asp:LinkButton>]
                [
                <asp:LinkButton ID="SubmitBtn" runat="server" OnClick="SubmitBtn_Click">Submit</asp:LinkButton>]
            </asp:Panel>
            <br />
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:aspnetdbConnectionString %>"
                SelectCommand="SELECT [ApplicationName], [ApplicationId] FROM [vw_aspnet_Applications]">
            </asp:SqlDataSource>
            <asp:SqlDataSource ID="AssigneeDS" runat="server" ConnectionString="<%$ ConnectionStrings:aspnetdbConnectionString %>">
            </asp:SqlDataSource>
            <asp:SqlDataSource ID="RolesDS" runat="server" ConnectionString="<%$ ConnectionStrings:aspnetdbConnectionString %>"
                SelectCommand="SELECT [RoleName] FROM [vw_aspnet_Roles] WHERE ([ApplicationId] = @ApplicationId)">
                <SelectParameters>
                    <asp:ControlParameter ControlID="AppName" Name="ApplicationId" PropertyName="SelectedValue"
                        Type="Object" />
                </SelectParameters>
            </asp:SqlDataSource>
        </div>
        <div class="appStatus">
            <table width="100%">
                <tr>
                    <td style="width: 30%; text-align: left; font-weight:bold;">
                        Selected application:
                        <asp:Label ID="SelectedAppName" runat="server" />
                    </td>
                    <td style="text-align: right;">
                        <asp:Label ID="StatusMsg" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
