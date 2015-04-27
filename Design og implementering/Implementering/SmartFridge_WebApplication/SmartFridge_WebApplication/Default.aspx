<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SmartFridge_WebApplication.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
        <meta name="viewport" content="width=device-width" />
    <title>SmartFridge Web Application</title>
    <link href="Style.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <div id="wrapper">
        <header>

            <button onclick="history.go(-1);return true;" >
                <img src="Images/Backward.png" width="70px" class="controlButton"/>
            </button>
            <button onclick="history.go(+1);return true;">
                <img src="Images/Forward.png" width="70px" class="controlButton" />
            </button>
            <button onclick="window.location.href='Default.aspx'">
                <img src="Images/Home.png" width="70px" class="controlButton" />
            </button>
            <button class="controlButton">
                <img src="Images/Settings.png" width="70px" class="controlButton" />
            </button>
            <div id="HTitle">
                SmartFridge Web Application
            </div>

        </header>
        <div id="content">
            <button>
                <img src="Images/Fridge.png" id="Fridge" />
            </button>

            <button>
                <img src="Images/List.png" id="ShopList" />
            </button>
            <button>
                <img src="Images/Standard.png" id="StandardList" />
            </button>
        </div>
        <footer>
            Copyright &copy; 2015 SmartFridge
        </footer>
    </div>
    </div>
    </form>
</body>
</html>
