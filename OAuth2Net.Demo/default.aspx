<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="OAuth2Net.Demo._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                <tr>
                    <td rowspan="5"><asp:Image ID="PersonPicture" runat="server" /></td>
                    <td>ID</td><td><asp:Label ID="PersonId" runat="server" Text=""></asp:Label></td>
                </tr>                
                <tr>
                    <td>Name</td><td><asp:Label ID="PersonName" runat="server" Text=""></asp:Label></td>                    
                </tr>                
                <tr>
                    <td>EMail</td><td><asp:Label ID="PersonEmail" runat="server" Text=""></asp:Label></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
