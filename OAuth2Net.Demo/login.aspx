<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="OAuth2Net.Demo.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                <thead>
                    <tr><th>Sign in with...</th></tr>
                </thead>
                <tbody>
                    <tr><td><asp:LinkButton ID="LinkedInLoginBtn" runat="server" OnClick="LinkedInLoginBtn_Click">LinkedIn</asp:LinkButton></td></tr>
                    <tr><td><asp:LinkButton ID="GithubLoginBtn" runat="server" OnClick="GithubLoginBtn_Click">GitHub</asp:LinkButton></td></tr>
                    <tr><td><asp:LinkButton ID="FacebookLoginBtn" runat="server" OnClick="FacebookLoginBtn_Click">Facebook</asp:LinkButton></td></tr>
                </tbody>
            </table>            
        </div>
    </form>
</body>
</html>
