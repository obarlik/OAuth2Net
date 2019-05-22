<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="True" CodeBehind="default.aspx.cs" Inherits="OAuth2Net.Demo._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table style="float:left; margin-right:20px">
                <thead>
                    <tr>
                        <th>Sign in with...</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>
                            <asp:LinkButton ID="LinkedInLoginBtn" runat="server" OnClick="LinkedInLoginBtn_Click">LinkedIn</asp:LinkButton></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:LinkButton ID="GithubLoginBtn" runat="server" OnClick="GithubLoginBtn_Click">GitHub</asp:LinkButton></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:LinkButton ID="FacebookLoginBtn" runat="server" OnClick="FacebookLoginBtn_Click">Facebook</asp:LinkButton></td>
                    </tr>
                </tbody>
            </table>
            <table>
                <tr>
                    <td rowspan="5">
                        <asp:Image ID="PersonPicture" runat="server" Width="100px" Height="100px" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" /></td>
                    <td>ID</td>
                    <td>
                        <asp:Label ID="PersonId" runat="server" Text=""></asp:Label></td>
                </tr>
                <tr>
                    <td>Name</td>
                    <td>
                        <asp:Label ID="PersonName" runat="server" Text=""></asp:Label></td>
                </tr>
                <tr>
                    <td>EMail</td>
                    <td>
                        <asp:Label ID="PersonEmail" runat="server" Text=""></asp:Label></td>
                </tr>
            </table>

        </div>
    </form>
</body>
</html>
