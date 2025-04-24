<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TryIt.aspx.cs" Inherits="WebAPIStuff.TryIt" Async="true" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TryIt Page</title>
    <link href="~/Content/bootstrap.css" rel="stylesheet" />
    <link href="~/Content/site.css" rel="stylesheet" />
    <script src="~/Scripts/modernizr-2.8.3.js"></script>
    <script src="~/Scripts/jquery-3.6.0.js"></script>
    <script src="~/Scripts/bootstrap.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2 class="mt-4">Application and Components Summary Table</h2>
            <asp:Table ID="ComponentTable" runat="server" CssClass="table table-bordered table-striped">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell>Component Name</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Description</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Provider</asp:TableHeaderCell>
                    <asp:TableHeaderCell>TryIt Function</asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableCell>PdfUploadHandler</asp:TableCell>
                    <asp:TableCell>Validates and processes PDF file uploads</asp:TableCell>
                    <asp:TableCell>Hrisika Jagdeep</asp:TableCell>
                    <asp:TableCell>
                        <asp:FileUpload ID="PdfUpload" runat="server" CssClass="form-control-file" />
                        <asp:Button ID="TestUpload" runat="server" Text="Test Upload" CssClass="btn btn-primary mt-2" OnClick="TestUpload_Click" />
                        <br />
                        <asp:Label ID="UploadResult" runat="server" CssClass="mt-2" ForeColor="Green" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>PdfToTextService</asp:TableCell>
                    <asp:TableCell>Extracts text from PDFs as chunks or formatted text</asp:TableCell>
                    <asp:TableCell>Hrisika Jagdeep</asp:TableCell>
                    <asp:TableCell>
                        <asp:FileUpload ID="PdfServiceUpload" runat="server" CssClass="form-control-file" />
                        <asp:CheckBox ID="IncludeFormatting" runat="server" Text="Include Formatting" CssClass="form-check-input mt-2" />
                        <asp:Button ID="TestService" runat="server" Text="Test Service" CssClass="btn btn-primary mt-2" OnClick="TestService_Click" />
                        <br />
                        <asp:Label ID="ServiceResult" runat="server" CssClass="mt-2" ForeColor="Green" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>
    </form>
</body>
</html>