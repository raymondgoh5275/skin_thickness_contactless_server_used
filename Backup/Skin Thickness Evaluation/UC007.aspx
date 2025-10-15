<%@ Page Title="Rework Analysis Filtering" Language="C#" MasterPageFile="~/SiteSub.Master" AutoEventWireup="true" CodeBehind="UC007.aspx.cs" Inherits="Skin_Thickness_Evaluation.UC095" %>

<%@ Register TagPrefix="STE" TagName="ReworkAnalysis" Src="~/Components/UC_ReworkAnalysis.ascx" %>
<%@ Register TagPrefix="filter" TagName="BladeList" Src="~/Components/Filter/Filter_BladeList.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="LeftBar" runat="server">
    <h3>
        <a href="#">Export Options</a></h3>
    <div>
        <filter:BladeList ID="BladeList1" runat="server" MultiSelect="false" />
        <div class="clear"></div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <STE:ReworkAnalysis ID="ReworkAnalysis01" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
    <script type="text/javascript">
        function OnClick_Export() {
            var BladeID = $('#BladeList option:selected').val();

            if (BladeID == '') {
                alert('you must select a blade.');
                return false;
            }

            RequestImagesOut(BladeID);
            RequestImagesIn(BladeID);

            RequestBladeInfo(SelectedProfileText, $('#BladeList option:selected').text());
            return false;
        }
    </script>
    <a href="#" class="button" onclick="OnClick_Export();">Export</a>
</asp:Content>