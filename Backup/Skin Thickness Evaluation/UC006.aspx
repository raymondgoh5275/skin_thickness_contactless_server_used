<%@ Page Title="Export Analysis Filtering" Language="C#" MasterPageFile="~/SiteSub.Master"
    AutoEventWireup="true" CodeBehind="UC006.aspx.cs" Inherits="Skin_Thickness_Evaluation.UC094" %>

<%@ Register TagPrefix="filter" TagName="BladeList" Src="~/Components/Filter/Filter_BladeList.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="LeftBar" runat="server">
    <h3>
        <a href="#">Export Options</a></h3>
    <div>
        <filter:BladeList runat="server" MultiSelect="false" />
        <fieldset class="ui-corner-all">
            <div>
                <input type="checkbox" id="PivotTable" name="PivotTable" value="true"/>
                <label for="PivotTable">Pivot the tabular results</label>
            </div>
            <!--div>
                <input type="checkbox" id="CheckBox3" name="CheckBox3" style="float: left;" />
                < label for="CheckBox3" style="display: block; padding-left: 20px;">
                    Apply conditional formatting to points which are less than
                    <input type="text" name="Text1" id="Text2" style="width: 30px;" value="0.15" />
                    above bottom limit
                </label>
            </div -->
        </fieldset>
        <div class="clear">
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        #thicknessData tbody td
        {
            text-align: center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
    <script type="text/javascript" language="javascript">

        function OnClick_Export_CSV() {
            var SelectedBlade = $('#BladeList option:selected').val();
            var PivotTable = $('#PivotTable').is(':checked');

            if (SelectedBlade == "") {
                alert("No Blade Selected.");
                return false;
            }
            window.open('API/CSV.aspx?M=EAF&id=' + SelectedBlade + '&p=' + PivotTable);
        }
    </script>
    <a href="#" class="button" onclick="OnClick_Export_CSV();">Export To CSV</a>
</asp:Content>
